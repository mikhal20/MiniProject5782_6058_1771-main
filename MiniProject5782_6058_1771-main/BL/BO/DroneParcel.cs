using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    ///class of drone Parcels: every droneParcel has these fields
    /// </summary>
    public class DroneParcel //רחפן בחבילה
    {
        public int Id { get; set; }
        public double BatteryLevel { get; set; }
        public Location Location { get; set; }

        /// <summary>
        /// allow to print a variable of droneParcel Type
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $"Id: {Id}\n";
            result += $"Battery Level: {String.Format("{0:0.00}", BatteryLevel)}\n";
            result += $"Location: {Location}\n";
            return result;
        }
    }
}
