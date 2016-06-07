//---------------------------------------------------------------------------------------------------
// <copyright file="RealTimeTests.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2014.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Moq;
using NUnit.Framework;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.RealTime;
using PIS.Ground.Core;

/// Test classes for RealTime Service.
///
namespace PIS.Ground.RealTimeTests
{
	/// <summary>RealTime service test class.</summary>
	[TestFixture]
	public class RealTimeServiceTests
	{
		#region attributes

		/// <summary>The mission with all data string.</summary>
		private const string MissionWithAllDataStr = "MissionWithAllDataStr";

		/// <summary>The mission with data missing string.</summary>
		private const string MissionWithDataMissingStr = "MissionWithDataMissingStr";

		/// <summary>The Database for URBAN tests.</summary>
		private string _urbanDB = "LMTURBAN.db";

		/// <summary>The Database for SIVENG test.</summary>
		private string _sivengDB = "LMT2N2.db";

		/// <summary>Full pathname of the db source folder.</summary>
		private string _dbSourcePath = string.Empty;

		/// <summary>Full pathname of the execution folder.</summary>
		private string _dbWorkingPath = string.Empty;

		/// <summary>The full path to Database for URBAN tests.</summary>
		private string _urbanDBPath = string.Empty;

		/// <summary>The full path to Database for SIVENG test.</summary>
		private string _sivengDBPath = string.Empty;

		/// <summary>The train 2ground client mock.</summary>
		private Mock<IT2GManager> _train2groundClientMock;

		/// <summary>The session manager mock.</summary>
		private Mock<ISessionManagerExtended> _sessionManagerMock;

        /// <summary>The notification sender mock.</summary>
        private Mock<INotificationSender> _notificationSenderMock;

		/// <summary>The request processor mock.</summary>
		private Mock<IRequestProcessor> _requestProcessorMock;

		/// <summary>The RealTimeService instance.</summary>
		private IRealTimeService _rts;

		/// <summary>The remote data store factory.</summary>
		private Mock<IRemoteDataStoreFactory> _remoteDataStoreFactory;

		/// <summary>The rtpis data store.</summary>
		private Mock<IRTPISDataStore> _rtpisDataStore;

		#endregion

		#region Tests management

		/// <summary>Initializes a new instance of the RealTimeServiceTests class.</summary>
		public RealTimeServiceTests()
		{
			// Nothing to do
		}

		/// <summary>Setups called before each test to initialize variables.</summary>
		[SetUp]
		public void Setup()
		{
			this._dbSourcePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty) + "\\..\\..\\..\\GroundCoreTests\\PackageAccess\\";
			this._dbWorkingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty) + "\\pool\\";
			this._urbanDBPath = _dbSourcePath + _urbanDB;
			this._sivengDBPath = _dbSourcePath + _sivengDB;

            this._train2groundClientMock = new Mock<IT2GManager>();
			this._sessionManagerMock = new Mock<ISessionManagerExtended>();
            this._notificationSenderMock = new Mock<INotificationSender>();
			this._requestProcessorMock = new Mock<IRequestProcessor>();
			this._remoteDataStoreFactory = new Mock<IRemoteDataStoreFactory>();
			this._rtpisDataStore = new Mock <IRTPISDataStore>();

			// callback registration
			this._train2groundClientMock.Setup(
                x => x.SubscribeToElementChangeNotification(
					It.IsAny<string>(),
					It.IsAny<EventHandler<ElementEventArgs>>()));

			RealTimeService.Initialize(
				this._train2groundClientMock.Object,
				this._sessionManagerMock.Object,
                this._notificationSenderMock.Object,
				this._requestProcessorMock.Object,
				this._remoteDataStoreFactory.Object,
				this._rtpisDataStore.Object);

			this._rts = new RealTimeServiceTested();

			if (!Directory.Exists(_dbWorkingPath))
			{
				Directory.CreateDirectory(_dbWorkingPath);
			}
		}

		/// <summary>Tear down called after each test to clean.</summary>
		[TearDown]
		public void TearDown()
		{
			if (Directory.Exists(_dbWorkingPath))
			{
				string filepath = Path.Combine(_dbWorkingPath, "file.db");
				if (File.Exists(filepath))
				{
					File.SetAttributes(filepath, FileAttributes.Normal);
					File.Delete(filepath);
				}

				Directory.Delete(_dbWorkingPath, true);
			}
		}

		/// <summary>Initialises the train element.</summary>
		/// <returns>An instance of AvailableElementData for testing purpose.</returns>
		private AvailableElementData InitTrainElement()
		{
			AvailableElementData element = new AvailableElementData();
			element.ElementNumber = "TRAIN-1";
			element.PisBasicPackageVersion = "1.0.0.0";
			element.LmtPackageVersion = "1.0.0.0";
			element.PisBaselineData = new PisBaseline();
			element.PisBaselineData.CurrentVersionLmtOut = "1.0.0.0";
			element.OnlineStatus = true;
			return element;
		}

		/// <summary>Initialises the service information.</summary>
		/// <returns>ServiceInfo struct filled with dummy data.</returns>
		private ServiceInfo InitServiceInfo()
		{
            return new ServiceInfo(123, "ServiceName", 100, 55, true, "ServiceIPAddress", "AID", "SID", 1500);			
		}

		/// <summary>Initialises the urban station list.</summary>
		/// <param name="missionCode">The mission code.</param>
		/// <returns>List of station id.</returns>
		private List<string> InitUrbanStationList(string missionCode)
		{
			List<string> refList = new List<string>();

			if (missionCode == null)
			{
				refList.Add("132");
				refList.Add("146");
				refList.Add("222");
				refList.Add("224");
				refList.Add("228");
				refList.Add("230");
				refList.Add("232");
				refList.Add("234");
				refList.Add("236");
				refList.Add("238");
				refList.Add("242");
				refList.Add("244");
				refList.Add("248");
				refList.Add("250");
				refList.Add("252");
				refList.Add("254");
				refList.Add("256");
				refList.Add("258");
				refList.Add("262");
				refList.Add("264");
				refList.Add("266");
				refList.Add("268");
				refList.Add("270");
				refList.Add("272");
				refList.Add("274");
				refList.Add("276");
				refList.Add("278");
				refList.Add("280");
				refList.Add("282");
				refList.Add("286");
				refList.Add("288");
			}
			else
			{
				refList.Add("222");
				refList.Add("224");
				refList.Add("228");
				refList.Add("230");
				refList.Add("232");
				refList.Add("234");
				refList.Add("236");
				refList.Add("238");
				refList.Add("242");
				refList.Add("244");
				refList.Add("132");
				refList.Add("248");
				refList.Add("250");
				refList.Add("252");
				refList.Add("254");
				refList.Add("256");
				refList.Add("258");
				refList.Add("146");
				refList.Add("262");
				refList.Add("264");
				refList.Add("266");
				refList.Add("268");
				refList.Add("270");
				refList.Add("272");
				refList.Add("274");
				refList.Add("276");
				refList.Add("278");
				refList.Add("280");
			}

			return refList;
		}

		/// <summary>Initialises the station list information.</summary>
		/// <param name="stationList">List of stations.</param>
		/// <returns>List of station information.</returns>
		private List<RealTimeStationStatusType> InitStationListInformation(List<string> stationList)
		{
			List<RealTimeStationStatusType> result = null;

			if (stationList != null)
			{
				result = new List<RealTimeStationStatusType>();

				foreach (var stationid in stationList)
				{
					RealTimeStationStatusType status = new RealTimeStationStatusType();
					status.StationID = stationid;
					result.Add(status);
				}
			}

			return result;
		}

		/// <summary>Initialises the station list information.</summary>
		/// <param name="start">The start.</param>
		/// <param name="count">Number of.</param>
		/// <returns>List of station information.</returns>
		private List<RealTimeStationInformationType> InitStationListInformation(int start, int count)
		{
			List<RealTimeStationInformationType> returnList = new List<RealTimeStationInformationType>();

			for (int i = start; i < count; i++)
			{
				RealTimeStationInformationType newInfo = new RealTimeStationInformationType();
				newInfo.StationCode = "Station" + i.ToString();
				newInfo.StationData = new RealTimeStationDataType();
				returnList.Add(newInfo);
			}

			return returnList;
		}

		/// <summary>Initialises the station list information.</summary>
		/// <param name="start">The start.</param>
		/// <param name="count">Number of.</param>
		/// <returns>List of station information.</returns>
		private List<RealTimeStationStatusType> InitStationResultListInformation(int start, int count)
		{
			List<RealTimeStationStatusType> returnList = new List<RealTimeStationStatusType>();

			for (int i = start; i < count; i++)
			{
				RealTimeStationStatusType newInfo = new RealTimeStationStatusType();
				newInfo.StationID = "Station" + i.ToString();
				returnList.Add(newInfo);
			}

			return returnList;
		}

		#endregion

		#region Tests

		#region GetAvailableElementList

		/// <summary>Gets available element list test case with invalid session id result.</summary>
		[Test]
		public void GetAvailableElementListInvalidSessionId()
		{
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);

			RealTimeAvailableElementListResult rtr = this._rts.GetAvailableElementList(Guid.NewGuid());
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidSessionId, rtr.ResultCode);

			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			this._train2groundClientMock.Verify(w => w.GetAvailableElementDataList(out elementList), Times.Never());
		}

		/// <summary>
		/// Gets available element list test case with element list not available result.
		/// </summary>
		[Test]
		public void GetAvailableElementListNotAvailable()
		{
			ElementList<AvailableElementData> elementList;
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eElementNotFound;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._train2groundClientMock.Setup(y => y.GetAvailableElementDataList(out elementList)).Returns(returns);

			RealTimeAvailableElementListResult rtr = this._rts.GetAvailableElementList(Guid.NewGuid());
			Assert.AreEqual(RealTimeServiceErrorEnum.ElementListNotAvailable, rtr.ResultCode);

			this._train2groundClientMock.Verify(w => w.GetAvailableElementDataList(out elementList), Times.Once());
		}

		/// <summary>Gets available element list test case with Request accepted result.</summary>
		[Test]
		public void GetAvailableElementListRequestAcceptedWithoutElement()
		{
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);

			RealTimeAvailableElementListResult rtr = this._rts.GetAvailableElementList(Guid.NewGuid());
			Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, rtr.ResultCode);

			this._train2groundClientMock.Verify(w => w.GetAvailableElementDataList(out elementList), Times.Once());
		}

		/// <summary>Gets available element list test case with Request accepted result.</summary>
		[Test]
		public void GetAvailableElementListRequestAcceptedWithElement()
		{
			AvailableElementData element = this.InitTrainElement();
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>() { element };
			ServiceInfo serviceInfo = this.InitServiceInfo();
			
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
            this._train2groundClientMock.Setup(x => x.GetAvailableServiceData(
				element.ElementNumber,
				(int)eServiceID.eSrvSIF_RealTimeServer,
				out serviceInfo)).Returns(returns);
			
			RealTimeAvailableElementListResult rtr = this._rts.GetAvailableElementList(Guid.NewGuid());

			this._train2groundClientMock.Verify(w => w.GetAvailableElementDataList(out elementList), Times.Once());
			Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, rtr.ResultCode);
			Assert.AreEqual(1, rtr.ElementList.Count);
			Assert.AreEqual(element, rtr.ElementList[0]);
		}

		#endregion

		#region RetrieveStationList

		/// <summary>Retrieves station list invalid session identifier.</summary>
		[Test]
		public void RetrieveStationListInvalidSessionId()
		{
			string missionCode = null;
			string elementId = "TRAIN-1";
			Guid validGuid = Guid.NewGuid();
			
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeRetrieveStationListResult rtr = this._rts.RetrieveStationList(Guid.NewGuid(), missionCode, elementId);

			AvailableElementData elementData = new AvailableElementData();
			this._train2groundClientMock.Verify(w => w.GetAvailableElementDataByElementNumber(elementId, out elementData), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidSessionId, rtr.ResultCode);
			Assert.IsNull(rtr.MissionCode);
			Assert.AreEqual(elementId, rtr.ElementID);
			Assert.IsNull(rtr.StationList);
		}

		/// <summary>Retrieves station list 2 g serer offline.</summary>
		[Test]
		public void RetrieveStationListT2GSererOffline()
		{
			string missionCode = null;
			string elementId = "TRAIN-1";
			AvailableElementData elementData = null;
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eT2GServerOffline;
			Guid validGuid = Guid.NewGuid();
			
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(elementId, out elementData)).Returns(returns);

			RealTimeRetrieveStationListResult rtr = this._rts.RetrieveStationList(Guid.NewGuid(), missionCode, elementId);

			this._train2groundClientMock.Verify(w => w.GetAvailableElementDataByElementNumber(elementId, out elementData), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.T2GServerOffline, rtr.ResultCode);
			Assert.IsNull(rtr.MissionCode);
			Assert.AreEqual(elementId, rtr.ElementID);
			Assert.IsNull(rtr.StationList);
		}

		/// <summary>Retrieves station list element not found.</summary>
		[Test]
		public void RetrieveStationListElementNotFound()
		{
			string missionCode = null;
			string elementId = "TRAIN-1";
			AvailableElementData elementData = null;
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eElementNotFound;
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(elementId, out elementData)).Returns(returns);

			RealTimeRetrieveStationListResult rtr = this._rts.RetrieveStationList(Guid.NewGuid(), missionCode, elementId);

			this._train2groundClientMock.Verify(w => w.GetAvailableElementDataByElementNumber(elementId, out elementData), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidElementId, rtr.ResultCode);
			Assert.IsNull(rtr.MissionCode);
			Assert.AreEqual(elementId, rtr.ElementID);
			Assert.IsNull(rtr.StationList);
		}

		/// <summary>Retrieves station list error with lmtdb.</summary>
		[Test]
		public void RetrieveStationListErrorWithLMTDB()
		{
			string missionCode = null;
			AvailableElementData elementData = InitTrainElement();
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(elementData.ElementNumber, out elementData)).Returns(returns);
			this._remoteDataStoreFactory.Setup(z => z.GetRemoteDataStoreInstance()).Returns(new RemoteDataStoreSimulator.RemoteDataStore("NotValidPath"));

			RealTimeRetrieveStationListResult rtr = this._rts.RetrieveStationList(Guid.NewGuid(), missionCode, elementData.ElementNumber);

			this._train2groundClientMock.Verify(w => w.GetAvailableElementDataByElementNumber(elementData.ElementNumber, out elementData), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorRemoteDatastoreUnavailable, rtr.ResultCode);
			Assert.IsNull(rtr.MissionCode);
			Assert.AreEqual(elementData.ElementNumber, rtr.ElementID);
			Assert.IsNull(rtr.StationList);
		}

		/// <summary>Retrieves station list success without mission code.</summary>
		[Test]
		public void RetrieveStationListSuccessWithoutMissionCode()
		{
			((RealTimeServiceTested)this._rts).PlateformType = CommonConfiguration.PlatformTypeEnum.URBAN;
			File.Copy(_urbanDBPath, Path.Combine(_dbWorkingPath, "file.db") );

			string missionCode = null;
			AvailableElementData elementData = InitTrainElement();
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid validGuid = Guid.NewGuid();
			List<string> refList = InitUrbanStationList(null);

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(elementData.ElementNumber, out elementData)).Returns(returns);
			this._remoteDataStoreFactory.Setup(z => z.GetRemoteDataStoreInstance()).Returns(new RemoteDataStoreSimulator.RemoteDataStore(this._dbWorkingPath));

			RealTimeRetrieveStationListResult rtr = this._rts.RetrieveStationList(Guid.NewGuid(), missionCode, elementData.ElementNumber);

			this._train2groundClientMock.Verify(w => w.GetAvailableElementDataByElementNumber(elementData.ElementNumber, out elementData), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, rtr.ResultCode);
			Assert.IsNull(rtr.MissionCode);
			Assert.AreEqual(elementData.ElementNumber, rtr.ElementID);
			Assert.AreEqual(refList, rtr.StationList);
		}

		/// <summary>Retrieves station list success with mission code.</summary>
		[Test]
		public void RetrieveStationListSuccessWithMissionCode()
		{
			((RealTimeServiceTested)this._rts).PlateformType = CommonConfiguration.PlatformTypeEnum.URBAN;
			File.Copy(_urbanDBPath, Path.Combine(_dbWorkingPath, "file.db"));

			string missionCode = "231";
			AvailableElementData elementData = InitTrainElement();
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid validGuid = Guid.NewGuid();

			List<string> refList = InitUrbanStationList(missionCode);

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(elementData.ElementNumber, out elementData)).Returns(returns);
			this._remoteDataStoreFactory.Setup(z => z.GetRemoteDataStoreInstance()).Returns(new RemoteDataStoreSimulator.RemoteDataStore(this._dbWorkingPath));

			RealTimeRetrieveStationListResult rtr = this._rts.RetrieveStationList(Guid.NewGuid(), missionCode, elementData.ElementNumber);

			this._train2groundClientMock.Verify(w => w.GetAvailableElementDataByElementNumber(elementData.ElementNumber, out elementData), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.AreEqual(elementData.ElementNumber, rtr.ElementID);
			Assert.AreEqual(refList, rtr.StationList);
		}

		#endregion

		#region GetMissionRealTimeInformation

		/// <summary>Gets mission real time information invalid session identifier.</summary>
		[Test]
		public void GetMissionRealTimeInformationInvalidSessionId()
		{
			string missionCode = null;
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeGetMissionRealTimeInformationResult rtr = this._rts.GetMissionRealTimeInformation(Guid.NewGuid(), missionCode);

			this._rtpisDataStore.Verify(w => w.GetMissionRealTimeInformation(missionCode), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidSessionId, rtr.ResultCode);
			Assert.IsNull(rtr.MissionCode);
			Assert.IsNull(rtr.InformationStructure);
		}

		/// <summary>Gets mission real time information invalid mission code null.</summary>
		[Test]
		public void GetMissionRealTimeInformationInvalidMissionCodeNull()
		{
			string missionCode = null;
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeGetMissionRealTimeInformationResult rtr = this._rts.GetMissionRealTimeInformation(Guid.NewGuid(), missionCode);

			this._rtpisDataStore.Verify(w => w.GetMissionRealTimeInformation(missionCode), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidMissionCode, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.IsNull(rtr.InformationStructure);
		}

		/// <summary>Gets mission real time information invalid mission code empty.</summary>
		[Test]
		public void GetMissionRealTimeInformationInvalidMissionCodeEmpty()
		{
			string missionCode = string.Empty;
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeGetMissionRealTimeInformationResult rtr = this._rts.GetMissionRealTimeInformation(Guid.NewGuid(), missionCode);

			this._rtpisDataStore.Verify(w => w.GetMissionRealTimeInformation(missionCode), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidMissionCode, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.IsNull(rtr.InformationStructure);
		}

		/// <summary>Gets mission real time information invalid mission unknown.</summary>
		[Test]
		public void GetMissionRealTimeInformationInvalidMissionUnknown()
		{
			string missionCode = "NotARealMission";
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._rtpisDataStore.Setup(y => y.GetMissionRealTimeInformation(missionCode)).Returns((RealTimeInformationType)null);

			RealTimeGetMissionRealTimeInformationResult rtr = this._rts.GetMissionRealTimeInformation(Guid.NewGuid(), missionCode);

			this._rtpisDataStore.Verify(w => w.GetMissionRealTimeInformation(missionCode), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidMissionCode, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.IsNull(rtr.InformationStructure);
		}

        /// <summary>Gets mission real time information no rtpis data.</summary>
        [Test]
        public void GetMissionRealTimeInformationNoRtpisData()
        {
            string missionCode = "NotARealMission";
            Guid validGuid = Guid.NewGuid();
            RealTimeInformationType missionData = new RealTimeInformationType();
            missionData.MissionDelay = null;
            missionData.MissionWeather = null;

            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
            this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
            this._rtpisDataStore.Setup(y => y.GetMissionRealTimeInformation(missionCode)).Returns(missionData);

            RealTimeGetMissionRealTimeInformationResult rtr = this._rts.GetMissionRealTimeInformation(Guid.NewGuid(), missionCode);

            this._rtpisDataStore.Verify(w => w.GetMissionRealTimeInformation(missionCode), Times.Once());

            Assert.AreEqual(validGuid, rtr.RequestId);
            Assert.AreEqual(RealTimeServiceErrorEnum.ErrorNoRtpisData, rtr.ResultCode);
            Assert.AreEqual(missionCode, rtr.MissionCode);
            Assert.IsNotNull(rtr.InformationStructure);
            Assert.IsNull(rtr.InformationStructure.MissionDelay);
            Assert.IsNull(rtr.InformationStructure.MissionWeather);
        }

		/// <summary>Gets mission real time information no delay data.</summary>
		[Test]
		public void GetMissionRealTimeInformationNoDelayData()
		{
			string missionCode = "NotARealMission";
			RealTimeInformationType returnValue = new RealTimeInformationType();
			returnValue.MissionDelay = null;
			returnValue.MissionWeather = new RealTimeWeatherType();
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._rtpisDataStore.Setup(y => y.GetMissionRealTimeInformation(missionCode)).Returns(returnValue);

			RealTimeGetMissionRealTimeInformationResult rtr = this._rts.GetMissionRealTimeInformation(Guid.NewGuid(), missionCode);

			this._rtpisDataStore.Verify(w => w.GetMissionRealTimeInformation(missionCode), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.InfoNoDelayData, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.AreEqual(returnValue, rtr.InformationStructure);
		}

		/// <summary>Gets mission real time information no weather data.</summary>
		[Test]
		public void GetMissionRealTimeInformationNoWeatherData()
		{
			string missionCode = "NotARealMission";
			RealTimeInformationType returnValue = new RealTimeInformationType();
			returnValue.MissionDelay = new RealTimeDelayType();
			returnValue.MissionWeather = null;
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._rtpisDataStore.Setup(y => y.GetMissionRealTimeInformation(missionCode)).Returns(returnValue);

			RealTimeGetMissionRealTimeInformationResult rtr = this._rts.GetMissionRealTimeInformation(Guid.NewGuid(), missionCode);

			this._rtpisDataStore.Verify(w => w.GetMissionRealTimeInformation(missionCode), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.InfoNoWeatherData, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.AreEqual(returnValue, rtr.InformationStructure);
		}

		/// <summary>Gets mission real time information request accepted.</summary>
		[Test]
		public void GetMissionRealTimeInformationRequestAccepted()
		{
			string missionCode = "NotARealMission";
			RealTimeInformationType returnValue = new RealTimeInformationType();
			returnValue.MissionDelay = new RealTimeDelayType();
			returnValue.MissionWeather = new RealTimeWeatherType();
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._rtpisDataStore.Setup(y => y.GetMissionRealTimeInformation(missionCode)).Returns(returnValue);

			RealTimeGetMissionRealTimeInformationResult rtr = this._rts.GetMissionRealTimeInformation(Guid.NewGuid(), missionCode);

			this._rtpisDataStore.Verify(w => w.GetMissionRealTimeInformation(missionCode), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.AreEqual(returnValue, rtr.InformationStructure);
		}

		#endregion

		#region GetStationRealTimeInformation

		/// <summary>Gets station real time information invalid session identifier.</summary>
		[Test]
		public void GetStationRealTimeInformationInvalidSessionId()
		{
			string missionCode = null;
			List<string> stationList = null;
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeGetStationRealTimeInformationResult rtr = this._rts.GetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(w => w.GetStationRealTimeInformation(missionCode, stationList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidSessionId, rtr.ResultCode);
			Assert.IsNull(rtr.StationStatusList);
		}

		/// <summary>Gets station real time information invalid mission code null.</summary>
		[Test]
		public void GetStationRealTimeInformationInvalidMissionCodeNull()
		{
			string missionCode = null;
			List<string> stationList = null;
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeGetStationRealTimeInformationResult rtr = this._rts.GetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(w => w.GetStationRealTimeInformation(missionCode, stationList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidMissionCode, rtr.ResultCode);
			Assert.IsNull(rtr.StationStatusList);
		}

		/// <summary>Gets station real time information invalid mission code empty.</summary>
		[Test]
		public void GetStationRealTimeInformationInvalidMissionCodeEmpty()
		{
			string missionCode = string.Empty;
			List<string> stationList = null;
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeGetStationRealTimeInformationResult rtr = this._rts.GetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(w => w.GetStationRealTimeInformation(missionCode, stationList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidMissionCode, rtr.ResultCode);
			Assert.IsNull(rtr.StationStatusList);
		}

		/// <summary>Gets station real time information invalid mission unknown.</summary>
		[Test]
		public void GetStationRealTimeInformationInvalidMissionUnknown()
		{
			string missionCode = "NotARealMission";
			List<string> stationList = null;
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeGetStationRealTimeInformationResult rtr = this._rts.GetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(w => w.GetStationRealTimeInformation(missionCode, stationList), Times.Once());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidMissionCode, rtr.ResultCode);
			Assert.IsNull(rtr.StationStatusList);
		}

		/// <summary>Gets station real time information station list not provided.</summary>
		[Test]
		public void GetStationRealTimeInformationStationListNotProvided()
		{
			string missionCode = "NotARealMission";
			List<string> stationList = null;
			List<RealTimeStationStatusType> returnValue = new List<RealTimeStationStatusType>();
			returnValue.Add(new RealTimeStationStatusType());
			Guid validGuid = Guid.NewGuid();
			
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._rtpisDataStore.Setup(y => y.GetStationRealTimeInformation(missionCode, stationList)).Returns(returnValue);

			RealTimeGetStationRealTimeInformationResult rtr = this._rts.GetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(w => w.GetStationRealTimeInformation(missionCode, stationList), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, rtr.ResultCode);
			Assert.AreEqual(returnValue, rtr.StationStatusList);
		}

		/// <summary>Gets station real time information station list provided.</summary>
		[Test]
		public void GetStationRealTimeInformationStationListProvided()
		{
			string missionCode = "NotARealMission";
			List<string> stationList = new List<string>();
			stationList.Add("Station1");
			List<RealTimeStationStatusType> returnValue = new List<RealTimeStationStatusType>();
			returnValue.Add(new RealTimeStationStatusType());
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._rtpisDataStore.Setup(y => y.GetStationRealTimeInformation(missionCode, stationList)).Returns(returnValue);

			RealTimeGetStationRealTimeInformationResult rtr = this._rts.GetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(w => w.GetStationRealTimeInformation(missionCode, stationList), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, rtr.ResultCode);
			Assert.AreEqual(returnValue, rtr.StationStatusList);
		}

		#endregion

		#region SetMissionRealTimeInformation

		/// <summary>Sets mission real time information invalid session identifier.</summary>
		[Test]
		public void SetMissionRealTimeInformationInvalidSessionId()
		{
			string missionCode = null;
			RealTimeDelayType delay = null;
			RealTimeWeatherType weather = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);

			RealTimeSetMissionRealTimeInformationResult rtr = this._rts.SetMissionRealTimeInformation(Guid.NewGuid(), missionCode, delay, weather);

			this._rtpisDataStore.Verify(x => x.SetMissionRealTimeInformation(missionCode, delay, weather), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidSessionId, rtr.ResultCode);
		}

		/// <summary>Sets mission real time information invalid mission code null.</summary>
		[Test]
		public void SetMissionRealTimeInformationInvalidMissionCodeNull()
		{
			string missionCode = null;
			Guid validGuid = Guid.NewGuid();
			RealTimeDelayType delay = null;
			RealTimeWeatherType weather = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeSetMissionRealTimeInformationResult rtr = this._rts.SetMissionRealTimeInformation(Guid.NewGuid(), missionCode, delay, weather);

			this._rtpisDataStore.Verify(w => w.GetMissionRealTimeInformation(missionCode), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidMissionCode, rtr.ResultCode);
		}

		/// <summary>Sets mission real time information invalid mission code empty.</summary>
		[Test]
		public void SetMissionRealTimeInformationInvalidMissionCodeEmpty()
		{
			string missionCode = string.Empty;
			Guid validGuid = Guid.NewGuid();
			RealTimeDelayType delay = null;
			RealTimeWeatherType weather = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeSetMissionRealTimeInformationResult rtr = this._rts.SetMissionRealTimeInformation(Guid.NewGuid(), missionCode, delay, weather);

			this._rtpisDataStore.Verify(w => w.GetMissionRealTimeInformation(missionCode), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidMissionCode, rtr.ResultCode);
		}

		/// <summary>Sets mission real time information no rtpis.</summary>
		[Test]
		public void SetMissionRealTimeInformationNoRTPIS()
		{
			string missionCode = "DummyMissionCode";
			RealTimeDelayType delay = null;
			RealTimeWeatherType weather = null;
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeSetMissionRealTimeInformationResult rtr = this._rts.SetMissionRealTimeInformation(Guid.NewGuid(), missionCode, delay, weather);

			this._rtpisDataStore.Verify(x => x.SetMissionRealTimeInformation(missionCode, delay, weather), Times.Never());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorNoRtpisData, rtr.ResultCode);
		}

		/// <summary>Sets mission real time information no delay data.</summary>
		[Test]
		public void SetMissionRealTimeInformationNoDelayData()
		{
			string missionCode = "DummyMissionCode";
			RealTimeDelayType delay = null;
			RealTimeWeatherType weather = new RealTimeWeatherType();
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			
			RealTimeSetMissionRealTimeInformationResult rtr = this._rts.SetMissionRealTimeInformation(Guid.NewGuid(), missionCode, delay, weather);

			this._rtpisDataStore.Verify(x => x.SetMissionRealTimeInformation(missionCode, delay, weather), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.InfoNoDelayData, rtr.ResultCode);
		}

		/// <summary>Sets mission real time information no weather data.</summary>
		[Test]
		public void SetMissionRealTimeInformationNoWeatherData()
		{
			string missionCode = "DummyMissionCode";
			RealTimeDelayType delay = new RealTimeDelayType();
			RealTimeWeatherType weather = null;
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeSetMissionRealTimeInformationResult rtr = this._rts.SetMissionRealTimeInformation(Guid.NewGuid(), missionCode, delay, weather);

			this._rtpisDataStore.Verify(x => x.SetMissionRealTimeInformation(missionCode, delay, weather), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.InfoNoWeatherData, rtr.ResultCode);
		}

		/// <summary>Sets mission real time information request accepted.</summary>
		[Test]
		public void SetMissionRealTimeInformationRequestAccepted()
		{
			string missionCode = "DummyMissionCode";
			RealTimeDelayType delay = new RealTimeDelayType();
			RealTimeWeatherType weather = new RealTimeWeatherType();
			Guid validGuid = Guid.NewGuid();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeSetMissionRealTimeInformationResult rtr = this._rts.SetMissionRealTimeInformation(Guid.NewGuid(), missionCode, delay, weather);

			this._rtpisDataStore.Verify(x => x.SetMissionRealTimeInformation(missionCode, delay, weather), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, rtr.ResultCode);
		}

		#endregion

		#region SetStationRealTimeInformation

		/// <summary>Sets station real time information invalid session identifier.</summary>
		[Test]
		public void SetStationRealTimeInformationInvalidSessionId()
		{
			string missionCode = null;
			List<RealTimeStationInformationType> stationInformationList = null;
			Guid validGuid = Guid.NewGuid();
			List<RealTimeStationResultType> stationResultsList = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeSetStationRealTimeInformationResult rtr = this._rts.SetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationInformationList);

			this._rtpisDataStore.Verify(x => x.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidSessionId, rtr.ResultCode);
		}

		/// <summary>Sets station real time information invalid mission code null.</summary>
		[Test]
		public void SetStationRealTimeInformationInvalidMissionCodeNull()
		{
			string missionCode = null;
			List<RealTimeStationInformationType> stationInformationList = null;
			Guid validGuid = Guid.NewGuid();
			List<RealTimeStationResultType> stationResultsList = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeSetStationRealTimeInformationResult rtr = this._rts.SetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationInformationList);

			this._rtpisDataStore.Verify(x => x.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidMissionCode, rtr.ResultCode);
		}

		/// <summary>Sets station real time information invalid mission code empty.</summary>
		[Test]
		public void SetStationRealTimeInformationInvalidMissionCodeEmpty()
		{
			string missionCode = string.Empty;
			List<RealTimeStationInformationType> stationInformationList = null;
			Guid validGuid = Guid.NewGuid();
			List<RealTimeStationResultType> stationResultsList = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeSetStationRealTimeInformationResult rtr = this._rts.SetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationInformationList);

			this._rtpisDataStore.Verify(x => x.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidMissionCode, rtr.ResultCode);
		}

		/// <summary>Sets station real time information list null.</summary>
		[Test]
		public void SetStationRealTimeInformationInformationListNull()
		{
			string missionCode = "DummyMission";
			List<RealTimeStationInformationType> stationInformationList = null;
			Guid validGuid = Guid.NewGuid();
			List<RealTimeStationResultType> stationResultsList = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeSetStationRealTimeInformationResult rtr = this._rts.SetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationInformationList);

			this._rtpisDataStore.Verify(x => x.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorNoRtpisData, rtr.ResultCode);
		}

		/// <summary>Sets station real time information list empty.</summary>
		[Test]
		public void SetStationRealTimeInformationInformationListEmpty()
		{
			string missionCode = "DummyMission";
			List<RealTimeStationInformationType> stationInformationList = null;
			Guid validGuid = Guid.NewGuid();
			List<RealTimeStationResultType> stationResultsList = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeSetStationRealTimeInformationResult rtr = this._rts.SetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationInformationList);

			this._rtpisDataStore.Verify(x => x.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorNoRtpisData, rtr.ResultCode);
		}

		/// <summary>Sets station real time information list limit exceeded one request.</summary>
		[Test]
		public void SetStationRealTimeInformationInformationListLimitExceededOneRequest()
		{
			string missionCode = "DummyMission";
			List<RealTimeStationInformationType> stationInformationList = InitStationListInformation(0, 70);
			Guid validGuid = Guid.NewGuid();
			List<RealTimeStationResultType> stationResultsList = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeSetStationRealTimeInformationResult rtr = this._rts.SetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationInformationList);

			this._rtpisDataStore.Verify(x => x.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorStationListLimitExcedeed, rtr.ResultCode);
		}

		/// <summary>Sets station real time information list limit exceeded multiple requests.</summary>
		[Test]
		public void SetStationRealTimeInformationInformationListLimitExceededMultipleRequests()
		{
			string missionCode = "DummyMission";
			List<RealTimeStationStatusType> stationInformationResultList = InitStationResultListInformation(0, 20);
			List<RealTimeStationInformationType> stationInformationInputList = InitStationListInformation(15, 65);
			Guid validGuid = Guid.NewGuid();
			List<RealTimeStationResultType> stationResultsList = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._rtpisDataStore.Setup(x => x.GetStationRealTimeInformation(missionCode, It.IsAny<List<string>>())).Returns(stationInformationResultList);

			RealTimeSetStationRealTimeInformationResult rtr = this._rts.SetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationInformationInputList);

			this._rtpisDataStore.Verify(x => x.GetStationRealTimeInformation(missionCode, It.IsAny<List<string>>()), Times.Once());
			this._rtpisDataStore.Verify(x => x.SetStationRealTimeInformation(missionCode, stationInformationInputList, out stationResultsList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorStationListLimitExcedeed, rtr.ResultCode);
		}

		/// <summary>Sets station real time information list request accepted.</summary>
		[Test]
		public void SetStationRealTimeInformationInformationListRequestAccepted()
		{
			string missionCode = "DummyMission";
			List<RealTimeStationStatusType> stationInformationResultList = InitStationResultListInformation(0, 1);
			List<RealTimeStationInformationType> stationInformationInputList = InitStationListInformation(1, 59);
			Guid validGuid = Guid.NewGuid();
			List<RealTimeStationResultType> stationResultsList = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._rtpisDataStore.Setup(x => x.GetStationRealTimeInformation(missionCode, It.IsAny<List<string>>())).Returns(stationInformationResultList);

			RealTimeSetStationRealTimeInformationResult rtr = this._rts.SetStationRealTimeInformation(Guid.NewGuid(), missionCode, stationInformationInputList);

			this._rtpisDataStore.Verify(x => x.GetStationRealTimeInformation(missionCode, It.IsAny<List<string>>()), Times.Once());
			this._rtpisDataStore.Verify(x => x.SetStationRealTimeInformation(missionCode, stationInformationInputList, out stationResultsList), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, rtr.ResultCode);
		}

		#endregion

		#region clear
		/// <summary>Clears the real time information invalid session identifier.</summary>
		[Test]
		public void ClearRealTimeInformationInvalidSessionId()
		{
			string missionCode = null;
			List<string> stationList = null;
			Guid validGuid = Guid.NewGuid();
			List<string> clearedStationList = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeClearRealTimeInformationResult rtr = this._rts.ClearRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(x => x.ClearRealTimeInformation(missionCode, stationList, out clearedStationList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidSessionId, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.AreEqual(clearedStationList, rtr.StationList);
		}

		/// <summary>Clears the real time information invalid mission code null.</summary>
		[Test]
		public void ClearRealTimeInformationInvalidMissionCodeNull()
		{
			string missionCode = null;
			List<string> stationList = null;
			Guid validGuid = Guid.NewGuid();
			List<string> clearedStationList = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeClearRealTimeInformationResult rtr = this._rts.ClearRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(x => x.ClearRealTimeInformation(missionCode, stationList, out clearedStationList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidMissionCode, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.AreEqual(clearedStationList, rtr.StationList);
		}

		/// <summary>Clears the real time information invalid mission code empty.</summary>
		[Test]
		public void ClearRealTimeInformationInvalidMissionCodeEmpty()
		{
			string missionCode = string.Empty;
			List<string> stationList = null;
			Guid validGuid = Guid.NewGuid();
			List<string> clearedStationList = null;

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);

			RealTimeClearRealTimeInformationResult rtr = this._rts.ClearRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(x => x.ClearRealTimeInformation(missionCode, stationList, out clearedStationList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorInvalidMissionCode, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.AreEqual(clearedStationList, rtr.StationList);
		}

		/// <summary>Clears the real time information no data.</summary>
		[Test]
		public void ClearRealTimeInformationNoData()
		{
			string missionCode = "DummyMission";
			List<string> stationList = new List<string>();
			Guid validGuid = Guid.NewGuid();
			List<string> clearedStationList = null;
			RealTimeInformationType missionData = new RealTimeInformationType();
            missionData.MissionDelay = null;
            missionData.MissionWeather = null;
            
            List<RealTimeStationStatusType> stationData = new List<RealTimeStationStatusType>();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._rtpisDataStore.Setup(x => x.GetMissionRealTimeInformation(missionCode)).Returns(missionData);
            this._rtpisDataStore.Setup(x => x.GetStationRealTimeInformation(missionCode, stationList)).Returns(stationData);

			RealTimeClearRealTimeInformationResult rtr = this._rts.ClearRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(x => x.ClearRealTimeInformation(missionCode, stationList, out clearedStationList), Times.Never());

			Assert.AreEqual(Guid.Empty, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.ErrorNoRtpisData, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.AreEqual(clearedStationList, rtr.StationList);
		}

		/// <summary>Clears the real time information station list null.</summary>
		[Test]
		public void ClearRealTimeInformationStationListNull()
		{
			string missionCode = "DummyMission";
			List<string> stationList = null;
			Guid validGuid = Guid.NewGuid();
			RealTimeInformationType missionData = new RealTimeInformationType();
            missionData.MissionDelay = new RealTimeDelayType();
            missionData.MissionWeather = new RealTimeWeatherType();

			List<string> clearedStationList = new List<string>();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._rtpisDataStore.Setup(x => x.GetMissionRealTimeInformation(missionCode)).Returns(missionData);
			this._rtpisDataStore.Setup(x => x.ClearRealTimeInformation(missionCode, stationList, out clearedStationList));

			RealTimeClearRealTimeInformationResult rtr = this._rts.ClearRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(x => x.ClearRealTimeInformation(missionCode, stationList, out clearedStationList), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.AreEqual(clearedStationList, rtr.StationList);
		}

		/// <summary>Clears the real time information station list empty.</summary>
		[Test]
		public void ClearRealTimeInformationStationListEmpty()
		{
			string missionCode = "DummyMission";
			List<string> stationList = new List<string>();
			Guid validGuid = Guid.NewGuid();
			RealTimeInformationType missionData = new RealTimeInformationType();
            missionData.MissionDelay = new RealTimeDelayType();
            missionData.MissionWeather = new RealTimeWeatherType();
			List<string> clearedStationList = new List<string>();

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._rtpisDataStore.Setup(x => x.GetMissionRealTimeInformation(missionCode)).Returns(missionData);
			this._rtpisDataStore.Setup(x => x.ClearRealTimeInformation(missionCode, stationList, out clearedStationList));            

			RealTimeClearRealTimeInformationResult rtr = this._rts.ClearRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(x => x.ClearRealTimeInformation(missionCode, stationList, out clearedStationList), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.AreEqual(clearedStationList, rtr.StationList);
		}

		/// <summary>Clears the real time information cleared list equal.</summary>
		[Test]
		public void ClearRealTimeInformationClearedListEqual()
		{
			string missionCode = "DummyMission";
			List<string> stationList = InitUrbanStationList(missionCode);
			Guid validGuid = Guid.NewGuid();
			RealTimeInformationType missionData = new RealTimeInformationType();
            missionData.MissionDelay = new RealTimeDelayType();
            missionData.MissionWeather = new RealTimeWeatherType();
			List<string> clearedStationList = InitUrbanStationList(missionCode);
			List<RealTimeStationStatusType> stationListInfo = InitStationListInformation(clearedStationList);

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._rtpisDataStore.Setup(x => x.GetMissionRealTimeInformation(missionCode)).Returns(missionData);
			this._rtpisDataStore.Setup(x => x.ClearRealTimeInformation(missionCode, stationList, out clearedStationList));
			this._rtpisDataStore.Setup(x => x.GetStationRealTimeInformation(missionCode, clearedStationList)).Returns(stationListInfo);

			RealTimeClearRealTimeInformationResult rtr = this._rts.ClearRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(x => x.ClearRealTimeInformation(missionCode, stationList, out clearedStationList), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.AreEqual(clearedStationList, rtr.StationList);
		}

		/// <summary>Clears the real time information cleared list greater.</summary>
		[Test]
		public void ClearRealTimeInformationClearedListGreater()
		{
			string missionCode = "DummyMission";
			List<string> stationList = InitUrbanStationList(null);
			Guid validGuid = Guid.NewGuid();
			RealTimeInformationType missionData = new RealTimeInformationType();
            missionData.MissionDelay = new RealTimeDelayType();
            missionData.MissionWeather = new RealTimeWeatherType();
			List<string> clearedStationList = InitUrbanStationList(missionCode);
            List<RealTimeStationStatusType> stationListInfo = InitStationListInformation(stationList);

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out validGuid)).Returns(string.Empty);
			this._rtpisDataStore.Setup(x => x.GetMissionRealTimeInformation(missionCode)).Returns(missionData);
			this._rtpisDataStore.Setup(x => x.ClearRealTimeInformation(missionCode, stationList, out clearedStationList));
            this._rtpisDataStore.Setup(x => x.GetStationRealTimeInformation(missionCode, stationList)).Returns(stationListInfo);

			RealTimeClearRealTimeInformationResult rtr = this._rts.ClearRealTimeInformation(Guid.NewGuid(), missionCode, stationList);

			this._rtpisDataStore.Verify(x => x.ClearRealTimeInformation(missionCode, stationList, out clearedStationList), Times.Once());

			Assert.AreEqual(validGuid, rtr.RequestId);
			Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, rtr.ResultCode);
			Assert.AreEqual(missionCode, rtr.MissionCode);
			Assert.AreEqual(clearedStationList, rtr.StationList);
		}

		#endregion

		#endregion
	}

	public class RealTimeServiceTested : RealTimeService
	{
		public CommonConfiguration.PlatformTypeEnum? PlateformType
		{
			get
			{
				return RealTimeService._platformType;
			}
			set
			{
				RealTimeService._platformType = value;
			}
		}
	}
}