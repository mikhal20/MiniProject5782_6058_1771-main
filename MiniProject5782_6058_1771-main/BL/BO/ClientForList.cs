using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// a class of ClientForList-every ClientForList has all those fields
    /// </summary>
    public class ClientForList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int SentAndDelivered { get; set; }
        public int SentNotDelivered { get; set; }
        public int Received { get; set; }
        public int ParcelOnWay { get; set; }

        /// <summary>
        /// allow to print a variable of ClientForList Type
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $"Client's Id: {Id}\n";
            result += $"Name: {Name}\n";
            result += $"Phone: {Phone}\n";//
            result += $"Sent And Delivered: {SentAndDelivered}\n";
            result += $"Sent But Not Delivered: {SentNotDelivered}\n";
            result += $"Received: {Received}\n";//
            result += $"ParcelOnWay: {ParcelOnWay}\n";//
            return result;
        }
    }
}
