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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using PIS.Ground.Core.Data;

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

            if (!Directory.Exists(_dbWorkingPath))
            {
                Directory.CreateDirectory(_dbWorkingPath);
            }

            string from = Path.Combine(_dbSourcePath, UrbanDB);
            string to = Path.Combine(_dbWorkingPath, UrbanDB);
            File.Copy(from, to, true);
            File.SetAttributes(to, FileAttributes.ReadOnly | FileAttributes.Temporary);
        }

        /// <summary>Method called once to perform cleanup when all test of this fixture has been executed.</summary>
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
        }

        /// <summary>Method called after the execution of every test</summary>
        [TearDown]
        public void TearDown()
        {


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

        #endregion
    }
}
