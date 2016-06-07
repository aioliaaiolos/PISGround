//---------------------------------------------------------------------------------------------------
// <copyright file="SessionService.svc.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Text;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.LogMgmt;
    using PIS.Ground.Core.T2G;
    using PIS.Ground.Core.SessionMgmt;
    using System.Configuration;
    using PIS.Ground.Common;
    using PIS.Ground.Core.Utility;
    using PIS.Ground.Core.Common;
    using System.Threading;

    /// <summary>
    /// Session service
    /// </summary>    
    [CreateOnDispatchService(typeof(SessionService))]
    [ServiceBehavior(Namespace = "http://alstom.com/pacis/pis/ground/session/")]
    public class SessionService : ISessionService
    {
        #region static fields

        private static volatile bool _initialized = false;

        private static object _initializationLock = new object();

        private static IT2GManager _t2gManager = null;
        
        private static ISessionManager _sessionManager = null;

        private static INotificationSender _notificationSender = null;

        #endregion

        /// <summary>Initializes a new instance of the SessionService class.</summary>
        public SessionService()
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "SessionService";
            }

            Initialize();
        }

        public static void Initialize()
        {
            if (!_initialized)
            {
                lock (_initializationLock)
                {
                    if (!_initialized)
                    {
                        try
                        {
                            _sessionManager = new SessionManager();
                            
                            _sessionManager.StartMonitoringSessions();

                            _notificationSender = new NotificationSender(_sessionManager);

                            _t2gManager = T2GManagerContainer.T2GManager;

                            _t2gManager.SubscribeToT2GOnlineStatusNotification(
                                "PIS.Ground.Session.SessionService",
                                new EventHandler<T2GOnlineStatusNotificationArgs>(OnT2GOnlineOffline),
                                true);                            
                        }
                        catch (System.Exception e)
                        {
                            LogManager.WriteLog(TraceType.ERROR, e.Message, "PIS.Ground.Session.SessionService.Initialize", e, EventIdEnum.Session);
                        }

                        _initialized = true;
                    }
                }
            }
        }
        
        /// <summary>
        /// Notification received when a T2G is connecting / disconnecting.
        /// </summary>
        /// <param name="pSender">Sender Info.</param>
        /// <param name="pNotification">The notification data.</param>
        private static void OnT2GOnlineOffline(object pSender, T2GOnlineStatusNotificationArgs pNotification)
        {
            if (pNotification != null)
            {
                string message = "OnT2GOnlineOffline : " + pNotification.online.ToString();

                LogManager.WriteLog(TraceType.INFO, message, "PIS.Ground.Session.SessionService.OnT2GOnlineOffline", null, EventIdEnum.Session);

                _notificationSender.SendT2GServerConnectionStatus(pNotification.online);                                
            }
        }

        /// <summary>
        /// Login to service
        /// </summary>
        /// <param name="username">user name </param>
        /// <param name="password">user password</param>
        /// <returns>session id</returns>
        public Guid Login(string username, string password)
        {
            SessionManager objSessionMgr = new SessionManager();
            Guid objGuid;
            try
            {
                objSessionMgr.Login(username, password, out objGuid);
                return objGuid;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Session.SessionService.Login()", ex, EventIdEnum.Session);
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Logoff from service
        /// </summary>
        /// <param name="sessionId">valid session id</param>
        /// <returns>true if success else false</returns>
        public bool Logout(Guid sessionId)
        {
            SessionManager objSessionMgr = new SessionManager();
            try
            {
                string error = objSessionMgr.RemoveSessionID(sessionId);
                if (error == string.Empty)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Session.SessionService.Logout()", ex, EventIdEnum.Session);
                return false;
            }
        }

        /// <summary>
        /// set the notification url
        /// </summary>
        /// <param name="sessionId">session id</param>
        /// <param name="notificationURL">notificaion url</param>
        /// <returns>true if success else false</returns>
        public bool SetNotificationInformation(Guid sessionId, string notificationURL)
        {
            SessionManager objSessionMgr = new SessionManager();
            try
            {
                System.Net.HttpWebRequest lRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(notificationURL); 
                lRequest.Method = System.Net.WebRequestMethods.Http.Get; 
                System.Net.HttpWebResponse lResponse = (System.Net.HttpWebResponse)lRequest.GetResponse();
                if (lResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string error = objSessionMgr.SetNotificationURL(sessionId, notificationURL);
                    if (error == string.Empty)
                    {
                        return true;
                    }
                    else
                    {
                        LogManager.WriteLog(TraceType.ERROR, "Invalid Session Id", "PIS.Ground.Session.SessionService.SetNotificationInformation()", null, EventIdEnum.Session);
                        return false;
                    }
                }
                else
                {
                    LogManager.WriteLog(TraceType.ERROR, "Invalid Notification URL", "PIS.Ground.Session.SessionService.SetNotificationInformation()", null, EventIdEnum.Session);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Session.SessionService.SetNotificationInformation()", ex, EventIdEnum.Session);
                return false;
            }
        }

        /// <summary>
        /// Used to Validate Session
        /// </summary>
        /// <param name="sessionId">input session Id</param>
        /// <returns>if Valid return true else false</returns>
        public bool IsSessionValid(Guid sessionId)
        {
            SessionManager objSessionMgr = new SessionManager();
            try
            {
                return objSessionMgr.IsSessionValid(sessionId);                
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Session.SessionService.IsSessionValid()", ex, EventIdEnum.Session);
                return false;
            }
        }
    }
}
