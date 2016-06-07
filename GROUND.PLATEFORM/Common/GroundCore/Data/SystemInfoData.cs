//---------------------------------------------------------------------------------------------------
// <copyright file="SystemInfoData.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2013.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Linq;

namespace PIS.Ground.Core.Data
{

	/// <summary>
	/// 
	/// </summary>
	public enum CommunicationLink
	{
		/// <summary>
		/// 
		/// </summary>
		WIFI = 0,

		/// <summary>
		/// 
		/// </summary>
		_2G3G = 1,

		/// <summary>
		/// 
		/// </summary>
		NotApplicable = 2,
	}

	/// <summary>
	/// Representing system information
	/// </summary>
	public class SystemInfo : EventArgs
	{
		#region Private Variable
		/// <summary>
		/// 
		/// </summary>
		private readonly string strSystemIdField;

		/// <summary>
		/// 
		/// </summary>
		private readonly int vehiclePhysicalIdField;

		/// <summary>
		/// 
		/// </summary>
		private readonly string strMissionIdField;

		/// <summary>
		/// 
		/// </summary>
		private readonly CommunicationLink communicationLinkField;

		/// <summary>
		/// 
		/// </summary>
		private readonly uint statusField;

		/// <summary>
		/// 
		/// </summary>
		private readonly bool isOnlineField;

		/// <summary>
		/// List of Services which will be filled by calling Notification service of Notification
		/// </summary>
		private readonly ServiceInfoList objServiceList;

		/// <summary>
		/// Pis Base line information
		/// </summary>
		private readonly PisBaseline objPisBaseline;

		/// <summary>true if this object is pis baseline up to date.</summary>
		private readonly bool isPisBaselineUpToDate;

		/// <summary>
		/// Pis Software information
		/// </summary>
		private readonly PisVersion objPisVersion;

		/// <summary>
		/// Pis mission information
		/// </summary>
		private readonly PisMission objPisMission;

		#endregion

		#region Constructor

		public SystemInfo()
		{
			this.strSystemIdField = string.Empty;
			this.strMissionIdField = string.Empty;
			this.vehiclePhysicalIdField = 0;
			this.statusField = 0;
			this.isOnlineField = false;
			this.communicationLinkField = CommunicationLink.NotApplicable;

			this.objServiceList = new ServiceInfoList();
			this.objPisBaseline = new PisBaseline();
			this.objPisVersion = new PisVersion();
			this.objPisMission = new PisMission();

			this.isPisBaselineUpToDate = false;
		}

		public SystemInfo(
			string pstrSystemIdField,
			string pstrMissionFiled,
			int piVehiclePhysicalIdField,
			uint piStatus,
			bool pIsOnline,
			CommunicationLink peCommunicationLinkField,
			ServiceInfoList serviceList,
			PisBaseline pisBaseline,
			PisVersion pisVersion,
			PisMission pisMission,
			bool isPisBaselineUpToDate)
			: this()
		{
			this.strSystemIdField = pstrSystemIdField;
			this.strMissionIdField = pstrMissionFiled;
			this.vehiclePhysicalIdField = piVehiclePhysicalIdField;
			this.statusField = piStatus;
			this.isOnlineField = pIsOnline;
			this.communicationLinkField = peCommunicationLinkField;

			this.objServiceList = serviceList;
			this.objPisBaseline = pisBaseline;
			this.objPisVersion = pisVersion;
			this.objPisMission = pisMission;

			this.isPisBaselineUpToDate = isPisBaselineUpToDate;
		}

		public SystemInfo(SystemInfo other)
			: this()
		{
			if (other != null)
			{
				// copy string references (strings are immutable)
				this.strSystemIdField = other.strSystemIdField;
				this.strMissionIdField = other.strMissionIdField;
				this.vehiclePhysicalIdField = other.vehiclePhysicalIdField;
				this.statusField = other.statusField;
				this.isOnlineField = other.isOnlineField;
				this.communicationLinkField = other.communicationLinkField;

				// shallow copy of list (service infos are immutable)
				this.objServiceList = new ServiceInfoList(other.objServiceList);

				// deep copy
				this.objPisBaseline = new PisBaseline(other.objPisBaseline);
				this.objPisVersion = new PisVersion(other.objPisVersion);
				this.objPisMission = new PisMission(other.objPisMission);

				this.isPisBaselineUpToDate = other.isPisBaselineUpToDate;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// PIS Baseline
		/// </summary>
		public PisBaseline PisBaseline
		{
			get
			{
				return this.objPisBaseline;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this object has valid information for pis baseline or not.
		/// </summary>
		/// <value>true if this object is pis baseline up to date, false if not.</value>
		public bool IsPisBaselineUpToDate
		{
			get
			{
				return this.isPisBaselineUpToDate;
			}
		}

		/// <summary>
		/// PIS Version
		/// </summary>
		public PisVersion PisVersion
		{
			get
			{
				return this.objPisVersion;
			}
		}

		/// <summary>
		/// PIS Mission
		/// </summary>
		public PisMission PisMission
		{
			get
			{
				return this.objPisMission;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int VehiclePhysicalId
		{
			get
			{
				return this.vehiclePhysicalIdField;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string SystemId
		{
			get
			{
				return this.strSystemIdField;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string MissionId
		{
			get
			{
				return this.strMissionIdField;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public CommunicationLink CommunicationLink
		{
			get
			{
				return this.communicationLinkField;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public uint Status
		{
			get
			{
				return this.statusField;
			}
		}

		/// <summary>Gets a value indicating whether the system of this object is online.</summary>
		/// <value>true if this described system is online, false if not.</value>
		public bool IsOnline
		{
			get
			{
				return this.isOnlineField;
			}
		}

		/// <summary>
		/// ServiceList property
		/// </summary>
		public ServiceInfoList ServiceList
		{
			get
			{
				return this.objServiceList;
			}
		}

		#endregion

		public override bool Equals(Object obj)
		{
			SystemInfo lOther = obj as SystemInfo;

			// If parameter is null return false:
			if (lOther == null)
			{
				return false;
			}

			if (this.VehiclePhysicalId != lOther.VehiclePhysicalId) { return false; }
			if (this.CommunicationLink != lOther.CommunicationLink) { return false; }
			if (this.Status != lOther.Status) { return false; }
			if (this.IsOnline != lOther.IsOnline) { return false; }
			if (this.SystemId != lOther.SystemId) { return false; }
			if (this.MissionId != lOther.MissionId) { return false; }
			if (this.IsPisBaselineUpToDate != lOther.IsPisBaselineUpToDate) { return false; }
			if (!this.ServiceList.SequenceEqual(lOther.ServiceList)) { return false; }
			if (!this.PisBaseline.Equals(lOther.PisBaseline)) { return false; }
			if (!this.PisVersion.Equals(lOther.PisVersion)) { return false; }
			if (!this.PisMission.Equals(lOther.PisMission)) { return false; }

			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
