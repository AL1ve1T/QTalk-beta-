using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, ILoginForm
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Password_OnGotFocus(object sender, RoutedEventArgs e)
        {
            PassBox.Password = "";
        }

        private void Login_OnGotFocus(object sender, RoutedEventArgs e)
        {
            Login.Text = "";
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.UserName = Login.Text;
            this.Password = PassBox.Password;
            this.ErrorMsg.Content = "";
            if (await AccountController.Login(this))
            {
                this.Close();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
        }
    }
}
