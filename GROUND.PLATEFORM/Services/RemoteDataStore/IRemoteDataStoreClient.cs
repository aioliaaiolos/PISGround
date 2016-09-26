//---------------------------------------------------------------------------------------------------
// <copyright file="IRemoteDataStoreClient.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using PIS.Ground.RemoteDataStore;

namespace PIS.Ground.RemoteDataStore
{

    /// <summary>
    /// Interface that describe a remote data store client.
    /// </summary>
    public interface IRemoteDataStoreClient : IRemoteDataStore, IDisposable
    {
        /// <summary>
        /// Gets or sets the operation timeout.
        /// </summary>
        TimeSpan OperationTimeout { get; set; }
    }
}
