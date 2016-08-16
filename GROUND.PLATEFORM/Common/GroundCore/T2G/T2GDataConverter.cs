//---------------------------------------------------------------------------------------------------
// <copyright file="T2GDataConverter.cs" company="Alstom">
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
	using System.Text;
	using PIS.Ground.Core.Data;
	using PIS.Ground.Core.T2G.WebServices.Notification;
	using PIS.Ground.Core.T2G.WebServices.FileTransfer;

	/// <summary>T2G data converter helper class.</summary>
	internal class T2GDataConverter
	{
		/// <summary>Message id PIS.BASELINE.</summary>
		internal const string PisBaseline = "PIS.BASELINE";

		/// <summary>Message id PIS.VERSION.</summary>
		internal const string PisVersion = "PIS.VERSION";

		/// <summary>Message id PIS.MISSION.</summary>
		internal const string PisMission = "PIS.MISSION";

		/// <summary>Message id SIVNG.MISSION</summary>
		internal const string SivngMission = "SIVNG.MISSION";

		/// <summary>Maps the CommercialNumber of the FieldInfo from T2G Service.</summary>
		internal const string CommercialNumber = "CommercialNumber";

		/// <summary>Maps the OperatorCode of the FieldInfo from T2G Service.</summary>
		internal const string OperatorCode = "OperatorCode";

		/// <summary>Maps the mission state of the FieldInfo from T2G Service.</summary>
		internal const string MissionState = "State";

		/// <summary>Maps the ElementNumber of the FieldInfo from T2G Service.</summary>
		internal const string ElementNumber = "Number";

		/// <summary>Maps the PISBasicPackageVersion of the FieldInfo from T2G Service.</summary>
		internal const string PISBasicPackageVersion = "PISBasicPackageVersion";

		/// <summary>Maps the LMTPackageVersion of the FieldInfo from T2G Service.</summary>
		internal const string LMTPackageVersion = "LMTPackageVersion";

		/// <summary>Variable Archived Valid Out.</summary>
		private const string ArchivedValidOut = "Archived Valid Out";

		/// <summary>Variable Archived Version Out.</summary>
		private const string ArchivedVersionOut = "Archived Version Out";

		/// <summary>Variable Archived Version PisBase Out.</summary>
		private const string ArchivedVersionPisBaseOut = "Archived Version PisBase Out";

		/// <summary>Variable Archived Version PisMission Out.</summary>
		private const string ArchivedVersionPisMissionOut = "Archived Version PisMission Out";

		/// <summary>Variable Archived Version PisInfotainment Out.</summary>
		private const string ArchivedVersionPisInfotainmentOut = "Archived Version PisInfotainment Out";

		/// <summary>Variable Archived Version Lmt Out.</summary>
		private const string ArchivedVersionLmtOut = "Archived Version Lmt Out";

		/// <summary>Variable Current Forced Out.</summary>
		private const string CurrentForcedOut = "Current Forced Out";

		/// <summary>Variable Current Valid Out.</summary>
		private const string CurrentValidOut = "Current Valid Out";

		/// <summary>Variable Current Version Out.</summary>
		private const string CurrentVersionOut = "Current Version Out";

		/// <summary>Variable Current Version PisBase Out.</summary>
		private const string CurrentVersionPisBaseOut = "Current Version PisBase Out";

		/// <summary>Variable Current Version PisMission Out.</summary>
		private const string CurrentVersionPisMissionOut = "Current Version PisMission Out";

		/// <summary>Variable Current Version PisInfotainment Out.</summary>
		private const string CurrentVersionPisInfotainmentOut = "Current Version PisInfotainment Out";

		/// <summary>Variable Current Version Lmt Out.</summary>
		private const string CurrentVersionLmtOut = "Current Version Lmt Out";

		/// <summary>Variable Current ExpirationDate Out.</summary>
		private const string CurrentExpirationDateOut = "Current ExpirationDate Out";

		/// <summary>Variable Future Valid Out.</summary>
		private const string FutureValidOut = "Future Valid Out";

		/// <summary>Variable Future Version Out.</summary>
		private const string FutureVersionOut = "Future Version Out";

		/// <summary>Variable Future Version PisBase Out.</summary>
		private const string FutureVersionPisBaseOut = "Future Version PisBase Out";

		/// <summary>Variable Archived Valid Out.</summary>
		private const string FutureVersionPisMissionOut = "Future Version PisMission Out";

		/// <summary>Variable Future Version PisInfotainment Out.</summary>
		private const string FutureVersionPisInfotainmentOut = "Future Version PisInfotainment Out";

		/// <summary>Variable Future Version Lmt Out.</summary>
		private const string FutureVersionLmtOut = "Future Version Lmt Out";

		/// <summary>Variable Future ActivationDate Out.</summary>
		private const string FutureActivationDateOut = "Future ActivationDate Out";

		/// <summary>Variable Future ExpirationDate Out.</summary>
		private const string FutureExpirationDateOut = "Future ExpirationDate Out";

		/// <summary>Variable Version PIS Software.</summary>
		private const string VersionPISSoftware = "Version PIS Software";

		/// <summary>
		///   Logs an error for a notification message item missing error.
		///   This was added for the following CR:
		///   atvcm00614906 - [PIS-GROUND URBAN] Gap 2277: missing notification when mission code not defined in t2gvehicleinfo.xml
		/// </summary>
		/// <param name="expectedItemsInMessage">List of items that where expected but not found in the received T2G notification message.</param>
		/// <param name="messageType">The type of the notification message (for logging).</param>
		private static void LogNotificationMessageItemMissingError(List<string> expectedItemsInMessage, string messageType)
		{
			// CR: atvcm00614906 - [PIS-GROUND URBAN] Gap 2277: missing notification when mission code not defined in t2gvehicleinfo.xml
			// Log an error if the message did not contain all expected items.
			StringBuilder listOfItems = new StringBuilder();
			string message;

			foreach (string item in expectedItemsInMessage)
			{
				if (listOfItems.Length > 0)
				{
					listOfItems.Append(", ");
				}

				listOfItems.Append(item);
			}

			message = string.Format(
			CultureInfo.CurrentCulture,
			Properties.Resources.T2GConverterMissingItemsInNotificationMessageError,
			messageType,
			listOfItems.ToString());

			LogMgmt.LogManager.WriteLog(TraceType.ERROR, message, "PIS.Ground.Core.T2G.T2GDataConverter", null, EventIdEnum.GroundCore);
		}

		/// <summary>Build PisBaseLine object.</summary>
		/// <param name="fieldList">FieldList for a message.</param>
		/// <returns>PisBaseline object if it succeeds, null if it fails.</returns>
		internal static PisBaseline BuildPisBaseLine(fieldStruct[] fieldList)
		{
			PisBaseline baseline = null;

			if (fieldList != null && fieldList.Length > 0)
			{
				baseline = new PisBaseline();

				// CR: atvcm00614906 - [PIS-GROUND URBAN] Gap 2277: missing notification when mission code not defined in t2gvehicleinfo.xml
				// Must verify if all expected items are in the message... If not, we will log a trace.
				List<string> expectedItemsInMessage = new List<string>() { ArchivedValidOut, ArchivedVersionOut, ArchivedVersionPisBaseOut, ArchivedVersionPisMissionOut, ArchivedVersionPisInfotainmentOut, ArchivedVersionLmtOut, CurrentForcedOut, CurrentValidOut, CurrentVersionOut, CurrentVersionPisBaseOut, CurrentVersionPisMissionOut, CurrentVersionPisInfotainmentOut, CurrentVersionLmtOut, CurrentExpirationDateOut, FutureValidOut, FutureVersionOut, FutureVersionPisBaseOut, FutureVersionPisMissionOut, FutureVersionPisInfotainmentOut, FutureVersionLmtOut, FutureActivationDateOut, FutureExpirationDateOut };

				foreach (fieldStruct objfieldStruct in fieldList)
				{
					if (ArchivedValidOut == objfieldStruct.id)
					{
						baseline.ArchivedValidOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(ArchivedValidOut);
						}

						continue;
					}

					if (ArchivedVersionOut == objfieldStruct.id)
					{
						baseline.ArchivedVersionOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(ArchivedVersionOut);
						}

						continue;
					}

					if (ArchivedVersionPisBaseOut == objfieldStruct.id)
					{
						baseline.ArchivedVersionPisBaseOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(ArchivedVersionPisBaseOut);
						}

						continue;
					}

					if (ArchivedVersionPisMissionOut == objfieldStruct.id)
					{
						baseline.ArchivedVersionPisMissionOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(ArchivedVersionPisMissionOut);
						}

						continue;
					}

					if (ArchivedVersionPisInfotainmentOut == objfieldStruct.id)
					{
						baseline.ArchivedVersionPisInfotainmentOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(ArchivedVersionPisInfotainmentOut);
						}

						continue;
					}

					if (ArchivedVersionLmtOut == objfieldStruct.id)
					{
						baseline.ArchivedVersionLmtOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(ArchivedVersionLmtOut);
						}

						continue;
					}

					if (CurrentForcedOut == objfieldStruct.id)
					{
						baseline.CurrentForcedOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(CurrentForcedOut);
						}

						continue;
					}

					if (CurrentValidOut == objfieldStruct.id)
					{
						baseline.CurrentValidOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(CurrentValidOut);
						}

						continue;
					}

					if (CurrentVersionOut == objfieldStruct.id)
					{
						baseline.CurrentVersionOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(CurrentVersionOut);
						}

						continue;
					}

					if (CurrentVersionPisBaseOut == objfieldStruct.id)
					{
						baseline.CurrentVersionPisBaseOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(CurrentVersionPisBaseOut);
						}

						continue;
					}

					if (CurrentVersionPisMissionOut == objfieldStruct.id)
					{
						baseline.CurrentVersionPisMissionOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(CurrentVersionPisMissionOut);
						}

						continue;
					}

					if (CurrentVersionPisInfotainmentOut == objfieldStruct.id)
					{
						baseline.CurrentVersionPisInfotainmentOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(CurrentVersionPisInfotainmentOut);
						}

						continue;
					}

					if (CurrentVersionLmtOut == objfieldStruct.id)
					{
						baseline.CurrentVersionLmtOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(CurrentVersionLmtOut);
						}

						continue;
					}

					if (CurrentExpirationDateOut == objfieldStruct.id)
					{
						baseline.CurrentExpirationDateOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(CurrentExpirationDateOut);
						}

						continue;
					}

					if (FutureValidOut == objfieldStruct.id)
					{
						baseline.FutureValidOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(FutureValidOut);
						}

						continue;
					}

					if (FutureVersionOut == objfieldStruct.id)
					{
						baseline.FutureVersionOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(FutureVersionOut);
						}

						continue;
					}

					if (FutureVersionPisBaseOut == objfieldStruct.id)
					{
						baseline.FutureVersionPisBaseOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(FutureVersionPisBaseOut);
						}

						continue;
					}

					if (FutureVersionPisMissionOut == objfieldStruct.id)
					{
						baseline.FutureVersionPisMissionOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(FutureVersionPisMissionOut);
						}

						continue;
					}

					if (FutureVersionPisInfotainmentOut == objfieldStruct.id)
					{
						baseline.FutureVersionPisInfotainmentOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(FutureVersionPisInfotainmentOut);
						}

						continue;
					}

					if (FutureVersionLmtOut == objfieldStruct.id)
					{
						baseline.FutureVersionLmtOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(FutureVersionLmtOut);
						}

						continue;
					}

					if (FutureActivationDateOut == objfieldStruct.id)
					{
						baseline.FutureActivationDateOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(FutureActivationDateOut);
						}

						continue;
					}

					if (FutureExpirationDateOut == objfieldStruct.id)
					{
						baseline.FutureExpirationDateOut = objfieldStruct.value;

						if (objfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(FutureExpirationDateOut);
						}

						continue;
					}
				}

				// CR: atvcm00614906 - [PIS-GROUND URBAN] Gap 2277: missing notification when mission code not defined in t2gvehicleinfo.xml
				// Log an error if the message did not contain all expected items.
				if (expectedItemsInMessage.Count > 0)
				{
					LogNotificationMessageItemMissingError(expectedItemsInMessage, T2GDataConverter.PisVersion);
				}
			}

			return baseline;
		}

		/// <summary>Build PisVersion.</summary>
		/// <param name="fieldList">FieldList for a message.</param>
		/// <returns>PisVersion object if it succeeds, null if it fails.</returns>
		internal static PisVersion BuildPisVersion(fieldStruct[] fieldList)
		{
			PisVersion version = null;

			if (fieldList != null && fieldList.Length > 0)
			{
				version = new PisVersion();

				// CR: atvcm00614906 - [PIS-GROUND URBAN] Gap 2277: missing notification when mission code not defined in t2gvehicleinfo.xml
				// Must verify if all expected items are in the message... If not, we will log a trace.
				List<string> expectedItemsInMessage = new List<string>() { VersionPISSoftware };

				foreach (fieldStruct lObjfieldStruct in fieldList)
				{
					if (VersionPISSoftware == lObjfieldStruct.id)
					{
						version.VersionPISSoftware = lObjfieldStruct.value;

						if (lObjfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(VersionPISSoftware);
						}

						continue;
					}
				}

				// CR: atvcm00614906 - [PIS-GROUND URBAN] Gap 2277: missing notification when mission code not defined in t2gvehicleinfo.xml
				// Log an error if the message did not contain all expected items.
				if (expectedItemsInMessage.Count > 0)
				{
					LogNotificationMessageItemMissingError(expectedItemsInMessage, T2GDataConverter.PisVersion);
				}
			}

			return version;
		}

		/// <summary>Build PisMission.</summary>
		/// <param name="fieldList">FieldList for a message.</param>
		/// <returns>PisVersion object if it succeeds, null if it fails.</returns>
		internal static PisMission BuildPisMission(fieldStruct[] fieldList)
		{
			PisMission mission = null;

			if (fieldList != null && fieldList.Length > 0)
			{
				mission = new PisMission();

				// CR: atvcm00614906 - [PIS-GROUND URBAN] Gap 2277: missing notification when mission code not defined in t2gvehicleinfo.xml
				// Must verify if all expected items are in the message... If not, we will log a trace.
				List<string> expectedItemsInMessage = new List<string>() { CommercialNumber, OperatorCode, MissionState };

				foreach (fieldStruct lObjfieldStruct in fieldList)
				{
					if (lObjfieldStruct.id == CommercialNumber)
					{
						mission.CommercialNumber = lObjfieldStruct.value;

						if (lObjfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(CommercialNumber);
						}

						continue;
					}

					if (lObjfieldStruct.id == OperatorCode)
					{
						mission.OperatorCode = lObjfieldStruct.value;

						if (lObjfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(OperatorCode);
						}

						continue;
					}

					if (lObjfieldStruct.id == MissionState)
					{
						try
						{
							mission.MissionState = (MissionStateEnum)Enum.Parse(typeof(MissionStateEnum), lObjfieldStruct.value);
						}
						catch (Exception)
						{
							mission.MissionState = MissionStateEnum.NI;
						}

						if (lObjfieldStruct.type != fieldTypeEnum.unknown)
						{
							expectedItemsInMessage.Remove(MissionState);
						}

						continue;
					}
				}

				// CR: atvcm00614906 - [PIS-GROUND URBAN] Gap 2277: missing notification when mission code not defined in t2gvehicleinfo.xml
				// Log an error if the message did not contain all expected items.
				if (expectedItemsInMessage.Count > 0)
				{
					LogNotificationMessageItemMissingError(expectedItemsInMessage, string.Concat(T2GDataConverter.PisMission, "/", T2GDataConverter.SivngMission));
				}
			}

			return mission;
		}

		/// <summary>Builds system list.</summary>
		/// <param name="pSystemListIn">The system list in.</param>
		/// <param name="pSystemList">The list of systems [out].</param>
		/// <returns>True if it succeeds, false if it fails.</returns>
		internal static bool BuildSystemList(PIS.Ground.Core.T2G.WebServices.Identification.systemList pSystemListIn, out List<SystemInfo> pSystemList)
		{
			bool lResult = false;
			pSystemList = new List<SystemInfo>();

			if (pSystemListIn != null)
			{
				foreach (PIS.Ground.Core.T2G.WebServices.Identification.systemInfoStruct lSystemInfoStruct in pSystemListIn)
				{
					SystemInfo lSystem;
					if (BuildSystem(lSystemInfoStruct, out lSystem))
					{
						pSystemList.Add(lSystem);
					}
				}

				lResult = true;
			}

			return lResult;
		}

		/// <summary>Builds a service.</summary>
		/// <param name="pServiceStruct">The service structure.</param>
		/// <returns>Service object if it succeeds, null if it fails.</returns>
		internal static ServiceInfo BuildService(PIS.Ground.Core.T2G.WebServices.Notification.serviceStruct pServiceStruct)
		{
			ServiceInfo service = null;

			if (pServiceStruct != null)
			{
				service = new ServiceInfo(
					pServiceStruct.serviceId,
					pServiceStruct.name,
					pServiceStruct.vehiclePhysicalId,
					pServiceStruct.operatorId,
					pServiceStruct.isAvailable,
					pServiceStruct.serviceIPAddress,
					pServiceStruct.AID,
					pServiceStruct.SID,
					pServiceStruct.servicePortNumber);
			}

			return service;
		}

		/// <summary>Builds a system.</summary>
		/// <param name="pSystemStruct">The system structure.</param>
		/// <returns>A built system if it succeeds, null if it fails.</returns>
		internal static SystemInfo BuildSystem(PIS.Ground.Core.T2G.WebServices.Notification.systemInfoStruct pSystemStruct)
		{
			SystemInfo newSystem = null;

			if (pSystemStruct != null)
			{
				CommunicationLink commLink;

				switch (pSystemStruct.communicationLink)
				{
					case PIS.Ground.Core.T2G.WebServices.Notification.commLinkEnum.Item2G3G:
						commLink = CommunicationLink._2G3G;
						break;
					case PIS.Ground.Core.T2G.WebServices.Notification.commLinkEnum.notApplicable:
						commLink = CommunicationLink.NotApplicable;
						break;
					case PIS.Ground.Core.T2G.WebServices.Notification.commLinkEnum.wifi:
						commLink = CommunicationLink.WIFI;
						break;
					default:
						commLink = CommunicationLink.NotApplicable;
						break;
				}

				newSystem = new SystemInfo(
					pSystemStruct.systemId,
					pSystemStruct.missionId,
					pSystemStruct.vehiclePhysicalId,
					pSystemStruct.status,
					pSystemStruct.isOnline,
					commLink,
					new ServiceInfoList(),
					new PisBaseline(),
					new PisVersion(),
					new PisMission(),
					false);
			}

			return newSystem;
		}

		/// <summary>Builds a system.</summary>
		/// <param name="systemStruct">The system structure.</param>
		/// <param name="system">The builded system [out].</param>
		/// <returns>True if it succeeds, false if it fails.</returns>
		internal static bool BuildSystem(PIS.Ground.Core.T2G.WebServices.Identification.systemInfoStruct systemStruct, out SystemInfo system)
		{
			bool lResult = false;
			system = new SystemInfo();

			if (systemStruct != null)
			{
				CommunicationLink commLink;

				switch (systemStruct.communicationLink)
				{
					case PIS.Ground.Core.T2G.WebServices.Identification.commLinkEnum._2G3G:
						commLink = CommunicationLink._2G3G;
						break;
					case PIS.Ground.Core.T2G.WebServices.Identification.commLinkEnum.notApplicable:
						commLink = CommunicationLink.NotApplicable;
						break;
					case PIS.Ground.Core.T2G.WebServices.Identification.commLinkEnum.wifi:
						commLink = CommunicationLink.WIFI;
						break;
					default:
						commLink = CommunicationLink.NotApplicable;
						break;
				}

				system = new SystemInfo(
						systemStruct.systemId,
						systemStruct.missionId,
						systemStruct.vehiclePhysicalId,
						systemStruct.status,
						systemStruct.isOnline,
						commLink,
						new ServiceInfoList(),
						new PisBaseline(),
						new PisVersion(),
						new PisMission(),
						false);

				lResult = true;
			}

			return lResult;
		}

		/// <summary>Builds a service.</summary>
		/// <param name="pServiceStruct">The service structure.</param>
		/// <param name="pService">The builded service [out].</param>
		/// <returns>True if it succeeds, false if it fails.</returns>
		internal static bool BuildService(PIS.Ground.Core.T2G.WebServices.VehicleInfo.serviceStruct pServiceStruct, out ServiceInfo pService)
		{
			bool lResult = false;
			pService = new ServiceInfo();

			if (pServiceStruct != null)
			{
				pService = new ServiceInfo(
					pServiceStruct.serviceId,
					pServiceStruct.name,
					pServiceStruct.vehiclePhysicalId,
					pServiceStruct.operatorId,
					pServiceStruct.isAvailable,
					pServiceStruct.serviceIPAddress,
					pServiceStruct.AID,
					pServiceStruct.SID,
					pServiceStruct.servicePortNumber);

				lResult = true;
			}

			return lResult;
		}

		/// <summary>Builds service list.</summary>
		/// <param name="pServiceStructList">List of service structures.</param>
		/// <param name="pServiceList">The List of services [out].</param>
		/// <returns>Service list.</returns>
		internal static ServiceInfoList BuildServiceList(PIS.Ground.Core.T2G.WebServices.Notification.serviceStruct[] pServiceStructList)
		{
			List<ServiceInfo> tempList = new List<ServiceInfo>(pServiceStructList != null ? pServiceStructList.Length : 0);

			if (pServiceStructList != null)
			{
				foreach (PIS.Ground.Core.T2G.WebServices.Notification.serviceStruct lServiceStruct in pServiceStructList)
				{
					ServiceInfo service = BuildService(lServiceStruct);

					if (service != null)
					{
						tempList.Add(service);
					}
				}
			}

			return new ServiceInfoList(tempList);
		}

		/*
		/// <summary>Builds service list.</summary>
		/// <param name="pServiceStructList">List of service structures.</param>
		/// <param name="pServiceList">The List of services [out].</param>
		/// <returns>True if it succeeds, false if it fails.</returns>
		internal static bool BuildServiceList(PIS.Ground.Core.T2G.WebServices.VehicleInfo.serviceList pServiceStructList, out List<ServiceInfo> pServiceList)
		{
			bool lResult = false;
			pServiceList = new List<ServiceInfo>();

			if (pServiceStructList != null)
			{
				foreach (PIS.Ground.Core.T2G.WebServices.VehicleInfo.serviceStruct lServiceStruct in pServiceStructList)
				{
					ServiceInfo lService;
					if (BuildService(lServiceStruct, out lService))
					{
						pServiceList.Add(lService);
					}
				}

				lResult = true;
			}

			return lResult;
		}
		 * */

		/// <summary>Build LinkTypeEnum of T2G from Ground.</summary>
		/// <param name="objFileTransferMode">FileTransferMode of PIS ground.</param>        
		/// <returns>LinkTypeEnum of T2G.</returns>
		internal static PIS.Ground.Core.T2G.WebServices.FileTransfer.linkTypeEnum BuildLinkTypeEnum(FileTransferMode objFileTransferMode)
		{
			PIS.Ground.Core.T2G.WebServices.FileTransfer.linkTypeEnum lnkTypeEnum;

			switch (objFileTransferMode)
			{
				case FileTransferMode.LowBandwidth:
					lnkTypeEnum = PIS.Ground.Core.T2G.WebServices.FileTransfer.linkTypeEnum.lowBandwidth;
					break;
				case FileTransferMode.HighBandwidth:
					lnkTypeEnum = PIS.Ground.Core.T2G.WebServices.FileTransfer.linkTypeEnum.highBandwidth;
					break;
				case FileTransferMode.AnyBandwidth:
					lnkTypeEnum = PIS.Ground.Core.T2G.WebServices.FileTransfer.linkTypeEnum.anyBandwidth;
					break;
				default:
					lnkTypeEnum = PIS.Ground.Core.T2G.WebServices.FileTransfer.linkTypeEnum.lowBandwidth;
					break;
			}

			return lnkTypeEnum;
		}

		/// <summary>Build LinkTypeEnum of Ground from T2G.</summary>
		/// <param name="lnkTypeEnum">LinkTypeEnum of T2G.</param>
		/// <returns>FileTransferMode of PIS ground.</returns>        
		internal static FileTransferMode BuildFileTransferMode(PIS.Ground.Core.T2G.WebServices.FileTransfer.linkTypeEnum lnkTypeEnum)
		{
			FileTransferMode objFileTransferMode;

			switch (lnkTypeEnum)
			{
				case PIS.Ground.Core.T2G.WebServices.FileTransfer.linkTypeEnum.lowBandwidth:
					objFileTransferMode = FileTransferMode.LowBandwidth;
					break;
				case PIS.Ground.Core.T2G.WebServices.FileTransfer.linkTypeEnum.highBandwidth:
					objFileTransferMode = FileTransferMode.HighBandwidth;
					break;
				case PIS.Ground.Core.T2G.WebServices.FileTransfer.linkTypeEnum.anyBandwidth:
					objFileTransferMode = FileTransferMode.AnyBandwidth;
					break;
				default:
					objFileTransferMode = FileTransferMode.LowBandwidth;
					break;
			}

			return objFileTransferMode;
		}

		/// <summary>Build TransferTypeEnum.</summary>
		/// <param name="objTransferType">TransferType of Ground.</param>
		/// <param name="transferTypeEnum">TransferTypeEnum of T2G.</param>
		internal static PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTypeEnum BuildTransferTypeEnum(TransferType objTransferType)
		{
			PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTypeEnum transferTypeEnum;

			switch (objTransferType)
			{
				case TransferType.GroundToTrain: transferTypeEnum = transferTypeEnum.groundToTrain;
					break;
				case TransferType.TrainToGround: transferTypeEnum = transferTypeEnum.trainToGround;
					break;
				default:
					transferTypeEnum = transferTypeEnum.groundToTrain;
					break;
			}

			return transferTypeEnum;
		}

		/// <summary>Build TransferType.</summary>
		/// <param name="transferTypeEnum">TransferTypeEnum of T2G.</param>
		/// <returns>TransferType of Ground..</returns>        
		internal static TransferType BuildTransferType(PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTypeEnum transferTypeEnum)
		{
			TransferType objTransferType;

			switch (transferTypeEnum)
			{
				case transferTypeEnum.groundToTrain:
					objTransferType = TransferType.GroundToTrain;
					break;
				case transferTypeEnum.trainToGround:
					objTransferType = TransferType.TrainToGround;
					break;
				default:
					objTransferType = TransferType.GroundToTrain;
					break;
			}

			return objTransferType;
		}

		/// <summary>Build TaskState.</summary>
		/// <param name="transferTypeEnum">TaskStateEnum of T2G.</param>
		/// <returns>TaskState of Ground</returns>        
		internal static TaskState BuildTaskState(PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum transferTypeEnum)
		{
			TaskState taskStatus;

			switch (transferTypeEnum)
			{
				case PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskCancelled:
					taskStatus = TaskState.Cancelled;
					break;
				case PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskCompleted:
					taskStatus = TaskState.Completed;
					break;
				case PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskCreated:
					taskStatus = TaskState.Created;
					break;
				case PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskError:
					taskStatus = TaskState.Error;
					break;
				case PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskStarted:
					taskStatus = TaskState.Started;
					break;
				case PIS.Ground.Core.T2G.WebServices.FileTransfer.taskStateEnum.taskStopped:
					taskStatus = TaskState.Stopped;
					break;
				default: taskStatus = TaskState.Started;
					break;
			}

			return taskStatus;
		}

		/// <summary>Build TaskState.</summary>
		/// <param name="transferTypeEnum">TaskStateEnum of T2G.</param>
		/// <returns>TaskState of Ground</returns>  
		internal static TaskState BuildTaskState(PIS.Ground.Core.T2G.WebServices.Notification.taskStateEnum transferTypeEnum)
		{
			TaskState taskStatus;

			switch (transferTypeEnum)
			{
				case PIS.Ground.Core.T2G.WebServices.Notification.taskStateEnum.taskCancelled:
					taskStatus = TaskState.Cancelled;
					break;
				case PIS.Ground.Core.T2G.WebServices.Notification.taskStateEnum.taskCompleted:
					taskStatus = TaskState.Completed;
					break;
				case PIS.Ground.Core.T2G.WebServices.Notification.taskStateEnum.taskCreated:
					taskStatus = TaskState.Created;
					break;
				case PIS.Ground.Core.T2G.WebServices.Notification.taskStateEnum.taskError:
					taskStatus = TaskState.Error;
					break;
				case PIS.Ground.Core.T2G.WebServices.Notification.taskStateEnum.taskStarted:
					taskStatus = TaskState.Started;
					break;
				case PIS.Ground.Core.T2G.WebServices.Notification.taskStateEnum.taskStopped:
					taskStatus = TaskState.Stopped;
					break;
				default: taskStatus = TaskState.Started;
					break;
			}

			return taskStatus;
		}

		/// <summary>Build TaskPhase.</summary>
		/// <param name="transferTypeEnum">TaskPhaseEnum of T2G.</param>
		/// <returns>TaskPhase of Ground</returns>        
		internal static TaskPhase BuildTaskPhase(PIS.Ground.Core.T2G.WebServices.FileTransfer.taskPhaseEnum transferTypeEnum)
		{
			TaskPhase taskPhase;

			switch (transferTypeEnum)
			{
				case PIS.Ground.Core.T2G.WebServices.FileTransfer.taskPhaseEnum.acquisitionPhase:
					taskPhase = TaskPhase.Acquisition;
					break;
				case PIS.Ground.Core.T2G.WebServices.FileTransfer.taskPhaseEnum.creationPhase:
					taskPhase = TaskPhase.Creation;
					break;
				case PIS.Ground.Core.T2G.WebServices.FileTransfer.taskPhaseEnum.distributionPhase:
					taskPhase = TaskPhase.Distribution;
					break;
				case PIS.Ground.Core.T2G.WebServices.FileTransfer.taskPhaseEnum.transferPhase:
					taskPhase = TaskPhase.Transfer;
					break;
				default: taskPhase = TaskPhase.Creation;
					break;
			}

			return taskPhase;
		}

		/// <summary>Build TaskPhase.</summary>
		/// <param name="transferTypeEnum">TaskPhaseEnum of T2G.</param>
		/// <returns>TaskPhase of Ground</returns>
		internal static TaskPhase BuildTaskPhase(PIS.Ground.Core.T2G.WebServices.Notification.taskPhaseEnum transferTypeEnum)
		{
			TaskPhase taskPhase;

			switch (transferTypeEnum)
			{
				case PIS.Ground.Core.T2G.WebServices.Notification.taskPhaseEnum.acquisitionPhase: taskPhase = TaskPhase.Acquisition;
					break;
				case PIS.Ground.Core.T2G.WebServices.Notification.taskPhaseEnum.creationPhase: taskPhase = TaskPhase.Creation;
					break;
				case PIS.Ground.Core.T2G.WebServices.Notification.taskPhaseEnum.distributionPhase: taskPhase = TaskPhase.Distribution;
					break;
				case PIS.Ground.Core.T2G.WebServices.Notification.taskPhaseEnum.transferPhase: taskPhase = TaskPhase.Transfer;
					break;
				default: taskPhase = TaskPhase.Creation;
					break;
			}

			return taskPhase;
		}

		/// <summary>Build TransferTaskData.</summary>
		/// <param name="transferTaskStruct">TransferTaskStruct of T2G.</param>
		/// <param name="transferTaskData">TransferTaskData of Ground.</param>
		internal static TransferTaskData BuildTransferTaskData(PIS.Ground.Core.T2G.WebServices.FileTransfer.transferTaskStruct transferTaskStruct)
		{
			TransferTaskData transferTaskData = new TransferTaskData();

			if (transferTaskStruct != null)
			{
				transferTaskData.AcquisitionCompletionPercent = transferTaskStruct.acquisitionCompletionPercent;
				transferTaskData.ActiveFileTransferCount = transferTaskStruct.activeFileTransferCount;
				transferTaskData.AutomaticallyStop = transferTaskStruct.automaticallyStop;
				transferTaskData.CompletionDate = transferTaskStruct.completionDate;
				transferTaskData.CreationDate = transferTaskStruct.creationDate;
				transferTaskData.Creator = transferTaskStruct.creator;
				transferTaskData.Description = transferTaskStruct.description;
				transferTaskData.DistributionCompletionPercent = transferTaskStruct.distributionCompletionPercent;
				transferTaskData.ErrorCount = transferTaskStruct.errorCount;
				transferTaskData.ExpirationDate = transferTaskStruct.expirationDate;
				transferTaskData.FolderId = transferTaskStruct.folderId;
				transferTaskData.FolderSystemId = transferTaskStruct.folderSystemId;
				transferTaskData.ForeignTaskId = transferTaskStruct.foreignTaskId;

				transferTaskData.LinkType = BuildFileTransferMode(transferTaskStruct.linkType);

				transferTaskData.Priority = transferTaskStruct.priority;
				transferTaskData.StartDate = transferTaskStruct.startDate;
				transferTaskData.TaskId = transferTaskStruct.taskId;

				transferTaskData.TaskPhase = BuildTaskPhase(transferTaskStruct.taskPhase);
				transferTaskData.TaskState = BuildTaskState(transferTaskStruct.taskState);

				transferTaskData.TaskSystemId = transferTaskStruct.taskSystemId;
				transferTaskData.TransferCompletionPercent = transferTaskStruct.transferCompletionPercent;
				transferTaskData.TransferNotifURL = transferTaskStruct.transferNotifURL;
			}

			return transferTaskData;
		}

		/// <summary>Builds available element data.</summary>
		/// <param name="system">The system.</param>
		/// <returns>.</returns>
		internal static AvailableElementData BuildAvailableElementData(SystemInfo system)
		{
			AvailableElementData elementData = null;

			if (system != null)
			{
				elementData = new AvailableElementData();

				elementData.OnlineStatus = system.IsOnline;
				elementData.PisBaselineData = new PisBaseline(system.PisBaseline); // deep copy
				elementData.ElementNumber = system.SystemId;

				if (system.PisMission != null)
				{
					elementData.MissionCommercialNumber = system.PisMission.CommercialNumber;
					elementData.MissionOperatorCode = system.PisMission.OperatorCode;
					elementData.MissionState = system.PisMission.MissionState;
				}
				else
				{
					elementData.MissionCommercialNumber = string.Empty;
					elementData.MissionOperatorCode = string.Empty;
					elementData.MissionState = MissionStateEnum.NI;
				}

				if (elementData.PisBaselineData != null)
				{
					elementData.PisBasicPackageVersion = elementData.PisBaselineData.CurrentVersionPisBaseOut;
					elementData.LmtPackageVersion = elementData.PisBaselineData.CurrentVersionLmtOut;
				}
				else
				{
					elementData.PisBasicPackageVersion = string.Empty;
					elementData.LmtPackageVersion = string.Empty;
				}
			}

			return elementData;
		}
	}
}
