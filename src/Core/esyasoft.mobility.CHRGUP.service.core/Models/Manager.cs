//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace esyasoft.mobility.CHRGUP.service.core.Models
//{
//    [Table("s_manager", Schema = "sso")]
//    public class Manager
//    {
//        [Key] public string Id { get; set; }
//        [Required] public string Username { get; set; }
//        [Required] public string Email { get; set; }
//        [Required] public string Password { get; set; }
//        [Required] public string Status { get; set; }
//        public string Company { get; set; }
//        public string Department { get; set; }
//        public DateTime CreatedAt { get; set; }
//        public DateTime? UpdatedAt { get; set; }
//        public DateTime? LastActiveAt { get; set; }
//    }
//}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace esyasoft.mobility.CHRGUP.service.core.Models
{
    public class Manager
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = null!;

        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Status { get; set; } = null!;

        public string? Company { get; set; }
        public string? Department { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastActiveAt { get; set; }
    }
}
