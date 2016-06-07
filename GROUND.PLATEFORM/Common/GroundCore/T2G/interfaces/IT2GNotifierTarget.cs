//---------------------------------------------------------------------------------------------------
// <copyright file="IT2GNotifierTarget.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.T2G
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using PIS.Ground.Core.Data;

    /// <summary>Interface for consuming T2GNotificationProcessor events.</summary>
    public interface IT2GNotifierTarget
    {
        /// <summary>Raises the on file distribute notification event.</summary>
        /// <param name="args">Event arguments.</param>
        void RaiseOnFileDistributeNotificationEvent(FileDistributionStatusArgs args);

        /// <summary>Raises the on file distribute notification event.</summary>
        /// <param name="args">Event arguments.</param>
        /// <param name="taskId">Identifier for the task.</param>
        void RaiseOnFileDistributeNotificationEvent(FileDistributionStatusArgs args, int taskId);

        /// <summary>Raises the on file distribute notification event.</summary>
        /// <param name="args">Event arguments.</param>
        /// <param name="RequestId">Identifier for the request.</param>
        void RaiseOnFileDistributeNotificationEvent(FileDistributionStatusArgs args, Guid RequestId);

        /// <summary>Raises the on file publication notification event.</summary>
        /// <param name="args">Event arguments.</param>
        void RaiseOnFilePublicationNotificationEvent(FilePublicationNotificationArgs args);

        /// <summary>Raises the on file published notification event.</summary>
        /// <param name="args">Event arguments.</param>
        void RaiseOnFilePublishedNotificationEvent(FilePublishedNotificationArgs args);

        /// <summary>Raises the on element information change event.</summary>
        /// <param name="args">Event arguments.</param>
        void RaiseOnElementInfoChangeEvent(ElementEventArgs args);

        /// <summary>Raises the on file received notification event.</summary>
        /// <param name="args">Event arguments.</param>
        void RaiseOnFileReceivedNotificationEvent(FileReceivedArgs args);        

        /// <summary>Raises the on system deleted notification event.</summary>
        /// <param name="args">Event arguments.</param>
        void RaiseOnSystemDeletedNotificationEvent(SystemDeletedNotificationArgs args);
    }
}
