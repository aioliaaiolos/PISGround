//---------------------------------------------------------------------------------------------------
// <copyright file="EnumTypes.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System.ComponentModel;
using System.Runtime.Serialization;

namespace PIS.Ground.Core.Data
{
    /// <summary>
    /// Logging types
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Representing Debug
        /// </summary>
        EVENT = 0,

        /// <summary>
        /// Representing Information
        /// </summary>
        DATABASE = 1,

        /// <summary>
        /// Representing Error
        /// </summary>
        BOTH = 2,

    }

    /// <summary>
    /// Different Traces
    /// </summary>
    public enum TraceType
    {
        /// <summary>
        /// Representing None
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Representing Debug
        /// </summary>
        DEBUG = 1,

        /// <summary>
        /// Representing Information
        /// </summary>
        INFO = 2,

        /// <summary>
        /// Representing Error
        /// </summary>
        ERROR = 3,

        /// <summary>
        /// Representing Warning
        /// </summary>
        WARNING = 4,

        /// <summary>
        /// Representing Exception
        /// </summary>
        EXCEPTION = 5

    }

    /// <summary>
    ///  Defines different event ids for applications
    /// </summary>
    public enum EventIdEnum
    {
        /// <summary>
        /// Default value.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Data package service.
        /// </summary>
        DataPackage = 1,

        /// <summary>
        /// GroundCore module.
        /// </summary>
        GroundCore = 2,

        /// <summary>
        /// Infotainment journaling service.
        /// </summary>
        InfotainmentJournaling = 3,

        /// <summary>
        /// Instant message service.
        /// </summary>
        InstantMessage = 4,

        /// <summary>
        /// Maintenance service.
        /// </summary>
        Maintenance = 5,

        /// <summary>
        /// Mission service.
        /// </summary>
        Mission = 6,

        /// <summary>
        /// Remote datastore service.
        /// </summary>
        RemoteDataStore = 7,

        /// <summary>
        /// Session service.
        /// </summary>
        Session = 8,

        /// <summary>
        /// Debug trace.
        /// </summary>
        Debug = 9,

        /// <summary>
        /// History log module.
        /// </summary>
        HistoryLog = 10,

        /// <summary>
        /// Live video control service.
        /// </summary>
        LiveVideoControl = 11,

        /// <summary>
        /// Real-time service.
        /// </summary>
        RealTime = 12,

        /// <summary>
        /// Error when notifying external application fails.
        /// </summary>
        SendNotification = 13
    }

    /// <summary>
    /// Represents the Status of the Request
    /// </summary>
    public enum RequestStatus
    {
        /// <summary>
        /// Represents Status In progress
        /// </summary>
        InProgress = 0,

        /// <summary>
        /// Represents Status Finished
        /// </summary>
        Finished = 1,

        /// <summary>
        /// Represents Status Canceled
        /// </summary>
        Canceled = 2
    }

    /// <summary>
    /// Different status of Task
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// Task Created
        /// </summary>
        Created = 0,

        /// <summary>
        /// Task Started
        /// </summary>
        Started = 1,

        /// <summary>
        /// Task Stopped
        /// </summary>
        Stopped = 2,

        /// <summary>
        /// Task Cancelled
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// Task Completed
        /// </summary>
        Completed = 4,

        /// <summary>
        /// Task Error
        /// </summary>
        Error = 5
    }

    /// <summary>
    /// Different Phase of Task
    /// </summary>
    public enum TaskPhase 
    {
        /// <summary>
        /// Phase Creation
        /// </summary>
        Creation = 0,

        /// <summary>
        /// Phase Acquisition
        /// </summary>
        Acquisition = 1,

        /// <summary>
        /// Phase Transfer
        /// </summary>
        Transfer = 2,

        /// <summary>
        /// Phase Distribution
        /// </summary>
        Distribution = 3
    }

    /// <summary>
    /// Types of Transfer
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/core/data/", Name = "TransferType")]
    public enum TransferType
    {
        /// <summary>
        /// Transfer Train To Ground
        /// </summary>
        TrainToGround = 0,

        /// <summary>
        /// Transfer Ground To Train
        /// </summary>
        GroundToTrain = 1
    }

    /// <summary>
    /// Represents File Transfer Mode
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "FileTransferMode")]
    public enum FileTransferMode 
    {
        /// <summary>
        /// Mode LowBandwidth
        /// </summary>
        [EnumMember]
        LowBandwidth = 0,

        /// <summary>
        /// Mode HighBandwidth
        /// </summary>
        [EnumMember]
        HighBandwidth = 1,

        /// <summary>
        /// Mode AnyBandwidth
        /// </summary>
        [EnumMember]
        AnyBandwidth = 2
    }

    /// <summary>
    /// Represents Acquisition State
    /// </summary>
    public enum AcquisitionState
    {
        /// <summary>
        /// AcquisitionState NotAcquired
        /// </summary>
        NotAcquired = 0,

        /// <summary>
        /// AcquisitionState AcquisitionStarted
        /// </summary>
        AcquisitionStarted = 1,

        /// <summary>
        /// AcquisitionState AcquisitionStopped
        /// </summary>
        AcquisitionStopped = 2,

        /// <summary>
        /// AcquisitionState AcquisitionError
        /// </summary>
        AcquisitionError = 3,

        /// <summary>
        /// AcquisitionState AcquisitionSuccess
        /// </summary>
        AcquisitionSuccess = 4
    }

    /// <summary>
    /// Represents Mission Insertion Type
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "MissionInsertionType")]
    public enum MissionInsertionType
    {
        [DataMember]
        StoppedatStation = 0,
        [DataMember]
        HeadingTowardStation = 1
    }

    /// <summary>
    ///  Represents the type of a file : UNC, ftp, http
    /// </summary>
    public enum FileTypeEnum
    {
        Undefined   = 0,
        LocalFile   = 1,
        FtpFile     = 2,
        HttpFile    = 3
    }

    /// <summary>
    /// Types of Scheduled Message command
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/core/data/", Name = "CommandType")]
    public enum CommandType
    {
        /// <summary>
        /// All logs
        /// </summary>
        [EnumMember(Value = "ALL_LOGS")]
        AllLogs = 0,

        /// <summary>
        /// Send Scheduled Msg
        /// </summary>
        [EnumMember(Value = "SEND_SCHEDULE_MESSAGE")]
        SendScheduledMessage = 1,

        /// <summary>
        /// Cancel Scheduled Msg
        /// </summary>
        [EnumMember(Value = "CANCEL_SCHEDULE_MESSAGE")]
        CancelScheduledMessage = 2
    }

    /// <summary>
    /// Types of MessageStatus
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/core/data/", Name = "MessageStatus")]
    public enum MessageStatusType
    {
        /// <summary>
        /// InstantMessage Distribution Processing Status
        /// </summary>
        [EnumMember(Value = "InstantMessageDistributionProcessing")]
        InstantMessageDistributionProcessing = 0,

        /// <summary>
        /// InstantMessage Distribution Received Status
        /// </summary>
        [EnumMember(Value = "InstantMessageDistributionReceived")]
        InstantMessageDistributionReceived = 1,

        /// <summary>
        /// InstantMessage Distribution Waiting To Send Status
        /// </summary>
        [EnumMember(Value = "InstantMessageDistributionWaitingToSend")]
        InstantMessageDistributionWaitingToSend = 2,

        /// <summary>
        /// InstantMessage Distribution Sent Status
        /// </summary>
        [EnumMember(Value = "InstantMessageDistributionSent")]
        InstantMessageDistributionSent = 3,

        /// <summary>
        /// InstantMessage Distribution TimedOut Status
        /// </summary>
        [EnumMember(Value = "InstantMessageDistributionTimedOut")]
        InstantMessageDistributionTimedOut = 5,

		/// <summary>
		/// An enum constant representing that instant message distribution failed because it was canceled on startup.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionCanceledByStartupError")]
        InstantMessageDistributionCanceledByStartupError = 6,

		/// <summary>
		/// An enum constant representing that instant message distribution failed because the template is invalid.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionInvalidTemplateError")]
        InstantMessageDistributionInvalidTemplateError = 7,

		/// <summary>
		/// An enum constant representing that instant message distribution failed because the schedule period is invalid.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionInvalidScheduledPeriodError")]
        InstantMessageDistributionInvalidScheduledPeriodError = 8,

		/// <summary>
		/// An enum constant representing that instant message distribution failed because the repetition count is invalid.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionInvalidRepetitionCountError")]
        InstantMessageDistributionInvalidRepetitionCountError = 9,

		/// <summary>
		/// An enum constant representing that instant message distribution failed because the template file is invalid.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionInvalidTemplateFileError")]
        InstantMessageDistributionInvalidTemplateFileError = 10,

		/// <summary>
		/// An enum constant representing that instant message distribution failed because the car identifier is unknown.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionUnknownCarIdError")]
        InstantMessageDistributionUnknownCarIdError = 11,

		/// <summary>
		/// An enum constant representing that instant message distribution failed because the delay is invalid.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionInvalidDelayError")]
        InstantMessageDistributionInvalidDelayError = 12,

		/// <summary>
		/// An enum constant representing that instant message distribution failed because the delay reason is invalid.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionInvalidDelayReasonError")]
        InstantMessageDistributionInvalidDelayReasonError = 13,

		/// <summary>
		/// An enum constant representing that instant message distribution failed because the hour is invalid.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionInvalidHourError")]
        InstantMessageDistributionInvalidHourError = 14,

		/// <summary>
		/// An enum constant representing that instant message distribution failed because the station identifier is invalid.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionUndefinedStationIdError")]
        InstantMessageDistributionUndefinedStationIdError = 15,

		/// <summary>
		/// An enum constant representing that instant message distribution was canceled.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionCanceled")]
		InstantMessageDistributionCanceled = 16,

		/// <summary>
		/// An enum constant representing that instant message distribution was canceled on ground system only.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionCanceledGroundOnly")]
		InstantMessageDistributionCanceledGroundOnly = 17,

		/// <summary>
		/// An enum constant representing that instant message distribution failed because the provided text is invalid.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionInvalidTextError")]
		InstantMessageDistributionInvalidTextError = 18,

		/// <summary>
		/// An enum constant representing that instant message distribution failed the message limit on embedded side has been exceeded.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionMessageLimitExceededError")]
		InstantMessageDistributionMessageLimitExceededError = 19,

		/// <summary>
		/// An enum constant representing that instant message distribution failed because it is inhibited on embedded side.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionInhibited")]
		InstantMessageDistributionInhibited = 20,

		/// <summary>
		/// An enum constant representing that instant message distribution failed because an unexpected error occurred.
		/// </summary>
		[EnumMember(Value = "InstantMessageDistributionUnexpectedError")]
		InstantMessageDistributionUnexpectedError = 21
	}

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/core/data/", Name = "ResultCodeEnum")]
    public enum ResultCodeEnum
    {
        [EnumMember(Value = "REQUEST_ACCEPTED")]
        RequestAccepted,

        [EnumMember(Value = "INVALID_SESSION_ID")]
        InvalidSessionId,

        [EnumMember(Value = "INVALID_REQUEST_TIMEOUT")]
        InvalidRequestTimeout,

        [EnumMember(Value = "UNKNOWN_ELEMENT_ID")]
        UnknownElementId,

        [EnumMember(Value = "UNKNOWN_MISSION_ID")]
        UnknownMissionId,

        [EnumMember(Value = "ELEMENT_LIST_NOT_AVAILABLE")]
        ElementListNotAvailable,

        [EnumMember(Value = "INTERNAL_ERROR")]
        InternalError,

        [EnumMember(Value = "SQL_ERROR")]
        SqlError,

        [EnumMember(Value = "INVALID_COMMAND_TYPE")]
        InvalidCommandType,

        [EnumMember(Value = "INVALID_END_DATE")]
        InvalidEndDate,

        [EnumMember(Value = "INVALID_START_DATE")]
        InvalidStartDate,

        [EnumMember(Value = "INVALID_CONTEXT")]
        InvalidContext,

        [EnumMember(Value = "INVALID_REQUEST_ID")]
        InvalidRequestID,

        [EnumMember(Value = "INVALID_TRAIN_ID")]
        InvalidTrainID,

        [EnumMember(Value = "INVALID_STATUS")]
        InvalidStatus,

        [EnumMember(Value = "T2G_SERVER_OFFLINE")]
        T2GServerOffline,

        [EnumMember(Value = "OUTPUT_LIMIT_EXCEEDED")]
        OutputLimitExceed,

        [EnumMember(Value = "EMPTY_RESULT")]
        Empty_Result
    }

	/// <summary>Values that represent Mission State.</summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/core/data/", Name = "MissionStateEnum")]
	public enum MissionStateEnum
	{
		/// <summary>An enum constant representing the mission initialized option.</summary>
		[EnumMember(Value = "MI")]
		MI = 0,

		/// <summary>An enum constant representing mission end option.</summary>
		[EnumMember(Value = "ME")]
		ME = 1,

		/// <summary>An enum constant representing the not initialized option.</summary>
		[EnumMember(Value = "NI")]
		NI = 2,

		/// <summary>An enum constant representing the mission initialize error option.</summary>
		[EnumMember(Value = "MIE")]
		MIE = 3,

		/// <summary>An enum constant representing the degraded option.</summary>
		[EnumMember(Value = "DEG")]
		DEG = 4
	}

    /// <summary>Values that represent Baseline Progress Status.</summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/core/data/", Name = "BaselineProgressStatusEnum")]
    public enum BaselineProgressStatusEnum
    {
        /// <summary>An enum constant representing the updated state.</summary>
        [EnumMember(Value = "UPDATED")]
        [Description("Updated")]
        UPDATED = 0,

        /// <summary>An enum constant representing deployed state.</summary>
        [EnumMember(Value = "DEPLOYED")]
        [Description("Deployed")]
        DEPLOYED = 1,

        /// <summary>An enum constant representing the transfer completed state.</summary>
        [EnumMember(Value = "TRANSFER_COMPLETED")]
        [Description("TransferCompleted")]
        TRANSFER_COMPLETED = 2,
        
        /// <summary>An enum constant representing the transfer in progress state.</summary>
        [EnumMember(Value = "TRANSFER_IN_PROGRESS")]
        [Description("TransferInProgress")]
        TRANSFER_IN_PROGRESS = 3,

        /// <summary>An enum constant representing the transfer paused state.</summary>
        [EnumMember(Value = "TRANSFER_PAUSED")]
        [Description("TransferPaused")]
        TRANSFER_PAUSED = 4,

        /// <summary>An enum constant representing the transfer planned state.</summary>
        [EnumMember(Value = "TRANSFER_PLANNED")]
        [Description("TransferPlanned")]
        TRANSFER_PLANNED = 5,
        
        /// <summary>An enum constant representing the unknown state.</summary>
        [EnumMember(Value = "UNKNOWN")]
        [Description("Unknown")]
        UNKNOWN = 6

    }

}