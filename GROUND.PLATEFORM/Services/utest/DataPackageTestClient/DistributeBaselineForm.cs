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
    public partial class DistributeBaselineForm : Form
    {
        public bool _isOk { get; set; }

        public DistributeBaselineForm()
        {
            InitializeComponent();
            foreach (string lTAType in Enum.GetNames(typeof(DataPackageServiceClient.AddressTypeEnum)))
            {
                this.AddressTypeList.Items.Add(lTAType);
            }
            foreach (string lTMode in Enum.GetNames(typeof(DataPackageServiceClient.FileTransferMode)))
            {
                this.TransferModeBox.Items.Add(lTMode);
            }
            this.AddressIdBox.Text = "TRAIN1";
            this.FileCompressionBox.Checked = false;
            this.TransferDatePicker.Value = DateTime.Now;
            this.ExpirationDatePicker.Value = DateTime.Now.AddDays(5);
            this.PriorityBox.Text = "0";
            this.RequestTimeOutBox.Text = "15000";
            this.IncrementalBox.Checked = true;
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
