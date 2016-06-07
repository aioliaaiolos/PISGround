using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PIS.Ground.Core.Data
{
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "ManualModeRequest")]
    public class ManualModeRequest : MissionInitializeCommonRequest
    {
        /// <summary>
        /// Code of the mission type
        /// </summary>
        [DataMember]
        public string MissionTypeCode;

        /// <summary>
        /// list of commercial numbers of the mission
        /// </summary>
        [DataMember]
        public List<string> CommercialNumberList=new List<string>();

     
        /// <summary>
        /// It is a list of arrival times and departure times for each station
        /// </summary>
        [DataMember]
        public List<StationServiceHours> ServiceHourList=new List<StationServiceHours>();

        /// <summary>
        /// identifying the train to be used to initialize a mission
        /// </summary>
        [DataMember]
        public string TrainName;

        /// <summary>
        /// Sorted list of the stations of the active mission
        /// </summary>
        [DataMember]
        public List<StationList> ServicedStationList=new List<StationList>();

        /// <summary>
        /// identifying the region associated to the mission
        /// </summary>
        [DataMember]
        public uint RegionCode;
    }
}
