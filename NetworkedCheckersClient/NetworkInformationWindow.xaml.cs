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
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NetworkedCheckersClient
{
    /// <summary>
    /// Interaction logic for NetworkInformationWindow.xaml
    /// </summary>
    public partial class NetworkInformationWindow : Window
    {
        string ip;
        int port;

        public NetworkInformationWindow()
        {
            InitializeComponent();
        }

        public string getIP()
        {
            return ip;
        }

        public int getPort()
        {
            return port;
        }

        private void portSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int value = (int)portSlider.Value;
            portTxt.Text = "Port: " + value.ToString();
        }

        private void ipTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.Match(ipTxt.Text, @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$").Success)
            {
                okBtn.IsEnabled = true;
            }
            else
            {
                okBtn.IsEnabled = false;
            }
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            ip = ipTxt.Text;
            port = (int)portSlider.Value;
            Close();
        }

    }
}
