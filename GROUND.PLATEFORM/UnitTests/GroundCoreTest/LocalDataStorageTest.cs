using PIS.Ground.Core.T2G;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for LocalDataStorageTest and is intended
    ///to contain all LocalDataStorageTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LocalDataStorageTest
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
        ///A test for GetSystemData
        ///</summary>
        [TestMethod()]
        public void GetSystemDataTest_TC_Sw_GroundCore_Classes_0010_A1_A()
        {
            //PIS.Ground.Core.Data.SystemInfo actual=null;
            //actual = PIS.Ground.Core.T2G.LocalDataStorage.Instance.GetSystemData("");
            //Assert.AreEqual(null, actual);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for GetSystemData
        ///</summary>
        [TestMethod()]
        public void GetSystemDataTest_TC_Sw_GroundCore_Classes_0010_A1_B()
        {
            //PIS.Ground.Core.Data.SystemInfo actual = null;
            //actual = PIS.Ground.Core.T2G.LocalDataStorage.Instance.GetSystemData("TRAIN");
            //Assert.AreEqual(null, actual);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for GetSystemData
        ///</summary>
        [TestMethod()]
        public void GetSystemDataTest_TC_Sw_GroundCore_Classes_0010_A1_C()
        {
            //PIS.Ground.Core.Data.SystemInfo actual = null;
            //actual = PIS.Ground.Core.T2G.LocalDataStorage.Instance.GetSystemData("TRAIN");
            //Assert.AreEqual(null, actual);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for UpdateServiceList
        ///</summary>
        [TestMethod()]
        public void UpdateServiceListTest_TC_Sw_GroundCore_Classes_0010_A2_A()
        {
            //bool test=false;
            //test = PIS.Ground.Core.T2G.LocalDataStorage.Instance.UpdateServiceList(null,"",true);
            //Assert.AreEqual(false, test);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for UpdateServiceList
        ///</summary>
        [TestMethod()]
        public void UpdateServiceListTest_TC_Sw_GroundCore_Classes_0010_A2_B()
        {
            //bool test = false;
            //PIS.Ground.Core.Data.ServiceInfo objServiceInfo = new PIS.Ground.Core.Data.ServiceInfo();
            //test = PIS.Ground.Core.T2G.LocalDataStorage.Instance.UpdateServiceList(objServiceInfo, "", true);
            //Assert.AreEqual(false, test);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for UpdateServiceList
        ///</summary>
        [TestMethod()]
        public void UpdateServiceListTest_TC_Sw_GroundCore_Classes_0010_A2_C()
        {
            //bool test = false;
            //PIS.Ground.Core.Data.ServiceInfo objServiceInfo = new PIS.Ground.Core.Data.ServiceInfo();
            //test = PIS.Ground.Core.T2G.LocalDataStorage.Instance.UpdateServiceList(objServiceInfo, "TRAIN", true);
            //Assert.AreEqual(false, test);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for BuildPisBaseLine
        ///</summary>
        [TestMethod()]
        public void BuildPisBaseLineListTest_TC_Sw_GroundCore_Classes_0010_A3_A()
        {
            //bool test = false;
            //PIS.Ground.Core.T2G.fieldStruct[] fieldList = null;
            //PIS.Ground.Core.Data.SystemInfo objSystemInfo = new PIS.Ground.Core.Data.SystemInfo();
            //test = PIS.Ground.Core.T2G.LocalDataStorage.Instance.BuildPisBaseLine(fieldList, objSystemInfo);
            //Assert.AreEqual(false, test);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for BuildPisBaseLine
        ///</summary>
        [TestMethod()]
        public void BuildPisBaseLineTest_TC_Sw_GroundCore_Classes_0010_A3_B()
        {
            //bool test = false;
            //PIS.Ground.Core.T2G.fieldStruct[] fieldList = new fieldStruct[0];
            //PIS.Ground.Core.Data.SystemInfo objSystemInfo = new PIS.Ground.Core.Data.SystemInfo(); 
            //test = PIS.Ground.Core.T2G.LocalDataStorage.Instance.BuildPisBaseLine(fieldList, objSystemInfo);
            //Assert.AreEqual(false, test);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for BuildPisBaseLine
        ///</summary>
        [TestMethod()]
        public void BuildPisBaseLineTest_TC_Sw_GroundCore_Classes_0010_A3_C()
        {
            //bool test = false;
            //PIS.Ground.Core.T2G.fieldStruct[] fieldList = new fieldStruct[2];
            //PIS.Ground.Core.Data.SystemInfo objSystemInfo = null;
            //test = PIS.Ground.Core.T2G.LocalDataStorage.Instance.BuildPisBaseLine(fieldList, objSystemInfo);
            //Assert.AreEqual(false, test);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for BuildPisBaseLine
        ///</summary>
        [TestMethod()]
        public void BuildPisBaseLineTest_TC_Sw_GroundCore_Classes_0010_A3_D()
        {
            //bool test = false;
            //PIS.Ground.Core.T2G.fieldStruct[] fieldList = new fieldStruct[1];
            //fieldStruct st=new fieldStruct();
            //st.id="1";
            //st.type=fieldTypeEnum.boolean;
            //st.value="true";
            //fieldList[0] = st;
            //PIS.Ground.Core.Data.SystemInfo objSystemInfo = new PIS.Ground.Core.Data.SystemInfo(); 
            //test = PIS.Ground.Core.T2G.LocalDataStorage.Instance.BuildPisBaseLine(fieldList, objSystemInfo);
            //Assert.AreEqual(true, test);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for BuildAvailabeElement
        ///</summary>
        [TestMethod()]
        public void BuildAvailabeElementTest_TC_Sw_GroundCore_Classes_0010_A4_A()
        {
            //PIS.Ground.Core.Data.ElementEventArgs objElementEventArgs = null;
            //PIS.Ground.Core.Data.SystemInfo objSystemInfo = null;
            //objElementEventArgs = PIS.Ground.Core.T2G.LocalDataStorage.Instance.BuildAvailabeElement(objSystemInfo);
            //Assert.AreEqual(null, objElementEventArgs);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for BuildAvailabeElement
        ///</summary>
        [TestMethod()]
        public void BuildAvailabeElementTest_TC_Sw_GroundCore_Classes_0010_A4_B()
        {
            //PIS.Ground.Core.Data.ElementEventArgs objElementEventArgs = null;
            //PIS.Ground.Core.Data.SystemInfo objSystemInfo = new PIS.Ground.Core.Data.SystemInfo();
            //objElementEventArgs = PIS.Ground.Core.T2G.LocalDataStorage.Instance.BuildAvailabeElement(objSystemInfo);
            //Assert.AreNotEqual(null, objElementEventArgs);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for InitializeService
        ///</summary>
        [TestMethod()]
        public void InitializeServiceTest_TC_Sw_GroundCore_Classes_0010_A5_A()
        {
            //bool res = PIS.Ground.Core.T2G.LocalDataStorage.Instance.InitializeService();
            //Assert.AreEqual(false, res);
            Assert.Fail(); //OBSOLETE
        }

        /// <summary>
        ///A test for InitializeService
        ///</summary>
        [TestMethod()]
        public void InitializeServiceTest_TC_Sw_GroundCore_Classes_0010_A5_B()
        {
            //bool res = PIS.Ground.Core.T2G.LocalDataStorage.Instance.InitializeService();
            //Assert.AreEqual(false, res);
            Assert.Fail(); //OBSOLETE
        }
       
    }
}
