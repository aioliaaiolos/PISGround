//---------------------------------------------------------------------------------------------------
// <copyright file="InstantMessageServiceStub.cs" company="Alstom">
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
using PIS.Ground.InstantMessage;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.Common;
using PIS.Ground.Core.T2G;
using PIS.Ground.Core.LogMgmt;

namespace PIS.Ground.InstantMessageTests
{
	internal class InstantMessageServiceStub : InstantMessageService
	{
		private bool _blockAddNewRequest;
		private List<InstantMessageService.InstantMessageRequestContext> _blockedNewRequest = new List<InstantMessageService.InstantMessageRequestContext>();


		internal bool AddNewRequestToRequestListBlocked
		{
			get
			{
				return _blockAddNewRequest;
			}

			set
			{
				if (value != _blockAddNewRequest)
				{
					_blockAddNewRequest = value;
					if (!value && _blockedNewRequest.Count != 0)
					{
						base.AddNewRequestToRequestList(_blockedNewRequest);
						_blockedNewRequest.Clear();
					}
				}

			}
		}
		internal InstantMessageService.InstantMessageRequestContext LastAddedRequest { get; private set; }
		/// <summary>Initializes a new instance of the InstantMessageService class to be used for unitests purpose.</summary>
		/// <param name="sessionManager">Manager for session.</param>
		/// <param name="notificationSender">The notification sender.</param>
		/// <param name="train2groungManager">Manager for 2g.</param>
		/// <param name="logManager">Manager for log.</param>
		internal InstantMessageServiceStub(
			ISessionManager sessionManager,
			INotificationSender notificationSender,
			IT2GManager train2groungManager,
			ILogManager logManager) :
			base(sessionManager, notificationSender, train2groungManager, logManager)
		{
			// No logic body
		}

		/// <summary>Query if 'elementId' is element known by datastore.</summary>
		/// <param name="elementId">Identifier for the element.</param>
		/// <returns>true if element known by datastore, false if not.</returns>
		protected override bool IsElementKnownByDatastore(string elementId)
		{
			return true;
		}

		/// <summary>Query if 'packageType' is data package known by datastore.</summary>
		/// <param name="packageType">Type of the package.</param>
		/// <param name="packageVersion">The package version.</param>
		/// <returns>true if data package known by datastore, false if not.</returns>
		protected override bool IsDataPackageKnownByDatastore(string packageType, string packageVersion)
		{
			return true;
		}

		/// <summary>Validate template and message identifier.</summary>
		/// <param name="category">The category of the message.</param>
		/// <param name="element">The element.</param>
		/// <param name="messageIdentifier">Identifier for the message.</param>
		/// <returns>
		/// null if template and message identifier are valid, otherwise return the error code.
		/// </returns>
		protected override PIS.Ground.GroundCore.AppGround.NotificationIdEnum? ValidateMessageIdentifier(TemplateCategory category, PIS.Ground.Core.Data.AvailableElementData element, string messageIdentifier)
		{
			return null;
		}

		/// <summary>Validate predefined messages template with baseline information.</summary>
		/// <param name="element">The element.</param>
		/// <param name="request">Predefined messages request to validate.</param>
		/// <returns>
		/// null if all predefined message identifiers are valid and the provided parameters, otherwise
		/// return an error code.
		/// </returns>
		protected override PIS.Ground.GroundCore.AppGround.NotificationIdEnum? ValidatePredefinedMessageTemplate(PIS.Ground.Core.Data.AvailableElementData element, InstantMessageService.ProcessSendPredefMessagesRequestContext request)
		{
			return null;
		}

		/// <summary>Adds a new request to request list.</summary>
		/// <param name="newRequests">The new requests.</param>
		protected override void AddNewRequestToRequestList(List<InstantMessageService.InstantMessageRequestContext> newRequests)
		{
			if (newRequests != null && newRequests.Count > 0)
			{
				LastAddedRequest = newRequests[newRequests.Count - 1];
			}
			else
			{
				LastAddedRequest = null;
			}

			if (!AddNewRequestToRequestListBlocked)
			{
				base.AddNewRequestToRequestList(newRequests);
			}
			else
			{
				_blockedNewRequest.AddRange(newRequests);
			}
		}
	}
}
