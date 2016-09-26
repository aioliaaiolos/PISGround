//---------------------------------------------------------------------------------------------------
// <copyright file="RemoteDataStoreService.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Xml;
using java.io;
using java.util;
using java.util.zip;
using Microsoft.XmlDiffPatch;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.Utility;
using PIS.Ground.RemoteDataStore.DataPackageCallbackClient;

namespace PIS.Ground.RemoteDataStore
{
    /// <summary>
    /// Implement the IRemoteDataStore service contract in service class
    /// </summary>
    [ServiceBehavior]
    public class RemoteDataStoreService : IRemoteDataStore, IDisposable
    {
        private DatabaseAccessImplClass _dBAccess;
        private string _dataStorePath;
        private string _openPackagesPath;
        private string _tmpPath;
        private List<string> _dataPackagesTypes;
        private DataPackageCallbackClient.DataPackageCallbackServiceClient _dataPackageCallbackClient;
        private const string _zeroBaseLineVersion = "0.0.0.0";
        private object _unzipPackageLock = new object();
        private DateTime _lowestActivationDateTime = new DateTime(1900, 1, 1);
        private DateTime _lowestExpirationDateTime = new DateTime(1900, 1, 2);

        public RemoteDataStoreService()
        {
            try
            {                           
                _dataPackageCallbackClient = new DataPackageCallbackClient.DataPackageCallbackServiceClient();
                _dBAccess = DatabaseAccessImplClass.asInstance;           
                _dataStorePath = System.IO.Path.GetFullPath(ConfigurationSettings.AppSettings["RemoteDataStoreUrl"]);
                _openPackagesPath = System.IO.Path.GetFullPath(ConfigurationSettings.AppSettings["OpenPackagesPath"]);
                _tmpPath = Path.Combine(_dataStorePath, "Temp");
                _dataPackagesTypes = new List<string>();
                Directory.CreateDirectory(_tmpPath);
                Directory.CreateDirectory(Path.Combine(_dataStorePath, "PISBASE"));
                Directory.CreateDirectory(Path.Combine(_dataStorePath, "PISMISSION"));
                Directory.CreateDirectory(Path.Combine(_dataStorePath, "PISINFOTAINMENT"));
                Directory.CreateDirectory(Path.Combine(_dataStorePath, "LMT"));
                string lDPTypes = ConfigurationSettings.AppSettings["DataPackagesTypes"];
                foreach (string  lType in lDPTypes.Split(','))
                {
                    _dataPackagesTypes.Add(lType.ToUpperInvariant());
                }
                
                LogManager.LogLevel = (TraceType)Enum.Parse(typeof(TraceType), ConfigurationSettings.AppSettings["LogLevel"]);
            }
			catch(Exception ex)
			{
				LogManager.WriteLog(TraceType.ERROR, ex.Message, "Remote Data Store Initialization", ex.InnerException, EventIdEnum.RemoteDataStore);
			}
            finally
            {
                if (_dBAccess == null)
                {
                    LogManager.WriteLog(
                        TraceType.ERROR,
                        "Remote Data Store access initialization error. The probable cause - one of the *.sql files is missing",
                        "PIS.Ground.RemoteDataStore.RemoteDataStoreService.RemoteDataStoreService()", null, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);                    
                }
            }
        }

        private bool mVersionMatch(String pVersion)
        {
            return Regex.Match(pVersion, @"^(\d+\.){3}\d+$").Success;
        }

        public DatabaseAccessImplClass DBAccess
        {
            get { return _dBAccess; }
        }

        public string DataStorePath
        {
            get { return _dataStorePath; }
        }

        public List<string> DataPackagesTypes
        {
            get { return _dataPackagesTypes; }
        }

        /// <summary>
        /// Gets or sets the operation timeout.
        /// </summary>
        public TimeSpan OperationTimeout { get; set; }

        OpenDataPackageResult IRemoteDataStore.openLocalDataPackage(
            string packageType,
            string packageVersion,
            string fileRegexMatchPattern)
        {
            OpenDataPackageResult result = new OpenDataPackageResult();
            result.Status = OpenDataPackageStatusEnum.FAILED_UNKNOWN_PACKAGE;
            try
            {
                result.LocalPackagePath = openPackage(packageType, packageVersion, fileRegexMatchPattern);
                if (!string.IsNullOrEmpty(result.LocalPackagePath))
                {
                    result.Status = OpenDataPackageStatusEnum.COMPLETED;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.RemoteDataStore.RemoteDataStoreService.openLocalDataPackage", ex, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);
            }
            return result;
        }

        public void moveTheNewDataPackageFiles(Guid pReqID, string pFileURL)
        {
            PIS.Ground.Core.LogMgmt.LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.INFO, "Downloading " + pFileURL, "PIS.Ground.RemoteDataStore.RemoteDataStoreService.moveTheNewDataPackageFile", null, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);

            //Get different informations from path
            string lFileName = Path.GetFileName(pFileURL).ToLowerInvariant();
            string lFileNameWExt = Path.GetFileNameWithoutExtension(pFileURL);
            string lType = "";
            string lVersion = "";            
            string lDestFolder = "";
            string lDestFile = "";

            bool lFileNameIsOk = true;

            Regex lRegex = new Regex(@"^(\w+)\-(\d+\.\d+\.\d+\.\d+)\.(zip)$", RegexOptions.IgnoreCase);
            Match lMatch = lRegex.Match(lFileName);

            if (lMatch.Success &&
                lMatch.Groups.Count == 4)
            {
                lType = lMatch.Groups[1].ToString().ToUpperInvariant();
                lVersion = lMatch.Groups[2].ToString();
            }
            else
            {
                lRegex = new Regex(@"^(\w+)\-(\d+\.\d+\.\d+\.\d+)\-(20[0-9][0-9][0-1][0-9][0-3][0-9])\-([0-2][0-9][0-5][0-9])\.(zip)$", RegexOptions.IgnoreCase);
                lMatch = lRegex.Match(lFileName);

                if (lMatch.Success &&
                    lMatch.Groups.Count == 6)
                {
                    lType = lMatch.Groups[1].ToString().ToUpperInvariant();
                    lVersion = lMatch.Groups[2].ToString();                    
                }
                else
                {
                    lRegex = new Regex(@"^(20[0-9][0-9][0-1][0-9][0-3][0-9])\-([0-2][0-9][0-5][0-9])\-(\w+)_(\d+_\d+_\d+_\d+)\.(zip)$", RegexOptions.IgnoreCase);
                    lMatch = lRegex.Match(lFileName);

                    if (lMatch.Success &&
                        lMatch.Groups.Count == 6)
                    {
                        lType = lMatch.Groups[3].ToString().ToUpperInvariant();
                        if (lType == "LMTDB")
                        {
                            lType = "LMT";
                        }
                        lVersion = lMatch.Groups[4].ToString();
                        lVersion = lVersion.Replace('_', '.');
                    }
                    else
                    {
                        lFileNameIsOk = false;
                    }
                }
            }

            //Init callback dictionnary
            Dictionary<string, string> lPkgDictionnary = new Dictionary<string, string>();

            lPkgDictionnary.Add(lType.ToUpper(), lVersion);

            //Send callback to DataPackage
            _dataPackageCallbackClient.updatePackageUploadStatus(pReqID, NotificationIdEnum.DataPackageUploadProcessing, lPkgDictionnary);

            if (!lFileNameIsOk || !_dataPackagesTypes.Contains(lType))
            {
                //Send callback to DataPackage
                _dataPackageCallbackClient.updatePackageUploadStatus(pReqID, NotificationIdEnum.DataPackageUploadFailed, lPkgDictionnary);
            }
            else
            {
                lDestFolder = _dataStorePath + lType;
                lDestFile = Path.Combine(lDestFolder, lFileName);
                
                if (checkIfDataPackageExists(lType, lVersion))
                {
                    //Send callback to DataPackage
                    _dataPackageCallbackClient.updatePackageUploadStatus(pReqID, NotificationIdEnum.DataPackageUploadAlreadyExist, lPkgDictionnary);
                }
                else
                {
                    try
                    {
                        FileInfo lDestFileInfo = new FileInfo(lDestFile);
                        if (lDestFileInfo.Exists)
                        {
                            try
                            {
                                System.IO.File.Delete(lDestFile);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        PIS.Ground.Core.Data.RemoteFileClass lSrcFile = new PIS.Ground.Core.Data.RemoteFileClass(pFileURL, true);
                        if (lSrcFile.Exists)
                        {
                            if (lSrcFile.Size > 0)
                            {
                                Stream lSrcStream;
                                lSrcFile.OpenStream(out lSrcStream);
                                if (lSrcStream != null)
                                {
                                    try
                                    {
                                            // If is a local file, the file was not copied yet to the destination 
                                            // folder.
                                            if (lSrcFile.FileType == FileTypeEnum.LocalFile)
                                            {
                                                lSrcStream.Seek(0, SeekOrigin.Begin);
                                                FileStream lDestStream = lDestFileInfo.OpenWrite();

                                                int lBuffLength = 2048;
                                                byte[] lBuffer = new byte[lBuffLength];
                                                int lContentLen = lSrcStream.Read(lBuffer, 0, lBuffLength);

                                                while (lContentLen != 0)
                                                {
                                                    lDestStream.Write(lBuffer, 0, lContentLen);
                                                    lContentLen = lSrcStream.Read(lBuffer, 0, lBuffLength);
                                                }
                                                lDestStream.Close();
                                            }
                                            lSrcStream.Close();

                                        //If package is encrypted - decrypt it
                                        bool lFileIsOk = true;

                                        if (AXORCryptClass.IsFileEncrypted(lDestFile))
                                        {
                                            int lLengthWithoutExt = lDestFile.Length - lDestFileInfo.Extension.Length;

                                            String lTempFileName = lDestFile.Substring(0, lLengthWithoutExt) + "_temp" + lDestFileInfo.Extension;                                         

                                            if (AXORCryptClass.ConvertFile(lDestFile, lTempFileName))
                                            {
                                                System.IO.File.Replace(lTempFileName, lDestFile, null);
                                            }
                                            else //Can't decrypt the src file
                                            {
                                                lFileIsOk = false;
                                                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.ERROR, "Can't decrypt file", "PIS.Ground.RemoteDataStore.RemoteDataStoreService.moveTheNewDataPackageFile", null, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);
                                                _dataPackageCallbackClient.updatePackageUploadStatus(pReqID, NotificationIdEnum.DataPackageUploadFailed, lPkgDictionnary);
                                            }
                                        }

                                        if (lFileIsOk)
                                        {
                                            //DataBase
                                            DataContainer lDPTable = new DataContainer();
                                            lDPTable.Columns.Add("DataPackageType");
                                            lDPTable.Columns.Add("DataPackageVersion");
                                            lDPTable.Columns.Add("DataPackagePath");

                                            lDPTable.Rows.Add(lType);
                                            lDPTable.Rows.Add(lVersion);
                                            lDPTable.Rows.Add(lType + "/" + lFileName);

                                            setNewDataPackage(lDPTable);
                                            //Send callback to DataPackage
                                            _dataPackageCallbackClient.updatePackageUploadStatus(pReqID, NotificationIdEnum.DataPackageUploadCompleted, lPkgDictionnary);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        PIS.Ground.Core.LogMgmt.LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.ERROR, ex.Message, "PIS.Ground.RemoteDataStore.RemoteDataStoreService.moveTheNewDataPackageFile", ex, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);
                                        _dataPackageCallbackClient.updatePackageUploadStatus(pReqID, NotificationIdEnum.DataPackageUploadFailed, lPkgDictionnary);
                                    }
                                }
                                else //Can't open a stream on the src file
                                {
                                    PIS.Ground.Core.LogMgmt.LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.ERROR, "Can't read " + pFileURL, "PIS.Ground.RemoteDataStore.RemoteDataStoreService.moveTheNewDataPackageFile", null, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);
                                    _dataPackageCallbackClient.updatePackageUploadStatus(pReqID, NotificationIdEnum.DataPackageUploadFailed, lPkgDictionnary);
                                }
                            }
                            else //file is empty : size equals 0
                            {
                                PIS.Ground.Core.LogMgmt.LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.ERROR, pFileURL + " is empty.", "PIS.Ground.RemoteDataStore.RemoteDataStoreService.moveTheNewDataPackageFile", null, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);
                                _dataPackageCallbackClient.updatePackageUploadStatus(pReqID, NotificationIdEnum.DataPackageUploadFailed, lPkgDictionnary);
                            }
                        }
                        else //The src file does not exists
                        {
                            PIS.Ground.Core.LogMgmt.LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.ERROR, pFileURL + " does not exists or is not accessible.", "PIS.Ground.RemoteDataStore.RemoteDataStoreService.moveTheNewDataPackageFile", null, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);
                            _dataPackageCallbackClient.updatePackageUploadStatus(pReqID, NotificationIdEnum.DataPackageUploadFailed, lPkgDictionnary);
                        }
                    }
                    catch ( Exception ex )
                    {
                        PIS.Ground.Core.LogMgmt.LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.ERROR, ex.Message, "PIS.Ground.RemoteDataStore.RemoteDataStoreService.moveTheNewDataPackageFile", ex, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);
                        _dataPackageCallbackClient.updatePackageUploadStatus(pReqID, NotificationIdEnum.DataPackageUploadFailed, lPkgDictionnary);
                    }                    
                }
            }
        }

        public bool checkUrl(Guid pReqID, string pUrl)
        {
            return PIS.Ground.Core.Data.RemoteFileClass.checkUrl(pUrl);
        }

        public void checkDataPackagesAvailability(Guid pReqID, DataContainer pBLDefs)
        {
            if (pBLDefs == null || pBLDefs.Rows.Count != pBLDefs.Columns.Count)
            {
                throw new FaultException("Baseline Definition Not Valid", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_BASELINE_DEFINITION.ToString()));
            }

            Dictionary<string, string> lResList = new Dictionary<string, string>();
            if (!checkIfDataPackageExists("PISBASE", pBLDefs.getStrValue("PISBaseDataPackageVersion")))
            {
                lResList.Add("PISBASE", pBLDefs.getStrValue("PISBaseDataPackageVersion"));
            }
            if (!checkIfDataPackageExists("PISMISSION", pBLDefs.getStrValue("PISMissionDataPackageVersion")))
            {
                lResList.Add("PISMISSION", pBLDefs.getStrValue("PISMissionDataPackageVersion"));
            }
            if (!checkIfDataPackageExists("PISINFOTAINMENT", pBLDefs.getStrValue("PISInfotainmentDataPackageVersion")))
            {
                lResList.Add("PISINFOTAINMENT", pBLDefs.getStrValue("PISInfotainmentDataPackageVersion"));
            }
            if (!checkIfDataPackageExists("LMT", pBLDefs.getStrValue("LMTDataPackageVersion")))
            {
                lResList.Add("LMT", pBLDefs.getStrValue("LMTDataPackageVersion"));
            }
            if (lResList.Count != 0)
            {
                //Send callback to DataPackage
                _dataPackageCallbackClient.missingDataPackageNotification(pReqID, lResList);
            }
        }

        public bool checkIfDataPackageExists(string pDPType, string pDPVersion)
        {
            if (!_dataPackagesTypes.Contains(pDPType.ToUpperInvariant()))
            {
                throw new FaultException("Unknown DataPackage type", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_DATAPACKAGE_TYPE.ToString()));
            }
            if (!mVersionMatch(pDPVersion))
            {
                throw new FaultException("Invalid DataPackage version", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_DATAPACKAGE_VERSION.ToString()));
            }

            return _dBAccess.mDataPackageExists(pDPType.ToUpperInvariant(), pDPVersion);
        }

        public bool checkIfBaselineExists(string pBLVersion)
        {
            if (!mVersionMatch(pBLVersion))
            {
                throw new FaultException("Baseline version Not Valid", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_BASELINE_VERSION.ToString()));
            }

            return _dBAccess.mBaselineExists(pBLVersion);
        }

        public bool checkIfElementExists(string pEID)
        {
            if (String.IsNullOrEmpty(pEID))
            {
                throw new FaultException("Invalid element id", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_ELEMENT_ID.ToString()));
            }

            return _dBAccess.mElementExists(pEID);
        }

        public void setNewDataPackage(DataContainer pNDPkg)
        {
            if (pNDPkg == null)
            {
                throw new FaultException("Invalid datapackage definition", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_DATAPACKAGE_DEFINITION.ToString()));
            }
            if (!checkIfDataPackageExists(pNDPkg.getStrValue("DataPackageType"), pNDPkg.getStrValue("DataPackageVersion")))
            {
                _dBAccess.mSetNewDataPackage(pNDPkg);
            }
        }

        public void setNewBaselineDefinition(Guid pReqID, DataContainer pNBLDef)
        {
            if (pNBLDef == null)
            {
                throw new FaultException("Baseline Definition Not Valid", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_BASELINE_DEFINITION.ToString()));
            }

            string lBLVersion = pNBLDef.getStrValue("BaselineVersion");
            _dataPackageCallbackClient.updateBaselineDefinitionStatus(pReqID, lBLVersion, NotificationIdEnum.DataPackageBaselineDefinitionProcessing);

            if(checkIfBaselineExists(lBLVersion))
            {
                //Send callback to DataPackage
                _dataPackageCallbackClient.updateBaselineDefinitionStatus(pReqID, lBLVersion, NotificationIdEnum.DataPackageBaselineDefinitionFailed);
            }
            else
            {
                //Send callback to DataPackage
                _dBAccess.mSetNewBaselineDefinition(pNBLDef);
                _dataPackageCallbackClient.updateBaselineDefinitionStatus(pReqID, lBLVersion, NotificationIdEnum.DataPackageBaselineDefinitionCompleted);
            }
        }

        public DataContainer getBaselinesDefinitions()
        {
            return _dBAccess.mGetBaselinesDefinitions();
        }
		
		/// <summary>Delete the baseline definition described by version.</summary>
        /// <param name="pVersion">The version. Could contains wildcars.</param>
        /// <returns>The list of assigned baselines, that weren't removed</returns>
        public void deleteBaselineDefinition(string pVersion)
		{
            if (!checkIfBaselineExists(pVersion))
            {
                throw new FaultException("Unknown Baseline version", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_BASELINE_VERSION.ToString()));
            }

            _dBAccess.mDeleteBaselineDefinition(pVersion);
		}

        public DataContainer getBaselineDefinition(string pBLVersion)
        {
            if (!checkIfBaselineExists(pBLVersion))
            {
                throw new FaultException("Unknown Baseline version", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_BASELINE_VERSION.ToString()));
            }

            return _dBAccess.mGetBaselineDefinition(pBLVersion);
        }

        public DataContainer getAssignedBaselinesVersions()
        {
            return _dBAccess.mGetAssignedBaselinesVersions();
        }

        public string getAssignedCurrentBaselineVersion(string pEID)
        {

            if (!checkIfElementExists(pEID))
            {
                throw new FaultException("Unknow ElementID", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_ELEMENT_ID.ToString()));
            }

            DataContainer lElBLs = _dBAccess.mGetElementAssignedBaselines(pEID);
            return lElBLs.getStrValue("AssignedCurrentBaseline");
        }

        public string getAssignedFutureBaselineVersion(string pEID)
        {
            if (!checkIfElementExists(pEID))
            {
                throw new FaultException("Unknow ElementID", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_ELEMENT_ID.ToString()));
            }

            DataContainer lElBLs = _dBAccess.mGetElementAssignedBaselines(pEID);
            
            return lElBLs.getStrValue("AssignedFutureBaseline");
        }

        public DataContainer getDataPackages(DataContainer pBLDef)
        {
            if (pBLDef == null || pBLDef.Rows.Count != pBLDef.Columns.Count)
            {
                throw new FaultException("Baseline Definition Not Valid", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_BASELINE_DEFINITION.ToString()));
            }
            return _dBAccess.mGetDataPackages(pBLDef.getStrValue("BaselineVersion"));
        }

        public DataContainer getDataPackagesList()
        {                    
            return _dBAccess.mGetDataPackagesList();
        }

        public DataContainer getDataPackageCharacteristics(string pDPType, string pDPVersion)
        {
            if (!_dataPackagesTypes.Contains(pDPType.ToUpperInvariant()))
            {
                throw new FaultException("Unknown DataPackage type", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_DATAPACKAGE_TYPE.ToString()));
            }
            if (!mVersionMatch(pDPVersion))
            {
                throw new FaultException("Invalid DataPackage version", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_DATAPACKAGE_VERSION.ToString()));
            }

            return _dBAccess.mGetDataPackageCharacteristics(pDPType.ToUpperInvariant(), pDPVersion);
        }

        public DataContainer getElementBaselinesDefinitions(string pEID)
        {
            if (!checkIfElementExists(pEID))
            {
                throw new FaultException("Unknow ElementID", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_ELEMENT_ID.ToString()));
            }

            return _dBAccess.mGetElementAssignedBaselines(pEID);
        }

        public DataContainer getCurrentBaselineDefinition(string pEID)
        {
            if (!checkIfElementExists(pEID))
            {
                throw new FaultException("Unknow ElementID", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_ELEMENT_ID.ToString()));
            }

            return _dBAccess.mGetElementSpecificBaselineDefinition(pEID, "AssignedCurrent");
        }

        public string createBaselineFile(Guid pReqId, string pEID, string pBLVersion, string pActivationDate, string pExpirationDate)
        {
            if (!checkIfElementExists(pEID))
            {
                throw new FaultException("Unknow ElementID", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_ELEMENT_ID.ToString()));
            }

            string lBaselineFile = _dataStorePath + "\\BaselinesDefinitions\\" + pReqId.ToString() + "\\" + pEID;
            Directory.CreateDirectory(lBaselineFile);

            DataContainer lBLDef = _dBAccess.mGetBaselineDefinition(pBLVersion);
            lBaselineFile += "\\baseline-" + lBLDef.getStrValue("BaselineVersion") + ".xml";


            //create xml file
            XmlWriterSettings lWSettings = new XmlWriterSettings();
            lWSettings.Indent = true;
            XmlWriter lXW = XmlWriter.Create(lBaselineFile, lWSettings);
            lXW.WriteStartDocument();

            lXW.WriteStartElement("Baseline");
            lXW.WriteStartAttribute("Version");
            lXW.WriteString(lBLDef.getStrValue("BaselineVersion"));
            lXW.WriteEndAttribute();
            lXW.WriteStartAttribute("ActivationDate");
            DateTime ldt = new DateTime();
            DateTime.TryParse(pActivationDate,out ldt);
            lXW.WriteString(ldt.ToString("s"));
            lXW.WriteEndAttribute();
            lXW.WriteStartAttribute("ExpirationDate");
            DateTime.TryParse(pExpirationDate,out ldt);
            lXW.WriteString(ldt.ToString("s"));
            lXW.WriteEndAttribute();

            lXW.WriteStartElement("FileDescription");
            lXW.WriteString("XML Data Packages Baseline Definition");
            lXW.WriteEndElement();

            lXW.WriteStartElement("Packages");

            foreach (string lPkg in _dataPackagesTypes)
            {
                lXW.WriteStartElement("Package");
                lXW.WriteStartAttribute("Name");
				lXW.WriteString(lPkg);
                lXW.WriteStartAttribute("Version");
                lXW.WriteString(lBLDef.getStrValue(lPkg + "DataPackageVersion"));
                lXW.WriteEndElement();
            }

            lXW.WriteEndElement();

            lXW.WriteEndElement();

            lXW.WriteEndDocument();

            lXW.Close();

            string lReturn = "/BaselinesDefinitions/" + pReqId.ToString() + "/" + pEID + "/" + "baseline-" + lBLDef.getStrValue("BaselineVersion") + ".xml";
            return lReturn;
        }

        public string getDiffDataPackageUrl(Guid pReqId, string pEID, string pDPType, string pDPVersionOnBoard, string pDPVersionOnGround)
        {
            //check parameters
            if (String.IsNullOrEmpty(pEID))
            {
                throw new FaultException("Unknown ElementID", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_ELEMENT_ID.ToString()));
            }
            if (!mVersionMatch(pDPVersionOnBoard))
            {
                throw new FaultException("Invalid Onboard package version", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_ONBOARD_PACKAGE_VERSION.ToString()));
            }
            if (!mVersionMatch(pDPVersionOnGround))
            {
                throw new FaultException("Invalid Onground package version", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_ONGROUND_PACKAGE_VERSION.ToString()));
            }

            string onGroundPackage = getDataPackagePath(pDPType, pDPVersionOnGround);
            if (!System.IO.File.Exists(onGroundPackage))
            {
                throw new FaultException("The Onground package does not exists in the RemoteDataStore file system", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_ONGROUND_PACKAGE_VERSION.ToString()));
            }

            //init destination
            string lIncrDataPackageDir = _dataStorePath + "\\IncrementalDataPackages\\" + pReqId.ToString() + "\\" + pEID + "\\" + pDPType.ToUpperInvariant() + "-" + pDPVersionOnGround;
            string lUrl = "IncrementalDataPackages/" + pReqId.ToString() + "/" + pEID + "/" + pDPType.ToUpperInvariant() + "-" + pDPVersionOnGround + ".zip";
            
            //get diff file and clean temporary manifests
            string lDiffManifestFile = Path.GetRandomFileName();
            lDiffManifestFile = lDiffManifestFile.Substring(0, lDiffManifestFile.IndexOf('.')) + ".xml";
            lDiffManifestFile = Path.Combine(_tmpPath, lDiffManifestFile);

            // create destination folder 
            Directory.CreateDirectory(lIncrDataPackageDir);

            try
            {
                // get manifests
                string lOnBoardManifestFile = getManifest(pDPType, pDPVersionOnBoard);
                string lOnGroundManifestFile = getManifest(pDPType, pDPVersionOnGround);

                MainDiffClass lMainDiff = new MainDiffClass();
                lMainDiff.nGenerateReadableDiffXml(lOnBoardManifestFile, lOnGroundManifestFile, lDiffManifestFile, new string[] { "version", "type" }, _tmpPath, pDPType + "-" + pDPVersionOnBoard, pDPType + "-" + pDPVersionOnGround);
                System.IO.File.Delete(lOnBoardManifestFile);
                System.IO.File.Delete(lOnGroundManifestFile);

                //extract necessary files from new data package
                extractFilesFromPackage(lIncrDataPackageDir, lDiffManifestFile, pDPType, pDPVersionOnGround);

                //construct list of file from on board package that needed by new package
                generateOldPackageNeededFiles(lIncrDataPackageDir, pDPType, pDPVersionOnBoard, pDPVersionOnGround, lDiffManifestFile);

                //Zip the incremental data package
                zipPackage(lIncrDataPackageDir, pDPType, pDPVersionOnGround);
            }
            catch(Exception ex)
            {
                LogManager.WriteLog(TraceType.WARNING,
                    string.Format(CultureInfo.CurrentCulture,
                    "An exception happened when acquiring the Onboard package '{0}' with the version '{1}'." +
                    "The full Onground package version '{2}' will be sent to the train.", pDPType, pDPVersionOnBoard, pDPVersionOnGround),
                    "PIS.Ground.RemoteDataStore.RemoteDataStoreService.getDiffDataPackageUrl", ex, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);
                
                // Something went wrong, use the full on ground package for incremental.
                // Its existence is checked and an exception is thrown if it's not found.
                System.IO.File.Copy(onGroundPackage, lIncrDataPackageDir + ".zip", true);
            }
            finally
            {
                //delete temp directory
                Directory.Delete(lIncrDataPackageDir, true);
                if (System.IO.File.Exists(lDiffManifestFile))
                {
                    System.IO.File.Delete(lDiffManifestFile);
                }
            }

            //return incremental package url
            return lUrl;
        }

        private void extractFilesFromPackage(string pDataPackageDir, string pDiffManifestFile, string pDPType, string pDPVersionOnGround)
        {
            //--------------------------------------------
            //extract necessary files from new datapackage
            //--------------------------------------------
                //read diff file
            XmlDocument lXmlDiffDoc = new XmlDocument();
            lXmlDiffDoc.Load(pDiffManifestFile);
                //get add/modify tags
            XmlElement lXmlDiffDocRoot = lXmlDiffDoc.DocumentElement;
            XmlNodeList lAddNodes = lXmlDiffDocRoot.SelectNodes("//add|//modify");
                //get zip file from remote data store
            string lDPSrc = _dataStorePath;
            DataContainer lDPChar = getDataPackageCharacteristics(pDPType, pDPVersionOnGround);
            lDPSrc = Path.Combine(lDPSrc, lDPChar.getStrValue("DataPackagePath"));
            lDPSrc = lDPSrc.Replace("/", "\\");
            ZipFile lInputZipFile = new ZipFile(lDPSrc);

               //Do work for each add/modify node (each file to add or modify)
            foreach (XmlNode lNode in lAddNodes)
            {
                //get file path/name
                string lItem = "";
                foreach (XmlAttribute lAttr in lNode.Attributes)
                {
                    if (lAttr.LocalName == "item")
                    {
                        lItem = lAttr.Value;
                    }
                }
                //get the datapackage name as know by parameters
                string lPkgNameAsItIs = pDPType + "-" + pDPVersionOnGround;
                //do work for each file of the zip
                Enumeration lEnum = lInputZipFile.entries();
                while (lEnum.hasMoreElements())
                {
                    //get a file
                    ZipEntry lCurrEntry = (ZipEntry)lEnum.nextElement();
                    //get the datapackage name as in zip file
                    string lDPFromZip = lCurrEntry.getName().Substring(0, lCurrEntry.getName().IndexOf("/"));
                    //generate the full needed file name as know in zip file
                    string lEntryForComparison = lDPFromZip + "/" + lItem;
                    //check if we need this file or not.
                    bool lIsOnPath = false;
                    //if current item is a file, check if it's this entry
                    if (lInputZipFile.getEntry(lEntryForComparison) != null)
                    {
                        lIsOnPath = String.Equals(lCurrEntry.getName(), lEntryForComparison, StringComparison.OrdinalIgnoreCase);
                    }
                    //if it's a folder, check if it contains this entry
                    else
                    {
                        lIsOnPath = (lCurrEntry.getName().IndexOf(lEntryForComparison, StringComparison.OrdinalIgnoreCase) >= 0);
                    }
                    //if we want this file (don't work on directory, zip files dont store it)
                    if (lIsOnPath && !lCurrEntry.isDirectory())
                    {
                        //create an input stream for extracting (using J# libs)
                        java.io.InputStream lEntryStream = lInputZipFile.getInputStream(lCurrEntry);
                        try
                        {
                            //get parameters for output
                            string lFileName = System.IO.Path.GetFileName(lCurrEntry.getName());
                            string lNewPath = System.IO.Path.Combine(pDataPackageDir, System.IO.Path.GetDirectoryName(lCurrEntry.getName().Substring(lCurrEntry.getName().IndexOf("/") + 1)));
                            //create output dir
                            System.IO.Directory.CreateDirectory(lNewPath);
                            //create output stream on outpput file
                            lNewPath = System.IO.Path.Combine(lNewPath, lFileName);
                            if (false == System.IO.File.Exists( lNewPath ) )
                            {
                                java.io.FileOutputStream lDest = new java.io.FileOutputStream(lNewPath);
                                try
                                {
                                    //extract file from input zip to ouput file
                                    sbyte[] lBuffer = new sbyte[8192];
                                    int lGot;
                                    while ((lGot = lEntryStream.read(lBuffer, 0, lBuffer.Length)) > 0) lDest.write(lBuffer, 0, lGot);
                                }
                                finally
                                {
                                    lDest.close();
                                }
                            }
                        }
                        finally
                        {
                            lEntryStream.close();
                        }
                    }
                }
            }

            // Don't forget the Package.Info.xml file!
            Enumeration lEnumForPkgInfoFile = lInputZipFile.entries();
            while (lEnumForPkgInfoFile.hasMoreElements())
            {
                //get a file
                ZipEntry lCurrEntry = (ZipEntry)lEnumForPkgInfoFile.nextElement();
                string lDPFromZip = lCurrEntry.getName().Substring(0, lCurrEntry.getName().IndexOf("/"));
                //generate the full needed file name as know in zip file
                string lEntryForComparison = lDPFromZip + "/Package.info.xml";
                if (String.Equals(lCurrEntry.getName(), lEntryForComparison, StringComparison.OrdinalIgnoreCase))
                {
                    //create an input stream for extracting (using J# libs)
                    java.io.InputStream lEntryStream = lInputZipFile.getInputStream(lCurrEntry);
                    try
                    {
                        //get parameters for output
                        string lFileName = System.IO.Path.GetFileName(lCurrEntry.getName());
                        string lNewPath = System.IO.Path.Combine(pDataPackageDir, System.IO.Path.GetDirectoryName(lCurrEntry.getName().Substring(lCurrEntry.getName().IndexOf("/") + 1)));
                        //create output dir
                        System.IO.Directory.CreateDirectory(lNewPath);
                        //create output stream on outpput file
                        java.io.FileOutputStream lDest = new java.io.FileOutputStream(System.IO.Path.Combine(lNewPath, lFileName));
                        try
                        {
                            //extract file from input zip to ouput file
                            sbyte[] lBuffer = new sbyte[8192];
                            int lGot;
                            while ((lGot = lEntryStream.read(lBuffer, 0, lBuffer.Length)) > 0) lDest.write(lBuffer, 0, lGot);
                        }
                        finally
                        {
                            lDest.close();
                        }
                    }
                    finally
                    {
                        lEntryStream.close();
                    }
                    break;
                }
            }

        }

        private void generateOldPackageNeededFiles(string pDataPackageDir, string pDPType, string pDPVersionOnBoard, string pDPVersionOnGround, string pDiffManifest)
        {
            //------------------------------------------------------------------------
            //construct list of file from on borad package that needded by new package
            //------------------------------------------------------------------------
            //get list of files that come from new package
            XmlDocument lXmlDocFilesToAvoid = new XmlDocument();
            lXmlDocFilesToAvoid.Load(pDiffManifest);
            XmlElement lXmlRootFilesToAvoid = lXmlDocFilesToAvoid.DocumentElement;
            XmlNodeList lFilesToAvoid = lXmlRootFilesToAvoid.SelectNodes("//add|//modify|//remove");
            List<string> lFilesToZip = new List<string>();
            foreach (XmlNode lFileToAvoid in lFilesToAvoid)
            {
                foreach (XmlAttribute lAttr in lFileToAvoid.Attributes)
                {
                    if (lAttr.Name == "item")
                    {
                        lFilesToZip.Add(lAttr.Value.Replace("/",@"\"));
                    }
                }
            }
            lFilesToZip.Add("Package.info.xml");
            //init the new xml file
            XmlDocument lRefXmlDoc = new XmlDocument();
            XmlDeclaration lRefXmlDec = lRefXmlDoc.CreateXmlDeclaration("1.0", null, null);
            lRefXmlDoc.AppendChild(lRefXmlDec);
            XmlElement lRefXmlRoot = lRefXmlDoc.CreateElement("Diff");
            lRefXmlRoot.SetAttribute("ReferenceDataPackage", pDPVersionOnBoard);
            lRefXmlRoot.SetAttribute("NewDataPackage", pDPVersionOnGround);
            lRefXmlDoc.AppendChild(lRefXmlRoot);
            //get the old zip file
            string lDPRef = _dataStorePath;
            DataContainer lDPChar = getDataPackageCharacteristics(pDPType, pDPVersionOnBoard);
            lDPRef = Path.Combine(lDPRef, lDPChar.getStrValue("DataPackagePath"));
            lDPRef = lDPRef.Replace("/","\\");
            ZipFile lRefZipFile = new ZipFile(lDPRef);
            Enumeration lInputZipFileEnum = lRefZipFile.entries();
            //do work for each file in zip
            while (lInputZipFileEnum.hasMoreElements())
            {
                //get the entry and set the name to be compare to items from new package
                ZipEntry lCurrEntry = (ZipEntry)lInputZipFileEnum.nextElement();
                string lCurrEntryName = lCurrEntry.getName();
                lCurrEntryName = lCurrEntryName.Substring(lCurrEntryName.IndexOf("/"));
                lCurrEntryName = lCurrEntryName.Replace("/", @"\");
                //check if we want this file
                bool lIsAnExtractFile = true;
                for (int i = 0; i < lFilesToZip.Count; i++)
                {
                    string lTmpEntryName = lCurrEntryName.Replace(@"\", "");
                    string lSrcEntryName = lFilesToZip[i].Replace(@"\", "");
                    //if one of the new files/folders is in the entry name
                    if (lSrcEntryName.IndexOf(lTmpEntryName) == 0)
                    {
                        //we don't want it from the old package
                        lIsAnExtractFile = false;
                        break;
                    }
                    else
                    {
                        if (String.Equals(lCurrEntryName, "Package.info.xml", StringComparison.OrdinalIgnoreCase))
                        {
                            lIsAnExtractFile = false;
                            break;
                        }
                    }
                }
                //add the file to the list if we want it and it's not the root dir
                if (!lCurrEntry.isDirectory() && lIsAnExtractFile )
                {
                    XmlElement lRefXmlElement = lRefXmlDoc.CreateElement("File");
                    lRefXmlElement.InnerText = lCurrEntryName;
                    lRefXmlRoot.AppendChild(lRefXmlElement);
                }
            }
            //write the xml
            lRefXmlDoc.Save(Path.Combine(pDataPackageDir, "diff.xml"));
        }

        private void zipPackage(string pDataPackageDir, string pDPType, string pDPVersion)
        {
            //---------------------------
            //Zip the incremental package
            //---------------------------
            //get the new zip full name
            string lZipFile = pDataPackageDir + ".zip";
            if (System.IO.File.Exists(lZipFile))
            {
                System.IO.File.Delete(lZipFile);
            }
            //list all the files to add
            string[] lFilesToZip = Directory.GetFiles(pDataPackageDir, "*.*", SearchOption.AllDirectories);
            //init outputstreams for creating zip content
            FileOutputStream lFOS = new FileOutputStream(pDataPackageDir + ".zip");
            ZipOutputStream lZOS = new ZipOutputStream(lFOS);
            lZOS.setLevel(9);
            //do work for each file
            for (int i = 0; i < lFilesToZip.Length; i++)
            {
                string lSrcFile = lFilesToZip[i];
                //create a stream on current file for reading
                FileInputStream lFIS = new FileInputStream(lSrcFile);
                string lStrEntry;
                //if it's the diff list, we want it in root directory
                if (lSrcFile == Path.Combine(pDataPackageDir, "diff.xml"))
                {
                    lStrEntry = lSrcFile.Replace(pDataPackageDir + @"\", "");
                }
                //else respect the directory struct
                else
                {
                    lStrEntry = pDPType.ToUpperInvariant() + "-" + pDPVersion + @"\" + lSrcFile.Replace(pDataPackageDir + @"\", "");
                }
                //create the new entry and add it to zip file
                ZipEntry lZE = new ZipEntry(lStrEntry);
                lZOS.putNextEntry(lZE);
                //write it
                sbyte[] lBuff = new sbyte[1024];
                int lLen;
                while ((lLen = lFIS.read(lBuff)) >= 0)
                {
                    lZOS.write(lBuff, 0, lLen);
                }

                lFIS.close();

            }

            lZOS.closeEntry();
            lZOS.close();
            lFOS.close();
        }

        private string createManifest(string pDPType, string pDPVersion)
        {
            //create manifest from zip file
            //create temp folder using reqid
            string lDestFile = Path.GetRandomFileName();
            lDestFile = lDestFile.Substring(0, lDestFile.IndexOf('.')) + ".xml";
            lDestFile = Path.Combine(_tmpPath, lDestFile);
            //unzip data package
            string lDPSrc = _dataStorePath;
            DataContainer lDPChar = getDataPackageCharacteristics(pDPType, pDPVersion);
            lDPSrc = Path.Combine(lDPSrc, lDPChar.getStrValue("DataPackagePath"));
            lDPSrc = lDPSrc.Replace("/", "\\");
            ZipFile lInputZipFile = new ZipFile(lDPSrc);
            Dictionary<string, long> lEntries = new Dictionary<string,long>();
            Enumeration lEnum = lInputZipFile.entries();
            while (lEnum.hasMoreElements())
            {
                ZipEntry lEntry = (ZipEntry)lEnum.nextElement();
                string lName = lEntry.getName();
                lName = "/" + lName;
                if (lName.Length != 0 && lName.LastIndexOf("/") == lName.Length - 1)
                {
                    lName = lName.Remove(lName.LastIndexOf("/"));
                }
                if (!String.IsNullOrEmpty(lName.Trim()))
                {
                    long lCRC = lEntry.getTime()+lEntry.getSize();
                    lEntries.Add(lName, lCRC);
                }

            }
            MainDiffClass lDiffCl = new MainDiffClass();
            lDiffCl.mCreateManifestXmlFromDic(lEntries, lDestFile);

            return lDestFile;
        }

        private string getDataPackagePath(string pDPType, string pDPVersion)
        {
            string lDPSrc = _dataStorePath;
            DataContainer lDPChar = getDataPackageCharacteristics(pDPType, pDPVersion);
            lDPSrc = Path.Combine(lDPSrc, lDPChar.getStrValue("DataPackagePath"));
            lDPSrc = lDPSrc.Replace("/", "\\");

            return lDPSrc;
        }

        private string getManifest(string pDPType, string pDPVersion)
        {
            string lDPSrc = getDataPackagePath(pDPType, pDPVersion);
            string lDestFile = Path.GetRandomFileName();
            lDestFile = lDestFile.Substring(0, lDestFile.IndexOf('.')) + ".xml";
            lDestFile = Path.Combine(_tmpPath, lDestFile);

            ZipFile lZipFile = new ZipFile(lDPSrc);

            Enumeration lEnum = lZipFile.entries();
            while (lEnum.hasMoreElements())
            {
                ZipEntry lEntry = (ZipEntry)lEnum.nextElement();
                string lEntryName = lEntry.getName();
                lEntryName = lEntryName.Substring(lEntryName.IndexOf("/")+1);

                if (lEntryName == "Package.info.xml")
                {
                    java.io.InputStream lEntryStream = lZipFile.getInputStream(lEntry);
                    try
                    {
                        java.io.FileOutputStream lDest = new java.io.FileOutputStream(lDestFile);
                        try
                        {
                            sbyte[] lBuffer = new sbyte[8192];
                            int lGot;
                            while ((lGot = lEntryStream.read(lBuffer, 0, lBuffer.Length)) > 0) lDest.write(lBuffer, 0, lGot);
                        }
                        finally
                        {
                            lDest.close();
                        }
                    }
                    finally
                    {
                        lEntryStream.close();
                    }
                    break;
                }
            }

            if (!System.IO.File.Exists(lDestFile))
            {
                lDestFile = createManifest(pDPType, pDPVersion);
            }
            return lDestFile;
        }

        private string openPackage(string pDPType, string pDPVersion, string pFileMatchPattern)
        {
            string lLocalOpenPackagePath = unzipPackage(pDPType, pDPVersion, pFileMatchPattern, _openPackagesPath);

            try
            {
                _dBAccess.mUpdateDataPackageLastOpenInfo(pDPType, pDPVersion, DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.RemoteDataStore.RemoteDataStoreService.openPackage", ex, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);
            }

            return lLocalOpenPackagePath;
        }

        private string unzipPackage(string pDPType, string pDPVersion, string pRegexFileMatchPattern, string pTargetDir)
        {
            string lOpenPackagePath = string.Empty;

            LogManager.WriteLog(TraceType.INFO, "unzipPackage called " + pDPType + "," + pDPVersion + "," + pTargetDir, "PIS.Ground.RemoteDataStore.RemoteDataStoreService.unzipPackage", null, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);

            lock (_unzipPackageLock)
            {
                try
                {
                    string lDPSrc = _dataStorePath;
                    DataContainer lDPChar = getDataPackageCharacteristics(pDPType, pDPVersion);
                    lDPSrc = Path.Combine(lDPSrc, lDPChar.getStrValue("DataPackagePath"));
                    lDPSrc = lDPSrc.Replace("/", "\\");

                    string lDestDir = Path.Combine(
                        pTargetDir,
                        lDPChar.getStrValue("DataPackageType") + "_" + lDPChar.getStrValue("DataPackageVersion") + "\\");
                    

                    LogManager.WriteLog(TraceType.INFO, "unzipPackage From=" + lDPSrc + ", To=" + lDestDir + ", Pattern=" + pRegexFileMatchPattern,
                        "PIS.Ground.RemoteDataStore.RemoteDataStoreService.unzipPackage", null, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);

                    sbyte[] lBuffer = new sbyte[0x80000]; // 512 kB

                    ZipFile lZipFile = new ZipFile(lDPSrc);
                    try
                    {
                        Enumeration lEnum = lZipFile.entries();

                        while (lEnum.hasMoreElements())
                        {
                            ZipEntry lEntry = (ZipEntry)lEnum.nextElement();
                            string lEntryName = lEntry.getName();

                            if (!lEntry.isDirectory())
                            {
                                if (string.IsNullOrEmpty(pRegexFileMatchPattern) || Regex.Match(lEntryName, pRegexFileMatchPattern, RegexOptions.IgnoreCase).Success)
                                {
                                    string targetFile = Path.Combine(
                                        lDestDir + Path.GetDirectoryName(lEntryName),
                                        Path.GetFileName(lEntryName));

                                    if (!System.IO.File.Exists(targetFile))
                                    {
                                        java.io.InputStream lEntryStream = lZipFile.getInputStream(lEntry);
                                        try
                                        {
                                            Directory.CreateDirectory(lDestDir + Path.GetDirectoryName(lEntryName));

                                            FileOutputStream lOutStream = new FileOutputStream(targetFile);
                                            try
                                            {
                                                int lGot;
                                                while ((lGot = lEntryStream.read(lBuffer, 0, lBuffer.Length)) > 0) lOutStream.write(lBuffer, 0, lGot);
                                            }
                                            finally
                                            {
                                                lOutStream.close();
                                            }
                                        }
                                        finally
                                        {
                                            lEntryStream.close();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        lZipFile.close();
                    }                    

                    lOpenPackagePath = lDestDir;
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.RemoteDataStore.RemoteDataStoreService.unzipPackage", ex, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);
                }
            }

            return lOpenPackagePath;
        }

        public void assignAFutureBaselineToElement(Guid pReqID, string pEID, string pBLVersion, DateTime pActDate, DateTime pExpDate)
        {
            _dataPackageCallbackClient.updateBaselineAssignmentStatus(pReqID, NotificationIdEnum.DataPackageBaselineAssignmentProcessing, pEID, pBLVersion);
            if (String.IsNullOrEmpty(pEID))
            {
                throw new FaultException("Invalid element id", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_ELEMENT_ID.ToString()));
            }
            if (!mVersionMatch(pBLVersion))
            {
                throw new FaultException("Baseline version is invalid", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_BASELINE_VERSION.ToString()));
            }
            if (pActDate == null)
            {
                throw new FaultException("Bad activation date", new FaultCode(RemoteDataStoreExceptionCodeEnum.BAD_ACTIVATION_DATE.ToString()));
            }
            if (pExpDate == null)
            {
                throw new FaultException("Bad activation date", new FaultCode(RemoteDataStoreExceptionCodeEnum.BAD_EXPIRATION_DATE.ToString()));
            }

            if (!checkIfBaselineExists(pBLVersion))
            {
                //Send callback to DataPackage
                _dataPackageCallbackClient.updateBaselineAssignmentStatus(pReqID, NotificationIdEnum.DataPackageBaselineAssignmentFailed, pEID, pBLVersion);
            }
            else
            {
                _dBAccess.mAssignFutureBaselineToElement(pEID, pBLVersion, pActDate, pExpDate);
                checkDataPackagesAvailability(pReqID, getBaselineDefinition(pBLVersion));
                //Send callback to DataPackage
                _dataPackageCallbackClient.updateBaselineAssignmentStatus(pReqID, NotificationIdEnum.DataPackageBaselineAssignmentCompleted, pEID, pBLVersion);
            }
        }

        public void assignACurrentBaselineToElement(Guid pReqID, string pEID, string pBLVersion, DateTime pExpDate)
        {
            _dataPackageCallbackClient.updateBaselineAssignmentStatus(pReqID, NotificationIdEnum.DataPackageBaselineAssignmentProcessing, pEID, pBLVersion);
            if (String.IsNullOrEmpty(pEID))
            {
                throw new FaultException("Invalid element id", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_ELEMENT_ID.ToString()));
            }
            if (!mVersionMatch(pBLVersion))
            {
                throw new FaultException("Baseline version is invalid", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_BASELINE_VERSION.ToString()));
            }
            if (pExpDate == null)
            {
                throw new FaultException("Bad activation date", new FaultCode(RemoteDataStoreExceptionCodeEnum.BAD_EXPIRATION_DATE.ToString()));
            }

            if (!checkIfBaselineExists(pBLVersion))
            {
                //Send callback to DataPackage
                _dataPackageCallbackClient.updateBaselineAssignmentStatus(pReqID, NotificationIdEnum.DataPackageBaselineAssignmentFailed, pEID, pBLVersion);
            }
            else
            {
                _dBAccess.mAssignCurrentBaselineToElement(pEID, pBLVersion, pExpDate);
                checkDataPackagesAvailability(pReqID, getBaselineDefinition(pBLVersion));
                //Send callback to DataPackage
                _dataPackageCallbackClient.updateBaselineAssignmentStatus(pReqID, NotificationIdEnum.DataPackageBaselineAssignmentCompleted, pEID, pBLVersion);
            }
        }

        public void deleteBaselineFile(Guid pReqId, string pEID)
        {
            string lDirToDel = _dataStorePath + @"\BaselinesDefinitions\" + pReqId.ToString();
            Directory.Delete(lDirToDel + @"\" + pEID, true);
            try
            {
                Directory.Delete(lDirToDel);
            }
            catch(System.IO.IOException)
            {
            }
        }

        public void deleteDataPackage(Guid pReqId, string pDPType, string pDPVersion)
        {
            if ( checkIfDataPackageExists(pDPType, pDPVersion) )
            {
                DataContainer lBLDef = _dBAccess.mGetDataPackageCharacteristics(pDPType, pDPVersion);

                string lFileToDel = _dataStorePath + @"\" + lBLDef.getStrValue("DataPackagePath");

                try
                {
                    System.IO.File.Delete(lFileToDel);
                }
                catch (System.IO.IOException e) 
                {
                    string lErr = "Failed to delete  [" + lFileToDel + "]. Reason - " + e.Message;
                    LogManager.WriteLog(TraceType.ERROR, lErr, "PIS.Ground.RemoteDataStore.RemoteDataStoreService.deleteDataPackage", null, PIS.Ground.Core.Data.EventIdEnum.RemoteDataStore);
                }
                                
                _dBAccess.mDeleteDataPackage(pDPType, pDPVersion);
            }
        }

        public void deleteDataPackageDiffFile(Guid pReqId, string pEID)
        {
            string lDirToDel = _dataStorePath + @"\IncrementalDataPackages\" + pReqId.ToString();
            Directory.Delete(lDirToDel + @"\" + pEID, true);
            try
            {
                Directory.Delete(lDirToDel);
            }
            catch (System.IO.IOException)
            {
            }
        }

        public void setElementUndefinedBaselineParams(
            string pEID,
            string pPisBasePackageVersion,
            string pPisMissionPackageVersion,
            string pPisInfotainmentPackageVersion,
            string pLmtPackageVersion)
        {
            if (String.IsNullOrEmpty(pEID))
            {
                throw new FaultException("Invalid element id", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_ELEMENT_ID.ToString()));
            }

            _dBAccess.mSetElementUndefinedBaselineParams(pEID, pPisBasePackageVersion, pPisMissionPackageVersion, pPisInfotainmentPackageVersion, pLmtPackageVersion);
        }

        public DataContainer getElementsDescription()
        {
            return _dBAccess.mGetElementsDescription();
        }

        public DataContainer getUndefinedBaselinesList()
        {
            return _dBAccess.mGetUndefinedBaselinesList();
        }

        public void unassignFutureBaselineFromElement(string pEID)
        {            
            if (String.IsNullOrEmpty(pEID))
            {
                throw new FaultException("Invalid element id", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_ELEMENT_ID.ToString()));
            }

            _dBAccess.mUnassignFutureBaselineFromElement(pEID);
        }

        public void unassignCurrentBaselineFromElement(string pEID)
        {
            if (String.IsNullOrEmpty(pEID))
            {
                throw new FaultException("Invalid element id", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_ELEMENT_ID.ToString()));
            }

            _dBAccess.mUnassignCurrentBaselineFromElement(pEID);
		}

		#region Baseline distribution requests processing

		/// <summary>Saves a baseline distributing request.</summary>
		/// <param name="baselineDistributingTask">The baseline distributing task.</param>
		public void saveBaselineDistributingRequest(DataContainer baselineDistributingTask)
		{
			if (baselineDistributingTask == null)
			{
				throw new ArgumentNullException("baselineDistributingTask");
			}

			_dBAccess.mSaveBaselineDistributingTask(baselineDistributingTask);
		}

		/// <summary>Gets all baseline distributing saved requests.</summary>
		/// <returns>all baseline distributing saved requests.</returns>
		public DataContainer getAllBaselineDistributingSavedRequests()
		{
			return _dBAccess.mGetBaselineDistributingTasks();
		}

		/// <summary>Deletes the baseline distributing request.</summary>
		/// <param name="elementId">Identifier for the element.</param>
		public void deleteBaselineDistributingRequest(string elementId)
		{
			if (string.IsNullOrEmpty(elementId))
			{
				throw new ArgumentNullException("elementId");
			}

			_dBAccess.mDeleteBaselineDistributingTask(elementId);
		}

		#endregion

        #region IRemoteDataStore Members

        void IRemoteDataStore.moveTheNewDataPackageFiles(Guid pReqID, string pFileURL)
        {
            throw new NotImplementedException();
        }

        bool IRemoteDataStore.checkUrl(Guid pReqID, string pUrl)
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.checkDataPackagesAvailability(Guid pRequestID, DataContainer pBLDef)
        {
            throw new NotImplementedException();
        }

        bool IRemoteDataStore.checkIfDataPackageExists(string pDPType, string pDPVersion)
        {
            throw new NotImplementedException();
        }

        bool IRemoteDataStore.checkIfBaselineExists(string pBLVersion)
        {
            throw new NotImplementedException();
        }

        bool IRemoteDataStore.checkIfElementExists(string pEID)
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.setNewDataPackage(DataContainer pNDPkg)
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.setNewBaselineDefinition(Guid pRequestID, DataContainer pNBLDef)
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.saveBaselineDistributingRequest(DataContainer baselineDistributingTask)
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.deleteBaselineDefinition(string pVersion)
        {
            throw new NotImplementedException();
        }

        DataContainer IRemoteDataStore.getBaselinesDefinitions()
        {
            throw new NotImplementedException();
        }

        DataContainer IRemoteDataStore.getBaselineDefinition(string pBLVersion)
        {
            throw new NotImplementedException();
        }

        DataContainer IRemoteDataStore.getAssignedBaselinesVersions()
        {
            throw new NotImplementedException();
        }

        string IRemoteDataStore.getAssignedCurrentBaselineVersion(string pEID)
        {
            throw new NotImplementedException();
        }

        string IRemoteDataStore.getAssignedFutureBaselineVersion(string pEID)
        {
            throw new NotImplementedException();
        }

        DataContainer IRemoteDataStore.getDataPackages(DataContainer pBLDef)
        {
            throw new NotImplementedException();
        }

        DataContainer IRemoteDataStore.getDataPackagesList()
        {
            throw new NotImplementedException();
        }

        DataContainer IRemoteDataStore.getDataPackageCharacteristics(string pDPType, string pDPVersion)
        {
            throw new NotImplementedException();
        }

        DataContainer IRemoteDataStore.getCurrentBaselineDefinition(string pEID)
        {
            throw new NotImplementedException();
        }

        DataContainer IRemoteDataStore.getElementBaselinesDefinitions(string pEID)
        {
            throw new NotImplementedException();
        }

        DataContainer IRemoteDataStore.getAllBaselineDistributingSavedRequests()
        {
            throw new NotImplementedException();
        }

        string IRemoteDataStore.createBaselineFile(Guid pReqId, string pEID, string pBLVersion, string pActivationDate, string pExpirationDate)
        {
            throw new NotImplementedException();
        }

        string IRemoteDataStore.getDiffDataPackageUrl(Guid pReqID, string pEID, string pDPType, string pDPVersionOnBoard, string pDPVersionOnGround)
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.assignAFutureBaselineToElement(Guid ReqID, string pEID, string pBLVersion, DateTime pActDate, DateTime pExpDate)
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.assignACurrentBaselineToElement(Guid ReqID, string pEID, string pBLVersion, DateTime pExpDate)
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.deleteBaselineFile(Guid pReqId, string pEID)
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.deleteDataPackage(Guid pReqId, string pDPType, string pDPVersion)
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.deleteDataPackageDiffFile(Guid pReqId, string pEID)
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.deleteBaselineDistributingRequest(string elementId)
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.setElementUndefinedBaselineParams(string pEID, string pPisBasePackageVersion, string pPisMissionPackageVersion, string pPisInfotainmentPackageVersion, string pLmtPackageVersion)
        {
            throw new NotImplementedException();
        }

        DataContainer IRemoteDataStore.getElementsDescription()
        {
            throw new NotImplementedException();
        }

        DataContainer IRemoteDataStore.getUndefinedBaselinesList()
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.unassignFutureBaselineFromElement(string pEID)
        {
            throw new NotImplementedException();
        }

        void IRemoteDataStore.unassignCurrentBaselineFromElement(string pEID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (_dataPackageCallbackClient != null)
            {
                bool success = false;
                try
                {
                    if (_dataPackageCallbackClient.State != CommunicationState.Faulted)
                    {
                        _dataPackageCallbackClient.Close();
                        success = true;
                    }
                }
                finally
                {
                    if (!success)
                    {
                        _dataPackageCallbackClient.Abort();
                    }
                }

            }
        }

        #endregion
    }

    public class RemoteDataStoreWinService : ServiceBase
    {
        public ServiceHost host = null;
        public RemoteDataStoreWinService()
        {
            // Name the Windows Service
            ServiceName = "RemoteDataStoreService";
        }
        public static void Main()
        {
            ServiceBase.Run(new RemoteDataStoreWinService());
            
            //NOTE : If you need to run RemoteDataStore in debug mode in visual studio on your local computer, use the following code instead of the previous line.
            //The following code as been commented as service installation in debug mode does not work with it.
            /*
            RemoteDataStoreWinService service = new RemoteDataStoreWinService();
            service.OnStart(null);

            int r = 0;
            while (r != (int)'q')
            {
                System.Threading.Thread.Sleep(1000);
                r = Console.Read();
            }

            service.OnStop();
            service.OnShutdown();
            */
        }

        // Start the Windows service.
        protected override void OnStart(string[] args)
        {
            if (host != null)
            {
                host.Close();
            }

            //Create a URI to serve as the base address
            Uri httpURL = new Uri("http://localhost:8070/RemoteDataStoreService");

            //Create ServiceHost
            host = new ServiceHost(typeof(PIS.Ground.RemoteDataStore.RemoteDataStoreService), httpURL);

            //Add a service endpoint
            host.AddServiceEndpoint(typeof(PIS.Ground.RemoteDataStore.IRemoteDataStore), new BasicHttpBinding(), "");

            //Enable metadata exchange
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;

            host.Description.Behaviors.Add(smb);
            host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;

            //Start the Service

            host.Open();
        }

        protected override void OnStop()
        {
            if (host != null)
            {
                host.Close();
                host = null;
            }
        }

    }

    // Provide the ProjectInstaller class which allows
    // the service to be installed by the Installutil.exe tool
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "RemoteDataStoreService";
            service.StartType = ServiceStartMode.Automatic;
            Installers.Add(process);
            Installers.Add(service);

            // Register to receive AfterInstall event in order to start the service right away.
            this.AfterInstall += new InstallEventHandler(AfterInstallCallback);
        }

        // Once AfterInstallCallback received,  start the service.
        void AfterInstallCallback(object sender, InstallEventArgs e)
        {
            ServiceController serviceCtrl = new ServiceController("RemoteDataStoreService");
            serviceCtrl.Start();
        }
    }

}