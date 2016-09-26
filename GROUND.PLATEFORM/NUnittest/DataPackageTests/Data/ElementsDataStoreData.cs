//---------------------------------------------------------------------------------------------------
// <copyright file="ElementsDataStore.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace DataPackageTests.Data
{
    /// <summary>
    /// Describes the data for ElementsDataStore table in remote data store.
    /// </summary>
    /// <seealso cref="DataPackageTests.Data.RemoteDataStoreDataBase" />
    public class ElementsDataStoreData : RemoteDataStoreDataBase
    {
        public static readonly string[] FieldNames = { "ElementID", 
                                                         "AssignedCurrentBaseline", 
                                                         "AssignedCurrentBaselineExpirationDate", 
                                                         "AssignedFutureBaseline", 
                                                         "AssignedFutureBaselineActivationDate", 
                                                         "AssignedFutureBaselineExpirationDate", 
                                                         "UndefinedBaselinePISBaseVersion", 
                                                         "UndefinedBaselinePISMissionVersion", 
                                                         "UndefinedBaselinePISInfotainmentVersion", 
                                                         "UndefinedBaselineLmtVersion" };

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementsDataStoreData"/> class.
        /// </summary>
        public ElementsDataStoreData()
            : base(FieldNames)
        {
            /* No logic body */
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementsDataStoreData"/> class.
        /// </summary>
        /// <param name="elementId">The element identifier.</param>
        public ElementsDataStoreData(string elementId)
            : this()
        {
            string[] defaultData = { elementId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
            AddRow(defaultData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementsDataStoreData"/> class.
        /// </summary>
        /// <param name="other">The other object to copy.</param>
        public ElementsDataStoreData(ElementsDataStoreData other)
            : base(other)
        {

        }

        public string ElementID
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

        public string CurrentBaseline
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

        public string CurrentBaselineExpiration
        {
            get
            {
                return Rows[2];
            }
            set
            {
                Rows[2] = value;
            }
        }

        public string FutureBaseline
        {
            get
            {
                return Rows[3];
            }
            set
            {
                Rows[3] = value;
            }
        }

        public string FutureBaselineActivationDate
        {
            get
            {
                return Rows[4];
            }
            set
            {
                Rows[4] = value;
            }
        }

        public string FutureBaselineExpirationDate
        {
            get
            {
                return Rows[5];
            }
            set
            {
                Rows[5] = value;
            }
        }
    }
}
