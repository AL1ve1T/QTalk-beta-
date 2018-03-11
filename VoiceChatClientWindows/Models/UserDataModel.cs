using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceChatClientWindows.Models
{
    public class UserDataModel
    {
        public string UserName { get; set; }
        public string Id { get; set; }
        public byte[] Avatar { get; set; }
        public Dictionary<string, byte[]> Friends { get; set; }
        public string AccessToken { get; set; }
        public string RecentlyVisitedRooms { get; set; }
    }
}
