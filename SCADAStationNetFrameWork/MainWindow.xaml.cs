
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

        //private void FunctionalLab_AlarmAdded(object sender, EventArgs e)
        //{
        //    Dispatcher.Invoke(() => { lvAlarm.Items.Refresh(); });
        //}

        public MainWindow(FunctionalLab pfunctionalLab)
        {
            InitializeComponent();

            functionalLab = pfunctionalLab;
            //functionalLab.AlarmAdded += FunctionalLab_AlarmAdded;
            //lvAlarm.ItemsSource = functionalLab.listAlarmPoints;
            if (generalPage == null)
            {
                generalPage = new GeneralPage();
                generalPage.setUrl(functionalLab.url);
            }
            this.ContentView.Content = generalPage;
        }
        public MainWindow(string filename)
        {
            InitializeComponent();

            functionalLab = new FunctionalLab(filename);
            //functionalLab.AlarmAdded += FunctionalLab_AlarmAdded;
            //lvAlarm.ItemsSource = functionalLab.listAlarmPoints;
            if (generalPage == null)
            {
                generalPage = new GeneralPage();
                generalPage.setUrl(functionalLab.url);
            }
            this.ContentView.Content = generalPage;
        }


        private void TestSend_Click(object sender, RoutedEventArgs e)
        {
            functionalLab.testfunc();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //TrendPoint trendPoint = new TrendPoint() { TimeStamp = DateTime.Now, Value = 12.0, TagLoggingId = 0 };
            ////context.TrendPoints.Add(trendPoint);
            ////SCADAStationDbContext.Instance.TrendPoints.Add(trendPoint);
            //functionalLab.SendTrendPointToClient(trendPoint);
            //SCADAStationDbContext.Instance.SaveChanges();

            //TagInfo tagInfo = new TagInfo();
            //tagInfo.Type = TagInfo.TagType.eDouble;
            //tagInfo.BitPosition = 1;
            //tagInfo.Data = 1085276160;
            //Trace.WriteLine(tagInfo.Value);
            //tagInfo.Value = "-3005";
            //Trace.WriteLine(tagInfo.Data);
            //Trace.WriteLine(tagInfo.Value);
            //ControlDevice device = new ControlDevice();
            //device.test();
            functionalLab.testfunc2();
        }


        private void btnAckAlarm_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var chosenAlarmPoint = button.DataContext as AlarmPoint;
            functionalLab.ACKAlarmPoint(chosenAlarmPoint.Id);
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
