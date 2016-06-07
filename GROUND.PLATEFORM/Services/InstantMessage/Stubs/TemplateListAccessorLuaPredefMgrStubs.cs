// <copyright file="TemplateListAccessorLuaPredefMgrStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("predef")]
    public class LuaPredefMgrPlugin
    {
        private bool templatesValid = false;

        private List<Template> templates = new List<Template>();

        public List<Template> Templates
        {
            get
            {
                if (this.templatesValid)
                {
                    return this.templates;
                }
                else
                {
                    return new List<Template>();
                }
            }
        }

        [LuaPluginConstant]
        public TemplateParameterType DELAY_PARAM
        {
            get { return TemplateParameterType.Delay; }
        }

        [LuaPluginConstant]
        public TemplateParameterType CARID_PARAM
        {
            get { return TemplateParameterType.CarNumber; }
        }

        [LuaPluginConstant]
        public TemplateParameterType STATION_PARAM
        {
            get { return TemplateParameterType.StationId; }
        }

        [LuaPluginConstant]
        public TemplateParameterType DELAYREASON_PARAM
        {
            get { return TemplateParameterType.DelayReasonCode; }
        }

        [LuaPluginConstant]
        public TemplateParameterType TEXT_PARAM
        {
            get { return TemplateParameterType.Text; }
        }

        [LuaPluginConstant]
        public TemplateParameterType HOUR_PARAM
        {
            get { return TemplateParameterType.Hour; }
        }

        [LuaPluginConstant]
        public string ONE_LANGUAGE_BY_CALL
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string LANGUAGE_LIST_BY_CALL
        {
            get { return string.Empty; }
        }

        [LuaPluginMethod]
        public void init(params object[] args)
        {
            this.templatesValid = false;
        }

        [LuaPluginMethod]
        public Template create_instant_msg(string id, params object[] args)
        {
            Template template = new Template();
            template.ID = id;
            template.Category = TemplateCategory.Predefined;
            this.templates.Add(template);
            return template;
        }

        [LuaPluginMethod]
        public Template create_free_msg(string id, params object[] args)
        {
            Template template = new Template();
            template.ID = id;
            template.Category = TemplateCategory.FreeText;
            this.templates.Add(template);
            return template;
        }

        [LuaPluginMethod]
        public Template create_auto_delay_msg(string id, params object[] args)
        {
            Template template = new Template();
            template.ID = id;
            template.Category = TemplateCategory.Predefined; // TODO: Check !!!
            this.templates.Add(template);
            return template;
        }

        [LuaPluginMethod]
        public Template create_scheduled_msg(string id, params object[] args)
        {
            Template template = new Template();
            template.ID = id;
            template.Category = TemplateCategory.Scheduled;
            this.templates.Add(template);
            return template;
        }

        [LuaPluginMethod]
        public void set_class(Template template, string classdesc)
        {
            if (template != null)
            {
                template.Class = classdesc;
            }
        }

        [LuaPluginMethod]
        public void set_description(Template template, string lang, string desc)
        {
            if (template != null)
            {
                TemplateDescription description = new TemplateDescription();
                description.Language = lang;
                description.Value = desc;
                if (template.DescriptionList == null)
                {
                    template.DescriptionList = new List<TemplateDescription>();
                }

                template.DescriptionList.Add(description);
            }
        }

        [LuaPluginMethod]
        public void add_input_param(Template template, TemplateParameterType param)
        {
            if (template != null)
            {
                if (template.ParameterList == null)
                {
                    template.ParameterList = new List<TemplateParameterType>();
                }

                template.ParameterList.Add(param);
            }
        }

        [LuaPluginMethod]
        public object run()
        {
            this.templatesValid = true;
            return null;
        }

        [LuaPluginMethod]
        public object register_cancel_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_varargs(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_varargs(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_language_list_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_free_text_cmd(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_cancel_cmd(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_instant_msg_cmd(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_auto_delay_msg_cmd(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_update_notification_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object set_auto_delay_mission_state(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object destroy(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object stop(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object has_station_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object has_carid_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object has_delay_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object has_delayReason_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object has_hour_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object has_text_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object has_free_text_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_station_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_delay_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_carid_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_delayReason_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_hour_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_text_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_free_text_param(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_msg_id(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_broadcast_scope(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object adjust_broadcast_scope(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object execute_active_scheduled_msgs(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object set_version(params object[] args)
        {
            return null;
        }
    }
}
