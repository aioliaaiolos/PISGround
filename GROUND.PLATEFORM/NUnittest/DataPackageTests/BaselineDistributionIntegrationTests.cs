//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineDistributionIntegrationTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DataPackageTests
{
    /// <summary>
    /// Class that test baseline distribution scenarios from distribute baseline until completion on embedded system.
    /// 
    /// This class simulate T2G, services on train to perform a complete validation of expected status of every stages of a baseline distribution.
    /// This class also validate the history log database and the notifications send by PIS-Ground.
    /// </summary>
    [TestFixture]
    class BaselineDistributionIntegrationTests
    {
        #region Fields

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaselineDistributionIntegrationTests"/> class.
        /// </summary>
        public BaselineDistributionIntegrationTests()
        {
            // No logic to apply
        }

        #endregion

        #region Tests managment

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

        #region Test - Baseline distribution nominal scenario

        /// <summary>
        /// Distributes the baseline scenario nominal.
        /// </summary>
        [Test]
        void DistributeBaselineScenario_Nominal()
        {

        }

        #endregion

        #region Utilities methods
        #endregion
    }
}
