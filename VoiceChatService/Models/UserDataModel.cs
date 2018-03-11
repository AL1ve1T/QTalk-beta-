using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Web.UI.WebControls;

namespace VoiceChatService.Models
{
    public class UserDataModel
    {
        public static UserDataModel Construct()
        {
            return new UserDataModel();
        }
        
        public string Id { get; set; }
        public string UserName { get; set; }
        public byte[] Avatar { get; set; }
        public string FriendList { get; set; }
        public string RecenltyVisitedRooms { get; set; }
    }

    public class UserData
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public byte[] Avatar { get; set; }
        public Dictionary<string, byte[]> Friends { get; set; }
        public string RecentlyVisitedRooms { get; set; }

        public UserData(UserDataModel userDataModel)
        {
            UserDataRepository db = new UserDataRepository();
            Friends = new Dictionary<string, byte[]>();

            this.Id = userDataModel.Id;
            this.UserName = userDataModel.UserName;
            this.Avatar = userDataModel.Avatar;
            this.RecentlyVisitedRooms = userDataModel.RecenltyVisitedRooms;

            string[] friends = userDataModel.FriendList.Split(',');

            // Get Friends' metadata

            foreach (string friend in friends)
            {
                if (friend != "")
                {
                    UserDataModel userModel = db.FindByName(friend);
                    this.Friends.Add(friend, userModel.Avatar);
                }
            }
        }
    }
}