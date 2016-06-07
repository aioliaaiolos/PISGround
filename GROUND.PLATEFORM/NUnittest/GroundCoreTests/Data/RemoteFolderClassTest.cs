//---------------------------------------------------------------------------------------------------
// <copyright file="RemoteFolderClassTest.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using NUnit.Framework;
using PIS.Ground.Core.Data;
using Moq;

namespace GroundCoreTests.Data
{
    /// <summary>RemoteFolderClassTest test class.</summary>
    [TestFixture]
    public class RemoteFolderClassTest
    {
        #region attributes

        /// <summary>
        /// Remote folder object
        /// </summary>
        public IRemoteFolderClass lRemoteFolder;

        /// <summary>
        /// List of mocked IRemoteFileClass
        /// </summary>
        private List<Mock<IRemoteFileClass>> _remoteFileMockList;

        /// <summary>
        /// Number or Mock RemoteFileClass required to be created
        /// </summary>
        private int _nbFiles = 4;

        #endregion

        #region Tests managment

        /// <summary>Initializes a new instance of the LocalDataStorageTests class.</summary>
        public RemoteFolderClassTest()
        {
            _remoteFileMockList = new List<Mock<IRemoteFileClass>>();
        }

        /// <summary>Setups called before each test to initialize variables.</summary>
        [SetUp]
        public void Setup()
        {
            ConfigurationSettings.AppSettings["RemoteDataStoreUrl"] = "c:/RemoteDataStore";

            lRemoteFolder = new RemoteFolderClass("FolderName", 0);

            //Create Mocks
            for (int i = 0; i < _nbFiles; i++)
            {
                _remoteFileMockList.Add(new Mock<IRemoteFileClass>());
                _remoteFileMockList[i].Setup(x => x.FileName).Returns("Filename" + i.ToString());
                _remoteFileMockList[i].Setup(x => x.CRC).Returns(i.ToString());
                _remoteFileMockList[i].Setup(x => x.Size).Returns(1);
                _remoteFileMockList[i].Setup<int>(x => x.CompareTo(It.IsAny<IRemoteFileClass>())).Returns(0);
            }
        }

        /// <summary>Tear down called after each test to clean.</summary>
        [TearDown]
        public void TearDown()
        {
            // Do something after each tests
            _remoteFileMockList.Clear();
        }

        #endregion

        /// <summary>Test class the simpliest constructor.</summary>
        [Test]
        public void TestConstructor()
        {
            //Check information are correct.
            Assert.AreEqual("FolderName", lRemoteFolder.FolderName);
            Assert.AreEqual(0, lRemoteFolder.TotalFilesSize);
            Assert.IsEmpty(lRemoteFolder.CRCGuid);
            Assert.IsEmpty(lRemoteFolder.FilenameList);
            Assert.IsEmpty(lRemoteFolder.FolderFilesList);
        }

        /// <summary>
        /// Check RemoteFolderClass creation with files in it
        /// </summary>
        [Test]
        public void TestAddingRemoteFiles()
        {
            foreach (Mock<IRemoteFileClass> remoteFileMock in _remoteFileMockList)
            {
                lRemoteFolder.AddFileToFolder(remoteFileMock.Object);
            }

            lRemoteFolder.CalculateCRCGuid();

            Assert.AreEqual(4, lRemoteFolder.TotalFilesSize);

            foreach (Mock<IRemoteFileClass> remoteFileMock in _remoteFileMockList)
            {
                Assert.Contains(remoteFileMock.Object.FileName, lRemoteFolder.FilenameList);
                Assert.Contains(remoteFileMock.Object, lRemoteFolder.FolderFilesList);
            }
        }

        /// <summary>
        /// Check RemoteFolderClass creation with copy constructor
        /// </summary>
        [Test]
        public void TestFolderValidity()
        {
            IRemoteFolderClass lRemoteFolder2 = new RemoteFolderClass("CopyTest", 1);
            foreach (Mock<IRemoteFileClass> remoteFileMock in _remoteFileMockList)
            {
                lRemoteFolder2.AddFileToFolder(remoteFileMock.Object);
            }

            lRemoteFolder2.CalculateCRCGuid();

            Assert.AreEqual(4, lRemoteFolder2.TotalFilesSize);

            foreach (Mock<IRemoteFileClass> remoteFileMock in _remoteFileMockList)
            {
                Assert.Contains(remoteFileMock.Object.FileName, lRemoteFolder2.FilenameList);
                Assert.Contains(remoteFileMock.Object, lRemoteFolder2.FolderFilesList);
            }

            Assert.AreEqual(4, lRemoteFolder2.TotalFilesSize);

            lRemoteFolder = new RemoteFolderClass((RemoteFolderClass)lRemoteFolder2);

            lRemoteFolder.CalculateCRCGuid();

            Assert.AreEqual(4, lRemoteFolder.TotalFilesSize);
            //Assert.AreEqual(lRemoteFolder2.CRCGuid, lRemoteFolder.CRCGuid);

            foreach (Mock<IRemoteFileClass> remoteFileMock in _remoteFileMockList)
            {
                Assert.Contains(remoteFileMock.Object.FileName, lRemoteFolder.FilenameList);
                Assert.Contains(remoteFileMock.Object, lRemoteFolder.FolderFilesList);
            }
        }
    }
}