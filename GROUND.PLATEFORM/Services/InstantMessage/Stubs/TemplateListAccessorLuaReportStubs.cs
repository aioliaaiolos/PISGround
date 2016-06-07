// <copyright file="TemplateListAccessorLuaReportStubs.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2014.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
namespace PIS.Ground.InstantMessage.TemplateListAccessorLuaStubs
{
    using System.Collections.Generic;
    using PIS.Ground.Common.LuaPluginLoader.Attributes;

    [LuaPluginClass("report")]
    public class LuaReportPlugin
    {
        [LuaPluginMethod]
        public object open(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object close(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public bool write(params object[] args)
        {
            return true;
        }
    }
}
