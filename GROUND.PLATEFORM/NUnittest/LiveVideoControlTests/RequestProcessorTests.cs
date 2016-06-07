//---------------------------------------------------------------------------------------------------
// <copyright file="RequestProcessorTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2013.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.LiveVideoControl;

namespace LiveVideoControlTests
{
	/// <summary>Request processor tests.</summary>
	[TestFixture]
	public class RequestProcessorTests
	{
		#region attributes

		/// <summary>Identifier for the application.</summary>
		private static string _applicationId = "ApplicationId";

		/// <summary>The train 2ground client mock.</summary>
        private Mock<IT2GManager> _train2groundManagerMock;

		/// <summary>The session manager mock.</summary>
		private Mock<ISessionManager> _sessionManagerMock;

		/// <summary>The request processor.</summary>
		private IRequestProcessor _requestProcessor;

		/// <summary>Maximum time to wait for transmit event (in ms).</summary>
		private const int _transmitEventWaitingTime = 30000;

		#endregion

		#region Tests managment

		/// <summary>Initializes a new instance of the RequestProcessorTests class.</summary>
		public RequestProcessorTests()
		{
			// Nothing special
		}

		/// <summary>Setups this object.</summary>
		[SetUp]
		public void Setup()
		{
            this._train2groundManagerMock = new Mock<IT2GManager>();
			this._sessionManagerMock = new Mock<ISessionManager>();
			this._requestProcessor = new RequestProcessor();
			PIS.Ground.LiveVideoControl.RequestProcessor._transmitEvent.Reset();
		}

		/// <summary>Tear down.</summary>
		[TearDown]
		public void TearDown()
		{
			// Do something after each tests
		}

		#endregion

		#region SetSessionMgr

		/// <summary>Sets session manager null.</summary>
		[Test]
		[ExpectedException(typeof(NullReferenceException))]
		public void SetSessionMgrNull()
		{
			this._requestProcessor.SetSessionMgr(null);
		}

		/// <summary>Sets session manager valid.</summary>
		[Test]
		public void SetSessionMgrValid()
		{
			this._requestProcessor.SetSessionMgr(this._sessionManagerMock.Object);
		}

		#endregion

		#region SetT2GManager

		/// <summary>Sets train to ground client null.</summary>
		[Test]
		[ExpectedException(typeof(NullReferenceException))]
		public void SetT2GClientNull()
		{
			this._requestProcessor.SetT2GManager(null);
		}		

		/// <summary>Sets train to ground client valid.</summary>
		[Test]
		public void SetT2GClientValid()
		{
            this._train2groundManagerMock.Setup(x => x.SubscribeToElementChangeNotification(It.IsAny<string>(), It.IsAny<EventHandler<ElementEventArgs>>()));
			this._requestProcessor.SetT2GManager(this._train2groundManagerMock.Object, RequestProcessorTests._applicationId);
		}

		#endregion

		#region AddRequest

		/// <summary>Adds request null.</summary>
		[Test]
		[ExpectedException(typeof(NullReferenceException))]
		public void AddRequestNull()
		{
			this._requestProcessor.AddRequest(null);
		}

		/// <summary>Adds request valid.</summary>
		[Test]
		public void AddRequestValid()
		{
			string testString = string.Empty;
			bool testBool = false;

			this._sessionManagerMock.Setup(x => x.GetNotificationUrlByRequestId(Guid.Empty, out testString)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.IsElementOnline(string.Empty, out testBool)).Returns(T2GManagerErrorEnum.eElementNotFound);
			this._requestProcessor.SetSessionMgr(this._sessionManagerMock.Object);
			this._requestProcessor.SetT2GManager(this._train2groundManagerMock.Object, RequestProcessorTests._applicationId);

			RequestContext requestContext = new ProcessStopVideoStreamingCommandRequestContext(string.Empty, Guid.Empty, Guid.Empty);

			new Timer(
						x => { this._requestProcessor.AddRequest(requestContext); },
						null,
						0,
						500);

			Assert.IsTrue(PIS.Ground.LiveVideoControl.RequestProcessor._transmitEvent.WaitOne(_transmitEventWaitingTime));
		}

		#endregion

		#region AddRequestRange

		/// <summary>Adds request range null.</summary>
		[Test]
		[ExpectedException(typeof(NullReferenceException))]
		public void AddRequestRangeNull()
		{
			this._requestProcessor.AddRequestRange(null);
		}

		/// <summary>Adds request range empty.</summary>
		[Test]
		public void AddRequestRangeEmpty()
		{
			string testString = string.Empty;

			List<RequestContext> requestsContexts = new List<RequestContext>();

			new Timer(
						x => { this._requestProcessor.AddRequestRange(requestsContexts); },
						null,
						0,
						500);

			Assert.IsFalse(PIS.Ground.LiveVideoControl.RequestProcessor._transmitEvent.WaitOne(_transmitEventWaitingTime));
		}

		/// <summary>Adds request range valid.</summary>
		[Test]
		public void AddRequestRangeValid()
		{
			string testString = string.Empty;
			bool testBool = false;

			this._sessionManagerMock.Setup(x => x.GetNotificationUrlByRequestId(Guid.Empty, out testString)).Returns(string.Empty);
			this._train2groundManagerMock.Setup(x => x.IsElementOnline(string.Empty, out testBool)).Returns(T2GManagerErrorEnum.eElementNotFound);
			this._requestProcessor.SetSessionMgr(this._sessionManagerMock.Object);
			this._requestProcessor.SetT2GManager(this._train2groundManagerMock.Object, RequestProcessorTests._applicationId);

			RequestContext stopRequestContext = new ProcessStopVideoStreamingCommandRequestContext(string.Empty, Guid.Empty, Guid.Empty);
			RequestContext startRequestContext = new ProcessStartVideoStreamingCommandRequestContext(string.Empty, Guid.Empty, Guid.Empty, string.Empty);
			List<RequestContext> requestsContexts = new List<RequestContext>();
			requestsContexts.Add(startRequestContext);
			requestsContexts.Add(stopRequestContext);

			new Timer(
						x => { this._requestProcessor.AddRequestRange(requestsContexts); },
						null,
						0,
						1000);

			Assert.IsTrue(PIS.Ground.LiveVideoControl.RequestProcessor._transmitEvent.WaitOne(_transmitEventWaitingTime));
		}

		#endregion
	}
}