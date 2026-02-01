namespace esyasoft.mobility.CHRGUP.service.ocpp.State
{
    public static class ChargerStateStore
    {
        private static readonly Dictionary<string, ChargerState> _states = new();

        public static ChargerState Get(string chargePointId)
        {
            if (!_states.ContainsKey(chargePointId))  
            {
                _states[chargePointId] = new ChargerState
                {
                    ChargePointId = chargePointId  
                };
            }

            return _states[chargePointId];
        }
    }

}
