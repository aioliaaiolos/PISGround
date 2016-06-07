using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Web;
using PIS.Ground.DataPackage;
using System.Threading;

namespace PIS.Ground.DataPackage.Notification
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
                Thread.CurrentThread.Name = "DataPkg.NotificationGroundService";
            }

            DataPackageService.Initialize();
        }

        public SendNotificationResponse SendNotification(SendNotificationRequest pNotification)
        {
            DataPackageService.sendElementIdNotificationToGroundApp(pNotification.RequestId, (PIS.Ground.GroundCore.AppGround.NotificationIdEnum)pNotification.NotificationId, pNotification.ElementId);         

            return (new SendNotificationResponse()); // Always an empty response
        }
    }
}