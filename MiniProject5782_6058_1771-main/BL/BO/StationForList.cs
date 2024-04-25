using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class StationForList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FreeChargeSlots { get; set; }
        public int OccupiedChargeSlots { get; set; }

        public override string ToString()
        {
            string result = "";
            result += $"station: {Id}\n";
            result += $"Name: {Name}\n";
            result += $"Free Charge Slots: {FreeChargeSlots}\n";
            result += $"Occupied Charge Slots: {OccupiedChargeSlots}\n";
            return result;
        }
    }
}
