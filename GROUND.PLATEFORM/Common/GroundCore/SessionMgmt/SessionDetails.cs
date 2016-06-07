namespace GroundCore.SessionMgmt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Class represting the Session Details
    /// </summary>
    public class SessionDetails
    {
        #region Filelds
        string strSessionID;
        string strUserName;
        string strPassword;
        string strNotificationUrl;
        DateTime objLoginDateTime;
        DateTime objLastAccessedTime;
        List<RequestDetails> lstRequestDetails;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public SessionDetails()
        {
            lstRequestDetails = new List<RequestDetails>();
            strUserName = "";
            strPassword = "";
            strNotificationUrl = "";
            objLoginDateTime = DateTime.Now;
            objLastAccessedTime = DateTime.Now;
            strSessionID = "";
        }
        /// <summary>
        /// Constructure with all the parameter
        /// </summary>
        /// <param name="pstrUserName">Username</param>
        /// <param name="pstrPassword">password</param>
        /// <param name="pstrLoginUrl">LoginURL</param>
        /// <param name="pLoginDateTime">LoginTime</param>
        /// <param name="pLastAccessedTime">LastAccessDate</param>
        /// <param name="plstRequestDetails">List of Request by the user</param>
        public SessionDetails(string pstrUserName, string pstrPassword, string pstrLoginUrl,
            DateTime pLoginDateTime, DateTime pLastAccessedTime, List<RequestDetails> plstRequestDetails,string pstrSessionID)
        {
            lstRequestDetails = plstRequestDetails;
            strUserName = pstrUserName;
            strPassword = pstrPassword;
            strNotificationUrl = pstrLoginUrl;
            objLoginDateTime = pLoginDateTime;
            objLastAccessedTime = pLastAccessedTime;
            strSessionID=pstrSessionID;
        }
        #endregion

        #region Properties
        public List<RequestDetails> RequestDetails
        {
            get
            {
                return lstRequestDetails;
            }
            set
            {
                lstRequestDetails = value;
            }
        }
         public string SessionID
        {
            get
            {
                return strSessionID;
            }
            set
            {
                strSessionID = value;
            }
        }
        public string Password
        {
            get
            {
                return strPassword;
            }
            set
            {
                strPassword = value;
            }
        }
        public string UserName
        {
            get
            {
                return strUserName;
            }
            set
            {
                strUserName = value;
            }
        }
        public string NotificationUrl
        {
            get
            {
                return strNotificationUrl;
            }
            set
            {
                strNotificationUrl = value;
            }
        }
        public DateTime LoginDateTime
        {
            get
            {
                return objLoginDateTime;
            }
            set
            {
                objLoginDateTime = value;
            }
        }
        public DateTime LastAccessedTime
        {
            get
            {
                return objLastAccessedTime;
            }
            set
            {
                objLastAccessedTime = value;
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
                return lstRequestDetails[Index];
            }
            set
            {
                lstRequestDetails[Index] = value;
            }
        }
        /// <summary>
        /// Returns the Count of list items
        /// </summary>
        public int RequestDetailsCount
        {
            get
            {
                return lstRequestDetails.Count;
            }
        }
        // Methods

        /// <summary>
        /// Add an item to the Parameter Collection.
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        /// 
        public void Add(RequestDetails objValue)
        {
            if (lstRequestDetails.Count == 0)
            {
                lstRequestDetails.Capacity = 1;
            }
            else
            {
                lstRequestDetails.Capacity = lstRequestDetails.Count + 1;
            }
            lstRequestDetails.Add(objValue);
        }
        /// <summary>
        /// Removes all items from the Parameter Collection.
        /// </summary>
        public void Clear()
        {
            lstRequestDetails.Clear();
        }

        /// <summary>
        /// Determines whether the Parameter Collection contains a specific value.
        /// </summary>
        /// <param name="objValue">Object for which the search has to be made.</param>
        /// <returns>Boolean Value indicating whether Parameter Collection contains particular object.
        /// </returns>
        bool Contains(RequestDetails objValue)
        {
            return lstRequestDetails.Contains(objValue);
        }

        /// <summary>
        /// Determines the index of a specific item in the Parameter Collection.
        /// </summary>
        /// <param name="objValue">Object for which index has to be retreived.</param>
        /// <returns>Position of the object in the Parameter Collection.</returns>
        int IndexOf(RequestDetails objValue)
        {

            return lstRequestDetails.IndexOf(objValue);
        }

        /// <summary>
        /// Inserts an item to the Parameter Collection at the specified position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="objValue"></param>
        void Insert(int index, RequestDetails objValue)
        {
            lstRequestDetails.Insert(index, objValue);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the Parameter Collection.
        /// </summary>
        /// <param name="objValue">Object </param>
        void Remove(RequestDetails objValue)
        {
            lstRequestDetails.Remove(objValue);
        }

        /// <summary>
        /// Removes the Parmeter Collection item at the specified index.
        /// </summary>
        /// <param name="index"></param>
        void RemoveAt(int index)
        {
            lstRequestDetails.RemoveAt(index);

        }
        #endregion
   
    }
    /// <summary>
    /// Class Representing the Request Details
    /// </summary>
    public class RequestDetails
    {
        #region Fields
        string strRequestID;
        string strStatus;
        #endregion

        #region Constructor
        public RequestDetails()
        {
            strRequestID = "";
            strStatus = "";
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pstrRequestID"></param>
        /// <param name="pstrNotifyURL"></param>
        public RequestDetails(string pstrRequestID, string pstrStatus)
        {
            strRequestID=pstrRequestID;
            strStatus = pstrStatus;
        }
        #endregion

        #region Properties
        public string RequestID
        {
            get
            {
                return strRequestID;
            }
            set
            {
                strRequestID = value;
            }
        }
        public string Status
        {
            get
            {
                return strStatus;
            }
            set
            {
                strStatus = value;
            }
        }
        #endregion
    }
}
