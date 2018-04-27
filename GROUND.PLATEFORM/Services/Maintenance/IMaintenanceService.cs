// <copyright file="IMaintenanceService.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2011.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
namespace PIS.Ground.Maintenance
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using PIS.Ground.Core.Data;

    /// <summary>
    /// Interface Maintenance Service.
    /// </summary>
    [ServiceContract(Namespace = "http://alstom.com/pacis/pis/ground/maintenance/")]
    public interface IMaintenanceService
    {
        [OperationContract]
        MaintenanceElementListResponse GetAvailableElementList(Guid sessionId);

        [OperationContract]
        MaintenanceResponse GetSystemMessagesFiles(Guid sessionId, TargetAddressType targetAddress, uint requestTimeout);

        [OperationContract]
        MaintenanceResponse GetVersionsFile(Guid sessionId, TargetAddressType targetAddress, uint requestTimeout);

        /// <summary>
        /// Method to get the history log.
        /// </summary>
        /// <param name="sessionId">Input session id.</param>
        /// <param name="commandList">List of commands.</param>
        /// <param name="startDateTime">Start date of search.</param>
        /// <param name="endDateTime">End date of search.</param>
        /// <param name="language">Language used.</param>
        /// <returns>Log response.</returns>
        [OperationContract]
        HistoryLogResponse GetLogs(Guid sessionId, List<CommandType> commandList, DateTime startDateTime, DateTime endDateTime, uint language);

        /// <summary>
        /// Get the oldest log date time.
        /// </summary>
        /// <param name="sessionId">Input session id.</param>
        /// <param name="commandList">List of commands.</param>
        /// <returns>Log response</returns>
        [OperationContract]
        HistoryLogResponse GetOldestLogDateTime(Guid sessionId, List<CommandType> commandList);

        /// <summary>
        /// Get the latest log date time.
        /// </summary>
        /// <param name="sessionId">Input session id.</param>
        /// <param name="commandList">List of commands.</param>
        /// <returns>Log response.</returns>
        [OperationContract]
        HistoryLogResponse GetLatestLogDateTime(Guid sessionId, List<CommandType> commandList);

        /// <summary>
        /// Method used to clean the log based on command.
        /// </summary>
        /// <param name="sessionId">Input session id.</param>
        /// <param name="commandList">List of commands.</param>
        /// <returns>Maintenance response.</returns>
        [OperationContract]
        MaintenanceResponse CleanLog(Guid sessionId, List<CommandType> commandList);

        /// <summary>
        /// OBSOLETE - Get the statuses of the baselines current and future.
        /// NOTE: This method is obsolete. Use GetFleetBaselineStatus() instead. 
        /// </summary>
        /// <param name="sessionId">Input session id.</param>
        /// <returns>Baseline Status response.</returns>
        [OperationContract]
        MaintenanceTrainBaselineStatusListResponse GetTrainBaselineStatus(Guid sessionId);

        /// <summary>
        /// Get the statuses of the baselines current and future for the train fleet.
        /// NOTE: Replaces GetTrainBaselineStatus().
        /// </summary>
        /// <param name="sessionId">Input session id.</param>
        /// <returns>The fleet Baseline Status response.</returns>
        [OperationContract]
        MaintenanceTrainBaselineStatusListResponse GetFleetBaselineStatus(Guid sessionId);

        [OperationContract(Name = "GetFleetBaselineStatus_v2")]
        MaintenanceTrainBaselineStatusListResponse GetFleetBaselineStatus(Guid sessionId, int version);
    }

    /// <summary>
    /// Class representing Maintenance baseline status list response.
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/maintenance/", Name = "MaintenanceTrainBaselineStatusListOutput")]
    public class MaintenanceTrainBaselineStatusListResponse
    {
        /// <summary>
        /// Result code of the query
        /// </summary>
        [DataMember]
        public ResultCodeEnum ResultCode;

        /// <summary>
        /// List of baseline status
        /// </summary>
        [DataMember]
        public TrainBaselineStatusList<TrainBaselineStatusData> TrainBaselineStatusList;
    }

    /// <summary>
    /// Class representing Maintenance element list response.
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/maintenance/", Name = "MaintenanceElementListOutput")]
    public class MaintenanceElementListResponse
    {     
        /// <summary>
        /// Result code of the query
        /// </summary>
        [DataMember]
        public ResultCodeEnum ResultCode;

        /// <summary>
        /// List of elements
        /// </summary>
        [DataMember]
        public ElementList<AvailableElementData> ElementList;
    }
}
