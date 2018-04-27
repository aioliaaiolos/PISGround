//---------------------------------------------------------------------------------------------------
// <copyright file="TrainBaselineStatusData.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

/// Class used to store the train baseline status information.
/// 
namespace PIS.Ground.Core.Data
{
    /// <summary>
    /// Collection of Types
    /// </summary>
    /// <typeparam name="T">Type of which the collection holds</typeparam>
    [CollectionDataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "TrainBaselineStatus{0}List")]
    public class TrainBaselineStatusList<T> : List<T>
    {
    }

    [DataContract(Namespace = "http://alstom.com/pacis/pis/schema/", Name = "TrainBaselineStatusData")]
    public class TrainBaselineStatusData
    {
        private string _trainId;
        private Guid _requestId;
        private int _taskId;
        private string _trainNumber;
        private bool _onlineStatus;
        private BaselineProgressStatusEnum _progressStatus;
        private BaselineProgressStatusStateEnum _progressStatusState;
        private string _currentBaselineVersion;
        private string _futureBaselineVersion;
        private string _pisOnBoardVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainBaselineStatusData"/> class.
        /// </summary>
        public TrainBaselineStatusData()
        {
            _trainId = string.Empty;
            _requestId = new Guid();
            _taskId = 0;
            _trainNumber = string.Empty;
            _onlineStatus = false;
            _progressStatus = BaselineProgressStatusEnum.UNKNOWN;
            _currentBaselineVersion = string.Empty;
            _futureBaselineVersion = string.Empty;
            _pisOnBoardVersion = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainBaselineStatusData"/> class.
        /// </summary>
        /// <param name="trainId">The train identifier.</param>
        /// <param name="trainNumber">The train number.</param>
        /// <param name="isOnline">Indicates the online status of the train.</param>
        /// <param name="currentBaselineVersion">The current baseline version.</param>
        public TrainBaselineStatusData(string trainId, int trainNumber, bool isOnline, string currentBaselineVersion)
            : this()
        {
            _trainId = trainId;
            _trainNumber = trainNumber.ToString(CultureInfo.InvariantCulture);
            _onlineStatus = isOnline;
            _currentBaselineVersion = currentBaselineVersion;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainBaselineStatusData"/> class.
        /// </summary>
        /// <param name="trainId">The train identifier.</param>
        /// <param name="trainNumber">The train number.</param>
        /// <param name="isOnline">Indicates the online status of the train.</param>
        /// <param name="currentBaselineVersion">The current baseline version.</param>
        /// <param name="futureBaselineVersion">The future baseline version.</param>
        /// <param name="pisOnboardVersion">The pis onboard version.</param>
        /// <param name="progressStatus">The progress status.</param>
        public TrainBaselineStatusData(string trainId, int trainNumber, bool isOnline, string currentBaselineVersion, string futureBaselineVersion, string pisOnboardVersion, BaselineProgressStatusEnum progressStatus)
            : this(trainId, trainNumber, isOnline, currentBaselineVersion)
        {
            _futureBaselineVersion = futureBaselineVersion;
            _pisOnBoardVersion = pisOnboardVersion;
            _progressStatus = progressStatus;
        }

        [DataMember]
        public string TrainId
        {
            get { return _trainId; }
            set { _trainId = value; }
        }

        [DataMember]
        public Guid RequestId
        {
            get { return _requestId; }
            set { _requestId = value; }
        }

        [DataMember]
        public int TaskId
        {
            get { return _taskId; }
            set { _taskId = value; }
        }

        [DataMember]
        public string TrainNumber
        {
            get { return _trainNumber; }
            set { _trainNumber = value; }
        }

        [DataMember]
        public bool OnlineStatus
        {
            get { return _onlineStatus; }
            set { _onlineStatus = value; }
        }

        [DataMember]
        public BaselineProgressStatusEnum ProgressStatus
        {
            get 
            { 
                return _progressStatus; 
            }

            set 
            { 
                _progressStatus = value;
                Console.Out.WriteLine("       progressStatus: {0}", _progressStatus);
            }
        }

        [DataMember]
        public BaselineProgressStatusStateEnum ProgressStatusState
        {
            get 
            { 
                return _progressStatusState; 
            }

            set 
            { 
                _progressStatusState = value;
            }
        }

        [DataMember]
        public string CurrentBaselineVersion
        {
            get { return _currentBaselineVersion; }
            set { _currentBaselineVersion = value; }
        }

        [DataMember]
        public string FutureBaselineVersion
        {
            get { return _futureBaselineVersion; }
            set { _futureBaselineVersion = value; }
        }

        [DataMember]
        public string PisOnBoardVersion
        {
            get { return _pisOnBoardVersion; }
            set { _pisOnBoardVersion = value; }
        }

		/// <summary>Makes a deep copy of the current object.</summary>
		/// <returns>A copy of this object.</returns>
		public TrainBaselineStatusData Clone()
		{
			// Since all reference members are strings and since strings are immutables,
			// a shallow copy is equivalent to a deep copy
			return (TrainBaselineStatusData)this.MemberwiseClone();
		}

		/// <summary>Compares the content of two TrainBaselineStatusData objects.</summary>
		/// <param name="object1">First object to be compared.</param>
		/// <param name="object2">Second object to be compared.</param>
		/// <returns>true if equal, false if not.</returns>
		public static bool AreEqual(TrainBaselineStatusData object1, TrainBaselineStatusData object2)
		{
			bool lEqual = true;

			if(object1 != null && object2 != null)
			{
				if (Guid.Equals(object1.RequestId, object2.RequestId) == false)
				{
					lEqual = false;
				}
				else if (object1.TrainNumber != object2.TrainNumber)
				{
					lEqual = false;
				}
				else if (object1.OnlineStatus != object2.OnlineStatus)
				{
					lEqual = false;
				}
				else if (object1.ProgressStatus != object2.ProgressStatus)
				{
					lEqual = false;
				}
				else if (object1.CurrentBaselineVersion != object2.CurrentBaselineVersion)
				{
					lEqual = false;
				}
				else if (object1.FutureBaselineVersion != object2.FutureBaselineVersion)
				{
					lEqual = false;
				}
				else if (object1.PisOnBoardVersion != object2.PisOnBoardVersion)
				{
					lEqual = false;
				}
				else if (object1.TaskId != object2.TaskId)
				{
					lEqual = false;
				}
			}
			else
			{
				lEqual = false;
			}

			return lEqual;
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
            return AreEqual(this, obj as TrainBaselineStatusData);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Convert.ToInt32(_trainNumber, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder(150);

            output.Append("<id='").Append(TrainId).Append("'");
            output.Append(", vehicleId=").Append(TrainNumber);
            output.Append(", request='").Append(RequestId).Append("'");
            output.Append(", task=").Append(TaskId);
            output.Append(", online=").Append(OnlineStatus);
            output.Append(", progress=").Append(ProgressStatus);
            output.Append(", current=").Append(CurrentBaselineVersion);
            output.Append(", future=").Append(FutureBaselineVersion);
            output.Append(", pisVersion=").Append(PisOnBoardVersion);
            output.Append(">");
            return output.ToString();
        }
    }
}
