using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using RobotMonitor_3.ViewModels;
using System.Net.Sockets;
using System.Net;

namespace RobotMonitor_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindow_ViewModel();

            (DataContext as MainWindow_ViewModel).Opening(this);
        }

        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).Closing();
        }

        private void btn_Setting_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).SettingButtonPress();
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).ResetButtonPress();
        }

        private void btn_Reset_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).ResetButtonPress();
        }

        private void btn_Reset_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).ResetButtonUp();
        }

        private void btn_Stop_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).StopButtonUp();
        }

        private void btn_Stop_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).StopButtonPress();
        }

        private void btn_Start_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).StartButtonDown(e);
        }

        private void btn_Start_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).StartButtonUp();
        }

        private void btn_Auto_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).AutoButtonPress();
        }

        private void btn_Manual_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).ManualButtonPress();
        }

        private void btn_Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btn_SetView_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).SetViewButtonPress();
        }


    }
}