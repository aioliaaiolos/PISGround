// <copyright file="TrainBaselineStatusData.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2014.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

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
        private string _currentBaselineVersion;
        private string _futureBaselineVersion;
        private string _pisOnBoardVersion;

        public TrainBaselineStatusData()
        {
            _trainId = "";
            _requestId = new Guid();
            _taskId = 0;
            _trainNumber = "";
            _onlineStatus = false;
            _progressStatus = BaselineProgressStatusEnum.UNKNOWN;
            _currentBaselineVersion = "";
            _futureBaselineVersion = "";
            _pisOnBoardVersion = "";
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
            get { return _progressStatus; }
            set { _progressStatus = value; }
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
    }
}
