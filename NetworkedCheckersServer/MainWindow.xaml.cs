using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkedCheckersServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal UDPMessageServer msgServer;
        internal string networkIP;
        internal int networkPort;

        public MainWindow()
        {
            InitializeComponent();
            msgServer = new UDPMessageServer();
            logList.ItemsSource = msgServer.messages;
            leaderboardList.ItemsSource = msgServer.leaderboard;
        }

        public void AddNetworkSetupMenuHandler(RoutedEventHandler handler)
        {
            networkSetupMenu.Click += handler;
        }

        public void AddMulticastSetupMenuHandler(RoutedEventHandler handler)
        {
            multicastSetupMenu.Click += handler;
        }

        public void AddStartServerMenuHandler(RoutedEventHandler handler)
        {
            startServerMenu.Click += handler;
        }

        public void AddStopServerMenuHandler(RoutedEventHandler handler)
        {
            stopServerMenu.Click += handler;
        }
    }
}
