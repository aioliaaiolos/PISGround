using PIS.Ground.Core.LogMgmt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTests
{


    /// <summary>
    ///This is a test class for LogManagerTest and is intended
    ///to contain all LogManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LogManagerTest
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
        ///A test for WriteLog info
        ///</summary>
        [TestMethod()]
        public void WriteLogInfoTest()
        {
            string strMessage = "Test for Info Log";
            string strContext = "SessionManager.ValidateSession";
            System.Exception objEx = null;
            Assert.AreEqual(true, LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.INFO, strMessage, strContext, objEx, PIS.Ground.Core.Data.EventIdEnum.Debug));
        }

        /// <summary>
        ///A test for WriteLog info
        ///</summary>
        [TestMethod()]
        public void WriteLogErrorTest()
        {
            string strMessage = "Test for Error Log";
            string strContext = "SessionManager.ValidateSession";
            System.Exception objEx = null;
            Assert.AreEqual(true, LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.ERROR, strMessage, strContext, objEx, PIS.Ground.Core.Data.EventIdEnum.Debug));
        }

        /// <summary>
        ///A test for WriteLog info
        ///</summary>
        [TestMethod()]
        public void WriteLogExceptionTest()
        {
            string strMessage = "Test for Exception Log";
            string strContext = "SessionManager.ValidateSession";
            System.Exception objEx = new System.Exception("Invalid Exception Raised");
            Assert.AreEqual(true, LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.EXCEPTION, strMessage, strContext, objEx, PIS.Ground.Core.Data.EventIdEnum.Debug));
        }

        /// <summary>
        ///A test for WriteLog Warning
        ///</summary>
        [TestMethod()]
        public void WriteLogWarningTest()
        {
            string strMessage = "Test for Exception Log";
            string strContext = "SessionManager.ValidateSession";
            System.Exception objEx = null;
            Assert.AreEqual(true, LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.WARNING, strMessage, strContext, objEx, PIS.Ground.Core.Data.EventIdEnum.Debug));
        }

        /// <summary>
        ///A test for WriteLog Debug
        ///</summary>
        [TestMethod()]
        public void WriteLogDebugTest()
        {
            string strMessage = "Test for Debug Log";
            string strContext = "SessionManager.ValidateSession";
            System.Exception objEx = null;
            Assert.AreEqual(true, LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.DEBUG, strMessage, strContext, objEx, PIS.Ground.Core.Data.EventIdEnum.Debug));
        }


        /// <summary>
        ///A test for LogException
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void WriteEventLogTest()
        {
            string strMessage = "Test for Exception Log";
            string strContext = "SessionManager.ValidateSession";
            System.Exception objEx = null;
            Assert.AreEqual(true, PIS.Ground.Core.LogMgmt.LogManager_Accessor.WriteEventLog(objEx, strContext, strMessage, System.Diagnostics.EventLogEntryType.Error, 0));
        }
        /// <summary>
        ///A test for LogException
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void WriteEventLogExceptionTest()
        {
            string strMessage = "Test for Exception Log";
            string strContext = "SessionManager.ValidateSession";
            System.Exception objEx = new System.Exception("Invalid Exception Raised"); ;
            Assert.AreEqual(true, PIS.Ground.Core.LogMgmt.LogManager_Accessor.WriteEventLog(objEx, strContext, strMessage, System.Diagnostics.EventLogEntryType.Error, 0));
        }

    }
}
