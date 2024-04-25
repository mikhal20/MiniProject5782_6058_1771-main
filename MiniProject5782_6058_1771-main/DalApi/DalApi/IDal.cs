using DO;
using System.Collections.Generic;
using System;

namespace DalApi
{
    public interface IDal
    {
        void addClient(Client c);
        void addDrone(Drone d);
        void addParcel(Parcel p);
        void addStation(Station s);
        void addDroneCharge(DroneCharge dr);
        void clearClient(Client c);
        void clearDrone(Drone d);
        void clearParcel(Parcel p);
        void clearStation(Station s);
        void clearDroneCharge(DroneCharge temp);
        void Assign(Parcel p, Drone d);
        double[] DroneElectricity(); 
        Client GetClient(int ClientID);
        Drone GetDrone(int DronetID);
        DroneCharge GetDroneCharge(int DroneChargeId);
        Parcel GetParcel(int ParcelID);
        Station GetStation(int StationID);
        void parcelDelivery(Parcel p);
        void PickUp(Parcel p);
        void sendToCharge(Station s, Drone d);
        void freeCharge(Station s);
        void showOptions(int id, int num);
        IEnumerable<Client> showClients();
        IEnumerable<Drone> showDrones();
        IEnumerable<Parcel> showParcels(Func<Parcel, bool> predicate = null);
        //IEnumerable<Parcel> showParcelWithoutDrone();
        IEnumerable<Station> showStations(Func<Station, bool> predicate = null);
        //IEnumerable<Station> showStationWithCharge();
        IEnumerable<DroneCharge> showDroneCharges();
        void UpdateStationName(Station tempS ,string name,string num);
        void UpdateClientName(int id, string name, string phone);
        void UpdateDrone(Drone d);
        void UpdateParcel(Parcel p);
        void UpdateStation(Station s);
    }
}