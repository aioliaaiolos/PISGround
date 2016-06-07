//---------------------------------------------------------------------------------------------------
// <copyright file="ServiceConfiguration.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.Utility
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.IO;
    using System.Xml;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.LogMgmt;

    /// <summary>
    /// Utility Class
    /// </summary>
    public static class ServiceConfiguration
    {
        #region Private Variable Declaration

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
        /// Holds T2G Service list in Web.config
        /// </summary>
        private const string T2GSERVICEID = "T2G_ServiceID";

        /// <summary>
        /// Holds Sql lite DataBase path value which is mentioned in Web.config
        /// </summary>
        private static string strSessionSqLiteDBPath;

        /// <summary>
        /// Holds Session time out value which is mentioned in Web.config
        /// </summary>
        private static int intSessionTimeOut;

        /// <summary>
        /// Holds Session timer checking value which is mentioned in Web.config
        /// </summary>
        private static long intSessionTimerCheck;

        /// <summary>
        /// Holds T2G Service UserName value which is mentioned in Web.config
        /// </summary>
        private static string strT2GServiceUserName;

        /// <summary>
        /// Holds T2G Service password value which is mentioned in Web.config
        /// </summary>
        private static string strT2GServicePwd;

        /// <summary>
        /// Holds T2G Service Notification url value which is mentioned in Web.config
        /// </summary>
        private static string strT2GServiceNotificataionUrl;

        /// <summary>
        /// Holds the ApplicationId value which is mentioned in Web.config
        /// </summary>
        private static string strApplicationId;

        /// <summary>
        /// Holds LogLevel name which is mentioned in Web.config
        /// </summary>
        private static string EVENTMGRLOGLEVEL = "LogLevel";

        /// <summary>
        /// Holds the LogLevel value which is mentioned in Web.config
        /// </summary>
        private static TraceType enmLogLevel = TraceType.NONE;

        #endregion

        #region Public properties

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
        public static string T2GServiceNotificationUrl
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

        static ServiceConfiguration()
        {
            Initialize();
        }

        /// <summary>
        ///  Initialize the service-related configuration from the web.config.
        /// </summary>
        /// <returns> if error return true else false </returns>
        public static bool Initialize()
        {
            bool error = false;
            // Initialize LogLovel for Event Viewer
            if (ConfigurationManager.AppSettings[EVENTMGRLOGLEVEL] != null)
            {
                try
                {
                    enmLogLevel = (TraceType)Enum.Parse(typeof(TraceType), ConfigurationSettings.AppSettings["LogLevel"]);
                    if (enmLogLevel >= TraceType.NONE && enmLogLevel <= TraceType.WARNING)
                    {
                        LogManager.LogLevel = enmLogLevel;
                    }
                    else
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.EventLogLevelValueInvalid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                        error = true;
                    }
                }
                catch
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.EventLogLevelValueInvalid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                    error = true;
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.EventLogLevelValueInvalid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                error = true;
            }

            // Initialize Session Time Out
            if (ConfigurationManager.AppSettings[SESSIONTIMEOUT] != null)
            {
                try
                {
                    intSessionTimeOut = 0;
                    if (!int.TryParse(ConfigurationManager.AppSettings[SESSIONTIMEOUT], out intSessionTimeOut))
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotValid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                        error = true;
                    }

                    if (intSessionTimeOut < 0)
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotValid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                        error = true;
                    }
                }
                catch
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                    error = true;
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                error = true;
            }

            // Initialize Session Timer check
            if (ConfigurationManager.AppSettings[SESSIONTIMERCHECK] != null)
            {
                try
                {
                    intSessionTimerCheck = 0;
                    if (!long.TryParse(ConfigurationManager.AppSettings[SESSIONTIMERCHECK], out intSessionTimerCheck))
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotValid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                        error = true;
                    }
                    else
                    {
                        //// converting to milliseconds
                        intSessionTimerCheck = intSessionTimerCheck * 60000;
                    }

                    if (intSessionTimerCheck < 0)
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotValid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                        error = true;
                    }
                }
                catch
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                    error = true;
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                error = true;
            }

            // Initialize SqlLite DB path
            if (ConfigurationManager.AppSettings[SQLLITESESSIONSTOREPATH] != null)
            {
                try
                {
                    strSessionSqLiteDBPath = ConfigurationManager.AppSettings[SQLLITESESSIONSTOREPATH];
                }
                catch
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExConnectionStringNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                    error = true;
                }

                if (string.IsNullOrEmpty(strSessionSqLiteDBPath))
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExConnectionStringNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                    error = true;
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExConnectionStringNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                error = true;
            }

            if (ConfigurationManager.AppSettings[T2GUSERNAME] != null)
            {
                try
                {
                    strT2GServiceUserName = ConfigurationManager.AppSettings[T2GUSERNAME];
                }
                catch
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_UserNameNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                    error = true;
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_UserNameNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                error = true;
            }

            if (ConfigurationManager.AppSettings[T2GPWD] != null)
            {
                try
                {
                    strT2GServicePwd = ConfigurationManager.AppSettings[T2GPWD];
                }
                catch
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_PwdNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                    error = true;
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_PwdNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                error = true;
            }

            if (ConfigurationManager.AppSettings[T2GNOTIFICATIONURL] != null)
            {
                try
                {
                    strT2GServiceNotificataionUrl = ConfigurationManager.AppSettings[T2GNOTIFICATIONURL];
                }
                catch
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_NotificationUrlNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                    error = true;
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_NotificationUrlNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                error = true;
            }

            if (ConfigurationManager.AppSettings[ApplicationId] != null)
            {
                try
                {
                    strApplicationId = ConfigurationManager.AppSettings[ApplicationId];
                }
                catch
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2GApplicationIdNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                    error = true;
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2GApplicationIdNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                error = true;
            }

            /* TODO: Add eventually.
            if (ConfigurationManager.AppSettings[T2GSERVICEID] != null)
            {
                try
                {
                    intT2GServiceID = 0;
                    if (!int.TryParse(ConfigurationManager.AppSettings[T2GSERVICEID], out intT2GServiceID))
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_ServiceIDNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                        error = true;
                    }
                    if (intSessionTimeOut < 0)
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_ServiceIDNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                        error = true;
                    }
                }
                catch
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_ServiceIDNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                    error = true;
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogExceptionT2G_ServiceIDNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                error = true;
            }
            */

            if (!error)
            {
                LogManager.WriteLog(TraceType.INFO, "Initialize success.", "PIS.Ground.Core.Utility.Initialize", null, EventIdEnum.GroundCore);
            }


            string message = string.Empty;
            message += "ServiceConfiguration.SessionSqLiteDBPath=[" + ServiceConfiguration.SessionSqLiteDBPath + "]" + Environment.NewLine;
            message += "ServiceConfiguration.SessionTimeOut=[" + ServiceConfiguration.SessionTimeOut.ToString() + "]" + Environment.NewLine;
            message += "ServiceConfiguration.SessionTimerCheck=[" + ServiceConfiguration.SessionTimerCheck.ToString() + "]" + Environment.NewLine;
            message += "ServiceConfiguration.T2GApplicationId=[" + ServiceConfiguration.T2GApplicationId + "]" + Environment.NewLine;
            message += "ServiceConfiguration.T2GServiceNotificationUrl=[" + ServiceConfiguration.T2GServiceNotificationUrl + "]" + Environment.NewLine;
            message += "ServiceConfiguration.T2GServicePwd=[" + ServiceConfiguration.T2GServicePwd + "]" + Environment.NewLine;
            message += "ServiceConfiguration.T2GServiceUserName=[" + ServiceConfiguration.T2GServiceUserName + "]" + Environment.NewLine;

            LogManager.WriteLog(TraceType.INFO, message, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);

            return error;
        }
    }
}
