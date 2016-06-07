using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PIS.Ground.DataPackage
{
    /// <summary>
    /// DataPackageCallbackService Allows the RemoteDataStore to notify DataPackage about update/assign or check events.
    /// </summary>
    [ServiceContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/", Name = "DataPackageCallbackService")]
    [ServiceKnownType(typeof(Notification.NotificationIdEnum))]
    public interface IDataPackageCallbackService
    {
        [OperationContract(IsOneWay = true)]
        void updateBaselineDefinitionStatus(Guid pReqID, string pBLVersion, Notification.NotificationIdEnum pStatus);

        [OperationContract(IsOneWay = true)]
        void missingDataPackageNotification(Guid pReqID, Dictionary<string, string> pDPCharsList);

        [OperationContract(IsOneWay = true)]
        void updatePackageUploadStatus(Guid pReqID, Notification.NotificationIdEnum pStatus, Dictionary<string, string> pDPCharsList);

        [OperationContract(IsOneWay = true)]
        void updateBaselineAssignmentStatus(Guid pReqID, Notification.NotificationIdEnum pStatus, string pElementId, string pBLVersion);       
    }
}

