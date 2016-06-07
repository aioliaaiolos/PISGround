// <copyright file="TemplateListAccessorLuaMACStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("speech")]
    public class LuaSpeechPlugin
    {
        [LuaPluginConstant]
        public string TONE_NEUTRAL
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string TONE_DESCENDING
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string TONE_ASCENDING
        {
            get { return string.Empty; }
        }

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
        public object number(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object digits(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object time(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object delay(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object delay_reason(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object stations(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object station(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object carnum(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object greeting(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object sentence(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object region(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object stats(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object set_garbage_collection(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object mission_type(params object[] args)
        {
            return null;
        }
    }
}
