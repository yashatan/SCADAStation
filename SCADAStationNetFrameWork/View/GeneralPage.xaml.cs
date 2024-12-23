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
        List<ClientItem> ClientList;
        public string url;
        public GeneralPage()
        {
            InitializeComponent();
            ClientList = new List<ClientItem>();
            //ClientList.Add("SAMSUNG M32");
            //ClientList.Add("Iphone 13");
            lsbMobileList.ItemsSource = ClientList;
            lsbMobileList.Items.Refresh();
            txtUrl.Text = url;
            txtProjectName.Text = FunctionalLab.currentProjectInformation.Name;
        }

        public GeneralPage(List<ClientItem> clientItems)
        {
            InitializeComponent();
            ClientList = clientItems;
            lsbMobileList.ItemsSource = ClientList;
            lsbMobileList.Items.Refresh();
            txtUrl.Text = url;
            txtProjectName.Text = FunctionalLab.currentProjectInformation.Name;
        }
        public void setUrl(string url)
        {
            txtUrl.Text = url;
        }

        public void Reresh()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                lsbMobileList.Items.Refresh();
            });
        }
    }
}
