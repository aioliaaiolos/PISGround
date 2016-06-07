//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineDistributingRequestContext.cs" company="Alstom">
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
using PIS.Ground.Core.Data;
using PIS.Ground.DataPackage;

namespace PIS.Ground.DataPackage.RequestMgt
{
	/// <summary>Implementation of the request context specific to baseline distribution.</summary>
	public class BaselineDistributingRequestContext : RequestContext
	{
		/// <summary>The distribution attributes.</summary>
		private readonly BaselineDistributionAttributes _distributionAttributes;

		/// <summary>True if this request is incremental.</summary>
		private readonly bool _isIncremental;

		/// <summary>The baseline version.</summary>
		private readonly string _baselineVersion;

		/// <summary>Date of the baseline activation.</summary>
		private readonly DateTime _baselineActivationDate;

		/// <summary>Date of the baseline expiratoin.</summary>
		private readonly DateTime _baselineExpirationDate;

		/// <summary>
		/// Initializes a new instance of the BaselineDistributingRequestContext class.
		/// </summary>
		protected BaselineDistributingRequestContext()
			: base(string.Empty, string.Empty, Guid.Empty, Guid.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the BaselineDistributingRequestContext class.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
		/// <param name="endpoint">The endpoint.</param>
		/// <param name="elementId">Identifier for the element.</param>
		/// <param name="requestId">Identifier for the request.</param>
		/// <param name="sessionId">Identifier for the session.</param>
		/// <param name="distributionAttributes">The distribution attributes.</param>
		/// <param name="incremental">True if the request is incremental.</param>
		/// <param name="baselineVersion">The baseline version.</param>
		/// <param name="baselineActivationDate">Date of the baseline activation.</param>
		/// <param name="baselineExpirationDate">Date of the baseline expiratoin.</param>
		public BaselineDistributingRequestContext(string endpoint, string elementId, Guid requestId, Guid sessionId, BaselineDistributionAttributes distributionAttributes, bool incremental, string baselineVersion, DateTime baselineActivationDate, DateTime baselineExpirationDate)
			: base(endpoint, elementId, requestId, sessionId)
		{
			if (null == distributionAttributes)
			{
				throw new ArgumentNullException("distributionAttributes");
			}

			_distributionAttributes = distributionAttributes;
			_isIncremental = incremental;

			RequestTimeout = _distributionAttributes.transferExpirationDate > DateTime.Now ? Convert.ToUInt32((_distributionAttributes.transferExpirationDate - DateTime.Now).TotalMinutes) : 0;

			if (string.IsNullOrEmpty(baselineVersion))
			{
				throw new ArgumentNullException("baselineVersion");
			}

			_baselineVersion = baselineVersion;
			_baselineActivationDate = baselineActivationDate;
			_baselineExpirationDate = baselineExpirationDate;
		}

		/// <summary>Gets the distribution attributes.</summary>
		/// <value>The distribution attributes.</value>
		public BaselineDistributionAttributes DistributionAttributes
		{
			get
			{
				return _distributionAttributes;
			}
		}

		/// <summary>Gets a value indicating whether this request is incremental.</summary>
		/// <value>True if this request is incremental, false if not.</value>
		public bool IsIncremental
		{
			get
			{
				return _isIncremental;
			}
		}

		/// <summary>Gets the baseline version.</summary>
		/// <value>The baseline version.</value>
		public string BaselineVersion
		{
			get
			{
				return _baselineVersion;
			}
		}

		/// <summary>Gets the date of the baseline activation.</summary>
		/// <value>The date of the baseline activation.</value>
		public DateTime BaselineActivationDate
		{
			get
			{
				return _baselineActivationDate;
			}
		}

		/// <summary>Gets the date of the baseline expiration.</summary>
		/// <value>The date of the baseline expiration.</value>
		public DateTime BaselineExpirationDate
		{
			get
			{
				return _baselineExpirationDate;
			}
		}
	}
}