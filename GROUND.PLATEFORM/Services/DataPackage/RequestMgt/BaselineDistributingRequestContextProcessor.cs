//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineForcingRequestContextProcessor.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.T2G;
using PIS.Ground.DataPackage.RemoteDataStoreFactory;
using PIS.Ground.RemoteDataStore;

namespace PIS.Ground.DataPackage.RequestMgt
{
	/// <summary>Baseline forcing request context processor.</summary>
	public class BaselineDistributingRequestContextProcessor : IRequestContextProcessor
	{
		/// <summary>The string list XML serializer.</summary>
		private static System.Xml.Serialization.XmlSerializer _stringListXmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(List<string>));

		/// <summary>The remote data store factory.</summary>
		private IRemoteDataStoreFactory _remoteDataStoreFactory;

		/// <summary>Manager for train to ground.</summary>
		private IT2GManager _trainToGroundManager;

		/// <summary>
		/// Initializes a new instance of the BaselineDistributingRequestContextProcessor class.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
		/// <param name="remoteDataStoreFactory">The remote data store factory.</param>
		/// <param name="trainToGroundManager">Manager for train to ground.</param>
		public BaselineDistributingRequestContextProcessor(IRemoteDataStoreFactory remoteDataStoreFactory, IT2GManager trainToGroundManager)
		{
			if (null == remoteDataStoreFactory)
			{
				throw new ArgumentNullException("remoteDataStoreFactory");
			}

			_remoteDataStoreFactory = remoteDataStoreFactory;

			if (null == trainToGroundManager)
			{
				throw new ArgumentNullException("trainToGroundManager");
			}

			_trainToGroundManager = trainToGroundManager;
		}

		/// <summary>Process the distribute request described by request.</summary>
		/// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
		/// <param name="request">The request.</param>
		private void ProcessDistributeRequest(IRequestContext request)
		{
			BaselineDistributingRequestContext requestContext = request as BaselineDistributingRequestContext;

			if (requestContext == null)
			{
				throw new ArgumentException(Logs.ERROR_INVALID_BASELINE_DISTRIBUTE_REQUEST_CONTEXT, "request");
			}

			List<string> parametersList = new List<string>()
								{
									requestContext.ElementId,
									requestContext.BaselineVersion
								};
			using (StringWriter stringWriter = new StringWriter())
			{
				_stringListXmlSerializer.Serialize(stringWriter, parametersList);

				if (requestContext.TransferAttemptsDone == 1)
				{
					DataPackageService.sendNotificationToGroundApp(
						request.RequestId.ToString(),
						PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionPending,
						stringWriter.ToString());
				}

				if (_trainToGroundManager.IsElementOnlineAndPisBaselineUpToDate(requestContext.ElementId))
				{
					ServiceInfo serviceInfo;
					if (T2GManagerErrorEnum.eSuccess == _trainToGroundManager.GetAvailableServiceData(requestContext.ElementId, (int)Core.Common.eServiceID.eSrvSIF_DataPackageServer, out serviceInfo))
					{
						// Rebuild the url of the service in case that it has changed since the last invocation.
						requestContext.Endpoint = "http://" + serviceInfo.ServiceIPAddress + ":" + serviceInfo.ServicePortNumber;
						try
						{
							using (PIS.Ground.DataPackage.DataPackageTrainServiceClient lTrainDataPackageClient = new PIS.Ground.DataPackage.DataPackageTrainServiceClient("DataPackageEndpoint", requestContext.Endpoint))
							{
								try
								{
									List<RecipientId> recipients = new List<RecipientId>()
								{
									new RecipientId()
									{
										ApplicationId = ConfigurationSettings.AppSettings["ApplicationId"],
										SystemId = requestContext.ElementId,
										MissionId = string.Empty
									}
								};

									DataPackageService.mWriteLog(TraceType.INFO, System.Reflection.MethodBase.GetCurrentMethod().Name, null, Logs.INFO_FUTURE_BASELINE, requestContext.ElementId, requestContext.BaselineVersion);

									if (!string.IsNullOrEmpty(requestContext.BaselineVersion))
									{
										List<PackageParams> packagesParamsList;
										List<string> filesUrlsList;
										if (GetBaselineFilesURLs(requestContext.RequestId, requestContext.ElementId, requestContext.BaselineVersion, requestContext.IsIncremental, true, out filesUrlsList, out packagesParamsList))
										{
											try
											{
												using (var remoteDataStore = _remoteDataStoreFactory.GetRemoteDataStoreInstance() as RemoteDataStoreProxy)
												{
													try
													{
														filesUrlsList.Add(ConfigurationSettings.AppSettings["RemoteDataStoreUrl"]
															+ remoteDataStore.createBaselineFile(
																requestContext.RequestId,
																requestContext.ElementId,
																requestContext.BaselineVersion,
																requestContext.BaselineActivationDate.ToString(),
																requestContext.BaselineExpirationDate.ToString()));
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
											catch (Exception ex)
											{
												DataPackageService.mWriteLog(TraceType.EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_REMOTEDATASTORE_FAULTED);
											}

											if (filesUrlsList.Count > 0)
											{
												UploadFileDistributionRequest uploadFilesRequestContext = new UploadFileDistributionRequest(
													requestContext.RequestId,
													requestContext.RequestId.ToString(),
													requestContext.DistributionAttributes.transferExpirationDate,
													filesUrlsList,
													requestContext.DistributionAttributes.fileCompression,
													recipients,
													requestContext.DistributionAttributes.transferDate,
													"Distribute baseline for element " + requestContext.ElementId,
													requestContext.DistributionAttributes.TransferMode,
													requestContext.DistributionAttributes.priority,
													new EventHandler<FileDistributionStatusArgs>(DataPackageService.OnFileDistributeNotification));

												DataPackageService.sendNotificationToGroundApp(requestContext.RequestId.ToString(), PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageFutureBaselineDefinition, stringWriter.ToString());

												string logMessage = "Distribute baseline for element " + requestContext.ElementId;
												logMessage += ". Files to upload : ";
												foreach (string file in filesUrlsList)
												{
													logMessage += file + ", ";
												}

												logMessage = logMessage.Substring(0, logMessage.Length - 2);
												DataPackageService.mWriteLog(TraceType.INFO, System.Reflection.MethodBase.GetCurrentMethod().Name, null, logMessage);

												AvailableElementData elementData;
												T2GManagerErrorEnum lRqstResult = _trainToGroundManager.GetAvailableElementDataByElementNumber(requestContext.ElementId, out elementData);

												if (lRqstResult == T2GManagerErrorEnum.eSuccess &&
													elementData != null && elementData.PisBaselineData != null)
												{
													BaselineStatusUpdater.ProcessDistributeBaselineRequest(
														elementData.ElementNumber,
														requestContext.RequestId,
														elementData.OnlineStatus,
														elementData.PisBaselineData.CurrentVersionOut,
														elementData.PisBaselineData.FutureVersionOut,
														requestContext.BaselineVersion);
												}
												else
												{
													DataPackageService.mWriteLog(TraceType.ERROR, System.Reflection.MethodBase.GetCurrentMethod().Name, null, Logs.ERROR_ELEMENT_INFO, requestContext.ElementId);
												}

												DataPackageService.mAddBaselineVersionToDictionary(requestContext.RequestId, requestContext.ElementId, requestContext.BaselineVersion);
												DataPackageService.mAddPackagesToUsedPackagesList(packagesParamsList);
												_trainToGroundManager.T2GFileDistributionManager.AddUploadRequest(uploadFilesRequestContext);
												requestContext.TransmissionStatus = true;
												DataPackageService.mRemovePackagesFromUsedPackagesList(packagesParamsList);
											}
										}
										else
										{
											requestContext.CompletionStatus = true;
											DataPackageService.sendNotificationToGroundApp(requestContext.RequestId.ToString(), PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailed, stringWriter.ToString());
											DataPackageService.mWriteLog(TraceType.ERROR, System.Reflection.MethodBase.GetCurrentMethod().Name, null, Logs.ERROR_DISTRIBUTE_BASELINE_FAILED, requestContext.BaselineVersion, requestContext.ElementId, Logs.ERROR_GETTING_URL_LIST);
										}
									}
									else
									{
										requestContext.CompletionStatus = true;
										DataPackageService.sendNotificationToGroundApp(requestContext.RequestId.ToString(), PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailed, stringWriter.ToString());
										DataPackageService.mWriteLog(TraceType.ERROR, System.Reflection.MethodBase.GetCurrentMethod().Name, null, Logs.ERROR_DISTRIBUTE_BASELINE_FAILED_UNKNOW_BASELINE_VERSION, requestContext.ElementId);
									}
								}
								catch (Exception ex)
								{
									if (false == requestContext.OnCommunicationError(ex))
									{
										requestContext.CompletionStatus = true;
										DataPackageService.sendNotificationToGroundApp(requestContext.RequestId.ToString(), PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedRejectedByElement, stringWriter.ToString());
										DataPackageService.mWriteLog(TraceType.ERROR, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_DISTRIBUTE_BASELINE_FAILED, requestContext.BaselineVersion, requestContext.ElementId, ex.Message);
									}
									else
									{
										DataPackageService.mWriteLog(TraceType.DEBUG, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_DISTRIBUTE_BASELINE_FAILED, requestContext.BaselineVersion, requestContext.ElementId, ex.Message);
									}
								}
								finally
								{
									if (lTrainDataPackageClient.State == CommunicationState.Faulted)
									{
										lTrainDataPackageClient.Abort();
									}
								}
							}
						}
						catch (Exception ex)
						{
							if (false == requestContext.OnCommunicationError(ex))
							{
								requestContext.CompletionStatus = true;
								DataPackage.DataPackageService.mWriteLog(TraceType.ERROR, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_DISTRIBUTE_BASELINE_FAILED, requestContext.BaselineVersion, requestContext.ElementId, ex.Message);
								DataPackageService.sendNotificationToGroundApp(requestContext.RequestId.ToString(), PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailed, stringWriter.ToString());
							}
							else
							{
								DataPackage.DataPackageService.mWriteLog(TraceType.DEBUG, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_DISTRIBUTE_BASELINE_FAILED, requestContext.BaselineVersion, requestContext.ElementId, ex.Message);
							}
						}
					}
					else
					{
						DataPackage.DataPackageService.mWriteLog(TraceType.ERROR, System.Reflection.MethodBase.GetCurrentMethod().Name, null, Logs.ERROR_DISTRIBUTE_BASELINE_FAILED, requestContext.BaselineVersion, requestContext.ElementId, "Cannot get embedded DataPackage service data.");
						requestContext.TransmissionStatus = false;
					}
				}
				else
				{
					requestContext.TransmissionStatus = false;
				}
			}
		}

		/// <summary>Gets baseline files urls.</summary>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="baselineVersion">The baseline version.</param>
		/// <param name="isIncremental">True if this object is incremental.</param>
		/// <param name="notifyGroundApp">True to notify ground application.</param>
		/// <param name="packagesURLs">[Out] The packages ur ls.</param>
		/// <param name="packagesParams">[Out] Options for controlling the packages.</param>
		/// <returns>True if it succeeds, false if it fails.</returns>
		private bool GetBaselineFilesURLs(
			Guid requestId,
			string elementId,
			string baselineVersion,
			bool isIncremental,
			bool notifyGroundApp,
			out List<string> packagesURLs,
			out List<PackageParams> packagesParams)
		{
			bool result = false;
			packagesParams = new List<PackageParams>();
			packagesURLs = new List<string>();
			BaselineDefinition baselineDefinition = null;
			List<string> parametersList = new List<string>()
				{
					elementId,
					baselineVersion
				};
			using (StringWriter stringWriter = new StringWriter())
			{
				_stringListXmlSerializer.Serialize(stringWriter, parametersList);

				if (!string.IsNullOrEmpty(baselineVersion))
				{
					try
					{
						using (var remoteDataStore = _remoteDataStoreFactory.GetRemoteDataStoreInstance() as RemoteDataStoreProxy)
						{
							try
							{
								if (remoteDataStore.checkIfBaselineExists(baselineVersion))
								{
									baselineDefinition = DataTypeConversion.fromDataContainerToBaselineDefinition(remoteDataStore.getBaselineDefinition(baselineVersion));
								}

								if (baselineDefinition == null)
								{
									DataPackageService.sendNotificationToGroundApp(requestId.ToString(), PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedMissingDataPackage, stringWriter.ToString());
									DataPackageService.mWriteLog(TraceType.ERROR, System.Reflection.MethodBase.GetCurrentMethod().Name, null, Logs.ERROR_INVALID_BASELINE_VERSION, baselineVersion);
									result = false;
								}
								else if (remoteDataStore.checkIfDataPackageExists(DataPackageType.PISBASE.ToString(), baselineDefinition.PISBaseDataPackageVersion) == false ||
									remoteDataStore.checkIfDataPackageExists(DataPackageType.PISMISSION.ToString(), baselineDefinition.PISMissionDataPackageVersion) == false ||
									remoteDataStore.checkIfDataPackageExists(DataPackageType.PISINFOTAINMENT.ToString(), baselineDefinition.PISInfotainmentDataPackageVersion) == false ||
									remoteDataStore.checkIfDataPackageExists(DataPackageType.LMT.ToString(), baselineDefinition.LMTDataPackageVersion) == false)
								{
									if (notifyGroundApp)
									{
										DataPackageService.sendNotificationToGroundApp(requestId.ToString(), PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionFailedMissingDataPackage, stringWriter.ToString());
									}

									DataPackageService.mWriteLog(TraceType.ERROR, System.Reflection.MethodBase.GetCurrentMethod().Name, null, Logs.ERROR_INVALID_BASELINE_VERSION, baselineVersion);
									result = false;
								}
								else
								{
									AvailableElementData elementData;
									T2GManagerErrorEnum requestResult = _trainToGroundManager.GetAvailableElementDataByElementNumber(elementId, out elementData);

									if (requestResult == T2GManagerErrorEnum.eT2GServerOffline)
									{
										DataPackageService.sendNotificationToGroundApp(requestId.ToString(), PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageT2GServerOffline, string.Empty);
										DataPackageService.mWriteLog(TraceType.ERROR, System.Reflection.MethodBase.GetCurrentMethod().Name, null, Logs.ERROR_T2G_SERVER_OFFLINE);
									}
									else if (requestResult == T2GManagerErrorEnum.eElementNotFound)
									{
										DataPackageService.sendNotificationToGroundApp(requestId.ToString(), PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageDistributionUnknowElementId, stringWriter.ToString());
										DataPackageService.mWriteLog(TraceType.ERROR, System.Reflection.MethodBase.GetCurrentMethod().Name, null, Logs.ERROR_ELEMENT_NOT_FOUND, elementId);
									}

									Dictionary<DataPackageType, string> uploadPackagesList = new Dictionary<DataPackageType, string>();
									BaselineDefinition embeddedBaselineDefinition = null;

									if (elementData.PisBaselineData != null &&
										!string.IsNullOrEmpty(elementData.PisBaselineData.CurrentVersionOut) &&
										elementData.PisBaselineData.CurrentVersionOut != "0.0.0.0")
									{
										if (remoteDataStore.checkIfBaselineExists(elementData.PisBaselineData.CurrentVersionOut))
										{
											embeddedBaselineDefinition = DataTypeConversion.fromDataContainerToBaselineDefinition(remoteDataStore.getBaselineDefinition(elementData.PisBaselineData.CurrentVersionOut));
										}
										else
										{
											string message = string.Format(CultureInfo.CurrentCulture, "Baseline version '{0}' installed on element '{1}' is unknown. Full baseline distribution will be performed.", elementData.PisBaselineData.CurrentVersionOut, elementData.ElementNumber);
											DataPackageService.mWriteLog(TraceType.WARNING, System.Reflection.MethodBase.GetCurrentMethod().Name, null, message);
										}

										if (embeddedBaselineDefinition != null)
										{
											if (baselineDefinition.PISBaseDataPackageVersion != embeddedBaselineDefinition.PISBaseDataPackageVersion)
											{
												uploadPackagesList.Add(DataPackageType.PISBASE, baselineDefinition.PISBaseDataPackageVersion);
											}

											if (baselineDefinition.PISMissionDataPackageVersion != embeddedBaselineDefinition.PISMissionDataPackageVersion)
											{
												uploadPackagesList.Add(DataPackageType.PISMISSION, baselineDefinition.PISMissionDataPackageVersion);
											}

											if (baselineDefinition.PISInfotainmentDataPackageVersion != embeddedBaselineDefinition.PISInfotainmentDataPackageVersion)
											{
												uploadPackagesList.Add(DataPackageType.PISINFOTAINMENT, baselineDefinition.PISInfotainmentDataPackageVersion);
											}

											if (baselineDefinition.LMTDataPackageVersion != embeddedBaselineDefinition.LMTDataPackageVersion)
											{
												uploadPackagesList.Add(DataPackageType.LMT, baselineDefinition.LMTDataPackageVersion);
											}
										}
									}

									if (embeddedBaselineDefinition == null)
									{
										uploadPackagesList.Add(DataPackageType.PISBASE, baselineDefinition.PISBaseDataPackageVersion);
										uploadPackagesList.Add(DataPackageType.PISMISSION, baselineDefinition.PISMissionDataPackageVersion);
										uploadPackagesList.Add(DataPackageType.PISINFOTAINMENT, baselineDefinition.PISInfotainmentDataPackageVersion);
										uploadPackagesList.Add(DataPackageType.LMT, baselineDefinition.LMTDataPackageVersion);
									}

									if (isIncremental == false || embeddedBaselineDefinition == null)
									{
										foreach (KeyValuePair<DataPackageType, string> lDP in uploadPackagesList)
										{
											DataPackagesCharacteristics packageCharacteristics = DataTypeConversion.fromDataContainerToDataPackagesCharacteristics(remoteDataStore.getDataPackageCharacteristics(lDP.Key.ToString(), lDP.Value));

											packagesURLs.Add(ConfigurationSettings.AppSettings["RemoteDataStoreUrl"] + "/" + packageCharacteristics.DataPackagePath);
											packagesParams.Add(new PackageParams(lDP.Key, lDP.Value));
										}

										result = true;
									}
									else
									{
										foreach (KeyValuePair<DataPackageType, string> packageToUpload in uploadPackagesList)
										{
											string embeddedBaselineVersion = string.Empty;
											switch (packageToUpload.Key)
											{
												case DataPackageType.PISBASE:
													embeddedBaselineVersion = embeddedBaselineDefinition.PISBaseDataPackageVersion;
													break;
												case DataPackageType.PISMISSION:
													embeddedBaselineVersion = embeddedBaselineDefinition.PISMissionDataPackageVersion;
													break;
												case DataPackageType.PISINFOTAINMENT:
													embeddedBaselineVersion = embeddedBaselineDefinition.PISInfotainmentDataPackageVersion;
													break;
												case DataPackageType.LMT:
													embeddedBaselineVersion = embeddedBaselineDefinition.LMTDataPackageVersion;
													break;
												default:
													break;
											}

											bool isEmbPackageOnGround = remoteDataStore.checkIfDataPackageExists(packageToUpload.Key.ToString(), embeddedBaselineVersion);
											if (!isEmbPackageOnGround)
											{
												string message = string.Format(CultureInfo.CurrentCulture, "{0}'s data package version '{1}' associated to baseline '{2}' is unknown. Distribution the element '{3}' will receive a complete update for that data package.", packageToUpload.Key.ToString(), embeddedBaselineVersion, packageToUpload.Value, elementData.ElementNumber);
												DataPackageService.mWriteLog(TraceType.WARNING, System.Reflection.MethodBase.GetCurrentMethod().Name, null, message);

												DataPackagesCharacteristics packageCharacteristics = DataTypeConversion.fromDataContainerToDataPackagesCharacteristics(remoteDataStore.getDataPackageCharacteristics(packageToUpload.Key.ToString(), packageToUpload.Value));

												packagesURLs.Add(ConfigurationSettings.AppSettings["RemoteDataStoreUrl"] + "/" + packageCharacteristics.DataPackagePath);
												packagesParams.Add(new PackageParams(packageToUpload.Key, packageToUpload.Value));
											}
											else
											{
                                                // Store the current operational timeout.
                                                TimeSpan oldOperationTimeOut = remoteDataStore.InnerChannel.OperationTimeout;
                                                try
                                                {
                                                    // Set the operational timeout to 10minutes because the getDiffDataPackageUrl can take a long time (Zip operation) 
                                                    remoteDataStore.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(10);
                                                    string differentialPackagePath = remoteDataStore.getDiffDataPackageUrl(requestId, elementId, packageToUpload.Key.ToString(), embeddedBaselineVersion, packageToUpload.Value);
                                                    if (null != differentialPackagePath && !string.IsNullOrEmpty(differentialPackagePath))
                                                    {
                                                        packagesURLs.Add(ConfigurationSettings.AppSettings["RemoteDataStoreUrl"] + "/" + differentialPackagePath);
                                                        packagesParams.Add(new PackageParams(packageToUpload.Key, packageToUpload.Value));
                                                    }
                                                }
                                                finally
                                                {
                                                    // restore the operation timeout.
                                                    remoteDataStore.InnerChannel.OperationTimeout = oldOperationTimeOut;
                                                }
											}

											result = true;
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
					catch (Exception ex)
					{
						DataPackageService.mWriteLog(TraceType.ERROR, System.Reflection.MethodBase.GetCurrentMethod().Name, ex, Logs.ERROR_REMOTEDATASTORE_NOT_ACCESSIBLE, baselineVersion);
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}

			return result;
		}

		#region IRequestContextProcessor Members

		/// <summary>Gets the process callback.</summary>
		/// <value>The process.</value>
		ProcessDelegate IRequestContextProcessor.Process
		{
			get
			{
				return ProcessDistributeRequest;
			}
		}

		#endregion
	}
}