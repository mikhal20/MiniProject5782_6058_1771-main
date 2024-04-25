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
    partial class DalXml //using xml Serializer
    {
        XElement ParcelRoot = LoadListFromXMLElement(@"Parcel.xml");
        /// <summary>
        /// return the DO parcel from the list of parcels
        /// </summary>
        /// <param name="ParcelID">the id of the parcel</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Parcel GetParcel(int ParcelID)
        {
            Parcel parcel = default; //create a Parcel
            var parcelList = LoadListFromXMLSerializer<Parcel>(ParcelPath); //load the list of parcels in parcelList
            if (parcelList.Exists(p => p.ID == ParcelID)) //if the parcel entered exist in the list
            {
                int num = parcelList.FindIndex(p => p.ID == ParcelID);
                parcel = parcelList[num]; //we found it thanks to the id and save the value in parcel
            }
            else
                throw new XmlDoesntExistException($"Parcel {ParcelID} doesn't exist"); //exception if the parel entered doesn't exist
            return parcel;
        }
        /// <summary>
        /// return the list of parcels
        /// </summary>
        /// <param name="predicate"> a condition to get only some of the parcels </param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Parcel> showParcels(Func<Parcel, bool> predicate = null)
        {
            if (predicate != null) //if there is a condition
                return LoadListFromXMLSerializer<Parcel>(ParcelPath).Where(x => predicate(x)); //we return the list of parcels which respond to the condition
            else //if there is NO condition
                return LoadListFromXMLSerializer<Parcel>(ParcelPath); //we returns all the parcels in the list
        }
        /// <summary>
        /// add a parcel to the list of parcels
        /// </summary>
        /// <param name="p"> is the parcel to add to the list</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void addParcel(Parcel p)
        {
            List<string> config = LoadListFromXMLSerializer<string>(ConfigPath); //load data from the config
            p.ID = int.Parse(config[5]); //take the next running number for the parcel
            int newConfigValue = int.Parse(config[5]) + 1; //create a int object to the running number in order to add one
            config[5] = newConfigValue.ToString(); //update the running number 
            SaveListToXMLSerializer(config, ConfigPath); //save the new list of config with the updated running number of the parcel
           
            var parcelList = LoadListFromXMLSerializer<Parcel>(ParcelPath); //load the list into parcelList
            if (parcelList.Exists(parcel => parcel.ID == p.ID)) //if the parcel already exist we throw an exception cause we can't add it
                throw new XmlAlreadyExistException($"Parcel {p.ID} already exist");
            parcelList.Add(p); //otherwise we add it
            SaveListToXMLSerializer(parcelList, ParcelPath); //save the updated list in the file
        }
        /// <summary>
        /// remove a parcel from the list
        /// </summary>
        /// <param name="p">is the parcel to remove</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void clearParcel(Parcel p)
        {
            LoadListFromXMLElement(ParcelPath);
            XElement ParcelElement;
            try
            {
                ParcelElement = (from parcel in ParcelRoot.Elements()
                                      where Convert.ToInt32(parcel.Element("ID").Value) == p.ID
                                      select parcel).FirstOrDefault(); //find the dronecharge to remove
                if (ParcelElement == null)
                    throw new XmlDoesntExistException($"Parcel {p.ID} doesn't exist");
                ParcelElement.Remove(); //remove it from the list
                SaveListToXMLElement(ParcelRoot, ParcelPath); //save-update the new list
            }
            catch { }
        }
        /// <summary>
        /// update the list with the updated parcel
        /// </summary>
        /// <param name="p"> is the undated parcel </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateParcel(Parcel p) //update the parcel in the list of parcels
        {
            var parcelList = LoadListFromXMLSerializer<Parcel>(ParcelPath); //load the list into parcelList
            if (parcelList.Exists(parcel => parcel.ID == p.ID)) //if the parcel exist in the list according to the updated parcel's id
            {
                int num = parcelList.FindIndex(parcel => parcel.ID == p.ID); //we find its place in the list
                parcelList[num] = p; //and update the list with the updated parcel
            }
            else //if the parcel was not found we throw an exception cause we cant update something that doesn't exist
               throw new XmlDoesntExistException($"Parcel {p.ID} doesn't exist");
            SaveListToXMLSerializer(parcelList, ParcelPath); //save the updated list in the file
        }
    }
}