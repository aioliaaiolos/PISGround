//---------------------------------------------------------------------------------------------------
// <copyright file="T2GNotificationServiceStub.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace DataPackageTests.T2GServiceInterface.Notification
{
    /// <summary>
    /// Specialization of systemInfoStruct class.
    /// </summary>
    public partial class systemInfoStruct
    {
        /// <summary>
        /// Convert this instance to systemInfoStruct for PIS-Ground core service.
        /// </summary>
        public PIS.Ground.Core.T2G.WebServices.Notification.systemInfoStruct AsImplementation
        {
            get
            {
                PIS.Ground.Core.T2G.WebServices.Notification.systemInfoStruct other = new PIS.Ground.Core.T2G.WebServices.Notification.systemInfoStruct();
                other.communicationLink = (PIS.Ground.Core.T2G.WebServices.Notification.commLinkEnum)communicationLink;
                other.isOnline = isOnline;
                other.missionId = missionId;
                other.status = status;
                other.systemId = systemId;
                other.vehiclePhysicalId = vehiclePhysicalId;

                return other;
            }
        }
    }

    /// <summary>
    /// Specialization of serviceStruct class.
    /// </summary>
    public partial class serviceStruct
    {
        /// <summary>
        /// Convert this instance to serviceStruct for PIS-Ground core service.
        /// </summary>
        public PIS.Ground.Core.T2G.WebServices.Notification.serviceStruct AsImplementation
        {
            get
            {
                PIS.Ground.Core.T2G.WebServices.Notification.serviceStruct other = new PIS.Ground.Core.T2G.WebServices.Notification.serviceStruct();
                other.AID = AID;
                other.carId = carId;
                other.carIdStr = carIdStr;
                other.deviceId = deviceId;
                other.deviceName = deviceName;
                other.isAvailable = isAvailable;
                other.name = name;
                other.operatorId = operatorId;
                other.serviceId = serviceId;
                other.serviceIPAddress = serviceIPAddress;
                other.servicePortNumber = servicePortNumber;
                other.SID = SID;
                other.vehiclePhysicalId = vehiclePhysicalId;

                return other;
            }
        }
    }

    /// <summary>
    /// Specialization of fieldStruct class.
    /// </summary>
    public partial class fieldStruct
    {
        /// <summary>
        /// Convert this instance to fieldStruct for PIS-Ground core service.
        /// </summary>
        public PIS.Ground.Core.T2G.WebServices.Notification.fieldStruct AsImplementation
        {
            get
            {
                PIS.Ground.Core.T2G.WebServices.Notification.fieldStruct other = new PIS.Ground.Core.T2G.WebServices.Notification.fieldStruct();
                other.id = id;
                other.type = (PIS.Ground.Core.T2G.WebServices.Notification.fieldTypeEnum)type;
                other.value = value;
                return other;
            }
        }
    }
}

namespace DataPackageTests.ServicesStub
{
    /// <summary>
    /// Implementation of the T2GNotification service interface
    /// </summary>
    [ServiceBehaviorAttribute(InstanceContextMode = InstanceContextMode.Single, ConfigurationName = "DataPackageTests.T2GServiceInterface.Notification.NotificationPortType")]
    class T2GNotificationServiceStub : DataPackageTests.T2GServiceInterface.Notification.NotificationPortType
    {
        #region Fields
        private PIS.Ground.Core.T2G.WebServices.Notification.NotificationService _serviceImpl;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the service implementation in PIS-Ground Core module.
        /// </summary>
        public PIS.Ground.Core.T2G.WebServices.Notification.NotificationService Implementation
        {
            get
            {
                if (_serviceImpl == null)
                {
                    _serviceImpl = new PIS.Ground.Core.T2G.WebServices.Notification.NotificationService();
                }

                return _serviceImpl;
            }
        }

        #endregion


        #region NotificationPortType Members

        public DataPackageTests.T2GServiceInterface.Notification.onMessageNotificationOutput onMessageNotification(DataPackageTests.T2GServiceInterface.Notification.onMessageNotificationInput request)
        {
            var fieldValues = request.Body.fieldList.Select(f => f.AsImplementation).ToArray();
            Implementation.onMessageNotification(request.Body.systemId, request.Body.messageId,
               fieldValues , request.Body.timestamp, true, request.Body.inhibited, true);
            DataPackageTests.T2GServiceInterface.Notification.onMessageNotificationOutputBody body = new DataPackageTests.T2GServiceInterface.Notification.onMessageNotificationOutputBody();
            DataPackageTests.T2GServiceInterface.Notification.onMessageNotificationOutput result = new DataPackageTests.T2GServiceInterface.Notification.onMessageNotificationOutput(body);
            return result;
        }

        public DataPackageTests.T2GServiceInterface.Notification.onServiceNotificationOutput onServiceNotification(DataPackageTests.T2GServiceInterface.Notification.onServiceNotificationInput request)
        {
            Implementation.onServiceNotification(request.Body.systemId, request.Body.isSystemOnline, request.Body.subscriptionId, request.Body.serviceList.Select(s => s.AsImplementation).ToArray());
            DataPackageTests.T2GServiceInterface.Notification.onServiceNotificationOutputBody body = new DataPackageTests.T2GServiceInterface.Notification.onServiceNotificationOutputBody();
            DataPackageTests.T2GServiceInterface.Notification.onServiceNotificationOutput result = new DataPackageTests.T2GServiceInterface.Notification.onServiceNotificationOutput(body);
            return result;
        }

        public DataPackageTests.T2GServiceInterface.Notification.onFileTransferNotificationOutput onFileTransferNotification(DataPackageTests.T2GServiceInterface.Notification.onFileTransferNotificationInput request)
        {
            
            Implementation.onFileTransferNotification(request.Body.taskId, 
                (PIS.Ground.Core.T2G.WebServices.Notification.taskStateEnum)request.Body.taskState, 
                (PIS.Ground.Core.T2G.WebServices.Notification.taskPhaseEnum)request.Body.taskPhase,
                request.Body.activeFileTransferCount, request.Body.errorCount, request.Body.acquisitionCompletionPercent, request.Body.transferCompletionPercent,
                request.Body.distributionCompletionPercent, request.Body.priority, true,
                (PIS.Ground.Core.T2G.WebServices.Notification.linkTypeEnum)request.Body.linkType, true,
                request.Body.automaticallyStop, true,
                request.Body.startDate, true,
                request.Body.notificationURL,
                request.Body.completionDate, true,
                request.Body.waitingFileTransferCount, true,
                request.Body.completedFileTransferCount, true,
                (PIS.Ground.Core.T2G.WebServices.Notification.taskSubStateEnum)request.Body.taskSubState, true,
                request.Body.distributingFileTransferCount, true);
            DataPackageTests.T2GServiceInterface.Notification.onFileTransferNotificationOutputBody body = new DataPackageTests.T2GServiceInterface.Notification.onFileTransferNotificationOutputBody();
            DataPackageTests.T2GServiceInterface.Notification.onFileTransferNotificationOutput result = new DataPackageTests.T2GServiceInterface.Notification.onFileTransferNotificationOutput(body);
            return result;
        }

        public DataPackageTests.T2GServiceInterface.Notification.onFilePublicationNotificationOutput onFilePublicationNotification(DataPackageTests.T2GServiceInterface.Notification.onFilePublicationNotificationInput request)
        {
            Implementation.onFilePublicationNotification(request.Body.folderId, request.Body.completionPercent, (PIS.Ground.Core.T2G.WebServices.Notification.acquisitionStateEnum)request.Body.acquisitionState, request.Body.error);
            DataPackageTests.T2GServiceInterface.Notification.onFilePublicationNotificationOutputBody body = new DataPackageTests.T2GServiceInterface.Notification.onFilePublicationNotificationOutputBody();
            DataPackageTests.T2GServiceInterface.Notification.onFilePublicationNotificationOutput result = new DataPackageTests.T2GServiceInterface.Notification.onFilePublicationNotificationOutput(body);
            return result;
        }

        public void onFilesReceivedNotification(int folderId)
        {
            Implementation.onFilesReceivedNotification(folderId);
        }

        public DataPackageTests.T2GServiceInterface.Notification.onFilesPublishedNotificationOutput onFilesPublishedNotification(DataPackageTests.T2GServiceInterface.Notification.onFilesPublishedNotificationInput request)
        {
            Implementation.onFilesPublishedNotification(request.Body.folderId, request.Body.systemId);
            DataPackageTests.T2GServiceInterface.Notification.onFilesPublishedNotificationOutputBody body = new DataPackageTests.T2GServiceInterface.Notification.onFilesPublishedNotificationOutputBody();
            DataPackageTests.T2GServiceInterface.Notification.onFilesPublishedNotificationOutput result = new DataPackageTests.T2GServiceInterface.Notification.onFilesPublishedNotificationOutput(body);
            return result;
        }

        public DataPackageTests.T2GServiceInterface.Notification.onEventEnumNotificationOutput onEventEnumNotification(DataPackageTests.T2GServiceInterface.Notification.onEventEnumNotificationInput request)
        {
            throw new NotImplementedException();
/*            DataPackageTests.T2GServiceInterface.Notification.onEventEnumNotificationOutputBody body = new DataPackageTests.T2GServiceInterface.Notification.onEventEnumNotificationOutputBody();
            DataPackageTests.T2GServiceInterface.Notification.onEventEnumNotificationOutput result = new DataPackageTests.T2GServiceInterface.Notification.onEventEnumNotificationOutput(body);
            return result;*/
        }

        public DataPackageTests.T2GServiceInterface.Notification.onSystemChangedNotificationOutput onSystemChangedNotification(DataPackageTests.T2GServiceInterface.Notification.onSystemChangedNotificationInput request)
        {
            Implementation.onSystemChangedNotification(request.Body.system.AsImplementation);
            DataPackageTests.T2GServiceInterface.Notification.onSystemChangedNotificationOutputBody body = new DataPackageTests.T2GServiceInterface.Notification.onSystemChangedNotificationOutputBody();
            DataPackageTests.T2GServiceInterface.Notification.onSystemChangedNotificationOutput result = new DataPackageTests.T2GServiceInterface.Notification.onSystemChangedNotificationOutput(body);
            return result;
        }

        public DataPackageTests.T2GServiceInterface.Notification.onSystemDeletedNotificationOutput onSystemDeletedNotification(DataPackageTests.T2GServiceInterface.Notification.onSystemDeletedNotificationInput request)
        {
            Implementation.onSystemDeletedNotification(request.Body.systemId);
            DataPackageTests.T2GServiceInterface.Notification.onSystemDeletedNotificationOutputBody body = new DataPackageTests.T2GServiceInterface.Notification.onSystemDeletedNotificationOutputBody();
            DataPackageTests.T2GServiceInterface.Notification.onSystemDeletedNotificationOutput result = new DataPackageTests.T2GServiceInterface.Notification.onSystemDeletedNotificationOutput(body);
            return result;
        }

        #endregion
    }
}
