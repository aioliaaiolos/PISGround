///
namespace PIS.Ground.Core.T2G.WebServices.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.T2G.WebServices.Notification;

    /// <summary>
    /// 
    /// </summary>
    public class T2GNotificationClient : System.ServiceModel.ClientBase<Notification.NotificationPortType>, Notification.NotificationPortType
    {

        public T2GNotificationClient()
        {
        }

        public T2GNotificationClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public T2GNotificationClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public T2GNotificationClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public T2GNotificationClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Notification.onMessageNotificationOutput Notification.NotificationPortType.onMessageNotification(Notification.onMessageNotificationInput request)
        {
            return base.Channel.onMessageNotification(request);
        }

        public void onMessageNotification(string systemId, string messageId, Ground.Core.T2G.WebServices.Notification.fieldList fieldList)
        {
            //Ground.Core.T2G.WebServices.Notification.onMessageNotificationInput inValue = new Ground.Core.T2G.WebServices.Notification.onMessageNotificationInput();
            //inValue.Body = new Ground.Core.T2G.WebServices.Notification.onMessageNotificationInputBody();
            //inValue.Body.systemId = systemId;
            //inValue.Body.messageId = messageId;
            //inValue.Body.fieldList = fieldList;
            T2GClient objT2GConnector = new T2GClient();
            SystemInfo objSys = objT2GConnector.GetSystemData(systemId);
            if (objSys != null)
            {
                objSys.FieldList = new List<FieldInfo>();
                foreach (fieldStruct objfieldStruct in fieldList)
                {
                    FieldInfo objFieldInfo = new FieldInfo();
                    objFieldInfo.FieldId = objfieldStruct.id;
                    objFieldInfo.FieldValue = objfieldStruct.value;
                    switch (objfieldStruct.type)
                    {
                        case fieldTypeEnum.boolean: objFieldInfo.FieldType = TypeCode.Boolean;
                            break;
                        case fieldTypeEnum.@byte: objFieldInfo.FieldType = TypeCode.Byte;
                            break;
                        case fieldTypeEnum.dateTime: objFieldInfo.FieldType = TypeCode.DateTime;
                            break;
                        case fieldTypeEnum.@double: objFieldInfo.FieldType = TypeCode.Double;
                            break;
                        case fieldTypeEnum.@float: objFieldInfo.FieldType = TypeCode.Single;
                            break;
                        case fieldTypeEnum.@int: objFieldInfo.FieldType = TypeCode.Int32;
                            break;
                        case fieldTypeEnum.int64: objFieldInfo.FieldType = TypeCode.Int64;
                            break;
                        case fieldTypeEnum.@long: objFieldInfo.FieldType = TypeCode.Int64;
                            break;
                        case fieldTypeEnum.@short: objFieldInfo.FieldType = TypeCode.Int16;
                            break;
                        case fieldTypeEnum.@string: objFieldInfo.FieldType = TypeCode.String;
                            break;
                        case fieldTypeEnum.unknown: objFieldInfo.FieldType = TypeCode.String;
                            break;
                        case fieldTypeEnum.unsignedByte: objFieldInfo.FieldType = TypeCode.Byte;
                            break;
                        case fieldTypeEnum.unsignedInt: objFieldInfo.FieldType = TypeCode.UInt32;
                            break;
                        case fieldTypeEnum.unsignedShort: objFieldInfo.FieldType = TypeCode.UInt16;
                            break;
                        default: objFieldInfo.FieldType = TypeCode.String;
                            break;
                    }
                    objSys.FieldList.Add(objFieldInfo);
                }

                ElementEventArgs objElementEventArgs = objT2GConnector.BuildAvailabeElement(objSys);
                if (objElementEventArgs != null)
                {
                    objElementEventArgs.MessageId = messageId;
                    objElementEventArgs.SystemId = systemId;
                    //if (objT2GConnector.ElementChangeNotificationList.Contains(objElementEventArgs.ElementNumber))
                    //{
                    objT2GConnector.RaiseOnElementInfoChangeEvent(objElementEventArgs);
                    //}
                }
            }
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Ground.Core.T2G.WebServices.Notification.onServiceNotificationOutput Ground.Core.T2G.WebServices.Notification.NotificationPortType.onServiceNotification(Ground.Core.T2G.WebServices.Notification.onServiceNotificationInput request)
        {
            return base.Channel.onServiceNotification(request);
        }

        public void onServiceNotification(string systemId, bool isSystemOnline, int subscriptionId, Ground.Core.T2G.WebServices.Notification.serviceList serviceList)
        {
            Ground.Core.T2G.WebServices.Notification.onServiceNotificationInput inValue = new Ground.Core.T2G.WebServices.Notification.onServiceNotificationInput();
            inValue.Body = new Ground.Core.T2G.WebServices.Notification.onServiceNotificationInputBody();
            inValue.Body.systemId = systemId;
            inValue.Body.isSystemOnline = isSystemOnline;
            inValue.Body.subscriptionId = subscriptionId;
            inValue.Body.serviceList = serviceList;
            Ground.Core.T2G.WebServices.Notification.onServiceNotificationOutput retVal = ((Ground.Core.T2G.WebServices.Notification.NotificationPortType)(this)).onServiceNotification(inValue);

            T2GClient objT2GConnector = new T2GClient(); 
            foreach (Ground.Core.T2G.WebServices.Notification.serviceStruct objSer in serviceList)
            {
                ServiceInfo objServiceInfo = new ServiceInfo(objSer.serviceId, objSer.name, objSer.vehiclePhysicalId, objSer.operatorId, objSer.isAvailable, objSer.serviceIPAddress, objSer.AID, objSer.SID, objSer.servicePortNumber);
                /// check for update
                objT2GConnector.UpdateServiceList(objServiceInfo, systemId);
            }
        }

        public void onFileTransferNotification(int taskId, Ground.Core.T2G.WebServices.Notification.taskStateEnum taskState, Ground.Core.T2G.WebServices.Notification.taskPhaseEnum taskPhase, ushort activeFileTransferCount, ushort errorCount, sbyte acquisitionCompletionPercent, sbyte transferCompletionPercent, sbyte distributionCompletionPercent)
        {
            base.Channel.onFileTransferNotification(taskId, taskState, taskPhase, activeFileTransferCount, errorCount, acquisitionCompletionPercent, transferCompletionPercent, distributionCompletionPercent);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Ground.Core.T2G.WebServices.Notification.onFilePublicationNotificationOutput Ground.Core.T2G.WebServices.Notification.NotificationPortType.onFilePublicationNotification(Ground.Core.T2G.WebServices.Notification.onFilePublicationNotificationInput request)
        {
            return base.Channel.onFilePublicationNotification(request);
        }

        public void onFilePublicationNotification(int folderId, sbyte completionPercent, Ground.Core.T2G.WebServices.Notification.acquisitionStateEnum acquisitionState, string error)
        {
            Ground.Core.T2G.WebServices.Notification.onFilePublicationNotificationInput inValue = new Ground.Core.T2G.WebServices.Notification.onFilePublicationNotificationInput();
            inValue.Body = new Ground.Core.T2G.WebServices.Notification.onFilePublicationNotificationInputBody();
            inValue.Body.folderId = folderId;
            inValue.Body.completionPercent = completionPercent;
            inValue.Body.acquisitionState = acquisitionState;
            inValue.Body.error = error;
            Ground.Core.T2G.WebServices.Notification.onFilePublicationNotificationOutput retVal = ((Ground.Core.T2G.WebServices.Notification.NotificationPortType)(this)).onFilePublicationNotification(inValue);
        }

        public void onFilesReceivedNotification(int folderId)
        {
            base.Channel.onFilesReceivedNotification(folderId);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Ground.Core.T2G.WebServices.Notification.onFilesPublishedNotificationOutput Ground.Core.T2G.WebServices.Notification.NotificationPortType.onFilesPublishedNotification(Ground.Core.T2G.WebServices.Notification.onFilesPublishedNotificationInput request)
        {
            return base.Channel.onFilesPublishedNotification(request);
        }

        public void onFilesPublishedNotification(int folderId, string systemId)
        {
            Ground.Core.T2G.WebServices.Notification.onFilesPublishedNotificationInput inValue = new Ground.Core.T2G.WebServices.Notification.onFilesPublishedNotificationInput();
            inValue.Body = new Ground.Core.T2G.WebServices.Notification.onFilesPublishedNotificationInputBody();
            inValue.Body.folderId = folderId;
            inValue.Body.systemId = systemId;
            Ground.Core.T2G.WebServices.Notification.onFilesPublishedNotificationOutput retVal = ((Ground.Core.T2G.WebServices.Notification.NotificationPortType)(this)).onFilesPublishedNotification(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Ground.Core.T2G.WebServices.Notification.onEventEnumsNotificationOutput Ground.Core.T2G.WebServices.Notification.NotificationPortType.onEventEnumsNotification(Ground.Core.T2G.WebServices.Notification.onEventEnumsNotificationInput request)
        {
            return base.Channel.onEventEnumsNotification(request);
        }

        public void onEventEnumsNotification(int requestId, sbyte completionPercent, Ground.Core.T2G.WebServices.Notification.eventList eventList)
        {
            Ground.Core.T2G.WebServices.Notification.onEventEnumsNotificationInput inValue = new Ground.Core.T2G.WebServices.Notification.onEventEnumsNotificationInput();
            inValue.Body = new Ground.Core.T2G.WebServices.Notification.onEventEnumsNotificationInputBody();
            inValue.Body.requestId = requestId;
            inValue.Body.completionPercent = completionPercent;
            inValue.Body.eventList = eventList;
            PIS.Ground.Core.T2G.WebServices.Notification.onEventEnumsNotificationOutput retVal = ((Ground.Core.T2G.WebServices.Notification.NotificationPortType)(this)).onEventEnumsNotification(inValue);
        }
    }
}
