//---------------------------------------------------------------------------------------------------
// <copyright file="T2GOnlineStatusNotificationArgs.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------

namespace PIS.Ground.Core.Data
{
    using System;

    /// <summary>
    /// T2G Online / Offline Notification argument
    /// </summary>
    public class T2GOnlineStatusNotificationArgs : EventArgs
    {
        public bool online;
    }
}
