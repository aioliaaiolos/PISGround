/// 
namespace PIS.Ground.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// File publication Notification Argument
    /// </summary>
    public class FilePublicationNotificationArgs : EventArgs
    {
        public int FolderId;

        public sbyte CompletionPercent;

        public AcquisitionState PublicationAcquisitionState;

        public string Error;
    }
}
