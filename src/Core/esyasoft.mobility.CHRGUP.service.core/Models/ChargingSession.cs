//using esyasoft.mobility.CHRGUP.service.core.Metadata;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace esyasoft.mobility.CHRGUP.service.core.Models
//{
//    [Table("t_chargingSession", Schema = "transactions")]
//    public class ChargingSession
//    {
//        [Key]
//        public string Id { get; set; }

//        [Required]
//        [ForeignKey(nameof(Charger))]
//        public string ChargerId { get; set; }
//        public Charger Charger { get; set; }

//        [Required]
//        [ForeignKey(nameof(Driver))]
//        public string DriverId { get; set; }
//        public Driver Driver { get; set; }

//        [Required]
//        public DateTime StartTime { get; set; }

//        public DateTime? EndTime { get; set; }

//        public double InitialCharge { get; set; }

//        public double SOC { get; set; }

//        public decimal? EnergyConsumedKwh { get; set; }

//        [Required]
//        public SessionStatus Status { get; set; }

//        public DateTime LastMeterUpdate { get; set; }
//    }
//}


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using esyasoft.mobility.CHRGUP.service.core.Metadata;

namespace esyasoft.mobility.CHRGUP.service.core.Models
{
    public class ChargingSession
    {
        // 🔹 MongoDB primary key
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = null!;

        // 🔹 Reference IDs only (no navigation objects)
        public string ChargerId { get; set; } = null!;
        public string DriverId { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        // SOC / charging data
        public double InitialCharge { get; set; }
        public double SOC { get; set; }
        public decimal? EnergyConsumedKwh { get; set; }

        // Enum stored as string (human readable)
        [BsonRepresentation(BsonType.String)]
        public SessionStatus Status { get; set; }

        public DateTime? LastMeterUpdate { get; set; }
    }
}
