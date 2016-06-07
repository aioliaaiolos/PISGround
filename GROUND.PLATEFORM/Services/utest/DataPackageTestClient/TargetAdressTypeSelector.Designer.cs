namespace DataPackageTestClient
{
    partial class TargetAdressTypeSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TargetAdressTypeSelector));
            this.AddressType = new System.Windows.Forms.Label();
            this.AddressTypeList = new System.Windows.Forms.ListBox();
            this.AddressId = new System.Windows.Forms.Label();
            this.AddressIdBox = new System.Windows.Forms.TextBox();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // AddressType
            // 
            this.AddressType.AutoSize = true;
            this.AddressType.Location = new System.Drawing.Point(12, 9);
            this.AddressType.Name = "AddressType";
            this.AddressType.Size = new System.Drawing.Size(69, 13);
            this.AddressType.TabIndex = 0;
            this.AddressType.Text = "AddressType";
            // 
            // AddressTypeList
            // 
            this.AddressTypeList.FormattingEnabled = true;
            this.AddressTypeList.Location = new System.Drawing.Point(83, 9);
            this.AddressTypeList.Name = "AddressTypeList";
            this.AddressTypeList.Size = new System.Drawing.Size(197, 56);
            this.AddressTypeList.TabIndex = 1;
            // 
            // AddressId
            // 
            this.AddressId.AutoSize = true;
            this.AddressId.Location = new System.Drawing.Point(12, 78);
            this.AddressId.Name = "AddressId";
            this.AddressId.Size = new System.Drawing.Size(54, 13);
            this.AddressId.TabIndex = 2;
            this.AddressId.Text = "AddressId";
            // 
            // AddressIdBox
            // 
            this.AddressIdBox.Location = new System.Drawing.Point(83, 78);
            this.AddressIdBox.Name = "AddressIdBox";
            this.AddressIdBox.Size = new System.Drawing.Size(197, 20);
            this.AddressIdBox.TabIndex = 3;
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(15, 109);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(117, 24);
            this.OK.TabIndex = 4;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(155, 109);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(124, 23);
            this.Cancel.TabIndex = 5;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // TargetAdressTypeSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(290, 145);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.AddressIdBox);
            this.Controls.Add(this.AddressId);
            this.Controls.Add(this.AddressTypeList);
            this.Controls.Add(this.AddressType);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TargetAdressTypeSelector";
            this.Text = "TargetAdressTypeSelector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label AddressType;
        private System.Windows.Forms.Label AddressId;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        public System.Windows.Forms.ListBox AddressTypeList;
        public System.Windows.Forms.TextBox AddressIdBox;
    }
}