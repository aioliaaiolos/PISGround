//---------------------------------------------------------------------------------------------------
// <copyright file="TrainBaselineStatusExtendedData.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Globalization;
using System.Text;
using PIS.Ground.Core.Data;

namespace PIS.Ground.DataPackage
{

    /// <summary>Train baseline deployment status with extended data.</summary>
    public class TrainBaselineStatusExtendedData
    {
        /// <summary>
        /// Initializes a new instance of the TrainBaselineStatusExtendedData class.
        /// </summary>
        /// <param name="status">The basic status information.</param>
        /// <param name="assignedFutureBaseline">The extended information: assigned future baseline version.</param>
        /// <param name="onBoardFutureBaseline">The extended information: on board future baseline version.</param>
        /// <param name="assignedCurrentBaseline">The assigned current baseline.</param>
        /// <param name="isT2GPollingRequired">true if T2G must be ask for possible on-going transfer.</param>
        public TrainBaselineStatusExtendedData(
            TrainBaselineStatusData status,
            string assignedFutureBaseline,
            string onBoardFutureBaseline,
            string assignedCurrentBaseline,
            bool isT2GPollingRequired)
        {
            Status = status;
            AssignedFutureBaseline = assignedFutureBaseline;
            OnBoardFutureBaseline = onBoardFutureBaseline;
            IsT2GPollingRequired = isT2GPollingRequired;
            AssignedCurrentBaseline = assignedCurrentBaseline;
        }

        /// <summary>
        /// Initializes a new instance of the TrainBaselineStatusExtendedData class.
        /// </summary>
        /// <param name="status">The basic status information.</param>
        public TrainBaselineStatusExtendedData(
            TrainBaselineStatusData status) : this(status, null, null, null, true)
        {
            // No logic body
        }

        /// <summary>
        /// Initializes a new instance of the TrainBaselineStatusExtendedData class.
        /// </summary>
        public TrainBaselineStatusExtendedData() : this(null)
        {
            // No logic body
        }

        /// <summary>Gets or sets the basic status information.</summary>
        /// <value>The status.</value>
        public TrainBaselineStatusData Status
        {
            get;
            set;
        }

        /// <summary>Gets or sets the assigned future baseline.</summary>
        /// <value>The assigned future baseline.</value>
        public string AssignedFutureBaseline
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the assigned current baseline.
        /// </summary>
        /// <value>
        /// The assigned current baseline.
        /// </value>
        public string AssignedCurrentBaseline
        {
            get;
            set;
        }

        /// <summary>Gets or sets the on-board future baseline.</summary>
        /// <value>The on-board future baseline.</value>
        /// <remarks>Status.FutureBaselineVersion, in some conditions, may be forced artificially to the
        /// 		 AssignedFutureBaseline value. On the other hand, the OnBoardFutureBaseline
        /// 		 will always reflect the future baseline loaded on the train
        /// </remarks>
        public string OnBoardFutureBaseline
        {
            get;
            set;
        }

        /// <summary>Gets or sets the T2G polling required flag.</summary>
        /// <value>The flag.</value>
        public bool IsT2GPollingRequired
        {
            get;
            set;
        }

        /// <summary>Makes a deep copy of the current TrainBaselineStatusExtendedData object.</summary>
        /// <returns>A copy of the original object.</returns>
        public TrainBaselineStatusExtendedData Clone()
        {
            TrainBaselineStatusExtendedData lExtendedCopy = (TrainBaselineStatusExtendedData)this.MemberwiseClone();

            if (Status != null)
            {
                lExtendedCopy.Status = Status.Clone();
            }

            return lExtendedCopy;
        }

        /// <summary>Compares the content of two TrainBaselineStatusExtendedData objects.</summary>
        /// <param name="object1">First object to be compared.</param>
        /// <param name="object2">Second object to be compared.</param>
        /// <returns>true if equal, false if not.</returns>
        public static bool AreEqual(TrainBaselineStatusExtendedData object1, TrainBaselineStatusExtendedData object2)
        {
            bool lEqual = object1 != null
                    && object2 != null
                    && object1.AssignedFutureBaseline == object2.AssignedFutureBaseline
                    && object1.AssignedCurrentBaseline == object2.AssignedCurrentBaseline
                    && object1.IsT2GPollingRequired == object2.IsT2GPollingRequired
                    && object1.OnBoardFutureBaseline == object2.OnBoardFutureBaseline
                    && TrainBaselineStatusData.AreEqual(object1.Status, object2.Status);

            return lEqual;
        }

        /// <summary>
        /// Fetch information from the specified system object and update the value of the managed object.
        /// </summary>
        /// <param name="info">The system information to synchronize with.</param>
        /// <exception cref="ArgumentNullException">info is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified info object has a different system identifier than current object.</exception>
        public void Update(SystemInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            else if (Status == null)
            {
                Status = new TrainBaselineStatusData(info.SystemId, info.VehiclePhysicalId, info.IsOnline, BaselineStatusUpdater.NoBaselineVersion);
            }
            else if (info.SystemId != Status.TrainId)
            {
                throw new ArgumentOutOfRangeException("info");
            }

            ushort currentValue;
            if (!ushort.TryParse(Status.TrainNumber, NumberStyles.Integer, CultureInfo.InvariantCulture, out currentValue) || info.VehiclePhysicalId != currentValue)
            {
                Status.TrainNumber = info.VehiclePhysicalId.ToString(CultureInfo.InvariantCulture);
            }

            Status.OnlineStatus = info.IsOnline;
            if (info.PisVersion != null && !string.IsNullOrEmpty(info.PisVersion.VersionPISSoftware))
            {
                Status.PisOnBoardVersion = info.PisVersion.VersionPISSoftware;
            }


            if (info.PisBaseline != null)
            {

                string lNotificationCurrentVersion = info.PisBaseline.CurrentVersionOut;
                string lNotificationFutureVersion = info.PisBaseline.FutureVersionOut;

                if (lNotificationCurrentVersion != null &&
                    lNotificationFutureVersion != null)
                {
                    if (lNotificationCurrentVersion.Length == 0)
                    {
                        lNotificationCurrentVersion = BaselineStatusUpdater.NoBaselineVersion;
                    }

                    if (lNotificationFutureVersion.Length == 0)
                    {
                        lNotificationFutureVersion = BaselineStatusUpdater.NoBaselineVersion;
                    }

                    if (Status == null)
                    {
                        OnBoardFutureBaseline = lNotificationFutureVersion;
                    }
                    else // Status != null
                    {
                        switch (Status.ProgressStatus)
                        {
                            case BaselineProgressStatusEnum.UPDATED:

                                if (Status.CurrentBaselineVersion != lNotificationCurrentVersion)
                                {
                                    // The current baseline changed unexpectedly

                                    Status.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
                                    Status.RequestId = Guid.Empty;
                                    Status.TaskId = 0;
                                    Status.CurrentBaselineVersion = lNotificationCurrentVersion;
                                }

                                Status.FutureBaselineVersion = lNotificationFutureVersion;
                                OnBoardFutureBaseline = lNotificationFutureVersion;
                                IsT2GPollingRequired = false;
                                break;

                            case BaselineProgressStatusEnum.DEPLOYED:

                                if (Status.FutureBaselineVersion == lNotificationCurrentVersion)
                                {
                                    // The future baseline has been promoted to current

                                    Status.ProgressStatus = BaselineProgressStatusEnum.UPDATED;
                                }
                                else if (Status.FutureBaselineVersion != lNotificationFutureVersion)
                                {
                                    // The future baseline changed unexpectedly

                                    Status.ProgressStatus = BaselineProgressStatusEnum.UNKNOWN;
                                    Status.RequestId = Guid.Empty;
                                    Status.TaskId = 0;
                                }
                                else
                                {
                                    // Nothing special occurred
                                }

                                Status.CurrentBaselineVersion = lNotificationCurrentVersion;
                                Status.FutureBaselineVersion = lNotificationFutureVersion;
                                OnBoardFutureBaseline = lNotificationFutureVersion;
                                IsT2GPollingRequired = false;
                                break;

                            case BaselineProgressStatusEnum.TRANSFER_COMPLETED:
                            case BaselineProgressStatusEnum.TRANSFER_IN_PROGRESS:
                            case BaselineProgressStatusEnum.TRANSFER_PAUSED:
                            case BaselineProgressStatusEnum.TRANSFER_PLANNED:

                                if (Status.FutureBaselineVersion == lNotificationCurrentVersion)
                                {
                                    // The assigned baseline has been promoted to current baseline
                                    // Note: In the current state, the future baseline is set to the assigned baseline.

                                    Status.ProgressStatus = BaselineProgressStatusEnum.UPDATED;
                                    Status.FutureBaselineVersion = lNotificationFutureVersion;
                                    OnBoardFutureBaseline = lNotificationFutureVersion;
                                    IsT2GPollingRequired = false;
                                }
                                else if (Status.FutureBaselineVersion == lNotificationFutureVersion)
                                {
                                    // The assigned baseline has been promoted to future baseline
                                    // Note: In the current state, the future baseline is set to the assigned baseline.

                                    Status.ProgressStatus = BaselineProgressStatusEnum.DEPLOYED;
                                    OnBoardFutureBaseline = lNotificationFutureVersion;
                                    IsT2GPollingRequired = false;
                                }
                                else
                                {
                                    AssignedFutureBaseline = Status.FutureBaselineVersion;
                                    OnBoardFutureBaseline = lNotificationFutureVersion;
                                }

                                // Do not update the future baseline since in the current state,
                                // it is set to the assigned baseline (not the on board baseline)
                                Status.CurrentBaselineVersion = lNotificationCurrentVersion;

                                break;

                            case BaselineProgressStatusEnum.UNKNOWN:

                                if (Status.RequestId != Guid.Empty)
                                {
                                    string lAssignedBaseline = AssignedFutureBaseline;

                                    if (string.IsNullOrEmpty(lAssignedBaseline) || lAssignedBaseline == BaselineStatusUpdater.NoBaselineVersion)
                                    {
                                        lAssignedBaseline = AssignedCurrentBaseline;
                                    }

                                    if (!string.IsNullOrEmpty(lAssignedBaseline) && lAssignedBaseline != BaselineStatusUpdater.NoBaselineVersion)
                                    {
                                        if (lAssignedBaseline == lNotificationCurrentVersion)
                                        {
                                            // The assigned baseline has been promoted to current baseline

                                            Status.ProgressStatus = BaselineProgressStatusEnum.UPDATED;
                                            IsT2GPollingRequired = false;
                                        }
                                        else if (lAssignedBaseline == lNotificationFutureVersion)
                                        {
                                            // The assigned baseline has been promoted to future baseline
                                            // Note: In the current state, the future baseline is set to the assigned baseline.

                                            Status.ProgressStatus = BaselineProgressStatusEnum.DEPLOYED;
                                            IsT2GPollingRequired = false;
                                        }
                                        else
                                        {
                                            // Nothing to do
                                        }
                                    }
                                }

                                Status.CurrentBaselineVersion = lNotificationCurrentVersion;
                                Status.FutureBaselineVersion = lNotificationFutureVersion;
                                OnBoardFutureBaseline = lNotificationFutureVersion;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(350);

            result.Append("<Status=");
            result.Append((Status == null)? "(null)" : Status.ToString());
            result.Append(", AssignedCurrentBaseline=").Append(AssignedCurrentBaseline);
            result.Append(", AssignedFutureBaseline=").Append(AssignedFutureBaseline);
            result.Append(", IsT2GPollingRequired=").Append(IsT2GPollingRequired);
            result.Append(", OnBoardFutureBaseline=").Append(OnBoardFutureBaseline);
            result.Append(">");
            return result.ToString();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return AreEqual(this, obj as TrainBaselineStatusExtendedData);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Status != null ? (Status.TrainId ?? string.Empty).GetHashCode() : string.Empty.GetHashCode();
        }
    }
}