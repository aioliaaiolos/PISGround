//---------------------------------------------------------------------------------------------------
// <copyright file="TrainDataPackageServiceStub.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System.ServiceModel;
using PIS.Ground.DataPackage;

namespace DataPackageTests.ServicesStub
{
    /// <summary>
    /// Implementation of the date package service on a train.
    /// </summary>
    /// <seealso cref="PIS.Ground.DataPackage.IDataPackageTrainService" />
    [ServiceBehaviorAttribute(InstanceContextMode = InstanceContextMode.Single, ConfigurationName = "PIS.Ground.DataPackage.IDataPackageTrainService")]
    public class TrainDataPackageServiceStub : IDataPackageTrainService
    {
        /// <summary>
        /// Gets the system identifier.
        /// </summary>
        public string SystemId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainDataPackageServiceStub"/> class.
        /// </summary>
        /// <param name="systemId">The system identifier.</param>
        public TrainDataPackageServiceStub(string systemId)
        {
            SystemId = systemId;
        }

        #region IDataPackageTrainService Members

        public ForceFutureBaselineResponse ForceFutureBaseline(ForceFutureBaselineRequest request)
        {
            return new ForceFutureBaselineResponse();
        }

        public ForceArchivedBaselineResponse ForceArchivedBaseline(ForceArchivedBaselineRequest request)
        {
            return new ForceArchivedBaselineResponse();
        }

        public CancelBaselineForcingResponse CancelBaselineForcing(CancelBaselineForcingRequest request)
        {
            return new CancelBaselineForcingResponse();
        }

        public SetBaselineVersionResponse SetBaselineVersion(SetBaselineVersionRequest request)
        {
            
            return new SetBaselineVersionResponse();
        }

        #endregion
    }
}
