//---------------------------------------------------------------------------------------------------
// <copyright file="TrainBaselineStatusExtendedDataTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PIS.Ground.Core.Data;
using PIS.Ground.DataPackage;

namespace DataPackageTests
{

    /// <summary>
    /// Class that perform test on TrainBaselineStatusExtendedData class.
    /// </summary>
    [TestFixture, Category("TrainBaselineStatusExtendedData"), Timeout(60 * 1000)]
    public class TrainBaselineStatusExtendedDataTests
    {
        #region Constants and readonly fields 
        public const string DefaultMissionName = "";
        public const int DefaultStatus = 0;
        public const CommunicationLink DefaultCommunicationLink = CommunicationLink.WIFI;
        public const string NoBaselineVersion = BaselineStatusUpdater.NoBaselineVersion;
        public static readonly PisMission DefaultMission = new PisMission();
        public static readonly ServiceInfoList EmptyServiceList = new ServiceInfoList();
        public static readonly PisBaseline DefaultBaseline = new PisBaseline();

        public static readonly Guid RequestId = new Guid("84D24621-F6E3-4EF1-9A0A-457639988577");
        public static readonly int TaskId = 200;
        public const string CurrentBaselineVersion = "5.0.0.0";
        public const string FutureBaselineversion = "7.0.0.0";

        public static readonly PisBaseline BaselineWithCurrentOnly = CreatePisBaseline(CurrentBaselineVersion, null, null);
        public static readonly PisBaseline BaselineWithFutureOnly = CreatePisBaseline(null, FutureBaselineversion, null);
        public static readonly PisBaseline BaselineWithCurrentAndFutureOnly = CreatePisBaseline(CurrentBaselineVersion, FutureBaselineversion, null);
        public static readonly PisVersion DefaultVersion = new PisVersion();

        public const string PisVersionString = "5.16.4.0";
        public static readonly PisVersion InitializedPisVersion = new PisVersion(PisVersionString);

        public const string TrainName_1 = "TRAIN-1";
        public const ushort TrainId_1 = 1;
        

        #endregion

        #region Constructor


        #endregion


        #region Test on update message


        #region Test update after create

        [Test, Category("UpdateAfterCreate")]
        public void TrainBaselineStatusExtendedData_UpdateAfterCreation_SystemOffline_WithoutT2GMessage()
        {
            TrainBaselineStatusExtendedData data = new TrainBaselineStatusExtendedData();

            SystemInfo system = CreateSystem(TrainName_1, TrainId_1, false);
            data.Update(system);

            TrainBaselineStatusData expectedStatusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, system.IsOnline, NoBaselineVersion, NoBaselineVersion, string.Empty, BaselineProgressStatusEnum.UNKNOWN);
            TrainBaselineStatusExtendedData expectedData = new TrainBaselineStatusExtendedData(expectedStatusData, null, NoBaselineVersion, null, false);
            Assert.AreEqual(expectedData, data, "Method TrainBaselineStatusExtendedData.Update didn't behave as expected");
        }

        [Test, Category("UpdateAfterCreate")]
        public void TrainBaselineStatusExtendedData_UpdateAfterCreation_SystemOffline_WithPisVersionSet()
        {
            TrainBaselineStatusExtendedData data = new TrainBaselineStatusExtendedData();

            SystemInfo system = CreateSystem(TrainName_1, TrainId_1, false, InitializedPisVersion);
            data.Update(system);

            TrainBaselineStatusData expectedStatusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, system.IsOnline, NoBaselineVersion, NoBaselineVersion, PisVersionString, BaselineProgressStatusEnum.UNKNOWN);
            TrainBaselineStatusExtendedData expectedData = new TrainBaselineStatusExtendedData(expectedStatusData, null, NoBaselineVersion, null, false);

            Assert.AreEqual(expectedData, data, "Method TrainBaselineStatusExtendedData.Update didn't behave as expected");
        }


        [Test, Category("UpdateAfterCreate")]
        public void TrainBaselineStatusExtendedData_UpdateAfterCreation_SystemOffline_WithPisVersionSetAndCurrentBaselineVersion()
        {
            TrainBaselineStatusExtendedData data = new TrainBaselineStatusExtendedData();

            SystemInfo system = CreateSystem(TrainName_1, TrainId_1, false, InitializedPisVersion, BaselineWithCurrentOnly);
            data.Update(system);

            TrainBaselineStatusData expectedStatusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, system.IsOnline, CurrentBaselineVersion, NoBaselineVersion, PisVersionString, BaselineProgressStatusEnum.UNKNOWN);
            TrainBaselineStatusExtendedData expectedData = new TrainBaselineStatusExtendedData(expectedStatusData, null, NoBaselineVersion, null, false);

            Assert.AreEqual(expectedData, data, "Method TrainBaselineStatusExtendedData.Update didn't behave as expected");
        }

        [Test, Category("UpdateAfterCreate")]
        public void TrainBaselineStatusExtendedData_UpdateAfterCreation_SystemOffline_WithPisVersionSetAndFutureBaselineVersion()
        {
            TrainBaselineStatusExtendedData data = new TrainBaselineStatusExtendedData();

            SystemInfo system = CreateSystem(TrainName_1, TrainId_1, false, InitializedPisVersion, BaselineWithFutureOnly);
            data.Update(system);

            TrainBaselineStatusData expectedStatusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, system.IsOnline, NoBaselineVersion, FutureBaselineversion, PisVersionString, BaselineProgressStatusEnum.UNKNOWN);
            TrainBaselineStatusExtendedData expectedData = new TrainBaselineStatusExtendedData(expectedStatusData, null, FutureBaselineversion, null, false);

            Assert.AreEqual(expectedData, data, "Method TrainBaselineStatusExtendedData.Update didn't behave as expected");
        }


        [Test, Category("UpdateAfterCreate")]
        public void TrainBaselineStatusExtendedData_UpdateAfterCreation_SystemOffline_WithPisVersion_CurrentAndFutureBaselineVersionSet()
        {
            TrainBaselineStatusExtendedData data = new TrainBaselineStatusExtendedData();

            SystemInfo system = CreateSystem(TrainName_1, TrainId_1, false, InitializedPisVersion, BaselineWithCurrentAndFutureOnly);
            data.Update(system);

            TrainBaselineStatusData expectedStatusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, system.IsOnline, CurrentBaselineVersion, FutureBaselineversion, PisVersionString, BaselineProgressStatusEnum.UNKNOWN);
            TrainBaselineStatusExtendedData expectedData = new TrainBaselineStatusExtendedData(expectedStatusData, null, FutureBaselineversion, null, false);

            Assert.AreEqual(expectedData, data, "Method TrainBaselineStatusExtendedData.Update didn't behave as expected");
        }

        [Test, Category("UpdateAfterCreate")]
        public void TrainBaselineStatusExtendedData_UpdateAfterCreation_SystemOnline_WithoutT2GMessage()
        {
            TrainBaselineStatusExtendedData data = new TrainBaselineStatusExtendedData();

            SystemInfo system = CreateSystem(TrainName_1, TrainId_1, false);
            data.Update(system);

            TrainBaselineStatusData expectedStatusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, system.IsOnline, NoBaselineVersion, NoBaselineVersion, string.Empty, BaselineProgressStatusEnum.UNKNOWN);
            TrainBaselineStatusExtendedData expectedData = new TrainBaselineStatusExtendedData(expectedStatusData, null, NoBaselineVersion, null, false);

            Assert.AreEqual(expectedData, data, "Method TrainBaselineStatusExtendedData.Update didn't behave as expected");
        }

        [Test, Category("UpdateAfterCreate")]
        public void TrainBaselineStatusExtendedData_UpdateAfterCreation_SystemOnline_WithPisVersionSet()
        {
            TrainBaselineStatusExtendedData data = new TrainBaselineStatusExtendedData();

            SystemInfo system = CreateSystem(TrainName_1, TrainId_1, false, InitializedPisVersion);
            data.Update(system);

            TrainBaselineStatusData expectedStatusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, system.IsOnline, NoBaselineVersion, NoBaselineVersion, PisVersionString, BaselineProgressStatusEnum.UNKNOWN);
            TrainBaselineStatusExtendedData expectedData = new TrainBaselineStatusExtendedData(expectedStatusData, null, NoBaselineVersion, null, false);

            Assert.AreEqual(expectedData, data, "Method TrainBaselineStatusExtendedData.Update didn't behave as expected");
        }

        [Test, Category("UpdateAfterCreate")]
        public void TrainBaselineStatusExtendedData_UpdateAfterCreation_SystemOnline_WithPisVersionSetAndCurrentBaselineVersion()
        {
            TrainBaselineStatusExtendedData data = new TrainBaselineStatusExtendedData();

            SystemInfo system = CreateSystem(TrainName_1, TrainId_1, true, InitializedPisVersion, BaselineWithCurrentOnly);
            data.Update(system);

            TrainBaselineStatusData expectedStatusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, system.IsOnline, CurrentBaselineVersion, NoBaselineVersion, PisVersionString, BaselineProgressStatusEnum.UNKNOWN);
            TrainBaselineStatusExtendedData expectedData = new TrainBaselineStatusExtendedData(expectedStatusData, null, NoBaselineVersion, null, false);

            Assert.AreEqual(expectedData, data, "Method TrainBaselineStatusExtendedData.Update didn't behave as expected");
        }

        [Test, Category("UpdateAfterCreate")]
        public void TrainBaselineStatusExtendedData_UpdateAfterCreation_SystemOnline_WithPisVersionSetAndFutureBaselineVersion()
        {
            TrainBaselineStatusExtendedData data = new TrainBaselineStatusExtendedData();

            SystemInfo system = CreateSystem(TrainName_1, TrainId_1, true, InitializedPisVersion, BaselineWithFutureOnly);
            data.Update(system);

            TrainBaselineStatusData expectedStatusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, system.IsOnline, NoBaselineVersion, FutureBaselineversion, PisVersionString, BaselineProgressStatusEnum.UNKNOWN);
            TrainBaselineStatusExtendedData expectedData = new TrainBaselineStatusExtendedData(expectedStatusData, null, FutureBaselineversion, null, false);

            Assert.AreEqual(expectedData, data, "Method TrainBaselineStatusExtendedData.Update didn't behave as expected");
        }

        [Test, Category("UpdateAfterCreate")]
        public void TrainBaselineStatusExtendedData_UpdateAfterCreation_SystemOnline_WithPisVersion_CurrentAndFutureBaselineVersionSet()
        {
            TrainBaselineStatusExtendedData data = new TrainBaselineStatusExtendedData();

            SystemInfo system = CreateSystem(TrainName_1, TrainId_1, true, InitializedPisVersion, BaselineWithCurrentAndFutureOnly);
            data.Update(system);

            TrainBaselineStatusData expectedStatusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, system.IsOnline, CurrentBaselineVersion, FutureBaselineversion, PisVersionString, BaselineProgressStatusEnum.UNKNOWN);
            TrainBaselineStatusExtendedData expectedData = new TrainBaselineStatusExtendedData(expectedStatusData, null, FutureBaselineversion, null, false);

            Assert.AreEqual(expectedData, data, "Method TrainBaselineStatusExtendedData.Update didn't behave as expected");
        }

        #endregion

        #region Test update with existing data

        [Test, Category("Update")]
        public void TrainBaselineStatusExtendedData_UpdateExisting_Offline_StatusUnknown_CurrentAndFutureVersionUnset_WithOfflineSystem_WithoutAnyT2GMessage()
        {
            TrainBaselineStatusData statusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, false, NoBaselineVersion, NoBaselineVersion, string.Empty, BaselineProgressStatusEnum.UNKNOWN);
            TrainBaselineStatusExtendedData data = new TrainBaselineStatusExtendedData(statusData);


            SystemInfo system = CreateSystem(TrainName_1, TrainId_1, false);
            data.Update(system);


            TrainBaselineStatusData expectedStatusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, system.IsOnline, NoBaselineVersion, NoBaselineVersion, string.Empty, BaselineProgressStatusEnum.UNKNOWN);
            TrainBaselineStatusExtendedData expectedData = new TrainBaselineStatusExtendedData(expectedStatusData, null, NoBaselineVersion, null, false);

            Assert.AreEqual(expectedData, data, "Method TrainBaselineStatusExtendedData.Update didn't behave as expected");
        }

        [Test, Category("Update")]
        public void TrainBaselineStatusExtendedData_UpdateExisting_Offline_StatusTransferPlanned_CurrentVersionUnsetAndFutureVersionSet_WithOfflineSystem_WithoutAnyT2GMessage()
        {
            TrainBaselineStatusData statusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, false, NoBaselineVersion, FutureBaselineversion, string.Empty, BaselineProgressStatusEnum.TRANSFER_PLANNED);
            statusData.RequestId = RequestId;
            statusData.TaskId = TaskId;
            TrainBaselineStatusExtendedData data = new TrainBaselineStatusExtendedData(statusData, FutureBaselineversion, NoBaselineVersion, NoBaselineVersion, true);


            SystemInfo system = CreateSystem(TrainName_1, TrainId_1, false);
            data.Update(system);


            TrainBaselineStatusData expectedStatusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, system.IsOnline, NoBaselineVersion, FutureBaselineversion, string.Empty, BaselineProgressStatusEnum.TRANSFER_PLANNED);
            expectedStatusData.RequestId = RequestId;
            expectedStatusData.TaskId = TaskId;
            TrainBaselineStatusExtendedData expectedData = new TrainBaselineStatusExtendedData(expectedStatusData, FutureBaselineversion, NoBaselineVersion, NoBaselineVersion, true);

            Assert.AreEqual(expectedData, data, "Method TrainBaselineStatusExtendedData.Update didn't behave as expected");
        }

        [Test, Category("Update")]
        public void TrainBaselineStatusExtendedData_UpdateExisting_Offline_StatusTransferPlanned_CurrentVersionAndFutureVersionUnset_AssignedFutureSet_WithOfflineSystem_WithoutAnyT2GMessage()
        {
            TrainBaselineStatusData statusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, false, NoBaselineVersion, NoBaselineVersion, string.Empty, BaselineProgressStatusEnum.TRANSFER_PLANNED);
            statusData.RequestId = RequestId;
            statusData.TaskId = TaskId;
            TrainBaselineStatusExtendedData data = new TrainBaselineStatusExtendedData(statusData, FutureBaselineversion, NoBaselineVersion, NoBaselineVersion, true);


            SystemInfo system = CreateSystem(TrainName_1, TrainId_1, false);
            data.Update(system);


            TrainBaselineStatusData expectedStatusData = new TrainBaselineStatusData(TrainName_1, TrainId_1, system.IsOnline, NoBaselineVersion, FutureBaselineversion, string.Empty, BaselineProgressStatusEnum.TRANSFER_PLANNED);
            expectedStatusData.RequestId = RequestId;
            expectedStatusData.TaskId = TaskId;
            TrainBaselineStatusExtendedData expectedData = new TrainBaselineStatusExtendedData(expectedStatusData, FutureBaselineversion, NoBaselineVersion, NoBaselineVersion, true);

            Assert.AreEqual(expectedData, data, "Method TrainBaselineStatusExtendedData.Update didn't behave as expected");
        }

        #endregion

        #endregion

        #region Test Management

        public static SystemInfo CreateSystem(string systemId, ushort vehicleId, bool isOnline)
        {
            return CreateSystem(systemId, vehicleId, isOnline, DefaultVersion);
        }

        public static SystemInfo CreateSystem(string systemId, ushort vehicleId, bool isOnline, PisVersion version)
        {
            return CreateSystem(systemId, vehicleId, isOnline, version, DefaultBaseline);
        }

        public static SystemInfo CreateSystem(string systemId, ushort vehicleId, bool isOnline, PisVersion version, PisBaseline baseline)
        {
            SystemInfo createdSystem = new SystemInfo(systemId, DefaultMissionName, vehicleId, DefaultStatus, isOnline, DefaultCommunicationLink, EmptyServiceList, baseline, version, DefaultMission, baseline.CurrentValidOut == Boolean.TrueString || baseline.FutureValidOut == Boolean.TrueString || baseline.ArchivedValidOut == Boolean.TrueString);
            return createdSystem;
        }

        public static PisBaseline CreatePisBaseline(string currentBaselineVersion, string futureBaselineVersion, string archiveBaselineVersion)
        {
            PisBaseline baseline = new PisBaseline();

            if (currentBaselineVersion != null)
            {
                baseline.CurrentVersionOut = currentBaselineVersion;
                baseline.CurrentVersionLmtOut = currentBaselineVersion;
                baseline.CurrentVersionPisBaseOut = currentBaselineVersion;
                baseline.CurrentVersionPisInfotainmentOut = currentBaselineVersion;
                baseline.CurrentVersionPisMissionOut = currentBaselineVersion;
                baseline.CurrentValidOut = string.IsNullOrEmpty(currentBaselineVersion) && currentBaselineVersion != NoBaselineVersion ? Boolean.TrueString : Boolean.FalseString;
            }

            if (futureBaselineVersion != null)
            {
                baseline.FutureVersionOut = futureBaselineVersion;
                baseline.FutureVersionLmtOut = futureBaselineVersion;
                baseline.FutureVersionPisBaseOut = futureBaselineVersion;
                baseline.FutureVersionPisInfotainmentOut = futureBaselineVersion;
                baseline.FutureVersionPisMissionOut = futureBaselineVersion;
                baseline.FutureValidOut = string.IsNullOrEmpty(futureBaselineVersion) && futureBaselineVersion != NoBaselineVersion ? Boolean.TrueString : Boolean.FalseString;
            }

            if (archiveBaselineVersion != null)
            {
                baseline.ArchivedVersionOut = archiveBaselineVersion;
                baseline.ArchivedVersionLmtOut = archiveBaselineVersion;
                baseline.ArchivedVersionPisBaseOut = archiveBaselineVersion;
                baseline.ArchivedVersionPisInfotainmentOut = archiveBaselineVersion;
                baseline.ArchivedVersionPisMissionOut = archiveBaselineVersion;
                baseline.ArchivedValidOut = string.IsNullOrEmpty(archiveBaselineVersion) && archiveBaselineVersion != NoBaselineVersion ? Boolean.TrueString : Boolean.FalseString;
            }

            return baseline;
        }

        #endregion

    }
}
