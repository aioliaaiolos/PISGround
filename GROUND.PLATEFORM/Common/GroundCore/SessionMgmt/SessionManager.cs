/// 
namespace PIS.Ground.Core.SessionMgmt
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.LogMgmt;
    using PIS.Ground.Core.Properties;
    using PIS.Ground.Core.SQLite;
    using PIS.Ground.Core.Utility;
    using System.Timers;

    /// <summary>
    /// Manages the user session
    /// </summary>
    public class SessionManager : PIS.Ground.Core.SessionMgmt.ISessionManagerExtended
    {
        #region Constants
        /// <summary>
        /// Query to delete Session Data Table
        /// </summary>
        private const string DeleteSessionDataQuery = "Delete from SessionData where SessionID = '{0}'";

        /// <summary>
        /// Query to delete all Session Data Table
        /// </summary>
        private const string DeleteAllSessionDataQuery = "Delete from SessionData";
        
        /// <summary>
        /// Query to delete Session Table
        /// </summary>
        private const string DeleteSessionQuery = "Delete from Session where SessionID = '{0}'";

        /// <summary>
        /// Query to delete all Session Table
        /// </summary>
        private const string DeleteAllSessionQuery = "Delete from Session";
        
        /// <summary>
        /// Query to select NotificationURL by RequestID
        /// </summary>
        private const string SelectNotificationUrlQuery = "SELECT NotificationURL FROM Session INNER JOIN SessionData ON Session.SessionID = SessionData.SessionID AND SessionData.RequestID= '{0}'";

        /// <summary>
        /// Query to select all NotificationURLs
        /// </summary>
        private const string SelectNotificationUrls = "SELECT NotificationURL FROM Session";

        /// <summary>
        /// Query to select NotificationURL by session id
        /// </summary>
        private const string SelectNotificationUrlBySessionIdQuery = "SELECT NotificationURL FROM Session Where Session.SessionID = '{0}'";
        
        /// <summary>
        /// Query to select SessionID by Request id
        /// </summary>
        private const string SelectSessionIdByRequestIdQuery = "SELECT SessionID FROM SessionData Where SessionData.RequestID = '{0}'";
        
        /// <summary>
        /// Query to select SessionData by session id
        /// </summary>
        private const string SelectSessionBySessionIdQuery = "SELECT SessionID,UserName,Password,NotificationURL,LastAccessedTime,LoginTime FROM Session Where Session.SessionID = '{0}'";
        
        /// <summary>
        /// Query to select SessionID,RequestID,Status by session id
        /// </summary>
        private const string SelectSessionDataBySessionIdQuery = "SELECT SessionID,RequestID,Status FROM SessionData Where SessionData.SessionID = '{0}'";
        
        /// <summary>
        /// Query to select from Session.
        /// </summary>
        private const string SelectSessionQuery = "SELECT SessionID,LastAccessedTime,UserTimeOutSet,UserTimeOut FROM Session";

        /// <summary>
        /// Sesion Id field
        /// </summary>
        private const string SessionIdField = "SessionID";

        /// <summary>
        /// Session Table
        /// </summary>
        private const string SessionTable = "Session";

        /// <summary>
        /// SessionData Table
        /// </summary>
        private const string SessionDataTable = "SessionData";

        /// <summary>
        /// UserName field
        /// </summary>
        private const string UserNameField = "UserName";

        /// <summary>
        /// Password field
        /// </summary>
        private const string PasswordField = "Password";

        /// <summary>
        /// Notification URL field
        /// </summary>
        private const string NotificationUrlField = "NotificationURL";

        /// <summary>
        /// RequestID field
        /// </summary>
        private const string RequestIdField = "RequestID";

        /// <summary>
        /// Status field
        /// </summary>
        private const string StatusField = "Status";

        /// <summary>
        /// Sesion LastAccessedTime field
        /// </summary>
        private const string LastAccessTimeField = "LastAccessedTime";

        /// <summary>
        /// User TimeOut set field
        /// </summary>
        private const string UserTimeOut = "UserTimeOut";

        /// <summary>
        /// User Time out Set
        /// </summary>
        private const string UserTimeOutSet = "UserTimeOutSet";

        /// <summary>
        /// Sesion LoginTime field
        /// </summary>
        private const string LoginTimeField = "LoginTime";

        /// <summary>
        /// Sesion Id field used for query
        /// </summary>
        private const string SessionAssign = "SessionID = '{0}'";

        /// <summary>
        /// Request Id field used for query
        /// </summary>
        private const string SessionDataAssign = "RequestID = '{0}'";

        /// <summary>
        /// Sesion status InProgress
        /// </summary>
        private const string InProgressStatus = "InProgress";

        /// <summary>
        /// Sesion status Finished
        /// </summary>
        private const string FinishedStatus = "Finished";

        /// <summary>
        /// Sesion status Cancelled
        /// </summary>
        private const string CancelledStatus = "Cancelled";

        /// <summary>
        /// lock for synchronisation
        /// </summary>
        private static readonly object locker = new object();

        /// <summary>
        /// timer to clean unused session
        /// </summary>
        private Timer timer;

        #endregion

        /// <summary>
        /// Initializes a new instance of the SessionManager class.
        /// </summary>
        public SessionManager()
        {
        }

        /// <summary>
        /// Used to add the user into session
        /// </summary>
        /// <param name="username">Username of the LoginUser</param>
        /// <param name="password">Password of the User</param>
        /// <param name="objGuid"> Session id</param>
        /// <returns>Error if any</returns>
        public string Login(string username, string password, out Guid objGuid)
        {
            string error = string.Empty;
            objGuid = Guid.Empty; 
            Dictionary<string, string> dicParamter = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                objGuid = Guid.NewGuid();
                dicParamter.Add(UserNameField, username);
                dicParamter.Add(PasswordField, password);
                dicParamter.Add(SessionIdField, objGuid.ToString());
                dicParamter.Add(NotificationUrlField, string.Empty);
                dicParamter.Add(LastAccessTimeField, System.DateTime.Now.ToString(CultureInfo.InvariantCulture));
                dicParamter.Add(LoginTimeField, System.DateTime.Now.ToString(CultureInfo.InvariantCulture));
                try
                {
                    using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                    {
                        lock (locker)
                        {
                            objSqlWpr.mInsert(SessionTable, dicParamter);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExInsertLogin, username, ex.Message), "PIS.Ground.Core.SessionMgmt.SessionManager.Login", ex, EventIdEnum.GroundCore);
                    error = string.Format(CultureInfo.CurrentCulture, Resources.ExInsertLogin, username, ex.Message);
                    objGuid = Guid.Empty;
                }
            }
            else
            {
                LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExInsertLoginValidation), "PIS.Ground.Core.SessionMgmt.SessionManager.Login", null, EventIdEnum.GroundCore);
                error = string.Format(CultureInfo.CurrentCulture, Resources.ExInsertLoginValidation);
                objGuid = Guid.Empty;
            }

            return error;
        }

        /// <summary>
        /// Check for the session is alive
        /// </summary>
        /// <param name="sessionId">input session id</param>
        /// <returns>true if alive else dead</returns>
        public bool IsSessionValid(Guid sessionId)
        {
            return this.ValidateSession(sessionId);
        }

        /// <summary>
        /// Used to Update the NotificationURL for the given SessionID
        /// </summary>
        /// <param name="sessionId">SessionID for which the NotificationURL must be added</param>
        /// <param name="strNotificationURL">NotificationURL that needs to be added</param>
        /// <returns>Error if any</returns>
        public string SetNotificationURL(Guid sessionId, string strNotificationURL)
        {
            string error = string.Empty;
            if (string.IsNullOrEmpty(strNotificationURL))
            {
                LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExInsertNotificationUrlValidation), "PIS.Ground.Core.SessionMgmt.SessionManager.SetNotificationURL", null, EventIdEnum.GroundCore);
                error = string.Format(CultureInfo.CurrentCulture, Resources.ExInsertNotificationUrlValidation);
            }

            if (this.ValidateSession(sessionId))
            {
                try
                {
                    using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                    {
                        Dictionary<string, string> dicParameter = new Dictionary<string, string>();
                        string strUpdateWhereClause = string.Format(CultureInfo.InvariantCulture, SessionAssign, sessionId.ToString());
                        dicParameter.Add(LastAccessTimeField, System.DateTime.Now.ToString(CultureInfo.InvariantCulture));
                        dicParameter.Add(NotificationUrlField, strNotificationURL);
                        lock (locker)
                        {
                            objSqlWpr.mUpdate(SessionTable, dicParameter, strUpdateWhereClause);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExInsertNotificationURL, sessionId, ex.Message), "PIS.Ground.Core.SessionMgmt.SessionManager.SetNotificationURL", ex, EventIdEnum.GroundCore);
                    error = string.Format(CultureInfo.CurrentCulture, Resources.ExInsertNotificationURL, sessionId, ex.Message);
                }
            }
            else
            {
                error = string.Format(CultureInfo.CurrentCulture, Resources.ErrorSessionNotFound, sessionId);
            }

            return error;
        }

        /// <summary>
        /// Removes all session.
        /// </summary>
        /// <returns>The error message. Empty string on success.</returns>
        public string RemoveAllSessions()
        {
            string error = string.Empty;
            List<string> lstQuery = new List<string>(2);
            lstQuery.Add(DeleteAllSessionDataQuery);
            lstQuery.Add(DeleteAllSessionQuery);
            try
            {
                using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                {
                    lock (locker)
                    {
                        objSqlWpr.mExecuteTransactionQuery(lstQuery);
                    }
                }
            }
            catch (Exception ex)
            {
                error = string.Format(CultureInfo.CurrentCulture, Resources.ExRemoveAllSession,ex.Message);
                LogManager.WriteLog(TraceType.ERROR, error , "PIS.Ground.Core.SessionMgmt.SessionManager.RemoveAllSessions", ex, EventIdEnum.GroundCore);
            }

            return error;
        }

        /// <summary>
        /// Used to Remove the Session Details for the given SessionID
        /// </summary>
        /// <param name="sessionId">SessionID for which the Session must be removed</param>
        /// <returns>Error if any</returns>
        public string RemoveSessionID(Guid sessionId)
        {
            string error = string.Empty;
            if (this.ValidateSession(sessionId))
            {
                List<string> lstQuery = new List<string>();
                string strDeleteSessionData = string.Format(CultureInfo.InvariantCulture, DeleteSessionDataQuery, sessionId);
                string strDeleteSession = string.Format(CultureInfo.InvariantCulture, DeleteSessionQuery, sessionId);
                lstQuery.Add(strDeleteSessionData);
                lstQuery.Add(strDeleteSession);
                try
                {
                    using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                    {
                        lock (locker)
                        {
                            objSqlWpr.mExecuteTransactionQuery(lstQuery);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExRemoveSession, sessionId, ex.Message), "PIS.Ground.Core.SessionMgmt.SessionManager.RemoveSessionID", ex, EventIdEnum.GroundCore);
                    error = string.Format(CultureInfo.CurrentCulture, Resources.ExRemoveSession, sessionId, ex.Message);
                }
            }
            else
            {
                error = string.Format(CultureInfo.CurrentCulture, Resources.ErrorSessionNotFound, sessionId);
            }

            return error;
        }

        /// <summary>
        /// Selects the Notification URL of SessionID
        /// </summary>
        /// <param name="notificationUrl">Notification URLs for all sessionID</param>
        /// <returns>Error if any</returns>
        public string GetNotificationUrls(List<string> notificationUrls)
        {
			if (notificationUrls == null)
			{
				throw new ArgumentNullException("notificationUrls");
			}

            string error = string.Empty;
			notificationUrls.Clear();

            try
            {
                using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                using (DataTable tableResult = new DataTable())
                {
                    tableResult.Locale = CultureInfo.InvariantCulture;
                    objSqlWpr.mExecuteQuery(SelectNotificationUrls, tableResult);
                    if (tableResult.Rows != null && tableResult.Columns != null && tableResult.Columns.Contains(NotificationUrlField) && tableResult.Rows.Count > 0)
                    {
                        for (int i = 0; i < tableResult.Rows.Count; i++)
                        {
                            notificationUrls.Add(tableResult.Rows[i][NotificationUrlField].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, "", "PIS.Ground.Core.SessionMgmt.SessionManager.GetNotificationURLs", ex, EventIdEnum.GroundCore);
                error = string.Format(CultureInfo.CurrentCulture, Resources.ExSelectNotificationUrls, ex.Message);
            }

            return error;
        }

        /// <summary>
        /// Selects the Notification URL of SessionID
        /// </summary>
        /// <param name="sessionId">Input SessionID</param>
        /// <param name="notificationUrl">Notification URL of the SessionID</param>
        /// <returns>Error if any</returns>
        public string GetNotificationUrlBySessionId(Guid sessionId, out string notificationUrl)
        {
            string error = string.Empty;
            notificationUrl = string.Empty;
            if (this.ValidateSession(sessionId))
            {
                string strQuery = string.Format(CultureInfo.InvariantCulture, SelectNotificationUrlBySessionIdQuery, sessionId.ToString());
                try
                {
                    using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                    using (DataTable tableResult = new DataTable())
                    {
                        tableResult.Locale = CultureInfo.InvariantCulture;
                        objSqlWpr.mExecuteQuery(strQuery, tableResult);
                        if (tableResult.Rows != null && tableResult.Columns != null && tableResult.Columns.Contains(NotificationUrlField) && tableResult.Rows.Count > 0)
                        {
                            notificationUrl = tableResult.Rows[0][NotificationUrlField].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExSelectNotificationUrlBySessionID, sessionId, ex.Message), "PIS.Ground.Core.SessionMgmt.SessionManager.GetNotificationURL", ex, EventIdEnum.GroundCore);
                    error = string.Format(CultureInfo.CurrentCulture, Resources.ExSelectNotificationUrlBySessionID, sessionId, ex.Message);
                }
            }
            else
            {
                error = string.Format(CultureInfo.CurrentCulture, Resources.ErrorSessionNotFound, sessionId);
            }

            return error;
        }

        /// <summary>
        /// Selects the Notification URL of RequestId
        /// </summary>
        /// <param name="requestId">Input RequestId</param>
        /// <param name="notificationUrl">output notification url</param>
        /// <returns>Notification URL of the Request id</returns>
        public string GetNotificationUrlByRequestId(Guid requestId, out string notificationUrl)
        {
            string error = string.Empty;
            notificationUrl = string.Empty;
            string strQuery = string.Format(CultureInfo.InvariantCulture, SelectNotificationUrlQuery, requestId);
            try
            {
                using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                using (DataTable tableResult = new DataTable())
                {
                    tableResult.Locale = CultureInfo.InvariantCulture;
                    objSqlWpr.mExecuteQuery(strQuery, tableResult);
                    if (tableResult.Rows != null && tableResult.Columns != null && tableResult.Columns.Contains(NotificationUrlField) && tableResult.Rows.Count > 0)
                    {
                        notificationUrl = tableResult.Rows[0][NotificationUrlField].ToString();
                    }
                    else
                    {
                        LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExRequestIDNotFound, requestId), "PIS.Ground.Core.SessionMgmt.SessionManager.GetNotificationURL", null, EventIdEnum.GroundCore);
                        error = string.Format(CultureInfo.CurrentCulture, Resources.ExRequestIDNotFound, requestId);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExSelectNotificationUrl, requestId, ex.Message), "PIS.Ground.Core.SessionMgmt.SessionManager.GetNotificationURL", ex, EventIdEnum.GroundCore);
                error = string.Format(CultureInfo.CurrentCulture, Resources.ExSelectNotificationUrl, requestId, ex.Message);
            }

            return error;
        }

		/// <summary>Gets session identifier corresponding to the specified request identifier.</summary>
		/// <param name="requestId">input: Request ID.</param>
		/// <param name="sessionId">output: Session ID.</param>
		/// <returns>error if any.</returns>
		public string GetSessionIdByRequestId(Guid requestId, out Guid sessionId)
		{
			// Setting the result variables for the worst case scenario
			string error = string.Format(CultureInfo.CurrentCulture, Resources.ExInternalError,
				"Method GetSessionIdByRequestId()");

			sessionId = Guid.Empty;

			try
			{
				// Looking into the database for the specified request id
				string strQuery = string.Format(CultureInfo.InvariantCulture, SelectSessionIdByRequestIdQuery, requestId);
				using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                using (DataTable tableResult = new DataTable())
                {
                    tableResult.Locale = CultureInfo.CurrentCulture;
                    objSqlWpr.mExecuteQuery(strQuery, tableResult);

                    // If the database contains what we are looking for...
                    if (tableResult.Rows != null && tableResult.Columns != null &&
                        tableResult.Columns.Contains(SessionIdField) && tableResult.Rows.Count > 0)
                    {
                        // ... store the result to the output argument
                        sessionId = new Guid(tableResult.Rows[0][SessionIdField].ToString());
                        error = string.Empty;
                    }
                    else
                    {
                        error = string.Format(CultureInfo.CurrentCulture, Resources.ExRequestIDNotFound, requestId);

                        // ... otherwise, log and return an error
                        LogManager.WriteLog(TraceType.ERROR, error,
                            "PIS.Ground.Core.SessionMgmt.SessionManager.GetSessionIdByRequestId",
                            null, EventIdEnum.GroundCore);
                    }
                }
			}
			catch (Exception ex)
			{
				error = string.Format(CultureInfo.CurrentCulture, Resources.ExSelectNotificationUrl,
					requestId, ex.Message);

				LogManager.WriteLog(TraceType.ERROR, error,
					"PIS.Ground.Core.SessionMgmt.SessionManager.GetSessionIdByRequestId", 
					ex, EventIdEnum.GroundCore);
			}

			return error;
		}

        /// <summary>
        /// Extend the session time out for the given session
        /// </summary>
        /// <param name="sessionId"> input session id</param>
        /// <param name="timeOut">value in minutes; max 60 minutes</param>
        /// <returns>Error if any else empty string</returns>
        public string KeepSessionAlive(Guid sessionId, int timeOut)
        {
            string error = string.Empty;
            if (timeOut > 60 || timeOut < 0)
            {
                LogManager.WriteLog(TraceType.ERROR, Resources.LogUserTimeOutValueInvalid, "PIS.Ground.Core.SessionMgmt.SessionManager.KeepSessionAlive", null, EventIdEnum.GroundCore);
                error = Resources.LogUserTimeOutValueInvalid;
                return error;
            }

            if (this.ValidateSession(sessionId))
            {
                try
                {
                    using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                    {
                        Dictionary<string, string> dicParameter = new Dictionary<string, string>();
                        string strUpdateWhereClause = string.Format(CultureInfo.InvariantCulture, SessionAssign, sessionId.ToString());
                        dicParameter.Add(UserTimeOutSet, System.DateTime.Now.ToString(CultureInfo.InvariantCulture));
                        dicParameter.Add(UserTimeOut, timeOut.ToString());
                        lock (locker)
                        {
                            objSqlWpr.mUpdate(SessionTable, dicParameter, strUpdateWhereClause);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExInsertNotificationURL, sessionId, ex.Message), "PIS.Ground.Core.SessionMgmt.SessionManager.SetNotificationURL", ex, EventIdEnum.GroundCore);
                    error = string.Format(CultureInfo.CurrentCulture, Resources.ExInsertNotificationURL, sessionId, ex.Message);
                }
            }
            else
            {
                error = string.Format(CultureInfo.CurrentCulture, Resources.ErrorSessionNotFound, sessionId);
            }

            return error;
        }

        /// <summary>
        /// Get Session Details of the SessionId
        /// </summary>
        /// <param name="sessionId">Input sessionId</param>
        /// <param name="objSessionDetails"> output session details</param>
        /// <returns>True if Session exists else False</returns>
        public string GetSessionDetails(Guid sessionId, out SessionData objSessionDetails)
        {
            string error = string.Empty;
            objSessionDetails = null;
            if (this.ValidateSession(sessionId))
            {
                string strQuery = string.Format(CultureInfo.InvariantCulture, SelectSessionBySessionIdQuery, sessionId.ToString());
                bool executeNextStep = false;
                try
                {
                    objSessionDetails = new SessionData();
                    using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                    using (DataTable tableResult = new DataTable())
                    {
                        tableResult.Locale = CultureInfo.InvariantCulture;
                        objSqlWpr.mExecuteQuery(strQuery, tableResult);
                        if (tableResult.Rows != null && tableResult.Columns != null && tableResult.Rows.Count > 0)
                        {
                            executeNextStep = true;
                            DataRow rowSession = tableResult.Rows[0];
                            if (tableResult.Columns.Contains(SessionIdField) && rowSession[SessionIdField] != null)
                            {
                                objSessionDetails.SessionID = rowSession[SessionIdField].ToString();
                            }

                            if (tableResult.Columns.Contains(UserNameField) && rowSession[UserNameField] != null)
                            {
                                objSessionDetails.UserName = rowSession[UserNameField].ToString();
                            }

                            if (tableResult.Columns.Contains(PasswordField) && rowSession[PasswordField] != null)
                            {
                                objSessionDetails.Password = rowSession[PasswordField].ToString();
                            }

                            if (tableResult.Columns.Contains(NotificationUrlField) && rowSession[NotificationUrlField] != null)
                            {
                                objSessionDetails.NotificationUrl = rowSession[NotificationUrlField].ToString();
                            }

                            if (tableResult.Columns.Contains(LastAccessTimeField) && rowSession[LastAccessTimeField] != null)
                            {
                                objSessionDetails.LastAccessedTime = DateTime.Parse(rowSession[LastAccessTimeField].ToString(), CultureInfo.InvariantCulture);
                            }

                            if (tableResult.Columns.Contains(LoginTimeField) && rowSession[LoginTimeField] != null)
                            {
                                objSessionDetails.LoginDateTime = DateTime.Parse(rowSession[LoginTimeField].ToString(), CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExSelectSessionDetails, sessionId, ex.Message), "PIS.Ground.Core.SessionMgmt.SessionManager.GetSessionDetails", ex, EventIdEnum.GroundCore);
                    error = string.Format(CultureInfo.CurrentCulture, Resources.ExSelectSessionDetails, sessionId, ex.Message);
                    objSessionDetails = null;
                    return error;
                }

                try
                {
                    if (executeNextStep)
                    {
                        using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                        using (DataTable tableResult = new DataTable())
                        {
                            strQuery = string.Format(CultureInfo.InvariantCulture, SelectSessionDataBySessionIdQuery, sessionId.ToString());
                            tableResult.Locale = CultureInfo.InvariantCulture;
                            objSqlWpr.mExecuteQuery(strQuery, tableResult);
                            if (tableResult.Rows != null && tableResult.Columns != null && tableResult.Rows.Count > 0)
                            {
                                foreach (DataRow rowRequest in tableResult.Rows)
                                {
                                    RequestDetails objRequestDetails = new RequestDetails();
                                    if (rowRequest[RequestIdField] != null)
                                    {
                                        objRequestDetails.RequestID = rowRequest[RequestIdField].ToString();
                                    }

                                    if (rowRequest[StatusField] != null)
                                    {
                                        objRequestDetails.Status = rowRequest[StatusField].ToString();
                                    }

                                    objSessionDetails.Add(objRequestDetails);
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            else
            {
                error = string.Format(CultureInfo.CurrentCulture, Resources.ErrorSessionNotFound, sessionId);
                objSessionDetails = null;
                return error;
            }

            return error;
        }

        /// <summary>
        /// Insert a new RequestId to an already existing SessionId
        /// </summary>
        /// <param name="sessionId">Input sessionId</param>
        /// <param name="objGuid">Request id or Guid.empty if an error occurs</param>
        /// <returns>Error if any</returns>
        public string GenerateRequestID(Guid sessionId, out Guid objGuid)
        {
            string error = string.Empty;
            objGuid = Guid.Empty;
            if (this.ValidateSession(sessionId))
            {
                error = AddRequestID(sessionId, out objGuid);
            }
            else
            {
                error = string.Format(CultureInfo.CurrentCulture, Resources.ErrorSessionNotFound, sessionId);
                return error;
            }

            return error;
        }

        /// <summary>
        /// Insert a new RequestId not associated with a SessionID.
        /// </summary>
        /// <param name="objGuid">Request id or Guid.empty if an error occurs</param>
        /// <returns>Error if any</returns>
        public string GenerateRequestID(out Guid objGuid)
        {
            return AddRequestID(Guid.Empty, out objGuid);
        }

        /// <summary>
        /// Update the status of the RequestID
        /// </summary>
        /// <param name="strRequestId">Input RequestId</param>
        /// <param name="status">Status to be updated</param>
        /// <returns>Error if any</returns>
        public string UpdateRequestIdStatus(string strRequestId, RequestStatus status)
        {
            string error = string.Empty;
            Guid tst = new Guid(strRequestId);
            if (tst != Guid.Empty)
            {
                try
                {
                    Dictionary<string, string> dicParamter = new Dictionary<string, string>();
                    dicParamter.Add(RequestIdField, strRequestId);
                    switch (status)
                    {
                        case RequestStatus.InProgress: dicParamter.Add(StatusField, InProgressStatus);
                            break;
                        case RequestStatus.Finished: dicParamter.Add(StatusField, FinishedStatus);
                            break;
                        case RequestStatus.Canceled: dicParamter.Add(StatusField, CancelledStatus);
                            break;
                        default: dicParamter.Add(StatusField, InProgressStatus);
                            break;
                    }

                    string strUpdateWhereClause = string.Format(CultureInfo.InvariantCulture, SessionDataAssign, strRequestId);
                    lock (locker)
                    {
                        using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                        {
                            objSqlWpr.mUpdate(SessionDataTable, dicParamter, strUpdateWhereClause);

                            string strQuery = string.Format(CultureInfo.InvariantCulture, SelectSessionIdByRequestIdQuery, strRequestId);
                            string strSessionID = string.Empty;
                            using (DataTable tableResult = new DataTable())
                            {
                                tableResult.Locale = CultureInfo.InvariantCulture;
                                objSqlWpr.mExecuteQuery(strQuery, tableResult);
                                if (tableResult.Rows != null && tableResult.Columns != null && tableResult.Columns.Contains(NotificationUrlField) && tableResult.Rows.Count > 0)
                                {
                                    strSessionID = tableResult.Rows[0][NotificationUrlField].ToString();
                                }

                                if (!string.IsNullOrEmpty(strSessionID))
                                {
                                    dicParamter.Clear();
                                    strUpdateWhereClause = string.Empty;
                                    strUpdateWhereClause = string.Format(CultureInfo.InvariantCulture, SessionAssign, strSessionID);
                                    dicParamter.Add(LastAccessTimeField, System.DateTime.Now.ToString(CultureInfo.InvariantCulture));
                                    objSqlWpr.mUpdate(SessionTable, dicParamter, strUpdateWhereClause);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExRequestID_StatusUpdate, strRequestId, ex.Message), "PIS.Ground.Core.SessionMgmt.SessionManager.UpdateRequestID_Status", ex, EventIdEnum.GroundCore);
                    error = string.Format(CultureInfo.CurrentCulture, Resources.ExRequestID_StatusUpdate, strRequestId, ex.Message);
                }
            }
            else
            {
                error = "Invalid request id.";
            }
            return error;
        }

        /// <summary>Starts monitoring sessions.</summary>
        public void StartMonitoringSessions()
        {
            SetSessionCheckTimer();
        }
            
        /// <summary>
        /// Set the Session Timer to check the unused session
        /// </summary>
        internal void SetSessionCheckTimer()
        {
            if (timer != null)
            {
                timer.Enabled = false;
                timer.Dispose();
            }

            timer = new Timer(ServiceConfiguration.SessionTimerCheck);
            timer.Elapsed += this.CheckSessions;
            timer.Enabled = true;
        }

        /// <summary>
        /// Validate the presence of sessionId
        /// </summary>
        /// <param name="sessionId">Input sessionId</param>
        /// <returns>True if Session exists else False</returns>
        internal bool ValidateSession(Guid sessionId)
        {
            Dictionary<string, string> dicParameter = new Dictionary<string, string>();
            dicParameter.Add(SessionIdField, sessionId.ToString());
            try
            {
                using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                {
                    return objSqlWpr.mEntryExists(SessionTable, dicParameter);
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExValidateSessionDetails, sessionId, ex.Message), "PIS.Ground.Core.SessionMgmt.SessionManager.ValidateSession", ex, EventIdEnum.GroundCore);
                return false;
            }
        }

        /// <summary>
        /// check the session and Remove the unused session
        /// </summary>
        /// <param name="stateInfo">user state</param>
        private void CheckSessions(Object source, ElapsedEventArgs e)
        {
			LogManager.WriteLog(TraceType.DEBUG, "CheckSessions", "PIS.Ground.Core.SessionMgmt.SessionManager", null, EventIdEnum.GroundCore);
			
			try
            {
                using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                using (DataTable tableResult = new DataTable())
                {
                    tableResult.Locale = CultureInfo.InvariantCulture;
                    objSqlWpr.mExecuteQuery(SelectSessionQuery, tableResult);
                    if (tableResult.Rows != null && tableResult.Columns != null && tableResult.Rows.Count > 0)
                    {
                        foreach (DataRow rowSession in tableResult.Rows)
                        {
                            if (rowSession[SessionIdField] != null)
                            {
                                string strSessionID = rowSession[SessionIdField].ToString();

                                DateTime lastAccess;
                                DateTime userTimeOutSet;
                                int userTimeOutValue = 0;
                                bool userTimeSet;
                                if (rowSession[LastAccessTimeField] != null)
                                {
                                    DateTime.TryParse(rowSession[LastAccessTimeField].ToString(), out lastAccess);
                                    if (rowSession[UserTimeOut] != null && rowSession[UserTimeOutSet] != null)
                                    {
                                        DateTime.TryParse(rowSession[UserTimeOutSet].ToString(), out userTimeOutSet);
                                        int.TryParse(rowSession[UserTimeOutSet].ToString(), out userTimeOutValue);
                                        if (userTimeOutValue > 0)
                                        {
                                            if (DateTime.Now.Subtract(userTimeOutSet).TotalMinutes > userTimeOutValue)
                                            {
                                                userTimeSet = true;
                                            }
                                            else
                                            {
                                                userTimeSet = false;
                                            }
                                        }
                                        else
                                        {
                                            userTimeSet = true;
                                        }
                                    }
                                    else
                                    {
                                        userTimeSet = true;
                                    }

                                    if (DateTime.Now.Subtract(lastAccess).TotalMinutes > ServiceConfiguration.SessionTimeOut && userTimeSet)
                                    {
                                        this.RemoveSessionID(new Guid(strSessionID));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // No logic to apply.
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.ExSessionTimerCheckError, ex.Message), "PIS.Ground.Core.SessionMgmt.SessionManager.CheckSessions", ex, EventIdEnum.GroundCore);
            }            
        }

        /// <summary>
        /// Insert a new RequestId with the specified SessionId.
        /// The SessionId might already exist in the database or be a new one.
        /// </summary>
        /// <param name="sessionId">Input sessionId</param>
        /// <param name="objGuid">Request id or Guid.empty if an error occurs</param>
        /// <returns>Error if any</returns>
        private string AddRequestID(Guid sessionId, out Guid objGuid)
        {
			// Setting the result variables for the worst case scenario
			string error = string.Format(CultureInfo.CurrentCulture, Resources.ExInternalError,
				"Method AddRequestID()");

            objGuid = Guid.NewGuid();

			// Prepare the dictionary structure to be used for the database update
            Dictionary<string, string> dicParamter = new Dictionary<string, string>();
            dicParamter.Add(RequestIdField, objGuid.ToString());
            dicParamter.Add(SessionIdField, sessionId.ToString());
            dicParamter.Add(StatusField, InProgressStatus);
            try
            {
                using (SQLiteWrapperClass objSqlWpr = new SQLiteWrapperClass(ServiceConfiguration.SessionSqLiteDBPath))
                {
                    lock (locker)
                    {
						// Insert a new request to the database request table
                        objSqlWpr.mInsert(SessionDataTable, dicParamter);

						// If there is a session id...
						if (sessionId != Guid.Empty)
						{
							// ...update also the session table
							dicParamter.Clear();
							string strUpdateWhereClause = string.Format(CultureInfo.InvariantCulture, SessionAssign, sessionId.ToString());
							dicParamter.Add(LastAccessTimeField, System.DateTime.Now.ToString(CultureInfo.InvariantCulture));
							objSqlWpr.mUpdate(SessionTable, dicParamter, strUpdateWhereClause);
						}
						error = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
				error = string.Format(CultureInfo.CurrentCulture, Resources.ExRequestIDInsert,
					sessionId, ex.Message);

                LogManager.WriteLog(TraceType.ERROR, error,
					"PIS.Ground.Core.SessionMgmt.SessionManager.AddRequestID", 
					ex, EventIdEnum.GroundCore);
            }

            return error;
        }
    }
}
