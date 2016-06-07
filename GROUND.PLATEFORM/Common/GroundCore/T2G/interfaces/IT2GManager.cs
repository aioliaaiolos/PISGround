//---------------------------------------------------------------------------------------------------
// <copyright file="IT2GManager.cs" company="Alstom">
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

	/// <summary>Values that represent T2GManagerErrorEnum.</summary>
	public enum T2GManagerErrorEnum
	{
		/// <summary>An enum constant representing the success option.</summary>
		eSuccess,
		/// <summary>An enum constant representing the failed option.</summary>
		eFailed,
		/// <summary>An enum constant representing the T2G server offline option.</summary>
		eT2GServerOffline,
		/// <summary>An enum constant representing the element not found option.</summary>
		eElementNotFound,
		/// <summary>An enum constant representing the service information not found option.</summary>
		eServiceInfoNotFound
	}

	/// <summary>Interface for T2G manager.</summary>
	public interface IT2GManager
	{
		/// <summary>Gets the T2G notifier to signal notifications received from T2G.</summary>
		/// <value>The T2G notifier.</value>
		IT2GNotificationProcessor T2GNotifier
		{
			get;
		}

		/// <summary>Gets the T2G file distribution manager.</summary>
		/// <value>The T2G file distribution manager.</value>
		IT2GFileDistributionManager T2GFileDistributionManager
		{
			get;
		}

		/// <summary>Gets a value indicating the T2G server current connection status.</summary>
		/// <value>true if online, false if not.</value>
		bool T2GServerConnectionStatus
		{
			get;
		}

		/// <summary>Is element online.</summary>
		/// <param name="elementNumber">The element number.</param>
		/// <param name="IsOnline">[out] The is online.</param>
		/// <returns>T2GManagerErrorEnum.</returns>
		T2GManagerErrorEnum IsElementOnline(string elementNumber, out bool IsOnline);

		/// <summary>
		/// Query if 'elementNumber' is online and pis baseline information is up to date.
		/// </summary>
		/// <param name="elementNumber">The element number.</param>
		/// <returns>true if element is online and pis baseline information is up to date, false if not.</returns>
		bool IsElementOnlineAndPisBaselineUpToDate(string elementNumber);

		/// <summary>Gets available service data.</summary>
		/// <param name="strSystemId">Identifier for the system.</param>
		/// <param name="intServiceId">Identifier for the int service.</param>
		/// <param name="objSer">[out] The object ser.</param>
		/// <returns>T2GManagerErrorEnum.</returns>
		T2GManagerErrorEnum GetAvailableServiceData(string strSystemId, int intServiceId, out PIS.Ground.Core.Data.ServiceInfo objSer);

		/// <summary>Gets available element data by element number.</summary>
		/// <param name="elementNumber">The element number.</param>
		/// <param name="objAvailableElementData">[out] Information describing the object available element.</param>
		/// <returns>T2GManagerErrorEnum.</returns>
		T2GManagerErrorEnum GetAvailableElementDataByElementNumber(string elementNumber, out PIS.Ground.Core.Data.AvailableElementData objAvailableElementData);

		/// <summary>Gets available element data by target address.</summary>
		/// <param name="targetAddress">Target address.</param>
		/// <param name="lstAvailableElementData">[out] Information describing the list available element.</param>
		/// <returns>T2GManagerErrorEnum.</returns>
		T2GManagerErrorEnum GetAvailableElementDataByTargetAddress(PIS.Ground.Core.Data.TargetAddressType targetAddress, out PIS.Ground.Core.Data.ElementList<PIS.Ground.Core.Data.AvailableElementData> lstAvailableElementData);

		/// <summary>Gets available element data list.</summary>
		/// <param name="lstAvailableElementData">[out] Information describing the list available element.</param>
		/// <returns>T2GManagerErrorEnum.</returns>
		T2GManagerErrorEnum GetAvailableElementDataList(out PIS.Ground.Core.Data.ElementList<PIS.Ground.Core.Data.AvailableElementData> lstAvailableElementData);

		/// <summary>Gets available element data list by mission code.</summary>
		/// <param name="missionCommercialNumber">The mission commercial number.</param>
		/// <param name="lstAvailableElementData">[out] Information describing the list available element.</param>
		/// <returns>T2GManagerErrorEnum.</returns>
		T2GManagerErrorEnum GetAvailableElementDataListByMissionCode(string missionCommercialNumber, out PIS.Ground.Core.Data.ElementList<PIS.Ground.Core.Data.AvailableElementData> lstAvailableElementData);

		/// <summary>Gets available element data list by mission operator code.</summary>
		/// <param name="missionOperatorCode">The mission operator code.</param>
		/// <param name="lstAvailableElementData">[out] Information describing the list available element.</param>
		/// <returns>T2GManagerErrorEnum.</returns>
		T2GManagerErrorEnum GetAvailableElementDataListByMissionOperatorCode(string missionOperatorCode, out PIS.Ground.Core.Data.ElementList<PIS.Ground.Core.Data.AvailableElementData> lstAvailableElementData);

		/// <summary>Subscribe to the system deleted notification.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		/// <param name="eventHandler">The event handler.</param>
		void SubscribeToSystemDeletedNotification(string subscriberId, EventHandler<PIS.Ground.Core.Data.SystemDeletedNotificationArgs> eventHandler);

		/// <summary>Unsubscribe to the system deleted notification.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		void UnsubscribeFromSystemDeletedNotification(string subscriberId);

		/// <summary>Subscribe to T2G connection notification.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		/// <param name="eventHandler">The event handler.</param>
		/// <param name="notifyImmediately">true to notify immediately after subscription.</param>
		void SubscribeToT2GOnlineStatusNotification(string subscriberId, EventHandler<PIS.Ground.Core.Data.T2GOnlineStatusNotificationArgs> eventHandler, bool notifyImmediately);

		/// <summary>Unsubscribe from T2G connection notification.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		void UnsubscribeFromT2GOnlineStatusNotification(string subscriberId);

		/// <summary>Subscribe to element change notification.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		/// <param name="eventHandler">The event handler.</param>
		void SubscribeToElementChangeNotification(string subscriberId, EventHandler<PIS.Ground.Core.Data.ElementEventArgs> eventHandler);

		/// <summary>Unsubscribe from the element change notification.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		void UnsubscribeFromElementChangeNotification(string subscriberId);

		/// <summary>Subscribe to to file distribution notifications.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		/// <param name="eventHandler">The event handler.</param>
		void SubscribeToFileDistributionNotifications(string subscriberId, EventHandler<PIS.Ground.Core.Data.FileDistributionStatusArgs> eventHandler);

		/// <summary>Unsubscribe from file distribution notifications.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		void UnsubscribeFromFileDistributionNotifications(string subscriberId);

		/// <summary>Subscribe to file publication notification.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		/// <param name="eventHandler">The event handler.</param>
		void SubscribeToFilePublicationNotification(string subscriberId, EventHandler<PIS.Ground.Core.Data.FilePublicationNotificationArgs> eventHandler);

		/// <summary>Unsubscribe from file publication notification.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		void UnsubscribeFromFilePublicationNotification(string subscriberId);

		/// <summary>Subscribe to file published notification.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		/// <param name="eventHandler">The event handler.</param>
		void SubscribeToFilePublishedNotification(string subscriberId, EventHandler<PIS.Ground.Core.Data.FilePublishedNotificationArgs> eventHandler);

		/// <summary>Unsubscribe from file published notification.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		void UnsubscribeFromFilePublishedNotification(string subscriberId);

		/// <summary>Subscribe to file recieved notification.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		/// <param name="eventHandler">The event handler.</param>
		void SubscribeToFileReceivedNotification(string subscriberId, EventHandler<PIS.Ground.Core.Data.FileReceivedArgs> eventHandler);

		/// <summary>Unsubscribe from file received notification.</summary>
		/// <param name="subscriberId">The unique subscriber identifier.</param>
		void UnsubscribeFromFileReceivedNotification(string subscriberId);
	}
}
