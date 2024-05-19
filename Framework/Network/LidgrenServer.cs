using Lidgren.Network;
using StardewValley;
using System;
using System.IO;
using System.Linq;
using System.Net;
using StardewModdingAPI;
using StardewValley.Network;

namespace MultiplayerMod.Framework.Network
{
    internal class LidgrenServer : StardewValley.Network.Server
    {
        public const int defaultPort = 24642;
        public NetServer server;
        private HashSet<NetConnection> introductionsSent = new HashSet<NetConnection>();
        protected Bimap<long, NetConnection> peers = new Bimap<long, NetConnection>();

        public override int connectionsCount
        {
            get
            {
                if (server == null)
                {
                    return 0;
                }
                return server.ConnectionsCount;
            }
        }

        public LidgrenServer(IGameServer gameServer)
            : base(gameServer)
        {
        }

        public override bool isConnectionActive(string connectionID)
        {
            foreach (NetConnection connection in server.Connections)
            {
                if (getConnectionId(connection) == connectionID && connection.Status == NetConnectionStatus.Connected)
                {
                    return true;
                }
            }
            return false;
        }

        public override string getUserId(long farmerId)
        {
            if (!peers.ContainsLeft(farmerId))
            {
                return null;
            }
            return peers[farmerId].RemoteEndPoint.Address.ToString();
        }

        public override bool hasUserId(string userId)
        {
            foreach (NetConnection rightValue in peers.RightValues)
            {
                if (rightValue.RemoteEndPoint.Address.ToString().Equals(userId))
                {
                    return true;
                }
            }
            return false;
        }

        public override string getUserName(long farmerId)
        {
            if (!peers.ContainsLeft(farmerId))
            {
                return null;
            }
            return peers[farmerId].RemoteEndPoint.Address.ToString();
        }

        public override float getPingToClient(long farmerId)
        {
            if (!peers.ContainsLeft(farmerId))
            {
                return -1f;
            }
            return peers[farmerId].AverageRoundtripTime / 2f * 1000f;
        }

        public override void setPrivacy(ServerPrivacy privacy)
        {
        }

        public override bool canAcceptIPConnections()
        {
            return true;
        }

        public override bool connected()
        {
            return server != null;
        }

        public override void initialize()
        {
            Console.WriteLine("Starting LAN server");
            NetPeerConfiguration config = new NetPeerConfiguration("StardewValley");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.Port = defaultPort;
            config.ConnectionTimeout = 30f;
            config.PingInterval = 5f;
            
            config.MaximumConnections = ModUtilities.multiplayer.playerLimit * 2;
            config.MaximumTransmissionUnit = 1200;
            server = new NetServer(config);
            server.Start();
            ModUtilities.ModMonitor.Log($"LidgrenServer is running with port {config.Port}... max player : " + ModUtilities.multiplayer.playerLimit , LogLevel.Alert);
        }

        public override void stopServer()
        {
            Console.WriteLine("Stopping LAN server");
            server.Shutdown("Server shutting down...");
            server.FlushSendQueue();
            introductionsSent.Clear();
            peers.Clear();
        }

        public static bool IsLocal(string host_name_or_address)
        {
            if (string.IsNullOrEmpty(host_name_or_address))
            {
                return false;
            }
            try
            {
                IPAddress[] hostAddresses = Dns.GetHostAddresses(host_name_or_address);
                IPAddress[] local_ips = Dns.GetHostAddresses(Dns.GetHostName());
            if (bandwidthLogger != null)
            {
                bandwidthLogger.RecordBytesUp(msg.LengthBytes);
            }
        }

        public override void setLobbyData(string key, string value)
        {
        }
    }
}
