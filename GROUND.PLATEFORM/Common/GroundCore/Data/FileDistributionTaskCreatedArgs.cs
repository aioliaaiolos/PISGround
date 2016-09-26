//---------------------------------------------------------------------------------------------------
// <copyright file="FileDistributionTaskCreatedArgs.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIS.Ground.Core.Data
{
    /// <summary>
    /// Define parameters of event that notify about transfer task created
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class FileDistributionTaskCreatedArgs : EventArgs
    {        
        /// <summary>
        /// Gets the created transfer task identifier.
        /// </summary>
        public int TaskId { get; private set; }
        
        /// <summary>
        /// Gets the distribution request identifier.
        /// </summary>
        public Guid RequestId { get; private set; }

        /// <summary>
        /// Gets the recipients.
        /// </summary>
        public List<RecipientId> Recipients { get; private set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDistributionTaskCreatedArgs"/> class.
        /// </summary>
        /// <param name="taskId">The task identifier created.</param>
        /// <param name="requestId">The distribution request identifier.</param>
        /// <param name="recipients">The recipients.</param>
        public FileDistributionTaskCreatedArgs(int taskId, Guid requestId, List<RecipientId> recipients)
        {
            TaskId = taskId;
            RequestId = requestId;
            Recipients = recipients;
        }
    }
}
