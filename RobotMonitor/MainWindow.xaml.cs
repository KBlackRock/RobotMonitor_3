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
using RobotMonitor_2.ViewModels;
using System.Net.Sockets;
using System.Net;

namespace RobotMonitor_2
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

        private void btn_Home_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).HomeButtonUp();
        }

        private void btn_CountReset_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).CountResetPressUp();
        }

        private void btn_CountReset_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).WorkCountReset();
        }

        private void btn_Home_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).HomeButtonPress();
        }

        private void tBox_IP_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).IPAdressBox();
        }

        private void tBox_Port_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).PortBox();
        }

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

        private void btn_ServerConnect_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).ServerOpenClose();
        }

        private void btn_EMCSupply_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMCSuplly();
        }

        private void btn_EMCOpen_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMCOpen();
        }

        private void btn_EMCClose_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMCClose();
        }

        private void btn_Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void tBox_PCBCount_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).PCBCount();
        }

        private void ComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).WorkModeOpen();
        }

        private void CB_WS_DropDownClosed(object sender, EventArgs e)
        {
            (DataContext as MainWindow_ViewModel).WorkModeClose();
        }

        private void btn_PCBWorkCountReset_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).PCBResetPress();
        }

        private void btn_PCBWorkCountReset_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).PCBResetRelease();
        }

        private void btn_PCBEMCSkip_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).LoadingsSkipPress();
        }

        private void btn_PCBEMCSkip_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).LoadingSkipRelease();
        }

        private void tBox__PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobSpeedSet_Persent();
        }

        private void tBox_1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobSpeedSet_MMS();
        }

        private void tBox_ExtractCount_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).ExtractCount();
        }

        private void btn_ExtractCountReset_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).ExtractResetPress();
        }

        private void btn_ExtractCountReset_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).ExtractResetPress();
        }

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

        private void btn_SpraySkip_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).SpraySkipPress();
        }

        private void tBox_PortSpeed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobPortSpeedSet();
        }

        private void tBox_PortCount_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).RobPortCountSet();
        }

        private void btn_SetView_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).SetViewButtonPress();
        }

        private void btn_ImageColloect_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).ImageColloection();
        }

        private void btn_Monitor_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel)?.ScreenOff();
        }

        private void btn_EMCStopper_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMCStopperFWD();
        }

        private void btn_EMCRelease_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).EMCStopperBWD();
        }

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

        private void tBox_2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).PressCloseTimeSet();
        }

        private void tBox_3_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).ChamberCloseTimeSet();
        }

        private void btn_MGZCNG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).MagazineChangePress();
        }

        private void btn_MGZCNG_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).MagazineChangeRelease();
        }

        private void btn_DeviceSet_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindow_ViewModel).DeviceNameSet();
        }
    }
}