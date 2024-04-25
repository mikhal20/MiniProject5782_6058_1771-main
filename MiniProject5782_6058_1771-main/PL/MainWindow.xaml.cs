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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlApi;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BlApi.IBL myBl;
        public MainWindow()
        {
            myBl = BlApi.BlFactory.GetBl();
            InitializeComponent();
        }

        /// <summary>
        /// press the button to sign in or sign up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSign_Click(object sender, RoutedEventArgs e)
        {
            new SignWindow(myBl).Show();
        }
    }
}
