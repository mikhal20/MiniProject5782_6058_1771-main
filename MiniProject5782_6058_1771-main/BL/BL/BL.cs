using System;
using BO;
using System.Collections.Generic;
using System.Threading.Tasks;
using DalApi;
using BlApi;

namespace BL
{
    internal sealed partial class BL : IBL //singelton
    {
        internal static readonly IDal myDal;
        internal static readonly Lazy<BL> singleInstance = new Lazy<BL>(() => new BL()); //lazy initialization
        public static BL SingleInstance
        {
            get
            {
                return singleInstance.Value;
            }
        }
        static BL()
        {
            myDal = DalFactory.GetDal(); //allow access to dalObject
        }
        private List<DroneForList> dronesList; //creating a list of "droneForList"
        private static Random r = new Random(); //creating a random variable
        internal BL() //constructor
        {
            dronesList = new List<DroneForList>();
            double[] ElectricityArr = new double[]
                {
                 myDal.DroneElectricity()[0],
                 myDal.DroneElectricity()[1],
                 myDal.DroneElectricity()[2],
                 myDal.DroneElectricity()[3]
               };

            double ChargePerHour = myDal.DroneElectricity()[4];

            List<DO.Drone> tempDr = (List<DO.Drone>)myDal.showDrones();
            List<DO.Parcel> tempPr = (List<DO.Parcel>)myDal.showParcels();
            List<DO.Station> tempSt = (List<DO.Station>)myDal.showStations();
            List<DO.Client> tempCl = (List<DO.Client>)myDal.showClients();

            tempDr.ForEach(d => //for each drone in the data source copy ID, model and maximum weight to the list of drones
            {
                DroneStatuses S = (DroneStatuses)r.Next(0, 2);
                int num = r.Next(0, tempSt.Count);
                DO.Station s = tempSt[num];
                if (S == DroneStatuses.Maintenance)
                {
                    DO.DroneCharge droneCharge = new DO.DroneCharge
                    {
                        DroneId = d.ID,
                        StationId=s.ID,
                    };
                    myDal.addDroneCharge(droneCharge);
                }
                dronesList.Add(new DroneForList
                {
                    ID = d.ID,
                    Model = d.Model,
                    Weight = (WeightCategories)d.Weight,
                    DroneStatus = S, //intialize status to be available or maintenance
                   
                });
            });

            //checking all the parcels to get the drones that are currently shipping parcels
            tempPr.ForEach(p => //fore each parcel in the list of parcels from data
            {
                if (p.Scheduled != null && p.Delivered == null) //if the parcel has a drone assigned but was not yet delivered
                {
                    int index = dronesList.FindIndex(d => d.ID == p.DroneId); //finding the index of the drone of the parcel in the list of drones

                    dronesList[index].DroneStatus = DroneStatuses.Shipping; //changing the status of the drone to be in shipping

                    DO.Client sender = myDal.GetClient(p.SenderId); //getting the sender of the package
                    Location senderLoc = new Location { Longitude = sender.Longitude, Latitude = sender.Latitude }; //saving the location of the sender
                    DO.Client target = myDal.GetClient(p.TargetId); //getting the sender of the package
                    Location targetLoc = new Location { Longitude = target.Longitude, Latitude = target.Latitude }; //saving the location of the sender

                    int station1 = Neareststation(senderLoc, true).Id; //getting the id of the nearest station to the sender
                    Location station1Loc = new Location //saving the location of that station
                    {
                        Latitude = myDal.GetStation(station1).Latitude,
                        Longitude = myDal.GetStation(station1).Longitude,
                    };

                    int station2 = Neareststation(targetLoc, true).Id; //getting the id of the nearest station to the target
                    Location station2Loc = new Location //saving the location of that station
                    {
                        Latitude = myDal.GetStation(station2).Latitude,
                        Longitude = myDal.GetStation(station2).Longitude,
                    };

                    //the total distance the drone needs to go for the delivery
                    double totalDIstance = GetDistance(station1Loc.Latitude, station1Loc.Longitude, senderLoc.Latitude, station1Loc.Longitude) + GetDistance(senderLoc.Latitude, senderLoc.Longitude, targetLoc.Latitude, targetLoc.Longitude) + GetDistance(senderLoc.Latitude, senderLoc.Longitude, station2Loc.Latitude, station2Loc.Longitude);

                    //the battery will be a random number between the minimum battery needed to complete the delivery (according to weight and distance), and full battery
                    if ((BO.WeightCategories)p.Weight == BO.WeightCategories.Low)
                    {
                        dronesList[index].BatteryLevel = (double)r.Next((int)(totalDIstance * ElectricityArr[1]), 100);
                    }
                    else if ((BO.WeightCategories)p.Weight == BO.WeightCategories.Middle)
                    {
                        dronesList[index].BatteryLevel = (double)r.Next((int)(totalDIstance * ElectricityArr[2]), 100);
                    }
                    else
                    {
                        dronesList[index].BatteryLevel = (double)r.Next((int)(totalDIstance * ElectricityArr[3]), 100);
                    }

                    //if the parcel hasnt been picked up by a drone, the location is that of the station closest to the sender
                    if (p.PickedUp == default)
                    {
                        dronesList[index].Location = station1Loc; //saving the location of the nearest station to the sender to be the location of the drone
                    }
                    else //if it was picked up by a drone but not yet delivered to to the target then the location of the drone is the location of the sender
                    {
                        dronesList[index].Location = senderLoc; //saving the location of the drone to be the location of the sender
                    }
                }
            });

            dronesList.ForEach(d => //for each drone in the list of drones - update the drones that are not shipping
            {
                int rand;
                //if the drone status is available, the location is a random customer, battery is enough to reach the nearest station-100%
                if (d.DroneStatus == DroneStatuses.Free)
                {
                    rand = r.Next(0, tempCl.Count);
                    d.Location = new Location()
                    {
                        Latitude = tempCl[rand].Latitude,
                        Longitude = tempCl[rand].Longitude
                    };
                    BO.Station S = Neareststation(d.Location, true);
                    d.BatteryLevel = r.Next((int)(GetDistance(d.Location.Latitude, d.Location.Longitude, S.Location.Latitude, S.Location.Longitude) * ElectricityArr[0]), 100);
                    //battery between the minimum battery needed to get to the nearest station, and full battery
                }

                //if the drone is in maintenance, the location is a random station, the battery is betweer 0-20%
                else if (d.DroneStatus == DroneStatuses.Maintenance)
                {
                    rand = r.Next(0, tempSt.Count);
                    d.Location = new Location()
                    {
                        Latitude = tempSt[rand].Latitude,
                        Longitude = tempSt[rand].Longitude,
                    };
                    d.BatteryLevel = r.Next(0, 21);
                }
            });
        }

        /// <summary>
        /// activate the simulator
        /// </summary>
        /// <param name="id">id of the drone</param>
        /// <param name="update">delegate for progress</param>
        /// <param name="checkStop">stop condition</param>
        public void RunDroneSimulator(int id, Action update, Func<bool> checkStop)
        {
            //new DroneSimulator(this, id, update, checkStop);
            new DroneSimulator(this, id, update, checkStop);
        }
    }
}