using System.Collections.Concurrent;

namespace esyasoft.mobility.CHRGUP.service.ocpp.State
{
    public static class HeartbeatStore
    {
        private static readonly ConcurrentDictionary<string, DateTime> _lastSeen
            = new();

        public static void Update(string chargerId)
        {
            _lastSeen[chargerId] = DateTime.Now;
        }

        public static bool TryGetLastSeen(string chargerId, out DateTime lastSeen)
        {
            return _lastSeen.TryGetValue(chargerId, out lastSeen);
        }

        public static IEnumerable<(string ChargerId, DateTime LastSeen)> GetAll()
        {
            foreach (var kv in _lastSeen)
                yield return (kv.Key, kv.Value);
        }

        public static void Remove(string chargerId)
        {
            _lastSeen.TryRemove(chargerId, out _);
        }
    }
}
