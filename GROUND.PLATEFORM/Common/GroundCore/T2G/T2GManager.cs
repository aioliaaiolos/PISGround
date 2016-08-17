//---------------------------------------------------------------------------------------------------
// <copyright file="T2GManager.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.T2G
{
	using System;
	using System.Collections.Generic;
	using PIS.Ground.Core.LogMgmt;
	using PIS.Ground.Core.Data;
	using PIS.Ground.Core.Utility;

	/// <summary>T2G server communication manager.</summary>
	internal class T2GManager : IT2GManager, IT2GNotifierTarget, IT2GConnectionListener
	{
		#region Properties

		/// <summary>Gets the T2G notification processor.</summary>
		/// <value>The T2G notification processor.</value>
		public IT2GNotificationProcessor T2GNotifier
		{
			get
			{
				return _notifier;
			}
		}

		/// <summary>Gets the T2G file distribution manager.</summary>
		/// <value>The T2G file distribution manager.</value>
		public IT2GFileDistributionManager T2GFileDistributionManager
		{
			get
			{
				return _fileDistributionManager;
			}
		}

		/// <summary>Gets a value indicating whether T2G Server Connection status value.</summary>
		/// <value>The T2G server connection status.</value>
		public bool T2GServerConnectionStatus
		{
			get
			{
				return _connectionManager.T2GServerConnectionStatus;
			}
		}

		#endregion

		#region Private fields

		/// <summary>The local data storage.</summary>
		private T2GLocalDataStorage _localDataStorage;

		/// <summary>The notifier.</summary>
		private IT2GNotificationProcessor _notifier;

		/// <summary>Manager for file distribution.</summary>
		private T2GFileDistributionManager _fileDistributionManager;

		/// <summary>Manager for connection.</summary>
		private IT2GConnectionManager _connectionManager;

		/// <summary>Information describing the session.</summary>
		private T2GSessionData _sessionData;

		/// <summary>The subscriber lock for all handler containers.</summary>
		private object _subscriberLock = new object();

		/// <summary>The list of system changed event handler.</summary>
		private Dictionary<string, EventHandler<SystemChangedNotificationArgs>> _systemChangedEventHandlers;

		/// <summary>The list of system deleted event handler.</summary>
		private Dictionary<string, EventHandler<SystemDeletedNotificationArgs>> _systemDeletedEventHandlers;

		/// <summary>The list of T2G online/offline event handler.</summary>
		private Dictionary<string, EventHandler<T2GOnlineStatusNotificationArgs>> _T2GOnlineOfflineEventHandlers;

		/// <summary>The element event handler.</summary>
		private Dictionary<string, EventHandler<ElementEventArgs>> _elementEventHandlers;

		/// <summary>The file publication notification event handlers.</summary>        
		private Dictionary<string, EventHandler<FilePublicationNotificationArgs>> _filePublicationNotificationEventHandlers;

		/// <summary>The file published notification event handlers.</summary>
		private Dictionary<string, EventHandler<FilePublishedNotificationArgs>> _filePublishedNotificationEventHandlers;

		/// <summary>The file received notification event handlers.</summary>
		private Dictionary<string, EventHandler<FileReceivedArgs>> _fileReceivedNotificationEventHandlers;

		/// <summary>The file distribution event handlers.</summary>
		private Dictionary<string, EventHandler<FileDistributionStatusArgs>> _fileDistributionEventHandlers;

		#endregion

		#region Constructor

		/// <summary>Initializes a new instance of the T2GManager class.</summary>
		internal T2GManager()
		{
			_systemChangedEventHandlers = new Dictionary<string, EventHandler<SystemChangedNotificationArgs>>();
			_systemDeletedEventHandlers = new Dictionary<string, EventHandler<SystemDeletedNotificationArgs>>();
			_T2GOnlineOfflineEventHandlers = new Dictionary<string, EventHandler<T2GOnlineStatusNotificationArgs>>();
			_elementEventHandlers = new Dictionary<string, EventHandler<ElementEventArgs>>();
			_filePublicationNotificationEventHandlers = new Dictionary<string, EventHandler<FilePublicationNotificationArgs>>();
			_filePublishedNotificationEventHandlers = new Dictionary<string, EventHandler<FilePublishedNotificationArgs>>();
			_fileReceivedNotificationEventHandlers = new Dictionary<string, EventHandler<FileReceivedArgs>>();
			_fileDistributionEventHandlers = new Dictionary<string, EventHandler<FileDistributionStatusArgs>>();

			_sessionData = new T2GSessionData();
			_localDataStorage = new T2GLocalDataStorage(_sessionData, null);

			if (!string.IsNullOrEmpty(ServiceConfiguration.T2GServiceNotificationUrl))
			{
				_fileDistributionManager = new T2GFileDistributionManager(_sessionData, this);
			}

			_notifier = new T2GNotificationProcessor(this, _localDataStorage, _fileDistributionManager);
			_connectionManager = new T2GConnectionManager(_sessionData, this);
		}

		/// <summary>Initializes a new instance of the T2GManager class. This constructor is used for unit tests.</summary>
		/// <param name="sessionData">Information describing the session.</param>
		/// <param name="localDataStorage">The local data storage.</param>
		/// <param name="fileDistributionManager">Manager for file distribution.</param>
		/// <param name="notifier">The notifier.</param>
		/// <param name="connectionManager">Manager for connection.</param>
		internal T2GManager(T2GSessionData sessionData, T2GLocalDataStorage localDataStorage, T2GFileDistributionManager fileDistributionManager, IT2GNotificationProcessor notifier, IT2GConnectionManager connectionManager)
		{
			_systemChangedEventHandlers = new Dictionary<string, EventHandler<SystemChangedNotificationArgs>>();
			_systemDeletedEventHandlers = new Dictionary<string, EventHandler<SystemDeletedNotificationArgs>>();
			_T2GOnlineOfflineEventHandlers = new Dictionary<string, EventHandler<T2GOnlineStatusNotificationArgs>>();
			_elementEventHandlers = new Dictionary<string, EventHandler<ElementEventArgs>>();
			_filePublicationNotificationEventHandlers = new Dictionary<string, EventHandler<FilePublicationNotificationArgs>>();
			_filePublishedNotificationEventHandlers = new Dictionary<string, EventHandler<FilePublishedNotificationArgs>>();
			_fileReceivedNotificationEventHandlers = new Dictionary<string, EventHandler<FileReceivedArgs>>();
			_fileDistributionEventHandlers = new Dictionary<string, EventHandler<FileDistributionStatusArgs>>();

			_sessionData = sessionData;
			_localDataStorage = localDataStorage;
			_fileDistributionManager = fileDistributionManager;
			_notifier = notifier;
			_connectionManager = connectionManager;
		}

		#endregion

		public void OnConnectionStatusChanged(bool connected)
		{
			if (!string.IsNullOrEmpty(ServiceConfiguration.T2GServiceNotificationUrl))
			{
				if (connected)
				{
				    _localDataStorage.InitializeLocalStorageData();
					if (_fileDistributionManager != null)
                        _fileDistributionManager.Initialize();
				}
				else
				{
					_localDataStorage.DeinitializeLocalStorageData();
                    if (_fileDistributionManager != null)
					    _fileDistributionManager.Deinitialize();
				}
			}

			RaiseOnT2GOnlineStatusNotificationEvent(new T2GOnlineStatusNotificationArgs { online = connected });
		}

		/// <summary>Raises OnFileDistributeNotificationEvent.</summary>
		/// <param name="eventArgs">FileDistributionStatusArgs object.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public void RaiseOnFileDistributeNotificationEvent(FileDistributionStatusArgs eventArgs)
		{
			lock (_subscriberLock)
			{
				NotifyEventHandlersAsync(eventArgs, _fileDistributionEventHandlers);
			}
		}

		/// <summary>Raises OnFileDistributeNotification.</summary>
		/// <param name="eventArgs">FileDistributionStatusArgs object.</param>
		/// <param name="taskId">task id.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public void RaiseOnFileDistributeNotificationEvent(FileDistributionStatusArgs eventArgs, int taskId)
		{
			// (1) Per taskId processing

            if (taskId > 0 && eventArgs != null && eventArgs.RequestId != Guid.Empty && _fileDistributionManager != null)
            {
                EventHandler<FileDistributionStatusArgs> handler = _fileDistributionManager.GetFileDistributionEventByTaskId(taskId);

                lock (_subscriberLock)
                {
                    NotifyEventHandlerAsync(eventArgs, handler);
                }

                // download the file from T2G ground server (asynchronously)
                if (eventArgs.DistributionCompletionPercent == 100)
                {
                    Func<int, bool> invoker = _fileDistributionManager.DownloadFile;
                    invoker.BeginInvoke(taskId, result => invoker.EndInvoke(result), null);
                }
            }

			// (2) General processing not specific to a particular request

			RaiseOnFileDistributeNotificationEvent(eventArgs);
		}

		/// <summary>Raises OnFileDistributeNotification.</summary>
		/// <param name="eventArgs">FileDistributionStatusArgs object.</param>
		/// <param name="RequestId">Request id.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public void RaiseOnFileDistributeNotificationEvent(FileDistributionStatusArgs eventArgs, Guid RequestId)
		{
			if (RequestId != Guid.Empty && _fileDistributionManager != null)
			{
				EventHandler<FileDistributionStatusArgs> handler = _fileDistributionManager.GetFileDistributionEventByRequestId(RequestId);

				lock (_subscriberLock)
				{
					NotifyEventHandlerAsync(eventArgs, handler);
				}
			}
		}

		/// <summary>Raises OnFilePublicationNotificationEvent.</summary>
		/// <param name="eventArgs">FilePublicationNotificationArgs object.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public void RaiseOnFilePublicationNotificationEvent(FilePublicationNotificationArgs eventArgs)
		{
			lock (_subscriberLock)
			{
				NotifyEventHandlersAsync(eventArgs, _filePublicationNotificationEventHandlers);
			}
		}

		/// <summary>Raises OnFilePublishedNotification.</summary>
		/// <param name="eventArgs">FilePublishedNotificationArgs object.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public void RaiseOnFilePublishedNotificationEvent(FilePublishedNotificationArgs eventArgs)
		{
			lock (_subscriberLock)
			{
				NotifyEventHandlersAsync(eventArgs, _filePublishedNotificationEventHandlers);
			}
		}

		/// <summary>Raises OnElementInfoChangeEvent.</summary>
		/// <param name="eventArgs">ElementEventArgs object.</param>
		/// <returns>True if input parameter is not null, false otherwise.</returns>
		public void RaiseOnElementInfoChangeEvent(ElementEventArgs eventArgs)
		{
			lock (_subscriberLock)
			{
				NotifyEventHandlersAsync(eventArgs, _elementEventHandlers);
			}
		}

		/// <summary>Raised when files after downloaded is finished.</summary>
		/// <param name="eventArgs">file recieved arguments.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public void RaiseOnFileReceivedNotificationEvent(FileReceivedArgs eventArgs)
		{
			lock (_subscriberLock)
			{
				NotifyEventHandlersAsync(eventArgs, _fileReceivedNotificationEventHandlers);
			}
		}

		/// <summary>Raises the on system deleted notification event.</summary>
		/// <param name="pEvent">The notification event.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public void RaiseOnSystemDeletedNotificationEvent(SystemDeletedNotificationArgs eventArgs)
		{
			lock (_subscriberLock)
			{
				NotifyEventHandlersAsync(eventArgs, _systemDeletedEventHandlers);
			}
		}

		/// <summary>Raises an event asynchronously.</summary>
		/// <typeparam name="T">Generic type parameter.</typeparam>
		/// <param name="eventArgs">Event arguments object.</param>
		/// <param name="handlers">The handlers.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public void NotifyEventHandlersAsync<T>(T eventArgs, Dictionary<string, EventHandler<T>> handlers) where T : EventArgs
		{
			foreach (var handler in handlers.Values)
			{
				NotifyEventHandlerAsync(eventArgs, handler);
			}
		}

		/// <summary>Notifies an event handler asynchronously.</summary>
		/// <typeparam name="T">Generic event arguments type parameter.</typeparam>
		/// <param name="eventArgs">Event arguments object.</param>
		/// <param name="handler">The handler.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public void NotifyEventHandlerAsync<T>(T eventArgs, EventHandler<T> handler) where T : EventArgs
		{
			if (handler != null && eventArgs != null)
			{
				handler.BeginInvoke(this, eventArgs, handler.EndInvoke, null);
				//handler(this, eventArgs);
			}
		}

		/// <summary>Subscribe the file received notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		/// <param name="handler">event handler.</param>
		public void SubscribeToFileReceivedNotification(string subscriberId, EventHandler<FileReceivedArgs> handler)
		{
			SubscribeToNotification(subscriberId, handler, _fileReceivedNotificationEventHandlers);
		}

		/// <summary>Unsubscribe from file received notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		public void UnsubscribeFromFileReceivedNotification(string subscriberId)
		{
			UnsubscribeFromNotification(subscriberId, _fileReceivedNotificationEventHandlers);
		}

		/// <summary>Subscribe the file distribution notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		/// <param name="eventHandler">event handler.</param>
		public void SubscribeToFileDistributionNotifications(string subscriberId, EventHandler<FileDistributionStatusArgs> eventHandler)
		{
			SubscribeToNotification(subscriberId, eventHandler, _fileDistributionEventHandlers);
		}

		/// <summary>Unsubscribe from file distribution notifications.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		public void UnsubscribeFromFileDistributionNotifications(string subscriberId)
		{
			UnsubscribeFromNotification(subscriberId, _fileDistributionEventHandlers);
		}

		/// <summary>Subscribe the file published notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		/// <param name="eventHandler">event handler.</param>
		public void SubscribeToFilePublishedNotification(string subscriberId, EventHandler<FilePublishedNotificationArgs> eventHandler)
		{
			SubscribeToNotification(subscriberId, eventHandler, _filePublishedNotificationEventHandlers);
		}

		/// <summary>Unsubscribe from file published notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		public void UnsubscribeFromFilePublishedNotification(string subscriberId)
		{
			UnsubscribeFromNotification(subscriberId, _filePublishedNotificationEventHandlers);
		}

		/// <summary>Subscribe the file publication notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		/// <param name="eventHandler">event handler.</param>
		public void SubscribeToFilePublicationNotification(string subscriberId, EventHandler<FilePublicationNotificationArgs> eventHandler)
		{
			SubscribeToNotification(subscriberId, eventHandler, _filePublicationNotificationEventHandlers);
		}

		/// <summary>Unsubscribe from file publication notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		public void UnsubscribeFromFilePublicationNotification(string subscriberId)
		{
			UnsubscribeFromNotification(subscriberId, _filePublicationNotificationEventHandlers);
		}

		/// <summary>Subscribe to the system deleted notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		/// <param name="handler">The event handler.</param>
		public void SubscribeToSystemDeletedNotification(string subscriberId, EventHandler<PIS.Ground.Core.Data.SystemDeletedNotificationArgs> handler)
		{
			SubscribeToNotification(subscriberId, handler, _systemDeletedEventHandlers);
		}

		/// <summary>Unsubscribe to the system deleted notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		public void UnsubscribeFromSystemDeletedNotification(string subscriberId)
		{
			UnsubscribeFromNotification(subscriberId, _systemDeletedEventHandlers);
		}

		/// <summary>Subscribe to T2G connection notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		/// <param name="eventHandler">The event handler.</param>
		public void SubscribeToT2GOnlineStatusNotification(string subscriberId, EventHandler<PIS.Ground.Core.Data.T2GOnlineStatusNotificationArgs> eventHandler)
		{
			SubscribeToT2GOnlineStatusNotification(subscriberId, eventHandler, false);
		}

		/// <summary>Subscribe to T2G connection notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		/// <param name="handler">The event handler.</param>
		/// <param name="notifyImmediately">If true, call the notification callback immediately.</param>
		public void SubscribeToT2GOnlineStatusNotification(string subscriberId,
			EventHandler<PIS.Ground.Core.Data.T2GOnlineStatusNotificationArgs> handler,
			bool notifyImmediately)
		{
			SubscribeToNotification(subscriberId, handler, _T2GOnlineOfflineEventHandlers);

			if (notifyImmediately)
			{
				NotifyEventHandlerAsync(new T2GOnlineStatusNotificationArgs { online = T2GServerConnectionStatus }, handler);
			}
		}

		/// <summary>Subscribe to a notification.</summary>
		/// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
		/// illegal values.</exception>
		/// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
		/// <typeparam name="T">Generic event argument type parameter.</typeparam>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		/// <param name="handler">new event handler.</param>
		/// <param name="handlers">existing event handlers.</param>
		public void SubscribeToNotification<T>(string subscriberId, EventHandler<T> handler, Dictionary<string, EventHandler<T>> handlers) where T : EventArgs
		{
			if (string.IsNullOrEmpty(subscriberId))
			{
				throw new ArgumentNullException("subscriberId");
			}

			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}

			lock (_subscriberLock)
			{
				if (handlers.ContainsKey(subscriberId))
				{
					handlers[subscriberId] = handler;
				}
				else
				{
					handlers.Add(subscriberId, handler);
				}
			}
		}

		/// <summary>Unsubscribe from notification.</summary>
		/// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
		/// <typeparam name="T">Generic event argument type parameter.</typeparam>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		/// <param name="handlers">existing event handlers.</param>
		public void UnsubscribeFromNotification<T>(string subscriberId, Dictionary<string, EventHandler<T>> handlers) where T : EventArgs
		{
			if (string.IsNullOrEmpty(subscriberId))
			{
				throw new ArgumentNullException("subscriberId");
			}

			lock (_subscriberLock)
			{
				if (handlers.ContainsKey(subscriberId))
				{
					handlers.Remove(subscriberId);
				}
			}
		}

		/// <summary>Sends a T2G online / offline notification event to local subscriber(s).</summary>
		/// <param name="pEvent">The notification event.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		internal void RaiseOnT2GOnlineStatusNotificationEvent(T2GOnlineStatusNotificationArgs eventArgs)
		{
			NotifyEventHandlersAsync(eventArgs, _T2GOnlineOfflineEventHandlers);
		}

		/// <summary>Unsubscribe to T2G connection notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		public void UnsubscribeFromT2GOnlineStatusNotification(string subscriberId)
		{
			UnsubscribeFromNotification(subscriberId, _T2GOnlineOfflineEventHandlers);
		}

		/// <summary>Subscribe the Element change notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		/// <param name="handler">event handler.</param>
		public void SubscribeToElementChangeNotification(string subscriberId, EventHandler<ElementEventArgs> handler)
		{
			SubscribeToNotification(subscriberId, handler, _elementEventHandlers);
		}

		/// <summary>Unsubscribe to the element change notification.</summary>
		/// <param name="subscriberId">Unique subscriber identifier of the event.</param>
		public void UnsubscribeFromElementChangeNotification(string subscriberId)
		{
			UnsubscribeFromNotification(subscriberId, _elementEventHandlers);
		}

		/// <summary>Get all the list of AvailableElements.</summary>
		/// <param name="elementDataList">[out] Information describing the list available element.</param>
		/// <returns>ElementList of AvailableElementData.</returns>
		public T2GManagerErrorEnum GetAvailableElementDataList(out ElementList<AvailableElementData> elementDataList)
		{
			LogManager.WriteLog(TraceType.INFO, "GetAvailableElementDataList called", "PIS.Ground.Core.T2G.T2GClient.GetAvailableElementDataList", null, EventIdEnum.GroundCore);

			elementDataList = new ElementList<AvailableElementData>(); // always return an object

			T2GManagerErrorEnum result = T2GManagerErrorEnum.eFailed;

			if (T2GServerConnectionStatus)
			{
				elementDataList = _localDataStorage.GetAvailableElementDataList();

				result = T2GManagerErrorEnum.eSuccess;
			}
			else
			{
				result = T2GManagerErrorEnum.eT2GServerOffline;
			}

			return result;
		}

		/// <summary>
		/// Search AvailableElementData By TargetAddress (By Element, MissionOperatorCode, or MissionId)
		/// </summary>
		/// <param name="targetAddress">Target address.</param>
		/// <param name="lstAvailableElementData">[out] Information describing the list available element.</param>
		/// <returns>List of AvailableElementData matching TargetAddress criteria.</returns>
		public T2GManagerErrorEnum GetAvailableElementDataByTargetAddress(TargetAddressType targetAddress, out ElementList<AvailableElementData> lstAvailableElementData)
		{
			T2GManagerErrorEnum lReturn = T2GManagerErrorEnum.eFailed;
			lstAvailableElementData = new ElementList<AvailableElementData>();

			if (targetAddress != null && !string.IsNullOrEmpty(targetAddress.Id))
			{
				// Resolve target elements
				switch (targetAddress.Type)
				{
					case AddressTypeEnum.Element:
						AvailableElementData element = new AvailableElementData();
						lReturn = GetAvailableElementDataByElementNumber(targetAddress.Id, out element);
						if (lReturn == T2GManagerErrorEnum.eSuccess)
						{
							lstAvailableElementData.Add(element);
						}
						break;

					case AddressTypeEnum.MissionOperatorCode:
						lReturn = GetAvailableElementDataListByMissionOperatorCode(targetAddress.Id, out lstAvailableElementData);
						break;

					case AddressTypeEnum.MissionCode:
						lReturn = GetAvailableElementDataListByMissionCode(targetAddress.Id, out lstAvailableElementData);
						break;
				}
			}

			return lReturn;
		}

		/// <summary>Search AvailableElementData By Mission CommercialNumber.</summary>
		/// <param name="commercialNumber">Mission Commercial Number.</param>
		/// <param name="elementDataList">[out] Information describing the list available element.</param>
		/// <returns>The available element data list by mission code.</returns>        
		public T2GManagerErrorEnum GetAvailableElementDataListByMissionCode(string commercialNumber, out ElementList<AvailableElementData> elementDataList)
		{
			LogManager.WriteLog(TraceType.INFO, "GetAvailableElementDataListByMissionOperatorCode called", "PIS.Ground.Core.T2G.T2GClient.GetAvailableElementDataByElementNumber", null, EventIdEnum.GroundCore);

			elementDataList = new ElementList<AvailableElementData>(); // always return an object

			T2GManagerErrorEnum result = T2GManagerErrorEnum.eFailed;

			if (T2GServerConnectionStatus)
			{
				elementDataList = _localDataStorage.GetAvailableElementDataListByMissionCode(commercialNumber);

				if (elementDataList.Count > 0)
				{
					result = T2GManagerErrorEnum.eSuccess;
				}
				else
				{
					result = T2GManagerErrorEnum.eElementNotFound;
				}
			}
			else
			{
				result = T2GManagerErrorEnum.eT2GServerOffline;
			}

			return result;
		}

		/// <summary>Search AvailableElementData By MissionOperatorCode.</summary>
		/// <param name="missionOperatorCode">Mission Operator code.</param>
		/// <param name="elementDataList">[out] Information describing the list available element.</param>
		/// <returns>ElementList of AvailableElementData.</returns>
		public T2GManagerErrorEnum GetAvailableElementDataListByMissionOperatorCode(string missionOperatorCode, out ElementList<AvailableElementData> elementDataList)
		{
			LogManager.WriteLog(TraceType.INFO, "GetAvailableElementDataListByMissionOperatorCode called", "PIS.Ground.Core.T2G.T2GClient.GetAvailableElementDataByElementNumber", null, EventIdEnum.GroundCore);

			elementDataList = new ElementList<AvailableElementData>(); // always return an object

			T2GManagerErrorEnum result = T2GManagerErrorEnum.eFailed;

			if (T2GServerConnectionStatus)
			{
				elementDataList = _localDataStorage.GetAvailableElementDataListByMissionOperatorCode(missionOperatorCode);

				if (elementDataList.Count > 0)
				{
					result = T2GManagerErrorEnum.eSuccess;
				}
				else
				{
					result = T2GManagerErrorEnum.eElementNotFound;
				}
			}
			else
			{
				result = T2GManagerErrorEnum.eT2GServerOffline;
			}

			return result;
		}

		/// <summary>Search AvailableElementData By ElementNumber.</summary>
		/// <param name="elementNumber">element number.</param>
		/// <param name="objAvailableElementData">[out] Information describing the object available element.</param>
		/// <returns>if found return AvailableElementData else null.</returns>
		public T2GManagerErrorEnum GetAvailableElementDataByElementNumber(string elementNumber, out AvailableElementData objAvailableElementData)
		{
			LogManager.WriteLog(TraceType.INFO, "GetAvailableElementDataByElementNumber called", "PIS.Ground.Core.T2G.T2GClient.GetAvailableElementDataByElementNumber", null, EventIdEnum.GroundCore);

			objAvailableElementData = new AvailableElementData(); // always return an object

			T2GManagerErrorEnum result = T2GManagerErrorEnum.eFailed;

			if (T2GServerConnectionStatus)
			{
				AvailableElementData elementData = _localDataStorage.GetAvailableElementDataByElementNumber(elementNumber);

				if (elementData != null)
				{
					objAvailableElementData = elementData; // copy reference

					result = T2GManagerErrorEnum.eSuccess;
				}
				else
				{
					result = T2GManagerErrorEnum.eElementNotFound;
				}
			}
			else
			{
				result = T2GManagerErrorEnum.eT2GServerOffline;
			}

			return result;
		}

        /// <summary>Verify if a system is online or not.</summary>
        /// <param name="elementNumber">The system identifier to query.</param>
        /// <param name="IsOnline">[out] Online status on the train. In case of error, value is forced to false.</param>
        /// <returns>The success of the operation. Possible values are:
        /// <list type="table">
        /// <listheader><term>Error code</term><description>Description</description></listheader>
        /// <item><term>T2GManagerErrorEnum.eSuccess</term><description>Service retrieved successfully and it is available</description></item>
        /// <item><term>T2GManagerErrorEnum.eElementNotFound</term><description>Queried system is unknown.</description></item>
        /// <item><term>T2GManagerErrorEnum.eT2GServerOffline</term><description>T2G services are down.</description></item>
        /// </list>
        /// </returns>
        public T2GManagerErrorEnum IsElementOnline(string elementNumber, out bool isOnline)
		{
			T2GManagerErrorEnum lReturn = T2GManagerErrorEnum.eFailed;

			isOnline = false;

			if (T2GServerConnectionStatus)
			{
                // Get connection status of element first to improve speed when system is online.
                // Then, check if the element exist.
                isOnline = _localDataStorage.IsElementOnline(elementNumber);
                if (isOnline || _localDataStorage.ElementExists(elementNumber))
                {
                    lReturn = T2GManagerErrorEnum.eSuccess;
                }
				else
				{
					lReturn = T2GManagerErrorEnum.eElementNotFound;
				}

			}
			else
			{
				lReturn = T2GManagerErrorEnum.eT2GServerOffline;
			}

			return lReturn;
		}

		/// <summary>
		/// Query if 'elementNumber' is online and pis baseline information is up to date.
		/// </summary>
		/// <param name="elementNumber">The element number.</param>
		/// <returns>true if element is online and pis baseline information is up to date, false if not.</returns>
		public bool IsElementOnlineAndPisBaselineUpToDate(string elementNumber)
		{
			SystemInfo lSystemInfo;
			bool lIsOnlineAndUpToDate = T2GServerConnectionStatus
							&& (lSystemInfo = _localDataStorage.GetSystem(elementNumber)) != null
							&& lSystemInfo.IsOnline
							&& lSystemInfo.IsPisBaselineUpToDate;

			return lIsOnlineAndUpToDate;
		}

		/// <summary>Get the information on a specific service on a specific train. Service shall be available.</summary>
        /// <param name="systemId">The system identifier to retrieve the information.</param>
        /// <param name="serviceId">The service identifier to retrieve information on</param>
        /// <param name="serviceDataResult">[out] Information on the service retrieve. It's never null.</param>
		/// <returns>The success of the operation. Possible values are:
        /// <list type="table">
        /// <listheader><term>Error code</term><description>Description</description></listheader>
        /// <item><term>T2GManagerErrorEnum.eSuccess</term><description>Service retrieved successfully and it is available</description></item>
        /// <item><term>T2GManagerErrorEnum.eServiceInfoNotFound</term><description>Service is unknown or it is not available..</description></item>
        /// <item><term>T2GManagerErrorEnum.eElementNotFound</term><description>Queried system is unknown.</description></item>
        /// <item><term>T2GManagerErrorEnum.eT2GServerOffline</term><description>T2G services are down.</description></item>
        /// </list>
        /// </returns>
		public T2GManagerErrorEnum GetAvailableServiceData(string systemId, int serviceId, out ServiceInfo serviceDataResult)
		{
			T2GManagerErrorEnum lReturn = T2GManagerErrorEnum.eFailed;

            serviceDataResult = null;

            if (T2GServerConnectionStatus)
            {
                // By default, assume that system exist to speed up expected behavior
                ServiceInfo service = _localDataStorage.GetAvailableServiceData(systemId, serviceId);
                if (object.ReferenceEquals(service, null))
                {
                    lReturn = _localDataStorage.ElementExists(systemId) ? T2GManagerErrorEnum.eServiceInfoNotFound : T2GManagerErrorEnum.eElementNotFound;
                }
                else if (service.IsAvailable)
                {
                    serviceDataResult = service; // Copy reference, service info immutable

                    lReturn = T2GManagerErrorEnum.eSuccess;

                }
                else
                {
                    lReturn = T2GManagerErrorEnum.eServiceInfoNotFound;
                }
            }
            else
            {
                lReturn = T2GManagerErrorEnum.eT2GServerOffline;
            }

            if (lReturn != T2GManagerErrorEnum.eSuccess)
            {
                // Always return a valid object.
                serviceDataResult = new ServiceInfo();
            }
			return lReturn;
		}
	}
}
