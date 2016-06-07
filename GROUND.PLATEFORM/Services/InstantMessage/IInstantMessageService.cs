//---------------------------------------------------------------------------------------------------
// <copyright file="IInstantMessageService.cs" company="Alstom">
//          (c) Copyright ALSTOM 2015.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using PIS.Ground.Core.Data;
using PIS.Ground.InstantMessage;

namespace PIS.Ground.InstantMessage
{
    [ServiceContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "InstantMessage")]
    public interface IInstantMessageService
    {       
        /// <summary>Gets available element list.</summary>
        /// <param name="sessionId">Identifier for the session.</param>
        /// <returns>The available element list.</returns>
        [OperationContract]
        InstantMessageElementListResult GetAvailableElementList(Guid sessionId);

        /// <summary>Retrieves message template list.</summary>
        /// <param name="sessionId">Identifier for the session.</param>
        /// <param name="targetAddress">Target address.</param>
        /// <returns>A list of.</returns>
        [OperationContract]
        InstantMessageTemplateListResult RetrieveMessageTemplateList(Guid sessionId, TargetAddressType targetAddress);

        /// <summary>Retrieves station list.</summary>
        /// <param name="sessionId">Identifier for the session.</param>
        /// <param name="targetAddress">Target address.</param>
        /// <returns>A list of.</returns>
        [OperationContract]
        InstantMessageStationListResult RetrieveStationList(Guid sessionId, TargetAddressType targetAddress);

        /// <summary>Sends a predefined messages.</summary>
        /// <param name="sessionId">Identifier for the session.</param>
        /// <param name="targetAddress">Target address.</param>
        /// <param name="requestTimeout">The request timeout.</param>
        /// <param name="Messages">The messages.</param>
        /// <returns>.</returns>
        [OperationContract]
        InstantMessageResult SendPredefinedMessages(Guid sessionId, TargetAddressType targetAddress, uint? requestTimeout, PredefinedMessageType[] Messages);

        /// <summary>Sends a free text message.</summary>
        /// <param name="sessionId">Identifier for the session.</param>
        /// <param name="targetAddress">Target address.</param>
        /// <param name="requestTimeout">The request timeout.</param>
        /// <param name="Message">The message.</param>
        /// <returns>.</returns>
        [OperationContract]
        InstantMessageResult SendFreeTextMessage(Guid sessionId, TargetAddressType targetAddress, uint? requestTimeout, FreeTextMessageType Message);

        /// <summary>Sends a scheduled message.</summary>
        /// <param name="sessionId">Identifier for the session.</param>
        /// <param name="targetAddress">Target address.</param>
        /// <param name="requestTimeout">The request timeout.</param>
        /// <param name="Message">The message.</param>
        /// <returns>.</returns>
        [OperationContract]
        InstantMessageResult SendScheduledMessage(Guid sessionId, TargetAddressType targetAddress, uint? requestTimeout, ScheduledMessageType Message);

        /// <summary>Cancel all messages.</summary>
        /// <param name="sessionId">Identifier for the session.</param>
        /// <param name="targetAddress">Target address.</param>
        /// <param name="requestTimeout">The request timeout.</param>
        /// <returns>.</returns>
        [OperationContract]
        InstantMessageResult CancelAllMessages(Guid sessionId, TargetAddressType targetAddress, uint? requestTimeout);

        /// <summary>Cancel scheduled message.</summary>
        /// <param name="sessionId">Identifier for the session.</param>
        /// <param name="requestId">Identifier for the request.</param>
        /// <param name="targetAddress">Target address.</param>
        /// <param name="requestTimeout">The request timeout.</param>
        /// <returns>.</returns>
        [OperationContract]
        InstantMessageResult CancelScheduledMessage(Guid sessionId, Guid requestId, TargetAddressType targetAddress, uint? requestTimeout);
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "InstantMessageResult")]
    public class InstantMessageResult
    {
        /// <summary>The result code.</summary>
        [DataMember(IsRequired = true)]
        public InstantMessageErrorEnum ResultCode;

        /// <summary>Identifier for the request.</summary>
        [DataMember(IsRequired = true)]
        public Guid RequestId;        
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "InstantMessageTemplateListResult")]
    public class InstantMessageTemplateListResult
    {
        /// <summary>The result code.</summary>
        [DataMember(IsRequired = true)]
        public InstantMessageErrorEnum ResultCode;

        /// <summary>List of message templates.</summary>
        [DataMember(IsRequired = true)]
        public MessageTemplateListType MessageTemplateList;        
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "InstantMessageStationListResult")]
    public class InstantMessageStationListResult
    {
        /// <summary>The result code.</summary>
        [DataMember(IsRequired = true)]
        public InstantMessageErrorEnum ResultCode;

        /// <summary>List of instant message stations.</summary>
        [DataMember(IsRequired = true)]
        public List<StationType> InstantMessageStationList;        
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "InstantMessageElementListResult")]
    public class InstantMessageElementListResult
    {
        /// <summary>The result code.</summary>
        [DataMember(IsRequired = true)]
        public InstantMessageErrorEnum ResultCode;

        /// <summary>List of elements.</summary>
        [DataMember(IsRequired = true)]
        public ElementList<AvailableElementData> ElementList;
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "InstantMessageErrorEnum")]
    public enum InstantMessageErrorEnum 
    {
        /// <summary>An enum constant representing the request accepted option.</summary>
        [EnumMember(Value = "REQUEST_ACCEPTED")]
        RequestAccepted,

        /// <summary>An enum constant representing the invalid session identifier option.</summary>
        [EnumMember(Value = "INVALID_SESSION_ID")]
        InvalidSessionId,

        /// <summary>An enum constant representing the invalid request timeout option.</summary>
        [EnumMember(Value = "INVALID_REQUEST_TIMEOUT")]
        InvalidRequestTimeout,

        /// <summary>An enum constant representing the unknown element identifier option.</summary>
        //[EnumMember(Value = "UNKNOWN_REQUEST_ID")]
        //UnknownRequestId,

        [EnumMember(Value = "UNKNOWN_ELEMENT_ID")]
        UnknownElementId,

        /// <summary>An enum constant representing the unknown mission identifier option.</summary>
        [EnumMember(Value = "UNKNOWN_MISSION_ID")]
        UnknownMissionId,
        
        /// <summary>An enum constant representing the template list not available option.</summary>
        [EnumMember(Value = "MESSAGE_TEMPLATE_LIST_NOT_AVAILABLE")]
        TemplateListNotAvailable,

        /// <summary>An enum constant representing the station list not available option.</summary>
        [EnumMember(Value = "STATION_LIST_NOT_AVAILABLE")]
        StationListNotAvailable,

        /// <summary>An enum constant representing the element list not available option.</summary>
        [EnumMember(Value = "ELEMENT_LIST_NOT_AVAILABLE")]
        ElementListNotAvailable,        
        
        /// <summary>An enum constant representing the datastore not accessible option.</summary>
        [EnumMember(Value = "PIS_DATASTORE_NOT_ACCESSIBLE")]
        DatastoreNotAccessible, 
       
        /// <summary>An enum constant representing the 2 g server offline option.</summary>
        [EnumMember(Value = "T2G_SERVER_OFFLINE")]
        T2GServerOffline,

        /// <summary>An enum constant representing the template file not found option.</summary>
        [EnumMember(Value = "TEMPLATE_FILE_NOT_FOUND")]
        TemplateFileNotFound,

        /// <summary>An enum constant representing the template file not valid option.</summary>
        [EnumMember(Value = "TEMPLATE_FILE_NOT_VALID")]
        TemplateFileNotValid,

        /// <summary>An enum constant representing the SQL error option.</summary>
        [EnumMember(Value = "SQL_ERROR")]
        SqlError,

        /// <summary>An enum constant representing the invalid command type option.</summary>
        [EnumMember(Value = "INVALID_COMMAND_TYPE")]
        InvalidCommandType,

        /// <summary>An enum constant representing the invalid end date option.</summary>
        [EnumMember(Value = "INVALID_END_DATE")]
        InvalidEndDate,

        /// <summary>An enum constant representing the invalid start date option.</summary>
        [EnumMember(Value = "INVALID_START_DATE")]
        InvalidStartDate,

        /// <summary>An enum constant representing the invalid context option.</summary>
        [EnumMember(Value = "INVALID_CONTEXT")]
        InvalidContext,

        /// <summary>An enum constant representing the invalid request identifier option.</summary>
        [EnumMember(Value = "INVALID_REQUEST_ID")]
        InvalidRequestID,

        /// <summary>An enum constant representing the invalid train identifier option.</summary>
        [EnumMember(Value = "INVALID_TRAIN_ID")]
        InvalidTrainID,

        /// <summary>An enum constant representing the invalid status option.</summary>
        [EnumMember(Value = "INVALID_STATUS")]
        InvalidStatus,

        /// <summary>An enum constant representing the internal error option.</summary>
        [EnumMember(Value = "INTERNAL_ERROR")]
        InternalError
    }    

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "StationName")]
    public class StationNameType
    {
        /// <summary>The language.</summary>
        [DataMember(IsRequired = true)]
        public string Language;

        /// <summary>The name.</summary>
        [DataMember(IsRequired = true)]
        public string Name;
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "Station")]
    public class StationType
    {
        /// <summary>The operator code.</summary>
        [DataMember(IsRequired = true)]
        public string OperatorCode;

        /// <summary>List of names.</summary>
        [DataMember(IsRequired = true)]
        public List<StationNameType> NameList;
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "Description")]
    public class TemplateDescriptionType
    {
        /// <summary>The language.</summary>
        [DataMember(IsRequired = true)]
        public string Language;

        /// <summary>The text.</summary>
        [DataMember(IsRequired = true)]
        public string Text;  
    }    

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "ScheduledPeriod")]
    public class ScheduledPeriodType
    {
        /// <summary>The start date time.</summary>
        [DataMember(IsRequired = true)]
        public DateTime StartDateTime;
        
        /// <summary>The end date time.</summary>
        [DataMember(IsRequired = true)]
        public DateTime EndDateTime;
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "FreeTextMessageTemplate")]
    public class FreeTextMessageTemplateType
    {
        /// <summary>The identifier.</summary>
        [DataMember(IsRequired = true)]
        public string Identifier;
       
        /// <summary>The category.</summary>
        [DataMember(IsRequired = true)]
        public string Category;
       
        /// <summary>List of descriptions.</summary>
        [DataMember(IsRequired = true)]
        public List<TemplateDescriptionType> DescriptionList;
       
        /// <summary>The class.</summary>
        [DataMember(IsRequired = true)]
        public string Class;        
    }    

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "ScheduledMessageTemplate")]
    public class ScheduledMessageTemplateType
    {
        /// <summary>The identifier.</summary>
        [DataMember(IsRequired = true)]
        public string Identifier;
        
        /// <summary>The category.</summary>
        [DataMember(IsRequired = true)]
        public string Category;
       
        /// <summary>List of descriptions.</summary>
        [DataMember(IsRequired = true)]
        public List<TemplateDescriptionType> DescriptionList;
       
        /// <summary>The class.</summary>
        [DataMember(IsRequired = true)]
        public string Class;        
    }   

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "Parameter")]
    public enum PredefinedMessageTemplateParameterType
    {
        /// <summary>An enum constant representing the class option.</summary>
        [EnumMember]
        Class,
       
        /// <summary>An enum constant representing the station identifier option.</summary>
        [EnumMember]
        StationId,
      
        /// <summary>An enum constant representing the car identifier option.</summary>
        [EnumMember]
        CarId,
      
        /// <summary>An enum constant representing the delay option.</summary>
        [EnumMember]
        Delay,
       
        /// <summary>An enum constant representing the delay reason option.</summary>
        [EnumMember]
        DelayReason,
       
        /// <summary>An enum constant representing the hour option.</summary>
        [EnumMember]
        Hour
    }  

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "PredefinedMessageTemplate")]
    public class PredefinedMessageTemplateType
    {
        /// <summary>The identifier.</summary>
        [DataMember(IsRequired = true)]
        public string Identifier;
       
        /// <summary>The category.</summary>
        [DataMember(IsRequired = true)]
        public string Category;
       
        /// <summary>List of descriptions.</summary>
        [DataMember(IsRequired = true)]
        public List<TemplateDescriptionType> DescriptionList;
       
        /// <summary>The class.</summary>
        [DataMember(IsRequired = true)]
        public string Class;
      
        /// <summary>List of parameters.</summary>
        [DataMember(IsRequired = true)]
        public List<PredefinedMessageTemplateParameterType> ParameterList;
    }    

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "MessageTemplateList")]
    public class MessageTemplateListType
    {
        /// <summary>List of predefined message templates.</summary>
        [DataMember(IsRequired = true)]
        public List<PredefinedMessageTemplateType> PredefinedMessageTemplateList;
        
        /// <summary>List of free text message templates.</summary>
        [DataMember(IsRequired = true)]
        public List<FreeTextMessageTemplateType> FreeTextMessageTemplateList;
       
        /// <summary>List of scheduled message templates.</summary>
        [DataMember(IsRequired = true)]
        public List<ScheduledMessageTemplateType> ScheduledMessageTemplateList;
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "FreeTextMessageType")]
    public class FreeTextMessageType
    {
        /// <summary>The identifier.</summary>
        [DataMember(IsRequired = true)]
        public string Identifier;
       
        /// <summary>Number of repetitions.</summary>
        [DataMember(IsRequired = true)]
        public uint NumberOfRepetitions;
       
        /// <summary>The delay between repetitions.</summary>
        [DataMember(IsRequired = true)]
        public uint DelayBetweenRepetitions;
       
        /// <summary>Duration of the display.</summary>
        [DataMember(IsRequired = true)]
        public uint DisplayDuration;
       
        /// <summary>true to attention getter.</summary>
        [DataMember(IsRequired = true)]
        public bool AttentionGetter;
        
        /// <summary>The free text.</summary>
        [DataMember(IsRequired = true)]
        public string FreeText;
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "PredefinedMessageType")]
    public class PredefinedMessageType
    {
        /// <summary>The identifier.</summary>
        [DataMember(IsRequired = true)]
        public string Identifier;
       
        /// <summary>Identifier for the station.</summary>
        [DataMember(IsRequired = false)]
        public string StationId;
        
        /// <summary>Identifier for the car.</summary>
        [DataMember(IsRequired = false)]
        public uint? CarId;
        
        /// <summary>The delay.</summary>
        [DataMember(IsRequired = false)]
        public uint? Delay;
       
        /// <summary>The delay reason.</summary>
        [DataMember(IsRequired = false)]
        public string DelayReason;
        
        /// <summary>The hour.</summary>
        [DataMember(IsRequired = false)]
        public DateTime? Hour;
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "ScheduledMessageType")]
    public class ScheduledMessageType
    {
        /// <summary>The identifier.</summary>
        [DataMember(IsRequired = true)]
        public string Identifier;
        
        /// <summary>The period.</summary>
        [DataMember(IsRequired = true)]
        public ScheduledPeriodType Period;
        
        /// <summary>The free text.</summary>
        [DataMember(IsRequired = true)]
        public string FreeText;
    }
    /*
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "PredefinedMessageListType")]
    public class PredefinedMessageListType
    {
        [DataMember]
        public List<PredefinedMessageType> List;
    }    

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", Name = "MessageListType")]
    public class MessageListType
    {
        [DataMember]
        public PredefinedMessageListType PredefinedMessageList;
        [DataMember]
        public FreeTextMessageType FreeTextMessage;
        [DataMember]
        public ScheduledMessageType ScheduleMessage;
    } */
}