//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineForcingRequestContext.cs" company="Alstom">
//          (c) Copyright ALSTOM 2015.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PIS.Ground.Core.Data;
using PIS.Ground.DataPackage;

namespace PIS.Ground.DataPackage.RequestMgt
{
	/// <summary>Implementation of the request context specific to baseline forcing.</summary>
	public class BaselineForcingRequestContext : RequestContext
	{
		/// <summary>Type of the command.</summary>
		private BaselineCommandType _commandType;

		/// <summary>Initializes a new instance of the BaselineForcingRequestContext class.</summary>
		/// <param name="endpoint">The endpoint.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="timeout">The timeout.</param>
		/// <param name="commandType">Type of the command.</param>
		public BaselineForcingRequestContext(string endpoint, string elementId, Guid requestId, Guid sessionId, uint timeout, BaselineCommandType commandType)
			: base(endpoint, elementId, requestId, sessionId, timeout)
		{
			_commandType = commandType;
		}

		/// <summary>Gets the type of the command.</summary>
		/// <value>The type of the command.</value>
		public BaselineCommandType CommandType
		{
			get
			{
				return _commandType;
			}
		}
	}
}