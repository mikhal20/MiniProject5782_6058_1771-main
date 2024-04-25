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
using static PL.SignUpWindow;

namespace PL
{
    /// <summary>
    /// Interaction logic for SignWindow.xaml
    /// </summary>
    public partial class SignWindow : Window
    {
        BlApi.IBL bL;

        public List<Worker> WorkerList = new List<Worker>(); //list of workers
        Worker Director = new Worker() //creating a temp Director (Worker Type) to sign in easily
        {
            Id = 123456789,
            Name = "Director Name",
            Phone= "0587310903"
        };
      
        public SignWindow(BlApi.IBL Bl)
        {
            bL = Bl;
            InitializeComponent();
            WorkerList.Add(Director);
        }

        /// <summary>
        /// Allow client to sign in 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSignClient_Click(object sender, RoutedEventArgs e)
        {
            bool flag=false;
            ClientForList tempC = default;
            foreach (var client in bL.GetClientList()) //search if the client already exist
            {
                if (client.Id == Int32.Parse(ClientId.Text))
                {
                    flag = true;
                    tempC = client;
                }
            }
            if (!flag) //if the client doesn't exist
            {
                MessageBox.Show("You don't have an account yet? \n Sign Up!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnSignClient.IsEnabled = false;
            }
            else
                new ClientManageWindow(bL, tempC).Show(); //open window if the client exist
            ClientId.Clear();
        }

        /// <summary>
        /// allow Employee to sign in
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSignEmployee_Click(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            foreach (var worker in WorkerList) //search if the Employee already exist
            {
                if (worker.Id==Int32.Parse(EmployeeId.Text))
                    flag = true;
            }
            if (!flag) //if the Employee doesn't exist
            {
                MessageBox.Show("You don't have an account yet? \n Sign Up!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnSignEmployee.IsEnabled = false;
            }
            else
                new EmployeeWindow(bL).Show(); //open window if the client exist
            EmployeeId.Clear(); //clear the textBox from former employee
        }

        /// <summary>
        /// open a new window to create an account - sign up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSignUp_Click(object sender, RoutedEventArgs e)
        {
            new SignUpWindow(bL, WorkerList).Show();
        }

        /// <summary>
        /// allow the Employee to sign in only if there are 9 digits
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmployeeId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EmployeeId.Text != "Enter Id" && EmployeeId.Text != "") //only if there are 9 digits to the id allow to sign in
            {
                if (Int32.Parse(EmployeeId.Text) >= 100000000 && Int32.Parse(EmployeeId.Text) <= 999999999)
                    BtnSignEmployee.IsEnabled = true;
                else
                    BtnSignEmployee.IsEnabled = false;
            }
        }

        /// <summary>
        /// allow the client to sign in only if there are 9 digits
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ClientId.Text != "Enter Id" && ClientId.Text !="") //only if there are 9 digits to the id allow to sign in
            {
                if (Int32.Parse(ClientId.Text) >= 100000000 && Int32.Parse(ClientId.Text) <= 999999999)
                    BtnSignClient.IsEnabled = true;
                else
                    BtnSignClient.IsEnabled = false;
            }
        }

        /// <summary>
        /// Close Sign in/up window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
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
