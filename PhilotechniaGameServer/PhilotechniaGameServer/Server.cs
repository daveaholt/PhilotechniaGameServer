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
        public delegate void PacketHandler(int fromClient, Packet p, ILogger l);
        public static Dictionary<int, PacketHandler> PacketHandlers;
        private static ILogger logger;
        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        public static void Start(int maxPlayers, int portNumber, ILogger loggerImpl)
        {
            MaxPlayers = maxPlayers;
            Port = portNumber;
            logger = loggerImpl;

            logger.WriteLine("Starting Server...");
            InitilizeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            logger.WriteLine($"Server started on port {Port}.");

        }

        public static void SendUDPData(IPEndPoint clientEndPoint, Packet p)
        {
            try
            {
                if(clientEndPoint != null)
                {
                    udpListener.BeginSend(p.ToArray(), p.Length(), clientEndPoint, null, null);
                }
            }
            catch(Exception ex)
            {
                logger.WriteLine($"Error sending data to {clientEndPoint} via UDP: {ex}");
            }
        }

        private static void TCPConnectCallback(IAsyncResult result)
        {
            var client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            logger.WriteLine($"Incomming connnection from {client.Client.RemoteEndPoint}...");

            for (var i = 1; i <= MaxPlayers; i++)
            {
                if(Clients[i].tcp.socket == null)
                {
                    Clients[i].tcp.Connect(client, logger);
                    return;
                }
            }

            logger.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: Server full.");
        }

        private static void UDPReceiveCallback(IAsyncResult result)
        {
            try
            {
                var clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var data = udpListener.EndReceive(result, ref clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < 4)
                {
                    return;
                }

                using(var p = new Packet(data))
                {
                    var clientId = p.ReadInt();

                    if(clientId == 0)
                    {
                        return;
                    }

                    if(Clients[clientId].udp.endPoint == null)
                    {
                        Clients[clientId].udp.Connect(clientEndPoint, logger);
                        return;
                    }

                    if(Clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString())
                    {
                        Clients[clientId].udp.HandleData(p);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.WriteLine($"Error receiving UDP data: {ex}");
            }
        }

        private static void InitilizeServerData()
        {
            for(var i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new Client(i));
            }

            PacketHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReveived },
                {(int)ClientPackets.udpTestReveived, ServerHandle.UDPTestReveived }
            };
            logger.WriteLine("Initilized packets.");
        }
    }
}
