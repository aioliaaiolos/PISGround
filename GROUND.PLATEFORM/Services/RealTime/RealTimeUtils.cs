//---------------------------------------------------------------------------------------------------
// <copyright file="RealTimeUtils.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace PIS.Ground.RealTime
{
    /// <summary>Real time utilities.</summary>
    public class RealTimeUtils
    {
        /// <summary>
        /// Prevents a default instance of the RealTimeUtils class from being created.
        /// </summary>
        private RealTimeUtils()
        {
        }

        /// <summary>Convert ground mission data to train mission data.</summary>
        /// <param name="inputData">Information describing the input.</param>
        /// <param name="outputDelay">[out] The output delay.</param>
        /// <param name="outputWeather">[out] The output weather.</param>
        public static void ConvertGroundMissionDataToTrainMissionData(RealTimeInformationType inputData, out Train.RealTime.DelayType outputDelay, out Train.RealTime.WeatherType outputWeather)
        {
            outputDelay = null;
            outputWeather = null;

            if (inputData != null)
            {
                if (inputData.MissionDelay != null)
                {
                    TimeSpan age = DateTime.Now - inputData.MissionDelay.UpdateDate;
                    outputDelay = new Train.RealTime.DelayType()
                    {
                        Age = (uint)age.TotalSeconds,
                        DelayValue = inputData.MissionDelay.Delay,
                        DelayReason = inputData.MissionDelay.DelayReason,
                        DelayReasonCode = inputData.MissionDelay.DelayReasonCode,
                    };
                }

                if (inputData.MissionWeather != null)
                {
                    TimeSpan age = DateTime.Now - inputData.MissionWeather.UpdateDate;
                    outputWeather = new Train.RealTime.WeatherType()
                    {
                        Age = (uint)age.TotalSeconds,
                        WeatherCondition = inputData.MissionWeather.WeatherCondition,
                        WeatherConditionCode = inputData.MissionWeather.WeatherConditionCode,
                        TemperatureInCentigrade = inputData.MissionWeather.TemperatureInCentigrade,
                        TemperatureInFahrenheit = inputData.MissionWeather.TemperatureInFahrenheit,
                        Humidity = inputData.MissionWeather.Humidity
                    };
                }
            }
        }

        /// <summary>Convert ground station data to train station data.</summary>
        /// <param name="inputDataList">List of input data.</param>
        /// <param name="outputDataList">[out] List of output data.</param>
        public static void ConvertGroundStationDataToTrainStationData(List<RealTimeStationStatusType> inputDataList, out Train.RealTime.ListOfStationDataType outputDataList)
        {
            outputDataList = null;
            if (inputDataList != null)
            {
                outputDataList = new Train.RealTime.ListOfStationDataType();
                foreach (var stationData in inputDataList)
                {
                    if (stationData != null)
                    {
                        Train.RealTime.StationDataType stationDataTrain = new Train.RealTime.StationDataType() { StationCode = stationData.StationID };
                        Train.RealTime.StationDataStructureType stationDataStructureTrain = new Train.RealTime.StationDataStructureType();

                        if (stationData.StationDelay == null)
                        {
                            stationDataStructureTrain.StationDelayAction = Train.RealTime.ActionTypeEnum.Delete;
                            stationDataStructureTrain.StationDelay = null;
                        }
                        else
                        {
                            TimeSpan age = DateTime.Now - stationData.StationDelay.UpdateDate;
                            stationDataStructureTrain.StationDelayAction = Train.RealTime.ActionTypeEnum.Set;
                            stationDataStructureTrain.StationDelay = new Train.RealTime.DelayType()
                            {
                                Age = (uint)age.TotalSeconds,
                                DelayValue = stationData.StationDelay.Delay,
                                DelayReason = stationData.StationDelay.DelayReason,
                                DelayReasonCode = stationData.StationDelay.DelayReasonCode,
                            };
                        }

                        if (stationData.StationWeather == null)
                        {
                            stationDataStructureTrain.StationWeatherAction = Train.RealTime.ActionTypeEnum.Delete;
                            stationDataStructureTrain.StationWeather = null;
                        }
                        else
                        {
                            TimeSpan age = DateTime.Now - stationData.StationWeather.UpdateDate;
                            stationDataStructureTrain.StationWeatherAction = Train.RealTime.ActionTypeEnum.Set;
                            stationDataStructureTrain.StationWeather = new Train.RealTime.WeatherType()
                            {
                                Age = (uint)age.TotalSeconds,
                                WeatherCondition = stationData.StationWeather.WeatherCondition,
                                WeatherConditionCode = stationData.StationWeather.WeatherConditionCode,
                                TemperatureInCentigrade = stationData.StationWeather.TemperatureInCentigrade,
                                TemperatureInFahrenheit = stationData.StationWeather.TemperatureInFahrenheit,
                                Humidity = stationData.StationWeather.Humidity
                            };
                        }

                        if (stationData.StationPlatform == null)
                        {
                            stationDataStructureTrain.StationPlatformAction = Train.RealTime.ActionTypeEnum.Delete;
                            stationDataStructureTrain.StationPlatform = null;
                        }
                        else
                        {
                            TimeSpan age = DateTime.Now - stationData.StationPlatform.UpdateDate;
                            stationDataStructureTrain.StationPlatformAction = Train.RealTime.ActionTypeEnum.Set;
                            stationDataStructureTrain.StationPlatform = new Train.RealTime.PlatformType()
                            {
                                Age = (uint)age.TotalSeconds,
                                Platform = stationData.StationPlatform.Platform,
                                Track = stationData.StationPlatform.Track,
                                TrackCode = stationData.StationPlatform.TrackCode,
                                ExitSide = stationData.StationPlatform.ExitSide,
                                ExitSideCode = stationData.StationPlatform.ExitSideCode,
                                PlatformIssueDescription = stationData.StationPlatform.IssueDescription,
                                PlatformIssueDescriptionCode = stationData.StationPlatform.IssueDescriptionCode ?? default(int),
                            };
                        }

                        if (stationData.StationConnectionList == null)
                        {
                            stationDataStructureTrain.StationConnectionListAction = Train.RealTime.ActionTypeEnum.Delete;
                            stationDataStructureTrain.StationConnectionList = null;
                        }
                        else
                        {
                            stationDataStructureTrain.StationConnectionListAction = Train.RealTime.ActionTypeEnum.Set;
                            stationDataStructureTrain.StationConnectionList = new Train.RealTime.ListOfConnectionEntryType();
                            foreach (var stationConnectionData in stationData.StationConnectionList)
                            {
                                if (stationConnectionData != null)
                                {
                                    TimeSpan age = DateTime.Now - stationConnectionData.UpdateDate;
                                    Train.RealTime.ConnectionType connectionDataTrain = new Train.RealTime.ConnectionType()
                                    {
                                        Age = (uint)age.TotalSeconds,
                                        Operator = stationConnectionData.Operator,
                                        OperatorCode = stationConnectionData.OperatorCode,
                                        CommercialNumber = stationConnectionData.CommercialNumber,
                                        ModelType = stationConnectionData.ModelType,
                                        ModelTypeCode = stationConnectionData.ModelTypeCode,
                                        Line = stationConnectionData.Line,
                                        LineCode = stationConnectionData.LineCode,
                                        DepartureTime = stationConnectionData.DepartureTime,
                                        NextDepartureTime = stationConnectionData.NextDepartureTime,
                                        DepartureFrequency = stationConnectionData.DepartureFrequency,
                                        Platform = stationConnectionData.ConnectionPlatform,
                                        Delay = stationConnectionData.ConnectionDelay,
                                        DestinationLabel = stationConnectionData.DestinationName,
                                    };

                                    stationDataStructureTrain.StationConnectionList.Add(connectionDataTrain);
                                }

                            }
                        }

                        stationDataTrain.StationDataStructure = stationDataStructureTrain;
                        outputDataList.Add(stationDataTrain);
                    }
                }
            }
        }
    }
}
