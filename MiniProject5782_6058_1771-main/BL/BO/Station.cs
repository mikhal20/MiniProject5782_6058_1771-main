using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }
        public int ChargeSlots { get; set; }
        public List<DroneCharge> ListDroneCharge { get; set; }

        public override string ToString()
        {
            string result = "";
            result += $"station: {Id}\n";
            result += $"Name: {Name}\n";
            result += $"{Location}\n";
            result += $"ChargeSlots: {ChargeSlots}\n";
            return result;
        }
    }
}
