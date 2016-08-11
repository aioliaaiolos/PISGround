// <copyright file="LogManager.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2011.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
// </copyright>
namespace PIS.Ground.Core.LogMgmt
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.Properties;
    using PIS.Ground.Core.Utility;

    /// <summary>
    /// Class used for logging the Traces that occurs during the different operations.
    /// </summary>
    public sealed class LogManager : ILogManager
    {
        #region Fields

        /// <summary>DateTime Format for the Error message.</summary>
        private const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss";

        /// <summary>Source of the Event.</summary>
        private const string EventLogSource = "PIS";

        /// <summary>The user log level.</summary>
        private static TraceType _userLogLevel = TraceType.ERROR;

        #endregion

        public static TraceType LogLevel
        {
            get
            {
                return _userLogLevel;
            }
            set
            {
                _userLogLevel = value;
            }
        }

        /// <summary>
        /// Allows an instance of the LogManager class to be created.
        /// </summary>
        public LogManager()
        {
        }

        #region Error Logs

        /// <summary>
        /// Determines whether f trace is active for the specified trace lever.
        /// </summary>
        /// <param name="level">The level to evaluate.</param>
        /// <returns>true if the trace is active, false otherwise.</returns>
        public static bool IsTraceActive(TraceType level)
        {
            bool isActive;
            switch (level)
            {
                case TraceType.ERROR:
                    isActive = _userLogLevel != TraceType.NONE;
                    break;
                case TraceType.DEBUG:
                    isActive = _userLogLevel == TraceType.DEBUG;
                    break;
                case TraceType.EXCEPTION:
                    isActive = (_userLogLevel != TraceType.NONE);
                    break;
                case TraceType.INFO:
                    isActive = (_userLogLevel == TraceType.INFO || _userLogLevel == TraceType.DEBUG);
                    break;
                case TraceType.WARNING:
                    isActive = (_userLogLevel == TraceType.WARNING || _userLogLevel == TraceType.INFO || _userLogLevel == TraceType.DEBUG);
                    break;
                default:
                    isActive = (_userLogLevel == TraceType.DEBUG);
                    break;
            }

            return isActive;
        }

        /// <summary>
        /// Logs the errors to log file.
        /// </summary>
        /// <param name="trace">Type of trace.</param>
        /// <param name="message">User Message to be written.</param>
        /// <param name="context">The context where the error has occurred.</param>
        /// <param name="objEx">The exception object.</param>
        /// <param name="eventId">The EventIdEnum to facilitate reading in log manager.</param>
        /// <returns>if any error return false else true</returns>
        public static bool WriteLog(TraceType trace, string message, string context, Exception objEx, EventIdEnum eventId)
        {
            bool objWriteLogSuccess = false;
            try
            {
                if (LogManager.IsTraceActive(trace))
                {
                    EventLogEntryType objEntryType;
                    switch (trace)
                    {
                        case TraceType.ERROR:
                            objEntryType = EventLogEntryType.Error;
                            break;
                        case TraceType.DEBUG:
                            objEntryType = EventLogEntryType.FailureAudit;
                            break;
                        case TraceType.EXCEPTION:
                            objEntryType = EventLogEntryType.Error;
                            break;
                        case TraceType.INFO:
                            objEntryType = EventLogEntryType.Information;
                            break;
                        case TraceType.WARNING:
                            objEntryType = EventLogEntryType.Warning;
                            break;
                        default:
                            objEntryType = EventLogEntryType.Information;
                            break;
                    }

                    objWriteLogSuccess = WriteEventLog(objEx, context, message, objEntryType, (int)eventId);
                }
            }
            catch
            {
                Console.WriteLine("Problem in writing to Log file.");
                objWriteLogSuccess = false;
            }
            return objWriteLogSuccess;
        }

        /// <summary>
        /// Method to get the Database Version.
        /// </summary>      
        /// <param name="version">Database version</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum GetDatabaseVersion(out string version)
        {
            if (HistoryLoggerConfiguration.Used)
            {
                return HistoryLogger.GetDatabaseVersion(out version);
            }
            else
            {
                version = "";
                return ResultCodeEnum.RequestAccepted;
            }
        }

           /// <summary>
        /// Method to insert the Database Version.
        /// </summary>      
        /// <param name="version">Database version</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum InsertDatabaseVersion(string version)
        {
            if (HistoryLoggerConfiguration.Used)
            {
                return HistoryLogger.InsertDatabaseVersion(version);
            }
            else
            {
                version = "";
                return ResultCodeEnum.RequestAccepted;
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
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum WriteLog(string context, Guid requestId, PIS.Ground.Core.Data.CommandType commandType, string trainId, MessageStatusType messageStatus, DateTime startDate, DateTime endDate)
        {
            if (HistoryLoggerConfiguration.Used)
            {
                return HistoryLogger.WriteLog(context, requestId, commandType, trainId, messageStatus, startDate, endDate);
            }
            else
            {
                return ResultCodeEnum.RequestAccepted;
            }
        }

        /// <summary>
        /// Method used to cancel history log details.
        /// </summary>
        /// <param name="requestId">request id of the message</param>
        /// <param name="commandType">command type</param>
        /// <param name="trainId">list of train ids</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum CancelLog(Guid requestId, PIS.Ground.Core.Data.CommandType commandType, string trainId, MessageStatusType messageStatus)
        {
            if (HistoryLoggerConfiguration.Used)
            {
                return HistoryLogger.CancelLog(requestId, commandType, trainId, messageStatus);
            }
            else
            {
                return ResultCodeEnum.RequestAccepted;
            }
        }

        /// <summary>
        /// Method used to get the latest message
        /// </summary>
        /// <param name="commandList">list of commands</param>
        /// <param name="logResponse">The latest message</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum GetLatestLog(List<PIS.Ground.Core.Data.CommandType> commandList, out string logResponse)
        {
            logResponse = string.Empty;
            if (HistoryLoggerConfiguration.Used)
            {
                return HistoryLogger.GetLatestLog(commandList, out logResponse);
            }
            else
            {
                return ResultCodeEnum.RequestAccepted;
            }
        }

        /// <summary>
        /// Method used to get the latest message DateTime
        /// </summary>
        /// <param name="commandList">list of commands</param>
        /// <param name="logResponse">The latest message</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum GetLatestLogDateTime(List<PIS.Ground.Core.Data.CommandType> commandList, out string logResponse)
        {
            logResponse = string.Empty;
            if (HistoryLoggerConfiguration.Used)
            {
                return HistoryLogger.GetLatestLogDateTime(commandList, out logResponse);
            }
            else
            {
                return ResultCodeEnum.RequestAccepted;
            }
        }

        /// <summary>
        /// Method used to get the oldest message
        /// </summary>
        /// <param name="commandList">list of commands</param>
        /// <param name="logResponse">The oldest message</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum GetOldestLog(List<PIS.Ground.Core.Data.CommandType> commandList, out string logResponse)
        {
            logResponse = string.Empty;
            if (HistoryLoggerConfiguration.Used)
            {
                return HistoryLogger.GetOldestLog(commandList, out logResponse);
            }
            else
            {
                return ResultCodeEnum.RequestAccepted;
            }
        }

        /// <summary>
        /// Method used to get the oldest message DateTime
        /// </summary>
        /// <param name="commandList">list of commands</param>
        /// <param name="logResponse">The oldest message</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum GetOldestLogDateTime(List<PIS.Ground.Core.Data.CommandType> commandList, out string logResponse)
        {
            logResponse = string.Empty;
            if (HistoryLoggerConfiguration.Used)
            {
                return HistoryLogger.GetOldestLogDateTime(commandList, out logResponse);
            }
            else
            {
                return ResultCodeEnum.RequestAccepted;
            }
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
            logResponse = string.Empty;
            if (HistoryLoggerConfiguration.Used)
            {
                return HistoryLogger.GetAllLog(commandList, startDateTime, endDateTime, language, out logResponse);
            }
            else
            {
                return ResultCodeEnum.RequestAccepted;
            }
        }

        /// <summary>
        /// Clean the history log database
        /// </summary>
        /// <param name="commandList">list of commands</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum CleanLog(List<PIS.Ground.Core.Data.CommandType> commandList)
        {
            if (HistoryLoggerConfiguration.Used)
            {
                return HistoryLogger.CleanLog(commandList);
            }
            else
            {
                return ResultCodeEnum.RequestAccepted;
            }
        }

        /// <summary>
        /// Method used to get all the status of the baselines
        /// </summary>
        /// <param name="dictionaryResponse">output dictionary response</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum GetTrainBaselineStatus(out Dictionary<string, TrainBaselineStatusData> dictionaryResponse)
        {
            ResultCodeEnum resultCode = ResultCodeEnum.RequestAccepted;
            dictionaryResponse = null;

            if (HistoryLoggerConfiguration.Used)
            {
                resultCode = HistoryLogger.GetTrainBaselineStatus(out dictionaryResponse);
            }

            return resultCode;
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
			ResultCodeEnum resultCode = ResultCodeEnum.RequestAccepted;

			if (HistoryLoggerConfiguration.Used)
			{
				resultCode = HistoryLogger.UpdateTrainBaselineStatus(
					trainId, requestId, taskId, trainNumber,
					onlineStatus, progressStatus,
					currentBaselineVersion, futureBaselineVersion,
					pisOnBoardVersion);
			}

			return resultCode;
		}

		/// <summary>
        /// Clean the Baseline progress status
        /// </summary>
        /// <param name="trainId">train id.</param>
        /// <returns>Error code if any</returns>
        public static ResultCodeEnum CleanTrainBaselineStatus(string trainId)
		{
			ResultCodeEnum resultCode = ResultCodeEnum.RequestAccepted;

			if (HistoryLoggerConfiguration.Used)
			{
				resultCode = HistoryLogger.CleanTrainBaselineStatus(trainId);
			}

			return resultCode;
		}

		/// <summary>Method to update the message status.</summary>
		/// <param name="trainId">list of train ids.</param>
		/// <param name="requestId">request id of the message.</param>
		/// <param name="messageStatus">message status.</param>
		/// <param name="commandType">Type of message to update.</param>
		/// <returns>Error code if any.</returns>
		public static ResultCodeEnum UpdateMessageStatus(string trainId, Guid requestId, MessageStatusType messageStatus, CommandType commandType)
        {
            if (HistoryLoggerConfiguration.Used)
            {
                return HistoryLogger.UpdateMessageStatus(trainId, requestId, messageStatus, commandType);
            }
            else
            {
                return ResultCodeEnum.RequestAccepted;
            }
        }

        /// <summary>
        /// Get Database Version From configuration file.
        /// </summary>
        /// <returns> return data base version</returns>
        public static string GetDatabaseVersionFromFile()
        {
            if (HistoryLoggerConfiguration.Used)
            {
                return HistoryLoggerConfiguration.LogDataBaseStructureVersion;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Generate an entry into the event log of an error.
        /// </summary>
        /// <param name="objException">The exception object.</param>
        /// <param name="strContext">The context of the error.</param>
        /// <param name="strUserMessage">The error message to display to the user.</param>
        /// <param name="objEntryType">Event log entry type.</param>
        /// <param name="ierrorNumber">Error number </param>
        /// <returns>if any error return false else true</returns>
        private static bool WriteEventLog(Exception objException, string strContext, string strUserMessage, EventLogEntryType objEntryType, int ierrorNumber)
        {
            try
            {
                StringBuilder errorMessage = new StringBuilder(500);
                if (strUserMessage == null)
                {
					if (objException != null)
					{
						errorMessage.Append(Resources.LogExceptionErrorMessage).Append(": ").AppendLine(objException.Message);
					}
					else
					{
						errorMessage.Append(Resources.LogExceptionErrorMessage);
					}
                }
                else
                {
                    errorMessage.Append(Resources.LogExceptionErrorMessage).Append(": ").AppendLine(strUserMessage);
                }

                errorMessage.Append(Resources.LogExceptionContext).Append(": ").AppendLine(strContext ?? string.Empty);
                DateTime errorDateTime = DateTime.Now;
                string localErrorDateTime = errorDateTime.ToString(DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                errorMessage.Append(Resources.LogExceptionDateTimeLocal).Append(": ").AppendLine(localErrorDateTime);
                string utcErrorDateTime = errorDateTime.ToUniversalTime().ToString(DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                errorMessage.Append(Resources.LogExceptionDateTimeUtc).Append(": ").AppendLine(utcErrorDateTime);
                System.Threading.Thread currentThread = System.Threading.Thread.CurrentThread;
                errorMessage.Append(Resources.LogExceptionThreadName).Append(": ").AppendLine(currentThread.Name);
                errorMessage.Append(Resources.LogExceptionThreadId).Append(": ").Append(currentThread.ManagedThreadId).AppendLine();
                errorMessage.AppendLine();
                if (objException != null)
                {
                    errorMessage.Append(Resources.LogExceptionMainExceptionMessage).Append(": ").AppendLine(objException.Message);
                    errorMessage.Append(Resources.LogExceptionExceptionType).Append(": ").AppendLine(objException.GetType().FullName);
                    int innerCount = 1;
                    for (Exception innerException = objException.InnerException; innerException != null; innerException = innerException.InnerException, ++innerCount)
                    {
                        errorMessage.AppendLine("==========================");
                        errorMessage.AppendFormat(Resources.LogExceptionInnerExceptionMessage, innerCount).Append(": ").AppendLine(innerException.Message);
                        errorMessage.AppendFormat(Resources.LogExceptionInnerExceptionType, innerCount).Append(": ").AppendLine(innerException.GetType().FullName);
                    }

                    errorMessage.AppendLine("==========================");
                    errorMessage.Append(Resources.LogExceptionStackTrace).Append(":\r\n");
                    errorMessage.Append(objException.StackTrace);
                }

                System.Diagnostics.EventLog.WriteEntry(EventLogSource, errorMessage.ToString(), objEntryType, ierrorNumber);

                return true;
                
            }
            catch (System.Exception)
            {
                return false;
            }
        }        
        
        #endregion

        #region ILogManager Members

        bool ILogManager.WriteLog(TraceType trace, string message, string context, Exception objEx, EventIdEnum eventId)
        {
            return LogManager.WriteLog(trace, message, context, objEx, eventId);
        }

        ResultCodeEnum ILogManager.GetDatabaseVersion(out string version)
        {
            return LogManager.GetDatabaseVersion(out version);
        }

        ResultCodeEnum ILogManager.InsertDatabaseVersion(string version)
        {
            return LogManager.InsertDatabaseVersion(version);
        }

        ResultCodeEnum ILogManager.WriteLog(string context, Guid requestId, CommandType commandType, string trainId, MessageStatusType messageStatus, DateTime startDate, DateTime endDate)
        {
            return LogManager.WriteLog(context, requestId, commandType, trainId, messageStatus, startDate, endDate);
        }

        ResultCodeEnum ILogManager.CancelLog(Guid requestId, CommandType commandType, string trainId, MessageStatusType messageStatus)
        {
            return LogManager.CancelLog(requestId, commandType, trainId, messageStatus);
        }

        ResultCodeEnum ILogManager.GetLatestLog(List<CommandType> commandList, out string logResponse)
        {
            return LogManager.GetLatestLog(commandList, out logResponse);
        }

        ResultCodeEnum ILogManager.GetLatestLogDateTime(List<CommandType> commandList, out string logResponse)
        {
            return LogManager.GetLatestLogDateTime(commandList, out logResponse);
        }

        ResultCodeEnum ILogManager.GetOldestLog(List<CommandType> commandList, out string logResponse)
        {
            return LogManager.GetOldestLog(commandList, out logResponse);
        }

        ResultCodeEnum ILogManager.GetOldestLogDateTime(List<CommandType> commandList, out string logResponse)
        {
            return LogManager.GetOldestLogDateTime(commandList, out logResponse);
        }

        ResultCodeEnum ILogManager.GetAllLog(List<CommandType> commandList, DateTime startDateTime, DateTime endDateTime, uint language, out string logResponse)
        {
            return LogManager.GetAllLog(commandList, startDateTime, endDateTime, language, out logResponse);
        }

        ResultCodeEnum ILogManager.CleanLog(List<CommandType> commandList)
        {
            return LogManager.CleanLog(commandList);
        }

        ResultCodeEnum ILogManager.GetTrainBaselineStatus(out Dictionary<string, TrainBaselineStatusData> dictionaryResponse)
        {
            return LogManager.GetTrainBaselineStatus(out dictionaryResponse);
        }

        ResultCodeEnum ILogManager.UpdateTrainBaselineStatus(string trainId, Guid requestId, int taskId, string trainNumber, bool onlineStatus, BaselineProgressStatusEnum progressStatus, string currentBaselineVersion, string futureBaselineVersion, string pisOnBoardVersion)
        {
            return LogManager.UpdateTrainBaselineStatus(trainId, requestId, taskId, trainNumber, onlineStatus, progressStatus, currentBaselineVersion, futureBaselineVersion, pisOnBoardVersion);
        }

        ResultCodeEnum ILogManager.CleanTrainBaselineStatus(string trainId)
        {
            return LogManager.CleanTrainBaselineStatus(trainId);
        }

		ResultCodeEnum ILogManager.UpdateMessageStatus(string trainId, Guid requestId, MessageStatusType messageStatus, CommandType commandType)
        {
            return LogManager.UpdateMessageStatus(trainId, requestId, messageStatus, commandType);
        }

        string ILogManager.GetDatabaseVersionFromFile()
        {
            return LogManager.GetDatabaseVersionFromFile();
        }

        #endregion
    }
}
