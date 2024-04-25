using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        BlApi.IBL LastBL;
        public EmployeeWindow(BlApi.IBL bL)
        {
            LastBL = bL;
            InitializeComponent();
        }

        /// <summary>
        /// when you Click the button, open a new window with the list of stations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnShowListStations_Click(object sender, RoutedEventArgs e)
        {
            new StationListWindow(LastBL).Show(); //show list of stations
        }

        /// <summary>
        /// when you Click the button, open a new window with the list of drones
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnShowListDrones_Click(object sender, RoutedEventArgs e)
        {
            new DronesListWindow(LastBL).Show(); //show list of drones
        }

        /// <summary>
        /// when you Click the button, open a new window with the list of clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnShowListClients_Click(object sender, RoutedEventArgs e)
        {
            new ClientsListWindow(LastBL).Show(); //show list of Clients
        }

        /// <summary>
        /// when you Click the button, open a new window with the list of Parcels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnShowListParcels_Click(object sender, RoutedEventArgs e)
        {
            new ParcelListWindow(LastBL).Show();//show list of Parcels
        }
    }
}