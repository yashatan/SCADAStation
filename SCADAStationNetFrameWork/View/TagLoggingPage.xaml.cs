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

namespace SCADAStationNetFrameWork
{
    /// <summary>
    /// Interaction logic for TagLoggingPages.xaml
    /// </summary>
    public partial class TagLoggingPage : Page
    {
        List<TagLoggingSetting> settingList;
        DateTime StartDateTime;
        DateTime EndDateTime;
        TagLoggingSetting currentSetting;
        public TagLoggingPage()
        {
            InitializeComponent();

        }

        public TagLoggingPage(List<TagLoggingSetting> tagloggingSettings) 
        {
            InitializeComponent();
            settingList = tagloggingSettings;
            pickerStartDate.SelectedDate = DateTime.Now;
            pickerStartTime.SelectedTime = DateTime.Now;
            pickerEndTime.SelectedTime = DateTime.Now;
            pickerEndDate.SelectedDate = DateTime.Now;
            cbbTagLogging.ItemsSource = settingList;
            cbbTagLogging.SelectedIndex = 0;
            cbbTagLogging.Items.Refresh();
        }
        private void ListViewItems_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewerPage.ScrollToVerticalOffset(ScrollViewerPage.VerticalOffset - e.Delta / 3);
            e.Handled = true;
        }

        private void btnGetData_Click(object sender, RoutedEventArgs e)
        {
            currentSetting = cbbTagLogging.SelectedItem as TagLoggingSetting;
            DateTime StartDate = (DateTime) pickerStartDate.SelectedDate;
            DateTime StartTime = (DateTime) pickerStartTime.SelectedTime;
            DateTime EndDate = (DateTime) pickerEndDate.SelectedDate;
            DateTime EndTime = (DateTime) pickerEndTime.SelectedTime;

            if (StartDate == null || EndDate == null)
            {
                MessageBox.Show("Please choose valid Start Date and End Date");
                return;
            }
            else
            {
                StartDateTime = StartDate.Date + StartTime.TimeOfDay;
                EndDateTime = EndDate.Date + EndTime.TimeOfDay;
            }

            if (StartDateTime > EndDateTime)
            {
                MessageBox.Show("Please set Start point earlier End point");
                return;
            }

            if (currentSetting != null) { 
                var listPoint = SCADAStationDbContext.Instance.TrendPoints.Where(m => m.TagLoggingId == currentSetting.Id).Where(m => (m.TimeStamp > StartDateTime) && (m.TimeStamp < EndDateTime)).Take(Convert.ToInt16(txtMaximunPoints.Text)).ToList();
                lvDataPoint.ItemsSource = listPoint;
                lvDataPoint.Items.Refresh();
            }
        }
    }
}
