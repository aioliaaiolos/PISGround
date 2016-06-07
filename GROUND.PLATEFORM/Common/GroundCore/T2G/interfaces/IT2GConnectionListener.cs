//---------------------------------------------------------------------------------------------------
// <copyright file="IT2GConnectionListener.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.T2G
{
    /// <summary>Interface for T2G connection listener.</summary>
    interface IT2GConnectionListener
    {
        /// <summary>Executes the connection status changed action.</summary>
        /// <param name="connected">true if connected.</param>
        void OnConnectionStatusChanged(bool connected);
    }
}
