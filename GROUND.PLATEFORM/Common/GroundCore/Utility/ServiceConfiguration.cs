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
    using System.Web;
using System.Text.RegularExpressions;
    using System.Globalization;

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

        /// <summary>The default value for enable filtering on local train service.</summary>
        private const bool DefaultValueForFilterOnLocalTrainService = false;

        /// <summary>The parameter name that enable or disable filtering on local train service.</summary>
        private const string ParameterNameFilterLocalTrainService = "EnableFilterLocalServiceOnly";
        /// <summary>
        /// Holds Sql lite DataBase path value which is mentioned in Web.config
        /// </summary>
        private static string _sessionSqLiteDBPath;

        /// <summary>
        /// Holds Session time out value which is mentioned in Web.config
        /// </summary>
        private static int _sessionTimeOut;

        /// <summary>
        /// Holds Session timer checking value which is mentioned in Web.config
        /// </summary>
        private static long _sessionTimerCheck;

        /// <summary>
        /// Holds T2G Service UserName value which is mentioned in Web.config
        /// </summary>
        private static string _t2gServiceUserName;

        /// <summary>
        /// Holds T2G Service password value which is mentioned in Web.config
        /// </summary>
        private static string _t2gServicePassword;

        /// <summary>
        /// Holds T2G Service Notification url value which is mentioned in Web.config
        /// </summary>
        private static string _t2gServiceNotificationUrl;

        /// <summary>
        /// Holds the ApplicationId value which is mentioned in Web.config
        /// </summary>
        private static string _applicationId;

        /// <summary>
        /// Holds LogLevel name which is mentioned in Web.config
        /// </summary>
        private static string EVENTMGRLOGLEVEL = "LogLevel";

        /// <summary>
        /// Holds the LogLevel value which is mentioned in Web.config
        /// </summary>
        private static TraceType _logLevel = TraceType.ERROR;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the T2GService UserName value
        /// </summary>
        public static string T2GServiceUserName
        {
            get
            {
                return _t2gServiceUserName;
            }
        }

        /// <summary>
        /// Gets the T2GService password check value
        /// </summary>
        public static string T2GServicePwd
        {
            get
            {
                return _t2gServicePassword;
            }
        }

        /// <summary>
        /// Gets the T2GService Notification Url check value
        /// </summary>
        public static string T2GServiceNotificationUrl
        {
            get
            {
                return _t2gServiceNotificationUrl;
            }
        }

        /// <summary>
        /// Gets the SessionTimer check value
        /// </summary>
        public static long SessionTimerCheck
        {
            get
            {
                return _sessionTimerCheck;
            }
        }

        /// <summary>
        /// Gets the SessionTimeOut value
        /// </summary>
        public static int SessionTimeOut
        {
            get
            {
                return _sessionTimeOut;
            }
        }

        /// <summary>
        /// Gets the SqlLite DataBase Path
        /// </summary>
        public static string SessionSqLiteDBPath
        {
            get
            {
                return _sessionSqLiteDBPath;
            }
        }

        /// <summary>
        /// Gets the T2G Application Id
        /// </summary>
        public static string T2GApplicationId
        {
            get
            {
                return _applicationId;
            }
        }

        /// <summary>
        /// Gets the name of the running service.
        /// </summary>
        public static string RunningServiceName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether service shall be filtered for local service only.
        /// </summary>
        public static bool FilterLocalTrainService
        {
            get;
            private set;
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
            // Initialize LogLevel for Event Viewer
            if (ConfigurationManager.AppSettings[EVENTMGRLOGLEVEL] != null)
            {
                try
                {
                    _logLevel = (TraceType)Enum.Parse(typeof(TraceType), ConfigurationSettings.AppSettings["LogLevel"]);
                    if (_logLevel >= TraceType.NONE && _logLevel <= TraceType.EXCEPTION)
                    {
                        LogManager.LogLevel = _logLevel;
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

            // Initialize the application identifier
            InitializeRunningServiceName();

            error |= !InitializeFilterLocalTrainParameter();

            // Initialize Session Time Out
            if (ConfigurationManager.AppSettings[SESSIONTIMEOUT] != null)
            {
                try
                {
                    _sessionTimeOut = 0;
                    if (!int.TryParse(ConfigurationManager.AppSettings[SESSIONTIMEOUT], out _sessionTimeOut))
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimeOutNotValid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                        error = true;
                    }

                    if (_sessionTimeOut < 0)
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
                    _sessionTimerCheck = 0;
                    if (!long.TryParse(ConfigurationManager.AppSettings[SESSIONTIMERCHECK], out _sessionTimerCheck))
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExSessionTimerCheckNotValid, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                        error = true;
                    }
                    else
                    {
                        //// converting to milliseconds
                        _sessionTimerCheck = _sessionTimerCheck * 60000;
                    }

                    if (_sessionTimerCheck < 0)
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
                    _sessionSqLiteDBPath = ConfigurationManager.AppSettings[SQLLITESESSIONSTOREPATH];
                }
                catch
                {
                    LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.ExConnectionStringNotFound, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
                    error = true;
                }

                if (string.IsNullOrEmpty(_sessionSqLiteDBPath))
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
                    _t2gServiceUserName = ConfigurationManager.AppSettings[T2GUSERNAME];
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
                    _t2gServicePassword = ConfigurationManager.AppSettings[T2GPWD];
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
                    _t2gServiceNotificationUrl = ConfigurationManager.AppSettings[T2GNOTIFICATIONURL];
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
                    _applicationId = ConfigurationManager.AppSettings[ApplicationId];
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


            if (LogManager.IsTraceActive(TraceType.INFO))
            {
                string message = string.Empty;
                message += "ServiceConfiguration.SessionSqLiteDBPath=[" + ServiceConfiguration.SessionSqLiteDBPath + "]" + Environment.NewLine;
                message += "ServiceConfiguration.SessionTimeOut=[" + ServiceConfiguration.SessionTimeOut.ToString() + "]" + Environment.NewLine;
                message += "ServiceConfiguration.SessionTimerCheck=[" + ServiceConfiguration.SessionTimerCheck.ToString() + "]" + Environment.NewLine;
                message += "ServiceConfiguration.T2GApplicationId=[" + ServiceConfiguration.T2GApplicationId + "]" + Environment.NewLine;
                message += "ServiceConfiguration.T2GServiceNotificationUrl=[" + ServiceConfiguration.T2GServiceNotificationUrl + "]" + Environment.NewLine;
                message += "ServiceConfiguration.T2GServicePwd=[" + ServiceConfiguration.T2GServicePwd + "]" + Environment.NewLine;
                message += "ServiceConfiguration.T2GServiceUserName=[" + ServiceConfiguration.T2GServiceUserName + "]" + Environment.NewLine;
                message += "ServiceConfiguration.EnableFilterLocalServiceOnly=[" + ServiceConfiguration.FilterLocalTrainService.ToString() + "]" + Environment.NewLine;
                message += "ServiceConfiguration.RunningServiceName=[" + ServiceConfiguration.RunningServiceName + "]" + Environment.NewLine;

                LogManager.WriteLog(TraceType.INFO, message, "PIS.Ground.Core.Utility.InitializeConfigPaths", null, EventIdEnum.GroundCore);
            }

            return error;
        }

        /// <summary>
        /// Initializes the name of the running service.
        /// </summary>
        private static void InitializeRunningServiceName()
        {
            try
            {
                // Usually, the application run into IIS, so HttpContext is initialized.
                HttpContext context = HttpContext.Current;
                if (context != null && context.Request != null)
                {
                    RunningServiceName = context.Request.ApplicationPath.Trim('/');
                }
                else
                {
                    string path = new System.Uri(typeof(ServiceConfiguration).Assembly.CodeBase).LocalPath;
                    
                    // In case that we run into the debugger of Visual studio, web site might be located into a temporary folder.
                    string tempAspNetFiles = Path.Combine(Path.GetTempPath(), "Temporary ASP.NET Files" + Path.DirectorySeparatorChar);

                    if (path.StartsWith(tempAspNetFiles, StringComparison.OrdinalIgnoreCase))
                    {
                        path = path.Substring(tempAspNetFiles.Length);

                        char firstChar = path[0];
                        int foundIndex = path.IndexOfAny(new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }, 0);

                        if (foundIndex > 0)
                        {
                            path = path.Substring(0, foundIndex);
                        }
                    }
                    else
                    {
                        // Run in the context of automated tests.
                        string[] pathSplitted = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                        int index = pathSplitted.Length;
                        if (index > 1)
                        {
                            index -= 2;
                            if (index > 1 && (string.Equals(pathSplitted[index], "Debug", StringComparison.OrdinalIgnoreCase) ||
                                               string.Equals(pathSplitted[index], "Release", StringComparison.OrdinalIgnoreCase)))
                            {
                                index--;
                            }
                            if (index > 1 && string.Equals(pathSplitted[index], "bin", StringComparison.OrdinalIgnoreCase))
                            {
                                index--;
                            }
                        }
                        else
                        {
                            index = 0;
                        }

                        path = pathSplitted[index];
                    }

                    if (!string.IsNullOrEmpty(path))
                    {
                        RunningServiceName = path;
                    }
                    else
                    {
                        RunningServiceName = "PIS.Ground.Core";
                    }
                }
            }
            catch
            {
                RunningServiceName = "PIS.Ground.Core";
            }
        }

        /// <summary>
        /// Loads the parameter filter local train from the configuration file.
        /// </summary>
        /// <returns>Success of the initialization. True if no error has been detected, false otherwise.</returns>
        private static bool InitializeFilterLocalTrainParameter()
        {
            bool success = true;

            string filterValue = ConfigurationManager.AppSettings[ParameterNameFilterLocalTrainService];
            if (string.Equals(filterValue, Boolean.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                FilterLocalTrainService = true;
            }
            else if (string.Equals(filterValue, Boolean.FalseString, StringComparison.OrdinalIgnoreCase))
            {
                FilterLocalTrainService = false;
            }
            else if (string.IsNullOrEmpty(filterValue))
            {
                success = false;
                FilterLocalTrainService = DefaultValueForFilterOnLocalTrainService;
                string errorMessage = string.Format(CultureInfo.CurrentCulture,
                                        Properties.Resources.ConfigurationErrorMissingParameter,
                                        ParameterNameFilterLocalTrainService,
                                        (FilterLocalTrainService ? Boolean.TrueString : Boolean.FalseString));
                string errorContext = string.Format(CultureInfo.CurrentCulture, "{0} of service {1}", "PIS.Ground.Core.Utility.ServiceConfiguration.InitializeFilterLocalTrainParameter", RunningServiceName);
                LogManager.WriteLog(TraceType.ERROR, errorMessage, errorContext, null, EventIdEnum.GroundCore);
            }
            else
            {
                success = false;
                FilterLocalTrainService = DefaultValueForFilterOnLocalTrainService;

                string errorMessage = string.Format(CultureInfo.CurrentCulture,
                                        Properties.Resources.ConfigurationErrorInvalidBooleanValue,
                                        filterValue,
                                        ParameterNameFilterLocalTrainService,
                                        (FilterLocalTrainService ? Boolean.TrueString : Boolean.FalseString),
                                        Boolean.TrueString,
                                        Boolean.FalseString);
                string errorContext = string.Format(CultureInfo.CurrentCulture, "{0} of service {1}", "PIS.Ground.Core.Utility.ServiceConfiguration.InitializeFilterLocalTrainParameter", RunningServiceName);
                LogManager.WriteLog(TraceType.ERROR, errorMessage, errorContext, null, EventIdEnum.GroundCore);
            }

            return success;
        }
    }
}
