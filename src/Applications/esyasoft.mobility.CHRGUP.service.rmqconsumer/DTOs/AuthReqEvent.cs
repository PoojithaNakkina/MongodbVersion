using System;
using System.Collections.Generic;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.rmqconsumer.DTOs
{
    public class AuthReqEvent
    {
        public string ChargerId {  get; set; }
        public string Token {  get; set; }
        public string TokenType {  get; set; }
        public string MessageId { get; set; }
    }
}
