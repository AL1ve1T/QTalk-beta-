using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceChatClientWindows.Interfaces
{
    public interface ILoginForm
    {
        string UserName { get; set; }
        string Password { get; set; }
    }
}
