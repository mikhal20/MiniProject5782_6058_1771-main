//mikhal Levy: 332381771
//shaili benloulou: 328606058
//mini Project exercise 1
using DO;
using Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUI
{
    class Program
    {
        static DalApi.IDal mydal = DalApi.DalFactory.GetDal(); //call to the DalObject's constructor who call to inisialize and create the objects.

        static void Main(string[] args)
        {
            Console.WriteLine("please choose one of the following possibilities:\n" +
                "1: add an object\n2: update an object\n3: display an object\n4: display a list of objects\n5: exit\n");
            int choice = int.Parse(Console.ReadLine());
            while (choice != 5) //exit when the choice is 5
            {
                int parcelId;
                int droneId;
                int stationId;
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
                                Console.WriteLine("please enter the id, name, longitude, latitude and chargeslotes of the station \n");
                                tempS = new Station();
                                tempS.ID = int.Parse(Console.ReadLine());
                                tempS.Name = Console.ReadLine();
                                tempS.Longitude = double.Parse(Console.ReadLine());
                                tempS.Latitude = double.Parse(Console.ReadLine());
                                tempS.ChargeSlots = int.Parse(Console.ReadLine());
                                mydal.addStation(tempS); //call to the function to add the station's details entered
                                break;
                            case 2: // add a Drone to the list of drones
                                Console.WriteLine("Please enter ID, model, maximum Weight (0-low, 1-medium, 2-heavy) and battery");
                                tempD = new Drone();
                                tempD.ID = int.Parse(Console.ReadLine());
                                tempD.Model = Console.ReadLine();
                                int weight = int.Parse(Console.ReadLine());
                                if (weight == 0)
                                    tempD.Weight = WeightCategories.Low;
                                else if (weight == 1)
                                    tempD.Weight = WeightCategories.Middle;
                                else
                                    tempD.Weight = WeightCategories.Heavy;
                                mydal.addDrone(tempD); //call to the function to add the drone's details entered
                                break;
                            case 3: // add a Client to the list of clients
                                Console.WriteLine("please enter id, name, phone, longitude and latitude of the client\n");
                                tempC = new Client();
                                tempC.ID = int.Parse(Console.ReadLine());
                                tempC.Name = Console.ReadLine();
                                tempC.Phone = Console.ReadLine();
                                tempC.Longitude = double.Parse(Console.ReadLine());
                                tempC.Latitude = double.Parse(Console.ReadLine());
                                mydal.addClient(tempC); //call to the function to add the client's details entered
                                break;
                            case 4: // add a Parcel in the list of parcels
                                Console.WriteLine("Enter sender's ID, target's ID, parcel weight (0-low,1-mediun, 2-heavy) " +
                                    "and priority (0-regular, 1-express, 2-urgent) of the parcel:\n");
                                tempP = new Parcel();
                                tempP.SenderId = int.Parse(Console.ReadLine());
                                tempP.TargetId = int.Parse(Console.ReadLine());
                                int PWeight = int.Parse(Console.ReadLine());
                                int Ppriority = int.Parse(Console.ReadLine());
                                if (PWeight == 0)
                                    tempP.Weight = WeightCategories.Low;
                                else if (PWeight == 1)
                                    tempP.Weight = WeightCategories.Middle;
                                else
                                    tempP.Weight = WeightCategories.Heavy;
                                if (Ppriority == 0)
                                    tempP.Priority = Priorities.Regular;
                                else if (Ppriority == 1)
                                    tempP.Priority = Priorities.Fast;
                                else
                                    tempP.Priority = Priorities.Emergency;
                                mydal.addParcel(tempP); //call to the function to add the parcel's details entered
                                break;
                        }
                        break;

                    case 2://allow to update an object
                        Console.WriteLine("1: assign parcel\n2: pick up parcel\n3: deliver parcel\n4: send drone to charge\n5: stop charging\n");
                        int updateChoice = int.Parse(Console.ReadLine());//gets what the user wanting to update

                        switch (updateChoice)
                        {
                            case 1: //allow to assign parcel to a drone by changing the droneId of the getted parcel
                                Console.WriteLine("enter parcel and drone's ids");
                                parcelId = int.Parse(Console.ReadLine());
                                droneId = int.Parse(Console.ReadLine());
                                tempP = mydal.GetParcel(parcelId);
                                tempD = mydal.GetDrone(droneId);
                                mydal.Assign(tempP, tempD); //calls to the function assign from dalobject
                                break;
                            case 2://allow to pick up parcel
                                Console.WriteLine("enter parcel's id");
                                parcelId = int.Parse(Console.ReadLine());//get the parcel's id
                                tempP = mydal.GetParcel(parcelId);
                                mydal.PickUp(tempP); //calls to the function pickUp from DalObject by sending to it the getted id
                                break;
                            case 3://allows to deliver parcel
                                Console.WriteLine("enter parcel's id");
                                parcelId = int.Parse(Console.ReadLine());
                                tempP = mydal.GetParcel(parcelId);
                                mydal.parcelDelivery(tempP);//calls to the function parcelDelivery from DalObject by sending to it the getted id
                                break;
                            case 4://allow to send drone to charge
                                Console.WriteLine("enter drone's id\n");
                                droneId = int.Parse(Console.ReadLine());
                                Console.WriteLine("enter a station ID from one of the following stations\n");
                                foreach (Station s in mydal.showStations(x => x.ChargeSlots != 0))
                                {
                                    Console.WriteLine(s);
                                }
                                //present the list of the stations to allow to the user to choose a station
                                stationId = int.Parse(Console.ReadLine());

                                DroneCharge drc = new DroneCharge();//create a  new object of droneCharge and put to it the getted value
                                drc.DroneId = droneId;
                                drc.StationId = stationId;
                                tempD = mydal.GetDrone(droneId);
                                tempS = mydal.GetStation(stationId);
                                mydal.sendToCharge(tempS, tempD);//call to sendToCharge from dalobject
                                break;
                            case 5://allow to stop to charge a drone
                                Console.WriteLine("enter drone's id and station's id\n");
                                //droneId = int.Parse(Console.ReadLine());
                                stationId = int.Parse(Console.ReadLine());
                                tempS = mydal.GetStation(stationId);
                                mydal.freeCharge(tempS);//call freeCharge from dalObjct 
                                // mydal.ChangeStatus(droneId,DroneStatuses.free);//call to ChangeStatus to change the status of this drone to free
                                break;
                        }
                        break; //exit

                    case 3: //print one specific object
                        Console.WriteLine("1: station \n2: drone\n3: client\n4: parcel\n");
                        int showChoice = int.Parse(Console.ReadLine());
                        Console.WriteLine("please enter ID\n");
                        int id;
                        int.TryParse(Console.ReadLine(), out id);
                        mydal.showOptions(id, showChoice);
                        break; //exit

                    case 4: //print the list of the object entered
                        Console.WriteLine("1: station's list\n2: drone's list\n3: client's list\n4: parcel's list\n" +
                            "5: list of parcels without drone\n6: Stations with available chargers\n");
                        int listChoice;
                        int.TryParse(Console.ReadLine(), out listChoice);
                        switch (listChoice)
                        {
                            case 1:
                                foreach (Station s in mydal.showStations())
                                {
                                    Console.WriteLine(s);
                                }
                                break;
                            case 2:
                                foreach (Drone d in mydal.showDrones())
                                {
                                    Console.WriteLine(d);
                                }
                                break;
                            case 3:
                                foreach (Client c in mydal.showClients())
                                {
                                    Console.WriteLine(c);
                                }
                                break;
                            case 4:
                                foreach (Parcel p in mydal.showParcels())
                                {
                                    Console.WriteLine(p);
                                }
                                break;
                            case 5:
                                foreach (Parcel p in mydal.showParcels(x => x.DroneId == 0))
                                {
                                    Console.WriteLine(p);
                                }
                                break;
                            case 6:
                                foreach (Station s in mydal.showStations(x => x.ChargeSlots != 0))
                                {
                                    Console.WriteLine(s);
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
    }
}

/* please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

4
1: station's list
2: drone's list
3: client's list
4: parcel's list
5: list of parcels without drone
6: Stations with available chargers

1
station: 3762608,
Name: Central Station,
Location: 37° 0' 1392,1800000000103'' W 37° 0' 1392,1800000000103'' W
ChargeSlots: 28

station: 4780293,
Name: Malcha Mall,
Location: 31° 0' 4500,719999999994'' E 31° 0' 4500,719999999994'' E
ChargeSlots: 66

please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

1
1: stations
2: drones
3: clients
4: parcels

1
please enter the id, name, longitude, latitude and chargeslotes of the station

111
newstat
31,3
32,4
23
please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

4
1: station's list
2: drone's list
3: client's list
4: parcel's list
5: list of parcels without drone
6: Stations with available chargers

1
station: 3762608,
Name: Central Station,
Location: 37° 0' 1392,1800000000103'' W 37° 0' 1392,1800000000103'' W
ChargeSlots: 28

station: 4780293,
Name: Malcha Mall,
Location: 31° 0' 4500,719999999994'' E 31° 0' 4500,719999999994'' E
ChargeSlots: 66

station: 111,
Name: newstat,
Location: 32° 0' 2399,9999999999914'' E 32° 0' 2399,9999999999914'' E
ChargeSlots: 23

please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

4
1: station's list
2: drone's list
3: client's list
4: parcel's list
5: list of parcels without drone
6: Stations with available chargers

4
Parcel's Id: 10000000,
Sender's Id: 5815167,
Target's Id: 8699584,
Drone's Id: 4951649,
Weight: heavy,
Priority: emergency,
Requested time: 28/10/2021 22:10:49
Scheduled time: 28/10/2021 22:10:49
PickedUp time: 28/10/2021 22:10:49
Delivered time: 28/10/2021 22:10:49

Parcel's Id: 10000001,
Sender's Id: 2833282,
Target's Id: 8860530,
Drone's Id: 6713537,
Weight: heavy,
Priority: emergency,
Requested time: 28/10/2021 22:10:49
Scheduled time: 28/10/2021 22:10:49
PickedUp time: 28/10/2021 22:10:49
Delivered time: 28/10/2021 22:10:49

Parcel's Id: 10000002,
Sender's Id: 6396426,
Target's Id: 3874459,
Drone's Id: 6594181,
Weight: low,
Priority: emergency,
Requested time: 28/10/2021 22:10:49
Scheduled time: 28/10/2021 22:10:49
PickedUp time: 28/10/2021 22:10:49
Delivered time: 28/10/2021 22:10:49

Parcel's Id: 10000003,
Sender's Id: 2128880,
Target's Id: 3502756,
Drone's Id: 3874374,
Weight: middle,
Priority: regular,
Requested time: 28/10/2021 22:10:49
Scheduled time: 28/10/2021 22:10:49
PickedUp time: 28/10/2021 22:10:49
Delivered time: 28/10/2021 22:10:49

Parcel's Id: 10000004,
Sender's Id: 4943391,
Target's Id: 6142328,
Drone's Id: 1931954,
Weight: low,
Priority: regular,
Requested time: 28/10/2021 22:10:49
Scheduled time: 28/10/2021 22:10:49
PickedUp time: 28/10/2021 22:10:49
Delivered time: 28/10/2021 22:10:49

Parcel's Id: 10000005,
Sender's Id: 6714919,
Target's Id: 1672901,
Drone's Id: 2332056,
Weight: middle,
Priority: emergency,
Requested time: 28/10/2021 22:10:49
Scheduled time: 28/10/2021 22:10:49
PickedUp time: 28/10/2021 22:10:49
Delivered time: 28/10/2021 22:10:49

Parcel's Id: 10000006,
Sender's Id: 8423257,
Target's Id: 4509642,
Drone's Id: 3362554,
Weight: heavy,
Priority: fast,
Requested time: 28/10/2021 22:10:49
Scheduled time: 28/10/2021 22:10:49
PickedUp time: 28/10/2021 22:10:49
Delivered time: 28/10/2021 22:10:49

Parcel's Id: 10000007,
Sender's Id: 1192792,
Target's Id: 5107329,
Drone's Id: 1804531,
Weight: middle,
Priority: fast,
Requested time: 28/10/2021 22:10:49
Scheduled time: 28/10/2021 22:10:49
PickedUp time: 28/10/2021 22:10:49
Delivered time: 28/10/2021 22:10:49

Parcel's Id: 10000008,
Sender's Id: 2260470,
Target's Id: 4299438,
Drone's Id: 2814207,
Weight: middle,
Priority: fast,
Requested time: 28/10/2021 22:10:49
Scheduled time: 28/10/2021 22:10:49
PickedUp time: 28/10/2021 22:10:49
Delivered time: 28/10/2021 22:10:49

Parcel's Id: 10000009,
Sender's Id: 571347,
Target's Id: 1207484,
Drone's Id: 6053996,
Weight: middle,
Priority: regular,
Requested time: 28/10/2021 22:10:49
Scheduled time: 28/10/2021 22:10:49
PickedUp time: 28/10/2021 22:10:49
Delivered time: 28/10/2021 22:10:49

please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

1
1: stations
2: drones
3: clients
4: parcels

2
Please enter ID, model, maximum Weight (0-low, 1-medium, 2-heavy) and battery
123456789
kH8jb12
0
45,8
please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

3
1: station
2: drone
3: client
4: parcel

2
please enter ID

123456789
Drone's Id: 123456789,
Model: kH8jb12,
MaxWeight: low,
Status: free,
Battery: 45,8

please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

2
1: assign parcel
2: pick up parcel
3: deliver parcel
4: send drone to charge
5: stop charging

1
enter parcel and drone's ids
10000004
123456789
please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

2
1: assign parcel
2: pick up parcel
3: deliver parcel
4: send drone to charge
5: stop charging

2
enter parcel's id
10000004
please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

3
1: station
2: drone
3: client
4: parcel

4
please enter ID

10000004
Parcel's Id: 10000004,
Sender's Id: 4943391,
Target's Id: 6142328,
Drone's Id: 123456789,
Weight: low,
Priority: regular,
Requested time: 28/10/2021 22:10:49
Scheduled time: 28/10/2021 22:13:08
PickedUp time: 28/10/2021 22:13:15
Delivered time: 28/10/2021 22:10:49

please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

2
1: assign parcel
2: pick up parcel
3: deliver parcel
4: send drone to charge
5: stop charging

3
enter parcel's id
10000001
please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

3
1: station
2: drone
3: client
4: parcel

4
please enter ID

10000004
Parcel's Id: 10000004,
Sender's Id: 4943391,
Target's Id: 6142328,
Drone's Id: 123456789,
Weight: low,
Priority: regular,
Requested time: 28/10/2021 22:10:49
Scheduled time: 28/10/2021 22:13:08
PickedUp time: 28/10/2021 22:13:15
Delivered time: 28/10/2021 22:10:49

please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

2
1: assign parcel
2: pick up parcel
3: deliver parcel
4: send drone to charge
5: stop charging

4
enter drone's id

123456789
enter a station ID from one of the following stations

station: 3762608,
Name: Central Station,
Location: 37° 0' 1392,1800000000103'' W 37° 0' 1392,1800000000103'' W
ChargeSlots: 28

station: 4780293,
Name: Malcha Mall,
Location: 31° 0' 4500,719999999994'' E 31° 0' 4500,719999999994'' E
ChargeSlots: 66

station: 111,
Name: newstat,
Location: 32° 0' 2399,9999999999914'' E 32° 0' 2399,9999999999914'' E
ChargeSlots: 23

3762608
please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

4
1: station's list
2: drone's list
3: client's list
4: parcel's list
5: list of parcels without drone
6: Stations with available chargers

2
Drone's Id: 4249495,
Model: ONe7H4F,
MaxWeight: heavy,
Status: shipping,
Battery: 79

Drone's Id: 8054498,
Model: ssBJm3x,
MaxWeight: low,
Status: free,
Battery: 76

Drone's Id: 4339121,
Model: lAsgwwm,
MaxWeight: heavy,
Status: shipping,
Battery: 86

Drone's Id: 5075044,
Model: BSYNlQ1,
MaxWeight: heavy,
Status: free,
Battery: 36

Drone's Id: 4746752,
Model: u7ON5zG,
MaxWeight: low,
Status: maintenance,
Battery: 49

Drone's Id: 123456789,
Model: kH8jb12,
MaxWeight: low,
Status: maintenance,
Battery: 45,8

please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

2
1: assign parcel
2: pick up parcel
3: deliver parcel
4: send drone to charge
5: stop charging

5
enter drone's id and station's id

123456789
3762608
please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

3
1: station
2: drone
3: client
4: parcel

2
please enter ID

123456789
Drone's Id: 123456789,
Model: kH8jb12,
MaxWeight: low,
Status: free,
Battery: 45,8

please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

4
1: station's list
2: drone's list
3: client's list
4: parcel's list
5: list of parcels without drone
6: Stations with available chargers

3
Client's Id: 6261448
Name:  Mikhal Levy ,
Phone: 053-5955070,
Location: 31° 0' 933,5324564639222'' E 31° 0' 933,5324564639222'' E

Client's Id: 1483957
Name:  Shaili Benloulou ,
Phone: 057-9953494,
Location: 31° 0' 1711,8880161605218'' E 31° 0' 1711,8880161605218'' E

Client's Id: 6739450
Name:  Talia Azoulay,
Phone: 056-6023778,
Location: 31° 0' 2331,093308409258'' E 31° 0' 2331,093308409258'' E

Client's Id: 7873768
Name:  Moti Cohen ,
Phone: 056-4260097,
Location: 31° 0' 2833,995942924076'' E 31° 0' 2833,995942924076'' E

Client's Id: 1844833
Name: Yoel Ivgi ,
Phone: 052-5051112,
Location: 31° 0' 3597,3083552854705'' E 31° 0' 3597,3083552854705'' E

Client's Id: 7340755
Name: Reouven Bensimon,
Phone: 052-5808498,
Location: 31° 0' 3798,68968159829'' E 31° 0' 3798,68968159829'' E

Client's Id: 1979576
Name: Eliezer Daby,
Phone: 053-4224758,
Location: 31° 0' 4949,820080649026'' E 31° 0' 4949,820080649026'' E

Client's Id: 3056116
Name: Chyrel Barouh,
Phone: 052-2207715,
Location: 31° 0' 4820,914340401501'' E 31° 0' 4820,914340401501'' E

Client's Id: 4984982
Name: David Dayan,
Phone: 052-2415835,
Location: 31° 0' 5953,8443401021295'' E 31° 0' 5953,8443401021295'' E

Client's Id: 2465863
Name: Yossef Amar,
Phone: 056-8398708,
Location: 31° 0' 5547,106204025681'' E 31° 0' 5547,106204025681'' E

please choose one of the following possibilities:
1: add an object
2: update an object
3: display an object
4: display a list of objects
5: exit

5

Sortie de C:\Users\שילי\source\repos\MiniProject5782_6058_1771\ConsoleUI\bin\Debug\net5.0\ConsoleUI.exe (processus 30012). Code : 0.
Appuyez sur une touche pour fermer cette fenêtre. . . */
