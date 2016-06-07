//---------------------------------------------------------------------------------------------------
// <copyright file="LiveVideoControlService.svc.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using System.Globalization;
using LiveVideoControl;
using PIS.Ground.Core.Utility;
using PIS.Ground.Common;
using System.Threading;

namespace PIS.Ground.LiveVideoControl
{
	/// <summary>Live video control service.</summary>
	[CreateOnDispatchService(typeof(LiveVideoControlService))]
	[ServiceBehavior(Namespace = "http://alstom.com/pacis/pis/ground/livevideocontrol/")]
	public class LiveVideoControlService : ILiveVideoControlService
	{
		#region Constants

		/// <summary>Identifier for event subscription.</summary>
		private const string SubscriberId = "PIS.Ground.LiveVideoControl.LiveVideoControlService";

		#endregion

		#region Private fields

		private static volatile bool _initialized = false;

		private static object _initializationLock = new object();

		private static IT2GManager _t2gManager = null;

		private static INotificationSender _notificationSender = null;

		/// <summary>Manager for session. Used for checking input request id and resolving notification urls.</summary>
		private static ISessionManagerExtended _sessionManager = null;

		/// <summary>The request processor instance.</summary>
		private static IRequestProcessor _requestProcessor = null;

		/// <summary>The configuration accessor interface.</summary>
		private static ILiveVideoControlConfiguration _configuration = null;

		/// <summary>The delegate to add a status request to request processing.</summary>
		private static SendVideoStreamingStatusRequestDelegate _delegate = new SendVideoStreamingStatusRequestDelegate(LiveVideoControlService.SendVideoStreamingStatusRequest);

		private static List<Guid> _statusRequestedSessions = new List<Guid>();

		/// <summary>Multithread protection for accessing automatic/manual command mode fields.</summary>
		private static object _automaticModeLock = new object();

		/// <summary>Current command mode.</summary>
		private static bool _isAutomaticMode = false;

		/// <summary>The dictionary with the latest url sent on START command. In case of STOP the value is removed.</summary>
		private static Dictionary<TargetAddressType, string> _dicVideoHistory = new Dictionary<TargetAddressType, string>();

		/// <summary>The dictionary video history with the latest sent status. This is used to avoid sending multiple.
		///          START command to live video. It is reset in case the service is off.
		/// </summary>
		private static Dictionary<TargetAddressType, bool> _dicVideoHistorySentStatus = new Dictionary<TargetAddressType, bool>();

		#endregion

		#region start methods

		/// <summary>Initializes a new instance of the LiveVideoControlService class.</summary>
		public LiveVideoControlService()
		{
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "LiveVideoControlService";
            }

			Initialize();
		}

		/// <summary>
		/// Initializes the LiveVideoControl web service. This is only called once, when the service is
		/// instantiated.
		/// </summary>
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

							LiveVideoControlConfiguration.InitializeConfiguration();

							// Register a callback that will start streaming on new trains
							_t2gManager.SubscribeToElementChangeNotification(
								SubscriberId,
								new EventHandler<ElementEventArgs>(OnElementInfoChanged));

							_requestProcessor = new RequestProcessor();

							_requestProcessor.SetT2GManager(_t2gManager, ConfigurationSettings.AppSettings["ApplicationId"]);
							_requestProcessor.SetSessionMgr(_sessionManager);
							_requestProcessor.SetNotificationSender(_notificationSender);

							LoadConfiguration(new LiveVideoControlConfiguration());
						}
						catch (System.Exception e)
						{
							LogManager.WriteLog(TraceType.ERROR, e.Message, "PIS.Ground.Maintenance.MaintenanceService.Initialize", e, EventIdEnum.LiveVideoControl);
						}

                        _initialized = true;
					}
				}
			}
		}

		/// <summary>
		/// Initializes the LiveVideoControl web service. This is only called once, when the service is
		/// instantiated.
		/// </summary>
		/// <param name="train2groundClient">The T2G client to use local data store.</param>
		/// <param name="sessionManager">Manager for session.</param>
		/// <param name="requestProcessor">The request processor instance.</param>
		/// <param name="configuration">The configuration accessor instance.</param>
		public static void Initialize(
			IT2GManager train2groundManager,
			ISessionManagerExtended sessionManager,
			INotificationSender notificationSender,
			IRequestProcessor requestProcessor,
			ILiveVideoControlConfiguration configuration)
		{
			try
			{
				if (train2groundManager != null)
				{
					_t2gManager = train2groundManager;

					// Register a callback that will start streaming on new trains
					_t2gManager.SubscribeToElementChangeNotification(
						SubscriberId,
						new EventHandler<ElementEventArgs>(OnElementInfoChanged));
				}

				if (sessionManager != null)
				{
					_sessionManager = sessionManager;
				}

				if (notificationSender != null)
				{
					_notificationSender = notificationSender;
				}

				if (requestProcessor != null)
				{
					_requestProcessor = requestProcessor;
					_requestProcessor.SetT2GManager(_t2gManager, ConfigurationSettings.AppSettings["ApplicationId"]);
					_requestProcessor.SetSessionMgr(_sessionManager);
					_requestProcessor.SetNotificationSender(_notificationSender);
				}

				if (configuration != null)
				{
					LoadConfiguration(configuration);
				}

				_initialized = true;
			}
			catch (System.Exception e)
			{
				LogManager.WriteLog(TraceType.ERROR, e.Message, "PIS.Ground.Maintenance.MaintenanceService.Initialize", e, EventIdEnum.LiveVideoControl);
			}
		}

		/// <summary>Loads a configuration.</summary>
		/// <param name="configuration">The configuration accessor interface.</param>
		public static void LoadConfiguration(ILiveVideoControlConfiguration configuration)
		{
			if (configuration != null)
			{
				lock (_automaticModeLock)
				{
					LiveVideoControlService._configuration = configuration;

					if (string.IsNullOrEmpty(LiveVideoControlService._configuration.AutomaticModeURL))
					{
						_isAutomaticMode = false;
					}
					else
					{
						_isAutomaticMode = true;

						ProcessAutomaticMode(LiveVideoControlService._configuration.AutomaticModeURL);
					}
				}
			}
		}

		/// <summary>
		/// Callback called when Element Online state changes (signaled by the T2G Client).
		/// </summary>
		/// <param name="sender">Source of the event.</param>
		/// <param name="args">Event information to send to registered event handlers.</param>
		public static void OnElementInfoChanged(object sender, ElementEventArgs args)
		{
			if (args != null &&
				args.SystemInformation != null &&
				args.SystemInformation.IsOnline == true &&
				args.SystemInformation.PisMission != null &&
				args.SystemInformation.PisMission.MissionState == MissionStateEnum.MI)
			{
				// Will be called multiple times for the same train.
				// Some sort of debouncing will be welcome in a future revision
				// to prevent multiple notifications to the console(s)

				if (string.IsNullOrEmpty(args.SystemInformation.SystemId) == false)
				{
					TargetAddressType target = new TargetAddressType();
					target.Type = AddressTypeEnum.Element;
					target.Id = args.SystemInformation.SystemId;

					if (_isAutomaticMode == true)
					{
						LogManager.WriteLog(
							TraceType.INFO,
							"Automatically starting streaming on newly detected train: " + args.SystemInformation.SystemId,
							"PIS.Ground.LiveVideoControl.LiveVideoControlService.OnElementInfoChanged",
							null,
							EventIdEnum.LiveVideoControl);

						LiveVideoControlResult result =
							SendStartStreamingCommand(Guid.Empty, target, _configuration.AutomaticModeURL);

						if (result.ResultCode != LiveVideoControlErrorEnum.RequestAccepted)
						{
							LogManager.WriteLog(TraceType.ERROR,
								"Problem sending a start command with url "
								+ _configuration.AutomaticModeURL
								+ " to train "
								+ target.Id
								+ ". Error: "
								+ result.ResultCode.ToString(),

								"PIS.Ground.LiveVideoControl.LiveVideoControlService.OnElementInfoChanged",
								null, EventIdEnum.LiveVideoControl);
						}
					}
					else
					{
						// Manual Mode, resend the latest Start command if available
						if (_dicVideoHistory.ContainsKey(target) && _dicVideoHistorySentStatus.ContainsKey(target))
						{
							bool lServiceLiveVideoControlServerAvailable = true;

							if (args.SystemInformation.ServiceList != null)
							{
								// Checking if LiveVideo service on the Element is available
								foreach (ServiceInfo lServiceInfo in args.SystemInformation.ServiceList)
								{
									if (lServiceInfo.IsAvailable == false && lServiceInfo.ServiceId == (ushort)eServiceID.eSrvSIF_LiveVideoControlServer)
									{
										lServiceLiveVideoControlServerAvailable = false;
										// In case of not available, reset the resend flag to send
										// the start command again when the service becomes available
										if (_dicVideoHistorySentStatus[target] == true)
										{
											_dicVideoHistorySentStatus[target] = false;
										}
									}
								}
							}

							// Avoiding sending multiple start notifications.
							// The LiveVideoService have to be online
							if (_dicVideoHistorySentStatus[target] == false && lServiceLiveVideoControlServerAvailable == true)
							{

								LogManager.WriteLog(
									TraceType.INFO,
									"Re-starting streaming on newly detected train: " + args.SystemInformation.SystemId,
									"PIS.Ground.LiveVideoControl.LiveVideoControlService.OnElementInfoChanged",
									null,
									EventIdEnum.LiveVideoControl);

								LiveVideoControlResult result =
									SendStartStreamingCommand(Guid.Empty, target, _dicVideoHistory[target]);
								// Setting the flag that the start command was already sent
								_dicVideoHistorySentStatus[target] = true;

								if (result.ResultCode != LiveVideoControlErrorEnum.RequestAccepted)
								{
									LogManager.WriteLog(TraceType.ERROR,
										"Problem sending a start command with url "
										+ _dicVideoHistory[target]
										+ " to train "
										+ target.Id
										+ ". Error: "
										+ result.ResultCode.ToString(),

										"PIS.Ground.LiveVideoControl.LiveVideoControlService.OnElementInfoChanged",
										null, EventIdEnum.LiveVideoControl);
								}
							}
						}
					}
				}
			}
		}

		#endregion

		#region static internal

		/// <summary>Gets invalid target address response.</summary>
		/// <param name="targetAddress">Addressee information.</param>
		/// <returns>The invalid target address response.</returns>
		private static LiveVideoControlErrorEnum GetInvalidTargetAddressResponse(TargetAddressType targetAddress)
		{
			LiveVideoControlErrorEnum result = LiveVideoControlErrorEnum.UnknownElementId;

			if (targetAddress != null)
			{
				switch (targetAddress.Type)
				{
					case AddressTypeEnum.Element:
						result = LiveVideoControlErrorEnum.UnknownElementId;
						break;
					case AddressTypeEnum.MissionCode:
						result = LiveVideoControlErrorEnum.UnknownMissionId;
						break;
					case AddressTypeEnum.MissionOperatorCode:
						result = LiveVideoControlErrorEnum.UnknownMissionId;
						break;
				}
			}

			return result;
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
			LogManager.WriteLog(TraceType.INFO, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_CALLED, "SendNotificationToGroundApp"), "PIS.Ground.LiveVideoControl.LiveVideoControlService.SendNotificationToGroundApp", null, EventIdEnum.Mission);
			try
			{
				//send notification to ground app
				_notificationSender.SendNotification(status, parameter, requestId);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.ERROR, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_EXCEPTION, "SendNotificationToGroundApp"), "PIS.Ground.LiveVideoControl.LiveVideoControlService.SendNotificationToGroundApp", ex, EventIdEnum.Mission);
			}
		}

		/// <summary>Send Notification to Ground App, specific to Element Id.</summary>
		/// <param name="requestId">The RequestId for the corresponding request.</param>
		/// <param name="pStatus">Status : Completed/Failed/Processing.</param>
		/// <param name="pElementId">The concerned ElementId.</param>
		public static void SendElementIdNotificationToGroundApp(string requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum pStatus, string pElementId)
		{
			//String.Format(CultureInfo.CurrentCulture, Logs.INFO_NOTIFICATIONTOAPPGROUND, pElementId, requestId, pStatus)
			LogManager.WriteLog(TraceType.INFO, String.Format(CultureInfo.CurrentCulture, Logs.INFO_NOTIFICATIONTOAPPGROUND, pElementId, requestId, pStatus), "PIS.Ground.DataPackage.LiveVideoControl.sendElementIdNotificationToGroundApp", null, EventIdEnum.LiveVideoControl);
			try
			{
				// serialize ElementId.
				System.Xml.Serialization.XmlSerializer xmlSzr = new System.Xml.Serialization.XmlSerializer(typeof(string));
				StringWriter lstr = new StringWriter();
				xmlSzr.Serialize(lstr, pElementId);
				LiveVideoControlService.SendNotificationToGroundApp(new Guid(requestId), pStatus, lstr.ToString());
			}
			catch (FormatException)
			{
				// serialize ElementId.
				System.Xml.Serialization.XmlSerializer xmlSzr = new System.Xml.Serialization.XmlSerializer(typeof(string));
				StringWriter lstr = new StringWriter();
				xmlSzr.Serialize(lstr, pElementId);
				LiveVideoControlService.SendNotificationToGroundApp(Guid.Empty, pStatus, lstr.ToString());
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(
									TraceType.ERROR,
									ex.Message,
									"PIS.Ground.LiveVideoControl.LiveVideoControlService.sendNotificationToGroundApp",
									ex,
									EventIdEnum.DataPackage);
			}
		}

		/// <summary>DelegateAutomaticModeBackgroundProcessing.</summary>
		/// <param name="url">URL of the document.</param>
		public delegate void DelegateAutomaticModeBackgroundProcessing(string url);

		/// <summary>Performing the necessary operations when entering the automatic mode.</summary>
		/// <param name="url">Streaming URL.</param>
		/// <returns>.</returns>
		private static LiveVideoControlResult ProcessAutomaticMode(string url)
		{
			LogManager.WriteLog(
				TraceType.INFO,
				"Entering automatic mode",
				"PIS.Ground.LiveVideoControl.LiveVideoControlService.ProcessAutomaticMode",
				null,
				EventIdEnum.LiveVideoControl);

			LiveVideoControlResult result = new LiveVideoControlResult();
			result.RequestId = Guid.Empty;
			result.ResultCode = LiveVideoControlErrorEnum.InternalError;

			// Starting a background process to send start commands to the trains that have the
			// desired status
			DelegateAutomaticModeBackgroundProcessing handler = SendStartStreamingCommandToProperStatusElements;
			handler.BeginInvoke(url, handler.EndInvoke, null);

			result.Url = url;
			result.ResultCode = LiveVideoControlErrorEnum.RequestAccepted;

			return result;
		}

		private static void SendStartStreamingCommandToProperStatusElements(string url)
		{
			try
			{
				// Get the list of all currently available trains
				ElementList<AvailableElementData> elements = new ElementList<AvailableElementData>();
				T2GManagerErrorEnum rqstResult = _t2gManager.GetAvailableElementDataList(out elements);

				switch (rqstResult)
				{
					case T2GManagerErrorEnum.eSuccess:
						// Send notification to all clients
						LiveVideoControlService.SendNotificationToGroundApp(
							Guid.Empty,
							PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlAutomaticMode,
							url);

						// Send a start command to each train that is online and in mission
						foreach (AvailableElementData element in elements)
						{
							TargetAddressType target = new TargetAddressType();
							target.Type = AddressTypeEnum.Element;
							target.Id = element.ElementNumber;

							if (_dicVideoHistory.ContainsKey(target))
							{
								_dicVideoHistory[target] = url;
							}
							else
							{
								_dicVideoHistory.Add(target, url);
							}

							if (_dicVideoHistorySentStatus.ContainsKey(target))
							{
								_dicVideoHistorySentStatus[target] = true;
							}
							else
							{
								_dicVideoHistorySentStatus.Add(target, true);
							}

							if (element.OnlineStatus == true && element.MissionState == MissionStateEnum.MI)
							{
								SendStartStreamingCommand(Guid.Empty, target, url);
							}
						}
						break;

					case T2GManagerErrorEnum.eT2GServerOffline:
						LogManager.WriteLog(TraceType.ERROR, "T2G Server: Offline",
						"PIS.Ground.LiveVideoControl.LiveVideoControlService.ProcessAutomaticMode",
						null, EventIdEnum.LiveVideoControl);
						break;

					default:
						LogManager.WriteLog(TraceType.ERROR, "Element not found",
							"PIS.Ground.LiveVideoControl.LiveVideoControlService.ProcessAutomaticMode", null, EventIdEnum.LiveVideoControl);
						break;
				}
			}
			catch (System.Exception exception)
			{
				LogManager.WriteLog(TraceType.EXCEPTION, exception.Message,
					"PIS.Ground.LiveVideoControl.LiveVideoControlService.ProcessAutomaticMode", exception, EventIdEnum.LiveVideoControl);
			}
		}

		/// <summary>Sets automatic mode. If already in this mode, the command is rejected</summary>
		/// <param name="url">The streaming URL to be used.</param>
		/// <returns>true if the previous mode was manual, false otherwise.</returns>
		private static bool SetAutomaticMode(string url)
		{
			bool isModeModified = false;

			lock (_automaticModeLock)
			{
				if (_isAutomaticMode == false)
				{
					isModeModified = true;
					_isAutomaticMode = true;

					if (_configuration != null)
					{
						_configuration.AutomaticModeURL = url;
					}
				}
			}
			return isModeModified;
		}

		/// <summary>Sets manual mode. If already in this mode, the command is rejected</summary>
		/// <returns>true if the previous mode was automatic, false otherwise.</returns>
		private static bool ClearAutomaticMode()
		{
			bool isModeModified = false;

			lock (_automaticModeLock)
			{
				if (_isAutomaticMode == true)
				{
					isModeModified = true;
					_isAutomaticMode = false;

					if (_configuration != null)
					{
						_configuration.AutomaticModeURL = null;
					}
				}
			}
			return isModeModified;
		}

		/// <summary>Gets current operation mode.</summary>
		/// <param name="url">If automatic mode, the streaming URL used. If manual, null</param>
		/// <returns>true if automatic mode, false if manual.</returns>
		private static bool GetAutomaticMode(out string url)
		{
			bool isAutomaticMode = false;
			url = null;

			lock (_automaticModeLock)
			{
				isAutomaticMode = _isAutomaticMode;
				if (isAutomaticMode)
				{
					url = _configuration.AutomaticModeURL;
				}
			}
			return isAutomaticMode;
		}

		/// <summary>Gets current operation mode.</summary>
		/// <returns>true if automatic mode, false if manual.</returns>
		private static bool GetAutomaticMode()
		{
			string url;
			return GetAutomaticMode(out url);
		}

		#endregion

		#region web service request

		/// <summary>LiveVideoControlService web service method "GetAvailableElementList".</summary>
		/// <param name="sessionId">The session identifier.</param>
		/// <returns>Response <see cref="LiveVideoControlElementListResult"/>.</returns>
		LiveVideoControlElementListResult ILiveVideoControlService.GetAvailableElementList(Guid sessionId)
		{
			LiveVideoControlElementListResult lResult = new LiveVideoControlElementListResult();
			lResult.ResultCode = LiveVideoControlErrorEnum.ElementListNotAvailable;

			if (LiveVideoControlService._sessionManager.IsSessionValid(sessionId))
			{
				// Get the list of all trains
				// 
				ElementList<AvailableElementData> lElementList;
				T2GManagerErrorEnum lT2GResult = _t2gManager.GetAvailableElementDataList(out lElementList);

				if (lT2GResult == T2GManagerErrorEnum.eSuccess)
				{
					var lLiveVideoServiceElementList = new ElementList<AvailableElementData>();
					ServiceInfo lServiceInfo;
					bool lT2GDisconnection = false;

					// For each train...
					// 
					foreach (AvailableElementData lElement in lElementList)
					{
						// ...get information related to the live video streaming service
						// 
						lT2GResult = _t2gManager.GetAvailableServiceData(lElement.ElementNumber,
							(int)eServiceID.eSrvSIF_LiveVideoControlServer, out lServiceInfo);

						switch (lT2GResult)
						{
							case T2GManagerErrorEnum.eSuccess:

								// If the streaming service is provided...
								// 
								if (lServiceInfo.IsAvailable)
								{
									// ...add the train to the streaming service elements list
									lLiveVideoServiceElementList.Add(lElement);
								}
								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								lT2GDisconnection = true;
								break;
							default:
								break;
						}

						// Since we need T2G to build the list of streaming service elements...
						if (lT2GDisconnection)
						{
							// ...stop wasting time and return immediately
							lResult.ResultCode = LiveVideoControlErrorEnum.T2GServerOffline;
							break;
						}
					}

					if (!lT2GDisconnection)
					{
						// In case of success, return the list of train(s) that provide video steaming service
						// 
						lResult.ElementList = lLiveVideoServiceElementList;
						lResult.ResultCode = LiveVideoControlErrorEnum.RequestAccepted;
					}
					else
					{
						lResult.ResultCode = LiveVideoControlErrorEnum.T2GServerOffline;
					}
				}
				else
				{
					lResult.ResultCode = LiveVideoControlErrorEnum.ElementListNotAvailable;
				}

				if (false == LiveVideoControlService._statusRequestedSessions.Contains(sessionId))
				{
					LiveVideoControlService._statusRequestedSessions.Add(sessionId);
					LiveVideoControlService._delegate(sessionId);
				}
			}
			else
			{
				lResult.ResultCode = LiveVideoControlErrorEnum.InvalidSessionId;
			}

			return lResult;
		}

		/// <summary>
		/// LiveVideoControlService Web service method "StartVideoStreamingCommand" that sends a
		/// scheduled message to addressee.
		/// </summary>
		/// <param name="sessionId">The session identifier.</param>
		/// <param name="targetAddress">Addressee information.</param>
		/// <param name="url">The request timeout.</param>
		/// <returns>Response <see cref="LiveVideoControlResult"/>.</returns>
		LiveVideoControlResult ILiveVideoControlService.StartVideoStreamingCommand(
			Guid sessionId,
			TargetAddressType targetAddress,
			string url)
		{
			LiveVideoControlResult result = new LiveVideoControlResult();
			result.RequestId = Guid.Empty;
			result.ResultCode = LiveVideoControlErrorEnum.RequestAccepted;

			string automaticModeURL;

			if (_sessionManager.IsSessionValid(sessionId))
			{
				if (GetAutomaticMode(out automaticModeURL) == false)
				{
					if (_dicVideoHistory.ContainsKey(targetAddress))
					{
						_dicVideoHistory[targetAddress] = url;
					}
					else
					{
						_dicVideoHistory.Add(targetAddress, url);
					}

					if (_dicVideoHistorySentStatus.ContainsKey(targetAddress))
					{
						_dicVideoHistorySentStatus[targetAddress] = true;
					}
					else
					{
						_dicVideoHistorySentStatus.Add(targetAddress, true);
					}

					result = SendStartStreamingCommand(
						sessionId,
						targetAddress,
						url
						);
				}
				else
				{
					result.Url = automaticModeURL;
					result.ResultCode = LiveVideoControlErrorEnum.AutomaticModeActivated;
				}
			}
			else
			{
				result.ResultCode = LiveVideoControlErrorEnum.InvalidSessionId;
			}

			return result;
		}

		/// <summary>Sends a start streaming command.</summary>
		/// <param name="sessionId">The session identifier.</param>
		/// <param name="targetAddress">Addressee information.</param>
		/// <param name="url">The streaming URL to be used.</param>
		/// <returns>Response <see cref="LiveVideoControlElementListResult"/>.</returns>
		private static LiveVideoControlResult SendStartStreamingCommand(
			Guid sessionId,
			TargetAddressType targetAddress,
			string url)
		{
			LiveVideoControlResult result = new LiveVideoControlResult();
			result.RequestId = Guid.Empty;
			result.ResultCode = LiveVideoControlErrorEnum.InternalError;
			Guid requestId = Guid.Empty;
			string error;

			if (sessionId != Guid.Empty)
			{
				error = _sessionManager.GenerateRequestID(sessionId, out requestId);
			}
			else
			{
				error = _sessionManager.GenerateRequestID(out requestId);
			}

			if (requestId != Guid.Empty)
			{
				ElementList<AvailableElementData> elements;
				T2GManagerErrorEnum rqstResult = _t2gManager.GetAvailableElementDataByTargetAddress(targetAddress, out elements);

				switch (rqstResult)
				{
					case T2GManagerErrorEnum.eSuccess:
						Guid notificationRequestId = requestId;
						List<RequestContext> newRequests = new List<RequestContext>();
						foreach (AvailableElementData element in elements)
						{
							LiveVideoControlService.SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlDistributionProcessing, element.ElementNumber);
							ProcessStartVideoStreamingCommandRequestContext request = new ProcessStartVideoStreamingCommandRequestContext(
								element.ElementNumber,
								requestId,
								sessionId,
								url);

							newRequests.Add(request);
						}

						_requestProcessor.AddRequestRange(newRequests);
						result.RequestId = requestId;
						result.ResultCode = LiveVideoControlErrorEnum.RequestAccepted;
						break;
					case T2GManagerErrorEnum.eT2GServerOffline:
						LogManager.WriteLog(TraceType.ERROR, "T2G Offline", "PIS.Ground.LiveVideoControl.LiveVideoControlService.SendStartStreamingCommand", null, EventIdEnum.LiveVideoControl);
						result.ResultCode = LiveVideoControlErrorEnum.T2GServerOffline;
						break;
					case T2GManagerErrorEnum.eElementNotFound:
						LogManager.WriteLog(TraceType.ERROR, "Element not found", "PIS.Ground.LiveVideoControl.LiveVideoControlService.SendStartStreamingCommand", null, EventIdEnum.LiveVideoControl);
						result.ResultCode = LiveVideoControlService.GetInvalidTargetAddressResponse(targetAddress);
						break;
					default:
						LogManager.WriteLog(TraceType.ERROR, "Problem looking for an element. T2GClient returned: " + rqstResult.ToString(),
							"PIS.Ground.LiveVideoControl.LiveVideoControlService.SendStartStreamingCommand", null, EventIdEnum.LiveVideoControl);
						result.ResultCode = LiveVideoControlErrorEnum.InternalError;
						break;
				}
			}
			else
			{
				LogManager.WriteLog(TraceType.ERROR, error, "PIS.Ground.LiveVideoControl.LiveVideoControlService.SendStartStreamingCommand", null, EventIdEnum.LiveVideoControl);
				result.ResultCode = LiveVideoControlErrorEnum.InvalidRequestID;
			}

			return result;
		}

		/// <summary>
		/// LiveVideoControlService Web service method "StopVideoStreamingCommand" that cancels all
		/// predefined and free-text messages at addressee.
		/// </summary>
		/// <param name="sessionId">The session identifier.</param>
		/// <param name="targetAddress">Addressee information.</param>
		/// <returns>Response <see cref="LiveVideoControlResult"/>.</returns>
		LiveVideoControlResult ILiveVideoControlService.StopVideoStreamingCommand(
			Guid sessionId,
			TargetAddressType targetAddress)
		{
			LiveVideoControlResult result = new LiveVideoControlResult();
			result.RequestId = Guid.Empty;
			result.ResultCode = LiveVideoControlErrorEnum.RequestAccepted;

			_dicVideoHistory.Remove(targetAddress);
			_dicVideoHistorySentStatus.Remove(targetAddress);

			if (_sessionManager.IsSessionValid(sessionId))
			{
				string automaticModeURL;

				if (GetAutomaticMode(out automaticModeURL) == false)
				{
					Guid requestId = Guid.Empty;
					string error = _sessionManager.GenerateRequestID(sessionId, out requestId);

					if (requestId != Guid.Empty)
					{
						ElementList<AvailableElementData> elements;
						T2GManagerErrorEnum rqstResult = _t2gManager.GetAvailableElementDataByTargetAddress(targetAddress, out elements);
						switch (rqstResult)
						{
							case T2GManagerErrorEnum.eSuccess:
								{
									List<RequestContext> newRequests = new List<RequestContext>();
									foreach (AvailableElementData element in elements)
									{
										LiveVideoControlService.SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlDistributionProcessing, element.ElementNumber);

										ProcessStopVideoStreamingCommandRequestContext request = new ProcessStopVideoStreamingCommandRequestContext(
											element.ElementNumber,
											requestId,
											sessionId);

										newRequests.Add(request);
									}

									_requestProcessor.AddRequestRange(newRequests);
									result.RequestId = requestId;
									result.ResultCode = LiveVideoControlErrorEnum.RequestAccepted;
								}

								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								result.ResultCode = LiveVideoControlErrorEnum.T2GServerOffline;
								break;
							case T2GManagerErrorEnum.eElementNotFound:
								result.ResultCode = LiveVideoControlService.GetInvalidTargetAddressResponse(targetAddress);
								break;
							default:
								break;
						}
					}
					else
					{
						LogManager.WriteLog(TraceType.ERROR, error, "PIS.Ground.LiveVideoControl.LiveVideoControlService.StopVideoStreamingCommand", null, EventIdEnum.LiveVideoControl);
						result.ResultCode = LiveVideoControlErrorEnum.InvalidRequestID;
					}
				}
				else
				{
					result.Url = automaticModeURL;
					result.ResultCode = LiveVideoControlErrorEnum.AutomaticModeActivated;
				}
			}
			else
			{
				result.ResultCode = LiveVideoControlErrorEnum.InvalidSessionId;
			}

			return result;
		}

		/// <summary>Command to select the automatic or manual starting mode for the video streaming.
		///          If the specified URL is empty or null, manual mode is selected. 
		///          Otherwise, automatic mode is selected</summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="url">URL of the stream to be used in automatic mode.</param>
		/// <returns>Return statement of the request (Success, failed,...) with URL and request id.</returns>
		LiveVideoControlResult ILiveVideoControlService.ChangeCommandMode(Guid sessionId, string url)
		{
			LiveVideoControlResult result = new LiveVideoControlResult();
			result.RequestId = Guid.Empty;
			result.ResultCode = LiveVideoControlErrorEnum.InternalError;

			if (_sessionManager.IsSessionValid(sessionId))
			{
				// Checking if the specified url contains an non empty string
				if (string.IsNullOrEmpty(url) || url.Trim().Length == 0)
				{
					// If empty url, it means we want to clear the automatic mode
					if (ClearAutomaticMode())
					{
						// Notify all clients that we are entering the manual mode
						LiveVideoControlService.SendNotificationToGroundApp(
							Guid.Empty,
							PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlManualMode,
							null);
					}
					result.ResultCode = LiveVideoControlErrorEnum.RequestAccepted;
				}
				else
				{
					// If non empty url, it means we want to enter the automatic mode
					lock (_automaticModeLock)
					{
						if (SetAutomaticMode(url))
						{
							// Send notifications and start commands
							result = ProcessAutomaticMode(url);

							// If the command failed, restore the manual mode
							if (result.ResultCode != LiveVideoControlErrorEnum.RequestAccepted)
							{
								ClearAutomaticMode();
							}
						}
						else
						{
							// We are already in automatic mode. Get the current url used.
							string currentAutomaticURL;
							GetAutomaticMode(out currentAutomaticURL);
							result.Url = currentAutomaticURL;
							result.ResultCode = LiveVideoControlErrorEnum.AutomaticModeActivated;
						}
					}
				}
			}
			else
			{
				result.ResultCode = LiveVideoControlErrorEnum.InvalidSessionId;
			}

			return result;
		}

		/// <summary>Command to determine the current command mode.</summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <returns>Null if manual mode, streaming URL if automatic mode</returns>
		LiveVideoControlResult ILiveVideoControlService.GetCommandMode(Guid sessionId)
		{
			LiveVideoControlResult result = new LiveVideoControlResult();
			result.RequestId = Guid.Empty;
			result.ResultCode = LiveVideoControlErrorEnum.InternalError;
			result.Url = null;

			if (_sessionManager.IsSessionValid(sessionId))
			{

				string url;
				GetAutomaticMode(out url);
				result.Url = url;
				result.ResultCode = LiveVideoControlErrorEnum.RequestAccepted;
			}
			else
			{
				result.ResultCode = LiveVideoControlErrorEnum.InvalidSessionId;
			}

			return result;
		}

		#endregion

		#region processing

		/// <summary>Sends a video streaming status request delegate.</summary>
		/// <param name="requestId">Identifier for the request.</param>
		private delegate void SendVideoStreamingStatusRequestDelegate(Guid requestId);

		/// <summary>Sends a video streaming status request.</summary>
		/// <param name="requestId">Request ID for the corresponding request.</param>
		private static void SendVideoStreamingStatusRequest(Guid requestId)
		{
			ElementList<AvailableElementData> availableElementData = new ElementList<AvailableElementData>();
			T2GManagerErrorEnum result = _t2gManager.GetAvailableElementDataList(out availableElementData);
			switch (result)
			{
				case T2GManagerErrorEnum.eSuccess:
					foreach (AvailableElementData element in availableElementData)
					{
						ServiceInfo serviceInfo;
						result = _t2gManager.GetAvailableServiceData(element.ElementNumber, (int)eServiceID.eSrvSIF_LiveVideoControlServer, out serviceInfo);
						switch (result)
						{
							case T2GManagerErrorEnum.eSuccess:
								ProcessSendVideoStreamingStatusRequestContext request = new ProcessSendVideoStreamingStatusRequestContext(
										element.ElementNumber,
										requestId,
										serviceInfo);
								_requestProcessor.AddRequest(request);
								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								LiveVideoControlService.SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlT2GServerOffline, string.Empty);
								break;
							case T2GManagerErrorEnum.eElementNotFound:
								LiveVideoControlService.SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlElementNotFound, element.ElementNumber);
								break;
							case T2GManagerErrorEnum.eServiceInfoNotFound:
								LiveVideoControlService.SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlServiceNotFound, element.ElementNumber);
								break;
							default:
								break;
						}
					}

					break;
				case T2GManagerErrorEnum.eT2GServerOffline:
					LiveVideoControlService.SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.LiveVideoControlT2GServerOffline, string.Empty);
					break;
				default:
					break;
			}
		}

		#endregion
	}
}