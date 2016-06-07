///
namespace PIS.Ground.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;   

    /// <summary>
    /// Representing Transfer Task data
    /// </summary>
    public class TransferTaskData 
    {
        private int taskIdField;

        private string taskSystemIdField;

        private int foreignTaskIdField;

        private int folderIdField;

        private string folderSystemIdField;

        private string descriptionField;

        private sbyte priorityField;

        private FileTransferMode linkTypeField;

        private bool automaticallyStopField;

        private string creatorField;

        private System.DateTime creationDateField;

        private System.DateTime startDateField;

        private System.DateTime expirationDateField;

        private System.DateTime completionDateField;

        private TaskPhase taskPhaseField;

        private TaskState taskStateField;

        private ushort activeFileTransferCountField;

        private ushort errorCountField;

        private sbyte acquisitionCompletionPercentField;

        private sbyte transferCompletionPercentField;

        private sbyte distributionCompletionPercentField;

        private string transferNotifURLField;

        public int TaskId
        {
            get
            {
                return this.taskIdField;
            }

            set
            {
                this.taskIdField = value;
            }
        }

        public string TaskSystemId
        {
            get
            {
                return this.taskSystemIdField;
            }

            set
            {
                this.taskSystemIdField = value;
            }
        }

        public int ForeignTaskId
        {
            get
            {
                return this.foreignTaskIdField;
            }

            set
            {
                this.foreignTaskIdField = value;
            }
        }

        public int FolderId
        {
            get
            {
                return this.folderIdField;
            }

            set
            {
                this.folderIdField = value;
            }
        }

        public string FolderSystemId
        {
            get
            {
                return this.folderSystemIdField;
            }

            set
            {
                this.folderSystemIdField = value;
            }
        }

        public string Description
        {
            get
            {
                return this.descriptionField;
            }

            set
            {
                this.descriptionField = value;
            }
        }

        public sbyte Priority
        {
            get
            {
                return this.priorityField;
            }

            set
            {
                this.priorityField = value;
            }
        }

        public FileTransferMode LinkType
        {
            get
            {
                return this.linkTypeField;
            }

            set
            {
                this.linkTypeField = value;
            }
        }

        public bool AutomaticallyStop
        {
            get
            {
                return this.automaticallyStopField;
            }

            set
            {
                this.automaticallyStopField = value;
            }
        }

        public string Creator
        {
            get
            {
                return this.creatorField;
            }

            set
            {
                this.creatorField = value;
            }
        }

        public System.DateTime CreationDate
        {
            get
            {
                return this.creationDateField;
            }

            set
            {
                this.creationDateField = value;
            }
        }

        public System.DateTime StartDate
        {
            get
            {
                return this.startDateField;
            }

            set
            {
                this.startDateField = value;
            }
        }

        public System.DateTime ExpirationDate
        {
            get
            {
                return this.expirationDateField;
            }

            set
            {
                this.expirationDateField = value;
            }
        }

        public System.DateTime CompletionDate
        {
            get
            {
                return this.completionDateField;
            }

            set
            {
                this.completionDateField = value;
            }
        }

        public TaskPhase TaskPhase
        {
            get
            {
                return this.taskPhaseField;
            }

            set
            {
                this.taskPhaseField = value;
            }
        }

        public TaskState TaskState
        {
            get
            {
                return this.taskStateField;
            }

            set
            {
                this.taskStateField = value;
            }
        }

        public ushort ActiveFileTransferCount
        {
            get
            {
                return this.activeFileTransferCountField;
            }

            set
            {
                this.activeFileTransferCountField = value;
            }
        }

        public ushort ErrorCount
        {
            get
            {
                return this.errorCountField;
            }

            set
            {
                this.errorCountField = value;
            }
        }

        public sbyte AcquisitionCompletionPercent
        {
            get
            {
                return this.acquisitionCompletionPercentField;
            }

            set
            {
                this.acquisitionCompletionPercentField = value;
            }
        }

        public sbyte TransferCompletionPercent
        {
            get
            {
                return this.transferCompletionPercentField;
            }

            set
            {
                this.transferCompletionPercentField = value;
            }
        }

        public sbyte DistributionCompletionPercent
        {
            get
            {
                return this.distributionCompletionPercentField;
            }

            set
            {
                this.distributionCompletionPercentField = value;
            }
        }

        public string TransferNotifURL
        {
            get
            {
                return this.transferNotifURLField;
            }

            set
            {
                this.transferNotifURLField = value;
            }
        }
    }    
}
