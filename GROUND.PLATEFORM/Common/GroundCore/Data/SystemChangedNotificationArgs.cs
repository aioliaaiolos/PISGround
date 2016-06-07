/// 
namespace PIS.Ground.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// System changed Notification argument
    /// </summary>
    public class SystemChangedNotificationArgs : EventArgs
    {
        public SystemInfo ChangedSystemInfo;
    }
}
