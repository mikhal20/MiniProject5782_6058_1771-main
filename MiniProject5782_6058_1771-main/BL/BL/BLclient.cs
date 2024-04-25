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
        /// Add a customer to the data list of customers
        /// </summary>
        /// <param name="c">the customer to add</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddClient(Client c)
        {
            if (c.Id < 100000000 || c.Id > 999999999)
            {
                throw new IDException("Id is not Valid");
            }
            DO.Client tempC = new DO.Client()
            {
                ID = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Longitude = c.Location.Longitude,
                Latitude = c.Location.Latitude,
            };
            try
            {
                lock (myDal)
                {
                    myDal.addClient(tempC);
                }
            }
            catch (DO.AlreadyExist ExC)
            {
                throw new BLAlreadyExist(ExC.Message, ExC);
            }
        }

        /// <summary>
        /// returns Bl client 
        /// </summary>
        /// <param name="ClientId">the id of the client</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Client GetBlClient(int ClientId)
        {
            DO.Client dalClient;
            Client client = default;
            if (ClientId > 100000000 && ClientId < 999999999)
            {
                try
                {
                    lock (myDal)
                    {
                        dalClient = myDal.GetClient(ClientId);
                    }
                }
                catch (DO.DoesNotExist exCl)
                {
                    throw new BLDoesNotExist(exCl.Message);
                }
                Location l = new Location()
                {
                    Latitude = dalClient.Latitude,
                    Longitude = dalClient.Longitude
                };
                client = new Client()
                {
                    Id = dalClient.ID,
                    Name = dalClient.Name,
                    Phone = dalClient.Phone,
                    Location = l,
                    ReceiveParcels = ReceivedXParcel(ClientId),
                    SentParcels = SentXParcel(ClientId),
                };
            }
            return client;
        }

        /// <summary>
        /// return the List of ClientForList
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<ClientForList> GetClientList()
        {
            List<ClientForList> result = new List<ClientForList>();
            lock (myDal)
            {
                foreach (var item in myDal.showClients())
                {
                    int CountSentAndDelivered = 0, CountSentNotDelivered = 0, CountParcelOnWay = 0, CountReceived = 0;
                    foreach (var p in GetParcelList())
                    {
                        if (p.RecipientName == item.Name)
                        {
                            if (p.Status == Status.Delivered) //calculates how many parcels were already Received
                                CountReceived++;
                            if (p.Status == Status.Picked) // calculates how many parcels are on their way
                                CountParcelOnWay++;
                        }
                        else if (p.SenderName == item.Name)
                        {
                            if (p.Status == Status.Delivered) // calculates how many parcels were sent and delivered
                                CountSentAndDelivered++;
                            if (p.Status == Status.Picked) //calculates how many parcels were sent but not delivered
                                CountSentNotDelivered++;
                        }
                    }
                    result.Add(new ClientForList //add the client from dal to Bl to the Bl list of clients
                    {
                        Id = item.ID,
                        Name = item.Name,
                        Phone = item.Phone,
                        SentAndDelivered = CountSentAndDelivered,
                        SentNotDelivered = CountSentNotDelivered,
                        ParcelOnWay = CountParcelOnWay,
                        Received = CountReceived
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// update the name and or the phone of a given client 
        /// </summary>
        /// <param name="ClientId">if of the client</param>
        /// <param name="name">name of the client to update</param>
        /// <param name="phone">phone of the client to update</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateClientName(int ClientId, string name = "", string phone = "")
        {
            if (ClientId < 100000000 || ClientId > 999999999)
            {
                throw new IDException("Id is not Valid");
            }
            DO.Client tempC;
            try
            {
                lock (myDal)
                {
                    tempC = myDal.GetClient(ClientId);
                    myDal.UpdateClientName(ClientId, name, phone);
                }
            }
            catch (DO.DoesNotExist ExC)
            {
                throw new BLDoesNotExist(ExC.Message);
            }
        }

        /// <summary>
        /// delete a client from the clients list
        /// </summary>
        /// <param name="client">the given client tot remove</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveClient(Client client)
        {
            DO.Client temp = new DO.Client()
            {
                ID = client.Id,
                Longitude = client.Location.Longitude,
                Latitude = client.Location.Latitude,
                Name = client.Name,
                Phone = client.Phone
            };
            try
            {
                lock (myDal)
                {
                    myDal.clearClient(temp);
                }
            }
            catch (DO.DoesNotExist Ex)
            {
                throw new BLDoesNotExist(Ex.Message);
            }
        }
    }
}