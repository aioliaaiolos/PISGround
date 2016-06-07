//---------------------------------------------------------------------------------------------------
// <copyright file="T2GConnectorTests.cs" company="Alstom">
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
using PIS.Ground.Core.Data;
using PIS.Ground.Core.T2G.WebServices.FileTransfer;

namespace GroundCoreTests
{
	/// <summary>T2GConnectorTests test class.</summary>
	[TestFixture, Category("T2G")]
	public class T2GConnectorTests
	{
		#region attributes

		/// <summary>The class to be tested.</summary>
		private T2GFileDistributionManager _t2gFileDistMgr;

        private Mock<PIS.Ground.Core.T2G.IT2GNotifierTarget> _notifierTargetMock;
		private Mock<PIS.Ground.Core.T2G.WebServices.FileTransfer.FileTransferPortType> _fileTransferPortMock;

		private DateTime _startTransferDate;
		private DateTime _endTransferDate;

		private PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList _transferTaskList;
		private PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct _transferTask1;
		private PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct _transferTask2;
		private PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct _transferTask3;
		private PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct _transferTask4;
		private PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct _transferTask5;
		private PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct _transferTask6;
		private PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct _transferTask7;
		private PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct _transferTask8;
		private PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutput _transferReply;

        private Mock<IRemoteFolderClass> _remoteFolderMock;
        private folderInfoStruct _testFolder;
        private enumFoldersOutput _enumFoldersOutput;
        private List<RecipientId> _recs = new List<RecipientId>();

		#endregion

		#region Tests managment

		/// <summary>Initializes a new instance of the T2GConnectorTests class.</summary>
		public T2GConnectorTests()
		{
			// Nothing Special
			_startTransferDate = new DateTime(2014, 3, 10, 17, 59, 10);
			_endTransferDate = new DateTime(2015, 5, 18, 13, 01, 34);
		}

		/// <summary>Setups called before each test to initialize variables.</summary>
		[SetUp]
		public void Setup()
		{
			_fileTransferPortMock = new Mock<PIS.Ground.Core.T2G.WebServices.FileTransfer.FileTransferPortType>();
            _notifierTargetMock = new Mock<PIS.Ground.Core.T2G.IT2GNotifierTarget>();
			_t2gFileDistMgr = new T2GFileDistributionManager(new T2GSessionData(), _notifierTargetMock.Object, _fileTransferPortMock.Object);
			_transferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			_transferTask1 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			_transferTask1.taskId = 67;
			_transferTask2 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			_transferTask2.taskId = 72;
			_transferTask3 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			_transferTask3.taskId = 98;
			_transferTask4 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			_transferTask4.taskId = 118;
			_transferTask5 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			_transferTask5.taskId = 128;
			_transferTask6 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			_transferTask6.taskId = 138;
			_transferTask7 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			_transferTask7.taskId = 148;
			_transferTask8 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			_transferTask8.taskId = 158;
			_transferReply = new PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutput();
			_transferReply.Body = new PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutputBody();
			_transferReply.Body.transferTaskList = null;
			_transferReply.Body.endOfEnum = true;

            //Build Mock folder for a request
            _remoteFolderMock = new Mock<IRemoteFolderClass>();

            //build Mock answer for enumFolders
            _testFolder = new folderInfoStruct();

            //Build fake answer to enumFolders function
            _enumFoldersOutput = new enumFoldersOutput();
            _enumFoldersOutput.Body = new enumFoldersOutputBody();
            _enumFoldersOutput.Body.endOfEnum = true;
            _enumFoldersOutput.Body.folderList = new folderList();
            _enumFoldersOutput.Body.folderList.Add(_testFolder);
		}

		/// <summary>Compare two task lists.</summary>
		/// <param name="listA">The first list.</param>
		/// <param name="listB">The second list.</param>
		/// <returns>true if identical, false otherwise.</returns>
		public bool CompareTaskLists(PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList listA,
			PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList listB)
		{
			bool equal = true;

			if (listA.Count == listB.Count)
			{
				int indexA = 0;
				int indexB = 0;

				for(; indexA < listA.Count; ++indexA, ++indexB)
				{
					// Only comparing the task id
					if (listA[indexA].taskId != listA[indexA].taskId)
					{
						equal = false;
						break;
					}
				}
			}
			else
			{
				equal = false;
			}

			return equal;
		}

		/// <summary>Tear down called after each test to clean.</summary>
		[TearDown]
		public void TearDown()
		{
			// Do something after each tests
		}

		#endregion

		#region EnumTransferTask

		/// <summary>Task list is null.</summary>
		[Test]
		public void EnumTransferTaskNoList()
		{
			_transferReply.Body.transferTaskList = null;
			_transferReply.Body.endOfEnum = true;

			_fileTransferPortMock.Setup(x => x.enumTransferTask(
				It.IsAny<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>()))
				.Returns(_transferReply);

			PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList taskListResult;
			bool lSuccess = _t2gFileDistMgr.EnumTransferTask(_startTransferDate, _endTransferDate, out taskListResult);

			_fileTransferPortMock.Verify(x => x.enumTransferTask(
				It.Is<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>
				(l => l.Body.startDate == _startTransferDate && l.Body.endDate == _endTransferDate)),
				Times.Once());

			Assert.AreEqual(0, taskListResult.Count);
			Assert.IsTrue(lSuccess);
		}

		/// <summary>Task list is empty.</summary>
		[Test]
		public void EnumTransferTaskEmptyList()
		{
			_transferReply.Body.transferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			_transferReply.Body.endOfEnum = true;

			_fileTransferPortMock.Setup(x => x.enumTransferTask(
				It.IsAny<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>()))
				.Returns(_transferReply);

			PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList taskListResult;
			bool lSuccess = _t2gFileDistMgr.EnumTransferTask(_startTransferDate, _endTransferDate, out taskListResult);

			_fileTransferPortMock.Verify(x => x.enumTransferTask(
				It.Is<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>
				(l => l.Body.startDate == _startTransferDate && l.Body.endDate == _endTransferDate)),
				Times.Once());

			Assert.AreEqual(0, taskListResult.Count);
			Assert.IsTrue(lSuccess);
		}

		/// <summary>Task list contains one entry.</summary>
		[Test]
		public void EnumTransferTaskOneEntry()
		{
			_transferReply.Body.transferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			_transferReply.Body.transferTaskList.Add(_transferTask1);
			_transferReply.Body.endOfEnum = true;

			_fileTransferPortMock.Setup(x => x.enumTransferTask(
				It.IsAny<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>()))
				.Returns(_transferReply);

			PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList taskListResult;
			bool lSuccess = _t2gFileDistMgr.EnumTransferTask(_startTransferDate, _endTransferDate, out taskListResult);

			_fileTransferPortMock.Verify(x => x.enumTransferTask(
				It.Is<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>
				(l => l.Body.startDate == _startTransferDate && l.Body.endDate == _endTransferDate)),
				Times.Once());

			Assert.IsTrue(CompareTaskLists(_transferReply.Body.transferTaskList, taskListResult));
			Assert.IsTrue(lSuccess);
		}

		/// <summary>Task list contains three entries.</summary>
		[Test]
		public void EnumTransferTaskThreeEntries()
		{
			_transferReply.Body.transferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			_transferReply.Body.transferTaskList.Add(_transferTask1);
			_transferReply.Body.transferTaskList.Add(_transferTask2);
			_transferReply.Body.transferTaskList.Add(_transferTask3);
			_transferReply.Body.endOfEnum = true;

			_fileTransferPortMock.Setup(x => x.enumTransferTask(
				It.IsAny<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>()))
				.Returns(_transferReply);

			PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList taskListResult;
			bool lSuccess = _t2gFileDistMgr.EnumTransferTask(_startTransferDate, _endTransferDate, out taskListResult);

			_fileTransferPortMock.Verify(x => x.enumTransferTask(
				It.Is<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>
				(l => l.Body.startDate == _startTransferDate && l.Body.endDate == _endTransferDate)),
				Times.Once());

			Assert.IsTrue(CompareTaskLists(_transferReply.Body.transferTaskList, taskListResult));
			Assert.IsTrue(lSuccess);
		}


		/// <summary>Task list contains four entries in two chunks.</summary>
		[Test]
		public void EnumTransferTaskThreeEntriesTwoChunks()
		{
			_transferReply.Body.transferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			_transferReply.Body.transferTaskList.Add(_transferTask1);
			_transferReply.Body.transferTaskList.Add(_transferTask2);
			_transferReply.Body.transferTaskList.Add(_transferTask3);
			_transferReply.Body.endOfEnum = false;

			var transferReplyPart2 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutput();
			transferReplyPart2.Body = new PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutputBody();
			transferReplyPart2.Body.transferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			transferReplyPart2.Body.transferTaskList.Add(_transferTask4);
			transferReplyPart2.Body.endOfEnum = true;

			var expectedTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			expectedTaskList.AddRange(_transferReply.Body.transferTaskList);
			expectedTaskList.AddRange(transferReplyPart2.Body.transferTaskList);

			var transferList = new List<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutput>();
			transferList.Add(_transferReply);
			transferList.Add(transferReplyPart2);
			ushort currentTransferListIndex = 0;

			_fileTransferPortMock.Setup(x => x.enumTransferTask(
				It.IsAny<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>()))
				.Throws(new Exception("Invalid invocation"));

			_fileTransferPortMock.Setup(x => x.enumTransferTask(
				It.Is<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>(
				t => t.Body.enumPos < transferList.Count && t.Body.enumPos == currentTransferListIndex && t.Body.startDate == _startTransferDate && t.Body.endDate == _endTransferDate)))
				.Returns(() => transferList[currentTransferListIndex++]);

			PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList taskListResult;
			bool lSuccess = _t2gFileDistMgr.EnumTransferTask(_startTransferDate, _endTransferDate, out taskListResult);

			_fileTransferPortMock.Verify(x => x.enumTransferTask(
				It.Is<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>
				(l => l.Body.startDate == _startTransferDate && l.Body.endDate == _endTransferDate)),
				Times.Exactly(2));

			Assert.IsTrue(lSuccess, "EnumTransferTask didn't succeed as expected");
			Assert.IsTrue(CompareTaskLists(expectedTaskList, taskListResult));
		}

		/// <summary>Task list contains height entries in three chunks.</summary>
		[Test]
		public void EnumTransferTaskThreeEntriesThreeChunks()
		{
			_transferReply.Body.transferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			_transferReply.Body.transferTaskList.Add(_transferTask1);
			_transferReply.Body.transferTaskList.Add(_transferTask2);
			_transferReply.Body.transferTaskList.Add(_transferTask3);
			_transferReply.Body.endOfEnum = false;

			var transferReplyPart2 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutput();
			transferReplyPart2.Body = new PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutputBody();
			transferReplyPart2.Body.transferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			transferReplyPart2.Body.transferTaskList.Add(_transferTask4);
			transferReplyPart2.Body.transferTaskList.Add(_transferTask5);
			transferReplyPart2.Body.transferTaskList.Add(_transferTask6);
			transferReplyPart2.Body.endOfEnum = false;

			var transferReplyPart3 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutput();
			transferReplyPart3.Body = new PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutputBody();
			transferReplyPart3.Body.transferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			transferReplyPart3.Body.transferTaskList.Add(_transferTask7);
			transferReplyPart3.Body.transferTaskList.Add(_transferTask8);
			transferReplyPart3.Body.endOfEnum = true;

			var expectedTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			expectedTaskList.AddRange(_transferReply.Body.transferTaskList);
			expectedTaskList.AddRange(transferReplyPart2.Body.transferTaskList);
			expectedTaskList.AddRange(transferReplyPart3.Body.transferTaskList);

			var transferList = new List<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutput>();
			transferList.Add(_transferReply);
			transferList.Add(transferReplyPart2);
			transferList.Add(transferReplyPart3);
			ushort currentTransferListIndex = 0;

			_fileTransferPortMock.Setup(x => x.enumTransferTask(
				It.IsAny<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>()))
				.Throws(new Exception("Invalid invocation"));

			_fileTransferPortMock.Setup(x => x.enumTransferTask(
				It.Is<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>(
				t => t.Body.enumPos < transferList.Count && t.Body.enumPos == currentTransferListIndex && t.Body.startDate == _startTransferDate && t.Body.endDate == _endTransferDate)))
				.Returns(() => transferList[currentTransferListIndex++]);

			PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList taskListResult;
			bool lSuccess = _t2gFileDistMgr.EnumTransferTask(_startTransferDate, _endTransferDate, out taskListResult);

			_fileTransferPortMock.Verify(x => x.enumTransferTask(
				It.Is<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>
				(l => l.Body.startDate == _startTransferDate && l.Body.endDate == _endTransferDate)),
				Times.Exactly(3));

			Assert.IsTrue(lSuccess, "EnumTransferTask didn't succeed as expected");
			Assert.IsTrue(CompareTaskLists(expectedTaskList, taskListResult));
		}


		/// <summary>Task list contains three entries in two chunks with the first chunk empty.</summary>
		[Test]
		public void EnumTransferTaskThreeEntriesEmptyFirstChunk()
		{
			_transferReply.Body.transferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			_transferReply.Body.endOfEnum = false;

			var transferReplyPart2 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutput();
			transferReplyPart2.Body = new PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutputBody();
			transferReplyPart2.Body.transferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			transferReplyPart2.Body.transferTaskList.Add(_transferTask1);
			transferReplyPart2.Body.transferTaskList.Add(_transferTask2);
			transferReplyPart2.Body.transferTaskList.Add(_transferTask3);
			transferReplyPart2.Body.endOfEnum = true;

			var expectedTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			expectedTaskList.AddRange(_transferReply.Body.transferTaskList);
			expectedTaskList.AddRange(transferReplyPart2.Body.transferTaskList);

			var transferList = new List<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskOutput>();
			transferList.Add(_transferReply);
			transferList.Add(transferReplyPart2);
			var currentTransferListIndex = 0;

			_fileTransferPortMock.Setup(x => x.enumTransferTask(
				It.IsAny<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>()))
				.Returns(() => transferList[currentTransferListIndex++]);

			PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList taskListResult;
			bool lSuccess = _t2gFileDistMgr.EnumTransferTask(_startTransferDate, _endTransferDate, out taskListResult);

			_fileTransferPortMock.Verify(x => x.enumTransferTask(
				It.Is<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>
				(l => l.Body.startDate == _startTransferDate && l.Body.endDate == _endTransferDate)),
				Times.Exactly(2));

			Assert.IsTrue(CompareTaskLists(expectedTaskList, taskListResult));
			Assert.IsTrue(lSuccess);
		}

		/// <summary>Error condition.</summary>
		[Test]
		public void EnumTransferTaskErrorCondition()
		{
			_transferReply.Body.transferTaskList = null;
			_transferReply.Body.endOfEnum = true;

			_fileTransferPortMock.Setup(x => x.enumTransferTask(
				It.IsAny<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>()))
				.Throws(new Exception("Error during request"));

			PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList taskListResult;
			bool lSuccess = _t2gFileDistMgr.EnumTransferTask(_startTransferDate, _endTransferDate, out taskListResult);

			_fileTransferPortMock.Verify(x => x.enumTransferTask(
				It.Is<PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput>
				(l => l.Body.startDate == _startTransferDate && l.Body.endDate == _endTransferDate && l.Body.enumPos == 0)),
				Times.Once());

			Assert.AreEqual(0, taskListResult.Count);
			Assert.IsFalse(lSuccess);
		}

		#endregion

        #region GetFolderIdFromSimilarT2GFolder

        /// <summary>first request and no upload folder on T2G server</summary>
        [Test]
        public void GetFolderIdFromSimilarT2GFolderNoFolderOnT2GServer()
        {
            Guid lRequestId = Guid.NewGuid();
            string lCRCGuid = "1111-2222-3333-4444";

            _remoteFolderMock.Setup(x => x.CRCGuid).Returns(lCRCGuid);
            _remoteFolderMock.Setup(x => x.FolderName).Returns(lRequestId.ToString() + lCRCGuid);

            UploadFileDistributionRequest lUploadFileRequest = new UploadFileDistributionRequest(lRequestId, _remoteFolderMock.Object, new DateTime(2099, 12, 31), false, _recs, DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);

            _t2gFileDistMgr.GetFolderIdFromSimilarT2GFolder(ref lUploadFileRequest);

            Assert.AreEqual(0, lUploadFileRequest.Folder.FolderId);
        }

        /// <summary>An upload folder on T2G server exists and is not usable because of different CRCGuid (expiration date is correct and acquisition state is a success</summary>
        [Test]
        public void GetFolderIdFromSimilarT2GFolderNotSameCRCGuidOnT2GServer()
        {
            Guid lRequestId = Guid.NewGuid();
            string lCRCGuid = "1111-2222-3333-4444";

            //Build fake folder on T2G Server that will be answered by EnumFolders function
            _testFolder.acquisitionState = acquisitionStateEnum.acquisitionSuccess;
            _testFolder.expirationDate = new DateTime(3099, 12, 31);
            _testFolder.folderId = 1;
            _testFolder.name = Guid.NewGuid().ToString() + "|5555-6666-7777-8888";

            _fileTransferPortMock.Setup(x => x.enumFolders(It.IsAny<enumFoldersInput>())).Returns(_enumFoldersOutput);

            //Build folder to use as a mock in the UploadFileDistributionRequest
            _remoteFolderMock.Setup(x => x.CRCGuid).Returns(lCRCGuid);
            _remoteFolderMock.Setup(x => x.FolderName).Returns(lRequestId.ToString() + lCRCGuid);
            _remoteFolderMock.SetupProperty(x => x.FolderId);
            _remoteFolderMock.SetupProperty(x => x.ExpirationDate);

            UploadFileDistributionRequest lUploadFileRequest = new UploadFileDistributionRequest(lRequestId, _remoteFolderMock.Object, new DateTime(2099, 12, 31), false, _recs, DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);

            _t2gFileDistMgr.GetFolderIdFromSimilarT2GFolder(ref lUploadFileRequest);

            Assert.AreEqual(0, lUploadFileRequest.Folder.FolderId);
        }


        /// <summary>An upload folder on T2G server exists but is not usable because of its expiration date</summary>
        [Test]
        public void GetFolderIdFromSimilarT2GFolderInvalidExpirationDateOnT2GServer()
        {
            Guid lRequestId = Guid.NewGuid();
            string lCRCGuid = "1111-2222-3333-4444";

            //Build fake folder on T2G Server that will be answered by EnumFolders function
            _testFolder.acquisitionState = acquisitionStateEnum.acquisitionSuccess;
            _testFolder.expirationDate = new DateTime(2000,12,31);
            _testFolder.folderId = 1;
            _testFolder.name = Guid.NewGuid().ToString() + "|" + lCRCGuid;

            _fileTransferPortMock.Setup(x => x.enumFolders(It.IsAny<enumFoldersInput>())).Returns(_enumFoldersOutput);

            //Build folder to use as a mock in the UploadFileDistributionRequest
            _remoteFolderMock.Setup(x => x.CRCGuid).Returns(lCRCGuid);
            _remoteFolderMock.Setup(x => x.FolderName).Returns(lRequestId.ToString() + lCRCGuid);
            _remoteFolderMock.SetupProperty(x => x.FolderId);
            _remoteFolderMock.SetupProperty(x => x.ExpirationDate);

            UploadFileDistributionRequest lUploadFileRequest = new UploadFileDistributionRequest(lRequestId, _remoteFolderMock.Object, new DateTime(2099, 12, 31), false, _recs, DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);

            _t2gFileDistMgr.GetFolderIdFromSimilarT2GFolder(ref lUploadFileRequest);

            Assert.AreEqual(0, lUploadFileRequest.Folder.FolderId);
        }

        /// <summary>An upload folder on T2G server exists but is not usable because of its acquisition state</summary>
        [Test]
        public void GetFolderIdFromSimilarT2GFolderAcquisitionStateNotSuccessOnT2GServer()
        {
            Guid lRequestId = Guid.NewGuid();
            string lCRCGuid = "1111-2222-3333-4444";

            //Build fake folder on T2G Server that will be answered by EnumFolders function
            _testFolder.acquisitionState = acquisitionStateEnum.acquisitionError;
            _testFolder.expirationDate = new DateTime(3099, 12, 31);
            _testFolder.folderId = 1;
            _testFolder.name = Guid.NewGuid().ToString() + "|" + lCRCGuid;

            _fileTransferPortMock.Setup(x => x.enumFolders(It.IsAny<enumFoldersInput>())).Returns(_enumFoldersOutput);

            //Build folder to use as a mock in the UploadFileDistributionRequest
            _remoteFolderMock.Setup(x => x.CRCGuid).Returns(lCRCGuid);
            _remoteFolderMock.Setup(x => x.FolderName).Returns(lRequestId.ToString() + lCRCGuid);
            _remoteFolderMock.SetupProperty(x => x.FolderId);
            _remoteFolderMock.SetupProperty(x => x.ExpirationDate);

            UploadFileDistributionRequest lUploadFileRequest = new UploadFileDistributionRequest(lRequestId, _remoteFolderMock.Object, new DateTime(2099, 12, 31), false, _recs, DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);

            _t2gFileDistMgr.GetFolderIdFromSimilarT2GFolder(ref lUploadFileRequest);

            Assert.AreEqual(0, lUploadFileRequest.Folder.FolderId);
        }

        /// <summary>An upload folder on T2G server exists and is usable</summary>
        [Test]
        public void GetFolderIdFromSimilarT2GFolderUsableOnT2GServer()
        {
            Guid lRequestId = Guid.NewGuid();
            string lCRCGuid = "1111-2222-3333-4444";

            //Build fake folder on T2G Server that will be answered by EnumFolders function
            _testFolder.acquisitionState = acquisitionStateEnum.acquisitionSuccess;
            _testFolder.expirationDate = new DateTime(3099, 12, 31);
            _testFolder.folderId = 1;
            _testFolder.name = Guid.NewGuid().ToString() + "|" + lCRCGuid;

            _fileTransferPortMock.Setup(x => x.enumFolders(It.IsAny<enumFoldersInput>())).Returns(_enumFoldersOutput);

            //Build folder to use as a mock in the UploadFileDistributionRequest
            _remoteFolderMock.Setup(x => x.CRCGuid).Returns(lCRCGuid);
            _remoteFolderMock.Setup(x => x.FolderName).Returns(lRequestId.ToString() + lCRCGuid);
            _remoteFolderMock.SetupProperty(x => x.FolderId);
            _remoteFolderMock.SetupProperty(x => x.ExpirationDate);

            UploadFileDistributionRequest lUploadFileRequest = new UploadFileDistributionRequest(lRequestId, _remoteFolderMock.Object, new DateTime(2099, 12, 31), false, _recs, DateTime.Now, "", FileTransferMode.AnyBandwidth, 1, null);

            _t2gFileDistMgr.GetFolderIdFromSimilarT2GFolder(ref lUploadFileRequest);

            Assert.AreEqual(1, lUploadFileRequest.Folder.FolderId);
        }


        #endregion
    }
}