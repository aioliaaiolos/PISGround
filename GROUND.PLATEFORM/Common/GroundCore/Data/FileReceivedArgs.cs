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
    public class FileReceivedArgs : EventArgs
    {
        public int FolderId;

        public Guid RequestId;
    }
}
