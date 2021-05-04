using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace PhilotechniaGameServer
{
    public class Client
    {
        public int Id { get; set; }
        public TCP tcp { get; set; }
        public UDP udp { get; set; }

        public Client(int clientId)
        { 
            Id = clientId;
            tcp = new TCP(Id);
            udp = new UDP(Id);
        }        
    }
}
