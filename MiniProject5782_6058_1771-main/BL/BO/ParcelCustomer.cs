namespace BO
{
    public class ParcelCustomer //חבילה אצל לקוח
    {
        public int Id { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public Status Status { get; set; }
        public CustomerParcel CustomerParcel { get; set; }


        /// <summary>
        /// print Parcel Customer's details
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $"Parcel's Id: {Id}\n";
            result += $"Weight: {Weight}\n";
            result += $"Priority: {Priority}\n";
            result += $"Status: {Status}\n";
            result += $"Customer Parcel: {CustomerParcel}\n";
            return result;
        }
    }
}