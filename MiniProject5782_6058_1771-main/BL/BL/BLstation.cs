using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using System.Runtime.CompilerServices;

namespace BL
{
    internal partial class BL
    {
        /// <summary>
        /// returns Bl Station
        /// </summary>
        /// <param name="StationId">id of the station</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station GetBlStation(int StationId)
        {
            List<DroneCharge> tempDroneCharges = new List<DroneCharge>();
            DO.Station dalStation;
            Station station;
            Location l;
            lock (myDal)
            {
                try
                {
                    dalStation = myDal.GetStation(StationId); //get the station according to the stationd Id entered
                }
                catch (DO.DoesNotExist exSt)
                {
                    throw new BLDoesNotExist(exSt.Message);
                }
                l = new Location() { Longitude = dalStation.Longitude, Latitude = dalStation.Latitude }; //find the location of the drone
                tempDroneCharges.AddRange(from item in myDal.showDroneCharges() //go throught the drone charges
                                          where item.StationId == dalStation.ID //if of of the drone's charging station is the same station entered
                                          let d = GetBlDrone(item.DroneId) //we add the dronecharge to the list of charging drones of that station
                                          let dr = new DroneCharge()
                                          {
                                              ID = item.DroneId,
                                              BatteryLevel = d.BatteryLevel
                                          }
                                          select dr);
            }
            station = new Station() //station from Dal to Bl
            {
                Id = dalStation.ID,
                Name = dalStation.Name,
                ChargeSlots = dalStation.ChargeSlots,
                Location = l,
                ListDroneCharge = tempDroneCharges
            };
            return station;
        }

        /// <summary>
        /// Adds a station to the list of station in data
        /// </summary>
        /// <param name="s">The station to add</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(Station s)
        {
            if (s.ChargeSlots < 0) //ChargeSlots has to be a positive number
            {
                throw new NegException("can not be a negative number");
            }
            DO.Station tempS = new DO.Station() //create a DO station object
            {
                ID = s.Id,
                Name = s.Name,
                Longitude = s.Location.Longitude,
                Latitude = s.Location.Latitude,
                ChargeSlots = s.ChargeSlots
            };
            s.ListDroneCharge = null;
            try
            {
                lock (myDal)
                {
                    myDal.addStation(tempS); //and add the station to the list of stations
                }
            }
            catch (DO.AlreadyExist ExS)
            {
                throw new BLAlreadyExist(ExS.Message, ExS);
            }
        }

        /// <summary>
        /// To clear a station from the list of the stations
        /// </summary>
        /// <param name="station">enter a station to remove</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveStation(Station station)
        {
            DO.Station temp = new DO.Station()
            {
                ID = station.Id,
                Name = station.Name,
                ChargeSlots = station.ChargeSlots,
                Latitude = station.Location.Latitude,
                Longitude = station.Location.Longitude,
            };
            try
            {
                lock (myDal)
                {
                    myDal.clearStation(temp);
                }
            }
            catch (DO.DoesNotExist Ex)
            {
                throw new BLDoesNotExist(Ex.Message);
            }
        }

        /// <summary>
        /// returns the list of StationForList
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<StationForList> GetStationList()
        {
            lock (myDal)
            {
                return from item in myDal.showStations()
                       let bs = (DO.Station)item
                       select new StationForList
                       {
                           Id = bs.ID,
                           Name = bs.Name,
                           FreeChargeSlots = getFreeStationSlots(bs.ID),
                           OccupiedChargeSlots = getOccupiedStationSlots(bs.ID)
                       };
            }
        }

        /// <summary>
        /// returns list of StationForList with available chargeslots
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<StationForList> showStationWithCharge()
        {
            IEnumerable<DO.Station> DalStationList;
            List<StationForList> result = new List<StationForList>();
            lock (myDal)
            {
                DalStationList = myDal.showStations(x => x.ChargeSlots != 0); //predicate for list of StationForList with available chargeslots
                result.AddRange(from item in DalStationList
                                select new StationForList //add to the result list from station to StationForList
                                {
                                    Id = item.ID,
                                    Name = item.Name,
                                    FreeChargeSlots = getFreeStationSlots(item.ID),
                                    OccupiedChargeSlots = getOccupiedStationSlots(item.ID)
                                });
            }
            return result;
        }

        /// <summary>
        /// calculate FreeStationSlots
        /// </summary>
        /// <param name="stationiD">id of the station</param>
        /// <returns></returns>
        private int getFreeStationSlots(int stationiD)
        {
            return GetBlStation(stationiD).ChargeSlots;
        }

        /// <summary>
        /// calculate OccupiedStationSlots
        /// </summary>
        /// <param name="stationiD">id of the station</param>
        /// <returns></returns>
        private int getOccupiedStationSlots(int stationiD)
        {
            return GetBlStation(stationiD).ListDroneCharge.Count();
        }

        /// <summary>
        /// find the nearest station from a given location
        /// </summary>
        /// <param name="l">given location</param>
        /// <param name="flag">sign</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station Neareststation(Location l, bool flag)
        {
            double lati = l.Latitude;
            double longi = l.Longitude;
            double mindistance = 99999999;
            double tempDistance = 0;
            Station s = default;
            lock (myDal)
            {
                foreach (var item in myDal.showStations())
                {
                    if (flag == true)
                        tempDistance = GetDistance(lati, longi, item.Latitude, item.Longitude);
                    if (mindistance > tempDistance)
                    {
                        mindistance = tempDistance;// keeps the closest one
                        Location loc = new Location()
                        {
                            Latitude = item.Latitude,
                            Longitude = item.Longitude
                        };
                        s = new Station()
                        {
                            Name = item.Name,
                            Id = item.ID,
                            Location = loc,
                            ChargeSlots = item.ChargeSlots
                        };
                    }
                }
            }
            return s;
        }

        /// <summary>
        /// Update the name and or the number of available charslots of a given station
        /// </summary>
        /// <param name="StationId">id of the station given</param>
        /// <param name="StationName">Name to update to the station</param>
        /// <param name="num">num of chargeslots to update</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStationName(int StationId, string StationName = " ", string num = " ")
        {
            DO.Station tempS;
            try
            {
                lock (myDal)
                {
                    tempS = myDal.GetStation(StationId);
                    myDal.UpdateStationName(tempS, StationName, num);
                }
            }
            catch (DO.DoesNotExist ExD)
            {
                throw new BLDoesNotExist(ExD.Message);
            }
        }

        /// <summary>
        /// Update a station
        /// </summary>
        /// <param name="drone">The station to update</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStation(Station station)
        {
            DO.Station temp = new DO.Station()
            {
                ID = station.Id,
                Name = station.Name,
                ChargeSlots = station.ChargeSlots,
                Longitude = station.Location.Longitude,
                Latitude = station.Location.Latitude
            };
            lock (myDal)
            {
                myDal.UpdateStation(temp);
            }
        }
    }
}