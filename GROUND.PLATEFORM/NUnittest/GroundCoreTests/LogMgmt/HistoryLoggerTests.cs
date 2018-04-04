//---------------------------------------------------------------------------------------------------
// <copyright file="HistoryLoggerTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace GroundCoreTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using NUnit.Framework;
    using PIS.Ground.Core;
    using PIS.Ground.Core.SessionMgmt;
    using PIS.Ground.Core.T2G;
    using System.Reflection;
    using PIS.Ground.Core.LogMgmt;
    using PIS.Ground.Core.Data;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;
    using PIS.Ground.Core.SqlServerAccess;
    using System.IO;
	using System.Globalization;

    /// <summary>HistoryLoggerTests test fixture class.</summary>
    [TestFixture, Category("HistoryLogger")]
    public class HistoryLoggerTests : AssertionHelper
	{
		#region private constants
		
		private const string MessageContextTableName = "MessageContext";
		private const string MessageRequestTableName = "MessageRequest";
		private const string MessageStatusTableName = "MessageStatus";

		#endregion

		#region private fields

		string _databaseName = string.Empty;
        string _databaseFilePath = string.Empty;
        string _databaseLogPath = string.Empty;

        #endregion

        #region Tests managment       

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
            
            HistoryLoggerConfiguration.CreateTableScriptPath = _executionPath + "\\..\\..\\..\\..\\" + "Services\\Maintenance\\App_Data\\HistoryLogDataBaseScript.sql";

            HistoryLoggerConfiguration.LogBackupPath = databaseFolderPath;

            HistoryLoggerConfiguration.Valid = true;

            HistoryLoggerConfiguration.Used = true;

            HistoryLoggerConfiguration.LogDataBaseStructureVersion = "4.0";

            HistoryLoggerConfiguration.MaximumLogMessageSize = 10;

            DropTestDb();

            HistoryLogger.Initialize();
            
        }

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

        #region HistoryLoggerTests

        /// <summary>Task list is null.</summary>
        [Test]
        public void TestUpdateTrainBaselineStatus()
        {
            // Test UpdateTrainBaselineStatus - Insert
            Guid id1 = Guid.NewGuid();
            ResultCodeEnum error;
            error = HistoryLogger.UpdateTrainBaselineStatus("1", id1, 1, "TRAIN1", false, BaselineProgressStatusEnum.DEPLOYED, 
                                                            "1.1.1.1", "1.1.1.2", "1.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);
            Guid id2 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("2", id2, 2, "TRAIN2", false, BaselineProgressStatusEnum.TRANSFER_COMPLETED, 
                                                            "2.1.1.1", "2.1.1.2", "2.1.1.3");
			Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);
            Guid id3 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("3", id3, 3, "TRAIN3", false, BaselineProgressStatusEnum.TRANSFER_PAUSED, 
                                                            "3.1.1.1", "3.1.1.2", "3.1.1.3");
			Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);
            Guid id4 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("4", id4, 4, "TRAIN4", false, BaselineProgressStatusEnum.UPDATED, 
                                                            "4.1.1.1", "4.1.1.2", "4.1.1.3");
			Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Dictionary<string, TrainBaselineStatusData> dictionaryResponse;
            HistoryLogger.GetTrainBaselineStatus(out dictionaryResponse);

            Assert.AreEqual(dictionaryResponse["1"].TrainId, "1");
            Assert.AreEqual(dictionaryResponse["1"].RequestId, id1);
            Assert.AreEqual(dictionaryResponse["1"].TaskId, 1);
            Assert.AreEqual(dictionaryResponse["1"].TrainNumber, "TRAIN1");
            Assert.AreEqual(dictionaryResponse["1"].OnlineStatus, false);
            Assert.AreEqual(dictionaryResponse["1"].ProgressStatus, BaselineProgressStatusEnum.DEPLOYED);
            Assert.AreEqual(dictionaryResponse["1"].CurrentBaselineVersion, "1.1.1.1");
            Assert.AreEqual(dictionaryResponse["1"].FutureBaselineVersion, "1.1.1.2");
            Assert.AreEqual(dictionaryResponse["1"].PisOnBoardVersion, "1.1.1.3");

            Assert.AreEqual(dictionaryResponse["2"].TrainId, "2");
            Assert.AreEqual(dictionaryResponse["2"].RequestId, id2);
            Assert.AreEqual(dictionaryResponse["2"].TaskId, 2);
            Assert.AreEqual(dictionaryResponse["2"].TrainNumber, "TRAIN2");
            Assert.AreEqual(dictionaryResponse["2"].OnlineStatus, false);
            Assert.AreEqual(dictionaryResponse["2"].ProgressStatus, BaselineProgressStatusEnum.TRANSFER_COMPLETED);
            Assert.AreEqual(dictionaryResponse["2"].CurrentBaselineVersion, "2.1.1.1");
            Assert.AreEqual(dictionaryResponse["2"].FutureBaselineVersion, "2.1.1.2");
            Assert.AreEqual(dictionaryResponse["2"].PisOnBoardVersion, "2.1.1.3");

            Assert.AreEqual(dictionaryResponse["3"].TrainId, "3");
            Assert.AreEqual(dictionaryResponse["3"].RequestId, id3);
            Assert.AreEqual(dictionaryResponse["3"].TaskId, 3);
            Assert.AreEqual(dictionaryResponse["3"].TrainNumber, "TRAIN3");
            Assert.AreEqual(dictionaryResponse["3"].OnlineStatus, false);
            Assert.AreEqual(dictionaryResponse["3"].ProgressStatus, BaselineProgressStatusEnum.TRANSFER_PAUSED);
            Assert.AreEqual(dictionaryResponse["3"].CurrentBaselineVersion, "3.1.1.1");
            Assert.AreEqual(dictionaryResponse["3"].FutureBaselineVersion, "3.1.1.2");
            Assert.AreEqual(dictionaryResponse["3"].PisOnBoardVersion, "3.1.1.3");

            Assert.AreEqual(dictionaryResponse["4"].TrainId, "4");
            Assert.AreEqual(dictionaryResponse["4"].RequestId, id4);
            Assert.AreEqual(dictionaryResponse["4"].TaskId, 4);
            Assert.AreEqual(dictionaryResponse["4"].TrainNumber, "TRAIN4");
            Assert.AreEqual(dictionaryResponse["4"].OnlineStatus, false);
            Assert.AreEqual(dictionaryResponse["4"].ProgressStatus, BaselineProgressStatusEnum.UPDATED);
            Assert.AreEqual(dictionaryResponse["4"].CurrentBaselineVersion, "4.1.1.1");
            Assert.AreEqual(dictionaryResponse["4"].FutureBaselineVersion, "4.1.1.2");
            Assert.AreEqual(dictionaryResponse["4"].PisOnBoardVersion, "4.1.1.3");

            // Test UpdateTrainBaselineStatus - Update
            id1 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("1", id1, 5, "TRAIN1-1", true, BaselineProgressStatusEnum.TRANSFER_COMPLETED,
                                                            "11.1.1.1", "11.1.1.2", "11.1.1.3");
            Assert.AreEqual(error, ResultCodeEnum.RequestAccepted);
            id2 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("2", id2, 6, "TRAIN2-2", true, BaselineProgressStatusEnum.DEPLOYED,
                                                            "22.1.1.1", "22.1.1.2", "22.1.1.3");
            Assert.AreEqual(error, ResultCodeEnum.RequestAccepted);
            id3 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("3", id3, 7, "TRAIN3-3", true, BaselineProgressStatusEnum.TRANSFER_PLANNED,
                                                            "33.1.1.1", "33.1.1.2", "33.1.1.3");
            Assert.AreEqual(error, ResultCodeEnum.RequestAccepted);
            id4 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("4", id4, 8, "TRAIN4-4", true, BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS,
                                                            "44.1.1.1", "44.1.1.2", "44.1.1.3");
            Assert.AreEqual(error, ResultCodeEnum.RequestAccepted);

            HistoryLogger.GetTrainBaselineStatus(out dictionaryResponse);

            Assert.AreEqual(dictionaryResponse["1"].TrainId, "1");
            Assert.AreEqual(dictionaryResponse["1"].RequestId, id1);
            Assert.AreEqual(dictionaryResponse["1"].TaskId, 5);
            Assert.AreEqual(dictionaryResponse["1"].TrainNumber, "TRAIN1-1");
            Assert.AreEqual(dictionaryResponse["1"].OnlineStatus, true);
            Assert.AreEqual(dictionaryResponse["1"].ProgressStatus, BaselineProgressStatusEnum.TRANSFER_COMPLETED);
            Assert.AreEqual(dictionaryResponse["1"].CurrentBaselineVersion, "11.1.1.1");
            Assert.AreEqual(dictionaryResponse["1"].FutureBaselineVersion, "11.1.1.2");
            Assert.AreEqual(dictionaryResponse["1"].PisOnBoardVersion, "11.1.1.3");

            Assert.AreEqual(dictionaryResponse["2"].TrainId, "2");
            Assert.AreEqual(dictionaryResponse["2"].RequestId, id2);
            Assert.AreEqual(dictionaryResponse["2"].TaskId, 6);
            Assert.AreEqual(dictionaryResponse["2"].TrainNumber, "TRAIN2-2");
            Assert.AreEqual(dictionaryResponse["2"].OnlineStatus, true);
            Assert.AreEqual(dictionaryResponse["2"].ProgressStatus, BaselineProgressStatusEnum.DEPLOYED);
            Assert.AreEqual(dictionaryResponse["2"].CurrentBaselineVersion, "22.1.1.1");
            Assert.AreEqual(dictionaryResponse["2"].FutureBaselineVersion, "22.1.1.2");
            Assert.AreEqual(dictionaryResponse["2"].PisOnBoardVersion, "22.1.1.3");

            Assert.AreEqual(dictionaryResponse["3"].TrainId, "3");
            Assert.AreEqual(dictionaryResponse["3"].RequestId, id3);
            Assert.AreEqual(dictionaryResponse["3"].TaskId, 7);
            Assert.AreEqual(dictionaryResponse["3"].TrainNumber, "TRAIN3-3");
            Assert.AreEqual(dictionaryResponse["3"].OnlineStatus, true);
            Assert.AreEqual(dictionaryResponse["3"].ProgressStatus, BaselineProgressStatusEnum.TRANSFER_PLANNED);
            Assert.AreEqual(dictionaryResponse["3"].CurrentBaselineVersion, "33.1.1.1");
            Assert.AreEqual(dictionaryResponse["3"].FutureBaselineVersion, "33.1.1.2");
            Assert.AreEqual(dictionaryResponse["3"].PisOnBoardVersion, "33.1.1.3");

            Assert.AreEqual(dictionaryResponse["4"].TrainId, "4");
            Assert.AreEqual(dictionaryResponse["4"].RequestId, id4);
            Assert.AreEqual(dictionaryResponse["4"].TaskId, 8);
            Assert.AreEqual(dictionaryResponse["4"].TrainNumber, "TRAIN4-4");
            Assert.AreEqual(dictionaryResponse["4"].OnlineStatus, true);
            Assert.AreEqual(dictionaryResponse["4"].ProgressStatus, BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS);
            Assert.AreEqual(dictionaryResponse["4"].CurrentBaselineVersion, "44.1.1.1");
            Assert.AreEqual(dictionaryResponse["4"].FutureBaselineVersion, "44.1.1.2");
            Assert.AreEqual(dictionaryResponse["4"].PisOnBoardVersion, "44.1.1.3");

            HistoryLogger.GetTrainBaselineStatusByProgressStatus(BaselineProgressStatusEnum.TRANSFER_COMPLETED, out dictionaryResponse);
            Assert.AreEqual(dictionaryResponse.ContainsKey("1"), true);
            Assert.AreEqual(dictionaryResponse.Count, 1);

            // Checking the train id 1
            Assert.AreEqual(dictionaryResponse["1"].TrainId, "1");
            Assert.AreEqual(dictionaryResponse["1"].RequestId, id1);
            Assert.AreEqual(dictionaryResponse["1"].TaskId, 5);
            Assert.AreEqual(dictionaryResponse["1"].TrainNumber, "TRAIN1-1");
            Assert.AreEqual(dictionaryResponse["1"].OnlineStatus, true);
            Assert.AreEqual(dictionaryResponse["1"].ProgressStatus, BaselineProgressStatusEnum.TRANSFER_COMPLETED);
            Assert.AreEqual(dictionaryResponse["1"].CurrentBaselineVersion, "11.1.1.1");
            Assert.AreEqual(dictionaryResponse["1"].FutureBaselineVersion, "11.1.1.2");
            Assert.AreEqual(dictionaryResponse["1"].PisOnBoardVersion, "11.1.1.3");

            //Test GetTrainBaselineStatusByProgressStatus
            HistoryLogger.GetTrainBaselineStatusByProgressStatus(BaselineProgressStatusEnum.DEPLOYED, out dictionaryResponse);
            Assert.AreEqual(dictionaryResponse.ContainsKey("2"), true);
            Assert.AreEqual(dictionaryResponse.Count, 1);

            Assert.AreEqual(dictionaryResponse["2"].TrainId, "2");
            Assert.AreEqual(dictionaryResponse["2"].RequestId, id2);
            Assert.AreEqual(dictionaryResponse["2"].TaskId, 6);
            Assert.AreEqual(dictionaryResponse["2"].TrainNumber, "TRAIN2-2");
            Assert.AreEqual(dictionaryResponse["2"].OnlineStatus, true);
            Assert.AreEqual(dictionaryResponse["2"].ProgressStatus, BaselineProgressStatusEnum.DEPLOYED);
            Assert.AreEqual(dictionaryResponse["2"].CurrentBaselineVersion, "22.1.1.1");
            Assert.AreEqual(dictionaryResponse["2"].FutureBaselineVersion, "22.1.1.2");
            Assert.AreEqual(dictionaryResponse["2"].PisOnBoardVersion, "22.1.1.3");


            HistoryLogger.GetTrainBaselineStatusByProgressStatus(BaselineProgressStatusEnum.TRANSFER_PLANNED, out dictionaryResponse);
            Assert.AreEqual(dictionaryResponse.ContainsKey("3"), true);
            Assert.AreEqual(dictionaryResponse.Count, 1);

            Assert.AreEqual(dictionaryResponse["3"].TrainId, "3");
            Assert.AreEqual(dictionaryResponse["3"].RequestId, id3);
            Assert.AreEqual(dictionaryResponse["3"].TaskId, 7);
            Assert.AreEqual(dictionaryResponse["3"].TrainNumber, "TRAIN3-3");
            Assert.AreEqual(dictionaryResponse["3"].OnlineStatus, true);
            Assert.AreEqual(dictionaryResponse["3"].ProgressStatus, BaselineProgressStatusEnum.TRANSFER_PLANNED);
            Assert.AreEqual(dictionaryResponse["3"].CurrentBaselineVersion, "33.1.1.1");
            Assert.AreEqual(dictionaryResponse["3"].FutureBaselineVersion, "33.1.1.2");
            Assert.AreEqual(dictionaryResponse["3"].PisOnBoardVersion, "33.1.1.3");

            HistoryLogger.GetTrainBaselineStatusByProgressStatus(BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS, out dictionaryResponse);
            Assert.AreEqual(dictionaryResponse.ContainsKey("4"), true);
            Assert.AreEqual(dictionaryResponse.Count, 1);

            Assert.AreEqual(dictionaryResponse["4"].TrainId, "4");
            Assert.AreEqual(dictionaryResponse["4"].RequestId, id4);
            Assert.AreEqual(dictionaryResponse["4"].TaskId, 8);
            Assert.AreEqual(dictionaryResponse["4"].TrainNumber, "TRAIN4-4");
            Assert.AreEqual(dictionaryResponse["4"].OnlineStatus, true);
            Assert.AreEqual(dictionaryResponse["4"].ProgressStatus, BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS);
            Assert.AreEqual(dictionaryResponse["4"].CurrentBaselineVersion, "44.1.1.1");
            Assert.AreEqual(dictionaryResponse["4"].FutureBaselineVersion, "44.1.1.2");
            Assert.AreEqual(dictionaryResponse["4"].PisOnBoardVersion, "44.1.1.3");


            // Test Delete
            HistoryLogger.CleanTrainBaselineStatus("1");
            HistoryLogger.GetTrainBaselineStatus(out dictionaryResponse);

            Assert.AreEqual(dictionaryResponse.ContainsKey("1"), false);
            Assert.AreEqual(dictionaryResponse.ContainsKey("2"), true);
            Assert.AreEqual(dictionaryResponse.ContainsKey("3"), true);
            Assert.AreEqual(dictionaryResponse.ContainsKey("4"), true);
            Assert.AreEqual(dictionaryResponse.Count, 3);

            HistoryLogger.CleanTrainBaselineStatus("2");
            HistoryLogger.GetTrainBaselineStatus(out dictionaryResponse);

            Assert.AreEqual(dictionaryResponse.ContainsKey("1"), false);
            Assert.AreEqual(dictionaryResponse.ContainsKey("2"), false);
            Assert.AreEqual(dictionaryResponse.ContainsKey("3"), true);
            Assert.AreEqual(dictionaryResponse.ContainsKey("4"), true);
            Assert.AreEqual(dictionaryResponse.Count, 2);

            HistoryLogger.CleanTrainBaselineStatus("3");
            HistoryLogger.GetTrainBaselineStatus(out dictionaryResponse);

            Assert.AreEqual(dictionaryResponse.ContainsKey("1"), false);
            Assert.AreEqual(dictionaryResponse.ContainsKey("2"), false);
            Assert.AreEqual(dictionaryResponse.ContainsKey("3"), false);
            Assert.AreEqual(dictionaryResponse.ContainsKey("4"), true);
            Assert.AreEqual(dictionaryResponse.Count, 1);

            HistoryLogger.CleanTrainBaselineStatus("4");
            HistoryLogger.GetTrainBaselineStatus(out dictionaryResponse);

            Assert.AreEqual(dictionaryResponse.Count, 0);

        }

		[Test]
		public void TestMethodMarkPendingMessagesAsCanceledByStartup_BehaveAsExpected_WhenDatabaseIsEmpty()
		{
			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(0), "Expect that table MessageContext is empty");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(0), "Expect that table MessageRequest is empty");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(0), "Expect that table MessageStatus is empty");

			TestDelegate method = () => HistoryLogger.MarkPendingMessagesAsCanceledByStartup();
			Assert.That<TestDelegate>(ref method, Throws.Nothing, "Method MarkPendingMessagesAsCanceledByStartup failed while expecting not");

			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(0), "Table MessageContext has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(0), "Table MessageRequest has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(0), "Table MessageStatus has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");

		}

		[Test]
		public void TestMethodMarkPendingMessagesAsCanceledByStartup_BehaveAsExpected_WhenDatabaseContainOneRecordInFinalState()
		{
			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(0), "Expect that table MessageContext is empty");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(0), "Expect that table MessageRequest is empty");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(0), "Expect that table MessageStatus is empty");

			string messageText = "This is a scheduledMessage";
			Guid requestId = Guid.NewGuid();
			string trainId = "TRAIN-1";
			DateTime startDate = DateTime.UtcNow;
			DateTime endDate = startDate + new TimeSpan(1,0,0,0);

			ResultCodeEnum result = HistoryLogger.WriteLog(messageText, requestId, CommandType.SendScheduledMessage, trainId, MessageStatusType.InstantMessageDistributionProcessing, startDate, endDate);
			Expect(result, Is.EqualTo(ResultCodeEnum.RequestAccepted), "Failed to insert schedule message into the history log");

			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(1), "Table MessageContext wasn't updated as expected by method HistoryLogger.WriteLog");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(1), "Table MessageRequest wasn't updated as expected by method HistoryLogger.WriteLog");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(1), "Table MessageStatus wasn't updated as expected by method HistoryLogger.WriteLog");

			result = HistoryLogger.UpdateMessageStatus(trainId, requestId, MessageStatusType.InstantMessageDistributionSent, CommandType.SendScheduledMessage);
			Expect(result, Is.EqualTo(ResultCodeEnum.RequestAccepted), "Failed to update status of a request");

			result = HistoryLogger.UpdateMessageStatus(trainId, requestId, MessageStatusType.InstantMessageDistributionReceived, CommandType.SendScheduledMessage);
			Expect(result, Is.EqualTo(ResultCodeEnum.RequestAccepted), "Failed to update status of a request");


			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(1), "Table MessageContext has been updated by method HistoryLogger.UpdateMessageStatus while expecting not");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(1), "Table MessageRequest has been updated by method HistoryLogger.UpdateMessageStatus while expecting not");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(3), "Table MessageStatus wasn't updated as expected by method HistoryLogger.UpdateMessageStatus");


			TestDelegate method = () => HistoryLogger.MarkPendingMessagesAsCanceledByStartup();
			Assert.That<TestDelegate>(ref method, Throws.Nothing, "Method MarkPendingMessagesAsCanceledByStartup failed while expecting not");

			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(1), "Table MessageContext has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(1), "Table MessageRequest has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(3), "Table MessageStatus has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");
		}

		[Test]
		public void TestMethodMarkPendingMessagesAsCanceledByStartup_BehaveAsExpected_WhenDatabaseContainOneRecordInProgressState()
		{
			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(0), "Expect that table MessageContext is empty");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(0), "Expect that table MessageRequest is empty");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(0), "Expect that table MessageStatus is empty");

			string messageText = "This is a scheduledMessage in progress";
			Guid requestId = Guid.NewGuid();
			string trainId = "TRAIN-1";
			DateTime startDate = DateTime.UtcNow;
			DateTime endDate = startDate + new TimeSpan(1, 0, 0, 0);

			ResultCodeEnum result = HistoryLogger.WriteLog(messageText, requestId, CommandType.SendScheduledMessage, trainId, MessageStatusType.InstantMessageDistributionProcessing, startDate, endDate);
			Expect(result, Is.EqualTo(ResultCodeEnum.RequestAccepted), "Failed to insert schedule message into the history log");

			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(1), "Table MessageContext wasn't updated as expected by method HistoryLogger.WriteLog");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(1), "Table MessageRequest wasn't updated as expected by method HistoryLogger.WriteLog");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(1), "Table MessageStatus wasn't updated as expected by method HistoryLogger.WriteLog");

			TestDelegate method = () => HistoryLogger.MarkPendingMessagesAsCanceledByStartup();
			Assert.That<TestDelegate>(ref method, Throws.Nothing, "Method MarkPendingMessagesAsCanceledByStartup failed while expecting not");

			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(1), "Table MessageContext has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(1), "Table MessageRequest has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(2), "Table MessageStatus wasn't updated as expected by method MarkPendingMessagesAsCanceledByStartup");

		}

		[Test]
		public void TestMethodMarkPendingMessagesAsCanceledByStartup_BehaveAsExpected_WhenDatabaseContainTwoRecordsInProgressState()
		{
			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(0), "Expect that table MessageContext is empty");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(0), "Expect that table MessageRequest is empty");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(0), "Expect that table MessageStatus is empty");

			string messageText = "This is a scheduledMessage in progress";
			Guid requestId = Guid.NewGuid();
			string trainId = "TRAIN-1";
			DateTime startDate = DateTime.UtcNow;
			DateTime endDate = startDate + new TimeSpan(1, 0, 0, 0);

			ResultCodeEnum result = HistoryLogger.WriteLog(messageText, requestId, CommandType.SendScheduledMessage, trainId, MessageStatusType.InstantMessageDistributionProcessing, startDate, endDate);
			Expect(result, Is.EqualTo(ResultCodeEnum.RequestAccepted), "Failed to insert schedule message into the history log");

			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(1), "Table MessageContext wasn't updated as expected by method HistoryLogger.WriteLog");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(1), "Table MessageRequest wasn't updated as expected by method HistoryLogger.WriteLog");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(1), "Table MessageStatus wasn't updated as expected by method HistoryLogger.WriteLog");

			result = HistoryLogger.UpdateMessageStatus(trainId, requestId, MessageStatusType.InstantMessageDistributionSent, CommandType.SendScheduledMessage);
			Expect(result, Is.EqualTo(ResultCodeEnum.RequestAccepted), "Failed to update status of a request");

			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(1), "Table MessageContext has been updated by method HistoryLogger.UpdateMessageStatus while expecting not");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(1), "Table MessageRequest has been updated by method HistoryLogger.UpdateMessageStatus while expecting not");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(2), "Table MessageStatus wasn't updated as expected by method HistoryLogger.UpdateMessageStatus");


			requestId = Guid.NewGuid();
			trainId = "TRAIN-2";
			messageText = "This is a second scheduled message in progress";

			result = HistoryLogger.WriteLog(messageText, requestId, CommandType.SendScheduledMessage, trainId, MessageStatusType.InstantMessageDistributionProcessing, startDate, endDate);
			Expect(result, Is.EqualTo(ResultCodeEnum.RequestAccepted), "Failed to insert schedule message into the history log");

			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(2), "Table MessageContext wasn't updated as expected by method HistoryLogger.WriteLog");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(2), "Table MessageRequest wasn't updated as expected by method HistoryLogger.WriteLog");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(3), "Table MessageStatus wasn't updated as expected by method HistoryLogger.WriteLog");


			TestDelegate method = () => HistoryLogger.MarkPendingMessagesAsCanceledByStartup();
			Assert.That<TestDelegate>(ref method, Throws.Nothing, "Method MarkPendingMessagesAsCanceledByStartup failed while expecting not");

			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(2), "Table MessageContext has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(2), "Table MessageRequest has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(5), "Table MessageStatus wasn't updated as expected by method MarkPendingMessagesAsCanceledByStartup");
		}


		[Test]
		public void TestMethodMarkPendingMessagesAsCanceledByStartup_BehaveAsExpected_WhenDatabaseContainFourRecordsInProgressStateAndFourRecorsInFinalState()
		{
			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(0), "Expect that table MessageContext is empty");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(0), "Expect that table MessageRequest is empty");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(0), "Expect that table MessageStatus is empty");

			string trainId = "TRAIN-1";
			DateTime startDate = DateTime.UtcNow;
			DateTime endDate = startDate + new TimeSpan(1, 0, 0, 0);

			KeyValuePair<Guid, string>[] records = {
												new KeyValuePair<Guid, String>(Guid.NewGuid(), "This is a message"),
												new KeyValuePair<Guid, String>(Guid.NewGuid(), "This is a message 2"),
												new KeyValuePair<Guid, String>(Guid.NewGuid(), "This is a message 3"),
												new KeyValuePair<Guid, String>(Guid.NewGuid(), "This is a message 4"),
												new KeyValuePair<Guid, String>(Guid.NewGuid(), "This is a message 5"),
												new KeyValuePair<Guid, String>(Guid.NewGuid(), "This is a message 6"),
												new KeyValuePair<Guid, String>(Guid.NewGuid(), "This is a message 7"),
												new KeyValuePair<Guid, String>(Guid.NewGuid(), "This is a message 8"),
											};

			foreach (var recordInfo in records)
			{
				ResultCodeEnum result = HistoryLogger.WriteLog(recordInfo.Value, recordInfo.Key, CommandType.SendScheduledMessage, trainId, MessageStatusType.InstantMessageDistributionProcessing, startDate, endDate);
				Expect(result, Is.EqualTo(ResultCodeEnum.RequestAccepted), "Failed to insert schedule message into the history log");

			}

			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(records.Length), "Table MessageContext wasn't updated as expected by method HistoryLogger.WriteLog");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(records.Length), "Table MessageRequest wasn't updated as expected by method HistoryLogger.WriteLog");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(records.Length), "Table MessageStatus wasn't updated as expected by method HistoryLogger.WriteLog");

			KeyValuePair<Guid, MessageStatusType>[] extraStatus = {
																	  new KeyValuePair<Guid, MessageStatusType>(records[1].Key, MessageStatusType.InstantMessageDistributionSent),
																	  new KeyValuePair<Guid, MessageStatusType>(records[1].Key, MessageStatusType.InstantMessageDistributionTimedOut),
																	  new KeyValuePair<Guid, MessageStatusType>(records[5].Key, MessageStatusType.InstantMessageDistributionSent),
																	  new KeyValuePair<Guid, MessageStatusType>(records[5].Key, MessageStatusType.InstantMessageDistributionReceived),
																	  new KeyValuePair<Guid, MessageStatusType>(records[3].Key, MessageStatusType.InstantMessageDistributionWaitingToSend),
																	  new KeyValuePair<Guid, MessageStatusType>(records[2].Key, MessageStatusType.InstantMessageDistributionMessageLimitExceededError),
																	  new KeyValuePair<Guid, MessageStatusType>(records[7].Key, MessageStatusType.InstantMessageDistributionUnexpectedError)
																  };

			foreach (var status in extraStatus)
			{
				ResultCodeEnum result = HistoryLogger.UpdateMessageStatus(trainId, status.Key, status.Value, CommandType.SendScheduledMessage);
				Expect(result, Is.EqualTo(ResultCodeEnum.RequestAccepted), "Failed to update status of a request");

			}


			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(records.Length), "Table MessageContext has been updated by method HistoryLogger.UpdateMessageStatus while expecting not");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(records.Length), "Table MessageRequest has been updated by method HistoryLogger.UpdateMessageStatus while expecting not");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(records.Length + extraStatus.Length), "Table MessageStatus wasn't updated as expected by method HistoryLogger.UpdateMessageStatus");


			TestDelegate method = () => HistoryLogger.MarkPendingMessagesAsCanceledByStartup();
			Assert.That<TestDelegate>(ref method, Throws.Nothing, "Method MarkPendingMessagesAsCanceledByStartup failed while expecting not");

			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(records.Length), "Table MessageContext has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(records.Length), "Table MessageRequest has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(records.Length + extraStatus.Length + 4), "Table MessageStatus wasn't updated as expected by method MarkPendingMessagesAsCanceledByStartup");
		}

		[Test]
		public void TestMethodMarkPendingMessagesAsCanceledByStartup_BehaveAsExpected_ForAllStatus()
		{
			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(0), "Expect that table MessageContext is empty");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(0), "Expect that table MessageRequest is empty");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(0), "Expect that table MessageStatus is empty");

			int recordCount = 0;
			int finalStateCount = CountFinalStateStatus();
			string trainId = "TRAIN-1";
			DateTime startDate = DateTime.UtcNow;
			DateTime endDate = startDate + new TimeSpan(1, 0, 0, 0);

			

			foreach (MessageStatusType status in Enum.GetValues(typeof(MessageStatusType)))
			{
				recordCount++;
				Guid requestId = Guid.NewGuid();
				string message = "This is the scheduled message " + recordCount.ToString();
				ResultCodeEnum result = HistoryLogger.WriteLog(message, requestId, CommandType.SendScheduledMessage, trainId, status, startDate, endDate);
				Expect(result, Is.EqualTo(ResultCodeEnum.RequestAccepted), "Failed to insert schedule message into the history log for status '" + status.ToString() + "'");
			}


			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(recordCount), "Table MessageContext wasn't updated as expected by method HistoryLogger.WriteLog");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(recordCount), "Table MessageRequest wasn't updated as expected by method HistoryLogger.WriteLog");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(recordCount), "Table MessageStatus wasn't updated as expected by method HistoryLogger.WriteLog");


			TestDelegate method = () => HistoryLogger.MarkPendingMessagesAsCanceledByStartup();
			Assert.That<TestDelegate>(ref method, Throws.Nothing, "Method MarkPendingMessagesAsCanceledByStartup failed while expecting not");

			Expect(CountRecordInTable(MessageContextTableName), Is.EqualTo(recordCount), "Table MessageContext has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");
			Expect(CountRecordInTable(MessageRequestTableName), Is.EqualTo(recordCount), "Table MessageRequest has been updated by method MarkPendingMessagesAsCanceledByStartup while expecting not");
			Expect(CountRecordInTable(MessageStatusTableName), Is.EqualTo(recordCount + (recordCount - finalStateCount)), "Table MessageStatus wasn't updated as expected by method MarkPendingMessagesAsCanceledByStartup");
		}

		/// <summary>Count record in table.</summary>
		/// <param name="tableName">Name of the table.</param>
		/// <returns>The total number of record in table.</returns>
		private int CountRecordInTable(string tableName)
		{
			string query = "SELECT COUNT(*) FROM " + tableName;
			object result = SqlHelper.ExecuteScalar(HistoryLoggerConfiguration.SqlConnectionString, System.Data.CommandType.Text, query);
			return Convert.ToInt32(result, CultureInfo.InvariantCulture);
		}

		/// <summary>Count record in table StatusType that designate a final state.</summary>
		/// <returns>The total number of record in table StatusType that are considered in final state.</returns>
		private int CountFinalStateStatus()
		{
			string query = "SELECT COUNT(*) FROM StatusType where IsFinal <> 0";
			object result = SqlHelper.ExecuteScalar(HistoryLoggerConfiguration.SqlConnectionString, System.Data.CommandType.Text, query);
			return Convert.ToInt32(result, CultureInfo.InvariantCulture);
		}

		#endregion
    }
}