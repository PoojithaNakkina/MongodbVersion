//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Text;

//namespace esyasoft.mobility.CHRGUP.service.core.Models
//{
//    [Table("c_chargerconfig", Schema = "config")]
//    public class ChargerConfig
//    {
//        [Key] public string ChargerId { get; set; }
//        [Required] public string Manufacturer {  get; set; }
//        [Required] public string FirmwareVersion { get; set; }
//        public double InputPower { get; set; }
//        public double OutputPower { get; set; }
//        public string ConnectorType { get; set; }
//        public int NoOfPorts { get; set; }
//    }
//}


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace esyasoft.mobility.CHRGUP.service.core.Models
{
    public class ChargerConfig
    {
        // MongoDB document primary key
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string ChargerId { get; set; } = null!;

        [BsonElement("manufacturer")]
        public string Manufacturer { get; set; } = null!;

        [BsonElement("firmwareVersion")]
        public string FirmwareVersion { get; set; } = null!;

        [BsonElement("inputPower")]
        public double InputPower { get; set; }

        [BsonElement("outputPower")]
        public double OutputPower { get; set; }

        [BsonElement("connectorType")]
        public string? ConnectorType { get; set; }

        [BsonElement("noOfPorts")]
        public int NoOfPorts { get; set; }
    }
}
