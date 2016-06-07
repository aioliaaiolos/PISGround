using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using NUnit.Framework;
using PIS.Ground.Mission;
using PIS.Ground.Core;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;

/// Test classes for MissionControl Service.
///
namespace PIS.Ground.MissionControlTests
{
    public enum StationListTypeEnum
    {
        /// <summary>
        /// All stations
        /// </summary>
        ALL = 0,

        /// <summary>
        /// Only valid stations
        /// </summary>
        VALID_ONLY = 1,

        /// <summary>
        /// Only invalid stations
        /// </summary>
        INVALID_ONLY = 2,
    }
    
    /// <summary>MissionControl service test class.</summary>
    [TestFixture]
    public class MissionControlTests
    {
        #region attributes

        /// <summary>The element Id</summary>
        private string _elementId = string.Empty;

        /// <summary>The session Id</summary>
        private Guid _sessionId = Guid.NewGuid();
        
        /// <summary>The existing mission code</summary>
        private string _existingMissionCode = string.Empty;

        /// <summary>The new mission code</summary>
        private string _newMissionCode = string.Empty;        

        /// <summary>Full pathname of the db source folder.</summary>
        private string _dbSourcePath = string.Empty;

        /// <summary>Full pathname of the execution folder.</summary>
        private string _dbWorkingPath = string.Empty;
        
        /// <summary>The session manager mock.</summary>
        private Mock<ISessionManagerExtended> _sessionManagerMock;

        /// <summary>The train 2ground client mock.</summary>
        private Mock<IT2GManager> _train2groundClientMock;

        /// <summary>The notification sender mock.</summary>
        private Mock<INotificationSender> _notificationSenderMock;

        /// <summary>The remote data store factory.</summary>
        private Mock<IRemoteDataStoreFactory> _remoteDataStoreFactoryMock;

        /// <summary>The MissionService instance.</summary>
        private IMissionService _missionService;

        #endregion

        #region Tests management

        /// <summary>Initializes a new instance of the RealTimeServiceTests class.</summary>
        public MissionControlTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>Setups called before each test to initialize variables.</summary>
        [SetUp]
        public void Setup()
        {
            this._elementId = "Train-150";
            this._existingMissionCode = "M53_Gpp-Cs";
            this._newMissionCode = "NewMission";
            this._dbSourcePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty) + "\\..\\..\\Data\\lmt.db";
            this._dbWorkingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty) + "\\temp\\";

            this._sessionManagerMock = new Mock<ISessionManagerExtended>();
            this._train2groundClientMock = new Mock<IT2GManager>();
            this._notificationSenderMock = new Mock<INotificationSender>();
            this._remoteDataStoreFactoryMock = new Mock<IRemoteDataStoreFactory>();

            MissionService.Initialize(
                this._train2groundClientMock.Object,
                this._sessionManagerMock.Object,
                this._notificationSenderMock.Object,
                this._remoteDataStoreFactoryMock.Object);

            this._missionService = new MissionServiceTested();
            ((MissionServiceTested)this._missionService).PlateformType = CommonConfiguration.PlatformTypeEnum.URBAN;

            if (!Directory.Exists(_dbWorkingPath))
            {
                Directory.CreateDirectory(_dbWorkingPath);
            }
        }

        /// <summary>Initialises the service information.</summary>
        /// <returns>ServiceInfo struct filled with dummy data.</returns>
        private ServiceInfo InitServiceInfo()
        {
            return new ServiceInfo(123, "ServiceName", 100, 55, true, "ServiceIPAddress", "AID", "SID", 1500);
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
            element.ElementNumber = "TRAIN-150";
            element.PisBasicPackageVersion = "3.0.13.0";
            element.LmtPackageVersion = "3.0.13.0";
            element.PisBaselineData = new PisBaseline();
            element.PisBaselineData.CurrentVersionLmtOut = "3.0.13.0";
            element.PisBaselineData.CurrentVersionPisBaseOut = "3.0.13.0";
            element.PisBaselineData.CurrentVersionPisMissionOut = "3.0.13.0";
            element.PisBaselineData.FutureVersionPisInfotainmentOut = "3.0.13.0";
            
            element.OnlineStatus = true;

            return element;
        }
       
        /// <summary>Initialises a station list.</summary>		
        /// <param name="onlyValidStations">Should there be only valid stations in the list.</param>
		/// <returns>List of station id.</returns>        
        private List<string> InitStationList(StationListTypeEnum listType)
        {
            List<string> refList = new List<string>();

            switch (listType)
            {
                case StationListTypeEnum.ALL:
                    refList.Add("Gzh_53");
                    refList.Add("Ken_53");
                    refList.Add("Ken_53_invalid");
                    refList.Add("Gpp_53");
                    refList.Add("Gpp_53_invalid");
                    refList.Add("Ken_53");
                    break;

                case StationListTypeEnum.INVALID_ONLY:                    
                    refList.Add("Ken_53_invalid");                    
                    refList.Add("Gpp_53_invalid");                    
                    break;

                case StationListTypeEnum.VALID_ONLY:
                    refList.Add("Gzh_53");
                    refList.Add("Ken_53");                    
                    refList.Add("Gpp_53");                    
                    refList.Add("Ken_53");
                    break;
            }

            return refList;
        }

        /// <summary>Initializes the mission params.</summary>
        /// <returns>An instance of MissionParams for testing purpose.</returns>
        private MissionParams InitMissionParams(
            Guid sessionId,
            string missionCode,
            string elementId,
            List<string> stationList,
            int? timeout)
        {
            MissionParams missionParams = new MissionParams(
                                                sessionId,
                                                missionCode,
                                                elementId,
                                                stationList,
                                                timeout);            
            return missionParams;
        }

        #endregion

        #region Tests

        #region GetAvailableElementList

        /// <summary>Gets available element list test case with invalid session id result.</summary>
        [Test]
        public void GetAvailableElementListInvalidSessionId()
        {
            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);
            
            MissionAvailableElementListResult result = this._missionService.GetAvailableElementList(Guid.NewGuid());
            
            ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();

            this._train2groundClientMock.Verify(w => w.GetAvailableElementDataList(out elementList), Times.Never());

            Assert.AreEqual(MissionServiceErrorCodeEnum.ErrorInvalidSessionId, result.ResultCode);
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

            MissionAvailableElementListResult result = this._missionService.GetAvailableElementList(Guid.NewGuid());

            this._train2groundClientMock.Verify(w => w.GetAvailableElementDataList(out elementList), Times.Once());

            Assert.AreEqual(MissionServiceErrorCodeEnum.ErrorElementListNotAvailable, result.ResultCode);            
        }

        /// <summary>Gets available element list test case with Request accepted result.</summary>
        [Test]
        public void GetAvailableElementListRequestAccepted()
        {
            ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
            T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
            this._train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
            
            MissionAvailableElementListResult result = this._missionService.GetAvailableElementList(Guid.NewGuid());

            this._train2groundClientMock.Verify(w => w.GetAvailableElementDataList(out elementList), Times.Once());

            Assert.AreEqual(MissionServiceErrorCodeEnum.RequestAccepted, result.ResultCode);            
        }
               
        #endregion

        #region InitializeMission

        /// <summary>InitializeMission invalid session identifier.</summary>
        [Test]
        public void InitializeMissionInvalidSessionId()
        {            
            Guid requestId = Guid.NewGuid();
            AvailableElementData elementData = null;

            MissionParams missionParams = InitMissionParams(
                this._sessionId,
                this._existingMissionCode,
                this._elementId,
                null,
                1);

            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);
            this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);

            MissionInitializeMissionResult result = this._missionService.InitializeMission(
                Guid.NewGuid(),
                missionParams.MissionCode,
                missionParams.ElementId,
                missionParams.StationList,
                missionParams.Timeout);

            this._train2groundClientMock.Verify(a => a.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData), Times.Never());
            
            Assert.AreEqual(MissionServiceErrorCodeEnum.ErrorInvalidSessionId, result.ResultCode);
            Assert.AreEqual(Guid.Empty, result.RequestId);
            Assert.AreEqual(result.MissionCode, missionParams.MissionCode);
            Assert.IsNull(result.InvalidStationList);
        }

        /// <summary>InitializeMission invalid element identifier.</summary>
        [Test]
        public void InitializeMissionInvalidElementId()
        {
            AvailableElementData elementData = null;
            Guid requestId = Guid.NewGuid();

            MissionParams missionParams = InitMissionParams(
                this._sessionId,
                this._existingMissionCode,
                this._elementId,
                null,
                1);

            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
            this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
            this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData)).Returns(T2GManagerErrorEnum.eElementNotFound);

            MissionInitializeMissionResult result = this._missionService.InitializeMission(
                missionParams.SessionId,
                missionParams.MissionCode,
                missionParams.ElementId,
                missionParams.StationList,
                missionParams.Timeout);

            this._train2groundClientMock.Verify(a => a.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData), Times.Once());

            Assert.AreEqual(MissionServiceErrorCodeEnum.ErrorInvalidElementId, result.ResultCode);
            Assert.AreEqual(requestId, result.RequestId);
            Assert.AreEqual(result.MissionCode, missionParams.MissionCode);
            Assert.IsNull(result.InvalidStationList);
        }

        /// <summary>InitializeMission invalid reuqest timeout.</summary>
        [Test]
        public void InitializeMissionInvalidRequestTimeout()
        {
            AvailableElementData elementData = null;
            Guid requestId = Guid.NewGuid();

            MissionParams missionParams = InitMissionParams(
                this._sessionId,
                this._existingMissionCode,
                this._elementId,
                null,
                50000);

            MissionInitializeMissionResult result = this._missionService.InitializeMission(
                missionParams.SessionId,
                missionParams.MissionCode,
                missionParams.ElementId,
                missionParams.StationList,
                missionParams.Timeout);

            this._sessionManagerMock.Verify(a => a.IsSessionValid(It.IsAny<Guid>()), Times.Never());
            this._sessionManagerMock.Verify(b => b.GenerateRequestID(It.IsAny<Guid>(), out requestId), Times.Never());
            this._train2groundClientMock.Verify(c => c.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData), Times.Never());

            Assert.AreEqual(MissionServiceErrorCodeEnum.ErrorInvalidRequestTimeout, result.ResultCode);
            Assert.AreEqual(Guid.Empty, result.RequestId);
            Assert.AreEqual(result.MissionCode, missionParams.MissionCode);            
            Assert.IsNull(result.InvalidStationList);
        }

        /// <summary>InitializeMission invalid mission code.</summary>
        [Test]
        public void InitializeMissionInvalidMissionCode()
        {
            AvailableElementData elementData = null;
            Guid requestId = Guid.NewGuid();

            MissionParams missionParams = InitMissionParams(
                this._sessionId,
                string.Empty,
                this._elementId,
                null,
                1);            

            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
            this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
            this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData)).Returns(T2GManagerErrorEnum.eSuccess);

            MissionInitializeMissionResult result = this._missionService.InitializeMission(
                missionParams.SessionId,
                missionParams.MissionCode,
                missionParams.ElementId,
                missionParams.StationList,
                missionParams.Timeout);

            Assert.AreEqual(MissionServiceErrorCodeEnum.ErrorInvalidMissionCode, result.ResultCode);
            Assert.AreEqual(requestId, result.RequestId);
            Assert.IsNullOrEmpty(result.MissionCode);
            Assert.IsNull(result.InvalidStationList);
        }

        /// <summary>InitializeMission unknown mission code and empty stations list.</summary>
        [Test]
        public void InitializeMissionUnknownMissionCodeAndEmptyStationList()
        {
            AvailableElementData elementData = InitTrainElement();;
            Guid requestId = Guid.NewGuid();
            File.Copy(this._dbSourcePath, Path.Combine(this._dbWorkingPath, "file.db"));

            MissionParams missionParams = InitMissionParams(
                this._sessionId,
                this._newMissionCode,
                this._elementId,
                null,
                1);

            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
            this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
            this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData)).Returns(T2GManagerErrorEnum.eSuccess);
            this._remoteDataStoreFactoryMock.Setup(z => z.GetRemoteDataStoreInstance()).Returns(new RemoteDataStoreSimulator.RemoteDataStore(this._dbWorkingPath, elementData.LmtPackageVersion));

            MissionInitializeMissionResult result = this._missionService.InitializeMission(
                missionParams.SessionId,
                missionParams.MissionCode,
                missionParams.ElementId,
                missionParams.StationList,
                missionParams.Timeout);

            Assert.AreEqual(MissionServiceErrorCodeEnum.ErrorInvalidMissionCode, result.ResultCode);
            Assert.AreEqual(result.RequestId, requestId);
            Assert.AreEqual(result.MissionCode, missionParams.MissionCode);
            Assert.IsNull(result.InvalidStationList);            
        }

        /// <summary>InitializeMission invalid station Id.</summary>
        [Test]
        public void InitializeMissionInvalidStationId()
        {
            AvailableElementData elementData = InitTrainElement(); ;
            Guid requestId = Guid.NewGuid();
            File.Copy(this._dbSourcePath, Path.Combine(this._dbWorkingPath, "file.db"));
            List<string> inputStationList = InitStationList(StationListTypeEnum.ALL);

            MissionParams missionParams = InitMissionParams(
                this._sessionId,
                this._newMissionCode,
                this._elementId,
                inputStationList,
                1);

            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
            this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
            this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData)).Returns(T2GManagerErrorEnum.eSuccess);
            this._remoteDataStoreFactoryMock.Setup(z => z.GetRemoteDataStoreInstance()).Returns(new RemoteDataStoreSimulator.RemoteDataStore(this._dbWorkingPath, elementData.LmtPackageVersion));

            MissionInitializeMissionResult result = this._missionService.InitializeMission(
                missionParams.SessionId,
                missionParams.MissionCode,
                missionParams.ElementId,
                missionParams.StationList,
                missionParams.Timeout);

            Assert.AreEqual(MissionServiceErrorCodeEnum.ErrorInvalidStationId, result.ResultCode);
            Assert.AreEqual(result.RequestId, requestId);
            Assert.AreEqual(result.MissionCode, missionParams.MissionCode);
            Assert.AreEqual(result.InvalidStationList, InitStationList(StationListTypeEnum.INVALID_ONLY));
        }

        /// <summary>InitializeMission Error Opening LMT Db.</summary>
        [Test]
        public void InitializeMissionErrorOpeningLMTDb()
        {
            AvailableElementData elementData = InitTrainElement();            
            Guid requestId = Guid.NewGuid();
            File.Copy(this._dbSourcePath, Path.Combine(this._dbWorkingPath, "file.db"));

            MissionParams missionParams = InitMissionParams(
                this._sessionId,
                this._existingMissionCode,
                this._elementId,
                null,                
                1);

            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
            this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
            this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData)).Returns(T2GManagerErrorEnum.eSuccess);
            this._remoteDataStoreFactoryMock.Setup(z => z.GetRemoteDataStoreInstance()).Returns(new RemoteDataStoreSimulator.RemoteDataStore(this._dbWorkingPath, "1.1.1.1"));

            MissionInitializeMissionResult result = this._missionService.InitializeMission(
                missionParams.SessionId,
                missionParams.MissionCode,
                missionParams.ElementId,
                missionParams.StationList,
                missionParams.Timeout);

            Assert.AreEqual(MissionServiceErrorCodeEnum.ErrorOpeningLMTDb, result.ResultCode);
            Assert.AreEqual(result.RequestId, requestId);
            Assert.AreEqual(result.MissionCode, missionParams.MissionCode);
        }

        /// <summary>InitializeMission Existing Mission Request Accepted.</summary>
        [Test]
        public void InitializeMissionExistingMissionRequestAccepted()
        {
            AvailableElementData elementData = InitTrainElement();
            Guid requestId = Guid.NewGuid();
            File.Copy(this._dbSourcePath, Path.Combine(this._dbWorkingPath, "file.db"));

            MissionParams missionParams = InitMissionParams(
                this._sessionId,
                this._existingMissionCode,
                this._elementId,
                null,
                1);

            ServiceInfo serviceInfo = this.InitServiceInfo();

            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
            this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
            this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData)).Returns(T2GManagerErrorEnum.eSuccess);
            this._remoteDataStoreFactoryMock.Setup(z => z.GetRemoteDataStoreInstance()).Returns(new RemoteDataStoreSimulator.RemoteDataStore(this._dbWorkingPath, "3.0.13.0"));

            T2GManagerErrorEnum rslt = T2GManagerErrorEnum.eSuccess;
            
            this._train2groundClientMock.Setup(x => x.GetAvailableServiceData(
                missionParams.ElementId,
                (int)eServiceID.eSrvSIF_MissionServer,
                out serviceInfo)).Returns(rslt);

            MissionInitializeMissionResult result = this._missionService.InitializeMission(
                missionParams.SessionId,
                missionParams.MissionCode,
                missionParams.ElementId,
                missionParams.StationList,
                missionParams.Timeout);

            Assert.AreEqual(MissionServiceErrorCodeEnum.RequestAccepted, result.ResultCode);
            Assert.AreEqual(result.RequestId, requestId);
            Assert.AreEqual(result.MissionCode, this._existingMissionCode);
            Assert.IsNull(result.InvalidStationList);
        }

        /// <summary>InitializeMission New Mission Request Accepted.</summary>
        [Test]
        public void InitializeMissionNewMissionRequestAccepted()
        {
            AvailableElementData elementData = InitTrainElement();
            Guid requestId = Guid.NewGuid();
            File.Copy(this._dbSourcePath, Path.Combine(this._dbWorkingPath, "file.db"));
            List<string> inputStationList = InitStationList(StationListTypeEnum.VALID_ONLY);

            MissionParams missionParams = InitMissionParams(
                this._sessionId,
                this._newMissionCode,
                this._elementId,
                inputStationList,
                1);

            ServiceInfo serviceInfo = this.InitServiceInfo();

            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
            this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
            this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData)).Returns(T2GManagerErrorEnum.eSuccess);
            this._remoteDataStoreFactoryMock.Setup(z => z.GetRemoteDataStoreInstance()).Returns(new RemoteDataStoreSimulator.RemoteDataStore(this._dbWorkingPath, "3.0.13.0"));

            T2GManagerErrorEnum rslt = T2GManagerErrorEnum.eSuccess;

            this._train2groundClientMock.Setup(x => x.GetAvailableServiceData(
                missionParams.ElementId,
                (int)eServiceID.eSrvSIF_MissionServer,
                out serviceInfo)).Returns(rslt);

            MissionInitializeMissionResult result = this._missionService.InitializeMission(
                missionParams.SessionId,
                missionParams.MissionCode,
                missionParams.ElementId,
                missionParams.StationList,
                missionParams.Timeout);

            Assert.AreEqual(MissionServiceErrorCodeEnum.RequestAccepted, result.ResultCode);
            Assert.AreEqual(result.RequestId, requestId);
            Assert.AreEqual(result.MissionCode, this._newMissionCode);            
        }

        #endregion

        #region CancelMission

        /// <summary>CancelMission invalid session identifier.</summary>
        [Test]
        public void CancelMissionInvalidSessionId()
        {
            Guid requestId = Guid.NewGuid();
            AvailableElementData elementData = null;

            MissionParams initParams = InitMissionParams(
                this._sessionId,
                this._existingMissionCode,
                this._elementId,
                null,
                1);

            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);
            this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);

            MissionCancelMissionResult result = this._missionService.CancelMission(
                Guid.NewGuid(),
                initParams.MissionCode,
                initParams.ElementId,                
                initParams.Timeout);

            this._train2groundClientMock.Verify(a => a.GetAvailableElementDataByElementNumber(initParams.ElementId, out elementData), Times.Never());

            Assert.AreEqual(MissionServiceErrorCodeEnum.ErrorInvalidSessionId, result.ResultCode);
            Assert.AreEqual(Guid.Empty, result.RequestId);
            Assert.AreEqual(result.MissionCode, initParams.MissionCode);            
        }

        /// <summary>CancelMission invalid element identifier.</summary>
        [Test]
        public void CancelMissionInvalidElementId()
        {
            AvailableElementData elementData = null;
            Guid requestId = Guid.NewGuid();

            MissionParams missionParams = InitMissionParams(
                this._sessionId,
                this._existingMissionCode,
                this._elementId,
                null,
                1);

            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
            this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
            this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData)).Returns(T2GManagerErrorEnum.eElementNotFound);

            MissionCancelMissionResult result = this._missionService.CancelMission(
                missionParams.SessionId,
                missionParams.MissionCode,
                missionParams.ElementId,                
                missionParams.Timeout);

            this._train2groundClientMock.Verify(a => a.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData), Times.Once());

            Assert.AreEqual(MissionServiceErrorCodeEnum.ErrorInvalidElementId, result.ResultCode);
            Assert.AreEqual(requestId, result.RequestId);
            Assert.AreEqual(result.MissionCode, missionParams.MissionCode);            
        }

        /// <summary>CancelMission invalid reuqest timeout.</summary>
        [Test]
        public void CancelMissionInvalidRequestTimeout()
        {
            AvailableElementData elementData = null;
            Guid requestId = Guid.NewGuid();

            MissionParams missionParams = InitMissionParams(
                this._sessionId,
                this._existingMissionCode,
                this._elementId,
                null,
                50000);

            MissionCancelMissionResult result = this._missionService.CancelMission(
                missionParams.SessionId,
                missionParams.MissionCode,
                missionParams.ElementId,
                missionParams.Timeout);

            this._sessionManagerMock.Verify(a => a.IsSessionValid(It.IsAny<Guid>()), Times.Never());
            this._sessionManagerMock.Verify(b => b.GenerateRequestID(It.IsAny<Guid>(), out requestId), Times.Never());
            this._train2groundClientMock.Verify(c => c.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData), Times.Never());

            Assert.AreEqual(MissionServiceErrorCodeEnum.ErrorInvalidRequestTimeout, result.ResultCode);
            Assert.AreEqual(Guid.Empty, result.RequestId);
            Assert.AreEqual(result.MissionCode, missionParams.MissionCode);
        }

        /// <summary>CancelMission reuqest accepted.</summary>
        [Test]
        public void CancelMissionRequestAccepted()
        {
            AvailableElementData elementData = null;
            Guid requestId = Guid.NewGuid();

            MissionParams missionParams = InitMissionParams(
                this._sessionId,
                this._existingMissionCode,
                this._elementId,
                null,
                1);

            ServiceInfo serviceInfo = this.InitServiceInfo();

            T2GManagerErrorEnum rslt = T2GManagerErrorEnum.eSuccess;

            this._train2groundClientMock.Setup(x => x.GetAvailableServiceData(
                missionParams.ElementId,
                (int)eServiceID.eSrvSIF_MissionServer,
                out serviceInfo)).Returns(rslt);

            this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
            this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
            this._train2groundClientMock.Setup(y => y.GetAvailableElementDataByElementNumber(missionParams.ElementId, out elementData)).Returns(T2GManagerErrorEnum.eSuccess);
            this._remoteDataStoreFactoryMock.Setup(z => z.GetRemoteDataStoreInstance()).Returns(new RemoteDataStoreSimulator.RemoteDataStore(this._dbWorkingPath, "3.0.13.0"));

            MissionCancelMissionResult result = this._missionService.CancelMission(
                missionParams.SessionId,
                missionParams.MissionCode,
                missionParams.ElementId,
                missionParams.Timeout);            

            Assert.AreEqual(MissionServiceErrorCodeEnum.RequestAccepted, result.ResultCode);
            Assert.AreEqual(result.RequestId, requestId);
            Assert.AreEqual(result.MissionCode, this._existingMissionCode);
        }

        #endregion

        #endregion
    }

    public class MissionParams
    {
        private Guid _sessionId;
        private string _missionCode;
        private string _elementId;
        private List<string> _stationList;
        private int? _timeout;
        
        public MissionParams(
            Guid sessionId,
            string missionCode,
            string elementId,
            List<string> stationList,
            int? timeout)
        {
            _sessionId = sessionId;
            _missionCode = missionCode;
            _elementId = elementId;
            _stationList = stationList;
            _timeout = timeout;
        }
        
        public Guid SessionId
        { 
            get {return _sessionId;}
            set { _sessionId = value; }
        }

        public string MissionCode
        {
            get { return _missionCode; }
            set { _missionCode = value; }
        }

        public string ElementId
        {
            get { return _elementId; }
            set { _elementId = value; }
        }

        public List<string> StationList
        {
            get { return _stationList; }
            set { _stationList = value; }
        }

        public int? Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
    }

    public class MissionServiceTested : MissionService
    {
        public CommonConfiguration.PlatformTypeEnum? PlateformType
        {
            get
            {
                return MissionService._plateformType;
            }
            set
            {
                MissionService._plateformType = value;
            }
        }
    }
}
