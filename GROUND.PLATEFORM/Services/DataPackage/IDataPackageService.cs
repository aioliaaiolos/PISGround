    using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using PIS.Ground.Core.Data;

namespace PIS.Ground.DataPackage
{
    [ServiceContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/", Name = "DataPackageService")]
    public interface IDataPackageService
    {
        [OperationContract]
        DataPackageResult uploadDataPackages(Guid pSessionId, List<string> pFilesURLs);

        [OperationContract]
        DataPackageResult defineNewBaseline(Guid pSessionId, BaselineDefinition pBLDef);

		[OperationContract]
		DataPackageErrorEnum deleteBaselineDefinition(Guid pSessionId, string pVersion);

		[OperationContract]
        DataPackageResult GetAvailableElementDataList(Guid pSessionId, out ElementList<AvailableElementData> pElementDataList);

        [OperationContract]
        DataPackageErrorEnum unassignCurrentBaselineFromElement(Guid pSessionId, string pElementId);

        [OperationContract]
        DataPackageErrorEnum unassignFutureBaselineFromElement(Guid pSessionId, string pElementId);        

        [OperationContract]
        DataPackageErrorEnum getDataPackagesVersionsList(Guid pSessionId, DataPackageType pPackageType, out DataPackagesVersionsList pVersionsList);

        [OperationContract]
        DataPackageResult deleteDataPackage(Guid pSessionId, DataPackageType pPackageType, string pPackageVersion, bool pForceDeleting);
		
        [OperationContract]
        DataPackageResult removeBaseline(Guid pSessionId, string pVersion);

        [OperationContract]
		GetBaselineListResult getBaselinesList(Guid pSessionId, BaselinesListType pListType);

		[OperationContract]
        GetAdresseesDataPackageBaselinesResult getAddresseesDataPackagesBaselines(Guid pSessionId, TargetAddressType pTargetAddress, TargetAddressType pElementAddress);

		[OperationContract]
        DataPackageResult assignFutureBaselineToElement(Guid pSessionId, TargetAddressType pElementAddress, string pElementId, string pBLVersion, DateTime pActDate, DateTime pExpDate);

		[OperationContract]
        DataPackageResult assignCurrentBaselineToElement(Guid pSessionId, TargetAddressType pElementAddress, string pElementId, string pBLVersion, DateTime pExpDate);

		[OperationContract]
        DataPackageResult forceAddresseesFutureBaseline(Guid pSessionId, TargetAddressType pElementAddress, string pElementId, uint pReqTimeout);

		[OperationContract]
        DataPackageResult forceAddresseesArchivedBaseline(Guid pSessionId, TargetAddressType pElementAddress, string pElementId, uint pReqTimeout);

		[OperationContract]
        DataPackageResult clearAddreeseesForcingStatus(Guid pSessionId, TargetAddressType pElementAddress, string pElementId, uint pReqTimeout);
		
		[OperationContract]
        DataPackageResult distributeBaseline(Guid pSessionId, TargetAddressType pElementAddress, TargetAddressType pTargetAddress, BaselineDistributionAttributes pBLAttributes, bool pIncr);

	}


    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/", Name = "ElementBaseline")]
    public class ElementBaseline
    {
        private string _baselineVersion;
        private bool _baselineValidity;
        private string _baselineDescription;
        private string _pISBaseDataPackageVersion; 
        private bool _pISBaseDataPackageValidity; 
        private string _pISMissionDataPackageVersion; 
        private bool _pISMissionDataPackageValidity; 
        private string _pISInfotainmentDataPackageVersion; 
        private bool _pISInfotainmentDataPackageValidity; 
        private string _lMTDataPackageVersion;
        private bool _lMTDataPackageValidity;

        [DataMember]
        public string BaselineVersion { get { return _baselineVersion; } set {_baselineVersion = value; } }

        [DataMember]
        public string BaselineDescription { get { return _baselineDescription; } set { _baselineDescription = value; } }

        [DataMember]
        public bool BaselineValidity { get { return _baselineValidity; } set { _baselineValidity = value; } }

        [DataMember]
        public string PISBaseDataPackageVersion { get { return _pISBaseDataPackageVersion ?? (_pISBaseDataPackageVersion = ""); } set { _pISBaseDataPackageVersion = value; } }

        [DataMember]
        public bool PISBaseDataPackageValidity { get { return _pISBaseDataPackageValidity; } set { _pISBaseDataPackageValidity = value; } }

        [DataMember]
        public string PISMissionDataPackageVersion { get { return _pISMissionDataPackageVersion ?? (_pISMissionDataPackageVersion = ""); } set { _pISMissionDataPackageVersion = value; } }

        [DataMember]
        public bool PISMissionDataPackageValidity { get { return _pISMissionDataPackageValidity; } set { _pISMissionDataPackageValidity = value; } }

        [DataMember]
        public string PISInfotainmentDataPackageVersion { get { return _pISInfotainmentDataPackageVersion ?? (_pISInfotainmentDataPackageVersion = ""); } set { _pISInfotainmentDataPackageVersion = value; } }

        [DataMember]
        public bool PISInfotainmentDataPackageValidity { get { return _pISInfotainmentDataPackageValidity; } set { _pISInfotainmentDataPackageValidity = value; } }

        [DataMember]
        public string LMTDataPackageVersion { get { return _lMTDataPackageVersion ?? (_lMTDataPackageVersion = ""); } set { _lMTDataPackageVersion = value; } }

        [DataMember]
        public bool LMTDataPackageValidity { get { return _lMTDataPackageValidity; } set { _lMTDataPackageValidity = value; } }
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/", Name = "ElementDescription")]
    public class ElementDescription
    {
        private string _elementID;
        private ElementBaseline _elementArchivedBaseline;
        private DateTime _elementArchivedBaselineExpirationDate;
        private ElementBaseline _elementCurrentBaseline;
        private DateTime _elementCurrentBaselineExpirationDate;
        private bool _isCurrentBaselineForced;
        private ElementBaseline _elementFutureBaseline;
        private DateTime _elementFutureBaselineActivationDate;
        private DateTime _elementFutureBaselineExpirationDate;
        private string _assignedCurrentBaseline;
        private DateTime _assignedCurrentBaselineExpirationDate;
        private string _assignedFutureBaseline;
        private DateTime _assignedFutureBaselineActivationDate;
        private DateTime _assignedFutureBaselineExpirationDate;
        private string _undefinedBaselinePisBaseVersion;
        private string _undefinedBaselinePisMissionVersion;
        private string _undefinedBaselinePisInfotainmentVersion;
        private string _undefinedBaselineLmtVersion;

        [DataMember]
        public string ElementID { get { return _elementID ?? (_elementID = ""); } set { _elementID = value; } }

        [DataMember]
        public ElementBaseline ElementArchivedBaseline { get { return _elementArchivedBaseline ?? (_elementArchivedBaseline = new ElementBaseline()); } set { _elementArchivedBaseline = value; } }

        [DataMember]
        public DateTime ElementArchivedBaselineExpirationDate { get { return _elementArchivedBaselineExpirationDate; } set { _elementArchivedBaselineExpirationDate = value; } }

        [DataMember]
        public ElementBaseline ElementCurrentBaseline { get { return _elementCurrentBaseline ?? (_elementCurrentBaseline = new ElementBaseline()); } set { _elementCurrentBaseline = value; } }

        [DataMember]
        public DateTime ElementCurrentBaselineExpirationDate { get { return _elementCurrentBaselineExpirationDate; } set { _elementCurrentBaselineExpirationDate = value; } }

        [DataMember]
        public bool isCurrentBaselineForced { get { return _isCurrentBaselineForced; } set { _isCurrentBaselineForced = value; } }

        [DataMember]
        public ElementBaseline ElementFutureBaseline { get { return _elementFutureBaseline ?? (_elementFutureBaseline = new ElementBaseline()); } set { _elementFutureBaseline = value; } }

        [DataMember]
        public DateTime ElementFutureBaselineActivationDate { get { return _elementFutureBaselineActivationDate; } set { _elementFutureBaselineActivationDate = value; } }

        [DataMember]
        public DateTime ElementFutureBaselineExpirationDate { get { return _elementFutureBaselineExpirationDate; } set { _elementFutureBaselineExpirationDate = value; } }

        [DataMember]
        public string AssignedCurrentBaseline { get { return _assignedCurrentBaseline ?? ( _assignedCurrentBaseline = ""); } set { _assignedCurrentBaseline = value; } }

        [DataMember]
        public DateTime AssignedCurrentBaselineExpirationDate { get { return _assignedCurrentBaselineExpirationDate; } set { _assignedCurrentBaselineExpirationDate = value; } }

        [DataMember]
        public string AssignedFutureBaseline { get { return _assignedFutureBaseline ?? (_assignedFutureBaseline = ""); } set { _assignedFutureBaseline = value; } }

        [DataMember]
        public DateTime AssignedFutureBaselineActivationDate { get { return _assignedFutureBaselineActivationDate; } set { _assignedFutureBaselineActivationDate = value; } }

        [DataMember]
        public DateTime AssignedFutureBaselineExpirationDate { get { return _assignedFutureBaselineExpirationDate; } set { _assignedFutureBaselineExpirationDate = value; } }

        [DataMember]
        public string UndefinedBaselinePisBaseVersion { get { return _undefinedBaselinePisBaseVersion; } set { _undefinedBaselinePisBaseVersion = value; } }

        [DataMember]
        public string UndefinedBaselinePisMissionVersion { get { return _undefinedBaselinePisMissionVersion; } set { _undefinedBaselinePisMissionVersion = value; } }

        [DataMember]
        public string UndefinedBaselinePisInfotainmentVersion { get { return _undefinedBaselinePisInfotainmentVersion; } set { _undefinedBaselinePisInfotainmentVersion = value; } }

        [DataMember]
        public string UndefinedBaselineLmtVersion { get { return _undefinedBaselineLmtVersion; } set { _undefinedBaselineLmtVersion = value; } }
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/", Name = "BaselineDefinition")]
    public class BaselineDefinition
    {
        private string _baselineVersion;
        private string _baselineDescription;
        private DateTime _baselineCreationDate;
        private string _pISBaseDataPackageVersion;
        private string _pISMissionDataPackageVersion;
        private string _pISInfotainmentDataPackageVersion;
        private string _lMTDataPackageVersion;

        [DataMember]
        public string BaselineVersion { get { return _baselineVersion ?? (_baselineVersion = ""); } set { _baselineVersion = value; } }

        [DataMember]
        public string BaselineDescription { get { return _baselineDescription ?? (_baselineDescription = ""); } set { _baselineDescription = value; } }

        [DataMember]
        public DateTime BaselineCreationDate { get { return _baselineCreationDate; } set { _baselineCreationDate = value; } }

        [DataMember]
        public string PISBaseDataPackageVersion { get { return _pISBaseDataPackageVersion ?? (_pISBaseDataPackageVersion = ""); } set { _pISBaseDataPackageVersion = value; } }

        [DataMember]
        public string PISMissionDataPackageVersion { get { return _pISMissionDataPackageVersion ?? (_pISMissionDataPackageVersion = ""); } set { _pISMissionDataPackageVersion = value; } }

        [DataMember]
        public string PISInfotainmentDataPackageVersion { get { return _pISInfotainmentDataPackageVersion ?? (_pISInfotainmentDataPackageVersion = ""); } set { _pISInfotainmentDataPackageVersion = value; } }

        [DataMember]
        public string LMTDataPackageVersion { get { return _lMTDataPackageVersion ?? (_lMTDataPackageVersion = ""); } set { _lMTDataPackageVersion = value; } }
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage", Name = "GetBaselineListResult")]
    public class GetBaselineListResult
    {
        private DataPackageErrorEnum _error_code;
        private List<BaselineDefinition> _baselineDef;

        [DataMember]
        public DataPackageErrorEnum error_code { get { return _error_code; } set { _error_code = value; } }

        [DataMember]
        public List<BaselineDefinition> baselineDef { get { return _baselineDef ?? ( _baselineDef = new List<BaselineDefinition>()); } set { _baselineDef = value; } }
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/", Name = "GetAdresseesDataPackageBaselinesResult")]
    public class GetAdresseesDataPackageBaselinesResult
    {
        private DataPackageErrorEnum _error_code;
        private List<ElementDescription> _elementDesc;

        [DataMember]
        public DataPackageErrorEnum error_code { get { return _error_code; } set { _error_code = value; } }

        [DataMember]
        public List<ElementDescription> ElementDesc { get { return _elementDesc ?? ( _elementDesc = new List<ElementDescription>()); } set { _elementDesc = value; } }
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/", Name = "DataPackageResult")]
	public class DataPackageResult
    {
        private DataPackageErrorEnum _error_code;
        private Guid _reqId;

        [DataMember]
        public DataPackageErrorEnum error_code { get { return _error_code; } set { _error_code = value; } }

        [DataMember]
        public Guid reqId { get { return _reqId; } set { _reqId = value; } }
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/", Name = "BaselineDistributionAttributes")]
    public class BaselineDistributionAttributes
    {
        private FileTransferMode _transferMode;
        private bool _fileCompression;
        private DateTime _transferDate;
        private DateTime _transferExpirationDate;
        private sbyte _priority;

        [DataMember]
        public FileTransferMode TransferMode { get { return _transferMode; } set { _transferMode = value; } }

        [DataMember]
        public bool fileCompression { get { return _fileCompression; } set { _fileCompression = value; } }

        [DataMember]
        public DateTime transferDate { get { return _transferDate; } set { _transferDate = value; } }

        [DataMember]
        public DateTime transferExpirationDate { get { return _transferExpirationDate; } set { _transferExpirationDate = value; } }

        [DataMember]
        public sbyte priority { get { return _priority; } set { _priority = value; } }
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/", Name = "DataPackagesCharacteristics")]
    public class DataPackagesCharacteristics
    {
        private DataPackageType _dataPackageType;
        private string _dataPackageVersion;
        private string _dataPackagePath;

        [DataMember]
        public DataPackageType DataPackageType { get { return _dataPackageType; } set { _dataPackageType = value; } }

        [DataMember]
        public string DataPackageVersion { get { return _dataPackageVersion ?? ( _dataPackageVersion = ""); } set { _dataPackageVersion = value; } }

        [DataMember]
        public string DataPackagePath { get { return _dataPackagePath ?? (_dataPackagePath = ""); } set { _dataPackagePath = value; } }
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/", Name = "DataPackageOfflineElements")]
    public class DataPackageOfflineElements
    {
        private string _elementId;
        private long _elementTimeStamp;
        private uint _elementReqIdTimeout;

        [DataMember]
        public string ElementId { get { return _elementId ?? (_elementId = ""); } set { _elementId = value; } }

        [DataMember]
        public long ElementTimeStamp { get { return _elementTimeStamp; } set { _elementTimeStamp = value; } }

        [DataMember]
        public uint ElementReqIdTimeout { get { return _elementReqIdTimeout; } set { _elementReqIdTimeout = value; } }
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage", Name = "DataPackagesVersionsList")]
    public class DataPackagesVersionsList
    {        
        private DataPackageType _dataPackageType;
        private List<string> _versionsList;

        [DataMember]
        public DataPackageType DataPackageType { get { return _dataPackageType; } set { _dataPackageType = value; } }

        [DataMember]
        public List<string> VersionsList { get { return _versionsList ?? (_versionsList = new List<string>()); } set { _versionsList = value; } }
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/", Name = "DataPackageErrorEnum")]
    public enum DataPackageErrorEnum : uint
    {
        //All
        [EnumMember]
        ERROR = 0,
        [EnumMember]
        REQUEST_ACCEPTED = 1,
        [EnumMember]
        INVALID_SESSION_ID = 2,
        [EnumMember]
        REMOTEDATASTORE_NOT_ACCESSIBLE = 3,
        [EnumMember]
        T2G_SERVER_OFFLINE = 4,

        //define new baseline
        [EnumMember]
        INVALID_BASELINE_VERSION = 10,
        [EnumMember]
        INVALID_PIS_MISSION_DATA_PACKAGE_VERSION = 11,
        [EnumMember]
        INVALID_PIS_INFOTAINMENT_DATA_PACKAGE_VERSION = 12,
        [EnumMember]
        INVALID_LMT_DATA_PACKAGE_VERSION = 13,
        [EnumMember]
        INVALID_PIS_BASE_DATA_PACKAGE_VERSION = 14,

        //get adressees data packages
        [EnumMember]
        ELEMENT_ID_NOT_FOUND = 20,
        [EnumMember]
        INVALID_MISSION_ID = 21,
        [EnumMember]
        SERVICE_INFO_NOT_FOUND = 22,

        //assign future baseline to element
        [EnumMember]
        INVALID_EXPIRATION_DATEANDTIME = 30,

        //upload data package
        [EnumMember]
        FILE_NOT_FOUND = 40,
        [EnumMember]
        INVALID_PATH = 41,
        [EnumMember]
        FILE_CURRENTLY_DOWNLOADING = 42,

        //distribute baselines
        [EnumMember]
        INVALID_TRANSFER_MODE = 50,
        [EnumMember]
        INVALID_TIMEOUT = 51,

        //delete baseline        
        [EnumMember]
        BASELINE_IS_ASSIGNED = 61,
        [EnumMember]
        SOME_BASELINES_ARE_ASSIGNED = 62,
        [EnumMember]
        BASELINE_NOT_FOUND = 63,

        //delete data package
        [EnumMember]
        DATA_PACKAGE_IS_USED = 70,
        [EnumMember]
        DATA_PACKAGE_IS_ASSIGNED = 71,
        [EnumMember]
        DATA_PACKAGE_INVALID_VERSION = 72,
        [EnumMember]
        DATA_PACKAGE_NOT_FOUND = 73,
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/", Name = "DataPackageType")]
    public enum DataPackageType : uint
    {
        [EnumMember]
        PISBASE = 0,
        [EnumMember]
        PISMISSION = 1,
        [EnumMember]
        PISINFOTAINMENT = 2,
        [EnumMember]
        LMT = 3
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/ground/datapackage/", Name = "BaselinesListType")]
    public enum BaselinesListType : uint
    {
        [EnumMember]
        ALL = 0,
        [EnumMember]
        DEFINED = 1,
        [EnumMember]
        UNDEFINED = 2        
    }
}
