using BlApi;
using BO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {

        Parcel ParcelOfDrone = default;
        BlApi.IBL LastBL;
        BO.Drone drone;
        ListView list;
        public DroneWindow(BlApi.IBL bL, object DronesListWindow) //add constuctor
        {
            LastBL = bL;
            InitializeComponent();
            UpdateGrid.Visibility = Visibility.Hidden;
            Title = "Add drone"; //change the tittle
            ComboDroneWeight.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            List<string> StationsName = (from item in LastBL.GetStationList()
                                         select item.Name).ToList();
            ComboStationId.ItemsSource = StationsName;
            list = (ListView)DronesListWindow;
        }
        public DroneWindow(BlApi.IBL bL, Drone d, ListView DroneListView) //update constructor
        {
            LastBL = bL;
            InitializeComponent();
            AddGrid.Visibility = Visibility.Hidden;
            Title = "Update drone"; //change the tittle
            list = DroneListView;
            drone = d;
            DataContext = drone; //for binding to know the fields of the drone
            //update what buttons are available:
            if (drone.DroneStatus != DroneStatuses.Shipping)
            {
                drone.ParcelSending = null;
                showParcel.Visibility = Visibility.Hidden;
                parcelToPrint.Content = "(no parcel)";
            }
            else
            {
                parcelToPrint.Content = drone.ParcelSending.Id;
                showParcel.Visibility = Visibility.Visible;
                ParcelOfDrone = LastBL.GetBlParcel(drone.ParcelSending.Id);
            }

            if (drone.DroneStatus == DroneStatuses.Free)
            {
                BtnCharge.Visibility = Visibility.Visible;
                BtnReleaseCharge.Visibility = Visibility.Hidden;
                BtnDroneDelivery.Visibility = Visibility.Visible; //assign is available
                BtnParcelDelivery.Visibility = Visibility.Hidden;
                BtnPickUp.Visibility = Visibility.Hidden;
            }

            if (drone.DroneStatus == DroneStatuses.Maintenance)
            {
                BtnReleaseCharge.Visibility = Visibility.Visible;
                BtnCharge.Visibility = Visibility.Hidden;
                BtnDroneDelivery.Visibility = Visibility.Hidden;
                BtnParcelDelivery.Visibility = Visibility.Hidden;
                BtnPickUp.Visibility = Visibility.Hidden;
            }

            if (drone.DroneStatus == DroneStatuses.Shipping && ParcelOfDrone.PickedUp == null)
            {
                BtnReleaseCharge.Visibility = Visibility.Hidden;
                BtnCharge.Visibility = Visibility.Hidden;
                BtnPickUp.Visibility = Visibility.Visible;
                BtnParcelDelivery.Visibility = Visibility.Hidden;
                BtnDroneDelivery.Visibility = Visibility.Hidden;
            }

            if (drone.DroneStatus == DroneStatuses.Shipping && ParcelOfDrone.PickedUp != null && ParcelOfDrone.Delivered == null)
            {
                BtnReleaseCharge.Visibility = Visibility.Hidden;
                BtnCharge.Visibility = Visibility.Hidden;
                BtnPickUp.Visibility = Visibility.Hidden;
                BtnParcelDelivery.Visibility = Visibility.Visible;
                BtnDroneDelivery.Visibility = Visibility.Hidden;
            }
            ManualBtn.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// buttton to show the parcel in sedding of the drone if there is one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showParcel_Click(object sender, RoutedEventArgs e)
        {
            if (drone.ParcelSending == null)
            {
                MessageBox.Show("Could Not show the parcel\nAlready delivered!", "ERROR", MessageBoxButton.OKCancel);
                return;
            }
            Parcel parcel = LastBL.GetBlParcel(drone.ParcelSending.Id);
            new ParcelWindow(LastBL, parcel).Show();
        }

        /// <summary>
        /// allow only to enter numbers for drone's id
        /// https://stackoverflow.com/questions/49546723/binding-text-change-event-to-prevent-invalid-input-wpf-xaml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumbersOnly(object sender, TextCompositionEventArgs e) //If the user attempts to enter an invalid input I would like to prevent it
        {
            Regex regex = new Regex("[^0-9]+"); //only numbers allowed (-is inaccessible)
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// remove the text in the textbox when you click to write
        /// https://stackoverflow.com/questions/6972701/remove-text-after-clicking-in-the-textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        /// <summary>
        /// when ok is pressed add the drone to the list of drones
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            int DroneId = Convert.ToInt32(ID.Text.ToString()); //convert from int to string
            string StationName = (string)ComboStationId.SelectedItem; //s is the selectioned Station's name
            StationForList s = default;
            foreach (var item in LastBL.GetStationList())
            {
                if (item.Name == StationName) //find the station itself in the list of stations with the name
                    s = item;
            }
            Station LocationS = LastBL.GetBlStation(s.Id); //Get the Bl Station to access Location of the Station
            string model = MODEL.Text.ToString();
            drone = new Drone //create an object with the entered Data to send to the function AddDrone
            {
                ID = DroneId,
                Model = model,
                Weight = (WeightCategories)ComboDroneWeight.SelectedItem,
                DroneStatus = (DroneStatuses)Status.Created,
                Location = LocationS.Location,
                BatteryLevel = r.Next(20, 41),
                ParcelSending = null
            };
            try
            {
                LastBL.AddDrone(drone, s.Id); //add the Drone to the list of Drones
            }
            catch (Exception)
            {
                MessageBox.Show("Could Not Add The Drone \n Try Again!", "ERROR", MessageBoxButton.OKCancel);
                return;
            }
            MessageBox.Show("Drone Added Succesfuly", "Message", MessageBoxButton.OK, MessageBoxImage.Information); //message for user that the drone was Added.
            list.ItemsSource = LastBL.GetDronesList().ToList(); //update the drones list within every changes
            allowClosing = true;
            Close(); //Close the current window
        }

        /// <summary>
        /// Allow choosing between the Weight possibilities (low, middle, heavy)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboDroneWeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Allow choosing between the Status possibilities (Free, Maintenance, Shipping)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboStationId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// allow input for droneId
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DroneId_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /// <summary>
        /// Aloow input for Drone's Model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Model_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /// <summary>
        /// cancel means do dot add the drone and so the window will be closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            allowClosing = true;
            Close();
        }

        /// <summary>
        /// Close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            allowClosing = true;
            Close();
        }

        /// <summary>
        /// update drone's model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LastBL.UpdateDroneName(drone.ID, modelToPrint.Text.ToString());
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("drone updated succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            drone = LastBL.GetBlDrone(drone.ID);
            list.ItemsSource = LastBL.GetDronesList().ToList(); //update the drones list within every changes
        }

        /// <summary>
        /// send the drone to charge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCharge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LastBL.SendDroneToCharge(drone.ID); //cal to the function to send drone to charge
                drone = LastBL.GetBlDrone(drone.ID);
                statusToPrint.Content = drone.DroneStatus; //update the status to be in maintenance - charging
                batteryBar.Value = drone.BatteryLevel; //show updated battery
                BtnCharge.Visibility = Visibility.Hidden;
                BtnDroneDelivery.Visibility = Visibility.Hidden;
                BtnReleaseCharge.Visibility = Visibility.Visible;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("drone succesfully sent", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            list.ItemsSource = LastBL.GetDronesList().ToList(); //update the drones list within every changes
        }

        /// <summary>
        /// release drone from charge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReleaseCharge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LastBL.FreeCharge(drone.ID); //call to function to release drone from charge
                drone = LastBL.GetBlDrone(drone.ID);
                BtnReleaseCharge.Visibility = Visibility.Hidden;
                statusToPrint.Content = drone.DroneStatus; //update the status to be Available again - free
                batteryBar.Value = drone.BatteryLevel; // show updated battery
                BtnCharge.Visibility = Visibility.Visible;
                BtnDroneDelivery.Visibility = Visibility.Visible;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("drone succesfully released", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            list.ItemsSource = LastBL.GetDronesList().ToList(); //update the drones list within every changes
        }

        /// <summary>
        /// the button to send a drone to a delivery (assign)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDroneDelivery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LastBL.AssignParcelToDrone(drone.ID); //assign a parcel to the drone
                drone = LastBL.GetBlDrone(drone.ID);
                batteryBar.Value = drone.BatteryLevel; // show updated battery
                statusToPrint.Content = drone.DroneStatus; //show the updated status of the drone
                //update which of the following buttons are "available" - visible:
                showParcel.Visibility = Visibility.Visible;
                BtnCharge.Visibility = Visibility.Hidden;
                BtnDroneDelivery.Visibility = Visibility.Hidden;
                BtnPickUp.Visibility = Visibility.Visible;
                MessageBox.Show("Drone assigned to parcel", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                list.ItemsSource = LastBL.GetDronesList().ToList(); //update the drones list within every changes
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// pick up parcel with drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPickUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LastBL.PickUpParcelWithDrone(drone.ID); //call the function to pick up parcel
                drone = LastBL.GetBlDrone(drone.ID);
                batteryBar.Value = drone.BatteryLevel; //drone's updated battery
                Parcel parcel = LastBL.GetBlParcel(drone.ParcelSending.Id);
                ParcelForList p = default;
                foreach (var item in LastBL.GetParcelList())
                {
                    if (item.Id == drone.ParcelSending.Id)
                        p = item;
                }
                BtnDroneDelivery.Visibility = Visibility.Hidden;
                BtnParcelDelivery.Visibility = Visibility.Hidden;
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            int IdSender = drone.ParcelSending.Sender.Id;
            MessageBox.Show("Drone was sent to pick up parcel\nrequires confirmation from Sender " + IdSender, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            list.ItemsSource = LastBL.GetDronesList().ToList(); //update the drones list within every changes
        }

        /// <summary>
        /// The button to update a drone to deliver a parcel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnParcelDelivery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                drone = LastBL.GetBlDrone(drone.ID);
                int IdRecipient = drone.ParcelSending.Recipient.Id;
                LastBL.DeliverParcelWithDrone(drone.ID);
                drone = LastBL.GetBlDrone(drone.ID);
                batteryBar.Value = drone.BatteryLevel; //show updated drone's battery
                statusToPrint.Content = drone.DroneStatus; //show updated drone status
                //update which of the following buttons are "available" - visible:
                BtnDroneDelivery.Visibility = Visibility.Visible;
                BtnParcelDelivery.Visibility = Visibility.Hidden;
                BtnCharge.Visibility = Visibility.Visible;
                MessageBox.Show("Drone delivered parcel\nrequires confirmation from Recipient " + IdRecipient, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                list.ItemsSource = LastBL.GetDronesList().ToList(); //update the drones list within every changes
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        BackgroundWorker worker;
        private void update()
        {
            worker.ReportProgress(0);
        }
        private bool checkStop()
        {
            return worker.CancellationPending;
        }
        private void updateDroneView()
        {
            lock (LastBL)
            {
                drone = LastBL.GetBlDrone(drone.ID);
                batteryBar.Value = drone.BatteryLevel; //show updated drone's battery
                statusToPrint.Content = drone.DroneStatus; //show updated drone status
                locationToPrint.Content = drone.Location; //show updated location
                if (drone.DroneStatus != DroneStatuses.Shipping)
                {
                    drone.ParcelSending = null;
                    showParcel.Visibility = Visibility.Hidden;
                    parcelToPrint.Content = "(no parcel)";
                }
                else
                {
                    parcelToPrint.Content = drone.ParcelSending.Id;
                    showParcel.Visibility = Visibility.Visible;
                    ParcelOfDrone = LastBL.GetBlParcel(drone.ParcelSending.Id);
                }
            }
            list.ItemsSource = LastBL.GetDronesList().ToList(); //update the drones list within every changes
        }

        /// <summary>
        /// Button that activates the automatic simulator in the bl
        /// </summary>
        /// <param name="sender"></param>  
        /// <param name="e"></param>
        private void BtnSimulator_Click(object sender, RoutedEventArgs e)
        {
            //hide all buttons visibility
            BtnDroneDelivery.Visibility = Visibility.Hidden;
            BtnParcelDelivery.Visibility = Visibility.Hidden;
            BtnPickUp.Visibility = Visibility.Hidden;
            BtnReleaseCharge.Visibility = Visibility.Hidden;
            BtnCharge.Visibility = Visibility.Hidden;
            Simulator.Visibility = Visibility.Hidden;
            ManualBtn.Visibility = Visibility.Visible; //except manual button which is visible

            worker = new() { WorkerReportsProgress = true, WorkerSupportsCancellation = true, };
            worker.DoWork += (sender, args) => LastBL.RunDroneSimulator(drone.ID, update, checkStop);
            worker.RunWorkerCompleted += (sender, args) =>
            {
                worker = null;
            };
            worker.ProgressChanged += (sender, args) => updateDroneView();
            worker.RunWorkerAsync(drone.ID);
        }

        /// <summary>
        /// dont acctivate the automatic simulator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Manual_Click(object sender, RoutedEventArgs e)
        {
            worker?.CancelAsync();
            ManualBtn.Visibility = Visibility.Hidden;
            Simulator.Visibility = Visibility.Visible;
            drone = LastBL.GetBlDrone(drone.ID); //update details when we stop automatic
            //if the drone is free we can or send it to charge or assign the drone
            if (drone.DroneStatus == DroneStatuses.Free)
            {
                BtnCharge.Visibility = Visibility.Visible;
                BtnDroneDelivery.Visibility = Visibility.Visible; 
            }
            //if the drone is charging we can only release it
            if (drone.DroneStatus == DroneStatuses.Maintenance)
            {
                BtnReleaseCharge.Visibility = Visibility.Visible;
            }
            //if the drone is shiiping (was assigned) but not picked up then only pick up is available
            if (drone.DroneStatus == DroneStatuses.Shipping && ParcelOfDrone.PickedUp == null)
            {
                BtnPickUp.Visibility = Visibility.Visible;
            }
            //if the drone has not deliver yet
            if (drone.DroneStatus == DroneStatuses.Shipping && ParcelOfDrone.PickedUp != null && ParcelOfDrone.Delivered == null)
            {
                BtnParcelDelivery.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// https://stackoverflow.com/questions/743906/how-to-hide-close-button-in-wpf-window#:~:text=WPF%20doesn't%20have%20a,%3B%20%5BDllImport(%22user32.
        /// prevent closing from the X button
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;

            if (hwndSource != null)
            {
                hwndSource.AddHook(HwndSourceHook);
            }
        }

        private bool allowClosing = false;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;

        private const uint SC_CLOSE = 0xF060;

        private const int WM_SHOWWINDOW = 0x00000018;
        private const int WM_CLOSE = 0x10;

        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_SHOWWINDOW:
                    {
                        IntPtr hMenu = GetSystemMenu(hwnd, false);
                        if (hMenu != IntPtr.Zero)
                        {
                            EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
                        }
                    }
                    break;
                case WM_CLOSE:
                    if (!allowClosing)
                    {
                        handled = true;
                    }
                    break;
            }
            return IntPtr.Zero;
        }
    }
}