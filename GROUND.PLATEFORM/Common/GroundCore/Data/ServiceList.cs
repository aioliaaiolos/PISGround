using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundCore.Data
{
    public class ServiceList : System.Collections.Generic.List<ServiceInfo>
    {
    }

    public class ServiceInfo
    {
        private ushort iVehiclePhysicalIdField;

        private ushort iServiceIdField;

        private string strNameField;

        private ushort iOperatorIdField;

        private string strAIDField;

        private string strSIDField;

        private string strServiceIPAddressField;

        private ushort iServicePortNumberField;

        private bool bIsAvailableField;

        #region Properties
        public ushort VehiclePhysicalIdField
        {
            get
            {
                return iVehiclePhysicalIdField;
            }
            set
            {
                iVehiclePhysicalIdField = value;
            }
        }
        public ushort ServiceIdField
        {
            get
            {
                return iServiceIdField;
            }
            set
            {
                iServiceIdField = value;
            }
        }
        public string ServiceName
        {
            get
            {
                return strNameField;
            }
            set
            {
                strNameField = value;
            }
        }
        public ushort OperatorIdField
        {
            get
            {
                return iOperatorIdField;
            }
            set
            {
                iOperatorIdField = value;
            }
        }
        public string AIDField
        {
            get
            {
                return strAIDField;
            }
            set
            {
                strAIDField = value;
            }
        }
        public string SIDField
        {
            get
            {
                return strSIDField;
            }
            set
            {
                strSIDField = value;
            }
        }
        public string ServiceIPAddressField
        {
            get
            {
                return strServiceIPAddressField;
            }
            set
            {
                strServiceIPAddressField = value;
            }
        }
        public ushort ServicePortNumberField
        {
            get
            {
                return iServicePortNumberField;
            }
            set
            {
                iServicePortNumberField = value;
            }
        }
        public bool IsAvailable
        {
            get
            {
                return bIsAvailableField;
            }
            set
            {
                bIsAvailableField = value;
            }
        }

        #endregion

        public ServiceInfo()
        {
            iServiceIdField = 0;
            strNameField = string.Empty;
            iVehiclePhysicalIdField = 0;
            strServiceIPAddressField = string.Empty;
            IsAvailable = false;
        }

        public ServiceInfo(ushort piServiceIdField, string pstrServiceName, ushort piVehiclePhysicalIdField,
            ushort piOperatorIdField, bool pIsAvailable, string pstrServiceIPAddressField, string pstrAIDField, string pstrSIDField, ushort piServicePortNumberField)
        {
            iServiceIdField = piServiceIdField;
            strNameField = pstrServiceName;
            iVehiclePhysicalIdField = piVehiclePhysicalIdField;
            iOperatorIdField = piOperatorIdField;
            IsAvailable = pIsAvailable;
            strServiceIPAddressField = pstrServiceIPAddressField;
            iServicePortNumberField = piServicePortNumberField;
            strSIDField = pstrSIDField;
            strAIDField = pstrAIDField;
        }
    }
}
