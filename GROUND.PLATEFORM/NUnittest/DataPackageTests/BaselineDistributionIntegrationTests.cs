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
using System.Windows.Forms;

namespace DataPackageTests
{
    /// <summary>
    /// Class that test baseline distribution scenarios from distribute baseline until completion on embedded system.
    /// 
    /// This class simulate T2G, services on train to perform a complete validation of expected status of every stages of a baseline distribution.
    /// This class also validate the history log database and the notifications send by PIS-Ground.
    /// </summary>
    [TestFixture, Category("DistributionScenario"), Timeout(5 * 60 * 1000)]
    class BaselineDistributionIntegrationTests : IntegrationTestsBase
    {
        #region Fields


        private const string DatabaseName="TestDatabaseDataPackage";


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

        #endregion

        #region Test - Baseline distribution nominal scenario

        /// <summary>
        /// Distributes the baseline scenario nominal.
        /// </summary>
        [Test]
        public void DistributeBaselineScenario_Nominal()
        {
            MessageBox.Show("DistributeBaselineScenario_Nominal()");
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

            // Probleme
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
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, FUTURE_VERSION, "0.0.0.0", Guid.Empty, 0, BaselineProgressStatusEnum.UPDATED);
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
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, FUTURE_VERSION, "0.0.0.0", Guid.Empty, 0, BaselineProgressStatusEnum.UPDATED);
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

            // Stop data package service.
            StopDataPackageService();

            // Wait 2 seconds
            Thread.Sleep(2 * ONE_SECOND);

            // Restart the datapackage service
            InitializeDataPackageService(true);

            WaitPisGroundIsConnectedWithT2G();
            WaitTrainOnlineWithPISGround(TRAIN_NAME_1, true);
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, FUTURE_VERSION, result.reqId, transferTaskId, _fileTransferServiceStub.GetTask(transferTaskId).BaselineProgress);

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
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, FUTURE_VERSION, "0.0.0.0", Guid.Empty, 0, BaselineProgressStatusEnum.UPDATED);
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

            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, BaselineStatusUpdater.NoBaselineVersion, Guid.Empty, 0, BaselineProgressStatusEnum.UNKNOWN);
            Assert.IsNull(_fileTransferServiceStub.LastCreatedFolder, "Folder created while expecting not");
            Assert.IsNull(_fileTransferServiceStub.LastCreatedTransfer, "Transfer task created while expecting not");

            // Wait 5 seconds
            Thread.Sleep(5 * ONE_SECOND);
            VerifyTrainBaselineStatusInHistoryLog(TRAIN_NAME_1, true, DEFAULT_BASELINE, BaselineStatusUpdater.NoBaselineVersion, Guid.Empty, 0, BaselineProgressStatusEnum.UNKNOWN);

            Assert.IsNull(_fileTransferServiceStub.LastCreatedFolder, "Folder created while expecting not");
            Assert.IsNull(_fileTransferServiceStub.LastCreatedTransfer, "Transfer task created while expecting not");
        }

        #endregion
    }
}
