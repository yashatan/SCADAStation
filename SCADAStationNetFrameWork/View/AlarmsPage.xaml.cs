using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace SCADAStationNetFrameWork
{
    /// <summary>
    /// Interaction logic for AlarmPage.xaml
    /// </summary>
    public partial class AlarmPage : Page
    {
        List<AlarmPoint> alarmpointsList;
        FunctionalLab functionalLab;
        public AlarmPage()
        {
            InitializeComponent();
        }
        public AlarmPage(List<AlarmPoint> alarmPoints, FunctionalLab functionalLab)
        {
            InitializeComponent();
            alarmpointsList = alarmPoints;
            var testpoint = new AlarmPoint()
            {
                Id = 4,
                Name = "Test",
                Text = "Test Content",
                Type = AlarmSetting.AlarmType.Error,
                TimeStamp = DateTime.Now,
            };
            alarmpointsList.Add(testpoint);
            lvAlarm.ItemsSource = alarmpointsList;
            lvAlarm.Items.Refresh();
            this.functionalLab = functionalLab;
        }
        private void btnAckAlarm_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var chosenAlarmPoint = button.DataContext as AlarmPoint;
            functionalLab.ACKAlarmPoint(chosenAlarmPoint.Id);
            lvAlarm.Items.Refresh();
        }
        private void ListViewItems_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewerPage.ScrollToVerticalOffset(ScrollViewerPage.VerticalOffset - e.Delta / 3);
            e.Handled = true;
        }

        public void Refresh()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                lvAlarm.Items.Refresh();
            });
        }
    }

    public class EnumToAlarmTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "Warning";
            if (value is AlarmSetting.AlarmType enumValue)
            {
                switch (enumValue)
                {
                    case AlarmSetting.AlarmType.Warning:
                        result = "Warning";
                        break;
                    case AlarmSetting.AlarmType.Error:
                        result = "Error";
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
