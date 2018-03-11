using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceChatClientWindows.Interfaces
{
    public interface ICreateChannelForm
    {
        string ChannelName { get; set; }
        string ChannelID { get; set; }
    }
}
