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
            receiveBuffer = new byte[defaultBufferSize];

            stream.BeginRead(receiveBuffer, 0, defaultBufferSize, ReceiveCallback, null);
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

                //TODO: handle data
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
