// <copyright file="LiveVideoControlConfiguration.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2014.  All rights reserved.
//
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
namespace PIS.Ground.LiveVideoControl
{
    using System;
    using System.Configuration;
	using System.Web;
	using System.Web.Configuration;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.LogMgmt;

    /// <summary>
    /// Utility Class
    /// </summary>
    public class LiveVideoControlConfiguration : ILiveVideoControlConfiguration
    {
		/// <summary>
		/// Name of the configuration file
		/// </summary>
		private const string ConfigurationFileName = "LiveVideoControl.config";

		/// <summary>Name of the configuration file including its path</summary>
		private static string _configurationFilePathAndName;

        /// <summary>
        /// URL to be used for automatic mode, null for manual mode
        /// </summary>
        private static string _automaticModeURL;

		/// <summary>Automatic mode URL (Null in manual mode).</summary>
		/// <value>The automatic mode URL.</value>
		public string AutomaticModeURL
		{
			get
			{
				return _automaticModeURL;
			}
			set
			{
				_automaticModeURL = value;
				WriteToFile();
			}
		}

		/// <summary>Stores the configuration to a file.</summary>
		/// <returns> true if success, false otherwise </returns>
		private bool WriteToFile()
		{
			bool success = false;

			try
			{
				string message = "Writing the configuration file: " + _configurationFilePathAndName + Environment.NewLine;

				LogManager.WriteLog(
					TraceType.INFO,
					message,
					"PIS.Ground.LiveVideoControl.LiveVideoControlConfiguration.Store",
					null,
					EventIdEnum.LiveVideoControl);

				var serializer = new System.Xml.Serialization.XmlSerializer(typeof(LiveVideoControlConfiguration));

				using (var writer = new System.IO.StreamWriter(_configurationFilePathAndName))
				{
					serializer.Serialize(writer, this);
				}

				success = true;
			}
			catch (System.Exception ex)
			{
				LogManager.WriteLog(
					TraceType.ERROR,
					"Exception while storing the configuration to file: " +
						LiveVideoControlConfiguration.ConfigurationFileName,
					"PIS.Ground.LiveVideoControl.LiveVideoControlConfiguration.Store",
					ex,
					EventIdEnum.LiveVideoControl);
			}
			return success;
		}

        /// <summary>
        ///  Initialize the configuration settings from the configuration file
        /// </summary>
        /// <returns> if error return true else false </returns>
        public static bool InitializeConfiguration()
        {
            bool error = true;

            _automaticModeURL = string.Empty;
			LiveVideoControlConfiguration configurationObject = new LiveVideoControlConfiguration();

			try
			{
				string folder = HttpRuntime.AppDomainAppPath;
				_configurationFilePathAndName = System.IO.Path.Combine(folder, LiveVideoControlConfiguration.ConfigurationFileName);

				string message = "Reading the configuration file: " + _configurationFilePathAndName + Environment.NewLine;

				LogManager.WriteLog(
					TraceType.INFO,
					message,
					"PIS.Ground.LiveVideoControl.LiveVideoControlConfiguration.InitializeConfiguration",
					null,
					EventIdEnum.LiveVideoControl);

				var serializer = new System.Xml.Serialization.XmlSerializer(typeof(LiveVideoControlConfiguration));

				using (var reader = new System.IO.StreamReader(_configurationFilePathAndName))
				{
					configurationObject = (LiveVideoControlConfiguration)serializer.Deserialize(reader);
				}

				error = false;
			}
			catch (System.Exception ex)
			{
				LogManager.WriteLog(
					TraceType.ERROR,
					"Exception while storing the configuration to file: " +
						LiveVideoControlConfiguration.ConfigurationFileName,
					"PIS.Ground.LiveVideoControl.LiveVideoControlConfiguration.InitializeConfiguration",
					ex,
					EventIdEnum.LiveVideoControl);

				// In case of error, try to write a fresh configuration to the file.
				// If the file does not exists yet, it will be created.
				configurationObject.WriteToFile();
			}

			return error;
        }
    }
}
