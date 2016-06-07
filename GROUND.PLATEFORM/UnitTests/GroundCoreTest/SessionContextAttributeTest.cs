using PIS.Ground.Core.SessionMgmt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

namespace PIS.Ground.Core.Utility
{
    
    
    /// <summary>
    ///This is a test class for SessionContextAttributeTest and is intended
    ///to contain all SessionContextAttributeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SessionContextAttributeTest
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
        public void SessionContextAttributeTest_TC_Sw_GroundCore_Classes_0002_A()
        {
            SessionContextAttribute obj = new SessionContextAttribute();
            UpdateConfigKey("SqlLiteSessionStorePath", "SqlLiteSessionStorePath1", "D://Session//SessionData.s3db");
            Assert.AreNotEqual(false,PIS.Ground.Core.Utility.T2GConfiguration.InitializeConfigPaths());
            UpdateConfigKey("SqlLiteSessionStorePath1", "SqlLiteSessionStorePath", "D://Session//SessionData.s3db");
        }

        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void SessionContextAttributeTest_TC_Sw_GroundCore_Classes_0002_B()
        {
            SessionContextAttribute obj = new SessionContextAttribute();
            UpdateConfigValue("SqlLiteSessionStorePath", "E://Session//SessionData.s3db");
            Assert.AreNotEqual(false, PIS.Ground.Core.Utility.T2GConfiguration.InitializeConfigPaths());
            UpdateConfigValue("SqlLiteSessionStorePath", "D://Session//SessionData.s3db");
        }

        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void SessionContextAttributeTest_TC_Sw_GroundCore_Classes_0002_C()
        {
            SessionContextAttribute obj = new SessionContextAttribute();
            UpdateConfigKey("SessionTimeOut", "SessionTimeOut1", "60");
            Assert.AreNotEqual(false, PIS.Ground.Core.Utility.T2GConfiguration.InitializeConfigPaths());
            UpdateConfigKey("SessionTimeOut1", "SessionTimeOut", "60");
        }

        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void SessionContextAttributeTest_TC_Sw_GroundCore_Classes_0002_D()
        {
            SessionContextAttribute obj = new SessionContextAttribute();
            UpdateConfigValue("SessionTimeOut", "-5");
            Assert.AreNotEqual(false, PIS.Ground.Core.Utility.T2GConfiguration.InitializeConfigPaths());
            UpdateConfigValue("SessionTimeOut", "60");
        }

        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void SessionContextAttributeTest_TC_Sw_GroundCore_Classes_0002_E()
        {
            SessionContextAttribute obj = new SessionContextAttribute();

            UpdateConfigValue("SessionTimeOut", "any");
            Assert.AreNotEqual(false, PIS.Ground.Core.Utility.T2GConfiguration.InitializeConfigPaths());
            UpdateConfigValue("SessionTimeOut", "60");
        }

        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void SessionContextAttributeTest_TC_Sw_GroundCore_Classes_0002_F()
        {
            SessionContextAttribute obj = new SessionContextAttribute();

            UpdateConfigKey("SessionCheckTimer", "SessionCheckTimer1", "30");
            Assert.AreNotEqual(false, PIS.Ground.Core.Utility.T2GConfiguration.InitializeConfigPaths());
            UpdateConfigKey("SessionCheckTimer1", "SessionCheckTimer", "30");
        }


        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void SessionContextAttributeTest_TC_Sw_GroundCore_Classes_0002_G()
        {
            SessionContextAttribute obj = new SessionContextAttribute();
            UpdateConfigValue("SessionCheckTimer", "any");
            Assert.AreNotEqual(false, PIS.Ground.Core.Utility.T2GConfiguration.InitializeConfigPaths());
            UpdateConfigValue("SessionCheckTimer", "30");
        }

        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void SessionContextAttributeTest_TC_Sw_GroundCore_Classes_0002_H()
        {
            SessionContextAttribute obj = new SessionContextAttribute();
            UpdateConfigValue("SessionCheckTimer", "-5");
            Assert.AreNotEqual(false, PIS.Ground.Core.Utility.T2GConfiguration.InitializeConfigPaths());
            UpdateConfigValue("SessionCheckTimer", "30");
        }

        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void SessionContextAttributeTest_TC_Sw_GroundCore_Classes_0002_I()
        {
            SessionContextAttribute obj = new SessionContextAttribute();
            Assert.AreNotEqual(true, PIS.Ground.Core.Utility.T2GConfiguration.InitializeConfigPaths());
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
