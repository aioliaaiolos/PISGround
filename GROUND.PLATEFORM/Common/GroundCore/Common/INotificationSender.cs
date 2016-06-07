//---------------------------------------------------------------------------------------------------
// <copyright file="INotificationSender.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
namespace PIS.Ground.Core.Common
{
	/// <summary>Interface for notification sender.</summary>
	public interface INotificationSender
	{

        /// <summary>Sends a notification asynchronously</summary>
        /// <param name="pNotification">The notification.</param>
        /// <param name="pParameter">Parameter sent with the notification, can be a simple string or an XML serialize data.</param>
        /// <param name="pRequestId">Unique ID of the request</param>
        void SendNotification(PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification, string pParameter, Guid pRequestID);

		/// <summary>Sends a notification.</summary>
		/// <param name="pNotification">The notification.</param>
		/// <param name="pParameter">The parameter.</param>
		void SendNotification(PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification, string pParameter);

        /// <summary>Sends a notification.</summary>
        /// <param name="pNotification">The notification.</param>
        /// <param name="pRequestId">Unique ID of the request</param>
        void SendNotification(PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification, Guid pRequestID);

		/// <summary>Sends a notification.</summary>
		/// <param name="pNotification">The notification.</param>
		void SendNotification(PIS.Ground.GroundCore.AppGround.NotificationIdEnum pNotification);

		/// <summary>Sends a t 2 g server connection status.</summary>
		/// <param name="pStatus">true to status.</param>
		void SendT2GServerConnectionStatus(bool pStatus);
	}
}
