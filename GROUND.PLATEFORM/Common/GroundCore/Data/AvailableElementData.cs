namespace PIS.Ground.Core.Data
{
    using System;    
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Collection of Types
    /// </summary>
    /// <typeparam name="T">Type of which the collection holds</typeparam>
    [CollectionDataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "Element{0}List")]
    public class ElementList<T> : List<T>
    {
    }

    /// <summary>
    /// Represents each Element data
    /// </summary>    
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "AvailableElementData")]
    public class AvailableElementData
    {
        #region DataMember
        /// <summary>
        /// Element alpha number
        /// </summary>
        [DataMember]
        public string ElementNumber;

        /// <summary>
        /// Mission Commercial Number 
        /// </summary>
        [DataMember]
        public string MissionCommercialNumber;

        /// <summary>
        /// Mission Operator Code 
        /// </summary>
        [DataMember]
        public string MissionOperatorCode;

		/// <summary>
        /// Mission State
        /// </summary>
		[DataMember]
		public MissionStateEnum MissionState;

        /// <summary>
        /// Online status
        /// </summary>
        [DataMember]
        public bool OnlineStatus;

        /// <summary>
        /// PIS basic package version
        /// </summary>
        [DataMember]
        public string PisBasicPackageVersion;

        /// <summary>
        /// LMT package version
        /// </summary>
        [DataMember]
        public string LmtPackageVersion;

        /// <summary>
        /// PIS basic package version
        /// </summary>
        [DataMember]
        public PisBaseline PisBaselineData;

        #endregion
    }
}
