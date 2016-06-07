// <copyright file="TemplateListAccessorLuaAVStreamMgrStubs.cs" company="Alstom Transport Telecite Inc.">
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

    [LuaPluginClass("avstream")]
    public class LuaAVStreamMgrPlugin
    {
        [LuaPluginConstant]
        public int PROTOCOL_PlayLocally
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int PROTOCOL_HTTP
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int PROTOCOL_MMSH
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int PROTOCOL_RTP
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int PROTOCOL_UDP
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int NOTIFICATION_PLAYING
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int NOTIFICATION_STOPPED
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int NOTIFICATION_PAUSED
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int NOTIFICATION_ENDED
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int NOTIFICATION_PLAYLIST_ENDED
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int NOTIFICATION_PLAYLIST_CLEARED
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int NOTIFICATION_ERROR
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int STATUS_NOTCONFIGURED
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int STATUS_STOPPED
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int STATUS_ENDED
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int STATUS_PAUSED
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int STATUS_PLAYING
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int INTERRUPT_VIDEO
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int WAIT_VIDEO_COMPLETE
        {
            get { return 0; }
        }

        [LuaPluginMethod]
        public object play_stream(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object pause_stream(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object stop_stream(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object configure_channel(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object assign_playlist(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object clear_playlist(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_current_playlist_reference(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_url(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_protocol(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_current_video(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_stream_status(params object[] args)
        {
            return null;
        }
    }

    [LuaPluginClass("playlist")]
    public class LuaPlaylistPlugin
    {
        [LuaPluginConstant]
        public int RECURRING
        {
            get { return 0; }
        }

        [LuaPluginConstant]
        public int ONCE
        {
            get { return 0; }
        }

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
        public object add(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object insert(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object get_file_list(params object[] args)
        {
            return null;
        }

        [LuaPluginMethod]
        public object equals(params object[] args)
        {
            return null;
        }
    }
}
