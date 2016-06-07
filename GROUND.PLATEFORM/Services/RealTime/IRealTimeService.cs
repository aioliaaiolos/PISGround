//---------------------------------------------------------------------------------------------------
// <copyright file="IRealTimeService.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2014.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using PIS.Ground.Core.Data;

namespace PIS.Ground.RealTime
{
	#region web service interface

	/// <summary>
	/// The PIS-GroundApp interface allows to exchange real time information about the weather, connections, platform, and delay  of the current mission or the mission station. This interface provides the GroundApp user with commands to:
	/// 1.	Get available Element List;
	/// 2.	Retreive Station list;
	/// 3.	Get Mission RealTime information;
	/// 4.	Set Mission RealTime information;
	/// 5.	Get Station Real Time information;
	/// 6.	Set Station Real Time information.
	/// </summary>
	[ServiceContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeService")]
	public interface IRealTimeService
	{		
		/// <summary>
		/// This function allows the GroundApp to request from the ground PIS the list of available elements. This list includes also missions that are running for each element, and the versions of the LMT and PIS Base data packages.
		/// </summary>
		/// <param name="sessionId">A valid session identifier or 0, if the provided credentials are not valid.</param>
		/// <returns>The code “request accepted” when the command is valid and the list of elements, or and error code when the command is rejected.</returns>
		[OperationContract]
		RealTimeAvailableElementListResult GetAvailableElementList(Guid sessionId);

		/// <summary>
		/// The function commands the retreival of the station list from an addressee. If a mission code is provided, only the stations of that mission are returned. By default, all the station list from the database are returned.
		/// </summary>
		/// <param name="sessionId">A valid session identifier or 0, if the provided credentials are not valid.</param>
		/// <param name="missionCode">Mission Code of a particular mission.</param>
		/// <param name="elementId">The element alpha number.</param>
		/// <returns>The code “request accepted” when the command is valid and the list of stations for the given mission code/element id, or and error code when the command is rejected.</returns>
		[OperationContract]
		RealTimeRetrieveStationListResult RetrieveStationList(Guid sessionId, string missionCode, string elementId);

		/// <summary>
		/// This function allows the GroundApp to obtain Mission specific Real Time information.
		/// </summary>
		/// <param name="sessionId">A valid session identifier or 0, if the provided credentials are not valid.</param>
		/// <param name="missionCode">Mission Code of a particular mission.</param>
		/// <returns>The code “request accepted” when the command is valid and the list of informations for the given mission code, or and error code when the command is rejected.</returns>
		[OperationContract]
		RealTimeGetMissionRealTimeInformationResult GetMissionRealTimeInformation(Guid sessionId, string missionCode);

		/// <summary>
		/// This function allows the GroundApp to set the Mission Real Time information for a mission.
		/// </summary>
		/// <param name="sessionId">A valid session identifier or 0, if the provided credentials are not valid.</param>
		/// <param name="missionCode">Mission Code of a particular mission.</param>
		/// <param name="missionDelay">Delay informations for the mission.</param>
		/// <param name="missionWeather">Weather informations for the mission.</param>
		/// <returns>Error code according to success or not of the request.</returns>
		[OperationContract]
		RealTimeSetMissionRealTimeInformationResult SetMissionRealTimeInformation(Guid sessionId, string missionCode, RealTimeDelayType missionDelay, RealTimeWeatherType missionWeather);

		/// <summary>
		/// This function allows the GroundApp to obtain the Station Real Time information associated to a particular list of station.
		/// </summary>
		/// <param name="sessionId">A valid session identifier or 0, if the provided credentials are not valid.</param>
		/// <param name="missionCode">Mission Code of a particular mission.</param>
		/// <param name="stationList">List of Station ID.</param>
		/// <returns>The code “request accepted” when the command is valid and the list of informations for the given stations codes, or and error code when the command is rejected.</returns>
		[OperationContract]
		RealTimeGetStationRealTimeInformationResult GetStationRealTimeInformation(Guid sessionId, string missionCode, List<string> stationList);

		/// <summary>
		/// This function allows the GroundApp to set Real Time information for a list of stations of a specific mission.
		/// </summary>
		/// <param name="sessionId">A valid session identifier or 0, if the provided credentials are not valid.</param>
		/// <param name="missionCode">Mission Code of a particular mission.</param>
		/// <param name="stationInformationList">List of station informations.</param>
		/// <returns>Error code according to success or not of the request with list of specific error code for each station.</returns>
		[OperationContract]
		RealTimeSetStationRealTimeInformationResult SetStationRealTimeInformation(Guid sessionId, string missionCode, List<RealTimeStationInformationType> stationInformationList);

		/// <summary>
		/// This function allows the GroundApp to clear the Real Time information associated to a particular mission and list of station.
		/// </summary>
		/// <param name="sessionId">A valid session identifier or 0, if the provided credentials are not valid.</param>
		/// <param name="missionCode">Mission Code of a particular mission.</param>
		/// <param name="stationList">List of Station ID. If null or empty, will clear all mission related data, including all station data linked to the mission.</param>
		/// <returns>The code “request accepted” when the command is valid and the list of informations for the given stations codes, or and error code when the command is rejected.</returns>
		[OperationContract]
		RealTimeClearRealTimeInformationResult ClearRealTimeInformation(Guid sessionId, string missionCode, List<string> stationList);
	}

	#endregion

	#region enums

	/// <summary>
	/// Error code for all RealTime service requests..
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeStartServiceErrorEnum")]
	public enum RealTimeServiceErrorEnum
	{
		/// <summary>
		/// Request accepted, no error in the format.
		/// </summary>
		[EnumMember(Value = "REQUEST_ACCEPTED")]
		RequestAccepted = 0,

		/// <summary>
		/// Invalid Session ID used in the request.
		/// </summary>
		[EnumMember(Value = "ERROR_INVALID_SESSION_ID")]
		ErrorInvalidSessionId = 1,

		/// <summary>
		/// Error while trying to generate a Request Id.
		/// </summary>
		[EnumMember(Value = "ERROR_REQUESTID_GENERATION")]
		ErrorRequestIdGeneration = 2,

		/// <summary>
		/// Mission code unknown for element defined in request.
		/// </summary>
		[EnumMember(Value = "ERROR_INVALID_MISSION_CODE")]
		ErrorInvalidMissionCode = 3,

		/// <summary>
		/// Invalid Station ID supplied with the request.
		/// </summary>
		[EnumMember(Value = "ERROR_INVALID_STATION_ID")]
		ErrorInvalidStationId = 4,

		/// <summary>
		/// Invalid element ID used in the request.
		/// </summary>
		[EnumMember(Value = "ERROR_INVALID_ELEMENT_ID")]
		ErrorInvalidElementId = 5,

		/// <summary>
		/// Provided information list raised a limit exceeded error.
		/// </summary>
		[EnumMember(Value = "ERROR_STATION_LIST_LIMIT_EXCEDEED")]
		ErrorStationListLimitExcedeed = 6,

		/// <summary>
		/// No RealTime data.
		/// </summary>
		[EnumMember(Value = "ERROR_NO_RTPIS_DATA")]
		ErrorNoRtpisData = 7,

		/// <summary>
		/// PIS Datastore not accessible, preventing request to be process.
		/// </summary>
		[EnumMember(Value = "ERROR_REMOTE_DATASTORE_UNAVAILABLE")]
		ErrorRemoteDatastoreUnavailable = 8,

		/// <summary>
		/// No delay information in command.
		/// </summary>
		[EnumMember(Value = "INFO_NO_DELAY_DATA")]
		InfoNoDelayData = 9,

		/// <summary>
		/// No weather information in command.
		/// </summary>
		[EnumMember(Value = "INFO_NO_WEATHER_DATA")]
		InfoNoWeatherData = 10,

		/// <summary>
		/// No data associated to requested element ID.
		/// </summary>
		[EnumMember(Value = "INFO_NO_DATA_FOR_ELEMENT")]
		InfoNoDataForElement = 11,

		/// <summary>
		/// PIS ground can't get the list of elements from T2G Ground.
		/// </summary>
		[EnumMember(Value = "ELEMENT_LIST_NOT_AVAILABLE")]
		ElementListNotAvailable = 12,

		/// <summary>
		/// PIS ground can't get data from T2G Ground.
		/// </summary>
		[EnumMember(Value = "T2G_SERVER_OFFLINE")]
		T2GServerOffline = 13,
	}

	/// <summary>
	/// Error code for station requests..
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeStationResultEnum")]
	public enum RealTimeStationResultEnum
	{
		/// <summary>
		/// Data taken into account for station.
		/// </summary>
		[EnumMember(Value = "DATA_OK")]
		DataOk = 0,

		/// <summary>
		/// Station ID provided is not associated with identified MissionCode.
		/// </summary>
		[EnumMember(Value = "ERROR_INVALID_STATION_FOR_MISSION")]
		ErrorInvalidStationForMission = 1,

		/// <summary>
		/// No data for this block of information (delay, platform, weather pr connection).
		/// </summary>
		[EnumMember(Value = "INFO_NO_DATA")]
		InfoNoData = 2,
	}

	#endregion

	#region input types

	/// <summary>
	/// Informations about delay for a station/mission.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeDelayType")]
	public class RealTimeDelayType
	{
		/// <summary>
		/// Station/mission delay in minutes.
		/// </summary>
		[DataMember(IsRequired = true)]
		public uint Delay;
		
		/// <summary>
		/// Delay reason in text.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string DelayReason;

		/// <summary>
		/// Predefined delay reason code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public int DelayReasonCode;

		/// <summary>Date/Time of the update.</summary>
		public DateTime UpdateDate;
	}

	/// <summary>
	/// Informations about delay for a station/mission.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeWeatherType")]
	public class RealTimeWeatherType
	{
		/// <summary>
		/// General temperature for the mission in Celcius.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string TemperatureInCentigrade;

		/// <summary>
		/// General temperature for the mission in Farenheit.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string TemperatureInFahrenheit;

		/// <summary>
		/// Global weather for the mission in text.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string WeatherCondition;

		/// <summary>
		/// Global weather predetermined code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public int WeatherConditionCode;

		/// <summary>
		/// General humidity level for the mission in text.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Humidity;

		/// <summary>Date/Time of the update.</summary>
		public DateTime UpdateDate;
	}

	/// <summary>
	/// Station specific informations.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeStationInformationType")]
	public class RealTimeStationInformationType
	{
		/// <summary>
		/// Station id.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string StationCode;

		/// <summary>
		/// Station detailed data.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeStationDataType StationData;

		/// <summary>Updates from data described by src.</summary>
		/// <param name="src">Source for the update.</param>
		public void UpdateFrom(RealTimeStationInformationType src)
		{
			if (src != null)
			{
				if (string.Compare(this.StationCode, src.StationCode) == 0)
				{
					if (src.StationData != null)
					{
						if (this.StationData == null)
						{
							this.StationData = src.StationData;
						}
						else
						{
							if (src.StationData.StationDelay != null)
							{
								this.StationData.StationDelay = src.StationData.StationDelay;
								this.StationData.StationDelay.UpdateDate = DateTime.Now;
							}

							if (src.StationData.StationPlatform != null)
							{
								this.StationData.StationPlatform = src.StationData.StationPlatform;
								this.StationData.StationPlatform.UpdateDate = DateTime.Now;
							}

							if (src.StationData.StationWeather != null)
							{
								this.StationData.StationWeather = src.StationData.StationWeather;
								this.StationData.StationWeather.UpdateDate = DateTime.Now;
							}

							if (src.StationData.StationConnectionList != null)
							{
								if (this.StationData.StationConnectionList == null)
								{
									this.StationData.StationConnectionList = src.StationData.StationConnectionList;
								}
								else
								{
									var srcConnectionList = new List<RealTimeStationConnectionType>(src.StationData.StationConnectionList);
									for (int i = 0; i < this.StationData.StationConnectionList.Count; i++)
									{
										var storedconnection = this.StationData.StationConnectionList[i];
										var newConnection = srcConnectionList.Find(x => string.Compare(x.Operator, storedconnection.Operator) == 0 && string.Compare(x.CommercialNumber, storedconnection.CommercialNumber) == 0);
										if (newConnection != null)
										{
											this.StationData.StationConnectionList[i] = newConnection;
											this.StationData.StationConnectionList[i].UpdateDate = DateTime.Now;
											srcConnectionList.Remove(newConnection);
										}
									}

									foreach (var newConnection in srcConnectionList)
									{
										if (newConnection != null)
										{
											this.StationData.StationConnectionList.Add(newConnection);
										}
									}
								}
							}
						}
					}
				}
			}
		}

        /// <summary>Set the update date.</summary>
        /// <param name="updateDate">update date.</param>
        public void SetUpdateDate(System.DateTime updateDate)
        {
            if (this.StationData != null)
            {
                if (this.StationData.StationDelay != null)
                {
					this.StationData.StationDelay.UpdateDate = updateDate;
                }

                if (this.StationData.StationPlatform != null)
                {
					this.StationData.StationPlatform.UpdateDate = updateDate;
                }

                if (this.StationData.StationWeather != null)
                {
					this.StationData.StationWeather.UpdateDate = updateDate;
                }

                if (this.StationData.StationConnectionList != null)
                {                                        
                    for (int i = 0; i < this.StationData.StationConnectionList.Count; i++)
                    {
						this.StationData.StationConnectionList[i].UpdateDate = updateDate;                            
                    }                    
                }
            }
        }
	}

	/// <summary>
	/// Station detailed data.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "StationDataType")]
	public class RealTimeStationDataType
	{
		/// <summary>
		/// Informations about delay for a station.
		/// </summary>
		[DataMember(IsRequired = false)]
		public RealTimeDelayType StationDelay;

		/// <summary>
		/// Informations about platform for a station.
		/// </summary>
		[DataMember(IsRequired = false)]
		public RealTimeStationPlatformType StationPlatform;

		/// <summary>
		/// List of informations about connections for a station.
		/// </summary>
		[DataMember(IsRequired = false)]
		public List<RealTimeStationConnectionType> StationConnectionList;

		/// <summary>
		/// Informations about weather for a station.
		/// </summary>
		[DataMember(IsRequired = false)]
		public RealTimeWeatherType StationWeather;
	}

	/// <summary>
	/// Informations about platform for a station.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "StationPlatformType")]
	public class RealTimeStationPlatformType
	{
		/// <summary>
		/// Identification of a platform in text.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Platform;

		/// <summary>
		/// Track name for the platform.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Track;

		/// <summary>
		/// Track predetermined code for the platform.
		/// </summary>
		[DataMember(IsRequired = true)]
		public int TrackCode;

		/// <summary>
		/// Exit side of the element for the platform.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string ExitSide;

		/// <summary>
		/// Exit side predetermined code for the platform.
		/// </summary>
		[DataMember(IsRequired = true)]
		public int ExitSideCode;

		/// <summary>
		/// Platform issue description, if any.
		/// </summary>
		[DataMember(IsRequired = false)]
		public string IssueDescription;

		/// <summary>
		/// Platform issue description predetermined code, if any.
		/// </summary>
		[DataMember(IsRequired = false)]
		public int? IssueDescriptionCode;

		/// <summary>Date/Time of the update.</summary>
		public DateTime UpdateDate;
	}

	/// <summary>
	/// Informations about connections for a station.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "StationConnectionType")]
	public class RealTimeStationConnectionType
	{
		/// <summary>
		/// Operator of the connection.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Operator;

		/// <summary>
		/// Predetermined operator code of the connection.
		/// </summary>
		[DataMember(IsRequired = true)]
		public int OperatorCode;

		/// <summary>
		/// Commercial number of the connection.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string CommercialNumber;
		
		/// <summary>
		/// Type of connection.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string ModelType;

		/// <summary>
		/// Predetermined code of the type of the connection.
		/// </summary>
		[DataMember(IsRequired = true)]
		public int ModelTypeCode;

		/// <summary>
		/// Connection’s line name.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Line;

		/// <summary>
		/// Connection’s line predetermined code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public int LineCode;

		/// <summary>
		/// Departure time of the connection.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string DepartureTime;

		/// <summary>
		/// Next departure time of the connection.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string NextDepartureTime;

		/// <summary>
		/// Departure frequency for this connection.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string DepartureFrequency;

		/// <summary>
		/// Platform associated to this connection.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string ConnectionPlatform;

		/// <summary>
		/// Delay for connection departure, if any.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string ConnectionDelay;

		/// <summary>
		/// Destination of the current connection.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string DestinationName;

		/// <summary>Date/Time of the update.</summary>
		public DateTime UpdateDate;
	}

	#endregion

	#region output types

	/// <summary>
	/// Return type for GetAvailableElementList request.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeAvailableElementListResult")]
	public class RealTimeAvailableElementListResult
	{
		/// <summary>
		/// Request result code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeServiceErrorEnum ResultCode;

		/// <summary>
		/// List of elements.
		/// </summary>
		[DataMember(IsRequired = true)]
		public ElementList<AvailableElementData> ElementList;
	}

	/// <summary>
	/// Return type for RetrieveStationList request.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeRetrieveStationListResult")]
	public class RealTimeRetrieveStationListResult
	{
		/// <summary>
		/// Request identifier.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Guid RequestId;

		/// <summary>
		/// Request result code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeServiceErrorEnum ResultCode;

		/// <summary>
		/// Request mission code.
		/// </summary>
		[DataMember(IsRequired = false)]
		public string MissionCode;

		/// <summary>
		/// Request element id.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string ElementID;

		/// <summary>
		/// Station id list.
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<string> StationList;
	}

	/// <summary>
	/// Mission detailed real time data.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeGetMissionRealTimeInformationResult")]
	public class RealTimeGetMissionRealTimeInformationResult
	{
		/// <summary>
		/// Request identifier.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Guid RequestId;

		/// <summary>
		/// Request result code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeServiceErrorEnum ResultCode;

		/// <summary>
		/// Request mission code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string MissionCode;

		/// <summary>
		/// Request mission data.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeInformationType InformationStructure;
	}

	/// <summary>
	/// Mission detailed real time data.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeInformationType")]
	public class RealTimeInformationType
	{
		/// <summary>
		/// Informations about delay for a mission.
		/// </summary>
		[DataMember(IsRequired = false)]
		public RealTimeDelayType MissionDelay;

		/// <summary>
		/// Informations about weather for a mission.
		/// </summary>
		[DataMember(IsRequired = false)]
		public RealTimeWeatherType MissionWeather;
	}

	/// <summary>
	/// Set mission detailed real time data result.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeSetMissionRealTimeInformationResult")]
	public class RealTimeSetMissionRealTimeInformationResult
	{
		/// <summary>
		/// Request identifier.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Guid RequestId;

		/// <summary>
		/// Request result code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeServiceErrorEnum ResultCode;
	}

	/// <summary>
	/// Station list detailed real time data.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeGetStationRealTimeInformationResult")]
	public class RealTimeGetStationRealTimeInformationResult
	{
		/// <summary>
		/// Request identifier.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Guid RequestId;

		/// <summary>
		/// Request result code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeServiceErrorEnum ResultCode;

		/// <summary>
		/// Request station data.
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<RealTimeStationStatusType> StationStatusList;
	}

	/// <summary>
	/// Station detailed real time data.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeStationStatusType")]
	public class RealTimeStationStatusType
	{
		/// <summary>Initializes a new instance of the RealTimeStationStatusType class.</summary>
		public RealTimeStationStatusType()
		{
		}

		/// <summary>Initializes a new instance of the RealTimeStationStatusType class.</summary>
		/// <param name="src">RealTimeStationInformationType to copy data from.</param>
		public RealTimeStationStatusType(RealTimeStationInformationType src)
		{
			if (src != null)
			{
				this.StationID = src.StationCode;

				if (src.StationData != null && !(src.StationData.StationDelay == null && src.StationData.StationPlatform == null &&
					src.StationData.StationConnectionList == null && src.StationData.StationWeather == null ))
				{
					this.StationDelay = src.StationData.StationDelay;
					this.StationPlatform = src.StationData.StationPlatform;
					this.StationConnectionList = src.StationData.StationConnectionList;
					this.StationWeather = src.StationData.StationWeather;
					this.StationResult = RealTimeStationResultEnum.DataOk;
				}
				else
				{
					this.StationResult = RealTimeStationResultEnum.InfoNoData;
				}
			}
		}

		/// <summary>
		/// Station identifier.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string StationID;

		/// <summary>
		/// Request result code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeStationResultEnum StationResult;

		/// <summary>
		/// Informations about delay for a mission.
		/// </summary>
		[DataMember(IsRequired = false)]
		public RealTimeDelayType StationDelay;

		/// <summary>
		/// Informations about platform for a station.
		/// </summary>
		[DataMember(IsRequired = false)]
		public RealTimeStationPlatformType StationPlatform;

		/// <summary>
		/// List of informations about connections for a station.
		/// </summary>
		[DataMember(IsRequired = false)]
		public List<RealTimeStationConnectionType> StationConnectionList;

		/// <summary>
		/// Informations about weather for a mission.
		/// </summary>
		[DataMember(IsRequired = false)]
		public RealTimeWeatherType StationWeather;
	}

	/// <summary>
	/// Set station detailed real time data result.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeSetStationRealTimeInformationResult")]
	public class RealTimeSetStationRealTimeInformationResult
	{
		/// <summary>
		/// Request identifier.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Guid RequestId;

		/// <summary>
		/// Request result code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeServiceErrorEnum ResultCode;

		/// <summary>
		/// Request mission code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string MissionCode;

		/// <summary>
		/// Request station data.
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<RealTimeStationResultType> StationResultList;
	}

	/// <summary>
	/// Station short real time data result.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeStationResultType")]
	public class RealTimeStationResultType
	{
		/// <summary>
		/// Station identifier.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string StationID;

		/// <summary>
		/// Delay result code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeStationResultEnum DelayResult;

		/// <summary>
		/// Platform result code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeStationResultEnum PlatformResult;

		/// <summary>
		/// Weather result code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeStationResultEnum WeatherResult;

		/// <summary>
		/// Connection result list.
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<RealTimeConnectionResultType> ConnectionsResultList;
	}

	/// <summary>
	/// Connection short real time data result.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeConnectionResultType")]
	public class RealTimeConnectionResultType
	{
		/// <summary>
		/// Operator of the connection.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Operator;

		/// <summary>
		/// Commercial number of the connection.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string CommercialNumber;

		/// <summary>
		/// Connection result code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeStationResultEnum ConnectionResult;
	}

	/// <summary>
	/// Clear detailed data.
	/// </summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/realtime/", Name = "RealTimeClearRealTimeInformationResult")]
	public class RealTimeClearRealTimeInformationResult
	{
		/// <summary>
		/// Request identifier.
		/// </summary>
		[DataMember(IsRequired = true)]
		public Guid RequestId;

		/// <summary>
		/// Request result code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public RealTimeServiceErrorEnum ResultCode;

		/// <summary>
		/// Request mission code.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string MissionCode;

		/// <summary>
		/// Request mission data.
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<string> StationList;
	}

	#endregion
}