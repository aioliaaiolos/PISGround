//---------------------------------------------------------------------------------------------------
// <copyright file="CompleteRealTimeServiceTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using Moq;
using NUnit.Framework;
using PIS.Ground.Core;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.RealTime;
using PIS.Ground.RealTimeTests;
using PIS.Ground.RealTimeTests.TrainServerSimulator;

namespace RealTimeTests
{
    /// <summary>Class specialized to execute tests on real-time web service by using only simulator for: remote data store, train service and T2G.</summary>
    [TestFixture]
    class CompleteRealTimeServiceTests
    {
#region Constants
        
        /// <summary>The Database for URBAN tests.</summary>
		public const string UrbanDB = "LMTURBAN.db";

#endregion

        #region Fields



        /// <summary>Full pathname of the db source folder.</summary>
        private static string _dbSourcePath = string.Empty;

        /// <summary>Full pathname of the execution folder.</summary>
        private static string _dbWorkingPath = string.Empty;

        /// <summary>The train 2ground client mock.</summary>
        private Mock<IT2GManager> _train2groundClientMock;

        /// <summary>The session manager mock.</summary>
        private Mock<ISessionManagerExtended> _sessionManagerMock;

        /// <summary>The notification sender mock.</summary>
        private Mock<INotificationSender> _notificationSenderMock;

        /// <summary>The remote data store factory.</summary>
        private Mock<IRemoteDataStoreFactory> _remoteDataStoreFactory;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CompleteRealTimeServiceTests"/> class.
        /// </summary>
        public CompleteRealTimeServiceTests()
        {
            /* No logic body */
        }

        #region Test Initialization/Finalization

        /// <summary>Method called once to perform initialization that apply to all test in this fixture.</summary>
        [TestFixtureSetUp]
        public static void Init()
        {
            CompleteRealTimeServiceTests._dbSourcePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty), "..\\..\\..\\GroundCoreTests\\PackageAccess");
            CompleteRealTimeServiceTests._dbWorkingPath = Path.Combine(Path.GetTempPath(), "PISGTestsTemp");

            // Avoid runtime errors
            if (Directory.Exists(_dbWorkingPath))
            {
                Cleanup();
            }

            if (!Directory.Exists(_dbWorkingPath))
            {
                Directory.CreateDirectory(_dbWorkingPath);
            }


            string from = Path.Combine(_dbSourcePath, UrbanDB);
            string to = Path.Combine(_dbWorkingPath, UrbanDB);
            File.Copy(from, to, true);
            FileAttributes currentAttribute = File.GetAttributes(to);
            FileAttributes newAttributes = currentAttribute | FileAttributes.ReadOnly;
            if (newAttributes != currentAttribute)
                File.SetAttributes(to, newAttributes);
        }

        /// <summary>Method called once to perform cleanup when all test of this fixture has been executed.</summary>
        [TestFixtureTearDown]
        public static void Cleanup()
        {
            if (Directory.Exists(_dbWorkingPath))
            {
                string filepath = Path.Combine(_dbWorkingPath, UrbanDB);
                if (File.Exists(filepath))
                {
                    FileAttributes currentAttributes = File.GetAttributes(filepath);
                    FileAttributes newAttributes = currentAttributes & ~FileAttributes.ReadOnly;
                    if (currentAttributes != newAttributes)
                    {
                        File.SetAttributes(filepath, newAttributes);
                    }

                    File.Delete(filepath);
                }

                Directory.Delete(_dbWorkingPath, true);
            }
        }

        /// <summary>Setups the execution of one test. Called before the execution of every test.</summary>
        [SetUp]
        public void Setup()
        {
            _sessionManagerMock = new Mock<ISessionManagerExtended>();
            _train2groundClientMock = new Mock<IT2GManager>();
            _notificationSenderMock = new Mock<INotificationSender>();
            _remoteDataStoreFactory = new Mock<IRemoteDataStoreFactory>();

        }

        /// <summary>Method called after the execution of every test</summary>
        [TearDown]
        public void TearDown()
        {
            _sessionManagerMock = null;
            _train2groundClientMock = null;
            _notificationSenderMock = null;
            _remoteDataStoreFactory = null;
        }

        #endregion

        #region Common test functions

        /// <summary>Create an available element structure.</summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="isOnline">true if the train is online.</param>
        /// <param name="baselineVersion">The baseline version.</param>
        /// <param name="missionState">The state of the mission.</param>
        /// <param name="missionCode">The mission code</param>
        /// <returns>The new available element.</returns>
        private AvailableElementData CreateAvailableElement(string elementName, bool isOnline, string baselineVersion, MissionStateEnum missionState, string missionCode)
        {
            AvailableElementData trainInfo = new AvailableElementData();
            trainInfo.ElementNumber = elementName;
            trainInfo.OnlineStatus = isOnline;
            trainInfo.MissionState = missionState;
            trainInfo.MissionOperatorCode = "";
            trainInfo.MissionCommercialNumber = "";
            trainInfo.LmtPackageVersion = baselineVersion;
            trainInfo.PisBaselineData = CreateBaselineData(baselineVersion);
            trainInfo.PisBasicPackageVersion = string.Empty;
            return trainInfo;

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

        private void AssertAreEqual(RealTimeStationPlatformType expected, RealTimeStationPlatformType actual, DateTime expectedSmallestUpdateDate, DateTime expectedHighestUpdateDate, string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                Assert.AreEqual(expected.ExitSide, actual.ExitSide, "{0}. Property 'ExitSide' of platform object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.ExitSideCode, actual.ExitSideCode, "{0}. Property 'ExitSideCode' of platform object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.IssueDescription, actual.IssueDescription, "{0}. Property 'IssueDescription' of platform object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.IssueDescriptionCode, actual.IssueDescriptionCode, "{0}. Property 'IssueDescriptionCode' of platform object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.Platform, actual.Platform, "{0}. Property 'Platform' of platform object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.Track, actual.Track, "{0}. Property 'Track' of platform object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.TrackCode, actual.TrackCode, "{0}. Property 'TrackCode' of platform object is not set to expected value.", errorMessage);
                Assert.That(actual.UpdateDate, Is.InRange(expectedSmallestUpdateDate, expectedHighestUpdateDate), "{0}. Property 'UpdateDate' of platform object is not set to expected value.", errorMessage);
            }
        }

        private void AssertAreEqual(RealTimeStationConnectionType expected, RealTimeStationConnectionType actual, DateTime expectedSmallestUpdateDate, DateTime expectedHighestUpdateDate, string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                Assert.AreEqual(expected.CommercialNumber, actual.CommercialNumber, "{0}. Property 'CommercialNumber' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.ConnectionDelay, actual.ConnectionDelay, "{0}. Property 'ConnectionDelay' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.ConnectionPlatform, actual.ConnectionPlatform, "{0}. Property 'ConnectionPlatform' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.DepartureFrequency, actual.DepartureFrequency, "{0}. Property 'DepartureFrequency' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.DepartureTime, actual.DepartureTime, "{0}. Property 'DepartureTime' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.DestinationName, actual.DestinationName, "{0}. Property 'DestinationName' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.Line, actual.Line, "{0}. Property 'Line' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.LineCode, actual.LineCode, "{0}. Property 'LineCode' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.ModelType, actual.ModelType, "{0}. Property 'ModelType' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.ModelTypeCode, actual.ModelTypeCode, "{0}. Property 'ModelTypeCode' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.NextDepartureTime, actual.NextDepartureTime, "{0}. Property 'NextDepartureTime' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.Operator, actual.Operator, "{0}. Property 'Operator' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.OperatorCode, actual.OperatorCode, "{0}. Property 'OperatorCode' of station connection object is not set to expected value.", errorMessage);
                Assert.That(actual.UpdateDate, Is.InRange(expectedSmallestUpdateDate, expectedHighestUpdateDate), "{0}. Property 'UpdateDate' of station connection object is not set to expected value.", errorMessage);
            }
        }

        private void AssertAreEqual(RealTimeStationConnectionType expected, PIS.Train.RealTime.ConnectionType actual, uint expectedSmallestAge, uint expectedHighestAge, string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                Assert.AreEqual(expected.CommercialNumber, actual.CommercialNumber, "{0}. Property 'CommercialNumber' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.ConnectionDelay, actual.Delay, "{0}. Property 'Delay' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.ConnectionPlatform, actual.Platform, "{0}. Property 'Platform' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.DepartureFrequency, actual.DepartureFrequency, "{0}. Property 'DepartureFrequency' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.DepartureTime, actual.DepartureTime, "{0}. Property 'DepartureTime' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.DestinationName, actual.DestinationLabel, "{0}. Property 'DestinationLabel' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.Line, actual.Line, "{0}. Property 'Line' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.LineCode, actual.LineCode, "{0}. Property 'LineCode' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.ModelType, actual.ModelType, "{0}. Property 'ModelType' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.ModelTypeCode, actual.ModelTypeCode, "{0}. Property 'ModelTypeCode' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.NextDepartureTime, actual.NextDepartureTime, "{0}. Property 'NextDepartureTime' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.Operator, actual.Operator, "{0}. Property 'Operator' of station connection object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.OperatorCode, actual.OperatorCode, "{0}. Property 'OperatorCode' of station connection object is not set to expected value.", errorMessage);
                Assert.That(actual.Age, Is.InRange(expectedSmallestAge, expectedHighestAge), "{0}. Property 'Age' of station connection object is not set to expected value.", errorMessage);
            }

        }

        private void AssertAreEqual(RealTimeStationPlatformType expected, PIS.Train.RealTime.PlatformType actual, uint expectedSmallestAge, uint expectedHighestAge, string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                Assert.AreEqual(expected.ExitSide, actual.ExitSide, "{0}. Property 'ExitSide' of platform object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.ExitSideCode, actual.ExitSideCode, "{0}. Property 'ExitSideCode' of platform object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.IssueDescription, actual.PlatformIssueDescription, "{0}. Property 'PlatformIssueDescription' of platform object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.IssueDescriptionCode, actual.PlatformIssueDescriptionCode, "{0}. Property 'PlatformIssueDescriptionCode' of platform object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.Platform, actual.Platform, "{0}. Property 'Platform' of platform object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.Track, actual.Track, "{0}. Property 'Track' of platform object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.TrackCode, actual.TrackCode, "{0}. Property 'TrackCode' of platform object is not set to expected value.", errorMessage);
                Assert.That(actual.Age, Is.InRange(expectedSmallestAge, expectedHighestAge), "{0}. Property 'Age' of platform object is not set to expected value.", errorMessage);
            }
        }

        private void AssertAreEqual(RealTimeDelayType expected, RealTimeDelayType actual, DateTime expectedSmallestUpdateDate, DateTime expectedHighestUpdateDate, string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                Assert.AreEqual(expected.Delay, actual.Delay, "{0}. Property 'Delay' of delay object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.DelayReason, actual.DelayReason, "{0}. Property 'DelayReason' of delay object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.DelayReasonCode, actual.DelayReasonCode, "{0}. Property 'DelayReasonCode' of delay object is not set to expected value.", errorMessage);
                Assert.That(actual.UpdateDate, Is.InRange(expectedSmallestUpdateDate, expectedHighestUpdateDate), "{0}. Property 'UpdateDate' of delay object is not set to expected value.", errorMessage);
            }
        }

        private void AssertAreEqual(RealTimeDelayType expected, PIS.Train.RealTime.DelayType actual, uint expectedSmallestAge, uint expectedHighestAge, string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                Assert.AreEqual(expected.Delay, actual.DelayValue, "{0}. Property 'DelayValue' of delay object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.DelayReason, actual.DelayReason, "{0}. Property 'DelayReason' of delay object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.DelayReasonCode, actual.DelayReasonCode, "{0}. Property 'DelayReasonCode' of delay object is not set to expected value.", errorMessage);
                Assert.That(actual.Age, Is.InRange(expectedSmallestAge, expectedHighestAge), "{0}. Property 'Age' of delay object is not set to expected value.", errorMessage);
            }
        }

        private void AssertAreEqual(RealTimeWeatherType expected, RealTimeWeatherType actual, DateTime expectedSmallestUpdateDate, DateTime expectedHighestUpdateDate, string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                Assert.AreEqual(expected.Humidity, actual.Humidity, "{0}. Property 'Humidity' of weather object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.TemperatureInCentigrade, actual.TemperatureInCentigrade, "{0}. Property 'TemperatureInCentigrade' of weather object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.TemperatureInFahrenheit, actual.TemperatureInFahrenheit, "{0}. Property 'TemperatureInFahrenheit' of weather object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.WeatherCondition, actual.WeatherCondition, "{0}. Property 'WeatherCondition' of weather object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.WeatherConditionCode, actual.WeatherConditionCode, "{0}. Property 'WeatherConditionCode' of weather object is not set to expected value.", errorMessage);
                Assert.That(actual.UpdateDate, Is.InRange(expectedSmallestUpdateDate, expectedHighestUpdateDate), "{0}. Property 'UpdateDate' of weather object is not set to expected value.", errorMessage);
            }
        }

        private void AssertAreEqual(RealTimeWeatherType expected, PIS.Train.RealTime.WeatherType actual, uint expectedSmallestAge, uint expectedHighestAge, string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                Assert.AreEqual(expected.Humidity, actual.Humidity, "{0}. Property 'Humidity' of weather object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.TemperatureInCentigrade, actual.TemperatureInCentigrade, "{0}. Property 'TemperatureInCentigrade' of weather object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.TemperatureInFahrenheit, actual.TemperatureInFahrenheit, "{0}. Property 'TemperatureInFahrenheit' of weather object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.WeatherCondition, actual.WeatherCondition, "{0}. Property 'WeatherCondition' of weather object is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.WeatherConditionCode, actual.WeatherConditionCode, "{0}. Property 'WeatherConditionCode' of weather object is not set to expected value.", errorMessage);
                Assert.That(actual.Age, Is.InRange(expectedSmallestAge, expectedHighestAge), "{0}. Property 'Age' of weather object is not set to expected value.", errorMessage);
            }
        }

        private void AssertAreEqual(RealTimeConnectionResultType expected, RealTimeConnectionResultType actual, string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                Assert.AreEqual(expected.ConnectionResult, actual.ConnectionResult, "{0}. Property 'ConnectionResult' of connection result is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.CommercialNumber, actual.CommercialNumber, "{0}. Property 'CommercialNumber' of connection result is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.Operator, actual.Operator, "{0}. Property 'Operator' of connection result is not set to expected value.", errorMessage);
            }
        }

        private void AssertAreEqual(RealTimeStationStatusType expected, RealTimeStationStatusType actual, DateTime expectedSmallestUpdateDate, DateTime expectedHighestUpdateDate, string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                Assert.AreEqual(expected.StationResult, actual.StationResult, "{0}. Property 'StationResult' of station status type is not set to expected value.", errorMessage);
                string internalErrorMessage = string.Format(CultureInfo.InvariantCulture, "{0}. Property 'StationDelay' of station status type is not set to expected value.", errorMessage);
                AssertAreEqual(expected.StationDelay, actual.StationDelay, expectedSmallestUpdateDate, expectedHighestUpdateDate, internalErrorMessage);
                internalErrorMessage = string.Format(CultureInfo.InvariantCulture, "{0}. Property 'StationPlatform' of station status type is not set to expected value.", errorMessage);
                AssertAreEqual(expected.StationPlatform, actual.StationPlatform, expectedSmallestUpdateDate, expectedHighestUpdateDate, internalErrorMessage);
                internalErrorMessage = string.Format(CultureInfo.InvariantCulture, "{0}. Property 'StationWeather' of station status type is not set to expected value.", errorMessage);
                AssertAreEqual(expected.StationWeather, actual.StationWeather, expectedSmallestUpdateDate, expectedHighestUpdateDate, internalErrorMessage);
                internalErrorMessage = string.Format(CultureInfo.InvariantCulture, "{0}. Property 'StationConnectionList' of station status type is not set to expected value.", errorMessage);

                Action<RealTimeStationConnectionType, RealTimeStationConnectionType, string> comparer = (RealTimeStationConnectionType expectedComparer, RealTimeStationConnectionType actualComparer, string errorMessageComparer) => { AssertAreEqual(expectedComparer, actualComparer, expectedSmallestUpdateDate, expectedHighestUpdateDate, errorMessageComparer); };
                AssertListEqual(expected.StationConnectionList, actual.StationConnectionList, comparer, internalErrorMessage);
            }
        }

        private void AssertAreEqual(RealTimeStationDataType expected, PIS.Train.RealTime.StationDataStructureType actual, uint expectedSmallestAge, uint expectedHighestAge, string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                PIS.Train.RealTime.ActionTypeEnum expectedAction = (actual.StationConnectionList != null) ? PIS.Train.RealTime.ActionTypeEnum.Set : PIS.Train.RealTime.ActionTypeEnum.Delete;
                Assert.AreEqual(expectedAction, actual.StationConnectionListAction, "{0}. Property 'StationConnectionListAction' is not set to expected value", errorMessage);
                expectedAction = (actual.StationDelay != null) ? PIS.Train.RealTime.ActionTypeEnum.Set : PIS.Train.RealTime.ActionTypeEnum.Delete;
                Assert.AreEqual(expectedAction, actual.StationDelayAction, "{0}. Property 'StationDelayAction' is not set to expected value", errorMessage);
                expectedAction = (actual.StationPlatform != null) ? PIS.Train.RealTime.ActionTypeEnum.Set : PIS.Train.RealTime.ActionTypeEnum.Delete;
                Assert.AreEqual(expectedAction, actual.StationPlatformAction, "{0}. Property 'StationPlatformAction' is not set to expected value", errorMessage);
                expectedAction = (actual.StationWeather != null) ? PIS.Train.RealTime.ActionTypeEnum.Set : PIS.Train.RealTime.ActionTypeEnum.Delete;
                Assert.AreEqual(expectedAction, actual.StationWeatherAction, "{0}. Property 'StationWeatherAction' is not set to expected value", errorMessage);

                string internalErrorMessage = string.Format(CultureInfo.InvariantCulture, "{0}. Property 'StationDelay' is not set to expected value", errorMessage);
                AssertAreEqual(expected.StationDelay, actual.StationDelay, expectedSmallestAge, expectedHighestAge, internalErrorMessage);
                internalErrorMessage = string.Format(CultureInfo.InvariantCulture, "{0}. Property 'StationPlatform' is not set to expected value", errorMessage);
                AssertAreEqual(expected.StationPlatform, actual.StationPlatform, expectedSmallestAge, expectedHighestAge, internalErrorMessage);
                internalErrorMessage = string.Format(CultureInfo.InvariantCulture, "{0}. Property 'StationWeather' is not set to expected value", errorMessage);
                AssertAreEqual(expected.StationWeather, actual.StationWeather, expectedSmallestAge, expectedHighestAge, internalErrorMessage);
                internalErrorMessage = string.Format(CultureInfo.InvariantCulture, "{0}. Property 'StationWeather' is not set to expected value", errorMessage);

                Action<RealTimeStationConnectionType, PIS.Train.RealTime.ConnectionType, string> itemComparer = (x, y, z) => AssertAreEqual(x, y, expectedSmallestAge, expectedHighestAge, z);
                internalErrorMessage = string.Format(CultureInfo.InvariantCulture, "{0}. Property 'StationConnectionList' is not set to expected value", errorMessage);
                AssertListEqual(expected.StationConnectionList, actual.StationConnectionList, itemComparer, internalErrorMessage);
            }
        }

        private void AssertAreEqual(RealTimeStationInformationType expected, PIS.Train.RealTime.StationDataType actual, uint expectedSmallestAge, uint expectedHighestAge, string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                Assert.AreEqual(expected.StationCode, actual.StationCode, "{0}. Property 'StationCode' of station request is not set to expected value.", errorMessage);
                string internalErrorMessage = string.Format(CultureInfo.InvariantCulture, "{0}. Property 'StationDataStructure' of station request is not set to expected value.", errorMessage);
                AssertAreEqual(expected.StationData, actual.StationDataStructure, expectedSmallestAge, expectedHighestAge, internalErrorMessage);
            }
        }

        private void AssertAreEqual(RealTimeStationResultType expected, RealTimeStationResultType actual, string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                Assert.AreEqual(expected.StationID, actual.StationID, "{0}. Property 'StationId' of station result is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.WeatherResult, actual.WeatherResult, "{0}. Property 'WeatherResult' of station result is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.DelayResult, actual.DelayResult, "{0}. Property 'DelayResult' of station result is not set to expected value.", errorMessage);
                Assert.AreEqual(expected.PlatformResult, actual.PlatformResult, "{0}. Property 'PlatformResult' of station result is not set to expected value.", errorMessage);
                
                string internalErrorMessage =  string.Format(CultureInfo.InvariantCulture, "{0}. Property 'ConnectionsResultList' of station result is not set to expected value", errorMessage);

                var listComparer = new Action<RealTimeConnectionResultType, RealTimeConnectionResultType, string>(AssertAreEqual);
                AssertListEqual(expected.ConnectionsResultList, actual.ConnectionsResultList, listComparer, internalErrorMessage);
            }
        }

        private void AssertListEqual<T, Y>(List<T> expected, List<Y> actual, Action<T, Y, string> itemComparer,  string errorMessage)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, errorMessage);
            }
            else if (actual == null)
            {
                Assert.IsNotNull(actual, errorMessage);
            }
            else
            {
                Assert.AreEqual(expected.Count, actual.Count, "{0}. Number of items in list mismatch", errorMessage);

                for (int i = 0; i < expected.Count; ++i)
                {
                    string errorMessageForItem = string.Format(CultureInfo.InvariantCulture, "{0}. Item {1} in list mismatch", errorMessage, i);
                    itemComparer(expected[i], actual[i], errorMessageForItem);
                }
            }
        }


        #endregion

        #region Test Cases

        #region SetMissionRealTimeInformation / GetMissionRealTimeInformation

        [Test, Category("RealTime")]
        public void SetMissionRealTimeInformation_CommunicateWithTrain_Nominal()
        {
            Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();

            // Step 1 : Send delay and weather information
            // Step 2 : Send delay only
            // Step 3 : Send weather only

            // Initialization of the session manager
            SessionData sessionData = new SessionData();
            Guid sessionId = Guid.NewGuid();
            Guid sessionIdGet = Guid.NewGuid();
            Guid generatedRequestGuid = Guid.NewGuid();
            Guid generatedRequestGuidForGet = Guid.NewGuid();
            _sessionManagerMock.Setup(x => x.GetSessionDetails(sessionId, out sessionData)).Returns(string.Empty);
            _sessionManagerMock.Setup(x => x.GetSessionDetails(sessionIdGet, out sessionData)).Returns(string.Empty);
            _sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
            _sessionManagerMock.Setup(x => x.IsSessionValid(sessionIdGet)).Returns(true);
            _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestGuid)).Returns(string.Empty).Verifiable("Method SetMissionRealTimeInformation does not invoke GenerateRequestGuid as expected");
            _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionIdGet, out generatedRequestGuidForGet)).Returns(string.Empty).Verifiable("Method GetMissionRealTimeInformation does not invoke GenerateRequestGuid as expected");

            // Initialization of the T2G manager
            bool isTrain1Online = true;
            string missionCode = "1";
            AvailableElementData train1 = CreateAvailableElement("TRAIN-1", isTrain1Online, "5.12.14.0", MissionStateEnum.MI, missionCode);
            ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>
            {
                train1
            };

            ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_RealTimeServer, "RealTimeDataServer", 0, 0, isTrain1Online, "127.0.0.1" /* ip */, "", "", 8201 /* port */);
            Uri trainServiceAddress = new Uri("http://127.0.0.1:8201/");

            _train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataListByMissionCode(missionCode, out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_RealTimeServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(
                x => x.SubscribeToElementChangeNotification(
                    RequestProcessor.SubscriberId,
                    It.IsAny<EventHandler<ElementEventArgs>>())).Verifiable("RequestProcessor does not subscribes to element change notification as expected.");
            _train2groundClientMock.Setup(
                x => x.UnsubscribeFromElementChangeNotification(
                    RequestProcessor.SubscriberId));
            // Initialization of remote data store factory.
            _remoteDataStoreFactory.Setup(z => z.GetRemoteDataStoreInstance()).Returns(new PIS.Ground.RealTimeTests.RemoteDataStoreSimulator.RemoteDataStore(_dbWorkingPath));

            RTPISDataStore rtpisDataStore = new RTPISDataStore();

            RequestProcessor requestProcessor = new RequestProcessor(_train2groundClientMock.Object, rtpisDataStore);

            using (RealTimeTrainService trainService = new RealTimeTrainService(train1ServiceInfo.ServiceIPAddress, train1ServiceInfo.ServicePortNumber))
            using (RealTimeServiceTested realTimeService = new RealTimeServiceTested(
                    _train2groundClientMock.Object,
                    _sessionManagerMock.Object,
                    _notificationSenderMock.Object,
                    requestProcessor,
                    _remoteDataStoreFactory.Object,
                    rtpisDataStore))
            {
                Assert.DoesNotThrow(() => { trainService.Start(); }, "Unable to start real-time service for the train");
                realTimeService.PlateformType = CommonConfiguration.PlatformTypeEnum.URBAN;
                IRealTimeService webService = (IRealTimeService)realTimeService;

                //---------------------------------------------
                // STEP 1: Update delay and weather information
                //---------------------------------------------

                DateTime updateDateSet = DateTime.Now.Subtract(TimeSpan.FromMinutes(2));
                RealTimeDelayType delay = new RealTimeDelayType
                {
                    Delay = 5,
                    DelayReason = "Weather is too hot",
                    DelayReasonCode = 0,
                    UpdateDate = updateDateSet
                };
                RealTimeWeatherType weather = new RealTimeWeatherType
                {
                    Humidity = "50%",
                    TemperatureInCentigrade = "45",
                    TemperatureInFahrenheit = "113",
                    UpdateDate = updateDateSet,
                    WeatherCondition = "Sunny",
                    WeatherConditionCode = 0
                };

                RealTimeInformationType realTimeInfo = new RealTimeInformationType
                {
                    MissionDelay = delay,
                    MissionWeather = weather
                };

                DateTime dateTimeBeforeSet = DateTime.Now;
                DateTime dateTimeAfterSet;
                {
                    RealTimeSetMissionRealTimeInformationResult setResult = webService.SetMissionRealTimeInformation(sessionId, missionCode, delay, weather);
                    dateTimeAfterSet = DateTime.Now;

                    Assert.IsNotNull(setResult, "SetMissionRealTimeInformation does not return a valid object as expected");
                    Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, setResult.ResultCode, "SetMissionRealTimeInformation does not return the expected result code");
                    Assert.AreEqual(generatedRequestGuid, setResult.RequestId, "SetMissionRealTimeInformation does not return a valid request id");

                    var dataStoreWeatherObject = ((IRTPISDataStore)rtpisDataStore).GetMissionRealTimeInformation(missionCode).MissionWeather;
                    var dataStoreDelayObject = ((IRTPISDataStore)rtpisDataStore).GetMissionRealTimeInformation(missionCode).MissionDelay;
                    Assert.AreNotSame(weather, dataStoreWeatherObject, "SetMissionRealTimeInformation does not create a copy of the weather object");
                    Assert.AreNotSame(delay, dataStoreDelayObject, "SetMissionRealTimeInformation does not create a copy of the delay object");
                    AssertAreEqual(delay, dataStoreDelayObject, dateTimeBeforeSet, dateTimeAfterSet, "SetMissionRealTimeInformation does not update properly delay information in RTPIS data store");
                    AssertAreEqual(weather, dataStoreWeatherObject, dateTimeBeforeSet, dateTimeAfterSet, "SetMissionRealTimeInformation does not update properly weather information in RTPIS data store");
                }

                RealTimeGetMissionRealTimeInformationResult getResult = webService.GetMissionRealTimeInformation(sessionIdGet, missionCode);

                Assert.IsNotNull(getResult, "GetMissionRealTimeInformation does not return a valid object");
                Assert.AreEqual(missionCode, getResult.MissionCode, "GetMissionRealTimeInformation does not return the proper mission code");
                Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, getResult.ResultCode, "GetMissionRealTimeInformation does not return the proper error code");
                Assert.AreEqual(generatedRequestGuidForGet, getResult.RequestId, "GetMissionRealTimeInformation does not return the expected request id");
                Assert.IsNotNull(getResult.InformationStructure, "GetMissionRealTimeInformation does not set the InformationStructure property");
                Assert.IsNotNull(getResult.InformationStructure.MissionDelay, "GetMissionRealTimeInformation does not return the delay information");
                Assert.IsNotNull(getResult.InformationStructure.MissionWeather, "GetMissionRealTimeInformation does not return the weather information");
                Assert.AreNotSame(weather, getResult.InformationStructure.MissionWeather, "GetMissionRealTimeInfortmation does not return a copy of the weather object");
                Assert.AreNotSame(delay, getResult.InformationStructure.MissionDelay, "GetMissionRealTimeInfortmation does not return a copy of the delay object");

                Assert.AreNotSame(((IRTPISDataStore)rtpisDataStore).GetMissionRealTimeInformation(missionCode).MissionWeather, getResult.InformationStructure.MissionWeather, "GetMissionRealTimeInfortmation does not return a copy of the weather object");
                Assert.AreNotSame(((IRTPISDataStore)rtpisDataStore).GetMissionRealTimeInformation(missionCode).MissionDelay, getResult.InformationStructure.MissionDelay, "GetMissionRealTimeInfortmation does not return a copy of the delay object");

                AssertAreEqual(delay, getResult.InformationStructure.MissionDelay, dateTimeBeforeSet, dateTimeAfterSet, "GetMissionRealTimeInformation does return expected delay information");
                AssertAreEqual(weather, getResult.InformationStructure.MissionWeather, dateTimeBeforeSet, dateTimeAfterSet, "GetMissionRealTimeInformation does return expected weather information");

                _train2groundClientMock.Verify();
                _remoteDataStoreFactory.Verify();

                DateTime dateBeforeRetrieveMissionRealTimeData = DateTime.Now;
                PIS.Train.RealTime.SetMissionRealTimeRequest trainRequest = trainService.LastMissionRealTimeRequest;
                uint ageMax = (uint)(dateBeforeRetrieveMissionRealTimeData - dateTimeBeforeSet).Seconds;

                Assert.IsNotNull(trainRequest, "Mission real time information on the train was not updated as expected");
                Assert.AreEqual(missionCode, trainRequest.MissionID, "Train real-time service was updated with unexpected value for the mission id");
                Assert.AreEqual(PIS.Train.RealTime.ActionTypeEnum.Set, trainRequest.MissionDelayAction, "Train real-time service was updated with unexpected value for the mission delay action");
                Assert.AreEqual(PIS.Train.RealTime.ActionTypeEnum.Set, trainRequest.MissionWeatherAction, "Train real-time service was updated with unexpected value for the mission weather action");

                AssertAreEqual(delay, trainRequest.MissionDelay, 0, ageMax, "Real-time service on the Train was not updated properly with delay information");
                AssertAreEqual(weather, trainRequest.MissionWeather, 0, ageMax, "Real-time service on the Train was not updated properly with weather information");

                //--------------------------------------
                // STEP 2: Update delay information only
                //--------------------------------------
                trainService.LastMissionRealTimeRequest = null;
                updateDateSet = DateTime.Now.Subtract(TimeSpan.FromMinutes(3));
                delay = new RealTimeDelayType
                {
                    Delay = 3,
                    DelayReason = "Thunderstorm",
                    DelayReasonCode = 2,
                    UpdateDate = updateDateSet
                };
                weather = null;

                dateTimeBeforeSet = DateTime.Now;
                {
                    RealTimeSetMissionRealTimeInformationResult setResult = webService.SetMissionRealTimeInformation(sessionId, missionCode, delay, weather);
                    dateTimeAfterSet = DateTime.Now;

                    Assert.IsNotNull(setResult, "SetMissionRealTimeInformation does not return a valid object as expected");
                    Assert.AreEqual(RealTimeServiceErrorEnum.InfoNoWeatherData, setResult.ResultCode, "SetMissionRealTimeInformation does not return the expected result code");
                    Assert.AreEqual(generatedRequestGuid, setResult.RequestId, "SetMissionRealTimeInformation does not return a valid request id");

                    var dataStoreWeatherObject = ((IRTPISDataStore)rtpisDataStore).GetMissionRealTimeInformation(missionCode).MissionWeather;
                    var dataStoreDelayObject = ((IRTPISDataStore)rtpisDataStore).GetMissionRealTimeInformation(missionCode).MissionDelay;
                    Assert.AreNotSame(delay, dataStoreDelayObject, "SetMissionRealTimeInformation does not create a copy of the delay object");

                    AssertAreEqual(delay, dataStoreDelayObject, dateTimeBeforeSet, dateTimeAfterSet, "SetMissionRealTimeInformation does not update properly delay information in RTPIS data store");
                    AssertAreEqual(weather, dataStoreWeatherObject, dateTimeBeforeSet, dateTimeAfterSet, "SetMissionRealTimeInformation does not update properly weather information in RTPIS data store");
                }

                getResult = webService.GetMissionRealTimeInformation(sessionIdGet, missionCode);

                Assert.IsNotNull(getResult, "GetMissionRealTimeInformation does not return a valid object");
                Assert.AreEqual(missionCode, getResult.MissionCode, "GetMissionRealTimeInformation does not return the proper mission code");
                Assert.AreEqual(RealTimeServiceErrorEnum.InfoNoWeatherData, getResult.ResultCode, "GetMissionRealTimeInformation does not return the proper error code");
                Assert.AreEqual(generatedRequestGuidForGet, getResult.RequestId, "GetMissionRealTimeInformation does not return the expected request id");
                Assert.IsNotNull(getResult.InformationStructure, "GetMissionRealTimeInformation does not set the InformationStructure property");
                Assert.IsNotNull(getResult.InformationStructure.MissionDelay, "GetMissionRealTimeInformation does not return the delay information");
                Assert.IsNull(getResult.InformationStructure.MissionWeather, "GetMissionRealTimeInformation returns the weather information while expecting not");
                Assert.AreNotSame(delay, getResult.InformationStructure.MissionDelay, "GetMissionRealTimeInfortmation does not return a copy of the delay object");

                Assert.AreNotSame(((IRTPISDataStore)rtpisDataStore).GetMissionRealTimeInformation(missionCode).MissionDelay, getResult.InformationStructure.MissionDelay, "GetMissionRealTimeInfortmation does not return a copy of the delay object");

                AssertAreEqual(delay, getResult.InformationStructure.MissionDelay, dateTimeBeforeSet, dateTimeAfterSet, "GetMissionRealTimeInformation does return expected delay information");
                AssertAreEqual(weather, getResult.InformationStructure.MissionWeather, dateTimeBeforeSet, dateTimeAfterSet, "GetMissionRealTimeInformation does return expected weather information");

                dateBeforeRetrieveMissionRealTimeData = DateTime.Now;
                trainRequest = trainService.LastMissionRealTimeRequest;
                ageMax = (uint)(dateBeforeRetrieveMissionRealTimeData - dateTimeBeforeSet).Seconds;
                Assert.IsNotNull(trainRequest, "Mission real time information on the train was not updated as expected");
                Assert.AreEqual(missionCode, trainRequest.MissionID, "Train real-time service was updated with unexpected value for the mission id");
                Assert.AreEqual(PIS.Train.RealTime.ActionTypeEnum.Set, trainRequest.MissionDelayAction, "Train real-time service was updated with unexpected value for the mission delay action");
                Assert.AreEqual(PIS.Train.RealTime.ActionTypeEnum.Delete, trainRequest.MissionWeatherAction, "Train real-time service was updated with unexpected value for the mission weather action");
                Assert.IsNotNull(trainRequest.MissionDelay, "Train real-time service was updated with no delay information");
                Assert.IsNull(trainRequest.MissionWeather, "Train real-time service was updated with weather information while expecting not");

                AssertAreEqual(delay, trainRequest.MissionDelay, 0, ageMax, "Real-time service on the Train was not updated properly with delay information");
                AssertAreEqual(weather, trainRequest.MissionWeather, 0, ageMax, "Real-time service on the Train was not updated properly with weather information");

                //----------------------------------------
                // STEP 3: Update weather information only
                //----------------------------------------

                trainService.LastMissionRealTimeRequest = null;
                updateDateSet = DateTime.Now.Subtract(TimeSpan.FromMinutes(5));
                delay = null;
                weather = new RealTimeWeatherType
                {
                    Humidity = "10%",
                    TemperatureInCentigrade = "22",
                    TemperatureInFahrenheit = "71.6",
                    UpdateDate = updateDateSet,
                    WeatherCondition = "Cloudy",
                    WeatherConditionCode = 2
                };

                realTimeInfo = new RealTimeInformationType
                {
                    MissionDelay = delay,
                    MissionWeather = weather
                };

                dateTimeBeforeSet = DateTime.Now;
                {
                    RealTimeSetMissionRealTimeInformationResult setResult = webService.SetMissionRealTimeInformation(sessionId, missionCode, delay, weather);
                    dateTimeAfterSet = DateTime.Now;

                    Assert.IsNotNull(setResult, "SetMissionRealTimeInformation does not return a valid object as expected");
                    Assert.AreEqual(RealTimeServiceErrorEnum.InfoNoDelayData, setResult.ResultCode, "SetMissionRealTimeInformation does not return the expected result code");
                    Assert.AreEqual(generatedRequestGuid, setResult.RequestId, "SetMissionRealTimeInformation does not return a valid request id");

                    var dataStoreWeatherObject = ((IRTPISDataStore)rtpisDataStore).GetMissionRealTimeInformation(missionCode).MissionWeather;
                    var dataStoreDelayObject = ((IRTPISDataStore)rtpisDataStore).GetMissionRealTimeInformation(missionCode).MissionDelay;
                    Assert.AreNotSame(weather, dataStoreWeatherObject, "SetMissionRealTimeInformation does not create a copy of the weather object");

                    AssertAreEqual(delay, dataStoreDelayObject, dateTimeBeforeSet, dateTimeAfterSet, "SetMissionRealTimeInformation does not update properly delay information in RTPIS data store");
                    AssertAreEqual(weather, dataStoreWeatherObject, dateTimeBeforeSet, dateTimeAfterSet, "SetMissionRealTimeInformation does not update properly weather information in RTPIS data store");
                }

                getResult = webService.GetMissionRealTimeInformation(sessionIdGet, missionCode);

                Assert.IsNotNull(getResult, "GetMissionRealTimeInformation does not return a valid object");
                Assert.AreEqual(missionCode, getResult.MissionCode, "GetMissionRealTimeInformation does not return the proper mission code");
                Assert.AreEqual(RealTimeServiceErrorEnum.InfoNoDelayData, getResult.ResultCode, "GetMissionRealTimeInformation does not return the proper error code");
                Assert.AreEqual(generatedRequestGuidForGet, getResult.RequestId, "GetMissionRealTimeInformation does not return the expected request id");
                Assert.IsNotNull(getResult.InformationStructure, "GetMissionRealTimeInformation does not set the InformationStructure property");
                Assert.IsNull(getResult.InformationStructure.MissionDelay, "GetMissionRealTimeInformation does not return the proper value for the delay information");
                Assert.IsNotNull(getResult.InformationStructure.MissionWeather, "GetMissionRealTimeInformation does not return the weather information");
                Assert.AreNotSame(weather, getResult.InformationStructure.MissionWeather, "GetMissionRealTimeInfortmation does not return a copy of the weather object");

                Assert.AreNotSame(((IRTPISDataStore)rtpisDataStore).GetMissionRealTimeInformation(missionCode).MissionWeather, getResult.InformationStructure.MissionWeather, "GetMissionRealTimeInfortmation does not return a copy of the weather object");

                AssertAreEqual(delay, getResult.InformationStructure.MissionDelay, dateTimeBeforeSet, dateTimeAfterSet, "GetMissionRealTimeInformation does return expected delay information");
                AssertAreEqual(weather, getResult.InformationStructure.MissionWeather, dateTimeBeforeSet, dateTimeAfterSet, "GetMissionRealTimeInformation does return expected weather information");

                dateBeforeRetrieveMissionRealTimeData = DateTime.Now;
                trainRequest = trainService.LastMissionRealTimeRequest;
                ageMax = (uint)(dateBeforeRetrieveMissionRealTimeData - dateTimeBeforeSet).Seconds;
                Assert.IsNotNull(trainRequest, "Mission real time information on the train was not updated as expected");
                Assert.AreEqual(missionCode, trainRequest.MissionID, "Train real-time service was updated with unexpected value for the mission id");
                Assert.AreEqual(PIS.Train.RealTime.ActionTypeEnum.Delete, trainRequest.MissionDelayAction, "Train real-time service was updated with unexpected value for the mission delay action");
                Assert.AreEqual(PIS.Train.RealTime.ActionTypeEnum.Set, trainRequest.MissionWeatherAction, "Train real-time service was updated with unexpected value for the mission weather action");
                Assert.IsNull(trainRequest.MissionDelay, "Train real-time service was updated with delay information while expecting not");
                Assert.IsNotNull(trainRequest.MissionWeather, "Train real-time service was updated with no weather information");

                AssertAreEqual(delay, trainRequest.MissionDelay, 0, ageMax, "Real-time service on the Train was not updated properly with delay information");
                AssertAreEqual(weather, trainRequest.MissionWeather, 0, ageMax, "Real-time service on the Train was not updated properly with weather information");
            }

            this._train2groundClientMock.Verify(x => x.UnsubscribeFromElementChangeNotification(RequestProcessor.SubscriberId), Times.Once(), "RequestProcess does not unsubscribes to element change notification as expected or the RealTime service does not call Dispose method on request processor object.");
        }

        #endregion

        #region SetStationRealTimeInformation / GetStationRealTimeInformation

        [Test, Category("RealTime")]
        public void SetStationRealTimeInformation_CommunicateWithTrain_Nominal()
        {
            Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<ServiceContractAttribute>();
            // Initialization of the session manager
            SessionData sessionData = new SessionData();
            Guid sessionId = Guid.NewGuid();
            Guid sessionIdGet = Guid.NewGuid();
            Guid generatedRequestGuid = Guid.NewGuid();
            Guid generatedRequestGuidForGet = Guid.NewGuid();
            _sessionManagerMock.Setup(x => x.GetSessionDetails(sessionId, out sessionData)).Returns(string.Empty);
            _sessionManagerMock.Setup(x => x.GetSessionDetails(sessionIdGet, out sessionData)).Returns(string.Empty);
            _sessionManagerMock.Setup(x => x.IsSessionValid(sessionId)).Returns(true);
            _sessionManagerMock.Setup(x => x.IsSessionValid(sessionIdGet)).Returns(true);
            _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionId, out generatedRequestGuid)).Returns(string.Empty).Verifiable("Method SetMissionRealTimeInformation does not invoke GenerateRequestGuid as expected");
            _sessionManagerMock.Setup(x => x.GenerateRequestID(sessionIdGet, out generatedRequestGuidForGet)).Returns(string.Empty).Verifiable("Method GetMissionRealTimeInformation does not invoke GenerateRequestGuid as expected");

            // Initialization of the T2G manager
            bool isTrain1Online = true;
            string missionCode = "1";
            AvailableElementData train1 = CreateAvailableElement("TRAIN-1", isTrain1Online, "5.12.14.0", MissionStateEnum.MI, missionCode);
            ElementList<AvailableElementData> elementList = new ElementList<AvailableElementData>
            {
                train1
            };

            ServiceInfo train1ServiceInfo = new ServiceInfo((ushort)eServiceID.eSrvSIF_RealTimeServer, "RealTimeDataServer", 0, 0, isTrain1Online, "127.0.0.1" /* ip */, "", "", 8201 /* port */);
            Uri trainServiceAddress = new Uri("http://127.0.0.1:8201/");

            _train2groundClientMock.Setup(x => x.GetAvailableElementDataList(out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataByElementNumber("TRAIN-1", out train1)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableElementDataListByMissionCode(missionCode, out elementList)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.IsElementOnline("TRAIN-1", out isTrain1Online)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(x => x.GetAvailableServiceData("TRAIN-1", (int)eServiceID.eSrvSIF_RealTimeServer, out train1ServiceInfo)).Returns(T2GManagerErrorEnum.eSuccess);
            _train2groundClientMock.Setup(
                x => x.SubscribeToElementChangeNotification(
                    RequestProcessor.SubscriberId,
                    It.IsAny<EventHandler<ElementEventArgs>>())).Verifiable("RequestProcessor does not subscribes to element change notification as expected.");
            _train2groundClientMock.Setup(
                x => x.UnsubscribeFromElementChangeNotification(
                    RequestProcessor.SubscriberId));
            // Initialization of remote data store factory.
            _remoteDataStoreFactory.Setup(z => z.GetRemoteDataStoreInstance()).Returns(new PIS.Ground.RealTimeTests.RemoteDataStoreSimulator.RemoteDataStore(_dbWorkingPath));

            RTPISDataStore rtpisDataStore = new RTPISDataStore();

            RequestProcessor requestProcessor = new RequestProcessor(_train2groundClientMock.Object, rtpisDataStore);

            using (RealTimeTrainService trainService = new RealTimeTrainService(train1ServiceInfo.ServiceIPAddress, train1ServiceInfo.ServicePortNumber))
            using (RealTimeServiceTested realTimeService = new RealTimeServiceTested(
                    _train2groundClientMock.Object,
                    _sessionManagerMock.Object,
                    _notificationSenderMock.Object,
                    requestProcessor,
                    _remoteDataStoreFactory.Object,
                    rtpisDataStore))
            {
                Assert.DoesNotThrow(() => { trainService.Start(); }, "Unable to start real-time service for the train");
                realTimeService.PlateformType = CommonConfiguration.PlatformTypeEnum.URBAN;
                IRealTimeService webService = (IRealTimeService)realTimeService;

                //--------------------------------
                // STEP 1: All information is set
                //--------------------------------
                {   

                    RealTimeDelayType delayStation_1 = new RealTimeDelayType() {
                        Delay = 1,
                        DelayReason = "because",
                        DelayReasonCode = 56,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(2))
                    };

                    RealTimeWeatherType weatherStation_1 = new RealTimeWeatherType() {
                        Humidity = "5%",
                        TemperatureInCentigrade = "10",
                        TemperatureInFahrenheit = "50",
                        WeatherCondition = "Good",
                        WeatherConditionCode = 77,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(3))
                    };

                    RealTimeStationPlatformType platformStation_1 = new RealTimeStationPlatformType
                    {
                        ExitSide = "Left",
                        ExitSideCode = 1,
                        IssueDescription = "Nothing special",
                        IssueDescriptionCode = 3,
                        Platform = "Quaie",
                        Track = "P1",
                        TrackCode = 2,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(5))
                    };

                    RealTimeStationConnectionType connection_1_Station_1 = new RealTimeStationConnectionType()
                    {
                        CommercialNumber = "CN2",
                        ConnectionDelay = "5",
                        ConnectionPlatform = "C1S1 Platform",
                        DepartureFrequency = "10 minutes",
                        DepartureTime = "15:00",
                        DestinationName = "Mexico",
                        Line = "L1",
                        LineCode = 2,
                        ModelType = "En metal",
                        ModelTypeCode = 3,
                        NextDepartureTime = "17:00",
                        Operator = "STM",
                        OperatorCode = 3,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(5))
                    };

                    RealTimeStationConnectionType connection_2_Station_1 = new RealTimeStationConnectionType()
                    {
                        CommercialNumber = "CN3",
                        ConnectionDelay = "4",
                        ConnectionPlatform = "C2S1 Platform",
                        DepartureFrequency = "15 minutes",
                        DepartureTime = "15:06",
                        DestinationName = "Laval",
                        Line = "L2",
                        LineCode = 3,
                        ModelType = "En plastique",
                        ModelTypeCode = 4,
                        NextDepartureTime = "15:21",
                        Operator = "STL",
                        OperatorCode = 2,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(4))
                    };

                    List<RealTimeStationConnectionType> connectionsStation_1 = new List<RealTimeStationConnectionType>() {
                        connection_1_Station_1,
                        connection_2_Station_1
                    };

                    RealTimeStationDataType stationData_1 = new RealTimeStationDataType() {
                        StationDelay = delayStation_1,
                        StationWeather = weatherStation_1,
                        StationPlatform = platformStation_1,
                        StationConnectionList = connectionsStation_1
                    };

                    string codeStation_1 = "222";

                    RealTimeStationInformationType stationInfo_1 = new RealTimeStationInformationType()
                    {
                        StationCode = codeStation_1,
                        StationData = stationData_1
                    };

                    List<RealTimeStationInformationType> stationsInfo = new List<RealTimeStationInformationType>()
                    {
                        stationInfo_1
                    };

                    RealTimeStationResultType expectedSetResultStation_1 = new RealTimeStationResultType()
                    {
                        DelayResult = RealTimeStationResultEnum.DataOk,
                        PlatformResult = RealTimeStationResultEnum.DataOk,
                        StationID = codeStation_1,
                        WeatherResult = RealTimeStationResultEnum.DataOk,
                        ConnectionsResultList = new List<RealTimeConnectionResultType>() {
                            new RealTimeConnectionResultType() {
                                CommercialNumber = connection_1_Station_1.CommercialNumber,
                                Operator = connection_1_Station_1.Operator,
                                ConnectionResult = RealTimeStationResultEnum.DataOk
                            },
                            new RealTimeConnectionResultType() {
                                CommercialNumber = connection_2_Station_1.CommercialNumber,
                                Operator = connection_2_Station_1.Operator,
                                ConnectionResult = RealTimeStationResultEnum.DataOk
                            }
                        }
                    };

                    List<RealTimeStationResultType> expectedSetResult = new List<RealTimeStationResultType>() {
                        expectedSetResultStation_1
                    };

                    DateTime dateTimeBeforeSet = DateTime.Now;
                    RealTimeSetStationRealTimeInformationResult setResult = webService.SetStationRealTimeInformation(sessionId, missionCode, stationsInfo);
                    DateTime dateTimeAfterSet = DateTime.Now;
                    Assert.IsNotNull(setResult, "Method SetStationRealTimeInformation does not return a valid object");
                    Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, setResult.ResultCode, "Method SetStationRealTimeInformation does not return the expected error code");
                    Assert.AreEqual(generatedRequestGuid, setResult.RequestId, "Method SetStationRealTimeInformation does not return a valid request id");
                    Assert.AreEqual(missionCode, setResult.MissionCode, "Method SetStationRealTimeInformation does not return the expected mission code");
                    AssertListEqual(expectedSetResult, setResult.StationResultList, AssertAreEqual, "Method SetStationRealTimeInformation does not return the expected station result list");



                    List<string> stationCodes = new List<string>() {
                        codeStation_1
                    };

                    RealTimeStationStatusType rtpisStatusStation_1 = new RealTimeStationStatusType()
                    {
                        StationDelay = delayStation_1,
                        StationConnectionList = connectionsStation_1,
                        StationID = codeStation_1,
                        StationPlatform = platformStation_1,
                        StationResult = RealTimeStationResultEnum.DataOk,
                        StationWeather = weatherStation_1,
                    };

                    List<RealTimeStationStatusType> expectedRtpisResult = new List<RealTimeStationStatusType>()
                    {
                        rtpisStatusStation_1
                    };

                    List<RealTimeStationStatusType> rtpisResult = ((IRTPISDataStore)rtpisDataStore).GetStationRealTimeInformation(missionCode, stationCodes);

                    Action<RealTimeStationStatusType, RealTimeStationStatusType, string> comparerStationStatusType = (RealTimeStationStatusType c1, RealTimeStationStatusType c2, string em) => AssertAreEqual(c1, c2, dateTimeBeforeSet, dateTimeAfterSet, em);
                    AssertListEqual(expectedRtpisResult, rtpisResult, comparerStationStatusType, "RTPISDataStore Method GetStationRealTimeInformation does not return the expected result");

                    RealTimeGetStationRealTimeInformationResult getResult = webService.GetStationRealTimeInformation(sessionIdGet, missionCode, stationCodes);
                    Assert.IsNotNull(getResult, "Method GetStationRealTimeInformation does not return a valid object");
                    Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, getResult.ResultCode, "Method GetStationRealTimeInformation does not return the expected result code");
                    Assert.AreEqual(generatedRequestGuidForGet, getResult.RequestId, "Method GetStationRealTimeInformation does not return the expected request id");
                    AssertListEqual(expectedRtpisResult, getResult.StationStatusList, comparerStationStatusType, "Method GetStationRealTimeInformation does not return the expected data for queryed stations");

                    DateTime dateBeforeRetrieveMissionRealTimeData = DateTime.Now;
                    PIS.Train.RealTime.SetStationRealTimeRequest trainRequest = trainService.LastStationRealTimeRequest;
                    uint ageMax = (uint)(dateBeforeRetrieveMissionRealTimeData - dateTimeBeforeSet).Seconds;

                    Assert.IsNotNull(trainRequest, "Method SetStationRealTimeInformation does not send the information to the train.");
                    Assert.AreEqual(missionCode, trainRequest.MissionID, "Train real-time service received an unexpected mission id.");

                    Action<RealTimeStationInformationType, PIS.Train.RealTime.StationDataType, string> itemComparer = (x, y, z) => AssertAreEqual(x, y, 0, ageMax, "Train real-time  service didn't received the expected data");
                    AssertListEqual(stationsInfo, trainRequest.StationDataList, itemComparer, "Train real-time service didn't received expected data-update");
                }

                //--------------------------------
                // STEP 2: Send same data than step 1 except for connection information.
                //--------------------------------
                {
                    RealTimeDelayType delayStation_1 = new RealTimeDelayType()
                    {
                        Delay = 1,
                        DelayReason = "because",
                        DelayReasonCode = 56,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(2))
                    };

                    RealTimeWeatherType weatherStation_1 = new RealTimeWeatherType()
                    {
                        Humidity = "5%",
                        TemperatureInCentigrade = "10",
                        TemperatureInFahrenheit = "50",
                        WeatherCondition = "Good",
                        WeatherConditionCode = 77,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(3))
                    };

                    RealTimeStationPlatformType platformStation_1 = new RealTimeStationPlatformType
                    {
                        ExitSide = "Left",
                        ExitSideCode = 1,
                        IssueDescription = "Nothing special",
                        IssueDescriptionCode = 3,
                        Platform = "Quaie",
                        Track = "P1",
                        TrackCode = 2,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(5))
                    };

                    RealTimeStationConnectionType connection_1_Station_1 = new RealTimeStationConnectionType()
                    {
                        CommercialNumber = "CN4",
                        ConnectionDelay = "6",
                        ConnectionPlatform = "C1S1 Platform",
                        DepartureFrequency = "10 minutes",
                        DepartureTime = "15:00",
                        DestinationName = "Berri",
                        Line = "L2",
                        LineCode = 2,
                        ModelType = "En metal",
                        ModelTypeCode = 3,
                        NextDepartureTime = "17:00",
                        Operator = "STM",
                        OperatorCode = 3,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(5))
                    };

                    List<RealTimeStationConnectionType> connectionsStation_1 = new List<RealTimeStationConnectionType>() {
                        connection_1_Station_1
                    };

                    RealTimeStationDataType stationData_1 = new RealTimeStationDataType()
                    {
                        StationDelay = delayStation_1,
                        StationWeather = weatherStation_1,
                        StationPlatform = platformStation_1,
                        StationConnectionList = connectionsStation_1
                    };

                    string codeStation_1 = "222";

                    RealTimeStationInformationType stationInfo_1 = new RealTimeStationInformationType()
                    {
                        StationCode = codeStation_1,
                        StationData = stationData_1
                    };

                    List<RealTimeStationInformationType> stationsInfo = new List<RealTimeStationInformationType>()
                    {
                        stationInfo_1
                    };

                    RealTimeStationResultType expectedSetResultStation_1 = new RealTimeStationResultType()
                    {
                        DelayResult = RealTimeStationResultEnum.DataOk,
                        PlatformResult = RealTimeStationResultEnum.DataOk,
                        StationID = codeStation_1,
                        WeatherResult = RealTimeStationResultEnum.DataOk,
                        ConnectionsResultList = new List<RealTimeConnectionResultType>() {
                            new RealTimeConnectionResultType() {
                                CommercialNumber = connection_1_Station_1.CommercialNumber,
                                Operator = connection_1_Station_1.Operator,
                                ConnectionResult = RealTimeStationResultEnum.DataOk
                            }
                        }
                    };

                    List<RealTimeStationResultType> expectedSetResult = new List<RealTimeStationResultType>() {
                        expectedSetResultStation_1
                    };

                    DateTime dateTimeBeforeSet = DateTime.Now;
                    RealTimeSetStationRealTimeInformationResult setResult = webService.SetStationRealTimeInformation(sessionId, missionCode, stationsInfo);
                    DateTime dateTimeAfterSet = DateTime.Now;
                    Assert.IsNotNull(setResult, "Method SetStationRealTimeInformation does not return a valid object");
                    Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, setResult.ResultCode, "Method SetStationRealTimeInformation does not return the expected error code");
                    Assert.AreEqual(generatedRequestGuid, setResult.RequestId, "Method SetStationRealTimeInformation does not return a valid request id");
                    Assert.AreEqual(missionCode, setResult.MissionCode, "Method SetStationRealTimeInformation does not return the expected mission code");
                    AssertListEqual(expectedSetResult, setResult.StationResultList, AssertAreEqual, "Method SetStationRealTimeInformation does not return the expected station result list");

                    List<string> stationCodes = new List<string>() {
                        codeStation_1
                    };

                    RealTimeStationStatusType rtpisStatusStation_1 = new RealTimeStationStatusType()
                    {
                        StationDelay = delayStation_1,
                        StationConnectionList = connectionsStation_1,
                        StationID = codeStation_1,
                        StationPlatform = platformStation_1,
                        StationResult = RealTimeStationResultEnum.DataOk,
                        StationWeather = weatherStation_1,
                    };

                    List<RealTimeStationStatusType> expectedRtpisResult = new List<RealTimeStationStatusType>()
                    {
                        rtpisStatusStation_1
                    };

                    List<RealTimeStationStatusType> rtpisResult = ((IRTPISDataStore)rtpisDataStore).GetStationRealTimeInformation(missionCode, stationCodes);

                    Action<RealTimeStationStatusType, RealTimeStationStatusType, string> comparerStationStatusType = (RealTimeStationStatusType c1, RealTimeStationStatusType c2, string em) => AssertAreEqual(c1, c2, dateTimeBeforeSet, dateTimeAfterSet, em);
                    AssertListEqual(expectedRtpisResult, rtpisResult, comparerStationStatusType, "RTPISDataStore Method GetStationRealTimeInformation does not return the expected result");

                    RealTimeGetStationRealTimeInformationResult getResult = webService.GetStationRealTimeInformation(sessionIdGet, missionCode, stationCodes);
                    Assert.IsNotNull(getResult, "Method GetStationRealTimeInformation does not return a valid object");
                    Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, getResult.ResultCode, "Method GetStationRealTimeInformation does not return the expected result code");
                    Assert.AreEqual(generatedRequestGuidForGet, getResult.RequestId, "Method GetStationRealTimeInformation does not return the expected request id");
                    AssertListEqual(expectedRtpisResult, getResult.StationStatusList, comparerStationStatusType, "Method GetStationRealTimeInformation does not return the expected data for queryed stations");

                    DateTime dateBeforeRetrieveMissionRealTimeData = DateTime.Now;
                    PIS.Train.RealTime.SetStationRealTimeRequest trainRequest = trainService.LastStationRealTimeRequest;
                    uint ageMax = (uint)(dateBeforeRetrieveMissionRealTimeData - dateTimeBeforeSet).Seconds;

                    Assert.IsNotNull(trainRequest, "Method SetStationRealTimeInformation does not send the information to the train.");
                    Assert.AreEqual(missionCode, trainRequest.MissionID, "Train real-time service received an unexpected mission id.");

                    Action<RealTimeStationInformationType, PIS.Train.RealTime.StationDataType, string> itemComparer = (x, y, z) => AssertAreEqual(x, y, 0, ageMax, "Train real-time  service didn't received the expected data");
                    AssertListEqual(stationsInfo, trainRequest.StationDataList, itemComparer, "Train real-time service didn't received expected data-update");
                }

                //------------------------------------------------------------------------
                // STEP 3: Update connections and weather. Delay and platform set to null.
                //         Add new station
                //------------------------------------------------------------------------
                {
                    RealTimeDelayType delayStation_1 = null;
                    RealTimeDelayType delayStation_2 = null;
                    RealTimeWeatherType weatherStation_1 = new RealTimeWeatherType()
                    {
                        Humidity = "10%",
                        TemperatureInCentigrade = "20",
                        TemperatureInFahrenheit = "68",
                        WeatherCondition = "Sunny",
                        WeatherConditionCode = 8,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(2))
                    };

                    RealTimeWeatherType weatherStation_2 = new RealTimeWeatherType()
                    {
                        Humidity = "10%",
                        TemperatureInCentigrade = "21",
                        TemperatureInFahrenheit = "69",
                        WeatherCondition = "Sunny",
                        WeatherConditionCode = 8,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(2))
                    };

                    RealTimeStationPlatformType platformStation_1 = null;
                    RealTimeStationPlatformType platformStation_2 = new RealTimeStationPlatformType
                    {
                        ExitSide = "Right",
                        ExitSideCode = 2,
                        IssueDescription = "Slow opening",
                        IssueDescriptionCode = 5,
                        Platform = "Concrete",
                        Track = "P2",
                        TrackCode = 4,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(5))
                    };

                    RealTimeStationConnectionType connection_1_Station_1 = new RealTimeStationConnectionType()
                    {
                        CommercialNumber = "CN2",
                        ConnectionDelay = "5",
                        ConnectionPlatform = "C1S1 Platform",
                        DepartureFrequency = "10 minutes",
                        DepartureTime = "15:00",
                        DestinationName = "Mexico",
                        Line = "L1",
                        LineCode = 2,
                        ModelType = "En metal",
                        ModelTypeCode = 3,
                        NextDepartureTime = "17:00",
                        Operator = "STM",
                        OperatorCode = 3,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(5))
                    };

                    RealTimeStationConnectionType connection_1_Station_2 = new RealTimeStationConnectionType()
                    {
                        CommercialNumber = "SS2",
                        ConnectionDelay = "1",
                        ConnectionPlatform = "C1S2 Platform",
                        DepartureFrequency = "10 minutes",
                        DepartureTime = "15:00",
                        DestinationName = "Mexico",
                        Line = "L6",
                        LineCode = 2,
                        ModelType = "Bus",
                        ModelTypeCode = 8,
                        NextDepartureTime = "15:10",
                        Operator = "STM",
                        OperatorCode = 3,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(5))
                    };

                    RealTimeStationConnectionType connection_2_Station_1 = new RealTimeStationConnectionType()
                    {
                        CommercialNumber = "CN6",
                        ConnectionDelay = "8",
                        ConnectionPlatform = "C2S1 Platform",
                        DepartureFrequency = "5 minutes",
                        DepartureTime = "15:16",
                        DestinationName = "Longueuil",
                        Line = "L4",
                        LineCode = 6,
                        ModelType = "Azur",
                        ModelTypeCode = 9,
                        NextDepartureTime = "15:21",
                        Operator = "AMT",
                        OperatorCode = 5,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(7))
                    };

                    List<RealTimeStationConnectionType> connectionsStation_1 = new List<RealTimeStationConnectionType>() {
                        connection_1_Station_1,
                        connection_2_Station_1
                    };
                    List<RealTimeStationConnectionType> connectionsStation_2 = new List<RealTimeStationConnectionType>() {
                        connection_1_Station_2
                    };

                    RealTimeStationDataType stationData_1 = new RealTimeStationDataType()
                    {
                        StationDelay = delayStation_1,
                        StationWeather = weatherStation_1,
                        StationPlatform = platformStation_1,
                        StationConnectionList = connectionsStation_1
                    };

                    RealTimeStationDataType stationData_2 = new RealTimeStationDataType()
                    {
                        StationDelay = delayStation_2,
                        StationWeather = weatherStation_2,
                        StationPlatform = platformStation_2,
                        StationConnectionList = connectionsStation_2
                    };

                    string codeStation_1 = "222";
                    string codeStation_2 = "224";

                    RealTimeStationInformationType stationInfo_1 = new RealTimeStationInformationType()
                    {
                        StationCode = codeStation_1,
                        StationData = stationData_1
                    };

                    RealTimeStationInformationType stationInfo_2 = new RealTimeStationInformationType()
                    {
                        StationCode = codeStation_2,
                        StationData = stationData_2
                    };

                    List<RealTimeStationInformationType> stationsInfo = new List<RealTimeStationInformationType>()
                    {
                        stationInfo_1,
                        stationInfo_2
                    };

                    RealTimeStationResultType expectedSetResultStation_1 = new RealTimeStationResultType()
                    {
                        DelayResult = RealTimeStationResultEnum.InfoNoData,
                        PlatformResult = RealTimeStationResultEnum.InfoNoData,
                        StationID = codeStation_1,
                        WeatherResult = RealTimeStationResultEnum.DataOk,
                        ConnectionsResultList = new List<RealTimeConnectionResultType>() {
                            new RealTimeConnectionResultType() {
                                CommercialNumber = connection_1_Station_1.CommercialNumber,
                                Operator = connection_1_Station_1.Operator,
                                ConnectionResult = RealTimeStationResultEnum.DataOk
                            },
                            new RealTimeConnectionResultType() {
                                CommercialNumber = connection_2_Station_1.CommercialNumber,
                                Operator = connection_2_Station_1.Operator,
                                ConnectionResult = RealTimeStationResultEnum.DataOk
                            }
                        }
                    };

                    RealTimeStationResultType expectedSetResultStation_2 = new RealTimeStationResultType()
                    {
                        DelayResult = RealTimeStationResultEnum.InfoNoData,
                        PlatformResult = RealTimeStationResultEnum.DataOk,
                        StationID = codeStation_2,
                        WeatherResult = RealTimeStationResultEnum.DataOk,
                        ConnectionsResultList = new List<RealTimeConnectionResultType>() {
                            new RealTimeConnectionResultType() {
                                CommercialNumber = connection_1_Station_2.CommercialNumber,
                                Operator = connection_1_Station_2.Operator,
                                ConnectionResult = RealTimeStationResultEnum.DataOk
                            }
                        }
                    };

                    List<RealTimeStationResultType> expectedSetResult = new List<RealTimeStationResultType>() {
                        expectedSetResultStation_1,
                        expectedSetResultStation_2
                    };

                    DateTime dateTimeBeforeSet = DateTime.Now;
                    RealTimeSetStationRealTimeInformationResult setResult = webService.SetStationRealTimeInformation(sessionId, missionCode, stationsInfo);
                    DateTime dateTimeAfterSet = DateTime.Now;
                    Assert.IsNotNull(setResult, "Method SetStationRealTimeInformation does not return a valid object");
                    Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, setResult.ResultCode, "Method SetStationRealTimeInformation does not return the expected error code");
                    Assert.AreEqual(generatedRequestGuid, setResult.RequestId, "Method SetStationRealTimeInformation does not return a valid request id");
                    Assert.AreEqual(missionCode, setResult.MissionCode, "Method SetStationRealTimeInformation does not return the expected mission code");
                    AssertListEqual(expectedSetResult, setResult.StationResultList, AssertAreEqual, "Method SetStationRealTimeInformation does not return the expected station result list");

                    List<string> stationCodes = new List<string>() {
                        codeStation_1,
                        codeStation_2
                    };

                    RealTimeStationStatusType rtpisStatusStation_1 = new RealTimeStationStatusType()
                    {
                        StationDelay = delayStation_1,
                        StationConnectionList = connectionsStation_1,
                        StationID = codeStation_1,
                        StationPlatform = platformStation_1,
                        StationResult = RealTimeStationResultEnum.DataOk,
                        StationWeather = weatherStation_1,
                    };

                    RealTimeStationStatusType rtpisStatusStation_2 = new RealTimeStationStatusType()
                    {
                        StationDelay = delayStation_2,
                        StationConnectionList = connectionsStation_2,
                        StationID = codeStation_2,
                        StationPlatform = platformStation_2,
                        StationResult = RealTimeStationResultEnum.DataOk,
                        StationWeather = weatherStation_2,
                    };

                    List<RealTimeStationStatusType> expectedRtpisResult = new List<RealTimeStationStatusType>()
                    {
                        rtpisStatusStation_1,
                        rtpisStatusStation_2,
                    };

                    List<RealTimeStationStatusType> rtpisResult = ((IRTPISDataStore)rtpisDataStore).GetStationRealTimeInformation(missionCode, stationCodes);

                    Action<RealTimeStationStatusType, RealTimeStationStatusType, string> comparerStationStatusType = (RealTimeStationStatusType c1, RealTimeStationStatusType c2, string em) => AssertAreEqual(c1, c2, dateTimeBeforeSet, dateTimeAfterSet, em);
                    AssertListEqual(expectedRtpisResult, rtpisResult, comparerStationStatusType, "RTPISDataStore Method GetStationRealTimeInformation does not return the expected result");

                    RealTimeGetStationRealTimeInformationResult getResult = webService.GetStationRealTimeInformation(sessionIdGet, missionCode, stationCodes);
                    Assert.IsNotNull(getResult, "Method GetStationRealTimeInformation does not return a valid object");
                    Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, getResult.ResultCode, "Method GetStationRealTimeInformation does not return the expected result code");
                    Assert.AreEqual(generatedRequestGuidForGet, getResult.RequestId, "Method GetStationRealTimeInformation does not return the expected request id");
                    AssertListEqual(expectedRtpisResult, getResult.StationStatusList, comparerStationStatusType, "Method GetStationRealTimeInformation does not return the expected data for queryed stations");

                    DateTime dateBeforeRetrieveMissionRealTimeData = DateTime.Now;
                    PIS.Train.RealTime.SetStationRealTimeRequest trainRequest = trainService.LastStationRealTimeRequest;
                    uint ageMax = (uint)(dateBeforeRetrieveMissionRealTimeData - dateTimeBeforeSet).Seconds;

                    Assert.IsNotNull(trainRequest, "Method SetStationRealTimeInformation does not send the information to the train.");
                    Assert.AreEqual(missionCode, trainRequest.MissionID, "Train real-time service received an unexpected mission id.");

                    Action<RealTimeStationInformationType, PIS.Train.RealTime.StationDataType, string> itemComparer = (x, y, z) => AssertAreEqual(x, y, 0, ageMax, "Train real-time  service didn't received the expected data");
                    AssertListEqual(stationsInfo, trainRequest.StationDataList, itemComparer, "Train real-time service didn't received expected data-update");
                }

                //------------------------------------------------------------------------
                // STEP 4: Update Delay and platform. Connections and weather are null.
                //------------------------------------------------------------------------
                {
                    RealTimeDelayType delayStation_1 = new RealTimeDelayType()
                    {
                        Delay = 7,
                        DelayReason = "I don't know",
                        DelayReasonCode = 99,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(2))
                    };

                    RealTimeWeatherType weatherStation_1 = null;

                    RealTimeStationPlatformType platformStation_1 = new RealTimeStationPlatformType
                    {
                        ExitSide = "Both",
                        ExitSideCode = 3,
                        IssueDescription = "In the middle",
                        IssueDescriptionCode = 6,
                        Platform = "Sand",
                        Track = "M2",
                        TrackCode = 6,
                        UpdateDate = DateTime.Now.Subtract(TimeSpan.FromHours(5))
                    };


                    List<RealTimeStationConnectionType> connectionsStation_1 = null;

                    RealTimeStationDataType stationData_1 = new RealTimeStationDataType()
                    {
                        StationDelay = delayStation_1,
                        StationWeather = weatherStation_1,
                        StationPlatform = platformStation_1,
                        StationConnectionList = connectionsStation_1
                    };

                    string codeStation_1 = "222";

                    RealTimeStationInformationType stationInfo_1 = new RealTimeStationInformationType()
                    {
                        StationCode = codeStation_1,
                        StationData = stationData_1
                    };

                    List<RealTimeStationInformationType> stationsInfo = new List<RealTimeStationInformationType>()
                    {
                        stationInfo_1
                    };

                    RealTimeStationResultType expectedSetResultStation_1 = new RealTimeStationResultType()
                    {
                        DelayResult = RealTimeStationResultEnum.DataOk,
                        PlatformResult = RealTimeStationResultEnum.DataOk,
                        StationID = codeStation_1,
                        WeatherResult = RealTimeStationResultEnum.InfoNoData,
                        ConnectionsResultList = null
                    };

                    List<RealTimeStationResultType> expectedSetResult = new List<RealTimeStationResultType>() {
                        expectedSetResultStation_1
                    };

                    DateTime dateTimeBeforeSet = DateTime.Now;
                    RealTimeSetStationRealTimeInformationResult setResult = webService.SetStationRealTimeInformation(sessionId, missionCode, stationsInfo);
                    DateTime dateTimeAfterSet = DateTime.Now;
                    Assert.IsNotNull(setResult, "Method SetStationRealTimeInformation does not return a valid object");
                    Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, setResult.ResultCode, "Method SetStationRealTimeInformation does not return the expected error code");
                    Assert.AreEqual(generatedRequestGuid, setResult.RequestId, "Method SetStationRealTimeInformation does not return a valid request id");
                    Assert.AreEqual(missionCode, setResult.MissionCode, "Method SetStationRealTimeInformation does not return the expected mission code");
                    AssertListEqual(expectedSetResult, setResult.StationResultList, AssertAreEqual, "Method SetStationRealTimeInformation does not return the expected station result list");



                    List<string> stationCodes = new List<string>() {
                        codeStation_1
                    };

                    RealTimeStationStatusType rtpisStatusStation_1 = new RealTimeStationStatusType()
                    {
                        StationDelay = delayStation_1,
                        StationConnectionList = connectionsStation_1,
                        StationID = codeStation_1,
                        StationPlatform = platformStation_1,
                        StationResult = RealTimeStationResultEnum.DataOk,
                        StationWeather = weatherStation_1,
                    };

                    List<RealTimeStationStatusType> expectedRtpisResult = new List<RealTimeStationStatusType>()
                    {
                        rtpisStatusStation_1
                    };

                    List<RealTimeStationStatusType> rtpisResult = ((IRTPISDataStore)rtpisDataStore).GetStationRealTimeInformation(missionCode, stationCodes);

                    Action<RealTimeStationStatusType, RealTimeStationStatusType, string> comparerStationStatusType = (RealTimeStationStatusType c1, RealTimeStationStatusType c2, string em) => AssertAreEqual(c1, c2, dateTimeBeforeSet, dateTimeAfterSet, em);
                    AssertListEqual(expectedRtpisResult, rtpisResult, comparerStationStatusType, "RTPISDataStore Method GetStationRealTimeInformation does not return the expected result");

                    RealTimeGetStationRealTimeInformationResult getResult = webService.GetStationRealTimeInformation(sessionIdGet, missionCode, stationCodes);
                    Assert.IsNotNull(getResult, "Method GetStationRealTimeInformation does not return a valid object");
                    Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, getResult.ResultCode, "Method GetStationRealTimeInformation does not return the expected result code");
                    Assert.AreEqual(generatedRequestGuidForGet, getResult.RequestId, "Method GetStationRealTimeInformation does not return the expected request id");
                    AssertListEqual(expectedRtpisResult, getResult.StationStatusList, comparerStationStatusType, "Method GetStationRealTimeInformation does not return the expected data for queryed stations");

                    DateTime dateBeforeRetrieveMissionRealTimeData = DateTime.Now;
                    PIS.Train.RealTime.SetStationRealTimeRequest trainRequest = trainService.LastStationRealTimeRequest;
                    uint ageMax = (uint)(dateBeforeRetrieveMissionRealTimeData - dateTimeBeforeSet).Seconds;

                    Assert.IsNotNull(trainRequest, "Method SetStationRealTimeInformation does not send the information to the train.");
                    Assert.AreEqual(missionCode, trainRequest.MissionID, "Train real-time service received an unexpected mission id.");

                    Action<RealTimeStationInformationType, PIS.Train.RealTime.StationDataType, string> itemComparer = (x, y, z) => AssertAreEqual(x, y, 0, ageMax, "Train real-time  service didn't received the expected data");
                    AssertListEqual(stationsInfo, trainRequest.StationDataList, itemComparer, "Train real-time service didn't received expected data-update");
                }

                //------------------------------------------------------------------------
                // STEP 4: Platform, weather, connection and delay are null
                //------------------------------------------------------------------------
                {
                    RealTimeDelayType delayStation_1 = null;

                    RealTimeWeatherType weatherStation_1 = null;

                    RealTimeStationPlatformType platformStation_1 = null;

                    List<RealTimeStationConnectionType> connectionsStation_1 = null;

                    RealTimeStationDataType stationData_1 = new RealTimeStationDataType()
                    {
                        StationDelay = delayStation_1,
                        StationWeather = weatherStation_1,
                        StationPlatform = platformStation_1,
                        StationConnectionList = connectionsStation_1
                    };

                    string codeStation_1 = "222";

                    RealTimeStationInformationType stationInfo_1 = new RealTimeStationInformationType()
                    {
                        StationCode = codeStation_1,
                        StationData = stationData_1
                    };

                    List<RealTimeStationInformationType> stationsInfo = new List<RealTimeStationInformationType>()
                    {
                        stationInfo_1
                    };

                    RealTimeStationResultType expectedSetResultStation_1 = new RealTimeStationResultType()
                    {
                        DelayResult = RealTimeStationResultEnum.InfoNoData,
                        PlatformResult = RealTimeStationResultEnum.InfoNoData,
                        StationID = codeStation_1,
                        WeatherResult = RealTimeStationResultEnum.InfoNoData,
                        ConnectionsResultList = null
                    };

                    List<RealTimeStationResultType> expectedSetResult = new List<RealTimeStationResultType>() {
                        expectedSetResultStation_1
                    };

                    DateTime dateTimeBeforeSet = DateTime.Now;
                    RealTimeSetStationRealTimeInformationResult setResult = webService.SetStationRealTimeInformation(sessionId, missionCode, stationsInfo);
                    DateTime dateTimeAfterSet = DateTime.Now;
                    Assert.IsNotNull(setResult, "Method SetStationRealTimeInformation does not return a valid object");
                    Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, setResult.ResultCode, "Method SetStationRealTimeInformation does not return the expected error code");
                    Assert.AreEqual(generatedRequestGuid, setResult.RequestId, "Method SetStationRealTimeInformation does not return a valid request id");
                    Assert.AreEqual(missionCode, setResult.MissionCode, "Method SetStationRealTimeInformation does not return the expected mission code");
                    AssertListEqual(expectedSetResult, setResult.StationResultList, AssertAreEqual, "Method SetStationRealTimeInformation does not return the expected station result list");


                    List<string> stationCodes = new List<string>() {
                        codeStation_1
                    };

                    RealTimeStationStatusType rtpisStatusStation_1 = new RealTimeStationStatusType()
                    {
                        StationDelay = delayStation_1,
                        StationConnectionList = connectionsStation_1,
                        StationID = codeStation_1,
                        StationPlatform = platformStation_1,
                        StationResult = RealTimeStationResultEnum.InfoNoData,
                        StationWeather = weatherStation_1,
                    };

                    List<RealTimeStationStatusType> expectedRtpisResult = new List<RealTimeStationStatusType>()
                    {
                        rtpisStatusStation_1
                    };

                    List<RealTimeStationStatusType> rtpisResult = ((IRTPISDataStore)rtpisDataStore).GetStationRealTimeInformation(missionCode, stationCodes);

                    Action<RealTimeStationStatusType, RealTimeStationStatusType, string> comparerStationStatusType = (RealTimeStationStatusType c1, RealTimeStationStatusType c2, string em) => AssertAreEqual(c1, c2, dateTimeBeforeSet, dateTimeAfterSet, em);
                    AssertListEqual(expectedRtpisResult, rtpisResult, comparerStationStatusType, "RTPISDataStore Method GetStationRealTimeInformation does not return the expected result");

                    RealTimeGetStationRealTimeInformationResult getResult = webService.GetStationRealTimeInformation(sessionIdGet, missionCode, stationCodes);
                    Assert.IsNotNull(getResult, "Method GetStationRealTimeInformation does not return a valid object");
                    Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, getResult.ResultCode, "Method GetStationRealTimeInformation does not return the expected result code");
                    Assert.AreEqual(generatedRequestGuidForGet, getResult.RequestId, "Method GetStationRealTimeInformation does not return the expected request id");
                    AssertListEqual(expectedRtpisResult, getResult.StationStatusList, comparerStationStatusType, "Method GetStationRealTimeInformation does not return the expected data for queryed stations");

                    DateTime dateBeforeRetrieveMissionRealTimeData = DateTime.Now;
                    PIS.Train.RealTime.SetStationRealTimeRequest trainRequest = trainService.LastStationRealTimeRequest;
                    uint ageMax = (uint)(dateBeforeRetrieveMissionRealTimeData - dateTimeBeforeSet).Seconds;

                    Assert.IsNotNull(trainRequest, "Method SetStationRealTimeInformation does not send the information to the train.");
                    Assert.AreEqual(missionCode, trainRequest.MissionID, "Train real-time service received an unexpected mission id.");

                    Action<RealTimeStationInformationType, PIS.Train.RealTime.StationDataType, string> itemComparer = (x, y, z) => AssertAreEqual(x, y, 0, ageMax, "Train real-time  service didn't received the expected data");
                    AssertListEqual(stationsInfo, trainRequest.StationDataList, itemComparer, "Train real-time service didn't received expected data-update");
                }

                //------------------------------------------------------------------------
                // STEP 4: StationData is null
                //------------------------------------------------------------------------
                {
                    RealTimeDelayType delayStation_1 = null;

                    RealTimeWeatherType weatherStation_1 = null;

                    RealTimeStationPlatformType platformStation_1 = null;

                    List<RealTimeStationConnectionType> connectionsStation_1 = null;

                    RealTimeStationDataType stationData_1 = null;
                    string codeStation_1 = "222";

                    RealTimeStationInformationType stationInfo_1 = new RealTimeStationInformationType()
                    {
                        StationCode = codeStation_1,
                        StationData = stationData_1
                    };

                    List<RealTimeStationInformationType> stationsInfo = new List<RealTimeStationInformationType>()
                    {
                        stationInfo_1
                    };

                    RealTimeStationResultType expectedSetResultStation_1 = new RealTimeStationResultType()
                    {
                        DelayResult = RealTimeStationResultEnum.InfoNoData,
                        PlatformResult = RealTimeStationResultEnum.InfoNoData,
                        StationID = codeStation_1,
                        WeatherResult = RealTimeStationResultEnum.InfoNoData,
                        ConnectionsResultList = null
                    };

                    List<RealTimeStationResultType> expectedSetResult = new List<RealTimeStationResultType>() {
                        expectedSetResultStation_1
                    };

                    DateTime dateTimeBeforeSet = DateTime.Now;
                    RealTimeSetStationRealTimeInformationResult setResult = webService.SetStationRealTimeInformation(sessionId, missionCode, stationsInfo);
                    DateTime dateTimeAfterSet = DateTime.Now;
                    Assert.IsNotNull(setResult, "Method SetStationRealTimeInformation does not return a valid object");
                    Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, setResult.ResultCode, "Method SetStationRealTimeInformation does not return the expected error code");
                    Assert.AreEqual(generatedRequestGuid, setResult.RequestId, "Method SetStationRealTimeInformation does not return a valid request id");
                    Assert.AreEqual(missionCode, setResult.MissionCode, "Method SetStationRealTimeInformation does not return the expected mission code");
                    AssertListEqual(expectedSetResult, setResult.StationResultList, AssertAreEqual, "Method SetStationRealTimeInformation does not return the expected station result list");


                    List<string> stationCodes = new List<string>() {
                        codeStation_1
                    };

                    RealTimeStationStatusType rtpisStatusStation_1 = new RealTimeStationStatusType()
                    {
                        StationDelay = delayStation_1,
                        StationConnectionList = connectionsStation_1,
                        StationID = codeStation_1,
                        StationPlatform = platformStation_1,
                        StationResult = RealTimeStationResultEnum.InfoNoData,
                        StationWeather = weatherStation_1,
                    };

                    List<RealTimeStationStatusType> expectedRtpisResult = new List<RealTimeStationStatusType>()
                    {
                        rtpisStatusStation_1
                    };

                    List<RealTimeStationStatusType> rtpisResult = ((IRTPISDataStore)rtpisDataStore).GetStationRealTimeInformation(missionCode, stationCodes);

                    Action<RealTimeStationStatusType, RealTimeStationStatusType, string> comparerStationStatusType = (RealTimeStationStatusType c1, RealTimeStationStatusType c2, string em) => AssertAreEqual(c1, c2, dateTimeBeforeSet, dateTimeAfterSet, em);
                    AssertListEqual(expectedRtpisResult, rtpisResult, comparerStationStatusType, "RTPISDataStore Method GetStationRealTimeInformation does not return the expected result");

                    RealTimeGetStationRealTimeInformationResult getResult = webService.GetStationRealTimeInformation(sessionIdGet, missionCode, stationCodes);
                    Assert.IsNotNull(getResult, "Method GetStationRealTimeInformation does not return a valid object");
                    Assert.AreEqual(RealTimeServiceErrorEnum.RequestAccepted, getResult.ResultCode, "Method GetStationRealTimeInformation does not return the expected result code");
                    Assert.AreEqual(generatedRequestGuidForGet, getResult.RequestId, "Method GetStationRealTimeInformation does not return the expected request id");
                    AssertListEqual(expectedRtpisResult, getResult.StationStatusList, comparerStationStatusType, "Method GetStationRealTimeInformation does not return the expected data for queryed stations");

                    DateTime dateBeforeRetrieveMissionRealTimeData = DateTime.Now;
                    PIS.Train.RealTime.SetStationRealTimeRequest trainRequest = trainService.LastStationRealTimeRequest;
                    uint ageMax = (uint)(dateBeforeRetrieveMissionRealTimeData - dateTimeBeforeSet).Seconds;

                    Assert.IsNotNull(trainRequest, "Method SetStationRealTimeInformation does not send the information to the train.");
                    Assert.AreEqual(missionCode, trainRequest.MissionID, "Train real-time service received an unexpected mission id.");

                    // Update the StationData because on embedded side, it is not supposed to be null.
                    RealTimeStationDataType stationData_1_ws = new RealTimeStationDataType()
                    {
                        StationConnectionList = null,
                        StationDelay = null,
                        StationPlatform = null,
                        StationWeather = null
                    };

                    stationInfo_1.StationData = stationData_1_ws;

                    Action<RealTimeStationInformationType, PIS.Train.RealTime.StationDataType, string> itemComparer = (x, y, z) => AssertAreEqual(x, y, 0, ageMax, "Train real-time  service didn't received the expected data");
                    AssertListEqual(stationsInfo, trainRequest.StationDataList, itemComparer, "Train real-time service didn't received expected data-update");
                }
            }

            this._train2groundClientMock.Verify(x => x.UnsubscribeFromElementChangeNotification(RequestProcessor.SubscriberId), Times.Once(), "RequestProcess does not unsubscribes to element change notification as expected or the RealTime service does not call Dispose method on request processor object.");
        }

        #endregion

        #endregion
    }
}
