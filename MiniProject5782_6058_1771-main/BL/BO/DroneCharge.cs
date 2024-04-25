using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{/// <summary>
/// class of drone in Charge: every DroneCharge has these fields
/// </summary>
    public class DroneCharge
    {
        public int ID { get; set; }
        public double BatteryLevel { get; set; }

        /// <summary>
        /// allow to print a variable of DroneCharge Type
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $"Id: {ID}\n";
            result += $"Battery Level: {String.Format("{0:0.00}", BatteryLevel)}\n";
            return result;
        }
    }
}