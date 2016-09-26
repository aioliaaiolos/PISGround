//---------------------------------------------------------------------------------------------------
// <copyright file="RequestProcessorTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using Moq;
using NUnit.Framework;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.T2G;
using PIS.Ground.RealTime;

namespace PIS.Ground.RealTimeTests
{
	/// <summary>Request processor tests.</summary>
	[TestFixture, Category("RequestProcessor")]
	class RequestProcessorTests
	{
		#region attributes

		/// <summary>The train 2ground client mock.</summary>
        private Mock<IT2GManager> _train2groundClientMock;

		/// <summary>The rtpis data store.</summary>
		private Mock<IRTPISDataStore> _rtpisDataStore;
		
		/// <summary>The RealTimeService instance.</summary>
		private IRequestProcessor _rp;

		#endregion

		#region Tests management

		/// <summary>Initializes a new instance of the RealTimeServiceTests class.</summary>
		public RequestProcessorTests()
		{
			// Nothing to do
		}

		/// <summary>Setups called before each test to initialize variables.</summary>
		[SetUp]
		public void Setup()
		{
            this._train2groundClientMock = new Mock<IT2GManager>();
			this._rtpisDataStore = new Mock <IRTPISDataStore>();

			// callback registration
			this._train2groundClientMock.Setup(
				x => x.SubscribeToElementChangeNotification(
					It.IsAny<string>(),
					It.IsAny<EventHandler<ElementEventArgs>>()));

            _train2groundClientMock.Setup(t => t.T2GServerConnectionStatus).Returns(true);

			this._rp = new RequestProcessor(this._train2groundClientMock.Object, this._rtpisDataStore.Object);
		}

		/// <summary>Tear down called after each test to clean.</summary>
		[TearDown]
		public void TearDown()
		{
		}

		/// <summary>Initialises the element list two elements.</summary>
		/// <returns>A list of elements data.</returns>
		private ElementList<AvailableElementData> InitElementListTwoElements()
		{
			ElementList<AvailableElementData> result = new ElementList<AvailableElementData>();

			result.Add(new AvailableElementData()
			{
				ElementNumber = "Train1",
				MissionCommercialNumber = "Mission1",
				MissionOperatorCode = "Mission1Seg1",
				MissionState = MissionStateEnum.MI,
				OnlineStatus = true,
				PisBasicPackageVersion = "1.0.0.0",
				LmtPackageVersion = "1.0.0.0",
				PisBaselineData = new PisBaseline()
				{
					FutureVersionPisInfotainmentOut = "1.0.0.0",
					FutureVersionLmtOut = "1.0.0.0",
					FutureActivationDateOut = "1.0.0.0",
					FutureExpirationDateOut = "1.0.0.0",
					FutureValidOut = "1.0.0.0",
					FutureVersionOut = "1.0.0.0",
					FutureVersionPisBaseOut = "1.0.0.0",
					FutureVersionPisMissionOut = "1.0.0.0",
					CurrentVersionLmtOut = "1.0.0.0",
					CurrentExpirationDateOut = "1.0.0.0",
					CurrentVersionPisMissionOut = "1.0.0.0",
					CurrentVersionPisInfotainmentOut = "1.0.0.0",
					CurrentVersionOut = "1.0.0.0",
					CurrentVersionPisBaseOut = "1.0.0.0",
					CurrentForcedOut = "1.0.0.0",
					CurrentValidOut = "1.0.0.0",
					ArchivedValidOut = "1.0.0.0",
					ArchivedVersionOut = "1.0.0.0",
					ArchivedVersionPisBaseOut = "1.0.0.0",
					ArchivedVersionPisMissionOut = "1.0.0.0",
					ArchivedVersionPisInfotainmentOut = "1.0.0.0",
					ArchivedVersionLmtOut = "1.0.0.0"
				}
			});

			result.Add(new AvailableElementData()
			{
				ElementNumber = "Train2",
				MissionCommercialNumber = "Mission1",
				MissionOperatorCode = "Mission1Seg1",
				MissionState = MissionStateEnum.MI,
				OnlineStatus = true,
				PisBasicPackageVersion = "1.0.0.0",
				LmtPackageVersion = "1.0.0.0",
				PisBaselineData = new PisBaseline()
				{
					FutureVersionPisInfotainmentOut = "1.0.0.0",
					FutureVersionLmtOut = "1.0.0.0",
					FutureActivationDateOut = "1.0.0.0",
					FutureExpirationDateOut = "1.0.0.0",
					FutureValidOut = "1.0.0.0",
					FutureVersionOut = "1.0.0.0",
					FutureVersionPisBaseOut = "1.0.0.0",
					FutureVersionPisMissionOut = "1.0.0.0",
					CurrentVersionLmtOut = "1.0.0.0",
					CurrentExpirationDateOut = "1.0.0.0",
					CurrentVersionPisMissionOut = "1.0.0.0",
					CurrentVersionPisInfotainmentOut = "1.0.0.0",
					CurrentVersionOut = "1.0.0.0",
					CurrentVersionPisBaseOut = "1.0.0.0",
					CurrentForcedOut = "1.0.0.0",
					CurrentValidOut = "1.0.0.0",
					ArchivedValidOut = "1.0.0.0",
					ArchivedVersionOut = "1.0.0.0",
					ArchivedVersionPisBaseOut = "1.0.0.0",
					ArchivedVersionPisMissionOut = "1.0.0.0",
					ArchivedVersionPisInfotainmentOut = "1.0.0.0",
					ArchivedVersionLmtOut = "1.0.0.0"
				}
			});

			return result;
		}

		#endregion

		#region Tests

		/// <summary>Empty element data changed notification.</summary>
		[Test]
		public void EmptyElementDataChangedNotification()
		{
			ElementList<AvailableElementData> elementList = null;
			RTPISDataStoreEventArgs eventArgs = new RTPISDataStoreEventArgs("DummyMission",null);

			this._rtpisDataStore.Raise(m => m.Changed -= null, eventArgs);
			this._train2groundClientMock.Verify(foo => foo.GetAvailableElementDataListByMissionCode(It.IsAny<string>(), out elementList), Times.Once());
		}

		/// <summary>Mission changed notification multiple elements.</summary>
        [Test]
        public void MissionChangedNotificationMultipleElements()
        {
            int numberOfElements = 10;
            ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
            System.Collections.Generic.List<TrainServerSimulator.RealTimeTrainService> trainList = new System.Collections.Generic.List<TrainServerSimulator.RealTimeTrainService>();
            trainList.Capacity = numberOfElements;
            System.Collections.Generic.List<ServiceInfo> serviceInfoList = new System.Collections.Generic.List<ServiceInfo>();

            try
            {
                string missionCode = "Mission1";

                for (int i = 0; i < numberOfElements; i++)
                {
                    elementList.Add(new AvailableElementData()
                    {
                        ElementNumber = "Train" + i,
                        MissionCommercialNumber = "Mission" + i,
                        MissionOperatorCode = "Mission" + i + "Seg" + i,
                        MissionState = MissionStateEnum.MI,
                        OnlineStatus = true,
                        PisBasicPackageVersion = "1.0.0.0",
                        LmtPackageVersion = "1.0.0.0",
                        PisBaselineData = new PisBaseline()
                        {
                            FutureVersionPisInfotainmentOut = "1.0.0.0",
                            FutureVersionLmtOut = "1.0.0.0",
                            FutureActivationDateOut = "1.0.0.0",
                            FutureExpirationDateOut = "1.0.0.0",
                            FutureValidOut = "1.0.0.0",
                            FutureVersionOut = "1.0.0.0",
                            FutureVersionPisBaseOut = "1.0.0.0",
                            FutureVersionPisMissionOut = "1.0.0.0",
                            CurrentVersionLmtOut = "1.0.0.0",
                            CurrentExpirationDateOut = "1.0.0.0",
                            CurrentVersionPisMissionOut = "1.0.0.0",
                            CurrentVersionPisInfotainmentOut = "1.0.0.0",
                            CurrentVersionOut = "1.0.0.0",
                            CurrentVersionPisBaseOut = "1.0.0.0",
                            CurrentForcedOut = "1.0.0.0",
                            CurrentValidOut = "1.0.0.0",
                            ArchivedValidOut = "1.0.0.0",
                            ArchivedVersionOut = "1.0.0.0",
                            ArchivedVersionPisBaseOut = "1.0.0.0",
                            ArchivedVersionPisMissionOut = "1.0.0.0",
                            ArchivedVersionPisInfotainmentOut = "1.0.0.0",
                            ArchivedVersionLmtOut = "1.0.0.0"
                        }
                    });

                    string systemId = "Train" + i;
                    ServiceInfo serviceInfo = new ServiceInfo(
                        (ushort)eServiceID.eSrvSIF_RealTimeServer,
                        "RealTimeServer",
                        (ushort)i,
                        (ushort)i,
                        true,
                        "localhost",
                        "1908",
                        "1908",
                        (ushort)(65000 + i));

                    serviceInfoList.Add(serviceInfo);

                    TrainServerSimulator.RealTimeTrainService train = new TrainServerSimulator.RealTimeTrainService(serviceInfo.ServiceIPAddress, serviceInfo.ServicePortNumber);
                    trainList.Add(train);
                    train.ExpectedMissionCode = missionCode;
                    train.Start();

                    this._train2groundClientMock.Setup(foo => foo.GetAvailableServiceData(systemId, (int)eServiceID.eSrvSIF_RealTimeServer, out serviceInfo));
                }

                RTPISDataStoreEventArgs eventArgs = new RTPISDataStoreEventArgs(missionCode, null);
                RealTimeInformationType newRealTimeInformationType = new RealTimeInformationType();

                this._rtpisDataStore.Setup(foo => foo.GetMissionRealTimeInformation(missionCode)).Returns(newRealTimeInformationType);
                this._train2groundClientMock.Setup(foo => foo.GetAvailableElementDataListByMissionCode(missionCode, out elementList));

                try
                {
                    this._rtpisDataStore.Raise(m => m.Changed -= null, eventArgs);
                }
                finally
                {
                    for (int i = 0; i < numberOfElements && i < trainList.Count; i++)
                    {
                        trainList[i].Stop();
                    }
                }

                this._train2groundClientMock.Verify(foo => foo.GetAvailableElementDataListByMissionCode(missionCode, out elementList), Times.Once());

                for (int i = 0; i < numberOfElements; i++)
                {
                    ServiceInfo serviceInfo = serviceInfoList[i];
                    Assert.IsNotNull(trainList[i].LastMissionRealTimeRequest, "RealTime mission information not updated on train {0}.", i);
                    this._train2groundClientMock.Verify(foo => foo.GetAvailableServiceData("Train" + i, (int)eServiceID.eSrvSIF_RealTimeServer, out serviceInfo), Times.Once());
                }
            }
            finally
            {
                foreach (var train in trainList)
                {
                    train.Dispose();
                }
            }
        }
        #endregion
	}
}
