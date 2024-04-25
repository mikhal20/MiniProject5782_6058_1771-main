using System;
using System.Collections;
using System.Collections.Generic;

namespace DO
{
    /// <summary>
    /// make a struct for each client with the charachteristics:id, name, phone, coordinates:longitude and latitude
    /// </summary>
    public struct Client
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        /// <summary>
        /// function that allows printing client's details
        /// </summary>
        /// <returns>the string with all the client's details to be print</returns>
        public override string ToString()
        {
            string result = "";
            result += $"Client's Id: {ID}\n";
            result += $"Name: {Name}\n";
            result += $"Phone: {Phone.Substring(0, 3) + '-' + Phone.Substring(3)}\n";//
            result += $"Location: {Tools.LatitudeBase60(Latitude)} {Tools.LongitudeBase60(Longitude)}\n";//calls from the function in dalobject class 
            return result; //return all the data in one string variable
        }
    }
}