namespace DataPackageTestClient
{
    partial class AssignFutureBaselineForm
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
            this.AddressTypeList = new System.Windows.Forms.ListBox();
            this.AddressType = new System.Windows.Forms.Label();
            this.Version = new System.Windows.Forms.Label();
            this.VersionBox = new System.Windows.Forms.TextBox();
            this.Activation = new System.Windows.Forms.Label();
            this.Expiration = new System.Windows.Forms.Label();
            this.ActivationPicker = new System.Windows.Forms.DateTimePicker();
            this.ExpirationPicker = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(156, 186);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(124, 23);
            this.Cancel.TabIndex = 11;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(15, 185);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(117, 24);
            this.OK.TabIndex = 10;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // AddressIdBox
            // 
            this.AddressIdBox.Location = new System.Drawing.Point(80, 78);
            this.AddressIdBox.Name = "AddressIdBox";
            this.AddressIdBox.Size = new System.Drawing.Size(200, 20);
            this.AddressIdBox.TabIndex = 9;
            // 
            // AddressId
            // 
            this.AddressId.AutoSize = true;
            this.AddressId.Location = new System.Drawing.Point(12, 78);
            this.AddressId.Name = "AddressId";
            this.AddressId.Size = new System.Drawing.Size(54, 13);
            this.AddressId.TabIndex = 8;
            this.AddressId.Text = "AddressId";
            // 
            // AddressTypeList
            // 
            this.AddressTypeList.FormattingEnabled = true;
            this.AddressTypeList.Location = new System.Drawing.Point(80, 9);
            this.AddressTypeList.Name = "AddressTypeList";
            this.AddressTypeList.Size = new System.Drawing.Size(200, 56);
            this.AddressTypeList.TabIndex = 7;
            // 
            // AddressType
            // 
            this.AddressType.AutoSize = true;
            this.AddressType.Location = new System.Drawing.Point(12, 9);
            this.AddressType.Name = "AddressType";
            this.AddressType.Size = new System.Drawing.Size(69, 13);
            this.AddressType.TabIndex = 6;
            this.AddressType.Text = "AddressType";
            // 
            // Version
            // 
            this.Version.AutoSize = true;
            this.Version.Location = new System.Drawing.Point(12, 108);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(42, 13);
            this.Version.TabIndex = 12;
            this.Version.Text = "Version";
            // 
            // VersionBox
            // 
            this.VersionBox.Location = new System.Drawing.Point(80, 105);
            this.VersionBox.Name = "VersionBox";
            this.VersionBox.Size = new System.Drawing.Size(199, 20);
            this.VersionBox.TabIndex = 13;
            // 
            // Activation
            // 
            this.Activation.AutoSize = true;
            this.Activation.Location = new System.Drawing.Point(12, 136);
            this.Activation.Name = "Activation";
            this.Activation.Size = new System.Drawing.Size(54, 13);
            this.Activation.TabIndex = 14;
            this.Activation.Text = "Activation";
            // 
            // Expiration
            // 
            this.Expiration.AutoSize = true;
            this.Expiration.Location = new System.Drawing.Point(12, 163);
            this.Expiration.Name = "Expiration";
            this.Expiration.Size = new System.Drawing.Size(53, 13);
            this.Expiration.TabIndex = 15;
            this.Expiration.Text = "Expiration";
            // 
            // ActivationPicker
            // 
            this.ActivationPicker.Location = new System.Drawing.Point(80, 132);
            this.ActivationPicker.Name = "ActivationPicker";
            this.ActivationPicker.Size = new System.Drawing.Size(200, 20);
            this.ActivationPicker.TabIndex = 16;
            // 
            // ExpirationPicker
            // 
            this.ExpirationPicker.Location = new System.Drawing.Point(80, 159);
            this.ExpirationPicker.Name = "ExpirationPicker";
            this.ExpirationPicker.Size = new System.Drawing.Size(200, 20);
            this.ExpirationPicker.TabIndex = 17;
            // 
            // AssignFutureBaselineForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 217);
            this.Controls.Add(this.ExpirationPicker);
            this.Controls.Add(this.ActivationPicker);
            this.Controls.Add(this.Expiration);
            this.Controls.Add(this.Activation);
            this.Controls.Add(this.VersionBox);
            this.Controls.Add(this.Version);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.AddressIdBox);
            this.Controls.Add(this.AddressId);
            this.Controls.Add(this.AddressTypeList);
            this.Controls.Add(this.AddressType);
            this.MaximumSize = new System.Drawing.Size(300, 244);
            this.MinimumSize = new System.Drawing.Size(300, 244);
            this.Name = "AssignFutureBaselineForm";
            this.Text = "AssignFutureBaselineForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK;
        public System.Windows.Forms.TextBox AddressIdBox;
        private System.Windows.Forms.Label AddressId;
        public System.Windows.Forms.ListBox AddressTypeList;
        private System.Windows.Forms.Label AddressType;
        private System.Windows.Forms.Label Version;
        private System.Windows.Forms.Label Activation;
        private System.Windows.Forms.Label Expiration;
        public System.Windows.Forms.TextBox VersionBox;
        public System.Windows.Forms.DateTimePicker ActivationPicker;
        public System.Windows.Forms.DateTimePicker ExpirationPicker;
    }
}