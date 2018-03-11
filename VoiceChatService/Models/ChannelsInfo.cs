using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using Microsoft.Ajax.Utilities;

namespace VoiceChatService.Models
{
    public static class ChannelsInfo
    {
        public static Dictionary<string, ChannelInfo> channels = new Dictionary<string, ChannelInfo>();                                      // Key: ChannelName    Value: Info
        public static Dictionary<string, VoiceServer.VoiceServer> ListOfSockets = new Dictionary<string, VoiceServer.VoiceServer>();         // Key: Port;          Value: Server

        static ChannelsInfo()
        {

        }

        public static bool AddChannel(ChannelDataModel dataModel, string socketPort)
        {
            if (!channels.ContainsKey(dataModel.Name))
            {
                ChannelInfo channelInfo = new ChannelInfo();

                channelInfo.Id = dataModel.Id;
                channelInfo.Name = dataModel.Name;
                channelInfo.Port = socketPort;

                channels.Add(channelInfo.Name, channelInfo);

                return true;
            }

            return false;
        }

        public static void RemoveChannel(string _name)
        {
            if (channels.ContainsKey(_name))
            {
                ListOfSockets[channels[_name].Port].Dispose();
                ListOfSockets.Remove(channels[_name].Port);
                channels.Remove(_name);
            }
        }

        public static void AddMember(string _groupName, string _member)
        {
            channels[_groupName].Members.Add(_member);
        }

        public static bool RemoveMember(string _groupName, string _member)
        {
            if (channels[_groupName].Members.Remove(_member))
            {
                if (channels[_groupName].Members.Count == 0)
                {
                    RemoveChannel(_groupName);
                }

                return true;
            }

            return false;
        }

        public static ChannelInfo GetChannelInfo(string _name)
        {
            if (channels.ContainsKey(_name))
            {
                return channels[_name];
            }

            return null;
        }
    }

    public class ChannelInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Members = new List<string>();
        public string Port { get; set; }
    }
}