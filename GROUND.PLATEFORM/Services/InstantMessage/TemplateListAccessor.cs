// <copyright file="TemplateListAccessor.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2016.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
// </copyright>

namespace PIS.Ground.InstantMessage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using LuaInterface;
    using PIS.Ground.Common.LuaPluginLoader;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.LogMgmt;
    using PIS.Ground.InstantMessage.TemplateListAccessorLuaStubs;    

    /// <summary>
    /// Helper class to access the message template list defined in a LUA script.
    /// </summary>
    public sealed class TemplateListAccessor : IDisposable
    {
        /// <summary>
        /// The list of message template structures initialized from the LUA script.
        /// </summary>
        private List<Template> templates = new List<Template>();

        private string lastError;

        /// <summary>
        /// Execute a lua script, given the
        /// path to the LUA script containing the message template definitions. After
        /// using this function, use <see cref="TemplateExists"/>,
        /// <see cref="GetTemplate"/>, and <see cref="GetAllTemplates"/> to
        /// examine initialized template structures.
        /// </summary>
        /// <param name="templateDefinitionScriptPath">LUA script file path</param>
        ///       
        public bool ExecuteTemplate(string templateDefinitionScriptPath, List<string> scriptsList)
        {
            bool result = false;
            try
            {
                lastError = "no error.";
                LuaPluginLoader loader = new LuaPluginLoader();

                LuaPredefMgrPlugin predef = new LuaPredefMgrPlugin();
                loader.RegisterPlugin(predef);

                LuaEventMgrPlugin evtmgr = new LuaEventMgrPlugin();
                loader.RegisterPlugin(evtmgr);

                LuaTagConstantsPlugin tag = new LuaTagConstantsPlugin();
                loader.RegisterPlugin(tag);

                LuaDictionaryPlugin dic = new LuaDictionaryPlugin();
                loader.RegisterPlugin(dic);

                LuaUtilPlugin util = new LuaUtilPlugin();
                loader.RegisterPlugin(util);

                LuaReportPlugin report = new LuaReportPlugin();
                loader.RegisterPlugin(report);

                LuaDataplugPlugin dataplug = new LuaDataplugPlugin();
                loader.RegisterPlugin(dataplug);

                LuaTimerPlugin timer = new LuaTimerPlugin();
                loader.RegisterPlugin(timer);

                LuaAVStreamMgrPlugin avstreammgr = new LuaAVStreamMgrPlugin();
                loader.RegisterPlugin(avstreammgr);

                LuaDecisionMatrixPlugin matrix = new LuaDecisionMatrixPlugin();
                loader.RegisterPlugin(matrix);

                LuaDm400TPlugin player400t = new LuaDm400TPlugin();
                loader.RegisterPlugin(player400t);

                Lua100TPlugin player100t = new Lua100TPlugin();
                loader.RegisterPlugin(player100t);

                Lua150TPlugin player150t = new Lua150TPlugin();
                loader.RegisterPlugin(player150t);

                Lua180TPlugin player180t = new Lua180TPlugin();
                loader.RegisterPlugin(player180t);

                LuaPlaylistPlugin playlist = new LuaPlaylistPlugin();
                loader.RegisterPlugin(playlist);

                LuaPlayerPldPlugin playerPld = new LuaPlayerPldPlugin();
                loader.RegisterPlugin(playerPld);

                LuaPlayerVVPlugin playerVV = new LuaPlayerVVPlugin();
                loader.RegisterPlugin(playerVV);

                LuaEventSchedulerPlugin eventScheduler = new LuaEventSchedulerPlugin();
                loader.RegisterPlugin(eventScheduler);

                LuaLmtDbPlugin lmtDb = new LuaLmtDbPlugin();
                loader.RegisterPlugin(lmtDb);

                LuaSpeechPlugin speech = new LuaSpeechPlugin();
                loader.RegisterPlugin(speech);

                LuaPlayerMp3Plugin playerMp3 = new LuaPlayerMp3Plugin();
                loader.RegisterPlugin(playerMp3);

                LuaSonoDbPlugin sonoDb = new LuaSonoDbPlugin();
                loader.RegisterPlugin(sonoDb);

                LuaTaskSchedulerPlugin taskScheduler = new LuaTaskSchedulerPlugin();
                loader.RegisterPlugin(taskScheduler);

                LuaPlayerWavPlugin playerWav = new LuaPlayerWavPlugin();
                loader.RegisterPlugin(playerWav);

                LuaMissionDataClientPlugin mission_data = new LuaMissionDataClientPlugin();
                loader.RegisterPlugin(mission_data);

                LuaBaseConstantsPlugin constants1 = new LuaBaseConstantsPlugin();
                loader.RegisterPlugin(constants1);

                LuaMissionConstantsPlugin constants2 = new LuaMissionConstantsPlugin();
                loader.RegisterPlugin(constants2);

                LuaInfotainmentConstantsPlugin constants3 = new LuaInfotainmentConstantsPlugin();
                loader.RegisterPlugin(constants3);

                LuaLanguageConstantsPlugin constants4 = new LuaLanguageConstantsPlugin();
                loader.RegisterPlugin(constants4);

                LuaPisConstantsPlugin constants5 = new LuaPisConstantsPlugin();
                loader.RegisterPlugin(constants5);

                LuaConfigConstantsPlugin constants6 = new LuaConfigConstantsPlugin();
                loader.RegisterPlugin(constants6);

                LuaTrainConstantsPlugin constants7 = new LuaTrainConstantsPlugin();
                loader.RegisterPlugin(constants7);

                using (Lua lua = new Lua())
                {
                    loader.LoadPlugins(lua);

                    lua.DoString("function dofile(...) end;");

                    foreach (string script in scriptsList)
                    {
                        lua.DoFile(script);
                    }

                    lua.DoFile(templateDefinitionScriptPath);

                    lua.GetFunction("_Initialization").Call(new object[0]);

                    this.templates = predef.Templates;
                }

                result = true;
            }
            catch (Exception ex)
            {
               LogManager.WriteLog(
                    TraceType.ERROR,
                    "ExecuteTemplate error : Predefined message template file not valid  [" + templateDefinitionScriptPath + "]",
                    "PIS.Ground.InstantMessage.TemplateListAccessorClass",
                    ex,
                    EventIdEnum.InstantMessage);
               lastError = ex.Message;
            }
            return result;
        }


        /// <summary>
        /// Return the error string of the last execution of <see cref="ExecuteTemplate"/>
        /// </summary>        
        public string GetLastExecutionError()
        {
            return lastError;
        }

        /// <summary>
        /// Helper static function that serializes a provided list of <see cref="Template"/>
        /// objects to an XML file.
        /// </summary>        
        /// <param name="obj">the list of <see cref="Template"/> objects to serialize</param>
        /// <param name="fileName">the target XML file name</param>
        public static void WriteTemplatesToFile(List<Template> obj, string fileName)
        {
            using (FileStream writer = new FileStream(fileName, FileMode.Create))
            {
                XmlSerializer serializer =
                    new XmlSerializer(typeof(List<Template>), new XmlRootAttribute("TemplateList"));

                serializer.Serialize(writer, obj);
            }
        }

        /// <summary>
        /// Helper static function that initializes a provided list of <see cref="Template"/>
        /// objects from an XML file.
        /// </summary>        
        /// <param name="fileName">the source XML file name</param>
        /// <returns>the deserialized list of <see cref="Template"/> objects</returns>
        public static List<Template> ReadTemplatesFromFile(string fileName)
        {
            XmlSerializer serializer =
                new XmlSerializer(typeof(List<Template>), new XmlRootAttribute("TemplateList"));

            using (StreamReader reader = new StreamReader(fileName))
            {
                return (List<Template>)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Helper static function that serializes a provided list of <see cref="Template"/>
        /// objects to an XML-formatted (UTF-8 encoding) string.
        /// </summary>        
        /// <param name="obj">the list of <see cref="Template"/> objects to serialize</param>
        /// <returns>the XML string</returns>
        public static string SerializeTemplates(List<Template> obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Template>), new XmlRootAttribute("TemplateList"));
            using (Utf8StringWriter writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        // we want to be able to use the using() keyword with this class.
        void IDisposable.Dispose()
        {
        }
 
        /// <summary>
        /// Returns true if the templates initialized in the constructor contain a
        /// template with ID provided.        
        /// </summary>
        /// <param name="templateId">template ID</param>
        /// <returns>true if the template exists in the list, false otherwise</returns>
        public bool TemplateExists(string templateId)
        {
            return this.templates.Exists(t => (t.ID == templateId));            
        }

        /// <summary>
        /// Returns the template structure that has a given template ID.
        /// </summary>
        /// <param name="templateId">template ID</param>
        /// <returns><see cref="Template"/> instance if the template exists, or null otherwise</returns>
        public Template GetTemplate(string templateId)
        {
            Template result = null;

            if (this.templates.Exists(t => (t.ID == templateId)))
            {
                result = this.templates.Find(t => (t.ID == templateId));
            }
                
            return result;
        }

        /// <summary>
        /// Returns all template structures initialized in the constructor.
        /// </summary>        
        /// <returns>List of <see cref="Template"/> objects</returns>
        public List<Template> GetAllTemplates()
        {
            return this.templates;
        }

        /// <summary>
        /// Helper private class to use UTF-8 encoding with StringWriter.
        /// </summary>                
        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
    }    
}