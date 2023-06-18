using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

namespace PR6
{
    public class TcpServer
    {
        private const int MaxClients = 100;
        private readonly ListBox _usersListBox;
        private readonly ListBox _logsListBox;
        private readonly OpChat _window;

        private readonly List<Socket> _clients = new List<Socket>();
        private readonly List<string> _clientNames = new List<string>();

        private Socket _socket;

        public TcpServer(ListBox usersListBox, ListBox logsListBox, OpChat window)
        {
            _usersListBox = usersListBox;
            _logsListBox = logsListBox;
            _window = window;
        }

        public bool Start()
        {
            try
            {
                var ipEndPoint = new IPEndPoint(IPAddress.Any, 8888);

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(ipEndPoint);
                _socket.Listen(MaxClients);

                ListeningAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task ListeningAsync()
        {
            while (true)
            {
                var client = await _socket.AcceptAsync();
                ReceiveMessageAsync(client);
            }
        }

        private async Task ReceiveMessageAsync(Socket client)
        {
            var nick = "";
            string allClients = null;

            while (true)
            {
                var bytes = new byte[1024];
                await client.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
                var message = Encoding.UTF8.GetString(bytes);

                if (message.StartsWith("/disconnect", StringComparison.OrdinalIgnoreCase))
                {
                    _logsListBox.Items.Add($"[{nick}] Вышел");
                    break;
                }

                if (message.StartsWith("(*&"))
                {
                    message = message.Substring(3).TrimEnd('\0');
                    _clientNames.Add(message);
                    nick = message;

                    _logsListBox.Items.Add($"[{nick}] Зашел");

                    _clients.Add(client);
                    _usersListBox.ItemsSource = null;
                    _usersListBox.ItemsSource = _clientNames;

                    allClients = string.Join(" ", _clientNames);
                    foreach (var clientUser in _clients)
                    {
                        await SendMessage(clientUser, $"(*&{allClients}");
                    }
                }
                else
                {
                    foreach (var item in _clients)
                    {
                        await SendMessage(item, "[" + nick + "]: " + message.Substring(0, message.IndexOf('\0')));
                    }
                }
            }

            if (client == _socket)
            {
                return;
            }

            _clients.Remove(client);
            _clientNames.Remove(nick);

            _usersListBox.ItemsSource = null;
            _usersListBox.ItemsSource = _clientNames;

            allClients = string.Join(" ", _clientNames);
            foreach (var clientUser in _clients)
            {
                await SendMessage(clientUser, $"(*&{allClients}");
            }

            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        public async Task SendMessage(Socket client, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
        }
    }
}
