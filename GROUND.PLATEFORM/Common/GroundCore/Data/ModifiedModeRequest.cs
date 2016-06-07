using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PIS.Ground.Core.Data
{
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "ModifiedModeRequest")]
    public class ModifiedModeRequest : ManualModeRequest
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
}
