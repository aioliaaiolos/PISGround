﻿//​---------------------------------------​---------------------------------------​---------------------
// <copyright file="IFolderInfo.cs" company="Alstom">
//          (c) Copyright ALSTOM 2013.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//​---------------------------------------​---------------------------------------​---------------------
using PIS.Ground.Core.T2G.WebServices.FileTransfer;
using System;

namespace PIS.Ground.Core.Data
{
    /// <summary>
    /// Interface defining a folder info
    /// </summary>
    public interface IFolderInfo
    {
        #region Properties
        /// <summary>
        /// Gets or sets Ftpport
        /// </summary>
        ushort FtpPort
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets Pwd
        /// </summary>
        string Pwd
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets User Name
        /// </summary>
        string Username
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets FtpIP
        /// </summary>
        string FtpIP
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Fillist
        /// </summary>
        fileList FileList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets publicationList
        /// </summary>
        publicationList PublicationList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets acquisitionstate
        /// </summary>
        acquisitionStateEnum AcquisitionState
        {
            get;
        }

        /// <summary>
        /// Gets or sets FolderId
        /// </summary>
        int FolderId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets FolderName
        /// </summary>
        string FolderName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets ExpirationDate
        /// </summary>
        DateTime ExpirationDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets SystemId
        /// </summary>
        string SystemId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets FtpPath
        /// </summary>
        string FtpPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets RequestId
        /// </summary>
        Guid RequestId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets CRCGuid
        /// </summary>
        string CRCGuid
        {
            get;
            set;
        }

        #endregion
    }
}
