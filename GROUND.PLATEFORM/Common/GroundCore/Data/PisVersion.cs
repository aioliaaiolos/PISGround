//---------------------------------------------------------------------------------------------------
// <copyright file="PisVersion.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.Data
{
    using System.Runtime.Serialization;
    using System;

    /// <summary>
    /// Represents each Element data
    /// </summary>    
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "PisVersion")]
    public class PisVersion
    {
        #region DataMember
        /// <summary>
		/// Version PIS Software
        /// </summary>
        [DataMember]
        public string VersionPISSoftware;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PisVersion"/> class.
        /// </summary>
        public PisVersion()
        {
            this.VersionPISSoftware = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PisVersion"/> class.
        /// </summary>
        /// <param name="version">The value for version.</param>
        public PisVersion(string version)
        {
            this.VersionPISSoftware = version;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PisVersion"/> class with values provider by another instance..
        /// </summary>
        /// <param name="other">The initial values.</param>
        public PisVersion(PisVersion other) : this()
        {
            if (other != null)
            {
                // shallow copy (copy references, since strings are immutable)

                this.VersionPISSoftware = other.VersionPISSoftware;                
            }        
        }

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
            PisVersion lOther = obj as PisVersion;

            // If parameter is null return false:
            if (lOther == null)
            {
                return false;
            }

            if (this.VersionPISSoftware != lOther.VersionPISSoftware) { return false; }

            return true;
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return this.VersionPISSoftware.GetHashCode();            
        }

        #endregion
    }
}
