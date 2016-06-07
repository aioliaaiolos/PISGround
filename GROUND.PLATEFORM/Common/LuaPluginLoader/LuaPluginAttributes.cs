// <copyright file="LuaPluginAttributes.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2011.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
// </copyright>

namespace PIS.Ground.Common.LuaPluginLoader.Attributes
{
    using System;

    /// <summary>
    /// Attribute to tag a class as a LUA plugin
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class LuaPluginClassAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the LuaPluginClassAttribute class. 
        /// LUA plugin name will be the same as that of the tagged class.
        /// </summary>
        public LuaPluginClassAttribute()
        {
            this.Name = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the LuaPluginClassAttribute class.
        /// LUA plugin name will be one specified.
        /// </summary>
        /// <param name="name">LUA plugin name</param>
        public LuaPluginClassAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the LUA plugin name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Attribute to tag a class method as a LUA plugin method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class LuaPluginMethodAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the LuaPluginMethodAttribute class. 
        /// LUA plugin method name will be the same as that of the tagged method.
        /// </summary>
        public LuaPluginMethodAttribute()
        {
            this.Name = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the LuaPluginMethodAttribute class.
        /// LUA plugin method name will be one specified.
        /// </summary>
        /// <param name="name">LUA plugin method name</param>
        public LuaPluginMethodAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the LUA plugin method name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Attribute to tag a class property as a LUA plugin constant
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LuaPluginConstantAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the LuaPluginConstantAttribute class. 
        /// LUA plugin constant name will be the same as that of the tagged property.
        /// </summary>
        public LuaPluginConstantAttribute()
        {
            this.Name = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the LuaPluginConstantAttribute class.
        /// LUA plugin constant name will be one specified.
        /// </summary>
        /// <param name="name">LUA plugin method name</param>
        public LuaPluginConstantAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the LUA plugin constant name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Attribute to tag a class method as a LUA plugin error handler method. This
    /// method will be called when there is as error invoking or executing any of
    /// the plugin methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class LuaPluginErrorHandlerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the LuaPluginErrorHandlerAttribute class.        
        /// </summary>
        public LuaPluginErrorHandlerAttribute()
        {
        }        
    }
}
