// <copyright file="TemplateListAccessorLuaEventMgrStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("evtmgr")]
    public class LuaEventMgrPlugin
    {
        [LuaPluginConstant]
        public string STATION_DEPARTURE
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string STATION_APPROACH
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string STATION_ARRIVAL
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string STATION_STOP
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string STATION_APPROACH_LAST
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string STATION_ARRIVAL_LAST
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string STATION_STOP_LAST
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string MISSION_INIT
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string MISSION_END
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string MANUAL_MISSION_INIT
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string MANUAL_MISSION_END
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string STOP_IN_TRANSIT
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string STOP_IN_TRANSIT_END
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string DEGRADED_TRACKING
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string MISSION_NOT_INIT
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string EMERGENCY_HANDLE_ACTIVATED
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string EMERGENCY_HANDLE_DEACTIVATED
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string FIRE_ON_BOARD
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string FIRE_ON_BOARD_END
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string HANDICAP_REQUEST
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string HANDICAP_REQUEST_END
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string NO_PASSENGER
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string NO_PASSENGER_END
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string IDLE
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string WAYPOINT_IN
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public string WAYPOINT_OUT
        {
            get { return string.Empty; }
        }

        [LuaPluginConstant]
        public int ONE_LANGUAGE_BY_CALL
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int LANGUAGE_LIST_BY_CALL
        {
            get { return 0; }
        }

        [LuaPluginMethod]
        public object init(params object[] args)
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
        public object add_tracking_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_tracking_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_tracking_time_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_tracking_time_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_tracking_time_before_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_tracking_time_before_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_tracking_distance_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_tracking_distance_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_language_list_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_tracking_state_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_mission_state_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_sit_state_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_hr_state_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_mission_station_id_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_segment_distance_traveled_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_mission_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_mission_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_mission_time_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_mission_time_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object run()
        {
            return null;
        }

        [LuaPluginMethod]
        public object stop(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_base_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_base_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_derived_time_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_derived_time_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_derived_distance_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_derived_distance_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_train_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_train_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_fob_state_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_emh_state_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_np_state_var(params object[] args)
        {
            return null;
        }
        
        [LuaPluginMethod]
        public object assign_emh_location_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_hr_location_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_emh_location(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_fob_location(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_hr_location(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_idle_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_idle_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_waypoint_in_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_waypoint_out_var(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object add_waypoint_event(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object remove_waypoint_event(params object[] args)
        {
            return null;
        }
    }

    [LuaPluginClass("tag")]
    public class LuaTagConstantsPlugin
    {
        [LuaPluginConstant]
        public int MISSION_MESSAGE
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int TRAIN_MESSAGE
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int INSTANT_MESSAGE
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int DELAY_MESSAGE
        {
            get { return 0; }
        }
    }
}
