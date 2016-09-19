//---------------------------------------------------------------------------------------------------
// <copyright file="RequestProcessor.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.T2G;

namespace PIS.Ground.RealTime
{
	/// <summary>Request processor.</summary>
	public class RequestProcessor : IRequestProcessor, IDisposable
	{
		#region const

		/// <summary>Identifier for event subscription.</summary>
		public const string SubscriberId = "PIS.Ground.RealTime.RequestProcessor";

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

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_rtpisDataStore != null)
                {
                    _rtpisDataStore.Changed -= new ChangedEventHandler(RTPISDataStoreChanged);
                }

                if (_t2gManager != null)
                {
                    _t2gManager.UnsubscribeFromElementChangeNotification(SubscriberId);
                }
            }
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
				args.SystemInformation.PisMission.MissionState == MissionStateEnum.MI &&
                args.SystemInformation.ServiceList != null &&
                args.SystemInformation.ServiceList.Any(s => s.ServiceId == (ushort)eServiceID.eSrvSIF_RealTimeServer && s.IsAvailable == true))
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
                // Skip the processing if PIS-Ground does not communicate with T2G.
                if (_t2gManager.T2GServerConnectionStatus)
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
                            if (LogManager.IsTraceActive(TraceType.INFO))
                            {
                                LogManager.WriteLog(TraceType.INFO, string.Format(CultureInfo.CurrentCulture, Logs.INFO_NO_ELEMENTS_FOR_MISSION, e.MissionCode), "PIS.Ground.RealTime.RealTimeService.RTPISDataStoreChanged", null, EventIdEnum.RealTime);
                            }
                        }
                    }

                    string missionCode;
                    if (e.StationCodeList.HasValue
                        && !string.IsNullOrEmpty((missionCode = e.StationCodeList.Value.Key))
                        && e.StationCodeList.Value.Value.Count > 0)
                    {
                        if (elementList == null)
                        {
                            T2GManagerErrorEnum rqstResult = _t2gManager.GetAvailableElementDataListByMissionCode(missionCode, out elementList);
                            if (rqstResult != T2GManagerErrorEnum.eSuccess && LogManager.IsTraceActive(TraceType.INFO))
                            {
                                LogManager.WriteLog(TraceType.INFO, string.Format(CultureInfo.CurrentCulture, Logs.INFO_NO_ELEMENTS_FOR_MISSION, missionCode), "PIS.Ground.RealTime.RealTimeService.RTPISDataStoreChanged", null, EventIdEnum.RealTime);
                            }
                        }

                        if (elementList != null)
                        {
                            foreach (var element in elementList)
                            {
                                ProcessStationDataSending(element.ElementNumber, e.StationCodeList.Value);
                            }
                        }
                    }
                }
                else if (LogManager.IsTraceActive(TraceType.DEBUG))
                {
                    LogManager.WriteLog(TraceType.DEBUG, Logs.SKIP_NOTIFICATION_RTPISDATASTORECHANGED_T2G_OFFLINE, "PIS.Ground.RealTime.RealTimeService.RTPISDataStoreChanged", null, EventIdEnum.RealTime);
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
									rtinfoDelay != null ? Train.RealTime.ActionTypeEnum.Set : Train.RealTime.ActionTypeEnum.Delete,
									rtinfoDelay,
									rtinfoWeather != null ? Train.RealTime.ActionTypeEnum.Set : Train.RealTime.ActionTypeEnum.Delete,
									rtinfoWeather);
							Train.RealTime.SetMissionRealTimeResponse response = ((Train.RealTime.IRealTimeTrainService)lTrainClient).SetMissionRealTime(request);

							ProcessCommandResultList(elementId, missionCode, response.ResultList);
						}
						catch (Exception ex)
						{
							LogManager.WriteLog(TraceType.EXCEPTION, string.Format(CultureInfo.CurrentCulture, Logs.ERROR_FAILED_SEND_REQUEST_TO_EMBEDDED, elementId), "PIS.Ground.RealTime.RealTimeService.ProcessMissionDataSending", ex, EventIdEnum.RealTime);
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
					LogManager.WriteLog(TraceType.EXCEPTION, string.Format(CultureInfo.CurrentCulture, Logs.ERROR_FAILED_SEND_REQUEST_TO_EMBEDDED, elementId), "PIS.Ground.RealTime.RealTimeService.ProcessMissionDataSending", ex, EventIdEnum.RealTime);
				}
			}
			else if (t2gResult == T2GManagerErrorEnum.eElementNotFound)
            {
                LogManager.WriteLog(TraceType.WARNING, string.Format(CultureInfo.CurrentCulture, Logs.ERROR_GET_SERVICE_DATA_FOR_ELEMENT, elementId, t2gResult), "PIS.Ground.RealTime.RealTimeService.ProcessMissionDataSending", null, EventIdEnum.RealTime);
            }
            else if (LogManager.IsTraceActive(TraceType.DEBUG))
            {
                LogManager.WriteLog(TraceType.DEBUG, string.Format(CultureInfo.CurrentCulture, Logs.DEBUG_GET_SERVICE_DATA_FOR_ELEMENT, elementId, t2gResult), "PIS.Ground.RealTime.RealTimeService.ProcessMissionDataSending", null, EventIdEnum.RealTime);
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
                        stationDataList.Capacity = result.StationList.Count;
                        foreach (var station in result.StationList)
                        {
                            stationDataList.Add(new RealTimeStationStatusType() { StationID = station });
                        }
                    }
                    else
                    {
                        LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Logs.ERROR_ACCESSING_STATIONLIST_FOR_ELEMENT, elementId, "GetStationListFromLMTDataBase", result.ResultCode), "PIS.Ground.RealTime.RealTimeService.ProcessStationDataSending", null, EventIdEnum.RealTime);
                    }
                }
                else
                {
                    LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Logs.ERROR_ACCESSING_STATIONLIST_FOR_ELEMENT, elementId, "GetAvailableElementDataByElementNumber", t2gTmpResult), "PIS.Ground.RealTime.RealTimeService.ProcessStationDataSending", null, EventIdEnum.RealTime);
                }
            }

            // Updating the train is important only when the data of at least one station need to be updated.
            if (stationDataList.Count != 0)
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
                                Train.RealTime.ListOfStationDataType stationDataListType = null;

                                RealTimeUtils.ConvertGroundStationDataToTrainStationData(
                                    stationDataList,
                                    out stationDataListType);

                                Train.RealTime.SetStationRealTimeRequest request = new Train.RealTime.SetStationRealTimeRequest(stationCodeList.Key, stationDataListType);
                                Train.RealTime.SetStationRealTimeResponse response = ((Train.RealTime.IRealTimeTrainService)lTrainClient).SetStationRealTime(request);

                                ProcessCommandResultList(elementId, stationCodeList.Key, response.ResultList);
                            }
                            catch (Exception ex)
                            {
                                LogManager.WriteLog(TraceType.EXCEPTION, string.Format(CultureInfo.CurrentCulture, Logs.ERROR_FAILED_SEND_REQUEST_TO_EMBEDDED, elementId), "PIS.Ground.RealTime.RealTimeService.ProcessStationDataSending", ex, EventIdEnum.RealTime);
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
                        LogManager.WriteLog(TraceType.EXCEPTION, string.Format(CultureInfo.CurrentCulture, Logs.ERROR_FAILED_SEND_REQUEST_TO_EMBEDDED, elementId), "PIS.Ground.RealTime.RealTimeService.ProcessStationDataSending", ex, EventIdEnum.RealTime);
                    }
                }
                else if (t2gResult == T2GManagerErrorEnum.eElementNotFound)
                {
                    LogManager.WriteLog(TraceType.WARNING, string.Format(CultureInfo.CurrentCulture, Logs.ERROR_GET_SERVICE_DATA_FOR_ELEMENT, elementId, t2gResult), "PIS.Ground.RealTime.RealTimeService.ProcessStationDataSending", null, EventIdEnum.RealTime);
                }
                else if (LogManager.IsTraceActive(TraceType.DEBUG))
                {
                    LogManager.WriteLog(TraceType.DEBUG, string.Format(CultureInfo.CurrentCulture, Logs.DEBUG_GET_SERVICE_DATA_FOR_ELEMENT, elementId, t2gResult), "PIS.Ground.RealTime.RealTimeService.ProcessStationDataSending", null, EventIdEnum.RealTime);
                }
            }
        }

		/// <summary>Process the command result list.</summary>
		/// <param name="elementId">Identifier for the element.</param>
        /// <param name="missionCode">The default mission code.</param>
		/// <param name="listOfResultType">Type of the list of result.</param>
        private static void ProcessCommandResultList(string elementId, string missionCode, PIS.Train.RealTime.ListOfResultType listOfResultType)
        {
            if (listOfResultType != null)
            {
                TraceType logLevel = TraceType.NONE;

                // Determinate the log level.
                foreach (var resultType in listOfResultType)
                {
                    if (resultType == null)
                    {
                        continue;
                    }

                    switch (resultType.ResultCode)
                    {
                        case PIS.Train.RealTime.ResultCodeEnum.OK:
                            if (logLevel == TraceType.NONE)
                            {
                                logLevel = TraceType.INFO;
                            }
                            break;
                        case PIS.Train.RealTime.ResultCodeEnum.NotCurrentMission:
                        case PIS.Train.RealTime.ResultCodeEnum.StationNotInTrainRoute:
                            if (logLevel != TraceType.ERROR)
                            {
                                logLevel = TraceType.WARNING;
                            }
                            break;
                        case PIS.Train.RealTime.ResultCodeEnum.ComplexError:
                        case PIS.Train.RealTime.ResultCodeEnum.InvalidSoapRequest:
                        case PIS.Train.RealTime.ResultCodeEnum.Error:
                        default:
                            logLevel = TraceType.ERROR;
                            break;
                    }

                    if (logLevel == TraceType.ERROR)
                    {
                        break;
                    }
                }

                // Generate the log string
                if (LogManager.IsTraceActive(logLevel))
                {
                    StringBuilder logStr = new StringBuilder(500 + 100 * listOfResultType.Count);

                    logStr.AppendFormat(CultureInfo.CurrentCulture,
                            Logs.RESULT_PROCESS_TITLE,
                            elementId,
                            missionCode);
                    logStr.AppendLine();
                    foreach (var resultType in listOfResultType)
                    {
                        if (resultType == null)
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(resultType.StationCode))
                        {
                            logStr.AppendFormat(CultureInfo.CurrentCulture, Logs.RESULT_PROCESS_STATION_UPDATE, resultType.InformationType, resultType.StationCode, resultType.ResultCode);
                        }
                        else
                        {
                            logStr.AppendFormat(CultureInfo.CurrentCulture, Logs.RESULT_PROCESS_MISSION_UPDATE, resultType.InformationType, resultType.ResultCode);
                        }

                        logStr.AppendLine();

                        if (resultType.ComplexErrorList != null)
                        {
                            foreach (var complexError in resultType.ComplexErrorList)
                            {
                                if (complexError.ParameterErrorList != null)
                                {
                                    foreach (var parameterError in complexError.ParameterErrorList)
                                    {
                                        logStr.AppendFormat(CultureInfo.CurrentCulture,
                                            Logs.RESULT_PROCESS_PARAMETER,
                                            parameterError.ParameterName,
                                            parameterError.ErrorMessage,
                                            parameterError.ErroneousValue);
                                        logStr.AppendLine();
                                    }
                                }
                            }
                        }
                    }

                    LogManager.WriteLog(logLevel, logStr.ToString(), "PIS.Ground.RealTime.RequestProcessor.ProcessCommandResultList", null, EventIdEnum.RealTime);
                }
            }
        }

		#endregion
	}
}