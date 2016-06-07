using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.T2G;
using PIS.Train.Mission;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for MissionProcessorTest and is intended
    ///to contain all MissionProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MissionProcessorTest
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
        ///A test for ProcessMissionRequest 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void ProcessMissionRequestTestTC_Sw_MissionServiceClasses_0001_A1_A()
        {
            PIS.Ground.Mission.MissionProcessor test=new PIS.Ground.Mission.MissionProcessor();
            string error;
            test.ProcessMissionRequest(null,out error);
            Assert.AreNotEqual(string.Empty, error);
        }

        /// <summary>
        ///A test for ProcessMissionRequest 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void ProcessMissionRequestTestTC_Sw_MissionServiceClasses_0001_A1_B()
        {
            PIS.Ground.Mission.AutomaticActivationRequestContext obj = new PIS.Ground.Mission.AutomaticActivationRequestContext("", "", new Guid(), Guid.Empty, 5, new AutomaticMissionInfoType());
            PIS.Ground.Mission.MissionProcessor test = new PIS.Ground.Mission.MissionProcessor();
            //obj.SessionId = Guid.Empty;
            string error;
            test.ProcessMissionRequest(obj, out error);
            Assert.AreNotEqual(string.Empty, error);
        }

        /// <summary>
        ///A test for ProcessMissionRequest 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void ProcessMissionRequestTestTC_Sw_MissionServiceClasses_0001_A1_C()
        {
            PIS.Ground.Mission.AutomaticActivationRequestContext obj = new PIS.Ground.Mission.AutomaticActivationRequestContext("", "", new Guid(), new Guid(), 5, new AutomaticMissionInfoType());
            PIS.Ground.Mission.MissionProcessor test = new PIS.Ground.Mission.MissionProcessor();
;
            string error;
            test.ProcessMissionRequest(obj, out error);
            Assert.AreNotEqual(string.Empty, error);
        }

        /// <summary>
        ///A test for SendGroundAppNotification 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void SendGroundAppNotificationTestTC_Sw_MissionServiceClasses_0001_A5_A()
        {
            PIS.Ground.Mission.MissionProcessor test = new PIS.Ground.Mission.MissionProcessor();
            Assert.AreEqual(false, test.SendGroundAppNotification(PIS.Ground.Mission.AppGround.NotificationIdEnum.Completed, "", new Guid().ToString(),""));
        }

        /// <summary>
        ///A test for SendGroundAppNotification 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void SendGroundAppNotificationTestTC_Sw_MissionServiceClasses_0001_A5_B()
        {
            PIS.Ground.Mission.MissionProcessor test = new PIS.Ground.Mission.MissionProcessor();
            Assert.AreEqual(false, test.SendGroundAppNotification(PIS.Ground.Mission.AppGround.NotificationIdEnum.Completed, "test", Guid.Empty.ToString(), ""));
        }

        /// <summary>
        ///A test for SendGroundAppNotification 
        ///</summary>
        ///<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        [TestMethod()]
        public void SendGroundAppNotificationTestTC_Sw_MissionServiceClasses_0001_A5_C()
        {
            PIS.Ground.Mission.MissionProcessor test = new PIS.Ground.Mission.MissionProcessor();
            Assert.AreEqual(false, test.SendGroundAppNotification(PIS.Ground.Mission.AppGround.NotificationIdEnum.Completed, "test", new Guid().ToString(), ""));
        }

        ///// <summary>
        /////A test for InitializeAutomaticMission 
        /////</summary>
        /////<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        //[TestMethod()]
        //public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_D()
        //{
        //    UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
        //    System.Guid testguid = testSession.Login("admin", "admin");
        //    PIS.Ground.Mission.MissionProcessor test = new PIS.Ground.Mission.MissionProcessor();
        //    AutomaticModeRequest obj = new AutomaticModeRequest();
        //    obj.SessionId = testguid;
        //    obj.ElementAlphaNumber = "";
        //    obj.MissionOperatorCode = "1";
        //    obj.LmtDataPackageVersion = "2";
        //    MissionServiceResult res = test.v(obj);
        //    Assert.AreNotEqual(MissionErrorCode.SUCCESS, res.errorCode);
        //    Assert.AreNotEqual(Guid.Empty, res.reqId);
        //}

        ///// <summary>
        /////A test for InitializeAutomaticMission 
        /////</summary>
        /////<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        //[TestMethod()]
        //public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_E()
        //{
        //    UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
        //    System.Guid testguid = testSession.Login("admin", "admin");
        //    PIS.Ground.Mission.MissionProcessor test = new PIS.Ground.Mission.MissionProcessor();
        //    AutomaticModeRequest obj = new AutomaticModeRequest();
        //    obj.SessionId = testguid;
        //    obj.ElementAlphaNumber = "1";
        //    obj.MissionOperatorCode = "";
        //    obj.LmtDataPackageVersion = "2";
        //    MissionServiceResult res = test.InitializeAutomaticMission(obj);
        //    Assert.AreNotEqual(MissionErrorCode.SUCCESS, res.errorCode);
        //    Assert.AreNotEqual(Guid.Empty, res.reqId);
        //}


        ///// <summary>
        /////A test for InitializeAutomaticMission 
        /////</summary>
        /////<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        //[TestMethod()]
        //public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_F()
        //{
        //    UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
        //    System.Guid testguid = testSession.Login("admin", "admin");
        //    PIS.Ground.Mission.MissionProcessor test = new PIS.Ground.Mission.MissionProcessor();
        //    AutomaticModeRequest obj = new AutomaticModeRequest();
        //    obj.SessionId = testguid;
        //    obj.ElementAlphaNumber = "1";
        //    obj.MissionOperatorCode = "2";
        //    obj.LmtDataPackageVersion = "";
        //    MissionServiceResult res = test.InitializeAutomaticMission(obj);
        //    Assert.AreNotEqual(MissionErrorCode.SUCCESS, res.errorCode);
        //    Assert.AreNotEqual(Guid.Empty, res.reqId);
        //}

        ///// <summary>
        /////A test for InitializeAutomaticMission 
        /////</summary>
        /////<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        //[TestMethod()]
        //public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_G()
        //{
        //    UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
        //    System.Guid testguid = testSession.Login("admin", "admin");
        //    PIS.Ground.Mission.MissionProcessor test = new PIS.Ground.Mission.MissionProcessor();
        //    AutomaticModeRequest obj = new AutomaticModeRequest();
        //    obj.SessionId = testguid;
        //    obj.ElementAlphaNumber = "1";
        //    obj.MissionOperatorCode = "2";
        //    obj.LmtDataPackageVersion = "2";
        //    obj.LanguageCodeList = new System.Collections.Generic.List<string>();
        //    MissionServiceResult res = test.InitializeAutomaticMission(obj);
        //    Assert.AreNotEqual(MissionErrorCode.SUCCESS, res.errorCode);
        //    Assert.AreNotEqual(Guid.Empty, res.reqId);
        //}


        ///// <summary>
        /////A test for InitializeAutomaticMission 
        /////</summary>
        /////<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        //[TestMethod()]
        //public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_H()
        //{
        //    UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
        //    System.Guid testguid = testSession.Login("admin", "admin");
        //    PIS.Ground.Mission.MissionProcessor test = new PIS.Ground.Mission.MissionProcessor();
        //    AutomaticModeRequest obj = new AutomaticModeRequest();
        //    obj.SessionId = testguid;
        //    obj.ElementAlphaNumber = "1";
        //    obj.MissionOperatorCode = "2";
        //    obj.LmtDataPackageVersion = "2";
        //    obj.LanguageCodeList = new System.Collections.Generic.List<string>();
        //    obj.OnboardServiceCodeList = new System.Collections.Generic.List<uint>();
        //    MissionServiceResult res = test.InitializeAutomaticMission(obj);
        //    Assert.AreNotEqual(MissionErrorCode.SUCCESS, res.errorCode);
        //    Assert.AreNotEqual(Guid.Empty, res.reqId);
        //}

        ///// <summary>
        /////A test for InitializeAutomaticMission 
        /////</summary>
        /////<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        //[TestMethod()]
        //public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_I()
        //{
        //    UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
        //    System.Guid testguid = testSession.Login("admin", "admin");
        //    PIS.Ground.Mission.MissionProcessor test = new PIS.Ground.Mission.MissionProcessor();
        //    AutomaticModeRequest obj = new AutomaticModeRequest();
        //    obj.SessionId = testguid;
        //    obj.ElementAlphaNumber = "1";
        //    obj.MissionOperatorCode = "2";
        //    obj.LmtDataPackageVersion = "2";
        //    System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
        //    lst.Add("test");
        //    System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
        //    ser.Add(1);
        //    obj.LanguageCodeList = lst;
        //    obj.OnboardServiceCodeList = ser;
        //    obj.CarNumberingOffsetCode = 0;
        //    MissionServiceResult res = test.InitializeAutomaticMission(obj);
        //    Assert.AreNotEqual(MissionErrorCode.SUCCESS, res.errorCode);
        //    Assert.AreNotEqual(Guid.Empty, res.reqId);
        //}

        ///// <summary>
        /////A test for InitializeAutomaticMission 
        /////</summary>
        /////<remarks>TC_Sw_ SessionService Classes_0001: Action A1 : case a:</remarks>
        //[TestMethod()]
        //public void InitializeAutomaticMissionTestTC_Sw_MissionServiceClasses_0001_A2_J()
        //{
        //    UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
        //    System.Guid testguid = testSession.Login("admin", "admin");
        //    PIS.Ground.Mission.MissionProcessor test = new PIS.Ground.Mission.MissionProcessor();
        //    AutomaticModeRequest obj = new AutomaticModeRequest();
        //    obj.SessionId = testguid;
        //    obj.ElementAlphaNumber = "1";
        //    obj.MissionOperatorCode = "2";
        //    obj.LmtDataPackageVersion = "2";
        //    System.Collections.Generic.List<string> lst = new System.Collections.Generic.List<string>();
        //    lst.Add("test");
        //    System.Collections.Generic.List<uint> ser = new System.Collections.Generic.List<uint>();
        //    ser.Add(1);
        //    obj.LanguageCodeList = lst;
        //    obj.OnboardServiceCodeList = ser;
        //    obj.CarNumberingOffsetCode = 5;
        //    MissionServiceResult res = test.InitializeAutomaticMission(obj);
        //    Assert.AreNotEqual(MissionErrorCode.SUCCESS, res.errorCode);
        //    Assert.AreNotEqual(Guid.Empty, res.reqId);
        //}
    }
}
