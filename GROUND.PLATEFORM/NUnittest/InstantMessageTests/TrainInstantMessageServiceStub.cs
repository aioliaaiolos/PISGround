//---------------------------------------------------------------------------------------------------
// <copyright file="TrainInstantMessageServiceStub.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2015.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.ServiceModel;
using PIS.Train.InstantMessage;

namespace PIS.Ground.InstantMessageTests
{
	/// <summary>Implementation of instant message service of embedded system designed for testing purpose.</summary>
	[ServiceBehaviorAttribute(InstanceContextMode = InstanceContextMode.Single, ConfigurationName = "PIS.Train.InstantMessage.IInstantMessageTrainService")]
	class TrainInstantMessageServiceStub : IInstantMessageTrainService
	{
		public TrainInstantMessageServiceStub()
		{
			SendMessageReturnValue = ErrorType.Succeed;
			CancelMessageReturnValue = ErrorType.Succeed;
			CancelAllMessagesReturnValue = ErrorType.Succeed;
		}

		public ErrorType SendMessageReturnValue { get; set; }
		public ErrorType CancelMessageReturnValue { get; set; }
		public ErrorType CancelAllMessagesReturnValue { get; set; }
		public int CancelMessageCallCount { get; set; }
		public int CancelAllMessagesCallCount { get; set; }
		public int SendFreeTextMessageCallCount { get; set; }
		public int SendScheduledMessageCallCount { get; set; }
		public int SendPredefinedMesssageCallCount { get; set; }

		#region IInstantMessageTrainService Members

		public RetrieveMessageTemplateListResponse RetrieveMessageTemplateList(RetrieveMessageTemplateListRequest request)
		{
			throw new NotImplementedException();
		}

		public SendPredefinedMessagesResponse SendPredefinedMessages(SendPredefinedMessagesRequest request)
		{
			SendPredefinedMesssageCallCount = SendPredefinedMesssageCallCount + 1;
			SendPredefinedMessagesResponse response = new SendPredefinedMessagesResponse();
			response.Error = SendMessageReturnValue;
			return response;
		}

		public SendFreeTextMessageResponse SendFreeTextMessage(SendFreeTextMessageRequest request)
		{
			SendFreeTextMessageCallCount = SendFreeTextMessageCallCount + 1;
			SendFreeTextMessageResponse response = new SendFreeTextMessageResponse();
			response.Error = SendMessageReturnValue;
			return response;
		}

		public SendScheduledMessageResponse SendScheduledMessage(SendScheduledMessageRequest request)
		{
			SendScheduledMessageCallCount = SendScheduledMessageCallCount + 1;
			SendScheduledMessageResponse response = new SendScheduledMessageResponse();
			response.Error = SendMessageReturnValue;
			return response;
		}

		public CancelAllMessageResponse CancelAllMessage(CancelAllMessageRequest request)
		{
			CancelAllMessagesCallCount = CancelAllMessagesCallCount + 1;
			CancelAllMessageResponse response = new CancelAllMessageResponse();
			response.Error = CancelAllMessagesReturnValue;
			return response;
		}

		public CancelScheduledMessageResponse CancelScheduledMessage(CancelScheduledMessageRequest request)
		{
			CancelMessageCallCount = CancelMessageCallCount + 1;
			CancelScheduledMessageResponse response = new CancelScheduledMessageResponse();
			response.Error = CancelMessageReturnValue;
			return response;
		}

		public RetrieveStationListResponse RetrieveStationList(RetrieveStationListRequest request)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
