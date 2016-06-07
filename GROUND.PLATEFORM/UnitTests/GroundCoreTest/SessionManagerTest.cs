using PIS.Ground.Core.SessionMgmt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for SessionManagerTest and is intended
    ///to contain all SessionManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SessionManagerTest
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
            //OBSOLETE
            //PIS.Ground.Core.Utility.T2GConfiguration.InitializeConfigPaths();
        }

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
        ///<remarks>TC_Sw_ GroundCore Classes_0003 : Action A1 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void LoginTest_TC_Sw_GroundCore_Classes_0003_A1_A()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("", "", out testguid);
            Assert.AreEqual(Guid.Empty, testguid);
        }

        /// <summary>
        ///A test for Login
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003 : Action A1 : case b</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void LoginTest_TC_Sw_GroundCore_Classes_0003_A1_B()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "", out testguid);
            Assert.AreEqual(Guid.Empty, testguid);
        }

        /// <summary>
        ///A test for Login
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003 : Action A1 : case c</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void LoginTest_TC_Sw_GroundCore_Classes_0003_A1_C()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("", "admin", out testguid);
            Assert.AreEqual(Guid.Empty, testguid);
        }

        /// <summary>
        ///A test for Login
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003 : Action A1 : case d</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void LoginTest_TC_Sw_GroundCore_Classes_0003_A1_D()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "admin", out testguid);
            Assert.AreNotEqual(Guid.Empty, testguid);
        }

        /// <summary>
        ///A test for RemoveSessionID
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003 : Action A4 : case a</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void RemoveSessionIDTest_TC_Sw_GroundCore_Classes_0003_A4_A()
        {
            SessionManager testSession = new SessionManager();
            string res = testSession.RemoveSessionID(new Guid());
            Assert.AreNotEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for RemoveSessionID
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003 : Action A4 : case b</remarks>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void RemoveSessionIDTest_TC_Sw_GroundCore_Classes_0003_A4_B()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "admin", out testguid);
            string res = testSession.RemoveSessionID(testguid);
            Assert.AreEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A2 : case a:</remarks>
        [TestMethod()]
        public void SetNotificationInformationTestTC_Sw_GroundCoreClasses_0003_A2_A()
        {
            SessionManager testSession = new SessionManager();
            string res = testSession.SetNotificationURL(new System.Guid(), "");
            Assert.AreNotEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A13: case b:</remarks>
        [TestMethod()]
        public void SetNotificationInformationTestTC_Sw_GroundCoreClasses_0003_A2_B()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "admin", out testguid);
            string res = testSession.SetNotificationURL(testguid, "");
            Assert.AreNotEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A2 : case c:</remarks>
        [TestMethod()]
        public void SetNotificationInformationTestTC_Sw_GroundCoreClasses_0003_A2_C()
        {
            SessionManager testSession = new SessionManager();
            string res = testSession.SetNotificationURL(new System.Guid(), "test url");
            Assert.AreNotEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A2 : case d:</remarks>
        [TestMethod()]
        public void SetNotificationInformationTestTC_Sw_GroundCoreClasses_0003_A2_D()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "admin", out testguid);
            string res = testSession.SetNotificationURL(testguid, "test url");
            Assert.AreEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A3 : case a:</remarks>
        [TestMethod()]
        public void IsSessionValidTestTC_Sw_GroundCoreClasses_0003_A3_A()
        {
            SessionManager testSession = new SessionManager();
            bool res = testSession.IsSessionValid(System.Guid.Empty);
            Assert.AreEqual(false, res);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A3 : case b:</remarks>
        [TestMethod()]
        public void IsSessionValidTestTC_Sw_GroundCoreClasses_0003_A3_B()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "admin",out testguid);
            bool res = testSession.IsSessionValid(testguid);
            Assert.AreEqual(true, res);
        }

        /// <summary>
        ///A test for GetNotificationURL 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A5 : case a:</remarks>
        [TestMethod()]
        public void GetNotificationURLTestTC_Sw_GroundCoreClasses_0003_A5_A()
        {
            SessionManager testSession = new SessionManager();
            string notiUrl = string.Empty;
            string res = testSession.GetNotificationUrlBySessionId(System.Guid.Empty,out notiUrl);
            Assert.AreNotEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for GetNotificationURL 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A5 : case b:</remarks>
        [TestMethod()]
        public void GetNotificationURLTestTC_Sw_GroundCoreClasses_0003_A5_B()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid ;
            testSession.Login("admin", "admin", out testguid);
            testSession.SetNotificationURL(testguid, "test Url");
            string notiUrl = string.Empty;
            string res = testSession.GetNotificationUrlBySessionId(testguid, out notiUrl);
            //Assert.AreEqual(string.Empty, res);
            //Assert.AreEqual("test Url", notiUrl);
        }

        /// <summary>
        ///A test for GetNotificationURL 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A6 : case a:</remarks>
        [TestMethod()]
        public void GetNotificationURLTestTC_Sw_GroundCoreClasses_0003_A6_A()
        {
            SessionManager testSession = new SessionManager();
            string notiUrl = string.Empty;
            string res = testSession.GetNotificationUrlByRequestId(System.Guid.Empty,out notiUrl);
            Assert.AreNotEqual(string.Empty, res);
            Assert.AreEqual(string.Empty, notiUrl);
        }

        /// <summary>
        ///A test for GetNotificationURL 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A6 : case b:</remarks>
        [TestMethod()]
        public void GetNotificationURLTestTC_Sw_GroundCoreClasses_0003_A6_B()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "admin", out testguid);
            string str =testSession.SetNotificationURL(testguid, "test Url");
            //Assert.AreEqual(string.Empty, str);
            string notiUrl = string.Empty;
            //System.Guid reqId;
            //testSession.GenerateRequestID(testguid, out reqId);
            //string res = testSession.GetNotificationURL(reqId.ToString(), out notiUrl);
            //Assert.AreEqual(string.Empty, res);
            //Assert.AreEqual("test Url", notiUrl);
        }

        /// <summary>
        ///A test for KeepSessionAlive 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A7 : case a:</remarks>
        [TestMethod()]
        public void KeepSessionAliveTestTC_Sw_GroundCoreClasses_0003_A7_A()
        {
            SessionManager testSession = new SessionManager();
            string res = testSession.KeepSessionAlive(Guid.Empty, 20);
            Assert.AreNotEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for KeepSessionAlive 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A7 : case b:</remarks>
        [TestMethod()]
        public void KeepSessionAliveTestTC_Sw_GroundCoreClasses_0003_A7_B()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "admin", out testguid);
            string res = testSession.KeepSessionAlive(testguid, -1);
            Assert.AreNotEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for KeepSessionAlive 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A7 : case c:</remarks>
        [TestMethod()]
        public void KeepSessionAliveTestTC_Sw_GroundCoreClasses_0003_A7_C()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "admin", out testguid);
            string res = testSession.KeepSessionAlive(testguid, 80);
            Assert.AreNotEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for KeepSessionAlive 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A7 : case d:</remarks>
        [TestMethod()]
        public void KeepSessionAliveTestTC_Sw_GroundCoreClasses_0003_A7_D()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "admin", out testguid);
            string res = testSession.KeepSessionAlive(testguid, 55);
            Assert.AreEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for GetSessionDetails 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A8 : case a:</remarks>
        [TestMethod()]
        public void GetSessionDetailsTestTC_Sw_GroundCoreClasses_0003_A8_A()
        {
            SessionManager testSession = new SessionManager();
            PIS.Ground.Core.Data.SessionData sessionData;
            string res = testSession.GetSessionDetails(Guid.Empty, out sessionData);
            Assert.AreNotEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for GetSessionDetails 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A8 : case b:</remarks>
        [TestMethod()]
        public void GetSessionDetailsTestTC_Sw_GroundCoreClasses_0003_A8_B()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "admin", out testguid);
            PIS.Ground.Core.Data.SessionData sessionData;
            string res = testSession.GetSessionDetails(testguid, out sessionData);
            Assert.AreEqual(string.Empty, res);
            Assert.AreNotEqual(null, sessionData);
        }

        /// <summary>
        ///A test for GenerateRequestID 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A9 : case a:</remarks>
        [TestMethod()]
        public void GenerateRequestIDTestTC_Sw_GroundCoreClasses_0003_A9_A()
        {
            SessionManager testSession = new SessionManager();
            Guid reqID;
            string res = testSession.GenerateRequestID(Guid.Empty, out reqID);
            Assert.AreNotEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for GenerateRequestID 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A9 : case b:</remarks>
        [TestMethod()]
        public void GenerateRequestIDTestTC_Sw_GroundCoreClasses_0003_A9_B()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "admin", out testguid);
            Guid reqID;
            string res = testSession.GenerateRequestID(testguid, out reqID);
            Assert.AreEqual(string.Empty, res);
            Assert.AreNotEqual(Guid.Empty, reqID);
        }

        /// <summary>
        ///A test for UpdateRequestIdStatus 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A10 : case a:</remarks>
        [TestMethod()]
        public void UpdateRequestIdStatusTestTC_Sw_GroundCoreClasses_0003_A10_A()
        {
            SessionManager testSession = new SessionManager();
            string res = testSession.UpdateRequestIdStatus(Guid.Empty.ToString(), PIS.Ground.Core.Data.RequestStatus.InProgress);
            Assert.AreNotEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for UpdateRequestIdStatus 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A10 : case b:</remarks>
        [TestMethod()]
        public void UpdateRequestIdStatusTestTC_Sw_GroundCoreClasses_0003_A10_B()
        {
            SessionManager testSession = new SessionManager();
            System.Guid testguid;
            testSession.Login("admin", "admin", out testguid);
            Guid reqID;
            testSession.GenerateRequestID(testguid, out reqID);
            string res = testSession.UpdateRequestIdStatus(reqID.ToString(), PIS.Ground.Core.Data.RequestStatus.InProgress);
            Assert.AreEqual(string.Empty, res);
        }

        /// <summary>
        ///A test for ValidateSession 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A11 : case a:</remarks>
        [TestMethod()]
        public void ValidateSessionTestTC_Sw_GroundCoreClasses_0003_A11_A()
        {
            //SessionManager testSession = new SessionManager();
            //bool res = testSession.GetSessionDetails(System.Guid.Empty);
            //Assert.AreEqual(false, res);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for ValidateSession 
        ///</summary>
        ///<remarks>TC_Sw_ GroundCore Classes_0003: Action A11 : case b:</remarks>
        [TestMethod()]
        public void ValidateSessionTestTC_Sw_GroundCoreClasses_0003_A11_B()
        {
            //SessionManager testSession = new SessionManager();
            //System.Guid testguid;
            //testSession.Login("admin", "admin", out testguid);
            //bool res = testSession.ValidateSession(testguid);
            //Assert.AreEqual(true, res);
            Assert.Fail(); //OBSOLETE
        }
       
    }
}
