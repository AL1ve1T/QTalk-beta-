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
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window, IRegistrationForm
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void EmailBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            EmailBox.Text = "";
        }

        private void NickNameBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            NickNameBox.Text = "";
        }

        private void PassBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            PassBox.Password = "";
        }

        private void RePassBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            PassRepeatBox.Password = "";
        }

        private async void SendForm_Click(object sender, RoutedEventArgs e)
        {
            this.Email = EmailBox.Text;
            this.UserName = NickNameBox.Text;
            this.Password = PassBox.Password;
            this.ConfirmPassword = PassRepeatBox.Password;

            this.ErrorMsg.Content = "";

            if (this.Password != this.ConfirmPassword)
            {
                ErrorMsg.Content = "Repeat your password";
                return;
            }

            await AccountController.Register(this);
        }
    }
}
