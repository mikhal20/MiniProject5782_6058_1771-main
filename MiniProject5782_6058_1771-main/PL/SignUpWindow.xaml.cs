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
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        BlApi.IBL LastBL;
        public struct Worker  //creating a worker struct in order to sign in and up
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
        }

        List<Worker> workers; //create a list of workers
        public SignUpWindow(BlApi.IBL bL, object WorkerList)
        {
            LastBL = bL;
            workers = (List<Worker>)WorkerList;
            InitializeComponent();
        }

        private void Id_TextChanged(object sender, TextChangedEventArgs e)
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

        private void ComboTypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// close sign up window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            allowClosing = true;
            Close();
        }

        /// <summary>
        /// after writing all your details press "Ok" to sign up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (ComboTypeSelector.SelectedIndex == -1)
            {
                MessageBox.Show("Could Not Sign Up, Try Again", "ERROR", MessageBoxButton.OKCancel);
                return;
            }
            if (EnterPhoneNumber.Text[0] != '0')
            {
                EnterPhoneNumber.BorderBrush = Brushes.Red;
                MessageBox.Show("Could Not Sign Up, Try Again", "ERROR", MessageBoxButton.OKCancel);
                return;
            }
            Location tempLocation = new Location() //if the string contains a "." change it to a "," in order to convert to double
            {
                Latitude = Convert.ToDouble(EnterYourLatitude.Text.ToString().Replace('.', ',')),
                Longitude = Convert.ToDouble(EnterYourLongitude.Text.ToString().Replace('.', ','))
            };
            if (ComboTypeSelector.SelectedIndex == 1) //if client was selected to add
            {
                Client c = new Client() //create a client object with the details entered
                {
                    Id= Int32.Parse(ID.Text.ToString()),
                    Name=EnterName.Text.ToString(),
                    Location= tempLocation,
                    Phone=EnterPhoneNumber.Text.ToString(),
                    ReceiveParcels=null,
                    SentParcels=null
                };
                try
                {
                    LastBL.AddClient(c); //add the client
                }
                catch(Exception)
                {
                    MessageBox.Show("Could Not Sign Up, Try Again", "ERROR", MessageBoxButton.OKCancel);
                    return;
                }
            }
            else //if employee was selected to add
            {
                Worker Employee = new Worker() //create a worker object to add to the list of workers
                {
                    Id = Int32.Parse(ID.Text.ToString()),
                    Name = EnterName.Text.ToString(),
                    Phone = EnterPhoneNumber.Text.ToString(),
                };
                workers.Add(Employee);
            }
            MessageBox.Show("Welcome! you can Sign In now :) ", "Message", MessageBoxButton.OK, MessageBoxImage.Information); //message for user that the drone was Added.
            allowClosing = true;
            Close(); //Close the current window
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