﻿//​---------------------------------------​---------------------------------------​---------------------
// <copyright file="IRemoteFolderClass.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//​---------------------------------------​---------------------------------------​---------------------

using System;
using System.Collections.Generic;

namespace PIS.Ground.Core.Data
{
    /// <summary>
    /// The different state of a remote folder while uploading
    /// </summary>
    public enum UploadingStateEnum
    {
        NotUploaded = 0,
        InProgress = 1,
        Uploaded = 2,
        Failed = 3,
    }

    /// <summary>
    /// Struct of a foldername
    /// </summary>
    public struct FolderNameStruct
    {
        public Guid RequestId;
        public string CRCGuid;

        public FolderNameStruct(string pFoldername)
        {
            string[] SplittedElements = pFoldername.Split('|');

            if (SplittedElements.Length > 1)
            {
                this.RequestId = new Guid(SplittedElements[0]);
                this.CRCGuid = SplittedElements[1];
            }
            else
            {
                this.RequestId = new Guid(SplittedElements[0]);
                this.CRCGuid = "";
            }
        }
    }

    /// <summary>
    /// Interface defining a remote folder. It can calculate the CRCGuid from its content (format is CRCFile1-CRCFile2-...-CRCFileX sorted in ascending order by name)
    /// </summary>
    public interface IRemoteFolderClass
    {
        #region Properties
        /// <summary>
        /// Gets or sets the folder ID
        /// </summary>
        int FolderId
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the name of the folder
        /// </summary>
        string FolderName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets ApplicationId
        /// </summary>
        string ApplicationId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the expiration date of the folder
        /// </summary>
        DateTime ExpirationDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets total size of files in the folder
        /// </summary>
        long TotalFilesSize
        {
            get;
        }

        /// <summary>
        /// Gets List of files in the folder
        /// </summary>
        List<string> FilenameList
        {
            get;
        }

        /// <summary>
        /// Gets List of files
        /// </summary>
        List<IRemoteFileClass> FolderFilesList
        {
            get;
        }

        /// <summary>
        /// Gets CRCGuid of the folder.
        /// CRCGuid is composed of the CRC of each file in the folder sorted in ascendant order and separated by '-'
        /// </summary>
        string CRCGuid
        {
            get;
        }

        /// <summary>
        /// Gets number of files in the folder
        /// </summary>
        int NbFiles
        {
            get;
        }

        /// <summary>
        /// Gets and Sets UploadingState
        /// </summary>
        UploadingStateEnum UploadingState
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a remote file to folder
        /// </summary>
        void AddFileToFolder(IRemoteFileClass pFile);

        /// <summary>
        /// Add list of files to the folder
        /// </summary>
        void AddListOfFilesToFolder(List<IRemoteFileClass> pListOfFiles);

        /// <summary>
        /// use to generate attribute _CRCGuid from RemoteFileClass in _RemoteFileList
        /// The list is sorted by ascendant order of filenames when elements are added to the list
        /// </summary>
        void CalculateCRCGuid();

        #endregion
    }
}
