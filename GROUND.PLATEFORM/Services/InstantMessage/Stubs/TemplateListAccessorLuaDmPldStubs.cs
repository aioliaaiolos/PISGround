// <copyright file="TemplateListAccessorLuaDmPldStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("playerPld")]
    public class LuaPlayerPldPlugin
    {
        [LuaPluginConstant] 
        public string GROUP_ALL_DISPLAYS
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant] 
        public string SERVICED_STATIONS
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant] 
        public string TARGET_STATIONS
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant] 
        public string CLEARED_STATIONS
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string OUT_OF_ROUTE_STATIONS
        {
            get { return string.Empty; }
        }

        [LuaPluginMethod]
        public object register_display(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_display_group(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_display_group(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object load_templates(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object create_page_from_template(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object play(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object destroy_page(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object init_displays(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object stop(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object execute_test(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object set_status_specified_stations(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object set_status_all_stations(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object set_station_code_list(params object[] args)
        {
            return null;
        }
    }
}