//---------------------------------------------------------------------------------------------------
// <copyright file="MaintenanceService.svc.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
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
using PIS.Ground.Maintenance.Train;
using System.Globalization;
using Maintenance;
using PIS.Ground.Core.Utility;
using PIS.Ground.Common;

namespace PIS.Ground.Maintenance
{
	[CreateOnDispatchService(typeof(MaintenanceService))]
	[ServiceBehavior(Namespace = "http://alstom.com/pacis/pis/ground/maintenance/")]
	public class MaintenanceService : IMaintenanceService
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
		private const string SubscriberId = "PIS.Ground.Maintenance.MaintenanceService";

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

		#endregion

		/// <summary>Initializes a new instance of the MaintenanceService class.</summary>
		public MaintenanceService()
		{
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "MaintenanceService";
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

							HistoryLogger.Initialize();

							_t2gManager.SubscribeToElementChangeNotification(SubscriberId, new EventHandler<ElementEventArgs>(OnElementInfoChanged));
							_t2gManager.SubscribeToFilePublishedNotification(SubscriberId, new EventHandler<FilePublishedNotificationArgs>(OnFilesPublished));
							_t2gManager.SubscribeToFileReceivedNotification(SubscriberId, new EventHandler<FileReceivedArgs>(OnFileReceived));

							_transmitThread = new Thread(new ThreadStart(OnTransmitEvent));
                            _transmitThread.Name = "Maint. Transmit";

							_transmitThread.Start();
						}
						catch (System.Exception e)
						{
							LogManager.WriteLog(TraceType.ERROR, e.Message, "PIS.Ground.Maintenance.MaintenanceService.Initialize", e, EventIdEnum.Maintenance);
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
			LogManager.WriteLog(TraceType.INFO, "OnFilesPublished called, folderId=" + status.FolderId + " systemId=" + status.SystemId, "PIS.Ground.Maintenance.MaintenanceService.OnFilesPublished", null, EventIdEnum.Maintenance);

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
						"Maintenance Service Transfer Task",
						FileTransferMode.AnyBandwidth,
						16, //normal priority
						new EventHandler<FileDistributionStatusArgs>(OnFilesDistribution),
						status.FolderId);

					objRequest.SystemId = status.SystemId;
					objRequest.Compression = false;

					String lErrorMessage = _t2gManager.T2GFileDistributionManager.DownloadFolder(objRequest);
					if (!String.IsNullOrEmpty(lErrorMessage))
					{
						LogManager.WriteLog(TraceType.ERROR, "T2G DownloadFolder failure, error " + lErrorMessage, "PIS.Ground.Maintenance.MaintenanceService.OnFilesPublished", null, EventIdEnum.Maintenance);
					}
				}
				else
				{
					LogManager.WriteLog(TraceType.ERROR, "Failure to retrieve folder info, error " + lGetFolderInformationError, "PIS.Ground.Maintenance.MaintenanceService.OnFilesPublished", null, EventIdEnum.Maintenance);
				}
			}
			catch (System.Exception e)
			{
				LogManager.WriteLog(TraceType.ERROR, "Exception thrown", "PIS.Ground.Maintenance.MaintenanceService.OnFilesPublished", e, EventIdEnum.Maintenance);
			}
		}

		/// <summary>
		/// Callback called when File Transfer state changes (signaled by the T2G client)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="status"></param>
		private static void OnFilesDistribution(object sender, FileDistributionStatusArgs status)
		{
			LogManager.WriteLog(TraceType.INFO, "OnFilesDistribution called, folderId=" + status.FolderId + " systemId=" + status.TaskId, "PIS.Ground.Maintenance.MaintenanceService.OnFilesDistribution", null, EventIdEnum.Maintenance);

			try
			{
				if (status.TaskStatus == TaskState.Started && status.CurrentTaskPhase == TaskPhase.Transfer && status.ActiveFileTransferCount == 0)
				{
					FileDistributionRequest lRequest = _t2gManager.T2GFileDistributionManager.GetFileDistributionRequestByTaskId(status.TaskId);
					if (lRequest != null)
					{
						SendNotificationToGroundApp(lRequest.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadDownloading, lRequest.SystemId);
					}
				}
			}
			catch (System.Exception e)
			{
				LogManager.WriteLog(TraceType.ERROR, "Exception thrown", "PIS.Ground.Maintenance.MaintenanceService.OnFilesDistribution", e, EventIdEnum.Maintenance);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="status"></param>
		private static void OnFileReceived(object sender, FileReceivedArgs status)
		{
			LogManager.WriteLog(TraceType.INFO, "OnFileReceived called", "PIS.Ground.Maintenance.MaintenanceService.OnFileReceived", null, EventIdEnum.Maintenance);

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

					SendNotificationToGroundApp(lRequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadCompleted, lParamMap);

					lock (_lock)
					{
						_requestCompletedEvents.Add(new RequestCompletedEvent(lFolderInfo.SystemId, lRequestId));
					}

					_transmitEvent.Set();
				}
				else
				{
					LogManager.WriteLog(TraceType.ERROR, "Failure to retrieve folder info, error " + lGetFolderInformationError, "PIS.Ground.Maintenance.MaintenanceService.OnFileReceived", null, EventIdEnum.Maintenance);
				}
			}
			catch (System.Exception e)
			{
				LogManager.WriteLog(TraceType.ERROR, "Exception thrown", "PIS.Ground.Maintenance.MaintenanceService.OnFileReceived", e, EventIdEnum.Maintenance);
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
			LogManager.WriteLog(TraceType.INFO, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_CALLED), "PIS.Ground.Maintenance.MaintenanceService.SendNotificationToGroundApp", null, EventIdEnum.Maintenance);
			try
			{
				_notificationSender.SendNotification(status, null, requestId);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.ERROR, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_EXCEPTION), "PIS.Ground.Maintenance.MaintenanceService.SendNotificationToGroundApp", ex, EventIdEnum.Maintenance);
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
			LogManager.WriteLog(TraceType.INFO, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_CALLED, "SendNotificationToGroundApp"), "PIS.Ground.Maintenance.MaintenanceService.SendNotificationToGroundApp", null, EventIdEnum.Maintenance);
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
				LogManager.WriteLog(TraceType.ERROR, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_EXCEPTION, "SendNotificationToGroundApp"), "PIS.Ground.Maintenance.MaintenanceService.SendNotificationToGroundApp", ex, EventIdEnum.Maintenance);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public MaintenanceElementListResponse GetAvailableElementList(Guid sessionId)
		{
			MaintenanceElementListResponse result = new MaintenanceElementListResponse();
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
		public MaintenanceResponse GetVersionsFile(Guid sessionId, TargetAddressType targetAddress, uint requestTimeout)
		{
			MaintenanceResponse result = new MaintenanceResponse();
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
						T2GManagerErrorEnum lRqsResult = _t2gManager.GetAvailableElementDataByTargetAddress(targetAddress, out elements);

						switch (lRqsResult)
						{
							case T2GManagerErrorEnum.eSuccess:
								// Queue to request list
								foreach (AvailableElementData element in elements)
								{
									SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadProcessing, element.ElementNumber);

									GetVersionFileRequestContext request = new GetVersionFileRequestContext(element.ElementNumber, requestId, sessionId, requestTimeout);

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
		/// Method to get the log's from start date to end date.
		/// </summary>
		/// <param name="sessionId">Session id to validate the request</param>
		/// <param name="commandList">list of commands</param>
		/// <param name="startDateTime">start date time to be considered</param>
		/// <param name="endDateTime">end date time to be considered</param>
		/// <param name="language">language code</param>
		/// <returns>log response</returns>
		public HistoryLogResponse GetLogs(Guid sessionId, List<CommandType> commandList, DateTime startDateTime, DateTime endDateTime, uint language)
		{
			HistoryLogResponse result = new HistoryLogResponse();
			result.RequestId = Guid.Empty;
			if (_sessionManager.IsSessionValid(sessionId))
			{
				string response = string.Empty;
				if (startDateTime.SafeCompareTo(DateTime.UtcNow) > 0)
				{
					result.ResultCode = ResultCodeEnum.InvalidStartDate;
				}
				else if (startDateTime.SafeCompareTo(endDateTime) > 0)
				{
					result.ResultCode = ResultCodeEnum.InvalidEndDate;
				}
				else if (commandList.Count <= 0)
				{
					result.ResultCode = ResultCodeEnum.InvalidCommandType;
				}
				else
				{
					result.ResultCode = LogManager.GetAllLog(commandList, startDateTime, endDateTime, language, out response);
					result.LogResponse = response;
				}
			}
			else
			{
				result.ResultCode = ResultCodeEnum.InvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// Method to get the oldest log date time.
		/// </summary>
		/// <param name="sessionId">Session id to validate the request</param>
		/// <param name="commandList">list of commands</param>
		/// <returns>log response</returns>
		public HistoryLogResponse GetOldestLogDateTime(Guid sessionId, List<CommandType> commandList)
		{
			HistoryLogResponse result = new HistoryLogResponse();
			result.RequestId = Guid.Empty;
			if (_sessionManager.IsSessionValid(sessionId))
			{
				string response = string.Empty;
				if (commandList.Count <= 0)
				{
					result.ResultCode = ResultCodeEnum.InvalidCommandType;
				}
				else
				{
					result.ResultCode = LogManager.GetOldestLogDateTime(commandList, out response);
					result.LogResponse = response;
				}
			}
			else
			{
				result.ResultCode = ResultCodeEnum.InvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// Method to get the latest log date time.
		/// </summary>
		/// <param name="sessionId">Session id to validate the request</param>
		/// <param name="commandList">list of commands</param>
		/// <returns>log response</returns>
		public HistoryLogResponse GetLatestLogDateTime(Guid sessionId, List<CommandType> commandList)
		{
			HistoryLogResponse result = new HistoryLogResponse();
			result.RequestId = Guid.Empty;
			if (_sessionManager.IsSessionValid(sessionId))
			{
				string response = string.Empty;
				if (commandList.Count <= 0)
				{
					result.ResultCode = ResultCodeEnum.InvalidCommandType;
				}
				else
				{
					result.ResultCode = LogManager.GetLatestLogDateTime(commandList, out response);
					result.LogResponse = response;
				}
			}
			else
			{
				result.ResultCode = ResultCodeEnum.InvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// Method to clean log
		/// </summary>
		/// <param name="sessionId">Session id to validate the request</param>
		/// <param name="commandList">list of commands</param>
		/// <returns>Output response</returns>
		public MaintenanceResponse CleanLog(Guid sessionId, List<CommandType> commandList)
		{
			MaintenanceResponse result = new MaintenanceResponse();
			result.RequestId = Guid.Empty;
			if (_sessionManager.IsSessionValid(sessionId))
			{
				if (commandList.Count <= 0)
				{
					result.ResultCode = ResultCodeEnum.InvalidCommandType;
				}
				else
				{
					result.ResultCode = LogManager.CleanLog(commandList);
				}
			}
			else
			{
				result.ResultCode = ResultCodeEnum.InvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// OBSOLETE - Get the statuses of the baselines current and future.
		/// NOTE: This method is obsolete. Use GetFleetBaselineStatus() instead. 
		/// </summary>
		/// <param name="sessionId">Input session id.</param>
		/// <returns>Baseline Status response.</returns>
		public MaintenanceTrainBaselineStatusListResponse GetTrainBaselineStatus(Guid sessionId)
		{
			return GetFleetBaselineStatus(sessionId);
		}

		/// <summary>
		/// Get the statuses of the baselines current and future for the train fleet.
		/// NOTE: Replaces GetTrainBaselineStatus().
		/// </summary>
		/// <param name="sessionId">Input session id.</param>
		/// <returns>Baseline Status response.</returns>
		public MaintenanceTrainBaselineStatusListResponse GetFleetBaselineStatus(Guid sessionId)
		{
			MaintenanceTrainBaselineStatusListResponse result = new MaintenanceTrainBaselineStatusListResponse();
			if (_sessionManager.IsSessionValid(sessionId))
			{
				Dictionary<string, TrainBaselineStatusData> dictionaryResponse = null;
				result.ResultCode = LogManager.GetTrainBaselineStatus(out dictionaryResponse);

				if (dictionaryResponse != null && dictionaryResponse.Count > 0)
				{
					result.TrainBaselineStatusList = new TrainBaselineStatusList<TrainBaselineStatusData>();
					foreach (TrainBaselineStatusData trainBaselineStatusData in dictionaryResponse.Values)
					{
						result.TrainBaselineStatusList.Add(trainBaselineStatusData);
					}
				}
			}
			else
			{
				result.ResultCode = ResultCodeEnum.InvalidSessionId;
			}

			return result;
		}

		public MaintenanceResponse GetSystemMessagesFiles(Guid sessionId, TargetAddressType targetAddress, uint requestTimeout)
		{
			MaintenanceResponse result = new MaintenanceResponse();

			result.RequestId = Guid.Empty;
			result.ResultCode = ResultCodeEnum.InternalError;

			if (requestTimeout <= MAX_REQUEST_TIMEOUT)
			{
				if (_sessionManager.IsSessionValid(sessionId))
				{
					Guid requestId;
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
									SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadProcessing, element.ElementNumber);

									GetSystemMessagesFilesRequestContext request = new GetSystemMessagesFilesRequestContext(element.ElementNumber, requestId, sessionId, requestTimeout);

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
                        LogManager.WriteLog(TraceType.INFO, "No events to process. Sleeping...", "PIS.Ground.Infotainment.Journaling.JournalingService.OnTransmitEvent", null, EventIdEnum.Maintenance);

                        _transmitEvent.WaitOne();  //wait until new currentRequests

                        LogManager.WriteLog(TraceType.INFO, "Woke up. New events to process...", "PIS.Ground.Infotainment.Journaling.JournalingService.OnTransmitEvent", null, EventIdEnum.Maintenance);
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

                    foreach (RequestContext request in currentRequests)
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
                                SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadTimedOut, lParameters);
                                LogManager.WriteLog(TraceType.INFO, "Request expired.", "PIS.Ground.Maintenance.MaintenanceService.OnTransmitEvent", null, EventIdEnum.Maintenance);
                                break;

                            case RequestState.Transmitted:
                                break;

                            case RequestState.AllRetriesExhausted:
                                break;

                            case RequestState.Completed:
                                LogManager.WriteLog(TraceType.INFO, "Processing GetReportRequest... completed", "PIS.Ground.Maintenance.MaintenanceService.OnTransmitEvent", null, EventIdEnum.Maintenance);
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
                LogManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.Maintenance.MaintenanceService.OnTransmitEvent", exception, EventIdEnum.Maintenance);
            }
		}

		private static void TransmitRequest(RequestContext request)
		{
			if (request is GetVersionFileRequestContext)
			{
				TransmitGetVersionFileRequest(request as GetVersionFileRequestContext);
			}
			else if (request is GetSystemMessagesFilesRequestContext)
			{
				TransmitGetSystemMessagesFilesRequest(request as GetSystemMessagesFilesRequestContext);

			}
			else
			{
				// Other request types
				LogManager.WriteLog(TraceType.ERROR, "Invalid request type.", "PIS.Ground.Infotainment.Journaling.JournalingService.OnTransmitEvent", null, EventIdEnum.Maintenance);
			}

			if (request.State == RequestState.WaitingRetry && request.TransferAttemptsDone == 1)
			{
				// first attempt failed, send notification
				Dictionary<string, string> lParameters = new Dictionary<string, string>();
				lParameters.Add(NOTIFICATION_PARAMETER_ELEMENT_ID, request.ElementId);
				SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadWaitingToSend, lParameters);
			}
		}

		private static void TransmitGetVersionFileRequest(GetVersionFileRequestContext request)
		{
			bool lTransmitted = false;

			ServiceInfo serviceInfo;
			T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableServiceData(request.ElementId, (int)eServiceID.eSrvSIF_MaintenanceServer, out serviceInfo);
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
							if (true == lIsElementOnline)
							{
								// Call Report Exchange train service
								using (MaintenanceServiceClient client = new MaintenanceServiceClient("MaintenanceEndpoint", lEndpoint))
								{
									try
									{
										String requestIdStr = request.RequestId.ToString();

										// Call onboard train Report Exchange Web service
										Train.ResultType result;
										client.DownloadSoftwarePackageVersions(ref requestIdStr, out result);

										// TODO (???) extract Folder ID from request - Check with Malik request.FolderId = result.folderId

										switch (result)
										{
											case ResultType.ServiceInhibited:
												SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadInhibited, request.ElementId);
												request.CompletionStatus = true;
												break;
											case ResultType.Failure:
												SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.Failed, request.ElementId);
												break;
											case ResultType.Success:
												lTransmitted = true;
												break;
										}
									}
									catch (Exception ex)
									{
										LogManager.WriteLog(TraceType.ERROR,
											String.Format("Unexpected error from WebService client: {0}", ex.Message),
											"PIS.Ground.Maintenance.MaintenanceService.TransmitGetVersionFileRequest",
											ex, EventIdEnum.Maintenance);

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
							SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceT2GServerOffline, "");
							break;
						case T2GManagerErrorEnum.eElementNotFound:
							SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadElementNotFound, request.ElementId);
							break;
						default:
							break;
					}
					break;
				case T2GManagerErrorEnum.eT2GServerOffline:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceT2GServerOffline, "");
					break;
				case T2GManagerErrorEnum.eElementNotFound:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadElementNotFound, request.ElementId);
					break;
				case T2GManagerErrorEnum.eServiceInfoNotFound:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadServiceNotFound, eServiceID.eSrvSIF_MaintenanceServer.ToString());
					break;
				default:
					LogManager.WriteLog(TraceType.INFO, "Failed to obtain service data from T2G", "PIS.Ground.Maintenance.MaintenanceService.TransmitGetVersionFileRequest", null, EventIdEnum.Maintenance);
					break;
			}
			request.TransmissionStatus = lTransmitted;
		}

		private static void TransmitGetSystemMessagesFilesRequest(GetSystemMessagesFilesRequestContext request)
		{
			bool lTransmitted = false;

			ServiceInfo serviceInfo;
			T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableServiceData(request.ElementId, (int)eServiceID.eSrvSIF_MaintenanceServer, out serviceInfo);
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
							if (true == lIsElementOnline)
							{
								// Call Report Exchange train service
								using (MaintenanceServiceClient client = new MaintenanceServiceClient("MaintenanceEndpoint", lEndpoint))
								{
									try
									{
										String requestIdStr = request.RequestId.ToString();

										// Call onboard train Report Exchange Web service
										Train.ResultType result;
										client.DownloadSystemMessageFiles(ref requestIdStr, out result);

										switch (result)
										{
											case ResultType.ServiceInhibited:
												SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadInhibited, request.ElementId);
												request.CompletionStatus = true;
												break;
											case ResultType.Failure:
												SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.Failed, request.ElementId);
												break;
											case ResultType.Success:
												lTransmitted = true;
												break;
										}
									}
									catch (Exception ex)
									{
										LogManager.WriteLog(TraceType.ERROR,
											String.Format("Unexpected error from WebService client: {0}", ex.Message),
											"PIS.Ground.Maintenance.MaintenanceService.TransmitGetSystemMessagesFilesRequest",
											ex, EventIdEnum.Maintenance);

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
							SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceT2GServerOffline, "");
							break;
						case T2GManagerErrorEnum.eElementNotFound:
							SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadElementNotFound, request.ElementId);
							break;
						default:
							break;
					}
					break;
				case T2GManagerErrorEnum.eT2GServerOffline:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceT2GServerOffline, "");
					break;
				case T2GManagerErrorEnum.eElementNotFound:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadElementNotFound, request.ElementId);
					break;
				case T2GManagerErrorEnum.eServiceInfoNotFound:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MaintenanceDownloadServiceNotFound, request.ElementId);
					break;
				default:
					LogManager.WriteLog(TraceType.INFO, "Failed to obtain service data from T2G", "PIS.Ground.Maintenance.MaintenanceService.TransmitGetVersionFileRequest", null, EventIdEnum.Maintenance);
					break;
			}


			request.TransmissionStatus = lTransmitted;
		}

		internal static void OnTrainNotification(Guid pRequestId,
												 PIS.Ground.Maintenance.Notification.NotificationIdEnum pNotificationId,
												 string pElementId,
												 string pParameter)
		{
			try
			{
				switch (pNotificationId)
				{
					case PIS.Ground.Maintenance.Notification.NotificationIdEnum.Completed:
						//nothing to do
						break;

					case PIS.Ground.Maintenance.Notification.NotificationIdEnum.Failed:

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
								LogManager.WriteLog(TraceType.ERROR, "Failure to retrieve folder info, error " + lGetFolderInformationError, "PIS.Ground.Maintenance.MaintenanceService.OnTrainNotification", null, EventIdEnum.Maintenance);
							}
						}
						else
						{
							LogManager.WriteLog(TraceType.ERROR, "Invalid folder ID as notification parameter, actual value [" + pParameter + "]", "PIS.Ground.Maintenance.MaintenanceService.OnTrainNotification", null, EventIdEnum.Maintenance);
						}
						break;
				}

			}
			catch (System.Exception e)
			{
				LogManager.WriteLog(TraceType.ERROR, "Exception thrown", "PIS.Ground.Maintenance.MaintenanceService.OnTrainNotification", e, EventIdEnum.Maintenance);
			}
		}

		/// <summary>
		///
		/// </summary>
		internal class GetVersionFileRequestContext : RequestContext
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="requestId"></param>
			/// <param name="sessionId"></param>
			/// <param name="targetAddress"></param>
			/// <param name="timeout">Request timeout in minutes</param>
			public GetVersionFileRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout)
				: base("", elementId, requestId, sessionId, timeout)
			{
			}
		}
		/// <summary>
		///
		/// </summary>
		internal class GetSystemMessagesFilesRequestContext : RequestContext
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="requestId"></param>
			/// <param name="sessionId"></param>
			/// <param name="targetAddress"></param>
			/// <param name="timeout">Request timeout in minutes</param>
			public GetSystemMessagesFilesRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout)
				: base("", elementId, requestId, sessionId, timeout)
			{
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
}
