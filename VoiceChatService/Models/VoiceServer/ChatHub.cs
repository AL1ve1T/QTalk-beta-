using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using System.Collections;

namespace VoiceChatService.Models.VoiceServer
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        public Task JoinChannel(string channelName, string member)
        {
            this.Groups.Add(this.Context.ConnectionId, channelName);
            Dictionary<string, byte[]> toSend = new Dictionary<string, byte[]>();

            using (UserDataRepository db = new UserDataRepository())
            {
                List<string> list = ChannelsInfo.channels[channelName].Members;

                foreach (var item in list)
                {
                    var temp = db.FindByName(item);
                    toSend.Add(item, temp.Avatar);
                }
            }

            return Clients.Group(channelName).LoadData(toSend);
        }

        public Task LeaveChannel(string channelName, string username)
        {
            this.Groups.Remove(this.Context.ConnectionId, channelName);

            Dictionary<string, byte[]> toSend = new Dictionary<string, byte[]>();

            using (UserDataRepository db = new UserDataRepository())
            {
                List<string> list = ChannelsInfo.channels[channelName].Members;

                foreach (var item in list)
                {
                    if (item != username)
                    {
                        var temp = db.FindByName(item);
                        toSend.Add(item, temp.Avatar);
                    }
                }
            }

            return Clients.Group(channelName).LoadData(toSend);
        }
    }
}