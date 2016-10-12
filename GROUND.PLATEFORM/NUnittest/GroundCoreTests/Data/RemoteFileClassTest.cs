//---------------------------------------------------------------------------------------------------
// <copyright file="RemoteFileClassTest.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PIS.Ground.Core;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using System.Reflection;
using System.Diagnostics;
using PIS.Ground.Core.Data;
using System.Configuration;
using System.IO;

namespace GroundCoreTests
{
    /// <summary>RemoteFileClassTest test class.</summary>
    [TestFixture]
    public class RemoteFileClassTest
    {
        #region attributes

        /// <summary>
        /// Path of a local file
        /// </summary>
        private string pathString;

        /// <summary>
        /// Temporary path for remote data store.
        /// </summary>
        private string remoteDataStorePath;

        #endregion

        #region Tests managment

        /// <summary>Initializes a new instance of the LocalDataStorageTests class.</summary>
        public RemoteFileClassTest()
        {
            pathString = Path.Combine(Path.GetTempPath(), "lmt-Testfile.txt");
            remoteDataStorePath = Path.Combine(Path.GetTempPath(), "RemoteDataStoreUnitTest");
        }

        /// <summary>Setups called before each test to initialize variables.</summary>
        [SetUp]
        public void Setup()
        {
            ConfigurationSettings.AppSettings["RemoteDataStoreUrl"] = remoteDataStorePath;
            if (!Directory.Exists(remoteDataStorePath))
            {
                Directory.CreateDirectory(remoteDataStorePath);
            }


            if (!System.IO.File.Exists(pathString))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(pathString))
                {
                    fs.WriteByte(1);
                }
            }
            else
            {
                Console.WriteLine("File \"{0}\" already exists.", pathString);
            }
            
        }

        /// <summary>Tear down called after each test to clean.</summary>
        [TearDown]
        public void TearDown()
        {
            // Do something after each tests

            if (File.Exists(pathString))
            {
                File.Delete(pathString);
            }

            if (Directory.Exists(remoteDataStorePath))
            {
                Directory.Delete(remoteDataStorePath, true);
            }
        }

        #endregion

        /// <summary>Test check url function</summary>
        [Test]
        public void CheckUrlTests()
        {
            //Test with local file
            Assert.True(RemoteFileClass.checkUrl(pathString), "Method RemoteFileClass.checkUrl failed on url '{0}'", pathString);
            //Test HTTP url
            string url = "http://www.google.com";
            Assert.True(RemoteFileClass.checkUrl(url), "Method RemoteFileClass.checkUrl failed on url '{0}'", url);
            //Test FTP url
            url = "ftp://10.95.38.17:2121/Dev/PISGROUND/Tmp/pisbase-testfile.txt";
            Assert.True(RemoteFileClass.checkUrl(url), "Method RemoteFileClass.checkUrl failed on url '{0}'", url);
            //Test invalid URL
            url = "Hello world";
            Assert.False(RemoteFileClass.checkUrl(url), "Method RemoteFileClass.checkUrl succeeded on url '{0}'", url);
        }

        /// <summary>Test constructor function</summary>
        [Test]
        public void ContructorTestsAndCrcCalculation()
        {
            System.IO.FileStream lStream;
            PIS.Ground.Core.Utility.Crc32 lCrcCalculator = new PIS.Ground.Core.Utility.Crc32();

            //Check constructor with local file
            RemoteFileClass rFile1 = new RemoteFileClass(pathString, true);

            try
            {
                lStream = new System.IO.FileStream(pathString, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                Assert.AreEqual(lCrcCalculator.CalculateChecksum(lStream), rFile1.CRC);
            }
            catch (Exception ex)
            {
                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.Data.RemoteFileClass", ex, EventIdEnum.GroundCore);
                lStream = null;
            }
            Assert.True(rFile1.Exists);
        }
    }
}