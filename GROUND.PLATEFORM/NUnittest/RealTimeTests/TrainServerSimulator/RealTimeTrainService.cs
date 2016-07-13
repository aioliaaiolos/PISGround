//---------------------------------------------------------------------------------------------------
// <copyright file="RealTimeTrainService.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Moq;
using System.Globalization;

namespace PIS.Ground.RealTimeTests.TrainServerSimulator
{
	/// <summary>Real time train service.</summary>
	public class RealTimeTrainService : Train.RealTime.IRealTimeTrainService, IDisposable
	{
		#region fields

		/// <summary>The endpoint of the service.</summary>
		private string _endpoint;

		/// <summary>The host.</summary>
        private ServiceHost _host;

        /// <summary>
        /// Gets or sets the expected mission code.
        /// </summary>
        /// <value>
        /// The expected mission code.
        /// </value>
        public string ExpectedMissionCode { get; set; }

        /// <summary>
        /// Gets or sets the last request data received by method SetMissionRealTime.
        /// </summary>
        public PIS.Train.RealTime.SetMissionRealTimeRequest LastMissionRealTimeRequest { get; set; }

        /// <summary>
        /// Gets or sets the last request data received by method SetStationRealTime.
        /// </summary>
        public PIS.Train.RealTime.SetStationRealTimeRequest LastStationRealTimeRequest { get; set; }

		#endregion

		#region server managment

		/// <summary>Initializes a new instance of the RealTimeTrainService class.</summary>
        /// <param name="pIpAdress">The ip address of the service.</param>
        /// <param name="pPort">The port </param>
		public RealTimeTrainService(string pIpAdress, ushort pPort)
		{
            if (string.IsNullOrEmpty(pIpAdress))
            {
                throw new ArgumentNullException("pIpAddress");
            }

            if (pPort == 0)
            {
                throw new ArgumentOutOfRangeException("pPort");
            }

			_endpoint = string.Format(CultureInfo.InvariantCulture, "http://{0}:{1}", pIpAdress, pPort);
		}

		/// <summary>Starts this object.</summary>
		public void Start()
		{
			try
			{
				if (_host != null)
				{
                    if (_host.State == CommunicationState.Faulted)
                    {
                        _host.Abort();
                    }
                    
                    _host.Close();
                    _host = null;
				}

				//Create a URI to serve as the base address
				Uri httpURL = new Uri(_endpoint);

				//Create ServiceHost
				_host = new ServiceHost(this, httpURL);

				// set instancecontextmode
				var behavior = _host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
				behavior.InstanceContextMode = InstanceContextMode.Single;

				//Start the Service
				_host.Open();
			}
			catch (Exception)
			{
                if (_host != null)
                {
                    if (_host.State == CommunicationState.Faulted)
                    {
                        _host.Abort();
                    }

                    _host.Close();
                    _host = null;
                }
				throw;
			}
		}

		/// <summary>Stops this object.</summary>
		public void Stop()
		{
			if (_host != null)
			{
				_host.Close();
				_host = null;
			}
		}

		#endregion

		#region IRealTimeTrainService Members

		/// <summary>Sets mission real time.</summary>
		/// <param name="request">The request.</param>
		/// <returns>The response</returns>
		PIS.Train.RealTime.SetMissionRealTimeResponse PIS.Train.RealTime.IRealTimeTrainService.SetMissionRealTime(PIS.Train.RealTime.SetMissionRealTimeRequest pRequest)
		{
            LastMissionRealTimeRequest = pRequest;

            PIS.Train.RealTime.SetMissionRealTimeResponse response = new PIS.Train.RealTime.SetMissionRealTimeResponse(new PIS.Train.RealTime.ListOfResultType());

            if (pRequest == null)
            {
                response.ResultList.Add(new PIS.Train.RealTime.ResultType()
                {
                    MissionCode = pRequest.MissionID,
                    ResultCode = PIS.Train.RealTime.ResultCodeEnum.InvalidSoapRequest,
                    StationCode = string.Empty
                });

            }
            else
            {
                response.ResultList.Add(new PIS.Train.RealTime.ResultType()
                {
                    MissionCode = pRequest.MissionID,
                    ResultCode = (pRequest.MissionID == ExpectedMissionCode) ? PIS.Train.RealTime.ResultCodeEnum.NotCurrentMission : PIS.Train.RealTime.ResultCodeEnum.OK
                });

            }
            return response;
		}

		/// <summary>Sets station real time.</summary>
		/// <param name="request">The request.</param>
		/// <returns>The response</returns>
		PIS.Train.RealTime.SetStationRealTimeResponse PIS.Train.RealTime.IRealTimeTrainService.SetStationRealTime(PIS.Train.RealTime.SetStationRealTimeRequest request)
		{
            LastStationRealTimeRequest = request;
            return null;
		}

		#endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
        }

        #endregion
    }
}
