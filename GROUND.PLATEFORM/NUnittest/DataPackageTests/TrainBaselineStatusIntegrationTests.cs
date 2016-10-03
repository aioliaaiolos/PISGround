//---------------------------------------------------------------------------------------------------
// <copyright file="TrainBaselineStatusIntegrationTests.cs" company="Alstom">
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
using DataPackageTests.T2GServiceInterface.Identification;
using PIS.Ground.Core.LogMgmt;
using System.Globalization;
using PIS.Ground.GroundCore.AppGround;
using DataPackageTests.ServicesStub;

namespace DataPackageTests
{
    /// <summary>
    /// Class that tests baseline status management scenario by using logic that is very close to a real-system.
    /// 
    /// This class simulate T2G, services on train to perform a complete validation of expected status of every stages of a baseline distribution.
    /// This class also validate the history log database and the notifications send by PIS-Ground.
    /// </summary>
    /// <seealso cref="DataPackageTests.IntegrationTestsBase" />
    [TestFixture, Category("BaselineStatutScenario"), Timeout(1 * 60 * 1000)]
    class TrainBaselineStatusIntegrationTests : IntegrationTestsBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainBaselineStatusIntegrationTests"/> class.
        /// </summary>
        public TrainBaselineStatusIntegrationTests()
        {
            /* No logic body */
        }

        #endregion

        #region Startup initialization tests


        /// <summary>
        /// Verify that trains known by T2G are added into the train baselines status database when PIS ground connect with T2G.
        /// </summary>
        [Test, Category("Startup")]
        public void TrainBaselineStatusScenario_WhenConnectionIsEstablishedWithT2G_NewTrainsAreAddedToTrainBaselineStatus()
        {
            TrainBaselineStatusData expectedValue = new TrainBaselineStatusData(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, DEFAULT_BASELINE);
            Dictionary<string, TrainBaselineStatusData> expectedStatuses = new Dictionary<string, TrainBaselineStatusData>();
            expectedStatuses.Add(TRAIN_NAME_1, expectedValue);

            CreateT2GServicesStub();
            _hostIdentificationService.Close();
            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();
            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1, commLinkEnum._2G3G, false);
            InitializeDataPackageService(false);
            InitializePISGroundSession();

            _hostIdentificationService.Open();
            WaitPisGroundIsConnectedWithT2G();
            WaitNotificationSend(NotificationIdEnum.CommonT2GServerOnline);

            WaitTrainBaselineStatusesEquals(expectedStatuses, "Newly discovered train are not added as expected into the TrainBaselineStatus database when a connection with T2G is established");
        }


        /// <summary>
        /// Verify that trains unknown by T2G are removed from the train baselines status database when PIS ground connect with T2G.
        /// </summary>
        [Test, Category("Startup")]
        public void TrainBaselineStatusScenario_WhenConnectionIsEstablishedWithT2G_UnknownTrainsAreRemoved()
        {
            TrainBaselineStatusData expectedValueTrain1 = new TrainBaselineStatusData(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, DEFAULT_BASELINE);
            Dictionary<string, TrainBaselineStatusData> expectedStatuses = new Dictionary<string, TrainBaselineStatusData>();
            expectedStatuses.Add(expectedValueTrain1.TrainId, expectedValueTrain1);


            // Initialize the history log database

            UpdateHistoryLog(expectedValueTrain1);
            UpdateHistoryLog(new TrainBaselineStatusData("TRAIN-2", 2, false, "5.4.3.3"));
            UpdateHistoryLog(new TrainBaselineStatusData("TRAIN-0", 0, false, "1.0.0.0"));

            // Initialize services
            CreateT2GServicesStub();
            _hostIdentificationService.Close();

            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();
            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1, commLinkEnum._2G3G, false);
            InitializeDataPackageService(false);
            InitializePISGroundSession();
            _hostIdentificationService.Open();

            WaitPisGroundIsConnectedWithT2G();
            WaitNotificationSend(NotificationIdEnum.CommonT2GServerOnline);


            // Wait that history log was updated.
            WaitTrainBaselineStatusesEquals(expectedStatuses, "When PIS-Ground establish a connection with T2G, unknown train by T2G are not removed from the train baseline status database.");
        }

        /// <summary>
        /// Verify that online status of trains in train baseline status database are updated to be offline when PIS-Ground start
        /// </summary>
        [Test, Category("Startup")]
        public void TrainBaselineStatusScenario_OnStartupAllTrainAreOffline()
        {
            TrainBaselineStatusData valueTrain1 = new TrainBaselineStatusData(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, false, DEFAULT_BASELINE);
            TrainBaselineStatusData valueTrain2 = new TrainBaselineStatusData("TRAIN-2", 2, false, "5.4.3.3");
            TrainBaselineStatusData valueTrain3 = new TrainBaselineStatusData("TRAIN-0", 0, false, "1.0.0.0");
            Dictionary<string, TrainBaselineStatusData> expectedStatuses = new Dictionary<string, TrainBaselineStatusData>();
            expectedStatuses.Add(valueTrain1.TrainId, valueTrain1.Clone());
            expectedStatuses.Add(valueTrain2.TrainId, valueTrain2.Clone());
            expectedStatuses.Add(valueTrain3.TrainId, valueTrain3.Clone());


            // Initialize the history log database
            valueTrain1.OnlineStatus = true;
            valueTrain3.OnlineStatus = true;
            UpdateHistoryLog(valueTrain1);
            UpdateHistoryLog(valueTrain2);
            UpdateHistoryLog(valueTrain3);

            // Initialize services
            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();
            InitializeDataPackageService(false);
            InitializePISGroundSession();

            // Wait that history log was updated.
            WaitTrainBaselineStatusesEquals(expectedStatuses, "On startup of PIS-Ground, train baselines status are not set to state offline");
        }

        /// <summary>
        /// Verify that online status of trains are updated in train baseline status database when PIS-Ground connect with T2G.
        /// </summary>
        [Test, Category("Startup")]
        public void TrainBaselineStatusScenario_OnlineStatusOfTrainBaselineUpdateWhenCommunicationIsEstablishedWithT2G()
        {
            TrainBaselineStatusData valueTrain1 = new TrainBaselineStatusData(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, DEFAULT_BASELINE);
            TrainBaselineStatusData valueTrain2 = new TrainBaselineStatusData(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, "5.4.3.3");
            TrainBaselineStatusData valueTrain3 = new TrainBaselineStatusData(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, "1.0.0.0");
            Dictionary<string, TrainBaselineStatusData> expectedStatuses = new Dictionary<string, TrainBaselineStatusData>();
            expectedStatuses.Add(valueTrain1.TrainId, valueTrain1.Clone());
            expectedStatuses.Add(valueTrain2.TrainId, valueTrain2.Clone());
            expectedStatuses.Add(valueTrain3.TrainId, valueTrain3.Clone());


            // Initialize the history log database
            valueTrain1.OnlineStatus = false;
            valueTrain3.OnlineStatus = false;
            UpdateHistoryLog(valueTrain1);
            UpdateHistoryLog(valueTrain2);
            UpdateHistoryLog(valueTrain3);

            // Initialize services
            CreateT2GServicesStub();
            _hostIdentificationService.Close();

            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();
            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1, commLinkEnum._2G3G, true);
            InitializeTrain(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, TRAIN_IP_2, TRAIN_DATA_PACKAGE_PORT_2, commLinkEnum._2G3G, false);
            InitializeTrain(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, TRAIN_IP_0, TRAIN_DATA_PACKAGE_PORT_0, commLinkEnum.wifi, false);

            InitializeDataPackageService(false);
            InitializePISGroundSession();
            _hostIdentificationService.Open();
            WaitPisGroundIsConnectedWithT2G();
            WaitNotificationSend(NotificationIdEnum.CommonT2GServerOnline);

            // Wait that history log was updated.
            WaitTrainBaselineStatusesEquals(expectedStatuses, "Online statuses of train not updated in train baseline status when PIS-Ground connect with T2G");
        }

        #endregion

        #region System Changed notifications

        /// <summary>
        /// Scenario that verify that OnSystemChanged notification from T2G update the online status of train baseline status database.
        /// </summary>
        [Test, Category("SystemChangedNotifications")]
        public void TrainBaselineStatusScenario_OnSystemChangedNotificationUpdateOnlineStatus()
        {
            TrainBaselineStatusData valueTrain1 = new TrainBaselineStatusData(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, DEFAULT_BASELINE);
            TrainBaselineStatusData valueTrain2 = new TrainBaselineStatusData(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, "5.4.3.3");
            TrainBaselineStatusData valueTrain3 = new TrainBaselineStatusData(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, "1.0.0.0");
            Dictionary<string, TrainBaselineStatusData> expectedStatuses = new Dictionary<string, TrainBaselineStatusData>();
            expectedStatuses.Add(valueTrain1.TrainId, valueTrain1.Clone());
            expectedStatuses.Add(valueTrain2.TrainId, valueTrain2.Clone());
            expectedStatuses.Add(valueTrain3.TrainId, valueTrain3.Clone());


            // Initialize the history log database
            valueTrain1.OnlineStatus = true;
            valueTrain3.OnlineStatus = true;
            UpdateHistoryLog(valueTrain1);
            UpdateHistoryLog(valueTrain2);
            UpdateHistoryLog(valueTrain3);

            // Initialize services
            CreateT2GServicesStub();

            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();
            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1, commLinkEnum._2G3G, true);
            InitializeTrain(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, TRAIN_IP_2, TRAIN_DATA_PACKAGE_PORT_2, commLinkEnum._2G3G, false);
            InitializeTrain(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, TRAIN_IP_0, TRAIN_DATA_PACKAGE_PORT_0, commLinkEnum.wifi, false);

            InitializeDataPackageService(false);
            InitializePISGroundSession();

            // Wait that history log was on expected status.
            WaitTrainBaselineStatusesEquals(expectedStatuses, "Online statuses of train not updated in train baseline status when PIS-Ground connect with T2G");


            // TRAIN-1 become offline
            valueTrain1.OnlineStatus = false;
            expectedStatuses[valueTrain1.TrainId] = valueTrain1.Clone();
            expectedStatuses[valueTrain2.TrainId] = valueTrain2.Clone();
            expectedStatuses[valueTrain3.TrainId] = valueTrain3.Clone();

            _identificationServiceStub.UpdateSystem(valueTrain1.TrainId, Convert.ToInt32(valueTrain1.TrainNumber, CultureInfo.InvariantCulture), valueTrain1.OnlineStatus, 0, DEFAULT_MISSION, commLinkEnum.wifi, TRAIN_IP_1);

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnSystemChanged does not update the online status in train baseline statuses database.");

            // TRAIN-2 become offline
            // TRAIN-1 become online
            valueTrain1.OnlineStatus = true;
            valueTrain2.OnlineStatus = false;
            expectedStatuses[valueTrain1.TrainId] = valueTrain1.Clone();
            expectedStatuses[valueTrain2.TrainId] = valueTrain2.Clone();
            expectedStatuses[valueTrain3.TrainId] = valueTrain3.Clone();

            _identificationServiceStub.UpdateSystem(valueTrain1.TrainId, Convert.ToInt32(valueTrain1.TrainNumber, CultureInfo.InvariantCulture), valueTrain1.OnlineStatus, 0, DEFAULT_MISSION, commLinkEnum.wifi, TRAIN_IP_1);
            _identificationServiceStub.UpdateSystem(valueTrain2.TrainId, Convert.ToInt32(valueTrain2.TrainNumber, CultureInfo.InvariantCulture), valueTrain2.OnlineStatus, 0, DEFAULT_MISSION, commLinkEnum.wifi, TRAIN_IP_2);

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnSystemChanged does not update the online status in train baseline statuses database.");
        }

        /// <summary>
        /// Scenario that verify that OnSystemChanged notification from T2G update the train number field of train baseline status database.
        /// </summary>
        [Test, Category("SystemChangedNotifications")]
        public void TrainBaselineStatusScenario_OnSystemChangedNotificationUpdateTrainNumber()
        {
            TrainBaselineStatusData valueTrain1 = new TrainBaselineStatusData(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, DEFAULT_BASELINE);
            TrainBaselineStatusData valueTrain2 = new TrainBaselineStatusData(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, "5.4.3.3");
            TrainBaselineStatusData valueTrain3 = new TrainBaselineStatusData(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, "1.0.0.0");
            Dictionary<string, TrainBaselineStatusData> expectedStatuses = new Dictionary<string, TrainBaselineStatusData>();
            expectedStatuses.Add(valueTrain1.TrainId, valueTrain1.Clone());
            expectedStatuses.Add(valueTrain2.TrainId, valueTrain2.Clone());
            expectedStatuses.Add(valueTrain3.TrainId, valueTrain3.Clone());


            // Initialize the history log database
            valueTrain1.OnlineStatus = true;
            valueTrain3.OnlineStatus = true;
            UpdateHistoryLog(valueTrain1);
            UpdateHistoryLog(valueTrain2);
            UpdateHistoryLog(valueTrain3);

            // Initialize services
            CreateT2GServicesStub();

            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();
            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1, commLinkEnum._2G3G, true);
            InitializeTrain(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, TRAIN_IP_2, TRAIN_DATA_PACKAGE_PORT_2, commLinkEnum._2G3G, false);
            InitializeTrain(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, TRAIN_IP_0, TRAIN_DATA_PACKAGE_PORT_0, commLinkEnum.wifi, false);

            InitializeDataPackageService(false);
            InitializePISGroundSession();

            // Wait that history log was on expected status.
            WaitTrainBaselineStatusesEquals(expectedStatuses, "Online statuses of train not updated in train baseline status when PIS-Ground connect with T2G");


            // TRAIN-1 become train number 100
            valueTrain1.TrainNumber = "100";
            expectedStatuses[valueTrain1.TrainId] = valueTrain1.Clone();
            expectedStatuses[valueTrain2.TrainId] = valueTrain2.Clone();
            expectedStatuses[valueTrain3.TrainId] = valueTrain3.Clone();

            _identificationServiceStub.UpdateSystem(valueTrain1.TrainId, Convert.ToInt32(valueTrain1.TrainNumber, CultureInfo.InvariantCulture), valueTrain1.OnlineStatus, 0, DEFAULT_MISSION, commLinkEnum.wifi, TRAIN_IP_1);

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnSystemChanged does not update the train number field in train baseline statuses database.");

            // TRAIN-2 become train number 200
            // TRAIN-1 become train number 1000
            valueTrain1.TrainNumber = "1000";
            valueTrain2.TrainNumber = "200";
            expectedStatuses[valueTrain1.TrainId] = valueTrain1.Clone();
            expectedStatuses[valueTrain2.TrainId] = valueTrain2.Clone();
            expectedStatuses[valueTrain3.TrainId] = valueTrain3.Clone();

            _identificationServiceStub.UpdateSystem(valueTrain1.TrainId, Convert.ToInt32(valueTrain1.TrainNumber, CultureInfo.InvariantCulture), valueTrain1.OnlineStatus, 0, DEFAULT_MISSION, commLinkEnum.wifi, TRAIN_IP_1);
            _identificationServiceStub.UpdateSystem(valueTrain2.TrainId, Convert.ToInt32(valueTrain2.TrainNumber, CultureInfo.InvariantCulture), valueTrain2.OnlineStatus, 0, DEFAULT_MISSION, commLinkEnum.wifi, TRAIN_IP_2);

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnSystemChanged does not update the train number field in train baseline statuses database.");
        }

        /// <summary>
        /// Scenario that verify that OnSystemChanged notification from T2G update the train number field of train baseline status database.
        /// </summary>
        [Test, Category("SystemChangedNotifications")]
        public void TrainBaselineStatusScenario_OnSystemChangedNotificationAddNewTrain()
        {
            TrainBaselineStatusData valueTrain1 = new TrainBaselineStatusData(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, DEFAULT_BASELINE);
            TrainBaselineStatusData valueTrain2 = new TrainBaselineStatusData(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, "5.4.3.3");
            TrainBaselineStatusData valueTrain3 = new TrainBaselineStatusData(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, "1.0.0.0");
            Dictionary<string, TrainBaselineStatusData> expectedStatuses = new Dictionary<string, TrainBaselineStatusData>();


            // Initialize services
            CreateT2GServicesStub();

            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();

            InitializeDataPackageService(false);
            InitializePISGroundSession();

            // Wait that history log was on expected status.
            WaitTrainBaselineStatusesEquals(expectedStatuses, "Online statuses of train not updated in train baseline status when PIS-Ground connect with T2G");


            // TRAIN-1 become train online
            expectedStatuses.Add(valueTrain1.TrainId, valueTrain1.Clone());
            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1, commLinkEnum._2G3G, true);

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnSystemChanged does not update the train number field in train baseline statuses database.");

            // TRAIN-0 added
            expectedStatuses.Add(valueTrain3.TrainId, valueTrain3.Clone());
            InitializeTrain(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, TRAIN_IP_0, TRAIN_DATA_PACKAGE_PORT_0, commLinkEnum.wifi, false);

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnSystemChanged does not update the train number field in train baseline statuses database.");

            // TRAIN-2 added
            expectedStatuses.Add(valueTrain2.TrainId, valueTrain2.Clone());
            InitializeTrain(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, TRAIN_IP_2, TRAIN_DATA_PACKAGE_PORT_2, commLinkEnum._2G3G, false);

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnSystemChanged does not update the train number field in train baseline statuses database.");
        }


        #endregion

        #region System Deleted Notification

        /// <summary>
        /// Scenario that verify that OnSystemDeleted notification from T2G generate the proper notification and update train baseline status database accordingly
        /// </summary>
        [Test, Category("SystemDeletedNotifications")]
        public void TrainBaselineStatusScenario_OnSystemDeletedNotificationNominal()
        {
            TrainBaselineStatusData valueTrain1 = new TrainBaselineStatusData(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, DEFAULT_BASELINE);
            TrainBaselineStatusData valueTrain2 = new TrainBaselineStatusData(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, "5.4.3.3");
            TrainBaselineStatusData valueTrain3 = new TrainBaselineStatusData(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, "1.0.0.0");
            Dictionary<string, TrainBaselineStatusData> expectedStatuses = new Dictionary<string, TrainBaselineStatusData>();
            expectedStatuses.Add(valueTrain1.TrainId, valueTrain1.Clone());
            expectedStatuses.Add(valueTrain2.TrainId, valueTrain2.Clone());
            expectedStatuses.Add(valueTrain3.TrainId, valueTrain3.Clone());


            // Initialize the history log database
            valueTrain1.OnlineStatus = true;
            valueTrain3.OnlineStatus = true;
            UpdateHistoryLog(valueTrain1);
            UpdateHistoryLog(valueTrain2);
            UpdateHistoryLog(valueTrain3);

            // Initialize services
            CreateT2GServicesStub();

            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();
            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1, commLinkEnum._2G3G, true);
            InitializeTrain(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, TRAIN_IP_2, TRAIN_DATA_PACKAGE_PORT_2, commLinkEnum._2G3G, false);
            InitializeTrain(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, TRAIN_IP_0, TRAIN_DATA_PACKAGE_PORT_0, commLinkEnum.wifi, false);

            InitializeDataPackageService(false);
            InitializePISGroundSession();

            // Wait that history log was on expected status.
            WaitTrainBaselineStatusesEquals(expectedStatuses, "Online statuses of train not updated in train baseline status when PIS-Ground connect with T2G");


            // Remove TRAIN-0
            expectedStatuses.Remove(valueTrain3.TrainId);
            _identificationServiceStub.DeleteSystem(valueTrain3.TrainId);

            WaitNotificationSend(NotificationIdEnum.DeletedElement, valueTrain3.TrainId);
            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnSystemDeleted does not remove the deleted train in train baseline statuses database.");

            // Delete TRAIN-2
            expectedStatuses.Remove(valueTrain2.TrainId);
            _identificationServiceStub.DeleteSystem(valueTrain2.TrainId);

            WaitNotificationSend(NotificationIdEnum.DeletedElement, valueTrain2.TrainId);
            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnSystemDeleted does not remove the deleted train in train baseline statuses database.");

            // Delete TRAIN-1
            expectedStatuses.Remove(valueTrain1.TrainId);
            _identificationServiceStub.DeleteSystem(valueTrain1.TrainId);

            WaitNotificationSend(NotificationIdEnum.DeletedElement, valueTrain1.TrainId);
            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnSystemDeleted does not remove the deleted train in train baseline statuses database.");
        }

        /// <summary>
        /// Scenario that verify that system can be added again after an on system delete notification
        /// </summary>
        [Test, Category("SystemDeletedNotifications")]
        public void TrainBaselineStatusScenario_OnSystemAddedWorkAfterSystemWasPreviouslyDeleted()
        {
            TrainBaselineStatusData valueTrain1 = new TrainBaselineStatusData(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, DEFAULT_BASELINE);
            TrainBaselineStatusData valueTrain2 = new TrainBaselineStatusData(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, "5.4.3.3");
            TrainBaselineStatusData valueTrain3 = new TrainBaselineStatusData(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, "1.0.0.0");
            Dictionary<string, TrainBaselineStatusData> expectedStatuses = new Dictionary<string, TrainBaselineStatusData>();
            expectedStatuses.Add(valueTrain1.TrainId, valueTrain1.Clone());
            expectedStatuses.Add(valueTrain2.TrainId, valueTrain2.Clone());
            expectedStatuses.Add(valueTrain3.TrainId, valueTrain3.Clone());


            // Initialize the history log database
            valueTrain1.OnlineStatus = true;
            valueTrain3.OnlineStatus = true;
            UpdateHistoryLog(valueTrain1);
            UpdateHistoryLog(valueTrain2);
            UpdateHistoryLog(valueTrain3);

            // Initialize services
            CreateT2GServicesStub();

            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();
            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1, commLinkEnum._2G3G, true);
            InitializeTrain(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, TRAIN_IP_2, TRAIN_DATA_PACKAGE_PORT_2, commLinkEnum._2G3G, false);
            InitializeTrain(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, TRAIN_IP_0, TRAIN_DATA_PACKAGE_PORT_0, commLinkEnum.wifi, false);

            InitializeDataPackageService(false);
            InitializePISGroundSession();

            // Wait that history log was on expected status.
            WaitTrainBaselineStatusesEquals(expectedStatuses, "Online statuses of train not updated in train baseline status when PIS-Ground connect with T2G");


            // Remove TRAIN-0 and TRAIN-2
            expectedStatuses.Remove(valueTrain3.TrainId);
            expectedStatuses.Remove(valueTrain2.TrainId);
            _identificationServiceStub.DeleteSystem(valueTrain3.TrainId);
            _identificationServiceStub.DeleteSystem(valueTrain2.TrainId);

            WaitNotificationSend(NotificationIdEnum.DeletedElement, valueTrain3.TrainId);
            WaitNotificationSend(NotificationIdEnum.DeletedElement, valueTrain2.TrainId);
            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnSystemDeleted does not remove the deleted train in train baseline statuses database.");

            // TRAIN-2 become online again
            InitializeTrain(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, TRAIN_IP_2, TRAIN_DATA_PACKAGE_PORT_2, commLinkEnum._2G3G, false);
            expectedStatuses.Add(valueTrain2.TrainId, valueTrain2);

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnSystemChanged does not add train after it has been deleted.");
        }

        #endregion

        #region OnMessageChanged - PIS Software version

        /// <summary>
        /// Scenario that verify that OnMessageChanged notification from T2G update the pis software version in the train baseline status database.
        /// </summary>
        [Test, Category("MessageChangedNotifications")]
        public void TrainBaselineStatusScenario_OnMessageChangedUpdateThePisSoftwareVersion()
        {
            TrainBaselineStatusData valueTrain1 = new TrainBaselineStatusData(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, DEFAULT_BASELINE);
            TrainBaselineStatusData valueTrain2 = new TrainBaselineStatusData(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, "5.4.3.3");
            TrainBaselineStatusData valueTrain3 = new TrainBaselineStatusData(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, "1.0.0.0");
            Dictionary<string, TrainBaselineStatusData> expectedStatuses = new Dictionary<string, TrainBaselineStatusData>();
            expectedStatuses.Add(valueTrain1.TrainId, valueTrain1.Clone());
            expectedStatuses.Add(valueTrain2.TrainId, valueTrain2.Clone());
            expectedStatuses.Add(valueTrain3.TrainId, valueTrain3.Clone());

            // Initialize services
            CreateT2GServicesStub();

            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();

            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1, commLinkEnum._2G3G, true);
            InitializeTrain(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, TRAIN_IP_0, TRAIN_DATA_PACKAGE_PORT_0, commLinkEnum.wifi, false);
            InitializeTrain(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, TRAIN_IP_2, TRAIN_DATA_PACKAGE_PORT_2, commLinkEnum._2G3G, false);

            InitializeDataPackageService(false);
            InitializePISGroundSession();

            // Wait that history log was on expected status.
            WaitTrainBaselineStatusesEquals(expectedStatuses, "Online statuses of train not updated in train baseline status when PIS-Ground connect with T2G");


            // Update pis.version message of TRAIN-0
            valueTrain3.PisOnBoardVersion = "5.16.3.2";
            expectedStatuses[valueTrain1.TrainId] = valueTrain1.Clone();
            expectedStatuses[valueTrain2.TrainId] = valueTrain2.Clone();
            expectedStatuses[valueTrain3.TrainId] = valueTrain3.Clone();
            _vehicleInfoServiceStub.UpdateMessageData(new VersionMessage(valueTrain3.TrainId, valueTrain3.PisOnBoardVersion));

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnMessageChanged does not update the pis onboard software version in train baseline statuses database.");

            // Update pis.version message of TRAIN-1
            valueTrain1.PisOnBoardVersion = "5.18.0.0";
            expectedStatuses[valueTrain1.TrainId] = valueTrain1.Clone();
            expectedStatuses[valueTrain2.TrainId] = valueTrain2.Clone();
            expectedStatuses[valueTrain3.TrainId] = valueTrain3.Clone();
            _vehicleInfoServiceStub.UpdateMessageData(new VersionMessage(valueTrain1.TrainId, valueTrain1.PisOnBoardVersion));

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnMessageChanged does not update the pis onboard software version in train baseline statuses database.");

            // Update pis.version message of TRAIN-2
            valueTrain2.PisOnBoardVersion = "5.20.0.0";
            expectedStatuses[valueTrain1.TrainId] = valueTrain1.Clone();
            expectedStatuses[valueTrain2.TrainId] = valueTrain2.Clone();
            expectedStatuses[valueTrain3.TrainId] = valueTrain3.Clone();
            _vehicleInfoServiceStub.UpdateMessageData(new VersionMessage(valueTrain2.TrainId, valueTrain2.PisOnBoardVersion));

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnMessageChanged does not update the pis onboard software version in train baseline statuses database.");
        }



        #endregion

        #region OnMessageChanged - Baseline message

        /// <summary>
        /// Scenario that verify that OnMessageChanged notification from T2G update current and future baseline version in the train baseline status database.
        /// </summary>
        [Test, Category("MessageChangedNotifications")]
        public void TrainBaselineStatusScenario_OnMessageChangedUpdateTheBaselineInfo()
        {
            TrainBaselineStatusData valueTrain1 = new TrainBaselineStatusData(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, DEFAULT_BASELINE);
            TrainBaselineStatusData valueTrain2 = new TrainBaselineStatusData(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, "5.4.3.3");
            TrainBaselineStatusData valueTrain3 = new TrainBaselineStatusData(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, "1.0.0.0");
            Dictionary<string, TrainBaselineStatusData> expectedStatuses = new Dictionary<string, TrainBaselineStatusData>();
            expectedStatuses.Add(valueTrain1.TrainId, valueTrain1.Clone());
            expectedStatuses.Add(valueTrain2.TrainId, valueTrain2.Clone());
            expectedStatuses.Add(valueTrain3.TrainId, valueTrain3.Clone());

            // Initialize services
            CreateT2GServicesStub();

            _dataStoreServiceStub.InitializeRemoteDataStoreMockWithDefaultBehavior();

            InitializeTrain(TRAIN_NAME_1, TRAIN_VEHICLE_ID_1, true, TRAIN_IP_1, TRAIN_DATA_PACKAGE_PORT_1, commLinkEnum._2G3G, true);
            InitializeTrain(TRAIN_NAME_0, TRAIN_VEHICLE_ID_0, false, TRAIN_IP_0, TRAIN_DATA_PACKAGE_PORT_0, commLinkEnum.wifi, false);
            InitializeTrain(TRAIN_NAME_2, TRAIN_VEHICLE_ID_2, true, TRAIN_IP_2, TRAIN_DATA_PACKAGE_PORT_2, commLinkEnum._2G3G, false);

            InitializeDataPackageService(false);
            InitializePISGroundSession();

            // Wait that history log was on expected status.
            WaitTrainBaselineStatusesEquals(expectedStatuses, "Online statuses of train not updated in train baseline status when PIS-Ground connect with T2G");


            // Update current baseline message of TRAIN-0
            valueTrain3.CurrentBaselineVersion = "8.8.8.8";
            expectedStatuses[valueTrain1.TrainId] = valueTrain1.Clone();
            expectedStatuses[valueTrain2.TrainId] = valueTrain2.Clone();
            expectedStatuses[valueTrain3.TrainId] = valueTrain3.Clone();
            BaselineMessage baseline = new BaselineMessage(valueTrain3.TrainId);
            baseline.CurrentVersion = valueTrain3.CurrentBaselineVersion;
            baseline.FutureVersion = valueTrain3.FutureBaselineVersion;
            _vehicleInfoServiceStub.UpdateMessageData(baseline);

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnMessageChanged does not update the current baseline version in train baseline statuses database.");

            // Update future baseline pis.version message of TRAIN-1
            valueTrain1.FutureBaselineVersion = "7.7.7.7";
            expectedStatuses[valueTrain1.TrainId] = valueTrain1.Clone();
            expectedStatuses[valueTrain2.TrainId] = valueTrain2.Clone();
            expectedStatuses[valueTrain3.TrainId] = valueTrain3.Clone();
            baseline = new BaselineMessage(valueTrain1.TrainId);
            baseline.CurrentVersion = valueTrain1.CurrentBaselineVersion;
            baseline.FutureVersion = valueTrain1.FutureBaselineVersion;
            _vehicleInfoServiceStub.UpdateMessageData(baseline);

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnMessageChanged does not update the future baseline version in train baseline statuses database.");

            // Update current and future baseline version of TRAIN-2
            valueTrain2.FutureBaselineVersion = "9.8.7.6";
            valueTrain2.CurrentBaselineVersion = "5.4.3.2";
            expectedStatuses[valueTrain1.TrainId] = valueTrain1.Clone();
            expectedStatuses[valueTrain2.TrainId] = valueTrain2.Clone();
            expectedStatuses[valueTrain3.TrainId] = valueTrain3.Clone();
            baseline = new BaselineMessage(valueTrain2.TrainId);
            baseline.CurrentVersion = valueTrain2.CurrentBaselineVersion;
            baseline.FutureVersion = valueTrain2.FutureBaselineVersion;
            _vehicleInfoServiceStub.UpdateMessageData(baseline);

            WaitTrainBaselineStatusesEquals(expectedStatuses, "T2G - OnMessageChanged does not update current and future baseline version in train baseline statuses database.");
        }



        #endregion
    }
}
