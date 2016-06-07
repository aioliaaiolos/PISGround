//---------------------------------------------------------------------------------------------------
// <copyright file="ILiveVideoControlConfiguration.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2014.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.LiveVideoControl
{
	/// <summary>Interface for the live video control configuration.</summary>
    public interface ILiveVideoControlConfiguration
	{
		/// <summary>
		/// Automatic mode URL (Null in manual mode).
		/// </summary>
		string AutomaticModeURL {get; set;}
	}
}
