//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineDistributingTasksDataStoreData.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PIS.Ground.RemoteDataStore;

namespace DataPackageTests.Data
{
    /// <summary>
    /// Describes the data for BaselineDistributingTasksDataStore table in remote data store.
    /// </summary>
    /// <seealso cref="DataPackageTests.Data.RemoteDataStoreDataBase" />
    class BaselineDistributingTasksDataStoreData : RemoteDataStoreDataBase
    {
        public static readonly string[] FieldNames = { "ElementID", 
                                                         "RequestID", 
                                                         "TransferMode", 
                                                         "FileCompression", 
                                                         "TransferDate", 
                                                         "TransferExpirationDate", 
                                                         "Priority",
                                                         "Incremental",
                                                         "BaselineVersion",
                                                         "BaselineActivationDate",
                                                         "BaselineExpirationDate"
                                                     };

        /// <summary>
        /// Gets or sets the element identifier.
        /// </summary>
        public string ElementId
        {
            get
            {
                return Rows[0];
            }

            set
            {
                Rows[0] = value;
            }
        }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        public string RequestId
        {
            get
            {
                return Rows[1];
            }

            set
            {
                Rows[1] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaselineDistributingTasksDataStoreData"/> class.
        /// </summary>
        public BaselineDistributingTasksDataStoreData()
            : base(FieldNames)
        {
            /* No logic body */
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaselineDistributingTasksDataStoreData"/> class with another instance.
        /// </summary>
        /// <param name="other">The other object to copy.</param>
        public BaselineDistributingTasksDataStoreData(DataContainer other)
            : base(other)
        {
            if (other.Columns.Count != FieldNames.Length)
            {
                throw new ArgumentException("Provided container does not describe a baseline definition in data store.", "other");
            }
        }
    }
}
