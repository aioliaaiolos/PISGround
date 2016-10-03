//---------------------------------------------------------------------------------------------------
// <copyright file="T2GFileTransferServiceStub.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using PIS.Ground.Core.Data;
using AcquisitionStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.acquisitionStateEnum;
using FileInfoStruct = DataPackageTests.T2GServiceInterface.FileTransfer.fileInfoStruct;
using FileList = DataPackageTests.T2GServiceInterface.FileTransfer.fileList;
using FilePathStruct = DataPackageTests.T2GServiceInterface.FileTransfer.filePathStruct;
using FileTransferPortType = DataPackageTests.T2GServiceInterface.FileTransfer.FileTransferPortType;
using FolderInfoStruct = DataPackageTests.T2GServiceInterface.FileTransfer.folderInfoStruct;
using FolderList = DataPackageTests.T2GServiceInterface.FileTransfer.folderList;
using NotificationClient = DataPackageTests.T2GServiceInterface.Notification.NotificationPortTypeClient;
using PathList = DataPackageTests.T2GServiceInterface.FileTransfer.pathList;
using RecipientList = DataPackageTests.T2GServiceInterface.FileTransfer.recipientList;
using RecipientStruct = DataPackageTests.T2GServiceInterface.FileTransfer.recipientStruct;
using TaskPhaseEnum = DataPackageTests.T2GServiceInterface.FileTransfer.taskPhaseEnum;
using TaskStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.taskStateEnum;
using TaskSubStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.taskSubStateEnum;
using TransferStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.transferStateEnum;
using TransferTaskList = DataPackageTests.T2GServiceInterface.FileTransfer.transferTaskList;
using TransferTaskStruct = DataPackageTests.T2GServiceInterface.FileTransfer.transferTaskStruct;
using System.Threading;

namespace DataPackageTests.ServicesStub
{
    #region Data Definition
    
    /// <summary>
    /// Define a file path in T2G.
    /// </summary>
    /// <seealso cref="DataPackageTests.T2GServiceInterface.FileTransfer.filePathStruct" />
    [DataContractAttribute(Name = "filePathStruct", Namespace = "http://alstom.com/T2G/FileTransfer")]
    public class FilePathInfo : FilePathStruct
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="FilePathInfo"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="size">The size.</param>
        /// <param name="checksum">The checksum.</param>
        public FilePathInfo(string path, long size, uint checksum)
        {
            this.checksum = checksum;
            this.path = path;
            this.size = size;
        }
    }

    /// <summary>
    /// Define a file in T2G.
    /// </summary>
    /// <seealso cref="DataPackageTests.T2GServiceInterface.FileTransfer.fileInfoStruct" />
    [DataContractAttribute(Name = "fileInfoStruct", Namespace = "http://alstom.com/T2G/FileTransfer")]
    public class FileInfoData : FileInfoStruct
    {
        public long AcquiredSize { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInfoData"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="size">The size.</param>
        /// <param name="expectedChecksum">The expected checksum.</param>
        public FileInfoData(string path, long size, uint expectedChecksum)
        {
            this.path = path;
            this.size = size;
            this.expectedChecksum = expectedChecksum;
            this.actualChecksum = 0;
            this.AcquiredSize = 0;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public FileInfoData Clone()
        {
            return (FileInfoData)MemberwiseClone();
        }
    }

    /// <summary>
    /// Describes a folder in T2G.
    /// </summary>
    /// <seealso cref="DataPackageTests.T2GServiceInterface.FileTransfer.folderInfoStruct" />
    [DataContractAttribute(Name = "folderInfoStruct", Namespace = "http://alstom.com/T2G/FileTransfer")]
    public class FolderInfoData : FolderInfoStruct
    {
        public static readonly DateTime NullDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public List<FileInfoData> Files { get; private set; }
        public FileList AsFileList
        {
            get
            {
                FileList list = new FileList();
                list.Capacity = Files.Count;
                list.AddRange(Files.Cast<FileInfoStruct>());
                return list;
            }
        }

        public FolderInfoData(int folderId, string name, string path, string creator)
        {
            this.folderId = folderId;

            this.path = path;

            this.name = name;
            this.systemId = "ground";
            this.folderType = T2GServiceInterface.FileTransfer.folderTypeEnum.upload;
            this.acquisitionState = T2GServiceInterface.FileTransfer.acquisitionStateEnum.notAcquired;
            this.acquisitionError = string.Empty;
            this.applicationId = string.Empty;
            this.fileCompression = false;
            this.creator = creator;
            this.creationDate = DateTime.UtcNow;
            this.acquisitionDate = NullDate;
            this.expirationDate = NullDate;
            this.totalFilesSize = 0;
            this.totalFilesCount = 0;
            this.currentFilesSize = 0;

            this.currentFilesCount = 0;
            this.publicationNotifURL = string.Empty;
            this.ftpServerIP = string.Empty;
            this.ftpPortNumber = 0;
            this.ftpDirectory = string.Empty;
            this.ftpUserName = string.Empty;
            this.ftpPassword = string.Empty;
            this.Files = new List<FileInfoData>(5);
        }


        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public FolderInfoData Clone()
        {
            FolderInfoData clonedObject = (FolderInfoData)MemberwiseClone();
            clonedObject.Files = new List<FileInfoData>(this.Files.Count);
            foreach (FileInfoData file in this.Files)
            {
                clonedObject.Files.Add(file.Clone());
            }

            return clonedObject;
        }
    }

    /// <summary>
    /// Describes a T2G Transfer recipient.
    /// </summary>
    /// <seealso cref="DataPackageTests.T2GServiceInterface.FileTransfer.recipientStruct" />
    [DataContractAttribute(Name = "recipientStruct", Namespace = "http://alstom.com/T2G/FileTransfer")]
    public class RecipientInfo : RecipientStruct
    {
        public static readonly DateTime NullDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Gets a value indicating whether this recipient is in waiting state.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is waiting; otherwise, <c>false</c>.
        /// </value>
        public bool IsWaiting
        {
            get
            {
                return transferState == TransferStateEnum.waitingForConnection ||
                    transferState == TransferStateEnum.waitingForLink ||
                    transferState == TransferStateEnum.waitingForSpace ||
                    transferState == TransferStateEnum.waitingInQueue;
            }
        }

        /// <summary>
        /// Gets the distribution progress.
        /// </summary>
        public sbyte DistributionProgress
        {
            get
            {
                sbyte progress = (sbyte)0;

                if (transferState == TransferStateEnum.transferCompleted && distributionDate != NullDate)
                {
                    progress = (sbyte)100;
                }
                else if (transferState != TransferStateEnum.transferError && completionPercent == 100)
                {
                    if (!string.IsNullOrEmpty(applicationIds) && !string.IsNullOrEmpty(distributedApplicationIds))
                    {
                        int toNotifyCount = applicationIds.Count(c => c == ',') + 1;
                        int notifiedCount = distributedApplicationIds.Count(c => c == ',') + 1;

                        progress = (notifiedCount < toNotifyCount) ? (sbyte)(notifiedCount*100 / toNotifyCount): (sbyte)100;
                    }
                }

                return progress;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipientInfo"/> class.
        /// </summary>
        /// <param name="taskId">The task identifier.</param>
        /// <param name="folderId">The folder identifier.</param>
        /// <param name="systemId">The system identifier.</param>
        /// <param name="applicationIds">The application ids.</param>
        public RecipientInfo(int taskId, int folderId, string systemId, string applicationIds)
        {
            this.recipientId = taskId + 1000;
            this.systemId = systemId;
            this.missionId = string.Empty;
            this.applicationIds = applicationIds;
            this.taskId = taskId;
            this.folderId = folderId;
            this.transferState = T2GServiceInterface.FileTransfer.transferStateEnum.notTransferring;
            this.error = string.Empty;
            this.transferredFilesCount = 0;
            this.transferredFilesSize = 0;
            this.completionPercent = 0;
            this.transferStartDate = NullDate;
            this.transferEndDate = NullDate;
            this.distributionDate = NullDate;
            this.distributedApplicationIds = distributedApplicationIds;
        }


        public RecipientInfo Clone()
        {
            return (RecipientInfo)MemberwiseClone();
        }
    }


    /// <summary>
    /// Describes a T2G transfer task.
    /// </summary>
    /// <seealso cref="DataPackageTests.T2GServiceInterface.FileTransfer.transferTaskStruct" />
    /// <remarks>Only one recipient is supported.</remarks>
    [DataContractAttribute(Name = "transferTaskStruct", Namespace = "http://alstom.com/T2G/FileTransfer")]
    public class TransferTaskInfo : TransferTaskStruct
    {
        public static readonly DateTime NullDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #region Fields

        /// <summary>
        /// Gets the transfer recipient.
        /// </summary>
        public RecipientInfo Recipient { get; private set; }

        /// <summary>
        /// Gets the source folder.
        /// </summary>
        public FolderInfoData SourceFolder { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is in final state.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in final state; otherwise, <c>false</c>.
        /// </value>
        public bool IsInFinalState
        {
            get
            {
                bool inFinalState = taskSubState != TaskSubStateEnum.subtaskInProgress &&
                    (taskState == TaskStateEnum.taskCancelled || taskState == TaskStateEnum.taskError || taskState == TaskStateEnum.taskCompleted);
                return inFinalState;
            }
        }

        /// <summary>
        /// Gets the expected baseline progress in history log database depending of the transfer status.
        /// </summary>
        public BaselineProgressStatusEnum BaselineProgress
        {
            get
            {
                BaselineProgressStatusEnum progress = BaselineProgressStatusEnum.UNKNOWN;
                switch (taskState)
                {
                    case T2GServiceInterface.FileTransfer.taskStateEnum.taskCancelled:
                    case T2GServiceInterface.FileTransfer.taskStateEnum.taskStopped:
                        progress = BaselineProgressStatusEnum.TRANSFER_PAUSED;
                        break;

                    case T2GServiceInterface.FileTransfer.taskStateEnum.taskError:
                        progress = BaselineProgressStatusEnum.UNKNOWN;
                        break;
                    case T2GServiceInterface.FileTransfer.taskStateEnum.taskCompleted:
                        progress = BaselineProgressStatusEnum.TRANSFER_COMPLETED;
                        break;
                    case T2GServiceInterface.FileTransfer.taskStateEnum.taskCreated:
                        progress = BaselineProgressStatusEnum.TRANSFER_PLANNED;
                        break;
                    case T2GServiceInterface.FileTransfer.taskStateEnum.taskStarted:
                        switch (taskPhase)
                        {
                            case T2GServiceInterface.FileTransfer.taskPhaseEnum.creationPhase:
                            case T2GServiceInterface.FileTransfer.taskPhaseEnum.acquisitionPhase:
                                progress = BaselineProgressStatusEnum.TRANSFER_PLANNED;
                                break;
                            case T2GServiceInterface.FileTransfer.taskPhaseEnum.distributionPhase:
                                progress = BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS;
                                break;
                            case T2GServiceInterface.FileTransfer.taskPhaseEnum.transferPhase:
                                progress = transferCompletionPercent != 0 ? BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS : BaselineProgressStatusEnum.TRANSFER_PLANNED;
                                break;
                        }
                        break;
                }

                return progress;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get
            {
                return taskState == TaskStateEnum.taskStarted || taskSubState == TaskSubStateEnum.subtaskInProgress || Recipient.IsWaiting || this.activeFileTransferCount != 0 || this.distributingFileTransferCount != 0;
            }
        }

        #endregion

        #region "Constructor"

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferTaskInfo"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="description">The description.</param>
        /// <param name="creator">The creator.</param>
        /// <param name="notificationUrl">The notification URL.</param>
        /// <param name="sourceFolder">The source folder.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="applicationIds">The application ids.</param>
        public TransferTaskInfo(int id, string description, string creator, string notificationUrl, FolderInfoData sourceFolder, string destination, string applicationIds)
        {
            this.taskId = id;
            this.taskSystemId = "ground";
            this.foreignTaskId = id;
            this.folderId = sourceFolder.folderId;
            this.folderSystemId = sourceFolder.systemId;
            SourceFolder = sourceFolder;
            this.description = description;
            this.priority = 15;
            this.linkType = T2GServiceInterface.FileTransfer.linkTypeEnum.highBandwidth;
            this.automaticallyStop = false;
            this.creator = creator;
            this.creationDate = DateTime.UtcNow;
            this.startDate = NullDate;
            this.expirationDate = NullDate;
            this.completionDate = NullDate;
            this.taskPhase = T2GServiceInterface.FileTransfer.taskPhaseEnum.creationPhase;
            this.taskState = T2GServiceInterface.FileTransfer.taskStateEnum.taskCreated;
            this.activeFileTransferCount = 0;
            this.errorCount = 0;
            this.acquisitionCompletionPercent = 0;
            this.transferCompletionPercent = 0;
            this.distributionCompletionPercent = 0;
            this.transferNotifURL = string.Empty;
            this.waitingFileTransferCount = 0;
            this.completedFileTransferCount = 0;
            this.taskSubState = T2GServiceInterface.FileTransfer.taskSubStateEnum.subtaskNone;
            this.distributionCompletionPercent = 0;

            Recipient = new RecipientInfo(this.taskId, this.folderId, destination, applicationIds);
        }

        #endregion

        /// <summary>
        /// Updates the progress of a file transfer task.
        /// </summary>
        /// <remarks>Inspired of the logic in T2G source code(TransferTaskInfoClass.mFullProgressUpdate).</remarks>
        public void UpdateProgress()
        {
            this.transferCompletionPercent = Recipient.completionPercent;
            this.distributionCompletionPercent = Recipient.DistributionProgress;

            activeFileTransferCount = (Recipient.transferState == TransferStateEnum.transferring) ? (ushort)1 : (ushort)0;
            waitingFileTransferCount = (Recipient.IsWaiting) ? (ushort)1 : (ushort)0;
            completedFileTransferCount = (Recipient.transferState == TransferStateEnum.transferCompleted && Recipient.distributionDate != NullDate) ? (ushort)1 : (ushort)0;
            distributingFileTransferCount = (Recipient.transferState == TransferStateEnum.transferCompleted && Recipient.distributionDate == NullDate && taskState == TaskStateEnum.taskStarted) ? (ushort)1 : (ushort)0;


            if (taskPhase == TaskPhaseEnum.creationPhase && acquisitionCompletionPercent != 0)
            {
                taskPhase = TaskPhaseEnum.acquisitionPhase;
            }

            if (taskPhase == TaskPhaseEnum.acquisitionPhase && acquisitionCompletionPercent == 100)
            {
                taskPhase = TaskPhaseEnum.transferPhase;
            }

            if (taskPhase == TaskPhaseEnum.transferPhase || taskPhase == TaskPhaseEnum.distributionPhase)
            {
                if (Recipient.completionPercent < 100 || Recipient.transferState == T2GServiceInterface.FileTransfer.transferStateEnum.transferError)
                    taskPhase = TaskPhaseEnum.transferPhase;
                else
                    taskPhase = TaskPhaseEnum.distributionPhase;
            }
            switch (taskPhase)
            {
                case TaskPhaseEnum.distributionPhase:
                case TaskPhaseEnum.transferPhase:
                    acquisitionCompletionPercent = 100;
                    break;
                case TaskPhaseEnum.creationPhase:
                    acquisitionCompletionPercent = 0;
                    break;
            }

            // Update the task subState;
            TaskSubStateEnum subState = TaskSubStateEnum.subtaskNone;
            switch (taskState)
            {
                case TaskStateEnum.taskCreated:
                case TaskStateEnum.taskCompleted:
                    // keep subTaskNone
                    break;
                case TaskStateEnum.taskStarted:
                    switch (taskPhase)
                    {
                        case TaskPhaseEnum.acquisitionPhase:
                            subState = TaskSubStateEnum.subtaskInProgress;
                            break;
                        case TaskPhaseEnum.distributionPhase:
                            if (distributingFileTransferCount != 0)
                            {
                                subState = TaskSubStateEnum.subtaskInProgress;
                            }
                            break;
                        case TaskPhaseEnum.transferPhase:
                            if (activeFileTransferCount != 0)
                            {
                                subState = TaskSubStateEnum.subtaskInProgress;
                            }
                            else if (waitingFileTransferCount != 0)
                            {
                                if (Recipient.transferState == TransferStateEnum.waitingInQueue)
                                {
                                    subState = TaskSubStateEnum.subtaskWaitingSchedule;
                                }
                                else if (Recipient.transferState == TransferStateEnum.waitingForLink)
                                {
                                    subState = TaskSubStateEnum.subtaskWaitingLink;
                                }
                                else if (Recipient.transferState == TransferStateEnum.waitingForSpace)
                                {
                                    subState = TaskSubStateEnum.subtaskWaitingSpace;
                                }
                                else if (Recipient.transferState == TransferStateEnum.waitingForConnection)
                                {
                                    subState = TaskSubStateEnum.subtaskWaitingComm;
                                }
                            }
                            break;
                    }
                    break;
                case TaskStateEnum.taskCancelled:
                case TaskStateEnum.taskStopped:
                    if (activeFileTransferCount != 0 || waitingFileTransferCount != 0 || distributingFileTransferCount != 0)
                    {
                        subState = TaskSubStateEnum.subtaskInProgress;
                    }
                    break;
                case TaskStateEnum.taskError:
                    if (taskPhase == TaskPhaseEnum.acquisitionPhase)
                    {
                        subState = TaskSubStateEnum.subtaskAcquisitionError;
                    }
                    else if (taskPhase == TaskPhaseEnum.distributionPhase && Recipient.transferState != TransferStateEnum.transferError)
                    {
                        subState = TaskSubStateEnum.subtaskNotificationError;
                    }
                    break;
            }

            taskSubState = subState;

            if (completedFileTransferCount != 0 || Recipient.transferState == TransferStateEnum.transferError || Recipient.transferState == TransferStateEnum.notificationError)
            {
                if (completionDate == NullDate)
                {
                    completionDate = DateTime.UtcNow;
                }
                if (taskState != TaskStateEnum.taskCancelled)
                {
                    if (completedFileTransferCount != 0)
                    {
                        taskState = TaskStateEnum.taskCompleted;
                    }
                    else
                    {
                        taskState = TaskStateEnum.taskError;
                    }
                }
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public TransferTaskInfo Clone()
        {
            TransferTaskInfo clonedObject = (TransferTaskInfo)MemberwiseClone();
            clonedObject.Recipient = Recipient.Clone();
            clonedObject.SourceFolder = SourceFolder.Clone();
            return clonedObject;
        }
    }

    #endregion

    /// <summary>
    /// Class that simulate T2G File-Transfer service
    /// </summary>
    /// <seealso cref="DataPackageTests.T2GServiceInterface.FileTransfer.FileTransferPortType"/>
    [ServiceBehaviorAttribute(InstanceContextMode = InstanceContextMode.Single, ConfigurationName = "DataPackageTests.T2GServiceInterface.FileTransfer.FileTransferPortType")]
    public class T2GFileTransferServiceStub : FileTransferPortType
    {
        #region Fields

        const int MaximumConcurrentTransfer = 2;
        T2GIdentificationServiceStub _identificationService;


        private string _ftpServerIP = "127.0.0.1";
        private string _ftpUserName = "T2G";
        private string _ftpPassword = "admin";
        private ushort _ftpPort = 21;

        object _lock = new object();
        List<FolderInfoData> _folders = new List<FolderInfoData>(100);
        List<TransferTaskInfo> _transfers = new List<TransferTaskInfo>(100);
        string _notificationSubscriberUrl = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the last created folder identifier.
        /// </summary>
        public int? LastCreatedFolder { get; set; }

        /// <summary>
        /// Gets or sets the last created transfer.
        /// </summary>
        public int? LastCreatedTransfer { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T2GFileTransferServiceStub"/> class.
        /// </summary>
        /// <param name="identificationService">The identification service.</param>
        /// <exception cref="ArgumentNullException">Privided identificationService is null.</exception>
        public T2GFileTransferServiceStub(T2GIdentificationServiceStub identificationService)
        {
            if (identificationService == null)
            {
                throw new ArgumentNullException("identificationService");
            }

            _identificationService = identificationService;
        }
        #endregion

        #region public functions

        /// <summary>
        /// Computes the progress percent.
        /// </summary>
        /// <param name="progress">The progress.</param>
        /// <param name="maximumProgress">The maximum progress.</param>
        /// <returns>The completion percent(0-100)</returns>
        public static sbyte ComputePercent(long progress, long maximumProgress)
        {
            long total = (maximumProgress == 0) ? 0 : (progress * 100) / maximumProgress;
            if (total > 100)
            {
                total = 100;
            }
            else if (total < 0)
            {
                total = 0;
            }

            return (sbyte)total;
        }

        /// <summary>
        /// Iterates on all transfer task and update the transfer progression.
        /// </summary>
        public void PerformTransferProgression()
        {
            Dictionary<int, TransferTaskInfo> _notificationList = new Dictionary<int, TransferTaskInfo>(10);

            lock (_lock)
            {
                List<TransferTaskInfo> _startedList = _transfers.Where(t => t != null && t.taskState == TaskStateEnum.taskStarted).ToList();

                // Update the task in distribution phase
                foreach (TransferTaskInfo t in _startedList.Where(t => t.taskPhase == TaskPhaseEnum.distributionPhase))
                {
                    if (t.expirationDate != TransferTaskInfo.NullDate && DateTime.UtcNow > t.expirationDate)
                    {
                        t.Recipient.error = "Expired";
                        t.Recipient.transferState = T2GServiceInterface.FileTransfer.transferStateEnum.notificationError;
                        t.taskState = T2GServiceInterface.FileTransfer.taskStateEnum.taskError;
                    }
                    else
                    {
                        t.Recipient.distributedApplicationIds = t.Recipient.applicationIds;
                        t.Recipient.distributionDate = DateTime.UtcNow;
                        t.distributionCompletionPercent = 100;
                        t.completionDate = DateTime.UtcNow;
                        t.taskState = TaskStateEnum.taskCompleted;
                    }

                    t.UpdateProgress();
                    _notificationList[t.taskId] = t.Clone();
                }

                // Update the waiting state for task in transfer phase
                foreach (TransferTaskInfo t in _startedList.Where(t => t.taskPhase == TaskPhaseEnum.transferPhase))
                {
                    if (t.expirationDate != TransferTaskInfo.NullDate && DateTime.UtcNow > t.expirationDate)
                    {
                        t.Recipient.error = "Expired";
                        t.Recipient.transferState = T2GServiceInterface.FileTransfer.transferStateEnum.transferError;
                        t.taskState = T2GServiceInterface.FileTransfer.taskStateEnum.taskError;
                        t.UpdateProgress();
                        _notificationList[t.taskId] = t.Clone();
                    }
                    else if (_identificationService.IsSystemOnline(t.Recipient.systemId))
                    {
                        if (t.Recipient.transferState != TransferStateEnum.waitingForSpace)
                        {
                            if (_identificationService.GetSystemLink(t.Recipient.systemId) != DataPackageTests.T2GServiceInterface.Identification.commLinkEnum.wifi)
                            {
                                if (t.Recipient.transferState != TransferStateEnum.waitingForLink)
                                {
                                    t.Recipient.transferState = TransferStateEnum.waitingForLink;
                                    t.UpdateProgress();
                                    _notificationList[t.taskId] = t.Clone();
                                }
                            }
                            else if (t.Recipient.transferState == TransferStateEnum.waitingForLink ||
                                t.Recipient.transferState == TransferStateEnum.waitingForConnection)
                            {
                                t.Recipient.transferState = TransferStateEnum.waitingInQueue;
                                t.UpdateProgress();
                                _notificationList[t.taskId] = t.Clone();
                            }
                        }
                    }
                    else if (t.Recipient.transferState != TransferStateEnum.waitingForConnection)
                    {
                        t.Recipient.transferState = TransferStateEnum.waitingForConnection;
                        t.UpdateProgress();
                        _notificationList[t.taskId] = t.Clone();
                    }
                }

                int transferCount = 0;
                
                foreach (TransferTaskInfo t in _startedList.Where(t => t.taskPhase == TaskPhaseEnum.transferPhase && t.Recipient.transferState == TransferStateEnum.transferring))
                {
                    transferCount++;
                    if (t.Recipient.transferStartDate == TransferTaskInfo.NullDate)
                    {
                        t.Recipient.transferStartDate = DateTime.UtcNow;
                    }

                    if (t.Recipient.transferredFilesCount < t.SourceFolder.totalFilesCount)
                    {
                        t.Recipient.transferredFilesSize += t.SourceFolder.Files[(int)t.Recipient.transferredFilesCount].size;
                        t.Recipient.transferredFilesCount += 1;
                    }

                    if (t.Recipient.transferredFilesCount >= t.SourceFolder.totalFilesCount)
                    {
                        t.Recipient.transferEndDate = DateTime.UtcNow;
                        t.Recipient.transferredFilesSize = t.SourceFolder.totalFilesSize;
                        t.Recipient.transferredFilesCount = t.SourceFolder.totalFilesCount;
                        t.Recipient.transferState = TransferStateEnum.transferCompleted;
                        t.Recipient.completionPercent = 100;
                        t.activeFileTransferCount = 0;
                        t.taskPhase = TaskPhaseEnum.distributionPhase;
                    }
                    else
                    {
                        t.Recipient.completionPercent = ComputePercent(t.Recipient.transferredFilesSize, t.SourceFolder.totalFilesSize);
                        t.activeFileTransferCount = 1;
                    }

                    t.UpdateProgress();
                    _notificationList[t.taskId] = t.Clone();
                }
                if (transferCount < MaximumConcurrentTransfer)
                {
                    foreach(TransferTaskInfo t in _startedList.Where(t => 
                                    t.taskPhase == TaskPhaseEnum.transferPhase && 
                                    t.Recipient.transferState == TransferStateEnum.waitingInQueue &&
                                    (t.startDate == TransferTaskInfo.NullDate || t.startDate <= DateTime.UtcNow)).OrderBy(t => t.priority).ThenBy(t => t.taskId).Take(MaximumConcurrentTransfer - transferCount))
                    {
                        if (t.Recipient.transferStartDate == TransferTaskInfo.NullDate)
                        {
                            t.Recipient.transferStartDate = DateTime.UtcNow;
                        }

                        t.Recipient.transferState = TransferStateEnum.transferring;

                        t.UpdateProgress();
                        _notificationList[t.taskId] = t.Clone();
                    }
                }

                // Update the task in acquisition phase
                foreach (TransferTaskInfo t in _startedList.Where(t => t.taskPhase == TaskPhaseEnum.acquisitionPhase))
                {
                    if (t.SourceFolder.acquisitionState != T2GServiceInterface.FileTransfer.acquisitionStateEnum.acquisitionError)
                    {
                        if ((t.SourceFolder.expirationDate != TransferTaskInfo.NullDate && DateTime.UtcNow > t.SourceFolder.expirationDate) ||
                            (t.expirationDate != TransferTaskInfo.NullDate && DateTime.UtcNow > t.expirationDate))
                        {
                            t.SourceFolder.acquisitionError = "Expired";
                            t.SourceFolder.acquisitionState = T2GServiceInterface.FileTransfer.acquisitionStateEnum.acquisitionError;
                            t.taskState = T2GServiceInterface.FileTransfer.taskStateEnum.taskError;
                            if (t.SourceFolder.acquisitionDate == TransferTaskInfo.NullDate)
                            {
                                t.SourceFolder.acquisitionDate = DateTime.UtcNow;
                            }
                        }
                        else
                        {
                            if (t.SourceFolder.currentFilesCount < t.SourceFolder.Files.Count)
                            {
                                int updatedIndex = (int)t.SourceFolder.currentFilesCount;
                                t.SourceFolder.currentFilesCount = t.SourceFolder.currentFilesCount + 1;
                                t.SourceFolder.Files[updatedIndex].AcquiredSize = t.SourceFolder.Files[updatedIndex].size;
                                t.SourceFolder.Files[updatedIndex].actualChecksum = t.SourceFolder.Files[updatedIndex].expectedChecksum;
                            }

                            t.SourceFolder.currentFilesSize = t.SourceFolder.Files.Select(f => f.AcquiredSize).Sum();
                            t.acquisitionCompletionPercent = ComputePercent(t.SourceFolder.currentFilesSize, t.SourceFolder.totalFilesSize);
                            if (t.acquisitionCompletionPercent == (sbyte)100)
                            {
                                t.SourceFolder.acquisitionState = AcquisitionStateEnum.acquisitionSuccess;
                                if (t.SourceFolder.acquisitionDate == TransferTaskInfo.NullDate)
                                {
                                    t.SourceFolder.acquisitionDate = DateTime.UtcNow;
                                }

                                t.taskPhase = TaskPhaseEnum.transferPhase;
                                t.Recipient.transferState = TransferStateEnum.waitingInQueue;
                            }
                        }
                    }

                    t.UpdateProgress();
                    _notificationList[t.taskId] = t.Clone();
                }
            }

            if (_notificationList.Count > 0)
            {
                foreach (TransferTaskInfo task in _notificationList.Values)
                {
                    NotifySubscribers(task);
                }

                // Wait one second to give a chance to background processing to be processed.
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Determines whether if specified task is running or not.
        /// </summary>
        /// <param name="taskId">The task identifier.</param>
        /// <returns>
        ///   <c>true</c> if the specified task is running; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTaskRunning(int taskId)
        {
            return GetTask(taskId).IsRunning;
        }

        public TransferTaskInfo GetTask(int taskId)
        {
            lock (_lock)
            {
                if (taskId > 0 && taskId <= _transfers.Count)
                {
                    TransferTaskInfo task =  _transfers[taskId - 1];
                    if (task != null)
                        return task.Clone();
                }
            }

            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Task {0} does not exist", taskId), "taskId");
        }

        public void SetTransferExpiration(int taskId, DateTime expirationDate)
        {
            lock (_lock)
            {
                if (taskId > 0 && taskId <= _transfers.Count)
                {
                    TransferTaskInfo task = _transfers[taskId - 1];
                    task.SourceFolder.expirationDate = expirationDate;
                    task.expirationDate = expirationDate;
                }
                else
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Task {0} does not exist", taskId), "taskId");
                }
            }

        }

        #endregion

        #region FileTransferPortType Members

        /// <summary>
        /// Helper to call create upload folder of T2G.
        /// </summary>
        /// <param name="sessionid">The sessionid.</param>
        /// <param name="name">The name.</param>
        /// <param name="expirationDate">The expiration date.</param>
        /// <param name="fileCompression">if set to <c>true</c> [file compression].</param>
        /// <param name="paths">The paths.</param>
        /// <returns>The folder identifier created/</returns>
        public int CreateUploadFolder(int sessionid, string name, DateTime expirationDate, bool fileCompression, params FilePathInfo[] paths)
        {
            PathList list = new PathList();
            list.Capacity = paths.Length;
            list.AddRange(paths);
            DataPackageTests.T2GServiceInterface.FileTransfer.createUploadFolderInputBody body = new DataPackageTests.T2GServiceInterface.FileTransfer.createUploadFolderInputBody(
                sessionid, name, expirationDate.ToUniversalTime(), list, fileCompression);
            DataPackageTests.T2GServiceInterface.FileTransfer.createUploadFolderInput request = new DataPackageTests.T2GServiceInterface.FileTransfer.createUploadFolderInput(body);

            DataPackageTests.T2GServiceInterface.FileTransfer.createUploadFolderOutput result = createUploadFolder(request);
            return result.Body.folderId;
        }

        /// <summary>
        /// Creates the upload folder.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The information on created folder.</returns>
        public DataPackageTests.T2GServiceInterface.FileTransfer.createUploadFolderOutput createUploadFolder(DataPackageTests.T2GServiceInterface.FileTransfer.createUploadFolderInput request)
        {
            if (!_identificationService.IsSessionValid(request.Body.sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }
            string creator = _identificationService.GetUserName(request.Body.sessionId);

            if (request.Body.fileCompression)
            {
                throw FaultExceptionFactory.CreateCompressionNotSupportedFault();
            }
            else if (request.Body.pathList == null ||
                    request.Body.pathList.Count == 0)
            {
                throw FaultExceptionFactory.CreateInvalidPathListFault();
            }


            FolderInfoData folderInfo;
            lock (_lock)
            {
                int folderId = _folders.Count + 1;
                string path = string.Format(CultureInfo.InvariantCulture, "upload\\{0}", folderId);
                folderInfo = new FolderInfoData(folderId, request.Body.name, path, creator);
                folderInfo.expirationDate = request.Body.expirationDate.ToUniversalTime();
                folderInfo.totalFilesCount = (uint)request.Body.pathList.Count;
                folderInfo.totalFilesSize = request.Body.pathList.Select(p => p.size).Sum();
                foreach (FilePathStruct p in request.Body.pathList)
                {
                    folderInfo.Files.Add(new FileInfoData(p.path, p.size, p.checksum));
                }

                _folders.Add(folderInfo);
                LastCreatedFolder = folderId;
            }

            DataPackageTests.T2GServiceInterface.FileTransfer.createUploadFolderOutputBody body = new DataPackageTests.T2GServiceInterface.FileTransfer.createUploadFolderOutputBody(folderInfo.folderId, _ftpServerIP, _ftpPort, _ftpUserName, _ftpPassword, folderInfo.path);
            DataPackageTests.T2GServiceInterface.FileTransfer.createUploadFolderOutput result = new DataPackageTests.T2GServiceInterface.FileTransfer.createUploadFolderOutput(body);
            return result;
        }

        public DataPackageTests.T2GServiceInterface.FileTransfer.createDownloadFolderOutput createDownloadFolder(DataPackageTests.T2GServiceInterface.FileTransfer.createDownloadFolderInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        /// <summary>
        /// Deletes the folder.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="folderId">The folder identifier.</param>
        public void deleteFolder(int sessionId, int folderId)
        {
            if (!_identificationService.IsSessionValid(sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            lock (_lock)
            {
                if (folderId <= _folders.Count && folderId > 0)
                {
                    _folders[folderId] = null;
                }
            }

            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        /// <summary>
        /// Gets the folder information.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="folderId">The folder identifier.</param>
        /// <param name="files">[out] The files.</param>
        /// <returns>The folder information structure</returns>
        public FolderInfoStruct GetFolderInfo(int sessionId, int folderId, out FileList files)
        {
            DataPackageTests.T2GServiceInterface.FileTransfer.getFolderInfoInputBody body = new DataPackageTests.T2GServiceInterface.FileTransfer.getFolderInfoInputBody(sessionId, folderId);
            DataPackageTests.T2GServiceInterface.FileTransfer.getFolderInfoInput request = new DataPackageTests.T2GServiceInterface.FileTransfer.getFolderInfoInput(body);

            DataPackageTests.T2GServiceInterface.FileTransfer.getFolderInfoOutput result = getFolderInfo(request);
            files = result.Body.fileList;
            return result.Body.folderInfo;
        }

        /// <summary>
        /// Gets the folder information.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><the folder information./returns>
        public DataPackageTests.T2GServiceInterface.FileTransfer.getFolderInfoOutput getFolderInfo(DataPackageTests.T2GServiceInterface.FileTransfer.getFolderInfoInput request)
        {
            if (!_identificationService.IsSessionValid(request.Body.sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            FolderInfoData folderInfo = null;

            lock (_lock)
            {
                if (request.Body.folderId > 0 && request.Body.folderId <= _folders.Count)
                {
                    folderInfo = _folders[request.Body.folderId - 1].Clone();
                }
            }

            if (folderInfo == null)
            {
                throw FaultExceptionFactory.CreateInvalidFolderIdFault();
            }

            DataPackageTests.T2GServiceInterface.FileTransfer.getFolderInfoOutputBody body = new DataPackageTests.T2GServiceInterface.FileTransfer.getFolderInfoOutputBody(folderInfo, folderInfo.AsFileList, null, _ftpServerIP, _ftpPort, _ftpUserName, _ftpPassword);
            DataPackageTests.T2GServiceInterface.FileTransfer.getFolderInfoOutput result = new DataPackageTests.T2GServiceInterface.FileTransfer.getFolderInfoOutput(body);
            return result;
        }

        public DataPackageTests.T2GServiceInterface.FileTransfer.publishFolderOutput publishFolder(DataPackageTests.T2GServiceInterface.FileTransfer.publishFolderInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        public DataPackageTests.T2GServiceInterface.FileTransfer.enumPublicFoldersOutput enumPublicFolders(DataPackageTests.T2GServiceInterface.FileTransfer.enumPublicFoldersInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        /// <summary>
        /// Implements the enumFolders function of T2G.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The folder list</returns>
        public DataPackageTests.T2GServiceInterface.FileTransfer.enumFoldersOutput enumFolders(DataPackageTests.T2GServiceInterface.FileTransfer.enumFoldersInput request)
        {
            if (!_identificationService.IsSessionValid(request.Body.sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            bool endOfEnum = true;
            FolderList list = new FolderList();

            lock (_lock)
            {
                list.Capacity = _folders.Count;
                list.AddRange(_folders.Where(f => f != null).Select(f => (FolderInfoStruct)f.Clone()));
                endOfEnum = (100 * (request.Body.enumPos + 1)) >= _folders.Count;

            }
            DataPackageTests.T2GServiceInterface.FileTransfer.enumFoldersOutputBody body = new DataPackageTests.T2GServiceInterface.FileTransfer.enumFoldersOutputBody(list, endOfEnum);
            DataPackageTests.T2GServiceInterface.FileTransfer.enumFoldersOutput result = new DataPackageTests.T2GServiceInterface.FileTransfer.enumFoldersOutput(body);

            return result;
        }

        /// <summary>
        /// Wrapper to call create transfer task function..
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="description">The description.</param>
        /// <param name="transferType">Type of the transfer.</param>
        /// <param name="folderSystemId">The folder system identifier.</param>
        /// <param name="folderId">The folder identifier.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="expirationDate">The expiration date.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="applicationIds">The application ids.</param>
        /// <returns></returns>
        public int CreateTransferTask(int sessionId, string description, DataPackageTests.T2GServiceInterface.FileTransfer.transferTypeEnum transferType, string folderSystemId, int folderId, System.DateTime startDate, System.DateTime expirationDate, string destination, string applicationIds)
        {
            DataPackageTests.T2GServiceInterface.FileTransfer.createTransferTaskInput inValue = new DataPackageTests.T2GServiceInterface.FileTransfer.createTransferTaskInput();
            inValue.Body = new DataPackageTests.T2GServiceInterface.FileTransfer.createTransferTaskInputBody();
            inValue.Body.sessionId = sessionId;
            inValue.Body.description = description;
            inValue.Body.transferType = transferType;
            inValue.Body.folderSystemId = folderSystemId;
            inValue.Body.folderId = folderId;
            inValue.Body.startDate = startDate;
            inValue.Body.expirationDate = expirationDate;

            DataPackageTests.T2GServiceInterface.FileTransfer.recipientIdList recipients = new DataPackageTests.T2GServiceInterface.FileTransfer.recipientIdList();
            recipients.Capacity = 1;
            DataPackageTests.T2GServiceInterface.FileTransfer.recipientIdStruct recipient = new DataPackageTests.T2GServiceInterface.FileTransfer.recipientIdStruct();
            recipient.applicationId = applicationIds;
            recipient.missionId = string.Empty;
            recipient.systemId = destination;
            recipients.Add(recipient);
            inValue.Body.recipientIdList = recipients;
            DataPackageTests.T2GServiceInterface.FileTransfer.createTransferTaskOutput retVal =createTransferTask(inValue);
            return retVal.Body.taskId;
        }

        /// <summary>
        /// Implements the createTransferTask function of T2G File-Transfer
        /// </summary>
        /// <param name="request">The request input.</param>
        /// <returns>The transfer task created.</returns>
        public DataPackageTests.T2GServiceInterface.FileTransfer.createTransferTaskOutput createTransferTask(DataPackageTests.T2GServiceInterface.FileTransfer.createTransferTaskInput request)
        {
            if (!_identificationService.IsSessionValid(request.Body.sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }
            else if (request.Body.recipientIdList == null ||
                request.Body.recipientIdList.Count != 1)
            {
                throw FaultExceptionFactory.CreateSupportTransferToOneRecipientOnlyFault();
            }
            else if (request.Body.transferType != T2GServiceInterface.FileTransfer.transferTypeEnum.groundToTrain)
            {
                throw FaultExceptionFactory.CreateSupportOnlyGroundToTrainTransferFault();
            }
            else if (!request.Body.folderSystemId.Equals("ground", StringComparison.OrdinalIgnoreCase))
            {
                throw FaultExceptionFactory.CreateSupportOnlyGroundToTrainTransferFault();
            }
            else if (!_identificationService.IsSystemExist(request.Body.recipientIdList[0].systemId))
            {
                throw FaultExceptionFactory.CreateInvalidSystemIdentifierFault();
            }
    
            string creator = _identificationService.GetUserName(request.Body.sessionId);


            int taskId;
            lock (_lock)
            {
                FolderInfoData folder = null;
                if (request.Body.folderId > 0 && request.Body.folderId <= _folders.Count)
                {
                    folder = _folders[request.Body.folderId - 1];
                }

                if (folder == null)
                {
                    throw FaultExceptionFactory.CreateInvalidFolderIdFault();
                }

                taskId = _transfers.Count+1;
                TransferTaskInfo task = new TransferTaskInfo(taskId, request.Body.description, creator, string.Empty, folder, request.Body.recipientIdList[0].systemId, request.Body.recipientIdList[0].applicationId);
                task.expirationDate = request.Body.expirationDate.ToUniversalTime();
                task.startDate = request.Body.startDate.ToUniversalTime();
                _transfers.Add(task);
                LastCreatedTransfer = taskId;
            }

            DataPackageTests.T2GServiceInterface.FileTransfer.createTransferTaskOutputBody body = new DataPackageTests.T2GServiceInterface.FileTransfer.createTransferTaskOutputBody(taskId);
            DataPackageTests.T2GServiceInterface.FileTransfer.createTransferTaskOutput result = new DataPackageTests.T2GServiceInterface.FileTransfer.createTransferTaskOutput(body);

            return result;
        }

        /// <summary>
        /// Implements the Starts the transfer.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="taskId">The task identifier.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="linkType">Type of the link. (ignored for now)</param>
        /// <param name="sendProgressNotification">if set to <c>true</c>, notification about progress will be send.</param>
        /// <param name="automaticallyStop">if set to <c>true</c>, automatic stop between each phase will be executed. (not supported).</param>
        public void startTransfer(int sessionId, int taskId, sbyte priority, DataPackageTests.T2GServiceInterface.FileTransfer.linkTypeEnum linkType, bool sendProgressNotification, bool automaticallyStop)
        {
            if (!_identificationService.IsSessionValid(sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }
            string transferNotifUrl = string.Empty;
            if (sendProgressNotification)
            {
                transferNotifUrl = _identificationService.GetNotificationUrl(sessionId);
                if (string.IsNullOrEmpty(transferNotifUrl))
                {
                    throw FaultExceptionFactory.CreateNoNotificationUrlFault();
                }
            }

            TransferTaskInfo clonedTask = null;
            lock (_lock)
            {
                TransferTaskInfo taskInfo = null;
                if (taskId > 0 && taskId <= _transfers.Count)
                {
                    taskInfo = _transfers[taskId - 1];
                }

                if (taskInfo == null)
                {
                    throw FaultExceptionFactory.CreateInvalidTaskIdentifierFault();
                }
                if (taskInfo.taskState != TaskStateEnum.taskCreated)
                {
                    throw FaultExceptionFactory.CreateCannotStartTransferFault();
                }

                if (taskInfo.SourceFolder.acquisitionState == AcquisitionStateEnum.acquisitionStopped || taskInfo.SourceFolder.acquisitionState == AcquisitionStateEnum.notAcquired)
                {
                    taskInfo.SourceFolder.acquisitionState = AcquisitionStateEnum.acquisitionStarted;
                }

                if (taskInfo.SourceFolder.acquisitionState != AcquisitionStateEnum.acquisitionSuccess)
                {
                    taskInfo.taskPhase = TaskPhaseEnum.acquisitionPhase;
                    taskInfo.Recipient.transferState = TransferStateEnum.notTransferring;
                }
                else
                {
                    taskInfo.taskPhase = TaskPhaseEnum.transferPhase;
                    taskInfo.Recipient.transferState = TransferStateEnum.waitingInQueue;
                }

                taskInfo.taskState = TaskStateEnum.taskStarted;
                taskInfo.priority = priority;
                taskInfo.transferNotifURL = transferNotifUrl;
                taskInfo.UpdateProgress();

                if (!string.IsNullOrEmpty(transferNotifUrl) || !string.IsNullOrEmpty(_notificationSubscriberUrl))
                {
                    clonedTask = taskInfo.Clone();
                }
            }

            if (clonedTask != null)
            {
                NotifySubscribers(clonedTask);
            }
        }

        public void stopTransfer(int sessionId, int taskId)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        public void resumeTransfer(int sessionId, int taskId)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        /// <summary>
        /// Implements the cancelTransfer method of T2G FileTransfer.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="taskId">The task identifier to cancel.</param>
        public void cancelTransfer(int sessionId, int taskId)
        {
            if (!_identificationService.IsSessionValid(sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            TransferTaskInfo clonedTask = null;
            lock (_lock)
            {
                TransferTaskInfo taskInfo = null;
                if (taskId > 0 && taskId <= _transfers.Count)
                {
                    taskInfo = _transfers[taskId - 1];
                }

                if (taskInfo == null)
                {
                    throw FaultExceptionFactory.CreateInvalidTaskIdentifierFault();
                }

                if (taskInfo.taskState == TaskStateEnum.taskStarted)
                {
                    if (taskInfo.SourceFolder.acquisitionState == AcquisitionStateEnum.acquisitionStarted && 
                        !_transfers.Any(t => t != null && t.taskId != taskInfo.taskId && t.taskState == TaskStateEnum.taskStarted &&
                        t.SourceFolder.folderId == taskInfo.SourceFolder.folderId))
                    {
                        taskInfo.SourceFolder.acquisitionState = AcquisitionStateEnum.acquisitionStopped;
                    }
                }

                taskInfo.taskState = TaskStateEnum.taskCancelled;
                taskInfo.taskSubState = TaskSubStateEnum.subtaskNone;
                if (taskInfo.Recipient.IsWaiting || taskInfo.Recipient.transferState == TransferStateEnum.transferring)
                {
                    taskInfo.Recipient.transferState = TransferStateEnum.notTransferring;
                }

                taskInfo.UpdateProgress();

                if (string.IsNullOrEmpty(taskInfo.transferNotifURL) || !string.IsNullOrEmpty(_notificationSubscriberUrl))
                {
                    clonedTask = taskInfo.Clone();
                }
            }

            if (clonedTask != null)
            {
                NotifySubscribers(clonedTask);
            }
        }

        /// <summary>
        /// Deletes the transfer task.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="taskId">The task identifier.</param>
        public void deleteTransferTask(int sessionId, int taskId)
        {
            if (!_identificationService.IsSessionValid(sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            lock (_lock)
            {
                TransferTaskInfo taskInfo = null;
                if (taskId > 0 && taskId <= _transfers.Count)
                {
                    taskInfo = _transfers[taskId - 1];
                }

                if (taskInfo == null)
                {
                    throw FaultExceptionFactory.CreateInvalidTaskIdentifierFault();
                }

                if (taskInfo.IsRunning || 
                    ((taskInfo.taskState == TaskStateEnum.taskCreated || taskInfo.taskState == TaskStateEnum.taskStopped)
                    && (taskInfo.expirationDate == TransferTaskInfo.NullDate || DateTime.UtcNow < taskInfo.expirationDate)))
                {
                    throw FaultExceptionFactory.CreateCannotDeleteActiveTransferFault();
                }

                _transfers[taskId - 1] = null;
            }
        }

        public DataPackageTests.T2GServiceInterface.FileTransfer.verifyTransferredFilesOutput verifyTransferredFiles(DataPackageTests.T2GServiceInterface.FileTransfer.verifyTransferredFilesInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        public void changeTransferPriority(int sessionId, int taskId, sbyte priority)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        /// <summary>
        /// Gets the transfer task.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="taskId">The task identifier.</param>
        /// <param name="recipient">[out]The recipient.</param>
        /// <returns>The transfer task</returns>
        public DataPackageTests.T2GServiceInterface.FileTransfer.transferTaskStruct GetTransferTask(int sessionId, int taskId, out DataPackageTests.T2GServiceInterface.FileTransfer.recipientStruct recipient)
        {
            DataPackageTests.T2GServiceInterface.FileTransfer.getTransferTaskInput inValue = new DataPackageTests.T2GServiceInterface.FileTransfer.getTransferTaskInput();
            inValue.Body = new DataPackageTests.T2GServiceInterface.FileTransfer.getTransferTaskInputBody();
            inValue.Body.sessionId = sessionId;
            inValue.Body.taskId = taskId;
            DataPackageTests.T2GServiceInterface.FileTransfer.getTransferTaskOutput retVal = getTransferTask(inValue);
            recipient = retVal.Body.recipientList[0];
            return retVal.Body.transferTask;
        }

        /// <summary>
        /// Gets the transfer task.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="taskId">The task identifier.</param>
        /// <param name="recipient">[out]The recipient.</param>
        /// <returns>The transfer task</returns>
        public DataPackageTests.T2GServiceInterface.FileTransfer.transferTaskStruct GetTransferTask(int sessionId, int taskId)
        {
            DataPackageTests.T2GServiceInterface.FileTransfer.recipientStruct recipient;
            return GetTransferTask(sessionId, taskId, out recipient);
        }

        /// <summary>
        /// Gets the transfer task.
        /// </summary>
        /// <param name="request">The request input.</param>
        /// <returns>The transfer task found.</returns>
        public DataPackageTests.T2GServiceInterface.FileTransfer.getTransferTaskOutput getTransferTask(DataPackageTests.T2GServiceInterface.FileTransfer.getTransferTaskInput request)
        {
            if (!_identificationService.IsSessionValid(request.Body.sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            int taskId = request.Body.taskId;
            TransferTaskInfo clonedTask;
            lock (_lock)
            {
                TransferTaskInfo taskInfo = null;
                if (taskId > 0 && taskId <= _transfers.Count)
                {
                    taskInfo = _transfers[taskId - 1];
                }

                if (taskInfo == null)
                {
                    throw FaultExceptionFactory.CreateInvalidTaskIdentifierFault();
                }

                clonedTask = taskInfo.Clone();
            }
            
            
            RecipientList recipients = new RecipientList();
            recipients.Add(clonedTask.Recipient);
            DataPackageTests.T2GServiceInterface.FileTransfer.getTransferTaskOutputBody body = new DataPackageTests.T2GServiceInterface.FileTransfer.getTransferTaskOutputBody(clonedTask, recipients);
            DataPackageTests.T2GServiceInterface.FileTransfer.getTransferTaskOutput result = new DataPackageTests.T2GServiceInterface.FileTransfer.getTransferTaskOutput(body);
            return result;
        }

        /// <summary>
        /// Implements the enumTransferTask method of T2G FileTransfer.
        /// </summary>
        /// <param name="request">The request input.</param>
        /// <returns>The list of transfer task found.</returns>
        public DataPackageTests.T2GServiceInterface.FileTransfer.enumTransferTaskOutput enumTransferTask(DataPackageTests.T2GServiceInterface.FileTransfer.enumTransferTaskInput request)
        {
            if (!_identificationService.IsSessionValid(request.Body.sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }
            const int TakeCount = 25;
            TransferTaskList foundTasks = new TransferTaskList();
            foundTasks.Capacity = TakeCount + 1;

            DateTime startDate = request.Body.startDate.ToUniversalTime();
            DateTime endDate = request.Body.endDate.ToUniversalTime();
            lock (_lock)
            {
                IEnumerable<TransferTaskInfo> iterator = _transfers.Where(t => t != null);
                if (startDate != TransferTaskInfo.NullDate)
                {
                    iterator = iterator.Where(t => t.completionDate == TransferTaskInfo.NullDate || startDate < t.completionDate);
                }

                if (endDate != TransferTaskInfo.NullDate)
                {
                    iterator = iterator.Where(t => endDate >= t.creationDate);
                }

                if (request.Body.enumPos != 0)
                {
                    iterator = iterator.Skip(TakeCount * request.Body.enumPos);
                }

                iterator = iterator.Take(TakeCount);
            }

            bool endOfEnum = foundTasks.Count <= TakeCount;
            if (!endOfEnum)
            {
                foundTasks.RemoveAt(TakeCount);
            }

            DataPackageTests.T2GServiceInterface.FileTransfer.enumTransferTaskOutputBody body = new DataPackageTests.T2GServiceInterface.FileTransfer.enumTransferTaskOutputBody(foundTasks, endOfEnum);
            DataPackageTests.T2GServiceInterface.FileTransfer.enumTransferTaskOutput result = new DataPackageTests.T2GServiceInterface.FileTransfer.enumTransferTaskOutput(body);

            return result;
        }

        /// <summary>
        /// Implements the subscribeToNotifications of T2G FileTransfer.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>The subscription id.</returns>
        /// <remarks>This implementation support only one subscriber.</remarks>
        public int subscribeToNotifications(int sessionId)
        {
            if (!_identificationService.IsSessionValid(sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            string notificationUrl = _identificationService.GetNotificationUrl(sessionId);
            if (string.IsNullOrEmpty(notificationUrl))
            {
                throw FaultExceptionFactory.CreateNoNotificationUrlFault();
            }

            _notificationSubscriberUrl = notificationUrl;

            return 1;
        }

        /// <summary>
        /// Implements the unsubscribeToNotifications of T2G FileTransfer.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <remarks>This implementation support only one subscriber.</remarks>
        public void unsubscribeToNotifications(int sessionId, int subscriptionId)
        {
            if (!_identificationService.IsSessionValid(sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }
            else if (subscriptionId == 1)
            {
                _notificationSubscriberUrl = string.Empty;
            }
        }

        #endregion

        #region Internal functions

        /// <summary>
        /// Notifies the subscribers of a task about progress update.
        /// </summary>
        /// <param name="task">The task that progress has been updated.</param>
        private void NotifySubscribers(TransferTaskInfo task)
        {
            string notificationUrl;
            lock (_lock)
            {
                notificationUrl = _notificationSubscriberUrl;
            }

            string lastNotificationUrl = string.Empty;
            foreach (string currentUrl in new string[] { notificationUrl, task.transferNotifURL })
            {
                if (string.IsNullOrEmpty(currentUrl) || string.Equals(lastNotificationUrl, currentUrl, StringComparison.OrdinalIgnoreCase) )
                {
                    continue;
                }

                lastNotificationUrl = currentUrl;
                

                EndpointAddress address = new EndpointAddress(currentUrl);
                using (NotificationClient client = new NotificationClient("NotificationClient", address))
                {
                    try
                    {
                        client.Open();
                        SendOneNotification(client, task);
                    }
                    finally
                    {
                        if (client.State == CommunicationState.Faulted)
                        {
                            client.Abort();
                        }
                    }
                }


            }
        }

        private static void SendOneNotification(NotificationClient client, TransferTaskInfo task)
        {
            client.onFileTransferNotification(
                /*int taskId */ task.taskId,
                /* taskStateEnum taskState */ (DataPackageTests.T2GServiceInterface.Notification.taskStateEnum)task.taskState,
                /*taskPhaseEnum taskPhase */ (DataPackageTests.T2GServiceInterface.Notification.taskPhaseEnum)task.taskPhase,
                /* ushort activeFileTransferCount */ task.activeFileTransferCount,
                /* ushort errorCount */ task.errorCount,
                /* sbyte acquisitionCompletionPercent */ task.acquisitionCompletionPercent,
                /* sbyte transferCompletionPercent */ task.transferCompletionPercent,
                /* sbyte distributionCompletionPercent */ task.distributionCompletionPercent,
                /* sbyte priority */ task.priority,
                /* linkTypeEnum linkType */ (DataPackageTests.T2GServiceInterface.Notification.linkTypeEnum)task.linkType,
                /*bool automaticallyStop */ task.automaticallyStop,
                /* System.DateTime startDate */ task.startDate,
                /* string notificationURL */ task.transferNotifURL,
                /* System.DateTime completionDate */ task.completionDate,
                /* ushort waitingFileTransferCount */ task.waitingFileTransferCount,
                /* ushort completedFileTransferCount */ task.completedFileTransferCount,
                /* taskSubStateEnum taskSubState */ (DataPackageTests.T2GServiceInterface.Notification.taskSubStateEnum)task.taskSubState,
                /* ushort distributingFileTransferCount*/ task.distributingFileTransferCount
            );
        }

        #endregion
    }
}
