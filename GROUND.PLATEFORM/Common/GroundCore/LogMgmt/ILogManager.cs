// <copyright file="ILogManager.cs" company="Alstom Transport Telecite Inc.">
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
    public interface ILogManager
    {
        
        #region Execution trace logs
        
        /// <summary>
        /// Logs the errors to log file.
        /// </summary>
        /// <param name="trace">Type of trace.</param>
        /// <param name="message">User Message to be written.</param>
        /// <param name="context">The context where the error has occurred.</param>
        /// <param name="objEx">The exception object.</param>
        /// <param name="eventId">The EventIdEnum to facilitate reading in log manager.</param>
        /// <returns>if any error return false else true</returns>
        bool WriteLog(TraceType trace, string message, string context, Exception objEx, EventIdEnum eventId);

        /// <summary>
        /// Determines whether trace is active for the specified trace level.
        /// </summary>
        /// <param name="level">The level to evaluate.</param>
        /// <returns>true if the trace is active, false otherwise.</returns>
        bool IsTraceActive(TraceType level);

        #endregion

        #region Database management 

        /// <summary>
        /// Method to get the Database Version.
        /// </summary>      
        /// <param name="version">Database version</param>
        /// <returns>Error code if any</returns>
        ResultCodeEnum GetDatabaseVersion(out string version);

        /// <summary>
        /// Get Database Version From configuration file.
        /// </summary>
        /// <returns> return data base version</returns>
        string GetDatabaseVersionFromFile();

        /// <summary>
        /// Method to insert the Database Version.
        /// </summary>      
        /// <param name="version">Database version</param>
        /// <returns>Error code if any</returns>
        ResultCodeEnum InsertDatabaseVersion(string version);

        #endregion

        #region Instant Message logs

        /// <summary>Method to update the message status.</summary>
        /// <param name="trainId">list of train ids.</param>
        /// <param name="requestId">request id of the message.</param>
        /// <param name="messageStatus">message status.</param>
        /// <param name="commandType">The type of message to update.</param>
        /// <returns>Error code if any.</returns>
        ResultCodeEnum UpdateMessageStatus(string trainId, Guid requestId, MessageStatusType messageStatus, PIS.Ground.Core.Data.CommandType commandType);

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
        ResultCodeEnum WriteLog(string context, Guid requestId, PIS.Ground.Core.Data.CommandType commandType, string trainId, MessageStatusType messageStatus, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Method used to cancel history log details.
        /// </summary>
        /// <param name="requestId">request id of the message</param>
        /// <param name="commandType">command type</param>
        /// <param name="trainId">list of train ids</param>
        /// <returns>Error code if any</returns>
        ResultCodeEnum CancelLog(Guid requestId, PIS.Ground.Core.Data.CommandType commandType, string trainId, MessageStatusType messageStatus);

        /// <summary>
        /// Method used to get the latest message
        /// </summary>
        /// <param name="commandList">list of commands</param>
        /// <param name="logResponse">The latest message</param>
        /// <returns>Error code if any</returns>
        ResultCodeEnum GetLatestLog(List<PIS.Ground.Core.Data.CommandType> commandList, out string logResponse);

        /// <summary>
        /// Method used to get the latest message DateTime
        /// </summary>
        /// <param name="commandList">list of commands</param>
        /// <param name="logResponse">The latest message</param>
        /// <returns>Error code if any</returns>
        ResultCodeEnum GetLatestLogDateTime(List<PIS.Ground.Core.Data.CommandType> commandList, out string logResponse);

        /// <summary>
        /// Method used to get the oldest message
        /// </summary>
        /// <param name="commandList">list of commands</param>
        /// <param name="logResponse">The oldest message</param>
        /// <returns>Error code if any</returns>
        ResultCodeEnum GetOldestLog(List<PIS.Ground.Core.Data.CommandType> commandList, out string logResponse);

        /// <summary>
        /// Method used to get the oldest message DateTime
        /// </summary>
        /// <param name="commandList">list of commands</param>
        /// <param name="logResponse">The oldest message</param>
        /// <returns>Error code if any</returns>
        ResultCodeEnum GetOldestLogDateTime(List<PIS.Ground.Core.Data.CommandType> commandList, out string logResponse);

        /// <summary>
        /// Method used to get all the log message
        /// </summary>
        /// <param name="commandList">command list</param>
        /// <param name="startDateTime">start date time to be considered</param>
        /// <param name="endDateTime">end date time to be considered</param>
        /// <param name="language">language code</param>
        /// <param name="logResponse">output log response</param>
        /// <returns>Error code if any</returns>
        ResultCodeEnum GetAllLog(List<PIS.Ground.Core.Data.CommandType> commandList, DateTime startDateTime, DateTime endDateTime, uint language, out string logResponse);

        /// <summary>
        /// Clean the history log database
        /// </summary>
        /// <param name="commandList">list of commands</param>
        /// <returns>Error code if any</returns>
        ResultCodeEnum CleanLog(List<PIS.Ground.Core.Data.CommandType> commandList);

        #endregion

        #region Train baseline statuses

        /// <summary>
        /// Method used to get all the status of the baselines
        /// </summary>
        /// <param name="dictionaryResponse">output dictionary response</param>
        /// <returns>Error code if any</returns>
        ResultCodeEnum GetTrainBaselineStatus(out Dictionary<string, TrainBaselineStatusData> dictionaryResponse);

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
        ResultCodeEnum UpdateTrainBaselineStatus(string trainId, Guid requestId, int taskId, string trainNumber,
                                                               bool onlineStatus, BaselineProgressStatusEnum progressStatus,
                                                               string currentBaselineVersion, string futureBaselineVersion,
                                                               string pisOnBoardVersion);

        /// <summary>
        /// Clean the Baseline progress status
        /// </summary>
        /// <param name="trainId">train id.</param>
        /// <returns>Error code if any</returns>
        ResultCodeEnum CleanTrainBaselineStatus(string trainId);

        #endregion
    }
}
