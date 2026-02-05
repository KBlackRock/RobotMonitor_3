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
    /// Setting_Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Setting_Page : Page
    {
        public Setting_Page()
        {
            InitializeComponent();
            DataContext = new MainWindow_ViewModel();
        }

        private void btn_DeviceSet_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).DeviceNameSet();
        }

        #region Robot Page
        // Robot Controll
        private void tBox__PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobSpeedSet_Percent();
        }

        private void tBox_1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobSpeedSet_MMS();
        }

        private void tBox_2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).PressCloseTimeSet();
        }

        private void tBox_3_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).ChamberCloseTimeSet();
        }

        // Air Blow Controll
        private void tBox_BlowSpeed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobBlowSpeedSet();
        }

        private void tBox_Blow1stTop_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobBlowTop1();
        }

        private void tBox_Blow1stBot_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobBlowBot1();
        }

        private void tBox_Blow2ndTop_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobBlowTop2();
        }

        private void tBox_Blow2ndBot_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobBlowBot2();
        }

        private void tBox_BlowPort_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobBlowPort();
        }

        // Extracter Spray Controll
        private void tBox_SpraySpeed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobSpraySpeedSet();
        }

        private void tBox_SprayTop_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobSprayTop();
        }

        private void tBox_SprayBot_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobSprayPort();
        }

        // Port Cleaning Controll
        private void tBox_PortSpeed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobPortSpeedSet();
        }

        private void tBox_PortCount_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobPortCountSet();
        }
        #endregion

        #region Servo/Cylinder Page
        private void tBox_M1Speed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).M1_SpeedSet();
        }

        private void tBox_M2Speed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).M2_SpeedSet();
        }

        private void tBox_EMCOpen_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMC_DoorOpenDelaySet();
        }

        private void tBox_EMCClose_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMC_DoorCloseDelaySet();
        }

        private void tBox_EMCFWD_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMC_StopperFWDDelaySet();
        }

        private void tBox_EMCBWD_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMC_StopperBWDDelaySet();
        }
        #endregion

        #region Panel Page
        // TCP/IP Setting
        private void tBox_IP_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).IPAddressBox();
        }

        private void tBox_Port_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).PortBox();
        }

        private void btn_ServerConnect_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).ServerOpenClose();
        }

        // Button Delay Setting
        private void tBox_StartDelay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).StartDelayBox();
        }

        private void tBox_HomeDelay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).HomeDelayBox();
        }

        private void tBox_CountResetDelay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).CountResetDelayBox();
        }


        // Press Signal Set

        private void btn_Out1_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).OutBtn1Click();
        }

        private void btn_Out2_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).OutBtn2Click();
        }

        private void btn_Out3_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).OutBtn3Click();
        }

        private void btn_Out4_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).OutBtn4Click();
        }

        private void btn_Out5_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).OutBtn5Click();
        }

        private void btn_Out6_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).OutBtn6Click();
        }
        #endregion
    }
}
