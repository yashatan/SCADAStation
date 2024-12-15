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
    /// Interaction logic for TagsPage.xaml
    /// </summary>
    public partial class TagsPage : Page
    {
        List<TagInfo> tagslist;
        public TagsPage()
        {
            InitializeComponent();
        }

        public TagsPage(List<TagInfo> taginfos)
        {
            InitializeComponent();
            tagslist = taginfos;
            lvTags.ItemsSource = tagslist;
        }

        private void ListViewItems_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewerPage.ScrollToVerticalOffset(ScrollViewerPage.VerticalOffset - e.Delta / 3); 
            e.Handled = true;
        }
    }
}
