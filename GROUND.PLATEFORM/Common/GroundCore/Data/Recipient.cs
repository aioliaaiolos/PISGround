///
namespace PIS.Ground.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Representiing recpient
    /// </summary>
    public class Recipient
    {
        #region variables

        /// <summary>
        /// Recipient ID
        /// </summary>
        private int recipientId;

        /// <summary>
        /// A T2G system identifier: "ground" or an on-board system identifier (as returned by the enumSystems request).
        /// </summary>
        private string systemId;
        
        /// <summary>
        /// The mission Identifier or an empty string, if a systemId is provided.
        /// </summary>
        private string missionId;

        /// <summary>
        /// The targeted application identifiers. If this field is empty, no notification will be sent when the files arrive at destination. Instead, the files will be moved to a shared inbox.
        /// </summary>        
        private string applicationIds;

        /// <summary>
        /// Task ID
        /// </summary>
        private int taskId;

        /// <summary>
        /// Folder ID
        /// </summary>
        private int folderId;

        #endregion

        #region Properties

        /// <summary>
        /// A T2G Recipient ID.
        /// </summary>
        public int RecipientId
        {
            get { return recipientId; }
            set { recipientId = value; }
        }

        /// <summary>
        /// A T2G system identifier: "ground" or an on-board system identifier (as returned by the enumSystems request).
        /// </summary>
        public string SystemId
        {
            get
            {
                return this.systemId;
            }

            set
            {
                this.systemId = value;
            }
        }

        /// <summary>
        /// The mission Identifier or an empty string, if a systemId is provided.
        /// </summary>
        public string MissionId
        {
            get
            {
                return this.missionId;
            }

            set
            {
                this.missionId = value;
            }
        }

        /// <summary>
        /// The targeted application identifiers. If this field is empty, no notification will be sent when the files arrive at destination.
        /// </summary>
        public string ApplicationIds
        {
            get
            {
                return this.applicationIds;
            }

            set
            {
                this.applicationIds = value;
            }
        }

        /// <summary>
        /// The T2G folder ID
        /// </summary>
        public int FolderId
        {
            get { return folderId; }
            set { folderId = value; }
        }

        /// <summary>
        /// The T2G task ID
        /// </summary>
        public int TaskId
        {
            get { return taskId; }
            set { taskId = value; }
        }

        #endregion
    }

    /// <summary>
    /// Representing recepient ID
    /// </summary>
    public class RecipientId
    {
        #region variables
        /// <summary>
        /// A T2G system identifier: "ground" or an on-board system identifier (as returned by the enumSystems request).
        /// </summary>
        private string systemId;

        /// <summary>
        /// The mission Identifier or an empty string, if a systemId is provided.
        /// </summary>
        private string missionId;

        /// <summary>
        /// The targeted application identifiers. If this field is empty, no notification will be sent when the files arrive at destination. Instead, the files will be moved to a shared inbox.
        /// </summary>        
        private string applicationId;
        #endregion

        #region Properties
        /// <summary>
        /// A T2G system identifier: "ground" or an on-board system identifier (as returned by the enumSystems request).
        /// </summary>
        public string SystemId
        {
            get
            {
                return this.systemId;
            }

            set
            {
                this.systemId = value;
            }
        }

        /// <summary>
        /// The mission Identifier or an empty string, if a systemId is provided.
        /// </summary>
        public string MissionId
        {
            get
            {
                return this.missionId;
            }

            set
            {
                this.missionId = value;
            }
        }

        /// <summary>
        /// The targeted application identifiers. If this field is empty, no notification will be sent when the files arrive at destination.
        /// </summary>
        public string ApplicationId
        {
            get
            {
                return this.applicationId;
            }

            set
            {
                this.applicationId = value;
            }
        }
        #endregion
    }
}
