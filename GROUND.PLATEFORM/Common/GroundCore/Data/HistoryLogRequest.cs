using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PIS.Ground.Core.Data
{
    /// <summary>
    /// Class representing History Log Request
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "HistoryLogRequest")]
    public class HistoryLogRequest
    {
        #region DataMember
        /// <summary>
        /// Request Id of the corresponding request
        /// </summary>
        [DataMember]
        public Guid RequestID;

        /// <summary>
        /// Contains all scheduled message parameters. 
        /// </summary>
        [DataMember]
        public MessageContextData MessageContext;

        /// <summary>
        /// Message Command Type
        /// </summary>
        [DataMember]
        public CommandType MessageCommandType;

        /// <summary>
        /// List of Trains to which message was sent.
        /// </summary>
        [DataMember]
        public List<int> TrainIDs;

        #endregion
    }

    /// <summary>
    /// Class for Message Context 
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "MessageContextData")]
    public class MessageContextData
    {
        #region DataMember
        /// <summary>
        /// Template Id of the message
        /// </summary>
        [DataMember]
        public int TemplateId;

        /// <summary>
        /// Message text
        /// </summary>
        [DataMember]
        public string Text;

        /// <summary>
        /// Activation start date/time 
        /// </summary>
        [DataMember]
        public DateTime ActivationStartDateTime;

        /// <summary>
        /// Activation end date/time 
        /// </summary>
        [DataMember]
        public DateTime ActivationEndDateTime;
        #endregion
    }

    /// <summary>
    /// class for Message status
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "MessageStatus")]
    public class MessageStatus
    {
        #region DataMember
        /// <summary>
        /// Unique id
        /// </summary>
        [DataMember]
        public int Id;

        /// <summary>
        /// Train id
        /// </summary>
        [DataMember]
        public int TrainId;

        /// <summary>
        /// Message status
        /// </summary>
        [DataMember]
        public MessageStatusType Status;

        /// <summary>
        /// Status updated DateTime 
        /// </summary>
        [DataMember]
        public DateTime UpdatedDateTime;

        #endregion
    }

    /// <summary>
    /// class for history log data
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "HistoryLogData")]
    public class HistoryLogData : HistoryLogRequest
    {
        #region DataMember
        /// <summary>
        /// Message sent datetime
        /// </summary>
        [DataMember]
        public DateTime MessageSentDateTime;

        /// <summary>
        /// Message status
        /// </summary>
        [DataMember]
        public List<MessageStatus> MessageStatus;

        #endregion
    }
}
