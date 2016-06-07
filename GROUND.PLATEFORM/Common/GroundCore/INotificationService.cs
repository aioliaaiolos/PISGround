///
namespace PIS.Ground.Core.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Text;
    using PIS.Ground.Core;
    using PIS.Ground.Core.Notification;

    /// <summary>
    /// 
    /// </summary>
    [ServiceContract(Name = "NotificationBinding", Namespace = "http://alstom.com/T2G/Notification")]
    public interface INotificationBinding
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract(Action = "http://alstom.com/T2G/Notification/onMessageNotification", ReplyAction = "*")]
        void onMessageNotification(string systemId, string messageId, fieldStruct[] fieldList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract(Action = "http://alstom.com/T2G/Notification/onServiceNotification", ReplyAction = "*")]
        void onServiceNotification(string systemId, bool isSystemOnline, int subscriptionId, serviceStruct[] serviceList);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskState"></param>
        /// <param name="taskPhase"></param>
        /// <param name="activeFileTransferCount"></param>
        /// <param name="errorCount"></param>
        /// <param name="acquisitionCompletionPercent"></param>
        /// <param name="transferCompletionPercent"></param>
        /// <param name="distributionCompletionPercent"></param>
        [OperationContract(Action = "http://alstom.com/T2G/Notification/onFileTransferNotification", ReplyAction = "*")]
        void onFileTransferNotification(int taskId, taskStateEnum taskState, taskPhaseEnum taskPhase, ushort activeFileTransferCount, ushort errorCount, sbyte acquisitionCompletionPercent, sbyte transferCompletionPercent, sbyte distributionCompletionPercent);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract(Action = "http://alstom.com/T2G/Notification/onFilePublicationNotification", ReplyAction = "*")]
        void onFilePublicationNotification(int folderId, sbyte completionPercent, acquisitionStateEnum acquisitionState, string error);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderId"></param>
        [OperationContract(Action = "http://alstom.com/T2G/Notification/onFilesReceivedNotification", ReplyAction = "*")]
        void onFilesReceivedNotification(int folderId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
         [OperationContract(Action = "http://alstom.com/T2G/Notification/onFilesPublishedNotification", ReplyAction = "*")]
        void onFilesPublishedNotification(int folderId, string systemId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
       [OperationContract(Action = "http://alstom.com/T2G/Notification/onEventEnumsNotification", ReplyAction = "*")]
        void onEventEnumsNotification(int requestId, sbyte completionPercent, eventStruct[] eventList);
    }
}
