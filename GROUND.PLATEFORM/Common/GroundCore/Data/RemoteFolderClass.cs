﻿//​---------------------------------------​---------------------------------------​---------------------
// <copyright file="RemoteFolderClass.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//​---------------------------------------​---------------------------------------​---------------------
using System.Collections.Generic;
using System;
using System.Text;

namespace PIS.Ground.Core.Data
{
    class RemoteFolderClass : IRemoteFolderClass
    {
        #region attributes

        /// <summary>
        /// folder ID
        /// </summary>
        private int _folderId = 0;

        /// <summary>
        /// folder's name
        /// </summary>
        private string _name = string.Empty;

        /// <summary>
        /// Application ID
        /// </summary>
        private string _applicationId = string.Empty;

        /// <summary>
        /// Expiration Date of the folder. After it expired the folder is considered as not valid
        /// </summary>
        private DateTime _expirationDate = new DateTime();

        /// <summary>
        /// Total size in bytes of the folder's contents
        /// </summary>
        private long _totalFilesSize = 0;

        /// <summary>
        /// List of filenames in the folder. Warning : Should be same size as _RemoteFileList
        /// </summary>
        private List<string> _StrFileList = new List<string>();

        /// <summary>
        /// List of RemoteFileClass object in the folder. Warning : Should be same size as _StrFileList
        /// </summary>
        private List<IRemoteFileClass> _RemoteFileList = new List<IRemoteFileClass>();

        /// <summary>
        /// CRCGuid. CRC of folder's files in ascendant order separated by '-'
        /// </summary>
        private string _CRCGuid = string.Empty;

        /// <summary>
        /// Indicate if the folder has been not uploaded (0), uploaded (1), is in progress (2) or has failed (3).
        /// </summary>
        private UploadingStateEnum _uploadingState = UploadingStateEnum.NotUploaded;
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the folder ID
        /// </summary>
        public int FolderId
        {
            get { return _folderId; }
            set { _folderId = value; }
        }
        /// <summary>
        /// Gets or sets the name of the folder
        /// </summary>
        public string FolderName
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets ApplicationId
        /// </summary>
        public string ApplicationId
        {
            get { return _applicationId; }
            set { _applicationId = value; }
        }

        /// <summary>
        /// Gets or sets the expiration date of the folder
        /// </summary>
        public DateTime ExpirationDate
        {
            get { return _expirationDate; }
            set { _expirationDate = value; }
        }

        /// <summary>
        /// Gets total size of files in the folder
        /// </summary>
        public long TotalFilesSize
        {
            get { return _totalFilesSize; }
        }

        /// <summary>
        /// Gets List of files in the folder
        /// </summary>
        public List<string> FilenameList
        {
            get { return _StrFileList; }
        }

        /// <summary>
        /// Gets List of files
        /// </summary>
        public List<IRemoteFileClass> FolderFilesList
        {
            get { return _RemoteFileList; }
        }

        /// <summary>
        /// Gets CRCGuid of the folder.
        /// CRCGuid is composed of the CRC of each file in the folder sorted in ascendant order and separated by '-'
        /// </summary>
        public string CRCGuid
        {
            get { return _CRCGuid; }
        }

        /// <summary>
        /// Gets number of files in the folder
        /// </summary>
        public int NbFiles
        {
            get { return _RemoteFileList.Count; }
        }

        /// <summary>
        /// Gets and Sets UploadingState
        /// </summary>
        public UploadingStateEnum UploadingState
        {
            get { return _uploadingState; }
            set { _uploadingState = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a (empty) RemoteFolderClass from foldername and an ID
        /// </summary>
        /// <param name="pName">Name of the folder</param>
        /// <param name="pFolderId">Id of the folder</param>
        public RemoteFolderClass()
        {
            CalculateTotalFilesSize();
        }

        /// <summary>
        /// Create a (empty) RemoteFolderClass from foldername
        /// </summary>
        /// <param name="pName">Name of the folder</param>
        public RemoteFolderClass(string pName)
        {
            _name = pName;
            CalculateTotalFilesSize();
        }

        /// <summary>
        /// Create a  (empty) RemoteFolderClass from foldername and an ID
        /// </summary>
        /// <param name="pName">Name of the folder</param>
        /// <param name="pFolderId">Id of the folder</param>
        public RemoteFolderClass(string pName, int pFolderId)
        {
            _folderId = pFolderId;
            _name = pName;

            CalculateTotalFilesSize();
        }

        /// <summary>
        /// Copy constructor of RemoteFolderClass
        /// </summary>
        /// <param name="pRemoteFolder">RemoteFolderClass to use to create a new one</param>
        public RemoteFolderClass(RemoteFolderClass pRemoteFolder)
        {
            _folderId = pRemoteFolder.FolderId;
            _name = pRemoteFolder.FolderName;
            
            _applicationId = pRemoteFolder.ApplicationId;
            _expirationDate = pRemoteFolder.ExpirationDate;
            
            //this function will add RemoteFiles to the list, sort the list, synchronize List of RemoteFile with List of filename, calculate CRCGuid and total size of files
            AddListOfFilesToFolder(pRemoteFolder.FolderFilesList);
        }

        #endregion

        #region private Methods

        /// <summary>
        /// Add a filename in _StrFileList
        /// </summary>
        /// <param name="pFilename">name of the file</param>
        private void AddFilenameInStrFileList(string pFilename)
        {
            _StrFileList.Add(pFilename);
        }

        /// <summary>
        /// Calculate the totalFileSize from _RemoteFileList
        /// </summary>
        private void CalculateTotalFilesSize()
        {
            //Clean previous total
            _totalFilesSize = 0;

            if (_RemoteFileList.Count > 0)
            {
                //Add size of each file to the total size
                for (int i = 0; i < _RemoteFileList.Count; i++)
                {
                    _totalFilesSize += _RemoteFileList[i].Size;
                }
            }
        }

        /// <summary>
        /// Recalculate the totalFileSize from _RemoteFileList with new element added
        /// </summary>
        /// <<param name="pSize">RemoteFileClass object to add to the total</param>
        private void RecalculateTotalFilesSize(IRemoteFileClass pRemoteFile)
        {
            //Add to total size
            _totalFilesSize += pRemoteFile.Size;
        }

        #endregion

        #region public Methods

        /// <summary>
        /// use to generate attribute _CRCGuid from RemoteFileClass in _RemoteFileList
        /// The list is sorted by ascendant order of filenames when elements are added to the list
        /// </summary>
        public void CalculateCRCGuid()
        {
            //Clean previous CRCGuid
            _CRCGuid = "";

            if (_RemoteFileList.Count > 0)
            {
                //Initialize all file if not done
                foreach (IRemoteFileClass remoteFiles in _RemoteFileList)
                {
                    if (!remoteFiles.IsInitialized)
                    {
                        remoteFiles.mInitFile();
                    }
                }

                //Sort the list by filenames in ascendant order
                _RemoteFileList.Sort();

                //Concatenate CRC of RemoteFileClass in the list
                _CRCGuid = String.Join("-", Array.ConvertAll(_RemoteFileList.ToArray(), item => item.CRC));
            }
        }

        /// <summary>
        /// Add a remote file to folder in ascendant order of filename
        /// </summary>
        public void AddFileToFolder(IRemoteFileClass pFile)
        {
            //Check that file does not already exists in the list

            if(_RemoteFileList.Find(item => item.FileName == pFile.FileName) == null)
            {
                _RemoteFileList.Add(pFile);
                AddFilenameInStrFileList(pFile.FileName);
                RecalculateTotalFilesSize(pFile);
            }
        }

        /// <summary>
        /// Add list of files to the folder
        /// </summary>
        public void AddListOfFilesToFolder(List<IRemoteFileClass> pListOfFiles)
        {
            foreach (IRemoteFileClass pRemoteFile in pListOfFiles)
            {
                //Add file but do not recalculate CRC now
                AddFileToFolder(pRemoteFile);
            }
        }

        #endregion
    }
}
