using System;
using System.Collections.Generic;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs
{
    public class ChargerFaultEvent
    {
        public string ChargerId { get; set; }
        public string FaultCode { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
