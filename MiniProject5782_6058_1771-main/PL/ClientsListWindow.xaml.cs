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
    /// Interaction logic for ClientsListWindow.xaml
    /// </summary>
    public partial class ClientsListWindow : Window, INotifyPropertyChanged
    {
        BlApi.IBL bL;
        private List<ClientForList> clients;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public ClientsListWindow(BlApi.IBL Bl)
        {
            bL = Bl;
            InitializeComponent();
            clients = bL.GetClientList().ToList(); //show the list of clients
            DataContext = this;
        }

        /// <summary>
        /// property for list of drones(binding)
        /// </summary>
        public List<ClientForList> Clients
        {
            get { return clients; }
            set { clients = value; PropertyChanged(this, new PropertyChangedEventArgs("Clients")); }
        }

        /// <summary>
        /// click to add a client to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            new ClientWindow(bL, ClientsListView).ShowDialog(); //open Station Window and come back here

            ClientsListView.ItemsSource = bL.GetClientList(); //update the list of clients
        }

        private void ClientsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// when you click on a client in the list, open the client window with its details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ClientForList client = ClientsListView.SelectedItem as ClientForList; //put the selected client in client
            Client c = bL.GetBlClient(client.Id); //gets the client from BL as a "Client" type
            new ClientWindow(bL, c).ShowDialog(); //open to see details and or update

            ClientsListView.ItemsSource = bL.GetClientList(); //update the list of clients
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
        /// refresh client list to the way it was
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ClientsListView.ItemsSource = bL.GetClientList();
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