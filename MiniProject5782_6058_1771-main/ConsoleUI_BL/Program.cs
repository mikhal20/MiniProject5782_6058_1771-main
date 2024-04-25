////mikhal Levy: 332381771
////shaili benloulou: 328606058
////mini Project exercise 2
using BO;
using System;
using System.Collections.Generic;

namespace ConsoleUI_BL
{
    class Program
    {
        static void Main(string[] args)
        {
            BlApi.IBL bl = BlApi.BlFactory.GetBl();
            Main(bl);
        }
        static void Main(BlApi.IBL bl)
        {
            try
            {
                Console.WriteLine("please choose one of the following possibilities:\n" +
                    "1: add an object\n2: update an object\n3: display an object\n4: display a list of objects\n5: exit\n");
                int choice = int.Parse(Console.ReadLine());
                while (choice != 5) //exit when the choice is 5
                {
                    int weight;
                    Station tempS;
                    Drone tempD;
                    Parcel tempP;
                    Client tempC;
                    switch (choice)
                    {
                        case 1: // allow to add an object
                            Console.WriteLine("1: stations \n2: drones\n3: clients\n4: parcels \n");
                            int addChoice = int.Parse(Console.ReadLine());
                            switch (addChoice)
                            {
                                case 1: // add a Station to the list of stations
                                    Console.WriteLine("please enter id, name, longitude, latitude and number of chargeSlots\n");
                                    double longi, lati;
                                    tempS = new Station();
                                    tempS.Id = int.Parse(Console.ReadLine());
                                    tempS.Name = Console.ReadLine();
                                    double.TryParse(Console.ReadLine(), out longi);
                                    double.TryParse(Console.ReadLine(), out lati);
                                    Location l = new Location();
                                    l.Longitude = longi;
                                    l.Latitude = lati;
                                    tempS.Location = l;
                                    tempS.ChargeSlots = int.Parse(Console.ReadLine());
                                    tempS.ListDroneCharge = new List<DroneCharge>();
                                    try
                                    {
                                        bl.AddStation(tempS);
                                    }
                                    catch (Exception Ex)
                                    {
                                        Console.WriteLine(Ex);
                                    }
                                    break;
                                case 2: // add a Drone to the list of drones
                                    Console.WriteLine("Please enter ID, model, maximum Weight (0-low, 1-medium, 2-heavy) and station's id\n");
                                    tempD = new Drone();
                                    tempD.ID = int.Parse(Console.ReadLine());
                                    tempD.Model = Console.ReadLine();
                                    weight = int.Parse(Console.ReadLine());
                                    if (weight == 0)
                                        tempD.Weight = WeightCategories.Low;
                                    else if (weight == 1)
                                        tempD.Weight = WeightCategories.Middle;
                                    else
                                        tempD.Weight = WeightCategories.Heavy;
                                    int stationId = int.Parse(Console.ReadLine());
                                    bl.AddDrone(tempD, stationId);
                                    break;
                                case 3: // add a Client tp the list of clients
                                    Console.WriteLine("Please enter ID, name ,phone number and location of the client\n");
                                    tempC = new Client();
                                    tempC.Id = int.Parse(Console.ReadLine());
                                    tempC.Name = Console.ReadLine();
                                    tempC.Phone = Console.ReadLine();
                                    tempC.Location.Longitude = double.Parse(Console.ReadLine());
                                    tempC.Location.Latitude = double.Parse(Console.ReadLine());
                                    bl.AddClient(tempC);
                                    break;
                                case 4: // add a Parcel in the list of parcels
                                    Console.WriteLine("Please enter targetId, SenderId, Weight (0-low, 1-medium, 2-heavy) and priority (0-Regular, 1-Fast, 2-Emergency)\n");
                                    tempP = new Parcel();
                                    tempP.Sender.Id = int.Parse(Console.ReadLine());
                                    tempP.Recipient.Id = int.Parse(Console.ReadLine());
                                    weight = int.Parse(Console.ReadLine());
                                    if (weight == 0)
                                        tempP.Weight = WeightCategories.Low;
                                    else if (weight == 1)
                                        tempP.Weight = WeightCategories.Middle;
                                    else
                                        tempP.Weight = WeightCategories.Heavy;
                                    int priority = int.Parse(Console.ReadLine());
                                    if (priority == 0)
                                        tempP.Priority = Priorities.Regular;
                                    else if (priority == 1)
                                        tempP.Priority = Priorities.Fast;
                                    else
                                        tempP.Priority = Priorities.Emergency;
                                    bl.AddParcel(tempP);
                                    break;
                            }
                            break;

                        case 2://allow to update an object
                            Console.WriteLine("1: Update drone's name\n2: update station's details\n3: update client's details\n4: send drone to charge\n5: stop charging\n" +
                                "6: Assign parcel to drone\n7: pickup parcel with drone\n8: deliver parcel with drone\n");
                            int updateChoice = int.Parse(Console.ReadLine());//gets what the user wanting to update

                            switch (updateChoice)
                            {
                                case 1:
                                    Console.WriteLine("enter drone's id and Model\n");
                                    int droneId = int.Parse(Console.ReadLine());
                                    string model = Console.ReadLine();
                                    bl.UpdateDroneName(droneId, model);
                                    break;
                                case 2:
                                    Console.WriteLine("enter station's id and name or number of charge slots\n");
                                    int StationId = int.Parse(Console.ReadLine());
                                    string StationName = Console.ReadLine();
                                    string num = Console.ReadLine();
                                    bl.UpdateStationName(StationId, StationName, num);
                                    break;
                                case 3:
                                    Console.WriteLine("enter client's id and name or phone\n");
                                    int Id = int.Parse(Console.ReadLine());
                                    string name = " ";
                                    name = Console.ReadLine();
                                    string phone = " ";
                                    phone = Console.ReadLine();
                                    bl.UpdateClientName(Id, name, phone);
                                    break;
                                case 4:
                                    Console.WriteLine("enter drone's id\n");
                                    int DroneId = int.Parse(Console.ReadLine());
                                    bl.SendDroneToCharge(DroneId);
                                    break;
                                case 5:
                                    Console.WriteLine("enter drone's id\n");
                                    DroneId = int.Parse(Console.ReadLine());
                                    double time = double.Parse(Console.ReadLine());
                                    bl.FreeCharge(DroneId, time);
                                    break;
                                case 6:
                                    Console.WriteLine("enter drone's id\n");
                                    DroneId = int.Parse(Console.ReadLine());
                                    bl.AssignParcelToDrone(DroneId);
                                    break;
                                case 7:
                                    Console.WriteLine("enter drone's id\n");
                                    DroneId = int.Parse(Console.ReadLine());
                                    bl.PickUpParcelWithDrone(DroneId);
                                    break;
                                case 8:
                                    Console.WriteLine("enter drone's id\n");
                                    DroneId = int.Parse(Console.ReadLine());
                                    bl.DeliverParcelWithDrone(DroneId);
                                    break;
                            }
                            break; //exit

                        case 3: //print one specific object
                            Console.WriteLine("1: station \n2: drone\n3: client\n4: parcel\n");
                            int showChoice = int.Parse(Console.ReadLine());
                            Console.WriteLine("please enter ID\n");
                            int id = int.Parse(Console.ReadLine());
                            switch (showChoice)
                            {
                                case 1:
                                    Console.WriteLine(bl.GetBlStation(id));
                                    break;
                                case 2:
                                    Console.WriteLine(bl.GetBlDrone(id));
                                    break;
                                case 3:
                                    Console.WriteLine(bl.GetBlClient(id));
                                    break;
                                case 4:
                                    Console.WriteLine(bl.GetBlParcel(id));
                                    break;
                            }
                            break; //exit

                        case 4: //print the list of the object entered
                            Console.WriteLine("1: station's list\n2: drone's list\n3: client's list\n4: parcel's list\n" +
                                "5: list of parcels without drone\n6: Stations with available chargers\n");
                            int ChoiceList = int.Parse(Console.ReadLine());
                            switch (ChoiceList)
                            {
                                case 1:
                                    foreach (var item in bl.GetStationList())
                                    {
                                        Console.WriteLine(item);
                                    }
                                    break;

                                case 2:
                                    foreach (var item in bl.GetDronesList())
                                    {
                                        Console.WriteLine(item);
                                    }
                                    break;

                                case 3:
                                    foreach (var item in bl.GetClientList())
                                    {
                                        Console.WriteLine(item);
                                    }
                                    break;
                                case 4:
                                    foreach (var item in bl.GetParcelList())
                                    {
                                        Console.WriteLine(item);
                                    }
                                    break;
                                case 5:
                                    foreach (var item in bl.showParcelWithoutDrone())
                                    {
                                        Console.WriteLine(item);
                                    }
                                    break;
                                case 6:
                                    foreach (var item in bl.showStationWithCharge())
                                    {
                                        Console.WriteLine(item);
                                    }
                                    break;
                            }
                            break; //exit
                    }
                    Console.WriteLine("please choose one of the following possibilities:\n" +
                      "1: add an object\n2: update an object\n3: display an object\n4: display a list of objects\n5: exit\n");
                    choice = int.Parse(Console.ReadLine());
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
            }
        }
    }
}