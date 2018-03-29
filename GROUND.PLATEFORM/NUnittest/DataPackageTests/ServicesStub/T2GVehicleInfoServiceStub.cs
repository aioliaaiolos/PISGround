using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using DataPackageTests.T2GServiceInterface.VehicleInfo;
using PIS.Ground.Core.Common;

using FieldStruct = DataPackageTests.T2GServiceInterface.Notification.fieldStruct;
using NotificationClient = DataPackageTests.T2GServiceInterface.Notification.NotificationPortTypeClient;
using System.Globalization;


namespace DataPackageTests.ServicesStub
{

    #region T2G Message definition

    /// <summary>
    /// Base class for T2G Messages
    /// </summary>
    /// <seealso cref="DataPackageTests.T2GServiceInterface.Notification.onMessageNotificationInputBody" />
    [System.Runtime.Serialization.DataContractAttribute(Name = "onMessageNotificationInputBody", Namespace = "http://alstom.com/T2G/Notification")]
    public abstract class MessageBase : DataPackageTests.T2GServiceInterface.Notification.onMessageNotificationInputBody
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
            this.messageId = messageIdentifier;
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
            this.systemId = other.systemId;
            this.messageId = other.messageId;
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
    [System.Runtime.Serialization.DataContractAttribute(Name = "onMessageNotificationInputBody", Namespace = "http://alstom.com/T2G/Notification")]
    public class MissionMessage : MessageBase
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
    [System.Runtime.Serialization.DataContractAttribute(Name = "onMessageNotificationInputBody", Namespace = "http://alstom.com/T2G/Notification")]
    public class VersionMessage : MessageBase
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
    [System.Runtime.Serialization.DataContractAttribute(Name = "onMessageNotificationInputBody", Namespace = "http://alstom.com/T2G/Notification")]
    public class BaselineMessage : MessageBase
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
                if (string.IsNullOrEmpty(fieldList[CurrentForcedOutIndex].value))
                {
                    fieldList[CurrentForcedOutIndex].value = "false";
                }
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
                fieldList[ArchivedVersionLmtOutIndex].value = value;
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

    #endregion

    #region T2G Service data definition

    /// <summary>
    /// Describe the service data
    /// </summary>
    /// <seealso cref="DataPackageTests.T2GServiceInterface.Notification.serviceStruct" />
    [System.Runtime.Serialization.DataContractAttribute(Name = "serviceStruct", Namespace = "http://alstom.com/T2G/Notification")]
    public class ServiceInfoData : DataPackageTests.T2GServiceInterface.Notification.serviceStruct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInfoData"/> class.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="isAvailable">if set to <c>true</c> [is available].</param>
        /// <param name="IPAddress">The ip address.</param>
        /// <param name="port">The port.</param>
        /// <param name="vehicleId">The vehicle identifier.</param>
        /// <param name="carId">The car identifier.</param>
        public ServiceInfoData(ushort serviceId, string serviceName, bool isAvailable, string IPAddress, ushort port, ushort vehicleId, ushort carId)
        {
            this.vehiclePhysicalId = vehicleId;
            this.carId = carId;
            this.carIdStr = carId.ToString();
            this.deviceId = 200;
            this.deviceName = string.Empty;
            this.serviceId = serviceId;
            this.name = serviceName;
            this.operatorId = 200;
            this.AID = "MEDIA";
            this.SID = string.Format(CultureInfo.InvariantCulture, "TRAIN={0}.CAR-{1}.VMC-1", vehicleId, carId);
            this.serviceIPAddress = IPAddress;
            this.servicePortNumber = port;
            this.isAvailable = isAvailable;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance</returns>
        public ServiceInfoData Clone()
        {
            return (ServiceInfoData)this.MemberwiseClone();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as ServiceInfoData);
        }

        /// <summary>
        /// Verify if this object is Equals to the specified other object instance.
        /// </summary>
        /// <param name="other">The other to compare with.</param>
        ///   <c>true</c> if the specified <see cref="ServiceInfoData" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ServiceInfoData other)
        {
            return (other != null &&
                other.vehiclePhysicalId == vehiclePhysicalId &&
                other.carId == carId &&
                other.carIdStr == carIdStr &&
                other.deviceId == deviceId &&
                other.deviceName == deviceName &&
                other.serviceId == serviceId &&
                other.name == name &&
                other.operatorId == operatorId &&
                other.AID == AID &&
                other.SID == SID &&
                other.serviceIPAddress == serviceIPAddress &&
                other.servicePortNumber == servicePortNumber &&
                other.isAvailable == isAvailable);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.vehiclePhysicalId;
        }
    }

    #endregion


    /// <summary>
    /// Simulate an implementation of service T2G Vehicle-Info.
    /// </summary>
    /// <seealso cref="DataPackageTests.T2GServiceInterface.VehicleInfo.VehicleInfoPortType" />
    /// <remarks>Only subscribe, unsubscribe to service and message notification are implemented.
    /// Support subscription to all systems only.
    /// Only one subscription per message or service is supported.</remarks>
    [ServiceBehaviorAttribute(InstanceContextMode = InstanceContextMode.Single, ConfigurationName = "DataPackageTests.T2GServiceInterface.VehicleInfo.VehicleInfoPortType")]
    public class T2GVehicleInfoServiceStub : VehicleInfoPortType, IDisposable
    {
        #region Fields

        public const int BaselineMessageIndex = 0;
        public const int MissionMessageIndex = 1;
        public const int VersionMessageIndex = 2;

        private static readonly string[] SupportedMessages = { BaselineMessage.MessageIdentifier, MissionMessage.MessageIdentifier, VersionMessage.MessageIdentifier };
        private static readonly ushort[] SupportedServices = { (ushort)eServiceID.eSrvSIF_DataPackageServer,
                                                            (ushort)eServiceID.eSrvSIF_InstantMessageServer,
                                                            (ushort)eServiceID.eSrvSIF_ReportExchangeServer,
                                                            (ushort)eServiceID.eSrvSIF_MaintenanceServer,
                                                            (ushort)eServiceID.eSrvSIF_MissionServer,
		                                                    (ushort)eServiceID.eSrvSIF_LiveVideoControlServer,
                                                            (ushort)eServiceID.eSrvSIF_RealTimeServer};

        private T2GIdentificationServiceStub _identificationService;

        private object _messageSubscriptionLock = new object();

        /// <summary>
        /// An indicator that indicates if a subscription exist for every message defined in SupportedMessages variables.
        /// </summary>
        private bool[] _messageSubscriptions = { false, false, false };
        private string _messageNotificationUrl = string.Empty;

        private object _messageDataLock = new object();

        /// <summary>
        /// The messages data indexed by message index defined by SupportedMessages variable. Key is the train-id and Value is the message value.
        /// </summary>
        public Dictionary<string, MessageBase>[] _messagesData = {
                            new Dictionary<string, MessageBase>(10, StringComparer.OrdinalIgnoreCase),
                            new Dictionary<string, MessageBase>(10, StringComparer.OrdinalIgnoreCase),
                            new Dictionary<string, MessageBase>(10, StringComparer.OrdinalIgnoreCase)
                                                                 };

        private object _serviceSubscriptionLock = new object();

        /// <summary>
        /// An indicator that indicates if a subscription exist for every service defined in SupportedServices variables.
        /// </summary>
        private bool[] _serviceSubscriptions = { false, false, false, false, false, false, false };
        private string _serviceNotificationUrl = string.Empty;

        public object _serviceDataLock = new object();

        /// <summary>
        /// The services data indexed by service index defined by SupportedServices variable. ey is the train-id and Value is the service data.
        /// </summary>
        public Dictionary<string, ServiceInfoData>[] _serviceData = {
                            new Dictionary<string, ServiceInfoData>(10, StringComparer.OrdinalIgnoreCase),
                            new Dictionary<string, ServiceInfoData>(10, StringComparer.OrdinalIgnoreCase),
                            new Dictionary<string, ServiceInfoData>(10, StringComparer.OrdinalIgnoreCase),
                            new Dictionary<string, ServiceInfoData>(10, StringComparer.OrdinalIgnoreCase),
                            new Dictionary<string, ServiceInfoData>(10, StringComparer.OrdinalIgnoreCase),
                            new Dictionary<string, ServiceInfoData>(10, StringComparer.OrdinalIgnoreCase),
                            new Dictionary<string, ServiceInfoData>(10, StringComparer.OrdinalIgnoreCase)
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
            _identificationService.OnlineStatusChanged += new T2GIdentificationServiceStub.OnlineStatusChangedHandler(OnlineStatusChangedLogic);
            _identificationService.SystemDeleted += new T2GIdentificationServiceStub.SystemDeletedHandler(SystemDeletedLogic);
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
        public void UpdateMessageData(VersionMessage versionMessage)
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

            string notificationUrl;
            bool subscribed;
            lock (_messageSubscriptionLock)
            {
                subscribed = _messageSubscriptions[messageIndex];
                notificationUrl = _messageNotificationUrl;
            }

            if (subscribed)
            {
                EndpointAddress address = new EndpointAddress(notificationUrl);
                using (NotificationClient client = new NotificationClient("NotificationClient", address))
                {
                    try
                    {
                        client.Open();
                        client.onMessageNotification(clonedData.systemId, clonedData.messageId, clonedData.fieldList, clonedData.timestamp, clonedData.inhibited);
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
        }

        /// <summary>
        /// Updates the service data.
        /// </summary>
        /// <param name="systemId">The system identifier.</param>
        /// <param name="serviceData">The service data.</param>
        public void UpdateServiceData(string systemId, ServiceInfoData serviceData)
        {
            ServiceInfoData clonedService = serviceData.Clone();
            int serviceIndex = Array.IndexOf(SupportedServices, serviceData.serviceId);
            if (serviceIndex >= 0)
            {
                lock (_serviceDataLock)
                {
                    ServiceInfoData existingService;
                    if (_serviceData[serviceIndex].TryGetValue(systemId, out existingService))
                    {
                        if (existingService.Equals(serviceData))
                        {
                            clonedService = null;
                        }
                    }

                    if (clonedService != null)
                    {
                        _serviceData[serviceIndex][systemId] = clonedService;
                    }
                }

                string notificationUrl;
                bool subscribed;
                lock (_serviceSubscriptionLock)
                {
                    subscribed = _serviceSubscriptions[serviceIndex] == true;
                    notificationUrl = _serviceNotificationUrl;
                }

                if (subscribed)
                {
                    bool isOnline = _identificationService.IsSystemOnline(systemId);
                    DataPackageTests.T2GServiceInterface.Notification.serviceList serviceList = new DataPackageTests.T2GServiceInterface.Notification.serviceList();
                    serviceList.Capacity = 1;

                    if (isOnline || clonedService.isAvailable == false)
                    {
                        serviceList.Add(clonedService);
                    }
                    else
                    {
                        serviceList.Add(clonedService.Clone());
                        serviceList[0].isAvailable = false;
                    }

                    EndpointAddress address = new EndpointAddress(notificationUrl);
                    using (NotificationClient client = new NotificationClient("NotificationClient", address))
                    {
                        try
                        {
                            client.Open();
                            client.onServiceNotification(systemId, isOnline , serviceIndex + 1, serviceList);
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
            }
        }

        #endregion

        #region VehicleInfoPortType Members

        public subscribeToMessageNotificationsOutput subscribeToMessageNotifications(subscribeToMessageNotificationsInput request)
        {
            if (!_identificationService.IsSessionValid(request.Body.sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            string notificationUrl = _identificationService.GetNotificationUrl(request.Body.sessionId);

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

            string[] messageId_FR = new string[3];
            messageId_FR[0] = "PIS.BASELINE";
            messageId_FR[0] = "PIS.MISSION";
            messageId_FR[0] = "PIS.VERSION";
            int messageIndex = Array.IndexOf(SupportedMessages, messageId_FR[0]);
            if (messageIndex < 0)
            {
                throw FaultExceptionFactory.CreateInvalidMessageIdentifierFault();
            }

            int subscriptionId;
            lock (_messageSubscriptionLock)
            {
                _messageSubscriptions[messageIndex] = true;
                _messageNotificationUrl = notificationUrl;
                subscriptionId = messageIndex+1;
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
            if (!_identificationService.IsSessionValid(sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            lock (_messageSubscriptionLock)
            {
                if (subscriptionId > 0 && subscriptionId <= SupportedMessages.Length)
                {
                    _messageSubscriptions[subscriptionId - 1] = false;
                }
            }
        }

        public demandMessageNotificationOutput demandMessageNotification(demandMessageNotificationInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        public subscribeToServiceNotificationsOutput subscribeToServiceNotifications(subscribeToServiceNotificationsInput request)
        {
            if (!_identificationService.IsSessionValid(request.Body.sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            string notificationUrl = _identificationService.GetNotificationUrl(request.Body.sessionId);

            if (string.IsNullOrEmpty(notificationUrl))
            {
                throw FaultExceptionFactory.CreateNoNotificationUrlFault();
            }

            if (request.Body.systemIdList.Count != 0)
            {
                throw FaultExceptionFactory.CreateOnlySubscriptionToAllSystemIsSupportedFault();
            }

            int serviceIndex = Array.IndexOf(SupportedServices, (ushort)request.Body.serviceId);
            if (serviceIndex < 0)
            {
                throw FaultExceptionFactory.CreateInvalidServiceIdentifierFault();
            }

            lock (_serviceSubscriptionLock)
            {
                _serviceSubscriptions[serviceIndex] = true;
                _serviceNotificationUrl = notificationUrl;
            }

            Dictionary<string, ServiceInfoData> serviceData;
            lock (_serviceDataLock)
            {
                serviceData = new Dictionary<string, ServiceInfoData>(_serviceData[serviceIndex]);
            }

            // Notify the subscribers
            if (serviceData.Count > 0)
            {
                DataPackageTests.T2GServiceInterface.Notification.serviceList serviceList = new DataPackageTests.T2GServiceInterface.Notification.serviceList();
                serviceList.Capacity = 1;
                EndpointAddress address = new EndpointAddress(notificationUrl);
                using (NotificationClient client = new NotificationClient("NotificationClient", address))
                {
                    try
                    {
                        client.Open();
                        foreach (KeyValuePair<string, ServiceInfoData> notification in serviceData)
                        {
                            if (serviceList.Count == 0)
                            {
                                serviceList.Add(notification.Value);
                            }
                            else
                            {
                                serviceList[0] = notification.Value;
                            }
                            client.onServiceNotification(notification.Key, _identificationService.IsSystemOnline(notification.Key) , serviceIndex+1, serviceList);
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

            subscribeToServiceNotificationsOutputBody body = new subscribeToServiceNotificationsOutputBody(serviceIndex + 1);
            subscribeToServiceNotificationsOutput result = new subscribeToServiceNotificationsOutput(body);
            return result;
        }

        /// <summary>
        /// Implements the unsubscribeToServiceNotifications method of T2G Vehicle-Info service.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="subscriptionId">The subscription identifier to unsubscribe.</param>
        public void unsubscribeToServiceNotifications(int sessionId, int subscriptionId)
        {
            if (!_identificationService.IsSessionValid(sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            lock (_serviceSubscriptionLock)
            {
                if (subscriptionId > 0 && subscriptionId <= SupportedServices.Length)
                {
                    _serviceSubscriptions[subscriptionId - 1] = false;
                }
            }
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


        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_identificationService != null)
                {
                    _identificationService.OnlineStatusChanged -= new T2GIdentificationServiceStub.OnlineStatusChangedHandler(OnlineStatusChangedLogic);
                    _identificationService.SystemDeleted -= new T2GIdentificationServiceStub.SystemDeletedHandler(SystemDeletedLogic);
                    _identificationService = null;
                }
            }
        }

        #endregion

        #region Identification event handlers

        /// <summary>
        /// Manage the system deleted logic.
        /// </summary>
        /// <param name="systemId">The system identifier deleted.</param>
        private void SystemDeletedLogic(string systemId)
        {
            lock (_messageDataLock)
            {
                foreach (var data in _messagesData)
                {
                    data.Remove(systemId);
                }
            }

            lock (_serviceDataLock)
            {
                foreach (var data in _serviceData)
                {
                    data.Remove(systemId);
                }
            }
        }

        /// <summary>
        /// Called when online status on a train changed.
        /// </summary>
        /// <param name="systemId">The system identifier.</param>
        /// <param name="isOnline">true if system became online, false if it became offline.</param>
        private void OnlineStatusChangedLogic(string systemId, bool isOnline)
        {
            ServiceInfoData[] data = (ServiceInfoData[])Array.CreateInstance(typeof(ServiceInfoData), SupportedServices.Length);

            lock (_serviceDataLock)
            {
                for (int i = 0; i < SupportedServices.Length;++i)
                {
                    ServiceInfoData serviceInfo;
                    if (_serviceData[i].TryGetValue(systemId, out serviceInfo))
                    {
                        data[i] = serviceInfo.Clone();
                        data[i].isAvailable = data[i].isAvailable && isOnline;
                    }
                }
            }

            bool[] subscribed;
            string notificationUrl;
            lock (_serviceSubscriptionLock)
            {
                notificationUrl = _serviceNotificationUrl;
                subscribed = (bool[])_serviceSubscriptions.Clone();
            }

            if (!string.IsNullOrEmpty(notificationUrl))
            {
                using (NotificationClient client = new NotificationClient("NotificationClient", notificationUrl))
                {
                    try
                    {
                        DataPackageTests.T2GServiceInterface.Notification.serviceList serviceList = new DataPackageTests.T2GServiceInterface.Notification.serviceList();
                        serviceList.Capacity = 1;

                        for (int i = 0; i < subscribed.Length; ++i)
                        {
                            if (subscribed[i] && data[i] != null)
                            {
                                if (client.State == CommunicationState.Closed)
                                {
                                    client.Open();
                                }

                                serviceList.Add(data[i]);
                                client.onServiceNotification(systemId, isOnline, i + 1, serviceList);
                                serviceList.Clear();

                            }
                        }

                        if (client.State == CommunicationState.Opened)
                        {
                            client.Close();
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
        }

        #endregion
    }
}
