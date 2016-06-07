// <copyright file="TemplateListAccessorLuaStubs.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2011.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
// </copyright>

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
        public void init(object arg)
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

    [LuaPluginClass("base_pack")]
    public class LuaBaseConstantsPlugin
    {
        [LuaPluginConstant]
        public string WEB_ROOT
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string SOUND_DIR
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string VIDEO_DIR
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string WEB_DIR
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string LED_DISPLAY_TEMPLATE_DIR
        {
            get { return string.Empty; }
        }
    }

    [LuaPluginClass("mission_pack")]
    public class LuaMissionConstantsPlugin
    {
        [LuaPluginConstant]
        public string WEB_ROOT
        {
            get { return string.Empty; }
        }
        
        [LuaPluginConstant]
        public string SOUND_DIR
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string VIDEO_DIR
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string WEB_DIR
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string LED_DISPLAY_TEMPLATE_DIR
        {
            get { return string.Empty; }
        }
    }

    [LuaPluginClass("infotainment_pack")]
    public class LuaInfotainmentConstantsPlugin
    {        
        [LuaPluginConstant]
        public string VIDEO_DIR
        {
            get { return string.Empty; }
        }     
    }

    [LuaPluginClass("language")]
    public class LuaLanguageConstantsPlugin
    {
        [LuaPluginConstant]
        public string FRA
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string ENG
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string GER
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string SPA
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string ITA
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string DUT
        {
            get { return string.Empty; }
        }
    }

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
            return null;
        }

        [LuaPluginMethod]
        public object getvalues(params object[] args)
        {
            return null;
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

    [LuaPluginClass("timer")]
    public class LuaTimerPlugin
    {
        [LuaPluginMethod]
        public object create(params object[] args)
        {
            return null;
        }        
        [LuaPluginMethod]
        public object destroy(params object[] args)
        {
            return null;
        }
        [LuaPluginMethod]
        public object start(params object[] args)
        {
            return null;
        }
        [LuaPluginMethod]
        public object stop(params object[] args)
        {
            return null;
        }
        [LuaPluginMethod]
        public object get_interval(params object[] args)
        {
            return null;
        }
        [LuaPluginMethod]
        public object set_interval(params object[] args)
        {
            return null;
        }
        [LuaPluginMethod]
        public object is_active(params object[] args)
        {
            return null;
        }
        [LuaPluginMethod]
        public object set_timerevent(params object[] args)
        {
            return null;
        }
    }

    [LuaPluginClass("util")]
    public class LuaUtilPlugin
    {
        [LuaPluginConstant]
        public string LE
        {
            get { return "LE"; }
        }

        [LuaPluginConstant]
        public string LW
        {
            get { return "LW"; }
        }

        [LuaPluginConstant]
        public string L1
        {
            get { return "L1"; }
        }

        [LuaPluginConstant]
        public string L2
        {
            get { return "L2"; }
        }

        [LuaPluginConstant]
        public string L3
        {
            get { return "L3"; }
        }

        [LuaPluginConstant]
        public string L4
        {
            get { return "L4"; }
        }

        [LuaPluginConstant]
        public string L5
        {
            get { return "L5"; }
        }

        [LuaPluginMethod]
        public void trace(params object[] args)
        {
        }

        [LuaPluginMethod]
        public object set_item_delimiter(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_item_count(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_first_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_last_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_last_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_first_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object insert_last_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object insert_first_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object insert_item(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object datetime(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_gmtime(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_localtime(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object url_encode(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object transform_prepositions(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_train_serial_num(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_car_num(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_localhost_ip_addr(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_operator_id(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_operator_model(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_train_serial_num_alpha(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_train_name(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_domain_name(params object[] args)
        {
            return null;
        }
    }

}