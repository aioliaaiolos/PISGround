﻿//---------------------------------------------------------------------------------------------------
// <copyright file="RemoteFileClass.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using PIS.Ground.Core.LogMgmt;
using System.Globalization;
using PIS.Ground.Core.Properties;

namespace PIS.Ground.Core.Data
{
    /// <summary>
    /// This class manage remote file for read access. It simplified acces methodes to don't care
    /// about ftp, http or local file when dealing with files.
    /// </summary>
    public class RemoteFileClass : IRemoteFileClass
    {
        #region attributes
        
        /// <summary>Full pathname of the file.</summary>
        private string           _filePath;
        /// <summary>Filename of the file.</summary>
        private string           _fileName;
        /// <summary>The size.</summary>
        private long             _size;
        /// <summary>true to exists.</summary>
        private bool             _exists;
        /// <summary>Type of the file.</summary>
        private FileTypeEnum     _fileType;
        /// <summary>The CRC.</summary>
        private string           _crc ="";
        /// <summary>Destination remote file in case of ftp or http.</summary>
        private string _destRemoteFile = "";
        /// <summary>FtpStatus of the file transfer</summary>
        private FtpStatus _ftpStatus;
        /// <summary>
        /// Flag to indicate if this RemoteFileClass has error such as FTP Transfer error.
        /// </summary>
        private bool _hasError = false;
        /// <summary>
        /// Flag if the file has been initialized yet
        /// </summary>
        private bool _initialized = false;

        #endregion

        #region constructor

        /// <summary>
        /// Constructor. Set filePath parameter according to input and the file type. For other fields,
        /// they are initialized only when needed so we save network access.
        /// </summary>
        /// <param name="pFilePath">The path to the file. Path begining with ftp or http are check
        /// <<param name="pInitFile">Specify if the file required to be initialized now. CRC calculation can take a long time depending on file size"</param>
        /// individually. All other path are checked as local path.</param>
        public RemoteFileClass(string pFilePath, bool pInitFile)
        {
            try
            {
                if (!String.IsNullOrEmpty(pFilePath))
                {
                    _filePath = pFilePath;
                    _fileName = System.IO.Path.GetFileName(_filePath);
                    string lDataStorePath = System.IO.Path.GetFullPath(ConfigurationSettings.AppSettings["RemoteDataStoreUrl"]);
                    string lFileName = Path.GetFileName(_filePath);
                    string lFileNameWExt = Path.GetFileNameWithoutExtension(_filePath);
                    string lType = lFileNameWExt.Substring(0, lFileName.IndexOf('-')).ToUpperInvariant();
                    string lDestFolder = lDataStorePath + lType;
                    // Saving the remote destination file.
                    _destRemoteFile = Path.Combine(lDestFolder, lFileName);

                    if (pInitFile)
                    {
                        mInitFile();
                        _initialized = true;
                    }
                }
            }
            catch (ArgumentOutOfRangeException lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, String.Format(CultureInfo.CurrentCulture, Resources.RemoteFileClassArgumentOutOfRangeException, _filePath), "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }

        }

        #endregion

        #region destructor
        /// <summary>Destructor.</summary>
        ~RemoteFileClass()
	    {
        }

        #endregion

        #region properties

        /// <summary>Return the file name. FileName is set from filePath.</summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>Return the file path.</summary>
        /// <value>The full pathname of the file.</value>
        public string FilePath
        {
            get { return _filePath; }
        }

        /// <summary>
        /// return the file size using filetype and http/ftp request to resolve it over network (FileInfo
        /// otherwise).
        /// </summary>
        /// <value>The size.</value>
        public long Size
        {
            get { return _size; }
        }

        /// <summary>
        /// Open the file for reading only. Return a stream on the file so you can read it.
        /// </summary>
        /// <param name="pStream">The ouput stream.</param>
        public void OpenStream(out System.IO.Stream pStream)
        {
            pStream = null;
            switch (_fileType)
            {
                case FileTypeEnum.Undefined:
                    break;
                case FileTypeEnum.LocalFile:
                    OpenReadLocalFile(out pStream);
                    break;
                case FileTypeEnum.FtpFile:
                    OpenReadRemoteDownloadedFile(out pStream);
                    break;
                case FileTypeEnum.HttpFile:
                    OpenReadRemoteDownloadedFile(out pStream);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Return wheter or not the file realy exists (and is a ccessible). True if the file is.
        /// </summary>
        /// <value>true if exists, false if not.</value>
        public bool Exists
        {
            get { return _exists; }
        }

        public FileTypeEnum FileType
        {
            get { return _fileType; }
        }

        /// <summary>Return the CRC of the file.</summary>
        /// <value>The CRC.</value>
        public string CRC
        {
            get { return _crc; }
        }

        /// <summary>Return or set FtpStatus</summary>
        /// <value>The CRC.</value>
        public FtpStatus FtpStatus
        {
            get { return _ftpStatus; }
            set { _ftpStatus = value; }
        }

        /// <summary>Return or flag is that file has error (during upload for example)</summary>
        /// <value>The CRC.</value>
        public bool HasError
        {
            get { return _hasError; }
            set { _hasError = value; }
        }

        /// <summary>Return if the file has been initialized.</summary>
        /// <value>The CRC.</value>
        public bool IsInitialized
        {
            get { return _initialized; }
        }

        #endregion

        #region methods

        /// <summary>
        /// Initialization for file. 
        /// </summary>
        public void mInitFile()
        {
            if (_initialized == false)
            {
                if (_filePath.StartsWith("ftp:"))
                {
                    mInitFtpFile();
                }
                else if (_filePath.StartsWith("http:"))
                {
                    mInitHttpFile();
                }
                else
                {
                    _filePath = mGetValidUriFromPath(_filePath);
                    mInitLocalFile();
                }
            }
        }

        /// <summary>
        /// Initialization for a local file (i.e UNC or local path). Use FileInfo for checking path and
        /// getting file size. Use Utility.Crc32 to calculate the CRC. All errors are catch and write in
        /// log manager.
        /// </summary>
        private void mInitLocalFile()
        {
            try
            {
                System.IO.FileInfo lFileInfo = new System.IO.FileInfo(_filePath);
                _exists = lFileInfo.Exists;
                _size = lFileInfo.Length;
                _fileType = FileTypeEnum.LocalFile;

                System.IO.Stream lFileStream = lFileInfo.OpenRead();

                //Calcul CRC
                Utility.Crc32 lCrcCalculator = new PIS.Ground.Core.Utility.Crc32();

                _crc = lCrcCalculator.CalculateChecksum(lFileStream);
            }
            catch (System.Security.SecurityException lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitLocalFile() SecurityException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }
            catch (ArgumentException lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitLocalFile() ArgumentException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }
            catch (UnauthorizedAccessException lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitLocalFile() UnauthorizedAccessException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }
            catch (System.IO.PathTooLongException lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitLocalFile() PathTooLongException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }
            catch (NotSupportedException lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitLocalFile() NotSupportedException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }
            catch (Exception lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitLocalFile() UnsupportedException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }
        }

        /// <summary>
        /// Initialization for a ftp remote file (i.e ftp://). Use FtpWebRequest for checking path. Then
        /// open a stream and get file size. Use Utility.Crc32 to calculate the CRC on the stream. All
        /// errors are catch and write in log manager.
        /// </summary>
        private void mInitFtpFile()
        {
            try
            {
                System.Net.FtpWebRequest lRequest = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(_filePath);

                lRequest.Method = System.Net.WebRequestMethods.Ftp.GetFileSize;
                using (System.Net.WebResponse lResponse = lRequest.GetResponse())
                {
                    _exists = true;
                    _fileType = FileTypeEnum.FtpFile;

                    //CalculCRC 
                    Utility.Crc32 lCrcCalculator = new PIS.Ground.Core.Utility.Crc32();
                    System.IO.Stream lFileStream;

                    OpenReadFtpFile(out lFileStream);

                    if (lFileStream != null)
                    {
                        _size = lFileStream.Length;
                        _crc = lCrcCalculator.CalculateChecksum(lFileStream);
                        lFileStream.Close();
                    }
                    else
                    {
                        PIS.Ground.Core.LogMgmt.LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.ERROR, "OpenReadFtpFile returned null file", "PIS.Ground.Core.Data.RemoteFileClass", null, EventIdEnum.GroundCore);
                    }
                }
            }
            catch (NotSupportedException lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitFtpFile() NotSupportedException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }
            catch (System.Security.SecurityException lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitFtpFile() SecurityException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }
            catch (UriFormatException lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitFtpFile() UriFormatException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }
            catch (Exception lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitFtpFile() UnsupportedException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }
        }

        /// <summary>
        /// Initialization for a ftp remote file (i.e http://). Use HttpWebRequest for checking path.
        /// Then open a stream and get file size. Use Utility.Crc32 to calculate the CRC on the stream.
        /// All errors are catch and write in log manager.
        /// </summary>
        private void mInitHttpFile()
        {
            try
            {
                System.Net.HttpWebRequest lRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(_filePath);

                lRequest.Method = System.Net.WebRequestMethods.Http.Head;
                using (System.Net.HttpWebResponse lResponse = (System.Net.HttpWebResponse)lRequest.GetResponse())
                {
                    _exists = true;
                    _fileType = FileTypeEnum.HttpFile;

                    //CalculCRC 
                    Utility.Crc32 lCrcCalculator = new PIS.Ground.Core.Utility.Crc32();

                    System.IO.Stream lFileStream;
                    OpenReadHttpFile(out lFileStream);
                    if (lFileStream != null)
                    {
                        _crc = lCrcCalculator.CalculateChecksum(lFileStream);
                        _size = lFileStream.Length;
						lFileStream.Close();
                    }
                    else
                    {
                        PIS.Ground.Core.LogMgmt.LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.ERROR, "mInitHttpFile returned null file", "PIS.Ground.Core.Data.RemoteFileClass", null, EventIdEnum.GroundCore);
                    }
                }

            }
            catch (NotSupportedException lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitHttpFile() NotSupportedException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }
            catch (System.Security.SecurityException lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitHttpFile() SecurityException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }
            catch (UriFormatException lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitHttpFile() UriFormatException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }
            catch (Exception lEx)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, "mInitHttpFile() UnsupportedException with FilePath : " + _filePath, "PIS.Ground.Core.Data.RemoteFileClass", lEx, EventIdEnum.GroundCore);
            }   
        }

        /// <summary>
        /// Open a FileStream on a local file for read only. On failed, output stream is null and a
        /// message is write in logs.
        /// </summary>
        /// <param name="pStream">The ouput stream.</param>
        private void OpenReadLocalFile(out System.IO.Stream pStream)
        {
            try
            {
                pStream = new System.IO.FileStream(_filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            }
            catch (Exception ex)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.Data.RemoteFileClass", ex, EventIdEnum.GroundCore);
                pStream = null;
            }

        }

        /// <summary>
        /// Open the previous downloaded FileStream from FTP or HTTP on a local file for read only. 
        /// On failed, output stream is null and a message is write in logs.
        /// </summary>
        /// <param name="pStream">The ouput stream.</param>
        private void OpenReadRemoteDownloadedFile(out System.IO.Stream pStream)
        {
            try
            {
                pStream = new System.IO.FileStream(_destRemoteFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            }
            catch (Exception ex)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.Data.RemoteFileClass", ex, EventIdEnum.GroundCore);
                pStream = null;
            }

        }

        /// <summary>
        /// Open Ftp file in a MemoryStream for read only. The FtpStream is copied in a MemoryStream.
        /// </summary>
        /// <param name="pStream">The ouput stream.</param>
        private void OpenReadFtpFile(out System.IO.Stream pStream)
        {
            pStream = null;
            try
            {
                System.Net.FtpWebRequest lRequestFileDl = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(_filePath);
                lRequestFileDl.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
                System.Net.FtpWebResponse lResponseFileDl = (System.Net.FtpWebResponse)lRequestFileDl.GetResponse();

                FileInfo lDestFileInfo = new FileInfo(_destRemoteFile);

                using (System.IO.FileStream lFileStream = lDestFileInfo.OpenWrite())
                {
                    using (System.IO.Stream lStream = lResponseFileDl.GetResponseStream())
                    {
                        int i = 0;
                        byte[] bytes = new byte[2048];
                        do
                        {
                            i = lStream.Read(bytes, 0, bytes.Length);
                            lFileStream.Write(bytes, 0, i);
                        } while (i != 0);

                        lStream.Close();
                    }
                    lFileStream.Close();
                }
                
                pStream = lDestFileInfo.OpenRead();
                pStream.Seek(0, System.IO.SeekOrigin.Begin);

            }
            catch (Exception ex)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.Data.RemoteFileClass", ex, EventIdEnum.GroundCore);
            }
        }

        /// <summary>
        /// Open Http file in a MemoryStream for read only. The HttpStream is copied in a MemoryStream.
        /// </summary>
        /// <param name="pStream">The ouput stream.</param>
        private void OpenReadHttpFile(out System.IO.Stream pStream)
        {
            pStream = null;
            try
            {
                System.Net.HttpWebRequest lRequestFileDl = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(_filePath);
                System.Net.HttpWebResponse lResponseFileDl = (System.Net.HttpWebResponse)lRequestFileDl.GetResponse();

                FileInfo lDestFileInfo = new FileInfo(_destRemoteFile);

                using (System.IO.FileStream lFileStream = lDestFileInfo.OpenWrite())
                {
                    using (System.IO.Stream lStream = lResponseFileDl.GetResponseStream())
                    {
                        int i = 0;
                        byte[] bytes = new byte[2048];
                        do
                        {
                            i = lStream.Read(bytes, 0, bytes.Length);
                            lFileStream.Write(bytes, 0, i);
                        } while (i != 0);
                        lStream.Close();
                    }
                    lFileStream.Close();
                }

                pStream = lDestFileInfo.OpenRead();
                pStream.Seek(0, System.IO.SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.Data.RemoteFileClass", ex, EventIdEnum.GroundCore);
            }
        }

        /// <summary>
        /// Transforme an UNC path in a valid path (i.e. replace hostname by Ip, remove file:///,
        /// harmonizes / and \)
        /// </summary>
        /// <param name="pUrl">The url to transform.</param>
        /// <returns>The valid URI from path.</returns>
        private static string mGetValidUriFromPath(string pUrl)
        {
            string lResultUrl = String.Empty;
            pUrl = pUrl.Replace("/",@"\");
            if (pUrl.StartsWith("file:") || pUrl.StartsWith(@"\\"))
            {
                if (pUrl.StartsWith("file:"))// remove file:
                {
                    pUrl = pUrl.Substring(5);
                }
                List<string> lPathParts = new List<string>(pUrl.Split('\\'));
                lPathParts.RemoveAll(String.IsNullOrEmpty);
                if (!System.Text.RegularExpressions.Regex.Match(lPathParts[0], @"\w:").Success) //we assumes that it's a ip or hostname
                {
                    System.Net.IPHostEntry lIpEntry;//we get the ip to replace in url
                    lIpEntry = System.Net.Dns.GetHostEntry(lPathParts[0]);
                    lPathParts[0] = lIpEntry.AddressList[0].ToString();
                }
                lResultUrl = @"\\" + lPathParts[0];
                foreach (string lpart in lPathParts.GetRange(1,lPathParts.Count-1))
                {
                    lResultUrl += '\\' + lpart;
                }
            }
            else
            {
                lResultUrl = pUrl;
            }

            return lResultUrl;
        }

        /// <summary>Check URL.</summary>
        /// <param name="pUrl">.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool checkUrl(string pUrl)
        {
            bool lResult = false;
            try 
	        {
	            if (pUrl.StartsWith("ftp:"))
                {
                    System.Net.FtpWebRequest lRequest = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(pUrl);
                    lRequest.Method = System.Net.WebRequestMethods.Ftp.GetFileSize;
                    using (System.Net.WebResponse lResponse = lRequest.GetResponse())
                    {
                        lResult = true;
                    }
                }
                else if (pUrl.StartsWith("http:"))
                {
                    System.Net.HttpWebRequest lRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(pUrl);

                    lRequest.Method = System.Net.WebRequestMethods.Http.Head;
                    using (System.Net.HttpWebResponse lResponse = (System.Net.HttpWebResponse)lRequest.GetResponse())
                    {
                        lResult = true;
                    }
                }
                else
                {
                    string lUrl =  mGetValidUriFromPath(pUrl);
                    System.IO.FileInfo lFileInfo = new System.IO.FileInfo(lUrl);
                    lResult = lFileInfo.Exists;
                }
	        }
	        catch (Exception ex)
	        {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.Data.RemoteFileClass", ex, EventIdEnum.GroundCore);
                lResult = false;
	        }
            return lResult;
        }

        #endregion

        #region Comparison
        
        /// <summary>
        /// Override CompareTo to compare two files on name.
        /// </summary>
        /// <param name="pCompareRemoteFildeClass">Object that needs to be compared</param>
        /// <returns></returns>
        public int CompareTo(IRemoteFileClass pCompareRemoteFildeClass)
        {
            if (pCompareRemoteFildeClass == null)
            {
                return 1;
            }
            else
            {
                return this._fileName.CompareTo(pCompareRemoteFildeClass.FileName);
            }
        }

        #endregion
    }
}
