//---------------------------------------------------------------------------------------------------
// <copyright file="NotificationSenderStub.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using PIS.Ground.Core.Common;
using PIS.Ground.GroundCore.AppGround;

namespace DataPackageTests.Stubs
{
    /// <summary>
    /// Implementation of INotificationSender interface that allow to display notification on the console and
    /// record notification send by datapackage service.
    /// </summary>
    /// <seealso cref="PIS.Ground.Core.Common.INotificationSender" />
    public class NotificationSenderStub : INotificationSender
    {
        #region Fields

        private object _lock = new object();

        private List<NotificationInfo> _notifications = new List<NotificationInfo>(100);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether notifications are output on the console.
        /// </summary>
        /// <value>
        ///   <c>true</c> to output notifications; otherwise, <c>false</c>.
        /// </value>
        bool DisplayNotifications { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSenderStub"/> class.
        /// </summary>
        /// <param name="displayNotifications">Indicates if notification shall be output on console.</param>
        public NotificationSenderStub(bool displayNotifications)
        {
            DisplayNotifications = displayNotifications;
        }
        #endregion

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <returns>The notifications</returns>
        public List<NotificationInfo> GetNotifications()
        {
            lock (_lock)
            {
                return new List<NotificationInfo>(_notifications);
            }
        }

        /// <summary>
        /// Gets the notifications that match the specified request identifier.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <returns>The notifications that match</returns>
        public List<NotificationInfo> GetNotifications(Guid requestId)
        {
            lock (_lock)
            {
                return _notifications.Where(n => n.RequestId == requestId).ToList();
            }
        }

        /// <summary>
        /// Determines whether if the specified notification has been send.
        /// </summary>
        /// <param name="id">The notification identifier.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <returns>
        ///   <c>true</c> if the specified notifications was send; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsNotification(NotificationIdEnum id, Guid requestId)
        {
            lock (_lock)
            {
                return _notifications.Any(n => n.Id == id && n.RequestId == requestId);
            }
        }

        /// <summary>
        /// Determines whether if the specified notification has been send.
        /// </summary>
        /// <param name="id">The notification identifier.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="parameter">The parameter value to search</param>
        /// <returns>
        ///   <c>true</c> if the specified notifications was send; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsNotification(NotificationIdEnum id, Guid requestId, string parameter)
        {
            lock (_lock)
            {
                return _notifications.Any(n => n.Id == id && n.RequestId == requestId && n.Parameter == parameter);
            }
        }

        #region INotificationSender Members

        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="requestId">The request identifier.</param>
        public void SendNotification(NotificationIdEnum notificationId, string parameter, Guid requestId)
        {
            Add(new NotificationInfo(notificationId, requestId, parameter));
        }

        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="parameter">The parametr.</param>
        public void SendNotification(NotificationIdEnum notificationId, string parameter)
        {
            Add(new NotificationInfo(notificationId, Guid.Empty, parameter));
        }

        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="requestId">The request identifier.</param>
        public void SendNotification(NotificationIdEnum notificationId, Guid requestId)
        {
            Add(new NotificationInfo(notificationId, requestId));
        }

        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        public void SendNotification(NotificationIdEnum notificationId)
        {
            Add(new NotificationInfo(notificationId));
        }

        public void SendT2GServerConnectionStatus(bool status)
        {
            if (DisplayNotifications)
            {
                Console.Out.WriteLine("  GROUND NOTIFICATION: T2GServerConnectionStatus({0})", status);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Adds the specified notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        private void Add(NotificationInfo notification)
        {
            lock (_lock)
            {
                _notifications.Add(notification);
            }

            if (DisplayNotifications)
            {
                Console.Out.WriteLine("  GROUND NOTIFICATION: {0}", notification);
            }
        }
        #endregion 
    }
}
