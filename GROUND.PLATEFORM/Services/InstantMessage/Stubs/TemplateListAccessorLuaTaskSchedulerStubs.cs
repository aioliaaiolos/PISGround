// <copyright file="TemplateListAccessorLuaTaskSchedulerStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("task_scheduler")]
    public class LuaTaskSchedulerPlugin
    {
        [LuaPluginMethod]
        public object set_dmatrix(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_task(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object clear_tasks(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object stop(params object[] args)
        {
            return null;
        }
    }
}
