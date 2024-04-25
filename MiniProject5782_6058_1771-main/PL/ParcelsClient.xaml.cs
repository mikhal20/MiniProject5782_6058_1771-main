using System;
using System.Collections.Generic;
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
    /// Interaction logic for ParcelsClient.xaml
    /// </summary>
    public partial class ParcelsClient : Window
    {
        BlApi.IBL ThisBl;
        Parcel parcelSender;
        Parcel parcelRecipient;
        public ParcelsClient(BlApi.IBL bL, Client client)
        {
            ThisBl = bL;
            InitializeComponent();
            if (client.SentParcels.Count == 0) //if the parcel list that the client has sent is empty we hide the list
                ListSentParcel.Visibility = Visibility.Hidden;
            else //otherwise we show the list
            {
                picked.Visibility = Visibility.Visible;
                ListSentParcel.ItemsSource = client.SentParcels;
                YouHaveNotSentAnyParcelsYet.Visibility = Visibility.Hidden;
            }
            if (client.ReceiveParcels.Count == 0) //if the parcel list that the client has received is empty we hide the list
                ListReceivedParcel.Visibility = Visibility.Hidden;
            else //otherwise we show the list
            {

                delivered.Visibility = Visibility.Visible;
                ListReceivedParcel.ItemsSource = client.ReceiveParcels;
                YouHaveNotReceivedAnyParcelsYet.Visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// the client confirm that the parcel was picked up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxPickeup_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxPickeup.IsChecked = true;
            try
            {
                ThisBl.ConfirmPickUp(parcelSender);
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("Parcel succesfuly picked up", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            ListSentParcel.ItemsSource = ThisBl.GetBlClient(parcelSender.Sender.Id).SentParcels;
        }
      
        /// <summary>
        /// the client confirm that the parcel was delivered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxdelivered_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ThisBl.ConfirmDelivery(parcelRecipient);
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("Parcel succesfuly delivered", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            ListReceivedParcel.ItemsSource = ThisBl.GetBlClient(parcelRecipient.Recipient.Id).ReceiveParcels;
        }

        /// <summary>
        /// save the selecioned parcel that was sent in order to confirm picking up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListSentParcel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParcelCustomer parcelS = ListSentParcel.SelectedItem as ParcelCustomer; //put the selected Parcel in parcel
            if (parcelS != null)
            {
                parcelSender = ThisBl.GetBlParcel(parcelS.Id);//gets the parcel from BL as a "Parcel" type
                if (parcelS.Status == Status.Assigned) //only if the parcel was assigned then you can confirm picked up
                    CheckBoxPickeup.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// save the selecioned received parcel in order to confirm delivery
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListReceivedParcel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParcelCustomer parcelR = ListReceivedParcel.SelectedItem as ParcelCustomer; //put the selected Parcel in parcel
            if (parcelR != null)
            {
                parcelRecipient = ThisBl.GetBlParcel(parcelR.Id); //gets the parcel from BL as a "Parcel" type
                if (parcelR.Status == Status.Picked) //only if parcel was picked then you can confirm delivery
                    CheckBoxDelivered.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// close parcels of client window
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