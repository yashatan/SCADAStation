
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


namespace SCADAStationNetFrameWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FunctionalLab functionalLab;
        public MainWindow()
        {
            InitializeComponent();

            functionalLab = new FunctionalLab();
            url.Text = $"Server started at:{functionalLab.url}";
            functionalLab.AlarmAdded += FunctionalLab_AlarmAdded;
            lvAlarm.ItemsSource = functionalLab.listTrendPoints;
        }

        private void FunctionalLab_AlarmAdded(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => { lvAlarm.Items.Refresh(); });
        }

        public MainWindow(string fileName)
        {
            InitializeComponent();

            functionalLab = new FunctionalLab(fileName);
            url.Text = $"Server started at:{functionalLab.url}";
        }

        private void TestSend_Click(object sender, RoutedEventArgs e)
        {
            functionalLab.testfunc();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TrendPoint trendPoint = new TrendPoint() { TimeStamp = DateTime.Now, Value = 12.0, TagLoggingId = 0 };
            //context.TrendPoints.Add(trendPoint);
            //SCADAStationDbContext.Instance.TrendPoints.Add(trendPoint);
            functionalLab.SendTrendPointToClient(trendPoint);
            //SCADAStationDbContext.Instance.SaveChanges();

        }


        private void btnAckAlarm_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var chosenAlarmPoint = button.DataContext as AlarmPoint;
            functionalLab.ACKAlarmPoint(chosenAlarmPoint.Id);
        }
    }
}
