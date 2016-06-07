//---------------------------------------------------------------------------------------------------
// <copyright file="RequestManager.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.T2G;

namespace PIS.Ground.DataPackage.RequestMgt
{
	/// <summary>Manager for requests.</summary>
	internal class RequestManager : IRequestManager
	{
		#region Constants

		public const string SubscriberId = "PIS.Ground.DataPackage.RequestMgt.RequestManager";

		#endregion

		#region attributes

		/// <summary>The transmit event to wake the transmit thread up.</summary>
		private static AutoResetEvent _TransmitEvent = new AutoResetEvent(false);

		/// <summary>Lock for thread safety.</summary>
		private static object _lock = new object();

		/// <summary>List of requests waiting for processing.</summary>
		private static List<IRequestContext> _newRequests = new List<IRequestContext>();

		/// <summary>The transmit thread. It process request transmission.</summary>
		private static Thread _transmitThread;

		/// <summary>The T2G manager to use local data store.</summary>
		private static IT2GManager _train2groundManager = null;

		/// <summary>The notification sender.</summary>
		private static INotificationSender _notificationSender = null;

		/// <summary>The string serializer.</summary>
		private static XmlSerializer _stringSerializer = new XmlSerializer(typeof(string));

		/// <summary>The string list serializer.</summary>
		private static XmlSerializer _stringListSerializer = new XmlSerializer(typeof(List<string>));

		#endregion

		#region static

		/// <summary>
		/// Callback called when Element Online state changes (signaled by the T2G Client).
		/// </summary>
		/// <param name="sender">Source of the event.</param>
		/// <param name="args">Event information to send to registered event handlers.</param>
		public static void OnElementInfoChanged(object sender, ElementEventArgs args)
		{
			if (args.SystemInformation != null && args.SystemInformation.IsOnline
				&& args.SystemInformation.ServiceList != null && args.SystemInformation.ServiceList.Count > 0
				&& args.SystemInformation.IsPisBaselineUpToDate)
			{
				_TransmitEvent.Set();
			}
		}

		/// <summary>Executes the transmit event action.</summary>
		private static void OnTransmitEvent()
		{
            try
            {
                List<IRequestContext> currentRequests = new List<IRequestContext>();
                BaselineDistributingRequestContext processBaselineDistributingRequest = null;

                while (true)
                {
                    if (currentRequests.Count == 0)
                    {
                        _TransmitEvent.WaitOne();
                    }

                    lock (_lock)
                    {
                        if (_newRequests.Count > 0)
                        {
                            currentRequests.AddRange(_newRequests);
                            _newRequests.Clear();
                            currentRequests.RemoveAll(c => c == null);
                        }
                    }

                    for (int i = 0; i < currentRequests.Count; ++i)
                    {
                        IRequestContext request = currentRequests[i];

                        switch (request.State)
                        {
                            case RequestState.Created:
                                Predicate<IRequestContext> comparisonPredicate;
                                if (request is BaselineDistributingRequestContext)
                                {
                                    comparisonPredicate = c => { return (c is BaselineDistributingRequestContext && (string.Equals(c.ElementId, request.ElementId) && (c.RequestId != request.RequestId || c.RequestId == Guid.Empty))); };
                                }
                                else if (request is BaselineForcingRequestContext)
                                {
                                    comparisonPredicate = c => { return (c is BaselineForcingRequestContext && (string.Equals(c.ElementId, request.ElementId) && (c.RequestId != request.RequestId || c.RequestId == Guid.Empty))); };
                                }
                                else
                                {
                                    comparisonPredicate = c => { return (c is BaselineSettingRequestContext && (string.Equals(c.ElementId, request.ElementId) && (c.RequestId != request.RequestId || c.RequestId == Guid.Empty))); };
                                }

                                for (int j = currentRequests.Count - 1; j >= 0; --j)
                                {
                                    if (comparisonPredicate(currentRequests[j]))
                                    {
                                        currentRequests.RemoveAt(j);
                                        if (j <= i)
                                        {
                                            --i;
                                        }
                                    }
                                }

                                break;
                            case RequestState.ReadyToSend:
                                break;
                            case RequestState.WaitingRetry:
                                break;
                            case RequestState.Expired:
                                BaselineForcingRequestContext processBaselineCommandRequest = request as BaselineForcingRequestContext;
                                if (null != processBaselineCommandRequest)
                                {
                                    if (processBaselineCommandRequest.CommandType == BaselineCommandType.CLEAR_FORCING)
                                    {
                                        SendNotificationToGroundApp(
                                            request.RequestId,
                                            PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageBaselineClearForcingTimedOut,
                                            FormatNotificationParameter(processBaselineCommandRequest.ElementId));
                                    }
                                    else
                                    {
                                        SendNotificationToGroundApp(
                                                request.RequestId,
                                                PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageBaselineForcingTimedOut,
                                                FormatNotificationParameter(processBaselineCommandRequest.ElementId));
                                    }
                                }
                                else
                                {
                                    processBaselineDistributingRequest = request as BaselineDistributingRequestContext;
                                    if (null != processBaselineDistributingRequest)
                                    {
                                        DataPackageService.deleteBaselineDistributingRequest(processBaselineDistributingRequest.ElementId);
                                        SendNotificationToGroundApp(
                                                request.RequestId,
                                                PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionTimedOut,
                                                FormatNotificationParameter(processBaselineDistributingRequest.ElementId, processBaselineDistributingRequest.BaselineVersion));
                                    }
                                }

                                break;
                            case RequestState.Transmitted:
                                request.CompletionStatus = true;
                                processBaselineDistributingRequest = request as BaselineDistributingRequestContext;
                                if (null != processBaselineDistributingRequest)
                                {
                                    DataPackageService.deleteBaselineDistributingRequest(processBaselineDistributingRequest.ElementId);
                                    SendNotificationToGroundApp(
                                            request.RequestId,
                                            PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionTransferred,
                                            FormatNotificationParameter(processBaselineDistributingRequest.ElementId, processBaselineDistributingRequest.BaselineVersion));
                                    processBaselineDistributingRequest.CompletionStatus = true;
                                }

                                break;
                            case RequestState.AllRetriesExhausted:
                                break;
                            case RequestState.Completed:
                                break;
                            case RequestState.Error:
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
                DataPackageService.mWriteLog(TraceType.EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, exception, exception.Message, EventIdEnum.DataPackage);
            }
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
				if (_notificationSender != null)
				{
					_notificationSender.SendNotification(status, parameter, requestId);
				}
				else
				{
					DataPackageService.mWriteLog(TraceType.ERROR, System.Reflection.MethodBase.GetCurrentMethod().Name, null, Logs.ERROR_UNDEFINED_NOTIFICATION_SENDER, EventIdEnum.DataPackage);
				}
			}
			catch (Exception ex)
			{
				DataPackageService.mWriteLog(TraceType.ERROR, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, ex.Message, EventIdEnum.DataPackage);
			}
		}

		/// <summary>Format a notification parameter.</summary>
		/// <param name="elementId">Identifier for the element.</param>
		/// <returns>The formatted notification parameter.</returns>
		private static string FormatNotificationParameter(string elementId)
		{
			using (StringWriter lWriter = new StringWriter())
			{
				_stringSerializer.Serialize(lWriter, elementId);
				return lWriter.ToString();
			}
		}

		/// <summary>Format a notification parameter.</summary>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="baselineVersion">The baseline version.</param>
		/// <returns>The formatted notification parameter.</returns>
		private static string FormatNotificationParameter(string elementId, string baselineVersion)
		{
			List<string> lParameters = new List<string>(2);
			lParameters.Add(elementId);
			lParameters.Add(baselineVersion);
			using (StringWriter lWriter = new StringWriter())
			{
				_stringListSerializer.Serialize(lWriter, lParameters);
				return lWriter.ToString();
			}
		}

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the RequestManager class.</summary>
		public RequestManager()
		{
			RequestManager._transmitThread = new Thread(new ThreadStart(RequestManager.OnTransmitEvent));
            RequestManager._transmitThread.Name = "DataPkg Rqt Mgr";
			if (_transmitThread.ThreadState != ThreadState.Running)
			{
				_transmitThread.Start();
			}
		}

		/// <summary>Initializes this object.</summary>
		/// <exception cref="ArgumentNullException">Thrown when a value was unexpectedly null.</exception>
		/// <param name="train2groundManager">The T2G manager to use local data store.</param>
		/// <param name="notificationSender">The notification sender.</param>
		public void Initialize(PIS.Ground.Core.T2G.IT2GManager train2groundManager, PIS.Ground.Core.Common.INotificationSender notificationSender)
		{
			lock (_lock)
			{
				if (train2groundManager != null)
				{
					RequestManager._train2groundManager = train2groundManager;
					RequestManager._train2groundManager.SubscribeToElementChangeNotification(SubscriberId, new EventHandler<ElementEventArgs>(RequestManager.OnElementInfoChanged));
				}
				else
				{
					throw new ArgumentNullException("train2groundManager");
				}

				if (notificationSender != null)
				{
					RequestManager._notificationSender = notificationSender;
				}
				else
				{
					throw new ArgumentNullException("notificationSender");
				}
			}
		}

		#endregion

		#region interface

		/// <summary>Adds a request range. This method is meant to be used at startup to reload saved requests.
		/// 		 Thereof, there is no save done if the requests are BaselineDistributingRequest, use AddRequest if needed.</summary>
		/// <param name="requestContextList">List of request contexts.</param>
		public void AddRequestRange(List<PIS.Ground.Core.Data.IRequestContext> requestContextList)
		{
			// Queue to request list
			if (requestContextList != null && requestContextList.Count > 0)
			{
				lock (_lock)
				{
					_newRequests.AddRange(requestContextList);
				}

				// Signal thread and start transmitting
				_TransmitEvent.Set();
			}
		}

		/// <summary>Adds a request.</summary>
		/// <exception cref="ArgumentNullException">Thrown when a value was unexpectedly null.</exception>
		/// <param name="requestContext">Context for the request.</param>
		public void AddRequest(PIS.Ground.Core.Data.IRequestContext requestContext)
		{
			if (requestContext != null)
			{
				BaselineDistributingRequestContext processBaselineDistributingRequest = requestContext as BaselineDistributingRequestContext;
				if (null != processBaselineDistributingRequest)
				{
					DataPackageService.saveBaselineDistributingRequest(processBaselineDistributingRequest);
				}

				lock (_lock)
				{
					_newRequests.Add(requestContext);
				}

				_TransmitEvent.Set();
			}
			else
			{
				throw new ArgumentNullException("requestContext");
			}
		}

		#endregion
	}
}