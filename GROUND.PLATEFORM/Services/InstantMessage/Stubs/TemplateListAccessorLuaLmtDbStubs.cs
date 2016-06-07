// <copyright file="TemplateListAccessorLuaLmtDbStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("lmtdb")]
    public class LuaLmtDbPlugin
    {
        [LuaPluginMethod]
        public object get_station_long_name(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_station_short_name(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_station_list_long_names(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_station_list_short_name(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_station_code(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_service_name_list(params object[] args)
        {
            return null;
        }
    }
}
