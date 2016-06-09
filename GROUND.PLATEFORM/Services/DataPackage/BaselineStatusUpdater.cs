//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineStatusUpdater.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.Data;
using PIS.Ground.DataPackage.Notification;
using PIS.Ground.RemoteDataStore;
using PIS.Ground.Core.T2G;

namespace PIS.Ground.DataPackage
{
	/// <summary>Train baseline deployment status with extended data.</summary>
	public class TrainBaselineStatusExtendedData
	{
		/// <summary>The basic status information.</summary>
		private TrainBaselineStatusData _status;

		/// <summary>The assigned future baseline version.</summary>
		private string _assignedFutureBaseline;

		/// <summary>The on-board future baseline. 
		/// 		 Note: _status._futureBaselineVersion, in some conditions, may be forced artificially to the
		/// 		 _assignedFutureBaseline value. On the other hand, the _onBoardFutureBaseline
		/// 		 will always reflect the future baseline loaded on the train.</summary>
		private string _onBoardFutureBaseline;

		/// <summary>true if T2G must be ask for possible on-going transfer.</summary>
		private bool _isT2GPollingRequired;

		/// <summary>Initializes a new instance of the TrainBaselineStatusExtendedData class.</summary>
		/// <param name="status">The basic status information.</param>
		/// <param name="assignedFutureBaseline">The extended information: assigned future baseline version.</param>
		/// <param name="onBoardFutureBaseline">The extended information: on board future baseline version.</param>		
		/// <param name="isT2GPollingRequired">true if T2G must be ask for possible on-going transfer.</param>
		public TrainBaselineStatusExtendedData(
			TrainBaselineStatusData status,
			string assignedFutureBaseline,
			string onBoardFutureBaseline,
			bool isT2GPollingRequired)
		{
			_status = status;
			_assignedFutureBaseline = assignedFutureBaseline;
			_onBoardFutureBaseline = onBoardFutureBaseline;
			_isT2GPollingRequired = isT2GPollingRequired;
		}

		/// <summary>
		/// Initializes a new instance of the TrainBaselineStatusExtendedData class.
		/// </summary>
		/// <param name="status">The basic status information.</param>
		public TrainBaselineStatusExtendedData(
			TrainBaselineStatusData status)
		{
			_status = status;
			_assignedFutureBaseline = null;
			_onBoardFutureBaseline = null;
			_isT2GPollingRequired = true;
		}

		/// <summary>
		/// Initializes a new instance of the TrainBaselineStatusExtendedData class.
		/// </summary>
		public TrainBaselineStatusExtendedData()
		{
			_status = null;
			_assignedFutureBaseline = null;
			_onBoardFutureBaseline = null;
			_isT2GPollingRequired = true;
		}

		/// <summary>Gets or sets the basic status information.</summary>
		/// <value>The status.</value>
		public TrainBaselineStatusData Status
		{
			get { return _status; }
			set { _status = value; }
		}

		/// <summary>Gets or sets the assigned future baseline.</summary>
		/// <value>The assigned future baseline.</value>
		public string AssignedFutureBaseline
		{
			get { return _assignedFutureBaseline; }
			set { _assignedFutureBaseline = value; }
		}

		/// <summary>Gets or sets the on-board future baseline.</summary>
		/// <value>The on-board future baseline.</value>
		public string OnBoardFutureBaseline
		{
			get { return _onBoardFutureBaseline; }
			set { _onBoardFutureBaseline = value; }
		}

		/// <summary>Gets or sets the T2G polling required flag.</summary>
		/// <value>The flag.</value>
		public bool IsT2GPollingRequired
		{
			get { return _isT2GPollingRequired; }
			set { _isT2GPollingRequired = value; }
		}

		/// <summary>Makes a deep copy of the current TrainBaselineStatusExtendedData object.</summary>
		/// <returns>A copy of the original object.</returns>
		public TrainBaselineStatusExtendedData Clone()
		{
			TrainBaselineStatusExtendedData lExtendedCopy = null;

			lExtendedCopy = (TrainBaselineStatusExtendedData)this.MemberwiseClone();

			if (this.Status != null)
			{
				lExtendedCopy.Status = this.Status.Clone();
			}

			return lExtendedCopy;
		}

		/// <summary>Compares the content of two TrainBaselineStatusExtendedData objects.</summary>
		/// <param name="object1">First object to be compared.</param>
		/// <param name="object2">Second object to be compared.</param>
		/// <returns>true if equal, false if not.</returns>
		public static bool AreEqual(TrainBaselineStatusExtendedData object1, TrainBaselineStatusExtendedData object2)
		{
			bool lEqual = true;

			if (object1 != null && object2 != null)
			{

				if (object1.AssignedFutureBaseline != object2.AssignedFutureBaseline)
				{
					lEqual = false;
				}
				else if (object1.IsT2GPollingRequired != object2.IsT2GPollingRequired)
				{
					lEqual = false;
				}
				else if (object1.OnBoardFutureBaseline != object2.OnBoardFutureBaseline)
				{
					lEqual = false;
				}
				else if (!TrainBaselineStatusData.AreEqual(object1.Status, object2.Status))
				{
					lEqual = false;
				}
				else
				{
					lEqual = true;
				}
			}
			else
			{
				lEqual = false;
			}

			return lEqual;
		}
	}

	/// <summary>Class providing baseline status update capabilities.</summary>
	class BaselineStatusUpdater
	{
		/// <summary>Exception for signalling history logger not installed errors.</summary>
		private class HistoryLoggerNotInstalledException : Exception
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
		private class HistoryLoggerFailureException : Exception
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
		private static bool _isInitialized;

		/// <summary>true if the persistent storage is installed, false otherwise.</summary>
		private static bool _isHistoryLoggerAvailable;

		/// <summary>The client providing access to the T2G server.</summary>
		private static IT2GFileDistributionManager _t2g;

		/// <summary>List of baseline progresses loaded from persistent storage and extended with additional information.</summary>
		private static Dictionary<string, TrainBaselineStatusExtendedData> _baselineProgresses;

		/// <summary>The baseline progress update procedure in persistent storage.</summary>
		private static BaselineProgressUpdateProcedure _baselineProgressUpdateProcedure;

		/// <summary>The baseline progress remove procedure from persistent storage.</summary>
		private static BaselineProgressRemoveProcedure _baselineProgressRemoveProcedure;

		/// <summary>The multithreading protection lock.</summary>
		private static object _baselineStatusUpdaterLock;

		/// <summary>Initializes a new instance of the BaselineStatusUpdater class by initializing its members.</summary>
		static BaselineStatusUpdater()
		{
			_isInitialized = false;
			_isHistoryLoggerAvailable = true;
			_t2g = null;
			_baselineProgresses = null;
			_baselineProgressUpdateProcedure = null;
			_baselineProgressRemoveProcedure = null;
			_baselineStatusUpdaterLock = new Object();
		}

		/// <summary>Initializes BaselineStatusUpdater. Must be called before any other public methods.</summary>
		/// <param name="t2g">The client providing access to the T2G server. Must not be null.</param>
		/// <returns>true if it succeeds, false otherwise.</returns>		
		public static bool Initialize(IT2GFileDistributionManager t2g)
		{
			LogManager.WriteLog(TraceType.INFO,
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
						throw (new InvalidOperationException("already initialized"));
					}

					if (t2g == null)
					{
						throw (new ArgumentNullException("t2g"));
					}

					// Get latest status from persistent storage
					// 
					BaselineStatusResponse lResult = new BaselineStatusResponse();
					Dictionary<string, TrainBaselineStatusData> lDictionaryResponse = null;

					lResult.ResultCode = LogManager.GetTrainBaselineStatus(out lDictionaryResponse);

					if (lResult.ResultCode != ResultCodeEnum.RequestAccepted)
					{
						throw (new HistoryLoggerFailureException("LogManager returned: " +
							Enum.GetName(typeof(ResultCodeEnum), lResult.ResultCode)));
					}

					if (lDictionaryResponse == null)
					{
						_isHistoryLoggerAvailable = false;
						throw (new HistoryLoggerNotInstalledException("LogManager returned null: Is the HistoryLogger installed?"));
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
						t2g);
				}
			}
			catch (ArgumentNullException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "Initialize() - Argument Null Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.Initialize",
					lException, EventIdEnum.DataPackage);
			}
			catch (InvalidOperationException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "Initialize() - Invalid Operation Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.Initialize",
					lException, EventIdEnum.DataPackage);
			}
			catch (HistoryLoggerFailureException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "Initialize() - History Logger Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.Initialize",
					lException, EventIdEnum.DataPackage);
			}
			catch (HistoryLoggerNotInstalledException lException)
			{
				LogManager.WriteLog(TraceType.WARNING, "Initialize() - History Logger Exception : ",
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
		/// <returns>true if it succeeds, false otherwise.</returns>	
		private static bool Initialize(
			Dictionary<string, TrainBaselineStatusExtendedData> baselineProgresses,
			BaselineProgressUpdateProcedure baselineProgressUpdateProcedure,
			BaselineProgressRemoveProcedure baselineProgressRemoveProcedure,
			IT2GFileDistributionManager t2g)
		{
			// Save the method arguments to class fields
			// 
			_baselineProgresses = baselineProgresses;
			_baselineProgressUpdateProcedure = baselineProgressUpdateProcedure;
			_baselineProgressRemoveProcedure = baselineProgressRemoveProcedure;
			_t2g = t2g;

			// Ok. From now one, other methods of this class may be called
			// 
			_isInitialized = true;

			// For now, assume all elements are off-line and that we do not know
			// the different baselines and software versions and other information that could have changed
			// 
			ResetStatusEntries();

			return _isInitialized;
		}

		/// <summary>Resets all status entries by clearing the fields that might have changed.</summary>
		public static void ResetStatusEntries()
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

									lStatus.OnlineStatus = false;

									LogProgress(lTrainId, lExtendedStatus);
								}
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
			catch (InvalidOperationException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ResetStatusEntries() - Invalid Operation Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ResetStatusEntries",
					lException, EventIdEnum.DataPackage);
			}
			catch (HistoryLoggerFailureException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ResetStatusEntries() - History Logger Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ResetStatusEntries",
					lException, EventIdEnum.DataPackage);
			}
		}

		/// <summary>Try to get the progress entry corresponding to the specifed train.</summary>
		/// <param name="trainId">Identifier of the train.</param>
		/// <param name="lStatus">[out] The status.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		private static bool TryGetEntry(string trainId, out TrainBaselineStatusExtendedData lStatus)
		{
			bool lSuccess = false;
			lStatus = null;

			TrainBaselineStatusExtendedData lOriginalStatus = null;

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
		private static void LogProgress(string trainId, TrainBaselineStatusExtendedData lNewStatus)
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
							throw (new HistoryLoggerFailureException("LogProgress() : cannot update HistoryLog entry for train: " + trainId));
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
		public static void ProcessDistributeBaselineRequest(
			string trainId,
			Guid requestId,
			bool onLine,
			string currentBaseline,
			string futureBaseline,
			string assignedFutureBaseline)
		{
			LogManager.WriteLog(TraceType.INFO,
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
			catch (ArgumentException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessDistributeBaselineRequest() - Argument Null Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessDistributeBaselineRequest",
					lException, EventIdEnum.DataPackage);
			}
			catch (InvalidOperationException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessDistributeBaselineRequest() - Invalid Operation Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessDistributeBaselineRequest",
					lException, EventIdEnum.DataPackage);
			}
			catch (HistoryLoggerFailureException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessDistributeBaselineRequest() - History Logger Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessDistributeBaselineRequest",
					lException, EventIdEnum.DataPackage);
			}
		}

		/// <summary>Add the task identifier to the baseline deployment status entry for the given train.</summary>
		/// <param name="trainId">Train Id.</param>
		/// <param name="requestId">Associated request id.</param>
		/// <param name="taskId">Task Id.</param>
		public static void ProcessTaskId(
			string trainId,
			Guid requestId,
			int taskId)
		{
			LogManager.WriteLog(TraceType.INFO,
				"ProcessTaskId" + Environment.NewLine +
				"(" + Environment.NewLine +
				"  trainId   : " + trainId + Environment.NewLine +
				"  requestId : " + requestId.ToString() + Environment.NewLine +
				"  taskId    : " + taskId + Environment.NewLine +
				")",
				"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessTaskId",
				null,
				EventIdEnum.DataPackage);

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
							throw (new InvalidOperationException("updater not initialized"));
						}
					}
				}
			}
			catch (ArgumentException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessTaskId() - Argument Null Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessTaskId",
					lException, EventIdEnum.DataPackage);
			}
			catch (InvalidOperationException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessTaskId() - Invalid Operation Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessTaskId",
					lException, EventIdEnum.DataPackage);
			}
			catch (HistoryLoggerFailureException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessTaskId() - History Logger Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessTaskId",
					lException, EventIdEnum.DataPackage);
			}
		}

		/// <summary>
		/// Update baseline deployment information for a specific train based on a T2G file 
		/// transfer notification.
		/// </summary>
		/// <param name="notification">The notification content.</param>
		public static void ProcessFileTransferNotification(
			FileDistributionStatusArgs notification)
		{
			LogManager.WriteLog(TraceType.INFO,
				"ProcessFileTransferNotification" + Environment.NewLine +
				"(" + Environment.NewLine +
				"  notification: " + Environment.NewLine +
				ToString(notification) + Environment.NewLine +
				")",
				"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessFileTransferNotification",
				null,
				EventIdEnum.DataPackage);

			try
			{
				lock (_baselineStatusUpdaterLock)
				{
					/// Checking initial conditions

					if (_isInitialized)
					{
						if (notification == null)
						{
							throw (new ArgumentNullException("notification is null"));
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
							throw (new InvalidOperationException("updater not initialized"));
						}
					}
				}
			}
			catch (ArgumentNullException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessFileTransferNotification() - Argument Null Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessFileTransferNotification",
					lException, EventIdEnum.DataPackage);
			}
			catch (InvalidOperationException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessFileTransferNotification() - Invalid Operation Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessFileTransferNotification",
					lException, EventIdEnum.DataPackage);
			}
			catch (HistoryLoggerFailureException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessFileTransferNotification() - History Logger Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessFileTransferNotification",
					lException, EventIdEnum.DataPackage);
			}
		}

		/// <summary>Process the SIF notification.</summary>
		/// <param name="trainId">Identifier for the train.</param>
		/// <param name="notification">The SIF notification code.</param>
		public static void ProcessSIFNotification(string trainId,
			PIS.Ground.GroundCore.AppGround.NotificationIdEnum notification)
		{
			string lNotificationString =
				Enum.GetName(typeof(PIS.Ground.GroundCore.AppGround.NotificationIdEnum), notification);

			LogManager.WriteLog(TraceType.INFO,
				"ProcessSIFNotification" + Environment.NewLine +
				"(" + Environment.NewLine +
				"  trainId      : " + trainId + Environment.NewLine +
				"  notification : " + lNotificationString + Environment.NewLine +
				")",
				"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSIFNotification",
				null,
				EventIdEnum.DataPackage);

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
				LogManager.WriteLog(TraceType.ERROR, "ProcessSIFNotification() - Argument Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSIFNotification",
					lException, EventIdEnum.DataPackage);
			}
			catch (InvalidOperationException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessSIFNotification() - Invalid Operation Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSIFNotification",
					lException, EventIdEnum.DataPackage);
			}
			catch (HistoryLoggerFailureException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessSIFNotification() - History Logger Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSIFNotification",
					lException, EventIdEnum.DataPackage);
			}
		}

		/// <summary>Process the train deleted notification.</summary>
		/// <param name="trainId">Identifier for the train.</param>
		public static void ProcessElementDeletedNotification(string trainId)
		{
			LogManager.WriteLog(TraceType.INFO,
				"ProcessElementDeletedNotification" + Environment.NewLine +
				"(" + Environment.NewLine +
				"  trainId      : " + trainId + Environment.NewLine +
				")",
				"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessElementDeletedNotification",
				null,
				EventIdEnum.DataPackage);

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

						if (_baselineProgresses.ContainsKey(trainId))
						{
							// Train deleted: remove corresponding entry
							// 
							_baselineProgresses.Remove(trainId);

							if (_baselineProgressRemoveProcedure(trainId) == false)
							{
								throw (new HistoryLoggerFailureException("cannot remove HistoryLog entry for train: " + trainId));
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
			catch (ArgumentException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessElementDeletedNotification() - Argument Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessElementDeletedNotification",
					lException, EventIdEnum.DataPackage);
			}
			catch (InvalidOperationException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessElementDeletedNotification() - Invalid Operation Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessElementDeletedNotification",
					lException, EventIdEnum.DataPackage);
			}
			catch (HistoryLoggerFailureException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessElementDeletedNotification() - History Logger Exception :",
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
		public static void ProcessSystemChangedNotification(
			SystemInfo notification,
			string assignedCurrentBaseline,
			string assignedFutureBaseline)
		{
			LogManager.WriteLog(TraceType.INFO,
				"ProcessSystemChangedNotification " + Environment.NewLine +
				"( " + Environment.NewLine +
				"  notification            : " + Environment.NewLine +
				ToString(notification) + Environment.NewLine +
				"  assignedCurrentBaseline : " + assignedCurrentBaseline + Environment.NewLine +
				"  assignedFutureBaseline  : " + assignedFutureBaseline + Environment.NewLine +
				")",
				"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSystemChangedNotification",
				null,
				EventIdEnum.DataPackage);

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

						TrainBaselineStatusData lStatus = lExtendedProgress.Status;

						if (lStatus == null)
						{
							lStatus = new TrainBaselineStatusData();
							lStatus.TrainNumber = UnknownVersion;
							lStatus.CurrentBaselineVersion = UnknownVersion;
							lStatus.FutureBaselineVersion = UnknownVersion;
							lStatus.PisOnBoardVersion = UnknownVersion;
							lExtendedProgress.Status = lStatus;
						}

						// Save the assigned future baseline to be used later
						// 
						string lAssignedCurrentBaseline = UnknownVersion;

						if (!string.IsNullOrEmpty(assignedCurrentBaseline))
						{
							lAssignedCurrentBaseline = assignedCurrentBaseline;
						}
						if (!string.IsNullOrEmpty(assignedFutureBaseline))
						{
							lExtendedProgress.AssignedFutureBaseline = assignedFutureBaseline;
						}

						// Update the status information from the method arguments
						// 
						TrainBaselineStatusData lUpdatedProgress = null;
						string lOnBoardFutureBaseline = lExtendedProgress.OnBoardFutureBaseline;
						bool lDeepUpdate = lExtendedProgress.IsT2GPollingRequired;

						ProcessSystemChangedNotification(
							notification,
							lAssignedCurrentBaseline,
							lExtendedProgress.AssignedFutureBaseline,
							ref lOnBoardFutureBaseline,
							ref lDeepUpdate,
							lExtendedProgress.Status,
							out lUpdatedProgress);

						lExtendedProgress.Status = lUpdatedProgress;
						lExtendedProgress.OnBoardFutureBaseline = lOnBoardFutureBaseline;
						lExtendedProgress.IsT2GPollingRequired = lDeepUpdate;

						// Save updated status if different than original
						// 
						LogProgress(lTrainId, lExtendedProgress);
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
				LogManager.WriteLog(TraceType.ERROR, "ProcessSystemChangedNotification() - Argument Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSystemChangedNotification",
					lException, EventIdEnum.DataPackage);
			}
			catch (InvalidOperationException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessSystemChangedNotification() - Invalid Operation Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSystemChangedNotification",
					lException, EventIdEnum.DataPackage);
			}
			catch (HistoryLoggerFailureException lException)
			{
				LogManager.WriteLog(TraceType.ERROR, "ProcessSystemChangedNotification() - History Logger Exception :",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSystemChangedNotification",
					lException, EventIdEnum.DataPackage);
			}
		}

		/// <summary>Convert a File Distribution Status object into a string representation.</summary>
		/// <param name="info">The input information.</param>
		/// <returns>Info as a string.</returns>
		private static string ToString(FileDistributionStatusArgs info)
		{
			string lResult = string.Empty;

			if (info != null)
			{
				lResult +=
					"    RequestId                     : " + info.RequestId.ToString() + Environment.NewLine +
					"    TaskId                        : " + info.TaskId + Environment.NewLine +
					"    TaskStatus                    : " + Enum.GetName(typeof(TaskState), info.TaskStatus) + Environment.NewLine +
					"    CurrentTaskPhase              : " + Enum.GetName(typeof(TaskPhase), info.CurrentTaskPhase) + Environment.NewLine +
					"    AcquisitionCompletionPercent  : " + info.AcquisitionCompletionPercent.ToString() + Environment.NewLine +
					"    DistributionCompletionPercent : " + info.DistributionCompletionPercent.ToString() + Environment.NewLine +
					"    TransferCompletionPercent     : " + info.TransferCompletionPercent.ToString();
			}

			return lResult;
		}

		/// <summary> Updates the baseline progress information based on T2G file transfer notification.</summary>
		/// <param name="notification">The transfer task current notification.</param>
		/// <param name="baselineProgressInfo">[in,out] Information describing the baseline deployment progress.</param>
		private static void UpdateBaselineProgressFromFileTransferNotification(
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

								LogManager.WriteLog(TraceType.ERROR,
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
		private static void UpdateBaselineProgressFromSIFNotification(
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
		private static bool UpdateBaselineProgressStatus(
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
		private static BaselineProgressStatusEnum ValidateBaselineProgressStatus(
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
		private static bool RemoveProgressFromHistoryLogger(string trainId)
		{
			bool lSuccess = false;

			if (trainId != null)
			{

				LogManager.WriteLog(TraceType.INFO,
					"RemoveProgressFromHistoryLogger" + Environment.NewLine +
					"(" + Environment.NewLine +
					"  TrainId : " + trainId + Environment.NewLine +
					")",
					"PIS.Ground.DataPackage.BaselineStatusUpdater.RemoveProgressFromHistoryLogger",
					null,
					EventIdEnum.DataPackage);

				BaselineStatusResponse lResult = new BaselineStatusResponse();
				lResult.ResultCode = LogManager.CleanTrainBaselineStatus(trainId);

				if (lResult.ResultCode == ResultCodeEnum.RequestAccepted)
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
		private static bool UpdateProgressOnHistoryLogger(string trainId, TrainBaselineStatusData progressInfo)
		{
			bool lSuccess = false;

			if (trainId != null && progressInfo != null)
			{
				string lProgressStatusString =
					Enum.GetName(typeof(BaselineProgressStatusEnum), progressInfo.ProgressStatus);

				LogManager.WriteLog(TraceType.INFO,
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

				BaselineStatusResponse lResult = new BaselineStatusResponse();
				lResult.ResultCode = LogManager.UpdateTrainBaselineStatus(
					trainId, progressInfo.RequestId, progressInfo.TaskId, progressInfo.TrainNumber,
					progressInfo.OnlineStatus, progressInfo.ProgressStatus,
					progressInfo.CurrentBaselineVersion, progressInfo.FutureBaselineVersion,
					progressInfo.PisOnBoardVersion);

				if (lResult.ResultCode == ResultCodeEnum.RequestAccepted)
				{
					lSuccess = true;
				}
			}

			return lSuccess;
		}

		/// <summary>Convert a SystemInfo object into a string representation.</summary>
		/// <param name="info">The input information.</param>
		/// <returns>info as a string.</returns>
		private static string ToString(SystemInfo info)
		{
			string lResult = string.Empty;

			if (info != null)
			{
				lResult +=
					"    IsOnline                       : " + info.IsOnline.ToString() + Environment.NewLine +
					"    MissionId                      : " + (info.MissionId ?? "null") + Environment.NewLine;

				if (info.PisBaseline != null)
				{
					lResult +=
					"    PisBaseline.CurrentVersionOut  : " + (info.PisBaseline.CurrentVersionOut ?? "null") + Environment.NewLine +
					"    PisBaseline.FutureVersionOut   : " + (info.PisBaseline.FutureVersionOut ?? "null") + Environment.NewLine;
				}

				lResult +=
					"    Status                         : " + info.Status + Environment.NewLine +
					"    SystemId                       : " + (info.SystemId ?? "null") + Environment.NewLine +
					"    VehiclePhysicalId              : " + info.VehiclePhysicalId + Environment.NewLine;

				if (info.PisVersion != null)
				{
					lResult +=
					"    PisVersion.VersionPISSoftware  : " + (info.PisVersion.VersionPISSoftware ?? "null") + Environment.NewLine;
				}
			}
			return lResult;
		}

		/// <summary>
		/// Updates baseline deployment information for a specific train based on a system changed notification.
		/// </summary>
		/// <param name="notification">The notification content.</param>
		/// <param name="assignedCurrentBaseline">The assigned current baseline.</param>
		/// <param name="assignedFutureBaseline">The assigned future baseline.</param>
		/// <param name="onBoardFutureBaseline">[in,out] The future baseline loaded on the train</param>
		/// <param name="isDeepUpdate">[in,out] true if T2G client must be used if deployment status cannot be known from the notification.</param>
		/// <param name="currentProgress">Provides current deployment information for that train if available, null otherwise.</param>
		/// <param name="updatedProgress">[out] The updated baseline deployment information.</param>		
		private static void ProcessSystemChangedNotification(
			SystemInfo notification,
			string assignedCurrentBaseline,
			string assignedFutureBaseline,
			ref string onBoardFutureBaseline,
			ref bool isDeepUpdate,
			TrainBaselineStatusData currentProgress,
			out TrainBaselineStatusData updatedProgress)
		{

			updatedProgress = null;

			/// Preparing an object for receiving the updated deployment information

			if (currentProgress != null)
			{
				updatedProgress = currentProgress.Clone();
			}
			else
			{
				updatedProgress = new TrainBaselineStatusData();
			}

			/// Updating the progress information based on notification information
			/// 

			updatedProgress.OnlineStatus = notification.IsOnline;

			if (notification.PisVersion != null)
			{
				if (!string.IsNullOrEmpty(notification.PisVersion.VersionPISSoftware))
				{
					updatedProgress.PisOnBoardVersion = notification.PisVersion.VersionPISSoftware;
				}
			}

			updatedProgress.TrainNumber = notification.VehiclePhysicalId.ToString();

			//
			// Update of: progress status, current and future baselines versions
			//

			if (notification.PisBaseline != null)
			{
				string lNotificationCurrentVersion = notification.PisBaseline.CurrentVersionOut;
				string lNotificationFutureVersion = notification.PisBaseline.FutureVersionOut;

				if (currentProgress != null && lNotificationCurrentVersion != null &&
					lNotificationFutureVersion != null)
				{
					if (lNotificationCurrentVersion == string.Empty)
					{
						lNotificationCurrentVersion = NoBaselineVersion;
					}

					if (lNotificationFutureVersion == string.Empty)
					{
						lNotificationFutureVersion = NoBaselineVersion;
					}

					switch (currentProgress.ProgressStatus)
					{
						case BaselineProgressStatusEnum.UPDATED:

							if (currentProgress.CurrentBaselineVersion != lNotificationCurrentVersion)
							{
								// The current baseline changed unexpectedly

								updatedProgress.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
								updatedProgress.RequestId = Guid.Empty;
								updatedProgress.TaskId = 0;
								updatedProgress.CurrentBaselineVersion = lNotificationCurrentVersion;
							}

							updatedProgress.FutureBaselineVersion = lNotificationFutureVersion;
							onBoardFutureBaseline = lNotificationFutureVersion;
							isDeepUpdate = false;
							break;

						case BaselineProgressStatusEnum.DEPLOYED:

							if (currentProgress.FutureBaselineVersion == lNotificationCurrentVersion)
							{
								// The future baseline has been promoted to current

								updatedProgress.ProgressStatus = BaselineProgressStatusEnum.UPDATED;
							}
							else if (currentProgress.FutureBaselineVersion != lNotificationFutureVersion)
							{
								// The future baseline changed unexpectedly

								updatedProgress.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
								updatedProgress.RequestId = Guid.Empty;
								updatedProgress.TaskId = 0;
							}
							else
							{
								// Nothing special occurred
							}

							updatedProgress.CurrentBaselineVersion = lNotificationCurrentVersion;
							updatedProgress.FutureBaselineVersion = lNotificationFutureVersion;
							onBoardFutureBaseline = lNotificationFutureVersion;
							isDeepUpdate = false;
							break;

						case BaselineProgressStatusEnum.TRANSFER_COMPLETED:
						case BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS:
						case BaselineProgressStatusEnum.TRANSFER_PAUSED:
						case BaselineProgressStatusEnum.TRANSFER_PLANNED:

							if (currentProgress.FutureBaselineVersion == lNotificationCurrentVersion)
							{
								// The assigned baseline has been promoted to current baseline
								// Note: In the current state, the future baseline is set to the assigned baseline.

								updatedProgress.ProgressStatus = BaselineProgressStatusEnum.UPDATED;
								updatedProgress.FutureBaselineVersion = lNotificationFutureVersion;
								onBoardFutureBaseline = lNotificationFutureVersion;
								isDeepUpdate = false;
							}
							else if (currentProgress.FutureBaselineVersion == lNotificationFutureVersion)
							{
								// The assigned baseline has been promoted to future baseline
								// Note: In the current state, the future baseline is set to the assigned baseline.

								updatedProgress.ProgressStatus = BaselineProgressStatusEnum.DEPLOYED;
								isDeepUpdate = false;
							}
							else
							{
								assignedFutureBaseline = currentProgress.FutureBaselineVersion;
							}

							// Do not update the future baseline since in the current state,
							// it is set to the assigned baseline (not the on board baseline)
							updatedProgress.CurrentBaselineVersion = lNotificationCurrentVersion;

							break;

						case BaselineProgressStatusEnum.UNKNOWN:

							if (currentProgress.RequestId != Guid.Empty)
							{
								string lAssignedBaseline = assignedFutureBaseline;

								if (string.IsNullOrEmpty(lAssignedBaseline) || lAssignedBaseline == NoBaselineVersion)
								{
									lAssignedBaseline = assignedCurrentBaseline;
								}

								if (!string.IsNullOrEmpty(lAssignedBaseline) && lAssignedBaseline != NoBaselineVersion)
								{
									if (lAssignedBaseline == lNotificationCurrentVersion)
									{
										// The assigned baseline has been promoted to current baseline

										updatedProgress.ProgressStatus = BaselineProgressStatusEnum.UPDATED;
										isDeepUpdate = false;
									}
									else if (lAssignedBaseline == lNotificationFutureVersion)
									{
										// The assigned baseline has been promoted to future baseline
										// Note: In the current state, the future baseline is set to the assigned baseline.

										updatedProgress.ProgressStatus = BaselineProgressStatusEnum.DEPLOYED;
										isDeepUpdate = false;
									}
									else
									{
										// Nothing to do
									}
								}
							}
							updatedProgress.CurrentBaselineVersion = lNotificationCurrentVersion;
							updatedProgress.FutureBaselineVersion = lNotificationFutureVersion;
							onBoardFutureBaseline = lNotificationFutureVersion;

							break;
					}
				}

				///
				/// Additional work is needed...
				///

				if (isDeepUpdate)
				{
					// Ask T2G if a transfer is targeting that train
					//
					if (updatedProgress.RequestId != Guid.Empty)
					{
						LogManager.WriteLog(TraceType.INFO,
							"Calling GetTransferTask() with RequestId == " + updatedProgress.RequestId.ToString(),
							"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessDistributeBaselineRequest",
							null,
							EventIdEnum.DataPackage);

						List<Recipient> lRecipients;
						TransferTaskData lTask;
						string lT2GError = _t2g.GetTransferTask(updatedProgress.TaskId, out lRecipients, out lTask);

						if (string.IsNullOrEmpty(lT2GError))
						{
							if (lTask != null)
							{

								LogManager.WriteLog(TraceType.INFO,
									"GetTransferTask" + Environment.NewLine +
									"(" + Environment.NewLine +
									"    TaskId                        : " + lTask.TaskId + Environment.NewLine +
									"    TaskStatus                    : " + Enum.GetName(typeof(TaskState), lTask.TaskState) + Environment.NewLine +
									"    CurrentTaskPhase              : " + Enum.GetName(typeof(TaskPhase), lTask.TaskPhase) + Environment.NewLine +
									"    AcquisitionCompletionPercent  : " + lTask.AcquisitionCompletionPercent.ToString() + Environment.NewLine +
									"    DistributionCompletionPercent : " + lTask.DistributionCompletionPercent.ToString() + Environment.NewLine +
									"    TransferCompletionPercent     : " + lTask.TransferCompletionPercent.ToString() + Environment.NewLine +
									")",
									"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessDistributeBaselineRequest",
									null,
									EventIdEnum.DataPackage);

								BaselineProgressStatusEnum lProgress = updatedProgress.ProgressStatus;

								var lUpdatedExtendedProgress = new TrainBaselineStatusExtendedData(updatedProgress,
									assignedFutureBaseline, onBoardFutureBaseline, isDeepUpdate);

								var lFileDistributionStatus = new FileDistributionStatusArgs();
								lFileDistributionStatus.AcquisitionCompletionPercent = lTask.AcquisitionCompletionPercent;
								lFileDistributionStatus.CurrentTaskPhase = lTask.TaskPhase;
								lFileDistributionStatus.DistributionCompletionPercent = lTask.DistributionCompletionPercent;
								lFileDistributionStatus.TransferCompletionPercent = lTask.TransferCompletionPercent;
								lFileDistributionStatus.TaskStatus = lTask.TaskState;

								UpdateBaselineProgressFromFileTransferNotification(lFileDistributionStatus, ref lUpdatedExtendedProgress);

								onBoardFutureBaseline = lUpdatedExtendedProgress.OnBoardFutureBaseline;
								isDeepUpdate = false;
							}
							else
							{
								LogManager.WriteLog(TraceType.ERROR, "ProcessSystemChangedNotification() - T2G Error - lTask is null",
									"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSystemChangedNotification",
									null, EventIdEnum.DataPackage);
							}
						}
						else
						{
							LogManager.WriteLog(TraceType.ERROR, "ProcessSystemChangedNotification() - T2G Error - " + lT2GError,
								"PIS.Ground.DataPackage.BaselineStatusUpdater.ProcessSystemChangedNotification",
								null, EventIdEnum.DataPackage);

                            updatedProgress.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
                            updatedProgress.RequestId = Guid.Empty;
                            updatedProgress.TaskId = 0;
						}
					}
				}
			}
		}
	}
}