/// 
namespace PIS.Ground.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using PIS.Ground.Core.LogMgmt;
    using System.Globalization;

    /// <summary>
    /// File distribution Request
    /// </summary>
    public abstract class FileDistributionRequest
    {
        /// <summary>
        /// A description of the file transfer task. 
        /// </summary>
        private string strDescription;

        /// <summary>
        /// Input request ID
        /// </summary>
        private Guid requestID;

        /// <summary>
        /// The date and time when the distribution of the specified files should start
        /// </summary>
        private DateTime startDate;

        /// <summary>
        /// The date and time when the distribution task will expire. 
        /// </summary>
        private DateTime expirationDate;

        /// <summary>
        /// If true, the files will be compressed before being transferred from train to the ground or vice-versa. false, otherwise
        /// </summary>
        private bool compression;

        /// <summary>
        /// The lists of targeted T2G systems and the list of T2G applications to be notified once the files are uploaded.
        /// </summary>
        private List<RecipientId> lstRecipient;

        /// <summary>
        /// File transfer mode
        /// </summary>
        private FileTransferMode transferMode;
        
        /// <summary>
        /// The priority of the transfer: a number between 0 and 32 (0 being the highest priority).
        /// </summary>
        private sbyte priority;

        /// <summary>
        /// task id 
        /// </summary>
        private int taskId;

        /// <summary>
        /// System ID
        /// </summary>
        private string systemId;

        /// <summary>
        /// Folder object that contains remote file. Calculate CRCGuid when files are added to it.
        /// </summary>
        private IRemoteFolderClass folder;

        private event EventHandler<FileDistributionStatusArgs> onFileDistributeNotification;

        private event EventHandler<FileDistributionTaskCreatedArgs> _onTaskCreatedNotification;

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public FileDistributionRequest()
        {
            this.lstRecipient = new List<RecipientId>();
            this.expirationDate = DateTime.Now;
            this.startDate = DateTime.Now;

            //create RemoteFolderClass
            this.folder = new RemoteFolderClass();

        }

        /// <summary>
        /// Create File Distribution request
        /// </summary>
        /// <param name="prequestID">Request id</param>
        /// <param name="pstrFolderName">The folder’s name to be uploaded/downloaded</param>
        /// <param name="pdtExpiration">The date and time when the distribution task will expire and the file will be deleted from the T2G product. </param>
        /// <param name="plstFilePathList">A list of relative file paths </param>
        /// <param name="pbCompression">If true, the files will be compressed before being transferred from train to the ground or vice-versa. false, otherwise</param>
        /// <param name="plstRecipient">The lists of targeted T2G systems and the list of T2G applications to be notified once the files are uploaded.</param>
        /// <param name="pstartDate">The date and time when the distribution of the specified files should start</param>
        /// <param name="pstrDescription">A description of the file transfer task.</param>
        /// <param name="ptransferMode">File transfer mode</param>
        /// <param name="ipriority">The priority of the transfer: a number between 0 and 32 (0 being the highest priority).</param>
        /// <param name="pOnFileDistributeNotification">Event handler for notification on file distribute</param>
        /// <param name="pOnTaskCreatedNotification">Event handler to be notified when transfer task is created.</param>
        public FileDistributionRequest(Guid prequestID, string pstrFolderName, DateTime pdtExpiration, List<string> plstFilePathList, bool pbCompression, List<RecipientId> plstRecipient, DateTime pstartDate, string pstrDescription, FileTransferMode ptransferMode, sbyte ipriority, EventHandler<FileDistributionStatusArgs> pOnFileDistributeNotification, EventHandler<FileDistributionTaskCreatedArgs> pOnTaskCreatedNotification)
        {
            this.requestID = prequestID;
            this.expirationDate = pdtExpiration;
            this.compression = pbCompression;
            this.lstRecipient = plstRecipient;
            this.startDate = pstartDate;
            this.strDescription = pstrDescription;
            this.transferMode = ptransferMode;
            this.priority = ipriority;
            this.OnFileDistributeNotification = pOnFileDistributeNotification;
            this._onTaskCreatedNotification = pOnTaskCreatedNotification;

            //create RemoteFolderClass
            this.folder = new RemoteFolderClass(pstrFolderName);

            //Fill the folder with files. This will automatically generate CRC for each file and CRCGuid for the folder
            foreach (string lFilePath in plstFilePathList)
            {
                //CRC Calculation is long. Developer can initialize file when required.
                this.folder.AddFileToFolder(new RemoteFileClass(lFilePath,false));
            }

            this.folder.ExpirationDate = pdtExpiration;
        }

        /// <summary>
        /// Create File Distribution request
        /// </summary>
        /// <param name="prequestID">Request id</param>
        /// <param name="pstrFolderName">The folder to be uploaded/downloaded</param>
        /// <param name="pdtExpiration">The date and time when the distribution task will expire and the file will be deleted from the T2G product. </param>
        /// <param name="pbCompression">If true, the files will be compressed before being transferred from train to the ground or vice-versa. false, otherwise</param>
        /// <param name="plstRecipient">The lists of targeted T2G systems and the list of T2G applications to be notified once the files are uploaded.</param>
        /// <param name="pstartDate">The date and time when the distribution of the specified files should start</param>
        /// <param name="pstrDescription">A description of the file transfer task.</param>
        /// <param name="ptransferMode">File transfer mode</param>
        /// <param name="ipriority">The priority of the transfer: a number between 0 and 32 (0 being the highest priority).</param>
        /// <param name="pOnFileDistributeNotification">Event handler for notification on file distribute</param>
        /// <param name="pOnTaskCreatedNotification">Event handler to be notified when transfer task is created.</param>
        public FileDistributionRequest(Guid prequestID, IRemoteFolderClass pfolder, DateTime pdtExpiration, bool pbCompression, List<RecipientId> plstRecipient, DateTime pstartDate, string pstrDescription, FileTransferMode ptransferMode, sbyte ipriority, EventHandler<FileDistributionStatusArgs> pOnFileDistributeNotification, EventHandler<FileDistributionTaskCreatedArgs> pOnTaskCreatedNotification)
        {
            this.requestID = prequestID;
            this.expirationDate = pdtExpiration;
            this.compression = pbCompression;
            this.lstRecipient = plstRecipient;
            this.startDate = pstartDate;
            this.strDescription = pstrDescription;
            this.transferMode = ptransferMode;
            this.priority = ipriority;
            this.OnFileDistributeNotification = pOnFileDistributeNotification;
            this._onTaskCreatedNotification = pOnTaskCreatedNotification;

            //assign RemoteFolderClass
            this.folder = pfolder;

            this.folder.ExpirationDate = pdtExpiration;
        }

        /// <summary>
        /// Create Download Folder Request
        /// </summary>
        /// <param name="prequestID">Request id</param>
        /// <param name="pstrFolderName">The folder’s name to be uploaded/downloaded</param>
        /// <param name="pdtExpiration">The date and time when the distribution task will expire and the file will be deleted from the T2G product. </param>
        /// <param name="pDownloadFolderPath">Download folder path</param>
        /// <param name="plstRecipient">The lists of targeted T2G systems and the list of T2G applications to be notified once the files are uploaded.</param>
        /// <param name="pstartDate">The date and time when the distribution of the specified files should start</param>
        /// <param name="pstrDescription">A description of the file transfer task.</param>
        /// <param name="ptransferMode">File transfer mode</param>
        /// <param name="ipriority">The priority of the transfer: a number between 0 and 32 (0 being the highest priority).</param>
        /// <param name="pOnFileDistributeNotification">File Transfer notification event handler</param>
        /// <param name="pfolderId">folder id to be downloaded</param>
        /// <param name="pOnFileDistributeNotification">Event handler for notification on file distribute</param>
        /// <param name="pOnTaskCreatedNotification">Event handler to be notified when transfer task is created.</param>
        public FileDistributionRequest(Guid prequestID, DateTime pdtExpiration, List<RecipientId> plstRecipient, DateTime pstartDate, string pstrDescription, FileTransferMode ptransferMode, sbyte ipriority, EventHandler<FileDistributionStatusArgs> pOnFileDistributeNotification, int pfolderId, EventHandler<FileDistributionTaskCreatedArgs> pOnTaskCreatedNotification)
        {
            this.requestID = prequestID;
            this.expirationDate = pdtExpiration;
            //this.compression = pbCompression;
            this.lstRecipient = plstRecipient;
            this.startDate = pstartDate;
            this.strDescription = pstrDescription;
            this.priority = ipriority;
            this.onFileDistributeNotification = pOnFileDistributeNotification;
            this._onTaskCreatedNotification = pOnTaskCreatedNotification;
            this.transferMode = ptransferMode;

            //create RemoteFolderClass
            this.folder = new RemoteFolderClass();
            this.folder.FolderId = pfolderId;
            this.folder.ExpirationDate = pdtExpiration;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Error List
        /// </summary>
        internal EventHandler<FileDistributionStatusArgs> OnFileDistributeNotification
        {
            get
            {
                return this.onFileDistributeNotification;
            }

            set
            {
                this.onFileDistributeNotification = value;
            }
        }

        /// <summary>
        /// System id
        /// </summary>
        public string SystemId
        {
            get
            {
                return this.systemId;
            }

            set
            {
                this.systemId = value;
            }
        }
 
        /// <summary>
        /// Gets or sets the  date and time when the distribution task will expire and the file will be deleted from the T2G product. 
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return this.startDate;
            }

            set
            {
                this.startDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the  description of the file transfer task.
        /// </summary>
        public string Description
        {
            get
            {
                return this.strDescription;
            }

            set
            {
                this.strDescription = value;
            }
        }

        /// <summary>
        /// Gets or sets the  priority of the transfer: a number between 0 and 32 (0 being the highest priority).
        /// </summary>
        public sbyte Priority
        {
            get
            {
                return this.priority;
            }

            set
            {
                this.priority = value;
            }
        }

        /// <summary>
        /// Gets or sets the File transfer mode
        /// </summary>
        public FileTransferMode FileTransferMode
        {
            get
            {
                return this.transferMode;
            }

            set
            {
                this.transferMode = value;
            }
        }

        /// <summary>
        /// Gets the lists of targeted T2G systems and the list of T2G applications to be notified once the files are uploaded. 
        /// </summary>
        public List<RecipientId> RecipientList
        {
            get
            {
                return this.lstRecipient;
            }
        }

        /// <summary>
        /// Gets or sets If true, the files will be compressed before being transferred from train to the ground or vice-versa. false, otherwise
        /// </summary>
        public bool Compression
        {
            get
            {
                return this.compression;
            }

            set
            {
                this.compression = value;
            }
        }

        /// <summary>
        /// Gets or sets the  date and time when the distribution task will expire and the file will be deleted from the T2G product. 
        /// </summary>
        public DateTime ExpirationDate
        {
            get
            {
                return this.expirationDate;
            }

            set
            {
                this.expirationDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the Input request id
        /// </summary>
        public Guid RequestId
        {
            get
            {
                return this.requestID;
            }

            set
            {
                this.requestID = value;
            }
        }

        /// <summary>
        /// Gets or sets the task Id
        /// </summary>
        public int TaskId
        {
            get
            {
                return this.taskId;
            }

            set
            {
                if (this.taskId != value)
                {
                    this.taskId = value;

                    if (value != 0 && _onTaskCreatedNotification != null)
                    {
                        try
                        {
                            _onTaskCreatedNotification(this, new FileDistributionTaskCreatedArgs(value, RequestId, RecipientList));
                        }
                        catch (System.Exception ex)
                        {
                            LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Properties.Resources.FailedToNotifyDistributionTransferTaskCreated, RequestId), "PIS.Ground.Core.Data.FileDistributionRequest.TaskId", ex, EventIdEnum.GroundCore);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// folder
        /// </summary>
        public IRemoteFolderClass Folder
        {
            get
            {
                return this.folder;
            }
        }
       
        #endregion
    }

}
