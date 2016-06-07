using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace DataPackageTestClient
{
    public partial class DataPackageCallbackServiceClient : Form
    {
        private static string _logtxt;
        private int _start;
        private int _indexOfSearchText;
        private Guid _sid;
        SessionServiceClient.SessionServiceClient _sessionServiceCl;
        DataPackageServiceClient.DataPackageServiceClient _dataPkgServiceCl;

        public DataPackageCallbackServiceClient()
        {
            InitializeComponent();
            if (_logtxt == null)
            {
                lock (_logtxt = "")
                {
                    _logtxt = "";
                }
            }
            this.RefreshTimer.Start();
            _start = 0;
            _indexOfSearchText = 0;

            _sessionServiceCl = new SessionServiceClient.SessionServiceClient();
            _dataPkgServiceCl = new DataPackageServiceClient.DataPackageServiceClient();

            _sid = _sessionServiceCl.Login(System.Configuration.ConfigurationSettings.AppSettings["SessionLogin"], System.Configuration.ConfigurationSettings.AppSettings["SessionPassword"]);
            _sessionServiceCl.SetNotificationInformation(_sid, System.Configuration.ConfigurationSettings.AppSettings["SessionNotification"]);
        }

        ~DataPackageCallbackServiceClient()
        {
            _sessionServiceCl.Logout(_sid);
        }

        #region graphic
        private void Clear_Click(object sender, EventArgs e)
        {
            this.resultTextBox.Clear();
            lock (_logtxt)
            {
                _logtxt = "";
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog lSaveDialog = new SaveFileDialog();
            lSaveDialog.Title = "Save log file";
            lSaveDialog.ShowDialog();

            if (lSaveDialog.FileName != "")
            {
                System.IO.StreamWriter lStrWr = new System.IO.StreamWriter(lSaveDialog.FileName);
                lStrWr.Write(_logtxt);
                lStrWr.Close();
            }   
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
             Save_Click(sender, e);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clear_Click(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.IO.Stream lStream = null;
            OpenFileDialog lOFDial = new OpenFileDialog();
            lOFDial.RestoreDirectory = true;
            if (lOFDial.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((lStream = lOFDial.OpenFile()) != null)
                    {
                        using (lStream)
                        {
                            this.resultTextBox.Clear();
                            System.IO.StreamReader lReader = new System.IO.StreamReader(lStream);
                            lock (_logtxt)
                            {
                                _logtxt += lReader.ReadToEnd();
                                this.resultTextBox.Text = _logtxt;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        
        #endregion graphic
        #region datapackage

        private void GetBaselineListButton_Click(object sender, EventArgs e)
        {
            Application.UseWaitCursor = true;
            DataPackageTestClient.DataPackageServiceClient.GetBaselineListResult lBLRes = _dataPkgServiceCl.getBaselinesList(_sid);

            string lTxt = "------------------------------\n";
            lTxt += "GetBaselineList (" + DateTime.Now.ToString() + ")\n";
            lTxt += "------------------------------\n";
            lTxt += "Error Code : " + lBLRes.error_code + "\n";
            if (lBLRes.error_code == DataPackageTestClient.DataPackageServiceClient.DataPackageErrorEnum.SUCCESS)
            {
                foreach (DataPackageTestClient.DataPackageServiceClient.BaselineDefinition lBLDef in lBLRes.baselineDef)
                {
                    lTxt += "Baseline-" + lBLDef.BaselineVersion + " :\n";
                    lTxt += "\tCreation Date : " + lBLDef.BaselineCreationDate.ToString() + "\n";
                    lTxt += "\tDescription : " + lBLDef.BaselineDescription + "\n";
                    lTxt += "\tPISBase : " + lBLDef.PISBaseDataPackageVersion + "\n";
                    lTxt += "\tPISMission : " + lBLDef.PISMissionDataPackageVersion + "\n";
                    lTxt += "\tPISInfotainment : " + lBLDef.PISInfotainmentDataPackageVersion + "\n";
                    lTxt += "\tLMT : " + lBLDef.LMTDataPackageVersion + "\n";
                }
            }
            lock (_logtxt)
            {
                _logtxt += lTxt;
            }
            Application.UseWaitCursor = false;
        }

        private void GetAdresseesDataPackageBaselinesButton_Click(object sender, EventArgs e)
        {
            DataPackageTestClient.DataPackageServiceClient.TargetAddressType lTAType = new DataPackageTestClient.DataPackageServiceClient.TargetAddressType();
            TargetAdressTypeSelector lTATSel = new TargetAdressTypeSelector();
            lTATSel.ShowDialog(this);
            if (lTATSel._isOk)
            {
                Application.UseWaitCursor = true; 
                lTAType.Type = (DataPackageServiceClient.AddressTypeEnum)Enum.Parse(typeof(DataPackageServiceClient.AddressTypeEnum), (string)lTATSel.AddressTypeList.SelectedItem);
                lTAType.Id = lTATSel.AddressIdBox.Text;

                DataPackageTestClient.DataPackageServiceClient.GetAdresseesDataPackageBaselinesResult lElDescrRes = _dataPkgServiceCl.getAddresseesDataPackagesBaselines(_sid, lTAType);
                

                string lTxt = "-----------------------------------------------\n";
                lTxt += "GetAdresseesDataPackageBaselines (" + DateTime.Now.ToString() + ")\n";
                lTxt += "-----------------------------------------------\n";
                lTxt += "Error Code : " + lElDescrRes.error_code + "\n"; 
                if (lElDescrRes.error_code == DataPackageTestClient.DataPackageServiceClient.DataPackageErrorEnum.SUCCESS)
                {
                    foreach (DataPackageServiceClient.ElementDescription lElDescr in lElDescrRes.ElementDesc)
                    {
                        lTxt += "\tElement Id : " + lElDescr.ElementID + "\n";

                        lTxt += "\t\tElement Archived Baseline : " + lElDescr.ElementArchivedBaseline.BaselineVersion + "\n";
                        lTxt += "\t\tElement Archived Baseline Expiration Date : " + lElDescr.ElementArchivedBaselineExpirationDate.ToString() + "\n";
                        lTxt += "\t\tElement Archived Baseline Validity : " + lElDescr.ElementArchivedBaseline.BaselineValidity.ToString() + "\n";
                        lTxt += "\t\tElement Archived Baseline PISBase Version : " + lElDescr.ElementArchivedBaseline.PISBaseDataPackageVersion + "\n";
                        lTxt += "\t\tElement Archived Baseline PISBase Validity : " + lElDescr.ElementArchivedBaseline.PISBaseDataPackageValidity.ToString() + "\n";
                        lTxt += "\t\tElement Archived Baseline PISMission Version : " + lElDescr.ElementArchivedBaseline.PISMissionDataPackageVersion + "\n";
                        lTxt += "\t\tElement Archived Baseline PISMission Validity : " + lElDescr.ElementArchivedBaseline.PISMissionDataPackageValidity.ToString() + "\n";
                        lTxt += "\t\tElement Archived Baseline PISInfotainment Version : " + lElDescr.ElementArchivedBaseline.PISInfotainmentDataPackageVersion + "\n";
                        lTxt += "\t\tElement Archived Baseline PISInfotainment Validity : " + lElDescr.ElementArchivedBaseline.PISInfotainmentDataPackageValidity.ToString() + "\n";
                        lTxt += "\t\tElement Archived Baseline LMT Version : " + lElDescr.ElementArchivedBaseline.LMTDataPackageVersion + "\n";
                        lTxt += "\t\tElement Archived Baseline LMT Validity : " + lElDescr.ElementArchivedBaseline.LMTDataPackageValidity.ToString() + "\n";


                        lTxt += "\t\tElement Current Baseline : " + lElDescr.ElementCurrentBaseline.BaselineVersion + "\n";
                        lTxt += "\t\tElement Current Baseline Expiration Date : " + lElDescr.ElementCurrentBaselineExpirationDate.ToString() + "\n";
                        lTxt += "\t\tElement Current Baseline is Forced? : " + lElDescr.isCurrentBaselineForced + "\n";
                        lTxt += "\t\tElement Current Baseline Validity : " + lElDescr.ElementCurrentBaseline.BaselineValidity.ToString() + "\n";
                        lTxt += "\t\tElement Current Baseline PISBase Version : " + lElDescr.ElementCurrentBaseline.PISBaseDataPackageVersion + "\n";
                        lTxt += "\t\tElement Current Baseline PISBase Validity : " + lElDescr.ElementCurrentBaseline.PISBaseDataPackageValidity.ToString() + "\n";
                        lTxt += "\t\tElement Current Baseline PISMission Version : " + lElDescr.ElementCurrentBaseline.PISMissionDataPackageVersion + "\n";
                        lTxt += "\t\tElement Current Baseline PISMission Validity : " + lElDescr.ElementCurrentBaseline.PISMissionDataPackageValidity.ToString() + "\n";
                        lTxt += "\t\tElement Current Baseline PISInfotainment Version : " + lElDescr.ElementCurrentBaseline.PISInfotainmentDataPackageVersion + "\n";
                        lTxt += "\t\tElement Current Baseline PISInfotainment Validity : " + lElDescr.ElementCurrentBaseline.PISInfotainmentDataPackageValidity.ToString() + "\n";
                        lTxt += "\t\tElement Current Baseline LMT Version : " + lElDescr.ElementCurrentBaseline.LMTDataPackageVersion + "\n";
                        lTxt += "\t\tElement Current Baseline LMT Validity : " + lElDescr.ElementCurrentBaseline.LMTDataPackageValidity.ToString() + "\n";

                        lTxt += "\t\tElement Future Baseline : " + lElDescr.ElementFutureBaseline.BaselineVersion + "\n";
                        lTxt += "\t\tElement Future Baseline Expiration Date : " + lElDescr.ElementFutureBaselineExpirationDate.ToString() + "\n";
                        lTxt += "\t\tElement Future Baseline Activation Date : " + lElDescr.ElementFutureBaselineActivationDate + "\n";
                        lTxt += "\t\tElement Future Baseline Validity : " + lElDescr.ElementFutureBaseline.BaselineValidity.ToString() + "\n";
                        lTxt += "\t\tElement Future Baseline PISBase Version : " + lElDescr.ElementFutureBaseline.PISBaseDataPackageVersion + "\n";
                        lTxt += "\t\tElement Future Baseline PISBase Validity : " + lElDescr.ElementFutureBaseline.PISBaseDataPackageValidity.ToString() + "\n";
                        lTxt += "\t\tElement Future Baseline PISMission Version : " + lElDescr.ElementFutureBaseline.PISMissionDataPackageVersion + "\n";
                        lTxt += "\t\tElement Future Baseline PISMission Validity : " + lElDescr.ElementFutureBaseline.PISMissionDataPackageValidity.ToString() + "\n";
                        lTxt += "\t\tElement Future Baseline PISInfotainment Version : " + lElDescr.ElementFutureBaseline.PISInfotainmentDataPackageVersion + "\n";
                        lTxt += "\t\tElement Future Baseline PISInfotainment Validity : " + lElDescr.ElementFutureBaseline.PISInfotainmentDataPackageValidity.ToString() + "\n";
                        lTxt += "\t\tElement Future Baseline LMT Version : " + lElDescr.ElementFutureBaseline.LMTDataPackageVersion + "\n";
                        lTxt += "\t\tElement Future Baseline LMT Validity : " + lElDescr.ElementFutureBaseline.LMTDataPackageValidity.ToString() + "\n";

                        lTxt += "\t\tElement Assigned Current Baseline : " + lElDescr.AssignedCurrentBaseline + "\n";
                        lTxt += "\t\tElement Assigned Current Baseline Expiration Date : " + lElDescr.AssignedCurrentBaselineExpirationDate.ToString() + "\n";

                        lTxt += "\t\tElement Assigned Future Baseline : " + lElDescr.AssignedFutureBaseline + "\n";
                        lTxt += "\t\tElement Assigned Future Baseline Expiration Date : " + lElDescr.AssignedFutureBaselineExpirationDate.ToString() + "\n";
                        lTxt += "\t\tElement Assigned Future Baseline Activation Date : " + lElDescr.AssignedFutureBaselineActivationDate + "\n";
                    }
                }
                lock (_logtxt)
                {
                    _logtxt += lTxt;
                }
                Application.UseWaitCursor = false;
            }
        }

        private void UploadDataPackagesButton_Click(object sender, EventArgs e)
        {
            UploadPackagesExplorer lUDPEx = new UploadPackagesExplorer();
            lUDPEx.ShowDialog(this);

            if (lUDPEx._isOk)
	        {
                Application.UseWaitCursor = true;
                List<string> lPkgToUp = new List<string>();
                foreach (string lPkg in lUDPEx._listPackages)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(System.IO.Path.GetFileName(lPkg), @"^(pisbase|pismission|pisinfotainment|lmt)-(\d+\.){3}\d+\.zip$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        lPkgToUp.Add(lPkg);    
                    } 
                }
                DataPackageTestClient.DataPackageServiceClient.DataPackageResult lUpRes = _dataPkgServiceCl.uploadDataPackages(_sid, lPkgToUp.ToArray());
                
                string lTxt = "---------------------------------\n";
                lTxt += "UploadDataPackages (" + DateTime.Now.ToString() + ")\n";
                lTxt += "---------------------------------\n";
                lTxt += "Request Id : " + lUpRes.reqId + "\n";
                lTxt += "Error Code : " + lUpRes.error_code + "\n";
                lock (_logtxt)
                {
                    _logtxt += lTxt;
                }
                Application.UseWaitCursor = false;
            }
        }

        private void DefineNewBaseline_Click(object sender, EventArgs e)
        {
            BaselineDefinitionBox lBlDefBx = new BaselineDefinitionBox();
            lBlDefBx.ShowDialog(this);

            if (lBlDefBx._isOk)
            {
                Application.UseWaitCursor = true;
                DataPackageTestClient.DataPackageServiceClient.DataPackageResult lUpRes = _dataPkgServiceCl.defineNewBaseline(_sid, lBlDefBx._baselineDefinition); ;
                
                string lTxt = "---------------------------------\n";
                lTxt += "DefineNewBaseline (" + DateTime.Now.ToString() + ")\n";
                lTxt += "---------------------------------\n";
                lTxt += "Request Id : " + lUpRes.reqId + "\n";
                lTxt += "Error Code : " + lUpRes.error_code + "\n";
                lock (_logtxt)
                {
                    _logtxt += lTxt;
                }
                Application.UseWaitCursor = false;
            }
        }

        private void AssignFutureBaselineToElement_Click(object sender, EventArgs e)
        {
            AssignFutureBaselineForm lFBlBox = new AssignFutureBaselineForm();
            lFBlBox.ShowDialog(this);
            if (lFBlBox._isOk)
            {
                Application.UseWaitCursor = true;
                DataPackageTestClient.DataPackageServiceClient.TargetAddressType lTAType = new DataPackageTestClient.DataPackageServiceClient.TargetAddressType();
                lTAType.Type = (DataPackageServiceClient.AddressTypeEnum)Enum.Parse(typeof(DataPackageServiceClient.AddressTypeEnum), (string)lFBlBox.AddressTypeList.SelectedItem);
                lTAType.Id = lFBlBox.AddressIdBox.Text;

                DataPackageTestClient.DataPackageServiceClient.DataPackageResult lUpRes = _dataPkgServiceCl.assignFutureBaselineToElement(_sid, lTAType, lFBlBox.VersionBox.Text, lFBlBox.ActivationPicker.Value, lFBlBox.ExpirationPicker.Value);
                
                string lTxt = "---------------------------------\n";
                lTxt += "AssignFutureBaselineToElement (" + DateTime.Now.ToString() + ")\n";
                lTxt += "---------------------------------\n";
                lTxt += "Request Id : " + lUpRes.reqId + "\n";
                lTxt += "Error Code : " + lUpRes.error_code + "\n";
                lock (_logtxt)
                {
                    _logtxt += lTxt;
                }
                Application.UseWaitCursor = false;
            }
        }

        private void AssignCurrentBaselineToElement_Click(object sender, EventArgs e)
        {
            AssignCurrentBaselineForm lCBlBox = new AssignCurrentBaselineForm();
            lCBlBox.ShowDialog(this);
            if (lCBlBox._isOk)
            {
                Application.UseWaitCursor = true;
                DataPackageTestClient.DataPackageServiceClient.TargetAddressType lTAType = new DataPackageTestClient.DataPackageServiceClient.TargetAddressType();
                lTAType.Type = (DataPackageServiceClient.AddressTypeEnum)Enum.Parse(typeof(DataPackageServiceClient.AddressTypeEnum), (string)lCBlBox.AddressTypeList.SelectedItem);
                lTAType.Id = lCBlBox.AddressIdBox.Text;

                DataPackageTestClient.DataPackageServiceClient.DataPackageResult lUpRes = _dataPkgServiceCl.assignCurrentBaselineToElement(_sid, lTAType, lCBlBox.VersionBox.Text, lCBlBox.ExpirationPicker.Value);
                
                string lTxt = "---------------------------------\n";
                lTxt += "AssignCurrentBaselineToElement (" + DateTime.Now.ToString() + ")\n";
                lTxt += "---------------------------------\n";
                lTxt += "Request Id : " + lUpRes.reqId + "\n";
                lTxt += "Error Code : " + lUpRes.error_code + "\n";
                lock (_logtxt)
                {
                    _logtxt += lTxt;
                }
                Application.UseWaitCursor = false;
            }
        }

        private void DistributeBaseline_Click(object sender, EventArgs e)
        {
            DistributeBaselineForm lDistrBl  = new DistributeBaselineForm();
            lDistrBl.ShowDialog();
            if (lDistrBl._isOk)
        	{
                Application.UseWaitCursor = true;
                DataPackageTestClient.DataPackageServiceClient.TargetAddressType lTAType = new DataPackageTestClient.DataPackageServiceClient.TargetAddressType();
                lTAType.Type = (DataPackageServiceClient.AddressTypeEnum)Enum.Parse(typeof(DataPackageServiceClient.AddressTypeEnum), (string)lDistrBl.AddressTypeList.SelectedItem);
                lTAType.Id = lDistrBl.AddressIdBox.Text;
                
                DataPackageTestClient.DataPackageServiceClient.BaselineDistributionAttributes lBlDistrAttr = new DataPackageTestClient.DataPackageServiceClient.BaselineDistributionAttributes();
                lBlDistrAttr.TransferMode = (DataPackageServiceClient.FileTransferMode)Enum.Parse(typeof(DataPackageServiceClient.FileTransferMode), (string)lDistrBl.TransferModeBox.SelectedItem);
                lBlDistrAttr.fileCompression = lDistrBl.FileCompressionBox.Checked;
                lBlDistrAttr.transferDate = lDistrBl.TransferDatePicker.Value;
                lBlDistrAttr.transferExpirationDate = lDistrBl.ExpirationDatePicker.Value;
                lBlDistrAttr.priority = SByte.Parse(lDistrBl.PriorityBox.Text);
                uint lReqTimeOut = UInt32.Parse(lDistrBl.RequestTimeOutBox.Text);
                bool lIncr = lDistrBl.IncrementalBox.Checked;

                DataPackageTestClient.DataPackageServiceClient.DataPackageResult lUpRes = _dataPkgServiceCl.distributeBaseline(_sid, lTAType, lBlDistrAttr, lReqTimeOut, lIncr) ;
                
                string lTxt = "---------------------------------\n";
                lTxt += "DistributeBaseline (" + DateTime.Now.ToString() + ")\n";
                lTxt += "---------------------------------\n";
                lTxt += "Request Id : " + lUpRes.reqId + "\n";
                lTxt += "Error Code : " + lUpRes.error_code + "\n";
                lock (_logtxt)
                {
                    _logtxt += lTxt;
                }
                Application.UseWaitCursor = false;
            }
        }

        private void ForceAddresseesFutureBaseline_Click(object sender, EventArgs e)
        {
            ForceAdressees lFrcAddr = new ForceAdressees();
            lFrcAddr.ShowDialog();
            if (lFrcAddr._isOk)
            {
                Application.UseWaitCursor = true;
                DataPackageTestClient.DataPackageServiceClient.TargetAddressType lTAType = new DataPackageTestClient.DataPackageServiceClient.TargetAddressType();
                lTAType.Type = (DataPackageServiceClient.AddressTypeEnum)Enum.Parse(typeof(DataPackageServiceClient.AddressTypeEnum), (string)lFrcAddr.AddressTypeList.SelectedItem);
                lTAType.Id = lFrcAddr.AddressIdBox.Text;
                uint lReqTimeOut = UInt32.Parse(lFrcAddr.TimeOutBox.Text);

                DataPackageTestClient.DataPackageServiceClient.DataPackageResult lUpRes = _dataPkgServiceCl.forceAddresseesFutureBaseline(_sid, lTAType, lReqTimeOut); ;
                
                string lTxt = "---------------------------------\n";
                lTxt += "ForceAddresseesFutureBaseline (" + DateTime.Now.ToString() + ")\n";
                lTxt += "---------------------------------\n";
                lTxt += "Request Id : " + lUpRes.reqId + "\n";
                lTxt += "Error Code : " + lUpRes.error_code + "\n";
                lock (_logtxt)
                {
                    _logtxt += lTxt;
                }
                Application.UseWaitCursor = false;
            }
        }

        private void ForceAddresseesArchivedBaseline_Click(object sender, EventArgs e)
        {
            ForceAdressees lFrcAddr = new ForceAdressees();
            lFrcAddr.ShowDialog();
            if (lFrcAddr._isOk)
            {
                Application.UseWaitCursor = true;
                DataPackageTestClient.DataPackageServiceClient.TargetAddressType lTAType = new DataPackageTestClient.DataPackageServiceClient.TargetAddressType();
                lTAType.Type = (DataPackageServiceClient.AddressTypeEnum)Enum.Parse(typeof(DataPackageServiceClient.AddressTypeEnum), (string)lFrcAddr.AddressTypeList.SelectedItem);
                lTAType.Id = lFrcAddr.AddressIdBox.Text;
                uint lReqTimeOut = UInt32.Parse(lFrcAddr.TimeOutBox.Text);

                DataPackageTestClient.DataPackageServiceClient.DataPackageResult lUpRes = _dataPkgServiceCl.forceAddresseesArchivedBaseline(_sid, lTAType, lReqTimeOut); ;
                
                string lTxt = "---------------------------------\n";
                lTxt += "ForceAddresseesArchivedBaseline (" + DateTime.Now.ToString() + ")\n";
                lTxt += "---------------------------------\n";
                lTxt += "Request Id : " + lUpRes.reqId + "\n";
                lTxt += "Error Code : " + lUpRes.error_code + "\n";
                lock (_logtxt)
                {
                    _logtxt += lTxt;
                }
                Application.UseWaitCursor = false;
            }
        }
        
        private void ClearAddreeseesForcingStatus_Click(object sender, EventArgs e)
        {
            ForceAdressees lFrcAddr = new ForceAdressees();
            lFrcAddr.ShowDialog();
            if (lFrcAddr._isOk)
            {
                Application.UseWaitCursor = true;
                DataPackageTestClient.DataPackageServiceClient.TargetAddressType lTAType = new DataPackageTestClient.DataPackageServiceClient.TargetAddressType();
                lTAType.Type = (DataPackageServiceClient.AddressTypeEnum)Enum.Parse(typeof(DataPackageServiceClient.AddressTypeEnum), (string)lFrcAddr.AddressTypeList.SelectedItem);
                lTAType.Id = lFrcAddr.AddressIdBox.Text;
                uint lReqTimeOut = UInt32.Parse(lFrcAddr.TimeOutBox.Text);

                DataPackageTestClient.DataPackageServiceClient.DataPackageResult lUpRes = _dataPkgServiceCl.clearAddreeseesForcingStatus(_sid, lTAType, lReqTimeOut); ;
                
                string lTxt = "---------------------------------\n";
                lTxt += "ClearAddreeseesForcingStatus (" + DateTime.Now.ToString() + ")\n";
                lTxt += "---------------------------------\n";
                lTxt += "Request Id : " + lUpRes.reqId + "\n";
                lTxt += "Error Code : " + lUpRes.error_code + "\n";
                lock (_logtxt)
                {
                    _logtxt += lTxt;
                }
                Application.UseWaitCursor = false;
            }
        }


        private void AutoTest_Click(object sender, EventArgs e)
        {
            UploadPackagesExplorer lUDPEx = new UploadPackagesExplorer();
            lUDPEx.ShowDialog(this);

            string lTxt = "";
            DataPackageTestClient.DataPackageServiceClient.DataPackageResult lUpRes = new DataPackageTestClient.DataPackageServiceClient.DataPackageResult();

            if (lUDPEx._isOk)
            {
                Application.UseWaitCursor = true;
                List<string> lPkgToUp = new List<string>();
                List<string> lBLToUp = new List<string>();
                foreach (string lPkg in lUDPEx._listPackages)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(System.IO.Path.GetFileName(lPkg), @"^(pisbase|pismission|pisinfotainment|lmt)-(\d+\.){3}\d+\.zip$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        lPkgToUp.Add(lPkg);
                    }
                    else
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(System.IO.Path.GetFileName(lPkg), @"^baseline-(\d+\.){3}\d+\.xml$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        {
                            lBLToUp.Add(lPkg);
                        }
                    }
                }

                foreach (string lBLfile in lBLToUp)
                {
                    DataPackageTestClient.DataPackageServiceClient.BaselineDefinition lBlDef = new DataPackageTestClient.DataPackageServiceClient.BaselineDefinition();

                    XmlDocument lXmlDiffDoc = new XmlDocument();
                    lXmlDiffDoc.Load(lBLfile);
                    XmlElement lXmlDiffDocRoot = lXmlDiffDoc.DocumentElement;

                    XmlNode lXmlBaselineDef = lXmlDiffDocRoot.SelectSingleNode("//Baseline");
                    foreach (XmlAttribute lAttr in lXmlBaselineDef.Attributes)
                    {
                        switch (lAttr.Name)
                        {
                            case "Version":
                                lBlDef.BaselineVersion = lAttr.Value;
                                break;
                            case "ActivationDate":
                                lBlDef.BaselineCreationDate = DateTime.Parse(lAttr.Value);
                                break;
                            default:
                                break;
                        }
                    }

                    XmlNode lXmlBaselineDescr = lXmlDiffDocRoot.SelectSingleNode("//FileDescription");
                    lBlDef.BaselineDescription = lXmlBaselineDescr.InnerText;

                    XmlNodeList lXmlPackageNodes = lXmlDiffDocRoot.SelectNodes("//Package");
                    foreach (XmlNode lXmlPackageNode in lXmlPackageNodes)
                    {
                        string lDPType = "";
                        string lDPVersion = "";
                        foreach (XmlAttribute lAttr in lXmlPackageNode.Attributes)
                        {
                            switch (lAttr.Name)
                            {
                                case "Name":
                                    lDPType = lAttr.Value.ToUpper();
                                    break;
                                case "Version":
                                    lDPVersion = lAttr.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                        switch (lDPType)
                        {
                            case "PISBASE":
                                lBlDef.PISBaseDataPackageVersion = lDPVersion;
                                break;
                            case "PISMISSION":
                                lBlDef.PISMissionDataPackageVersion = lDPVersion;
                                break;
                            case "LMT":
                                lBlDef.LMTDataPackageVersion = lDPVersion;
                                break;
                            case "PISINFOTAINMENT":
                                lBlDef.PISInfotainmentDataPackageVersion = lDPVersion;
                                break;
                            default:
                                break;
                        }
                    }
                    lUpRes = _dataPkgServiceCl.defineNewBaseline(_sid, lBlDef);
                    lTxt = "---------------------------------\n";
                    lTxt += "DefineNewBaseline (" + DateTime.Now.ToString() + ")\n";
                    lTxt += "---------------------------------\n";
                    lTxt += "Request Id : " + lUpRes.reqId + "\n";
                    lTxt += "Error Code : " + lUpRes.error_code + "\n";
                    lock (_logtxt)
                    {
                        _logtxt += lTxt;
                    }
                }


                lUpRes = _dataPkgServiceCl.uploadDataPackages(_sid, lPkgToUp.ToArray());
                
                lTxt = "---------------------------------\n";
                lTxt += "UploadDataPackages (" + DateTime.Now.ToString() + ")\n";
                lTxt += "---------------------------------\n";
                lTxt += "Request Id : " + lUpRes.reqId + "\n";
                lTxt += "Error Code : " + lUpRes.error_code + "\n";
                lock (_logtxt)
                {
                    _logtxt += lTxt;
                }
                Application.UseWaitCursor = false;
            }
        }

        #endregion datapackage
        #region callbacks
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            lock (_logtxt)
            {
                if (_logtxt != this.resultTextBox.Text)
                {
                    this.resultTextBox.Text = _logtxt;
                    this.resultTextBox.Select(this.resultTextBox.Text.Length, 0);
                    this.resultTextBox.ScrollToCaret();
                }
            }
        }

        #endregion callbacks
        #region textmanagment
        public void RichTextBoxHighlight(object sender, EventArgs e)
        {
            _start = 0;
            _indexOfSearchText = 0;
            
            string lTextToSearch = this.resultTextBox.SelectedText;
            this.resultTextBox.SelectionStart = 0;
            this.resultTextBox.SelectionLength = this.resultTextBox.Text.Length;
            this.resultTextBox.SelectionColor = Color.Black;
            
            int lstartindex = 0;

            while (lstartindex != -1 && this._start < this.resultTextBox.Text.Length)
            {
                if (lTextToSearch.Length > 0)
                    lstartindex = FindMyText(lTextToSearch.Trim(), this._start, this.resultTextBox.Text.Length);

                if (lstartindex >= 0)
                {
                    this.resultTextBox.SelectionColor = Color.Red;
                    int lendindex = lTextToSearch.Length;
                    this.resultTextBox.Select(lstartindex, lendindex);
                    this._start = lstartindex + lendindex;
                }
            }
            this.resultTextBox.Select(this.resultTextBox.Text.Length, 0);
            this.resultTextBox.ScrollToCaret();
        }

        private int FindMyText(string pTxtToSearch, int pSearchStart, int pSearchEnd)
        {
            // Unselect the previously searched string
            if (pSearchStart > 0 && pSearchEnd > 0 && _indexOfSearchText >= 0)
            {
                this.resultTextBox.Undo();
            }

            // Set the return value to -1 by default.
            int lretVal = -1;

            // A valid starting index should be specified.
            // if indexOfSearchText = -1, the end of search
            if (pSearchStart >= 0 && _indexOfSearchText >= 0)
            {
                // A valid ending index
                if (pSearchEnd > pSearchStart || pSearchEnd == -1)
                {
                    // Find the position of search string in RichTextBox
                    _indexOfSearchText = this.resultTextBox.Find(pTxtToSearch, pSearchStart, pSearchEnd, RichTextBoxFinds.None);
                    // Determine whether the text was found in richTextBox1.
                    if (_indexOfSearchText != -1)
                    {
                        // Return the index to the specified search text.
                        lretVal = _indexOfSearchText;
                    }
                }
            }
            return lretVal;
        }

        void RichTextBoxCopy(object sender, EventArgs e)
        {
            this.resultTextBox.Copy();
        }

        private void DataPackageTestForm_Load(object sender, EventArgs e)
        {
            lock (_logtxt)
            {
                if (_logtxt != this.resultTextBox.Text)
                {
                    this.resultTextBox.Text = _logtxt;
                    this.resultTextBox.Select(this.resultTextBox.Text.Length, 0);
                    this.resultTextBox.ScrollToCaret();
                }
            }
            ContextMenu lNewCM = new ContextMenu();
            MenuItem lNewMI = new MenuItem("Highlight");
            lNewMI.Click += new EventHandler(RichTextBoxHighlight);
            lNewCM.MenuItems.Add(lNewMI);

            lNewMI = new MenuItem("Copy");
            lNewMI.Click += new EventHandler(RichTextBoxCopy);
            lNewCM.MenuItems.Add(lNewMI);

            this.resultTextBox.ContextMenu = lNewCM;
        }


        #endregion textmanagment
    }
}
