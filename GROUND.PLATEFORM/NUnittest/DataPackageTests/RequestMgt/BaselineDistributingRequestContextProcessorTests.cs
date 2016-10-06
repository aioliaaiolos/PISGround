//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineDistributingRequestContextProcessorTests.cs" company="Alstom">
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
	/// <summary>Baseline distributing request context processor tests.</summary>
	[TestFixture]
	class BaselineDistributingRequestContextProcessorTests
	{
		/// <summary>The train 2ground manager mock.</summary>
		private Mock<PIS.Ground.Core.T2G.IT2GManager> _train2groundManagerMock;

		/// <summary>The remote data store factory.</summary>
		private Mock<PIS.Ground.DataPackage.RemoteDataStoreFactory.IRemoteDataStoreFactory> _remoteDataStoreFactoryMock;

		/// <summary>The remote data store mock.</summary>
		private Mock<PIS.Ground.RemoteDataStore.IRemoteDataStore> _remoteDataStoreMock;

		/// <summary>The tested instance.</summary>
		private PIS.Ground.Core.Data.IRequestContextProcessor _testedInstance;

        /// <summary>
        /// The baseline status updater
        /// </summary>
        private BaselineStatusUpdater _baselineStatusUpdater;

		/// <summary>Initializes a new instance of the RequestManagerTests class.</summary>
		public BaselineDistributingRequestContextProcessorTests()
		{
		}

		/// <summary>Sets the up.</summary>
		[SetUp]
		protected void SetUp()
		{
			_train2groundManagerMock = new Mock<PIS.Ground.Core.T2G.IT2GManager>();
			_remoteDataStoreMock = new Mock<PIS.Ground.RemoteDataStore.IRemoteDataStore>();
			_remoteDataStoreFactoryMock = new Mock<PIS.Ground.DataPackage.RemoteDataStoreFactory.IRemoteDataStoreFactory>();
            _baselineStatusUpdater = new BaselineStatusUpdater();

			_testedInstance = new PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContextProcessor(_remoteDataStoreFactoryMock.Object, _train2groundManagerMock.Object, _baselineStatusUpdater);
		}

		/// <summary>Tear down.</summary>
		[TearDown]
		protected void TearDown()
		{
            if (_baselineStatusUpdater != null)
            {
                _baselineStatusUpdater.Dispose();
                _baselineStatusUpdater = null;
            }
		}

		#region contructor

		/// <summary>Constructor with a RemoteDataStoreFactory null.</summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorRemoteDataStoreFactoryNull()
		{
			_testedInstance = new PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContextProcessor(null, _train2groundManagerMock.Object, _baselineStatusUpdater);
		}

        /// <summary>Constructor with a T2GManager null.</summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorT2GManagerNull()
        {
            _testedInstance = new PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContextProcessor(_remoteDataStoreFactoryMock.Object, null, _baselineStatusUpdater);
        }

        /// <summary>Constructor with a T2GManager null.</summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorBaselineStatusUpdaterNull()
        {
            _testedInstance = new PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContextProcessor(_remoteDataStoreFactoryMock.Object, _train2groundManagerMock.Object, null);
        }

		#endregion

		#region process

		/// <summary>Process with a null context.</summary>
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ProcessContextNull()
		{
			_testedInstance.Process(null);
		}

		/// <summary>Process with a not supported context.</summary>
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ProcessContextNotValid()
		{
			Mock<PIS.Ground.Core.Data.IRequestContext> requestContext = new Mock<PIS.Ground.Core.Data.IRequestContext>();
			_testedInstance.Process(requestContext.Object);
		}

		/// <summary>Process with an element online/not available.</summary>
		[Test]
		public void ProcessContextElementOffline()
		{
			bool isOnline = false;
			PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext requestContext
				= new PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext(
					string.Empty,
					string.Empty,
					Guid.NewGuid(),
					Guid.NewGuid(),
					new PIS.Ground.DataPackage.BaselineDistributionAttributes(),
					false,
					"1.2.3.4",
					DateTime.Now,
					DateTime.Now.AddDays(1));

			requestContext.TransmissionStatus = true;
			_train2groundManagerMock.Setup(f => f.IsElementOnline(It.IsAny<string>(), out isOnline)).Returns(PIS.Ground.Core.T2G.T2GManagerErrorEnum.eFailed);
			_testedInstance.Process(requestContext);
			Assert.AreEqual(2, requestContext.TransferAttemptsDone);

			requestContext.TransmissionStatus = true;
			_train2groundManagerMock.Setup(f => f.IsElementOnline(It.IsAny<string>(), out isOnline)).Returns(PIS.Ground.Core.T2G.T2GManagerErrorEnum.eSuccess);
			_testedInstance.Process(requestContext);
			Assert.AreEqual(4, requestContext.TransferAttemptsDone);
		}

		/// <summary>Process with an invalid endpoint.</summary>
		[Test]
		public void ProcessContextInvalidEndpoint()
		{
			bool isOnline = true;
			PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext requestContext
				= new PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext(
					string.Empty,
					string.Empty,
					Guid.NewGuid(),
					Guid.NewGuid(),
					new PIS.Ground.DataPackage.BaselineDistributionAttributes(),
					false,
					"1.2.3.4",
					DateTime.Now,
					DateTime.Now.AddDays(1));
			PIS.Ground.Core.Data.ServiceInfo serviceInfo = new PIS.Ground.Core.Data.ServiceInfo(
				0,
				string.Empty,
				0,
				0,
				false,
				string.Empty,
				string.Empty,
				string.Empty,
				0);

			_train2groundManagerMock.Setup(f => f.IsElementOnline(It.IsAny<string>(), out isOnline)).Returns(PIS.Ground.Core.T2G.T2GManagerErrorEnum.eSuccess);
			_train2groundManagerMock.Setup(f => f.GetAvailableServiceData(It.IsAny<string>(), It.IsAny<int>(), out serviceInfo)).Returns(PIS.Ground.Core.T2G.T2GManagerErrorEnum.eFailed);

			requestContext.TransmissionStatus = true;
			_testedInstance.Process(requestContext);
			Assert.AreEqual(2, requestContext.TransferAttemptsDone);
		}

		#endregion
	}
}
