//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace esyasoft.mobility.CHRGUP.service.core.Models
//{
//    [Table("t_fault", Schema = "transactions")]
//    public class Fault
//    {
//        [Key] public string Id { get; set; }
//        [Required][ForeignKey("Charger")] public string ChargerId { get; set; }
//        public Charger Charger { get; set; }
//        [Required] public string FaultCode { get; set; }
//        public DateTime Timestamp { get; set; }
//    }
//}


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace esyasoft.mobility.CHRGUP.service.core.Models
{
    public class Fault
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = null!;

        public string ChargerId { get; set; } = null!;
        public string FaultCode { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
}
