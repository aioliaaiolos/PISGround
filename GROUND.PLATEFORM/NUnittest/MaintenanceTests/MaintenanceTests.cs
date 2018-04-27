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

        [Test]
        public void GetFleetBaselineStatus()
        {
            SessionService session = new SessionService();
            Guid id = session.Login("admin", "admin");



            // Test UpdateTrainBaselineStatus - Insert
            Guid id1 = Guid.NewGuid();
            ResultCodeEnum error;
            error = HistoryLogger.UpdateTrainBaselineStatus("1", id1, 1, "TRAIN1", false, BaselineProgressStatusEnum.DEPLOYED,
                                                            BaselineProgressStatusStateEnum.NONE,
                                                            "1.1.1.1", "1.1.1.2", "1.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);
            Guid id2 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("2", id2, 2, "TRAIN2", false, BaselineProgressStatusEnum.TRANSFER_COMPLETED,
                                                            BaselineProgressStatusStateEnum.NONE,
                                                            "2.1.1.1", "2.1.1.2", "2.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);
            Guid id3 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("3", id3, 3, "TRAIN3", false, BaselineProgressStatusEnum.TRANSFER_PAUSED,
                                                            BaselineProgressStatusStateEnum.NONE,
                                                            "3.1.1.1", "3.1.1.2", "3.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);
            Guid id4 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("4", id4, 4, "TRAIN4", false, BaselineProgressStatusEnum.UPDATED,
                                                            BaselineProgressStatusStateEnum.NONE,
                                                            "4.1.1.1", "4.1.1.2", "4.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);


            // DEPLOYMENT

            Guid id5 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("5", id5, 5, "TRAIN5", true, BaselineProgressStatusEnum.DEPLOYMENT,
                                                            BaselineProgressStatusStateEnum.PLANNED,
                                                            "5.1.1.1", "55.1.1.2", "55.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            
            Guid id6 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("6", id6, 6, "TRAIN6", true, BaselineProgressStatusEnum.DEPLOYMENT,
                                                            BaselineProgressStatusStateEnum.IN_PROGRESS,
                                                            "6.1.1.1", "66.1.1.2", "66.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id7 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("7", id7, 7, "TRAIN7", true, BaselineProgressStatusEnum.DEPLOYMENT,
                                                            BaselineProgressStatusStateEnum.COMPLETED,
                                                            "7.1.1.1", "77.1.1.2", "77.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id8 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("8", id8, 8, "TRAIN8", false, BaselineProgressStatusEnum.DEPLOYMENT,
                                                            BaselineProgressStatusStateEnum.TIMEOUT_EXPIRED,
                                                            "8.1.1.1", "8.1.1.2", "8.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id9 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("9", id9, 9, "TRAIN9", false, BaselineProgressStatusEnum.DEPLOYMENT,
                                                            BaselineProgressStatusStateEnum.PACKAGING_ERROR,
                                                            "9.1.1.1", "9.1.1.2", "9.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id10 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("10", id10, 10, "TRAIN10", false, BaselineProgressStatusEnum.DEPLOYMENT,
                                                            BaselineProgressStatusStateEnum.NO_DISK_SPACE,
                                                            "10.1.1.1", "10.1.1.2", "10.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id11 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("11", id11, 11, "TRAIN11", false, BaselineProgressStatusEnum.DEPLOYMENT,
                                                            BaselineProgressStatusStateEnum.NO_WRITE_PERMISSION,
                                                            "11.1.1.1", "11.1.1.2", "11.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id12 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("12", id12, 12, "TRAIN12", false, BaselineProgressStatusEnum.DEPLOYMENT,
                                                            BaselineProgressStatusStateEnum.COMPRESSION_ERROR,
                                                            "12.1.1.1", "12.1.1.2", "12.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id13 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("13", id13, 13, "TRAIN13", false, BaselineProgressStatusEnum.DEPLOYMENT,
                                                            BaselineProgressStatusStateEnum.OTHER_ERROR,
                                                            "13.1.1.1", "13.1.1.2", "13.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id14 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("14", id14, 14, "TRAIN14", false, BaselineProgressStatusEnum.DEPLOYMENT,
                                                            BaselineProgressStatusStateEnum.GROUND_BASELINE_MISMATCH,
                                                            "14.1.1.1", "14.1.1.2", "14.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);



            // BASELINE_TRANSFER_TO_T2G_FTP_REPOSITORY

            Guid id15 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("15", id15, 15, "TRAIN15", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_FTP_REPOSITORY,
                                                            BaselineProgressStatusStateEnum.IN_PROGRESS,
                                                            "15.1.1.1", "15.1.1.2", "15.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id16 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("16", id16, 16, "TRAIN16", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_FTP_REPOSITORY,
                                                            BaselineProgressStatusStateEnum.COMPLETED,
                                                            "16.1.1.1", "16.1.1.2", "16.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id17 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("17", id17, 17, "TRAIN17", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_FTP_REPOSITORY,
                                                            BaselineProgressStatusStateEnum.AUTHENTIFICATION_ERROR,
                                                            "17.1.1.1", "17.1.1.2", "17.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id18 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("18", id18, 18, "TRAIN18", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_FTP_REPOSITORY,
                                                            BaselineProgressStatusStateEnum.COMMUNICATION_ERROR,
                                                            "18.1.1.1", "18.1.1.2", "18.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id19 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("19", id19, 19, "TRAIN19", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_FTP_REPOSITORY,
                                                            BaselineProgressStatusStateEnum.TRANSFER_REFUSED_BY_SERVER,
                                                            "19.1.1.1", "19.1.1.2", "19.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id20 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("20", id20, 20, "TRAIN20", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_FTP_REPOSITORY,
                                                            BaselineProgressStatusStateEnum.FTP_ERROR,
                                                            "20.1.1.1", "20.1.1.2", "20.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id21 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("21", id21, 21, "TRAIN21", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_FTP_REPOSITORY,
                                                            BaselineProgressStatusStateEnum.TIMEOUT_EXPIRED,
                                                            "21.1.1.1", "21.1.1.2", "21.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id22 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("22", id22, 22, "TRAIN22", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_FTP_REPOSITORY,
                                                            BaselineProgressStatusStateEnum.OTHER_ERROR,
                                                            "22.1.1.1", "22.1.1.2", "22.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id23 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("23", id23, 23, "TRAIN23", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.TRANSFER_PLANNED,
                                                            "23.1.1.1", "23.1.1.2", "23.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id24 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("24", id24, 24, "TRAIN22", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.TRANSFER_CREATION_IN_PROGRESS,
                                                            "24.1.1.1", "24.1.1.2", "24.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id25 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("25", id22, 22, "TRAIN25", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.TRANSFER_WAITING_FOR_SCHEDULING,
                                                            "25.1.1.1", "25.1.1.2", "25.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id26 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("26", id26, 26, "TRAIN26", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.TRANSFER_WAITING_FOR_COMMUNICATION,
                                                            "26.1.1.1", "26.1.1.2", "26.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id27 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("27", id27, 27, "TRAIN22", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.TRANSFER_WAITING_FOR_LINK,
                                                            "27.1.1.1", "27.1.1.2", "27.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id28 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("28", id28, 28, "TRAIN28", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.TRANSFER_IN_PROGRESS,
                                                            "28.1.1.1", "28.1.1.2", "28.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id29 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("29", id29, 29, "TRAIN29", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.TRANSFER_WAITING_FOR_DISK_SPACE_ON_SERVER,
                                                            "29.1.1.1", "29.1.1.2", "29.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id30 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("30", id30, 30, "TRAIN30", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.TRANSFER_WAITING_FOR_DISK_SPACE_ON_TRAIN,
                                                            "30.1.1.1", "30.1.1.2", "30.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id31 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("31", id31, 31, "TRAIN22", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.TIMEOUT_EXPIRED,
                                                            "31.1.1.1", "31.1.1.2", "31.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id32 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("32", id32, 32, "TRAIN32", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.TRANSFER_START_ERROR,
                                                            "32.1.1.1", "32.1.1.2", "32.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id33 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("33", id33, 33, "TRAIN33", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.TRANSFER_TIME_OUT,
                                                            "33.1.1.1", "33.1.1.2", "33.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id34 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("34", id34, 34, "TRAIN34", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.TRANSFER_CANCELLED,
                                                            "34.1.1.1", "34.1.1.2", "34.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id35 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("35", id35, 35, "TRAIN35", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.DELIVERY_ERROR,
                                                            "35.1.1.1", "35.1.1.2", "35.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id36 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("36", id36, 36, "TRAIN36", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.T2G_ERROR,
                                                            "36.1.1.1", "36.1.1.2", "36.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id37 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("37", id37, 37, "TRAIN37", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.OTHER_ERROR,
                                                            "37.1.1.1", "37.1.1.2", "37.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id38 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("38", id38, 38, "TRAIN38", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.TRANSFER_COMPLETED,
                                                            "38.1.1.1", "38.1.1.2", "38.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id39 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("39", id39, 39, "TRAIN39", false, BaselineProgressStatusEnum.BASELINE_TRANSFER_TO_T2G_EMBEDDED,
                                                            BaselineProgressStatusStateEnum.OTHER_ERROR,
                                                            "39.1.1.1", "39.1.1.2", "39.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);


            Guid id40 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("40", id40, 40, "TRAIN40", false, BaselineProgressStatusEnum.BASELINE_ONBOARD_DISTRIBUTION,
                                                            BaselineProgressStatusStateEnum.IN_PROGRESS,
                                                            "40.1.1.1", "40.1.1.2", "40.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id41 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("41", id41, 41, "TRAIN41", false, BaselineProgressStatusEnum.BASELINE_ONBOARD_DISTRIBUTION,
                                                            BaselineProgressStatusStateEnum.DISTRIBUTION_WAITING_FOR_COMMUNICATION,
                                                            "41.1.1.1", "41.1.1.2", "41.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id42 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("42", id42, 42, "TRAIN42", false, BaselineProgressStatusEnum.BASELINE_ONBOARD_DISTRIBUTION,
                                                            BaselineProgressStatusStateEnum.COMPLETED,
                                                            "42.1.1.1", "42.1.1.2", "42.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id43 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("43", id43, 43, "TRAIN43", false, BaselineProgressStatusEnum.BASELINE_ONBOARD_DISTRIBUTION,
                                                            BaselineProgressStatusStateEnum.DISTRIBUTION_ERROR,
                                                            "43.1.1.1", "43.1.1.2", "43.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);


            Guid id44 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("44", id44, 44, "TRAIN44", false, BaselineProgressStatusEnum.BASELINE_ONBOARD_ACTIVATION,
                                                            BaselineProgressStatusStateEnum.PLANNED,
                                                            "44.1.1.1", "44.1.1.2", "44.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);


            Guid id45 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("45", id45, 45, "TRAIN45", false, BaselineProgressStatusEnum.BASELINE_ONBOARD_ACTIVATION,
                                                            BaselineProgressStatusStateEnum.ACTIVATION_WAITING_FOR_COMMUNICATION,
                                                            "45.1.1.1", "45.1.1.2", "45.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id46 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("46", id46, 46, "TRAIN46", false, BaselineProgressStatusEnum.BASELINE_ONBOARD_ACTIVATION,
                                                            BaselineProgressStatusStateEnum.ACTIVATED,
                                                            "46.1.1.1", "46.1.1.2", "46.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);


            Guid id47 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("47", id47, 47, "TRAIN47", false, BaselineProgressStatusEnum.BASELINE_ONBOARD_ACTIVATION,
                                                            BaselineProgressStatusStateEnum.ACTIVATION_ERROR,
                                                            "47.1.1.1", "47.1.1.2", "47.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);

            Guid id48 = Guid.NewGuid();
            error = HistoryLogger.UpdateTrainBaselineStatus("48", id48, 48, "TRAIN48", false, BaselineProgressStatusEnum.BASELINE_ONBOARD_ACTIVATION,
                                                            BaselineProgressStatusStateEnum.BASELINE_MISMATCH,
                                                            "48.1.1.1", "48.1.1.2", "48.1.1.3");
            Assert.AreEqual(ResultCodeEnum.RequestAccepted, error);


            MaintenanceTrainBaselineStatusListResponse r = _maintenanceService.GetFleetBaselineStatus(id, 1);

            session.Logout(id);
        }

        #endregion
    }
}
