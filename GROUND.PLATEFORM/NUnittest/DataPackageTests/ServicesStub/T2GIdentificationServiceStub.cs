//---------------------------------------------------------------------------------------------------
// <copyright file="T2GIdentificationServiceStub.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using DataPackageTests.T2GServiceInterface.Identification;
using IdentificationPortType = DataPackageTests.T2GServiceInterface.Identification.IdentificationPortType;
using ApplicationIdList = DataPackageTests.T2GServiceInterface.Identification.applicationIdList;
using SystemInfoStruct = DataPackageTests.T2GServiceInterface.Identification.systemInfoStruct;
using CommLinkEnum = DataPackageTests.T2GServiceInterface.Identification.commLinkEnum;
using NotificationClient = DataPackageTests.T2GServiceInterface.Notification.NotificationPortTypeClient;


namespace DataPackageTests.T2GServiceInterface.Identification
{
    public partial class systemInfoStruct
    {
        public string IPAddress { get; set; }
        public systemInfoStruct Clone()
        {
            return (systemInfoStruct)this.MemberwiseClone();
        }

        public DataPackageTests.T2GServiceInterface.Notification.systemInfoStruct ConvertToNotification()
        {
            DataPackageTests.T2GServiceInterface.Notification.systemInfoStruct newObject = new DataPackageTests.T2GServiceInterface.Notification.systemInfoStruct();
            newObject.communicationLink = (DataPackageTests.T2GServiceInterface.Notification.commLinkEnum)communicationLink;
            newObject.isOnline = isOnline;
            newObject.missionId = missionId;
            newObject.status = status;
            newObject.systemId = systemId;
            newObject.vehiclePhysicalId = vehiclePhysicalId;
            return newObject;
        }
    }

}
namespace DataPackageTests.ServicesStub
{
    /// <summary>
    /// Class that simulate the T2G services
    /// </summary>
    /// <seealso cref="DataPackageTests.T2GServiceInterface.Identification.IdentificationPortType" />
    [ServiceBehaviorAttribute(InstanceContextMode = InstanceContextMode.Single, ConfigurationName = "DataPackageTests.T2GServiceInterface.Identification.IdentificationPortType")]
    class T2GIdentificationServiceStub : IdentificationPortType
    {
        /// <summary>
        /// Class specialized to hold data on session
        /// </summary>
        private class SessionData
        {
            public string UserName {get;set;}
            public string NotificationUrl { get; set;}
            public int ProtocolVersion { get; set; }
            public bool NotificationEnabled { get; set;}
            public ApplicationIdList ApplicationIds { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="SessionData"/> class.
            /// </summary>
            /// <param name="userName">Name of the user.</param>
            /// <param name="protocolVersion">The protocol version.</param>
            public SessionData(string userName, int protocolVersion) : this(userName, null, protocolVersion, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SessionData"/> class.
            /// </summary>
            /// <param name="userName">Name of the user.</param>
            /// <param name="notificationUrl">The notification URL.</param>
            /// <param name="protocolVersion">The protocol version.</param>
            /// <param name="applicationIds">The application ids.</param>
            public SessionData(string userName, string notificationUrl, int protocolVersion, ApplicationIdList applicationIds)
            {
                UserName = userName ?? string.Empty;
                NotificationUrl = notificationUrl ?? string.Empty;
                ProtocolVersion = protocolVersion;
                NotificationEnabled = false;
                ApplicationIds = applicationIds ?? new ApplicationIdList();
            }
        }

        #region Fields

        private object _sessionLock = new object();
        private int _nextSessionId = 1;
        private Dictionary<int, SessionData> _sessions = new Dictionary<int, SessionData>(10);
        public const int CurrentProtocolVersion = 100;

        private object _systemInfoLock = new object();
        private Dictionary<string, SystemInfoStruct> _systems = new Dictionary<string, SystemInfoStruct>(10);

        #endregion

        #region Events

        /// <summary>
        /// Delegate to notify that online status of a system changed
        /// </summary>
        /// <param name="systemId">The system identifier.</param>
        public delegate void OnlineStatusChangedHandler(string systemId, bool isOnline);

        /// <summary>
        /// Occurs when online status changed with a train.
        /// </summary>
        public event OnlineStatusChangedHandler OnlineStatusChanged;

        #endregion

        #region public functions

        /// <summary>
        /// Determines whether the the specified session identifier is a valid session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>
        ///   <c>true</c> if session is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSessionValid(int sessionId)
        {
            lock (_sessionLock)
            {
                return _sessions.ContainsKey(sessionId);
            }
        }

        /// <summary>
        /// Determines whether the specified system identifier is online or not.
        /// </summary>
        /// <param name="systemId">The system identifier.</param>
        /// <returns>
        ///   <c>true</c> if the specified system identifier is online; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSystemOnline(string systemId)
        {
            lock (_systemInfoLock)
            {
                SystemInfoStruct systemInfo;
                return _systems.TryGetValue(systemId, out systemInfo) && systemInfo.isOnline;
            }
        }

        /// <summary>
        /// Gets the notification URL associated to a session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>The notification url associated to the session. Empty string if no url.</returns>
        public string GetNotificationUrl(int sessionId)
        {
            string notificationUrl;
            lock (_sessionLock)
            {
                SessionData sessionInfo;
                if (_sessions.TryGetValue(sessionId, out sessionInfo))
                {
                    notificationUrl = sessionInfo.NotificationUrl;
                }
                else
                {
                    notificationUrl = string.Empty;
                }
            }

            return notificationUrl;
        }

        /// <summary>
        /// Updates the information on a system.
        /// </summary>
        /// <param name="systemId">The system identifier.</param>
        /// <param name="vehicleId">The vehicle identifier.</param>
        /// <param name="isOnline">The online status of the system.</param>
        /// <param name="status">The status.</param>
        /// <param name="missionId">The mission identifier.</param>
        /// <param name="communicationLink">The communication link.</param>
        public void UpdateSystem(string systemId, int vehicleId, bool isOnline, uint status, string missionId, CommLinkEnum communicationLink, string IPAddress)
        {
            SystemInfoStruct newSystemInfo = new SystemInfoStruct();
            newSystemInfo.communicationLink = communicationLink;
            newSystemInfo.isOnline = isOnline;
            newSystemInfo.missionId = missionId;
            newSystemInfo.status = status;
            newSystemInfo.systemId = systemId;
            newSystemInfo.IPAddress = IPAddress;

            bool onlineStatusChanged = false;

            lock (_systemInfoLock)
            {
                SystemInfoStruct existingSystemInfo;
                if (_systems.TryGetValue(systemId, out existingSystemInfo))
                {
                    if (!(existingSystemInfo.vehiclePhysicalId != vehicleId ||
                        existingSystemInfo.isOnline != isOnline ||
                        existingSystemInfo.status != status ||
                        existingSystemInfo.missionId != missionId ||
                        existingSystemInfo.communicationLink != communicationLink ||
                        existingSystemInfo.IPAddress != IPAddress))
                    {
                        newSystemInfo = null;
                    }
                    else if (existingSystemInfo.isOnline != isOnline)
                    {
                        onlineStatusChanged = true;
                    }
                }
                else
                {
                    onlineStatusChanged = true;
                }

                if (newSystemInfo != null)
                {
                    _systems[systemId] = newSystemInfo;
                }
            }

            // Notify about changes
            if (newSystemInfo != null)
            {
                List<string> urlsToNotify;

                lock (_sessionLock)
                {
                    urlsToNotify = _sessions.Values.Where(s => s.NotificationEnabled && !string.IsNullOrEmpty(s.NotificationUrl)).Select(s => s.NotificationUrl).Distinct().ToList();
                }

                if (urlsToNotify.Count > 0)
                {

                    DataPackageTests.T2GServiceInterface.Notification.systemInfoStruct systemInfoNotification = newSystemInfo.ConvertToNotification();
                    foreach (string url in urlsToNotify)
                    {
                        EndpointAddress address = new EndpointAddress(url);
                        using (NotificationClient client = new NotificationClient("NotificationClient", address))
                        {
                            try
                            {
                                client.Open();
                                client.onSystemChangedNotification(systemInfoNotification);
                            }
                            finally
                            {
                                if (client.State == CommunicationState.Faulted)
                                {
                                    client.Abort();
                                }
                            }
                        }
                    }
                }

                if (onlineStatusChanged)
                {
                    OnlineStatusChangedHandler handlers = OnlineStatusChanged;
                    if (handlers != null)
                    {
                        handlers(newSystemInfo.systemId, newSystemInfo.isOnline);
                    }
                }
            }
        }

        #endregion

        #region Implementation of IdentificationPortType interface

        /// <summary>
        /// Implement the Login function of T2G Identification service.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>session id 0 if user name is invalid, otherwise a valid session id.</returns>
        public loginOutput login(loginInput request)
        {
            loginOutput result = new loginOutput();

            if (request.Body.name == "admin" && request.Body.name == "admin")
            {
                lock (_sessionLock)
                {
                    result.Body = new loginOutputBody(_nextSessionId++, Math.Min(CurrentProtocolVersion, Math.Max(1, request.Body.protocolVersion)));
                    _sessions.Add(result.Body.sessionId, new SessionData(request.Body.name, request.Body.notificationURL, result.Body.effectiveProtocolVersion, request.Body.applicationIdList));
                }
            }
            else
            {
                result.Body = new loginOutputBody(0, -1);
            }

            return result;
        }

        /// <summary>
        /// Implements the simple login function of T2G services.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>session id 0 if user name is invalid, otherwise a valid session id.</returns>
        public simpleLoginOutput simpleLogin(simpleLoginInput request)
        {
            simpleLoginOutput result = new simpleLoginOutput();

            if (request.Body.name == "admin" && request.Body.name == "admin")
            {
                lock (_sessionLock)
                {
                    result.Body = new simpleLoginOutputBody(_nextSessionId++);
                    _sessions.Add(result.Body.sessionId, new SessionData(request.Body.name, CurrentProtocolVersion));
                }
            }
            else
            {
                result.Body = new simpleLoginOutputBody(0);
            }

            return result;
        }

        /// <summary>
        /// Implement the logout function of T2G Identification interface.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        public void logout(int sessionId)
        {
            lock (_sessionLock)
            {
                _sessions.Remove(sessionId);
            }
        }

        /// <summary>
        /// Implements the keep alive session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="seconds">The seconds.</param>
        /// <exception cref="FaultException">Invalid session provided</exception>
        public void keepAliveSession(int sessionId, int seconds)
        {
            if (!IsSessionValid(sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }
        }

        /// <summary>
        /// Implements the enumSessions of T2G-Identification service.
        /// </summary>
        /// <param name="request">The request input.</param>
        /// <returns>The list of know sessions</returns>
        public enumSessionsOutput enumSessions(enumSessionsInput request)
        {
            if (!IsSessionValid(request.Body.sessionId))
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();

            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        /// <summary>
        /// Implements the enumSessions of T2G-Identification service.
        /// </summary>
        /// <param name="request">The request input.</param>
        /// <returns>The list of know systems.</returns>
        public enumSystemsOutput enumSystems(enumSystemsInput request)
        {
            if (!IsSessionValid(request.Body.sessionId))
            {
                throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
            }

            systemList systemList = new systemList();
            lock (_systemInfoLock)
            {
                systemList.Capacity = _systems.Count;
                systemList.AddRange(_systems.Values);
            }

            enumSystemsOutput result = new enumSystemsOutput();
            result.Body = new enumSystemsOutputBody(systemList);

            return result;
        }

        /// <summary>
        /// Implements the getSystemInfo of T2G-Identification services.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public getSystemInfoOutput getSystemInfo(getSystemInfoInput request)
        {
            throw FaultExceptionFactory.CreateNotImplementedFault();
        }

        /// <summary>
        /// Implements the enableSystemNotification of T2G-Identification services.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="enable">The new enable state for system notifications.</param>
        /// <exception cref="FaultException">Invalid session provided</exception>
        public void enableSystemNotifications(int sessionId, bool enable)
        {
            lock (_sessionLock)
            {
                SessionData sessionInfo;
                if (!_sessions.TryGetValue(sessionId, out sessionInfo))
                {
                    throw FaultExceptionFactory.CreateInvalidSessionIdentifierFault();
                }
                else if (string.IsNullOrEmpty(sessionInfo.NotificationUrl))
                {
                    throw FaultExceptionFactory.CreateNoNotificationUrlFault();
                }
                else
                {
                    _sessions[sessionId].NotificationEnabled = enable;
                }
            }
        }

        #endregion
    }
}
