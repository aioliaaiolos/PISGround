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

namespace PIS.Ground.RealTimeTests.TrainServerSimulator
{
	/// <summary>Real time train service.</summary>
	public class RealTimeTrainService : Train.RealTime.IRealTimeTrainService
	{
		#region fileds

		/// <summary>The mock train.</summary>
		private Mock<Train.RealTime.IRealTimeTrainService> _mockTrain = null;

		/// <summary>The mock endpoint.</summary>
		private string _mockEndpoint = null;

		/// <summary>The host.</summary>
		private ServiceHost _host = null;

		#endregion

		#region server managment

		/// <summary>Initializes a new instance of the RealTimeTrainService class.</summary>
		public RealTimeTrainService()
		{
		}

		/// <summary>Initializes a new instance of the RealTimeTrainService class.</summary>
		/// <param name="mockTrain">The mock train.</param>
		/// <param name="mockEndpoint">The mock endpoint.</param>
		public RealTimeTrainService(Mock<Train.RealTime.IRealTimeTrainService> mockTrain, string ipAdress, ushort ipPort)
		{
			_mockTrain = mockTrain;
			_mockEndpoint = "http://" + ipAdress + ":" + ipPort;
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
				Uri httpURL = new Uri(_mockEndpoint);

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
		PIS.Train.RealTime.SetMissionRealTimeResponse PIS.Train.RealTime.IRealTimeTrainService.SetMissionRealTime(PIS.Train.RealTime.SetMissionRealTimeRequest request)
		{
			return _mockTrain.Object.SetMissionRealTime(request);
		}

		/// <summary>Sets station real time.</summary>
		/// <param name="request">The request.</param>
		/// <returns>The response</returns>
		PIS.Train.RealTime.SetStationRealTimeResponse PIS.Train.RealTime.IRealTimeTrainService.SetStationRealTime(PIS.Train.RealTime.SetStationRealTimeRequest request)
		{
			return _mockTrain.Object.SetStationRealTime(request);
		}

		#endregion
	}
}
