//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using esyasoft.mobility.CHRGUP.service.core.Metadata;


//namespace esyasoft.mobility.CHRGUP.service.core.Models
//{
//    [Table("s_driver", Schema = "sso")]
//    public class Driver
//    {
//        [Key] public string Id { get; set; }
//        [Required] public string FullName { get; set; }
//        [Required] public string Email { get; set; }
//        [Required] public string Password { get; set; }
//        [Required] public string RfidTag { get; set; }
//        public string? Gender { get; set; }
//        public DateTime? DateOfBirth { get; set; }
//        [Required] public DriverStatus Status { get; set; }
//        public DateTime CreatedAt { get; set; }
//        public DateTime? UpdatedAt { get; set; }
//        public DateTime? LastActiveAt { get; set; }
//        [ForeignKey("Vehicle")] public string? VehicleId { get; set; }
//        public Vehicle Vehicle { get; set; }
//    }
//}


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using esyasoft.mobility.CHRGUP.service.core.Metadata;

namespace esyasoft.mobility.CHRGUP.service.core.Models
{
    public class Driver
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = null!;

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string RfidTag { get; set; } = null!;

        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DriverStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastActiveAt { get; set; }

        // Reference only
        public string? VehicleId { get; set; }
    }
}
