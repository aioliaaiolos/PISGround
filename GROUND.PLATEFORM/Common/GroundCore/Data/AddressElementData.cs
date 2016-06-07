/// 
namespace PIS.Ground.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Address data type
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "AddressTypeEnum")]
    public enum AddressTypeEnum
    {
        [EnumMember(Value = "Element")]
        Element,

        [EnumMember(Value = "MissionOperatorCode")]
        MissionOperatorCode,

        [EnumMember(Value = "MissionCode")]
        MissionCode
    }

    /// <summary>
    /// Target Address data type 
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "TargetAddressType")]
    public class TargetAddressType
    {
        [DataMember(IsRequired = true)]
        public AddressTypeEnum Type;

        [DataMember(IsRequired = true)]
        public string Id;

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int lHashCode = 0;
            if (Id != null)
            {
                lHashCode = Id.GetHashCode();
            }
            return lHashCode;
        }

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
            bool lIsEqual = false;

            // If parameter is null return false.
            if (obj == null)
            {
                lIsEqual = false;
            }
            else
            {
                TargetAddressType other = obj as TargetAddressType;
                if (other!=null && other.Id == this.Id && other.Type == this.Type)
                {
                    lIsEqual = true;
                }
            }
            return lIsEqual;
        }
    }
}
