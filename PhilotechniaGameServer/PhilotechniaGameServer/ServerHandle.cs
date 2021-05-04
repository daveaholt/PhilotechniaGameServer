using System;
using System.Collections.Generic;
using System.Text;

namespace PhilotechniaGameServer
{
    class ServerHandle
    {
        public static void WelcomeReveived(int fromClient, Packet p, ILogger logger)
        {
            var clientIdCheck = p.ReadInt();
            var username = p.ReadString();

            logger.WriteLine($"{Server.Clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}");
            if (fromClient != clientIdCheck)
            {
                logger.WriteLine($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client Id: {clientIdCheck}!");
            }
        }

        public static void UDPTestReveived(int fromClient, Packet p, ILogger logger)
        {
            var msg = p.ReadString();

            logger.WriteLine($"Received packet via UDP. Contains message: {msg}");
        }
    }
}
