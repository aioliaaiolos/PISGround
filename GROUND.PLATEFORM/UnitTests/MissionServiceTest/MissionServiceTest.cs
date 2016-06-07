using PIS.Ground.Mission;
using PIS.Ground.Core.Data;
using System;
using UnitTests.MissionService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for MissionServiceTest and is intended
    ///to contain all MissionServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MissionServiceTest
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
        public void RetrieveElementListTestTC_Sw_MissionServiceClasses_0001_A1_A()
        {
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ElementAvailableElementDataList res = test.RetrieveElementList(Guid.Empty);
            Assert.AreEqual(0, res.Count);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void RetrieveElementListTestTC_Sw_MissionServiceClasses_0001_A1_B()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ElementAvailableElementDataList res = test.RetrieveElementList(testguid);
            Assert.AreNotEqual(0, res.Count);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void RetrieveElementListTestTC_Sw_MissionServiceClasses_0001_A1_C()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ElementAvailableElementDataList res = test.RetrieveElementList(testguid);
            Assert.AreNotEqual(0, res.Count);
        }

        /// <summary>
        ///A test for Login 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void RetrieveElementListTestTC_Sw_MissionServiceClasses_0001_A1_D()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ElementAvailableElementDataList res = test.RetrieveElementList(testguid);
            Assert.AreNotEqual(0, res.Count);
        }

        #region InitializeAutomaticMission
        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_A()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj=new AutomaticModeRequest();
            obj.SessionId = Guid.Empty;
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_B()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.RequestTimeout = 0;
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_C()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.RequestTimeout = 43205;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "1";
            obj.LmtDataPackageVersion = "2";
            obj.StartDate = DateTime.Now.ToLongDateString();
            StationInsertion st = new StationInsertion();
            st.StationId = "ABC";
            st.Type = 1;
            obj.StationInsertion = st;
            obj.LanguageCodeList = new System.Collections.Generic.List<string>();
            obj.OnboardServiceCodeList = new System.Collections.Generic.List<uint>();
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_D()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "";
            obj.MissionOperatorCode = "1";
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_E()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "";
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }


        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_F()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "";
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_G()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.LanguageCodeList = new System.Collections.Generic.List<string>();
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_H()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.LanguageCodeList = new System.Collections.Generic.List<string>();
            obj.OnboardServiceCodeList = new System.Collections.Generic.List<uint>();
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_I()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 0;
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_J()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 5;
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_K()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.StartDate = DateTime.Now.ToLongDateString();
            StationInsertion st = new StationInsertion();
            st.StationId = "ABC";
            st.Type = 1;
            obj.StationInsertion = st;
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_L()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.StartDate = DateTime.Now.ToLongDateString();
            StationInsertion st = new StationInsertion();
            st.StationId = "ABC";
            st.Type = 1;
            obj.StationInsertion = st;
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_M()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.StartDate = DateTime.Now.ToLongDateString();
            StationInsertion st = new StationInsertion();
            st.StationId = "ABC";
            st.Type = 1;
            obj.StationInsertion = st;
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_N()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.StartDate = DateTime.Now.ToLongDateString();
            StationInsertion st=new StationInsertion();
            st.StationId = "ABC";
            st.Type=1;
            obj.StationInsertion = st;
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_O()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            AutomaticModeRequest obj = new AutomaticModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.StartDate = DateTime.Now.ToLongDateString();
            StationInsertion st = new StationInsertion();
            st.StationId = "ABC";
            st.Type = 1;
            obj.StationInsertion = st;
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            MissionServiceResult res = test.InitializeAutomaticMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.InvalidTrainName, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }
        #endregion

        #region InitializeManualMission
        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_A()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = Guid.Empty;
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_B()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.RequestTimeout = 0;
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_C()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.RequestTimeout = 43205;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "1";
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_D()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "";
            obj.MissionOperatorCode = "1";
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_E()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "";
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }


        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_F()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "";
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_G()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.LanguageCodeList = new System.Collections.Generic.List<string>();
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_H()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.LanguageCodeList = new System.Collections.Generic.List<string>();
            obj.OnboardServiceCodeList = new System.Collections.Generic.List<uint>();
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMissInitializeManualMissionion 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_I()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 0;
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_J()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 5;
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_K()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.MissionTypeCode = "1";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            System.Collections.Generic.List<string> cm = new System.Collections.Generic.List<string>();
            cm.Add("test");
            obj.CommercialNumberList = cm;
            System.Collections.Generic.List<StationList> sh = new System.Collections.Generic.List<StationList>();
            StationList s = new StationList();
            s.Id = "ABC";
            s.Name = "s";
            sh.Add(s);
            obj.ServicedStationList = sh;
            System.Collections.Generic.List<StationServiceHours> ssh = new System.Collections.Generic.List<StationServiceHours>();
            StationServiceHours ss = new StationServiceHours();
            ss.ArrivalTime = new time(DateTime.Now);
            ss.DepartureTime = new time(DateTime.Now);
            ssh.Add(ss);
            obj.ServiceHourList = ssh;
            obj.TrainName = "t";
            obj.StartDate = DateTime.Now.ToLongDateString();
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_L()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.MissionTypeCode = "1";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            System.Collections.Generic.List<string> cm = new System.Collections.Generic.List<string>();
            cm.Add("test");
            obj.CommercialNumberList = cm;
            System.Collections.Generic.List<StationList> sh = new System.Collections.Generic.List<StationList>();
            StationList s = new StationList();
            s.Id = "ABC";
            s.Name = "s";
            sh.Add(s);
            obj.ServicedStationList = sh;
            System.Collections.Generic.List<StationServiceHours> ssh = new System.Collections.Generic.List<StationServiceHours>();
            StationServiceHours ss = new StationServiceHours();
            ss.ArrivalTime = new time(DateTime.Now);
            ss.DepartureTime = new time(DateTime.Now);
            ssh.Add(ss);
            obj.ServiceHourList = ssh;
            obj.TrainName = "t";
            obj.StartDate = DateTime.Now.ToLongDateString();
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_M()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.MissionTypeCode = "1";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            System.Collections.Generic.List<string> cm = new System.Collections.Generic.List<string>();
            cm.Add("test");
            obj.CommercialNumberList = cm;
            System.Collections.Generic.List<StationList> sh = new System.Collections.Generic.List<StationList>();
            StationList s = new StationList();
            s.Id = "ABC";
            s.Name = "s";
            sh.Add(s);
            obj.ServicedStationList = sh;
            System.Collections.Generic.List<StationServiceHours> ssh = new System.Collections.Generic.List<StationServiceHours>();
            StationServiceHours ss = new StationServiceHours();
            ss.ArrivalTime = new time(DateTime.Now);
            ss.DepartureTime = new time(DateTime.Now);
            ssh.Add(ss);
            obj.ServiceHourList = ssh;
            obj.TrainName = "t";
            obj.StartDate = DateTime.Now.ToLongDateString();
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_N()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.MissionTypeCode = "1";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            System.Collections.Generic.List<string> cm = new System.Collections.Generic.List<string>();
            cm.Add("test");
            obj.CommercialNumberList = cm;
            System.Collections.Generic.List<StationList> sh = new System.Collections.Generic.List<StationList>();
            StationList s = new StationList();
            s.Id = "ABC";
            s.Name = "s";
            sh.Add(s);
            obj.ServicedStationList = sh;
            System.Collections.Generic.List<StationServiceHours> ssh = new System.Collections.Generic.List<StationServiceHours>();
            StationServiceHours ss = new StationServiceHours();
            ss.ArrivalTime = new time(DateTime.Now);
            ss.DepartureTime = new time(DateTime.Now);
            ssh.Add(ss);
            obj.ServiceHourList = ssh;
            obj.TrainName = "t";
            obj.StartDate = DateTime.Now.ToLongDateString();
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeManualMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeManualMissionTestTC_Sw_MissionServiceClasses_0001_A4_O()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ManualModeRequest obj = new ManualModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.MissionTypeCode = "1";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            System.Collections.Generic.List<string> cm = new System.Collections.Generic.List<string>();
            cm.Add("test");
            obj.CommercialNumberList = cm;
            System.Collections.Generic.List<StationList> sh = new System.Collections.Generic.List<StationList>();
            StationList s = new StationList();
            s.Id = "ABC";
            s.Name = "s";
            sh.Add(s);
            obj.ServicedStationList = sh;
            System.Collections.Generic.List<StationServiceHours> ssh = new System.Collections.Generic.List<StationServiceHours>();
            StationServiceHours ss = new StationServiceHours();
            ss.ArrivalTime = new time(DateTime.Now);
            ss.DepartureTime = new time(DateTime.Now);
            ssh.Add(ss);
            obj.ServiceHourList = ssh;
            obj.TrainName = "t";
            obj.StartDate = DateTime.Now.ToLongDateString();
            MissionServiceResult res = test.InitializeManualMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.InvalidTrainName, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }
        #endregion

        #region InitializeModifiedMission
        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_A()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = Guid.Empty;
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_B()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.RequestTimeout = 0;
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_C()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.RequestTimeout = 43205;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "1";
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_D()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "";
            obj.MissionOperatorCode = "1";
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_E()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "";
            obj.LmtDataPackageVersion = "2";
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }


        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_F()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "";
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_G()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.LanguageCodeList = new System.Collections.Generic.List<string>();
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_H()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.LanguageCodeList = new System.Collections.Generic.List<string>();
            obj.OnboardServiceCodeList = new System.Collections.Generic.List<uint>();
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeAutomaticMissInitializeModifiedMissionion 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_I()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 0;
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_J()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 5;
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_K()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.MissionTypeCode = "1";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            System.Collections.Generic.List<string> cm = new System.Collections.Generic.List<string>();
            cm.Add("test");
            obj.CommercialNumberList = cm;
            System.Collections.Generic.List<StationList> sh = new System.Collections.Generic.List<StationList>();
            StationList s = new StationList();
            s.Id = "ABC";
            s.Name = "s";
            sh.Add(s);
            obj.ServicedStationList = sh;
            System.Collections.Generic.List<StationServiceHours> ssh = new System.Collections.Generic.List<StationServiceHours>();
            StationServiceHours ss = new StationServiceHours();
            ss.ArrivalTime = new time(DateTime.Now);
            ss.DepartureTime = new time(DateTime.Now);
            ssh.Add(ss);
            obj.ServiceHourList = ssh;
            obj.TrainName = "t";
            obj.StartDate = DateTime.Now.ToLongDateString();
            StationInsertion stI=new StationInsertion();
            stI.StationId = "ABC";
            stI.Type=1;
            obj.StationInsertion = stI;
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_L()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.MissionTypeCode = "1";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            System.Collections.Generic.List<string> cm = new System.Collections.Generic.List<string>();
            cm.Add("test");
            obj.CommercialNumberList = cm;
            System.Collections.Generic.List<StationList> sh = new System.Collections.Generic.List<StationList>();
            StationList s=new StationList();
            s.Id = "ABC";
            s.Name="s";
            sh.Add(s);
            obj.ServicedStationList = sh;
            System.Collections.Generic.List<StationServiceHours> ssh = new System.Collections.Generic.List<StationServiceHours>();
            StationServiceHours ss=new StationServiceHours();
            ss.ArrivalTime = new time(DateTime.Now);
            ss.DepartureTime = new time(DateTime.Now);
            ssh.Add(ss);
            obj.ServiceHourList=ssh;
            obj.TrainName = "t";
            obj.StartDate = DateTime.Now.ToLongDateString();
            StationInsertion stI = new StationInsertion();
            stI.StationId = "ABC";
            stI.Type = 1;
            obj.StationInsertion = stI;
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_M()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.MissionTypeCode = "1";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            System.Collections.Generic.List<string> cm = new System.Collections.Generic.List<string>();
            cm.Add("test");
            obj.CommercialNumberList = cm;
              System.Collections.Generic.List<StationList> sh = new System.Collections.Generic.List<StationList>();
            StationList s=new StationList();
            s.Id = "ABC";
            s.Name="s";
            sh.Add(s);
            obj.ServicedStationList = sh;
            System.Collections.Generic.List<StationServiceHours> ssh = new System.Collections.Generic.List<StationServiceHours>();
            StationServiceHours ss = new StationServiceHours();
            ss.ArrivalTime = new time(DateTime.Now);
            ss.DepartureTime = new time(DateTime.Now);
            ssh.Add(ss);
            obj.ServiceHourList = ssh;
            obj.TrainName = "t";
            obj.StartDate = DateTime.Now.ToLongDateString();
            StationInsertion stI = new StationInsertion();
            stI.StationId = "ABC";
            stI.Type = 1;
            obj.StationInsertion = stI;
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_N()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "1";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.MissionTypeCode = "1";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            System.Collections.Generic.List<string> cm = new System.Collections.Generic.List<string>();
            cm.Add("test");
            obj.CommercialNumberList = cm;
            System.Collections.Generic.List<StationServiceHours> ssh = new System.Collections.Generic.List<StationServiceHours>();
            StationServiceHours ss = new StationServiceHours();
            ss.ArrivalTime = new time(DateTime.Now);
            ss.DepartureTime = new time(DateTime.Now);
            ssh.Add(ss);
            obj.ServiceHourList = ssh;
            obj.TrainName = "t";
            obj.StartDate = DateTime.Now.ToLongDateString();
            StationList stlst=new StationList();
            stlst.Id = "ABC";
            stlst.Name="st";
            obj.ServicedStationList.Add(stlst);
            StationInsertion stI = new StationInsertion();
            stI.StationId = "ABC";
            stI.Type = 1;
            obj.StationInsertion = stI;
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for InitializeModifiedMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void InitializeModifiedMissionTestTC_Sw_MissionServiceClasses_0001_A3_O()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            ModifiedModeRequest obj = new ModifiedModeRequest();
            obj.SessionId = testguid;
            obj.ElementAlphaNumber = "123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";
            obj.MissionOperatorCode = "2";
            obj.LmtDataPackageVersion = "2";
            obj.MissionTypeCode = "1";
            System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
            lst.Add("test");
            System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
            ser.Add(1);
            obj.LanguageCodeList = lst;
            obj.OnboardServiceCodeList = ser;
            obj.CarNumberingOffsetCode = 3;
            System.Collections.Generic.List<string> cm = new System.Collections.Generic.List<string>();
            cm.Add("test");
            obj.CommercialNumberList = cm;
            System.Collections.Generic.List<StationServiceHours> ssh = new System.Collections.Generic.List<StationServiceHours>();
            StationServiceHours ss = new StationServiceHours();
            ss.ArrivalTime = new time(DateTime.Now);
            ss.DepartureTime = new time(DateTime.Now);
            ssh.Add(ss);
            obj.ServiceHourList = ssh;
            obj.TrainName = "t";
            obj.StartDate = DateTime.Now.ToLongDateString();
            StationList stlst = new StationList();
            stlst.Id = "ABC";
            stlst.Name = "st";
            obj.ServicedStationList.Add(stlst);
            StationInsertion stI = new StationInsertion();
            stI.StationId = "ABC";
            stI.Type = 1;
            obj.StationInsertion = stI;
            MissionServiceResult res = test.InitializeModifiedMission(testguid, obj);
            Assert.AreEqual(MissionErrorCode.InvalidTrainName, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }
        #endregion

        /// <summary>
        ///A test for StopMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void StopMissionTestTC_Sw_MissionServiceClasses_0001_A5_A()
        {
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            MissionServiceResult res = test.StopMission(Guid.Empty,false,"1","1",23);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for StopMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void StopMissionTestTC_Sw_MissionServiceClasses_0001_A5_B()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            MissionServiceResult res = test.StopMission(testguid, true, "1", "1", 43205);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for StopMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void StopMissionTestTC_Sw_MissionServiceClasses_0001_A5_C()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            MissionServiceResult res = test.StopMission(testguid, true, "", "1", 4);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for StopMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void StopMissionTestTC_Sw_MissionServiceClasses_0001_A5_D()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            MissionServiceResult res = test.StopMission(testguid, true, "1", "", 4);
            Assert.AreNotEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for StopMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void StopMissionTestTC_Sw_MissionServiceClasses_0001_A5_E()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            MissionServiceResult res = test.StopMission(testguid, true, "1", "1", 4);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for StopMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void StopMissionTestTC_Sw_MissionServiceClasses_0001_A5_F()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            MissionServiceResult res = test.StopMission(testguid, true, "1", "1", 4);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }

        /// <summary>
        ///A test for StopMission 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void StopMissionTestTC_Sw_MissionServiceClasses_0001_A5_G()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            UnitTests.MissionService.MissionServiceClient test = new UnitTests.MissionService.MissionServiceClient();
            System.Guid testguid = testSession.Login("admin", "admin");
            MissionServiceResult res = test.StopMission(testguid, true, "1", "1", 4);
            Assert.AreEqual(MissionErrorCode.RequestAccepted, res.ResultCode);
            Assert.AreNotEqual(Guid.Empty, res.RequestId);
        }
    }
}
