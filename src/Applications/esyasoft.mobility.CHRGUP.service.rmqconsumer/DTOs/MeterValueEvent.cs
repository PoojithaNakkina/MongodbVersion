using System;
using System.Collections.Generic;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs
{
    public class MeterValueEvent
    {
        public string SessionId { get; set; }
        public string ChargerId { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal EnergyKwh { get; set; }
        public double SOC { get; set; }
    }
}
