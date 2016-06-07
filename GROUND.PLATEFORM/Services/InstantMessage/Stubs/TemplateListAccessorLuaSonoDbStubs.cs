// <copyright file="TemplateListAccessorLuaSonoDbStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("sonodb")]
    public class LuaSonoDbPlugin
    {
        [LuaPluginConstant]
        public string GREETING_BEGIN
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string GREETING_END
        {
            get { return string.Empty; }
        }
        
        [LuaPluginMethod]
        public object get_text_greeting(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_text_delay_reason(params object[] args)
        {
            return null;
        }
    }
}
