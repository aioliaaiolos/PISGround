// <copyright file="TemplateListAccessorLuaMissionDataClientStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("mission_data")]
    public class LuaMissionDataClientPlugin
    {
        [LuaPluginMethod]
        public object get_connection(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_delay(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_mission_info(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_platform(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_weather(params object[] args)
        {
            return null;
        }
    }
}
