using System.Net.WebSockets;
using System.Collections.Concurrent;
using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;

namespace esyasoft.mobility.CHRGUP.service.ocpp.WebSockets
{
    public static class ChargerConnectionManager
    {
        private static readonly ConcurrentDictionary<string, WebSocket> _sockets
            = new();

        private static readonly ConcurrentDictionary<string, ChargerConnection> _connections = new();

        public static void RegisterConnection(string chargePointId, ChargerConnection connection)
        {
            _connections[chargePointId] = connection;
        }

        public static void Add(string chargePointId, WebSocket socket)
        {
            _sockets[chargePointId] = socket;
        }

        public static void Remove(string chargePointId)
        {
            _sockets.TryRemove(chargePointId, out _);
            _connections.TryRemove(chargePointId, out _);
        }

        public static WebSocket? GetSocket(string chargePointId)
        {
            return _sockets.TryGetValue(chargePointId, out var s) && s.State == WebSocketState.Open
               ? s
               : null;
        }

        public static ChargerConnection? GetConnection(string chargePointId)
        {
            _connections.TryGetValue(chargePointId, out var c);
            return c;
        }
    }
}
