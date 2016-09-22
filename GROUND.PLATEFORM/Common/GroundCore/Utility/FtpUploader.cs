//---------------------------------------------------------------------------------------------------
// <copyright file="FtpUploader.cs" company="Alstom">
//          (c) Copyright ALSTOM 2013.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.Properties;
using PIS.Ground.Core.T2G.WebServices.FileTransfer;

namespace PIS.Ground.Core.Utility
{												   
    /// <summary>FTP utility.</summary>
    internal class FtpUtility
    {
        /// <summary>Time out period in minutes for uploading/downloading the file.</summary>
		const int TimeOut = 600000;

        /// <summary>Retry period in milliseconds for retrying the upload/download.</summary>
        const int RetryPeriod = 60000;

        /// <summary>Initializes the FTP request.</summary>
        /// <param name="pRemoteFile">The remote file.</param>
        /// <param name="serverIP">The server IP.</param>
        /// <param name="directoryName">Pathname of the directory.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="strPwd">The password.</param>
        /// <param name="reqFTP">[out] The request FTP.</param>
        private static void InitializeFtpRequest(IRemoteFileClass pRemoteFile, string serverIP, string directoryName, string userName, string strPwd, out FtpWebRequest reqFTP)
        {
            string url = "ftp://" + serverIP + "/" + directoryName + "/" + pRemoteFile.FileName;
            try
            {
                //// Create FtpWebRequest object from the Uri provided
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            }
            catch (System.Security.SecurityException ex)
            {
                reqFTP = null;
                LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExFtpLogin, pRemoteFile.FilePath), "PIS.Ground.Core.Utility.FtpUpLoader.InitializeFtpRequest", ex, EventIdEnum.GroundCore);
                return;
            }
            catch (ArgumentNullException ex)
            {
                reqFTP = null;
                LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExFtpLoginArgument, pRemoteFile.FilePath), "PIS.Ground.Core.Utility.FtpUpLoader.InitializeFtpRequest", ex, EventIdEnum.GroundCore);
                return;
            }
            catch(Exception ex)
            {
                reqFTP = null;
                LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExFtpLoginException, pRemoteFile.FilePath), "PIS.Ground.Core.Utility.FtpUpLoader.InitializeFtpRequest", ex, EventIdEnum.GroundCore);
                return;
            }

            //// Provide the WebPermission Credintials
            reqFTP.Credentials = new NetworkCredential(userName, strPwd);

            //// By default KeepAlive is true, where the control connection is not closed after a command is executed.
            reqFTP.KeepAlive = false;

            //// Specify the command to be executed.
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            //// Specify the data transfer type.
            reqFTP.UseBinary = true;
        }

        /// <summary>Downloads the file.</summary>
        /// <param name="filePath">Full pathname of the file.</param>
        /// <param name="fileName">Filename of the file.</param>
        /// <param name="ftpServerIP">The FTP server IP.</param>
        /// <param name="ftpUserID">Identifier for the FTP user.</param>
        /// <param name="ftpPassword">The FTP password.</param>
        /// <param name="ftpStatusCode">[out] The FTP status code.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        private static bool DownloadFile(string filePath, string fileName, string ftpServerIP, string ftpUserID, string ftpPassword, out FtpStatusCode ftpStatusCode)
        {
            FtpWebRequest reqFTP;
            ftpStatusCode=FtpStatusCode.Undefined;
            try
            {
                FileStream outputStream = new FileStream(filePath + "\\" + fileName, FileMode.Create);
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable || response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailableOrBusy
                    || response.StatusCode == FtpStatusCode.ServiceNotAvailable || response.StatusCode == FtpStatusCode.ServiceTemporarilyNotAvailable)
                {
                    ftpStatusCode = response.StatusCode;
                    return false;
                }
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStatusCode = response.StatusCode;
                ftpStream.Close();
                outputStream.Close();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.Utility.FtpUpLoader.DownloadFile", ex, EventIdEnum.GroundCore);
                ftpStatusCode = FtpStatusCode.Undefined;
                return false;
            }
        }

        /// <summary>Uploads a file.</summary>
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        /// <param name="state">The state.</param>
        private static void UploadFile(object state)
        {
            if (state is FtpState)
            {
                FtpState ftpState = state as FtpState;
                ftpState.Response = null;
                if (!string.IsNullOrEmpty(ftpState.RemoteFile.FileName) && ftpState.RemoteFile.Exists)
                {
                    //// Notify the server about the size of the uploaded file
                    ftpState.Request.ContentLength = ftpState.RemoteFile.Size;
                    //// The buffer size is set to 2kb
                    const int buffLength = 2048;
                    byte[] buff = new byte[buffLength];
                    int contentLen;

                    try
                    {
                        //// Opens a file stream to read the file to be uploaded
                        Stream fs;
                        ftpState.RemoteFile.OpenStream(out fs);
                        if (fs != null)
                        {
							using (fs)
							{
								fs.Seek(0, SeekOrigin.Begin);
								//// Stream to which the file to be upload is written
								using (Stream strm = ftpState.Request.GetRequestStream())
								{
									//// Read from the file stream 2kb at a time
									contentLen = fs.Read(buff, 0, buffLength);
									//// Till Stream content ends
									while (contentLen != 0)
									{
										//// Write Content from the file stream to the FTP Upload Stream
										strm.Write(buff, 0, contentLen);
										contentLen = fs.Read(buff, 0, buffLength);
									}
								}
							}
                        }
                        else
                        {
                            string lErrorMessage = "Can't read file ";
                            lErrorMessage += ftpState.RemoteFile.FilePath;
                            FileNotFoundException lFNFEx = new FileNotFoundException(lErrorMessage, ftpState.RemoteFile.FilePath);
                            LogManager.WriteLog(TraceType.ERROR, lFNFEx.Message, "PIS.Ground.Core.Utility.FtpUtility.UploadFile", lFNFEx, EventIdEnum.GroundCore);
                            throw lFNFEx;
                        }

                        if (ftpState.Request != null)
                        {
                            ftpState.Response = (FtpWebResponse)ftpState.Request.GetResponse();
                            if (ftpState.Response.StatusCode != FtpStatusCode.ServiceNotAvailable && ftpState.Response.StatusCode != FtpStatusCode.ServiceTemporarilyNotAvailable)
                            {
                                ftpState.OperationComplete.Set();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ftpState.FtpStatus.OperationException = ex;
                        ftpState.OperationComplete.Set();
						LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.Utility.FtpUtility.UploadFile", ex, EventIdEnum.GroundCore);
                    }
                }
            }
        }

        /// <summary>Upload the File to Ftp.</summary>
        /// <param name="objUploadFileDistributionRequest">Input File distribution request.</param>
        /// <returns>error if any.</returns>
        internal static string Upload(ref UploadFileDistributionRequest objUploadFileDistributionRequest)
        {
            string error = string.Empty;
            if (objUploadFileDistributionRequest != null && !string.IsNullOrEmpty(objUploadFileDistributionRequest.FtpServerIP) && !string.IsNullOrEmpty(objUploadFileDistributionRequest.FtpUserName) && !string.IsNullOrEmpty(objUploadFileDistributionRequest.FtpPassword) && objUploadFileDistributionRequest.Folder.NbFiles>0)
            {
                foreach (IRemoteFileClass lRemoteFile in objUploadFileDistributionRequest.Folder.FolderFilesList)
                {
                    if (lRemoteFile.Exists)
                    {
                        /* Testing mode is enable. So, simulate that file was uploaded properly.
                         * 
                         * In the future, a better approach might be found.
                         */
                        if (lRemoteFile.FileType == FileTypeEnum.Undefined && RemoteFileClass.TestingModeEnabled)
                        {
                            FtpStatus ftpStatus = new FtpStatus();
                            ftpStatus.FileName = lRemoteFile.FilePath;
                            ftpStatus.FtpStatusCode = FtpStatusCode.FileActionOK; ;
                            ftpStatus.OperationException = null;
                            ftpStatus.StatusDescription = "OK";

                            lRemoteFile.FtpStatus = ftpStatus;

                            continue;
                        }

                        DateTime dtTime = DateTime.Now;
                        FtpWebRequest reqFTP = null;
                        try
                        {
                            InitializeFtpRequest(lRemoteFile, objUploadFileDistributionRequest.FtpServerIP, objUploadFileDistributionRequest.FtpDirectory, objUploadFileDistributionRequest.FtpUserName, objUploadFileDistributionRequest.FtpPassword, out reqFTP);
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
						if (reqFTP != null)
						{
							FtpWebResponse response = null;
							try
							{
								using (ManualResetEvent timerEvent = new ManualResetEvent(false))
								using (FtpState ftpState = new FtpState())
								{
									ftpState.RemoteFile = lRemoteFile;
									ftpState.Request = reqFTP;
									ftpState.Request.Timeout = Timeout.Infinite;

									UploadFile(ftpState);
									if (ftpState.Response != null)
									{
										response = ftpState.Response;
										if (response.StatusCode == FtpStatusCode.ServiceNotAvailable || response.StatusCode == FtpStatusCode.ServiceTemporarilyNotAvailable)
										{
											if (DateTime.Now.Subtract(dtTime).TotalMinutes > TimeOut)
											{
												error = "T2G Ground Server not responding for " + TimeOut + " period uploading terminated";
                                                objUploadFileDistributionRequest.Folder.UploadingState = UploadingStateEnum.Failed;
												return error;
											}
										}

                                        if (response.StatusCode != FtpStatusCode.FileActionOK && response.StatusCode != FtpStatusCode.ClosingData && response.StatusCode != FtpStatusCode.DirectoryStatus && response.StatusCode != FtpStatusCode.FileStatus && response.StatusCode != FtpStatusCode.SystemType && response.StatusCode != FtpStatusCode.SendUserCommand && response.StatusCode != FtpStatusCode.ClosingControl && response.StatusCode != FtpStatusCode.ClosingData && response.StatusCode != FtpStatusCode.PathnameCreated)
                                        {
                                            lRemoteFile.HasError = true;
                                            LogManager.WriteLog(TraceType.INFO, "File upload error, create transfer task. RequestID = " + objUploadFileDistributionRequest.RequestId.ToString() + ", CRCGuid = " + objUploadFileDistributionRequest.Folder.CRCGuid + ", Folder ID = " + objUploadFileDistributionRequest.Folder.FolderId.ToString() + ", FTP error = "+response.StatusCode.ToString() , "Ground.Core.T2G.T2GClient.StartTransferTask", null, EventIdEnum.GroundCore);

                                            objUploadFileDistributionRequest.Folder.UploadingState = UploadingStateEnum.Failed;
                                        }

										FtpStatus ftpStatus = new FtpStatus();
										ftpStatus.FileName = lRemoteFile.FilePath;
										ftpStatus.FtpStatusCode = response.StatusCode;
										ftpStatus.OperationException = null;
										ftpStatus.StatusDescription = response.StatusDescription;

										lRemoteFile.FtpStatus = ftpStatus;
    
									}
									else
									{
										error = "Can't upload file on T2G Ftp Server. Server is not responding or input file is invalid, check error log.";
                                        objUploadFileDistributionRequest.Folder.UploadingState = UploadingStateEnum.Failed;
                                        return error;
									}

								}
							}
							catch (Exception ex)
							{
								response = (FtpWebResponse)reqFTP.GetResponse();
								if (response != null)
								{
									FtpStatus ftpStatus = new FtpStatus();
									ftpStatus.FileName = lRemoteFile.FilePath;
									ftpStatus.FtpStatusCode = response.StatusCode;
									ftpStatus.OperationException = ex;
									ftpStatus.StatusDescription = response.StatusDescription;
									
									lRemoteFile.FtpStatus = ftpStatus;
									
									LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExFtpError, "PIS.Ground.Core.Utility.Upload", ex, EventIdEnum.GroundCore);
								}
							}
							finally
							{
								if (response != null)
								{
									response.Close();
								}
							}
						}
                    }
                    else
                    {
                        error = "File " + lRemoteFile.FilePath + " does not exist.";
                        return error;
                    }
                }
            }
            else
            {
                error = "Invalid Input parameter";
            }
            return error;
        }

        /// <summary>Downloads the given objDownloadFolderRequest.</summary>
        /// <param name="objDownloadFolderRequest">[in,out] The object download folder request.</param>
        /// <returns>.</returns>
        internal static string Download(ref DownloadFolderRequest objDownloadFolderRequest)
        {
            string error = string.Empty;
            if (objDownloadFolderRequest != null && objDownloadFolderRequest.Folderinfo!=null && !string.IsNullOrEmpty(objDownloadFolderRequest.Folderinfo.FtpIP) && !string.IsNullOrEmpty(objDownloadFolderRequest.Folderinfo.Username) && !string.IsNullOrEmpty(objDownloadFolderRequest.Folderinfo.Pwd) && objDownloadFolderRequest.Folderinfo.FileList.Count > 0)
            {
                foreach (fileInfoStruct fileInfoStruct in objDownloadFolderRequest.Folderinfo.FileList)
                {
                    DateTime dtCurrentTime = DateTime.Now;
                    if (!string.IsNullOrEmpty(fileInfoStruct.path))
                    {
                        bool downResp = false;
                        while (DateTime.Now.Subtract(dtCurrentTime).TotalMinutes > TimeOut && !downResp)
                        {
                            FtpStatusCode ftpStatusCode;
                            downResp = DownloadFile(objDownloadFolderRequest.DownloadFolderPath, fileInfoStruct.path, objDownloadFolderRequest.Folderinfo.FtpIP, objDownloadFolderRequest.Folderinfo.Username, objDownloadFolderRequest.Folderinfo.Pwd, out ftpStatusCode);
                            if (!downResp && (ftpStatusCode == FtpStatusCode.ServiceNotAvailable || ftpStatusCode == FtpStatusCode.ServiceTemporarilyNotAvailable))
                            {
                                Thread.Sleep(RetryPeriod);
                            }
                            if (!downResp && (ftpStatusCode == FtpStatusCode.ActionNotTakenFileUnavailable || ftpStatusCode == FtpStatusCode.ActionNotTakenFileUnavailableOrBusy))
                            {
                                error = "File " + fileInfoStruct.path + " does not exist.";
                                return error;
                            }
                        }
                        if (DateTime.Now.Subtract(dtCurrentTime).TotalMinutes > TimeOut && !downResp)
                        {
                            error = "T2G Ground Server not responding for " + TimeOut + " period downloading terminated";
                            return error;
                        }
                    }
                    else
                    {
                        error = "Invalid Input parameter";
                        return error;
                    }
                }
            }
            else
            {
                error = "Invalid Input parameter";
            }
            return error;
        }
    }
}
