//---------------------------------------------------------------------------------------------------
// <copyright file="LocalDataStorageTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.T2G;
using System.Linq;

namespace GroundCoreTests
{
    /// <summary>LocalDataStorageTests test class.</summary>
    [TestFixture, Category("LocalDataStorageTest")]
    public class LocalDataStorageTests
    {
        #region attributes

        #endregion

        #region Tests managment

        /// <summary>Initializes a new instance of the LocalDataStorageTests class.</summary>
        public LocalDataStorageTests()
        {
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

        #region	RemoveSystem

        /// <summary>Searches for the first pending transfer tasks.</summary>
        [Test]
        public void RemoveSystem()
        {
            T2GLocalDataStorage ds = new T2GLocalDataStorage(new T2GSessionData(), false);

            string lTrainId = "TRAIN-1";

            PIS.Ground.Core.Data.SystemInfo lSystemInfo = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId,
                "",
                1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            ds.OnSystemChanged(lSystemInfo);

            Assert.IsNotNull(ds.GetSystem(lTrainId));

            ds.RemoveSystem(lSystemInfo.SystemId);

            Assert.IsNull(ds.GetSystem(lTrainId));
        }

        /// <summary>Check add function does work correctly to add or not a new SystemInfo or an existing one.</summary>
        [Test]
        public void AddSystemTest()
        {
            T2GLocalDataStorage ds = new T2GLocalDataStorage(new T2GSessionData(), false);

            //Test 1 : Add new SystemInfo in the list
            string lTrainId1 = "SYSTEM-1";

            PIS.Ground.Core.Data.SystemInfo lSystemInfo1 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId1,
                "",
                1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            ds.OnSystemChanged(lSystemInfo1);


            //Test 2 : Add a new SystemInfo in the list
            string lTrainId2 = "SYSTEM-2";
            PIS.Ground.Core.Data.SystemInfo lSystemInfo2 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId2,
                "",
                2,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            ds.OnSystemChanged(lSystemInfo2);

            //Test 2 : Add a new SystemInfo that is exactly the same as one already in the system list
            PIS.Ground.Core.Data.SystemInfo lSystemInfo3 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId1,
                "",
                1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            ds.OnSystemChanged(lSystemInfo3);

            //Test 2 : Add a new SystemInfo that has the same SystemID as one in the list but has different information.
            PIS.Ground.Core.Data.SystemInfo lSystemInfo4 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId1,
                "",
                3,
                0,
                false,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            ds.OnSystemChanged(lSystemInfo4);

            SystemInfo newSys1 = ds.GetSystem(lTrainId1);
            SystemInfo newSys2 = ds.GetSystem(lTrainId2);

            Assert.IsNotNull(newSys1);
            Assert.IsNotNull(newSys2);
            Assert.IsFalse(newSys1.IsOnline);
            Assert.IsTrue(newSys2.IsOnline);
        }

        /// <summary>Check if function does work correctly when updating one element in a larger list of system</summary>
        [Test]
        public void AddSystemRobustTest()
        {
            string lTrainId = "TRAIN-";
            int lTrainIdIndex;
            int nbSystem = 1000;
            int nbServices = 50;

            T2GLocalDataStorage ds = new T2GLocalDataStorage(new T2GSessionData(), false);

            //Fill system list with SystemInfo that have servicesInfo and fieldInfo
            List<SystemInfo> lTmpSystem = new List<SystemInfo>();

            for (lTrainIdIndex = 1; lTrainIdIndex <= nbSystem; lTrainIdIndex++)
            {
                ServiceInfo[] serviceList = new ServiceInfo[nbServices];

                for (int i = 0; i < nbServices; i++)
                {
                    serviceList[i] = new ServiceInfo(1, "test" + lTrainIdIndex.ToString(), 1, 1, true, "test", "test", "test", 1);
                }

                SystemInfo lSystemInfo = new SystemInfo(
                    lTrainId + lTrainIdIndex.ToString(),
                    "",
                    (ushort)lTrainIdIndex,
                    0,
                    true,
                    PIS.Ground.Core.Data.CommunicationLink.WIFI,
                    new ServiceInfoList(serviceList),
                    new PisBaseline(),
                    new PisVersion(),
                    new PisMission(),
                    false);

                ds.OnSystemChanged(lSystemInfo);

                //Create a clone of SystemList so you can just recreate the behavior of UpdateSystemList
                lTmpSystem.Add(lSystemInfo);
            }

            //Create a instance of systemInfo that is already in the list but with different information.

            ServiceInfo[] newServiceList = new ServiceInfo[nbServices];

            for (int i = 0; i < nbServices; i++)
            {
                newServiceList[i] = new ServiceInfo(1, "test" + "TRAIN-99", 1, 1, true, "test", "test", "test", 1);
            }

            PIS.Ground.Core.Data.SystemInfo lSystemInfo2 = new PIS.Ground.Core.Data.SystemInfo(
                "TRAIN-99",
                "",
                99,
                0,
                false,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(newServiceList),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            //Modify the cloned systemList for a specific item
            lTmpSystem.RemoveAll(item => item.SystemId == "TRAIN-99");
            lTmpSystem.Add(lSystemInfo2);

            //Check time used to update the list
            var watch = Stopwatch.StartNew();

            foreach (PIS.Ground.Core.Data.SystemInfo lSystem in lTmpSystem)
            {
                ds.OnSystemChanged(lSystem);
            }

            SystemInfo newSys = ds.GetSystem("TRAIN-99");
            Assert.IsNotNull(newSys);
            Assert.IsFalse(newSys.IsOnline);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Time elapsed : " + elapsedMs.ToString());
        }

        private void FillSystemListWithTestData(T2GLocalDataStorage ds, int nbSystem, int nbServices)
        {
            int lTrainIdIndex;
            string lTrainId = "TRAIN-";

            for (lTrainIdIndex = 1; lTrainIdIndex <= nbSystem; lTrainIdIndex++)
            {
                ServiceInfo[] serviceList = new ServiceInfo[nbServices];

                for (int i = 0; i < nbServices; i++)
                {
                    serviceList[i] = new ServiceInfo(1, "test" + "TRAIN-99", 1, 1, true, "test", "test", "test", 1);
                }

                SystemInfo lSystemInfo = new SystemInfo(
                    lTrainId + lTrainIdIndex.ToString(),
                    "",
                    1,
                    0,
                    true,
                    PIS.Ground.Core.Data.CommunicationLink.WIFI,
                    new ServiceInfoList(serviceList),
                    new PisBaseline(),
                    new PisVersion(),
                    new PisMission(),
                    false);

                ds.OnSystemChanged(lSystemInfo);
            }
        }

        /// <summary>Test Deep copy of SystemList execution time VS GetSystemData execution time</summary>
        [Test]
        public void GetSystemDataTest()
        {
            T2GLocalDataStorage ds = new T2GLocalDataStorage(new T2GSessionData(), false);

            List<SystemInfo> lTmpList;

            //Check time used perform Deep Copy of the system list
            Console.WriteLine("Creation of deep copy of system list");

            //Fill the system list
            FillSystemListWithTestData(ds, 500, 50);

            //Get the systemList
            var watch = Stopwatch.StartNew();

            lTmpList = ds.GetSystemList();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Time elapsed : " + elapsedMs.ToString() + "ms");

            //Check time used to get a specific systemInfo item in the system list
            Console.WriteLine("Get a specific item in the list");

            SystemInfo lTmpSystem;

            watch = Stopwatch.StartNew();

            lTmpSystem = ds.GetSystem("TRAIN-400");

            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Time elapsed : " + elapsedMs.ToString() + "ms");
            Console.WriteLine("System got : " + lTmpSystem.SystemId);
        }

        /// <summary>Test Deep copy of SystemList execution time VS IsElementOnline execution time</summary>
        [Test]
        public void IsElementOnlineTest()
        {
            T2GLocalDataStorage ds = new T2GLocalDataStorage(new T2GSessionData(), false);

            List<SystemInfo> lTmpList;

            //Check time used perform Deep Copy of the system list
            Console.WriteLine("Creation of deep copy of system list");

            //Fill the system list
            FillSystemListWithTestData(ds, 500, 50);

            //Get the systemList
            var watch = Stopwatch.StartNew();

            lTmpList = ds.GetSystemList();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Time elapsed : " + elapsedMs.ToString() + "ms");

            //Check time used to get the connexion status of a specific system
            Console.WriteLine("Get the connexion status of a specific system");

            bool lOnline = false;

            watch = Stopwatch.StartNew();

            lOnline = ds.IsElementOnline("TRAIN-400");

            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Time elapsed : " + elapsedMs.ToString() + "ms");
            Console.WriteLine("Train is online ? " + lOnline.ToString());
        }

        /// <summary>Test Deep copy of SystemList execution time VS DoesElementExist execution time</summary>
        [Test]
        public void DoesElementExistsTest()
        {
            T2GLocalDataStorage ds = new T2GLocalDataStorage(new T2GSessionData(), false);

            List<SystemInfo> lTmpList;

            //Check time used perform Deep Copy of the system list
            Console.WriteLine("Creation of deep copy of system list");

            //Fill the system list
            FillSystemListWithTestData(ds, 500, 50);

            //Get the systemList
            var watch = Stopwatch.StartNew();

            lTmpList = ds.GetSystemList();

            watch.Stop();
            var getListElapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("GetSystemList elapsed time : " + getListElapsedMs.ToString() + "ms");

            //Check time used to check if an element exists in the system list
            Console.WriteLine("Check if an element exists in the system list");

            watch = Stopwatch.StartNew();

            bool lElementExists = ds.ElementExists("TRAIN-400");

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("ElementExists elapsed time : " + elapsedMs.ToString() + "ms");
            Console.WriteLine("Element exists ? " + lElementExists.ToString());
            Assert.IsTrue(lElementExists, "Method ElementExists didn't returned the expected value");
            Assert.That(elapsedMs, Is.LessThanOrEqualTo(getListElapsedMs), "ElementExists is slower than GetSystemList");
        }

        #endregion

        #region Update Service list tests

        /// <summary>
        /// Method that test method UpdateServiceList of LocalDataStorage in these conditions:
        /// <list type="number">
        /// <item>Local data storage manage one system</item>
        /// <item>The number of service provided as parameter is one.</item>
        /// </list>
        /// </summary>
        [Test, Category("UpdateServiceList")]
        public void UpdateServiceListWithOfOneSystemWithOneServicePerUpdate_FilterLocalServiceDisable()
        {
            T2GLocalDataStorage ds = new T2GLocalDataStorage(new T2GSessionData(), false);

            //Test 1 : Add new SystemInfo in the list
            string lTrainId1 = "TRAIN-1";
            ushort lVehicleIdTrainId1 = 1;

            ServiceInfo service1 = new ServiceInfo(1, "Ha Ha", lVehicleIdTrainId1, 3, true, "10.0.0.1", "AID", "SID", 1038);
            ServiceInfo service2 = new ServiceInfo(2, "Drole", lVehicleIdTrainId1, 3, true, "10.0.0.1", "AID", "SID", 1039);

            PIS.Ground.Core.Data.SystemInfo lSystemInfo1 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            ////////////////////////
            // Register the train in the system
            ////////////////////////
            ds.OnSystemChanged(lSystemInfo1);
            Assert.IsTrue(ds.IsElementOnline(lTrainId1), "The system '{0}' is not online in data store", lTrainId1);
            Assert.AreEqual(lSystemInfo1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);

            ////////////////////////
            // Simulate the notification of one service that is available
            ////////////////////////
            ServiceInfoList serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { service1 });

            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, true, 1000, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            SystemInfo expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                serviceUpdateList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(serviceUpdateList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Another notification with same data
            ////////////////////////
            Assert.IsFalse(ds.OnServiceChanged(lTrainId1, true, 1000, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                serviceUpdateList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(serviceUpdateList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Another notification with update
            ////////////////////////
            service1 = new ServiceInfo(1, "Ha dd", lVehicleIdTrainId1, 3, false, "10.0.0.1", "AID", "SID", 1038);
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { service1 });
            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, true, 1000, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                serviceUpdateList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(serviceUpdateList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Another notification with empty list
            ////////////////////////
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { });
            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, true, 1000, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                serviceUpdateList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(serviceUpdateList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);
        }

        /// <summary>
        /// Method that test method UpdateServiceList of LocalDataStorage in these conditions:
        /// <list type="number">
        /// <item>Local data storage manage one system</item>
        /// <item>Local data storage filter local service</item>
        /// <item>The number of service provided as parameter is one.</item>
        /// </list>
        /// </summary>
        [Test, Category("UpdateServiceList")]
        public void UpdateServiceListWithOfOneSystemWithOneServicePerUpdate_FilterLocalServiceEnabled()
        {
            T2GLocalDataStorage ds = new T2GLocalDataStorage(new T2GSessionData(), true);

            //Test 1 : Add new SystemInfo in the list
            string lTrainId1 = "TRAIN-1";
            ushort lVehicleIdTrainId1 = 1;
            ushort lVehicleIdTrainId2 = 2;

            ServiceInfo service1 = new ServiceInfo(1, "Ha Ha", lVehicleIdTrainId1, 3, true, "10.0.0.1", "AID", "SID", 1038);
            ServiceInfo service2 = new ServiceInfo(1, "Ha Ha(other)", lVehicleIdTrainId2, 3, true, "10.0.2.1", "AID", "SID", 1038);

            PIS.Ground.Core.Data.SystemInfo lSystemInfo1 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            ////////////////////////
            // Register the train in the system
            ////////////////////////
            ds.OnSystemChanged(lSystemInfo1);
            Assert.IsTrue(ds.IsElementOnline(lTrainId1), "The system '{0}' is not online in data store", lTrainId1);
            Assert.AreEqual(lSystemInfo1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);

            ////////////////////////
            // Simulate the notification of one service that is available
            ////////////////////////
            ServiceInfoList serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { service1 });

            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, true, 1000, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            SystemInfo expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                serviceUpdateList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(serviceUpdateList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Another notification with same data
            ////////////////////////
            Assert.IsFalse(ds.OnServiceChanged(lTrainId1, true, 1000, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                serviceUpdateList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(serviceUpdateList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Another notification with wrong train id
            ////////////////////////
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { service2 });
            Assert.IsFalse(ds.OnServiceChanged(lTrainId1, true, 1000, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { service1 });

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                serviceUpdateList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(serviceUpdateList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Another notification with update
            ////////////////////////
            service1 = new ServiceInfo(1, "Ha dd", lVehicleIdTrainId1, 3, false, "10.0.0.1", "AID", "SID", 1038);
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { service1 });
            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, true, 1000, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                serviceUpdateList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(serviceUpdateList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Another notification with empty list
            ////////////////////////
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { });
            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, true, 1000, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                serviceUpdateList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(serviceUpdateList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);
        }

        /// <summary>
        /// Method that test method UpdateServiceList of LocalDataStorage in these conditions:
        /// <list type="number">
        /// <item>Local data storage manage one system</item>
        /// <item>The number of service provided as parameter is one.</item>
        /// </list>
        /// </summary>
        [Test, Category("UpdateServiceList")]
        public void UpdateServiceListWithOfOneSystemWithMultipleServicePerUpdate_FilterLocalServiceDisabled()
        {
            T2GLocalDataStorage ds = new T2GLocalDataStorage(new T2GSessionData(), false);

            //Test 1 : Add new SystemInfo in the list
            string lTrainId1 = "TRAIN-1";
            ushort lVehicleIdTrainId1 = 1;
            ushort lVehicleIdTrainId2 = 2;

            int lSubscriptionId1 = 1001;
            int lSubscriptionId2 = 1002;

            ServiceInfo service1_1 = new ServiceInfo(1, "Ha Ha", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 1038);
            ServiceInfo service1_2 = new ServiceInfo(1, "Ha Ha", lVehicleIdTrainId2, 10, true, "10.0.0.2", "AID", "SID", 1038);
            ServiceInfo service1_3 = new ServiceInfo(1, "Ha Ha", lVehicleIdTrainId1, 10, false, "10.0.1.1", "AID", "SID", 1038);

            ServiceInfo service2_1 = new ServiceInfo(2, "CAM", lVehicleIdTrainId1, 10, false, "10.0.0.1", "AID", "SID", 1020);
            ServiceInfo service2_2 = new ServiceInfo(2, "CAM", lVehicleIdTrainId2, 10, true, "10.0.0.2", "AID", "SID", 1020);
            ServiceInfo service2_3 = new ServiceInfo(2, "CAM", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 800);

            PIS.Ground.Core.Data.SystemInfo lSystemInfo1 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            ////////////////////////
            // Register the train in the system
            ////////////////////////
            ds.OnSystemChanged(lSystemInfo1);
            Assert.IsTrue(ds.IsElementOnline(lTrainId1), "The system '{0}' is not online in data store", lTrainId1);
            Assert.AreEqual(lSystemInfo1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);

            ////////////////////////
            // Simulate the notification of one service
            ////////////////////////
            ServiceInfoList serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { service1_1, service1_2, service1_3 });
            ServiceInfoList expectedServiceList = new ServiceInfoList(new ServiceInfo[] { service1_1, service1_2, service1_3 });

            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, true, lSubscriptionId1, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            SystemInfo expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Another notification with same data
            ////////////////////////
            Assert.IsFalse(ds.OnServiceChanged(lTrainId1, true, lSubscriptionId1, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Simulate the notifications of one another service
            ////////////////////////
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { service2_1, service2_2, service2_3 });
            expectedServiceList = new ServiceInfoList(new ServiceInfo[] { service1_1, service1_2, service2_3, service2_2, service1_3, service2_1 });

            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, true, lSubscriptionId2, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Simulate the update of one service of one another service
            ////////////////////////
            service1_3 = new ServiceInfo(1, "Ha Ha", lVehicleIdTrainId1, 10, true, "10.0.1.1", "AID", "SID", 1038);
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { service1_3 });

            expectedServiceList = new ServiceInfoList(new ServiceInfo[] { service1_1, service1_3, service1_2, service2_3, service2_2, service2_1 });

            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, true, lSubscriptionId1, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Simulate train offline
            ////////////////////////
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { });

            expectedServiceList = new ServiceInfoList(new ServiceInfo[] { service2_3, service2_2, service2_1 });

            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, false, lSubscriptionId1, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                false,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Simulate train offline with unknow subscription id
            ////////////////////////
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { });

            expectedServiceList = new ServiceInfoList(new ServiceInfo[] { service2_3, service2_2, service2_1 });

            Assert.IsFalse(ds.OnServiceChanged(lTrainId1, false, lSubscriptionId1 + 10, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                false,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Simulate train offline
            ////////////////////////
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { });

            expectedServiceList = new ServiceInfoList(new ServiceInfo[] { });

            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, false, lSubscriptionId2, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                false,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Simulate train offline
            ////////////////////////
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { });

            expectedServiceList = new ServiceInfoList(new ServiceInfo[] { });

            Assert.IsFalse(ds.OnServiceChanged(lTrainId1, false, lSubscriptionId2, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                false,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Simulate train online
            ////////////////////////
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { });

            expectedServiceList = new ServiceInfoList(new ServiceInfo[] { });

            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, true, lSubscriptionId2, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);
        }

        /// <summary>
        /// Method that test method UpdateServiceList of LocalDataStorage in these conditions:
        /// <list type="number">
        /// <item>Local data storage manage one system</item>
        /// <item>Local data storage filter local service</item>
        /// <item>The number of service provided as parameter is one.</item>
        /// </list>
        /// </summary>
        [Test, Category("UpdateServiceList")]
        public void UpdateServiceListWithOfOneSystemWithMultipleServicePerUpdate_FilterLocalServiceEnabled()
        {
            T2GLocalDataStorage ds = new T2GLocalDataStorage(new T2GSessionData(), true);

            //Test 1 : Add new SystemInfo in the list
            string lTrainId1 = "TRAIN-1";
            ushort lVehicleIdTrainId1 = 1;
            ushort lVehicleIdTrainId2 = 2;

            int lSubscriptionId1 = 1001;
            int lSubscriptionId2 = 1002;

            ServiceInfo service1_1 = new ServiceInfo(1, "Ha Ha", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 1038);
            ServiceInfo service1_2 = new ServiceInfo(1, "Ha Ha", lVehicleIdTrainId2, 10, true, "10.0.0.2", "AID", "SID", 1038);
            ServiceInfo service1_3 = new ServiceInfo(1, "Ha Ha", lVehicleIdTrainId1, 10, false, "10.0.1.1", "AID", "SID", 1038);

            ServiceInfo service2_1 = new ServiceInfo(2, "CAM", lVehicleIdTrainId1, 10, false, "10.0.0.1", "AID", "SID", 1020);
            ServiceInfo service2_2 = new ServiceInfo(2, "CAM", lVehicleIdTrainId2, 10, true, "10.0.0.2", "AID", "SID", 1020);
            ServiceInfo service2_3 = new ServiceInfo(2, "CAM", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 800);

            PIS.Ground.Core.Data.SystemInfo lSystemInfo1 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            ////////////////////////
            // Register the train in the system
            ////////////////////////
            ds.OnSystemChanged(lSystemInfo1);
            Assert.IsTrue(ds.IsElementOnline(lTrainId1), "The system '{0}' is not online in data store", lTrainId1);
            Assert.AreEqual(lSystemInfo1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);

            ////////////////////////
            // Simulate the notification of one service
            ////////////////////////
            ServiceInfoList serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { service1_1, service1_2, service1_3 });
            ServiceInfoList expectedServiceList = new ServiceInfoList(new ServiceInfo[] { service1_1, service1_3 });

            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, true, lSubscriptionId1, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            SystemInfo expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Another notification with same data
            ////////////////////////
            Assert.IsFalse(ds.OnServiceChanged(lTrainId1, true, lSubscriptionId1, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Simulate the notifications of one another service
            ////////////////////////
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { service2_1, service2_2, service2_3 });
            expectedServiceList = new ServiceInfoList(new ServiceInfo[] { service1_1, service2_3, service1_3, service2_1 });

            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, true, lSubscriptionId2, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Simulate the update of one service of one another service
            ////////////////////////
            service1_3 = new ServiceInfo(1, "Ha Ha", lVehicleIdTrainId1, 10, true, "10.0.1.1", "AID", "SID", 1038);
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { service1_3 });

            expectedServiceList = new ServiceInfoList(new ServiceInfo[] { service1_1, service1_3, service2_3, service2_1 });

            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, true, lSubscriptionId1, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Simulate train offline
            ////////////////////////
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { });

            expectedServiceList = new ServiceInfoList(new ServiceInfo[] { service2_3, service2_1 });

            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, false, lSubscriptionId1, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                false,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);

            ////////////////////////
            // Simulate train offline
            ////////////////////////
            serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { });

            expectedServiceList = new ServiceInfoList(new ServiceInfo[] { });

            Assert.IsTrue(ds.OnServiceChanged(lTrainId1, false, lSubscriptionId2, serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}'", lTrainId1);

            expectedSystemInfo = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                false,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo, ds.GetSystem(lTrainId1), "The method OnServiceChanged does not update properly the system '{0}'", lTrainId1);
            Assert.AreEqual(expectedServiceList, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData(trainid) does not return the proper value for system '{0}'", lTrainId1);
        }

        #endregion

        #region GetAvailableServiceData

        /// <summary>
        /// Method that test method UpdateServiceList of LocalDataStorage in these conditions:
        /// <list type="number">
        /// <item>Local data storage manage multiple systems.</item>
        /// <item>Local data storage does not filter local service</item>
        /// </list>
        /// </summary>
        [Test, Category("GetAvailableServiceData")]
        public void GetAvailableServiceData_MultipleServices_FilterLocalServiceDisabled()
        {
            T2GLocalDataStorage ds = new T2GLocalDataStorage(new T2GSessionData(), false);

            //Test 1 : Add new SystemInfo in the list
            string lTrainId1 = "TRAIN-1";
            string lTrainId2 = "TRAIN-2";
            string lTrainId3 = "TRAIN-3";
            string lTrainId4 = "TRAIN-4";
            ushort lVehicleIdTrainId1 = 1;
            ushort lVehicleIdTrainId2 = 2;
            ushort lVehicleIdTrainId3 = 3;
            ushort lVehicleIdTrainId4 = 4;

            int lSubscriptionId1 = 1001;
            int lSubscriptionId2 = 1002;
            int lSubscriptionId3 = 1003;

            int[] subscriptionIds = { 0, lSubscriptionId1, lSubscriptionId2, lSubscriptionId3 };

            ServiceInfo service1_T1_1 = new ServiceInfo(1, "CCTV", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 1038);
            ServiceInfo service1_T1_2 = new ServiceInfo(1, "CCTV", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 1037);
            ServiceInfo service1_T2_1_O = new ServiceInfo(1, "CCTV", lVehicleIdTrainId2, 10, false, "10.0.2.1", "AID", "SID", 1038);
            ServiceInfo service1_T3_1_O = new ServiceInfo(1, "CCTV", lVehicleIdTrainId3, 10, false, "10.0.3.1", "AID", "SID", 1038);
            ServiceInfo service1_T3_2 = new ServiceInfo(1, "CCTV", lVehicleIdTrainId3, 10, true, "10.0.3.2", "AID", "SID", 1038);
            ServiceInfo service1_T3_3 = new ServiceInfo(1, "CCTV", lVehicleIdTrainId3, 10, true, "10.0.3.3", "AID", "SID", 1038);
            ServiceInfo service1_T4_1_O = new ServiceInfo(1, "CCTV", lVehicleIdTrainId4, 10, false, "10.0.4.1", "AID", "SID", 1038);
            ServiceInfo service1_T4_2 = new ServiceInfo(1, "CCTV", lVehicleIdTrainId4, 10, true, "10.0.4.2", "AID", "SID", 1038);

            ServiceInfo service2_T1_1 = new ServiceInfo(2, "CAM", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 1020);
            ServiceInfo service2_T2_1 = new ServiceInfo(2, "CAM", lVehicleIdTrainId2, 10, true, "10.0.2.2", "AID", "SID", 1020);
            ServiceInfo service2_T3_1_O
                = new ServiceInfo(2, "CAM", lVehicleIdTrainId3, 10, false, "10.0.3.1", "AID", "SID", 1020);
            ServiceInfo service2_T4_1 = new ServiceInfo(2, "CAM", lVehicleIdTrainId4, 10, true, "10.0.4.1", "AID", "SID", 1020);

            ServiceInfo service3_T1_1 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 1020);
            ServiceInfo service3_T1_2 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 1021);
            ServiceInfo service3_T2_1 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId2, 10, true, "10.0.2.2", "AID", "SID", 1020);
            ServiceInfo service3_T2_2 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId2, 10, true, "10.0.2.2", "AID", "SID", 1021);
            ServiceInfo service3_T3_1 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId3, 10, true, "10.0.3.1", "AID", "SID", 1020);
            ServiceInfo service3_T3_2 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId3, 10, true, "10.0.3.2", "AID", "SID", 1020);
            ServiceInfo service3_T4_1 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId4, 10, true, "10.0.4.1", "AID", "SID", 1020);
            ServiceInfo service3_T4_2 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId4, 10, true, "10.0.4.2", "AID", "SID", 1020);

            ServiceInfoList expectedServiceList_t1 = new ServiceInfoList(new ServiceInfo[] { 
                service1_T1_2, service1_T1_1, service1_T3_2, service1_T3_3, service1_T4_2, 
                 service2_T1_1,service2_T2_1, service2_T4_1, service3_T1_1, service3_T1_2,
                 service3_T2_1, service3_T2_2, service3_T3_1, service3_T3_2, service3_T4_1, 
                 service3_T4_2, service1_T2_1_O, service1_T3_1_O, service1_T4_1_O, service2_T3_1_O});

            ServiceInfoList expectedServiceList_t2 = new ServiceInfoList(new ServiceInfo[] { 
                service1_T1_2, service1_T1_1, service1_T3_2, service1_T3_3, service1_T4_2, 
                 service2_T2_1, service2_T1_1, service2_T4_1, service3_T2_1, service3_T2_2, 
                 service3_T1_1, service3_T1_2, service3_T3_1, service3_T3_2, service3_T4_1, 
                 service3_T4_2, service1_T2_1_O, service1_T3_1_O, service1_T4_1_O, service2_T3_1_O});

            ServiceInfoList expectedServiceList_t3 = new ServiceInfoList(new ServiceInfo[] { 
                service1_T3_2, service1_T3_3, service1_T1_2, service1_T1_1, service1_T4_2, 
                 service2_T1_1,service2_T2_1, service2_T4_1, service3_T3_1, service3_T3_2, 
                 service3_T1_1, service3_T1_2, service3_T2_1, service3_T2_2, service3_T4_1, 
                 service3_T4_2, service1_T3_1_O, service1_T2_1_O, service1_T4_1_O, service2_T3_1_O});

            ServiceInfoList expectedServiceList_t4 = new ServiceInfoList(new ServiceInfo[] { 
                service1_T4_2, service1_T1_2, service1_T1_1, service1_T3_2, service1_T3_3, 
                 service2_T4_1, service2_T1_1,service2_T2_1, service3_T4_1, service3_T4_2, 
                 service3_T1_1, service3_T1_2, service3_T2_1, service3_T2_2, service3_T3_1,
                 service3_T3_2, service1_T4_1_O, service1_T2_1_O, service1_T3_1_O, service2_T3_1_O});

            ////////////////////////
            // Register the trains in the system
            ////////////////////////
            PIS.Ground.Core.Data.SystemInfo expectedSystemInfo_T1 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);
            PIS.Ground.Core.Data.SystemInfo expectedSystemInfo_T2 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId2,
                "",
                lVehicleIdTrainId2,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);
            PIS.Ground.Core.Data.SystemInfo expectedSystemInfo_T3 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId3,
                "",
                lVehicleIdTrainId3,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);
            PIS.Ground.Core.Data.SystemInfo expectedSystemInfo_T4 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId4,
                "",
                lVehicleIdTrainId4,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            ds.OnSystemChanged(expectedSystemInfo_T1);
            Assert.IsTrue(ds.IsElementOnline(lTrainId1), "The system '{0}' is not online in data store", lTrainId1);
            Assert.AreEqual(expectedSystemInfo_T1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);

            ds.OnSystemChanged(expectedSystemInfo_T2);
            Assert.IsTrue(ds.IsElementOnline(lTrainId2), "The system '{0}' is not online in data store", lTrainId2);
            Assert.AreEqual(expectedSystemInfo_T2, ds.GetSystem(lTrainId2), "The system '{0}' was not set to expected value in data store", lTrainId2);

            ds.OnSystemChanged(expectedSystemInfo_T3);
            Assert.IsTrue(ds.IsElementOnline(lTrainId3), "The system '{0}' is not online in data store", lTrainId3);
            Assert.AreEqual(expectedSystemInfo_T3, ds.GetSystem(lTrainId3), "The system '{0}' was not set to expected value in data store", lTrainId3);

            ds.OnSystemChanged(expectedSystemInfo_T4);
            Assert.IsTrue(ds.IsElementOnline(lTrainId4), "The system '{0}' is not online in data store", lTrainId4);
            Assert.AreEqual(expectedSystemInfo_T4, ds.GetSystem(lTrainId4), "The system '{0}' was not set to expected value in data store", lTrainId4);

            ////////////////////////
            // Update service list of TRAIN-1
            ////////////////////////

            for (int i = expectedServiceList_t1.Count - 1; i >= 0; --i)
            {
                ServiceInfo currentService = expectedServiceList_t1[i];
                string currentTrainId = lTrainId1;
                ServiceInfoList serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { currentService });

                Assert.IsTrue(ds.OnServiceChanged(currentTrainId, true, subscriptionIds[currentService.ServiceId], serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}' for service: <{1}>", currentTrainId, currentService);
            }

            expectedSystemInfo_T1 = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList_t1,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo_T1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);
            Assert.AreEqual(expectedSystemInfo_T2, ds.GetSystem(lTrainId2), "The system '{0}' was not set to expected value in data store", lTrainId2);
            Assert.AreEqual(expectedSystemInfo_T3, ds.GetSystem(lTrainId3), "The system '{0}' was not set to expected value in data store", lTrainId3);
            Assert.AreEqual(expectedSystemInfo_T4, ds.GetSystem(lTrainId4), "The system '{0}' was not set to expected value in data store", lTrainId4);

            ////////////////////////
            // Update service list of TRAIN-2
            ////////////////////////

            for (int i = expectedServiceList_t2.Count - 1; i >= 0; --i)
            {
                ServiceInfo currentService = expectedServiceList_t2[i];
                string currentTrainId = lTrainId2;
                ServiceInfoList serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { currentService });

                Assert.IsTrue(ds.OnServiceChanged(currentTrainId, true, subscriptionIds[currentService.ServiceId], serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}' for service: <{1}>", currentTrainId, currentService);
            }

            expectedSystemInfo_T2 = new SystemInfo(
                lTrainId2,
                "",
                lVehicleIdTrainId2,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList_t2,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo_T1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);
            Assert.AreEqual(expectedSystemInfo_T2, ds.GetSystem(lTrainId2), "The system '{0}' was not set to expected value in data store", lTrainId2);
            Assert.AreEqual(expectedSystemInfo_T3, ds.GetSystem(lTrainId3), "The system '{0}' was not set to expected value in data store", lTrainId3);
            Assert.AreEqual(expectedSystemInfo_T4, ds.GetSystem(lTrainId4), "The system '{0}' was not set to expected value in data store", lTrainId4);

            ////////////////////////
            // Update service list of TRAIN-3
            ////////////////////////

            for (int i = 0; i < expectedServiceList_t3.Count; ++i)
            {
                ServiceInfo currentService = expectedServiceList_t3[i];
                string currentTrainId = lTrainId3;
                ServiceInfoList serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { currentService });

                Assert.IsTrue(ds.OnServiceChanged(currentTrainId, true, subscriptionIds[currentService.ServiceId], serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}' for service: <{1}>", currentTrainId, currentService);
            }

            expectedSystemInfo_T3 = new SystemInfo(
                lTrainId3,
                "",
                lVehicleIdTrainId3,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList_t3,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo_T1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);
            Assert.AreEqual(expectedSystemInfo_T2, ds.GetSystem(lTrainId2), "The system '{0}' was not set to expected value in data store", lTrainId2);
            Assert.AreEqual(expectedSystemInfo_T3, ds.GetSystem(lTrainId3), "The system '{0}' was not set to expected value in data store", lTrainId3);
            Assert.AreEqual(expectedSystemInfo_T4, ds.GetSystem(lTrainId4), "The system '{0}' was not set to expected value in data store", lTrainId4);

            ////////////////////////
            // Update service list of TRAIN-4
            ////////////////////////

            for (int i = 0; i < expectedServiceList_t4.Count; ++i)
            {
                ServiceInfo currentService = expectedServiceList_t4[i];
                string currentTrainId = lTrainId4;
                ServiceInfoList serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { currentService });

                Assert.IsTrue(ds.OnServiceChanged(currentTrainId, true, subscriptionIds[currentService.ServiceId], serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}' for service: <{1}>", currentTrainId, currentService);
            }

            expectedSystemInfo_T4 = new SystemInfo(
                lTrainId4,
                "",
                lVehicleIdTrainId4,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList_t4,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo_T1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);
            Assert.AreEqual(expectedSystemInfo_T2, ds.GetSystem(lTrainId2), "The system '{0}' was not set to expected value in data store", lTrainId2);
            Assert.AreEqual(expectedSystemInfo_T3, ds.GetSystem(lTrainId3), "The system '{0}' was not set to expected value in data store", lTrainId3);
            Assert.AreEqual(expectedSystemInfo_T4, ds.GetSystem(lTrainId4), "The system '{0}' was not set to expected value in data store", lTrainId4);

            ////////////////////////
            // Verify GetAvailableServiceData on system
            ////////////////////////
            Assert.AreEqual(expectedServiceList_t1, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData('{0}') does not return the expected value", lTrainId1);
            Assert.AreEqual(expectedServiceList_t2, ds.GetAvailableServiceData(lTrainId2), "The method GetAvailableServiceData('{0}') does not return the expected value", lTrainId2);
            Assert.AreEqual(expectedServiceList_t3, ds.GetAvailableServiceData(lTrainId3), "The method GetAvailableServiceData('{0}') does not return the expected value", lTrainId3);
            Assert.AreEqual(expectedServiceList_t4, ds.GetAvailableServiceData(lTrainId4), "The method GetAvailableServiceData('{0}') does not return the expected value", lTrainId4);
            Assert.AreEqual(new ServiceInfoList(), ds.GetAvailableServiceData(lTrainId4 + "SSS"), "The method GetAvailableServiceData('{0}') does not return the expected value", lTrainId4 + "SSS");

            ////////////////////////
            // Verify GetAvailableServiceData on system and service
            ////////////////////////
            string[] trainIds = { lTrainId1, lTrainId2, lTrainId3, lTrainId4, "NonExistingTrain" };
            ServiceInfo[][] expectedResults = new ServiceInfo[][]{
                                                   new ServiceInfo[]{ null, service1_T1_2, service2_T1_1, service3_T1_1 },
                                                   new ServiceInfo[]{ null, service1_T1_2, service2_T2_1, service3_T2_1 },
                                                   new ServiceInfo[]{ null, service1_T3_2, service2_T1_1, service3_T3_1 },
                                                   new ServiceInfo[]{ null, service1_T4_2, service2_T4_1, service3_T4_1 },
                                                   new ServiceInfo[]{ null, null, null, null },

            };

            for (int trainIndex = 0; trainIndex < trainIds.Length; ++trainIndex)
            {
                string trainId = trainIds[trainIndex];
                for (int serviceIndex = 0; serviceIndex <= 3; ++serviceIndex)
                {
                    ServiceInfo expectedService = expectedResults[trainIndex][serviceIndex];
                    Assert.AreEqual(expectedService, ds.GetAvailableServiceData(trainId, serviceIndex), "The method GetAvailableServiceData('{0}', {1}) does not return the expected value", trainId, serviceIndex);
                }
            }
        }

        /// <summary>
        /// Method that test method UpdateServiceList of LocalDataStorage in these conditions:
        /// <list type="number">
        /// <item>Local data storage manage multiple systems.</item>
        /// <item>Local data storage does not filter local service</item>
        /// </list>
        /// </summary>
        [Test, Category("GetAvailableServiceData")]
        public void GetAvailableServiceData_MultipleServices_FilterLocalServiceEnabled()
        {
            T2GLocalDataStorage ds = new T2GLocalDataStorage(new T2GSessionData(), true);

            //Test 1 : Add new SystemInfo in the list
            string lTrainId1 = "TRAIN-1";
            string lTrainId2 = "TRAIN-2";
            string lTrainId3 = "TRAIN-3";
            string lTrainId4 = "TRAIN-4";
            ushort lVehicleIdTrainId1 = 1;
            ushort lVehicleIdTrainId2 = 2;
            ushort lVehicleIdTrainId3 = 3;
            ushort lVehicleIdTrainId4 = 4;

            int lSubscriptionId1 = 1001;
            int lSubscriptionId2 = 1002;
            int lSubscriptionId3 = 1003;

            int[] subscriptionIds = { 0, lSubscriptionId1, lSubscriptionId2, lSubscriptionId3 };

            ServiceInfo service1_T1_1 = new ServiceInfo(1, "CCTV", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 1038);
            ServiceInfo service1_T1_2 = new ServiceInfo(1, "CCTV", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 1037);
            ServiceInfo service1_T2_1_O = new ServiceInfo(1, "CCTV", lVehicleIdTrainId2, 10, false, "10.0.2.1", "AID", "SID", 1038);
            ServiceInfo service1_T3_1_O = new ServiceInfo(1, "CCTV", lVehicleIdTrainId3, 10, false, "10.0.3.1", "AID", "SID", 1038);
            ServiceInfo service1_T3_2 = new ServiceInfo(1, "CCTV", lVehicleIdTrainId3, 10, true, "10.0.3.2", "AID", "SID", 1038);
            ServiceInfo service1_T3_3 = new ServiceInfo(1, "CCTV", lVehicleIdTrainId3, 10, true, "10.0.3.3", "AID", "SID", 1038);
            ServiceInfo service1_T4_1_O = new ServiceInfo(1, "CCTV", lVehicleIdTrainId4, 10, false, "10.0.4.1", "AID", "SID", 1038);
            ServiceInfo service1_T4_2 = new ServiceInfo(1, "CCTV", lVehicleIdTrainId4, 10, true, "10.0.4.2", "AID", "SID", 1038);

            ServiceInfo service2_T1_1 = new ServiceInfo(2, "CAM", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 1020);
            ServiceInfo service2_T2_1 = new ServiceInfo(2, "CAM", lVehicleIdTrainId2, 10, true, "10.0.2.2", "AID", "SID", 1020);
            ServiceInfo service2_T3_1_O
                = new ServiceInfo(2, "CAM", lVehicleIdTrainId3, 10, false, "10.0.3.1", "AID", "SID", 1020);
            ServiceInfo service2_T4_1 = new ServiceInfo(2, "CAM", lVehicleIdTrainId4, 10, true, "10.0.4.1", "AID", "SID", 1020);

            ServiceInfo service3_T1_1 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 1020);
            ServiceInfo service3_T1_2 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId1, 10, true, "10.0.0.1", "AID", "SID", 1021);
            ServiceInfo service3_T2_1 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId2, 10, true, "10.0.2.2", "AID", "SID", 1020);
            ServiceInfo service3_T2_2 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId2, 10, true, "10.0.2.2", "AID", "SID", 1021);
            ServiceInfo service3_T3_1 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId3, 10, true, "10.0.3.1", "AID", "SID", 1020);
            ServiceInfo service3_T3_2 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId3, 10, true, "10.0.3.2", "AID", "SID", 1020);
            ServiceInfo service3_T4_1 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId4, 10, true, "10.0.4.1", "AID", "SID", 1020);
            ServiceInfo service3_T4_2 = new ServiceInfo(3, "MEDIA", lVehicleIdTrainId4, 10, true, "10.0.4.2", "AID", "SID", 1020);

            ServiceInfoList expectedServiceList_t1 = new ServiceInfoList(new ServiceInfo[] { 
                service1_T1_2, service1_T1_1, service1_T3_2, service1_T3_3, service1_T4_2, 
                 service2_T1_1,service2_T2_1, service2_T4_1, service3_T1_1, service3_T1_2,
                 service3_T2_1, service3_T2_2, service3_T3_1, service3_T3_2, service3_T4_1, 
                 service3_T4_2, service1_T2_1_O, service1_T3_1_O, service1_T4_1_O, service2_T3_1_O});

            ServiceInfoList expectedServiceList_t2 = new ServiceInfoList(new ServiceInfo[] { 
                service1_T1_2, service1_T1_1, service1_T3_2, service1_T3_3, service1_T4_2, 
                 service2_T2_1, service2_T1_1, service2_T4_1, service3_T2_1, service3_T2_2, 
                 service3_T1_1, service3_T1_2, service3_T3_1, service3_T3_2, service3_T4_1, 
                 service3_T4_2, service1_T2_1_O, service1_T3_1_O, service1_T4_1_O, service2_T3_1_O});

            ServiceInfoList expectedServiceList_t3 = new ServiceInfoList(new ServiceInfo[] { 
                service1_T3_2, service1_T3_3, service1_T1_2, service1_T1_1, service1_T4_2, 
                 service2_T1_1,service2_T2_1, service2_T4_1, service3_T3_1, service3_T3_2, 
                 service3_T1_1, service3_T1_2, service3_T2_1, service3_T2_2, service3_T4_1, 
                 service3_T4_2, service1_T3_1_O, service1_T2_1_O, service1_T4_1_O, service2_T3_1_O});

            ServiceInfoList expectedServiceList_t4 = new ServiceInfoList(new ServiceInfo[] { 
                service1_T4_2, service1_T1_2, service1_T1_1, service1_T3_2, service1_T3_3, 
                 service2_T4_1, service2_T1_1,service2_T2_1, service3_T4_1, service3_T4_2, 
                 service3_T1_1, service3_T1_2, service3_T2_1, service3_T2_2, service3_T3_1,
                 service3_T3_2, service1_T4_1_O, service1_T2_1_O, service1_T3_1_O, service2_T3_1_O});

            ////////////////////////
            // Register the trains in the system
            ////////////////////////
            PIS.Ground.Core.Data.SystemInfo expectedSystemInfo_T1 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);
            PIS.Ground.Core.Data.SystemInfo expectedSystemInfo_T2 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId2,
                "",
                lVehicleIdTrainId2,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);
            PIS.Ground.Core.Data.SystemInfo expectedSystemInfo_T3 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId3,
                "",
                lVehicleIdTrainId3,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);
            PIS.Ground.Core.Data.SystemInfo expectedSystemInfo_T4 = new PIS.Ground.Core.Data.SystemInfo(
                lTrainId4,
                "",
                lVehicleIdTrainId4,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                new ServiceInfoList(),
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            ds.OnSystemChanged(expectedSystemInfo_T1);
            Assert.IsTrue(ds.IsElementOnline(lTrainId1), "The system '{0}' is not online in data store", lTrainId1);
            Assert.AreEqual(expectedSystemInfo_T1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);

            ds.OnSystemChanged(expectedSystemInfo_T2);
            Assert.IsTrue(ds.IsElementOnline(lTrainId2), "The system '{0}' is not online in data store", lTrainId2);
            Assert.AreEqual(expectedSystemInfo_T2, ds.GetSystem(lTrainId2), "The system '{0}' was not set to expected value in data store", lTrainId2);

            ds.OnSystemChanged(expectedSystemInfo_T3);
            Assert.IsTrue(ds.IsElementOnline(lTrainId3), "The system '{0}' is not online in data store", lTrainId3);
            Assert.AreEqual(expectedSystemInfo_T3, ds.GetSystem(lTrainId3), "The system '{0}' was not set to expected value in data store", lTrainId3);

            ds.OnSystemChanged(expectedSystemInfo_T4);
            Assert.IsTrue(ds.IsElementOnline(lTrainId4), "The system '{0}' is not online in data store", lTrainId4);
            Assert.AreEqual(expectedSystemInfo_T4, ds.GetSystem(lTrainId4), "The system '{0}' was not set to expected value in data store", lTrainId4);

            ////////////////////////
            // Update service list of TRAIN-1
            ////////////////////////

            for (int i = expectedServiceList_t1.Count - 1; i >= 0; --i)
            {
                ServiceInfo currentService = expectedServiceList_t1[i];
                string currentTrainId = lTrainId1;
                ushort currentVehicleId = lVehicleIdTrainId1;
                ServiceInfoList serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { currentService });

                Assert.AreEqual(currentService.VehiclePhysicalId == currentVehicleId, ds.OnServiceChanged(currentTrainId, true, subscriptionIds[currentService.ServiceId], serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}' for service: <{1}>", currentTrainId, currentService);
            }

            expectedServiceList_t1 = new ServiceInfoList(expectedServiceList_t1.Where(i => i.VehiclePhysicalId == lVehicleIdTrainId1));

            expectedSystemInfo_T1 = new SystemInfo(
                lTrainId1,
                "",
                lVehicleIdTrainId1,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList_t1,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo_T1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);
            Assert.AreEqual(expectedSystemInfo_T2, ds.GetSystem(lTrainId2), "The system '{0}' was not set to expected value in data store", lTrainId2);
            Assert.AreEqual(expectedSystemInfo_T3, ds.GetSystem(lTrainId3), "The system '{0}' was not set to expected value in data store", lTrainId3);
            Assert.AreEqual(expectedSystemInfo_T4, ds.GetSystem(lTrainId4), "The system '{0}' was not set to expected value in data store", lTrainId4);

            ////////////////////////
            // Update service list of TRAIN-2
            ////////////////////////

            for (int i = expectedServiceList_t2.Count - 1; i >= 0; --i)
            {
                ServiceInfo currentService = expectedServiceList_t2[i];
                string currentTrainId = lTrainId2;
                ushort currentVehicleId = lVehicleIdTrainId2;
                ServiceInfoList serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { currentService });

                Assert.AreEqual(currentService.VehiclePhysicalId == currentVehicleId, ds.OnServiceChanged(currentTrainId, true, subscriptionIds[currentService.ServiceId], serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}' for service: <{1}>", currentTrainId, currentService);
            }
            expectedServiceList_t2 = new ServiceInfoList(expectedServiceList_t2.Where(i => i.VehiclePhysicalId == lVehicleIdTrainId2));


            expectedSystemInfo_T2 = new SystemInfo(
                lTrainId2,
                "",
                lVehicleIdTrainId2,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList_t2,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo_T1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);
            Assert.AreEqual(expectedSystemInfo_T2, ds.GetSystem(lTrainId2), "The system '{0}' was not set to expected value in data store", lTrainId2);
            Assert.AreEqual(expectedSystemInfo_T3, ds.GetSystem(lTrainId3), "The system '{0}' was not set to expected value in data store", lTrainId3);
            Assert.AreEqual(expectedSystemInfo_T4, ds.GetSystem(lTrainId4), "The system '{0}' was not set to expected value in data store", lTrainId4);

            ////////////////////////
            // Update service list of TRAIN-3
            ////////////////////////

            for (int i = 0; i < expectedServiceList_t3.Count; ++i)
            {
                ServiceInfo currentService = expectedServiceList_t3[i];
                string currentTrainId = lTrainId3;
                ushort currentVehicleId = lVehicleIdTrainId3;
                ServiceInfoList serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { currentService });

                Assert.AreEqual(currentService.VehiclePhysicalId == currentVehicleId, ds.OnServiceChanged(currentTrainId, true, subscriptionIds[currentService.ServiceId], serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}' for service: <{1}>", currentTrainId, currentService);
            }

            expectedServiceList_t3 = new ServiceInfoList(expectedServiceList_t3.Where(i => i.VehiclePhysicalId == lVehicleIdTrainId3));

            expectedSystemInfo_T3 = new SystemInfo(
                lTrainId3,
                "",
                lVehicleIdTrainId3,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList_t3,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo_T1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);
            Assert.AreEqual(expectedSystemInfo_T2, ds.GetSystem(lTrainId2), "The system '{0}' was not set to expected value in data store", lTrainId2);
            Assert.AreEqual(expectedSystemInfo_T3, ds.GetSystem(lTrainId3), "The system '{0}' was not set to expected value in data store", lTrainId3);
            Assert.AreEqual(expectedSystemInfo_T4, ds.GetSystem(lTrainId4), "The system '{0}' was not set to expected value in data store", lTrainId4);

            ////////////////////////
            // Update service list of TRAIN-4
            ////////////////////////

            for (int i = 0; i < expectedServiceList_t4.Count; ++i)
            {
                ServiceInfo currentService = expectedServiceList_t4[i];
                string currentTrainId = lTrainId4;
                ushort currentVehicleId = lVehicleIdTrainId4;
                ServiceInfoList serviceUpdateList = new ServiceInfoList(new ServiceInfo[] { currentService });

                Assert.AreEqual(currentService.VehiclePhysicalId == currentVehicleId, ds.OnServiceChanged(currentTrainId, true, subscriptionIds[currentService.ServiceId], serviceUpdateList), "Wrong return value for method OnServiceUpdate on train '{0}' for service: <{1}>", currentTrainId, currentService);
            }

            expectedServiceList_t4 = new ServiceInfoList(expectedServiceList_t4.Where(i => i.VehiclePhysicalId == lVehicleIdTrainId4));

            expectedSystemInfo_T4 = new SystemInfo(
                lTrainId4,
                "",
                lVehicleIdTrainId4,
                0,
                true,
                PIS.Ground.Core.Data.CommunicationLink.WIFI,
                expectedServiceList_t4,
                new PisBaseline(),
                new PisVersion(),
                new PisMission(),
                false);

            Assert.AreEqual(expectedSystemInfo_T1, ds.GetSystem(lTrainId1), "The system '{0}' was not set to expected value in data store", lTrainId1);
            Assert.AreEqual(expectedSystemInfo_T2, ds.GetSystem(lTrainId2), "The system '{0}' was not set to expected value in data store", lTrainId2);
            Assert.AreEqual(expectedSystemInfo_T3, ds.GetSystem(lTrainId3), "The system '{0}' was not set to expected value in data store", lTrainId3);
            Assert.AreEqual(expectedSystemInfo_T4, ds.GetSystem(lTrainId4), "The system '{0}' was not set to expected value in data store", lTrainId4);

            ////////////////////////
            // Verify GetAvailableServiceData on system
            ////////////////////////
            Assert.AreEqual(expectedServiceList_t1, ds.GetAvailableServiceData(lTrainId1), "The method GetAvailableServiceData('{0}') does not return the expected value", lTrainId1);
            Assert.AreEqual(expectedServiceList_t2, ds.GetAvailableServiceData(lTrainId2), "The method GetAvailableServiceData('{0}') does not return the expected value", lTrainId2);
            Assert.AreEqual(expectedServiceList_t3, ds.GetAvailableServiceData(lTrainId3), "The method GetAvailableServiceData('{0}') does not return the expected value", lTrainId3);
            Assert.AreEqual(expectedServiceList_t4, ds.GetAvailableServiceData(lTrainId4), "The method GetAvailableServiceData('{0}') does not return the expected value", lTrainId4);
            Assert.AreEqual(new ServiceInfoList(), ds.GetAvailableServiceData(lTrainId4 + "SSS"), "The method GetAvailableServiceData('{0}') does not return the expected value", lTrainId4 + "SSS");

            ////////////////////////
            // Verify GetAvailableServiceData on system and service
            ////////////////////////
            string[] trainIds = { lTrainId1, lTrainId2, lTrainId3, lTrainId4, "NonExistingTrain" };
            ServiceInfo[][] expectedResults = new ServiceInfo[][]{
                                                   new ServiceInfo[]{ null, service1_T1_2, service2_T1_1, service3_T1_1 },
                                                   new ServiceInfo[]{ null, service1_T2_1_O, service2_T2_1, service3_T2_1 },
                                                   new ServiceInfo[]{ null, service1_T3_2, service2_T3_1_O, service3_T3_1 },
                                                   new ServiceInfo[]{ null, service1_T4_2, service2_T4_1, service3_T4_1 },
                                                   new ServiceInfo[]{ null, null, null, null },

            };

            for (int trainIndex = 0; trainIndex < trainIds.Length; ++trainIndex)
            {
                string trainId = trainIds[trainIndex];
                for (int serviceIndex = 0; serviceIndex <= 3; ++serviceIndex)
                {
                    ServiceInfo expectedService = expectedResults[trainIndex][serviceIndex];
                    Assert.AreEqual(expectedService, ds.GetAvailableServiceData(trainId, serviceIndex), "The method GetAvailableServiceData('{0}', {1}) does not return the expected value", trainId, serviceIndex);
                }
            }
        }

        #endregion
    }
}