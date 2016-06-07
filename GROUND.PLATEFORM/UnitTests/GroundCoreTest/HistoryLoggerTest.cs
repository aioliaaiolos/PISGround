using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.Utility;

namespace UnitTests.GroundCoreTest
{
    /// <summary>
    /// Summary description for HistoryLoggerTest
    /// </summary>
    [TestClass]
    public class HistoryLoggerTest
    {
        public HistoryLoggerTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestWriteLog()
        {
            ResultCodeEnum res=HistoryLogger.WriteLog("TestWritelog", Guid.NewGuid(), CommandType.SendScheduledMessage, "TRAIN1", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted,res);
        }

        [TestMethod]
        public void TestWriteLogGUID_Error()
        {
            ResultCodeEnum res = HistoryLogger.WriteLog("TestWritelog", Guid.Empty, CommandType.SendScheduledMessage, "TRAIN1", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            Assert.AreNotEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestWriteLogContext_Error()
        {
            ResultCodeEnum res = HistoryLogger.WriteLog("", Guid.NewGuid(), CommandType.SendScheduledMessage, "TRAIN1", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            Assert.AreNotEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestWriteLogTrain_Error()
        {
            ResultCodeEnum res = HistoryLogger.WriteLog("Test1", Guid.NewGuid(), CommandType.SendScheduledMessage, "", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            Assert.AreNotEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestWriteLogMuiltiTrain()
        {
            Guid id= Guid.NewGuid();
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN1", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN2", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN3", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            ResultCodeEnum res=HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN4", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestWriteCancelLogError()
        {
            Guid id = Guid.NewGuid();
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN1", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN2", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN3", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN4", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);

            ResultCodeEnum res= HistoryLogger.CancelLog(id, CommandType.CancelScheduledMessage, "TRAIN5", MessageStatusType.InstantMessageDistributionProcessing);
            Assert.AreNotEqual(ResultCodeEnum.RequestAccepted,res);
        }

        [TestMethod]
        public void TestCancelLogOnlyError()
        {
            Guid id = Guid.NewGuid();
            ResultCodeEnum res = HistoryLogger.CancelLog(id, CommandType.CancelScheduledMessage, "TRAIN5", MessageStatusType.InstantMessageDistributionProcessing);
            Assert.AreNotEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestCancelLogTrainError()
        {
            Guid id = Guid.NewGuid();
            ResultCodeEnum res = HistoryLogger.CancelLog(id, CommandType.CancelScheduledMessage, "", MessageStatusType.InstantMessageDistributionProcessing);
            Assert.AreNotEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestCancelLog()
        {
            Guid id = Guid.NewGuid();
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN1", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN2", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN3", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN4", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);

            ResultCodeEnum res = HistoryLogger.CancelLog(id, CommandType.CancelScheduledMessage, "TRAIN1", MessageStatusType.InstantMessageDistributionProcessing);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestCancelLogUpdateMessageStatusError()
        {
            Guid id = Guid.NewGuid();
            ResultCodeEnum res = HistoryLogger.UpdateMessageStatus("TRAIN1", id, MessageStatusType.InstantMessageDistributionReceived);
            Assert.AreNotEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestWriteLogMuiltiTrainUpateError()
        {
            Guid id = Guid.NewGuid();
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN1", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            
            ResultCodeEnum res = HistoryLogger.UpdateMessageStatus("TRAIN2", id, MessageStatusType.InstantMessageDistributionWaitingToSend);
            Assert.AreNotEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestWriteLogMuiltiTrainUpateEmptyError()
        {
            Guid id = Guid.NewGuid();
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN1", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);

            ResultCodeEnum res = HistoryLogger.UpdateMessageStatus("", id, MessageStatusType.InstantMessageDistributionWaitingToSend);
            Assert.AreNotEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestCancelLogUpdateMessageStatus()
        {
            Guid id = Guid.NewGuid();
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN1", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN2", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN3", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN4", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);

            ResultCodeEnum res = HistoryLogger.CancelLog(id, CommandType.CancelScheduledMessage, "TRAIN1", MessageStatusType.InstantMessageDistributionProcessing);
            res = HistoryLogger.UpdateMessageStatus("TRAIN1", id, MessageStatusType.InstantMessageDistributionWaitingToSend);
            res = HistoryLogger.UpdateMessageStatus("TRAIN1", id, MessageStatusType.InstantMessageDistributionSent);
            res = HistoryLogger.UpdateMessageStatus("TRAIN1", id, MessageStatusType.InstantMessageDistributionReceived);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestWriteLogMuiltiTrainUpate()
        {
            Guid id = Guid.NewGuid();
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN1", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN2", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN3", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN4", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);


            ResultCodeEnum res = HistoryLogger.UpdateMessageStatus("TRAIN1", id, MessageStatusType.InstantMessageDistributionWaitingToSend);
            res = HistoryLogger.UpdateMessageStatus( "TRAIN1",id, MessageStatusType.InstantMessageDistributionSent);
            res = HistoryLogger.UpdateMessageStatus("TRAIN1", id, MessageStatusType.InstantMessageDistributionReceived);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestWriteLogMuiltiTrainUpateMsg()
        {
            Guid id = Guid.NewGuid();
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN1", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN2", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN3", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
            HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN4", MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);


            ResultCodeEnum res = HistoryLogger.UpdateMessageStatus("TRAIN1", id, MessageStatusType.InstantMessageDistributionWaitingToSend);
            res = HistoryLogger.UpdateMessageStatus( "TRAIN1",id, MessageStatusType.InstantMessageDistributionSent);
            res = HistoryLogger.UpdateMessageStatus("TRAIN1", id, MessageStatusType.InstantMessageDistributionReceived);

            res = HistoryLogger.UpdateMessageStatus("TRAIN2", id, MessageStatusType.InstantMessageDistributionWaitingToSend);
            res = HistoryLogger.UpdateMessageStatus("TRAIN2", id, MessageStatusType.InstantMessageDistributionSent);
            res = HistoryLogger.UpdateMessageStatus("TRAIN2", id, MessageStatusType.InstantMessageDistributionReceived);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestGetLatestLogError()
        {
            List<CommandType> lst=new List<CommandType>();
            string str;
            ResultCodeEnum res = HistoryLogger.GetLatestLog(lst, out str);
            Assert.AreNotEqual(ResultCodeEnum.InvalidCommandType, res);
        }

        [TestMethod]
        public void TestGetLatestLog()
        {
            Guid id = Guid.NewGuid();
            List<CommandType> lst = new List<CommandType>();
            lst.Add(CommandType.SendScheduledMessage);
            string str;
            ResultCodeEnum res = HistoryLogger.GetLatestLog(lst, out str);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestGetLatestLogMultiCommand()
        {
            Guid id = Guid.NewGuid();
            List<CommandType> lst = new List<CommandType>();
            lst.Add(CommandType.SendScheduledMessage);
            lst.Add(CommandType.CancelScheduledMessage);
            string str;
            ResultCodeEnum res = HistoryLogger.GetLatestLog(lst, out str);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestGetOldestLogError()
        {
            List<CommandType> lst = new List<CommandType>();
            string str;
            ResultCodeEnum res = HistoryLogger.GetOldestLog(lst, out str);
            Assert.AreNotEqual( ResultCodeEnum.InvalidCommandType, res);
        }
        [TestMethod]
        public void TestGetOldestLog()
        {
            Guid id = Guid.NewGuid();
            List<CommandType> lst = new List<CommandType>();
            lst.Add(CommandType.SendScheduledMessage);
            string str;
            ResultCodeEnum res = HistoryLogger.GetOldestLog(lst, out str);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestGetOldestLogMultiCommand()
        {
            Guid id = Guid.NewGuid();
            List<CommandType> lst = new List<CommandType>();
            lst.Add(CommandType.SendScheduledMessage);
            lst.Add(CommandType.CancelScheduledMessage);
            string str;
            ResultCodeEnum res = HistoryLogger.GetOldestLog(lst, out str);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestGetAllLogError()
        {
            List<CommandType> lst = new List<CommandType>();
            string str;
            ResultCodeEnum res = HistoryLogger.GetAllLog(lst, DateTime.Now, DateTime.Now, 0, out str);
            Assert.AreNotEqual(ResultCodeEnum.InvalidCommandType, res);
        }

        [TestMethod]
        public void TestGetAllLogDateError()
        {
            List<CommandType> lst = new List<CommandType>();
            string str;
            ResultCodeEnum res = HistoryLogger.GetAllLog(lst, DateTime.UtcNow, DateTime.UtcNow, 0, out str);
            Assert.AreNotEqual(ResultCodeEnum.InvalidCommandType, res);
        }

        [TestMethod]
        public void TestGetAllLog()
        {
            List<CommandType> lst = new List<CommandType>();
            lst.Add(CommandType.SendScheduledMessage);
            string str;
            ResultCodeEnum res = HistoryLogger.GetAllLog(lst, DateTime.Now, DateTime.Now, 0, out str);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestGetAllLogMultiCommand()
        {
            Guid id = Guid.NewGuid();
            List<CommandType> lst = new List<CommandType>();
            lst.Add(CommandType.SendScheduledMessage);
            lst.Add(CommandType.CancelScheduledMessage);
            string str;
            ResultCodeEnum res = HistoryLogger.GetAllLog(lst, DateTime.Parse("2012-06-26"), DateTime.Parse("2012-06-28"), 0, out str);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestClearAllError()
        {
            Guid id = Guid.NewGuid();
            List<CommandType> lst = new List<CommandType>();
            ResultCodeEnum res = HistoryLogger.CleanLog(lst);
            Assert.AreNotEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestClearAll()
        {
            Guid id = Guid.NewGuid();
            List<CommandType> lst = new List<CommandType>();
            lst.Add(CommandType.CancelScheduledMessage);
            ResultCodeEnum res = HistoryLogger.CleanLog(lst);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestClearAllMultiCommand()
        {
            Guid id = Guid.NewGuid();
            List<CommandType> lst = new List<CommandType>();
            lst.Add(CommandType.CancelScheduledMessage);
            lst.Add(CommandType.SendScheduledMessage);
            ResultCodeEnum res = HistoryLogger.CleanLog(lst);
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, res);
        }

        [TestMethod]
        public void TestMaxCountTest()
        {
            for (int i = 0; i < 100; i++)
            {
                Guid id = Guid.NewGuid();
                HistoryLogger.WriteLog("TestWritelog", id, CommandType.SendScheduledMessage, "TRAIN"+i.ToString(), MessageStatusType.InstantMessageDistributionProcessing, DateTime.Now, DateTime.Now);
                
            }
            List<CommandType> lst = new List<CommandType>();
            lst.Add(CommandType.CancelScheduledMessage);
            lst.Add(CommandType.SendScheduledMessage);
            string str;
            ResultCodeEnum res = HistoryLogger.GetAllLog(lst, DateTime.Parse("2012-06-26"), DateTime.Parse("2012-06-29"), 0, out str);
            Assert.AreEqual(ResultCodeEnum.OutputLimitExceed, res);
        }
    }
}
