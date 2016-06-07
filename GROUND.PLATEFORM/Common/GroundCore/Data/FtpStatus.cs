using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace PIS.Ground.Core.Data
{
    public class FtpStatus
    {
        private string fileName;
        private Exception operationException = null;
        private string uri;
        private string status;
        private FtpStatusCode ftpStatusCode;

        public string Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        public string StatusDescription
        {
            get { return status; }
            set { status = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public Exception OperationException
        {
            get { return operationException; }
            set { operationException = value; }
        }

        public FtpStatusCode FtpStatusCode
        {
            get { return ftpStatusCode; }
            set { ftpStatusCode = value; }
        }
    }
}
