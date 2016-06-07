//---------------------------------------------------------------------------------------------------
// <copyright file="LmtDatabaseStation.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2014.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PIS.Ground.Core.PackageAccess
{
	/// <summary>Class representing a Station with data from lmt database.</summary>
	public class Station
	{
		/// <summary>Initializes a new instance of the Station class.</summary>
		public Station()
		{
			this.Names = new List<StationName>();
		}

		/// <summary>Gets or sets the operator code.</summary>
		/// <value>The operator code.</value>
		public string OperatorCode { get; set; }

		/// <summary>Gets or sets the names.</summary>
		/// <value>The names.</value>
		[XmlArrayItem("Name")]
		public List<StationName> Names { get; set; }
	}

	/// <summary>Station name.</summary>
	public class StationName
	{
		/// <summary>Gets or sets the language.</summary>
		/// <value>The language.</value>
		[XmlAttribute]
		public string Language { get; set; }

		/// <summary>Gets or sets the name.</summary>
		/// <value>The name for the .</value>
		[XmlText]
		public string Name { get; set; }
	}
}