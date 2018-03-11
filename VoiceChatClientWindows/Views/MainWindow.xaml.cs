using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Image = System.Drawing.Image;
using VoiceChatClientWindows.Interfaces;
using VoiceChatClientWindows.Controllers;
using VoiceChatClientWindows.Models;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using VoiceChatClientWindows.Models.VoiceClient;

namespace VoiceChatClientWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, ImageBrush> FriendsDictionary { get; set; }
        public Dictionary<string, ImageBrush> ChannelMemberList;
        private Thread CaptureKey;
        
        public MainWindow()
        {
            InitializeComponent();
        }
        
        [STAThread]
        private static void Talking()
        {
            while (true)
            {
                if (Keyboard.IsKeyDown(Key.F))
                {
                    DirectHelper.StopLoop = false;
                    Thread.Sleep(200);
                }
                else
                {
                    DirectHelper.StopLoop = true;
                    Thread.Sleep(200);
                }
            }
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void HideBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void AccountDetailsShow_Click(object sender, RoutedEventArgs e)
        {
            this.BlackPanel.Visibility = Visibility.Visible;
            this.BlackPanel.Opacity = 0.5;
            this.SlidePanel.Margin = new Thickness(0, -6, 554, 0);
        }

        private void AccountDetailsHide_Click(object sender, RoutedEventArgs e)
        {
            this.BlackPanel.Visibility = Visibility.Collapsed;
            this.BlackPanel.Opacity = 0;
            this.SlidePanel.Margin = new Thickness(-450, -6, 1004, 0);
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CreateChannel_Click(object sender, RoutedEventArgs e)
        {
            CreateChannel createChannel = new CreateChannel(this);
            createChannel.Show();
        }

        public ImageBrush FillImage(byte[] _avatarBytes)
        {
            MemoryStream mstream = new MemoryStream(_avatarBytes, 0, _avatarBytes.Length);
            mstream.Write(_avatarBytes, 0, _avatarBytes.Length);
            System.Drawing.Image Avatar = Image.FromStream(mstream, true);

            var bitmap = new System.Drawing.Bitmap(Avatar);
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
            bitmap.Dispose();

            return new ImageBrush(bitmapSource);
        }

        private async void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as Label;

            if (await ChannelController.JoinChannel(obj.Content.ToString(), this))
            {
                this.ChannelRoom.Visibility = System.Windows.Visibility.Visible;
                CaptureKey = new Thread(Talking);
                CaptureKey.SetApartmentState(ApartmentState.STA);
                CaptureKey.Start();
            }
        }

        private async void LeaveChannel_Click(object sender, RoutedEventArgs e)
        {
            if (await ChannelController.LeaveChannel(this.ChannelName.Content.ToString(), this))
            {
                this.ChannelRoom.Visibility = System.Windows.Visibility.Collapsed;
                CaptureKey.Abort();
            }
        }

        private void JoinChannel_Click(object sender, RoutedEventArgs e)
        {
            JoinChannel joinChannel = new VoiceChatClientWindows.JoinChannel(this);
            joinChannel.Show();
        }
    }
}
