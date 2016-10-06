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
	public class BaselineForcingRequestContextProcessor : IRequestContextProcessor
	{
		/// <summary>
		/// Initializes a new instance of the BaselineForcingRequestContextProcessor class.
		/// </summary>
		public BaselineForcingRequestContextProcessor()
		{
		}

		/// <summary>Process the force request described by request.</summary>
		/// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
		/// <param name="request">The request.</param>
		private void ProcessForceRequest(IRequestContext request)
		{
			BaselineForcingRequestContext requestContext = request as BaselineForcingRequestContext;

			if (requestContext == null)
			{
				throw new NotSupportedException("Can't process forcing baseline with a invalid request context.");
			}

			try
			{
				using (PIS.Ground.DataPackage.DataPackageTrainServiceClient lTrainDataPackageClient = new PIS.Ground.DataPackage.DataPackageTrainServiceClient("DataPackageEndpoint", requestContext.Endpoint))
				{
					try
					{
						string requestIdStr = requestContext.RequestId.ToString();
						bool validCommand = true;
						switch (requestContext.CommandType)
						{
							case BaselineCommandType.FORCE_FUTURE:
								lTrainDataPackageClient.ForceFutureBaseline(requestIdStr);
								break;

							case BaselineCommandType.FORCE_ARCHIVED:
								lTrainDataPackageClient.ForceArchivedBaseline(requestIdStr);
								break;

							case BaselineCommandType.CLEAR_FORCING:
								lTrainDataPackageClient.CancelBaselineForcing(requestIdStr);
								break;

							default:
								validCommand = false;
								break;
						}

						if (validCommand)
						{
							if (requestContext.CommandType == BaselineCommandType.CLEAR_FORCING)
							{
								DataPackageService.sendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageBaselineClearForcingSent, request.ElementId);
							}
							else
							{
								DataPackageService.sendNotificationToGroundApp(request.RequestId, PIS.Ground.GroundCore.AppGround.NotificationIdEnum.DataPackageBaselineForcingSent, request.ElementId);
							}
							requestContext.TransmissionStatus = true;
							requestContext.CompletionStatus = true;
						}
					}
					catch (Exception ex)
					{
						if (false == requestContext.OnCommunicationError(ex))
						{
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
				return ProcessForceRequest;
			}
		}

		#endregion
	}
}