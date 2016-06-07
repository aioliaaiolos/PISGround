//---------------------------------------------------------------------------------------------------
// <copyright file="SpecificLmtDatabaseAccessor.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.SQLite;
using System.Globalization;

namespace PIS.Ground.Core.PackageAccess
{
	/// <summary>Interface for specific lmt database accessor.</summary>
	public abstract class SpecificLmtDatabaseAccessor : IDisposable
	{
		#region attributes
		
		// Flag: Has Dispose already been called.
		private bool _disposed = false;
		
		/// <summary>Gets the instance of SQLLite Wrapper to do query to database.</summary>
		protected SQLiteWrapperClass Database { get; private set; }

		/// <summary>Gets or sets the SQL query for getting station from code.</summary>
		/// <value>The query select station from code.</value>
		protected string QuerySelectStationFromCode { get; set; }

		/// <summary>Gets or sets the SQL query for getting mission from operator code.</summary>
		/// <value>The query select mission from operateur code.</value>
		protected string QuerySelectMissionFromOperatorCode { get; set; }

		/// <summary>Gets or sets the SQL query for getting language from language identifier.</summary>
		/// <value>The query select mission from operateur code.</value>
		protected string QuerySelectLanguageFromLanguageId { get; set; }

		/// <summary>Gets or sets the SQL query for getting service from service identifier.</summary>
		/// <value>The identifier of the query select service from service.</value>
		protected string QuerySelectServiceFromServiceId { get; set; }

		/// <summary>Gets or sets the SQL query for getting region from region identifier.</summary>
		/// <value>The identifier of the query select region from region.</value>
		protected string QuerySelectRegionFromRegionId { get; set; }

		/// <summary>
		/// Gets or sets the SQL query for getting station code from station identifier.
		/// </summary>
		/// <value>The identifier of the query select station code from station.</value>
		protected string QuerySelectStationCodeFromStationId { get; set; }

		/// <summary>Gets or sets the SQL query for getting all languages.</summary>
		/// <value>The query select all languages.</value>
		protected string QuerySelectAllLanguages { get; set; }

		/// <summary>Gets or sets the SQL query for getting mission route from mission identifier.</summary>
		/// <value>The identifier of the query select mission route from.</value>
		protected string QuerySelectMissionRouteFromId { get; set; }

		/// <summary>Gets or sets the SQL query for getting station code from station identifier.</summary>
		/// <value>The query select station with code.</value>
		protected string QuerySelectStationWithCode { get; set; }

		/// <summary>Gets or sets the SQL query for getting list of all stations.</summary>
		/// <value>A list of names of the query select stations.</value>
		protected string QuerySelectStationNames { get; set; }

		#endregion

		#region Con/Destructor

		/// <summary>Initializes a new instance of the SpecificLmtDatabaseAccessor class.</summary>
		/// <param name="databasePath">Full pathname of the database file.</param>
		protected SpecificLmtDatabaseAccessor(string databasePath)
		{
			Database = new SQLiteWrapperClass(databasePath);

			QuerySelectStationFromCode = string.Empty;
			QuerySelectMissionFromOperatorCode = string.Empty;
			QuerySelectLanguageFromLanguageId = string.Empty;
			QuerySelectServiceFromServiceId = string.Empty;
			QuerySelectRegionFromRegionId = string.Empty;
			QuerySelectStationCodeFromStationId = string.Empty;
			QuerySelectAllLanguages = string.Empty;
			QuerySelectMissionRouteFromId = string.Empty;
			QuerySelectStationWithCode = string.Empty;
			QuerySelectStationNames = string.Empty;
		}

		~SpecificLmtDatabaseAccessor()
		{
			Dispose(false);
		}

		#endregion

		#region database access

		/// <summary>Queries if a given station exists.</summary>
		/// <param name="stationOperatorCode">The station operator code.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public bool StationExists(string stationOperatorCode)
		{
			bool stationExists = false;

			lock (Database)
			{
				try
				{
                    using (DataTable table = new DataTable())
                    {
                        table.Locale = CultureInfo.InvariantCulture;
                        Database.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, QuerySelectStationFromCode, stationOperatorCode), table);
                        if (table.Rows.Count > 0)
                        {
                            stationExists = true;
                        }
                    }
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.PackageAccess.SpecificLmtDatabaseAccessor.StationExists", ex, EventIdEnum.GroundCore);
				}
			}

			return stationExists;
		}

		/// <summary>Gets mission internal code from operator code.</summary>
		/// <param name="operatorCode">The operator code.</param>
		/// <returns>The mission internal code from operator code.</returns>
		public uint? GetMissionInternalCodeFromOperatorCode(string operatorCode)
		{
			uint? result = null;

			lock (Database)
			{
				try
				{
                    using (DataTable table = new DataTable())
                    {
                        table.Locale = CultureInfo.InvariantCulture;
                        Database.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, QuerySelectMissionFromOperatorCode, operatorCode), table);
                        foreach (DataRow row in table.Rows)
                        {
                            result = (uint)((Int64)row.ItemArray[0]);
                        }
                    }
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.PackageAccess.SpecificLmtDatabaseAccessor.GetMissionIdByOperatorCode", ex, EventIdEnum.GroundCore);
				}
			}

			return result;
		}

		/// <summary>Language codes exist.</summary>
		/// <param name="languageCodeList">List of language codes.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public bool LanguageCodesExist(List<string> languageCodeList)
		{
			bool allLanguageCodesExist = false;

			lock (Database)
			{
				try
				{
                    using (DataTable table = new DataTable())
                    {
                        table.Locale = CultureInfo.InvariantCulture;
                        string languages = "'" + string.Join("','", languageCodeList.ToArray()) + "'";
                        Database.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, QuerySelectLanguageFromLanguageId, languages), table);
                        if (table.Rows.Count == languageCodeList.Count)
                        {
                            allLanguageCodesExist = true;
                        }
                    }
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.PackageAccess.SpecificLmtDatabaseAccessor.LanguageCodesExist", ex, EventIdEnum.GroundCore);
				}
			}

			return allLanguageCodesExist;
		}

		/// <summary>Service codes exist.</summary>
		/// <param name="onboardServiceCodeList">List of onboard service codes.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public virtual bool ServiceCodesExist(List<uint> onboardServiceCodeList)
		{
			{
				bool allServiceCodesExist = false;

				lock (Database)
				{
					try
					{
                        using (DataTable table = new DataTable())
                        {
                            table.Locale = CultureInfo.InvariantCulture;
                            List<string> onboardServiceCodeAsStringList = onboardServiceCodeList.ConvertAll<string>(delegate(uint i) { return i.ToString(); });
                            string services = string.Join(",", onboardServiceCodeAsStringList.ToArray());
                            Database.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, QuerySelectServiceFromServiceId, services), table);
                            if (table.Rows.Count == onboardServiceCodeList.Count)
                            {
                                allServiceCodesExist = true;
                            }
                        }
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.PackageAccess.SpecificLmtDatabaseAccessor.ServiceCodesExist", ex, EventIdEnum.GroundCore);
					}
				}

				return allServiceCodesExist;
			}
		}

		/// <summary>Region code exist.</summary>
		/// <param name="regionCode">The region code.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public virtual bool RegionCodeExist(uint regionCode)
		{
			bool regionCodeExist = false;

			lock (Database)
			{
				try
				{
                    using (DataTable table = new DataTable())
                    {
                        table.Locale = CultureInfo.InvariantCulture;
                        string regionCodeString = regionCode.ToString();

                        string query = String.Format(CultureInfo.InvariantCulture, QuerySelectRegionFromRegionId, regionCode);
                        LogManager.WriteLog(TraceType.INFO, query, "PIS.Ground.Core.PackageAccess.LmtDatabaseAccessor.RegionCodeExist", null, EventIdEnum.GroundCore);
                        Database.mExecuteQuery(query, table);
                        if (table.Rows.Count == 1)
                        {
                            regionCodeExist = true;
                        }
                    }
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.PackageAccess.SpecificLmtDatabaseAccessor.RegionCodeExist", ex, EventIdEnum.GroundCore);
				}
			}

			return regionCodeExist;
		}

		/// <summary>Gets station operator code from internal code.</summary>
		/// <param name="stationInternalCode">The station internal code.</param>
		/// <returns>The station operator code from internal code.</returns>
		public string GetStationOperatorCodeFromInternalCode(uint stationInternalCode)
		{
			string result = string.Empty;

            using (DataTable table = new DataTable())
            {
                table.Locale = CultureInfo.InvariantCulture;
                try
                {
                    lock (Database)
                    {
                        Database.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, QuerySelectStationCodeFromStationId, stationInternalCode), table);
                        if (table.Rows.Count == 1)
                        {
                            result = (string)table.Rows[0].ItemArray[0];
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.PackageAccess.SpecificLmtDatabaseAccessor.GetStationOperatorCodeFromInternalCode", ex, EventIdEnum.GroundCore);
                }
            }

			return result;
		}

		/// <summary>Gets station internal code from operator code.</summary>
		/// <param name="stationOperatorCode">The station operator code.</param>
		/// <returns>The station internal code from operator code.</returns>
		public uint? GetStationInternalCodeFromOperatorCode(string stationOperatorCode)
		{
			uint? result = null;

            using (DataTable table = new DataTable())
            {
                table.Locale = CultureInfo.InvariantCulture;
                try
                {
                    lock (Database)
                    {
                        if (!string.IsNullOrEmpty(stationOperatorCode))
                        {
                            Database.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, QuerySelectStationFromCode, stationOperatorCode), table);
                            if (table.Rows.Count == 1)
                            {
                                result = (uint)((Int64)table.Rows[0].ItemArray[0]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.PackageAccess.SpecificLmtDatabaseAccessor.GetStationInternalCodeFromOperatorCode", ex, EventIdEnum.GroundCore);
                }
            }

			return result;
		}

		/// <summary>Gets all languages.</summary>
		/// <param name="result">[In,out]The result.</param>
		public void GetAllLanguages(out List<string> result)
		{
			result = new List<string>();

            using (DataTable table = new DataTable())
            {
                table.Locale = CultureInfo.InvariantCulture;
                try
                {
                    lock (Database)
                    {
                        Database.mExecuteQuery(QuerySelectAllLanguages, table);
                        for (int i = 0; i < table.Rows.Count; ++i)
                        {
                            result.Add((string)table.Rows[i].ItemArray[0]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.PackageAccess.SpecificLmtDatabaseAccessor.GetAllLanguages", ex, EventIdEnum.GroundCore);
                }
            }
		}

		/// <summary>Gets mission route.</summary>
		/// <param name="missionId">Identifier for the mission.</param>
		/// <param name="missionRoute">[Out] The mission route as list of station id.</param>
		public void GetMissionRoute(uint missionId, out List<uint> missionRoute)
		{
			missionRoute = new List<uint>();

            using (DataTable table = new DataTable())
            {
                table.Locale = CultureInfo.InvariantCulture;
                try
                {
                    lock (Database)
                    {
                        Database.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, QuerySelectMissionRouteFromId, missionId), table);

                        // Query returns a list of tuple <station origine, station destination>.
                        // We will construct list of covered stations id from these tuples.
                        for (int i = 0; i < table.Rows.Count; ++i)
                        {
                            // use the origin station id of the first segment only, and
                            // use the destination station id of all segments to build the route
                            if (i == 0)
                            {
                                missionRoute.Add((uint)((Int64)table.Rows[i].ItemArray[0]));
                            }

                            missionRoute.Add((uint)((Int64)table.Rows[i].ItemArray[1]));
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.PackageAccess.SpecificLmtDatabaseAccessor.GetMissionRoute", ex, EventIdEnum.GroundCore);
                }
            }
		}

		/// <summary>Gets station list.</summary>
		/// <param name="result">[Out] The list of stations in LMT database.</param>
        public virtual void GetStationList(out List<Station> result)
        {
            result = new List<Station>();

            using (DataTable table1 = new DataTable())
            using (DataTable table2 = new DataTable())
            {
                table1.Locale = CultureInfo.InvariantCulture;
                table2.Locale = CultureInfo.InvariantCulture;
                try
                {
                    lock (Database)
                    {
                        /* Since the query below does not work for some unknown reason with System.Data.SQLite, we need to
                         * combine the results of two queries manually, in memory.
                         * 
                         * SELECT 
                         * GARE_CODE_UIC, ID_LANGUE, LIBELLE 
                         * FROM TBL_GARE JOIN TBL_DICO 
                         * ON (TBL_DICO.ID_DICO = TBL_GARE.ID_DICO_NOM_LONG) 
                         * WHERE TBL_GARE.GARE_ARRET = 1 AND TBL_GARE.GARE_CODE_UIC IS NOT NULL                    
                         * GROUP BY GARE_CODE_UIC, ID_LANGUE
                         */


                        DataTable table = table1;
                        Database.mExecuteQuery(QuerySelectStationWithCode, table);

                        Dictionary<string, Int64> stationToDicoIdMap = new Dictionary<string, Int64>();
                        for (int i = 0; i < table.Rows.Count; ++i)
                        {
                            string stationCode = (string)table.Rows[i].ItemArray[0];

                            if (!stationToDicoIdMap.ContainsKey(stationCode))
                            {
                                stationToDicoIdMap[stationCode] = (Int64)table.Rows[i].ItemArray[1];
                            }
                        }

                        table = table2;

                        Database.mExecuteQuery(QuerySelectStationNames, table);

                        Dictionary<Int64, List<StationName>> dico = new Dictionary<Int64, List<StationName>>();

                        for (int i = 0; i < table.Rows.Count; ++i)
                        {
                            Int64 id = (Int64)table.Rows[i].ItemArray[0];
                            string language = (string)table.Rows[i].ItemArray[1];
                            string name = (string)table.Rows[i].ItemArray[2];

                            if (!dico.ContainsKey(id))
                            {
                                dico[id] = new List<StationName>();
                            }

                            StationName stationName = new StationName();
                            stationName.Language = language;
                            stationName.Name = name;
                            dico[id].Add(stationName);
                        }

                        foreach (KeyValuePair<string, Int64> entry in stationToDicoIdMap)
                        {
                            if (dico.ContainsKey(entry.Value))
                            {
                                var station = new Station();
                                station.OperatorCode = entry.Key;
                                station.Names = dico[entry.Value];
                                result.Add(station);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.PackageAccess.SpecificLmtDatabaseAccessor.GetStationList", ex, EventIdEnum.GroundCore);
                }
            }
        }

		#endregion

		#region tools

		/// <summary>
		/// Helper static function that serializes a provided list of <see cref="LmtDatabaseStation"/>
		/// objects to an XML-formatted (UTF-8 encoding) string.
		/// </summary>		
		/// <param name="obj">The list of <see cref="LmtDatabaseStation"/> objects to serialize.</param>
		/// <returns>the XML string</returns>
		public static string SerializeStationList(List<Station> obj)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<Station>), new XmlRootAttribute("StationList"));
			using (Utf8StringWriter writer = new Utf8StringWriter())
			{
				serializer.Serialize(writer, obj);
				return writer.ToString();
			}
		}

		/// <summary>
		/// Helper private class to use UTF-8 encoding with StringWriter.
		/// </summary>				
		private class Utf8StringWriter : StringWriter
		{
			/// <summary>
			/// Gets the <see cref="T:System.Text.Encoding" /> in which the output is written.
			/// </summary>
			/// <value>The Encoding in which the output is written.</value>
			public override Encoding Encoding
			{
				get { return Encoding.UTF8; }
			}
		}

		#endregion

		#region IDisposable Members

		/// <summary>Public implementation of Dispose pattern callable by consumers.</summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Protected implementation of Dispose pattern.</summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; false to
		/// release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
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
            if (Database != null)
            {
                lock (Database)
                {
                    Database.Dispose();
                }
            }

			_disposed = true;
		}

		#endregion
	}
}