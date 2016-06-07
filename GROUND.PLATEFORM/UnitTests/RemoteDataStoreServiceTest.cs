using PIS.Ground.RemoteDataStore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    /// <summary>
    ///This is a test class for RemoteDataStoreServiceTest and is intended
    ///to contain all RemoteDataStoreServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RemoteDataStoreServiceTest
    {
        private TestContext testContextInstance;
        private static string _testResourceDir;
        private static string _dataStorePath;

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
            _testResourceDir = System.IO.Path.GetFullPath(System.Configuration.ConfigurationSettings.AppSettings["TestResourceDir"]);
            _dataStorePath = System.IO.Path.GetFullPath(System.Configuration.ConfigurationSettings.AppSettings["DataStorePath"]);
            System.IO.File.Copy(_testResourceDir + @"\UnitTestDB.db", System.Configuration.ConfigurationSettings.AppSettings["DataBaseFile"]);
            System.IO.File.SetAttributes(System.Configuration.ConfigurationSettings.AppSettings["DataBaseFile"], System.IO.FileAttributes.Normal);
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            string[] toDel = System.IO.Directory.GetFiles(_dataStorePath, "*.*", System.IO.SearchOption.AllDirectories);
            foreach (string file in toDel)
            {
                System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
                System.IO.File.Delete(file);
            }
            toDel = System.IO.Directory.GetDirectories(_dataStorePath, "*", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string dir in toDel)
            {
                System.IO.Directory.Delete(dir, true);
            }
        }
        
        //Use TestInitialize to run code before running each test
        /*[TestInitialize()]
        public void MyTestInitialize()
        {            
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {            
        }*/
        
        #endregion


        /// <summary>
        ///A test for DBAccess
        ///</summary>
        [TestMethod()]
        public void DBAccessTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            DatabaseAccessImplClass actual;
            actual = target.DBAccess;
            Assert.AreNotEqual(null, actual);
        }

        /// <summary>
        ///A test for DataStorePath
        ///</summary>
        [TestMethod()]
        public void DataStorePathTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            string actual;
            actual = target.DataStorePath;
            Assert.IsTrue(System.IO.Directory.Exists(actual));
        }

        /// <summary>
        ///A test for DataPackagesTypes
        ///</summary>
        [TestMethod()]
        public void DataPackagesTypesTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            string ltypes = System.Configuration.ConfigurationSettings.AppSettings["DataPackagesTypes"];
            List<string> expected = new List<string>();
            foreach (string ltype in ltypes.Split(','))
            {
                expected.Add(ltype);
            }
            expected.Sort();
            List<string> actual;
            actual = target.DataPackagesTypes;
            actual.Sort();
            for (int i = 0; i < actual.Count; i++)
            {
                if (i < expected.Count)
                {
                    Assert.AreEqual(expected[i], actual[i]);
                }
                else
                {
                    Assert.AreEqual(expected, actual);
                }
            }
        }

        /// <summary>
        ///A test for setNewDataPackage and deleteDataPackage
        ///</summary>
        [TestMethod()]
        public void setNewDataPackageAnddeleteDataPackageTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            Guid pReqID = new Guid();
            DataContainer pNDPkg = new DataContainer();
            pNDPkg.Columns = new List<string>();
            pNDPkg.Rows = new List<string>();
            pNDPkg.Columns.Add("DataPackageType");
            pNDPkg.Columns.Add("DataPackageVersion");
            pNDPkg.Columns.Add("DataPackagePath");

            string lVersion = "11.12.13.14";
            pNDPkg.Rows.Add("PISBASE");
            pNDPkg.Rows.Add(lVersion);
            pNDPkg.Rows.Add(@"\PISBASE\PISBase-11.12.13.14.zip");

            Assert.IsFalse(target.checkIfDataPackageExists("PISBASE", lVersion));

            target.setNewDataPackage(pNDPkg);
            Assert.IsTrue(target.checkIfDataPackageExists("PISBASE", lVersion));
            
            target.deleteDataPackage(pReqID,"PISBASE" , "11.12.13.14");
            Assert.IsFalse(target.checkIfDataPackageExists("PISBASE", lVersion));
        }

        /// <summary>
        ///A test for setNewBaselineDefinition deleteBaselineDefinition
        ///</summary>
        [TestMethod()]
        public void setNewBaselineDefinitionAndDeleteBaselineDefinitionTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            Guid pReqID = new Guid();
            DataContainer pNBLDef = new DataContainer();
            pNBLDef.Columns = new List<string>();
            pNBLDef.Rows = new List<string>();
            pNBLDef.Columns.Add("BaselineVersion");
            pNBLDef.Columns.Add("BaselineDescription");
            pNBLDef.Columns.Add("BaselineCreationDate");
            pNBLDef.Columns.Add("PISBaseDataPackageVersion");
            pNBLDef.Columns.Add("PISMissionDataPackageVersion");
            pNBLDef.Columns.Add("PISInfotainmentDataPackageVersion");
            pNBLDef.Columns.Add("LMTDataPackageVersion");

            string lBaselineVer = "11.12.13.14";
            pNBLDef.Rows.Add(lBaselineVer);
            pNBLDef.Rows.Add("SetNewBaselineTest");
            pNBLDef.Rows.Add(DateTime.Now.ToString());
            pNBLDef.Rows.Add("11.12.13.14");
            pNBLDef.Rows.Add("11.12.13.14");
            pNBLDef.Rows.Add("11.12.13.14");
            pNBLDef.Rows.Add("11.12.13.14");

            Assert.IsFalse(target.checkIfBaselineExists(lBaselineVer));

            target.setNewBaselineDefinition(pReqID, pNBLDef);
            Assert.IsTrue(target.checkIfBaselineExists(lBaselineVer));

            target.deleteBaselineDefinition(lBaselineVer);
            Assert.IsFalse(target.checkIfBaselineExists(lBaselineVer));
        }

        /// <summary>
        ///A test for mVersionMatch
        ///</summary>
        [TestMethod()]
        [DeploymentItem("RemoteDataStore.exe")]
        public void mVersionMatchTest()
        {
            RemoteDataStoreService_Accessor target = new RemoteDataStoreService_Accessor();
            string pVersion = "11.12.13.14";
            bool expected = true;
            bool actual;
            actual = target.mVersionMatch(pVersion);
            Assert.AreEqual(expected, actual);
            pVersion = "1.0.0";
            expected = false;
            actual = target.mVersionMatch(pVersion);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getElementBaselinesDefinitions
        ///</summary>
        [TestMethod()]
        public void getElementBaselinesDefinitionsTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            string pEID = "TRAIN1";
            DataContainer expected = new DataContainer();
            expected.Columns = new List<string>();
            expected.Rows = new List<string>();
            expected.Columns.Add("ElementID");
            expected.Columns.Add("AssignedCurrentBaseline");
            expected.Columns.Add("AssignedCurrentBaselineExpirationDate");
            expected.Columns.Add("AssignedFutureBaseline");
            expected.Columns.Add("AssignedFutureBaselineActivationDate");
            expected.Columns.Add("AssignedFutureBaselineExpirationDate");
            expected.Columns.Add("UndefinedBaselinePISBaseVersion");
            expected.Columns.Add("UndefinedBaselinePISMissionVersion");
            expected.Columns.Add("UndefinedBaselinePISInfotainmentVersion");
            expected.Columns.Add("UndefinedBaselineLmtVersion");

            expected.Rows.Add(pEID);
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("2011-09-17 14:21:40");
            expected.Rows.Add("1.0.0.1");
            expected.Rows.Add("2011-09-17 14:22:00");
            expected.Rows.Add("2011-09-23 18:43:58");
            expected.Rows.Add("");
            expected.Rows.Add("");
            expected.Rows.Add("");
            expected.Rows.Add("");

            DataContainer actual;
            actual = target.getElementBaselinesDefinitions(pEID);
            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for getDiffDataPackageUrl
        ///</summary>
        [TestMethod()]
        public void getDiffDataPackageUrlTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();

            System.IO.File.Copy(System.IO.Path.Combine(_testResourceDir, "PISBase-1.0.0.0.zip"), System.IO.Path.Combine(_dataStorePath, @"PISBASE\PISBase-1.0.0.0.zip"));
            System.IO.File.Copy(System.IO.Path.Combine(_testResourceDir, "PISBase-1.0.0.1.zip"), System.IO.Path.Combine(_dataStorePath, @"PISBASE\PISBase-1.0.0.1.zip"));

            Guid pReqId = new Guid();
            string pEID = "TRAIN1";
            string pDPType = "PISBASE";
            string pDPVersionOnBoard = "1.0.0.0";
            string pDPVersionOnGround = "1.0.0.1";
            string actual;
            actual = target.getDiffDataPackageUrl(pReqId, pEID, pDPType, pDPVersionOnBoard, pDPVersionOnGround);
            actual = System.IO.Path.Combine(_dataStorePath, actual.Replace("/", @"\"));
            Assert.IsTrue(System.IO.File.Exists(actual));
            
        }

        /// <summary>
        ///A test for getDataPackages
        ///</summary>
        [TestMethod()]
        public void getDataPackagesTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();

            DataContainer pBLDef = new DataContainer();
            pBLDef.Columns = new List<string>();
            pBLDef.Rows = new List<string>();
            pBLDef.Columns.Add("BaselineVersion");
            pBLDef.Rows.Add("1.0.0.0");
                        
            DataContainer expected = new DataContainer();
            expected.Columns = new List<string>();
            expected.Rows = new List<string>();
            expected.Columns.Add("DataPackageType");
            expected.Columns.Add("DataPackageVersion");
            expected.Columns.Add("DataPackagePath");
            expected.Rows.Add("PISBASE");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add(@"PISBASE\PISBase-1.0.0.0.zip");
            expected.Rows.Add("PISMISSION");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add(@"PISMISSION\PISMission-1.0.0.0.zip");
            expected.Rows.Add("PISINFOTAINMENT");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add(@"PISINFOTAINMENT\PISInfotainment-1.0.0.0.zip");
            expected.Rows.Add("LMT");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add(@"LMT\Lmt-1.0.0.0.zip");

            DataContainer actual;
            actual = target.getDataPackages(pBLDef);
            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for getDataPackagesList
        ///</summary>
        [TestMethod()]
        public void getDataPackagesListTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();

            DataContainer expected = new DataContainer();
            expected.Columns = new List<string>();
            expected.Rows = new List<string>();
            expected.Columns.Add("DataPackageType");
            expected.Columns.Add("DataPackageVersion");
            expected.Columns.Add("DataPackagePath");
            expected.Rows.Add("PISBASE");
            expected.Rows.Add("1.0.0.1");
            expected.Rows.Add(@"PISBASE\PISBase-1.0.0.1.zip");
            expected.Rows.Add("PISBASE");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add(@"PISBASE\PISBase-1.0.0.0.zip");
            expected.Rows.Add("PISMISSION");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add(@"PISMISSION\PISMission-1.0.0.0.zip");
            expected.Rows.Add("PISINFOTAINMENT");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add(@"PISINFOTAINMENT\PISInfotainment-1.0.0.0.zip");
            expected.Rows.Add("LMT");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add(@"LMT\Lmt-1.0.0.0.zip");

            DataContainer actual;
            actual = target.getDataPackagesList();
            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for getDataPackageCharacteristics
        ///</summary>
        [TestMethod()]
        public void getDataPackageCharacteristicsTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            string pDPType = "PISBASE";
            string pDPVersion = "1.0.0.0";

            DataContainer expected = new DataContainer();
            expected.Columns = new List<string>();
            expected.Rows = new List<string>();
            expected.Columns.Add("DataPackageType");
            expected.Columns.Add("DataPackageVersion");
            expected.Columns.Add("DataPackagePath");
            expected.Rows.Add("PISBASE");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add(@"PISBASE\PISBase-1.0.0.0.zip");

            DataContainer actual;
            actual = target.getDataPackageCharacteristics(pDPType, pDPVersion);
            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for getCurrentBaselineDefinition
        ///</summary>
        [TestMethod()]
        public void getCurrentBaselineDefinitionTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            string pEID = "TRAIN1";

            DataContainer expected = new DataContainer();
            expected.Columns = new List<string>();
            expected.Rows = new List<string>();
            expected.Columns.Add("BaselineVersion");
            expected.Columns.Add("BaselineDescription");
            expected.Columns.Add("BaselineCreationDate");
            expected.Columns.Add("PISBaseDataPackageVersion");
            expected.Columns.Add("PISMissionDataPackageVersion");
            expected.Columns.Add("PISInfotainmentDataPackageVersion");
            expected.Columns.Add("LMTDataPackageVersion");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("Baseline-1.0.0.0");
            expected.Rows.Add("2011-09-13 15:33:01");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("1.0.0.0");

            DataContainer actual;
            actual = target.getCurrentBaselineDefinition(pEID);
            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for getBaselinesDefinitions
        ///</summary>
        [TestMethod()]
        public void getBaselinesDefinitionsTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            DataContainer expected = new DataContainer();
            expected.Columns = new List<string>();
            expected.Rows = new List<string>();
            expected.Columns.Add("BaselineVersion");
            expected.Columns.Add("BaselineDescription");
            expected.Columns.Add("BaselineCreationDate");
            expected.Columns.Add("PISBaseDataPackageVersion");
            expected.Columns.Add("PISMissionDataPackageVersion");
            expected.Columns.Add("PISInfotainmentDataPackageVersion");
            expected.Columns.Add("LMTDataPackageVersion");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("Baseline-1.0.0.0");
            expected.Rows.Add("2011-09-13 15:33:01");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("1.0.0.1");
            expected.Rows.Add("Baseline-1.0.0.1");
            expected.Rows.Add("2011-09-14 8:33:01");
            expected.Rows.Add("1.0.0.1");
            expected.Rows.Add("1.0.0.1");
            expected.Rows.Add("1.0.0.1");
            expected.Rows.Add("1.0.0.1");

            DataContainer actual;
            actual = target.getBaselinesDefinitions();

            bool contains = false;
            actual.restart();
            while (actual.read())
            {
                if (actual.getStrValue("BaselineVersion") == expected.getStrValue("BaselineVersion"))
                {
                    if (actual.getStrValue("BaselineDescription") == expected.getStrValue("BaselineDescription"))
                    {
                        if (actual.getStrValue("BaselineCreationDate") == expected.getStrValue("BaselineCreationDate"))
                        {
                            if (actual.getStrValue("PISBaseDataPackageVersion") == expected.getStrValue("PISBaseDataPackageVersion"))
                            {
                                if (actual.getStrValue("PISMissionDataPackageVersion") == expected.getStrValue("PISMissionDataPackageVersion"))
                                {
                                    if (actual.getStrValue("PISInfotainmentDataPackageVersion") == expected.getStrValue("PISInfotainmentDataPackageVersion"))
                                    {
                                        if (actual.getStrValue("LMTDataPackageVersion") == expected.getStrValue("LMTDataPackageVersion"))
                                        {
                                            contains = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Assert.IsTrue(contains);
        }

        /// <summary>
        ///A test for getBaselineFilePath
        ///</summary>
        [TestMethod()]
        public void getBaselineFilePathTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            Guid pReqId = new Guid();
            string pEID = "TRAIN1";
            string actual = "";

            DataContainer lElDescr = target.getElementBaselinesDefinitions(pEID);
            DataContainer lBLDef = target.getBaselineDefinition(lElDescr.getStrValue("AssignedFutureBaseline"));

            actual = target.createBaselineFile(pReqId, pEID, lBLDef.getStrValue("BaselineVersion"), lElDescr.getStrValue("AssignedFutureBaselineActivationDate"), lElDescr.getStrValue("AssignedFutureBaselineExpirationDate"));

            actual = System.IO.Path.GetFullPath(_testResourceDir + @"\..\UnitTestFolder" + actual.Replace("/", @"\"));
            Assert.IsTrue(System.IO.File.Exists(actual));
        }

        /// <summary>
        ///A test for getBaselineDefinition
        ///</summary>
        [TestMethod()]
        public void getBaselineDefinitionTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            string pBLVersion = "1.0.0.0";

            DataContainer expected = new DataContainer();
            expected.Columns = new List<string>();
            expected.Rows = new List<string>();
            expected.Columns.Add("BaselineVersion");
            expected.Columns.Add("BaselineDescription");
            expected.Columns.Add("BaselineCreationDate");
            expected.Columns.Add("PISBaseDataPackageVersion");
            expected.Columns.Add("PISMissionDataPackageVersion");
            expected.Columns.Add("PISInfotainmentDataPackageVersion");
            expected.Columns.Add("LMTDataPackageVersion");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("Baseline-1.0.0.0");
            expected.Rows.Add("2011-09-13 15:33:01");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("1.0.0.0");

            DataContainer actual;
            actual = target.getBaselineDefinition(pBLVersion);
            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for getAssignedFutureBaselineVersion
        ///</summary>
        [TestMethod()]
        public void getAssignedFutureBaselineVersionTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            string pEID = "TRAIN1";
            string expected = "1.0.0.1";
            string actual;
            actual = target.getAssignedFutureBaselineVersion(pEID);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getAssignedCurrentBaselineVersion
        ///</summary>
        [TestMethod()]
        public void getAssignedCurrentBaselineVersionTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            string pEID = "TRAIN1";
            string expected = "1.0.0.0";
            string actual;
            actual = target.getAssignedCurrentBaselineVersion(pEID);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for deleteDataPackageDiffFile
        ///</summary>
        [TestMethod()]
        public void deleteDataPackageDiffFileTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            Guid pReqId = new Guid("936DA01F-9ABD-4d9d-80C7-02AF85C822A7");
            string pEID = "TRAIN1";

            string lDPDiffFile = _dataStorePath + @"\IncrementalDataPackages\" + pReqId.ToString();
            System.IO.Directory.CreateDirectory(lDPDiffFile + @"\" + pEID);

            target.deleteDataPackageDiffFile(pReqId, pEID);
            Assert.IsFalse(System.IO.Directory.Exists(lDPDiffFile + @"\" + pEID));
            Assert.IsFalse(System.IO.Directory.Exists(lDPDiffFile));
        }

        /// <summary>
        ///A test for deleteBaselineFile
        ///</summary>
        [TestMethod()]
        public void deleteBaselineFileTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            Guid pReqId = new Guid("936DA01F-9ABD-4d9d-80C7-02AF85C822A8");
            string pEID = "TRAIN1";
            string lDirToDel = _dataStorePath + @"\BaselinesDefinitions\" + pReqId.ToString();
            System.IO.Directory.CreateDirectory(lDirToDel + @"\" + pEID);

            target.deleteBaselineFile(pReqId, pEID);
            Assert.IsFalse(System.IO.Directory.Exists(lDirToDel + @"\" + pEID));
            Assert.IsFalse(System.IO.Directory.Exists(lDirToDel));
        }

        /// <summary>
        ///A test for checkUrl
        ///</summary>
        [TestMethod()]
        public void checkUrlTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            Guid pReqID = new Guid();
            string pUrl = System.IO.Path.GetFullPath(_testResourceDir);
            pUrl = System.IO.Path.Combine(pUrl, @"PISBase-1.0.0.0.zip");
            pUrl = @"\\127.0.0.1\" + pUrl.Replace(':', '$');
            bool actual;
            actual = target.checkUrl(pReqID, pUrl);
            Assert.IsTrue(actual);
            pUrl = "ftp://nustufru:56/";
            actual = target.checkUrl(pReqID, pUrl);
            Assert.IsFalse(actual);
        }

        /// <summary>
        ///A test for checkIfElementExists
        ///</summary>
        [TestMethod()]
        public void checkIfElementExistsTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            string pEID = "TRAIN1";
            bool expected = true;
            bool actual;
            actual = target.checkIfElementExists(pEID);
            Assert.AreEqual(expected, actual);
            pEID = "TRAIN11";
            expected = false;
            actual = target.checkIfElementExists(pEID);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for checkIfDataPackageExists
        ///</summary>
        [TestMethod()]
        public void checkIfDataPackageExistsTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            string pDPType = "PISBASE";
            string pDPVersion = "1.0.0.0";
            bool expected = true;
            bool actual;
            actual = target.checkIfDataPackageExists(pDPType, pDPVersion);
            Assert.AreEqual(expected, actual);
            pDPVersion = "24.23.22.21";
            expected = false;
            actual = target.checkIfDataPackageExists(pDPType, pDPVersion);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for checkIfBaselineExists
        ///</summary>
        [TestMethod()]
        public void checkIfBaselineExistsTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            string pBLVersion = "1.0.0.0";
            bool expected = true;
            bool actual;
            actual = target.checkIfBaselineExists(pBLVersion);
            Assert.AreEqual(expected, actual);
            pBLVersion = "24.23.22.21";
            expected = false;
            actual = target.checkIfBaselineExists(pBLVersion);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for checkDataPackagesAvailability
        ///</summary>
        [TestMethod()]
        public void checkDataPackagesAvailabilityTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            Guid pReqID = new Guid();
            DataContainer pBLDefs = new DataContainer();
            pBLDefs.Columns = new List<string>();
            pBLDefs.Rows = new List<string>();
            pBLDefs.Columns.Add("BaselineVersion");
            pBLDefs.Columns.Add("BaselineDescription");
            pBLDefs.Columns.Add("BaselineCreationDate");
            pBLDefs.Columns.Add("PISBaseDataPackageVersion");
            pBLDefs.Columns.Add("PISMissionDataPackageVersion");
            pBLDefs.Columns.Add("PISInfotainmentDataPackageVersion");
            pBLDefs.Columns.Add("LMTDataPackageVersion");
            pBLDefs.Rows.Add("1.0.0.0");
            pBLDefs.Rows.Add("Baseline-1.0.0.0");
            pBLDefs.Rows.Add("2011-09-13 15:33:01");
            pBLDefs.Rows.Add("1.0.0.0");
            pBLDefs.Rows.Add("1.0.0.0");
            pBLDefs.Rows.Add("1.0.0.0");
            pBLDefs.Rows.Add("1.0.0.0");

            target.checkDataPackagesAvailability(pReqID, pBLDefs);
        }

        /// <summary>
        ///A test for assignAFutureBaselineToElement and unassignFutureBaselineFromElement
        ///</summary>
        [TestMethod()]
        public void assignAFutureBaselineToElementTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            Guid pReqID = new Guid();
            string pEID = "TRAIN3";
            string pBLVersion = "1.0.0.0";
            DateTime pActDate = DateTime.Now;
            DateTime pExpDate = DateTime.Now.AddDays(5);
            target.assignAFutureBaselineToElement(pReqID, pEID, pBLVersion, pActDate, pExpDate);
            Assert.IsTrue(target.checkIfElementExists(pEID));

            target.unassignFutureBaselineFromElement(pEID);
            Assert.IsTrue(target.checkIfElementExists(pEID));
        }

        /// <summary>
        ///A test for assignACurrentBaselineToElement and unassignCurrentBaselineFromElement
        ///</summary>
        [TestMethod()]
        public void assignACurrentBaselineToElementTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            Guid pReqID = new Guid();
            string pEID = "TRAIN3";
            string pBLVersion = "1.0.0.0";
            DateTime pExpDate = DateTime.Now;
            target.assignACurrentBaselineToElement(pReqID, pEID, pBLVersion, pExpDate);
            Assert.IsTrue(target.checkIfElementExists(pEID));

            target.unassignCurrentBaselineFromElement(pEID);
            Assert.IsTrue(target.checkIfElementExists(pEID));
        }

        /// <summary>
        ///A test for RemoteDataStoreService Constructor
        ///</summary>
        [TestMethod()]
        public void RemoteDataStoreServiceConstructorTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            Assert.AreEqual(4, target.DataPackagesTypes.Count);
            Assert.AreNotEqual(null, target.DBAccess);
            Assert.IsTrue(System.IO.Directory.Exists(target.DataStorePath));
        }        

        /// <summary>
        ///A test for getAssignedBaselinesVersions
        ///</summary>
        [TestMethod()]
        public void getAssignedBaselinesVersionsTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            

            DataContainer expected = new DataContainer();
            expected.Columns = new List<string>();
            expected.Rows = new List<string>();
            expected.Columns.Add("ElementID");
            expected.Columns.Add("AssignedCurrentBaseline");
            expected.Columns.Add("AssignedFutureBaseline");
            expected.Rows.Add("TRAIN1");
            expected.Rows.Add("1.0.0.0");
            expected.Rows.Add("1.0.0.1");
            expected.Rows.Add("TRAIN2");
            expected.Rows.Add("1.0.0.3");
            expected.Rows.Add("1.0.0.4");
            expected.Rows.Add("TRAIN3");
            expected.Rows.Add("");
            expected.Rows.Add("");

            DataContainer actual;
            actual = target.getAssignedBaselinesVersions();
            Assert.IsTrue(expected == actual);
        }

        

        /// <summary>
        ///A test for cleanFutureBaselineAssignation
        ///</summary>
        [TestMethod()]
        public void cleanFutureBaselineAssignationTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();

            string pEID = "TRAIN1";
            string expected = "1.0.0.1";
            string actual;

            actual = target.getAssignedFutureBaselineVersion(pEID);
            Assert.AreEqual(expected, actual);
            target.unassignFutureBaselineFromElement("TRAIN1");
            expected = "";
            actual = target.getAssignedFutureBaselineVersion(pEID);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for setElementUndefinedBaselineParams
        ///</summary>
        [TestMethod()]
        public void setElementUndefinedBaselineParamsTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();

            DataContainer actualElemDef;
            actualElemDef = target.getElementBaselinesDefinitions("TRAIN1");
            
            Assert.AreEqual("", actualElemDef.getStrValue("UndefinedBaselinePISBaseVersion"));
            Assert.AreEqual("", actualElemDef.getStrValue("UndefinedBaselinePISMissionVersion"));
            Assert.AreEqual("", actualElemDef.getStrValue("UndefinedBaselinePISInfotainmentVersion"));
            Assert.AreEqual("", actualElemDef.getStrValue("UndefinedBaselineLmtVersion"));            
            
            target.setElementUndefinedBaselineParams("TRAIN1", "1.1.1.1", "2.2.2.2", "3.3.3.3", "4.4.4.4");

            actualElemDef = target.getElementBaselinesDefinitions("TRAIN1");

            Assert.AreEqual("1.1.1.1", actualElemDef.getStrValue("UndefinedBaselinePISBaseVersion"));
            Assert.AreEqual("2.2.2.2", actualElemDef.getStrValue("UndefinedBaselinePISMissionVersion"));
            Assert.AreEqual("3.3.3.3", actualElemDef.getStrValue("UndefinedBaselinePISInfotainmentVersion"));
            Assert.AreEqual("4.4.4.4", actualElemDef.getStrValue("UndefinedBaselineLmtVersion"));

            target.setElementUndefinedBaselineParams("TRAIN1", "", "", "", "");

            actualElemDef = target.getElementBaselinesDefinitions("TRAIN1");

            Assert.AreEqual("", actualElemDef.getStrValue("UndefinedBaselinePISBaseVersion"));
            Assert.AreEqual("", actualElemDef.getStrValue("UndefinedBaselinePISMissionVersion"));
            Assert.AreEqual("", actualElemDef.getStrValue("UndefinedBaselinePISInfotainmentVersion"));
            Assert.AreEqual("", actualElemDef.getStrValue("UndefinedBaselineLmtVersion"));
                                            
        }

        /// <summary>
        ///A test for getElementsDescription
        ///</summary>
        [TestMethod()]
        public void getElementsDescriptionTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();

            DataContainer lElemList;
            lElemList = target.getElementsDescription();

            lElemList.restart();
            lElemList.read();

            Assert.AreEqual(lElemList.getStrValue("ElementID"), "TRAIN1");
            Assert.AreEqual(lElemList.getStrValue("AssignedCurrentBaseline"), "1.0.0.0");
            Assert.AreEqual(lElemList.getStrValue("AssignedCurrentBaselineExpirationDate"), "2011-09-17 14:21:40");
            Assert.AreEqual(lElemList.getStrValue("AssignedFutureBaseline"), "1.0.0.1");
            Assert.AreEqual(lElemList.getStrValue("AssignedFutureBaselineActivationDate"), "2011-09-17 14:22:00");
            Assert.AreEqual(lElemList.getStrValue("AssignedFutureBaselineExpirationDate"), "2011-09-23 18:43:58");

            lElemList.read();
            Assert.AreEqual(lElemList.getStrValue("ElementID"), "TRAIN2");
            Assert.AreEqual(lElemList.getStrValue("AssignedCurrentBaseline"), "1.0.0.3");
            Assert.AreEqual(lElemList.getStrValue("AssignedCurrentBaselineExpirationDate"), "2011-05-05 12:25:02");
            Assert.AreEqual(lElemList.getStrValue("AssignedFutureBaseline"), "1.0.0.4");
            Assert.AreEqual(lElemList.getStrValue("AssignedFutureBaselineActivationDate"), "2013-02-05 12:12:00");
            Assert.AreEqual(lElemList.getStrValue("AssignedFutureBaselineExpirationDate"), "2015-08-12 10:00:30");
        }

        /// <summary>
        ///A test for getElementsDescription
        ///</summary>
        [TestMethod()]
        public void getUndefinedBaselinesListTest()
        {
            RemoteDataStoreService target = new RemoteDataStoreService();
            
            target.setElementUndefinedBaselineParams("TRAIN1", "1.1.1.1", "2.2.2.2", "3.3.3.3", "4.4.4.4");
            target.setElementUndefinedBaselineParams("TRAIN2", "5.5.5.5", "6.6.6.6", "7.7.7.7", "8.8.8.8");

            DataContainer lBList = target.getUndefinedBaselinesList();

            lBList.restart();
            lBList.read();

            Assert.AreEqual("1.1.1.1", lBList.getStrValue("UndefinedBaselinePISBaseVersion"));
            Assert.AreEqual("2.2.2.2", lBList.getStrValue("UndefinedBaselinePISMissionVersion"));
            Assert.AreEqual("3.3.3.3", lBList.getStrValue("UndefinedBaselinePISInfotainmentVersion"));
            Assert.AreEqual("4.4.4.4", lBList.getStrValue("UndefinedBaselineLmtVersion"));

            lBList.read();

            Assert.AreEqual("5.5.5.5", lBList.getStrValue("UndefinedBaselinePISBaseVersion"));
            Assert.AreEqual("6.6.6.6", lBList.getStrValue("UndefinedBaselinePISMissionVersion"));
            Assert.AreEqual("7.7.7.7", lBList.getStrValue("UndefinedBaselinePISInfotainmentVersion"));
            Assert.AreEqual("8.8.8.8", lBList.getStrValue("UndefinedBaselineLmtVersion"));

            target.setElementUndefinedBaselineParams("TRAIN1", "", "", "", "");
            target.setElementUndefinedBaselineParams("TRAIN2", "", "", "", "");
        }
    }
}
