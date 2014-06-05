using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetworkedCheckersClient
{
    class UDPBroadcastListener : BroadcastListener
    {
        int broadcastPort;
        string multicastAddress;

        public UDPBroadcastListener(string address, int port)
        {
            broadcastPort = port;
            multicastAddress = address;
        }

        public void ListenBroadcasts()
        {
            var localIP = new IPEndPoint(IPAddress.Any, broadcastPort);
            var udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.ExclusiveAddressUse = false;
            udpClient.Client.Bind(localIP);

            var multicastingIP = IPAddress.Parse(multicastAddress);
            udpClient.JoinMulticastGroup(multicastingIP);

            var running = true;
            while (running)
            {
                var bytes = udpClient.Receive(ref localIP);
                var message = Encoding.UTF8.GetString(bytes);

                if (Regex.Match(message, @"^USERS,[A-Za-z:]+$").Success)
                {

                }
                else if (Regex.Match(message, @"^GAMESTATE,[A-Za-z0-9|]+$").Success)
                {

                }
                else if (Regex.Match(message, @"^GAMESTART,[A-Za-z]+,[A-Za-z]+$").Success)
                {

                }
                else if (Regex.Match(message, @"^STATUS,[A-Za-z]+$").Success)
                {

                }
            }

            udpClient.DropMulticastGroup(multicastingIP);
            udpClient.Close();
        }
    }
}
