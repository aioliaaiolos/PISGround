//---------------------------------------------------------------------------------------------------
// <copyright file="RequestManagerMonitor.cs" company="Alstom">
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
using PIS.Ground.DataPackage.RequestMgt;
using System.Threading;

namespace DataPackageTests.RequestMgt
{
    /// <summary>
    /// Specialization of RequestManager class that allow monitoring internal states.
    /// </summary>
    class RequestManagerMonitor : RequestManager
    {
#region Fields

        private ManualResetEvent RequestHandledEvent = new ManualResetEvent(true);

#endregion

        public RequestManagerMonitor()
        {

        }

        #region Specialization methods

        /// <summary>
        /// Adds a request range. This method is meant to be used at startup to reload saved requests.
        /// Thereof, there is no save done if the requests are BaselineDistributingRequest, use AddRequest if needed.
        /// </summary>
        /// <param name="requestContextList">List of request contexts.</param>
		public override void AddRequestRange(List<PIS.Ground.Core.Data.IRequestContext> requestContextList)
		{
            lock (SyncLock)
            {
                base.AddRequestRange(requestContextList);

                if (requestContextList != null && requestContextList.Count > 0)
                {
                    RequestHandledEvent.Reset();
                }
            }
		}

        /// <summary>
        /// Adds a request.
        /// </summary>
        /// <param name="requestContext">Request to add.</param>
        public override void AddRequest(PIS.Ground.Core.Data.IRequestContext requestContext)
        {
            lock (SyncLock)
            {
                base.AddRequest(requestContext);

                if (requestContext != null)
                {
                    RequestHandledEvent.Reset();
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            RequestHandledEvent.Close();
        }

        /// <summary>
        /// Processes the requests.
        /// </summary>
        /// <param name="requests">The request to process</param>
        protected override void ProcessRequests(List<PIS.Ground.Core.Data.IRequestContext> requests)
        {
            base.ProcessRequests(requests);
            RequestHandledEvent.Set();
        }

        #endregion

        /// <summary>
        /// Waits that new requests where processed at least one time.
        /// </summary>
        /// <param name="timeout">The wait timeout.</param>
        /// <returns>True if all waiting request where processed, false otherwise.</returns>
        public bool WaitRequestProcessed(TimeSpan timeout)
        {
            return RequestHandledEvent.WaitOne(timeout);
        }
    }
}
