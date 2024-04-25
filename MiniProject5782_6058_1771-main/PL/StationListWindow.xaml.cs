using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
    /// Interaction logic for StationListWindow.xaml
    /// </summary>
    public partial class StationListWindow : Window, INotifyPropertyChanged
    {
        BlApi.IBL bL;
        private List<StationForList> stations;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public StationListWindow(BlApi.IBL Bl)
        {
            bL = Bl;
            InitializeComponent();
            stations = bL.GetStationList().ToList();
            DataContext = this;
        }

        /// <summary>
        /// property for stations (for binding)
        /// </summary>
        public List<StationForList> Stations
        {
            get { return stations; }
            set { stations = value; PropertyChanged(this, new PropertyChangedEventArgs("Stations")); }
        }

        /// <summary>
        /// group list of station by number of ChargeSlots
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grouping_Click(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(StationsListView.ItemsSource); //grouping by chargeslots
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("FreeChargeSlots");
            view.GroupDescriptions.Add(groupDescription);
        }

        /// <summary>
        /// click on a spesific station in the list to see its details or update the station
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StationsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Station s = default;
            StationForList station = StationsListView.SelectedItem as StationForList; //put the selected station in station
            if (station != null)
                s = bL.GetBlStation(station.Id); //gets the station from BL as a "Station" type
            new StationWindow(bL, s).ShowDialog(); //open to see details and or update

            StationsListView.ItemsSource = bL.GetStationList();
        } 

        /// <summary>
        /// add a station to the list of stations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddStation_Click(object sender, RoutedEventArgs e)
        {
            new StationWindow(bL, StationsListView).ShowDialog(); //Click and open Station Window

            StationsListView.ItemsSource = bL.GetStationList();
        }

        /// <summary>
        /// refresh the list the way it was
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<StationForList> temp = bL.GetStationList();
            StationsListView.ItemsSource = temp;
        }

        private void StationsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// close the station's list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            allowClosing = true;
            Close(); //close drone list window
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