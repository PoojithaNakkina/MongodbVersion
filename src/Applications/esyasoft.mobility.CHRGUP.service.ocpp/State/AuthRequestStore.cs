using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace esyasoft.mobility.CHRGUP.service.ocpp.State
{
    public record AuthRequest(string ChargerId, int EvseId, WebSocket Socket);
    public class AuthRequestStore
    {
        private static readonly ConcurrentDictionary<string, AuthRequest> _pending = new();

        public static void Register(string messageId, string chargerId, int evseId, WebSocket socket)
        {
            _pending[messageId] = new AuthRequest(chargerId, evseId, socket);
        }

        public static bool TryTake(string messageId, out AuthRequest request)
        {
            return _pending.TryRemove(messageId, out request!);
        }
    }
}
