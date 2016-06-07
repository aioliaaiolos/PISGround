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
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.Data;
using System.Threading;

namespace PIS.Ground.Infotainment.Journaling.Notification
{
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
                Thread.CurrentThread.Name = "Infotainment.NotificationGroundService";
            }

            JournalingService.Initialize();
        }

        public SendNotificationResponse SendNotification(SendNotificationRequest request)
        {
            try
            {
                Guid lRequestId = Guid.Empty;

                if (request.RequestId != "")
                {
                    lRequestId = new Guid(request.RequestId);
                }

                JournalingService.OnTrainNotification(
                    lRequestId,
                    request.NotificationId,
                    request.ElementId,
                    request.Parameter);
            }
            catch (System.Exception e)
            {
                LogManager.WriteLog(TraceType.EXCEPTION, "Exception in SendNotification thrown", "PIS.Ground.Infotainment.Journaling.Notification.NotificationGroundService", e, EventIdEnum.InfotainmentJournaling);
            }

            return (new SendNotificationResponse()); //always empty response
        }
    }
}
