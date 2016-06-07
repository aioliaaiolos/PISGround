/// 
namespace PIS.Ground.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Represents each Element data
    /// </summary>    
    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "PisBaseline")]
    public class PisBaseline
    {
        #region DataMember
        /// <summary>
        /// Future Version PisInfotainment Out
        /// </summary>
        [DataMember]
        public string FutureVersionPisInfotainmentOut;

        /// <summary>
        /// Future Version Lmt Out
        /// </summary>
        [DataMember]
        public string FutureVersionLmtOut;

         /// <summary>
        /// Future ActivationDate Out
        /// </summary>
        [DataMember]
        public string FutureActivationDateOut;

        /// <summary>
        /// Future ExpirationDate Out
        /// </summary>
        [DataMember]
        public string FutureExpirationDateOut;

        /// <summary>
        /// Future Valid Out
        /// </summary>
        [DataMember]
        public string FutureValidOut;

        /// <summary>
        /// Future Version Out
        /// </summary>
        [DataMember]
        public string FutureVersionOut;

         /// <summary>
        /// Future Version PisBase Out
        /// </summary>
        [DataMember]
        public string FutureVersionPisBaseOut;

        /// <summary>
        /// Future Version PisMission Out
        /// </summary>
        [DataMember]
        public string FutureVersionPisMissionOut;

        /// <summary>
        /// Current Version Lmt Out
        /// </summary>
        [DataMember]
        public string CurrentVersionLmtOut;

        /// <summary>
        /// Current ExpirationDate Out
        /// </summary>
        [DataMember]
        public string CurrentExpirationDateOut;

         /// <summary>
        /// Current Version PisMission Out
        /// </summary>
        [DataMember]
        public string CurrentVersionPisMissionOut;

        /// <summary>
        /// Current Version PisInfotainment Out
        /// </summary>
        [DataMember]
        public string CurrentVersionPisInfotainmentOut;

        /// <summary>
        /// Current Version Out
        /// </summary>
        [DataMember]
        public string CurrentVersionOut;

        /// <summary>
        /// Current Version PisBase Out
        /// </summary>
        [DataMember]
        public string CurrentVersionPisBaseOut;

        /// <summary>
        /// Current Forced Out
        /// </summary>
        [DataMember]
        public string CurrentForcedOut;

        /// <summary>
        /// Current Valid Out
        /// </summary>
        [DataMember]
        public string CurrentValidOut;

        /// <summary>
        /// Archived Valid Out
        /// </summary>
        [DataMember]
        public string ArchivedValidOut;

        /// <summary>
        /// Archived Version Out
        /// </summary>
        [DataMember]
        public string ArchivedVersionOut;

        /// <summary>
        /// Archived Version PisBase Out
        /// </summary>
        [DataMember]
        public string ArchivedVersionPisBaseOut;

        /// <summary>
        /// Archived Version PisMission Out
        /// </summary>
        [DataMember]
        public string ArchivedVersionPisMissionOut;

        /// <summary>
        /// Archived Version PisInfotainment Out
        /// </summary>
        [DataMember]
        public string ArchivedVersionPisInfotainmentOut;

        /// <summary>
        /// Archived Version Lmt Out
        /// </summary>
        [DataMember]
        public string ArchivedVersionLmtOut;
        
        #endregion

        public PisBaseline()
        {
            this.FutureVersionPisInfotainmentOut = string.Empty;
            this.FutureVersionLmtOut = string.Empty;
            this.FutureActivationDateOut = string.Empty;
            this.FutureExpirationDateOut = string.Empty;
            this.FutureValidOut = string.Empty;
            this.FutureVersionOut = string.Empty;
            this.FutureVersionPisBaseOut = string.Empty;
            this.FutureVersionPisMissionOut = string.Empty;
            this.CurrentVersionLmtOut = string.Empty;
            this.CurrentExpirationDateOut = string.Empty;
            this.CurrentVersionPisMissionOut = string.Empty;
            this.CurrentVersionPisInfotainmentOut = string.Empty;
            this.CurrentVersionOut = string.Empty;
            this.CurrentVersionPisBaseOut = string.Empty;
            this.CurrentForcedOut = string.Empty;
            this.CurrentValidOut = string.Empty;
            this.ArchivedValidOut = string.Empty;
            this.ArchivedVersionOut = string.Empty;
            this.ArchivedVersionPisBaseOut = string.Empty;
            this.ArchivedVersionPisMissionOut = string.Empty;
            this.ArchivedVersionPisInfotainmentOut = string.Empty;
            this.ArchivedVersionLmtOut = string.Empty;
        }

        public PisBaseline(PisBaseline other) : this()
        {
            if (other != null)
            {
                // shallow copy (copy references, since strings are immutable)
                                 
                this.FutureVersionPisInfotainmentOut = other.FutureVersionPisInfotainmentOut;
                this.FutureVersionLmtOut = other.FutureVersionLmtOut;
                this.FutureActivationDateOut = other.FutureActivationDateOut;
                this.FutureExpirationDateOut = other.FutureExpirationDateOut;
                this.FutureValidOut = other.FutureValidOut;
                this.FutureVersionOut = other.FutureVersionOut;
                this.FutureVersionPisBaseOut = other.FutureVersionPisBaseOut;
                this.FutureVersionPisMissionOut = other.FutureVersionPisMissionOut;
                this.CurrentVersionLmtOut = other.CurrentVersionLmtOut;
                this.CurrentExpirationDateOut = other.CurrentExpirationDateOut;
                this.CurrentVersionPisMissionOut = other.CurrentVersionPisMissionOut;
                this.CurrentVersionPisInfotainmentOut = other.CurrentVersionPisInfotainmentOut;
                this.CurrentVersionOut = other.CurrentVersionOut;
                this.CurrentVersionPisBaseOut = other.CurrentVersionPisBaseOut;
                this.CurrentForcedOut = other.CurrentForcedOut;
                this.CurrentValidOut = other.CurrentValidOut;
                this.ArchivedValidOut = other.ArchivedValidOut;
                this.ArchivedVersionOut = other.ArchivedVersionOut;
                this.ArchivedVersionPisBaseOut = other.ArchivedVersionPisBaseOut;
                this.ArchivedVersionPisMissionOut = other.ArchivedVersionPisMissionOut;
                this.ArchivedVersionPisInfotainmentOut = other.ArchivedVersionPisInfotainmentOut;
                this.ArchivedVersionLmtOut = other.ArchivedVersionLmtOut;
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
            PisBaseline lOther = obj as PisBaseline;

            // If parameter is null return false:
            if (lOther == null)
            {
                return false;
            }                        

            if (this.FutureVersionPisInfotainmentOut != lOther.FutureVersionPisInfotainmentOut) { return false; }
            if (this.FutureVersionLmtOut != lOther.FutureVersionLmtOut) { return false; }
            if (this.FutureActivationDateOut != lOther.FutureActivationDateOut) { return false; }
            if (this.FutureExpirationDateOut != lOther.FutureExpirationDateOut) { return false; }
            if (this.FutureValidOut != lOther.FutureValidOut) { return false; }
            if (this.FutureVersionOut != lOther.FutureVersionOut) { return false; }
            if (this.FutureVersionPisBaseOut != lOther.FutureVersionPisBaseOut) { return false; }
            if (this.FutureVersionPisMissionOut != lOther.FutureVersionPisMissionOut) { return false; }

            if (this.CurrentVersionLmtOut != lOther.CurrentVersionLmtOut) { return false; }
            if (this.CurrentExpirationDateOut != lOther.CurrentExpirationDateOut) { return false; }
            if (this.CurrentVersionPisMissionOut != lOther.CurrentVersionPisMissionOut) { return false; }
            if (this.CurrentVersionPisInfotainmentOut != lOther.CurrentVersionPisInfotainmentOut) { return false; }
            if (this.CurrentVersionOut != lOther.CurrentVersionOut) { return false; }
            if (this.CurrentVersionPisBaseOut != lOther.CurrentVersionPisBaseOut) { return false; }
            if (this.CurrentForcedOut != lOther.CurrentForcedOut) { return false; }
            if (this.CurrentValidOut != lOther.CurrentValidOut) { return false; }

            if (this.ArchivedValidOut != lOther.ArchivedValidOut) { return false; }
            if (this.ArchivedVersionPisBaseOut != lOther.ArchivedVersionPisBaseOut) { return false; }
            if (this.ArchivedVersionPisMissionOut != lOther.ArchivedVersionPisMissionOut) { return false; }
            if (this.ArchivedVersionPisInfotainmentOut != lOther.ArchivedVersionPisInfotainmentOut) { return false; }
            if (this.ArchivedVersionOut != lOther.ArchivedVersionOut) { return false; }
            if (this.ArchivedVersionLmtOut != lOther.ArchivedVersionLmtOut) { return false; }

            return true;
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return
                this.FutureVersionPisInfotainmentOut.GetHashCode() +
                this.FutureVersionLmtOut.GetHashCode() +
                this.FutureActivationDateOut.GetHashCode() +
                this.FutureExpirationDateOut.GetHashCode() +
                this.FutureValidOut.GetHashCode() +
                this.FutureVersionOut.GetHashCode() +
                this.FutureVersionPisBaseOut.GetHashCode() +
                this.FutureVersionPisMissionOut.GetHashCode() +
                this.CurrentVersionLmtOut.GetHashCode() +
                this.CurrentExpirationDateOut.GetHashCode() +
                this.CurrentVersionPisMissionOut.GetHashCode() +
                this.CurrentVersionPisInfotainmentOut.GetHashCode() +
                this.CurrentVersionOut.GetHashCode() +
                this.CurrentVersionPisBaseOut.GetHashCode() +
                this.CurrentForcedOut.GetHashCode() +
                this.CurrentValidOut.GetHashCode() +
                this.ArchivedValidOut.GetHashCode() +
                this.ArchivedVersionOut.GetHashCode() +
                this.ArchivedVersionPisBaseOut.GetHashCode() +
                this.ArchivedVersionPisMissionOut.GetHashCode() +
                this.ArchivedVersionPisInfotainmentOut.GetHashCode() +
                this.ArchivedVersionLmtOut.GetHashCode();
        }

        #endregion
    }
}
