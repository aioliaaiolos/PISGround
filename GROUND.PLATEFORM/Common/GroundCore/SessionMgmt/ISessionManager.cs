//---------------------------------------------------------------------------------------------------
// <copyright file="ISessionManager.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
namespace PIS.Ground.Core.SessionMgmt
{
	/// <summary>Interface for session manager.</summary>
	public interface ISessionManager
	{
        /// <summary>
        /// Starts the monitoring sessions.
        /// </summary>
        void StartMonitoringSessions();

        /// <summary>
        /// Stops the monitoring sessions.
        /// </summary>
        void StopMonitoringSessions();

		/// <summary>Generates a request identifier.</summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="objGuid">[out] Unique identifier for the object.</param>
		/// <returns>The request identifier.</returns>
		string GenerateRequestID(Guid sessionId, out Guid objGuid);

		/// <summary>Gets notification URL by request identifier.</summary>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="notificationUrl">[out] URL of the notification.</param>
		/// <returns>The notification URL by request identifier.</returns>
		string GetNotificationUrlByRequestId(Guid requestId, out string notificationUrl);

		/// <summary>Gets notification URL by session identifier.</summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="notificationUrl">[out] URL of the notification.</param>
		/// <returns>The notification URL by session identifier.</returns>
		string GetNotificationUrlBySessionId(Guid sessionId, out string notificationUrl);

		/// <summary>Gets session identifier corresponding to the specified request identifier.</summary>
		/// <param name="requestId">input: Request ID.</param>
		/// <param name="sessionId">output: Session ID.</param>
		/// <returns>error if any.</returns>
		string GetSessionIdByRequestId(Guid requestId, out Guid sessionId);

		/// <summary>Gets notification urls.</summary>
		/// <param name="notificationUrls">[out] The notification urls.</param>
		/// <returns>The notification urls.</returns>
		string GetNotificationUrls(System.Collections.Generic.List<string> notificationUrls);

		/// <summary>Query if 'sessionId' is session valid.</summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <returns>true if session valid, false if not.</returns>
		bool IsSessionValid(Guid sessionId);

		/// <summary>Keep session alive.</summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="timeOut">The time out.</param>
		/// <returns>.</returns>
		string KeepSessionAlive(Guid sessionId, int timeOut);

		/// <summary>Login.</summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <param name="objGuid">[out] Unique identifier for the object.</param>
		/// <returns>.</returns>
		string Login(string username, string password, out Guid objGuid);

		/// <summary>Removes the session identifier described by sessionId.</summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <returns>.</returns>
		string RemoveSessionID(Guid sessionId);

		/// <summary>Sets notification URL.</summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="strNotificationURL">URL of the notification.</param>
		/// <returns>.</returns>
		string SetNotificationURL(Guid sessionId, string strNotificationURL);

		/// <summary>Updates the request identifier status.</summary>
		/// <param name="strRequestId">Identifier for the request.</param>
		/// <param name="status">The status.</param>
		/// <returns>.</returns>
		string UpdateRequestIdStatus(string strRequestId, PIS.Ground.Core.Data.RequestStatus status);
	}
}
