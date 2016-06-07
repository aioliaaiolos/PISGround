//---------------------------------------------------------------------------------------------------
// <copyright file="ILiveVideoControlService.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2013.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using PIS.Ground.Core.Data;

namespace PIS.Ground.LiveVideoControl
{
	/// <summary>Interface for live video control service.</summary>
	[ServiceContract(Namespace = "http://alstom.com/pacis/pis/ground/livevideocontrol/", Name = "LiveVideoControl")]
	public interface ILiveVideoControlService
	{
		/// <summary>Gets available element list.</summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <returns>The available element list.</returns>
		[OperationContract]
		LiveVideoControlElementListResult GetAvailableElementList(Guid sessionId);

		/// <summary>Command to starts video streaming.</summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="targetAddress">Target element address.</param>
		/// <param name="url">URL of the stream to play onboard.</param>
		/// <returns>Return statement of the request (Success, failed,...) with a request id.</returns>
		[OperationContract]
		LiveVideoControlResult StartVideoStreamingCommand(Guid sessionId, TargetAddressType targetAddress, string url);

		/// <summary>Command to stops video streaming.</summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="targetAddress">Target element address.</param>
		/// <returns>Return statement of the request (Success, failed,...) with a request id.</returns>
		[OperationContract]
		LiveVideoControlResult StopVideoStreamingCommand(Guid sessionId, TargetAddressType targetAddress);

		/// <summary>Command to select the automatic or manual starting mode for the video streaming.
		///          If the specified URL is empty or null, manual mode is selected. 
		///          Otherwise, automatic mode is selected</summary>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="url">URL of the stream to be used in automatic mode.</param>
		/// <returns>Return statement of the request (Success, failed,...) with URL and request id.</returns>
		[OperationContract]
		LiveVideoControlResult ChangeCommandMode(Guid sessionId, string url);

        /// <summary>Command to determine the current command mode.</summary>
        /// <param name="sessionId">Identifier for the session.</param>
		/// <returns>Return statement of the request (Success, failed,...) with URL and request id.</returns>
        [OperationContract]
		LiveVideoControlResult GetCommandMode(Guid sessionId);
	}

	/// <summary>Live video control result.</summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/livevideocontrol/", Name = "LiveVideoControlResult")]
	public class LiveVideoControlResult
	{
		/// <summary>Gets or sets the result code.</summary>
		/// <value>The result code.</value>
		[DataMember(IsRequired = true)]
		public LiveVideoControlErrorEnum ResultCode { get; set; }

		/// <summary>Gets or sets the Identifier for the request.</summary>
		/// <value>The identifier of the request.</value>
		[DataMember(IsRequired = true)]
		public Guid RequestId { get; set; }

		/// <summary>Gets or sets the streaming URL used in automatic mode.</summary>
		/// <value>URL if in automatic mode, empty or null otherwise.</value>
		[DataMember(IsRequired = true)]
		public string Url { get; set; }
	}

	/// <summary>Live video control element list result.</summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/livevideocontrol/", Name = "LiveVideoControlElementListResult")]
	public class LiveVideoControlElementListResult
	{
		/// <summary>Gets or sets the result code.</summary>
		/// <value>The result code.</value>
		[DataMember(IsRequired = true)]
		public LiveVideoControlErrorEnum ResultCode { get; set; }

		/// <summary>Gets or sets the list of elements.</summary>
		/// <value>A List of elements.</value>
		[DataMember(IsRequired = true)]
		public ElementList<AvailableElementData> ElementList { get; set; }
	}

	/// <summary>Values that represent LiveVideoControlErrorEnum.</summary>
	[DataContract(Namespace = "http://alstom.com/pacis/pis/ground/livevideocontrol/", Name = "LiveVideoControlErrorEnum")]
	public enum LiveVideoControlErrorEnum
	{
		/// <summary>An enum constant representing the request accepted option.</summary>
		[EnumMember(Value = "REQUEST_ACCEPTED")]
		RequestAccepted = 0,

		/// <summary>An enum constant representing the invalid session identifier option.</summary>
		[EnumMember(Value = "INVALID_SESSION_ID")]
		InvalidSessionId = 1,

		/// <summary>An enum constant representing the unknown element identifier option.</summary>
		[EnumMember(Value = "UNKNOWN_ELEMENT_ID")]
		UnknownElementId = 2,

		/// <summary>An enum constant representing the unknown mission identifier option.</summary>
		[EnumMember(Value = "UNKNOWN_MISSION_ID")]
		UnknownMissionId = 3,

		/// <summary>An enum constant representing the element list not available option.</summary>
		[EnumMember(Value = "ELEMENT_LIST_NOT_AVAILABLE")]
		ElementListNotAvailable = 4,
		
		/// <summary>An enum constant representing the T2G server offline option.</summary>
		[EnumMember(Value = "T2G_SERVER_OFFLINE")]
		T2GServerOffline = 5,

		/// <summary>An enum constant representing the invalid request identifier option.</summary>
		[EnumMember(Value = "INVALID_REQUEST_ID")]
		InvalidRequestID = 6,

		/// <summary>An enum constant representing the internal error option.</summary>
		[EnumMember(Value = "INTERNAL_ERROR")]
		InternalError = 7,

		/// <summary>An enum constant representing the command is not available in automatic mode.</summary>
		[EnumMember(Value = "AUTOMATIC_MODE_ACTIVATED")]
		AutomaticModeActivated = 8
	}
}