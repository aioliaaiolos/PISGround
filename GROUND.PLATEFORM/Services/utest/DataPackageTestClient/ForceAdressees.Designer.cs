namespace DataPackageTestClient
{
    partial class ForceAdressees
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
            this.TimeOut = new System.Windows.Forms.Label();
            this.TimeOutBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(156, 131);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(124, 23);
            this.Cancel.TabIndex = 11;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(16, 131);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(117, 24);
            this.OK.TabIndex = 10;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // AddressIdBox
            // 
            this.AddressIdBox.Location = new System.Drawing.Point(83, 78);
            this.AddressIdBox.Name = "AddressIdBox";
            this.AddressIdBox.Size = new System.Drawing.Size(197, 20);
            this.AddressIdBox.TabIndex = 9;
            // 
            // AddressId
            // 
            this.AddressId.AutoSize = true;
            this.AddressId.Location = new System.Drawing.Point(12, 81);
            this.AddressId.Name = "AddressId";
            this.AddressId.Size = new System.Drawing.Size(54, 13);
            this.AddressId.TabIndex = 8;
            this.AddressId.Text = "AddressId";
            // 
            // AddressTypeList
            // 
            this.AddressTypeList.FormattingEnabled = true;
            this.AddressTypeList.Location = new System.Drawing.Point(83, 9);
            this.AddressTypeList.Name = "AddressTypeList";
            this.AddressTypeList.Size = new System.Drawing.Size(197, 56);
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
            // TimeOut
            // 
            this.TimeOut.AutoSize = true;
            this.TimeOut.Location = new System.Drawing.Point(13, 108);
            this.TimeOut.Name = "TimeOut";
            this.TimeOut.Size = new System.Drawing.Size(47, 13);
            this.TimeOut.TabIndex = 12;
            this.TimeOut.Text = "TimeOut";
            // 
            // TimeOutBox
            // 
            this.TimeOutBox.Location = new System.Drawing.Point(83, 105);
            this.TimeOutBox.Name = "TimeOutBox";
            this.TimeOutBox.Size = new System.Drawing.Size(196, 20);
            this.TimeOutBox.TabIndex = 13;
            // 
            // ForceAdressees
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 160);
            this.Controls.Add(this.TimeOutBox);
            this.Controls.Add(this.TimeOut);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.AddressIdBox);
            this.Controls.Add(this.AddressId);
            this.Controls.Add(this.AddressTypeList);
            this.Controls.Add(this.AddressType);
            this.Name = "ForceAdressees";
            this.Text = "ForceAdressees";
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
        private System.Windows.Forms.Label TimeOut;
        public System.Windows.Forms.TextBox TimeOutBox;
    }
}