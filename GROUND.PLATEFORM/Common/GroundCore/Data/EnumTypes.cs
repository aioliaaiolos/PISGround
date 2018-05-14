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
        UNKNOWN = 12,


		/// <summary>An enum constant representing the deployment state.</summary>
        [EnumMember(Value = "DEPLOYMENT")]
        [Description("Deployment")]
        DEPLOYMENT = 7,

		/// <summary>An enum constant representing the baseline Transfer to T2G FTP repository state.</summary>
        [EnumMember(Value = "BASELINE_TRANSFER_TO_T2G_FTP_REPOSITORY")]
        [Description("BaselineTransferToT2GFtpRepository")]
        BASELINE_TRANSFER_TO_T2G_FTP_REPOSITORY = 9,

        /// <summary>An enum constant representing the baseline transfer to T2G embedded state.</summary>
        [EnumMember(Value = "BASELINE_TRANSFER_TO_T2G_EMBEDDED")]
        [Description("BaselineTransferToT2GEmbedded")]
        BASELINE_TRANSFER_TO_T2G_EMBEDDED = 8,

        /// <summary>An enum constant representing the baseline onboard distribution state.</summary>
        [EnumMember(Value = "BASELINE_ONBOARD_DISTRIBUTION")]
        [Description("BaselineOnboardDistribution")]
        BASELINE_ONBOARD_DISTRIBUTION = 10,

        /// <summary>An enum constant representing the baseline onboard distribution state.</summary>
        [EnumMember(Value = "BASELINE_ONBOARD_ACTIVATION")]
        [Description("BaselineOnboardActivation")]
        BASELINE_ONBOARD_ACTIVATION = 11
    }

    /// <summary>Values that represent Baseline Progress Status state.</summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/core/data/", Name = "BaselineProgressStatusStateEnum")]
    public enum BaselineProgressStatusStateEnum
    {
        /// <summary> An enum constant representing the activated state.</summary>
        [EnumMember(Value = "NONE")]
        [Description("None")]
        NONE = -1,

        /// <summary> An enum constant representing the activated state.</summary>
        [EnumMember(Value = "ACTIVATED")]
        [Description("Activated")]
        ACTIVATED = 0,

        /// <summary> An enum constant representing the activation error state.</summary>
        [EnumMember(Value = "ACTIVATION_ERROR")]
        [Description("ActivationError")]
        ACTIVATION_ERROR = 1,

        /// <summary> An enum constant representing the waiting for communication state.</summary>
        [EnumMember(Value = "ACTIVATION_WAITING_FOR_COMMUNICATION")]
        [Description("ActivationWaitingForCommunication")]
        ACTIVATION_WAITING_FOR_COMMUNICATION = 2,

        /// <summary> An enum constant representing the authentification error state.</summary>
        [EnumMember(Value = "AUTHENTIFICATION_ERROR")]
        [Description("AuthenticationError")]
        AUTHENTIFICATION_ERROR = 3,

        /// <summary> An enum constant representing the baseline already exists state.</summary>
        [EnumMember(Value = "BASELINE_ALREADY_EXISTS")]
        [Description("BaselineAlreadyExists")]
        BASELINE_ALREADY_EXISTS = 4,

        /// <summary> An enum constant representing the baseline mismatch state.</summary>
        [EnumMember(Value = "BASELINE_MISMATCH")]
        [Description("BaselineMismatch")]
        BASELINE_MISMATCH = 5,

        /// <summary> An enum constant representing the baseline not found state.</summary>
        [EnumMember(Value = "BASELINE_NOT_FOUND")]
        [Description("BaselineNotFound")]
        BASELINE_NOT_FOUND = 6,

        /// <summary> An enum constant representing the canceled by startup state.</summary>
        [EnumMember(Value = "CANCELED_BY_STARTUP_ERROR")]
        [Description("CanceledByStartupError")]
        CANCELED_BY_STARTUP_ERROR = 7,

        /// <summary> An enum constant representing the communication error state.</summary>
        [EnumMember(Value = "COMMUNICATION_ERROR")]
        [Description("CommunicationError")]
        COMMUNICATION_ERROR = 8,

        /// <summary> An enum constant representing the completed state.</summary>
        [EnumMember(Value = "COMPLETED")]
        [Description("Completed")]
        COMPLETED = 9,

        /// <summary> An enum constant representing the compression error state.</summary>
        [EnumMember(Value = "COMPRESSION_ERROR")]
        [Description("CompressionError")]
        COMPRESSION_ERROR = 10,

        /// <summary> An enum constant representing the content error state.</summary>
        [EnumMember(Value = "CONTENT_ERROR")]
        [Description("ContentError")]
        CONTENT_ERROR = 11,

        /// <summary> An enum constant representing the database access error state.</summary>
        [EnumMember(Value = "DATABASE_ACCESS_ERROR")]
        [Description("DatabaseAccessError")]
        DATABASE_ACCESS_ERROR = 12,

        /// <summary> An enum constant representing the delivery error state.</summary>
        [EnumMember(Value = "DELIVERY_ERROR")]
        [Description("DeliveryError")]
        DELIVERY_ERROR = 13,

        /// <summary> An enum constant representing the distribution error state.</summary>
        [EnumMember(Value = "DISTRIBUTION_ERROR")]
        [Description("DistributionError")]
        DISTRIBUTION_ERROR = 14,

        /// <summary> An enum constant representing the distribution timeout state.</summary>
        [EnumMember(Value = "DISTRIBUTION_TIMEOUT")]
        [Description("DistributionTimeout")]
        DISTRIBUTION_TIMEOUT = 16,

        /// <summary> An enum constant representing the distribution waiting for communication state.</summary>
        [EnumMember(Value = "DISTRIBUTION_WAITING_FOR_COMMUNICATION")]
        [Description("DistributionWaitingForCommunication")]
        DISTRIBUTION_WAITING_FOR_COMMUNICATION = 17,

        /// <summary> An enum constant representing the ftp error state.</summary>
        [EnumMember(Value = "FTP_ERROR")]
        [Description("FtpError")]
        FTP_ERROR = 18,

        /// <summary> An enum constant representing the ground baseline mismatch state.</summary>
        [EnumMember(Value = "GROUND_BASELINE_MISMATCH")]
        [Description("GroundBaselineMismatch")]
        GROUND_BASELINE_MISMATCH = 19,

        /// <summary> An enum constant representing the in progress state.</summary>
        [EnumMember(Value = "IN_PROGRESS")]
        [Description("InProgress")]
        IN_PROGRESS = 20,

        /// <summary> An enum constant representing the invalid element id state.</summary>
        [EnumMember(Value = "INVALID_ELEMENT_ID")]
        [Description("InvalidElementId")]
        INVALID_ELEMENT_ID = 21,

        /// <summary> An enum constant representing the invalid exploration date time state.</summary>
        [EnumMember(Value = "INVALID_EXPLORATION_DATE_TIME")]
        [Description("InvalidExpirationDateTime")]
        INVALID_EXPLORATION_DATE_TIME = 22,

        /// <summary> An enum constant representing the invalid session id state.</summary>
        [EnumMember(Value = "INVALID_SESSION_ID")]
        [Description("InvalidSessionId")]
        INVALID_SESSION_ID = 23,

        /// <summary> An enum constant representing the missing file state.</summary>
        [EnumMember(Value = "MISSING_FILE")]
        [Description("MissingFile")]
        MISSING_FILE = 24,

        /// <summary> An enum constant representing the no disk space state.</summary>
        [EnumMember(Value = "NO_DISK_SPACE")]
        [Description("NoDiskSpace")]
        NO_DISK_SPACE = 25,

        /// <summary> An enum constant representing the no write permission state.</summary>
        [EnumMember(Value = "NO_WRITE_PERMISSION")]
        [Description("NoWritePermission")]
        NO_WRITE_PERMISSION = 26,

        /// <summary> An enum constant representing the out of memory error state.</summary>
        [EnumMember(Value = "OUT_OF_MEMORY_ERROR")]
        [Description("OutOfMemoryError")]
        OUT_OF_MEMORY_ERROR = 27,

        /// <summary> An enum constant representing the package already exist error state.</summary>
        [EnumMember(Value = "PACKAGE_ALREADY_EXIST_ERROR")]
        [Description("PackageAlreadyExistError")]
        PACKAGE_ALREADY_EXIST_ERROR = 28,

        /// <summary> An enum constant representing the package not error state.</summary>
        [EnumMember(Value = "PACKAGE_NOT_FOUND")]
        [Description("PackagesNotFound")]
        PACKAGE_NOT_FOUND = 29,

        /// <summary> An enum constant representing the packaging error state.</summary>
        [EnumMember(Value = "PACKAGING_ERROR")]
        [Description("PackagingError")]
        PACKAGING_ERROR = 30,

        /// <summary> An enum constant representing the planned state.</summary>
        [EnumMember(Value = "PLANNED")]
        [Description("Planned")]
        PLANNED = 31,

        /// <summary> An enum constant representing the program error state.</summary>
        [EnumMember(Value = "PROGRAM_ERROR")]
        [Description("ProgramError")]
        PROGRAM_ERROR = 32,

        /// <summary> An enum constant representing the remote datastore error state.</summary>
        [EnumMember(Value = "REMOTE_DATASTORE_ERROR")]
        [Description("RemoteDataStoreError")]
        REMOTE_DATASTORE_ERROR = 33,

        /// <summary> An enum constant representing the T2F error state.</summary>
        [EnumMember(Value = "T2G_ERROR")]
        [Description("T2GError")]
        T2G_ERROR = 34,

        /// <summary> An enum constant representing the T2G server online state.</summary>
        [EnumMember(Value = "T2G_SERVER_OFFLINE")]
        [Description("T2GServerOffline")]
        T2G_SERVER_OFFLINE = 35,

        /// <summary> An enum constant representing the timeout expired state.</summary>
        [EnumMember(Value = "TIMEOUT_EXPIRED")]
        [Description("TimeoutExpired")]
        TIMEOUT_EXPIRED = 36,

        /// <summary> An enum constant representing the transfer cancelled updated state.</summary>
        [EnumMember(Value = "TRANSFER_CANCELLED")]
        [Description("TransferCancelled")]
        TRANSFER_CANCELLED = 37,

        /// <summary> An enum constant representing the transfer completed state.</summary>
        [EnumMember(Value = "TRANSFER_COMPLETED")]
        [Description("TransferCompleted")]
        TRANSFER_COMPLETED = 38,

        /// <summary> An enum constant representing the transfer creation in progress state.</summary>
        [EnumMember(Value = "TRANSFER_CREATION_IN_PROGRESS")]
        [Description("TransferCreationInProgress")]
        TRANSFER_CREATION_IN_PROGRESS = 39,

        /// <summary> An enum constant representing the transfer in progress state.</summary>
        [EnumMember(Value = "TRANSFER_IN_PROGRESS")]
        [Description("TransferInProgress")]
        TRANSFER_IN_PROGRESS = 40,

        /// <summary> An enum constant representing the transfer planned state.</summary>
        [EnumMember(Value = "TRANSFER_PLANNED")]
        [Description("TransferPlanned")]
        TRANSFER_PLANNED = 41,

        /// <summary> An enum constant representing the transfer paused state.</summary>
        [EnumMember(Value = "TRANSFER_PAUSED")]
        [Description("TransferPaused")]
        TRANSFER_PAUSED = 42,

        /// <summary> An enum constant representing the transfer refused by server state.</summary>
        [EnumMember(Value = "TRANSFER_REFUSED_BY_SERVER")]
        [Description("TransferRefusedByServer")]
        TRANSFER_REFUSED_BY_SERVER = 43,

        /// <summary> An enum constant representing the transfer start error state.</summary>
        [EnumMember(Value = "TRANSFER_START_ERROR")]
        [Description("TransferStartError")]
        TRANSFER_START_ERROR = 44,

        /// <summary> An enum constant representing the transfer time out state.</summary>
        [EnumMember(Value = "TRANSFER_TIME_OUT")]
        [Description("TransferTimeOut")]
        TRANSFER_TIME_OUT = 45,

        /// <summary> An enum constant representing the transfer waiting for communication state.</summary>
        [EnumMember(Value = "TRANSFER_WAITING_FOR_COMMUNICATION")]
        [Description("TransferWaitingForCommunication")]
        TRANSFER_WAITING_FOR_COMMUNICATION = 46,

        /// <summary> An enum constant representing the transfer waiting for disk space on server state.</summary>
        [EnumMember(Value = "TRANSFER_WAITING_FOR_DISK_SPACE_ON_SERVER")]
        [Description("TransferWaitingForDiskSpaceOnServer")]
        TRANSFER_WAITING_FOR_DISK_SPACE_ON_SERVER = 47,

        /// <summary> An enum constant representing the transfer wainting for disk space on train state.</summary>
        [EnumMember(Value = "TRANSFER_WAITING_FOR_DISK_SPACE_ON_TRAIN")]
        [Description("TransferWaitingForDiskSpaceOnTrain")]
        TRANSFER_WAITING_FOR_DISK_SPACE_ON_TRAIN = 48,

        /// <summary> An enum constant representing the transfer waiting for link state.</summary>
        [EnumMember(Value = "TRANSFER_WAITING_FOR_LINK")]
        [Description("TransferWaitingForLink")]
        TRANSFER_WAITING_FOR_LINK = 49,

        /// <summary> An enum constant representing the transfert waiting for scheduling state.</summary>
        [EnumMember(Value = "TRANSFER_WAITING_FOR_SCHEDULING")]
        [Description("TransferWaitingForScheduling")]
        TRANSFER_WAITING_FOR_SCHEDULING = 50,


        // TODO : placer au bon endroit avec la bonne valeur
        /// <summary> An enum constant representing the out of other error state.</summary>
        [EnumMember(Value = "OTHER_ERROR")]
        [Description("OtherError")]
        OTHER_ERROR = 27,
    }

}