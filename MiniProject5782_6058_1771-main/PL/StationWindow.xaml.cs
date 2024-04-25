using System;
using System.Collections.Generic;
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
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for StationWindow.xaml
    /// </summary>
    public partial class StationWindow : Window
    {
        BlApi.IBL LastBL;
        Station station;
        ListView list;
        public StationWindow(BlApi.IBL bL, object StationsListWindow) //add constuctor
        {
            LastBL = bL;
            InitializeComponent();
            UpdateGrid.Visibility = Visibility.Hidden;
            Title = "Add station"; //change the tittle
            list = (ListView)StationsListWindow;
        }

        public StationWindow(BlApi.IBL bL, Station s) //update constuctor
        {
            LastBL = bL;
            InitializeComponent();
            AddGrid.Visibility = Visibility.Hidden;
            Title = "Update station"; //change the tittle
            station = s;
            DataContext = station;
            StationForList tempS = default;
            if (s != null)
            {
                foreach (var item in LastBL.GetStationList())
                {
                    if (item.Id == s.Id)
                        tempS = item;
                }
                availableChargeSlotsToPrint.Content = tempS.FreeChargeSlots;
                OccupiedChargeSlotsToPrint.Content = tempS.OccupiedChargeSlots;
                if (s.ListDroneCharge.Count == 0) //if the list of drone charge is empty hide the list view and print "Empty"
                {
                    ListDroneCharges.Visibility = Visibility.Hidden;
                    ListDroneChargeToPrint.Content = "Empty";
                }
                else //otherwise print the list of drones that are curently charging
                    ListDroneCharges.ItemsSource = s.ListDroneCharge;
            }
        }

        /// <summary>
        /// allow only to enter numbers for drone's id
        /// https://stackoverflow.com/questions/49546723/binding-text-change-event-to-prevent-invalid-input-wpf-xaml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumbersOnly(object sender, TextCompositionEventArgs e) //If the user attempts to enter an invalid input I would like to prevent it
        {
            Regex regex = new Regex("[^0-9]+"); //only numbers allowed (positive) (-is inaccessible)
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

        private void StationId_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Latitude_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Longitude_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ChargeSlots_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /// <summary>
        /// Click on a drone charge and open the relevant Drone window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListDroneCharges_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DroneCharge Dr = ListDroneCharges.SelectedItem as DroneCharge;
            if (Dr != null)
            {
                DroneCharge tempD = default;
                foreach (var item in LastBL.GetDroneChargeList())
                {
                    if (item.ID == Dr.ID)
                        tempD = item;
                }
                new DroneChargeWindow(LastBL, tempD, station).ShowDialog(); //open to see details and or update
            }
            station = LastBL.GetBlStation(station.Id);
            StationForList st = default ;
            foreach (var item in LastBL.GetStationList())
            {
                if (item.Id == station.Id)
                    st = item;
            }
            OccupiedChargeSlotsToPrint.Content = st.OccupiedChargeSlots;
            if (station.ListDroneCharge.Count == 0)
                ListDroneCharges.Visibility = Visibility.Hidden;
            else
                ListDroneCharges.ItemsSource = station.ListDroneCharge;
        }

        /// <summary>
        /// See station's details, Update the name of the station or it's chargeslots number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LastBL.UpdateStationName(station.Id, nameToPrint.Text, ChargeToPrint.Text.ToString());
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("station updated succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            station = LastBL.GetBlStation(station.Id);
            availableChargeSlotsToPrint.Content = station.ChargeSlots;
        }

        /// <summary>
        /// press ok to add the station to the list of stations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Location tempL = new Location() //if the string contains a "." change it to a "," in order to convert to double
            {
                Latitude = Convert.ToDouble(EnterYourLatitude.Text.ToString().Replace('.', ',')),
                Longitude = Convert.ToDouble(EnterYourLongitude.Text.ToString().Replace('.', ','))
            };
            Station station = new Station //create an object with the entered Data to send to the function AddStation
            {
                Id = Convert.ToInt32(ID.Text.ToString()),
                Name = NAME.Text.ToString(),
                ChargeSlots = Convert.ToInt32(ChargeSlots.Text.ToString()),
                Location = tempL,
                ListDroneCharge = new List<DroneCharge>()
            };
            try
            {
                LastBL.AddStation(station); //add the Station to the list of Stations
            }
            catch (Exception)
            {
                MessageBox.Show("Could Not Add The Station, Try Again", "ERROR", MessageBoxButton.OKCancel);
                return;
            }
            MessageBox.Show("Station Added Succesfuly", "Message", MessageBoxButton.OK, MessageBoxImage.Information); //message for user that the drone was Added.
            allowClosing = true;
            Close(); //Close the current window
        }

        /// <summary>
        /// cancel means do dot add the station and so the window will be closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            allowClosing = true;
            Close();
        }

        /// <summary>
        /// close station window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            allowClosing = true;
            Close();
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