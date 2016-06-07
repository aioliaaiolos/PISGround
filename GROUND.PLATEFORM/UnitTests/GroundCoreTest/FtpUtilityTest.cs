using PIS.Ground.Core.Utility;
using PIS.Ground.Core.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for FtpUtilityTest and is intended
    ///to contain all FtpUtilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FtpUtilityTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for UploadFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadTest_TC_Sw_GroundCore_Classes_0009_A1_A()
        {
            UploadFileDistributionRequest objUploadFileDistributionRequest = null;
            string res = PIS.Ground.Core.Utility.FtpUtility_Accessor.Upload(ref objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for UploadFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadTest_TC_Sw_GroundCore_Classes_0009_A1_B()
        {
            //UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(System.Guid.Empty, "", DateTime.Now, new System.Collections.Generic.List<string>(), true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            //objUploadFileDistributionRequest.FtpUserName = "";
            //string res = PIS.Ground.Core.Utility.FtpUtility_Accessor.Upload(ref objUploadFileDistributionRequest);
            //Assert.AreNotEqual(string.Empty, res);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for UploadFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadTest_TC_Sw_GroundCore_Classes_0009_A1_C()
        {
            //UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(System.Guid.Empty, "", DateTime.Now, new System.Collections.Generic.List<string>(), true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            //objUploadFileDistributionRequest.FtpUserName = "test";
            //objUploadFileDistributionRequest.FtpPassword = "test";
            //objUploadFileDistributionRequest.FtpServerIP = "";
            //string res = PIS.Ground.Core.Utility.FtpUtility_Accessor.Upload(ref objUploadFileDistributionRequest);
            //Assert.AreNotEqual(string.Empty, res);
            Assert.Fail(); //OBSOLETE
        }

          /// <summary>
        ///A test for UploadFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadTest_TC_Sw_GroundCore_Classes_0009_A1_D()
        {
            //UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(System.Guid.Empty, "", DateTime.Now, new System.Collections.Generic.List<string>(), true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            //objUploadFileDistributionRequest.FtpUserName = "test";
            //objUploadFileDistributionRequest.FtpPassword = "";
            //objUploadFileDistributionRequest.FtpServerIP = "test"; ;
            //string res = PIS.Ground.Core.Utility.FtpUtility_Accessor.Upload(ref objUploadFileDistributionRequest);
            //Assert.AreNotEqual(string.Empty, res);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for UploadFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadTest_TC_Sw_GroundCore_Classes_0009_A1_E()
        {
            //UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(System.Guid.Empty, "", DateTime.Now, new System.Collections.Generic.List<string>(), true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            //objUploadFileDistributionRequest.FtpUserName = "test";
            //objUploadFileDistributionRequest.FtpPassword = "test";
            //objUploadFileDistributionRequest.FtpServerIP = "test";
            //string res = PIS.Ground.Core.Utility.FtpUtility_Accessor.Upload(ref objUploadFileDistributionRequest);
            //Assert.AreNotEqual(string.Empty, res);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for UploadFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadTest_TC_Sw_GroundCore_Classes_0009_A1_F()
        {
            //List<string> lst = new List<string>();
            //lst.Add("D:\\Viswanath\\architecture.pdf");
            //UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(System.Guid.Empty, "", DateTime.Now, lst, true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            //objUploadFileDistributionRequest.FtpUserName = "test";
            //objUploadFileDistributionRequest.FtpPassword = "test";
            //objUploadFileDistributionRequest.FtpServerIP = "test";
            //string res = PIS.Ground.Core.Utility.FtpUtility_Accessor.Upload(ref objUploadFileDistributionRequest);
            //Assert.AreNotEqual(string.Empty, res);
            Assert.Fail(); //OBSOLETE
        }


        /// <summary>
        ///A test for DownloadFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadTest_TC_Sw_GroundCore_Classes_0009_A2_A()
        {
            DownloadFolderRequest objDownloadFolderRequest = null;
            string res = PIS.Ground.Core.Utility.FtpUtility_Accessor.Download(ref objDownloadFolderRequest);
            Assert.AreNotEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for Download File
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadTest_TC_Sw_GroundCore_Classes_0009_A2_B()
        {
            //DownloadFolderRequest objDownloadFolderRequest = new DownloadFolderRequest();
            //objDownloadFolderRequest.Folderinfo = null;
            //string res = PIS.Ground.Core.Utility.FtpUtility_Accessor.Download(ref objDownloadFolderRequest);
            //Assert.AreNotEqual(string.Empty, res);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for Download File
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadTest_TC_Sw_GroundCore_Classes_0009_A2_C()
        {
            //DownloadFolderRequest objDownloadFolderRequest = new DownloadFolderRequest();
            //objDownloadFolderRequest.Folderinfo = new FolderInfo(new PIS.Ground.Core.T2G.WebServices.FileTransfer.fileList(),null,"",1,"test","test");
            //string res = PIS.Ground.Core.Utility.FtpUtility_Accessor.Download(ref objDownloadFolderRequest);
            //Assert.AreNotEqual(string.Empty, res);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for Download File
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadTest_TC_Sw_GroundCore_Classes_0009_A2_D()
        {
            //DownloadFolderRequest objDownloadFolderRequest = new DownloadFolderRequest();
            //objDownloadFolderRequest.Folderinfo = new FolderInfo(new PIS.Ground.Core.T2G.WebServices.FileTransfer.fileList(), null, "test", 1, "", "test");
            //string res = PIS.Ground.Core.Utility.FtpUtility_Accessor.Download(ref objDownloadFolderRequest);
            //Assert.AreNotEqual(string.Empty, res);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for Download File
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadTest_TC_Sw_GroundCore_Classes_0009_A2_E()
        {
            //DownloadFolderRequest objDownloadFolderRequest = new DownloadFolderRequest();
            //objDownloadFolderRequest.Folderinfo = new FolderInfo(new PIS.Ground.Core.T2G.WebServices.FileTransfer.fileList(), null, "test", 1, "test", "", null);
            //string res = PIS.Ground.Core.Utility.FtpUtility_Accessor.Download(ref objDownloadFolderRequest);
            //Assert.AreNotEqual(string.Empty, res);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for Download File
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadTest_TC_Sw_GroundCore_Classes_0009_A2_F()
        {
            //DownloadFolderRequest objDownloadFolderRequest = new DownloadFolderRequest();
            //objDownloadFolderRequest.Folderinfo = new FolderInfo(new PIS.Ground.Core.T2G.WebServices.FileTransfer.fileList(), null, "test", 1, "test", "test", null);
            //string res = PIS.Ground.Core.Utility.FtpUtility_Accessor.Download(ref objDownloadFolderRequest);
            //Assert.AreNotEqual(string.Empty, res);
            Assert.Fail(); //OBSOLETE
        }
    }
}
