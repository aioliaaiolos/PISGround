using PIS.Ground.Core.T2G;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for T2GContextAttributeTest and is intended
    ///to contain all T2GContextAttributeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class T2GContextAttributeTest
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
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void T2GContextAttributeTest_TC_Sw_GroundCore_Classes_0006_A()
        {
            T2GContextAttribute obj = new T2GContextAttribute();
            UpdateConfigKey("T2GServiceUserName", "T2GServiceUserName1", "admin");
            Assert.AreNotEqual(false, PIS.Ground.Core.Utility.T2GConfiguration.InitializeT2GConfig());
            UpdateConfigKey("T2GServiceUserName1", "T2GServiceUserName", "admin");
        }
         /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void T2GContextAttributeTest_TC_Sw_GroundCore_Classes_0006_B()
        {
            T2GContextAttribute obj = new T2GContextAttribute();
            UpdateConfigKey("T2G_Password", "T2G_Password1", "admin");
            Assert.AreNotEqual(false, PIS.Ground.Core.Utility.T2GConfiguration.InitializeT2GConfig());
            UpdateConfigKey("T2G_Password1", "T2G_Password", "admin");
        }
         /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void T2GContextAttributeTest_TC_Sw_GroundCore_Classes_0006_C()
        {
            T2GContextAttribute obj = new T2GContextAttribute();

            UpdateConfigKey("T2G_NotificationUrl", "T2G_NotificationUrl1", "http://10.0.16.101/Maintenance/NotificationService.svc");
            Assert.AreNotEqual(false, PIS.Ground.Core.Utility.T2GConfiguration.InitializeT2GConfig());
            UpdateConfigKey("T2G_NotificationUrl1", "T2G_NotificationUrl", "http://10.0.16.101/Maintenance/NotificationService.svc");
        }
         /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void T2GContextAttributeTest_TC_Sw_GroundCore_Classes_0006_D()
        {
            T2GContextAttribute obj = new T2GContextAttribute();
            UpdateConfigKey("ApplicationId", "ApplicationId1", "PIS.Maintenance");
            Assert.AreNotEqual(false, PIS.Ground.Core.Utility.T2GConfiguration.InitializeT2GConfig());
            UpdateConfigKey("ApplicationId1", "ApplicationId", "PIS.Maintenance");
        }

        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void T2GContextAttributeTest_TC_Sw_GroundCore_Classes_0006_E()
        {
            T2GContextAttribute obj = new T2GContextAttribute();
            Assert.AreNotEqual(true, PIS.Ground.Core.Utility.T2GConfiguration.InitializeT2GConfig());
        }

        internal static void UpdateConfigKey(string oldkey, string newkey, string val)
        {
            // Initilise Session Time Out
            if (ConfigurationManager.AppSettings[oldkey] != null)
            {
                // Open App.Config of executable
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Add an Application Setting.
                config.AppSettings.Settings.Remove(oldkey);
                config.AppSettings.Settings.Add(newkey, val);
                // Save the configuration file.
                config.Save(ConfigurationSaveMode.Modified, true);
                // Force a reload of a changed section.
                ConfigurationManager.RefreshSection("appSettings");
            }


        }
        internal static void UpdateConfigValue(string key, string val)
        {
            // Initilise Session Time Out
            if (ConfigurationManager.AppSettings[key] != null)
            {
                // Open App.Config of executable
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Add an Application Setting.
                config.AppSettings.Settings.Remove(key);
                config.AppSettings.Settings.Add(key, val);
                // Save the configuration file.
                config.Save(ConfigurationSaveMode.Modified, true);
                // Force a reload of a changed section.
                ConfigurationManager.RefreshSection("appSettings");
            }


        }
        
    }
}
