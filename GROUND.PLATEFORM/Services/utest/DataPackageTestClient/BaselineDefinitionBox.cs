using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DataPackageTestClient
{
    public partial class BaselineDefinitionBox : Form
    {
        public bool _isOk { get; set; }
        public DataPackageTestClient.DataPackageServiceClient.BaselineDefinition _baselineDefinition { get; set; }

        public BaselineDefinitionBox()
        {
            _isOk = false;
            _baselineDefinition = new DataPackageTestClient.DataPackageServiceClient.BaselineDefinition();
            InitializeComponent();
            this.VersionBox.Text = "1.0.0.0";
            this.DescriptionBox.Text = "A new baseline";
            this.CreationDateBox.Text = DateTime.Now.ToString();
            this.PISBaseBox.Text = "1.0.0.0";
            this.PISMissionBox.Text = "1.0.0.0";
            this.PISInfotainmentBox.Text = "1.0.0.0";
            this.LMTBox.Text = "1.0.0.0";
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.Version.Text))
            {
                MessageBox.Show("Empty Baseline Version.");
            }

            _baselineDefinition.BaselineVersion = this.VersionBox.Text;
            _baselineDefinition.BaselineDescription = this.DescriptionBox.Text;
            _baselineDefinition.BaselineCreationDate = DateTime.Parse(this.CreationDateBox.Text);
            _baselineDefinition.PISBaseDataPackageVersion = this.PISBaseBox.Text;
            _baselineDefinition.PISMissionDataPackageVersion = this.PISMissionBox.Text;
            _baselineDefinition.PISInfotainmentDataPackageVersion = this.PISInfotainmentBox.Text;
            _baselineDefinition.LMTDataPackageVersion = this.LMTBox.Text;

            this._isOk = true;
            
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this._isOk = false;
            this.Close();
        }
    }
}
