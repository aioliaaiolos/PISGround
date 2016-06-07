//---------------------------------------------------------------------------------------------------
// <copyright file="ProjectInstaller.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;


namespace Alstom.Product.PISGroundKicker
{
	/// <summary>Project installer.</summary>
	[RunInstaller(true)]
	public partial class ProjectInstaller : Installer
	{
		/// <summary>Initializes a new instance of the ProjectInstaller class.</summary>
		public ProjectInstaller()
		{
			InitializeComponent();
		}

		// Override the 'OnAfterInstall' method.
		protected override void OnAfterInstall(IDictionary savedState)
		{
			base.OnAfterInstall(savedState);
			// Add steps to be done after the installation is over.
			ServiceController service = new ServiceController("Alstom.Product.PISGroundKicker");
			try
			{
				service.Start();
				service.WaitForStatus(ServiceControllerStatus.Running);
			}
			catch
			{
				// No can do.
			}
		}
	}
}
