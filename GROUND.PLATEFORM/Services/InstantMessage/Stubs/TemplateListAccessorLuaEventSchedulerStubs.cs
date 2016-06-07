// <copyright file="TemplateListAccessorLuaEventSchedulerStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("event_scheduler")]
    public class LuaEventSchedulerPlugin
    {
        [LuaPluginConstant]
        public string MONDAY
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string TUESDAY
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string WEDNESDAY
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string THURSDAY
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string FRIDAY
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string SATURDAY
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string SUNDAY
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string SECOND
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string MINUTE
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string HOUR
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string DAY
        {
            get { return string.Empty; }
        }

        [LuaPluginMethod]
        public object add_onetime_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_periodic_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_periodic_daytime_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_periodic_weekday_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_periodic_monthday_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_onetime_event_on_range(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_periodic_event_on_range(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_event(params object[] args)
        {
            return null;
        }
    }
}
