//---------------------------------------------------------------------------------------------------
// <copyright file="T2GManagerTests.cs" company="Alstom">
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
using PIS.Ground.Core.T2G;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.Utility;

namespace GroundCoreTests.T2G
{
	/// <summary>T2G manager tests.</summary>
	[TestFixture, Category("T2G")]
	public class T2GManagerTests : IT2GNotifierTarget, IT2GConnectionListener
	{
		#region Attributes

		// The T2GManager class to test!.
		private T2GManager _t2gManager;

		// Mock classes to help testing.
		//private Mock<T2GSessionData> _sessionData;
		private T2GSessionData _sessionData;
		//private Mock<T2GLocalDataStorage> _localDataStorage;
		private T2GLocalDataStorage _localDataStorage;
		//private Mock<IT2GNotifierTarget> _notifierTarget;
		//private Mock<IT2GConnectionListener> _connectionListener;
		//private Mock<T2GFileDistributionManager> _fileDistributionManager;
		private T2GFileDistributionManager _fileDistributionManager;
		//private Mock<T2GNotificationProcessor> _notifier;
		private T2GNotificationProcessor _notifier;
		//private Mock<T2GConnectionManager> _connectionManager;
		//private T2GConnectionManager _connectionManager;
		//private MockT2GConnectionManager _connectionManager;
		private Mock<IT2GConnectionManager> _connectionManager;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the T2GManagerTests class.
		/// NOTE: The class is created only once for all tests.
		/// Use Setup() and TearDown() to initialize / free test objects.
		/// </summary>
		public T2GManagerTests()
		{
		}

		#endregion

		#region Tests Management

		/// <summary>Setup called before each test to initialize variables.</summary>
		[SetUp]
		public void Setup()
		{
			ServiceConfiguration.Initialize();
			//_sessionData = new Mock<T2GSessionData>();
			_sessionData = new T2GSessionData();
			//_localDataStorage = new Mock<T2GLocalDataStorage>(_sessionData);
			_localDataStorage = new T2GLocalDataStorage(_sessionData, false);
			//_notifierTarget = new Mock<IT2GNotifierTarget>();
			//_connectionListener = new Mock<IT2GConnectionListener>();
			//_fileDistributionManager = new Mock<T2GFileDistributionManager>(_sessionData, _notifierTarget.Object);
			_fileDistributionManager = new T2GFileDistributionManager(_sessionData, this);
			//_notifier = new Mock<T2GNotificationProcessor>(_notifierTarget.Object, _localDataStorage, _fileDistributionManager.Object);
			_notifier = new T2GNotificationProcessor(this, _localDataStorage, _fileDistributionManager);
			//_connectionManager = new Mock<T2GConnectionManager>(_sessionData, _connectionListener.Object);
			//_connectionManager = new MockT2GConnectionManager();
			_connectionManager = new Mock<IT2GConnectionManager>();

			//_t2gManager = new T2GManager(_sessionData, _localDataStorage, _fileDistributionManager, _notifier, _connectionManager);
			_t2gManager = new T2GManager(_sessionData, _localDataStorage, _fileDistributionManager, _notifier, _connectionManager.Object);
		}

		/// <summary>Tear down called after each test to clean up.</summary>
		[TearDown]
		public void TearDown()
		{
            if (_t2gManager != null)
            {
                _t2gManager.Dispose();
            }

			_t2gManager = null;

			_sessionData = null;
			_localDataStorage = null;
			//_notifierTarget = null;
			//_connectionListener = null;
			_fileDistributionManager = null;
			_notifier = null;
			_connectionManager = null;
		}

		#endregion

		#region IT2GManager Test Methods

		/// <summary>Tests the IT2GManager interface method IsElementOnline: Tests if an element is online.</summary>
		[Test]
		public void T2GManagerIsElementOnlineTest()
		{
			string train1Element = "TRAIN-1";
			bool isOnline;
			T2GManagerErrorEnum res;

			// Test we have no connection to T2G Server
			res = _t2gManager.IsElementOnline(train1Element, out isOnline);
			Assert.AreEqual(T2GManagerErrorEnum.eT2GServerOffline, res, "We should not be connected to T2G Server.");
			Assert.IsFalse(isOnline, "T2G Server is not considered online so the element should not be considered online.");

			// Simulate T2G Server becoming online...
			OnConnectionStatusChanged(true);

			// Test Element is not online / not found:
			res = _t2gManager.IsElementOnline(train1Element, out isOnline);
			Assert.AreEqual(T2GManagerErrorEnum.eElementNotFound, res, "The element is unknown (not in the database), so we should receive eElementNotFound.");
			Assert.IsFalse(isOnline, "Should not be online, the element is unkown to the system.");

			// Add an element to the data storage.
			SystemInfo newElement = new SystemInfo(
						train1Element,
						"MISSION-1",
						1,
						0,
						true,
						CommunicationLink.WIFI,
						new ServiceInfoList(),
						new PisBaseline(),
						new PisVersion(),
						new PisMission(),
						true);
			_localDataStorage.OnSystemChanged(newElement);

			res = _t2gManager.IsElementOnline(train1Element, out isOnline);
			Assert.AreEqual(T2GManagerErrorEnum.eSuccess, res, "The element should exist and T2G is suppose to be online.");
			Assert.IsTrue(isOnline, "The element should be online, we added it previously.");

			// Update the element and make it offline!
			newElement = new SystemInfo(
						train1Element,
						"MISSION-1",
						1,
						0,
						false,
						CommunicationLink.WIFI,
						new ServiceInfoList(),
						new PisBaseline(),
						new PisVersion(),
						new PisMission(),
						true);
			_localDataStorage.OnSystemChanged(newElement);

			res = _t2gManager.IsElementOnline(train1Element, out isOnline);
			Assert.AreEqual(T2GManagerErrorEnum.eSuccess, res, "The element should exist and T2G is suppose to be online.");
			Assert.IsFalse(isOnline, "The element should be offline, we updated it's online property to false.");

			// Simulate T2G Server is going offline...
			OnConnectionStatusChanged(false);

			// Test we have no connection to T2G Server
			res = _t2gManager.IsElementOnline(train1Element, out isOnline);
			Assert.AreEqual(T2GManagerErrorEnum.eT2GServerOffline, res, "We should not be connected to T2G Server.");
			Assert.IsFalse(isOnline, "T2G Server is not considered online so the element should not be considered online.");
		}

		/// <summary>Test something.</summary>
		[Test]
		public void T2GManagerIsElementOnlineAndPisBaselineUpToDateTest()
		{
			string train1Element = "TRAIN-1";
			bool res;

			// Test we have no connection to T2G Server
			res = _t2gManager.IsElementOnlineAndPisBaselineUpToDate(train1Element);
			Assert.IsFalse(res, "T2G server is offline, should have returned false.");

			// Simulate T2G Server becoming online...
			OnConnectionStatusChanged(true);

			// Test Element is not online / not found:
			res = _t2gManager.IsElementOnlineAndPisBaselineUpToDate(train1Element);
			Assert.IsFalse(res, "Should not be online, the element is unkown to the system.");

			// Add an element to the data storage (simulate that the pis baseline is not up to date).
			SystemInfo newElement = new SystemInfo(
						train1Element,
						"MISSION-1",
						1,
						0,
						true,
						CommunicationLink.WIFI,
						new ServiceInfoList(),
						new PisBaseline(),
						new PisVersion(),
						new PisMission(),
						false);
			_localDataStorage.OnSystemChanged(newElement);

			res = _t2gManager.IsElementOnlineAndPisBaselineUpToDate(train1Element);
			Assert.IsFalse(res, "The element is online but does not have it's baseline up to date, should have returned false.");

			// Update the element in the data storage (simulate that the pis baseline is now up to date).
			newElement = new SystemInfo(
			            train1Element,
			            "MISSION-1",
			            1,
			            0,
			            true,
			            CommunicationLink.WIFI,
			            new ServiceInfoList(),
			            new PisBaseline(),
			            new PisVersion(),
			            new PisMission(),
			            true);
			_localDataStorage.OnSystemChanged(newElement);
			res = _t2gManager.IsElementOnlineAndPisBaselineUpToDate(train1Element);
            Assert.IsTrue(res, "The element is online and it's baseline is up to date, should have returned true.");

			// Update the element in the data storage (simulate that the pis baseline is now up to date).
			_localDataStorage.OnMessageChanged(train1Element, "PIS.BASELINE", new PisBaseline());

			res = _t2gManager.IsElementOnlineAndPisBaselineUpToDate(train1Element);
			Assert.IsTrue(res, "The element is online and it's baseline is up to date, should have returned true.");

			// Go offline
			newElement = new SystemInfo(
						train1Element,
						"MISSION-1",
						1,
						0,
						false,
						CommunicationLink.WIFI,
						new ServiceInfoList(),
						new PisBaseline(),
						new PisVersion(),
						new PisMission(),
						false);
			_localDataStorage.OnSystemChanged(newElement);

			res = _t2gManager.IsElementOnlineAndPisBaselineUpToDate(train1Element);
			Assert.IsFalse(res, "Should not be online, the element is offline.");

			// Element now back online!
			newElement = new SystemInfo(
						train1Element,
						"MISSION-1",
						1,
						0,
						true,
						CommunicationLink.WIFI,
						new ServiceInfoList(),
						new PisBaseline(),
						new PisVersion(),
						new PisMission(),
						true);
			_localDataStorage.OnSystemChanged(newElement);

			res = _t2gManager.IsElementOnlineAndPisBaselineUpToDate(train1Element);
            Assert.IsTrue(res, "The element is online and it's baseline is up to date, should have returned true.");

			// Update the baseline information.
			_localDataStorage.OnMessageChanged(train1Element, "PIS.BASELINE", new PisBaseline());

			res = _t2gManager.IsElementOnlineAndPisBaselineUpToDate(train1Element);
			Assert.IsTrue(res, "The element is online and it's baseline is up to date, should have returned true.");
		}

		#endregion

		#region Implemented interfaces
		
		#region IT2GConnectionListener Members

		/// <summary>Executes the connection status changed action.</summary>
		/// <param name="connected">The new connection status: true if connected.</param>
		public void OnConnectionStatusChanged(bool connected)
		{
			_connectionManager.Setup(x => x.T2GServerConnectionStatus).Returns(connected);

			if (connected)
			{
				_localDataStorage.InitializeLocalStorageData();
				_fileDistributionManager.Initialize();
			}
			else
			{
				_localDataStorage.DeinitializeLocalStorageData();
				_fileDistributionManager.Deinitialize();
			}

			RaiseOnT2GOnlineStatusNotificationEvent(new T2GOnlineStatusNotificationArgs { online = connected });
		}

		#endregion

		#region IT2GNotifierTarget Members

		/// <summary>Raises the on file distribute notification event.</summary>
		/// <param name="args">The arguments.</param>
		public void RaiseOnFileDistributeNotificationEvent(FileDistributionStatusArgs args)
		{
			if (_t2gManager != null)
			{
				_t2gManager.RaiseOnFileDistributeNotificationEvent(args);
			}
		}

		/// <summary>Raises the on file distribute notification event.</summary>
		/// <param name="args">The arguments.</param>
		/// <param name="taskId">Identifier for the task.</param>
		public void RaiseOnFileDistributeNotificationEvent(FileDistributionStatusArgs args, int taskId)
		{
			if (_t2gManager != null)
			{
				_t2gManager.RaiseOnFileDistributeNotificationEvent(args, taskId);
			}
		}

		/// <summary>Raises the on file distribute notification event.</summary>
		/// <param name="args">The arguments.</param>
		/// <param name="RequestId">Identifier for the request.</param>
		public void RaiseOnFileDistributeNotificationEvent(FileDistributionStatusArgs args, Guid RequestId)
		{
			if (_t2gManager != null)
			{
				_t2gManager.RaiseOnFileDistributeNotificationEvent(args);
			}
		}

		/// <summary>Raises the on file publication notification event.</summary>
		/// <param name="args">The arguments.</param>
		public void RaiseOnFilePublicationNotificationEvent(FilePublicationNotificationArgs args)
		{
			if (_t2gManager != null)
			{
				_t2gManager.RaiseOnFilePublicationNotificationEvent(args);
			}
		}

		/// <summary>Raises the on file published notification event.</summary>
		/// <param name="args">The arguments.</param>
		public void RaiseOnFilePublishedNotificationEvent(FilePublishedNotificationArgs args)
		{
			if (_t2gManager != null)
			{
				_t2gManager.RaiseOnFilePublishedNotificationEvent(args);
			}
		}

		/// <summary>Raises the on element information change event.</summary>
		/// <param name="args">Event information to send to registered event handlers.</param>
		public void RaiseOnElementInfoChangeEvent(ElementEventArgs args)
		{
			if (_t2gManager != null)
			{
				_t2gManager.RaiseOnElementInfoChangeEvent(args);
			}
		}

		/// <summary>Raises the on file received notification event.</summary>
		/// <param name="args">The arguments.</param>
		public void RaiseOnFileReceivedNotificationEvent(FileReceivedArgs args)
		{
			if (_t2gManager != null)
			{
				_t2gManager.RaiseOnFileReceivedNotificationEvent(args);
			}
		}

		/// <summary>Raises the on system deleted notification event.</summary>
		/// <param name="args">The arguments.</param>
		public void RaiseOnSystemDeletedNotificationEvent(SystemDeletedNotificationArgs args)
		{
			if (_t2gManager != null)
			{
				_t2gManager.RaiseOnSystemDeletedNotificationEvent(args);
			}
		}

		#endregion
		
		/// <summary>Sends a T2G online / offline notification event to local subscriber(s).</summary>
		/// <param name="pEvent">The notification event.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public void RaiseOnT2GOnlineStatusNotificationEvent(T2GOnlineStatusNotificationArgs eventArgs)
		{
			if (_t2gManager != null)
			{
				_t2gManager.NotifyEventHandlerAsync(eventArgs, null);
			}
		}

		#endregion

	}
}