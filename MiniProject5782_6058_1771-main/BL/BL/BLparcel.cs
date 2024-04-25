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
        /// add a parcel to the list of parcels
        /// </summary>
        /// <param name="p"> a Parcel received to add</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddParcel(Parcel p)
        {
            DO.Parcel tempP = new DO.Parcel()
            {
                ID = p.Id,
                Priority = (DO.Priorities)p.Priority,
                Weight = (DO.WeightCategories)p.Weight,
                SenderId = p.Sender.Id,
                TargetId = p.Recipient.Id,
                Requested = null,
                Scheduled = null,
                PickedUp = null,
                Delivered = null,
                DroneId = 0
            };
            p.DroneInParcel = null;
            try
            {
                lock (myDal)
                {
                    myDal.addParcel(tempP);
                }
            }
            catch (DO.AlreadyExist ExP)
            {
                throw new BLAlreadyExist(ExP.Message, ExP);
            }
        }

        /// <summary>
        /// Removes a parcel from the list
        /// </summary>
        /// <param name="parcel">The parcel to remove</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveParcel(Parcel parcel)
        {
            DO.Parcel temp = new DO.Parcel()
            {
                ID=parcel.Id,
                SenderId = parcel.Sender.Id,
                TargetId = parcel.Recipient.Id,
                Weight = (DO.WeightCategories)parcel.Weight,
                Priority = (DO.Priorities)parcel.Priority,
                Requested = DateTime.Now,
                DroneId = 0,
                Scheduled = null,
                PickedUp = null,
                Delivered = null
            };
            try
            {
                lock (myDal)
                {
                    myDal.clearParcel(temp);
                }
            }
            catch (DO.DoesNotExist Ex)
            {
                throw new BLDoesNotExist(Ex.Message);
            }
        }

        /// <summary>
        /// Assign a drone to a parcel
        /// </summary>
        /// <param name="DroneId">The ID of the drone</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AssignParcelToDrone(int DroneId)
        {
            Drone drone;
            try
            {
                drone = GetBlDrone(DroneId); //try to get the drone, will throw an exception if ID doesnt exist
            }
            catch (BLDoesNotExist ex)
            {
                throw new BLDoesNotExist(ex.Message);
            }
            if (drone.DroneStatus != DroneStatuses.Free) //checking that the drone is available
                throw new DroneStateException($"Drone {DroneId} is currently unavailable for shipping.");

            List<Parcel> parcels = new List<Parcel>();  //getting the list of parcels
            List<Parcel> highPriority = new List<Parcel>();
            List<Parcel> mediumPriority = new List<Parcel>();
            List<Parcel> lowPriority = new List<Parcel>();
            lock (myDal)
            {
                foreach (var item in myDal.showParcels())
                {
                    Parcel pl = GetBlParcel(item.ID);
                    parcels.Add(pl);
                }
                foreach (Parcel p in parcels) //deviding the parcels into 3 lists according to priority
                {
                    if (p.Scheduled == null) //if the parcel does not yet have a drone
                    {
                        if (p.Priority == Priorities.Emergency && p.Weight <= drone.Weight)
                        {
                            switch (p.Weight)
                            {
                                case WeightCategories.Heavy:
                                    if (drone.BatteryLevel >= myDal.DroneElectricity()[3] * DeliveryDistance(DroneId, p.Sender.Id, p.Recipient.Id)) //if there is enough battery to do the delivery
                                        highPriority.Add(p);
                                    break;
                                case WeightCategories.Middle:
                                    if (drone.BatteryLevel >= myDal.DroneElectricity()[2] * DeliveryDistance(DroneId, p.Sender.Id, p.Recipient.Id)) //if there is enough battery to do the delivery
                                        highPriority.Add(p);
                                    break;
                                case WeightCategories.Low:
                                    if (drone.BatteryLevel >= myDal.DroneElectricity()[1] * DeliveryDistance(DroneId, p.Sender.Id, p.Recipient.Id)) //if there is enough battery to do the delivery
                                        highPriority.Add(p);
                                    break;
                            }
                        }
                        else if (p.Priority == Priorities.Fast && p.Weight <= drone.Weight)
                        {
                            switch (p.Weight)
                            {
                                case WeightCategories.Heavy:
                                    if (drone.BatteryLevel >= myDal.DroneElectricity()[3] * DeliveryDistance(DroneId, p.Sender.Id, p.Recipient.Id)) //if there is enough battery to do the delivery
                                        mediumPriority.Add(p);
                                    break;
                                case WeightCategories.Middle:
                                    if (drone.BatteryLevel >= myDal.DroneElectricity()[2] * DeliveryDistance(DroneId, p.Sender.Id, p.Recipient.Id)) //if there is enough battery to do the delivery
                                        mediumPriority.Add(p);
                                    break;
                                case WeightCategories.Low:
                                    if (drone.BatteryLevel >= myDal.DroneElectricity()[1] * DeliveryDistance(DroneId, p.Sender.Id, p.Recipient.Id)) //if there is enough battery to do the delivery
                                        mediumPriority.Add(p);
                                    break;
                            }
                        }
                        else if (p.Priority == Priorities.Regular && p.Weight <= drone.Weight)
                        {
                            switch (p.Weight)
                            {
                                case WeightCategories.Heavy:
                                    if (drone.BatteryLevel >= myDal.DroneElectricity()[3] * DeliveryDistance(DroneId, p.Sender.Id, p.Recipient.Id)) //if there is enough battery to do the delivery
                                        lowPriority.Add(p);
                                    break;
                                case WeightCategories.Middle:
                                    if (drone.BatteryLevel >= myDal.DroneElectricity()[2] * DeliveryDistance(DroneId, p.Sender.Id, p.Recipient.Id)) //if there is enough battery to do the delivery
                                        lowPriority.Add(p);
                                    break;
                                case WeightCategories.Low:
                                    if (drone.BatteryLevel >= myDal.DroneElectricity()[1] * DeliveryDistance(DroneId, p.Sender.Id, p.Recipient.Id)) //if there is enough battery to do the delivery
                                        lowPriority.Add(p);
                                    break;
                            }
                        }
                    }
                }
            }
            List<Parcel> heavy = new List<Parcel>();
            List<Parcel> medium = new List<Parcel>();
            List<Parcel> light = new List<Parcel>();

            if (highPriority.Count > 0) //if there is at least one high priority parcel that the drone could carry
            {
                foreach (Parcel p in highPriority)
                {
                    if (p.Weight == WeightCategories.Heavy)
                        heavy.Add(p);
                    else if (p.Weight == WeightCategories.Middle)
                        medium.Add(p);
                    else if (p.Weight == WeightCategories.Low)
                        light.Add(p);
                }
            }
            else if (mediumPriority.Count > 0) //otherwise if there is at least one medium priotity parcel that the drone can carry
            {
                foreach (Parcel p in mediumPriority)
                {
                    if (p.Weight == WeightCategories.Heavy)
                        heavy.Add(p);
                    else if (p.Weight == WeightCategories.Middle)
                        medium.Add(p);
                    else if (p.Weight == WeightCategories.Low)
                        light.Add(p);
                }
            }
            else if (lowPriority.Count > 0) //otherwise if there is at least one low priotity parcel that the drone can carry
            {
                foreach (Parcel p in lowPriority)
                {
                    if (p.Weight == WeightCategories.Heavy)
                        heavy.Add(p);
                    else if (p.Weight == WeightCategories.Middle)
                        medium.Add(p);
                    else if (p.Weight == WeightCategories.Low)
                        light.Add(p);
                }
            }
            else //otherwise the drone cannot carry any parcel, throw an exception
            {
                throw new DroneStateException($"Drone {DroneId} cannot currently deliver any parcel.");
            }

            Parcel parcel;

            //finding the nearest parcel among the possible parcels
            if (heavy.Count > 0) //if there is at least one heavy parcel the drone could carry
            {
                parcel = closestParcel(DroneId, heavy);
            }
            else if (medium.Count > 0) //otherwise if there is at least one medium weight parcel the drone could carry
            {
                parcel = closestParcel(DroneId, medium);
            }
            else //otherwise there is only a lowweight parcel the drone could carry
            {
                parcel = closestParcel(DroneId, light);
            }
            if (parcel != null)
            {
                Client cSender = GetBlClient(parcel.Sender.Id);
                Client cRecipient = GetBlClient(parcel.Recipient.Id);
                int num = (int)drone.Weight + 1; //check the weight of the drone 
                //check if the drone has enought battery to pick up parcel:
                if (drone.BatteryLevel < (GetDistance(drone.Location.Latitude, drone.Location.Longitude, cSender.Location.Latitude, cSender.Location.Longitude) * myDal.DroneElectricity()[num])+ 
                    GetDistance(cSender.Location.Latitude, cSender.Location.Longitude, cRecipient.Location.Latitude, cRecipient.Location.Longitude) * myDal.DroneElectricity()[num])
                {
                    throw new BatteryException($"Drone {DroneId} cannot assign the parcel since it doesn't have enough battery");
                }

                ParcelSending ps = new ParcelSending() //updating the parcel that the drone carries
                {
                    Id = parcel.Id,
                    Sender = parcel.Sender,
                    Recipient = parcel.Recipient,
                    Priority = parcel.Priority,
                    DeliverLocation = GetBlClient(parcel.Recipient.Id).Location,
                    PickLocation = GetBlClient(parcel.Sender.Id).Location,
                    ParcelStatus = true
                };
                drone.ParcelSending = ps;
                drone.DroneStatus = DroneStatuses.Shipping; //updating the status of the drone to be in shipping
                drone.ParcelSending.Id = parcel.Id;
                DroneParcel Dp = new DroneParcel()
                {
                    Id = DroneId,
                    BatteryLevel = drone.BatteryLevel,
                    Location = drone.Location,
                };
                parcel.DroneInParcel = Dp;
                parcel.Scheduled = DateTime.Now;
                UpdateParcel(parcel); //update the parcel
            }
            UpdateDrone(drone);//update the drone
        }

        /// <summary>
        /// caculates the distance of a delivery
        /// </summary>
        /// <param name="droneId">id of the drone</param>
        /// <param name="SenderId">id of the sender</param>
        /// <param name="TargetId">id of the recipient</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double DeliveryDistance(int droneId, int SenderId, int TargetId)
        {
            Drone d = GetBlDrone(droneId);
            Client Sender = GetBlClient(SenderId);
            Client Recipient = GetBlClient(TargetId);

            return (GetDistance(d.Location.Latitude, d.Location.Longitude, Sender.Location.Latitude, Sender.Location.Longitude)
                + GetDistance(Sender.Location.Latitude, Sender.Location.Longitude, Recipient.Location.Latitude, Recipient.Location.Longitude)
                + GetDistance(Recipient.Location.Latitude, Recipient.Location.Longitude, Neareststation(Recipient.Location, true).Location.Latitude, Neareststation(Recipient.Location, true).Location.Longitude)) / 1000;
        }

        /// <summary>
        /// finding the closest parcel to the drone among the parcels
        /// </summary>
        /// <param name="DroneId">id of the drone</param>
        /// <param name="parcels">list of the parcels</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel closestParcel(int DroneId, List<Parcel> parcels)
        {
            Parcel p = default;
            foreach (var item in parcels)
            {
               // if (item.DroneInParcel.Id == DroneId)
                    p = item;
            }
            return p;
        }

        /// <summary>
        /// the function returns the list of Parcel that the client has received
        /// </summary>
        /// <param name="ClientId">the id of the client</param>
        /// <returns></returns>
        private List<ParcelCustomer> ReceivedXParcel(int ClientId)
        {
            List<ParcelCustomer> ParcelCustomers = new List<ParcelCustomer>();
            string TempName;
            lock (myDal)
            {
                foreach (var item in myDal.showParcels())
                {
                    if (item.TargetId == ClientId)
                    {
                        try
                        {
                            TempName = myDal.GetClient(ClientId).Name;
                        }
                        catch (DO.DoesNotExist ExC)
                        {
                            throw new BLDoesNotExist(ExC.Message);
                        }
                        CustomerParcel customer = new CustomerParcel()
                        {
                            Id = ClientId,
                            Name = TempName
                        };
                        Status Stat;
                        if (item.Delivered != null)
                            Stat = Status.Delivered;
                        else if (item.PickedUp != null)
                            Stat = Status.Picked;
                        else if (item.Scheduled != null)
                            Stat = Status.Assigned;
                        else Stat = Status.Created;
                        ParcelCustomer tempPc = new ParcelCustomer
                        {
                            Id = item.ID,
                            Weight = (WeightCategories)item.Weight,
                            Priority = (Priorities)item.Priority,
                            Status = Stat,
                            CustomerParcel = customer
                        };
                        if (Stat == Status.Picked || Stat == Status.Delivered)
                            ParcelCustomers.Add(tempPc);
                    }
                }
            }
            return ParcelCustomers;
        }

        /// <summary>
        /// returns BL Parcel
        /// </summary>
        /// <param name="id"> id of the parcel</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetBlParcel(int id)
        {
            DO.Parcel dalParcel;
            Parcel parcel;
            try
            {
                lock (myDal)
                {
                    dalParcel = myDal.GetParcel(id); //get the parcel from the dal
                }
            }
            catch (DO.DoesNotExist exPr)
            {
                throw new BLDoesNotExist(exPr.Message);
            }
            CustomerParcel customerTarget = new CustomerParcel() //find the id of the client who will receive the parcel
            {
                Id = dalParcel.TargetId
            };
            CustomerParcel CustomerSender = new CustomerParcel() //find the id of the client who sent the parcel
            {
                Id = dalParcel.SenderId
            };
            Client TargetClient, SenderClient;
            try //try to find the client themselves with their ids
            {
                TargetClient = GetBlClient(dalParcel.TargetId);
                SenderClient = GetBlClient(dalParcel.SenderId);
            }
            catch (BLDoesNotExist ExC)
            {
                throw new BLDoesNotExist(ExC.Message);
            }
            if (TargetClient != null)
                customerTarget.Name = TargetClient.Name;
            if (SenderClient != null)
                CustomerSender.Name = SenderClient.Name;
            Drone d;
            DroneParcel droneParcel = new DroneParcel() //Find the drone's details of the parcel
            {
                Id = dalParcel.DroneId
            };
            if (SenderClient == null && TargetClient == null) //if there is no sender and no target then the parcel is not assiged to a drone yet
            {
                try
                {
                    d = GetBlDrone(dalParcel.DroneId); //Get the BlDrone (if there is one)
                }
                catch (BLDoesNotExist Exd)
                {
                    throw new BLDoesNotExist(Exd.Message);
                }
                droneParcel = new DroneParcel() //Find the drone's details of the parcel
                {
                    Id = dalParcel.DroneId,
                    BatteryLevel = d.BatteryLevel,
                    Location = d.Location
                };
            }
            parcel = new Parcel() //From Dal Parcel to Bl Parcel
            {
                Id = dalParcel.ID,
                Delivered = dalParcel.Delivered,
                PickedUp = dalParcel.PickedUp,
                Priority = (Priorities)dalParcel.Priority,
                Requested = dalParcel.Requested,
                Scheduled = dalParcel.Scheduled,
                Weight = (WeightCategories)dalParcel.Weight,
                Recipient = customerTarget,
                Sender = CustomerSender,
                DroneInParcel = droneParcel,
            };
            return parcel;
        }

        /// <summary>
        /// to deliver a parcel with a drone
        /// </summary>
        /// <param name="DroneId">the id of the drone</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeliverParcelWithDrone(int DroneId)
        {
            Drone drone = GetBlDrone(DroneId);
            Parcel parcel = GetBlParcel(drone.ParcelSending.Id);
            if (drone.ParcelSending.Id == 0) //if the drone does not have a parcel 
            {
                throw new DroneStateException($"Drone {DroneId} does not have a parcel assigned.");
            }
            if (parcel.PickedUp == DateTime.Now) //if the parcel has not already been picked up
            {
                throw new DroneStateException($"Drone {DroneId} cannot deliver the parcel since it has not been picked up.");
            }
            if (drone.BatteryLevel < DeliveryDistance(DroneId, drone.ParcelSending.Sender.Id, drone.ParcelSending.Recipient.Id))
            {
                throw new DroneStateException($"Drone {DroneId} cannot deliver the parcel since it doesn't have enough battery");
            }
            if (drone.BatteryLevel < 0) //the battery is between 0 to 100 %
            {
                throw new BatteryException("Cannot deliver parcel, Send drone to charge");
            }
            lock (myDal)
            {
                switch (parcel.Weight) //update the battery according to weight and distance
                {
                    case WeightCategories.Heavy:
                        drone.BatteryLevel -= myDal.DroneElectricity()[3] * GetDistance(drone.Location.Latitude, drone.Location.Longitude, GetBlClient(parcel.Recipient.Id).Location.Latitude, GetBlClient(parcel.Recipient.Id).Location.Longitude);
                        break;
                    case WeightCategories.Middle:
                        drone.BatteryLevel -= myDal.DroneElectricity()[2] * GetDistance(drone.Location.Latitude, drone.Location.Longitude, GetBlClient(parcel.Recipient.Id).Location.Latitude, GetBlClient(parcel.Recipient.Id).Location.Longitude);
                        break;
                    case WeightCategories.Low:
                        drone.BatteryLevel -= myDal.DroneElectricity()[1] * GetDistance(drone.Location.Latitude, drone.Location.Longitude, GetBlClient(parcel.Recipient.Id).Location.Latitude, GetBlClient(parcel.Recipient.Id).Location.Longitude);
                        break;
                }
            }
            if (drone.BatteryLevel < 0)
                drone.BatteryLevel = 0;
            drone.Location = GetBlClient(parcel.Recipient.Id).Location; //changing the location of the drone to the location of the target
            drone.DroneStatus = DroneStatuses.Free;
            drone.ParcelSending.Id = 0;
            UpdateDrone(drone); //call to the function to update the drone
        }

        /// <summary>
        /// update the given parcel
        /// </summary>
        /// <param name="parcel">is the parcel to update</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateParcel(Parcel parcel)
        {
            DO.Parcel temp = new DO.Parcel()
            {
                ID = parcel.Id,
                SenderId = parcel.Sender.Id,
                TargetId = parcel.Recipient.Id,
                Weight = (DO.WeightCategories)parcel.Weight,
                Priority = (DO.Priorities)parcel.Priority,
                Requested = parcel.Requested,
                DroneId = parcel.DroneInParcel.Id,
                Scheduled = parcel.Scheduled,
                PickedUp = parcel.PickedUp,
                Delivered = parcel.Delivered
            }; 
            lock (myDal)
            {
                myDal.UpdateParcel(temp);
            }
        }

        /// <summary>
        /// returns a Parcelsending for Bl Drone
        /// </summary>
        /// <param name="DroneId">id of the drone</param>
        /// <returns></returns>
        private ParcelSending GetParcelSending(int DroneId)
        {
            ParcelSending parcelSending = default;
            foreach (var item in GetDronesList())
            {
                if (item.ID == DroneId) //item is the drone that we want to find his parcel
                {
                    if (item.ParcelID != 0)
                    {
                        Parcel p = default;
                        try
                        {
                            p = GetBlParcel(item.ParcelID); //p is the parcel associated with item drone
                        }
                        catch (BLDoesNotExist ExD)
                        {
                            throw new BLDoesNotExist(ExD.Message); ;
                        }
                        Client Client1, Client2;
                        try
                        {
                            Client1 = GetBlClient(p.Recipient.Id); //client who will receive the parcel p
                            Client2 = GetBlClient(p.Sender.Id); //client who sent parcel p
                        }
                        catch (BLDoesNotExist ExC)
                        {
                            throw new BLDoesNotExist(ExC.Message);
                        }
                        bool flag = false;
                        if (p.PickedUp != null)
                            flag = true;
                        parcelSending = new ParcelSending //convert parcel to parcelSending
                        {
                            Id = p.Id,
                            Priority = p.Priority,
                            Recipient = p.Recipient,
                            Sender = p.Sender,
                            DeliverLocation = Client1.Location,
                            Distance = GetDistance(Client1.Location.Latitude, Client1.Location.Longitude, Client2.Location.Latitude, Client2.Location.Longitude),
                            ParcelStatus = flag,
                            PickLocation = Client2.Location
                        };
                    }
                }
            }
            return parcelSending;
        }

        /// <summary>
        /// returns the list of ParcelForList
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<ParcelForList> GetParcelList() //predicate for list of parcel without associated drones
        {
            List<ParcelForList> result = new List<ParcelForList>();
            Client clientSender, clientTarget;
            lock (myDal)
            {
                foreach (var item in myDal.showParcels())
                {
                    Status Stat;
                    if (item.Delivered != null)
                        Stat = Status.Delivered;
                    else if (item.PickedUp != null)
                        Stat = Status.Picked;
                    else if (item.Scheduled != null)
                        Stat = Status.Assigned;
                    else Stat = Status.Created;
                    try
                    {
                        clientSender = GetBlClient(item.SenderId); //get the client who sent the parcel
                        clientTarget = GetBlClient(item.TargetId); //get the client whom the parcel is sent to
                    }
                    catch (BLDoesNotExist ExC)
                    {
                        throw new BLDoesNotExist(ExC.Message);
                    }
                    if (clientSender != null && clientTarget != null)
                    {
                        result.Add(new ParcelForList
                        {
                            Id = item.ID,
                            SenderName = clientSender.Name,
                            RecipientName = clientTarget.Name,
                            Weight = (WeightCategories)item.Weight,
                            Priority = (Priorities)item.Priority,
                            Status = Stat
                        });
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// returns the list of ParcelForList but without drone associated
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<ParcelForList> showParcelWithoutDrone()
        {
            IEnumerable<DO.Parcel> DalParcelList;
            lock (myDal)
            {
                DalParcelList = myDal.showParcels(x => x.DroneId == 0); //get the filtered list with the predicate
            }
            List<ParcelForList> result = new List<ParcelForList>();
            foreach (var item in DalParcelList)
            {
                string SendName, GetName;
                try
                {
                    SendName = GetBlClient(item.SenderId).Name;
                    GetName = GetBlClient(item.TargetId).Name;
                }
                catch (BLDoesNotExist ExC)
                {
                    throw new BLDoesNotExist(ExC.Message);
                }
                result.Add(new ParcelForList //add all the parcels to result list from parcel to ParcelForList
                {
                    Id = item.ID,
                    SenderName = SendName,
                    RecipientName = GetName,
                    Weight = (WeightCategories)item.Weight,
                    Priority = (Priorities)item.Priority,
                    Status = Status.Created
                });
            }
            return result; //return the list of parcel without drone associated
        }

        /// <summary>
        /// the function returns the list of Parcel that the client has sent
        /// </summary>
        /// <param name="ClientId">the id of the client</param>
        /// <returns></returns>
        private List<ParcelCustomer> SentXParcel(int ClientId)
        {
            List<ParcelCustomer> ParcelCustomers = new List<ParcelCustomer>();
            string TempName;
            lock (myDal)
            {
                foreach (var item in myDal.showParcels())
                {
                    if (item.SenderId == ClientId)
                    {
                        try
                        {
                            TempName = myDal.GetClient(ClientId).Name;
                        }
                        catch (DO.DoesNotExist ExC)
                        {
                            throw new BLDoesNotExist(ExC.Message);
                        }
                        CustomerParcel customer = new CustomerParcel()
                        {
                            Id = ClientId,
                            Name = TempName
                        };
                        Status Stat;
                        if (item.Delivered != null)
                            Stat = Status.Delivered;
                        else if (item.PickedUp != null)
                            Stat = Status.Picked;
                        else if (item.Scheduled != null)
                            Stat = Status.Assigned;
                        else Stat = Status.Created;
                        ParcelCustomer tempPc = new ParcelCustomer
                        {
                            Id = item.ID,
                            Weight = (WeightCategories)item.Weight,
                            Priority = (Priorities)item.Priority,
                            Status = Stat,
                            CustomerParcel = customer
                        };
                        ParcelCustomers.Add(tempPc);
                    }
                }
            }
            return ParcelCustomers;
        }

        /// <summary>
        /// Update that a drone picked up a parcel
        /// </summary>
        /// <param name="DroneId">The ID of the drone</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickUpParcelWithDrone(int DroneId)
        {
            DroneForList droneFromList = default;
            Drone drone;
            try
            {
                drone = GetBlDrone(DroneId); //get the drone (from Bl)
                foreach (var item in dronesList)
                {
                    if (item.ID == drone.ID)
                        droneFromList = item; //save the drone also as droneforlist
                }
            }
            catch (BLDoesNotExist ExD)
            {
                throw new BLDoesNotExist(ExD.Message);
            }
            if (droneFromList.ParcelID == 0) //if the drone does not have a parcel assigned
            {
                throw new DroneStateException($"Drone {DroneId} does not have a parcel assigned.");
            }
            Parcel parcel = GetBlParcel(droneFromList.ParcelID); //get the parcel to pick up
            if (parcel.PickedUp == DateTime.Now) //if the parcel has already been picked up
            {
                throw new DroneStateException($"Drone {DroneId} cannot pick up the parcel since it has already been picked up.");
            }
           
            lock (myDal)
            {
                switch (parcel.Weight) //update the battery according to weight and distance
                {
                    case WeightCategories.Heavy:
                        drone.BatteryLevel -= myDal.DroneElectricity()[3] * GetDistance(drone.Location.Latitude, drone.Location.Longitude, GetBlClient(parcel.Sender.Id).Location.Latitude, GetBlClient(parcel.Sender.Id).Location.Longitude);
                        break;
                    case WeightCategories.Middle:
                        drone.BatteryLevel -= myDal.DroneElectricity()[2] * GetDistance(drone.Location.Latitude, drone.Location.Longitude, GetBlClient(parcel.Sender.Id).Location.Latitude, GetBlClient(parcel.Sender.Id).Location.Longitude);
                        break;
                    case WeightCategories.Low:
                        drone.BatteryLevel -= myDal.DroneElectricity()[1] * GetDistance(drone.Location.Latitude, drone.Location.Longitude, GetBlClient(parcel.Sender.Id).Location.Latitude, GetBlClient(parcel.Sender.Id).Location.Longitude);
                        break;
                }
            }
            if (drone.BatteryLevel < 0)
                drone.BatteryLevel = 0;
            drone.Location = GetBlClient(parcel.Sender.Id).Location; //change the location of the drone to the location of the sender
            UpdateDrone(drone);
        }

        /// <summary>
        /// the client has to comfirm that the parcel was picked-up
        /// </summary>
        /// <param name="parcel"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ConfirmPickUp(Parcel parcel)
        {
            parcel.PickedUp = DateTime.Now; //update the pick up time to be now
            UpdateParcel(parcel); //call to the function to update the parcel
        }

        /// <summary>
        /// the client comfirms that the parcel was delivered
        /// </summary>
        /// <param name="parcel"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ConfirmDelivery(Parcel parcel)
        {
            parcel.Delivered = DateTime.Now; //update the deliverey time to be now
            UpdateParcel(parcel); //call to the function to update the parcel
        }
    }
}