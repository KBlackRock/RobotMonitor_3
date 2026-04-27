using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using RobotMonitor_3.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobotMonitor_3.ViewModels
{
    internal partial class DeviceWindow_ViewModel : ObservableObject
    {
        private XmlParser m_XmlParser;
        public Window DeviceWindow;

        [ObservableProperty] private string deviceName1; 
        [ObservableProperty] private string deviceName2; 
        [ObservableProperty] private string deviceName3; 
        [ObservableProperty] private string deviceName4; 
        [ObservableProperty] private string deviceName5; 
        [ObservableProperty] private string deviceName6; 
        [ObservableProperty] private string deviceName7; 
        [ObservableProperty] private string deviceName8; 
        [ObservableProperty] private string deviceName9; 
        [ObservableProperty] private string deviceName10;
        [ObservableProperty] private string deviceSize1; 
        [ObservableProperty] private string deviceSize2; 
        [ObservableProperty] private string deviceSize3; 
        [ObservableProperty] private string deviceSize4; 
        [ObservableProperty] private string deviceSize5; 
        [ObservableProperty] private string deviceSize6; 
        [ObservableProperty] private string deviceSize7; 
        [ObservableProperty] private string deviceSize8; 
        [ObservableProperty] private string deviceSize9; 
        [ObservableProperty] private string deviceSize10;

        public string DeviceName1_Saved { get { return m_XmlParser.SavedData.SavedDeviceName1; } set { if (m_XmlParser.SavedData.SavedDeviceName1 != value) { m_XmlParser.SavedData.SavedDeviceName1 = value; OnPropertyChanged(); } } }
        public string DeviceName2_Saved { get { return m_XmlParser.SavedData.SavedDeviceName2; } set { if (m_XmlParser.SavedData.SavedDeviceName2 != value) { m_XmlParser.SavedData.SavedDeviceName2 = value; OnPropertyChanged(); } } }
        public string DeviceName3_Saved { get { return m_XmlParser.SavedData.SavedDeviceName3; } set { if (m_XmlParser.SavedData.SavedDeviceName3 != value) { m_XmlParser.SavedData.SavedDeviceName3 = value; OnPropertyChanged(); } } }
        public string DeviceName4_Saved { get { return m_XmlParser.SavedData.SavedDeviceName4; } set { if (m_XmlParser.SavedData.SavedDeviceName4 != value) { m_XmlParser.SavedData.SavedDeviceName4 = value; OnPropertyChanged(); } } }
        public string DeviceName5_Saved { get { return m_XmlParser.SavedData.SavedDeviceName5; } set { if (m_XmlParser.SavedData.SavedDeviceName5 != value) { m_XmlParser.SavedData.SavedDeviceName5 = value; OnPropertyChanged(); } } }
        public string DeviceName6_Saved { get { return m_XmlParser.SavedData.SavedDeviceName6; } set { if (m_XmlParser.SavedData.SavedDeviceName6 != value) { m_XmlParser.SavedData.SavedDeviceName6 = value; OnPropertyChanged(); } } }
        public string DeviceName7_Saved { get { return m_XmlParser.SavedData.SavedDeviceName7; } set { if (m_XmlParser.SavedData.SavedDeviceName7 != value) { m_XmlParser.SavedData.SavedDeviceName7 = value; OnPropertyChanged(); } } }
        public string DeviceName8_Saved { get { return m_XmlParser.SavedData.SavedDeviceName8; } set { if (m_XmlParser.SavedData.SavedDeviceName8 != value) { m_XmlParser.SavedData.SavedDeviceName8 = value; OnPropertyChanged(); } } }
        public string DeviceName9_Saved { get { return m_XmlParser.SavedData.SavedDeviceName9; } set { if (m_XmlParser.SavedData.SavedDeviceName9 != value) { m_XmlParser.SavedData.SavedDeviceName9 = value; OnPropertyChanged(); } } }
        public string DeviceName10_Saved { get { return m_XmlParser.SavedData.SavedDeviceName10; } set { if (m_XmlParser.SavedData.SavedDeviceName10 != value) { m_XmlParser.SavedData.SavedDeviceName10 = value; OnPropertyChanged(); } } }

        public string DeviceSize1_Saved { get { return m_XmlParser.SavedData.SavedDeviceSize1.ToString(); } set { if (m_XmlParser.SavedData.SavedDeviceSize1.ToString() != value) { m_XmlParser.SavedData.SavedDeviceSize1 = int.Parse(value); OnPropertyChanged(); } } }
        public string DeviceSize2_Saved { get { return m_XmlParser.SavedData.SavedDeviceSize2.ToString(); } set { if (m_XmlParser.SavedData.SavedDeviceSize2.ToString() != value) { m_XmlParser.SavedData.SavedDeviceSize2 = int.Parse(value); OnPropertyChanged(); } } }
        public string DeviceSize3_Saved { get { return m_XmlParser.SavedData.SavedDeviceSize3.ToString(); } set { if (m_XmlParser.SavedData.SavedDeviceSize3.ToString() != value) { m_XmlParser.SavedData.SavedDeviceSize3 = int.Parse(value); OnPropertyChanged(); } } }
        public string DeviceSize4_Saved { get { return m_XmlParser.SavedData.SavedDeviceSize4.ToString(); } set { if (m_XmlParser.SavedData.SavedDeviceSize4.ToString() != value) { m_XmlParser.SavedData.SavedDeviceSize4 = int.Parse(value); OnPropertyChanged(); } } }
        public string DeviceSize5_Saved { get { return m_XmlParser.SavedData.SavedDeviceSize5.ToString(); } set { if (m_XmlParser.SavedData.SavedDeviceSize5.ToString() != value) { m_XmlParser.SavedData.SavedDeviceSize5 = int.Parse(value); OnPropertyChanged(); } } }
        public string DeviceSize6_Saved { get { return m_XmlParser.SavedData.SavedDeviceSize6.ToString(); } set { if (m_XmlParser.SavedData.SavedDeviceSize6.ToString() != value) { m_XmlParser.SavedData.SavedDeviceSize6 = int.Parse(value); OnPropertyChanged(); } } }
        public string DeviceSize7_Saved { get { return m_XmlParser.SavedData.SavedDeviceSize7.ToString(); } set { if (m_XmlParser.SavedData.SavedDeviceSize7.ToString() != value) { m_XmlParser.SavedData.SavedDeviceSize7 = int.Parse(value); OnPropertyChanged(); } } }
        public string DeviceSize8_Saved { get { return m_XmlParser.SavedData.SavedDeviceSize8.ToString(); } set { if (m_XmlParser.SavedData.SavedDeviceSize8.ToString() != value) { m_XmlParser.SavedData.SavedDeviceSize8 = int.Parse(value); OnPropertyChanged(); } } }
        public string DeviceSize9_Saved { get { return m_XmlParser.SavedData.SavedDeviceSize9.ToString(); } set { if (m_XmlParser.SavedData.SavedDeviceSize9.ToString() != value) { m_XmlParser.SavedData.SavedDeviceSize9 = int.Parse(value); OnPropertyChanged(); } } }
        public string DeviceSize10_Saved { get { return m_XmlParser.SavedData.SavedDeviceSize10.ToString(); } set { if (m_XmlParser.SavedData.SavedDeviceSize10.ToString() != value) { m_XmlParser.SavedData.SavedDeviceSize10 = int.Parse(value); OnPropertyChanged(); } } }

        public string KeyboardData;

        // Win32 API 선언
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_SYSCOMMAND = 0x0112;
        private const uint SC_CLOSE = 0xF060;

        public DeviceWindow_ViewModel()
        {
            m_XmlParser = new Utilities.XmlParser();
            KeyboardData = "";
        }

        internal void Start(Window window)
        {
            DeviceWindow = window;
            DeviceWindow.ResizeMode = ResizeMode.NoResize;
            DeviceName1 = DeviceName1_Saved;
            DeviceName2 = DeviceName2_Saved;
            DeviceName3 = DeviceName3_Saved;
            DeviceName4 = DeviceName4_Saved;
            DeviceName5 = DeviceName5_Saved;
            DeviceName6 = DeviceName6_Saved;
            DeviceName7 = DeviceName7_Saved;
            DeviceName8 = DeviceName8_Saved;
            DeviceName9 = DeviceName9_Saved;
            DeviceName10 = DeviceName10_Saved;
            DeviceSize1 = DeviceSize1_Saved;
            DeviceSize2 = DeviceSize2_Saved;
            DeviceSize3 = DeviceSize3_Saved;
            DeviceSize4 = DeviceSize4_Saved;
            DeviceSize5 = DeviceSize5_Saved;
            DeviceSize6 = DeviceSize6_Saved;
            DeviceSize7 = DeviceSize7_Saved;
            DeviceSize8 = DeviceSize8_Saved;
            DeviceSize9 = DeviceSize9_Saved;
            DeviceSize10 = DeviceSize10_Saved;
        }

        internal void OKClick()
        {
            DeviceName1_Saved = DeviceName1;
            DeviceName2_Saved = DeviceName2;
            DeviceName3_Saved = DeviceName3;
            DeviceName4_Saved = DeviceName4;
            DeviceName5_Saved = DeviceName5;
            DeviceName6_Saved = DeviceName6;
            DeviceName7_Saved = DeviceName7;
            DeviceName8_Saved = DeviceName8;
            DeviceName9_Saved = DeviceName9;
            DeviceName10_Saved = DeviceName10;
            DeviceSize1_Saved = DeviceSize1;
            DeviceSize2_Saved = DeviceSize2;
            DeviceSize3_Saved = DeviceSize3;
            DeviceSize4_Saved = DeviceSize4;
            DeviceSize5_Saved = DeviceSize5;
            DeviceSize6_Saved = DeviceSize6;
            DeviceSize7_Saved = DeviceSize7;
            DeviceSize8_Saved = DeviceSize8;
            DeviceSize9_Saved = DeviceSize9;
            DeviceSize10_Saved = DeviceSize10;
            m_XmlParser.SavedDataSave();
            DeviceWindow.Close();
        }

        internal void CancelClick()
        {
            DeviceWindow.Close();
        }

        internal void ApplyClick()
        {
            DeviceName1_Saved = DeviceName1;
            DeviceName2_Saved = DeviceName2;
            DeviceName3_Saved = DeviceName3;
            DeviceName4_Saved = DeviceName4;
            DeviceName5_Saved = DeviceName5;
            DeviceName6_Saved = DeviceName6;
            DeviceName7_Saved = DeviceName7;
            DeviceName8_Saved = DeviceName8;
            DeviceName9_Saved = DeviceName9;
            DeviceName10_Saved = DeviceName10;
            DeviceSize1_Saved = DeviceSize1;
            DeviceSize2_Saved = DeviceSize2;
            DeviceSize3_Saved = DeviceSize3;
            DeviceSize4_Saved = DeviceSize4;
            DeviceSize5_Saved = DeviceSize5;
            DeviceSize6_Saved = DeviceSize6;
            DeviceSize7_Saved = DeviceSize7;
            DeviceSize8_Saved = DeviceSize8;
            DeviceSize9_Saved = DeviceSize9;
            DeviceSize10_Saved = DeviceSize10;
            m_XmlParser.SavedDataSave();
        }

        internal void OpenTouchKeyboard()  // 플로팅 키보드 호출 ( TabTip이 가능하면 우선 시도, 불가하면 OSK 호출 )
        {
            try
            {
                var existingProcesses = Process.GetProcessesByName("TabTip");
                foreach (var p in existingProcesses)  // 기존에 실행중인 TabTip이 있으면 종료
                {
                    p.Kill();
                    p.WaitForExit(100);
                }

                string progFiles = @"C:\Program Files\Common Files\Microsoft Shared\ink";  // TabTip 프로그램 위치
                string keyboardPath = Path.Combine(progFiles, "TabTip.exe"); 

                if (File.Exists(keyboardPath))  // 경로에 TabTip.exe 가 있으면 콜
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = keyboardPath;
                    psi.UseShellExecute = true;
                    Process.Start(psi);
                }
                else  // 아니면 osk 콜
                {
                    ProcessStartInfo psi = new ProcessStartInfo("osk.exe");
                    psi.UseShellExecute = true;
                    Process.Start(psi);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("키보드를 열 수 없습니다: " + ex.Message);
            }
        }

        internal void KillTouchKeyboard() 
        {
            try
            {
                // TabTip (터치 키보드) 종료 처리
                // TabTip은 프로세스 킬이 아니라 창을 찾아서 닫기 메시지를 보내야 함
                var touchKeyboardWnd = FindWindow("IPTip_Main_Window", null);
                if (touchKeyboardWnd != IntPtr.Zero)
                {
                    PostMessage(touchKeyboardWnd, WM_SYSCOMMAND, (IntPtr)SC_CLOSE, IntPtr.Zero);
                }

                // OSK (화상 키보드) 종료 처리
                var processes2 = Process.GetProcessesByName("osk");
                foreach (var p in processes2)
                {
                    p.Kill();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        internal void SizeBtn1_Click() => DeviceSize1 = CallNumKey(DeviceSize1);
        internal void SizeBtn2_Click() => DeviceSize2 = CallNumKey(DeviceSize2);
        internal void SizeBtn3_Click() => DeviceSize3 = CallNumKey(DeviceSize3);
        internal void SizeBtn4_Click() => DeviceSize4 = CallNumKey(DeviceSize4);
        internal void SizeBtn5_Click() => DeviceSize5 = CallNumKey(DeviceSize5);
        internal void SizeBtn6_Click() => DeviceSize6 = CallNumKey(DeviceSize6);
        internal void SizeBtn7_Click() => DeviceSize7 = CallNumKey(DeviceSize7);
        internal void SizeBtn8_Click() => DeviceSize8 = CallNumKey(DeviceSize8);
        internal void SizeBtn9_Click() => DeviceSize9 = CallNumKey(DeviceSize9);
        internal void SizeBtn10_Click() => DeviceSize10 = CallNumKey(DeviceSize10);

        public string CallNumKey(string originalData) // 숫자키보드 출력
        {
            var KeyWindow = new NumberKeyboard();
            KeyWindow.Left = 300;
            KeyWindow.Top = 100;
            KeyWindow.ResizeMode = ResizeMode.NoResize;
            KeyWindow.Topmost = true;
            WeakReferenceMessenger.Default.Send(new DisplayDataSender(originalData)); // 기존 데이터 전송
            WeakReferenceMessenger.Default.Unregister<KeyboardDataSender>(this);  // 키보드 데이터 수신 대기
            WeakReferenceMessenger.Default.Register<KeyboardDataSender>(this, (r, m) => { KeyboardData = m.Value; }); // 키보드 데이터 수신

            KeyWindow.ShowDialog();

            return KeyboardData != "" ? KeyboardData : originalData; //키보드 데이터가 없으면("") 기존값, 있으면 키보드 데이터 반환
        }
    }
}
