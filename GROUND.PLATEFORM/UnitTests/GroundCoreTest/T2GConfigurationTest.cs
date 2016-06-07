using PIS.Ground.Core.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for T2GConfigurationTest and is intended
    ///to contain all T2GConfigurationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class T2GConfigurationTest
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
        ///A test for T2GServiceUserName
        ///</summary>
        [TestMethod()]
        public void T2GServiceUserNameTest()
        {
            string actual;
            actual = PIS.Ground.Core.Utility.T2GConfiguration.T2GServiceUserName;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for T2GServicePwd
        ///</summary>
        [TestMethod()]
        public void T2GServicePwdTest()
        {
            string actual;
            actual = PIS.Ground.Core.Utility.T2GConfiguration.T2GServicePwd;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for T2GServiceNotificataionUrl
        ///</summary>
        [TestMethod()]
        public void T2GServiceNotificataionUrlTest()
        {
            string actual;
            actual = PIS.Ground.Core.Utility.T2GConfiguration.T2GServiceNotificataionUrl;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for T2GApplicationId
        ///</summary>
        [TestMethod()]
        public void T2GApplicationIdTest()
        {
            string actual;
            actual = PIS.Ground.Core.Utility.T2GConfiguration.T2GApplicationId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SessionTimerCheck
        ///</summary>
        [TestMethod()]
        public void SessionTimerCheckTest()
        {
            long actual;
            actual = PIS.Ground.Core.Utility.T2GConfiguration.SessionTimerCheck;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SessionTimeOut
        ///</summary>
        [TestMethod()]
        public void SessionTimeOutTest()
        {
            int actual;
            actual = PIS.Ground.Core.Utility.T2GConfiguration.SessionTimeOut;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SessionSqLiteDBPath
        ///</summary>
        [TestMethod()]
        public void SessionSqLiteDBPathTest()
        {
            string actual;
            actual = PIS.Ground.Core.Utility.T2GConfiguration.SessionSqLiteDBPath;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InitializeT2GConfig
        ///</summary>
        [TestMethod()]
        public void InitializeT2GConfigTest()
        {
            PIS.Ground.Core.Utility.T2GConfiguration.InitializeT2GConfig();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for InitializeConfigPaths
        ///</summary>
        [TestMethod()]
        public void InitializeConfigPathsTest()
        {
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = PIS.Ground.Core.Utility.T2GConfiguration.InitializeConfigPaths();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
