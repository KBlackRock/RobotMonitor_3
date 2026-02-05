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
using RobotMonitor_3.ViewModels;

namespace RobotMonitor_3.Pages
{
    /// <summary>
    /// Manual_Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Manual_Page : Page
    {
        public Manual_Page()
        {
            InitializeComponent();
            DataContext = new MainWindow_ViewModel();
        }

        private void btn_Home_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).HomeButtonUp();
        }

        // Robot Home Button
        private void btn_Home_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).HomeButtonPress();
        }

        // EMC Manual Control Buttons
        private void btn_EMCSupply_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMCSupply();
        }

        private void btn_EMCOpen_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMCOpen();
        }

        private void btn_EMCClose_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMCClose();
        }

        private void btn_EMCStopper_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMCStopperFWD();
        }

        private void btn_EMCRelease_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMCStopperBWD();
        }

        // Motor 1 Manual Control Buttons
        private void btn_M1_Origin_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).Servo1Home();
        }

        private void btn_M1_Feed_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).Servo1Feed();
        }

        private void btn_M1_Release_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).Servo1Release();
        }

        private void btn_M1_CylLeft_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).Servo1CylinderLeft();
        }

        private void btn_M1_CylRight_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).Servo1CylinderRight();
        }

        // Motor 2 Manual Control Buttons
        private void btn_M2_Origin_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).Servo2Home();
        }

        private void btn_M2_Turn_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).Servo2Turn();
        }

        private void btn_M2_Return_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).Servo2Return();
        }

        // Monitor Off Button
        private void btn_Monitor_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel)?.ScreenOff();
        }
    }
}
