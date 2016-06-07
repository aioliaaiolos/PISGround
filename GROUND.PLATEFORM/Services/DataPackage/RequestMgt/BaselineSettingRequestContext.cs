//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineSettingRequestContext.cs" company="Alstom">
//          (c) Copyright ALSTOM 2015.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PIS.Ground.Core.Data;
using PIS.Ground.DataPackage;

namespace PIS.Ground.DataPackage.RequestMgt
{
	/// <summary>
	/// Implementation of the request context specific to the setting of the baseline on the embedded.
	/// </summary>
	public class BaselineSettingRequestContext : RequestContext
	{
		/// <summary>The baseline version.</summary>
		private string _baselineVersion;

		/// <summary>The pis base package version.</summary>
		private string _pisBasePackageVersion;

		/// <summary>The pis mission package version.</summary>
		private string _pisMissionPackageVersion;

		/// <summary>The pis infotainment package version.</summary>
		private string _pisInfotainmentPackageVersion;

		/// <summary>The lmt package version.</summary>
		private string _lmtPackageVersion;

		/// <summary>Initializes a new instance of the BaselineSettingRequestContext class.</summary>
		/// <param name="endpoint">The endpoint.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="timeout">The timeout.</param>
		/// <param name="baselineVersion">The baseline version.</param>
		/// <param name="PISBaseVersion">The pis base version.</param>
		/// <param name="PISMissionVersion">The pis mission version.</param>
		/// <param name="PISInfotainmentVersion">The pis infotainment version.</param>
		/// <param name="LmtVersion">The lmt version.</param>
		public BaselineSettingRequestContext(
			string endpoint, 
			string elementId, 
			Guid requestId, 
			Guid sessionId, 
			uint timeout, 
			string baselineVersion,
			string PISBaseVersion,
			string PISMissionVersion,
			string PISInfotainmentVersion,
			string LmtVersion)
			: base(endpoint, elementId, requestId, sessionId, timeout)
		{
			_baselineVersion = baselineVersion;
			_pisBasePackageVersion = PISBaseVersion;
			_pisMissionPackageVersion = PISMissionVersion;
			_pisInfotainmentPackageVersion = PISInfotainmentVersion;
			_lmtPackageVersion = LmtVersion;
		}

		/// <summary>Gets the baseline version.</summary>
		/// <value>The baseline version.</value>
		public string BaselineVersion
		{
			get
			{
				return _baselineVersion;
			}
		}

		/// <summary>Gets the pis base package version.</summary>
		/// <value>The pis base package version.</value>
		public string PISBasePackageVersion
		{
			get
			{
				return _pisBasePackageVersion;
			}
		}

		/// <summary>Gets the pis mission package version.</summary>
		/// <value>The pis mission package version.</value>
		public string PISMissionPackageVersion
		{
			get
			{
				return _pisMissionPackageVersion;
			}
		}

		/// <summary>Gets the pis infotainment package version.</summary>
		/// <value>The pis infotainment package version.</value>
		public string PISInfotainmentPackageVersion
		{
			get
			{
				return _pisInfotainmentPackageVersion;
			}
		}

		/// <summary>Gets the lmt package version.</summary>
		/// <value>The lmt package version.</value>
		public string LmtPackageVersion
		{
			get
			{
				return _lmtPackageVersion;
			}
		}
	}
}