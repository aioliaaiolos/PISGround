//---------------------------------------------------------------------------------------------------
// <copyright file="IRequestContext.cs" company="Alstom">
//          (c) Copyright ALSTOM 2015.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;

namespace PIS.Ground.Core.Data
{
	/// <summary>Common interface describing minimum requirement for all requests contexts.</summary>
	public interface IRequestContext
	{
		/// <summary>Gets or sets the request processor. Can be null for retrocompatibility, should be defined to a processor in new devloppements.</summary>
		/// <value>The request processor.</value>
		IRequestContextProcessor RequestProcessor { get; set; }

		/// <summary>Sets a value indicating whether the request was completed or not (request sent and response received from the embedded).</summary>
		/// <value>True if request is completed, false if not.</value>
		bool CompletionStatus { set; }

		/// <summary>Gets the identifier of the element.</summary>
		/// <value>The identifier of the element.</value>
		string ElementId { get; }

		/// <summary>Gets or sets the endpoint.</summary>
		/// <value>The endpoint.</value>
		string Endpoint { get; set; }

		/// <summary>Gets the date of the expiration.</summary>
		/// <value>The date of the expiration.</value>
		DateTime ExpirationDate { get; }

		/// <summary>Gets or sets the identifier of the folder. Optional parameter as it is used only for file transfers through T2G.</summary>
		/// <value>The identifier of the folder.</value>
		uint FolderId { get; set; }

		/// <summary>Gets or sets URL of the notification. Optional as used only by certain type of requests.</summary>
		/// <value>The notification URL.</value>
		string NotificationUrl { get; set; }

		/// <summary>Gets the identifier of the request.</summary>
		/// <value>The identifier of the request.</value>
		Guid RequestId { get; }

		/// <summary>Gets or sets the request timeout.</summary>
		/// <value>The request timeout.</value>
		uint RequestTimeout { get; set; }

		/// <summary>Gets the identifier of the session.</summary>
		/// <value>The identifier of the session.</value>
		Guid SessionId { get; }

		/// <summary>Gets the state of the request.</summary>
		/// <value>The state's state.</value>
		RequestState State { get; }

		/// <summary>Gets a value indicating whether this object state is final or not.</summary>
		/// <value>true if this object is in a final state, false if not.</value>
		bool IsStateFinal { get; }

		/// <summary>Gets the number of transfer attempts done.</summary>
		/// <value>The number of transfer attempts done.</value>
		uint TransferAttemptsDone { get; }

		/// <summary>Sets a value indicating whether the request was transmitted to the embedded or not.</summary>
		/// <value>True if transmission is done, false if not.</value>
		bool TransmissionStatus { set; }
	}
}
