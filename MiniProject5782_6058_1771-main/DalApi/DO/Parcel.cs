using System;

namespace DO
{
    /// <summary>
    /// each Parcel is represented by a struct with all the necesarry information such as: Id, Weight, DroneId ect... 
    /// </summary>
    public struct Parcel
    {
        public int ID { get; set; }
        public int SenderId { get; set; }
        public int TargetId { get; set; }
        public int DroneId { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public DateTime? Requested { get; set; }
        public DateTime? Scheduled { get; set; }
        public DateTime? PickedUp { get; set; }
        public DateTime? Delivered { get; set; }

        /// <summary>
        /// the function allow to print all the deatils of each parcel
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $"Parcel's Id: {ID}\n";
            result += $"Sender's Id: {SenderId}\n";
            result += $"Target's Id: {TargetId}\n";
            result += $"Drone's Id: {DroneId}\n";
            result += $"Weight: {Weight}\n";
            result += $"Priority: {Priority}\n";
            result += $"Requested time: {Requested}\n";
            result += $"Scheduled time: {Scheduled}\n";
            result += $"PickedUp time: {PickedUp}\n";
            result += $"Delivered time: {Delivered}\n";
            return result;
        }
    }
}