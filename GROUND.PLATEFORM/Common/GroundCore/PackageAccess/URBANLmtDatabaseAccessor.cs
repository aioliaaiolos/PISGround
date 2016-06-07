//---------------------------------------------------------------------------------------------------
// <copyright file="URBANLmtDatabaseAccessor.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using System.Globalization;

namespace PIS.Ground.Core.PackageAccess
{
	/// <summary>Urban lmt database accessor.</summary>
	public class URBANLmtDatabaseAccessor : SpecificLmtDatabaseAccessor
	{
		#region attributes

		// Flag: Has Dispose already been called?
		private bool _disposed = false;

		#endregion

		#region Con/Destructor

		/// <summary>Initializes a new instance of the URBANLmtDatabaseAccessor class.</summary>
		/// <param name="databasePath">Full pathname of the database file.</param>
		public URBANLmtDatabaseAccessor(string databasePath)
			: base(databasePath)
		{
			QuerySelectStationFromCode = DatabaseQueries.URBAN_SELECT_STATION_FROM_CODE_UIC;
			QuerySelectMissionFromOperatorCode = DatabaseQueries.URBAN_SELECT_MISSION_FROM_OPERATEUR_ID;
			QuerySelectLanguageFromLanguageId = DatabaseQueries.URBAN_SELECT_LANGUE_FROM_LANGUAGE_ID;
			QuerySelectServiceFromServiceId = DatabaseQueries.URBAN_SELECT_SERVICE_FROM_SERVICE_ID;
			QuerySelectRegionFromRegionId = DatabaseQueries.URBAN_SELECT_REGION_FROM_REGION_ID;
			QuerySelectStationCodeFromStationId = DatabaseQueries.URBAN_SELECT_STATION_FROM_ID;
			QuerySelectAllLanguages = DatabaseQueries.URBAN_GET_ALL_LANGUAGES;
			QuerySelectMissionRouteFromId = DatabaseQueries.URBAN_SELECT_MISSION_ROUTE_FROM_ID;
			QuerySelectStationWithCode = DatabaseQueries.URBAN_SELECT_STATIONS_WITH_CODE;
			QuerySelectStationNames = DatabaseQueries.URBAN_SELECT_STATIONS_NAMES;
		}

		/// <summary>Finalizes an instance of the URBANLmtDatabaseAccessor class.</summary>
		~URBANLmtDatabaseAccessor()
		{
			Dispose(false);
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
		/// resources.
		/// </summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="disposing">True to release both managed and unmanaged resources; false to
		/// release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				// Free any other managed objects here.
			}

			// Free any unmanaged objects here.
			_disposed = true;

			// Call base class implementation.
			base.Dispose(disposing);
		}

		#endregion

		#region Override

		/// <summary>Gets station list.</summary>
		/// <param name="result">The list of stations in LMT Database.</param>
		public override void GetStationList(out List<Station> result)
		{
			result = new List<Station>();

			lock (Database)
			{
				try
				{
                    using (DataTable table = new DataTable())
                    {
                        table.Locale = CultureInfo.InvariantCulture;
                        Database.mExecuteQuery(QuerySelectStationWithCode, table);

                        List<string> stationToDicoIdMap = new List<string>();
                        for (int i = 0; i < table.Rows.Count; ++i)
                        {
                            var station = new Station();
                            station.OperatorCode = (string)table.Rows[i].ItemArray[0];
                            result.Add(station);
                        }
                    }
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.PackageAccess.URBANLmtDatabaseAccessor.GetStationList", ex, EventIdEnum.GroundCore);
				}
			}
		}

		/// <summary>Service codes exist.</summary>
		/// <exception cref="NotImplementedException">Thrown as the requested operation is unimplemented.</exception>
		/// <param name="onboardServiceCodeList">List of onboard service codes.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public override bool ServiceCodesExist(List<uint> onboardServiceCodeList)
		{
			// Not needed nor working with URBAN LMT database.
			throw new NotImplementedException();
		}

		/// <summary>Region code exist.</summary>
		/// <exception cref="NotImplementedException">Thrown as the requested operation is unimplemented.</exception>
		/// <param name="regionCode">The region code.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public override bool RegionCodeExist(uint regionCode)
		{
			// Not needed nor working with URBAN LMT database.
			throw new NotImplementedException();
		}

		#endregion
	}
}