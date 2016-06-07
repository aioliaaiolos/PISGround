//---------------------------------------------------------------------------------------------------
// <copyright file="RequestProcessor.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2013.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Threading;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Train.LiveVideoControl;

namespace PIS.Ground.LiveVideoControl
{
	/// <summary>Request processor.</summary>
	public class RequestProcessor : IRequestProcessor
	{
		#region Constants

		/// <summary>Identifier for event subscription.</summary>
		private const string SubscriberId = "PIS.Ground.LiveVideoControl.RequestProcessor";

		#endregion

		/// <summary>The transmit event to wake the transmit thread up.</summary>
		public static AutoResetEvent _transmitEvent = new AutoResetEvent(false);

		/// <summary>Lock for thread safety.</summary>
		private static object _lock = new object();

		/// <summary>List of requests waiting for processing.</summary>
		private static List<RequestContext> _newRequests = new List<RequestContext>();

		/// <summary>The transmit thread. It process request transmission.</summary>
		private static Thread _transmitThread;

		/// <summary>The T2G manager to use local data store.</summary>
		private static IT2GManager _train2groundManager = null;

		/// <summary>
		/// Manager for session. Used for checkink input request id and resolving notification urls.
		/// </summary>
		private static ISessionManager _sessionManager = null;

		private static INotificationSender _notificationSender = null;

		/// <summary>
		/// Callback called when Element Online state changes (signaled by the T2G Client).
		/// </summary>
		/// <param name="sender">Source of the event.</param>
		/// <param name="args">Event information to send to registered event handlers.</param>
		public static void OnElementInfoChanged(object sender, ElementEventArgs args)
		{
			// Signal the event to start handling the request.
			_transmitEvent.Set();
		}

		/// <summary>Initializes a new instance of the RequestProcessor class.</summary>
		public RequestProcessor()
		{
			RequestProcessor._transmitThread = new Thread(new ThreadStart(RequestProcessor.OnTransmitEvent));
            RequestProcessor._transmitThread.Name = "LiveVideo Transmit";
			if (_transmitThread.ThreadState != ThreadState.Running)
			{
				_transmitThread.Start();
			}
		}

		/// <summary>Sets Train2Ground client.</summary>
		/// <param name="train2groundClient">The T2G client to use local data store.</param>
		public void SetT2GManager(IT2GManager train2groundManager)
		{
			this.SetT2GManager(train2groundManager, string.Empty);
		}

		/// <summary>Sets Train2Ground client.</summary>
		/// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
		/// illegal values.</exception>
		/// <exception cref="NullReferenceException">Thrown when a value was unexpectedly null.</exception>
		/// <param name="train2groundClient">The T2G client to use local data store.</param>
		/// <param name="applicationId">Identifier for the application.</param>
		public void SetT2GManager(IT2GManager train2groundManager, string applicationId)
		{
			if (train2groundManager != null)
			{
				RequestProcessor._train2groundManager = train2groundManager;
				if (applicationId == string.Empty)
				{
					RequestProcessor._train2groundManager.SubscribeToElementChangeNotification(SubscriberId, new EventHandler<ElementEventArgs>(RequestProcessor.OnElementInfoChanged));
				}
			}
			else
			{
				throw new NullReferenceException("Provided IT2GManager is null.");
			}
		}

		/// <summary>Sets session manager.</summary>
		/// <exception cref="NullReferenceException">Thrown when a value was unexpectedly null.</exception>
		/// <param name="sessionMgr">Manager for session. Used for checkink input request id and resolving
		/// notification urls.</param>
		public void SetSessionMgr(ISessionManager sessionMgr)
		{
			if (sessionMgr != null)
			{
				RequestProcessor._sessionManager = sessionMgr;
			}
			else
			{
				throw new NullReferenceException("Provided SessionManager is null.");
			}
		}

		/// <summary>Sets Notification Sender.</summary>
		/// <param name="notificationSender">The Notification Sender.</param>
		public void SetNotificationSender(INotificationSender notificationSender)
		{
			if (notificationSender != null)
			{
				RequestProcessor._notificationSender = notificationSender;
			}
			else
			{
				throw new NullReferenceException("Provided INotificationSender is null.");
			}
		}

		/// <summary>Adds a request range.</summary>
		/// <param name="newRequests">List of requests waiting for processing.</param>
		public void AddRequestRange(List<RequestContext> newRequests)
		{
			// Queue to request list
			if (newRequests.Count > 0)
			{
				lock (RequestProcessor._lock)
				{
					_newRequests.AddRange(newRequests);

					// Signal thread and start transmitting
					_transmitEvent.Set();
				}
			}
		}

		/// <summary>Adds a request.</summary>
		/// <exception cref="NullReferenceException">Thrown when a value was unexpectedly null.</exception>
		/// <param name="newRequest">The new request.</param>
		public void AddRequest(RequestContext newRequest)
		{
			// Queue to request list
			if (newRequest != null)
			{
				lock (RequestProcessor._lock)
				{
					_newRequests.Add(newRequest);

					// Signal thread and start transmitting
					_transmitEvent.Set();
				}
			}
			else
			{
				throw new NullReferenceException("New request is null.");
			}
		}

		/// <summary>Executes the transmit event action.</summary>
		private static void OnTransmitEvent()
		{
            try
            {
                List<RequestContext> currentRequests = new List<RequestContext>();

                while (true)
                {
                    if (currentRequests.Count == 0)
                    {
                        _transmitEvent.WaitOne();
                    }

                    lock (RequestProcessor._lock)
                    {
                        // Move pending from _newRequests to currentRequests
                        if (_newRequests.Count > 0)
                        {
                            currentRequests.AddRange(RequestProcessor._newRequests);
                            RequestProcessor._newRequests.Clear();
                            currentRequests.RemoveAll(c => c == null);
                        }
                    }

                    for (int i = 0; i < currentRequests.Count; ++i)
                    {
                        RequestContext request = currentRequests[i];

                        switch (request.State)
                        {
                            case RequestState.Created:
                                for (int j = currentRequests.Count - 1; j >= 0; --j)
                                {
                                    RequestContext c = currentRequests[j];
                                    if (i != j && (c.ElementId == request.ElementId && ((c.RequestId != request.RequestId) || (c.RequestId == Guid.Empty || request.RequestId == Guid.Empty))))
                                    {
                                        currentRequests.RemoveAt(j);
                                        if (j < i)
                                        {
                                            --i;
                                        }
                                    }
                                }
                                break;
                            case RequestState.ReadyToSend:

                                RequestProcessor.TransmitRequest(request);
                                break;
                            case RequestState.WaitingRetry:
                                break;
                            case RequestState.Expired:
                                lock (RequestProcessor._lock)
                                {
                                    RequestProcessor._newRequests.Add(request);
                                    RequestProcessor._transmitEvent.Set();
                                }

                                break;
                            case RequestState.Transmitted:
                                request.CompletionStatus = true;
                                break;
                            case RequestState.AllRetriesExhausted:
                                break;
                            case RequestState.Completed:
                                break;
                        }
                    }

                    currentRequests.RemoveAll(c => c.IsStateFinal);
                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException)
            {
                // No logic to apply
            }
            catch (System.Exception exception)
            {
                LogManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.LiveVideoControl.LiveVideoControlService.OnTransmitEvent", exception, EventIdEnum.LiveVideoControl);
            }
		}

		/// <summary>Transmit request to embedded system.</summary>
		/// <param name="request">The request to send to embedded.</param>
		private static void TransmitRequest(RequestContext request)
		{
			bool requestIsTransmitted = false;

			ServiceInfo serviceInfo;

			// Check if target element is online
			bool elementIsOnline;
			T2GManagerErrorEnum rqstResult = _train2groundManager.IsElementOnline(request.ElementId, out elementIsOnline);
			switch (rqstResult)
			{
				case T2GManagerErrorEnum.eSuccess:
					if (elementIsOnline == true)
					{
						T2GManagerErrorEnum result = _train2groundManager.GetAvailableServiceData(request.ElementId, (int)eServiceID.eSrvSIF_LiveVideoControlServer, out serviceInfo);
						switch (result)
						{
							case T2GManagerErrorEnum.eSuccess:
								{
									string endpoint = "http://" + serviceInfo.ServiceIPAddress + ":" + serviceInfo.ServicePortNumber;
									try
									{
										// Call LiveVideoControl train service
										using (LiveVideoControlServiceClient trainClient = new LiveVideoControlServiceClient("LiveVideoControlEndpoint", endpoint))
										{
											try
											{
												if (request is ProcessStopVideoStreamingCommandRequestContext)
												{
													RequestProcessor.TransmitStopVideoStreamingCommandRequest(trainClient, request as ProcessStopVideoStreamingCommandRequestContext);
												}
												else if (request is ProcessStartVideoStreamingCommandRequestContext)
												{
													RequestProcessor.TransmitStartVideoStreamingCommandRequest(trainClient, request as ProcessStartVideoStreamingCommandRequestContext);
												}
												else if (request is ProcessSendVideoStreamingStatusRequestContext)
												{
													RequestProcessor.TransmitSendVideoStreamingStatusRequest(trainClient, request as ProcessSendVideoStreamingStatusRequestContext);
												}
												else
												{
													// No other request type supported
												}

												requestIsTransmitted = true;
											}
											catch (Exception ex)
											{
												if (!RequestProcessor.ShouldContinueOnTransmissionError(ex))
												{
													// Assume transmitted (no reason to retry)
													requestIsTransmitted = true;
												}
											}
											finally
											{
												if (trainClient.State == CommunicationState.Faulted)
												{
													trainClient.Abort();
												}
											}
										}
									}
									catch (Exception ex)
									{
										if (!RequestProcessor.ShouldContinueOnTransmissionError(ex))
										{
											// Assume transmitted (no reason to retry)
											requestIsTransmitted = true;
										}
									}
								}

								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlT2GServerOffline, string.Empty);
								break;
							case T2GManagerErrorEnum.eElementNotFound:
								RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlElementNotFound, request.ElementId);
								break;
							case T2GManagerErrorEnum.eServiceInfoNotFound:
								RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlServiceNotFound, request.ElementId);
								break;
							default:
								break;
						}
					}

					break;
				case T2GManagerErrorEnum.eT2GServerOffline:
					RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlT2GServerOffline, string.Empty);
					break;
				case T2GManagerErrorEnum.eElementNotFound:
					requestIsTransmitted = true;
					RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlElementNotFound, request.ElementId);
					break;
				default:
					break;
			}

			request.TransmissionStatus = requestIsTransmitted;

			if (request.State == RequestState.WaitingRetry && request.TransferAttemptsDone == 1)
			{
				// first attempt failed, send notification
				RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlDistributionWaitingToSend, request.ElementId);
			}
		}

		/// <summary>Transmit start video streaming command request.</summary>
		/// <exception cref="FaultException">Thrown when a Fault error condition occurs.</exception>
		/// <param name="client">The emebedded web service client.</param>
		/// <param name="request">The request to send to embedded.</param>
		private static void TransmitStartVideoStreamingCommandRequest(LiveVideoControlServiceClient client, ProcessStartVideoStreamingCommandRequestContext request)
		{
			LiveVideoControlRspEnumType result = LiveVideoControlRspEnumType.StopVideoStreamingCommandReceived;
			try
			{
				result = client.StartVideoStreamingCommand(request.RequestId.ToString(), request.Url);
			}
			catch (FaultException)
			{
				// When there is a SOAP fault while sending, we still notify that we sent the request
				RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlDistributionSent, request.ElementId);
				throw;
			}

			RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlDistributionSent, request.ElementId);

			if (result == LiveVideoControlRspEnumType.StartVideoStreamingCommandReceived)
			{
				RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlDistributionReceived, request.ElementId);
			}
			else
			{
				RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlErrorUndefined, request.ElementId);
			}

			request.TransmissionStatus = true;
		}

		/// <summary>Transmit start video streaming command request.</summary>
		/// <param name="client">The emebedded web service client.</param>
		/// <param name="request">The request to send to embedded.</param>
		private static void TransmitSendVideoStreamingStatusRequest(LiveVideoControlServiceClient client, ProcessSendVideoStreamingStatusRequestContext request)
		{
			client.SendVideoStreamingStatusCommand();
		}

		/// <summary>Sends a notification to Ground App.</summary>
		/// <param name="requestId">Request ID for the corresponding request.</param>
		/// <param name="status">Processing status.</param>
		/// <param name="parameter">The generic notification parameter.</param>
		private static void SendNotificationToGroundApp(
			Guid requestId,
			PIS.Ground.GroundCore.AppGround.NotificationIdEnum status,
			string parameter)
		{
			try
			{
				_notificationSender.SendNotification(status, parameter, requestId);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.ERROR, ex.Message,
					"PIS.Ground.LiveVideoControl.RequestProcessor.SendNotificationToGroundApp",
					ex, EventIdEnum.LiveVideoControl);
			}
		}

		/// <summary>Transmit stop video streaming command request.</summary>
		/// <exception cref="FaultException">Thrown when a Fault error condition occurs.</exception>
		/// <param name="client">The emebedded web service client.</param>
		/// <param name="request">The request to send to embedded.</param>
		private static void TransmitStopVideoStreamingCommandRequest(LiveVideoControlServiceClient client, ProcessStopVideoStreamingCommandRequestContext request)
		{
			LiveVideoControlRspEnumType result = LiveVideoControlRspEnumType.StartVideoStreamingCommandReceived;
			try
			{
				result = client.StopVideoStreamingCommand(request.RequestId.ToString());
			}
			catch (FaultException)
			{
				// When there is a SOAP fault while sending, we still notify that we sent the request
				RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlDistributionSent, request.ElementId);
				throw;
			}

			RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlDistributionSent, request.ElementId);

			if (result == LiveVideoControlRspEnumType.StopVideoStreamingCommandReceived)
			{
				RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlDistributionReceived, request.ElementId);
			}
			else
			{
				RequestProcessor.SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlErrorUndefined, request.ElementId);
			}

			request.TransmissionStatus = true;
		}

		/// <summary>
		/// Determines whether to continue retrying after a transmission error, or discard the request.
		/// </summary>
		/// <param name="exception">The intercepted exception.</param>
		/// <returns>
		/// true if the caller can continue to communicate with train web service, false otherwise.
		/// </returns>
		private static bool ShouldContinueOnTransmissionError(Exception exception)
		{
			bool canContinue = false;
			EndpointNotFoundException endpointException;
			if (exception is TimeoutException)
			{
				canContinue = true;
			}
			else if ((endpointException = exception as EndpointNotFoundException) != null)
			{
				WebException webException = endpointException.InnerException as WebException;
				if (webException != null && webException.Status == WebExceptionStatus.NameResolutionFailure)
				{
					canContinue = true;
				}
				else if (webException != null && webException.Status == WebExceptionStatus.ConnectFailure)
				{
					canContinue = true;
				}
				else
				{
					canContinue = true;
				}
			}
			else if (exception is ActionNotSupportedException)
			{
				canContinue = false;
			}
			else if (exception is ProtocolException)
			{
				WebException webException = exception.InnerException as WebException;
				if (webException != null && webException.Status == WebExceptionStatus.NameResolutionFailure)
				{
					canContinue = true;
				}
				else
				{
					canContinue = false;
				}
			}
			else
			{
				canContinue = false;
			}

			return canContinue;
		}
	}

	#region internal class

	/// <summary>Process stop video streaming command request context.</summary>
	public class ProcessStopVideoStreamingCommandRequestContext : RequestContext
	{
		/// <summary>
		/// Initializes a new instance of the ProcessStopVideoStreamingCommandRequestContext class.
		/// </summary>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">The request identifier.</param>
		/// <param name="sessionId">The session identifier.</param>
		public ProcessStopVideoStreamingCommandRequestContext(string elementId, Guid requestId, Guid sessionId)
			: base(string.Empty, elementId, requestId, sessionId)
		{
		}
	}

	/// <summary>Process start video streaming command request context.</summary>
	public class ProcessStartVideoStreamingCommandRequestContext : RequestContext
	{
		/// <summary>URL of the streaming.</summary>
		private string _url;

		/// <summary>
		/// Initializes a new instance of the ProcessStartVideoStreamingCommandRequestContext class.
		/// </summary>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">The request identifier.</param>
		/// <param name="sessionId">The session identifier.</param>
		/// <param name="url">URL to send to embedded system for streaming.</param>
		public ProcessStartVideoStreamingCommandRequestContext(string elementId, Guid requestId, Guid sessionId, string url)
			: base(string.Empty, elementId, requestId, sessionId)
		{
			this._url = url;
		}

		/// <summary>Gets URL of the document.</summary>
		/// <value>The streaming URL.</value>
		public string Url
		{
			get
			{
				return this._url;
			}
		}
	}

	/// <summary>Process send video streaming status request context.</summary>
	public class ProcessSendVideoStreamingStatusRequestContext : RequestContext
	{
		/// <summary>Information describing the service.</summary>
		private ServiceInfo _serviceInfo;

		/// <summary>
		/// Initializes a new instance of the ProcessSendVideoStreamingStatusRequestContext class.
		/// </summary>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">The request identifier.</param>
		/// <param name="serviceInfo">Information describing the service.</param>
		public ProcessSendVideoStreamingStatusRequestContext(string elementId, Guid requestId, ServiceInfo serviceInfo)
			: base(string.Empty, elementId, requestId, Guid.Empty)
		{
			this._serviceInfo = serviceInfo;
		}

		/// <summary>Gets the information describing the service.</summary>
		/// <value>Information describing the service.</value>
		public ServiceInfo ServiceInfo
		{
			get
			{
				return this._serviceInfo;
			}
		}
	}

	#endregion
}