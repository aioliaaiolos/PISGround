//---------------------------------------------------------------------------------------------------
// <copyright file="IRequestProcessor.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2013.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.LiveVideoControl
{
	/// <summary>Interface for request processor.</summary>
	public interface IRequestProcessor
	{
		/// <summary>Adds a request range.</summary>
		/// <param name="newRequests">The new requests.</param>
		void AddRequestRange(System.Collections.Generic.List<PIS.Ground.Core.Data.RequestContext> newRequests);

		/// <summary>Adds a request.</summary>
		/// <param name="newRequest">The new request.</param>
		void AddRequest(PIS.Ground.Core.Data.RequestContext newRequest);

        /// <summary>Sets Train2Ground manager.</summary>
        /// <param name="train2groundClient">The train 2 ground manager.</param>
		void SetT2GManager(PIS.Ground.Core.T2G.IT2GManager train2groundManager);

		/// <summary>Sets Train2Ground manager.</summary>
        /// <param name="train2groundClient">The train 2 ground manager.</param>
		/// <param name="applicationId">Identifier for the application.</param>
        void SetT2GManager(PIS.Ground.Core.T2G.IT2GManager train2groundManager, string applicationId);

		/// <summary>Sets session manager.</summary>
		/// <param name="sessionMgr">Manager for session.</param>
		void SetSessionMgr(PIS.Ground.Core.SessionMgmt.ISessionManager sessionMgr);

        /// <summary>Sets Notification Sender.</summary>
        /// <param name="notificationSender">The Notification Sender.</param>
        void SetNotificationSender(PIS.Ground.Core.Common.INotificationSender notificationSender);
	}
}