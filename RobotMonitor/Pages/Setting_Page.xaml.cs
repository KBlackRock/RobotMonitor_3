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
            this.DataContext = Application.Current.MainWindow.DataContext;
        }

        private void btn_DeviceSet_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).DeviceNameSet();
        }
        private void btn_Interface_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).InterfaceView();
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

        #region Auto Loader Page
        private void tBox_M1_PickUpPos_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).M1_PickUpPosSet();
        private void tBox_M1_FirstPos_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).M1_FirstPosSet();
        private void tBox_M1_SecondPos_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).M1_SecondPosSet();
        private void tBox_M1_CheckPos_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).M1_CheckPosSet();
        private void tBox_M1_HighSpeed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).M1_HighSpeedSet();
        private void tBox_M1_LowSpeed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).M1_LowSpeedSet();

        private void tBox_M2_1stPos_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).M2_1stPosSet();
        private void tBox_M2_Pitch_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).M2_PitchSet();
        private void tBox_M2_Speed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).M2_SpeedSet();

        private void tBox_MoveCyl1DownDelay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).MoveCyl1DownDelaySet();
        private void tBox_MoveCyl2DownDelay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).MoveCyl2DownDelaySet();
        private void tBox_MoveCyl1UpDelay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).MoveCyl1UpDelaySet();
        private void tBox_MoveCyl2UpDelay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).MoveCyl2UpDelaySet();
        private void tBox_FixCylUpDelay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).FixCylUpDelaySet();
        private void tBox_FixCylDownDelay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainWindow_ViewModel).FixCylDownDelaySet();
        #endregion

        #region Panel Page
        // TCP/IP Setting
        private void tBox_RobIP_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).IPAddressBox_Robot();
        }

        private void tBox_RobPort_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).PortBox_Robot();
        }

        private void tBox_PlcIP_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).IPAddressBox_PLC();
        }

        private void tBox_PlcPort_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).PortBox_PLC();
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
