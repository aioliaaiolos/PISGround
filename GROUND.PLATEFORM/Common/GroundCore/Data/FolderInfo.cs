using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PIS.Ground.Core.T2G.WebServices.FileTransfer;

namespace PIS.Ground.Core.Data
{
    public class FolderInfo : IFolderInfo
    {
        #region attributes

        /// <summary>
        /// List of file in the folder
        /// </summary>
        private fileList _fileList = new fileList();

        /// <summary>
        /// List of publication data
        /// </summary>
        private publicationList _publicationList = new publicationList();

        /// <summary>
        /// Ip adress of ftp server
        /// </summary>
        private string _ftpIp = string.Empty;

        /// <summary>
        /// Port of ftp server
        /// </summary>
        private ushort _ftpPort;

        /// <summary>
        /// Username for ftp server
        /// </summary>
        private string _username = string.Empty;

        /// <summary>
        /// Password for ftp server
        /// </summary>
        private string _pwd = string.Empty;

        /// <summary>
        /// FolderInfoStruct
        /// </summary>
        private folderInfoStruct _folderInfoStruct = new folderInfoStruct();

        /// <summary>
        /// Id of the request
        /// </summary>
        private Guid _requestId;

        /// <summary>
        /// Unique CRCGuid of the folder (list of files CRC in the folder separated by '-')
        /// </summary>
        private string _CRCGuid;

        #endregion

        #region constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pfilelist">list of files</param>
        /// <param name="pPublicationList">list of publication Data</param>
        /// <param name="pFtpIP">Ip of ftp server</param>
        /// <param name="pFtpport">Port number of ftp server</param>
        /// <param name="pUsername">username for ftp server access</param>
        /// <param name="pPwd">Password for ftp server access</param>
        /// <param name="pFolderInfoStruct">structure of FolderInfo on T2G server</param>
        public FolderInfo(fileList pfilelist, publicationList pPublicationList, string pFtpIP, ushort pFtpport, string pUsername, string pPwd, folderInfoStruct pFolderInfoStruct)
        {
            _fileList = pfilelist;
            _publicationList = pPublicationList;
            _ftpIp = pFtpIP;
            _ftpPort = pFtpport;
            _username = pUsername;
            _pwd = pPwd;
            _folderInfoStruct = pFolderInfoStruct;
            
            if (_folderInfoStruct.folderType == folderTypeEnum.upload)
            {
                string[] lSplitFoldername =  _folderInfoStruct.name.Split('|');
                if (lSplitFoldername.Length > 1)
                {
                    _CRCGuid = lSplitFoldername[1];
                    _folderInfoStruct.name = lSplitFoldername[0];
                }
            }
        }

        public FolderInfo(int pFolderId, string pFolderName, string pApplicationId, DateTime pExpirationDate, long pTotalFilesSize)
        {
            _folderInfoStruct.folderId = pFolderId;
            _folderInfoStruct.name = pFolderName;
            _folderInfoStruct.applicationId = pApplicationId;
            _folderInfoStruct.expirationDate = pExpirationDate;
            _folderInfoStruct.totalFilesSize = pTotalFilesSize;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Ftpport
        /// </summary>
        public ushort FtpPort
        {
            get
            {
                return this._ftpPort;
            }

            set
            {
                this._ftpPort = value;
            }

        }
        /// <summary>
        /// Gets or sets Pwd
        /// </summary>
        public string Pwd
        {
            get
            {
                return this._pwd;
            }

            set
            {
                this._pwd = value;
            }
        }

        /// <summary>
        /// Gets or sets User Name
        /// </summary>
        public string Username
        {
            get
            {
                return this._username;
            }

            set
            {
                this._username = value;
            }
        }

        /// <summary>
        /// Gets or sets FtpIP
        /// </summary>
        public string FtpIP
        {
            get
            {
                return this._ftpIp;
            }

            set
            {
                this._ftpIp = value;
            }
        }

        /// <summary>
        /// Gets or sets Filelist
        /// </summary>
        public fileList FileList
        {
            get
            {
                return this._fileList;
            }

            set
            {
                this._fileList = value;
            }
        }

        /// <summary>
        /// Gets or sets publicationList
        /// </summary>
        public publicationList PublicationList
        {
            get
            {
                return this._publicationList;
            }

            set
            {
                this._publicationList = value;
            }
        }

        /// <summary>
        /// Gets or sets FolderId
        /// </summary>
        public int FolderId
        {
            get
            {
                return this._folderInfoStruct.folderId;
            }

            set
            {
                this._folderInfoStruct.folderId = value;
            }
        }

        /// <summary>
        /// Gets or sets FolderName
        /// </summary>
        public string FolderName
        {
            get
            {
                return this._folderInfoStruct.name;
            }

            set
            {
                this._folderInfoStruct.name = value;
            }
        }

        /// <summary>
        /// Gets or sets ExpirationDate
        /// </summary>
        public DateTime ExpirationDate
        {
            get
            {
                return this._folderInfoStruct.expirationDate;
            }

            set
            {
                this._folderInfoStruct.expirationDate = value;
            }
        }

        /// <summary>
        /// Gets or sets acquisitionstate
        /// </summary>
        public acquisitionStateEnum AcquisitionState
        {
            get { return _folderInfoStruct.acquisitionState; }
        }

        /// <summary>
        /// Gets or sets SystemId
        /// </summary>
        public string SystemId
        {
            get
            {
                return this._folderInfoStruct.systemId;
            }

            set
            {
                this._folderInfoStruct.systemId = value;
            }
        }

        /// <summary>
        /// Gets or sets FtpPath
        /// </summary>
        public string FtpPath
        {
            get
            {
                return this._folderInfoStruct.path;
            }

            set
            {
                this._folderInfoStruct.path = value;
            }
        }

        /// <summary>
        /// Gets or sets RequestId
        /// </summary>
        public Guid RequestId
        {
            get
            {
                return this._requestId;
            }

            set
            {
                this._requestId = value;
            }
        }

        /// <summary>
        /// Gets or sets CRCGuid
        /// </summary>
        public string CRCGuid
        {
            get
            {
                return this._CRCGuid;

            }

            set
            {
                this._CRCGuid = value;
            }
        }

        #endregion
    }
}