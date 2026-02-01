
using System.Collections.Concurrent;

namespace esyasoft.mobility.CHRGUP.service.ocpp.State
{
    public enum OcppProtocol
    {
        V16,
        V201
    }
    public class ChargerProtocolStore
    {
        private static readonly ConcurrentDictionary<string, OcppProtocol> _protocols = new();
        private static readonly ConcurrentDictionary<string, bool> _booted = new();

        public static void MarkBooted(string chargerId)
        {
            _booted[chargerId] = true;
        }

        public static bool HasBooted(string chargerId)
        {
            return _booted.TryGetValue(chargerId, out var b) && b;
        }


        public static void Set(string chargerId, OcppProtocol protocol)
        {
            _protocols[chargerId] = protocol;
        }

        public static OcppProtocol Get(string chargerId)
        {
            return _protocols.TryGetValue(chargerId, out var p)
                ? p
                : OcppProtocol.V201; 
        }

        public static void Remove(string chargerId)
        {
            _protocols.TryRemove(chargerId, out _);
            //
            _booted.TryRemove(chargerId, out _);
            //
        }
    }
}
