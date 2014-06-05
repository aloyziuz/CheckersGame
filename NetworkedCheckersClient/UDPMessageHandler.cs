using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedCheckersClient
{
    class UDPMessageHandler : MessageHandler
    {
        private UdpClient udpClient;

        public void connectTo(string ip, int port)
        {
            udpClient = new UdpClient(ip, port);
        }

        public void sendRequest(string request)
        {
            var requestBytes = Encoding.UTF8.GetBytes(request);
            udpClient.Send(requestBytes, requestBytes.Length);
        }

        public string getResponse()
        {
            var endPoint = new IPEndPoint(IPAddress.Any, 50000);
            var responseBytes = udpClient.Receive(ref endPoint);
            var response = Encoding.UTF8.GetString(responseBytes);
            return response;
        }
    }
}
