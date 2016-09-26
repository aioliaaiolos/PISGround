//---------------------------------------------------------------------------------------------------
// <copyright file="ServicesStubTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using DataPackageTests.ServicesStub;
using NUnit.Framework;
using AcquisitionStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.acquisitionStateEnum;
using CommLinkEnum = DataPackageTests.T2GServiceInterface.Identification.commLinkEnum;
using FileList = DataPackageTests.T2GServiceInterface.FileTransfer.fileList;
using FolderInfoStruct = DataPackageTests.T2GServiceInterface.FileTransfer.folderInfoStruct;
using RecipientStruct = DataPackageTests.T2GServiceInterface.FileTransfer.recipientStruct;
using TaskPhaseEnum = DataPackageTests.T2GServiceInterface.FileTransfer.taskPhaseEnum;
using TaskStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.taskStateEnum;
using TaskSubStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.taskSubStateEnum;
using TransferStateEnum = DataPackageTests.T2GServiceInterface.FileTransfer.transferStateEnum;
using TransferTaskStruct = DataPackageTests.T2GServiceInterface.FileTransfer.transferTaskStruct;

namespace DataPackageTests
{
    /// <summary>
    /// Perform sanity checks on services stub developed for testing.
    /// </summary>
    [Category("SanityCheck"), Category("DistributionScenario")]
    public class ServicesStubTests
    {
        /// <summary>
        /// Perform sanity check on identification service stub to ensure that it can be used for testing purpose
        /// </summary>
        [Test]
        public void VerifyIdentificationServiceStub()
        {
            T2GIdentificationServiceStub identificationService = new T2GIdentificationServiceStub();

            Assert.False(identificationService.IsSessionValid(1), "IsSessionValid return the wrong result on unknown session id");
            Assert.False(identificationService.IsSystemExist("TRAIN-1"), "IsSystemExist return the wrong result with unknown system id");
            Assert.False(identificationService.IsSystemOnline("TRAIN-1"), "IsSystemOnline return the wrong result with unknown system id");
            Assert.AreEqual(CommLinkEnum.notApplicable, identificationService.GetSystemLink("TRAIN-1"), "GetSystemLink return the wrong result with unknown system id");
            Assert.AreEqual(string.Empty, identificationService.GetUserName(2), "GetUserName return the wrong result with invalid session id");
            Assert.AreEqual(string.Empty, identificationService.GetNotificationUrl(2), "GetNotificationUrl return the wrong result with invalid session id");

            { // Test on login function - Success
                int effectiveProtocolVersion = 0;
                string notificationUrl = "http://localhost";
                Assert.AreEqual(0, identificationService.login("admind", "admin", notificationUrl, 3, out effectiveProtocolVersion, "aaa"), "identification login return the wrong session id when user name is invalid");
                Assert.AreEqual(-1, effectiveProtocolVersion, "identification login return the wrong effective protocol id when user name is invalid");

                int sessionId = identificationService.login("admin", "admin", notificationUrl, 3, out effectiveProtocolVersion, "aaa");
                Assert.AreNotEqual(0, sessionId, "identification login return the wrong session id when user name is valid");
                Assert.AreEqual(3, effectiveProtocolVersion, "identification login return the wrong effective protocol id when user name is valid");

                Assert.True(identificationService.IsSessionValid(sessionId), "Session is supposed to be valid");
                Assert.AreEqual(notificationUrl, identificationService.GetNotificationUrl(sessionId), "GetNotificationUrl return the wrong result");
                Assert.AreEqual("admin", identificationService.GetUserName(sessionId), "GetUserName return the wrong result");
            }

            {   // Test system update

                identificationService.UpdateSystem("TRAIN-1", 1, true, 0, "mission", CommLinkEnum.wifi, "127.0.0.1");
                Assert.IsTrue(identificationService.IsSystemExist("TRAIN-1"), "IsSystemExist return the wrong result");
                Assert.IsTrue(identificationService.IsSystemOnline("TRAIN-1"), "IsSystemOnline return the wrong result");
                Assert.AreEqual(CommLinkEnum.wifi, identificationService.GetSystemLink("TRAIN-1"), "GetSystemLink return the wrong result with unknown system id");

                identificationService.UpdateSystem("TRAIN-2", 2, true, 0, "mission", CommLinkEnum._2G3G, "127.0.0.2");
                Assert.IsTrue(identificationService.IsSystemExist("TRAIN-1"), "IsSystemExist return the wrong result");
                Assert.IsTrue(identificationService.IsSystemOnline("TRAIN-1"), "IsSystemOnline return the wrong result");
                Assert.AreEqual(CommLinkEnum.wifi, identificationService.GetSystemLink("TRAIN-1"), "GetSystemLink return the wrong result with unknown system id");
                Assert.IsTrue(identificationService.IsSystemExist("TRAIN-2"), "IsSystemExist return the wrong result");
                Assert.IsTrue(identificationService.IsSystemOnline("TRAIN-2"), "IsSystemOnline return the wrong result");
                Assert.AreEqual(CommLinkEnum._2G3G, identificationService.GetSystemLink("TRAIN-2"), "GetSystemLink return the wrong result with unknown system id");

                identificationService.UpdateSystem("TRAIN-1", 1, false, 0, "mission", CommLinkEnum.wifi, "128.0.0.1");
                Assert.IsTrue(identificationService.IsSystemExist("TRAIN-1"), "IsSystemExist return the wrong result");
                Assert.IsFalse(identificationService.IsSystemOnline("TRAIN-1"), "IsSystemOnline return the wrong result");
                Assert.AreEqual(CommLinkEnum.notApplicable, identificationService.GetSystemLink("TRAIN-1"), "GetSystemLink return the wrong result with unknown system id");
            }
        }

        /// <summary>
        /// Perform sanity check on file transfer service
        /// </summary>
        [Test]
        public void VerifyFileTransferServiceStub()
        {
            T2GIdentificationServiceStub identificationService = new T2GIdentificationServiceStub();
            int effectiveProtocolVersion = 0;
            int sessionId = identificationService.login("admin", "admin", string.Empty, 100, out effectiveProtocolVersion, "Test");
            Assert.AreNotEqual(0, sessionId, "login function return the wrong result");
            identificationService.UpdateSystem("TRAIN-1", 1, true, 0, string.Empty, CommLinkEnum.wifi, "127.0.0.1");

            T2GFileTransferServiceStub fileTransferService = new T2GFileTransferServiceStub(identificationService);

            int folderId = fileTransferService.CreateUploadFolder(sessionId, "This is a test", DateTime.UtcNow.AddDays(1), false, new FilePathInfo("file1.txt", 30, 100), new FilePathInfo("file2.txt", 1000, 101));
            Assert.Greater(folderId, 0, "createUploadeFolder return a wrong folder id");
            Assert.AreEqual(folderId, fileTransferService.LastCreatedFolder, "createUploadFolder does not initialize LastCreateFolder property");

            FileList fileList;

            FolderInfoStruct folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(2, fileList.Count, "GetFolderInfo return the wrong file list");

            int taskId = fileTransferService.CreateTransferTask(sessionId, "One transfer", T2GServiceInterface.FileTransfer.transferTypeEnum.groundToTrain, "ground", folderId, DateTime.UtcNow, TransferTaskInfo.NullDate, "TRAIN-1", "bbb,ccc");
            Assert.Greater(taskId, 0, "CreateTransferTask return an invalid task identifier");
            Assert.AreEqual(taskId, fileTransferService.LastCreatedTransfer, "CreateTransferTask does not initialize LastCreatedTransfer property");

            TransferTaskStruct task = fileTransferService.GetTransferTask(sessionId, taskId);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskCreated, task.taskState, "CreateTransferTask does not initialize properly the taskState");
            Assert.AreEqual(TaskPhaseEnum.creationPhase, task.taskPhase, "CreateTransferTask does not initialize properly the taskPhase");

            // Start the transfer
            fileTransferService.startTransfer(sessionId, taskId, (sbyte)10, T2GServiceInterface.FileTransfer.linkTypeEnum.anyBandwidth, false, false);
            task = fileTransferService.GetTransferTask(sessionId, taskId);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "startTransfer does not initialize properly the taskState");
            Assert.AreEqual(TaskPhaseEnum.acquisitionPhase, task.taskPhase, "startTransfer does not initialize properly the taskPhase");
            Assert.AreEqual(TaskSubStateEnum.subtaskInProgress, task.taskSubState, "startTransfer does not initialize properly the taskSubState");

            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionStarted, folderInfo.acquisitionState, "startTransfer does not initialize properly the acquisition state of the folder");
            Assert.AreEqual(0, folderInfo.currentFilesCount, "folder current file count differ than one expected");

            // Perform a progression. Expect that one file was acquired.
            fileTransferService.PerformTransferProgression();

            RecipientStruct recipient;
            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.acquisitionPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskInProgress, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.notTransferring, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(0, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(0, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionStarted, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(1, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // Perform a progression. Expect that acquisition was completed.
            fileTransferService.PerformTransferProgression();

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.transferPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskWaitingSchedule, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.waitingInQueue, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(0, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(0, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // System become offline
            identificationService.UpdateSystem("TRAIN-1", 1, false, 0, string.Empty, CommLinkEnum.wifi, "127.0.0.1");
            fileTransferService.PerformTransferProgression();

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.transferPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskWaitingComm, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.waitingForConnection, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(0, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(0, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // System become online
            identificationService.UpdateSystem("TRAIN-1", 1, true, 0, string.Empty, CommLinkEnum.wifi, "127.0.0.1");
            fileTransferService.PerformTransferProgression();

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.transferPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskInProgress, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.transferring, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(1, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(0, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // Update the progression
            fileTransferService.PerformTransferProgression();

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.transferPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskInProgress, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.transferring, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(1, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(1, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // Update the progression  - Expect  to complete the transfer phase
            fileTransferService.PerformTransferProgression();

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskStarted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.distributionPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskInProgress, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.transferCompleted, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(0, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(1, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(2, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.transferCompletionPercent, "Transfert completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // Update the progression  - Expect  that whole transfer is completed/
            fileTransferService.PerformTransferProgression();

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskCompleted, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.distributionPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskNone, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.transferCompleted, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(0, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(1, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(2, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.transferCompletionPercent, "Transfer completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");

            // Cancel the transfer task
            fileTransferService.cancelTransfer(sessionId, taskId);

            task = fileTransferService.GetTransferTask(sessionId, taskId, out recipient);
            Assert.AreEqual(taskId, task.taskId, "GetTransferTask return the wrong task");
            Assert.AreEqual(TaskStateEnum.taskCancelled, task.taskState, "TaskState after transfer progression is not set to expected value");
            Assert.AreEqual(TaskPhaseEnum.distributionPhase, task.taskPhase, "TaskPhase after transfer progression is not set to expected value");
            Assert.AreEqual(TaskSubStateEnum.subtaskNone, task.taskSubState, "TaskSubState after transfer progression is not set to expected value.");
            Assert.AreEqual(TransferStateEnum.transferCompleted, recipient.transferState, "Transfer state after transfer progression is not set to expected value");
            Assert.AreEqual(0, task.activeFileTransferCount, "Active transfer count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.distributingFileTransferCount, "distributing count is not set to expected value after transfer progression");
            Assert.AreEqual(0, task.errorCount, "Error count is not set to expected value after transfer progression");
            Assert.AreEqual(1, task.completedFileTransferCount, "Completion count is not set to expected value after transfer progression");
            Assert.AreEqual(2, recipient.transferredFilesCount, "Recipient transfer count is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.acquisitionCompletionPercent, "Acquisition completion percent is not set to expected value after transfer progression");
            Assert.AreEqual((sbyte)100, task.transferCompletionPercent, "Transfer completion percent is not set to expected value after transfer progression");
            folderInfo = fileTransferService.GetFolderInfo(sessionId, folderId, out fileList);
            Assert.IsNotNull(folderInfo, "GetFolderInfo does not return a folder as expected");
            Assert.AreEqual(folderId, folderInfo.folderId, "GetFolderInfo return the wrong folder");
            Assert.AreEqual(AcquisitionStateEnum.acquisitionSuccess, folderInfo.acquisitionState, "Acquisition state after transfer progression is not set to expected value");
            Assert.AreEqual(2, folderInfo.currentFilesCount, "CurrentFileCount after transfer progression is not set to expected value");
        }

    }
}
