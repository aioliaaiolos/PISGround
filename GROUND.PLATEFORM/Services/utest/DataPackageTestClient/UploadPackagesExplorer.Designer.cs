namespace DataPackageTestClient
{
    partial class UploadPackagesExplorer
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
            this.FtpAddress = new System.Windows.Forms.Label();
            this.FtpAdressBox = new System.Windows.Forms.TextBox();
            this.Login = new System.Windows.Forms.Label();
            this.LoginBox = new System.Windows.Forms.TextBox();
            this.Password = new System.Windows.Forms.Label();
            this.PasswordBox = new System.Windows.Forms.MaskedTextBox();
            this.FilesList = new System.Windows.Forms.ListBox();
            this.List = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FtpAddress
            // 
            this.FtpAddress.AutoSize = true;
            this.FtpAddress.Location = new System.Drawing.Point(12, 9);
            this.FtpAddress.Name = "FtpAddress";
            this.FtpAddress.Size = new System.Drawing.Size(63, 13);
            this.FtpAddress.TabIndex = 0;
            this.FtpAddress.Text = "Ftp Address";
            // 
            // FtpAdressBox
            // 
            this.FtpAdressBox.Location = new System.Drawing.Point(78, 6);
            this.FtpAdressBox.Name = "FtpAdressBox";
            this.FtpAdressBox.Size = new System.Drawing.Size(281, 20);
            this.FtpAdressBox.TabIndex = 1;
            // 
            // Login
            // 
            this.Login.AutoSize = true;
            this.Login.Location = new System.Drawing.Point(12, 35);
            this.Login.Name = "Login";
            this.Login.Size = new System.Drawing.Size(33, 13);
            this.Login.TabIndex = 2;
            this.Login.Text = "Login";
            // 
            // LoginBox
            // 
            this.LoginBox.Location = new System.Drawing.Point(78, 32);
            this.LoginBox.Name = "LoginBox";
            this.LoginBox.Size = new System.Drawing.Size(200, 20);
            this.LoginBox.TabIndex = 3;
            // 
            // Password
            // 
            this.Password.AutoSize = true;
            this.Password.Location = new System.Drawing.Point(12, 61);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(53, 13);
            this.Password.TabIndex = 4;
            this.Password.Text = "Password";
            // 
            // PasswordBox
            // 
            this.PasswordBox.Location = new System.Drawing.Point(78, 58);
            this.PasswordBox.Name = "PasswordBox";
            this.PasswordBox.Size = new System.Drawing.Size(200, 20);
            this.PasswordBox.TabIndex = 5;
            // 
            // FilesList
            // 
            this.FilesList.FormattingEnabled = true;
            this.FilesList.Location = new System.Drawing.Point(12, 94);
            this.FilesList.Name = "FilesList";
            this.FilesList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.FilesList.Size = new System.Drawing.Size(266, 212);
            this.FilesList.TabIndex = 6;
            this.FilesList.DoubleClick += new System.EventHandler(this.FilesList_DoubleClick);
            // 
            // List
            // 
            this.List.Location = new System.Drawing.Point(284, 31);
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(75, 20);
            this.List.TabIndex = 7;
            this.List.Text = "List";
            this.List.UseVisualStyleBackColor = true;
            this.List.Click += new System.EventHandler(this.List_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(284, 58);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 20);
            this.Cancel.TabIndex = 8;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(285, 284);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 21);
            this.OK.TabIndex = 9;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // UploadPackagesExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 320);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.List);
            this.Controls.Add(this.FilesList);
            this.Controls.Add(this.PasswordBox);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.LoginBox);
            this.Controls.Add(this.Login);
            this.Controls.Add(this.FtpAdressBox);
            this.Controls.Add(this.FtpAddress);
            this.Name = "UploadPackagesExplorer";
            this.Text = "UploadPackagesExplorer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label FtpAddress;
        private System.Windows.Forms.TextBox FtpAdressBox;
        private System.Windows.Forms.Label Login;
        private System.Windows.Forms.TextBox LoginBox;
        private System.Windows.Forms.Label Password;
        private System.Windows.Forms.MaskedTextBox PasswordBox;
        private System.Windows.Forms.ListBox FilesList;
        private System.Windows.Forms.Button List;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK;
    }
}