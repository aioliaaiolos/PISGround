using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Web;
using PIS.Ground.DataPackage;
using PIS.Ground.RemoteDataStore;
using PIS.Ground.Common;
using System.Threading;

namespace PIS.Ground.DataPackage.DataPackageSync
{
    [CreateOnDispatchService(typeof(DataPackageService))]
    [ServiceBehavior(Namespace = "http://alstom.com/pacis/pis/ground/datapackagesync/")]
    public class DataPackageSyncGroundService : IDataPackageSyncGroundService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataPackageSyncGroundService"/> class.
        /// </summary>
        public DataPackageSyncGroundService()
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "DataPackageSyncGroundService";
            }

            DataPackageService.Initialize();
        }

        public GetAssignedBaselineVersionResponse GetAssignedBaselineVersion(GetAssignedBaselineVersionRequest request)
        {
            GetAssignedBaselineVersionResponse lResponse = new GetAssignedBaselineVersionResponse();
            lResponse.Result = ResultEnumType.Failure;
            lResponse.CurrentVersion = "";
            lResponse.FutureVersion = "";

            try
            {
                using (RemoteDataStoreProxy lRemDSProxy = new RemoteDataStoreProxy())
                {
                    try
                    {
                        lResponse.CurrentVersion = lRemDSProxy.getAssignedCurrentBaselineVersion(request.ElementId);
                        lResponse.FutureVersion = lRemDSProxy.getAssignedFutureBaselineVersion(request.ElementId);
                        lResponse.Result = ResultEnumType.Success;
                    }
                    catch (FaultException fe)
                    {
                        if (fe.Code.Name == "UNKNOWN_ELEMENT_ID" || fe.Code.Name == "INVALID_ELEMENT_ID" )
                        {
                            lResponse.Result = ResultEnumType.InvalidElementId;
                        }
                        else
                        {
                            PIS.Ground.Core.LogMgmt.LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.EXCEPTION, "Element ID : " + request.ElementId + ". Error Code : " + fe.Code.Name + ". Error Message : " + fe.Message
                                                        , "PIS.Ground.DataPackage.DataPackageSyncGroundService.GetAssignedBaselineVersion"
                                                        , fe, PIS.Ground.Core.Data.EventIdEnum.DataPackage);
                            lResponse.Result = ResultEnumType.Failure;
                        }
                    }
                                       
                    finally
                    {
                        if (lRemDSProxy.State == CommunicationState.Faulted)
                        {
                            lRemDSProxy.Abort();
                        }
                    }
                }
            }
            catch (TimeoutException)
            {
                lResponse.Result = ResultEnumType.RemoteDataStoreNotAccessible;
            }
            catch (CommunicationException)
            {
                lResponse.Result = ResultEnumType.RemoteDataStoreNotAccessible;
            }            

            return lResponse;
        }

        public RequestBaselineResponse RequestBaseline(RequestBaselineRequest request)
        {
            RequestBaselineResponse lResponse = new RequestBaselineResponse();
            DataPackageService lDataPackageService = new DataPackageService();
            DataPackageErrorEnum lResult = lDataPackageService.distributeTargetedBaseline(request.ElementId, request.Version, request.ActivationDate, request.ExpirationDate);

            switch (lResult)
            {
                case DataPackageErrorEnum.REQUEST_ACCEPTED:
                    lResponse.Result = ResultEnumType.Success;
                    break;
                case DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE:
                    lResponse.Result = ResultEnumType.RemoteDataStoreNotAccessible;
                    break;
                case DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND:
                    lResponse.Result = ResultEnumType.InvalidElementId;
                    break;
                case DataPackageErrorEnum.BASELINE_NOT_FOUND:
                    lResponse.Result = ResultEnumType.BaselineNotFound;
                    break;
                case DataPackageErrorEnum.DATA_PACKAGE_NOT_FOUND:
                    lResponse.Result = ResultEnumType.PackageNotFound;
                    break;
                default:
                    lResponse.Result = ResultEnumType.Failure;
                    break;
            }         

            return lResponse;
        }

        public RequestAssignedBaselineResponse RequestAssignedBaseline(RequestAssignedBaselineRequest request)
        {
            RequestAssignedBaselineResponse lResponse = new RequestAssignedBaselineResponse();
            lResponse.Result = ResultEnumType.Failure;
            lResponse.Version = "";

            try
            {
                using (RemoteDataStoreProxy lRemDSProxy = new RemoteDataStoreProxy())
                {
                    try
                    {
                        DataContainer baselinesDef = lRemDSProxy.getElementBaselinesDefinitions(request.ElementId);
                        ElementDescription elemDescr = DataTypeConversion.fromDataContainerToElementDescription(baselinesDef);
                        
                        if (request.Current)
                        {
                            lResponse.Version = lRemDSProxy.getAssignedCurrentBaselineVersion(request.ElementId);
                            lResponse.ActivationDate = DateTime.Now;
                            lResponse.ExpirationDate = elemDescr.AssignedCurrentBaselineExpirationDate;                            
                        }
                        else
                        {
                            lResponse.Version = lRemDSProxy.getAssignedFutureBaselineVersion(request.ElementId);
                            lResponse.ActivationDate = elemDescr.AssignedFutureBaselineActivationDate;
                            lResponse.ExpirationDate = elemDescr.AssignedFutureBaselineExpirationDate;
                        }

                        lResponse.Result = ResultEnumType.Success;

                        DataPackageService lDataPackageService = new DataPackageService();
                        DataPackageErrorEnum lResult = lDataPackageService.distributeTargetedBaseline(request.ElementId, lResponse.Version, lResponse.ActivationDate, lResponse.ExpirationDate);

                        switch (lResult)
                        {
                            case DataPackageErrorEnum.REQUEST_ACCEPTED:
                                lResponse.Result = ResultEnumType.Success;
                                break;
                            case DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE:
                                lResponse.Result = ResultEnumType.RemoteDataStoreNotAccessible;
                                break;
                            case DataPackageErrorEnum.ELEMENT_ID_NOT_FOUND:
                                lResponse.Result = ResultEnumType.InvalidElementId;
                                break;
                            case DataPackageErrorEnum.BASELINE_NOT_FOUND:
                                lResponse.Result = ResultEnumType.BaselineNotFound;
                                break;
                            case DataPackageErrorEnum.DATA_PACKAGE_NOT_FOUND:
                                lResponse.Result = ResultEnumType.PackageNotFound;
                                break;
                            default:
                                lResponse.Result = ResultEnumType.Failure;
                                break;
                        }  
                    }
                    catch (FaultException fe)
                    {
                        if (fe.Code.Name == "UNKNOWN_ELEMENT_ID" || fe.Code.Name == "INVALID_ELEMENT_ID")
                        {
                            lResponse.Result = ResultEnumType.InvalidElementId;
                        }
                        else
                        {
                            PIS.Ground.Core.LogMgmt.LogManager.WriteLog(PIS.Ground.Core.Data.TraceType.EXCEPTION, "Element ID : " + request.ElementId + ". Error Code : " + fe.Code.Name + ". Error Message : " + fe.Message
                                                        , "PIS.Ground.DataPackage.DataPackageSyncGroundService.RequestAssignedbaseline"
                                                        , fe, PIS.Ground.Core.Data.EventIdEnum.DataPackage);
                            lResponse.Result = ResultEnumType.Failure;
                        }
                    }
                    finally
                    {
                        if (lRemDSProxy.State == CommunicationState.Faulted)
                        {
                            lRemDSProxy.Abort();
                        }
                    }
                }
            }
            catch (TimeoutException)
            {
                lResponse.Result = ResultEnumType.RemoteDataStoreNotAccessible;
            }
            catch (CommunicationException)
            {
                lResponse.Result = ResultEnumType.RemoteDataStoreNotAccessible;
            }

            return lResponse;
        }

        public RequestBaselineVersionResponse RequestBaselineVersion(RequestBaselineVersionRequest request)
        {
            RequestBaselineVersionResponse lResponse = new RequestBaselineVersionResponse();
            DataPackageService lDataPackageService = new DataPackageService();

            DataPackageErrorEnum lResult = lDataPackageService.requestBaselineVesion(request.ElementId, request.PisBaseVersion, request.PisMissionVersion, request.PisInfotainmentVersion, request.LmtVersion);

            switch (lResult)
            {                   
                case DataPackageErrorEnum.REQUEST_ACCEPTED:
                    lResponse.Result = ResultEnumType.Success;
                    break;
                case DataPackageErrorEnum.REMOTEDATASTORE_NOT_ACCESSIBLE:
                    lResponse.Result = ResultEnumType.RemoteDataStoreNotAccessible;
                    break;
                default:
                    lResponse.Result = ResultEnumType.Failure;
                    break;
            }
           
            return lResponse;
        }
    }
}
