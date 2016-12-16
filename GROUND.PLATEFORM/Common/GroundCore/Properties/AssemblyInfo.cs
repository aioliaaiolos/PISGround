//---------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// For unit test purpose
[assembly: InternalsVisibleTo("GroundCoreTests")]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("PIS2GGround.Core")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Alstom")]
[assembly: AssemblyProduct("PIS2GGround.Core")]
[assembly: AssemblyCopyright("Copyright © Alstom 2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("ea8ed72d-0041-4685-a55c-1e5b5360d0ff")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("5.16.10.1")]
[assembly: AssemblyFileVersion("5.16.10.1")]
[assembly: NeutralResourcesLanguageAttribute("en")]

[assembly: InternalsVisibleTo("UnitTests")]
[assembly: InternalsVisibleTo("InstantMessageTests")]