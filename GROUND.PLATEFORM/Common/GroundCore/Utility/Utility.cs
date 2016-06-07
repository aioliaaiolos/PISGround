/// 
namespace PIS.Ground.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.IO;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.LogMgmt;

    /// <summary>
    /// Utility Class
    /// </summary>
    internal static class T2GConfiguration
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
        /// Holds T2G Service Notification Url name in Web.config
        /// </summary>
        private const string ApplicationId = "ApplicationId";

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

        /// <summary>
        /// Holds the ApplicationId value which is mentoned in Web.config
        /// </summary>
        private static string strApplicationId;

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

        /// <summary>
        /// Gets the T2G Application Id 
        /// </summary>
        public static string T2GApplicationId
        {
            get
            {
                return strApplicationId;
            }
        }

        #endregion

        #region internal Methods
        /// <summary>
        ///  Initialize the SqlLite and Session related configuration from the web.config.
        /// </summary>
        /// <returns> if error return true else false </returns>
        internal static bool InitializeConfigPaths()
        {
            bool error = false;
            // Initilise Session Time Out
            if (ConfigurationManager.AppSettings[SESSIONTIMEOUT] != null)
            {
                try
                {
                    intSessionTimeOut = 0;
                    if (!int.TryParse(ConfigurationManager.AppSettings[SESSIONTIMEOUT], out intSessionTimeOut))
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotValid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                        error = true;
                    }
                    if (intSessionTimeOut < 0)
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotValid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                        error = true;
                    }
                }
                catch 
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                    error = true;
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
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
                        error = true;
                    }
                    else
                    {
                        //// converting to missiseconds
                        intSessionTimerCheck = intSessionTimerCheck * 60000;
                    }
                    if (intSessionTimerCheck < 0)
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotValid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                        error = true;
                    }
                }
                catch 
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                    error = true;
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
            }

            // Initlise SqlLite DB path
            if (ConfigurationManager.AppSettings[SQLLITESESSIONSTOREPATH] != null)
            {
                try
                {
                    strSessionSqLiteDBPath = ConfigurationManager.AppSettings[SQLLITESESSIONSTOREPATH];
                }
                catch
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExConnectionStringNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                    error = true;
                }

                if (string.IsNullOrEmpty(strSessionSqLiteDBPath))
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExConnectionStringNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                    error = true;
                }
                if (!File.Exists(strSessionSqLiteDBPath))
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionDBNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                    error = true;
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExConnectionStringNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                error = true;
            }
            return error;
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
                catch
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_UserNameNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_UserNameNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
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
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_PwdNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
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
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_UserNameNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
            }

            if (ConfigurationManager.AppSettings[ApplicationId] != null)
            {
                try
                {
                    strApplicationId = ConfigurationManager.AppSettings[ApplicationId];
                }
                catch //(ConfigurationErrorsException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2GApplicationIdNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
                    //throw new InvalidOperationException(PIS.Ground.Core.Properties.Resources.LogExceptionT2G_UserNameNotFound, ex);
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2GApplicationIdNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null);
            }
        }

        #endregion
    }
}
