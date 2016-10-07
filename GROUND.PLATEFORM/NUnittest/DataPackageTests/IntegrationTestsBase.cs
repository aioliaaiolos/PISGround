//---------------------------------------------------------------------------------------------------
// <copyright file="IntegrationTestsBase.cs" company="Alstom">
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
using DataPackageTests.Data;
using DataPackageTests.ServicesStub;
using DataPackageTests.Stubs;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
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
using PIS.Ground.GroundCore.AppGround;
using CommLinkEnum = DataPackageTests.T2GServiceInterface.Identification.commLinkEnum;

namespace DataPackageTests
{
    /// <summary>
    /// Base class for integration tests.
    /// </summary>
    [Category("IntegrationTests")]
    class IntegrationTestsBase : AssertionHelper
    {
        #region Fields

        #region Public constants


        public const int ONE_SECOND = 1000;

        // Define the IP address to use. Change it if you would like to debug with a proxy server.
        public const string DEFAULT_IP = "127.0.0.1";

        public const string IdentificationServiceUrl = "http://" + DEFAULT_IP + ":5000/T2G/Identification.asmx";
        public const string FileTransferServiceUrl = "http://" + DEFAULT_IP + ":5000/T2G/FileTransfer.asmx";
        public const string VehicleInfoServiceUrl = "http://" + DEFAULT_IP + ":5000/T2G/VehicleInfo.asmx";
        public const string PisGroundNotificationServiceUrl = "http://" + DEFAULT_IP + ":5002/PIS_GROUND/notification.svc";

        public const string TRAIN_NAME_0 = "TRAIN-0";
        public const string TRAIN_NAME_1 = "TRAIN-1";
        public const string TRAIN_NAME_2 = "TRAIN-2";
        public const int TRAIN_VEHICLE_ID_0 = 0;
        public const int TRAIN_VEHICLE_ID_1 = 1;
        public const int TRAIN_VEHICLE_ID_2 = 2;
        public const string TRAIN_IP_0 = "127.0.0.250";
        public const string TRAIN_IP_1 = DEFAULT_IP;
        public const string TRAIN_IP_2 = "127.0.0.2";
        public const ushort TRAIN_DATA_PACKAGE_PORT_0 = 3999;
        public const ushort TRAIN_DATA_PACKAGE_PORT_1 = 4000;
        public const ushort TRAIN_DATA_PACKAGE_PORT_2 = 4001;

        public const string DEFAULT_BASELINE = "3.0.0.0";
        public const string DEFAULT_MISSION = "";
        public const CommLinkEnum DEFAULT_COMMUNICATION_LINK = CommLinkEnum.wifi;
        public const string DEFAULT_OPERATOR_CODE = "77";

        public const string DEFAULT_PIS_VERSION = "5.16.3.2";

        public const string SERVICE_NAME_DATA_PACKAGE = "PIS2G DataPackage";
        public const ushort DEFAULT_CAR_ID = 1;

        public const string BASELINE_STATUS_UNKNOWN = "UNKNOWN";

        #endregion

        #region Protected members

        protected T2GFileTransferServiceStub _fileTransferServiceStub;
        protected T2GIdentificationServiceStub _identificationServiceStub;
        protected T2GVehicleInfoServiceStub _vehicleInfoServiceStub;
        protected T2GNotificationServiceStub _notificationServiceStub;
        protected TrainDataPackageServiceStub _trainDataPackageServiceStub;
        protected DataPackageServiceStub _datapackageServiceStub;
        protected RemoteDataStoreServiceStub _dataStoreServiceStub;
        protected ServiceHost _hostIdentificationService;
        protected ServiceHost _hostFileTransferService;
        protected ServiceHost _hostVehicleInfoService;
        protected ServiceHost _hostNotificationService;
        protected ServiceHost _hostTrainDataPackageService;
        protected IT2GManager _t2gManager;

        private const string DatabaseName = "TestDatabaseDataPackage";
        protected string _databaseFilePath = string.Empty;
        protected string _databaseLogPath = string.Empty;
        protected string _databaseFolderPath = string.Empty;
        protected string _databaseName = string.Empty;
        protected bool _databaseConfigurationValid = false;

        /// <summary>The notification sender mock.</summary>
        protected NotificationSenderStub _notificationsSender;
        protected Mock<IRemoteDataStoreFactory> _remoteDataStoreFactoryMock;
        protected SessionManager _sessionManager;
        protected BaselineStatusUpdater _baselineStatusUpdater;
        protected RequestContextFactory _requestFactory;
        protected RequestManager _requestManager;
        protected Guid _pisGroundSessionId;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationTestsBase"/> class.
        /// </summary>
        public IntegrationTestsBase()
        {
            /* No logic body */
        }

        #endregion

        #region Test setup management

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

            if (_vehicleInfoServiceStub != null)
            {
                _vehicleInfoServiceStub.Dispose();
            }

            if (_baselineStatusUpdater != null)
            {
                _baselineStatusUpdater.Dispose();
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
            _baselineStatusUpdater = null;
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

        #region "Utilities functions"

        #region Initialization methods
        /// <summary>
        /// Creates the T2G services stub.
        /// </summary>
        protected void CreateT2GServicesStub()
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

        protected void ReopenIdentificationService()
        {
            if (_identificationServiceStub == null)
            {
                throw new InvalidOperationException("Identification service shall be created first");
            }

            if (_hostIdentificationService != null)
            {
                bool closed = true;
                try
                {
                    if (_hostIdentificationService.State != CommunicationState.Closed)
                    {
                        closed = false;
                        if (_hostIdentificationService.State != CommunicationState.Faulted)
                        {
                            _hostIdentificationService.Close();
                            closed = true;
                        }
                    }
                }
                finally
                {
                	if (!closed)
                    {
                        _hostIdentificationService.Abort();
                    }
                }
            }

            Uri identificationAddress = new Uri(IdentificationServiceUrl);
            _hostIdentificationService = new ServiceHost(_identificationServiceStub, identificationAddress);
            _hostIdentificationService.Open();
        }

        /// <summary>
        /// Initializes the data package service.
        /// </summary>
        /// <param name="isRestart">Parameter that indicates if it is a restart simulation of the services.</param>
        protected void InitializeDataPackageService(bool isRestart)
        {
            Assert.IsTrue(HistoryLoggerConfiguration.Used, "The test application is misconfigured. HistoryLoggerConfiguration.Used is not set to proper value");
            Assert.IsTrue(HistoryLoggerConfiguration.Valid, "The test application is misconfigured. HistoryLoggerConfiguration.Valid is not set to proper value");

            // Create a complete T2G Manager
            _t2gManager = T2GManagerContainer.T2GManager;
            _sessionManager = new SessionManager();
            _baselineStatusUpdater = new BaselineStatusUpdater();

            if (!isRestart)
            {
                Assert.IsEmpty(_sessionManager.RemoveAllSessions(), "Cannot empty the session database");
            }

            _requestFactory = new RequestContextFactory(_t2gManager, _remoteDataStoreFactoryMock.Object, _baselineStatusUpdater);
            _requestManager = new RequestManager();

            _datapackageServiceStub = new DataPackageServiceStub(_sessionManager,
                _notificationsSender,
                _t2gManager,
                _requestFactory,
                _remoteDataStoreFactoryMock.Object,
                _requestManager,
                _baselineStatusUpdater
                );
        }

        /// <summary>
        /// Stops the data package service.
        /// </summary>
        protected void StopDataPackageService()
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
        protected void InitializePISGroundSession()
        {
            Assert.IsEmpty(_sessionManager.Login("admin", "admin", out _pisGroundSessionId), "Cannot create a session with PIS-Ground");
            Assert.IsEmpty(_sessionManager.SetNotificationURL(_pisGroundSessionId, PisGroundNotificationServiceUrl), "Cannot associate the notification url to PIS-Ground session");
        }

        /// <summary>
        /// Drops the test database and delete the physical files.
        /// </summary>
        protected void DropTestDb()
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

        #region Train Management

        /// <summary>
        /// Add a know train in T2G and initialize the data-package embedded service.
        /// </summary>
        /// <param name="trainId">The train identifier.</param>
        /// <param name="vehicleId">The vehicle identifier.</param>
        /// <param name="isOnline">if set to <c>true</c> [is online].</param>
        /// <param name="ipAddress">The ip address.</param>
        /// <param name="dataPackagePort">The data package port.</param>
        protected void InitializeTrain(string trainId, int vehicleId, bool isOnline, string ipAddress, ushort dataPackagePort)
        {
            InitializeTrain(trainId, vehicleId, isOnline, ipAddress, dataPackagePort, DEFAULT_COMMUNICATION_LINK);
        }

        /// <summary>
        /// Add a know train in T2G and initialize the data-package embedded service.
        /// </summary>
        /// <param name="trainId">The train identifier.</param>
        /// <param name="vehicleId">The vehicle identifier.</param>
        /// <param name="isOnline">Indicates the initiale online status of the train.</param>
        /// <param name="ipAddress">The ip address of the train.</param>
        /// <param name="dataPackagePort">The data package port.</param>
        /// <param name="communicationLink">The communication link.</param>
        /// <exception cref="NotImplementedException">Support to multiple train need to be implemented if needed.</exception>
        protected void InitializeTrain(string trainId, int vehicleId, bool isOnline, string ipAddress, ushort dataPackagePort, CommLinkEnum communicationLink)
        {
            InitializeTrain(trainId, vehicleId, isOnline, ipAddress, dataPackagePort, communicationLink, true);
        }

        /// <summary>
        /// Add a know train in T2G and might initialize the data-package embedded service.
        /// </summary>
        /// <param name="trainId">The train identifier.</param>
        /// <param name="vehicleId">The vehicle identifier.</param>
        /// <param name="isOnline">Indicates the initiale online status of the train.</param>
        /// <param name="ipAddress">The ip address of the train.</param>
        /// <param name="dataPackagePort">The data package port.</param>
        /// <param name="communicationLink">The communication link.</param>
        /// <param name="initializeEmbeddedService">Indicates if the embedded data-package service shall be initialized.</param>
        /// <exception cref="NotImplementedException">Support to multiple train need to be implemented if needed.</exception>
        protected void InitializeTrain(string trainId, int vehicleId, bool isOnline, string ipAddress, ushort dataPackagePort, CommLinkEnum communicationLink, bool initializeEmbeddedService)
        {
            InitializeTrain(trainId, vehicleId, isOnline, ipAddress, dataPackagePort, communicationLink, initializeEmbeddedService, DEFAULT_BASELINE);
        }

        /// <summary>
        /// Add a know train in T2G and might initialize the data-package embedded service.
        /// </summary>
        /// <param name="trainId">The train identifier.</param>
        /// <param name="vehicleId">The vehicle identifier.</param>
        /// <param name="isOnline">Indicates the initiale online status of the train.</param>
        /// <param name="ipAddress">The ip address of the train.</param>
        /// <param name="dataPackagePort">The data package port.</param>
        /// <param name="communicationLink">The communication link.</param>
        /// <param name="initializeEmbeddedService">Indicates if the embedded data-package service shall be initialized.</param>
        /// <param name="currentBaseline">The current baseline version.</param>
        /// <exception cref="NotImplementedException">Support to multiple train need to be implemented if needed.</exception>
        protected void InitializeTrain(string trainId, int vehicleId, bool isOnline, string ipAddress, ushort dataPackagePort, CommLinkEnum communicationLink, bool initializeEmbeddedService, string currentBaseline)
        {
            _identificationServiceStub.UpdateSystem(trainId, vehicleId, isOnline, 0, DEFAULT_MISSION, communicationLink, ipAddress);
            _vehicleInfoServiceStub.UpdateMessageData(new VersionMessage(trainId, DEFAULT_PIS_VERSION));
            BaselineMessage baseline = new BaselineMessage(trainId);
            baseline.CurrentVersion = currentBaseline;
            _vehicleInfoServiceStub.UpdateMessageData(baseline);

            MissionMessage mission = new MissionMessage(trainId, DEFAULT_MISSION, (string.IsNullOrEmpty(DEFAULT_MISSION)) ? "NI" : "MI", DEFAULT_OPERATOR_CODE);
            _vehicleInfoServiceStub.UpdateMessageData(mission);

            ServiceInfoData datapackageService = new ServiceInfoData((ushort)eServiceID.eSrvSIF_DataPackageServer, SERVICE_NAME_DATA_PACKAGE, isOnline && initializeEmbeddedService, ipAddress, dataPackagePort, (ushort)vehicleId, DEFAULT_CAR_ID);
            _vehicleInfoServiceStub.UpdateServiceData(trainId, datapackageService);

            _dataStoreServiceStub.UpdateDataStore(new ElementsDataStoreData(trainId));

            if (initializeEmbeddedService)
            {
                if (_trainDataPackageServiceStub != null)
                {
                    throw new NotImplementedException("Support to multiple train need to be implemented if needed.");
                }

                _trainDataPackageServiceStub = new TrainDataPackageServiceStub(trainId);
                Uri address = new Uri("http://" + ipAddress + ":" + dataPackagePort);
                _hostTrainDataPackageService = new ServiceHost(_trainDataPackageServiceStub, address);
                _hostTrainDataPackageService.Open();
            }
        }

        #endregion

        #region HistoryLog Helpers

        /// <summary>
        /// Gets the baseline progress in history log for a train.
        /// </summary>
        /// <param name="trainName">Name of the train to query.</param>
        /// <returns>The baseline progress status. UNKNOWN if train name has no progress information</returns>
        protected BaselineProgressStatusEnum GetBaselineProgress(string trainName)
        {
            return GetBaselineProgress(trainName, BaselineProgressStatusEnum.UNKNOWN);
        }

        /// <summary>
        /// Gets the baseline progress in history log for a train.
        /// </summary>
        /// <param name="trainName">Name of the train to query.</param>
        /// <param name="defaultValue">The value to return if train is unknown in history log database.</param>
        /// <returns>The baseline progress status. defaultValue if train name has no progress information</returns>
        protected BaselineProgressStatusEnum GetBaselineProgress(string trainName, BaselineProgressStatusEnum defaultValue)
        {
            Dictionary<string, TrainBaselineStatusData> statuses;

            HistoryLogger.GetTrainBaselineStatus(out statuses);

            TrainBaselineStatusData trainStatus;
            return statuses.TryGetValue(trainName, out trainStatus) ? trainStatus.ProgressStatus : defaultValue;

        }

        protected void VerifyTrainBaselineStatusInHistoryLog(string systemId, bool expectedOnlineStatus, string expectedBaselineVersion, string expectedFutureVersion, Guid expectedRequestId, int expectedTaskId, BaselineProgressStatusEnum expectedProgress)
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

        protected void WaitBaselineStatusBecomeInState(string trainName, BaselineProgressStatusEnum expectedStatus)
        {
            WaitBaselineStatusBecomeInState(trainName, expectedStatus, 10 * ONE_SECOND);
        }

        protected void WaitBaselineStatusBecomeInState(string trainName, BaselineProgressStatusEnum expectedStatus, int delay)
        {
            Assert.That(() => GetBaselineProgress(trainName), Is.EqualTo(expectedStatus).After(delay, ONE_SECOND / 3), "The baseline deployment status in history log database is no set to expected value for train '{0}'", trainName);
        }

        /// <summary>
        /// Updates the history log database with information provided by data..
        /// </summary>
        /// <param name="data">The train baseline status information to write into database.</param>
        protected void UpdateHistoryLog(TrainBaselineStatusData data)
        {
            HistoryLogger.UpdateTrainBaselineStatus(data.TrainId, data.RequestId, data.TaskId, data.TrainNumber, data.OnlineStatus, data.ProgressStatus, data.CurrentBaselineVersion, data.FutureBaselineVersion, data.PisOnBoardVersion);
        }


        #endregion

        #region T2G Helpers

        protected void WaitPisGroundIsConnectedWithT2G()
        {
            WaitPisGroundIsConnectedWithT2G(false);
        }

        protected void WaitPisGroundIsConnectedWithT2G(bool isRestart)
        {
            int delay = (isRestart) ? 35 * ONE_SECOND : 5 * ONE_SECOND;
            int checkDelay = (isRestart) ? ONE_SECOND : ONE_SECOND / 4;
            Assert.That(() => _t2gManager.T2GServerConnectionStatus, Is.True.After(delay, checkDelay), "Pis-Ground cannot establish connection with T2G");
        }

        protected  void WaitTrainOnlineWithPISGround(string trainName, bool waitForDataPackage)
        {
            WaitTrainOnlineWithPISGround(trainName, waitForDataPackage, 3 * ONE_SECOND);
        }

        protected void WaitTrainOnlineWithPISGround(string trainName, bool waitForDataPackage, int delay)
        {
            bool isOnline;

            Assert.That(() => _t2gManager.IsElementOnline(trainName, out isOnline) == T2GManagerErrorEnum.eSuccess && isOnline == true, Is.True.After(delay, ONE_SECOND / 5), "The train '{0}' is not online with PIS-Ground as expected", trainName);

            if (waitForDataPackage)
            {

                PIS.Ground.Core.Data.ServiceInfo serviceInfo;
                Assert.That(_t2gManager.GetAvailableServiceData(trainName, (int)eServiceID.eSrvSIF_DataPackageServer, out serviceInfo) == T2GManagerErrorEnum.eSuccess, Is.True.After(delay, ONE_SECOND / 5), "The service DataPackageService is not available with PIS-Ground as expected on train '{0}'", trainName);
            }

        }

        protected void WaitTrainBaselineStatusesEquals(Dictionary<string, TrainBaselineStatusData> expectedStatuses, string message)
        {
            Dictionary<string, TrainBaselineStatusData> statuses=null;

            ActualValueDelegate comparer = () => { HistoryLogger.GetTrainBaselineStatus(out statuses); return AreDictionariesEquals(expectedStatuses, statuses); };

            AssertAll(() => Assert.That(comparer, Is.True.After(5 * ONE_SECOND, ONE_SECOND/2), "Train baseline status wasn't updated as expected"),
                    () => AssertDictionariesEqual(expectedStatuses, statuses, message));
        }


        #endregion

        #region DataPackage Helpers

        protected static BaselineDistributionAttributes CreateDistributionAttribute()
        {
            BaselineDistributionAttributes transferAttributes = new BaselineDistributionAttributes();
            transferAttributes.TransferMode = FileTransferMode.AnyBandwidth;
            transferAttributes.priority = (sbyte)10;
            transferAttributes.fileCompression = false;
            transferAttributes.transferDate = DateTime.Now;
            transferAttributes.transferExpirationDate = DateTime.Now.AddDays(1);
            return transferAttributes;
        }

        #endregion

        #region Testing
        /// <summary>
        /// Utility function that allow to executed multiple asserts.
        /// </summary>
        /// <param name="assertionsToRun">The assertions to run.</param>
        public static void AssertAll(params Action[] assertionsToRun)
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

        /// <summary>
        /// Verifies if two dictionary are equals.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="a">The first dictionary object to compare.</param>
        /// <param name="b">The second dictionary object to compare.</param>
        /// <returns>true if a is equals to b, otherwise false.</returns>
        public static bool AreDictionariesEquals<TKey, TValue>(IDictionary<TKey, TValue> a, IDictionary<TKey, TValue> b)
        {
            bool areEquals = (a == null ? 0 : a.Count) == (b == null ? 0 : b.Count);
            if (areEquals && a != null && b != null)
            {
                foreach (KeyValuePair<TKey, TValue> item in a)
                {
                    TValue actualValue;
                    if (!b.TryGetValue(item.Key, out actualValue) || !item.Value.Equals(actualValue))
                    {
                        areEquals = false;
                        break;
                    }
                }

            }

            return areEquals;
        }

        /// <summary>
        /// Asserts the dictionaries equal and generate detailed message if not.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">The message to add if mismatch detected.</param>
        public static void AssertDictionariesEqual<TKey, TValue>(IDictionary<TKey, TValue> expected, IDictionary<TKey, TValue> actual, string message)
        {

            if (!IntegrationTestsBase.AreDictionariesEquals(expected, actual))
            {
                StringBuilder formattedMessage = new StringBuilder(2048);
                if (expected.Count != actual.Count)
                {
                    formattedMessage.AppendFormat(CultureInfo.CurrentCulture, "  - The expected item count<{0}> does not match the actual item count<{1}>.\r\n", expected.Count, actual.Count);
                }

                DumpDictionary(formattedMessage, "ACTUAL", actual);
                DumpDictionary(formattedMessage, "EXPECTED", expected);
                if (!string.IsNullOrEmpty(message))
                {
                    Assert.Fail("{0}\r\nThe actual dictionary values does not match the expected dictionary values:\r\n{1}", message, formattedMessage.ToString());
                }
                else
                {
                    Assert.Fail("The actual dictionary values does not match the expected dictionary values:\r\n{1}", formattedMessage.ToString());
                }
            }
        }

        /// <summary>
        /// Dumps the dictionary into an output string.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="output">The output string buffer.</param>
        /// <param name="name">Name of the dictionary to dump</param>
        /// <param name="dictionary">The dictionary to dump.</param>
        public static void DumpDictionary<TKey, TValue>(StringBuilder output, string name, IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                output.Append(name).AppendLine(" = (null)");
            }
            else
            {
                int beforeLenght = output.Length;
                output.AppendFormat(CultureInfo.InvariantCulture, "  {0}[Count={1}] = {{", name, dictionary.Count);
                int afterLength = output.Length;
                if (dictionary.Count == 0)
                {
                    output.Append(" <empty> ");
                }
                else
                {
                    bool isFirst = true;

                    foreach(KeyValuePair<TKey, TValue> item in dictionary)
                    {
                        if (!isFirst)
                        {
                            output.Append(",").AppendLine().Append(' ', afterLength-beforeLenght);
                        }
                        else
                        {
                            isFirst = false;
                        }

                        output.AppendFormat(CultureInfo.InvariantCulture, "[{0}] = {1}", item.Key, item.Value);
                    }

                }

                output.AppendLine("}");

            }
        }


        #endregion

        #region NotificationSend

        /// <summary>
        /// Waits the notification send.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        protected void WaitNotificationSend(NotificationIdEnum notificationId)
        {
            WaitNotificationSend(notificationId, Guid.Empty);
        }

        /// <summary>
        /// Waits the notification send.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="parameter">The notification parameter</param>
        protected void WaitNotificationSend(NotificationIdEnum notificationId, string parameter)
        {
            WaitNotificationSend(notificationId, Guid.Empty, parameter);
        }

        /// <summary>
        /// Waits the notification send.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="requestId">The request identifier associated to the request.</param>
        protected void WaitNotificationSend(NotificationIdEnum notificationId, Guid requestId)
        {
            Assert.That(() => _notificationsSender.ContainsNotification(notificationId, requestId), Is.True.After(5 * ONE_SECOND, ONE_SECOND / 4), "Failed to received expected notification '{0}' for request '{1}'.", notificationId, requestId);
        }


        /// <summary>
        /// Waits the notification send.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="requestId">The request identifier associated to the request.</param>
        /// <param name="parameter">The request parameter of the notification</param>
        protected void WaitNotificationSend(NotificationIdEnum notificationId, Guid requestId, string parameter)
        {
            Assert.That(() => _notificationsSender.ContainsNotification(notificationId, requestId, parameter), Is.True.After(5 * ONE_SECOND, ONE_SECOND / 4), "Failed to received expected notification '{0}' for request '{1}' with parameter '{2}'.", notificationId, requestId, parameter);
        }

        #endregion

        #endregion

    }
}
