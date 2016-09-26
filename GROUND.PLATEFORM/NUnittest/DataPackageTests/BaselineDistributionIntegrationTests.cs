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
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Threading;
using DataPackageTests.Data;
using DataPackageTests.ServicesStub;
using DataPackageTests.Stubs;
using Moq;
using NUnit.Framework;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.SqlServerAccess;
using PIS.Ground.Core.T2G;
using PIS.Ground.Core.Utility;
using PIS.Ground.DataPackage;
using PIS.Ground.DataPackage.RemoteDataStoreFactory;
using PIS.Ground.DataPackage.RequestMgt;
using CommLinkEnum = DataPackageTests.T2GServiceInterface.Identification.commLinkEnum;
using TaskPhaseEnum = DataPackageTests.T2GServiceInterface.FileTransfer.taskPhaseEnum;
using TaskStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.taskStateEnum;
using TaskSubStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.taskSubStateEnum;


namespace DataPackageTests
{
    /// <summary>
    /// Class that test baseline distribution scenarios from distribute baseline until completion on embedded system.
    /// 
    /// This class simulate T2G, services on train to perform a complete validation of expected status of every stages of a baseline distribution.
    /// This class also validate the history log database and the notifications send by PIS-Ground.
    /// </summary>
    [TestFixture, Category("DistributionScenario"), Timeout(5 * 60 * 1000)]
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
        private NotificationSenderStub _notificationsSender;
        private Mock<IRemoteDataStoreFactory> _remoteDataStoreFactoryMock;
        private SessionManager _sessionManager;
        private RequestContextFactory _requestFactory;
        private RequestManager _requestManager;
        private Guid _pisGroundSessionId;

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

            Console.Out.WriteLine("===================================");
            Console.Out.WriteLine("BEGIN TEST {0}", currentContext.Test.Name);
            Console.Out.WriteLine("===================================");

            _notificationsSender = new NotificationSenderStub(true);
            _remoteDataStoreFactoryMock = new Mock<IRemoteDataStoreFactory>();
            _dataStoreServiceStub = new RemoteDataStoreServiceStub();
            _remoteDataStoreFactoryMock.Setup(f => f.GetRemoteDataStoreInstance()).Returns(_dataStoreServiceStub.Interface);
            HistoryLogger.EmptyDatabase();
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
                _requestManager.Dispose();
                _requestManager = null;
            }

            foreach (ServiceHost service in new ServiceHost[] { _hostVehicleInfoService, _hostFileTransferService, _hostIdentificationService, _hostNotificationService, _hostTrainDataPackageService })
            {
                if (service == null)
                    continue;

                if (service.State == CommunicationState.Faulted)
                {
                    service.Abort();
                }

                service.Close();
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
            _trainDataPackageServiceStub = null;
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

            Console.Out.WriteLine("===================================");
            Console.Out.WriteLine("END TEST {0}", currentContext.Test.Name);
            Console.Out.WriteLine("===================================");
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
            InitializeDataPackageService(false);
            InitializePISGroundSession();
            WaitPisGroundIsConnectedWithT2G();
            WaitTrainOnlineWithPISGround(TRAIN_NAME_1, true);

            // Initializations specific to this test.
            ElementsDataStoreData data = new ElementsDataStoreData(TRAIN_NAME_1);

            data.FutureBaseline = FUTURE_VERSION;
            data.FutureBaselineActivationDate = RemoteDataStoreDataBase.ToString(DateTime.Today);
            data.FutureBaselineExpirationDate = RemoteDataStoreDataBase.ToString(DateTime.Now.AddMinutes(20));

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

            
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, BaselineProgressStatusEnum.TRANSFER_PLANNED);
            while (_fileTransferServiceStub.IsTaskRunning(transferTaskId))
            {
                _fileTransferServiceStub.PerformTransferProgression();
                BaselineProgressStatusEnum expectedProgress = _fileTransferServiceStub.GetTask(transferTaskId).BaselineProgress;
                VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, expectedProgress);
                Thread.Sleep(ONE_SECOND / 4);
            }

            Thread.Sleep(5 * ONE_SECOND);
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, BaselineProgressStatusEnum.TRANSFER_COMPLETED);


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

        /// <summary>
        /// Test a distribute baseline scenario that cause the transfer to wait for a communication link during 2 minutes and then complete.
        /// </summary>
        [Test]
        public void DistributeBaselineScenario_WaitForCommunicationLinkDuringTwoMinutes()
        {
            const string FUTURE_VERSION = "1.0.0.0";
            // Common initialization
            CreateT2GServicesStub();
            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();
            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1, CommLinkEnum.notApplicable);
            InitializeDataPackageService(false);
            InitializePISGroundSession();
            WaitPisGroundIsConnectedWithT2G();
            WaitTrainOnlineWithPISGround(TRAIN_NAME_1, true);

            // Initializations specific to this test.
            ElementsDataStoreData data = new ElementsDataStoreData(TRAIN_NAME_1);

            data.FutureBaseline = FUTURE_VERSION;
            data.FutureBaselineActivationDate = RemoteDataStoreDataBase.ToString(DateTime.Today);
            data.FutureBaselineExpirationDate = RemoteDataStoreDataBase.ToString(DateTime.Now.AddMinutes(20));

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

            while (_fileTransferServiceStub.GetTask(transferTaskId).taskPhase == T2GServiceInterface.FileTransfer.taskPhaseEnum.acquisitionPhase)
            {
                VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, BaselineProgressStatusEnum.TRANSFER_PLANNED);
                _fileTransferServiceStub.PerformTransferProgression();
                AssertAll(
                () => Assert.IsNull(_fileTransferServiceStub.LastCreatedFolder, "Folder created while expecting not"),
                () => Assert.IsNull(_fileTransferServiceStub.LastCreatedTransfer, "Transfer task created while expecting not"),
                () => Assert.IsTrue(_fileTransferServiceStub.IsTaskRunning(transferTaskId), "The transfer task is not running as expected"));
                Thread.Sleep(ONE_SECOND / 4);
            }

            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, _fileTransferServiceStub.GetTask(transferTaskId).BaselineProgress);

            // Wait 2 minutes, check every seconds
            for (int t = 0; t < (2 * 60); ++t)
            {
                Thread.Sleep(ONE_SECOND);
                _fileTransferServiceStub.PerformTransferProgression();

                TransferTaskInfo task = _fileTransferServiceStub.GetTask(transferTaskId);
                AssertAll(
                 () => Assert.IsNull(_fileTransferServiceStub.LastCreatedFolder, "Folder created while expecting not"),
                 () => Assert.IsNull(_fileTransferServiceStub.LastCreatedTransfer, "Transfer task created while expecting not"),
                 () => Assert.IsTrue(task.IsRunning, "The transfer task is not running as expected"),
                 () => Assert.AreEqual(TaskPhaseEnum.transferPhase, task.taskPhase, "The transfer task phase is not the one expected"),
                 () => Assert.AreEqual(TaskSubStateEnum.subtaskWaitingLink, task.taskSubState, "The transfer task sub state is not the one expected"));
                VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, _fileTransferServiceStub.GetTask(transferTaskId).BaselineProgress);
            }


            // Update the communication link of the train
            _identificationServiceStub.UpdateSystem(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, 0, DEFAULT_MISSION, CommLinkEnum.wifi, TRAIN_IP_1);

            Assert.That(() => { _fileTransferServiceStub.PerformTransferProgression(); return _fileTransferServiceStub.GetTask(transferTaskId).IsInFinalState; }, Is.True.After(10 * ONE_SECOND, ONE_SECOND / 4), "Transfer does not complete as expected");

            Assert.IsNull(_fileTransferServiceStub.LastCreatedFolder, "Folder created while expecting not");
            Assert.IsNull(_fileTransferServiceStub.LastCreatedTransfer, "Transfer task created while expecting not");
            {
                TransferTaskInfo task = _fileTransferServiceStub.GetTask(transferTaskId);
                Assert.AreEqual(TaskStateEnum.taskCompleted, task.taskState, "Transfer does not complete as expected");
            }
            WaitBaselineStatusBecomeInState(TRAIN_NAME_1, BaselineProgressStatusEnum.TRANSFER_COMPLETED);
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, BaselineProgressStatusEnum.TRANSFER_COMPLETED);

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


        /// <summary>
        /// Test a distribute baseline scenario that cause the transfer to wait for a communication link, then datapackage service is restarted and then transfer complete.
        /// </summary>
        [Test, Category("Restart")]
        public void DistributeBaselineScenario_WaitForCommunicationLink_RestartPisGround_ThenDistributionComplete()
        {
            const string FUTURE_VERSION = "1.0.0.0";
            // Common initialization
            CreateT2GServicesStub();
            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();
            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1, CommLinkEnum.notApplicable);
            InitializeDataPackageService(false);
            InitializePISGroundSession();
            WaitPisGroundIsConnectedWithT2G();
            WaitTrainOnlineWithPISGround(TRAIN_NAME_1, true);

            // Initializations specific to this test.
            ElementsDataStoreData data = new ElementsDataStoreData(TRAIN_NAME_1);

            data.FutureBaseline = FUTURE_VERSION;
            data.FutureBaselineActivationDate = RemoteDataStoreDataBase.ToString(DateTime.Today);
            data.FutureBaselineExpirationDate = RemoteDataStoreDataBase.ToString(DateTime.Now.AddMinutes(20));

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

            while (_fileTransferServiceStub.GetTask(transferTaskId).taskPhase == T2GServiceInterface.FileTransfer.taskPhaseEnum.acquisitionPhase)
            {
                VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, _fileTransferServiceStub.GetTask(transferTaskId).BaselineProgress);
                _fileTransferServiceStub.PerformTransferProgression();
                Assert.IsNull(_fileTransferServiceStub.LastCreatedFolder, "Folder created while expecting not");
                Assert.IsNull(_fileTransferServiceStub.LastCreatedTransfer, "Transfer task created while expecting not");
                Assert.IsTrue(_fileTransferServiceStub.IsTaskRunning(transferTaskId), "The transfer task is not running as expected");
                Thread.Sleep(ONE_SECOND / 4);
            }
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, _fileTransferServiceStub.GetTask(transferTaskId).BaselineProgress);

            // Stop data package servie.
            StopDataPackageService();

            // Wait 2 seconds
            Thread.Sleep(2 * ONE_SECOND);

            // Restart the datapackage service
            InitializeDataPackageService(true);

            WaitPisGroundIsConnectedWithT2G();
            WaitTrainOnlineWithPISGround(TRAIN_NAME_1, true);


            // Wait 2 seconds
            Thread.Sleep(2 * ONE_SECOND);
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, _fileTransferServiceStub.GetTask(transferTaskId).BaselineProgress);

            // Update the communication link of the train
            _identificationServiceStub.UpdateSystem(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, 0, DEFAULT_MISSION, CommLinkEnum.wifi, TRAIN_IP_1);

            _fileTransferServiceStub.PerformTransferProgression();
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, _fileTransferServiceStub.GetTask(transferTaskId).BaselineProgress);
            _fileTransferServiceStub.PerformTransferProgression();
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS);

            Assert.That(() => { _fileTransferServiceStub.PerformTransferProgression(); return _fileTransferServiceStub.GetTask(transferTaskId).IsInFinalState; }, Is.True.After(10 * ONE_SECOND, ONE_SECOND / 4), "Transfer does not complete as expected");
            Assert.IsNull(_fileTransferServiceStub.LastCreatedFolder, "Folder created while expecting not");
            Assert.IsNull(_fileTransferServiceStub.LastCreatedTransfer, "Transfer task created while expecting not");
            {
                TransferTaskInfo task = _fileTransferServiceStub.GetTask(transferTaskId);
                Assert.AreEqual(TaskStateEnum.taskCompleted, task.taskState, "Transfer does not complete as expected");
            }
            WaitBaselineStatusBecomeInState(TRAIN_NAME_1, BaselineProgressStatusEnum.TRANSFER_COMPLETED);
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, BaselineProgressStatusEnum.TRANSFER_COMPLETED);

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

        /// <summary>
        /// Test a distribute baseline scenario that cause the transfer to expire while performing the acquisition.
        /// </summary>
        [Test, Category("Restart"), Category("Error")]
        public void DistributeBaselineScenario_TransferTask_Expire_During_Acquisition()
        {
            const string FUTURE_VERSION = "1.0.0.0";
            // Common initialization
            CreateT2GServicesStub();
            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();
            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1, CommLinkEnum.notApplicable);
            InitializeDataPackageService(false);
            InitializePISGroundSession();
            WaitPisGroundIsConnectedWithT2G();
            WaitTrainOnlineWithPISGround(TRAIN_NAME_1, true);

            // Initializations specific to this test.
            ElementsDataStoreData data = new ElementsDataStoreData(TRAIN_NAME_1);

            data.FutureBaseline = FUTURE_VERSION;
            data.FutureBaselineActivationDate = RemoteDataStoreDataBase.ToString(DateTime.Today);
            data.FutureBaselineExpirationDate = RemoteDataStoreDataBase.ToString(DateTime.Now.AddSeconds(40));

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

            _fileTransferServiceStub.SetTransferExpiration(transferTaskId, DateTime.UtcNow.AddSeconds(10));

            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, BaselineProgressStatusEnum.TRANSFER_PLANNED);

            _fileTransferServiceStub.PerformTransferProgression();
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, BaselineProgressStatusEnum.TRANSFER_PLANNED);
            _fileTransferServiceStub.PerformTransferProgression();
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, BaselineProgressStatusEnum.TRANSFER_PLANNED);
            Assert.AreEqual(TaskPhaseEnum.acquisitionPhase, _fileTransferServiceStub.GetTask(transferTaskId).taskPhase, "The transfer task is supported to be in acquisition");
            Assert.IsTrue(_fileTransferServiceStub.IsTaskRunning(transferTaskId), "The transfer task is not running as expected");
            Assert.IsNull(_fileTransferServiceStub.LastCreatedFolder, "Folder created while expecting not");
            Assert.IsNull(_fileTransferServiceStub.LastCreatedTransfer, "Transfer task created while expecting not");

            Thread.Sleep(15 * ONE_SECOND);
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, BaselineProgressStatusEnum.TRANSFER_PLANNED);
            Assert.IsNull(_fileTransferServiceStub.LastCreatedFolder, "Folder created while expecting not");
            Assert.IsNull(_fileTransferServiceStub.LastCreatedTransfer, "Transfer task created while expecting not");
            Assert.AreEqual(TaskPhaseEnum.acquisitionPhase, _fileTransferServiceStub.GetTask(transferTaskId).taskPhase, "The transfer task is supported to be in acquisition");
            Assert.IsTrue(_fileTransferServiceStub.IsTaskRunning(transferTaskId), "The transfer task is not running as expected");
            _fileTransferServiceStub.PerformTransferProgression();
            Assert.AreEqual(TaskPhaseEnum.acquisitionPhase, _fileTransferServiceStub.GetTask(transferTaskId).taskPhase, "The transfer task is supported to be in acquisition");
            Assert.IsFalse(_fileTransferServiceStub.IsTaskRunning(transferTaskId), "The transfer task is running while expecting not");

            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, string.Empty, Guid.Empty, 0, BaselineProgressStatusEnum.UNKNOWN);
            Assert.IsNull(_fileTransferServiceStub.LastCreatedFolder, "Folder created while expecting not");
            Assert.IsNull(_fileTransferServiceStub.LastCreatedTransfer, "Transfer task created while expecting not");

            // Wait 5 seconds
            Thread.Sleep(5 * ONE_SECOND);
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, string.Empty, Guid.Empty, 0, BaselineProgressStatusEnum.UNKNOWN);

            Assert.IsNull(_fileTransferServiceStub.LastCreatedFolder, "Folder created while expecting not");
            Assert.IsNull(_fileTransferServiceStub.LastCreatedTransfer, "Transfer task created while expecting not");
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
        /// <param name="isRestart">Parameter that indicates if it is a restart simulation of the services.</param>
        private void InitializeDataPackageService(bool isRestart)
        {
            Assert.IsTrue(HistoryLoggerConfiguration.Used, "The test application is misconfigured. HistoryLoggerConfiguration.Used is not set to proper value");
            Assert.IsTrue(HistoryLoggerConfiguration.Valid, "The test application is misconfigured. HistoryLoggerConfiguration.Valid is not set to proper value");

            // Create a complete T2G Manager
            _t2gManager = T2GManagerContainer.T2GManager;
            _sessionManager = new SessionManager();

            if (!isRestart)
            {
                Assert.IsEmpty(_sessionManager.RemoveAllSessions(), "Cannot empty the session database");
            }

            _requestFactory = new RequestContextFactory();
            _requestManager = new RequestManager();

            _datapackageServiceStub = new DataPackageServiceStub(_sessionManager,
                _notificationsSender, 
                _t2gManager, 
                _requestFactory,
                _remoteDataStoreFactoryMock.Object,
                _requestManager
                );
        }

        /// <summary>
        /// Stops the data package service.
        /// </summary>
        private void StopDataPackageService()
        {
            if (_datapackageServiceStub != null)
            {
                _datapackageServiceStub.Dispose();
                _datapackageServiceStub = null;
            }

            if (_requestManager != null)
            {
                _requestManager.Dispose();
                _requestManager = null;
            }

            DataPackageService.Uninitialize();
            T2GManagerContainer.T2GManager = null;

            if (_t2gManager != null)
            {
                _t2gManager.Dispose();
                _t2gManager = null;
            }

            _sessionManager = null;
            _requestFactory = null;
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
            InitializeTrain(trainId, vehicleId, isOnline, ipAddress, dataPackagePort, DEFAULT_COMMUNICATION_LINK);
        }

        private void InitializeTrain(string trainId, int vehicleId, bool isOnline, string ipAddress, ushort dataPackagePort, CommLinkEnum communicationLink)
        {
            _identificationServiceStub.UpdateSystem(trainId, vehicleId, isOnline, 0, DEFAULT_MISSION, communicationLink, ipAddress);
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
            AssertAll(
            () => Assert.AreEqual(expectedOnlineStatus, analysedTrain.OnlineStatus, "History log database integrity error for train '{0}': online status differ than expected", systemId),
            () => Assert.AreEqual(expectedRequestId, analysedTrain.RequestId, "History log database integrity error for train '{0}': request id differ than expected", systemId),
            () => Assert.AreEqual(expectedTaskId, analysedTrain.TaskId, "History log database integrity error for train '{0}': task id differ than expected", systemId),
            () => Assert.AreEqual(expectedProgress, analysedTrain.ProgressStatus, "History log database integrity error for train '{0}': progress status differ than expected", systemId),
            () => Assert.AreEqual(expectedBaselineVersion, analysedTrain.CurrentBaselineVersion, "History log database integrity error for train '{0}': current baseline version differ than expected", systemId),
            () => Assert.AreEqual(expectedFutureVersion, analysedTrain.FutureBaselineVersion, "History log database integrity error for train '{0}': future baseline version differ than expected", systemId));
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

        /// <summary>
        /// Utility function that allow to executed multiple asserts.
        /// </summary>
        /// <param name="assertionsToRun">The assertions to run.</param>
        private static void AssertAll(params Action[] assertionsToRun)
        {
            StringBuilder errorsMessageString = null;
            foreach (var action in assertionsToRun)
            {
                try
                {
                    action.Invoke();
                }
                catch (AssertionException exception)
                {
                    if (errorsMessageString == null)
                    {
                        errorsMessageString = new StringBuilder(exception.ToString());
                    }
                    else
                    {
                        errorsMessageString.AppendLine().AppendLine().Append(exception.ToString());
                    }
                }
            }

            if (errorsMessageString != null)
            {

                Assert.Fail(string.Format(CultureInfo.CurrentCulture, "The following condtions failed:{0}{1}",
                             Environment.NewLine, errorsMessageString.ToString()));
            }
        }
        #endregion
    }
}
