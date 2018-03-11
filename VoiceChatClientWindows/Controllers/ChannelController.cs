using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Media;
using VoiceChatClientWindows.Interfaces;
using VoiceChatClientWindows.Models;
using Microsoft.AspNet.SignalR.Client;
using VoiceChatClientWindows.Models.VoiceClient;
using System.Collections.Concurrent;
using System.Windows.Threading;

namespace VoiceChatClientWindows.Controllers
{
    public static class ChannelController
    {
        public static async Task<bool> CreateChannel(CreateChannel createChannelForm, MainWindow mainWindow)
        {
            var response = await ChannelModel.SendCreateRequestAsync(createChannelForm as ICreateChannelForm);

            if (response != null)
            {
                createChannelForm.ErrorMsg.Foreground = Brushes.Green;
                createChannelForm.ErrorMsg.Content = "Ok!";
                
                Client.RecentlyVisitedChannels.Add(response.Name);
                mainWindow.RecentlyVisitedRooms.Items.Refresh();
                return true;
            }
            else
            {
                createChannelForm.ErrorMsg.Foreground = Brushes.Red;
                createChannelForm.ErrorMsg.Content = "Exists...";
                return false;
            }
        }

        public static async Task<bool> JoinChannel(string _channelName, MainWindow mainWindow)
        {
            string Port = await ChannelModel.SendJoinRequestAsync(_channelName);

            if (Port == null)
            {
                return false;
            }

            Client.Port = Convert.ToInt32(Port);
            try
            {
                Client.voiceClient = new VoiceClient();
                Client.voiceClient.Connect("192.168.1.112", Client.Port);
                Client.hubProxy = Client.hubConnection.CreateHubProxy("ChatHub");
                
                await Client.hubConnection.Start();
                
                Client.ChannelMemberList = new Dictionary<string, byte[]>();
                Client.hubProxy.On<Dictionary<string, byte[]>>("LoadData", members =>
                {
                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        Client.ChannelMemberList = members;
                        mainWindow.ChannelMemberList = new Dictionary<string, ImageBrush>();
                        foreach (var member in Client.ChannelMemberList)
                        {
                            mainWindow.ChannelMemberList.Add(member.Key, mainWindow.FillImage(member.Value));
                        }
                        mainWindow.MemberList.ItemsSource = mainWindow.ChannelMemberList;
                        mainWindow.MemberList.Items.Refresh();
                    });
                });
                await Client.hubProxy.Invoke("JoinChannel", _channelName, Client.Name);
                mainWindow.ChannelName.Content = _channelName;
            }
            catch (SocketException)
            {
                return false;
            }

            if (!Client.RecentlyVisitedChannels.Contains(_channelName))
            {
                Client.RecentlyVisitedChannels.Add(_channelName);
                mainWindow.RecentlyVisitedRooms.Items.Refresh();
            }
            DirectHelper.OnBufferFulfill += new EventHandler(Client.voiceClient.SendVoiceBuffer);

            return true;
        }

        public static async Task<bool> LeaveChannel(string _channelName, MainWindow mainWindow)
        {
            if (!await ChannelModel.SendLeaveRequestAsync(_channelName))
            {
                return false;
            }
            Client.hubConnection.Stop();
            DirectHelper.OnBufferFulfill -= Client.voiceClient.SendVoiceBuffer;
            return true;
        }
    }
}
