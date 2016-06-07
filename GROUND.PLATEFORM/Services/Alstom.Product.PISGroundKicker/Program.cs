//---------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Alstom.Product.PISGroundKicker
{
	/// <summary>Program.</summary>
	static class Program
	{
		/// <summary>The main entry point for the application.</summary>
		static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new PISGroundKicker() 
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
