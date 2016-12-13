//---------------------------------------------------------------------------------------------------
// <copyright file="DataPackageService.svc.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.RemoteDataStore;
using PIS.Ground.Common;
using PIS.Ground.Core.Utility;

// For unit test purpose
[assembly: InternalsVisibleTo("DataPackageTests")]

namespace PIS.Ground.DataPackage
{
	[CreateOnDispatchService(typeof(DataPackageService))]
	[ServiceBehavior(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/")]
	/// <summary>Data package service implementation.</summary>
	public class DataPackageService : IDataPackageService
	{
		#region fields

		/// <summary>Identifier for event subscription.</summary>
		private const string SubscriberId = "PIS.Ground.DataPackage.DataPackageService";

		private static volatile bool _initialized = false;

		private static object _initializationLock = new object();
        
        private static volatile bool _stopRequested;

		/// <summary>Maximum end date for enumerating transfer tasks on the T2G.
		/// DateTime.MaxValue cannot be used as the
		/// enumeration always return an empty list.</summary>
		private static DateTime _MaxTransferDate = new DateTime(3001, 1, 1);

		private static string _remoteDateStoreUrl = string.Empty;

		private static Object _lock = new Object();

		private static List<RequestCompletedEvent> _requestCompletedEvents = new List<RequestCompletedEvent>();

		/// <summary>The object to serialize a string list in XML.</summary>
		private static System.Xml.Serialization.XmlSerializer _stringListXmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(List<string>));

		/// <summary>The object to serialize a string in XML.</summary>
		private static System.Xml.Serialization.XmlSerializer _stringXmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(string));


		// Baseline Status Update
		//
		private static AutoResetEvent _baselineStatusEvent = new AutoResetEvent(false);
		private static Thread _baselineStatusThread = null;
		private static Queue<SystemInfo> _baselineStatusSystemInformationQueue = new Queue<SystemInfo>();
		private static Object _baselineStatusLock = new Object();

		private static IT2GManager _t2gManager = null;

		private static ISessionManager _sessionManager = null;

		private static INotificationSender _notificationSender = null;

        private static BaselineStatusUpdater _baselineStatusUpdater = null;

		// Dictionary where the key is the pair(requestId, trainId) and the value is the Baseline Version.
		private static Dictionary<KeyValuePair<Guid, string>, string> _baselineVersionDic = new Dictionary<KeyValuePair<Guid, string>, string>();

		private const int _timeInterval = 5;

		private static object _updateBaseLineTimerState = new object();

		private static Timer _updateBaseLineTimer;

		private static Dictionary<string, DistributingElementData> _distributingElementDic = new Dictionary<string, DistributingElementData>();

		// List of data packages that are being transfered to T2G
		private static List<PackageParams> _usedPackages = new List<PackageParams>();

		/// <summary>The request factory.</summary>
		private static RequestMgt.IRequestContextFactory _requestFactory = null;

		/// <summary>Manager for request.</summary>
		private static RequestMgt.IRequestManager _requestManager = null;

		/// <summary>The remote data store factory.</summary>
		private static RemoteDataStoreFactory.IRemoteDataStoreFactory _remoteDataStoreFactory = null;

		#endregion

		#region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPackageService"/> class.
        /// </summary>
		public DataPackageService()
		{
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "DataPackageService";
            }

			Initialize();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPackageService" /> class.
        /// </summary>
        /// <param name="sessionManager">The session manager.</param>
        /// <param name="notificationSender">The notification sender.</param>
        /// <param name="t2gManager">The T2G manager.</param>
        /// <param name="requestsFactory">The requests factory.</param>
        /// <param name="remoteDataStoreFactory">The remote data store factory.</param>
        /// <param name="requestManager">The request manager.</param>
        /// <param name="baselineStatusUpdater">The baseline status updater.</param>
        /// <remarks>
        /// Available for automated testing.
        /// </remarks>
        protected DataPackageService(
            ISessionManager sessionManager,
            INotificationSender notificationSender,
            IT2GManager t2gManager,
            RequestMgt.IRequestContextFactory requestsFactory,
            RemoteDataStoreFactory.IRemoteDataStoreFactory remoteDataStoreFactory,
            RequestMgt.IRequestManager requestManager,
            BaselineStatusUpdater baselineStatusUpdater)
        {
            Initialize(sessionManager, notificationSender, t2gManager, requestsFactory, remoteDataStoreFactory, requestManager, baselineStatusUpdater, true);
        }

		#endregion

        #region Public static properties

        /// <summary>
        /// Gets the remote data store factory.
        /// </summary>
        public static RemoteDataStoreFactory.IRemoteDataStoreFactory RemoteDataStoreFactory
        {
            get
            {
                return _remoteDataStoreFactory;
            }
        }

        #endregion

        #region private static methods

        public static void Initialize()
		{
			if (!_initialized)
			{
				lock (_initializationLock)
				{
					if (!_initialized)
					{
						try
						{
							_sessionManager = new SessionManager();

							_notificationSender = new NotificationSender(_sessionManager);

							_t2gManager = T2GManagerContainer.T2GManager;
                            _baselineStatusUpdater = new BaselineStatusUpdater();
                            _remoteDataStoreFactory = new RemoteDataStoreFactory.RemoteDataStoreFactory();

							_remoteDateStoreUrl = ConfigurationSettings.AppSettings["RemoteDataStoreUrl"];
                            _requestManager = new RequestMgt.RequestManager();
                            _requestFactory = new RequestMgt.RequestContextFactory(_t2gManager, _remoteDataStoreFactory, _baselineStatusUpdater);


                            CommonInitialize();
						}
						catch (Exception e)
						{
							LogManager.WriteLog(TraceType.ERROR, e.Message, "PIS.Ground.DataPackage.DataPackageService.Initialize", e, EventIdEnum.DataPackage);
						}
					}
				}
			}
		}

        /// <summary>
        /// Implements the common logic of initialization function.
        /// </summary>
        private static void CommonInitialize()
        {
            _stopRequested = false;
            _baselineStatusSystemInformationQueue.Clear();
            _remoteDateStoreUrl = ConfigurationSettings.AppSettings["RemoteDataStoreUrl"];
            _baselineStatusThread = new Thread(new ThreadStart(OnBaselineStatusEvent));
            _baselineStatusThread.Name = "Baseline Status";

            // Baseline Deployments Status

            // Getting from the HistoryLogger the list of all baseline deployments statuses recorded so far
            // and registering a T2G client object to be used for updating those statuses
            //
            IDictionary<string, SystemInfo> currentSystems = _t2gManager.GetAvailableSystems();
            _baselineStatusUpdater.Initialize(_t2gManager.T2GFileDistributionManager, currentSystems);

            _t2gManager.T2GFileDistributionManager.RegisterUpdateFileCompletionCallBack(new UpdateFileCompletionCallBack(OnUploadFileMethodCompleted));

            _t2gManager.SubscribeToSystemDeletedNotification(SubscriberId, new EventHandler<SystemDeletedNotificationArgs>(OnSystemDeleted));
            _t2gManager.SubscribeToT2GOnlineStatusNotification(SubscriberId, new EventHandler<T2GOnlineStatusNotificationArgs>(OnT2GOnlineOffline), false);
            _t2gManager.SubscribeToElementChangeNotification(SubscriberId, new EventHandler<ElementEventArgs>(OnElementInfoChanged));
            _t2gManager.SubscribeToFileDistributionNotifications(SubscriberId, new EventHandler<FileDistributionStatusArgs>(OnFileTransfer));

            _baselineStatusThread.Start();


            _requestManager.Initialize(_t2gManager, _notificationSender);
            _requestManager.AddRequestRange(getAllBaselineDistributingSavedRequests());
            _updateBaseLineTimer = new Timer(mUpdateBaselinesAssignation, _updateBaseLineTimerState, TimeSpan.Zero, TimeSpan.FromMinutes(_timeInterval));
            _initialized = true;
        }

        /// <summary>
        /// Initializes this instance by providing objects to use.
        /// </summary>
        /// <param name="sessionManager">The session manager.</param>
        /// <param name="notificationSender">The notification sender.</param>
        /// <param name="t2gManager">The T2G manager.</param>
        /// <param name="requestsFactory">The requests factory.</param>
        /// <param name="remoteDataStoreFactory">The remote data store factory.</param>
        /// <param name="requestManager">The request manager.</param>
        /// <param name="executeCommonLogic">Indicates if the common logic shall be executed</param>
        public static void Initialize(
            ISessionManager sessionManager,
            INotificationSender notificationSender,
            IT2GManager t2gManager,
            RequestMgt.IRequestContextFactory requestsFactory,
            RemoteDataStoreFactory.IRemoteDataStoreFactory remoteDataStoreFactory,
            RequestMgt.IRequestManager requestManager,
            BaselineStatusUpdater baselineStatusUpdater,
            bool executeCommonLogic)
		{
            lock (_initializationLock)
            {
                Uninitialize();

                _sessionManager = sessionManager;
                _notificationSender = notificationSender;
                _t2gManager = t2gManager;
                _requestFactory = requestsFactory;
                _remoteDataStoreFactory = remoteDataStoreFactory;
                _requestManager = requestManager;
                _baselineStatusUpdater = baselineStatusUpdater ?? new BaselineStatusUpdater();

                if (executeCommonLogic)
                {
                    CommonInitialize();
                }
            }
		}

        /// <summary>
        /// Uninitializes this instance.
        /// </summary>
        public static void Uninitialize()
        {
            lock (_initializationLock)
            {
                _stopRequested = true;
                if (_baselineStatusEvent != null && _baselineStatusThread != null)
                {
                    _baselineStatusEvent.Set();
                }

                if (_updateBaseLineTimer != null)
                {
                    _updateBaseLineTimer.Dispose();
                    _updateBaseLineTimer = null;
                }

                if (_requestManager != null)
                {
                    _requestManager.Dispose();
                    _requestManager = null;
                }

                if (_baselineStatusThread != null)
                {
                    if (_baselineStatusThread.ThreadState != ThreadState.Unstarted)
                    {
                        if (!_baselineStatusThread.Join(new TimeSpan(0, 1, 0)))
                        {
                            // Force the thread to stop
                            _baselineStatusThread.Abort();
                            _baselineStatusThread.Join(new TimeSpan(0, 1, 0));
                        }
                    }

                    _baselineStatusThread = null;
                }

                if (_baselineStatusUpdater != null)
                {
                    _baselineStatusUpdater.Dispose();
                }

                if (_t2gManager != null)
                {
                    if (_t2gManager.T2GFileDistributionManager != null)
                    {
                        _t2gManager.T2GFileDistributionManager.RegisterUpdateFileCompletionCallBack(null);
                    }
                    
                    _t2gManager.UnsubscribeFromSystemDeletedNotification(SubscriberId);
                    _t2gManager.UnsubscribeFromT2GOnlineStatusNotification(SubscriberId);
                    _t2gManager.UnsubscribeFromElementChangeNotification(SubscriberId);
                    _t2gManager.UnsubscribeFromFileDistributionNotifications(SubscriberId);

                    _t2gManager.Dispose();
                }

                if (_baselineStatusSystemInformationQueue != null)
                {
                    _baselineStatusSystemInformationQueue.Clear();
                }

                _remoteDateStoreUrl = null;
                _initialized = false;
                _stopRequested = false;
            }

        }

		/// <summary>Called when the UploadFile method completes.</summary>
		/// <param name="request">The request information from UploadFile.</param>
		private static void OnUploadFileMethodCompleted(PIS.Ground.Core.Data.UploadFileDistributionRequest request)
		{
			if (request != null)
			{
				if (request.RecipientList != null)
				{
					foreach (RecipientId lTrainId in request.RecipientList)
					{
						if (lTrainId.SystemId != null)
						{
							_baselineStatusUpdater.ProcessTaskId(lTrainId.SystemId, request.RequestId, request.TaskId);
						}
					}
				}
			}
		}

		/// <summary>
		/// Notification received when Element Changes OnlineState (triggered by T2G).
		/// </summary>
		/// <param name="pSender">Sender Info.</param>
		/// <param name="pElement">The Element that just changed Online State.</param>
		private static void OnElementInfoChanged(object pSender, ElementEventArgs pElement)
		{
			if (pElement != null)
			{
				SystemInfo lNewInfo = pElement.SystemInformation;

				if (lNewInfo != null && !string.IsNullOrEmpty(lNewInfo.SystemId))
				{
					lock (DataPackageService._baselineStatusLock)
					{
						_baselineStatusSystemInformationQueue.Enqueue(lNewInfo);
					}
					_baselineStatusEvent.Set();
				}
			}
		}

		/// <summary>Gets assigned baselines.</summary>
		/// <param name="trainId">Identifier for the train.</param>
		/// <param name="currentBaseline">The current baseline or null if unknown.</param>
		/// <param name="futureBaseline">The future baseline or null if unknown.</param>
		private static void GetAssignedBaselines(string trainId, out string currentBaseline, out string futureBaseline)
		{
			currentBaseline = null;
			futureBaseline = null;

			try
			{
                using (IRemoteDataStoreClient lRemDSProxy = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                {
                    currentBaseline = lRemDSProxy.getAssignedCurrentBaselineVersion(trainId);
                    futureBaseline = lRemDSProxy.getAssignedFutureBaselineVersion(trainId);
                }
			}
			catch (Exception lException)
			{
				mWriteLog(TraceType.ERROR, "GetAssignedBaselines", lException, Logs.ERROR_REMOTEDATASTORE_FAULTED);
			}
		}


		/// <summary>
		/// Notification received when a system deleted message is issued by T2G.
		/// </summary>
		/// <param name="pSender">Sender Info.</param>
		/// <param name="pNotification">The notification data.</param>
		private static void OnSystemDeleted(object pSender, SystemDeletedNotificationArgs pNotification)
		{
			if (pNotification != null)
			{
				if (string.IsNullOrEmpty(pNotification.SystemId) == false)
				{
					_baselineStatusUpdater.ProcessElementDeletedNotification(pNotification.SystemId);

					_notificationSender.SendNotification(PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DeletedElement, pNotification.SystemId);
				}
			}
		}

		/// <summary>
		/// Notification received when a T2G is connecting / disconnecting.
		/// </summary>
		/// <param name="pSender">Sender Info.</param>
		/// <param name="pNotification">The notification data.</param>
		private static void OnT2GOnlineOffline(object pSender, T2GOnlineStatusNotificationArgs pNotification)
		{
			if (pNotification != null)
			{
                try
                {
                    mWriteLog(TraceType.INFO, "OnT2GOnlineOffline", null, "OnT2GOnlineOffline : {0}", pNotification.online);

                    IDictionary<string, SystemInfo> currentSystems = (pNotification.online) ? _t2gManager.GetAvailableSystems() : null;
                    _baselineStatusUpdater.ResetStatusEntries(currentSystems);
                }
                catch (System.Exception ex)
                {
                    mWriteLog(TraceType.EXCEPTION, "PIs.Ground.DataPackage.DataPackageService.OnT2GOnlineOffline", ex, "Processing error while becoming {0} with T2G", (pNotification.online) ? "online" : "offline");
                }
			}
		}

		/// <summary>Executes the file transfer action following a T2G notification.</summary>
		/// <param name="pSender">Sender Info.</param>
		/// <param name="pNotification">The notification information.</param>
		private static void OnFileTransfer(object pSender, FileDistributionStatusArgs pNotification)
		{
			if (pNotification != null)
			{
				// Baseline Deployments Status

				// Updating on the HistoryLogger the baseline deployments statuses based
				// on that T2G notification
				//

				_baselineStatusUpdater.ProcessFileTransferNotification(pNotification);
			}
		}

		/// <summary>
		/// Notification received when a File distribution status changes (triggered from T2G).
		/// </summary>
		/// <param name="pSender">The sender of this notification.</param>
		/// <param name="pFDistrStatusArg">Info about the notification.</param>
		public static void OnFileDistributeNotification(object pSender, FileDistributionStatusArgs pFDistrStatusArg)
		{
			bool lSendNotification = true;
			PIS.Ground.GroundCore.AppGround.NotificationIdEnum lNotifEnum = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionProcessing;
			switch (pFDistrStatusArg.CurrentTaskPhase)
			{
				case TaskPhase.Creation://create task on T2GGround
					break;
				case TaskPhase.Acquisition://transferring files on T2GGround
					lNotifEnum = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionWaitingToTransfer;
					break;
				case TaskPhase.Transfer://transferring files from T2GGround to T2GOnBoard
					switch (pFDistrStatusArg.TaskStatus)
					{
						case TaskState.Created:
							break;
						case TaskState.Started:
							// Check if transfer is really started by verifying TransferCompletionPercent.
							// because started alone means only that the task has been kicked
							if (pFDistrStatusArg.TransferCompletionPercent > 0)
							{
								lNotifEnum = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionTransferring;
							}
							else
							{
								lSendNotification = false;
							}
							break;
						case TaskState.Stopped:
						case TaskState.Cancelled:
							lNotifEnum = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedTransferCancelledManually;
							break;
						case TaskState.Error:
							lNotifEnum = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedRejectedByElement;
							break;
						default:
							lSendNotification = false;
							break;
					}
					break;
				case TaskPhase.Distribution://sending notifications
					if (pFDistrStatusArg.TaskStatus == TaskState.Completed)
					{
						lNotifEnum = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionTransferred;

						//Check if there is an element data in dictionary that corresponds to the completed task
						//If it is there - remove it from the dictionary                        

						lock (_distributingElementDic)
						{
							KeyValuePair<string, DistributingElementData> elementData = _distributingElementDic.FirstOrDefault(element => element.Value.requestId == pFDistrStatusArg.RequestId);

							//If element is found - remove it
							if (!string.IsNullOrEmpty(elementData.Key))
							{
								_distributingElementDic.Remove(elementData.Key);
							}
							else
							{
								// if not found, don't send the notification because we don't know the request
								lSendNotification = false;
							}
						}
					}
					else
					{
						lSendNotification = false;
					}
					break;
				default:
					lSendNotification = false;
					break;
			}

			if (true == lSendNotification)
			{
				List<Recipient> recipient;
				TransferTaskData task;
				_t2gManager.T2GFileDistributionManager.GetTransferTask(pFDistrStatusArg.RequestId, out recipient, out task);

				foreach (Recipient lrec in recipient)
				{
					// Serialize the parameters to send in the notification.
					List<string> lParamList = new List<string>();
					lParamList.Add(lrec.SystemId);
					// Read the Baseline version from the dictionary where it was stored before calling T2G.
					string lBLVersion = "";
					bool lSuccess;
					lock (_baselineVersionDic)
					{
						lSuccess = _baselineVersionDic.TryGetValue(new KeyValuePair<Guid, string>(pFDistrStatusArg.RequestId, lrec.SystemId), out lBLVersion);
					}
					lParamList.Add(lBLVersion);

					if (!lSuccess)
					{
						mWriteLog(TraceType.WARNING, "OnFileDistributeNotification", null, Logs.WARNING_NO_BASELINE_VERSION_FOUND, pFDistrStatusArg.RequestId, lrec.SystemId);
					}

					using (StringWriter lstr = new StringWriter())
					{
						_stringListXmlSerializer.Serialize(lstr, lParamList);

						sendNotificationToGroundApp(pFDistrStatusArg.RequestId, lNotifEnum, lstr.ToString());
					}
				}
			}

		}

		/// <summary>
		/// Set a new assigned future baseline to an element.
		/// </summary>
		/// <param name="pElementId">The element Id you want to be assigned.</param>
		/// <param name="pBLVersion">The baseline version to assign.</param>
		/// <param name="pActDate">The activation date for the baseline.</param>
		/// <param name="pExpDate">The expiration date.</param>
		/// <param name="pResult">Result of the request as a DataPackageResult structure.</param>
		private static void assignFutureBaseline(string pElementId, string pBLVersion, DateTime pActDate, DateTime pExpDate, ref DataPackageResult pResult)
		{
			mWriteLog(TraceType.DEBUG, "assignFutureBaseline", null, Logs.DEBUG_ENTERING_FUNCTION, "assignFutureBaseline");

			if (pResult.reqId != Guid.Empty)
			{
				try
				{
                    using (IRemoteDataStoreClient lRemProx = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                    {
                        mWriteLog(TraceType.DEBUG, "assignFutureBaseline", null, Logs.DEBUG_ASSIGN_FUTURE_BASELINE, pBLVersion, pElementId);

                        lRemProx.assignAFutureBaselineToElement(pResult.reqId, pElementId, pBLVersion, pActDate, pExpDate);
                        pResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;

                        lock (_distributingElementDic)
                        {
                            if (_distributingElementDic.ContainsKey(pElementId))
                            {
                                _distributingElementDic.Remove(pElementId);
                            }
                        }
                    }
				}
				catch (TimeoutException ex)
				{
					mWriteLog(TraceType.EXCEPTION, "assignFutureBaseline", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);

					pResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
				}
				catch (CommunicationException ex)
				{
					mWriteLog(TraceType.EXCEPTION, "assignFutureBaseline", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);

					pResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
				}
				catch (Exception ex)
				{
					mWriteLog(TraceType.EXCEPTION, "assignFutureBaseline", ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
					pResult.error_code = DataPackageErrorEnum.ERROR;
				}
			}
			else
			{
				mWriteLog(TraceType.EXCEPTION, "assignFutureBaseline", null, Logs.ERROR_INVALID_REQUEST_ID);

				pResult.error_code = DataPackageErrorEnum.ERROR;
			}
		}

		/// <summary>
		/// Updates the Request state to be completed.
		/// </summary>
		/// <param name="requestId">Request Id related to the request.</param>
		/// <param name="pElementId">Element Id related to the request.</param>
		private static void UpdateRequestStateToCompleted(Guid pRequestId, string pElementId)
		{
			lock (_lock)
			{
				_requestCompletedEvents.Add(new RequestCompletedEvent(pElementId, pRequestId));
			}
		}

		/// <summary>
		/// Check if a version is valid (matches to the predefined regex patern (*.*.*.*)).
		/// </summary>
		/// <param name="pVersion">The version to check.</param>
		/// <param name="pAllowWildcards">Are the wildcards (*) allowed.</param>
		/// <returns>"true" - if version is correct or "fales" - if not.</returns>
		private static bool mIsVersionValid(string pVersion, bool pAllowWildcards)
		{
			if (pAllowWildcards)
			{
				return Regex.Match(pVersion, @"^((\d+|\*)\.){3}(\d+|\*)$").Success;
			}
			else
			{
				return Regex.Match(pVersion, @"^(\d+\.){3}\d+$").Success;
			}
		}

		/// <summary>
		/// Check if a version matches the input patern (*.*.*.*)
		/// </summary>
		/// <param name="pVersion">The version to check.</param>
		/// <param name="pPattern">The pattern.</param>
		/// <returns>Result is "true" - if version matches the pattern or "fales" - if not</returns>
		private static bool mDoesVersionMatchPattern(string pVersion, string pPattern)
		{
			bool lResult = false;

			string[] lPatternSplitted = pPattern.Split('.');
			string[] lVersionSplitted = pVersion.Split('.');

			if (lVersionSplitted.Length == 4)
			{
				if (lPatternSplitted.Length == 4)
				{
					if ((lPatternSplitted[0] == "*" || lPatternSplitted[0] == lVersionSplitted[0])
						&& (lPatternSplitted[1] == "*" || lPatternSplitted[1] == lVersionSplitted[1])
						&& (lPatternSplitted[2] == "*" || lPatternSplitted[2] == lVersionSplitted[2])
						&& (lPatternSplitted[3] == "*" || lPatternSplitted[3] == lVersionSplitted[3]))
					{
						lResult = true;
					}
				}
			}
			return lResult;
		}

		/// <summary>
		/// Set a new assigned current baseline to an element.
		/// </summary>
		/// <param name="pElementId">The element Id you want to be assigned.</param>
		/// <param name="pBLVersion">The baseline version to assign.</param>
		/// <param name="pExpDate">The expiration date.</param>
		/// <param name="lResult">A structure that contains the error code plus a request id.</param>
		private static void assignCurrentBaseline(string pElementId, string pBLVersion, DateTime pExpDate, ref DataPackageResult pResult)
		{
			mWriteLog(TraceType.DEBUG, "assignCurrentBaseline", null, Logs.DEBUG_ENTERING_FUNCTION, "assignCurrentBaseline");

			if (pResult.reqId != Guid.Empty)
			{
				try
				{
                    using (IRemoteDataStoreClient lRemProx = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                    {
                        mWriteLog(TraceType.DEBUG, "assignCurrentBaseline", null, Logs.DEBUG_ASSIGN_CURRENT_BASELINE, pBLVersion, pElementId);

                        lRemProx.assignACurrentBaselineToElement(pResult.reqId, pElementId, pBLVersion, pExpDate);
                        pResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;
                    }
				}
				catch (TimeoutException ex)
				{
					mWriteLog(TraceType.EXCEPTION, "assignCurrentBaseline", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);

					pResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
				}
				catch (CommunicationException ex)
				{
					mWriteLog(TraceType.EXCEPTION, "assignCurrentBaseline", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);

					pResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
				}
				catch (Exception ex)
				{
					mWriteLog(TraceType.EXCEPTION, "assignCurrentBaseline", ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
					pResult.error_code = DataPackageErrorEnum.ERROR;
				}
			}
			else
			{
				mWriteLog(TraceType.ERROR, "assignCurrentBaseline", null, Logs.ERROR_INVALID_REQUEST_ID);

				pResult.error_code = DataPackageErrorEnum.ERROR;
			}
		}

		/// <summary>Searches for transfer(s) inside the specified list that are not completed or already cancelled.</summary>
		/// <param name="inputTasksList">List of tasks to analyse.</param>
		/// <returns>The found pending tasks.</returns>
		private static PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList
			findPendingTransferTasks(PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList inputTasksList)
		{
			var lOutputList = new PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList();

			if (inputTasksList != null)
			{
				foreach (PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct lCurrentTask in inputTasksList)
				{
					if (lCurrentTask != null)
					{
						if (lCurrentTask.taskState != PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskCancelled
							&& lCurrentTask.taskState != PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskCompleted)
						{
							lOutputList.Add(lCurrentTask);
						}
					}
				}
			}
			return lOutputList;
		}

		/// <summary>Cancel transfer tasks.</summary>
		/// <param name="tasksList">List of tasks.</param>
		/// <returns>True if all task cancellation requests have been sent, false otherwise</returns>
		private static bool cancelTransferTasks(List<CancellableTransferTaskInfo> tasksList)
		{
			bool lSuccess = true;

			if (tasksList != null)
			{
				var lFailureLists = new List<string>();
				string lCurrentError;

				foreach (CancellableTransferTaskInfo currentTask in tasksList)
				{
					if (currentTask != null)
					{
						_t2gManager.T2GFileDistributionManager.CancelTransferTask(currentTask.TaskID, out lCurrentError);

						if (lCurrentError != string.Empty)
						{
							lFailureLists.Add(currentTask.TaskID.ToString());
							lSuccess = false;
						}
					}
				}
				if (lFailureLists.Count > 0)
				{
					mWriteLog(TraceType.WARNING, "cancelTransferTasks", null, Logs.ERROR_TRANSFER_TASKS_CANCELING, string.Join(",", lFailureLists.ToArray()));
				}
			}
			return lSuccess;
		}

		/// <summary>Gets the list of cancellable transfer tasks.</summary>
		/// <param name="cancellableTasks">[out] The tasks.</param>
		/// <returns>True if the list is complete, false otherwise.</returns>
		private static bool getCancellableTransferTasks(out List<CancellableTransferTaskInfo> cancellableTasks)
		{
			bool lIsComplete = true;
			cancellableTasks = new List<CancellableTransferTaskInfo>();

			// The task recipient applicationId field is needed to determine if a particular transfer will be cancelled
			string lApplicationId = ConfigurationSettings.AppSettings["ApplicationId"];

			// Getting the complete list of transfer tasks from T2G.
			// This performs an actual T2G Ground DataPackage web service request
			PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList transferTaskList;

			bool lCompleteEnum = _t2gManager.T2GFileDistributionManager.EnumTransferTask(DateTime.MinValue, _MaxTransferDate, out transferTaskList);

			string lEnumTransferTaskResultsInfo = " no transfer found";

			if (transferTaskList != null && transferTaskList.Count > 0)
			{
				lEnumTransferTaskResultsInfo = string.Empty;
				transferTaskList.ForEach((lTransfer) => lEnumTransferTaskResultsInfo += " Task id: "
					+ lTransfer.taskId + " state: " + lTransfer.taskState.ToString());
			}

			mWriteLog(TraceType.INFO, "getCancellableTransferTasks", null, Logs.INFO_LIST_OF_TRANSFER_TASKS, lEnumTransferTaskResultsInfo);

			if (lCompleteEnum == false)
			{
				mWriteLog(TraceType.WARNING, "getCancellableTransferTasks", null, Logs.WARNING_INCOMPLETE_TRANSFER_TASKS_LIST);

				lIsComplete = false;
			}

			if (transferTaskList != null && transferTaskList.Count > 0)
			{
				// Filtering from the transfer tasks list the ones that are not already completed or cancelled
				PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskList lPendingTasks = findPendingTransferTasks(transferTaskList);

				// Build another list of transfers with information needed for the cancellation process
				var lPendingTransfers = new List<CancellableTransferTaskInfo>();
				foreach (PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct lTask in lPendingTasks)
				{
					lPendingTransfers.Add(new CancellableTransferTaskInfo(lTask.taskId, null));
				}

				// List of tasks that are not found in our distribution list
				var lUnrecordedPendingTransfers = new List<CancellableTransferTaskInfo>();

				// Checking if the pending transfer tasks are listed in our distribution list.
				foreach (CancellableTransferTaskInfo lTransfer in lPendingTransfers)
				{
					// This does NOT perform a T2G Ground DataPackage web service request
					var lRequest = _t2gManager.T2GFileDistributionManager.GetFileDistributionRequestByTaskId(lTransfer.TaskID);

					if (lRequest != null)
					{
						StringBuilder lFileDistributionRequestInfo = new StringBuilder(" no recipient");

						// Task information found in local data
						var lElements = new List<string>();

						// Get the list of recipient(s) involved in that task
						if (lRequest.RecipientList != null && lRequest.RecipientList.Count > 0)
						{
							lFileDistributionRequestInfo.Length = 0;

							foreach (RecipientId lCurrentRecipient in lRequest.RecipientList)
							{
								lFileDistributionRequestInfo.Append(" | Application Id: ");
								lFileDistributionRequestInfo.Append(lCurrentRecipient.ApplicationId);
								lFileDistributionRequestInfo.Append(" System Id: ");
								lFileDistributionRequestInfo.Append(lCurrentRecipient.SystemId);

								if (lCurrentRecipient.ApplicationId == lApplicationId)
								{
									lElements.Add(lCurrentRecipient.SystemId);
								}
							}
						}

						mWriteLog(TraceType.INFO, "getCancellableTransferTasks", null, Logs.INFO_TRANSFER_TASK_PARAMS, lTransfer.TaskID, lFileDistributionRequestInfo.ToString());

						if (lElements.Count > 0)
						{
							var recordedTransfer = new CancellableTransferTaskInfo(lTransfer.TaskID, lElements);
							cancellableTasks.Add(recordedTransfer);
						}
					}
					else
					{
						var recordedTransfer = new CancellableTransferTaskInfo(lTransfer.TaskID, null);
						lUnrecordedPendingTransfers.Add(recordedTransfer);
					}
				}

				// Ask T2G about all unrecorded transfer tasks
				//

				foreach (CancellableTransferTaskInfo lTransfer in lUnrecordedPendingTransfers)
				{
					List<Recipient> lRecipients;
					TransferTaskData lTransferTaskData;

					mWriteLog(TraceType.INFO, "getCancellableTransferTasks", null, Logs.INFO_RETRIEVE_TRANSFER_TASK_ID, lTransfer.TaskID);

					string lGetTransferTaskError = _t2gManager.T2GFileDistributionManager.GetTransferTask(lTransfer.TaskID, out lRecipients, out lTransferTaskData);
					if (lGetTransferTaskError != string.Empty)
					{
						mWriteLog(TraceType.WARNING, "getCancellableTransferTasks", null, Logs.WARNING_CANT_RETRIVE_TRANSFER_TASK_INFO, lTransfer.TaskID);

						lIsComplete = false;
					}
					else
					{
						// Extract the list of recipient elements

						StringBuilder lTranferTaskInfo = new StringBuilder(" no recipient");


						if (lRecipients != null && lRecipients.Count > 0)
						{
							var lElements = new List<string>();
							lTranferTaskInfo.Length = 0;

							foreach (Recipient lCurrentRecipient in lRecipients)
							{
								lTranferTaskInfo.Append(" | Application Id: ");
								lTranferTaskInfo.Append(lCurrentRecipient.ApplicationIds);
								lTranferTaskInfo.Append(" System Id: ");
								lTranferTaskInfo.Append(lCurrentRecipient.SystemId);

								// Add recipient only if associated to BaselineConnector
								if (lCurrentRecipient.ApplicationIds == lApplicationId)
								{
									lElements.Add(lCurrentRecipient.SystemId);
								}
							}
							if (lElements.Count > 0)
							{
								lTransfer.Elements = lElements;
								cancellableTasks.Add(lTransfer);
							}
						}

						mWriteLog(TraceType.INFO, "getCancellableTransferTasks", null, Logs.WARNING_CANT_RETRIVE_TRANSFER_TASK_INFO, lTransfer.TaskID, lTranferTaskInfo.ToString());
					}
				}
			}

			return lIsComplete;
		}

		/// <summary>
		/// Thread that will handle all requests that were Enqueued in the request list.
		/// </summary>
		private static void OnBaselineStatusEvent()
		{
            try
            {
                SystemInfo lNextSystemInfo = null;

                while (!_stopRequested)
                {
                    mWriteLog(TraceType.INFO, "OnBaselineStatusEvent", null, Logs.INFO_NO_EVENTS_TO_PROCESS);

                    _baselineStatusEvent.WaitOne();

                    mWriteLog(TraceType.INFO, "OnBaselineStatusEvent", null, Logs.INFO_NEW_EVENTS_TO_PROCESS);

                    if (_stopRequested)
                        break;

                    int lRemainingCount = 0;

                    do
                    {
                        lNextSystemInfo = null;

                        lock (DataPackageService._baselineStatusLock)
                        {
                            lRemainingCount = _baselineStatusSystemInformationQueue.Count;

                            if (lRemainingCount > 0)
                            {
                                lNextSystemInfo = _baselineStatusSystemInformationQueue.Dequeue();

                                if (lNextSystemInfo != null)
                                {
                                    if (!string.IsNullOrEmpty(lNextSystemInfo.SystemId))
                                    {
                                        string lAssignedCurrentBaseline;
                                        string lAssignedFutureBaseline;

                                        DataPackageService.GetAssignedBaselines(lNextSystemInfo.SystemId, out lAssignedCurrentBaseline, out lAssignedFutureBaseline);

                                        _baselineStatusUpdater.ProcessSystemChangedNotification(
                                            lNextSystemInfo, lAssignedCurrentBaseline, lAssignedFutureBaseline);
                                    }
                                }
                            }
                        }
                    }
                    while (lRemainingCount > 0 && !_stopRequested);
                }
            }
            catch (ThreadAbortException)
            {
                // No logic to apply
            }
            catch (System.Exception exception)
            {
                mWriteLog(TraceType.EXCEPTION, "OnBaselineStatusEvent", exception, Logs.INFO_NEW_EVENTS_TO_PROCESS);
            }
		}

		private static void mUpdateBaselinesAssignation(object state)
		{
			try
			{
                lock (_distributingElementDic)
                {
                    IRemoteDataStoreClient lRemDSProxy = _remoteDataStoreFactory.GetRemoteDataStoreInstance();
                    try
                    {
                        foreach (var pair in _distributingElementDic)
                        {
                            if (_stopRequested)
                            {
                                break;
                            }

                            try
                            {
                                DataContainer lElDescrCont = lRemDSProxy.getElementBaselinesDefinitions(pair.Key);
                                if (_stopRequested)
                                {
                                    break;
                                }

                                ElementDescription lElDescr = DataTypeConversion.fromDataContainerToElementDescription(lElDescrCont);
                                //condition to check if future baseline activation date is reached/has passed
                                if (DateTime.Compare(DateTime.Now, lElDescr.AssignedFutureBaselineActivationDate) >= 0)
                                {
                                    //condition to check if future baseline version is different from current baseline version
                                    if (string.Compare(lElDescr.AssignedFutureBaseline, lElDescr.AssignedCurrentBaseline) != 0)
                                    {
                                        //assigning future baseline version to current baseline version
                                        lRemDSProxy.assignACurrentBaselineToElement(Guid.Empty, pair.Key, lElDescr.AssignedFutureBaseline, lElDescr.AssignedFutureBaselineExpirationDate);
                                    }
                                    //condition to check if DistributionStatus is completed
                                    if (pair.Value.distributionStatus == PIS.Ground.GroundCore.AppGround.NotificationIdEnum.Completed)
                                    {
                                        //assigning future baseline version as ""
                                        lRemDSProxy.unassignFutureBaselineFromElement(pair.Key);
                                    }
                                }
                            }
                            catch (System.Exception lEx)
                            {
                                LogManager.WriteLog(
                                    TraceType.EXCEPTION,
                                    lEx.Message,
                                    string.Format(CultureInfo.CurrentCulture, "{0}({1})", "PIS.Ground.DataPackage.DataPackageService.mUpdateBaselinesAssignation", pair.Key),
                                    lEx,
                                    EventIdEnum.DataPackage);
                                lRemDSProxy.Dispose();
                                lRemDSProxy = _remoteDataStoreFactory.GetRemoteDataStoreInstance();
                            }
                        }
                    }
                    finally
                    {
                        lRemDSProxy.Dispose();
                    }
                }
			}
			catch (System.Exception lEx)
			{
				LogManager.WriteLog(
					TraceType.EXCEPTION,
					lEx.Message,
					"PIS.Ground.DataPackage.DataPackageService.mUpdateBaselinesAssignation",
					lEx,
					EventIdEnum.DataPackage);
			}
		}

		/// <summary>Save the Baseline version in the dictionary so it could be resused when a notification is received from T2G.</summary>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="futureBaseline">The future baseline or null if unknown.</param>
		public static void mAddBaselineVersionToDictionary(Guid requestId, string elementId, string futureBaseline)
		{
			lock (_baselineVersionDic)
			{
				_baselineVersionDic.Add(new KeyValuePair<Guid, string>(requestId, elementId), futureBaseline);
			}
		}

		/// <summary>Adds the packages to used packages list.</summary>
		/// <param name="pPackages">A variable-length parameters list containing packages.</param>
		internal static void mAddPackagesToUsedPackagesList(List<PackageParams> pPackages)
		{
			lock (_usedPackages)
			{
				foreach (PackageParams pp in pPackages)
				{
					_usedPackages.Add(pp);
				}
			}
		}

		/// <summary>Removes the packages from used packages list described by pPackages.</summary>
		/// <param name="pPackages">A variable-length parameters list containing packages.</param>
		internal static void mRemovePackagesFromUsedPackagesList(List<PackageParams> pPackages)
		{
			lock (_usedPackages)
			{
				foreach (PackageParams pp in pPackages)
				{
					foreach (PackageParams pparams in _usedPackages)
					{
						if (pp.aType == pparams.aType &&
							pp.aVersion == pparams.aVersion)
						{
							_usedPackages.Remove(pparams);

							break;
						}
					}
				}
			}
		}

		private static bool mIsPackageUsed(DataPackageType pType, string pVersion)
		{
			lock (_usedPackages)
			{
				foreach (PackageParams pp in _usedPackages)
				{
					if (pp.aType == pType &&
						pp.aVersion == pVersion)
					{
						return true;
					}
				}
			}

			return false;
		}

		internal static void mWriteLog(TraceType traceType, string context, Exception ex, string messageId, params object[] args)
		{
			try
			{
				LogManager.WriteLog(
					traceType,
					string.Format(CultureInfo.CurrentCulture, messageId, args),
					"PIS.Ground.DataPackage.DataPackageService." + context,
					ex,
					EventIdEnum.DataPackage);
			}
			catch (Exception lEx)
			{
				try
				{
					LogManager.WriteLog(
						TraceType.EXCEPTION,
						ex.Message,
						"PIS.Ground.DataPackage.DataPackageService.mWriteLog",
						lEx,
						EventIdEnum.DataPackage);
				}
				catch
				{
					// Nothing can be done for logging the error.					
				}
			}
		}

		#endregion

		#region internal static methods

		/// <summary>Saves a baseline distributing request.</summary>
		/// <param name="processBaselineDistributingRequest">The process baseline distributing request.</param>
		internal static void saveBaselineDistributingRequest(RequestMgt.BaselineDistributingRequestContext processBaselineDistributingRequest)
		{
			try
			{
                using (IRemoteDataStoreClient remoteDataStoreProxy = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                {
                    DataContainer baselineDistributingTask = DataTypeConversion.fromBaselineDistributingRequestToDataContainer(processBaselineDistributingRequest);
                    remoteDataStoreProxy.saveBaselineDistributingRequest(baselineDistributingTask);
                }
			}
			catch (TimeoutException ex)
			{
				mWriteLog(TraceType.EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
			}
			catch (CommunicationException ex)
			{
				mWriteLog(TraceType.EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
			}
			catch (Exception ex)
			{
				mWriteLog(TraceType.EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
			}
		}

		/// <summary>Gets all baseline distributing saved requests.</summary>
		/// <returns>all baseline distributing saved requests.</returns>
		internal static List<IRequestContext> getAllBaselineDistributingSavedRequests()
		{
			List<IRequestContext> requestsList = new List<IRequestContext>();

			try
			{
                using (IRemoteDataStoreClient remoteDataStoreProxy = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                {
                    DataContainer baselineDistributingSavedRequestsList = remoteDataStoreProxy.getAllBaselineDistributingSavedRequests();
                    requestsList = DataTypeConversion.fromDataContainerToBaselineDistributingSavedRequestsList(
                        baselineDistributingSavedRequestsList,
                        _requestFactory);
                }
			}
			catch (TimeoutException ex)
			{
				mWriteLog(TraceType.EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
			}
			catch (CommunicationException ex)
			{
				mWriteLog(TraceType.EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
			}
			catch (Exception ex)
			{
				mWriteLog(TraceType.EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
			}

			return requestsList;
		}

		/// <summary>Deletes the baseline distributing request described by elementId.</summary>
		/// <param name="elementId">Identifier for the element.</param>
		internal static void deleteBaselineDistributingRequest(string elementId)
		{
			try
			{
                using (IRemoteDataStoreClient remoteDataStoreProxy = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                {
                    remoteDataStoreProxy.deleteBaselineDistributingRequest(elementId);
                }
			}
			catch (TimeoutException ex)
			{
				mWriteLog(TraceType.EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
			}
			catch (CommunicationException ex)
			{
				mWriteLog(TraceType.EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
			}
			catch (Exception ex)
			{
				mWriteLog(TraceType.EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
			}
		}

		#endregion

		#region private methods

		/// <summary>
		/// Return the baselines corresponding to an element : the on board baselines, asking T2G, the assigned baselines, asking RemoteDataStore
		/// </summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pTargetAddress">The element(s) address you want the baselines.</param>
		/// <returns>A structure that contains the error code plus the element description.</returns>
		private GetAdresseesDataPackageBaselinesResult getAddresseesDataPackagesBaselines(Guid pSessionId, TargetAddressType pTargetAddress)
		{
			GetAdresseesDataPackageBaselinesResult lResult = new GetAdresseesDataPackageBaselinesResult();
			lResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;

			if (_sessionManager.IsSessionValid(pSessionId))
			{
				List<string> lEIDs;

				lResult.error_code = mGetElementsListByTargetAddress(pTargetAddress, out lEIDs);

				if (lResult.error_code == DataPackageErrorEnum.REQUEST_ACCEPTED)
				{
					foreach (string lEID in lEIDs)
					{
						ElementDescription lElDescr = new ElementDescription();
						lElDescr.ElementID = lEID;

						try
						{
                            using (IRemoteDataStoreClient lRemDSProxy = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                            {
                                DataContainer lElDescrCont = lRemDSProxy.getElementBaselinesDefinitions(lEID);
                                lElDescr = DataTypeConversion.fromDataContainerToElementDescription(lElDescrCont);

                                if (lElDescr != null)
                                {
                                    // Fill in the train id field if still missing
                                    if (string.IsNullOrEmpty(lElDescr.ElementID))
                                    {
                                        lElDescr.ElementID = lEID;
                                    }
                                }
                            }
						}
						catch (FaultException lEx)
						{
							if (lEx.Code.Name == RemoteDataStore.RemoteDataStoreExceptionCodeEnum.UNKNOWN_ELEMENT_ID.ToString())
							{
								mWriteLog(TraceType.EXCEPTION, "getAddresseesDataPackagesBaselines", lEx, Logs.ERROR_ELEMENT_NOT_FOUND, lEID);
							}
						}
						catch (TimeoutException lEx)
						{
							mWriteLog(TraceType.EXCEPTION, "getAddresseesDataPackagesBaselines", lEx, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);

							lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
							return lResult;
						}
						catch (CommunicationException lEx)
						{
							mWriteLog(TraceType.EXCEPTION, "getAddresseesDataPackagesBaselines", lEx, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);

							lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
							return lResult;
						}
						catch (Exception lEx)
						{
							mWriteLog(TraceType.EXCEPTION, "getAddresseesDataPackagesBaselines", lEx, Logs.ERROR_REMOTEDATASTORE_FAULTED);
							lResult.error_code = DataPackageErrorEnum.ERROR;
							return lResult;
						}

						AvailableElementData lElData;
						T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableElementDataByElementNumber(lEID, out lElData);
						switch (lRqstResult)
						{
							case T2GManagerErrorEnum.eSuccess:
								if (lElData.PisBaselineData != null)
								{
									lElDescr.ElementArchivedBaseline = new ElementBaseline();
									if (!string.IsNullOrEmpty(lElData.PisBaselineData.ArchivedVersionOut))
									{
										lElDescr.ElementArchivedBaseline.BaselineVersion = lElData.PisBaselineData.ArchivedVersionOut;
										lElDescr.ElementArchivedBaseline.BaselineValidity = Boolean.Parse(lElData.PisBaselineData.ArchivedValidOut);
										lElDescr.ElementArchivedBaseline.PISBaseDataPackageVersion = lElData.PisBaselineData.ArchivedVersionPisBaseOut;
										lElDescr.ElementArchivedBaseline.PISMissionDataPackageVersion = lElData.PisBaselineData.ArchivedVersionPisMissionOut;
										lElDescr.ElementArchivedBaseline.PISInfotainmentDataPackageVersion = lElData.PisBaselineData.ArchivedVersionPisInfotainmentOut;
										lElDescr.ElementArchivedBaseline.LMTDataPackageVersion = lElData.PisBaselineData.ArchivedVersionLmtOut;
									}
									lElDescr.ElementCurrentBaseline = new ElementBaseline();

                                    const string dateTimeFormat = "(yyyy-MM-dd\\THH:mm:ss.f)";
									if (!string.IsNullOrEmpty(lElData.PisBaselineData.CurrentVersionOut))
									{
										lElDescr.isCurrentBaselineForced = Boolean.Parse(lElData.PisBaselineData.CurrentForcedOut);
                                        DateTime ltmpDateTime;
                                        DateTime.TryParseExact(lElData.PisBaselineData.CurrentExpirationDateOut, dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out ltmpDateTime);
										lElDescr.ElementCurrentBaselineExpirationDate = ltmpDateTime;
										lElDescr.ElementCurrentBaseline.BaselineVersion = lElData.PisBaselineData.CurrentVersionOut;
										lElDescr.ElementCurrentBaseline.BaselineValidity = Boolean.Parse(lElData.PisBaselineData.CurrentValidOut);
										lElDescr.ElementCurrentBaseline.PISBaseDataPackageVersion = lElData.PisBaselineData.CurrentVersionPisBaseOut;
										lElDescr.ElementCurrentBaseline.PISMissionDataPackageVersion = lElData.PisBaselineData.CurrentVersionPisMissionOut;
										lElDescr.ElementCurrentBaseline.PISInfotainmentDataPackageVersion = lElData.PisBaselineData.CurrentVersionPisInfotainmentOut;
										lElDescr.ElementCurrentBaseline.LMTDataPackageVersion = lElData.PisBaselineData.CurrentVersionLmtOut;
									}
									lElDescr.ElementFutureBaseline = new ElementBaseline();
									if (!string.IsNullOrEmpty(lElData.PisBaselineData.FutureVersionOut))
									{
                                        DateTime ltmpDateTime;
										lElDescr.ElementFutureBaseline.BaselineVersion = lElData.PisBaselineData.FutureVersionOut;
										lElDescr.ElementFutureBaseline.BaselineValidity = Boolean.Parse(lElData.PisBaselineData.FutureValidOut);
										lElDescr.ElementFutureBaseline.PISBaseDataPackageVersion = lElData.PisBaselineData.FutureVersionPisBaseOut;
										lElDescr.ElementFutureBaseline.PISMissionDataPackageVersion = lElData.PisBaselineData.FutureVersionPisMissionOut;
										lElDescr.ElementFutureBaseline.PISInfotainmentDataPackageVersion = lElData.PisBaselineData.FutureVersionPisInfotainmentOut;
										lElDescr.ElementFutureBaseline.LMTDataPackageVersion = lElData.PisBaselineData.FutureVersionLmtOut;
                                        DateTime.TryParseExact(lElData.PisBaselineData.FutureActivationDateOut, dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out ltmpDateTime);
                                        lElDescr.ElementFutureBaselineActivationDate = ltmpDateTime;
                                        DateTime.TryParseExact(lElData.PisBaselineData.FutureExpirationDateOut, dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out ltmpDateTime);
                                        lElDescr.ElementFutureBaselineExpirationDate = ltmpDateTime;
									}

									//Add baseline description from RemoteDataStore
									try
									{
                                        using (IRemoteDataStoreClient lRemProx = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                                        {
                                            if (!string.IsNullOrEmpty(lElDescr.ElementArchivedBaseline.BaselineVersion) && lRemProx.checkIfBaselineExists(lElDescr.ElementArchivedBaseline.BaselineVersion) == true)
                                            {
                                                BaselineDefinition lBlDef = DataTypeConversion.fromDataContainerToBaselineDefinition(lRemProx.getBaselineDefinition(lElDescr.ElementArchivedBaseline.BaselineVersion));
                                                lElDescr.ElementArchivedBaseline.BaselineDescription = lBlDef.BaselineDescription;
                                            }
                                            if (!string.IsNullOrEmpty(lElDescr.ElementCurrentBaseline.BaselineVersion) && lRemProx.checkIfBaselineExists(lElDescr.ElementCurrentBaseline.BaselineVersion) == true)
                                            {
                                                BaselineDefinition lBlDef = DataTypeConversion.fromDataContainerToBaselineDefinition(lRemProx.getBaselineDefinition(lElDescr.ElementCurrentBaseline.BaselineVersion));
                                                lElDescr.ElementCurrentBaseline.BaselineDescription = lBlDef.BaselineDescription;
                                            }
                                            if (!string.IsNullOrEmpty(lElDescr.ElementFutureBaseline.BaselineVersion) && lRemProx.checkIfBaselineExists(lElDescr.ElementFutureBaseline.BaselineVersion) == true)
                                            {
                                                BaselineDefinition lBlDef = DataTypeConversion.fromDataContainerToBaselineDefinition(lRemProx.getBaselineDefinition(lElDescr.ElementFutureBaseline.BaselineVersion));
                                                lElDescr.ElementArchivedBaseline.BaselineDescription = lBlDef.BaselineDescription;
                                            }
                                        }
									}
									catch (TimeoutException lEx)
									{
										mWriteLog(TraceType.EXCEPTION, "getAddresseesDataPackagesBaselines", lEx, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
									}
									catch (CommunicationException lEx)
									{
										mWriteLog(TraceType.EXCEPTION, "getAddresseesDataPackagesBaselines", lEx, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
									}
									catch (Exception lEx)
									{
										mWriteLog(TraceType.EXCEPTION, "getAddresseesDataPackagesBaselines", lEx, Logs.ERROR_REMOTEDATASTORE_FAULTED);
									}
								}
								else
								{
									lElDescr.ElementArchivedBaseline = new ElementBaseline();
									lElDescr.ElementArchivedBaseline.BaselineVersion = "";
									lElDescr.isCurrentBaselineForced = false;
									lElDescr.ElementCurrentBaselineExpirationDate = new DateTime();
									lElDescr.ElementCurrentBaseline = new ElementBaseline();
									lElDescr.ElementCurrentBaseline.BaselineVersion = "";
									lElDescr.ElementFutureBaselineActivationDate = new DateTime();
									lElDescr.ElementFutureBaselineExpirationDate = new DateTime();
									lElDescr.ElementFutureBaseline = new ElementBaseline();
									lElDescr.ElementFutureBaseline.BaselineVersion = "";
								}
								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								{
									lElDescr.ElementArchivedBaseline = new ElementBaseline();
									lElDescr.ElementArchivedBaseline.BaselineVersion = "";
									lElDescr.isCurrentBaselineForced = false;
									lElDescr.ElementCurrentBaselineExpirationDate = new DateTime();
									lElDescr.ElementCurrentBaseline = new ElementBaseline();
									lElDescr.ElementCurrentBaseline.BaselineVersion = "";
									lElDescr.ElementFutureBaselineActivationDate = new DateTime();
									lElDescr.ElementFutureBaselineExpirationDate = new DateTime();
									lElDescr.ElementFutureBaseline = new ElementBaseline();
									lElDescr.ElementFutureBaseline.BaselineVersion = "";
									sendNotificationToGroundApp(Guid.Empty, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageT2GServerOffline, string.Empty);
								}
								break;
							case T2GManagerErrorEnum.eElementNotFound:
								{
									lElDescr.ElementArchivedBaseline = new ElementBaseline();
									lElDescr.ElementArchivedBaseline.BaselineVersion = "";
									lElDescr.isCurrentBaselineForced = false;
									lElDescr.ElementCurrentBaselineExpirationDate = new DateTime();
									lElDescr.ElementCurrentBaseline = new ElementBaseline();
									lElDescr.ElementCurrentBaseline.BaselineVersion = "";
									lElDescr.ElementFutureBaselineActivationDate = new DateTime();
									lElDescr.ElementFutureBaselineExpirationDate = new DateTime();
									lElDescr.ElementFutureBaseline = new ElementBaseline();
									lElDescr.ElementFutureBaseline.BaselineVersion = "";
								}
								break;
							default:
								break;
						}

						//Add to list
						lResult.ElementDesc.Add(lElDescr);
					}
				}
			}
			else
			{
				lResult.error_code = DataPackageErrorEnum.INVALID_SESSION_ID;
				return lResult;
			}
			return lResult;
		}

		/// <summary>
		/// Set a new assigned future baseline to an element.
		/// </summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pElementId">The element Id you want to be assigned.</param>
		/// <param name="pBLVersion">The baseline version to assign.</param>
		/// <param name="pActDate">The activation date for the baseline.</param>
		/// <param name="pExpDate">The expiration date.</param>
		/// <returns>A structure that contains the error code plus a request id.</returns>
		private DataPackageResult assignFutureBaselineToElement(Guid pSessionId, string pElementId, string pBLVersion, DateTime pActDate, DateTime pExpDate)
		{
			DataPackageResult lResult = new DataPackageResult();
			lResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;

			if (_sessionManager.IsSessionValid(pSessionId))
			{
				Guid lrequestId;
				if (pExpDate.Date < DateTime.Today.Date)
				{
					lResult.error_code = DataPackageErrorEnum.INVALID_EXPIRATION_DATEANDTIME;
					return lResult;
				}
				string lGetResult = _sessionManager.GenerateRequestID(pSessionId, out lrequestId);
				lResult.reqId = lrequestId;

				if (lrequestId != Guid.Empty)
				{
					if (!DataPackageService.mIsVersionValid(pBLVersion, false))
					{
						lResult.error_code = DataPackageErrorEnum.INVALID_BASELINE_VERSION;
					}
					else
					{
						if (pActDate >= pExpDate)
						{
							lResult.error_code = DataPackageErrorEnum.INVALID_EXPIRATION_DATEANDTIME;
						}
						else
						{
							lResult.error_code = checkIfElementExists(pElementId);

							if (lResult.error_code == DataPackageErrorEnum.REQUEST_ACCEPTED)
							{
								lResult.reqId = lrequestId;
								DataPackageService.assignFutureBaseline(pElementId, pBLVersion, pActDate, pExpDate, ref lResult);
							}
							else if (lResult.error_code != DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE)
							{
								lResult.error_code = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
							}
						}
					}
				}
				else
				{
					lResult.error_code = DataPackageErrorEnum.ERROR;
				}
			}
			else
			{
				lResult.error_code = DataPackageErrorEnum.INVALID_SESSION_ID;
			}

			return lResult;
		}

		/// <summary>
		/// Set a new assigned current baseline to an element.
		/// </summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pElementId">The element Id you want to be assigned.</param>
		/// <param name="pBLVersion">The baseline version to assign.</param>
		/// <param name="pExpDate">The expiration date.</param>
		/// <returns>A structure that contains the error code plus a request id.</returns>
		private DataPackageResult assignCurrentBaselineToElement(Guid pSessionId, string pElementId, string pBLVersion, DateTime pExpDate)
		{
			DataPackageResult lResult = new DataPackageResult();
			lResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;

			if (_sessionManager.IsSessionValid(pSessionId))
			{
				if (pExpDate.Date < DateTime.Today.Date)
				{
					lResult.error_code = DataPackageErrorEnum.INVALID_EXPIRATION_DATEANDTIME;
				}
				else
				{
					Guid lrequestId;
					string lGetResult = _sessionManager.GenerateRequestID(pSessionId, out lrequestId);
					lResult.reqId = lrequestId;

					if (lrequestId != Guid.Empty)
					{
						if (!DataPackageService.mIsVersionValid(pBLVersion, false))
						{
							lResult.error_code = DataPackageErrorEnum.INVALID_BASELINE_VERSION;
						}
						else
						{
							lResult.error_code = checkIfElementExists(pElementId);

							if (lResult.error_code == DataPackageErrorEnum.REQUEST_ACCEPTED)
							{
								lResult.reqId = lrequestId;
								DataPackageService.assignCurrentBaseline(pElementId, pBLVersion, pExpDate, ref lResult);
							}
							else if (lResult.error_code != DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE)
							{
								lResult.error_code = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
							}
						}
					}
					else
					{
						lResult.error_code = DataPackageErrorEnum.ERROR;
					}
				}
			}
			else
			{
				lResult.error_code = DataPackageErrorEnum.INVALID_SESSION_ID;
			}
			return lResult;
		}

		#endregion

		#region public static methods

		/// <summary>
		/// Send Notification to Ground App, specific to Element Id.
		/// <param name="requestId">The RequestId for the corresponding request.</param>
		/// <param name="pStatus">Status : Completed/Failed/Processing.</param>
		/// <param name="pElementId">The concerned ElementId.</param>
		/// </summary>
		public static void sendElementIdNotificationToGroundApp(Guid pRequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum pStatus, string pElementId)
		{
            if (LogManager.IsTraceActive(TraceType.INFO))
            {
                string message = "sendElementIdNotificationToGroundApp : pElementId : {0}; ReqID = {1}; pStatus = {2}";
                mWriteLog(TraceType.INFO, "sendElementIdNotificationToGroundApp", null, message, pElementId, pRequestId, pStatus);
            }

			// Request ID must be a GUID, but may be followed by the "|<CRC>" that should be removed

			try
			{
				// Update request state to completed if the notification Is ok.
				if (pStatus == PIS.Ground.GroundCore.AppGround.NotificationIdEnum.Completed
					|| pStatus == PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionCompleted
					|| pStatus == PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageBaselineForcingCompleted
					|| pStatus == PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageBaselineClearForcingCompleted)
				{
					UpdateRequestStateToCompleted(pRequestId, pElementId);
					DistributingElementData distElementData = new DistributingElementData();
					distElementData.requestId = pRequestId;
					distElementData.distributionStatus = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.Completed;
					lock (_distributingElementDic)
					{
						_distributingElementDic[pElementId] = distElementData;
					}
				}

				// Baseline Deployments Status

				// Updating on the HistoryLogger the baseline deployments statuses based
				// on that SIF notification (from PIS Embedded - Media Controller)
				_baselineStatusUpdater.ProcessSIFNotification(pElementId, pStatus);

				//serialize ElementId.
				using (StringWriter lstr = new StringWriter())
				{
					_stringXmlSerializer.Serialize(lstr, pElementId);
					sendNotificationToGroundApp(pRequestId, pStatus, lstr.ToString());
				}
			}
			catch (Exception ex)
			{
				mWriteLog(TraceType.ERROR, "sendElementIdNotificationToGroundApp", ex, ex.Message);
			}
		}

		/// <summary>
		/// Send Notification to Ground App.
		/// <param name="requestId"> The RequestId for the corresponding request </param>
		/// <param name="pStatus"> Status : Completed/Failed/Processing </param>
		/// <param name="pParameter"> The serialized parameter under the form of a string</param>
		/// </summary>
		public static void sendNotificationToGroundApp(Guid pRequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum pStatus, string pParameter)
		{
			mWriteLog(TraceType.INFO, "sendNotificationToGroundApp", null, Logs.INFO_NOTIFICATIONTOAPPGROUND, pRequestId, pStatus);

			try
			{
				//Send Notification to AppGround
				_notificationSender.SendNotification(pStatus, pParameter, pRequestId);
			}
			catch (Exception ex)
			{
				mWriteLog(TraceType.EXCEPTION, "sendNotificationToGroundApp", ex, Logs.INFO_FUNCTION_EXCEPTION_MESSAGE, "sendNotificationToGroundApp", ex.Message);
			}
		}

		/// <summary>
		/// Distributes baselines assigned to element address.
		/// </summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pTargetAddress">The element(s) address you want to be assigned.</param>
		/// <param name="pBLAttributes">The distribution attributes (TransferMode, fileCompression, transferDate, transferExpirationDate, priority).</param>
		/// <param name="pReqTimeout">The permitted timeout to execute the request.</param>
		/// <returns>A structure that contains the error code plus a request id.</returns>        
		private DataPackageResult distributeBaseline(Guid pSessionId, TargetAddressType pTargetAddress, BaselineDistributionAttributes pBLAttributes, bool pIncr)
		{
			DataPackageResult lResult = new DataPackageResult();
			lResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;

			if (_sessionManager.IsSessionValid(pSessionId))
			{
				if (pBLAttributes.transferExpirationDate > pBLAttributes.transferDate)
				{
					Guid lrequestId;
					string lGetResult = _sessionManager.GenerateRequestID(pSessionId, out lrequestId);
					lResult.reqId = lrequestId;
					if (lrequestId != Guid.Empty)
					{
						List<string> lElementsIDs;

						lResult.error_code = mGetElementsListByTargetAddress(pTargetAddress, out lElementsIDs);

						if (lResult.error_code == DataPackageErrorEnum.REQUEST_ACCEPTED)
						{
							List<DataPackageService.CancellableTransferTaskInfo> lCancellableTransfers;
							if (getCancellableTransferTasks(out lCancellableTransfers) && lCancellableTransfers != null && LogManager.IsTraceActive(TraceType.INFO))
							{
								StringBuilder lCancellableTransfersInfo = new StringBuilder(30 + 30 * lCancellableTransfers.Count);
                                lCancellableTransfers.ForEach(lTask => lCancellableTransfersInfo.Append(lTask.ToString()).Append(' '));

								mWriteLog(TraceType.INFO, "distributeBaseline", null, Logs.INFO_PENDING_TRANSFERS_LIST, lCancellableTransfersInfo.ToString());
							}

							foreach (string lElID in lElementsIDs)
							{
								mWriteLog(TraceType.INFO, "distributeBaseline", null, Logs.INFO_BASELINE_DISTRIBUTING, lElID);

								string baselineVersion;
								DateTime baselineActivationDate;
								DateTime baselineExpirationDate;
								getFutureBaseline(lResult, lElID, out baselineActivationDate, out baselineExpirationDate, out baselineVersion);

								if (lResult.error_code == DataPackageErrorEnum.REQUEST_ACCEPTED)
								{
									cancelTasksInProgress(lCancellableTransfers, lElID);

									IRequestContext request = _requestFactory.CreateBaselineDistributingRequestContext(
										null,
										lElID,
										lResult.reqId,
										pSessionId,
										pBLAttributes,
										pIncr,
										baselineVersion,
										baselineActivationDate,
										baselineExpirationDate);

									if (request != null)
									{
										_requestManager.AddRequest(request);

										DistributingElementData distElementData = new DistributingElementData();
										distElementData.requestId = lrequestId;
										distElementData.distributionStatus = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionTransferring;

										lock (_distributingElementDic)
										{
											_distributingElementDic[lElID] = distElementData;
										}
									}
									else
									{
										mWriteLog(TraceType.ERROR, "distributeBaseline", null, Logs.ERROR_CREATE_DISTRIBUTION_REQUEST, lElID ?? "???", baselineVersion);
										lResult.reqId = Guid.Empty;
										lResult.error_code = DataPackageErrorEnum.ERROR;
									}
								}
								else
								{
									lResult.reqId = Guid.Empty;
								}
							}
						}
					}
					else
					{
						lResult.error_code = DataPackageErrorEnum.ERROR;
					}
				}
				else
				{
					lResult.error_code = DataPackageErrorEnum.INVALID_EXPIRATION_DATEANDTIME;
				}
			}
			else
			{
				lResult.error_code = DataPackageErrorEnum.INVALID_SESSION_ID;
			}

			return lResult;
		}

		private static void cancelTasksInProgress(List<DataPackageService.CancellableTransferTaskInfo> lCancellableTransfers, string lElID)
		{
			if (lCancellableTransfers != null && lCancellableTransfers.Count > 0)
			{
				// The previous task(s) to cancel must be targeting the same train(s) as the
				// new transfer task.
				var lCurrentTransfer = new CancellableTransferTaskInfo(0,
					new List<string>(new string[] { lElID }));

				List<CancellableTransferTaskInfo> lMatchList = lCancellableTransfers.FindAll((lTask) => lTask.IsMatching(lCurrentTransfer));

				if (lMatchList != null && lMatchList.Count > 0)
				{
					// Now asking T2G to cancel the useless transfer task(s)
					bool lCancelSuccess = cancelTransferTasks(lMatchList);

					StringBuilder lCancelInfo = new StringBuilder();
					lMatchList.ForEach(lTask => { lCancelInfo.Append(lTask.TaskID); lCancelInfo.Append(" "); });

					if (lCancelSuccess == false)
					{

						mWriteLog(TraceType.WARNING, "distributeBaseline", null, Logs.WARNING_TASK_CANCELING_PROBLEM, lCancelInfo.ToString());
					}
					else
					{
						mWriteLog(TraceType.INFO, "distributeBaseline", null, Logs.INFO_TASK_IS_CANCELED, lCancelInfo.ToString());

						// Remove the cancelled transfer from the local list
						lock (_distributingElementDic)
						{
							_distributingElementDic.Remove(lElID);
						}
					}
				}
			}
		}

		private void getFutureBaseline(DataPackageResult lResult, string lElID, out DateTime baselineActivationDate, out DateTime baselineExpirationDate, out string baselineVersion)
		{
			baselineVersion = "";
			baselineActivationDate = DateTime.MinValue;
			baselineExpirationDate = DateTime.MaxValue;

			try
			{
                using (IRemoteDataStoreClient remoteDataStore = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                {
                    DataContainer elementDescription = remoteDataStore.getElementBaselinesDefinitions(lElID);
                    baselineVersion = elementDescription.getStrValue("AssignedFutureBaseline");
                    if (!string.IsNullOrEmpty(baselineVersion) && remoteDataStore.checkIfBaselineExists(baselineVersion))
                    {
                        mWriteLog(TraceType.INFO, "distributeBaseline", null, Logs.INFO_FUTURE_BASELINE, lElID, baselineVersion);
                        DataContainer baselineDefinitionContainer = remoteDataStore.getBaselineDefinition(baselineVersion);
                        remoteDataStore.checkDataPackagesAvailability(lResult.reqId, baselineDefinitionContainer);
                        baselineActivationDate = DateTime.Parse(elementDescription.getStrValue("AssignedFutureBaselineActivationDate"));
                        baselineExpirationDate = DateTime.Parse(elementDescription.getStrValue("AssignedFutureBaselineExpirationDate"));
                    }
                    else
                    {
                        lResult.error_code = DataPackageErrorEnum.INVALID_BASELINE_VERSION;
                    }
                }
			}
			catch (FaultException ex)
			{
				if (ex.Code.Name == RemoteDataStore.RemoteDataStoreExceptionCodeEnum.UNKNOWN_ELEMENT_ID.ToString())
				{
					lResult.error_code = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
					mWriteLog(TraceType.ERROR, "distributeBaseline", null, Logs.ERROR_ELEMENT_NOT_FOUND, lElID);
				}
				if (ex.Code.Name == RemoteDataStore.RemoteDataStoreExceptionCodeEnum.UNKNOWN_BASELINE_VERSION.ToString())
				{
					lResult.error_code = DataPackageErrorEnum.INVALID_BASELINE_VERSION;
					mWriteLog(TraceType.ERROR, "distributeBaseline", null, Logs.ERROR_UNKNOWN_BASELINE_VERSION, lElID);
				}
			}
			catch (TimeoutException ex)
			{
				lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
				mWriteLog(TraceType.EXCEPTION, "distributeBaseline", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
			}
			catch (CommunicationException ex)
			{
				lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
				mWriteLog(TraceType.EXCEPTION, "distributeBaseline", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
			}
			catch (Exception ex)
			{
				lResult.error_code = DataPackageErrorEnum.ERROR;
				mWriteLog(TraceType.EXCEPTION, "distributeBaseline", ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
			}
		}

		/// <summary>
		/// Force an Element On train to use a future baseline.
		/// </summary>
		/// <param name="pSessionId"> The Id of the session in which the request is executed.</param>
		/// <param name="pElementId"> The Id of the Element where the request is to be sent.</param>
		/// <param name="pReqTimeout"> The permitted timeout to execute the request.</param>
		private DataPackageResult forceAddresseesFutureBaseline(Guid pSessionId, string pElementId, uint pReqTimeout)
		{
			var pElementAdress = new TargetAddressType { Type = AddressTypeEnum.Element, Id = pElementId };
			return ProcessAddresseesBaselineCommand(pSessionId, pElementAdress, pReqTimeout, BaselineCommandType.FORCE_FUTURE);
		}

		/// <summary>
		/// Force an Element On train to use an archived baseline.
		/// </summary>
		/// <param name="pSessionId"> The Id of the session in which the request is executed.</param>
		/// <param name="pElementId"> The Id of the Element where the request is to be sent.</param>
		/// <param name="pReqTimeout"> The permitted timeout to execute the request.</param>
		private DataPackageResult forceAddresseesArchivedBaseline(Guid pSessionId, string pElementId, uint pReqTimeout)
		{
			var pElementAdress = new TargetAddressType { Type = AddressTypeEnum.Element, Id = pElementId };
			return ProcessAddresseesBaselineCommand(pSessionId, pElementAdress, pReqTimeout, BaselineCommandType.FORCE_ARCHIVED);
		}

		/// <summary>
		/// Clears a forcing status of  an Element On train.
		/// </summary>
		/// <param name="pSessionId"> The Id of the session in which the request is executed.</param>
		/// <param name="pElementId"> The Id of the Element where the request is to be sent.</param>
		/// <param name="pReqTimeout"> The permitted timeout to execute the request.</param>
		private DataPackageResult clearAddreeseesForcingStatus(Guid pSessionId, string pElementId, uint pReqTimeout)
		{
			var pElementAdress = new TargetAddressType { Type = AddressTypeEnum.Element, Id = pElementId };
			return ProcessAddresseesBaselineCommand(pSessionId, pElementAdress, pReqTimeout, BaselineCommandType.CLEAR_FORCING);
		}

		/// <summary>
		/// Process a Addressee' Baseline request, it checks the validity of input parameters, dispatches the request to another thread and returns right away the response.
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pElementAddress">The Id of the Element where the request is to be sent.</param>
		/// <param name="pReqTimeout">The permitted timeout to execute the request.</param>
		/// <param name="pCommandType">The commandType: FORCE_FUTURE/FORCE_ARCHIVED/CLEAR_FORCING.</param>
		/// </summary>
		private DataPackageResult ProcessAddresseesBaselineCommand(Guid pSessionId, TargetAddressType pElementAddress, uint pReqTimeout, BaselineCommandType pCommandType)
		{
			DataPackageResult lResult = new DataPackageResult();
			lResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;

			if (pReqTimeout >= 0 && pReqTimeout <= 43200)
			{
				if (_sessionManager.IsSessionValid(pSessionId))
				{
					Guid lrequestId;
					string lGetResult = _sessionManager.GenerateRequestID(pSessionId, out lrequestId);
					lResult.reqId = lrequestId;

					if (lrequestId != Guid.Empty)
					{
						ElementList<AvailableElementData> lElList;
						T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableElementDataByTargetAddress(pElementAddress, out lElList);
						switch (lRqstResult)
						{
							case T2GManagerErrorEnum.eSuccess:
								{
									// Queue to request list
									foreach (AvailableElementData element in lElList)
									{
										if (pCommandType == BaselineCommandType.CLEAR_FORCING)
										{
											sendElementIdNotificationToGroundApp(lrequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageBaselineClearForcingProcessing, element.ElementNumber);
										}
										else
										{
											sendElementIdNotificationToGroundApp(lrequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageBaselineForcingProcessing, element.ElementNumber);
										}

										ServiceInfo serviceInfo;
										lRqstResult = _t2gManager.GetAvailableServiceData(element.ElementNumber, (int)eServiceID.eSrvSIF_DataPackageServer, out serviceInfo);
										switch (lRqstResult)
										{
											case T2GManagerErrorEnum.eSuccess:
												{
													string endpoint = "http://" + serviceInfo.ServiceIPAddress + ":" + serviceInfo.ServicePortNumber;

													IRequestContext request = _requestFactory.CreateBaselineForcingRequestContext(
														endpoint,
														element.ElementNumber,
														lrequestId,
														pSessionId,
														pReqTimeout,
														pCommandType);

													if (request != null)
													{
														_requestManager.AddRequest(request);

														lResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;
														lResult.reqId = lrequestId;

														if (pCommandType == BaselineCommandType.CLEAR_FORCING)
														{
															sendElementIdNotificationToGroundApp(lrequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageBaselineClearForcingWaitingToSend, element.ElementNumber);
														}
														else
														{
															sendElementIdNotificationToGroundApp(lrequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageBaselineForcingWaitingToSend, element.ElementNumber);
														}
													}
													else
													{
														sendElementIdNotificationToGroundApp(lrequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageBaselineForcingFailed, element.ElementNumber);
													}
												}
												break;
											case T2GManagerErrorEnum.eT2GServerOffline:
												lResult.error_code = DataPackageErrorEnum.T2G_SERVER_OFFLINE;
												break;
											case T2GManagerErrorEnum.eElementNotFound:
												switch (pElementAddress.Type)
												{
													case AddressTypeEnum.Element:
														lResult.error_code = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
														break;
													case AddressTypeEnum.MissionOperatorCode:
														lResult.error_code = DataPackageErrorEnum.INVALID_MISSION_ID;
														break;
													case AddressTypeEnum.MissionCode:
														lResult.error_code = DataPackageErrorEnum.INVALID_MISSION_ID;
														break;
													default:
														break;
												}
												break;
											case T2GManagerErrorEnum.eServiceInfoNotFound:
												lResult.error_code = DataPackageErrorEnum.SERVICE_INFO_NOT_FOUND;
												break;
											default:
                                                lResult.error_code = DataPackageErrorEnum.ERROR;
                                                break;
										}
									}
								}
								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								lResult.error_code = DataPackageErrorEnum.T2G_SERVER_OFFLINE;
								break;
							case T2GManagerErrorEnum.eElementNotFound:
								switch (pElementAddress.Type)
								{
									case AddressTypeEnum.Element:
										lResult.error_code = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
										break;
									case AddressTypeEnum.MissionOperatorCode:
										lResult.error_code = DataPackageErrorEnum.INVALID_MISSION_ID;
										break;
									case AddressTypeEnum.MissionCode:
										lResult.error_code = DataPackageErrorEnum.INVALID_MISSION_ID;
										break;
									default:
										break;
								}
								break;
							default:
								break;
						}
					}
				}
				else
				{
					lResult.error_code = DataPackageErrorEnum.INVALID_SESSION_ID;
				}
			}
			else
			{
				lResult.error_code = DataPackageErrorEnum.INVALID_TIMEOUT;
			}

			return lResult;
		}

		private DataPackageErrorEnum checkIfElementExists(string pEID)
		{
			DataPackageErrorEnum lResult = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
			AvailableElementData lElData;
			T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableElementDataByElementNumber(pEID, out lElData);

			if (lRqstResult == T2GManagerErrorEnum.eT2GServerOffline)
			{
				sendNotificationToGroundApp(Guid.Empty, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageT2GServerOffline, string.Empty);
			}

			if (lElData == null || string.Compare(pEID, lElData.ElementNumber, true) != 0)
			{
				try
				{
                    using (IRemoteDataStoreClient lRemDSProxy = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                    {
                        if (lRemDSProxy.checkIfElementExists(pEID))
                        {
                            lResult = DataPackageErrorEnum.REQUEST_ACCEPTED;
                        }
                    }
				}
				catch (TimeoutException)
				{
					lResult = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
				}
				catch (CommunicationException)
				{
					lResult = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
				}
				catch (Exception)
				{
					lResult = DataPackageErrorEnum.ERROR;
				}
			}
			else
			{
				lResult = DataPackageErrorEnum.REQUEST_ACCEPTED;
			}

			return lResult;
		}

		private DataPackageErrorEnum mGetElementsListByTargetAddress(TargetAddressType pTargetAddress, out List<string> lElementIDs)
		{
			DataPackageErrorEnum lResult = DataPackageErrorEnum.REQUEST_ACCEPTED;

			lElementIDs = new List<string>();

			ElementList<AvailableElementData> lElements;

			T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableElementDataByTargetAddress(pTargetAddress, out lElements);

			switch (lRqstResult)
			{
				case T2GManagerErrorEnum.eSuccess:
					foreach (AvailableElementData lElement in lElements)
					{
						lElementIDs.Add(lElement.ElementNumber);
					}

					if (lElementIDs.Count == 0)
					{
						lResult = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
					}
					break;

				case T2GManagerErrorEnum.eT2GServerOffline:
					lResult = DataPackageErrorEnum.T2G_SERVER_OFFLINE;
					break;

				case T2GManagerErrorEnum.eElementNotFound:
					switch (pTargetAddress.Type)
					{
						case AddressTypeEnum.Element:
							lResult = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
							break;
						default:
							lResult = DataPackageErrorEnum.INVALID_MISSION_ID;
							break;
					}
					break;

				default:
					lResult = DataPackageErrorEnum.ERROR;
					break;
			}

			return lResult;
		}

		/// <summary>
		/// Set a new assigned future baseline to an element.
		/// </summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pTargetAddress">The element you want to be assigned.</param>
		/// <param name="pBLVersion">The baseline version to assign.</param>
		/// <param name="pActDate">The activation date for the baseline.</param>
		/// <param name="pExpDate">The expiration date.</param>
		/// <returns>A structure that contains the error code plus a request id.</returns>
		private DataPackageResult assignFutureBaselineToElement(Guid pSessionId, TargetAddressType pTargetAddress, string pBLVersion, DateTime pActDate, DateTime pExpDate)
		{
			mWriteLog(TraceType.DEBUG, "assignFutureBaselineToElement", null, Logs.DEBUG_ENTERING_FUNCTION, "assignFutureBaselineToElement");
			DataPackageResult lResult = new DataPackageResult();
			lResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;

			if (_sessionManager.IsSessionValid(pSessionId))
			{
				Guid lrequestId;
				if (pExpDate.Date < DateTime.Today.Date)
				{
					mWriteLog(TraceType.EXCEPTION, "assignFutureBaselineToElement", null, Logs.ERROR_INVALID_EXPIRATION_DATE, pExpDate, DateTime.Today);
					lResult.error_code = DataPackageErrorEnum.INVALID_EXPIRATION_DATEANDTIME;
					return lResult;
				}
				string lGetResult = _sessionManager.GenerateRequestID(pSessionId, out lrequestId);
				if (lrequestId != Guid.Empty)
				{
					if (!DataPackageService.mIsVersionValid(pBLVersion, false))
					{
						mWriteLog(TraceType.EXCEPTION, "assignFutureBaselineToElement", null, Logs.ERROR_INVALID_BASELINE_VERSION, pBLVersion);
						lResult.error_code = DataPackageErrorEnum.INVALID_BASELINE_VERSION;
					}
					else
					{
						if (pActDate >= pExpDate)
						{
							mWriteLog(TraceType.EXCEPTION, "assignFutureBaselineToElement", null, Logs.ERROR_INVALID_ACTIVATION_DATE, pActDate, pExpDate);
							lResult.error_code = DataPackageErrorEnum.INVALID_EXPIRATION_DATEANDTIME;
						}
						else
						{
							ElementList<AvailableElementData> lElList;
							T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableElementDataByTargetAddress(pTargetAddress, out lElList);
							switch (lRqstResult)
							{
								case T2GManagerErrorEnum.eSuccess:
									{
										foreach (AvailableElementData lEID in lElList)
										{
											lResult.error_code = checkIfElementExists(lEID.ElementNumber);

											if (lResult.error_code != DataPackageErrorEnum.REQUEST_ACCEPTED)
											{
												if (lResult.error_code == DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE)
												{
													mWriteLog(TraceType.EXCEPTION, "assignFutureBaselineToElement", null, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
													lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
												}
												else
												{
													mWriteLog(TraceType.EXCEPTION, "assignFutureBaselineToElement", null, Logs.ERROR_ELEMENT_NOT_FOUND, lEID.ElementNumber);
													lResult.error_code = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
												}

												break;
											}
										}
										if (lResult.error_code == DataPackageErrorEnum.REQUEST_ACCEPTED)
										{
											lResult.reqId = lrequestId;
											foreach (AvailableElementData lEID in lElList)
											{
												DataPackageResult lTempResult = new DataPackageResult { reqId = lrequestId, error_code = DataPackageErrorEnum.REQUEST_ACCEPTED };
												DataPackageService.assignFutureBaseline(lEID.ElementNumber, pBLVersion, pActDate, pExpDate, ref lTempResult);
												if (lTempResult.error_code != DataPackageErrorEnum.REQUEST_ACCEPTED)
												{
													mWriteLog(TraceType.EXCEPTION, "assignFutureBaselineToElement", null, Logs.ERROR_ASSIGN_BASELINE_FAILED, "future", pBLVersion, lEID.ElementNumber);
													lResult.error_code = lTempResult.error_code;
												}
											}
										}
									}
									break;
								case T2GManagerErrorEnum.eT2GServerOffline:
									mWriteLog(TraceType.EXCEPTION, "assignFutureBaselineToElement", null, Logs.ERROR_T2G_SERVER_OFFLINE);
									lResult.error_code = DataPackageErrorEnum.T2G_SERVER_OFFLINE;
									break;
								case T2GManagerErrorEnum.eElementNotFound:
									mWriteLog(TraceType.EXCEPTION, "assignFutureBaselineToElement", null, Logs.ERROR_ELEMENT_NOT_FOUND, pTargetAddress.Id);

									switch (pTargetAddress.Type)
									{
										case AddressTypeEnum.Element:
											lResult.error_code = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
											break;
										case AddressTypeEnum.MissionOperatorCode:
											lResult.error_code = DataPackageErrorEnum.INVALID_MISSION_ID;
											break;
										case AddressTypeEnum.MissionCode:
											lResult.error_code = DataPackageErrorEnum.INVALID_MISSION_ID;
											break;
										default:
											break;
									}
									break;
								default:
									break;
							}
						}
					}
				}
				else
				{
					mWriteLog(TraceType.EXCEPTION, "assignFutureBaselineToElement", null, Logs.ERROR_INVALID_REQUEST_ID);
					lResult.reqId = Guid.Empty;
					lResult.error_code = DataPackageErrorEnum.ERROR;
				}
			}
			else
			{
				mWriteLog(TraceType.EXCEPTION, "assignFutureBaselineToElement", null, Logs.ERROR_INVALID_SESSION_ID, pSessionId);
				lResult.error_code = DataPackageErrorEnum.INVALID_SESSION_ID;
			}

			return lResult;
		}

		/// <summary>
		/// Set a new assigned current baseline to an element.
		/// </summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pTargetAddress">The element you want to be assigned.</param>
		/// <param name="pBLVersion">The baseline version to assign.</param>
		/// <param name="pExpDate">The expiration date.</param>
		/// <returns>A structure that contains the error code plus a request id.</returns>
		private DataPackageResult assignCurrentBaselineToElement(Guid pSessionId, TargetAddressType pElementAddress, string pBLVersion, DateTime pExpDate)
		{
			DataPackageResult lResult = new DataPackageResult();
			lResult.error_code = DataPackageErrorEnum.ERROR;

			if (_sessionManager.IsSessionValid(pSessionId))
			{
				Guid lrequestId;
				if (pExpDate.Date < DateTime.Today.Date)
				{
					mWriteLog(TraceType.EXCEPTION, "assignCurrentBaselineToElement", null, Logs.ERROR_INVALID_EXPIRATION_DATE, pExpDate, DateTime.Today);
					lResult.error_code = DataPackageErrorEnum.INVALID_EXPIRATION_DATEANDTIME;
				}
				else
				{
					string lGetResult = _sessionManager.GenerateRequestID(pSessionId, out lrequestId);
					if (lrequestId != Guid.Empty)
					{
						if (!DataPackageService.mIsVersionValid(pBLVersion, false))
						{
							mWriteLog(TraceType.EXCEPTION, "assignCurrentBaselineToElement", null, Logs.ERROR_INVALID_BASELINE_VERSION, pBLVersion);
							lResult.error_code = DataPackageErrorEnum.INVALID_BASELINE_VERSION;
						}
						else
						{
							ElementList<AvailableElementData> lElList;
							T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableElementDataByTargetAddress(pElementAddress, out lElList);
							switch (lRqstResult)
							{
								case T2GManagerErrorEnum.eSuccess:
									{
										foreach (AvailableElementData lEID in lElList)
										{
											lResult.error_code = checkIfElementExists(lEID.ElementNumber);

											if (lResult.error_code != DataPackageErrorEnum.REQUEST_ACCEPTED)
											{
												if (lResult.error_code == DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE)
												{
													mWriteLog(TraceType.EXCEPTION, "assignCurrentBaselineToElement", null, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
													lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
												}
												else
												{
													mWriteLog(TraceType.EXCEPTION, "assignCurrentBaselineToElement", null, Logs.ERROR_ELEMENT_NOT_FOUND, lEID.ElementNumber);
													lResult.error_code = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
												}

												break;
											}
										}
										if (lResult.error_code == DataPackageErrorEnum.REQUEST_ACCEPTED)
										{
											foreach (AvailableElementData lEID in lElList)
											{
												DataPackageResult lTempResult = new DataPackageResult { reqId = lrequestId, error_code = DataPackageErrorEnum.REQUEST_ACCEPTED };
												DataPackageService.assignCurrentBaseline(lEID.ElementNumber, pBLVersion, pExpDate, ref lTempResult);
												if (lTempResult.error_code != DataPackageErrorEnum.REQUEST_ACCEPTED)
												{
													mWriteLog(TraceType.EXCEPTION, "assignCurrentBaselineToElement", null, Logs.ERROR_ASSIGN_BASELINE_FAILED, "current", pBLVersion, lEID.ElementNumber);
													lResult.error_code = lTempResult.error_code;
												}
											}
										}
									}
									break;
								case T2GManagerErrorEnum.eT2GServerOffline:
									mWriteLog(TraceType.EXCEPTION, "assignCurrentBaselineToElement", null, Logs.ERROR_T2G_SERVER_OFFLINE, pBLVersion);
									lResult.error_code = DataPackageErrorEnum.T2G_SERVER_OFFLINE;
									break;
								case T2GManagerErrorEnum.eElementNotFound:
									mWriteLog(TraceType.EXCEPTION, "assignCurrentBaselineToElement", null, Logs.ERROR_ELEMENT_NOT_FOUND, pElementAddress.Id);
									switch (pElementAddress.Type)
									{
										case AddressTypeEnum.Element:
											lResult.error_code = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
											break;
										case AddressTypeEnum.MissionOperatorCode:
											lResult.error_code = DataPackageErrorEnum.INVALID_MISSION_ID;
											break;
										case AddressTypeEnum.MissionCode:
											lResult.error_code = DataPackageErrorEnum.INVALID_MISSION_ID;
											break;
										default:
											break;
									}
									break;
								default:
									break;
							}
						}
					}
					else
					{
						mWriteLog(TraceType.EXCEPTION, "assignCurrentBaselineToElement", null, Logs.ERROR_INVALID_REQUEST_ID);
						lResult.reqId = Guid.Empty;
						lResult.error_code = DataPackageErrorEnum.ERROR;
					}
				}
			}
			else
			{
				mWriteLog(TraceType.EXCEPTION, "assignCurrentBaselineToElement", null, Logs.ERROR_INVALID_SESSION_ID, pSessionId);
				lResult.error_code = DataPackageErrorEnum.INVALID_SESSION_ID;
			}
			return lResult;
		}

		#endregion

		#region public methods

		/// <summary>
		/// Return the list of baselines already defines in the RemoteDataStore (in the data base).
		/// </summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pType">The type of the baseline list, that has to be returned.</param>
		/// <returns>A structure that contains the error code plus the list of baselines definitions.</returns>
		public GetBaselineListResult getBaselinesList(Guid pSessionId, BaselinesListType pListType)
		{
			GetBaselineListResult lResult = new GetBaselineListResult();
			lResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;

			if (!_sessionManager.IsSessionValid(pSessionId))
			{
				lResult.error_code = DataPackageErrorEnum.INVALID_SESSION_ID;
			}
			else
			{
				try
				{
                    using (IRemoteDataStoreClient lRemDSProxy = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                    {
                        if (pListType == BaselinesListType.ALL ||
                            pListType == BaselinesListType.DEFINED)
                        {
                            DataContainer lBLList = lRemDSProxy.getBaselinesDefinitions();
                            lResult.baselineDef = DataTypeConversion.fromDataContainerToBaselinesDefinitionsList(lBLList);
                        }

                        if (pListType == BaselinesListType.ALL ||
                            pListType == BaselinesListType.UNDEFINED)
                        {
                            DataContainer lUBLList = lRemDSProxy.getUndefinedBaselinesList();
                            List<BaselineDefinition> lBaselinesList = DataTypeConversion.fromDataContainerToBaselinesDefinitionsList(lUBLList);

                            if (lBaselinesList.Count > 0)
                            {
                                foreach (BaselineDefinition lBLDef in lBaselinesList)
                                {
                                    if (string.IsNullOrEmpty(lBLDef.PISBaseDataPackageVersion) ||
                                        string.IsNullOrEmpty(lBLDef.PISBaseDataPackageVersion) ||
                                        string.IsNullOrEmpty(lBLDef.PISBaseDataPackageVersion) ||
                                        string.IsNullOrEmpty(lBLDef.PISBaseDataPackageVersion))
                                    {
                                        continue;
                                    }

                                    lResult.baselineDef.Add(lBLDef);
                                }
                            }
                        }
                    }
				}
				catch (TimeoutException ex)
				{
					mWriteLog(TraceType.EXCEPTION, "getBaselinesList", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);

					lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
				}
				catch (CommunicationException ex)
				{
					mWriteLog(TraceType.EXCEPTION, "getBaselinesList", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);

					lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
				}
				catch (Exception ex)
				{
					mWriteLog(TraceType.EXCEPTION, "getBaselinesList", ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
					lResult.error_code = DataPackageErrorEnum.ERROR;
				}
			}
			return lResult;
		}

		/// <summary>
		/// Return the baselines corresping to an element : the on board baselines, asking T2G, the
		/// assigned baselines, asking RemoteDataStore.
		/// </summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pTargetAddress">The element(s) address you want the baselines.</param>
		/// <param name="pElementAddress">The element(s) address you want the baselines (OBSOLETE).</param>
		/// <returns>A structure that contains the error code plus the element description.</returns>
		public GetAdresseesDataPackageBaselinesResult getAddresseesDataPackagesBaselines(Guid pSessionId, TargetAddressType pTargetAddress, TargetAddressType pElementAddress)
		{
			if (pTargetAddress != null)
			{
				return getAddresseesDataPackagesBaselines(pSessionId, pTargetAddress);
			}
			else
			{
				return getAddresseesDataPackagesBaselines(pSessionId, pElementAddress);
			}
		}

		/// <summary>
		/// Store a list of data packages in the RemoteDataStore.
		/// </summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pFilesURLs">The list of urls of files to store.</param>
		/// <returns>A structure that contains the error code plus a request id.</returns>
		public DataPackageResult uploadDataPackages(Guid pSessionId, List<string> pFilesURLs)
		{
			DataPackageResult lResult = new DataPackageResult();
			lResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;

			if (_sessionManager.IsSessionValid(pSessionId))
			{
				Guid lrequestId;
				string lGetResult = _sessionManager.GenerateRequestID(pSessionId, out lrequestId);
				lResult.reqId = lrequestId;

				if (lrequestId != Guid.Empty)
				{
					foreach (string lFile in pFilesURLs)
					{
						try
						{
							//lUri is used to check if the format is a good uri.
							Uri lUri = new Uri(lFile);
							try
							{
                                using (IRemoteDataStoreClient lRemProx = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                                {
                                    if (!lRemProx.checkUrl(lrequestId, lFile))
                                    {
                                        lResult.error_code = DataPackageErrorEnum.FILE_NOT_FOUND;
                                    }
                                }
							}
							catch (TimeoutException ex)
							{
								mWriteLog(TraceType.EXCEPTION, "uploadDataPackages", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
								lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
								return lResult;
							}
							catch (CommunicationException ex)
							{
								mWriteLog(TraceType.EXCEPTION, "uploadDataPackages", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
								lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
								return lResult;
							}
							catch (Exception ex)
							{
								mWriteLog(TraceType.EXCEPTION, "uploadDataPackages", ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
								lResult.error_code = DataPackageErrorEnum.ERROR;
								return lResult;
							}
						}
						catch (UriFormatException)
						{
							lResult.error_code = DataPackageErrorEnum.INVALID_PATH;
						}
					}
					if (lResult.error_code == DataPackageErrorEnum.REQUEST_ACCEPTED)
					{
						foreach (string lFile in pFilesURLs)
						{
							try
							{
                                using (IRemoteDataStoreClient lRemProx = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                                {
                                    lRemProx.moveTheNewDataPackageFiles(lrequestId, lFile);
                                }
							}
							catch (TimeoutException ex)
							{
								mWriteLog(TraceType.EXCEPTION, "uploadDataPackages", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
								lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
								return lResult;
							}
							catch (CommunicationException ex)
							{
								mWriteLog(TraceType.EXCEPTION, "uploadDataPackages", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
								lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
								return lResult;
							}
							catch (Exception ex)
							{
								mWriteLog(TraceType.EXCEPTION, "uploadDataPackages", ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
								lResult.error_code = DataPackageErrorEnum.ERROR;
								return lResult;
							}
						}
						lResult.reqId = lrequestId;
					}
				}
				else
				{
					lResult.error_code = DataPackageErrorEnum.ERROR;
				}
			}
			else
			{
				lResult.error_code = DataPackageErrorEnum.INVALID_SESSION_ID;
				return lResult;
			}
			return lResult;
		}

		/// <summary>
		/// Add a new baseline definition to the RemoteDataStore.
		/// </summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pBLDef">The new baseline definition.</param>
		/// <returns>A structure that contains the error code plus a request id.</returns>
		public DataPackageResult defineNewBaseline(Guid pSessionId, BaselineDefinition pBLDef)
		{
			DataPackageResult lResult = new DataPackageResult();
			lResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;

			if (_sessionManager.IsSessionValid(pSessionId))
			{
				Guid lrequestId;
				string lGetResult = _sessionManager.GenerateRequestID(pSessionId, out lrequestId);
				lResult.reqId = lrequestId;

				if (lrequestId != Guid.Empty)
				{
					if (!DataPackageService.mIsVersionValid(pBLDef.BaselineVersion, false))
						lResult.error_code = DataPackageErrorEnum.INVALID_BASELINE_VERSION;
					if (!DataPackageService.mIsVersionValid(pBLDef.PISBaseDataPackageVersion, false))
						lResult.error_code = DataPackageErrorEnum.INVALID_PIS_BASE_DATA_PACKAGE_VERSION;
					if (!DataPackageService.mIsVersionValid(pBLDef.PISMissionDataPackageVersion, false))
						lResult.error_code = DataPackageErrorEnum.INVALID_PIS_MISSION_DATA_PACKAGE_VERSION;
					if (!DataPackageService.mIsVersionValid(pBLDef.PISInfotainmentDataPackageVersion, false))
						lResult.error_code = DataPackageErrorEnum.INVALID_PIS_INFOTAINMENT_DATA_PACKAGE_VERSION;
					if (!DataPackageService.mIsVersionValid(pBLDef.LMTDataPackageVersion, false))
						lResult.error_code = DataPackageErrorEnum.INVALID_LMT_DATA_PACKAGE_VERSION;

					if (lResult.error_code == DataPackageErrorEnum.REQUEST_ACCEPTED)
					{
						try
						{
                            using (IRemoteDataStoreClient lRemProx = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                            {
                                DataContainer lBLDef = DataTypeConversion.fromBaselineDefinitionToDataContainer(pBLDef);
                                lRemProx.setNewBaselineDefinition(lrequestId, lBLDef);
                                lRemProx.checkDataPackagesAvailability(lrequestId, lBLDef);
                                lResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;

                                // Check if there are any elements that are waiting for a baseline confirmation
                                //  with the provided packages version. If so - send them a "SetBaselineVersionRequest"
                                //  and clear the undefined baseline packages version in "ElementsDataStore" table
                                DataContainer pEList = lRemProx.getElementsDescription();
                                List<ElementDescription> lElements = DataTypeConversion.fromDataContainerToElementDescriptionList(pEList);

                                foreach (ElementDescription lEDsc in lElements)
                                {
                                    if (lEDsc.UndefinedBaselinePisBaseVersion == pBLDef.PISBaseDataPackageVersion &&
                                        lEDsc.UndefinedBaselinePisMissionVersion == pBLDef.PISMissionDataPackageVersion &&
                                        lEDsc.UndefinedBaselinePisInfotainmentVersion == pBLDef.PISInfotainmentDataPackageVersion &&
                                        lEDsc.UndefinedBaselineLmtVersion == pBLDef.LMTDataPackageVersion)
                                    {
                                        // Reset the undefined baseline packages version in ElementDataStore table
                                        lRemProx.setElementUndefinedBaselineParams(lEDsc.ElementID, "", "", "", "");

                                        DataPackageResult lCommandResult = ProcessSetBaselineVersionCommand(
                                            lEDsc.ElementID,
                                            pBLDef.BaselineVersion,
                                            pBLDef.PISBaseDataPackageVersion,
                                            pBLDef.PISMissionDataPackageVersion,
                                            pBLDef.PISInfotainmentDataPackageVersion,
                                            pBLDef.LMTDataPackageVersion);
                                    }
                                }
                            }
						}
						catch (TimeoutException ex)
						{
							mWriteLog(TraceType.EXCEPTION, "defineNewBaseline", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
							lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
							return lResult;
						}
						catch (CommunicationException ex)
						{
							mWriteLog(TraceType.EXCEPTION, "defineNewBaseline", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
							lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
							return lResult;
						}
						catch (Exception ex)
						{
							mWriteLog(TraceType.EXCEPTION, "defineNewBaseline", ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
							lResult.error_code = DataPackageErrorEnum.ERROR;
							return lResult;
						}
					}
				}
				else
				{
					lResult.error_code = DataPackageErrorEnum.ERROR;
				}
			}
			else
			{
				lResult.error_code = DataPackageErrorEnum.INVALID_SESSION_ID;
			}
			return lResult;
		}

		/// <summary>Delete the baseline definition described by version.</summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pVersion">The version. Could contains pattern like *.*.*.* or 1.1.*.*. But shoud be
		/// 4 digits.</param>
		/// <returns>Request result as DataPackageResultEnum.</returns>
		public DataPackageErrorEnum deleteBaselineDefinition(Guid pSessionId, string pVersion)
		{
			DataPackageErrorEnum lResult = DataPackageErrorEnum.REQUEST_ACCEPTED; ;

			if (DataPackageService.mIsVersionValid(pVersion, true) == false)
			{
				lResult = DataPackageErrorEnum.INVALID_BASELINE_VERSION;
				mWriteLog(TraceType.ERROR, "deleteBaselineDefinition", null, Logs.ERROR_INVALID_BASELINE_VERSION, pVersion);
			}
			else if (_sessionManager.IsSessionValid(pSessionId))
			{
				try
				{
                    using (IRemoteDataStoreClient lRemProx = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                    {
                        DataContainer lBLList = lRemProx.getBaselinesDefinitions();
                        List<BaselineDefinition> lBaselines = DataTypeConversion.fromDataContainerToBaselinesDefinitionsList(lBLList);

                        DataContainer lEList = lRemProx.getAssignedBaselinesVersions();
                        List<ElementDescription> lElements = DataTypeConversion.fromDataContainerToElementDescriptionList(lEList);

                        bool lFound = false;
                        bool lAssigned = false;

                        foreach (BaselineDefinition lBaseline in lBaselines)
                        {
                            if (DataPackageService.mDoesVersionMatchPattern(lBaseline.BaselineVersion, pVersion))
                            {
                                lFound = true;

                                foreach (ElementDescription lElem in lElements)
                                {
                                    if (lBaseline.BaselineVersion == lElem.AssignedCurrentBaseline ||
                                        lBaseline.BaselineVersion == lElem.AssignedFutureBaseline)
                                    {
                                        lAssigned = true;

                                        break;
                                    }
                                }

                                if (!lAssigned)
                                {
                                    lRemProx.deleteBaselineDefinition(lBaseline.BaselineVersion);
                                }
                            }
                        }

                        if (!lFound)
                        {
                            lResult = DataPackageErrorEnum.BASELINE_NOT_FOUND;
                            mWriteLog(TraceType.ERROR, "deleteBaselineDefinition", null, Logs.ERROR_BASELINE_NOT_FOUND);
                        }
                        else if (lAssigned)
                        {
                            if (DataPackageService.mIsVersionValid(pVersion, false) == true)
                            {
                                lResult = DataPackageErrorEnum.BASELINE_IS_ASSIGNED;
                                mWriteLog(TraceType.ERROR, "deleteBaselineDefinition", null, Logs.ERROR_BASELINE_IS_ASSIGNED, pVersion);
                            }
                            else
                            {
                                lResult = DataPackageErrorEnum.SOME_BASELINES_ARE_ASSIGNED;
                                mWriteLog(TraceType.ERROR, "deleteBaselineDefinition", null, Logs.ERROR_SOME_BASELINE_ARE_ASSIGNED);
                            }
                        }
                    }
				}
				catch (TimeoutException ex)
				{
					lResult = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
					mWriteLog(TraceType.EXCEPTION, "deleteBaselineDefinition", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
				}
				catch (CommunicationException ex)
				{
					mWriteLog(TraceType.EXCEPTION, "deleteBaselineDefinition", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
					lResult = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
				}
				catch (Exception ex)
				{
					mWriteLog(TraceType.EXCEPTION, "deleteBaselineDefinition", ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
					lResult = DataPackageErrorEnum.ERROR;
				}
			}
			else
			{
				lResult = DataPackageErrorEnum.INVALID_SESSION_ID;
			}

			return lResult;
		}

		/// <summary>Set a new assigned future baseline to an element.</summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pElementAddress">The address of the element(s) to which to assign(OBSOLETE).</param>
		/// <param name="pElementId">The ID of the element to which to assign.</param>
		/// <param name="pBLVersion">The baseline version to assign.</param>
		/// <param name="pActDate">The activation date for the baseline.</param>
		/// <param name="pExpDate">The expiration date.</param>
		/// <returns>A structure that contains the error code plus a request id.</returns>
		public DataPackageResult assignFutureBaselineToElement(
			Guid pSessionId,
			TargetAddressType pElementAddress,
			string pElementId,
			string pBLVersion,
			DateTime pActDate,
			DateTime pExpDate)
		{
			if (!string.IsNullOrEmpty(pElementId))
			{
				return assignFutureBaselineToElement(pSessionId, pElementId, pBLVersion, pActDate, pExpDate);
			}
			else
			{
				return assignFutureBaselineToElement(pSessionId, pElementAddress, pBLVersion, pActDate, pExpDate);
			}
		}



		/// <summary>Set a new assigned current baseline to an element.</summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pElementAddress">The address of the element to which to assign (OBSOLETE).</param>
		/// <param name="pElementId">The ID of the element to which to assign.</param>
		/// <param name="pBLVersion">The baseline version to assign.</param>
		/// <param name="pExpDate">The expiration date.</param>
		/// <returns>A structure that contains the error code plus a request id.</returns>
		public DataPackageResult assignCurrentBaselineToElement(Guid pSessionId, TargetAddressType pElementAddress, string pElementId, string pBLVersion, DateTime pExpDate)
		{
			if (!string.IsNullOrEmpty(pElementId))
			{
				return assignCurrentBaselineToElement(pSessionId, pElementId, pBLVersion, pExpDate);
			}
			else
			{
				return assignCurrentBaselineToElement(pSessionId, pElementAddress, pBLVersion, pExpDate);
			}
		}

		/// <summary>
		/// Distributes a targeted Baseline (specified by its version)
		/// </summary>
		/// <param name="pElementAddress"> The Address of the Element where to distriubte the baseline.</param>
		/// <param name="pBLVersion"> The targeted baseline version.</param>
		/// <param name="pActivationDate"> The date the new baseline is activated.</param>
		/// <param name="pExpirationDate"> The date the request expires.</param>
		/// <returns>A structure that contains the error code.</returns>
		public DataPackageErrorEnum distributeTargetedBaseline(string pElementAddress, string pBLVersion, DateTime pActivationDate, DateTime pExpirationDate)
		{
			string message = "distributeTargetedBaseline with parameters [ pElementAddress : "
								+ pElementAddress + "; pBLVersion  : "
								+ pBLVersion + "; pActivationDate  : "
								+ pActivationDate + "; pExpirationDate  : "
								+ pExpirationDate + " ]";

			mWriteLog(TraceType.INFO, "distributeTargetedBaseline", null, message);

			BaselineDistributionAttributes pBLAttributes = new BaselineDistributionAttributes()
			{
				fileCompression = false,
				priority = 1,
				transferDate = DateTime.Now,
				transferExpirationDate = DateTime.Now + (new TimeSpan(4, 0, 0)),
				TransferMode = FileTransferMode.AnyBandwidth
			};

			DataPackageErrorEnum lResult = checkIfElementExists(pElementAddress);
			if (lResult == DataPackageErrorEnum.REQUEST_ACCEPTED)
			{
				ServiceInfo serviceInfo;
				T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableServiceData(pElementAddress, (int)eServiceID.eSrvSIF_DataPackageServer, out serviceInfo);
				switch (lRqstResult)
				{
					case T2GManagerErrorEnum.eSuccess:
						{
							string endpoint = "http://" + serviceInfo.ServiceIPAddress + ":" + serviceInfo.ServicePortNumber;

							IRequestContext request = _requestFactory.CreateBaselineDistributingRequestContext(
							endpoint,
							pElementAddress,
							Guid.NewGuid(),
							Guid.Empty,
							pBLAttributes,
							false,
							pBLVersion,
							pActivationDate,
							pExpirationDate);

							if (request != null)
							{
								_requestManager.AddRequest(request);
							}
							else
							{
								lResult = DataPackageErrorEnum.ERROR;
							}
						}
						break;
					case T2GManagerErrorEnum.eT2GServerOffline:
						lResult = DataPackageErrorEnum.T2G_SERVER_OFFLINE;
						break;
					case T2GManagerErrorEnum.eElementNotFound:
						lResult = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
						break;
					case T2GManagerErrorEnum.eServiceInfoNotFound:
						lResult = DataPackageErrorEnum.SERVICE_INFO_NOT_FOUND;
						break;
					default:
                        lResult = DataPackageErrorEnum.ERROR;
						break;
				}
			}
			else if (lResult != DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE)
			{
				lResult = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
			}

			return lResult;
		}

		/// <summary>Distributes baselines assigned to element address.</summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pElementAddress">The element(s) address you want the baselines (OBSOLETE).</param>
		/// <param name="pTargetAddress">The element(s) address you want to be assigned.</param>
		/// <param name="pBLAttributes">The distribution attributes (TransferMode, fileCompression,
		/// transferDate, transferExpirationDate, priority).</param>
		/// <param name="pIncr">Specify if this update is incremental or not.</param>
		/// <returns>A structure that contains the error code plus a request id.</returns>
		public DataPackageResult distributeBaseline(Guid pSessionId, TargetAddressType pElementAddress, TargetAddressType pTargetAddress, BaselineDistributionAttributes pBLAttributes, bool pIncr)
		{
			if (pTargetAddress != null)
			{
				return distributeBaseline(pSessionId, pTargetAddress, pBLAttributes, pIncr);
			}
			else
			{
				return distributeBaseline(pSessionId, pElementAddress, pBLAttributes, pIncr);
			}
		}

		public DataPackageErrorEnum requestBaselineVesion(string pElementAddress, string pPisBaseVersion, string pPisMissionVersion, string pPisInfotainmentVersion, string pLmtVersion)
		{
			DataPackageErrorEnum lResult = DataPackageErrorEnum.REQUEST_ACCEPTED;

			try
			{
                using (IRemoteDataStoreClient lRemProx = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                {
                    // search a baseline in datastore with the packages versions provided
                    string lBaselineVersion = "";

                    DataContainer BLList = lRemProx.getBaselinesDefinitions();

                    List<BaselineDefinition> lBaselines = DataTypeConversion.fromDataContainerToBaselinesDefinitionsList(BLList);

                    foreach (BaselineDefinition lBaseline in lBaselines)
                    {
                        if (lBaseline.PISBaseDataPackageVersion == pPisBaseVersion &&
                            lBaseline.PISMissionDataPackageVersion == pPisMissionVersion &&
                            lBaseline.PISInfotainmentDataPackageVersion == pPisInfotainmentVersion &&
                            lBaseline.LMTDataPackageVersion == pLmtVersion)
                        {
                            lBaselineVersion = lBaseline.BaselineVersion;

                            break;
                        }
                    }

                    if (lBaselineVersion.Length > 0)
                    {
                        // if a baseline was found - call the function that will process SetBaselineVersionRequest
                        DataPackageResult lCommandResult = ProcessSetBaselineVersionCommand(pElementAddress, lBaselineVersion, pPisBaseVersion, pPisMissionVersion, pPisInfotainmentVersion, pLmtVersion);

                        lResult = lCommandResult.error_code;
                    }
                    else
                    {
                        // if there is no any baseline with packages version provided -
                        // save these packages version in UndefinedBaselines table in datastore and
                        // send the "Get Baseline Definition" notification to the GroundApp

                        // Save undefined baseline packages version in ElementDataStore table
                        lRemProx.setElementUndefinedBaselineParams(
                            pElementAddress,
                            pPisBaseVersion,
                            pPisMissionVersion,
                            pPisInfotainmentVersion,
                            pLmtVersion);

                        // Serialize the parameters to send in the notification.
                        List<string> lParamList = new List<string>(4);

                        lParamList.Add(pPisBaseVersion);
                        lParamList.Add(pPisMissionVersion);
                        lParamList.Add(pPisInfotainmentVersion);
                        lParamList.Add(pLmtVersion);

                        using (StringWriter lstr = new StringWriter())
                        {
                            _stringListXmlSerializer.Serialize(lstr, lParamList);
                            sendNotificationToGroundApp(Guid.Empty, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageBaselineDefinitionRequest, lstr.ToString());
                        }
                    }
                }
			}
			catch (TimeoutException ex)
			{
				mWriteLog(TraceType.EXCEPTION, "requestBaselineVesion", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
				lResult = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
			}
			catch (CommunicationException ex)
			{
				mWriteLog(TraceType.EXCEPTION, "requestBaselineVesion", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
				lResult = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
			}
			catch (Exception ex)
			{
				mWriteLog(TraceType.EXCEPTION, "requestBaselineVesion", ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
				lResult = DataPackageErrorEnum.ERROR;
			}

			return lResult;
		}

		public DataPackageResult GetAvailableElementDataList(Guid pSessionId, out ElementList<AvailableElementData> pElementDataList)
		{
			DataPackageResult result = new DataPackageResult();
			result.error_code = DataPackageErrorEnum.ERROR;
			pElementDataList = new ElementList<AvailableElementData>();

            if (_sessionManager.IsSessionValid(pSessionId))
            {
				T2GManagerErrorEnum lResult = _t2gManager.GetAvailableElementDataList(out pElementDataList);

				if (lResult == T2GManagerErrorEnum.eSuccess)
				{
					result.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;
				}
				else if (lResult == T2GManagerErrorEnum.eT2GServerOffline)
				{
					result.error_code = DataPackageErrorEnum.T2G_SERVER_OFFLINE;
				}
				else
				{
					result.error_code = DataPackageErrorEnum.ERROR;
				}
			}
			else
			{
				result.error_code = DataPackageErrorEnum.INVALID_SESSION_ID;
			}

			return result;
		}

		/// <summary>
		/// Unassign future baseline from an element.
		/// </summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pElementId">The element Id you want to be assigned.</param>
		/// <returns>Response of the unassignFutureBaselineFromElement</returns>
		public DataPackageErrorEnum unassignFutureBaselineFromElement(Guid pSessionId, string pElementId)
		{
			DataPackageErrorEnum lResult = DataPackageErrorEnum.REQUEST_ACCEPTED;

			if (_sessionManager.IsSessionValid(pSessionId))
			{
				lResult = checkIfElementExists(pElementId);

				if (lResult == DataPackageErrorEnum.REQUEST_ACCEPTED)
				{
					try
					{
                        using (IRemoteDataStoreClient lRemProx = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                        {
                            lRemProx.unassignFutureBaselineFromElement(pElementId);
                            lResult = DataPackageErrorEnum.REQUEST_ACCEPTED;

                            lock (_distributingElementDic)
                            {
                                if (_distributingElementDic.ContainsKey(pElementId))
                                {
                                    _distributingElementDic.Remove(pElementId);
                                }
                            }
                        }
					}
					catch (TimeoutException ex)
					{
						lResult = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
						mWriteLog(TraceType.EXCEPTION, "unassignFutureBaselineFromElement", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
					}
					catch (CommunicationException ex)
					{
						lResult = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
						mWriteLog(TraceType.EXCEPTION, "unassignFutureBaselineFromElement", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
					}
					catch (Exception ex)
					{
						mWriteLog(TraceType.EXCEPTION, "unassignFutureBaselineFromElement", ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
						lResult = DataPackageErrorEnum.ERROR;
					}
				}
				else if (lResult != DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE)
				{
					lResult = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
				}
			}
			else
			{
				lResult = DataPackageErrorEnum.INVALID_SESSION_ID;
			}

			return lResult;
		}

		/// <summary>
		/// Unassign current baseline from an element.
		/// </summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pElementId">The element Id you want to be assigned.</param>
		/// <returns>Response of the unassignCurrentBaselineFromElement</returns>
		public DataPackageErrorEnum unassignCurrentBaselineFromElement(Guid pSessionId, string pElementId)
		{
			DataPackageErrorEnum lResult = DataPackageErrorEnum.REQUEST_ACCEPTED;

			if (_sessionManager.IsSessionValid(pSessionId))
			{
				lResult = checkIfElementExists(pElementId);

				if (lResult == DataPackageErrorEnum.REQUEST_ACCEPTED)
				{
					try
					{
                        using (IRemoteDataStoreClient lRemProx = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                        {
                            lRemProx.unassignCurrentBaselineFromElement(pElementId);
                            lResult = DataPackageErrorEnum.REQUEST_ACCEPTED;
                        }
					}
					catch (TimeoutException ex)
					{
						lResult = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
						mWriteLog(TraceType.EXCEPTION, "unassignCurrentBaselineFromElement", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
					}
					catch (CommunicationException ex)
					{
						lResult = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
						mWriteLog(TraceType.EXCEPTION, "unassignCurrentBaselineFromElement", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
					}
					catch (Exception ex)
					{
						mWriteLog(TraceType.EXCEPTION, "unassignCurrentBaselineFromElement", ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
						lResult = DataPackageErrorEnum.ERROR;
					}
				}
				else if (lResult != DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE)
				{
					lResult = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
				}
			}
			else
			{
				lResult = DataPackageErrorEnum.INVALID_SESSION_ID;
			}

			return lResult;
		}

		/// <summary>		
		/// Returns the list of packages versions already defined in the RemoteDataStore (in the data base)
		/// </summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pPackageType">Type of the package.</param>
		/// <param name="pVersionsList">[out]List of packages versions.</param>
		/// <returns>Response of the getDataPackagesVersionsList.</returns>
		public DataPackageErrorEnum getDataPackagesVersionsList(Guid pSessionId, DataPackageType pPackageType, out DataPackagesVersionsList pVersionsList)
		{
			DataPackageErrorEnum lResult = DataPackageErrorEnum.REQUEST_ACCEPTED;

			pVersionsList = new DataPackagesVersionsList();

			if (!_sessionManager.IsSessionValid(pSessionId))
			{
				lResult = DataPackageErrorEnum.INVALID_SESSION_ID;
			}
			else
			{
				try
				{
                    using (IRemoteDataStoreClient lRemDSProxy = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                    {
                        DataContainer lDPList = lRemDSProxy.getDataPackagesList();

                        List<DataPackagesCharacteristics> lDPCList = DataTypeConversion.fromDataContainerToDataPackagesCharacteristicsList(lDPList);

                        foreach (DataPackagesCharacteristics lDPChar in lDPCList)
                        {
                            if (lDPChar.DataPackageType == pPackageType)
                            {
                                pVersionsList.VersionsList.Add(lDPChar.DataPackageVersion);
                            }
                        }

                        pVersionsList.DataPackageType = pPackageType;

                        lResult = DataPackageErrorEnum.REQUEST_ACCEPTED;
                    }
				}
				catch (TimeoutException ex)
				{
					mWriteLog(TraceType.EXCEPTION, "getDataPackagesVersionsList", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
					lResult = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
				}
				catch (CommunicationException ex)
				{
					mWriteLog(TraceType.EXCEPTION, "getDataPackagesVersionsList", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
					lResult = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;
				}
				catch (Exception ex)
				{
					mWriteLog(TraceType.EXCEPTION, "getDataPackagesVersionsList", ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
					lResult = DataPackageErrorEnum.ERROR;
				}
			}
			return lResult;
		}

		/// <summary>Delete a data package.</summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pPackageType">The package type.</param>
		/// <param name="pPackageVersion">The package version.</param>
		/// <param name="pForceDeleting">Force deleting an assigned package.</param>
		/// <returns>Response of the deleteDataPackage.</returns>
		public DataPackageResult deleteDataPackage(Guid pSessionId, DataPackageType pPackageType, string pPackageVersion, bool pForceDeleting)
		{
			DataPackageResult lResult = new DataPackageResult();
			lResult.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;

			string message = "deleteDataPackage with parameters [ pSessionId : "
								+ pSessionId.ToString() + "; pPackageType  : "
								+ pPackageType.ToString() + "; pPackageVersion  : "
								+ pPackageVersion + "; pForceDeleting  : "
								+ pForceDeleting.ToString() + " ]";

			mWriteLog(TraceType.INFO, "deleteDataPackage", null, message);

			if (DataPackageService.mIsVersionValid(pPackageVersion, false) == false)
			{
				mWriteLog(TraceType.ERROR, "deleteDataPackage", null, Logs.ERROR_INVALID_DATAPACKAGE_VERSION, pPackageVersion);

				lResult.error_code = DataPackageErrorEnum.DATA_PACKAGE_INVALID_VERSION;
			}
			else if (!_sessionManager.IsSessionValid(pSessionId))
			{
				mWriteLog(TraceType.ERROR, "deleteDataPackage", null, Logs.ERROR_INVALID_SESSION_ID, pSessionId);

				lResult.error_code = DataPackageErrorEnum.INVALID_SESSION_ID;
			}
			else
			{
				Guid lrequestId;
				string lGetResult = _sessionManager.GenerateRequestID(pSessionId, out lrequestId);
				lResult.reqId = lrequestId;

				if (lrequestId != Guid.Empty)
				{
					if (mIsPackageUsed(pPackageType, pPackageVersion))
					{
						mWriteLog(TraceType.ERROR, "deleteDataPackage", null, Logs.ERROR_DATAPACKAGE_IS_USED, pPackageType.ToString(), pPackageVersion);

						lResult.error_code = DataPackageErrorEnum.DATA_PACKAGE_IS_USED;
					}
					else
					{
						try
						{
                            using (IRemoteDataStoreClient lRemDSProxy = _remoteDataStoreFactory.GetRemoteDataStoreInstance())
                            {
                                if (lRemDSProxy.checkIfDataPackageExists(pPackageType.ToString(), pPackageVersion) == false)
                                {
                                    lResult.error_code = DataPackageErrorEnum.DATA_PACKAGE_NOT_FOUND;
                                    mWriteLog(TraceType.ERROR, "deleteDataPackage", null, Logs.ERROR_DATAPACKAGE_ISNT_FOUND, pPackageType.ToString(), pPackageVersion);
                                }
                                else
                                {
                                    //Check if data package is used by an assigned baseline
                                    bool lAssigned = false;
                                    DataContainer lEList = lRemDSProxy.getAssignedBaselinesVersions();
                                    List<ElementDescription> lElements = DataTypeConversion.fromDataContainerToElementDescriptionList(lEList);

                                    foreach (ElementDescription lElem in lElements)
                                    {
                                        if (!string.IsNullOrEmpty(lElem.AssignedCurrentBaseline) &&
                                            lRemDSProxy.checkIfBaselineExists(lElem.AssignedCurrentBaseline))
                                        {
                                            BaselineDefinition lBLDef = DataTypeConversion.fromDataContainerToBaselineDefinition(lRemDSProxy.getBaselineDefinition(lElem.AssignedCurrentBaseline));

                                            switch (pPackageType)
                                            {
                                                case DataPackageType.PISBASE:
                                                    lAssigned = (lBLDef.PISBaseDataPackageVersion == pPackageVersion);
                                                    break;

                                                case DataPackageType.PISMISSION:
                                                    lAssigned = (lBLDef.PISMissionDataPackageVersion == pPackageVersion);
                                                    break;

                                                case DataPackageType.PISINFOTAINMENT:
                                                    lAssigned = (lBLDef.PISInfotainmentDataPackageVersion == pPackageVersion);
                                                    break;

                                                case DataPackageType.LMT:
                                                    lAssigned = (lBLDef.LMTDataPackageVersion == pPackageVersion);
                                                    break;
                                            }

                                            if (lAssigned)
                                            {
                                                break;
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(lElem.AssignedFutureBaseline) &&
                                            lRemDSProxy.checkIfBaselineExists(lElem.AssignedFutureBaseline))
                                        {
                                            BaselineDefinition lBLDef = DataTypeConversion.fromDataContainerToBaselineDefinition(lRemDSProxy.getBaselineDefinition(lElem.AssignedFutureBaseline));

                                            switch (pPackageType)
                                            {
                                                case DataPackageType.PISBASE:
                                                    lAssigned = (lBLDef.PISBaseDataPackageVersion == pPackageVersion);
                                                    break;

                                                case DataPackageType.PISMISSION:
                                                    lAssigned = (lBLDef.PISMissionDataPackageVersion == pPackageVersion);
                                                    break;

                                                case DataPackageType.PISINFOTAINMENT:
                                                    lAssigned = (lBLDef.PISInfotainmentDataPackageVersion == pPackageVersion);
                                                    break;

                                                case DataPackageType.LMT:
                                                    lAssigned = (lBLDef.LMTDataPackageVersion == pPackageVersion);
                                                    break;
                                            }

                                            if (lAssigned)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    //If data package isn't used by an assigned baseline or
                                    //deleting is forced - delete data package from Remote Data Store
                                    if (lAssigned && pForceDeleting ||
                                        !lAssigned)
                                    {
                                        lRemDSProxy.deleteDataPackage(lrequestId, pPackageType.ToString(), pPackageVersion);

                                        //If data package is used by an assigned baseline -
                                        //send a notification to the Ground App
                                        if (lAssigned)
                                        {
                                            // Serialize the parameters to send in the notification.
                                            List<string> lParamList = new List<string>();

                                            lParamList.Add(pPackageType.ToString());
                                            lParamList.Add(pPackageVersion);

                                            using (StringWriter lstr = new StringWriter())
                                            {
                                                _stringListXmlSerializer.Serialize(lstr, lParamList);
                                                sendNotificationToGroundApp(lrequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageAssignedPackageWasDeleted, lstr.ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        lResult.error_code = DataPackageErrorEnum.DATA_PACKAGE_IS_ASSIGNED;

                                        mWriteLog(TraceType.ERROR, "deleteDataPackage", null, Logs.ERROR_DATAPACKAGE_IS_ASSIGNED, pPackageType.ToString(), pPackageVersion);
                                    }
                                }
                            }
						}
						catch (TimeoutException ex)
						{
							mWriteLog(TraceType.EXCEPTION, "deleteDataPackage", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
							lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;

							return lResult;
						}
						catch (CommunicationException ex)
						{
							mWriteLog(TraceType.EXCEPTION, "deleteDataPackage", ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE);
							lResult.error_code = DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE;

							return lResult;
						}
						catch (Exception ex)
						{
							mWriteLog(TraceType.EXCEPTION, "deleteDataPackage", ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
							lResult.error_code = DataPackageErrorEnum.ERROR;

							return lResult;
						}
					}
				}
				else
				{
					lResult.error_code = DataPackageErrorEnum.ERROR;
				}
			}

			return lResult;
		}

		/// <summary>Force an Element On train to use a future baseline.</summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pElementAddress">The element(s) address you want the baselines (OBSOLETE).</param>
		/// <param name="pElementId">The Id of the Element where the request is to be sent.</param>
		/// <param name="pReqTimeout">The permitted timeout to execute the request.</param>
		/// <returns>The operation result</returns>
		public DataPackageResult forceAddresseesFutureBaseline(Guid pSessionId, TargetAddressType pElementAddress, string pElementId, uint pReqTimeout)
		{
			if (!string.IsNullOrEmpty(pElementId))
			{
				return forceAddresseesFutureBaseline(pSessionId, pElementId, pReqTimeout);
			}
			else
			{
				return ProcessAddresseesBaselineCommand(pSessionId, pElementAddress, pReqTimeout, BaselineCommandType.FORCE_FUTURE);
			}
		}

		/// <summary>Force an Element On train to use an archived baseline.</summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pElementAddress">The element(s) address you want the baselines (OBSOLETE).</param>
		/// <param name="pElementId">The Id of the Element where the request is to be sent.</param>
		/// <param name="pReqTimeout">The permitted timeout to execute the request.</param>
		/// <returns>.</returns>
		public DataPackageResult forceAddresseesArchivedBaseline(Guid pSessionId, TargetAddressType pElementAddress, string pElementId, uint pReqTimeout)
		{
			if (!string.IsNullOrEmpty(pElementId))
			{
				return forceAddresseesFutureBaseline(pSessionId, pElementId, pReqTimeout);
			}
			else
			{
				return ProcessAddresseesBaselineCommand(pSessionId, pElementAddress, pReqTimeout, BaselineCommandType.FORCE_ARCHIVED);
			}
		}

		/// <summary>Clears a forcing status of  an Element On train.</summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pElementAddress">The address of the element(s) where the request is to be sent(OBSOLETE).</param>
		/// <param name="pElementId">The Id of the Element where the request is to be sent.</param>
		/// <param name="pReqTimeout">The permitted timeout to execute the request.</param>
		/// <returns>.</returns>
		public DataPackageResult clearAddreeseesForcingStatus(Guid pSessionId, TargetAddressType pElementAddress, string pElementId, uint pReqTimeout)
		{
			if (!string.IsNullOrEmpty(pElementId))
			{
				return clearAddreeseesForcingStatus(pSessionId, pElementId, pReqTimeout);
			}
			else
			{
				return ProcessAddresseesBaselineCommand(pSessionId, pElementAddress, pReqTimeout, BaselineCommandType.CLEAR_FORCING);
			}
		}

		/// <summary>Removes the baseline.</summary>
		/// <param name="pSessionId">The Id of the session in which the request is executed.</param>
		/// <param name="pVersion">The version of the baseline to remove</param>
		/// <returns>The operation result</returns>
		public DataPackageResult removeBaseline(Guid pSessionId, string pVersion)
		{
			DataPackageResult pResult = new DataPackageResult();

			pResult.error_code = deleteBaselineDefinition(pSessionId, pVersion);

			return pResult;
		}

		/// <summary>Process the set baseline version command.</summary>
		/// <param name="pElementId">The element Id you want to be assigned.</param>
		/// <param name="pBaselineVersion">The baseline version.</param>
		/// <param name="pPisBaseVersion">The pis base version.</param>
		/// <param name="pPisMissionVersion">The pis mission version.</param>
		/// <param name="pPisInfotainmentVersion">The pis infotainment version.</param>
		/// <param name="pLmtVersion">The lmt version.</param>
		/// <returns>Request result.</returns>
		private DataPackageResult ProcessSetBaselineVersionCommand(string pElementId, string pBaselineVersion, string pPisBaseVersion, string pPisMissionVersion, string pPisInfotainmentVersion, string pLmtVersion)
		{
			DataPackageResult result = new DataPackageResult();

			Guid lRequestId = Guid.NewGuid();  // Put a random ReqID, it is not important, it is only used as getUrlsForDistributeBaselines parameter.

			ServiceInfo serviceInfo;
			string endpoint = string.Empty;
			T2GManagerErrorEnum lRqstResult = _t2gManager.GetAvailableServiceData(pElementId, (int)eServiceID.eSrvSIF_DataPackageServer, out serviceInfo);
			switch (lRqstResult)
			{
				case T2GManagerErrorEnum.eSuccess:
					endpoint = "http://" + serviceInfo.ServiceIPAddress + ":" + serviceInfo.ServicePortNumber;
					break;
				case T2GManagerErrorEnum.eT2GServerOffline:
					result.error_code = DataPackageErrorEnum.T2G_SERVER_OFFLINE;
					break;
				case T2GManagerErrorEnum.eElementNotFound:
					result.error_code = DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND;
					break;
				case T2GManagerErrorEnum.eServiceInfoNotFound:
					result.error_code = DataPackageErrorEnum.SERVICE_INFO_NOT_FOUND;
					break;
				default:
                    result.error_code = DataPackageErrorEnum.ERROR;
                    break;
			}

			IRequestContext request = _requestFactory.CreateBaselineSettingRequestContext(
				endpoint,
				pElementId,
				lRequestId,
				Guid.Empty,
				RequestConstants.Timeout,
				pBaselineVersion,
				pPisBaseVersion,
				pPisMissionVersion,
				pPisInfotainmentVersion,
				pLmtVersion);


			// Enqueue the request in the requests list, so they will be executed in the OnTransmitEvent thread.
			if (request != null)
			{
				lock (_lock)
				{
					_requestManager.AddRequest(request);
				}

				result.error_code = DataPackageErrorEnum.REQUEST_ACCEPTED;
				result.reqId = lRequestId;
			}
			else
			{
				result.error_code = DataPackageErrorEnum.ERROR;
				result.reqId = Guid.Empty;
			}

			return result;
		}

		#endregion

		#region internal classes

		/// <summary>Information about a cancellable transfer task.</summary>
		/// <note>Internal since used by the unit tests</note>
		internal class CancellableTransferTaskInfo
		{
			/// <summary>Initializes a new instance of the CancellableTransferTaskInfo class.</summary>
			/// <param name="taskID">Identifier for the task.</param>
			/// <param name="elements">List of recipient elements for that task.</param>			
			public CancellableTransferTaskInfo(int taskID, List<string> elements)
			{
				_taskID = taskID;
				_Elements = elements;
			}

			/// <summary>Gets or sets the identifier of the task.</summary>
			/// <value>The identifier of the task.</value>
			public int TaskID
			{
				get { return _taskID; }
				set { _taskID = value; }
			}

			/// <summary>Gets or sets the list of recipient elements.</summary>
			/// <value>The elements list.</value>
			public List<string> Elements
			{
				get { return _Elements; }
				set { _Elements = value; }
			}

			/// <summary>Convert this object into a string representation.</summary>
			/// <returns>This object as a string.</returns>
			public override string ToString()
			{
				string lString = "TaskID : " + _taskID + " Element(s) : ";
				if (_Elements != null)
				{
					lString += string.Join(",", _Elements.ToArray());
				}
				return lString;
			}

			/// <summary>Query if the transfer is matching the specified template task.</summary>
			/// <param name="templateTask">The template task.</param>
			/// <returns>true if matching, false if not.</returns>
			public bool IsMatching(CancellableTransferTaskInfo templateTask)
			{
				bool lMatching = false;

				if (_Elements != null && templateTask != null && templateTask._Elements != null)
				{
					if (_Elements.Count > 0 && templateTask._Elements.Count > 0)
					{
						lMatching = true;

						foreach (string lElement in _Elements)
						{
							// All elements of the current task must be present in the template task
							if (templateTask._Elements.Exists((lParameter) => lParameter == lElement) == false)
							{
								lMatching = false;
								break;
							}
						}
					}
				}

				return lMatching;
			}

			/// <summary>Identifier for the task.</summary>
			private int _taskID;

			/// <summary>The elements list for that transfer task.</summary>
			private List<string> _Elements;
		}

		#endregion

	}

	#region internal classes

	/// <summary>
	///	Event throwed when a request is completed.
	/// </summary>
	internal class RequestCompletedEvent
	{
		/// <summary>
		/// Initializes a new instance of the RequestCompletedEvent class.
		/// </summary>
		/// <param name="elementId"></param>
		/// <param name="requestId"></param>
		public RequestCompletedEvent(string elementId, Guid requestId)
		{
			this.elementId = elementId;
			this.requestId = requestId;
		}

		public string elementId;
		public Guid requestId;
	}

	internal class DistributingElementData
	{
		public Guid requestId;
		public PIS.Ground.GroundCore.AppGround.NotificationIdEnum distributionStatus;
	}

	/// <summary>
	///	Parameters used for a package (PisBase, PisMission,...).
	/// </summary>
	internal class PackageParams
	{
		/// <summary>
		/// Initializes a new instance of the PackageParams class.
		/// </summary>
		/// <param name="pType"></param>
		/// <param name="pVersion"></param>
		public PackageParams(DataPackageType pType, string pVersion)
		{
			aType = pType;
			aVersion = pVersion;
		}

		public DataPackageType aType { get; set; }
		public string aVersion { get; set; }
	}

	#endregion
}