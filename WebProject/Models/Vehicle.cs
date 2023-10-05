using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string VehicleType { get; set; }
        public string VehicleBrand { get; set; }
        public int YearOfProduction { get; set; }
        public int TopSpeed { get; set; }
        public int VehicleMileage { get; set; }
        public string VehicleOwner { get; set; }

        public List<VehicleServiceHistory> VehicleServiceHistory { get; set; }
    }

}
