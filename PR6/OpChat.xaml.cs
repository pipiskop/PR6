using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PR6
{
    /// <summary>
    /// Логика взаимодействия для OpChat.xaml
    /// </summary>
    public partial class OpChat : Window
    {
        TcpServer server;
        TcpClient adminClient;

        public OpChat(string name)
        {
            InitializeComponent();
            server = new TcpServer(UsersList, LoginsList, this);
            if (!server.Start())
            {
                Environment.Exit(0);
            }
            this.Show();
            adminClient = new TcpClient(MessageList, this, IPAddress.Parse("127.0.0.1"), name, UsersList);
            adminClient.Start();
        }

        private void SendButton(object sender, RoutedEventArgs e)
        {
            adminClient.SendMessage(MessageText.Text);
            MessageText.Clear();
        }

        private void ExitButton(object sender, RoutedEventArgs e)
        {
            adminClient.SendMessage("/disconnect");
            adminClient.DisconnectServer();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            adminClient.SendMessage("/disconnect");
            adminClient.DisconnectServer();
        }
    }
}

