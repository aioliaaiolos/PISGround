// <copyright file="HistoryLogResponse.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2011.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
namespace PIS.Ground.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Class representing the output response of baseline status
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/core/data/", Name = "StatusOutput")]
    public class BaselineStatusResponse : MaintenanceResponse
    {
        /// <summary>
        /// Status response in xml format.
        /// </summary>
        [DataMember]
        public string StatusResponse;
    }

    /// <summary>
    /// Class representing the output response of history log
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/core/data/", Name = "HistoryLogOutput")]
    public class HistoryLogResponse : MaintenanceResponse
    {
        /// <summary>
        /// Log response in xml format.
        /// </summary>
        [DataMember]
        public string LogResponse;
    }

    /// <summary>
    /// Class representing the Maintenance output response
    /// </summary>
    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/core/data/", Name = "MaintenanceOutput")]
    public class MaintenanceResponse
    {
        /// <summary>
        /// variable request id
        /// </summary>
        [DataMember]
        public Guid RequestId;

        /// <summary>
        /// variable result code
        /// </summary>
        [DataMember]
        public ResultCodeEnum ResultCode;
    }
}
