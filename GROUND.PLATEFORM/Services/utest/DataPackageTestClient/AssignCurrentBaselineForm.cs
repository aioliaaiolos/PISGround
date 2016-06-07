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
    public partial class AssignCurrentBaselineForm : Form
    {
        public bool _isOk { get; set; }

        public AssignCurrentBaselineForm()
        {
            InitializeComponent();
            foreach (string lTAType in Enum.GetNames(typeof(DataPackageServiceClient.AddressTypeEnum)))
            {
                this.AddressTypeList.Items.Add(lTAType);
            }
            this.AddressTypeList.SetSelected(0, true);
            this.AddressIdBox.Text = "TRAIN1";
            this.VersionBox.Text = "1.0.0.0";
            this.ExpirationPicker.Value = DateTime.Now.AddDays(5);
            this.AcceptButton = this.OK;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            _isOk = true;
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            _isOk = false;
            this.Close();
        }
    }
}
