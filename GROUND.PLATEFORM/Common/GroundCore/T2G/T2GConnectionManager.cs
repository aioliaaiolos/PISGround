//---------------------------------------------------------------------------------------------------
// <copyright file="T2GConnectionManager.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.T2G
{
    using System;
    using PIS.Ground.Core.LogMgmt;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.T2G.WebServices.Identification;
    using PIS.Ground.Core.Utility;
    using System.Timers;

    /// <summary>T2G server connection manager.</summary>
	internal class T2GConnectionManager : IT2GConnectionManager
    {
        /// <summary>Exception for signaling T2G connection errors.</summary>
        private class T2GLoginFailureException : Exception { }

        /// <summary>Information describing the session.</summary>
        private T2GSessionData _sessionData;

        /// <summary>The connection listener.</summary>
        private IT2GConnectionListener _connectionListener;

        /// <summary>T2G Ground server connection status. Null initially, when unknown.</summary>
		private bool? _connectionStatus = null;

        /// <summary>Timer to keep the T2G session alive.</summary>
        private Timer _timer;

        /// <summary>Session is to be kept alive for at most 1 minute without pings.</summary>
        private const int SessionKeepAliveMillis = 60000;

        /// <summary>Session keep-alive ping period of 30 sec.</summary>
        private const int SessionKeepAliveTimerPeriodMillis = 30000;

        /// <summary>Protocol version parameter to use when login to T2G.</summary>
		private const int T2GProtocolVersion = 7;

        /// <summary>Gets the T2G server connection status .</summary>
        /// <value>true if T2G server is online, false if not.</value>
        public bool T2GServerConnectionStatus
        {
            get
            {
                // if unknown, pretend it is offline
                return _connectionStatus ?? false;

            }
        }

        /// <summary>Initializes a new instance of the T2GConnectionManager class.</summary>
        /// <param name="sessionData">Information describing the session.</param>
        /// <param name="connectionListener">The connection listener.</param>
        public T2GConnectionManager(T2GSessionData sessionData, IT2GConnectionListener connectionListener)
        {
            if (sessionData == null)
            {
                throw new ArgumentNullException("sessionData");
            }

            if (connectionListener == null)
            {
                throw new ArgumentNullException("connectionListener");
            }

            _sessionData = sessionData;

            _connectionListener = connectionListener;

            // First, call the T2G session establishment routine right away once
            try
            {
                CheckSession(null, null);
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                LogManager.WriteLog(TraceType.DEBUG, "Check session aborded", "PIS.Ground.Core.T2G.T2GConnectionManager", ex, EventIdEnum.GroundCore);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.EXCEPTION, "Check session failed", "PIS.Ground.Core.T2G.T2GConnectionManager", ex, EventIdEnum.GroundCore);
            }

            // Second, schedule it to re-execute periodically            
            _timer = new Timer(SessionKeepAliveTimerPeriodMillis);
            _timer.Elapsed += this.CheckSession;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Timer routine to periodically check the T2G server connection: establish when not
        /// established, and monitor afterwards.
        /// </summary>
        /// <param name="source">State info that can be passed.</param>
        /// <param name="e">Elapsed event information.</param>
        private void CheckSession(Object source, ElapsedEventArgs e)
        {
            LogManager.WriteLog(TraceType.INFO, "CheckSession", "PIS.Ground.Core.T2G.T2GConnectionManager", null, EventIdEnum.GroundCore);

            try
            {
                if (_sessionData.SessionId == 0)
                {
                    _sessionData.SessionId = SessionInitialize();
                }
                else
                {
                    SessionKeepAlive();
                }

                UpdateConnectionStatus(true);
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                LogManager.WriteLog(TraceType.DEBUG, "CheckSession aborted", "PIS.Ground.Core.T2G.T2GConnectionManager", ex, EventIdEnum.GroundCore);
                _sessionData.SessionId = 0;
                // Do not call UpdateConnectionStatus. Processing shall stop.
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                LogManager.WriteLog(TraceType.ERROR, "CheckSession", "PIS.Ground.Core.T2G.T2GConnectionManager", ex, EventIdEnum.GroundCore);
                _sessionData.SessionId = 0;
                UpdateConnectionStatus(false);
            }
            catch (TimeoutException ex)
            {
                LogManager.WriteLog(TraceType.WARNING, "CheckSession", "PIS.Ground.Core.T2G.T2GConnectionManager", ex, EventIdEnum.GroundCore);
                _sessionData.SessionId = 0;
                UpdateConnectionStatus(false);
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                LogManager.WriteLog(TraceType.WARNING, "CheckSession", "PIS.Ground.Core.T2G.T2GConnectionManager", ex, EventIdEnum.GroundCore);
                _sessionData.SessionId = 0;
                UpdateConnectionStatus(false);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, "CheckSession", "PIS.Ground.Core.T2G.T2GConnectionManager", ex, EventIdEnum.GroundCore);
                _sessionData.SessionId = 0;
                UpdateConnectionStatus(false);
            }
        }

        /// <summary>Method is used to call the T2G Service to login</summary>
        /// <returns>T2G session ID</returns>
        private int SessionInitialize()
        {
            LogManager.WriteLog(TraceType.INFO, "SessionInitialize", "PIS.Ground.Core.T2G.T2GConnectionManager", null, EventIdEnum.GroundCore);

            int intSessionID = 0;
            
            applicationIdList objapplicationIdList = new applicationIdList();

            if (!string.IsNullOrEmpty(ServiceConfiguration.T2GApplicationId))
            {
                objapplicationIdList.Add(ServiceConfiguration.T2GApplicationId);
            }
            
            using (PIS.Ground.Core.T2G.WebServices.Identification.IdentificationPortTypeClient objIdentification = new IdentificationPortTypeClient())
            {
                int effectiveProtoc = 0;
                
                // Change protocol from version 1 to another to get deleted element notification.
                // Else, T2G will only send element changed, even on deletion.
                
                intSessionID = objIdentification.login(
                    ServiceConfiguration.T2GServiceUserName,
                    ServiceConfiguration.T2GServicePwd,
                    ServiceConfiguration.T2GServiceNotificationUrl,
                    objapplicationIdList,
                    T2GProtocolVersion, out effectiveProtoc);
            }

            if (intSessionID == 0)
            {
                throw new T2GLoginFailureException();
            }
            
            return intSessionID;                       
        }        

        /// <summary>Send a session-keep-alive message to T2G server.</summary>
        private void SessionKeepAlive()
        {
            LogManager.WriteLog(TraceType.INFO, "SessionKeepAlive", "PIS.Ground.Core.T2G.T2GConnectionManager", null, EventIdEnum.GroundCore);
            
            using (PIS.Ground.Core.T2G.WebServices.Identification.IdentificationPortTypeClient objIdentification = new IdentificationPortTypeClient())
            {
                objIdentification.keepAliveSession(_sessionData.SessionId, SessionKeepAliveMillis / 1000);                
            }
        }

        /// <summary>
        /// Updates internally the connection status described by connectionStatus.
        /// Notifies listener when connection status changes
        /// </summary>
        /// <param name="connectionStatus">T2G Ground server connection status.</param>
        private void UpdateConnectionStatus(bool connectionStatus)
        {
            if (_connectionStatus == null || _connectionStatus != connectionStatus)
            {
                _connectionStatus = connectionStatus;

                LogManager.WriteLog(
                    TraceType.INFO,
                    "T2G online=" + _connectionStatus.ToString(),
                    "PIS.Ground.Core.T2G.T2GConnectionManager.UpdateConnectionStatus",
                    null,
                    EventIdEnum.GroundCore);
                
                try
                {
                    _connectionListener.OnConnectionStatusChanged((bool)_connectionStatus);
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                    LogManager.WriteLog(
                        TraceType.DEBUG,
                        "Thread aborted",
                        "PIS.Ground.Core.T2G.T2GConnectionManager.UpdateConnectionStatus",
                        ex,
                        EventIdEnum.GroundCore);
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(
                        TraceType.EXCEPTION,
                        "T2G online=" + _connectionStatus.ToString(),
                        "PIS.Ground.Core.T2G.T2GConnectionManager.UpdateConnectionStatus",
                        ex,
                        EventIdEnum.GroundCore);
                }
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Enabled = false;
                    _timer.Dispose();
                }
            }
        }

        #endregion
    }
}
