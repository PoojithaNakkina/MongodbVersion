using System;
using System.Collections.Generic;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs
{
    public class SessionStartEvent
    {
        public string SessionId { get; set; }
        public string ChargerId { get; set; }
        public string UserId { get; set; }
        public DateTime StartTime { get; set; }
        public double SOC { get; set; }
    }
}
