namespace esyasoft.mobility.CHRGUP.service.api.DTOs.Vehicle
{
    public class CreateVehicleDto
    {
        //public string VehicleName { get; set; }
        //public string Make { get; set; }
        //public string Model { get; set; }
        //public string Variant { get; set; }
        //public string RegistrationNumber { get; set; }
        //public string VIN { get; set; }

        //public int? RangeKm { get; set; }
        //public double BatteryCapacityKwh { get; set; }
        //public double MaxChargeRateKw { get; set; }

        public string VehicleName { get; set; }
        public string MakeandModel { get; set; }
        public string RegistrationNumber { get; set; }
        public string VIN { get; set; }
        public int? RangeKm { get; set; }
        public double BatteryCapacityKwh { get; set; }
        public double MaxChargeRateKw { get; set; }

    }
}
