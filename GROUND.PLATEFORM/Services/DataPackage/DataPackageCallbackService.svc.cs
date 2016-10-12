using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Threading;

namespace PIS.Ground.DataPackage
{    
    public class DataPackageCallbackService : IDataPackageCallbackService
    {
		/// <summary>The object that serialize a string into XML.</summary>
		private static XmlSerializer _stringXmlSerializer = new XmlSerializer(typeof(string));

		/// <summary>The object that serialize a string list into XML.</summary>
		private static XmlSerializer _stringListXmlSerializer = new XmlSerializer(typeof(List<string>));

		/// <summary>Format notification parameter of type string.</summary>
		/// <param name="parameter">The string parameter value to formation.</param>
		/// <returns>The formatted notification parameter.</returns>
		private static string FormatNotificationParameter(string parameter)
		{
			using (StringWriter lWriter = new StringWriter())
			{
				_stringXmlSerializer.Serialize(lWriter, parameter);
				return lWriter.ToString();
			}
		}

		/// <summary>Format notification parameter of type string.</summary>
		/// <param name="parameter">The string list parameter value to formation.</param>
		/// <returns>The formatted notification parameter.</returns>
		private static string FormatNotificationParameter(List<string> parameters)
		{
			using (StringWriter lWriter = new StringWriter())
			{
				_stringListXmlSerializer.Serialize(lWriter, parameters);
				return lWriter.ToString();
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPackageCallbackService"/> class.
        /// </summary>
        public DataPackageCallbackService()
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "DataPackageCallbackService";
            }

            DataPackageService.Initialize();
        }

        public void updateBaselineDefinitionStatus(Guid pReqID, string pBLVersion, Notification.NotificationIdEnum pStatus)
        {
            DataPackageService.sendNotificationToGroundApp(pReqID, (PIS.Ground.GroundCore.AppGround.NotificationIdEnum)pStatus, FormatNotificationParameter(pBLVersion)); 
        }

        public void missingDataPackageNotification(Guid pReqID, Dictionary<string, string> pDPCharsList)
        {
			List<string> CharacteristicsList = new List<string>(2 * pDPCharsList.Count);
            // Put dictionary entries into the CharacteristicsList
            foreach (KeyValuePair<string, string> dicEntry in pDPCharsList)
            {
                CharacteristicsList.Add(dicEntry.Key);
                CharacteristicsList.Add(dicEntry.Value);
            }

            DataPackageService.sendNotificationToGroundApp(pReqID, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageMissing, FormatNotificationParameter(CharacteristicsList));
        }

        public void updatePackageUploadStatus(Guid pReqID, Notification.NotificationIdEnum pStatus, Dictionary<string, string> pDPCharsList)
        {          
            List<string> CharacteristicsList = new List<string>(2 * pDPCharsList.Count);
            // Put dictionary entries into the CharacteristicsList
            foreach (KeyValuePair<string, string> dicEntry in pDPCharsList)
            {
                CharacteristicsList.Add(dicEntry.Key.ToString());
                CharacteristicsList.Add(dicEntry.Value);
            }                          

            DataPackageService.sendNotificationToGroundApp(pReqID, (PIS.Ground.GroundCore.AppGround.NotificationIdEnum)pStatus, FormatNotificationParameter(CharacteristicsList));           
        }

        public void updateBaselineAssignmentStatus(Guid pReqID, Notification.NotificationIdEnum pStatus, string pElementId, string pBLVersion)
        {
			List<string> lParamList = new List<string>(2);
			lParamList.Add(pElementId);
			lParamList.Add(pBLVersion);
            DataPackageService.sendNotificationToGroundApp(pReqID, (PIS.Ground.GroundCore.AppGround.NotificationIdEnum)pStatus, FormatNotificationParameter(lParamList));
        }        
    }
}
