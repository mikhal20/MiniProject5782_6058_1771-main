using Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using DO;
using DalApi;
using System.Runtime.CompilerServices;

namespace Dal
{
    // the thread is locked on a shared object and checks whether an instance has been created or not.
    // It takes care of the memory barrier issue and ensures that only one thread will create an instance.
    // For example: Since only one thread can be in that part of the code at a time, by the time the second thread enters it,
    // the first thread will have created the instance, so the expression will evaluate as false.
    internal sealed class DalObject : IDal //singelton thread safety
    {
        internal static readonly Lazy<DalObject> singleInstance = new Lazy<DalObject>(() => new DalObject());

        static DalObject()
        {

        }
        public static IDal Instance
        {
            get
            {
                return singleInstance.Value;
            }
        }

        internal DalObject() //constructor
        {
            DataSource.Initialize();
        }

        /// <summary>
        /// find and return the Client by the id
        /// </summary>
        /// <param name="ClientID">id of the client</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]

        public Client GetClient(int ClientID)
        {
            Client ClientReturn = default;
            if (!DataSource.clients.Exists(client => client.ID == ClientID))
            {
                throw new DoesNotExist($"id {ClientID} doesn't exist");
            }
            ClientReturn = DataSource.clients.Find(c => c.ID == ClientID);
            return ClientReturn;
        }

        /// <summary>
        /// find and return the dronecharge by id
        /// </summary>
        /// <param name="DroneChargeId">id of the charging drone</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]

        public DroneCharge GetDroneCharge(int DroneChargeId)
        {
            DroneCharge DroneChargeReturn = default;
            if (!DataSource.droneCharges.Exists(dr => dr.DroneId == DroneChargeId))
            {
                throw new DoesNotExist($"id {DroneChargeId} doesn't exist");
            }
            DroneChargeReturn = DataSource.droneCharges.Find(d => d.DroneId == DroneChargeId);
            return DroneChargeReturn;
        }

        /// <summary>
        /// find and return the drone by id
        /// </summary>
        /// <param name="DronetID">id of the drone</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]

        public Drone GetDrone(int DronetID)
        {
            Drone DroneReturn = default;
            if (!DataSource.drones.Exists(drone => drone.ID == DronetID))
            {
                throw new DoesNotExist($"id {DronetID} doesn't exist");
            }
            DroneReturn = DataSource.drones.Find(d => d.ID == DronetID);
            return DroneReturn;
        }

        /// <summary>
        /// find and return a parcel by id
        /// </summary>
        /// <param name="ParcelID">id of the parcel</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetParcel(int ParcelID)
        {
            Parcel ParcelReturn = default;
            if (!DataSource.parcels.Exists(parcel => parcel.ID == ParcelID))
            {
                throw new DoesNotExist($"id {ParcelID} doesn't exist");
            }
            ParcelReturn = DataSource.parcels.Find(p => p.ID == ParcelID);
            return ParcelReturn;
        }

        /// <summary>
        /// find and return Station bu id
        /// </summary>
        /// <param name="StationID">id of the station</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]

        public Station GetStation(int StationID)
        {
            Station StationReturn = default;
            if (!DataSource.stations.Exists(station => station.ID == StationID))
            {
                throw new DoesNotExist($"id {StationID} doesn't exist");
            }
            StationReturn = DataSource.stations.Find(s => s.ID == StationID);
            return StationReturn;
        }

        /// <summary>
        /// the function assign a parcel to a drone
        /// </summary>
        /// <param name="parcelId"> the id of the parcel </param>
        /// <param name="droneId"> the id of the drone </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Assign(Parcel p, Drone d)
        {
            p.DroneId = d.ID;
            int index = DataSource.parcels.FindIndex(item => item.ID == p.ID);
            DataSource.parcels[index] = p;
        }

        /// <summary>
        /// the function send a drone to pickeup a parcel
        /// </summary>
        /// <param name="parcelId"> id of the parcel</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickUp(Parcel p)
        {
            p.PickedUp = DateTime.Now;
            int index = DataSource.parcels.FindIndex(item => item.ID == p.ID);
            DataSource.parcels[index] = p;
        }

        /// <summary>
        /// deliver a parcel to the client
        /// </summary>
        /// <param name="parcelId"> the id of the parcel </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void parcelDelivery(Parcel p)
        {
            p.Delivered = DateTime.Now;
            int index = DataSource.parcels.FindIndex(item => item.ID == p.ID);
            DataSource.parcels[index] = p;

        }

        /// <summary>
        /// send the drone to charge in a station
        /// </summary>
        /// <param name="stationId"> the id of the station </param>
        /// <param name="droneId"> the id of the drone </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void sendToCharge(Station s, Drone d)
        {
            s.ChargeSlots--;
            int index = DataSource.stations.FindIndex(item => item.ID == s.ID);
            DataSource.stations[index] = s;
        }

        /// <summary>
        /// free a drone from a charge slot
        /// </summary>
        /// <param name="stationId"> the id of the station </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void freeCharge(Station s)
        {
            s.ChargeSlots++;
            int index = DataSource.stations.FindIndex(item => item.ID == s.ID);
            DataSource.stations[index] = s;
        }

        /// <summary>
        /// print a unique object chosen by the user
        /// </summary>
        /// <param name="id"> the id of the chosen object (could be station, drone, client or parcel) </param>
        /// <param name="num"> the object </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void showOptions(int id, int num)
        {
            switch (num)
            {
                case 1:
                    Station tempS = GetStation(id);
                    Console.WriteLine(tempS); //print the station of the id entered
                    break;
                case 2:
                    Drone tempD = GetDrone(id);
                    Console.WriteLine(tempD); //print the drone of the id entered
                    break;
                case 3:
                    Client tempC = GetClient(id);
                    Console.WriteLine(tempC); //print the client of the id entered
                    break;
                case 4:
                    Parcel tempP = GetParcel(id);
                    Console.WriteLine(tempP); //print the parcel of the id entered
                    break;
            }
        }

        /// <summary>
        /// returns the list of stations (DAL)
        /// </summary>
        /// <param name="predicate">myDal.showStation(x => x.ChargeSlots != 0)</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> showStations(Func<Station, bool> predicate = null)
        {
            if (predicate == null)
                return DataSource.stations.ToList();
            return DataSource.stations.Where(predicate).ToList();
            //cal: myDal.showStation(x => x.ChargeSlots != 0);
        }

        /// <summary>
        /// returns the list of drones (DAL)
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Drone> showDrones()
        {
            return DataSource.drones.ToList();
        }

        /// <summary>
        /// returns the list of clients (DAL)
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Client> showClients()
        {
            return DataSource.clients.ToList();
        }

        /// <summary>
        /// returns the list of parcels (DAL)
        /// </summary>
        /// <param name="predicate">myDal.showParcel(x => x.droneId == 0)</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Parcel> showParcels(Func<Parcel, bool> predicate = null)
        {
            if (predicate == null)
                return DataSource.parcels.ToList();
            return DataSource.parcels.Where(predicate).ToList();
        }

        /// <summary>
        /// returns the list of drone charhes (DAL)
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneCharge> showDroneCharges()
        {
            return DataSource.droneCharges.ToList();
        }

        /// <summary>
        /// the function add a station entered to the list of stations
        /// </summary>
        /// <param name="id"> id of the station </param>
        /// <param name="name"> name of the station </param>
        /// <param name="longi"> longitude of the station </param>
        /// <param name="lati"> latitude of the station </param>
        /// <param name="charge"> number of free charge slots</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void addStation(Station s)
        {
            if (DataSource.stations.Exists(station => station.ID == s.ID))
            {
                throw new AlreadyExist($"id {s.ID} already exists");
            }
            DataSource.stations.Add(s);
        }

        /// <summary>
        /// the function add a drone entered to the list of drones
        /// </summary>
        /// <param name="droneId"> the id of the drone  </param>
        /// <param name="model"> the model of the drone </param>
        /// <param name="weight"> the weight of the drone </param>
        /// <param name="battery"> the type of baterry of the drone </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void addDrone(Drone d)
        {
            if (DataSource.drones.Exists(drone => drone.ID == d.ID))
            {
                throw new AlreadyExist($"id {d.ID} already exists");
            }
            DataSource.drones.Add(d);
        }

        /// <summary>
        /// the function add a client to the list of clients
        /// </summary>
        /// <param name="id"> id of the client </param>
        /// <param name="name"> name of the client </param>
        /// <param name="phone"> phone number of the client </param>
        /// <param name="longi"> longitude of the client location </param>
        /// <param name="lati"> latitude of the client location </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void addClient(Client c) //do everything like this
        {
            if (DataSource.clients.Exists(client => client.ID == c.ID))
            {
                throw new AlreadyExist($"id {c.ID} already exists");
            }
            DataSource.clients.Add(c);
        }

        /// <summary>
        /// the function add a parcel to the list of parcels
        /// </summary>
        /// <param name="senderId"> id of the sender client </param>
        /// <param name="targetId"> id of the receiving client </param> 
        /// <param name="weight"> the weight of the parcel </param>
        /// <param name="priority"> the type of priority of the parcel </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void addParcel(Parcel p)
        {
            if (DataSource.parcels.Exists(parcel => parcel.ID == p.ID))
            {
                throw new AlreadyExist($"id {p.ID} already exists");
            }
            p.ID = DataSource.Config.NumberId;
            UpdateParcel(p);
            DataSource.parcels.Add(p);
            DataSource.Config.NumberId++;
        }
        /// <summary>
        /// the function add a Drone Charge to the list of drones Charge
        /// </summary>
        /// <param name="dr"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void addDroneCharge(DroneCharge dr)
        {
            if (DataSource.droneCharges.Exists(DroneCharge => DroneCharge.DroneId == dr.DroneId))
                throw new AlreadyExist($"id {dr.DroneId} already exists");
            else
                DataSource.droneCharges.Add(dr);
        }

        /// <summary>
        /// delete the client entered from the list of clients
        /// </summary>
        /// <param name="c">the client to remove</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void clearClient(Client c)
        {
            bool flag = false;
            foreach (var item in DataSource.clients)
            {
                if (item.ID == c.ID)
                    flag = true;
            }
            if (!flag)
                throw new DoesNotExist($"id {c.ID} does not exists");
            Client temp = default;
            int index = DataSource.clients.FindIndex(cl => cl.ID == c.ID);
            DataSource.clients[index] = temp;
        }

        /// <summary>
        /// delete the drone entered from the list of drones
        /// </summary>
        /// <param name="d">the drone to remove</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void clearDrone(Drone d)
        {
            bool flag = false;
            foreach (var item in DataSource.drones)
            {
                if (item.ID == d.ID)
                    flag = true;
            }
            if (!flag)
                throw new DoesNotExist($"id {d.ID} does not exists");
            Drone temp = default;
            int index = DataSource.drones.FindIndex(dr => dr.ID == d.ID);
            DataSource.drones[index] = temp;
        }

        /// <summary>
        /// delete the parcel entered from the list of parcels
        /// </summary>
        /// <param name="p">is the parcel to remove</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void clearParcel(Parcel p)
        {
            bool flag = false;
            foreach (var item in DataSource.parcels)
            {
                if (item.ID == p.ID)
                    flag = true;
            }
            if (!flag)
                throw new DoesNotExist($"id {p.ID} does not exists");
            Parcel temp = default;
            int index = DataSource.parcels.FindIndex(pr => pr.ID == p.ID);
            DataSource.parcels[index] = temp;
        }

        /// <summary>
        /// delete the station entered from the list of stations
        /// </summary>
        /// <param name="s"> the station to remove </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void clearStation(Station s)
        {
            bool flag = false;
            foreach (var item in DataSource.stations)
            {
                if (item.ID == s.ID)
                    flag = true;
            }
            if (!flag)
                throw new DoesNotExist($"id {s.ID} does not exists");
            Station temp = default;
            int index = DataSource.stations.FindIndex(st => st.ID == s.ID);
            DataSource.stations[index] = temp;
        }

        /// <summary>
        /// delete the droneCharge entered from the list of dronesCharge
        /// </summary>
        /// <param name="dr">the drone charge to remove</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void clearDroneCharge(DroneCharge dr)
        {
            if (!DataSource.droneCharges.Exists(d => d.DroneId == dr.DroneId))
                throw new DoesNotExist($"id {dr.DroneId} does not exists");
            DroneCharge drone = default;
            int index = DataSource.droneCharges.FindIndex(d => d.DroneId == dr.DroneId);
            DataSource.droneCharges[index] = drone;
            // _ = DataSource.droneCharges.Remove(dr);
        }

        /// <summary>
        /// returns the data of the Config in DataSource
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double[] DroneElectricity()
        {
            double[] arr = new double[5]
            {
                DataSource.Config.Available,
                DataSource.Config.LowWeight,
                DataSource.Config.MiddleWeight,
                DataSource.Config.HightWeight,
                DataSource.Config.chargePerHour
            };
            return arr;
        }

        /// <summary>
        /// update a station Name and or number of chargeslots
        /// </summary>
        /// <param name="tempS">the station to update</param>
        /// <param name="name">the new name for the station to update</param>
        /// <param name="num">the new number of chargeSlots for the station to update</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStationName(Station tempS, string name, string num)
        {
            int index = DataSource.stations.FindIndex(s => s.ID == tempS.ID);
            Station s = DataSource.stations[index];
            if (name == "Enter Name")
                name = "";
            if (name != "")
                s.Name = name;
            if (num == "Enter ChargeSlots")
                num = "";
            if (num != "")
            {
                int count = 0;
                foreach (var item in DataSource.droneCharges)
                {
                    if (item.StationId == tempS.ID)
                        count++;
                }
                int n = Int32.Parse(num);
                s.ChargeSlots = n - count;
            }
            DataSource.stations[index] = s;
        }

        /// <summary>
        /// update a Client Name and/or his phone number
        /// </summary>
        /// <param name="id">the id of the Client</param>
        /// <param name="name">the Name of the client to update</param>
        /// <param name="phone">the phone number of the client to update</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateClientName(int id, string name, string phone)
        {
            int index = DataSource.clients.FindIndex(c => c.ID == id);
            Client c = DataSource.clients[index];
            if (name != "")
                c.Name = name;
            if (phone != "")
                c.Phone = phone;
            DataSource.clients[index] = c;
        }

        /// <summary>
        /// update the drone
        /// </summary>
        /// <param name="d">the drone to update</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDrone(Drone d) //update the drone in the list of drones
        {
            int index = DataSource.drones.FindIndex(dr => dr.ID == d.ID);
            DataSource.drones[index] = d;
        }

        /// <summary>
        /// update the parcel
        /// </summary>
        /// <param name="p">the parcel to update</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateParcel(Parcel p) //update the parcel in the list of parcels
        {
            int index = DataSource.parcels.FindIndex(pr => pr.ID == p.ID);
            
            DataSource.parcels[index] = p;
        }

        /// <summary>
        /// update a station entered
        /// </summary>
        /// <param name="s"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStation(Station s) //update the station in the list of stations
        {
            int index = DataSource.stations.FindIndex(st => st.ID == s.ID);
            DataSource.stations[index] = s;
        }
    }
}
