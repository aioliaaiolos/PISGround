//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineStatusUpdater.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.Data;
using PIS.Ground.DataPackage.Notification;
using PIS.Ground.RemoteDataStore;
using PIS.Ground.Core.T2G;
using System.Globalization;

namespace PIS.Ground.DataPackage
{
	/// <summary>Class providing baseline status update capabilities.</summary>
	public class BaselineStatusUpdater : IDisposable
	{
		/// <summary>Exception for signalling history logger not installed errors.</summary>
		public class HistoryLoggerNotInstalledException : Exception
		{
			/// <summary>
			/// Initializes a new instance of the HistoryLoggerNotInstalledException class.
			/// </summary>
			public HistoryLoggerNotInstalledException()
				: base()
			{
			}

			/// <summary>
			/// Initializes a new instance of the HistoryLoggerNotInstalledException class.
			/// </summary>
			/// <param name="message">The message.</param>
			public HistoryLoggerNotInstalledException(string message)
				: base(message)
			{
			}
		}

		/// <summary>Exception for signalling history logger failure errors.</summary>
		public class HistoryLoggerFailureException : Exception
		{
			/// <summary>Initializes a new instance of the HistoryLoggerFailureException class.</summary>
			/// <param name="message">The message.</param>
			public HistoryLoggerFailureException(string message)
				: base(message)
			{
			}
		}

		/// <summary>Function to be called when the baseline progress for a particular train must be created or updated
		/// 		 in a persistent storage.</summary>
		/// <param name="parameter1">The corresponding train.</param>
		/// <param name="parameter2">The progress information.</param>
		/// <returns>true if successful, false otherwise.</returns>
		public delegate bool BaselineProgressUpdateProcedure(string trainId, TrainBaselineStatusData progressInfo);

		/// <summary>Function to be called when the baseline progress for a particular train must be removed
		/// 		 from the persistent storage.</summary>
		/// <param name="parameter1">The corresponding train.</param>
		/// <returns>true if successful, false otherwise.</returns>
		public delegate bool BaselineProgressRemoveProcedure(string trainId);

		/// <summary>The version string used when there is no baseline.</summary>
		public const string NoBaselineVersion = "0.0.0.0";

		/// <summary>The version string used when a version is not known.</summary>
		public const string UnknownVersion = "UNKNOWN";

		/// <summary>true if this updated class is initialized.</summary>
		private bool _isInitialized;

		/// <summary>true if the persistent storage is installed, false otherwise.</summary>
		private bool _isHistoryLoggerAvailable = true;

		/// <summary>The client providing access to the T2G server.</summary>
		private IT2GFileDistributionManager _t2g;

        /// <summary>The ILogManager interface reference.</summary>
        private ILogManager _logManager = new LogManager();

		/// <summary>List of baseline progresses loaded from persistent storage and extended with additional information.</summary>
		private Dictionary<string, TrainBaselineStatusExtendedData> _baselineProgresses;

		/// <summary>The baseline progress update procedure in persistent storage.</summary>
		private BaselineProgressUpdateProcedure _baselineProgressUpdateProcedure;

		/// <summary>The baseline progress remove procedure from persistent storage.</summary>
		private BaselineProgressRemoveProcedure _baselineProgressRemoveProcedure;

		/// <summary>The multithreading protection lock.</summary>
		private object _baselineStatusUpdaterLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaselineStatusUpdater"/> class.
        /// </summary>
		public BaselineStatusUpdater()
		{
            // No logic body
		}

		/// <summary>Initializes BaselineStatusUpdater. Must be called before any other public methods.</summary>
		/// <param name="t2g">The client providing access to the T2G server. Must not be null.</param>
        /// <param name="currentSystems">Optional list of available elements. Key is the train id and Value is the system information.</param>
        /// <returns>true if it succeeds, false otherwise.</returns>		
		public bool Initialize(IT2GFileDistributionManager t2g,
            IDictionary<string, SystemInfo> currentSystems)
		{
			_logManager.WriteLog(TraceType.INFO,
				"Initialize()",
				"PIS.Ground.DataPackage.BaselineStatusUpdater.Initialize",
				null,
				EventIdEnum.DataPackage);

			bool lSuccess = false;

			try
			{
				lock (_baselineStatusUpdaterLock)
				{
					if (_isInitialized)
					{
						throw new InvalidOperationException("already initialized");
					}

					if (t2g == null)
					{
						throw new ArgumentNullException("t2g");
					}

					// Get latest status from persistent storage
					// 
					Dictionary<string, TrainBaselineStatusData> lDictionaryResponse = null;

					ResultCodeEnum resultCode = _logManager.GetTrainBaselineStatus(out lDictionaryResponse);

					if (resultCode != ResultCodeEnum.RequestAccepted)
					{
                        string message = string.Format(CultureInfo.CurrentCulture, "_logManager.GetTrainBaselineStatus returned {0}", resultCode);
						throw new HistoryLoggerFailureException(message);
					}

					if (lDictionaryResponse == null)
					{
						_isHistoryLoggerAvailable = false;
						throw (new HistoryLoggerNotInstalledException("_logManager.GetTrainBaselineStatus returned null: Is the HistoryLogger installed?"));
					}

					// Convert status to extended flavor 
					// for additional information like assigned current and future baselines once known
					// 
					var lBaselineProgresses = new Dictionary<string, TrainBaselineStatusExtendedData>();

					foreach (KeyValuePair<string, TrainBaselineStatusData> lEntry in lDictionaryResponse)
					{
						string lTrainID = lEntry.Key;
						TrainBaselineStatusData lProgress = lEntry.Value;

						if (!string.IsNullOrEmpty(lTrainID) && lProgress != null)
						{
							var lExtendedProgress = new TrainBaselineStatusExtendedData(lProgress);

							switch (lProgress.ProgressStatus)
							{
								case BaselineProgressStatusEnum.UNKNOWN:
								case BaselineProgressStatusEnum.DEPLOYED:
								case BaselineProgressStatusEnum.UPDATED:
									lExtendedProgress.AssignedFutureBaseline = UnknownVersion;
									lExtendedProgress.OnBoardFutureBaseline = lProgress.FutureBaselineVersion;
									break;

								default:
									lExtendedProgress.AssignedFutureBaseline = lProgress.FutureBaselineVersion;
									lExtendedProgress.OnBoardFutureBaseline = UnknownVersion;
									break;
							}
							lBaselineProgresses[lTrainID] = lExtendedProgress;
						}
					}

					// Call the lower level initialization procedure
					// 
					lSuccess = Initialize(lBaselineProgresses,
						new BaselineProgressUpdateProcedure(UpdateProgressOnHistoryLogger),
						new BaselineProgressRemoveProcedure(RemoveProgressFromHistoryLogger),
						t2g,
                        _logManager,
                        currentSystems);
				}
			}
			catch (Exception lException)
			{
				_logManager.WriteLog(TraceType.WARNING, "Failed to initialize BaselineStatusUpdater",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.Initialize",
					lException, EventIdEnum.DataPackage);
			}

			return lSuccess;
		}

    	/// <summary>Initializes BaselineStatusUpdater. Use this method directly if performing unit tests.
		/// 		 Otherwise, use the public overload(s).</summary>
		/// <param name="baselineProgresses">List of baseline progresses retrieved from persistent storage.</param>
		/// <param name="baselineProgressUpdateProcedure">The baseline progress update procedure to persistent storage.</param>
		/// <param name="baselineProgressRemoveProcedure">The baseline progress remove procedure from persistent storage.</param>///
		/// <param name="t2g">The client providing access to the T2G server.</param>
        /// <param name="logManager">The ILogManager interface reference. Added for unit test purposes.</param>
        /// <param name="currentSystems">Optional list of available elements. Key is the train id and Value is the system information.</param>
		/// <returns>true if it succeeds, false otherwise.</returns>	
		protected bool Initialize(
			Dictionary<string, TrainBaselineStatusExtendedData> baselineProgresses,
			BaselineProgressUpdateProcedure baselineProgressUpdateProcedure,
			BaselineProgressRemoveProcedure baselineProgressRemoveProcedure,
			IT2GFileDistributionManager t2g,
            ILogManager logManager,
            IDictionary<string, SystemInfo> currentSystems)
		{
			// Save the method arguments to class fields
			// 
			_baselineProgresses = baselineProgresses;
			_baselineProgressUpdateProcedure = baselineProgressUpdateProcedure;
			_baselineProgressRemoveProcedure = baselineProgressRemoveProcedure;
			_t2g = t2g;
            _logManager = logManager;

			// Ok. From now one, other methods of this class may be called
			// 
			_isInitialized = true;

			// For now, assume all elements are off-line and that we do not know
			// the different baselines and software versions and other information that could have changed
			// 
            ResetStatusEntries(currentSystems);

			return _isInitialized;
		}

        /// <summary>
        /// Uninitializes this instance.
        /// </summary>
        private void Uninitialize()
        {
            lock (_baselineStatusUpdaterLock)
            {
                _isInitialized = false;
                _isHistoryLoggerAvailable = true;
                _t2g = null;
                if (_baselineProgresses != null)
                {
                    _baselineProgresses.Clear();
                }
                _baselineProgresses = null;
                _baselineProgressUpdateProcedure = null;
                _baselineProgressRemoveProcedure = null;
            }
        }


		/// <summary>Resets all status entries by clearing the fields that might have changed.</summary>
        /// <param name="currentSystems">Information on known system as stored in local data store. Key is the train name, value the system information</param>
		public void ResetStatusEntries(IDictionary<string, SystemInfo> currentSystems)
		{
			try
			{
				// For now, assume all elements are off-line and that we do not know
				// the different baselines and software versions and other information that could have changed
				// 
				lock (_baselineStatusUpdaterLock)
				{
					if (_isInitialized)
					{
						var lProgresses = new Dictionary<string, TrainBaselineStatusExtendedData>(_baselineProgresses);

						foreach (KeyValuePair<string, TrainBaselineStatusExtendedData> lEntry in lProgresses)
						{
							string lTrainId = lEntry.Key;
							TrainBaselineStatusExtendedData lOriginalStatus = lEntry.Value;

							if (string.IsNullOrEmpty(lTrainId) == false && lOriginalStatus != null)
							{
								TrainBaselineStatusExtendedData lExtendedStatus = lOriginalStatus.Clone();
								TrainBaselineStatusData lStatus = lExtendedStatus.Status;

								if (lStatus != null)
								{
									// Make sure that all trains will be fully investigated the next time we hear from them
									lExtendedStatus.IsT2GPollingRequired = true;

                                    bool deleted = false;
                                    SystemInfo currentSystem;
                                    if (currentSystems != null)
                                    {
                                        if (!currentSystems.TryGetValue(lTrainId, out currentSystem) || currentSystem == null)
                                        {
                                            // System deleted
                                            DeleteSystem(lTrainId);
                                            deleted = true;
                                        }
                                        else
                                        {
                                            lExtendedStatus.Update(currentSystem);
                                        }
                                    }
                                    else 
                                    {
                                        lStatus.OnlineStatus = false;
                                    }

                                    if (!deleted && !TrainBaselineStatusExtendedData.AreEqual(lExtendedStatus, lOriginalStatus))
                                    {
                                        LogProgress(lTrainId, lExtendedStatus);
                                    }
								}
							}
						}

                        // Add newly detected system
                        if (currentSystems != null && currentSystems.Count != _baselineProgresses.Count)
                        {
                            foreach (KeyValuePair<string, SystemInfo> item in currentSystems)
                            {
                                if (!_baselineProgresses.ContainsKey(item.Key))
                                {
                                    TrainBaselineStatusData newData = new TrainBaselineStatusData(item.Key, item.Value.VehiclePhysicalId, item.Value.IsOnline, NoBaselineVersion);
                                    TrainBaselineStatusExtendedData newSystem = new TrainBaselineStatusExtendedData(newData);
                                    newSystem.Update(item.Value);
                                    LogProgress(item.Key, newSystem);
                                }
                            }
                        }
					}
					else
					{
						if (_isHistoryLoggerAvailable)
						{
							throw new InvalidOperationException("BaselineStatusUpdater is not initialized");
						}
					}
				}
			}
			catch (Exception lException)
			{
				_logManager.WriteLog(TraceType.ERROR, "Failed to reset status entries",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ResetStatusEntries",
					lException, EventIdEnum.DataPackage);
			}
		}

		/// <summary>Try to get the progress entry corresponding to the specifed train.</summary>
		/// <param name="trainId">Identifier of the train.</param>
		/// <param name="lStatus">[out] The status.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public bool TryGetEntry(string trainId, out TrainBaselineStatusExtendedData lStatus)
		{
			bool lSuccess = false;
			lStatus = null;

			TrainBaselineStatusExtendedData lOriginalStatus;

			if (_baselineProgresses.TryGetValue(trainId, out lOriginalStatus))
			{
				lStatus = lOriginalStatus.Clone();
				lSuccess = true;
			}
			return lSuccess;
		}

		/// <summary>Logs the progress to volatile and persistent storage.</summary>
		/// <exception cref="HistoryLoggerFailureException">Thrown when a History Logger Failure error occurs.</exception>
		/// <param name="trainId">Train ID.</param>
		/// <param name="lNewStatus">The new status to be stored.</param>		
		private void LogProgress(string trainId, TrainBaselineStatusExtendedData lNewStatus)
		{
			if (!string.IsNullOrEmpty(trainId) && lNewStatus != null && lNewStatus.Status != null)
			{
				TrainBaselineStatusExtendedData lCurrentStatus = null;

				_baselineProgresses.TryGetValue(trainId, out lCurrentStatus);

				if (lCurrentStatus == null || !TrainBaselineStatusExtendedData.AreEqual(lNewStatus, lCurrentStatus))
				{
					if (lCurrentStatus == null || lCurrentStatus.Status == null ||
						!TrainBaselineStatusData.AreEqual(lNewStatus.Status, lCurrentStatus.Status))
					{
						// Write updated status to persistent storage
						// 
						if (!_baselineProgressUpdateProcedure(trainId, lNewStatus.Status))
						{
							throw new HistoryLoggerFailureException("LogProgress() : cannot update HistoryLog entry for train: " + (trainId??string.Empty));
						}
					}

					_baselineProgresses[trainId] = lNewStatus;
				}
			}
		}

		/// <summary>Process the distribute baseline request.</summary>
		/// <param name="trainId">Train ID.</param>
		/// <param name="requestId">Identifier for the new request.</param>
		/// <param name="onLine">true if the targetted train is on line, false otherwise.</param>
		/// <param name="currentBaseline">The current baseline.</param>
		/// <param name="futureBaseline">The future baseline.</param>
		/// <param name="assignedFutureBaseline">The assigned future baseline (the one we want to push on the train).</param>
		public void ProcessDistributeBaselineRequest(
			string trainId,
			Guid requestId,
			bool onLine,
			string currentBaseline,
			string futureBaseline,
			string assignedFutureBaseline)
		{
            if (_logManager.IsTraceActive(TraceType.INFO))
            {
                _logManager.WriteLog(TraceType.INFO,
                    "ProcessDistributeBaselineRequest" + Environment.NewLine +
                    "(" + Environment.NewLine +
                    "  trainId                  : " + trainId + Environment.NewLine +
                    "  requestId                : " + requestId.ToString() + Environment.NewLine +
                    "  onLine                   : " + onLine.ToString() + Environment.NewLine +
                    "  currentBaseline          : " + currentBaseline + Environment.NewLine +
                    "  futureBaseline           : " + futureBaseline + Environment.NewLine +
                    "  assignedFutureBaseline   : " + assignedFutureBaseline + Environment.NewLine +
                    ")",
                    "PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessDistributeBaselineRequest",
                    null,
                    EventIdEnum.DataPackage);
            }

			try
			{
				lock (_baselineStatusUpdaterLock)
				{
					/// Checking initial conditions

					if (_isInitialized)
					{
						if (string.IsNullOrEmpty(trainId))
						{
							throw (new ArgumentNullException("trainId"));
						}

						TrainBaselineStatusExtendedData lUpdatedProgress = null;

						// Retrieve the status corresponding to that train if it already exists
						// 
						TryGetEntry(trainId, out lUpdatedProgress);

						if (lUpdatedProgress == null)
						{
							// Otherwise, create a new status object
							// 
							lUpdatedProgress = new TrainBaselineStatusExtendedData();
						}

						TrainBaselineStatusData lStatus = lUpdatedProgress.Status;

						if (lStatus == null)
						{
							lStatus = new TrainBaselineStatusData();
							lStatus.TrainNumber = UnknownVersion;
							lStatus.CurrentBaselineVersion = UnknownVersion;
							lStatus.FutureBaselineVersion = UnknownVersion;
							lStatus.PisOnBoardVersion = UnknownVersion;
							lUpdatedProgress.Status = lStatus;
						}

						// Update the status information from the method arguments
						// 
						lStatus.RequestId = requestId;
						lStatus.TaskId = 0; // Task Id will be known later
						lStatus.OnlineStatus = onLine;
						lStatus.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;

						if (!string.IsNullOrEmpty(currentBaseline))
						{
							lStatus.CurrentBaselineVersion = currentBaseline;
						}
						if (!string.IsNullOrEmpty(futureBaseline))
						{
							lStatus.FutureBaselineVersion = futureBaseline;
							lUpdatedProgress.OnBoardFutureBaseline = futureBaseline;
						}
						if (!string.IsNullOrEmpty(assignedFutureBaseline))
						{
							lUpdatedProgress.AssignedFutureBaseline = assignedFutureBaseline;
						}

						LogProgress(trainId, lUpdatedProgress);
					}
					else
					{
						if (_isHistoryLoggerAvailable)
						{
							throw (new InvalidOperationException("updater not initialized"));
						}
					}
				}
			}
			catch (Exception lException)
			{
                string message = string.Format(CultureInfo.CurrentCulture, "Method ProcessDistributeBaselineRequest('{0}', {1}, {2}, '{3}', '{4}', '{5}') Failed",
                    			trainId, requestId, onLine, currentBaseline, futureBaseline, assignedFutureBaseline);
				_logManager.WriteLog(TraceType.ERROR, message,
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessDistributeBaselineRequest",
					lException, EventIdEnum.DataPackage);
			}
		}

		/// <summary>Add the task identifier to the baseline deployment status entry for the given train.</summary>
		/// <param name="trainId">Train Id.</param>
		/// <param name="requestId">Associated request id.</param>
		/// <param name="taskId">Task Id.</param>
		public void ProcessTaskId(
			string trainId,
			Guid requestId,
			int taskId)
		{
            if (_logManager.IsTraceActive(TraceType.INFO))
            {
                _logManager.WriteLog(TraceType.INFO,
                    "ProcessTaskId" + Environment.NewLine +
                    "(" + Environment.NewLine +
                    "  trainId   : " + trainId + Environment.NewLine +
                    "  requestId : " + requestId.ToString() + Environment.NewLine +
                    "  taskId    : " + taskId + Environment.NewLine +
                    ")",
                    "PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessTaskId",
                    null,
                    EventIdEnum.DataPackage);
            }

			try
			{
				lock (_baselineStatusUpdaterLock)
				{
					/// Checking initial conditions

					if (_isInitialized)
					{
						if (string.IsNullOrEmpty(trainId))
						{
							throw new ArgumentNullException("trainId");
						}

						// Make sure a corresponding entry exists for that train
						// 
						TrainBaselineStatusExtendedData lUpdatedProgress = null;

						TryGetEntry(trainId, out lUpdatedProgress);

						if (lUpdatedProgress != null && lUpdatedProgress.Status != null)
						{
							// Make sure the request id matches what we already have
							// 
							if (lUpdatedProgress.Status.RequestId == requestId)
							{
                                lUpdatedProgress.Status.TaskId = taskId;

								// Write updated status to persistent storage
								//
								LogProgress(trainId, lUpdatedProgress);
							}
						}
					}
					else
					{
						if (_isHistoryLoggerAvailable)
						{
							throw (new InvalidOperationException("BaselineStatusUpdates is not initialized"));
						}
					}
				}
			}
			catch (Exception lException)
			{
                string message = string.Format(CultureInfo.CurrentCulture, "ProcessTaskId failed with parameters values:\r\n\tTrainId={0}\r\n\tRequestId={1}\r\n\tTaskId={2}", trainId, requestId, taskId);
				_logManager.WriteLog(TraceType.ERROR, message,
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessTaskId",
					lException, EventIdEnum.DataPackage);
			}
		}

		/// <summary>
		/// Update baseline deployment information for a specific train based on a T2G file 
		/// transfer notification.
		/// </summary>
		/// <param name="notification">The notification content.</param>
		public void ProcessFileTransferNotification(
			FileDistributionStatusArgs notification)
		{
            if (_logManager.IsTraceActive(TraceType.INFO))
            {
                string message = string.Format(CultureInfo.InvariantCulture,
                    "ProcessFileTransferNotification\r\n(\r\n" +
                    "  notification: \r\n{0}\r\n)",
                    notification);
                _logManager.WriteLog(TraceType.INFO,
                    message,
                    "PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessFileTransferNotification",
                    null,
                    EventIdEnum.DataPackage);
            }

			try
			{
				lock (_baselineStatusUpdaterLock)
				{
					/// Checking initial conditions

					if (_isInitialized)
					{
						if (notification == null)
						{
							throw (new ArgumentNullException("notification"));
						}

						// Find which train(s) is matching the notification task id
						// 

						var lProgresses = new Dictionary<string, TrainBaselineStatusExtendedData>(_baselineProgresses);

						foreach (KeyValuePair<string, TrainBaselineStatusExtendedData> progressEntry in lProgresses)
						{
							string lTrainId = progressEntry.Key;
							TrainBaselineStatusExtendedData lProgress = progressEntry.Value;

							if (lTrainId != null && lProgress != null && lProgress.Status != null)
							{
								TrainBaselineStatusData lCurrentStatus = lProgress.Status;

								if (lCurrentStatus.TaskId == notification.TaskId)
								{
									TrainBaselineStatusExtendedData lExtendedProgress = lProgress.Clone();

									// Check if status must be updated
									UpdateBaselineProgressFromFileTransferNotification(notification, ref lExtendedProgress);

									// Save updated status if different than original
									LogProgress(lTrainId, lExtendedProgress);
								}
							}
						}
					}
					else
					{
						if (_isHistoryLoggerAvailable)
						{
							throw new InvalidOperationException("BaselineStatusUpdater is not initialized");
						}
					}
				}
			}
            catch (Exception lException)
            {
                string message = string.Format(CultureInfo.CurrentCulture, "Failed to process notification: {0}", notification);
				_logManager.WriteLog(TraceType.ERROR, message,
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessFileTransferNotification",
					lException, EventIdEnum.DataPackage);
            }
		}

		/// <summary>Process the SIF notification.</summary>
		/// <param name="trainId">Identifier for the train.</param>
		/// <param name="notification">The SIF notification code.</param>
		public void ProcessSIFNotification(string trainId,
			PIS.Ground.GroundCore.AppGround.NotificationIdEnum notification)
		{
			string lNotificationString =
				Enum.GetName(typeof(PIS.Ground.GroundCore.AppGround.NotificationIdEnum), notification);

            if (_logManager.IsTraceActive(TraceType.INFO))
            {
                _logManager.WriteLog(TraceType.INFO,
                    "ProcessSIFNotification" + Environment.NewLine +
                    "(" + Environment.NewLine +
                    "  trainId      : " + trainId + Environment.NewLine +
                    "  notification : " + lNotificationString + Environment.NewLine +
                    ")",
                    "PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSIFNotification",
                    null,
                    EventIdEnum.DataPackage);
            }

			try
			{
				lock (_baselineStatusUpdaterLock)
				{

					/// Checking initial conditions

					if (_isInitialized)
					{
						if (string.IsNullOrEmpty(trainId))
						{
							throw (new ArgumentException("trainId is null or empty"));
						}

						TrainBaselineStatusExtendedData lExtendedUpdatedProgress = null;

						// Retrieve the status corresponding to that train if it already exists
						// 

						TryGetEntry(trainId, out lExtendedUpdatedProgress);

						if (lExtendedUpdatedProgress != null && lExtendedUpdatedProgress.Status != null)
						{
							UpdateBaselineProgressFromSIFNotification(notification,
								ref lExtendedUpdatedProgress);

							LogProgress(trainId, lExtendedUpdatedProgress);
						}
					}
					else
					{
						if (_isHistoryLoggerAvailable)
						{
							throw (new InvalidOperationException("updater not initialized"));
						}
					}
				}
			}
			catch (ArgumentException lException)
			{
				_logManager.WriteLog(TraceType.ERROR, "ProcessSIFNotification() - Argument Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSIFNotification",
					lException, EventIdEnum.DataPackage);
			}
			catch (InvalidOperationException lException)
			{
				_logManager.WriteLog(TraceType.ERROR, "ProcessSIFNotification() - Invalid Operation Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSIFNotification",
					lException, EventIdEnum.DataPackage);
			}
			catch (HistoryLoggerFailureException lException)
			{
				_logManager.WriteLog(TraceType.ERROR, "ProcessSIFNotification() - History Logger Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSIFNotification",
					lException, EventIdEnum.DataPackage);
			}
		}

		/// <summary>Process the train deleted notification.</summary>
		/// <param name="trainId">Identifier for the train.</param>
		public void ProcessElementDeletedNotification(string trainId)
		{
			try
			{
                if (_logManager.IsTraceActive(TraceType.INFO))
                {
                    _logManager.WriteLog(TraceType.INFO,
                        "ProcessElementDeletedNotification" + Environment.NewLine +
                        "(" + Environment.NewLine +
                        "  trainId      : " + trainId + Environment.NewLine +
                        ")",
                        "PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessElementDeletedNotification",
                        null,
                        EventIdEnum.DataPackage);
                }

                DeleteSystem(trainId);
			}
			catch (Exception lException)
			{
                string message = string.Format("Failed to process element deleted notification on element '{0}'", trainId);
				_logManager.WriteLog(TraceType.ERROR, message,
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessElementDeletedNotification",
					lException, EventIdEnum.DataPackage);
			}
		}

		/// <summary>
		/// Updated baseline deployment information for a specific train based on a system
		/// changed notification. This notification is received, for example, when one or
		/// more variables of interest has changed in the dictionary. 
		/// See T2GVehicleInfoConfig.xml.
		/// </summary>
		/// <param name="notification">The notification content.</param>
		/// <param name="assignedCurrentBaseline">The assigned current baseline.</param>
		/// <param name="assignedFutureBaseline">The assigned future baseline.</param>		
		public void ProcessSystemChangedNotification(
			SystemInfo notification,
			string assignedCurrentBaseline,
			string assignedFutureBaseline)
		{
            if (_logManager.IsTraceActive(TraceType.INFO))
            {
                string message = string.Format(CultureInfo.InvariantCulture,
                            "ProcessSystemChangedNotification \r\n(\r\n" +
                            "  notification            :\r\n{0}\r\n" +
                            "  assignedCurrentBaseline : {1}\r\n" +
                            "  assignedFutureBaseline  : {2}\r\n)"
                            , notification
                            , assignedCurrentBaseline
                            , assignedFutureBaseline);
                _logManager.WriteLog(TraceType.INFO,
                    message,
                    "PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSystemChangedNotification",
                    null,
                    EventIdEnum.DataPackage);
            }

			try
			{
				lock (_baselineStatusUpdaterLock)
				{
					// Checking initial conditions

					if (_isInitialized)
					{
						if (notification == null || string.IsNullOrEmpty(notification.SystemId))
						{
							throw (new ArgumentException("notification or at least one of its properties is null or empty"));
						}

						TrainBaselineStatusExtendedData lExtendedProgress = null;

						// Trim the strings passed as arguments
						// 
						string lTrainId = notification.SystemId.Trim();

						// Retrieve the status corresponding to that train if it already exists
						// 
						TryGetEntry(lTrainId, out lExtendedProgress);

						if (lExtendedProgress == null)
						{
							// Otherwise, create a new status object
							// 
							lExtendedProgress = new TrainBaselineStatusExtendedData();
						}

						if (!string.IsNullOrEmpty(assignedCurrentBaseline))
						{
                            lExtendedProgress.AssignedCurrentBaseline = assignedCurrentBaseline;
						}
						if (!string.IsNullOrEmpty(assignedFutureBaseline))
						{
							lExtendedProgress.AssignedFutureBaseline = assignedFutureBaseline;
						}

                        lExtendedProgress.Update(notification);

                        if (lExtendedProgress.IsT2GPollingRequired)
                        {
                            PerformDeepUpdate(lExtendedProgress);
                        }

						// Save updated status if different than original
						// 
						LogProgress(lTrainId, lExtendedProgress);
					}
					else
					{
						if (_isHistoryLoggerAvailable)
						{
							throw new InvalidOperationException("BaselineStatusUpdater object is not initialized");
						}
					}
				}
			}
			catch (ArgumentException lException)
			{
				_logManager.WriteLog(TraceType.ERROR, "ProcessSystemChangedNotification() - Argument Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSystemChangedNotification",
					lException, EventIdEnum.DataPackage);
			}
			catch (InvalidOperationException lException)
			{
				_logManager.WriteLog(TraceType.ERROR, "ProcessSystemChangedNotification() - Invalid Operation Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSystemChangedNotification",
					lException, EventIdEnum.DataPackage);
			}
			catch (HistoryLoggerFailureException lException)
			{
				_logManager.WriteLog(TraceType.ERROR, "ProcessSystemChangedNotification() - History Logger Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSystemChangedNotification",
					lException, EventIdEnum.DataPackage);
			}
		}

        /// <summary>
        /// Performs the deep update. This mean that T2G is interrogated about transfer task.
        /// </summary>
        /// <param name="progress">The progress.</param>
        protected void PerformDeepUpdate(TrainBaselineStatusExtendedData progress)
        {
            // Ask T2G if a transfer is targeting that train
            // If the guid is empty and task id is zero, this means that transfer task wasn't created yet. Deep update is not possible.
            if (progress.Status.RequestId != Guid.Empty && progress.Status.TaskId != 0)
            {
                if (_logManager.IsTraceActive(TraceType.INFO))
                {
                    string message = string.Format(CultureInfo.InvariantCulture, "Calling GetTransferTask() for request '{0}'", progress.Status.RequestId);
                    _logManager.WriteLog(TraceType.INFO,
                        message,
                        "PIS.Ground.DataPackage.BaselineStatusUpdater.PerformDeepUpdate",
                        null,
                        EventIdEnum.DataPackage);
                }

                List<Recipient> lRecipients;
                TransferTaskData lTask;
                string lT2GError = _t2g.GetTransferTask(progress.Status.TaskId, out lRecipients, out lTask); ;

                if (string.IsNullOrEmpty(lT2GError))
                {
                    if (lTask != null)
                    {
                        if (_logManager.IsTraceActive(TraceType.INFO))
                        {
                            _logManager.WriteLog(TraceType.INFO,
                                "GetTransferTask" + Environment.NewLine +
                                "(" + Environment.NewLine +
                                "    TaskId                        : " + lTask.TaskId + Environment.NewLine +
                                "    TaskStatus                    : " + Enum.GetName(typeof(TaskState), lTask.TaskState) + Environment.NewLine +
                                "    CurrentTaskPhase              : " + Enum.GetName(typeof(TaskPhase), lTask.TaskPhase) + Environment.NewLine +
                                "    AcquisitionCompletionPercent  : " + lTask.AcquisitionCompletionPercent.ToString() + Environment.NewLine +
                                "    DistributionCompletionPercent : " + lTask.DistributionCompletionPercent.ToString() + Environment.NewLine +
                                "    TransferCompletionPercent     : " + lTask.TransferCompletionPercent.ToString() + Environment.NewLine +
                                ")",
                                "PIS.Ground.DataPackage.BaselineStatusUpdater.PerformDeepUpdate",
                                null,
                                EventIdEnum.DataPackage);
                        }

                        FileDistributionStatusArgs lFileDistributionStatus = new FileDistributionStatusArgs();
                        lFileDistributionStatus.AcquisitionCompletionPercent = lTask.AcquisitionCompletionPercent;
                        lFileDistributionStatus.CurrentTaskPhase = lTask.TaskPhase;
                        lFileDistributionStatus.DistributionCompletionPercent = lTask.DistributionCompletionPercent;
                        lFileDistributionStatus.TransferCompletionPercent = lTask.TransferCompletionPercent;
                        lFileDistributionStatus.TaskStatus = lTask.TaskState;
                        lFileDistributionStatus.TaskId = lTask.TaskId;

                        UpdateBaselineProgressFromFileTransferNotification(lFileDistributionStatus, ref progress);
                        progress.IsT2GPollingRequired = false;
                    }
                    else
                    {
                        string message = string.Format(CultureInfo.CurrentCulture, "GetTransfertTask on task '{0}' returned a null task", progress.Status.TaskId);
                        _logManager.WriteLog(TraceType.ERROR, message,
                            "PIS.Ground.DataPackage.BaselineStatusUpdater.PerformDeepUpdate",
                            null, EventIdEnum.DataPackage);
                    }
                }
                else
                {

                    TraceType traceType;
                    if (_t2g.GetErrorCodeByDescription(lT2GError) == T2GFileDistributionManagerErrorEnum.eT2GFD_BadTaskId)
                    {
                        traceType = TraceType.WARNING;
                        progress.Status.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
                        progress.Status.RequestId = Guid.Empty;
                        progress.Status.TaskId = 0;
                        progress.IsT2GPollingRequired = false;
                    }
                    else
                    {
                        traceType = TraceType.ERROR;
                    }

                    if (_logManager.IsTraceActive(traceType))
                    {
                        string message = string.Format(CultureInfo.CurrentCulture, "GetTransfertTask on task '{0}' returned the T2G error '{1}'", progress.Status.TaskId, lT2GError);
                        _logManager.WriteLog(traceType, message,
                            "PIS.Ground.DataPackage.BaselineStatusUpdater.PerformDeepUpdate",
                            null, EventIdEnum.DataPackage);

                    }
                }

            }
            else if (progress.Status.RequestId == Guid.Empty)
            {
                progress.Status.TaskId = 0;
                progress.IsT2GPollingRequired = false;
            }
        }

		/// <summary> Updates the baseline progress information based on T2G file transfer notification.</summary>
		/// <param name="notification">The transfer task current notification.</param>
		/// <param name="baselineProgressInfo">[in,out] Information describing the baseline deployment progress.</param>
		protected void UpdateBaselineProgressFromFileTransferNotification(
			FileDistributionStatusArgs notification, ref TrainBaselineStatusExtendedData baselineProgressInfo)
		{
			if (baselineProgressInfo != null && notification != null)
			{
				TrainBaselineStatusData lStatus = baselineProgressInfo.Status;

				if (lStatus != null)
				{
					BaselineProgressStatusEnum lProgress = lStatus.ProgressStatus;

					// Updating the progress status only is a transfer might be on-going
					// 
					if (lProgress != BaselineProgressStatusEnum.UPDATED &&
						lProgress != BaselineProgressStatusEnum.DEPLOYED &&
						lStatus.RequestId != Guid.Empty)
					{
						switch (notification.TaskStatus)
						{
							case TaskState.Cancelled:
							case TaskState.Error:

								if (lProgress != BaselineProgressStatusEnum.TRANSFER_COMPLETED)
								{

									// The deployment will never complete
									//
									lStatus.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
									lStatus.RequestId = Guid.Empty;
									lStatus.TaskId = 0;

									// Indicating to the client the real future baseline (in case
									//  the assigned future baseline was indicated instead)
									//
									lStatus.FutureBaselineVersion = baselineProgressInfo.OnBoardFutureBaseline;
								}
								break;

							case TaskState.Completed:

								if (UpdateBaselineProgressStatus(ref lProgress, BaselineProgressStatusEnum.TRANSFER_COMPLETED))
								{
									lStatus.ProgressStatus = lProgress;

									// Indicating to the client what baseline is going to be deployed instead of 
									// the real future baseline
									// 
									lStatus.FutureBaselineVersion = baselineProgressInfo.AssignedFutureBaseline;
								}
								break;

							case TaskState.Started:

								if ((notification.CurrentTaskPhase == TaskPhase.Transfer && notification.TransferCompletionPercent > 0) ||
									notification.CurrentTaskPhase == TaskPhase.Distribution)
								{
									if (UpdateBaselineProgressStatus(ref lProgress, BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS))
									{
										lStatus.ProgressStatus = lProgress;
										lStatus.FutureBaselineVersion = baselineProgressInfo.AssignedFutureBaseline;
									}
								}
								else
								{
									if (UpdateBaselineProgressStatus(ref lProgress, BaselineProgressStatusEnum.TRANSFER_PLANNED))
									{
										lStatus.ProgressStatus = lProgress;
										lStatus.FutureBaselineVersion = baselineProgressInfo.AssignedFutureBaseline;
									}
								}
								break;

							case TaskState.Stopped:

								if (lStatus.ProgressStatus == BaselineProgressStatusEnum.TRANSFER_PLANNED ||
									lStatus.ProgressStatus == BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS)
								{
									if (UpdateBaselineProgressStatus(ref lProgress, BaselineProgressStatusEnum.TRANSFER_PAUSED))
									{
										lStatus.ProgressStatus = lProgress;
										lStatus.FutureBaselineVersion = baselineProgressInfo.AssignedFutureBaseline;
									}
								}
								break;

							case TaskState.Created:

								// Not enough information to change status
								// 
								break;

							default:

								_logManager.WriteLog(TraceType.ERROR,
									"UpdateBaselineProgressFromFileTransferNotification() : Unexpected TaskState code: " + notification.TaskStatus,
									"PIS.Ground.DataPackage.BaselineStatusUpdater.UpdateBaselineProgressFromFileTransferNotification",
									null, EventIdEnum.DataPackage);

								break;
						}
					}
				}
			}
		}

		/// <summary>Updates the baseline progress from SIF notification.</summary>
		/// <param name="notification">The notification code.</param>
		/// <param name="baselineProgressInfo">[in,out] Information describing the updated baseline progress.</param>
		private void UpdateBaselineProgressFromSIFNotification(
			PIS.Ground.GroundCore.AppGround.NotificationIdEnum notification,
			ref TrainBaselineStatusExtendedData baselineProgressInfo)
		{
			if (baselineProgressInfo != null && baselineProgressInfo.Status != null &&
				baselineProgressInfo.Status.RequestId != Guid.Empty)
			{
				BaselineProgressStatusEnum lProgress = baselineProgressInfo.Status.ProgressStatus;

				switch (lProgress)
				{
					case BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS:
					case BaselineProgressStatusEnum.TRANSFER_PAUSED:

						switch (notification)
						{
							case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.Completed:

								// SIF:  Data Package Updater Notification
								// 
								// Note: Transition to the state BaselineProgressStatusEnum.DEPLOYED will be confirmed by 
								//       a "System has Changed" notification based on the future baseline version.
								//       We will perform the transition at that moment only in order to simplify 
								//       the state machine.
								break;

							case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionCompleted:

								// SIF: Central File Warehouse Notification
								// 
								// Note: Transition to the state BaselineProgressStatusEnum.TRANSFER_COMPLETED will be confirmed by
								//       a "File Transfer" notification.
								//       We will perform the transition at that moment only in order to simplify 
								//       the state machine.
								break;

							case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.Failed:
							case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedMissingDataPackage:
							case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedNoAssignedFutureBaseline:
							case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedRejectedByElement:
							case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedTransferCancelledManually:
							case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedUnknowOnBoardBaseline:
							case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedUnknowOnBoardDataPackage:
							case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionInhibited:
							case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionTimedOut:
							case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionUnknowElementId:

								// Something went wrong during deployment.
								// It will never complete.

								baselineProgressInfo.Status.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
								baselineProgressInfo.Status.RequestId = Guid.Empty;
								baselineProgressInfo.Status.TaskId = 0;

								// Indicating to the client the real future baseline instead of th assigned future baseline

								baselineProgressInfo.Status.FutureBaselineVersion = baselineProgressInfo.OnBoardFutureBaseline;

								break;

							default:

								// Nothing to do: this notification is of no interest.
								// 
								break;
						}
						break;
				}
			}
		}

		/// <summary>
		/// Updates the baseline progress status if the proposed new state is valid considering the
		/// current state.
		/// </summary>
		/// <param name="currentState">[in,out] The current state.</param>
		/// <param name="newState">The proposed new state.</param>
		/// <returns>true if transition is valid, false otherwise.</returns>		
		private bool UpdateBaselineProgressStatus(
			ref BaselineProgressStatusEnum currentState,
			BaselineProgressStatusEnum newState)
		{
			bool lSuccess = false;

			BaselineProgressStatusEnum lResultingStatus = ValidateBaselineProgressStatus(currentState, newState);

			if (lResultingStatus != currentState)
			{
				currentState = lResultingStatus;
				lSuccess = true;
			}

			return lSuccess;
		}

		/// <summary>Updates the baseline progress status if the proposed new state is valid considering the current state.</summary>
		/// <param name="currentState">The current state.</param>
		/// <param name="newState">The proposed new state.</param>
		/// <returns>The effective new state.</returns>
		protected BaselineProgressStatusEnum ValidateBaselineProgressStatus(
			BaselineProgressStatusEnum currentState,
			BaselineProgressStatusEnum newState)
		{
			BaselineProgressStatusEnum lNewValidState = currentState;

			/// In general, each new state must be smaller in numerical value than the current state

			if (newState < currentState)
			{
				lNewValidState = newState;
			}
			else if (newState == BaselineProgressStatusEnum.TRANSFER_PAUSED &&
				currentState == BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS)
			{
				/// On the other hand, both pause and in-progress states can alternate

				lNewValidState = newState;
			}
			else if (newState == BaselineProgressStatusEnum.TRANSFER_PLANNED &&
				currentState == BaselineProgressStatusEnum.TRANSFER_PAUSED)
			{
				/// On the other hand, both pause and planned states can alternate

				lNewValidState = newState;
			}
			else
			{
				// Nothing to do
			}

			return lNewValidState;
		}

		/// <summary>Removes a baseline progress entry from the history logger.</summary>
		/// <param name="trainId">Identifier for the train.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		protected bool RemoveProgressFromHistoryLogger(string trainId)
		{
			bool lSuccess = false;

			if (trainId != null)
			{
                if (_logManager.IsTraceActive(TraceType.INFO))
                {
				    _logManager.WriteLog(TraceType.INFO,
					    "RemoveProgressFromHistoryLogger" + Environment.NewLine +
					    "(" + Environment.NewLine +
					    "  TrainId : " + trainId + Environment.NewLine +
					    ")",
					    "PIS.Ground.DataPackage.BaselineStatusUpdater.RemoveProgressFromHistoryLogger",
					    null,
					    EventIdEnum.DataPackage);
                }

				ResultCodeEnum resultCode = _logManager.CleanTrainBaselineStatus(trainId);

				if (resultCode == ResultCodeEnum.RequestAccepted || resultCode == ResultCodeEnum.Empty_Result)
				{
					lSuccess = true;
				}
			}

			return lSuccess;
		}

		/// <summary>Updates the baseline progress entry on history logger.</summary>
		/// <param name="trainId">Identifier for the train.</param>
		/// <param name="progressInfo">Information describing the updated progress.</param>
		/// <returns>true if it succeeds, false otherwise.</returns>
		protected bool UpdateProgressOnHistoryLogger(string trainId, TrainBaselineStatusData progressInfo)
		{
			bool lSuccess = false;

			if (trainId != null && progressInfo != null)
			{
                if (_logManager.IsTraceActive(TraceType.INFO))
                {
                    string lProgressStatusString =
                        Enum.GetName(typeof(BaselineProgressStatusEnum), progressInfo.ProgressStatus);

                    _logManager.WriteLog(TraceType.INFO,
                        "UpdateProgressOnHistoryLogger" + Environment.NewLine +
                        "(" + Environment.NewLine +
                        "  TrainId                  : " + trainId + Environment.NewLine +
                        "  RequestId                : " + progressInfo.RequestId.ToString() + Environment.NewLine +
                        "  TaskId                   : " + progressInfo.TaskId + Environment.NewLine +
                        "  TrainNumber              : " + progressInfo.TrainNumber + Environment.NewLine +
                        "  OnlineStatus             : " + progressInfo.OnlineStatus.ToString() + Environment.NewLine +
                        "  ProgressStatus           : " + lProgressStatusString + Environment.NewLine +
                        "  CurrentBaselineVersion   : " + progressInfo.CurrentBaselineVersion + Environment.NewLine +
                        "  FutureBaselineVersion    : " + progressInfo.FutureBaselineVersion + Environment.NewLine +
                        "  PisOnBoardVersion        : " + progressInfo.PisOnBoardVersion + Environment.NewLine +
                        ")",
                        "PIS.Ground.DataPackage.BaselineStatusUpdater.UpdateProgressOnHistoryLogger",
                        null,
                        EventIdEnum.DataPackage);
                }

				ResultCodeEnum resultCode = _logManager.UpdateTrainBaselineStatus(
					trainId, progressInfo.RequestId, progressInfo.TaskId, progressInfo.TrainNumber,
					progressInfo.OnlineStatus, progressInfo.ProgressStatus,
					progressInfo.CurrentBaselineVersion, progressInfo.FutureBaselineVersion,
					progressInfo.PisOnBoardVersion);

                if (resultCode == ResultCodeEnum.RequestAccepted)
				{
					lSuccess = true;
				}
			}

			return lSuccess;
		}

        /// <summary>
        /// Called when the distribution of an upload request create the transfer task.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The arguments of the notification.</param>
        public void OnFileDistributionTaskCreated(object sender, FileDistributionTaskCreatedArgs e)
        {
            try
            {
                if (_logManager.IsTraceActive(TraceType.INFO))
                {
                    string message = "OnFileDistributionTaskCreated " + Environment.NewLine +
                        "( " + Environment.NewLine +
                        "  Request   : " + e.RequestId.ToString() + Environment.NewLine +
                        "  Task id   : " + e.TaskId.ToString() + Environment.NewLine +
                        "  Recipient : " + ((e.Recipients != null && e.Recipients.Count > 0) ? e.Recipients[0].SystemId ?? "" + e.Recipients[0].MissionId ?? "" : string.Empty) + Environment.NewLine +
                        ")";

                    _logManager.WriteLog(TraceType.INFO,
                        message,
                        "PIS.Ground.DataPackage.BaselineStatusUpdater.OnFileDistributionTaskCreated",
                        null,
                        EventIdEnum.DataPackage);
                }

                lock (_baselineStatusUpdaterLock)
                {
                    // Checking initial conditions

                    if (_isInitialized)
                    {
                        if (e.Recipients == null || e.Recipients.Count != 1 || string.IsNullOrEmpty(e.Recipients[0].SystemId))
                        {
                            _logManager.WriteLog(TraceType.DEBUG, "Notification excluded because the recipient count differ than one or the recipient system id is an empty string", "PIS.Ground.DataPackage.BaselineStatusUpdater.OnFileDistributionTaskCreated", null, EventIdEnum.DataPackage);
                        }
                        else
                        {
                            TrainBaselineStatusExtendedData lExtendedProgress = null;

                            // Trim the strings passed as arguments
                            // 
                            string lTrainId = e.Recipients[0].SystemId;

                            // Retrieve the status corresponding to that train if it already exists
                            // 
                            if (!TryGetEntry(lTrainId, out lExtendedProgress))
                            {
                                string message = string.Format(CultureInfo.CurrentCulture, Logs.WARNING_TASK_CREATED_NOTIFICATION_IGNORED_NO_DISTRIBUTION_FOR_TRAIN, lTrainId);
                                _logManager.WriteLog(TraceType.WARNING, message, "PIS.Ground.DataPackage.BaselineStatusUpdater.OnFileDistributionTaskCreated", null, EventIdEnum.DataPackage);
                            }
                            else if (lExtendedProgress.Status == null || lExtendedProgress.Status.RequestId != e.RequestId)
                            {
                                string message = string.Format(CultureInfo.CurrentCulture, Logs.WARNING_TASK_CREATED_NOTIFICATION_IGNORED_REQUEST_ID_DIFFER, lTrainId, e.RequestId);
                                _logManager.WriteLog(TraceType.WARNING, message, "PIS.Ground.DataPackage.BaselineStatusUpdater.OnFileDistributionTaskCreated", null, EventIdEnum.DataPackage);
                            }
                            else
                            {
                                lExtendedProgress.Status.TaskId = e.TaskId;
                                LogProgress(lTrainId, lExtendedProgress);
                            }
                        }
                    }
                    else
                    {
                        if (_isHistoryLoggerAvailable)
                        {
                            throw (new InvalidOperationException("updater not initialized"));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logManager.WriteLog(TraceType.EXCEPTION, exception.Message,
                    "PIS.Ground.DataPackage.BaselineStatusUpdater.OnFileDistributionTaskCreated",
                    exception, EventIdEnum.DataPackage);
            }
        }

        /// <summary>
        /// Deletes the system.
        /// </summary>
        /// <param name="trainId">The name of the system to delete.</param>
        protected void DeleteSystem(string trainId)
        {
            try
            {
                if (_logManager.IsTraceActive(TraceType.INFO))
                {
                    string message = string.Format(CultureInfo.CurrentCulture, "DeleteSystem({0})", trainId);
                    _logManager.WriteLog(TraceType.INFO,
                        message,
                        "PIS.Ground.DataPackage.BaselineStatusUpdater.OnFileDistributionTaskCreated",
                        null,
                        EventIdEnum.DataPackage);
                }

                if (string.IsNullOrEmpty(trainId))
                {
                    throw new ArgumentNullException("trainId");
                }


                lock (_baselineStatusUpdaterLock)
                {
                    // Checking initial conditions

                    if (_isInitialized)
                    {
                        if (_baselineProgresses.Remove(trainId))
                        {
                            if (!_baselineProgressRemoveProcedure(trainId))
                            {
                                throw new HistoryLoggerFailureException("Failed to update history log database");
                            }
                        }
                    }
                    else
                    {
                        if (_isHistoryLoggerAvailable)
                        {
                            throw new InvalidOperationException("BaselineStatusUpdater is not initialized");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                string message = string.Format(CultureInfo.CurrentCulture, "Failed to delete system '{0}' from baseline statuses", trainId);
                _logManager.WriteLog(TraceType.ERROR, message,
                    "PIS.Ground.DataPackage.BaselineStatusUpdater.DeleteSystem",
                    exception, EventIdEnum.DataPackage);
            }
        }

        /// <summary>
        /// Forces the progress for a specific train.
        /// </summary>
        /// <param name="data">The progress data to force.</param>
        /// <remarks>Designed to be used in automated test only.</remarks>
        public void ForceProgress(TrainBaselineStatusExtendedData data)
        {
            lock (_baselineStatusUpdaterLock)
            {
                _baselineProgresses[data.Status.TrainId] = data;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Uninitialize();
            }
        }

        #endregion
    }
}