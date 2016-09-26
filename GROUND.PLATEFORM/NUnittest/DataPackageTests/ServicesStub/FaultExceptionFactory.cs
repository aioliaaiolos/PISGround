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
        public const string CodeInvalidServiceIdentifier = "F0204";
        public const string CodeInvalidPathList = "F0301";
        public const string CodeInvalidFolderId = "F0304";
        public const string CodeBadSystemId = "F0107";
        public const string CodeBadTaskId = "F0308";
        public const string CodeCannotDelete = "F010B";
        public const string CodeCannotStart = "F0109";
        public const string MessageInvalidSession = "The provided session Id is either invalid or has expired. (F0103)";
        public const string MessageNotImplementedException = "The invoked service method is not implemented. (F0100)";
        public const string MessageNoNotificationUrl = "No notification URL is associated to the session. (F020A)";
        public const string MessageOnlySubscriptionToAllSystemIsSupported = "Only subscription to all system is supported (F0100)";
        public const string MessageInvalidSubscriptionCount = "Only one subscription is allowed at time (F0100)";
        public const string MessageOnlyOnChangeNotificationSupported = "Only onChange notification is supported (F0100)";
        public const string MessageInvalidMessageIdentifier = "The provided Message identifier is invalid (F0208)";
        public const string MessageInvalidServiceIdentifier = "Unknown service identifier (F0204)";
        public const string MessageCompressionNotSupported = "File compression is not supported. (F0100)";
        public const string MessageInvalidPathList = "Invalid path specified. (F0301)";
        public const string MessageInvalidFolderId = "The provided Folder identifier is invalid. (F0304)";
        public const string MessageSupportTransferToOneRecipientOnly = "Only transfer to one recipient is supported. (F0100)";
        public const string MessageSupportOnlyGroundToTrainTransfer = "Only ground to train transfer are supported. (F0100)";
        public const string MessageBadSystemId = "The provided System identifier is invalid. (F0107)";
        public const string MessageBadTaskId = "The provided Task identifier is invalid. (F0308)";
        public const string MessageCannotDelete = "Cannot delete an active transfer's task (in state \"taskCreated\", \"taskStarted\" or \"taskStopped\". (F010B)";
        public const string MessageCannotStart = "Cannot start the specified Transfer Task (the task state must be \"taskCreated\"). (F0109)";

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

        /// <summary>
        /// Creates the invalid service identifier fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateInvalidServiceIdentifierFault()
        {
            return new FaultException(new FaultReason(MessageInvalidServiceIdentifier), new FaultCode(CodeInvalidServiceIdentifier, CodeNS));
        }
        
        /// <summary>
        /// Creates the compression not supported fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateCompressionNotSupportedFault()
        {
            return new FaultException(new FaultReason(MessageCompressionNotSupported), new FaultCode(CodeUnexpectedError, CodeNS));
        }
        
        /// <summary>
        /// Creates the invalid path list fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateInvalidPathListFault()
        {
            return new FaultException(new FaultReason(MessageInvalidPathList), new FaultCode(CodeInvalidPathList, CodeNS));
        }

        /// <summary>
        /// Creates the invalid folder identifier fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateInvalidFolderIdFault()
        {
            return new FaultException(new FaultReason(MessageInvalidFolderId), new FaultCode(CodeInvalidFolderId, CodeNS));
        }

        /// <summary>
        /// Creates the support transfer to one recipient only fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateSupportTransferToOneRecipientOnlyFault()
        {
            return new FaultException(new FaultReason(MessageSupportTransferToOneRecipientOnly), new FaultCode(CodeUnexpectedError, CodeNS));
        }

        /// <summary>
        /// Creates the support only ground to train transfer fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateSupportOnlyGroundToTrainTransferFault()
        {
            return new FaultException(new FaultReason(MessageSupportOnlyGroundToTrainTransfer), new FaultCode(CodeUnexpectedError, CodeNS));
        }

        /// <summary>
        /// Creates the invalid system identifier fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateInvalidSystemIdentifierFault()
        {
            return new FaultException(new FaultReason(MessageBadSystemId), new FaultCode(CodeBadSystemId, CodeNS));
        }

        /// <summary>
        /// Creates the invalid task identifier fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateInvalidTaskIdentifierFault()
        {
            return new FaultException(new FaultReason(MessageBadTaskId), new FaultCode(CodeBadTaskId, CodeNS));
        }

        /// <summary>
        /// Creates the cannot delete active transfer fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateCannotDeleteActiveTransferFault()
        {
            return new FaultException(new FaultReason(MessageCannotDelete), new FaultCode(CodeCannotDelete, CodeNS));
        }

        /// <summary>
        /// Creates the cannot start transfer fault.
        /// </summary>
        /// <returns>The fault object created.</returns>
        public static FaultException CreateCannotStartTransferFault()
        {
            return new FaultException(new FaultReason(MessageCannotStart), new FaultCode(CodeCannotStart, CodeNS));
        }
    }
}
