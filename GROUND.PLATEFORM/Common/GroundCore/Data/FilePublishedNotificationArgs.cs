/// 
namespace PIS.Ground.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// File published Notification argument
    /// </summary>
    public class FilePublishedNotificationArgs : EventArgs
    {
        public int FolderId;

        public string SystemId;

        public Guid RequestId;
    }
}
