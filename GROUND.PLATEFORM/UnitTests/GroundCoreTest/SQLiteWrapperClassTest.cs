using PIS.Ground.Core.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for SQLiteWrapperClassTest and is intended
    ///to contain all SQLiteWrapperClassTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SQLiteWrapperClassTest
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
        ///A test for mUpdate
        ///</summary>
        [TestMethod()]
        public void mUpdateTest()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(); // TODO: Initialize to an appropriate value
            string pTableName = string.Empty; // TODO: Initialize to an appropriate value
            System.Collections.Generic.Dictionary<string, string> pData = null; // TODO: Initialize to an appropriate value
            string pWhere = string.Empty; // TODO: Initialize to an appropriate value
            target.mUpdate(pTableName, pData, pWhere);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for mInsert
        ///</summary>
        [TestMethod()]
        public void mInsertTest()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(); // TODO: Initialize to an appropriate value
            string pTableName = string.Empty; // TODO: Initialize to an appropriate value
            System.Collections.Generic.Dictionary<string, string> pData = null; // TODO: Initialize to an appropriate value
            target.mInsert(pTableName, pData);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for mExecuteTransactionQuery
        ///</summary>
        [TestMethod()]
        public void mExecuteTransactionQueryTest()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(); // TODO: Initialize to an appropriate value
            System.Collections.Generic.List<string> plstSql = null; // TODO: Initialize to an appropriate value
            target.mExecuteTransactionQuery(plstSql);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for mExecuteScalar
        ///</summary>
        [TestMethod()]
        public void mExecuteScalarTest()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(); // TODO: Initialize to an appropriate value
            string pSql = string.Empty; // TODO: Initialize to an appropriate value
            string pValue = string.Empty; // TODO: Initialize to an appropriate value
            string pValueExpected = string.Empty; // TODO: Initialize to an appropriate value
            target.mExecuteScalar(pSql, ref pValue);
            Assert.AreEqual(pValueExpected, pValue);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for mExecuteQuery
        ///</summary>
        [TestMethod()]
        public void mExecuteQueryTest()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(); // TODO: Initialize to an appropriate value
            string pSql = string.Empty; // TODO: Initialize to an appropriate value
            System.Data.DataTable pDt = null; // TODO: Initialize to an appropriate value
            System.Data.DataTable pDtExpected = null; // TODO: Initialize to an appropriate value
            target.mExecuteQuery(pSql, ref pDt);
            Assert.AreEqual(pDtExpected, pDt);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for mExecuteNonQuery
        ///</summary>
        [TestMethod()]
        public void mExecuteNonQueryTest()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(); // TODO: Initialize to an appropriate value
            string pSql = string.Empty; // TODO: Initialize to an appropriate value
            target.mExecuteNonQuery(pSql);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for mEntryExists
        ///</summary>
        [TestMethod()]
        public void mEntryExistsTest()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(); // TODO: Initialize to an appropriate value
            string pTable = string.Empty; // TODO: Initialize to an appropriate value
            System.Collections.Generic.Dictionary<string, string> pEntry = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.mEntryExists(pTable, pEntry);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for mDelete
        ///</summary>
        [TestMethod()]
        public void mDeleteTest()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(); // TODO: Initialize to an appropriate value
            string pTableName = string.Empty; // TODO: Initialize to an appropriate value
            string pWhere = string.Empty; // TODO: Initialize to an appropriate value
            target.mDelete(pTableName, pWhere);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for mCreateFile
        ///</summary>
        [TestMethod()]
        public void mCreateFileTest()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(); // TODO: Initialize to an appropriate value
            string pFile = string.Empty; // TODO: Initialize to an appropriate value
            target.mCreateFile(pFile);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for mClearTable
        ///</summary>
        [TestMethod()]
        public void mClearTableTest()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(); // TODO: Initialize to an appropriate value
            string pTable = string.Empty; // TODO: Initialize to an appropriate value
            target.mClearTable(pTable);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for mClearDB
        ///</summary>
        [TestMethod()]
        public void mClearDBTest()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(); // TODO: Initialize to an appropriate value
            target.mClearDB();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest1()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(); // TODO: Initialize to an appropriate value
            target.Dispose();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PIS.Ground.Core.dll")]
        public void DisposeTest()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass_Accessor target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass_Accessor(); // TODO: Initialize to an appropriate value
            bool disposing = false; // TODO: Initialize to an appropriate value
            target.Dispose(disposing);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SQLiteWrapperClass Constructor
        ///</summary>
        [TestMethod()]
        public void SQLiteWrapperClassConstructorTest2()
        {
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for SQLiteWrapperClass Constructor
        ///</summary>
        [TestMethod()]
        public void SQLiteWrapperClassConstructorTest1()
        {
            string pInputFile = string.Empty; // TODO: Initialize to an appropriate value
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(pInputFile);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for SQLiteWrapperClass Constructor
        ///</summary>
        [TestMethod()]
        public void SQLiteWrapperClassConstructorTest()
        {
            System.Collections.Generic.Dictionary<string, string> pConnectionOpts = null; // TODO: Initialize to an appropriate value
            PIS.Ground.Core.SQLite.SQLiteWrapperClass target = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(pConnectionOpts);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
