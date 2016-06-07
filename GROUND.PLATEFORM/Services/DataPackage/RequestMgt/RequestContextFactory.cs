//---------------------------------------------------------------------------------------------------
// <copyright file="RequestContextFactory.cs" company="Alstom">
//          (c) Copyright ALSTOM 2015.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PIS.Ground.DataPackage.RequestMgt
{
	/// <summary>Request context factory.</summary>
	public class RequestContextFactory : IRequestContextFactory
	{
		/// <summary>Initializes a new instance of the RequestContextFactory class.</summary>
		public RequestContextFactory()
		{
		}

		#region IRequestContextFactory Members

		/// <summary>Creates baseline forcing request context.</summary>
		/// <param name="endpoint">The endpoint.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="timeout">The timeout.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <returns>The new baseline forcing request context.</returns>
		public PIS.Ground.Core.Data.IRequestContext CreateBaselineForcingRequestContext(string endpoint, string elementId, Guid requestId, Guid sessionId, uint timeout, PIS.Ground.DataPackage.BaselineCommandType commandType)
		{
			BaselineForcingRequestContext requestContext = new BaselineForcingRequestContext(endpoint, elementId, requestId, sessionId, timeout, commandType);
			PIS.Ground.Core.Data.IRequestContextProcessor requestProcessor = new BaselineForcingRequestContextProcessor();
			requestContext.RequestProcessor = requestProcessor;
			return requestContext;
		}

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
		public PIS.Ground.Core.Data.IRequestContext CreateBaselineSettingRequestContext(string endpoint, string elementId, Guid requestId, Guid sessionId, uint timeout, string baselineVersion, string PISBaseVersion, string PISMissionVersion, string PISInfotainmentVersion, string LMTVersion)
		{
			BaselineSettingRequestContext requestContext = new BaselineSettingRequestContext(endpoint, elementId, requestId, sessionId, timeout, baselineVersion, PISBaseVersion, PISMissionVersion, PISInfotainmentVersion, LMTVersion);
			PIS.Ground.Core.Data.IRequestContextProcessor requestProcessor = new BaselineSettingRequestContextProcessor();
			requestContext.RequestProcessor = requestProcessor;
			return requestContext;
		}

		/// <summary>Creates baseline distributing request context.</summary>
		/// <param name="endpoint">The endpoint.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="distributionAttributes">The distribution attributes.</param>
		/// <param name="incremental">True if the request is incremental.</param>
		/// <param name="baselineVersion">The baseline version.</param>
		/// <param name="baselineActivationDate">Date of the baseline activation.</param>
		/// <param name="baselineExpirationDate">Date of the baseline expiration.</param>
		/// <param name="remoteDataStoreFactory">The remote data store factory.</param>
		/// <param name="trainToGroundManager">Manager for train to ground.</param>
		/// <returns>The new baseline distributing request context.</returns>
		public PIS.Ground.Core.Data.IRequestContext CreateBaselineDistributingRequestContext(string endpoint, string elementId, Guid requestId, Guid sessionId, BaselineDistributionAttributes distributionAttributes, bool incremental, string baselineVersion, DateTime baselineActivationDate, DateTime baselineExpirationDate, RemoteDataStoreFactory.IRemoteDataStoreFactory remoteDataStoreFactory, Core.T2G.IT2GManager trainToGroundManager)
		{
			BaselineDistributingRequestContext requestContext = new BaselineDistributingRequestContext(endpoint, elementId, requestId, sessionId, distributionAttributes, incremental, baselineVersion, baselineActivationDate, baselineExpirationDate);
			PIS.Ground.Core.Data.IRequestContextProcessor requestProcessor = new BaselineDistributingRequestContextProcessor(remoteDataStoreFactory, trainToGroundManager);
			requestContext.RequestProcessor = requestProcessor;
			return requestContext;
		}

		/// <summary>Creates baseline distributing request context.</summary>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="transferMode">The transfer mode.</param>
		/// <param name="fileCompression">True if file compression is used.</param>
		/// <param name="isIncremental">True if this request is incremental.</param>
		/// <param name="transferDate">Date of the transfer.</param>
		/// <param name="transferExpirationDate">Date of the transfer expiration.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="baselineVersion">The baseline version.</param>
		/// <param name="baselineActivationDate">Date of the baseline activation.</param>
		/// <param name="baselineExpirationDate">Date of the baseline expiration.</param>
		/// <param name="remoteDataStoreFactory">The remote data store factory.</param>
		/// <param name="trainToGroundManager">Manager for train to ground.</param>
		/// <returns>The new baseline distributing request context.</returns>
		public PIS.Ground.Core.Data.IRequestContext CreateBaselineDistributingRequestContext(string elementId, Guid requestId, Core.Data.FileTransferMode transferMode, bool fileCompression, bool isIncremental, DateTime transferDate, DateTime transferExpirationDate, sbyte priority, string baselineVersion, DateTime baselineActivationDate, DateTime baselineExpirationDate, RemoteDataStoreFactory.IRemoteDataStoreFactory remoteDataStoreFactory, Core.T2G.IT2GManager trainToGroundManager)
		{
			BaselineDistributionAttributes distributionAttributes = new BaselineDistributionAttributes();
			distributionAttributes.fileCompression = fileCompression;
			distributionAttributes.priority = priority;
			distributionAttributes.transferDate = transferDate;
			distributionAttributes.transferExpirationDate = transferExpirationDate;
			distributionAttributes.TransferMode = transferMode;

			BaselineDistributingRequestContext requestContext = new BaselineDistributingRequestContext(null, elementId, requestId, Guid.Empty, distributionAttributes, isIncremental, baselineVersion, baselineActivationDate, baselineExpirationDate);
			PIS.Ground.Core.Data.IRequestContextProcessor requestProcessor = new BaselineDistributingRequestContextProcessor(remoteDataStoreFactory, trainToGroundManager);
			requestContext.RequestProcessor = requestProcessor;
			return requestContext;
		}

		#endregion
	}
}