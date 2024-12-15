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
using System.Windows.Shapes;

namespace SCADAStationNetFrameWork
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Window
    {
        string filePath_SCADAStationConfiguration;
        public StartPage()
        {
            InitializeComponent();
            filePath_SCADAStationConfiguration = "C:\\Users\\Admin\\Work\\DemoSCADA\\DemoSCADAStation.json";
            txtFileLocation.Text = filePath_SCADAStationConfiguration;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = new MainWindow(filePath_SCADAStationConfiguration);
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Show();
            this.Close();
        }

        private void CloseIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void MinimizeIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void DockPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
