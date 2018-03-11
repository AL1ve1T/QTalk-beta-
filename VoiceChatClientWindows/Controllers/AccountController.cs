using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Mime;
using System.Text;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using VoiceChatClientWindows;
using VoiceChatClientWindows.Interfaces;
using VoiceChatClientWindows.Models;
using Brushes = System.Windows.Media.Brushes;

namespace VoiceChatClientWindows.Controllers
{
    public static class AccountController
    {
        public static async Task Register(RegistrationWindow registrationForm)
        {
            string result = await AccountModel.RegisterAsync(registrationForm as IRegistrationForm);

            if (result != "Ok")
            {
                registrationForm.ErrorMsg.Content = result;
            }
            else
            {
                registrationForm.ErrorMsg.Foreground = Brushes.Green;
                registrationForm.ErrorMsg.Content = "Check your E-mail and confirm the registration";
            }
        }

        public static async Task<bool> Login(LoginWindow loginForm)
        {
            var User = await AccountModel.LoginAsync(loginForm as ILoginForm);
            
            if (User == null)
            {
                loginForm.ErrorMsg.Content = "Invalid password";
                return false;
            }
            
            MainWindow mainWindow = new MainWindow();
            mainWindow.UserName.Content = User.UserName;
            mainWindow.FriendsDictionary = new Dictionary<string, ImageBrush>();

            foreach (var userFriend in User.Friends)
            {
                mainWindow.FriendsDictionary.Add(userFriend.Key, mainWindow.FillImage(userFriend.Value));
            }

            mainWindow.FriendListBox.ItemsSource = mainWindow.FriendsDictionary;

            Client.RecentlyVisitedChannels = User.RecentlyVisitedRooms.Split(',').ToList();
            Client.RecentlyVisitedChannels.Remove("");
            mainWindow.RecentlyVisitedRooms.ItemsSource = Client.RecentlyVisitedChannels;
            mainWindow.Avatar.Fill = mainWindow.FillImage(User.Avatar);
            
            mainWindow.Show();
            return true;
        }
    }
}
