namespace DataPackageTestClient
{
    partial class DistributeBaselineForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Cancel = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.AddressIdBox = new System.Windows.Forms.TextBox();
            this.AddressId = new System.Windows.Forms.Label();
            this.AddressType = new System.Windows.Forms.Label();
            this.BaselineDistributionAttribute = new System.Windows.Forms.Label();
            this.TransferMode = new System.Windows.Forms.Label();
            this.FileCompressionBox = new System.Windows.Forms.CheckBox();
            this.TransferDatePicker = new System.Windows.Forms.DateTimePicker();
            this.TransferDate = new System.Windows.Forms.Label();
            this.ExpirationDatePicker = new System.Windows.Forms.DateTimePicker();
            this.ExpirationDate = new System.Windows.Forms.Label();
            this.Priority = new System.Windows.Forms.Label();
            this.PriorityBox = new System.Windows.Forms.TextBox();
            this.RequestTimeOutBox = new System.Windows.Forms.TextBox();
            this.RequestTimeOut = new System.Windows.Forms.Label();
            this.IncrementalBox = new System.Windows.Forms.CheckBox();
            this.AddressTypeList = new System.Windows.Forms.ListBox();
            this.TransferModeBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(170, 355);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(130, 25);
            this.Cancel.TabIndex = 11;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(15, 355);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(130, 25);
            this.OK.TabIndex = 10;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // AddressIdBox
            // 
            this.AddressIdBox.Location = new System.Drawing.Point(105, 77);
            this.AddressIdBox.Name = "AddressIdBox";
            this.AddressIdBox.Size = new System.Drawing.Size(200, 20);
            this.AddressIdBox.TabIndex = 9;
            // 
            // AddressId
            // 
            this.AddressId.AutoSize = true;
            this.AddressId.Location = new System.Drawing.Point(12, 77);
            this.AddressId.Name = "AddressId";
            this.AddressId.Size = new System.Drawing.Size(54, 13);
            this.AddressId.TabIndex = 8;
            this.AddressId.Text = "AddressId";
            // 
            // AddressType
            // 
            this.AddressType.AutoSize = true;
            this.AddressType.Location = new System.Drawing.Point(12, 15);
            this.AddressType.Name = "AddressType";
            this.AddressType.Size = new System.Drawing.Size(69, 13);
            this.AddressType.TabIndex = 6;
            this.AddressType.Text = "AddressType";
            // 
            // BaselineDistributionAttribute
            // 
            this.BaselineDistributionAttribute.AutoSize = true;
            this.BaselineDistributionAttribute.Location = new System.Drawing.Point(80, 109);
            this.BaselineDistributionAttribute.Name = "BaselineDistributionAttribute";
            this.BaselineDistributionAttribute.Size = new System.Drawing.Size(138, 13);
            this.BaselineDistributionAttribute.TabIndex = 12;
            this.BaselineDistributionAttribute.Text = "BaselineDistributionAttribute";
            // 
            // TransferMode
            // 
            this.TransferMode.AutoSize = true;
            this.TransferMode.Location = new System.Drawing.Point(12, 138);
            this.TransferMode.Name = "TransferMode";
            this.TransferMode.Size = new System.Drawing.Size(73, 13);
            this.TransferMode.TabIndex = 13;
            this.TransferMode.Text = "TransferMode";
            // 
            // FileCompressionBox
            // 
            this.FileCompressionBox.AutoSize = true;
            this.FileCompressionBox.Location = new System.Drawing.Point(15, 200);
            this.FileCompressionBox.Name = "FileCompressionBox";
            this.FileCompressionBox.Size = new System.Drawing.Size(102, 17);
            this.FileCompressionBox.TabIndex = 16;
            this.FileCompressionBox.Text = "FileCompression";
            this.FileCompressionBox.UseVisualStyleBackColor = true;
            // 
            // TransferDatePicker
            // 
            this.TransferDatePicker.Location = new System.Drawing.Point(105, 222);
            this.TransferDatePicker.Name = "TransferDatePicker";
            this.TransferDatePicker.Size = new System.Drawing.Size(200, 20);
            this.TransferDatePicker.TabIndex = 17;
            // 
            // TransferDate
            // 
            this.TransferDate.AutoSize = true;
            this.TransferDate.Location = new System.Drawing.Point(12, 226);
            this.TransferDate.Name = "TransferDate";
            this.TransferDate.Size = new System.Drawing.Size(69, 13);
            this.TransferDate.TabIndex = 18;
            this.TransferDate.Text = "TransferDate";
            // 
            // ExpirationDatePicker
            // 
            this.ExpirationDatePicker.Location = new System.Drawing.Point(105, 249);
            this.ExpirationDatePicker.Name = "ExpirationDatePicker";
            this.ExpirationDatePicker.Size = new System.Drawing.Size(200, 20);
            this.ExpirationDatePicker.TabIndex = 19;
            // 
            // ExpirationDate
            // 
            this.ExpirationDate.AutoSize = true;
            this.ExpirationDate.Location = new System.Drawing.Point(12, 253);
            this.ExpirationDate.Name = "ExpirationDate";
            this.ExpirationDate.Size = new System.Drawing.Size(76, 13);
            this.ExpirationDate.TabIndex = 20;
            this.ExpirationDate.Text = "ExpirationDate";
            // 
            // Priority
            // 
            this.Priority.AutoSize = true;
            this.Priority.Location = new System.Drawing.Point(12, 279);
            this.Priority.Name = "Priority";
            this.Priority.Size = new System.Drawing.Size(38, 13);
            this.Priority.TabIndex = 21;
            this.Priority.Text = "Priority";
            // 
            // PriorityBox
            // 
            this.PriorityBox.Location = new System.Drawing.Point(105, 276);
            this.PriorityBox.Name = "PriorityBox";
            this.PriorityBox.Size = new System.Drawing.Size(200, 20);
            this.PriorityBox.TabIndex = 22;
            // 
            // RequestTimeOutBox
            // 
            this.RequestTimeOutBox.Location = new System.Drawing.Point(105, 303);
            this.RequestTimeOutBox.Name = "RequestTimeOutBox";
            this.RequestTimeOutBox.Size = new System.Drawing.Size(200, 20);
            this.RequestTimeOutBox.TabIndex = 23;
            // 
            // RequestTimeOut
            // 
            this.RequestTimeOut.AutoSize = true;
            this.RequestTimeOut.Location = new System.Drawing.Point(12, 306);
            this.RequestTimeOut.Name = "RequestTimeOut";
            this.RequestTimeOut.Size = new System.Drawing.Size(87, 13);
            this.RequestTimeOut.TabIndex = 24;
            this.RequestTimeOut.Text = "RequestTimeOut";
            // 
            // IncrementalBox
            // 
            this.IncrementalBox.AutoSize = true;
            this.IncrementalBox.Location = new System.Drawing.Point(15, 332);
            this.IncrementalBox.Name = "IncrementalBox";
            this.IncrementalBox.Size = new System.Drawing.Size(81, 17);
            this.IncrementalBox.TabIndex = 25;
            this.IncrementalBox.Text = "Incremental";
            this.IncrementalBox.UseVisualStyleBackColor = true;
            // 
            // AddressTypeList
            // 
            this.AddressTypeList.FormattingEnabled = true;
            this.AddressTypeList.Location = new System.Drawing.Point(105, 15);
            this.AddressTypeList.Name = "AddressTypeList";
            this.AddressTypeList.Size = new System.Drawing.Size(199, 56);
            this.AddressTypeList.TabIndex = 26;
            // 
            // TransferModeBox
            // 
            this.TransferModeBox.FormattingEnabled = true;
            this.TransferModeBox.Location = new System.Drawing.Point(105, 138);
            this.TransferModeBox.Name = "TransferModeBox";
            this.TransferModeBox.Size = new System.Drawing.Size(200, 56);
            this.TransferModeBox.TabIndex = 27;
            // 
            // DistributeBaselineForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 386);
            this.Controls.Add(this.TransferModeBox);
            this.Controls.Add(this.AddressTypeList);
            this.Controls.Add(this.IncrementalBox);
            this.Controls.Add(this.RequestTimeOut);
            this.Controls.Add(this.RequestTimeOutBox);
            this.Controls.Add(this.PriorityBox);
            this.Controls.Add(this.Priority);
            this.Controls.Add(this.ExpirationDate);
            this.Controls.Add(this.ExpirationDatePicker);
            this.Controls.Add(this.TransferDate);
            this.Controls.Add(this.TransferDatePicker);
            this.Controls.Add(this.FileCompressionBox);
            this.Controls.Add(this.TransferMode);
            this.Controls.Add(this.BaselineDistributionAttribute);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.AddressIdBox);
            this.Controls.Add(this.AddressId);
            this.Controls.Add(this.AddressType);
            this.Name = "DistributeBaselineForm";
            this.Text = "DistributeBaselineForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK;
        public System.Windows.Forms.TextBox AddressIdBox;
        private System.Windows.Forms.Label AddressId;
        private System.Windows.Forms.Label AddressType;
        private System.Windows.Forms.Label BaselineDistributionAttribute;
        private System.Windows.Forms.Label TransferMode;
        private System.Windows.Forms.Label TransferDate;
        private System.Windows.Forms.Label ExpirationDate;
        private System.Windows.Forms.Label Priority;
        private System.Windows.Forms.Label RequestTimeOut;
        public System.Windows.Forms.CheckBox FileCompressionBox;
        public System.Windows.Forms.DateTimePicker TransferDatePicker;
        public System.Windows.Forms.DateTimePicker ExpirationDatePicker;
        public System.Windows.Forms.TextBox PriorityBox;
        public System.Windows.Forms.TextBox RequestTimeOutBox;
        public System.Windows.Forms.CheckBox IncrementalBox;
        public System.Windows.Forms.ListBox AddressTypeList;
        public System.Windows.Forms.ListBox TransferModeBox;

    }
}