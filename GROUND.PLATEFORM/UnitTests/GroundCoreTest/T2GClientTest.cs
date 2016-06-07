using PIS.Ground.Core.T2G;
using PIS.Ground.Core.SessionMgmt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.T2G.WebServices.FileTransfer;
namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for T2GClientTest and is intended
    ///to contain all T2GClientTest Unit Tests
    ///</summary>
    [TestClass()]
    public class T2GClientTest
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            PIS.Ground.Core.Utility.T2GConfiguration.InitializeT2GConfig();
        }
        
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
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A1 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetTransferTaskTest_TC_Sw_GroundCore_Classes_0005_A1_A()
        {
            T2GClient testT2GClient = new T2GClient();
            List<Recipient> lstRecipient=new List<Recipient>();
            TransferTaskData objTransferTaskData;
            string str=testT2GClient.GetTransferTask(Guid.Empty, out lstRecipient, out objTransferTaskData);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A1 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetTransferTaskTest_TC_Sw_GroundCore_Classes_0005_A1_B()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "admin", out testguid);
            Guid reqID;
            string res = testSession.GenerateRequestID(testguid, out reqID);
            T2GClient testT2GClient = new T2GClient();
            List<Recipient> lstRecipient = new List<Recipient>();
            TransferTaskData objTransferTaskData;
            string str = testT2GClient.GetTransferTask(reqID, out lstRecipient, out objTransferTaskData);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTest_TC_Sw_GroundCore_Classes_0005_A2_A()
        {
            T2GClient testT2GClient = new T2GClient();
            UploadFileDistributionRequest objUploadFileDistributionRequest = null;
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case b</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTest_TC_Sw_GroundCore_Classes_0005_A2_B()
        {
            T2GClient testT2GClient = new T2GClient();
            UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(new System.Guid(), "test", DateTime.Now, new System.Collections.Generic.List<string>(), true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTest_TC_Sw_GroundCore_Classes_0005_A2_C()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(new System.Guid(), "test", DateTime.Now, lst, true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, -1, null);
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case d</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTest_TC_Sw_GroundCore_Classes_0005_A2_D()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(new System.Guid(), "test", DateTime.Now, lst, true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 35, null);
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case e</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTest_TC_Sw_GroundCore_Classes_0005_A2_E()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(System.Guid.Empty, "test", DateTime.Now, lst, true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case f</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTaskTest_TC_Sw_GroundCore_Classes_0005_A2_F()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(new System.Guid(), "test", DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0)), lst, true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case g</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTaskTest_TC_Sw_GroundCore_Classes_0005_A2_G()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(new System.Guid(), "test", DateTime.Now, lst, true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0)), "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case h</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTaskTest_TC_Sw_GroundCore_Classes_0005_A2_H()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(new System.Guid(), "test", DateTime.Now.AddDays(3).Subtract(new TimeSpan(1, 0, 0, 0, 0)), lst, true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now.AddDays(3), "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case i</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTaskTest_TC_Sw_GroundCore_Classes_0005_A2_I()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\architecture.pdf");
            UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(new System.Guid(), "test", DateTime.Now.AddDays(3), lst, true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case j</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTaskTest_TC_Sw_GroundCore_Classes_0005_A2_J()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            lst.Add("");
            UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(new System.Guid(), "test", DateTime.Now.AddDays(3), lst, true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case k</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTest_TC_Sw_GroundCore_Classes_0005_A2_K()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            System.Collections.Generic.List<RecipientId> rec = new System.Collections.Generic.List<RecipientId>();
            RecipientId rp = new RecipientId();
            rp.SystemId = "TRAIN";
            rec.Add(rp);
            UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(new System.Guid(), "test", DateTime.Now.AddDays(3), lst, true, rec, DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case l</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTest_TC_Sw_GroundCore_Classes_0005_A2_L()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            System.Collections.Generic.List<RecipientId> rec = new System.Collections.Generic.List<RecipientId>();
            RecipientId rp = new RecipientId();
            rp.SystemId = "TRAIN";
            rec.Add(rp);
            UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(new System.Guid(), "test", DateTime.Now.AddDays(3), lst, true, rec, DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }


        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case m</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTaskTest_TC_Sw_GroundCore_Classes_0005_A2_M()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            System.Collections.Generic.List<RecipientId> rec = new System.Collections.Generic.List<RecipientId>();
            UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(new System.Guid(), "test", DateTime.Now.AddDays(3), lst, true, rec, DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case n</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void UploadFileTest_TC_Sw_GroundCore_Classes_0005_A2_N()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            System.Collections.Generic.List<RecipientId> rec = new System.Collections.Generic.List<RecipientId>();
            RecipientId rp = new RecipientId();
            rec.Add(rp);
            UploadFileDistributionRequest objUploadFileDistributionRequest = new UploadFileDistributionRequest(new System.Guid(), "test", DateTime.Now.AddDays(3), lst, true, new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.UploadFile(objUploadFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for DownloadFile
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A3 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadFileTest_TC_Sw_GroundCore_Classes_0005_A3_A()
        {
            T2GClient testT2GClient = new T2GClient();
            DownloadFileDistributionRequest objFileDistributionRequest = null;
            string str = testT2GClient.DownLoadFile(objFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for DownloadFile
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case b</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadFileTest_TC_Sw_GroundCore_Classes_0005_A3_B()
        {
            T2GClient testT2GClient = new T2GClient();
            DownloadFileDistributionRequest objFileDistributionRequest = new DownloadFileDistributionRequest(new System.Guid(), "", DateTime.Now, new System.Collections.Generic.List<string>(), true, "test", 1, "test", "test", "", new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.DownLoadFile(objFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for DownloadFile
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A3 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadFileTest_TC_Sw_GroundCore_Classes_0005_A3_C()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            DownloadFileDistributionRequest objFileDistributionRequest = new DownloadFileDistributionRequest(new System.Guid(), "test", DateTime.Now, new System.Collections.Generic.List<string>(), true, "", 1, "test", "test", "", new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.DownLoadFile(objFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for DownloadFile
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A3 : case d</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadFileTest_TC_Sw_GroundCore_Classes_0005_A3_D()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            DownloadFileDistributionRequest objFileDistributionRequest = new DownloadFileDistributionRequest(new System.Guid(), "test", DateTime.Now, new System.Collections.Generic.List<string>(), true, "test", 0, "test", "test", "", new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.DownLoadFile(objFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for DownloadFile
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A3 : case e</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadFileTest_TC_Sw_GroundCore_Classes_0005_A2_E()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            DownloadFileDistributionRequest objFileDistributionRequest = new DownloadFileDistributionRequest(new System.Guid(), "test", DateTime.Now, new System.Collections.Generic.List<string>(), true, "test", 1, "", "test", "", new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.DownLoadFile(objFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for DownloadFile
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case f</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadFileTest_TC_Sw_GroundCore_Classes_0005_A3_F()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            DownloadFileDistributionRequest objFileDistributionRequest = new DownloadFileDistributionRequest(new System.Guid(), "test", DateTime.Now, new System.Collections.Generic.List<string>(), true, "test", 1, "test", "", "", new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.DownLoadFile(objFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for DownloadFile
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case g</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadFileTest_TC_Sw_GroundCore_Classes_0005_A3_G()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            DownloadFileDistributionRequest objFileDistributionRequest = new DownloadFileDistributionRequest(new System.Guid(), "test", DateTime.Now, new System.Collections.Generic.List<string>(), true, "", 1, "test", "test", "", new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.DownLoadFile(objFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case h</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadFileTest_TC_Sw_GroundCore_Classes_0005_A3_H()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            DownloadFileDistributionRequest objFileDistributionRequest = new DownloadFileDistributionRequest(new System.Guid(), "test", DateTime.Now, new System.Collections.Generic.List<string>(), true, "", 1, "test", "test", "", new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.DownLoadFile(objFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for DownloadFile
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case i</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadFileTest_TC_Sw_GroundCore_Classes_0005_A3_I()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\architecture.pdf");
            DownloadFileDistributionRequest objFileDistributionRequest = new DownloadFileDistributionRequest(System.Guid.Empty, "test", DateTime.Now, new System.Collections.Generic.List<string>(), true, "", 1, "test", "test", "", new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.DownLoadFile(objFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for DownloadFile
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case j</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadFileTest_TC_Sw_GroundCore_Classes_0005_A2_J()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            lst.Add("");
            DownloadFileDistributionRequest objFileDistributionRequest = new DownloadFileDistributionRequest(new System.Guid(), "test", DateTime.Now, new System.Collections.Generic.List<string>(), true, "", 1, "test", "test", "", new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.DownLoadFile(objFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case k</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadFileTest_TC_Sw_GroundCore_Classes_0005_A3_K()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            System.Collections.Generic.List<RecipientId> rec = new System.Collections.Generic.List<RecipientId>();
            RecipientId rp = new RecipientId();
            rp.SystemId = "TRAIN";
            rec.Add(rp);
            DownloadFileDistributionRequest objFileDistributionRequest = new DownloadFileDistributionRequest(new System.Guid(), "test", DateTime.Now, new System.Collections.Generic.List<string>(), true, "", 1, "test", "test", "", new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.DownLoadFile(objFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for DownloadFile
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A2 : case l</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownloadFileTest_TC_Sw_GroundCore_Classes_0005_A3_L()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst = new List<string>();
            lst.Add("D:\\Viswanath\\architecture.pdf");
            System.Collections.Generic.List<RecipientId> rec = new System.Collections.Generic.List<RecipientId>();
            RecipientId rp = new RecipientId();
            rp.SystemId = "TRAIN";
            rec.Add(rp);
            DownloadFileDistributionRequest objFileDistributionRequest = new DownloadFileDistributionRequest(new System.Guid(), "test", DateTime.Now, new System.Collections.Generic.List<string>(), true, "", 1, "test", "test", "", new System.Collections.Generic.List<RecipientId>(), DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);
            string str = testT2GClient.DownLoadFile(objFileDistributionRequest);
            Assert.AreNotEqual(string.Empty, str);
        }

        /// <summary>
        ///A test for GetAllElementDataList
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A4 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAllElementDataListTest_TC_Sw_GroundCore_Classes_0005_A4_A()
        {
            T2GClient testT2GClient = new T2GClient();
            Dictionary<string,List<FieldInfo>> lst = testT2GClient.GetAllElementDataList("");
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetAllElementDataList
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A4 : case b</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAllElementDataListTest_TC_Sw_GroundCore_Classes_0005_A4_B()
        {
            T2GClient testT2GClient = new T2GClient();
            Dictionary<string, List<FieldInfo>> lst = testT2GClient.GetAllElementDataList("TRAIN1");
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetAllElementDataList
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A4 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAllElementDataListFileTest_TC_Sw_GroundCore_Classes_0005_A4_C()
        {
            T2GClient testT2GClient = new T2GClient();
            Dictionary<string, List<FieldInfo>> lst = testT2GClient.GetAllElementDataList("TRAIN");
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetAllElementDataList
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A5 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataListTest_TC_Sw_GroundCore_Classes_0005_A5_A()
        {
            T2GClient testT2GClient = new T2GClient();
            ElementList<AvailableElementData> lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataList(out lst);
            Assert.AreEqual(0, lst.Count);
        }

        /// <summary>
        ///A test for GetAllElementDataList
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A5 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataListTest_TC_Sw_GroundCore_Classes_0005_A5_B()
        {
            T2GClient testT2GClient = new T2GClient();
            ElementList<AvailableElementData> lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataList(out lst);
            Assert.AreEqual(0, lst.Count);
        }

        /// <summary>
        ///A test for GetAvailableElementDataListByMissionCode
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A4 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataListByMissionCodeTest_TC_Sw_GroundCore_Classes_0005_A6_A()
        {
            T2GClient testT2GClient = new T2GClient();
            ElementList<AvailableElementData> lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataListByMissionCode("", out lst);
            Assert.AreEqual(0, lst.Count);
        }

        /// <summary>
        ///A test for GetAvailableElementDataListByMissionCode
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A6 : case b</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataListByMissionCodeTest_TC_Sw_GroundCore_Classes_0005_A6_B()
        {
            T2GClient testT2GClient = new T2GClient();
            ElementList<AvailableElementData> lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataListByMissionCode("TRAIN1", out lst);
            Assert.AreEqual(0, lst.Count);
        }

        /// <summary>
        ///A test for GetAllElementDataList
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A6 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataListByMissionCodeTest_TC_Sw_GroundCore_Classes_0005_A6_C()
        {
            T2GClient testT2GClient = new T2GClient();
            ElementList<AvailableElementData> lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataListByMissionCode("TRAIN", out lst);
            Assert.AreEqual(0, lst.Count);
        }

        /// <summary>
        ///A test for GetAvailableElementDataListByMissionOperatorCode
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A4 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataListByMissionOperatorCodeTest_TC_Sw_GroundCore_Classes_0005_A7_A()
        {
            T2GClient testT2GClient = new T2GClient();
            ElementList<AvailableElementData> lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataListByMissionOperatorCode("", out lst);
            Assert.AreEqual(0, lst.Count);
        }

        /// <summary>
        ///A test for GetAvailableElementDataListByMissionOperatorCode
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A6 : case b</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataListByMissionOperatorCodeTest_TC_Sw_GroundCore_Classes_0005_A7_B()
        {
            T2GClient testT2GClient = new T2GClient();
            ElementList<AvailableElementData> lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataListByMissionOperatorCode("TRAIN1", out lst);
            Assert.AreEqual(0, lst.Count);
        }

        /// <summary>
        ///A test for GetAvailableElementDataListByMissionOperatorCode
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A6 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataListByMissionCodeTest_TC_Sw_GroundCore_Classes_0005_A7_C()
        {
            T2GClient testT2GClient = new T2GClient();
            ElementList<AvailableElementData> lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataListByMissionOperatorCode("TRAIN", out lst);
            Assert.AreEqual(0, lst.Count);
        }

        /// <summary>
        ///A test for GetAvailableElementDataByElementNumber
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A8 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataByElementNumberTest_TC_Sw_GroundCore_Classes_0005_A8_A()
        {
            T2GClient testT2GClient = new T2GClient();
            AvailableElementData lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataByElementNumber("", out lst);
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetAvailableElementDataByElementNumber
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A8 : case b</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataByElementNumberTest_TC_Sw_GroundCore_Classes_0005_A8_B()
        {
            T2GClient testT2GClient = new T2GClient();
            AvailableElementData lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataByElementNumber("TRAIN1", out lst);
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetAvailableElementDataByElementNumber
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A8 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataByElementNumberTest_TC_Sw_GroundCore_Classes_0005_A8_C()
        {
            T2GClient testT2GClient = new T2GClient();
            AvailableElementData lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataByElementNumber("TRAIN", out lst);
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for IsElementOnline
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A9 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void IsElementOnlineTest_TC_Sw_GroundCore_Classes_0005_A9_A()
        {
            T2GClient testT2GClient = new T2GClient();
            bool lst;
            T2GConnectorErrorEnum lRs = testT2GClient.IsElementOnline("", out lst);
            Assert.AreEqual(false, lst);
        }

        /// <summary>
        ///A test for IsElementOnline
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A9 : case b</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void IsElementOnlineTest_TC_Sw_GroundCore_Classes_0005_A9_B()
        {
            T2GClient testT2GClient = new T2GClient();
            bool lst;
            T2GConnectorErrorEnum lRs = testT2GClient.IsElementOnline("TRAIN1", out lst);
            Assert.AreEqual(false, lst);
        }

        /// <summary>
        ///A test for IsElementOnline
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A9 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void IsElementOnlineTest_TC_Sw_GroundCore_Classes_0005_A9_C()
        {
            T2GClient testT2GClient = new T2GClient();
            bool lst;
            T2GConnectorErrorEnum lRs = testT2GClient.IsElementOnline("TRAIN", out lst);
            Assert.AreEqual(false, lst);
        }

        /// <summary>
        ///A test for GetAvailableElementDataListByMissionOperatorCode
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A10 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataForSystemIDTest_TC_Sw_GroundCore_Classes_0005_A10_A()
        {
            T2GClient testT2GClient = new T2GClient();
            AvailableElementData lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataForSystemID("", out lst);
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetAvailableElementDataForSystemID
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A10 : case b</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataForSystemIDTest_TC_Sw_GroundCore_Classes_0005_A10_B()
        {
            T2GClient testT2GClient = new T2GClient();
            AvailableElementData lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataForSystemID("TRAIN1", out lst);
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetAvailableElementDataForSystemID
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A10 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetAvailableElementDataForSystemIDTest_TC_Sw_GroundCore_Classes_0005_A10_C()
        {
            T2GClient testT2GClient = new T2GClient();
            AvailableElementData lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetAvailableElementDataForSystemID("TRAIN", out lst);
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetSystemIDList
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A11 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetSystemIDListTest_TC_Sw_GroundCore_Classes_0005_A11_A()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetSystemIDList( out lst);
            Assert.AreEqual(0, lst.Count);
        }

        /// <summary>
        ///A test for GetSystemIDList
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A11 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetSystemIDListTest_TC_Sw_GroundCore_Classes_0005_A11_B()
        {
            T2GClient testT2GClient = new T2GClient();
            List<string> lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetSystemIDList( out lst);
            Assert.AreEqual(0, lst.Count);
        }

        /// <summary>
        ///A test for GetSystemData
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetSystemDataTest_TC_Sw_GroundCore_Classes_0005_A12_A()
        {
            T2GClient testT2GClient = new T2GClient();
            SystemInfo lst = testT2GClient.GetSystemData("");
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetSystemData
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case b</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetSystemDataTest_TC_Sw_GroundCore_Classes_0005_A12_B()
        {
            T2GClient testT2GClient = new T2GClient();
            SystemInfo lst = testT2GClient.GetSystemData("TRAIN1");
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetSystemData
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetSystemDataTest_TC_Sw_GroundCore_Classes_0005_A12_C()
        {
            T2GClient testT2GClient = new T2GClient();
            SystemInfo lst = testT2GClient.GetSystemData("TRAIN");
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetServiceIDNameList
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A13 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetServiceIDNameListTest_TC_Sw_GroundCore_Classes_0005_A13_A()
        {
            T2GClient testT2GClient = new T2GClient();
            Dictionary<ushort,string> lst = testT2GClient.GetServiceIDNameList("");
            Assert.AreEqual(0, lst.Count);
        }

        /// <summary>
        ///A test for GetServiceIDNameList
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A13 : case b</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetServiceIDNameListTest_TC_Sw_GroundCore_Classes_0005_A13_B()
        {
            T2GClient testT2GClient = new T2GClient();
            Dictionary<ushort, string> lst = testT2GClient.GetServiceIDNameList("TRAIN1");
            Assert.AreEqual(0, lst.Count);
        }

        /// <summary>
        ///A test for GetServiceIDNameList
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A13 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetServiceIDNameListTest_TC_Sw_GroundCore_Classes_0005_A13_C()
        {
            T2GClient testT2GClient = new T2GClient();
            Dictionary<ushort, string> lst = testT2GClient.GetServiceIDNameList("TRAIN");
            Assert.AreEqual(0, lst.Count);
        }

        /// <summary>
        ///A test for GetServiceData
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetServiceDataTest_TC_Sw_GroundCore_Classes_0005_A14_A()
        {
            T2GClient testT2GClient = new T2GClient();
            ServiceInfo lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetServiceData("", 0, out lst);
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetServiceData
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case b</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetServiceDataTest_TC_Sw_GroundCore_Classes_0005_A14_B()
        {
            T2GClient testT2GClient = new T2GClient();
            ServiceInfo lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetServiceData("TRAIN1", 0, out lst);
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetServiceData
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetServiceDataTest_TC_Sw_GroundCore_Classes_0005_A14_C()
        {
            T2GClient testT2GClient = new T2GClient();
            ServiceInfo lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetServiceData("TRAIN", 0, out lst);
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetServiceData
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetServiceDataTest_TC_Sw_GroundCore_Classes_0005_A14_D()
        {
            T2GClient testT2GClient = new T2GClient();
            ServiceInfo lst;
            T2GConnectorErrorEnum lRs = testT2GClient.GetServiceData("TRAIN", 1, out lst);
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for SubscribeFileRecievedNotification
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeFileRecievedNotificationTest_TC_Sw_GroundCore_Classes_0005_A15_A()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeFileRecievedNotification(null, null );
            Assert.AreNotEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeFileRecievedNotification
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeFileRecievedNotificationTest_TC_Sw_GroundCore_Classes_0005_A15_B()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeFileRecievedNotification("test", null);
            Assert.AreNotEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeFileRecievedNotification
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeFileRecievedNotificationTest_TC_Sw_GroundCore_Classes_0005_A15_C()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeFileRecievedNotification("test", new EventHandler<FileReceivedArgs>(OnFileRecChanged));
            Assert.AreEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeFileRecievedNotification
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeFileRecievedNotificationTest_TC_Sw_GroundCore_Classes_0005_A15_D()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeFileRecievedNotification("test", new EventHandler<FileReceivedArgs>(OnFileRecChanged));
            Assert.AreEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeFilePublishedNotification 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeFilePublishedNotificationTest_TC_Sw_GroundCore_Classes_0005_A16_A()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeFilePublishedNotification(null, null);
            Assert.AreNotEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeFilePublishedNotification 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeFilePublishedNotificationTest_TC_Sw_GroundCore_Classes_0005_A16_B()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeFilePublishedNotification("test", null);
            Assert.AreNotEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeFilePublishedNotification 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeFilePublishedNotificationTest_TC_Sw_GroundCore_Classes_0005_A16_C()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeFilePublishedNotification("test", new EventHandler<FilePublishedNotificationArgs>(OnFilePubChanged));
            Assert.AreEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeFilePublishedNotification 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeFilePublishedNotificationTest_TC_Sw_GroundCore_Classes_0005_A16_D()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeFilePublishedNotification("test", new EventHandler<FilePublishedNotificationArgs>(OnFilePubChanged));
            Assert.AreEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeFilePublicationNotification  
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeFilePublicationNotificationTest_TC_Sw_GroundCore_Classes_0005_A17_A()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeFilePublicationNotification(null, null);
            Assert.AreNotEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeFilePublicationNotification  
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A12 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeFilePublicationNotificationTest_TC_Sw_GroundCore_Classes_0005_A17_B()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeFilePublicationNotification("test", null);
            Assert.AreNotEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeFilePublicationNotification  
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A17 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeFilePublicationNotificationTest_TC_Sw_GroundCore_Classes_0005_A17_C()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeFilePublicationNotification("test", new EventHandler<FilePublicationNotificationArgs>(OnFilePubChanged));
            Assert.AreEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeFilePublicationNotification  
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A17 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeFilePublicationNotificationTest_TC_Sw_GroundCore_Classes_0005_A17_D()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeFilePublicationNotification("test", new EventHandler<FilePublicationNotificationArgs>(OnFilePubChanged));
            Assert.AreEqual(string.Empty, lst);
        }


        /// <summary>
        ///A test for SubscribeElementChangeNotification   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A18 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeElementChangeNotificationTest_TC_Sw_GroundCore_Classes_0005_A18_A()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeElementChangeNotification(null, null);
            Assert.AreNotEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeElementChangeNotification   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A18 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeElementChangeNotificationTest_TC_Sw_GroundCore_Classes_0005_A18_B()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeElementChangeNotification("test", null);
            Assert.AreNotEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeElementChangeNotification   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A18 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeElementChangeNotificationTest_TC_Sw_GroundCore_Classes_0005_A18_C()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeElementChangeNotification("test", new EventHandler<ElementEventArgs>(OnElementInfoChanged));
            Assert.AreEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for SubscribeElementChangeNotification   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A18 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void SubscribeElementChangeNotificationTest_TC_Sw_GroundCore_Classes_0005_A18_D()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.SubscribeElementChangeNotification("test", new EventHandler<ElementEventArgs>(OnElementInfoChanged));
            Assert.AreEqual(string.Empty, lst);
        }

        private static void OnFileRecChanged(object pSender, FileReceivedArgs pElement)
        {

        }

        private static void OnFilePubChanged(object pSender, FilePublicationNotificationArgs pElement)
        {

        }

        private static void OnFilePubChanged(object pSender, FilePublishedNotificationArgs pElement)
        {

        }

        private static void OnElementInfoChanged(object pSender, ElementEventArgs pElement)
        {

        }

        /// <summary>
        ///A test for DownLoadFolder   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownLoadFolderTest_TC_Sw_GroundCore_Classes_0005_A19_A()
        {
            T2GClient testT2GClient = new T2GClient();
            string lst = testT2GClient.DownLoadFolder(null);
            Assert.AreNotEqual(string.Empty, lst);
        }


        /// <summary>
        ///A test for DownLoadFolder   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownLoadFolderTest_TC_Sw_GroundCore_Classes_0005_A19_B()
        {
            T2GClient testT2GClient = new T2GClient();
            DownloadFolderRequest obj = new DownloadFolderRequest();
            obj.RequestId = new Guid();
            obj.DownloadFolderPath = "";
            obj.ExpirationDate = DateTime.Now.AddDays(1);
            string lst = testT2GClient.DownLoadFolder(obj);
            Assert.AreNotEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for DownLoadFolder   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownLoadFolderTest_TC_Sw_GroundCore_Classes_0005_A19_C()
        {
            T2GClient testT2GClient = new T2GClient();
            DownloadFolderRequest obj = new DownloadFolderRequest();
            obj.RequestId = Guid.Empty;
            obj.DownloadFolderPath = "test";
            obj.ExpirationDate = DateTime.Now.AddDays(1);
            string lst = testT2GClient.DownLoadFolder(obj);
            Assert.AreNotEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for DownLoadFolder   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownLoadFolderTest_TC_Sw_GroundCore_Classes_0005_A19_D()
        {
            T2GClient testT2GClient = new T2GClient();
            DownloadFolderRequest obj = new DownloadFolderRequest();
            obj.RequestId = new Guid();
            obj.DownloadFolderPath = "test";
            obj.ExpirationDate = DateTime.Now.Subtract(new TimeSpan(5,0,0));
            string lst = testT2GClient.DownLoadFolder(obj);
            Assert.AreNotEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for DownLoadFolder   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownLoadFolderTest_TC_Sw_GroundCore_Classes_0005_A19_E()
        {
            T2GClient testT2GClient = new T2GClient();
            DownloadFolderRequest obj = new DownloadFolderRequest();
            obj.RequestId = new Guid();
            obj.DownloadFolderPath = "test";
            obj.ExpirationDate = DateTime.Now.AddDays(1);
            string lst = testT2GClient.DownLoadFolder(obj);
            Assert.AreNotEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for DownLoadFolder   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DownLoadFolderTest_TC_Sw_GroundCore_Classes_0005_A19_F()
        {
            T2GClient testT2GClient = new T2GClient();
            DownloadFolderRequest obj = new DownloadFolderRequest();
            obj.RequestId = new Guid();
            obj.DownloadFolderPath = "test";
            obj.ExpirationDate = DateTime.Now.AddDays(1);
            string lst = testT2GClient.DownLoadFolder(obj);
            Assert.AreNotEqual(string.Empty, lst);
        }

        /// <summary>
        ///A test for GetRequestIdByTaskId   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetRequestIdByTaskIdTest_TC_Sw_GroundCore_Classes_0005_A20_A()
        {
            Guid lst = T2GClient.GetRequestIdByTaskId(0);
            Assert.AreEqual(Guid.Empty, lst);
        }

        /// <summary>
        ///A test for GetRequestIdByTaskId   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetRequestIdByTaskIdTest_TC_Sw_GroundCore_Classes_0005_A20_B()
        {
             Guid lst = T2GClient.GetRequestIdByTaskId(1);
            Assert.AreEqual(Guid.Empty, lst);
        }

        /// <summary>
        ///A test for GetFileDistributionEventByTaskId   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetFileDistributionEventByTaskIdTest_TC_Sw_GroundCore_Classes_0005_A21_A()
        {
            EventHandler<FileDistributionStatusArgs> lst = T2GClient.GetFileDistributionEventByTaskId(0);
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetFileDistributionEventByTaskId   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetFileDistributionEventByTaskIdTest_TC_Sw_GroundCore_Classes_0005_A21_B()
        {
            EventHandler<FileDistributionStatusArgs> lst = T2GClient.GetFileDistributionEventByTaskId(1);
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetFileDistributionEventByRequestId   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetFileDistributionEventByRequestIdTest_TC_Sw_GroundCore_Classes_0005_A22_A()
        {
            EventHandler<FileDistributionStatusArgs> lst = T2GClient.GetFileDistributionEventByRequestId(Guid.Empty);
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetFileDistributionEventByRequestId   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetFileDistributionEventByRequestIdTest_TC_Sw_GroundCore_Classes_0005_A22_B()
        {
            EventHandler<FileDistributionStatusArgs> lst = T2GClient.GetFileDistributionEventByRequestId(new Guid());
            Assert.AreEqual(null, lst);
        }

        /// <summary>
        ///A test for GetRequestIdByTaskId   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetRequestIdByFolderIdTest_TC_Sw_GroundCore_Classes_0005_A23_A()
        {
            Guid lst = T2GClient.GetRequestIdByFolderId(0);
            Assert.AreEqual(Guid.Empty, lst);
        }

        /// <summary>
        ///A test for GetRequestIdByFolderId   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetRequestIdByFolderIdTest_TC_Sw_GroundCore_Classes_0005_A23_B()
        {
            Guid lst = T2GClient.GetRequestIdByFolderId(1);
            Assert.AreEqual(Guid.Empty, lst);
        }

        /// <summary>
        ///A test for RaiseOnElementInfoChangeEvent   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void RaiseOnElementInfoChangeEventTest_TC_Sw_GroundCore_Classes_0005_A24_A()
        {
            bool lst = new T2GClient().RaiseOnElementInfoChangeEvent(null);
            Assert.AreEqual(false, lst);
        }


        /// <summary>
        ///A test for RaiseOnElementInfoChangeEvent   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void RaiseOnElementInfoChangeEventTest_TC_Sw_GroundCore_Classes_0005_A24_B()
        {
            bool lst = new T2GClient().RaiseOnElementInfoChangeEvent(new ElementEventArgs());
            Assert.AreNotEqual(false, lst);
        }

        /// <summary>
        ///A test for RaiseOnFileReceivedNotificationEvent   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void RaiseOnFileReceivedNotificationEventTest_TC_Sw_GroundCore_Classes_0005_A25_A()
        {
            bool lst = new T2GClient().RaiseOnFileReceivedNotificationEvent(null);
            Assert.AreEqual(false, lst);
        }


        /// <summary>
        ///A test for RaiseOnFileReceivedNotificationEvent   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void RaiseOnFileReceivedNotificationEventTest_TC_Sw_GroundCore_Classes_0005_A25_B()
        {
            bool lst = new T2GClient().RaiseOnFileReceivedNotificationEvent(new FileReceivedArgs());
            Assert.AreNotEqual(false, lst);
        }

        /// <summary>
        ///A test for RaiseOnFileDistributeNotificationEvent   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void RaiseOnFileDistributeNotificationEventTest_TC_Sw_GroundCore_Classes_0005_A26_A()
        {
            bool lst = new T2GClient().RaiseOnFileDistributeNotificationEvent(null, 1);
            Assert.AreEqual(false, lst);
        }


        /// <summary>
        ///A test for RaiseOnFileDistributeNotificationEvent   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void RaiseOnFileDistributeNotificationEventTest_TC_Sw_GroundCore_Classes_0005_A26_B()
        {
            bool lst = new T2GClient().RaiseOnFileDistributeNotificationEvent(new FileDistributionStatusArgs(), 0);
            Assert.AreEqual(false, lst);
        }

        /// <summary>
        ///A test for RaiseOnFileDistributeNotificationEvent   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void RaiseOnFileDistributeNotificationEventTest_TC_Sw_GroundCore_Classes_0005_A26_C()
        {
            bool lst = new T2GClient().RaiseOnFileDistributeNotificationEvent(new FileDistributionStatusArgs(), 1);
            Assert.AreNotEqual(false, lst);
        }

        /// <summary>
        ///A test for RaiseOnFilePublishedNotificationEvent   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void RaiseOnFilePublishedNotificationEventTest_TC_Sw_GroundCore_Classes_0005_A27_A()
        {
            bool lst = new T2GClient().RaiseOnFilePublishedNotificationEvent(null);
            Assert.AreEqual(false, lst);
        }


        /// <summary>
        ///A test for RaiseOnFilePublishedNotificationEvent   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void RaiseOnFilePublishedNotificationEventTest_TC_Sw_GroundCore_Classes_0005_A27_B()
        {
            bool lst = new T2GClient().RaiseOnFilePublishedNotificationEvent(new FilePublishedNotificationArgs());
            Assert.AreNotEqual(false, lst);
        }

        /// <summary>
        ///A test for GetFolderInformation   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetFolderInformationTest_TC_Sw_GroundCore_Classes_0005_A28_A()
        {
            string error;
            FolderInfo lst = new T2GClient().GetFolderInformation(-1,out error);
            Assert.AreEqual(null, lst);
            Assert.AreEqual(string.Empty, error);
        }


        /// <summary>
        ///A test for GetFolderInformation   
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A19 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void GetFolderInformationTest_TC_Sw_GroundCore_Classes_0005_A28_B()
        {
            string error;
            FolderInfo lst = new T2GClient().GetFolderInformation(1, out error);
            Assert.AreEqual(null, lst);
            Assert.AreEqual(string.Empty, error);
        }
        /// <summary>
        ///A test for CreateFileTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A1 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void CreateFileTransferTaskTest_TC_Sw_GroundCore_Classes_0003_A29_A()
        {
            string error;
            T2GClient testT2GClient = new T2GClient();
            int res = testT2GClient.CreateFileTransferTask("", TransferType.GroundToTrain, "ground", -1, DateTime.Now, DateTime.Now, new List<RecipientId>(), out error);
            Assert.AreEqual(0, res);
            Assert.AreEqual(string.Empty, error);
        }

        /// <summary>
        ///A test for CreateFileTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A1 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void CreateFileTransferTaskTest_TC_Sw_GroundCore_Classes_0003_A29_B()
        {
            string error;
            T2GClient testT2GClient = new T2GClient();
            int res = testT2GClient.CreateFileTransferTask("", TransferType.GroundToTrain, "ground", 1, DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0)), DateTime.Now, new List<RecipientId>(), out error);
            Assert.AreEqual(0, res);
            Assert.AreEqual(string.Empty, error);
        }

        /// <summary>
        ///A test for CreateFileTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A1 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void CreateFileTransferTaskTest_TC_Sw_GroundCore_Classes_0003_A29_C()
        {
            string error;
            T2GClient testT2GClient = new T2GClient();
            int res = testT2GClient.CreateFileTransferTask("", TransferType.GroundToTrain, "ground", 1, DateTime.Now, DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0)), new List<RecipientId>(), out error);
            Assert.AreEqual(0, res);
            Assert.AreEqual(string.Empty, error);
        }

        /// <summary>
        ///A test for CreateFileTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A1 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void CreateFileTransferTaskTest_TC_Sw_GroundCore_Classes_0003_A29_D()
        {
            string error;
            T2GClient testT2GClient = new T2GClient();
            int res = testT2GClient.CreateFileTransferTask("", TransferType.GroundToTrain, "ground", 1, DateTime.Now, DateTime.Now.AddDays(1), new List<RecipientId>(), out error);
            Assert.AreEqual(0, res);
            Assert.AreEqual(string.Empty, error);
        }

        /// <summary>
        ///A test for StartTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A1 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void StartTransferTaskTest_TC_Sw_GroundCore_Classes_0003_A30_A()
        {
            string error;
            T2GClient testT2GClient = new T2GClient();
            bool res = testT2GClient.StartTransferTask(0, 1, linkTypeEnum.anyBandwidth, true, true,out error);
            Assert.AreEqual(false, res);
            Assert.AreEqual(string.Empty, error);
        }

        /// <summary>
        ///A test for StartTransferTask
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0005 : Action A1 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void StartTransferTaskTest_TC_Sw_GroundCore_Classes_0003_A30_B()
        {
            string error;
            int taskId = 0;
            T2GClient testT2GClient = new T2GClient();
            bool res = testT2GClient.StartTransferTask(taskId, 1, linkTypeEnum.anyBandwidth, true, true,out error);
            Assert.AreEqual(false, res);
            Assert.AreEqual(string.Empty, error);
        }
      
    }
}
