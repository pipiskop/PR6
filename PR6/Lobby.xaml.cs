using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace PR6
{
    /// <summary>
    /// Логика взаимодействия для Lobby.xaml
    /// </summary>
    public partial class Lobby : Window
    {
        private string ipPattern = @"\b(?:\d{1,3}\.){3}\d{1,3}\b";
        private string nicknamePattern = @"^[A-Za-z0-9_]+$";
        public Lobby()
        {
            InitializeComponent();
        }

        private void CreateChat(object sender, RoutedEventArgs e)
        {
            if (!ValidateNickname())
            {
                return;
            }
            if (!ValidateInput())
            {
                return;
            }
            new OpChat(NameText.Text);
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }
            if (!ValidateInput())
            {
                return;
            }

            try
            {
                ConnectToChat(IPAddress.Parse(IPText.Text), NameText.Text);
            }
            catch
            {
                MessageBox.Show("Такого чата не существует");
                IPText.Text = null;
            }
        }

        private bool ValidateInput()
        {
            if (!Regex.IsMatch(IPText.Text, ipPattern, RegexOptions.IgnoreCase))
            {
                MessageBox.Show("Неправильный формат IP. Попробуйте снова");
                IPText.Text = null;
                return false;
            }

            return ValidateNickname();
        }

        private bool ValidateNickname()
        {
            if (!Regex.IsMatch(NameText.Text, nicknamePattern, RegexOptions.IgnoreCase))
            {
                MessageBox.Show("Неправильный формат никнейма. Попробуйте снова");
                NameText.Text = null;
                return false;
            }
            return true;
        }

        private void ConnectToChat(IPAddress ip, string nickname)
        {
            new UserChat(ip, nickname).Show();
        }
    }
}
