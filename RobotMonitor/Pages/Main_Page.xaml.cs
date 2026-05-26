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
    /// Main_Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Main_Page : Page
    {
        public Main_Page()
        {
            InitializeComponent();
            this.DataContext = Application.Current.MainWindow.DataContext;
            //DataContext = new MainWindow_ViewModel();

        }

        private void ComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).WorkModeOpen();
        }

        private void btn_CountReset_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).CountResetPressUp();
        }

        private void btn_CountReset_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).WorkCountReset();
        }
        private void btn_PCBEMCSkip_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).LoadingsSkipPress();
        }

        private void btn_PCBEMCSkip_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).LoadingSkipRelease();
        }


        private void CB_WS_DropDownClosed(object sender, EventArgs e)
        {
            (DataContext as MainWindow_ViewModel).WorkModeClose();
        }

        private void btn_Heater1_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).Heater1Press();
        }

        private void btn_Heater2_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).Heater2Press();
        }
    }
}
