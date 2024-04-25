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
    /// Interaction logic for DroneChargeWindow.xaml
    /// </summary>
    public partial class DroneChargeWindow : Window
    {
        BlApi.IBL LastBl;
        DroneCharge droneCharge;
        Station station;
        public DroneChargeWindow(BlApi.IBL BL, DroneCharge dr, Station s)
        {
            LastBl = BL;
            InitializeComponent();
            droneCharge = dr;
            DataContext = droneCharge; //for binding to know the fields of dronecharge
            station = s;
        }

        /// <summary>
        /// close drone charge window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            allowClosing = true;
            Close();
        }

        /// <summary>
        /// remove drone from charge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                station.ListDroneCharge.Remove(droneCharge);
                LastBl.RemoveDroneCharge(droneCharge); //cal the function RemoveDroneCharge to remove the drone
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("Drone charge removed succesfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
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