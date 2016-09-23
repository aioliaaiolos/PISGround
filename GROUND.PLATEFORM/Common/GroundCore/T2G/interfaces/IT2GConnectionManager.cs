//---------------------------------------------------------------------------------------------------
// <copyright file="IT2GConnectionManager.cs" company="Alstom">
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

namespace PIS.Ground.Core.T2G
{
	/// <summary>T2G server connection manager.</summary>
	public interface IT2GConnectionManager: IDisposable
	{
		/// <summary>Gets the T2G server connection status .</summary>
		/// <value>true if T2G server is online, false if not.</value>
		bool T2GServerConnectionStatus { get; }
	}
}
