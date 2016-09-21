//---------------------------------------------------------------------------------------------------
// <copyright file="IRequestManager.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIS.Ground.DataPackage.RequestMgt
{
	/// <summary>Interface for request manager.</summary>
	public interface IRequestManager
	{
		/// <summary>Initializes this object.</summary>
		/// <param name="train2groundManager">T2GManager instance.</param>
		/// <param name="notificationSender">The notification sender.</param>
		void Initialize(Ground.Core.T2G.IT2GManager train2groundManager, PIS.Ground.Core.Common.INotificationSender notificationSender);

        /// <summary>
        /// Uninitializes this instance.
        /// </summary>
        void Uninitialize();

		/// <summary>Adds a request.</summary>
		/// <param name="requestContext">Context for the request.</param>
		void AddRequest(PIS.Ground.Core.Data.IRequestContext requestContext);

		/// <summary>Adds a request range.</summary>
		/// <param name="requestContextList">List of request contexts.</param>
		void AddRequestRange(List<PIS.Ground.Core.Data.IRequestContext> requestContextList);
	}
}