using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public static class Tools
    {
        /// <summary>
        /// the function convert a double latitude entered to a base 60 latitude (a string)
        /// </summary>
        /// <param name="lat"> the latitude </param>
        /// <returns> the latitude in base 60 </returns>
        public static string LatitudeBase60(double lat)
        {
            string str;
            if (lat < 0)
            {
                str = "W";
                lat *= -1;
            }
            else
                str = "E";
            int degree = (int)lat;
            double difference = lat - degree;
            int min = (int)(difference * 60);
            double sec = (difference) * 3600 - min * 60;
            sec = Math.Round(sec, 4);
            return $"{degree}° {min}' {string.Format("{0:0000}", sec)}'' {str}";
        }
        /// <summary>
        /// the function convert the double longitude entered to a base 60 longitude (a string)
        /// </summary>
        /// <param name="longi"> the longitude </param>
        /// <returns> the longitude in base 60 </returns>
        public static string LongitudeBase60(double longi)
        {
            string str;
            if (longi < 0)
            {
                str = "S";
                longi *= -1;
            }
            else
                str = "N";
            int degree = (int)longi;
            double difference = longi - degree;
            int min = (int)(difference * 60);
            double sec = (difference) * 3600 - min * 60;
            sec = Math.Round(sec, 4);
            return $"{degree}° {min}' {string.Format("{0:0000}", sec)}'' {str}";
        }
    }
}