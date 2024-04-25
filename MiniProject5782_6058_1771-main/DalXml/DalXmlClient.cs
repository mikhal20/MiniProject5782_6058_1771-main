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
        XElement ClientRoot = LoadListFromXMLElement(@"Client.xml");

        /// <summary>
        /// return the client according to the client's id entered
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Client GetClient(int ClientID)
        {
            Client client = default;//create a Client
            var ClientList = LoadListFromXMLSerializer<Client>(ClientPath);//load the list of clients in ClientList
            if (ClientList.Exists(c => c.ID == ClientID))//if the client entered exist in the list
            {
                int num = ClientList.FindIndex(c => c.ID == ClientID);
                client = ClientList[num];//we found it thanks to the id and save the value in client
            }
            else
                throw new XmlDoesntExistException($"Client {ClientID} Doesn't exist");//exception if the client entered doesn't exist
            return client;
        }

        /// <summary>
        /// returns the list of clients
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Client> showClients()
        {
           return LoadListFromXMLSerializer<Client>(ClientPath);//return the loaded list of clients
        }

        /// <summary>
        /// add a client to the list of clients
        /// </summary>
        /// <param name="c"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void addClient(Client c) 
        {
            var ClientList = LoadListFromXMLSerializer<Client>(ClientPath);//load the list into ClientList
            if (ClientList.Exists(client => client.ID == c.ID))//check if the client doesn't already exist.
                throw new XmlAlreadyExistException($"Client {c.ID} Already exist.");
            //save the client's data into XElements
            XElement Id = new XElement("ID", c.ID);
            XElement Name = new XElement("Name", c.Name);
            XElement Phone = new XElement("Phone", c.Phone);
            XElement Latitude = new XElement("Latitude", c.Latitude);
            XElement Longitude = new XElement("Longitude", c.Longitude);
            ClientRoot.Add(new XElement("Client", Id, Name, Phone, Latitude, Longitude));//add the client to the list of clients
            SaveListToXMLElement(ClientRoot, ClientPath);//save-update the new list
        }

        /// <summary>
        /// remove a client from the list of clients
        /// </summary>
        /// <param name="c"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void clearClient(Client c)
        {
            LoadListFromXMLElement(ClientPath);//load the list
            XElement ClientElement;
            try
            {
                ClientElement = (from client in ClientRoot.Elements()
                                 where Convert.ToInt32(client.Element("ID").Value) == c.ID
                                 select client).FirstOrDefault();//find the client thanks to the id
                if (ClientElement == null)
                    throw new XmlDoesntExistException($"Client {c.ID} doesn't exist");
                ClientElement.Remove();//remove the client from the list
                SaveListToXMLElement(ClientRoot, ClientPath);//save-update the new list
            }
            catch { }
        }

        /// <summary>
        ///  update the name and or the phone of a given client 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateClientName(int id, string name, string phone)
        {
            LoadListFromXMLElement(ClientPath);//load the list 
            XElement ClientElement = (from client in ClientRoot.Elements()
                                      where Convert.ToInt32(client.Element("ID").Value) == id
                                      select client).FirstOrDefault();//find the client thanks to the id
            if (ClientElement == null)//if there is no client with such an id throw an exception
                throw new XmlDoesntExistException($"Client {id} doesn't exist");
            ClientElement.Element("Name").Value = name;//update the name 
            ClientElement.Element("Phone").Value = phone;//update the phone
            SaveListToXMLElement(ClientRoot, ClientPath);//save-update the new list
        }
    }
}
