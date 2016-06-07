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
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Mission;
using PIS.Ground.Common;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.PackageAccess;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.Core.Utility;
using PIS.Ground.RemoteDataStore;
using PIS.Train.Mission;

namespace PIS.Ground.Mission
{
	/// <summary> Mission Service </summary>
	[CreateOnDispatchService(typeof(MissionService))]
	[ServiceBehavior(Namespace = "http://alstom.com/pacis/pis/ground/mission/")]
	public class MissionService : IMissionService
	{
		#region consts

		private const string NOTIFICATION_PARAMETER_ELEMENT_ID = "elementId";

		private const String DATE_TIME_FORMAT_IN = "ddMMyyyy";

		private const String DATE_TIME_FORMAT_OUT = "ddMMyyyy";

		private const uint MAX_REQUEST_TIMEOUT = 43200;

		/// <summary>Identifier for event subscription.</summary>
		private const string SubscriberId = "PIS.Ground.Mission.MissionService";

		#endregion

		#region static fields

		private static volatile bool _initialized = false;

		private static object _initializationLock = new object();

		private static Object _lock = new Object();

		private static List<MissionControlRequestContext> _newRequests = new List<MissionControlRequestContext>();

		private static List<ExternalEvent> _externalEvents = new List<ExternalEvent>();

		private static AutoResetEvent _transmitEvent = new AutoResetEvent(false);

		private static Thread _transmitThread = null;

		private static IT2GManager _t2gManager = null;

		private static ISessionManager _sessionManager = null;

		private static INotificationSender _notificationSender = null;

		#endregion

		public delegate void MissionRequestDelegate(RequestContext context, out string error);

		/// <summary>Initializes a new instance of the MissionService class.</summary>
		public MissionService()
		{
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "MissionService";
            }

			Initialize();
		}

		/// <summary>Initializes a new instance of the MissionService class.</summary>
		/// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
		/// <param name="sessionManager">Manager for session.</param>
		/// <param name="notificationSender">The notification sender.</param>
		/// <param name="t2gManager">Manager for 2g.</param>
		internal MissionService(ISessionManager sessionManager, INotificationSender notificationSender, IT2GManager t2gManager)
		{
			if (sessionManager == null)
			{
				throw new ArgumentNullException("sessionManager");
			}

			if (notificationSender == null)
			{
				throw new ArgumentNullException("notificationSender");
			}

			if (t2gManager == null)
			{
				throw new ArgumentNullException("t2gManager");
			}

			_sessionManager = sessionManager;
			_notificationSender = notificationSender;
			_t2gManager = t2gManager;

			// Register to receive a notification when an element Online State changes.
			_t2gManager.SubscribeToElementChangeNotification(SubscriberId, new EventHandler<ElementEventArgs>(OnElementInfoChanged));

			_transmitThread = new Thread(new ThreadStart(OnTransmitEvent));
            _transmitThread.Name = "Mission Transmit";
			_transmitThread.Start();
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

							// Register to receive a notification when an element Online State changes.
							_t2gManager.SubscribeToElementChangeNotification(SubscriberId, new EventHandler<ElementEventArgs>(OnElementInfoChanged));

							_sessionManager = new SessionManager();

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

		/// <summary>
		/// Callback called when Element Online state changes (signaled by the T2G Client)
		/// </summary>
		private static void OnElementInfoChanged(object sender, ElementEventArgs args)
		{
			//Signal the event to start handling the request.
			_transmitEvent.Set();
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
			LogManager.WriteLog(TraceType.INFO, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_CALLED), "PIS.Ground.Mission.MissionService.SendNotificationToGroundApp", null, EventIdEnum.Mission);
			try
			{
				//send notification to ground app
				_notificationSender.SendNotification(status, null, requestId);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.ERROR, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_EXCEPTION), "PIS.Ground.Mission.MissionService.SendNotificationToGroundApp", ex, EventIdEnum.Mission);
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
			LogManager.WriteLog(TraceType.INFO, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_CALLED, "SendNotificationToGroundApp"), "PIS.Ground.Mission.MissionService.SendNotificationToGroundApp", null, EventIdEnum.Mission);
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

				//Send Notification to AppGround
				_notificationSender.SendNotification(status, lStringWriter.ToString(), requestId);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.ERROR, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_EXCEPTION), "PIS.Ground.Mission.MissionService.SendNotificationToGroundApp", ex, EventIdEnum.Mission);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public MissionServiceElementListResult GetAvailableElementList(Guid sessionId)
		{
			MissionServiceElementListResult result = new MissionServiceElementListResult();
			result.ResultCode = MissionErrorCode.InternalError;

			SessionData sessionData;
			string error = _sessionManager.GetSessionDetails(sessionId, out sessionData);
			if (string.IsNullOrEmpty(error))
			{
				T2GManagerErrorEnum lResult = _t2gManager.GetAvailableElementDataList(out result.ElementList);

				if (lResult == T2GManagerErrorEnum.eSuccess)
				{
					result.ResultCode = MissionErrorCode.RequestAccepted;
				}
				else
				{
					result.ResultCode = MissionErrorCode.ElementListNotAvailable;
				}
			}
			else
			{
				result.ResultCode = MissionErrorCode.InvalidSessionId;
			}

			return result;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="automaticModeRequest"></param>
		/// <param name="requestId"></param>
		/// <returns>Error if any</returns>
		MissionServiceResult IMissionService.InitializeAutomaticMission(AutomaticModeRequest automaticModeRequest)
		{
			MissionServiceResult lResult = new MissionServiceResult();
			lResult.RequestId = Guid.Empty;
			lResult.ResultCode = MissionErrorCode.InternalError;

			if (automaticModeRequest != null)
			{
				if (automaticModeRequest.RequestTimeout <= MAX_REQUEST_TIMEOUT)
				{
					SessionData sessionData;
					_sessionManager.GetSessionDetails(automaticModeRequest.SessionId, out sessionData);

					if (sessionData != null)
					{
						Guid requestId = Guid.Empty;
						_sessionManager.GenerateRequestID(automaticModeRequest.SessionId, out requestId);

						if (requestId != Guid.Empty)
						{
							lResult.RequestId = requestId;
							lResult.ResultCode = MissionErrorCode.RequestAccepted;

							string elementId = automaticModeRequest.ElementAlphaNumber ?? string.Empty;
							if (elementId.Length > 80)
							{
								lResult.RequestId = Guid.Empty;
								lResult.ResultCode = MissionErrorCode.InvalidTrainName;
							}
							else
							{
								SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressProcessing, elementId);

								// Resolve target element
								AvailableElementData element;
								T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableElementDataByElementNumber(elementId, out element);
								switch (lRqstResult)
								{
									case T2GManagerErrorEnum.eSuccess:
										{
											try
											{
												AutomaticActivationRequestContext request = new AutomaticActivationRequestContext(
													elementId,
													requestId,
													automaticModeRequest.SessionId,
													automaticModeRequest.RequestTimeout);

												request.IncomingRequest = automaticModeRequest;

												PIS.Ground.GroundCore.AppGround.NotificationIdEnum? errorNotification =
													PreValidateAutomaticModeRequest(request);

												if (errorNotification == null)
												{
													lock (_lock)
													{
														_newRequests.Add(request);
													}

													_transmitEvent.Set();
												}
												else
												{
													SendNotificationToGroundApp(requestId, errorNotification.Value, request.ElementId);
												}
											}
											catch (PisDataStoreNotAccessibleException ex)
											{
												lResult.RequestId = Guid.Empty; // discard the generated request ID
												lResult.ResultCode = MissionErrorCode.DatastoreNotAccessible;

												LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Mission.MissionService.InitializeAutomaticMission", ex, EventIdEnum.Mission);
											}
										}
										break;
									case T2GManagerErrorEnum.eT2GServerOffline:
										SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline, "");
										break;
									case T2GManagerErrorEnum.eElementNotFound:
										SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownElementId, elementId);
										break;
									default:
										break;
								}
							}
						}
						else
						{
							lResult.ResultCode = MissionErrorCode.InvalidSessionId;
						}
					}
					else
					{
						lResult.ResultCode = MissionErrorCode.InvalidSessionId;
					}
				}
				else
				{
					lResult.ResultCode = MissionErrorCode.InvalidRequestTimeout;
				}
			}

			return lResult;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="modifiedModeRequest"></param>
		/// <param name="requestId"></param>
		/// <returns>Error if any</returns>
		MissionServiceResult IMissionService.InitializeModifiedMission(ModifiedModeRequest modifiedModeRequest)
		{
			MissionServiceResult lResult = new MissionServiceResult();
			lResult.RequestId = Guid.Empty;
			lResult.ResultCode = MissionErrorCode.InternalError;

			LogManager.WriteLog(TraceType.ERROR, modifiedModeRequest.RegionCode.ToString() + " value", "PIS.Ground.Mission.MissionService.InitializeModifiedMission", null, EventIdEnum.Mission);

			if (modifiedModeRequest != null)
			{
				if (modifiedModeRequest.RequestTimeout <= MAX_REQUEST_TIMEOUT)
				{
					SessionData sessionData;
					_sessionManager.GetSessionDetails(modifiedModeRequest.SessionId, out sessionData);

					if (sessionData != null)
					{
						Guid requestId = Guid.Empty;
						_sessionManager.GenerateRequestID(modifiedModeRequest.SessionId, out requestId);

						if (requestId != Guid.Empty)
						{
							lResult.RequestId = requestId;
							lResult.ResultCode = MissionErrorCode.RequestAccepted;

							string elementId = modifiedModeRequest.ElementAlphaNumber ?? string.Empty;
							if (elementId.Length > 80)
							{
								lResult.RequestId = Guid.Empty;
								lResult.ResultCode = MissionErrorCode.InvalidTrainName;
							}
							else
							{
								SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressProcessing, elementId);

								// Resolve target element
								AvailableElementData element;
								T2GManagerErrorEnum lRqsResult = _t2gManager.GetAvailableElementDataByElementNumber(elementId, out element);
								switch (lRqsResult)
								{
									case T2GManagerErrorEnum.eSuccess:
										{
											try
											{
												ModifiedActivationRequestContext request = new ModifiedActivationRequestContext(
														elementId,
														requestId,
														modifiedModeRequest.SessionId,
														modifiedModeRequest.RequestTimeout);

												request.IncomingRequest = modifiedModeRequest;

												PIS.Ground.GroundCore.AppGround.NotificationIdEnum? errorNotification =
													PreValidateModifiedModeRequest(request);

												if (errorNotification == null)
												{
													lock (_lock)
													{
														_newRequests.Add(request);
													}

													_transmitEvent.Set();
												}
												else
												{
													SendNotificationToGroundApp(requestId, errorNotification.Value, request.ElementId);
												}
											}
											catch (PisDataStoreNotAccessibleException ex)
											{
												lResult.RequestId = Guid.Empty; // discard the generated request ID
												lResult.ResultCode = MissionErrorCode.DatastoreNotAccessible;

												LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Mission.MissionService.InitializeAutomaticMission", ex, EventIdEnum.Mission);
											}
										}
										break;
									case T2GManagerErrorEnum.eT2GServerOffline:
										SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline, "");
										break;
									case T2GManagerErrorEnum.eElementNotFound:
										SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownElementId, elementId);
										break;
									default:
										break;
								}
							}
						}
						else
						{
							lResult.ResultCode = MissionErrorCode.InvalidSessionId;
						}
					}
					else
					{
						lResult.ResultCode = MissionErrorCode.InvalidSessionId;
					}
				}
				else
				{
					lResult.ResultCode = MissionErrorCode.InvalidRequestTimeout;
				}
			}

			return lResult;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="manualModeRequest"></param>
		/// <param name="requestId"></param>
		/// <returns>Error if any</returns>
		MissionServiceResult IMissionService.InitializeManualMission(ManualModeRequest manualModeRequest)
		{
			MissionServiceResult lResult = new MissionServiceResult();
			lResult.RequestId = Guid.Empty;
			lResult.ResultCode = MissionErrorCode.InternalError;

			if (manualModeRequest != null)
			{
				if (manualModeRequest.RequestTimeout <= MAX_REQUEST_TIMEOUT)
				{
					SessionData sessionData = new SessionData();
					_sessionManager.GetSessionDetails(manualModeRequest.SessionId, out sessionData);

					if (sessionData != null)
					{
						Guid requestId = Guid.Empty;
						_sessionManager.GenerateRequestID(manualModeRequest.SessionId, out requestId);

						if (requestId != Guid.Empty)
						{
							lResult.RequestId = requestId;
							lResult.ResultCode = MissionErrorCode.RequestAccepted;

							string elementId = manualModeRequest.ElementAlphaNumber ?? string.Empty;
							if (elementId.Length > 80)
							{
								lResult.RequestId = Guid.Empty;
								lResult.ResultCode = MissionErrorCode.InvalidTrainName;
							}
							else
							{
								SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressProcessing, elementId);

								// Resolve target element
								AvailableElementData element;
								T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableElementDataByElementNumber(elementId, out element);

								switch (lRqstResult)
								{
									case T2GManagerErrorEnum.eSuccess:
										{
											try
											{
												ManualActivationRequestContext request = new ManualActivationRequestContext(
															elementId,
															requestId,
															manualModeRequest.SessionId,
															manualModeRequest.RequestTimeout);

												request.IncomingRequest = manualModeRequest;

												PIS.Ground.GroundCore.AppGround.NotificationIdEnum? errorNotification =
													PreValidateManualModeRequest(request);

												if (errorNotification == null)
												{
													lock (_lock)
													{
														_newRequests.Add(request);
													}

													_transmitEvent.Set();
												}
												else
												{
													SendNotificationToGroundApp(requestId, errorNotification.Value, request.ElementId);
												}
											}
											catch (PisDataStoreNotAccessibleException ex)
											{
												lResult.RequestId = Guid.Empty; // discard the generated request ID
												lResult.ResultCode = MissionErrorCode.DatastoreNotAccessible;

												LogManager.WriteLog(
													TraceType.ERROR,
													ex.Message,
													"PIS.Ground.Mission.MissionService.InitializeManualMission",
													ex,
													EventIdEnum.Mission);
											}
										}
										break;
									case T2GManagerErrorEnum.eT2GServerOffline:
										SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline, "");
										break;
									case T2GManagerErrorEnum.eElementNotFound:
										SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownElementId, elementId);
										break;
									default:
										break;
								}
							}
						}
						else
						{
							lResult.ResultCode = MissionErrorCode.InvalidSessionId;
						}
					}
					else
					{
						lResult.ResultCode = MissionErrorCode.InvalidSessionId;
					}
				}
				else
				{
					lResult.ResultCode = MissionErrorCode.InvalidRequestTimeout;
				}
			}

			return lResult;
		}

		/// <summary>
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="onBoardValidationFlag"></param>
		/// <param name="elementAlphaNumber"></param>
		/// <param name="missionId"></param>
		/// <param name="timeOut"></param>
		/// <param name="requestId"></param>
		/// <returns>Error if any</returns>
		MissionServiceResult IMissionService.StopMission(Guid pSessionId, bool pOnBoardValidationFlag, string pElementAlphaNumber, string pMissionId, uint pTimeOut)
		{
			MissionServiceResult lResult = new MissionServiceResult();
			lResult.RequestId = Guid.Empty;
			lResult.ResultCode = MissionErrorCode.InternalError;

			if (pTimeOut <= MAX_REQUEST_TIMEOUT)
			{
				SessionData sessionData = new SessionData();
				_sessionManager.GetSessionDetails(pSessionId, out sessionData);

				if (sessionData != null)
				{
					Guid requestId = Guid.Empty;
					_sessionManager.GenerateRequestID(pSessionId, out requestId);

					if (requestId != Guid.Empty)
					{
						lResult.RequestId = requestId;
						lResult.ResultCode = MissionErrorCode.RequestAccepted;

						string elementId = pElementAlphaNumber ?? string.Empty;

						SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressProcessing, elementId);

						// Resolve target element
						AvailableElementData element;
						T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableElementDataByElementNumber(elementId, out element);

						switch (lRqstResult)
						{
							case T2GManagerErrorEnum.eSuccess:
								{
									try
									{
										PIS.Ground.GroundCore.AppGround.NotificationIdEnum? errorNotification = PreValidateStopMissionRequest(elementId, pMissionId);
										if (errorNotification == null)
										{
											lock (_lock)
											{
												StopMissionRequest stopRequest = new StopMissionRequest();
												stopRequest.ElementId = pElementAlphaNumber;
												stopRequest.MissionOperatorId = pMissionId ?? string.Empty;
												stopRequest.OnboardValidation = pOnBoardValidationFlag;
												stopRequest.RequestId = requestId.ToString();

												StopMissionRequestContext request = new StopMissionRequestContext(
													pElementAlphaNumber,
													requestId,
													pSessionId,
													pTimeOut,
													stopRequest);

												_newRequests.Add(request);
											}

											_transmitEvent.Set();
										}
										else
										{
											SendNotificationToGroundApp(requestId, errorNotification.Value, elementId);
										}
									}
									catch (PisDataStoreNotAccessibleException ex)
									{
										lResult.RequestId = Guid.Empty; // discard the generated request ID
										lResult.ResultCode = MissionErrorCode.DatastoreNotAccessible;

										LogManager.WriteLog(
											TraceType.ERROR,
											ex.Message,
											"PIS.Ground.Mission.MissionService.StopMission",
											ex,
											EventIdEnum.Mission);
									}
								}
								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline, "");
								break;
							case T2GManagerErrorEnum.eElementNotFound:
								SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownElementId, elementId);
								break;
							default:
								break;
						}
					}
					else
					{
						lResult.ResultCode = MissionErrorCode.InvalidSessionId;
					}
				}
				else
				{
					lResult.ResultCode = MissionErrorCode.InvalidSessionId;
				}
			}
			else
			{
				lResult.ResultCode = MissionErrorCode.InvalidRequestTimeout;
			}

			return lResult;
		}

		/// <summary>
		///
		/// </summary>
		private static void OnTransmitEvent()
		{
            try
            {
                var currentRequests = new List<MissionControlRequestContext>();

                while (true)
                {
                    if (currentRequests.Count == 0)
                    {
                        _transmitEvent.WaitOne();  //wait until new currentRequests
                    }

                    lock (_lock)
                    {
                        // Move pending from _newRequests to currentRequests                    
                        if (_newRequests.Count > 0)
                        {
                            currentRequests.AddRange(_newRequests);
                            _newRequests.Clear();
                            currentRequests.RemoveAll(c => c == null);
                        }
                    }

                    foreach (var request in currentRequests)
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
                                SendNotificationToGroundApp(
                                    request.RequestId,
                                    PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressTimedOut,
                                    request.ElementId);
                                break;

                            case RequestState.AllRetriesExhausted:
                                break;

                            case RequestState.Transmitted:
                                break;

                            case RequestState.Completed:
                                break;

                            case RequestState.Error:
                                break;
                        }
                    }

                    lock (_lock)
                    {
                        foreach (ExternalEvent extEvent in _externalEvents)
                        {
                            if (extEvent is RequestCompletedEvent)
                            {
                                var evt = extEvent as RequestCompletedEvent;

                                var lReqList = currentRequests.FindAll(
                                    delegate(MissionControlRequestContext c) { return c.ElementId == evt.elementId && c.RequestId == evt.requestId; });

                                foreach (var lRequest in lReqList)
                                {
                                    lRequest.CompletionStatus = true;
                                }
                            }
                        }
                        _externalEvents.Clear();
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
                LogManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.Mission.MissionService.OnTransmitEvent", exception, EventIdEnum.Mission);
            }
		}

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
				LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Mission.MissionService.FindLmtDatabaseFilePath", ex, EventIdEnum.Mission);
			}
			return lmtDatabaseFilePath;
		}

		private static PIS.Ground.GroundCore.AppGround.NotificationIdEnum? PreValidateAutomaticModeRequest(AutomaticActivationRequestContext request)
		{
			if (request.IncomingRequest.StationInsertion == null ||
				!(request.IncomingRequest.StationInsertion.Type == 1 ||
				request.IncomingRequest.StationInsertion.Type == 2))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidInsertionTypeCode;
			}

			if (string.IsNullOrEmpty(request.IncomingRequest.MissionOperatorCode))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownMissionOperatorCode;
			}

			if (string.IsNullOrEmpty(request.IncomingRequest.LmtDataPackageVersion))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
			}

			if (request.IncomingRequest.LanguageCodeList == null ||
				request.IncomingRequest.LanguageCodeList.Count <= 0)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLanguageCode;
			}

			if (request.IncomingRequest.OnboardServiceCodeList == null ||
				request.IncomingRequest.OnboardServiceCodeList.Count <= 0)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownOnboardServiceCode;
			}

			//TODO check 0-3 or 1-4 or ... finalize with SyID
			if (request.IncomingRequest.CarNumberingOffsetCode < 0 ||
				request.IncomingRequest.CarNumberingOffsetCode > 4)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidCarNumberingOffsetCode;
			}

			DateTime lTemp;

			if (!DateTime.TryParseExact(
					request.IncomingRequest.StartDate,
					DATE_TIME_FORMAT_IN,
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out lTemp))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidStartDate;
			}

			if (string.IsNullOrEmpty(request.IncomingRequest.StationInsertion.StationId))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownInsertionStationId;
			}

			try
			{
				using (var remoteDataStore = new RemoteDataStoreProxy())
				{
					if (remoteDataStore.checkIfDataPackageExists("LMT", request.IncomingRequest.LmtDataPackageVersion))
					{
						var openPackageResult = remoteDataStore.openLocalDataPackage(
							"LMT",
							request.IncomingRequest.LmtDataPackageVersion,
							String.Empty);

						if (openPackageResult.Status == OpenDataPackageStatusEnum.COMPLETED)
						{
							string lmtDatabaseFilePath = FindLmtDatabaseFilePath(openPackageResult.LocalPackagePath);
							if (!string.IsNullOrEmpty(lmtDatabaseFilePath))
							{
								try
								{
									using (var dbAccess = new LmtDatabaseAccessor(lmtDatabaseFilePath))
									{
										if (!dbAccess.LanguageCodesExist(request.IncomingRequest.LanguageCodeList))
										{
											return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLanguageCode;
										}

										uint? missionInternalId = dbAccess.GetMissionInternalCodeFromOperatorCode(
											request.IncomingRequest.MissionOperatorCode);
										if (missionInternalId == null)
										{
											return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownMissionOperatorCode;
										}

										if (!dbAccess.ServiceCodesExist(request.IncomingRequest.OnboardServiceCodeList))
										{
											return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownOnboardServiceCode;
										}

										uint? insertionStationInternalId = dbAccess.GetStationInternalCodeFromOperatorCode(request.IncomingRequest.StationInsertion.StationId);
										if (insertionStationInternalId == null)
										{
											return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownInsertionStationId;
										}

										if (!dbAccess.GetMissionRoute(missionInternalId.Value).Contains(insertionStationInternalId.Value))
										{
											return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInsertionStationIsNotInRoute;
										}

										// All verifications OK

										PostValidateAutomaticModeRequest(request, missionInternalId.Value, insertionStationInternalId.Value);

										return null; //no error
									}
								}
								catch (Exception ex)
								{
									LogManager.WriteLog(
										TraceType.ERROR,
										"Error reading LMT database (" + lmtDatabaseFilePath + ")",
										"PIS.Ground.Mission.MissionService.PreValidateAutomaticModeRequest",
										ex,
										EventIdEnum.Mission);

									return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
								}
							}
							else
							{
								return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
							}
						}
						else
						{
							return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
						}
					}
					else
					{
						return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
					}
				}
			}
			catch (CommunicationException ex)
			{
				throw new PisDataStoreNotAccessibleException("PIS.Ground.Mission.MissionService.PreValidateAutomaticModeRequest", ex);
			}
		}

		private static void PostValidateAutomaticModeRequest(AutomaticActivationRequestContext request, uint missionInternalId, uint insertionStationInternalId)
		{
			request.OutgoingRequest = new ActivateAutomaticModeRequest();
			request.OutgoingRequest.MissionInfo = new AutomaticMissionInfoType();

			request.OutgoingRequest.MissionInfo.RequestId = request.RequestId.ToString();
			request.OutgoingRequest.MissionInfo.ElementId = request.IncomingRequest.ElementAlphaNumber;

			request.OutgoingRequest.MissionInfo.LanguageCodeList = new ActivationMissionCommonInfoType.LanguageCodeListType();

			foreach (string code in request.IncomingRequest.LanguageCodeList)
			{
				request.OutgoingRequest.MissionInfo.LanguageCodeList.Add(code);
			}
			request.OutgoingRequest.MissionInfo.LmtDataPackageVersion = request.IncomingRequest.LmtDataPackageVersion;
			request.OutgoingRequest.MissionInfo.MissionId = missionInternalId;
			request.OutgoingRequest.MissionInfo.MissionOperatorId = request.IncomingRequest.MissionOperatorCode;

			DateTime startDate = DateTime.Now;

			if (DateTime.TryParseExact(request.IncomingRequest.StartDate,
					DATE_TIME_FORMAT_IN,
					System.Globalization.CultureInfo.InvariantCulture,
					System.Globalization.DateTimeStyles.None, out startDate))
			{
				request.OutgoingRequest.MissionInfo.MissionStartDate = startDate.ToString(DATE_TIME_FORMAT_OUT);
			}

			request.OutgoingRequest.MissionInfo.OnboardServiceCodeList = new ActivationMissionCommonInfoType.OnboardServiceCodeListType();

			foreach (uint serCode in request.IncomingRequest.OnboardServiceCodeList)
			{
				request.OutgoingRequest.MissionInfo.OnboardServiceCodeList.Add(serCode);
			}

			request.OutgoingRequest.MissionInfo.OnboardValidation = request.IncomingRequest.OnBoardValidationFlag;

			request.OutgoingRequest.MissionInfo.StationInsertion = new StationInsertionType();
			request.OutgoingRequest.MissionInfo.StationInsertion.StationId = insertionStationInternalId;
			request.OutgoingRequest.MissionInfo.StationInsertion.Type = request.IncomingRequest.StationInsertion.Type;
			request.OutgoingRequest.MissionInfo.CarNumberingOffsetCode = request.IncomingRequest.CarNumberingOffsetCode;
		}

		private static PIS.Ground.GroundCore.AppGround.NotificationIdEnum? PreValidateModifiedModeRequest(ModifiedActivationRequestContext request)
		{
			if (request.IncomingRequest.ServicedStationList.Count <= 1)
			{
				// NOTE: No other corresponding notification (e.g. MissionCommandProgressInvalidStationList)
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownStationId;
			}

			if (request.IncomingRequest.ServicedStationList.FindAll(
				delegate(StationList lItem) { return string.IsNullOrEmpty(lItem.Id); }
				).Count > 0)
			{
				// NOTE: No other corresponding notification (e.g. MissionCommandProgressInvalidStationList)
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownStationId;
			}

			if (request.IncomingRequest.ServiceHourList.Count != request.IncomingRequest.ServicedStationList.Count)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidServiceHours;
			}

			for (int i = 0; i < request.IncomingRequest.ServiceHourList.Count; i++)
			{
				if ((request.IncomingRequest.ServiceHourList[i].ArrivalTime == null && i != 0) ||
					(request.IncomingRequest.ServiceHourList[i].DepartureTime == null && i != request.IncomingRequest.ServiceHourList.Count - 1))
				{
					return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidServiceHours;
				}
			}

			if (request.IncomingRequest.CommercialNumberList.Count != request.IncomingRequest.ServicedStationList.Count - 1)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidCommercialNumberList;
			}

			if (request.IncomingRequest.CommercialNumberList.FindAll(delegate(string pItem)
			{
				return string.IsNullOrEmpty(pItem);
			}).Count > 0)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidCommercialNumberList;
			}

			if (request.IncomingRequest.StationInsertion == null ||
				!(request.IncomingRequest.StationInsertion.Type == 1 || request.IncomingRequest.StationInsertion.Type == 2))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidInsertionTypeCode;
			}

			if (string.IsNullOrEmpty(request.IncomingRequest.LmtDataPackageVersion))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
			}

			if (request.IncomingRequest.LanguageCodeList.Count <= 0)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLanguageCode;
			}

			if (request.IncomingRequest.OnboardServiceCodeList.Count <= 0)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownOnboardServiceCode;
			}

			if (request.IncomingRequest.CarNumberingOffsetCode < 0 || request.IncomingRequest.CarNumberingOffsetCode > 4)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidCarNumberingOffsetCode;
			}

			DateTime lTemp;
			if (!DateTime.TryParseExact(
					request.IncomingRequest.StartDate,
					DATE_TIME_FORMAT_IN,
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out lTemp))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidStartDate;
			}

			if (string.IsNullOrEmpty(request.IncomingRequest.StationInsertion.StationId))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownInsertionStationId;
			}

			if (request.IncomingRequest.ServicedStationList.FindAll(
				delegate(StationList lItem) { return lItem.Id == request.IncomingRequest.StationInsertion.StationId; }
				).Count == 0)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInsertionStationIsNotInRoute;
			}

			try
			{
				using (var remoteDataStore = new RemoteDataStoreProxy())
				{
					if (remoteDataStore.checkIfDataPackageExists("LMT", request.IncomingRequest.LmtDataPackageVersion))
					{
						var openPackageResult = remoteDataStore.openLocalDataPackage(
							"LMT",
							request.IncomingRequest.LmtDataPackageVersion,
							String.Empty);

						if (openPackageResult.Status == OpenDataPackageStatusEnum.COMPLETED)
						{
							string lmtDatabaseFilePath = FindLmtDatabaseFilePath(openPackageResult.LocalPackagePath);
							if (!string.IsNullOrEmpty(lmtDatabaseFilePath))
							{
								try
								{
									using (var dbAccess = new LmtDatabaseAccessor(lmtDatabaseFilePath))
									{
										if (!dbAccess.LanguageCodesExist(request.IncomingRequest.LanguageCodeList))
										{
											return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLanguageCode;
										}

										if (!dbAccess.ServiceCodesExist(request.IncomingRequest.OnboardServiceCodeList))
										{
											return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownOnboardServiceCode;
										}

										List<uint> stationInternalIds = new List<uint>();

										foreach (StationList station in request.IncomingRequest.ServicedStationList)
										{
											uint? stationInternalId = dbAccess.GetStationInternalCodeFromOperatorCode(station.Id);
											if (stationInternalId == null)
											{
												return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownStationId;
											}

											stationInternalIds.Add(stationInternalId.Value);
										}

										uint? insertionStationInternalId = dbAccess.GetStationInternalCodeFromOperatorCode(
											request.IncomingRequest.StationInsertion.StationId);

										if (insertionStationInternalId == null)
										{
											return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownInsertionStationId;
										}

										if (!dbAccess.RegionCodeExist(request.IncomingRequest.RegionCode))
										{
											return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownRegionCode;
										}

										// All verifications OK

										PostValidateModifiedModeRequest(request, stationInternalIds, insertionStationInternalId.Value);

										return null; //no error
									}
								}
								catch (Exception ex)
								{
									LogManager.WriteLog(
										TraceType.ERROR,
										"Error reading LMT database (" + lmtDatabaseFilePath + ")",
										"PIS.Ground.Mission.MissionService.PreValidateModifiedModeRequest",
										ex,
										EventIdEnum.Mission);

									return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
								}
							}
							else
							{
								return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
							}
						}
						else
						{
							return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
						}
					}
					else
					{
						return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
					}
				}
			}
			catch (CommunicationException ex)
			{
				throw new PisDataStoreNotAccessibleException(
					"PIS.Ground.Mission.MissionService.PreValidateModifiedModeRequest",
					ex);
			}
		}

		private static void PostValidateModifiedModeRequest(ModifiedActivationRequestContext request, List<uint> stationInternalIds, uint insertionStationInternalId)
		{
			request.OutgoingRequest = new ActivateModifiedModeRequest();
			request.OutgoingRequest.MissionInfo = new ModifiedMissionInfoType();

			request.OutgoingRequest.MissionInfo.RequestId = request.RequestId.ToString();
			request.OutgoingRequest.MissionInfo.ElementId = request.IncomingRequest.ElementAlphaNumber;
			request.OutgoingRequest.MissionInfo.LanguageCodeList = new ActivationMissionCommonInfoType.LanguageCodeListType();

			foreach (string code in request.IncomingRequest.LanguageCodeList)
			{
				request.OutgoingRequest.MissionInfo.LanguageCodeList.Add(code);
			}

			request.OutgoingRequest.MissionInfo.LmtDataPackageVersion = request.IncomingRequest.LmtDataPackageVersion;
			request.OutgoingRequest.MissionInfo.MissionId = 0;
			request.OutgoingRequest.MissionInfo.MissionOperatorId = request.IncomingRequest.MissionOperatorCode;

			DateTime startDate = DateTime.Now;
			if (DateTime.TryParseExact(request.IncomingRequest.StartDate,
					DATE_TIME_FORMAT_IN,
					System.Globalization.CultureInfo.InvariantCulture,
					System.Globalization.DateTimeStyles.None, out startDate))
			{
				request.OutgoingRequest.MissionInfo.MissionStartDate = startDate.ToString(DATE_TIME_FORMAT_OUT);
			}

			request.OutgoingRequest.MissionInfo.MissionType = request.IncomingRequest.MissionTypeCode;
			request.OutgoingRequest.MissionInfo.StationList = new ManualMissionInfoType.StationListType();

			for (int i = 0; i < stationInternalIds.Count(); ++i)
			{
				PIS.Train.Mission.StationListType station = new PIS.Train.Mission.StationListType();
				station.Id = stationInternalIds.ElementAt(i);
				station.Name = string.Empty;
				request.OutgoingRequest.MissionInfo.StationList.Add(station);
			}

			request.OutgoingRequest.MissionInfo.OnboardServiceCodeList = new ActivationMissionCommonInfoType.OnboardServiceCodeListType();

			foreach (uint serCode in request.IncomingRequest.OnboardServiceCodeList)
			{
				request.OutgoingRequest.MissionInfo.OnboardServiceCodeList.Add(serCode);
			}

			request.OutgoingRequest.MissionInfo.OnboardValidation = request.IncomingRequest.OnBoardValidationFlag;
			request.OutgoingRequest.MissionInfo.CarNumberingOffsetCode = request.IncomingRequest.CarNumberingOffsetCode;
			request.OutgoingRequest.MissionInfo.StationServiceHourList = new ManualMissionInfoType.StationServiceHourListType();

			foreach (StationServiceHours stHr in request.IncomingRequest.ServiceHourList)
			{
				StationServiceHoursType stHrTyp = new StationServiceHoursType();
				stHrTyp.ArrivalTime = stHr.ArrivalTime.ToString();
				stHrTyp.DepartureTime = stHr.DepartureTime.ToString();
				request.OutgoingRequest.MissionInfo.StationServiceHourList.Add(stHrTyp);
			}

			request.OutgoingRequest.MissionInfo.CommercialNumberList = new ManualMissionInfoType.CommercialNumberListType();

			foreach (string number in request.IncomingRequest.CommercialNumberList)
			{
				request.OutgoingRequest.MissionInfo.CommercialNumberList.Add(number);
			}

			request.OutgoingRequest.MissionInfo.StationInsertion = new StationInsertionType();
			request.OutgoingRequest.MissionInfo.StationInsertion.StationId = insertionStationInternalId;
			request.OutgoingRequest.MissionInfo.StationInsertion.Type = request.IncomingRequest.StationInsertion.Type;

			request.OutgoingRequest.MissionInfo.TrainName = request.IncomingRequest.TrainName;

			request.OutgoingRequest.MissionInfo.RegionCode = request.IncomingRequest.RegionCode;
		}

		private static PIS.Ground.GroundCore.AppGround.NotificationIdEnum? PreValidateManualModeRequest(ManualActivationRequestContext request)
		{
			if (request.IncomingRequest.ServicedStationList.Count <= 1)
			{
				// NOTE: No other corresponding notification (e.g. MissionCommandProgressInvalidStationList)
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownStationId;
			}

			if (request.IncomingRequest.ServicedStationList.FindAll(
			   delegate(StationList lItem) { return string.IsNullOrEmpty(lItem.Id) && string.IsNullOrEmpty(lItem.Name); }
			   ).Count > 0)
			{
				// NOTE: No other corresponding notification (e.g. MissionCommandProgressInvalidStationList)
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownStationId;
			}

			if (request.IncomingRequest.ServiceHourList.Count != request.IncomingRequest.ServicedStationList.Count)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidServiceHours;
			}

			for (int i = 0; i < request.IncomingRequest.ServiceHourList.Count; i++)
			{
				if ((request.IncomingRequest.ServiceHourList[i].ArrivalTime == null && i != 0) ||
					(request.IncomingRequest.ServiceHourList[i].DepartureTime == null && i != request.IncomingRequest.ServiceHourList.Count - 1))
				{
					return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidServiceHours;
				}
			}

			if (request.IncomingRequest.CommercialNumberList.Count != request.IncomingRequest.ServicedStationList.Count - 1)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidCommercialNumberList;
			}

			if (request.IncomingRequest.CommercialNumberList.FindAll(delegate(string pItem) { return string.IsNullOrEmpty(pItem); }).Count > 0)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidCommercialNumberList;
			}

			if (string.IsNullOrEmpty(request.IncomingRequest.LmtDataPackageVersion))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
			}

			if (request.IncomingRequest.LanguageCodeList.Count <= 0)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLanguageCode;
			}
			if (request.IncomingRequest.OnboardServiceCodeList.Count <= 0)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownOnboardServiceCode;
			}
			if (request.IncomingRequest.CarNumberingOffsetCode < 0 || request.IncomingRequest.CarNumberingOffsetCode > 4)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidCarNumberingOffsetCode;
			}

			DateTime lTemp;
			if (!DateTime.TryParseExact(
					request.IncomingRequest.StartDate,
					DATE_TIME_FORMAT_IN,
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out lTemp))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressInvalidStartDate;
			}

			try
			{
				using (var remoteDataStore = new RemoteDataStoreProxy())
				{
					if (remoteDataStore.checkIfDataPackageExists("LMT", request.IncomingRequest.LmtDataPackageVersion))
					{
						var openPackageResult = remoteDataStore.openLocalDataPackage(
							"LMT",
							request.IncomingRequest.LmtDataPackageVersion,
							String.Empty);

						if (openPackageResult.Status == OpenDataPackageStatusEnum.COMPLETED)
						{
							string lmtDatabaseFilePath = FindLmtDatabaseFilePath(openPackageResult.LocalPackagePath);
							if (!string.IsNullOrEmpty(lmtDatabaseFilePath))
							{
								try
								{
									using (var dbAccess = new LmtDatabaseAccessor(lmtDatabaseFilePath))
									{
										if (!dbAccess.LanguageCodesExist(request.IncomingRequest.LanguageCodeList))
										{
											return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLanguageCode;
										}

										if (!dbAccess.ServiceCodesExist(request.IncomingRequest.OnboardServiceCodeList))
										{
											return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownOnboardServiceCode;
										}

										Dictionary<string, uint> stationInternalIds = new Dictionary<string, uint>();

										foreach (StationList station in request.IncomingRequest.ServicedStationList)
										{
											if (!string.IsNullOrEmpty(station.Id))
											{
												uint? stationInternalId = dbAccess.GetStationInternalCodeFromOperatorCode(station.Id);

												if (stationInternalId == null)
												{
													return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownStationId;
												}
												stationInternalIds[station.Id] = stationInternalId.Value;
											}
										}

										if (!dbAccess.RegionCodeExist(request.IncomingRequest.RegionCode))
										{
											return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownRegionCode;
										}

										// All verifications OK

										PostValidateManualModeRequest(request, stationInternalIds);

										return null; //no error
									}
								}
								catch (Exception ex)
								{
									LogManager.WriteLog(
										TraceType.ERROR,
										"Error reading LMT database (" + lmtDatabaseFilePath + ")",
										"PIS.Ground.Mission.MissionService.PreValidateManualModeRequest",
										ex,
										EventIdEnum.Mission);

									return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
								}
							}
							else
							{
								return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
							}
						}
						else
						{
							return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
						}
					}
					else
					{
						return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownLmtDataPackageVersion;
					}
				}
			}
			catch (CommunicationException ex)
			{
				throw new PisDataStoreNotAccessibleException(
					"PIS.Ground.Mission.MissionService.PreValidateManualModeRequest",
					ex);
			}
		}

		private static void PostValidateManualModeRequest(ManualActivationRequestContext request, Dictionary<string, uint> stationInternalIds)
		{
			request.OutgoingRequest = new ActivateManualModeRequest();
			request.OutgoingRequest.MissionInfo = new ManualMissionInfoType();

			request.OutgoingRequest.MissionInfo.RequestId = request.RequestId.ToString();
			request.OutgoingRequest.MissionInfo.ElementId = request.IncomingRequest.ElementAlphaNumber;
			request.OutgoingRequest.MissionInfo.LanguageCodeList = new ActivationMissionCommonInfoType.LanguageCodeListType();

			foreach (string code in request.IncomingRequest.LanguageCodeList)
			{
				request.OutgoingRequest.MissionInfo.LanguageCodeList.Add(code);
			}

			request.OutgoingRequest.MissionInfo.LmtDataPackageVersion = request.IncomingRequest.LmtDataPackageVersion;
			request.OutgoingRequest.MissionInfo.MissionId = 0;
			request.OutgoingRequest.MissionInfo.MissionOperatorId = request.IncomingRequest.MissionOperatorCode;
			DateTime startDate = DateTime.Now;

			if (DateTime.TryParseExact(request.IncomingRequest.StartDate,
					DATE_TIME_FORMAT_IN,
					System.Globalization.CultureInfo.InvariantCulture,
					System.Globalization.DateTimeStyles.None, out startDate))
			{
				request.OutgoingRequest.MissionInfo.MissionStartDate = startDate.ToString(DATE_TIME_FORMAT_OUT);
			}

			request.OutgoingRequest.MissionInfo.MissionType = request.IncomingRequest.MissionTypeCode;
			request.OutgoingRequest.MissionInfo.TrainName = request.IncomingRequest.TrainName;
			request.OutgoingRequest.MissionInfo.OnboardServiceCodeList = new ActivationMissionCommonInfoType.OnboardServiceCodeListType();

			foreach (uint serCode in request.IncomingRequest.OnboardServiceCodeList)
			{
				request.OutgoingRequest.MissionInfo.OnboardServiceCodeList.Add(serCode);
			}

			request.OutgoingRequest.MissionInfo.StationList = new ManualMissionInfoType.StationListType();

			foreach (StationList stationIn in request.IncomingRequest.ServicedStationList)
			{
				PIS.Train.Mission.StationListType stationOut = new PIS.Train.Mission.StationListType();

				if (!string.IsNullOrEmpty(stationIn.Id))
				{
					stationOut.Id = stationInternalIds[stationIn.Id];
				}
				else
				{
					stationOut.Id = 0;
				}

				stationOut.Name = stationIn.Name;
				request.OutgoingRequest.MissionInfo.StationList.Add(stationOut);
			}

			request.OutgoingRequest.MissionInfo.StationServiceHourList = new ManualMissionInfoType.StationServiceHourListType();

			foreach (StationServiceHours stHr in request.IncomingRequest.ServiceHourList)
			{
				StationServiceHoursType stHrTyp = new StationServiceHoursType();
				stHrTyp.ArrivalTime = stHr.ArrivalTime;
				stHrTyp.DepartureTime = stHr.DepartureTime;
				request.OutgoingRequest.MissionInfo.StationServiceHourList.Add(stHrTyp);
			}

			request.OutgoingRequest.MissionInfo.CommercialNumberList = new ManualMissionInfoType.CommercialNumberListType();

			foreach (string number in request.IncomingRequest.CommercialNumberList)
			{
				request.OutgoingRequest.MissionInfo.CommercialNumberList.Add(number);
			}

			request.OutgoingRequest.MissionInfo.OnboardValidation = request.IncomingRequest.OnBoardValidationFlag;
			request.OutgoingRequest.MissionInfo.CarNumberingOffsetCode = request.IncomingRequest.CarNumberingOffsetCode;

			request.OutgoingRequest.MissionInfo.RegionCode = request.IncomingRequest.RegionCode;
		}

		private static void TransmitRequest(RequestContext request)
		{
			if (request is AutomaticActivationRequestContext)
			{
				TransmitActivateAutomaticModeRequest(request as AutomaticActivationRequestContext);
			}
			else if (request is ModifiedActivationRequestContext)
			{
				TransmitActivateModifiedModeRequest(request as ModifiedActivationRequestContext);
			}
			else if (request is ManualActivationRequestContext)
			{
				TransmitActivateManualModeRequest(request as ManualActivationRequestContext);
			}
			else if (request is StopMissionRequestContext)
			{
				TransmitStopMissionRequest(request as StopMissionRequestContext);
			}
			else
			{
				// Other request types
				LogManager.WriteLog(
					TraceType.ERROR,
					"Invalid request type.",
					"PIS.Ground.Mission.MissionService.TransmitRequest",
					null,
					EventIdEnum.Mission);
			}

			if (request.State == RequestState.WaitingRetry && request.TransferAttemptsDone == 1)
			{
				// first attempt failed, send notification
				Dictionary<string, string> lParameters = new Dictionary<string, string>();
				lParameters.Add(NOTIFICATION_PARAMETER_ELEMENT_ID, request.ElementId);
				SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressWaitingToSend, lParameters);
			}
		}

		private static void TransmitActivateAutomaticModeRequest(AutomaticActivationRequestContext request)
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
										using (MissionServiceClient client = new MissionServiceClient("MissionEndpoint", lEndpoint))
										{
											try
											{
												PIS.Train.Mission.ResultType result = client.ActivateAutomaticMode(request.OutgoingRequest.MissionInfo);

												SendNotificationToGroundApp(
															request.RequestId,
															PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressSent,
															request.ElementId);

												switch (result)
												{
													case ResultType.Success:
														break;

													case ResultType.ServiceInhibited:
														SendNotificationToGroundApp(
															request.RequestId,
															PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressServiceInhibited,
															request.ElementId);
														break;

													case ResultType.Failure:
														break;
												}

												lTransmitted = true;
											}
											catch (Exception ex)
											{
												LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Mission.MissionService.TransmitActivateAutomaticModeRequest", ex, EventIdEnum.Mission);
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
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline,
												"");
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
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline,
												"");
					break;
				case T2GManagerErrorEnum.eServiceInfoNotFound:
					SendNotificationToGroundApp(
												request.RequestId,
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownServiceId,
												eServiceID.eSrvSIF_MissionServer.ToString());
					break;
				default:
					break;
			}
			request.TransmissionStatus = lTransmitted;
		}

		private static void TransmitActivateModifiedModeRequest(ModifiedActivationRequestContext request)
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
										using (MissionServiceClient client = new MissionServiceClient("MissionEndpoint", lEndpoint))
										{
											try
											{
												PIS.Train.Mission.ResultType result = client.ActivateModifiedMode(request.OutgoingRequest.MissionInfo);

												SendNotificationToGroundApp(
															request.RequestId,
															PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressSent,
															request.ElementId);

												switch (result)
												{
													case ResultType.Success:
														break;

													case ResultType.ServiceInhibited:
														SendNotificationToGroundApp(
															request.RequestId,
															PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressServiceInhibited,
															request.ElementId);
														break;

													case ResultType.Failure:
														break;
												}

												lTransmitted = true;
											}
											catch (Exception ex)
											{
												LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Mission.MissionService.TransmitActivateModifiedModeRequest", ex, EventIdEnum.Mission);
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
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline,
												"");
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
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline,
												"");
					break;
				case T2GManagerErrorEnum.eServiceInfoNotFound:
					SendNotificationToGroundApp(
												request.RequestId,
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownServiceId,
												eServiceID.eSrvSIF_MissionServer.ToString());
					break;
				default:
					break;
			}

			request.TransmissionStatus = lTransmitted;
		}

		private static void TransmitActivateManualModeRequest(ManualActivationRequestContext request)
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
										using (MissionServiceClient client = new MissionServiceClient("MissionEndpoint", lEndpoint))
										{
											try
											{
												PIS.Train.Mission.ResultType result = client.ActivateManualMode(request.OutgoingRequest.MissionInfo);

												SendNotificationToGroundApp(
															request.RequestId,
															PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressSent,
															request.ElementId);

												switch (result)
												{
													case ResultType.Success:
														break;

													case ResultType.ServiceInhibited:
														SendNotificationToGroundApp(
															request.RequestId,
															PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressServiceInhibited,
															request.ElementId);
														break;

													case ResultType.Failure:
														break;
												}

												lTransmitted = true;
											}
											catch (Exception ex)
											{
												LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Mission.MissionService.TransmitActivateManualModeRequest", ex, EventIdEnum.Mission);
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
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline,
												"");
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
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline,
												"");
					break;
				case T2GManagerErrorEnum.eServiceInfoNotFound:
					SendNotificationToGroundApp(
												request.RequestId,
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownServiceId,
												eServiceID.eSrvSIF_MissionServer.ToString());
					break;
				default:
					break;
			}

			request.TransmissionStatus = lTransmitted;
		}

		private static void TransmitStopMissionRequest(StopMissionRequestContext request)
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
										using (MissionServiceClient client = new MissionServiceClient("MissionEndpoint", lEndpoint))
										{
											try
											{
												PIS.Train.Mission.ResultType result = client.StopMission(
													request.StopMissionRequest.RequestId,
													request.StopMissionRequest.ElementId,
													request.StopMissionRequest.MissionOperatorId,
													request.StopMissionRequest.OnboardValidation);

												SendNotificationToGroundApp(
															request.RequestId,
															PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressSent,
															request.ElementId);

												switch (result)
												{
													case ResultType.Success:
														break;

													case ResultType.ServiceInhibited:
														SendNotificationToGroundApp(
															request.RequestId,
															PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressServiceInhibited,
															request.ElementId);
														break;

													case ResultType.Failure:
														break;
												}

												lTransmitted = true;
											}
											catch (Exception ex)
											{
												LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Mission.MissionService.TransmitStopMissionRequest", ex, EventIdEnum.Mission);
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
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline,
												"");
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
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandT2GServerOffline,
												"");
					break;
				case T2GManagerErrorEnum.eServiceInfoNotFound:
					SendNotificationToGroundApp(
												request.RequestId,
												PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownServiceId,
												eServiceID.eSrvSIF_MissionServer.ToString());
					break;
				default:
					break;
			}

			request.TransmissionStatus = lTransmitted;
		}

		private static PIS.Ground.GroundCore.AppGround.NotificationIdEnum? PreValidateStopMissionRequest(string pElementAlphaNumber, string pMissionId)
		{
			if (string.IsNullOrEmpty(pMissionId))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownMissionOperatorCode;
			}

			if (string.IsNullOrEmpty(pElementAlphaNumber))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressUnknownElementId;
			}

			return null;
		}

		internal static void OnTrainNotification(
			Guid pRequestId,
			PIS.Ground.Mission.Notification.NotificationIdEnum pNotificationId,
			string pElementId, string pParameter)
		{
			switch (pNotificationId)
			{
				case PIS.Ground.Mission.Notification.NotificationIdEnum.MissionCommandProgressCancelledByNewCommand:
				case PIS.Ground.Mission.Notification.NotificationIdEnum.MissionCommandProgressLmtDataPackageVersionDifferent:
				case PIS.Ground.Mission.Notification.NotificationIdEnum.MissionCommandProgressMissionIdDifferent:
				case PIS.Ground.Mission.Notification.NotificationIdEnum.MissionCommandProgressNoActiveMission:
				case PIS.Ground.Mission.Notification.NotificationIdEnum.MissionCommandProgressPendingValidation:
				case PIS.Ground.Mission.Notification.NotificationIdEnum.MissionCommandProgressSuccess:
				case PIS.Ground.Mission.Notification.NotificationIdEnum.MissionCommandProgressUnknownElementId:
				case PIS.Ground.Mission.Notification.NotificationIdEnum.MissionCommandProgressValidationRejected:
				case PIS.Ground.Mission.Notification.NotificationIdEnum.MissionCommandProgressTrainIsInMaintenanceMode:
				case PIS.Ground.Mission.Notification.NotificationIdEnum.MissionCommandProgressOngoingBaselineActivation:
				case PIS.Ground.Mission.Notification.NotificationIdEnum.Failed: // there is no other command failure code

					if (pNotificationId != PIS.Ground.Mission.Notification.NotificationIdEnum.Failed) // there is no other command failure code
					{
						// Relay to Ground app directly
						SendNotificationToGroundApp(
								pRequestId,
								(PIS.Ground.GroundCore.AppGround.NotificationIdEnum)pNotificationId,
								pElementId);
					}

					// All notifications (except for PendingValidation) from the train mean that
					// the request is completed and we can remove it.

					if (pNotificationId != PIS.Ground.Mission.Notification.NotificationIdEnum.MissionCommandProgressPendingValidation)
					{
						lock (_lock)
						{
							_externalEvents.Add(new RequestCompletedEvent(pElementId, pRequestId));
						}

						_transmitEvent.Set();
					}

					break;
			}
		}

		internal class MissionControlRequestContext : RequestContext
		{
			public enum ValidationStateEnum { Prepare, Waiting, Validate, ValidationSuccess, ValidationFailure };

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="lrequestId"></param>
			/// <param name="sessionId"></param>
			/// <param name="targetAddress"></param>
			/// <param name="timeout">Request timeout in minutes</param>
			public MissionControlRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout)
				: base("", elementId, requestId, sessionId, timeout)
			{
				ValidationState = ValidationStateEnum.Prepare;
			}

			public ValidationStateEnum ValidationState;
			public string ValidationDataPackagePath;
		}

		internal class AutomaticActivationRequestContext : MissionControlRequestContext
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="lrequestId"></param>
			/// <param name="sessionId"></param>
			/// <param name="targetAddress"></param>
			/// <param name="timeout">Request timeout in minutes</param>
			public AutomaticActivationRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout)
				: base(elementId, requestId, sessionId, timeout)
			{
			}

			public AutomaticModeRequest IncomingRequest { get; set; }

			public ActivateAutomaticModeRequest OutgoingRequest { get; set; }

		}

		internal class ModifiedActivationRequestContext : MissionControlRequestContext
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="lrequestId"></param>
			/// <param name="sessionId"></param>
			/// <param name="targetAddress"></param>
			/// <param name="timeout">Request timeout in minutes</param>
			public ModifiedActivationRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout)
				: base(elementId, requestId, sessionId, timeout)
			{
			}

			public ModifiedModeRequest IncomingRequest { get; set; }

			public ActivateModifiedModeRequest OutgoingRequest { get; set; }
		}

		internal class ManualActivationRequestContext : MissionControlRequestContext
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="lrequestId"></param>
			/// <param name="sessionId"></param>
			/// <param name="targetAddress"></param>
			/// <param name="timeout">Request timeout in minutes</param>
			public ManualActivationRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout)
				: base(elementId, requestId, sessionId, timeout)
			{
			}

			public ManualModeRequest IncomingRequest { get; set; }

			public ActivateManualModeRequest OutgoingRequest { get; set; }
		}

		internal class StopMissionRequestContext : MissionControlRequestContext
		{
			private StopMissionRequest objStopMissionRequest;
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="lrequestId"></param>
			/// <param name="sessionId"></param>
			/// <param name="targetAddress"></param>
			/// <param name="timeout">Request timeout in minutes</param>
			public StopMissionRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout, StopMissionRequest objpStopMissionRequest)
				: base(elementId, requestId, sessionId, timeout)
			{
				objStopMissionRequest = objpStopMissionRequest;
			}
			public StopMissionRequest StopMissionRequest
			{
				get
				{
					return objStopMissionRequest;
				}
			}
		}
		/// <summary>
		///
		/// </summary>
		internal class ExternalEvent
		{
			/// <summary>
			/// Constructor
			/// </summary>
		}

		/// <summary>
		///
		/// </summary>
		internal class RequestCompletedEvent : ExternalEvent
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

		internal class PisDataStoreNotAccessibleException : Exception
		{
			public PisDataStoreNotAccessibleException()
				: base() { }

			public PisDataStoreNotAccessibleException(string pMessage, Exception pInnerException)
				: base(pMessage + " [" + pInnerException.Message + "]", pInnerException) { }

		}
	}
}
