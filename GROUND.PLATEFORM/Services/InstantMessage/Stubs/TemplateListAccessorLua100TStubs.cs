// <copyright file="TemplateListAccessorLua100TStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("player100t")]
    public class Lua100TPlugin
    {
        [LuaPluginConstant]
        public string GROUP_ALL_DISPLAYS
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string GROUP_ALL_ALSTOM
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string GROUP_ALL_ANNAX
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string GROUP_ALL_ANNAX_RGB
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string GROUP_ALL_MITRON
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public int INFINITE_DURATION
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int TEST_VERTICAL_BAR
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int FORM_NONE
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int FORM_ARABIC
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int FORM_TAMIL
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int BMP_ANNAX_RGB
        {
            get { return 0; }
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
        public object set_variable_value(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object set_variable_value_utf8(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object clear_zone(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object print_text(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object print_text_utf8(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object print_cr(params object[] args)
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
        public object set_dmatrix(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object stop_test(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object print_font_select(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object print_bmp_from_file(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object toggle_blinking(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public bool set_page_autodelete(params object[] args)
        {
            return true;
        }
    }
}