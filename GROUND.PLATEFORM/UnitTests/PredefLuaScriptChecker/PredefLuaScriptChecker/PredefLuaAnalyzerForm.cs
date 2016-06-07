using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PIS.Ground.InstantMessage;

namespace PredefLuaScriptChecker
{
    public partial class PredefLuaAnalyzerForm : Form
    {
        public PredefLuaAnalyzerForm()
        {
            InitializeComponent();
        }

        private void FileSelectionButton_Click(object sender, EventArgs e)
        {

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                scriptFilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void AnalyzeButton_Click(object sender, EventArgs e)
        {
            mDoScriptAnalyzis();
        }


        private void mDoScriptAnalyzis()
        {
            logLabel.Text = "Running analysis...";
            OutputTextBox.Clear();
            using (var templateAccessor = new TemplateListAccessor())
            {
                if (templateAccessor.ExecuteTemplate(scriptFilePathTextBox.Text) == true)
                {
                    logLabel.Text = "Script complies with an expected predef.lua script";
                    List<Template> templateList = templateAccessor.GetAllTemplates();
                
                    foreach (Template template in templateList)
                    {
                        string s =  "id = " + template.ID.ToString()                  + " - " + 
                                    "classid = " + template.Class.ToString()               + "\n";
                        OutputTextBox.AppendText(s);
                        
                    }
                }
                else
                {
                    OutputTextBox.AppendText(templateAccessor.GetLastExecutionError());
                    logLabel.Text = "Script DOES NOT comply with an expected predef.lua script";
                }
            }
        }

       
    }
}
