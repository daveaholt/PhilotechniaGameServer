using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace PhilotechniaGameServer
{
    public class Server
    {
        public static int MaxPlayers { get; private set; }
        public static Dictionary<int, Client> Clients = new Dictionary<int, Client>();

        public static int Port { get; private set; }

        private static TcpListener tcpListener;

        public static void Start(int maxPlayers, int portNumber)
        {
            MaxPlayers = maxPlayers;
            Port = portNumber;

            Console.WriteLine("Starting Server...");
            InitilizeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Server started on port {Port}.");

        }

        private static void TCPConnectCallback(IAsyncResult result)
        {
            var client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Incomming connnection from {client.Client.RemoteEndPoint}...");

            for (var i = 1; i <= MaxPlayers; i++)
            {
                if(Clients[i].tcp.socket == null)
                {
                    Clients[i].tcp.Connect(client);
                    return;
                }
            }

            Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: Server full.");
        }

        private static void InitilizeServerData()
        {
            for(var i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new Client(i));
            }
        }
    }
}
