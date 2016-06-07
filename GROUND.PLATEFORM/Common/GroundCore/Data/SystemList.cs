using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundCore.Data
{
    public class SystemList : System.Collections.Generic.List<SystemInfo>
    {
    }

    public enum CommunicationLink
    {
        WiFi = 0,
        _2G3G = 1,
        NotApplicable = 2,
    }

    public class SystemInfo 
    {
        private string strSystemIdField;

        private int iVehiclePhysicalIdField;

        private string strMissionIdField;

        private CommunicationLink eCommunicationLinkField;

        private uint iStatusField;

        private bool bIsOnlineField;

        #region Properties
        public int VehiclePhysicalIdField
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
        public string SystemIdField
        {
            get
            {
                return strSystemIdField;
            }
            set
            {
                strSystemIdField = value;
            }
        }
        public string MissionIdField
        {
            get
            {
                return strMissionIdField;
            }
            set
            {
                strMissionIdField = value;
            }
        }
        public CommunicationLink CommunicationLinkField
        {
            get
            {
                return eCommunicationLinkField;
            }
            set
            {
                eCommunicationLinkField = value;
            }
        }
        public uint StatusField
        {
            get
            {
                return iStatusField;
            }
            set
            {
                iStatusField = value;
            }
        }
        public bool IsOnlineField
        {
            get
            {
                return bIsOnlineField;
            }
            set
            {
                bIsOnlineField = value;
            }
        }

        #endregion

        public SystemInfo()
        {
            strSystemIdField = string.Empty;
            strMissionIdField = string.Empty;
            iVehiclePhysicalIdField = 0;
            iStatusField = 0;
            bIsOnlineField = false;
        }

        public SystemInfo(string pstrSystemIdField, string pstrMissionFiled, int piVehiclePhysicalIdField,
            uint piStatus, bool pIsOnline, CommunicationLink peCommunicationLinkField)
        {
            strSystemIdField = pstrSystemIdField;
            strMissionIdField = pstrMissionFiled;
            iVehiclePhysicalIdField = piVehiclePhysicalIdField;
            iStatusField = piStatus;
            bIsOnlineField = pIsOnline;
            eCommunicationLinkField = peCommunicationLinkField;
        }
    }
}
