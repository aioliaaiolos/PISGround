using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using PIS.Ground.Core.Data;

namespace PIS.Ground.Infotainment.Journaling
{
    [ServiceContract(Namespace="http://alstom.com/pacis/pis/ground/infotainment/journaling/", Name="JournalingService")]
    interface IJournalingService
    {
        [OperationContract]
        InfotainmentJournalingElementListResponse GetAvailableElementList(Guid sessionId); // TODO -> GroundCore
        
        [OperationContract]
        GetReportResponse GetReport(Guid SessionId, TargetAddressType TargetAddress, uint RequestTimeout);
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/infotainment/journaling/", Name = "InfotainmentJournalingGetReportResponse")]
    public class GetReportResponse
    {
        [DataMember]
        public Guid RequestId;

        [DataMember]
        public ResultCodeEnum ResultCode;      
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/infotainment/journaling/", Name = "InfotainmentJournalingElementListResponse")]
    public class InfotainmentJournalingElementListResponse
    {        
        [DataMember]
        public ResultCodeEnum ResultCode;

        [DataMember]
        public ElementList<AvailableElementData> ElementList;
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/infotainment/journaling/", Name = "ResultCodeEnum")]
    public enum ResultCodeEnum
    {
        [EnumMember(Value = "INTERNAL_ERROR")]
        InternalError,

        [EnumMember(Value = "REQUEST_ACCEPTED")]
        RequestAccepted,

        [EnumMember(Value = "INVALID_SESSION_ID")]
        InvalidSessionId,

        [EnumMember(Value = "INVALID_REQUEST_TIMEOUT")]
        InvalidRequestTimeout,

        [EnumMember(Value = "ELEMENT_LIST_NOT_AVAILABLE")]
        ElementListNotAvailable,

        [EnumMember(Value = "UNKNOWN_ELEMENT_ID")]
        UnknownElementId,

        [EnumMember(Value = "UNKNOWN_MISSION_ID")]
        UnknownMissionId,

        [EnumMember(Value = "T2G_SERVER_OFFLINE")]
        T2GServerOffline
    }


}