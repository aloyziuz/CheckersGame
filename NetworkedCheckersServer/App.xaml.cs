using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace NetworkedCheckersServer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static MainWindow serverWindow;
        string ipAddress, multicastAddress, p1Name, p2Name;
        int multicastPort, networkPort;
        Thread requestHandlerThread;

        public App()
            : base()
        {
            //get this host ip address
            ipAddress = "";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                }
            }
            
            networkPort = 50000;

            serverWindow = new MainWindow();
            serverWindow.Show();

            serverWindow.AddNetworkSetupMenuHandler(HandleNetworkSetupMenu);
            serverWindow.AddMulticastSetupMenuHandler(HandleMulticastMenu);
            serverWindow.AddStartServerMenuHandler(HandleStartServerMenu);
            serverWindow.AddStopServerMenuHandler(HandleStopServerMenu);

            Record("System Start Up Complete");
        }

        public void HandleNetworkSetupMenu(object sender, RoutedEventArgs e)
        {
            NetworkSetupWindow dialog = new NetworkSetupWindow();
            dialog.ipTxt.Text = ipAddress;
            dialog.Owner = serverWindow;
            dialog.ShowDialog();
            networkPort = dialog.portNumber;

            Record("Server IP: " + ipAddress);
            Record("Server Port: " + networkPort.ToString());
        }

        public void HandleMulticastMenu(object sender, RoutedEventArgs e)
        {
            //modify the network setup window a bit
            NetworkSetupWindow dialog = new NetworkSetupWindow();
            dialog.ipLbl.IsEnabled = true;
            dialog.ipTxt.IsEnabled = true;
            dialog.okBtn.IsEnabled = false;
            dialog.portSlider.Value = 50001;
            dialog.Owner = serverWindow;
            dialog.ShowDialog();
            multicastAddress = dialog.address;
            multicastPort = dialog.portNumber;
            Record("Multicast IP: " + multicastAddress);
            Record("Multicast Port: " + multicastPort.ToString());
        } 
        public void HandleStartServerMenu(object sender, RoutedEventArgs e)
        {
            serverWindow.startServerMenu.IsEnabled = false;
            serverWindow.stopServerMenu.IsEnabled = true;

            serverWindow.msgServer.connectToNetwork(ipAddress, networkPort);

            requestHandlerThread = new Thread(serverWindow.msgServer.HandleRequests);
            requestHandlerThread.IsBackground = true;
            requestHandlerThread.Start();

            serverWindow.msgServer.connectToMulticast(multicastAddress, multicastPort);

            Record("Server Started");
        }

        public void HandleStopServerMenu(object sender, RoutedEventArgs e)
        {
            serverWindow.startServerMenu.IsEnabled = true;
            serverWindow.stopServerMenu.IsEnabled = false;
            requestHandlerThread = null;
            Record("Server Stopped");
        }

        public void Record(string msg)
        {
            Dispatcher.Invoke(() =>
            {
                string time = DateTime.Now.ToShortTimeString();
                serverWindow.msgServer.messages.Insert(0, time + ": " + msg);
            });
        }
    }
}
