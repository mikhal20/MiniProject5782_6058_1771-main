using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using System.Threading;
using static BL.BL;

namespace BL
{
    internal class DroneSimulator
    {
        int timer = 1000;
        int cycle = 1; //one second in cycle
        double speed = 0.5; //half a km per second
        Drone drone;

        public DroneSimulator(BL myBL, int id, Action updateDelegate, Func<bool> checkStop)
        {
            lock (myBL)
            {
                drone = myBL.GetBlDrone(id); //get the drone with updated details
            }

            while (!checkStop())
            {
                switch (drone.DroneStatus)
                {
                    case DroneStatuses.Free: //when the drone is available
                        lock (myBL)
                        {
                            try
                            {
                                myBL.AssignParcelToDrone(drone.ID); //try to assign the drone with a parcel
                                drone = myBL.GetBlDrone(drone.ID); //get the drone with updated details
                            }
                            catch (DroneStateException) { } 
                            catch (BatteryException) //if there is not enough battery to make a delivery for any of the parcels
                            {
                                try
                                {
                                    //get the station that is nearest to the drone, among the stations that have available charge slots
                                    Station s = myBL.Neareststation(drone.Location, true);

                                    if (s != null) //if a station was found
                                    {
                                        Thread.Sleep((int)(myBL.GetDistance(s.Location.Latitude, s.Location.Longitude, drone.Location.Latitude, drone.Location.Longitude) / speed)); //update the drone only after the drone had enough time to reach the station
                                        myBL.SendDroneToCharge(drone.ID); //send the drone to charge
                                        drone = myBL.GetBlDrone(drone.ID);
                                        updateDelegate();
                                    }
                                }
                                //in both cases, the drone will wait for the next cycle and try again to see if there is an available station for him to reach
                                catch (DroneStateException) { } //if there is no available charge slots try again in the next cycle
                                catch (BatteryException) { } //if there is not enough battery to get to the nearest station with available charge slots
                            }
                        }
                        break;

                    case DroneStatuses.Maintenance: //when the drone is charging
                        lock (myBL)
                        {
                            if (drone.BatteryLevel == 100)
                            {
                                myBL.FreeCharge(drone.ID); //release charge
                                drone = myBL.GetBlDrone(drone.ID); //get the drone with updated details
                            }
                            else
                            {
                                drone.BatteryLevel += cycle * myDal.DroneElectricity()[0]; //charge per hour
                                if (drone.BatteryLevel > 100)
                                    drone.BatteryLevel = 100;
                                myBL.UpdateDrone(drone);
                            }
                        }
                        break;

                    case DroneStatuses.Shipping: //when the drone is shipping
                        lock (myBL)
                        {
                            Parcel p = myBL.GetBlParcel(drone.ParcelSending.Id);

                            if (p.PickedUp == null) //if the parcel was not picked up yet
                            {
                                if (drone.ParcelSending.Distance / speed < ((TimeSpan)(DateTime.Now - p.Scheduled)).Seconds)
                                {
                                    drone.BatteryLevel -= speed * cycle * myDal.DroneElectricity()[0]; //d=v*t , substract from the battery according to distance
                                    drone.Location = drone.ParcelSending.PickLocation; //update location
                                    p.PickedUp = DateTime.Now;
                                    drone.ParcelSending.Distance = myBL.GetDistance(drone.ParcelSending.PickLocation.Latitude, drone.ParcelSending.PickLocation.Longitude, drone.ParcelSending.DeliverLocation.Latitude, drone.ParcelSending.DeliverLocation.Longitude);
                                    myBL.UpdateParcel(p);
                                    myBL.UpdateDrone(drone);
                                }
                                else
                                {
                                    drone.BatteryLevel -= speed * cycle * myDal.DroneElectricity()[0]; //d=v*t , substract from the battery according to distance
                                    myBL.UpdateDrone(drone);
                                }
                            }
                            else //if it was picked up but not delivered
                            {
                                if (drone.ParcelSending.Distance / speed < ((TimeSpan)(DateTime.Now - p.PickedUp)).Seconds)
                                {
                                    drone.BatteryLevel -= speed * cycle * myDal.DroneElectricity()[(int)p.Weight]; //d=v*t , substract from the battery according to distance
                                    drone.Location = drone.ParcelSending.PickLocation; //update location
                                    drone.DroneStatus = DroneStatuses.Free; //make drone available
                                    drone.ParcelSending.Id = 0;
                                    p.Delivered = DateTime.Now;
                                    myBL.UpdateParcel(p);
                                    myBL.UpdateDrone(drone);
                                }
                                else
                                {
                                    drone.BatteryLevel -= speed * cycle * myDal.DroneElectricity()[(int)p.Weight];
                                    myBL.UpdateDrone(drone);
                                }
                            }
                        }
                        break;
                }
                updateDelegate();
                Thread.Sleep(timer);
            }
        }
    }
}

