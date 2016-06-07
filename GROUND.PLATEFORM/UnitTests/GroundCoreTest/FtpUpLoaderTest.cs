using PIS.Ground.Core.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for FtpUpLoaderTest and is intended
    ///to contain all FtpUpLoaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FtpUpLoaderTest
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
        public void UploadFileTest()
        {
            object state = null; // TODO: Initialize to an appropriate value
            PIS.Ground.Core.Utility.FtpUpLoader_Accessor.UploadFile(state);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Upload
        ///</summary>
        [TestMethod()]
        public void UploadTest()
        {
            PIS.Ground.Core.Data.UploadFileDistributionRequest objUploadFileDistributionRequest = null; // TODO: Initialize to an appropriate value
            PIS.Ground.Core.Data.UploadFileDistributionRequest objUploadFileDistributionRequestExpected = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = PIS.Ground.Core.Utility.FtpUpLoader.Upload(ref objUploadFileDistributionRequest);
            Assert.AreEqual(objUploadFileDistributionRequestExpected, objUploadFileDistributionRequest);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InitializeFtpRequest
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void InitializeFtpRequestTest()
        {
            string strfilepath = string.Empty; // TODO: Initialize to an appropriate value
            string serverIP = string.Empty; // TODO: Initialize to an appropriate value
            string directoryName = string.Empty; // TODO: Initialize to an appropriate value
            string userName = string.Empty; // TODO: Initialize to an appropriate value
            string strPwd = string.Empty; // TODO: Initialize to an appropriate value
            System.Net.FtpWebRequest reqFTP = null; // TODO: Initialize to an appropriate value
            System.Net.FtpWebRequest reqFTPExpected = null; // TODO: Initialize to an appropriate value
            PIS.Ground.Core.Utility.FtpUpLoader_Accessor.InitializeFtpRequest(strfilepath, serverIP, directoryName, userName, strPwd, out reqFTP);
            Assert.AreEqual(reqFTPExpected, reqFTP);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for EndGetStreamCallback
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void EndGetStreamCallbackTest()
        {
            System.IAsyncResult ar = null; // TODO: Initialize to an appropriate value
            PIS.Ground.Core.Utility.FtpUpLoader_Accessor.EndGetStreamCallback(ar);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for EndGetResponseCallback
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void EndGetResponseCallbackTest()
        {
            System.IAsyncResult ar = null; // TODO: Initialize to an appropriate value
            PIS.Ground.Core.Utility.FtpUpLoader_Accessor.EndGetResponseCallback(ar);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AsynchronousUploadFiles
        ///</summary>
        [TestMethod()]
        public void AsynchronousUploadFilesTest()
        {
            PIS.Ground.Core.Data.UploadFileDistributionRequest objUploadFileDistributionRequest = null; // TODO: Initialize to an appropriate value
            PIS.Ground.Core.Data.UploadFileDistributionRequest objUploadFileDistributionRequestExpected = null; // TODO: Initialize to an appropriate value
            PIS.Ground.Core.Utility.FtpUpLoader.AsynchronousUploadFiles(ref objUploadFileDistributionRequest);
            Assert.AreEqual(objUploadFileDistributionRequestExpected, objUploadFileDistributionRequest);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for FtpUpLoader Constructor
        ///</summary>
        [TestMethod()]
        public void FtpUpLoaderConstructorTest()
        {
            PIS.Ground.Core.Utility.FtpUpLoader target = new PIS.Ground.Core.Utility.FtpUpLoader();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
