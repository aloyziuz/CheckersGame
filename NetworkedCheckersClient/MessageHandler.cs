using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedCheckersClient
{
    interface MessageHandler
    {
        void connectTo(string ip, int port);

        void sendRequest(string request);

        string getResponse();
    }
}
