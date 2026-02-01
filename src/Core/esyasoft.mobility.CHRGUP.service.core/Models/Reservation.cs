//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Text;

//namespace esyasoft.mobility.CHRGUP.service.core.Models
//{
//    [Table("t_reservation", Schema = "transactions")]
//    public class Reservation
//    {
//        [Key]
//        public string Id { get; set; } = Guid.NewGuid().ToString();

//        [Required]
//        public string ChargerId { get; set; }

//        [Required]
//        public string DriverId { get; set; }

//        public int ConnectorId { get; set; }

//        [Required]
//        public string Status { get; set; }

//        [Required]
//        public string CreatedBy { get; set; }

//        public string? CancelledBy { get; set; }

//        public DateTime StartTime { get; set; }
//        public DateTime? EndTime { get; set; }

//        public DateTime CreatedAt { get; set; } = DateTime.Now;
//    }
//}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace esyasoft.mobility.CHRGUP.service.core.Models
{
    public class Reservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string ChargerId { get; set; } = null!;
        public string DriverId { get; set; } = null!;
        public int ConnectorId { get; set; }

        public string Status { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public string? CancelledBy { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
