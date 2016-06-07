//---------------------------------------------------------------------------------------------------
// <copyright file="RemoteDataStoreFactory.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using PIS.Ground.RemoteDataStore;

namespace PIS.Ground.RealTime
{
	/// <summary>Remote data store factory.</summary>
	public class RemoteDataStoreFactory : IRemoteDataStoreFactory
	{
		#region Constructor

		/// <summary>Initializes a new instance of the RemoteDataStoreFactory class.</summary>
		public RemoteDataStoreFactory()
		{
			// Do Nothing
		}

		#endregion

		#region IRemoteDataStoreFactory Members

		/// <summary>Gets remote data store instance.</summary>
		/// <returns>The remote data store instance.</returns>
		PIS.Ground.RemoteDataStore.IRemoteDataStore IRemoteDataStoreFactory.GetRemoteDataStoreInstance()
		{
			return new RemoteDataStoreProxy();
		}

		#endregion
	}
}