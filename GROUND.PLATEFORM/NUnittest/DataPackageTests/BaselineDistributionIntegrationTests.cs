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
using PIS.Ground.Core.SqlServerAccess;
using PIS.Ground.Core.T2G;
using PIS.Ground.Core.Utility;
using PIS.Ground.DataPackage;
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
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.DataPackage.RequestMgt;
using PIS.Ground.DataPackage.RemoteDataStoreFactory;


namespace DataPackageTests
{
    /// <summary>
    /// Class that test baseline distribution scenarios from distribute baseline until completion on embedded system.
    /// 
    /// This class simulate T2G, services on train to perform a complete validation of expected status of every stages of a baseline distribution.
    /// This class also validate the history log database and the notifications send by PIS-Ground.
    /// </summary>
    [TestFixture, Category("DistributionScenario")]
    class BaselineDistributionIntegrationTests
    {
        #region Fields

        public const string IdentificationServiceUrl = "http://127.0.0.1:5000/T2G/Identification.asmx";
        public const string FileTransferServiceUrl = "http://127.0.0.1:5000/T2G/FileTransfer.asmx";
        public const string VehicleInfoServiceUrl = "http://127.0.0.1:5000/T2G/VehicleInfo.asmx";

        private T2GFileTransferServiceStub _fileTransferServiceStub;
        private T2GIdentificationServiceStub _identificationServiceStub;
        private T2GVehicleInfoServiceStub _vehicleInfoServiceStub;
        private T2GNotificationServiceStub _notificationServiceStub;
        private DataPackageServiceStub _datapackageServiceStub;
        private ServiceHost _hostIdentificationService;
        private ServiceHost _hostFileTransferService;
        private ServiceHost _hostVehicleInfoService;
        private ServiceHost _hostNotificationService;
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
        private SessionManager _sessionManager;
        private RequestContextFactory _requestFactory;
        private RequestManager _requestManager;


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

            HistoryLogger.Initialize();
        }

        /// <summary>Test fixture cleanup.</summary>
        [TestFixtureTearDown]
        public void MyCleanup()
        {
            DropTestDb();
        }


        /// <summary>Setups called before each test to initialize variables.</summary>
        [SetUp]
        public void Setup()
        {
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

            foreach (ServiceHost service in new ServiceHost[] { _hostVehicleInfoService, _hostFileTransferService, _hostIdentificationService, _hostNotificationService })
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
            _fileTransferServiceStub = null;
            _identificationServiceStub = null;
            _vehicleInfoServiceStub = null;
            _notificationServiceStub = null;
            DataPackageService.Uninitialize();
            T2GManagerContainer.T2GManager = null;
            _t2gManager = null;
            _sessionManager = null;
            _requestFactory = null;
            _remoteDataStoreFactoryMock = null;

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
            CreateT2GServicesStub();
            InitializeDataPackageService();
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
    }
}
