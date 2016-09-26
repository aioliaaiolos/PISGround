//---------------------------------------------------------------------------------------------------
// <copyright file="BaselinesDataStoreData.cs" company="Alstom">
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
    /// Describes the data for BaselinesDataStore table in remote data store.
    /// </summary>
    /// <seealso cref="DataPackageTests.Data.RemoteDataStoreDataBase" />
    class BaselinesDataStoreData : RemoteDataStoreDataBase
    {
        public static readonly string[] FieldNames = { "BaselineVersion", 
                                                         "BaselineDescription", 
                                                         "BaselineCreationDate", 
                                                         "PISBaseDataPackageVersion", 
                                                         "PISMissionDataPackageVersion", 
                                                         "PISInfotainmentDataPackageVersion", 
                                                         "LMTDataPackageVersion" };

        public string BaselineVersion
        {
            get
            {
                return Rows[0];
            }
        }

        public string Baselinedescription
        {
            get
            {
                return Rows[1];
            }
        }

        public string BaselineCreationDate
        {
            get
            {
                return Rows[2];
            }
        }

        public string PISBaseDataPackageVersion
        {
            get
            {
                return Rows[3];
            }
        }

        public string PISMissionDataPackageVersion
        {
            get
            {
                return Rows[4];
            }
        }

        public string PISInfotainmentDataPackageVersion
        {
            get
            {
                return Rows[5];
            }
        }

        public string LMTDataPackageVersion
        {
            get
            {
                return Rows[6];
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BaselinesDataStoreData"/> class.
        /// </summary>
        public BaselinesDataStoreData()
            : base(FieldNames)
        {
            /* No logic body */
        }

                /// <summary>


        public BaselinesDataStoreData(string baselineVersion)
            : this(baselineVersion, DateTime.Today)
        {
            /* No logic body */
        }

        public BaselinesDataStoreData(string baselineVersion, DateTime creationDate) 
            : this()
        {
            string description = string.Concat("This is the baseline '", baselineVersion, "'.");
            string[] defaultData = { baselineVersion, description, ToString(creationDate), baselineVersion, baselineVersion, baselineVersion, baselineVersion };
            AddRow(defaultData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaselinesDataStoreData"/> class with another instance.
        /// </summary>
        /// <param name="other">The other object to copy.</param>
        public BaselinesDataStoreData(BaselinesDataStoreData other) : base(other)
        {
            /* No logic body */
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaselinesDataStoreData"/> class with another instance.
        /// </summary>
        /// <param name="other">The other object to copy.</param>
        public BaselinesDataStoreData(DataContainer other)
            : base(other)
        {
            if (other.Columns.Count != FieldNames.Length)
            {
                throw new ArgumentException("Provided container does not describe a baseline definition in data store.", "other");
            }
        }

    }
}
