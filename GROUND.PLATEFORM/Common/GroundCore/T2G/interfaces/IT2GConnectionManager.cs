using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIS.Ground.Core.T2G
{
	/// <summary>T2G server connection manager.</summary>
	public interface IT2GConnectionManager
	{
		/// <summary>Gets the T2G server connection status .</summary>
		/// <value>true if T2G server is online, false if not.</value>
		bool T2GServerConnectionStatus { get; }
	}
}
