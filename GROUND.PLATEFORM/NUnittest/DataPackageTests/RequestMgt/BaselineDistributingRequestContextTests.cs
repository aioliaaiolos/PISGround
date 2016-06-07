//---------------------------------------------------------------------------------------------------
// <copyright file="BaselineDistributingRequestContextTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2015.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DataPackageTests.RequestMgt
{
	/// <summary>Baseline distributing request context tests.</summary>
	[TestFixture]
	class BaselineDistributingRequestContextTests
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorNoBaselineDistributionAttributes()
		{
			PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext requestContext = new PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext(
				string.Empty,
				string.Empty,
				Guid.NewGuid(),
				Guid.NewGuid(),
				null,
				false,
				null,
				DateTime.Now,
				DateTime.Now);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorBaselineDistributionAttributesBaselineNull()
		{
			PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext requestContext = new PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext(
				string.Empty,
				string.Empty,
				Guid.NewGuid(),
				Guid.NewGuid(),
				new	PIS.Ground.DataPackage.BaselineDistributionAttributes(),
				false,
				null,
				DateTime.Now,
				DateTime.Now);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorBaselineDistributionAttributesBaselineEmpty()
		{
			PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext requestContext = new PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext(
				string.Empty,
				string.Empty,
				Guid.NewGuid(),
				Guid.NewGuid(),
				new PIS.Ground.DataPackage.BaselineDistributionAttributes(),
				false,
				string.Empty,
				DateTime.Now,
				DateTime.Now);
		}

		[Test]
		public void ConstructorBaselineDistributionAttributesexpirationDateNotValid()
		{
			PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext requestContext = new PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext(
				string.Empty,
				string.Empty,
				Guid.NewGuid(),
				Guid.NewGuid(),
				new PIS.Ground.DataPackage.BaselineDistributionAttributes(),
				false,
				"1.2.3.4",
				DateTime.Now,
				DateTime.Now);

			Assert.AreEqual(0, requestContext.RequestTimeout);
		}

		[Test]
		public void ConstructorBaselineDistributionAttributesexpirationDateValid()
		{
			uint initialTime = 60;
			PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext requestContext = new PIS.Ground.DataPackage.RequestMgt.BaselineDistributingRequestContext(
				string.Empty,
				string.Empty,
				Guid.NewGuid(),
				Guid.NewGuid(),
				new PIS.Ground.DataPackage.BaselineDistributionAttributes() { transferExpirationDate = DateTime.Now.AddMinutes(60) },
				false,
				"1.2.3.4",
				DateTime.Now,
				DateTime.Now);

			Assert.LessOrEqual(requestContext.RequestTimeout, initialTime);
			Assert.LessOrEqual(60, requestContext.RequestTimeout);
		}
	}
}
