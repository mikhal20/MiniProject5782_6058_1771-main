using System;

namespace DO
{
    /// <summary>
    /// make a struct to represent the droneCharges  with the charachteristics:droneid, stationid
    /// </summary>
    public struct DroneCharge
    {
        public int DroneId { get; set; }
        public int StationId { get; set; }

        /// <summary>
        /// a function to allow printing a variable of droneCharge type
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = " ";
            result += $"Drone's Id: {DroneId}\n";
            result += $"Station's Id: {StationId}\n";
            return result;//return all the data as a one string variable
        }
    }
}
