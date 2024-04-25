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
    /// Interaction logic for ClientManageWindow.xaml
    /// </summary>
    public partial class ClientManageWindow : Window
    {
        BlApi.IBL LastBL;
        ClientForList client;
        public ClientManageWindow(BlApi.IBL bL, ClientForList c)
        {
            LastBL = bL;
            InitializeComponent();
            client = c;
            DataContext = client; //for binding to know the field of client
        }

        /// <summary>
        /// show the clients all his parcels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnShowMyParcels_Click(object sender, RoutedEventArgs e)
        {
            Client BlClient=LastBL.GetBlClient(client.Id);
            new ParcelsClient(LastBL, BlClient).Show(); //let the client look at his parcels
        }

        /// <summary>
        /// allow the client to see and update his profile 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMyProfile_Click(object sender, RoutedEventArgs e)
        {
            Client tempC = LastBL.GetBlClient(client.Id);
            new ClientWindow(LastBL, tempC).Show();
        }

        /// <summary>
        /// send a parcel to someone! press the button and add a parcel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSendAparcel_Click(object sender, RoutedEventArgs e)
        {
            List<ParcelForList> parcels = (List<ParcelForList>)LastBL.GetParcelList();
            new ParcelWindow(LastBL, parcels).Show(); //add the parcel
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
    }
}
