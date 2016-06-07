using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIS.Ground.Core.Data
{
    public class DownloadFolderRequest : FileDistributionRequest
    {
        ///// <summary>
        ///// A description of the file transfer task. 
        ///// </summary>
        private IFolderInfo folderinfo;

        ///// <summary>
        ///// Input request ID
        ///// </summary>
        //private Guid requestID;

        ///// <summary>
        ///// The date and time when the distribution of the specified files should start
        ///// </summary>
        //private DateTime startDate;

        ///// <summary>
        ///// The date and time when the distribution task will expire and the file will be deleted from the T2G product. 
        ///// </summary>
        //private DateTime expirationDate;

        ///// <summary>
        ///// The priority of the transfer: a number between 0 and 32 (0 being the highest priority).
        ///// </summary>
        //private sbyte priority;

        /// <summary>
        /// download Folder Path
        /// </summary>
        private string downloadFolderPath;

        /////// <summary>
        /////// If true, the files will be compressed before being transferred from train to the ground or vice-versa. false, otherwise
        /////// </summary>
        ////private bool compression;

        ///// <summary>
        ///// The lists of targeted T2G systems and the list of T2G applications to be notified once the files are downloaded.
        ///// </summary>
        //private List<Recipient> lstRecipient;

        ///// <summary>
        ///// Holds the status of each file like upload/ download
        ///// </summary>
        //private Dictionary<string, FtpStatus> dicFileOperationStatus;

        ///// <summary>
        ///// Holds the error for the files
        ///// </summary>
        //private List<string> lstError;

        //private event EventHandler<FileDistributionStatusArgs> onFileDistributeNotification;

        //private int folderId;

        ///// <summary>
        ///// task id 
        ///// </summary>
        //private int taskId;

        ///// <summary>
        ///// SystemID
        ///// </summary>
        //private string systemId;

        ///// <summary>
        ///// File transfer mode
        ///// </summary>
        //private FileTransferMode transferMode;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DownloadFolderRequest()
            : base()
        {
            ////this.lstFilePathList = new List<string>();
            //this.lstRecipient = new List<Recipient>();
            //this.expirationDate = DateTime.Now;
            //this.startDate = DateTime.Now;
            //this.dicFileOperationStatus = new Dictionary<string, FtpStatus>();
            //lstError = new List<string>();
            ////onFileDistributeNotification = new EventHandler<FileDistributionStatusArgs>();
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
        public DownloadFolderRequest(Guid prequestID, DateTime pdtExpiration, string pDownloadFolderPath, List<RecipientId> plstRecipient, DateTime pstartDate, string pstrDescription,FileTransferMode ptransferMode, sbyte ipriority, EventHandler<FileDistributionStatusArgs> pOnFileDistributeNotification,int pfolderId)
            : base(prequestID, pdtExpiration, plstRecipient, pstartDate, pstrDescription, ptransferMode, ipriority, pOnFileDistributeNotification,pfolderId)
        {
            //this.requestID = prequestID;
            //this.expirationDate = pdtExpiration;
            this.downloadFolderPath = pDownloadFolderPath;
            ////this.compression = pbCompression;
            //this.lstRecipient = plstRecipient;
            //this.startDate = pstartDate;
            //this.strDescription = pstrDescription;
            //this.priority = ipriority;
            //this.onFileDistributeNotification = pOnFileDistributeNotification;
            //lstError = new List<string>();
            //this.dicFileOperationStatus = new Dictionary<string, FtpStatus>();
            //folderId = pfolderId;
            //this.transferMode = ptransferMode;
        }

        #region Properties
        ///// <summary>
        ///// Error List
        ///// </summary>
        //internal EventHandler<FileDistributionStatusArgs> OnFileDistributeNotification
        //{
        //    get
        //    {
        //        return this.onFileDistributeNotification;
        //    }

        //    set
        //    {
        //        this.onFileDistributeNotification = value;
        //    }
        //}

        /// <summary>
        /// FolderInfo
        /// </summary>
        internal IFolderInfo Folderinfo
        {
            get
            {
                return this.folderinfo;
            }

            set
            {
                this.folderinfo = value;
            }

        }

        ///// <summary>
        ///// System id
        ///// </summary>
        //public string SystemId
        //{
        //    get
        //    {
        //        return this.systemId;
        //    }

        //    set
        //    {
        //        this.systemId = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the  date and time when the distribution task will expire and the file will be deleted from the T2G product. 
        ///// </summary>
        //public DateTime StartDate
        //{
        //    get
        //    {
        //        return this.startDate;
        //    }

        //    set
        //    {
        //        this.startDate = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the  description of the file transfer task.
        ///// </summary>
        //public string Description
        //{
        //    get
        //    {
        //        return this.strDescription;
        //    }

        //    set
        //    {
        //        this.strDescription = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the  priority of the transfer: a number between 0 and 32 (0 being the highest priority).
        ///// </summary>
        //public sbyte Priority
        //{
        //    get
        //    {
        //        return this.priority;
        //    }

        //    set
        //    {
        //        this.priority = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the File transfer mode
        ///// </summary>
        //public FileTransferMode FileTransferMode
        //{
        //    get
        //    {
        //        return this.transferMode;
        //    }

        //    set
        //    {
        //        this.transferMode = value;
        //    }
        //}

        ///// <summary>
        ///// Gets the lists of targeted T2G systems and the list of T2G applications to be notified once the files are uploaded. 
        ///// </summary>
        //public List<Recipient> RecipientList
        //{
        //    get
        //    {
        //        return this.lstRecipient;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets If true, the files will be compressed before being transferred from train to the ground or vice-versa. false, otherwise
        ///// </summary>
        ////public bool Compression
        ////{
        ////    get
        ////    {
        ////        return this.compression;
        ////    }

        ////    set
        ////    {
        ////        this.compression = value;
        ////    }
        ////}

        ///// <summary>
        ///// Gets or sets the  date and time when the distribution task will expire and the file will be deleted from the T2G product. 
        ///// </summary>
        //public DateTime ExpirationDate
        //{
        //    get
        //    {
        //        return this.expirationDate;
        //    }

        //    set
        //    {
        //        this.expirationDate = value;
        //    }
        //}

        /////// <summary>
        /////// Gets or sets the  list of relative file paths 
        /////// </summary>
        ////public List<string> FilePathList
        ////{
        ////    get
        ////    {
        ////        return this.lstFilePathList;
        ////    }
        ////}

        ///// <summary>
        ///// Gets or sets the Input request id
        ///// </summary>
        //public Guid RequestId
        //{
        //    get
        //    {
        //        return this.requestID;
        //    }

        //    set
        //    {
        //        this.requestID = value;
        //    }
        //}

        /// <summary>
        /// Gets or sets the  folder’s name to be uploaded/downloaded
        /// </summary>
        public string DownloadFolderPath
        {
            get
            {
                return this.downloadFolderPath;
            }

            set
            {
                this.downloadFolderPath = value;
            }
        }

        ///// <summary>
        ///// Gets or sets the  FileOperationStatus of the files
        ///// </summary>
        //internal Dictionary<string, FtpStatus> FileOperationStatus
        //{
        //    get
        //    {
        //        return this.dicFileOperationStatus;
        //    }

        //    set
        //    {
        //        this.dicFileOperationStatus = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the task Id
        ///// </summary>
        //internal int TaskId
        //{
        //    get
        //    {
        //        return this.taskId;
        //    }

        //    set
        //    {
        //        this.taskId = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the Folder Id
        ///// </summary>
        //internal int FolderId
        //{
        //    get
        //    {
        //        return this.folderId;
        //    }

        //    set
        //    {
        //        this.folderId = value;
        //    }
        //}
        #endregion

    }
}
