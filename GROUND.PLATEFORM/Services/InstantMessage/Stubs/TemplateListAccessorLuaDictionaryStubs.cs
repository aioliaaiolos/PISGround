// <copyright file="TemplateListAccessorLuaDictionaryStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("dictionary")]
    public class LuaDictionaryPlugin
    {
        [LuaPluginConstant]
        public int TRAIN_ANY
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int TRAIN_REMOTE
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int TRAIN_LOCAL
        {
            get { return 0; }
        }

        [LuaPluginMethod]
        public object create()
        {
            return null;
        }

        [LuaPluginMethod]
        public object destroy()
        {
            return null;
        }

        [LuaPluginMethod]
        public bool connect(params object[] args)
        {
            return true;
        }

        [LuaPluginMethod]
        public object disconnect(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public bool isconnected(params object[] args)
        {
            return true;
        }

        [LuaPluginMethod]
        public object subscribe(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object unsubscribe(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object getvalue(params object[] args)
        {
            return string.Empty;
        }

        [LuaPluginMethod]
        public object getvalues(params object[] args)
        {
            return string.Empty;
        }

        [LuaPluginMethod]
        public object setvalue(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object link(params object[] args)
        {
            return null;
        }
        [LuaPluginMethod]
        public object unlink(params object[] args)
        {
            return null;
        }
    }
}
