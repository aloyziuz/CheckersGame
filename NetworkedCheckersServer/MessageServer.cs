using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedCheckersServer
{
    interface MessageServer
    {
        void connectToNetwork(string address, int port);

        void connectToMulticast(string multicastAddress, int multicastPort);

        void HandleRequests();

        void BroadcastMessages(string msg);
    }
}
