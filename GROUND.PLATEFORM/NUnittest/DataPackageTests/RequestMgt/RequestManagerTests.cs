//---------------------------------------------------------------------------------------------------
// <copyright file="RequestManagerTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2015.  All rights reserved.
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
using Moq;
using PIS.Ground.DataPackage;

namespace DataPackageTests.RequestMgt
{
	/// <summary>Request manager tests.</summary>
	[TestFixture, Category("RequestManager")]
	class RequestManagerTests
	{
		/// <summary>The train 2ground manager mock.</summary>
		Mock<PIS.Ground.Core.T2G.IT2GManager> _train2groundManagerMock;

		/// <summary>The notification sender mock.</summary>
		Mock<PIS.Ground.Core.Common.INotificationSender> _notificationSenderMock;

		/// <summary>The session manager mock.</summary>
		Mock<PIS.Ground.Core.SessionMgmt.ISessionManager> _sessionMgrMock;

		/// <summary>The request factory.</summary>
		Mock<PIS.Ground.DataPackage.RequestMgt.IRequestContextFactory> _requestFactoryMock;

		/// <summary>The remote data store factory.</summary>
		Mock<PIS.Ground.DataPackage.RemoteDataStoreFactory.IRemoteDataStoreFactory> _remoteDataStoreFactoryMock;

		/// <summary>The remote data store mock.</summary>
		Mock<PIS.Ground.RemoteDataStore.IRemoteDataStore> _remoteDataStoreMock;

		/// <summary>The tested instance.</summary>
		RequestManagerMonitor _testedInstance;

		/// <summary>Initializes a new instance of the RequestManagerTests class.</summary>
		public RequestManagerTests()
		{
		}

		/// <summary>Sets the up.</summary>
		[SetUp]
		protected void SetUp()
		{
			_train2groundManagerMock = new Mock<PIS.Ground.Core.T2G.IT2GManager>();
			_notificationSenderMock = new Mock<PIS.Ground.Core.Common.INotificationSender>();
			_testedInstance = new RequestManagerMonitor();
			_testedInstance.Initialize(_train2groundManagerMock.Object, _notificationSenderMock.Object);

			_sessionMgrMock = new Mock<PIS.Ground.Core.SessionMgmt.ISessionManager>();
			_requestFactoryMock = new Mock<PIS.Ground.DataPackage.RequestMgt.IRequestContextFactory>();
			_remoteDataStoreMock = new Mock<PIS.Ground.RemoteDataStore.IRemoteDataStore>();
			_remoteDataStoreFactoryMock = new Mock<PIS.Ground.DataPackage.RemoteDataStoreFactory.IRemoteDataStoreFactory>();
			PIS.Ground.DataPackage.DataPackageService.Initialize(
				_sessionMgrMock.Object,
				_notificationSenderMock.Object,
				_train2groundManagerMock.Object,
				_requestFactoryMock.Object,
				_remoteDataStoreFactoryMock.Object,
				_testedInstance,
                false);
		}

		/// <summary>Tear down.</summary>
		[TearDown]
		protected void TearDown()
		{
            if (_testedInstance != null)
            {
                _testedInstance.Dispose();
            }

            DataPackageService.Uninitialize();
		}

		#region Initialize

		/// <summary>Initializes the request manager with a null T2GManager instance.</summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InitializeT2GManagerNull()
		{
			_testedInstance.Initialize(null, _notificationSenderMock.Object);
		}

		/// <summary>Initializes the request manager with a null notification sender instance.</summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InitializeNotificationSenderNull()
		{
			_testedInstance.Initialize(_train2groundManagerMock.Object, null);
		}

		#endregion

		#region AddRequest

		/// <summary>Test the add of a null request.</summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddRequestRequestNull()
		{
			_testedInstance.AddRequest(null);
		}

		/// <summary>Test the add of a request which is not a distribute baseline request.</summary>
		[Test]
		public void AddRequestRequestNotBaselineDistributing()
		{
			Mock<PIS.Ground.Core.Data.IRequestContext> requestContext = new Mock<PIS.Ground.Core.Data.IRequestContext>();
			requestContext.Setup(f => f.RequestId).Returns(Guid.NewGuid());

			_testedInstance.AddRequest(requestContext.Object);
            Assert.IsTrue(_testedInstance.WaitRequestProcessed(new TimeSpan(0, 0, 5)), "New requests were not processes at least one time by request processor");

			requestContext.Verify(f => f.State, Times.AtLeastOnce());
			_remoteDataStoreMock.Verify(f => f.saveBaselineDistributingRequest(It.IsAny<PIS.Ground.RemoteDataStore.DataContainer>()), Times.Never());
		}

		/// <summary>Test the add of a request which is a distribute baseline request. We are checking the call for a remote datastore instance to know if DataPackage static funtion as been called.</summary>
		[Test]
		public void AddRequestRequestBaselineDistributing()
		{
			PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext requestContext
				= new PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext(
					string.Empty,
					"107",
					Guid.NewGuid(),
					Guid.NewGuid(),
					new PIS.Ground.DataPackage.BaselineDistributionAttributes(),
					false,
					"1.2.3.4",
					DateTime.Now,
					DateTime.Now.AddHours(1));

			_testedInstance.AddRequest(requestContext);
            Assert.IsTrue(_testedInstance.WaitRequestProcessed(new TimeSpan(0, 0, 5)), "New requests were not processes at least one time by request processor");

			_remoteDataStoreFactoryMock.Verify(f => f.GetRemoteDataStoreInstance(), Times.Once());
		}

		#endregion

		#region AddRequestRange

		/// <summary>Adds a null request list.</summary>
        [Test, Category("AddRequestRange")]
		public void AddRequestRangeNullList()
		{
			List<PIS.Ground.Core.Data.IRequestContext> requestContextList = null;
			_testedInstance.AddRequestRange(requestContextList);
		}

		/// <summary>Adds an empty request list.</summary>
        [Test, Category("AddRequestRange")]
		public void AddRequestRangeEmptyList()
		{
			List<PIS.Ground.Core.Data.IRequestContext> requestContextList = new List<PIS.Ground.Core.Data.IRequestContext>();
			_testedInstance.AddRequestRange(requestContextList);
		}

		/// <summary>Adds a request list with one element.</summary>
        [Test, Category("AddRequestRange")]
		public void AddRequestRangeOneElementList()
		{
			List<PIS.Ground.Core.Data.IRequestContext> requestContextList = new List<PIS.Ground.Core.Data.IRequestContext>();

			Mock<PIS.Ground.Core.Data.IRequestContext> requestContext = new Mock<PIS.Ground.Core.Data.IRequestContext>();
			requestContext.Setup(f => f.RequestId).Returns(Guid.NewGuid());
			requestContext.Setup(f => f.ElementId).Returns("requestContext");
			requestContext.Setup(f => f.State).Returns(PIS.Ground.Core.Data.RequestState.Created);

			requestContextList.Add(requestContext.Object);

			_testedInstance.AddRequestRange(requestContextList);
            Assert.IsTrue(_testedInstance.WaitRequestProcessed(new TimeSpan(0, 0, 5)), "New requests were not processes at least one time by request processor");

			requestContext.Verify(f => f.State, Times.AtLeastOnce());
		}

		/// <summary>Adds a request list with two elements.</summary>
        [Test, Category("AddRequestRange")]
		public void AddRequestRangeTwoElementsList()
		{
			List<PIS.Ground.Core.Data.IRequestContext> requestContextList = new List<PIS.Ground.Core.Data.IRequestContext>();

			Mock<PIS.Ground.Core.Data.IRequestContext> requestContextOne = new Mock<PIS.Ground.Core.Data.IRequestContext>();
			requestContextOne.Setup(f => f.RequestId).Returns(Guid.NewGuid());
			requestContextOne.Setup(f => f.ElementId).Returns("One");
			requestContextOne.Setup(f => f.State).Returns(PIS.Ground.Core.Data.RequestState.Created);
			Mock<PIS.Ground.Core.Data.IRequestContext> requestContextTwo = new Mock<PIS.Ground.Core.Data.IRequestContext>();
			requestContextTwo.Setup(g => g.RequestId).Returns(Guid.NewGuid());
			requestContextTwo.Setup(g => g.ElementId).Returns("Two");
			requestContextTwo.Setup(g => g.State).Returns(PIS.Ground.Core.Data.RequestState.Created);

			requestContextList.Add(requestContextOne.Object);
			requestContextList.Add(requestContextTwo.Object);

			_testedInstance.AddRequestRange(requestContextList);
            Assert.IsTrue(_testedInstance.WaitRequestProcessed(new TimeSpan(0, 0, 5)), "New requests were not processes at least one time by request processor");

			requestContextOne.Verify(f => f.State, Times.AtLeastOnce());
			requestContextTwo.Verify(g => g.State, Times.AtLeastOnce());
		}

		#endregion
	}
}
