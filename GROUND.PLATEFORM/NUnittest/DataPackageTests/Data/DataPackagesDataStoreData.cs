//---------------------------------------------------------------------------------------------------
// <copyright file="DataPackagesDataStoreData.cs" company="Alstom">
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
using PIS.Ground.DataPackage;

namespace DataPackageTests.Data
{
    /// <summary>
    /// Describes the data for DataPackagesDataStore table in remote data store.
    /// </summary>
    /// <seealso cref="DataPackageTests.Data.RemoteDataStoreDataBase" />
    public class DataPackagesDataStoreData : RemoteDataStoreDataBase
    {
        public static readonly string[] FieldNames = { "DataPackageType", 
                                                         "DataPackageVersion", 
                                                         "DataPackagePath", 
                                                         "DataPackageLastOpenDate" };

        public DataPackagesDataStoreData(DataPackageType type, string version) : this(type, version, DateTime.Now)
        {
            /* No logic body */
        }

        public DataPackagesDataStoreData(DataPackageType type, string version, DateTime lastOpenDate) :
            base(FieldNames)
        {
            string typeString = type.ToString();
            string path = "/" + typeString + "/" + type + "-" + version + ".zip";
            string[] values = { typeString, version, path, ToString(lastOpenDate)};
            AddRow(values);
        }

    }
}
