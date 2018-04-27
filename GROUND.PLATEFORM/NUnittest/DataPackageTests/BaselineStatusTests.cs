//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineStatusTest.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.DataPackage;
using PIS.Ground.RemoteDataStore;

using System.Reflection;
using System.Globalization;

namespace DataPackageTests
{
	/// <summary>BaselineStatus test class.</summary>
	[TestFixture, Category("BaselineStatus")]
	public class BaselineStatusTests
	{
		#region attributes

		/// <summary>Identifier for the system.</summary>
		private static readonly string _systemId = "MyTrain-69";
        private static readonly ushort _vehicleId = 69;
		private static readonly string _trainCurrentVersion = "2.9.0.2";
		private static readonly string _trainFutureVersion = "3.1.0.2";
		private static readonly string _trainPISVersion = "5.12.4.5";
		private static readonly string _latestKnownPISOnboardVersion = "5.13.0.1";
		private static readonly string _latestKnownCurrentBaselineVersion = "1.2.3.4";
		private static readonly string _latestKnownFutureBaselineVersion = "5.6.7.8";
		private static readonly string _latestKnownOnBoardFutureBaselineVersion = "6.5.4.3";
		private static readonly string _latestKnownAssignedFutureBaselineVersion = "5.8.7.3";

		/// <summary>The train 2ground client mock.</summary>
		private Mock<IT2GFileDistributionManager> _t2gMock;

		/// <summary>The baseline progresses to be used is input for the tests.</summary>
		private Dictionary<string, TrainBaselineStatusExtendedData> _baselineProgresses;

		private BaselineStatusUpdater.BaselineProgressUpdateProcedure _baselineProgressUpdateProc;
		private BaselineStatusUpdater.BaselineProgressRemoveProcedure _baselineProgressRemoveProc;

		#endregion


		#region Tests managment

		/// <summary>Initializes a new instance of the BaselineStatusTests class.</summary>
		public BaselineStatusTests()
		{
			_t2gMock = new Mock<IT2GFileDistributionManager>();

			_baselineProgresses = new Dictionary<string, TrainBaselineStatusExtendedData>();
			_baselineProgressUpdateProc = new BaselineStatusUpdater.BaselineProgressUpdateProcedure(UpdateProcedureDelegate);
			_baselineProgressRemoveProc = new BaselineStatusUpdater.BaselineProgressRemoveProcedure(RemoveProcedureDelegate);
		}

        /// <summary>
        /// Creates and initialize the baseline status updater object with default parameter.
        /// </summary>
        /// <returns>Baseline status updater object initialized.</returns>
        private BaselineStatusUpdaterInstrumented CreateBaselineStatusUpdater()
        {
            BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented();
            if (!baselineStatusUpdater.Initialize(_baselineProgresses,
                _baselineProgressUpdateProc, _baselineProgressRemoveProc, _t2gMock.Object, null))
            {
                baselineStatusUpdater.Dispose();
                Assert.Fail("Failed to initialize baseline status updated object");
            }

            return baselineStatusUpdater;
        }

		public bool UpdateProcedureDelegate(string trainId, TrainBaselineStatusData progressInfo)
		{
			return true;
		}

		public bool RemoveProcedureDelegate(string trainId)
		{
			return true;
		}



		/// <summary>Builds a sample transfer task to be used as input for the tests.</summary>
		/// <returns>A new TransferTaskData object with selected values.</returns>
		private static TransferTaskData BuildSampleTransferTask()
		{
			var sampleTransferTask = new TransferTaskData();

			sampleTransferTask.TaskId = 213;
			sampleTransferTask.TaskState = TaskState.Completed;
			sampleTransferTask.TaskSystemId = _systemId;

			return sampleTransferTask;
		}

		/// <summary>Builds sample system information.</summary>
		/// <returns>A new SystemInfo object with selected values.</returns>
		private static SystemInfo BuildSampleSystemInfo()
		{
			PisBaseline baseline = new PisBaseline();
			baseline.CurrentVersionOut = _trainCurrentVersion;
			baseline.FutureVersionOut = _trainFutureVersion;
            baseline.CurrentValidOut = "true";
            baseline.FutureValidOut = "true";

			PisVersion version = new PisVersion();
			version.VersionPISSoftware = _trainPISVersion;

			PisMission mission = new PisMission();

			var sampleSystemInfo = new SystemInfo(
				_systemId,
				"MISSION-ABC",
                _vehicleId,
                0,
				true,
				CommunicationLink.WIFI,
				new ServiceInfoList(),
				baseline,
				version,
				mission,
				true);

			return sampleSystemInfo;
		}

		/// <summary>Builds sample system information.</summary>
		/// <returns>A new SystemInfo object with selected values.</returns>
		private static SystemInfo BuildSampleSystemInfo_OfflineWithoutVersions()
		{
			PisBaseline baseline = new PisBaseline();

			PisVersion version = new PisVersion();
			version.VersionPISSoftware = string.Empty;

			PisMission mission = new PisMission();

			var sampleSystemInfo = new SystemInfo(
				_systemId,
				"MISSION-ABC",
                _vehicleId,
                0,
				false,
				CommunicationLink.WIFI,
				new ServiceInfoList(),
				baseline,
				version,
				mission,
				false);

			return sampleSystemInfo;
		}

		/// <summary>Builds sample system information.</summary>
		/// <returns>A new SystemInfo object with selected values.</returns>
		private static SystemInfo BuildSampleSystemInfo_OfflineWithVersions()
		{
			PisBaseline baseline = new PisBaseline();
			baseline.CurrentVersionOut = _trainCurrentVersion;
			baseline.FutureVersionOut = _trainFutureVersion;
            baseline.CurrentValidOut = "true";
            baseline.FutureValidOut = "true";

			PisVersion version = new PisVersion();
			version.VersionPISSoftware = _trainPISVersion;

			PisMission mission = new PisMission();

			var sampleSystemInfo = new SystemInfo(
				_systemId,
				"MISSION-ABC",
				_vehicleId,
				0,
				false,
				CommunicationLink.WIFI,
				new ServiceInfoList(),
				baseline,
				version,
				mission,
				false);

			return sampleSystemInfo;
		}

		private static TrainBaselineStatusData BuildSampleTrainBaselineStatusData()
		{
			TrainBaselineStatusData TrainBaselineStatusData = new TrainBaselineStatusData();

			TrainBaselineStatusData.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
			TrainBaselineStatusData.CurrentBaselineVersion = _latestKnownCurrentBaselineVersion;
			TrainBaselineStatusData.FutureBaselineVersion = _latestKnownFutureBaselineVersion;
			TrainBaselineStatusData.OnlineStatus = true;
			TrainBaselineStatusData.PisOnBoardVersion = _latestKnownPISOnboardVersion;
			TrainBaselineStatusData.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
			TrainBaselineStatusData.TaskId = 746;
            TrainBaselineStatusData.TrainNumber = "";
            TrainBaselineStatusData.TrainId = _systemId;

			return TrainBaselineStatusData;
		}

		private static TrainBaselineStatusExtendedData BuildTrainBaselineStatusExtendedData()
		{
			var lStatusDate = new TrainBaselineStatusData();

			lStatusDate.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
			lStatusDate.CurrentBaselineVersion = _latestKnownCurrentBaselineVersion;
			lStatusDate.FutureBaselineVersion = _latestKnownFutureBaselineVersion;
			lStatusDate.OnlineStatus = true;
			lStatusDate.PisOnBoardVersion = _latestKnownPISOnboardVersion;
			lStatusDate.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
			lStatusDate.TaskId = 746;
			lStatusDate.TrainNumber = _systemId;

			var lStatusExtendedData = new TrainBaselineStatusExtendedData(
				lStatusDate,
				_latestKnownAssignedFutureBaselineVersion,
				_latestKnownOnBoardFutureBaselineVersion,
                _latestKnownCurrentBaselineVersion,
				true);

			return lStatusExtendedData;
		}


		public void UpdateBaselineProgressFromSystemInfo(
			SystemInfo notification,
			string assignedCurrentBaseline,
			string assignedFutureBaseline,
			ref TrainBaselineStatusData TrainBaselineStatusData)
		{
			MethodInfo lMethodInfo = typeof(BaselineStatusUpdater).GetMethod("UpdateBaselineProgressFromSystemInfo",
				BindingFlags.NonPublic | BindingFlags.Public);

			object value = lMethodInfo.Invoke(null, new object[] { notification, assignedCurrentBaseline,
				assignedFutureBaseline, TrainBaselineStatusData });

			return;
		}

		/// <summary>Setups called before each test to initialize variables.</summary>
		[SetUp]
		public void Setup()
		{
		}

		/// <summary>Tear down called after each test to clean.</summary>
		[TearDown]
		public void TearDown()
		{
			// Do something after each tests
		}

		#endregion


		#region UpdateBaselineProgressFromFileTransferNotificationTests

		[Test]
		public void UpdateBaselineProgressFromFileTransferNotificationTest()
		{
			TrainBaselineStatusExtendedData expectedProgress = null;
			TrainBaselineStatusExtendedData workingProgress = null;
			var notification = new FileDistributionStatusArgs();

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = CreateBaselineStatusUpdater())
            {
                // TaskState.Error
                // 
                expectedProgress = BuildTrainBaselineStatusExtendedData();
                expectedProgress.Status.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
                expectedProgress.Status.RequestId = Guid.Empty;
                expectedProgress.Status.TaskId = 0;
                expectedProgress.Status.FutureBaselineVersion = expectedProgress.OnBoardFutureBaseline;
                workingProgress = BuildTrainBaselineStatusExtendedData();
                notification.TaskStatus = TaskState.Error;
                baselineStatusUpdater.UpdateBaselineProgressFromFileTransferNotification(notification, ref workingProgress);
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(expectedProgress.Status, workingProgress.Status));

                // TaskState.Cancelled
                // 
                expectedProgress = BuildTrainBaselineStatusExtendedData();
                expectedProgress.Status.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
                expectedProgress.Status.RequestId = Guid.Empty;
                expectedProgress.Status.TaskId = 0;
                expectedProgress.Status.FutureBaselineVersion = expectedProgress.OnBoardFutureBaseline;
                workingProgress = BuildTrainBaselineStatusExtendedData();
                notification.TaskStatus = TaskState.Cancelled;
                baselineStatusUpdater.UpdateBaselineProgressFromFileTransferNotification(notification, ref workingProgress);
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(expectedProgress.Status, workingProgress.Status));

                // TaskState.Started + TaskPhase.Acquisition
                // 
                expectedProgress = BuildTrainBaselineStatusExtendedData();
                expectedProgress.Status.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_PLANNED;
                expectedProgress.Status.FutureBaselineVersion = expectedProgress.AssignedFutureBaseline;
                workingProgress = BuildTrainBaselineStatusExtendedData();
                notification.TaskStatus = TaskState.Started;
                notification.CurrentTaskPhase = TaskPhase.Acquisition;
                baselineStatusUpdater.UpdateBaselineProgressFromFileTransferNotification(notification, ref workingProgress);
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(expectedProgress.Status, workingProgress.Status));


                // TaskState.Started + TaskPhase.Distribution
                // 
                expectedProgress = BuildTrainBaselineStatusExtendedData();
                expectedProgress.Status.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS;
                expectedProgress.Status.FutureBaselineVersion = expectedProgress.AssignedFutureBaseline;
                workingProgress = BuildTrainBaselineStatusExtendedData();
                notification.TaskStatus = TaskState.Started;
                notification.CurrentTaskPhase = TaskPhase.Distribution;
                notification.DistributionCompletionPercent = 33;
                baselineStatusUpdater.UpdateBaselineProgressFromFileTransferNotification(notification, ref workingProgress);
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(expectedProgress.Status, workingProgress.Status));


                // TaskState.Stopped
                // 
                expectedProgress = BuildTrainBaselineStatusExtendedData();
                expectedProgress.Status.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_PAUSED;
                expectedProgress.Status.FutureBaselineVersion = expectedProgress.AssignedFutureBaseline;
                workingProgress = BuildTrainBaselineStatusExtendedData();
                notification.TaskStatus = TaskState.Stopped;
                workingProgress.Status.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS;
                baselineStatusUpdater.UpdateBaselineProgressFromFileTransferNotification(notification, ref workingProgress);
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(expectedProgress.Status, workingProgress.Status));


                // TaskState.Completed
                // 
                expectedProgress = BuildTrainBaselineStatusExtendedData();
                expectedProgress.Status.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_COMPLETED;
                expectedProgress.Status.FutureBaselineVersion = expectedProgress.AssignedFutureBaseline;
                workingProgress = BuildTrainBaselineStatusExtendedData();
                notification.TaskStatus = TaskState.Completed;
                baselineStatusUpdater.UpdateBaselineProgressFromFileTransferNotification(notification, ref workingProgress);
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(expectedProgress.Status, workingProgress.Status));
            }
		}

		#endregion


		#region ValidateBaselineProgressStatusTests

		[Test]
		public void ValidateBaselineProgressStatusTest()
		{
			BaselineProgressStatusEnum result = BaselineProgressStatusEnum.UNKNOWN;

			// Accepted updates
			// Monotonic progression from UNKNOWN to UPDATED

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = CreateBaselineStatusUpdater())
            {

                result = baselineStatusUpdater.ValidateBaselineProgressStatus(
                    BaselineProgressStatusEnum.UNKNOWN, BaselineProgressStatusEnum.TRANSFER_PLANNED);
                Assert.AreEqual(BaselineProgressStatusEnum.TRANSFER_PLANNED, result);

                result = baselineStatusUpdater.ValidateBaselineProgressStatus(
                    BaselineProgressStatusEnum.TRANSFER_PLANNED, BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS);
                Assert.AreEqual(BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS, result);

                result = baselineStatusUpdater.ValidateBaselineProgressStatus(
                    BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS, BaselineProgressStatusEnum.TRANSFER_PAUSED);
                Assert.AreEqual(BaselineProgressStatusEnum.TRANSFER_PAUSED, result);

                result = baselineStatusUpdater.ValidateBaselineProgressStatus(
                    BaselineProgressStatusEnum.TRANSFER_PAUSED, BaselineProgressStatusEnum.TRANSFER_COMPLETED);
                Assert.AreEqual(BaselineProgressStatusEnum.TRANSFER_COMPLETED, result);

                result = baselineStatusUpdater.ValidateBaselineProgressStatus(
                    BaselineProgressStatusEnum.TRANSFER_COMPLETED, BaselineProgressStatusEnum.DEPLOYED);
                Assert.AreEqual(BaselineProgressStatusEnum.DEPLOYED, result);

                result = baselineStatusUpdater.ValidateBaselineProgressStatus(
                    BaselineProgressStatusEnum.DEPLOYED, BaselineProgressStatusEnum.UPDATED);
                Assert.AreEqual(BaselineProgressStatusEnum.UPDATED, result);

                // Accepted updates
                // Going back from TRANSFER_PAUSED to TRANSFER_IN_PROGRESS

                result = baselineStatusUpdater.ValidateBaselineProgressStatus(
                    BaselineProgressStatusEnum.TRANSFER_PAUSED, BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS);
                Assert.AreEqual(BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS, result);

                // Rejected update

                result = baselineStatusUpdater.ValidateBaselineProgressStatus(
                    BaselineProgressStatusEnum.UPDATED, BaselineProgressStatusEnum.TRANSFER_COMPLETED);
                Assert.AreEqual(BaselineProgressStatusEnum.UPDATED, result);

                result = baselineStatusUpdater.ValidateBaselineProgressStatus(
                    BaselineProgressStatusEnum.DEPLOYED, BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS);
                Assert.AreEqual(BaselineProgressStatusEnum.DEPLOYED, result);

                result = baselineStatusUpdater.ValidateBaselineProgressStatus(
                    BaselineProgressStatusEnum.TRANSFER_PLANNED, BaselineProgressStatusEnum.UNKNOWN);
                Assert.AreEqual(BaselineProgressStatusEnum.TRANSFER_PLANNED, result);
            }
		}

		#endregion




		#region ProcessSystemChangedNotificationTests

        [Test]
        public void ProcessSystemChangedNotificationTest_Online()
        {
            TrainBaselineStatusData lUpdatedProgress;
            List<Recipient> lRecipients = new List<Recipient>();
            TransferTaskData lTask = BuildSampleTransferTask();
            Mock<IT2GFileDistributionManager> lT2GMock;

            /// SystemInfo = BuildSampleSystemInfo
            /// CurrentBaseline = _latestKnownCurrentBaselineVersion;
            /// FutureBaseline = _latestKnownFutureBaselineVersion;
            /// OnBoardFutureBaseline = null;
            /// IsDeepUpdate = true;
            /// CurrentProgress = BuildSampleTrainBaselineStatusData();

            lT2GMock = new Mock<IT2GFileDistributionManager>();

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                Assert.IsTrue(baselineStatusUpdater.Initialize(_baselineProgresses, _baselineProgressUpdateProc, _baselineProgressRemoveProc, lT2GMock.Object, null), "Failed to initialize baseline status updater");

                lT2GMock.Setup(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask));

                baselineStatusUpdater.ForceProgress(new TrainBaselineStatusExtendedData(BuildSampleTrainBaselineStatusData()));

                SystemInfo systemInfo = BuildSampleSystemInfo();
                baselineStatusUpdater.ProcessSystemChangedNotification(
                    systemInfo, _latestKnownCurrentBaselineVersion, _latestKnownFutureBaselineVersion);
                TrainBaselineStatusExtendedData statusUpdated;
                Assert.IsTrue(baselineStatusUpdater.TryGetEntry(systemInfo.SystemId, out statusUpdated), "System '{0}' is unknown by baseline status updater", systemInfo.SystemId);
                lUpdatedProgress = statusUpdated.Status;

                lT2GMock.Verify(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask),
                    Times.Once());

                Assert.AreNotEqual(Guid.Empty, lUpdatedProgress.RequestId);
                Assert.AreEqual(746, lUpdatedProgress.TaskId);
                Assert.AreEqual(_vehicleId.ToString(), lUpdatedProgress.TrainNumber);
                Assert.AreEqual(true, lUpdatedProgress.OnlineStatus);
                Assert.AreEqual(BaselineProgressStatusEnum.TRANSFER_COMPLETED, lUpdatedProgress.ProgressStatus);
                Assert.AreEqual(_trainCurrentVersion, lUpdatedProgress.CurrentBaselineVersion);
                Assert.AreEqual(_latestKnownFutureBaselineVersion, lUpdatedProgress.FutureBaselineVersion);
                Assert.AreEqual(_trainPISVersion, lUpdatedProgress.PisOnBoardVersion);
                Assert.IsFalse(statusUpdated.IsT2GPollingRequired, "IsT2GPollingRequired wasn't updated as expected");
            }

            /// SystemInfo = BuildSampleSystemInfo
            /// CurrentBaseline = _latestKnownCurrentBaselineVersion;
            /// FutureBaseline = _latestKnownFutureBaselineVersion;
            /// OnBoardFutureBaseline = null;
            /// IsDeepUpdate = false;
            /// CurrentProgress = BuildSampleTrainBaselineStatusData();

            lT2GMock = new Mock<IT2GFileDistributionManager>();

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(_baselineProgresses, _baselineProgressUpdateProc, _baselineProgressRemoveProc, lT2GMock.Object, null);

                lT2GMock.Setup(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask));

                TrainBaselineStatusExtendedData initialProgress = new TrainBaselineStatusExtendedData(BuildSampleTrainBaselineStatusData());
                initialProgress.IsT2GPollingRequired = false;
                baselineStatusUpdater.ForceProgress(initialProgress);

                SystemInfo systemInfo = BuildSampleSystemInfo();
                baselineStatusUpdater.ProcessSystemChangedNotification(
                    systemInfo, _latestKnownCurrentBaselineVersion, _latestKnownFutureBaselineVersion);
                TrainBaselineStatusExtendedData statusUpdated;
                Assert.IsTrue(baselineStatusUpdater.TryGetEntry(systemInfo.SystemId, out statusUpdated), "System '{0}' is unknown by baseline status updater", systemInfo.SystemId);
                lUpdatedProgress = statusUpdated.Status;

                lT2GMock.Verify(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask),
                    Times.Never());

                Assert.AreNotEqual(Guid.Empty, lUpdatedProgress.RequestId);
                Assert.AreEqual(746, lUpdatedProgress.TaskId);
                Assert.AreEqual(_vehicleId.ToString(), lUpdatedProgress.TrainNumber);
                Assert.AreEqual(true, lUpdatedProgress.OnlineStatus);
                Assert.AreEqual(BaselineProgressStatusEnum.UNKNOWN, lUpdatedProgress.ProgressStatus);
                Assert.AreEqual(_trainCurrentVersion, lUpdatedProgress.CurrentBaselineVersion);
                Assert.AreEqual(_trainFutureVersion, lUpdatedProgress.FutureBaselineVersion);
                Assert.AreEqual(_trainPISVersion, lUpdatedProgress.PisOnBoardVersion);
                Assert.IsFalse(statusUpdated.IsT2GPollingRequired, "IsT2GPollingRequired wasn't updated as expected");
            }
        }

		[Test]
		public void ProcessSystemChangedNotificationTest_OfflineWithoutVersions()
		{
			string lOnBoardFutureBaseline = string.Empty;
			TrainBaselineStatusData lUpdatedProgress;
			List<Recipient> lRecipients = new List<Recipient>();
			TransferTaskData lTask = BuildSampleTransferTask();
			Mock<IT2GFileDistributionManager> lT2GMock;


			/// SystemInfo = BuildSampleSystemInfo_OfflineWithoutVersions
			/// CurrentBaseline = "";
			/// FutureBaseline = "";
			/// OnBoardFutureBaseline = "";
			/// IsDeepUpdate = true;
			/// CurrentProgress = null;

			lT2GMock = new Mock<IT2GFileDistributionManager>();

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(_baselineProgresses, _baselineProgressUpdateProc, _baselineProgressRemoveProc, lT2GMock.Object, null);

                lT2GMock.Setup(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask));


                SystemInfo systemInfo = BuildSampleSystemInfo_OfflineWithoutVersions();
                baselineStatusUpdater.ProcessSystemChangedNotification(
                    systemInfo, string.Empty, string.Empty);
                TrainBaselineStatusExtendedData statusUpdated;
                Assert.IsTrue(baselineStatusUpdater.TryGetEntry(systemInfo.SystemId, out statusUpdated), "System '{0}' is unknown by baseline status updater", systemInfo.SystemId);
                lUpdatedProgress = statusUpdated.Status;

                lT2GMock.Verify(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask),
                    Times.Never());

                Assert.AreEqual(Guid.Empty, lUpdatedProgress.RequestId);
                Assert.AreEqual(0, lUpdatedProgress.TaskId);
                Assert.AreEqual(_vehicleId.ToString(), lUpdatedProgress.TrainNumber);
                Assert.AreEqual(false, lUpdatedProgress.OnlineStatus);
                Assert.AreEqual(BaselineProgressStatusEnum.UNKNOWN, lUpdatedProgress.ProgressStatus);
                Assert.AreEqual(BaselineStatusUpdater.NoBaselineVersion, lUpdatedProgress.CurrentBaselineVersion);
                Assert.AreEqual(BaselineStatusUpdater.NoBaselineVersion, lUpdatedProgress.FutureBaselineVersion);
                Assert.AreEqual(String.Empty, lUpdatedProgress.PisOnBoardVersion);
                Assert.IsFalse(statusUpdated.IsT2GPollingRequired, "IsT2GPollingRequired wasn't updated as expected");
            }


			/// SystemInfo = BuildSampleSystemInfo_OfflineWithoutVersions
			/// CurrentBaseline = "";
			/// FutureBaseline = "";
			/// OnBoardFutureBaseline = "";
			/// IsDeepUpdate = false;
			/// CurrentProgress = null;

			lT2GMock = new Mock<IT2GFileDistributionManager>();

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(_baselineProgresses, _baselineProgressUpdateProc, _baselineProgressRemoveProc, lT2GMock.Object, null);

                lT2GMock.Setup(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask));

                SystemInfo systemInfo = BuildSampleSystemInfo_OfflineWithoutVersions();
                baselineStatusUpdater.ProcessSystemChangedNotification(
                    systemInfo, string.Empty, string.Empty);
                TrainBaselineStatusExtendedData statusUpdated;
                Assert.IsTrue(baselineStatusUpdater.TryGetEntry(systemInfo.SystemId, out statusUpdated), "System '{0}' is unknown by baseline status updater", systemInfo.SystemId);
                lUpdatedProgress = statusUpdated.Status;

                lT2GMock.Verify(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask),
                    Times.Never());

                Assert.AreEqual(Guid.Empty, lUpdatedProgress.RequestId);
                Assert.AreEqual(0, lUpdatedProgress.TaskId);
                Assert.AreEqual(_vehicleId.ToString(), lUpdatedProgress.TrainNumber);
                Assert.AreEqual(false, lUpdatedProgress.OnlineStatus);
                Assert.AreEqual(BaselineProgressStatusEnum.UNKNOWN, lUpdatedProgress.ProgressStatus);
                Assert.AreEqual(BaselineStatusUpdater.NoBaselineVersion, lUpdatedProgress.CurrentBaselineVersion);
                Assert.AreEqual(BaselineStatusUpdater.NoBaselineVersion, lUpdatedProgress.FutureBaselineVersion);
                Assert.AreEqual(String.Empty, lUpdatedProgress.PisOnBoardVersion);
                Assert.IsFalse(statusUpdated.IsT2GPollingRequired, "IsT2GPollingRequired wasn't updated as expected");
            }
            
			/// SystemInfo = BuildSampleSystemInfo_OfflineWithoutVersions
			/// CurrentBaseline = _latestKnownCurrentBaselineVersion;
			/// FutureBaseline = _latestKnownFutureBaselineVersion;
			/// OnBoardFutureBaseline = _latestKnownOnBoardFutureBaselineVersion;
			/// IsDeepUpdate = true;
			/// CurrentProgress = BuildSampleTrainBaselineStatusData();

			lT2GMock = new Mock<IT2GFileDistributionManager>();

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(_baselineProgresses, _baselineProgressUpdateProc, _baselineProgressRemoveProc, lT2GMock.Object, null);

                lT2GMock.Setup(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask));

                lOnBoardFutureBaseline = "5.12.5.9";

                baselineStatusUpdater.ForceProgress(new TrainBaselineStatusExtendedData(BuildSampleTrainBaselineStatusData()));

                SystemInfo systemInfo = BuildSampleSystemInfo_OfflineWithoutVersions();
                baselineStatusUpdater.ProcessSystemChangedNotification(
                    systemInfo, _latestKnownCurrentBaselineVersion, _latestKnownFutureBaselineVersion);
                TrainBaselineStatusExtendedData statusUpdated;
                Assert.IsTrue(baselineStatusUpdater.TryGetEntry(systemInfo.SystemId, out statusUpdated), "System '{0}' is unknown by baseline status updater", systemInfo.SystemId);
                lUpdatedProgress = statusUpdated.Status;

                lT2GMock.Verify(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask),
                    Times.Once());

                Assert.AreNotEqual(Guid.Empty, lUpdatedProgress.RequestId);
                Assert.AreEqual(746, lUpdatedProgress.TaskId);
                Assert.AreEqual(_vehicleId.ToString(), lUpdatedProgress.TrainNumber);
                Assert.AreEqual(false, lUpdatedProgress.OnlineStatus);
                Assert.AreEqual(BaselineProgressStatusEnum.TRANSFER_COMPLETED, lUpdatedProgress.ProgressStatus);
                Assert.AreEqual(_latestKnownCurrentBaselineVersion, lUpdatedProgress.CurrentBaselineVersion);
                Assert.AreEqual(_latestKnownFutureBaselineVersion, lUpdatedProgress.FutureBaselineVersion);
                Assert.AreEqual(_latestKnownPISOnboardVersion, lUpdatedProgress.PisOnBoardVersion);
                Assert.IsFalse(statusUpdated.IsT2GPollingRequired, "IsT2GPollingRequired wasn't updated as expected");
            }


			/// SystemInfo = BuildSampleSystemInfo_OfflineWithoutVersions
			/// CurrentBaseline = _latestKnownCurrentBaselineVersion;
			/// FutureBaseline = _latestKnownFutureBaselineVersion;
			/// OnBoardFutureBaseline = "";
			/// IsDeepUpdate = false;
			/// CurrentProgress = BuildSampleTrainBaselineStatusData();

			lT2GMock = new Mock<IT2GFileDistributionManager>();

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(_baselineProgresses, _baselineProgressUpdateProc, _baselineProgressRemoveProc, lT2GMock.Object, null);

                lT2GMock.Setup(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask));


                TrainBaselineStatusExtendedData initialProgress = new TrainBaselineStatusExtendedData(BuildSampleTrainBaselineStatusData());
                initialProgress.IsT2GPollingRequired = false;
                baselineStatusUpdater.ForceProgress(initialProgress);

                SystemInfo systemInfo = BuildSampleSystemInfo_OfflineWithoutVersions();
                baselineStatusUpdater.ProcessSystemChangedNotification(
                    systemInfo, _latestKnownCurrentBaselineVersion, _latestKnownFutureBaselineVersion);
                TrainBaselineStatusExtendedData statusUpdated;
                Assert.IsTrue(baselineStatusUpdater.TryGetEntry(systemInfo.SystemId, out statusUpdated), "System '{0}' is unknown by baseline status updater", systemInfo.SystemId);
                lUpdatedProgress = statusUpdated.Status;

                lT2GMock.Verify(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask),
                    Times.Never());

                Assert.AreNotEqual(Guid.Empty, lUpdatedProgress.RequestId);
                Assert.AreEqual(746, lUpdatedProgress.TaskId);
                Assert.AreEqual(_vehicleId.ToString(), lUpdatedProgress.TrainNumber);
                Assert.AreEqual(false, lUpdatedProgress.OnlineStatus);
                Assert.AreEqual(BaselineProgressStatusEnum.UNKNOWN, lUpdatedProgress.ProgressStatus);
                Assert.AreEqual(_latestKnownCurrentBaselineVersion, lUpdatedProgress.CurrentBaselineVersion);
                Assert.AreEqual(_latestKnownFutureBaselineVersion, lUpdatedProgress.FutureBaselineVersion);
                Assert.AreEqual(_latestKnownPISOnboardVersion, lUpdatedProgress.PisOnBoardVersion);
                Assert.IsFalse(statusUpdated.IsT2GPollingRequired, "IsT2GPollingRequired wasn't updated as expected");
            }
		}

		[Test]
		public void ProcessSystemChangedNotificationTest_OfflineWithVersions()
		{
			string lOnBoardFutureBaseline = string.Empty;
			TrainBaselineStatusData lUpdatedProgress;
			List<Recipient> lRecipients = new List<Recipient>();
			TransferTaskData lTask = BuildSampleTransferTask();
			Mock<IT2GFileDistributionManager> lT2GMock;

			/// SystemInfo = BuildSampleSystemInfo_OfflineWithVersions
			/// CurrentBaseline = _latestKnownCurrentBaselineVersion;
			/// FutureBaseline = _latestKnownFutureBaselineVersion;
			/// OnBoardFutureBaseline = "";
			/// IsDeepUpdate = true;
			/// CurrentProgress = BuildSampleTrainBaselineStatusData();

			lT2GMock = new Mock<IT2GFileDistributionManager>();

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(_baselineProgresses, _baselineProgressUpdateProc, _baselineProgressRemoveProc, lT2GMock.Object, null);

                lT2GMock.Setup(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask));

                baselineStatusUpdater.ForceProgress(new TrainBaselineStatusExtendedData(BuildSampleTrainBaselineStatusData()));

                SystemInfo systemInfo = BuildSampleSystemInfo_OfflineWithVersions();
                baselineStatusUpdater.ProcessSystemChangedNotification(
                    systemInfo, _latestKnownCurrentBaselineVersion, _latestKnownFutureBaselineVersion);
                TrainBaselineStatusExtendedData statusUpdated;
                Assert.IsTrue(baselineStatusUpdater.TryGetEntry(systemInfo.SystemId, out statusUpdated), "System '{0}' is unknown by baseline status updater", systemInfo.SystemId);
                lUpdatedProgress = statusUpdated.Status;

                lT2GMock.Verify(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask),
                    Times.Once());

                Assert.AreNotEqual(Guid.Empty, lUpdatedProgress.RequestId);
                Assert.AreEqual(746, lUpdatedProgress.TaskId);
                Assert.AreEqual(_vehicleId.ToString(), lUpdatedProgress.TrainNumber);
                Assert.AreEqual(false, lUpdatedProgress.OnlineStatus);
                Assert.AreEqual(BaselineProgressStatusEnum.TRANSFER_COMPLETED, lUpdatedProgress.ProgressStatus);
                Assert.AreEqual(_trainCurrentVersion, lUpdatedProgress.CurrentBaselineVersion);
                Assert.AreEqual(_latestKnownFutureBaselineVersion, lUpdatedProgress.FutureBaselineVersion);
                Assert.AreEqual(_trainPISVersion, lUpdatedProgress.PisOnBoardVersion);
                Assert.IsFalse(statusUpdated.IsT2GPollingRequired, "IsT2GPollingRequired wasn't updated as expected");
            }

			/// SystemInfo = BuildSampleSystemInfo_OfflineWithVersions
			/// CurrentBaseline = _latestKnownCurrentBaselineVersion;
			/// FutureBaseline = _latestKnownFutureBaselineVersion;
			/// OnBoardFutureBaseline = "";
			/// IsDeepUpdate = false;
			/// CurrentProgress = BuildSampleTrainBaselineStatusData();

			lT2GMock = new Mock<IT2GFileDistributionManager>();

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(_baselineProgresses, _baselineProgressUpdateProc, _baselineProgressRemoveProc, lT2GMock.Object, null);

                lT2GMock.Setup(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask));

                TrainBaselineStatusExtendedData initialProgress = new TrainBaselineStatusExtendedData(BuildSampleTrainBaselineStatusData());
                initialProgress.IsT2GPollingRequired = false;
                baselineStatusUpdater.ForceProgress(initialProgress);

                SystemInfo systemInfo = BuildSampleSystemInfo_OfflineWithVersions();
                baselineStatusUpdater.ProcessSystemChangedNotification(
                    systemInfo, _latestKnownCurrentBaselineVersion, _latestKnownFutureBaselineVersion);
                TrainBaselineStatusExtendedData statusUpdated;
                Assert.IsTrue(baselineStatusUpdater.TryGetEntry(systemInfo.SystemId, out statusUpdated), "System '{0}' is unknown by baseline status updater", systemInfo.SystemId);
                lUpdatedProgress = statusUpdated.Status;

                lT2GMock.Verify(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask),
                    Times.Never());

                Assert.AreNotEqual(Guid.Empty, lUpdatedProgress.RequestId);
                Assert.AreEqual(746, lUpdatedProgress.TaskId);
                Assert.AreEqual("69", lUpdatedProgress.TrainNumber);
                Assert.AreEqual(false, lUpdatedProgress.OnlineStatus);
                Assert.AreEqual(BaselineProgressStatusEnum.UNKNOWN, lUpdatedProgress.ProgressStatus);
                Assert.AreEqual(_trainCurrentVersion, lUpdatedProgress.CurrentBaselineVersion);
                Assert.AreEqual(_trainFutureVersion, lUpdatedProgress.FutureBaselineVersion);
                Assert.AreEqual(_trainPISVersion, lUpdatedProgress.PisOnBoardVersion);
                Assert.IsFalse(statusUpdated.IsT2GPollingRequired, "IsT2GPollingRequired wasn't updated as expected");
            }
		}
		#endregion

		#region GigiTests

		class BaselineCallbackMock
		{
			public BaselineCallbackMock()
			{
				Reset();
			}

			public void Reset()
			{
				UpdateCallCount = 0;
				UpdatedTrain = null;
				UpdatedProgressInfo = null;
				ReturnValueForUpdate = true;
				RemoveCallCount = 0;
				RemoveTrain = null;
				ReturnValueForRemove = true;
			}

			public int UpdateCallCount;
			public string UpdatedTrain;
			public TrainBaselineStatusData UpdatedProgressInfo;
			public bool ReturnValueForUpdate;
			public int RemoveCallCount;
			public string RemoveTrain;
			public bool ReturnValueForRemove;

			public bool UpdateCallback(string trainId, TrainBaselineStatusData progressInfo)
			{
				++UpdateCallCount;
				UpdatedTrain = trainId;
				UpdatedProgressInfo = progressInfo;

				return ReturnValueForUpdate;
			}

			public bool RemoveCallback(string trainId)
			{
				++RemoveCallCount;
				RemoveTrain = trainId;

				return ReturnValueForRemove;
			}
		}


		[Test]
		public void ResetStatusEntriesTest()
		{
			// Preparing for the test
			//

			var lT2GMock = new Mock<IT2GFileDistributionManager>();
			var lBaselineProgresses = new Dictionary<string, TrainBaselineStatusExtendedData>();
			var lCallbackMock = new BaselineCallbackMock();
			var lUpdateProcedure = new BaselineStatusUpdater.BaselineProgressUpdateProcedure(lCallbackMock.UpdateCallback);
			var lRemoveProcedure = new BaselineStatusUpdater.BaselineProgressRemoveProcedure(lCallbackMock.RemoveCallback);

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(lBaselineProgresses, lUpdateProcedure, lRemoveProcedure, lT2GMock.Object, null);

                //
                // Resetting an empty dictionary
                // 

                lCallbackMock.Reset();
                baselineStatusUpdater.ResetStatusEntries(null);

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                //
                // Resetting a dictionary with 1 entry
                // 

                var lNewStatus = new TrainBaselineStatusData();
                var lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lNewStatus.RequestId = new Guid("6b20eac2-12c7-4b30-bdbb-22ab87015e56");
                lNewStatus.TrainId = "TRAIN-5";
                lNewStatus.TaskId = 94;
                lNewStatus.OnlineStatus = true;
                lNewStatus.CurrentBaselineVersion = "1.2.3.4";
                lNewStatus.FutureBaselineVersion = "2.3.4.5";
                lNewStatus.PisOnBoardVersion = "5.13.2.3";
                lNewStatus.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS;

                lBaselineProgresses["TRAIN-5"] = lNewExtendedStatus;

                TrainBaselineStatusData lExpectedStatus = lNewStatus.Clone();

                TrainBaselineStatusData lNewStatus2 = lNewStatus.Clone();
                TrainBaselineStatusData lNewStatus3 = lNewStatus.Clone();
                TrainBaselineStatusData lNewStatus4 = lNewStatus.Clone();

                lExpectedStatus.OnlineStatus = false;

                lCallbackMock.Reset();
                baselineStatusUpdater.ResetStatusEntries(null);

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-5", lCallbackMock.UpdatedTrain);
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(lExpectedStatus, lCallbackMock.UpdatedProgressInfo));


                //
                // Resetting a dictionary with 3 entries
                // 

                var lNewExtendedStatus2 = new TrainBaselineStatusExtendedData(lNewStatus2);
                var lNewExtendedStatus3 = new TrainBaselineStatusExtendedData(lNewStatus3);
                var lNewExtendedStatus4 = new TrainBaselineStatusExtendedData(lNewStatus4);

                lBaselineProgresses["TRAIN-2"] = lNewExtendedStatus2;
                lBaselineProgresses["TRAIN-3"] = lNewExtendedStatus3;
                lBaselineProgresses["TRAIN-4"] = lNewExtendedStatus3;

                lCallbackMock.Reset();
                baselineStatusUpdater.ResetStatusEntries(null);

                Assert.AreEqual(3, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                // Our callback mock can only record the parameters from the last invocation.
                // Because of that, we will not validate further when multiple calls are expected
            }
		}

		[Test]
		public void ProcessDistributeBaselineRequestTest()
		{
			// Preparing for the test
			//

			var lT2GMock = new Mock<IT2GFileDistributionManager>();
			var lBaselineProgresses = new Dictionary<string, TrainBaselineStatusExtendedData>();
			var lCallbackMock = new BaselineCallbackMock();
			var lUpdateProcedure = new BaselineStatusUpdater.BaselineProgressUpdateProcedure(lCallbackMock.UpdateCallback);
			var lRemoveProcedure = new BaselineStatusUpdater.BaselineProgressRemoveProcedure(lCallbackMock.RemoveCallback);

			//
			// Testing with an empty baseline deployment dictionary
			// 

			var lExpectedStatus = new TrainBaselineStatusData();
			lExpectedStatus.CurrentBaselineVersion = "3.2.1.8";
			lExpectedStatus.FutureBaselineVersion = "4.6.7.8";
			lExpectedStatus.OnlineStatus = true;
			lExpectedStatus.PisOnBoardVersion = BaselineStatusUpdater.UnknownVersion;
			lExpectedStatus.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
			lExpectedStatus.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
			lExpectedStatus.TaskId = 0;
			lExpectedStatus.TrainId = "TRAIN-5";
			lExpectedStatus.TrainNumber = "UNKNOWN";
			string lAssignedFutureBaselineVersion = "4.8.0.2";

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(lBaselineProgresses, lUpdateProcedure, lRemoveProcedure, lT2GMock.Object, null);

                baselineStatusUpdater.ProcessDistributeBaselineRequest(
                    lExpectedStatus.TrainId,
                    lExpectedStatus.RequestId,
                    lExpectedStatus.OnlineStatus,
                    lExpectedStatus.CurrentBaselineVersion,
                    lExpectedStatus.FutureBaselineVersion,
                    lAssignedFutureBaselineVersion);

                // Checking if the proper callback has been invoked and that the passed arguments are as expected

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-5", lCallbackMock.UpdatedTrain);
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(lExpectedStatus, lCallbackMock.UpdatedProgressInfo));

                //
                // Testing with an entry already existing for the train to be processed
                // 

                lCallbackMock.Reset();
                lBaselineProgresses["TRAIN-5"].Status.PisOnBoardVersion = "5.13.0.1";
                lBaselineProgresses["TRAIN-5"].Status.ProgressStatus = BaselineProgressStatusEnum.UPDATED;
                lBaselineProgresses["TRAIN-5"].Status.TaskId = 84;
                lBaselineProgresses["TRAIN-5"].Status.TrainNumber = "634";

                lExpectedStatus = new TrainBaselineStatusData();
                lExpectedStatus.CurrentBaselineVersion = "4.3.2.9";
                lExpectedStatus.FutureBaselineVersion = "5.7.8.9";
                lExpectedStatus.OnlineStatus = false;
                lExpectedStatus.PisOnBoardVersion = "5.13.0.1";
                lExpectedStatus.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
                lExpectedStatus.RequestId = new Guid("5b20eac4-92c7-0b30-bdbb-22a887015e59");
                lExpectedStatus.TaskId = 0;
                lExpectedStatus.TrainId = "TRAIN-5";
                lExpectedStatus.TrainNumber = "634";
                lAssignedFutureBaselineVersion = "5.9.1.3";

                baselineStatusUpdater.ProcessDistributeBaselineRequest(
                    lExpectedStatus.TrainId,
                    lExpectedStatus.RequestId,
                    lExpectedStatus.OnlineStatus,
                    lExpectedStatus.CurrentBaselineVersion,
                    lExpectedStatus.FutureBaselineVersion,
                    lAssignedFutureBaselineVersion);

                // Checking if the proper callback has been invoked and that the passed arguments are as expected

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-5", lCallbackMock.UpdatedTrain);
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(lExpectedStatus, lCallbackMock.UpdatedProgressInfo));

                //
                // Testing with a train that has no entry in the baseline deployment dictionary
                // 

                lCallbackMock.Reset();

                lExpectedStatus = new TrainBaselineStatusData();
                lExpectedStatus.CurrentBaselineVersion = "4.1.0.5";
                lExpectedStatus.FutureBaselineVersion = "6.8.9.10";
                lExpectedStatus.OnlineStatus = true;
                lExpectedStatus.PisOnBoardVersion = BaselineStatusUpdater.UnknownVersion;
                lExpectedStatus.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
                lExpectedStatus.RequestId = new Guid("8c20eac7-02c4-1b32-fdba-32a887015e50");
                lExpectedStatus.TaskId = 0;
                lExpectedStatus.TrainId = "TRAIN-XXX";
                lExpectedStatus.TrainNumber = "UNKNOWN";
                lAssignedFutureBaselineVersion = "6.0.2.5";

                baselineStatusUpdater.ProcessDistributeBaselineRequest(
                    lExpectedStatus.TrainId,
                    lExpectedStatus.RequestId,
                    lExpectedStatus.OnlineStatus,
                    lExpectedStatus.CurrentBaselineVersion,
                    lExpectedStatus.FutureBaselineVersion,
                    lAssignedFutureBaselineVersion);

                // Checking if the proper callback has been invoked and that the passed arguments are as expected

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-XXX", lCallbackMock.UpdatedTrain);
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(lExpectedStatus, lCallbackMock.UpdatedProgressInfo));
            }
		}

		[Test]
		public void ProcessTaskIdTest()
		{
			// Preparing for the test
			//

			var lT2GMock = new Mock<IT2GFileDistributionManager>();
			var lBaselineProgresses = new Dictionary<string, TrainBaselineStatusExtendedData>();
			var lCallbackMock = new BaselineCallbackMock();
			var lUpdateProcedure = new BaselineStatusUpdater.BaselineProgressUpdateProcedure(lCallbackMock.UpdateCallback);
			var lRemoveProcedure = new BaselineStatusUpdater.BaselineProgressRemoveProcedure(lCallbackMock.RemoveCallback);

			//
			// Testing with an empty baseline deployment dictionary
			// 

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                Assert.IsTrue(baselineStatusUpdater.Initialize(lBaselineProgresses, lUpdateProcedure, lRemoveProcedure, lT2GMock.Object, null), "Failed to initialize baseline status updater object");

                var lExpectedStatus = new TrainBaselineStatusData();
                lExpectedStatus.TrainId = "TRAIN-5";
                lExpectedStatus.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lExpectedStatus.TaskId = 93;

                baselineStatusUpdater.ProcessTaskId(
                    lExpectedStatus.TrainId,
                    lExpectedStatus.RequestId,
                    lExpectedStatus.TaskId);

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                var lNewStatus = new TrainBaselineStatusData();
                var lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lNewStatus.RequestId = new Guid("6b20eac2-12c7-4b30-bdbb-22ab87015e56");
                lNewStatus.TrainId = "TRAIN-XXX";
                lNewStatus.TaskId = 0;

                lBaselineProgresses["TRAIN-XXX"] = lNewExtendedStatus;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessTaskId(
                    lExpectedStatus.TrainId,
                    lExpectedStatus.RequestId,
                    lExpectedStatus.TaskId);

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                lNewStatus = new TrainBaselineStatusData();
                lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lNewStatus.RequestId = new Guid("6b20eac2-12c7-4b30-bdbb-22ab87015e56");
                lNewStatus.TrainId = "TRAIN-5";
                lNewStatus.TaskId = 0;

                lBaselineProgresses["TRAIN-5"] = lNewExtendedStatus;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessTaskId(
                    lExpectedStatus.TrainId,
                    lExpectedStatus.RequestId,
                    lExpectedStatus.TaskId);

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                lNewStatus = new TrainBaselineStatusData();
                lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lNewStatus.RequestId = lExpectedStatus.RequestId;
                lNewStatus.TrainId = lExpectedStatus.TrainId;
                lNewStatus.TaskId = 0;

                lBaselineProgresses["TRAIN-5"] = lNewExtendedStatus;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessTaskId(
                    lExpectedStatus.TrainId,
                    lExpectedStatus.RequestId,
                    lExpectedStatus.TaskId);

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-5", lCallbackMock.UpdatedTrain);
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(lExpectedStatus, lCallbackMock.UpdatedProgressInfo));
            }
		}

		[Test]
		public void ProcessFileTransferNotificationTest()
		{
			// Preparing for the test
			//

			var lT2GMock = new Mock<IT2GFileDistributionManager>();
			var lBaselineProgresses = new Dictionary<string, TrainBaselineStatusExtendedData>();
			var lCallbackMock = new BaselineCallbackMock();
			var lUpdateProcedure = new BaselineStatusUpdater.BaselineProgressUpdateProcedure(lCallbackMock.UpdateCallback);
			var lRemoveProcedure = new BaselineStatusUpdater.BaselineProgressRemoveProcedure(lCallbackMock.RemoveCallback);

			//
			// Testing with an empty baseline deployment dictionary
			// 

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                Assert.IsTrue(baselineStatusUpdater.Initialize(lBaselineProgresses, lUpdateProcedure, lRemoveProcedure, lT2GMock.Object, null), "Failed to initialize baseline status updater");

                var lNotification = new FileDistributionStatusArgs();
                lNotification.TaskId = 71;
                lNotification.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lNotification.TaskStatus = TaskState.Started;
                lNotification.CurrentTaskPhase = TaskPhase.Acquisition;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessFileTransferNotification(lNotification);

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                //
                // Testing with one entry this does not match the notification task id and request id
                // 

                var lNewStatus = new TrainBaselineStatusData();
                var lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lNewStatus.RequestId = new Guid("6b20eac2-12c7-4b30-bdbb-22ab87015e56");
                lNewStatus.TrainId = "TRAIN-5";
                lNewStatus.TaskId = 94;

                lBaselineProgresses["TRAIN-5"] = lNewExtendedStatus;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessFileTransferNotification(lNotification);

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                //
                // Testing with one entry that matches only the request id
                // 

                lNewStatus = new TrainBaselineStatusData();
                lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lNewStatus.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lNewStatus.TrainId = "TRAIN-6";
                lNewStatus.TaskId = 94;

                lBaselineProgresses["TRAIN-6"] = lNewExtendedStatus;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessFileTransferNotification(lNotification);

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                //
                // Testing with one entry that matches only the task id
                // 

                lNewStatus = new TrainBaselineStatusData();
                lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lNewStatus.RequestId = new Guid("6b20eac2-12c7-4b30-bdbb-22ab87015e56");
                lNewStatus.TrainId = "TRAIN-7";
                lNewStatus.TaskId = 71;

                lBaselineProgresses["TRAIN-7"] = lNewExtendedStatus;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessFileTransferNotification(lNotification);

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                //
                // Testing with one entry that matches both task id and request id
                // 

                lNewStatus = new TrainBaselineStatusData();
                lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lNewExtendedStatus.AssignedFutureBaseline = "2.3.4.5";
                lNewExtendedStatus.OnBoardFutureBaseline = "1.2.3.4";
                lNewStatus.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lNewStatus.TrainId = "TRAIN-7";
                lNewStatus.TaskId = 71;


                lBaselineProgresses["TRAIN-7"] = lNewExtendedStatus;

                lCallbackMock.Reset();
                var lExpectedStatus = lNewStatus.Clone();

                baselineStatusUpdater.ProcessFileTransferNotification(lNotification);

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-7", lCallbackMock.UpdatedTrain);
                lExpectedStatus.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_PLANNED;
                lExpectedStatus.FutureBaselineVersion = lNewExtendedStatus.AssignedFutureBaseline;
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(lExpectedStatus, lCallbackMock.UpdatedProgressInfo));

                //
                // Updating the entry from TRANSFER_PLANNED to TRANSFER_IN_PROGRESS
                //

                lNotification.TaskStatus = TaskState.Started;
                lNotification.CurrentTaskPhase = TaskPhase.Distribution;
                lNotification.DistributionCompletionPercent = 13;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessFileTransferNotification(lNotification);

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-7", lCallbackMock.UpdatedTrain);
                lExpectedStatus.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS;
                lExpectedStatus.FutureBaselineVersion = lNewExtendedStatus.AssignedFutureBaseline;
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(lExpectedStatus, lCallbackMock.UpdatedProgressInfo));

                //
                // Updating the entry from  TRANSFER_IN_PROGRESS to TRANSFER_COMPLETED
                //

                lNotification.TaskStatus = TaskState.Completed;
                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessFileTransferNotification(lNotification);

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-7", lCallbackMock.UpdatedTrain);
                lExpectedStatus.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_COMPLETED;
                lExpectedStatus.FutureBaselineVersion = lNewExtendedStatus.AssignedFutureBaseline;
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(lExpectedStatus, lCallbackMock.UpdatedProgressInfo));

                //
                // Trying to update the entry from TRANSFER_COMPLETED to TRANSFER_PAUSED
                //

                lNotification.TaskStatus = TaskState.Stopped;
                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessFileTransferNotification(lNotification);

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);


                //
                // Transfer error while TRANSFER_IN_PROGRESS
                //

                lBaselineProgresses["TRAIN-7"].Status.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS;
                lNotification.TaskStatus = TaskState.Error;
                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessFileTransferNotification(lNotification);

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-7", lCallbackMock.UpdatedTrain);
                lExpectedStatus.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
                lExpectedStatus.RequestId = Guid.Empty;
                lExpectedStatus.TaskId = 0;
                lExpectedStatus.FutureBaselineVersion = lNewExtendedStatus.OnBoardFutureBaseline;
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(lExpectedStatus, lCallbackMock.UpdatedProgressInfo));


                //
                // Transfer cancelled while TRANSFER_IN_PROGRESS
                //

                lBaselineProgresses["TRAIN-7"].Status.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS;
                lBaselineProgresses["TRAIN-7"].Status.RequestId = lNotification.RequestId;
                lBaselineProgresses["TRAIN-7"].Status.TaskId = lNotification.TaskId;
                lNotification.TaskStatus = TaskState.Cancelled;
                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessFileTransferNotification(lNotification);

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-7", lCallbackMock.UpdatedTrain);
                lExpectedStatus.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
                lExpectedStatus.RequestId = Guid.Empty;
                lExpectedStatus.TaskId = 0;
                lExpectedStatus.FutureBaselineVersion = lNewExtendedStatus.OnBoardFutureBaseline;
                Assert.AreEqual(true, TrainBaselineStatusData.AreEqual(lExpectedStatus, lCallbackMock.UpdatedProgressInfo));

                //
                // Transfer progress while in error (or no request)
                //

                lBaselineProgresses["TRAIN-7"].Status.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
                lBaselineProgresses["TRAIN-7"].Status.RequestId = Guid.Empty;
                lNotification.TaskStatus = TaskState.Started;
                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessFileTransferNotification(lNotification);

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
            }
		}

		[Test]
		public void ProcessSIFNotificationTest()
		{
			// Preparing for the test
			//

			var lT2GMock = new Mock<IT2GFileDistributionManager>();
			var lBaselineProgresses = new Dictionary<string, TrainBaselineStatusExtendedData>();
			var lCallbackMock = new BaselineCallbackMock();
			var lUpdateProcedure = new BaselineStatusUpdater.BaselineProgressUpdateProcedure(lCallbackMock.UpdateCallback);
			var lRemoveProcedure = new BaselineStatusUpdater.BaselineProgressRemoveProcedure(lCallbackMock.RemoveCallback);

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                Assert.IsTrue(baselineStatusUpdater.Initialize(lBaselineProgresses, lUpdateProcedure, lRemoveProcedure, lT2GMock.Object, null), "Failed to initialize baseline status updater");

                //
                // Testing with an empty baseline deployment dictionary
                // 

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessSIFNotification("TRAIN-5",
                    PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedRejectedByElement);

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                //
                // Testing with one entry of the wrong train
                // 

                var lNewStatus = new TrainBaselineStatusData();
                var lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lNewStatus.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS;
                lNewStatus.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lBaselineProgresses["TRAIN-5"] = lNewExtendedStatus;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessSIFNotification("TRAIN-7",
                    PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedRejectedByElement);

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                //
                // Testing a failure notification with a matching entry while transfer in progress
                // 

                lNewStatus = new TrainBaselineStatusData();
                lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lNewStatus.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS;
                lNewStatus.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lNewStatus.FutureBaselineVersion = "2.3.4.5";
                lNewExtendedStatus.OnBoardFutureBaseline = "1.2.3.4";
                lNewExtendedStatus.AssignedFutureBaseline = "3.5.4.2";

                lBaselineProgresses["TRAIN-7"] = lNewExtendedStatus;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessSIFNotification("TRAIN-7",
                    PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedRejectedByElement);

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-7", lCallbackMock.UpdatedTrain);

                if (lCallbackMock.UpdatedProgressInfo != null)
                {
                    Assert.AreEqual(BaselineProgressStatusEnum.UNKNOWN, lCallbackMock.UpdatedProgressInfo.ProgressStatus);
                }
                else
                {
                    Assert.Fail("lCallbackMock.UpdatedProgressInfo is null");
                }

                Assert.AreEqual("1.2.3.4", lCallbackMock.UpdatedProgressInfo.FutureBaselineVersion);

                //
                // Testing a failure notification with a matching entry while transfer paused
                // 

                lNewStatus = new TrainBaselineStatusData();
                lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lNewStatus.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_PAUSED;
                lNewStatus.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lNewStatus.FutureBaselineVersion = "2.3.4.5";
                lNewExtendedStatus.OnBoardFutureBaseline = "1.2.3.4";
                lNewExtendedStatus.AssignedFutureBaseline = "3.5.4.2";

                lBaselineProgresses["TRAIN-7"] = lNewExtendedStatus;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessSIFNotification("TRAIN-7",
                    PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedRejectedByElement);

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-7", lCallbackMock.UpdatedTrain);

                if (lCallbackMock.UpdatedProgressInfo != null)
                {
                    Assert.AreEqual(BaselineProgressStatusEnum.UNKNOWN, lCallbackMock.UpdatedProgressInfo.ProgressStatus);
                }
                else
                {
                    Assert.Fail("lCallbackMock.UpdatedProgressInfo is null");
                }

                Assert.AreEqual("1.2.3.4", lCallbackMock.UpdatedProgressInfo.FutureBaselineVersion);

                //
                // Testing a failure notification with a matching entry that is already completed
                // 

                lNewStatus = new TrainBaselineStatusData();
                lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lNewStatus.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_COMPLETED;
                lNewStatus.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lNewStatus.FutureBaselineVersion = "2.3.4.5";
                lNewExtendedStatus.OnBoardFutureBaseline = "1.2.3.4";
                lNewExtendedStatus.AssignedFutureBaseline = "3.5.4.2";

                lBaselineProgresses["TRAIN-7"] = lNewExtendedStatus;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessSIFNotification("TRAIN-7",
                    PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedRejectedByElement);

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                //
                // Testing a failure notification with a matching entry that is already deployed
                // 

                lNewStatus = new TrainBaselineStatusData();
                lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lNewStatus.ProgressStatus = BaselineProgressStatusEnum.DEPLOYED;
                lNewStatus.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lNewStatus.FutureBaselineVersion = "3.5.4.2";
                lNewExtendedStatus.OnBoardFutureBaseline = "1.2.3.4";
                lNewExtendedStatus.AssignedFutureBaseline = "3.5.4.2";

                lBaselineProgresses["TRAIN-7"] = lNewExtendedStatus;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessSIFNotification("TRAIN-7",
                    PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedRejectedByElement);

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
            }
		}

		[Test]
		public void ProcessElementDeletedNotificationTest()
		{
			// Preparing for the test
			//

			//public static void ProcessElementDeletedNotification(string trainId)

			var lT2GMock = new Mock<IT2GFileDistributionManager>();
			var lBaselineProgresses = new Dictionary<string, TrainBaselineStatusExtendedData>();
			var lCallbackMock = new BaselineCallbackMock();
			var lUpdateProcedure = new BaselineStatusUpdater.BaselineProgressUpdateProcedure(lCallbackMock.UpdateCallback);
			var lRemoveProcedure = new BaselineStatusUpdater.BaselineProgressRemoveProcedure(lCallbackMock.RemoveCallback);

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {

                Assert.IsTrue(baselineStatusUpdater.Initialize(lBaselineProgresses, lUpdateProcedure, lRemoveProcedure, lT2GMock.Object, null), "Failed to initialize baseline status updater.");

                //
                // Testing with an empty baseline deployment dictionary
                // 

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessElementDeletedNotification("TRAIN-7");

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                //
                // Testing with one entry of the wrong train
                // 

                var lNewStatus = new TrainBaselineStatusData();
                var lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lBaselineProgresses["TRAIN-5"] = lNewExtendedStatus;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessElementDeletedNotification("TRAIN-7");

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);

                //
                // Testing a completion notification with a matching entry
                // 

                lNewStatus = new TrainBaselineStatusData();
                lNewExtendedStatus = new TrainBaselineStatusExtendedData(lNewStatus);
                lBaselineProgresses["TRAIN-7"] = lNewExtendedStatus;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessElementDeletedNotification("TRAIN-7");

                Assert.AreEqual(0, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(1, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-7", lCallbackMock.RemoveTrain);
            }

		}

		[Test]
		public void ProcessSystemChangedNotificationTest()
		{
			// Preparing for the test
			//

			var lT2GMock = new Mock<IT2GFileDistributionManager>();
			List<Recipient> lRecipients = new List<Recipient>();
			TransferTaskData lTask = BuildSampleTransferTask();
			var lBaselineProgresses = new Dictionary<string, TrainBaselineStatusExtendedData>();
			var lCallbackMock = new BaselineCallbackMock();
			var lUpdateProcedure = new BaselineStatusUpdater.BaselineProgressUpdateProcedure(lCallbackMock.UpdateCallback);
			var lRemoveProcedure = new BaselineStatusUpdater.BaselineProgressRemoveProcedure(lCallbackMock.RemoveCallback);

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(lBaselineProgresses, lUpdateProcedure, lRemoveProcedure, lT2GMock.Object, null);

                //
                // Testing with an empty baseline deployment dictionary
                // 

                var lNotification = new SystemInfo(
                    "TRAIN-5",
                    "",
                    968,
                    0,
                    true,
                    CommunicationLink.WIFI,
                    new ServiceInfoList(),
                    new PisBaseline { CurrentVersionOut = "3.4.5.6", FutureVersionOut = "5.6.7.8", CurrentValidOut="true", FutureValidOut="true" },
                    new PisVersion { VersionPISSoftware = "5.13.0.6" },
                    new PisMission(),
                    true);

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessSystemChangedNotification(lNotification, "3.4.5.6", "7.4.6.2");

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-5", lCallbackMock.UpdatedTrain);

                if (lCallbackMock.UpdatedProgressInfo != null)
                {
                    Assert.AreEqual(BaselineProgressStatusEnum.UNKNOWN, lCallbackMock.UpdatedProgressInfo.ProgressStatus);
                }
                else
                {
                    Assert.Fail("lCallbackMock.UpdatedProgressInfo is null");
                }

                Assert.AreEqual("3.4.5.6", lCallbackMock.UpdatedProgressInfo.CurrentBaselineVersion);
                Assert.AreEqual("5.6.7.8", lCallbackMock.UpdatedProgressInfo.FutureBaselineVersion);

                //
                // Testing with current baseline == assigned future baseline
                // 

                lNotification = new SystemInfo(
                    "TRAIN-5",
                    "",
                    968,
                    0,
                    true,
                    CommunicationLink.WIFI,
                    new ServiceInfoList(),
                    new PisBaseline { CurrentVersionOut = "5.6.7.8", FutureVersionOut = "0.0.0.0", CurrentValidOut="true", FutureValidOut="false" },
                    new PisVersion { VersionPISSoftware = "5.13.0.6" },
                    new PisMission(),
                    true);

                lBaselineProgresses["TRAIN-5"].Status.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lBaselineProgresses["TRAIN-5"].Status.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessSystemChangedNotification(lNotification, "0.0.0.0", "5.6.7.8");

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-5", lCallbackMock.UpdatedTrain);

                if (lCallbackMock.UpdatedProgressInfo != null)
                {
                    Assert.AreEqual(BaselineProgressStatusEnum.UPDATED, lCallbackMock.UpdatedProgressInfo.ProgressStatus);
                }
                else
                {
                    Assert.Fail("lCallbackMock.UpdatedProgressInfo is null");
                }

                Assert.AreEqual("5.6.7.8", lCallbackMock.UpdatedProgressInfo.CurrentBaselineVersion);
                Assert.AreEqual("0.0.0.0", lCallbackMock.UpdatedProgressInfo.FutureBaselineVersion);

                //
                // Testing with future baseline == assigned future baseline
                // 

                lNotification = new SystemInfo(
                    "TRAIN-5",
                    "",
                    968,
                    0,
                    true,
                    CommunicationLink.WIFI,
                    new ServiceInfoList(),
                    new PisBaseline { CurrentVersionOut = "5.6.7.8", FutureVersionOut = "6.7.8.9", CurrentValidOut="true", FutureValidOut="true"},
                    new PisVersion { VersionPISSoftware = "5.13.0.6" },
                    new PisMission(),
                    true);

                lBaselineProgresses["TRAIN-5"].Status.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lBaselineProgresses["TRAIN-5"].Status.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessSystemChangedNotification(lNotification, "0.0.0.0", "6.7.8.9");

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-5", lCallbackMock.UpdatedTrain);

                if (lCallbackMock.UpdatedProgressInfo != null)
                {
                    Assert.AreEqual(BaselineProgressStatusEnum.DEPLOYED, lCallbackMock.UpdatedProgressInfo.ProgressStatus);
                }
                else
                {
                    Assert.Fail("lCallbackMock.UpdatedProgressInfo is null");
                }

                Assert.AreEqual("5.6.7.8", lCallbackMock.UpdatedProgressInfo.CurrentBaselineVersion);
                Assert.AreEqual("6.7.8.9", lCallbackMock.UpdatedProgressInfo.FutureBaselineVersion);

                //
                // Testing with future baseline != assigned future baseline &&
                //  current baseline != assigned future baseline
                // 

                lNotification = new SystemInfo(
                    "TRAIN-5",
                    "",
                    968,
                    0,
                    true,
                    CommunicationLink.WIFI,
                    new ServiceInfoList(),
                    new PisBaseline { CurrentVersionOut = "5.6.2.1", FutureVersionOut = "6.7.5.6", CurrentValidOut="true", FutureValidOut="true" },
                    new PisVersion { VersionPISSoftware = "5.13.0.6" },
                    new PisMission(),
                    true);

                lBaselineProgresses["TRAIN-5"].Status.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lBaselineProgresses["TRAIN-5"].Status.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessSystemChangedNotification(lNotification, "0.0.0.0", "3.4.5.6");

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-5", lCallbackMock.UpdatedTrain);

                if (lCallbackMock.UpdatedProgressInfo != null)
                {
                    Assert.AreEqual(BaselineProgressStatusEnum.UNKNOWN, lCallbackMock.UpdatedProgressInfo.ProgressStatus);
                }
                else
                {
                    Assert.Fail("lCallbackMock.UpdatedProgressInfo is null");
                }

                Assert.AreEqual("5.6.2.1", lCallbackMock.UpdatedProgressInfo.CurrentBaselineVersion);
                Assert.AreEqual("6.7.5.6", lCallbackMock.UpdatedProgressInfo.FutureBaselineVersion);

                //
                // Testing with an on-going transfer
                // 

                lBaselineProgresses["TRAIN-5"].Status.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lBaselineProgresses["TRAIN-5"].Status.TaskId = 85;
                lBaselineProgresses["TRAIN-5"].Status.ProgressStatus = BaselineProgressStatusEnum.TRANSFER_PLANNED;
                lBaselineProgresses["TRAIN-5"].Status.CurrentBaselineVersion = "1.2.3.4";
                lBaselineProgresses["TRAIN-5"].Status.FutureBaselineVersion = "2.5.3.1";
                lBaselineProgresses["TRAIN-5"].IsT2GPollingRequired = true;

                lNotification = new SystemInfo(
                    "TRAIN-5",
                    "",
                    968,
                    0,
                    true,
                    CommunicationLink.WIFI,
                    new ServiceInfoList(),
                    new PisBaseline { CurrentVersionOut = "5.6.7.8", FutureVersionOut = "6.7.8.9", CurrentValidOut = "true", FutureValidOut = "true" },
                    new PisVersion { VersionPISSoftware = "5.13.0.6" },
                    new PisMission(),
                    true);

                lTask.TaskState = TaskState.Started;
                lTask.TaskPhase = TaskPhase.Distribution;
                lTask.DistributionCompletionPercent = 95;

                lT2GMock.Setup(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask));

                lCallbackMock.Reset();

                baselineStatusUpdater.ProcessSystemChangedNotification(lNotification, "0.0.0.0", "2.5.3.1");

                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-5", lCallbackMock.UpdatedTrain);

                if (lCallbackMock.UpdatedProgressInfo != null)
                {
                    Assert.AreEqual(BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS, lCallbackMock.UpdatedProgressInfo.ProgressStatus);
                }
                else
                {
                    Assert.Fail("lCallbackMock.UpdatedProgressInfo is null");
                }

                Assert.AreEqual("5.6.7.8", lCallbackMock.UpdatedProgressInfo.CurrentBaselineVersion);
                Assert.AreEqual("2.5.3.1", lCallbackMock.UpdatedProgressInfo.FutureBaselineVersion);

                lT2GMock.Verify(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask),
                    Times.Once());
            }
		}

        [Test]
        public void ProcessSystemChangedNotificationTest_T2GBadTaskIdError1()
        {
            // Preparing for the test
            //
            var lT2GMock = new Mock<IT2GFileDistributionManager>();
            var lLogManagerMock = new Mock<ILogManager>();
            List<Recipient> lRecipients = new List<Recipient>();
            TransferTaskData lTask = BuildSampleTransferTask();
            var lBaselineProgresses = new Dictionary<string, TrainBaselineStatusExtendedData>();

            TrainBaselineStatusExtendedData initialInfo = BuildTrainBaselineStatusExtendedData();
            initialInfo.Status.TrainId = "TRAIN-5";
            lBaselineProgresses["TRAIN-5"] = initialInfo;

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(lBaselineProgresses, lT2GMock.Object, lLogManagerMock.Object, null);

                var lNotification = new SystemInfo(
                    "TRAIN-5",
                    "",
                    968,
                    0,
                    true,
                    CommunicationLink.WIFI,
                    new ServiceInfoList(),
                    new PisBaseline { CurrentVersionOut = "5.6.7.8", FutureVersionOut = "6.7.8.9", CurrentValidOut="true", FutureValidOut="true" },
                    new PisVersion { VersionPISSoftware = "5.13.0.6" },
                    new PisMission(),
                    true);

                lBaselineProgresses["TRAIN-5"].Status.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lBaselineProgresses["TRAIN-5"].Status.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
                lBaselineProgresses["TRAIN-5"].IsT2GPollingRequired = true;

                // If the provided task Id is unknown for T2G, GetTransferTask should return an appropriate error
                // (it will be a string that will contain 'F0308' code)
                lT2GMock.Setup(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask)).Returns("Error: F0308");

                // If there is a "Bad Task Id" error GetErrorCodeByDescription should return
                // the T2GFileDistributionManagerErrorEnum.eT2GFD_BadTaskId error
                lT2GMock.Setup(x => x.GetErrorCodeByDescription(It.IsAny<string>())).Returns(T2GFileDistributionManagerErrorEnum.eT2GFD_BadTaskId);

                baselineStatusUpdater.ProcessSystemChangedNotification(lNotification, "0.0.0.0", "7.4.6.2");

                lT2GMock.Verify(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask),
                    Times.Once());

                lT2GMock.Verify(x => x.GetErrorCodeByDescription(It.IsAny<string>()),
                    Times.Once());

                // Verify that the LogManager.UpdateTrainBaselineStatus() function is called
                // in order to update baseline update state in persistent storage
                lLogManagerMock.Verify(x => x.UpdateTrainBaselineStatus(
                    "TRAIN-5",
                    Guid.Empty,
                    0,
                    "968",
                    true,
                    BaselineProgressStatusEnum.UNKNOWN,
                    BaselineProgressStatusStateEnum.NONE,
                    "5.6.7.8",
                    "6.7.8.9",
                    "5.13.0.6"),
                    Times.Once());
            }
        }

        [Test]
        public void ProcessSystemChangedNotificationTest_T2GBadTaskIdError2()
        {
            // Preparing for the test
            //
            var lT2GMock = new Mock<IT2GFileDistributionManager>();
            List<Recipient> lRecipients = new List<Recipient>();
            TransferTaskData lTask = BuildSampleTransferTask();
            var lBaselineProgresses = new Dictionary<string, TrainBaselineStatusExtendedData>();
            var lCallbackMock = new BaselineCallbackMock();
            var lUpdateProcedure = new BaselineStatusUpdater.BaselineProgressUpdateProcedure(lCallbackMock.UpdateCallback);
            var lRemoveProcedure = new BaselineStatusUpdater.BaselineProgressRemoveProcedure(lCallbackMock.RemoveCallback);

            TrainBaselineStatusExtendedData initialInfo = BuildTrainBaselineStatusExtendedData();
            initialInfo.Status.TrainId = "TRAIN-5";
            lBaselineProgresses["TRAIN-5"] = initialInfo;

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(lBaselineProgresses, lUpdateProcedure, lRemoveProcedure, lT2GMock.Object, null);

                var lNotification = new SystemInfo(
                    "TRAIN-5",
                    "",
                    968,
                    0,
                    true,
                    CommunicationLink.WIFI,
                    new ServiceInfoList(),
                    new PisBaseline { CurrentVersionOut = "5.6.7.8", FutureVersionOut = "6.7.8.9", CurrentValidOut = "true", FutureValidOut = "true" },
                    new PisVersion { VersionPISSoftware = "5.13.0.6" },
                    new PisMission(),
                    true);

                lBaselineProgresses["TRAIN-5"].Status.RequestId = new Guid("4b20eac4-82c7-4b30-bdbb-22ab87015e55");
                lBaselineProgresses["TRAIN-5"].Status.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
                lBaselineProgresses["TRAIN-5"].IsT2GPollingRequired = true;

                lCallbackMock.Reset();

                // If the provided task Id is unknown for T2G, GetTransferTask should return an appropriate error
                // (it will be a string that will contain 'F0308' code)
                lT2GMock.Setup(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask)).Returns("Error: F0308");

                // If there is a "Bad Task Id" error GetErrorCodeByDescription should return
                // the T2GFileDistributionManagerErrorEnum.eT2GFD_BadTaskId error
                lT2GMock.Setup(x => x.GetErrorCodeByDescription(It.IsAny<string>())).Returns(T2GFileDistributionManagerErrorEnum.eT2GFD_BadTaskId);

                baselineStatusUpdater.ProcessSystemChangedNotification(lNotification, "0.0.0.0", "7.4.6.2");

                lT2GMock.Verify(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask),
                    Times.Once());

                lT2GMock.Verify(x => x.GetErrorCodeByDescription(It.IsAny<string>()),
                    Times.Once());

                // If baseline update status has changed - the BaselineProgressUpdateProcedure should be called
                // with the appropriate parameters
                // In the same time the BaselineProgressRemoveProcedure should not be called 
                Assert.AreEqual(1, lCallbackMock.UpdateCallCount);
                Assert.AreEqual(0, lCallbackMock.RemoveCallCount);
                Assert.AreEqual("TRAIN-5", lCallbackMock.UpdatedTrain);

                if (lCallbackMock.UpdatedProgressInfo != null)
                {
                    Assert.AreEqual(BaselineProgressStatusEnum.UNKNOWN, lCallbackMock.UpdatedProgressInfo.ProgressStatus);
                    Assert.AreEqual(Guid.Empty, lCallbackMock.UpdatedProgressInfo.RequestId);
                    Assert.AreEqual(0, lCallbackMock.UpdatedProgressInfo.TaskId);
                    Assert.AreEqual(false, lBaselineProgresses["TRAIN-5"].IsT2GPollingRequired);
                }
                else
                {
                    Assert.Fail("lCallbackMock.UpdatedProgressInfo is null");
                }
            }
        }

        [Test]
        public void ProcessSystemChangedNotificationTest_T2GUnknownError()
        {
            TrainBaselineStatusData lUpdatedProgress;
            List<Recipient> lRecipients = new List<Recipient>();
            TransferTaskData lTask = BuildSampleTransferTask();
            Mock<IT2GFileDistributionManager> lT2GMock;
            bool isDeepUpdate = true;
            TrainBaselineStatusData lBaselineStatusData = BuildSampleTrainBaselineStatusData();

            /// SystemInfo = BuildSampleSystemInfo
            /// CurrentBaseline = _latestKnownCurrentBaselineVersion;
            /// FutureBaseline = _latestKnownFutureBaselineVersion;
            /// OnBoardFutureBaseline = null;
            /// IsDeepUpdate = true;
            /// CurrentProgress = BuildSampleTrainBaselineStatusData();

            lT2GMock = new Mock<IT2GFileDistributionManager>();

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(_baselineProgresses, _baselineProgressUpdateProc, _baselineProgressRemoveProc, lT2GMock.Object, null);

                // If there is a T2G error the GetTransferTask should return a non-empty string with error's description
                lT2GMock.Setup(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask)).Returns("Error");

                // If there is a T2G error other than "Bad Task Id" GetErrorCodeByDescription should return
                // the T2GFileDistributionManagerErrorEnum.eT2GFD_Other error
                lT2GMock.Setup(x => x.GetErrorCodeByDescription(It.IsAny<string>())).Returns(T2GFileDistributionManagerErrorEnum.eT2GFD_Other);


                baselineStatusUpdater.ForceProgress(new TrainBaselineStatusExtendedData(BuildSampleTrainBaselineStatusData()));

                SystemInfo systemInfo = BuildSampleSystemInfo();
                baselineStatusUpdater.ProcessSystemChangedNotification(
                    systemInfo, _latestKnownCurrentBaselineVersion, _latestKnownFutureBaselineVersion);
                TrainBaselineStatusExtendedData statusUpdated;
                Assert.IsTrue(baselineStatusUpdater.TryGetEntry(systemInfo.SystemId, out statusUpdated), "System '{0}' is unknown by baseline status updater", systemInfo.SystemId);
                lUpdatedProgress = statusUpdated.Status;


                lT2GMock.Verify(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask),
                    Times.Once());

                lT2GMock.Verify(x => x.GetErrorCodeByDescription(It.IsAny<string>()),
                    Times.Once());

                // If GetTransferTask returns an error, other than "Bad Task Id" error - the request params shouldn't be changed          
                Assert.AreEqual(lBaselineStatusData.RequestId, lUpdatedProgress.RequestId);
                Assert.AreEqual(lBaselineStatusData.TaskId, lUpdatedProgress.TaskId);
                Assert.AreEqual(true, isDeepUpdate);
                Assert.IsTrue(statusUpdated.IsT2GPollingRequired, "IsT2GPollingRequired wasn't updated as expected");
            }
        }

        [Test]
        public void ProcessSystemChangedNotificationTest_T2GNoError()
        {
            TrainBaselineStatusData lUpdatedProgress;
            List<Recipient> lRecipients = new List<Recipient>();
            TransferTaskData lTask = BuildSampleTransferTask();
            Mock<IT2GFileDistributionManager> lT2GMock;
            TrainBaselineStatusData lBaselineStatusData = BuildSampleTrainBaselineStatusData();

            /// SystemInfo = BuildSampleSystemInfo
            /// CurrentBaseline = _latestKnownCurrentBaselineVersion;
            /// FutureBaseline = _latestKnownFutureBaselineVersion;
            /// OnBoardFutureBaseline = null;
            /// IsDeepUpdate = true;
            /// CurrentProgress = BuildSampleTrainBaselineStatusData();

            lT2GMock = new Mock<IT2GFileDistributionManager>();

            using (BaselineStatusUpdaterInstrumented baselineStatusUpdater = new BaselineStatusUpdaterInstrumented())
            {
                baselineStatusUpdater.Initialize(_baselineProgresses, _baselineProgressUpdateProc, _baselineProgressRemoveProc, lT2GMock.Object, null);

                // If there is a T2G error the GetTransferTask should return a non-empty string with error's description
                lT2GMock.Setup(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask)).Returns("");


                baselineStatusUpdater.ForceProgress(new TrainBaselineStatusExtendedData(BuildSampleTrainBaselineStatusData()));

                SystemInfo systemInfo = BuildSampleSystemInfo();
                baselineStatusUpdater.ProcessSystemChangedNotification(
                    systemInfo, _latestKnownCurrentBaselineVersion, _latestKnownFutureBaselineVersion);
                TrainBaselineStatusExtendedData statusUpdated;
                Assert.IsTrue(baselineStatusUpdater.TryGetEntry(systemInfo.SystemId, out statusUpdated), "System '{0}' is unknown by baseline status updater", systemInfo.SystemId);
                lUpdatedProgress = statusUpdated.Status;


                lT2GMock.Verify(x => x.GetTransferTask(
                    It.IsAny<int>(), out lRecipients, out lTask),
                    Times.Once());

                // If GetTransferTask doesn't return any error - 
                // the GetErrorCodeByDescription() should never be called
                lT2GMock.Verify(x => x.GetErrorCodeByDescription(It.IsAny<string>()),
                    Times.Never());

                // If GetTransferTask doesn't return any error - the request params shouldn't be changed          
                Assert.AreEqual(lBaselineStatusData.RequestId, lUpdatedProgress.RequestId);
                Assert.AreEqual(lBaselineStatusData.TaskId, lUpdatedProgress.TaskId);
                Assert.IsFalse(statusUpdated.IsT2GPollingRequired, "IsT2GPollingRequired wasn't updated as expected");
            }
        }

		#endregion
    }
}