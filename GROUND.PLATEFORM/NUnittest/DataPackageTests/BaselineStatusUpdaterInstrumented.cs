//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineStatusUpdaterInstrumented.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Moq;
using PIS.Ground.Core.LogMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.DataPackage;
using PIS.Ground.Core.Data;

namespace DataPackageTests
{
    /// <summary>
    /// Wrapper to access internal functionalities of BaselineStatusUpdater.
    /// </summary>
    /// <seealso cref="PIS.Ground.DataPackage.BaselineStatusUpdater" />
    internal class BaselineStatusUpdaterInstrumented : BaselineStatusUpdater
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaselineStatusUpdaterInstrumented"/> class.
        /// </summary>
        public BaselineStatusUpdaterInstrumented()
        {
            // No logic body
        }

        public bool Initialize(
            Dictionary<string, TrainBaselineStatusExtendedData> baselineProgresses,
            BaselineProgressUpdateProcedure baselineProgressUpdateProcedure,
            BaselineProgressRemoveProcedure baselineProgressRemoveProcedure,
            IT2GFileDistributionManager t2g,
            IDictionary<string, SystemInfo> currentSystems)
        {
            Mock<ILogManager> logManagerMock = new Mock<ILogManager>();

            return base.Initialize(baselineProgresses, baselineProgressUpdateProcedure, baselineProgressRemoveProcedure, t2g, logManagerMock.Object, currentSystems);
        }

        public bool Initialize(
            Dictionary<string, TrainBaselineStatusExtendedData> baselineProgresses,
            IT2GFileDistributionManager t2g,
            ILogManager logManager,
            IDictionary<string, SystemInfo> currentSystems)
        {
            BaselineStatusUpdater.BaselineProgressUpdateProcedure baselineProgressUpdateProcedureDelegate = new BaselineStatusUpdater.BaselineProgressUpdateProcedure(UpdateProgressOnHistoryLogger);

            BaselineStatusUpdater.BaselineProgressRemoveProcedure baselineProgressRemoveProcedureDelegate = new BaselineStatusUpdater.BaselineProgressRemoveProcedure(RemoveProgressFromHistoryLogger);


            return base.Initialize(baselineProgresses,
                    baselineProgressUpdateProcedureDelegate, baselineProgressRemoveProcedureDelegate, t2g, logManager, currentSystems);
        }

        public delegate void ProcessSystemChangedNotificationDelegate(SystemInfo notification,
            string assignedCurrentBaseline,
            string assignedFutureBaseline,
            ref string onBoardFutureBaseline,
            ref bool isDeepUpdate,
            TrainBaselineStatusData currentProgress,
            out TrainBaselineStatusData updatedProgress);

        public new void UpdateBaselineProgressFromFileTransferNotification(
            FileDistributionStatusArgs notification,
            ref TrainBaselineStatusExtendedData TrainBaselineStatusData)
        {
            base.UpdateBaselineProgressFromFileTransferNotification(notification, ref TrainBaselineStatusData);
        }

        public new BaselineProgressStatusEnum ValidateBaselineProgressStatus(
            BaselineProgressStatusEnum currentState,
            BaselineProgressStatusEnum newState)
        {
            return base.ValidateBaselineProgressStatus(currentState, newState);
        }


    }
}
