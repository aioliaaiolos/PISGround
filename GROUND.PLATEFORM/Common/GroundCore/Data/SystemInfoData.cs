//---------------------------------------------------------------------------------------------------
// <copyright file="SystemInfoData.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Globalization;
using System.Text;

namespace PIS.Ground.Core.Data
{

	/// <summary>
	/// Define the communication link of a system
	/// </summary>
	public enum CommunicationLink
	{
		/// <summary>
		/// Wifi
		/// </summary>
		WIFI = 0,

		/// <summary>
		/// GPRS 
		/// </summary>
		_2G3G = 1,

		/// <summary>
		/// Unknown
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
		/// Name of the described system.
		/// </summary>
		private readonly string _systemId;

		/// <summary>
		/// Unique vehicle physical identifier.
		/// </summary>
		private readonly ushort _vehiclePhysicalId;

		/// <summary>
		/// Current mission identifier.
		/// </summary>
		private readonly string _missionId;

		/// <summary>
		/// Communication link between the embedded system and T2G ground server.
		/// </summary>
		private readonly CommunicationLink _communicationLink;

		/// <summary>
		/// Status of the system as provided by T2G. It's a bit field.
		/// </summary>
		private readonly uint _status;

		/// <summary>
		/// Indicates if the embedded system communicate with the T2G ground server.
		/// </summary>
		private readonly bool _isOnline;

		/// <summary>
		/// List of Services which will be filled by calling Notification service of Notification
		/// </summary>
		private readonly ServiceInfoList _serviceList;

		/// <summary>
		/// Pis Base line information
		/// </summary>
		private readonly PisBaseline _pisBaseline;

		/// <summary>true if this object is pis baseline up to date.</summary>
		private readonly bool _isPisBaselineUpToDate;

		/// <summary>
		/// Pis Software information
		/// </summary>
		private readonly PisVersion _pisVersion;

		/// <summary>
		/// Pis mission information
		/// </summary>
		private readonly PisMission _pisMission;

		#endregion

		#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemInfo"/> class.
        /// </summary>
		public SystemInfo()
		{
			this._systemId = string.Empty;
			this._missionId = string.Empty;
			this._vehiclePhysicalId = 0;
			this._status = 0;
			this._isOnline = false;
			this._communicationLink = CommunicationLink.NotApplicable;

			this._serviceList = new ServiceInfoList();
			this._pisBaseline = new PisBaseline();
			this._pisVersion = new PisVersion();
			this._pisMission = new PisMission();

			this._isPisBaselineUpToDate = false;
		}

		public SystemInfo(
			string systemId,
			string missionId,
			ushort vehiclePhysicalIdField,
			uint status,
			bool isOnline,
			CommunicationLink communicationLink,
			ServiceInfoList serviceList,
			PisBaseline pisBaseline,
			PisVersion pisVersion,
			PisMission pisMission,
			bool isPisBaselineUpToDate)
		{
			this._systemId = systemId;
			this._missionId = missionId;
			this._vehiclePhysicalId = vehiclePhysicalIdField;
			this._status = status;
			this._isOnline = isOnline;
			this._communicationLink = communicationLink;

			this._serviceList = serviceList;
			this._pisBaseline = pisBaseline;
			this._pisVersion = pisVersion;
			this._pisMission = pisMission;

			this._isPisBaselineUpToDate = isPisBaselineUpToDate;
		}

		public SystemInfo(SystemInfo other)
			: this()
		{
			if (other != null)
			{
				// copy string references (strings are immutable)
				this._systemId = other._systemId;
				this._missionId = other._missionId;
				this._vehiclePhysicalId = other._vehiclePhysicalId;
				this._status = other._status;
				this._isOnline = other._isOnline;
				this._communicationLink = other._communicationLink;

				// shallow copy of list (service infos are immutable)
				this._serviceList = new ServiceInfoList(other._serviceList);

				// deep copy
				this._pisBaseline = new PisBaseline(other._pisBaseline);
				this._pisVersion = new PisVersion(other._pisVersion);
				this._pisMission = new PisMission(other._pisMission);

				this._isPisBaselineUpToDate = other._isPisBaselineUpToDate;
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
				return this._pisBaseline;
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
				return this._isPisBaselineUpToDate;
			}
		}

		/// <summary>
		/// PIS Version
		/// </summary>
		public PisVersion PisVersion
		{
			get
			{
				return this._pisVersion;
			}
		}

		/// <summary>
		/// PIS Mission
		/// </summary>
		public PisMission PisMission
		{
			get
			{
				return this._pisMission;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ushort VehiclePhysicalId
		{
			get
			{
				return this._vehiclePhysicalId;
			}
		}

		/// <summary>
		/// Gets the system identifier.
		/// </summary>
		public string SystemId
		{
			get
			{
				return this._systemId;
			}
		}

		/// <summary>
		/// Gets the mission identifier.
		/// </summary>
		public string MissionId
		{
			get
			{
				return this._missionId;
			}
		}

		/// <summary>
		/// Gets the communication link.
		/// </summary>
		public CommunicationLink CommunicationLink
		{
			get
			{
				return this._communicationLink;
			}
		}

		/// <summary>
		/// Gets the status of the embedded system.
		/// </summary>
		public uint Status
		{
			get
			{
				return this._status;
			}
		}

		/// <summary>Gets a value indicating whether the system of this object is online.</summary>
		/// <value>true if this described system is online, false if not.</value>
		public bool IsOnline
		{
			get
			{
				return this._isOnline;
			}
		}

		/// <summary>
		/// ServiceList property
		/// </summary>
		public ServiceInfoList ServiceList
		{
			get
			{
				return this._serviceList;
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
			if (!this.ServiceList.Equals(lOther.ServiceList)) { return false; }
			if (!this.PisBaseline.Equals(lOther.PisBaseline)) { return false; }
			if (!this.PisVersion.Equals(lOther.PisVersion)) { return false; }
			if (!this.PisMission.Equals(lOther.PisMission)) { return false; }

			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder(250);
            Dump(string.Empty, output);
            return output.ToString();
        }

        /// <summary>
        /// Dumps the current object into an output string.
        /// </summary>
        /// <param name="prefix">The prefix to add when writing each member.</param>
        /// <param name="output">The output string.</param>
        public void Dump(string prefix, StringBuilder output)
        {
            string memberPrefix = prefix + "\t";
            output.Append("{");

            output.AppendFormat(CultureInfo.InvariantCulture, "SystemId = '{0}',", SystemId).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}VehicleId = '{1}',", memberPrefix, VehiclePhysicalId).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}IsOnline = '{1}',", memberPrefix, IsOnline).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}MissionId = '{1}',", memberPrefix, MissionId).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}CommunicationLink = '{1}',", memberPrefix, CommunicationLink).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}Status = '{1}',", memberPrefix, Status).AppendLine();
            output.AppendFormat(CultureInfo.InvariantCulture, "{0}IsPisBaselineUpToDate = '{1}',", memberPrefix, IsPisBaselineUpToDate).AppendLine();
            if (PisBaseline != null)
            {
                output.AppendFormat(CultureInfo.InvariantCulture, "{0}PisBaseline.CurrentVersionOut = '{1}'", memberPrefix, PisBaseline.CurrentVersionOut).AppendLine();
                output.AppendFormat(CultureInfo.InvariantCulture, "{0}PisBaseline.FutureVersionOut = '{1}'", memberPrefix, PisBaseline.FutureVersionOut).AppendLine();
            }
            else
            {
                output.AppendFormat(CultureInfo.InvariantCulture, "{0}PisBaseline.CurrentVersionOut = '{1}'", memberPrefix, null).AppendLine();
                output.AppendFormat(CultureInfo.InvariantCulture, "{0}PisBaseline.FutureVersionOut = '{1}'", memberPrefix, null).AppendLine();
            }

            if (PisVersion != null)
            {
                output.AppendFormat(CultureInfo.InvariantCulture, "{0}PisVersion.VersionPISSoftware = '{1}'", memberPrefix, PisVersion.VersionPISSoftware).AppendLine();
            }
            else
            {
                output.AppendFormat(CultureInfo.InvariantCulture, "{0}PisVersion.VersionPISSoftware = '{1}'", memberPrefix, null).AppendLine();
            }

            if (PisMission != null)
            {
                output.AppendFormat(CultureInfo.InvariantCulture, "{0}PisMission.CommercialNumber = '{1}'", memberPrefix, PisMission.CommercialNumber).AppendLine();
                output.AppendFormat(CultureInfo.InvariantCulture, "{0}PisMission.MissionState = '{1}'", memberPrefix, PisMission.MissionState).AppendLine();
                output.AppendFormat(CultureInfo.InvariantCulture, "{0}PisMission.OperatorCode = '{1}'", memberPrefix, PisMission.OperatorCode).AppendLine();
            }
            else
            {
                output.AppendFormat(CultureInfo.InvariantCulture, "{0}PisMission.CommercialNumber = '{1}'", memberPrefix, null).AppendLine();
                output.AppendFormat(CultureInfo.InvariantCulture, "{0}PisMission.MissionState = '{1}'", memberPrefix, null).AppendLine();
                output.AppendFormat(CultureInfo.InvariantCulture, "{0}PisMission.OperatorCode = '{1}'", memberPrefix, null).AppendLine();
            }

            output.AppendFormat(CultureInfo.InvariantCulture, "{0}ServiceList[Count={1}] = ", memberPrefix, ServiceList.Count);
            ServiceList.Dump(memberPrefix + "\t", output);

            output.Append("}");
        }
	}
}
