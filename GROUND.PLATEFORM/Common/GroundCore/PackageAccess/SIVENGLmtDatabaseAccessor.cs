//---------------------------------------------------------------------------------------------------
// <copyright file="SIVENGLmtDatabaseAccessor.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIS.Ground.Core.PackageAccess
{
	/// <summary>Siveng lmt database accessor.</summary>
	public class SIVENGLmtDatabaseAccessor : SpecificLmtDatabaseAccessor, IDisposable
	{
		#region attributes

		// Flag: Has Dispose already been called.
		private bool _disposed = false;

		#endregion

		#region Con/Destructor

		/// <summary>Initializes a new instance of the SIVENGLmtDatabaseAccessor class.</summary>
		/// <param name="databasePath">Full pathname of the database file.</param>
		public SIVENGLmtDatabaseAccessor(string databasePath)
			: base(databasePath)
		{
			QuerySelectStationFromCode = DatabaseQueries.SIVENG_SELECT_STATION_FROM_CODE_UIC;
			QuerySelectMissionFromOperatorCode = DatabaseQueries.SIVENG_SELECT_MISSION_FROM_OPERATEUR_ID;
			QuerySelectLanguageFromLanguageId = DatabaseQueries.SIVENG_SELECT_LANGUE_FROM_LANGUAGE_ID;
			QuerySelectServiceFromServiceId = DatabaseQueries.SIVENG_SELECT_SERVICE_FROM_SERVICE_ID;
			QuerySelectRegionFromRegionId = DatabaseQueries.SIVENG_SELECT_REGION_FROM_REGION_ID;
			QuerySelectStationCodeFromStationId = DatabaseQueries.SIVENG_SELECT_STATION_FROM_ID;
			QuerySelectAllLanguages = DatabaseQueries.SIVENG_GET_ALL_LANGUAGES;
			QuerySelectMissionRouteFromId = DatabaseQueries.SIVENG_SELECT_MISSION_ROUTE_FROM_ID;
			QuerySelectStationWithCode = DatabaseQueries.SIVENG_SELECT_STATIONS_WITH_CODE;
			QuerySelectStationNames = DatabaseQueries.SIVENG_SELECT_STATIONS_NAMES;
		}

		/// <summary>Finalizes an instance of the SIVENGLmtDatabaseAccessor class.</summary>
		~SIVENGLmtDatabaseAccessor()
		{
			Dispose(false);
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
		/// resources.
		/// </summary>
		/// <param name="disposing">True if we want to dispose all ressources, false otherwise.</param>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
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
	}
}