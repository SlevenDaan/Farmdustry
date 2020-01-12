using System;
using System.Net;
using System.Net.Sockets;

namespace Farmdustry.Network
{
    sealed public class Client : NetworkEntity
    {
        private Socket socket;

        public bool IsConnected
        {
            get => socket != null && socket.Connected;
        }

        public event Action Connected;

        public Client(int port) : base(port)
        {

        }

        /// <summary>
        /// Connect to a server.
        /// </summary>
        /// <param name="iPAddress">The IPv4 address to connect to.</param>
        public void Connect(IPAddress iPAddress)
        {
            if (IsConnected)
            {
                throw new NetworkException("Client is already connected");
            }

            //Set up new socket
            socket = Network.CreateSocket();

            try
            {
                //Try connecting to the server
                IPEndPoint remoteEndPoint = new IPEndPoint(iPAddress, Port);
                socket.Connect(remoteEndPoint);

                //Start receiving data
                BeginReceiving(socket);
            }
            catch (SocketException)
            {
                throw new NetworkException($"Client can not connect to {iPAddress.ToString()}:{Port}");
            }
        }

        /// <summary>
        /// Disconnect the client.
        /// </summary>
        public override void Shutdown()
        {
            Disconnect(socket);
        }

        /// <summary>
        /// Sends data to the connected server.
        /// </summary>
        /// <param name="data">The data to send.</param>
        public void Send(byte[] data)
        {
            if (!IsConnected)
            {
                throw new NetworkException("The client is not yet connected");
            }

            try
            {
                socket.Send(data);
            }
            catch (SocketException)
            {
                //Client has disconnected
            }
        }
    }
}
