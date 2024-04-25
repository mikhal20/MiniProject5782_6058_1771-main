using DO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dal
{
   internal static class DataSource
    {
        /// <summary>
        /// creating internal static list for each of the structs
        /// </summary>
        internal static List<Drone> drones = new List<Drone>();
        internal static List<Parcel> parcels = new List<Parcel>();
        internal static List<Station> stations = new List<Station>();
        internal static List<Client> clients = new List<Client>();
        internal static List<DroneCharge> droneCharges = new List<DroneCharge>();

        internal class Config
        {
            internal static int NumberId = 10000000; //running number to put in the id of the clients
            internal static double Available { get => 1; }
            internal static double LowWeight { get => 2; }
            internal static double MiddleWeight { get => 3; }
            internal static double HightWeight { get => 4; }
            internal static double chargePerHour { get => 5; } //of the drone
        }

        internal static Random r = new Random(); //a variable to the random of data

        internal static string RandomString(int length) //gets lenght and return a random word of this lenght
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";//random a word from those letters
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[r.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// function to initialise data in the program at the beggining of the debuging
        /// </summary>
        public static void Initialize() //call to 4 functions that initialize data
        {
            CreateDrone(); //initialize drone
            CreateStation(); //initialize station
            CreateClient(); //initialize client
            CreateParcel(); //initialize parcel
        }

        /// <summary>
        /// initialize 5 drones and add each of them in the drones list
        /// </summary>
        private static void CreateDrone()
        {
            for (int i = 0; i < 5; i++) //create 5 drones
            {
                drones.Add(new Drone //add the drones in the drones list
                {
                    ID = r.Next(1111111, 9999999), //random a 7 digits number
                    Model = RandomString(7), //random string
                    Weight = (WeightCategories)r.Next(0, 3), //random from the enum of weightcategories
                });
            }
        }

        /// <summary>
        /// initialize 10 parcels and add each of them in the parcels list
        /// </summary>
        private static void CreateParcel()
        {
            for (int i = 0; i < 10; i++)
            {
                int numSender = r.Next(0, 10); //random client out of 10 clients
                int numTarget = r.Next(0, 10); //random client out of 10 clients
                while (numTarget == numSender)
                {
                    numTarget = r.Next(0, 10);
                }
                parcels.Add(new Parcel //add the parcels to the list of parcels
                {
                    ID = Config.NumberId, //the running number that we initialized in "config" to 1000000
                    SenderId = clients[numSender].ID,
                    TargetId = clients[numTarget].ID,
                    DroneId = r.Next(111111, 9999999),///random number for the drone's id
                    Weight = (WeightCategories)r.Next(0, 3), //random from enum
                    Priority = (Priorities)r.Next(0, 3), //random from enum
                                                         //initialize the times with the actual time:
                    Requested = DateTime.Now,
                    Scheduled = null,
                    PickedUp = null,
                    Delivered = null,
                });
                Config.NumberId++;//each time numberId grows from 1 to give another id for each parcel
            }
        }

        /// <summary>
        /// function to initialize stations and add them to the stations list
        /// </summary>
        private static void CreateStation()
        {
            stations.Add(new Station //add the station to the list of stations
            {
                ID = r.Next(1111111, 8888888),//random number of 7 digits
                Name = "Central Station",
                Latitude = 31.788588,
                Longitude = 35.202459,
                ChargeSlots = r.Next(5, 80) //random number between 5 and 80
            });
            stations.Add(new Station //add a second station
            {
                ID = r.Next(1111111, 8888888),
                Name = "Malcha Mall",
                Latitude = 31.750822,
                Longitude = 35.186824,
                ChargeSlots = r.Next(5, 80)
            });
        }

        /// <summary>
        /// add 10 initialized clients to the clients list
        /// </summary>
        private static void CreateClient()
        {
            clients.Add(new Client
            {
                ID = r.Next(100000000, 999999999),//random number of 9 digits
                Name = " Mikhal Levy ",
                Phone = $"0{r.Next(50, 58)}{r.Next(1000000, 10000000)}",//begins in "0"+random number between 50-58 + 7 random digits
                Latitude = 31.769959, //bayt vegan
                Longitude = 35.184812,
            });

            clients.Add(new Client
            {
                ID = r.Next(100000000, 999999999),
                Name = " Shaili Benloulou ",
                Phone = $"0{r.Next(50, 58)}{r.Next(1000000, 10000000)}",
                Latitude = 31.784591, //ar noff
                Longitude = 35.173452,
            });

            clients.Add(new Client
            {
                ID = r.Next(100000000, 999999999),
                Name = " Talia Azoulay",
                Phone = $"0{r.Next(50, 58)}{r.Next(1000000, 10000000)}",
                Latitude = 31.730643, //gilo
                Longitude = 35.184062,
            });

            clients.Add(new Client
            {
                ID = r.Next(100000000, 999999999),
                Name = " Moti Cohen ",
                Phone = $"0{r.Next(50, 58)}{r.Next(1000000, 10000000)}",
                Latitude = 31.763177, //givat mordechai
                Longitude = 35.196806,
            });

            clients.Add(new Client
            {
                ID = r.Next(100000000, 999999999),
                Name = "Yoel Ivgi ",
                Phone = $"0{r.Next(50, 58)}{r.Next(1000000, 10000000)}",
                Latitude = 31.809853, //Ramot
                Longitude = 35.196550,
            });

            clients.Add(new Client
            {
                ID = r.Next(100000000, 999999999),
                Name = "Reouven Bensimon",
                Phone = $"0{r.Next(50, 58)}{r.Next(1000000, 10000000)}",
                Latitude = 31.762155, //Kiryat HaYovel
                Longitude = 35.175401,
            });
            clients.Add(new Client
            {
                ID = r.Next(100000000, 999999999),
                Name = "Eliezer Daby",
                Phone = $"0{r.Next(50, 58)}{r.Next(1000000, 10000000)}",
                Latitude = 31.752997, //moshav Ora
                Longitude = 35.149562,
            });

            clients.Add(new Client
            {
                ID = r.Next(100000000, 999999999),
                Name = "Chyrel Barouh",
                Phone = $"0{r.Next(50, 58)}{r.Next(1000000, 10000000)}",
                Latitude = 31.752980, //Talpiot
                Longitude = 35.221041,
            });

            clients.Add(new Client
            {
                ID = r.Next(100000000, 999999999),
                Name = "David Dayan",
                Phone = $"0{r.Next(50, 58)}{r.Next(1000000, 10000000)}",
                Latitude = 31.781257, //gan saker
                Longitude = 35.206477,
            });

            clients.Add(new Client
            {
                ID = r.Next(100000000, 999999999),
                Name = "Yossef Amar",
                Phone = $"0{r.Next(50, 58)}{r.Next(1000000, 10000000)}",
                Latitude = 31.774441,//Rehavia
                Longitude = 35.213439,
            });
        }
    }
}