using System;

namespace DO
{
    /// <summary>
    /// make a struct to represent the drones  with the charachteristics:id, model, maxweight ,status and battery
    /// </summary>
    public struct Drone
    {
        public int ID { get; set; }
        public string Model { get; set; }
        public WeightCategories Weight { get; set; }

        /// <summary>
        /// a function to allow the print of a variable of drone type
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $"Drone's Id: {ID}\n";
            result += $"Model: {Model}\n";
            result += $"Max Weight: {Weight}\n";
            return result;//return all the data as a one string variable
        }
    }
}