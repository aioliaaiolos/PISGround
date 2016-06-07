/// 
namespace PIS.Ground.Core.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public enum eServiceID
    {
        // no service associated
        eSrv_NULL = 0,

        // all DRCS services and sub-services regardless of equipment
        eSrvDRCS_RadioNode = 1,
        eSrvDRCS_Station = 2,
        eSrvDRCS_OCC = 3,
        eSrvDRCS_SecSrv = 4,
        eSrvDRCS_Shell = 5,
        eSrvDRCS_Forwarder = 6,
        eSrvDRCS_HorzRoaming = 7,
        eSrvDRCS_VertRoaming = 8,
        eSrvDRCS_Security = 9,
        eSrvDRCS_Transactional = 10,
        eSrvDRCS_eTrain = 11,
        eSrvDRCS_MMC = 12,
        eSrvDRCS_Last,

        // Digital PA services
        eSrvDPA_DPC = 100,
        eSrvDPA_DPA = 101,
        eSrvDPA_DPU = 102,
        eSrvDPA_DPI = 103,
        eSrvDPA_Last,

        // PIS services
        eSrvPIS_ = 200,
        eSysMsgCentralMngr = 201,
        eSrvIPCCentral = 202,
        eSrvPIS_Last,

        // IPCCTV Services	 ( 300..399 )
        eSrvIPCCTV_LiveView = 300,  // allows live viewing of IPCCTV ( Monitor / OCU / DDU... )
        eSrvIPCCTV_Review = 301,  // allows re-viewing of IPCCTV ( pre-recorded )
        // NVR
        eSrvIPCCTV_DATASTORE = 320,	 // allows datastore access ( search, browse, get files, etc. )
        eSrvIPCCTV_METADATASVR = 321,	 // metadata server for streams viewers or cameras (metadata authentication)
        eSrvIPCCTV_ONBOARDRECORDER = 322,	 // on-board stream recorder
        // Streams
        eSrvIPCCTV_CAM_MPEG4 = 380,  // IP-CCTV Cameras MPEG4 stream by RTSP (model 2007)
        eSrvIPCCTV_CAM_H264 = 381,  // IP-CCTV H264 Cameras  (model 2008)
        eSrvIPCCTV_Last,

        // ASA Services
        eSrvASA_ASA = 400,
        eSrvASA_Last,

        // SRS Services
        eSrvSRS_SRS = 500,
        eSrvSRS_Intranet = 501,
        eSrvSRS_PMU = 502,
        eSrvSRS_WebLoader = 503,
        eSrvSRS_Last,

        // Intranet Services
        eSrvIntranet_HTTP = 600,
        eSrvIntranet_WebLoader = 601,
        eSrvIntranet_SRS = 602,
        eSrvIntranet_Last,

        // IOB Services
        eSrvIOB_ = 700,
        eSrvIOB_Last,

        // Media Services
        eSrvMedia_Database = 800,
        eSrvMedia_AudioPlayer = 801,
        eSrvMedia_Last,

        // Maintenance Services
        eSrvMaint_PDS = 900,
        eSrvMaint_Last,

        // Backbone Services
        eSrvBB_Shelf = 1000,
        eSrvBB_Bridge = 1001,
        eSrvBB_485Cnv = 1002,
        eSrvBB_Last,

        // TCMS Services
        eSrvTCMS_ = 1100,
        eSrvTCMS_Last,

        // eTrain Services
        eSrveTrain_ = 1200,
        eSrveTrain_Last,

        eSrvDictionary_ = 1300,
        eSrvCentralLog_ = 1301,

        // PMU Service
        eSrvNPENPMU_ = 1302,
        eSrvNPENPMU_WebLoader = 1303,
        eSrvNPENPMU_Last,

        // External Display service
        eSrvNPENExternalDisplay = 1305,

        // Dictionary Webservice
        eSrvDictionaryWebService = 1310,

        // Redundancy Manager Webservice (RGV2N2)
        eSrvVMC2N2MasterShipWS = 1311,

        // LMT Database Access Webservice
        eSrvLMTDbAccessWS = 1312,

        // PIS MasterShip Webservice (RGV2N2)
        eSrvPIS2N2MasterShipWS = 1313,

        eSrvUNETDictionary_ = 1314,

        eSrvHMIWebLoader_ = 1315,

        // NTP Server
        eSrvNTPServer = 1316,

        eSrvMLPISWebServer = 1317,

        // Fault2N2 Sync WebService Server (RGV2N2)
        eSrvFault2N2SyncWS = 1329,

        eSrvMLPISManager = 1330,

        eSrvFtpServer = 1350,

        // VMC and MEDIA redundancy services
        eSrvRED_VMC = 1360,
        eSrvRED_MEDIA = 1361,

        eSrvTrainCompositionWS = 1362,

        // SONO Database Access Webservice
        eSrvSONODbAccessWS = 1363,

        // CCTV Services
        eSrvCCTV_Console = 1400,  // CCTV Console / Monitor / OCU / DDU...
        eSrvCCTV_CU = 1401,  // Compression Unit (DVR with or without recording)
        eSrvCCTV_Last,

        // Custom Services (PLS New-Pen Ground Station with a WebLoader
        eSrvCustomPLSWebLoader = 1500,  // PLS New-Pen Ground Station with a WebLoader in it that deletes older files based on version in filename
        eSrvMLPISWebLoader = 1550,

        // 400T Display and 400T Display Manager
        eSrv400T_Player = 1600,
        eSrv400T_Maintenance = 1601,
        eSrv400T_Notification = 1602,

        // LMT equipments
        eSrvLMT_LMT = 1700,
        eSrvLMT_Last,

        // T2G Services
        eSrvT2G_Idenfication = 1800,
        eSrvT2G_FileTransfer = 1801,

        // SIF Services
        eSrvSIF_DataPackageServer = 1900,
        eSrvSIF_InstantMessageServer = 1901,
        eSrvSIF_ReportExchangeServer = 1902,
        eSrvSIF_MaintenanceServer = 1903,
        eSrvSIF_MissionServer = 1904,
		eSrvSIF_LiveVideoControlServer = 1907,
        eSrvSIF_RealTimeServer = 1908,

        eSrvAll_Last
    }
}
