//---------------------------------------------------------------------------------------------------
// <copyright file="RTPISDataStoreEventArgs.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace PIS.Ground.RealTime
{
	/// <summary>Additional information for rtpis data store events.</summary>
	public class RTPISDataStoreEventArgs : EventArgs
	{
		/// <summary>Initializes a new instance of the RTPISDataStoreEventArgs class.</summary>
		public RTPISDataStoreEventArgs()
		{
		}

		/// <summary>Initializes a new instance of the RTPISDataStoreEventArgs class.</summary>
		/// <param name="missionData">Information describing the mission.</param>
		/// <param name="stationDataList">List of station data.</param>
		public RTPISDataStoreEventArgs(string missionCode, KeyValuePair<string, List<string>>? stationCodeList)
		{
			MissionCode = missionCode;
			StationCodeList = stationCodeList;
		}

		/// <summary>Information describing the mission.</summary>
		public string MissionCode;
		
		/// <summary>List of station data.</summary>
		public KeyValuePair<string, List<string>>? StationCodeList;
	}
}
