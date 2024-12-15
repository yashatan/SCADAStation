using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SCADAStationNetFrameWork
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            bool configFormat = false;
            string fileName ="";
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "-f")
                {
                    configFormat = true;
                }
                else
                {
                    if (configFormat)
                    {
                        fileName = e.Args[i];
                    }
                }
            }

            // Create main application window, starting minimized if specified
            if (configFormat)
            {
                MainWindow mainWindow = new MainWindow(fileName);
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Show();
            }
            else
            {
                StartPage startPage = new StartPage();
                startPage.WindowState = WindowState.Normal;
                startPage.Show();
            }

        }
    }
}
