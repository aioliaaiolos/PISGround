//---------------------------------------------------------------------------------------------------
// <copyright file="HistoryLogger.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.LogMgmt
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Xml;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.SqlServerAccess;
    using PIS.Ground.Core.Utility;
    using System.Configuration;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Definition of History logger
    /// </summary>
    public class HistoryLogger
    {
        #region Fields

        /// <summary>
        /// Stored procedure name sp_InsertUpdateDatabaseVersion
        /// </summary>
        private const string SpInsertUpdateDatabaseVersion = "sp_InsertUpdateDatabaseVersion";

        /// <summary>
        /// Stored procedure name sp_GetDatabaseVersion
        /// </summary>
        private const string SpGetDatabaseVersion = "sp_GetDatabaseVersion";

        /// <summary>
        /// Stored procedure name sp_InsertScheduleMessage
        /// </summary>
        private const string SpInsertScheduleMessage = "sp_InsertLogMessage";

        /// <summary>
        /// Stored procedure name sp_UpdateMessageRequest
        /// </summary>
        private const string SpUpdateMessageRequest = "sp_UpdateMessageRequest";

        /// <summary>
        /// Stored procedure name sp_DeleteMessage
        /// </summary>
        private const string SpDeleteMessage = "sp_DeleteAllMessage";

		/// <summary>
        /// Stored procedure name sp_DeletePendingMessages
        /// </summary>
		private const string SpDeletePendingMessages = "sp_DeletePendingMessages";

        /// <summary>
        /// Stored procedure name sp_GetOldestMessage
        /// </summary>
        private const string SpGetOldestMessage = "sp_GetOldestMessage";

        /// <summary>
        /// Stored procedure name sp_GetOldestMessageDateTime
        /// </summary>
        private const string SpGetOldestMessageDateTime = "sp_GetOldestMessageDateTime";

        /// <summary>
        /// Stored procedure name sp_GetNewestMessage
        /// </summary>
        private const string SpGetNewestMessage = "sp_GetNewestMessage";

        /// <summary>
        /// Stored procedure name sp_GetNewestMessageDateTime
        /// </summary>
        private const string SpGetNewestMessageDateTime = "sp_GetNewestMessageDateTime";

        /// <summary>
        /// Stored procedure name sp_GetAllMessages
        /// </summary>
        private const string SpGetAllMessages = "sp_GetAllMessages";

        /// <summary>
        /// Stored procedure name sp_UpdateMessageStatus
        /// </summary>
        private const string SpUpdateMessageStatus = "sp_UpdateMessageStatus";

        /// <summary>
        /// Stored procedure name sp_UpdateTrainBaselineStatus
        /// </summary>
        private const string SpUpdateTrainBaselineStatus = "sp_UpdateTrainBaselineStatus";

        /// <summary>
        /// Stored procedure name sp_GetTrainBaselineStatus
        /// </summary>
        private const string SpGetTrainBaselineStatus = "sp_GetTrainBaselineStatus";

        /// <summary>
        /// Stored procedure name sp_DeleteTrainBaselineStatus
        /// </summary>
        private const string SpDeleteTrainBaselineStatus = "sp_DeleteTrainBaselineStatus";

		/// <summary>
		/// Stored procedure name that insert a status record with a specified status to all pending messages in the history log.
		/// </summary>
		private const string SpInsertStatusToPendingMessages = "sp_InsertStatusToPendingMessages";
		
		/// <summary>True if database was already cleaned by another service, false otherwise.</summary>
		private static bool CleanedAtStartup = false;

		/// <summary>The locker used for shared resources management.</summary>
		private static object Locker = new object();

		/// <summary>The list of table names that can be updated into the history log database.</summary>
		private static readonly string[] UpdatableTableNames = { "MessageStatus", "MessageRequest", "MessageContext", "TrainBaselineStatus" };

        #endregion

        #region History Logs

        public static void Initialize()
        {
            if (!HistoryLoggerConfiguration.Valid)
            {
                LogManager.WriteLog(TraceType.ERROR, "Configuration is invalid. Cannot proceed.", "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", null, EventIdEnum.HistoryLog);
                return;
            }

            LogManager.WriteLog(TraceType.INFO, "Configuration is valid.", "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", null, EventIdEnum.HistoryLog);
            
            SqlConnectionStringBuilder stringBuilder = new SqlConnectionStringBuilder(HistoryLoggerConfiguration.SqlConnectionString);
            
            string strDBName = string.Empty;
            string strDBPath = string.Empty;
            string strDBFileName = string.Empty;

            if (stringBuilder != null)
            {
                if (stringBuilder.InitialCatalog != null)
                {
                    strDBName = stringBuilder.InitialCatalog;
                }

                try
                {
                    FileInfo file = new FileInfo(stringBuilder.AttachDBFilename);
                    strDBFileName = file.Name.Replace(file.Extension, null);
                    strDBPath = file.DirectoryName + "\\";
                }
                catch
                {
                }
            }            

            if (strDBName.Length <= 0)
            {
                LogManager.WriteLog(TraceType.ERROR, "DataBaseName in web.config not valid", "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", null, EventIdEnum.HistoryLog);
            }

            if (strDBFileName.Length <= 0)
            {
                LogManager.WriteLog(TraceType.ERROR, "Database file name in web.config not valid", "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", null, EventIdEnum.HistoryLog);
            }

            if (strDBPath.Length <= 0)
            {
                LogManager.WriteLog(TraceType.ERROR, "LogDBPath in web.config not valid", "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", null, EventIdEnum.HistoryLog);
            }            

            string dbFilePath = strDBPath;
            string strLogPath = strDBPath + strDBFileName + "_log.ldf";
            strDBPath = strDBPath + strDBFileName + ".mdf";
           
            string archiveFileName = string.Empty;
            string strBackupPath = string.Empty;

            if (File.Exists(strDBPath) && File.Exists(strLogPath))
            {
                string dbversion = string.Empty;
                LogManager.GetDatabaseVersion(out dbversion);

                if (!string.IsNullOrEmpty(dbversion) && LogManager.GetDatabaseVersionFromFile() != dbversion)
                {
                    if (!Directory.Exists(HistoryLoggerConfiguration.LogBackupPath))
                    {
                        Directory.CreateDirectory(HistoryLoggerConfiguration.LogBackupPath);
                    }

                    strBackupPath = 
                        HistoryLoggerConfiguration.LogBackupPath +
                        "Archive_" + strDBName + "_v" + dbversion.Replace('.', '_');

                    if (File.Exists(strBackupPath))
                    {
                        File.Delete(strBackupPath);
                    }

                    try
                    {
                        SqlHelper.BackupDatabase(strDBName, HistoryLoggerConfiguration.SqlConnectionString, strBackupPath);
                        LogManager.WriteLog(TraceType.INFO, "Archived Database at " + strBackupPath, "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", null, EventIdEnum.HistoryLog);
                    }
                    catch (SqlException sqlEx)
                    {
                        LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", sqlEx, EventIdEnum.HistoryLog);
                    }
                    catch (Exception ex)
                    {
                        LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", ex, EventIdEnum.HistoryLog);
                    }

                    string strCreateDB = "IF EXISTS( select name from sys.databases where NAME= '" + strDBName + "')" +
                        " BEGIN " +
                        "ALTER DATABASE [" + strDBName + "] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE" +
                        " EXEC sp_detach_db '" + strDBName + "', 'true'" +
                        " END";

                    try
                    {
                        SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlCreateDbConnectionString, System.Data.CommandType.Text, strCreateDB);
                        File.Delete(strDBPath);
                        File.Delete(strLogPath);
                    }
                    catch (SqlException sqlEx)
                    {
                        LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", sqlEx, EventIdEnum.HistoryLog);
                    }
                    catch (Exception ex)
                    {
                        LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", ex, EventIdEnum.HistoryLog);
                    }
                }
                else if (string.IsNullOrEmpty(dbversion) && LogManager.GetDatabaseVersionFromFile() != dbversion)
                {
                    string strCreateDB = "IF EXISTS( select name from sys.databases where NAME= '" + strDBName + "')" +
                        " BEGIN " +
                        "ALTER DATABASE [" + strDBName + "] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE" +
                        " EXEC sp_detach_db '" + strDBName + "', 'true'" +
                        " END";

                    try
                    {
                        SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlCreateDbConnectionString, System.Data.CommandType.Text, strCreateDB);
                        File.Delete(strDBPath);
                        File.Delete(strLogPath);
                    }
                    catch (SqlException sqlEx)
                    {
                        LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", sqlEx, EventIdEnum.HistoryLog);
                    }
                    catch (Exception ex)
                    {
                        LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", ex, EventIdEnum.HistoryLog);
                    }
                }
                else if (LogManager.GetDatabaseVersionFromFile() == dbversion)
                {
                    return;
                }
            }
            
            try
            {
                string createDB = "IF EXISTS( select name from sys.databases where NAME= '" + strDBName + "')" +
                        " BEGIN " +
                        "ALTER DATABASE [" + strDBName + "] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE" +
                        " EXEC sp_detach_db '" + strDBName + "', 'true'" +
                        " END ";

                SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlCreateDbConnectionString, System.Data.CommandType.Text, createDB);
                SqlConnection.ClearAllPools();
            }
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.EXCEPTION, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", ex, EventIdEnum.HistoryLog);
			}
            
            
            try
            {
                string createDB = 
                    "CREATE DATABASE " + strDBName + " ON PRIMARY " +
                    "(NAME = " + strDBName + ", " +
                    "FILENAME = '" + strDBPath + "', " +
                    "SIZE = 5MB, MAXSIZE = UNLIMITED, FILEGROWTH = 10%) " +
                    "LOG ON (NAME = " + strDBName + "_log, " +
                    "FILENAME = '" + strLogPath + "', " +
                    "SIZE = 2MB, " +
                    "MAXSIZE = UNLIMITED, " +
                    "FILEGROWTH = 10%) ";

                SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlCreateDbConnectionString, System.Data.CommandType.Text, createDB);
                
                SqlConnection.ClearAllPools();

                FileInfo file = new FileInfo(HistoryLoggerConfiguration.CreateTableScriptPath);
                if (file.Exists)
                {
                    StringBuilder script = new StringBuilder();
                    script.AppendLine("USE [" + strDBName + "]");
                    script.Append(file.OpenText().ReadToEnd());
                    if (script.Length <= 0)
                    {
                        LogManager.WriteLog(TraceType.ERROR, "File " + HistoryLoggerConfiguration.CreateTableScriptPath + " does not contain any database creation scripts.", "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", null, EventIdEnum.HistoryLog);
                    }

                    string[] sqlLine;
                    Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    sqlLine = regex.Split(script.ToString());
                    foreach (string line in sqlLine)
                    {
                        if (line.Length > 0)
                        {
                            SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlConnectionString, System.Data.CommandType.Text, line);
                        }
                    }

                    LogManager.InsertDatabaseVersion(LogManager.GetDatabaseVersionFromFile());
                }
                else
                {
                    LogManager.WriteLog(TraceType.ERROR, "File " + HistoryLoggerConfiguration.CreateTableScriptPath + " does not exist.", "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", null, EventIdEnum.HistoryLog);
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, "Line=" + sqlEx.LineNumber.ToString() + ",Message=" + sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", sqlEx, EventIdEnum.HistoryLog);
                
                if (File.Exists(strBackupPath))
                {
                    if (File.Exists(strDBPath) && File.Exists(strLogPath))
                    {
                        string strCreateDB = "IF EXISTS( select name from sys.databases where NAME= '" + strDBName + "')" +
                           " BEGIN " +
                           "ALTER DATABASE [" + strDBName + "] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE" +
                           " EXEC sp_detach_db '" + strDBName + "', 'true'" +
                           " END";

                        try
                        {
                            SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlCreateDbConnectionString, System.Data.CommandType.Text, strCreateDB);
                            File.Delete(strDBPath);
                            File.Delete(strLogPath);
                        }
                        catch (SqlException sqlExcpt)
                        {
                            LogManager.WriteLog(TraceType.ERROR, sqlExcpt.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", sqlExcpt, EventIdEnum.HistoryLog);
                        }
                    }

                    RestoreDatabase(strDBName, HistoryLoggerConfiguration.LogBackupPath, HistoryLoggerConfiguration.SqlCreateDbConnectionString, dbFilePath, dbFilePath, strDBFileName);
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.Initialize", ex, EventIdEnum.HistoryLog);
            }
        }

        /// <summary>
        /// Method to restore the database
        /// </summary>
        /// <param name="databaseName">name of the database to backup</param>
        /// <param name="archveDatabasePath"> Archive database path</param>
        /// <param name="connectionString">connection string</param>
        /// <param name="destinationPath">destination path of database mdf file</param>
        /// <param name="destinationLogPath">destination path of database ldf file</param>
        /// <param name="dataBaseFileName">database file name of the restored.</param>
        private static void RestoreDatabase(string databaseName, string archveDatabasePath, string connectionString, string destinationPath, string destinationLogPath, string dataBaseFileName)
        {
            try
            {
                LogManager.WriteLog(TraceType.ERROR, "Failed to Create DataBase restoring the previous database from archive.", "PIS.Ground.Core.LogMgmt.HistoryLogger.RestoreDatabase", null, EventIdEnum.HistoryLog);
                SqlHelper.RestoreDatabase(databaseName, archveDatabasePath, connectionString, destinationPath, destinationLogPath, dataBaseFileName);
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.RestoreDatabase", sqlEx, EventIdEnum.HistoryLog);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.RestoreDatabase", ex, EventIdEnum.HistoryLog);
            }
        }

        /// <summary>
        /// Method used to write history log details.
        /// </summary>
        /// <param name="context"> message details</param>
        /// <param name="requestId">request id of the message</param>
        /// <param name="commandType">command type</param>
        /// <param name="trainId">list of train ids</param>
        /// <param name="messageStatus">message status</param>
        /// <param name="startDate"> start date of activation</param>
        /// <param name="endDate">end date of activation</param>
        /// <returns>empty string on success otherwise error message</returns>
        public static ResultCodeEnum WriteLog(string context, Guid requestId, PIS.Ground.Core.Data.CommandType commandType, string trainId, MessageStatusType messageStatus, DateTime startDate, DateTime endDate)
        {
            ResultCodeEnum error = new ResultCodeEnum();
            try
            {
                string status = GetMessageStatusString(messageStatus);
                string command = GetCommandTypeString(commandType);

                if (string.IsNullOrEmpty(context) || context.Length >= 4000)
                {
                    error = ResultCodeEnum.InvalidContext;
                }
                else if (commandType == PIS.Ground.Core.Data.CommandType.AllLogs || string.IsNullOrEmpty(command) || command == "AllLogs")
                {
                    error = ResultCodeEnum.InvalidCommandType;
                }
                else if (requestId == Guid.Empty)
                {
                    error = ResultCodeEnum.InvalidRequestID;
                }
                else if (string.IsNullOrEmpty(trainId) || trainId.Length >= 50)
                {
                    error = ResultCodeEnum.InvalidTrainID;
                }
                else if (requestId == Guid.Empty)
                {
                    error = ResultCodeEnum.InvalidRequestID;
                }
                else if (string.Compare(status, "NoStatus", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    error = ResultCodeEnum.InvalidStatus;
                }
                else if (startDate == null)
                {
                    error = ResultCodeEnum.InvalidStartDate;
                }
                else if (endDate == null)
                {
                    error = ResultCodeEnum.InvalidEndDate;
                }
                else
                {
                    List<object> parameters = new List<object>();
                    parameters.Add(requestId.ToString());
                    parameters.Add(context);
                    parameters.Add(trainId);
                    parameters.Add(command);
                    parameters.Add(startDate.ToUniversalTime());
                    parameters.Add(endDate.ToUniversalTime());
                    parameters.Add(HistoryLoggerConfiguration.MaximumLogMessageCount);
                    parameters.Add(HistoryLoggerConfiguration.PercentageToCleanUpInLogDatabase);
                    parameters.Add(status);
					parameters.Add(true); // AllowUpdate
                    int response = SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlConnectionString, SpInsertScheduleMessage, parameters);
                    if (response > 0)
                    {
                        error = ResultCodeEnum.RequestAccepted;
                    }
                    else
                    {
                        error = ResultCodeEnum.SqlError;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.WriteLog", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.WriteLog", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
            }

            return error;
        }

		/// <summary>Empty all tables in the history log database.</summary>
		/// <returns>Error code if any.</returns>
		public static ResultCodeEnum EmptyDatabase()
		{
			ResultCodeEnum error = ResultCodeEnum.RequestAccepted;
            try
            {
				foreach (string tableName in UpdatableTableNames)
				{
					string query = "DELETE FROM " + tableName;
					int response = SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlConnectionString, System.Data.CommandType.Text, query);
					if (response < 0)
					{
						error = ResultCodeEnum.SqlError;
					}
				}

			}
			catch (SqlException sqlEx)
			{
				error = ResultCodeEnum.SqlError;
				LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.EmptyDatabase", sqlEx, EventIdEnum.HistoryLog);
				error = ResultCodeEnum.SqlError;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.EmptyDatabase", ex, EventIdEnum.HistoryLog);
				error = ResultCodeEnum.InternalError;
			}

			return error;
		}

        /// <summary>
        /// Method used to cancel history log details.
        /// </summary>
        /// <param name="requestId">request id of the message</param>
        /// <param name="commandType">command type</param>
        /// <param name="trainId">list of train ids</param>
        /// <param name="messageStatus">message status</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum CancelLog(Guid requestId, PIS.Ground.Core.Data.CommandType commandType, string trainId, MessageStatusType messageStatus)
        {
            ResultCodeEnum error = new ResultCodeEnum();
            try
            {
                string status = GetMessageStatusString(messageStatus);
                string command = GetCommandTypeString(commandType);

                if (commandType == PIS.Ground.Core.Data.CommandType.AllLogs || string.IsNullOrEmpty(command) || command == "AllLogs")
                {
                    error = ResultCodeEnum.InvalidCommandType;
                }
                else if (requestId == Guid.Empty)
                {
                    error = ResultCodeEnum.InvalidRequestID;
                }
                else if (string.IsNullOrEmpty(trainId) || trainId.Length >= 50)
                {
                    error = ResultCodeEnum.InvalidTrainID;
                }
                else if (requestId == Guid.Empty)
                {
                    error = ResultCodeEnum.InvalidRequestID;
                }
                else if (string.Compare(status, "NoStatus", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    error = ResultCodeEnum.InvalidStatus;
                }
                else
                {
                    List<object> parameters = new List<object>();
                    parameters.Add(requestId.ToString());
                    parameters.Add(trainId);
                    parameters.Add(command);
                    parameters.Add(status);
                    int response = SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlConnectionString, SpUpdateMessageRequest, parameters);
                    if (response > 0)
                    {
                        error = ResultCodeEnum.RequestAccepted;
                    }
                    else
                    {
                        error = ResultCodeEnum.SqlError;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Message == "Invalid Train ID")
                {
                    error = ResultCodeEnum.InvalidRequestID;
                }
                else
                {
                    error = ResultCodeEnum.SqlError;
                }
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.CancelLog", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.CancelLog", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
            }

            return error;
        }

        /// <summary>
        /// Method used to get the latest scheduled message DateTime
        /// </summary>
        /// <param name="commandList">list of command</param>
        /// <param name="logResponse">out put response</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum GetLatestLogDateTime(List<PIS.Ground.Core.Data.CommandType> commandList, out string logResponse)
        {
            ResultCodeEnum error = new ResultCodeEnum();
            logResponse = string.Empty;
            string commands = string.Empty;
            if (commandList.Contains(PIS.Ground.Core.Data.CommandType.AllLogs))
            {
                commands = BuildCommandTypeString();
            }
            else
            {
                foreach (PIS.Ground.Core.Data.CommandType commandType in commandList)
                {
                    string command = GetCommandTypeString(commandType);
                    if (string.IsNullOrEmpty(command))
                    {
                        error = ResultCodeEnum.InvalidCommandType;
                    }
                    else
                    {
                        commands = commands + "," + command;
                    }
                }
            }

            if (commands.StartsWith(",", StringComparison.OrdinalIgnoreCase))
            {
                commands = commands.Substring(1);
            }

            try
            {
                List<object> parameters = new List<object>();
                parameters.Add(commands);
                using (DataSet logResponseDS = SqlHelper.ExecuteDataSet(HistoryLoggerConfiguration.SqlConnectionString, SpGetNewestMessageDateTime, parameters))
                {
                    error = ResultCodeEnum.RequestAccepted;
                    if (logResponseDS != null && logResponseDS.Tables.Count > 0 && logResponseDS.Tables[0] != null)
                    {
                        DateTime logDateTime = DateTime.UtcNow;
                        if (logResponseDS.Tables[0].Rows.Count > 0 && logResponseDS.Tables[0].Rows[0]["DateTime"] != null && DateTime.TryParse(logResponseDS.Tables[0].Rows[0]["DateTime"].ToString(), out logDateTime))
                        {
                            logResponse = logDateTime.ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            error = ResultCodeEnum.Empty_Result;
                        }
                    }
                    else
                    {
                        error = ResultCodeEnum.Empty_Result;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetLatestLogDateTime", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetLatestLogDateTime", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
            }

            return error;
        }

        /// <summary>
        /// Method used to get the latest scheduled message
        /// </summary>
        /// <param name="commandList">list of command</param>
        /// <param name="logResponse">out put response</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum GetLatestLog(List<PIS.Ground.Core.Data.CommandType> commandList, out string logResponse)
        {
            ResultCodeEnum error = new ResultCodeEnum();
            logResponse = string.Empty;
            string commands = string.Empty;
            if (commandList.Contains(PIS.Ground.Core.Data.CommandType.AllLogs))
            {
                commands = BuildCommandTypeString();
            }
            else
            {
                foreach (PIS.Ground.Core.Data.CommandType commandType in commandList)
                {
                    string command = GetCommandTypeString(commandType);
                    if (string.IsNullOrEmpty(command))
                    {
                        error = ResultCodeEnum.InvalidCommandType;
                    }
                    else
                    {
                        commands = commands + "," + command;
                    }
                }
            }

            if (commands.StartsWith(",", StringComparison.OrdinalIgnoreCase))
            {
                commands = commands.Substring(1);
            }

            try
            {
                List<object> parameters = new List<object>();
                parameters.Add(commands);
                using (DataSet logResponseDS = SqlHelper.ExecuteDataSet(HistoryLoggerConfiguration.SqlConnectionString, SpGetNewestMessage, parameters))
                {
                    error = ResultCodeEnum.RequestAccepted;
                    error = GenerateLogResponse(logResponseDS, out logResponse);
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetLatestLog", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetLatestLog", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
            }

            return error;
        }

        /// <summary>
        /// Method used to get the oldest scheduled message DateTime
        /// </summary>
        /// <param name="commandList">list of command</param>
        /// <param name="logResponse">out put response</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum GetOldestLogDateTime(List<PIS.Ground.Core.Data.CommandType> commandList, out string logResponse)
        {
            ResultCodeEnum error = new ResultCodeEnum();
            logResponse = string.Empty;
            string commands = string.Empty;

            if (commandList.Contains(PIS.Ground.Core.Data.CommandType.AllLogs))
            {
                commands = BuildCommandTypeString();
            }
            else
            {
                foreach (PIS.Ground.Core.Data.CommandType commandType in commandList)
                {
                    string command = GetCommandTypeString(commandType);
                    if (string.IsNullOrEmpty(command))
                    {
                        error = ResultCodeEnum.InvalidCommandType;
                    }
                    else
                    {
                        commands = commands + "," + command;
                    }
                }
            }

            if (commands.StartsWith(",", StringComparison.OrdinalIgnoreCase))
            {
                commands = commands.Substring(1);
            }

            try
            {
                List<object> parameters = new List<object>();
                parameters.Add(commands);
                using (DataSet logResponseDS = SqlHelper.ExecuteDataSet(HistoryLoggerConfiguration.SqlConnectionString, SpGetOldestMessageDateTime, parameters))
                {
                    error = ResultCodeEnum.RequestAccepted;
                    if (logResponseDS != null && logResponseDS.Tables.Count > 0 && logResponseDS.Tables[0] != null)
                    {
                        DateTime logDateTime = DateTime.UtcNow;
                        if (logResponseDS.Tables[0].Rows.Count > 0 && logResponseDS.Tables[0].Rows[0]["DateTime"] != null && DateTime.TryParse(logResponseDS.Tables[0].Rows[0]["DateTime"].ToString(), out logDateTime))
                        {
                            logResponse = logDateTime.ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            error = ResultCodeEnum.Empty_Result;
                        }
                    }
                    else
                    {
                        error = ResultCodeEnum.Empty_Result;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetOldestLogDateTime", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetOldestLogDateTime", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
            }

            return error;
        }

        /// <summary>
        /// Method used to get the oldest scheduled message
        /// </summary>
        /// <param name="commandList">list of command</param>
        /// <param name="logResponse">out put response</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum GetOldestLog(List<PIS.Ground.Core.Data.CommandType> commandList, out string logResponse)
        {
            ResultCodeEnum error = new ResultCodeEnum();
            logResponse = string.Empty;
            string commands = string.Empty;
            if (commandList.Contains(PIS.Ground.Core.Data.CommandType.AllLogs))
            {
                commands = BuildCommandTypeString();
            }
            else
            {
                foreach (PIS.Ground.Core.Data.CommandType commandType in commandList)
                {
                    string command = GetCommandTypeString(commandType);
                    if (string.IsNullOrEmpty(command))
                    {
                        error = ResultCodeEnum.InvalidCommandType;
                    }
                    else
                    {
                        commands = commands + "," + command;
                    }
                }
            }

            if (commands.StartsWith(",", StringComparison.OrdinalIgnoreCase))
            {
                commands = commands.Substring(1);
            }

            try
            {
                List<object> parameters = new List<object>();
                parameters.Add(commands);
                using (DataSet logResponseDS = SqlHelper.ExecuteDataSet(HistoryLoggerConfiguration.SqlConnectionString, SpGetOldestMessage, parameters))
                {
                    error = ResultCodeEnum.RequestAccepted;
                    error = GenerateLogResponse(logResponseDS, out logResponse);
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetOldestLog", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetOldestLog", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
            }

            return error;
        }

        /// <summary>
        /// Method used to get all the log message
        /// </summary>
        /// <param name="commandList">command list</param>
        /// <param name="startDateTime">start date time to be considered</param>
        /// <param name="endDateTime">end date time to be considered</param>
        /// <param name="language">language code</param>
        /// <param name="logResponse">output log response</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum GetAllLog(List<PIS.Ground.Core.Data.CommandType> commandList, DateTime startDateTime, DateTime endDateTime, uint language, out string logResponse)
        {
            ResultCodeEnum error = new ResultCodeEnum();
            logResponse = string.Empty;
            string commands = string.Empty;
            //DateTime commandEndDate = DateTime.UtcNow;
            //if (!DateTime.TryParse(endDateTime.ToLongDateString(), out commandEndDate))
            //{
            //    error = ResultCodeEnum.InvalidEndDate;
            //}

            if (commandList.Contains(PIS.Ground.Core.Data.CommandType.AllLogs))
            {
                commands = BuildCommandTypeString();
            }
            else
            {
                foreach (PIS.Ground.Core.Data.CommandType commandType in commandList)
                {
                    string command = GetCommandTypeString(commandType);
                    if (string.IsNullOrEmpty(command))
                    {
                        error = ResultCodeEnum.InvalidCommandType;
                    }
                    else
                    {
                        commands = commands + "," + command;
                    }
                }
            }

            if (commands.StartsWith(",", StringComparison.OrdinalIgnoreCase))
            {
                commands = commands.Substring(1);
            }

            try
            {
                List<object> parameters = new List<object>();
                parameters.Add(startDateTime.ToUniversalTime());
                parameters.Add(endDateTime.ToUniversalTime());
                parameters.Add(commands);
                using (DataSet logResponseDS = SqlHelper.ExecuteDataSet(HistoryLoggerConfiguration.SqlConnectionString, SpGetAllMessages, parameters))
                {
                    error = ResultCodeEnum.RequestAccepted;
                    if (logResponseDS != null && logResponseDS.Tables.Count > 0 && logResponseDS.Tables[0] != null)
                    {
                        if (logResponseDS.Tables[0].Rows.Count <= HistoryLoggerConfiguration.MaximumLogMessageSize)
                        {
                            error = GenerateLogResponse(logResponseDS, out logResponse);
                            if (logResponse.Length > HistoryLoggerConfiguration.MaxStringContentLength)
                            {
                                error = ResultCodeEnum.OutputLimitExceed;
                                return error;
                            }
                        }
                        else
                        {
                            error = ResultCodeEnum.OutputLimitExceed;
                            return error;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetAllLog", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
                logResponse = sqlEx.Message;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetAllLog", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
                logResponse = ex.Message;
            }

            return error;
        }

        /// <summary>
        /// Clean the history log database
        /// </summary>
        /// <param name="commandList">input command list</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum CleanLog(List<PIS.Ground.Core.Data.CommandType> commandList)
        {
            ResultCodeEnum error = new ResultCodeEnum();
            error = ResultCodeEnum.InvalidCommandType;
            if (commandList.Contains(PIS.Ground.Core.Data.CommandType.AllLogs))
            {
                try
                {
                    List<object> parameters = new List<object>();
                    parameters.Add("CancelScheduledMsg");
                    int response = SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlConnectionString, SpDeleteMessage, parameters);
                    if (response > 0)
                    {
                        error = ResultCodeEnum.RequestAccepted;
                    }
                    else
                    {
                        error = ResultCodeEnum.Empty_Result;
                    }

                    parameters = new List<object>();
                    parameters.Add("SendScheduledMsg");
                    response = SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlConnectionString, SpDeleteMessage, parameters);
                    if (response > 0)
                    {
                        error = ResultCodeEnum.RequestAccepted;
                    }
                    else if(error != ResultCodeEnum.RequestAccepted)
                    {
                        error = ResultCodeEnum.Empty_Result;
                    }
                }
                catch (SqlException sqlEx)
                {
                    LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.CleanLog", sqlEx, EventIdEnum.HistoryLog);
                    error = ResultCodeEnum.SqlError;
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.CleanLog", ex, EventIdEnum.HistoryLog);
                    error = ResultCodeEnum.InternalError;
                }
            }
            else
            {
                foreach (PIS.Ground.Core.Data.CommandType commandType in commandList)
                {
                    if (commandType == PIS.Ground.Core.Data.CommandType.AllLogs)
                    {
                        continue;
                    }
                    else
                    {
                        try
                        {
                            List<object> parameters = new List<object>();
                            string command = GetCommandTypeString(commandType);
                            if (string.IsNullOrEmpty(command))
                            {
                                error = ResultCodeEnum.InvalidCommandType;
                            }

                            parameters.Add(command);
                            int response = SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlConnectionString, SpDeleteMessage, parameters);
                            if (response > 0)
                            {
                                error = ResultCodeEnum.RequestAccepted;
                            }
                            else
                            {
                                error = ResultCodeEnum.Empty_Result;
                            }
                        }
                        catch (SqlException sqlEx)
                        {
                            LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.CleanLog", sqlEx, EventIdEnum.HistoryLog);
                            error = ResultCodeEnum.SqlError;
                        }
                        catch (Exception ex)
                        {
                            LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.CleanLog", ex, EventIdEnum.HistoryLog);
                            error = ResultCodeEnum.InternalError;
                        }
                    }
                }
            }

            return error;
        }

        /// <summary>Method to update the message status.</summary>
        /// <param name="trainId">list of train ids.</param>
        /// <param name="requestId">request id of the message.</param>
        /// <param name="messageStatus">message status.</param>
        /// <param name="commandType">Type of message to update.</param>
        /// <returns>Error code if any.</returns>
        public static ResultCodeEnum UpdateMessageStatus(string trainId, Guid requestId, MessageStatusType messageStatus, PIS.Ground.Core.Data.CommandType commandType)
        {
            ResultCodeEnum error = new ResultCodeEnum();
            try
            {
				string command = GetCommandTypeString(commandType);
                string status = GetMessageStatusString(messageStatus);
				if (commandType == PIS.Ground.Core.Data.CommandType.AllLogs || string.IsNullOrEmpty(command) || command == "AllLogs")
				{
					error = ResultCodeEnum.InvalidCommandType;
				}
				else if (string.IsNullOrEmpty(trainId) || trainId.Length >= 50)
                {
                    error = ResultCodeEnum.InvalidTrainID;
                }
                else if (requestId == Guid.Empty)
                {
                    error = ResultCodeEnum.InvalidRequestID;
                }
				else if (string.Compare(status, "NoStatus", StringComparison.OrdinalIgnoreCase) == 0)
				{
					error = ResultCodeEnum.InvalidStatus;
				}
				else
				{
					SqlParameter returnValueParameter = new SqlParameter("ReturnValue", SqlDbType.Int);
					returnValueParameter.Value = Int32.MinValue;
					returnValueParameter.Direction = ParameterDirection.ReturnValue;
					SqlParameter trainIdParameter = new SqlParameter("@TrainId", SqlDbType.NVarChar, 50);
					trainIdParameter.Value = trainId;
					SqlParameter requestIdParameter = new SqlParameter("@RequestID", SqlDbType.NVarChar, 50);
					requestIdParameter.Value = requestId.ToString();
					SqlParameter statusParameter = new SqlParameter("@Status", SqlDbType.NVarChar, 50);
					statusParameter.Value = status;
					SqlParameter commandTypeParameter = new SqlParameter("@CmdType", SqlDbType.NVarChar, 50);
					commandTypeParameter.Value = command;

					// When a stored procedure return a value, the return value of ExecuteNonQuery is not accurate.
					SqlHelper.ExecuteNonQuery(
						HistoryLoggerConfiguration.SqlConnectionString,
						System.Data.CommandType.StoredProcedure,
						SpUpdateMessageStatus,
						trainIdParameter,
						requestIdParameter,
						statusParameter,
						commandTypeParameter,
						returnValueParameter);

					int response = Convert.ToInt32(returnValueParameter.Value);
					if (response > 0)
					{
						error = ResultCodeEnum.RequestAccepted;
					}
					else if (response == 0)
					{
						error = ResultCodeEnum.InvalidRequestID;
					}
					else
					{
						error = ResultCodeEnum.SqlError;
					}
				}
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.UpdateMessageStatus", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.UpdateMessageStatus", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
            }

            return error;
        }

        /// <summary>Method to update the baseline progress status.</summary>
        /// <param name="trainId">train id.</param>
        /// <param name="requestId">request id of the message.</param>
        /// <param name="taskId">Identifier for the task.</param>
        /// <param name="trainNumber">The train number.</param>
        /// <param name="onlineStatus">online status of the train.</param>
        /// <param name="progressStatus">The progress status.</param>
        /// <param name="currentBaselineVersion">The current baseline version.</param>
        /// <param name="futureBaselineVersion">The future baseline version.</param>
        /// <param name="pisOnBoardVersion">The pis on board version.</param>
        /// <returns>Error code if any.</returns>
        public static ResultCodeEnum UpdateTrainBaselineStatus(string trainId, Guid requestId, int taskId, string trainNumber,
                                                               bool onlineStatus, BaselineProgressStatusEnum progressStatus,
                                                               string currentBaselineVersion, string futureBaselineVersion,
                                                               string pisOnBoardVersion)
        {
            ResultCodeEnum error = new ResultCodeEnum();
            try
            {
                if (string.IsNullOrEmpty(trainId) || trainId.Length >= 50)
                {
                    error = ResultCodeEnum.InvalidTrainID;
                }
                else if (requestId == null)
                {
                    error = ResultCodeEnum.InvalidRequestID;
                }
                else
                {
                    List<object> parameters = new List<object>(9);
                    parameters.Add(trainId);
                    parameters.Add(requestId.ToString());
                    parameters.Add(taskId);
                    parameters.Add(trainNumber ?? string.Empty);
                    parameters.Add(Convert.ToInt32(onlineStatus));
                    parameters.Add((int)progressStatus);
                    parameters.Add(currentBaselineVersion ?? string.Empty);
                    parameters.Add(futureBaselineVersion ?? string.Empty);
                    parameters.Add(pisOnBoardVersion??string.Empty);

                    int response = SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlConnectionString, SpUpdateTrainBaselineStatus, parameters);
                    if (response > 0)
                    {
                        error = ResultCodeEnum.RequestAccepted;
                    }
                    else
                    {
                        error = ResultCodeEnum.SqlError;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.UpdateTrainBaselineStatus", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.UpdateTrainBaselineStatus", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
            }
            return error;
        }

        /// <summary>
        /// Clean the Baseline progress status
        /// </summary>
        /// <param name="trainId">train id.</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum CleanTrainBaselineStatus(string trainId)
        {
            ResultCodeEnum error = new ResultCodeEnum();
            error = ResultCodeEnum.InvalidCommandType;
            try
            {
                List<object> parameters = new List<object>();
                parameters.Add(trainId);
                int response = SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlConnectionString, SpDeleteTrainBaselineStatus, parameters);
                if (response > 0)
                {
                    error = ResultCodeEnum.RequestAccepted;
                }
                else
                {
                    error = ResultCodeEnum.Empty_Result;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.CleanTrainBaselineStatus", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.CleanTrainBaselineStatus", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
            }
            return error;
        }

        /// <summary>Method used to get all the status of the baselines.</summary>
        /// <param name="dictionaryResponse">[out] The dictionary response.</param>
        /// <returns>Error code if any.</returns>
        public static ResultCodeEnum GetTrainBaselineStatus(out Dictionary<string, TrainBaselineStatusData> dictionaryResponse)
        {
            ResultCodeEnum error = new ResultCodeEnum();

            dictionaryResponse = null;
            try
            {
                List<object> parameters = new List<object>();
                parameters.Add(null);
                using (DataSet logResponseDS = SqlHelper.ExecuteDataSet(HistoryLoggerConfiguration.SqlConnectionString, SpGetTrainBaselineStatus, parameters))
                {
                    error = ResultCodeEnum.RequestAccepted;
                    if (logResponseDS != null && logResponseDS.Tables.Count > 0 && logResponseDS.Tables[0] != null)
                    {
                        if (logResponseDS.Tables[0].Rows.Count <= HistoryLoggerConfiguration.MaximumLogMessageSize)
                        {
                            error = GenerateStatusResponse(logResponseDS, out dictionaryResponse);
                        }
                        else
                        {
                            error = ResultCodeEnum.OutputLimitExceed;
                            return error;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetTrainBaselineStatus", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetTrainBaselineStatus", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
            }
            return error;
        }

        /// <summary>Gets train baseline status.</summary>
        /// <param name="progressStatus">The progress status to search.</param>
        /// <param name="dictionaryResponse">[out] The dictionary response.</param>
        /// <returns>The train baseline status.</returns>
        public static ResultCodeEnum GetTrainBaselineStatusByProgressStatus(BaselineProgressStatusEnum progressStatus,
                                                            out Dictionary<string, TrainBaselineStatusData> dictionaryResponse)
        {
            ResultCodeEnum error = new ResultCodeEnum();

            dictionaryResponse = null;
            try
            {
                List<object> parameters = new List<object>();
                parameters.Add(progressStatus);
                using (DataSet logResponseDS = SqlHelper.ExecuteDataSet(HistoryLoggerConfiguration.SqlConnectionString, SpGetTrainBaselineStatus, parameters))
                {
                    error = ResultCodeEnum.RequestAccepted;
                    if (logResponseDS != null && logResponseDS.Tables.Count > 0 && logResponseDS.Tables[0] != null)
                    {
                        if (logResponseDS.Tables[0].Rows.Count <= HistoryLoggerConfiguration.MaximumLogMessageSize)
                        {
                            error = GenerateStatusResponse(logResponseDS, out dictionaryResponse);
                        }
                        else
                        {
                            error = ResultCodeEnum.OutputLimitExceed;
                            return error;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetTrainBaselineStatusByProgressStatus", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetTrainBaselineStatusByProgressStatus", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
            }
            return error;
        }

        /// <summary>
        /// Method to generate output xml response
        /// </summary>
        /// <param name="response">list of recordset</param>
        /// <param name="logResponse">status response</param>
        /// <returns>out xml string</returns>
        private static ResultCodeEnum GenerateStatusResponse(DataSet response, out Dictionary<string, TrainBaselineStatusData> dictionaryResponse)
        {
            dictionaryResponse = null;
            ResultCodeEnum lReturnCode = ResultCodeEnum.Empty_Result;
            if (response != null && response.Tables.Count > 0)
            {
                if (response.Tables[0] != null)
                {
                    dictionaryResponse = new Dictionary<string, TrainBaselineStatusData>();

                    foreach (DataRow rowContext in response.Tables[0].Rows)
                    {
                        TrainBaselineStatusData lData = new TrainBaselineStatusData();

                        if (rowContext["TrainId"] != null)
                        {
                            lData.TrainId = (string)rowContext["TrainId"];
                            if (rowContext["RequestId"] != null)
                            {
                                lData.RequestId = new Guid((string)rowContext["RequestId"]);
                            }
                            if (rowContext["TaskId"] != null)
                            {
                                lData.TaskId = (int)rowContext["TaskId"];
                            }
                            if (rowContext["TrainNumber"] != null)
                            {
                                lData.TrainNumber = (string)rowContext["TrainNumber"];
                            }
                            if (rowContext["OnlineStatus"] != null)
                            {
                                lData.OnlineStatus = (bool)rowContext["OnlineStatus"];
                            }
                            if (rowContext["BaselineProgressStatus"] != null)
                            {
                                lData.ProgressStatus = (BaselineProgressStatusEnum)rowContext["BaselineProgressStatus"];
                            }
                            if (rowContext["CurrentBaselineVersion"] != null)
                            {
                                lData.CurrentBaselineVersion = (string)rowContext["CurrentBaselineVersion"];
                            }
                            if (rowContext["FutureBaselineVersion"] != null)
                            {
                                lData.FutureBaselineVersion = (string)rowContext["FutureBaselineVersion"];
                            }
                            if (rowContext["PISOnBoardVersion"] != null)
                            {
                                lData.PisOnBoardVersion = (string)rowContext["PISOnBoardVersion"];
                            }

                            dictionaryResponse.Add(lData.TrainId, lData);
                        }
                    }
                    lReturnCode = ResultCodeEnum.RequestAccepted;
                }
            }
            return lReturnCode;
        }

        /// <summary>
        /// Method to get the Database Version.
        /// </summary>      
        /// <param name="version">Database version</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum GetDatabaseVersion(out string version)
        {
            ResultCodeEnum error = new ResultCodeEnum();
            version = string.Empty;
            try
            {
                using (DataSet logResponseDS = SqlHelper.ExecuteDataSet(HistoryLoggerConfiguration.SqlConnectionString, SpGetDatabaseVersion, null))
                {
                    error = ResultCodeEnum.RequestAccepted;
                    if (logResponseDS != null && logResponseDS.Tables.Count > 0 && logResponseDS.Tables[0] != null)
                    {
                        if (logResponseDS.Tables[0].Rows.Count > 0 && logResponseDS.Tables[0].Rows[0]["Version"] != null)
                        {
                            version = logResponseDS.Tables[0].Rows[0]["Version"].ToString();
                        }
                        else
                        {
                            error = ResultCodeEnum.SqlError;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetDatabaseVersion", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.GetDatabaseVersion", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
            }

            return error;
        }

        /// <summary>
        /// Method to insert the Database Version.
        /// </summary>      
        /// <param name="version">Database version</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum InsertDatabaseVersion(string version)
        {
            ResultCodeEnum error = new ResultCodeEnum();
            try
            {
                if (string.IsNullOrEmpty(version) || version.Length >= 10)
                {
                    error = ResultCodeEnum.InvalidTrainID;
                }
                else
                {
                    List<object> parameters = new List<object>();
                    parameters.Add(version);
                    int response = SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlConnectionString, SpInsertUpdateDatabaseVersion, parameters);
                    if (response > 0)
                    {
                        error = ResultCodeEnum.RequestAccepted;
                    }
                    else
                    {
                        error = ResultCodeEnum.SqlError;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.WriteLog(TraceType.ERROR, sqlEx.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.InsertDatabaseVersion", sqlEx, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.SqlError;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.LogMgmt.HistoryLogger.InsertDatabaseVersion", ex, EventIdEnum.HistoryLog);
                error = ResultCodeEnum.InternalError;
            }

            return error;
        }

        /// <summary>
        /// Method to get the message status in string format.
        /// </summary>
        /// <param name="messageStatus">message status type</param>
        /// <returns>message status as a string</returns>
        public static string GetMessageStatusString(MessageStatusType messageStatus)
        {
            switch (messageStatus)
            {
                case MessageStatusType.InstantMessageDistributionProcessing:
                    return "MsgProcessing";

                case MessageStatusType.InstantMessageDistributionReceived:
                    return "MsgReceived";

                case MessageStatusType.InstantMessageDistributionWaitingToSend:
                    return "MsgWaitingToSend";

                case MessageStatusType.InstantMessageDistributionSent:
                    return "MsgSent";

                case MessageStatusType.InstantMessageDistributionTimedOut:
                    return "MsgTimedOut";

				case MessageStatusType.InstantMessageDistributionCanceledByStartupError:
					return "MsgCanceledByStartupError";

				case MessageStatusType.InstantMessageDistributionCanceled:
					return "MsgCanceled";

				case MessageStatusType.InstantMessageDistributionInvalidTemplateError:
					return "MsgInvalidTemplateError";

				case MessageStatusType.InstantMessageDistributionInvalidScheduledPeriodError:
					return "MsgInvalidScheduledPeriodError";

				case MessageStatusType.InstantMessageDistributionInvalidRepetitionCountError:
					return "MsgInvalidRepetitionCountError";

				case MessageStatusType.InstantMessageDistributionInvalidTemplateFileError:
					return "MsgInvalidTemplateFileError";

				case MessageStatusType.InstantMessageDistributionUnknownCarIdError:
					return "MsgUnknownCarIdError";

				case MessageStatusType.InstantMessageDistributionInvalidDelayError:
					return "MsgInvalidDelayError";

				case MessageStatusType.InstantMessageDistributionInvalidDelayReasonError:
					return "MsgInvalidDelayReasonError";

				case MessageStatusType.InstantMessageDistributionInvalidHourError:
					return "MsgInvalidHourError";

				case MessageStatusType.InstantMessageDistributionUndefinedStationIdError:
					return "MsgUndefinedStationIdError";

				case MessageStatusType.InstantMessageDistributionInvalidTextError:
					return "MsgInvalidTextError";

				case MessageStatusType.InstantMessageDistributionMessageLimitExceededError:
					return "MsgLimitExceededError";
				case MessageStatusType.InstantMessageDistributionUnexpectedError:
					return "MsgUnexpectedError";

				case MessageStatusType.InstantMessageDistributionInhibited:
					return "MsgInhibited";
				case MessageStatusType.InstantMessageDistributionCanceledGroundOnly:
					return "CanceledGroundOnly";
                default:
                    return "NoStatus";
            }
        }

        /// <summary>
        /// Method to get the command type in string format.
        /// </summary>
        /// <param name="commandType"> command type</param>
        /// <returns>command type as a string</returns>
        private static string GetCommandTypeString(PIS.Ground.Core.Data.CommandType commandType)
        {
            switch (commandType)
            {
                case PIS.Ground.Core.Data.CommandType.AllLogs:
                    return "AllLogs";
                case PIS.Ground.Core.Data.CommandType.SendScheduledMessage:
                    return "SendScheduledMsg";
                case PIS.Ground.Core.Data.CommandType.CancelScheduledMessage:
                    return "CancelScheduledMsg";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Method to build the command type in string format.
        /// </summary>
        /// <returns>command type as a string seperated by comma.</returns>
        private static string BuildCommandTypeString()
        {
            string commands = string.Empty;
            commands = "SendScheduledMsg" + "," + "CancelScheduledMsg";
            return commands;
        }

        /// <summary>
        /// Method to generate output xml response
        /// </summary>
        /// <param name="response">list of recordset</param>
        /// <param name="logResponse">log response</param>
        /// <returns>out xml string</returns>
        private static ResultCodeEnum GenerateLogResponse(DataSet response, out string logResponse)
        {
            logResponse = string.Empty;
            if (response != null && response.Tables.Count > 0)
            {
                // Create the xml document container
                XmlDocument doc = new XmlDocument();
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(dec);
                XmlElement root = doc.CreateElement("Result");
                doc.AppendChild(root);
                if (response.Tables[0] != null)
                {
                    XmlElement scheduledMessages = doc.CreateElement("ScheduledMessages");                        
                    foreach (DataRow rowContext in response.Tables[0].Rows)
                    {
                        string messageContextId = string.Empty;
                        if (rowContext["MessageContextId"] != null)
                        {
                            messageContextId = (string)rowContext["MessageContextId"].ToString();
                        }

                        XmlElement scheduledMessage = doc.CreateElement("ScheduledMessage");
                        if (rowContext["RequestID"] != null)
                        {
                            scheduledMessage.SetAttribute("RequestId", (string)rowContext["RequestID"]);
                        }
                        else
                        {
                            scheduledMessage.SetAttribute("RequestId", string.Empty);
                        }

                        XmlElement context = doc.CreateElement("Context");
                        if (rowContext["Context"] != null)
                        {
                            context.InnerText = (string)rowContext["Context"];
                        }
                        else
                        {
                            context.InnerText = string.Empty;
                        }

                        XmlElement startDate = doc.CreateElement("StartDate");
                        if (rowContext["StartDate"] != null)
                        {
                            try
                            {
                                startDate.InnerText = DataRowExtensions.Field<DateTime>(rowContext, "StartDate").ToString(CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                startDate.InnerText = string.Empty;
                            }
                        }
                        else
                        {
                            startDate.InnerText = string.Empty;
                        }

                        XmlElement endDate = doc.CreateElement("EndDate");
                        if (rowContext["EndDate"] != null)
                        {
                            try
                            {
                                endDate.InnerText = DataRowExtensions.Field<DateTime>(rowContext, "EndDate").ToString(CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                endDate.InnerText = string.Empty;
                            }
                        }
                        else
                        {
                            endDate.InnerText = string.Empty;
                        }

                        scheduledMessage.AppendChild(context);
                        scheduledMessage.AppendChild(startDate);
                        scheduledMessage.AppendChild(endDate);

                        if (response.Tables[1] != null)
                        {
                            XmlElement trains = doc.CreateElement("Trains");
                            DataRow[] foundRows;
                            List<string> trainList = new List<string>();
                            foundRows = response.Tables[1].Select("MessageContextId = '" + messageContextId + "'");
                            foreach (DataRow rw in foundRows)
                            {
                                if (rw["TrainID"] != null && !trainList.Contains((string)rw["TrainID"]))
                                {
                                    trainList.Add((string)rw["TrainID"]);
                                }
                            }

                            foreach (string trainId in trainList)
                            {
                                XmlElement train = doc.CreateElement("Train");
                                train.SetAttribute("Id", trainId);
                                XmlElement commands = doc.CreateElement("Commands");
                                DataRow[] foundCommand;
                                foundCommand = response.Tables[1].Select("MessageContextId = '" + messageContextId + "' AND TrainID = '" + trainId + "'");
                                foreach (DataRow rw in foundCommand)
                                {
                                    XmlElement command = doc.CreateElement("Command");
                                    if (rw["Command"] != null)
                                    {
                                        command.SetAttribute("Type", (string)rw["Command"]);
                                    }

                                    DateTime commandDate = DateTime.UtcNow;
                                    if (rw["DateTime"] != null && DateTime.TryParse(rw["DateTime"].ToString(), out commandDate))
                                    {
                                        command.SetAttribute("DateTime", commandDate.ToString());
                                    }

                                    string msgReqId = string.Empty;
                                    if (rw["MessageRequestID"] != null)
                                    {
                                        msgReqId = (string)rw["MessageRequestID"].ToString();
                                    }

                                    if (response.Tables[2] != null && !string.IsNullOrEmpty(msgReqId))
                                    {
                                        XmlElement states = doc.CreateElement("States");
                                        DataRow[] foundStates;
                                        foundStates = response.Tables[2].Select("MessageRequestID = '" + msgReqId + "'");
                                        foreach (DataRow stateRow in foundStates)
                                        {
                                            XmlElement state = doc.CreateElement("State");
                                            XmlElement stateName = doc.CreateElement("Name");
                                            XmlElement stateTime = doc.CreateElement("DateTime");
                                            if (stateRow["Status"] != null)
                                            {
                                                stateName.InnerText = (string)stateRow["Status"];
                                            }

                                            DateTime stateDate = DateTime.UtcNow;
                                            if (stateRow["DateTime"] != null && DateTime.TryParse(stateRow["DateTime"].ToString(), out stateDate))
                                            {
                                                stateTime.InnerText = stateDate.ToString(CultureInfo.InvariantCulture);
                                            }

                                            state.AppendChild(stateName);
                                            state.AppendChild(stateTime);
                                            states.AppendChild(state);
                                        }

                                        command.AppendChild(states);
                                    }

                                    commands.AppendChild(command);
                                }

                                train.AppendChild(commands);
                                trains.AppendChild(train);
                            }

                            scheduledMessage.AppendChild(trains);
                        }

                        scheduledMessages.AppendChild(scheduledMessage);
                    }
                    
                    root.AppendChild(scheduledMessages);
                }
                else
                {
                    logResponse = string.Empty;
                    return ResultCodeEnum.Empty_Result;
                }

                logResponse = doc.OuterXml;
                return ResultCodeEnum.RequestAccepted;
            }
            else
            {
                logResponse = string.Empty;
                return ResultCodeEnum.Empty_Result;
            }
        }

		public static void MarkPendingMessagesAsCanceledByStartup()
		{
			lock (Locker)
			{
				try
				{
					List<object> parameters = new List<object>(1);
					parameters.Add("MsgCanceledByStartupError");
					SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlConnectionString, SpInsertStatusToPendingMessages, parameters);
					CleanedAtStartup = true;
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(TraceType.EXCEPTION, ex.Message, "HistoryLogger.MarkPendingMessagesAsCanceledByStartup", ex, EventIdEnum.HistoryLog);
				}

			}
		}


		/// <summary>Update pending messages in history log database to status Cancel.</summary>
		public static void DeletePendingMessages()
		{
			if (!CleanedAtStartup)
			{
				lock (Locker)
				{
					if (!CleanedAtStartup)
					{
						try
						{
							SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlConnectionString, SpDeletePendingMessages, null);
							CleanedAtStartup = true;
						}
						catch (Exception ex)
						{
							LogManager.WriteLog(TraceType.ERROR, ex.Message, "HistoryLogger::DeletePendingMessages", ex, EventIdEnum.HistoryLog);
						}
					}
				}
			}
		}

        #endregion
    }
}
