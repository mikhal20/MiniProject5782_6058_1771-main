using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// class of drone for list:every DroneForList has these fields
    /// </summary>
    public class DroneForList
    {
        public int ID { get; set; }
        public string Model { get; set; }
        public WeightCategories Weight { get; set; }
        public double BatteryLevel { get; set; }
        public DroneStatuses DroneStatus { get; set; }
        public Location Location { get; set; }
        public int ParcelID { get; set; }
        public DateTime ChargingTime { get; set; }

        /// <summary>
        /// allow to print a variable of DroneForList Type
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $"Drone's Id: {ID}\n";
            result += $"Model: {Model}\n";
            result += $"Weight: {Weight}\n";
            result += $"Battery level: {String.Format("{0:0.00}", BatteryLevel)}\n";
            result += $"Drone's Status: {DroneStatus}\n";
            result += $"Location: {Location}\n";
            result += $"parcel's Id: {ParcelID}\n";
            return result;
        }
    }
}
