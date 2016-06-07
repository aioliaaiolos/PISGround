// <copyright file="LuaPluginLoader.cs" company="Alstom Transport Telecite Inc.">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using LuaInterface;
    using PIS.Ground.Common.LuaPluginLoader.Attributes;
    using System.Collections;

    /// <summary>
    /// LUA plugin loader class. It allows adding plugins with functions and constants
    /// to the LUA interpreter.
    /// </summary>
    public class LuaPluginLoader
    {
        const byte sMaxParamsCount = 20;
        
        #region Private fields

        /// <summary>
        /// Plugin table with entries mapping "plugin_name" strings to plugin instances.
        /// </summary>
        private Dictionary<string, object> pluginTable = new Dictionary<string, object>();

        /// <summary>
        /// Dispatch table with entries mapping "plugin_name.plugin_method" strings
        /// to corresponding methods that are to be called by LUA interpreter. It is
        /// a list of methods for each "plugin_name.plugin_method" to accomodate overloads.
        /// </summary>
        private Dictionary<string, List<MethodInfo>> dispatchTable = new Dictionary<string, List<MethodInfo>>();

        /// <summary>
        /// Property table with entries mapping "plugin_name.plugin_constant" strings
        /// to corresponding properties that are to be read and loaded as constants into
        /// LUA interpreter.
        /// </summary>
        private Dictionary<string, PropertyInfo> propertyTable = new Dictionary<string, PropertyInfo>();

        #endregion

        /// <summary>
        /// Analyses the type of the plugin instance, looking for LUA plugin attributes, and
        /// extracts the plugin name, methods, and constants definitions to be loaded into
        /// the LUA interpreter afterwards with LoadPlugins(). Must be called before LoadPlugins().
        /// </summary>
        /// <param name="pluginObj">plugin class instance</param>
        public void RegisterPlugin(object pluginObj)
        {
            if (pluginObj != null)
            {
                Type t = pluginObj.GetType();

                if (IsPluginClass(t))
                {
                    this.pluginTable[GetPluginClassName(t)] = pluginObj;

                    foreach (MethodInfo method in t.GetMethods())
                    {
                        if (IsPluginMethod(method))
                        {
                            string key = GetPluginClassName(t) + "." + GetPluginMethodName(method);
                            if (!this.dispatchTable.ContainsKey(key))
                            {
                                this.dispatchTable[key] = new List<MethodInfo>();
                            }

                            this.dispatchTable[key].Add(method);
                        }
                    }

                    foreach (PropertyInfo property in t.GetProperties())
                    {
                        if (IsPluginConstant(property))
                        {
                            this.propertyTable[GetPluginClassName(t) + "." + GetPluginConstantName(property)] = property;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the plugin definitions into the interpreter execution context. Must be called after
        /// all plugins have been registered with RegisterPlugin().
        /// </summary>
        /// <param name="interpreter">LUA interpreter instance</param>
        public void LoadPlugins(Lua interpreter)
        {
            if (interpreter == null)
            {
                throw new ArgumentNullException("interpreter");
            }

            interpreter["__plugin_mgr__"] = this;

            foreach (string pluginName in this.pluginTable.Keys)
            {
                interpreter.NewTable(pluginName);
            }

            foreach (KeyValuePair<string, PropertyInfo> entry in this.propertyTable)
            {
                string[] pluginConstant = entry.Key.Split('.');
                if (pluginConstant.Count() == 2)
                {
                    string pluginName = pluginConstant[0];
                    string constantName = pluginConstant[1];

                    object pluginInstance = null;
                    if (this.pluginTable.TryGetValue(pluginName, out pluginInstance) && pluginInstance != null)
                    {
                        interpreter.GetTable(pluginName)[constantName] = entry.Value.GetValue(pluginInstance, null);
                    }
                }
            }

            foreach (string key in this.dispatchTable.Keys.Distinct())
            {
                string[] pluginMethod = key.Split('.');
                if (pluginMethod.Count() == 2)
                {
                    string pluginName = pluginMethod[0];
                    string methodName = pluginMethod[1];

                    string luaCommand = "function " + pluginName + "." + methodName + "(...) return __plugin_mgr__:Dispatch('" + pluginName + "', '" + methodName + "', arg); end; ";
                    
                    interpreter.DoString(luaCommand);
                }
            }
        }

        /// <summary>
        /// Dispatch the call from the LUA interpreter to the corresponding registered plugin
        /// instance method. This method is not intended to be called directly. It is exposed
        /// to be accessible by the LUA interpreter.
        /// </summary>
        /// <param name="pluginName">registered plugin name</param>
        /// <param name="methodName">registered plugin method name</param>
        /// <param name="arg">a LuaTable that contains plugin method arguments</param>
        /// <returns>plugin method call result</returns>
        public object Dispatch(string pluginName, string methodName, object arg)
        {
            if (arg is LuaTable)
            {
                LuaTable lTable = arg as LuaTable;

                int lParamsCount = Math.Min(Convert.ToInt32(lTable["n"]), sMaxParamsCount);

                object[] lParams = new object[lParamsCount];                               

                for (int i = 0; i < lParamsCount; i++)
                {
                    lParams[i] = lTable[i + 1];
                }

                return this.DispatchInternal(pluginName, methodName, lParams);
            }
            else
            {
                return null;
            }
        }

        #region Private methods

        /// <summary>
        /// Determines whether the class type has been tagged with
        /// <see cref="LuaPluginClassAttribute"/>
        /// </summary>
        /// <param name="type">class type</param>
        /// <returns>true if type is tagged, false otherwise</returns>
        private static bool IsPluginClass(Type type)
        {
            foreach (object attribute in type.GetCustomAttributes(true))
            {
                if (attribute is LuaPluginClassAttribute)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the tagged plugin name as set by <see cref="LuaPluginClassAttribute"/>
        /// </summary>
        /// <param name="type">class type</param>
        /// <returns>plugin name, or an empty string if the type does not have <see cref="LuaPluginClassAttribute"/> set</returns>
        private static string GetPluginClassName(Type type)
        {
            string name = string.Empty;
            if (IsPluginClass(type))
            {
                foreach (object attribute in type.GetCustomAttributes(true))
                {
                    if (attribute is LuaPluginClassAttribute)
                    {
                        LuaPluginClassAttribute attr = (LuaPluginClassAttribute)attribute;
                        name = attr.Name;
                        break;
                    }
                }

                if (name == string.Empty)
                {
                    name = type.Name;
                }
            }

            return name;
        }

        /// <summary>
        /// Determines whether the class method type has been tagged with
        /// <see cref="LuaPluginMethodAttribute"/>
        /// </summary>
        /// <param name="method">method type</param>
        /// <returns>true if method is tagged, false otherwise</returns>
        private static bool IsPluginMethod(MethodInfo method)
        {
            foreach (object attribute in method.GetCustomAttributes(true))
            {
                if (attribute is LuaPluginMethodAttribute)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the tagged plugin method name as set by <see cref="LuaPluginMethodAttribute"/>
        /// </summary>
        /// <param name="method">method type</param>
        /// <returns>plugin method name, or an empty string if the method does not have
        /// <see cref="LuaPluginMethodAttribute"/> set
        /// </returns>
        private static string GetPluginMethodName(MethodInfo method)
        {
            string name = string.Empty;
            if (IsPluginMethod(method))
            {
                foreach (object attribute in method.GetCustomAttributes(true))
                {
                    if (attribute is LuaPluginMethodAttribute)
                    {
                        LuaPluginMethodAttribute attr = (LuaPluginMethodAttribute)attribute;
                        name = attr.Name;
                        break;
                    }
                }

                if (name == string.Empty)
                {
                    name = method.Name;
                }
            }

            return name;
        }

        /// <summary>
        /// Determines whether the class method type has been tagged with
        /// <see cref="LuaPluginErrorHandlerAttribute"/>
        /// </summary>
        /// <param name="method">method type</param>
        /// <returns>true if method is tagged, false otherwise</returns>
        private static bool IsPluginErrorHandler(MethodInfo method)
        {
            foreach (object attribute in method.GetCustomAttributes(true))
            {
                if (attribute is LuaPluginErrorHandlerAttribute)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the tagged plugin error handler method as set by
        /// <see cref="LuaPluginErrorHandlerAttribute"/> on one of the
        /// methods of class type.
        /// </summary>
        /// <param name="type">class type</param>
        /// <returns>tagged method, or null, if the type does not have
        /// any method with <see cref="LuaPluginErrorHandlerAttribute"/>
        /// set
        /// </returns>
        private static MethodInfo GetPluginErrorHandler(Type type)
        {
            foreach (MethodInfo method in type.GetMethods())
            {
                foreach (object attribute in method.GetCustomAttributes(true))
                {
                    if (attribute is LuaPluginErrorHandlerAttribute)
                    {
                        return method;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the class property type has been tagged with the LUA plugin constant attribute.
        /// <see cref="LuaPluginConstantAttribute"/>
        /// </summary>
        /// <param name="property">property type</param>
        /// <returns>true if property is tagged as a LUA plugin constant, false otherwise</returns>
        private static bool IsPluginConstant(PropertyInfo property)
        {
            foreach (object prop in property.GetCustomAttributes(true))
            {
                if (prop is LuaPluginConstantAttribute)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the tagged plugin constant name as set by <see cref="LuaPluginConstantAttribute"/>
        /// </summary>
        /// <param name="property">property type</param>
        /// <returns>plugin constant name, or an empty string if the property does not have
        /// <see cref="LuaPluginConstantAttribute"/> set
        /// </returns>
        private static string GetPluginConstantName(PropertyInfo property)
        {
            string name = string.Empty;
            if (IsPluginConstant(property))
            {
                foreach (object prop in property.GetCustomAttributes(true))
                {
                    if (prop is LuaPluginConstantAttribute)
                    {
                        LuaPluginConstantAttribute attr = (LuaPluginConstantAttribute)prop;
                        name = attr.Name;
                        break;
                    }
                }

                if (name == string.Empty)
                {
                    name = property.Name;
                }
            }

            return name;
        }

        /// <summary>
        /// Dispatch the call from the LUA interpreter to the corresponding registered plugin
        /// instance method. This method is the variable argument private dispatcher method called
        /// by overloaded Dispatch() methods, invoked by the LUA interpreter.
        /// </summary>
        /// <param name="pluginName">registered plugin name</param>
        /// <param name="methodName">registered plugin method name</param>
        /// <param name="args">variable plugin method parameter array</param>
        /// <returns>plugin method call result</returns>
        private object DispatchInternal(string pluginName, string methodName, params object[] args)
        {
            object ret = null;
            object pluginInstance = null;
            if (this.pluginTable.TryGetValue(pluginName, out pluginInstance) && pluginInstance != null)
            {
                bool executed = false;
                LuaPluginException exception = null;

                try
                {
                    string key = pluginName + "." + methodName;

                    List<MethodInfo> overloads = null;
                    if (this.dispatchTable.TryGetValue(key, out overloads))
                    {
                        foreach (MethodInfo method in overloads)
                        {                        
                            bool doCall = true;

                            // Check if target method is variadic
                            ParameterInfo[] parameters = method.GetParameters();
                            
                            bool targetMethodIsVariadic = false;
                            if (parameters.Length > 0)
                            {
                                targetMethodIsVariadic = parameters[parameters.Length - 1].GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0;
                            }

                            // Validate target method and source method parameters count
                            if (targetMethodIsVariadic)
                            {
                                if (args.Length < (parameters.Length - 1))
                                {
                                    doCall = false;                                    
                                }                            
                            }
                            else
                            {
                                if (parameters.Length != args.Length)
                                {
                                    doCall = false;                                    
                                }  
                            }
                            
                            object[] invokedArgs = null;
                            if (doCall)
                            {                                
                                if (targetMethodIsVariadic)
                                {
                                    int lastParamPosition = parameters.Length - 1;

                                    invokedArgs = new object[parameters.Length];
                                    for (int i = 0; i < lastParamPosition; i++)
                                    {
                                        invokedArgs[i] = args[i];
                                    }

                                    Type paramsType = parameters[lastParamPosition].ParameterType.GetElementType() ?? typeof(object);

                                    Array extra = Array.CreateInstance(paramsType, args.Length - lastParamPosition);
                                    for (int i = 0; i < extra.Length; i++)
                                    {
                                        extra.SetValue(args[i + lastParamPosition], i);
                                    }

                                    invokedArgs[lastParamPosition] = extra;
                                    
                                    // TODO Check types for variadic target methods
                                }
                                else
                                {
                                    invokedArgs = args;
                                    
                                    // Check types
                                    for (int i = 0; i < invokedArgs.Length; ++i)
                                    {
                                        if (invokedArgs[i] != null && !parameters[i].ParameterType.IsAssignableFrom(invokedArgs[i].GetType()))
                                        {
                                            // Type mismatch
                                            doCall = false;
                                            break;
                                        }
                                    }
                                }
                            }                
                            
                            if (doCall)
                            {
                                ret = method.Invoke(pluginInstance, invokedArgs);
                                executed = true;
                                break;
                            }                        
                        }
                    }

                    if (!executed)
                    {
                        exception = new LuaPluginException("Could not find a method with matching signature while dispatching the call to " + pluginName + "." + methodName);                        
                    }
                }
                catch (Exception ex)
                {
                    exception = new LuaPluginException("Exception thrown during dispatching the call to " + pluginName + "." + methodName, ex);
                }

                if (exception != null)
                {
                    MethodInfo errorHandler = GetPluginErrorHandler(pluginInstance.GetType());
                    if (errorHandler != null)
                    {
                        errorHandler.Invoke(pluginInstance, new object[] { exception });
                    }
                    else
                    {
                        throw exception;
                    }
                }
            }

            return ret;
        }       

        #endregion
    }
}