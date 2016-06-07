//---------------------------------------------------------------------------------------------------
// <copyright file="RequestData.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2015.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;

namespace PIS.Ground.Core.Data
{	
	/// <summary>Values that represent RequestState.</summary>
	public enum RequestState
	{
		/// <summary>Request is created, not yet ready to send. This state can be used to clear previous requests in a list or send created notifications.</summary>
		Created = 0,

		/// <summary>Request is ready to send. You use the Process callback to send the request (recommended) or do the processing in an external class (depreciated).</summary>
		ReadyToSend = 1,

		/// <summary>Request already failed at least one time (check AttemptDone to know) and is waiting for a retry.</summary>
		WaitingRetry = 2,

		/// <summary>The process method has be called but is not finished.</summary>
		InProgress = 3,

		/// <summary>A limited number of retry is reached, request will be dropped.</summary>
		AllRetriesExhausted = 4,

		/// <summary>Request is transmitted to the embedded, we are waiting for the response.</summary>
		Transmitted = 5,

		/// <summary>Timeout is reached, request will be dropped.</summary>
		Expired = 6,

		/// <summary>Request is completed.</summary>
		Completed = 7,

		/// <summary>Request terminated with an error.</summary>
		Error = 8
	}

	/// <summary>Request constants.</summary>
	public class RequestConstants
	{
		/// <summary>
		/// Timeout in minutes after which the request, if not completed, is discarded.
		/// </summary>
		public static readonly uint Timeout = 30;

		/// <summary>
		/// Total number of sending attempts to perform after which the request is discarded.
		/// </summary>
		public static readonly uint MaxRetries = 3;

		/// <summary>Time interval in seconds between attempting to retry sending the request.</summary>
		public static readonly uint RetryTimeIntervalSeconds = 10;

		/// <summary>
		/// Prevents a default instance of the RequestConstants class from being created.
		/// </summary>
		private RequestConstants()
		{
		}
	}
	
	/// <summary>Request context.</summary>
	public abstract class RequestContext : PIS.Ground.Core.Data.IRequestContext
	{
		/// <summary>The endpoint.</summary>
		private string _endpoint;

		/// <summary>Identifier for the request.</summary>
		private Guid _requestId;

		/// <summary>Identifier for the element.</summary>
		private string _elementId;

		/// <summary>Identifier for the session.</summary>
		private Guid _sessionId;

		/// <summary>Identifier for the folder.</summary>
		private uint _folderId;

		/// <summary>Date of the expiration.</summary>
		private DateTime _expirationDate;

		/// <summary>The state.</summary>
		private RequestState _state;

		/// <summary>URL of the notification.</summary>
		private string _notificationUrl;

		/// <summary>Date of the next retry.</summary>
		private DateTime _nextRetryDate;

		/// <summary>The request processor.</summary>
		private IRequestContextProcessor _requestProcessor;

		/// <summary>The request timeout.</summary>
		protected uint _requestTimeout;

		/// <summary>The retry time interval in seconds.</summary>
		public uint _retryTimeIntervalSeconds;

		/// <summary>The transfer attempts done.</summary>
		public uint _transferAttemptsDone;

		/// <summary>Initializes a new instance of the RequestContext class.</summary>
		/// <param name="endpoint">The endpoint.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="sessionId">Identifier for the session.</param>
		public RequestContext(string endpoint, string elementId, Guid requestId, Guid sessionId)
			: this(endpoint, elementId, requestId, sessionId, RequestConstants.Timeout)
		{
		}

		/// <summary>Initializes a new instance of the RequestContext class.</summary>
		/// <param name="endpoint">The endpoint.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="timeout">The timeout.</param>
		public RequestContext(string endpoint, string elementId, Guid requestId, Guid sessionId, uint timeout)
		{
			_state = RequestState.Created;
			_endpoint = endpoint;
			_elementId = elementId;
			_requestId = requestId;
			_sessionId = sessionId;
			_expirationDate = DateTime.Now.AddMinutes(timeout);
			_folderId = 0;
			_notificationUrl = string.Empty;
			_requestTimeout = timeout;
			_transferAttemptsDone = 0;

			_retryTimeIntervalSeconds = Math.Min(RequestConstants.RetryTimeIntervalSeconds, timeout * 60);
			
			_nextRetryDate = DateTime.Now.AddSeconds(_retryTimeIntervalSeconds);

			_requestProcessor = null;
		}

		/// <summary>Gets or sets the request timeout.</summary>
		/// <value>The request timeout.</value>
		public uint RequestTimeout
		{
			get
			{
				return _requestTimeout;
			}

			set
			{
				_requestTimeout = value;
				_expirationDate = DateTime.Now.AddMinutes(_requestTimeout);
			}
		}

		/// <summary>Gets the transfer attempts done.</summary>
		/// <value>The transfer attempts done.</value>
		public uint TransferAttemptsDone
		{
			get
			{
				return _transferAttemptsDone;
			}
		}

		/// <summary>Gets or sets the endpoint.</summary>
		/// <value>The endpoint.</value>
		public string Endpoint
		{
			get
			{
				return _endpoint;
			}

			set
			{
				_endpoint = value;
			}
		}

		/// <summary>Gets the identifier of the request.</summary>
		/// <value>The identifier of the request.</value>
		public Guid RequestId
		{
			get { return _requestId; }
		}

		/// <summary>Gets the identifier of the element.</summary>
		/// <value>The identifier of the element.</value>
		public string ElementId
		{
			get { return _elementId; }
		}

		/// <summary>Gets the identifier of the session.</summary>
		/// <value>The identifier of the session.</value>
		public Guid SessionId
		{
			get { return _sessionId; }
		}

		/// <summary>Gets the date of the expiration.</summary>
		/// <value>The date of the expiration.</value>
		public DateTime ExpirationDate
		{
			get { return _expirationDate; }
		}

		/// <summary>
		/// Gets or sets the identifier of the folder. Optional parameter as it is used only for file
		/// transfers through T2G.
		/// </summary>
		/// <value>The identifier of the folder.</value>
		public uint FolderId
		{
			get { return _folderId; }
			set { _folderId = value; }
		}

		/// <summary>Sets a value indicating whether the transmission status.</summary>
		/// <value>True if transmission status, false if not.</value>
		public bool TransmissionStatus
		{
			set
			{
				switch (State)
				{
					case RequestState.ReadyToSend:
					case RequestState.InProgress:
						if (value == true)
						{
							_state = RequestState.Transmitted;
						}
						else
						{
							_state = RequestState.WaitingRetry;
						}
						 
						break;
				}

				if (_transferAttemptsDone != uint.MaxValue)
				{
					_transferAttemptsDone++;
				}
			}
		}

		/// <summary>Sets a value indicating whether the completion status.</summary>
		/// <value>True if completion status, false if not.</value>
		public bool CompletionStatus
		{
			set
			{
				if (value == true)
				{
					_state = RequestState.Completed;
				}
			}
		}

		/// <summary>Sets a value indicating that request terminated with error status or not.</summary>
		/// <value>true if error status, false if not.</value>
		public bool ErrorStatus
		{
			set
			{
				if (value == true)
				{
					_state = RequestState.Error;
				}
			}
		}

		/// <summary>Gets a value indicating whether this object state is final or not.</summary>
		/// <value>true if this object is in a final state, false if not.</value>
		public bool IsStateFinal
		{
			get
			{
				return _state == RequestState.Expired ||
				_state == RequestState.Completed ||
				_state == RequestState.Error ||
				_state == RequestState.AllRetriesExhausted;
			}
		}

		/// <summary>Gets the state.</summary>
		/// <value>The state.</value>
		public RequestState State
		{
			get
			{
				DateTime now = DateTime.Now;

				switch (_state)
				{
					case RequestState.Created:
						_state = RequestState.ReadyToSend;
						break;
					
					case RequestState.ReadyToSend:
						if (now > _expirationDate)
						{
							_state = RequestState.Expired;
						}
						else if (RequestProcessor != null && _state != RequestState.InProgress)
						{
							_state = RequestState.InProgress;
							// TODO: Carl M: Review this design. It's a bad idea to perform processing into a getter.
							RequestProcessor.Process(this);
						}
						else
						{
							// Nothing to do until next trigger.
						}

						break;

					case RequestState.WaitingRetry:
						if (now > _expirationDate)
						{
							_state = RequestState.Expired;
						}
						else if (now > _nextRetryDate)
						{
							_nextRetryDate = now.AddSeconds(_retryTimeIntervalSeconds);
							_state = RequestState.ReadyToSend;
						}

						break;
					
					case RequestState.Transmitted:
						if (now > _expirationDate)
						{
							_state = RequestState.Expired;
						}

						break;
					
					case RequestState.AllRetriesExhausted:
						break;

					case RequestState.Completed:
						break;

					case RequestState.Expired:
						break;

					case RequestState.Error:
						break;
				}

				return _state;
			}
		}

		/// <summary>
		/// Gets or sets URL of the notification. Optional as used only by certain type of requests.
		/// </summary>
		/// <value>The notification URL.</value>
		public string NotificationUrl
		{
			get
			{
				return _notificationUrl;
			}

			set
			{
				_notificationUrl = value;
			}
		}

		/// <summary>Gets or sets the request processor.</summary>
		/// <value>The request processor.</value>
		public IRequestContextProcessor RequestProcessor
		{
			get
			{
				return _requestProcessor;
			}

			set
			{
				if (value != _requestProcessor && value != null)
				{
					_requestProcessor = value;
				}
			}
		}

		/// <summary>Manage communication error with SIF TRAIN.</summary>
		/// <param name="exception">The intercepted exception.</param>
		/// <returns>
		/// True if the caller can continue to communicate with the web service, false otherwise.
		/// </returns>
		public bool OnCommunicationError(System.Exception exception)
		{
			bool canContinue = true;
			EndpointNotFoundException endpointException;
			if (exception is TimeoutException)
			{
				// Ignore this exception so request will continue
			}
			else if ((endpointException = exception as EndpointNotFoundException) != null)
			{
				WebException webException = endpointException.InnerException as WebException;
				if (webException != null && webException.Status == WebExceptionStatus.NameResolutionFailure)
				{
					// Ignore this exception so request will continue
				}
				else if (webException != null && webException.Status == WebExceptionStatus.ConnectFailure)
				{
					// Ignore this exception so request will continue
				}
				else
				{
					// Ignore this exception so request will continue
				}
			}
			else if (exception is ActionNotSupportedException)
			{
				canContinue = false;
			}
			else if (exception is ProtocolException)
			{
				WebException webException = exception.InnerException as WebException;
				if (webException != null && webException.Status == WebExceptionStatus.NameResolutionFailure)
				{
					// Ignore this exception so request will continue
				}
				else
				{
					canContinue = false;
				}
			}
			else
			{
				canContinue = false;
			}

			return canContinue;
		}
	}

	/// <summary>List of requests.</summary>
	public class RequestList : IEnumerable
	{
		/// <summary>The requests.</summary>
		private List<RequestContext> _requests;

		/// <summary>Initializes a new instance of the RequestList class.</summary>
		public RequestList()
		{
			_requests = new List<RequestContext>();
		}

		/// <summary>Returns a list of Requests matching the specified Element ID.</summary>
		/// <param name="elementId">The elementId to look for.</param>
		/// <returns>A list containing all the requests matching the elementId. An empty list is not match found.</returns>
		public List<RequestContext> Find(string elementId)
		{
			List<RequestContext> result = new List<RequestContext>();

			lock (_requests)
			{
				foreach (var request in _requests)
				{
					if (request.ElementId.CompareTo(elementId) == 0)
					{
						result.Add(request);
					}
				}
			}

			return result;
		}

		/// <summary>Returns a list of Requests matching the specified Request ID.</summary>
		/// <param name="requestId">The request id to look for.</param>
		/// <returns>A list containing all the requests matching the requestId. An empty list is not match found.</returns>
		public List<RequestContext> Find(Guid requestId)
		{
			List<RequestContext> result = new List<RequestContext>();

			lock (_requests)
			{
				foreach (var request in _requests)
				{
					if (request.RequestId == requestId)
					{
						result.Add(request);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Returns a list of Requests matching the specified Element ID and Request ID.
		/// </summary>
		/// <param name="elementId">The element id to look for.</param>
		/// <param name="requestId">The request id to look for in the list.</param>
		/// <returns>A list containing all the requests matching both elementId and requestId. An empty list is not match found.</returns>
		public List<RequestContext> Find(string elementId, Guid requestId)
		{
			List<RequestContext> result = new List<RequestContext>();

			lock (_requests)
			{
				foreach (var request in _requests)
				{
					if ((request.ElementId.CompareTo(elementId) == 0) && (request.RequestId == requestId))
					{
						result.Add(request);
					}
				}
			}

			return result;
		}

		/// <summary>Removes a Request object from the list.</summary>
		/// <param name="request">The request to remove from the list.</param>
		public void Remove(RequestContext request)
		{
			lock (_requests)
			{
				_requests.Remove(request);
			}
		}

		/// <summary>Erases all Request objects from the list.</summary>
		public void Clear()
		{
			lock (_requests)
			{
				_requests.Clear();
			}
		}

		/// <summary>Adds a request object to the list.</summary>
		/// <param name="request">The request to add to the list.</param>
		public void Add(RequestContext request)
		{
			lock (_requests)
			{
				_requests.Add(request);
			}
		}

		/// <summary>Gets the enumerator.</summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator GetEnumerator()
		{
			return new RequestEnumerator(_requests);
		}
	}

	/// <summary>Request enumerator.</summary>
	public class RequestEnumerator : IEnumerator
	{
		/// <summary>The requests.</summary>
		private List<RequestContext> _requests;
		
		/// <summary>The position.</summary>
		private int position = -1;

		/// <summary>Initializes a new instance of the RequestEnumerator class.</summary>
		/// <param name="list">The list used to initialize the enumerator.</param>
		public RequestEnumerator(List<RequestContext> list)
		{
			_requests = list;
		}

		/// <summary>Determines if we can move next.</summary>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public bool MoveNext()
		{
			lock (_requests)
			{
				position++;
				return position < _requests.Count;
			}
		}

		/// <summary>Resets this object.</summary>
		public void Reset()
		{
			lock (_requests)
			{
				position = 0;
			}
		}

		/// <summary>Gets the current.</summary>
		/// <value>The current.</value>
		public object Current
		{
			get
			{
				lock (_requests)
				{
					if (_requests.Count > 0)
					{
						try
						{
							return _requests[position];
						}
						catch (IndexOutOfRangeException)
						{
							return null;
						}
						catch (ArgumentOutOfRangeException)
						{
							return null;
						}
					}
					else
					{
						return null;
					}
				}
			}
		}
	}
}