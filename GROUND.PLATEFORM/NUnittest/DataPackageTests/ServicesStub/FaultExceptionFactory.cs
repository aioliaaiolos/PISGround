//---------------------------------------------------------------------------------------------------
// <copyright file="FaultExceptionFactory.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace DataPackageTests.ServicesStub
{
    /// <summary>
    /// Factory of fault exception for T2G Service
    /// </summary>
    internal static class FaultExceptionFactory
    {
        #region Constants
        public const string CodeNS = "http://transport.alstom.com/webservices/faults";

        public const string CodeInvalidSession = "F0103";
        public const string CodeNoNotificationUrl = "F020A";
        public const string CodeUnexpectedError = "F0100";
        public const string CodeInvalidMessageIdentifier = "F0208";
        public const string MessageInvalidSession = "The provided session Id is either invalid or has expired. (F0103)";
        public const string MessageNotImplementedException = "The invoked service method is not implemented. (F0100)";
        public const string MessageNoNotificationUrl = "No notification URL is associated to the session. (F020A)";
        public const string MessageOnlySubscriptionToAllSystemIsSupported = "Only subscription to all system is supported (F0100)";
        public const string MessageInvalidSubscriptionCount = "Only one subscription is allowed at time (F0100)";
        public const string MessageOnlyOnChangeNotificationSupported = "Only onChange notification is supported (F0100)";
        public const string MessageInvalidMessageIdentifier = "The provided Message identifier is invalid (F0208)";

        #endregion

        /// <summary>
        /// Creates the invalid session identifier fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateInvalidSessionIdentifierFault()
        {
            return new FaultException(new FaultReason(MessageInvalidSession), new FaultCode(CodeInvalidSession, CodeNS));
        }

        /// <summary>
        /// Creates the not implemented fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateNotImplementedFault()
        {
            return new FaultException(new FaultReason(MessageNotImplementedException), new FaultCode(CodeUnexpectedError, CodeNS));
        }

        /// <summary>
        /// Creates the no notification URL fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateNoNotificationUrlFault()
        {
            return new FaultException(new FaultReason(MessageNoNotificationUrl), new FaultCode(CodeNoNotificationUrl, CodeNS));
        }

        /// <summary>
        /// Creates the only subscription to all system is supported fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateOnlySubscriptionToAllSystemIsSupportedFault()
        {
            return new FaultException(new FaultReason(MessageOnlySubscriptionToAllSystemIsSupported), new FaultCode(CodeUnexpectedError, CodeNS));
        }

        /// <summary>
        /// Creates the invalid subscription count fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateInvalidSubscriptionCountFault()
        {
            return new FaultException(new FaultReason(MessageInvalidSubscriptionCount), new FaultCode(CodeUnexpectedError, CodeNS));
        }

        /// <summary>
        /// Creates the only on change notification supported fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateOnlyOnChangeNotificationSupportedFault()
        {
            return new FaultException(new FaultReason(MessageOnlyOnChangeNotificationSupported), new FaultCode(CodeUnexpectedError, CodeNS));
        }

        /// <summary>
        /// Creates the invalid message identifier fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateInvalidMessageIdentifierFault()
        {
            return new FaultException(new FaultReason(MessageInvalidMessageIdentifier), new FaultCode(CodeInvalidMessageIdentifier, CodeNS));
        }
    }
}
