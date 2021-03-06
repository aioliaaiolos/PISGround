﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :2.0.50727.5485
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PIS.Ground.Maintenance.Train
{
    using System.Runtime.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ResultType", Namespace="http://alstom.com/pacis/pis/schema/")]
    public enum ResultType : int
    {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Failure = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ServiceInhibited = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://alstom.com/pacis/pis/train/maintenance/", ConfigurationName="PIS.Ground.Maintenance.Train.IMaintenanceService")]
    public interface IMaintenanceService
    {
        
        // CODEGEN : La génération du contrat de message depuis l’espace de noms de wrapper (http://alstom.com/pacis/pis/schema/) du message DownloadSystemMessageFilesRequest ne correspond pas à la valeur par défaut (http://alstom.com/pacis/pis/train/maintenance/)
        [System.ServiceModel.OperationContractAttribute(Action="http://alstom.com/pacis/pis/train/maintenance/DownloadSystemMessageFiles", ReplyAction="*")]
        PIS.Ground.Maintenance.Train.DownloadSystemMessageFilesResponse DownloadSystemMessageFiles(PIS.Ground.Maintenance.Train.DownloadSystemMessageFilesRequest request);
        
        // CODEGEN : La génération du contrat de message depuis l’espace de noms de wrapper (http://alstom.com/pacis/pis/schema/) du message RetrieveFolderInfoRequest ne correspond pas à la valeur par défaut (http://alstom.com/pacis/pis/train/maintenance/)
        [System.ServiceModel.OperationContractAttribute(Action="http://alstom.com/pacis/pis/train/maintenance/RetrieveFolderInfo", ReplyAction="*")]
        PIS.Ground.Maintenance.Train.RetrieveFolderInfoResponse RetrieveFolderInfo(PIS.Ground.Maintenance.Train.RetrieveFolderInfoRequest request);
        
        // CODEGEN : La génération du contrat de message depuis l’espace de noms de wrapper (http://alstom.com/pacis/pis/schema/) du message DownloadSoftwarePackageVersionsRequest ne correspond pas à la valeur par défaut (http://alstom.com/pacis/pis/train/maintenance/)
        [System.ServiceModel.OperationContractAttribute(Action="http://alstom.com/pacis/pis/train/maintenance/DownloadSoftwarePackageVersions", ReplyAction="*")]
        PIS.Ground.Maintenance.Train.DownloadSoftwarePackageVersionsResponse DownloadSoftwarePackageVersions(PIS.Ground.Maintenance.Train.DownloadSoftwarePackageVersionsRequest request);
        
        // CODEGEN : La génération du contrat de message depuis l’espace de noms de wrapper (http://alstom.com/pacis/pis/schema/) du message RetrieveDataPackageBaselineListRequest ne correspond pas à la valeur par défaut (http://alstom.com/pacis/pis/train/maintenance/)
        [System.ServiceModel.OperationContractAttribute(Action="http://alstom.com/pacis/pis/train/maintenance/RetrieveDataPackageBaselineList", ReplyAction="*")]
        PIS.Ground.Maintenance.Train.RetrieveDataPackageBaselineListResponse RetrieveDataPackageBaselineList(PIS.Ground.Maintenance.Train.RetrieveDataPackageBaselineListRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DownloadSystemMessageFiles", WrapperNamespace="http://alstom.com/pacis/pis/schema/", IsWrapped=true)]
    public partial class DownloadSystemMessageFilesRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string RequestId;
        
        public DownloadSystemMessageFilesRequest()
        {
        }
        
        public DownloadSystemMessageFilesRequest(string RequestId)
        {
            this.RequestId = RequestId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DownloadSystemMessageFilesResponse", WrapperNamespace="http://alstom.com/pacis/pis/schema/", IsWrapped=true)]
    public partial class DownloadSystemMessageFilesResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public uint FolderId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=1)]
        public string RequestId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=2)]
        public PIS.Ground.Maintenance.Train.ResultType Result;
        
        public DownloadSystemMessageFilesResponse()
        {
        }
        
        public DownloadSystemMessageFilesResponse(uint FolderId, string RequestId, PIS.Ground.Maintenance.Train.ResultType Result)
        {
            this.FolderId = FolderId;
            this.RequestId = RequestId;
            this.Result = Result;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RetrieveFolderInfo", WrapperNamespace="http://alstom.com/pacis/pis/schema/", IsWrapped=true)]
    public partial class RetrieveFolderInfoRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string RequestId;
        
        public RetrieveFolderInfoRequest()
        {
        }
        
        public RetrieveFolderInfoRequest(string RequestId)
        {
            this.RequestId = RequestId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RetrieveFolderInfoResponse", WrapperNamespace="http://alstom.com/pacis/pis/schema/", IsWrapped=true)]
    public partial class RetrieveFolderInfoResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string RequestId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=1)]
        public PIS.Ground.Maintenance.Train.ResultType Result;
        
        public RetrieveFolderInfoResponse()
        {
        }
        
        public RetrieveFolderInfoResponse(string RequestId, PIS.Ground.Maintenance.Train.ResultType Result)
        {
            this.RequestId = RequestId;
            this.Result = Result;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DownloadSoftwarePackageVersions", WrapperNamespace="http://alstom.com/pacis/pis/schema/", IsWrapped=true)]
    public partial class DownloadSoftwarePackageVersionsRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string RequestId;
        
        public DownloadSoftwarePackageVersionsRequest()
        {
        }
        
        public DownloadSoftwarePackageVersionsRequest(string RequestId)
        {
            this.RequestId = RequestId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DownloadSoftwarePackageVersionsResponse", WrapperNamespace="http://alstom.com/pacis/pis/schema/", IsWrapped=true)]
    public partial class DownloadSoftwarePackageVersionsResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public uint FolderId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=1)]
        public string RequestId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=2)]
        public PIS.Ground.Maintenance.Train.ResultType Result;
        
        public DownloadSoftwarePackageVersionsResponse()
        {
        }
        
        public DownloadSoftwarePackageVersionsResponse(uint FolderId, string RequestId, PIS.Ground.Maintenance.Train.ResultType Result)
        {
            this.FolderId = FolderId;
            this.RequestId = RequestId;
            this.Result = Result;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RetrieveDataPackageBaselineList", WrapperNamespace="http://alstom.com/pacis/pis/schema/", IsWrapped=true)]
    public partial class RetrieveDataPackageBaselineListRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string RequestId;
        
        public RetrieveDataPackageBaselineListRequest()
        {
        }
        
        public RetrieveDataPackageBaselineListRequest(string RequestId)
        {
            this.RequestId = RequestId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RetrieveDataPackageBaselineListResponse", WrapperNamespace="http://alstom.com/pacis/pis/schema/", IsWrapped=true)]
    public partial class RetrieveDataPackageBaselineListResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string RequestId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=1)]
        public PIS.Ground.Maintenance.Train.ResultType Result;
        
        public RetrieveDataPackageBaselineListResponse()
        {
        }
        
        public RetrieveDataPackageBaselineListResponse(string RequestId, PIS.Ground.Maintenance.Train.ResultType Result)
        {
            this.RequestId = RequestId;
            this.Result = Result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface IMaintenanceServiceChannel : PIS.Ground.Maintenance.Train.IMaintenanceService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class MaintenanceServiceClient : System.ServiceModel.ClientBase<PIS.Ground.Maintenance.Train.IMaintenanceService>, PIS.Ground.Maintenance.Train.IMaintenanceService
    {
        
        public MaintenanceServiceClient()
        {
        }
        
        public MaintenanceServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public MaintenanceServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public MaintenanceServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public MaintenanceServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PIS.Ground.Maintenance.Train.DownloadSystemMessageFilesResponse PIS.Ground.Maintenance.Train.IMaintenanceService.DownloadSystemMessageFiles(PIS.Ground.Maintenance.Train.DownloadSystemMessageFilesRequest request)
        {
            return base.Channel.DownloadSystemMessageFiles(request);
        }
        
        public uint DownloadSystemMessageFiles(ref string RequestId, out PIS.Ground.Maintenance.Train.ResultType Result)
        {
            PIS.Ground.Maintenance.Train.DownloadSystemMessageFilesRequest inValue = new PIS.Ground.Maintenance.Train.DownloadSystemMessageFilesRequest();
            inValue.RequestId = RequestId;
            PIS.Ground.Maintenance.Train.DownloadSystemMessageFilesResponse retVal = ((PIS.Ground.Maintenance.Train.IMaintenanceService)(this)).DownloadSystemMessageFiles(inValue);
            RequestId = retVal.RequestId;
            Result = retVal.Result;
            return retVal.FolderId;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PIS.Ground.Maintenance.Train.RetrieveFolderInfoResponse PIS.Ground.Maintenance.Train.IMaintenanceService.RetrieveFolderInfo(PIS.Ground.Maintenance.Train.RetrieveFolderInfoRequest request)
        {
            return base.Channel.RetrieveFolderInfo(request);
        }
        
        public PIS.Ground.Maintenance.Train.ResultType RetrieveFolderInfo(ref string RequestId)
        {
            PIS.Ground.Maintenance.Train.RetrieveFolderInfoRequest inValue = new PIS.Ground.Maintenance.Train.RetrieveFolderInfoRequest();
            inValue.RequestId = RequestId;
            PIS.Ground.Maintenance.Train.RetrieveFolderInfoResponse retVal = ((PIS.Ground.Maintenance.Train.IMaintenanceService)(this)).RetrieveFolderInfo(inValue);
            RequestId = retVal.RequestId;
            return retVal.Result;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PIS.Ground.Maintenance.Train.DownloadSoftwarePackageVersionsResponse PIS.Ground.Maintenance.Train.IMaintenanceService.DownloadSoftwarePackageVersions(PIS.Ground.Maintenance.Train.DownloadSoftwarePackageVersionsRequest request)
        {
            return base.Channel.DownloadSoftwarePackageVersions(request);
        }
        
        public uint DownloadSoftwarePackageVersions(ref string RequestId, out PIS.Ground.Maintenance.Train.ResultType Result)
        {
            PIS.Ground.Maintenance.Train.DownloadSoftwarePackageVersionsRequest inValue = new PIS.Ground.Maintenance.Train.DownloadSoftwarePackageVersionsRequest();
            inValue.RequestId = RequestId;
            PIS.Ground.Maintenance.Train.DownloadSoftwarePackageVersionsResponse retVal = ((PIS.Ground.Maintenance.Train.IMaintenanceService)(this)).DownloadSoftwarePackageVersions(inValue);
            RequestId = retVal.RequestId;
            Result = retVal.Result;
            return retVal.FolderId;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PIS.Ground.Maintenance.Train.RetrieveDataPackageBaselineListResponse PIS.Ground.Maintenance.Train.IMaintenanceService.RetrieveDataPackageBaselineList(PIS.Ground.Maintenance.Train.RetrieveDataPackageBaselineListRequest request)
        {
            return base.Channel.RetrieveDataPackageBaselineList(request);
        }
        
        public PIS.Ground.Maintenance.Train.ResultType RetrieveDataPackageBaselineList(ref string RequestId)
        {
            PIS.Ground.Maintenance.Train.RetrieveDataPackageBaselineListRequest inValue = new PIS.Ground.Maintenance.Train.RetrieveDataPackageBaselineListRequest();
            inValue.RequestId = RequestId;
            PIS.Ground.Maintenance.Train.RetrieveDataPackageBaselineListResponse retVal = ((PIS.Ground.Maintenance.Train.IMaintenanceService)(this)).RetrieveDataPackageBaselineList(inValue);
            RequestId = retVal.RequestId;
            return retVal.Result;
        }
    }
}
