using BO;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for DronesListWindow.xaml
    /// </summary>
    public partial class DronesListWindow : Window, INotifyPropertyChanged
    {
        BlApi.IBL bL;

        private List<DroneForList> drones;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public DronesListWindow(BlApi.IBL Bl)
        {
            bL = Bl;
            InitializeComponent();
            drones = bL.GetDronesList().ToList();
            DataContext = this;
            ComboStatusSelector.ItemsSource = Enum.GetValues(typeof(DroneStatuses)); //comboBox to display the choices of DroneStatuses to filter the list
            ComboWeightSelector.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));  //comboBox to display the choices of WeightCategories to filter the list
        }

        /// <summary>
        /// propert for list of drones(binding)
        /// </summary>
        public List<DroneForList> Drones
        {
            get { return drones; }
            set { drones = value; PropertyChanged(this, new PropertyChangedEventArgs("Drones")); }
        }

        /// <summary>
        /// get the status that was selected by the ComboStatusSelector and update the list according to it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboStatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) //show the drone according to the Status Selection
        {
            DroneStatuses status = (DroneStatuses)ComboStatusSelector.SelectedItem;
            try //get the drones with the selected status
            {
                IEnumerable<DroneForList> temp = bL.GetDronesList();

                if (ComboWeightSelector.SelectedItem != null) //if there is a selected Weight get only those drones
                    temp = temp.Where(x => x.Weight == (BO.WeightCategories)ComboWeightSelector.SelectedItem);

                DronesListView.ItemsSource = temp.Where(x => x.DroneStatus == status);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// get the weight that was selected by the ComboWeightSelector and update the list according to it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboWeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) //show the drone according to the Weight Selection
        {
            BO.WeightCategories weight = (BO.WeightCategories)ComboWeightSelector.SelectedItem;

            try //get the drones with the selected Weight
            {
                IEnumerable<DroneForList> temp = bL.GetDronesList();

                if (ComboStatusSelector.SelectedItem != null) //if there is a selected status get only those drones
                    temp = temp.Where(x => x.DroneStatus == (DroneStatuses)ComboStatusSelector.SelectedItem);

                DronesListView.ItemsSource = temp.Where(x => x.Weight == weight);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// open a new window in order to add a drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddDrone_Click(object sender, RoutedEventArgs e) //call to add constructor //new
        {
            new DroneWindow(bL, DronesListView).ShowDialog(); //Click and open Drone Window

            IEnumerable<DroneForList> temp = bL.GetDronesList();
            if (ComboWeightSelector.SelectedItem != null)
                temp = temp.Where(x => x.Weight == (BO.WeightCategories)ComboWeightSelector.SelectedItem);
            if (ComboStatusSelector.SelectedItem != null)
                temp = temp.Where(x => x.DroneStatus == (DroneStatuses)ComboWeightSelector.SelectedItem);
            DronesListView.ItemsSource = temp;
        }

        private void DronesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        /// <summary>
        /// close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            allowClosing = true;
            Close(); //close drone list window
        }

        /// <summary>
        /// click on a spesific drone in the list to see its details, add or update the drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DronesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DroneForList d = DronesListView.SelectedItem as DroneForList; //put the selected drone in d
            Drone dr = bL.GetBlDrone(d.ID); //gets the drone from BL as a "Drone" type
            new DroneWindow(bL, dr, DronesListView).Show();
        }

        /// <summary>
        /// refresh the page after making changes to original
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshWindow_Click(object sender, RoutedEventArgs e)
        {
            DronesListView.ItemsSource = bL.GetDronesList();
        }

        /// <summary>
        /// button that group the drones by their status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupeStatus_Click(object sender, RoutedEventArgs e) //add a great explanation or change.
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DronesListView.ItemsSource); //grouping
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("DroneStatus");
            view.GroupDescriptions.Add(groupDescription);
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