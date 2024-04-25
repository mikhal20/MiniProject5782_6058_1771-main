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
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        BlApi.IBL LastBL;
        Client tempClient;
        public ClientWindow(BlApi.IBL bL, object ClientsListView) //add constructor
        {
            LastBL = bL;
            InitializeComponent();
            UpdateGrid.Visibility = Visibility.Hidden;
        }

        public ClientWindow(BlApi.IBL bL, Client client) //update constructor
        {
            LastBL = bL;
            InitializeComponent();
            DataContext = client; //for binding to know the fields of client
            AddGrid.Visibility = Visibility.Hidden;
            tempClient = LastBL.GetBlClient(client.Id);
            if (tempClient.ReceiveParcels.Count == 0) //if the list of parcel that the client has received is empty
            {
                ListReceivedParcel.Visibility = Visibility.Hidden; //hide this list
                Empty.Content = "Empty"; //and say that its empty
            }
            else
                ListReceivedParcel.ItemsSource = tempClient.ReceiveParcels;

            if (tempClient.SentParcels.Count == 0) //if the list of parcel that the client has sent is empty
            {
                ListSentParcel.Visibility = Visibility.Hidden; //hide this list
                anotherEmpty.Content = "Empty"; //and say that its empty
            }
            else
                ListSentParcel.ItemsSource = tempClient.SentParcels;
        }

        private void ClientId_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Phone_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Latitude_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Longitude_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /// <summary>
        /// press ok to add the client to the list of client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (ID.Text== "Enter Client's Id"|| NAME.Text== "Enter Client's Name"|| PhoneClient.Text== "Enter phone number"|| EnterYourLatitude.Text== "Latitude"||EnterYourLongitude.Text=="Longitude")
            {
                MessageBox.Show("Could Not Add the client, Try Again", "ERROR", MessageBoxButton.OKCancel);
                return; //if one of the field above are empty we cannot add the client.
            }
            if(PhoneClient.Text[0]!='0')
            {
                PhoneClient.BorderBrush = Brushes.Red;
                MessageBox.Show("Could Not Add The Client, Try Again", "ERROR", MessageBoxButton.OKCancel);
                return;
            }
            Location tempL = new Location() //if the string contains a "." change it to a "," in order to convert to double
            {
                Latitude = Convert.ToDouble(EnterYourLatitude.Text.ToString().Replace('.', ',')),
                Longitude = Convert.ToDouble(EnterYourLongitude.Text.ToString().Replace('.', ','))
            };
            Client client = new Client() //create an object with the entered Data to send to the function AddClient
            {
               Id= Convert.ToInt32(ID.Text.ToString()),
               Name = NAME.Text.ToString(),
               Phone = PhoneClient.Text.ToString(),
               Location = tempL,
               ReceiveParcels=null,
               SentParcels= null
            };
            try
            {
                LastBL.AddClient(client); //add the Client to the list of clients
            }
            catch (Exception)
            {
                MessageBox.Show("Could Not Add The Client, Try Again", "ERROR", MessageBoxButton.OKCancel);
                return;
            }
            MessageBox.Show("Client Added Succesfuly", "Message", MessageBoxButton.OK, MessageBoxImage.Information); //message for user that the drone was Added.
            allowClosing = true;
            Close(); //Close the current window
        }

        /// <summary>
        /// do not add the client (cancel)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            allowClosing = true;
            Close();
        }

        /// <summary>
        /// close actual window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            allowClosing = true;
            Close();
        }

        /// <summary>
        /// update the name of the client and/or its phone number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LastBL.UpdateClientName(Int32.Parse(ClientIdToPrint.Content.ToString()), nameToPrint.Text, clientPhoneToPrint.Text.ToString());
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("Client updated succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            tempClient = LastBL.GetBlClient(Int32.Parse(ClientIdToPrint.Content.ToString())); //update the client with the details
        }

        /// <summary>
        /// allow to click on the list of parcel that the client has received and open the parcel itself to see the details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListReceivedParcel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParcelCustomer parcel = ListReceivedParcel.SelectedItem as ParcelCustomer; //get the parcel (ParcelForList) that wa chosen
            if (parcel != null)
            {
                Parcel tempP = LastBL.GetBlParcel(parcel.Id);
                new ParcelWindow(LastBL, tempP).ShowDialog(); //open to see parcek's details and/or update
            }
            tempClient = LastBL.GetBlClient(Int32.Parse(ClientIdToPrint.Content.ToString()));
            ListReceivedParcel.ItemsSource = tempClient.ReceiveParcels;
        }

        /// <summary>
        /// allow to click on the list of parcel that the client has sent and open the parcel itself to see the details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListSentParcel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParcelCustomer parcel = ListSentParcel.SelectedItem as ParcelCustomer;
            if (parcel != null)
            {
                Parcel tempP = LastBL.GetBlParcel(parcel.Id);
                new ParcelWindow(LastBL, tempP).ShowDialog(); //open to see details and or update
            }
            tempClient = LastBL.GetBlClient(Int32.Parse(ClientIdToPrint.Content.ToString()));
            ListSentParcel.ItemsSource = tempClient.SentParcels;
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