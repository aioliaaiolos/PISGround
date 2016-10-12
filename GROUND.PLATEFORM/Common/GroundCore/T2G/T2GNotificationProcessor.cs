//---------------------------------------------------------------------------------------------------
// <copyright file="T2GNotificationProcessor.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.T2G
{
    using System;
    using System.Text;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.LogMgmt;
    using PIS.Ground.Core.T2G.WebServices.Notification;

    /// <summary>
    /// T2GNotificationProcessor class which process notifications
    /// received from T2G server.
    /// </summary>
    public class T2GNotificationProcessor : IT2GNotificationProcessor
    {
        /// <summary>The notifier target.</summary>
        private readonly IT2GNotifierTarget _notifierTarget;
        
        /// <summary>The local data storage.</summary>
        private readonly T2GLocalDataStorage _localDataStorage;
        
        /// <summary>Manager for file distribution.</summary>
        private readonly T2GFileDistributionManager _fileDistributionManager;

        /// <summary>Lock for thread safety.</summary>
        private readonly object _lock;

        /// <summary>Initializes a new instance of the T2GNotificationProcessor class.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="notifierTarget">The notifier target.</param>
        /// <param name="localDataStorage">The local data storage.</param>
        /// <param name="fileDistributionManager">Manager for file distribution(optional).</param>
        internal T2GNotificationProcessor(
            IT2GNotifierTarget notifierTarget,
            T2GLocalDataStorage localDataStorage,
            T2GFileDistributionManager fileDistributionManager)
        {
            if (notifierTarget == null)
            {
                throw new ArgumentNullException("notifierTarget");
            }

            if (localDataStorage == null)
            {
                throw new ArgumentNullException("localDataStorage");
            }

            _notifierTarget = notifierTarget;
            _localDataStorage = localDataStorage;
            _fileDistributionManager = fileDistributionManager;

            _lock = new object();
        }

        /// <summary>Process Message Notification.</summary>
        /// <param name="systemId">system id.</param>
        /// <param name="messageId">message id.</param>
        /// <param name="fieldList">field list.</param>
        public void OnMessageNotification(string systemId, string messageId, fieldStruct[] fieldList)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(systemId) && !string.IsNullOrEmpty(messageId) && fieldList != null && fieldList.Length != 0)
                {
                    if (messageId == T2GDataConverter.PisBaseline)
                    {
                        PisBaseline baseline = T2GDataConverter.BuildPisBaseLine(fieldList);

                        if (baseline != null)
                        {
                            _localDataStorage.OnMessageChanged(systemId, messageId, baseline);
                        }
                    }
                    else if (messageId == T2GDataConverter.PisVersion)
                    {
                        PisVersion version = T2GDataConverter.BuildPisVersion(fieldList);

                        if (version != null)
                        {
                            _localDataStorage.OnMessageChanged(systemId, messageId, version);
                        }
                    }
                    else if (messageId == T2GDataConverter.PisMission || messageId == T2GDataConverter.SivngMission)
                    {
                        PisMission mission = T2GDataConverter.BuildPisMission(fieldList);

                        if (mission != null)
                        {
                            _localDataStorage.OnMessageChanged(systemId, messageId, mission);
                        }
                    }

                    ElementEventArgs elementEventArgs = _localDataStorage.BuildElementInfoChangedEvent(systemId);

                    if (elementEventArgs != null)
                    {
                        _notifierTarget.RaiseOnElementInfoChangeEvent(elementEventArgs);
                    }
                }
            }
        }

        /// <summary>Process Service Notification.</summary>
        /// <param name="systemId">system id.</param>
        /// <param name="isSystemOnline">is system online.</param>
        /// <param name="subscriptionId">service subscription id.</param>
        /// <param name="serviceList">list of services.</param>
        public void OnServiceNotification(string systemId, bool isSystemOnline, int subscriptionId, serviceStruct[] serviceList)
        {
            lock (_lock)
            {
                // This notification is sent by T2G for a single subscription, i.e. for a specific service ID. The list is not the
                // complete list of services on a train, it is a list of services with the same service ID. T2G supports multiple
                // services with the same ID present on a train, but PIS ground uses only those services that can only have one
                // instance on a given train. 

                if (!string.IsNullOrEmpty(systemId))
                {
                    ServiceInfoList services = T2GDataConverter.BuildServiceList(serviceList);

                    if (services != null)
                    {
                        _localDataStorage.OnServiceChanged(systemId, isSystemOnline, subscriptionId, services);

                        ElementEventArgs elementEventArgs = _localDataStorage.BuildElementInfoChangedEvent(systemId);

                        if (elementEventArgs != null)
                        {
                            _notifierTarget.RaiseOnElementInfoChangeEvent(elementEventArgs);
                        }
                    }
                }
            }
        }

        /// <summary>Process File Transfer Notification.</summary>
        /// <param name="taskId">task id.</param>
        /// <param name="taskState">task state.</param>
        /// <param name="taskPhase">task phase.</param>
        /// <param name="activeFileTransferCount">file transfer count.</param>
        /// <param name="errorCount">error count.</param>
        /// <param name="acquisitionCompletionPercent">acquisition complete percentage.</param>
        /// <param name="transferCompletionPercent">transfer complete percentage.</param>
        /// <param name="distributionCompletionPercent">distribute complete percentage.</param>
        public void OnFileTransferNotification(
            int taskId,
            taskStateEnum taskState,
            taskPhaseEnum taskPhase,
            ushort activeFileTransferCount,
            ushort errorCount,
            sbyte acquisitionCompletionPercent,
            sbyte transferCompletionPercent,
            sbyte distributionCompletionPercent)
        {
			if (_fileDistributionManager != null)
			{
				lock (_lock)
				{
					FileDistributionStatusArgs fdsArgs = new FileDistributionStatusArgs();

					fdsArgs.TaskId = taskId;
					fdsArgs.RequestId = _fileDistributionManager.GetFileDistributionRequestIdByTaskId(taskId);
					fdsArgs.ActiveFileTransferCount = activeFileTransferCount;
					fdsArgs.AcquisitionCompletionPercent = acquisitionCompletionPercent;
					fdsArgs.TransferCompletionPercent = transferCompletionPercent;
					fdsArgs.ErrorCount = errorCount;
					fdsArgs.DistributionCompletionPercent = distributionCompletionPercent;
					fdsArgs.TaskStatus = T2GDataConverter.BuildTaskState(taskState);
					fdsArgs.CurrentTaskPhase = T2GDataConverter.BuildTaskPhase(taskPhase);

                    if (LogManager.IsTraceActive(TraceType.INFO))
                    {
                        StringBuilder msg = new StringBuilder(200);
                        msg.Append("OnFileTransferNotification(");
                        msg.Append("task=").Append(fdsArgs.TaskId);
                        msg.Append(",request=").Append(fdsArgs.RequestId);
                        msg.Append(",aftc=").Append(fdsArgs.ActiveFileTransferCount);
                        msg.Append(",acp=").Append(fdsArgs.AcquisitionCompletionPercent);
                        msg.Append(",dcp=").Append(fdsArgs.DistributionCompletionPercent);
                        msg.Append(",tcp=").Append(fdsArgs.TransferCompletionPercent);
                        msg.Append(",TaskStatus=").Append(fdsArgs.TaskStatus);
                        msg.Append(",CurrentTaskPhase=").Append(fdsArgs.CurrentTaskPhase);
                        msg.Append(")");

                        LogManager.WriteLog(TraceType.INFO, msg.ToString(), "PIS.Ground.Core.T2G.T2GNotificationProcessor.OnFileTransferNotification", null, EventIdEnum.GroundCore);

                    }

					_notifierTarget.RaiseOnFileDistributeNotificationEvent(fdsArgs, taskId);
				}
			}
        }

        /// <summary>Process File Publication Notification.</summary>
        /// <param name="folderId">folder id.</param>
        /// <param name="completionPercent">complete percentage.</param>
        /// <param name="acquisitionState">acqisition state.</param>
        /// <param name="error">error if any.</param>
        public void OnFilePublicationNotification(int folderId, sbyte completionPercent, acquisitionStateEnum acquisitionState, string error)
        {
			if (_fileDistributionManager != null)
			{
				lock (_lock)
				{
					FilePublicationNotificationArgs fpnArgs =
						_fileDistributionManager.BuildFilePublicationNotificationArgs(
							folderId,
							completionPercent,
							acquisitionState,
							error);

					if (fpnArgs != null)
					{
						_notifierTarget.RaiseOnFilePublicationNotificationEvent(fpnArgs);
					}
				}
			}
        }

        /// <summary>Process Files Received Notification.</summary>
        /// <param name="folderId">folder id.</param>
        public void OnFilesReceivedNotification(int folderId)
        {
            lock (_lock)
            {
                FileReceivedArgs frArgs = new FileReceivedArgs();
                frArgs.FolderId = folderId;
                frArgs.RequestId = new Guid();

                _notifierTarget.RaiseOnFileReceivedNotificationEvent(frArgs);
            }
        }

        /// <summary>Process Files Published Notification.</summary>
        /// <param name="folderId">folder id.</param>
        /// <param name="systemId">system id.</param>
        public void OnFilesPublishedNotification(int folderId, string systemId)
        {
			if (_fileDistributionManager != null)
			{
				lock (_lock)
				{
					FilePublishedNotificationArgs objFilePublishedArgs = _fileDistributionManager.BuildFilePublishedNotificationArgs(folderId, systemId);

					_notifierTarget.RaiseOnFilePublishedNotificationEvent(objFilePublishedArgs);
				}
			}
        }

        /// <summary>Process System Changed Notification.</summary>
        /// <param name="system">The system.</param>
        public void OnSystemChangedNotification(systemInfoStruct system)
        {
            lock (_lock)
            {
                if (system != null)
                {
                    SystemInfo newSystem = T2GDataConverter.BuildSystem(system);

                    if (newSystem != null)
                    {
                        _localDataStorage.OnSystemChanged(newSystem);

                        ElementEventArgs args = _localDataStorage.BuildElementInfoChangedEvent(newSystem.SystemId);

                        if (args != null)
                        {
                            _notifierTarget.RaiseOnElementInfoChangeEvent(args);
                        }
                    }
                }
            }
        }

        /// <summary>Process System Deleted Notification.</summary>
        /// <param name="systemId">The onboard system identifier.</param>
        public void OnSystemDeletedNotification(string systemId)
		{
            lock (_lock)
            {
                _localDataStorage.OnSystemDeleted(systemId);

                var lEvent = new SystemDeletedNotificationArgs();
                lEvent.SystemId = systemId;

                _notifierTarget.RaiseOnSystemDeletedNotificationEvent(lEvent);
            }
		}
    }
}
