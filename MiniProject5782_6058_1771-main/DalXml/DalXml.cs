using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

namespace Dal
{
    sealed partial class DalXml : IDal //singelton class
    {
        private static DalXml instance = null;
        private static readonly object padlock = new object();
        public static DalXml Instance
        {
            get
            {
                if (instance == null)
                    lock (padlock)
                    {
                        if (instance == null)
                            instance = new DalXml();
                    }
                return instance;
            }
        }

        readonly string StationPath; //variable for saving Station.xml
        readonly string ClientPath; //variable for saving Client.xml
        readonly string DronePath; //variable for saving Drone.xml
        readonly string ParcelPath; //variable for saving Parcel.xml
        readonly string DroneChargePath; //variable for saving DroneCharge.xml
        readonly string ConfigPath;

        #region Constructor
        DalXml()
        {
            //inisializing the path with the files
            DronePath = @"Drone.xml"; 
            StationPath = @"Station.xml";
            ClientPath = @"Client.xml";
            ParcelPath = @"Parcel.xml";
            DroneChargePath = @"DroneCharge.xml";
            ConfigPath = @"config.xml";
        }
        #endregion


        #region Save and Load With XElement
        /// <summary>
        /// Outputs the XElement entered to the specified Stream.
        /// </summary>
        /// <param name="rootElem">the type of root entered</param>
        /// <param name="filePath">the path enetered</param>
        public static void SaveListToXMLElement(XElement rootElem, string filePath)
        {
            try
            {
                rootElem.Save(filePath); //save the path entered to the root entered
            }
            catch (Exception)
            {
                throw new DO.XmlFileCreationFailException($"Failed to create XML file {filePath}");
            }
        }

        /// <summary>
        /// A static function that returns a XElement who's structure is Identical to the xml file found in the path entered
        /// </summary>
        /// <param name="filePath">the path enetered</param>
        /// <returns></returns>
        public static XElement LoadListFromXMLElement(string filePath)
        {
            try
            {
                if (File.Exists(filePath)) //if the file from the path entered exist (was created)
                {
                    return XElement.Load(filePath); //loads XElement from the file and returns the data
                }
                else
                {
                    XElement rootElem = new XElement(filePath); //create a root XElement according to the path entered
                    rootElem.Save(filePath); //save the path entered to the root entered
                    return rootElem;
                }
            }
            catch (Exception)
            {
                throw new DO.XmlFileCreationFailException($"Failed to create XML file {filePath}");
            }
        }
        #endregion

        #region Save and Load With XMLSerializer
        /// <summary>
        /// XML serialization enables an object's public fields and properties to be saved and loaded to an XML file.
        /// </summary>
        /// <typeparam name="T">the type of list entered</typeparam>
        /// <param name="list"> the list entered</param>
        /// <param name="filePath">the path of the file entered</param>
        public static void SaveListToXMLSerializer<T>(List<T> list, string filePath)
        {
            try
            {
                FileStream file = new FileStream(filePath, FileMode.Create);  //Create a file with the path entered
                XmlSerializer x = new XmlSerializer(list.GetType()); //Creates an instance of the XmlSerializer class and specifies the type of the list to serialize.
                x.Serialize(file, list); //The Serialize method is used to serialize an object to XML, the list to the file.
                file.Close(); //close the file
            }
            catch (Exception)
            {
                throw new DO.XmlFileCreationFailException(filePath);
            }
        }

        /// <summary>
        /// XML serialization enables an object's public fields and properties to be saved and loaded to/from an XML file.
        /// </summary>
        /// <typeparam name="T">the type of list entered</typeparam>
        /// <param name="filePath">the path of the file entered</param>
        /// <returns></returns>
        public static List<T> LoadListFromXMLSerializer<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    List<T> list;
                    XmlSerializer x = new XmlSerializer(typeof(List<T>)); //Creates an instance of the XmlSerializer class and specifies the type of the list to serialize according to the type entered
                    FileStream file = new FileStream(filePath, FileMode.Open);  //Open the file with the path entered
                    list = (List<T>)x.Deserialize(file); //deserialization is the reversed process of serialization. get back the serialized object so that it can be loaded into memory.
                    file.Close(); //close the file
                    return list;
                }
                else
                    return new List<T>();
            }
            catch (Exception)
            {
                throw new DO.XmlFileCreationFailException(filePath);
            }
        }
        #endregion
    }
}


