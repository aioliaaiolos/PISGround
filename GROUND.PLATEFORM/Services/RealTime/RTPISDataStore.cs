//---------------------------------------------------------------------------------------------------
// <copyright file="RTPISDataStore.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System;

namespace PIS.Ground.RealTime
{
	/// <summary>A delegate type for hooking up change notifications.</summary>
	/// <param name="sender">Source of the event.</param>
	/// <param name="e">Event information.</param>
	public delegate void ChangedEventHandler(object sender, RTPISDataStoreEventArgs e);

	/// <summary>Rtpis data store.</summary>
	public class RTPISDataStore : IRTPISDataStore
	{
		#region const

		#endregion

		#region fields

		/// <summary>Information describing the mission.</summary>
		protected static Dictionary<string, RealTimeInformationType> _missionData = null;

		/// <summary>Information describing the station.</summary>
		protected static Dictionary<string, List<RealTimeStationInformationType>> _stationData = null;

		/// <summary>
		/// An event that clients can use to be notified whenever the elements of the list change.
		/// </summary>
		public event ChangedEventHandler _changed;

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the RTPISDataStore class.</summary>
		public RTPISDataStore()
		{
			if (_missionData == null)
			{
				_missionData = new Dictionary<string, RealTimeInformationType>();
			}

			if (_stationData == null)
			{
				_stationData = new Dictionary<string, List<RealTimeStationInformationType>>();
			}
		}

		#endregion

		#region events

		// Invoke the Changed event; called whenever list changes
		protected virtual void OnChanged(RTPISDataStoreEventArgs e)
		{
			if (_changed != null)
				_changed(this, e);
		}

		#endregion

		#region properties

		/// <summary>Event queue for all listeners interested in Changed events.</summary>
		public event ChangedEventHandler Changed
		{
			add
			{
				_changed += value;
			}
			remove
			{
				_changed -= value;
			}
		}

		#endregion

		#region methods 

		#region IRTPISDataStore Members

		/// <summary>Gets mission real time information.</summary>
		/// <param name="missionCode">The mission code.</param>
		/// <returns>The mission real time information.</returns>
		RealTimeInformationType IRTPISDataStore.GetMissionRealTimeInformation(string missionCode)
		{
   			RealTimeInformationType result = null;

			if (!string.IsNullOrEmpty(missionCode))
			{
				lock (_missionData)
				{
                    _missionData.TryGetValue(missionCode, out result);
				}
			}

			return result;
		}

		/// <summary>Sets mission real time information.</summary>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="missionDelay">The mission delay.</param>
		/// <param name="missionWeather">The mission weather.</param>
		void IRTPISDataStore.SetMissionRealTimeInformation(string missionCode, RealTimeDelayType missionDelay, RealTimeWeatherType missionWeather)
		{
			bool dataUpdated = false;
			if (missionDelay != null || missionWeather != null)
			{

				if (!string.IsNullOrEmpty(missionCode))
				{
					lock (_missionData)
					{
                        RealTimeInformationType update;
                        bool found = _missionData.TryGetValue(missionCode, out update);
                        if (!found)
                        {
                            update = new RealTimeInformationType();
                        }

						if (missionDelay != null)
						{
							update.MissionDelay = missionDelay;
							update.MissionDelay.UpdateDate = DateTime.Now;
							dataUpdated = true;
						}

						if (missionWeather != null)
						{
							update.MissionWeather = missionWeather;
							update.MissionWeather.UpdateDate = DateTime.Now;
							dataUpdated = true;
						}

                        if (!found)
                        {
                            _missionData.Add(missionCode, update);
                        }

						///Call event
						if (dataUpdated)
						{
							OnChanged(new RTPISDataStoreEventArgs(missionCode, null));
						}
					}
				}
			}
		}

		/// <summary>Gets station real time information.</summary>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="stationList">List of stations.</param>
		/// <returns>The station real time information.</returns>
		List<RealTimeStationStatusType> IRTPISDataStore.GetStationRealTimeInformation(string missionCode, List<string> stationList)
		{
			List<RealTimeStationStatusType> result = new List<RealTimeStationStatusType>();

			if (!string.IsNullOrEmpty(missionCode))
			{
				lock (_stationData)
				{
					try
					{
                        List<RealTimeStationInformationType>  stationForMission;
                        if (_stationData.TryGetValue(missionCode, out stationForMission))
                        {
						if (stationList != null && stationList.Count > 0)
						{
							List<string> stationListCopy = new List<string>(stationList);

							foreach (var stationInfo in _stationData[missionCode])
							{
								if (stationListCopy.Contains(stationInfo.StationCode))
								{
									result.Add(new RealTimeStationStatusType(stationInfo));
									stationListCopy.Remove(stationInfo.StationCode);
								}
							}

							foreach (var item in stationListCopy)
							{
                                RealTimeStationStatusType tmpStatus = new RealTimeStationStatusType(){ 
                                    StationID = item,
                                    StationResult = RealTimeStationResultEnum.ErrorInvalidStationForMission };
								result.Add(tmpStatus);
							}
						}
						else
						{
                            result.Capacity = stationForMission.Count;
							foreach (var stationInfo in _stationData[missionCode])
							{
								result.Add(new RealTimeStationStatusType(stationInfo));
							}
						}
                        }
					}
					catch (KeyNotFoundException)
					{
						// key not found, return null
						result = null;
					}
				}
			}

			return result;
		}

		/// <summary>Sets station real time information.</summary>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="stationInformationList">List of station informations.</param>
		/// <param name="stationResultList">[out] List of station results.</param>
		void IRTPISDataStore.SetStationRealTimeInformation(string missionCode, List<RealTimeStationInformationType> stationInformationList, out List<RealTimeStationResultType> stationResultList)
		{
			bool dataUpdated = false;
            stationResultList = new List<RealTimeStationResultType>();

			if (!string.IsNullOrEmpty(missionCode) && stationInformationList != null && stationInformationList.Count > 0)
			{
				KeyValuePair<string, List<string>> stationUpdateList = new KeyValuePair<string, List<string>>(missionCode,new List<string>());

				lock (_stationData)
				{
					foreach (var stationInfo in stationInformationList)
					{
						if (stationInfo != null)
						{
							RealTimeStationResultType stationUpdate = null;
							RealTimeStationInformationType stationInfoType = null;

							try
							{
								stationInfoType = _stationData[missionCode].Find(x => x.StationCode == stationInfo.StationCode);
								if (stationInfoType != null)
								{
									_stationData[missionCode].Remove(stationInfoType);
									stationInfoType.UpdateFrom(stationInfo);
								}
								else
								{
									stationInfoType = stationInfo;
                                    stationInfoType.SetUpdateDate(DateTime.Now);                                    
								}
							}
							catch (KeyNotFoundException)
							{
								// key not found, we will use the newly created instance
								_stationData.Add(missionCode, new List<RealTimeStationInformationType>());
								stationInfoType = stationInfo;
								stationInfoType.SetUpdateDate(DateTime.Now);
							}

							stationUpdate = new RealTimeStationResultType() { StationID = stationInfoType.StationCode };

							if (stationInfoType != null)
							{
								if (stationInfo.StationData != null)
								{
									if (stationInfo.StationData.StationDelay != null)
									{
										stationUpdate.DelayResult = RealTimeStationResultEnum.DataOk;
									}
									else
									{
										stationUpdate.DelayResult = RealTimeStationResultEnum.InfoNoData;
									}

									if (stationInfo.StationData.StationPlatform != null)
									{
										stationUpdate.PlatformResult = RealTimeStationResultEnum.DataOk;
									}
									else
									{
										stationUpdate.PlatformResult = RealTimeStationResultEnum.InfoNoData;
									}

									if (stationInfo.StationData.StationWeather != null)
									{
										stationUpdate.WeatherResult = RealTimeStationResultEnum.DataOk;
									}
									else
									{
										stationUpdate.WeatherResult = RealTimeStationResultEnum.InfoNoData;
									}

									if (stationInfo.StationData.StationConnectionList != null)
									{
                                        stationUpdate.ConnectionsResultList = new List<RealTimeConnectionResultType>();

										foreach (var connectionData in stationInfoType.StationData.StationConnectionList)
										{
                                            stationUpdate.ConnectionsResultList.Add(new RealTimeConnectionResultType()
                                            {
																					Operator = connectionData.Operator,
																					CommercialNumber = connectionData.CommercialNumber,
																					ConnectionResult = RealTimeStationResultEnum.InfoNoData});
										}

										foreach (var connectionData in stationInfo.StationData.StationConnectionList)
										{
											if (connectionData != null)
											{
                                                var connection = stationUpdate.ConnectionsResultList.Find(x => string.Compare(x.Operator, connectionData.Operator) == 0 && string.Compare(x.CommercialNumber, connectionData.CommercialNumber) == 0);
												if (connection != null)
												{
                                                    stationUpdate.ConnectionsResultList.Remove(connection);
												}

                                                stationUpdate.ConnectionsResultList.Add(new RealTimeConnectionResultType()
                                                {
																					Operator = connectionData.Operator,
																					CommercialNumber = connectionData.CommercialNumber,
																					ConnectionResult = RealTimeStationResultEnum.DataOk});
											}
										}
									}
								}
								else
								{
									stationUpdate.DelayResult = RealTimeStationResultEnum.InfoNoData;
									stationUpdate.PlatformResult = RealTimeStationResultEnum.InfoNoData;
									stationUpdate.WeatherResult = RealTimeStationResultEnum.InfoNoData;
                                    stationUpdate.ConnectionsResultList = null;
								}

								_stationData[missionCode].Add(stationInfoType);
								stationUpdateList.Value.Add(stationUpdate.StationID);
								stationResultList.Add(stationUpdate);
								dataUpdated = true;
							}
						}
					}

					///Call event
					if (dataUpdated)
					{
						OnChanged(new RTPISDataStoreEventArgs(null, stationUpdateList));
					}
				}
			}
		}

		/// <summary>Clears the real time information.</summary>
		/// <param name="missionCode">The mission code.</param>
		/// <param name="stationList">List of stations.</param>
		/// <param name="clearedStationList">[out] List of cleared stations.</param>
		void IRTPISDataStore.ClearRealTimeInformation(string missionCode, List<string> stationList, out List<string> clearedStationList)
		{
			bool dataUpdated = false;
			clearedStationList = null;

			if (!string.IsNullOrEmpty(missionCode))
			{
				clearedStationList = new List<string>();
				string missionUpdate = null;
				KeyValuePair<string, List<string>>? stationUpdateList = new KeyValuePair<string, List<string>>(missionCode, new List<string>());

				if (stationList == null || stationList.Count == 0)
				{
					try
					{
						lock (_missionData)
						{
							if (_missionData.ContainsKey(missionCode))
							{
								_missionData.Remove(missionCode);
								missionUpdate = missionCode;
								dataUpdated = true;
							}
						}
					}
					catch (KeyNotFoundException)
					{
						// no mission data to clear, clear station data only
					}
				}

				try
				{
					lock (_stationData)
					{
						if (stationList == null || stationList.Count == 0)
						{
							foreach (var stationData in _stationData[missionCode])
							{
								clearedStationList.Add(stationData.StationCode);
							}

							_stationData.Remove(missionCode);
						}
						else
						{
							foreach (var stationStr in stationList)
							{
								var stationData = _stationData[missionCode].Find(x => string.Compare(x.StationCode, stationStr) == 0);
								if (stationData != null)
								{
									clearedStationList.Add(stationData.StationCode);
									_stationData[missionCode].Remove(stationData);
								}
							}
						}
					}
				}
				catch (KeyNotFoundException)
				{
					//Nothing to do as there is no station data associated to mission code
				}

				if (clearedStationList.Count == 0)
				{
					clearedStationList = null;
					stationUpdateList = null;
				}
				else
				{
					((KeyValuePair<string, List<string>>)stationUpdateList).Value.AddRange(clearedStationList);
					dataUpdated = true;
				}

				///Call event
				if (dataUpdated)
				{
					OnChanged(new RTPISDataStoreEventArgs(missionCode, stationUpdateList));
				}
			}
		}

		#endregion

		#endregion
	}
}