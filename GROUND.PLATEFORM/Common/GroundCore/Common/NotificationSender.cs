//---------------------------------------------------------------------------------------------------
// <copyright file="NotificationSender.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.GroundCore.AppGround;

namespace PIS.Ground.Core.Common
{

	/// <summary>Class used to send notification to PIS Ground clients.</summary>
	public class NotificationSender : INotificationSender
	{
		#region Private Variables

		/// <summary>Manager for session.</summary>
		private ISessionManager _sessionManager;

        /// <summary>
        /// Collection that keep previous exception intercepted.
        /// </summary>
        private ExceptionMemoryCollection _previousExceptions = new ExceptionMemoryCollection();

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

		private delegate void SendNotificationAppGroundAsync(Guid pRequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification, string pParameter, System.ServiceModel.Channels.Binding ingpBinding, System.ServiceModel.EndpointAddress pEndpointAddress, string pNotifcationURL);

		#endregion

		#region Private Methods

		/// <summary>Those notification are performed asynchronously as a gain of performance. The function calling it does not actually need to wait and know the result of this operation</summary>
		/// <param name="pRequestId">Unique ID of the request.</param>
		/// <param name="pNotification">Type of notification.</param>
		/// <param name="pParameter">Parameters of the notification to send.</param>
		/// <param name="pBinding">Represents an interoperable binding that supports distributed transactions and secure, reliable sessions.</param>
		/// <param name="pEndpointAddress">Provides a unique network address that a client uses to communicate with a service endpoint.</param>
		/// <param name="pNotifcationURL">Destination URL of the notification sent.</param>
		private void SendNotificationAppGroundTask(Guid pRequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification, string pParameter, System.ServiceModel.Channels.Binding pBinding, System.ServiceModel.EndpointAddress pEndpointAddress, string pNotifcationURL)
		{
			try
			{
				using (PIS.Ground.GroundCore.AppGround.NotificationAppGroundServiceClient lGroundNotificationClient = new PIS.Ground.GroundCore.AppGround.NotificationAppGroundServiceClient(pBinding, pEndpointAddress))
				{
					try
					{
						lGroundNotificationClient.SendNotification(pRequestId.ToString(), pNotification, pParameter);
                        lGroundNotificationClient.Close();
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
			catch (Exception ex)
			{
                LogException(pNotifcationURL, ex, pRequestId, pNotification, pParameter);
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
                        // EndPoint Configuration
                        System.ServiceModel.Channels.Binding lBinding = getBinding();

                        if (!string.IsNullOrEmpty(pParameter))
                        {
                            lParameter = pParameter;
                        }

                        // Send notifications
                        foreach (string lNotifcationURL in lNotificationURLs.Distinct())
                        {
                            // Send notification only if we have a non empty url.
                            if (!string.IsNullOrEmpty(lNotifcationURL))
                            {
                                try
                                {
                                    System.ServiceModel.EndpointAddress lEndpointAddress = new System.ServiceModel.EndpointAddress(lNotifcationURL);

                                    SendNotificationAppGroundAsync worker = new SendNotificationAppGroundAsync(SendNotificationAppGroundTask);
                                    worker.BeginInvoke(pRequestID, pNotification, lParameter, lBinding, lEndpointAddress, lNotifcationURL, worker.EndInvoke, null);
                                }
                                catch (Exception exception)
                                {
                                    LogException(lNotifcationURL, exception, pRequestID, pNotification, pParameter);
                                }
                            }
                        }
                    }
                    else
                    {
                        LogManager.WriteLog(TraceType.DEBUG,
                                                    "NotificationURLs list is empty.",
                                                    "PIS.Ground.GroundCore.Common.NotificationSender",
                                                    null,
                                                    EventIdEnum.SendNotification);
                    }
				}
			}
			catch (System.Exception exception)
			{
                LogException(string.Empty, exception, pRequestID, pNotification, pParameter);
			}
		}

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="notificationUrl">The notification URL.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="id">The notification identifier send.</param>
        private void LogException(string notificationUrl, Exception exception, Guid requestId, NotificationIdEnum id, string parameters)
        {
            TraceType traceType = _previousExceptions.IsFirstOccurrence(notificationUrl??string.Empty, exception) ? TraceType.ERROR : TraceType.DEBUG;

            if (LogManager.IsTraceActive(traceType))
            {
                if (IsPredictableException(exception))
                {
                    string messageFormat = (string.IsNullOrEmpty(parameters)) ? Properties.Resources.SendNotificationPredictableMessageWithoutParameter : Properties.Resources.SendNotificationPredictableMessage;
                    string errorMessage = string.Format(CultureInfo.CurrentCulture, messageFormat, notificationUrl, id, exception.Message, requestId, parameters);
                    LogManager.WriteLog(traceType, errorMessage, "PIS.Ground.GroundCore.Common.NotificationSender", null, EventIdEnum.SendNotification);
                    
                }
                else
                {
                    string messageFormat = (string.IsNullOrEmpty(parameters)) ? Properties.Resources.SendNotificationErrorMessageWithoutParameter : Properties.Resources.SendNotificationErrorMessage;
                    string errorMessage = string.Format(CultureInfo.CurrentCulture, messageFormat, notificationUrl, id, requestId, parameters);
                    LogManager.WriteLog(traceType, errorMessage, "PIS.Ground.GroundCore.Common.NotificationSender", exception, EventIdEnum.SendNotification);
                }
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

        /// <summary>
        /// Determines whether is specified exception object is considered as predictable error or not.
        /// </summary>
        /// <param name="ex">The exception object to evaluate.</param>
        /// <returns>
        ///   <c>true</c> if specified exception object is considered as predictable error, false otherwise.
        /// </returns>
        public static bool IsPredictableException(Exception ex)
        {
            return (ex is FaultException
                    || ex is TimeoutException
                    || ex is EndpointNotFoundException
                    || ex is ActionNotSupportedException
                    || ex is ServerTooBusyException
                    || ex is ProtocolException
                    || ex is AddressAccessDeniedException
                    || ex is ChannelTerminatedException
                    || ex is SecurityNegotiationException
                    || ex is SecurityAccessDeniedException
                    || ex is MessageSecurityException
                );
        }
	}

		#endregion
}