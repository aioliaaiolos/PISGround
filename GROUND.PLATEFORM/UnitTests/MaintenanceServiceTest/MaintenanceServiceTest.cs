using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PIS.Ground.Maintenance;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using PIS.Ground.Core.Data;

namespace UnitTests
{
    /// <summary>
    ///This is a test class for MaintenanceServiceTest and is intended
    ///to contain all MaintenanceServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MaintenanceServiceTest
    {
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

        /// <summary>
        ///A test for GetLogs - InvalidStartDate
        ///</summary>
        ///<remarks></remarks>
        [TestMethod()]
        public void GetLogsTestTC_Sw_MaintenanceServiceClasses_InvalidStartDate()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid sessionId = testSession.Login("admin", "admin");
            List<CommandType> commandList = new List<CommandType>();
            commandList.Add(CommandType.AllLogs);
            commandList.Add(CommandType.SendScheduledMessage);
            commandList.Add(CommandType.CancelScheduledMessage);
            UnitTests.MaintenanceService.MaintenanceServiceClient testMaintenance = new UnitTests.MaintenanceService.MaintenanceServiceClient();
            HistoryLogResponse testLogResponse = testMaintenance.GetLogs(sessionId, commandList.ToArray(), DateTime.Parse("2013-01-01"), DateTime.Parse("2014-01-01"),  1);
            Assert.AreEqual(ResultCodeEnum.InvalidStartDate,testLogResponse.ResultCode);
            
        }

        /// <summary>
        ///A test for GetLogs - InvalidEndDate
        ///</summary>
        ///<remarks></remarks>
        [TestMethod()]
        public void GetLogsTestTC_Sw_MaintenanceServiceClasses_InValidEndDate()
        {
            UnitTests.SessionService.SessionServiceClient testSession = new UnitTests.SessionService.SessionServiceClient();
            System.Guid sessionId = testSession.Login("admin", "admin");
            List<CommandType> commandList = new List<CommandType>();
            commandList.Add(CommandType.AllLogs);
            commandList.Add(CommandType.SendScheduledMessage);
            commandList.Add(CommandType.CancelScheduledMessage);
            UnitTests.MaintenanceService.MaintenanceServiceClient testMaintenance = new UnitTests.MaintenanceService.MaintenanceServiceClient();
            HistoryLogResponse testLogResponse = testMaintenance.GetLogs(sessionId, commandList.ToArray(), DateTime.Parse("2013-01-01"), DateTime.Parse("2012-01-01"), 1);
            Assert.AreEqual(ResultCodeEnum.InvalidEndDate,testLogResponse.ResultCode);
            
        }
    }
}
