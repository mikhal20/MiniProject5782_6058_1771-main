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
        XElement StationRoot = LoadListFromXMLElement(@"Station.xml");

        /// <summary>
        ///  return the DO station from the list of parcels
        /// </summary>
        /// <param name="StationID"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station GetStation(int StationID)
        {
            Station station = default;//create a Station
            var StationList = LoadListFromXMLSerializer<Station>(StationPath);//load the list of stations in StationList
            if (StationList.Exists(s => s.ID == StationID))//if the station entered exist in the list
            {
                int num = StationList.FindIndex(s => s.ID == StationID);
                station = StationList[num];//we found it thanks to the id and save the value in station
            }
            else
                throw new XmlDoesntExistException($"Station {StationID} Doesn't exist");//exception if the station entered doesn't exist
            return station;
        }

        /// <summary>
        ///  return the list of stations
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> showStations(Func<Station, bool> predicate = null)
        {
            if (predicate != null)//if there is a condition
                return LoadListFromXMLSerializer<Station>(StationPath).Where(x => predicate(x));//we return the list of stations which respond to the condition
            else //if there is NO condition
                return LoadListFromXMLSerializer<Station>(StationPath);//we returns all the stations in the list
        }

        /// <summary>
        /// add a station to the list of stations
        /// </summary>
        /// <param name="s"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void addStation(Station s)
        {
            var stationList = LoadListFromXMLSerializer<Station>(StationPath);//load the list into stationList
            if (stationList.Exists(station => station.ID == s.ID))//if the station already exist we throw an exception cause we can't add it
                throw new XmlAlreadyExistException($"station {s.ID} already exist");
            stationList.Add(s);//otherwise we add it
            SaveListToXMLSerializer(stationList, StationPath); //save the updated list in the file
        }

        /// <summary>
        /// remove a station from the list
        /// </summary>
        /// <param name="s"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void clearStation(Station s)
        {
            var stationList = LoadListFromXMLSerializer<Station>(StationPath);//load the list into stationList
            if (stationList.Exists(station => station.ID == s.ID))//if the station entered exist in the list
            {
                stationList.Remove(s); //remove it
                SaveListToXMLSerializer(stationList, StationPath);//save the updated list in the file
            }
            else//if it doesn't exist we throw an exception cause we can't delete it
                throw new XmlDoesntExistException($"Station {s.ID} doesn't exist");
        }

        /// <summary>
        /// update the list with the updated station
        /// </summary>
        /// <param name="s"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStation(Station s) //update the station in the list of stations
        {
            var stationList = LoadListFromXMLSerializer<Station>(StationPath);//load the list into stationList
            if (stationList.Exists(station => station.ID == s.ID))//if the station exists in the list according to the updated station's id
            {
                int num = stationList.FindIndex(station => station.ID == s.ID);//we find its place in the list
                stationList[num] = s;//and update the list with the updated station
            }
            else//if the station was not found we throw an exception cause we cant update something that doesn't exist
                throw new XmlDoesntExistException($"Station {s.ID} doesn't exist");
            SaveListToXMLSerializer(stationList, StationPath);//save the updated list in the file
        }

        /// <summary>
        /// Update the name and/or the number of available chargeslots of a given station
        /// </summary>
        /// <param name="s"></param>
        /// <param name="name"></param>
        /// <param name="num"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStationName(Station s, string name, string num)
        {
            var stationList = LoadListFromXMLSerializer<Station>(StationPath);//load the list into stationList
            if (stationList.Exists(station => station.ID == s.ID))//if the station exists in the list according to the updated station's id
            {
                int index = stationList.FindIndex(station => station.ID == s.ID);//saves the index of the station with the updated station'id
                s.Name = name;//changes the name of the given station to the given name
                s.ChargeSlots = Convert.ToInt32(num);//changes the number of chargslots of the given station to the given number
                stationList[index] = s;//add to the list of stations the updated station in the found index 
            }
            else//if the station was not found we throw an exception cause we cant update something that doesn't exist
                throw new XmlDoesntExistException($"Station {s.ID} doesn't exist");
            SaveListToXMLSerializer(stationList, StationPath);//save the updated list in the file
        }
    }
}