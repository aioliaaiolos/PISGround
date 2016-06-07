//---------------------------------------------------------------------------------------------------
// <copyright file="IRequestContextFactory.cs" company="Alstom">
//          (c) Copyright ALSTOM 2015.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using PIS.Ground.Core.Data;

namespace PIS.Ground.DataPackage.RequestMgt
{
	/// <summary>Interface for request context factory.</summary>
	public interface IRequestContextFactory
	{
		/// <summary>Creates baseline forcing request context.</summary>
		/// <param name="endpoint">The endpoint.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="timeout">The timeout.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <returns>The new baseline forcing request context.</returns>
		IRequestContext CreateBaselineForcingRequestContext(
			string endpoint,
			string elementId,
			Guid requestId,
			Guid sessionId,
			uint timeout,
			BaselineCommandType commandType);

		/// <summary>Creates baseline setting request context.</summary>
		/// <param name="endpoint">The endpoint.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="timeout">The timeout.</param>
		/// <param name="baselineVersion">The baseline version.</param>
		/// <param name="PISBaseVersion">The pis base version.</param>
		/// <param name="PISMissionVersion">The pis mission version.</param>
		/// <param name="PISInfotainmentVersion">The pis infotainment version.</param>
		/// <param name="LMTVersion">The lmt version.</param>
		/// <returns>The new baseline setting request context.</returns>
		IRequestContext CreateBaselineSettingRequestContext(
			string endpoint,
			string elementId,
			Guid requestId,
			Guid sessionId,
			uint timeout,
			string baselineVersion,
			string PISBaseVersion,
			string PISMissionVersion,
			string PISInfotainmentVersion,
			string LMTVersion);

		/// <summary>Creates baseline distributing request context.</summary>
		/// <param name="endpoint">The endpoint.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="distributionAttributes">The distribution attributes.</param>
		/// <param name="incremental">True if the request is incremental.</param>
		/// <param name="baselineVersion">The baseline version.</param>
		/// <param name="baselineActivationDate">Date of the baseline activation.</param>
		/// <param name="baselineExpiratoinDate">Date of the baseline expiratoin.</param>
		/// <param name="remoteDataStoreFactory">The remote data store factory.</param>
		/// <param name="trainToGroundManager">Manager for train to ground.</param>
		/// <returns>The new baseline distributing request context.</returns>
		IRequestContext CreateBaselineDistributingRequestContext(
			string endpoint,
			string elementId,
			Guid requestId,
			Guid sessionId,
			BaselineDistributionAttributes distributionAttributes,
			bool incremental,
			string baselineVersion,
			DateTime baselineActivationDate,
			DateTime baselineExpiratoinDate,
			RemoteDataStoreFactory.IRemoteDataStoreFactory remoteDataStoreFactory,
			Core.T2G.IT2GManager trainToGroundManager);

		/// <summary>Creates baseline distributing request context.</summary>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="transferMode">The transfer mode.</param>
		/// <param name="fileCompression">True to file compression.</param>
		/// <param name="isIncremental">True if this object is incremental.</param>
		/// <param name="transferDate">Date of the transfer.</param>
		/// <param name="transferExpirationDate">Date of the transfer expiration.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="baselineVersion">The baseline version.</param>
		/// <param name="baselineActivationDate">Date of the baseline activation.</param>
		/// <param name="baselineExpiratoinDate">Date of the baseline expiratoin.</param>
		/// <param name="remoteDataStoreFactory">The remote data store factory.</param>
		/// <param name="trainToGroundManager">Manager for train to ground.</param>
		/// <returns>The new baseline distributing request context.</returns>
		IRequestContext CreateBaselineDistributingRequestContext(
				string elementId,
				Guid requestId,
				Core.Data.FileTransferMode transferMode,
				bool fileCompression,
				bool isIncremental,
				DateTime transferDate,
				DateTime transferExpirationDate,
				sbyte priority,
				string baselineVersion,
				DateTime baselineActivationDate,
				DateTime baselineExpiratoinDate,
				RemoteDataStoreFactory.IRemoteDataStoreFactory remoteDataStoreFactory,
				Core.T2G.IT2GManager trainToGroundManager);
	}
}