//---------------------------------------------------------------------------------------------------
// <copyright file="DataPackageServiceTest.cs" company="Alstom">
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
using PIS.Ground.Core.Data;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.DataPackage;

using System.Reflection;

namespace DataPackageTests
{
	/// <summary>Data Package service test class.</summary>
	[TestFixture]
	public class DataPackageServiceTests
	{
		#region attributes 

        /// <summary>The train 2ground client mock.</summary>
        private Mock<IT2GFileDistributionManager> _train2groundClientMock;

		/// <summary>The train 2ground manager mock.</summary>
		private Mock<IT2GManager> _train2groundManagerMock;

		#endregion

		#region Tests managment


		private class DataPackageServiceInstrumented : DataPackageService
		{
			public static PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList
				findPendingTransferTasks(PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList inputTasksList)
			{
				MethodInfo lMethodInfo = typeof(DataPackageService).GetMethod("findPendingTransferTasks",
					BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

				object value = lMethodInfo.Invoke(null, new object[] { inputTasksList });

				return (PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList)value;
			}

			public static bool cancelTransferTasks(List<DataPackageService.CancellableTransferTaskInfo> tasksList)
			{
				MethodInfo lMethodInfo = typeof(DataPackageService).GetMethod("cancelTransferTasks",
					BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

				object value = lMethodInfo.Invoke(null, new object[] { tasksList });

				return (bool)value;
			}

			public static bool getCancellableTransferTasks(out List<CancellableTransferTaskInfo> tasks)
			{
				MethodInfo lMethodInfo = typeof(DataPackageService).GetMethod("getCancellableTransferTasks",
					BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

				object[] lParameters = new object[] { null };
				object value = lMethodInfo.Invoke(null, lParameters);
				tasks = (List<CancellableTransferTaskInfo>)lParameters[0];
				return (bool)value;
			}
		}

		/// <summary>Initializes a new instance of the DataPackageServiceTests class.</summary>
		public DataPackageServiceTests()
		{
			// Nothing to do
		}

		/// <summary>Setups called before each test to initialize variables.</summary>
		[SetUp]
		public void Setup()
		{
			_train2groundClientMock = new Mock<IT2GFileDistributionManager>();

            _train2groundManagerMock = new Mock<IT2GManager>();
            _train2groundManagerMock.SetupGet(x => x.T2GFileDistributionManager).Returns(_train2groundClientMock.Object);

			typeof(DataPackageService).GetField("_t2gManager", 
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
				.SetValue(null, _train2groundManagerMock.Object);
		}

		/// <summary>Tear down called after each test to clean.</summary>
		[TearDown]
		public void TearDown()
		{
			// Do something after each tests
		}

		#endregion

		#region findPendingTransferTasks

		/// <summary>Searches for the first pending transfer tasks.</summary>
		[Test]
		public void findPendingTransferTasks()
		{
			PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList lInputList;
			PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList lOutputList;
			var lTask1 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			var lTask2 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			var lTask3 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			var lTask4 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();

			lInputList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			lOutputList = DataPackageServiceInstrumented.findPendingTransferTasks(lInputList);
			Assert.IsEmpty(lOutputList, "Input list is empty");

			lInputList.Add(null);
			lOutputList = DataPackageServiceInstrumented.findPendingTransferTasks(lInputList);
			Assert.IsEmpty(lOutputList, "Input list contains a null element");

			lTask1.taskState = PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskCancelled;
			lInputList.Add(lTask1);
			lTask2.taskState = PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskCompleted;
			lInputList.Add(lTask2);
			lOutputList = DataPackageServiceInstrumented.findPendingTransferTasks(lInputList);
			Assert.IsEmpty(lOutputList, "Input list contains only non pending elements");

			lTask3.taskState = PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskStarted;
			lInputList.Add(lTask3);
			lOutputList = DataPackageServiceInstrumented.findPendingTransferTasks(lInputList);
			Assert.AreEqual(1, lOutputList.Count, "Input list contains one pending element");
			Assert.AreEqual(lTask3, lOutputList[0], "Input list contains one pending element");

			lTask4.taskState = PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskCreated;
			lInputList.Insert(1, lTask4);
			lOutputList = DataPackageServiceInstrumented.findPendingTransferTasks(lInputList);
			Assert.AreEqual(2, lOutputList.Count, "Input list contains two pending element");
			Assert.AreEqual(lTask4, lOutputList[0], "Input list contains two pending element");
			Assert.AreEqual(lTask3, lOutputList[1], "Input list contains two pending element");
		}

		#region cancelTransferTasks

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void cancelTransferTasks_emptyList()
		{
			var lInputList = new List<DataPackageService.CancellableTransferTaskInfo>();
			bool lSuccess = false;
			string lErrorMessage;


			lSuccess = DataPackageServiceInstrumented.cancelTransferTasks(lInputList);

			_train2groundClientMock.Verify(x => x.CancelTransferTask(
				It.IsAny<int>(), out lErrorMessage),
				Times.Never(), "Input list is empty");

			Assert.IsTrue(lSuccess, "Input list is empty");
		}

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void cancelTransferTasks_oneNullTask()
		{
			var lInputList = new List<DataPackageService.CancellableTransferTaskInfo>();
			lInputList.Add(null);
			bool lSuccess = false;
			string lErrorMessage = string.Empty;

			lSuccess = DataPackageServiceInstrumented.cancelTransferTasks(lInputList);

			_train2groundClientMock.Verify(x => x.CancelTransferTask(
				It.IsAny<int>(), out lErrorMessage),
				Times.Never(), "Input list contains a null element");

			Assert.IsTrue(lSuccess, "Input list contains a null element");
		}

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void cancelTransferTasks_oneTask()
		{
			int lTaskID1 = 364;
			var lInputList = new List<DataPackageService.CancellableTransferTaskInfo>();
			var lTask1 = new DataPackageService.CancellableTransferTaskInfo(lTaskID1, null);
			lInputList.Add(lTask1);
			bool lSuccess = false;
			string lErrorMessage = string.Empty;

			_train2groundClientMock.Setup(x => x.CancelTransferTask(
				It.IsAny<int>(), out lErrorMessage));

			lSuccess = DataPackageServiceInstrumented.cancelTransferTasks(lInputList);

			_train2groundClientMock.Verify(x => x.CancelTransferTask(
				It.Is<int>((lVal) => lVal == lTaskID1), out lErrorMessage),
				Times.Once(), "Input list contains one element");

			Assert.IsTrue(lSuccess, "Input list contains one element");
		}

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void cancelTransferTasks_multipleTasks()
		{
			int[] lTaskIDs = { 364, 547, 786 };
			var lInputList = new List<DataPackageService.CancellableTransferTaskInfo>();
			var lTask1 = new DataPackageService.CancellableTransferTaskInfo(lTaskIDs[0], null);
			lInputList.Add(lTask1);
			var lTask2 = new DataPackageService.CancellableTransferTaskInfo(lTaskIDs[1], null);
			lInputList.Add(lTask2);
			var lTask3 = new DataPackageService.CancellableTransferTaskInfo(lTaskIDs[2], null);
			lInputList.Add(lTask3);

			bool lSuccess = false;
			string lErrorMessage = string.Empty;

			_train2groundClientMock.Setup(x => x.CancelTransferTask(
				It.IsAny<int>(), out lErrorMessage));

			lSuccess = DataPackageServiceInstrumented.cancelTransferTasks(lInputList);

			_train2groundClientMock.Verify(x => x.CancelTransferTask(
				It.Is<int>((lVal) => lVal == lTaskIDs[0]), out lErrorMessage),
				Times.Exactly(1), "Input list contains multiple elements 1/3");

			_train2groundClientMock.Verify(x => x.CancelTransferTask(
				It.Is<int>((lVal) => lVal == lTaskIDs[1]), out lErrorMessage),
				Times.Exactly(1), "Input list contains multiple elements 2/3");

			_train2groundClientMock.Verify(x => x.CancelTransferTask(
				It.Is<int>((lVal) => lVal == lTaskIDs[2]), out lErrorMessage),
				Times.Exactly(1), "Input list contains multiple elements 3/3");

			Assert.IsTrue(lSuccess, "Input list contains multiple elements");
		}

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void cancelTransferTasks_error()
		{
			int[] lTaskIDs = { 364, 547 };
			var lInputList = new List<DataPackageService.CancellableTransferTaskInfo>();
			var lTask1 = new DataPackageService.CancellableTransferTaskInfo(lTaskIDs[0], null);
			lInputList.Add(lTask1);
			var lTask2 = new DataPackageService.CancellableTransferTaskInfo(lTaskIDs[1], null);
			lInputList.Add(lTask2);

			bool lSuccess = false;
			string lErrorMessage = "Cancel error";

			_train2groundClientMock.Setup(x => x.CancelTransferTask(
				It.IsAny<int>(), out lErrorMessage));

			lSuccess = DataPackageServiceInstrumented.cancelTransferTasks(lInputList);

			_train2groundClientMock.Verify(x => x.CancelTransferTask(
				It.Is<int>((lVal) => lVal == lTaskIDs[0]), out lErrorMessage),
				Times.Exactly(1), "Error during cancel 1/2");

			_train2groundClientMock.Verify(x => x.CancelTransferTask(
				It.Is<int>((lVal) => lVal == lTaskIDs[1]), out lErrorMessage),
				Times.Exactly(1), "Error during cancel 2/2");

			Assert.IsFalse(lSuccess, "Error during cancel");
		}

		#endregion

		#region CancellableTransferTaskInfo

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void CancellableTransferTaskInfo_IsMatching_emptyLists()
		{
			var lTemplate = new DataPackageService.CancellableTransferTaskInfo(514, null);
			var lTaskInfo = new DataPackageService.CancellableTransferTaskInfo(718, null);

			bool lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Empty transfer task elements lists 1/6");

			lTemplate.Elements = null;
			lTaskInfo.Elements = new List<string>();
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Empty transfer task elements lists 2/6");

			lTemplate.Elements = new List<string>();
			lTaskInfo.Elements = null;
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Empty transfer task elements lists 3/6");

			lTemplate.Elements = new List<string>();
			lTaskInfo.Elements = new List<string>();
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Empty transfer task elements lists 4/6");

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train1", "train2" });
			lTaskInfo.Elements = new List<string>();
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Empty transfer task elements lists 5/6");

			lTemplate.Elements = new List<string>();
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train15", "train16", "train17" });
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Empty transfer task elements lists 6/6");
		}


		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void CancellableTransferTaskInfo_IsMatching_completeMismatch()
		{
			var lTemplate = new DataPackageService.CancellableTransferTaskInfo(514, null);
			var lTaskInfo = new DataPackageService.CancellableTransferTaskInfo(718, null);

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train1" });
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train15" });
			bool lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Complete mismatch of elements lists 1/3");

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train1", "train2" });
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train15" });
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Complete mismatch of elements lists 2/3");

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train1", "train2" });
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train15", "train16", "train17" });
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Complete mismatch of elements lists 3/3");

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train1", "train2" });
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train15", "train1" });
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Parial mismatch of elements lists 1/3");

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train1", "train2" });
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train15", "train2", "train1" });
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Parial mismatch of elements lists 2/3");

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train1", "train2" });
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train1", "train2", "train17" });
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Parial mismatch of elements lists 3/3");
		}

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void CancellableTransferTaskInfo_IsMatching_partialMismatch()
		{
			var lTemplate = new DataPackageService.CancellableTransferTaskInfo(514, null);
			var lTaskInfo = new DataPackageService.CancellableTransferTaskInfo(718, null);

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train1", "train2" });
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train15", "train1" });
			bool lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Parial mismatch of elements lists 1/3");

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train1", "train2" });
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train15", "train2", "train1" });
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Parial mismatch of elements lists 2/3");

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train1", "train2" });
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train1", "train2", "train17" });
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsFalse(lMatching, "Parial mismatch of elements lists 3/3");
		}

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void CancellableTransferTaskInfo_IsMatching_Match()
		{
			var lTemplate = new DataPackageService.CancellableTransferTaskInfo(514, null);
			var lTaskInfo = new DataPackageService.CancellableTransferTaskInfo(718, null);

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train1" });
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train1" });
			bool lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsTrue(lMatching, "Match of elements lists 1/4");

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train1", "train2" });
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train1" });
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsTrue(lMatching, "Match of elements lists 2/4");

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train1", "train2" });
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train2", "train1" });
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsTrue(lMatching, "Match of elements lists 3/4");

			lTemplate.Elements = new List<string>();
			lTemplate.Elements.AddRange(new string[] { "train3", "train1", "train2" });
			lTaskInfo.Elements = new List<string>();
			lTaskInfo.Elements.AddRange(new string[] { "train2", "train1" });
			lMatching = lTaskInfo.IsMatching(lTemplate);
			Assert.IsTrue(lMatching, "Match of elements lists 4/4");
		}

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void CancellableTransferTaskInfo_IsMatching_asPredicate()
		{
			var lTemplate = new DataPackageService.CancellableTransferTaskInfo(514, null);
			lTemplate.Elements = new List<string>(new string[] { "train1", "train2" });
			var lTaskInfoList = new List<DataPackageService.CancellableTransferTaskInfo>();
			var lTaskInfo1 = new DataPackageService.CancellableTransferTaskInfo(718, null);
			lTaskInfo1.Elements = new List<string>(new string[] { "train2" });
			lTaskInfoList.Add(lTaskInfo1);
			var lTaskInfo2 = new DataPackageService.CancellableTransferTaskInfo(465, null);
			lTaskInfo2.Elements = new List<string>(new string[] { "train15" });
			lTaskInfoList.Add(lTaskInfo2);
			var lTaskInfo3 = new DataPackageService.CancellableTransferTaskInfo(112, null);
			lTaskInfo3.Elements = new List<string>(new string[] { "train2", "train1" });
			lTaskInfoList.Add(lTaskInfo3);

			var lMatchList = lTaskInfoList.FindAll((lTask) => lTask.IsMatching(lTemplate));

			Assert.AreEqual(2, lMatchList.Count, "Testing as a predicate. Checking the number of matches");
			Assert.IsTrue(718 == lMatchList[0].TaskID, "Testing as a predicate. Checking the first match");
			Assert.IsTrue(112 == lMatchList[1].TaskID, "Testing as a predicate. Checking the second match");
		}

		#endregion

		#region GetCancellableTransferTasks

		class FileDistributionRequestForTest : PIS.Ground.Core.Data.FileDistributionRequest
		{
		}

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void GetCancellableTransferTasks_nominalCase()
		{
			var lApplicationId = System.Configuration.ConfigurationSettings.AppSettings["ApplicationId"];
			var lInputTransferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			var lInputTasks1 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			lInputTasks1.taskId = 65;
			lInputTasks1.taskState = PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskCancelled;
			lInputTransferTaskList.Add(lInputTasks1);
			var lInputTasks2 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			lInputTasks2.taskId = 172;
			lInputTasks2.taskState = PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskCreated;
			lInputTransferTaskList.Add(lInputTasks2);
			var lInputTasks3 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			lInputTasks3.taskId = 34;
			lInputTasks3.taskState = PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskStarted;
			lInputTransferTaskList.Add(lInputTasks3);

			var lInputDistributionRequest1 = new FileDistributionRequestForTest();
			var lInputRecipient1 = new RecipientId();
			lInputRecipient1.SystemId = "train1";
			lInputRecipient1.ApplicationId = lApplicationId;
			var lInputRecipient2 = new RecipientId();
			lInputRecipient2.SystemId = "train2";
			lInputRecipient2.ApplicationId = lApplicationId;
			var lInputRecipient3 = new RecipientId();
			lInputRecipient3.SystemId = "train3";
			lInputRecipient3.ApplicationId = lApplicationId;
			var lInputRecipient4 = new RecipientId();
			lInputRecipient4.SystemId = "train4";
			lInputRecipient4.ApplicationId = "application xxx"; // Forcing mismatch

			lInputDistributionRequest1.RecipientList.AddRange(
				new RecipientId[] { lInputRecipient1, lInputRecipient2, lInputRecipient3, lInputRecipient4 });

			var lInputUnrecordedRecipients = new List<Recipient>();
			var lInputUnrecordedRecipient1 = new Recipient();
			lInputUnrecordedRecipient1.SystemId = "train15";
			lInputUnrecordedRecipient1.ApplicationIds = lApplicationId;
			lInputUnrecordedRecipients.Add(lInputUnrecordedRecipient1);
			var lInputUnrecordedRecipient2 = new Recipient();
			lInputUnrecordedRecipient2.SystemId = "train16";
			lInputUnrecordedRecipient2.ApplicationIds = "application yyy"; // Forcing mismatch
			lInputUnrecordedRecipients.Add(lInputUnrecordedRecipient2);
			var lInputUnrecordedRecipient3 = new Recipient();
			lInputUnrecordedRecipient3.SystemId = "train17";
			lInputUnrecordedRecipient3.ApplicationIds = lApplicationId;
			lInputUnrecordedRecipients.Add(lInputUnrecordedRecipient3);

			var lInputUnrecordedTransferTask = new TransferTaskData();

			_train2groundClientMock.Setup(x => x.EnumTransferTask(
				It.IsAny<DateTime>(), It.IsAny<DateTime>(), out lInputTransferTaskList))
					.Returns(true);

			_train2groundClientMock.Setup(x => x.GetFileDistributionRequestByTaskId(
				It.Is<int>((lValue) => lValue == 172)))
					.Returns<PIS.Ground.Core.Data.FileDistributionRequest>(null);

			_train2groundClientMock.Setup(x => x.GetFileDistributionRequestByTaskId(
				It.Is<int>((lValue) => lValue == 34)))
					.Returns(lInputDistributionRequest1);

			_train2groundClientMock.Setup(x => x.GetTransferTask(
				It.Is<int>((lValue) => lValue == 172), out lInputUnrecordedRecipients, out lInputUnrecordedTransferTask))
					.Returns("");

			List<DataPackageService.CancellableTransferTaskInfo> lOutputTransferTask;
			bool lSuccess = DataPackageServiceInstrumented.getCancellableTransferTasks(out lOutputTransferTask);

			_train2groundClientMock.Verify(x => x.GetTransferTask(
				It.IsAny<int>(), out lInputUnrecordedRecipients, out lInputUnrecordedTransferTask),
				Times.Once());

			Assert.AreEqual(2, lOutputTransferTask.Count, "Number of expected transfer task results");
			Assert.AreEqual(34, lOutputTransferTask[0].TaskID, "Recorded task id");
			Assert.AreEqual(3, lOutputTransferTask[0].Elements.Count, "Number of elements");
			Assert.AreEqual("train1", lOutputTransferTask[0].Elements[0], "First element");
			Assert.AreEqual("train2", lOutputTransferTask[0].Elements[1], "Second element");
			Assert.AreEqual("train3", lOutputTransferTask[0].Elements[2], "Third element");
			Assert.AreEqual(172, lOutputTransferTask[1].TaskID, "Unrecorded task id");
			Assert.AreEqual(2, lOutputTransferTask[1].Elements.Count, "Number of elements");
			Assert.AreEqual("train15", lOutputTransferTask[1].Elements[0], "First element");
			Assert.AreEqual("train17", lOutputTransferTask[1].Elements[1], "Second element");
			Assert.IsTrue(lSuccess);
		}

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void GetCancellableTransferTasks_emptyLists()
		{
			var lInputTransferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();

			var lInputUnrecordedRecipients = new List<Recipient>();
			var lInputUnrecordedTransferTask = new TransferTaskData();

			_train2groundClientMock.Setup(x => x.EnumTransferTask(
				It.IsAny<DateTime>(), It.IsAny<DateTime>(), out lInputTransferTaskList))
					.Returns(true);

			List<DataPackageService.CancellableTransferTaskInfo> lOutputTransferTask;
			bool lSuccess = DataPackageServiceInstrumented.getCancellableTransferTasks(out lOutputTransferTask);

			_train2groundClientMock.Verify(x => x.GetTransferTask(
				It.IsAny<int>(), out lInputUnrecordedRecipients, out lInputUnrecordedTransferTask),
				Times.Never());

			Assert.AreEqual(0, lOutputTransferTask.Count, "Number of expected transfer task results");
			Assert.IsTrue(lSuccess);

			_train2groundClientMock.Setup(x => x.EnumTransferTask(
				It.IsAny<DateTime>(), It.IsAny<DateTime>(), out lInputTransferTaskList))
					.Returns(false);

			lSuccess = DataPackageServiceInstrumented.getCancellableTransferTasks(out lOutputTransferTask);

			_train2groundClientMock.Verify(x => x.GetTransferTask(
				It.IsAny<int>(), out lInputUnrecordedRecipients, out lInputUnrecordedTransferTask),
				Times.Never());

			Assert.AreEqual(0, lOutputTransferTask.Count, "Number of expected transfer task results");
			Assert.IsFalse(lSuccess);
		}

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void GetCancellableTransferTasks_noRecordedTasks()
		{
			var lApplicationId = System.Configuration.ConfigurationSettings.AppSettings["ApplicationId"];
			var lInputTransferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			var lInputTasks2 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			lInputTasks2.taskId = 172;
			lInputTasks2.taskState = PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskCreated;
			lInputTransferTaskList.Add(lInputTasks2);

			var lInputUnrecordedRecipients = new List<Recipient>();
			var lInputUnrecordedRecipient1 = new Recipient();
			lInputUnrecordedRecipient1.SystemId = "train15";
			lInputUnrecordedRecipient1.ApplicationIds = lApplicationId;
			lInputUnrecordedRecipients.Add(lInputUnrecordedRecipient1);

			var lInputUnrecordedTransferTask = new TransferTaskData();

			_train2groundClientMock.Setup(x => x.EnumTransferTask(
				It.IsAny<DateTime>(), It.IsAny<DateTime>(), out lInputTransferTaskList))
					.Returns(true);

			_train2groundClientMock.Setup(x => x.GetFileDistributionRequestByTaskId(
				It.Is<int>((lValue) => lValue == 172)))
					.Returns<PIS.Ground.Core.Data.FileDistributionRequest>(null);

			_train2groundClientMock.Setup(x => x.GetTransferTask(
				It.Is<int>((lValue) => lValue == 172), out lInputUnrecordedRecipients, out lInputUnrecordedTransferTask))
					.Returns("");

			List<DataPackageService.CancellableTransferTaskInfo> lOutputTransferTask;
			bool lSuccess = DataPackageServiceInstrumented.getCancellableTransferTasks(out lOutputTransferTask);

			_train2groundClientMock.Verify(x => x.GetTransferTask(
				It.IsAny<int>(), out lInputUnrecordedRecipients, out lInputUnrecordedTransferTask),
				Times.Once());

			Assert.AreEqual(1, lOutputTransferTask.Count, "Number of expected transfer task results");
			Assert.AreEqual(172, lOutputTransferTask[0].TaskID, "Unrecorded task id");
			Assert.AreEqual(1, lOutputTransferTask[0].Elements.Count, "Number of elements");
			Assert.AreEqual("train15", lOutputTransferTask[0].Elements[0], "First element");
			Assert.IsTrue(lSuccess);
		}

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void GetCancellableTransferTasks_noUnrecordedTasks()
		{
			var lApplicationId = System.Configuration.ConfigurationSettings.AppSettings["ApplicationId"];
			var lInputTransferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			var lInputTasks3 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			lInputTasks3.taskId = 34;
			lInputTasks3.taskState = PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskStarted;
			lInputTransferTaskList.Add(lInputTasks3);

			var lInputDistributionRequest1 = new FileDistributionRequestForTest();
			var lInputRecipient1 = new RecipientId();
			lInputRecipient1.SystemId = "train1";
			lInputRecipient1.ApplicationId = lApplicationId;
			var lInputRecipient2 = new RecipientId();
			lInputRecipient2.SystemId = "train2";
			lInputRecipient2.ApplicationId = lApplicationId;
			var lInputRecipient3 = new RecipientId();
			lInputRecipient3.SystemId = "train3";
			lInputRecipient3.ApplicationId = lApplicationId;

			lInputDistributionRequest1.RecipientList.AddRange(
				new RecipientId[] { lInputRecipient1, lInputRecipient2, lInputRecipient3 });

			var lInputUnrecordedRecipients = new List<Recipient>();
			var lInputUnrecordedTransferTask = new TransferTaskData();

			_train2groundClientMock.Setup(x => x.EnumTransferTask(
				It.IsAny<DateTime>(), It.IsAny<DateTime>(), out lInputTransferTaskList))
					.Returns(true);

			_train2groundClientMock.Setup(x => x.GetFileDistributionRequestByTaskId(
				It.Is<int>((lValue) => lValue == 34)))
					.Returns(lInputDistributionRequest1);

			List<DataPackageService.CancellableTransferTaskInfo> lOutputTransferTask;
			bool lSuccess = DataPackageServiceInstrumented.getCancellableTransferTasks(out lOutputTransferTask);

			_train2groundClientMock.Verify(x => x.GetTransferTask(
				It.IsAny<int>(), out lInputUnrecordedRecipients, out lInputUnrecordedTransferTask), 
				Times.Never());

			Assert.AreEqual(1, lOutputTransferTask.Count, "Number of expected transfer task results");
			Assert.AreEqual(34, lOutputTransferTask[0].TaskID, "Recorded task id");
			Assert.AreEqual(3, lOutputTransferTask[0].Elements.Count, "Number of elements");
			Assert.AreEqual("train1", lOutputTransferTask[0].Elements[0], "First element");
			Assert.AreEqual("train2", lOutputTransferTask[0].Elements[1], "Second element");
			Assert.AreEqual("train3", lOutputTransferTask[0].Elements[2], "Third element");
			Assert.IsTrue(lSuccess);
		}

		/// <summary>Cancel transfer tasks.</summary>
		[Test]
		public void GetCancellableTransferTasks_errorGetTransferTask()
		{
			var lInputTransferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();
			var lInputTasks2 = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct();
			lInputTasks2.taskId = 172;
			lInputTasks2.taskState = PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskCreated;
			lInputTransferTaskList.Add(lInputTasks2);

			var lInputUnrecordedRecipients = new List<Recipient>();
			var lInputUnrecordedTransferTask = new TransferTaskData();

			_train2groundClientMock.Setup(x => x.EnumTransferTask(
				It.IsAny<DateTime>(), It.IsAny<DateTime>(), out lInputTransferTaskList))
					.Returns(true);

			_train2groundClientMock.Setup(x => x.GetFileDistributionRequestByTaskId(
				It.Is<int>((lValue) => lValue == 172)))
					.Returns<PIS.Ground.Core.Data.FileDistributionRequest>(null);

			_train2groundClientMock.Setup(x => x.GetTransferTask(
				It.Is<int>((lValue) => lValue == 172), out lInputUnrecordedRecipients, out lInputUnrecordedTransferTask))
					.Returns("Error in GetTransferTask()");

			List<DataPackageService.CancellableTransferTaskInfo> lOutputTransferTask;
			bool lSuccess = DataPackageServiceInstrumented.getCancellableTransferTasks(out lOutputTransferTask);

			_train2groundClientMock.Verify(x => x.GetTransferTask(
				It.IsAny<int>(), out lInputUnrecordedRecipients, out lInputUnrecordedTransferTask),
				Times.Once());

			Assert.AreEqual(0, lOutputTransferTask.Count, "Number of expected transfer task results");
			Assert.IsFalse(lSuccess);
		}

		#endregion

		#endregion
	}
}