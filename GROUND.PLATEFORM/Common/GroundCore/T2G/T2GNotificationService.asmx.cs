//---------------------------------------------------------------------------------------------------
// <copyright file="T2GNotificationService.asmx.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.T2G.WebServices.Notification
{
    using System;
    using System.ComponentModel;
    using System.Web.Services;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.T2G;
    using PIS.Ground.Core.Properties;
    using System.Threading;
    using PIS.Ground.Core.Utility;

    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class NotificationService : PIS.Ground.Core.T2G.WebServices.Notification.INotificationBinding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        public NotificationService()
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = (ServiceConfiguration.RunningServiceName??String.Empty) + ".NotificationService";
            }
        }

        /// <summary>Service OnMessageNotification.</summary>
        /// <param name="systemId">system id.</param>
        /// <param name="messageId">message id.</param>
        /// <param name="fieldList">field list.</param>
        /// <param name="timestamp">Date/Time of the timestamp.</param>
        /// <param name="timestampSpecified">The timestamp specified.</param>
        /// <param name="inhibited">true if inhibited.</param>
        /// <param name="inhibitedSpecified">The inhibited specified.</param>
        public void onMessageNotification(string systemId, string messageId, fieldStruct[] fieldList, System.DateTime timestamp, bool timestampSpecified, bool inhibited, bool inhibitedSpecified)
        {            
            Action <string, string, fieldStruct[]> invoker = onMessageNotificationAsync;
            invoker.BeginInvoke(systemId, messageId, fieldList, invoker.EndInvoke, null);            
        }

        /// <summary>Executes the message notification asynchronous action.</summary>
        /// <param name="systemId">system id.</param>
        /// <param name="messageId">message id.</param>
        /// <param name="fieldList">field list.</param>
        private void onMessageNotificationAsync(string systemId, string messageId, fieldStruct[] fieldList)
        {
			try
			{
				if (!string.IsNullOrEmpty(systemId) && !string.IsNullOrEmpty(messageId) && fieldList != null)
				{
					T2GManagerContainer.T2GManager.T2GNotifier.OnMessageNotification(systemId, messageId, fieldList);
				}
				else
				{
					LogMgmt.LogManager.WriteLog(TraceType.ERROR, Resources.LogNotificationServiceOnMessageParameterNotValid, "PIS.Ground.Core.T2G.onMessageNotification", null, EventIdEnum.GroundCore);
				}
			}
			catch (System.Exception exception)
			{
				LogMgmt.LogManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.Core.T2G.onMessageNotificationAsync", exception, EventIdEnum.GroundCore);
			}
        }

        /// <summary>Service onFileTransferNotification.</summary>
        /// <param name="systemId">system id.</param>
        /// <param name="isSystemOnline">true if this object is system online.</param>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        /// <param name="serviceList">Service list.</param>
        public void onServiceNotification(string systemId, bool isSystemOnline, int subscriptionId, serviceStruct[] serviceList)
        {
            Action<string, bool, int, serviceStruct[]> invoker = onServiceNotificationAsync;
            invoker.BeginInvoke(systemId, isSystemOnline, subscriptionId, serviceList, invoker.EndInvoke, null);
        }

        /// <summary>Executes the service notification asynchronous action.</summary>
        /// <param name="systemId">system id.</param>
        /// <param name="isSystemOnline">true if this object is system online.</param>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        /// <param name="serviceList">Service list.</param>
        private void onServiceNotificationAsync(string systemId, bool isSystemOnline, int subscriptionId, serviceStruct[] serviceList)
        {
			try
			{
				if (!string.IsNullOrEmpty(systemId) && subscriptionId > 0 && serviceList != null)
				{
					T2GManagerContainer.T2GManager.T2GNotifier.OnServiceNotification(systemId, isSystemOnline, subscriptionId, serviceList);
				}
				else
				{
					LogMgmt.LogManager.WriteLog(TraceType.ERROR, Resources.LogNotificationServiceOnServiceParameterNotValid, "PIS.Ground.Core.T2G.onServiceNotification", null, EventIdEnum.GroundCore);
				}
			}
			catch (System.Exception exception)
			{
				LogMgmt.LogManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.Core.T2G.onServiceNotificationAsync", exception, EventIdEnum.GroundCore);
			}
        }

        /// <summary>Service onFileTransferNotification.</summary>
        /// <param name="taskId">task id.</param>
        /// <param name="taskState">task state.</param>
        /// <param name="taskPhase">task phase.</param>
        /// <param name="activeFileTransferCount">file transfer count.</param>
        /// <param name="errorCount">error count.</param>
        /// <param name="acquisitionCompletionPercent">acquisition complete percentage.</param>
        /// <param name="transferCompletionPercent">transfer complete percentage.</param>
        /// <param name="distributionCompletionPercent">distribute complete percentage.</param>
        /// <param name="priority">distribute complete percentage.</param>
        /// <param name="prioritySpecified">true if priority specified.</param>
        /// <param name="linkType">Type of the link.</param>
        /// <param name="linkTypeSpecified">true if link type specified.</param>
        /// <param name="automaticallyStop">true to automatically stop.</param>
        /// <param name="automaticallyStopSpecified">true if automatically stop specified.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="startDateSpecified">true if start date specified.</param>
        /// <param name="notificationURL">URL of the notification.</param>
        /// <param name="completionDate">Date of the completion.</param>
        /// <param name="completionDateSpecified">true if completion date specified.</param>
        /// <param name="waitingFileTransferCount">Number of waiting file transfers.</param>
        /// <param name="waitingFileTransferCountSpecified">true if waiting file transfer count specified.</param>
        /// <param name="completedFileTransferCount">Number of completed file transfers.</param>
        /// <param name="completedFileTransferCountSpecified">true if completed file transfer count
        /// specified.</param>
        /// <param name="taskSubState">State of the task sub.</param>
        /// <param name="taskSubStateSpecified">true if task sub state specified.</param>
        /// <param name="distributingFileTransferCount">Number of distributing file transfers.</param>
        /// <param name="distributingFileTransferCountSpecified">true if distributing file transfer count
        /// specified.</param>
        public void onFileTransferNotification(
                    int taskId,
                    taskStateEnum taskState,
                    taskPhaseEnum taskPhase,
                    ushort activeFileTransferCount,
                    ushort errorCount,
                    sbyte acquisitionCompletionPercent,
                    sbyte transferCompletionPercent,
                    sbyte distributionCompletionPercent,
                    sbyte priority,
                    bool prioritySpecified,
                    linkTypeEnum linkType,
                    bool linkTypeSpecified,
                    bool automaticallyStop,
                    bool automaticallyStopSpecified,
                    System.DateTime startDate,
                    bool startDateSpecified,
                    string notificationURL,
                    System.DateTime completionDate,
                    bool completionDateSpecified,
                    ushort waitingFileTransferCount,
                    bool waitingFileTransferCountSpecified,
                    ushort completedFileTransferCount,
                    bool completedFileTransferCountSpecified,
                    taskSubStateEnum taskSubState,
                    bool taskSubStateSpecified,
                    ushort distributingFileTransferCount,
                    bool distributingFileTransferCountSpecified)
        {
            Action<onFileTransferNotificationArgs> invoker = onFileTransferNotificationAsync;

            string msg = Environment.NewLine +
                " task=" + taskId.ToString() + Environment.NewLine +
                " ts=" + taskState.ToString() + Environment.NewLine +
                " tp=" + taskPhase.ToString() + Environment.NewLine +
                " url=" + notificationURL.ToString() + Environment.NewLine +
                " aftc=" + activeFileTransferCount.ToString() + Environment.NewLine +
                " ec=" + errorCount.ToString() + Environment.NewLine +
                " acp=" + acquisitionCompletionPercent.ToString() + Environment.NewLine +
                " tcp=" + transferCompletionPercent.ToString() + Environment.NewLine +
                " dcp=" + distributionCompletionPercent.ToString() + Environment.NewLine +
                " p=" + priority.ToString() + Environment.NewLine +
                " ps=" + prioritySpecified.ToString() + Environment.NewLine +
                " lt=" + linkType.ToString() + Environment.NewLine +
                " lts=" + linkTypeSpecified.ToString() + Environment.NewLine;

            invoker.BeginInvoke(
                new onFileTransferNotificationArgs
                {
                    taskId = taskId,
                    taskState = taskState,
                    taskPhase = taskPhase,
                    activeFileTransferCount = activeFileTransferCount,
                    errorCount = errorCount,
                    acquisitionCompletionPercent = acquisitionCompletionPercent,
                    transferCompletionPercent = transferCompletionPercent,
                    distributionCompletionPercent = distributionCompletionPercent
                },
                invoker.EndInvoke,
                null);
        }

        /// <summary>Arguments for on file transfer notification.</summary>
        private struct onFileTransferNotificationArgs
        {
            public int taskId;
            public taskStateEnum taskState;
            public taskPhaseEnum taskPhase;
            public ushort activeFileTransferCount;
            public ushort errorCount;
            public sbyte acquisitionCompletionPercent;
            public sbyte transferCompletionPercent;
            public sbyte distributionCompletionPercent;
        }

        /// <summary>Executes the file transfer notification asynchronous action.</summary>
        /// <param name="args">The arguments.</param>
        private void onFileTransferNotificationAsync(onFileTransferNotificationArgs args)
        {
			try
			{
				if (args.taskId > 0)
				{
					T2GManagerContainer.T2GManager.T2GNotifier.OnFileTransferNotification(
						args.taskId,
						args.taskState,
						args.taskPhase,
						args.activeFileTransferCount,
						args.errorCount,
						args.acquisitionCompletionPercent,
						args.transferCompletionPercent,
						args.distributionCompletionPercent);
				}
				else
				{
					LogMgmt.LogManager.WriteLog(TraceType.ERROR, Resources.LogonFileTransferNotificationParameterNotValid, "PIS.Ground.Core.T2G.onFileTransferNotification", null, EventIdEnum.GroundCore);
				}
			}
			catch (System.Exception exception)
			{
				LogMgmt.LogManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.Core.T2G.onFileTransferNotificationAsync", exception, EventIdEnum.GroundCore);
			}
        }

        /// <summary>service onFilePublicationNotification.</summary>
        /// <param name="folderId">folder id.</param>
        /// <param name="completionPercent">complete percentage.</param>
        /// <param name="acquisitionState">acqisition state.</param>
        /// <param name="error">error if any.</param>
        public void onFilePublicationNotification(int folderId, sbyte completionPercent, acquisitionStateEnum acquisitionState, string error)
        {
            Action<int, sbyte, acquisitionStateEnum, string> invoker = onFilePublicationNotificationAsync;
            invoker.BeginInvoke(folderId, completionPercent, acquisitionState, error, invoker.EndInvoke, null);
        }

        /// <summary>Executes the file publication notification asynchronous action.</summary>
        /// <param name="folderId">folder id.</param>
        /// <param name="completionPercent">complete percentage.</param>
        /// <param name="acquisitionState">acqisition state.</param>
        /// <param name="error">error if any.</param>
        private void onFilePublicationNotificationAsync(int folderId, sbyte completionPercent, acquisitionStateEnum acquisitionState, string error)
        {
			try
			{
				if (folderId > 0)
				{
					T2GManagerContainer.T2GManager.T2GNotifier.OnFilePublicationNotification(folderId, completionPercent, acquisitionState, error);
				}
				else
				{
					LogMgmt.LogManager.WriteLog(TraceType.ERROR, Resources.LogonFilePublicationNotificationParameterNotValid, "PIS.Ground.Core.T2G.onFilePublicationNotification", null, EventIdEnum.GroundCore);
				}
			}
			catch (System.Exception exception)
			{
				LogMgmt.LogManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.Core.T2G.onFilePublicationNotificationAsync", exception, EventIdEnum.GroundCore);
			}
        }

        /// <summary>service onFilesReceivedNotification.</summary>
        /// <param name="folderId">folder id.</param>
        public void onFilesReceivedNotification(int folderId)
        {
            Action<int> invoker = onFilesReceivedNotificationAsync;
            invoker.BeginInvoke(folderId, invoker.EndInvoke, null);
        }

        /// <summary>Executes the files received notification asynchronous action.</summary>
        /// <param name="folderId">folder id.</param>
        private void onFilesReceivedNotificationAsync(int folderId)
        {
			try
			{
				if (folderId > 0)
				{
					T2GManagerContainer.T2GManager.T2GNotifier.OnFilesReceivedNotification(folderId);
				}
				else
				{
					LogMgmt.LogManager.WriteLog(TraceType.ERROR, Resources.LogonFilesReceivedNotificationParameterNotValid, "PIS.Ground.Core.T2G.onFilesReceivedNotification", null, EventIdEnum.GroundCore);
				}
			}
			catch (System.Exception exception)
			{
				LogMgmt.LogManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.Core.T2G.onFilesReceivedNotificationAsync", exception, EventIdEnum.GroundCore);
			}
        }

        /// <summary>service onFilesPublishedNotification.</summary>
        /// <param name="folderId">folder id.</param>
        /// <param name="systemId">system id.</param>
        public void onFilesPublishedNotification(int folderId, string systemId)
        {
            Action<int, string> invoker = onFilesPublishedNotificationAsync;
            invoker.BeginInvoke(folderId, systemId, invoker.EndInvoke, null);
        }

        /// <summary>Executes the files published notification asynchronous action.</summary>
        /// <param name="folderId">folder id.</param>
        /// <param name="systemId">system id.</param>
        public void onFilesPublishedNotificationAsync(int folderId, string systemId)
        {
			try
			{
				if (folderId > 0 && !string.IsNullOrEmpty(systemId))
				{
					T2GManagerContainer.T2GManager.T2GNotifier.OnFilesPublishedNotification(folderId, systemId);
				}
				else
				{
					LogMgmt.LogManager.WriteLog(TraceType.ERROR, Resources.LogonFilesPublishedNotificationParametrNotValid, "PIS.Ground.Core.T2G.onFilesPublishedNotification", null, EventIdEnum.GroundCore);
				}
			}
			catch (System.Exception exception)
			{
				LogMgmt.LogManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.Core.T2G.onFilesPublishedNotificationAsync", exception, EventIdEnum.GroundCore);
			}
        }

        /// <summary>service onEventEnumsNotification.</summary>
        /// <param name="requestId">rewuest id.</param>
        /// <param name="completionPercent">completion percentage.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="eventList">event list.</param>
        public void onEventEnumNotification(int requestId, sbyte completionPercent, ushort errorCode, eventStruct[] eventList)
        {
            // Not used
        }
        
        /// <summary>
        /// service OnSystemChangedNotification
        /// </summary>
        /// <param name="system">system descriptor</param>
        public void onSystemChangedNotification(systemInfoStruct system)
        {
            if (system != null)
            {
                Action<systemInfoStruct> invoker = onSystemChangedNotificationAsync;
                invoker.BeginInvoke(system, invoker.EndInvoke, null);
            }
        }

        /// <summary>Executes the system changed notification asynchronous action.</summary>
        /// <param name="system">system descriptor.</param>
        private void onSystemChangedNotificationAsync(systemInfoStruct system)
        {
			try
			{
				T2GManagerContainer.T2GManager.T2GNotifier.OnSystemChangedNotification(system);
			}
			catch (System.Exception exception)
			{
				LogMgmt.LogManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.Core.T2G.onSystemChangedNotificationAsync", exception, EventIdEnum.GroundCore);
			}
        }

		/// <summary>
		/// service OnSystemDeletedNotification
		/// </summary>
		/// <param name="system">system descriptor</param>
		public void onSystemDeletedNotification(string systemId)
		{
            if (!String.IsNullOrEmpty(systemId))
            {
                Action<string> invoker = onSystemDeletedNotificationAsync;
                invoker.BeginInvoke(systemId, invoker.EndInvoke, null);
            }
		}

        /// <summary>Executes the system deleted notification asynchronous action.</summary>
        /// <param name="systemId">system id.</param>
        private void onSystemDeletedNotificationAsync(string systemId)
        {
			try
			{
	            T2GManagerContainer.T2GManager.T2GNotifier.OnSystemDeletedNotification(systemId);
			}
			catch (System.Exception exception)
			{
				LogMgmt.LogManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.Core.T2G.onSystemDeletedNotificationAsync", exception, EventIdEnum.GroundCore);
			}
        }
    }    
}
