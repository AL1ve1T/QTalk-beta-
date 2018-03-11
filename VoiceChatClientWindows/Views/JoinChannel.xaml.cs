using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VoiceChatClientWindows.Controllers;
using VoiceChatClientWindows.Interfaces;
using VoiceChatClientWindows.Models;

namespace VoiceChatClientWindows
{
    /// <summary>
    /// Interaction logic for JoinChannel.xaml
    /// </summary>
    public partial class JoinChannel : Window
    {
        private static MainWindow mainWindow { get; set; }

        public JoinChannel(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private async void SendJoinRequest_Click(object sender, RoutedEventArgs e)
        {
            if (!await ChannelController.JoinChannel(ChannelNameBox.Text, mainWindow))
            {
                ErrorMsg.Foreground = Brushes.Red;
                ErrorMsg.Content = "Error occured";
            }
            else
            {
                this.Hide();
                mainWindow.ChannelRoom.Visibility = Visibility.Visible;
            }
        }

        private void ChannelName_GotFocus(object sender, RoutedEventArgs e)
        {
            ChannelNameBox.Text = "";
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
