using System;
using System.Collections;
namespace DO
{
    /// <summary>
    /// each station is represented by a struct which contain all the necessary information such as: Name of the Station, the exact localisation ect...
    /// </summary>
    public struct Station
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int ChargeSlots { get; set; } //free charge

        /// <summary>
        /// the function allow to print all the details of each station
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $"station: {ID}\n";
            result += $"Name: {Name}\n";
            result += $"Location:{Tools.LatitudeBase60(Latitude)} {Tools.LongitudeBase60(Longitude)} \n";
            result += $"ChargeSlots: {ChargeSlots}\n";
            return result;
        }
    }
}