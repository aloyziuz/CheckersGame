using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;

namespace NetworkedCheckersServer
{
    /// <summary>
    /// Interaction logic for networkSetupWindow.xaml
    /// </summary>
    public partial class NetworkSetupWindow : Window
    {
        internal string address;
        internal int portNumber;

        public NetworkSetupWindow()
        {
            InitializeComponent();
        }

        private void portSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            portLbl.Content = "Port Number : " + (int)portSlider.Value;
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            address = ipTxt.Text;
            portNumber = (int)portSlider.Value;
            this.Close();
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
    }
}
