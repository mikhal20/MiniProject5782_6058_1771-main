using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class ParcelSending
    {
        public int Id { get; set; }
        public bool ParcelStatus { get; set; }//ממתין לאיסוף או בדרך ליעד
        public Priorities Priority { get; set; }
        public CustomerParcel Sender { get; set; }
        public CustomerParcel Recipient { get; set; }
        public Location PickLocation { get; set; }
        public Location DeliverLocation { get; set; }
        public double Distance { get; set; }

        public override string ToString()
        {
            string result = "";
            result += $"Parcel's Id: {Id}\n"; 
            result += $"Parcel Status: {ParcelStatus}\n";
            result += $"Priority: {Priority}\n";
            result += $"Sender: {Sender}\n";
            result += $"Recipient: {Recipient}\n";
            result += $"Pick up Location: {PickLocation}\n";
            result += $"Delivery Location: {DeliverLocation}\n";
            result += $"Distance: {Distance}\n";
            return result;
        }
    }
}
