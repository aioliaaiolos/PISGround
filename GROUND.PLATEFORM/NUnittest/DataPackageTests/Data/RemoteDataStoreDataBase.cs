//---------------------------------------------------------------------------------------------------
// <copyright file="RemoteDataStoreDataBase.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using PIS.Ground.RemoteDataStore;

namespace DataPackageTests.Data
{
    /// <summary>
    /// Base class to describe remote data store data.
    /// </summary>
    public class RemoteDataStoreDataBase : DataContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDataStoreDataBase"/> class.
        /// </summary>
        /// <param name="fieldNames">The field names.</param>
        protected RemoteDataStoreDataBase(string[] fieldNames)
        {
            Columns.Capacity = fieldNames.Length;
            Rows.Capacity = fieldNames.Length;
            Columns.AddRange(fieldNames);
        }

        protected RemoteDataStoreDataBase(DataContainer other)
        {
            Columns.AddRange(other.Columns);
            Rows.AddRange(other.Rows);
        }

        public void AddRow(string[] values)
        {
            if (Columns.Count != values.Length)
            {
                throw new ArgumentOutOfRangeException("values", "The values count shall be equals to columns count.");
            }

            Rows.AddRange(values);
        }

        /// <summary>
        /// Convert the specified date object to a string in the formation of the remote data store.
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents the provided date.
        /// </returns>
        public static string ToString(DateTime date)
        {
            return date.ToString();
        }
    }
}
