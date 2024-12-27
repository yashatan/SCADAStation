
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
        FunctionalLab functionalLab;
        GeneralPage generalPage;
        DevicesPage devicesPage;
        AlarmPage alarmPage;
        TagsPage tagsPage;
        TagLoggingPage tagLoggingPage;
        public MainWindow()
        {
            InitializeComponent();

            functionalLab = new FunctionalLab();
            //functionalLab.AlarmAdded += FunctionalLab_AlarmAdded;
            //lvAlarm.ItemsSource = functionalLab.listAlarmPoints;
        }

        public MainWindow(FunctionalLab pfunctionalLab)
        {
            InitializeComponent();

            functionalLab = pfunctionalLab;
            functionalLab.AlarmAdded += FunctionalLab_AlarmAdded;
            functionalLab.TagUpdated += FunctionalLab_TagUpdated;
            functionalLab.DeviceUpdated += FunctionalLab_DeviceUpdated;
            functionalLab.NewClientConnected += FunctionalLab_NewClientConnected;
            //lvAlarm.ItemsSource = functionalLab.listAlarmPoints;
            if (generalPage == null)
            {
                generalPage = new GeneralPage(pfunctionalLab.listClient);
                generalPage.setUrl(functionalLab.url);
            }
            if (devicesPage == null)
            {
                devicesPage = new DevicesPage(functionalLab.listcontrolDevices);
            }
            if (alarmPage == null)
            {
                alarmPage = new AlarmPage(functionalLab.listAlarmPoints, functionalLab);
            }
            if (tagsPage == null)
            {
                tagsPage = new TagsPage(functionalLab.listTags);
            }
            if (tagLoggingPage == null)
            {
                tagLoggingPage = new TagLoggingPage(functionalLab.listTagLoggingSettings);
            }

            this.ContentView.Content = generalPage;
        }
        public MainWindow(string filename)
        {
            InitializeComponent();

            functionalLab = new FunctionalLab(filename);
            functionalLab.AlarmAdded += FunctionalLab_AlarmAdded;
            functionalLab.TagUpdated += FunctionalLab_TagUpdated;
            functionalLab.DeviceUpdated += FunctionalLab_DeviceUpdated;
            functionalLab.NewClientConnected += FunctionalLab_NewClientConnected;
            //lvAlarm.ItemsSource = functionalLab.listAlarmPoints;
            if (generalPage == null)
            {
                generalPage = new GeneralPage(functionalLab.listClient);
                generalPage.setUrl(functionalLab.url);
            }
            if (devicesPage == null)
            {
                devicesPage = new DevicesPage(functionalLab.listcontrolDevices);
            }
            if (alarmPage == null)
            {
                alarmPage = new AlarmPage(functionalLab.listAlarmPoints, functionalLab);
            }
            if (tagsPage == null)
            {
                tagsPage = new TagsPage(functionalLab.listTags);
            }
            if (tagLoggingPage == null)
            {
                tagLoggingPage = new TagLoggingPage(functionalLab.listTagLoggingSettings);
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

        private void TestSend_Click(object sender, RoutedEventArgs e)
        {
            functionalLab.testfunc();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            functionalLab.testfunc2();
        }


        private void btnAckAlarm_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var chosenAlarmPoint = button.DataContext as AlarmPoint;
            //functionalLab.ACKAlarmPoint(chosenAlarmPoint.Id);
        }

        #region MenuItem
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemTagLogging_Click(object sender, RoutedEventArgs e)
        {

            if (tagLoggingPage == null)
            {
                tagLoggingPage = new TagLoggingPage(functionalLab.listTagLoggingSettings);
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
                devicesPage = new DevicesPage(functionalLab.listcontrolDevices);
            }

            this.ContentView.Content = devicesPage;
        }

        private void MenuItemTags_Click(object sender, RoutedEventArgs e)
        {
            if (tagsPage == null)
            {
                tagsPage = new TagsPage(functionalLab.listTags);
            }
            this.ContentView.Content = tagsPage;
        }

        private void MenuItemAlarms_Click(object sender, RoutedEventArgs e)
        {
            if (alarmPage == null)
            {
                alarmPage = new AlarmPage(functionalLab.listAlarmPoints, functionalLab);
            }
            this.ContentView.Content = alarmPage;
        }
        #endregion

    }
}
