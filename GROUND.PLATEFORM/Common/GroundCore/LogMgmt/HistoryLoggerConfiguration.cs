//---------------------------------------------------------------------------------------------------
// <copyright file="HistoryLoggerConfiguration.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.LogMgmt
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.IO;
    using System.ServiceModel.Configuration;
    using System.Web.Configuration;
    using System.Xml;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.Utility;

    /// <summary>
    /// Utility Class
    /// </summary>
    public class HistoryLoggerConfiguration
    {
        #region Private Variable Declaration

        /// <summary>
        /// Holds LogDataBaseStructureVersion in Web.config
        /// </summary>
        private const string DATABASECONFIGPATH = "HistoryLogDBConfigPath";

        /// <summary>
        /// Holds LogDataBaseStructureVersion in Web.config
        /// </summary>
        private const string CREATETABLESCRIPTPATH = "CreateTableScript";
        
        /// <summary>
        /// Holds SqlServerDataDirectory in Web.config
        /// </summary>
        private const string SQLCONNECTIONSTRING = "SqlServerDataDirectory";

        /// <summary>
        /// Holds SqlServerCreateDB in Web.config
        /// </summary>
        private const string SQLCREATEDBCONNECTIONSTRING = "SqlServerCreateDB";

        /// <summary>
        /// Holds SqlServerCreateDB in Web.config
        /// </summary>
        private const string LOGBACKUPPATH = "LogBackUpPath";
        
        /// <summary>
        ///Variable to hold the Sql connection string
        /// </summary>
        private string _connectionString = string.Empty;

        /// <summary>
        ///Variable to hold the Sql connection string
        /// </summary>
        private string _createDbConnectionString = string.Empty;

        /// <summary>
        ///Variable to hold the Sql connection string
        /// </summary>
        private string _logBackupPath = string.Empty;

        /// <summary>
        /// Variable to hold MaxLogMessageSize
        /// </summary>
        private int _maxLogMessageSize = 0;

        /// <summary>
        /// Variable to hold MaxLogMessageCount
        /// </summary>
        private int _maxLogMessageCount = 0;

        /// <summary>
        /// Variable to hold MaxLogMessageCount
        /// </summary>
        private int _percentageOfLogToCleanUpInLogDatabase = 0;

        /// <summary>
        /// Variable to hold LogDataBaseStructureVersion
        /// </summary>
        private string _dataBaseStructureVersion = string.Empty;

        /// <summary>
        /// Variable to hold Database Config Path
        /// </summary>
        private string _dataBaseConfigPath = string.Empty;

        /// <summary>
        /// Variable to hold Script path
        /// </summary>
        private string _createTableScriptPath = string.Empty;
        
        /// <summary>
        /// Variable to hold config validity
        /// </summary>
        private bool _valid = false;

        /// <summary>
        /// Variable to hold whether history log is used or not
        /// </summary>
        private bool _used = false;
        
        /// <summary>
        /// Singleton instance creation lock
        /// </summary>
        private static object _lock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static HistoryLoggerConfiguration _instance = null;
 
        /// <summary>
        /// Variable to hold MaxStringContentLength
        /// </summary>
        private int _maxStringContentLength = 0;

        #endregion

        private static HistoryLoggerConfiguration Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new HistoryLoggerConfiguration();
                        _instance.Initialize();
                    }
                }
                return _instance;
            }
        }       

        #region Properties

        /// <summary>
        /// Gets the validity value
        /// </summary>
        public static bool Valid
        {
            get
            {
                return Instance._valid;
            }
            set { Instance._valid = value; }
        }
        /// <summary>
        /// Gets the used statement
        /// </summary>
        public static bool Used
        {
            get
            {
                return Instance._used;
            }
            set { Instance._used = value; }
        }
        /// <summary>
        /// Gets the MaximumLogMessageSize value
        /// </summary>
        public static int PercentageToCleanUpInLogDatabase
        {
            get
            {
                return Instance._percentageOfLogToCleanUpInLogDatabase;
            }
            set { Instance._percentageOfLogToCleanUpInLogDatabase = value; }
        }

        /// <summary>
        /// Gets the LogDataBaseStructureVersion value
        /// </summary>
        public static string DataBaseConfigPath
        {
            get
            {
                return Instance._dataBaseConfigPath;
            }
        }

        /// <summary>
        /// Gets the CreateTableScriptPath value
        /// </summary>
        public static string CreateTableScriptPath
        {
            get
            {
                return Instance._createTableScriptPath;
            }
            set { Instance._createTableScriptPath = value; }
        }

        /// <summary>
        /// Gets the LogDataBaseStructureVersion value
        /// </summary>
        public static string LogDataBaseStructureVersion
        {
            get
            {
                return Instance._dataBaseStructureVersion;
            }
            set { Instance._dataBaseStructureVersion = value; }
        }

        /// <summary>
        /// Gets the LogBackupPath value
        /// </summary>
        public static string LogBackupPath
        {
            get
            {
                return Instance._logBackupPath;
            }
            set { Instance._logBackupPath = value; }
        }

        /// <summary>
        /// Gets the MaximumLogMessageSize value
        /// </summary>
        public static int MaximumLogMessageSize
        {
            get
            {
                return Instance._maxLogMessageSize;
            }
            set { Instance._maxLogMessageSize = value; }
        }

        /// <summary>
        /// Gets the MaximumLogMessageCount value
        /// </summary>
        public static int MaximumLogMessageCount
        {
            get
            {
                return Instance._maxLogMessageCount;
            }
        }

        /// <summary>
        /// Gets the Sql Connection String
        /// </summary>
        public static string SqlConnectionString
        {
            get
            {
                return Instance._connectionString;
            }
            set { Instance._connectionString = value; }
        }

        /// <summary>
        /// Gets the Sql Create DB Connection String
        /// </summary>
        public static string SqlCreateDbConnectionString
        {
            get
            {
                return Instance._createDbConnectionString;
            }
            set { Instance._createDbConnectionString = value; }
        }

        /// <summary>
        /// Gets the Maximum String Content Length
        /// </summary>
        public static int MaxStringContentLength
        {
            get
            {
                return Instance._maxStringContentLength;
            }
        }

        #endregion

        #region internal Methods

        /// <summary>
        ///  Initialize History Logger configuration from the web.config.
        /// </summary>
        /// <returns> if error return true else false </returns>
        private void Initialize()
        {
            _valid = false;
            _used = false;

            bool error = false;

            try
            {

				bool isLoadingFromWebApplication = AppDomain.CurrentDomain.GetData("DataDirectory") != null;;

                if (ConfigurationManager.AppSettings[DATABASECONFIGPATH] != null)
                {
                    _used = true;
                    try
                    {
                        try
                        {
                            _dataBaseConfigPath = ConfigurationManager.AppSettings[DATABASECONFIGPATH];
                            _dataBaseConfigPath = _dataBaseConfigPath.Replace("|DataDirectory|", ServiceConfiguration.AppDataPath);

                            ReadDBConfigFile();
                        }
                        catch (Exception)
                        {
                            LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogDataBaseConfigFile, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.Initialize", null, EventIdEnum.GroundCore);
                            error = true;
                        }
                    }
                    catch
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogDataBaseConfigFile, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.Initialize", null, EventIdEnum.GroundCore);
                        error = true;
                    }
                }

                if (ConfigurationManager.AppSettings[LOGBACKUPPATH] != null)
                {
                    try
                    {
                        try
                        {
                            _logBackupPath = ConfigurationManager.AppSettings[LOGBACKUPPATH];
                            _logBackupPath = _logBackupPath.Replace("|DataDirectory|", ServiceConfiguration.AppDataPath);
                        }
                        catch (Exception)
                        {
                            LogManager.WriteLog(TraceType.ERROR, "LogBackUpPath in web.config not valid", "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.Initialize", null, EventIdEnum.GroundCore);
                            error = true;
                        }
                    }
                    catch
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogDataBaseConfigFile, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.Initialize", null, EventIdEnum.GroundCore);
                        error = true;
                    }
                }

                if (ConfigurationManager.AppSettings[CREATETABLESCRIPTPATH] != null)
                {
                    try
                    {
                        try
                        {
                            _createTableScriptPath = ConfigurationManager.AppSettings[CREATETABLESCRIPTPATH];
                            _createTableScriptPath = _createTableScriptPath.Replace("|DataDirectory|", ServiceConfiguration.AppDataPath);
                        }
                        catch (Exception)
                        {
                            LogManager.WriteLog(TraceType.ERROR, "CreateTableScript in web.config not valid", "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.Initialize", null, EventIdEnum.GroundCore);
                            error = true;
                        }
                    }
                    catch
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogDataBaseConfigFile, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.Initialize", null, EventIdEnum.GroundCore);
                        error = true;
                    }
                }

                if (ConfigurationManager.ConnectionStrings[SQLCONNECTIONSTRING] != null)
                {
                    try
                    {
                        _connectionString = ConfigurationManager.ConnectionStrings[SQLCONNECTIONSTRING].ConnectionString;
                        SqlConnectionStringBuilder stringBuiilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["SqlServerDataDirectory"].ConnectionString);
                        if (stringBuiilder != null && stringBuiilder.AttachDBFilename != null && stringBuiilder.AttachDBFilename.Contains("|DataDirectory|"))
                        {
                            string configConvertedPath = stringBuiilder.AttachDBFilename.Replace("|DataDirectory|", ServiceConfiguration.AppDataPath);
                            //if (File.Exists(configConvertedPath))
                            try
                            {
                                FileInfo file = new FileInfo(configConvertedPath);
                                stringBuiilder.AttachDBFilename = file.FullName;
                                _connectionString = stringBuiilder.ConnectionString;
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch
                    {
                        LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogSqlServerDataDirectoryError, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.Initialize", null, EventIdEnum.GroundCore);
                        error = true;
                    }
                }

                if (ConfigurationManager.ConnectionStrings[SQLCREATEDBCONNECTIONSTRING] != null)
                {
                    try
                    {
                        _createDbConnectionString = ConfigurationManager.ConnectionStrings[SQLCREATEDBCONNECTIONSTRING].ConnectionString;

                    }
                    catch
                    {
                        LogManager.WriteLog(TraceType.ERROR, "Error in connection string " + SQLCREATEDBCONNECTIONSTRING, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.Initialize", null, EventIdEnum.GroundCore);
                        error = true;
                    }
                }

                try
                {
					ServiceModelSectionGroup section;
					if (isLoadingFromWebApplication)
					{
						Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
						section = config.GetSectionGroup("system.serviceModel") as ServiceModelSectionGroup;
					}
					else
					{
						section = ConfigurationManager.GetSection("system.serviceModel") as ServiceModelSectionGroup;
					}

					if (section != null)
					{
						CustomBindingElementCollection customBindingElements = section.Bindings.CustomBinding.Bindings;
						for (int customElements = 0; customElements < customBindingElements.Count; customElements++)
						{
							CustomBindingElement customBindingElement = customBindingElements[customElements];
							if (customBindingElement.Name == "MaintenanceBinding")
							{
								ElementInformation txtMessageEncodingElementInfo = customBindingElement.ElementInformation;
								TextMessageEncodingElement txtMessageEncodingElement = (TextMessageEncodingElement)txtMessageEncodingElementInfo.Properties["textMessageEncoding"].Value;
								ElementInformation readerQuotasElementInfo = txtMessageEncodingElement.ElementInformation;
								XmlDictionaryReaderQuotasElement readerQuotasElement = (XmlDictionaryReaderQuotasElement)readerQuotasElementInfo.Properties["readerQuotas"].Value;
								_maxStringContentLength = readerQuotasElement.MaxStringContentLength;
								break;
							}
						}
					}
                }
                catch
                {
                    LogManager.WriteLog(TraceType.ERROR, "Error in retrieving max String Content Length", "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.Initialize", null, EventIdEnum.GroundCore);
                    error = true;
                }

                if (!error)
                {
                    string message = "Initialize success:"
                        + " DataBaseConfigPath=" + DataBaseConfigPath
                        + " LogDataBaseStructureVersion=" + LogDataBaseStructureVersion
                        + " PercentageToCleanUpInLogDatabase=" + PercentageToCleanUpInLogDatabase
                        + " MaximumLogMessageSize=" + MaximumLogMessageSize
                        + " MaximumLogMessageCount=" + MaximumLogMessageCount
                        + " SqlConnectionString=" + SqlConnectionString
                        + " SqlCreateDbConnectionString=" + SqlCreateDbConnectionString
                        + " LogBackupPath=" + LogBackupPath
                        + " CreateTableScriptPath=" + CreateTableScriptPath;

                    LogManager.WriteLog(TraceType.INFO, message, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.Initialize", null, EventIdEnum.GroundCore);
                }
            }
            catch
            {
                LogManager.WriteLog(TraceType.ERROR, "Error Initializing HistoryLoggerConfiguration", "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.Initialize", null, EventIdEnum.GroundCore);
            }
            
            _valid = !error;            
        }       

        /// <summary>
        /// read database config file
        /// </summary>
        private void ReadDBConfigFile()
        {
            XmlDocument xml = new XmlDocument();

            if (File.Exists(_dataBaseConfigPath))
            {
                xml.Load(_dataBaseConfigPath);
                if (xml != null)
                {
                    XmlNode node = xml.SelectSingleNode("/Config/DatabaseCleanupConfig/MaxNumberOfScheduledMessageInLogDatabase");
                    if (node != null)
                    {
                        try
                        {
                            _maxLogMessageCount = 0;
                            if (!int.TryParse(node.InnerText, out _maxLogMessageCount))
                            {
                                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogMaxMessageSizeNotValid, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.ReadDBConfigFile", null, EventIdEnum.GroundCore);
                            }

                            if (_maxLogMessageCount <= 0)
                            {
                                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogMaxMessageSizeNotValid, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.ReadDBConfigFile", null, EventIdEnum.GroundCore);
                            }
                        }
                        catch
                        {
                            LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogMaxMessageSizeNotValid, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.ReadDBConfigFile", null, EventIdEnum.GroundCore);
                        }
                    }

                    node = xml.SelectSingleNode("/Config/DatabaseCleanupConfig/PercentageOfMaxNumberOfScheduledMessageToCleanUpInLogDatabase");
                    if (node != null)
                    {
                        try
                        {
                            _percentageOfLogToCleanUpInLogDatabase = 0;
                            if (!int.TryParse(node.InnerText, out _percentageOfLogToCleanUpInLogDatabase))
                            {
                                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogPercentageOfLogToCleanUpInLogDatabase, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.ReadDBConfigFile", null, EventIdEnum.GroundCore);
                            }

                            if (_percentageOfLogToCleanUpInLogDatabase <= 0)
                            {
                                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogPercentageOfLogToCleanUpInLogDatabase, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.ReadDBConfigFile", null, EventIdEnum.GroundCore);
                            }
                        }
                        catch
                        {
                            LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogPercentageOfLogToCleanUpInLogDatabase, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.ReadDBConfigFile", null, EventIdEnum.GroundCore);
                        }
                    }

                    node = xml.SelectSingleNode("/Config/GetLogConfig/MaxNumberOfScheduledMessageInGetLog");
                    if (node != null)
                    {
                        try
                        {
                            _maxLogMessageSize = 0;
                            if (!int.TryParse(node.InnerText, out _maxLogMessageSize))
                            {
                                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogMaxMessageSizeNotValid, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.ReadDBConfigFile", null, EventIdEnum.GroundCore);
                            }

                            if (_maxLogMessageSize <= 0)
                            {
                                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogMaxMessageSizeNotValid, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.ReadDBConfigFile", null, EventIdEnum.GroundCore);
                            }
                        }
                        catch
                        {
                            LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.LogMaxMessageSizeNotValid, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.ReadDBConfigFile", null, EventIdEnum.GroundCore);
                        }
                    }
                    node = xml.SelectSingleNode("/Config/DatabaseConfig/DatabaseVersion");
                    if (node != null)
                    {
                        try
                        {
                            _dataBaseStructureVersion = node.InnerText;
                            if (string.IsNullOrEmpty(_dataBaseStructureVersion))
                            {
                                LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.DataBaseStructureVersion, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.ReadDBConfigFile", null, EventIdEnum.GroundCore);                                
                            }                            
                        }
                        catch
                        {
                            LogManager.WriteLog(TraceType.ERROR, PIS.Ground.Core.Properties.Resources.DataBaseStructureVersion, "PIS.Ground.Core.LogMgmt.HistoryLoggerConfiguration.ReadDBConfigFile", null, EventIdEnum.GroundCore);                            
                        }
                    }
                }
            }
        }

        #endregion
    }        
}
