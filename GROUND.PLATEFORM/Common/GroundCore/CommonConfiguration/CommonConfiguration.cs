//---------------------------------------------------------------------------------------------------
// <copyright file="CommonConfiguration.cs" company="Alstom">
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
using System.Web.Configuration;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.Data;
using System.Configuration;

namespace PIS.Ground.Core
{
    /// <summary>Common configuration for all PIS Ground Web services.</summary>
    public sealed class CommonConfiguration
    {

        private const string PARAM_NAME_PLATFORM_TYPE = "PlatformType";

        /// <summary>Values that represent PlatformTypeEnum.</summary>
        public enum PlatformTypeEnum
        {
            UNKNOWN,
            URBAN,
            SIVENG,
        }

        /// <summary>Gets or sets the type of the platform.</summary>
        /// <value>The type of the platform.</value>
        public static PlatformTypeEnum PlatformType
        {
            get
            {
                try
                {
                    return (PlatformTypeEnum)Enum.Parse(typeof(PlatformTypeEnum), GetConfigurationParameter(PARAM_NAME_PLATFORM_TYPE));
                }
                catch(ArgumentException)
                {
                    return PlatformTypeEnum.UNKNOWN;
                }
            }

            set
            {
                SetConfigurationParameter(PARAM_NAME_PLATFORM_TYPE, value.ToString());
            }
        }

        /// <summary>Gets persistent common configuration parameter string value.</summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>The configuration parameter.</returns>
        private static string GetConfigurationParameter(string paramName)
        {
            string paramValue = "";

            try
            {
                // Get the configuration object to access the related Web.config file.
                System.Configuration.Configuration config =
                    WebConfigurationManager.OpenWebConfiguration("/CommonConfiguration");

                paramValue = config.AppSettings.Settings[paramName].Value;                
                
                LogManager.WriteLog(
                    TraceType.DEBUG,
                    "Read configuration parameter [" + paramName + "=" + paramValue + "]",
                    "PIS.Ground.Core.CommonConfiguration.GetConfigurationParameter()",
                    null,
                    EventIdEnum.GroundCore);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(
                    TraceType.ERROR,
                    ex.Message,
                    "PIS.Ground.Core.CommonConfiguration.GetConfigurationParameter()",
                    ex,
                    EventIdEnum.GroundCore);
            }

            return paramValue;
        }

        /// <summary>Sets persistent common configuration parameter string value.</summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">The parameter value.</param>
        private static void SetConfigurationParameter(string paramName, string paramValue)
        {
            try
            {
                // Get the configuration object to access the related Web.config file.
                System.Configuration.Configuration config =
                    WebConfigurationManager.OpenWebConfiguration("/CommonConfiguration");

                AppSettingsSection settings = (AppSettingsSection)config.GetSection("appSettings");

                if (settings != null)
                {
                    settings.Settings[paramName].Value = paramValue;
                    config.Save();
                }

                LogManager.WriteLog(
                    TraceType.DEBUG,
                    "Write configuration parameter [" + paramName + "=" + paramValue + "]",
                    "PIS.Ground.Core.CommonConfiguration.SetConfigurationParameter()",
                    null,
                    EventIdEnum.GroundCore);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(
                    TraceType.ERROR,
                    ex.Message,
                    "PIS.Ground.Core.CommonConfiguration.SetConfigurationParameter()",
                    ex,
                    EventIdEnum.GroundCore);
            }            
        } 
    }
}
