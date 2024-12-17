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
    /// Interaction logic for GeneralPage.xaml
    /// </summary>
    public partial class GeneralPage : Page
    {
        List<string> deviceList;
        public string url;
        public GeneralPage()
        {
            InitializeComponent();
            deviceList = new List<string>();
            deviceList.Add("SAMSUNG M32");
            deviceList.Add("Iphone 13");
            lsbMobileList.ItemsSource = deviceList;
            lsbMobileList.Items.Refresh();
            txtUrl.Text = url;
        }
        public void setUrl(string url)
        {
            txtUrl.Text = url;
        }
    }
}
