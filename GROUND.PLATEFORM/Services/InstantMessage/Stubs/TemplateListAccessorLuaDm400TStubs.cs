// <copyright file="TemplateListAccessorLuaDm400TStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("player400t")]
    public class LuaDm400TPlugin
    {
        [LuaPluginConstant]
        public bool BACKLIGHT_AUTO
        {
            get { return false; }
        }

        [LuaPluginConstant]
        public bool BACKLIGHT_MANUAL
        {
            get { return false; }
        }

        [LuaPluginConstant]
        public string GROUP_ALL_DISPLAYS
        {
            get { return string.Empty; }
        }

        [LuaPluginMethod]
        public object set_dmatrix(params object[] args)
        {
            return null;
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
        public object play(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object stop(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object clear_cache(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object set_backlight_luminosity(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object turn_backlight_on(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object turn_backlight_off(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object set_backlight_mode(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object set_webserver(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object set_location_prefix(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object start_listen_stream(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object stop_listen_stream(params object[] args)
        {
            return null;
        }       
    }
}
