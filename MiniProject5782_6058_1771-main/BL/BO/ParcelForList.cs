using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class ParcelForList
    {
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string RecipientName { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public Status Status { get; set; }


        public override string ToString()
        {
            string result = "";
            result += $"Parcel's Id: {Id}\n";
            result += $"Sender: {SenderName}\n";
            result += $"Recipient: {RecipientName}\n";
            result += $"Weight: {Weight}\n";
            result += $"Priority: {Priority}\n";
            result += $"Status: {Status}\n";
            return result;
        }
    }
}
