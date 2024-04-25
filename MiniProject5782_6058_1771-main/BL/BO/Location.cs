using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        /// <summary>
        /// print location
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $" {DO.Tools.LatitudeBase60(Latitude)} {DO.Tools.LongitudeBase60(Longitude)}";
            return result;
        }

    }
}
