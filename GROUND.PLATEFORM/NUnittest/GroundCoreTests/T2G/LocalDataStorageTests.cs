//---------------------------------------------------------------------------------------------------
// <copyright file="LocalDataStorageTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
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
using PIS.Ground.Core;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using System.Reflection;
using System.Diagnostics;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.Common;

namespace GroundCoreTests
{
	/// <summary>LocalDataStorageTests test class.</summary>
	[TestFixture]
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
				1,
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
				1,
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
				1,
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
			var elapsedMs = watch.ElapsedMilliseconds;
			Console.WriteLine("Time elapsed : " + elapsedMs.ToString() + "ms");

			//Check time used to check if an element exists in the system list
			Console.WriteLine("Check if an element exists in the system list");

			bool lElementExists = false;

			watch = Stopwatch.StartNew();

			lElementExists = ds.ElementExists("TRAIN-400");

			watch.Stop();
			elapsedMs = watch.ElapsedMilliseconds;
			Console.WriteLine("Time elapsed : " + elapsedMs.ToString() + "ms");
			Console.WriteLine("Element exists ? " + lElementExists.ToString());
		}

		#endregion
	}
}