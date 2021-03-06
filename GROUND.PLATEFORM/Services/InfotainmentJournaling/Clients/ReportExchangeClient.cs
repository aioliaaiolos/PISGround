﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :2.0.50727.5485
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PIS.Ground.Infotainment.Journaling.ReportExchange
{
    using System.Runtime.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ResultType", Namespace="http://alstom.com/pacis/pis/schema/train/reportexchange/")]
    public enum ResultType : int
    {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Failure = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ServiceInhibited = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        NoEntryFound = 3,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://alstom.com/pacis/pis/train/reportexchange/", ConfigurationName="PIS.Ground.Infotainment.Journaling.ReportExchange.IReportExchangeService")]
    public interface IReportExchangeService
    {
        
        // CODEGEN : La génération du contrat de message depuis l’espace de noms de wrapper (http://alstom.com/pacis/pis/schema/train/reportexchange/) du message GetInfotainmentJournalRequest ne correspond pas à la valeur par défaut (http://alstom.com/pacis/pis/train/reportexchange/)
        [System.ServiceModel.OperationContractAttribute(Action="http://alstom.com/pacis/pis/train/reportexchange/GetInfotainmentJournal", ReplyAction="*")]
        PIS.Ground.Infotainment.Journaling.ReportExchange.GetInfotainmentJournalResponse GetInfotainmentJournal(PIS.Ground.Infotainment.Journaling.ReportExchange.GetInfotainmentJournalRequest request);
        
        // CODEGEN : La génération du contrat de message depuis l’espace de noms de wrapper (http://alstom.com/pacis/pis/schema/train/reportexchange/) du message NotifyInfotainmentJournalRetrievedRequest ne correspond pas à la valeur par défaut (http://alstom.com/pacis/pis/train/reportexchange/)
        [System.ServiceModel.OperationContractAttribute(Action="http://alstom.com/pacis/pis/train/reportexchange/NotifyInfotainmentJournalRetriev" +
            "ed", ReplyAction="*")]
        PIS.Ground.Infotainment.Journaling.ReportExchange.NotifyInfotainmentJournalRetrievedResponse NotifyInfotainmentJournalRetrieved(PIS.Ground.Infotainment.Journaling.ReportExchange.NotifyInfotainmentJournalRetrievedRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetInfotainmentJournal", WrapperNamespace="http://alstom.com/pacis/pis/schema/train/reportexchange/", IsWrapped=true)]
    public partial class GetInfotainmentJournalRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/reportexchange/", Order=0)]
        public string requestId;
        
        public GetInfotainmentJournalRequest()
        {
        }
        
        public GetInfotainmentJournalRequest(string requestId)
        {
            this.requestId = requestId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetInfotainmentJournalResponse", WrapperNamespace="http://alstom.com/pacis/pis/schema/train/reportexchange/", IsWrapped=true)]
    public partial class GetInfotainmentJournalResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/reportexchange/", Order=0)]
        public uint folderId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/reportexchange/", Order=1)]
        public string requestId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/reportexchange/", Order=2)]
        public PIS.Ground.Infotainment.Journaling.ReportExchange.ResultType Result;
        
        public GetInfotainmentJournalResponse()
        {
        }
        
        public GetInfotainmentJournalResponse(uint folderId, string requestId, PIS.Ground.Infotainment.Journaling.ReportExchange.ResultType Result)
        {
            this.folderId = folderId;
            this.requestId = requestId;
            this.Result = Result;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="NotifyInfotainmentJournalRetrieved", WrapperNamespace="http://alstom.com/pacis/pis/schema/train/reportexchange/", IsWrapped=true)]
    public partial class NotifyInfotainmentJournalRetrievedRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/reportexchange/", Order=0)]
        public string requestId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/reportexchange/", Order=1)]
        public uint folderId;
        
        public NotifyInfotainmentJournalRetrievedRequest()
        {
        }
        
        public NotifyInfotainmentJournalRetrievedRequest(string requestId, uint folderId)
        {
            this.requestId = requestId;
            this.folderId = folderId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="NotifyInfotainmentJournalRetrievedResponse", WrapperNamespace="http://alstom.com/pacis/pis/schema/train/reportexchange/", IsWrapped=true)]
    public partial class NotifyInfotainmentJournalRetrievedResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/reportexchange/", Order=0)]
        public string requestId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/reportexchange/", Order=1)]
        public PIS.Ground.Infotainment.Journaling.ReportExchange.ResultType result;
        
        public NotifyInfotainmentJournalRetrievedResponse()
        {
        }
        
        public NotifyInfotainmentJournalRetrievedResponse(string requestId, PIS.Ground.Infotainment.Journaling.ReportExchange.ResultType result)
        {
            this.requestId = requestId;
            this.result = result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface IReportExchangeServiceChannel : PIS.Ground.Infotainment.Journaling.ReportExchange.IReportExchangeService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class ReportExchangeServiceClient : System.ServiceModel.ClientBase<PIS.Ground.Infotainment.Journaling.ReportExchange.IReportExchangeService>, PIS.Ground.Infotainment.Journaling.ReportExchange.IReportExchangeService
    {
        
        public ReportExchangeServiceClient()
        {
        }
        
        public ReportExchangeServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public ReportExchangeServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public ReportExchangeServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public ReportExchangeServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PIS.Ground.Infotainment.Journaling.ReportExchange.GetInfotainmentJournalResponse PIS.Ground.Infotainment.Journaling.ReportExchange.IReportExchangeService.GetInfotainmentJournal(PIS.Ground.Infotainment.Journaling.ReportExchange.GetInfotainmentJournalRequest request)
        {
            return base.Channel.GetInfotainmentJournal(request);
        }
        
        public uint GetInfotainmentJournal(ref string requestId, out PIS.Ground.Infotainment.Journaling.ReportExchange.ResultType Result)
        {
            PIS.Ground.Infotainment.Journaling.ReportExchange.GetInfotainmentJournalRequest inValue = new PIS.Ground.Infotainment.Journaling.ReportExchange.GetInfotainmentJournalRequest();
            inValue.requestId = requestId;
            PIS.Ground.Infotainment.Journaling.ReportExchange.GetInfotainmentJournalResponse retVal = ((PIS.Ground.Infotainment.Journaling.ReportExchange.IReportExchangeService)(this)).GetInfotainmentJournal(inValue);
            requestId = retVal.requestId;
            Result = retVal.Result;
            return retVal.folderId;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PIS.Ground.Infotainment.Journaling.ReportExchange.NotifyInfotainmentJournalRetrievedResponse PIS.Ground.Infotainment.Journaling.ReportExchange.IReportExchangeService.NotifyInfotainmentJournalRetrieved(PIS.Ground.Infotainment.Journaling.ReportExchange.NotifyInfotainmentJournalRetrievedRequest request)
        {
            return base.Channel.NotifyInfotainmentJournalRetrieved(request);
        }
        
        public PIS.Ground.Infotainment.Journaling.ReportExchange.ResultType NotifyInfotainmentJournalRetrieved(ref string requestId, uint folderId)
        {
            PIS.Ground.Infotainment.Journaling.ReportExchange.NotifyInfotainmentJournalRetrievedRequest inValue = new PIS.Ground.Infotainment.Journaling.ReportExchange.NotifyInfotainmentJournalRetrievedRequest();
            inValue.requestId = requestId;
            inValue.folderId = folderId;
            PIS.Ground.Infotainment.Journaling.ReportExchange.NotifyInfotainmentJournalRetrievedResponse retVal = ((PIS.Ground.Infotainment.Journaling.ReportExchange.IReportExchangeService)(this)).NotifyInfotainmentJournalRetrieved(inValue);
            requestId = retVal.requestId;
            return retVal.result;
        }
    }
}
