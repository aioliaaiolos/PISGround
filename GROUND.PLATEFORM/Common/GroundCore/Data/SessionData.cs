namespace PIS.Ground.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Class represting the Session Details
    /// </summary>
    public class SessionData : IDisposable
    {
        #region Filelds
        private string strSessionID;
        private string strUserName;
        private string strPassword;
        private string strNotificationUrl;
        private DateTime objLoginDateTime;
        private DateTime objLastAccessedTime;
        private List<RequestDetails> lstRequestDetails;
        private bool disposed;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public SessionData()
        {
            this.lstRequestDetails = new List<RequestDetails>();
            this.objLoginDateTime = DateTime.Now;
            this.objLastAccessedTime = DateTime.Now;
        }

        /// <summary>
        /// Constructure with all the parameter
        /// </summary>
        /// <param name="pstrUserName">User name</param>
        /// <param name="pstrPassword">user password</param>
        /// <param name="pstrLoginUrl">Login URL</param>
        /// <param name="pLoginDateTime">Login Time</param>
        /// <param name="pLastAccessedTime">Last Access Date</param>
        /// <param name="plstRequestDetails">List of Request by the user</param>
        public SessionData(string pstrUserName, string pstrPassword, string pstrLoginUrl, DateTime pLogOnDateTime, DateTime pLastAccessedTime, List<RequestDetails> plstRequestDetails, string pstrSessionID)
        {
            this.lstRequestDetails = plstRequestDetails;
            this.strUserName = pstrUserName;
            this.strPassword = pstrPassword;
            this.strNotificationUrl = pstrLoginUrl;
            this.objLoginDateTime = pLogOnDateTime;
            this.objLastAccessedTime = pLastAccessedTime;
            this.strSessionID = pstrSessionID;
        }
        #endregion

        #region Properties
        public List<RequestDetails> RequestDetails
        {
            get
            {
                return this.lstRequestDetails;
            }
        }

        public string SessionID
        {
            get
            {
                return this.strSessionID;
            }

            set
            {
                this.strSessionID = value;
            }
        }

        public string Password
        {
            get
            {
                return this.strPassword;
            }

            set
            {
                this.strPassword = value;
            }
        }

        public string UserName
        {
            get
            {
                return this.strUserName;
            }

            set
            {
                this.strUserName = value;
            }
        }

        public string NotificationUrl
        {
            get
            {
                return this.strNotificationUrl;
            }

            set
            {
                this.strNotificationUrl = value;
            }
        }

        public DateTime LoginDateTime
        {
            get
            {
                return this.objLoginDateTime;
            }

            set
            {
                this.objLoginDateTime = value;
            }
        }

        public DateTime LastAccessedTime
        {
            get
            {
                return this.objLastAccessedTime;
            }

            set
            {
                this.objLastAccessedTime = value;
            }
        }
        #endregion

        #region Implementation of the Iist Interface

        /// <summary>
        /// Indexers for the parameter collections.
        /// </summary>
        public RequestDetails this[int Index]
        {
            get
            {
                return this.lstRequestDetails[Index];
            }

            set
            {
                this.lstRequestDetails[Index] = value;
            }
        }

        /// <summary>
        /// Returns the Count of list items
        /// </summary>
        public int RequestDetailsCount
        {
            get
            {
                return this.lstRequestDetails.Count;
            }
        }

        /// <summary>
        /// Add an item to the Parameter Collection.
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        /// 
        public void Add(RequestDetails objValue)
        {
            if (this.lstRequestDetails.Count == 0)
            {
                this.lstRequestDetails.Capacity = 1;
            }
            else
            {
                this.lstRequestDetails.Capacity = this.lstRequestDetails.Count + 1;
            }

            this.lstRequestDetails.Add(objValue);
        }

        /// <summary>
        /// Removes all items from the Parameter Collection.
        /// </summary>
        public void Clear()
        {
            this.lstRequestDetails.Clear();
        }

        /// <summary>
        /// Determines whether the Parameter Collection contains a specific value.
        /// </summary>
        /// <param name="objValue">Object for which the search has to be made.</param>
        /// <returns>Boolean Value indicating whether Parameter Collection contains particular object.
        /// </returns>
        public bool Contains(RequestDetails objValue)
        {
            return this.lstRequestDetails.Contains(objValue);
        }

        /// <summary>
        /// Determines the index of a specific item in the Parameter Collection.
        /// </summary>
        /// <param name="objValue">Object for which index has to be retreived.</param>
        /// <returns>Position of the object in the Parameter Collection.</returns>
        public int IndexOf(RequestDetails objValue)
        {
            return this.lstRequestDetails.IndexOf(objValue);
        }

        /// <summary>
        /// Inserts an item to the Parameter Collection at the specified position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="objValue"></param>
        public void Insert(int index, RequestDetails objValue)
        {
            this.lstRequestDetails.Insert(index, objValue);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the Parameter Collection.
        /// </summary>
        /// <param name="objValue">Object </param>
        public void Remove(RequestDetails objValue)
        {
            this.lstRequestDetails.Remove(objValue);
        }

        /// <summary>
        /// Removes the Parmeter Collection item at the specified index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this.lstRequestDetails.RemoveAt(index);
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Disposing the resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //// Use SupressFinalize in case a subclass
            //// of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposing the resources
        /// </summary>
        /// <param name="disposing">input dispose</param>
        protected virtual void Dispose(bool disposing)
        {
            //// If you need thread safety, use a lock around these 
            //// operations, as well as in your methods that use the resource.
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.strSessionID = string.Empty;
                    this.strUserName = string.Empty;
                    this.strPassword = string.Empty;
                    this.strNotificationUrl = string.Empty;
                    this.lstRequestDetails.Clear();
                }

                // Indicate that the instance has been disposed.
                this.disposed = true;
            }
        }
        #endregion
    }

    /// <summary>
    /// Class Representing the Request Details
    /// </summary>
    public class RequestDetails : IDisposable
    {
        #region Fields
        private string strRequestID;
        private string strStatus;
        bool disposed;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of RequestDetails
        /// </summary>
        public RequestDetails()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pstrRequestID"></param>
        /// <param name="pstrNotifyURL"></param>
        public RequestDetails(string pstrRequestID, string pstrStatus)
        {
            this.strRequestID = pstrRequestID;
            this.strStatus = pstrStatus;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Request id
        /// </summary>
        public string RequestID
        {
            get
            {
                return this.strRequestID;
            }

            set
            {
                this.strRequestID = value;
            }
        }

        /// <summary>
        /// Request status
        /// </summary>
        public string Status
        {
            get
            {
                return this.strStatus;
            }

            set
            {
                this.strStatus = value;
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.strRequestID = string.Empty;
                    this.strStatus = string.Empty;
                }

                // Indicate that the instance has been disposed.
                this.disposed = true;
            }
        }
        #endregion
    }
}
