//---------------------------------------------------------------------------------------------------
// <copyright file="FtpState.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2013.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Net;
using System.Threading;

namespace PIS.Ground.Core.Data
{
	/// <summary>FTP state.</summary>
	internal class FtpState : IDisposable
	{
		#region private fields

		/// <summary>The wait event.</summary>
		private ManualResetEvent _wait;

		/// <summary>The ftp request.</summary>
		private FtpWebRequest _request;

		/// <summary>The remote target file.</summary>
		private IRemoteFileClass _remoteFile;

		/// <summary>The ftp response.</summary>
		private FtpWebResponse _response;

		/// <summary>The FTP status.</summary>
		private FtpStatus _ftpStatus;

		/// <summary>True if disposed.</summary>
		private bool _disposed = false;

		/// <summary>Name of the service point group.</summary>
		private string _servicePointGroupName = Guid.NewGuid().ToString();

		/// <summary>The service point.</summary>
		private ServicePoint _servicePoint;

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the FtpState class.</summary>
		public FtpState()
		{
			this._wait = new ManualResetEvent(false);
			this._ftpStatus = new FtpStatus();
		}

		/// <summary>Finalizes an instance of the FtpState class.</summary>
		~FtpState()
		{
			// Do not re-create Dispose clean-up code here.
			// Calling Dispose(false) is optimal in terms of
			// readability and maintainability.
			this.Dispose(false);
		}

		#endregion

		#region methods

		/// <summary>Gets the operation complete.</summary>
		/// <value>The operation complete.</value>
		public ManualResetEvent OperationComplete
		{
			get { return this._wait; }
		}

		/// <summary>Gets or sets the request.</summary>
		/// <value>The request.</value>
		public FtpWebRequest Request
		{
			get
			{
				return this._request;
			}

			set
			{
				this._request = value;
				this._request.ConnectionGroupName = this._servicePointGroupName;
				if (this._servicePoint == null)
				{
					this._servicePoint = this._request.ServicePoint;
				}
			}
		}

		/// <summary>Gets or sets the remote file.</summary>
		/// <value>The remote file.</value>
		public IRemoteFileClass RemoteFile
		{
			get { return this._remoteFile; }
			set { this._remoteFile = value; }
		}

		/// <summary>Gets or sets the FTP status.</summary>
		/// <value>The FTP status.</value>
		public FtpStatus FtpStatus
		{
			get { return this._ftpStatus; }
			set { this._ftpStatus = value; }
		}

		/// <summary>Gets or sets the response.</summary>
		/// <value>The response.</value>
		public FtpWebResponse Response
		{
			get { return this._response; }
			set { this._response = value; }
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
		/// resources by calling Dispose(true).
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);

			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the
		/// method has been called directly or indirectly by a user's code. Managed and unmanaged
		/// resources can be disposed. If disposing equals false, the method has been called by the
		/// runtime from inside the finalizer and you should not reference other objects. Only unmanaged
		/// resources can be disposed.
		/// </summary>
		/// <param name="disposing">True to disposing.</param>
		private void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					if (_wait != null)
					{
						_wait.Close();
					}
				}

				if (_response != null)
				{
					_response.Close();
				}

				if (this._servicePoint != null)
				{
					this._servicePoint.CloseConnectionGroup(this._servicePointGroupName);
					this._servicePoint = null;
				}

				this._disposed = true;
			}
		}

		#endregion
	}
}