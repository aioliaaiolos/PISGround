//---------------------------------------------------------------------------------------------------
// <copyright file="SessionServiceStub.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.Session;

namespace SessionTests
{
    /// <summary>
    /// Class to help instantiate session service for tests
    /// </summary>
    /// <seealso cref="PIS.Ground.Session.SessionService" />
    /// <seealso cref="System.IDisposable" />
    public class SessionServiceStub : SessionService, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionServiceStub"/> class.
        /// </summary>
        /// <param name="sessionManager">The session manager.</param>
        /// <param name="t2gManager">The T2G manager.</param>
        /// <param name="notificationSender">The notification sender.</param>
        public SessionServiceStub(ISessionManager sessionManager, IT2GManager t2gManager, INotificationSender notificationSender) :
            base(sessionManager, t2gManager, notificationSender)
        {
            // No logic body
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
            SessionService.Uninitialize();
        }

        #endregion
    }
}
