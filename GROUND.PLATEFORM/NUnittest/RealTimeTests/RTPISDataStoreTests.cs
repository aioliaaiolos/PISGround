//---------------------------------------------------------------------------------------------------
// <copyright file="RTPISDataStoreTests.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2014.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using NUnit.Framework;
using PIS.Ground.RealTime;

namespace PIS.Ground.RealTimeTests
{
	/// <summary>Rtpis data store tests.</summary>
	[TestFixture]
	public class RTPISDataStoreTests
	{
		#region attributes

		/// <summary>The RTPISDataStore.</summary>
		private static IRTPISDataStore _rtpisds = null;

		#endregion

		#region Tests management

		/// <summary>Initializes a new instance of the RTPISDataStoreTests class.</summary>
		public RTPISDataStoreTests()
		{
			// Nothing to do
		}

		/// <summary>Setups called before each test to initialize variables.</summary>
		[SetUp]
		public void Setup()
		{
			_rtpisds = new RTPISDataStoreTested();
		}

		/// <summary>Tear down called after each test to clean.</summary>
		[TearDown]
		public void TearDown()
		{
			((RTPISDataStoreTested)_rtpisds).ClearDataStores();
			_rtpisds = null;
		}

		#endregion

		#region Tests

		#region Get/Set/MissionRealTimeInformation

		/// <summary>Mission real time information mission code null.</summary>
		[Test]
		public void MissionRealTimeInformationMissionCodeNull()
		{
			string missionCode = null;
			RealTimeDelayType delay = null;
			RealTimeWeatherType weather = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			Assert.IsNull(rtr);
			Assert.IsFalse(WasItHit);
		}

		/// <summary>Mission real time information mission code empty.</summary>
		[Test]
		public void MissionRealTimeInformationMissionCodeEmpty()
		{
			string missionCode = string.Empty;
			RealTimeDelayType delay = null;
			RealTimeWeatherType weather = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			Assert.IsNull(rtr);
			Assert.IsFalse(WasItHit);
		}

		/// <summary>Mission real time information mission code not found.</summary>
		[Test]
		public void MissionRealTimeInformationMissionCodeNotFound()
		{
			string missionCode = "DummyMission";
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			Assert.IsNull(rtr);
			Assert.IsFalse(WasItHit);
		}

		/// <summary>Mission real time information no delay data.</summary>
		[Test]
		public void MissionRealTimeInformationNoDelayData()
		{
			string missionCode = "DummyMission";
			RealTimeDelayType delay = null;
			RealTimeWeatherType weather = new RealTimeWeatherType();
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			Assert.AreEqual(delay, rtr.MissionDelay);
			Assert.AreEqual(weather, rtr.MissionWeather);
			Assert.IsTrue(WasItHit);
		}

		/// <summary>Mission real time information no weather data.</summary>
		[Test]
		public void MissionRealTimeInformationNoWeatherData()
		{
			string missionCode = "DummyMission";
			RealTimeDelayType delay = new RealTimeDelayType();
			RealTimeWeatherType weather = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			Assert.AreEqual(delay, rtr.MissionDelay);
			Assert.AreEqual(weather, rtr.MissionWeather);
			Assert.IsTrue(WasItHit);
		}

		/// <summary>Mission real time information no data.</summary>
		[Test]
		public void MissionRealTimeInformationNoData()
		{
			string missionCode = "DummyMission";
			RealTimeDelayType delay = null;
			RealTimeWeatherType weather = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			Assert.IsNull(rtr);
			Assert.IsFalse(WasItHit);
		}

		/// <summary>Mission real time information all data.</summary>
		[Test]
		public void MissionRealTimeInformationAllData()
		{
			string missionCode = "DummyMission";
			RealTimeDelayType delay = new RealTimeDelayType();
			RealTimeWeatherType weather = new RealTimeWeatherType();
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			Assert.AreEqual(delay, rtr.MissionDelay);
			Assert.AreEqual(weather, rtr.MissionWeather);
			Assert.IsTrue(WasItHit);
		}

		/// <summary>Mission real time information all data check UpdateDate test.</summary>
		[Test]
		public void MissionRealTimeInformationAllDataCheckUpdateDate()
		{
			string missionCode = "DummyMission";
			RealTimeDelayType delay = new RealTimeDelayType() { Delay = 1, DelayReason = "Test Reason", DelayReasonCode = 2 };
			RealTimeWeatherType weather = new RealTimeWeatherType()
				{
					Humidity = "40%",
					TemperatureInCentigrade = "20C",
					TemperatureInFahrenheit = "68F",
					WeatherCondition = "Sunny",
					WeatherConditionCode = 1
				};

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			// Check Update Date
			DateTime dateTimeAfterSet = DateTime.Now;
			TimeSpan maxDelaySinceSet = new TimeSpan(0, 0, 5);

			Assert.Less(dateTimeAfterSet.Subtract(rtr.MissionDelay.UpdateDate), maxDelaySinceSet, "The 'MissionDelay.UpdateDate' should correspond to the date/time of when we called SetRealTimeMissionInformation() and should be smaller and very close (5 sec in this test) to the current date/time as we just set it.");
			Assert.GreaterOrEqual(dateTimeAfterSet.CompareTo(rtr.MissionDelay.UpdateDate), 0, "The current date/time should be greater (or equal) to the 'MissionDelay.UpdateDate' date time (the UpdateDate should correspond to the date/time of when we called SetRealTimeMissionInformation()).");
			Assert.Less(dateTimeAfterSet.Subtract(rtr.MissionWeather.UpdateDate), maxDelaySinceSet, "The 'MissionWeather.UpdateDate' should correspond to the date/time of when we called SetRealTimeMissionInformation() and should be smaller and very close to the current date/time as we just set it.");
			Assert.GreaterOrEqual(dateTimeAfterSet.CompareTo(rtr.MissionWeather.UpdateDate), 0, "The current date/time should be greater (or equal) to the 'MissionWeather.UpdateDate' date time (the UpdateDate should correspond to the date/time of when we called SetRealTimeMissionInformation()).");
		}

		#endregion

		#region Get/Set/StationRealTimeInformation

		/// <summary>Station real time information mission code null.</summary>
		[Test]
		public void StationRealTimeInformationMissionCodeNull()
		{
			string missionCode = null;
			List<RealTimeStationInformationType> stationInformationList = null;
			List<string> stationList = null;
			List<RealTimeStationResultType> stationResultsList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList);
			List<RealTimeStationStatusType> rtr = _rtpisds.GetStationRealTimeInformation(missionCode, stationList);

			Assert.IsNull(rtr);
			Assert.IsNull(stationResultsList);
			Assert.IsFalse(WasItHit);
		}

		/// <summary>Station real time information mission code empty.</summary>
		[Test]
		public void StationRealTimeInformationMissionCodeEmpty()
		{
			string missionCode = string.Empty;
			List<RealTimeStationInformationType> stationInformationList = null;
			List<string> stationList = null;
			List<RealTimeStationResultType> stationResultsList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList);
			List<RealTimeStationStatusType> rtr = _rtpisds.GetStationRealTimeInformation(missionCode, stationList);

			Assert.IsNull(rtr);
			Assert.IsNull(stationResultsList);
			Assert.IsFalse(WasItHit);
		}

		/// <summary>Station real time information mission code not found.</summary>
		[Test]
		public void StationRealTimeInformationMissionCodeNotFound()
		{
			string missionCode = "DummyMission";
			List<string> stationList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			List<RealTimeStationStatusType> rtr = _rtpisds.GetStationRealTimeInformation(missionCode, stationList);

			Assert.IsNull(rtr);
			Assert.IsFalse(WasItHit);
		}

		/// <summary>Station real time information station set null get not null.</summary>
		[Test]
		public void StationRealTimeInformationStationSetNullGetNotNull()
		{
			string missionCode = "DummyMission";
			List<RealTimeStationInformationType> stationInformationList = null;
			List<string> stationList = new List<string> { "Station0" };
			List<RealTimeStationResultType> stationResultsList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList);
			List<RealTimeStationStatusType> rtr = _rtpisds.GetStationRealTimeInformation(missionCode, stationList);

			Assert.IsNull(rtr);
			Assert.IsNull(stationResultsList);
			Assert.IsFalse(WasItHit);
		}

		/// <summary>Station real time information station set not null get null.</summary>
		[Test]
		public void StationRealTimeInformationStationSetNotNullGetNull()
		{
			string missionCode = "DummyMission";
			List<RealTimeStationInformationType> stationInformationList = new List<RealTimeStationInformationType> {new RealTimeStationInformationType() { StationCode = "Station0", StationData = new RealTimeStationDataType() } };
			List<string> stationList = null;
			List<RealTimeStationResultType> stationResultsList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList);
			List<RealTimeStationStatusType> rtr = _rtpisds.GetStationRealTimeInformation(missionCode, stationList);

			Assert.IsNotNull(rtr);
			Assert.IsNotNull(stationResultsList);
			Assert.AreEqual("Station0", rtr[0].StationID);
			Assert.AreEqual("Station0", stationResultsList[0].StationID);
			Assert.IsTrue(WasItHit);
		}

		/// <summary>Station real time information station set not null get not null.</summary>
		[Test]
		public void StationRealTimeInformationStationSetNotNullGetNotNull()
		{
			string missionCode = "DummyMission";
			List<RealTimeStationInformationType> stationInformationList = new List<RealTimeStationInformationType> { new RealTimeStationInformationType() { StationCode = "Station0", StationData = new RealTimeStationDataType() } };
			List<string> stationList = new List<string> { "Station0" };
			List<RealTimeStationResultType> stationResultsList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList);
			List<RealTimeStationStatusType> rtr = _rtpisds.GetStationRealTimeInformation(missionCode, stationList);

			Assert.IsNotNull(rtr);
			Assert.IsNotNull(stationResultsList);
			Assert.AreEqual("Station0", stationResultsList[0].StationID);
			Assert.AreEqual("Station0", rtr[0].StationID);
			Assert.IsTrue(WasItHit);
		}

		/// <summary>Station real time information station update.</summary>
		[Test]
		public void StationRealTimeInformationStationDataUpdate()
		{
			string missionCode = "DummyMission";
			List<RealTimeStationInformationType> stationInformationList = new List<RealTimeStationInformationType> {
					new RealTimeStationInformationType() {
						StationCode = "Station0", 
						StationData = new RealTimeStationDataType() {
							StationDelay = new RealTimeDelayType(),
							StationPlatform = new RealTimeStationPlatformType(),
							StationConnectionList = null,
							StationWeather = new RealTimeWeatherType() } } };

			List<RealTimeStationInformationType> stationInformationListUpdate = new List<RealTimeStationInformationType> {
					new RealTimeStationInformationType() {
						StationCode = "Station0", 
						StationData = new RealTimeStationDataType() {
							StationDelay = null,
							StationPlatform = null,
							StationConnectionList = new List<RealTimeStationConnectionType>(){
								new RealTimeStationConnectionType() {
									Operator = "Operator",
									CommercialNumber = "CommercialNumber"}},
							StationWeather = new RealTimeWeatherType() } } };

			List<RealTimeStationInformationType> stationInformationListUpdate2 = new List<RealTimeStationInformationType> {
					new RealTimeStationInformationType() {
						StationCode = "Station0", 
						StationData = new RealTimeStationDataType() {
							StationDelay = new RealTimeDelayType(),
							StationPlatform = null,
							StationConnectionList = new List<RealTimeStationConnectionType>() {
								new RealTimeStationConnectionType() {
									Operator = "Operator",
									CommercialNumber = "CommercialNumber"}},
							StationWeather = null } } };

			List<string> stationList = new List<string> { "Station0" };
			List<RealTimeStationResultType> stationResultsList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList);
			Assert.IsNotNull(stationResultsList);
			Assert.AreEqual("Station0", stationResultsList[0].StationID);
			Assert.AreEqual(RealTimeStationResultEnum.DataOk, stationResultsList[0].DelayResult);
			Assert.AreEqual(RealTimeStationResultEnum.DataOk, stationResultsList[0].PlatformResult);
			Assert.AreEqual(RealTimeStationResultEnum.DataOk, stationResultsList[0].WeatherResult);
            Assert.IsNull(stationResultsList[0].ConnectionsResultList);
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			List<RealTimeStationStatusType> rtr = _rtpisds.GetStationRealTimeInformation(missionCode, stationList);
			Assert.IsNotNull(rtr);
			Assert.AreEqual("Station0", rtr[0].StationID);

			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationListUpdate, out stationResultsList);
			Assert.IsNotNull(stationResultsList);
			Assert.AreEqual("Station0", stationResultsList[0].StationID);
			Assert.AreEqual(RealTimeStationResultEnum.InfoNoData, stationResultsList[0].DelayResult);
			Assert.AreEqual(RealTimeStationResultEnum.InfoNoData, stationResultsList[0].PlatformResult);
			Assert.AreEqual(RealTimeStationResultEnum.DataOk, stationResultsList[0].WeatherResult);
            Assert.IsNotNull(stationResultsList[0].ConnectionsResultList);
            foreach (var connection in stationResultsList[0].ConnectionsResultList)
			{
				Assert.AreEqual("Operator", connection.Operator);
				Assert.AreEqual("CommercialNumber", connection.CommercialNumber);
				Assert.AreEqual(RealTimeStationResultEnum.DataOk, connection.ConnectionResult);
			}
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			rtr = _rtpisds.GetStationRealTimeInformation(missionCode, stationList);
			Assert.IsNotNull(rtr);
			Assert.AreEqual("Station0", rtr[0].StationID);

			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationListUpdate2, out stationResultsList);
			Assert.IsNotNull(stationResultsList);
			Assert.AreEqual("Station0", stationResultsList[0].StationID);
			Assert.AreEqual(RealTimeStationResultEnum.DataOk, stationResultsList[0].DelayResult);
			Assert.AreEqual(RealTimeStationResultEnum.InfoNoData, stationResultsList[0].PlatformResult);
			Assert.AreEqual(RealTimeStationResultEnum.InfoNoData, stationResultsList[0].WeatherResult);
            Assert.IsNotNull(stationResultsList[0].ConnectionsResultList);
            foreach (var connection in stationResultsList[0].ConnectionsResultList)
			{
				Assert.AreEqual("Operator", connection.Operator);
				Assert.AreEqual("CommercialNumber", connection.CommercialNumber);
				Assert.AreEqual(RealTimeStationResultEnum.DataOk, connection.ConnectionResult);
			}
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			rtr = _rtpisds.GetStationRealTimeInformation(missionCode, stationList);
			Assert.IsNotNull(rtr);
			Assert.AreEqual("Station0", rtr[0].StationID);
		}

		/// <summary>Station real time information all data (with SetRealTimeMissionInformation()) - check UpdateDate test.</summary>
		[Test]
		public void StationRealTimeInformationStationAllDataWithSetMissionCheckUpdateDate()
		{
			string missionCode = "DummyMission";
			List<RealTimeStationInformationType> stationInformationList = CreateDummyRealTimeStationInformationList();
			List<string> stationList = new List<string> { "Station0" };
			List<RealTimeStationResultType> stationResultsList = null;
			RealTimeDelayType realTimeDelayInfo = new RealTimeDelayType();
			RealTimeWeatherType realTimeWeatherInfo = new RealTimeWeatherType();

			_rtpisds.SetMissionRealTimeInformation(missionCode, realTimeDelayInfo, realTimeWeatherInfo);
			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList);
			List<RealTimeStationStatusType> realTimeStationStatusList = _rtpisds.GetStationRealTimeInformation(missionCode, stationList);
			Assert.IsNotNull(realTimeStationStatusList, "GetStationRealTimeInformation() should return the real time station information we just set");
			Assert.AreEqual(1, realTimeStationStatusList.Count, "GetStationRealTimeInformation() should return the only real time station information we just set.");
			RealTimeStationStatusType realTimeStationStatus = realTimeStationStatusList[0];
			Assert.IsNotNull(realTimeStationStatus, "Make sure we have data to work with.");

			// Check Update Date
			DateTime dateTimeAfterSet = DateTime.Now;
			TimeSpan maxDelaySinceSet = new TimeSpan(0, 0, 5);
			DateTime updateDate;

			updateDate = realTimeStationStatus.StationConnectionList[0].UpdateDate;
			Assert.Less(dateTimeAfterSet.Subtract(updateDate), maxDelaySinceSet, "The 'StationConnectionList[0].UpdateDate' should correspond to the date/time of when we called SetRealTimeStationInformation() and should be smaller and very close (5 sec in this test) to the current date/time as we just set it.");
			Assert.GreaterOrEqual(dateTimeAfterSet.CompareTo(updateDate), 0, "The current date/time should be greater (or equal) to the 'StationConnectionList[0].UpdateDate' date time (the UpdateDate should correspond to the date/time of when we called SetRealTimeStationInformation()).");
			
			updateDate = realTimeStationStatus.StationDelay.UpdateDate;
			Assert.Less(dateTimeAfterSet.Subtract(updateDate), maxDelaySinceSet, "The 'StationDelay.UpdateDate' should correspond to the date/time of when we called SetRealTimeStationInformation() and should be smaller and very close (5 sec in this test) to the current date/time as we just set it.");
			Assert.GreaterOrEqual(dateTimeAfterSet.CompareTo(updateDate), 0, "The current date/time should be greater (or equal) to the 'StationDelay.UpdateDate' date time (the UpdateDate should correspond to the date/time of when we called SetRealTimeStationInformation()).");

			updateDate = realTimeStationStatus.StationPlatform.UpdateDate;
			Assert.Less(dateTimeAfterSet.Subtract(updateDate), maxDelaySinceSet, "The 'StationPlatform.UpdateDate' should correspond to the date/time of when we called SetRealTimeStationInformation() and should be smaller and very close (5 sec in this test) to the current date/time as we just set it.");
			Assert.GreaterOrEqual(dateTimeAfterSet.CompareTo(updateDate), 0, "The current date/time should be greater (or equal) to the 'StationPlatform.UpdateDate' date time (the UpdateDate should correspond to the date/time of when we called SetRealTimeStationInformation()).");

			updateDate = realTimeStationStatus.StationWeather.UpdateDate;
			Assert.Less(dateTimeAfterSet.Subtract(updateDate), maxDelaySinceSet, "The 'StationWeather.UpdateDate' should correspond to the date/time of when we called SetRealTimeStationInformation() and should be smaller and very close (5 sec in this test) to the current date/time as we just set it.");
			Assert.GreaterOrEqual(dateTimeAfterSet.CompareTo(updateDate), 0, "The current date/time should be greater (or equal) to the 'StationWeather.UpdateDate' date time (the UpdateDate should correspond to the date/time of when we called SetRealTimeStationInformation()).");
		}

		/// <summary>Station real time information all data (whithout SetRealTimeMissionInformation()) - check UpdateDate test.</summary>
		[Test]
		public void StationRealTimeInformationStationAllDataWithoutSetMissionCheckUpdateDate()
		{
			string missionCode = "DummyMission";
			List<RealTimeStationInformationType> stationInformationList = CreateDummyRealTimeStationInformationList();
			List<string> stationList = new List<string> { "Station0" };
			List<RealTimeStationResultType> stationResultsList = null;

			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList);
			List<RealTimeStationStatusType> realTimeStationStatusList = _rtpisds.GetStationRealTimeInformation(missionCode, stationList);
			Assert.IsNotNull(realTimeStationStatusList, "GetStationRealTimeInformation() should return the real time station information we just set");
			Assert.AreEqual(1, realTimeStationStatusList.Count, "GetStationRealTimeInformation() should return the only real time station information we just set.");
			RealTimeStationStatusType realTimeStationStatus = realTimeStationStatusList[0];
			Assert.IsNotNull(realTimeStationStatus, "Make sure we have data to work with.");

			// Check Update Date
			DateTime dateTimeAfterSet = DateTime.Now;
			TimeSpan maxDelaySinceSet = new TimeSpan(0, 0, 5);
			DateTime updateDate;

			updateDate = realTimeStationStatus.StationConnectionList[0].UpdateDate;
			Assert.Less(dateTimeAfterSet.Subtract(updateDate), maxDelaySinceSet, "The 'StationConnectionList[0].UpdateDate' should correspond to the date/time of when we called SetRealTimeStationInformation() and should be smaller and very close (5 sec in this test) to the current date/time as we just set it.");
			Assert.GreaterOrEqual(dateTimeAfterSet.CompareTo(updateDate), 0, "The current date/time should be greater (or equal) to the 'StationConnectionList[0].UpdateDate' date time (the UpdateDate should correspond to the date/time of when we called SetRealTimeStationInformation()).");

			updateDate = realTimeStationStatus.StationDelay.UpdateDate;
			Assert.Less(dateTimeAfterSet.Subtract(updateDate), maxDelaySinceSet, "The 'StationDelay.UpdateDate' should correspond to the date/time of when we called SetRealTimeStationInformation() and should be smaller and very close (5 sec in this test) to the current date/time as we just set it.");
			Assert.GreaterOrEqual(dateTimeAfterSet.CompareTo(updateDate), 0, "The current date/time should be greater (or equal) to the 'StationDelay.UpdateDate' date time (the UpdateDate should correspond to the date/time of when we called SetRealTimeStationInformation()).");

			updateDate = realTimeStationStatus.StationPlatform.UpdateDate;
			Assert.Less(dateTimeAfterSet.Subtract(updateDate), maxDelaySinceSet, "The 'StationPlatform.UpdateDate' should correspond to the date/time of when we called SetRealTimeStationInformation() and should be smaller and very close (5 sec in this test) to the current date/time as we just set it.");
			Assert.GreaterOrEqual(dateTimeAfterSet.CompareTo(updateDate), 0, "The current date/time should be greater (or equal) to the 'StationPlatform.UpdateDate' date time (the UpdateDate should correspond to the date/time of when we called SetRealTimeStationInformation()).");

			updateDate = realTimeStationStatus.StationWeather.UpdateDate;
			Assert.Less(dateTimeAfterSet.Subtract(updateDate), maxDelaySinceSet, "The 'StationWeather.UpdateDate' should correspond to the date/time of when we called SetRealTimeStationInformation() and should be smaller and very close (5 sec in this test) to the current date/time as we just set it.");
			Assert.GreaterOrEqual(dateTimeAfterSet.CompareTo(updateDate), 0, "The current date/time should be greater (or equal) to the 'StationWeather.UpdateDate' date time (the UpdateDate should correspond to the date/time of when we called SetRealTimeStationInformation()).");
		}

		/// <summary>Creates dummy real time station information data list.</summary>
		/// <returns>The new dummy real time station information data list.</returns>
		private static List<RealTimeStationInformationType> CreateDummyRealTimeStationInformationList()
		{
			List<RealTimeStationInformationType> stationInformationList = new List<RealTimeStationInformationType>
			{
				new RealTimeStationInformationType()
				{
					StationCode = "Station0", 
					StationData = new RealTimeStationDataType()
					{
						StationDelay = new RealTimeDelayType() { Delay = 1, DelayReason = "Strike", DelayReasonCode = 9 },
						StationPlatform = new RealTimeStationPlatformType()
						{
							ExitSide = "Left",
							ExitSideCode = 1,
							IssueDescription = "Light Construction",
							IssueDescriptionCode = 1,
							Platform = "Platform A",
							Track = "Track 1",
							TrackCode = 1
						},
						StationConnectionList = new List<RealTimeStationConnectionType>
						{
							new RealTimeStationConnectionType ()
							{
								CommercialNumber = "AT100",
								ConnectionDelay = "10 min",
								ConnectionPlatform = "Platform B",
								DepartureFrequency = "30 min",
								DepartureTime = "08:45",
								DestinationName = "Paris, Fr",
								Line = "Main Line",
								LineCode = 1,
								ModelType = "TGV",
								ModelTypeCode = 1,
								NextDepartureTime = "09:15",
								Operator = "SNCF",
								OperatorCode = 1
							}
						},
						StationWeather = new RealTimeWeatherType()
						{
							Humidity = "30%",
							TemperatureInCentigrade = "20C",
							TemperatureInFahrenheit = "68F",
							WeatherCondition = "Cloudy",
							WeatherConditionCode = 2
						}
					}
				}
			};
			return stationInformationList;
		}

		#endregion

		#region clear

		/// <summary>Clears the real time information mission code null.</summary>
		[Test]
		public void ClearRealTimeInformationMissionCodeNull()
		{
			string missionCode = null;
			List<string> stationList = null;
			List<string> clearedStationList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.ClearRealTimeInformation(missionCode, stationList, out clearedStationList);

			Assert.IsNull(clearedStationList);
			Assert.IsFalse(WasItHit);
		}

		/// <summary>Clears the real time information mission code empty.</summary>
		[Test]
		public void ClearRealTimeInformationMissionCodeEmpty()
		{
			string missionCode = string.Empty;
			List<string> stationList = null;
			List<string> clearedStationList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.ClearRealTimeInformation(missionCode, stationList, out clearedStationList);

			Assert.IsNull(clearedStationList);
			Assert.IsFalse(WasItHit);
		}

		/// <summary>Clears the real time information mission code not found.</summary>
		[Test]
		public void ClearRealTimeInformationMissionCodeNotFound()
		{
			string missionCode = "DummyMission";
			List<string> stationList = null;
			List<string> clearedStationList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.ClearRealTimeInformation(missionCode, stationList, out clearedStationList);

			Assert.IsNull(clearedStationList);
			Assert.IsFalse(WasItHit);
		}

		/// <summary>Clears the real time information station list null.</summary>
		[Test]
		public void ClearRealTimeInformationStationListNull()
		{
			string missionCode = "DummyMission";
			List<string> stationList = null;
			List<string> clearedStationList = null;
			RealTimeDelayType delay = new RealTimeDelayType();
			RealTimeWeatherType weather = new RealTimeWeatherType();
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);
			Assert.IsNotNull(rtr);
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			_rtpisds.ClearRealTimeInformation(missionCode, stationList, out clearedStationList);
			rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			Assert.IsNull(rtr);
			Assert.IsNull(clearedStationList);
			Assert.IsTrue(WasItHit);
		}

		/// <summary>Clears the real time information station list with elements.</summary>
		[Test]
		public void ClearRealTimeInformationStationListWithElements()
		{
			string missionCode = "DummyMission";
			List<string> stationList = new List<string> {"Station0"};
			List<string> clearedStationList = null;
			RealTimeDelayType delay = new RealTimeDelayType();
			RealTimeWeatherType weather = new RealTimeWeatherType();
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);
			Assert.IsNotNull(rtr);
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			_rtpisds.ClearRealTimeInformation(missionCode, stationList, out clearedStationList);
			rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			Assert.IsNotNull(rtr);
			Assert.IsNull(clearedStationList);
			Assert.IsFalse(WasItHit);
		}

		/// <summary>Clears the real time information station list empty.</summary>
		[Test]
		public void ClearRealTimeInformationStationListEmpty()
		{
			string missionCode = "DummyMission";
			List<string> stationList = new List<string>();
			List<string> clearedStationList = null;
			RealTimeDelayType delay = new RealTimeDelayType();
			RealTimeWeatherType weather = new RealTimeWeatherType();
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);
			Assert.IsNotNull(rtr);
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			_rtpisds.ClearRealTimeInformation(missionCode, stationList, out clearedStationList);
			rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			Assert.IsNull(rtr);
			Assert.IsNull(clearedStationList);
			Assert.IsTrue(WasItHit);
		}

		/// <summary>
		/// Mission success/station list null, with station list mission success/station list empty, with
		/// station list mission success/station list with elements, with station list.
		/// </summary>
		[Test]
		public void ClearRealTimeInformationStationListNullWithStoredList()
		{
			string missionCode = "DummyMission";
			List<string> stationList = null;
			List<string> clearedStationList = null;
			List<string> clearedStationListRef = new List<string>{"Station0"};
			RealTimeDelayType delay = new RealTimeDelayType();
			RealTimeWeatherType weather = new RealTimeWeatherType();
			List<RealTimeStationInformationType> stationInformationList = new List<RealTimeStationInformationType> { new RealTimeStationInformationType() { StationCode = "Station0", StationData = new RealTimeStationDataType() } };
			List<RealTimeStationResultType> stationResultsList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList);
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);
			Assert.IsNotNull(rtr);

			_rtpisds.ClearRealTimeInformation(missionCode, stationList, out clearedStationList);
			rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			Assert.IsNull(rtr);
			Assert.IsNotNull(stationResultsList);
			Assert.AreEqual("Station0", stationResultsList[0].StationID);
			Assert.AreEqual(clearedStationListRef, clearedStationList);
			Assert.IsTrue(WasItHit);
		}

		/// <summary>
		/// Clears the real time information station list with elements with stored list.
		/// </summary>
		[Test]
		public void ClearRealTimeInformationStationListWithElementsWithStoredList()
		{
			string missionCode = "DummyMission";
			List<string> stationList = new List<string> { "Station0" };
			List<string> clearedStationList = null;
			List<string> clearedStationListRef = new List<string> { "Station0" };
			RealTimeDelayType delay = new RealTimeDelayType();
			RealTimeWeatherType weather = new RealTimeWeatherType();
			List<RealTimeStationInformationType> stationInformationList = new List<RealTimeStationInformationType> { new RealTimeStationInformationType() { StationCode = "Station0", StationData = new RealTimeStationDataType() } };
			List<RealTimeStationResultType> stationResultsList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList);
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);
			Assert.IsNotNull(rtr);
			Assert.IsNotNull(stationResultsList);
			Assert.AreEqual("Station0", stationResultsList[0].StationID);

			_rtpisds.ClearRealTimeInformation(missionCode, stationList, out clearedStationList);
			rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			Assert.IsNotNull(rtr);
			Assert.AreEqual(clearedStationListRef, clearedStationList);
			Assert.IsTrue(WasItHit);
		}

		/// <summary>Clears the real time information station list empty with stored list.</summary>
		[Test]
		public void ClearRealTimeInformationStationListEmptyWithStoredList()
		{
			string missionCode = "DummyMission";
			List<string> stationList = new List<string>();
			List<string> clearedStationList = null;
			List<string> clearedStationListRef = new List<string> { "Station0" };
			RealTimeDelayType delay = new RealTimeDelayType();
			RealTimeWeatherType weather = new RealTimeWeatherType();
			List<RealTimeStationInformationType> stationInformationList = new List<RealTimeStationInformationType> { new RealTimeStationInformationType() { StationCode = "Station0", StationData = new RealTimeStationDataType() } };
			List<RealTimeStationResultType> stationResultsList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList);
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);
			Assert.IsNotNull(rtr);
			Assert.IsNotNull(stationResultsList);
			Assert.AreEqual("Station0", stationResultsList[0].StationID);

			_rtpisds.ClearRealTimeInformation(missionCode, stationList, out clearedStationList);
			rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);

			Assert.IsNull(rtr);
			Assert.AreEqual(clearedStationListRef, clearedStationList);
			Assert.IsTrue(WasItHit);
		}

		/// <summary>
		/// Clears the real time information station list empty with bigger stored list.
		/// </summary>
		[Test]
		public void ClearRealTimeInformationStationListWithBiggerStoredList()
		{
			string missionCode = "DummyMission";
			List<string> stationList = new List<string> { "Station0" };
			List<string> clearedStationList = null;
			List<string> clearedStationListRef = new List<string> { "Station0" };
			RealTimeDelayType delay = new RealTimeDelayType();
			RealTimeWeatherType weather = new RealTimeWeatherType();
			List<RealTimeStationInformationType> stationInformationList = new List<RealTimeStationInformationType> {
				new RealTimeStationInformationType() { StationCode = "Station0", StationData = new RealTimeStationDataType() },
				new RealTimeStationInformationType() { StationCode = "Station1", StationData = new RealTimeStationDataType() }};
			List<RealTimeStationResultType> stationResultsList = null;
			bool WasItHit = false;
			_rtpisds.Changed += delegate { WasItHit = true; };

			_rtpisds.SetStationRealTimeInformation(missionCode, stationInformationList, out stationResultsList);
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			_rtpisds.SetMissionRealTimeInformation(missionCode, delay, weather);
			Assert.IsTrue(WasItHit);
			WasItHit = false;

			RealTimeInformationType rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);
			Assert.IsNotNull(rtr);
			Assert.IsNotNull(stationResultsList);
			Assert.AreEqual("Station0", stationResultsList[0].StationID);
			Assert.AreEqual(stationResultsList[1].StationID, "Station1");

			_rtpisds.ClearRealTimeInformation(missionCode, stationList, out clearedStationList);
			rtr = _rtpisds.GetMissionRealTimeInformation(missionCode);
			List<RealTimeStationStatusType> rtrstation = _rtpisds.GetStationRealTimeInformation(missionCode, null);

			Assert.IsNotNull(rtr);
			Assert.IsNotNull(rtrstation);
			Assert.AreEqual("Station1", rtrstation[0].StationID);
			Assert.AreEqual(clearedStationListRef, clearedStationList);
			Assert.IsTrue(WasItHit);
		}

		#endregion

		#endregion
	}

	/// <summary>Class to test RTPISDataStore class.</summary>
	public class RTPISDataStoreTested : RTPISDataStore
	{
		#region constructor

		/// <summary>Initializes a new instance of the RTPISDataStoreTested class.</summary>
		public RTPISDataStoreTested()
		{
			// do nothing
		}

		#endregion

		#region methods

		/// <summary>Initializes a new instance of the RTPISDataStoreTested class.</summary>
		public void ClearDataStores()
		{
			_missionData = null;
			_stationData = null;
		}

		#endregion
	}

}