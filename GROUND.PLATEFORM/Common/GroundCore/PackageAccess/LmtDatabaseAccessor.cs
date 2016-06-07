//---------------------------------------------------------------------------------------------------
// <copyright file="LmtDatabaseAccessor.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2014.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace PIS.Ground.Core.PackageAccess
{
	/// <summary>Interface between DataPackage WebService and DataBase.</summary>
	public sealed class LmtDatabaseAccessor : IDisposable
	{
		#region Attributes

		/// <summary>The instance of SQLLite Wrapper to do query to database.</summary>
		private SpecificLmtDatabaseAccessor _accessor;
		
		/// <summary>Flag: Has Dispose already been called.</summary>
		private bool _disposed = false;

		#endregion

		#region Constructors

		/// <summary>Initializes a new instance of the LmtDatabaseAccessor class.</summary>
		/// <param name="databasePath">Full pathname of the database file.</param>
		public LmtDatabaseAccessor(string databasePath) : this(databasePath, null)
		{
		}

		/// <summary>Initializes a new instance of the LmtDatabaseAccessor class.</summary>
		/// <param name="databasePath">Full pathname of the database file.</param>
		/// <param name="currentConfig">If not null, the current config type, else, will be obtain from common configuration.</param>
		public LmtDatabaseAccessor(string databasePath, CommonConfiguration.PlatformTypeEnum? currentConfig)
		{
			CommonConfiguration.PlatformTypeEnum workingConfig = currentConfig ?? CommonConfiguration.PlatformType;

			if (workingConfig == CommonConfiguration.PlatformTypeEnum.URBAN)
			{
				_accessor = new URBANLmtDatabaseAccessor(databasePath);
			}
			else if (workingConfig == CommonConfiguration.PlatformTypeEnum.SIVENG)
			{
				_accessor = new SIVENGLmtDatabaseAccessor(databasePath);
			}
			else
			{
				_accessor = null;

				// Do not log, do not throw as error handling is made in CommonConfiguration class
			}
		}

		#endregion

		#region Methods

		/// <summary>Queries if a given station exists.</summary>
		/// <param name="stationOperatorCode">The station operator code.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public bool StationExists(string stationOperatorCode)
		{
			bool stationExists = false;
			if (_accessor != null)
			{
				stationExists = _accessor.StationExists(stationOperatorCode);
			}

			return stationExists;
		}

		/// <summary>Gets mission internal code from operator code.</summary>
		/// <param name="operatorCode">The operator code.</param>
		/// <returns>The mission internal code from operator code.</returns>
		public uint? GetMissionInternalCodeFromOperatorCode(string operatorCode)
		{
			uint? missionInternalCode = null;
			
			if (_accessor != null)
			{
				missionInternalCode = _accessor.GetMissionInternalCodeFromOperatorCode(operatorCode);
			}

			return missionInternalCode;
		}

		/// <summary>Language codes exist.</summary>
		/// <param name="languageCodeList">List of language codes.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public bool LanguageCodesExist(List<string> languageCodeList)
		{
			bool languageCodesExist = false;
			if (_accessor != null)
			{
				languageCodesExist = _accessor.LanguageCodesExist(languageCodeList);
			}

			return languageCodesExist;
		}

		/// <summary>Service codes exist.</summary>
		/// <param name="onboardServiceCodeList">List of onboard service codes.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public bool ServiceCodesExist(List<uint> onboardServiceCodeList)
		{
			bool serviceCodesExist = false;
			if (_accessor != null)
			{
				serviceCodesExist = _accessor.ServiceCodesExist(onboardServiceCodeList);
			}

			return serviceCodesExist;
		}

		/// <summary>Region code exist.</summary>
		/// <param name="regionCode">The region code.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public bool RegionCodeExist(uint regionCode)
		{
			bool regionCodeExist = false;

			if (_accessor != null)
			{
				regionCodeExist = _accessor.RegionCodeExist(regionCode);
			}

			return regionCodeExist;
		}

		/// <summary>Gets station operator code from internal code.</summary>
		/// <param name="stationInternalCode">The station internal code.</param>
		/// <returns>The station operator code from internal code.</returns>
		public string GetStationOperatorCodeFromInternalCode(uint stationInternalCode)
		{
			string result = string.Empty;

			if (_accessor != null)
			{
				result = _accessor.GetStationOperatorCodeFromInternalCode(stationInternalCode);
			}

			return result;
		}

		/// <summary>Gets station internal code from operator code.</summary>
		/// <param name="stationOperatorCode">The station operator code.</param>
		/// <returns>The station internal code from operator code.</returns>
		public uint? GetStationInternalCodeFromOperatorCode(string stationOperatorCode)
		{
			uint? result = null;

			if (_accessor != null)
			{
				result = _accessor.GetStationInternalCodeFromOperatorCode(stationOperatorCode);
			}

			return result;
		}

		/// <summary>Gets all languages.</summary>
		/// <returns>All languages from database.</returns>
		public List<string> GetAllLanguages()
		{
			List<string> result = new List<string>();

			if (_accessor != null)
			{
				_accessor.GetAllLanguages(out result);
			}

			return result;
		}

		/// <summary>Gets mission route.</summary>
		/// <param name="missionId">Identifier for the mission.</param>
		/// <returns>The mission route.</returns>
		public List<uint> GetMissionRoute(uint missionId)
		{
			List<uint> missionRoute = new List<uint>();

			if (_accessor != null)
			{
				_accessor.GetMissionRoute(missionId, out missionRoute);
			}

			return missionRoute;
		}

		/// <summary>Gets station list.</summary>
		/// <returns>The station list.</returns>
		public List<Station> GetStationList()
		{
			List<Station> result = new List<Station>();

			if (_accessor != null)
			{
				_accessor.GetStationList(out result);
			}

			return result;
		}

		#endregion

		#region  IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
		/// resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Protected implementation of Dispose pattern.</summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		private void Dispose(bool disposing)
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
			if (_accessor != null)
			{
				_accessor.Dispose();
			}

			_disposed = true;
		}

		#endregion
	}
}