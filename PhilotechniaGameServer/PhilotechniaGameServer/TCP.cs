using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace PhilotechniaGameServer
{
    public class TCP
    {
        public static int defaultBufferSize = 4096;

        public TcpClient socket { get; set; }

        private Packet receiveData;
        private readonly int id;
        private NetworkStream stream;
        private byte[] receiveBuffer;

        private ILogger logger;

        public TCP(int id)
        {
            this.id = id;
        }

        public void Connect(TcpClient socket, ILogger logger)
        {
            this.logger = logger;
            this.socket = socket;
            socket.ReceiveBufferSize = defaultBufferSize;
            socket.SendBufferSize = defaultBufferSize;

            stream = socket.GetStream();
            receiveData = new Packet();
            receiveBuffer = new byte[defaultBufferSize];

            stream.BeginRead(receiveBuffer, 0, defaultBufferSize, ReceiveCallback, null);

            ServerSend.Welcome(id, "Welcome to the Philotechnia game server!");
        }

        public void SendData(Packet p)
        {
            try
            {
                if(socket != null)
                {
                    stream.BeginWrite(p.ToArray(), 0, p.Length(), null, null);
                }
            }
            catch(Exception ex)
            {
                logger.WriteLine($"Error sending data to player {id} via TCP: {ex}");
            }
        }

        private bool HandleData(byte[] data)
        {
            var packetLength = 0;

            receiveData.SetBytes(data);
            if (receiveData.UnreadLength() >= 4)
            {
                packetLength = receiveData.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= receiveData.UnreadLength())
            {
                var packetBytes = receiveData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet p = new Packet(packetBytes))
                    {
                        var packetId = p.ReadInt();
                        Server.PacketHandlers[packetId](id, p, logger);
                    };
                }, logger);

                packetLength = 0;
                if (receiveData.UnreadLength() >= 4)
                {
                    packetLength = receiveData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (packetLength <= 0)
            {
                return true;
            }
            return false;
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var byteLength = stream.EndRead(result);
                if (byteLength <= 0)
                {
                    //TODO: disconnect
                    return;
                }

                var data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                receiveData.Reset(HandleData(data));
                stream.BeginRead(receiveBuffer, 0, defaultBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                logger.WriteLine($"Error receiving TCP data: {ex}");
                //TODO: disconnect
            }
        }
    }
}
