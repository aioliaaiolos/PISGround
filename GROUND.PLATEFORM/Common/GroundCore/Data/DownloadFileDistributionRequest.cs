/// 
namespace PIS.Ground.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Class represtenting Download file distribution request
    /// </summary>
    public class DownloadFileDistributionRequest : FileDistributionRequest
    {
        #region Variables
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
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the DownloadFileDistributionRequest class
        /// </summary>
        public DownloadFileDistributionRequest()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DownloadFileDistributionRequest class
        /// </summary>
        /// <param name="prequestID">Request id</param>
        /// <param name="pstrFolderName">The folder’s name to be uploaded/downloaded</param>
        /// <param name="pdtExpiration">The date and time when the distribution task will expire and the file will be deleted from the T2G product. </param>
        /// <param name="plstFilePathList">A list of relative file paths </param>
        /// <param name="pbCompression">If true, the files will be compressed before being transferred from train to the ground or vice-versa. false, otherwise</param>
        /// <param name="pftpServerIP">The IP address of the FTP server hosting the files to be downloaded.</param>
        /// <param name="pftpPortNumber">The FTP server port number.</param>
        /// <param name="pftpUserName">The login that should be used to log on to the FTP server.</param>
        /// <param name="pftpPassword">The password that should be used to log on to the FTP server.</param>
        /// <param name="pftpDirectory">The FTP directory of the files to be downloaded. The path specified must be relative to the FTP user’s root path. If this parameter is empty, the FTP user’s root directory is used (“.”).</param>
        /// <param name="plstRecipient">The lists of targeted T2G systems and the list of T2G applications to be notified once the files are uploaded.</param>
        /// <param name="pstartDate">The date and time when the distribution of the specified files should start</param>
        /// <param name="pstrDescription">A description of the file transfer task.</param>
        /// <param name="ptransferMode">File transfer mode</param>
        /// <param name="ipriority">The priority of the transfer: a number between 0 and 32 (0 being the highest priority).</param>
        public DownloadFileDistributionRequest(Guid prequestID, string pstrFolderName, DateTime pdtExpiration, List<string> plstFilePathList, bool pbCompression, string pftpServerIP, ushort pftpPortNumber, string pftpUserName, string pftpPassword, string pftpDirectory, List<RecipientId> plstRecipient, DateTime pstartDate, string pstrDescription, FileTransferMode ptransferMode, sbyte ipriority, EventHandler<FileDistributionStatusArgs> pOnFileDistributeNotification)
            : base(prequestID, pstrFolderName, pdtExpiration, plstFilePathList, pbCompression, plstRecipient, pstartDate, pstrDescription, ptransferMode, ipriority, pOnFileDistributeNotification)
        {
            this.ftpServerIP = pftpServerIP;
            this.ftpPortNumber = pftpPortNumber;
            this.ftpUserName = pftpUserName;
            this.ftpPassword = pftpPassword;
            this.ftpDirectory = pftpDirectory;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Set the FTP directory of the files to be downloaded. The path specified must be relative to the FTP user’s root path.
        /// </summary>
        public string FtpDirectory
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
        /// Gets or Set the IP address of the FTP server hosting the files to be downloaded.
        /// </summary>
        public string FtpServerIP
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
        /// Gets or Set the FTP server port number.
        /// </summary>
        public ushort FtpPortNumber
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
        /// Gets or Set the login that should be used to log on to the FTP server.
        /// </summary>
        public string FtpUserName
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
        /// Gets or Set the password that should be used to log on to the FTP server.
        /// </summary>
        public string FtpPassword
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
        #endregion
    }
}
