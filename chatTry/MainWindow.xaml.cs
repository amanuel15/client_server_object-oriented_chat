using ChattingInterfaces;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

namespace chatTry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static IChattingService Server;
        private static DuplexChannelFactory<IChattingService> _channelFactory;
        public MainWindow()
        {
            InitializeComponent();
            _channelFactory = new DuplexChannelFactory<IChattingService>(new ClientCallback(), "ChattingServiceEndPoint");
            Server = _channelFactory.CreateChannel();
            textDisplay.IsEnabled = false;
        }
        public void TakeMessage(string message, string userName)
        {
            textDisplay.Text += userName + ": " + message + "\n";
            textDisplay.ScrollToEnd();
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            if ( messageTB.Text.Length == 0)
            {
                return;
            }
            Server.SendMessageToALL(messageTB.Text, userNameTB.Text);
            TakeMessage(messageTB.Text, "you");
            messageTB.Text = "";
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            int returnValue = Server.Login(userNameTB.Text);
            if(returnValue == 1)
            {
                MessageBox.Show("You are already logged in. Try again");
            }
            else if(returnValue == 0)
            {
                MessageBox.Show("Login succefull!");
                labelUser.Content = "Welcom " + userNameTB.Text + "!";
                userNameTB.IsEnabled = false;
                loginButton.IsEnabled = false;

                LoadUserList(Server.GetCurrentUsers());
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Server.Logout();
        }

        public void AddUserToList(string userName)
        {
            if (UsersListBox.Items.Contains(userName))
            {
                return;
            }

            UsersListBox.Items.Add(userName);
        }

        public void RemoveUserFromList(string userName)
        {
            if (UsersListBox.Items.Contains(userName))
            {
                UsersListBox.Items.Remove(userName);
            }
        }
        private void LoadUserList(List<string> users)
        {
            foreach(var user in users)
            {
                AddUserToList(user);
            }
        }
    }
}
