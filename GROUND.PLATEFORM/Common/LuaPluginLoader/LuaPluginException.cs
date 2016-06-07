// <copyright file="LuaPluginException.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2011.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
// </copyright>

namespace PIS.Ground.Common.LuaPluginLoader
{
    using System;

    /// <summary>
    /// General LUA plugin exception
    /// </summary>
    public class LuaPluginException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the LuaPluginException class.
        /// Default constructor.
        /// </summary>
        public LuaPluginException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LuaPluginException class.
        /// Constructor with a specified error message.
        /// </summary>
        /// <param name="message">error message</param>
        public LuaPluginException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LuaPluginException class.
        /// Constructor with a specified error message and an inner exception.
        /// </summary>
        /// <param name="message">error message</param>
        /// <param name="inner">inner exception</param>
        public LuaPluginException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
