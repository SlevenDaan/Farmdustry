using Farmdustry.Network.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Farmdustry.Network
{
    sealed public class Server : NetworkEntity
    {
        private Socket listenSocket;

        private struct Client
        {
            private static byte nextId = 1; //0 is used as the server
            public byte Id { get; private set; }
            public Socket Socket { get; private set; }

            public Client(Socket socket)
            {
                Id = nextId++;
                Socket = socket;
            }
        }
        private List<Client> acceptedClients = new List<Client>();

        public ServerState State
        {
            get;
            private set;
        } = ServerState.Up;
        
        public int MaxQueuedConnections
        {
            get;
            private set;
        }

        /// <summary>
        /// Called when a new connection is established.
        /// </summary>
        public event Action ConnectionEstablished;

        public Server(int port, int maxQueuedConnections = 5) : base(port)
        {
            MaxQueuedConnections = maxQueuedConnections;

            listenSocket = Network.CreateSocket();
            BeginListening();
        }

        /// <summary>
        /// Start listening for new connections.
        /// </summary>
        private void BeginListening()
        {
            IPEndPoint localEndPoint = new IPEndPoint(Network.GetLocalIp(), Port);
            try
            {
                listenSocket.Bind(localEndPoint);
                listenSocket.Listen(MaxQueuedConnections);
                BeginAccepting();
            }
            catch (SocketException)
            {
                throw new NetworkException($"Port {Port} is already in use");
            }
        }

        /// <summary>
        /// Start accepting new connections.
        /// </summary>
        private void BeginAccepting()
        {
            listenSocket.BeginAccept(AcceptCallback, listenSocket);
        }
        /// <summary>
        /// Accept new connections.
        /// </summary>
        /// <param name="asyncResult"></param>
        private void AcceptCallback(IAsyncResult asyncResult)
        {
            //Stops accepting new connections when listenSocket is shutdown
            if (State==ServerState.Down)
            {
                return;
            }

            //Accept the new connection
            Socket acceptedSocket = listenSocket.EndAccept(asyncResult);
            byte clientId = AddClient(acceptedSocket);

            //Send the id to the client
            acceptedSocket.Send(Commands.SetPlayerId(clientId));

            //Start receiving data from the client
            ConnectionEstablished?.Invoke();
            BeginReceiving(acceptedSocket);

            //Start accepting the next connection
            BeginAccepting();
        }

        /// <summary>
        /// Add socket to the acceptedClients list.
        /// </summary>
        /// <param name="socket">The socket to add.</param>
        /// <returns>The id of the newly added client.</returns>
        private byte AddClient(Socket socket)
        {
            Client newClient = new Client(socket);
            acceptedClients.Add(newClient);
            return newClient.Id;
        }

        /// <summary>
        /// Disconnects all clients and stops listening for new connections.
        /// </summary>
        public override void Shutdown()
        {
            State = ServerState.Down;
            foreach (var client in acceptedClients)
            {
                Disconnect(client.Socket);
            }
            listenSocket.Close();
        }

        /// <summary>
        /// Sends data to all connected clients.
        /// </summary>
        /// <param name="data">The data to send.</param>
        public void Send(byte[] data)
        {
            foreach (var client in acceptedClients)
            {
                if (client.Socket.Connected)
                {
                    try
                    {
                        client.Socket.Send(data);
                    }
                    catch (SocketException)
                    {
                        //Client has disconnected
                    }
                }
            }
        }
        /// <summary>
        /// Sends data to a list of connected clients.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="clientIds">The set of all client id's to send it to.</param>
        public void Send(byte[] data, HashSet<byte> clientIds)
        {
            foreach (var client in acceptedClients)
            {
                if (clientIds.Contains(client.Id) && client.Socket.Connected)
                {
                    try
                    {
                        client.Socket.Send(data);
                    }
                    catch (SocketException)
                    {
                        //Client has disconnected
                    }
                }
            }
        }

        /// <summary>
        /// Removes all disconnected clients.
        /// </summary>
        public void CleanUp()
        {
            acceptedClients = new List<Client>(acceptedClients.Where(client => client.Socket.Connected));
        }
    }
}
