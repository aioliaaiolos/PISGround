//---------------------------------------------------------------------------------------------------
// <copyright file="LiveVideoControlNUnitTest.cs" company="Alstom">
//          (c) Copyright ALSTOM 2013.  All rights reserved.
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
using PIS.Ground.Core.Data;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.LiveVideoControl;
using System.Reflection;
using PIS.Ground.Core.Common;

namespace LiveVideoControlTests
{
	/// <summary>Live video control service test class.</summary>
	[TestFixture]
	public class LiveVideoControlServiceTests
	{
		#region attributes

		/// <summary>The train 2ground client mock.</summary>
		private Mock<IT2GManager> _train2groundManagerMock;

		/// <summary>The session manager mock.</summary>
		private Mock<ISessionManagerExtended> _sessionManagerMock;

		/// <summary>The notification sender mock.</summary>
		private Mock<INotificationSender> _notificationSenderMock;

		/// <summary>The request processor mock.</summary>
		private Mock<IRequestProcessor> _requestProcessorMock;

		private Mock<ILiveVideoControlConfiguration> _configurationMock;

		/// <summary>The LiveVideoControlService instance.</summary>
		private ILiveVideoControlService _lvcs;

		/// <summary>A valid session identifier string to be used to create a Guid.</summary>
		private const string ValidSessionIdString = "fb0de0d7-d76a-45f7-8b0c-37fbd43b38f8";

		/// <summary>A valid session identifier.</summary>
		private static readonly Guid ValidSessionId = new Guid(ValidSessionIdString);

		/// <summary>A valid URL of the streaming.</summary>
		private const string ValidStreamingUrl = "http://10.90.202.20/mediaweb/PIS.m3u8";

		#endregion

		#region Tests managment

		/// <summary>Initializes a new instance of the LiveVideoControlServiceTests class.</summary>
		public LiveVideoControlServiceTests()
		{
			// Nothing Special
		}

		/// <summary>Setups called before each test to initialize variables.</summary>
		[SetUp]
		public void Setup()
		{
			this._train2groundManagerMock = new Mock<IT2GManager>();
			this._sessionManagerMock = new Mock<ISessionManagerExtended>();
			this._notificationSenderMock = new Mock<INotificationSender>();
			this._requestProcessorMock = new Mock<IRequestProcessor>();
			this._configurationMock = new Mock<ILiveVideoControlConfiguration>();

			// callback registration
			this._train2groundManagerMock.Setup(
				x => x.SubscribeToElementChangeNotification(
					It.IsAny<string>(),
					It.IsAny<EventHandler<ElementEventArgs>>()));

			// starting in manual mode
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);

			LiveVideoControlService.Initialize(
				this._train2groundManagerMock.Object,
				this._sessionManagerMock.Object,
				this._notificationSenderMock.Object,
				this._requestProcessorMock.Object,
				this._configurationMock.Object);
			this._lvcs = new LiveVideoControlService();
		}

		/// <summary>Tear down called after each test to clean.</summary>
		[TearDown]
		public void TearDown()
		{
			// Do something after each tests
		}

		/// <summary>Set automatic mode.</summary>
		/// <param name="url">Streaming URL to be used.</param>
		private void SetAutomaticMode(string url)
		{
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(url);
			LiveVideoControlService.LoadConfiguration(this._configurationMock.Object);
		}

		/// <summary>Set manual mode.</summary>
		private void SetManualMode()
		{
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			LiveVideoControlService.LoadConfiguration(this._configurationMock.Object);
		}

		/// <summary>Get the current command mode directly by accessing private methods 
		///          of the class under test.</summary>
		/// <param name="url">Streaming URL if automatic mode, null otherwise.</param>
		/// <returns>true if automatic, false if manual</returns>
		private bool GetAutomaticMode(out string url)
		{
			Type[] types = new Type[] { typeof(string).MakeByRefType() };
			object[] parameters = new object[] { string.Empty };

			object result = this._lvcs.GetType().GetMethod("GetAutomaticMode",
				BindingFlags.NonPublic | BindingFlags.Static, null,
				types, null).Invoke(this._lvcs, parameters);

			url = (string)parameters[0];

			return (bool)result;
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
			element.OnlineStatus = true;
			return element;
		}

		/// <summary>Initialises service information structure.</summary>
		/// <returns>An instance of ServiceInfo for testing purpose.</returns>
		private ServiceInfo InitServiceInfo(bool available)
		{
			return new ServiceInfo(123, "ServiceName", 100, 55, available, "ServiceIPAddress", "AID", "SID", 1500);
		}

		#endregion

		#region GetAvailableElementList

		/// <summary>Gets available element list test case with invalid session id result.</summary>
		[Test]
		public void GetAvailableElementListInvalidSessionId()
		{
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);

			LiveVideoControlElementListResult lvcr = this._lvcs.GetAvailableElementList(new Guid());
			Assert.AreEqual(LiveVideoControlErrorEnum.InvalidSessionId, lvcr.ResultCode);
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
			this._train2groundManagerMock.Setup(y => y.GetAvailableElementDataList(out elementList)).Returns(returns);

			LiveVideoControlElementListResult lvcr = this._lvcs.GetAvailableElementList(new Guid());
			Assert.AreEqual(LiveVideoControlErrorEnum.ElementListNotAvailable, lvcr.ResultCode);
		}

		/// <summary>Gets available element list test case with Request accepted result.</summary>
		[Test]
		public void GetAvailableElementListRequestAcceptedWithoutElement()
		{
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);

			LiveVideoControlElementListResult lvcr = this._lvcs.GetAvailableElementList(new Guid());
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
		}

		/// <summary>Gets available element list: train exist but no live video service.
		/// 		 The return list should be empty</summary>
		[Test]
		public void GetAvailableElementListRequestAcceptedWithNoServiceElement()
		{
			AvailableElementData element = this.InitTrainElement();
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>() { element };
			ServiceInfo lServiceInfo = null;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			this._train2groundManagerMock.Setup(x => x.GetAvailableServiceData(element.ElementNumber,
				(int)eServiceID.eSrvSIF_LiveVideoControlServer, out lServiceInfo)).Returns(T2GManagerErrorEnum.eServiceInfoNotFound);
			LiveVideoControlElementListResult lvcr = this._lvcs.GetAvailableElementList(new Guid());
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
			Assert.AreEqual(0, lvcr.ElementList.Count);
		}

		/// <summary>Gets available element list: one train exists but live video service not available.
		/// 		 The return list should be empty</summary>
		[Test]
		public void GetAvailableElementListRequestAcceptedWithUnavailableServiceElement()
		{
			AvailableElementData element = this.InitTrainElement();
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>() { element };
			ServiceInfo lServiceInfo = this.InitServiceInfo(false);
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			this._train2groundManagerMock.Setup(x => x.GetAvailableServiceData(element.ElementNumber,
				(int)eServiceID.eSrvSIF_LiveVideoControlServer, out lServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);
			LiveVideoControlElementListResult lvcr = this._lvcs.GetAvailableElementList(new Guid());
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
			Assert.AreEqual(0, lvcr.ElementList.Count);
		}

		/// <summary>Gets available element list test case with Request accepted result.</summary>
		[Test]
		public void GetAvailableElementListRequestAcceptedWithElement()
		{
			AvailableElementData element = this.InitTrainElement();
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>() { element };
			ServiceInfo lServiceInfo = this.InitServiceInfo(true);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			this._train2groundManagerMock.Setup(x => x.GetAvailableServiceData(element.ElementNumber,
				(int)eServiceID.eSrvSIF_LiveVideoControlServer, out lServiceInfo)).Returns(returns);
			LiveVideoControlElementListResult lvcr = this._lvcs.GetAvailableElementList(new Guid());
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
			Assert.AreEqual(1, lvcr.ElementList.Count);
			Assert.AreEqual(element.ElementNumber, lvcr.ElementList[0].ElementNumber);
		}

		#endregion

		#region StartVideoStreamingCommand

		/// <summary>Test a sent start video streaming command to embedded element.</summary>
		[Test]
		public void StartVideoStreamingCommandInvalidSessionId()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			string streamingUrl = string.Empty;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			LiveVideoControlResult lvcr = this._lvcs.StartVideoStreamingCommand(ValidSessionId, targetAdress, streamingUrl);
			Assert.AreEqual(LiveVideoControlErrorEnum.InvalidSessionId, lvcr.ResultCode);
		}

		/// <summary>Starts video streaming command invalid request identifier.</summary>
		[Test]
		public void StartVideoStreamingCommandInvalidRequestId()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			string streamingUrl = string.Empty;
			Guid requestId = Guid.Empty;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			LiveVideoControlResult lvcr = this._lvcs.StartVideoStreamingCommand(ValidSessionId, targetAdress, streamingUrl);
			Assert.AreEqual(LiveVideoControlErrorEnum.InvalidRequestID, lvcr.ResultCode);
		}

		/// <summary>Starts video streaming command unknown mission code.</summary>
		[Test]
		public void StartVideoStreamingCommandUnknownMissionCode()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.MissionCode;
			string streamingUrl = string.Empty;
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList;
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eElementNotFound;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.StartVideoStreamingCommand(ValidSessionId, targetAdress, streamingUrl);
			Assert.AreEqual(LiveVideoControlErrorEnum.UnknownMissionId, lvcr.ResultCode);
		}

		/// <summary>Starts video streaming command unknown operator code.</summary>
		[Test]
		public void StartVideoStreamingCommandUnknownOperatorCode()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.MissionOperatorCode;
			string streamingUrl = string.Empty;
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList;
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eElementNotFound;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.StartVideoStreamingCommand(ValidSessionId, targetAdress, streamingUrl);
			Assert.AreEqual(LiveVideoControlErrorEnum.UnknownMissionId, lvcr.ResultCode);
		}

		/// <summary>Starts video streaming command unknown element identifier.</summary>
		[Test]
		public void StartVideoStreamingCommandUnknownElementId()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			string streamingUrl = string.Empty;
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList;
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eElementNotFound;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.StartVideoStreamingCommand(ValidSessionId, targetAdress, streamingUrl);
			Assert.AreEqual(LiveVideoControlErrorEnum.UnknownElementId, lvcr.ResultCode);
		}

		/// <summary>Starts video streaming command t 2 g server offline.</summary>
		[Test]
		public void StartVideoStreamingCommandT2GServerOffline()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			string streamingUrl = string.Empty;
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList;
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eT2GServerOffline;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.StartVideoStreamingCommand(ValidSessionId, targetAdress, streamingUrl);
			Assert.AreEqual(LiveVideoControlErrorEnum.T2GServerOffline, lvcr.ResultCode);
		}

		/// <summary>Starts video streaming command request accepted without element.</summary>
		[Test]
		public void StartVideoStreamingCommandRequestAcceptedWithoutElement()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			string streamingUrl = string.Empty;
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.StartVideoStreamingCommand(ValidSessionId, targetAdress, streamingUrl);
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count > 0)), Times.Never());
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count == 0)), Times.Once());
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
		}

		/// <summary>Starts video streaming command request accepted with element.</summary>
		[Test]
		public void StartVideoStreamingCommandRequestAcceptedWithElement()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			string notificationURL = string.Empty;
			string streamingURL = "http://10.90.202.20/mediaweb/PIS.m3u8";
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(this.InitTrainElement());
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GetNotificationUrlByRequestId(It.IsAny<Guid>(), out notificationURL)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.StartVideoStreamingCommand(ValidSessionId, targetAdress, streamingURL);
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count == 0)), Times.Never());
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count > 0)), Times.Once());
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
		}

		/// <summary>Starts video streaming command request rejected because in automatic mode.</summary>
		[Test]
		public void StartVideoStreamingCommandInAutomaticMode()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			string streamingURL = "http://10.90.202.20/mediaweb/PIS.m3u8";
			string automaticURL = "my_automatic_url";
			SetAutomaticMode(automaticURL);
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(automaticURL);
			LiveVideoControlResult lvcr = this._lvcs.StartVideoStreamingCommand(ValidSessionId, targetAdress, streamingURL);
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count >= 0)), Times.Never());
			Assert.AreEqual(LiveVideoControlErrorEnum.AutomaticModeActivated, lvcr.ResultCode);
			Assert.AreEqual(automaticURL, lvcr.Url);
			SetManualMode();
		}

		#endregion

		#region StopVideoStreamingCommand

		/// <summary>Test a sent start video streaming command to embedded element.</summary>
		[Test]
		public void StopVideoStreamingCommandInvalidSessionId()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);
			LiveVideoControlResult lvcr = this._lvcs.StopVideoStreamingCommand(ValidSessionId, targetAdress);
			Assert.AreEqual(LiveVideoControlErrorEnum.InvalidSessionId, lvcr.ResultCode);
		}

		/// <summary>Stops video streaming command invalid request identifier.</summary>
		[Test]
		public void StopVideoStreamingCommandInvalidRequestId()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			Guid requestId = Guid.Empty;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			LiveVideoControlResult lvcr = this._lvcs.StopVideoStreamingCommand(ValidSessionId, targetAdress);
			Assert.AreEqual(LiveVideoControlErrorEnum.InvalidRequestID, lvcr.ResultCode);
		}

		/// <summary>Stops video streaming command unknown mission code.</summary>
		[Test]
		public void StopVideoStreamingCommandUnknownMissionCode()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.MissionCode;
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList;
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eElementNotFound;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.StopVideoStreamingCommand(ValidSessionId, targetAdress);
			Assert.AreEqual(LiveVideoControlErrorEnum.UnknownMissionId, lvcr.ResultCode);
		}

		/// <summary>Stops video streaming command unknown operator code.</summary>
		[Test]
		public void StopVideoStreamingCommandUnknownOperatorCode()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.MissionOperatorCode;
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList;
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eElementNotFound;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.StopVideoStreamingCommand(ValidSessionId, targetAdress);
			Assert.AreEqual(LiveVideoControlErrorEnum.UnknownMissionId, lvcr.ResultCode);
		}

		/// <summary>Stops video streaming command unknown element identifier.</summary>
		[Test]
		public void StopVideoStreamingCommandUnknownElementId()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList;
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eElementNotFound;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);

			LiveVideoControlResult lvcr = this._lvcs.StopVideoStreamingCommand(ValidSessionId, targetAdress);
			Assert.AreEqual(LiveVideoControlErrorEnum.UnknownElementId, lvcr.ResultCode);
		}

		/// <summary>Stops video streaming command t 2 g server offline.</summary>
		[Test]
		public void StopVideoStreamingCommandT2GServerOffline()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList;
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eT2GServerOffline;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.StopVideoStreamingCommand(ValidSessionId, targetAdress);
			Assert.AreEqual(LiveVideoControlErrorEnum.T2GServerOffline, lvcr.ResultCode);
		}

		/// <summary>Stops video streaming command request accepted.</summary>
		[Test]
		public void StopVideoStreamingCommandRequestAccepted()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.StopVideoStreamingCommand(ValidSessionId, targetAdress);
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
		}

		/// <summary>Stops video streaming command request accepted without element.</summary>
		[Test]
		public void StopVideoStreamingCommandRequestAcceptedWithoutElement()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			string streamingUrl = string.Empty;
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.StopVideoStreamingCommand(ValidSessionId, targetAdress);
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count > 0)), Times.Never());
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count == 0)), Times.Once());
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
		}

		/// <summary>Stops video streaming command request accepted with element.</summary>
		[Test]
		public void StopVideoStreamingCommandRequestAcceptedWithElement()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			string emptyString = string.Empty;
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(this.InitTrainElement());
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GetNotificationUrlByRequestId(It.IsAny<Guid>(), out emptyString)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.StopVideoStreamingCommand(ValidSessionId, targetAdress);
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count == 0)), Times.Never());
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count > 0)), Times.Once());
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
		}

		/// <summary>Stops video streaming command request rejected because in automatic mode.</summary>
		[Test]
		public void StopVideoStreamingCommandInAutomaticMode()
		{
			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			string automaticURL = "my_automatic_url";
			SetAutomaticMode(automaticURL);
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(automaticURL);
			LiveVideoControlResult lvcr = this._lvcs.StopVideoStreamingCommand(ValidSessionId, targetAdress);
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count >= 0)), Times.Never());
			Assert.AreEqual(LiveVideoControlErrorEnum.AutomaticModeActivated, lvcr.ResultCode);
			Assert.AreEqual(automaticURL, lvcr.Url);
			SetManualMode();
		}

		#endregion

		#region ChangeCommandMode

		/// <summary>Change mode command invalid session id.</summary>
		[Test]
		public void ChangeCommandModeInvalidSessionId()
		{
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);
			LiveVideoControlResult lvcr = this._lvcs.ChangeCommandMode(ValidSessionId, ValidStreamingUrl);
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count >= 0)), Times.Never());
			Assert.AreEqual(LiveVideoControlErrorEnum.InvalidSessionId, lvcr.ResultCode);
		}

		/// <summary>Change mode command while already in automatic mode.</summary>
		[Test]
		public void ChangeCommandModeAlreadyAutomatic()
		{
			SetAutomaticMode(ValidStreamingUrl);
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			LiveVideoControlResult lvcr = this._lvcs.ChangeCommandMode(ValidSessionId,
				"http://10.90.201.10/nomedia/BAD.m3u8");
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count >= 0)), Times.Never());
			Assert.AreEqual(LiveVideoControlErrorEnum.AutomaticModeActivated, lvcr.ResultCode);
			Assert.AreEqual(ValidStreamingUrl, lvcr.Url);
			SetManualMode();
		}

		/// <summary>Change mode command while element is offline.</summary>
		[Test]
		public void ChangeCommandModeRequestAcceptedAutomaticButElementOffline()
		{
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			AvailableElementData element = this.InitTrainElement();
			element.OnlineStatus = false;
			element.MissionState = MissionStateEnum.MI;
			elementList.Add(element);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.ChangeCommandMode(ValidSessionId, ValidStreamingUrl);
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count >= 0)), Times.Never());
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
			Assert.AreEqual(ValidStreamingUrl, lvcr.Url);
			SetManualMode();
		}

		/// <summary>Change mode command while element has no mission.</summary>
		[Test]
		public void ChangeCommandModeRequestAcceptedAutomaticButElementNoMission()
		{
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			AvailableElementData element = this.InitTrainElement();
			element.OnlineStatus = true;
			element.MissionState = MissionStateEnum.NI;
			elementList.Add(element);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.ChangeCommandMode(ValidSessionId, ValidStreamingUrl);
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count >= 0)), Times.Never());
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
			Assert.AreEqual(ValidStreamingUrl, lvcr.Url);
			SetManualMode();
		}

		/// <summary>Change mode command successfully setting automatic mode.</summary>
		[Test]
		public void ChangeCommandModeRequestAcceptedAutomatic()
		{
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			AvailableElementData element = this.InitTrainElement();
			element.OnlineStatus = true;
			element.MissionState = MissionStateEnum.MI;
			elementList.Add(element);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);
			LiveVideoControlResult lvcr = this._lvcs.ChangeCommandMode(ValidSessionId, ValidStreamingUrl);
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
			Assert.AreEqual(ValidStreamingUrl, lvcr.Url);
			SetManualMode();
		}

		/// <summary>Change mode command invalid session id (manual mode).</summary>
		[Test]
		public void ChangeCommandModeManualInvalidSessionId()
		{
			SetAutomaticMode(ValidStreamingUrl);
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);
			LiveVideoControlResult lvcr = this._lvcs.ChangeCommandMode(ValidSessionId, null);
			Assert.AreEqual(LiveVideoControlErrorEnum.InvalidSessionId, lvcr.ResultCode);
			SetManualMode();
		}

		/// <summary>Change mode command while already in manual mode.</summary>
		[Test]
		public void ChangeCommandModeAlreadyManual()
		{
			Guid requestId = Guid.Empty;
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			LiveVideoControlResult lvcr = this._lvcs.ChangeCommandMode(ValidSessionId, null);
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
		}

		/// <summary>Change mode command successfully setting manual mode.</summary>
		[Test]
		public void ChangeCommandModeRequestAcceptedManual()
		{
			Guid requestId = Guid.Empty;
			SetAutomaticMode(ValidStreamingUrl);
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			LiveVideoControlResult lvcr = this._lvcs.ChangeCommandMode(ValidSessionId, null);
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
			SetManualMode();
		}

		#endregion

		#region OnElementInfoChanged

		/// <summary>Calling OnElementInfoChanged with offline element.</summary>
		[Test]
		public void OnElementInfoChangedOfflineElement()
		{
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementEventArgs args = new ElementEventArgs();
			args.SystemInformation = new SystemInfo(
				"TRAIN-111",
				"",
				1,
				0,
				false,
				PIS.Ground.Core.Data.CommunicationLink.WIFI,
				new ServiceInfoList(),
				new PisBaseline(),
				new PisVersion(),
				new PisMission { MissionState = MissionStateEnum.MI },
				false);
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(this.InitTrainElement());
			T2GManagerErrorEnum elementReturns = T2GManagerErrorEnum.eSuccess;
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(ValidStreamingUrl);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(elementReturns);
			LiveVideoControlService.OnElementInfoChanged(null, args);
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count >= 0)), Times.Never());
		}

		/// <summary>Calling OnElementInfoChanged with element that is out of mission.</summary>
		[Test]
		public void OnElementInfoChangedNoMission()
		{
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementEventArgs args = new ElementEventArgs();
			args.SystemInformation = new SystemInfo(
				"TRAIN-111",
				"",
				1,
				0,
				true,
				PIS.Ground.Core.Data.CommunicationLink.WIFI,
				new ServiceInfoList(),
				new PisBaseline(),
				new PisVersion(),
				new PisMission { MissionState = MissionStateEnum.NI },
				false);
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(this.InitTrainElement());
			T2GManagerErrorEnum elementReturns = T2GManagerErrorEnum.eSuccess;
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(ValidStreamingUrl);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(elementReturns);
			LiveVideoControlService.OnElementInfoChanged(null, args);
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count >= 0)), Times.Never());
		}

		/// <summary>Calling OnElementInfoChanged while in automatic mode.</summary>
		[Test]
		public void OnElementInfoChangedStartingStreaming()
		{
			Guid requestId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
			ElementEventArgs args = new ElementEventArgs();

			// Adding a service for the element changed
			args.SystemInformation = new SystemInfo(
				"TRAIN-1",
				"",
				1,
				0,
				true,
				PIS.Ground.Core.Data.CommunicationLink.WIFI,
				new ServiceInfoList(new ServiceInfo[] {
                    new ServiceInfo((ushort)eServiceID.eSrvSIF_LiveVideoControlServer, "Test", 5, 5, false, "127.0.0.1", "", "", 12345)
                }),
				new PisBaseline(),
				new PisVersion(),
				new PisMission { MissionState = MissionStateEnum.MI },
				false);

			TargetAddressType targetAdress = new TargetAddressType();
			targetAdress.Id = "TRAIN-1";
			targetAdress.Type = AddressTypeEnum.Element;
			string streamingURL = "http://10.90.202.20/mediaweb/PIS.m3u8";

			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;

			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(this.InitTrainElement());

			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			this._configurationMock.SetupGet(x => x.AutomaticModeURL).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(It.IsAny<Guid>(), out requestId)).Returns(string.Empty);
			this._sessionManagerMock.Setup(x => x.GenerateRequestID(out requestId)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(returns);

			LiveVideoControlService.Initialize();
			LiveVideoControlService.LoadConfiguration(this._configurationMock.Object);

			// Adding a new command to startvideo
			// Going to call the AddRequestRange for the first time
			LiveVideoControlResult lvcr = this._lvcs.StartVideoStreamingCommand(ValidSessionId, targetAdress, streamingURL);

			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count == 0)), Times.Never());
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count > 0)), Times.Once());
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);

			// Calling the element Info Changed (with a service not available)
			LiveVideoControlService.OnElementInfoChanged(null, args);

			args.SystemInformation = new SystemInfo(
				"TRAIN-1",
				"",
				1,
				0,
				true,
				PIS.Ground.Core.Data.CommunicationLink.WIFI,
				new ServiceInfoList(new ServiceInfo[] {
                    new ServiceInfo((ushort)eServiceID.eSrvSIF_LiveVideoControlServer, "Test", 5, 5, true, "127.0.0.1", "", "", 12345)
                }),
				new PisBaseline(),
				new PisVersion(),
				new PisMission { MissionState = MissionStateEnum.MI },
				false);

			// Calling the element Info Changed (with a service available)
			// Going to call the AddRequestRange for the second time
			LiveVideoControlService.OnElementInfoChanged(null, args);

			// Should call the AddRequestRange twice with only one command to start the stream
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count == 0)), Times.Never());
			this._requestProcessorMock.Verify(x => x.AddRequestRange(It.Is<List<RequestContext>>(l => l.Count > 0)), Times.Exactly(2));
		}

		#endregion

		#region GetCommandMode

		/// <summary>Getting the command mode with an invalid session id.</summary>
		[Test]
		public void GetCommandModeInvalidSessionId()
		{
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(false);
			LiveVideoControlResult lvcr = this._lvcs.GetCommandMode(ValidSessionId);
			Assert.AreEqual(LiveVideoControlErrorEnum.InvalidSessionId, lvcr.ResultCode);
		}

		/// <summary>Getting the command mode when currently manual.</summary>
		[Test]
		public void GetCommandModeManual()
		{
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			LiveVideoControlResult lvcr = this._lvcs.GetCommandMode(ValidSessionId);
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
			Assert.AreEqual(null, lvcr.Url);
		}

		/// <summary>Getting the command mode when currently automatic.</summary>
		[Test]
		public void GetCommandModeAutomatic()
		{
			string inputURL = "my_url";
			SetAutomaticMode(inputURL);
			this._sessionManagerMock.Setup(x => x.IsSessionValid(It.IsAny<Guid>())).Returns(true);
			LiveVideoControlResult lvcr = this._lvcs.GetCommandMode(ValidSessionId);
			Assert.AreEqual(LiveVideoControlErrorEnum.RequestAccepted, lvcr.ResultCode);
			Assert.AreEqual(inputURL, lvcr.Url);
			SetManualMode();
		}

		#endregion
	}
}