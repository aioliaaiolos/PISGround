//---------------------------------------------------------------------------------------------------
// <copyright file="IT2GFileDistributionManager.cs" company="Alstom">
//          (c) Copyright ALSTOM 2013.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.T2G
{
    using System;

    /// <summary>Values that represent T2GFileDistributionManagerErrorEnum.</summary>
	public enum T2GFileDistributionManagerErrorEnum
	{
		/// <summary>An enum constant representing the success option.</summary>
		eT2GFD_NoError,		
		/// <summary>An enum constant representing the bad task Id error.</summary>
		eT2GFD_BadTaskId,
        /// <summary>An enum constant representing the any other error.</summary>
		eT2GFD_Other,
	}
    
    /// <summary>
    /// Delegate to be called by UpdateFile at the end of its process. Useful when the method is
    /// called asynchronously.
    /// </summary>
    /// <param name="request">The request data.</param>
	public delegate void UpdateFileCompletionCallBack(PIS.Ground.Core.Data.UploadFileDistributionRequest request);
    
	/// <summary>Interface for T2G file distribution manager.</summary>
	public interface IT2GFileDistributionManager
	{
		/// <summary>Creates file transfer task.</summary>
		/// <param name="strDescription">The description.</param>
		/// <param name="transferType">Type of the transfer.</param>
		/// <param name="folderSystemId">Identifier for the folder system.</param>
		/// <param name="intFolderId">Identifier for the int folder.</param>
		/// <param name="startDt">The start dt.</param>
		/// <param name="expirationDt">Date/Time of the expiration dt.</param>
		/// <param name="pRecipientIdList">List of recipient identifiers.</param>
		/// <param name="strError">[out] The error.</param>
		/// <returns>The new file transfer task.</returns>
		int CreateFileTransferTask(string strDescription, PIS.Ground.Core.Data.TransferType transferType, string folderSystemId, int intFolderId, DateTime startDt, DateTime expirationDt, System.Collections.Generic.List<PIS.Ground.Core.Data.RecipientId> pRecipientIdList, out string strError);		

        /// <summary>Download file.</summary>
        /// <param name="objFileDistributionRequest">The object file distribution request.</param>
        /// <returns>.</returns>
		string DownloadFile(PIS.Ground.Core.Data.DownloadFileDistributionRequest objFileDistributionRequest);

        /// <summary>Download folder.</summary>
        /// <param name="objDownloadFolderRequest">The object download folder request.</param>
        /// <returns>.</returns>
		string DownloadFolder(PIS.Ground.Core.Data.DownloadFolderRequest objDownloadFolderRequest);
		
		/// <summary>Gets file distribution request by task identifier.</summary>
		/// <param name="taskId">Identifier for the task.</param>
		/// <returns>The file distribution request by task identifier.</returns>
		PIS.Ground.Core.Data.FileDistributionRequest GetFileDistributionRequestByTaskId(int taskId);

		/// <summary>Gets folder information.</summary>
		/// <param name="intFolderId">Identifier for the int folder.</param>
		/// <param name="strError">[out] The error.</param>
		/// <returns>The folder information.</returns>
		PIS.Ground.Core.Data.IFolderInfo GetFolderInformation(int intFolderId, out string strError);

		/// <summary>Gets remote folder information.</summary>
		/// <param name="intFolderId">Identifier for the int folder.</param>
		/// <param name="pSystemId">Identifier for the system.</param>
		/// <param name="pApplicationId">Identifier for the application.</param>
		/// <param name="strError">[out] The error.</param>
		/// <returns>The remote folder information.</returns>
		PIS.Ground.Core.Data.IFolderInfo GetRemoteFolderInformation(int intFolderId, string pSystemId, string pApplicationId, out string strError);
        
		/// <summary>Gets transfer task.</summary>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="lstRecipient">[out] The list recipient.</param>
		/// <param name="objTransferTaskData">[out] Information describing the object transfer task.</param>
        /// <returns>Message if any error.</returns>
		string GetTransferTask(Guid requestId, out System.Collections.Generic.List<PIS.Ground.Core.Data.Recipient> lstRecipient, out PIS.Ground.Core.Data.TransferTaskData objTransferTaskData);

		/// <summary>Get the transfer task.</summary>
		/// <param name="taskId">task identifier</param>
		/// <param name="lstRecipient">list of Recipient.</param>
		/// <param name="objTransferTaskData">Transfer Task Data.</param>
		/// <returns>Message if any error.</returns>
		string GetTransferTask(int taskId, out System.Collections.Generic.List<PIS.Ground.Core.Data.Recipient> lstRecipient, out PIS.Ground.Core.Data.TransferTaskData objTransferTaskData);

        /// <summary>Get the list of transfer task(s) within the specified time range.</summary>
        /// <param name="startDate">Start of time range.</param>
        /// <param name="endDate">End of time range.</param>
        /// <param name="transferTaskList">List of task(s).</param>
		/// <returns>True if the list complete, false otherwise.</returns>
		bool EnumTransferTask(DateTime startDate, DateTime endDate,
			out PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList transferTaskList);
		
		/// <summary>Starts transfer task.</summary>
		/// <param name="intTaskId">Identifier for the int task.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="linkType">Type of the link.</param>
		/// <param name="sendProgressNotification">true to send progress notification.</param>
		/// <param name="automaticallyStop">true to automatically stop.</param>
		/// <param name="strError">[out] The error.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		bool StartTransferTask(int intTaskId, sbyte priority, PIS.Ground.Core.T2G.WebServices.FileTransfer.linkTypeEnum linkType, bool sendProgressNotification, bool automaticallyStop, out string strError);

        /// <summary>Cancel TransferTask.</summary>
        /// <param name="intTaskID">task id.</param>
        /// <param name="strError">error that has occured.</param>
		void CancelTransferTask(int intTaskID, out string strError);

        /// <summary>Uploads a file.</summary>
        /// <param name="objFileDistributionRequest">The object file distribution request.</param>
        /// <returns>.</returns>
		string UploadFile(PIS.Ground.Core.Data.UploadFileDistributionRequest objFileDistributionRequest);

        /// <summary>Registers a delegate to be called by UpdateFile at the end of its process.</summary>
        /// <param name="callback">The delegate.</param>
		void RegisterUpdateFileCompletionCallBack(PIS.Ground.Core.T2G.UpdateFileCompletionCallBack callback);

        /// <summary>Uploads a file asynchronous.</summary>
        /// <param name="objFileDistributionRequest">The object file distribution request.</param>
		void UploadFileAsync(PIS.Ground.Core.Data.UploadFileDistributionRequest objFileDistributionRequest);

        /// <summary>Adds an upload request.</summary>
        /// <param name="objFileDistributionRequest">Distribute file Request.</param>
        void AddUploadRequest(PIS.Ground.Core.Data.UploadFileDistributionRequest objFileDistributionRequest);

        /// <summary>Returns the file distribution manager error code by its description.</summary>
        /// <param name="errorDescription">Error description.</param>
        /// <returns>Error code</returns>
        T2GFileDistributionManagerErrorEnum GetErrorCodeByDescription(string errorDescription);
	}
}
