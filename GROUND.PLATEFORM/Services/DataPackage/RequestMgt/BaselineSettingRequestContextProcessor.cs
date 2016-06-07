//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineForcingRequestContextProcessor.cs" company="Alstom">
//          (c) Copyright ALSTOM 2015.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.ServiceModel;
using PIS.Ground.Core.Data;
using PIS.Ground.DataPackage;

namespace PIS.Ground.DataPackage.RequestMgt
{
	/// <summary>Baseline forcing request context processor.</summary>
	public class BaselineSettingRequestContextProcessor : IRequestContextProcessor
	{
		/// <summary>
		/// Initializes a new instance of the BaselineSettingRequestContextProcessor class.
		/// </summary>
		public BaselineSettingRequestContextProcessor()
		{
		}

		/// <summary>Process the set request described by request.</summary>
		/// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
		/// <param name="request">The request.</param>
		private void ProcessSetRequest(IRequestContext request)
		{
			BaselineSettingRequestContext requestContext = request as BaselineSettingRequestContext;

			if (requestContext == null)
			{
				throw new NotSupportedException("Can't process setting baseline with a invalid request context.");
			}

			try
			{
				using (PIS.Ground.DataPackage.DataPackageTrainServiceClient lTrainDataPackageClient = new PIS.Ground.DataPackage.DataPackageTrainServiceClient("DataPackageEndpoint", requestContext.Endpoint))
				{
					try
					{
						string requestIdStr = request.RequestId.ToString();
						lTrainDataPackageClient.SetBaselineVersion(requestIdStr, requestContext.BaselineVersion, requestContext.PISBasePackageVersion, requestContext.PISMissionPackageVersion, requestContext.PISInfotainmentPackageVersion, requestContext.LmtPackageVersion);
						requestContext.TransmissionStatus = true;
						requestContext.CompletionStatus = true;
					}
					catch (Exception ex)
					{
						if (false == requestContext.OnCommunicationError(ex))
						{
							// At this stage nothing can be done, put the request state to completed in order to be deleted.
							requestContext.CompletionStatus = true;
						}
					}
					finally
					{
						if (lTrainDataPackageClient.State == CommunicationState.Faulted)
						{
							lTrainDataPackageClient.Abort();
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (false == requestContext.OnCommunicationError(ex))
				{
					requestContext.CompletionStatus = true;
				}
			}
		}

		#region IRequestContextProcessor Members

		/// <summary>Gets the process callback.</summary>
		/// <value>The process.</value>
		ProcessDelegate IRequestContextProcessor.Process
		{
			get
			{
				return ProcessSetRequest;
			}
		}

		#endregion
	}
}