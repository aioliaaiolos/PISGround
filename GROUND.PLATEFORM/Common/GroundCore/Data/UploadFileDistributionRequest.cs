using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIS.Ground.Core.Data
{
    /// <summary>
    /// Representinf Upload file distribution request.
    /// </summary>
    public class UploadFileDistributionRequest : FileDistributionRequest
    {
        /// <summary>
        /// The IP address of the FTP server hosting the files to be downloaded.
        /// </summary>
        private string ftpServerIP;

        /// <summary>
        /// The FTP server port number.
        /// </summary>
        private ushort ftpPortNumber;

        /// <summary>
        /// The login that should be used to log on to the FTP server.
        /// </summary>
        private string ftpUserName;

        /// <summary>
        /// The password that should be used to log on to the FTP server.
        /// </summary>
        private string ftpPassword;

        /// <summary>
        /// The FTP directory of the files to be downloaded. The path specified must be relative to the FTP user’s root path. If this parameter is empty, the FTP user’s root directory is used (“.”).
        /// </summary>
        private string ftpDirectory;

        /// <summary>
        /// Keep the Request ID which had uploaded the folder with the same CRCGuid as the one in the current request.
        /// </summary>
        private Guid UploadingRequestId;

        /// <summary>
        /// Holds the associated request if this request is an uploading one (has actually uploaded its folder)
        /// </summary>
        private List<Guid> associatedRequestId;

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public UploadFileDistributionRequest()
            : base()
        {
            this.associatedRequestId = new List<Guid>();
        }

        /// <summary>
        /// Create File Distribution request
        /// </summary>
        /// <param name="prequestID">Request id</param>
        /// <param name="pstrFolderName">The folder’s name to be uploaded</param>
        /// <param name="pdtExpiration">The date and time when the distribution task will expire and the file will be deleted from the T2G product. </param>
        /// <param name="plstFilePathList">A list of relative file paths </param>
        /// <param name="pbCompression">If true, the files will be compressed before being transferred from train to the ground or vice-versa. false, otherwise</param>
        /// <param name="plstRecipient">The lists of targeted T2G systems and the list of T2G applications to be notified once the files are uploaded.</param>
        /// <param name="pstartDate">The date and time when the distribution of the specified files should start</param>
        /// <param name="pstrDescription">A description of the file transfer task.</param>
        /// <param name="ptransferMode">File transfer mode</param>
        /// <param name="ipriority">The priority of the transfer: a number between 0 and 32 (0 being the highest priority).</param>
        /// <param name="pOnFileDistributeNotification">Event handler to be notified on file distribution progress.</param>
        /// <param name="pOnTaskCreatedNotification">Event handler to be notified when transfer task is created.</param>
        public UploadFileDistributionRequest(Guid prequestID, string pstrFolderName, DateTime pdtExpiration, List<string> plstFilePathList, bool pbCompression, List<RecipientId> plstRecipient, DateTime pstartDate, string pstrDescription, FileTransferMode ptransferMode, sbyte ipriority, EventHandler<FileDistributionStatusArgs> pOnFileDistributeNotification, EventHandler<FileDistributionTaskCreatedArgs> pOnTaskCreatedNotification)
            : base(prequestID, pstrFolderName, pdtExpiration, plstFilePathList, pbCompression, plstRecipient, pstartDate, pstrDescription, ptransferMode, ipriority, pOnFileDistributeNotification, pOnTaskCreatedNotification)
        {
            this.associatedRequestId = new List<Guid>();
        }

        /// <summary>
        /// Create File Distribution request
        /// </summary>
        /// <param name="prequestID">Request id</param>
        /// <param name="pstrFolderName">The folder to be uploaded</param>
        /// <param name="pdtExpiration">The date and time when the distribution task will expire and the file will be deleted from the T2G product. </param>
        /// <param name="pbCompression">If true, the files will be compressed before being transferred from train to the ground or vice-versa. false, otherwise</param>
        /// <param name="plstRecipient">The lists of targeted T2G systems and the list of T2G applications to be notified once the files are uploaded.</param>
        /// <param name="pstartDate">The date and time when the distribution of the specified files should start</param>
        /// <param name="pstrDescription">A description of the file transfer task.</param>
        /// <param name="ptransferMode">File transfer mode</param>
        /// <param name="ipriority">The priority of the transfer: a number between 0 and 32 (0 being the highest priority).</param>
        /// <param name="pOnFileDistributeNotification">Event handler to be notified on file distribution progress.</param>
        /// <param name="pOnTaskCreatedNotification">Event handler to be notified when transfer task is created.</param>
        public UploadFileDistributionRequest(Guid prequestID, IRemoteFolderClass pFolder, DateTime pdtExpiration, bool pbCompression, List<RecipientId> plstRecipient, DateTime pstartDate, string pstrDescription, FileTransferMode ptransferMode, sbyte ipriority, EventHandler<FileDistributionStatusArgs> pOnFileDistributeNotification, EventHandler<FileDistributionTaskCreatedArgs> pOnTaskCreatedNotification)
            : base(prequestID, pFolder, pdtExpiration, pbCompression, plstRecipient, pstartDate, pstrDescription, ptransferMode, ipriority, pOnFileDistributeNotification, pOnTaskCreatedNotification)
        {
            this.associatedRequestId = new List<Guid>();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the  FTP directory of the files to be downloaded. The path specified must be relative to the FTP user’s root path. If this parameter is empty, the FTP user’s root directory is used (“.”).
        /// </summary>
        internal string FtpDirectory
        {
            get
            {
                return this.ftpDirectory;
            }

            set
            {
                this.ftpDirectory = value;
            }
        }

        /// <summary>
        /// Gets or sets the  IP address of the FTP server hosting the files to be downloaded.
        /// </summary>
        internal string FtpServerIP
        {
            get
            {
                return this.ftpServerIP;
            }

            set
            {
                this.ftpServerIP = value;
            }
        }

        /// <summary>
        /// Gets or sets the  FTP server port number.
        /// </summary>
        internal ushort FtpPortNumber
        {
            get
            {
                return this.ftpPortNumber;
            }

            set
            {
                this.ftpPortNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the  login that should be used to log on to the FTP server.
        /// </summary>
        internal string FtpUserName
        {
            get
            {
                return this.ftpUserName;
            }

            set
            {
                this.ftpUserName = value;
            }
        }

        /// <summary>
        /// Gets or sets the  password that should be used to log on to the FTP server.
        /// </summary>
        internal string FtpPassword
        {
            get
            {
                return this.ftpPassword;
            }

            set
            {
                this.ftpPassword = value;
            }
        }

        /// <summary>
        /// RequestID of the request with same 
        /// </summary>
        public Guid UploadingRequestID
        {
            get
            {
                return this.UploadingRequestId;
            }

            set 
            { 
                this.UploadingRequestId = value; 
            }
        }

        /// <summary>
        /// Get list of requests waiting fot this request to finish its folder upload.
        /// </summary>
        public List<Guid> AssociatedRequestId
        {
            get
            {
                return this.associatedRequestId;
            }
        }

        /// <summary>
        /// Register another request as one waiting for this folder to complete its folder upload.
        /// </summary>
        public void RegisterForEndUploadNotification(Guid pRequestId)
        {
            this.associatedRequestId.Add(pRequestId);
        }

        #endregion
    }
}
