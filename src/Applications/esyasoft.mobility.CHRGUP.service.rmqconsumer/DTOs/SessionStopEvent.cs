using System;
using System.Collections.Generic;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs
{
    public class SessionStopEvent
    {
        public string SessionId { get; set; }
        public string ChargerId { get; set; }
        public DateTime StopTime { get; set; }
        public decimal EnergyConsumedKwh { get; set; }
        public double SOC { get; set; }
        public string triggerReason { get; set; }
    }
}
