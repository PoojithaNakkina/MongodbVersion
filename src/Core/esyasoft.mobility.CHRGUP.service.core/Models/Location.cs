//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace esyasoft.mobility.CHRGUP.service.core.Models
//{
//    [Table("m_location", Schema = "master")]
//    public class Location
//    {
//        [Key]
//        public string Id { get; set; }

//        [Required]
//        public string Name { get; set; }

//        public string Address { get; set; }

//        public decimal Latitude { get; set; }

//        public decimal Longitude { get; set; }
//    }
//}


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace esyasoft.mobility.CHRGUP.service.core.Models
{
    public class Location
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string? Address { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
