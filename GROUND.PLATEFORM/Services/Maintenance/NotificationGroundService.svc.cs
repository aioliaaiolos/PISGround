using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PIS.Ground.Maintenance.Notification;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.Data;
using System.Threading;

namespace PIS.Ground.Maintenance
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
                Thread.CurrentThread.Name = "Maint.NotificationGroundService";
            }

            MaintenanceService.Initialize();
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

                MaintenanceService.OnTrainNotification(
                    lRequestId,
                    request.NotificationId,
                    request.ElementId,
                    request.Parameter);
            }
            catch (System.Exception e)
            {
                LogManager.WriteLog(TraceType.EXCEPTION, e.Message, "PIS.Ground.Maintenance.NotificationGroundService.SendNotification", e, EventIdEnum.Maintenance);
            }

            return (new SendNotificationResponse()); //always empty response
        }
    }
}
