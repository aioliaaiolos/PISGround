//---------------------------------------------------------------------------------------------------
// <copyright file="RemoteDataStoreFactory.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using PIS.Ground.RemoteDataStore;

namespace PIS.Ground.DataPackage.RemoteDataStoreFactory
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

        /// <summary>Gets remote data store client.</summary>
        /// <returns>The remote data store client.</returns>
        PIS.Ground.RemoteDataStore.IRemoteDataStoreClient IRemoteDataStoreFactory.GetRemoteDataStoreInstance()
		{
			return new RemoteDataStoreProxy();
		}

		#endregion
	}
}