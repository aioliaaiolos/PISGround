//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineDistributionIntegrationTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using DataPackageTests.ServicesStub;
using Moq;
using NUnit.Framework;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.SqlServerAccess;
using PIS.Ground.Core.T2G;
using PIS.Ground.Core.Utility;
using PIS.Ground.DataPackage;
using PIS.Ground.DataPackage.RemoteDataStoreFactory;
using PIS.Ground.DataPackage.RequestMgt;
using AcquisitionStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.acquisitionStateEnum;
using CommLinkEnum = DataPackageTests.T2GServiceInterface.Identification.commLinkEnum;
using FileInfoStruct = DataPackageTests.T2GServiceInterface.FileTransfer.fileInfoStruct;
using FileList = DataPackageTests.T2GServiceInterface.FileTransfer.fileList;
using FilePathStruct = DataPackageTests.T2GServiceInterface.FileTransfer.filePathStruct;
using FolderInfoStruct = DataPackageTests.T2GServiceInterface.FileTransfer.folderInfoStruct;
using LinkTypeEnum = DataPackageTests.T2GServiceInterface.Notification.linkTypeEnum;
using RecipientStruct = DataPackageTests.T2GServiceInterface.FileTransfer.recipientStruct;
using TaskPhaseEnum = DataPackageTests.T2GServiceInterface.FileTransfer.taskPhaseEnum;
using TaskStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.taskStateEnum;
using TaskSubStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.taskSubStateEnum;
using TransferStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.transferStateEnum;
using TransferTaskStruct = DataPackageTests.T2GServiceInterface.FileTransfer.transferTaskStruct;
using PIS.Ground.RemoteDataStore;
using DataPackageTests.Data;
using PIS.Ground.Core.Data;
using System.Configuration;
using System.Threading;


namespace DataPackageTests
{
    /// <summary>
    /// Class that test baseline distribution scenarios from distribute baseline until completion on embedded system.
    /// 
    /// This class simulate T2G, services on train to perform a complete validation of expected status of every stages of a baseline distribution.
    /// This class also validate the history log database and the notifications send by PIS-Ground.
    /// </summary>
    [TestFixture, Category("DistributionScenario")]
    class BaselineDistributionIntegrationTests : AssertionHelper
    {
        #region Fields


        public const int ONE_SECOND = 1000;

        // Define the IP address to use. Change it if you would like to debug with a proxy server.
        public const string DEFAULT_IP = "127.0.0.1";

        public const string IdentificationServiceUrl = "http://" + DEFAULT_IP + ":5000/T2G/Identification.asmx";
        public const string FileTransferServiceUrl = "http://" + DEFAULT_IP + ":5000/T2G/FileTransfer.asmx";
        public const string VehicleInfoServiceUrl = "http://" + DEFAULT_IP + ":5000/T2G/VehicleInfo.asmx";
        public const string PisGroundNotificationServiceUrl = "http://" + DEFAULT_IP + ":5002/PIS_GROUND/notification.svc";

        public const string TRAIN_NAME_1 = "TRAIN-1";
        public const int TRAIN_VEHICLE_ID_1 = 1;
        public const string TRAIN_IP_1 = DEFAULT_IP;
        public const ushort TRAIN_DATA_PACKAGE_PORT_1 = 4000;

        public const string DEFAULT_BASELINE = "3.0.0.0";
        public const string DEFAULT_MISSION = "";
        public const CommLinkEnum DEFAULT_COMMUNICATION_LINK = CommLinkEnum.wifi;
        public const string DEFAULT_OPERATOR_CODE = "77";

        public const string DEFAULT_PIS_VERSION = "5.16.3.2";

        public const string SERVICE_NAME_DATA_PACKAGE = "PIS2G DataPackage";
        public const ushort DEFAULT_CAR_ID = 1;

        public const string BASELINE_STATUS_UNKNOWN = "UNKNOWN";

        private T2GFileTransferServiceStub _fileTransferServiceStub;
        private T2GIdentificationServiceStub _identificationServiceStub;
        private T2GVehicleInfoServiceStub _vehicleInfoServiceStub;
        private T2GNotificationServiceStub _notificationServiceStub;
        private TrainDataPackageServiceStub _trainDataPackageServiceStub;
        private DataPackageServiceStub _datapackageServiceStub;
        private RemoteDataStoreServiceStub _dataStoreServiceStub;
        private ServiceHost _hostIdentificationService;
        private ServiceHost _hostFileTransferService;
        private ServiceHost _hostVehicleInfoService;
        private ServiceHost _hostNotificationService;
        private ServiceHost _hostTrainDataPackageService;
        private IT2GManager _t2gManager;

        private const string DatabaseName="TestDatabaseDataPackage";
        private string _databaseFilePath = string.Empty;
        private string _databaseLogPath = string.Empty;
        private string _databaseFolderPath = string.Empty;
        private string _databaseName = string.Empty;
        private bool _databaseConfigurationValid = false;

        /// <summary>The notification sender mock.</summary>
        private Mock<INotificationSender> _notificationSenderMock;
        private Mock<IRemoteDataStoreFactory> _remoteDataStoreFactoryMock;
        //private Mock<IRemoteDataStore> _remoteDataStoreMock;
        private SessionManager _sessionManager;
        private RequestContextFactory _requestFactory;
        private RequestManager _requestManager;
        private Guid _pisGroundSessionId;

        #region RemoteDataStoreData

/*        private object _dataStoreLock = new object();

        /// <summary>
        /// The content of table ElementsDataStore in remoteDataStore indexed by system id
        /// </summary>
        private Dictionary<string, ElementsDataStoreData> _dataStoreElementsData = new Dictionary<string, ElementsDataStoreData>(10, StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, BaselinesDataStoreData> _dateStoreBaselinesData = new Dictionary<string, BaselinesDataStoreData>(10, StringComparer.Ordinal);*/
        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaselineDistributionIntegrationTests"/> class.
        /// </summary>
        public BaselineDistributionIntegrationTests()
        {
            // No logic to execute
        }

        #endregion

        #region Tests managment

        /// <summary>
        /// Test fixture setup.
        /// </summary>
        [TestFixtureSetUp]
        public void FixtureInit()
        {
            ConfigurationSettings.AppSettings["DataStorePath"] = Path.Combine(ServiceConfiguration.AppDataPath, "RemoteDataStore");
            ConfigurationSettings.AppSettings["RemoteDataStoreUrl"] = Path.Combine(ServiceConfiguration.AppDataPath, "RemoteDataStore");


            if (!HistoryLoggerConfiguration.Valid)
            {
                throw new Exception("Test application is misconfigured. The history logger configuration is invalid");
            }
            if (!HistoryLoggerConfiguration.Used)
            {
                throw new Exception("Test application is misconfigured. The history logger configuration is not set to used");
            }

            Console.Out.Write("ServiceConfiguration.SessionSqLiteDBPath=\"");
            Console.Out.Write(ServiceConfiguration.SessionSqLiteDBPath);
            Console.Out.WriteLine("\"");

            Console.Out.Write("HistoryLoggerConfiguration.LogBackupPath=\"");
            Console.Out.Write(HistoryLoggerConfiguration.LogBackupPath);
            Console.Out.WriteLine("\"");
            Console.Out.Write("HistoryLoggerConfiguration.CreateTableScriptPath=\"");
            Console.Out.Write(HistoryLoggerConfiguration.CreateTableScriptPath);
            Console.Out.WriteLine("\"");
            SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(HistoryLoggerConfiguration.SqlConnectionString);

            Console.Out.Write("HistoryLoggerConfiguration.DatabaseName=\"");
            Console.Out.Write(connectionBuilder.InitialCatalog);
            Console.Out.WriteLine("\"");
            Console.Out.Write("HistoryLoggerConfiguration.AttachDBFilename=\"");
            Console.Out.Write(connectionBuilder.AttachDBFilename);
            Console.Out.WriteLine("\"");

            if (string.IsNullOrEmpty(connectionBuilder.AttachDBFilename))
            {
                throw new Exception("Test application is misconfigured: the sql connection string of history logger configuration does not have AttachDBFilename value set.");
            }
            if (string.IsNullOrEmpty(connectionBuilder.InitialCatalog))
            {
                throw new Exception("Test application is misconfigured: the sql connection string of history logger configuration does not have Database value set.");
            }

            _databaseName = connectionBuilder.InitialCatalog;
            _databaseFilePath = connectionBuilder.AttachDBFilename;
            _databaseFolderPath = Path.GetDirectoryName(_databaseFilePath);
            _databaseLogPath = Path.Combine(_databaseFolderPath, Path.GetFileNameWithoutExtension(_databaseFilePath) + "_log.ldf");



            if (!Directory.Exists(HistoryLoggerConfiguration.LogBackupPath))
            {
                Directory.CreateDirectory(HistoryLoggerConfiguration.LogBackupPath);
            }

            if (!File.Exists(HistoryLoggerConfiguration.CreateTableScriptPath))
            {
                throw new Exception("Test application is misconfigured: the create table script of history logger configuration does not exist at this location: \"" + HistoryLoggerConfiguration.CreateTableScriptPath + "\".");
            }

            if (!Directory.Exists(_databaseFolderPath))
            {
                Directory.CreateDirectory(_databaseFolderPath);
            }

            if (!File.Exists(ServiceConfiguration.SessionSqLiteDBPath))
            {
                throw new Exception("The session database does not exist at this location: \"" + ServiceConfiguration.SessionSqLiteDBPath + "\".");
            }
            
            // Remove the readonly attribute on session database table if set.
            FileAttributes attributes = File.GetAttributes(ServiceConfiguration.SessionSqLiteDBPath);
            if ((attributes& FileAttributes.ReadOnly) != 0)
            {
                attributes = attributes& (~FileAttributes.ReadOnly);
                File.SetAttributes(ServiceConfiguration.SessionSqLiteDBPath, attributes);
            }


            // Verify that connection with SQL Server can be established.
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(HistoryLoggerConfiguration.SqlCreateDbConnectionString))
                {
                    dbConnection.Open();
                }
            }
            catch (System.Exception ex)
            {
                throw new Exception("Check that SQL server is running or the history logger database configuration because the application cannot connect to SQL server instance:" + ex.Message, ex);
            }

            _databaseConfigurationValid = true;
            DropTestDb();
            RemoteFileClass.TestingModeEnabled = true;

            HistoryLogger.Initialize();
        }

        /// <summary>Test fixture cleanup.</summary>
        [TestFixtureTearDown]
        public void MyCleanup()
        {
            RemoteFileClass.TestingModeEnabled = false;
            DropTestDb();
        }


        /// <summary>Setups called before each test to initialize variables.</summary>
        [SetUp]
        public void Setup()
        {
            TestContext currentContext = TestContext.CurrentContext;

            if (currentContext.Test.Name.Contains("Scenario"))
            {
                Console.Out.WriteLine("===================================");
                Console.Out.WriteLine("BEGIN TEST {0}", currentContext.Test.Name);
                Console.Out.WriteLine("===================================");
            }

            _notificationSenderMock = new Mock<INotificationSender>();
            _remoteDataStoreFactoryMock = new Mock<IRemoteDataStoreFactory>();
        }

        /// <summary>Tear down called after each test to clean.</summary>
        [TearDown]
        public void TearDown()
        {
            if (_datapackageServiceStub != null)
            {
                _datapackageServiceStub.Dispose();
                _datapackageServiceStub = null;
            }

            if (_requestManager != null)
            {
                _requestManager.Uninitialize();
                _requestManager = null;
            }

            foreach (ServiceHost service in new ServiceHost[] { _hostVehicleInfoService, _hostFileTransferService, _hostIdentificationService, _hostNotificationService, _hostTrainDataPackageService })
            {
                if (service == null)
                    continue;

                if (service.State == CommunicationState.Faulted)
                {
                    service.Abort();
                    service.Close();
                }
            }
            _hostIdentificationService = null;
            _hostFileTransferService = null;
            _hostVehicleInfoService = null;
            _hostNotificationService = null;
            _hostTrainDataPackageService = null;
            _fileTransferServiceStub = null;
            _identificationServiceStub = null;
            _vehicleInfoServiceStub = null;
            _notificationServiceStub = null;
            DataPackageService.Uninitialize();
            T2GManagerContainer.T2GManager = null;
            _t2gManager = null;
            _sessionManager = null;
            _requestFactory = null;

            if (_dataStoreServiceStub != null)
            {
                _dataStoreServiceStub.Dispose();
                _dataStoreServiceStub = null;
            }

            _remoteDataStoreFactoryMock = null;

            TestContext currentContext = TestContext.CurrentContext;

            if (currentContext.Test.Name.Contains("Scenario"))
            {
                Console.Out.WriteLine("===================================");
                Console.Out.WriteLine("END TEST {0}", currentContext.Test.Name);
                Console.Out.WriteLine("===================================");
            }
        }
        #endregion

        #region Test - Service stub test

        /// <summary>
        /// Perform sanity check on identification service stub to ensure that it can be used for testing purpose
        /// </summary>
        [Test, Category("SanityCheck")]
        public void VerifyIdentificationServiceStub()
        {
            T2GIdentificationServiceStub identificationService = new T2GIdentificationServiceStub();

            Assert.False(identificationService.IsSessionValid(1), "IsSessionValid return the wrong result on unknown session id");
            Assert.False(identificationService.IsSystemExist("TRAIN-1"), "IsSystemExist return the wrong result with unknown system id");
            Assert.False(identificationService.IsSystemOnline("TRAIN-1"), "IsSystemOnline return the wrong result with unknown system id");
            Assert.AreEqual(CommLinkEnum.notApplicable, identificationService.GetSystemLink("TRAIN-1"), "GetSystemLink return the wrong result with unknown system id");
            Assert.AreEqual(string.Empty, identificationService.GetUserName(2), "GetUserName return the wrong result with invalid session id");
            Assert.AreEqual(string.Empty, identificationService.GetNotificationUrl(2), "GetNotificationUrl return the wrong result with invalid session id");

            { // Test on login function - Success
                int effectiveProtocolVersion=0;
                string notificationUrl = "http://localhost";
                Assert.AreEqual(0, identificationService.login("admind", "admin", notificationUrl, 3, out effectiveProtocolVersion, "aaa"), "identification login return the wrong session id when user name is invalid");
                Assert.AreEqual(-1, effectiveProtocolVersion, "identification login return the wrong effective protocol id when user name is invalid");

                int sessionId = identificationService.login("admin", "admin", notificationUrl, 3, out effectiveProtocolVersion, "aaa");
                Assert.AreNotEqual(0, sessionId, "identification login return the wrong session id when user name is valid");
                Assert.AreEqual(3, effectiveProtocolVersion, "identification login return the wrong effective protocol id when user name is valid");

                Assert.True(identificationService.IsSessionValid(sessionId), "Session is supposed to be valid");
                Assert.AreEqual(notificationUrl, identificationService.GetNotificationUrl(sessionId), "GetNotificationUrl return the wrong result");
                Assert.AreEqual("admin", identificationService.GetUserName(sessionId), "GetUserName return the wrong result");
            }

            {   // Test system update

                identificationService.UpdateSystem("TRAIN-1", 1, true, 0, "mission", CommLinkEnum.wifi, "127.0.0.1");
                Assert.IsTrue(identificationService.IsSystemExist("TRAIN-1"), "IsSystemExist return the wrong result");
                Assert.IsTrue(identificationService.IsSystemOnline("TRAIN-1"), "IsSystemOnline return the wrong result");
                Assert.AreEqual(CommLinkEnum.wifi, identificationService.GetSystemLink("TRAIN-1"), "GetSystemLink return the wrong result with unknown system id");

                identificationService.UpdateSystem("TRAIN-2", 2, true, 0, "mission", CommLinkEnum._2G3G, "127.0.0.2");
                Assert.IsTrue(identificationService.IsSystemExist("TRAIN-1"), "IsSystemExist return the wrong result");
                Assert.IsTrue(identificationService.IsSystemOnline("TRAIN-1"), "IsSystemOnline return the wrong result");
                Assert.AreEqual(CommLinkEnum.wifi, identificationService.GetSystemLink("TRAIN-1"), "GetSystemLink return the wrong result with unknown system id");
                Assert.IsTrue(identificationService.IsSystemExist("TRAIN-2"), "IsSystemExist return the wrong result");
                Assert.IsTrue(identificationService.IsSystemOnline("TRAIN-2"), "IsSystemOnline return the wrong result");
                Assert.AreEqual(CommLinkEnum._2G3G, identificationService.GetSystemLink("TRAIN-2"), "GetSystemLink return the wrong result with unknown system id");

                identificationService.UpdateSystem("TRAIN-1", 1, false, 0, "mission", CommLinkEnum.wifi, "128.0.0.1");
                Assert.IsTrue(identificationService.IsSystemExist("TRAIN-1"), "IsSystemExist return the wrong result");
                Assert.IsFalse(identificationService.IsSystemOnline("TRAIN-1"), "IsSystemOnline return the wrong result");
                Assert.AreEqual(CommLinkEnum.notApplicable, identificationService.GetSystemLink("TRAIN-1"), "GetSystemLink return the wrong result with unknown system id");
            }
        }

        /// <summary>
        /// Perform sanity check on file transfer service
        /// </summary>
        [Test, Category("SanityCheck")]
        public void VerifyFileTransferServiceStub()
        {
            T2GIdentificationServiceStub identificationService = new T2GIdentificationServiceStub();
            int effectiveProtocolVersion = 0;
            int sessionId = identificationService.login("admin", "admin", string.Empty, 100, out effectiveProtocolVersion, "Test");
            Assert.AreNotEqual(0, sessionId, "login function return the wrong result");
            identificationService.UpdateSystem("TRAIN-1", 1, true, 0, string.Empty, CommLinkEnum.wifi, "127.0.0.1");

            T2GFileTransferServiceStub fileTransferService = new T2GFileTransferServiceStub(identificationService);

            int folderId = fileTransferService.CreateUploadFolder(sessionId, "This is a test", DateTime.UtcNow.AddDays(1), false, new FilePathInfo("file1.txt", 30, 100), new FilePathInfo("file2.txt", 1000, 101));
            Assert.Greater(folderId, 0, "createUploadeFolder return a wrong folder id");
            Assert.AreEqual(folderId, fileTransferService.LastCreatedFolder, "createUploadFolder does not initialize LastCreateFolder property");

            FileList fileList;

            FolderInfoStruct folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(2, fileList.Count, "GetFolderInfo return the wrong file list");

            int taskId = fileTransferService.CreateTransferTask(sessionId, "One transfer", T2GServiceInterface.FileTransfer.transferTypeEnum.groundToTrain, "ground", folderId, DateTime.UtcNow, TransferTaskInfo.NullDate, "TRAIN-1", "bbb,ccc");
            Assert.Greater(taskId, 0, "CreateTransferTask return an invalid task identifier");
            Assert.AreEqual(taskId, fileTransferService.LastCreatedTransfer, "CreateTransferTask does not initialize LastCreatedTransfer property");

            TransferTaskStruct task = fileTransferService.GetTransferTask(sessionId, taskId);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskCreated, task.taskState, "CreateTransferTask does not initialize properly the taskState");
            Assert.AreEqual(TaskPhaseEnum.creationPhase, task.taskPhase, "CreateTransferTask does not initialize properly the taskPhase");

            // Start the transfer
            fileTransferService.startTransfer(sessionId, taskId, (sbyte)10, T2GServiceInterface.FileTransfer.linkTypeEnum.anyBandwidth, false, false);
            task = fileTransferService.GetTransferTask(sessionId, taskId);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "startTransfer does not initialize properly the taskState");
            Assert.AreEqual(TaskPhaseEnum.acquisitionPhase, task.taskPhase, "startTransfer does not initialize properly the taskPhase");
            Assert.AreEqual(TaskSubStateEnum.subtaskInProgress, task.taskSubState, "startTransfer does not initialize properly the taskSubState");

            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionStarted, folderInfo.acquisitionState, "startTransfer does not initialize properly the acquisition state of the folder");
            Assert.AreEqual(0, folderInfo.currentFilesCount, "folder current file count differ than one expected");

            // Perform a progression. Expect that one file was acquired.
            fileTransferService.PerformTransferProgression();

            RecipientStruct recipient;
            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.acquisitionPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskInProgress, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.notTransferring, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(0, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(0, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionStarted, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(1, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // Perform a progression. Expect that acquisition was completed.
            fileTransferService.PerformTransferProgression();

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.transferPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskWaitingSchedule, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.waitingInQueue, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(0, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(0, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // System become offline
            identificationService.UpdateSystem("TRAIN-1", 1, false, 0, string.Empty, CommLinkEnum.wifi, "127.0.0.1");
            fileTransferService.PerformTransferProgression();

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.transferPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskWaitingComm, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.waitingForConnection, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(0, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(0, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // System become online
            identificationService.UpdateSystem("TRAIN-1", 1, true, 0, string.Empty, CommLinkEnum.wifi, "127.0.0.1");
            fileTransferService.PerformTransferProgression();

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.transferPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskInProgress, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.transferring, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(1, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(0, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // Update the progression
            fileTransferService.PerformTransferProgression();

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.transferPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskInProgress, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.transferring, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(1, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(1, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // Update the progression  - Expect  to complete the transfer phase
            fileTransferService.PerformTransferProgression();

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.distributionPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskInProgress, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.transferCompleted, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(0, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(1, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(2, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.transferCompletionPercent, "Transfert completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // Update the progression  - Expect  that whole transfer is completed/
            fileTransferService.PerformTransferProgression();

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskCompleted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.distributionPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskNone, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.transferCompleted, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(0, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(1, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(2, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.transferCompletionPercent, "Transfer completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // Cancel the transfer task
            fileTransferService.cancelTransfer(sessionId, taskId);

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskCancelled, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.distributionPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskNone, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.transferCompleted, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(0, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(1, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(2, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.transferCompletionPercent, "Transfer completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");
        }

        #endregion

        #region Test - Baseline distribution nominal scenario

        /// <summary>
        /// Distributes the baseline scenario nominal.
        /// </summary>
        [Test]
        public void DistributeBaselineScenario_Nominal()
        {
            const string FUTURE_VERSION = "1.0.0.0";
            // Common initialization
            CreateT2GServicesStub();
            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();
            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1);
            InitializeDataPackageService();
            InitializePISGroundSession();
            WaitPisGroundIsConnectedWithT2G();
            WaitTrainOnlineWithPISGround(TRAIN_NAME_1, true);


            // Initializations specific to this test.
            ElementsDataStoreData data = new ElementsDataStoreData(TRAIN_NAME_1);

            data.FutureBaseline = FUTURE_VERSION;
            data.FutureBaselineActivationDate = RemoteDataStoreDataBase.ToString(DateTime.Today);
            data.FutureBaselineExpirationDate = RemoteDataStoreDataBase.ToString(DateTime.Today.AddYears(1));

            _dataStoreServiceStub.UpdateDataStore(data);
            _dataStoreServiceStub.AddBaselineToRemoteDataStore(FUTURE_VERSION);

            // Request the datapackage service to distribute the baseline
            DataPackageResult result = _datapackageServiceStub.distributeBaseline(_pisGroundSessionId, null, new TargetAddressType(TRAIN_NAME_1), CreateDistributionAttribute(), false);
            Assert.AreEqual(DataPackageErrorEnum.REQUEST_ACCEPTED, result.error_code, "Distribute baseline to train {0} does not returned the expected value", TRAIN_NAME_1);

            // Wait that folder on T2G was created

            Assert.That(() => _fileTransferServiceStub.LastCreatedFolder.HasValue, Is.True.After(30 * ONE_SECOND, ONE_SECOND / 4), "Distribute baseline to train {0} failure. Transfer folder on T2G service not created", TRAIN_NAME_1);
            int transferFolderId = _fileTransferServiceStub.LastCreatedFolder.Value;
            _fileTransferServiceStub.LastCreatedFolder = null;
            Assert.That(() => _fileTransferServiceStub.LastCreatedTransfer.HasValue, Is.True.After(30 * ONE_SECOND, ONE_SECOND / 4), "Distribute baseline to train {0} failure. Transfer task on T2G service not created", TRAIN_NAME_1);
            int transferTaskId = _fileTransferServiceStub.LastCreatedTransfer.Value;
            _fileTransferServiceStub.LastCreatedTransfer = null;

            _fileTransferServiceStub.PerformTransferProgression();

            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, BASELINE_STATUS_UNKNOWN + "ddd", result.reqId, transferTaskId, BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS);
            for (int i = 0; i < (5 + 5 + 2); ++i)
            {
                _fileTransferServiceStub.PerformTransferProgression();
                Thread.Sleep(ONE_SECOND / 4);
            }

            Thread.Sleep(5 * ONE_SECOND);
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, BASELINE_STATUS_UNKNOWN, result.reqId, transferTaskId, BaselineProgressStatusEnum.TRANSFER_COMPLETED);


            // Simulate that train retrieved the baseline on embedded side.

            BaselineMessage baselineInfo = new BaselineMessage(TRAIN_NAME_1);
            baselineInfo.CurrentVersion = DEFAULT_BASELINE;
            baselineInfo.FutureVersion = FUTURE_VERSION;
            _vehicleInfoServiceStub.UpdateMessageData(baselineInfo);

            WaitBaselineStatusBecomeInState(TRAIN_NAME_1, BaselineProgressStatusEnum.DEPLOYED);
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, BaselineProgressStatusEnum.DEPLOYED);

            // Simulate that train replaced the current baseline with the future.
            baselineInfo.ArchivedVersion = baselineInfo.CurrentVersion;
            baselineInfo.CurrentVersion = baselineInfo.FutureVersion;
            baselineInfo.FutureVersion = string.Empty;
            _vehicleInfoServiceStub.UpdateMessageData(baselineInfo);

            WaitBaselineStatusBecomeInState(TRAIN_NAME_1, BaselineProgressStatusEnum.UPDATED);
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, FUTURE_VERSION, "0.0.0.0", result.reqId, transferTaskId, BaselineProgressStatusEnum.UPDATED);
        }

        #endregion

        #region Utilities methods

        /// <summary>
        /// Creates the T2G services stub.
        /// </summary>
        private void CreateT2GServicesStub()
        {
            _identificationServiceStub = new T2GIdentificationServiceStub();
            _fileTransferServiceStub = new T2GFileTransferServiceStub(_identificationServiceStub);
            _vehicleInfoServiceStub = new T2GVehicleInfoServiceStub(_identificationServiceStub);
            _notificationServiceStub = new T2GNotificationServiceStub();

            Uri identificationAddress = new Uri(IdentificationServiceUrl);
            Uri fileTransferAddress = new Uri(FileTransferServiceUrl);
            Uri vehicleInfoAddress = new Uri(VehicleInfoServiceUrl);
            Uri notificationAddress = new Uri(ServiceConfiguration.T2GServiceNotificationUrl);

            _hostIdentificationService = new ServiceHost(_identificationServiceStub, identificationAddress);
            _hostFileTransferService = new ServiceHost(_fileTransferServiceStub, fileTransferAddress);
            _hostVehicleInfoService = new ServiceHost(_vehicleInfoServiceStub, vehicleInfoAddress);
            _hostNotificationService = new ServiceHost(_notificationServiceStub, notificationAddress);

            _hostIdentificationService.Open();
            _hostFileTransferService.Open();
            _hostVehicleInfoService.Open();
            _hostNotificationService.Open();
        }

        #region Initialization methods

        /// <summary>
        /// Initializes the data package service.
        /// </summary>
        private void InitializeDataPackageService()
        {
            Assert.IsTrue(HistoryLoggerConfiguration.Used, "The test application is misconfigured. HistoryLoggerConfiguration.Used is not set to proper value");
            Assert.IsTrue(HistoryLoggerConfiguration.Valid, "The test application is misconfigured. HistoryLoggerConfiguration.Valid is not set to proper value");

            // Create a complete T2G Manager
            _t2gManager = T2GManagerContainer.T2GManager;
            _sessionManager = new SessionManager();

            Assert.IsEmpty(_sessionManager.RemoveAllSessions(), "Cannot empty the session database");
            _requestFactory = new RequestContextFactory();
            _requestManager = new RequestManager();

            _datapackageServiceStub = new DataPackageServiceStub(_sessionManager,
                _notificationSenderMock.Object, 
                _t2gManager, 
                _requestFactory,
                _remoteDataStoreFactoryMock.Object,
                _requestManager
                );

            _dataStoreServiceStub = new RemoteDataStoreServiceStub();
            _remoteDataStoreFactoryMock.Setup(f => f.GetRemoteDataStoreInstance()).Returns(_dataStoreServiceStub.Interface);

        }

        /// <summary>
        /// Establish a session with PIS-Ground.
        /// </summary>
        private void InitializePISGroundSession()
        {
            Assert.IsEmpty(_sessionManager.Login("admin", "admin", out _pisGroundSessionId), "Cannot create a session with PIS-Ground");
            Assert.IsEmpty(_sessionManager.SetNotificationURL(_pisGroundSessionId, PisGroundNotificationServiceUrl), "Cannot associate the notification url to PIS-Ground session");
        }

        private void InitializeTrain(string trainId, int vehicleId, bool isOnline, string ipAddress, ushort dataPackagePort)
        {
            _identificationServiceStub.UpdateSystem(trainId, vehicleId, isOnline, 0, DEFAULT_MISSION, DEFAULT_COMMUNICATION_LINK, ipAddress);
            _vehicleInfoServiceStub.UpdateMessageData(new VersionMessage(trainId, DEFAULT_PIS_VERSION));
            BaselineMessage baseline = new BaselineMessage(trainId);
            baseline.CurrentVersion = DEFAULT_BASELINE;
            _vehicleInfoServiceStub.UpdateMessageData(baseline);

            MissionMessage mission = new MissionMessage(trainId, DEFAULT_MISSION, (string.IsNullOrEmpty(DEFAULT_MISSION)) ? "NI" : "MI", DEFAULT_OPERATOR_CODE);
            _vehicleInfoServiceStub.UpdateMessageData(mission);

            ServiceInfoData datapackageService = new  ServiceInfoData((ushort)eServiceID.eSrvSIF_DataPackageServer, SERVICE_NAME_DATA_PACKAGE, isOnline, ipAddress, dataPackagePort, (ushort) vehicleId, DEFAULT_CAR_ID);
            _vehicleInfoServiceStub.UpdateServiceData(trainId, datapackageService);

            _dataStoreServiceStub.UpdateDataStore(new ElementsDataStoreData(trainId));

            if (_trainDataPackageServiceStub != null)
            {
                throw new NotImplementedException("Support to multiple train need to be implemented if needed.");
            }

            _trainDataPackageServiceStub = new TrainDataPackageServiceStub(trainId);
            Uri address = new Uri("http://" + ipAddress + ":" + dataPackagePort);
            _hostTrainDataPackageService = new ServiceHost(_trainDataPackageServiceStub, address);
            _hostTrainDataPackageService.Open();
        }


        /// <summary>
        /// Drops the test database and delete the physical files.
        /// </summary>
        private void DropTestDb()
        {
            if (_databaseConfigurationValid)
            {
                SqlConnection.ClearAllPools();

                if (File.Exists(_databaseFilePath))
                {
                    string cmdDropDB =
                        "IF EXISTS( select name from sys.databases where NAME= '" + _databaseName + "')" +
                        " BEGIN DROP DATABASE [" + _databaseName + "] END";

                    SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlCreateDbConnectionString, System.Data.CommandType.Text, cmdDropDB);
                    File.Delete(_databaseFilePath);
                }

                if (File.Exists(_databaseLogPath))
                {
                    File.Delete(_databaseLogPath);
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the baseline progress in history log for a train.
        /// </summary>
        /// <param name="trainName">Name of the train to query.</param>
        /// <returns>The baseline progress status. UNKNOWN if train name has no progress information</returns>
        private BaselineProgressStatusEnum GetBaselineProgress(string trainName)
        {
            return GetBaselineProgress(trainName, BaselineProgressStatusEnum.UNKNOWN);
        }

        /// <summary>
        /// Gets the baseline progress in history log for a train.
        /// </summary>
        /// <param name="trainName">Name of the train to query.</param>
        /// <param name="defaultValue">The value to return if train is unknown in history log database.</param>
        /// <returns>The baseline progress status. defaultValue if train name has no progress information</returns>
        private BaselineProgressStatusEnum GetBaselineProgress(string trainName, BaselineProgressStatusEnum defaultValue)
        {
            Dictionary<string, TrainBaselineStatusData> statuses;

            HistoryLogger.GetTrainBaselineStatus(out statuses);

            TrainBaselineStatusData trainStatus;
            return statuses.TryGetValue(trainName, out trainStatus) ? trainStatus.ProgressStatus : defaultValue;

        }

        private void VerifyTrainBaselineStatusInHistoryLog(string systemId, bool expectedOnlineStatus, string expectedBaselineVersion, string expectedFutureVersion, Guid expectedRequestId, int expectedTaskId, BaselineProgressStatusEnum expectedProgress)
        {
            Dictionary<string, TrainBaselineStatusData> statuses;

            HistoryLogger.GetTrainBaselineStatus(out statuses);

            TrainBaselineStatusData analysedTrain;
            Assert.IsTrue(statuses.TryGetValue(systemId, out analysedTrain), "History log database integrity error for train '{0}': no data found", systemId);
            Assert.AreEqual(expectedOnlineStatus, analysedTrain.OnlineStatus, "History log database integrity error for train '{0}': online status differ than expected", systemId);
            Assert.AreEqual(expectedRequestId, analysedTrain.RequestId, "History log database integrity error for train '{0}': request id differ than expected", systemId);
//            Assert.AreEqual(expectedTaskId, analysedTrain.TaskId, "History log database integrity error for train '{0}': task id differ than expected", systemId);
//            Assert.AreEqual(expectedProgress, analysedTrain.ProgressStatus, "History log database integrity error for train '{0}': progress status differ than expected", systemId);
            Assert.AreEqual(expectedBaselineVersion, analysedTrain.CurrentBaselineVersion, "History log database integrity error for train '{0}': current baseline version differ than expected", systemId);
            Assert.AreEqual(expectedFutureVersion, analysedTrain.FutureBaselineVersion, "History log database integrity error for train '{0}': future baseline version differ than expected", systemId);
        }

        private void WaitPisGroundIsConnectedWithT2G()
        {
            Assert.That(() => _t2gManager.T2GServerConnectionStatus, Is.True.After(5 * ONE_SECOND, ONE_SECOND / 4), "Pis-Ground cannot establish connection with T2G");
        }
        private void WaitTrainOnlineWithPISGround(string trainName, bool waitForDataPackage)
        {
            WaitTrainOnlineWithPISGround(trainName, waitForDataPackage, 3 *ONE_SECOND);
        }

        private void WaitBaselineStatusBecomeInState(string trainName, BaselineProgressStatusEnum expectedStatus)
        {
            WaitBaselineStatusBecomeInState(trainName, expectedStatus, 10 * ONE_SECOND);
        }

        private void WaitBaselineStatusBecomeInState(string trainName, BaselineProgressStatusEnum expectedStatus, int delay)
        {
            Assert.That(() => GetBaselineProgress(trainName), Is.EqualTo(expectedStatus).After(delay, ONE_SECOND / 3), "The baseline deployment status in history log database is no set to expected value for train '{0}'", trainName);
        }

        private void WaitTrainOnlineWithPISGround(string trainName, bool waitForDataPackage, int delay)
        {
            bool isOnline;

            Assert.That(() => _t2gManager.IsElementOnline(trainName, out isOnline) == T2GManagerErrorEnum.eSuccess && isOnline == true, Is.True.After(delay, ONE_SECOND / 5), "The train '{0}' is not online with PIS-Ground as expected", trainName);

            if (waitForDataPackage)
            {
                
                PIS.Ground.Core.Data.ServiceInfo serviceInfo;
                Assert.That(_t2gManager.GetAvailableServiceData(trainName, (int)eServiceID.eSrvSIF_DataPackageServer, out serviceInfo) == T2GManagerErrorEnum.eSuccess, Is.True.After(delay, ONE_SECOND / 5), "The service DataPackageService is not available with PIS-Ground as expected on train '{0}'", trainName);
            }

        }

        private static BaselineDistributionAttributes CreateDistributionAttribute()
        {
            BaselineDistributionAttributes transferAttributes = new BaselineDistributionAttributes();
            transferAttributes.TransferMode = FileTransferMode.AnyBandwidth;
            transferAttributes.priority = (sbyte)10;
            transferAttributes.fileCompression = false;
            transferAttributes.transferDate = DateTime.Now;
            transferAttributes.transferExpirationDate = DateTime.Now.AddDays(1);
            return transferAttributes;
        }

        #region RemoteDataStore


        #endregion

        #endregion
    }
}
