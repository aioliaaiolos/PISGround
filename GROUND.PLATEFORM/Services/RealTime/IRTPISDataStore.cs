//---------------------------------------------------------------------------------------------------
// <copyright file="IRTPISDataStore.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using PIS.Ground.Core.Data;

namespace PIS.Ground.RealTime
{
	/// <summary>Interface for irtpis data store.</summary>
	public interface IRTPISDataStore
	{
		/// <summary>Event queue for all listeners interested in Changed events.</summary>
		event ChangedEventHandler Changed;

		/// <summary>Gets mission real time information.</summary>
		/// <param name="missionCode">The mission code.</param>
		/// <returns>The mission real time information.</returns>
		RealTimeInformationType GetMissionRealTimeInformation(string missionCode);

		/// <summary>Sets mission real time information.</summary>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="missionDelay">The mission delay.</param>
		/// <param name="missionWeather">The mission weather.</param>
		void SetMissionRealTimeInformation(string missionCode, RealTimeDelayType missionDelay, RealTimeWeatherType missionWeather);

		/// <summary>Gets station real time information.</summary>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="stationList">List of stations.</param>
		/// <returns>The station real time information.</returns>
		List<RealTimeStationStatusType> GetStationRealTimeInformation(string missionCode, List<string> stationList);

		/// <summary>Sets station real time information.</summary>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="stationInformationList">List of station informations.</param>
		/// <param name="stationResultList">[out] List of station results.</param>
		void SetStationRealTimeInformation(string missionCode, List<RealTimeStationInformationType> stationInformationList, out List<RealTimeStationResultType> stationResultList);

		/// <summary>Clears the real time information.</summary>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="stationList">List of stations.</param>
		/// <param name="clearedStationList">[out] List of cleared stations.</param>
		void ClearRealTimeInformation(string missionCode, List<string> stationList, out List<string> clearedStationList);
	}
}