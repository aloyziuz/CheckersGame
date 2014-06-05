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

namespace NetworkedCheckersClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void AddSetNetworkMenuHandler(RoutedEventHandler handler)
        {
            setNetworkMenu.Click += handler;
        }

        public void AddLoginMenuHandler(RoutedEventHandler handler)
        {
            loginMenu.Click += handler;
        }

        public void AddLogoutMenuHandler(RoutedEventHandler handler)
        {
            logoutmenu.Click += handler;
        }

        public void AddSetMulticastMenuHandler(RoutedEventHandler handler)
        {
            setMulticastMenu.Click += handler;
        }

        public void AddListViewItemHandler(MouseButtonEventHandler handler)
        {
            MouseDoubleClick += handler;
        }
    }
}
