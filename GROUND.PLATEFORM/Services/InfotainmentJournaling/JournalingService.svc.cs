//---------------------------------------------------------------------------------------------------
// <copyright file="JournalingService.svc.cs" company="Alstom">
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
using System.ServiceModel;
using System.Threading;

using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.T2G;
using PIS.Ground.Core.Common;
using System.Globalization;
using PIS.Ground.Core.Utility;
using PIS.Ground.Common;

namespace PIS.Ground.Infotainment.Journaling
{
	[CreateOnDispatchService(typeof(JournalingService))]
	[ServiceBehavior(Namespace = "http://alstom.com/pacis/pis/ground/infotainment/journaling/")]
	public class JournalingService : IJournalingService
	{
		#region consts

		private const string NOTIFICATION_PARAMETER_ELEMENT_ID = "elementId";
		private const string NOTIFICATION_PARAMETER_FOLDER_ID = "folderId";
		private const string NOTIFICATION_PARAMETER_FTP_IP = "ftpServerIP";
		private const string NOTIFICATION_PARAMETER_FTP_PATH = "ftpServerPath";
		private const string NOTIFICATION_PARAMETER_FTP_PORT = "ftpServerPortNumber";
		private const string NOTIFICATION_PARAMETER_FTP_USERNAME = "ftpUserName";
		private const string NOTIFICATION_PARAMETER_FTP_PASSWORD = "ftpPassword";
		private const uint MAX_REQUEST_TIMEOUT = 43200;

		/// <summary>Identifier for event subscription.</summary>
		private const string SubscriberId = "PIS.Ground.Infotainment.Journaling.JournalingService";

		#endregion

		#region static fields

		private static volatile bool _initialized = false;

		private static object _initializationLock = new object();

		private static Object _lock = new Object();

		private static List<RequestContext> _newRequests = new List<RequestContext>();

		private static List<RequestCompletedEvent> _requestCompletedEvents = new List<RequestCompletedEvent>();

		private static AutoResetEvent _transmitEvent = new AutoResetEvent(false);

		private static Thread _transmitThread = null;

		private static IT2GManager _t2gManager = null;

		private static ISessionManager _sessionManager = null;

		private static INotificationSender _notificationSender = null;

		private static List<Guid> _reqIdsForDownloadingNotif = new List<Guid>();

		private static Object _reqIdsListLock = new Object();

		#endregion

		/// <summary>Initializes a new instance of the JournalingService class.</summary>
		public JournalingService()
		{
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "JournalingService";
            }

			Initialize();
		}

		/// <summary>Initializes this object.</summary>
		public static void Initialize()
		{
			if (!_initialized)
			{
				lock (_initializationLock)
				{
					if (!_initialized)
					{
						try
						{
							_sessionManager = new SessionManager();

							_notificationSender = new NotificationSender(_sessionManager);

							_t2gManager = T2GManagerContainer.T2GManager;

							_transmitThread = new Thread(new ThreadStart(OnTransmitEvent));
                            _transmitThread.Name = "Journ. Transmit";

							_t2gManager.SubscribeToElementChangeNotification(SubscriberId, new EventHandler<ElementEventArgs>(OnElementInfoChanged));
							_t2gManager.SubscribeToFilePublishedNotification(SubscriberId, new EventHandler<FilePublishedNotificationArgs>(OnFilesPublished));
							_t2gManager.SubscribeToFileReceivedNotification(SubscriberId, new EventHandler<FileReceivedArgs>(OnFileReceived));

							_transmitThread.Start();
						}
						catch (System.Exception e)
						{
							LogManager.WriteLog(TraceType.ERROR, e.Message, "PIS.Ground.Infotainment.Journaling.JournalingService.Initialize", e, EventIdEnum.InfotainmentJournaling);
						}

                        _initialized = true;
					}
				}
			}
		}

		/// <summary>
		/// Callback called when Element Online state changes (signaled by the T2G Client)
		/// </summary>
		private static void OnElementInfoChanged(object sender, ElementEventArgs args)
		{
			//Signal the event to start handling the request.
			_transmitEvent.Set();
		}

		/// <summary>
		/// Callback called when a download folder is published on the train (signaled by the T2G client)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="status"></param>
		private static void OnFilesPublished(object sender, FilePublishedNotificationArgs status)
		{
			LogManager.WriteLog(TraceType.INFO, "OnFilesPublished called, folderId=" + status.FolderId + " systemId=" + status.SystemId, "PIS.Ground.Infotainment.Journaling.JournalingService.OnFilesPublished", null, EventIdEnum.InfotainmentJournaling);

			try
			{
				string lGetFolderInformationError;
				IFolderInfo lFolderInfo = _t2gManager.T2GFileDistributionManager.GetRemoteFolderInformation(status.FolderId, status.SystemId, ConfigurationSettings.AppSettings["ApplicationId"], out lGetFolderInformationError);
				if (lFolderInfo != null)
				{
					List<RecipientId> lRecipientIdList = new List<RecipientId>();

					RecipientId lRecipient = new RecipientId();
					lRecipient.ApplicationId = ConfigurationSettings.AppSettings["ApplicationId"];
					lRecipient.MissionId = string.Empty;
					lRecipient.SystemId = "ground";
					lRecipientIdList.Add(lRecipient);

					DownloadFolderRequest objRequest = new DownloadFolderRequest(
						new Guid(lFolderInfo.FolderName), // The folder name should have the GUID-formatted name, this is our request ID
						lFolderInfo.ExpirationDate, // Set to the folder's expiration date
						lFolderInfo.FolderName,
						lRecipientIdList,
						DateTime.Now, //Start date: now
						"InfotainmentJournaling Service Transfer Task",
						FileTransferMode.AnyBandwidth,
						16, //normal priority
						new EventHandler<FileDistributionStatusArgs>(OnFilesDistribution),
						status.FolderId);

					objRequest.SystemId = status.SystemId;
					objRequest.Compression = false;

					String lErrorMessage = _t2gManager.T2GFileDistributionManager.DownloadFolder(objRequest);
					if (!String.IsNullOrEmpty(lErrorMessage))
					{
						LogManager.WriteLog(TraceType.ERROR, "T2G DownloadFolder failure, error " + lErrorMessage, "PIS.Ground.Infotainment.Journaling.JournalingService.OnFilesPublished", null, EventIdEnum.InfotainmentJournaling);
					}
				}
				else
				{
					LogManager.WriteLog(TraceType.ERROR, "Failure to retrieve folder info, error " + lGetFolderInformationError, "PIS.Ground.Infotainment.Journaling.JournalingService.OnFilesPublished", null, EventIdEnum.InfotainmentJournaling);
				}
			}
			catch (System.Exception e)
			{
				LogManager.WriteLog(TraceType.ERROR, "Exception thrown", "PIS.Ground.Infotainment.Journaling.JournalingService.OnFilesPublished", e, EventIdEnum.InfotainmentJournaling);
			}
		}

		/// <summary>
		/// Callback called when File Transfer state changes (signaled by the T2G client)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="status"></param>
		private static void OnFilesDistribution(object sender, FileDistributionStatusArgs status)
		{
			LogManager.WriteLog(TraceType.INFO, "OnFilesDistribution called, folderId=" + status.FolderId + " systemId=" + status.TaskId, "PIS.Ground.Infotainment.Journaling.JournalingService.OnFilesDistribution", null, EventIdEnum.InfotainmentJournaling);

			try
			{
				if (status.TaskStatus == TaskState.Started && status.CurrentTaskPhase == TaskPhase.Transfer)
				{
					FileDistributionRequest lRequest = _t2gManager.T2GFileDistributionManager.GetFileDistributionRequestByTaskId(status.TaskId);
					lock (_reqIdsListLock)
					{
						if (false == _reqIdsForDownloadingNotif.Contains(lRequest.RequestId))
						{
							if (lRequest != null)
							{
								SendNotificationToGroundApp(lRequest.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadDownloading, lRequest.SystemId);
							}
							_reqIdsForDownloadingNotif.Add(lRequest.RequestId);
						}
					}
				}
			}
			catch (System.Exception e)
			{
				LogManager.WriteLog(TraceType.ERROR, "Exception thrown", "PIS.Ground.Infotainment.Journaling.JournalingService.OnFilesDistribution", e, EventIdEnum.InfotainmentJournaling);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="status"></param>
		private static void OnFileReceived(object sender, FileReceivedArgs status)
		{
			LogManager.WriteLog(TraceType.INFO, "OnFileReceived called", "PIS.Ground.Infotainment.Journaling.JournalingService.OnFileReceived", null, EventIdEnum.InfotainmentJournaling);

			try
			{
				string lGetFolderInformationError;
				IFolderInfo lFolderInfo = _t2gManager.T2GFileDistributionManager.GetFolderInformation(status.FolderId, out lGetFolderInformationError);
				if (lFolderInfo != null)
				{
					Guid lRequestId = new Guid(lFolderInfo.FolderName); // The folder name should have the GUID-formatted name, this is our request ID

					Dictionary<string, string> lParamMap = new Dictionary<string, string>();

					lParamMap.Add(NOTIFICATION_PARAMETER_ELEMENT_ID, lFolderInfo.SystemId);
					lParamMap.Add(NOTIFICATION_PARAMETER_FOLDER_ID, lFolderInfo.FolderId.ToString());
					lParamMap.Add(NOTIFICATION_PARAMETER_FTP_IP, lFolderInfo.FtpIP);
					lParamMap.Add(NOTIFICATION_PARAMETER_FTP_PATH, lFolderInfo.FtpPath);
					lParamMap.Add(NOTIFICATION_PARAMETER_FTP_PORT, lFolderInfo.FtpPort.ToString());
					lParamMap.Add(NOTIFICATION_PARAMETER_FTP_USERNAME, lFolderInfo.Username);
					lParamMap.Add(NOTIFICATION_PARAMETER_FTP_PASSWORD, lFolderInfo.Pwd);

					SendNotificationToGroundApp(lRequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadCompleted, lParamMap);

					Guid lSessionId;
					string lSessionMgrError = _sessionManager.GetSessionIdByRequestId(lRequestId, out lSessionId);

					lock (_lock)
					{
						if (String.IsNullOrEmpty(lSessionMgrError))
						{
							_newRequests.Add(
								new NotifyReportRetrievedRequestContext(
									lFolderInfo.SystemId,
									lRequestId,
									(uint)lFolderInfo.FolderId,
									lSessionId,
									MAX_REQUEST_TIMEOUT));
						}
						else
						{
							LogManager.WriteLog(TraceType.ERROR,
								"Failure to retrieve Session ID using Request ID. Error: " + lSessionMgrError,
								"PIS.Ground.Infotainment.Journaling.JournalingService.OnFileReceived",
								null, EventIdEnum.InfotainmentJournaling);
						}

						_requestCompletedEvents.Add(new RequestCompletedEvent(lFolderInfo.SystemId, lRequestId));
					}

					_transmitEvent.Set();
				}
				else
				{
					LogManager.WriteLog(TraceType.ERROR, "Failure to retrieve folder info, error " + lGetFolderInformationError, "PIS.Ground.Infotainment.Journaling.JournalingService.OnFileReceived", null, EventIdEnum.InfotainmentJournaling);
				}
			}
			catch (System.Exception e)
			{
				LogManager.WriteLog(TraceType.ERROR, "Exception thrown", "PIS.Ground.Infotainment.Journaling.JournalingService.OnFileReceived", e, EventIdEnum.InfotainmentJournaling);
			}


		}

		/// <summary>
		/// Send notification to Ground Application.
		/// <param name="pRequestId"> Request ID corresponding to the specified request </param>
		/// <param name="pStatus"> Status : Completed/Failed/Processing </param>
		/// </summary>
		private static void SendNotificationToGroundApp(Guid pRequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum pStatus, string pElementId)
		{
			Dictionary<string, string> lParameters = new Dictionary<string, string>();
			lParameters.Add(NOTIFICATION_PARAMETER_ELEMENT_ID, pElementId);
			SendNotificationToGroundApp(pRequestId, pStatus, lParameters);
		}

		/// <summary>
		/// Send notification to Ground Application.
		/// <param name="pRequestId"> Request ID corresponding to the specified request </param>
		/// <param name="pStatus"> Status : Completed/Failed/Processing </param>
		/// </summary>
		private static void SendNotificationToGroundApp(Guid requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum status)
		{
			LogManager.WriteLog(TraceType.INFO, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_CALLED, "SendNotificationToGroundApp"), "PIS.Ground.Infotainment.Journaling.JournalingService.SendNotificationToGroundApp", null, EventIdEnum.InfotainmentJournaling);
			try
			{
				_notificationSender.SendNotification(status, null, requestId);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.ERROR, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_EXCEPTION, "SendNotificationToGroundApp"), "PIS.Ground.Infotainment.Journaling.JournalingService.SendNotificationToGroundApp", ex, EventIdEnum.InfotainmentJournaling);
			}
		}


		/// <summary>
		/// Send notification to Ground Application.
		/// <param name="pRequestId"> Request ID corresponding to the specified request </param>
		/// <param name="pStatus"> Status : Completed/Failed/Processing </param>
		/// <param name="pParameters">Parameters: key-value pairs</param>
		/// </summary>
		private static void SendNotificationToGroundApp(Guid requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum status, System.Collections.Generic.Dictionary<string, string> pParameters)
		{
			LogManager.WriteLog(TraceType.INFO, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_CALLED, "SendNotificationToGroundApp"), "PIS.Ground.Infotainment.Journaling.JournalingService.SendNotificationToGroundApp", null, EventIdEnum.InfotainmentJournaling);
			try
			{
				List<string> lParameters = new List<string>();
				foreach (KeyValuePair<string, string> lKeyValue in pParameters)
				{
					lParameters.Add(lKeyValue.Key);
					lParameters.Add(lKeyValue.Value);
				}

				System.Xml.Serialization.XmlSerializer lXmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(List<string>));

				StringWriter lStringWriter = new StringWriter();
				lXmlSerializer.Serialize(lStringWriter, lParameters);

				//Send Notification tp AppGround
				_notificationSender.SendNotification(status, lStringWriter.ToString(), requestId);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.ERROR, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_EXCEPTION, "SendNotificationToGroundApp"), "PIS.Ground.Infotainment.Journaling.JournalingService.SendNotificationToGroundApp", ex, EventIdEnum.InfotainmentJournaling);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public InfotainmentJournalingElementListResponse GetAvailableElementList(Guid sessionId)
		{
			InfotainmentJournalingElementListResponse result = new InfotainmentJournalingElementListResponse();
			result.ResultCode = ResultCodeEnum.InternalError;

			if (_sessionManager.IsSessionValid(sessionId))
			{
				T2GManagerErrorEnum lResult = _t2gManager.GetAvailableElementDataList(out result.ElementList);

				if (lResult == T2GManagerErrorEnum.eSuccess)
				{
					result.ResultCode = ResultCodeEnum.RequestAccepted;
				}
				else
				{
					result.ResultCode = ResultCodeEnum.ElementListNotAvailable;
				}
			}
			else
			{
				result.ResultCode = ResultCodeEnum.InvalidSessionId;
			}

			return result;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="targetAddress"></param>
		/// <param name="requestTimeout"></param>
		/// <returns></returns>
		public GetReportResponse GetReport(Guid sessionId, TargetAddressType targetAddress, uint requestTimeout)
		{
			GetReportResponse result = new GetReportResponse();
			result.RequestId = Guid.Empty;
			result.ResultCode = ResultCodeEnum.InternalError;

			if (requestTimeout <= MAX_REQUEST_TIMEOUT)
			{
				if (_sessionManager.IsSessionValid(sessionId))
				{
					Guid requestId = Guid.Empty;
					_sessionManager.GenerateRequestID(sessionId, out requestId);

					if (requestId != Guid.Empty) // TODO requires modification to method GenerateRequestID
					{
						ElementList<AvailableElementData> elements;
						T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableElementDataByTargetAddress(targetAddress, out elements);
						switch (lRqstResult)
						{
							case T2GManagerErrorEnum.eSuccess:
								// Queue to request list
								foreach (AvailableElementData element in elements)
								{
									SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadProcessing, element.ElementNumber);

									GetReportRequestContext request = new GetReportRequestContext(element.ElementNumber, requestId, sessionId, requestTimeout);

									lock (_lock)
									{
										_newRequests.Add(request);
									}
								}

								// Signal thread and start transmitting
								_transmitEvent.Set();

								result.RequestId = requestId;
								result.ResultCode = ResultCodeEnum.RequestAccepted;
								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								result.ResultCode = ResultCodeEnum.T2GServerOffline;
								break;
							case T2GManagerErrorEnum.eElementNotFound:
								switch (targetAddress.Type)
								{
									case AddressTypeEnum.Element:
										result.ResultCode = ResultCodeEnum.UnknownElementId;
										break;
									case AddressTypeEnum.MissionCode:
										result.ResultCode = ResultCodeEnum.UnknownMissionId;
										break;
									case AddressTypeEnum.MissionOperatorCode:
										result.ResultCode = ResultCodeEnum.UnknownMissionId;
										break;
								}
								break;
							default:
								break;
						}
					}
					else
					{
						result.ResultCode = ResultCodeEnum.InvalidSessionId;
					}
				}
				else
				{
					result.ResultCode = ResultCodeEnum.InvalidSessionId;
				}
			}
			else
			{
				result.ResultCode = ResultCodeEnum.InvalidRequestTimeout;
			}

			return result;
		}

		/// <summary>
		///
		/// </summary>
		private static void OnTransmitEvent()
		{
            try
            {
                List<RequestContext> currentRequests = new List<RequestContext>();

                while (true)
                {
                    if (currentRequests.Count == 0)
                    {
                        LogManager.WriteLog(TraceType.INFO, "No events to process. Sleeping...", "PIS.Ground.Infotainment.Journaling.JournalingService.OnTransmitEvent", null, EventIdEnum.InfotainmentJournaling);

                        _transmitEvent.WaitOne();  //wait until new currentRequests

                        LogManager.WriteLog(TraceType.INFO, "Woke up. New events to process...", "PIS.Ground.Infotainment.Journaling.JournalingService.OnTransmitEvent", null, EventIdEnum.InfotainmentJournaling);
                    }

                    lock (_lock)
                    {
                        //move pending from _newRequests to currentRequests
                        if (_newRequests.Count > 0)
                        {
                            currentRequests.AddRange(_newRequests);
                            _newRequests.Clear();
                            currentRequests.RemoveAll(c => c == null);
                        }
                    }

                    foreach (IRequestContext request in currentRequests)
                    {
                        switch (request.State)
                        {
                            case RequestState.Created:
                                break;

                            case RequestState.ReadyToSend:
                                TransmitRequest(request);
                                break;

                            case RequestState.WaitingRetry:
                                break;

                            case RequestState.Expired:
                                Dictionary<string, string> lParameters = new Dictionary<string, string>();
                                lParameters.Add(NOTIFICATION_PARAMETER_ELEMENT_ID, request.ElementId);
                                SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadTimedOut, lParameters);
                                LogManager.WriteLog(TraceType.INFO, "Request expired.", "PIS.Ground.Infotainment.Journaling.JournalingService.OnTransmitEvent", null, EventIdEnum.InfotainmentJournaling);
                                lock (_reqIdsListLock)
                                {
                                    if (_reqIdsForDownloadingNotif.Contains(request.RequestId))
                                    {
                                        _reqIdsForDownloadingNotif.Remove(request.RequestId);
                                    }
                                }

                                break;

                            case RequestState.Transmitted:
                                break;

                            case RequestState.AllRetriesExhausted:
                                break;

                            case RequestState.Completed:
                                lock (_reqIdsListLock)
                                {
                                    if (_reqIdsForDownloadingNotif.Contains(request.RequestId))
                                    {
                                        _reqIdsForDownloadingNotif.Remove(request.RequestId);
                                    }
                                }
                                LogManager.WriteLog(TraceType.INFO, "Processing GetReportRequest... completed", "PIS.Ground.Infotainment.Journaling.JournalingService.OnTransmitEvent", null, EventIdEnum.InfotainmentJournaling);
                                break;
                            case RequestState.Error:
                                break;
                        }
                    }

                    lock (_lock)
                    {
                        foreach (RequestCompletedEvent evt in _requestCompletedEvents)
                        {
                            List<RequestContext> lReqList = currentRequests.FindAll(
                                delegate(RequestContext c) { return c.ElementId == evt.elementId && c.RequestId == evt.requestId; });

                            foreach (RequestContext lRequest in lReqList)
                            {
                                lRequest.CompletionStatus = true;
                            }
                        }

                        _requestCompletedEvents.Clear();
                    }

                    currentRequests.RemoveAll(c => c.IsStateFinal);
                    Thread.Sleep(1000);
                }
            }
            catch (ThreadAbortException)
            {
                // No logic to apply
            }
            catch (System.Exception exception)
            {
                LogManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.Infotainment.Journaling.JournalingService.OnTransmitEvent", exception, EventIdEnum.InfotainmentJournaling);
            }
		}

		private static void TransmitRequest(IRequestContext request)
		{
			if (request is GetReportRequestContext)
			{
				TransmitGetReportRequest(request as GetReportRequestContext);
			}
			else if (request is NotifyReportRetrievedRequestContext)
			{
				TransmitNotifyReportRetrievedRequest(request as NotifyReportRetrievedRequestContext);
			}
			else
			{
				// Other request types
				LogManager.WriteLog(TraceType.ERROR, "Invalid request type.", "PIS.Ground.Infotainment.Journaling.JournalingService.OnTransmitEvent", null, EventIdEnum.InfotainmentJournaling);
			}

			if (request.State == RequestState.WaitingRetry && request.TransferAttemptsDone == 1)
			{
				// first attempt failed, send notification
				Dictionary<string, string> lParameters = new Dictionary<string, string>();
				lParameters.Add(NOTIFICATION_PARAMETER_ELEMENT_ID, request.ElementId);
				SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadWaitingToSend, lParameters);
			}
		}

		private static void TransmitGetReportRequest(GetReportRequestContext request)
		{
			bool lTransmitted = false;

			ServiceInfo serviceInfo;
			T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableServiceData(request.ElementId, (int)eServiceID.eSrvSIF_ReportExchangeServer, out serviceInfo);

			switch (lRqstResult)
			{
				case T2GManagerErrorEnum.eSuccess:
					{
						String lEndpoint = "http://" + serviceInfo.ServiceIPAddress + ":" + serviceInfo.ServicePortNumber;

						// Check if target element is online
						bool lIsElementOnline;
						lRqstResult = _t2gManager.IsElementOnline(request.ElementId, out lIsElementOnline);

						switch (lRqstResult)
						{
							case T2GManagerErrorEnum.eSuccess:
								if (true == lIsElementOnline)
								{
									// Call Report Exchange train service
									using (ReportExchange.ReportExchangeServiceClient client = new ReportExchange.ReportExchangeServiceClient("ReportExchangeEndpoint", lEndpoint))
									{
										try
										{
											String requestIdStr = request.RequestId.ToString();

											// Call onboard train Report Exchange Web service
											ReportExchange.ResultType result;
											client.GetInfotainmentJournal(ref requestIdStr, out result);

											// TODO (???) extract Folder ID from request - Check with Malik request.FolderId = result.folderId

											switch (result)
											{
												case ReportExchange.ResultType.ServiceInhibited:
													SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadInhibited, request.ElementId);
													break;
												case ReportExchange.ResultType.Failure:
													SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.Failed, request.ElementId);
													break;
												case ReportExchange.ResultType.NoEntryFound:
													SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadNoReportEntryFound, request.ElementId);
													lTransmitted = true;
													break;
												case ReportExchange.ResultType.Success:
													lTransmitted = true;
													break;
												default:
													LogManager.WriteLog(TraceType.ERROR,
														String.Format("Unexpected return value from WebService client: {0}", result),
														"PIS.Ground.Infotainment.Journaling.JournalingService.TransmitGetReportRequest",
														null, EventIdEnum.InfotainmentJournaling);
													break;
											}
										}
										catch (Exception ex)
										{
											LogManager.WriteLog(TraceType.ERROR,
												String.Format("Unexpected error from WebService client: {0}", ex.Message),
												"PIS.Ground.Infotainment.Journaling.JournalingService.TransmitGetReportRequest",
												ex, EventIdEnum.InfotainmentJournaling);

										}
										finally
										{
											if (client.State == CommunicationState.Faulted)
											{
												client.Abort();
											}
										}
									}
								}
								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentT2GServerOffline, "");
								break;
							case T2GManagerErrorEnum.eElementNotFound:
								SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadElementNotFound, request.ElementId);
								break;
							default:
								break;
						}
					}
					break;
				case T2GManagerErrorEnum.eT2GServerOffline:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentT2GServerOffline, "");
					break;
				case T2GManagerErrorEnum.eElementNotFound:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadElementNotFound, request.ElementId);
					break;
				case T2GManagerErrorEnum.eServiceInfoNotFound:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadServiceNotFound, eServiceID.eSrvSIF_ReportExchangeServer.ToString());
					break;
				default:
					LogManager.WriteLog(TraceType.INFO, "Failed to obtain service data from T2G", "PIS.Ground.Infotainment.Journaling.JournalingService.TransmitGetReportRequest", null, EventIdEnum.InfotainmentJournaling);
					break;
			}

			request.TransmissionStatus = lTransmitted;
		}

		private static void TransmitNotifyReportRetrievedRequest(NotifyReportRetrievedRequestContext request)
		{
			bool lTransmitted = false;

			ServiceInfo serviceInfo;
			T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableServiceData(request.ElementId, (int)eServiceID.eSrvSIF_ReportExchangeServer, out serviceInfo);

			switch (lRqstResult)
			{
				case T2GManagerErrorEnum.eSuccess:
					String lEndpoint = "http://" + serviceInfo.ServiceIPAddress + ":" + serviceInfo.ServicePortNumber;

					// Check if target element is online
					bool lIsElementOnline;
					lRqstResult = _t2gManager.IsElementOnline(request.ElementId, out lIsElementOnline);

					switch (lRqstResult)
					{
						case T2GManagerErrorEnum.eSuccess:
							if (lIsElementOnline)
							{
								// Call Report Exchange train service
								using (ReportExchange.ReportExchangeServiceClient client = new ReportExchange.ReportExchangeServiceClient("ReportExchangeEndpoint", lEndpoint))
								{
									try
									{
										String requestIdStr = request.RequestId.ToString();

										// Call onboard train Report Exchange Web service
										ReportExchange.ResultType result = client.NotifyInfotainmentJournalRetrieved(
											ref requestIdStr, request.FolderId);

										switch (result)
										{
											case ReportExchange.ResultType.ServiceInhibited:
												SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadInhibited, request.ElementId);
												break;
											case ReportExchange.ResultType.Failure:
												SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.Failed, request.ElementId);
												break;
											case ReportExchange.ResultType.Success:
												lTransmitted = true;
												break;
											default:
												LogManager.WriteLog(TraceType.ERROR,
													String.Format("Unexpected return value from WebService client: {0}", result),
													"PIS.Ground.Infotainment.Journaling.JournalingService.TransmitGetReportRequest",
													null, EventIdEnum.InfotainmentJournaling);
												break;
										}
									}
									catch (Exception ex)
									{
										LogManager.WriteLog(TraceType.ERROR,
											String.Format("Unexpected error from WebService client: {0}", ex.Message),
											"PIS.Ground.Infotainment.Journaling.JournalingService.TransmitNotifyReportRetrievedRequest",
											ex, EventIdEnum.InfotainmentJournaling);

									}
									finally
									{
										if (client.State == CommunicationState.Faulted)
										{
											client.Abort();
										}
									}
								}
							}
							break;
						case T2GManagerErrorEnum.eT2GServerOffline:
							SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentT2GServerOffline, String.Empty);
							break;
						case T2GManagerErrorEnum.eElementNotFound:
							SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadElementNotFound, request.ElementId);
							break;
						default:
							LogManager.WriteLog(TraceType.ERROR,
								String.Format("T2G method IsElementOnline failed with error code: {0}", lRqstResult),
								"PIS.Ground.Infotainment.Journaling.JournalingService.TransmitGetReportRequest",
								null, EventIdEnum.InfotainmentJournaling);
							break;
					}
					break;
				case T2GManagerErrorEnum.eT2GServerOffline:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentT2GServerOffline, String.Empty);
					break;
				case T2GManagerErrorEnum.eElementNotFound:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadElementNotFound, request.ElementId);
					break;
				case T2GManagerErrorEnum.eServiceInfoNotFound:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InfotainmentDownloadServiceNotFound, eServiceID.eSrvSIF_ReportExchangeServer.ToString());
					break;
				default:
					LogManager.WriteLog(TraceType.INFO, "Failed to obtain service data from T2G", "PIS.Ground.Infotainment.Journaling.JournalingService.TransmitGetVersionFileRequest", null, EventIdEnum.InfotainmentJournaling);
					break;
			}

			request.TransmissionStatus = lTransmitted;
		}

		internal static void OnTrainNotification(
			Guid pRequestId,
			PIS.Ground.Infotainment.Journaling.Notification.NotificationIdEnum pNotificationId,
			string pElementId,
			string pParameter)
		{
			try
			{
				switch (pNotificationId)
				{
					case PIS.Ground.Infotainment.Journaling.Notification.NotificationIdEnum.Completed:
						//nothing to do
						break;

					case PIS.Ground.Infotainment.Journaling.Notification.NotificationIdEnum.Failed:

						int lFailedRemoteFolderId = 0;
						if (int.TryParse(pParameter, out lFailedRemoteFolderId))
						{
							string lGetFolderInformationError;
							IFolderInfo lFolderInfo = _t2gManager.T2GFileDistributionManager.GetRemoteFolderInformation(lFailedRemoteFolderId, pElementId, ConfigurationSettings.AppSettings["ApplicationId"], out lGetFolderInformationError);
							if (lFolderInfo != null)
							{
								Guid lRequestId = new Guid(lFolderInfo.FolderName); // The folder name should have the GUID-formatted name, this is our request ID
								SendNotificationToGroundApp(lRequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.Failed, pElementId);

							}
							else
							{
								LogManager.WriteLog(TraceType.ERROR, "Failure to retrieve folder info, error " + lGetFolderInformationError, "PIS.Ground.Infotainment.Journaling.JournalingService.OnTrainNotification", null, EventIdEnum.InfotainmentJournaling);
							}
						}
						else
						{
							LogManager.WriteLog(TraceType.ERROR, "Invalid folder ID as notification parameter, actual value [" + pParameter + "]", "PIS.Ground.Infotainment.Journaling.JournalingService.OnTrainNotification", null, EventIdEnum.InfotainmentJournaling);
						}
						break;
				}

			}
			catch (System.Exception e)
			{
				LogManager.WriteLog(TraceType.ERROR, "Exception thrown", "PIS.Ground.Infotainment.Journaling.JournalingService.OnTrainNotification", e, EventIdEnum.InfotainmentJournaling);
			}
		}
	}

	/// <summary>
	///
	/// </summary>
	internal class GetReportRequestContext : RequestContext
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="sessionId"></param>
		/// <param name="targetAddress"></param>
		/// <param name="timeout">Request timeout in minutes</param>
		public GetReportRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout)
			: base("", elementId, requestId, sessionId, timeout)
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	internal class NotifyReportRetrievedRequestContext : RequestContext
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="elementId"></param>
		/// <param name="requestId"></param>
		/// <param name="folderId"></param>
		/// <param name="sessionId"></param>
		/// <param name="timeout">Request timeout in minutes.</param>
		public NotifyReportRetrievedRequestContext(string elementId, Guid requestId, uint folderId, Guid sessionId, uint timeout)
			: base("", elementId, requestId, sessionId, timeout)
		{
			FolderId = folderId;
		}
	}

	/// <summary>
	///
	/// </summary>
	internal class RequestCompletedEvent
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="elementId"></param>
		/// <param name="requestId"></param>
		public RequestCompletedEvent(string elementId, Guid requestId)
		{
			this.elementId = elementId;
			this.requestId = requestId;
		}

		public string elementId;
		public Guid requestId;
	}
}
