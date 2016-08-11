//---------------------------------------------------------------------------------------------------
// <copyright file="LocalDataStorage.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.T2G
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
    using System.Text.RegularExpressions;
    using PIS.Ground.Core.Data;
	using PIS.Ground.Core.LogMgmt;
	using PIS.Ground.Core.T2G.WebServices.Identification;
	using PIS.Ground.Core.T2G.WebServices.VehicleInfo;
	using PIS.Ground.Core.Common;
    using System.Configuration;
    using PIS.Ground.Core.Utility;
    using System.Globalization;

	/// <summary>Local Data storage for train information.</summary>
	internal class T2GLocalDataStorage
    {
        #region Private Variables

        /// <summary>Service request time out.</summary>
		private const int ServiceRequestTimeOut = 100;

		/// <summary>
		/// List of System which will be filled by calling enumSystems service of Identification.
		/// </summary>
		protected List<SystemInfo> _systemList = new List<SystemInfo>();

		/// <summary>Current system list lock</summary>
		protected object _systemListLock = new object();

		/// <summary>List of message identifiers.</summary>
		private List<string> _messageIdList;

		/// <summary>List of service identifiers.</summary>
		private List<eServiceID> _serviceIdList;

		private T2GSessionData _sessionData;

        /// <summary>Indicates if filtering of local train service is enabled or not.</summary>
        private bool _filterLocalServiceOnly;

        /// <summary>
        /// Dictionary that associate a subscription id with a service id. Key is the subscription id and Value is the associated service id.
        /// </summary>
        private Dictionary<int, ushort> _associationSubscriptionIdWithServiceId = new Dictionary<int, ushort>(20);

        /// <summary>
        /// The lock object to access variable _associationSubscriptionIdWithServiceId.
        /// </summary>
        private object _subscriptionAssociationsLock = new object();
		#endregion

		#region Constructor

		/// <summary>
		/// Prevents a default instance of the LocalDataStorage class from being created.
		/// </summary>
        /// <param name="sessionData">Information to hold T2G session data.</param>
        /// <param name="filterLocalTrainService">Indicates if the filtering on local train service is enabled or not.
        /// If not specified, the value is retrieved from the configuration file.</param>
		internal T2GLocalDataStorage(T2GSessionData sessionData, bool? filterLocalTrainService)
		{
            if (sessionData == null)
            {
                throw new ArgumentNullException("sessionData");
            }

			_sessionData = sessionData;

			_messageIdList = new List<string>(
				new string[] {
                    T2GDataConverter.PisBaseline,
                    T2GDataConverter.PisVersion,
                    T2GDataConverter.PisMission,
                    T2GDataConverter.SivngMission
                });

			_serviceIdList = new List<eServiceID>(
				 new eServiceID[] {
                    eServiceID.eSrvSIF_DataPackageServer,
                    eServiceID.eSrvSIF_InstantMessageServer,
                    eServiceID.eSrvSIF_LiveVideoControlServer,
                    eServiceID.eSrvSIF_MaintenanceServer,
                    eServiceID.eSrvSIF_MissionServer,
                    eServiceID.eSrvSIF_RealTimeServer,
                    eServiceID.eSrvSIF_ReportExchangeServer 
                });

            if (filterLocalTrainService.HasValue)
            {
                _filterLocalServiceOnly = filterLocalTrainService.Value;
            }
            else
            {
                // Value not specified, use value stored into the configuration file
                _filterLocalServiceOnly = ServiceConfiguration.FilterLocalTrainService;
            }
		}

		#endregion

		#region Internal Methods

		/// <summary>Builds element information changed event.</summary>
		/// <param name="systemId">System ID.</param>
		/// <returns>ElementEventArgs object.</returns>
        internal ElementEventArgs BuildElementInfoChangedEvent(string systemId)
        {
            ElementEventArgs objElementEventArgs = null;

            SystemInfo foundSystem = GetSystem(systemId);

            if (foundSystem != null)
            {
                objElementEventArgs = new ElementEventArgs();
                objElementEventArgs.SystemInformation = new SystemInfo(foundSystem); //Deep copy
            }

            return objElementEventArgs;
        }

		/// <summary>Dumps a system list in .</summary>
		/// <param name="traceLevel">The trace level.</param>
		/// <param name="message">The message to print.</param>
		/// <param name="context">The context to print.</param>
		internal void DumpCurrentSystemList(TraceType traceLevel, string message, string context)
		{
			if (LogManager.IsTraceActive(traceLevel))
			{
				StringBuilder log = new StringBuilder();
				log.AppendLine(message);

				lock (_systemListLock)
				{
					foreach (SystemInfo lSys in _systemList)
					{
						log.AppendLine("SYSTEM");
						log.AppendLine("  SystemId		  (" + lSys.SystemId + ")");
						log.AppendLine("  IsOnline		  (" + lSys.IsOnline + ")");
						log.AppendLine("  Status			(" + lSys.Status + ")");
						log.AppendLine("  VehiclePhysicalId (" + lSys.VehiclePhysicalId + ")");

						foreach (ServiceInfo lService in lSys.ServiceList)
						{
							log.AppendLine("  SERVICE");
							log.AppendLine("	ServiceId (" + lService.ServiceId + ")");
							log.AppendLine("	ServiceName (" + lService.ServiceName + ")");
							log.AppendLine("	IsAvailable (" + lService.IsAvailable + ")");
							log.AppendLine("	ServiceIPAddress (" + lService.ServiceIPAddress + ")");
							log.AppendLine("	ServicePortNumber (" + lService.ServicePortNumber + ")");
						}

						log.AppendLine("  BASELINE");
						log.AppendLine("	CurrentVersionOut (" + lSys.PisBaseline.CurrentVersionOut + ")");
						log.AppendLine("	FutureVersionOut  (" + lSys.PisBaseline.FutureVersionOut + ")");
						log.AppendLine("	... ");

						log.AppendLine("  VERSION");
						log.AppendLine("	VersionPISSoftware (" + lSys.PisVersion.VersionPISSoftware + ")");

						log.AppendLine("  MISSION");
						log.AppendLine("	MissionState (" + lSys.PisMission.MissionState + ")");
						log.AppendLine("	OperatorCode (" + lSys.PisMission.OperatorCode + ")");
						log.AppendLine("	CommercialNumber (" + lSys.PisMission.CommercialNumber + ")");
						log.AppendLine("");


					}
				}

				LogManager.WriteLog(traceLevel, log.ToString(), context, null, EventIdEnum.GroundCore);
			}
		}

		/// <summary>To Get the Service Information.</summary>
		/// <param name="strSystemId">System Id.</param>        
		/// <returns>ServiceInfo Data.</returns>
		internal ServiceInfoList GetAvailableServiceData(string systemId)
		{
			ServiceInfoList serviceList = null;

			if (!string.IsNullOrEmpty(systemId))
			{
				lock (_systemListLock)
				{
					SystemInfo system = _systemList.Find(element => element.SystemId == systemId);

					if (system != null)
					{
						// Copy immutable list ref is ok, keep same references
						serviceList = system.ServiceList;
					}
				}
			}

			return serviceList??ServiceInfoList.Empty;
		}

		/// <summary>To Get the Service Information.</summary>
		/// <param name="strSystemId">System Id.</param>
		/// <param name="intServiceId">Service ID.</param>        
		/// <returns>ServiceInfo Data.</returns>
		internal ServiceInfo GetAvailableServiceData(string systemId, int serviceId)
		{
			ServiceInfo service = GetAvailableServiceData(systemId).FirstOrDefault(element => element.ServiceId == serviceId);
			return service;
		}

		/// <summary>Returns a system object.</summary>
		/// <param name="systemID">System id.</param>
		/// <returns>System object or null.</returns>
		internal SystemInfo GetSystem(string systemId)
		{
			SystemInfo systemInfo = null;

			lock (_systemListLock)
			{
				// System info object is immutable. So, there is no need to create a copy.
				systemInfo = _systemList.Find(item => item.SystemId == systemId);
			}

			return systemInfo;
		}

		/// <summary>Returns all system objects by creating a deep copy.</summary>        
		/// <returns>List of all system objects.</returns>
		internal List<SystemInfo> GetSystemList()
		{
			List<SystemInfo> systemInfoList;

			lock (_systemListLock)
			{
				systemInfoList = new List<SystemInfo>(_systemList);
			}

            // No lock required here. Item in the list are immutable.
            for (int i = 0; i < systemInfoList.Count; ++i)
            {
                systemInfoList[i] = new SystemInfo(systemInfoList[i]);
            }
            
			return systemInfoList;
		}

		/// <summary>Returns whether an element is online or not.</summary>
		/// <param name="systemId">System ID.</param>
		/// <returns>True if the element is online, false otherwise.</returns>        
		internal bool IsElementOnline(string systemId)
		{
			bool result = false;

			if (!string.IsNullOrEmpty(systemId))
			{
				lock (_systemListLock)
				{
					result = _systemList.Exists(item => item.SystemId == systemId && item.IsOnline);
				}
			}

			return result;
		}

		/// <summary>Check if a given element exists in the list</summary>
		/// <param name="systemID">System id.</param>
		/// <returns>True if the element is in the list, false otherwise.</returns>
		internal bool ElementExists(string systemId)
		{
			bool result = false;

			if (!string.IsNullOrEmpty(systemId))
			{
				lock (_systemListLock)
				{
					result = _systemList.Exists(item => item.SystemId == systemId);
				}
			}

			return result;
		}

		/// <summary>Search AvailableElementData By ElementNumber.</summary>
		/// <param name="elementNumber">element number.</param>
		/// <param name="objAvailableElementData">[out] Information describing the object available element.</param>
		/// <returns>if found return AvailableElementData else null.</returns>
		internal AvailableElementData GetAvailableElementDataByElementNumber(string elementNumber)
		{
			AvailableElementData elementData = null;

			if (!string.IsNullOrEmpty(elementNumber))
			{
				lock (_systemListLock)
				{
					SystemInfo system = _systemList.Find(element => element.SystemId == elementNumber);

					elementData = T2GDataConverter.BuildAvailableElementData(system);
				}
			}

			return elementData;
		}

		/// <summary>Search AvailableElementData by mission operator code.</summary>
		/// <param name="operatorCode">Mission operator code</param>
		/// <returns>list of found AvailableElementData.</returns>
		internal ElementList<AvailableElementData> GetAvailableElementDataListByMissionOperatorCode(string operatorCode)
		{
			ElementList<AvailableElementData> elementDataList = new ElementList<AvailableElementData>();

			if (!string.IsNullOrEmpty(operatorCode))
			{
				lock (_systemListLock)
				{
					List<SystemInfo> systemList = _systemList.FindAll(element => (element.PisMission != null && element.PisMission.OperatorCode == operatorCode));

					foreach (SystemInfo system in systemList)
					{
						AvailableElementData elementData = T2GDataConverter.BuildAvailableElementData(system);

						if (elementData != null)
						{
							elementDataList.Add(elementData);
						}
					}
				}
			}

			return elementDataList;
		}

		/// <summary>Search AvailableElementData by mission code (aka commercial number).</summary>
		/// <param name="commercialNumber">Mission code (aka commercial number)</param>
		/// <returns>list of found AvailableElementData.</returns>
		internal ElementList<AvailableElementData> GetAvailableElementDataListByMissionCode(string commercialNumber)
		{
			ElementList<AvailableElementData> elementDataList = new ElementList<AvailableElementData>();

			if (!string.IsNullOrEmpty(commercialNumber))
			{
				lock (_systemListLock)
				{
					List<SystemInfo> systemList = _systemList.FindAll(element => (element.PisMission != null && element.PisMission.CommercialNumber == commercialNumber));

					foreach (SystemInfo system in systemList)
					{
						AvailableElementData elementData = T2GDataConverter.BuildAvailableElementData(system);

						if (elementData != null)
						{
							elementDataList.Add(elementData);
						}
					}
				}
			}

			return elementDataList;
		}

		/// <summary>Get all AvailableElementData</summary>
		/// <returns>list of all AvailableElementData.</returns>
		internal ElementList<AvailableElementData> GetAvailableElementDataList()
		{
			ElementList<AvailableElementData> elementDataList = new ElementList<AvailableElementData>();

			lock (_systemListLock)
			{
				foreach (SystemInfo system in _systemList)
				{
					AvailableElementData elementData = T2GDataConverter.BuildAvailableElementData(system);

					if (elementData != null)
					{
						elementDataList.Add(elementData);
					}
				}
			}

			return elementDataList;
		}

		#endregion

		#region Private Methods

		/// <summary>Initializes the local storage data.</summary>
		internal void InitializeLocalStorageData()
		{
			// Get available systems and subscribe to their services
			// This may or may not return any info at first. Subsequent
			// updates must be made on T2G notifications

			DeinitializeLocalStorageData();

			InitializeSystemList();

			EnableSystemNotifications(true);
		}

		/// <summary>Deinitialize local storage data.</summary>
		internal void DeinitializeLocalStorageData()
		{
			lock (_systemListLock)
			{
				_systemList.Clear();
			}
		}

		/// <summary>To get all known T2G systems.</summary>
		private void InitializeSystemList()
		{
			try
			{
				LogManager.WriteLog(TraceType.INFO, "Getting system list...", "PIS.Ground.Core.T2G.LocalDataStorage.GetSystemList", null, EventIdEnum.GroundCore);

				using (PIS.Ground.Core.T2G.WebServices.Identification.IdentificationPortTypeClient objIdentification = new IdentificationPortTypeClient())
				{
					systemList lSystemStructList = objIdentification.enumSystems(_sessionData.SessionId);

					List<SystemInfo> lSystemList;
					if (T2GDataConverter.BuildSystemList(lSystemStructList, out lSystemList))
					{
						// Construct the current system list from the returned data
						UpdateSystemList(lSystemList);
					}

					SubscribeToMessageNotifications();
					SubscribeToServiceNotifications();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.T2G.T2GClient.GetSystemList", ex, EventIdEnum.GroundCore);
			}
		}

		/// <summary>Subscribe to all message notifications.</summary>
		private void SubscribeToMessageNotifications()
		{
			LogManager.WriteLog(
				TraceType.INFO,
				"SubscribeToMessageNotifications: System=ALL, Message=ALL",
				"PIS.Ground.Core.T2G.T2GClient.SubscribeToMessageNotifications",
				null,
				EventIdEnum.GroundCore);

			// subscribe to each message individually
			foreach (string messageId in _messageIdList)
			{
				SubscribeToMessageNotifications(messageId);
			}
		}

		/// <summary>Subscribe to message notifications.</summary>
		/// <param name="messageId">Identifier for the message.</param>
		private void SubscribeToMessageNotifications(string messageId)
		{
			LogManager.WriteLog(
				TraceType.INFO,
				"SubscribeToMessageNotifications: System=ALL, Message=" + messageId,
				"PIS.Ground.Core.T2G.T2GClient.SubscribeToMessageNotifications",
				null,
				EventIdEnum.GroundCore);

			try
			{
				messageSubscriptionList messageSubscriptions = new messageSubscriptionList();

				messageSubscriptions.Add(
					new messageSubscriptionStruct
					{
						messageId = messageId,
						notificationMode = notificationModeEnum.onChanges,
						notifyFreq = 0
					});

				using (PIS.Ground.Core.T2G.WebServices.VehicleInfo.VehicleInfoPortTypeClient objVehicleInfo = new VehicleInfoPortTypeClient())
				{
					objVehicleInfo.subscribeToMessageNotifications(
						_sessionData.SessionId,
						new systemIdList(),
						messageSubscriptions);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.WARNING, ex.Message, "PIS.Ground.Core.T2G.LocalDataStorage.SubscribeToMessageNotifications", ex, EventIdEnum.GroundCore);
			}
		}

		/// <summary>Subscribe to all service notifications.</summary>
		private void SubscribeToServiceNotifications()
		{
			LogManager.WriteLog(
				TraceType.INFO,
				"SubscribeToServiceNotifications: System=ALL, Service=ALL",
				"PIS.Ground.Core.T2G.T2GClient.SubscribeToServiceNotifications",
				null,
				EventIdEnum.GroundCore);

			// subscribe to each service individually
			foreach (eServiceID serviceId in _serviceIdList)
			{
				SubscribeToServiceNotifications(serviceId);
			}
		}

		/// <summary>Subscribe to one or many notification Service.</summary>
		/// <param name="pServiceId">Service ID of the information needed.</param>
		private void SubscribeToServiceNotifications(eServiceID serviceId)
		{
			LogManager.WriteLog(
				TraceType.INFO,
				"Subscribing to serviceId=" + serviceId,
				"PIS.Ground.Core.T2G.LocalDataStorage.SubscribeToServiceNotifications",
				null,
				EventIdEnum.GroundCore);

			try
			{
				using (PIS.Ground.Core.T2G.WebServices.VehicleInfo.VehicleInfoPortTypeClient objVehicleInfo = new VehicleInfoPortTypeClient())
				{
					objVehicleInfo.subscribeToServiceNotifications(_sessionData.SessionId, new systemIdList(), (int)serviceId);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.WARNING, ex.Message, "PIS.Ground.Core.T2G.T2GClient.SubscribeToServiceNotifications", ex, EventIdEnum.GroundCore);
			}
		}

		/// <summary>To Build the System list from the response of the enumSystems service.</summary>
		/// <param name="objSysLst">Sender info.</param>
		private void UpdateSystemList(List<SystemInfo> objSysLst)
		{
			LogManager.WriteLog(TraceType.INFO, "Updating cached system list...", "PIS.Ground.Core.T2G.LocalDataStorage.UpdateSystemList", null, EventIdEnum.GroundCore);

			if (objSysLst != null)
			{
				foreach (SystemInfo objSys in objSysLst)
				{
					UpdateSystem(objSys);
				}
			}
		}

		/// <summary>Updates system info according to input parameter.</summary>
		/// <param name="systemInfo">The new system informations.</param>
		private void UpdateSystem(SystemInfo newSystemInfo)
		{
			if (newSystemInfo != null)
			{
				if (!string.IsNullOrEmpty(newSystemInfo.SystemId))
				{
					lock (_systemListLock)
					{
						//Check if an item with same SystemID exists

						int indexFound = _systemList.FindIndex(item => item.SystemId == newSystemInfo.SystemId);

						//Check if it simply needs to be added or updated

						if (indexFound < 0)
						{
							// Add fresh info

							_systemList.Add(newSystemInfo);
						}
						else
						{
                            SystemInfo existingSystemInfo = _systemList[indexFound];

							// Add updated info
							SystemInfo updatedSystemInfo = new SystemInfo(
								newSystemInfo.SystemId,
								newSystemInfo.MissionId,
								newSystemInfo.VehiclePhysicalId,
								newSystemInfo.Status,
								newSystemInfo.IsOnline,
								newSystemInfo.CommunicationLink,
								existingSystemInfo.ServiceList,
								existingSystemInfo.PisBaseline,
								existingSystemInfo.PisVersion,
								existingSystemInfo.PisMission,
								existingSystemInfo.IsPisBaselineUpToDate && newSystemInfo.IsOnline);

							_systemList[indexFound] = updatedSystemInfo;
						}
					}
				}
				else
				{
					LogManager.WriteLog(TraceType.ERROR, "Input SystemID is empty", "PIS.Ground.Core.T2G.LocalDataStorage.UpdateSystemData", null, EventIdEnum.GroundCore);
				}
			}
		}

		/// <summary>Enables the system notifications.</summary>
		/// <param name="pEnable">True to enable, false to disable.</param>
		private void EnableSystemNotifications(bool pEnable)
		{
			try
			{
				using (PIS.Ground.Core.T2G.WebServices.Identification.IdentificationPortTypeClient objIdentification = new IdentificationPortTypeClient())
				{
					objIdentification.enableSystemNotifications(_sessionData.SessionId, pEnable);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.ERROR, ex.Message, "Ground.Core.T2G.LocalDataStorage.EnableSystemNotifications", ex, EventIdEnum.GroundCore);
			}
		}

		/// <summary>Remove system according to system id.</summary>
		/// <param name="systemId">The system id to be removed from list.</param>
		internal void RemoveSystem(string systemId)
		{
			LogManager.WriteLog(TraceType.INFO, "Removing cached system: SystemID=" + systemId, "PIS.Ground.Core.T2G.LocalDataStorage.RemoveSystem", null, EventIdEnum.GroundCore);

			if (!string.IsNullOrEmpty(systemId))
			{
				lock (_systemListLock)
				{
					// Delete existing, if any
					_systemList.RemoveAll(item => item.SystemId == systemId);
				}
			}
			else
			{
				LogManager.WriteLog(TraceType.ERROR, "Input SystemID is empty", "PIS.Ground.Core.T2G.LocalDataStorage.RemoveSystem", null, EventIdEnum.GroundCore);
			}
		}

		/// <summary>
		/// Update service list
		/// </summary>
		/// <param name="systemId">Identifier for the system.</param>
        /// <param name="subscriptionId">Notification subscription identifier</param>
        /// <param name="updatedServices">The updated services.</param>
        /// <returns>true if the system has been updated, false otherwise.</returns>
		private bool UpdateServiceList(string systemId, int subscriptionId, ServiceInfoList updatedServices)
		{
            bool listUpdated = false;
            if (LogManager.IsTraceActive(TraceType.INFO))
            {
                string context = string.Format(CultureInfo.CurrentCulture, "{0} of service {1}", "PIS.Ground.Core.T2G.LocalDataStorage.UpdateServiceList", ServiceConfiguration.RunningServiceName);
                LogManager.WriteLog(TraceType.INFO, "Updating service list for systemId=" + systemId, context, null, EventIdEnum.GroundCore);
            }

			try
			{
                ushort subscriptionServiceId = 0;
                lock (_subscriptionAssociationsLock)
                {
                    if (!_associationSubscriptionIdWithServiceId.TryGetValue(subscriptionId, out subscriptionServiceId) && updatedServices.Count != 0)
                    {
                        _associationSubscriptionIdWithServiceId[subscriptionId] = updatedServices[0].ServiceId;
                    }
                }

				lock (_systemListLock)
				{
                    int existingSystemInfoIndex = _systemList.FindIndex(s => s.SystemId == systemId);

                    // System is know. Update possible.
                    if (existingSystemInfoIndex > 0)
					{
                        SystemInfo existingSystemInfo = _systemList[existingSystemInfoIndex];

						List<ServiceInfo> updatedList = null;

                        // If the array is empty, this mean that we shall remove all services. Later, T2G will send an update.
                        if (updatedServices.Count == 0)
                        {
                            // If serviceId is 0, this means that no mapping existing between the subscription id and the service id.
                            if (subscriptionServiceId != 0)
                            {
                                updatedList = existingSystemInfo.ServiceList.Where(s => s.ServiceId != subscriptionServiceId).ToList();
                                listUpdated = updatedList.Count != existingSystemInfo.ServiceList.Count;
                            }
                        }
                        else
                        {
                            updatedList = new List<ServiceInfo>(existingSystemInfo.ServiceList);

                            foreach (ServiceInfo service in updatedServices)
                            {
                                if (_filterLocalServiceOnly)
                                {
                                    if (service.VehiclePhysicalId != existingSystemInfo.VehiclePhysicalId)
                                    {
                                        continue;
                                    }
                                }

                                int insertIndex = updatedList.FindIndex(i => i.VehiclePhysicalId == service.VehiclePhysicalId &&
                                    i.ServiceId == service.ServiceId);

                                if (insertIndex < 0)
                                {
                                    updatedList.Add(service);
                                    listUpdated = true;
                                }
                                else if (!service.Equals(updatedList[insertIndex]))
                                {
                                    updatedList[insertIndex] = service;
                                    listUpdated = true;
                                }
                            }

                            if (listUpdated)
                            {
                                // Service list shall be sorted
                                System.Comparison<ServiceInfo> comparer = (ServiceInfo x, ServiceInfo y) => CompareServiceInfo(x, y, existingSystemInfo.VehiclePhysicalId);
                                updatedList.Sort(comparer);
                            }
                        }

                        if (listUpdated)
                        {
                            // Add updated info
                            SystemInfo updatedSystemInfo = new SystemInfo(
                                existingSystemInfo.SystemId,
                                existingSystemInfo.MissionId,
                                existingSystemInfo.VehiclePhysicalId,
                                existingSystemInfo.Status,
                                existingSystemInfo.IsOnline,
                                existingSystemInfo.CommunicationLink,
                                new ServiceInfoList(updatedList),
                                existingSystemInfo.PisBaseline,
                                existingSystemInfo.PisVersion,
                                existingSystemInfo.PisMission,
                                existingSystemInfo.IsPisBaselineUpToDate && existingSystemInfo.IsOnline);

                            _systemList[existingSystemInfoIndex] = updatedSystemInfo;
                        }
					}
				}
			}
			catch (Exception ex)
			{
                string context = string.Format(CultureInfo.CurrentCulture, "{0} of service {1}", "PIS.Ground.Core.T2G.LocalDataStorage.UpdateServiceList", ServiceConfiguration.RunningServiceName);
                LogManager.WriteLog(TraceType.ERROR, ex.Message, context, ex, EventIdEnum.GroundCore);
			}

            return listUpdated;
		}

        /// <summary>
        /// Compares the service information object.
        /// 
        /// <para>This method order item by this field priority:
        /// <list type="number">
        /// <item>IsAvailable. true &lt; false.</item>
        /// <item>ServiceId in ascending order.</item>
        /// <item>VehiclePhysicalId that match the system id. Only possible when filtering local train service is enabled.</item>
        /// <item>VehiclePhysicalId in ascending order.</item>
        /// </list></para>
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <param name="currentVehicleId">The current vehicle identifier.</param>
        /// <returns>a signed integer that indicates the relative values of x and y, as shown in the following table.
        /// <list type="table">
        /// <listheader><term>Value</term><description>Meaning</description></listheader>
        /// <item><term>Lest than 0</term><description>x is less than y.</description></item>
        /// <item><term>0</term><description>x equals y.</description></item>
        /// <item><term>Greater than 0</term><description>x is greater than y.</description></item>
        /// </list></returns>
        private static int CompareServiceInfo(ServiceInfo x, ServiceInfo y, ushort currentVehicleId)
        {
            int comparisonResult = 0;
            if (x.IsAvailable != y.IsAvailable)
            {
                comparisonResult = (x.IsAvailable) ? -1 : 1;
            }
            else if (x.ServiceId != y.ServiceId)
            {
                comparisonResult = Convert.ToInt32(x.ServiceId) - Convert.ToInt32(y.ServiceId);
            }
            else if (x.VehiclePhysicalId != y.VehiclePhysicalId)
            {
                if (x.VehiclePhysicalId == currentVehicleId)
                {
                    comparisonResult = -1;
                }
                else if (y.VehiclePhysicalId == currentVehicleId)
                {
                    comparisonResult = 1;
                }
                else
                {
                    comparisonResult = Convert.ToInt32(x.VehiclePhysicalId) - Convert.ToInt32(y.VehiclePhysicalId);
                }
            }

            return comparisonResult;
        }

		/// <summary>Executes the system changed action.</summary>
		/// <param name="systemInfo">The new system info.</param>
		internal void OnSystemChanged(SystemInfo systemInfo)
		{
			DumpCurrentSystemList(TraceType.DEBUG, "Before OnSystemChanged called:", "PIS.Ground.Core.T2G.LocalDataStorage.OnSystemChanged");

			UpdateSystem(systemInfo);

			DumpCurrentSystemList(TraceType.DEBUG, "After OnSystemChanged called:", "PIS.Ground.Core.T2G.LocalDataStorage.OnSystemChanged");
		}

		/// <summary>Executes the system deleted action.</summary>
		/// <param name="systemId">The removed system id.</param>
		internal void OnSystemDeleted(string systemId)
		{
			LogManager.WriteLog(
				TraceType.WARNING,
				"OnSystemDeleted called:" + systemId,
				"PIS.Ground.Core.T2G.LocalDataStorage.OnSystemDeleted",
				null,
				EventIdEnum.GroundCore);

			RemoveSystem(systemId);

			DumpCurrentSystemList(TraceType.DEBUG, "After OnSystemDeleted called:", "PIS.Ground.Core.T2G.LocalDataStorage.OnSystemDeleted");
		}

		/// <summary>Executes the service changed action.</summary>
		/// <param name="systemId">System id.</param>
		/// <param name="isSystemOnline">True if this object is system online.</param>
		/// <param name="subscriptionId">Identifier for the subscription.</param>
		/// <param name="services">Services that changed.</param>
        /// <returns>true if internal data changed, false otherwise.</returns>
		internal bool OnServiceChanged(string systemId, bool isSystemOnline, int subscriptionId, ServiceInfoList services)
		{
            bool updated = false;
            if (!string.IsNullOrEmpty(systemId) && services != null)
			{
                if (LogManager.IsTraceActive(TraceType.INFO))
                {
                    string message = String.Format(
                    "OnServiceChanged called, systemId={0}, isSystemOnline={1}, subscriptionId={2}, service={3}",
                    systemId, isSystemOnline, subscriptionId, services.Count != 0 ? services[0].ServiceName : "N/A");

                    LogManager.WriteLog(TraceType.INFO, message, "Ground.Core.T2G.T2GNotificationProcessor.OnServiceNotification", null, EventIdEnum.GroundCore);
                }

				lock (_systemListLock)
				{
					SystemInfo objSys = _systemList.Find(obj => obj.SystemId == systemId);

					if (objSys != null && objSys.IsOnline != isSystemOnline)
					{
						// This info is mainly updated in GetSystemList (initially) or OnSystemChanged (after), but
						// we might as well update it here, since it is returned along with the service list.
						SystemInfo updatedSystemInfo = new SystemInfo(
							objSys.SystemId,
							objSys.MissionId,
							objSys.VehiclePhysicalId,
							objSys.Status,
							isSystemOnline,
							objSys.CommunicationLink,
							objSys.ServiceList,
							objSys.PisBaseline,
							objSys.PisVersion,
							objSys.PisMission,
							objSys.IsPisBaselineUpToDate && isSystemOnline);

						UpdateSystem(objSys);
                        updated = true;
					}
				}

                updated |= UpdateServiceList(systemId, subscriptionId, services);
			}

			DumpCurrentSystemList(TraceType.DEBUG, "After OnServiceListChanged called:", "PIS.Ground.Core.T2G.LocalDataStorage.OnServiceListChanged");
            return updated;
		}

		/// <summary>Executes the message changed action.</summary>
		/// <param name="systemId">System id.</param>
		/// <param name="messageId">Identifier for the message.</param>
		/// <param name="lBaseline">The baseline.</param>
		internal void OnMessageChanged(string systemId, string messageId, PisBaseline pisBaseline)
		{
			LogManager.WriteLog(TraceType.INFO, "OnMessageChanged called, systemId=" + systemId + ", messageId=" + messageId, "PIS.Ground.Core.T2G.LocalDataStorage.OnMessageChanged", null, EventIdEnum.GroundCore);

			lock (_systemListLock)
			{
				SystemInfo existingSystemInfo = _systemList.Find(obj => obj.SystemId == systemId);

				if (existingSystemInfo != null)
				{
					// Update existing system

					// Remove obsolete info
					_systemList.Remove(existingSystemInfo);

					// Add updated info
					SystemInfo updatedSystemInfo = new SystemInfo(
						existingSystemInfo.SystemId,
						existingSystemInfo.MissionId,
						existingSystemInfo.VehiclePhysicalId,
						existingSystemInfo.Status,
						existingSystemInfo.IsOnline,
						existingSystemInfo.CommunicationLink,
						existingSystemInfo.ServiceList,
						pisBaseline,
						existingSystemInfo.PisVersion,
						existingSystemInfo.PisMission,
						existingSystemInfo.IsOnline);

					_systemList.Add(updatedSystemInfo);
				}
			}

			DumpCurrentSystemList(TraceType.DEBUG, "After OnMessageChanged called:", "PIS.Ground.Core.T2G.LocalDataStorage.OnMessageChanged");
		}

		/// <summary>Executes the message changed action.</summary>
		/// <param name="systemId">System id.</param>
		/// <param name="messageId">Identifier for the message.</param>
		/// <param name="lVersion">The software version information.</param>
		internal void OnMessageChanged(string systemId, string messageId, PisVersion pisVersion)
		{
			LogManager.WriteLog(TraceType.INFO, "OnMessageChanged called, systemId=" + systemId + ", messageId=" + messageId, "PIS.Ground.Core.T2G.LocalDataStorage.OnMessageChanged", null, EventIdEnum.GroundCore);

			lock (_systemListLock)
			{
				SystemInfo existingSystemInfo = _systemList.Find(obj => obj.SystemId == systemId);

				if (existingSystemInfo != null)
				{
					// Update existing system

					// Remove obsolete info
					_systemList.Remove(existingSystemInfo);

					// Add updated info
					SystemInfo updatedSystemInfo = new SystemInfo(
						existingSystemInfo.SystemId,
						existingSystemInfo.MissionId,
						existingSystemInfo.VehiclePhysicalId,
						existingSystemInfo.Status,
						existingSystemInfo.IsOnline,
						existingSystemInfo.CommunicationLink,
						existingSystemInfo.ServiceList,
						existingSystemInfo.PisBaseline,
						pisVersion,
						existingSystemInfo.PisMission,
						existingSystemInfo.IsPisBaselineUpToDate && existingSystemInfo.IsOnline);

					_systemList.Add(updatedSystemInfo);
				}
			}

			DumpCurrentSystemList(TraceType.DEBUG, "After OnMessageChanged called:", "PIS.Ground.Core.T2G.LocalDataStorage.OnMessageChanged");
		}

		/// <summary>Executes the message changed action.</summary>
		/// <param name="systemId">System id.</param>
		/// <param name="messageId">Identifier for the message.</param>
		/// <param name="lVersion">The software version information.</param>
		internal void OnMessageChanged(string systemId, string messageId, PisMission pisMission)
		{
			LogManager.WriteLog(TraceType.INFO, "OnMessageChanged called, systemId=" + systemId + ", messageId=" + messageId, "PIS.Ground.Core.T2G.LocalDataStorage.OnMessageChanged", null, EventIdEnum.GroundCore);

			lock (_systemListLock)
			{
				SystemInfo existingSystemInfo = _systemList.Find(obj => obj.SystemId == systemId);

				if (existingSystemInfo != null)
				{
					// Update existing system

					// Remove obsolete info
					_systemList.Remove(existingSystemInfo);

					// Add updated info
					SystemInfo updatedSystemInfo = new SystemInfo(
						existingSystemInfo.SystemId,
						existingSystemInfo.MissionId,
						existingSystemInfo.VehiclePhysicalId,
						existingSystemInfo.Status,
						existingSystemInfo.IsOnline,
						existingSystemInfo.CommunicationLink,
						existingSystemInfo.ServiceList,
						existingSystemInfo.PisBaseline,
						existingSystemInfo.PisVersion,
						pisMission,
						existingSystemInfo.IsPisBaselineUpToDate && existingSystemInfo.IsOnline);

					_systemList.Add(updatedSystemInfo);
				}
			}

			DumpCurrentSystemList(TraceType.DEBUG, "After OnMessageChanged called:", "PIS.Ground.Core.T2G.LocalDataStorage.OnMessageChanged");
		}

		#endregion
	}
}