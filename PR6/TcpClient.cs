using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using static PR6.Lobby;

namespace PR6
{
    public class TcpClient
    {
        DateTimeOffset currentTime = DateTimeOffset.Now;
        private Socket server;
        private ListBox MessageList;
        private ListBox UsersList;
        private Window window;
        private IPAddress ip;
        private string name;
        public TcpClient(ListBox listbox, Window window, IPAddress ip, string name, ListBox UsersList)
        {
            this.UsersList = UsersList;
            this.name = name;
            this.window = window;
            this.MessageList = listbox;
            this.ip = ip;
        }
        public void Start()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.ConnectAsync(ip, 8888);
            byte[] bytes = Encoding.UTF8.GetBytes($"(*&{name}");
            server.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

            ReceiveMessage();
        }

        private async Task ReceiveMessage()
        {
            byte[] bytes = new byte[1024];
            while (true)
            {
                try
                {
                    await server.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
                    string message = Encoding.UTF8.GetString(bytes);
                    if (message.Substring(0, 3) != "(*&")
                    {
                        MessageList.Items.Add($"[{currentTime.ToString("yyyy-MM-dd HH:mm:ss")}] {message}");
                    }
                    else if (message.Substring(0, 3) == "(*&")
                    {
                        List<string> users = new List<string>();
                        foreach (string user in message.Substring(3).Split(' '))
                        {
                            users.Add(user);
                        }
                        UsersList.ItemsSource = null;
                        UsersList.ItemsSource = users;
                    }
                }
                catch
                {
                    break;
                }
            }
            server.Close();
        }

        public async Task SendMessage(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await server.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
            if (message.Contains("/disconnect"))
            {
                DisconnectServer();
            }
        }
        public void DisconnectServer()
        {
            server.Close();
            window.Close();
        }
    }
}