using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VoiceChatClientWindows.Models.VoiceClient;

namespace VoiceChatClientWindows.Models
{
    public static class Client
    {
        public static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:63633/")
        };
        
        public static List<string> RecentlyVisitedChannels { get; set; }
        public static Dictionary<string, byte[]> ChannelMemberList;
        public static string Name { get; set; }
        public static string AuthorizationToken { get; set; }
        public static int Port { get; set; }
        public static VoiceClient.VoiceClient voiceClient;
        public static HubConnection hubConnection = new HubConnection("http://localhost:63633/");
        public static IHubProxy hubProxy;
    }
}
