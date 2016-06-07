//---------------------------------------------------------------------------------------------------
// <copyright file="PisMission.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
namespace PIS.Ground.Core.Data
{
    /// <summary>
    /// Represents each Mission data for an element
    /// </summary>        
    public class PisMission
    {
        /// <summary>The commercial number.</summary>
        public string CommercialNumber;

        /// <summary>The operator code.</summary>
        public string OperatorCode;

        /// <summary>State of the mission.</summary>
        public MissionStateEnum MissionState;

        public PisMission()
        {
            this.CommercialNumber = string.Empty;
            this.OperatorCode = string.Empty;
            this.MissionState = MissionStateEnum.NI;
        }

        /// <summary>Initializes a new instance of the PisMission class.</summary>
        /// <param name="other">The other.</param>
        public PisMission(PisMission other) : this()
        {
            if (other != null)
            {
                this.CommercialNumber = other.CommercialNumber;
                this.OperatorCode = other.OperatorCode;
                this.MissionState = other.MissionState;
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
            PisMission other = obj as PisMission;

            // If parameter is null return false:
            if (other == null)
            {
                return false;
            }

            if (this.CommercialNumber != other.CommercialNumber) { return false; }
            if (this.OperatorCode != other.OperatorCode) { return false; }
            if (this.MissionState != other.MissionState) { return false; }            

            return true;
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return
                this.CommercialNumber.GetHashCode() +
                this.OperatorCode.GetHashCode() +
                this.MissionState.GetHashCode();
        }

        #endregion
    }
}
