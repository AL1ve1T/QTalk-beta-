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

namespace VoiceChatClientWindows
{
    /// <summary>
    /// Interaction logic for CreateChannel.xaml
    /// </summary>
    public partial class CreateChannel : Window, ICreateChannelForm
    {
        public string ChannelName { get; set; }
        public string ChannelID { get; set; }
        public MainWindow mainWindow { get; set; }

        public CreateChannel(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void RoomName_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RoomName.Text = "";
        }

        private void RoomID_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RoomID.Text = "";
        }

        private async void SendCreateRequest_OnClick(object sender, RoutedEventArgs e)
        {
            this.ChannelName = RoomName.Text;
            this.ChannelID = RoomID.Text;

            await ChannelController.CreateChannel(this, mainWindow);
        }

        private void RoomName_OnGotFocus(object sender, RoutedEventArgs e)
        {
            RoomName.Text = "";
        }

        private void RoomID_OnGotFocus(object sender, RoutedEventArgs e)
        {
            RoomID.Text = "";
        }
    }
}
