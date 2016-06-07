//---------------------------------------------------------------------------------------------------
// <copyright file="NotificationGroundService.svc.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PIS.Ground.LiveVideoControl.Notification;
using System.Threading;

namespace PIS.Ground.LiveVideoControl
{
	/// <summary>Notification ground service.</summary>
	[ServiceBehavior(Namespace = "http://alstom.com/pacis/pis/ground/notification/")]
	public class NotificationGroundService : INotificationGroundService
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationGroundService"/> class.
        /// </summary>
        public NotificationGroundService()
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "LiveVideo.NotificationGroundService";
            }

            LiveVideoControlService.Initialize();
        }

		/// <summary>Sends a notification.</summary>
		/// <param name="request">The input request.</param>
		/// <returns>An empty response.</returns>
		public SendNotificationResponse SendNotification(SendNotificationRequest request)
		{
            LiveVideoControlService.SendElementIdNotificationToGroundApp(request.RequestId, (PIS.Ground.GroundCore.AppGround.NotificationIdEnum)request.NotificationId, request.ElementId);

			return new SendNotificationResponse(); // Always an empty response
		}
	}
}