// <copyright file="TemplateListAccessorLuaUtilStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("util")]
    public class LuaUtilPlugin
    {
        [LuaPluginConstant]
        public string LE
        {
            get { return "LE"; }
        }

        [LuaPluginConstant]
        public string LW
        {
            get { return "LW"; }
        }

        [LuaPluginConstant]
        public string L1
        {
            get { return "L1"; }
        }

        [LuaPluginConstant]
        public string L2
        {
            get { return "L2"; }
        }

        [LuaPluginConstant]
        public string L3
        {
            get { return "L3"; }
        }

        [LuaPluginConstant]
        public string L4
        {
            get { return "L4"; }
        }

        [LuaPluginConstant]
        public string L5
        {
            get { return "L5"; }
        }

        [LuaPluginMethod]
        public void trace(params object[] args)
        {
        }

        [LuaPluginMethod]
        public object set_item_delimiter(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_item_count(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_first_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_last_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_last_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_first_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object insert_last_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object insert_first_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object insert_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object datetime(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_gmtime(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_localtime(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object url_encode(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object transform_prepositions(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_train_serial_num(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_car_num(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_localhost_ip_addr(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_operator_id(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_operator_model(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_train_serial_num_alpha(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_train_name(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_domain_name(params object[] args)
        {
            return null;
        }
    }

    [LuaPluginClass("dataplug")]
    public class LuaDataplugPlugin
    {
        [LuaPluginMethod]
        public void get_train_serial_num(params object[] args)
        {
        }

        [LuaPluginMethod]
        public void get_car_num(params object[] args)
        {
        }

        [LuaPluginMethod]
        public void get_localhost_ip_addr(params object[] args)
        {
        }

        [LuaPluginMethod]
        public void get_operator_id(params object[] args)
        {
        }

        [LuaPluginMethod]
        public void get_operator_model(params object[] args)
        {
        }

        [LuaPluginMethod]
        public void get_train_serial_num_alpha(params object[] args)
        {
        }

        [LuaPluginMethod]
        public void get_train_name(params object[] args)
        {
        }

        [LuaPluginMethod]
        public void get_domain_name(params object[] args)
        {
        }
    }
}
