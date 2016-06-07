namespace PredefLuaScriptChecker
{
    partial class PredefLuaAnalyzerForm
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
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.FileSelectionButton = new System.Windows.Forms.Button();
            this.scriptFilePathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AnalyzeButton = new System.Windows.Forms.Button();
            this.logLabel = new System.Windows.Forms.Label();
            this.OutputTextBox = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "lua";
            this.openFileDialog.FileName = "Predef.lua";
            this.openFileDialog.Filter = "Lua files (*.lua)|*.lua|All files (*.*)|*.*";
            this.openFileDialog.InitialDirectory = "D:\\Data\\PisBase-2.0.0.5\\Scripts\\Background";
            this.openFileDialog.Title = "Open";
            // 
            // FileSelectionButton
            // 
            this.FileSelectionButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.FileSelectionButton.Location = new System.Drawing.Point(518, 38);
            this.FileSelectionButton.Name = "FileSelectionButton";
            this.FileSelectionButton.Size = new System.Drawing.Size(24, 20);
            this.FileSelectionButton.TabIndex = 0;
            this.FileSelectionButton.Text = "...";
            this.FileSelectionButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.FileSelectionButton.UseVisualStyleBackColor = true;
            this.FileSelectionButton.Click += new System.EventHandler(this.FileSelectionButton_Click);
            // 
            // scriptFilePathTextBox
            // 
            this.scriptFilePathTextBox.Location = new System.Drawing.Point(12, 39);
            this.scriptFilePathTextBox.Name = "scriptFilePathTextBox";
            this.scriptFilePathTextBox.ReadOnly = true;
            this.scriptFilePathTextBox.Size = new System.Drawing.Size(500, 20);
            this.scriptFilePathTextBox.TabIndex = 1;
            this.scriptFilePathTextBox.Text = "D:\\Data\\PisBase-2.0.0.5\\Scripts\\Background\\Predef.lua";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Lua Script to analyze :";
            // 
            // AnalyzeButton
            // 
            this.AnalyzeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AnalyzeButton.Location = new System.Drawing.Point(467, 295);
            this.AnalyzeButton.Name = "AnalyzeButton";
            this.AnalyzeButton.Size = new System.Drawing.Size(75, 23);
            this.AnalyzeButton.TabIndex = 3;
            this.AnalyzeButton.Text = "Analyze";
            this.AnalyzeButton.UseVisualStyleBackColor = true;
            this.AnalyzeButton.Click += new System.EventHandler(this.AnalyzeButton_Click);
            // 
            // logLabel
            // 
            this.logLabel.AutoSize = true;
            this.logLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logLabel.Location = new System.Drawing.Point(0, 332);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(0, 13);
            this.logLabel.TabIndex = 4;
            // 
            // OutputTextBox
            // 
            this.OutputTextBox.Location = new System.Drawing.Point(14, 104);
            this.OutputTextBox.Name = "OutputTextBox";
            this.OutputTextBox.Size = new System.Drawing.Size(527, 176);
            this.OutputTextBox.TabIndex = 5;
            this.OutputTextBox.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Output: ";
            // 
            // PredefLuaAnalyzerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 345);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OutputTextBox);
            this.Controls.Add(this.logLabel);
            this.Controls.Add(this.AnalyzeButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.scriptFilePathTextBox);
            this.Controls.Add(this.FileSelectionButton);
            this.Name = "PredefLuaAnalyzerForm";
            this.Text = "Predef lua script analyzer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button FileSelectionButton;
        private System.Windows.Forms.TextBox scriptFilePathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button AnalyzeButton;
        private System.Windows.Forms.Label logLabel;
        private System.Windows.Forms.RichTextBox OutputTextBox;
        private System.Windows.Forms.Label label2;
    }
}

