//---------------------------------------------------------------------------------------------------
// <copyright file="T2GFileDistributionManager.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.T2G
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.ServiceModel;
    using System.Threading;
    using PIS.Ground.Core.LogMgmt;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.Properties;
    using PIS.Ground.Core.T2G.WebServices.FileTransfer;
    using PIS.Ground.Core.Utility;
    
    public class T2GFileDistributionManager : IT2GFileDistributionManager
    {
        #region Private fields

        /// <summary>Polling to T2G server in milliseconds.</summary>
        private const int PollingTime = 60000;

        /// <summary>max retry.</summary>
        private const int MaxRetry = 5;

        /// <summary>Time out in minutes.</summary>
        private const int TimeOut = 300;

        /// <summary>Variable used for folderSystemID parameter of CreateTransferTask.</summary>
        private const string SystemFolderIDG2T = "ground";

        /// <summary>The system folder idt 2 g.</summary>
        private const string SystemFolderIDT2G = "on board";

        /// <summary>Information describing the session.</summary>
        private T2GSessionData _sessionData; 

        /// <summary>The notifier target.</summary>
        private IT2GNotifierTarget _notifierTarget;

        /// <summary>Lock for _fileDistributionRequests.</summary>
        private Object _fileDistributionRequestsLock = new object();

        /// <summary>Holds the requests.</summary>
        private Dictionary<Guid, FileDistributionRequest> _fileDistributionRequests;

        /// <summary>Waiting time between two uploads.</summary>
        private int _sleepTimeBetweenUploads;

        /// <summary>List of pending upload tasks.</summary>
        private ListWithChangedEvent _pendingUploadTaskList;

        /// <summary>Number of processing upload tasks.</summary>
        private int _processingUploadTaskCount;

        /// <summary>The processing upload task count limit.</summary>
        private int _processingUploadTaskCountLimit;

        /// <summary>The processing upload lock.</summary>
        private object _processingUploadLock = new object();                       

        /// <summary>The T2G file transfer port channel. Can be mocked for unit tests.</summary>
		private FileTransferPortType _fileTransferPort;

        /// <summary>
        /// Delegate to be called by UpdateFile at the end of its process.
        /// Useful when the method is called asynchronously.
        /// </summary>
		private UpdateFileCompletionCallBack _updateFileCompletionCallBack = null;

        #endregion

        /// <summary>Initializes a new instance of the T2GFileDistributionManager class.</summary>
        /// <param name="sessionData">Information describing the session.</param>
        internal T2GFileDistributionManager(T2GSessionData sessionData, IT2GNotifierTarget notifierTarget)
		{
            Create(sessionData, notifierTarget, null);
        }

        /// <summary>Initializes a new instance of the T2GFileDistributionManager class.</summary>
        /// <param name="sessionData">Information describing the session.</param>
        /// <param name="notifierTarget">The notifier target.</param>
        /// <param name="fileTransferPort">The T2G file transfer port channel. Can be mocked for unit
        /// tests.</param>
        internal T2GFileDistributionManager(T2GSessionData sessionData, IT2GNotifierTarget notifierTarget, FileTransferPortType fileTransferPort)
		{
            Create(sessionData, notifierTarget, fileTransferPort);
        }

        /// <summary>Creates this object.</summary>
        /// <param name="sessionData">Information describing the session.</param>
        /// <param name="notifierTarget">The notifier target.</param>
        /// <param name="fileTransferPort">The T2G file transfer port channel. Can be mocked for unit
        /// tests.</param>
        private void Create(T2GSessionData sessionData, IT2GNotifierTarget notifierTarget, FileTransferPortType fileTransferPort)
		{
            _sessionData = sessionData;
       
            _notifierTarget = notifierTarget;

            _fileDistributionRequests = new Dictionary<Guid, FileDistributionRequest>();

            _sleepTimeBetweenUploads = 60000;
            string parameterValue = System.Configuration.ConfigurationSettings.AppSettings["WaitingTimeBetweenUploads"];
            if (string.IsNullOrEmpty(parameterValue))
            {
                string message = string.Format(CultureInfo.CurrentCulture, Resources.OptionalConfigurationParameterMissing, "WaitingTimeBetweenUploads", _sleepTimeBetweenUploads);
                LogManager.WriteLog(TraceType.INFO, message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.Create", null, EventIdEnum.GroundCore);
            }
            else
            {
                try
                {
                    _sleepTimeBetweenUploads = int.Parse(parameterValue) * 1000;
                }
                catch (Exception ex)
                {
                    string message = string.Format(CultureInfo.CurrentCulture, Resources.ConfigurationErrorInvalidIntegerParameterValueWithDefault, parameterValue, "WaitingTimeBetweenUploads", _sleepTimeBetweenUploads);
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.Create", ex, EventIdEnum.GroundCore);
                }
            }

            _processingUploadTaskCountLimit = 1;
            parameterValue = System.Configuration.ConfigurationSettings.AppSettings["MaxParallelUploadsLimit"];
            if (string.IsNullOrEmpty(parameterValue))
            {
                string message = string.Format(CultureInfo.CurrentCulture, Resources.OptionalConfigurationParameterMissing, "MaxParallelUploadsLimit", _processingUploadTaskCountLimit);
                LogManager.WriteLog(TraceType.INFO, message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.Create", null, EventIdEnum.GroundCore);
            }
            else
            {
                try
                {
                    _processingUploadTaskCountLimit = int.Parse(parameterValue);
                }
                catch (Exception ex)
                {
                    string message = string.Format(CultureInfo.CurrentCulture, Resources.ConfigurationErrorInvalidIntegerParameterValueWithDefault, parameterValue, "MaxParallelUploadsLimit", _processingUploadTaskCountLimit);
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.Create", ex, EventIdEnum.GroundCore);
                }
            }

            _processingUploadTaskCount = 0;
            _pendingUploadTaskList = new ListWithChangedEvent();
            _pendingUploadTaskList.changed += new ChangedEventHandler(OnPendingUploadTaskListChanged);

            try
			{
				if (fileTransferPort == null)
                {
                    FileTransferPortTypeClient fileTransferClient = new FileTransferPortTypeClient();
				    System.ServiceModel.ChannelFactory<FileTransferPortType> channelFactory = fileTransferClient.ChannelFactory;
				    _fileTransferPort = channelFactory.CreateChannel();
                }
                else
                {
                    _fileTransferPort = fileTransferPort;
                }
			}
			catch (Exception ex)
			{
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.Create", ex, EventIdEnum.GroundCore);
			}
        }               

        /// <summary>Initializes this object when T2G server becomes online.</summary>
        public void Initialize()
        {
            SubscribeToTransferTaskNotifications();
        }

        /// <summary>De-initialises this object and frees any resources it is using when T2G server becomes offline.</summary>
        public void Deinitialize()
        {
            // nothing to do on deinitialization
        }

        /// <summary>Subscribe the File Transfer Task Notifications.</summary>
        private void SubscribeToTransferTaskNotifications()
        {
            try
            {
                using (FileTransferPortTypeClient client = new FileTransferPortTypeClient())
                {
                    client.subscribeToNotifications(_sessionData.SessionId);
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.SubscribeToTransferTaskNotifications", ex, EventIdEnum.GroundCore);
            }
        }

        /// <summary>Get the transfer task.</summary>
        /// <param name="requestId">input rewuest id.</param>
        /// <param name="lstRecipient">list of Recipient.</param>
        /// <param name="objTransferTaskData">Transfer Task Data.</param>
        /// <returns>If any error.</returns>
        public string GetTransferTask(Guid requestId, out List<Recipient> lstRecipient, out TransferTaskData objTransferTaskData)
        {
            lstRecipient = new List<Recipient>();
            string error = string.Empty;
            objTransferTaskData = new TransferTaskData();

            lock (_fileDistributionRequestsLock)
            {
                if (_fileDistributionRequests.ContainsKey(requestId))
                {
                    try
                    {
                        recipientList recipientList = new recipientList();
                        FileDistributionRequest objFileDistributionRequest = _fileDistributionRequests[requestId];
                        
                        if (objFileDistributionRequest != null && objFileDistributionRequest.TaskId > 0)
                        {
                            error = GetTransferTask(objFileDistributionRequest.TaskId, out lstRecipient, out objTransferTaskData);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.GetTransferTask", ex, EventIdEnum.GroundCore);
                        error = ex.Message;
                    }
                }
                else
                {
                    error = Resources.LogRequestIdNotExist;
                }
            }

            return error;
        }

		/// <summary>Get the transfer task.</summary>
		/// <param name="taskId">task identifier</param>
		/// <param name="lstRecipient">list of Recipient.</param>
		/// <param name="objTransferTaskData">Transfer Task Data.</param>
		/// <returns>Message if any error.</returns>
		public string GetTransferTask(int taskId, out List<Recipient> lstRecipient, out TransferTaskData objTransferTaskData)
		{
			lstRecipient = new List<Recipient>();
			string error = string.Empty;
			objTransferTaskData = new TransferTaskData();

			try
			{
				recipientList recipientList = new recipientList();

                using (FileTransferPortTypeClient objFileTransferPortTypeClient = new FileTransferPortTypeClient())
				{
					PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct transferTask = 
                        objFileTransferPortTypeClient.getTransferTask(
                            _sessionData.SessionId,
                            taskId,
                            out recipientList);

					if (recipientList != null)
					{
						foreach (recipientStruct recipient in recipientList)
						{
							Recipient objRecipient = new Recipient();
							objRecipient.ApplicationIds = recipient.applicationIds;
							objRecipient.MissionId = recipient.missionId;
							objRecipient.SystemId = recipient.systemId;
							lstRecipient.Add(objRecipient);
						}
					}

					if (transferTask != null)
					{
                        objTransferTaskData = T2GDataConverter.BuildTransferTaskData(transferTask);
					}
				}
			}
			catch (Exception ex)
			{
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.GetTransferTask", ex, EventIdEnum.GroundCore);
				error = ex.Message;
			}

			return error;
		}

        /// <summary>Get the list of transfer task(s) within the specified time range.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="startDate">Start of time range.</param>
        /// <param name="endDate">End of time range.</param>
        /// <param name="transferTaskList">List of task(s).</param>
        /// <returns>True if the list complete, false otherwise.</returns>
		public bool EnumTransferTask(DateTime startDate, DateTime endDate, out PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList transferTaskList)
		{
			bool lEndOfEnum = false;
			transferTaskList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();

			try
			{
				if (_fileTransferPort == null)
				{
					throw new InvalidOperationException("DataMember _fileTransferPort is null");
				}

				var lEnumRequest = new PIS.Ground.Core.T2G.WebServices.FileTransfer.enumTransferTaskInput();
				lEnumRequest.Body = new enumTransferTaskInputBody();
                lEnumRequest.Body.sessionId = _sessionData.SessionId;
				lEnumRequest.Body.startDate = startDate;
				lEnumRequest.Body.endDate = endDate;
				lEnumRequest.Body.enumPos = 0;

				do
				{
					// The T2G might return the list of tasks by chunks.
					
					DateTime lCurrentTime = DateTime.Now;
					do
					{
						if (DateTime.Now.Subtract(lCurrentTime).TotalMinutes > TimeOut)
						{
							throw new Exception("Timeout retrieving transfers list from T2G");
						}

						try
						{
							enumTransferTaskOutput lTransferTask = _fileTransferPort.enumTransferTask(lEnumRequest);

							if (lTransferTask != null && lTransferTask.Body != null)
							{
								lEndOfEnum = lTransferTask.Body.endOfEnum;

								if (lTransferTask.Body.transferTaskList != null &&
									lTransferTask.Body.transferTaskList.Count > 0)
								{
									transferTaskList.AddRange(lTransferTask.Body.transferTaskList);

									if (lEndOfEnum == false)
									{
										lEnumRequest.Body.enumPos++;
									}
								}
							}
							break;
						}
						catch (System.Web.Services.Protocols.SoapException ex)
						{
							LogManager.WriteLog(TraceType.WARNING, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.EnumTransferTask",
								ex, EventIdEnum.GroundCore);
						}
						Thread.Sleep(PollingTime);

					} while (true);

				} while (lEndOfEnum == false);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.EnumTransferTask", 
					ex, EventIdEnum.GroundCore);
			}
			return lEndOfEnum;
		}
		
		/// <summary>Uploads a file asynchronously.</summary>
		/// <param name="objFileDistributionRequest">Distribute file Request.</param>
		public void UploadFileAsync(UploadFileDistributionRequest objFileDistributionRequest)
		{
			Func<UploadFileDistributionRequest, string> invoker = UploadFile;
            invoker.BeginInvoke(objFileDistributionRequest, result => invoker.EndInvoke(result), null);
		}
		
        /// <summary>Adds an upload request.</summary>
        /// <param name="objFileDistributionRequest">Distribute file Request.</param>
        public void AddUploadRequest(UploadFileDistributionRequest objFileDistributionRequest)
        {
            _pendingUploadTaskList.Add(objFileDistributionRequest);
        }

        /// <summary>Waiting upload list changed.</summary>
        /// <param name="sender">sender of the event.</param>
        /// <param name="e">Event information.</param>
        private void OnPendingUploadTaskListChanged(object sender, EventArgs e)
        {
            lock (_processingUploadLock)
            {
                if (_processingUploadTaskCount < _processingUploadTaskCountLimit)
                {
                    object req = _pendingUploadTaskList.PopNoEvent();
                    
                    if (req is UploadFileDistributionRequest)
                    {
                        _processingUploadTaskCount++;
                        UploadFileAsync(req as UploadFileDistributionRequest);                        
                    }
                }
                else
                {
                    LogManager.WriteLog(TraceType.INFO, "Request is Waiting", "PIS.Ground.Core.T2G.T2GFileDistributionManager.OnPendingUploadTaskListChanged", null, EventIdEnum.GroundCore);
                }
            }
        }

        /// <summary>
        /// Find in the list of upload request a folderId containing similar data as the current request' one 
        /// </summary>
        /// <param name="objFileDistributionRequest">Current request</param>
        private void GetFolderIdFromSimilarRequest(ref UploadFileDistributionRequest objFileDistributionRequest)
        {
            string strError = string.Empty;

            lock (_fileDistributionRequestsLock)
            {
                //Search for request with a folder that has same CRCGuid, which means the exact same contents as the new request we want to send
                foreach (KeyValuePair<Guid, FileDistributionRequest> pair in _fileDistributionRequests)
                {
                    if (pair.Value is UploadFileDistributionRequest)
                    {
                        if (pair.Value.Folder.CRCGuid == objFileDistributionRequest.Folder.CRCGuid)
                        {
                            //Request with the CRCGuid has been found. Check if it has a folder ID and uploading state is not failed.
                            if ((pair.Value.Folder.FolderId > 0) && (pair.Value.Folder.UploadingState != UploadingStateEnum.Failed))
                            {
                                if (pair.Value.Folder.ExpirationDate >= objFileDistributionRequest.Folder.ExpirationDate)
                                {
                                    if (pair.Value.Folder.UploadingState == UploadingStateEnum.Uploaded)
                                    {
                                        //Check if still exists
                                        try
                                        {
                                            IFolderInfo lFolderInfo = GetFolderInformation(pair.Value.Folder.FolderId, out strError);

                                            if (lFolderInfo.FolderId == pair.Value.Folder.FolderId)
                                            {
                                                //Get the RequestId of the request which Uploaded those data. Use to flag if we are or not the uploader.
                                                objFileDistributionRequest.UploadingRequestID = ((UploadFileDistributionRequest)pair.Value).UploadingRequestID;

                                                objFileDistributionRequest.Folder.UploadingState = UploadingStateEnum.Uploaded;

                                                objFileDistributionRequest.Folder.FolderId = pair.Value.Folder.FolderId;

                                                break;
                                            }
                                        }
                                        catch (System.Web.Services.Protocols.SoapException ex)
                                        {
                                            LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.GetFolderIdFromSimilarRequest", ex, EventIdEnum.GroundCore);
                                        }
                                        catch (Exception ex)
                                        {
                                            LogManager.WriteLog(TraceType.ERROR, ex.Message, "Ground.Core.T2G.T2GClient.GetFolderIdFromSimilarRequest", ex, EventIdEnum.GroundCore);

                                        }

                                    }
                                    else
                                    {
                                        if (pair.Value.Folder.UploadingState == UploadingStateEnum.InProgress)
                                        {
                                            objFileDistributionRequest.Folder.FolderId = pair.Value.Folder.FolderId;
                                            
                                            //Our data are still uploading.
                                            //Get the RequestId of the request Uploading those data.
                                            objFileDistributionRequest.UploadingRequestID = ((UploadFileDistributionRequest)pair.Value).UploadingRequestID;

                                            //Register for an notification from this request when upload phase is done
                                            ((UploadFileDistributionRequest)pair.Value).RegisterForEndUploadNotification(objFileDistributionRequest.RequestId);

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find in the list of T2G folders a folderId containing similar data as the current request's one
        /// </summary>
        /// <param name="objFileDistributionRequest">Current request</param>
        public void GetFolderIdFromSimilarT2GFolder(ref UploadFileDistributionRequest objFileDistributionRequest)
        {
            //Search directly on T2G server if a folder could match our data
            bool lIsEndOfEnum = false;
            ushort lEnumPos = 0;
            string strError = string.Empty;

            if (objFileDistributionRequest.Folder.FolderId <= 0)
            {
                //Parse folders on T2G server
                while ((!lIsEndOfEnum) && (objFileDistributionRequest.Folder.FolderId <= 0))
                {
                    try
                    {
                        folderList T2GFoldersList = EnumFolders(objFileDistributionRequest.Folder.ApplicationId, PIS.Ground.Core.T2G.WebServices.FileTransfer.folderTypeEnum.upload, lEnumPos, out lIsEndOfEnum, out strError);

                        if (T2GFoldersList != null)
                        {
                            foreach (PIS.Ground.Core.T2G.WebServices.FileTransfer.folderInfoStruct T2GFolder in T2GFoldersList)
                            {
                                FolderNameStruct lFoldername = new FolderNameStruct(T2GFolder.name);

                                bool knownRequest;

                                lock (_fileDistributionRequestsLock)
                                {
                                    knownRequest = _fileDistributionRequests.ContainsKey(lFoldername.RequestId);
                                }

                                //ignore if Request ID match with a request in current request list.
                                if (!knownRequest)
                                {
                                    //Compare folder to current request CRCGuid
                                    if (objFileDistributionRequest.Folder.CRCGuid.CompareTo(lFoldername.CRCGuid) == 0)
                                    {
                                        //Check if folder expiration date on T2G server is far enough to be used for our current request and folder acquisition is a success
                                        if ((T2GFolder.expirationDate >= objFileDistributionRequest.Folder.ExpirationDate) && (T2GFolder.acquisitionState == acquisitionStateEnum.acquisitionSuccess))
                                        {
                                            //Use this FolderId
                                            objFileDistributionRequest.UploadingRequestID = lFoldername.RequestId;
                                            objFileDistributionRequest.Folder.FolderId = T2GFolder.folderId;
                                            objFileDistributionRequest.Folder.UploadingState = UploadingStateEnum.Uploaded;

                                            break;
                                        }
                                    }
                                }
                            }
                         }
                    }
                    catch (System.Web.Services.Protocols.SoapException ex)
                    {
                        LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.GetFolderIdFromSimilarT2GFolder", ex, EventIdEnum.GroundCore);
                        //get out of the while loop
                        lIsEndOfEnum = true;
                    }
                    catch (Exception ex)
                    {
                        LogManager.WriteLog(TraceType.ERROR, ex.Message, "Ground.Core.T2G.T2GClient.GetFolderIdFromSimilarT2GFolder", ex, EventIdEnum.GroundCore);
                        //get out of the while loop
                        lIsEndOfEnum = true;
                    }
                    lEnumPos++;
                }
            }
        }

        /// <summary>
        /// Search for a folderId that can be reused.
        /// </summary>
        /// <param name="objFileDistributionRequest">Current request</param>
        private void FindFolderIdWithSimilarCRC(ref UploadFileDistributionRequest objFileDistributionRequest)
        {
            GetFolderIdFromSimilarRequest(ref objFileDistributionRequest);

            if (objFileDistributionRequest.Folder.FolderId <= 0)
            {
                GetFolderIdFromSimilarT2GFolder(ref objFileDistributionRequest);
            }
        }

        /// <summary>Delegate to be called by UploadFile for task creation</summary>
        /// <param name="request">The request data.</param>
        public delegate string CreateAndStartTaskCallback(FileDistributionRequest request);

        /// <summary>
        /// Notify each associated request of a uploading request about the upload state. Has it succeed ? has it failed ?
        /// If it succeed, create transfer task for associated request.
        /// </summary>
        /// <param name="objFileDistributionRequest"></param>
        private void NotifyAssociatedRequestOfUploadingState(UploadFileDistributionRequest objFileDistributionRequest)
        {
            CreateAndStartTaskCallback callbackFunc = CreateAndStartTask;

            lock (_fileDistributionRequestsLock)
            {
                foreach (Guid lAssociatedRequestId in objFileDistributionRequest.AssociatedRequestId)
                {
                    if (objFileDistributionRequest.Folder.UploadingState == UploadingStateEnum.Uploaded)
                    {
                        _fileDistributionRequests[lAssociatedRequestId].Folder.UploadingState = UploadingStateEnum.Uploaded;

                        //If the folder was correctly uploaded. Create and start the task
                        //Function is async as we don't want to stay lock while performing task creation.
                        callbackFunc.BeginInvoke(_fileDistributionRequests[lAssociatedRequestId], null, null);
                    }
                    else
                    {
                        //Set all current associated as failed. If a new request occurs with same CRCGuid, it will then try to create its own folder.
                        _fileDistributionRequests[lAssociatedRequestId].Folder.UploadingState = UploadingStateEnum.Failed;
                    }
                }

                //Update object in the list
                this.UpdateRequestList(objFileDistributionRequest);
            }
        }

        /// <summary>Uploads a file.</summary>
        /// <param name="objFileDistributionRequest">Distribute file Request.</param>
        /// <returns>Error message if any.</returns>
		public string UploadFile(UploadFileDistributionRequest objFileDistributionRequest)
		{
			string strError = string.Empty;
            try
            {
                LogManager.WriteLog(TraceType.INFO, String.Format(CultureInfo.CurrentCulture, Resources.InUploadFile) + " " + String.Format(CultureInfo.CurrentCulture, Resources.RequestInfo, objFileDistributionRequest.RequestId.ToString(), objFileDistributionRequest.Folder.CRCGuid, objFileDistributionRequest.Folder.FolderId.ToString()), "Ground.Core.T2G.T2GClient.UploadFile", null, EventIdEnum.GroundCore);

                try
                {

                    if (objFileDistributionRequest != null)
                    {
                        if (this.ValidateUploadFileRequest(objFileDistributionRequest, out strError))
                        {
                            //Calculate CRCGuid and append it to Upload Folder name
                            objFileDistributionRequest.Folder.CalculateCRCGuid();
                            objFileDistributionRequest.Folder.FolderName += "|" + objFileDistributionRequest.Folder.CRCGuid;

                            lock (_fileDistributionRequestsLock)
                            {
                                //If not findAndUseFolderIdByCRC, this request must create a new UploadFolder
                                FindFolderIdWithSimilarCRC(ref objFileDistributionRequest);

                                if (objFileDistributionRequest.Folder.FolderId <= 0)
                                {
                                    //if not CreateUploadFolder
                                    CreateUploadFolder(ref objFileDistributionRequest, out strError);

                                    if (objFileDistributionRequest.Folder.FolderId > 0)
                                    {
                                        //Flag the folder as uploading. This permits others requests with same CRCGuid to know this request is already uploading the same data
                                        objFileDistributionRequest.Folder.UploadingState = UploadingStateEnum.InProgress;
                                        objFileDistributionRequest.UploadingRequestID = objFileDistributionRequest.RequestId;
                                    }
                                    else
                                    {
                                        //Folder Upload is considered as failed.
                                        objFileDistributionRequest.Folder.UploadingState = UploadingStateEnum.Failed;
                                        strError = Resources.UploadFolderCreationFailed;
                                    }
                                }

                                //Add request to request list

                                if ((!_fileDistributionRequests.ContainsKey(objFileDistributionRequest.RequestId)) && (objFileDistributionRequest.Folder.UploadingState != UploadingStateEnum.Failed))
                                {
                                    _fileDistributionRequests.Add(objFileDistributionRequest.RequestId, objFileDistributionRequest);
                                }
                            }

                            //If we need to upload files then do it
                            if ((objFileDistributionRequest.RequestId == objFileDistributionRequest.UploadingRequestID) && (objFileDistributionRequest.Folder.UploadingState != UploadingStateEnum.Failed))
                            {
                                strError = FtpUtility.Upload(ref objFileDistributionRequest);

                                //Report error
                                if (objFileDistributionRequest.Folder.UploadingState == UploadingStateEnum.Failed)
                                {

                                    foreach (IRemoteFileClass lRemoteFiles in objFileDistributionRequest.Folder.FolderFilesList)
                                    {
                                        if (lRemoteFiles.HasError)
                                        {
                                            string errorDesc = this.MapFtpStatusCode(lRemoteFiles.FtpStatus.FtpStatusCode);
                                            LogManager.WriteLog(TraceType.ERROR, errorDesc, "PIS.Ground.Core.Utility.UploadFile", null, EventIdEnum.GroundCore);
                                        }
                                    }

                                    if (strError != string.Empty)
                                    {
                                        LogManager.WriteLog(TraceType.ERROR, strError, "PIS.Ground.Core.Utility.UploadFile", null, EventIdEnum.GroundCore);
                                    }
                                }

                                if (objFileDistributionRequest.Folder.UploadingState != UploadingStateEnum.Failed)
                                {
                                    objFileDistributionRequest.Folder.UploadingState = UploadingStateEnum.Uploaded;
                                    strError = CreateAndStartTask(objFileDistributionRequest);
                                }

                                //wait before next upload
                                Thread.Sleep(_sleepTimeBetweenUploads);

                                //check if folder acquisition is OK. It might happen that upload worked correctly on pisground side but that the folder was not acquired correctly on T2G side after task creation.
                                try
                                {
                                    IFolderInfo lFolderInfo = GetFolderInformation(objFileDistributionRequest.Folder.FolderId, out strError);
                                    if (lFolderInfo == null || (lFolderInfo.AcquisitionState == acquisitionStateEnum.acquisitionError) || (lFolderInfo.AcquisitionState == acquisitionStateEnum.notAcquired))
                                    {
                                        objFileDistributionRequest.Folder.UploadingState = UploadingStateEnum.Failed;
                                    }
                                }
                                catch (System.Web.Services.Protocols.SoapException ex)
                                {
                                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.UploadFile", ex, EventIdEnum.GroundCore);
                                    objFileDistributionRequest.Folder.UploadingState = UploadingStateEnum.Failed;
                                }
                                catch (Exception ex)
                                {
                                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "Ground.Core.T2G.T2GClient.UploadFile", ex, EventIdEnum.GroundCore);
                                    objFileDistributionRequest.Folder.UploadingState = UploadingStateEnum.Failed;
                                }

                                this.UpdateRequestList(objFileDistributionRequest);

                                NotifyAssociatedRequestOfUploadingState(objFileDistributionRequest);

                            }
                            else
                            {
                                //If data upload is a success or they were already uploaded. Create the transfer task 
                                if (objFileDistributionRequest.Folder.UploadingState == UploadingStateEnum.Uploaded)
                                {
                                    strError = CreateAndStartTask(objFileDistributionRequest);
                                }
                            }
                        }
                        else
                        {
                            strError = Resources.InvalidUploadFileRequest;
                        }
                    }
                    else
                    {
                        strError = Resources.InvalidInputParameter;
                    }
                }
                // Avoid logging thread abort exception.
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.Utility.UploadFile", ex, EventIdEnum.GroundCore);
                }

                if (strError != string.Empty)
                {
                    LogManager.WriteLog(TraceType.ERROR, strError, "PIS.Ground.Core.Utility.UploadFile", null, EventIdEnum.GroundCore);
                }

                if (_updateFileCompletionCallBack != null)
                {
                    try
                    {
                        _updateFileCompletionCallBack(objFileDistributionRequest);

                    }
                    // Avoid logging thread abort exception.
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception lException)
                    {
                        LogManager.WriteLog(TraceType.ERROR,
                            Resources.ExceptionUpdateFileCallback,
                            "PIS.Ground.Core.T2G.T2GFileDistributionManager",
                            lException,
                            EventIdEnum.GroundCore);
                    }
                }

                lock (_processingUploadLock)
                {
                    if (_processingUploadTaskCount > 0)
                    {
                        _processingUploadTaskCount--;
                    }
                }

                OnPendingUploadTaskListChanged(this, EventArgs.Empty);
            }
            catch (ThreadAbortException exception)
            {
                // Log ThreadAbortException only for debugging purpose.
                LogManager.WriteLog(TraceType.DEBUG,
                    Resources.ExceptionUpdateFileCallback,
                    "PIS.Ground.Core.T2G.T2GFileDistributionManager",
                    exception,
                    EventIdEnum.GroundCore);
            }
            catch (System.Exception exception)
            {
                LogManager.WriteLog(TraceType.EXCEPTION,
                    Resources.ExceptionUpdateFileCallback,
                    "PIS.Ground.Core.T2G.T2GFileDistributionManager",
                    exception,
                    EventIdEnum.GroundCore);

            }

			return strError;
		}

        /// <summary>
        /// Create and start a transfer task
        /// </summary>
        /// <param name="objFileDistributionRequest">Request creating and starting the transfer task</param>
        /// <returns></returns>
        private string CreateAndStartTask(FileDistributionRequest objFileDistributionRequest)
        {
            string strError = string.Empty;

			try
			{
				//If data upload is a success or they were already uploaded. Create the transfer task 
				if (objFileDistributionRequest.Folder.UploadingState == UploadingStateEnum.Uploaded)
				{
					int taskid = this.CreateFileTransferTask(objFileDistributionRequest.Description, TransferType.GroundToTrain, SystemFolderIDG2T, objFileDistributionRequest.Folder.FolderId, objFileDistributionRequest.StartDate, objFileDistributionRequest.ExpirationDate, objFileDistributionRequest.RecipientList, out strError);

					if ((taskid > 0) && (strError.CompareTo(string.Empty) == 0))
					{
                        if (LogManager.IsTraceActive(TraceType.INFO))
                        {
                            LogManager.WriteLog(TraceType.INFO, String.Format(CultureInfo.CurrentCulture, Resources.RequestAndTaskInfo, objFileDistributionRequest.RequestId.ToString(), objFileDistributionRequest.Folder.CRCGuid, objFileDistributionRequest.Folder.FolderId, taskid), "Ground.Core.T2G.T2GClient.CreateAndStartTask", null, EventIdEnum.GroundCore);
                        }

						objFileDistributionRequest.TaskId = taskid;

						linkTypeEnum lnkTypeEnum = T2GDataConverter.BuildLinkTypeEnum(objFileDistributionRequest.FileTransferMode);

						this.StartTransferTask(taskid, objFileDistributionRequest.Priority, lnkTypeEnum, true, true, out strError);

						this.UpdateRequestList(objFileDistributionRequest);
					}
				}
				else
				{
					strError = Resources.FolderNotUploaded;
				}

				if (strError != string.Empty)
				{
					LogManager.WriteLog(TraceType.ERROR, strError, "PIS.Ground.Core.T2G.T2GFileDistributionManager.CreateAndStartTask", null, EventIdEnum.GroundCore);
				}
			}
			catch (System.Exception exception)
			{
				LogManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.CreateAndStartTask", exception, EventIdEnum.GroundCore);
			}

            return strError;
        }

        /// <summary>Registers a delegate to be called by UpdateFile at the end of its process.</summary>
        /// <param name="callback">The delegate.</param>
        public void RegisterUpdateFileCompletionCallBack(UpdateFileCompletionCallBack callback)
        {
            _updateFileCompletionCallBack = callback;
        }

        /// <summary>Method to Download file.</summary>
        /// <param name="objFileDistributionRequest">Distribute file Request.</param>
        /// <returns>Error if any.</returns>
        public string DownloadFile(DownloadFileDistributionRequest objFileDistributionRequest)
        {
            string strError = string.Empty;

            if (objFileDistributionRequest != null)
            {
                if (this.ValidateDownloadFileRequest(objFileDistributionRequest, out strError))
                {
                    lock (_fileDistributionRequestsLock)
                    {
                        if (!_fileDistributionRequests.ContainsKey(objFileDistributionRequest.RequestId))
                        {
                            _fileDistributionRequests.Add(objFileDistributionRequest.RequestId, objFileDistributionRequest);
                        }
                    }

                    this.CreateDownloadFolder(ref objFileDistributionRequest, out strError);
                    if (objFileDistributionRequest.Folder.FolderId > 0)
                    {
                        int taskid = this.CreateFileTransferTask(objFileDistributionRequest.Description, TransferType.TrainToGround, SystemFolderIDT2G, objFileDistributionRequest.Folder.FolderId, objFileDistributionRequest.StartDate, objFileDistributionRequest.ExpirationDate, objFileDistributionRequest.RecipientList, out strError);
                        if (taskid > 0)
                        {
                            linkTypeEnum lnkTypeEnum = T2GDataConverter.BuildLinkTypeEnum(objFileDistributionRequest.FileTransferMode);
                            this.StartTransferTask(taskid, objFileDistributionRequest.Priority, lnkTypeEnum, true, true, out strError);
                            objFileDistributionRequest.TaskId = taskid;
                            this.UpdateRequestList(objFileDistributionRequest);
                        }
                        else
                        {
                            return strError;
                        }
                    }
                    else
                    {
                        return strError;
                    }
                }
            }
            else
            {
                strError = "Invalid input parameter";
            }

            return strError;
        }

        /// <summary>Method to Download folder.</summary>
        /// <param name="objDownloadFolderRequest">Distribute file Request.</param>
        /// <returns>Error if any.</returns>
        public string DownloadFolder(DownloadFolderRequest objDownloadFolderRequest)
        {
            string strError = string.Empty;

            if (objDownloadFolderRequest != null)
            {
                if (this.ValidateDownloadFolderRequest(objDownloadFolderRequest, out strError))
                {
                    lock (_fileDistributionRequestsLock)
                    {
                        if (!_fileDistributionRequests.ContainsKey(objDownloadFolderRequest.RequestId))
                        {
                            _fileDistributionRequests.Add(objDownloadFolderRequest.RequestId, objDownloadFolderRequest);
                        }
                    }

                    if (objDownloadFolderRequest.Folder.FolderId > 0)
                    {
                        int taskid = this.CreateFileTransferTask(objDownloadFolderRequest.Description, TransferType.TrainToGround, objDownloadFolderRequest.SystemId, objDownloadFolderRequest.Folder.FolderId, objDownloadFolderRequest.StartDate, objDownloadFolderRequest.ExpirationDate, objDownloadFolderRequest.RecipientList, out strError);
                        if ((taskid > 0) && (strError.CompareTo(string.Empty) == 0))
                        {
                            linkTypeEnum lnkTypeEnum = T2GDataConverter.BuildLinkTypeEnum(objDownloadFolderRequest.FileTransferMode);
                            this.StartTransferTask(taskid, objDownloadFolderRequest.Priority, lnkTypeEnum, true, false, out strError);
                            objDownloadFolderRequest.TaskId = taskid;
                            this.UpdateRequestList(objDownloadFolderRequest);
                        }
                        else
                        {
                            return strError;
                        }
                    }
                    else
                    {
                        return strError;
                    }
                }
            }
            else
            {
                strError = "Invalid input parameter";
            }

            return strError;
        }

        /// <summary>Gets the request based on task id.</summary>
        /// <param name="requestId">input request id.</param>
        /// <returns>FileDistributionRequest</returns>
        public FileDistributionRequest GetFileDistributionRequestByRequestId(Guid requestId)
        {
            FileDistributionRequest request = null;

            lock (_fileDistributionRequestsLock)
            {
                _fileDistributionRequests.TryGetValue(requestId, out request);
            }

            return request;
        }

        /// <summary>Gets the request based on task id.</summary>
        /// <param name="taskId">input task id.</param>
        /// <returns>FileDistributionRequest</returns>
        public FileDistributionRequest GetFileDistributionRequestByTaskId(int taskId)
        {
            FileDistributionRequest request = null;

            lock (_fileDistributionRequestsLock)
            {
                foreach (FileDistributionRequest val in _fileDistributionRequests.Values)
                {
                    if (val.TaskId == taskId)
                    {
                        request = val;
                        break;
                    }
                }
            }

            return request;
        }

        /// <summary>Gets the request based on folder id.</summary>
        /// <param name="folderId">input folder id.</param>
        /// <returns>FileDistributionRequest</returns>
        internal FileDistributionRequest GetFileDistributionRequestByFolderId(int folderId)
        {
            FileDistributionRequest request = null;

            lock (_fileDistributionRequestsLock)
            {
                foreach (FileDistributionRequest val in _fileDistributionRequests.Values)
                {
                    if (val.Folder.FolderId == folderId)
                    {
                        request = val;
                        break;
                    }
                }
            }

            return request;
        }        

        /// <summary>Gets the request id based on task id.</summary>
        /// <param name="taskId">input task id.</param>
        /// <returns>returns request id else Guid.Empty.</returns>
        public Guid GetFileDistributionRequestIdByTaskId(int taskId)
        {
            FileDistributionRequest request = GetFileDistributionRequestByTaskId(taskId);
            return request != null ? request.RequestId : Guid.Empty;
        }

        /// <summary>Gets the request id based on folder id.</summary>
        /// <param name="folderId">input folder id.</param>
        /// <returns>returns request id else Guid.Empty.</returns>
        internal Guid GetFileDistributionRequestIdByFolderId(int folderId)
        {
            FileDistributionRequest request = GetFileDistributionRequestByFolderId(folderId);
            return request != null ? request.RequestId : Guid.Empty;
        }

        /// <summary>Gets the request event handler based on task id.</summary>
        /// <param name="taskId">input task id.</param>
        /// <returns>returns EventHandler else null.</returns>
        internal EventHandler<FileDistributionStatusArgs> GetFileDistributionEventByTaskId(int taskId)
        {
            FileDistributionRequest request = GetFileDistributionRequestByTaskId(taskId);
            return request != null ? request.OnFileDistributeNotification : null;            
        }

        /// <summary>Gets the request event handler based on request id.</summary>
        /// <param name="requestId">input rewuest id.</param>
        /// <returns>returns EventHandler else null.</returns>
        internal EventHandler<FileDistributionStatusArgs> GetFileDistributionEventByRequestId(Guid requestId)
        {
            FileDistributionRequest request = GetFileDistributionRequestByRequestId(requestId);
            return request != null ? request.OnFileDistributeNotification : null;            
        }            

        /// <summary>Download the file when the OnFile ReceivedNotificationEvent is raised.</summary>
        /// <param name="taskId">task id.</param>
        /// <returns>true or false.</returns>
        internal bool DownloadFile(int taskId)
        {
            string strError = string.Empty;
            bool lRes = false;
            
            FileDistributionRequest objFileDistributionRequest = GetFileDistributionRequestByTaskId(taskId);

            if (objFileDistributionRequest != null)
            {
                DownloadFolderRequest objDownloadFolderRequest = objFileDistributionRequest as DownloadFolderRequest;
                if (objDownloadFolderRequest != null)
                {
                    IFolderInfo objFolderInfo = GetFolderInformation(objDownloadFolderRequest.Folder.FolderId, out strError);
                    if ((objFolderInfo != null) && (strError.CompareTo(string.Empty) == 0))
                    {
                        objDownloadFolderRequest.Folderinfo = objFolderInfo;
                        FtpUtility.Download(ref objDownloadFolderRequest);
                        FileDistributionStatusArgs objFileDistributionStatusArgs = BuildFileDistributionStatusArgs(objDownloadFolderRequest.TaskId, TaskState.Completed, TaskPhase.Transfer, 0, 0, 100, 100, 100);
                        objFileDistributionStatusArgs.SetError = strError;
                        _notifierTarget.RaiseOnFileDistributeNotificationEvent(objFileDistributionStatusArgs, objDownloadFolderRequest.RequestId);
                        lRes = true;
                    }
                    else
                    {
                        lRes = false;
                    }
                }
                else
                {
                    lRes = false;
                }
            }
            else
            {
                lRes = true;
            }
            return lRes;
        }
        
        /// <summary>Mapping for the error message fron ftpStatus code.</summary>
        /// <param name="ftpStatusCode">ftpStatus code.</param>
        /// <returns>error message of the statuscode.</returns>
        private string MapFtpStatusCode(FtpStatusCode ftpStatusCode)
        {
            string strError = string.Empty;
            switch (ftpStatusCode)
            {
                case FtpStatusCode.Undefined: strError = "Included for completeness, this value is never returned by servers.";
                    break;
                case FtpStatusCode.RestartMarker: strError = "The response contains a restart marker reply. The text of the description that accompanies this status contains the user data stream marker and the server marker.";
                    break;
                case FtpStatusCode.ServiceTemporarilyNotAvailable: strError = "The service is not available now; try your request later.";
                    break;
                case FtpStatusCode.DataAlreadyOpen: strError = "The data connection is already open and the requested transfer is starting.";
                    break;
                case FtpStatusCode.OpeningData: strError = "The server is opening the data connection.";
                    break;
                case FtpStatusCode.CommandOK: strError = "The command completed successfully.";
                    break;
                case FtpStatusCode.CommandExtraneous: strError = "The command is not implemented by the server because it is not needed.";
                    break;
                case FtpStatusCode.DirectoryStatus:
                    break;
                case FtpStatusCode.FileStatus:
                    break;
                case FtpStatusCode.SystemType:
                    break;
                case FtpStatusCode.SendUserCommand:
                    break;
                case FtpStatusCode.ClosingControl:
                    break;
                case FtpStatusCode.ClosingData:
                    break;
                case FtpStatusCode.EnteringPassive: strError = "The server is entering passive mode";
                    break;
                case FtpStatusCode.LoggedInProceed:
                    break;
                case FtpStatusCode.ServerWantsSecureSession: strError = "The server accepts the authentication mechanism specified by the client, and the exchange of security data is complete";
                    break;
                case FtpStatusCode.FileActionOK:
                    break;
                case FtpStatusCode.PathnameCreated:
                    break;
                case FtpStatusCode.SendPasswordCommand: strError = "The server expects a password to be supplied.";
                    break;
                case FtpStatusCode.NeedLoginAccount: strError = "	The server requires a login account to be supplied.";
                    break;
                case FtpStatusCode.FileCommandPending: strError = "The requested file action requires additional information.";
                    break;
                case FtpStatusCode.ServiceNotAvailable: strError = "The service is not available.";
                    break;
                case FtpStatusCode.CantOpenData: strError = "The data connection cannot be opened.";
                    break;
                case FtpStatusCode.ConnectionClosed: strError = "The connection has been closed.";
                    break;
                case FtpStatusCode.ActionNotTakenFileUnavailableOrBusy: strError = "The requested action cannot be performed on the specified file because the file is not available or is being used.";
                    break;
                case FtpStatusCode.ActionAbortedLocalProcessingError: strError = "Error occurred that prevented the request action from completing.";
                    break;
                case FtpStatusCode.ActionNotTakenInsufficientSpace: strError = "The requested action cannot be performed because there is not enough space on the server.";
                    break;
                case FtpStatusCode.CommandSyntaxError: strError = "The command has a syntax error or is not a command recognized by the server";
                    break;
                case FtpStatusCode.ArgumentSyntaxError: strError = "Specifies that one or more command arguments has a syntax error.";
                    break;
                case FtpStatusCode.CommandNotImplemented: strError = "The command is not implemented by the FTP server.";
                    break;
                case FtpStatusCode.BadCommandSequence: strError = "The sequence of commands is not in the correct order.";
                    break;
                case FtpStatusCode.NotLoggedIn: strError = "Specifies that login information must be sent to the server";
                    break;
                case FtpStatusCode.AccountNeeded: strError = "Specifies that a user account on the server is required.";
                    break;
                case FtpStatusCode.ActionNotTakenFileUnavailable: strError = "The requested action cannot be performed on the specified file because the file is not available.";
                    break;
                case FtpStatusCode.ActionAbortedUnknownPageType: strError = "The requested action cannot be taken because the specified page type is unknown. Page types are described in RFC 959 Section 3.1.2.3";
                    break;
                case FtpStatusCode.FileActionAborted: strError = "The requested action cannot be performed.";
                    break;
                case FtpStatusCode.ActionNotTakenFilenameNotAllowed: strError = "The requested action cannot be performed on the specified file";
                    break;
                default: strError = "Included for completeness, this value is never returned by servers.";
                    break;
            }

            return strError;
        }

        /// <summary>Update the Dictionary that holds the request list.</summary>
        /// <param name="objFileDistributionRequest">File distribution request.</param>
        private void UpdateRequestList(FileDistributionRequest objFileDistributionRequest)
        {
            if (LogManager.IsTraceActive(TraceType.INFO))
            {
                LogManager.WriteLog(TraceType.INFO, "UpdateRequestList called, request id =" + objFileDistributionRequest.RequestId, "PIS.Ground.Core.T2G.T2GFileDistributionManager", null, EventIdEnum.GroundCore);
            }

            lock (_fileDistributionRequestsLock)
            {
                if (_fileDistributionRequests.ContainsKey(objFileDistributionRequest.RequestId))
                {
                    _fileDistributionRequests[objFileDistributionRequest.RequestId] = objFileDistributionRequest;
                    LogManager.WriteLog(TraceType.DEBUG, "UpdateRequestList updated distribution requests", "PIS.Ground.Core.T2G.T2GFileDistributionManager", null, EventIdEnum.GroundCore);
                }
            }
        }

        /// <summary>Validate the file download request.</summary>
        /// <param name="objFileDistributionRequest">file distribution request.</param>
        /// <param name="error">error if any.</param>
        /// <returns>valid or not.</returns>
        private bool ValidateDownloadFileRequest(DownloadFileDistributionRequest objFileDistributionRequest, out string error)
        {
            if (objFileDistributionRequest.Folder.FolderName == string.Empty)
            {
                error = Resources.LogDownloadFolderEmpty;
                return false;
            }
            else if (objFileDistributionRequest.FtpServerIP == string.Empty)
            {
                error = Resources.LogDownloadFtpServerIpInvalid;
                return false;
            }
            else if (objFileDistributionRequest.FtpPortNumber == 0)
            {
                error = Resources.LogDownloadFtpServerPortInvalid;
                return false;
            }
            else if (objFileDistributionRequest.FtpUserName == string.Empty)
            {
                error = Resources.LogDownloadFtpServerUserNameInvalid;
                return false;
            }
            else if (objFileDistributionRequest.FtpPassword == string.Empty)
            {
                error = Resources.LogDownloadFtpServerPwdInvalid;
                return false;
            }
            else if (objFileDistributionRequest.FtpDirectory == string.Empty)
            {
                error = Resources.LogDownloadFtpServerDirectoryInvalid;
                return false;
            }
            else if (objFileDistributionRequest.Folder.NbFiles <= 0)
            {
                error = Resources.LogDownloadFileListError;
                return false;
            }
            else if (objFileDistributionRequest.RequestId == null || objFileDistributionRequest.RequestId.ToString() == new Guid().ToString())
            {
                error = Resources.LogUploadRequestIDError;
                return false;
            }
            else if (objFileDistributionRequest.ExpirationDate.Date < DateTime.Today.Date)
            {
                error = Resources.LogDownloadExpirationDateError;
                return false;
            }
            else
            {
                error = string.Empty;
                return true;
            }
        }

        /// <summary>Validate the file download request.</summary>
        /// <param name="objDownloadFolderRequest">Distribute file Request.</param>
        /// <param name="error">error if any.</param>
        /// <returns>valid or not.</returns>
        private bool ValidateDownloadFolderRequest(DownloadFolderRequest objDownloadFolderRequest, out string error)
        {
            if (objDownloadFolderRequest != null)
            {
                if (objDownloadFolderRequest.DownloadFolderPath == string.Empty)
                {
                    error = Resources.LogDownloadFolderPath;
                    return false;
                }
                else if (objDownloadFolderRequest.RequestId == null || objDownloadFolderRequest.RequestId.ToString() == new Guid().ToString())
                {
                    error = Resources.LogUploadRequestIDError;
                    return false;
                }
                else
                {
                    error = string.Empty;
                    return true;
                }
            }
            else
            {
                error = "Invalid request";
                return false;
            }
        }

        /// <summary>Validate the file distribution request.</summary>
        /// <param name="objFileDistributionRequest">file distribution request.</param>
        /// <param name="error">error if any.</param>
        /// <returns>valid or not.</returns>
        private bool ValidateUploadFileRequest(FileDistributionRequest objFileDistributionRequest, out string error)
        {
            if (objFileDistributionRequest.Folder.NbFiles <= 0)
            {
                error = Resources.LogUploadFileListError;
                return false;
            }
            else if (objFileDistributionRequest.Priority < 0 || objFileDistributionRequest.Priority > 32)
            {
                error = Resources.LogUploadPriorityError;
                return false;
            }
            else if (objFileDistributionRequest.RequestId == null || objFileDistributionRequest.RequestId.ToString() == new Guid().ToString())
            {
                error = Resources.LogUploadRequestIDError;
                return false;
            }
            else if (objFileDistributionRequest.RecipientList.Count < 0)
            {
                error = Resources.LogUploadRecpientCountError;
                return false;
            }
            else if (objFileDistributionRequest.RecipientList.Count > 0)
            {
                foreach (RecipientId recipient in objFileDistributionRequest.RecipientList)
                {
                    if (string.IsNullOrEmpty(recipient.SystemId) && string.IsNullOrEmpty(recipient.MissionId))
                    {
                        error = Resources.LogUploadRecpientSystemIDError;
                        return false;
                    }
                }
                error = string.Empty;
                return true;
            }
            else
            {
                error = string.Empty;
                return true;
            }
        }

        /// <summary>Method to Upload folder.</summary>
        /// <param name="objUploadFileDistributionRequest">Distribute file upload Request.</param>
        /// <param name="strError">error that has occured.</param>
        private void CreateUploadFolder(ref UploadFileDistributionRequest objUploadFileDistributionRequest, out string strError)
        {
            strError = string.Empty;
            try
            {
                PIS.Ground.Core.T2G.WebServices.FileTransfer.pathList objPathList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.pathList();
                foreach (IRemoteFileClass remoteFile in objUploadFileDistributionRequest.Folder.FolderFilesList)
                {
                    if (remoteFile.Exists)
                    {
                        PIS.Ground.Core.T2G.WebServices.FileTransfer.filePathStruct objfilePathStruct = new PIS.Ground.Core.T2G.WebServices.FileTransfer.filePathStruct();
                        if (remoteFile.Size > 0)
                        {
                            objfilePathStruct.path = "\\" + remoteFile.FileName;
                            objfilePathStruct.size = remoteFile.Size;
                            try
                            {
                                objfilePathStruct.checksum = Convert.ToUInt32(remoteFile.CRC, 16);
                            }
                            catch (Exception ex)
                            {
                                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.CreateUploadFolder", ex, EventIdEnum.GroundCore);
                                strError = string.Format(CultureInfo.CurrentCulture, Resources.LogUploadFileNotExists, remoteFile.FilePath);
                            }
                            objPathList.Add(objfilePathStruct);
                        }
                        else
                        {
                            LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.LogUploadFileNotExists, remoteFile.FilePath), "PIS.Ground.Core.T2G.T2GFileDistributionManager.CreateUploadFolder", null, EventIdEnum.GroundCore);
                            strError = string.Format(CultureInfo.CurrentCulture, Resources.LogUploadFileNotExists, remoteFile.FilePath);
                        }
                    }
                    else
                    {
                        LogManager.WriteLog(TraceType.ERROR, string.Format(CultureInfo.CurrentCulture, Resources.LogUploadFileEmpty, remoteFile.FilePath), "PIS.Ground.Core.T2G.T2GFileDistributionManager.CreateUploadFolder", null, EventIdEnum.GroundCore);
                        strError = string.Format(CultureInfo.CurrentCulture, Resources.LogUploadFileEmpty, remoteFile.FilePath);
                    }
                }

                if (strError != string.Empty)
                {
                    return;
                }

                string strftpIP = string.Empty;
                ushort ftpport;
                string username = string.Empty;
                string pwd = string.Empty;
                string dir = string.Empty;
                int folderId;
                DateTime dtCurrentTime = DateTime.Now;
                bool isFatalError = false;
                while (DateTime.Now.Subtract(dtCurrentTime).TotalMinutes < TimeOut && !isFatalError)
                {
                    try
                    {
                        using (FileTransferPortTypeClient objFileTransferPortTypeClient = new FileTransferPortTypeClient())
                        {
                            folderId = objFileTransferPortTypeClient.createUploadFolder(_sessionData.SessionId, objUploadFileDistributionRequest.Folder.FolderName, objUploadFileDistributionRequest.ExpirationDate, objPathList, objUploadFileDistributionRequest.Compression, out strftpIP, out ftpport, out username, out pwd, out dir);
                            objUploadFileDistributionRequest.Folder.FolderId = folderId;
                            objUploadFileDistributionRequest.FtpDirectory = dir;
                            objUploadFileDistributionRequest.FtpPassword = pwd;
                            objUploadFileDistributionRequest.FtpPortNumber = ftpport;
                            objUploadFileDistributionRequest.FtpServerIP = strftpIP;
                            objUploadFileDistributionRequest.FtpUserName = username;
                            strError = string.Empty;
                            return;
                        }
                    }
                    catch (FaultException ex)
                    {
                        LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.CreateUploadFolder", ex, EventIdEnum.GroundCore);
                        strError = ex.Code.Name;

                        FileDistributionStatusArgs objFileDistributionStatusArgs = BuildFileDistributionStatusArgs(0, TaskState.Error, TaskPhase.Transfer, 0, 0, 0, 0, 0);
                        objFileDistributionStatusArgs.SetError = strError;
                        _notifierTarget.RaiseOnFileDistributeNotificationEvent(objFileDistributionStatusArgs, objUploadFileDistributionRequest.RequestId);
                        if (!isRecoverableT2GError(strError))
                        {
                            isFatalError = true;
                        }

                    }
                    catch (System.Web.Services.Protocols.SoapException ex)
                    {
                        LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.CreateUploadFolder", ex, EventIdEnum.GroundCore);
                        strError = ex.Code.Name;

                    }
                    catch (Exception ex)
                    {
                        LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.CreateUploadFolder", ex, EventIdEnum.GroundCore);
                        strError = ex.Message;
                        isFatalError = true;
                    }
                    
                        if (strError != string.Empty)
                        {
                            FileDistributionStatusArgs objFileDistributionStatusArgs = BuildFileDistributionStatusArgs(0, TaskState.Error, TaskPhase.Transfer, 0, 0, 0, 0, 0);
                            objFileDistributionStatusArgs.SetError = strError;
                            _notifierTarget.RaiseOnFileDistributeNotificationEvent(objFileDistributionStatusArgs, objUploadFileDistributionRequest.RequestId);
                        }

                    if (!isFatalError)
                    {
                        Thread.Sleep(PollingTime);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.CreateUploadFolder", ex, EventIdEnum.GroundCore);
                strError = ex.Message;
            }
        }

        /// <summary>Method to Download folder.</summary>
        /// <param name="objDownloadFileDistributionRequest">Distribute file Download Request.</param>
        /// <param name="strError">error that has occured.</param>
        private void CreateDownloadFolder(ref DownloadFileDistributionRequest objDownloadFileDistributionRequest, out string strError)
        {
            strError = string.Empty;
            DateTime dtCurrentTime = DateTime.Now;
            while (DateTime.Now.Subtract(dtCurrentTime).TotalMinutes < TimeOut)
            {
                try
                {
                    using (FileTransferPortTypeClient objFileTransferPortTypeClient = new FileTransferPortTypeClient())
                    {
                        int folderId = objFileTransferPortTypeClient.createDownloadFolder(
                            _sessionData.SessionId,
                            objDownloadFileDistributionRequest.Folder.FolderName,
                            objDownloadFileDistributionRequest.ExpirationDate,
                            objDownloadFileDistributionRequest.FtpServerIP,
                            objDownloadFileDistributionRequest.FtpPortNumber,
                            objDownloadFileDistributionRequest.FtpUserName,
                            objDownloadFileDistributionRequest.FtpPassword,
                            objDownloadFileDistributionRequest.FtpDirectory,
                            objDownloadFileDistributionRequest.Folder.FilenameList as filePathList,
                            objDownloadFileDistributionRequest.Compression);

                        objDownloadFileDistributionRequest.Folder.FolderId = folderId;
                        return;
                    }
                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.CreateUploadFolder", ex, EventIdEnum.GroundCore);
                    strError = ex.Code.Name;
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "Ground.Core.T2G.T2GClient.CreateUploadFolder", ex, EventIdEnum.GroundCore);
                    strError = ex.Message;
                    return;
                }
                Thread.Sleep(PollingTime);
            }
        }               

        /// <summary>Gets the information of the folder in T2G server.</summary>
        /// <param name="intFolderId">folder id.</param>
        /// <param name="strError">error that has occured.</param>
        /// <returns>folder information or null if any error.</returns>
        public IFolderInfo GetFolderInformation(int intFolderId, out string strError)
        {
            strError = string.Empty;
            DateTime dtStartTime = DateTime.Now;
            while (DateTime.Now.Subtract(dtStartTime).TotalMinutes < TimeOut)
            {
                try
                {
                    var lGetFolderInfoRequest = new getFolderInfoInput();
                    lGetFolderInfoRequest.Body = new getFolderInfoInputBody();
                    lGetFolderInfoRequest.Body.folderId = intFolderId;
                    lGetFolderInfoRequest.Body.sessionId = _sessionData.SessionId;

                    getFolderInfoOutput lGetFolderInfoOutput = _fileTransferPort.getFolderInfo(lGetFolderInfoRequest);
                    IFolderInfo objFolderInfo = new FolderInfo(lGetFolderInfoOutput.Body.fileList, lGetFolderInfoOutput.Body.publicationList, lGetFolderInfoOutput.Body.ftpServerIP, lGetFolderInfoOutput.Body.ftpPortNumber, lGetFolderInfoOutput.Body.ftpUserName, lGetFolderInfoOutput.Body.ftpPassword, lGetFolderInfoOutput.Body.folderInfo);
     
                    return objFolderInfo;
                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.GetFolderInformation", ex, EventIdEnum.GroundCore);
                    strError = ex.Code.Name;
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "Ground.Core.T2G.T2GFileDistributionManager.GetFolderInformation", ex, EventIdEnum.GroundCore);
                    strError = ex.Message;
                    return null;
                }
                Thread.Sleep(PollingTime);
            }
            return null;
        }

        /// <summary>Enumerate folders on T2GServer.</summary>
        /// <param name="strApplicationId">Application ID.</param>
        /// <param name="enumFolderType">Type of folder.</param>
        /// <param name="ushortEnumPos">Page of the folder enumeration.</param>
        /// <param name="boolEndOfEnum">Is last page of enum.</param>
        /// <param name="strError">error that occurs in the process of executing this function.</param>
        /// <returns>.</returns>
        public folderList EnumFolders(string strApplicationId, PIS.Ground.Core.T2G.WebServices.FileTransfer.folderTypeEnum enumFolderType, ushort ushortEnumPos, out bool boolEndOfEnum, out string strError)
        {
            strError = string.Empty;
            DateTime dtStartTime = DateTime.Now;
            boolEndOfEnum = true;
            while (DateTime.Now.Subtract(dtStartTime).TotalMinutes < TimeOut)
            {
                try
                {
                    var lEnumFoldersRequest = new enumFoldersInput();
                    lEnumFoldersRequest.Body = new enumFoldersInputBody();
                    lEnumFoldersRequest.Body.applicationId = strApplicationId;
                    lEnumFoldersRequest.Body.sessionId = _sessionData.SessionId;
                    lEnumFoldersRequest.Body.folderType = enumFolderType;
                    lEnumFoldersRequest.Body.enumPos = ushortEnumPos;

                    enumFoldersOutput lEnumFoldersOutput = _fileTransferPort.enumFolders(lEnumFoldersRequest);
                    boolEndOfEnum = lEnumFoldersOutput.Body.endOfEnum;

                    return lEnumFoldersOutput.Body.folderList;
                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.EnumFolders", ex, EventIdEnum.GroundCore);
                    strError = ex.Code.Name;
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "Ground.Core.T2G.T2GClient.EnumFolders", ex, EventIdEnum.GroundCore);
                    strError = ex.Message;
                    return null;
                }
                Thread.Sleep(PollingTime);
            }
            return null;
        }

        /// <summary>Gets the information of the folder in T2G server.</summary>
        /// <param name="intFolderId">folder id.</param>
        /// <param name="pSystemId">Identifier for the system.</param>
        /// <param name="pApplicationId">Identifier for the application.</param>
        /// <param name="strError">error that has occured.</param>
        /// <returns>folder information or null if any error.</returns>
        public IFolderInfo GetRemoteFolderInformation(int intFolderId, string pSystemId, string pApplicationId, out string strError)
        {
            strError = string.Empty;
            DateTime dtStartTime = DateTime.Now;
            LogManager.WriteLog(TraceType.INFO, "GetRemoteFolderInformation called:" + intFolderId + "," + pSystemId + "," + pApplicationId, "Ground.Core.T2G.T2GClient.GetRemoteFolderInformation", null, EventIdEnum.GroundCore);

            while (DateTime.Now.Subtract(dtStartTime).TotalMinutes < TimeOut)
            {
                try
                {
                    using (FileTransferPortTypeClient objFileTransferPortTypeClient = new FileTransferPortTypeClient())
                    {
                        ushort lPageOffset = 0;
                        bool lEndOfEnum = false;
                        IFolderInfo objFolderInfo = null;
                        while (!lEndOfEnum && objFolderInfo == null)
                        {
                            publicFolderList objT2GFolderInfoList = objFileTransferPortTypeClient.enumPublicFolders(_sessionData.SessionId, pSystemId, pApplicationId, TimeOut, lPageOffset, out lEndOfEnum);
                            
                            foreach (publicFolderInfoStruct lObjFolderInfo in objT2GFolderInfoList)
                            {
                                if (lObjFolderInfo.folderId == intFolderId)
                                {
                                    objFolderInfo = new FolderInfo(lObjFolderInfo.folderId, lObjFolderInfo.name, lObjFolderInfo.applicationId, lObjFolderInfo.expirationDate, lObjFolderInfo.totalFilesSize);
                                    break;
                                }
                            }
                            lPageOffset++;

                        }
                        return objFolderInfo;
                    }
                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.GetRemoteFolderInformation", ex, EventIdEnum.GroundCore);
                    strError = ex.Code.Name;
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.GetRemoteFolderInformation", ex, EventIdEnum.GroundCore);
                    strError = ex.Message;
                    return null;
                }
                Thread.Sleep(PollingTime);
            }
            LogManager.WriteLog(TraceType.ERROR, "GetRemoteFolderInformation timeout!", "Ground.Core.T2G.T2GClient.GetRemoteFolderInformation", null, EventIdEnum.GroundCore);
            return null;
        }

        /// <summary>Create transfer task.</summary>
        /// <param name="strDescription">description of the transfer.</param>
        /// <param name="transferType">type of transfer.</param>
        /// <param name="folderSystemId">system id.</param>
        /// <param name="intFolderId">folder id.</param>
        /// <param name="startDt">Start date of transfer.</param>
        /// <param name="expirationDt">Expiration date of transfer.</param>
        /// <param name="pRecipientIdList">list of recpient system.</param>
        /// <param name="strError">error that has occured.</param>
        /// <returns>Task id.</returns>
        public int CreateFileTransferTask(string strDescription, TransferType transferType, string folderSystemId, int intFolderId, DateTime startDt, DateTime expirationDt, List<RecipientId> pRecipientIdList, out string strError)
        {
            strError = string.Empty;
            DateTime dtCurrentTime = DateTime.Now;
            bool isFatalError = false;
            while (DateTime.Now.Subtract(dtCurrentTime).TotalMinutes < TimeOut && !isFatalError)
            {
                try
                {
                    transferTypeEnum transferTypeEnum = T2GDataConverter.BuildTransferTypeEnum(transferType);
                    recipientIdList recipientList = new recipientIdList();
                    
                    foreach (RecipientId objRecipient in pRecipientIdList)
                    {
                        recipientIdStruct objrecipientIdStruct = new recipientIdStruct();
                        objrecipientIdStruct.applicationId = objRecipient.ApplicationId;
                        objrecipientIdStruct.missionId = objRecipient.MissionId;
                        objrecipientIdStruct.systemId = objRecipient.SystemId;
                        recipientList.Add(objrecipientIdStruct);
                    }

                    var lCreateTransferTaskRequest = new createTransferTaskInput();
				    lCreateTransferTaskRequest.Body = new createTransferTaskInputBody();
                    lCreateTransferTaskRequest.Body.sessionId = _sessionData.SessionId;
                    lCreateTransferTaskRequest.Body.description = strDescription;
				    lCreateTransferTaskRequest.Body.transferType = transferTypeEnum;
				    lCreateTransferTaskRequest.Body.folderSystemId = folderSystemId;
				    lCreateTransferTaskRequest.Body.folderId = intFolderId;
                    lCreateTransferTaskRequest.Body.startDate = startDt;
                    lCreateTransferTaskRequest.Body.expirationDate = expirationDt;
                    lCreateTransferTaskRequest.Body.recipientIdList = recipientList;

                    createTransferTaskOutput lcreateTransferTaskOutput = _fileTransferPort.createTransferTask(lCreateTransferTaskRequest);

                    return lcreateTransferTaskOutput.Body.taskId;
                }
                catch (FaultException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.CreateFileTransferTask", ex, EventIdEnum.GroundCore);
                    strError = ex.Code.Name;
                    isFatalError = !isRecoverableT2GError(strError);
                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.CreateFileTransferTask", ex, EventIdEnum.GroundCore);
                    strError = ex.Code.Name;
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "Ground.Core.T2G.T2GClient.CreateFileTransferTask", ex, EventIdEnum.GroundCore);
                    strError = ex.Message;
                    return 0;
                }

                if (!isFatalError)
                {
                Thread.Sleep(PollingTime);
            }
            }
            return 0;
        }

        /// <summary>Start the transfer task.</summary>
        /// <param name="intTaskId">task id.</param>
        /// <param name="priority">priority of the transfer.</param>
        /// <param name="linkType">link type.</param>
        /// <param name="sendProgressNotification">Required the progress notification.</param>
        /// <param name="automaticallyStop">automatically stop.</param>
        /// <param name="strError">error that has occured.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool StartTransferTask(int intTaskId, sbyte priority, linkTypeEnum linkType, bool sendProgressNotification, bool automaticallyStop, out string strError)
        {
            strError = string.Empty;
            DateTime dtCurrentTime = DateTime.Now;
            while (DateTime.Now.Subtract(dtCurrentTime).TotalMinutes < TimeOut)
            {
                try
                {
                    _fileTransferPort.startTransfer(_sessionData.SessionId, intTaskId, priority, linkType, sendProgressNotification, automaticallyStop);
                    return true;
                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.StartTransferTask", ex, EventIdEnum.GroundCore);
                    strError = ex.Code.Name;
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "Ground.Core.T2G.T2GClient.StartTransferTask", ex, EventIdEnum.GroundCore);
                    strError = ex.Message;
                    return false;
                }
                Thread.Sleep(PollingTime);
            }
            return false;
        }

        /// <summary>Cancel TransferTask.</summary>
        /// <param name="intTaskID">task id.</param>
        /// <param name="strError">error that has occured.</param>
        public void CancelTransferTask(int intTaskID, out string strError)
        {
            strError = string.Empty;
            DateTime dtCurrentTime = DateTime.Now;
            while (DateTime.Now.Subtract(dtCurrentTime).TotalMinutes < TimeOut)
            {
                try
                {
                    using (FileTransferPortTypeClient objFileTransferPortTypeClient = new FileTransferPortTypeClient())
					{
                        objFileTransferPortTypeClient.cancelTransfer(_sessionData.SessionId, intTaskID);
					}
					strError = string.Empty;
                    return;
                }

                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GFileDistributionManager.CreateUploadFolder", ex, EventIdEnum.GroundCore);
                    strError = ex.Code.Name;
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "Ground.Core.T2G.T2GClient.CancelTransferTask", ex, EventIdEnum.GroundCore);
                    strError = ex.Message;
                    return;
                }
                Thread.Sleep(PollingTime);
            }
        }

        /// <summary>Returns the file distribution manager error code by its description.</summary>
        /// <param name="errorDescription">Error description.</param>
        /// <returns>Error code</returns>
        public T2GFileDistributionManagerErrorEnum GetErrorCodeByDescription(string errorDescription)
        {
            T2GFileDistributionManagerErrorEnum errorCode = T2GFileDistributionManagerErrorEnum.eT2GFD_NoError;
            
            if (string.IsNullOrEmpty(errorDescription) != true)
            {
                int index = errorDescription.IndexOf("F0");

                try
                {
                    string errorCodeString = errorDescription.Substring(index, 5);

                    switch (errorCodeString)
                    {
                        case "F0308":
                            errorCode = T2GFileDistributionManagerErrorEnum.eT2GFD_BadTaskId;
                            break;

                        default:
                            errorCode = T2GFileDistributionManagerErrorEnum.eT2GFD_Other;
                            break;
                    }
                }                
                catch
                {
                    errorCode = T2GFileDistributionManagerErrorEnum.eT2GFD_Other;   
                }
            }

            return errorCode;
        }   
        
        /// <summary>Build FileDistributionStatusArgs.</summary>
        /// <param name="taskId">The task id.</param>
        /// <param name="taskState">The task state.</param>
        /// <param name="taskPhase">The task phase.</param>
        /// <param name="activeFileTransferCount">File transfer count.</param>
        /// <param name="errorCount">The error count.</param>
        /// <param name="acquisitionCompletionPercent">Acquisition complete percentage.</param>
        /// <param name="transferCompletionPercent">Transfer complete percentage.</param>
        /// <param name="distributionCompletionPercent">Distribute complete percentage.</param>
        /// <returns>FileDistributionStatusArgs object.</returns>
        internal FileDistributionStatusArgs BuildFileDistributionStatusArgs(
            int taskId,
            TaskState taskState,
            TaskPhase taskPhase,
            ushort activeFileTransferCount,
            ushort errorCount,
            sbyte acquisitionCompletionPercent,
            sbyte transferCompletionPercent,
            sbyte distributionCompletionPercent)
        {
            FileDistributionStatusArgs objFileDistributionStatusArgs = new FileDistributionStatusArgs();
            
            if (taskId != 0)
            {
                Guid requestid = GetFileDistributionRequestIdByTaskId(taskId);
                if (requestid != Guid.Empty)
                {
                    objFileDistributionStatusArgs.TaskId = taskId;
                    objFileDistributionStatusArgs.RequestId = requestid;
                    objFileDistributionStatusArgs.ActiveFileTransferCount = activeFileTransferCount;
                    objFileDistributionStatusArgs.AcquisitionCompletionPercent = acquisitionCompletionPercent;
                    objFileDistributionStatusArgs.TransferCompletionPercent = transferCompletionPercent;
                    objFileDistributionStatusArgs.ErrorCount = errorCount;
                    objFileDistributionStatusArgs.DistributionCompletionPercent = distributionCompletionPercent;
                    objFileDistributionStatusArgs.TaskStatus = taskState;
                    objFileDistributionStatusArgs.CurrentTaskPhase = taskPhase;
                }
            }

            return objFileDistributionStatusArgs;
        }

        /// <summary>Build FilePublicationNotificationArgs.</summary>
        /// <param name="folderId">Folder id.</param>
        /// <param name="completionPercent">Complete percentage.</param>
        /// <param name="acquisitionState">Acqisition state.</param>
        /// <param name="error">Error if any.</param>
        /// <returns>FilePublicationNotificationArgs object.</returns>
        internal FilePublicationNotificationArgs BuildFilePublicationNotificationArgs(int folderId, sbyte completionPercent, PIS.Ground.Core.T2G.WebServices.Notification.acquisitionStateEnum acquisitionState, string error)
        {
            FilePublicationNotificationArgs objFilePublicationNotificationArgs = null;
            
            if (folderId != 0)
            {
                objFilePublicationNotificationArgs = new FilePublicationNotificationArgs();

                Guid requestid = GetFileDistributionRequestIdByFolderId(folderId);

                if (requestid != Guid.Empty)
                {
                    switch (acquisitionState)
                    {
                        case PIS.Ground.Core.T2G.WebServices.Notification.acquisitionStateEnum.acquisitionError: objFilePublicationNotificationArgs.PublicationAcquisitionState = AcquisitionState.AcquisitionError;
                            break;
                        case PIS.Ground.Core.T2G.WebServices.Notification.acquisitionStateEnum.acquisitionStarted: objFilePublicationNotificationArgs.PublicationAcquisitionState = AcquisitionState.AcquisitionStarted;
                            break;
                        case PIS.Ground.Core.T2G.WebServices.Notification.acquisitionStateEnum.acquisitionStopped: objFilePublicationNotificationArgs.PublicationAcquisitionState = AcquisitionState.AcquisitionStopped;
                            break;
                        case PIS.Ground.Core.T2G.WebServices.Notification.acquisitionStateEnum.acquisitionSuccess: objFilePublicationNotificationArgs.PublicationAcquisitionState = AcquisitionState.AcquisitionSuccess;
                            break;
                        case PIS.Ground.Core.T2G.WebServices.Notification.acquisitionStateEnum.notAcquired: objFilePublicationNotificationArgs.PublicationAcquisitionState = AcquisitionState.NotAcquired;
                            break;
                        default: objFilePublicationNotificationArgs.PublicationAcquisitionState = AcquisitionState.AcquisitionStarted;
                            break;
                    }

                    objFilePublicationNotificationArgs.Error = error;
                    objFilePublicationNotificationArgs.CompletionPercent = completionPercent;
                    objFilePublicationNotificationArgs.FolderId = folderId;
                }
            }

            return objFilePublicationNotificationArgs;
        }

        /// <summary>Build FilePublishedNotificationArgs.</summary>
        /// <param name="folderId">Folder id.</param>
        /// <param name="systemId">System id.</param>
        /// <returns>FilePublishedNotificationArgs object.</returns>
        internal FilePublishedNotificationArgs BuildFilePublishedNotificationArgs(int folderId, string systemId)
        {
            FilePublishedNotificationArgs objFilePublishedNotificationArgs = new FilePublishedNotificationArgs();
            
            if (folderId != 0)
            {
                Guid requestid = GetFileDistributionRequestIdByFolderId(folderId);

                if (requestid != Guid.Empty)
                {
                    objFilePublishedNotificationArgs.FolderId = folderId;
                    objFilePublishedNotificationArgs.SystemId = systemId;
                    objFilePublishedNotificationArgs.RequestId = requestid;
                }
                else
                {
                    objFilePublishedNotificationArgs.FolderId = folderId;
                    objFilePublishedNotificationArgs.SystemId = systemId;
                    objFilePublishedNotificationArgs.RequestId = new Guid();
                }
            }

            return objFilePublishedNotificationArgs;
        }

        /// <summary>
        /// Determinate if a T2G error code is a recoverable error or not for creating folder or transferring file.
        /// </summary>
        /// <param name="errorCode">The error code to evaluate</param>
        /// <returns>true if the error is recoverable, false otherwise.</returns>
        internal static bool isRecoverableT2GError(string errorCode)
        {
            bool isRecoverable = (StringComparer.OrdinalIgnoreCase.Equals("F0103", errorCode) /* Invalid session id */
                || StringComparer.OrdinalIgnoreCase.Equals("F0100", errorCode) /* unexpected error */
                || StringComparer.OrdinalIgnoreCase.Equals("F030E", errorCode) /* Disk full */
                || StringComparer.OrdinalIgnoreCase.Equals("F0306", errorCode) /* Cannot transfer to mission */
                || StringComparer.OrdinalIgnoreCase.Equals("F0302", errorCode) /* Cannot create directory error */
                );

            return isRecoverable;
        }
    }
}
