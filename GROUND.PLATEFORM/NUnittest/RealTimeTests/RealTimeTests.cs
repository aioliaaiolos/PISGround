// <copyright file="IRealTimeService.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2014.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
using System.Collections.Generic;
using System.Xml;
using Microsoft.XmlDiffPatch;
using NUnit.Framework;
using PIS.Ground.Core.Data;

/// Test classes for RealTime Service.
///
namespace RealTimeTests
{
    /// <summary>RealTime service test class.</summary>
    [TestFixture]
    public class RealTimeServiceTests
    {
        #region Tests management

        /// <summary>Initializes a new instance of the RealTimeServiceTests class.</summary>
        public RealTimeServiceTests()
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

		#region Tests
		
		/// <summary>Searches for the first pending transfer tasks.</summary>
        [Test]
        public void statusReportGenerator()
        {
        }

        #endregion
    }
}
