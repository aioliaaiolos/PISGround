using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Web;
using PIS.Ground.DataPackage;
using System.Threading;
using PIS.Ground.Core.Data;

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
            Guid requestId;
            try
            {
                int endPos = pNotification.RequestId.IndexOf('|');
                if (endPos != -1)
                {
                    requestId = new Guid(pNotification.RequestId.Substring(0, endPos));
                }
                else if (pNotification.RequestId.Length != 0)
                {
                    requestId = new Guid(pNotification.RequestId);
                }
                else
                {
                    requestId = Guid.Empty;
                }
            }
            catch (System.Exception ex)
            {
                DataPackageService.mWriteLog(TraceType.WARNING, "PIS.Ground.DataPackage.Notification.SendNotification", ex, Logs.WARNING_CANNOT_CONVERT_GUID_NOTIFICATION_REQUEST, pNotification.RequestId, pNotification.NotificationId, pNotification.ElementId);
                requestId = Guid.Empty;
            }

            DataPackageService.sendElementIdNotificationToGroundApp(requestId, (PIS.Ground.GroundCore.AppGround.NotificationIdEnum)pNotification.NotificationId, pNotification.ElementId);         

            return (new SendNotificationResponse()); // Always an empty response
        }
    }
}