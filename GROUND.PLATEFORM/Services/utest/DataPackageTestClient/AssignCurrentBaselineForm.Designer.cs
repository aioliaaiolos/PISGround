namespace DataPackageTestClient
{
    partial class AssignCurrentBaselineForm
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
            this.ExpirationPicker = new System.Windows.Forms.DateTimePicker();
            this.Expiration = new System.Windows.Forms.Label();
            this.VersionBox = new System.Windows.Forms.TextBox();
            this.Version = new System.Windows.Forms.Label();
            this.Cancel = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.AddressIdBox = new System.Windows.Forms.TextBox();
            this.AddressId = new System.Windows.Forms.Label();
            this.AddressTypeList = new System.Windows.Forms.ListBox();
            this.AddressType = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ExpirationPicker
            // 
            this.ExpirationPicker.Location = new System.Drawing.Point(81, 136);
            this.ExpirationPicker.Name = "ExpirationPicker";
            this.ExpirationPicker.Size = new System.Drawing.Size(200, 20);
            this.ExpirationPicker.TabIndex = 29;
            // 
            // Expiration
            // 
            this.Expiration.AutoSize = true;
            this.Expiration.Location = new System.Drawing.Point(13, 140);
            this.Expiration.Name = "Expiration";
            this.Expiration.Size = new System.Drawing.Size(53, 13);
            this.Expiration.TabIndex = 27;
            this.Expiration.Text = "Expiration";
            // 
            // VersionBox
            // 
            this.VersionBox.Location = new System.Drawing.Point(80, 108);
            this.VersionBox.Name = "VersionBox";
            this.VersionBox.Size = new System.Drawing.Size(199, 20);
            this.VersionBox.TabIndex = 25;
            // 
            // Version
            // 
            this.Version.AutoSize = true;
            this.Version.Location = new System.Drawing.Point(12, 111);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(42, 13);
            this.Version.TabIndex = 24;
            this.Version.Text = "Version";
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(157, 163);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(124, 23);
            this.Cancel.TabIndex = 23;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(16, 162);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(117, 24);
            this.OK.TabIndex = 22;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // AddressIdBox
            // 
            this.AddressIdBox.Location = new System.Drawing.Point(80, 81);
            this.AddressIdBox.Name = "AddressIdBox";
            this.AddressIdBox.Size = new System.Drawing.Size(200, 20);
            this.AddressIdBox.TabIndex = 21;
            // 
            // AddressId
            // 
            this.AddressId.AutoSize = true;
            this.AddressId.Location = new System.Drawing.Point(12, 81);
            this.AddressId.Name = "AddressId";
            this.AddressId.Size = new System.Drawing.Size(54, 13);
            this.AddressId.TabIndex = 20;
            this.AddressId.Text = "AddressId";
            // 
            // AddressTypeList
            // 
            this.AddressTypeList.FormattingEnabled = true;
            this.AddressTypeList.Location = new System.Drawing.Point(80, 12);
            this.AddressTypeList.Name = "AddressTypeList";
            this.AddressTypeList.Size = new System.Drawing.Size(200, 56);
            this.AddressTypeList.TabIndex = 19;
            // 
            // AddressType
            // 
            this.AddressType.AutoSize = true;
            this.AddressType.Location = new System.Drawing.Point(12, 12);
            this.AddressType.Name = "AddressType";
            this.AddressType.Size = new System.Drawing.Size(69, 13);
            this.AddressType.TabIndex = 18;
            this.AddressType.Text = "AddressType";
            // 
            // AssignCurrentBaselineForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 192);
            this.Controls.Add(this.ExpirationPicker);
            this.Controls.Add(this.Expiration);
            this.Controls.Add(this.VersionBox);
            this.Controls.Add(this.Version);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.AddressIdBox);
            this.Controls.Add(this.AddressId);
            this.Controls.Add(this.AddressTypeList);
            this.Controls.Add(this.AddressType);
            this.Name = "AssignCurrentBaselineForm";
            this.Text = "AssignCurrentBaselineForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DateTimePicker ExpirationPicker;
        private System.Windows.Forms.Label Expiration;
        public System.Windows.Forms.TextBox VersionBox;
        private System.Windows.Forms.Label Version;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK;
        public System.Windows.Forms.TextBox AddressIdBox;
        private System.Windows.Forms.Label AddressId;
        public System.Windows.Forms.ListBox AddressTypeList;
        private System.Windows.Forms.Label AddressType;
    }
}