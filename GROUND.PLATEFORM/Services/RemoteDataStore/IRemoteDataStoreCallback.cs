using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace PIS.Ground.RemoteDataStore
{
    /// <summary>
    /// RemoteDataStore notification allows the remote data store to notify the ground app or data package about update/assign or check events.
    /// </summary>
    [ServiceContract]
    public interface IRemoteDataStoreCallback
    {
        [OperationContract(IsOneWay = true)]
        void updateBaselineDefinitionStatus(Guid pReqID, string pBLVersion, StatusEnum pStatus);

        [OperationContract(IsOneWay = true)]
        void missingDataPackageNotification(Guid pReqID, List<string> pDPCharsList);

        [OperationContract(IsOneWay = true)]
        void updatePackageUploadStatus(Guid pReqID, StatusEnum pStatus);

        [OperationContract(IsOneWay = true)]
        void updateBaselineAssignmentStatus(Guid pReqID, StatusEnum pStatus);

        [OperationContract(IsOneWay = true)]
        void updatePackageDistributionStatus(Guid pReqID, string pEID, StatusEnum pStatus);

        [OperationContract(IsOneWay = true)]
        void futureBaselineDefinitionNotification(Guid pReqID, string pEID, object pFBaseline);
        
    }

    /// <summary>
    /// Enum used by the callback class to send notification about a status (transfer, assign, check,...).
    /// </summary>
    [DataContract]
    public enum StatusEnum
    {
        [EnumMember]
        PROCESSING = 0,
        [EnumMember]
        ALREADY_EXISTS = 1,
        [EnumMember]
        COMPLETED = 2,
        [EnumMember]
        FAILED = 3,
        [EnumMember]
        WAITING_TO_TRANSFER = 4,
        [EnumMember]
        TRANSFERRING = 5,
        [EnumMember]
        TRANSFERRED = 6,
        [EnumMember]
        TIME_OUT = 7,
        [EnumMember]
        FAILED_DATA_PACKAGE_MISSING = 8,
        [EnumMember]
        FAILED_UNKNOW_ONBOARD_DATA_PACKAGE = 9,
        [EnumMember]
        FAILED_NO_ASSIGNED_FUTURE_BASELINE = 10,
        [EnumMember]
        FAILED_TRANSFER_CANCELLED_MANUALY = 11,
        [EnumMember]
        FAILED_REJECTED_BY_ELEMENT = 12,
        [EnumMember]
        INHIBITED = 13,
    }    
}
