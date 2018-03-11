using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VoiceChatClientWindows.Interfaces;

namespace VoiceChatClientWindows.Models
{
    public static class ChannelModel
    {
        public static async Task<ChannelDataModel> SendCreateRequestAsync(ICreateChannelForm _createChannelForm)
        {
            var requestModel = new
            {
                Name = _createChannelForm.ChannelName,
                Id = _createChannelForm.ChannelID,
                Owner = Client.Name
            };
            
            var response = await Client.client.PostAsJsonAsync("api/Channel/Create", requestModel);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            ChannelDataModel channelDataModel = new ChannelDataModel()
            {
                Id = requestModel.Id,
                Name = requestModel.Name
            };

            return channelDataModel;
        }

        public static async Task<string> SendJoinRequestAsync(string _channelName)
        {
            var body = new
            {
                ChannelName = _channelName,
                User = Client.Name
            };

            var response = await Client.client.PostAsJsonAsync("api/Channel/Join", body);
            var port = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return port;
        }

        public static async Task<bool> SendLeaveRequestAsync(string _channelName)
        {
            await Client.hubProxy.Invoke("LeaveChannel", _channelName, Client.Name);

            var body = new
            {
                ChannelName = _channelName,
                User = Client.Name
            };

            var response = await Client.client.PostAsJsonAsync("api/Channel/Leave", body);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            Client.voiceClient.Disconncet();
            return true;
        }
    }
}
