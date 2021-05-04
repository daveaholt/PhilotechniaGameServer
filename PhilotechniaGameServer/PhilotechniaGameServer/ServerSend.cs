using System;
using System.Collections.Generic;
using System.Text;

namespace PhilotechniaGameServer
{
    public class ServerSend
    {
        public static void Welcome(int toClient, string msg)
        {
            using (Packet p = new Packet((int)ServerPackets.welcome))
            {
                p.Write(msg);
                p.Write(toClient);

                SendTCPData(toClient, p);
            };
        }

        public static void UDPTest(int toClient)
        {
            using(Packet p = new Packet((int)ServerPackets.udpTest))
            {
                p.Write("A test packet for UDP");
                SendUDPData(toClient, p);
            }
        }

        private static void SendTCPDataToAll(Packet p)
        {
            p.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.Clients[i].tcp.SendData(p);
            }
        }

        private static void SendTCPDataToAll(int exceptClient, Packet p)
        {
            p.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if(i != exceptClient)
                {
                    Server.Clients[i].tcp.SendData(p);
                }                
            }
        }

        private static void SendUDPDataToAll(Packet p)
        {
            p.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.Clients[i].udp.SendData(p);
            }
        }

        private static void SendUDPDataToAll(int exceptClient, Packet p)
        {
            p.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != exceptClient)
                {
                    Server.Clients[i].udp.SendData(p);
                }
            }
        }

        private static void SendUDPData(int c, Packet p)
        {
            p.WriteLength();
            Server.Clients[c].udp.SendData(p);
        }

        private static void SendTCPData(int c, Packet p)
        {
            p.WriteLength();
            Server.Clients[c].tcp.SendData(p);  
        }
    }
}
