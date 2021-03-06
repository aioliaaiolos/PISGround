﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :2.0.50727.5485
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PIS.Train.LiveVideoControl
{
    using System.Runtime.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="LiveVideoControlRspEnumType", Namespace="http://alstom.com/pacis/pis/schema/train/livevideocontrol/")]
    public enum LiveVideoControlRspEnumType : int
    {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        StartVideoStreamingCommandReceived = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        StopVideoStreamingCommandReceived = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        SendVideoStreamingStatusCommandReceived = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://alstom.com/pacis/pis/train/livevideocontrol/", ConfigurationName="PIS.Train.LiveVideoControl.ILiveVideoControlService")]
    public interface ILiveVideoControlService
    {
        
        // CODEGEN : La génération du contrat de message depuis l’espace de noms de wrapper (http://alstom.com/pacis/pis/schema/train/livevideocontrol/) du message StartVideoStreamingCommandRequest ne correspond pas à la valeur par défaut (http://alstom.com/pacis/pis/train/livevideocontrol/)
        [System.ServiceModel.OperationContractAttribute(Action="http://alstom.com/pacis/pis/train/livevideocontrol/StartVideoStreamingCommand", ReplyAction="*")]
        PIS.Train.LiveVideoControl.StartVideoStreamingCommandResponse StartVideoStreamingCommand(PIS.Train.LiveVideoControl.StartVideoStreamingCommandRequest request);
        
        // CODEGEN : La génération du contrat de message depuis l’espace de noms de wrapper (http://alstom.com/pacis/pis/schema/train/livevideocontrol/) du message StopVideoStreamingCommandRequest ne correspond pas à la valeur par défaut (http://alstom.com/pacis/pis/train/livevideocontrol/)
        [System.ServiceModel.OperationContractAttribute(Action="http://alstom.com/pacis/pis/train/livevideocontrol/StopVideoStreamingCommand", ReplyAction="*")]
        PIS.Train.LiveVideoControl.StopVideoStreamingCommandResponse StopVideoStreamingCommand(PIS.Train.LiveVideoControl.StopVideoStreamingCommandRequest request);
        
        // CODEGEN : La génération du contrat de message depuis l’espace de noms de wrapper (http://alstom.com/pacis/pis/schema/train/livevideocontrol/) du message SendVideoStreamingStatusCommandRequest ne correspond pas à la valeur par défaut (http://alstom.com/pacis/pis/train/livevideocontrol/)
        [System.ServiceModel.OperationContractAttribute(Action="http://alstom.com/pacis/pis/train/livevideocontrol/SendVideoStreamingStatusComman" +
            "d", ReplyAction="*")]
        PIS.Train.LiveVideoControl.SendVideoStreamingStatusCommandResponse SendVideoStreamingStatusCommand(PIS.Train.LiveVideoControl.SendVideoStreamingStatusCommandRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="StartVideoStreamingCommand", WrapperNamespace="http://alstom.com/pacis/pis/schema/train/livevideocontrol/", IsWrapped=true)]
    public partial class StartVideoStreamingCommandRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/livevideocontrol/", Order=0)]
        public string RequestId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/livevideocontrol/", Order=1)]
        public string URL;
        
        public StartVideoStreamingCommandRequest()
        {
        }
        
        public StartVideoStreamingCommandRequest(string RequestId, string URL)
        {
            this.RequestId = RequestId;
            this.URL = URL;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="StartVideoStreamingCommandResponse", WrapperNamespace="http://alstom.com/pacis/pis/schema/train/livevideocontrol/", IsWrapped=true)]
    public partial class StartVideoStreamingCommandResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/livevideocontrol/", Order=0)]
        public PIS.Train.LiveVideoControl.LiveVideoControlRspEnumType Result;
        
        public StartVideoStreamingCommandResponse()
        {
        }
        
        public StartVideoStreamingCommandResponse(PIS.Train.LiveVideoControl.LiveVideoControlRspEnumType Result)
        {
            this.Result = Result;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="StopVideoStreamingCommand", WrapperNamespace="http://alstom.com/pacis/pis/schema/train/livevideocontrol/", IsWrapped=true)]
    public partial class StopVideoStreamingCommandRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/livevideocontrol/", Order=0)]
        public string RequestId;
        
        public StopVideoStreamingCommandRequest()
        {
        }
        
        public StopVideoStreamingCommandRequest(string RequestId)
        {
            this.RequestId = RequestId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="StopVideoStreamingCommandResponse", WrapperNamespace="http://alstom.com/pacis/pis/schema/train/livevideocontrol/", IsWrapped=true)]
    public partial class StopVideoStreamingCommandResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/livevideocontrol/", Order=0)]
        public PIS.Train.LiveVideoControl.LiveVideoControlRspEnumType Result;
        
        public StopVideoStreamingCommandResponse()
        {
        }
        
        public StopVideoStreamingCommandResponse(PIS.Train.LiveVideoControl.LiveVideoControlRspEnumType Result)
        {
            this.Result = Result;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SendVideoStreamingStatusCommand", WrapperNamespace="http://alstom.com/pacis/pis/schema/train/livevideocontrol/", IsWrapped=true)]
    public partial class SendVideoStreamingStatusCommandRequest
    {
        
        public SendVideoStreamingStatusCommandRequest()
        {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SendVideoStreamingStatusCommandResponse", WrapperNamespace="http://alstom.com/pacis/pis/schema/train/livevideocontrol/", IsWrapped=true)]
    public partial class SendVideoStreamingStatusCommandResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://alstom.com/pacis/pis/schema/train/livevideocontrol/", Order=0)]
        public PIS.Train.LiveVideoControl.LiveVideoControlRspEnumType Result;
        
        public SendVideoStreamingStatusCommandResponse()
        {
        }
        
        public SendVideoStreamingStatusCommandResponse(PIS.Train.LiveVideoControl.LiveVideoControlRspEnumType Result)
        {
            this.Result = Result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface ILiveVideoControlServiceChannel : PIS.Train.LiveVideoControl.ILiveVideoControlService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class LiveVideoControlServiceClient : System.ServiceModel.ClientBase<PIS.Train.LiveVideoControl.ILiveVideoControlService>, PIS.Train.LiveVideoControl.ILiveVideoControlService
    {
        
        public LiveVideoControlServiceClient()
        {
        }
        
        public LiveVideoControlServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public LiveVideoControlServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public LiveVideoControlServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public LiveVideoControlServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PIS.Train.LiveVideoControl.StartVideoStreamingCommandResponse PIS.Train.LiveVideoControl.ILiveVideoControlService.StartVideoStreamingCommand(PIS.Train.LiveVideoControl.StartVideoStreamingCommandRequest request)
        {
            return base.Channel.StartVideoStreamingCommand(request);
        }
        
        public PIS.Train.LiveVideoControl.LiveVideoControlRspEnumType StartVideoStreamingCommand(string RequestId, string URL)
        {
            PIS.Train.LiveVideoControl.StartVideoStreamingCommandRequest inValue = new PIS.Train.LiveVideoControl.StartVideoStreamingCommandRequest();
            inValue.RequestId = RequestId;
            inValue.URL = URL;
            PIS.Train.LiveVideoControl.StartVideoStreamingCommandResponse retVal = ((PIS.Train.LiveVideoControl.ILiveVideoControlService)(this)).StartVideoStreamingCommand(inValue);
            return retVal.Result;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PIS.Train.LiveVideoControl.StopVideoStreamingCommandResponse PIS.Train.LiveVideoControl.ILiveVideoControlService.StopVideoStreamingCommand(PIS.Train.LiveVideoControl.StopVideoStreamingCommandRequest request)
        {
            return base.Channel.StopVideoStreamingCommand(request);
        }
        
        public PIS.Train.LiveVideoControl.LiveVideoControlRspEnumType StopVideoStreamingCommand(string RequestId)
        {
            PIS.Train.LiveVideoControl.StopVideoStreamingCommandRequest inValue = new PIS.Train.LiveVideoControl.StopVideoStreamingCommandRequest();
            inValue.RequestId = RequestId;
            PIS.Train.LiveVideoControl.StopVideoStreamingCommandResponse retVal = ((PIS.Train.LiveVideoControl.ILiveVideoControlService)(this)).StopVideoStreamingCommand(inValue);
            return retVal.Result;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PIS.Train.LiveVideoControl.SendVideoStreamingStatusCommandResponse PIS.Train.LiveVideoControl.ILiveVideoControlService.SendVideoStreamingStatusCommand(PIS.Train.LiveVideoControl.SendVideoStreamingStatusCommandRequest request)
        {
            return base.Channel.SendVideoStreamingStatusCommand(request);
        }
        
        public PIS.Train.LiveVideoControl.LiveVideoControlRspEnumType SendVideoStreamingStatusCommand()
        {
            PIS.Train.LiveVideoControl.SendVideoStreamingStatusCommandRequest inValue = new PIS.Train.LiveVideoControl.SendVideoStreamingStatusCommandRequest();
            PIS.Train.LiveVideoControl.SendVideoStreamingStatusCommandResponse retVal = ((PIS.Train.LiveVideoControl.ILiveVideoControlService)(this)).SendVideoStreamingStatusCommand(inValue);
            return retVal.Result;
        }
    }
}
