/// 
namespace PIS.Ground.Core
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Net;
    using System.Threading;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.LogMgmt;

    /// <summary>
    /// Utility Class
    /// </summary>
    internal static class Utility
    {
        #region Private Variable Decleration
        /// <summary>
        /// Holds Sql lite DataBase path name in Web.config
        /// </summary>
        private const string SQLLITESESSIONSTOREPATH = "SqlLiteSessionStorePath";

        /// <summary>
        /// Holds Session time out name in Web.config
        /// </summary>
        private const string SESSIONTIMEOUT = "SessionTimeOut";

        /// <summary>
        /// Holds Session time check out name in Web.config
        /// </summary>
        private const string SESSIONTIMERCHECK = "SessionCheckTimer";

        /// <summary>
        /// Holds T2G Service UserName name in Web.config
        /// </summary>
        private const string T2GUSERNAME = "T2GServiceUserName";

        /// <summary>
        /// Holds T2G Service password name in Web.config
        /// </summary>
        private const string T2GPWD = "T2G_Password";

        /// <summary>
        /// Holds T2G Service Notification Url name in Web.config
        /// </summary>
        private const string T2GNOTIFICATIONURL = "T2G_NotificationUrl";

        /// <summary>
        /// Holds Sql lite DataBase path value which is mentoned in Web.config
        /// </summary>
        private static string strSessionSqLiteDBPath;

        /// <summary>
        /// Holds Session time out value which is mentoned in Web.config
        /// </summary>
        private static int intSessionTimeOut;

        /// <summary>
        /// Holds Session timeer checking value which is mentoned in Web.config
        /// </summary>
        private static long intSessionTimerCheck;

        /// <summary>
        /// Holds T2G Service UserName value which is mentoned in Web.config
        /// </summary>
        private static string strT2GServiceUserName;

        /// <summary>
        /// Holds T2G Service password value which is mentoned in Web.config
        /// </summary>
        private static string strT2GServicePwd;

        /// <summary>
        /// Holds T2G Service Notification url value which is mentoned in Web.config
        /// </summary>
        private static string strT2GServiceNotificataionUrl;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the T2GService UserName value
        /// </summary>
        public static string T2GServiceUserName
        {
            get
            {
                return strT2GServiceUserName;
            }
        }

        /// <summary>
        /// Gets the T2GService password check value
        /// </summary>
        public static string T2GServicePwd
        {
            get
            {
                return strT2GServicePwd;
            }
        }

        /// <summary>
        /// Gets the T2GService Notification Url check value
        /// </summary>
        public static string T2GServiceNotificataionUrl
        {
            get
            {
                return strT2GServiceNotificataionUrl;
            }
        }

        /// <summary>
        /// Gets the SessionTimer check value
        /// </summary>
        public static long SessionTimerCheck
        {
            get
            {
                return intSessionTimerCheck;
            }
        }

        /// <summary>
        /// Gets the SessionTimeOut value
        /// </summary>
        public static int SessionTimeOut
        {
            get
            {
                return intSessionTimeOut;
            }
        }

        /// <summary>
        /// Gets the SqlLite DataBase Path
        /// </summary>
        public static string SessionSqLiteDBPath
        {
            get
            {
                return strSessionSqLiteDBPath;
            }
        }

        #endregion

        #region internal Methods
        /// <summary>
        ///  Initialize the SqlLite and Session related configuration from the web.config.
        /// </summary>
        internal static void InitializeConfigPaths()
        {
            // Initilise Session Time Out
            if (ConfigurationManager.AppSettings[SESSIONTIMEOUT] != null)
            {
                try
                {
                    intSessionTimeOut = 0;
                    if (!int.TryParse(ConfigurationManager.AppSettings[SESSIONTIMEOUT], out intSessionTimeOut))
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotValid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                        //throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotValid);
                    }
                    if (intSessionTimeOut < 0)
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotValid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                        //throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotValid);
                    }
                }
                catch //(ConfigurationErrorsException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                    //throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotFound, ex);
                }
            }

            // Initlise Session Timer check
            if (ConfigurationManager.AppSettings[SESSIONTIMERCHECK] != null)
            {
                try
                {
                    intSessionTimerCheck = 0;
                    if (!long.TryParse(ConfigurationManager.AppSettings[SESSIONTIMERCHECK], out intSessionTimerCheck))
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotValid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                        //throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotValid);
                    }
                    else
                    {
                        // converting to missiseconds
                        intSessionTimerCheck = intSessionTimerCheck * 60000;
                    }
                    if (intSessionTimerCheck < 0)
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotValid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                        //throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotValid);
                    }
                }
                catch //(ConfigurationErrorsException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                    //throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotFound, ex);
                }
            }

            // Initlise SqlLite DB path
            if (ConfigurationManager.AppSettings[SQLLITESESSIONSTOREPATH] != null)
            {
                try
                {
                    strSessionSqLiteDBPath = ConfigurationManager.AppSettings[SQLLITESESSIONSTOREPATH];
                }
                catch //(ConfigurationErrorsException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExConnectionStringNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                    //throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.ExConnectionStringNotFound, ex);
                }

                if (string.IsNullOrEmpty(strSessionSqLiteDBPath))
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExConnectionStringNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                    //throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.ExConnectionStringNotFound);
                }
                if (!File.Exists(strSessionSqLiteDBPath))
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionDBNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                    //throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.ExSessionDBNotFound);
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExConnectionStringNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                //throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.ExConnectionStringNotFound);
            }
        }

        /// <summary>
        ///  Initialize the T2G related configuration from the web.config.
        /// </summary>
        internal static void InitializeT2GConfig()
        {
            if (ConfigurationManager.AppSettings[T2GUSERNAME] != null)
            {
                try
                {
                    strT2GServiceUserName = ConfigurationManager.AppSettings[T2GUSERNAME];
                }
                catch //(ConfigurationErrorsException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_UserNameNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                    //throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.LogExceptionT2G_UserNameNotFound, ex);
                }
            }

            if (ConfigurationManager.AppSettings[T2GPWD] != null)
            {
                try
                {
                    strT2GServicePwd = ConfigurationManager.AppSettings[T2GPWD];
                }
                catch //(ConfigurationErrorsException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_PwdNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                   // throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.LogExceptionT2G_PwdNotFound, ex);
                }
            }

            if (ConfigurationManager.AppSettings[T2GNOTIFICATIONURL] != null)
            {
                try
                {
                    strT2GServiceNotificataionUrl = ConfigurationManager.AppSettings[T2GNOTIFICATIONURL];
                }
                catch //(ConfigurationErrorsException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_UserNameNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                    //throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.LogExceptionT2G_UserNameNotFound, ex);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    internal class FtpState
    {
        private ManualResetEvent wait;
        private FtpWebRequest request;
        private string fileName;
        private Exception operationException = null;
        string status;

        public FtpState()
        {
            wait = new ManualResetEvent(false);
        }

        public ManualResetEvent OperationComplete
        {
            get { return wait; }
        }

        public FtpWebRequest Request
        {
            get { return request; }
            set { request = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public Exception OperationException
        {
            get { return operationException; }
            set { operationException = value; }
        }
        public string StatusDescription
        {
            get { return status; }
            set { status = value; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class FtpUpLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objUploadFileDistributionRequest"></param>
        internal static void AsynchronousUploadFiles(ref UploadFileDistributionRequest objUploadFileDistributionRequest)
        {
            foreach (string strfilepath in objUploadFileDistributionRequest.FilePathList)
            {
                // Create a Uri instance with the specified URI string.
                // If the URI is not correctly formed, the Uri constructor
                // will throw an exception.
                ManualResetEvent waitObject;
                if (File.Exists(strfilepath))
                {
                    FileInfo objFileInfo = new FileInfo(strfilepath);
                    string uri = "ftp://" + objUploadFileDistributionRequest.FtpServerIP + "/" + objUploadFileDistributionRequest.FtpDirectory + "/" + objFileInfo.Name;
                    Uri target = new Uri(uri);
                    FtpState state = new FtpState();
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(target);
                    request.Method = WebRequestMethods.Ftp.UploadFile;

                    // This example uses anonymous logon.
                    // The request is anonymous by default; the credential does not have to be specified. 
                    // The example specifies the credential only to
                    // control how actions are logged on the server.

                    request.Credentials = new NetworkCredential(objUploadFileDistributionRequest.FtpUserName, objUploadFileDistributionRequest.FtpPassword);

                    // Store the request in the object that we pass into the
                    // asynchronous operations.
                    state.Request = request;
                    state.FileName = strfilepath;

                    // Get the event to wait on.
                    waitObject = state.OperationComplete;

                    // Asynchronously get the stream for the file contents.
                    request.BeginGetRequestStream(
                        new AsyncCallback(EndGetStreamCallback),
                        state
                    );

                    // Block the current thread until all operations are complete.
                    waitObject.WaitOne();

                    // The operations either completed or threw an exception.
                    if (state.OperationException != null)
                    {
                        objUploadFileDistributionRequest.FileOperationStatus.Add(strfilepath, state.OperationException.Message);
                    }
                    else
                    {
                        objUploadFileDistributionRequest.FileOperationStatus.Add(strfilepath, state.StatusDescription);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private static void EndGetStreamCallback(IAsyncResult ar)
        {
            FtpState state = (FtpState)ar.AsyncState;

            Stream requestStream = null;
            // End the asynchronous call to get the request stream.
            try
            {
                requestStream = state.Request.EndGetRequestStream(ar);
                // Copy the file contents to the request stream.
                const int bufferLength = 2048;
                byte[] buffer = new byte[bufferLength];
                int count = 0;
                int readBytes = 0;
                FileStream stream = File.OpenRead(state.FileName);
                do
                {
                    readBytes = stream.Read(buffer, 0, bufferLength);
                    requestStream.Write(buffer, 0, readBytes);
                    count += readBytes;
                }
                while (readBytes != 0);
                // Close the request stream before sending the request.
                requestStream.Close();
                // Asynchronously get the response to the upload request.
                state.Request.BeginGetResponse(
                    new AsyncCallback(EndGetResponseCallback),
                    state
                );
            }
            // Return exceptions to the main application thread.
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExFtpError, "PIS.Ground.Core.Utility.EndGetStreamCallback", ex);
                state.OperationException = ex;
                state.OperationComplete.Set();
                return;
            }

        }

        // The EndGetResponseCallback method  
        // completes a call to BeginGetResponse.
        private static void EndGetResponseCallback(IAsyncResult ar)
        {
            FtpState state = (FtpState)ar.AsyncState;
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)state.Request.EndGetResponse(ar);
                response.Close();
                state.StatusDescription = response.StatusDescription;
                // Signal the main application thread that 
                // the operation is complete.
                state.OperationComplete.Set();
            }
            // Return exceptions to the main application thread.
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExFtpError, "PIS.Ground.Core.Utility.EndGetResponseCallback", ex);
                state.OperationException = ex;
                state.OperationComplete.Set();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objUploadFileDistributionRequest"></param>
        internal static void Upload(ref UploadFileDistributionRequest objUploadFileDistributionRequest)
        {
            objUploadFileDistributionRequest.FileOperationStatus = new Dictionary<string, string>();
            foreach (string strfilepath in objUploadFileDistributionRequest.FilePathList)
            {
                if (File.Exists(strfilepath))
                {
                    FileInfo fileInf = new FileInfo(strfilepath);
                    string uri = "ftp://" + objUploadFileDistributionRequest.FtpServerIP + "/" + objUploadFileDistributionRequest.FtpDirectory + "/" + fileInf.Name;
                    FtpWebRequest reqFTP;

                    // Create FtpWebRequest object from the Uri provided

                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

                    // Provide the WebPermission Credintials

                    reqFTP.Credentials = new NetworkCredential(objUploadFileDistributionRequest.FtpUserName,
                                                               objUploadFileDistributionRequest.FtpPassword);

                    // By default KeepAlive is true, where the control connection is 

                    // not closed after a command is executed.

                    reqFTP.KeepAlive = false;

                    // Specify the command to be executed.

                    reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

                    // Specify the data transfer type.

                    reqFTP.UseBinary = true;

                    // Notify the server about the size of the uploaded file

                    reqFTP.ContentLength = fileInf.Length;

                    // The buffer size is set to 2kb

                    int buffLength = 2048;
                    byte[] buff = new byte[2048];
                    int contentLen;

                    // Opens a file stream (System.IO.FileStream) to read   the file to be uploaded
                    FileStream fs = fileInf.OpenRead();

                    try
                    {
                        // Stream to which the file to be upload is written

                        Stream strm = reqFTP.GetRequestStream();

                        // Read from the file stream 2kb at a time

                        contentLen = fs.Read(buff, 0, buffLength);

                        // Till Stream content ends

                        while (contentLen != 0)
                        {
                            // Write Content from the file stream to the 

                            // FTP Upload Stream

                            strm.Write(buff, 0, contentLen);
                            contentLen = fs.Read(buff, 0, buffLength);
                        }

                        // Close the file stream and the Request Stream

                        strm.Close();
                        fs.Close();
                        if (reqFTP != null)
                        {
                            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                            if (objUploadFileDistributionRequest.FileOperationStatus.ContainsKey(strfilepath))
                            {
                                objUploadFileDistributionRequest.FileOperationStatus[strfilepath] = response.StatusDescription;
                            }
                            else
                            {
                                objUploadFileDistributionRequest.FileOperationStatus.Add(strfilepath, response.StatusDescription);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (objUploadFileDistributionRequest.FileOperationStatus.ContainsKey(strfilepath))
                        {
                            objUploadFileDistributionRequest.FileOperationStatus[strfilepath] = ex.Message;
                        }
                        else
                        {
                            objUploadFileDistributionRequest.FileOperationStatus.Add(strfilepath, ex.Message);
                        }

                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExFtpError, "PIS.Ground.Core.Utility.EndGetStreamCallback", ex);
                    }
                }
            }
        }
    }
}
