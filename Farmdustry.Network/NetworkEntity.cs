using System;
using System.Net.Sockets;

namespace Farmdustry.Network
{
    public abstract class NetworkEntity
    {
        public int Port
        {
            get;
            private set;
        }

        public event Action<byte[]> DataReceived;

        public NetworkEntity(int port)
        {
            Port = port;
        }

        protected void BeginReceiving(Socket receivingSocket)
        {
            receivingSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, ReceivingCallback, receivingSocket);
        }

        private void ReceivingCallback(IAsyncResult asyncResult)
        {
            Socket receivingSocket = (Socket)asyncResult.AsyncState;

            //Handle closed connections
            if (!receivingSocket.Connected) 
            {
                return;
            }

            try
            {
                byte[] receivedData = new byte[receivingSocket.Available];
                receivingSocket.Receive(receivedData, SocketFlags.None);
                DataReceived?.Invoke(receivedData);
                BeginReceiving(receivingSocket);
            }
            //Handle abrupt disconnections
            catch (SocketException) 
            {
                Disconnect(receivingSocket);
            }
        }

        public abstract void Shutdown();

        protected void Disconnect(Socket socket)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
