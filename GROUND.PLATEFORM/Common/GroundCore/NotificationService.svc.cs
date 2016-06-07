///
namespace PIS.Ground.Core.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Text;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.T2G;

    /// <summary>
    /// 
    /// </summary>
    public abstract class NotificationService : INotificationBinding
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public void onMessageNotification(string systemId, string messageId, fieldStruct[] fieldList)
        {
            T2GNotifier objT2GNotifier = new T2GNotifier();
            System.IO.StreamWriter swLog = new System.IO.StreamWriter("D:\\Test.txt",true);
            swLog.WriteLine(DateTime.Now.ToLongTimeString() + " :Got the message Notification", true);
            swLog.Flush();
            swLog.Close();
            objT2GNotifier.MessageNotification(systemId, messageId, fieldList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public void onServiceNotification(string systemId, bool isSystemOnline, int subscriptionId, serviceStruct[] serviceList)
        {
            T2GNotifier objT2GNotifier = new T2GNotifier();
            System.IO.StreamWriter swLog = new System.IO.StreamWriter("D:\\Test.txt",true);
            swLog.WriteLine(DateTime.Now.ToLongTimeString() + " :Got the service Notification", true);
            swLog.Flush();
            swLog.Close();
            objT2GNotifier.ServiceNotification(systemId, isSystemOnline, subscriptionId, serviceList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskState"></param>
        /// <param name="taskPhase"></param>
        /// <param name="activeFileTransferCount"></param>
        /// <param name="errorCount"></param>
        /// <param name="acquisitionCompletionPercent"></param>
        /// <param name="transferCompletionPercent"></param>
        /// <param name="distributionCompletionPercent"></param>
        public void onFileTransferNotification(int taskId, taskStateEnum taskState, taskPhaseEnum taskPhase, ushort activeFileTransferCount, ushort errorCount, sbyte acquisitionCompletionPercent, sbyte transferCompletionPercent, sbyte distributionCompletionPercent)
        {
            T2GNotifier objT2GNotifier = new T2GNotifier();
            objT2GNotifier.FileTransferNotification(taskId, taskState, taskPhase, activeFileTransferCount, errorCount, acquisitionCompletionPercent, transferCompletionPercent, distributionCompletionPercent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public void onFilePublicationNotification(int folderId, sbyte completionPercent, acquisitionStateEnum acquisitionState, string error)
        {
            T2GNotifier objT2GNotifier = new T2GNotifier();
            objT2GNotifier.FilePublicationNotification( folderId,  completionPercent,  acquisitionState,  error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderId"></param>
        public void onFilesReceivedNotification(int folderId)
        {
            T2GNotifier objT2GNotifier = new T2GNotifier();
            objT2GNotifier.FilesReceivedNotification(folderId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public void onFilesPublishedNotification(int folderId, string systemId)
        {
            T2GNotifier objT2GNotifier = new T2GNotifier();
            objT2GNotifier.FilesPublishedNotification( folderId, systemId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public void onEventEnumsNotification(int requestId, sbyte completionPercent, eventStruct[] eventList)
        {
            T2GNotifier objT2GNotifier = new T2GNotifier();
            objT2GNotifier.EventEnumsNotification(requestId, completionPercent, eventList);
        }    
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name = "eventList", Namespace = "http://alstom.com/T2G/Notification", ItemName = "item")]
    public class eventList : System.Collections.Generic.List<eventStruct>
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name = "serviceList", Namespace = "http://alstom.com/T2G/Notification", ItemName = "item")]
    public class serviceList : System.Collections.Generic.List<serviceStruct>
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name = "fieldList", Namespace = "http://alstom.com/T2G/Notification", ItemName = "item")]
    public class fieldList : System.Collections.Generic.List<fieldStruct>
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://alstom.com/T2G/Notification")]
    public partial class fieldStruct
    {

        private string idField;

        private fieldTypeEnum typeField;

        private string valueField;

        /// <remarks/>
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public fieldTypeEnum type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://alstom.com/T2G/Notification")]
    public enum fieldTypeEnum
    {

        /// <remarks/>
        unknown,

        /// <remarks/>
        @string,

        /// <remarks/>
        boolean,

        /// <remarks/>
        @byte,

        /// <remarks/>
        unsignedByte,

        /// <remarks/>
        @short,

        /// <remarks/>
        unsignedShort,

        /// <remarks/>
        @int,

        /// <remarks/>
        unsignedInt,

        /// <remarks/>
        @float,

        /// <remarks/>
        @double,

        /// <remarks/>
        dateTime,

        /// <remarks/>
        @long,

        /// <remarks/>
        int64,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://alstom.com/T2G/Notification")]
    public partial class eventStruct
    {

        private string idField;

        private eventTypeEnum typeField;

        private eventSeverityEnum severityField;

        private alarmStateEnum alarmStateField;

        private string descriptionField;

        private string sourceField;

        private System.DateTime dateField;

        /// <remarks/>
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public eventTypeEnum type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public eventSeverityEnum severity
        {
            get
            {
                return this.severityField;
            }
            set
            {
                this.severityField = value;
            }
        }

        /// <remarks/>
        public alarmStateEnum alarmState
        {
            get
            {
                return this.alarmStateField;
            }
            set
            {
                this.alarmStateField = value;
            }
        }

        /// <remarks/>
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public string source
        {
            get
            {
                return this.sourceField;
            }
            set
            {
                this.sourceField = value;
            }
        }

        /// <remarks/>
        public System.DateTime date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://alstom.com/T2G/Notification")]
    public enum eventTypeEnum
    {

        /// <remarks/>
        @event,

        /// <remarks/>
        alarm,

        /// <remarks/>
        fault,

        /// <remarks/>
        anyType,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://alstom.com/T2G/Notification")]
    public enum eventSeverityEnum
    {

        /// <remarks/>
        warning,

        /// <remarks/>
        minor,

        /// <remarks/>
        major,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://alstom.com/T2G/Notification")]
    public enum alarmStateEnum
    {

        /// <remarks/>
        raised,

        /// <remarks/>
        cleared,

        /// <remarks/>
        none,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://alstom.com/T2G/Notification")]
    public partial class serviceStruct
    {

        private ushort vehiclePhysicalIdField;

        private ushort serviceIdField;

        private string nameField;

        private ushort operatorIdField;

        private string aIDField;

        private string sIDField;

        private string serviceIPAddressField;

        private ushort servicePortNumberField;

        private bool isAvailableField;

        /// <remarks/>
        public ushort vehiclePhysicalId
        {
            get
            {
                return this.vehiclePhysicalIdField;
            }
            set
            {
                this.vehiclePhysicalIdField = value;
            }
        }

        /// <remarks/>
        public ushort serviceId
        {
            get
            {
                return this.serviceIdField;
            }
            set
            {
                this.serviceIdField = value;
            }
        }

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public ushort operatorId
        {
            get
            {
                return this.operatorIdField;
            }
            set
            {
                this.operatorIdField = value;
            }
        }

        /// <remarks/>
        public string AID
        {
            get
            {
                return this.aIDField;
            }
            set
            {
                this.aIDField = value;
            }
        }

        /// <remarks/>
        public string SID
        {
            get
            {
                return this.sIDField;
            }
            set
            {
                this.sIDField = value;
            }
        }

        /// <remarks/>
        public string serviceIPAddress
        {
            get
            {
                return this.serviceIPAddressField;
            }
            set
            {
                this.serviceIPAddressField = value;
            }
        }

        /// <remarks/>
        public ushort servicePortNumber
        {
            get
            {
                return this.servicePortNumberField;
            }
            set
            {
                this.servicePortNumberField = value;
            }
        }

        /// <remarks/>
        public bool isAvailable
        {
            get
            {
                return this.isAvailableField;
            }
            set
            {
                this.isAvailableField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://alstom.com/T2G/Notification")]
    public enum taskStateEnum
    {

        /// <remarks/>
        taskCreated,

        /// <remarks/>
        taskStarted,

        /// <remarks/>
        taskStopped,

        /// <remarks/>
        taskCancelled,

        /// <remarks/>
        taskCompleted,

        /// <remarks/>
        taskError,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://alstom.com/T2G/Notification")]
    public enum taskPhaseEnum
    {

        /// <remarks/>
        creationPhase,

        /// <remarks/>
        acquisitionPhase,

        /// <remarks/>
        transferPhase,

        /// <remarks/>
        distributionPhase,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://alstom.com/T2G/Notification")]
    public enum acquisitionStateEnum
    {

        /// <remarks/>
        notAcquired,

        /// <remarks/>
        acquisitionStarted,

        /// <remarks/>
        acquisitionStopped,

        /// <remarks/>
        acquisitionError,

        /// <remarks/>
        acquisitionSuccess,
    }
}
