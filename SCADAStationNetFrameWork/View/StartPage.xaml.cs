﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        const string LastConfigFilePathPath = "Data\\LastConfigFilePath.txt";
        public StartPage()
        {
            InitializeComponent();
            LoadSCADAServerPATH();
            txtFileLocation.Text = filePath_SCADAStationConfiguration;

        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Configuration files (*Station.json)|*Station.json|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                filePath_SCADAStationConfiguration = openFileDialog.FileName;
                txtFileLocation.Text = filePath_SCADAStationConfiguration;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            filePath_SCADAStationConfiguration = txtFileLocation.Text;
            SCADAStationController.Instance.SetSCADAStationConfigurationPath(filePath_SCADAStationConfiguration);
            if (SCADAStationController.Instance.LoadFileStatus)
            {
                SaveSCADAServerPATH();
                MainWindow mainWindow = new MainWindow();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Show();
                this.Close();
            }

        }
        void LoadSCADAServerPATH()
        { 
            filePath_SCADAStationConfiguration = File.ReadAllText(LastConfigFilePathPath);
        }

        void SaveSCADAServerPATH()
        {
            File.WriteAllText(LastConfigFilePathPath, filePath_SCADAStationConfiguration);

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
