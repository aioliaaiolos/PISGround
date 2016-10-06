//---------------------------------------------------------------------------------------------------
// <copyright file="FileDistributionStatusArgs.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Globalization;

namespace PIS.Ground.Core.Data
{

    /// <summary>
    /// File distribution status argument used as Event argument
    /// </summary>
    public class FileDistributionStatusArgs : EventArgs
    {
        #region variables
        /// <summary>
        /// request id
        /// </summary>
        private Guid requestID;

        /// <summary>
        /// task id
        /// </summary>
        private int taskId;

        /// <summary>
        /// task state
        /// </summary>
        private TaskState taskState;

        /// <summary>
        /// task phase
        /// </summary>
        private TaskPhase taskPhase;

        /// <summary>
        /// avtive file transfer count
        /// </summary>
        private ushort activeFileTransferCount;

        /// <summary>
        /// error count
        /// </summary>
        private ushort errorCount;

        /// <summary>
        /// acqisition complete percentage
        /// </summary>
        private sbyte acquisitionCompletionPercent;

        /// <summary>
        /// transfer completion percentage
        /// </summary>
        private sbyte transferCompletionPercent;

        /// <summary>
        /// distibution complete percentage
        /// </summary>
        private sbyte distributionCompletionPercent;

        /// <summary>
        /// folder id
        /// </summary>
        private int folderId;

        /// <summary>
        /// error that has occured.
        /// </summary>
        private string error;
               
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the folder id
        /// </summary>
        public int FolderId
        {
            get
            {
                return this.folderId;
            }

            set
            {
                this.folderId = value;
            }
        }

        /// <summary>
        /// Gets or sets the the transfer complete percentage
        /// </summary>
        public sbyte TransferCompletionPercent
        {
            get
            {
                return this.transferCompletionPercent;
            }

            set
            {
                this.transferCompletionPercent = value;
            }
        }

        /// <summary>
        /// Gets or sets the  the distribution complete percentage
        /// </summary>
        public sbyte DistributionCompletionPercent
        {
            get
            {
                return this.distributionCompletionPercent;
            }

            set
            {
                this.distributionCompletionPercent = value;
            }
        }

        /// <summary>
        /// Gets or sets the current task phase
        /// </summary>
        public TaskPhase CurrentTaskPhase
        {
            get
            {
                return this.taskPhase;
            }

            set
            {
                this.taskPhase = value;
            }
        }

        /// <summary>
        /// Gets or sets the  active file transfer count
        /// </summary>
        public ushort ActiveFileTransferCount
        {
            get
            {
                return this.activeFileTransferCount;
            }

            set
            {
                this.activeFileTransferCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the error count
        /// </summary>
        public ushort ErrorCount
        {
            get
            {
                return this.errorCount;
            }

            set
            {
                this.errorCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the  acqisition complete percentage
        /// </summary>
        public sbyte AcquisitionCompletionPercent
        {
            get
            {
                return this.acquisitionCompletionPercent;
            }

            set
            {
                this.acquisitionCompletionPercent = value;
            }
        }

        /// <summary>
        /// Gets or sets the the request id
        /// </summary>
        public Guid RequestId
        {
            get
            {
                return this.requestID;
            }

            set
            {
                this.requestID = value; 
            }
        }

        /// <summary>
        /// Gets or sets the task id
        /// </summary>
        public int TaskId
        {
            get
            {
                return this.taskId;
            }

            set
            {
                this.taskId = value;
            }
        }

        /// <summary>
        /// Gets or sets the task status
        /// </summary>
        public TaskState TaskStatus
        {
            get
            {
                return this.taskState;
            }

            set
            {
                this.taskState = value;
            }
        }

        /// <summary>
        /// Get the latest error
        /// </summary>
        public string Error
        {
            get
            {
                return error;
            }
        }

        /// <summary>
        /// Set the error
        /// </summary>
        internal string SetError
        {
            set
            {
                error = value;
            }
        }

#endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                        "    RequestId                     : {0}\r\n" +
                        "    TaskId                        : {1}\r\n" +
                        "    TaskStatus                    : {2}\r\n" +
                        "    CurrentTaskPhase              : {3}\r\n" +
                        "    AcquisitionCompletionPercent  : {4}\r\n" +
                        "    DistributionCompletionPercent : {5}\r\n" +
                        "    TransferCompletionPercent     : {6}\r\n"
                            , RequestId
                            , TaskId
                            , TaskStatus
                            , CurrentTaskPhase
                            , AcquisitionCompletionPercent
                            , DistributionCompletionPercent
                            , TransferCompletionPercent);
        }
    }
}
