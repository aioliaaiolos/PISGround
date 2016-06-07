using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace PIS.Ground.Core.Data
{
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "AutomaticModeRequest")]
    public class AutomaticModeRequest : MissionInitializeCommonRequest
    {
        #region DataMember
        /// <summary>
        /// Id of the mission insertion station
        /// </summary>
        [DataMember]
        public StationInsertion StationInsertion;

        /// <summary>
        /// Code identifying the type of the mission insertion. 
        /// </summary>
        //[DataMember]
        //public MissionInsertionType MissionInsertionTypeCode;
        #endregion
    }

    [System.Runtime.Serialization.DataContractAttribute(Name = "StationInsertion", Namespace = "http://alstom.com/pacis/pis/schema/train/mission/")]
    public partial class StationInsertion
    {
        private string StationIdField;

        private uint TypeField;

        [DataMember]
        public string StationId
        {
            get
            {
                return this.StationIdField;
            }
            set
            {
                this.StationIdField = value;
            }
        }

        [DataMember]
        public uint Type
        {
            get
            {
                return this.TypeField;
            }
            set
            {
                this.TypeField = value;
            }
        }
    }

    [System.Runtime.Serialization.DataContractAttribute(Name = "StationList", Namespace = "http://alstom.com/pacis/pis/schema/train/mission/")]
    public partial class StationList 
    {

        private string IdField;

        private string NameField;

        [DataMember]
        public string Id
        {
            get
            {
                return this.IdField;
            }
            set
            {
                this.IdField = value;
            }
        }

        [DataMember]
        public string Name
        {
            get
            {
                return this.NameField;
            }
            set
            {
                this.NameField = value;
            }
        }
    }

    [System.Xml.Serialization.XmlSchemaProvider("ProvideSchema")]    
    public class time : System.Xml.Serialization.IXmlSerializable
    {
        DateTime _dateTime;
        bool _isInitialized;
        public time() { _dateTime = new DateTime(); _isInitialized = false; }
        public time(DateTime pDateTime) { _dateTime = pDateTime; _isInitialized = true; }

        #region IXmlSerializable Members

        public static System.Xml.XmlQualifiedName ProvideSchema(System.Xml.Schema.XmlSchemaSet xs)
        {
            return new System.Xml.XmlQualifiedName("time", "http://www.w3.org/2001/XMLSchema");
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            try
            {
                // additional parsing can be done here...
                this._dateTime = reader.ReadElementContentAsDateTime();
                _isInitialized = true;
            }
            catch (System.Exception)
            {
                _isInitialized = false;
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteValue(this.ToString());
        }

        #endregion

        public override string ToString()
        {
            if (this._isInitialized)
            {
                return this._dateTime.ToString("HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                return "";
            }
        }

        public static implicit operator string(time pInstance)
        {
            return pInstance.ToString();
        }

    }



    [System.Runtime.Serialization.DataContractAttribute(Name = "StationServiceHours", Namespace = "http://alstom.com/pacis/pis/schema/train/mission/")]
    public partial class StationServiceHours 
    {

        private time ArrivalTimeField;

        private time DepartureTimeField;

        [DataMember]
        public time ArrivalTime
        {
            get
            {
                return this.ArrivalTimeField;
            }
            set
            {
                this.ArrivalTimeField = value;
            }
        }

        [DataMember]
        public time DepartureTime
        {
            get
            {
                return this.DepartureTimeField;
            }
            set
            {
                this.DepartureTimeField = value;
            }
        }
        [OnDeserializing]
        public void OnDeserializing(StreamingContext ctx)
        {
            ArrivalTimeField = new time();
            DepartureTimeField = new time();
        }
    }
    
}
