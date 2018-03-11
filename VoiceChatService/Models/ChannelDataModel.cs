using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VoiceChatService.Models
{
    public class ChannelDataModel
    {
        public ChannelDataModel Create()
        {
            return new ChannelDataModel();
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}