using PIS.Ground.Session;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for SessionServiceTest and is intended
    ///to contain all SessionServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SessionServiceTest
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
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void LoginTestTC_Sw_SessionServiceClasses_0001_A1_A()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("", "");
            Assert.AreEqual(System.Guid.Empty, testguid);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case b:</remarks>
        [TestMethod()]
        public void LoginTestTC_Sw_SessionServiceClasses_0001_A1_B()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "");
            Assert.AreEqual(System.Guid.Empty, testguid);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case c:</remarks>
        [TestMethod()]
        public void LoginTestTC_Sw_SessionServiceClasses_0001_A1_C()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("", "admin");
            Assert.AreEqual(System.Guid.Empty, testguid);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case d:</remarks>
        [TestMethod()]
        public void LoginTestTC_Sw_SessionServiceClasses_0001_A1_D()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            Assert.AreNotEqual(System.Guid.Empty, testguid);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A2 : case a:</remarks>
        [TestMethod()]
        public void LogoutTestTC_Sw_SessionServiceClasses_0001_A2_A()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            bool res = testSession.Logout(new System.Guid());
            Assert.AreEqual(false, res);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A2 : case b:</remarks>
        [TestMethod()]
        public void LogoutTestTC_Sw_SessionServiceClasses_0001_A2_B()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            bool res = testSession.Logout(testguid);
            Assert.AreEqual(true, res);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A3 : case a:</remarks>
        [TestMethod()]
        public void SetNotificationInformationTestTC_Sw_SessionServiceClasses_0001_A3_A()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            bool res = testSession.SetNotificationInformation(new System.Guid(), "");
            Assert.AreEqual(false, res);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A13: case b:</remarks>
        [TestMethod()]
        public void SetNotificationInformationTestTC_Sw_SessionServiceClasses_0001_A3_B()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            bool res = testSession.SetNotificationInformation(testguid, "");
            Assert.AreEqual(false, res);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A3 : case c:</remarks>
        [TestMethod()]
        public void SetNotificationInformationTestTC_Sw_SessionServiceClasses_0001_A3_C()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            bool res = testSession.SetNotificationInformation(new System.Guid(), "http://www.google.co.in/");
            Assert.AreEqual(false, res);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A3 : case d:</remarks>
        [TestMethod()]
        public void SetNotificationInformationTestTC_Sw_SessionServiceClasses_0001_A3_D()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            bool res = testSession.SetNotificationInformation(testguid, "http://www.google.co.in/");
            Assert.AreEqual(true, res);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A4 : case a:</remarks>
        [TestMethod()]
        public void IsSessionValidTestTC_Sw_SessionServiceClasses_0001_A4_A()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            bool res = testSession.IsSessionValid(System.Guid.Empty);
            Assert.AreEqual(false, res);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A4 : case b:</remarks>
        [TestMethod()]
        public void IsSessionValidTestTC_Sw_SessionServiceClasses_0001_A4_B()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            bool res = testSession.IsSessionValid(testguid);
            Assert.AreEqual(true, res);
        }
    }
}
