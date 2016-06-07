//---------------------------------------------------------------------------------------------------
// <copyright file="IRequestContextProcessor.cs" company="Alstom">
//          (c) Copyright ALSTOM 2015.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIS.Ground.Core.Data
{
	/// <summary>Process the delegate described by requestContext.</summary>
	/// <param name="requestContext">Context for the request.</param>
	public delegate void ProcessDelegate(IRequestContext requestContext);

	/// <summary>Interface for request processor.</summary>
	public interface IRequestContextProcessor
	{
		/// <summary>Gets the process.</summary>
		/// <value>The process.</value>
		ProcessDelegate Process { get; }
	}
}
