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
    /// Interaction logic for ParcelWindow.xaml
    /// </summary>
    public partial class ParcelWindow : Window
    {
        BlApi.IBL ThisBl;
        Parcel parcel;
        public ParcelWindow(BlApi.IBL bL, Object ParcelListView) //add constructor
        {
            ThisBl = bL;
            InitializeComponent();
            UpdateGrid.Visibility = Visibility.Hidden; 
            ComboParcelWeight.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories)); //the comboBox shows the weight options from WeightCategories
            ComboParcelPriority.ItemsSource = Enum.GetValues(typeof(BO.Priorities)); //the comboBox shows the priority options from Priorities
        }

        public ParcelWindow(BlApi.IBL bL, Parcel p) //update constructor
        {
            ThisBl = bL;
            InitializeComponent();
            AddGrid.Visibility = Visibility.Hidden;
            parcel = p;
            DataContext = parcel; //for binding to know the fields of the parcel
            Status s;
            if (p.Delivered != null)
                s = Status.Delivered;
            else if (p.PickedUp != null)
                s = Status.Picked;
            else if (p.Scheduled != null)
                s = Status.Assigned;
            else
                s = Status.Created;
            ParcelStatusToPrint.Content = s; //show parcel's status
            if (p.Scheduled == null)
            {
                Remove.Visibility = Visibility.Visible;
            }
            if (p.Scheduled != null && p.Delivered == null)
                ShowDrone.Visibility = Visibility.Visible;
            else
                ShowDrone.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// close parcelwindow  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            allowClosing = true;
            Close();
        }

        /// <summary>
        /// remove drone from the list of drone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                parcel = ThisBl.GetBlParcel(Int32.Parse(ParcelidToPrint.Content.ToString()));
                ThisBl.RemoveParcel(parcel);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("Parcel Removed succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            allowClosing = true;
            Close();
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

        private void TextBoxParcelSenderID_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBoxParceTargetlId_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboParcelWeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboParcelPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// to see the drone that is assigned with the selected parcel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowDrone_Click(object sender, RoutedEventArgs e)
        {
            parcel = ThisBl.GetBlParcel(Int32.Parse(ParcelidToPrint.Content.ToString()));

            Drone temp = default;
            foreach (DroneForList d in ThisBl.GetDronesList())
            {
                if (d.ParcelID == parcel.Id)
                    temp = ThisBl.GetBlDrone(d.ID);
            }
            new DroneWindow(ThisBl, temp).Show();
        }

        /// <summary>
        /// to show the details of the sender of this parcel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowSender_Click(object sender, RoutedEventArgs e)
        {
            ClientForList TempC = default;
            foreach (var item in ThisBl.GetClientList())
            {
                if (item.Name == ParcelSenderToPrint.Content.ToString())
                    TempC = item;
            }
            Client c = ThisBl.GetBlClient(TempC.Id);
            new ClientWindow(ThisBl, c).Show();
        }

        /// <summary>
        ///  to show the details of the recipient of this parcel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowRecipient_Click(object sender, RoutedEventArgs e)
        {
            ClientForList TempC = default;
            foreach (var item in ThisBl.GetClientList())
            {
                if (item.Name == ParcelRecipientToPrint.Content.ToString())
                    TempC = item;
            }
            Client c = ThisBl.GetBlClient(TempC.Id);
            new ClientWindow(ThisBl, c).Show();
        }

        /// <summary>
        /// add a parcel to the list of parcels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            CustomerParcel customerSenderP = default, customerTargetP = default;
            try
            {
                customerSenderP = new CustomerParcel()
                {
                    Id = Int32.Parse(TextBoxParcelSenderID.Text.ToString()),
                    Name = ThisBl.GetBlClient(Int32.Parse(TextBoxParcelSenderID.Text.ToString())).Name
                };
                customerTargetP = new CustomerParcel()
                {
                    Id = Int32.Parse(textBoxParceTargetlId.Text.ToString()),
                    Name = ThisBl.GetBlClient(Int32.Parse(textBoxParceTargetlId.Text.ToString())).Name
                };
                Parcel parcel = new Parcel() //create an object with the entered Data to send to the function AddStation
                {
                    Sender = customerSenderP,
                    Recipient = customerTargetP,
                    Weight = (WeightCategories)ComboParcelWeight.SelectedItem,
                    Priority = (Priorities)ComboParcelPriority.SelectedItem,
                };
                ThisBl.AddParcel(parcel); //add the Station to the list of Stations
            }
            catch (Exception)
            {
                if (customerTargetP == null)
                {
                    textBoxParceTargetlId.BorderBrush = Brushes.Red;
                }
                if (customerSenderP == null)
                {
                    TextBoxParcelSenderID.BorderBrush = Brushes.Red;
                }
                MessageBox.Show("Could not send the parcel, Try again", "ERROR", MessageBoxButton.OKCancel);
                return;
            }
            MessageBox.Show("Parcel sent succesfuly", "Message", MessageBoxButton.OK, MessageBoxImage.Information); //message for user that the drone was Added.
            allowClosing = true;
            Close(); //Close the current window
        }

        /// <summary>
        /// do not add the parcel(cancel)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
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