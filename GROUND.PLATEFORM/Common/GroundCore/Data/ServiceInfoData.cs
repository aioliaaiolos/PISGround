//---------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfoData.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.Data
{
    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Representing Service information
    /// </summary>
    public class ServiceInfo
    {
        #region Private Variables
        
        /// <summary>Identifier for the vehicle physical.</summary>
        private ushort _vehiclePhysicalId;

        /// <summary>Identifier for the service.</summary>
        private ushort _id;

        /// <summary>The name.</summary>
        private string _name;

        /// <summary>Gets the identifier of the operator.</summary>
        /// <value>The identifier of the operator.</value>
        private ushort _operatorId;

        /// <summary>The AID.</summary>
        private string _aid;

        /// <summary>The SID.</summary>
        private string _sid;

        /// <summary>The IP.</summary>
        private string _ip;

        /// <summary>The port.</summary>
        private ushort _port;

        /// <summary>true if available.</summary>
        private bool _available;
        
        #endregion

        #region Constructor
        
        public ServiceInfo()
        {
            this._id = 0;
            this._name = string.Empty;
            this._vehiclePhysicalId = 0;
            this._operatorId = 0;
            this._available = false;
            this._ip = string.Empty;
            this._port = 0;
            this._sid = string.Empty;
            this._aid = string.Empty;
        }

        public ServiceInfo(
            ushort piServiceIdField,
            string pstrServiceName,
            ushort piVehiclePhysicalIdField,
            ushort piOperatorIdField,
            bool pIsAvailable,
            string pstrServiceIPAddressField,
            string pstrAIDField,
            string pstrSIDField,
            ushort piServicePortNumberField)
        {
            this._id = piServiceIdField;
            this._name = pstrServiceName;
            this._vehiclePhysicalId = piVehiclePhysicalIdField;
            this._operatorId = piOperatorIdField;
            this._available = pIsAvailable;
            this._ip = pstrServiceIPAddressField;
            this._port = piServicePortNumberField;
            this._sid = pstrSIDField;
            this._aid = pstrAIDField;
        }
        
        #endregion

        #region Properties

        public ushort VehiclePhysicalId
        {
            get
            {
                return this._vehiclePhysicalId;
            }            
        }

        public ushort ServiceId
        {
            get
            {
                return this._id;
            }        
        }

        public string ServiceName
        {
            get
            {
                return this._name;
            }
        }

        public ushort OperatorId
        {
            get
            {
                return this._operatorId;
            }
        }

        public string AID
        {
            get
            {
                return this._aid;
            }
        }

        public string SID
        {
            get
            {
                return this._sid;
            }
        }

        public string ServiceIPAddress
        {
            get
            {
                return this._ip;
            }
        }

        public ushort ServicePortNumber
        {
            get
            {
                return this._port;
            }
        }

        public bool IsAvailable
        {
            get
            {
                return this._available;
            }
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int lHashCode;

            lHashCode = this._vehiclePhysicalId +
                        this._id +
                        this._name.GetHashCode() +
                        this._operatorId +
                        this._aid.GetHashCode() +
                        this._sid.GetHashCode() +
                        this._ip.GetHashCode() +
                        this._port +
                        Convert.ToInt32(this._available);
            
            return lHashCode;
        }

        #endregion

        #region Equality comparison

        /// <summary>
        /// Determines whether the specified object is equal to the current
        /// object.
        /// </summary>
        /// <param name="obj">The object to compare with the current
        /// </param>
        /// <returns>
        /// true if the specified object is equal to the current
        /// object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            ServiceInfo lOther = obj as ServiceInfo;

            // If parameter is null return false:
            if (lOther == null)
            {
                return false;
            }

            if (this.VehiclePhysicalId != lOther.VehiclePhysicalId) { return false; }
            if (this.ServiceId != lOther.ServiceId) { return false; }
            if (this.ServiceName != lOther.ServiceName) { return false; }
            if (this.OperatorId != lOther.OperatorId) { return false; }
            if (this.AID != lOther.AID) { return false; }
            if (this.SID != lOther.SID) { return false; }
            if (this.ServiceIPAddress != lOther.ServiceIPAddress) { return false; }
            if (this.ServicePortNumber != lOther.ServicePortNumber) { return false; }
            if (this.IsAvailable != lOther.IsAvailable) { return false; }

            return true;
        }
        #endregion

        #region Dump and ToString()

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder(200);
            Dump(string.Empty, output);
            return output.ToString();
        }

        /// <summary>
        /// Dumps the current object into an output string.
        /// </summary>
        /// <param name="prefix">The prefix to add when writing each member.</param>
        /// <param name="output">The output string.</param>
        public void Dump(string prefix, StringBuilder output)
        {
            string memberPrefix = prefix + "\t";
            output.Append("{");

            output.AppendFormat(CultureInfo.InvariantCulture, "ServiceName = '{0}',", ServiceName).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}Id = '{1}',", memberPrefix, ServiceId).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}IsAvailable = '{1}',", memberPrefix, IsAvailable).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}VehicleId = '{1}',", memberPrefix, VehiclePhysicalId).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}Port = '{1}',", memberPrefix, ServicePortNumber).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}IP = '{1}',", memberPrefix, ServiceIPAddress).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}OperatorId = '{1}',", memberPrefix, OperatorId).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}AID = '{1}',", memberPrefix, AID).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}SID = '{1}'", memberPrefix, SID);

            output.Append("}");
        }


        #endregion
    }
}
