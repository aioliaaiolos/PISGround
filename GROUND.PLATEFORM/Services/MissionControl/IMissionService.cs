//---------------------------------------------------------------------------------------------------
// <copyright file="IMissionService.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2014.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.T2G;

namespace PIS.Ground.Mission
{
    /// <summary>
    /// Session service
    /// </summary>
    [ServiceContract(Namespace = "http://alstom.com/pacis/pis/ground/mission/")]
    public interface IMissionService
    {        
        /// <summary>
        /// This function allows the GroundApp to request from the ground PIS the list of available elements. This list includes also missions that are running for each element, and the versions of the LMT and PIS Base data packages
        /// </summary>
        /// <param name="sessionId">A valid session identifier or 0, if the provided credentials are not valid</param>
        /// <returns>The code “request accepted” when the command is valid and the list of elements, or and error code when the command is rejected</returns>
        [OperationContract]
        MissionAvailableElementListResult GetAvailableElementList(Guid sessionId);

        /// <summary>
        /// This function allows the GroundApp to initialize a mission onboard an element. If the station list isn't provided - an existing mission is initialized. Otherwise - a new mission is initialized using existing stations.
        /// </summary>
        /// <param name="sessionId">A valid session identifier or 0, if the provided credentials are not valid</param>
        /// <param name="missionCode">The mission code of the particular mission</param>
        /// <param name="elementId">The element alpha number</param>
        /// <param name="stationList">The stations code list to initialize a new mission</param>
        /// <param name="timeOut">The timeout that is used to send the "ProgressTimeOut" notification to the GroundApp</param>        
        /// <returns>The code “request accepted” when the command is valid, or an error code when the command is rejected</returns>
        [OperationContract]
        MissionInitializeMissionResult InitializeMission(Guid sessionId, string missionCode, string elementId, List<string> stationList, int? timeOut);

        /// <summary>
        /// This function allows the GroundApp to cancel a mission onboard an element. 
        /// </summary>
        /// <param name="sessionId">A valid session identifier or 0, if the provided credentials are not valid</param>
        /// <param name="missionCode">The mission code of the particular mission</param>
        /// <param name="elementId">The element alpha number</param>
        /// <param name="timeOut">The timeout that is used to send the "ProgressTimeOut" notification to the GroundApp</param>
        /// <returns>The code “request accepted” when the command is valid, or an error code when the command is rejected</returns>
        [OperationContract]
        MissionCancelMissionResult CancelMission(Guid sessionId, string missionCode, string elementId, int? timeOut);
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/mission/", Name = "MissionServiceErrorCodeEnum")]
    public enum MissionServiceErrorCodeEnum
    {
        /// <summary>
        /// Request accepted, no error in the format
        /// </summary>
        [EnumMember(Value = "REQUEST_ACCEPTED")]
        RequestAccepted = 0,

        /// <summary>
        /// Invalid session Id provided in the request
        /// </summary>
        [EnumMember(Value = "ERROR_INVALID_SESSION_ID")]
        ErrorInvalidSessionId = 1,

        /// <summary>
        /// Remote Data Store is unavailable. The request can't be processed
        /// </summary>
        [EnumMember(Value = "ERROR_REMOTE_DATASTORE_UNAVAILABLE")]
        ErrorRemoteDatastoreUnavailable = 2,

        /// <summary>
        /// Invalid element Id provided in the request
        /// </summary>
        [EnumMember(Value = "ERROR_INVALID_ELEMENT_ID")]
        ErrorInvalidElementId = 3,

        /// <summary>
        /// At least one of the station Id provided in the request is invalid
        /// </summary>
        [EnumMember(Value = "ERROR_INVALID_STATION_ID")]
        ErrorInvalidStationId = 4,

        /// <summary>
        /// Invalid mission Id provided in the request
        /// </summary>
        [EnumMember(Value = "ERROR_INVALID_MISSION_CODE")]
        ErrorInvalidMissionCode = 5,

        /// <summary>
        /// PIS Ground doesn't know the onboard LMT Db version
        /// </summary>
        [EnumMember(Value = "ERROR_UNKNOWN_LMT_DB")]
        ErrorUnknownLMTDb = 6,

        /// <summary>
        /// PIS Ground can't read the LMT Db content
        /// </summary>
        [EnumMember(Value = "ERROR_OPENING_LMT_DB")]
        ErrorOpeningLMTDb = 7,

        /// <summary>
        /// PIS Ground can't get the list of elements from T2G Ground.
        /// </summary>
        [EnumMember(Value = "ERROR_ELEMENT_LIST_NOT_AVAILABLE")]
        ErrorElementListNotAvailable = 8,

        /// <summary>
        /// Invalid timeout value provided in the request
        /// </summary>
        [EnumMember(Value = "ERROR_INVALID_REQUEST_TIMEOUT")]
        ErrorInvalidRequestTimeout = 9,
    }

    /// <summary>
    /// Return type for GetAvailableElementList request
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/mission/", Name = "MissionAvailableElementListResult")]
    public class MissionAvailableElementListResult
    {
        /// <summary>
        ///  Request result code
        /// </summary>
        [DataMember(IsRequired = true)]
        public MissionServiceErrorCodeEnum ResultCode;
        
        /// <summary>
        /// List of elements
        /// </summary>
        [DataMember(IsRequired = true)]
        public ElementList<AvailableElementData> ElementList;
    }

    /// <summary>
    /// Return type for InitializeMission request
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/mission/", Name = "MissionInitializeMissionResult")]
    public class MissionInitializeMissionResult
    {
        /// <summary>
        /// Request identifier
        /// </summary>
        [DataMember(IsRequired = true)]
        public Guid RequestId;

        /// <summary>
        /// Request mission code
        /// </summary>
        [DataMember(IsRequired = true)]
        public string MissionCode;

        /// <summary>
        /// Request result code
        /// </summary>
        [DataMember(IsRequired = true)]
        public MissionServiceErrorCodeEnum ResultCode;

        /// <summary>
        /// Invalid stations list
        /// </summary>
        [DataMember(IsRequired = false)]
        public List<string> InvalidStationList;               
    }

    /// <summary>
    /// Return type for CancelMission request
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/mission/", Name = "MissionCancelMissionResult")]
    public class MissionCancelMissionResult
    {
        /// <summary>
        /// Request identifier
        /// </summary>
        [DataMember(IsRequired = true)]
        public Guid RequestId;

        /// <summary>
        /// Request mission code
        /// </summary>
        [DataMember(IsRequired = true)]
        public string MissionCode;

        /// <summary>
        /// Request result code
        /// </summary>
        [DataMember(IsRequired = true)]
        public MissionServiceErrorCodeEnum ResultCode;
    }
}
