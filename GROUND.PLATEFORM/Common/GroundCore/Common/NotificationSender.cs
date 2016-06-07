//---------------------------------------------------------------------------------------------------
// <copyright file="NotificationSender.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2013.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using PIS.Ground.Core.Data;
	using PIS.Ground.Core.LogMgmt;
	using PIS.Ground.Core.Properties;
	using PIS.Ground.Core.T2G.WebServices.FileTransfer;
	using PIS.Ground.Core.T2G.WebServices.Identification;
	using PIS.Ground.Core.T2G.WebServices.Notification;
	using PIS.Ground.Core.T2G.WebServices.VehicleInfo;
	using PIS.Ground.Core.Utility;
	using System.ComponentModel;
	using PIS.Ground.Core.SessionMgmt;
	using PIS.Ground.GroundCore.AppGround;
	using System.Globalization;

	/// <summary>Class used to send notification to PIS Ground clients.</summary>
	public class NotificationSender : INotificationSender
	{
		#region Private Variables

		/// <summary>Manager for session.</summary>
		private ISessionManager _sessionManager;

		#endregion

		#region Constructor

		/// <summary>Initializes a new instance of the NotificationSender class.</summary>
		/// <param name="sessionMgr">The session manager to use.</param>
		public NotificationSender(ISessionManager sessionMgr)
		{
			_sessionManager = sessionMgr;
		}

		#endregion

		#region Delegates

		private delegate void SendNotificationAsync(PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification, string pParameter, Guid pRequestID);

		private delegate void SendNotificationAppGroundAsync(string pRequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification, string pParameter, System.ServiceModel.Channels.Binding ingpBinding, System.ServiceModel.EndpointAddress pEndpointAddress, string pNotifcationURL);

		#endregion

		#region Private Methods

		/// <summary>Those notification are performed asynchronously as a gain of performance. The function calling it does not actually need to wait and know the result of this operation</summary>
		/// <param name="pRequestId">Unique ID of the request.</param>
		/// <param name="pNotification">Type of notification.</param>
		/// <param name="pParameter">Parameters of the notification to send.</param>
		/// <param name="pBinding">Represents an interoperable binding that supports distributed transactions and secure, reliable sessions.</param>
		/// <param name="pEndpointAddress">Provides a unique network address that a client uses to communicate with a service endpoint.</param>
		/// <param name="pNotifcationURL">Destination URL of the notification sent.</param>
		private void SendNotificationAppGroundTask(string pRequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification, string pParameter, System.ServiceModel.Channels.Binding pBinding, System.ServiceModel.EndpointAddress pEndpointAddress, string pNotifcationURL)
		{
			try
			{
				using (PIS.Ground.GroundCore.AppGround.NotificationAppGroundServiceClient lGroundNotificationClient = new PIS.Ground.GroundCore.AppGround.NotificationAppGroundServiceClient(pBinding, pEndpointAddress))
				{
					try
					{
						lGroundNotificationClient.SendNotification(pRequestId, pNotification, pParameter);
					}
					finally
					{
						if (lGroundNotificationClient.State == System.ServiceModel.CommunicationState.Faulted)
						{
							lGroundNotificationClient.Abort();
						}
					}
				}
			}
			catch (System.Net.WebException lEx)
			{
				LogManager.WriteLog(TraceType.INFO,
									"Sending notification " + pNotification.ToString() + " to " + pNotifcationURL + " failed.",
									"PIS.Ground.GroundCore.Common.NotificationSender",
									lEx,
									EventIdEnum.GroundCore);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(TraceType.ERROR,
									"Sending notification " + pNotification.ToString() + " to " + pNotifcationURL + " failed.",
									"PIS.Ground.GroundCore.Common.NotificationSender",
									ex,
									EventIdEnum.GroundCore);
			}
		}

		/// <summary>
		/// Notification are required to be performed asynchronously as not reply are required.
		/// </summary>
		/// <param name="pNotification">Type of notification.</param>
		/// <param name="pParameter">Parameters of the notification to send.</param>
		/// <param name="pRequestID">Identifier for the request.</param>
		private void SendNotificationTask(PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification, string pParameter, Guid pRequestID)
		{
			try
			{
				if (!string.IsNullOrEmpty(PIS.Ground.Core.Utility.ServiceConfiguration.SessionSqLiteDBPath))
				{
					List<string> lNotificationURLs = new List<string>();
					string lParameter = String.Empty;
					string lGetResult;

					//If no request ID is specified or is null, get all URLs
					if (pRequestID == Guid.Empty)
					{
						//if no request, get all notification URLs
						lGetResult = _sessionManager.GetNotificationUrls(lNotificationURLs);
					}
					else
					{
						//Get notification URL for the requestID
						string NotifURL;
						lGetResult = _sessionManager.GetNotificationUrlByRequestId(pRequestID, out NotifURL);
						if (!string.IsNullOrEmpty(NotifURL))
						{
							lNotificationURLs.Add(NotifURL);
						}
					}

					if (0 != lNotificationURLs.Count)
					{
						string lRequestId;

						if (pRequestID == null)
						{
							lRequestId = new Guid().ToString();
						}
						else
						{
							lRequestId = pRequestID.ToString();
						}

						// EndPoint Configuration
						System.ServiceModel.Channels.Binding lBinding = getBinding();

						if (!string.IsNullOrEmpty(pParameter))
						{
							lParameter = pParameter;
						}

						// Send notifications
						try
						{
							foreach (string lNotifcationURL in lNotificationURLs.Distinct())
							{
								if (!string.IsNullOrEmpty(lNotifcationURL))
								{
									System.ServiceModel.EndpointAddress lEndpointAddress = new System.ServiceModel.EndpointAddress(lNotifcationURL);

									SendNotificationAppGroundAsync worker = new SendNotificationAppGroundAsync(SendNotificationAppGroundTask);
									worker.BeginInvoke(lRequestId, pNotification, lParameter, lBinding, lEndpointAddress, lNotifcationURL, worker.EndInvoke, null);
								}
								else
								{
									// Url is empty, don't send anything.
								}
							}
						}
						catch (Exception ex)
						{
							LogManager.WriteLog(TraceType.ERROR,
								ex.Message,
								"PIS.Ground.GroundCore.Common.NotificationSender",
								ex,
								EventIdEnum.GroundCore);
						}
					}
					else
					{
						LogManager.WriteLog(TraceType.INFO,
													"NotificationURLs list is empty.",
													"PIS.Ground.GroundCore.Common.NotificationSender",
													null,
													EventIdEnum.GroundCore);
					}
				}
			}
			catch (System.Exception exception)
			{
				LogManager.WriteLog(TraceType.EXCEPTION,
									string.Format(CultureInfo.InvariantCulture, "unexpected error while handling notification of type {0} for request {1}", pNotification, pRequestID),
									"PIS.Ground.GroundCore.Common.NotificationSender",
									exception,
									EventIdEnum.GroundCore);
			}
		}

		#endregion

		#region Public Methods

		/// <summary>Sends a notification.</summary>
		/// <param name="pNotification">The notification.</param>
		public void SendNotification(PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification)
		{
			SendNotification(pNotification, null, Guid.Empty);
		}

		/// <summary>Sends a notification asynchronously.</summary>
		/// <param name="pNotification">The notification.</param>
		/// <param name="pParameter">Parameter sent with the notification, can be a simple string or an XML serialize data.</param>
		public void SendNotification(PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification, string pParameter)
		{
			SendNotification(pNotification, pParameter, Guid.Empty);
		}

		/// <summary>Sends a notification asynchronously.</summary>
		/// <param name="pNotification">The notification.</param>
		/// <param name="pRequestID">Unique identifier for the request.</param>
		public void SendNotification(PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification, Guid pRequestID)
		{
			SendNotification(pNotification, null, pRequestID);
		}

		/// <summary>Sends a notification asynchronously.</summary>
		/// <param name="pNotification">The notification.</param>
		/// <param name="pParameter">Parameter sent with the notification, can be a simple string or an
		/// XML serialize data.</param>
		/// <param name="pRequestID">Unique identifier for the request.</param>
		public void SendNotification(PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification, string pParameter, Guid pRequestID)
		{
			//Function is performed Asynchronously.
			SendNotificationAsync worker = new SendNotificationAsync(SendNotificationTask);
			worker.BeginInvoke(pNotification, pParameter, pRequestID, worker.EndInvoke, null);
		}

		/// <summary>Gets the binding to send notifications to GroundApp.</summary>
		/// <returns>The binding.</returns>
		private static System.ServiceModel.Channels.Binding getBinding()
		{
			System.Configuration.Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/CommonConfiguration");

			var serviceModel = System.ServiceModel.Configuration.ServiceModelSectionGroup.GetSectionGroup(config);
			System.ServiceModel.Configuration.BindingsSection section = serviceModel.Bindings;

			foreach (var bindingCollection in section.BindingCollections)
			{
				if (bindingCollection.ConfiguredBindings.Count > 0 && bindingCollection.ConfiguredBindings[0].Name == "NotificationAppGroundBinding")
				{
					var bindingElement = bindingCollection.ConfiguredBindings[0];
					var binding = (System.ServiceModel.Channels.Binding)Activator.CreateInstance(bindingCollection.BindingType);
					binding.Name = bindingElement.Name;
					bindingElement.ApplyConfiguration(binding);

					return binding;
				}
			}

			return null;
		}

		/// <summary>Sends a t 2 g server connection status.</summary>
		/// <param name="pStatus">True to status.</param>
		public void SendT2GServerConnectionStatus(bool pStatus)
		{
			if (pStatus)
			{
				SendNotification(NotificationIdEnum.CommonT2GServerOnline);
			}
			else
			{
				SendNotification(NotificationIdEnum.CommonT2GServerOffline);
			}
		}
	}

		#endregion
}