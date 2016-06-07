// <copyright file="TemplateListAccessorLuaWavPlayerStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("playerwav")]
    public class LuaPlayerWavPlugin
    {
        [LuaPluginConstant]
        public string IGNORE_ON_CANCEL
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string RETRY_ON_CANCEL
        {
            get { return string.Empty; }
        }

        [LuaPluginMethod]
        public object set_gong_trig_time(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object set_gong(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_duration(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object set_dmatrix(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object concat(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object append(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object silence(params object[] args)
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
        public object stop_and_wait(params object[] args)
        {
            return null;
        }

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
        public object free(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object clone(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object register_on_play_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object unregister_on_play_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object register_on_interrupted_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object unregister_on_interrupted_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object register_on_ended_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object unregister_on_ended_event(params object[] args)
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



    }
}
