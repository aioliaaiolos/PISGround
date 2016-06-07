//---------------------------------------------------------------------------------------------------
// <copyright file="RequestProcessor.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using System.Configuration;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.Common;
using System.ServiceModel;

namespace PIS.Ground.RealTime
{
	/// <summary>Request processor.</summary>
	public class RequestProcessor : IRequestProcessor
	{
		#region const

		/// <summary>Identifier for event subscription.</summary>
		private const string SubscriberId = "PIS.Ground.RealTime.RequestProcessor";

		#endregion

		#region fields

		/// <summary>The T2G client to use local data store.</summary>
		private static IT2GManager _t2gManager = null;

		/// <summary>The rtpis data store.</summary>
		private static IRTPISDataStore _rtpisDataStore = null;

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the RequestProcessor class.</summary>
		/// <param name="t2gManager">The T2G manager to use local data store.</param>
		/// <param name="rtpisDataStore">The rtpis data store.</param>
		public RequestProcessor(
			IT2GManager t2gManager,
			IRTPISDataStore rtpisDataStore)
		{
			if (t2gManager != null)
			{
				_t2gManager = t2gManager;
			}
			else
			{
				throw new ArgumentNullException("t2gManager");
			}

			// Register a callback that will start streaming on new trains
			_t2gManager.SubscribeToElementChangeNotification(
				SubscriberId,
				new EventHandler<ElementEventArgs>(OnElementInfoChanged));

			if (rtpisDataStore != null)
			{
				_rtpisDataStore = rtpisDataStore;
			}
			else
			{
				_rtpisDataStore = new RTPISDataStore();
			}

			// Register a callback that will start streaming on new trains
			_rtpisDataStore.Changed += new ChangedEventHandler(RTPISDataStoreChanged);
		}

		#endregion

		#region events

		/// <summary>
		/// Callback called when Element Online state changes (signaled by the T2G Client).
		/// </summary>
		/// <param name="sender">Source of the event.</param>
		/// <param name="args">Event information to send to registered event handlers.</param>
		private static void OnElementInfoChanged(object sender, ElementEventArgs args)
		{
			if (args != null &&
				args.SystemInformation != null &&
				args.SystemInformation.IsOnline &&
				args.SystemInformation.PisMission != null &&
				args.SystemInformation.PisMission.MissionState == MissionStateEnum.MI)
			{
				ProcessMissionDataSending(
					args.SystemInformation.SystemId,
					args.SystemInformation.PisMission.OperatorCode);

				ProcessStationDataSending(
					args.SystemInformation.SystemId,
					new KeyValuePair<string, List<string>>(args.SystemInformation.PisMission.OperatorCode, null));
			}
		}

		/// <summary>Rtpis data store changed.</summary>
		/// <param name="sender">Source of the event.</param>
		/// <param name="e">Rtpis data store event information.</param>
		private static void RTPISDataStoreChanged(object sender, RTPISDataStoreEventArgs e)
		{
			ElementList<AvailableElementData> elementList = null;

			if (e != null)
			{
				if (!string.IsNullOrEmpty(e.MissionCode))
				{
					T2GManagerErrorEnum rqstResult = _t2gManager.GetAvailableElementDataListByMissionCode(e.MissionCode, out elementList);

					if (rqstResult == T2GManagerErrorEnum.eSuccess)
					{
						if (elementList != null)
						{
							foreach (var element in elementList)
							{
								ProcessMissionDataSending(element.ElementNumber, e.MissionCode);
							}
						}
					}
					else
					{
						LogManager.WriteLog(TraceType.INFO, string.Format(Logs.INFO_NO_ELEMENTS_FOR_MISSION, e.MissionCode), "PIS.Ground.RealTime.RealTimeService.RTPISDataStoreChanged", null, EventIdEnum.RealTime);
					}
				}

				if (e.StationCodeList != null
					&& !string.IsNullOrEmpty(((KeyValuePair<string, List<string>>)e.StationCodeList).Key)
					&& ((KeyValuePair<string, List<string>>)e.StationCodeList).Value.Count > 0)
				{
					if (elementList == null)
					{
						T2GManagerErrorEnum rqstResult = _t2gManager.GetAvailableElementDataListByMissionCode(((KeyValuePair<string, List<string>>)e.StationCodeList).Key, out elementList);
						if (rqstResult != T2GManagerErrorEnum.eSuccess)
						{
							LogManager.WriteLog(TraceType.INFO, string.Format(Logs.INFO_NO_ELEMENTS_FOR_MISSION, e.MissionCode), "PIS.Ground.RealTime.RealTimeService.RTPISDataStoreChanged", null, EventIdEnum.RealTime);
						}
					}

					if (elementList != null)
					{
						foreach (var element in elementList)
						{
							ProcessStationDataSending(element.ElementNumber, (KeyValuePair<string, List<string>>)e.StationCodeList);
						}
					}
				}
			}
		}

		/// <summary>Process the mission data sending.</summary>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="missionCode">The mission code.</param>
		private static void ProcessMissionDataSending(string elementId, string missionCode)
		{
			ServiceInfo serviceInfo = null;
			T2GManagerErrorEnum t2gResult = _t2gManager.GetAvailableServiceData(elementId, (int)eServiceID.eSrvSIF_RealTimeServer, out serviceInfo);

			if (t2gResult == T2GManagerErrorEnum.eSuccess)
			{
				string endpoint = "http://" + serviceInfo.ServiceIPAddress + ":" + serviceInfo.ServicePortNumber;
				try
				{
					// Call RealTime train service and send the request.
					using (Train.RealTime.RealTimeTrainServiceClient lTrainClient = new Train.RealTime.RealTimeTrainServiceClient("RealTimeTrainEndpoint", endpoint))
					{
						try
						{
							Train.RealTime.DelayType rtinfoDelay = null;
							Train.RealTime.WeatherType rtinfoWeather = null;

							RealTimeUtils.ConvertGroundMissionDataToTrainMissionData(
								_rtpisDataStore.GetMissionRealTimeInformation(missionCode),
								out rtinfoDelay,
								out rtinfoWeather);

							Train.RealTime.SetMissionRealTimeRequest request = new Train.RealTime.SetMissionRealTimeRequest(
									missionCode,
									rtinfoDelay != null ? Train.RealTime.ActionTypeEnum.Update : Train.RealTime.ActionTypeEnum.Delete,
									rtinfoDelay,
									rtinfoWeather != null ? Train.RealTime.ActionTypeEnum.Update : Train.RealTime.ActionTypeEnum.Delete,
									rtinfoWeather);
							Train.RealTime.SetMissionRealTimeResponse response = ((Train.RealTime.IRealTimeTrainService)lTrainClient).SetMissionRealTime(request);

							ProcessCommandResultList(elementId, response.ResultList);
						}
						catch (Exception ex)
						{
							LogManager.WriteLog(TraceType.EXCEPTION, string.Format(Logs.ERROR_FAILED_SEND_REQUEST_TO_EMBEDDED, elementId), "PIS.Ground.RealTime.RealTimeService.ProcessMissionDataSending", ex, EventIdEnum.RealTime);
						}
						finally
						{
							if (lTrainClient.State == CommunicationState.Faulted)
							{
								lTrainClient.Abort();
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(TraceType.EXCEPTION, string.Format(Logs.ERROR_FAILED_SEND_REQUEST_TO_EMBEDDED, elementId), "PIS.Ground.RealTime.RealTimeService.ProcessMissionDataSending", ex, EventIdEnum.RealTime);
				}
			}
			else
			{
				LogManager.WriteLog(TraceType.ERROR, string.Format(Logs.ERROR_GET_SERVICE_DATA_FOR_ELEMENT, eServiceID.eSrvSIF_RealTimeServer, elementId, t2gResult), "PIS.Ground.RealTime.RealTimeService.ProcessMissionDataSending", null, EventIdEnum.RealTime);
			}
		}

		/// <summary>Process the station data sending.</summary>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="stationCodeList">List of station codes.</param>
		private static void ProcessStationDataSending(string elementId, KeyValuePair<string, List<string>> stationCodeList)
		{
			List<RealTimeStationStatusType> stationDataList = _rtpisDataStore.GetStationRealTimeInformation(stationCodeList.Key, stationCodeList.Value);

			if (stationDataList == null)
			{
				stationDataList = new List<RealTimeStationStatusType>();
			}

			if (stationDataList.Count == 0)
			{
				RealTimeRetrieveStationListResult result = new RealTimeRetrieveStationListResult();
				AvailableElementData elementData = null;

				T2GManagerErrorEnum t2gTmpResult = _t2gManager.GetAvailableElementDataByElementNumber(elementId, out elementData);

				if (t2gTmpResult == T2GManagerErrorEnum.eSuccess)
				{
					RealTimeService.GetStationListFromLMTDataBase(stationCodeList.Key, elementId, elementData, ref result);

					if (result.ResultCode == RealTimeServiceErrorEnum.RequestAccepted)
					{
						foreach (var station in result.StationList)
						{
							stationDataList.Add(new RealTimeStationStatusType() { StationID = station });
						}
					}
					else
					{
						LogManager.WriteLog(TraceType.ERROR, string.Format(Logs.ERROR_ACCESSING_STATIONLIST_FOR_ELEMENT, elementId), "PIS.Ground.RealTime.RealTimeService.ProcessStationDataSending", null, EventIdEnum.RealTime);
					}
				}
				else
				{
					LogManager.WriteLog(TraceType.ERROR, string.Format(Logs.ERROR_ACCESSING_STATIONLIST_FOR_ELEMENT, elementId), "PIS.Ground.RealTime.RealTimeService.ProcessStationDataSending", null, EventIdEnum.RealTime);
				}
			}


			ServiceInfo serviceInfo = null;
			T2GManagerErrorEnum t2gResult = _t2gManager.GetAvailableServiceData(elementId, (int)eServiceID.eSrvSIF_RealTimeServer, out serviceInfo);

			if (t2gResult == T2GManagerErrorEnum.eSuccess)
			{
				string endpoint = "http://" + serviceInfo.ServiceIPAddress + ":" + serviceInfo.ServicePortNumber;
				try
				{
					// Call RealTime train service and send the request.
					using (Train.RealTime.RealTimeTrainServiceClient lTrainClient = new Train.RealTime.RealTimeTrainServiceClient("RealTimeTrainEndpoint", endpoint))
					{
						try
						{
							Train.RealTime.ListOfStationDataType stationDataListType = null;

							RealTimeUtils.ConvertGroundStationDataToTrainStationData(
								stationDataList,
								out stationDataListType);

							Train.RealTime.SetStationRealTimeRequest request = new Train.RealTime.SetStationRealTimeRequest(stationCodeList.Key, stationDataListType);
							Train.RealTime.SetStationRealTimeResponse response = ((Train.RealTime.IRealTimeTrainService)lTrainClient).SetStationRealTime(request);

							ProcessCommandResultList(elementId, response.ResultList);
						}
						catch (Exception ex)
						{
							LogManager.WriteLog(TraceType.EXCEPTION, string.Format(Logs.ERROR_FAILED_SEND_REQUEST_TO_EMBEDDED, elementId), "PIS.Ground.RealTime.RealTimeService.ProcessStationDataSending", ex, EventIdEnum.RealTime);
						}
						finally
						{
							if (lTrainClient.State == CommunicationState.Faulted)
							{
								lTrainClient.Abort();
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(TraceType.EXCEPTION, string.Format(Logs.ERROR_FAILED_SEND_REQUEST_TO_EMBEDDED, elementId), "PIS.Ground.RealTime.RealTimeService.ProcessStationDataSending", ex, EventIdEnum.RealTime);
				}
			}
			else
			{
				LogManager.WriteLog(TraceType.ERROR, string.Format(Logs.ERROR_GET_SERVICE_DATA_FOR_ELEMENT, eServiceID.eSrvSIF_RealTimeServer, elementId, t2gResult), "PIS.Ground.RealTime.RealTimeService.ProcessStationDataSending", null, EventIdEnum.RealTime);
			}
		}

		/// <summary>Process the command result list.</summary>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="listOfResultType">Type of the list of result.</param>
		private static void ProcessCommandResultList(string elementId, PIS.Train.RealTime.ListOfResultType listOfResultType)
		{
			if (listOfResultType != null)
			{
				foreach (var resultType in listOfResultType)
				{
					string logStr = string.Empty;
					TraceType logLevel = TraceType.EXCEPTION;

					switch (resultType.ResultCode)
					{
						case PIS.Train.RealTime.ResultCodeEnum.OK:
							logLevel = TraceType.INFO;
							break;
						case PIS.Train.RealTime.ResultCodeEnum.NotCurrentMission:
						case PIS.Train.RealTime.ResultCodeEnum.StationNotInTrainRoute:
							logLevel = TraceType.WARNING;
							break;
						case PIS.Train.RealTime.ResultCodeEnum.ComplexError:
						default:
							logLevel = TraceType.ERROR;
							break;
					}

					logStr += string.Format(
						Logs.RESULT_PROCESS_CMD,
						elementId,
						resultType.InformationType.ToString(),
						resultType.MissionCode,
						resultType.StationCode);

					foreach (var complexError in resultType.ComplexErrorList)
					{
						foreach (var parameterError in complexError.ParameterErrorList)
						{
							logStr += string.Format(
								Logs.RESULT_PROCESS_PARAMETER,
								parameterError.ParameterName,
								parameterError.ErrorMessage,
								parameterError.ErroneousValue);
						}
					}

					LogManager.WriteLog(logLevel, logStr, "PIS.Ground.RealTime.RequestProcessor.ProcessCommandResultList", null, EventIdEnum.RealTime);
				}
			}
		}

		#endregion

		#region properties

		#endregion

		#region methods

		#endregion
	}
}