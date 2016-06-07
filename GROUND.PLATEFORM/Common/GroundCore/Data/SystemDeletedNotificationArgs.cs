//---------------------------------------------------------------------------------------------------
// <copyright file="SystemDeletedNotificationArgs.cs" company="Alstom">
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
    /// System Deleted Notification argument
    /// </summary>
    public class SystemDeletedNotificationArgs : EventArgs
    {
        public string SystemId;
    }
}
