// <copyright file="TemplateListAccessorLuaDecisionMatrixStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("dmatrix")]
    public class LuaDecisionMatrixPlugin
    {
        [LuaPluginConstant]
        public int IGNORE
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int WAIT
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int OVERRIDE
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int TEMP_OVERRIDE
        {
            get { return 0; }
        }

        [LuaPluginMethod]
        public object create(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object destroy(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object insert_rule(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_rule(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_size(params object[] args)
        {
            return null;
        }              
    }
}
