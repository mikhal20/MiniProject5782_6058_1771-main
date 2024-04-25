using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BlApi;

namespace BL
{
    internal partial class BL
    {
        #region Drone's functions
        /// <summary>
        /// returns Bl drones
        /// </summary>
        /// <param name="DroneId">the id of the drone</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone GetBlDrone(int DroneId)
        {
            var drone = dronesList.Find(d => d.ID == DroneId); //find the drone thanks to the id in the list of drones
            return new Drone() //return the drone as Drone (and not DroneForList)
            {
                ID = drone.ID,
                Model = drone.Model,
                Weight = drone.Weight,
                Location = drone.Location,
                BatteryLevel = drone.BatteryLevel,
                DroneStatus = drone.DroneStatus,
                ParcelSending= GetParcelSending(drone.ID)
                
            };
        }

        /// <summary>
        /// Add a drone to the list of drone in data
        /// </summary>
        /// <param name="d">the drone to add</param>
        /// <param name="stationId">the station ID in which to put the drone</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDrone(Drone d, int stationId)
        {
            if (d.BatteryLevel < 0 || d.BatteryLevel > 100)
            {
                throw new BatteryException("Battery is not Valid");
            }
            DO.Drone tempD = new DO.Drone() //create DO drone object
            {
                ID = d.ID,
                Model = d.Model,
                Weight = (DO.WeightCategories)d.Weight
            };
            try
            {
                lock (myDal)
                {
                    myDal.addDrone(tempD); // the drone to the list of drones
                }
            }
            catch (DO.AlreadyExist ExD)
            {
                throw new BLAlreadyExist(ExD.Message, ExD);
            }
            Station tempS;
            try
            {
                tempS = GetBlStation(stationId);
            }
            catch (BLDoesNotExist Ex)
            {
                throw new BLDoesNotExist(Ex.Message);
            }
            if (tempS.ChargeSlots < 1) //checking that the station has available charging slots
            {
                throw new NoAvailableChargeSlotsException($"Station {stationId} has no available charging slots.");
            }
            tempS.ChargeSlots--;
            DO.DroneCharge droneCharge = new DO.DroneCharge()
            {
                DroneId = d.ID,
                StationId = stationId
            };
            lock (myDal)
            {
                myDal.addDroneCharge(droneCharge);
            }
            DroneForList dr = new DroneForList() //adding the drone to the list of drones in BL
            {
                ID = d.ID,
                Model = d.Model,
                Weight = d.Weight,
                BatteryLevel = r.Next(20, 41),
                DroneStatus = DroneStatuses.Maintenance,
                Location = tempS.Location,
                ParcelID = 0
            };
            dronesList.Add(dr);
        }

        /// <summary>
        /// remove the given drone from the list of drones
        /// </summary>
        /// <param name="drone">the drone to remove</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveDrone(Drone drone)
        {
            DO.Drone temp = new DO.Drone() //copy the drone to a temporary DAL drone
            {
                ID = drone.ID,
                Model = drone.Model,
                Weight = (DO.WeightCategories)drone.Weight
            };
            try
            {
                lock (myDal)
                {
                    myDal.clearDrone(temp); //removing from the list in data
                }
                int index = dronesList.FindIndex(d => d.ID == drone.ID); //finding the index for the list in bll
                dronesList.RemoveAt(index); //remove from the list in bll
            }
            catch (DO.DoesNotExist ex)
            {
                throw new BLDoesNotExist(ex.Message);
            }
        }

        /// <summary>
        /// Given a Drone we can update the model
        /// </summary>
        /// <param name="DroneId">the id of the drone to update</param>
        /// <param name="Model">new Model to give to the drone</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDroneName(int DroneId, string Model)
        {
            DO.Drone tempD = default;
            try
            {
                lock (myDal)
                {
                    tempD = myDal.GetDrone(DroneId); //Get the Daldrone
                    tempD.Model = Model;
                    myDal.UpdateDrone(tempD); //update the DalList
                }
            }
            catch (DO.DoesNotExist ExD)
            {
                throw new BLDoesNotExist(ExD.Message);
            }

            Drone drone = default;
            try
            {
                drone = GetBlDrone(DroneId); //Get BlDrone
            }
            catch (BLDoesNotExist exDr)
            {
                throw new BLDoesNotExist(exDr.Message);
            }
            DroneForList Dl = new DroneForList()
            {
                ID = drone.ID,
                Model = Model,
                Weight = drone.Weight,
                Location = drone.Location,
                DroneStatus = drone.DroneStatus,
                BatteryLevel = drone.BatteryLevel,
            };
            if (drone.ParcelSending != null)
                Dl.ParcelID = drone.ParcelSending.Id;

            int index = dronesList.FindIndex(dr => dr.ID == tempD.ID);
            dronesList[index] = Dl; //update the BlList
        }

        /// <summary>
        /// returns the list of drones
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneForList> GetDronesList()
        {
            return dronesList;
        }

        /// <summary>
        /// Update a drone
        /// </summary>
        /// <param name="drone">The drone to update</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDrone(Drone drone)
        {
            if (!dronesList.Exists(x => x.ID == drone.ID))
                throw new IDException($"Drone {drone.ID} does not exist.");
            int temp;
            if (drone.ParcelSending == null)
                temp = 0;
            else
                temp = drone.ParcelSending.Id;
            DroneForList tempD = new DroneForList()
            {
                ID = drone.ID,
                Model = drone.Model,
                Weight = drone.Weight,
                BatteryLevel = drone.BatteryLevel,
                DroneStatus = drone.DroneStatus,
                Location = drone.Location,
                ParcelID = temp
            };
            dronesList[dronesList.FindIndex(x => x.ID == drone.ID)] = tempD; //updating the drone in the list of drones in bll

            DO.Drone d = new DO.Drone()
            {
                ID = drone.ID,
                Weight = (DO.WeightCategories)drone.Weight,
                Model = drone.Model
            };
            lock (myDal)
            {
                myDal.UpdateDrone(d); //update the drone in data layer
            }
        }
        #endregion

        #region Drone Charge's functions
        /// <summary>
        /// remove the given drone charge from the list of drone charges
        /// </summary>
        /// <param name="droneCharge">the drone charge to remove</param>
        [MethodImpl(MethodImplOptions.Synchronized)]

        public void RemoveDroneCharge(DroneCharge droneCharge)
        {
            DO.DroneCharge temp = new DO.DroneCharge() //copy the drone to a temporary DAL drone
            {
                DroneId = droneCharge.ID,
                StationId = 0
            };
            try
            {
                lock (myDal)
                {
                    myDal.clearDroneCharge(temp); //removing from the list in data
                }
            }
            catch (DO.DoesNotExist ex)
            {
                throw new BLDoesNotExist(ex.Message);
            }
        }

        /// <summary>
        /// return bl dronecharge thanks to the id
        /// </summary>
        /// <param name="droneChargeId"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DroneCharge GetBlDroneCharge(int droneChargeId)
        {
            DroneCharge droneCharge = default;
            if (droneChargeId > 100000000 && droneChargeId < 999999999)
            {
                droneCharge = new DroneCharge() //create Bl DroneCharge object 
                {
                    ID = droneChargeId //save the id according to the id entered
                };
            }
            return droneCharge;
        }

        /// <summary>
        /// send a drone to charge in a station
        /// </summary>
        /// <param name="DroneId">id of the drone</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendDroneToCharge(int DroneId)
        {
            DroneForList temp = dronesList.Find(x => x.ID == DroneId);
            if (temp == null)
                throw new BLDoesNotExist($"Drone's id {DroneId} was not found");

            Drone d = GetBlDrone(DroneId);
            Station nearS = Neareststation(temp.Location, true); //finding the nearest station to the drone

            //if the station has available charge slot and the drone has enough battery to reach it
            if (temp.DroneStatus == DroneStatuses.Free)
            {
                if (nearS.ChargeSlots > 0 && temp.BatteryLevel >= GetDistance(temp.Location.Latitude, temp.Location.Longitude, nearS.Location.Latitude, nearS.Location.Longitude) * myDal.DroneElectricity()[0])
                {
                    DO.DroneCharge dc = new DO.DroneCharge()
                    {
                        DroneId = DroneId,
                        StationId = nearS.Id
                    };
                    lock (myDal)
                    {
                        myDal.addDroneCharge(dc);
                        d.BatteryLevel -= myDal.DroneElectricity()[0] * GetDistance(d.Location.Latitude, d.Location.Longitude, nearS.Location.Latitude, nearS.Location.Longitude);
                        temp.BatteryLevel -= myDal.DroneElectricity()[0] * GetDistance(d.Location.Latitude, d.Location.Longitude, nearS.Location.Latitude, nearS.Location.Longitude);
                    }
                    if (d.BatteryLevel < 0) //battery cannot be negative
                        d.BatteryLevel = 0;
                    if (temp.BatteryLevel < 0) //battery cannot be negative
                        temp.BatteryLevel = 0;
                    d.DroneStatus = DroneStatuses.Maintenance; //update the status
                    temp.DroneStatus = DroneStatuses.Maintenance;
                    d.Location = nearS.Location; //update the location
                    temp.Location = nearS.Location;
                    temp.ChargingTime = DateTime.Now;
                  
                    UpdateDrone(d);

                    dronesList[dronesList.FindIndex(x => x.ID == d.ID)] = temp;//add to dronelist the updated drone
                    nearS.ChargeSlots--;
                    UpdateStation(nearS); //update station's details

                    return;
                }
                else
                    throw new BLDroneChargeException("Can't send drone to charge.");
            }
            else
            {
                throw new DroneStateException("Drone is not available to charge.");
            }
        }

        /// <summary>
        /// release a drone from charging
        /// </summary>
        /// <param name="DroneId">id of the drone to release</param>
        /// <param name="time">the time that the drone was charging </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void FreeCharge(int DroneId, double time = 0)
        {
            DroneForList temp = dronesList.Find(x => x.ID == DroneId);
            Drone d = GetBlDrone(DroneId); //get also Bl drone in order to update the changed details
            if (temp == null)
                throw new BLDoesNotExist($"Drone's id {DroneId} was not found");

            TimeSpan NewTime = DateTime.Now - temp.ChargingTime;

            if (temp.DroneStatus == DroneStatuses.Maintenance) //if the drone is currently charging
            {
                try
                {
                    Station nearS = Neareststation(temp.Location, true); //find nearest station to update charge slots
                    nearS.ChargeSlots++;
                    UpdateStation(nearS); //update station's details
                    DO.DroneCharge dc = new DO.DroneCharge()
                    {
                        DroneId = temp.ID,
                        StationId = nearS.Id
                    };
                    lock (myDal)
                    {
                        myDal.clearDroneCharge(dc);
                        d.BatteryLevel += myDal.DroneElectricity()[4] * NewTime.Minutes;
                    }
                    if (d.BatteryLevel > 100)
                        d.BatteryLevel = 100;
                    d.DroneStatus = DroneStatuses.Free;
                    UpdateDrone(d);
                }
                catch (BLDoesNotExist Ex)
                {
                    throw new BLDoesNotExist(Ex.Message);
                }
            }
            else
            {
                throw new BLDroneException("Can't release drone from charge");
            }
        }

        /// <summary>
        /// returns the list of DroneCharge
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneCharge> GetDroneChargeList()
        {
            lock (myDal)
            {
                try
                {
                    return from item in myDal.showDroneCharges()
                           let dc = (DO.DroneCharge)item
                           select new DroneCharge
                           {
                               ID = dc.DroneId,
                               BatteryLevel = GetBlDrone(dc.DroneId).BatteryLevel
                           };
                }
                catch (BLDoesNotExist ExD)
                {
                    throw new BLDoesNotExist(ExD.Message);
                }
            }
        }
        #endregion

        #region Get Distance function
        /// <summary>
        /// Calculate the distance between two locations
        /// https://stackoverflow.com/questions/6366408/calculating-distance-between-two-latitude-and-longitude-geocoordinates/51839058#51839058
        /// </summary>
        /// <param name="longitude">first longitude </param>
        /// <param name="latitude">first latitude</param>
        /// <param name="otherLongitude">second longitude</param>
        /// <param name="otherLatitude">second latitude</param>
        /// <returns></returns>
        public double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(lat1 * Math.PI / 180.0) * Math.Sin(lat2 * Math.PI / 180.0) + Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                dist = Math.Acos(dist);
                dist = dist / Math.PI * 180.0;
                dist = dist * 60 * 1.1515;
                dist = dist * 1.609344; //return the distance in km
                return dist;
            }
        }
        #endregion
    }
}