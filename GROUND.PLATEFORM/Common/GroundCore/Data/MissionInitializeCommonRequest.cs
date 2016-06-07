using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PIS.Ground.Core.Data
{
    /// <summary>
    /// Represents each Element data
    /// </summary>    
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "MissionInitializeCommonRequest")]
    public abstract class MissionInitializeCommonRequest
    {
        #region DataMember
        /// <summary>
        /// session Id of the 
        /// </summary>
        [DataMember]
        public Guid SessionId;

        /// <summary>
        /// Specify if a command needs a validation by the train officer .
        /// </summary>
        [DataMember]
        public bool OnBoardValidationFlag;

        /// <summary>
        /// The element alpha number.
        /// </summary>
        [DataMember]
        public string ElementAlphaNumber;

        /// <summary>
        /// ID of the current mission of the element. 
        /// </summary>
        [DataMember]
        public string MissionOperatorCode;

        /// <summary>
        /// Date when the mission starts.
        /// </summary>
        [DataMember]
        public string StartDate;

        /// <summary>
        /// versions of the LMT data package associated with the current baseline installed 
        /// </summary>
        [DataMember]
        public string LmtDataPackageVersion;

        /// <summary>
        /// list of language codes to be used during the mission. 
        /// </summary>
        [DataMember]
        public List<string> LanguageCodeList;

        /// <summary>
        /// list of codes of services available onboard the element. 
        /// </summary>
        [DataMember]
        public List<uint> OnboardServiceCodeList;

        /// <summary>
        /// This is the code identifying the prefix to be used to complement the element’s car ids.
        /// </summary>
        [DataMember]
        public uint CarNumberingOffsetCode;

        /// <summary>
        /// It specifies the delay after which the request will be dropped if it is not transmitted to the element.
        /// </summary>
        [DataMember]
        public uint RequestTimeout;
        
        #endregion
    }
}
