//---------------------------------------------------------------------------------------------------
// <copyright file="MissionTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2015.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.Mission;

namespace PIS.Ground.MissionTests
{
  
    /// <summary>Mission service test class.</summary>
    [TestFixture]
    public class MissionTests
    {
        #region attributes
        
        /// <summary>The session manager mock.</summary>
        private Mock<ISessionManagerExtended> _sessionManagerMock;

        /// <summary>The train 2ground client mock.</summary>
        private Mock<IT2GManager> _train2groundClientMock;

        /// <summary>The notification sender mock.</summary>
        private Mock<INotificationSender> _notificationSenderMock;

        /// <summary>The MissionService instance.</summary>
        private IMissionService _missionService;

        #endregion

        #region Tests management

        /// <summary>Initializes a new instance of the RealTimeServiceTests class.</summary>
        public MissionTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>Setups called before each test to initialize variables.</summary>
        [SetUp]
        public void Setup()
        {
            _sessionManagerMock = new Mock<ISessionManagerExtended>();
            _train2groundClientMock = new Mock<IT2GManager>();
            _notificationSenderMock = new Mock<INotificationSender>();

            _missionService = new MissionService(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object);
        }

        /// <summary>Tear down called after each test to clean.</summary>
        [TearDown]
        public void TearDown()
        {
        }
       
        #endregion

        #region Tests

        #region GetAvailableElementList

        /// <summary>Gets available element list test case with invalid session id result.</summary>
        [Test]
        public void GetAvailableElementListInvalidSessionId()
        {
            Guid sessionId = Guid.NewGuid();
            SessionData sessionData;
            _sessionManagerMock.Setup(x => x.GetSessionDetails(sessionId, out sessionData)).Returns("error");

            MissionServiceElementListResult result = _missionService.GetAvailableElementList(sessionId);
            
            ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();

            this._train2groundClientMock.Verify(w => w.GetAvailableElementDataList(out elementList), Times.Never());

            Assert.AreEqual(MissionErrorCode.InvalidSessionId, result.ResultCode);
        }

        /// <summary>
        /// Gets available element list test case with element list not available result.
        /// </summary>
        [Test]
        public void GetAvailableElementListNotAvailable()
        {
            ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
            T2GManagerErrorEnum returns = T2GManagerErrorEnum.eElementNotFound;
            SessionData sessionData = new SessionData();
            Guid sessionId = Guid.NewGuid();

            _sessionManagerMock.Setup(x => x.GetSessionDetails(sessionId, out sessionData)).Returns(string.Empty);
            _train2groundClientMock.Setup(y => y.GetAvailableElementDataList(out elementList)).Returns(returns);

            MissionServiceElementListResult result = _missionService.GetAvailableElementList(sessionId);

            _train2groundClientMock.Verify(w => w.GetAvailableElementDataList(out elementList), Times.Once());
            Assert.AreEqual(MissionErrorCode.ElementListNotAvailable, result.ResultCode);            
        }

        /// <summary>Gets available element list test case with Request accepted result.</summary>
        [Test]
        public void GetAvailableElementListRequestAccepted()
        {
            ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
            T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
            SessionData sessionData = new SessionData();
            Guid sessionId = Guid.NewGuid();

            _sessionManagerMock.Setup(x => x.GetSessionDetails(sessionId, out sessionData)).Returns(string.Empty);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);

            MissionServiceElementListResult result = _missionService.GetAvailableElementList(sessionId);

            _train2groundClientMock.Verify(w => w.GetAvailableElementDataList(out elementList), Times.Once());
            Assert.AreEqual(MissionErrorCode.RequestAccepted, result.ResultCode);            
        }
               
        #endregion
        
        #region InitializeMission

        /// <summary>InitializeAutomaticMission RequestAccepted.</summary>
        [Test]
        public void InitializeAutomaticMissionRequestAccepted()
        {
            Guid sessionId = Guid.NewGuid();
            AutomaticModeRequest automaticModeRequest = new AutomaticModeRequest();
            automaticModeRequest.ElementAlphaNumber = "TRAIN-1";
            automaticModeRequest.LanguageCodeList = new List<string>(){"FR-fr", "EN-en"};
            automaticModeRequest.LmtDataPackageVersion = "1.0.0.0";
            automaticModeRequest.MissionOperatorCode = "";
            automaticModeRequest.OnboardServiceCodeList = new List<uint>(){1,2,3,4,5};
            automaticModeRequest.OnBoardValidationFlag = false;
            automaticModeRequest.RequestTimeout = 60;
            automaticModeRequest.SessionId = sessionId;
            automaticModeRequest.StartDate = DateTime.Now.ToString();
            automaticModeRequest.StationInsertion = new StationInsertion();
            SessionData sessionData = new SessionData();
            Guid requestId = Guid.NewGuid();
            AvailableElementData elementData;

            _sessionManagerMock.Setup(x => x.GetSessionDetails(sessionId, out sessionData)).Returns(string.Empty);
            _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out requestId)).Returns(string.Empty);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber(automaticModeRequest.ElementAlphaNumber, out elementData)).Returns(T2GManagerErrorEnum.eSuccess);

            var result = _missionService.InitializeAutomaticMission(automaticModeRequest);

            _notificationSenderMock.Verify(x => x.SendNotification(PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressProcessing, It.IsAny<string>(), requestId));
            Assert.AreEqual(MissionErrorCode.RequestAccepted, result.ResultCode); 
        }

        /// <summary>InitializeManualMission RequestAccepted.</summary>
        [Test]
        public void InitializeManualMissionRequestAccepted()
        {
            Guid sessionId = Guid.NewGuid();
            ManualModeRequest manualModeRequest = new ManualModeRequest();
            manualModeRequest.ElementAlphaNumber = "TRAIN-1";
            manualModeRequest.LanguageCodeList = new List<string>() { "FR-fr", "EN-en" };
            manualModeRequest.LmtDataPackageVersion = "1.0.0.0";
            manualModeRequest.MissionOperatorCode = "";
            manualModeRequest.OnboardServiceCodeList = new List<uint>() { 1, 2, 3, 4, 5 };
            manualModeRequest.OnBoardValidationFlag = false;
            manualModeRequest.RequestTimeout = 60;
            manualModeRequest.SessionId = sessionId;
            manualModeRequest.StartDate = DateTime.Now.ToString();
            SessionData sessionData = new SessionData();
            Guid requestId = Guid.NewGuid();
            AvailableElementData elementData;

            _sessionManagerMock.Setup(x => x.GetSessionDetails(sessionId, out sessionData)).Returns(string.Empty);
            _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out requestId)).Returns(string.Empty);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber(manualModeRequest.ElementAlphaNumber, out elementData)).Returns(T2GManagerErrorEnum.eSuccess);

            var result = _missionService.InitializeManualMission(manualModeRequest);

            _notificationSenderMock.Verify(x => x.SendNotification(PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressProcessing, It.IsAny<string>(), requestId));
            Assert.AreEqual(MissionErrorCode.RequestAccepted, result.ResultCode);
        }

        /// <summary>InitializeModifiedMission RequestAccepted.</summary>
        [Test]
        public void InitializeModifiedMissionRequestAccepted()
        {
            Guid sessionId = Guid.NewGuid();
            ModifiedModeRequest modifiedModeRequest = new ModifiedModeRequest();
            modifiedModeRequest.ElementAlphaNumber = "TRAIN-1";
            modifiedModeRequest.LanguageCodeList = new List<string>() { "FR-fr", "EN-en" };
            modifiedModeRequest.LmtDataPackageVersion = "1.0.0.0";
            modifiedModeRequest.MissionOperatorCode = "";
            modifiedModeRequest.OnboardServiceCodeList = new List<uint>() { 1, 2, 3, 4, 5 };
            modifiedModeRequest.OnBoardValidationFlag = false;
            modifiedModeRequest.RequestTimeout = 60;
            modifiedModeRequest.SessionId = sessionId;
            modifiedModeRequest.StartDate = DateTime.Now.ToString();
            SessionData sessionData = new SessionData();
            Guid requestId = Guid.NewGuid();
            AvailableElementData elementData;

            _sessionManagerMock.Setup(x => x.GetSessionDetails(sessionId, out sessionData)).Returns(string.Empty);
            _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out requestId)).Returns(string.Empty);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber(modifiedModeRequest.ElementAlphaNumber, out elementData)).Returns(T2GManagerErrorEnum.eSuccess);

            var result = _missionService.InitializeModifiedMission(modifiedModeRequest);

            _notificationSenderMock.Verify(x => x.SendNotification(PIS.Ground.GroundCore.AppGround.NotificationIdEnum.MissionCommandProgressProcessing, It.IsAny<string>(), requestId));
            Assert.AreEqual(MissionErrorCode.RequestAccepted, result.ResultCode);
        }

        #endregion

        #endregion
    }
}