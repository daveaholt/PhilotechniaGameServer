using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace PhilotechniaGameServer
{
    public class UDP
    {
        public IPEndPoint endPoint;

        private int id;
        private ILogger logger;

        public UDP(int id)
        {
            this.id = id;
        }

        public void Connect(IPEndPoint endPoint, ILogger logger)
        {
            this.endPoint = endPoint;
            this.logger = logger;
            ServerSend.UDPTest(id);
        }

        public void SendData(Packet p)
        {
            Server.SendUDPData(endPoint, p);
        }

        public void HandleData(Packet p)
        {
            var packetLength = p.ReadInt();
            var packetBytes = p.ReadBytes(packetLength);

            ThreadManager.ExecuteOnMainThread(() => 
            {
                using(Packet p = new Packet(packetBytes))
                {
                    var packetId = p.ReadInt();
                    Server.PacketHandlers[packetId](id, p, logger);
                };
            }, logger);
        }
    }
}
