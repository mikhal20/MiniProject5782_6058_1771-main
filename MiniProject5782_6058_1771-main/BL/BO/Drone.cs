using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// a class of drone- every drone variable has these fields
    /// </summary>
    public class Drone
    {
        public int ID { get; set; }
        public string Model { get; set; }
        public WeightCategories Weight { get; set; }
        public double BatteryLevel { get; set; }
        public DroneStatuses DroneStatus { get; set; }
        public ParcelSending ParcelSending { get; set; }
        public Location Location { get; set; }

        /// <summary>
        /// allow to print a variable of Drone Type
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $"Drone's Id: {ID}\n";
            result += $"Model: {Model}\n";
            result += $"Max Weight: {Weight}\n";
            result += $"Battery level: {String.Format("{0:0.00}", BatteryLevel)}\n";
            result += $"{Location}\n";
            return result;//return all the data as a one string variable
        }
    }
}
