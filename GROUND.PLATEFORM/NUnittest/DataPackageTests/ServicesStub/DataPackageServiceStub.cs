//---------------------------------------------------------------------------------------------------
// <copyright file="DataPackageServiceStub.cs" company="Alstom">
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
using PIS.Ground.Core.Common;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.DataPackage;
using PIS.Ground.DataPackage.RemoteDataStoreFactory;
using PIS.Ground.DataPackage.RequestMgt;


namespace DataPackageTests.ServicesStub
{
    /// <summary>
    /// Stub for DataPackage service
    /// </summary>
    public class DataPackageServiceStub : DataPackageService, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPackageServiceStub"/> class.
        /// </summary>
        /// <param name="sessionManager">The session manager.</param>
        /// <param name="notificationSender">The notification sender.</param>
        /// <param name="t2gManager">The T2G manager.</param>
        /// <param name="requestsFactory">The requests factory.</param>
        /// <param name="remoteDataStoreFactory">The remote data store factory.</param>
        /// <param name="requestManager">The request manager.</param>
        public DataPackageServiceStub(ISessionManager sessionManager,
            INotificationSender notificationSender,
            IT2GManager t2gManager,
            IRequestContextFactory requestsFactory,
            IRemoteDataStoreFactory remoteDataStoreFactory,
            IRequestManager requestManager) :
            base(sessionManager, notificationSender, t2gManager, requestsFactory, remoteDataStoreFactory, requestManager)
        {
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            Dispose(false);
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
                Uninitialize();
            }
        }

        #endregion
    }
}
