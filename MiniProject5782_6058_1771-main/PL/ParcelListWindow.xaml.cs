using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ParcelListWindow.xaml
    /// </summary>
    
    public partial class ParcelListWindow : Window, INotifyPropertyChanged
    {
        BlApi.IBL bL;

        private List<ParcelForList> parcels;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public ParcelListWindow(BlApi.IBL Bl)
        {
            bL = Bl;
            InitializeComponent();
            parcels = bL.GetParcelList().ToList();
            DataContext = this;
            ComboWeightSelector.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));
            ComboStatusSelector.ItemsSource = Enum.GetValues(typeof(BO.Status));
            ComboPrioritiesSelector.ItemsSource = Enum.GetValues(typeof(BO.Priorities));
        }

        /// <summary>
        /// property for list of parcel (for bunding)
        /// </summary>
        public List<ParcelForList> Parcels
        {
            get { return parcels; }
            set { parcels = value; PropertyChanged(this, new PropertyChangedEventArgs("Parcels")); }
        }

        /// <summary>
        /// allow only to enter numbers for drone's id
        /// https://stackoverflow.com/questions/49546723/binding-text-change-event-to-prevent-invalid-input-wpf-xaml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumbersOnly(object sender, TextCompositionEventArgs e) //If the user attempts to enter an invalid input I would like to prevent it
        {
            Regex regex = new Regex("[^0-9]+"); //only numbers allowed (-is inaccessible)
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
        /// to close the parcel window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            allowClosing = true;
            Close();
        }
       
        private void ParcelsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 

        }

        /// <summary>
        /// open the selected parcel to see its details or update it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParcelListView_doubleClick(object sender, MouseButtonEventArgs e)
        {
            ParcelForList parcel = ParcelsListView.SelectedItem as ParcelForList; //put the selected Parcel in parcel
            Parcel p = bL.GetBlParcel(parcel.Id); //gets the parcel from BL as a "Parcel" type
            new ParcelWindow(bL, p).ShowDialog(); //open to see details and or update

            ParcelsListView.ItemsSource = bL.GetParcelList();
        }

        /// <summary>
        /// refresh the list of the parcels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshWindow_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<ParcelForList> temp = bL.GetParcelList();
            ParcelsListView.ItemsSource = temp;
        }

        /// <summary>
        /// to group the parcels by sender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupeSender_Click(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelsListView.ItemsSource); //grouping by sender
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("SenderName");
            view.GroupDescriptions.Add(groupDescription);
        }

        /// <summary>
        /// to add a parcel to the list of parcels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddParcel_Click(object sender, RoutedEventArgs e)
        {
            new ParcelWindow(bL, ParcelsListView).ShowDialog(); //Click and open parcel Window

            IEnumerable<ParcelForList> temp = bL.GetParcelList();
            ParcelsListView.ItemsSource = temp;
        }

        /// <summary>
        /// select by weight 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboWeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BO.WeightCategories weight = (BO.WeightCategories)ComboWeightSelector.SelectedItem;

            try //get the parcels with the selected Weight
            {
                IEnumerable<ParcelForList> temp = bL.GetParcelList();

                if (ComboStatusSelector.SelectedItem != null) //if there is a selected status get only those parcels
                    temp = temp.Where(x => x.Status == (Status)ComboStatusSelector.SelectedItem);
                if (ComboPrioritiesSelector.SelectedItem != null) //if there is a selected priority get only those parcels
                    temp = temp.Where(x => x.Priority == (BO.Priorities)ComboPrioritiesSelector.SelectedItem);

                ParcelsListView.ItemsSource = temp.Where(x => x.Weight == weight);
            }
            catch (Exception)
            { }   
        }

        /// <summary>
        /// select by priorities
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboPrioritiesSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BO.Priorities Priority = (BO.Priorities)ComboPrioritiesSelector.SelectedItem;

            try //get the parcels with the selected priority
            {
                IEnumerable<ParcelForList> temp = bL.GetParcelList();

                if (ComboStatusSelector.SelectedItem != null) //if there is a selected status get only those parcels
                    temp = temp.Where(x => x.Status == (Status)ComboStatusSelector.SelectedItem);
                if (ComboWeightSelector.SelectedItem != null) //if there is a selected Weight get only those parcels
                    temp = temp.Where(x => x.Weight == (BO.WeightCategories)ComboWeightSelector.SelectedItem);

                ParcelsListView.ItemsSource = temp.Where(x => x.Priority == Priority);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// to select by status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboStatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Status status = (Status)ComboStatusSelector.SelectedItem;
            try //get the parcels with the selected status
            {
                IEnumerable<ParcelForList> temp = bL.GetParcelList();

                if (ComboWeightSelector.SelectedItem != null) //if there is a selected Weight get only those parcels
                    temp = temp.Where(x => x.Weight == (BO.WeightCategories)ComboWeightSelector.SelectedItem);
                if (ComboPrioritiesSelector.SelectedItem != null) //if there is a selected priority get only those parcels
                    temp = temp.Where(x => x.Priority == (BO.Priorities)ComboPrioritiesSelector.SelectedItem);

                ParcelsListView.ItemsSource = temp.Where(x => x.Status == status);
            }
            catch (Exception)
            { }
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
