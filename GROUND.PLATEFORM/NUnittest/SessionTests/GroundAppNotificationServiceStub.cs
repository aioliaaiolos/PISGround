//---------------------------------------------------------------------------------------------------
// <copyright file="GroundAppNotificationServiceStub.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using PIS.Ground.GroundCore.AppGround;

namespace SessionTests
{
    /// <summary>
    /// Implementation of PIS-Ground notification service
    /// </summary>
    /// <seealso cref="PIS.Ground.GroundCore.AppGround.INotificationAppGroundService" />
    [ServiceBehavior(Name = "GroundAppNotificationServiceStub", InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, Namespace = "http://alstom.com/pacis/pis/appground/")]
    class GroundAppNotificationServiceStub : INotificationAppGroundService
    {
        #region INotificationAppGroundService Members

        /// <summary>
        /// Method called when PIS-Ground send a notification
        /// </summary>
        /// <param name="request">The notification request data.</param>
        /// <returns></returns>
        public SendNotificationResponse SendNotification(SendNotificationRequest request)
        {
            // No logic body
            return new SendNotificationResponse();
        }

        #endregion
    }
}
