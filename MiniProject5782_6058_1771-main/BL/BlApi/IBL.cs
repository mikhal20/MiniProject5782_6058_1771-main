using BO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlApi
{
    public interface IBL
    {
        void AddClient(Client c);
        void AddDrone(Drone d, int stationId);
        void AddParcel(Parcel p);
        void AddStation(Station s);
        void FreeCharge(int DroneId,double time = 0);
        Client GetBlClient(int id);
        Drone GetBlDrone(int id);
        Parcel GetBlParcel(int id);
        Station GetBlStation(int id);
        DroneCharge GetBlDroneCharge(int droneChargeId);
        Station Neareststation(Location l, bool flag);
        void SendDroneToCharge(int id);
        void UpdateClientName(int Id, string name = " ", string phone = " ");
        void UpdateDroneName(int DroneId, string Model);
        void UpdateStationName(int StationId, string StationName = " ", string num =" ");
        void AssignParcelToDrone(int id);
        void PickUpParcelWithDrone(int id);
        void DeliverParcelWithDrone(int id);
        IEnumerable<DroneForList> GetDronesList();
        IEnumerable<ClientForList> GetClientList();
        IEnumerable<ParcelForList> GetParcelList();
        IEnumerable<StationForList> GetStationList();
        IEnumerable<DroneCharge> GetDroneChargeList();
        IEnumerable<ParcelForList> showParcelWithoutDrone();
        IEnumerable<StationForList> showStationWithCharge();
        void RemoveParcel(Parcel parcel);
        void RemoveDrone(Drone drone);
        void RemoveDroneCharge(DroneCharge droneCharge);
        void RemoveClient(Client client);
        void RemoveStation(Station station);
        void UpdateDrone(Drone drone);
        void UpdateParcel(Parcel p);
        void UpdateStation(Station station);
        double DeliveryDistance(int droneId, int id1, int id2);
        Parcel closestParcel(int DroneId, List<Parcel> parcels);
        void ConfirmDelivery(Parcel parcel);
        void ConfirmPickUp(Parcel parcel);
        void RunDroneSimulator(int DroneId, Action update, Func<bool> CkeckStop);
    }
}