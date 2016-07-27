// <copyright file="InstantMessageService.svc.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using PIS.Ground.Common;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.PackageAccess;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.Core.Utility;
using PIS.Ground.RemoteDataStore;
using PIS.Train.InstantMessage;

namespace PIS.Ground.InstantMessage
{
	/// <summary>Instant message service.</summary>
	[CreateOnDispatchService(typeof(InstantMessageService))]
	[ServiceBehavior(Namespace = "http://alstom.com/pacis/pis/ground/instantmessage/", InstanceContextMode=InstanceContextMode.Single, ConcurrencyMode=ConcurrencyMode.Multiple)]
	public class InstantMessageService : IInstantMessageService, IDisposable
	{
		#region Constants

		/// <summary>
		/// Maximum allowed request timeout (in minutes).
		/// </summary>
		private const uint MaxRequestTimeout = 43200;

		/// <summary>
		/// Maximum allowed scheduled message text size (in bytes).
		/// </summary>
		private const uint MaxScheduledMessageTextSize = 0x4000; // 16K

		/// <summary>
		/// Maximum allowed free-text message text size (in bytes).
		/// </summary>
		private const uint MaxFreeTextMessageTextSize = 0x0100; // 256

		/// <summary>
		/// Maximum allowed free-text message repetitions.
		/// </summary>
		private const uint MaxFreeTextMessageRepetitions = 255;

		/// <summary>Identifier for event subscription.</summary>
		private const string SubscriberId = "PIS.Ground.InstantMessage.InstantMessageService";

		#endregion

		#region Private fields

        //// These static fields are required because many instances of instant message service are created.
        //// It's easier to use static field than performing a complete refactoring.
        #region Static field

        /// <summary>
        /// Lock on initialization data.
        /// </summary>
        private static object _initializationLock = new object();

        /// <summary>
        /// The number of object that refer to static fields.
        /// </summary>
        private static uint _initializationCount = 0;

        /// <summary>
        /// The instant message service instance that initialized the static data.
        /// </summary>
        private static InstantMessageService _instanceCreator;

        private static AutoResetEvent _transmitEvent = new AutoResetEvent(false);

        private static Thread _transmitThread = null;

        private static IT2GManager _train2groundManager = null;

        private static ISessionManager _sessionManager = null;

        private static INotificationSender _notificationSender = null;

        private static ILogManager _logManager = null;

        private static volatile bool _stopRequested = false;

        /// <summary>
        /// Lock object used to lock _newRequests.
        /// </summary>
        private static object _lockNewRequests = new object();

        private static List<InstantMessageRequestContext> _newRequests = new List<InstantMessageRequestContext>();

        #endregion

		#endregion

		/// <summary>Initializes a new instance of the InstantMessageService class.</summary>
		public InstantMessageService()
		{
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "InstantMessageService";
            }

			Initialize(this);
		}

		/// <summary>Initializes a new instance of the InstantMessageService class to be used for unitests purpose.</summary>
		/// <param name="sessionManager">Manager for session.</param>
		/// <param name="notificationSender">The notification sender.</param>
		/// <param name="train2groungManager">Manager for 2g.</param>
		/// <param name="logManager">Manager for log.</param>
		internal InstantMessageService(
			ISessionManager sessionManager,
			INotificationSender notificationSender,
			IT2GManager train2groungManager,
			ILogManager logManager)
		{
			Initialize(this, sessionManager, notificationSender, train2groungManager, logManager);
		}

		/// <summary>
		/// Initializes the InstantMessage web service. This is only called once, when the service is instantiated.
		/// </summary>		
        /// <param name="instance">The instance that invoke this method. Permits to keep alive the object.</param>
        private static void Initialize(InstantMessageService instance)
        {
            lock (_initializationLock)
            {
                if (_initializationCount == 0)
                {
                    try
                    {
                        _instanceCreator = instance;
                        _newRequests.Clear();
                        _stopRequested = false;
                        _sessionManager = new SessionManager();

                        _notificationSender = new NotificationSender(_sessionManager);

                        _train2groundManager = T2GManagerContainer.T2GManager;

                        HistoryLogger.Initialize();
                        HistoryLogger.MarkPendingMessagesAsCanceledByStartup();

                        _logManager = new LogManager();

                        _train2groundManager.SubscribeToElementChangeNotification(SubscriberId, new EventHandler<ElementEventArgs>(_instanceCreator.OnElementInfoChanged));

                        _transmitThread = new Thread(new ThreadStart(_instanceCreator.OnTransmitEvent));
                        _transmitThread.Name = "InstantMsg Transmit";

                        _transmitThread.Start();
                        _initializationCount = 1;
                    }
                    catch (System.Exception e)
                    {
                        LogManager.WriteLog(TraceType.ERROR, e.Message, "PIS.Ground.InstantMessage.InstantMessageService.Initialize", e, EventIdEnum.InstantMessage);
                        Uninitialize(true);
                    }
                }
                else
                {
                    _initializationCount++;
                }
            }
        }

		/// <summary>Uninitializes this object.</summary>
        /// <param name="force">Indicates if the uninitialization shall be forced.</param>
		private static void Uninitialize(bool force)
		{
			lock (_initializationLock)
			{
                if (_initializationCount < 2 || force)
				{
					if (_transmitThread != null)
					{
						_stopRequested = true;
						_transmitEvent.Set();
					}

					if (_train2groundManager != null)
					{
						_train2groundManager.UnsubscribeFromElementChangeNotification(SubscriberId);
						_train2groundManager = null;
					}

					if (_transmitThread != null)
					{
						_transmitThread.Join();
						_transmitThread = null;
						_transmitEvent.Reset();
					}

					_sessionManager = null;
					_notificationSender = null;
                    _instanceCreator = null;
				}

                lock (_lockNewRequests)
                {
                    _newRequests.Clear();
                }

			}

            if (_initializationCount != 0 && !force)
            {
                _initializationCount--;
            }
        }

		/// <summary>
		/// Initializes the InstantMessage web service. This is only called once, when the service is instantiated.
		/// This version should be used for automated tests purpose only.
		/// </summary>
        /// <param name="creator">The instance of InstantMessageService class that call this method.</param>
		/// <param name="sessionManager">Manager for session.</param>
		/// <param name="notificationSender">The notification sender.</param>
		/// <param name="train2groungManager">Manager for 2g.</param>
		/// <param name="logManager">Manager for log.</param>
		private static void Initialize(
            InstantMessageService creator,
			ISessionManager sessionManager,
			INotificationSender notificationSender,
			IT2GManager train2groungManager,
			ILogManager logManager)
		{
			try
			{
				if (_initializationCount != 0)
				{
					Uninitialize(true);
				}
                else
                {
                    _newRequests.Clear();
                }

                _instanceCreator = creator;
				_sessionManager = sessionManager;
                _stopRequested = false;

				_notificationSender = notificationSender;

				_train2groundManager = train2groungManager;

				_logManager = logManager;

				_train2groundManager.SubscribeToElementChangeNotification(SubscriberId, new EventHandler<ElementEventArgs>(_instanceCreator.OnElementInfoChanged));

				_transmitThread = new Thread(new ThreadStart(_instanceCreator.OnTransmitEvent));
                _transmitThread.Name = "InstantMsg Transmit";

				_transmitThread.Start();

                _initializationCount++;
			}
			catch (System.Exception e)
			{
				_logManager.WriteLog(TraceType.ERROR, e.Message, "PIS.Ground.InstantMessage.InstantMessageService.Initialize", e, EventIdEnum.InstantMessage);
				Uninitialize(true);
			}
		}

		/// <summary> Callback called when Element Online state changes (signaled by the T2G Client).</summary>
		/// <param name="sender">Source of the event.</param>
		/// <param name="args">Event information to send to registered event handlers.</param>
		private void OnElementInfoChanged(object sender, ElementEventArgs args)
		{
			// Signal the event to start handling the request.
			_transmitEvent.Set();
		}

		/// <summary>Convert data type ground to train.</summary>
		/// <param name="objectIn">The object in.</param>
		/// <returns>The data converted type ground to train.</returns>
		private static PIS.Train.InstantMessage.FreeTextMessageType ConvertDataTypeGroundToTrain(PIS.Ground.InstantMessage.FreeTextMessageType objectIn)
		{
			PIS.Train.InstantMessage.FreeTextMessageType objectOut = new PIS.Train.InstantMessage.FreeTextMessageType();
			objectOut.AttentionGetterPresence = objectIn.AttentionGetter;
			objectOut.DelayBetweenRepetitions = objectIn.DelayBetweenRepetitions;
			objectOut.DisplayDuration = objectIn.DisplayDuration;
			objectOut.FreeText = objectIn.FreeText;
			objectOut.Identifier = objectIn.Identifier;
			objectOut.NumberOfRepetitions = objectIn.NumberOfRepetitions;
			return objectOut;
		}

		/// <summary>Convert data type ground to train.</summary>
		/// <param name="objectIn">The object in.</param>
		/// <returns>The data converted type ground to train.</returns>
		private static PIS.Train.InstantMessage.ScheduledPeriodType ConvertDataTypeGroundToTrain(PIS.Ground.InstantMessage.ScheduledPeriodType objectIn)
		{
			PIS.Train.InstantMessage.ScheduledPeriodType objectOut = new PIS.Train.InstantMessage.ScheduledPeriodType();
			objectOut.StartDateTime = objectIn.StartDateTime;
			objectOut.EndDateTime = objectIn.EndDateTime;
			return objectOut;
		}

		/// <summary>Convert data type ground to train.</summary>
		/// <param name="objectIn">The object in.</param>
		/// <returns>The data converted type ground to train.</returns>
		private static PIS.Train.InstantMessage.ScheduledMessageType ConvertDataTypeGroundToTrain(PIS.Ground.InstantMessage.ScheduledMessageType objectIn)
		{
			PIS.Train.InstantMessage.ScheduledMessageType objectOut = new PIS.Train.InstantMessage.ScheduledMessageType();
			objectOut.FreeText = objectIn.FreeText;
			objectOut.Identifier = objectIn.Identifier;
			objectOut.Period = ConvertDataTypeGroundToTrain(objectIn.Period);
			return objectOut;
		}

		/// <summary>Convert data type ground to train.</summary>
		/// <param name="request">The request.</param>
		/// <param name="objectIn">The object in.</param>
		/// <returns>The data converted type ground to train.</returns>
		private static PIS.Train.InstantMessage.PredefinedMessageType ConvertDataTypeGroundToTrain(
			ProcessSendPredefMessagesRequestContext request,
			PIS.Ground.InstantMessage.PredefinedMessageType objectIn)
		{
			PIS.Train.InstantMessage.PredefinedMessageType objectOut = new PIS.Train.InstantMessage.PredefinedMessageType();

			objectOut.Identifier = objectIn.Identifier;

			if (objectIn.CarId != null)
			{
				objectOut.CarId = objectIn.CarId.Value;
				objectOut.CarIdSpecified = true;
			}
			else
			{
				objectOut.CarId = 0;
				objectOut.CarIdSpecified = false;
			}

			if (objectIn.Delay != null)
			{
				objectOut.Delay = objectIn.Delay.Value;
				objectOut.DelaySpecified = true;
			}
			else
			{
				objectOut.Delay = 0;
				objectOut.DelaySpecified = false;
			}

			if (!string.IsNullOrEmpty(objectIn.DelayReason))
			{
				objectOut.DelayReason = objectIn.DelayReason;
			}
			else
			{
				objectOut.DelayReason = string.Empty;
			}

			if (objectIn.Hour != null)
			{
				objectOut.Hour = objectIn.Hour.Value;
				objectOut.HourSpecified = true;
			}
			else
			{
				objectOut.Hour = new DateTime();
				objectOut.HourSpecified = false;
			}

			objectOut.StationId = 0;
			objectOut.StationIdSpecified = false;

			if (!string.IsNullOrEmpty(objectIn.StationId) && request.StationInternalIdDictionary.ContainsKey(objectIn.StationId))
			{
				objectOut.StationId = request.StationInternalIdDictionary[objectIn.StationId];
				objectOut.StationIdSpecified = true;
			}

			return objectOut;
		}

		/// <summary>Convert data type ground to application ground.</summary>
		/// <param name="templateList">List of templates.</param>
		/// <returns>The data converted type ground to application ground.</returns>
		private static PIS.Ground.InstantMessage.MessageTemplateListType ConvertDataTypeGroundToAppGround(List<Template> templateList)
		{
			PIS.Ground.InstantMessage.MessageTemplateListType result = new MessageTemplateListType();
			result.FreeTextMessageTemplateList = new List<FreeTextMessageTemplateType>();
			result.PredefinedMessageTemplateList = new List<PredefinedMessageTemplateType>();
			result.ScheduledMessageTemplateList = new List<ScheduledMessageTemplateType>();

			foreach (var templateIn in templateList)
			{
				if (templateIn.Category == TemplateCategory.FreeText)
				{
					FreeTextMessageTemplateType templateOut = new FreeTextMessageTemplateType();
					templateOut.Identifier = templateIn.ID;
					templateOut.Category = templateIn.Category.ToString();
					templateOut.Class = templateIn.Class.ToString();
					templateOut.DescriptionList = new List<TemplateDescriptionType>();

					foreach (var descriptionIn in templateIn.DescriptionList)
					{
						TemplateDescriptionType descriptionOut = new TemplateDescriptionType();
						descriptionOut.Language = descriptionIn.Language;
						descriptionOut.Text = descriptionIn.Value;
						templateOut.DescriptionList.Add(descriptionOut);
					}

					result.FreeTextMessageTemplateList.Add(templateOut);
				}
				else if (templateIn.Category == TemplateCategory.Predefined)
				{
					PredefinedMessageTemplateType templateOut = new PredefinedMessageTemplateType();
					templateOut.Identifier = templateIn.ID;
					templateOut.Category = templateIn.Category.ToString();
					templateOut.Class = templateIn.Class.ToString();
					templateOut.DescriptionList = new List<TemplateDescriptionType>();

					foreach (var descriptionIn in templateIn.DescriptionList)
					{
						TemplateDescriptionType descriptionOut = new TemplateDescriptionType();
						descriptionOut.Language = descriptionIn.Language;
						descriptionOut.Text = descriptionIn.Value;
						templateOut.DescriptionList.Add(descriptionOut);
					}

					templateOut.ParameterList = new List<PredefinedMessageTemplateParameterType>();

					foreach (var parameterIn in templateIn.ParameterList)
					{
						switch (parameterIn)
						{
							case TemplateParameterType.CarNumber:
								templateOut.ParameterList.Add(PredefinedMessageTemplateParameterType.CarId);
								break;
							case TemplateParameterType.Delay:
								templateOut.ParameterList.Add(PredefinedMessageTemplateParameterType.Delay);
								break;
							case TemplateParameterType.DelayReasonCode:
								templateOut.ParameterList.Add(PredefinedMessageTemplateParameterType.DelayReason);
								break;
							case TemplateParameterType.Hour:
								templateOut.ParameterList.Add(PredefinedMessageTemplateParameterType.Hour);
								break;
							case TemplateParameterType.StationId:
								templateOut.ParameterList.Add(PredefinedMessageTemplateParameterType.StationId);
								break;
						}
					}

					result.PredefinedMessageTemplateList.Add(templateOut);
				}
				else if (templateIn.Category == TemplateCategory.Scheduled)
				{
					ScheduledMessageTemplateType templateOut = new ScheduledMessageTemplateType();
					templateOut.Identifier = templateIn.ID;

					templateOut.Category = templateIn.Category.ToString();
					templateOut.Class = templateIn.Class.ToString();
					templateOut.DescriptionList = new List<TemplateDescriptionType>();

					foreach (var descriptionIn in templateIn.DescriptionList)
					{
						TemplateDescriptionType descriptionOut = new TemplateDescriptionType();
						descriptionOut.Language = descriptionIn.Language;
						descriptionOut.Text = descriptionIn.Value;
						templateOut.DescriptionList.Add(descriptionOut);
					}

					result.ScheduledMessageTemplateList.Add(templateOut);
				}
			}

			return result;
		}

		/// <summary>Convert data type ground to application ground.</summary>
		/// <param name="stationList">List of stations.</param>
		/// <returns>The data converted type ground to application ground.</returns>
		private static List<PIS.Ground.InstantMessage.StationType> ConvertDataTypeGroundToAppGround(List<PIS.Ground.Core.PackageAccess.Station> stationList)
		{
			List<PIS.Ground.InstantMessage.StationType> result = new List<PIS.Ground.InstantMessage.StationType>();

			foreach (var stationIn in stationList)
			{
				PIS.Ground.InstantMessage.StationType stationOut = new PIS.Ground.InstantMessage.StationType();
				stationOut.OperatorCode = stationIn.OperatorCode;

				stationOut.NameList = new List<PIS.Ground.InstantMessage.StationNameType>();

				foreach (var nameIn in stationIn.Names)
				{
					PIS.Ground.InstantMessage.StationNameType nameOut = new PIS.Ground.InstantMessage.StationNameType();
					nameOut.Language = nameIn.Language;
					nameOut.Name = nameIn.Name;
					stationOut.NameList.Add(nameOut);
				}

				result.Add(stationOut);
			}

			return result;
		}

		/// <summary>Searches for the first template list path.</summary>
		/// <param name="packagePath">Full pathname of the package file.</param>
		/// <returns>The found template list path.</returns>
		private string FindTemplateListPath(string packagePath)
		{
			string templateListFilePath = string.Empty;
			try
			{
				string[] fileNames = Directory.GetFiles(packagePath, "Predef.lua", SearchOption.AllDirectories);
				if (fileNames != null && fileNames.Count() > 0)
				{
					templateListFilePath = fileNames[0]; // select the first
				}
			}
			catch (Exception ex)
			{
				templateListFilePath = string.Empty;
				_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.FindTemplateListPath", ex, EventIdEnum.InstantMessage);
			}

			return templateListFilePath;
		}

		/// <summary>List package lua scripts.</summary>
		/// <param name="scriptsList">List of scripts.</param>
		/// <param name="packagePath">Full pathname of the package file.</param>
		/// <returns>The number of scripts found.</returns>
		private int ListPackageLuaScripts(List<string> scriptsList, string packagePath)
		{
			int count = 0;
			try
			{
				string[] fileNames = Directory.GetFiles(packagePath, "*.lua", SearchOption.AllDirectories);

				if (fileNames != null && fileNames.Count() > 0)
				{
					scriptsList.AddRange(fileNames);
					count = scriptsList.Count();
				}
			}
			catch (Exception ex)
			{
				count = 0;
				_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.ListPackageLuaScripts", ex, EventIdEnum.InstantMessage);
			}

			return count;
		}

		/// <summary>Gets lua scripts list.</summary>
		/// <param name="remoteDataStore">The remote data store.</param>
		/// <param name="element">The element.</param>
		/// <param name="luaScriptsList">List of lua scripts.</param>
		/// <returns>The lua scripts list.</returns>
		protected virtual string GetLuaScriptsList(RemoteDataStoreProxy remoteDataStore, AvailableElementData element, List<string> luaScriptsList)
		{
			string templateFilePath = string.Empty;

			if (remoteDataStore != null &&
				element != null && element.PisBaselineData != null &&
				remoteDataStore.checkIfElementExists(element.ElementNumber))
			{
				string pisBaseVersion = GetElementCurrentPisBasePackageVersion(element);

				if (remoteDataStore.checkIfDataPackageExists("PISBASE", pisBaseVersion))
				{
					var openPisBasePackageResult = remoteDataStore.openLocalDataPackage(
						"PISBASE",
						pisBaseVersion,
						@"^*\.lua$");

					if (openPisBasePackageResult.Status == OpenDataPackageStatusEnum.COMPLETED)
					{
						templateFilePath = FindTemplateListPath(openPisBasePackageResult.LocalPackagePath);

						if (!string.IsNullOrEmpty(templateFilePath))
						{
							int scriptsCount = ListPackageLuaScripts(luaScriptsList, openPisBasePackageResult.LocalPackagePath);

							string pisMissionVersion = GetElementCurrentPisMissionPackageVersion(element);

							if (remoteDataStore.checkIfDataPackageExists("PISMISSION", pisMissionVersion))
							{
								var openPisMissionPackageResult = remoteDataStore.openLocalDataPackage(
									"PISMISSION",
									pisMissionVersion,
									@"^*\.lua$");

								if (openPisMissionPackageResult.Status == OpenDataPackageStatusEnum.COMPLETED)
								{
									int n = ListPackageLuaScripts(luaScriptsList, openPisMissionPackageResult.LocalPackagePath);
									scriptsCount += n;
								}
								else
								{
									string logMessage = "Cannot open PISMISSION package!";
									_logManager.WriteLog(TraceType.ERROR, logMessage, "PIS.Ground.InstantMessage.GetLuaScriptsList", null, EventIdEnum.InstantMessage);
								}
							}
							else
							{
								string logMessage = "PISMISSION package doesn't exist.";
								_logManager.WriteLog(TraceType.ERROR, logMessage, "PIS.Ground.InstantMessage.GetLuaScriptsList", null, EventIdEnum.InstantMessage);
							}
						}
						else
						{
							string logMessage = "Template file not found!";
							_logManager.WriteLog(TraceType.ERROR, logMessage, "PIS.Ground.InstantMessage.GetLuaScriptsList", null, EventIdEnum.InstantMessage);
						}
					}
					else
					{
						string logMessage = "Cannot open PISBASE package!";
						_logManager.WriteLog(TraceType.ERROR, logMessage, "PIS.Ground.InstantMessage.GetLuaScriptsList", null, EventIdEnum.InstantMessage);
					}
				}
				else
				{
					string logMessage = "PISBASE package doesn't exist!";
					_logManager.WriteLog(TraceType.ERROR, logMessage, "PIS.Ground.InstantMessage.GetLuaScriptsList", null, EventIdEnum.InstantMessage);
				}
			}

			return templateFilePath;
		}

		/// <summary>Searches for the first lmt database file path.</summary>
		/// <param name="packagePath">Full pathname of the package file.</param>
		/// <returns>The found lmt database file path.</returns>
		private string FindLmtDatabaseFilePath(string packagePath)
		{
			string lmtDatabaseFilePath = string.Empty;
			try
			{
				string[] fileNames = Directory.GetFiles(packagePath, "*.db", SearchOption.AllDirectories);
				if (fileNames != null && fileNames.Count() > 0)
				{
					lmtDatabaseFilePath = fileNames[0]; // select the first
				}
			}
			catch (Exception ex)
			{
				lmtDatabaseFilePath = string.Empty;
				_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.FindLmtDatabaseFilePath", ex, EventIdEnum.InstantMessage);
			}

			return lmtDatabaseFilePath;
		}

		/// <summary>Query if 'elementId' is element known by datastore.</summary>
		/// <exception cref="PisDataStoreNotAccessibleException">Thrown when the Pis Data Store Not
		/// Accessible error condition occurs.</exception>
		/// <param name="elementId">Identifier for the element.</param>
		/// <returns>true if element known by datastore, false if not.</returns>
		protected virtual bool IsElementKnownByDatastore(string elementId)
		{
			bool elementIsKnown = false;

			try
			{
				using (var remoteDataStore = new RemoteDataStoreProxy())
				{
					elementIsKnown = remoteDataStore.checkIfElementExists(elementId);
				}
			}
			catch (CommunicationException ex)
			{
				throw new PisDataStoreNotAccessibleException("PIS.Ground.InstantMessage.InstantMessageService.IsElementKnown", ex);
			}
			catch (Exception ex)
			{
				_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.IsElementKnown", ex, EventIdEnum.InstantMessage);
			}

			return elementIsKnown;
		}

		protected virtual bool IsDataPackageKnownByDatastore(string packageType, string packageVersion)
		{
			bool dataPackageIsKnown = false;
			try
			{
				if (!string.IsNullOrEmpty(packageType) && !string.IsNullOrEmpty(packageVersion))
				{
					using (var remoteDataStore = new RemoteDataStoreProxy())
					{
						dataPackageIsKnown = remoteDataStore.checkIfDataPackageExists(packageType, packageVersion);
					}
				}
			}
			catch (CommunicationException ex)
			{
				throw new PisDataStoreNotAccessibleException("PIS.Ground.InstantMessage.InstantMessageService.IsDataPackageKnownByDatastore", ex);
			}
			catch (Exception ex)
			{
				_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.IsDataPackageKnownByDatastore", ex, EventIdEnum.InstantMessage);
			}

			return dataPackageIsKnown;
		}

		private static string GetElementCurrentPisBasePackageVersion(AvailableElementData element)
		{
			string version = string.Empty;

			if (element != null)
			{
				if (element.PisBaselineData != null)
				{
					if (!string.IsNullOrEmpty(element.PisBaselineData.CurrentVersionPisBaseOut))
					{
						version = element.PisBaselineData.CurrentVersionPisBaseOut;
					}
				}
			}

			return version;
		}

		private static string GetElementCurrentLmtPackageVersion(AvailableElementData element)
		{
			string version = string.Empty;

			if (element != null)
			{
				if (element.PisBaselineData != null)
				{
					if (!string.IsNullOrEmpty(element.PisBaselineData.CurrentVersionLmtOut))
					{
						version = element.PisBaselineData.CurrentVersionLmtOut;
					}
				}
			}

			return version;
		}

		private static string GetElementCurrentPisMissionPackageVersion(AvailableElementData element)
		{
			string version = string.Empty;

			if (element != null)
			{
				if (element.PisBaselineData != null)
				{
					if (!string.IsNullOrEmpty(element.PisBaselineData.CurrentVersionPisMissionOut))
					{
						version = element.PisBaselineData.CurrentVersionPisMissionOut;
					}
				}
			}

			return version;
		}

		private static InstantMessageErrorEnum GetInvalidTargetAddressResponse(TargetAddressType targetAddress)
		{
			InstantMessageErrorEnum result = InstantMessageErrorEnum.UnknownElementId;

			if (targetAddress != null)
			{
				switch (targetAddress.Type)
				{
					case AddressTypeEnum.Element:
						result = InstantMessageErrorEnum.UnknownElementId;
						break;
					case AddressTypeEnum.MissionCode:
						result = InstantMessageErrorEnum.UnknownMissionId;
						break;
					case AddressTypeEnum.MissionOperatorCode:
						result = InstantMessageErrorEnum.UnknownMissionId;
						break;
				}
			}

			return result;
		}

		/// <summary>Adds a new request to request list.</summary>
		/// <param name="newRequests">The new requests.</param>
		protected virtual void AddNewRequestToRequestList(List<InstantMessageRequestContext> newRequests)
		{
			lock (_lockNewRequests)
			{
				_newRequests.AddRange(newRequests);

				// Signal thread and start transmitting
				_transmitEvent.Set();
			}
		}

#region "Web service methods"
		/// <summary>
		/// InstantMessageService web service method "GetAvailableElementList".
		/// </summary>
		/// <param name="sessionId">The session identifier.</param>
		/// <returns>Response <see cref="InstantMessageElementListResult"/>.</returns>
		public InstantMessageElementListResult GetAvailableElementList(Guid sessionId)
		{
			var result = new InstantMessageElementListResult();
			result.ResultCode = InstantMessageErrorEnum.ElementListNotAvailable;

			SessionData sessionData;
			_sessionManager.GetSessionDetails(sessionId, out sessionData);
			if (sessionData != null)
			{
				try
				{
					T2GManagerErrorEnum train2groundResult = _train2groundManager.GetAvailableElementDataList(out result.ElementList);

					if (train2groundResult == T2GManagerErrorEnum.eSuccess)
					{
						result.ResultCode = InstantMessageErrorEnum.RequestAccepted;
					}
					else
					{
						result.ResultCode = InstantMessageErrorEnum.ElementListNotAvailable;
					}
				}
				catch (Exception ex)
				{
					_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.GetAvailableElementList", ex, EventIdEnum.InstantMessage);
				}
			}
			else
			{
				result.ResultCode = InstantMessageErrorEnum.InvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// InstantMessageService Web service method "RetrieveMessageTemplateList" that returns the template list to use with addressee.
		/// </summary>
		/// <param name="sessionId">The session identifier.</param>
		/// <param name="targetAddress">The targeted addressee information.</param>
		/// <returns>Response <see cref="InstantMessageTemplateListResult"/>.</returns>
		InstantMessageTemplateListResult IInstantMessageService.RetrieveMessageTemplateList(
			Guid sessionId,
			TargetAddressType targetAddress)
		{
			var result = new InstantMessageTemplateListResult();
			result.ResultCode = InstantMessageErrorEnum.TemplateListNotAvailable;
			result.MessageTemplateList = new MessageTemplateListType();

			SessionData sessionData;
			_sessionManager.GetSessionDetails(sessionId, out sessionData);

			if (sessionData != null)
			{
				ElementList<AvailableElementData> elements;
				T2GManagerErrorEnum rqstResult = _train2groundManager.GetAvailableElementDataByTargetAddress(targetAddress, out elements);

				switch (rqstResult)
				{
					case T2GManagerErrorEnum.eSuccess:
						{
							List<Template> globalTemplateList = new List<Template>();
							List<string> luaScriptsList = new List<string>();

							try
							{
								foreach (AvailableElementData element in elements)
								{
									using (var remoteDataStore = new RemoteDataStoreProxy())
									{
										string templateFilePath = GetLuaScriptsList(remoteDataStore, element, luaScriptsList);

										if (!string.IsNullOrEmpty(templateFilePath))
										{
											using (var templateAccessor = new TemplateListAccessor())
											{
												if (templateAccessor.ExecuteTemplate(templateFilePath, luaScriptsList) == true)
												{
													List<Template> templateList = templateAccessor.GetAllTemplates();

													foreach (var template in templateList)
													{
														if (!globalTemplateList.Exists(item => item.ID == template.ID))
														{
															globalTemplateList.Add(template);
														}
													}
												}
												else
												{
													result.ResultCode = InstantMessageErrorEnum.TemplateFileNotValid;
													break;
												}
											}
										}
										else
										{
											result.ResultCode = InstantMessageErrorEnum.TemplateFileNotFound;
											break;
										}
									}
								}

								if ((result.ResultCode != InstantMessageErrorEnum.TemplateFileNotFound) &&
									(result.ResultCode != InstantMessageErrorEnum.TemplateFileNotValid) &&
									(globalTemplateList.Count > 0))
								{
									try
									{
										result.MessageTemplateList = ConvertDataTypeGroundToAppGround(globalTemplateList);
										result.ResultCode = InstantMessageErrorEnum.RequestAccepted;
									}
									catch (Exception ex)
									{
										_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.RetrieveMessageTemplateList", ex, EventIdEnum.InstantMessage);
									}
								}
							}
							catch (PisDataStoreNotAccessibleException ex)
							{
								result.MessageTemplateList = new MessageTemplateListType();
								result.ResultCode = InstantMessageErrorEnum.DatastoreNotAccessible;

								_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.RetrieveMessageTemplateList", ex, EventIdEnum.InstantMessage);
							}
						}

						break;
					case T2GManagerErrorEnum.eT2GServerOffline:
						result.ResultCode = InstantMessageErrorEnum.T2GServerOffline;
						break;
					case T2GManagerErrorEnum.eElementNotFound:
						result.ResultCode = GetInvalidTargetAddressResponse(targetAddress);
						break;
					default:
						break;
				}
			}
			else
			{
				result.ResultCode = InstantMessageErrorEnum.InvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// InstantMessageService Web service method "RetrieveStationList" that returns the station list to use with addressee.
		/// </summary>
		/// <param name="sessionId">The session identifier.</param>
		/// <param name="targetAddress">The targeted addressee information.</param>
		/// <returns>Response <see cref="InstantMessageStationListResult"/>.</returns>
		InstantMessageStationListResult IInstantMessageService.RetrieveStationList(
			Guid sessionId,
			TargetAddressType targetAddress)
		{
			var result = new InstantMessageStationListResult();
			result.ResultCode = InstantMessageErrorEnum.StationListNotAvailable;
			result.InstantMessageStationList = new List<StationType>();

			SessionData sessionData;
			_sessionManager.GetSessionDetails(sessionId, out sessionData);

			if (sessionData != null)
			{
				ElementList<AvailableElementData> elements;
				T2GManagerErrorEnum rqstResult = _train2groundManager.GetAvailableElementDataByTargetAddress(targetAddress, out elements);
				switch (rqstResult)
				{
					case T2GManagerErrorEnum.eSuccess:
						{
							List<PIS.Ground.Core.PackageAccess.Station> globalStationList = new List<PIS.Ground.Core.PackageAccess.Station>();
							try
							{
								foreach (AvailableElementData element in elements)
								{
									if (element.PisBaselineData != null && !string.IsNullOrEmpty(element.PisBaselineData.CurrentVersionLmtOut))
									{
										string lmtVersion = element.PisBaselineData.CurrentVersionLmtOut;

										using (var remoteDataStore = new RemoteDataStoreProxy())
										{
											if (remoteDataStore.checkIfElementExists(element.ElementNumber))
											{
												if (remoteDataStore.checkIfDataPackageExists("LMT", lmtVersion))
												{
													var openPackageResult = remoteDataStore.openLocalDataPackage(
														"LMT",
														lmtVersion,
														String.Empty);

													if (openPackageResult.Status == OpenDataPackageStatusEnum.COMPLETED)
													{
														string lmtDatabaseFilePath = FindLmtDatabaseFilePath(openPackageResult.LocalPackagePath);
														if (!string.IsNullOrEmpty(lmtDatabaseFilePath))
														{
															using (var lmtDatabaseAccessor = new LmtDatabaseAccessor(lmtDatabaseFilePath))
															{
																List<PIS.Ground.Core.PackageAccess.Station> stationList = lmtDatabaseAccessor.GetStationList();

																foreach (var station in stationList)
																{
																	if (!globalStationList.Exists(item => item.OperatorCode == station.OperatorCode))
																	{
																		globalStationList.Add(station);
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}

								if (globalStationList.Count > 0)
								{
									try
									{
										result.InstantMessageStationList = ConvertDataTypeGroundToAppGround(globalStationList);
										result.ResultCode = InstantMessageErrorEnum.RequestAccepted;
									}
									catch (Exception ex)
									{
										_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.RetrieveStationList", ex, EventIdEnum.InstantMessage);
									}
								}
							}
							catch (PisDataStoreNotAccessibleException ex)
							{
								result.InstantMessageStationList = new List<StationType>();
								result.ResultCode = InstantMessageErrorEnum.DatastoreNotAccessible;

								_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.RetrieveStationList", ex, EventIdEnum.InstantMessage);
							}
						}

						break;
					case T2GManagerErrorEnum.eT2GServerOffline:
						result.ResultCode = InstantMessageErrorEnum.T2GServerOffline;
						break;
					case T2GManagerErrorEnum.eElementNotFound:
						result.ResultCode = GetInvalidTargetAddressResponse(targetAddress);
						break;
					default:
						break;
				}
			}
			else
			{
				result.ResultCode = InstantMessageErrorEnum.InvalidSessionId;
			}

			return result;
		}

		/// <summary>
		/// InstantMessageService Web service method "SendPredefinedMessages" that sends predefined messages to addressee.
		/// </summary>
		/// <param name="sessionId">The session identifier.</param>
		/// <param name="targetAddress">The targeted addressee information.</param>
		/// <param name="requestTimeout">Request timeout.</param>
		/// <param name="messages">Predefined messages.</param>
		/// <returns>Response <see cref="InstantMessageResult"/>.</returns>
		InstantMessageResult IInstantMessageService.SendPredefinedMessages(
			Guid sessionId,
			TargetAddressType targetAddress,
			uint? requestTimeout,
			PredefinedMessageType[] messages)
		{
			var result = new InstantMessageResult();
			result.RequestId = Guid.Empty;
			result.ResultCode = InstantMessageErrorEnum.RequestAccepted;

			try
			{
				if (requestTimeout != null && requestTimeout.Value <= MaxRequestTimeout)
				{
					SessionData sessionData;
					_sessionManager.GetSessionDetails(sessionId, out sessionData);

					if (sessionData != null)
					{
						Guid requestId = Guid.Empty;
						_sessionManager.GenerateRequestID(sessionId, out requestId);

						if (requestId != Guid.Empty)
						{
							ElementList<AvailableElementData> elements;
							T2GManagerErrorEnum requestResult = _train2groundManager.GetAvailableElementDataByTargetAddress(targetAddress, out elements);
							switch (requestResult)
							{
								case T2GManagerErrorEnum.eSuccess:
									{
										result.RequestId = requestId;
										result.ResultCode = InstantMessageErrorEnum.RequestAccepted;

										List<InstantMessageRequestContext> newRequests = new List<InstantMessageRequestContext>(elements.Count);

										foreach (AvailableElementData element in elements)
										{
											SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionProcessing, element.ElementNumber);

											ProcessSendPredefMessagesRequestContext request = new ProcessSendPredefMessagesRequestContext(
												element.ElementNumber,
												requestId,
												sessionId,
												requestTimeout.Value,
												messages);

											try
											{
												if (IsElementKnownByDatastore(request.ElementId))
												{
													newRequests.Add(request);
												}
												else
												{
													result.RequestId = Guid.Empty; // discard the generated request ID
                                                    result.ResultCode = InstantMessageErrorEnum.NoBaselineFoundForElementId;
													break;
												}
											}
											catch (PisDataStoreNotAccessibleException ex)
											{
												result.RequestId = Guid.Empty; // discard the generated request ID
												result.ResultCode = InstantMessageErrorEnum.DatastoreNotAccessible;
												_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.SendPredefinedMessages", ex, EventIdEnum.InstantMessage);
												break;
											}
										}

										// Queue to request list
										if (newRequests.Count > 0 && result.ResultCode == InstantMessageErrorEnum.RequestAccepted)
										{
											AddNewRequestToRequestList(newRequests);
										}
									}

									break;
								case T2GManagerErrorEnum.eT2GServerOffline:
									result.ResultCode = InstantMessageErrorEnum.T2GServerOffline;
									break;
								case T2GManagerErrorEnum.eElementNotFound:
									result.ResultCode = GetInvalidTargetAddressResponse(targetAddress);
									break;
								default:
									string errorMessage = string.Format(CultureInfo.CurrentCulture, InstantMessage.Properties.Resources.RetrieveAvailableElementErrorWithFormat, requestResult);
									_logManager.WriteLog(TraceType.ERROR, errorMessage, "PIS.Ground.InstantMessage.InstantMessageService.SendPredefinedMessages", null, EventIdEnum.InstantMessage);
									result.ResultCode = InstantMessageErrorEnum.InternalError;
									break;
							}
						}
						else
						{
							_logManager.WriteLog(TraceType.ERROR, InstantMessage.Properties.Resources.GenerateRequestIdentifierError, "PIS.Ground.InstantMessage.InstantMessageService.SendPredefinedMessages", null, EventIdEnum.InstantMessage);
							result.ResultCode = InstantMessageErrorEnum.InternalError;
						}
					}
					else
					{
						result.ResultCode = InstantMessageErrorEnum.InvalidSessionId;
					}
				}
				else
				{
					result.ResultCode = InstantMessageErrorEnum.InvalidRequestTimeout;
				}
			}
			catch (System.Exception ex)
			{
				_logManager.WriteLog(TraceType.EXCEPTION, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.SendPredefinedMessages", ex, EventIdEnum.InstantMessage);
				result.RequestId = Guid.Empty; // discard the generated request ID
				result.ResultCode = InstantMessageErrorEnum.InternalError;
			}

			return result;
		}

		/// <summary>
		/// InstantMessageService Web service method "SendFreeTextMessage" that sends a free-text message to addressee.
		/// </summary>
		/// <param name="sessionId">The session identifier.</param>
		/// <param name="targetAddress">The targeted addressee information.</param>
		/// <param name="requestTimeout">Request timeout.</param>
		/// <param name="message">Free-text message.</param>
		/// <returns>Response <see cref="InstantMessageResult"/>.</returns>
		public InstantMessageResult SendFreeTextMessage(
			Guid sessionId,
			TargetAddressType targetAddress,
			uint? requestTimeout,
			FreeTextMessageType message)
		{
			InstantMessageResult result = new InstantMessageResult();
			result.RequestId = Guid.Empty;
			result.ResultCode = InstantMessageErrorEnum.InvalidSessionId;

			try
			{
				if (requestTimeout != null && requestTimeout.Value <= MaxRequestTimeout)
				{
					SessionData sessionData;
					_sessionManager.GetSessionDetails(sessionId, out sessionData);

					if (sessionData != null)
					{
						Guid requestId = Guid.Empty;
						_sessionManager.GenerateRequestID(sessionId, out requestId);

						if (requestId != Guid.Empty)
						{
							ElementList<AvailableElementData> elements;
							T2GManagerErrorEnum requestResult = _train2groundManager.GetAvailableElementDataByTargetAddress(targetAddress, out elements);
							switch (requestResult)
							{
								case T2GManagerErrorEnum.eSuccess:
									{
										result.RequestId = requestId;
										result.ResultCode = InstantMessageErrorEnum.RequestAccepted;

										List<InstantMessageRequestContext> newRequests = new List<InstantMessageRequestContext>(elements.Count);

										foreach (AvailableElementData element in elements)
										{
											SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionProcessing, element.ElementNumber);

											ProcessSendFreeTextMessageRequestContext request = new ProcessSendFreeTextMessageRequestContext(
												element.ElementNumber,
												requestId,
												sessionId,
												requestTimeout.Value,
												message);

											try
											{
												if (IsElementKnownByDatastore(request.ElementId))
												{
													newRequests.Add(request);
												}
												else
												{
													result.RequestId = Guid.Empty; // discard the generated request ID
                                                    result.ResultCode = InstantMessageErrorEnum.NoBaselineFoundForElementId;
													break;
												}
											}
											catch (PisDataStoreNotAccessibleException ex)
											{
												result.RequestId = Guid.Empty; // discard the generated request ID
												result.ResultCode = InstantMessageErrorEnum.DatastoreNotAccessible;
												_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.SendFreeTextMessage", ex, EventIdEnum.InstantMessage);
												break;
											}
										}

										// Queue to request list
										if (newRequests.Count > 0 && result.ResultCode == InstantMessageErrorEnum.RequestAccepted)
										{
											AddNewRequestToRequestList(newRequests);
										}
									}

									break;
								case T2GManagerErrorEnum.eT2GServerOffline:
									result.ResultCode = InstantMessageErrorEnum.T2GServerOffline;
									break;
								case T2GManagerErrorEnum.eElementNotFound:
									result.ResultCode = GetInvalidTargetAddressResponse(targetAddress);
									break;
								default:
									string errorMessage = string.Format(CultureInfo.CurrentCulture, InstantMessage.Properties.Resources.RetrieveAvailableElementErrorWithFormat, requestResult);
									_logManager.WriteLog(TraceType.ERROR, errorMessage, "PIS.Ground.InstantMessage.InstantMessageService.SendFreeTextMessage", null, EventIdEnum.InstantMessage);
									result.ResultCode = InstantMessageErrorEnum.InternalError;
									break;
							}
						}
						else
						{
							_logManager.WriteLog(TraceType.ERROR, InstantMessage.Properties.Resources.GenerateRequestIdentifierError, "PIS.Ground.InstantMessage.InstantMessageService.SendFreeTextMessage", null, EventIdEnum.InstantMessage);
							result.ResultCode = InstantMessageErrorEnum.InternalError;
						}
					}
					else
					{
						result.ResultCode = InstantMessageErrorEnum.InvalidSessionId;
					}
				}
				else
				{
					result.ResultCode = InstantMessageErrorEnum.InvalidRequestTimeout;
				}
			}
			catch (System.Exception ex)
			{
				_logManager.WriteLog(TraceType.EXCEPTION, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.SendFreeTextMessage", ex, EventIdEnum.InstantMessage);
				result.RequestId = Guid.Empty; // discard the generated request ID
				result.ResultCode = InstantMessageErrorEnum.InternalError;
			}

			return result;
		}

		private InstantMessageErrorEnum GetInstantMessageErrorEnum(ResultCodeEnum resCode)
		{
			switch (resCode)
			{
				case ResultCodeEnum.InvalidContext:
					return InstantMessageErrorEnum.InvalidContext;

				case ResultCodeEnum.InvalidRequestID:
					return InstantMessageErrorEnum.InvalidRequestID;

				case ResultCodeEnum.InvalidStartDate:
					return InstantMessageErrorEnum.InvalidStartDate;

				case ResultCodeEnum.InvalidEndDate:
					return InstantMessageErrorEnum.InvalidEndDate;

				case ResultCodeEnum.InvalidTrainID:
					return InstantMessageErrorEnum.InvalidTrainID;

				case ResultCodeEnum.SqlError:
					return InstantMessageErrorEnum.SqlError;

				case ResultCodeEnum.InvalidCommandType:
					return InstantMessageErrorEnum.InvalidCommandType;

				case ResultCodeEnum.InvalidStatus:
					return InstantMessageErrorEnum.InvalidStatus;

				case ResultCodeEnum.InternalError:
					return InstantMessageErrorEnum.InternalError;

				case ResultCodeEnum.RequestAccepted:
					return InstantMessageErrorEnum.RequestAccepted;

				default:
					return InstantMessageErrorEnum.InternalError;
			}
		}

		/// <summary>
		/// InstantMessageService Web service method "SendScheduledMessage" that sends a scheduled message to addressee.
		/// </summary>
		/// <param name="sessionId">The session identifier.</param>
		/// <param name="targetAddress">The targeted addressee information.</param>
		/// <param name="requestTimeout">Request timeout.</param>
		/// <param name="message">Scheduled message.</param>
		/// <returns>Response <see cref="InstantMessageResult"/>.</returns>
		public InstantMessageResult SendScheduledMessage(
			Guid sessionId,
			TargetAddressType targetAddress,
			uint? requestTimeout,
			ScheduledMessageType message)
		{
			var result = new InstantMessageResult();
			result.RequestId = Guid.Empty;
			result.ResultCode = InstantMessageErrorEnum.RequestAccepted;

			try
			{
				if (requestTimeout != null && requestTimeout.Value <= MaxRequestTimeout)
				{
					SessionData sessionData;
					_sessionManager.GetSessionDetails(sessionId, out sessionData);

					if (sessionData != null)
					{
						Guid requestId = Guid.Empty;
						_sessionManager.GenerateRequestID(sessionId, out requestId);

						if (requestId != Guid.Empty)
						{
							ElementList<AvailableElementData> elements;
							T2GManagerErrorEnum requestResult = _train2groundManager.GetAvailableElementDataByTargetAddress(targetAddress, out elements);
							switch (requestResult)
							{
								case T2GManagerErrorEnum.eSuccess:
									{
										result.RequestId = requestId;
										result.ResultCode = InstantMessageErrorEnum.RequestAccepted;

										List<InstantMessageRequestContext> newRequests = new List<InstantMessageRequestContext>(elements.Count);

										foreach (AvailableElementData element in elements)
										{
											ProcessSendScheduledMessageRequestContext request = new ProcessSendScheduledMessageRequestContext(
												element.ElementNumber,
												requestId,
												sessionId,
												requestTimeout.Value,
												message,
												_logManager);

											try
											{
												if (IsElementKnownByDatastore(request.ElementId))
												{
													newRequests.Add(request);
												}
												else
												{
													result.RequestId = Guid.Empty;
                                                    result.ResultCode = InstantMessageErrorEnum.NoBaselineFoundForElementId;
													break;
												}
											}
											catch (PisDataStoreNotAccessibleException ex)
											{
												result.RequestId = Guid.Empty; // discard the generated request ID
												result.ResultCode = InstantMessageErrorEnum.DatastoreNotAccessible;
												_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.SendScheduledMessage", ex, EventIdEnum.InstantMessage);
												break;
											}
										}

										// Queue to request list
										if (newRequests.Count > 0 && result.ResultCode == InstantMessageErrorEnum.RequestAccepted)
										{
											foreach (ProcessSendScheduledMessageRequestContext request in newRequests)
											{
												request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionProcessing);
												SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionProcessing, request.ElementId);
											}

											AddNewRequestToRequestList(newRequests);
										}
									}

									break;
								case T2GManagerErrorEnum.eT2GServerOffline:
									result.ResultCode = InstantMessageErrorEnum.T2GServerOffline;
									break;
								case T2GManagerErrorEnum.eElementNotFound:
									result.ResultCode = GetInvalidTargetAddressResponse(targetAddress);
									break;
								default:
									string errorMessage = string.Format(CultureInfo.CurrentCulture, InstantMessage.Properties.Resources.RetrieveAvailableElementErrorWithFormat, requestResult);
									_logManager.WriteLog(TraceType.ERROR, errorMessage, "PIS.Ground.InstantMessage.InstantMessageService.SendScheduledMessage", null, EventIdEnum.InstantMessage);
									result.ResultCode = InstantMessageErrorEnum.InternalError;
									break;
							}
						}
						else
						{
							_logManager.WriteLog(TraceType.ERROR, InstantMessage.Properties.Resources.GenerateRequestIdentifierError, "PIS.Ground.InstantMessage.InstantMessageService.SendScheduledMessage", null, EventIdEnum.InstantMessage);
							result.ResultCode = InstantMessageErrorEnum.InternalError;
						}
					}
					else
					{
						result.ResultCode = InstantMessageErrorEnum.InvalidSessionId;
					}
				}
				else
				{
					result.ResultCode = InstantMessageErrorEnum.InvalidRequestTimeout;
				}
			}
			catch (System.Exception ex)
			{
				_logManager.WriteLog(TraceType.EXCEPTION, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.SendScheduledMessage", ex, EventIdEnum.InstantMessage);
				result.RequestId = Guid.Empty; // discard the generated request ID
				result.ResultCode = InstantMessageErrorEnum.InternalError;
			}

			return result;
		}

		/// <summary>
		/// InstantMessageService Web service method "CancelAllMessages" that cancels all predefined and free-text messages at addressee.
		/// </summary>
		/// <param name="sessionId">The session identifier.</param>
		/// <param name="targetAddress">The targeted addressee information.</param>
		/// <param name="requestTimeout">Request timeout.</param>		
		/// <returns>Response <see cref="InstantMessageResult"/>.</returns>
		InstantMessageResult IInstantMessageService.CancelAllMessages(
			Guid sessionId,
			TargetAddressType targetAddress,
			uint? requestTimeout)
		{
			var result = new InstantMessageResult();
			result.RequestId = Guid.Empty;
			result.ResultCode = InstantMessageErrorEnum.RequestAccepted;

			try
			{
				if (requestTimeout != null && requestTimeout.Value <= MaxRequestTimeout)
				{
					SessionData sessionData;
					_sessionManager.GetSessionDetails(sessionId, out sessionData);

					if (sessionData != null)
					{
						Guid requestId = Guid.Empty;
						_sessionManager.GenerateRequestID(sessionId, out requestId);

						if (requestId != Guid.Empty)
						{
							ElementList<AvailableElementData> elements;
							T2GManagerErrorEnum requestResult = _train2groundManager.GetAvailableElementDataByTargetAddress(targetAddress, out elements);
							switch (requestResult)
							{
								case T2GManagerErrorEnum.eSuccess:
									{
										// Queue to request list
										List<InstantMessageRequestContext> requestList = new List<InstantMessageRequestContext>(1);
										foreach (AvailableElementData element in elements)
										{
											SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionProcessing, element.ElementNumber);

											ProcessCancelAllScheduledMessagesRequestContext request = new ProcessCancelAllScheduledMessagesRequestContext(
												element.ElementNumber,
												requestId,
												sessionId,
												requestTimeout.Value);

											requestList.Clear();
											requestList.Add(request);
											AddNewRequestToRequestList(requestList);
										}

										result.RequestId = requestId;
										result.ResultCode = InstantMessageErrorEnum.RequestAccepted;
									}

									break;
								case T2GManagerErrorEnum.eT2GServerOffline:
									result.ResultCode = InstantMessageErrorEnum.T2GServerOffline;
									break;
								case T2GManagerErrorEnum.eElementNotFound:
									result.ResultCode = GetInvalidTargetAddressResponse(targetAddress);
									break;
								default:
									string errorMessage = string.Format(CultureInfo.CurrentCulture, InstantMessage.Properties.Resources.RetrieveAvailableElementErrorWithFormat, requestResult);
									_logManager.WriteLog(TraceType.ERROR, errorMessage, "PIS.Ground.InstantMessage.InstantMessageService.CancelAllMessages", null, EventIdEnum.InstantMessage);
									result.ResultCode = InstantMessageErrorEnum.InternalError;
									break;
							}
						}
						else
						{
							_logManager.WriteLog(TraceType.ERROR, InstantMessage.Properties.Resources.GenerateRequestIdentifierError, "PIS.Ground.InstantMessage.InstantMessageService.CancelAllMessages", null, EventIdEnum.InstantMessage);
							result.ResultCode = InstantMessageErrorEnum.InternalError;
						}
					}
					else
					{
						result.ResultCode = InstantMessageErrorEnum.InvalidSessionId;
					}
				}
				else
				{
					result.ResultCode = InstantMessageErrorEnum.InvalidRequestTimeout;
				}
			}
			catch (System.Exception ex)
			{
				_logManager.WriteLog(TraceType.EXCEPTION, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.CancelAllMessages", ex, EventIdEnum.InstantMessage);
				result.ResultCode = InstantMessageErrorEnum.InternalError;
			}

			return result;
		}

		/// <summary>
		/// InstantMessageService Web service method "CancelScheduledMessage" that cancels all predefined and free-text messages at addressee.
		/// </summary>
		/// <param name="sessionId">The session identifier.</param>
		/// <param name="cancelMessageRequestId">Scheduled message ID to cancel.</param>
		/// <param name="targetAddress">The targeted addressee information.</param>
		/// <param name="requestTimeout">Request timeout.</param>		
		/// <returns>Response <see cref="InstantMessageResult"/>.</returns>
		InstantMessageResult IInstantMessageService.CancelScheduledMessage(
			Guid sessionId,
			Guid cancelMessageRequestId,
			TargetAddressType targetAddress,
			uint? requestTimeout)
		{
			var result = new InstantMessageResult();
			result.RequestId = Guid.Empty;
			result.ResultCode = InstantMessageErrorEnum.RequestAccepted;

			result.RequestId = cancelMessageRequestId;

			try
			{
				if (requestTimeout != null && requestTimeout.Value <= MaxRequestTimeout)
				{
					SessionData sessionData;
					_sessionManager.GetSessionDetails(sessionId, out sessionData);

					if (sessionData != null)
					{
						Guid requestId = Guid.Empty;
						_sessionManager.GenerateRequestID(sessionId, out requestId);

						if (requestId != Guid.Empty)
						{
							ElementList<AvailableElementData> elements;
							T2GManagerErrorEnum requestResult = _train2groundManager.GetAvailableElementDataByTargetAddress(targetAddress, out elements);
							switch (requestResult)
							{
								case T2GManagerErrorEnum.eSuccess:
									{
										List<InstantMessageRequestContext> requestList = new List<InstantMessageRequestContext>(1);
										foreach (AvailableElementData element in elements)
										{
											ProcessCancelScheduledMessageRequestContext request = new ProcessCancelScheduledMessageRequestContext(
												element.ElementNumber,
												requestId,
												sessionId,
												requestTimeout.Value,
												cancelMessageRequestId,
												_logManager);

											request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionProcessing);
											SendNotificationToGroundApp(requestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionProcessing, element.ElementNumber);

											requestList.Clear();
											requestList.Add(request);
											AddNewRequestToRequestList(requestList);
										}

										result.RequestId = requestId;
										result.ResultCode = InstantMessageErrorEnum.RequestAccepted;
									}

									break;
								case T2GManagerErrorEnum.eT2GServerOffline:
									result.ResultCode = InstantMessageErrorEnum.T2GServerOffline;
									break;
								case T2GManagerErrorEnum.eElementNotFound:
									result.ResultCode = GetInvalidTargetAddressResponse(targetAddress);
									break;
								default:
									string errorMessage = string.Format(CultureInfo.CurrentCulture, InstantMessage.Properties.Resources.RetrieveAvailableElementErrorWithFormat, requestResult);
									_logManager.WriteLog(TraceType.ERROR, errorMessage, "PIS.Ground.InstantMessage.InstantMessageService.CancelScheduledMessage", null, EventIdEnum.InstantMessage);
									result.ResultCode = InstantMessageErrorEnum.InternalError;
									break;
							}
						}
						else
						{
							_logManager.WriteLog(TraceType.ERROR, InstantMessage.Properties.Resources.GenerateRequestIdentifierError, "PIS.Ground.InstantMessage.InstantMessageService.CancelScheduledMessage", null, EventIdEnum.InstantMessage);
							result.ResultCode = InstantMessageErrorEnum.InternalError;
						}
					}
					else
					{
						result.ResultCode = InstantMessageErrorEnum.InvalidSessionId;
					}
				}
				else
				{
					result.ResultCode = InstantMessageErrorEnum.InvalidRequestTimeout;
				}
			}
			catch (System.Exception ex)
			{
				_logManager.WriteLog(TraceType.EXCEPTION, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.CancelScheduledMessage", ex, EventIdEnum.InstantMessage);
				result.ResultCode = InstantMessageErrorEnum.InternalError;
			}

			return result;
		}

#endregion

#region  "Methods"

		/// <summary>Sends a notification to ground application.</summary>
		/// <param name="requestId">Request ID for the corresponding request.</param>
		/// <param name="status">The status.</param>
		/// <param name="parameter">The parameter.</param>
		private void SendNotificationToGroundApp(
			Guid requestId,
			PIS.Ground.GroundCore.AppGround.NotificationIdEnum status,
			string parameter)
		{
			try
			{
				_logManager.WriteLog(TraceType.DEBUG, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_CALLED, "SendNotificationToGroundApp"), "PIS.Ground.Infotainment.InstantMessage.InstantMessageService.SendNotificationToGroundApp", null, EventIdEnum.InstantMessage);
				_notificationSender.SendNotification(status, parameter, requestId);
			}
			catch (Exception ex)
			{
				_logManager.WriteLog(TraceType.ERROR, String.Format(CultureInfo.CurrentCulture, Logs.INFO_FUNCTION_EXCEPTION_MESSAGE, "SendNotificationToGroundApp", ex.Message), "PIS.Ground.InstantMessage.InstantMessageService.SendNotificationToGroundApp", ex, EventIdEnum.InstantMessage);
			}
		}

		/// <summary>Executes the transmit event action.</summary>
		private void OnTransmitEvent()
		{
            try
            {
                var currentRequests = new List<InstantMessageRequestContext>();

                while (!_stopRequested)
                {
                    if (currentRequests.Count == 0)
                    {
                        _transmitEvent.WaitOne();
                    }

                    if (_stopRequested)
                    {
                        break;
                    }

                    lock (_lockNewRequests)
                    {
                        if (_newRequests.Count != 0)
                        {
                            currentRequests.AddRange(_newRequests);
                            _newRequests.Clear();
                            currentRequests.RemoveAll(c => c == null);
                        }
                    }

                    foreach (var request in currentRequests)
                    {
                        if (_stopRequested)
                        {
                            break;
                        }

                        try
                        {
                            switch (request.State)
                            {
                                case RequestState.Created:
                                    break;

                                case RequestState.ReadyToSend:
                                    if (request.NeedToApplyLogicToExistingRequests)
                                    {
                                        request.ApplyLogicToExistingRequests(currentRequests);
                                    }

                                    try
                                    {
                                        TransmitRequest(request);
                                        if (request.State == RequestState.Transmitted)
                                        {
                                            request.CompletionStatus = true;
                                        }
                                    }
                                    catch (PisDataStoreNotAccessibleException ex)
                                    {
                                        _logManager.WriteLog(TraceType.WARNING, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.OnTransmitEvent", ex, EventIdEnum.InstantMessage);
                                        request.TransmissionStatus = false;
                                        if (request.State == RequestState.WaitingRetry && request.TransferAttemptsDone == 1)
                                        {
                                            request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionWaitingToSend);
                                            SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionWaitingToSend, request.ElementId);
                                        }
                                    }
                                    catch (System.Exception ex)
                                    {
                                        request.ErrorStatus = true;
                                        request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionUnexpectedError);
                                        _logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.OnTransmitEvent", ex, EventIdEnum.InstantMessage);
                                        SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.Failed, request.ElementId);
                                    }
                                    break;

                                case RequestState.WaitingRetry:
                                    break;

                                case RequestState.Expired:
                                    string logString = string.Format(CultureInfo.CurrentCulture, Properties.Resources.RequestExpiredInfoWithFormat, request.RequestId, request.GetType().FullName);
                                    _logManager.WriteLog(TraceType.INFO, logString, "PIS.Ground.InstantMessage.InstantMessageService.OnTransmitEvent", null, EventIdEnum.InstantMessage);
                                    request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionTimedOut);
                                    SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionTimedOut, request.ElementId);
                                    break;

                                case RequestState.Transmitted:
                                    request.CompletionStatus = true;
                                    break;

                                case RequestState.AllRetriesExhausted:
                                    break;

                                case RequestState.Completed:
                                    break;

                                case RequestState.Error:
                                    break;
                            }
                        }
                        catch (ThreadAbortException)
                        {
                            throw;
                        }
                        catch (System.Exception ex)
                        {
                            request.ErrorStatus = true;
                            request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionUnexpectedError);
                            _logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.OnTransmitEvent", ex, EventIdEnum.InstantMessage);
                            SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.Failed, request.ElementId);
                        }
                    }

                    currentRequests.RemoveAll(c => c.IsStateFinal);
                    if (!_stopRequested)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // No logic to apply
            }
            catch (System.Exception exception)
            {
                if (_logManager != null)
                {
                    _logManager.WriteLog(TraceType.EXCEPTION, exception.Message, "PIS.Ground.InstantMessage.InstantMessageService.OnTransmitEvent", exception, EventIdEnum.InstantMessage);
                }
            }
		}

		/// <summary>Validate send predefined messages request.</summary>
		/// <exception cref="PisDataStoreNotAccessibleException">Thrown when the Pis Data Store Not
		/// Accessible error condition occurs.</exception>
		/// <param name="element">The element.</param>
		/// <param name="requestId">Request ID for the corresponding request.</param>
		/// <param name="request">The request.</param>
		/// <returns>The status to send as notification.</returns>
		private PIS.Ground.GroundCore.AppGround.NotificationIdEnum? ValidateSendPredefinedMessagesRequest(AvailableElementData element, Guid requestId, ProcessSendPredefMessagesRequestContext request)
		{
			if (!IsDataPackageKnownByDatastore("PISBASE", GetElementCurrentPisBasePackageVersion(element)))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
			}

			if (request.Messages == null)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
			}

			if (request.Messages.Length == 0)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
			}

			if (request.Messages.Where((item, i) => string.IsNullOrEmpty(item.Identifier)).Count() > 0)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
			}

			return ValidatePredefinedMessageTemplate(element, request);
		}

		/// <summary>Validate send free text message request.</summary>
		/// <exception cref="PisDataStoreNotAccessibleException">Thrown when the Pis Data Store Not
		/// Accessible error condition occurs.</exception>
		/// <param name="element">The element.</param>
		/// <param name="requestId">Request ID for the corresponding request.</param>
		/// <param name="request">The request.</param>
		/// <returns>The notification to send to ground app.</returns>
		private PIS.Ground.GroundCore.AppGround.NotificationIdEnum? ValidateSendFreeTextMessageRequest(AvailableElementData element, Guid requestId, ProcessSendFreeTextMessageRequestContext request)
		{
			if (!IsDataPackageKnownByDatastore("PISBASE", GetElementCurrentPisBasePackageVersion(element)))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
			}

			if (request.Message == null)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
			}

			if (string.IsNullOrEmpty(request.Message.Identifier))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
			}

			if (System.Text.Encoding.UTF8.GetByteCount(request.Message.FreeText) > MaxFreeTextMessageTextSize)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidText;
			}

			if (request.Message.NumberOfRepetitions > MaxFreeTextMessageRepetitions)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidRepetitionCount;
			}

			// The remaining validations are accessing the template list
			return ValidateMessageIdentifier(TemplateCategory.FreeText, element, request.Message.Identifier);
		}

		/// <summary>Validate predefined messages template with baseline information.</summary>
		/// <param name="element">The element.</param>
		/// <param name="request">Predefined messages request to validate.</param>
		/// <returns>Null if all predefined message identifiers are valid and the provided parameters, otherwise return an error code.</returns>
		protected virtual PIS.Ground.GroundCore.AppGround.NotificationIdEnum? ValidatePredefinedMessageTemplate(AvailableElementData element, ProcessSendPredefMessagesRequestContext request)
		{
			try
			{
				using (var remoteDataStore = new RemoteDataStoreProxy())
				{
					try
					{
						var openLmtPackageResult = remoteDataStore.openLocalDataPackage(
							"LMT",
							GetElementCurrentLmtPackageVersion(element),
							String.Empty);

						List<string> luaScriptsList = new List<string>();

						string templateFilePath = GetLuaScriptsList(remoteDataStore, element, luaScriptsList);
						string lmtDatabaseFilePath = FindLmtDatabaseFilePath(openLmtPackageResult.LocalPackagePath);

						if (string.IsNullOrEmpty(templateFilePath) && !string.IsNullOrEmpty(lmtDatabaseFilePath))
						{
							return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
						}

						using (var templateAccessor = new TemplateListAccessor())
						{
							if (templateAccessor.ExecuteTemplate(templateFilePath, luaScriptsList) == false)
							{
								return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageTemplateFileNotValid;
							}

							List<Template> templateList = templateAccessor.GetAllTemplates();

							foreach (var message in request.Messages)
							{
								Template template = templateList.FirstOrDefault(item => item.ID == message.Identifier && item.Category == TemplateCategory.Predefined);

								if (template == null)
								{
									return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
								}

								if (template.Category != TemplateCategory.Predefined)
								{
									return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
								}

								foreach (var parameter in template.ParameterList)
								{
									if (parameter == TemplateParameterType.CarNumber && message.CarId == null)
									{
										return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUnknownCarId;
									}

									if (parameter == TemplateParameterType.Delay && message.Delay == null)
									{
										return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidDelay;
									}

									if (parameter == TemplateParameterType.DelayReasonCode && string.IsNullOrEmpty(message.DelayReason))
									{
										return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidDelayReasonCode;
									}

									if (parameter == TemplateParameterType.Hour && message.Hour == null)
									{
										return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidHour;
									}

									if (parameter == TemplateParameterType.StationId)
									{
										bool stationIdIsValid = false;
										if (!string.IsNullOrEmpty(message.StationId))
										{
											using (var lmtDatabaseAccessor = new LmtDatabaseAccessor(lmtDatabaseFilePath))
											{
												uint? internalCode = lmtDatabaseAccessor.GetStationInternalCodeFromOperatorCode(message.StationId);
												if (internalCode != null)
												{
													request.StationInternalIdDictionary[message.StationId] = internalCode.Value;
													stationIdIsValid = true;
												}
											}
										}

										if (!stationIdIsValid)
										{
											return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedStationId;
										}
									}
								}
							}
						}
					}
					finally
					{
						if (remoteDataStore.State == CommunicationState.Faulted)
						{
							remoteDataStore.Abort();
						}
					}
				}
			}
			catch (CommunicationException ex)
			{
				throw new PisDataStoreNotAccessibleException("PIS.Ground.InstantMessage.InstantMessageService.ValidatePredefinedMessageTemplate", ex);
			}
			catch (Exception ex)
			{
				_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.ValidatePredefinedMessageTemplate", ex, EventIdEnum.InstantMessage);
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInternalError;
			}

			return null;
		}

		/// <summary>Validate template and message identifier.</summary>
		/// <param name="category">The category of the message.</param>
		/// <param name="element">The element.</param>
		/// <param name="messageIdentifier">Identifier for the message.</param>
		/// <returns>Null if template and message identifier are valid, otherwise return the error code.</returns>
		protected virtual PIS.Ground.GroundCore.AppGround.NotificationIdEnum? ValidateMessageIdentifier(TemplateCategory category, AvailableElementData element, string messageIdentifier)
		{
			try
			{
				using (var remoteDataStore = new RemoteDataStoreProxy())
				{
					try
					{
						List<string> luaScriptsList = new List<string>();

						string templateFilePath = GetLuaScriptsList(remoteDataStore, element, luaScriptsList);

						if (string.IsNullOrEmpty(templateFilePath))
						{
							return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
						}

						using (var templateAccessor = new TemplateListAccessor())
						{
							if (templateAccessor.ExecuteTemplate(templateFilePath, luaScriptsList) == false)
							{
								return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageTemplateFileNotValid;
							}

							List<Template> templateList = templateAccessor.GetAllTemplates();
							if (!templateList.Exists(item => item.ID == messageIdentifier && item.Category == category))
							{
								return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
							}
						}
					}
					finally
					{
						if (remoteDataStore.State == CommunicationState.Faulted)
						{
							remoteDataStore.Abort();
						}
					}
				}
			}
			catch (CommunicationException ex)
			{
				throw new PisDataStoreNotAccessibleException("PIS.Ground.InstantMessage.InstantMessageService.ValidateMessageIdentifier", ex);
			}
			catch (Exception ex)
			{
				_logManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.InstantMessage.InstantMessageService.ValidateMessageIdentifier", ex, EventIdEnum.InstantMessage);
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInternalError;
			}

			return null;
		}

		/// <summary>Validate send scheduled message request.</summary>
		/// <exception cref="PisDataStoreNotAccessibleException">Thrown when the Pis Data Store Not
		/// Accessible error condition occurs.</exception>
		/// <param name="element">The element.</param>
		/// <param name="requestId">Request ID for the corresponding request.</param>
		/// <param name="request">The request.</param>
		/// <returns>The notification to send to ground app.</returns>
		private PIS.Ground.GroundCore.AppGround.NotificationIdEnum? ValidateSendScheduledMessageRequest(AvailableElementData element, Guid requestId, ProcessSendScheduledMessageRequestContext request)
		{
			if (!IsDataPackageKnownByDatastore("PISBASE", GetElementCurrentPisBasePackageVersion(element)))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
			}

			if (request.Message == null)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
			}

			if (string.IsNullOrEmpty(request.Message.Identifier))
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
			}

			if (System.Text.Encoding.UTF8.GetByteCount(request.Message.FreeText) > MaxScheduledMessageTextSize)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidText;
			}

			if (request.Message.Period == null ||
				request.Message.Period.StartDateTime == null ||
				request.Message.Period.EndDateTime == null ||
				request.Message.Period.StartDateTime.SafeCompareTo(request.Message.Period.EndDateTime) >= 0 ||
				request.Message.Period.EndDateTime.SafeCompareTo(DateTime.UtcNow) <= 0)
			{
				return PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidScheduledPeriod;
			}

			// The remaining validations are accessing the template list
			return ValidateMessageIdentifier(TemplateCategory.Scheduled, element, request.Message.Identifier);
		}

		/// <summary>Transmit request.</summary>
		/// <param name="request">The request.</param>
		private void TransmitRequest(InstantMessageRequestContext request)
		{
			bool requestTransmitted = false;

			ServiceInfo serviceInfo;
			bool elementOnline;
			T2GManagerErrorEnum requestResult = _train2groundManager.IsElementOnline(request.ElementId, out elementOnline);
			switch (requestResult)
			{
				case T2GManagerErrorEnum.eSuccess:
					if (elementOnline == true)
					{
						T2GManagerErrorEnum result = _train2groundManager.GetAvailableServiceData(request.ElementId, (int)eServiceID.eSrvSIF_InstantMessageServer, out serviceInfo);
						switch (result)
						{
							case T2GManagerErrorEnum.eSuccess:
								{
									string endpoint = "http://" + serviceInfo.ServiceIPAddress + ":" + serviceInfo.ServicePortNumber;

									PIS.Ground.GroundCore.AppGround.NotificationIdEnum? errorNotification = null;
									AvailableElementData elementData;
									requestResult = _train2groundManager.GetAvailableElementDataByElementNumber(request.ElementId, out elementData);
									switch (requestResult)
									{
										case T2GManagerErrorEnum.eSuccess:
											{
												if (request is ProcessSendPredefMessagesRequestContext)
												{
													errorNotification = ValidateSendPredefinedMessagesRequest(
														elementData,
														request.RequestId,
														request as ProcessSendPredefMessagesRequestContext);
												}
												else if (request is ProcessSendFreeTextMessageRequestContext)
												{
													errorNotification = ValidateSendFreeTextMessageRequest(
														elementData,
														request.RequestId,
														request as ProcessSendFreeTextMessageRequestContext);
												}
												else if (request is ProcessSendScheduledMessageRequestContext)
												{
													errorNotification = ValidateSendScheduledMessageRequest(
														elementData,
														request.RequestId,
														request as ProcessSendScheduledMessageRequestContext);
												}
												else
												{
													// No validation required for other requests.
												}

												if (errorNotification.HasValue)
												{
													request.ErrorStatus = true;
													request.LogStatusInHistoryLog(ConvertToMessageStatusType(errorNotification.Value));
													SendNotificationToGroundApp(request.RequestId, errorNotification.Value, request.ElementId);
												}
												else
												{
													try
													{
														// Call InstantMessage train service
														using (InstantMessageTrainServiceClient trainClient = new InstantMessageTrainServiceClient("InstantMessageEndpoint", endpoint))
														{
															try
															{
																if (request is ProcessCancelAllScheduledMessagesRequestContext)
																{
																	TransmitCancelAllMessagesRequest(trainClient, request as ProcessCancelAllScheduledMessagesRequestContext);
																}
																else if (request is ProcessCancelScheduledMessageRequestContext)
																{
																	TransmitCancelScheduledMessageRequest(trainClient, request as ProcessCancelScheduledMessageRequestContext);
																}
																else if (request is ProcessSendPredefMessagesRequestContext)
																{
																	TransmitSendPredefMessagesRequest(trainClient, request as ProcessSendPredefMessagesRequestContext);
																}
																else if (request is ProcessSendFreeTextMessageRequestContext)
																{
																	TransmitSendFreeTextMessagesRequest(trainClient, request as ProcessSendFreeTextMessageRequestContext);
																}
																else if (request is ProcessSendScheduledMessageRequestContext)
																{
																	TransmitSendScheduledMessageRequest(trainClient, request as ProcessSendScheduledMessageRequestContext);
																}
																else
																{
																	// No other request type supported
																	throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "No implementation for request of type '{0}", request.GetType().FullName));
																}

																requestTransmitted = true;
															}
															catch (Exception ex)
															{
																if (!ShouldContinueOnTransmissionError(ex))
																{
																	request.ErrorStatus = true;
																	if (request.State == RequestState.Error)
																	{
																		request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionUnexpectedError);
																		SendNotificationToGroundApp(request.RequestId, GroundCore.AppGround.NotificationIdEnum.Failed, request.ElementId);
																	}
																}
															}
															finally
															{
																if (trainClient.State == CommunicationState.Faulted)
																{
																	trainClient.Abort();
																}
															}
														}
													}
													catch (Exception ex)
													{
														if (!ShouldContinueOnTransmissionError(ex))
														{
															request.ErrorStatus = true;
															if (request.State == RequestState.Error)
															{
																request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionUnexpectedError);
																SendNotificationToGroundApp(request.RequestId, GroundCore.AppGround.NotificationIdEnum.Failed, request.ElementId);
															}
														}
													}
												}
											}

											break;
										case T2GManagerErrorEnum.eT2GServerOffline:
											SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageT2GServerOffline, string.Empty);
											break;
										case T2GManagerErrorEnum.eElementNotFound:
											SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageElementNotFound, request.ElementId);
											break;
										default:
											break;
									}
								}

								break;
							case T2GManagerErrorEnum.eT2GServerOffline:
								SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageT2GServerOffline, string.Empty);
								break;
							case T2GManagerErrorEnum.eElementNotFound:
								SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageElementNotFound, request.ElementId);
								break;
							case T2GManagerErrorEnum.eServiceInfoNotFound:
								SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageServiceNotFound, eServiceID.eSrvSIF_InstantMessageServer.ToString());
								break;
							default:
								break;
						}
					}

					break;
				case T2GManagerErrorEnum.eT2GServerOffline:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageT2GServerOffline, string.Empty);
					break;
				case T2GManagerErrorEnum.eElementNotFound:
					SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageElementNotFound, request.ElementId);
					break;
				default:
					break;
			}

			request.TransmissionStatus = requestTransmitted;

			if (request.State == RequestState.WaitingRetry && request.TransferAttemptsDone == 1)
			{
				request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionWaitingToSend);
				SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionWaitingToSend, request.ElementId);
			}
		}

		/// <summary>Converts an ID of type NotificationIdEnum to type MessageStatusType.</summary>
		/// <param name="id">The identifier to convert.</param>
		/// <return>The corresponding value of type MessageStatusType for parameter value id.</return>
		private static MessageStatusType ConvertToMessageStatusType(PIS.Ground.GroundCore.AppGround.NotificationIdEnum id)
		{
			switch (id)
			{
				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId:
					return MessageStatusType.InstantMessageDistributionInvalidTemplateError;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidScheduledPeriod:
					return MessageStatusType.InstantMessageDistributionInvalidScheduledPeriodError;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidRepetitionCount:
					return MessageStatusType.InstantMessageDistributionInvalidRepetitionCountError;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageTemplateFileNotValid:
					return MessageStatusType.InstantMessageDistributionInvalidTemplateFileError;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUnknownCarId:
					return MessageStatusType.InstantMessageDistributionUnknownCarIdError;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidDelay:
					return MessageStatusType.InstantMessageDistributionInvalidDelayError;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidDelayReasonCode:
					return MessageStatusType.InstantMessageDistributionInvalidDelayReasonError;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidHour:
					return MessageStatusType.InstantMessageDistributionInvalidHourError;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedStationId:
					return MessageStatusType.InstantMessageDistributionUndefinedStationIdError;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidText:
					return MessageStatusType.InstantMessageDistributionInvalidTextError;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionTimedOut:
					return MessageStatusType.InstantMessageDistributionTimedOut;
			
				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionProcessing:
					return MessageStatusType.InstantMessageDistributionProcessing;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionReceived:
					return MessageStatusType.InstantMessageDistributionReceived;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionSent:
					return MessageStatusType.InstantMessageDistributionSent;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionWaitingToSend:
					return MessageStatusType.InstantMessageDistributionWaitingToSend;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageElementNotFound:
					return MessageStatusType.InstantMessageDistributionWaitingToSend;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInternalError:
					return MessageStatusType.InstantMessageDistributionUnexpectedError;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageMessageLimitExceeded:
					return MessageStatusType.InstantMessageDistributionMessageLimitExceededError;

				case PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionInhibited:
					return MessageStatusType.InstantMessageDistributionInhibited;
			}

			return MessageStatusType.InstantMessageDistributionUnexpectedError;
		}

		/// <summary>Transmit send predef messages request.</summary>
		/// <exception cref="FaultException">Thrown when a Fault error condition occurs.</exception>
		/// <param name="client">The client.</param>
		/// <param name="request">The request.</param>
		private void TransmitSendPredefMessagesRequest(InstantMessageTrainServiceClient client, ProcessSendPredefMessagesRequestContext request)
		{
			ErrorType errValue;

			PIS.Train.InstantMessage.PredefinedMessageType[] messages =
					Array.ConvertAll<PIS.Ground.InstantMessage.PredefinedMessageType, PIS.Train.InstantMessage.PredefinedMessageType>(
					request.Messages,
					messageIn => ConvertDataTypeGroundToTrain(request, messageIn));

			try
			{
				errValue = client.SendPredefinedMessages(request.RequestId.ToString(), messages);
			}
			catch (FaultException)
			{
				// When there is a SOAP fault while sending, we still notify that we sent the request
				request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);
				SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionSent, request.ElementId);
				throw;
			}

			request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);
			request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionReceived);
			SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionSent, request.ElementId);
			SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionReceived, request.ElementId);

			PIS.Ground.GroundCore.AppGround.NotificationIdEnum? statusNotificationId = null;

			switch (errValue)
			{
				case ErrorType.Succeed:
					// Nothing to do
					break;
				case ErrorType.InvalidHour:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidHour;
					break;
				case ErrorType.InvalidMessageId:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
					break;
				case ErrorType.InvalidParamCarId:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUnknownCarId;
					break;
				case ErrorType.InvalidParamDelay:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidDelay;
					break;
				case ErrorType.InvalidParamDelayReason:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidDelayReasonCode;
					break;
				case ErrorType.InvalidParamStationId:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedStationId;
					break;
				case ErrorType.ServiceInhibited:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionInhibited;
					break;
				default:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInternalError;
					break;
			}

			if (statusNotificationId != null)
			{
				request.LogStatusInHistoryLog(ConvertToMessageStatusType(statusNotificationId.Value));
				SendNotificationToGroundApp(request.RequestId, (PIS.Ground.GroundCore.AppGround.NotificationIdEnum)statusNotificationId.Value, request.ElementId);
			}
		}

		/// <summary>Transmit send free text messages request.</summary>
		/// <exception cref="FaultException">Thrown when a Fault error condition occurs.</exception>
		/// <param name="client">The client.</param>
		/// <param name="request">The request.</param>
		private void TransmitSendFreeTextMessagesRequest(InstantMessageTrainServiceClient client, ProcessSendFreeTextMessageRequestContext request)
		{
			ErrorType errValue;

			try
			{
				errValue = client.SendFreeTextMessage(request.RequestId.ToString(), ConvertDataTypeGroundToTrain(request.Message));
			}
			catch (FaultException)
			{
				// When there is a SOAP fault while sending, we still notify that we sent the request
				request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);
				SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionSent, request.ElementId);
				throw;
			}

			request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);
			request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionReceived);
			SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionSent, request.ElementId);
			SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionReceived, request.ElementId);

			PIS.Ground.GroundCore.AppGround.NotificationIdEnum? statusNotificationId = null;

			switch (errValue)
			{
				case ErrorType.Succeed:
					// Nothing to do
					break;
				case ErrorType.InvalidMessageId:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
					break;
				case ErrorType.ServiceInhibited:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionInhibited;
					break;
				default:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInternalError;
					break;
			}

			if (statusNotificationId != null)
			{
				request.LogStatusInHistoryLog(ConvertToMessageStatusType(statusNotificationId.Value));
				SendNotificationToGroundApp(request.RequestId, statusNotificationId.Value, request.ElementId);
			}
		}

		/// <summary>Transmit send scheduled message request.</summary>
		/// <exception cref="FaultException">Thrown when a Fault error condition occurs.</exception>
		/// <param name="client">The client.</param>
		/// <param name="request">The request.</param>
		private void TransmitSendScheduledMessageRequest(InstantMessageTrainServiceClient client, ProcessSendScheduledMessageRequestContext request)
		{
			ErrorType errValue;

			try
			{
				errValue = client.SendScheduledMessage(request.RequestId.ToString(), ConvertDataTypeGroundToTrain(request.Message));
			}
			catch (FaultException)
			{
				// When there is a SOAP fault while sending, we still notify that we sent the request
				request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);
				SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionSent, request.ElementId);
				throw;
			}

			request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);
			request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionReceived);
			SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionSent, request.ElementId);
			SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionReceived, request.ElementId);

			PIS.Ground.GroundCore.AppGround.NotificationIdEnum? statusNotificationId = null;

			switch (errValue)
			{
				case ErrorType.Succeed:
					break;
				case ErrorType.InvalidParamPeriod:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidScheduledPeriod;
					break;
				case ErrorType.InvalidScheduledMessageId:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
					break;
				case ErrorType.InvalidMessageId:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageUndefinedTemplateId;
					break;
				case ErrorType.InvalidParamText:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInvalidText;
					break;
				case ErrorType.IOError:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInternalError;
					break;
				case ErrorType.MessageLimitReached:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageMessageLimitExceeded;
					break;
				case ErrorType.ServiceInhibited:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionInhibited;
					break;
				default:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInternalError;
					break;
			}

			if (statusNotificationId.HasValue)
			{
				request.LogStatusInHistoryLog(ConvertToMessageStatusType(statusNotificationId.Value));
				SendNotificationToGroundApp(request.RequestId, statusNotificationId.Value, request.ElementId);
			}
		}

		/// <summary>Transmit cancel all messages request.</summary>
		/// <exception cref="FaultException">Thrown when a Fault error condition occurs.</exception>
		/// <param name="client">The client.</param>
		/// <param name="request">The request.</param>
		private void TransmitCancelAllMessagesRequest(InstantMessageTrainServiceClient client, ProcessCancelAllScheduledMessagesRequestContext request)
		{
			ErrorType errValue;

			try
			{
				errValue = client.CancelAllMessage();
			}
			catch (FaultException)
			{
				// When there is a SOAP fault while sending, we still notify that we sent the request
				request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);
				SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionSent, request.ElementId);
				throw;
			}

			request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);
			request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionReceived);
			SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionSent, request.ElementId);
			SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionReceived, request.ElementId);

			PIS.Ground.GroundCore.AppGround.NotificationIdEnum? statusNotificationId = null;

			switch (errValue)
			{
				case ErrorType.Succeed:
					// Nothing to do
					break;
				case ErrorType.ServiceInhibited:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionInhibited;
					break;
				default:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInternalError;
					break;
			}

			if (statusNotificationId.HasValue)
			{
				request.LogStatusInHistoryLog(ConvertToMessageStatusType(statusNotificationId.Value));
				SendNotificationToGroundApp(request.RequestId, statusNotificationId.Value, request.ElementId);
			}
		}

		/// <summary>Transmit cancel scheduled message request.</summary>
		/// <exception cref="FaultException">Thrown when a Fault error condition occurs.</exception>
		/// <param name="client">The client.</param>
		/// <param name="request">The request.</param>
		private void TransmitCancelScheduledMessageRequest(InstantMessageTrainServiceClient client, ProcessCancelScheduledMessageRequestContext request)
		{
			ErrorType errValue;
			ProcessCancelScheduledMessageRequestContext req = request as ProcessCancelScheduledMessageRequestContext;

			try
			{
				errValue = client.CancelScheduledMessage(request.CancelMessageRequestId.ToString());
			}
			catch (FaultException)
			{
				// When there is a SOAP fault while sending, we still notify that we sent the request
				request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);
				SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionSent, request.ElementId);
				throw;
			}

			request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionSent);
			request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionReceived);
			SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionSent, request.ElementId);
			SendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionReceived, request.ElementId);

			PIS.Ground.GroundCore.AppGround.NotificationIdEnum? statusNotificationId = null;

			switch (errValue)
			{
				case ErrorType.Succeed:
					// Nothing to do
					break;
				case ErrorType.InvalidScheduledMessageId:
					// Nothing to do, message does not exist
					break;
				case ErrorType.IOError:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInternalError;
					break;
				case ErrorType.ServiceInhibited:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageDistributionInhibited;
					break;
				default:
					statusNotificationId = PIS.Ground.GroundCore.AppGround.NotificationIdEnum.InstantMessageInternalError;
					break;
			}

			if (statusNotificationId.HasValue)
			{
				request.LogStatusInHistoryLog(ConvertToMessageStatusType(statusNotificationId.Value));
				SendNotificationToGroundApp(request.RequestId, statusNotificationId.Value, request.ElementId);
			}
		}

		/// <summary>
		/// Determines whether to continue retrying after a transmission error, or discard the request.
		/// </summary>
		/// <param name="exception">The intercepted exception.</param>
		/// <returns>true if the caller can continue to communicate with train web service, false otherwise.</returns>
		private static bool ShouldContinueOnTransmissionError(Exception exception)
		{
			bool canContinue = true;
			EndpointNotFoundException endpointException;
			if (exception is TimeoutException)
			{
			}
			else if ((endpointException = exception as EndpointNotFoundException) != null)
			{
				WebException webException = endpointException.InnerException as WebException;
				if (webException != null && webException.Status == WebExceptionStatus.NameResolutionFailure)
				{
				}
				else if (webException != null && webException.Status == WebExceptionStatus.ConnectFailure)
				{
				}
				else
				{
				}
			}
			else if (exception is ActionNotSupportedException)
			{
				canContinue = false;
			}
			else if (exception is ProtocolException)
			{
				WebException webException = exception.InnerException as WebException;
				if (webException != null && webException.Status == WebExceptionStatus.NameResolutionFailure)
				{
				}
				else
				{
					canContinue = false;
				}
			}
			else
			{
				canContinue = false;
			}

			return canContinue;
		}

		/// <summary>Instant message request context.</summary>
		public class InstantMessageRequestContext : RequestContext
		{
			/// <summary>
			/// Gets or sets a value indicating whether the need to apply logic to existing requests.
			/// </summary>
			/// <value>True if need to apply logic to existing requests, false if not.</value>
			public bool NeedToApplyLogicToExistingRequests
			{
				get;
				protected set;
			}

			/// <summary>Initializes a new instance of the InstantMessageRequestContext class.</summary>
			/// <param name="elementId">Identifier for the element.</param>
			/// <param name="requestId">Identifier for the request.</param>
			/// <param name="sessionId">Identifier for the session.</param>
			/// <param name="timeout">The timeout.</param>
			public InstantMessageRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout)
				: base(string.Empty, elementId, requestId, sessionId, timeout)
			{
			}

			/// <summary>Logs the status of the current request into the history log.</summary>
			/// <param name="messageStatus">The message status.</param>
			public virtual void LogStatusInHistoryLog(MessageStatusType messageStatus)
			{
				// By default, no log is generated.
			}

			/// <summary>Applies the logic to existing requests described by requests.</summary>
			/// <param name="requests">The existing requests.</param>
			public virtual void ApplyLogicToExistingRequests(List<InstantMessageRequestContext> requests)
			{
				// By default, no special logic is applied to existing requests.
			}
		}

		/// <summary>Process cancel all scheduled messages request context.</summary>
		public class ProcessCancelAllScheduledMessagesRequestContext : InstantMessageRequestContext
		{
			/// <summary>
			/// Initializes a new instance of the ProcessCancelAllScheduledMessagesRequestContext class.
			/// </summary>
			/// <param name="elementId">Identifier for the element.</param>
			/// <param name="requestId">Identifier for the request.</param>
			/// <param name="sessionId">Identifier for the session.</param>
			/// <param name="timeout">The timeout.</param>
			public ProcessCancelAllScheduledMessagesRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout)
				: base(elementId, requestId, sessionId, timeout)
			{
				NeedToApplyLogicToExistingRequests = true;
			}

			/// <summary>Applies the logic to existing requests to cancel unsent message into the request queue that precede this current request.</summary>
			/// <param name="requests">The existing requests.</param>
			public override void ApplyLogicToExistingRequests(List<InstantMessageRequestContext> requests)
			{
				foreach (InstantMessageRequestContext request in requests)
				{
					if (object.ReferenceEquals(this, request))
					{
						break;
					}

					if (!request.IsStateFinal && 
						(request is ProcessSendFreeTextMessageRequestContext ||
						request is ProcessSendPredefMessagesRequestContext ||
						request is ProcessSendScheduledMessageRequestContext))
					{
						request.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionCanceled);
						request.CompletionStatus = true;
					}
				}

				NeedToApplyLogicToExistingRequests = false;
			}
		}

		/// <summary>Process cancel scheduled message request context.</summary>
		public class ProcessCancelScheduledMessageRequestContext : InstantMessageRequestContext
		{
			private readonly Guid _cancelMessageRequestId;
			private readonly ILogManager _logManager;
			private bool _firstLogEntryGenerated;

			/// <summary>
			/// Initializes a new instance of the ProcessCancelScheduledMessageRequestContext class.
			/// </summary>
			/// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
			/// <param name="elementId">Identifier for the element.</param>
			/// <param name="requestId">Identifier for the request.</param>
			/// <param name="sessionId">Identifier for the session.</param>
			/// <param name="timeout">The timeout.</param>
			/// <param name="cancelMessageRequestId">Identifier for the cancel message request.</param>
			/// <param name="logManager">Manager for logging into the history log.</param>
			public ProcessCancelScheduledMessageRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout, Guid cancelMessageRequestId, ILogManager logManager)
				: base(elementId, requestId, sessionId, timeout)
			{
				if (logManager == null)
				{
					throw new ArgumentNullException("logManager");
				}
				else if (requestId == Guid.Empty)
				{
					throw new ArgumentOutOfRangeException("requestId");
				}
				else if (cancelMessageRequestId == Guid.Empty)
				{
					throw new ArgumentOutOfRangeException("cancelMessageRequestId");
				}

				_cancelMessageRequestId = cancelMessageRequestId;
				_logManager = logManager;
				NeedToApplyLogicToExistingRequests = true;
			}

			/// <summary>Gets the identifier of the cancel message request.</summary>
			/// <value>The identifier of the cancel message request.</value>
			public Guid CancelMessageRequestId
			{
				get { return _cancelMessageRequestId; }
			}

			/// <summary>Logs the status of the current request into the history log.</summary>
			/// <param name="messageStatus">The message status.</param>
			public override void LogStatusInHistoryLog(MessageStatusType messageStatus)
			{
				if (_firstLogEntryGenerated)
				{
					ResultCodeEnum result = _logManager.UpdateMessageStatus(ElementId, _cancelMessageRequestId, messageStatus, CommandType.CancelScheduledMessage);

					// Force the invocation of CancelLog that will create a default scheduled message
					if (result == ResultCodeEnum.InvalidRequestID)
					{
						_firstLogEntryGenerated = false;
					}
				}

				if (!_firstLogEntryGenerated)
				{
					_firstLogEntryGenerated = true;
					_logManager.CancelLog(_cancelMessageRequestId, CommandType.CancelScheduledMessage, ElementId, messageStatus);
				}
			}

			/// <summary>Verify if the scheduled message to cancel exist into the requests queue and cancel it if not in final state.</summary>
			/// <param name="requests">The existing requests.</param>
			public override void ApplyLogicToExistingRequests(List<InstantMessageRequestContext> requests)
			{
				foreach (var requestToCancel in requests.Where(
					c => c.RequestId.Equals(CancelMessageRequestId)
						&& !c.IsStateFinal
						&& c is ProcessSendScheduledMessageRequestContext))
				{
					requestToCancel.LogStatusInHistoryLog(MessageStatusType.InstantMessageDistributionCanceled);
					requestToCancel.CompletionStatus = true;
				}

				NeedToApplyLogicToExistingRequests = false;
			}
		}

		/// <summary>Process send predef messages request context.</summary>
		public class ProcessSendPredefMessagesRequestContext : InstantMessageRequestContext
		{
			private PredefinedMessageType[] _messages;
			private Dictionary<string, uint> _stationInternalIdDictionary;

			/// <summary>
			/// Initializes a new instance of the ProcessSendPredefMessagesRequestContext class.
			/// </summary>
			/// <param name="elementId">Identifier for the element.</param>
			/// <param name="requestId">Identifier for the request.</param>
			/// <param name="sessionId">Identifier for the session.</param>
			/// <param name="timeout">The timeout.</param>
			/// <param name="messages">MessageListType read only accessor.</param>
			public ProcessSendPredefMessagesRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout, PredefinedMessageType[] messages)
				: base(elementId, requestId, sessionId, timeout)
			{
				this._messages = messages;
				_stationInternalIdDictionary = new Dictionary<string, uint>();
			}

			/// <summary>Gets the messages.</summary>
			/// <value>The messages.</value>
			public PredefinedMessageType[] Messages
			{
				get
				{
					return this._messages;
				}
			}

			/// <summary>Gets a dictionary of station internal identifiers.</summary>
			/// <value>A Dictionary of station internal identifiers.</value>
			public Dictionary<string, uint> StationInternalIdDictionary
			{
				get
				{
					return this._stationInternalIdDictionary;
				}
			}
		}

		/// <summary>Process send free text message request context.</summary>
		public class ProcessSendFreeTextMessageRequestContext : InstantMessageRequestContext
		{
			private FreeTextMessageType _message;

			/// <summary>
			/// Initializes a new instance of the ProcessSendFreeTextMessageRequestContext class.
			/// </summary>
			/// <param name="elementId">Identifier for the element.</param>
			/// <param name="requestId">Identifier for the request.</param>
			/// <param name="sessionId">Identifier for the session.</param>
			/// <param name="timeout">The timeout.</param>
			/// <param name="message">FreeTextMessageType read only accessor.</param>
			public ProcessSendFreeTextMessageRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout, FreeTextMessageType message)
				: base(elementId, requestId, sessionId, timeout)
			{
				this._message = message;
			}

			/// <summary>Gets the message.</summary>
			/// <value>The message.</value>
			public FreeTextMessageType Message
			{
				get { return this._message; }
			}
		}

		/// <summary>Process send scheduled message request context.</summary>
		public class ProcessSendScheduledMessageRequestContext : InstantMessageRequestContext
		{
			private ScheduledMessageType _message;
			private ILogManager _logManager;
			private bool _firstLogEntryGenerated;

			/// <summary>
			/// Initializes a new instance of the ProcessSendScheduledMessageRequestContext class.
			/// </summary>
			/// <param name="elementId">Identifier for the element.</param>
			/// <param name="requestId">Identifier for the request.</param>
			/// <param name="sessionId">Identifier for the session.</param>
			/// <param name="timeout">The timeout.</param>
			/// <param name="message">The scheduled message associated with the request.</param>
			/// <param name="logManager">Manager for logging into the history log.</param>
			public ProcessSendScheduledMessageRequestContext(string elementId, Guid requestId, Guid sessionId, uint timeout, ScheduledMessageType message, ILogManager logManager)
				: base(elementId, requestId, sessionId, timeout)
			{
				if (logManager == null)
				{
					throw new ArgumentNullException("logManager");
				}
				else if (requestId == Guid.Empty)
				{
					throw new ArgumentOutOfRangeException("requestId");
				}

				_logManager = logManager;
				_message = message;
			}

			/// <summary>
			/// Gets the scheduled message associated with the request.
			/// </summary>
			public ScheduledMessageType Message
			{
				get { return _message; }
			}

			/// <summary>Logs the status of the current request into the history log.</summary>
			/// <param name="messageStatus">The message status.</param>
			public override void LogStatusInHistoryLog(MessageStatusType messageStatus)
			{
				if (_firstLogEntryGenerated)
				{
					ResultCodeEnum result = _logManager.UpdateMessageStatus(ElementId, RequestId, messageStatus, CommandType.SendScheduledMessage);
					if (result == ResultCodeEnum.InvalidRequestID)
					{
						// Force the creation of the schedule message entry.
						_firstLogEntryGenerated = false;
					}
				}
				
				if (!_firstLogEntryGenerated)
				{
					_logManager.WriteLog(_message.FreeText, RequestId, CommandType.SendScheduledMessage, ElementId, messageStatus, _message.Period.StartDateTime, _message.Period.EndDateTime);
					_firstLogEntryGenerated = true;
				}
			}
		}

		/// <summary>Exception for signalling pis data store not accessible errors.</summary>
		internal class PisDataStoreNotAccessibleException : Exception
		{
			/// <summary>
			/// Initializes a new instance of the PisDataStoreNotAccessibleException class.
			/// </summary>
			public PisDataStoreNotAccessibleException()
				: base()
			{
			}

			/// <summary>
			/// Initializes a new instance of the PisDataStoreNotAccessibleException class.
			/// </summary>
			/// <param name="message">The message.</param>
			/// <param name="innerException">The inner exception.</param>
			public PisDataStoreNotAccessibleException(string message, Exception innerException)
				: base(message + " [" + innerException.Message + "]", innerException)
			{
			}
		}

#endregion

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
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
                Uninitialize(false);
            }
        }

        #endregion
    }
}