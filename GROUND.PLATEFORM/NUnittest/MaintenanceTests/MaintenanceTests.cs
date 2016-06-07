// <copyright file="IMaintenanceService.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2014.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PIS.Ground.Maintenance;
using PIS.Ground.Core.Data;
using Microsoft.XmlDiffPatch;
using System.Xml;

/// Test classes for Maintenance Service.
///
namespace MaintenanceTests
{
    /// <summary>Maintenance service test class.</summary>
    [TestFixture]
    public class MaintenanceServiceTests
    {
        #region Tests management

        /// <summary>Initializes a new instance of the MaintenanceServiceTests class.</summary>
        public MaintenanceServiceTests()
        {
            // Nothing to do
        }

        /// <summary>Setups called before each test to initialize variables.</summary>
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>Tear down called after each test to clean.</summary>
        [TearDown]
        public void TearDown()
        {
            // Do something after each tests
        }

        #endregion

        #region statusReportGenerator

        private static readonly Dictionary<string, TrainBaselineStatusData> trainsStatuses
            = new Dictionary<string, TrainBaselineStatusData>
        {
            { "1", new TrainBaselineStatusData()
                { 
                    TrainId = "1", TrainNumber = "One", OnlineStatus = true, ProgressStatus = BaselineProgressStatusEnum.UNKNOWN, 
                    CurrentBaselineVersion = "1.0.0.4", FutureBaselineVersion = "1.0.0.5",  PisOnBoardVersion = "5.14.0.1"
                } 
            },
            { "2", new TrainBaselineStatusData()
                { 
                    TrainId = "2", TrainNumber = "Two", OnlineStatus = true, ProgressStatus = BaselineProgressStatusEnum.UPDATED, 
                    CurrentBaselineVersion = "1.0.0.4", FutureBaselineVersion = "1.0.0.5", PisOnBoardVersion = "5.14.0.1"
                } 
            },
            { "3", new TrainBaselineStatusData()
                { 
                    TrainId = "3", TrainNumber = "Three", OnlineStatus = true, ProgressStatus = BaselineProgressStatusEnum.DEPLOYED, 
                    CurrentBaselineVersion = "1.0.0.4", FutureBaselineVersion = "1.0.0.5", PisOnBoardVersion = "5.14.0.1"
                } 
            },
            { "4", new TrainBaselineStatusData()
                { 
                    TrainId = "4", TrainNumber = "Four", OnlineStatus = true, ProgressStatus = BaselineProgressStatusEnum.TRANSFER_COMPLETED, 
                    CurrentBaselineVersion = "1.0.0.4", FutureBaselineVersion = "1.0.0.5", PisOnBoardVersion = "5.14.0.1"
                } 
            },
            { "5", new TrainBaselineStatusData()
                { 
                    TrainId = "5", TrainNumber = "Five", OnlineStatus = true, ProgressStatus = BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS, 
                    CurrentBaselineVersion = "1.0.0.4", FutureBaselineVersion = "1.0.0.5", PisOnBoardVersion = "5.14.0.1"
                } 
            },
            { "6", new TrainBaselineStatusData()
                { 
                    TrainId = "6", TrainNumber = "Six", OnlineStatus = true, ProgressStatus = BaselineProgressStatusEnum.TRANSFER_PAUSED, 
                    CurrentBaselineVersion = "1.0.0.4", FutureBaselineVersion = "1.0.0.5", PisOnBoardVersion = "5.14.0.1"
                } 
            },
            { "7", new TrainBaselineStatusData()
                { 
                    TrainId = "7", TrainNumber = "Seven", OnlineStatus = true, ProgressStatus = BaselineProgressStatusEnum.TRANSFER_PLANNED, 
                    CurrentBaselineVersion = "1.0.0.4", FutureBaselineVersion = "1.0.0.5", PisOnBoardVersion = "5.14.0.1"
                } 
            },
            { "8", new TrainBaselineStatusData()
                { 
                    TrainId = "8", TrainNumber = "Eight", OnlineStatus = false, ProgressStatus = BaselineProgressStatusEnum.UNKNOWN, 
                    CurrentBaselineVersion = "", FutureBaselineVersion = "", PisOnBoardVersion = "5.14.0.1"
                } 
            }
        };

        private static readonly string expectedResponse = 
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<FleetBaselineStatus>" +
                    "<Train TrainId=\"1\" TrainNumberAlpha=\"One\" OnlineStatus=\"ONLINE\" BaselineProgressStatus=\"Unknown\"" +
                        " CurrentBaselineVersion=\"1.0.0.4\" FutureBaselineVersion=\"1.0.0.5\" PISOnBoardVersion=\"5.14.0.1\" />" +
                    "<Train TrainId=\"2\" TrainNumberAlpha=\"Two\" OnlineStatus=\"ONLINE\" BaselineProgressStatus=\"Updated\"" +
                        " CurrentBaselineVersion=\"1.0.0.4\" FutureBaselineVersion=\"1.0.0.5\" PISOnBoardVersion=\"5.14.0.1\" />" +
                    "<Train TrainId=\"3\" TrainNumberAlpha=\"Three\" OnlineStatus=\"ONLINE\" BaselineProgressStatus=\"Deployed\"" +
                        " CurrentBaselineVersion=\"1.0.0.4\" FutureBaselineVersion=\"1.0.0.5\" PISOnBoardVersion=\"5.14.0.1\" />" +
                    "<Train TrainId=\"4\" TrainNumberAlpha=\"Four\" OnlineStatus=\"ONLINE\" BaselineProgressStatus=\"TransferCompleted\"" +
                        " CurrentBaselineVersion=\"1.0.0.4\" FutureBaselineVersion=\"1.0.0.5\" PISOnBoardVersion=\"5.14.0.1\" />" +
                    "<Train TrainId=\"5\" TrainNumberAlpha=\"Five\" OnlineStatus=\"ONLINE\" BaselineProgressStatus=\"TransferInProgress\"" +
                        " CurrentBaselineVersion=\"1.0.0.4\" FutureBaselineVersion=\"1.0.0.5\" PISOnBoardVersion=\"5.14.0.1\" />" +
                    "<Train TrainId=\"6\" TrainNumberAlpha=\"Six\" OnlineStatus=\"ONLINE\" BaselineProgressStatus=\"TransferPaused\"" +
                        " CurrentBaselineVersion=\"1.0.0.4\" FutureBaselineVersion=\"1.0.0.5\" PISOnBoardVersion=\"5.14.0.1\" />" +
                    "<Train TrainId=\"7\" TrainNumberAlpha=\"Seven\" OnlineStatus=\"ONLINE\" BaselineProgressStatus=\"TransferPlanned\"" +
                        " CurrentBaselineVersion=\"1.0.0.4\" FutureBaselineVersion=\"1.0.0.5\" PISOnBoardVersion=\"5.14.0.1\" />" +
                    "<Train TrainId=\"8\" TrainNumberAlpha=\"Eight\" OnlineStatus=\"OFFLINE\" BaselineProgressStatus=\"Unknown\"" +
                        " CurrentBaselineVersion=\"\" FutureBaselineVersion=\"\" PISOnBoardVersion=\"5.14.0.1\" />" +
                "</FleetBaselineStatus>";

        /// <summary>Searches for the first pending transfer tasks.</summary>
        [Test]
        public void statusReportGenerator()
        {
            string response;
            ResultCodeEnum result = StatusReportGenerator.GenerateStatusResponse(trainsStatuses, out response);

            Assert.AreEqual(ResultCodeEnum.RequestAccepted, result, "Result code of GenerateStatusResponse");

            XmlDocument sourceDoc = new XmlDocument();
            XmlDocument changedDoc = new XmlDocument();
            try
            {
                sourceDoc.LoadXml(expectedResponse);
                changedDoc.LoadXml(response);
            }
            catch (XmlException)
            {
                Assert.Fail("Response of GenerateStatusResponse");
            }

            XmlDiff xmldiff = new XmlDiff(XmlDiffOptions.IgnoreChildOrder |
                XmlDiffOptions.IgnoreNamespaces | XmlDiffOptions.IgnorePrefixes);

            Assert.AreEqual(true, xmldiff.Compare(sourceDoc, changedDoc), "Response of GenerateStatusResponse");
        }

        #endregion
    }
}
