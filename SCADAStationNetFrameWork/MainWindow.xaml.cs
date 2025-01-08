
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Reflection.Emit;
using System.Security.Policy;
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
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using MaterialDesignThemes.Wpf;


namespace SCADAStationNetFrameWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GeneralPage generalPage;
        DevicesPage devicesPage;
        AlarmPage alarmPage;
        TagsPage tagsPage;
        TagLoggingPage tagLoggingPage;

        public MainWindow()
        {
            InitializeComponent();

            SCADAStationController.Instance.AlarmAdded += FunctionalLab_AlarmAdded;
            SCADAStationController.Instance.TagUpdated += FunctionalLab_TagUpdated;
            SCADAStationController.Instance.DeviceUpdated += FunctionalLab_DeviceUpdated;
            SCADAStationController.Instance.NewClientConnected += FunctionalLab_NewClientConnected;
            //lvAlarm.ItemsSource = SCADAStationController.Instance.listAlarmPoints;
            if (generalPage == null)
            {
                generalPage = new GeneralPage(SCADAStationController.Instance.listClient);
                generalPage.setUrl(SCADAStationController.Instance.url);
            }
            if (devicesPage == null)
            {
                devicesPage = new DevicesPage(SCADAStationController.Instance.listcontrolDevices);
            }
            if (alarmPage == null)
            {
                alarmPage = new AlarmPage(SCADAStationController.Instance.listAlarmPoints);
            }
            if (tagsPage == null)
            {
                tagsPage = new TagsPage(SCADAStationController.Instance.listTags);
            }
            if (tagLoggingPage == null)
            {
                tagLoggingPage = new TagLoggingPage(SCADAStationController.Instance.listTagLoggingSettings);
            }

            this.ContentView.Content = generalPage;
        }
        public MainWindow(string filename)
        {
            InitializeComponent();

            SCADAStationController.Instance.SetSCADAStationConfigurationPath(filename);
            SCADAStationController.Instance.AlarmAdded += FunctionalLab_AlarmAdded;
            SCADAStationController.Instance.TagUpdated += FunctionalLab_TagUpdated;
            SCADAStationController.Instance.DeviceUpdated += FunctionalLab_DeviceUpdated;
            SCADAStationController.Instance.NewClientConnected += FunctionalLab_NewClientConnected;
            if (generalPage == null)
            {
                generalPage = new GeneralPage(SCADAStationController.Instance.listClient);
                generalPage.setUrl(SCADAStationController.Instance.url);
            }
            if (devicesPage == null)
            {
                devicesPage = new DevicesPage(SCADAStationController.Instance.listcontrolDevices);
            }
            if (alarmPage == null)
            {
                alarmPage = new AlarmPage(SCADAStationController.Instance.listAlarmPoints);
            }
            if (tagsPage == null)
            {
                tagsPage = new TagsPage(SCADAStationController.Instance.listTags);
            }
            if (tagLoggingPage == null)
            {
                tagLoggingPage = new TagLoggingPage(SCADAStationController.Instance.listTagLoggingSettings);
            }

            this.ContentView.Content = generalPage;
        }
        private void FunctionalLab_NewClientConnected(object sender, EventArgs e)
        {
            generalPage.Reresh();
        }

        private void FunctionalLab_DeviceUpdated(object sender, EventArgs e)
        {
            devicesPage.Refresh();
        }

        private void FunctionalLab_AlarmAdded(object sender, EventArgs e)
        {
            alarmPage.Refresh();
        }

        private void FunctionalLab_TagUpdated(object sender, EventArgs e)
        {
            tagsPage.Refresh();
        }




        private void btnAckAlarm_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var chosenAlarmPoint = button.DataContext as AlarmPoint;
            //SCADAStationController.Instance.ACKAlarmPoint(chosenAlarmPoint.Id);
        }

        #region MenuItem
        private void MenuItemTagLogging_Click(object sender, RoutedEventArgs e)
        {

            if (tagLoggingPage == null)
            {
                tagLoggingPage = new TagLoggingPage(SCADAStationController.Instance.listTagLoggingSettings);
            }
            this.ContentView.Content = tagLoggingPage;
        }

        private void MenuItemGeneral_Click(object sender, RoutedEventArgs e)
        {
            if (generalPage == null)
            {
                generalPage = new GeneralPage();
            }

            this.ContentView.Content = generalPage;
        }

        private void MenuItemDevices_Click(object sender, RoutedEventArgs e)
        {
            if (devicesPage == null)
            {
                devicesPage = new DevicesPage(SCADAStationController.Instance.listcontrolDevices);
            }

            this.ContentView.Content = devicesPage;
        }

        private void MenuItemTags_Click(object sender, RoutedEventArgs e)
        {
            if (tagsPage == null)
            {
                tagsPage = new TagsPage(SCADAStationController.Instance.listTags);
            }
            this.ContentView.Content = tagsPage;
        }

        private void MenuItemAlarms_Click(object sender, RoutedEventArgs e)
        {
            if (alarmPage == null)
            {
                alarmPage = new AlarmPage(SCADAStationController.Instance.listAlarmPoints);
            }
            this.ContentView.Content = alarmPage;
        }

        #endregion
        #region TestZone
        private void TestSend_Click(object sender, RoutedEventArgs e)
        {
            SCADAStationController.Instance.testfunc();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SCADAStationController.Instance.testfunc2();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.F12 && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if(TestMenuItem1.Visibility == Visibility.Collapsed)
                {
                    TestMenuItem1.Visibility = Visibility.Visible;
                    TestMenuItem2.Visibility = Visibility.Visible;
                }
                else
                {
                    TestMenuItem1.Visibility = Visibility.Collapsed;
                    TestMenuItem2.Visibility = Visibility.Collapsed;
                }
;
            }
        }
        #endregion

    }
}
