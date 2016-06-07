namespace GroundCore.Data
{
    using System;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Collection of Types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CollectionDataContract(Name = "Element{0}List")]
    public class ElementList<T> : List<T>
    {
        public ElementList()
            : base()
        {
        }

        public ElementList(T[] items)
            : base()
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }
    }

    /// <summary>
    /// Represents each Element data
    /// </summary>
    
    [DataContract]
    public class ElementData
    {

        #region DataMember
        /// <summary>
        /// Element alpha number
        /// </summary>
        [DataMember]
        string strElementNumber;

        /// <summary>
        /// Mission Commercial Number 
        /// </summary>
        [DataMember]
        string strMissionCommercialNumber;

        /// <summary>
        /// Mission Operator Code 
        /// </summary>
        [DataMember]
        string strMissionOperatorCode;

        /// <summary>
        /// Online status
        /// </summary>
        [DataMember]
        bool bOnlineStatus;

        /// <summary>
        /// PIS basic package version
        /// </summary>
        [DataMember]
        string strPIS_BasicPackageVersion;

        /// <summary>
        /// LMT package version
        /// </summary>
        [DataMember]
        string strLMT_PackageVersion;
        #endregion

    }

    public class Station
    {
    }
}
