//---------------------------------------------------------------------------------------------------
// <copyright file="InstantMessageTests.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using Moq;
using NUnit.Framework;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.InstantMessage;
using System.IO;
using System.Data.SqlClient;
using PIS.Ground.Core.SqlServerAccess;
using System.Reflection;
using System.Threading;
using System.ServiceModel;
using System.Collections.Generic;
using PIS.Ground.GroundCore.AppGround;

namespace PIS.Ground.InstantMessageTests
{
	/// <summary>InstantMessage service test class.</summary>
	[TestFixture, Category("InstantMessage")]
	public class InstantMessageTests : AssertionHelper
	{
		#region attributes
		
		/// <summary>The session manager mock.</summary>
		private Mock<ISessionManagerExtended> _sessionManagerMock;

		/// <summary>The train 2ground client mock.</summary>
		private Mock<IT2GManager> _train2groundClientMock;

		/// <summary>The notification sender mock.</summary>
		private Mock<INotificationSender> _notificationSenderMock;

		/// <summary>The log manager mock.</summary>
		private Mock<ILogManager> _logManagerMock;

		private string _databaseName;
		private string _databaseFilePath;
		private string _databaseLogPath;
		#endregion

		#region Tests management

		/// <summary>Initializes a new instance of the InstantMessageTests class.</summary>
		public InstantMessageTests()
		{
		}

		/// <summary>Test fixture setup.</summary>
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			_databaseName = "TestDatabase";

			string _executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty);

			string databaseFolderPath = _executionPath + "\\";

			_databaseFilePath = databaseFolderPath + _databaseName + ".mdf";

			_databaseLogPath = databaseFolderPath + _databaseName + "_log.ldf";

			HistoryLoggerConfiguration.SqlConnectionString = "Server=.\\SQLExpress;AttachDbFilename=" +
				_databaseFilePath + "; Database=" + _databaseName + ";Trusted_Connection=Yes;Connect Timeout=200";

			HistoryLoggerConfiguration.SqlCreateDbConnectionString = "Server=.\\SQLExpress;Integrated security=SSPI;database=master;Connect Timeout=200";

			HistoryLoggerConfiguration.CreateTableScriptPath = _executionPath + "\\..\\..\\..\\..\\" + "Services\\Maintenance\\App_Data\\wlad.sql";

			HistoryLoggerConfiguration.LogBackupPath = databaseFolderPath;

			HistoryLoggerConfiguration.Valid = true;

			HistoryLoggerConfiguration.Used = true;

			HistoryLoggerConfiguration.LogDataBaseStructureVersion = "4.0";

			HistoryLoggerConfiguration.MaximumLogMessageSize = 10;

			DropTestDb();

			HistoryLogger.Initialize();

		}

        /// <summary>
        /// Creates the default instant message service.
        /// </summary>
        /// <param name="keepOnlyLatestFreeTextRequest">Indicates if only the latest free message request shall be kept for a train.</param>
        /// <returns>The service created</returns>
        private InstantMessageService CreateDefaultInstantMessageService(bool keepOnlyLatestFreeTextRequest)
        {
            return new InstantMessageService(
                   _sessionManagerMock.Object,
                   _notificationSenderMock.Object,
                   _train2groundClientMock.Object,
                   _logManagerMock.Object,
                   keepOnlyLatestFreeTextRequest);
        }

		/// <summary>Setups called before each test to initialize variables.</summary>
		[SetUp]
		public void Setup()
		{
			_sessionManagerMock = new Mock<ISessionManagerExtended>();
			_train2groundClientMock = new Mock<IT2GManager>();
			_notificationSenderMock = new Mock<INotificationSender>();
			_logManagerMock = new Mock<ILogManager>();

		}

		/// <summary>Tear down called after each test to clean.</summary>
		[TearDown]
		public void TearDown()
		{
			if (HistoryLogger.EmptyDatabase() != ResultCodeEnum.RequestAccepted)
			{
				throw new Exception("Cannot empty the history database");
			}
		}

		/// <summary>Test fixture cleanup.</summary>
		[TestFixtureTearDown]
		public void MyCleanup()
		{
			DropTestDb();
		}

		private void DropTestDb()
		{
			if (File.Exists(_databaseFilePath))
			{
				string cmdDropDB =
					"IF EXISTS( select name from sys.databases where NAME= '" + _databaseName + "')" +
					" BEGIN DROP DATABASE [" + _databaseName + "] END";

				SqlConnection.ClearAllPools();
				SqlHelper.ExecuteNonQuery(HistoryLoggerConfiguration.SqlCreateDbConnectionString, System.Data.CommandType.Text, cmdDropDB);
				File.Delete(_databaseFilePath);
			}

			if (File.Exists(_databaseLogPath))
			{
				File.Delete(_databaseLogPath);
			}
		}


		#endregion

		#region Tests

		#region GetAvailableElementList

		/// <summary>Gets available element list test case with invalid session id result.</summary>
		[Test]
		public void GetAvailableElementListInvalidSessionId()
		{
			Guid sessionId = Guid.NewGuid();
			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(false);

            using (InstantMessageService instantMessageService = CreateDefaultInstantMessageService(false))
            {
                InstantMessageElementListResult result = instantMessageService.GetAvailableElementList(sessionId);

                ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();

                this._train2groundClientMock.Verify(w => w.GetAvailableElementDataList(out elementList), Times.Never());

                Assert.AreEqual(InstantMessageErrorEnum.InvalidSessionId, result.ResultCode);
            }
		}

		/// <summary>
		/// Gets available element list test case with element list not available result.
		/// </summary>
		[Test]
		public void GetAvailableElementListNotAvailable()
		{
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eElementNotFound;
			Guid sessionId = Guid.NewGuid();
			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(y => y.GetAvailableElementDataList(out elementList)).Returns(returns);

            using (InstantMessageService instantMessageService = CreateDefaultInstantMessageService(false))
            {
                InstantMessageElementListResult result = instantMessageService.GetAvailableElementList(sessionId);

                _train2groundClientMock.Verify(w => w.GetAvailableElementDataList(out elementList), Times.Once());
                Assert.AreEqual(InstantMessageErrorEnum.ElementListNotAvailable, result.ResultCode);
            }
		}

		/// <summary>Gets available element list test case with Request accepted result.</summary>
		[Test]
		public void GetAvailableElementListRequestAccepted()
		{
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);

            using (InstantMessageService instantMessageService = CreateDefaultInstantMessageService(false))
            {
                InstantMessageElementListResult result = instantMessageService.GetAvailableElementList(sessionId);

                _train2groundClientMock.Verify(w => w.GetAvailableElementDataList(out elementList), Times.Once());
                Assert.AreEqual(InstantMessageErrorEnum.RequestAccepted, result.ResultCode);
            }
		}

		#endregion

		#region ScheduledMessageLogging

		/// <summary>Verify that logging one entry for a schedule message behave as expected.</summary>
		[Test, Category("Logs")]
		public void ScheduledMessageLogs_ScheduleMessageOneEntryNominal()
		{
			ILogManager logManager = new LogManager();
			Guid requestId = Guid.NewGuid();
			Guid sessionId = Guid.NewGuid();
			ScheduledMessageType messageType = new ScheduledMessageType();

	        messageType.Identifier = "Sched1";
	        messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = new DateTime(2015,1,1);
			messageType.Period.EndDateTime = new DateTime(2100,12,31);
	        messageType.FreeText = "Schedule message text";
			InstantMessageService.ProcessSendScheduledMessageRequestContext requestContext = new InstantMessageService.ProcessSendScheduledMessageRequestContext("Train-104", requestId, sessionId, 1, messageType, logManager);

			requestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionMessageLimitExceededError);

			List<string> expectedStatuses = new List<string> { HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionMessageLimitExceededError) };

			Expect(GetHistoryLogStatusForRequest(requestContext.RequestId), Is.EquivalentTo(expectedStatuses), "Statuses in history log database does not match expected values");
		}

		/// <summary>Verify that logging two entries for a schedule message behave as expected.</summary>
		[Test, Category("Logs")]
		public void ScheduledMessageLogs_ScheduleMessageTwoEntriesNominal()
		{
			ILogManager logManager = new LogManager();
			Guid requestId = Guid.NewGuid();
			Guid sessionId = Guid.NewGuid();
			ScheduledMessageType messageType = new ScheduledMessageType();

			messageType.Identifier = "Sched1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = new DateTime(2015, 1, 1);
			messageType.Period.EndDateTime = new DateTime(2100, 12, 31);
			messageType.FreeText = "Schedule message text";
			InstantMessageService.ProcessSendScheduledMessageRequestContext requestContext = new InstantMessageService.ProcessSendScheduledMessageRequestContext("Train-104", requestId, sessionId, 1, messageType, logManager);

			requestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionProcessing);
			requestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionMessageLimitExceededError);

			List<string> expectedStatuses = new List<string> { 
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionMessageLimitExceededError) };

			Expect(GetHistoryLogStatusForRequest(requestContext.RequestId), Is.EquivalentTo(expectedStatuses), "Statuses in history log database does not match expected values");
		}

		/// <summary>Verify that logging recreate scheduled message entries automatically when deleted from history logs.</summary>
		[Test, Category("Logs")]
		public void ScheduledMessageLogs_ScheduleMessageRecreateScheduledMessageProperlyInHistoryLog()
		{
			ILogManager logManager = new LogManager();
			Guid requestId = Guid.NewGuid();
			Guid sessionId = Guid.NewGuid();
			ScheduledMessageType messageType = new ScheduledMessageType();

			messageType.Identifier = "Sched1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = new DateTime(2015, 1, 1);
			messageType.Period.EndDateTime = new DateTime(2100, 12, 31);
			messageType.FreeText = "Schedule message text";
			InstantMessageService.ProcessSendScheduledMessageRequestContext requestContext = new InstantMessageService.ProcessSendScheduledMessageRequestContext("Train-104", requestId, sessionId, 1, messageType, logManager);

			requestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionProcessing);
			requestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionMessageLimitExceededError);

			List<string> expectedStatuses = new List<string> { 
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionMessageLimitExceededError) };

			Expect(GetHistoryLogStatusForRequest(requestContext.RequestId), Is.EquivalentTo(expectedStatuses), "Statuses in history log database does not match expected values");

			HistoryLogger.EmptyDatabase();

			requestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionTimedOut);

			expectedStatuses = new List<string> { 
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionTimedOut) };

			Expect(GetHistoryLogStatusForRequest(requestContext.RequestId), Is.EquivalentTo(expectedStatuses), "LogsStatusInHistoryLog does not recreate the scheduled message when entries were deleted from the database");
		}

		/// <summary>Verify that logging one entry for a cancel schedule message behave as expected.</summary>
		[Test, Category("Logs")]
		public void ScheduledMessageLogs_CancelRequestOneEntryNominal()
		{
			ILogManager logManager = new LogManager();
			Guid cancelRequestId = Guid.NewGuid();
			Guid sessionId = Guid.NewGuid();
			Guid requestId = Guid.NewGuid();
			Guid cancelMessageRequestId = requestId;
			ScheduledMessageType messageType = new ScheduledMessageType();

			messageType.Identifier = "Sched1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = new DateTime(2015, 1, 1);
			messageType.Period.EndDateTime = new DateTime(2100, 12, 31);
			messageType.FreeText = "Schedule message text";
			InstantMessageService.ProcessSendScheduledMessageRequestContext requestContext = new InstantMessageService.ProcessSendScheduledMessageRequestContext("Train-104", requestId, sessionId, 1, messageType, logManager);
			requestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);

			InstantMessageService.ProcessCancelScheduledMessageRequestContext cancelRequestContext = new InstantMessageService.ProcessCancelScheduledMessageRequestContext("Train-104", cancelRequestId, sessionId, 1, cancelMessageRequestId, logManager);

			cancelRequestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionProcessing);

			List<string> expectedStatuses = new List<string> { HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing) };

			Expect(GetHistoryLogStatusForRequest(cancelRequestContext.CancelMessageRequestId, CommandType.CancelScheduledMessage), Is.EquivalentTo(expectedStatuses), "Statuses in history log database does not match expected values");
		}

		/// <summary>Verify that logging two entries for a cancel schedule message behave as expected.</summary>
		[Test, Category("Logs")]
		public void ScheduledMessageLogs_CancelRequestTwoEntrieNominal()
		{
			ILogManager logManager = new LogManager();
			Guid cancelRequestId = Guid.NewGuid();
			Guid sessionId = Guid.NewGuid();
			Guid requestId = Guid.NewGuid();
			Guid cancelMessageRequestId = requestId;
			ScheduledMessageType messageType = new ScheduledMessageType();

			messageType.Identifier = "Sched1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = new DateTime(2015, 1, 1);
			messageType.Period.EndDateTime = new DateTime(2100, 12, 31);
			messageType.FreeText = "Schedule message text";
			InstantMessageService.ProcessSendScheduledMessageRequestContext requestContext = new InstantMessageService.ProcessSendScheduledMessageRequestContext("Train-104", requestId, sessionId, 1, messageType, logManager);
			requestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionProcessing);

			InstantMessageService.ProcessCancelScheduledMessageRequestContext cancelRequestContext = new InstantMessageService.ProcessCancelScheduledMessageRequestContext("Train-104", cancelRequestId, sessionId, 1, cancelMessageRequestId, logManager);

			cancelRequestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionProcessing);
			cancelRequestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);

			List<string> expectedStatuses = new List<string> { 
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionSent)
			};

			Expect(GetHistoryLogStatusForRequest(cancelRequestContext.CancelMessageRequestId, CommandType.CancelScheduledMessage), Is.EquivalentTo(expectedStatuses), "Statuses in history log database does not match expected values");
		}

		/// <summary>Verify that logging one entry for a cancel schedule message behave as expected when there is not sceduled message associated.</summary>
		[Test, Category("Logs")]
		public void ScheduledMessageLogs_CancelRequestOneEntryWhenScheduleMessageDoesNotExist()
		{
			ILogManager logManager = new LogManager();
			Guid cancelRequestId = Guid.NewGuid();
			Guid sessionId = Guid.NewGuid();
			Guid requestId = Guid.NewGuid();
			Guid cancelMessageRequestId = requestId;

			InstantMessageService.ProcessCancelScheduledMessageRequestContext cancelRequestContext = new InstantMessageService.ProcessCancelScheduledMessageRequestContext("Train-104", cancelRequestId, sessionId, 1, cancelMessageRequestId, logManager);

			cancelRequestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionProcessing);

			List<string> expectedStatuses = new List<string> { HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing) };

			Expect(GetHistoryLogStatusForRequest(cancelRequestContext.CancelMessageRequestId, CommandType.CancelScheduledMessage), Is.EquivalentTo(expectedStatuses), "Statuses in history log database does not match expected values");
		}

		/// <summary>Verify that logging two entries for a cancel schedule message when no scheduled message exist in database behave as expected.</summary>
		[Test, Category("Logs")]
		public void ScheduledMessageLogs_CancelRequestTwoEntrieWhenScheduleMessageDoesNotExist()
		{
			ILogManager logManager = new LogManager();
			Guid cancelRequestId = Guid.NewGuid();
			Guid sessionId = Guid.NewGuid();
			Guid requestId = Guid.NewGuid();
			Guid cancelMessageRequestId = requestId;

			InstantMessageService.ProcessCancelScheduledMessageRequestContext cancelRequestContext = new InstantMessageService.ProcessCancelScheduledMessageRequestContext("Train-104", cancelRequestId, sessionId, 1, cancelMessageRequestId, logManager);

			cancelRequestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionProcessing);
			cancelRequestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);

			List<string> expectedStatuses = new List<string> { 
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionSent)
			};

			Expect(GetHistoryLogStatusForRequest(cancelRequestContext.CancelMessageRequestId, CommandType.CancelScheduledMessage), Is.EquivalentTo(expectedStatuses), "Statuses in history log database does not match expected values");
		}


		/// <summary>Verify that logging two entries for a cancel schedule request and the scheduled message need to be created when logging the second entry behave as expected.</summary>
		[Test, Category("Logs")]
		public void ScheduledMessageLogs_CancelRequestTwoEntrieWhenScheduleMessageDoesNotExistOnSecondLog()
		{
			ILogManager logManager = new LogManager();
			Guid cancelRequestId = Guid.NewGuid();
			Guid sessionId = Guid.NewGuid();
			Guid requestId = Guid.NewGuid();
			Guid cancelMessageRequestId = requestId;

			InstantMessageService.ProcessCancelScheduledMessageRequestContext cancelRequestContext = new InstantMessageService.ProcessCancelScheduledMessageRequestContext("Train-104", cancelRequestId, sessionId, 1, cancelMessageRequestId, logManager);

			cancelRequestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionProcessing);

			List<string> expectedStatuses = new List<string> { 
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing)
			};

			Expect(GetHistoryLogStatusForRequest(cancelRequestContext.CancelMessageRequestId, CommandType.CancelScheduledMessage), Is.EquivalentTo(expectedStatuses), "Statuses in history log database does not match expected values");

			HistoryLogger.EmptyDatabase();

			cancelRequestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);
			expectedStatuses = new List<string> { 
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionSent)
			};
			Expect(GetHistoryLogStatusForRequest(cancelRequestContext.CancelMessageRequestId, CommandType.CancelScheduledMessage), Is.EquivalentTo(expectedStatuses), "Statuses in history log database does not match expected values");
		}

		/// <summary>Verify that logging of cancel request and send scheduled message request behave as expected in the following scenario:
		/// 		 CancelRequest, SendMessage, SendMessage, CancelRequest, SendMessage, CancelRequest</summary>
		[Test, Category("Logs")]
		public void ScheduledMessageLogs_Scenario_Cancel_Send_Send_Cancel_Send_Cancel()
		{
			ILogManager logManager = new LogManager();
			Guid cancelRequestId = Guid.NewGuid();
			Guid sessionId = Guid.NewGuid();
			Guid requestId = Guid.NewGuid();
			Guid cancelMessageRequestId = requestId;
			ScheduledMessageType messageType = new ScheduledMessageType();

			messageType.Identifier = "Sched1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = new DateTime(2015, 1, 1);
			messageType.Period.EndDateTime = new DateTime(2100, 12, 31);
			messageType.FreeText = "Schedule message text";
			InstantMessageService.ProcessSendScheduledMessageRequestContext requestContext = new InstantMessageService.ProcessSendScheduledMessageRequestContext("Train-104", requestId, sessionId, 1, messageType, logManager);
			InstantMessageService.ProcessCancelScheduledMessageRequestContext cancelRequestContext = new InstantMessageService.ProcessCancelScheduledMessageRequestContext("Train-104", cancelRequestId, sessionId, 1, cancelMessageRequestId, logManager);

			cancelRequestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionProcessing);
			requestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionTimedOut);
			requestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionUnexpectedError);
			cancelRequestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionMessageLimitExceededError);
			requestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionReceived);
			cancelRequestContext.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionInvalidTextError);

			List<string> expectedStatuses = new List<string> { 
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionMessageLimitExceededError),
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionInvalidTextError)
			};

			Expect(GetHistoryLogStatusForRequest(cancelRequestContext.CancelMessageRequestId, CommandType.CancelScheduledMessage), Is.EquivalentTo(expectedStatuses), "Statuses in history log database does not match expected values");

			expectedStatuses = new List<string> { 
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionTimedOut),
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionUnexpectedError),
				HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionReceived)
			};

			Expect(GetHistoryLogStatusForRequest(requestContext.RequestId, CommandType.SendScheduledMessage), Is.EquivalentTo(expectedStatuses), "Statuses in history log database does not match expected values");
		}


		#endregion

		#region SendFreeTextMessage

		/// <summary>Verify that method SendFreeTextMessage send the message successfully to the train.</summary>
		[Test, Category("SendFreeTextMessage")]
		public void SendFreeTextMessage_NominalScenario()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			bool isTrain1Online = true;

			FreeTextMessageType freeTextMessageType = new FreeTextMessageType();
			freeTextMessageType.Identifier = "M1";
			freeTextMessageType.NumberOfRepetitions = 1;
			freeTextMessageType.DelayBetweenRepetitions = 0;
			freeTextMessageType.DisplayDuration = 45;
			freeTextMessageType.AttentionGetter = false;
			freeTextMessageType.FreeText = "This is a free text message";

			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageService.SendFreeTextMessage(sessionId, target, 2, freeTextMessageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendFreeTextMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendFreeTextMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendFreeTextRequestContext = instantMessageService.LastAddedRequest;

                    Expect(() => sendFreeTextRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "FreeText message still not transmitted after a period of 30 seconds");
                    Expect(sendFreeTextRequestContext.State, Is.EqualTo(RequestState.Completed), "State of free text message is not set to expected value");
                    Expect(trainService.SendFreeTextMessageCallCount, Is.EqualTo(1), "The free text message has not been transmitted to the train as expected.");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

        /// <summary>Verify that method SendFreeTextMessage send multiple messages successfully to the train when keep latest free text request is configured to value false.</summary>
        [Test, Category("SendFreeTextMessage")]
        public void SendFreeTextMessage_SendThreeRequestsToOfflineTrain_ExpectSendingAllRequestWhenTrainBecomeOnline()
        {
            Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

            ILogManager logManager = new LogManager();
            AvailableElementData train1 = CreateAvailableElement("TRAIN-1", false, "5.12.14.0");
            AvailableElementData train2 = CreateAvailableElement("TRAIN-2", false, "5.12.14.1");
            ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
            elementList.Add(train1);
            ElementList<AvailableElementData> elementList2 = new ElementList<AvailableElementData>();
            elementList2.Add(train2);
            T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
            Guid sessionId = Guid.NewGuid();
            TargetAddressType target = new TargetAddressType();
            target.Id = "TRAIN-1";
            target.Type = AddressTypeEnum.Element;
            TargetAddressType target2 = new TargetAddressType();
            target2.Id = "TRAIN-2";
            target2.Type = AddressTypeEnum.Element;

            bool isTrain1Online = false;
            bool isTrain2Online = false;
            uint requestTimeout = 2;

            FreeTextMessageType freeTextMessageType = new FreeTextMessageType();
            freeTextMessageType.Identifier = "M1";
            freeTextMessageType.NumberOfRepetitions = 1;
            freeTextMessageType.DelayBetweenRepetitions = 0;
            freeTextMessageType.DisplayDuration = 45;
            freeTextMessageType.AttentionGetter = false;
            freeTextMessageType.FreeText = "This is a free text message";

            ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
            ServiceInfo train2ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8201 /* port */);
            Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");
            Uri train2ServiceAddress = new Uri("http://127.0.0.1:8201/");

            _sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(target, out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(target2, out elementList2)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-2", out train2)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-2", out isTrain2Online)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-2", (int)eServiceID.eSrvSIF_InstantMessageServer, out train2ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

            TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();
            TrainInstantMessageServiceStub train2Service = new TrainInstantMessageServiceStub();

            using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (ServiceHost host2 = new ServiceHost(train2Service, train2ServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();
                    host2.Open();

                    // SEND REQUEST 1 to TRAIN 1
                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageService.SendFreeTextMessage(sessionId, target, requestTimeout, freeTextMessageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendFreeTextMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendFreeTextMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendFreeTextRequestContext1 = instantMessageService.LastAddedRequest;
                    instantMessageService.LastAddedRequest = null;

                    // SEND REQUEST 2 to TRAIN 1
                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageService.SendFreeTextMessage(sessionId, target, requestTimeout, freeTextMessageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendFreeTextMessage didn't accepted the valid request");
                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendFreeTextMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendFreeTextRequestContext2 = instantMessageService.LastAddedRequest;
                    instantMessageService.LastAddedRequest = null;

                    // SEND REQUEST 3 to TRAIN 2
                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageService.SendFreeTextMessage(sessionId, target2, requestTimeout, freeTextMessageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendFreeTextMessage didn't accepted the valid request");
                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendFreeTextMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendFreeTextRequestContext3 = instantMessageService.LastAddedRequest;
                    instantMessageService.LastAddedRequest = null;

                    // Sleep 5 seconds
                    Thread.Sleep(5 * 1000);

                    Expect(sendFreeTextRequestContext3.IsStateFinal, Is.False, "Expect that this free text message is not send");
                    Expect(sendFreeTextRequestContext2.IsStateFinal, Is.False, "Expect that this free text message is not send");
                    Expect(sendFreeTextRequestContext1.IsStateFinal, Is.False, "Expect that this free text message is not send");

                    // TRAIN-2 become online
                    train2.OnlineStatus = isTrain2Online = true;
                    _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-2", out isTrain2Online)).Returns(T2GManagerErrorEnum.eSuccess);

                    Expect(() => sendFreeTextRequestContext3.IsStateFinal, Is.True.After(30 * 1000, 250), "FreeText message still not transmitted after a period of 30 seconds");
                    Expect(sendFreeTextRequestContext2.IsStateFinal, Is.False, "Expect that this free text message is not send");
                    Expect(sendFreeTextRequestContext1.IsStateFinal, Is.False, "Expect that this free text message is not send");
                    Expect(sendFreeTextRequestContext3.State, Is.EqualTo(RequestState.Completed), "State of free text message is not set to expected value");
                    Expect(train2Service.SendFreeTextMessageCallCount, Is.EqualTo(1), "The free text message has not been transmitted to the train as expected.");
                    Expect(trainService.SendFreeTextMessageCallCount, Is.EqualTo(0), "The free text message has not been transmitted to the train as expected.");


                    // TRAIN-1 become online
                    train1.OnlineStatus = isTrain1Online = true;
                    _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);

                    Expect(() => sendFreeTextRequestContext1.IsStateFinal, Is.True.After(30 * 1000, 250), "FreeText message still not transmitted after a period of 30 seconds");
                    Expect(() => sendFreeTextRequestContext2.IsStateFinal, Is.True.After(30 * 1000, 250), "FreeText message still not transmitted after a period of 30 seconds");
                    Expect(sendFreeTextRequestContext2.State, Is.EqualTo(RequestState.Completed), "State of free text message is not set to expected value");
                    Expect(sendFreeTextRequestContext1.State, Is.EqualTo(RequestState.Completed), "State of free text message is not set to expected value");
                    Expect(train2Service.SendFreeTextMessageCallCount, Is.EqualTo(1), "The free text message has not been transmitted to the train as expected.");
                    Expect(trainService.SendFreeTextMessageCallCount, Is.EqualTo(2), "The free text message has not been transmitted to the train as expected.");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }

                    if (host2.State == CommunicationState.Faulted)
                    {
                        host2.Abort();
                    }
                }
            }
        }

        /// <summary>Verify that method SendFreeTextMessage send the message a expected to the train when kepp only latest free text request is configured to value true.</summary>
        [Test, Category("SendFreeTextMessage")]
        public void SendFreeTextMessage_SendThreeRequestsToOfflineTrain_WhenKeepingOnlyLatestFreeTextRequest()
        {
            Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

            ILogManager logManager = new LogManager();
            AvailableElementData train1 = CreateAvailableElement("TRAIN-1", false, "5.12.14.0");
            AvailableElementData train2 = CreateAvailableElement("TRAIN-2", false, "5.12.14.1");
            ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
            elementList.Add(train1);
            ElementList<AvailableElementData> elementList2 = new ElementList<AvailableElementData>();
            elementList2.Add(train2);
            T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
            Guid sessionId = Guid.NewGuid();
            TargetAddressType target = new TargetAddressType();
            target.Id = "TRAIN-1";
            target.Type = AddressTypeEnum.Element;
            TargetAddressType target2 = new TargetAddressType();
            target2.Id = "TRAIN-2";
            target2.Type = AddressTypeEnum.Element;

            bool isTrain1Online = false;
            bool isTrain2Online = false;
            uint requestTimeout = 2;

            FreeTextMessageType freeTextMessageType = new FreeTextMessageType();
            freeTextMessageType.Identifier = "M1";
            freeTextMessageType.NumberOfRepetitions = 1;
            freeTextMessageType.DelayBetweenRepetitions = 0;
            freeTextMessageType.DisplayDuration = 45;
            freeTextMessageType.AttentionGetter = false;
            freeTextMessageType.FreeText = "This is a free text message";

            ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
            ServiceInfo train2ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8201 /* port */);
            Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");
            Uri train2ServiceAddress = new Uri("http://127.0.0.1:8201/");

            _sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(target, out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(target2, out elementList2)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-2", out train2)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-2", out isTrain2Online)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-2", (int)eServiceID.eSrvSIF_InstantMessageServer, out train2ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

            TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();
            TrainInstantMessageServiceStub train2Service = new TrainInstantMessageServiceStub();

            using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (ServiceHost host2 = new ServiceHost(train2Service, train2ServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager,
                true /* keep only latest free text request */))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();
                    host2.Open();

                    // SEND REQUEST 1 to TRAIN 1
                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageService.SendFreeTextMessage(sessionId, target, requestTimeout, freeTextMessageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendFreeTextMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendFreeTextMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendFreeTextRequestContext1 = instantMessageService.LastAddedRequest;
                    instantMessageService.LastAddedRequest = null;

                    // SEND REQUEST 2 to TRAIN 1
                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageService.SendFreeTextMessage(sessionId, target, requestTimeout, freeTextMessageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendFreeTextMessage didn't accepted the valid request");
                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendFreeTextMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendFreeTextRequestContext2 = instantMessageService.LastAddedRequest;
                    instantMessageService.LastAddedRequest = null;

                    // SEND REQUEST 3 to TRAIN 2
                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageService.SendFreeTextMessage(sessionId, target2, requestTimeout, freeTextMessageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendFreeTextMessage didn't accepted the valid request");
                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendFreeTextMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendFreeTextRequestContext3 = instantMessageService.LastAddedRequest;
                    instantMessageService.LastAddedRequest = null;

                    // Sleep 5 seconds
                    Thread.Sleep(5 * 1000);

                    Expect(sendFreeTextRequestContext3.IsStateFinal, Is.False, "Expect that this free text message is not send");
                    Expect(sendFreeTextRequestContext2.IsStateFinal, Is.False, "Expect that this free text message is not send");
                    Expect(sendFreeTextRequestContext1.IsStateFinal, Is.True, "Expect that this free text message is canceled");
                    Expect(sendFreeTextRequestContext1.State, Is.EqualTo(RequestState.Error), "State of free text message is not set to expected value");

                    // TRAIN-2 become online
                    train2.OnlineStatus = isTrain2Online = true;
                    _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-2", out isTrain2Online)).Returns(T2GManagerErrorEnum.eSuccess);

                    Expect(() => sendFreeTextRequestContext3.IsStateFinal, Is.True.After(30 * 1000, 250), "FreeText message still not transmitted after a period of 30 seconds");
                    Expect(sendFreeTextRequestContext2.IsStateFinal, Is.False, "Expect that this free text message is not send");
                    Expect(sendFreeTextRequestContext3.State, Is.EqualTo(RequestState.Completed), "State of free text message is not set to expected value");
                    Expect(train2Service.SendFreeTextMessageCallCount, Is.EqualTo(1), "The free text message has not been transmitted to the train as expected.");
                    Expect(trainService.SendFreeTextMessageCallCount, Is.EqualTo(0), "The free text message has not been transmitted to the train as expected.");


                    // TRAIN-1 become online
                    train1.OnlineStatus = isTrain1Online = true;
                    _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);

                    Expect(() => sendFreeTextRequestContext1.IsStateFinal, Is.True.After(30 * 1000, 250), "FreeText message still not transmitted after a period of 30 seconds");
                    Expect(() => sendFreeTextRequestContext2.IsStateFinal, Is.True.After(30 * 1000, 250), "FreeText message still not transmitted after a period of 30 seconds");
                    Expect(sendFreeTextRequestContext2.State, Is.EqualTo(RequestState.Completed), "State of free text message is not set to expected value");
                    Expect(sendFreeTextRequestContext1.State, Is.EqualTo(RequestState.Error), "State of free text message is not set to expected value");
                    Expect(train2Service.SendFreeTextMessageCallCount, Is.EqualTo(1), "The free text message has not been transmitted to the train as expected.");
                    Expect(trainService.SendFreeTextMessageCallCount, Is.EqualTo(1), "The free text message has not been transmitted to the train as expected.");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }

                    if (host2.State == CommunicationState.Faulted)
                    {
                        host2.Abort();
                    }
                }
            }
        }

        /// <summary>Verify that method SendFreeTextMessage returns the correct value when no baseline is assigned to the train in the remote datastore.</summary>
        [Test, Category("SendFreeTextMessage")]
        public void SendFreeTextMessage_NobaselineFoundForTrain()
        {
            Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

            ILogManager logManager = new LogManager();
            AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
            ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
            elementList.Add(train1);
            T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
            Guid sessionId = Guid.NewGuid();
            TargetAddressType target = new TargetAddressType();
            target.Id = "TRAIN-1";
            target.Type = AddressTypeEnum.Element;
            bool isTrain1Online = true;

            FreeTextMessageType freeTextMessageType = new FreeTextMessageType();
            freeTextMessageType.Identifier = "M1";
            freeTextMessageType.NumberOfRepetitions = 1;
            freeTextMessageType.DelayBetweenRepetitions = 0;
            freeTextMessageType.DisplayDuration = 45;
            freeTextMessageType.AttentionGetter = false;
            freeTextMessageType.FreeText = "This is a free text message";

            ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
            Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

            _sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

            TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

            using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    instantMessageService.SetIsElementKnownByDatastoreReturnValue(false);

                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageService.SendFreeTextMessage(sessionId, target, 2, freeTextMessageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.NoBaselineFoundForElementId), "SendFreeTextMessage didn't return the expected value.");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
        }

		#endregion

		#region SendPredefinedMessage

		/// <summary>
		/// Verify that method SendPredefinedMessages send the message successfully to the train.
		/// </summary>
		[Test, Category("SendPredefinedMessages")]
		public void SendPredefinedMessages_NominalScenario()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			bool isTrain1Online = true;

			PredefinedMessageType predefinedMessageType = new PredefinedMessageType();
			predefinedMessageType.Identifier = "Predef1";
			predefinedMessageType.StationId = "S1";
			predefinedMessageType.CarId = 1;
			predefinedMessageType.Delay = 30;
			predefinedMessageType.DelayReason = "a lot of snow";
			predefinedMessageType.Hour = new DateTime(2015, 11, 25, 10, 0, 0, 0, DateTimeKind.Utc);

			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageServiceInterface.SendPredefinedMessages(sessionId, target, 2, new PredefinedMessageType[] { predefinedMessageType });
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendPredefinedMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendPredefinedMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendPredefinedRequestContext = instantMessageService.LastAddedRequest;

                    Expect(() => sendPredefinedRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "PredefinedMessage not sent to the train after a period of 30 seconds");
                    host.Close();

                    Expect(sendPredefinedRequestContext.State, Is.EqualTo(RequestState.Completed), "The state of the predefined request is not set to expected value after been sent.");
                    Expect(trainService.SendPredefinedMesssageCallCount, Is.EqualTo(1), "The predefined message has not been transmitted to the train as expected.");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

        /// <summary>
        /// Verify that method SendPredefinedMessages returns the correct value when no baseline is assigned to the train in the remote datastore.
        /// </summary>
        [Test, Category("SendPredefinedMessages")]
        public void SendPredefinedMessages_NobaselineFoundForTrain()
        {
            Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

            ILogManager logManager = new LogManager();
            AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
            ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
            elementList.Add(train1);
            T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
            Guid sessionId = Guid.NewGuid();
            TargetAddressType target = new TargetAddressType();
            target.Id = "TRAIN-1";
            target.Type = AddressTypeEnum.Element;
            bool isTrain1Online = true;

            PredefinedMessageType predefinedMessageType = new PredefinedMessageType();
            predefinedMessageType.Identifier = "Predef1";
            predefinedMessageType.StationId = "S1";
            predefinedMessageType.CarId = 1;
            predefinedMessageType.Delay = 30;
            predefinedMessageType.DelayReason = "a lot of snow";
            predefinedMessageType.Hour = new DateTime(2015, 11, 25, 10, 0, 0, 0, DateTimeKind.Utc);

            ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
            Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

            _sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

            TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

            using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    instantMessageService.SetIsElementKnownByDatastoreReturnValue(false);

                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageServiceInterface.SendPredefinedMessages(sessionId, target, 2, new PredefinedMessageType[] { predefinedMessageType });
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.NoBaselineFoundForElementId), "SendPredefinedMessages didn't return the expected value.");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
        }

		#endregion

		#region SendScheduledMessage

		/// <summary>Verify that method SendScheduleMessage send the message successfully to the train.</summary>
		[Test, Category("SendScheduledMessage")]
		public void SendScheduledMessage_NominalScenario()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			Guid generatedRequestId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			ScheduledMessageType messageType = new ScheduledMessageType();
			bool isTrain1Online = true;
			messageType.FreeText = "This is a scheduled message";
			messageType.Identifier = "M1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = DateTime.UtcNow;
			messageType.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(10);
			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    host.Open();

                    InstantMessageResult result = instantMessageService.SendScheduledMessage(sessionId, target, 1, messageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendScheduledMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext requestContext = instantMessageService.LastAddedRequest;

                    Expect(() => requestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "Scheduled message is not transmitted after a period of 30 seconds");
                    host.Close();

                    List<string> expectedStatuses = new List<string>{ HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionSent),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionReceived)
											};

                    Expect(() => GetHistoryLogStatusForRequest(requestContext.RequestId), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

        /// <summary>Verify that method SendScheduleMessage returns the correct value when no baseline is assigned to the train in the remote datastore.</summary>
        [Test, Category("SendScheduledMessage")]
        public void SendScheduledMessage_NobaselineFoundForTrain()
        {
            Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

            ILogManager logManager = new LogManager();
            AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
            ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
            elementList.Add(train1);
            T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
            Guid sessionId = Guid.NewGuid();
            Guid generatedRequestId = Guid.NewGuid();
            TargetAddressType target = new TargetAddressType();
            target.Id = "TRAIN-1";
            target.Type = AddressTypeEnum.Element;
            ScheduledMessageType messageType = new ScheduledMessageType();
            bool isTrain1Online = true;
            messageType.FreeText = "This is a scheduled message";
            messageType.Identifier = "M1";
            messageType.Period = new ScheduledPeriodType();
            messageType.Period.StartDateTime = DateTime.UtcNow;
            messageType.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(10);
            ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
            Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

            _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
            _sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

            TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

            using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    host.Open();
                    instantMessageService.SetIsElementKnownByDatastoreReturnValue(false);
                    InstantMessageResult result = instantMessageService.SendScheduledMessage(sessionId, target, 1, messageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.NoBaselineFoundForElementId), "SendScheduledMessage didn't return the expected value.");

                    host.Close();
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
        }

		/// <summary>Verify that method SendScheduleMessage send the message successfully to the train.</summary>
		[Test, Category("SendScheduledMessage")]
		public void SendScheduledMessageScenario_NominalScenario()
		{
			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			Guid generatedRequestId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			ScheduledMessageType messageType = new ScheduledMessageType();
			bool isTrain1Online = true;
			messageType.FreeText = "This is a scheduled message";
			messageType.Identifier = "M1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = DateTime.UtcNow;
			messageType.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(10);
			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    host.Open();

                    InstantMessageResult result = instantMessageService.SendScheduledMessage(sessionId, target, 1, messageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendScheduledMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext requestContext = instantMessageService.LastAddedRequest;

                    Expect(() => requestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "Scheduled message is not transmitted after a period of 30 seconds");
                    host.Close();

                    List<string> expectedStatuses = new List<string>{ HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionSent),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionReceived)
											};

                    Expect(() => GetHistoryLogStatusForRequest(requestContext.RequestId), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		/// <summary>Verify a scenario with method SendScheduleMessage to ensure that train not responding is managed properly.</summary>
		[Test, Category("SendScheduledMessage")]
		public void SendScheduledMessageScenario_EmbeddedSystemNotResponding()
		{
			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			Guid generatedRequestId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			ScheduledMessageType messageType = new ScheduledMessageType();
			bool isTrain1Online = true;
			messageType.FreeText = "This is a scheduled message";
			messageType.Identifier = "M1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = DateTime.UtcNow;
			messageType.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(2).AddSeconds(20);
			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);

			_sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
				_sessionManagerMock.Object,
				_notificationSenderMock.Object,
				_train2groundClientMock.Object,
				logManager))
			{

				InstantMessageResult result = instantMessageService.SendScheduledMessage(sessionId, target, 1, messageType);
				Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendScheduledMessage didn't accepted the valid request");

				Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendScheduledMessage method didn't created a request");
				InstantMessageService.InstantMessageRequestContext requestContext = instantMessageService.LastAddedRequest;

				Expect(() => requestContext.IsStateFinal, Is.True.After(3 * 60 * 1000, 250), "Scheduled message is not marked as timed out after a period of 3 minutes");

				List<string> expectedStatuses = new List<string>{ HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionWaitingToSend),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionTimedOut),
											};

				Expect(() => GetHistoryLogStatusForRequest(requestContext.RequestId), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values");
			}
		}

		/// <summary>Verify that method SendScheduleMessage send the message to the train but train return the error InvalidParamPeriod.</summary>
		[Test, Category("SendScheduledMessage")]
		public void SendScheduledMessageScenario_EmbeddedSystemReturnInvalidSchedulePeriod()
		{
			TestOneSendScheduledMessageScenarioThatFailOnembeddedSystem(PIS.Train.InstantMessage.ErrorType.InvalidParamPeriod, MessageStatusType.InstantMessageDistributionInvalidScheduledPeriodError);
		}

		/// <summary>Verify that method SendScheduleMessage send the message to the train but train return the error InvalidMessageId.</summary>
		[Test, Category("SendScheduledMessage")]
		public void SendScheduledMessageScenario_EmbeddedSystemReturnInvalidMessageId()
		{
			TestOneSendScheduledMessageScenarioThatFailOnembeddedSystem(PIS.Train.InstantMessage.ErrorType.InvalidMessageId, MessageStatusType.InstantMessageDistributionInvalidTemplateError);
		}

		/// <summary>Verify that method SendScheduleMessage send the message to the train but train return the error InvalidScheduledMessageId.</summary>
		[Test, Category("SendScheduledMessage")]
		public void SendScheduledMessageScenario_EmbeddedSystemReturnInvalidScheduledMessageId()
		{
			TestOneSendScheduledMessageScenarioThatFailOnembeddedSystem(PIS.Train.InstantMessage.ErrorType.InvalidScheduledMessageId, MessageStatusType.InstantMessageDistributionInvalidTemplateError);
		}

		/// <summary>Verify that method SendScheduleMessage send the message to the train but train return the error InvalidParamText.</summary>
		[Test, Category("SendScheduledMessage")]
		public void SendScheduledMessageScenario_EmbeddedSystemReturnInvalidParamText()
		{
			TestOneSendScheduledMessageScenarioThatFailOnembeddedSystem(PIS.Train.InstantMessage.ErrorType.InvalidParamText, MessageStatusType.InstantMessageDistributionInvalidTextError);
		}

		/// <summary>Verify that method SendScheduleMessage send the message to the train but train return the error IOError.</summary>
		[Test, Category("SendScheduledMessage")]
		public void SendScheduledMessageScenario_EmbeddedSystemReturnIOError()
		{
			TestOneSendScheduledMessageScenarioThatFailOnembeddedSystem(PIS.Train.InstantMessage.ErrorType.IOError, MessageStatusType.InstantMessageDistributionUnexpectedError);
		}

		/// <summary>Verify that method SendScheduleMessage send the message to the train but train return the error MessageLimitReached.</summary>
		[Test, Category("SendScheduledMessage")]
		public void SendScheduledMessageScenario_EmbeddedSystemReturnMessageLimitReached()
		{
			TestOneSendScheduledMessageScenarioThatFailOnembeddedSystem(PIS.Train.InstantMessage.ErrorType.MessageLimitReached, MessageStatusType.InstantMessageDistributionMessageLimitExceededError);
		}

		/// <summary>Verify that method SendScheduleMessage send the message to the train but train return the error ServiceInhibited.</summary>
		[Test, Category("SendScheduledMessage")]
		public void SendScheduledMessageScenario_EmbeddedSystemReturnServiceInhibited()
		{
			TestOneSendScheduledMessageScenarioThatFailOnembeddedSystem(PIS.Train.InstantMessage.ErrorType.ServiceInhibited, MessageStatusType.InstantMessageDistributionInhibited);
		}

		/// <summary>Verify that method SendScheduleMessage send the message to the train but train return and unexpected error code.</summary>
		[Test, Category("SendScheduledMessage")]
		public void SendScheduledMessageScenario_EmbeddedSystemReturnAndUnexpectedError()
		{
			TestOneSendScheduledMessageScenarioThatFailOnembeddedSystem(PIS.Train.InstantMessage.ErrorType.NoStationList, MessageStatusType.InstantMessageDistributionUnexpectedError);
		}

		private void TestOneSendScheduledMessageScenarioThatFailOnembeddedSystem(PIS.Train.InstantMessage.ErrorType embeddedError, MessageStatusType expectedStatus)
		{
			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			Guid generatedRequestId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			ScheduledMessageType messageType = new ScheduledMessageType();
			bool isTrain1Online = true;
			messageType.FreeText = "This is a scheduled message";
			messageType.Identifier = "M1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = DateTime.UtcNow;
			messageType.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(10);
			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();
			trainService.SendMessageReturnValue = embeddedError;

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    host.Open();

                    InstantMessageResult result = instantMessageService.SendScheduledMessage(sessionId, target, 1, messageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendScheduledMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext requestContext = instantMessageService.LastAddedRequest;

                    Expect(() => requestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "Scheduled message is not transmitted after a period of 30 seconds");
                    host.Close();

                    List<string> expectedStatuses = new List<string>{ HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionSent),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionReceived),
												HistoryLogger.GetMessageStatusString(expectedStatus)
											};

                    Expect(() => GetHistoryLogStatusForRequest(requestContext.RequestId), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}



		/// <summary>Create an available element structure.</summary>
		/// <param name="elementName">Name of the element.</param>
		/// <param name="isOnline">true if the train is online.</param>
		/// <param name="baselineVersion">The baseline version.</param>
		/// <returns>The new available element.</returns>
		private AvailableElementData CreateAvailableElement(string elementName, bool isOnline, string baselineVersion)
		{
			AvailableElementData trainInfo = new AvailableElementData();
			trainInfo.ElementNumber = elementName;
			trainInfo.OnlineStatus = isOnline;
			trainInfo.MissionState = MissionStateEnum.NI;
			trainInfo.MissionOperatorCode = "";
			trainInfo.MissionCommercialNumber = "";
			trainInfo.LmtPackageVersion = baselineVersion;
			trainInfo.PisBaselineData = CreateBaselineData(baselineVersion);
			trainInfo.PisBasicPackageVersion = "";
			return trainInfo;
		}

		/// <summary>Initialize a PisBaseline structure.</summary>
		/// <param name="version">The version of the baseline.</param>
		/// <returns>The new baseline data.</returns>
		private PisBaseline CreateBaselineData(string version)
		{
			PisBaseline baselineInfo = new PisBaseline();
			baselineInfo.ArchivedValidOut = "False";
			baselineInfo.ArchivedVersionLmtOut = string.Empty;
			baselineInfo.CurrentValidOut = "True";
			baselineInfo.CurrentVersionLmtOut = version;
			baselineInfo.CurrentVersionOut = version;
			baselineInfo.CurrentVersionPisInfotainmentOut = version;
			baselineInfo.CurrentVersionPisMissionOut = version;
			baselineInfo.CurrentVersionPisBaseOut = version;
			baselineInfo.CurrentForcedOut = "False";
			return baselineInfo;
		}

		private List<string> GetHistoryLogStatusForRequest(Guid requestId)
		{
			return GetHistoryLogStatusForRequest(requestId, PIS.Ground.Core.Data.CommandType.SendScheduledMessage);
		}

		private List<string> GetHistoryLogStatusForRequest(Guid requestId, PIS.Ground.Core.Data.CommandType commandType)
		{
			const string query = @"SELECT ST.Status FROM MessageStatus as MS
INNER JOIN MessageRequest as MR ON MR.MessageRequestID = MS.MessageRequestID
INNER JOIN CommandStatus as CS ON CS.CommandStatusId = MS.CommandStatusId
INNER JOIN StatusType as ST ON ST.StatusId = CS.StatusId
INNER JOIN CommandType CT ON CT.CommandID = MR.CommandID
where MR.RequestID = @RequestId and CT.Command = @CommandType";

			List<string> statuses = new List<string>();

			string commandTypeString = (commandType == PIS.Ground.Core.Data.CommandType.CancelScheduledMessage) ? "CancelScheduledMsg" : "SendScheduledMsg";

			using (SqlConnection connection = new SqlConnection(HistoryLoggerConfiguration.SqlConnectionString))
			using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, System.Data.CommandType.Text, query, new SqlParameter("@RequestId", requestId), new SqlParameter("@CommandType", commandTypeString)))
			{
				while (reader.Read())
				{
					statuses.Add(reader.GetString(0));
				}
			}

			return statuses;
		}

		#endregion 

		#region CancelScheduleMessage

		[Test, Category("CancelScheduledMessage")]
		public void CancelScheduleMessageScenario_CancelUnsentMessageSendCancellationRequestToEmbeddedSystem()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			Guid cancelRequestId = Guid.NewGuid();
			Guid generatedCancelRequestId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			ScheduledMessageType messageType = new ScheduledMessageType();
			bool isTrain1Online = true;
			messageType.FreeText = "This is a scheduled message";
			messageType.Identifier = "M1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = DateTime.UtcNow;
			messageType.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(10);
			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedCancelRequestId)).Returns(string.Empty);
			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    InstantMessageResult result = instantMessageServiceInterface.CancelScheduledMessage(sessionId, cancelRequestId, target, 2);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "CancelScheduledMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "CancelScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext cancelRequestContext = instantMessageService.LastAddedRequest;

                    Expect(() => cancelRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "Cancel scheduled message request is not transmitted after a period of 30 seconds");
                    host.Close();

                    Expect(trainService.CancelMessageCallCount, Is.EqualTo(1), "The CancelMessage on the train has not been called");

                    // When there is no record for the canceled message into the history log database, it's not possible to log information for the cancelled message.
                    List<string> expectedStatuses = new List<string>();

                    Expect(() => GetHistoryLogStatusForRequest(cancelRequestContext.RequestId, CommandType.CancelScheduledMessage), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		/// <summary>
		/// Scenario that very that unsent message in queue is canceled and not communication is performed with embedded system
		/// </summary>
		[Test, Category("CancelScheduledMessage")]
		public void CancelScheduleMessageScenario_CancelUnsentMessageInQueueCommunicateWithEmbeddedSystem()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", false, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			Guid cancelRequestId = Guid.NewGuid();
			Guid generatedCancelRequestId = Guid.NewGuid();
			Guid generatedRequestId = cancelRequestId;
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			ScheduledMessageType messageType = new ScheduledMessageType();
			bool isTrain1Online = false;
			messageType.FreeText = "This is a scheduled message";
			messageType.Identifier = "M1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = DateTime.UtcNow;
			messageType.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(10);
			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    InstantMessageResult result = instantMessageService.SendScheduledMessage(sessionId, target, 1, messageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendScheduledMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendRequestContext = instantMessageService.LastAddedRequest;

                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedCancelRequestId)).Returns(string.Empty);

                    result = instantMessageServiceInterface.CancelScheduledMessage(sessionId, cancelRequestId, target, 2);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "CancelScheduledMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "CancelScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext cancelRequestContext = instantMessageService.LastAddedRequest;

                    Expect(() => sendRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "SendScheduledMessage as not been canceled in queue after a period of 30 seconds");
                    Expect(cancelRequestContext.IsStateFinal, Is.False, "The cancel request shall not be in final state because the request has not been send as expected");

                    // Now, the train1 shall become online

                    train1.OnlineStatus = true;
                    isTrain1Online = true;
                    _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);


                    Expect(() => cancelRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "Cancel scheduled message request is not transmitted after a period of 30 seconds");
                    host.Close();

                    Expect(trainService.CancelMessageCallCount, Is.EqualTo(1), "The CancelMessage on the train has not been called as expected");

                    List<string> expectedStatuses = new List<string>{ HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionWaitingToSend),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionCanceled)
											};

                    Expect(() => GetHistoryLogStatusForRequest(sendRequestContext.RequestId), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values for the SendScheduledMessageRequest");

                    expectedStatuses = new List<string>{ HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionWaitingToSend),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionSent),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionReceived)
											};

                    Expect(() => GetHistoryLogStatusForRequest(sendRequestContext.RequestId, CommandType.CancelScheduledMessage), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values for the CancelMessageRequest");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		/// <summary>
		/// Scenario that very that sent message is canceled on the embedded system.
		/// </summary>
		[Test, Category("CancelScheduledMessage")]
		public void CancelScheduleMessageScenario_CancelSentMessageCommunicateWithEmbeddedSystem()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			Guid cancelRequestId = Guid.NewGuid();
			Guid generatedCancelRequestId = Guid.NewGuid();
			Guid generatedRequestId = cancelRequestId;
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			ScheduledMessageType messageType = new ScheduledMessageType();
			bool isTrain1Online = true;
			messageType.FreeText = "This is a scheduled message";
			messageType.Identifier = "M1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = DateTime.UtcNow;
			messageType.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(10);
			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    InstantMessageResult result = instantMessageService.SendScheduledMessage(sessionId, target, 1, messageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendScheduledMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendRequestContext = instantMessageService.LastAddedRequest;

                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedCancelRequestId)).Returns(string.Empty);

                    result = instantMessageServiceInterface.CancelScheduledMessage(sessionId, cancelRequestId, target, 2);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "CancelScheduledMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "CancelScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext cancelRequestContext = instantMessageService.LastAddedRequest;

                    Expect(() => cancelRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "Cancel scheduled message request is not transmitted after a period of 30 seconds");
                    Expect(sendRequestContext.IsStateFinal, Is.True, "The SendScheduledMessage request in the queue wasn't canceled by the CancelSechduledMessage request");
                    host.Close();

                    Expect(trainService.CancelMessageCallCount, Is.EqualTo(1), "The CancelMessage on the train wasn't called as expected");

                    List<string> expectedStatuses = new List<string>{ HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionSent),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionReceived)
											};

                    Expect(() => GetHistoryLogStatusForRequest(sendRequestContext.RequestId), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values for the SendScheduledMessageRequest");

                    expectedStatuses = new List<string>{ HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionSent),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionReceived)
											};

                    Expect(() => GetHistoryLogStatusForRequest(sendRequestContext.RequestId, CommandType.CancelScheduledMessage), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values for the CancelMessageRequest");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		#endregion

		#region CancelAllMessages

		/// <summary>
		/// Verify that CancelAllMessages request is transmitted to the embedded system even is not messages was previously sent.
		/// </summary>
		[Test, Category("CancelAllMessages")]
		public void CancelAllMessagesScenario_CancelCommunicateWithEmbeddedSystemEvenIfNoMessageHasBeenSent()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			Guid cancelRequestId = Guid.NewGuid();
			Guid generatedCancelRequestId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			ScheduledMessageType messageType = new ScheduledMessageType();
			bool isTrain1Online = true;
			messageType.FreeText = "This is a scheduled message";
			messageType.Identifier = "M1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = DateTime.UtcNow;
			messageType.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(10);
			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedCancelRequestId)).Returns(string.Empty);
			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    InstantMessageResult result = instantMessageServiceInterface.CancelAllMessages(sessionId, target, 2);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "CancelAllMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "CancelAllMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext cancelRequestContext = instantMessageService.LastAddedRequest;

                    Expect(() => cancelRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages request is not transmitted after a period of 30 seconds");
                    host.Close();

                    Expect(trainService.CancelAllMessagesCallCount, Is.EqualTo(1), "The CancelAllMessages on the train has not been called");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		#region Schedule Message request with Cancel all messages request

		/// <summary>
		/// Verify that scheduled message request are canceled in memory properly by executing 
		/// these requests in this order SendScheduledMessage, CancelAllMessage.
		/// The request shall terminated with theses status: Cancelled, Sent.
		/// When request are sent, the train is offline and then become online during the test.
		/// </summary>
		[Test, Category("CancelAllMessages"), Category("SendScheduledMessage")]
		public void CancelAllMessagesScenario_CancelPreviousScheduledMessageInMemoryAndCommunicateWithEmbeddedSystem()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", false, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			ScheduledMessageType messageType = new ScheduledMessageType();
			bool isTrain1Online = false;
			messageType.FreeText = "This is a scheduled message";
			messageType.Identifier = "M1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = DateTime.UtcNow;
			messageType.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(10);
			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    // Block transmitting the new request to the worker threads to ensure that send request and cancel request are added atomically
                    instantMessageService.AddNewRequestToRequestListBlocked = true;

                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);

                    InstantMessageResult result = instantMessageService.SendScheduledMessage(sessionId, target, 2, messageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendScheduledMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);

                    result = instantMessageServiceInterface.CancelAllMessages(sessionId, target, 3);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "CancelAllMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "CancelAllMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext cancelRequestContext = instantMessageService.LastAddedRequest;

                    // Transmit blocked request
                    instantMessageService.AddNewRequestToRequestListBlocked = false;

                    Expect(() => sendRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages didn't canceled the ScheduledMessage in memory after a period of 30 seconds");
                    Expect(cancelRequestContext.IsStateFinal, Is.False, "CancelAllMessages request is not expected to be terminated");

                    // The train now become online
                    train1.OnlineStatus = true;
                    isTrain1Online = true;
                    _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);

                    Expect(() => cancelRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages request is not transmitted after a period of 30 seconds");
                    host.Close();

                    Expect(trainService.CancelAllMessagesCallCount, Is.EqualTo(1), "The CancelAllMessages on the train has not been called");
                    Expect(trainService.SendScheduledMessageCallCount, Is.EqualTo(0), "The scheduled message has been transmitted to the train even if it was canceled in memory.");

                    // Check the status of the scheduled message into the history log
                    List<string> expectedStatuses = new List<string>{ HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionWaitingToSend),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionCanceled)
											};

                    Expect(() => GetHistoryLogStatusForRequest(sendRequestContext.RequestId), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values for the SendScheduledMessageRequest");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		/// <summary>
		/// Verify that scheduled message request are canceled in memory properly by executing 
		/// these requests in this order SendScheduledMessage, CancelAllMessage, SendScheduledMessage.
		/// The request shall terminated with theses status: Cancelled, Sent, Sent.
		/// When requests are sent, the train is offline and then become online.
		/// </summary>
		[Test, Category("CancelAllMessages"), Category("SendScheduledMessage")]
		public void CancelAllMessagesScenario_CancelOnlyPreviousScheduledMessageInMemoryAndCommunicateWithEmbeddedSystem()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", false, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			ScheduledMessageType messageType = new ScheduledMessageType();
			bool isTrain1Online = false;
			messageType.FreeText = "This is a scheduled message";
			messageType.Identifier = "M1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = DateTime.UtcNow;
			messageType.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(10);

			ScheduledMessageType messageType2 = new ScheduledMessageType();
			messageType2.FreeText = "This is a second scheduled message";
			messageType2.Identifier = "M1";
			messageType2.Period = new ScheduledPeriodType();
			messageType2.Period.StartDateTime = DateTime.UtcNow;
			messageType2.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(10);

			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    // Block transmitting the new request to the worker threads to ensure that send request and cancel request are added atomically
                    instantMessageService.AddNewRequestToRequestListBlocked = true;

                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageService.SendScheduledMessage(sessionId, target, 2, messageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendScheduledMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);

                    result = instantMessageServiceInterface.CancelAllMessages(sessionId, target, 3);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "CancelAllMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "CancelAllMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext cancelRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);

                    result = instantMessageService.SendScheduledMessage(sessionId, target, 2, messageType2);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendScheduledMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendRequestContext2 = instantMessageService.LastAddedRequest;

                    // Transmit blocked request
                    instantMessageService.AddNewRequestToRequestListBlocked = false;

                    Expect(() => sendRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages didn't canceled the ScheduledMessage in memory after a period of 30 seconds");
                    Expect(cancelRequestContext.IsStateFinal, Is.False, "CancelAllMessages request is not expected to be terminated");
                    Expect(sendRequestContext2.IsStateFinal, Is.False, "CancelAllMessages request canceled the scheduled message added after the cancel request");

                    // The train now become online
                    train1.OnlineStatus = true;
                    isTrain1Online = true;
                    _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);

                    Expect(() => cancelRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages request is not transmitted after a period of 30 seconds");
                    Expect(() => sendRequestContext2.IsStateFinal, Is.True.After(30 * 1000, 250), "Second scheduled message is not transmitted after a period of 30 seconds");
                    host.Close();

                    Expect(trainService.CancelAllMessagesCallCount, Is.EqualTo(1), "The CancelAllMessages on the train has not been called");
                    Expect(trainService.SendScheduledMessageCallCount, Is.EqualTo(1), "The second scheduled message has not been transmitted to the train as expected.");

                    // Check the status of the scheduled message into the history log
                    List<string> expectedStatuses = new List<string>{ HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionWaitingToSend),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionCanceled)
											};

                    Expect(() => GetHistoryLogStatusForRequest(sendRequestContext.RequestId), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values for the SendScheduledMessageRequest");

                    expectedStatuses = new List<string>{ HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionWaitingToSend),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionSent),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionReceived)
											};
                    Expect(() => GetHistoryLogStatusForRequest(sendRequestContext2.RequestId), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values for the second SendScheduledMessageRequest");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		/// <summary>
		/// Verify that scheduled message request are canceled in memory properly by executing 
		/// these requests in this order SendScheduledMessage, CancelAllMessage, SendScheduledMessage.
		/// The request shall terminated with theses status: Sent, Sent, Sent.
		/// When request are sent, the train is online.
		/// </summary>
		[Test, Category("CancelAllMessages"), Category("SendScheduledMessage")]
		public void CancelAllMessagesScenario_Send_ScheduledMessage_CancelAllMessage_ScheduledMessage_requests_when_train_is_online_are_managed_properly()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			ScheduledMessageType messageType = new ScheduledMessageType();
			bool isTrain1Online = true;
			messageType.FreeText = "This is a scheduled message";
			messageType.Identifier = "M1";
			messageType.Period = new ScheduledPeriodType();
			messageType.Period.StartDateTime = DateTime.UtcNow;
			messageType.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(10);

			ScheduledMessageType messageType2 = new ScheduledMessageType();
			messageType2.FreeText = "This is a second scheduled message";
			messageType2.Identifier = "M1";
			messageType2.Period = new ScheduledPeriodType();
			messageType2.Period.StartDateTime = DateTime.UtcNow;
			messageType2.Period.EndDateTime = messageType.Period.StartDateTime.AddMinutes(10);

			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageService.SendScheduledMessage(sessionId, target, 2, messageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendScheduledMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageServiceInterface.CancelAllMessages(sessionId, target, 3);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "CancelAllMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "CancelAllMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext cancelRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageService.SendScheduledMessage(sessionId, target, 2, messageType2);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendScheduledMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendRequestContext2 = instantMessageService.LastAddedRequest;

                    Expect(() => sendRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "First scheduled message is not transmitted after a period of 30 seconds");
                    Expect(() => cancelRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages request is not transmitted after a period of 30 seconds");
                    Expect(() => sendRequestContext2.IsStateFinal, Is.True.After(30 * 1000, 250), "Second scheduled message is not transmitted after a period of 30 seconds");

                    host.Close();

                    Expect(trainService.CancelAllMessagesCallCount, Is.EqualTo(1), "The CancelAllMessages on the train has not been called");
                    Expect(trainService.SendScheduledMessageCallCount, Is.EqualTo(2), "The scheduled messages has not been transmitted to the train as expected.");

                    // Check the status of the scheduled message into the history log
                    List<string> expectedStatuses = new List<string>{ HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionSent),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionReceived)
											};

                    Expect(() => GetHistoryLogStatusForRequest(sendRequestContext.RequestId), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values for the SendScheduledMessageRequest");

                    expectedStatuses = new List<string>{ HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionProcessing),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionSent),
												HistoryLogger.GetMessageStatusString(MessageStatusType.InstantMessageDistributionReceived)
											};
                    Expect(() => GetHistoryLogStatusForRequest(sendRequestContext2.RequestId), Is.EquivalentTo(expectedStatuses).After(5 * 1000, 250), "Statuses in history log database does not match expected values for the second SendScheduledMessageRequest");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		#endregion

		#region FreeText Message request with Cancel all messages request

		/// <summary>
		/// Verify that free text message request are canceled in memory properly by executing 
		/// these requests in this order SendFreeTextMessage, CancelAllMessage.
		/// The request shall terminated with theses status: Cancelled, Sent.
		/// When request are sent, the train is offline and then become online during the test.
		/// </summary>
		[Test, Category("CancelAllMessages"), Category("SendFreeTextMessage")]
		public void CancelAllMessagesScenario_CancelPreviousFreeTextMessageInMemoryAndCommunicateWithEmbeddedSystem()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", false, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			bool isTrain1Online = false;

			FreeTextMessageType freeTextMessageType = new FreeTextMessageType();
			freeTextMessageType.Identifier = "M1";
			freeTextMessageType.NumberOfRepetitions = 1;
			freeTextMessageType.DelayBetweenRepetitions = 0;
			freeTextMessageType.DisplayDuration = 45;
			freeTextMessageType.AttentionGetter = false;
			freeTextMessageType.FreeText = "This is a free text message";

			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    // Block transmitting the new request to the worker threads to ensure that send request and cancel request are added atomically
                    instantMessageService.AddNewRequestToRequestListBlocked = true;

                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageService.SendFreeTextMessage(sessionId, target, 2, freeTextMessageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendFreeTextMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendFreeTextMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendFreeTextRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageServiceInterface.CancelAllMessages(sessionId, target, 3);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "CancelAllMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "CancelAllMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext cancelRequestContext = instantMessageService.LastAddedRequest;

                    // Transmit blocked request
                    instantMessageService.AddNewRequestToRequestListBlocked = false;

                    Expect(() => sendFreeTextRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages didn't canceled the FreeTextMessage in memory after a period of 30 seconds");
                    Expect(cancelRequestContext.IsStateFinal, Is.False, "CancelAllMessages request is not expected to be terminated");

                    // The train now become online
                    train1.OnlineStatus = true;
                    isTrain1Online = true;
                    _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);

                    Expect(() => cancelRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages request is not transmitted after a period of 30 seconds");
                    host.Close();

                    Expect(trainService.CancelAllMessagesCallCount, Is.EqualTo(1), "The CancelAllMessages on the train has not been called");
                    Expect(trainService.SendFreeTextMessageCallCount, Is.EqualTo(0), "The freetext message has been transmitted to the train even if it was canceled in memory.");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		/// <summary>
		/// Verify that free text message request are canceled in memory properly by executing 
		/// these requests in this order SendFreeTextMessage, CancelAllMessage, SendFreeTextMessage.
		/// The request shall terminated with theses status: Cancelled, Sent, Sent.
		/// When requests are sent, the train is offline and then become online.
		/// </summary>
		[Test, Category("CancelAllMessages"), Category("SendFreeTextMessage")]
		public void CancelAllMessagesScenario_CancelOnlyPreviousFreeTextMessageInMemoryAndCommunicateWithEmbeddedSystem()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", false, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			bool isTrain1Online = false;

			FreeTextMessageType freeTextMessageType = new FreeTextMessageType();
			freeTextMessageType.Identifier = "M1";
			freeTextMessageType.NumberOfRepetitions = 1;
			freeTextMessageType.DelayBetweenRepetitions = 0;
			freeTextMessageType.DisplayDuration = 45;
			freeTextMessageType.AttentionGetter = false;
			freeTextMessageType.FreeText = "This is a free text message";

			FreeTextMessageType freeTextMessageType2 = new FreeTextMessageType();
			freeTextMessageType2.Identifier = "M1";
			freeTextMessageType2.NumberOfRepetitions = 1;
			freeTextMessageType2.DelayBetweenRepetitions = 0;
			freeTextMessageType2.DisplayDuration = 45;
			freeTextMessageType2.AttentionGetter = false;
			freeTextMessageType2.FreeText = "This is a second free text message";

			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    // Block transmitting the new request to the worker threads to ensure that send request and cancel request are added atomically
                    instantMessageService.AddNewRequestToRequestListBlocked = true;

                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageService.SendFreeTextMessage(sessionId, target, 2, freeTextMessageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendFreeTextMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendFreeTextMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendFreeTextRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageServiceInterface.CancelAllMessages(sessionId, target, 3);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "CancelAllMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "CancelAllMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext cancelRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageService.SendFreeTextMessage(sessionId, target, 2, freeTextMessageType2);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendFreeTextMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendFreeTextMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendFreeTextRequestContext2 = instantMessageService.LastAddedRequest;

                    // Transmit blocked request
                    instantMessageService.AddNewRequestToRequestListBlocked = false;

                    Expect(() => sendFreeTextRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages didn't canceled the FreeTextMessage in memory after a period of 30 seconds");
                    Expect(cancelRequestContext.IsStateFinal, Is.False, "CancelAllMessages request is not expected to be terminated");
                    Expect(sendFreeTextRequestContext2.IsStateFinal, Is.False, "CancelAllMessages request canceled the free text message added after the cancel request");

                    // The train now become online
                    train1.OnlineStatus = true;
                    isTrain1Online = true;
                    _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);

                    Expect(() => cancelRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages request is not transmitted after a period of 30 seconds");
                    Expect(() => sendFreeTextRequestContext2.IsStateFinal, Is.True.After(30 * 1000, 250), "Second free text message is not transmitted after a period of 30 seconds");
                    host.Close();

                    Expect(trainService.CancelAllMessagesCallCount, Is.EqualTo(1), "The CancelAllMessages on the train has not been called");
                    Expect(trainService.SendFreeTextMessageCallCount, Is.EqualTo(1), "The second free text message has not been transmitted to the train as expected.");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		/// <summary>
		/// Verify that free text message request are canceled in memory properly by executing 
		/// these requests in this order SendFreeTextMessage, CancelAllMessage, SendFreeTextMessage.
		/// The request shall terminated with theses status: Sent, Sent, Sent.
		/// When request are sent, the train is online.
		/// </summary>
		[Test, Category("CancelAllMessages"), Category("SendFreeTextMessage")]
		public void CancelAllMessagesScenario_Send_FreeTextMessage_CancelAllMessage_ScheduledMessage_requests_when_train_is_online_are_managed_properly()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;

			bool isTrain1Online = true;

			FreeTextMessageType freeTextMessageType = new FreeTextMessageType();
			freeTextMessageType.Identifier = "M1";
			freeTextMessageType.NumberOfRepetitions = 1;
			freeTextMessageType.DelayBetweenRepetitions = 0;
			freeTextMessageType.DisplayDuration = 45;
			freeTextMessageType.AttentionGetter = false;
			freeTextMessageType.FreeText = "This is a free text message";

			FreeTextMessageType freeTextMessageType2 = new FreeTextMessageType();
			freeTextMessageType2.Identifier = "M1";
			freeTextMessageType2.NumberOfRepetitions = 1;
			freeTextMessageType2.DelayBetweenRepetitions = 0;
			freeTextMessageType2.DisplayDuration = 45;
			freeTextMessageType2.AttentionGetter = false;
			freeTextMessageType2.FreeText = "This is a second free text message";

			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageService.SendFreeTextMessage(sessionId, target, 2, freeTextMessageType);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendFreeTextMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendFreeTextMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendFreeTextRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageServiceInterface.CancelAllMessages(sessionId, target, 3);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "CancelAllMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "CancelAllMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext cancelRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageService.SendFreeTextMessage(sessionId, target, 2, freeTextMessageType2);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendFreeTextMessage didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendScheduledMessage method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendFreeTextRequestContext2 = instantMessageService.LastAddedRequest;

                    Expect(() => sendFreeTextRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "First free text message is not transmitted after a period of 30 seconds");
                    Expect(() => cancelRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages request is not transmitted after a period of 30 seconds");
                    Expect(() => sendFreeTextRequestContext2.IsStateFinal, Is.True.After(30 * 1000, 250), "Second free text message is not transmitted after a period of 30 seconds");

                    host.Close();

                    Expect(trainService.CancelAllMessagesCallCount, Is.EqualTo(1), "The CancelAllMessages on the train has not been called");
                    Expect(trainService.SendFreeTextMessageCallCount, Is.EqualTo(2), "The free text messages has not been transmitted to the train as expected.");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		#endregion

		#region Predefined Message request with Cancel all messages request

		/// <summary>
		/// Verify that predefined message request are canceled in memory properly by executing 
		/// these requests in this order SendPredefinedMessage, CancelAllMessage.
		/// The request shall terminated with theses status: Cancelled, Sent.
		/// When request are sent, the train is offline and then become online during the test.
		/// </summary>
		[Test, Category("CancelAllMessages"), Category("SendPredefinedMessages")]
		public void CancelAllMessagesScenario_CancelPreviousPredefinedMessageInMemoryAndCommunicateWithEmbeddedSystem()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", false, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			bool isTrain1Online = false;

			PredefinedMessageType predefinedMessageType = new PredefinedMessageType();
			predefinedMessageType.Identifier = "Predef1";
			predefinedMessageType.StationId = "S1";
			predefinedMessageType.CarId = 1;
			predefinedMessageType.Delay = 30;
			predefinedMessageType.DelayReason = "a lot of snow";
			predefinedMessageType.Hour = new DateTime(2015, 11, 25, 10, 0, 0, 0, DateTimeKind.Utc);

			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    // Block transmitting the new request to the worker threads to ensure that send request and cancel request are added atomically
                    instantMessageService.AddNewRequestToRequestListBlocked = true;

                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageServiceInterface.SendPredefinedMessages(sessionId, target, 2, new PredefinedMessageType[] { predefinedMessageType });
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendPredefinedMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendPredefinedMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendPredefinedRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageServiceInterface.CancelAllMessages(sessionId, target, 3);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "CancelAllMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "CancelAllMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext cancelRequestContext = instantMessageService.LastAddedRequest;

                    // Transmit blocked request
                    instantMessageService.AddNewRequestToRequestListBlocked = false;

                    Expect(() => sendPredefinedRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages didn't canceled the PredefinedMessage in memory after a period of 30 seconds");
                    Expect(cancelRequestContext.IsStateFinal, Is.False, "CancelAllMessages request is not expected to be terminated");

                    // The train now become online
                    train1.OnlineStatus = true;
                    isTrain1Online = true;
                    _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);

                    Expect(() => cancelRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages request is not transmitted after a period of 30 seconds");
                    host.Close();

                    Expect(trainService.CancelAllMessagesCallCount, Is.EqualTo(1), "The CancelAllMessages on the train has not been called");
                    Expect(trainService.SendPredefinedMesssageCallCount, Is.EqualTo(0), "The predefined message has been transmitted to the train even if it was canceled in memory.");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		/// <summary>
		/// Verify that predefined message request are canceled in memory properly by executing 
		/// these requests in this order SendPredefinedMessage, CancelAllMessage, SendPredefinedMessage.
		/// The request shall terminated with theses status: Cancelled, Sent, Sent.
		/// When requests are sent, the train is offline and then become online.
		/// </summary>
		[Test, Category("CancelAllMessages"), Category("SendPredefinedMessages")]
		public void CancelAllMessagesScenario_CancelOnlyPreviousPredefinedMessageInMemoryAndCommunicateWithEmbeddedSystem()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", false, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;
			bool isTrain1Online = false;

			PredefinedMessageType predefinedMessageType = new PredefinedMessageType();
			predefinedMessageType.Identifier = "Predef1";
			predefinedMessageType.StationId = "S1";
			predefinedMessageType.CarId = 1;
			predefinedMessageType.Delay = 30;
			predefinedMessageType.DelayReason = "a lot of snow";
			predefinedMessageType.Hour = new DateTime(2015, 11, 25, 10, 0, 0, 0, DateTimeKind.Utc);

			PredefinedMessageType predefinedMessageType2 = new PredefinedMessageType();
			predefinedMessageType2.Identifier = "Predef2";
			predefinedMessageType2.StationId = "S1";
			predefinedMessageType2.CarId = 1;
			predefinedMessageType2.Delay = 30;
			predefinedMessageType2.DelayReason = "a lot of snow";
			predefinedMessageType2.Hour = new DateTime(2015, 11, 25, 10, 0, 0, 0, DateTimeKind.Utc);

			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    // Block transmitting the new request to the worker threads to ensure that send request and cancel request are added atomically
                    instantMessageService.AddNewRequestToRequestListBlocked = true;

                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageServiceInterface.SendPredefinedMessages(sessionId, target, 2, new PredefinedMessageType[] { predefinedMessageType });
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendPredefinedMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendPredefinedMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendPredefinedRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageServiceInterface.CancelAllMessages(sessionId, target, 3);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "CancelAllMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "CancelAllMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext cancelRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageServiceInterface.SendPredefinedMessages(sessionId, target, 2, new PredefinedMessageType[] { predefinedMessageType2 });
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendPredefinedMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendPredefinedMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendPredefinedRequestContext2 = instantMessageService.LastAddedRequest;

                    // Transmit blocked request
                    instantMessageService.AddNewRequestToRequestListBlocked = false;

                    Expect(() => sendPredefinedRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages didn't canceled the PredefinedMessage in memory after a period of 30 seconds");
                    Expect(cancelRequestContext.IsStateFinal, Is.False, "CancelAllMessages request is not expected to be terminated");
                    Expect(sendPredefinedRequestContext2.IsStateFinal, Is.False, "CancelAllMessages request canceled the predefined message added after the cancel request");

                    // The train now become online
                    train1.OnlineStatus = true;
                    isTrain1Online = true;
                    _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);

                    Expect(() => cancelRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages request is not transmitted after a period of 30 seconds");
                    Expect(() => sendPredefinedRequestContext2.IsStateFinal, Is.True.After(30 * 1000, 250), "Second predefined message is not transmitted after a period of 30 seconds");
                    host.Close();

                    Expect(trainService.CancelAllMessagesCallCount, Is.EqualTo(1), "The CancelAllMessages on the train has not been called");
                    Expect(trainService.SendPredefinedMesssageCallCount, Is.EqualTo(1), "The second predefined message has not been transmitted to the train as expected.");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		/// <summary>
		/// Verify that predefined message request are canceled in memory properly by executing 
		/// these requests in this order SendPredefinedMessages, CancelAllMessage, SendPredefinedMessages.
		/// The request shall terminated with theses status: Sent, Sent, Sent.
		/// When request are sent, the train is online.
		/// </summary>
		[Test, Category("CancelAllMessages"), Category("SendPredefinedMessages")]
		public void CancelAllMessagesScenario_Send_PredefinedMessage_CancelAllMessage_PredefinedMessage_requests_when_train_is_online_are_managed_properly()
		{
			Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

			ILogManager logManager = new LogManager();
			AvailableElementData train1 = CreateAvailableElement("TRAIN-1", true, "5.12.14.0");
			ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>();
			elementList.Add(train1);
			T2GManagerErrorEnum returns = T2GManagerErrorEnum.eSuccess;
			Guid sessionId = Guid.NewGuid();
			TargetAddressType target = new TargetAddressType();
			target.Id = "TRAIN-1";
			target.Type = AddressTypeEnum.Element;

			bool isTrain1Online = true;

			PredefinedMessageType predefinedMessageType = new PredefinedMessageType();
			predefinedMessageType.Identifier = "Predef1";
			predefinedMessageType.StationId = "S1";
			predefinedMessageType.CarId = 1;
			predefinedMessageType.Delay = 30;
			predefinedMessageType.DelayReason = "a lot of snow";
			predefinedMessageType.Hour = new DateTime(2015, 11, 25, 10, 0, 0, 0, DateTimeKind.Utc);

			PredefinedMessageType predefinedMessageType2 = new PredefinedMessageType();
			predefinedMessageType2.Identifier = "Predef2";
			predefinedMessageType2.StationId = "S1";
			predefinedMessageType2.CarId = 1;
			predefinedMessageType2.Delay = 30;
			predefinedMessageType2.DelayReason = "a lot of snow";
			predefinedMessageType2.Hour = new DateTime(2015, 11, 25, 10, 0, 0, 0, DateTimeKind.Utc);

			ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_InstantMessageServer, "InstantMessageServer", 0, 0, true, "127.0.0.1" /* ip */, "", "", 8200 /* port */);
			Uri trainServiceAddress = new Uri("http://127.0.0.1:8200/");

			_sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(returns);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByTargetAddress(It.IsAny<TargetAddressType>(), out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
			_train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_InstantMessageServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);

			TrainInstantMessageServiceStub trainService = new TrainInstantMessageServiceStub();

			using (ServiceHost host = new ServiceHost(trainService, trainServiceAddress))
            using (InstantMessageServiceStub instantMessageService = new InstantMessageServiceStub(
                _sessionManagerMock.Object,
                _notificationSenderMock.Object,
                _train2groundClientMock.Object,
                logManager))
            {
                try
                {
                    IInstantMessageService instantMessageServiceInterface = (IInstantMessageService)instantMessageService;
                    host.Open();

                    Guid generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    InstantMessageResult result = instantMessageServiceInterface.SendPredefinedMessages(sessionId, target, 2, new PredefinedMessageType[] { predefinedMessageType });
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendPredefinedMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendPredefinedMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendPredefinedRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageServiceInterface.CancelAllMessages(sessionId, target, 3);
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "CancelAllMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "CancelAllMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext cancelRequestContext = instantMessageService.LastAddedRequest;

                    generatedRequestId = Guid.NewGuid();
                    _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestId)).Returns(string.Empty);
                    result = instantMessageServiceInterface.SendPredefinedMessages(sessionId, target, 2, new PredefinedMessageType[] { predefinedMessageType2 });
                    Expect(result.ResultCode, Is.EqualTo(InstantMessageErrorEnum.RequestAccepted), "SendPredefinedMessages didn't accepted the valid request");

                    Expect(instantMessageService.LastAddedRequest, Is.Not.Null, "SendPredefinedMessages method didn't created a request");
                    InstantMessageService.InstantMessageRequestContext sendPredefinedRequestContext2 = instantMessageService.LastAddedRequest;

                    Expect(() => sendPredefinedRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "First free text message is not transmitted after a period of 30 seconds");
                    Expect(() => cancelRequestContext.IsStateFinal, Is.True.After(30 * 1000, 250), "CancelAllMessages request is not transmitted after a period of 30 seconds");
                    Expect(() => sendPredefinedRequestContext2.IsStateFinal, Is.True.After(30 * 1000, 250), "Second free text message is not transmitted after a period of 30 seconds");

                    host.Close();

                    Expect(trainService.CancelAllMessagesCallCount, Is.EqualTo(1), "The CancelAllMessages on the train has not been called");
                    Expect(trainService.SendPredefinedMesssageCallCount, Is.EqualTo(2), "The predefined messages has not been transmitted to the train as expected.");
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
		}

		#endregion

		#endregion

		#endregion
	}
}