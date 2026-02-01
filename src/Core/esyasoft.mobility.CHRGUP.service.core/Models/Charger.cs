//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using esyasoft.mobility.CHRGUP.service.core.Metadata;

//namespace esyasoft.mobility.CHRGUP.service.core.Models
//{
//    [Table("m_charger", Schema = "master")]
//    public class Charger
//    {
//        [Key]
//        public string Id { get; set; }

//        [Required]
//        public ChargerStatus Status { get; set; }

//        [Required]
//        public DateTime LastSeen { get; set; }

//        [Required]
//        [ForeignKey(nameof(Location))]
//        public string LocationId { get; set; }

//        public Location Location { get; set; }

//    }

//}


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using esyasoft.mobility.CHRGUP.service.core.Metadata;

namespace esyasoft.mobility.CHRGUP.service.core.Models
{
    public class Charger
    {
        // 🔹 MongoDB primary key (_id)
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = null!;

        // Stored as string (good for debugging & events)
        [BsonRepresentation(BsonType.String)]
        public ChargerStatus Status { get; set; }

        public DateTime LastSeen { get; set; }

        // 🔹 Reference by ID only (NO navigation object)
        public string LocationId { get; set; } = null!;
    }
}

