using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// class of Client,every client has the fields: Id,Name,phone, location,sentParcels
    /// and receivedParcel
    /// </summary>
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Location Location { get; set; }
        public List<ParcelCustomer> SentParcels { get; set; } //from Client
        public List<ParcelCustomer> ReceiveParcels { get; set; } //to Client

        /// <summary>
        /// allow to print a variable of Client Type
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $"Client's Id: {Id}\n";
            result += $"Name: {Name}\n";
            result += $"Phone: {Phone}\n";//
            result += $"{Location}\n";
            return result; //return all the data in one string variable
        }
    }
}
