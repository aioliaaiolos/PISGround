//---------------------------------------------------------------------------------------------------
// <copyright file="MissionService.svc.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.IO;
using System.Globalization;

using PIS.Ground.Core;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.T2G;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.PackageAccess;
using PIS.Ground.Core.Utility;
using PIS.Ground.RemoteDataStore;
using PIS.Ground.Common;
using PIS.Ground.GroundCore.AppGround;
using PIS.Train.Mission;
using Mission;

namespace PIS.Ground.Mission
{
	/// <summary> Mission Service </summary>
	[CreateOnDispatchService(typeof(MissionService))]
	[ServiceBehavior(Namespace = "http://alstom.com/pacis/pis/ground/mission/")]
	public class MissionService : IMissionService
	{
		#region consts

		private const string NOTIFICATION_PARAMETER_ELEMENT_ID = "elementId";

		private const string NOTIFICATION_PARAMETER_SERVICE_ID = "serviceId";

		private const string NOTIFICATION_PARAMETER_STATION_LIST = "stationList";

		private const uint MAX_REQUEST_TIMEOUT = 43200;

		/// <summary>Identifier for event subscription.</summary>
		private const string SubscriberId = "PIS.Ground.MissionControl.MissionService";

		#endregion

		#region static fields

		private static volatile bool _initialized = false;

		private static object _initializationLock = new object();
		private static AutoResetEvent _transmitEvent = new AutoResetEvent(false);

		private static Thread _transmitThread = null;

		private static IT2GManager _t2gManager = null;

		private static ISessionManager _sessionManager = null;

		private static INotificationSender _notificationSender = null;

		private static IRemoteDataStoreFactory _remoteDataStoreFactory = null;

		private static Object _lock = new Object();

		private static List<MissionRequestContext> _newRequests = new List<MissionRequestContext>();

		private static List<RequestCompletedEvent> _requestCompletedEvents = new List<RequestCompletedEvent>();

		/// <summary>Type of the plateform.</summary>
		protected static CommonConfiguration.PlatformTypeEnum? _plateformType = null;

		#endregion

		/// <summary>Initializes a new instance of the MissionService class.</summary>
		public MissionService()
		{
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "MissionService";
            }

			Initialize();
		}

		#region private static methods

		/// <summary>
		/// Callback called when Element Online state changes (signaled by the T2G Client)
		/// </summary>
		private static void OnElementInfoChanged(object sender, ElementEventArgs args)
		{
			//Signal the event to start handling the request.
			_transmitEvent.Set();
		}

		/// <summary>
		/// This function allows to write a message in the Windows Application Log
		/// </summary>
		/// <param name="traceType"> Message type </param>
		/// <param name="context"> Message context </param>
		/// <param name="ex"> Related exception </param>
		/// <param name="message"> Message text. Can be an Id of a message from resources </param>
		/// <param name="args">Arguments that should be put in the message</param>
		private static void WriteLog(TraceType messageType, string context, Exception ex, string message, params object[] args)
		{
			try
			{
				LogManager.WriteLog(
					messageType,
					string.Format(CultureInfo.CurrentCulture, message, args),
					"PIS.Ground.Mission.MissionService" + context,
					ex,
					EventIdEnum.Mission);
			}
			catch (Exception lEx)
			{
				LogManager.WriteLog(
					TraceType.EXCEPTION,
					ex.Message,
					"PIS.Ground.Mission.MissionService",
					lEx,
					EventIdEnum.Mission);
			}
		}

		/// <summary>
		/// This function allows the Pis Ground to send a notification to a Ground Application
		/// with an element Id as a parameter
		/// </summary>
		/// <param name="requestId"> request ID corresponding to the specified request </param>
		/// <param name="notificationId"> Notification Id </param>        
		/// <param name="elementId"> Element identifier </param>        
		private static void SendNotificationToGroundApp(
			Guid requestId,
			NotificationIdEnum notificationId,
			string elementId)
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();

			parameters.Add(NOTIFICATION_PARAMETER_ELEMENT_ID, elementId);

			SendNotificationToGroundApp(requestId, notificationId, parameters);
		}

		/// <summary>
		/// This function allows the Pis Ground to send a notification to a Ground Application
		/// with an parameter and its value
		/// </summary>
		/// <param name="requestId"> request ID corresponding to the specified request </param>
		/// <param name="notificationId"> Notification Id </param>        
		/// <param name="paramName"> Param's name </param>        
		/// <param name="paramValue"> Param's value </param>        
		private static void SendNotificationToGroundApp(
			Guid requestId,
			NotificationIdEnum notificationId,
			string paramName,
			string paramValue)
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();

			parameters.Add(paramName, paramValue);

			SendNotificationToGroundApp(requestId, notificationId, parameters);
		}

		/// <summary>
		/// This function allows the Pis Ground to send a notification to a Ground Application
		/// with a set of parameters  
		/// </summary>
		/// <param name="requestId"> request ID corresponding to the specified request </param>
		/// <param name="notificationId"> Notification Id </param>
		/// <param name="parameters">parameters: notification parameters. Key-value pairs</param>
		/// </summary>
		private static void SendNotificationToGroundApp(
			Guid requestId,
			NotificationIdEnum notificationId,
			System.Collections.Generic.Dictionary<string, string> parameters)
		{
			List<string> paramList = new List<string>();

			foreach (KeyValuePair<string, string> lKeyValue in parameters)
			{
				paramList.Add(lKeyValue.Key);
				paramList.Add(lKeyValue.Value);
			}

			SendNotificationToGroundApp(requestId, notificationId, paramList);
		}

		/// <summary>
		/// This function allows the Pis Ground to send a notification to a Ground Application
		/// with a set of parameters  
		/// </summary>
		/// <param name="requestId"> request ID corresponding to the specified request </param>
		/// <param name="notificationId"> Notification Id </param>
		/// <param name="parameters"> parameters: notification parameters. List of strings </param>
		private static void SendNotificationToGroundApp(
			Guid requestId,
			NotificationIdEnum notificationId,
			System.Collections.Generic.List<string> parameters)
		{
			WriteLog(TraceType.INFO, "SendNotificationToGroundApp", null, Logs.INFO_FUNCTION_CALLED, "SendNotificationToGroundApp");

			try
			{
				System.Xml.Serialization.XmlSerializer lXmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(List<string>));

				StringWriter notifParams = new StringWriter();
				lXmlSerializer.Serialize(notifParams, parameters);

				//Send Notification to AppGround
				_notificationSender.SendNotification(notificationId, notifParams.ToString(), requestId);
			}
			catch (Exception ex)
			{
				WriteLog(TraceType.ERROR, "SendNotificationToGroundApp", ex, Logs.INFO_FUNCTION_EXCEPTION, "SendNotificationToGroundApp");
			}
		}

		/// <summary>
		/// This function allows the Pis Ground to send a notification to a Ground Application
		/// <param name="requestId"> Request ID corresponding to the specified request </param>
		/// <param name="notificationId"> Notification Id </param>
		/// </summary>
		private static void SendNotificationToGroundApp(Guid requestId, NotificationIdEnum notificationId)
		{
			WriteLog(TraceType.INFO, "SendNotificationToGroundApp", null, Logs.INFO_FUNCTION_CALLED, "SendNotificationToGroundApp");

			try
			{
				//send notification to ground app
				_notificationSender.SendNotification(notificationId, requestId);
			}
			catch (Exception ex)
			{
				WriteLog(TraceType.ERROR, "SendNotificationToGroundApp", ex, Logs.INFO_FUNCTION_EXCEPTION, "SendNotificationToGroundApp");
			}
		}

		/// <summary>
		/// Used by _transmitThread thread to process the commands that are sent to the train
		/// </summary>
		private static void OnTransmitEvent()
		{
            try
            {
                List<MissionRequestContext> currentRequests = new List<MissionRequestContext>();

                while (true)
                {
                    if (currentRequests.Count == 0)
                    {
                        //Wait for a new request
                        _transmitEvent.WaitOne();
                    }

                    lock (_lock)
                    {
                        // Move pending from _newRequests to currentRequests                    
                        currentRequests.AddRange(_newRequests);

                        _newRequests.Clear();
                    }

                    foreach (var request in currentRequests)
                    {
                        if (request != null)
                        {
                            switch (request.State)
                            {
                                case RequestState.ReadyToSend:
                                    TransmitRequest(request);
                                    break;

                                case RequestState.Expired:
                                    SendNotificationToGroundApp(
                                        request.RequestId,
                                        NotificationIdEnum.MissionCommandProgressTimedOut,
                                        request.ElementId);
                                    break;
                            }
                        }
                        else
                        {
                            WriteLog(TraceType.ERROR, "OnTransmitEvent", null, Logs.ERROR_REQUEST_CONTEXT_IS_NULL);
                        }
                    }

                    lock (_lock)
                    {
                        foreach (RequestCompletedEvent evt in _requestCompletedEvents)
                        {
                            List<MissionRequestContext> lReqList = currentRequests.FindAll(
                                delegate(MissionRequestContext c) { return c.ElementId == evt.elementId && c.RequestId == evt.requestId; });

                            foreach (RequestContext lRequest in lReqList)
                            {
                                lRequest.CompletionStatus = true;
                            }
                        }

                        _requestCompletedEvents.Clear();
                    }

                    currentRequests.RemoveAll(delegate(MissionRequestContext c) { return c == null || c.State == RequestState.Completed || c.State == RequestState.Expired; });

                    Thread.Sleep(1000);
                }
            }
            catch (ThreadAbortException)
            {
                // No logic to apply.
            }
            catch (System.Exception exception)
            {
                WriteLog(TraceType.EXCEPTION, "OnTransmitEvent", exception, exception.Message);
            }
		}

		/// <summary> Updates the Request state to be completed </summary>
		/// <param name="pRequestId">Request Id related to the request</param>
		/// <param name="pElementId">Element Id related to the request</param>
		private static void UpdateRequestStateToCompleted(Guid pRequestId, string pElementId)
		{
			lock (_lock)
			{
				_requestCompletedEvents.Add(new RequestCompletedEvent(pElementId, pRequestId));
			}
		}

		/// <summary> Processes mission request </summary>
		/// <param name="request">Request</param>
		private static void TransmitRequest(MissionRequestContext request)
		{
			if (request is InitializeMissionRequest)
			{
				TransmitInitializeMissionRequest(request as InitializeMissionRequest);
			}
			else if (request is CancelMissionRequest)
			{
				TransmitCancelMissionRequest(request as CancelMissionRequest);
			}
			else
			{
				// Other request types
				WriteLog(TraceType.ERROR, "TransmitRequest", null, Logs.ERROR_INVALID_REQUEST_TYPE);
			}

			if (request.State == RequestState.WaitingRetry && request.TransferAttemptsDone == 1)
			{
				// first attempt failed, send notification
				SendNotificationToGroundApp(request.RequestId, NotificationIdEnum.MissionCommandProgressWaitingToSend, request.ElementId);
			}
		}

		/// <summary> Transmits the InitializeMission request to train </summary>
		/// <param name="request">Request params</param>
		private static void TransmitInitializeMissionRequest(InitializeMissionRequest request)
		{
			bool lTransmitted = false;

			ServiceInfo lServiceInfo;
			T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableServiceData(request.ElementId, (int)eServiceID.eSrvSIF_MissionServer, out lServiceInfo);

			switch (lRqstResult)
			{
				case T2GManagerErrorEnum.eSuccess:
					{
						String lEndpoint = "http://" + lServiceInfo.ServiceIPAddress + ":" + lServiceInfo.ServicePortNumber;

						// Check if target element is online
						bool lIselementOnline;
						lRqstResult = _t2gManager.IsElementOnline(request.ElementId, out lIselementOnline);

						switch (lRqstResult)
						{
							case T2GManagerErrorEnum.eSuccess:
								{
									if (lIselementOnline == true)
									{
										// Call Mission train service                                                                                
										using (MissionControlServiceClient client = new MissionControlServiceClient("MissionEndpoint", lEndpoint))
										{
											try
											{
												PIS.Train.Mission.ResultType result = client.InitializeMission(
													request.RequestId.ToString(),
													request.ElementId,
													request.MissionCode,
													request.LMTDbVersion,
													request.StationList);

												SendNotificationToGroundApp(
													request.RequestId,
													NotificationIdEnum.MissionCommandProgressSent,
													request.ElementId);

												ProcessInitializeMissionRequestResult(request, result);

												lTransmitted = true;
											}
											catch (Exception ex)
											{
												WriteLog(TraceType.ERROR, "TransmitInitializeMIssionRequest", ex, ex.Message);
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
								}
								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								SendNotificationToGroundApp(
												request.RequestId,
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline);
								break;
							case T2GManagerErrorEnum.eElementNotFound:
								SendNotificationToGroundApp(
												request.RequestId,
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownElementId,
												request.ElementId);
								break;
							default:
								break;
						}
					}
					break;
				case T2GManagerErrorEnum.eT2GServerOffline:
					SendNotificationToGroundApp(
												request.RequestId,
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline);
					break;
				case T2GManagerErrorEnum.eServiceInfoNotFound:
					SendNotificationToGroundApp(
												request.RequestId,
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownServiceId,
												NOTIFICATION_PARAMETER_SERVICE_ID,
												eServiceID.eSrvSIF_MissionServer.ToString());
					break;
				default:
					break;
			}
			request.TransmissionStatus = lTransmitted;
		}

		/// <summary> Transmits the CancelMission request to train </summary>
		/// <param name="request">Request params</param>
		private static void TransmitCancelMissionRequest(CancelMissionRequest request)
		{
			bool lTransmitted = false;

			ServiceInfo lServiceInfo;
			T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableServiceData(request.ElementId, (int)eServiceID.eSrvSIF_MissionServer, out lServiceInfo);
			switch (lRqstResult)
			{
				case T2GManagerErrorEnum.eSuccess:
					{
						String lEndpoint = "http://" + lServiceInfo.ServiceIPAddress + ":" + lServiceInfo.ServicePortNumber;

						// Check if target element is online
						bool lIselementOnline;
						lRqstResult = _t2gManager.IsElementOnline(request.ElementId, out lIselementOnline);

						switch (lRqstResult)
						{
							case T2GManagerErrorEnum.eSuccess:
								{
									if (lIselementOnline == true)
									{
										// Call Mission train service
										using (MissionControlServiceClient client = new MissionControlServiceClient("MissionEndpoint", lEndpoint))
										{
											try
											{
												PIS.Train.Mission.ResultType result = client.CancelMission(
													request.RequestId.ToString(),
													request.ElementId,
													request.MissionCode);

												SendNotificationToGroundApp(
													request.RequestId,
													NotificationIdEnum.MissionCommandProgressSent,
													request.ElementId);

												ProcessCancelMissionRequestResult(request, result);

												lTransmitted = true;
											}
											catch (Exception ex)
											{
												WriteLog(TraceType.ERROR, "TransmitcancelMissionRequest", ex, ex.Message);
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
								}
								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								SendNotificationToGroundApp(
												request.RequestId,
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline);
								break;
							case T2GManagerErrorEnum.eElementNotFound:
								SendNotificationToGroundApp(
												request.RequestId,
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownElementId,
												request.ElementId);
								break;
							default:
								break;
						}
					}
					break;
				case T2GManagerErrorEnum.eT2GServerOffline:
					SendNotificationToGroundApp(
												request.RequestId,
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline);
					break;
				case T2GManagerErrorEnum.eServiceInfoNotFound:
					SendNotificationToGroundApp(
												request.RequestId,
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownServiceId,
												NOTIFICATION_PARAMETER_SERVICE_ID,
												eServiceID.eSrvSIF_MissionServer.ToString());
					break;
				default:
					break;
			}
			request.TransmissionStatus = lTransmitted;
		}

		/// <summary> Processes the InitializeMission request result </summary>
		/// <param name="request">Request params</param>
		/// <param name="result">Result code</param>
		private static void ProcessInitializeMissionRequestResult(InitializeMissionRequest request, PIS.Train.Mission.ResultType result)
		{
			NotificationIdEnum notification = NotificationIdEnum.Failed;

			switch (result)
			{
				case ResultType.Success:
					notification = NotificationIdEnum.MissionCommandProgressSuccess;
					break;

				case ResultType.ErrorInvalidElementId:
					notification = NotificationIdEnum.MissionCommandProgressUnknownElementId;
					break;

				case ResultType.ErrorUnknownLmtDb:
					notification = NotificationIdEnum.MissionCommandProgressLmtDataPackageVersionDifferent;
					break;

				case ResultType.ErrorServiceInhibited:
					notification = NotificationIdEnum.MissionCommandProgressServiceInhibited;
					break;

				case ResultType.ErrorActiveMission:
					notification = NotificationIdEnum.MissionCommandProgressActiveMissionOnElement;
					break;
			}

			//If there is an unexpected result code - log it
			WriteLog(TraceType.WARNING, "ProcessInitializeMissionRequest", null, Logs.INFO_FUNCTION_RESULT, "InitializeMission", result.ToString());

			if (result == ResultType.Success && request.StationList != null && request.StationList.Count > 0)
			{
				List<string> parameters = new List<string>();

				parameters.Add(NOTIFICATION_PARAMETER_ELEMENT_ID);
				parameters.Add(request.ElementId);
				parameters.Add(NOTIFICATION_PARAMETER_STATION_LIST);

				request.StationList.ForEach((station) => parameters.Add(station));

				SendNotificationToGroundApp(request.RequestId, notification, parameters);
			}
			else
			{
				SendNotificationToGroundApp(request.RequestId, notification, request.ElementId);
			}

			// Any result from the train means that the request is completed and we can remove it.
			lock (_lock)
			{
				_requestCompletedEvents.Add(new RequestCompletedEvent(request.ElementId, request.RequestId));
			}

			_transmitEvent.Set();
		}

		/// <summary> Processes the CancelMission request result </summary>
		/// <param name="request">Request params</param>
		/// <param name="result">Result code</param>
		private static void ProcessCancelMissionRequestResult(CancelMissionRequest request, PIS.Train.Mission.ResultType result)
		{
			NotificationIdEnum notification = NotificationIdEnum.Failed;

			switch (result)
			{
				case ResultType.Success:
					notification = NotificationIdEnum.MissionCommandProgressSuccess;
					break;

				case ResultType.ErrorInvalidElementId:
					notification = NotificationIdEnum.MissionCommandProgressUnknownElementId;
					break;

				case ResultType.ErrorMissionCodeDifferent:
					notification = NotificationIdEnum.MissionCommandProgressMissionIdDifferent;
					break;

				case ResultType.ErrorServiceInhibited:
					notification = NotificationIdEnum.MissionCommandProgressServiceInhibited;
					break;

				case ResultType.ErrorNoActiveMission:
					notification = NotificationIdEnum.MissionCommandProgressNoActiveMission;
					break;
			}

			//If there is an unexpected result code - log it
			WriteLog(TraceType.WARNING, "ProcessCancelMissionRequest", null, Logs.INFO_FUNCTION_RESULT, "CancelMission", result.ToString());

			SendNotificationToGroundApp(request.RequestId, notification, request.ElementId);

			// Any result from the train means that the request is completed and we can remove it.
			lock (_lock)
			{
				_requestCompletedEvents.Add(new RequestCompletedEvent(request.ElementId, request.RequestId));
			}

			_transmitEvent.Set();
		}

		/// <summary>
		/// Callback called to process the train notifications
		/// </summary>
		/// <param name="requestId">Request Id</param>
		/// <param name="notificationId">Notification Id</param>
		/// <param name="elementId">The Id of element that has sent the notification</param>
		/// <param name="parameter">Notification parameter</param>                
		internal static void OnTrainNotification(
			Guid requestId,
			PIS.Ground.Mission.Notification.NotificationIdEnum notificationId,
			string elementId,
			string parameter)
		{
			//The current version of MissionControl Train Service doesn't send any notifications
			//So, this function will remain empty at this moment
			//There will be just a trace in Windows Events Log in case if however any notification arrives
			WriteLog(TraceType.INFO, "OnTrainNotification", null, Logs.INFO_TRAIN_NOTIFICATION, notificationId);
		}

		/// <summary>Searches for the first lmt database file path.</summary>
		/// <param name="packagePath">Full pathname of the package file.</param>
		/// <returns>The found lmt database file path.</returns>
		private static string FindLmtDatabaseFilePath(string packagePath)
		{
			string lmtDatabaseFilePath = string.Empty;

			try
			{
				string[] fileNames = Directory.GetFiles(packagePath, "*.db", SearchOption.AllDirectories);
				if (fileNames != null && fileNames.Count() > 0)
				{
					lmtDatabaseFilePath = fileNames[0]; // select the first
				}
			}
			catch (Exception ex)
			{
				lmtDatabaseFilePath = string.Empty;

				WriteLog(TraceType.ERROR, "FindLmtDatabaseFilePath", ex, ex.Message);
			}

			return lmtDatabaseFilePath;
		}

		#endregion private static methods

		#region private methods

		/// <summary> Validates the InitializeMission request </summary>
		/// <param name="element">Element, on which the mission should be initialized</param>
		/// <param name="missionCode">Code of mission that should be initialized on the element</param>
		/// <param name="stationList">Station list, that will be used to initialize a new mission
		/// in case if there is no mission with the provided mission code </param>
		/// <param name="missionCodeExists">output. 'true' if provided missionCode exists in the LMT DB. 'false' - otherwise </param>
		/// <param name="invalidStationList">output. List of stations that don't exist in the current LMT DB</param>
		/// <param name="LMTDbVersion">output. The current LMT DB version</param>
		/// <returns>Validation result</returns>
		private MissionServiceErrorCodeEnum ValidateInitializeMissionRequest(
			AvailableElementData element,
			string missionCode,
			List<string> stationList,
			out bool missionCodeExists,
			out List<string> invalidStationList,
			out string LMTDbVersion)
		{
			MissionServiceErrorCodeEnum result = MissionServiceErrorCodeEnum.RequestAccepted;
			missionCodeExists = false;
			LMTDbVersion = "0.0.0.0";
			invalidStationList = new List<string>();

			//Check the provided mission code. 
			//If it is null or empty - return the Invalid Mission Code error
			if (!string.IsNullOrEmpty(missionCode))
			{
				//Check if there is a mission in the current LMT DB with the code provided.
				//If there is no such mission - validate the station list provided
				if (element != null &&
					!string.IsNullOrEmpty(element.LmtPackageVersion) &&
					element.LmtPackageVersion != "0.0.0.0")
				{
					LMTDbVersion = element.LmtPackageVersion;

					try
					{
						using (var remoteDataStore = _remoteDataStoreFactory.GetRemoteDataStoreInstance() as RemoteDataStoreProxy)
						{
							if (remoteDataStore != null)
							{
								//Check if the LMT package that is used on the element exists in the Remote Datastore
								if (remoteDataStore.checkIfDataPackageExists("LMT", element.LmtPackageVersion))
								{
									//Try to open LMT package
									var openPackageResult = remoteDataStore.openLocalDataPackage(
										"LMT",
										element.LmtPackageVersion,
										String.Empty);

									if (openPackageResult.Status == OpenDataPackageStatusEnum.COMPLETED)
									{
										//Get the LMT DB file path 
										string lmtDatabaseFilePath = FindLmtDatabaseFilePath(openPackageResult.LocalPackagePath);

										if (!string.IsNullOrEmpty(lmtDatabaseFilePath))
										{
											try
											{
												using (var dbAccess = new LmtDatabaseAccessor(lmtDatabaseFilePath, _plateformType))
												{
													//Check if there is a mission in the LMT DB with the mission code provided
													uint? missionInternalId = dbAccess.GetMissionInternalCodeFromOperatorCode(missionCode);

													//If there is a mission in LMT DB with the provided mission code - 
													//there is no need to check the station list
													if (missionInternalId != null)
													{
														missionCodeExists = true;

														result = MissionServiceErrorCodeEnum.RequestAccepted;
													}
													else
													{
														//If there is no mission in LMT DB with the provided mission code - 
														//validate the station list provided
														if (stationList == null || stationList.Count == 0)
														{
															//If there is no station list nor mission code that exists in LMT DB - request isn't valid
															result = MissionServiceErrorCodeEnum.ErrorInvalidMissionCode;
														}
														else
														{
															//if there is a station in stationList that doesn't exist in LMT DB - add it to invalidStationList                                                
															foreach (string station in stationList)
															{
																uint? insertionStationInternalId = dbAccess.GetStationInternalCodeFromOperatorCode(station);

																if (insertionStationInternalId == null)
																{
																	invalidStationList.Add(station);
																}
															}

															if (invalidStationList.Count != 0)
															{
																//If there is at least 1 station with unknown station Id - request isn't valid
																result = MissionServiceErrorCodeEnum.ErrorInvalidStationId;
															}
															else
															{
																//if the station list is ok and there is no any mission with the provided mission code - request is valid                                                            
																result = MissionServiceErrorCodeEnum.RequestAccepted;
															}
														}
													}
												}
											}
											catch (Exception ex)
											{
												WriteLog(
													TraceType.ERROR,
													"PIS.Ground.Mission.MissionService.ValidateInitializeMissionRequest",
													ex,
													"Error reading LMT database (" + lmtDatabaseFilePath + ")");

												result = MissionServiceErrorCodeEnum.ErrorOpeningLMTDb;
											}
										}
										else
										{
											result = MissionServiceErrorCodeEnum.ErrorOpeningLMTDb;
										}
									}
									else
									{
										result = MissionServiceErrorCodeEnum.ErrorOpeningLMTDb;
									}
								}
								else
								{
									result = MissionServiceErrorCodeEnum.ErrorOpeningLMTDb;
								}
							}
							else
							{
								result = MissionServiceErrorCodeEnum.ErrorRemoteDatastoreUnavailable;
								WriteLog(TraceType.ERROR, "PIS.Ground.Mission.MissionService.ValidateInitializeMissionRequest", null, Logs.ERROR_REMOTE_DATASTORE_NOT_ACCESSIBLE);
							}
						}
					}
					catch (Exception ex)
					{
						WriteLog(TraceType.ERROR, "PIS.Ground.Mission.MissionService.ValidateInitializeMissionRequest", ex, ex.Message);

						result = MissionServiceErrorCodeEnum.ErrorRemoteDatastoreUnavailable;
					}
				}
				else
				{
					//The LMT DB used on the element is unknown
					result = MissionServiceErrorCodeEnum.ErrorUnknownLMTDb;
				}
			}
			else
			{
				//Mission code is null or empty
				result = MissionServiceErrorCodeEnum.ErrorInvalidMissionCode;
			}

			return result;
		}

		#endregion private methods

		#region public static methods

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

							// Register to receive a notification when an element Online State changes.
							_t2gManager.SubscribeToElementChangeNotification(SubscriberId, new EventHandler<ElementEventArgs>(OnElementInfoChanged));

							_remoteDataStoreFactory = new RemoteDataStoreFactory();

							_transmitThread = new Thread(new ThreadStart(OnTransmitEvent));
                            _transmitThread.Name = "Mission Transmit";

							_transmitThread.Start();
						}
						catch (System.Exception e)
						{
							LogManager.WriteLog(TraceType.ERROR, e.Message, "PIS.Ground.Mission.MissionService.Initialize", e, EventIdEnum.Mission);
						}

                        _initialized = true;
					}
				}
			}
		}

		/// <summary>Initializes this object.</summary>
		public static void Initialize(
			IT2GManager t2gManager,
			ISessionManagerExtended sessionManager,
			INotificationSender notificationSender,
			IRemoteDataStoreFactory remoteDataStoreFactory)
		{
			try
			{
				if (t2gManager != null)
				{
					MissionService._t2gManager = t2gManager;

					// Register to receive a notification when an element Online State changes.
					MissionService._t2gManager.SubscribeToElementChangeNotification(SubscriberId, new EventHandler<ElementEventArgs>(OnElementInfoChanged));
				}

				if (sessionManager != null)
				{
					MissionService._sessionManager = sessionManager;
				}

				if (notificationSender != null)
				{
					MissionService._notificationSender = notificationSender;
				}

				if (remoteDataStoreFactory != null)
				{
					MissionService._remoteDataStoreFactory = remoteDataStoreFactory;
				}

				_transmitThread = new Thread(new ThreadStart(OnTransmitEvent));
                _transmitThread.Name = "Mission Transmit";

				_transmitThread.Start();

				_initialized = true;
			}
			catch (System.Exception e)
			{
				LogManager.WriteLog(TraceType.ERROR, e.Message, "PIS.Ground.Mission.MissionService.Initialize", e, EventIdEnum.Mission);
			}
		}

		#endregion public static methods

		#region public methods

		/// <summary>
		/// This function allows the GroundApp to request from the ground PIS the list of available
		/// elements. This list includes also missions that are running for each element, and the
		/// versions of the LMT and PIS Base data packages
		/// </summary>
		/// <param name="sessionId">A valid session identifier or 0, if the provided credentials
		/// are not valid
		/// </param>
		/// <returns>
		/// The code “request accepted” when the command is valid and the list of elements, or and
		/// error code when the command is rejected
		/// </returns>
		MissionAvailableElementListResult IMissionService.GetAvailableElementList(Guid sessionId)
		{
			MissionAvailableElementListResult result = new MissionAvailableElementListResult();

			result.ResultCode = MissionServiceErrorCodeEnum.RequestAccepted;

			if (_sessionManager.IsSessionValid(sessionId))
			{
				T2GManagerErrorEnum lResult = _t2gManager.GetAvailableElementDataList(out result.ElementList);

				if (lResult != T2GManagerErrorEnum.eSuccess)
				{
					result.ResultCode = MissionServiceErrorCodeEnum.ErrorElementListNotAvailable;
				}
			}
			else
			{
				result.ResultCode = MissionServiceErrorCodeEnum.ErrorInvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// This function allows the GroundApp to initialize a mission onboard an element.
		/// If the station list isn't provided - an existing mission is initialized. Otherwise -
		/// a new mission is initialized using existing stations.
		/// </summary>
		/// <param name="sessionId">A valid session identifier or 0, if the provided credentials are
		/// not valid</param>
		/// <param name="missionCode">The mission code of the particular mission</param>
		/// <param name="elementId">The element alpha number</param>
		/// <param name="stationList">The stations code list to initialize a new mission</param>
		/// <param name="timeOut">The timeout that is used to send the "ProgressTimeOut" notification
		/// to the GroundApp</param>        
		/// <returns>
		/// The code “request accepted” when the command is valid, or an error code when the command
		/// is rejected
		/// </returns>
		MissionInitializeMissionResult IMissionService.InitializeMission(
			Guid sessionId,
			string missionCode,
			string elementId,
			List<string> stationList,
			int? timeOut)
		{
			MissionInitializeMissionResult result = new MissionInitializeMissionResult();

			result.ResultCode = MissionServiceErrorCodeEnum.RequestAccepted;
			result.MissionCode = missionCode;

			if (timeOut == null || timeOut <= MAX_REQUEST_TIMEOUT)
			{
				if (_sessionManager.IsSessionValid(sessionId))
				{
					Guid requestId = Guid.Empty;
					_sessionManager.GenerateRequestID(sessionId, out requestId);

					if (requestId != Guid.Empty)
					{
						result.RequestId = requestId;

						// Resolve target element
						AvailableElementData element;
						T2GManagerErrorEnum rqstResult = _t2gManager.GetAvailableElementDataByElementNumber(elementId, out element);

						switch (rqstResult)
						{
							case T2GManagerErrorEnum.eSuccess:
								{
									bool missionCodeExists;
									List<string> invalidStationList;
									string LMTDbVersion;

									result.ResultCode = ValidateInitializeMissionRequest(
										element,
										missionCode,
										stationList,
										out missionCodeExists,
										out invalidStationList,
										out LMTDbVersion);

									if (result.ResultCode == MissionServiceErrorCodeEnum.RequestAccepted)
									{
										SendNotificationToGroundApp(requestId, NotificationIdEnum.MissionCommandProgressProcessing, elementId);

										InitializeMissionRequest request = new InitializeMissionRequest(
											requestId,
											sessionId,
											elementId,
											missionCode,
											(missionCodeExists == true) ? null : stationList,
											LMTDbVersion,
											(timeOut == null) ? RequestConstants.Timeout : (uint)timeOut.GetValueOrDefault());

										lock (_lock)
										{
											_newRequests.Add(request);
										}

										_transmitEvent.Set();
									}
									else if (invalidStationList.Count > 0)
									{
										result.InvalidStationList = invalidStationList.ToList();
									}
								}
								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								SendNotificationToGroundApp(requestId, NotificationIdEnum.MissionCommandT2GServerOffline);
								result.ResultCode = MissionServiceErrorCodeEnum.ErrorElementListNotAvailable;
								break;
							case T2GManagerErrorEnum.eElementNotFound:
								result.ResultCode = MissionServiceErrorCodeEnum.ErrorInvalidElementId;
								break;
							default:
								result.ResultCode = MissionServiceErrorCodeEnum.ErrorElementListNotAvailable;
								break;
						}
					}
					else
					{
						result.ResultCode = MissionServiceErrorCodeEnum.ErrorOpeningLMTDb;
						//result.ResultCode = MissionServiceErrorCodeEnum.ErrorInvalidSessionId;
					}
				}
				else
				{
					result.ResultCode = MissionServiceErrorCodeEnum.ErrorInvalidSessionId;
				}
			}
			else
			{
				result.ResultCode = MissionServiceErrorCodeEnum.ErrorInvalidRequestTimeout;
			}

			return result;
		}

		/// <summary>
		/// This function allows the GroundApp to cancel a mission onboard an element. 
		/// </summary>
		/// <param name="sessionId">A valid session identifier or 0, if the provided credentials
		/// are not valid</param>
		/// <param name="missionCode">The mission code of the particular mission</param>
		/// <param name="elementId">The element alpha number</param>
		/// <param name="timeOut">The timeout that is used to send the "ProgressTimeOut" notification
		/// to the GroundApp</param>
		/// <returns>The code “request accepted” when the command is valid, or an error code when
		/// the command is rejected
		/// </returns>
		MissionCancelMissionResult IMissionService.CancelMission(Guid sessionId, string missionCode, string elementId, int? timeOut)
		{
			MissionCancelMissionResult result = new MissionCancelMissionResult();

			result.ResultCode = MissionServiceErrorCodeEnum.RequestAccepted;
			result.MissionCode = missionCode;

			if (timeOut == null || timeOut <= MAX_REQUEST_TIMEOUT)
			{
				if (_sessionManager.IsSessionValid(sessionId))
				{
					Guid requestId = Guid.Empty;
					_sessionManager.GenerateRequestID(sessionId, out requestId);

					if (requestId != Guid.Empty)
					{
						result.RequestId = requestId;

						// Resolve target element
						AvailableElementData element;
						T2GManagerErrorEnum rqstResult = _t2gManager.GetAvailableElementDataByElementNumber(elementId, out element);

						switch (rqstResult)
						{
							case T2GManagerErrorEnum.eSuccess:
								{
									SendNotificationToGroundApp(requestId, NotificationIdEnum.MissionCommandProgressProcessing, elementId);

									CancelMissionRequest request = new CancelMissionRequest(
										requestId,
										sessionId,
										elementId,
										string.IsNullOrEmpty(missionCode) ? "" : missionCode,
										(timeOut == null) ? RequestConstants.Timeout : (uint)timeOut.GetValueOrDefault());

									lock (_lock)
									{
										_newRequests.Add(request);
									}

									_transmitEvent.Set();
								}
								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								SendNotificationToGroundApp(requestId, NotificationIdEnum.MissionCommandT2GServerOffline);
								result.ResultCode = MissionServiceErrorCodeEnum.ErrorElementListNotAvailable;
								break;
							case T2GManagerErrorEnum.eElementNotFound:
								result.ResultCode = MissionServiceErrorCodeEnum.ErrorInvalidElementId;
								break;
							default:
								result.ResultCode = MissionServiceErrorCodeEnum.ErrorElementListNotAvailable;
								break;
						}
					}
					else
					{
						result.ResultCode = MissionServiceErrorCodeEnum.ErrorInvalidSessionId;
					}
				}
				else
				{
					result.ResultCode = MissionServiceErrorCodeEnum.ErrorInvalidSessionId;
				}
			}
			else
			{
				result.ResultCode = MissionServiceErrorCodeEnum.ErrorInvalidRequestTimeout;
			}

			return result;
		}

		#endregion public methods

		#region internal classes

		/// <summary>
		/// MissionRequestContext class
		/// </summary>
		internal class MissionRequestContext : RequestContext
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="requestId">Request Id</param>
			/// <param name="sessionId">Session Id</param>
			/// <param name="elementId">Element Id</param>
			/// <param name="missionCode">Mission code</param>
			/// <param name="timeout">Request timeout</param>           
			public MissionRequestContext(
				Guid requestId,
				Guid sessionId,
				string elementId,
				string missionCode,
				uint timeout)
				: base("", elementId, requestId, sessionId, timeout)
			{
				MissionCode = missionCode;
			}

			public string MissionCode { get; set; }
		}

		/// <summary>
		/// InitializeMissionRequest class
		/// </summary>
		internal class InitializeMissionRequest : MissionRequestContext
		{
			private string _lmtDbVersion;
			private PIS.Train.Mission.StationList _stationList = new PIS.Train.Mission.StationList();

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="requestId">Request Id</param>
			/// <param name="sessionId">Session Id</param>
			/// <param name="elementId">Element Id</param>
			/// <param name="missionCode">New mission code</param>
			/// <param name="stationList">Station list</param>
			/// <param name="timeout">Request timeout</param>
			public InitializeMissionRequest(
				Guid requestId,
				Guid sessionId,
				string elementId,
				string missionCode,
				List<string> stationList,
				string lmtDbVersion,
				uint timeout)
				: base(requestId, sessionId, elementId, missionCode, timeout)
			{
				LMTDbVersion = lmtDbVersion;

				if (stationList != null)
				{
					_stationList.AddRange(stationList);
				}
			}

			public PIS.Train.Mission.StationList StationList
			{
				get { return _stationList; }
			}

			public string LMTDbVersion
			{
				get
				{
					return _lmtDbVersion;
				}

				set
				{
					if (string.IsNullOrEmpty(value))
					{
						_lmtDbVersion = "0.0.0.0";
					}
					else
					{
						_lmtDbVersion = value;
					}
				}
			}
		}

		/// <summary>
		/// CancelMissionRequest class
		/// </summary>
		internal class CancelMissionRequest : MissionRequestContext
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="requestId">Request Id</param>
			/// <param name="sessionId">Session Id</param>
			/// <param name="elementId">Element Id</param>
			/// <param name="missionCode">Code of the mission that should be canceled</param>
			/// <param name="timeout">request timeout</param>
			public CancelMissionRequest(
				Guid requestId,
				Guid sessionId,
				string elementId,
				string missionCode,
				uint timeout)
				: base(requestId, sessionId, elementId, missionCode, timeout)
			{

			}
		}

		/// <summary>
		///	Event throwed when a request is completed.
		/// </summary>
		internal class RequestCompletedEvent
		{
			/// <summary>
			/// Initializes a new instance of the RequestCompletedEvent class.
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

		#endregion
	}
}
