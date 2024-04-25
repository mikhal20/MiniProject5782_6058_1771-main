using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Parcel
    {
        public int Id { get; set; } 
        public CustomerParcel Sender { get; set; }
        public CustomerParcel Recipient { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public DroneParcel DroneInParcel { get; set; }
        public DateTime? Requested { get; set; } //יצירה
        public DateTime? Scheduled { get; set; } //שיוך
        public DateTime? PickedUp { get; set; } //זמן איסוף
        public DateTime? Delivered { get; set; } //אספקה

       /// <summary>
       /// print parcel's details
       /// </summary>
       /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $"Parcel's Id: {Id}\n";
            result += $"Sender: {Sender}\n";
            result += $"Recipient: {Recipient}\n";
            result += $"Weight: {Weight}\n";
            result += $"Priority: {Priority}\n";
            result += $"Requested time: {Requested}\n";
            result += $"Scheduled time: {Scheduled}\n";
            result += $"PickedUp time: {PickedUp}\n";
            result += $"Delivered time: {Delivered}\n";
            return result;
        }

        public static implicit operator Parcel(DO.Parcel v)
        {
            throw new NotImplementedException();
        }
    }
}
