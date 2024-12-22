using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
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

namespace SCADAStationNetFrameWork
{
    /// <summary>
    /// Interaction logic for DevicesPage.xaml
    /// </summary>
    public partial class DevicesPage : Page
    {
        List<ControlDevice> devicesList;
        public DevicesPage()
        {
            InitializeComponent();
        }
        public DevicesPage(List<ControlDevice> controlDevices)
        {
            InitializeComponent();
            devicesList = controlDevices;
            lvDevices.ItemsSource = devicesList;
            lvDevices.Items.Refresh();
        }
        private void ListViewItems_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewerPage.ScrollToVerticalOffset(ScrollViewerPage.VerticalOffset - e.Delta / 3);
            e.Handled = true;
        }

        public void Refresh() {
            Application.Current.Dispatcher.Invoke(() =>
            {
                lvDevices.Items.Refresh();
            });
        }
    }
    public class EnumToTypeConnectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "Unidentified";
            if (value is int enumValue)
            {
                switch (enumValue)
                {
                    case (int)ConnectDevice.emConnectionType.emS7:
                        result = "S7";
                        break;
                    case (int)ConnectDevice.emConnectionType.emTCP:
                        result = "Modbus TCP";
                        break;
                    case (int)ConnectDevice.emConnectionType.emOPCUA:
                        result = "OPC UA";
                        break;
                }
            }
            return result;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return ConnectDevice.emConnectionType.emS7;
        }
    }
}

