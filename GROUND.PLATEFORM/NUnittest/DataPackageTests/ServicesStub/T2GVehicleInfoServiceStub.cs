using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using DataPackageTests.T2GServiceInterface.VehicleInfo;

using FieldStruct = DataPackageTests.T2GServiceInterface.Notification.fieldStruct;
using NotificationClient = DataPackageTests.T2GServiceInterface.Notification.NotificationPortTypeClient;


namespace DataPackageTests.ServicesStub
{
    /// <summary>
    /// Base class for T2G Messages
    /// </summary>
    /// <seealso cref="DataPackageTests.T2GServiceInterface.Notification.onMessageNotificationInputBody" />
    abstract class MessageBase : DataPackageTests.T2GServiceInterface.Notification.onMessageNotificationInputBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase"/> class.
        /// </summary>
        /// <param name="systemId">The system identifier.</param>
        /// <param name="messageIdentifier">The message identifier.</param>
        /// <param name="fieldNames">The field names of the message.</param>
        protected MessageBase(string systemId, string messageIdentifier, string[] fieldNames)
        {
            this.systemId = systemId;
            this.timestamp = DateTime.UtcNow;
            this.inhibited = false;
            this.fieldList = new DataPackageTests.T2GServiceInterface.Notification.fieldList();
            fieldList.Capacity = fieldNames.Length;
            foreach (string name in fieldNames)
            {
                fieldList.Add(createField(name, string.Empty));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase"/> class by copying value of an existing one.
        /// </summary>
        /// <param name="other">The other message to copy.</param>
        protected MessageBase(MessageBase other)
        {
            systemId = other.systemId;
            this.timestamp = other.timestamp;
            this.inhibited = other.inhibited;
            this.fieldList = new DataPackageTests.T2GServiceInterface.Notification.fieldList();
            fieldList.Capacity = other.fieldList.Capacity;
            foreach (DataPackageTests.T2GServiceInterface.Notification.fieldStruct f in other.fieldList)
            {
                fieldList.Add(createField(f.id, f.value));
            }
        }

        /// <summary>
        /// Creates one field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The field created.</returns>
        private FieldStruct createField(string name, string value)
        {
            FieldStruct f = new FieldStruct();
            f.type = DataPackageTests.T2GServiceInterface.Notification.fieldTypeEnum.@string;
            f.id = name;
            f.value = value;

            return f;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        public abstract MessageBase Clone();
    }

    /// <summary>
    /// Class that represent the PIS.MISSION message in T2G
    /// </summary>
    /// <seealso cref="DataPackageTests.ServicesStub.MessageBase" />
    class MissionMessage : MessageBase
    {
        public static readonly string[] FieldsNameList = { "CommercialNumber", "OperatorCode", "State" };
        public const string MessageIdentifier = "PIS.MISSION";
        public const int CommercialNumberIndex = 0;
        public const int StateIndex = 1;
        public const int OperatorCodeIndex = 2;
        public string CommercialNumber
        {
            get
            {
                return fieldList[CommercialNumberIndex].value;
            }

            set
            {
                fieldList[CommercialNumberIndex].value = value;
            }
        }

        public string OperatorCode
        {
            get
            {
                return fieldList[OperatorCodeIndex].value;
            }

            set
            {
                fieldList[OperatorCodeIndex].value = value;
            }
        }

        public string State
        {
            get
            {
                return fieldList[StateIndex].value;
            }

            set
            {
                fieldList[StateIndex].value = value;
            }
        }

        public MissionMessage()
            : this(string.Empty)
        {
            // No logic body
        }

        public MissionMessage(string systemId)
            : base(systemId, MessageIdentifier, FieldsNameList)
        {
            // No logic body
        }

        public MissionMessage(string systemId, string commercialNumber, string state, string operatorCode)
            : this(systemId)
        {
            CommercialNumber = commercialNumber;
            State = state;
            OperatorCode = operatorCode;
        }

        public MissionMessage(MissionMessage other)
            : base(other)
        {
            // No logic body
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A copy of this instance.
        /// </returns>
        public override MessageBase Clone()
        {
            return new MissionMessage(this);
        }
    };

    /// <summary>
    /// Class that represent the PIS.VERSION message in T2G.
    /// </summary>
    /// <seealso cref="DataPackageTests.ServicesStub.MessageBase" />
    class VersionMessage : MessageBase
    {
        public static readonly string[] FieldsNameList = { "Version PIS Software" };
        public const string MessageIdentifier = "PIS.VERSION";
        public const int VersionIndex = 0;

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version
        {
            get
            {
                return fieldList[VersionIndex].value;
            }

            set
            {
                fieldList[VersionIndex].value = value;
            }
        }

        public VersionMessage()
            : this(string.Empty)
        {
            // No logic body
        }

        public VersionMessage(string systemId)
            : base(systemId, MessageIdentifier, FieldsNameList)
        {
            // No logic body
        }

        public VersionMessage(string systemId, string version)
            : this(systemId)
        {
            Version = version;
        }

        public VersionMessage(VersionMessage other)
            : base(other)
        {
            // No logic body
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A copy of this instance.
        /// </returns>
        public override MessageBase Clone()
        {
            return new VersionMessage(this);
        }
    }

    /// <summary>
    /// Class that represent the PIS.BASELINE message in T2G.
    /// </summary>
    /// <seealso cref="DataPackageTests.ServicesStub.MessageBase" />
    class BaselineMessage : MessageBase
    {
        public static readonly string[] FieldsNameList = { "Archived Valid Out", 
                                                             "Archived Version Out", 
                                                             "Archived Version PisBase Out", 
                                                             "Archived Version PisMission Out", 
                                                             "Archived Version PisInfotainment Out", 
                                                             "Archived Version Lmt Out", 
                                                             "Current Forced Out", 
                                                             "Current Valid Out", 
                                                             "Current Version Out", 
                                                             "Current Version PisBase Out", 
                                                             "Current Version PisMission Out", 
                                                             "Current Version PisInfotainment Out", 
                                                             "Current Version Lmt Out",
                                                             "Current ExpirationDate Out", 
                                                             "Future Valid Out", 
                                                             "Future Version Out", 
                                                             "Future Version PisBase Out", 
                                                             "Future Version PisMission Out", 
                                                             "Future Version PisInfotainment Out",
                                                             "Future Version Lmt Out", 
                                                             "Future ActivationDate Out", 
                                                             "Future ExpirationDate Out" };
        public const string MessageIdentifier = "PIS.BASELINE";
        public const int ArchivedValidOutIndex = 0;
        public const int ArchivedVersionOutIndex = 1;
        public const int ArchivedVersionPisBaseOutIndex = 2;
        public const int ArchivedVersionPisMissionOutIndex = 3;
        public const int ArchivedVersionPisInfotainmentOutIndex = 4;
        public const int ArchivedVersionLmtOutIndex = 5;

        public const int CurrentForcedOutIndex = 6;
        public const int CurrentValidOutIndex = 7;
        public const int CurrentVersionOutIndex = 8;
        public const int CurrentVersionPisBaseOutIndex = 9;
        public const int CurrentVersionPisMissionOutIndex = 10;
        public const int CurrentVersionPisInfotainmentOutIndex = 11;
        public const int CurrentVersionLmtOutIndex = 12;
        public const int CurrentExpirationDateOutIndex = 13;
        public const int FutureValidOutIndex = 14;
        public const int FutureVersionOutIndex = 15;
        public const int FutureVersionPisBaseOutIndex = 16;
        public const int FutureVersionPisMissionOutIndex = 17;
        public const int FutureVersionPisInfotainmentOutIndex = 18;
        public const int FutureVersionLmtOutIndex = 19;
        public const int FutureActivationDateOutIndex = 20;
        public const int FutureExpirationDateOutIndex = 21;

        /// <summary>
        /// Gets or sets the current version.
        /// </summary>
        public string CurrentVersion
        {
            get
            {
                return fieldList[CurrentVersionOutIndex].value;
            }

            set
            {
                fieldList[CurrentVersionOutIndex].value = value;
                fieldList[CurrentVersionPisBaseOutIndex].value = value;
                fieldList[CurrentVersionPisMissionOutIndex].value = value;
                fieldList[CurrentVersionPisInfotainmentOutIndex].value = value;
                fieldList[CurrentVersionLmtOutIndex].value = value;
                fieldList[CurrentValidOutIndex].value = string.IsNullOrEmpty(value) ? "false" : "true";
            }
        }

        /// <summary>
        /// Gets or sets the future version.
        /// </summary>
        public string FutureVersion
        {
            get
            {
                return fieldList[FutureVersionOutIndex].value;
            }

            set
            {
                fieldList[FutureVersionOutIndex].value = value;
                fieldList[FutureVersionPisBaseOutIndex].value = value;
                fieldList[FutureVersionPisMissionOutIndex].value = value;
                fieldList[FutureVersionPisInfotainmentOutIndex].value = value;
                fieldList[FutureVersionLmtOutIndex].value = value;
                fieldList[FutureValidOutIndex].value = string.IsNullOrEmpty(value) ? "false" : "true";
            }
        }

        /// <summary>
        /// Gets or sets the archived version.
        /// </summary>
        public string ArchivedVersion
        {
            get
            {
                return fieldList[ArchivedVersionOutIndex].value;
            }

            set
            {
                fieldList[ArchivedVersionOutIndex].value = value;
                fieldList[ArchivedVersionPisBaseOutIndex].value = value;
                fieldList[ArchivedVersionPisMissionOutIndex].value = value;
                fieldList[ArchivedVersionPisInfotainmentOutIndex].value = value;
                fieldList[FutureVersionLmtOutIndex].value = value;
                fieldList[ArchivedVersionLmtOutIndex].value = string.IsNullOrEmpty(value) ? "false" : "true";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaselineMessage"/> class.
        /// </summary>
        public BaselineMessage()
            : this(string.Empty)
        {
            // No logic body
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaselineMessage"/> class.
        /// </summary>
        /// <param name="systemId">The system identifier.</param>
        public BaselineMessage(string systemId)
            : base(systemId, MessageIdentifier, FieldsNameList)
        {
            // No logic body
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaselineMessage"/> class.
        /// </summary>
        /// <param name="other">The other object to copy.</param>
        public BaselineMessage(BaselineMessage other)
            : base(other)
        {
            // No logic body
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A copy of this instance.
        /// </returns>
        public override MessageBase Clone()
        {
            return new BaselineMessage(this);
        }
    }

    /// <summary>
    /// Simulate an implementation of service T2G Vehicle-Info.
    /// </summary>
    /// <seealso cref="DataPackageTests.T2GServiceInterface.VehicleInfo.VehicleInfoPortType" />
    /// <remarks>Only subscribe, unsubscribe to service and message notification are implemented</remarks>
    [ServiceBehaviorAttribute(InstanceContextMode = InstanceContextMode.Single, ConfigurationName = "DataPackageTests.T2GServiceInterface.VehicleInfo.VehicleInfoPortType")]
    class T2GVehicleInfoServiceStub : VehicleInfoPortType
    {
        #region Fields

        public const int BaselineMessageIndex = 0;
        public const int MissionMessageIndex = 1;
        public const int VersionMessageIndex = 2;

        private static readonly string[] SupportedMessages = { BaselineMessage.MessageIdentifier, MissionMessage.MessageIdentifier, VersionMessage.MessageIdentifier };

        private T2GIdentificationServiceStub _identificationService;

        private object _messageSubscriptionLock = new object();

        /// <summary>
        /// The message subscriptions. Key is notification url, Value is the subscription id. 
        /// The array is indexed by message identifier index defined by SupportedMessages variable.
        /// </summary>
        private Dictionary<string, int>[] _messageSubscriptions = {
                                                                       new Dictionary<string, int>(10, StringComparer.OrdinalIgnoreCase),
                                                                       new Dictionary<string, int>(10, StringComparer.OrdinalIgnoreCase),
                                                                       new Dictionary<string, int>(10, StringComparer.OrdinalIgnoreCase),
                                                                   };
        private int _nextMessageSubscriptionId = 1;

        private object _messageDataLock = new object();

        /// <summary>
        /// The messages data index by message index defined by SupportedMessages variable. Key is the train-id and Value is the message value.
        /// </summary>
        public Dictionary<string, MessageBase>[] _messagesData = {
                            new Dictionary<string, MessageBase>(10, StringComparer.OrdinalIgnoreCase),
                            new Dictionary<string, MessageBase>(10, StringComparer.OrdinalIgnoreCase),
                            new Dictionary<string, MessageBase>(10, StringComparer.OrdinalIgnoreCase)
                                                                 };

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T2GVehicleInfoServiceStub"/> class.
        /// </summary>
        /// <param name="identificationService">The identification service.</param>
        /// <exception cref="ArgumentNullException">identificationService parameter is null</exception>
        public T2GVehicleInfoServiceStub(T2GIdentificationServiceStub identificationService)
        {
            if (identificationService == null)
            {
                throw new ArgumentNullException("identificationService");
            }

            _identificationService = identificationService;

        }
        #endregion

        #region Update message and service data

        /// <summary>
        /// Updates the message data for the mission message.
        /// </summary>
        /// <param name="missionMessage">The mission message.</param>
        public void UpdateMessageData(MissionMessage missionMessage)
        {
            UpdateMessageData(missionMessage, MissionMessageIndex);
        }

        /// <summary>
        /// Updates the message data for the version message.
        /// </summary>
        /// <param name="missionMessage">The version message.</param>
        public void UpateMessageData(VersionMessage versionMessage)
        {
            UpdateMessageData(versionMessage, VersionMessageIndex);
        }

        /// <summary>
        /// Updates the message data for the baseline message.
        /// </summary>
        /// <param name="missionMessage">The baseline message.</param>
        public void UpdateMessageData(BaselineMessage baselineMessage)
        {
            UpdateMessageData(baselineMessage, BaselineMessageIndex);
        }

        public void UpdateMessageData(MessageBase messageData, int messageIndex)
        {
            MessageBase clonedData = messageData.Clone();
            clonedData.timestamp = DateTime.UtcNow;
            lock (_messageDataLock)
            {
                _messagesData[messageIndex][messageData.systemId] = clonedData;
            }
        }

        #endregion

        #region VehicleInfoPortType Members

        public subscribeToMessageNotificationsOutput subscribeToMessageNotifications(subscribeToMessageNotificationsInput request)
        {
            if (!_identificationService.isSessionValid(request.Body.sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            string notificationUrl = _identificationService.getNotificationUrl(request.Body.sessionId);

            if (string.IsNullOrEmpty(notificationUrl))
            {
                throw FaultExceptionFactory.CreateNoNotificationUrlFault();
            }

            if (request.Body.systemIdList.Count != 0)
            {
                throw FaultExceptionFactory.CreateOnlySubscriptionToAllSystemIsSupportedFault();
            }

            if (request.Body.messageSubscriptionList.Count != 1)
            {
                throw FaultExceptionFactory.CreateInvalidSubscriptionCountFault();
            }
            else if (request.Body.messageSubscriptionList[0].notificationMode != notificationModeEnum.onChanges)
            {
                throw FaultExceptionFactory.CreateOnlyOnChangeNotificationSupportedFault();
            }


            int messageIndex = Array.IndexOf(SupportedMessages, request.Body.messageSubscriptionList[0].messageId);
            if (messageIndex < 0)
            {
                throw FaultExceptionFactory.CreateInvalidMessageIdentifierFault();
            }

            int subscriptionId;
            lock (_messageSubscriptionLock)
            {
                if (!_messageSubscriptions[messageIndex].TryGetValue(notificationUrl, out subscriptionId))
                {
                    subscriptionId = _nextMessageSubscriptionId;
                    _messageSubscriptions[messageIndex][notificationUrl] = _nextMessageSubscriptionId++;
                }
            }

            Dictionary<string, MessageBase> messageData;
            lock (_messageDataLock)
            {
                messageData = new Dictionary<string, MessageBase>(_messagesData[messageIndex]);
            }

            // Notify the subscribers
            if (messageData.Count > 0)
            {
                EndpointAddress address = new EndpointAddress(notificationUrl);
                using (NotificationClient client = new NotificationClient("NotificationClient", address))
                {
                    try
                    {
                        client.Open();
                        foreach (MessageBase notification in messageData.Values)
                        {
                            client.onMessageNotification(notification.systemId, notification.messageId, notification.fieldList, notification.timestamp, notification.inhibited);
                        }
                    }
                    finally
                    {
                        if (client.State == CommunicationState.Faulted)
                        {
                            client.Abort();
                        }
                    }
                }
            }

            subscribeToMessageNotificationsOutput result = new subscribeToMessageNotificationsOutput();
            result.Body = new DataPackageTests.T2GServiceInterface.VehicleInfo.subscribeToMessageNotificationsOutputBody(subscriptionId);

            return result;
        }

        /// <summary>
        /// Implements the unsubscribeToMessageNotifications of T2G Vehicle-Info service.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        public void unsubscribeToMessageNotifications(int sessionId, int subscriptionId)
        {
            if (!_identificationService.isSessionValid(sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            lock (_messageSubscriptionLock)
            {
                string notificationUrl=null;

                foreach (Dictionary<string, int> dictionary in _messageSubscriptions)
                {
                    foreach (KeyValuePair<string,int> item in dictionary)
                    {
                        if (item.Value == subscriptionId)
                        {
                            notificationUrl = item.Key;
                            break;
                        }
                    }

                    if (notificationUrl != null)
                    {
                        dictionary.Remove(notificationUrl);
                        break;
                    }
                }
            }
        }

        public demandMessageNotificationOutput demandMessageNotification(demandMessageNotificationInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        public subscribeToServiceNotificationsOutput subscribeToServiceNotifications(subscribeToServiceNotificationsInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        public void unsubscribeToServiceNotifications(int sessionId, int subscriptionId)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        public getActiveServicesOutput getActiveServices(getActiveServicesInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        public getServiceInfoOutput getServiceInfo(getServiceInfoInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        public inhibitMessagesOutput inhibitMessages(inhibitMessagesInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        public enumInhibitedMessagesOutput enumInhibitedMessages(enumInhibitedMessagesInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        public setNotificationPeriodOutput setNotificationPeriod(setNotificationPeriodInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        public getNotificationPeriodsOutput getNotificationPeriods(getNotificationPeriodsInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        #endregion

    }
}
