using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

namespace Dal
{
    partial class DalXml //using link to xml
    {
        XElement DroneRoot = LoadListFromXMLElement(@"Drone.xml");
        XElement DroneChargeRoot = LoadListFromXMLElement(@"DroneCharge.xml");

        #region Drone's function
        /// <summary>
        /// add a drone to the list of drones
        /// </summary>
        /// <param name="d">is the drone to add</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void addDrone(Drone d)
        {
            var droneList = LoadListFromXMLSerializer<Drone>(DronePath); 
            if (droneList.Exists(dr => dr.ID == d.ID)) //check if the drone doesn't already exist.
                throw new XmlAlreadyExistException($"Drone {d.ID} already exist.");
            //save the drones data into XElements
            XElement Id = new XElement("ID", d.ID); 
            XElement Model = new XElement("Model", d.Model);
            XElement Weight = new XElement("Weight", d.Weight);
            DroneRoot.Add(new XElement("Drone", Id, Model, Weight)); //add the drone to the list of drones
            SaveListToXMLElement(DroneRoot, DronePath); //save-update the new list
        }
        /// <summary>
        /// remove a drone from the list of drones
        /// </summary>
        /// <param name="d">is the drone to remove</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void clearDrone(Drone d)
        {
            LoadListFromXMLElement(DronePath);
            XElement DroneElement;
            DroneElement = (from drone in DroneRoot.Elements()
                            where Convert.ToInt32(drone.Element("ID").Value) == d.ID
                            select drone).FirstOrDefault(); //find the drone thanks to the id
            if (DroneElement != null)
                DroneElement.Remove(); //remove the drone from the list
            else
                throw new XmlDoesntExistException($"Drone {d.ID} doesn't exist");
            SaveListToXMLElement(DroneRoot, DronePath); //save-update the new list

        }
        /// <summary>
        /// return the drone according to the drone's id entered
        /// </summary>
        /// <param name="DronetID"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone GetDrone(int DronetID)
        {
            LoadListFromXMLElement(DronePath);
            Drone drone; //create Drone object 
            try
            {
                drone = (from d in DroneRoot.Elements()
                         where Convert.ToInt32(d.Element("ID").Value) == DronetID
                         select new Drone() //find the drone thanks to the id
                         {
                             ID = Convert.ToInt32(d.Element("ID").Value),
                             Model = d.Element("Model").Value,
                             Weight = (WeightCategories)Enum.Parse(typeof(WeightCategories), d.Element("Weight").Value, true)
                         }).FirstOrDefault();
            }
            catch
            {
                drone = default;
            }
            return drone;
        }
        /// <summary>
        /// returns the list of drones
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Drone> showDrones()
        {
            return LoadListFromXMLSerializer<Drone>(DronePath); //return the loaded list of drones
        }
        /// <summary>
        /// update the list of drones with the updated drone
        /// </summary>
        /// <param name="d">is the updated drone</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDrone(Drone d) //update the drone in the list of drones
        {
            LoadListFromXMLElement(DronePath);
            XElement DroneElement = (from drone in DroneRoot.Elements()
                                     where Convert.ToInt32(drone.Element("ID").Value) == d.ID
                                     select drone).FirstOrDefault(); //find the drone in the list according to the id
            if(DroneElement==null)
                throw new XmlDoesntExistException($"Drone {d.ID} doesn't exist");

            DroneElement.Element("Model").Value = d.Model; //update the model with the new model entered
            DroneElement.Element("Weight").Value = d.Weight.ToString(); //update the weight with the new weight entered
            SaveListToXMLElement(DroneRoot, DronePath); //save-update the new list
        }
        /// <summary>
        /// assign drone to parcel
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Assign(Parcel p, Drone d)
        {
            p.DroneId = d.ID; //the drone associated with the parcel p is d
            UpdateParcel(p); //updated the parcel with the new droneId
        }
        /// <summary>
        /// update pick up parcel time
        /// </summary>
        /// <param name="p"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickUp(Parcel p)
        {
            p.PickedUp = DateTime.Now; //update the time to be Now
            UpdateParcel(p);
        }
        /// <summary>
        /// update parcel delivery time
        /// </summary>
        /// <param name="p"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void parcelDelivery(Parcel p)
        {
            p.Delivered = DateTime.Now; //update the time to be Now
            UpdateParcel(p);
        }
        /// <summary>
        /// return the battery consuption according to the values in the config
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double[] DroneElectricity()
        {
            List<string> config = LoadListFromXMLSerializer<string>(ConfigPath); //load data from the config
            double[] arr =
               {
                double.Parse(config[0]), //Available
                double.Parse(config[1]), //LowWeight
                double.Parse(config[2]), //MiddleWeight
                double.Parse(config[3]), //HightWeight
                double.Parse(config[4])  //chargePerHour
               };
            return arr;
        }
        #endregion


        #region Drone charge's function
        /// <summary>
        /// add a drone charge to the list of drone charges
        /// </summary>
        /// <param name="dr"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void addDroneCharge(DroneCharge dr)
        {
            var droneChargeList = LoadListFromXMLSerializer<DroneCharge>(DroneChargePath);
            if (droneChargeList.Exists(d => d.DroneId == dr.DroneId)) //check if the drone doesn't already exist.
                throw new XmlAlreadyExistException($"DroneCharge {dr.DroneId} already exists.");
            //save the dronecharges data into XElements
            XElement DroneId = new XElement("DroneId", dr.DroneId); //create xElement of the drone charge
            XElement StationId = new XElement("StationId", dr.StationId);
            DroneChargeRoot.Add(new XElement("DroneCharge", DroneId, StationId)); //add the drone charge to the list
            SaveListToXMLElement(DroneChargeRoot, DroneChargePath); //save-update the new list
        }
        /// <summary>
        /// remove the drone charhe entered from the list of drone charge
        /// </summary>
        /// <param name="dr"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void clearDroneCharge(DroneCharge dr)
        {
            LoadListFromXMLElement(DroneChargePath);
            XElement DroneChargeElement;
            try
            {
                DroneChargeElement = (from droneCharge in DroneChargeRoot.Elements()
                                      where Convert.ToInt32(droneCharge.Element("DroneId").Value) == dr.DroneId
                                      select droneCharge).FirstOrDefault(); //find the dronecharge to remove
                if(DroneChargeElement==null)
                    throw new XmlDoesntExistException($"Drone {dr.DroneId} doesn't exist");
                DroneChargeElement.Remove(); //remove it from the list
                SaveListToXMLElement(DroneChargeRoot, DroneChargePath); //save-update the new list
            }
            catch { }
        }
        /// <summary>
        /// return the drone charhe according to the id entered
        /// </summary>
        /// <param name="DroneChargeId">is the id of the drone charge</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DroneCharge GetDroneCharge(int DroneChargeId)
        {
            DroneCharge droneCharge; //create a DroneCharge object
            try
            {
                droneCharge = (from dr in DroneChargeRoot.Elements()
                               where Convert.ToInt32(dr.Element("Id").Value) == DroneChargeId
                               select new DroneCharge() //find the dronecharge according to the id
                               {
                                   DroneId = Convert.ToInt32(dr.Element("DroneId").Value),
                                   StationId = Convert.ToInt32(dr.Element("StationId").Value),
                               }).FirstOrDefault();
            }
            catch
            {
                droneCharge = default;
            }
            return droneCharge;
        }
        /// <summary>
        /// return the lust of drone charges
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneCharge> showDroneCharges()
        {
            return LoadListFromXMLSerializer<DroneCharge>(DroneChargePath); //return the loaded list of drone charges
        }
        /// <summary>
        /// seng drone to charge
        /// </summary>
        /// <param name="s">is the station to update</param>
        /// <param name="d">the drone to send to charge</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void sendToCharge(Station s, Drone d)
        {
            s.ChargeSlots--; //update number of chargeslots of the satation to be one less
            UpdateStation(s);
        }
        /// <summary>
        /// release drone from charging
        /// </summary>
        /// <param name="s">s is the station to update</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void freeCharge(Station s)
        {
            s.ChargeSlots++; //update number of chargeslots of the satation to be one more
            UpdateStation(s);
        }
        #endregion

        #region Get Type according to choice
        /// <summary>
        /// this function was used to print an object in consoleUi but is irelevant here
        /// </summary>
        /// <param name="id"> the id of the chosen object (could be station, drone, client or parcel) </param>
        /// <param name="num"> the object to print </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void showOptions(int id, int num)
        {
            switch (num)
            {
                case 1:
                    Station tempS = GetStation(id);
                    // Console.WriteLine(tempS); //print the station of the id entered
                    break;
                case 2:
                    Drone tempD = GetDrone(id);
                    // Console.WriteLine(tempD); //print the drone of the id entered
                    break;
                case 3:
                    Client tempC = GetClient(id);
                    // Console.WriteLine(tempC); //print the client of the id entered
                    break;
                case 4:
                    Parcel tempP = GetParcel(id);
                    // Console.WriteLine(tempP); //print the parcel of the id entered
                    break;
            }
        }
        #endregion
    }
}