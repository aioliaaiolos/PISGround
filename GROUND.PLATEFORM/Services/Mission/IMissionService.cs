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
        [OperationContract]
        MissionServiceElementListResult GetAvailableElementList(Guid sessionId);

        [OperationContract]
        MissionServiceResult InitializeAutomaticMission(AutomaticModeRequest automaticModeRequest);

        [OperationContract]
        MissionServiceResult InitializeModifiedMission(ModifiedModeRequest modifiedModeRequest);

        [OperationContract]
        MissionServiceResult InitializeManualMission(ManualModeRequest manualModeRequest);

        [OperationContract]
        MissionServiceResult StopMission(Guid sessionId, bool onBoardValidationFlag, string elementAlphaNumber, string missionId, uint timeOut);
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/mission/", Name = "MissionErrorCode")]
    public enum MissionErrorCode
    {
        [EnumMember(Value = "REQUEST_ACCEPTED")]
        RequestAccepted,

        [EnumMember(Value = "INVALID_SESSION_ID")]
        InvalidSessionId,

        [EnumMember(Value = "INVALID_REQUEST_TIMEOUT")]
        InvalidRequestTimeout,

        [EnumMember(Value = "PIS_DATASTORE_NOT_ACCESSIBLE")]
        DatastoreNotAccessible,

        [EnumMember(Value = "ELEMENT_LIST_NOT_AVAILABLE")]
        ElementListNotAvailable,

        [EnumMember(Value = "INTERNAL_ERROR")]
        InternalError,

        [EnumMember(Value = "T2G_SERVER_OFFLINE")]
        T2GServerOffline,

        [EnumMember(Value = "INVALID_TRAIN_NAME")]
        InvalidTrainName
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/mission/", Name = "MissionServiceResult")]
    public class MissionServiceResult
    {
        [DataMember]
        public MissionErrorCode ResultCode { get; set; }

        [DataMember]
        public Guid RequestId { get; set; }
    }
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/mission/", Name = "MissionServiceElementListResult")]
    public class MissionServiceElementListResult
    {
        [DataMember]
        public ElementList<AvailableElementData> ElementList;

        [DataMember]
        public MissionErrorCode ResultCode;
    }
}
