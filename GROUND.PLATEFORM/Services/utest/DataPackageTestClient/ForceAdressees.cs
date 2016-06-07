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
    public partial class ForceAdressees : Form
    {
        public bool _isOk { get; set; }
        public ForceAdressees()
        {
            InitializeComponent();
            foreach (string lTAType in Enum.GetNames(typeof(DataPackageServiceClient.AddressTypeEnum)))
            {
                this.AddressTypeList.Items.Add(lTAType);
            }
            this.AddressTypeList.SetSelected(0, true);
            this.AddressIdBox.Text = "TRAIN1";
            this.TimeOutBox.Text = "15000";
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
