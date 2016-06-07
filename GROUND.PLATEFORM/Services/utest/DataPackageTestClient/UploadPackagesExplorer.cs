using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace DataPackageTestClient
{
    public partial class UploadPackagesExplorer : Form
    {
        public bool _isOk { get; set; }
        public List<string> _listPackages { get; set; }
        public UploadPackagesExplorer()
        {
            _isOk = false;
            InitializeComponent();
            this.FtpAdressBox.Text = "ftp://127.0.0.1/";
            this.LoginBox.Text = "anonymous";
            this.PasswordBox.Text = "";
        }

        private void List_Click(object sender, EventArgs e)
        {
            if (this.FtpAdressBox.Text.LastIndexOf("/") == this.FtpAdressBox.Text.Length - 1)
            {
                this.FtpAdressBox.Text = this.FtpAdressBox.Text.Substring(0, this.FtpAdressBox.Text.LastIndexOf("/"));
            }
            else
            {
                if (this.FtpAdressBox.Text.LastIndexOf(@"\") == this.FtpAdressBox.Text.Length - 1)
                {
                    this.FtpAdressBox.Text = this.FtpAdressBox.Text.Substring(0, this.FtpAdressBox.Text.LastIndexOf(@"\"));
                }
            }
            this.FtpAdressBox.Refresh();
            if (String.IsNullOrEmpty(this.FtpAdressBox.Text))
            {
                MessageBox.Show(this, "Data packages store adress not define.");
            }
            else
            {
                try
                {
                    Uri lfullURI = new Uri(this.FtpAdressBox.Text);
                    switch (lfullURI.Scheme)
                    {
                        case "file":
                            {
                                string[] lFilesNames = Directory.GetFiles(lfullURI.LocalPath);
                                this.FilesList.Items.Clear();
                                this.FilesList.Refresh();
                                foreach (string lFile in lFilesNames)
                                {
                                    this.FilesList.Items.Add(lFile);
                                }
                            }
                            break;
                        case "ftp":
                            {
                                FtpWebRequest lReq = (FtpWebRequest)WebRequest.Create(lfullURI);
                                lReq.Credentials = new System.Net.NetworkCredential(this.LoginBox.Text, this.PasswordBox.Text);
                                lReq.Method = WebRequestMethods.Ftp.ListDirectory;
                                FtpWebResponse lResp = (FtpWebResponse)lReq.GetResponse();
                                Stream lRespStream = lResp.GetResponseStream();
                                StreamReader lReader = new StreamReader(lRespStream);
                                string lLine;
                                this.FilesList.Items.Clear();
                                this.FilesList.Refresh();
                                while ((lLine = lReader.ReadLine()) != null)
                                {
                                    this.FilesList.Items.Add(lLine);
                                }
                            }
                            break;
                        default:
                            {
                                MessageBox.Show(this, "Not supported uri format.");
                            }
                            break;
                    }
                }
                catch (UriFormatException _uriFormatException)
                {
                    MessageBox.Show(this, "Unknow uri format : {0}", _uriFormatException.ToString());
                }
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            _isOk = false;
            this.Close();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            _isOk = true;
            _listPackages = new List<string>();
            foreach (object lItem in this.FilesList.SelectedItems)
            {
                _listPackages.Add(this.FtpAdressBox.Text + "/" + lItem.ToString());
            }
            this.Close();
        }


        private void FilesList_DoubleClick(object sender, EventArgs e)
        {
            Uri lfullURI = new Uri(this.FtpAdressBox.Text);
            switch (lfullURI.Scheme)
            {
                case "file":
                    {
                        this.FtpAdressBox.Text = this.FtpAdressBox.Text + @"\" + this.FilesList.SelectedItem.ToString();
                        try
                        {
                            string[] lFilesNames = Directory.GetFiles(this.FtpAdressBox.Text);
                            this.FilesList.Items.Clear();
                            this.FilesList.Refresh();
                            foreach (string lFile in lFilesNames)
                            {
                                this.FilesList.Items.Add(lFile.Substring(lFile.IndexOf(@"\")+1));
                            }
                            this.FilesList.Refresh();
                        }
                        catch(DirectoryNotFoundException)
                        {
                            this.FtpAdressBox.Text = lfullURI.AbsoluteUri;
                        }
                    }
                    break;
                case "ftp":
                    {
                        this.FtpAdressBox.Text = this.FtpAdressBox.Text + "/" + this.FilesList.SelectedItem.ToString();
                        FtpWebRequest lReq = (FtpWebRequest)WebRequest.Create(this.FtpAdressBox.Text);
                        lReq.Credentials = new System.Net.NetworkCredential(this.LoginBox.Text, this.PasswordBox.Text);
                        lReq.Method = WebRequestMethods.Ftp.ListDirectory;
                        try
                        {
                            FtpWebResponse lResp = (FtpWebResponse)lReq.GetResponse();
                            Stream lRespStream = lResp.GetResponseStream();
                            StreamReader lReader = new StreamReader(lRespStream);
                            string lLine;
                            this.FilesList.Items.Clear();
                            this.FilesList.Refresh();
                            while ((lLine = lReader.ReadLine()) != null)
                            {
                                this.FilesList.Items.Add(lLine.Substring(lLine.IndexOf("/")+1));
                            }
                            this.FilesList.Refresh();
                        }
                        catch (WebException)
                        {
                            this.FtpAdressBox.Text = lfullURI.AbsoluteUri;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
