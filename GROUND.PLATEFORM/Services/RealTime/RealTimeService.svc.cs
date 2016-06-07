//---------------------------------------------------------------------------------------------------
// <copyright file="RealTimeService.svc.cs" company="Alstom">
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
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.PackageAccess;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.RemoteDataStore;
using PIS.Ground.Core;
using PIS.Ground.Core.Utility;
using PIS.Ground.Common;
using PIS.Ground.Core.Common;

namespace PIS.Ground.RealTime
{
	/// <summary>Real time service.</summary>
    [CreateOnDispatchService(typeof(RealTimeService))]
	[ServiceBehavior(Namespace = "http://alstom.com/pacis/pis/ground/realtime/")]
	public class RealTimeService : IRealTimeService
	{		
		#region fields

        private static volatile bool _initialized = false;

        private static object _initializationLock = new object();

        private static IT2GManager _t2gManager = null;
		
		/// <summary>
		/// Manager for session. Used for checking input request id and resolving notification urls.
		/// </summary>
		private static ISessionManagerExtended _sessionManager = null;

        private static INotificationSender _notificationSender = null;

		/// <summary>The request processor instance.</summary>
		private static IRequestProcessor _requestProcessor = null;
		
		/// <summary>The remote data store factory.</summary>
		private static IRemoteDataStoreFactory _remoteDataStoreFactory = null;

		/// <summary>The rtpis data store.</summary>
		private static IRTPISDataStore _rtpisDataStore = null;

		/// <summary>Type of the platform.</summary>
		protected static CommonConfiguration.PlatformTypeEnum? _platformType = null;

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the RealTimeService class.</summary>
		public RealTimeService()
		{
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "RealTimeService";
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
                            					                                    
                            _remoteDataStoreFactory = new RemoteDataStoreFactory();				
					
                            _rtpisDataStore = new RTPISDataStore();
				
					        _requestProcessor = new RequestProcessor(
                                _t2gManager,
                                _rtpisDataStore);
                            
				        }
                        catch (System.Exception e)
                        {
                            LogManager.WriteLog(TraceType.ERROR, e.Message, "PIS.Ground.RealTime.RealTimeService.Initialize", e, EventIdEnum.RealTime);
                        }

                        _initialized = true;
                    }
                }
            }
        }
		
		/// <summary>Initializes this object.</summary>
		/// <param name="train2groundClient">The T2G client to use local data store.</param>
		/// <param name="sessionManager">Manager for session.</param>
		/// <param name="requestProcessor">The request processor instance.</param>
		/// <param name="remoteDataStoreFactory">The remote data store factory.</param>
		/// <param name="rtpisDataStore">The rtpis data store.</param>
		/// <param name="plateformType">Type of the plateform.</param>
		public static void Initialize(
            IT2GManager t2gManager,
			ISessionManagerExtended sessionManager,
            INotificationSender notificationSender,
			IRequestProcessor requestProcessor,
			IRemoteDataStoreFactory remoteDataStoreFactory,
			IRTPISDataStore rtpisDataStore)
		{
			try
			{
                if (t2gManager != null)
				{
                    RealTimeService._t2gManager = t2gManager;
				}

				if (sessionManager != null)
				{
					RealTimeService._sessionManager = sessionManager;
				}

                if (notificationSender != null)
                {
                    RealTimeService._notificationSender = notificationSender;
                } 

                if (remoteDataStoreFactory != null)
				{
					RealTimeService._remoteDataStoreFactory = remoteDataStoreFactory;
				}

				if (rtpisDataStore != null)
				{
					RealTimeService._rtpisDataStore = rtpisDataStore;
				}

				if (RealTimeService._requestProcessor == null)
				{
					RealTimeService._requestProcessor = new RequestProcessor(_t2gManager, _rtpisDataStore);
				}

                _initialized = true;
			}
			catch (System.Exception e)
			{
				LogManager.WriteLog(TraceType.ERROR, e.Message, "PIS.Ground.RealTime.RealTimeService.Initialize", e, EventIdEnum.RealTime);
			}
		}

		#endregion

		#region events

		#endregion

		#region properties

		#endregion

		#region methods 

		#region private

		#region static

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
				LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.RealTime.RealTimeService.FindLmtDatabaseFilePath", ex, EventIdEnum.RealTime);
			}

			return lmtDatabaseFilePath;
		}

		/// <summary>Gets station list from lmt data base.</summary>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="elementData">Information describing the element.</param>
		/// <param name="result">[in,out] The result.</param>
		public static void GetStationListFromLMTDataBase(string missionCode, string elementId, AvailableElementData elementData, ref RealTimeRetrieveStationListResult result)
		{
			if (result == null)
			{
				result = new RealTimeRetrieveStationListResult();
				result.ResultCode = RealTimeServiceErrorEnum.RequestAccepted;
			}

			if (elementData.PisBaselineData != null)
			{
				if (!string.IsNullOrEmpty(elementData.PisBaselineData.CurrentVersionLmtOut))
				{
					string lmtVersion = elementData.PisBaselineData.CurrentVersionLmtOut;

					using (var remoteDataStore = _remoteDataStoreFactory.GetRemoteDataStoreInstance() as RemoteDataStoreProxy)
					{
						if (remoteDataStore != null)
						{
							if (remoteDataStore.checkIfDataPackageExists("LMT", lmtVersion))
							{
								var openPackageResult = remoteDataStore.openLocalDataPackage(
									"LMT",
									lmtVersion,
									string.Empty);

								if (openPackageResult.Status == OpenDataPackageStatusEnum.COMPLETED)
								{
									string lmtDatabaseFilePath = FindLmtDatabaseFilePath(openPackageResult.LocalPackagePath);
									if (!string.IsNullOrEmpty(lmtDatabaseFilePath))
									{
										using (var lmtDatabaseAccessor = new LmtDatabaseAccessor(lmtDatabaseFilePath, _platformType))
										{
											result.ResultCode = RealTimeServiceErrorEnum.RequestAccepted;
											result.StationList = new List<string>();

											if (string.IsNullOrEmpty(missionCode))
											{
												foreach (var station in lmtDatabaseAccessor.GetStationList())
												{
													if (!result.StationList.Contains(station.OperatorCode))
													{
														result.StationList.Add(station.OperatorCode);
													}
												}
											}
											else
											{
												uint? missionId = lmtDatabaseAccessor.GetMissionInternalCodeFromOperatorCode(missionCode);
												if (missionId != null)
												{
													List<uint> missionRoute = lmtDatabaseAccessor.GetMissionRoute((uint)missionId);
													foreach (uint stationId in missionRoute)
													{
														result.StationList.Add(lmtDatabaseAccessor.GetStationOperatorCodeFromInternalCode(stationId));
													}
												}
												else
												{
													result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidMissionCode;
												}
											}

                                            if (result.ResultCode == RealTimeServiceErrorEnum.RequestAccepted &&
                                                result.StationList.Count == 0)
											{
												result.ResultCode = RealTimeServiceErrorEnum.InfoNoDataForElement;
											}
										}
									}
									else
									{
										result.ResultCode = RealTimeServiceErrorEnum.ErrorRemoteDatastoreUnavailable;
										LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Logs.ERROR_RETRIEVESTATIONLIST_LMT_DB_NOT_FOUND, lmtVersion, elementId), "PIS.Ground.RealTime.RealTimeService.RetrieveStationList", null, EventIdEnum.RealTime);
									}
								}
								else
								{
									result.ResultCode = RealTimeServiceErrorEnum.ErrorRemoteDatastoreUnavailable;
									LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Logs.ERROR_RETRIEVESTATIONLIST_CANT_OPEN_PACKAGE, lmtVersion, elementId), "PIS.Ground.RealTime.RealTimeService.RetrieveStationList", null, EventIdEnum.RealTime);
								}
							}
							else
							{
								result.ResultCode = RealTimeServiceErrorEnum.ErrorRemoteDatastoreUnavailable;
								LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Logs.ERROR_RETRIEVESTATIONLIST_UNKNOWN_EMBEDDED_LMT, lmtVersion), "PIS.Ground.RealTime.RealTimeService.RetrieveStationList", null, EventIdEnum.RealTime);
							}
						}
						else
						{
							result.ResultCode = RealTimeServiceErrorEnum.ErrorRemoteDatastoreUnavailable;
							LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Logs.ERROR_UNKNOWN), "PIS.Ground.RealTime.RealTimeService.RetrieveStationList", null, EventIdEnum.RealTime);
						}
					}
				}
				else
				{
					result.ResultCode = RealTimeServiceErrorEnum.InfoNoDataForElement;
				}
			}
		}

		#endregion

		#endregion

		#region public

		#region IRealTimeService Members
		
		/// <summary>
		/// This function allows the GroundApp to request from the ground PIS the list of available
		/// elements. This list includes also missions that are running for each element, and the
		/// versions of the LMT and PIS Base data packages.
		/// </summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <returns>
		/// The code “request accepted” when the command is valid and the list of elements, or and error
		/// code when the command is rejected.
		/// </returns>
		RealTimeAvailableElementListResult IRealTimeService.GetAvailableElementList(Guid sessionId)
		{
			var result = new RealTimeAvailableElementListResult();
			result.ResultCode = RealTimeServiceErrorEnum.ElementListNotAvailable;
			result.ElementList = null;

			if (_sessionManager.IsSessionValid(sessionId))
			{
				T2GManagerErrorEnum rqstResult = _t2gManager.GetAvailableElementDataList(out result.ElementList);

				if (rqstResult == T2GManagerErrorEnum.eSuccess)
				{
					result.ResultCode = RealTimeServiceErrorEnum.RequestAccepted;
				}
				else
				{
					result.ResultCode = RealTimeServiceErrorEnum.ElementListNotAvailable;
				}
			}
			else
			{
				result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// The function commands the retreival of the station list from an addressee. If a mission code
		/// is provided, only the stations of that mission are returned. By default, all the station list
		/// from the database are returned.
		/// </summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <returns>
		/// The code “request accepted” when the command is valid and the list of stations for the given
		/// mission code/element id, or and error code when the command is rejected.
		/// </returns>
		RealTimeRetrieveStationListResult IRealTimeService.RetrieveStationList(Guid sessionId, string missionCode, string elementId)
		{
			var result = new RealTimeRetrieveStationListResult();
			result.RequestId = Guid.Empty;
			result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidSessionId;
			result.MissionCode = missionCode;
			result.ElementID = elementId;
			result.StationList = null;

			if (_sessionManager.IsSessionValid(sessionId))
			{
				string error = _sessionManager.GenerateRequestID(sessionId, out result.RequestId);

				if (string.IsNullOrEmpty(error))
				{
					AvailableElementData elementData;
                    T2GManagerErrorEnum rqstResult = _t2gManager.GetAvailableElementDataByElementNumber(elementId, out elementData);

					switch (rqstResult)
					{
						case T2GManagerErrorEnum.eSuccess:
							{
								try
								{
									GetStationListFromLMTDataBase(missionCode, elementId, elementData, ref result);
								}
								catch (TimeoutException)
								{
									result.ResultCode = RealTimeServiceErrorEnum.ErrorRemoteDatastoreUnavailable;
								}
								catch (CommunicationException)
								{
									result.ResultCode = RealTimeServiceErrorEnum.ErrorRemoteDatastoreUnavailable;
								}
							}

							break;
						case T2GManagerErrorEnum.eT2GServerOffline:
							result.ResultCode = RealTimeServiceErrorEnum.T2GServerOffline;
							break;
						case T2GManagerErrorEnum.eElementNotFound:
						default:
							result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidElementId;
							break;
					}
				}
				else
				{
					result.ResultCode = RealTimeServiceErrorEnum.ErrorRequestIdGeneration;
				}
			}
			else
			{
				result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// This function allows the GroundApp to obtain Mission specific Real Time information.
		/// </summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="missionCode">The mission code.</param>
		/// <returns>
		/// The code “request accepted” when the command is valid and the list of informations for the
		/// given mission code, or and error code when the command is rejected.
		/// </returns>
		RealTimeGetMissionRealTimeInformationResult IRealTimeService.GetMissionRealTimeInformation(Guid sessionId, string missionCode)
		{
			var result = new RealTimeGetMissionRealTimeInformationResult();
			result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidSessionId;
			result.RequestId = Guid.Empty;
			result.MissionCode = missionCode;
			result.InformationStructure = null;

			if (_sessionManager.IsSessionValid(sessionId))
			{
				if (!string.IsNullOrEmpty(missionCode))
				{
					string error = _sessionManager.GenerateRequestID(sessionId, out result.RequestId);

					if (string.IsNullOrEmpty(error))
					{
							result.InformationStructure = RealTimeService._rtpisDataStore.GetMissionRealTimeInformation(result.MissionCode);

							if (result.InformationStructure == null)
							{
                                result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidMissionCode;
                            }
                            else if (result.InformationStructure.MissionDelay == null &&
                                result.InformationStructure.MissionWeather == null)
                            {
                                result.ResultCode = RealTimeServiceErrorEnum.ErrorNoRtpisData;
							}
							else if (result.InformationStructure.MissionDelay == null)
							{
								result.ResultCode = RealTimeServiceErrorEnum.InfoNoDelayData;
							}
							else if (result.InformationStructure.MissionWeather == null)
							{
								result.ResultCode = RealTimeServiceErrorEnum.InfoNoWeatherData;
							}
							else
							{
								result.ResultCode = RealTimeServiceErrorEnum.RequestAccepted;
							}
					}
					else
					{
						result.ResultCode = RealTimeServiceErrorEnum.ErrorRequestIdGeneration;
					}
				}
				else
				{
					result.RequestId = Guid.Empty;
					result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidMissionCode;
				}
			}
			else
			{
				result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// This function allows the GroundApp to set the Mission Real Time information for a mission.
		/// </summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="missionDelay">The mission delay.</param>
		/// <param name="missionWeather">The mission weather.</param>
		/// <returns>Error code according to success or not of the request.</returns>
		RealTimeSetMissionRealTimeInformationResult IRealTimeService.SetMissionRealTimeInformation(Guid sessionId, string missionCode, RealTimeDelayType missionDelay, RealTimeWeatherType missionWeather)
		{
			var result = new RealTimeSetMissionRealTimeInformationResult();
			result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidSessionId;
			result.RequestId = Guid.Empty;

			if (_sessionManager.IsSessionValid(sessionId))
			{
				if (!string.IsNullOrEmpty(missionCode))
				{
					string error = _sessionManager.GenerateRequestID(sessionId, out result.RequestId);

					if (string.IsNullOrEmpty(error))
					{
							if (missionDelay == null && missionWeather == null)
							{
								result.ResultCode = RealTimeServiceErrorEnum.ErrorNoRtpisData;
							}
							else
							{
								RealTimeService._rtpisDataStore.SetMissionRealTimeInformation(missionCode, missionDelay, missionWeather);

								if (missionDelay == null)
								{
									result.ResultCode = RealTimeServiceErrorEnum.InfoNoDelayData;
								}
								else if (missionWeather == null)
								{
									result.ResultCode = RealTimeServiceErrorEnum.InfoNoWeatherData;
								}
								else
								{
									result.ResultCode = RealTimeServiceErrorEnum.RequestAccepted;
								}
							}
					}
					else
					{
						result.ResultCode = RealTimeServiceErrorEnum.ErrorRequestIdGeneration;
					}
				}
				else
				{
					result.RequestId = Guid.Empty;
					result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidMissionCode;
				}
			}
			else
			{
				result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// This function allows the GroundApp to obtain the Station Real Time information associated to
		/// a particular list of station.
		/// </summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="stationList">List of stations.</param>
		/// <returns>
		/// The code “request accepted” when the command is valid and the list of informations for the
		/// given stations codes, or and error code when the command is rejected.
		/// </returns>
		RealTimeGetStationRealTimeInformationResult IRealTimeService.GetStationRealTimeInformation(Guid sessionId, string missionCode, List<string> stationList)
		{
			var result = new RealTimeGetStationRealTimeInformationResult();
			result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidSessionId;
			result.RequestId = Guid.Empty;
			result.StationStatusList = null;

			if (_sessionManager.IsSessionValid(sessionId))
			{
				if (!string.IsNullOrEmpty(missionCode))
				{
					string error = _sessionManager.GenerateRequestID(sessionId, out result.RequestId);

					if (string.IsNullOrEmpty(error))
					{
							result.StationStatusList = RealTimeService._rtpisDataStore.GetStationRealTimeInformation(missionCode, stationList);

							if (result.StationStatusList != null && result.StationStatusList.Count > 0)
							{
								result.ResultCode = RealTimeServiceErrorEnum.RequestAccepted;
							}
							else
							{
								result.RequestId = Guid.Empty;
								result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidMissionCode;
							}
					}
					else
					{
						result.ResultCode = RealTimeServiceErrorEnum.ErrorRequestIdGeneration;
					}
				}
				else
				{
					result.RequestId = Guid.Empty;
					result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidMissionCode;
				}
			}
			else
			{
				result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// This function allows the GroundApp to set Real Time information for a list of stations of a
		/// specific mission.
		/// </summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="stationInformationList">List of station informations.</param>
		/// <returns>
		/// Error code according to success or not of the request with list of specific error code for
		/// each station.
		/// </returns>
		RealTimeSetStationRealTimeInformationResult IRealTimeService.SetStationRealTimeInformation(Guid sessionId, string missionCode, List<RealTimeStationInformationType> stationInformationList)
		{
			var result = new RealTimeSetStationRealTimeInformationResult();
			result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidSessionId;
			result.RequestId = Guid.Empty;
			result.MissionCode = missionCode;
			result.StationResultList = null;

			if (_sessionManager.IsSessionValid(sessionId))
			{
				if (!string.IsNullOrEmpty(result.MissionCode))
				{
					string error = _sessionManager.GenerateRequestID(sessionId, out result.RequestId);

					if (string.IsNullOrEmpty(error))
					{
							if (stationInformationList != null && stationInformationList.Count() != 0)
							{
								if (stationInformationList.Count <= 60)
								{
									List<RealTimeStationStatusType> stationStatusList = RealTimeService._rtpisDataStore.GetStationRealTimeInformation(result.MissionCode, null);
									List<string> stationList = new List<string>();

									if (stationStatusList != null)
									{
										stationList = stationStatusList.Select(item => item.StationID).ToList();
									}

									stationList.AddRange(stationInformationList.Select(item => item.StationCode).ToList());

									if (stationList.Distinct().Count() <= 60)
									{
										result.ResultCode = RealTimeServiceErrorEnum.RequestAccepted;
										RealTimeService._rtpisDataStore.SetStationRealTimeInformation(result.MissionCode, stationInformationList, out result.StationResultList);
									}
									else
									{
										result.RequestId = Guid.Empty;
										result.ResultCode = RealTimeServiceErrorEnum.ErrorStationListLimitExcedeed;
									}
								}
								else
								{
									result.RequestId = Guid.Empty;
									result.ResultCode = RealTimeServiceErrorEnum.ErrorStationListLimitExcedeed;
								}
							}
							else
							{
								result.RequestId = Guid.Empty;
								result.ResultCode = RealTimeServiceErrorEnum.ErrorNoRtpisData;
							}
					}
					else
					{
						result.RequestId = Guid.Empty;
						result.ResultCode = RealTimeServiceErrorEnum.ErrorRequestIdGeneration;
					}
				}
				else
				{
					result.RequestId = Guid.Empty;
					result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidMissionCode;
				}
			}
			else
			{
				result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// This function allows the GroundApp to clear the Real Time information associated to a
		/// particular mission and list of station.
		/// </summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="stationList">List of stations.</param>
		/// <returns>
		/// The code “request accepted” when the command is valid and the list of informations for the
		/// given stations codes, or and error code when the command is rejected.
		/// </returns>
		RealTimeClearRealTimeInformationResult IRealTimeService.ClearRealTimeInformation(Guid sessionId, string missionCode, List<string> stationList)
		{
			var result = new RealTimeClearRealTimeInformationResult();
			result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidSessionId;
			result.RequestId = Guid.Empty;
			result.MissionCode = missionCode;
			result.StationList = null;

			if (_sessionManager.IsSessionValid(sessionId))
			{
				if (!string.IsNullOrEmpty(missionCode))
				{
					string error = _sessionManager.GenerateRequestID(sessionId, out result.RequestId);

					if (string.IsNullOrEmpty(error))
					{
						
							var missionData = RealTimeService._rtpisDataStore.GetMissionRealTimeInformation(result.MissionCode);
							var stationListData = RealTimeService._rtpisDataStore.GetStationRealTimeInformation(result.MissionCode, stationList);

                            if (missionData == null)
                            {
                                result.RequestId = Guid.Empty;
                                result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidMissionCode;
                            }
                            else if ( missionData.MissionDelay != null ||
                                      missionData.MissionWeather != null ||
                                      (stationListData != null && stationListData.Count() > 0) )
                            {
                                result.ResultCode = RealTimeServiceErrorEnum.RequestAccepted;
                                result.StationList = new List<string>();
                                RealTimeService._rtpisDataStore.ClearRealTimeInformation(result.MissionCode, stationList, out result.StationList);
                            }                            
                            else
                            {
                                result.RequestId = Guid.Empty;
                                result.ResultCode = RealTimeServiceErrorEnum.ErrorNoRtpisData;
                            }
					}
					else
					{
						result.RequestId = Guid.Empty;
						result.ResultCode = RealTimeServiceErrorEnum.ErrorRequestIdGeneration;
					}
				}
				else
				{
					result.RequestId = Guid.Empty;
					result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidMissionCode;
				}
			}
			else
			{
				result.ResultCode = RealTimeServiceErrorEnum.ErrorInvalidSessionId;
			}

			return result;
		}

		#endregion

		#endregion

		#endregion
	}
}