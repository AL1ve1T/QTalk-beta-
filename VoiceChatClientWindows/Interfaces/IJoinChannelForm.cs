using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceChatClientWindows.Interfaces
{
    interface IJoinChannelForm
    {
        string UserName { get; set; }
        string ChannelName { get; set; }
    }
}
