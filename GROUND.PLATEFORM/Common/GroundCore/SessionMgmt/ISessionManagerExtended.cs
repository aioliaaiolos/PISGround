//---------------------------------------------------------------------------------------------------
// <copyright file="ISessionManagerExtended.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
namespace PIS.Ground.Core.SessionMgmt
{
	/// <summary>Interface for an extended session manager.</summary>
    public interface ISessionManagerExtended : ISessionManager
	{
        /// <summary>
        /// Insert a new RequestId not associated with a SessionID.
        /// To be used when notification is to be sent to all connected consoles
        /// </summary>
        /// <param name="objGuid">Request id or Guid.empty if an error occurs</param>
        /// <returns>Error if any</returns>
        string GenerateRequestID(out Guid objGuid);
	}
}
