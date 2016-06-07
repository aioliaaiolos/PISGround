using PIS.Ground.Core.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for Crc32Test and is intended
    ///to contain all Crc32Test Unit Tests
    ///</summary>
    [TestClass()]
    public class Crc32Test
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
        ///A test for CalculateChecksum
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void CalculateChecksumTest_TC_Sw_GroundCore_Classes_0007_A1_A()
        {
            //PIS.Ground.Core.Utility.Crc32_Accessor target = new PIS.Ground.Core.Utility.Crc32_Accessor(); 
            //string actual;
            //actual = target.CalculateChecksum("");
            //Assert.AreEqual("0", actual);
            Assert.Fail("Obsolete");
        }

        /// <summary>
        ///A test for CalculateChecksum
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void CalculateChecksumTest_TC_Sw_GroundCore_Classes_0007_A1_B()
        {
            PIS.Ground.Core.Utility.Crc32_Accessor target = new PIS.Ground.Core.Utility.Crc32_Accessor(); 
            string actual;
            using (var str = new System.IO.FileStream("C:\\Test.txt", System.IO.FileMode.Open))
            {
                actual = target.CalculateChecksum(str);
            }
            Assert.AreEqual("0", actual);
        }

        /// <summary>
        ///A test for CalculateChecksum
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void CalculateChecksumTest_TC_Sw_GroundCore_Classes_0007_A1_C()
        {
            PIS.Ground.Core.Utility.Crc32_Accessor target = new PIS.Ground.Core.Utility.Crc32_Accessor(); 
            string actual;
            using (var str = new System.IO.FileStream("D:\\Viswanath\\architecture.pdf", System.IO.FileMode.Open))
            {
                actual = target.CalculateChecksum(str);
            }
            Assert.AreNotEqual("0", actual);
        }
    }
}
