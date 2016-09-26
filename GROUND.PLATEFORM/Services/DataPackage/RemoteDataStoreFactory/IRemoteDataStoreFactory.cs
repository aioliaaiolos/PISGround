﻿//---------------------------------------------------------------------------------------------------
// <copyright file="IRemoteDataStoreFactory.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using PIS.Ground.RemoteDataStore;

namespace PIS.Ground.DataPackage.RemoteDataStoreFactory
{
	/// <summary>Interface for remote data store factory.</summary>
	public interface IRemoteDataStoreFactory
	{
		/// <summary>Gets remote data store client.</summary>
		/// <returns>The remote data store client.</returns>
		IRemoteDataStoreClient GetRemoteDataStoreInstance();
	}
}