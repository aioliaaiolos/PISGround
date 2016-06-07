//---------------------------------------------------------------------------------------------------
// <copyright file="Alstom.Product.PISGroundKicker.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Net;
using System.IO;

namespace Alstom.Product.PISGroundKicker
{
	/// <summary>Pis ground kicker windows service.</summary>
	public partial class PISGroundKicker : ServiceBase
	{
		/// <summary>The PIS Ground kicker event log entry name.</summary>
		private const string AlstomPISGroundKickerEventLogEntry = "Alstom.Product.PISGroundKicker";
		/// <summary>The urls separator used to parse url list.</summary>
		private readonly string[] UrlsSeparator = {";"};

		/// <summary>List of objects storing states for urls.</summary>
		private List<StateObjClass> _stateObjList;
		/// <summary>The polling interval in case of responding url.</summary>
		private int _pollingInterval;
		/// <summary>The retry on error interval, in case of non responding url.</summary>
		private int _retryOnErrorInterval;


		/// <summary>Initializes a new instance of the PISGroundKicker class.</summary>
		public PISGroundKicker()
		{
			InitializeComponent();
			if (!System.Diagnostics.EventLog.SourceExists(AlstomPISGroundKickerEventLogEntry))
			{
				System.Diagnostics.EventLog.CreateEventSource(
					AlstomPISGroundKickerEventLogEntry, null);
			}
			eventLog.Source = AlstomPISGroundKickerEventLogEntry;

			_stateObjList = new List<StateObjClass>();
			_pollingInterval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PollingIntervalSec"])*1000;
			_retryOnErrorInterval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["RetryOnErrorIntervalSec"]) * 1000;
		}

		/// <summary>
		/// When implemented in a derived class, executes when a Start command is sent to the service by
		/// the Service Control Manager (SCM) or when the operating system starts (for a service that
		/// starts automatically). Specifies actions to take when the service starts.
		/// </summary>
		/// <param name="args">Data passed by the start command.</param>
		protected override void OnStart(string[] args)
		{
			string urls = System.Configuration.ConfigurationManager.AppSettings["WebServicesUrls"];

			if (!String.IsNullOrEmpty(urls))
			{
				foreach (string url in urls.Split(UrlsSeparator, StringSplitOptions.RemoveEmptyEntries))
				{
					StateObjClass stateObj = new StateObjClass();
					stateObj.url = url;
					stateObj.timerCanceled = false;
					stateObj.errorMode = false;

					System.Threading.TimerCallback TimerDelegate = new System.Threading.TimerCallback(TimerTask);
					System.Threading.Timer TimerItem = new System.Threading.Timer(TimerDelegate, stateObj, 0, _pollingInterval);

					stateObj.timerReference = TimerItem;

					_stateObjList.Add(stateObj);
					eventLog.WriteEntry("Starting thread for url " + stateObj.url, EventLogEntryType.Information);
				}
			}
		}

		/// <summary>
		/// When implemented in a derived class, executes when a Stop command is sent to the service by
		/// the Service Control Manager (SCM). Specifies actions to take when a service stops running.
		/// </summary>
		protected override void OnStop()
		{
			foreach (StateObjClass stateObj in _stateObjList)
			{
				stateObj.timerCanceled = true;	
			}
		}

		/// <summary>Timer task executed, for each url, every pollingInterval or _retryOnErrorInterval.</summary>
		/// <param name="StateObj">The state object.</param>
		private void TimerTask(object StateObj)
		{
			try
			{
				if (StateObj != null)
				{
					StateObjClass stateObj = StateObj as StateObjClass;

					if (stateObj != null)
					{
						try
						{
							HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(stateObj.url);
							request.Credentials = System.Net.CredentialCache.DefaultCredentials;
							request.Method = System.Net.WebRequestMethods.Http.Head;

							using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
							{
								if (response.StatusCode != HttpStatusCode.OK)
								{
									throw new WebException("Server response not valid : " + response.StatusCode);
								}
							}
						}
						catch (Exception ex)
						{
							if (!stateObj.errorMode)
							{
								eventLog.WriteEntry("Timeout with url " + stateObj.url + ". Error : " + ex.Message, EventLogEntryType.Error);
								stateObj.timerReference.Change(0, _retryOnErrorInterval);
								stateObj.errorMode = true;
							}
						}

						if (stateObj.timerCanceled)
						// Dispose Requested.
						{
							stateObj.timerReference.Dispose();
							eventLog.WriteEntry("Done with url " + stateObj.url, EventLogEntryType.Information);
						}
					}
				}
			}
			catch (System.Exception exception)
			{
				eventLog.WriteEntry("Unexpected error in PISGroundKicker.TimerTask : " + exception.ToString(), EventLogEntryType.Error);
			}
		}
	}

	/// <summary>State object class.</summary>
	internal class StateObjClass
	{
		/// <summary>Used to hold url parameter for calls to TimerTask.</summary>
		public string url;
		/// <summary>The timer reference.</summary>
		public System.Threading.Timer timerReference;
		/// <summary>true if timer is canceled.</summary>
		public bool timerCanceled;
		/// <summary>true to enable error mode, false to disable it.</summary>
		public bool errorMode;
	}
}
