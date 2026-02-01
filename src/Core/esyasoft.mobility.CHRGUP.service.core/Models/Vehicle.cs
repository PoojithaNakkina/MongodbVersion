//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace esyasoft.mobility.CHRGUP.service.core.Models
//{
//    //[Index(nameof(RegistrationNumber), IsUnique = true)]
//    //[Index(nameof(VIN), IsUnique = true)]
//    [Table("m_vehicle", Schema = "master")]
//    public class Vehicle
//    {
//        [Key] public string Id { get; set; }
//        [Required] public string VehicleName { get; set; }
//        [Required] public string VIN { get; set; }
//        public string MakeandModel { get; set; }
//        [Required] public string RegistrationNumber { get; set; }
//        public int? RangeKm { get; set; }
//        public double BatteryCapacityKwh { get; set; }
//        public double MaxChargeRateKw { get; set; }

//    }
//}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace esyasoft.mobility.CHRGUP.service.core.Models
{
    public class Vehicle
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = null!;

        public string VehicleName { get; set; } = null!;
        public string VIN { get; set; } = null!;
        public string RegistrationNumber { get; set; } = null!;

        public string? MakeandModel { get; set; }
        public int? RangeKm { get; set; }

        public double BatteryCapacityKwh { get; set; }
        public double MaxChargeRateKw { get; set; }
    }
}
