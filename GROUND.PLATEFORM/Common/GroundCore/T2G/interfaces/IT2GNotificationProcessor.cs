//---------------------------------------------------------------------------------------------------
// <copyright file="IT2GNotificationProcessor.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.T2G
{
    using PIS.Ground.Core.T2G.WebServices.Notification;

    /// <summary>Interface for T2G notification processor.</summary>
    public interface IT2GNotificationProcessor
    {
        /// <summary>Raise OnMessageNotification.</summary>
        /// <param name="systemId">system id.</param>
        /// <param name="messageId">message id.</param>
        /// <param name="fieldList">field list.</param>
        void OnMessageNotification(string systemId, string messageId, fieldStruct[] fieldList);

        /// <summary>Raise onServiceNotification.</summary>
        /// <param name="systemId">system id.</param>
        /// <param name="isSystemOnline">is system online.</param>
        /// <param name="subscriptionId">service subscription id.</param>
        /// <param name="serviceList">list of services.</param>
        void OnServiceNotification(string systemId, bool isSystemOnline, int subscriptionId, serviceStruct[] serviceList);

        /// <summary>Raise onFileTransferNotification.</summary>
        /// <param name="taskId">task id.</param>
        /// <param name="taskState">task state.</param>
        /// <param name="taskPhase">task phase.</param>
        /// <param name="activeFileTransferCount">file transfer count.</param>
        /// <param name="errorCount">error count.</param>
        /// <param name="acquisitionCompletionPercent">acquisition complete percentage.</param>
        /// <param name="transferCompletionPercent">transfer complete percentage.</param>
        /// <param name="distributionCompletionPercent">distribute complete percentage.</param>
        void OnFileTransferNotification(
            int taskId,
            taskStateEnum taskState,
            taskPhaseEnum taskPhase,
            ushort activeFileTransferCount,
            ushort errorCount,
            sbyte acquisitionCompletionPercent,
            sbyte transferCompletionPercent,
            sbyte distributionCompletionPercent);

        /// <summary>Raise onFilePublicationNotification.</summary>
        /// <param name="folderId">folder id.</param>
        /// <param name="completionPercent">complete percentage.</param>
        /// <param name="acquisitionState">acqisition state.</param>
        /// <param name="error">error if any.</param>
        void OnFilePublicationNotification(int folderId, sbyte completionPercent, acquisitionStateEnum acquisitionState, string error);

        /// <summary>Raise onFilesReceivedNotification.</summary>
        /// <param name="folderId">folder id.</param>
        void OnFilesReceivedNotification(int folderId);

        /// <summary>Raise onFilesPublishedNotification.</summary>
        /// <param name="folderId">folder id.</param>
        /// <param name="systemId">system id.</param>
        void OnFilesPublishedNotification(int folderId, string systemId);

        /// <summary>Raise onSystemChangedNotification.</summary>
        /// <param name="system">changed system info.</param>
        void OnSystemChangedNotification(systemInfoStruct system);

        /// <summary>Raise OnSystemDeletedNotification.</summary>
        /// <param name="systemId">The onboard system identifier.</param>
        void OnSystemDeletedNotification(string systemId);
    }
}
