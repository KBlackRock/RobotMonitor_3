using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.IO.Ports;
using CommunityToolkit.Mvvm.Messaging;
using RobotMonitor_3.Models;
using System.Collections.Concurrent;

namespace RobotMonitor_3.ViewModels
{
    internal class MainWindow_ViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        public void RaisePropertyChangedEvent(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Member Variable
        private TcpListener m_Server;
        private TcpClient m_Client;
        private NetworkStream m_Stream;
        private Models.XmlParser m_XmlParser;
        public Window MainWindow;

        private Thread ServerListening;
        private Thread ClientReading;

        private ConcurrentQueue<string> _sendQueue;
        private DispatcherTimer _sendTimer;
        private const int MAX_PACKET_SIZE = 75;

        private Object thisLock = new Object();
        string inStream = "";


        DispatcherTimer timer;
        DispatcherTimer responseTimer;
        DispatcherTimer FlickerTimer;
        DispatcherTimer DeviceTimer;
        DispatcherTimer eggTimer;

        private string path = (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RobotMonitor_3");

        private bool m_IsServerOpened;
        private bool m_ServerConnection;
        private string m_ServerOutput;
        private string m_ServerSendMsg;
        private List<string> ServerMsgList;
        private int ServerMsgListNum;
        private bool m_IsClientOpened;
        private bool m_ClientConnection;
        private string m_ClientOutput;
        private string m_ClientSendMsg;
        private List<string> ClientMsgList;
        private int ClientMsgListNum;
        private bool buttonTiger;
        public string TimerWorkSelect;
        public string ParsingData;
        public int responseTimeStack;
        public int colorCount;
        public int eggCount;
        public bool OpReverse;
        public bool timerBreak;
        public bool needOrigin;
        public string workModeBuffer;
        public int CureTimeNum;

        public int[] robOverrideLimit;
        public int[] robSpeedMaxLimit;
        public int[] robPCBCountLimit;
        public int[] robExtractCountLimit;

        public bool Btn1Checked;
        public bool Btn2Checked;
        public bool Btn3Checked;
        public bool Btn4Checked;
        public bool Btn5Checked;
        public bool Btn6Checked;

        public bool IsServoBtnPressed;
        #endregion

        #region OnPropertyChange Properties
        private string _WindowPage; public string WindowPage { get { return _WindowPage; } set { _WindowPage = value; OnPropertyChanged(); } }
        private bool tabVisible; public bool TabVisible { get { return tabVisible; } set { { tabVisible = value; OnPropertyChanged(); } } }
        private bool r_IsRunning; public bool R_IsRunning { get { return r_IsRunning; } set { r_IsRunning = value; OnPropertyChanged(); } }
        private bool r_IsStopped; public bool R_IsStopped { get { return r_IsStopped; } set { r_IsStopped = value; OnPropertyChanged(); } }
        private bool r_IsReady; public bool R_IsReady { get { return r_IsReady; } set { r_IsReady = value; OnPropertyChanged(); } }
        private bool r_IsOrigin; public bool R_IsOrigin { get { return r_IsOrigin; } set { r_IsOrigin = value; OnPropertyChanged(); } }
        private bool roWork_Cleaning; public bool RoWork_Cleaning { get { return roWork_Cleaning; } set { roWork_Cleaning = value; OnPropertyChanged(); } }
        private bool roWork_Spray; public bool RoWork_Spray { get { return roWork_Spray; } set { roWork_Spray = value; OnPropertyChanged(); } }
        private bool roWork_PCBLoad; public bool RoWork_PCBLoad { get { return roWork_PCBLoad; } set { roWork_PCBLoad = value; OnPropertyChanged(); } }
        private bool roWork_PCBPlace; public bool RoWork_PCBPlace { get { return roWork_PCBPlace; } set { roWork_PCBPlace = value; OnPropertyChanged(); } }
        private bool roWork_EMC; public bool RoWork_EMC { get { return roWork_EMC; } set { roWork_EMC = value; OnPropertyChanged(); } }
        private bool roWork_Extract; public bool RoWork_Extract { get { return roWork_Extract; } set { roWork_Extract = value; OnPropertyChanged(); } }
        private bool roWork_Button; public bool RoWork_Button { get { return roWork_Button; } set { roWork_Button = value; OnPropertyChanged(); } }
        private bool roWork_ToolChange; public bool RoWork_ToolChange { get { return roWork_ToolChange; } set { roWork_ToolChange = value; OnPropertyChanged(); } }
        private bool roWork_Vision; public bool RoWork_Vision { get { return roWork_Vision; } set { roWork_Vision = value; OnPropertyChanged(); } }
        private string startButtonColor; public string StartButtonColor { get { return startButtonColor; } set { startButtonColor = value; OnPropertyChanged(); } }
        private double resetBtnOpacity; public double ResetBtnOpacity { get { return resetBtnOpacity; } set { resetBtnOpacity = value; OnPropertyChanged(); } }
        private bool resetbtnEnable; public bool ResetbtnEnable { get { return resetbtnEnable; } set { resetbtnEnable = value; OnPropertyChanged(); } }
        private int timerStack; public int TimerStack { get { return timerStack; } set { timerStack = value; OnPropertyChanged(); } }
        private bool isAuto; public bool IsAuto { get { return isAuto; } set { isAuto = value; OnPropertyChanged(); } }
        private bool isManual; public bool IsManual { get { return isManual; } set { isManual = value; OnPropertyChanged(); } }
        private bool isError; public bool IsError { get { return isError; } set { isError = value; OnPropertyChanged(); } }
        private bool isSetView; public bool IsSetView { get { return isSetView; } set { isSetView = value; OnPropertyChanged(); } }
        private bool autoBtn; public bool AutoBtn { get { return autoBtn; } set { autoBtn = value; OnPropertyChanged(); } }
        private bool manualBtn; public bool ManualBtn { get { return manualBtn; } set { manualBtn = value; OnPropertyChanged(); } }
        private bool isSetting; public bool IsSetting { get { return isSetting; } set { isSetting = value; OnPropertyChanged(); } }
        private bool homeButton; public bool HomeButton { get { return homeButton; } set { homeButton = value; OnPropertyChanged(); } }
        private string selectPage; public string SelectPage { get { return selectPage; } set { selectPage = value; OnPropertyChanged(); } }
        private string robotMessage; public string RobotMessage { get { return robotMessage; } set { { robotMessage = value; OnPropertyChanged(); } } }
        private bool btnWorkWait; public bool BtnWorkWait { get { return btnWorkWait; } set { btnWorkWait = value; OnPropertyChanged(); } }
        private string robotImage; public string RobotImage { get { return robotImage; } set { robotImage = value; OnPropertyChanged(); } }
        private string errorMessage; public string ErrorMessage { get { return errorMessage; } set { { errorMessage = value; OnPropertyChanged(); } } }
        private string errorImage; public string ErrorImage { get { return errorImage; } set { errorImage = value; OnPropertyChanged(); } }
        private string isRobSpeed_Persent; public string IsRobSpeed_Persent { get { return isRobSpeed_Persent; } set { isRobSpeed_Persent = value; OnPropertyChanged(); } }
        private string isRobSpeed_MMS; public string IsRobSpeed_MMS { get { return isRobSpeed_MMS; } set { isRobSpeed_MMS = value; OnPropertyChanged(); } }
        private string isPressCloseTime; public string IsPressCloseTime { get { return isPressCloseTime; } set { isPressCloseTime = value; OnPropertyChanged(); } }
        private string isChamberCloseTime; public string IsChamberCloseTime { get { return isChamberCloseTime; } set { isChamberCloseTime = value; OnPropertyChanged(); } }
        private string isPCBCount; public string IsPCBCount { get { return isPCBCount; } set { isPCBCount = value; OnPropertyChanged(); } }
        private string egg; public string EGG { get { return egg; } set { egg = value; OnPropertyChanged(); } }
        private string serverConnectBtnColor; public string ServerConnectBtnColor { get { return serverConnectBtnColor; } set { serverConnectBtnColor = value; OnPropertyChanged(); } }
        private string out1color; public string Out1Color { get { return out1color; } set { out1color = value; OnPropertyChanged(); } }
        private string out2color; public string Out2Color { get { return out2color; } set { out2color = value; OnPropertyChanged(); } }
        private string out3color; public string Out3Color { get { return out3color; } set { out3color = value; OnPropertyChanged(); } }
        private string out4color; public string Out4Color { get { return out4color; } set { out4color = value; OnPropertyChanged(); } }
        private string out5color; public string Out5Color { get { return out5color; } set { out5color = value; OnPropertyChanged(); } }
        private string out6color; public string Out6Color { get { return out6color; } set { out6color = value; OnPropertyChanged(); } }
        private string in1color; public string In1Color { get { return in1color; } set { in1color = value; OnPropertyChanged(); } }
        private string in2color; public string In2Color { get { return in2color; } set { in2color = value; OnPropertyChanged(); } }
        private string in3color; public string In3Color { get { return in3color; } set { in3color = value; OnPropertyChanged(); } }
        private string in4color; public string In4Color { get { return in4color; } set { in4color = value; OnPropertyChanged(); } }
        private string in5color; public string In5Color { get { return in5color; } set { in5color = value; OnPropertyChanged(); } }
        private string in6color; public string In6Color { get { return in6color; } set { in6color = value; OnPropertyChanged(); } }
        private string sv1pw_BerderColor; public string SV1PW_BorderColor { get { return sv1pw_BerderColor; } set { sv1pw_BerderColor = value; OnPropertyChanged(); } }
        private string sv2pw_BerderColor; public string SV2PW_BorderColor { get { return sv2pw_BerderColor; } set { sv2pw_BerderColor = value; OnPropertyChanged(); } }
        private string sv1pw_contect; public string SV1PW_Content { get { return sv1pw_contect; } set { sv1pw_contect = value; OnPropertyChanged(); } }
        private string sv2pw_contect; public string SV2PW_Content { get { return sv2pw_contect; } set { sv2pw_contect = value; OnPropertyChanged(); } }
        private string robAutoColor; public string RobAutoColor { get { return robAutoColor; } set { robAutoColor = value; OnPropertyChanged(); } }
        private string robMotorColor; public string RobMotorColor { get { return robMotorColor; } set { robMotorColor = value; OnPropertyChanged(); } }
        private string robTaskRunColor; public string RobTaskRunColor { get { return robTaskRunColor; } set { robTaskRunColor = value; OnPropertyChanged(); } }
        private string robBatteryColor; public string RobBatteryColor { get { return robBatteryColor; } set { robBatteryColor = value; OnPropertyChanged(); } }
        private string workMode; public string WorkMode { get { return workMode; } set { workMode = value; OnPropertyChanged(); } }
        private string isSelectedWorkMode; public string IsSelectedWorkMode { get { return isSelectedWorkMode; } set { isSelectedWorkMode = value; OnPropertyChanged(); } }
        private ObservableCollection<string> workModeList; public ObservableCollection<string> WorkModeList { get { return workModeList; } set { workModeList = value; OnPropertyChanged(); } }
        private bool isWorkModeSelected; public bool IsWorkModeSelected { get { return isWorkModeSelected; } set { isWorkModeSelected = value; OnPropertyChanged(); } }
        private bool isWorkModeOpen; public bool IsWorkModeOpen { get { return isWorkModeOpen; } set { isWorkModeOpen = value; OnPropertyChanged(); } }
        private string isWorkModeTextColor; public string IsWorkModeTextColor { get { return isWorkModeTextColor; } set { isWorkModeTextColor = value; OnPropertyChanged(); } }
        private string loadSkipColor; public string LoadSkipColor { get { return loadSkipColor; } set { loadSkipColor = value; OnPropertyChanged(); } }
        private string imgCollectColor; public string ImgCollectColor { get { return imgCollectColor; } set { imgCollectColor = value; OnPropertyChanged(); } }
        private string tCounting; public string TCounting { get { return tCounting; } set { tCounting = value; OnPropertyChanged(); } }
        private bool pressIOEnable; public bool PressIOEnable { get { return pressIOEnable; } set { pressIOEnable = value; OnPropertyChanged(); } }
        private string isExtractCount; public string IsExtactCount { get { return isExtractCount; } set { isExtractCount = value; OnPropertyChanged(); } }
        private string isRobBlowSpeed; public string IsRobBlowSpeed { get { return isRobBlowSpeed; } set { isRobBlowSpeed = value; OnPropertyChanged(); } }
        private string isRobBlow1stTopCount; public string IsRobBlow1stTopCount { get { return isRobBlow1stTopCount; } set { isRobBlow1stTopCount = value; OnPropertyChanged(); } }
        private string isRobBlow1stBotCount; public string IsRobBlow1stBotCount { get { return isRobBlow1stBotCount; } set { isRobBlow1stBotCount = value; OnPropertyChanged(); } }
        private string isRobBlow2ndTopCount; public string IsRobBlow2ndTopCount { get { return isRobBlow2ndTopCount; } set { isRobBlow2ndTopCount = value; OnPropertyChanged(); } }
        private string isRobBlow2ndBotCount; public string IsRobBlow2ndBotCount { get { return isRobBlow2ndBotCount; } set { isRobBlow2ndBotCount = value; OnPropertyChanged(); } }
        private string isRobBlowPortCount; public string IsRobBlowPortCount { get { return isRobBlowPortCount; } set { isRobBlowPortCount = value; OnPropertyChanged(); } }
        private string isRobSpraySpeed; public string IsRobSpraySpeed { get { return isRobSpraySpeed; } set { isRobSpraySpeed = value; OnPropertyChanged(); } }
        private string isRobSprayTopCount; public string IsRobSprayTopCount { get { return isRobSprayTopCount; } set { isRobSprayTopCount = value; OnPropertyChanged(); } }
        private string isRobSprayPortTime; public string IsRobSprayPortTime { get { return isRobSprayPortTime; } set { isRobSprayPortTime = value; OnPropertyChanged(); } }
        private string isRobPortSpeed; public string IsRobPortSpeed { get { return isRobPortSpeed; } set { isRobPortSpeed = value; OnPropertyChanged(); } }
        private string isRobPortCount; public string IsRobPortCount { get { return isRobPortCount; } set { isRobPortCount = value; OnPropertyChanged(); } }
        private int isWorkIndex; public int IsWorkIndex { get { return isWorkIndex; } set { isWorkIndex = value; OnPropertyChanged(); } }
        private string display_StackedDeviceCount; public string Display_StackedDeviceCount { get { return display_StackedDeviceCount; } set { display_StackedDeviceCount = value; OnPropertyChanged(); } }
        private string display_UnStackedDeviceCount; public string Display_UnStackedDeviceCount { get { return display_UnStackedDeviceCount; } set { display_UnStackedDeviceCount = value; OnPropertyChanged(); } }
        private string display_StackedShotCount; public string Display_StackedShotCount { get { return display_StackedShotCount; } set { display_StackedShotCount = value; OnPropertyChanged(); } }
        private string display_UnStackedShotCount; public string Display_UnStackedShotCount { get { return display_UnStackedShotCount; } set { display_UnStackedShotCount = value; OnPropertyChanged(); } }
        private bool robotSetting; public bool RobotSetting { get { return robotSetting; } set { robotSetting = value; OnPropertyChanged(); } }
        private string m1_CurrentPosition; public string M1_CurrentPosition { get { return m1_CurrentPosition; } set { m1_CurrentPosition = value; OnPropertyChanged(); } }
        private string m1_Speed; public string M1_Speed { get { return m1_Speed; } set { m1_Speed = value; OnPropertyChanged(); } }
        private string m1_FWDLIM; public string M1_FWDLIM { get { return m1_FWDLIM; } set { m1_FWDLIM = value; OnPropertyChanged(); } }
        private string m2_CurrentPosition; public string M2_CurrentPosition { get { return m2_CurrentPosition; } set { m2_CurrentPosition = value; OnPropertyChanged(); } }
        private string m2_Speed; public string M2_Speed { get { return m2_Speed; } set { m2_Speed = value; OnPropertyChanged(); } }
        private string emc_CylOpenDelay; public string EMC_CylOpenDelay { get { return emc_CylOpenDelay; } set { emc_CylOpenDelay = value; OnPropertyChanged(); } }
        private string emc_CylCloseDelay; public string EMC_CylCloseDelay { get { return emc_CylCloseDelay; } set { emc_CylCloseDelay = value; OnPropertyChanged(); } }
        private string emc_StopperFWDDelay; public string EMC_StopperFWDDelay { get { return emc_StopperFWDDelay; } set { emc_StopperFWDDelay = value; OnPropertyChanged(); } }
        private string emc_StopperBWDDelay; public string EMC_StopperBWDDelay { get { return emc_StopperBWDDelay; } set { emc_StopperBWDDelay = value; OnPropertyChanged(); } }
        #endregion

        #region Server Properties
        public bool SentReceivedSymbol { get { return m_XmlParser.SavedData.SentReceivedSymbol; } set { if (m_XmlParser.SavedData.SentReceivedSymbol != value) { m_XmlParser.SavedData.SentReceivedSymbol = value; RaisePropertyChangedEvent("SentReceivedSymbol"); } } }
        public bool ShowTimeStamp { get { return m_XmlParser.SavedData.ShowTimeStamp; } set { if (m_XmlParser.SavedData.ShowTimeStamp != value) { m_XmlParser.SavedData.ShowTimeStamp = value; RaisePropertyChangedEvent("ShowTimeStamp"); } } }
        public bool IsReadHex { get { return m_XmlParser.SavedData.IsReadHex; } set { if (m_XmlParser.SavedData.IsReadHex != value) { m_XmlParser.SavedData.IsReadHex = value; RaisePropertyChangedEvent("IsReadHex"); } } }
        public bool IsWriteHex { get { return m_XmlParser.SavedData.IsWriteHex; } set { if (m_XmlParser.SavedData.IsWriteHex != value) { m_XmlParser.SavedData.IsWriteHex = value; RaisePropertyChangedEvent("IsWriteHex"); } } }
        public string Prefix { get { return m_XmlParser.SavedData.Prefix; } set { if (m_XmlParser.SavedData.Prefix != value) { m_XmlParser.SavedData.Prefix = value; RaisePropertyChangedEvent("Prefix"); } } }
        public string Suffix { get { return m_XmlParser.SavedData.Suffix; } set { if (m_XmlParser.SavedData.Suffix != value) { m_XmlParser.SavedData.Suffix = value; RaisePropertyChangedEvent("Suffix"); } } }

        public string[] ServerItems { get; set; }
        public int ClientPort { get { return m_XmlParser.SavedData.ClientPort; } set { if (m_XmlParser.SavedData.ClientPort != value) { m_XmlParser.SavedData.ClientPort = value; RaisePropertyChangedEvent("ClientPort"); } } }
        public string ClientIP { get { return m_XmlParser.SavedData.ClientIP; } set { if (m_XmlParser.SavedData.ClientIP != value) { m_XmlParser.SavedData.ClientIP = value; RaisePropertyChangedEvent("ClientIP"); } } }
        public bool IsServerOpened { get { return m_IsServerOpened; } set { if (m_IsServerOpened != value) { m_IsServerOpened = value; RaisePropertyChangedEvent("IsServerOpened"); } } }
        public bool ServerConnection { get { return m_ServerConnection; } set { if (m_ServerConnection != value) { m_ServerConnection = value; RaisePropertyChangedEvent("ServerConnection"); } } }
        public string ServerOutput { get { return m_ServerOutput; } set { if (m_ServerOutput != value) { m_ServerOutput = value; RaisePropertyChangedEvent("ServerOutput"); } } }
        //public string ServerSendMsg { get { return m_ServerSendMsg; } set { if (m_ServerSendMsg != value) { m_ServerSendMsg = value; RaisePropertyChangedEvent("ServerSendMsg"); } } }
        #endregion

        #region Setting Data Properties
        private int dev1StackedShotCount; public int Dev1StackedShotCount { get { return m_XmlParser.SavedData.dev1StackedShotCount; } set { if (m_XmlParser.SavedData.dev1StackedShotCount != value) { m_XmlParser.SavedData.dev1StackedShotCount = value; RaisePropertyChangedEvent("Dev1StackedShotCount"); } } }
        private int dev1StackedDeviceCount; public int Dev1StackedDeviceCount { get { return m_XmlParser.SavedData.dev1StackedDeviceCount; } set { if (m_XmlParser.SavedData.dev1StackedDeviceCount != value) { m_XmlParser.SavedData.dev1StackedDeviceCount = value; RaisePropertyChangedEvent("Dev1StackedDeviceCount"); } } }
        private int dev1UnstackedShotCount; public int Dev1UnStackedShotCount { get { return m_XmlParser.SavedData.dev1UnstackedShotCount; } set { if (m_XmlParser.SavedData.dev1UnstackedShotCount != value) { m_XmlParser.SavedData.dev1UnstackedShotCount = value; RaisePropertyChangedEvent("Dev1UnStackedShotCount"); } } }
        private int dev1UnstackedDeviceCount; public int Dev1UnStackedDeviceCount { get { return m_XmlParser.SavedData.dev1UnstackedDeviceCount; } set { if (m_XmlParser.SavedData.dev1UnstackedDeviceCount != value) { m_XmlParser.SavedData.dev1UnstackedDeviceCount = value; RaisePropertyChangedEvent("Dev1UnStackedDeviceCount"); } } }
        private int dev2StackedShotCount; public int Dev2StackedShotCount { get { return m_XmlParser.SavedData.dev2StackedShotCount; } set { if (m_XmlParser.SavedData.dev2StackedShotCount != value) { m_XmlParser.SavedData.dev2StackedShotCount = value; RaisePropertyChangedEvent("Dev2StackedShotCount"); } } }
        private int dev2StackedDeviceCount; public int Dev2StackedDeviceCount { get { return m_XmlParser.SavedData.dev2StackedDeviceCount; } set { if (m_XmlParser.SavedData.dev2StackedDeviceCount != value) { m_XmlParser.SavedData.dev2StackedDeviceCount = value; RaisePropertyChangedEvent("Dev2StackedDeviceCount"); } } }
        private int dev2UnstackedShotCount; public int Dev2UnStackedShotCount { get { return m_XmlParser.SavedData.dev2UnstackedShotCount; } set { if (m_XmlParser.SavedData.dev2UnstackedShotCount != value) { m_XmlParser.SavedData.dev2UnstackedShotCount = value; RaisePropertyChangedEvent("Dev2UnStackedShotCount"); } } }
        private int dev2UnstackedDeviceCount; public int Dev2UnStackedDeviceCount { get { return m_XmlParser.SavedData.dev2UnstackedDeviceCount; } set { if (m_XmlParser.SavedData.dev2UnstackedDeviceCount != value) { m_XmlParser.SavedData.dev2UnstackedDeviceCount = value; RaisePropertyChangedEvent("Dev2UnStackedDeviceCount"); } } }
        private int dev3StackedShotCount; public int Dev3StackedShotCount { get { return m_XmlParser.SavedData.dev3StackedShotCount; } set { if (m_XmlParser.SavedData.dev3StackedShotCount != value) { m_XmlParser.SavedData.dev3StackedShotCount = value; RaisePropertyChangedEvent("Dev3StackedShotCount"); } } }
        private int dev3StackedDeviceCount; public int Dev3StackedDeviceCount { get { return m_XmlParser.SavedData.dev3StackedDeviceCount; } set { if (m_XmlParser.SavedData.dev3StackedDeviceCount != value) { m_XmlParser.SavedData.dev3StackedDeviceCount = value; RaisePropertyChangedEvent("Dev3StackedDeviceCount"); } } }
        private int dev3UnstackedShotCount; public int Dev3UnStackedShotCount { get { return m_XmlParser.SavedData.dev3UnstackedShotCount; } set { if (m_XmlParser.SavedData.dev3UnstackedShotCount != value) { m_XmlParser.SavedData.dev3UnstackedShotCount = value; RaisePropertyChangedEvent("Dev3UnStackedShotCount"); } } }
        private int dev3UnstackedDeviceCount; public int Dev3UnStackedDeviceCount { get { return m_XmlParser.SavedData.dev3UnstackedDeviceCount; } set { if (m_XmlParser.SavedData.dev3UnstackedDeviceCount != value) { m_XmlParser.SavedData.dev3UnstackedDeviceCount = value; RaisePropertyChangedEvent("Dev3UnStackedDeviceCount"); } } }
        private int startBtnDelay; public int StartBtnDelay { get { return m_XmlParser.SavedData.startBtnDelay; } set { if (m_XmlParser.SavedData.startBtnDelay != value) { m_XmlParser.SavedData.startBtnDelay = value; RaisePropertyChangedEvent("StartBtnDelay"); } } }
        private int homeBtnDelay; public int HomeBtnDelay { get { return m_XmlParser.SavedData.homeBtnDelay; } set { if (m_XmlParser.SavedData.homeBtnDelay != value) { m_XmlParser.SavedData.homeBtnDelay = value; RaisePropertyChangedEvent("HomeBtnDelay"); } } }
        private int countresetBtnDelay; public int CountresetBtnDelay { get { return m_XmlParser.SavedData.countresetBtnDelay; } set { if (m_XmlParser.SavedData.countresetBtnDelay != value) { m_XmlParser.SavedData.countresetBtnDelay = value; RaisePropertyChangedEvent("CountresetBtnDelay"); } } }
        #endregion

        #region Messenger Properties

        #endregion

        #region Messenger Variables
        public string KeyboardData = "";
        public string DisplayedData = "";
        public bool LoginCheck = false;
        public string ErrorData = "";
        public string LogMessageBuffer = "";
        #endregion

        public MainWindow_ViewModel()
        {
            #region Initialization
            m_XmlParser = new Models.XmlParser();

            _sendQueue = new ConcurrentQueue<string>();

            IsServerOpened = false;
            ServerConnection = false;
            TabVisible = false;

            robOverrideLimit = new int[2];
            robSpeedMaxLimit = new int[2];
            robPCBCountLimit = new int[2];
            robExtractCountLimit = new int[2];
            robOverrideLimit[0] = 0; robOverrideLimit[1] = 100;
            robSpeedMaxLimit[0] = 0; robSpeedMaxLimit[1] = 7500;
            robPCBCountLimit[0] = 1; robPCBCountLimit[1] = 20;
            robExtractCountLimit[0] = 1; robExtractCountLimit[1] = 10;


            R_IsRunning = false;
            R_IsStopped = false;
            R_IsReady = false;
            AutoBtn = true;
            IsAuto = true;
            IsManual = false;
            IsSetting = false;
            IsError = false;
            IsSetView = false;
            HomeButton = false;
            BtnWorkWait = true;
            ResetbtnEnable = false;
            IsWorkModeSelected = false;
            LoginCheck = false;
            needOrigin = false;
            IsRobSpeed_Persent = "";
            RobotImage = "";
            RobotMessage = "";
            ErrorData = "";

            Display_StackedShotCount = "0";
            Display_UnStackedShotCount = "0";
            Display_StackedDeviceCount = "0";
            Display_UnStackedDeviceCount = "0";

            ServerMsgList = new List<string>();
            ServerMsgListNum = 0;
            ClientMsgList = new List<string>();
            ClientMsgListNum = 0;

            WorkMode = "";
            WorkModeList = new ObservableCollection<string>();

            responseTimeStack = 0;

            StartButtonColor = "Gary";
            IsWorkModeTextColor = "Black";
            ResetBtnOpacity = 1;

            RobAutoColor = "Gray";
            RobMotorColor = "Gray";
            RobTaskRunColor = "Gray";
            RobBatteryColor = "Gray";

            Btn1Checked = false;
            Btn2Checked = false;
            Btn3Checked = false;
            Btn4Checked = false;
            Btn5Checked = false;
            Btn6Checked = false;

            IsServoBtnPressed = false;
            #endregion





            #region Timer Setting
            timer = new DispatcherTimer(DispatcherPriority.Send, System.Windows.Application.Current.Dispatcher);
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(TimerCount);

            FlickerTimer = new DispatcherTimer(DispatcherPriority.Send, System.Windows.Application.Current.Dispatcher);
            FlickerTimer.Interval = TimeSpan.FromMilliseconds(5);
            FlickerTimer.Tick += new EventHandler(ResetButtonFlicker);

            DeviceTimer = new DispatcherTimer(DispatcherPriority.Send, System.Windows.Application.Current.Dispatcher);
            DeviceTimer.Interval = TimeSpan.FromMilliseconds(383);
            DeviceTimer.Tick += new EventHandler(DeviceSend);

            eggTimer = new DispatcherTimer(DispatcherPriority.Send, System.Windows.Application.Current.Dispatcher);
            eggTimer.Interval = TimeSpan.FromMilliseconds(100);
            eggTimer.Tick += new EventHandler(EggEnd);

            responseTimer = new DispatcherTimer(DispatcherPriority.Send, System.Windows.Application.Current.Dispatcher);
            responseTimer.Interval = TimeSpan.FromMilliseconds(200);
            responseTimer.Tick += new EventHandler(ConnectionOK);

            _sendTimer = new DispatcherTimer(DispatcherPriority.Send, System.Windows.Application.Current.Dispatcher);
            _sendTimer.Interval = TimeSpan.FromMilliseconds(70);
            _sendTimer.Tick += new EventHandler(ProcessSendQueue);
            _sendTimer.Start();
            #endregion

        }




        internal void Opening(Window window)
        {
            MainWindow = window;
            MainWindow.ResizeMode = ResizeMode.NoResize;
            MainWindow.WindowState = WindowState.Maximized; // Window Maximize -------------------------------------
            MainWindow.WindowStyle = WindowStyle.None;
            WindowPage = "Pages/Main_Page.xaml";
            ServerOpenClose();
            WorkModeList.Add("정지모드"); // 0
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName1);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName2);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName3);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName4);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName5);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName6);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName7);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName8);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName9);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName10);
            IsWorkIndex = -1;
        }



        internal void Closing()
        {
            MessageBoxResult mBoxRst = MessageBox.Show("프로그램을 종료 하시겠습니까?", "Program Message", MessageBoxButton.YesNo);
            if (mBoxRst == MessageBoxResult.Yes)
            {
                timer.Stop();
                eggTimer.Stop();
                FlickerTimer.Stop();
                DeviceTimer.Stop();
                responseTimer.Stop();
                if (_sendTimer != null) _sendTimer.Stop();
                ServerClose();
                m_XmlParser.SavedDataSave();

                if (ServerListening != null && ServerListening.IsAlive) ServerListening.Abort();
                if (ClientReading != null && ClientReading.IsAlive) ClientReading.Abort();

                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }





        #region Function Methods
        public string CallNumKey(string originalData) // 숫자키보드 출력
        {
            if (IsSetView) return originalData;
            var KeyWindow = new NumberKeyboard();
            var MainWithSize = MainWindow.Width;
            var MainHeigtSize = MainWindow.Height;
            var KeyWithSize = KeyWindow.Width;
            var keyHeightSize = KeyWindow.Height;

            // 키 윈도우 출력 위치 설정
            Point mPoint = Mouse.GetPosition(MainWindow);
            // 위치가 메인윈도우 크기보다 작을때는 마우스 포인터 기준으로, 클때는 메인윈도우 크기 기준으로 설정
            if (mPoint.X < MainWithSize - KeyWithSize)
                KeyWindow.Left = mPoint.X - (KeyWindow.ActualWidth);
            else KeyWindow.Left = MainWithSize - (KeyWithSize * 1.2);

            if (mPoint.Y < MainHeigtSize - keyHeightSize)
                KeyWindow.Top = mPoint.Y - (KeyWindow.ActualHeight);
            else KeyWindow.Top = MainHeigtSize - (keyHeightSize * 1.1);
            KeyWindow.ResizeMode = ResizeMode.NoResize;
            KeyWindow.Topmost = true;

            WeakReferenceMessenger.Default.Send(new DisplayDataSender(originalData)); // 기존 데이터 전송
            WeakReferenceMessenger.Default.Unregister<KeyboardDataSender>(this);  // 키보드 데이터 수신 대기
            WeakReferenceMessenger.Default.Register<KeyboardDataSender>(this, (r, m) => { KeyboardData = m.Value; }); // 키보드 데이터 수신

            KeyWindow.ShowDialog();

            return KeyboardData != "" ? KeyboardData : originalData; //키보드 데이터가 없으면("") 기존값, 있으면 키보드 데이터 반환
        }



        public void CallLogin() // 로그인 창 출력
        {
            var LoginWindow = new LoginWindow();
            var MainWithSize = MainWindow.Width;
            var MainHeigtSize = MainWindow.Height;

            LoginWindow.ResizeMode = ResizeMode.NoResize;
            LoginWindow.Topmost = true;
            LoginWindow.Left = MainWithSize / 3;
            LoginWindow.Top = MainHeigtSize / 3;

            WeakReferenceMessenger.Default.Unregister<LoginBool>(this);
            WeakReferenceMessenger.Default.Register<LoginBool>(this, (r, m) => { LoginCheck = m.Value; });
            LoginWindow.ShowDialog();
        }



        internal void ButtonVisible(string select)
        {
            HomeButton = false;
            IsSetting = false;
            switch (select)
            {
                case "Auto":
                    WindowPage = "Pages/Main_Page.xaml";
                    IsAuto = true;
                    IsManual = false;
                    IsSetView = false;
                    HomeButton = false;
                    AutoBtn = true;
                    ManualBtn = false;
                    break;
                case "Manual":
                    WindowPage = "Pages/Manual_Page.xaml";
                    IsManual = true;
                    IsAuto = false;
                    IsSetView = false;
                    HomeButton = true;
                    AutoBtn = false;
                    ManualBtn = true;
                    break;
                case "Setting":
                    WindowPage = "Pages/Setting_Page.xaml";
                    IsSetting = true;
                    IsAuto = false;
                    IsManual = false;
                    IsSetView = false;
                    break;
                case "Error":
                    WindowPage = "Pages/Error_Page.xaml";
                    IsError = true;
                    IsSetting = false;
                    IsAuto = false;
                    IsManual = false;
                    IsSetView = false;
                    AutoBtn = true;
                    ManualBtn = false;
                    ResetbtnEnable = true;
                    break;
                case "SetView":
                    WindowPage = "Pages/Setting_Page.xaml";
                    IsSetting = true;
                    IsAuto = false;
                    IsManual = false;
                    IsSetView = true;
                    break;
                default:
                    break;
            }
        }



        /// <summary>
        /// eImg : 에러 이미지 파일명 ( 확장자 jpg 사용 _ 확장자 별도 입력 필요 없음 )
        /// </summary>
        public void Error(string eImg, string eMsg)
        {
            if (ErrorData == eMsg) return;
            R_IsOrigin = false; R_IsReady = false; R_IsRunning = false; R_IsStopped = true; // 스탑 레이블 셋팅
            ErrorImage = path + @"\\Images\\Error\\" + eImg + ".jpg"; // 에러 이미지 불러오기 ( jpg 타입만 )
            ErrorMessage = ErrorData = eMsg;
            WriteLog("!! Error !! : " + eMsg);
            ButtonVisible("Error");  // 에러 창 On
            ServerSend("Stop"); // 로봇 정지
            Thread.Sleep(200);
        }



        public void SystemError(string eMsg)
        {
            if (ErrorData == eMsg) return;
            R_IsOrigin = R_IsReady = R_IsRunning = R_IsStopped = false; // 레이블 초기화
            ErrorImage = "";
            ErrorData = eMsg;
            ErrorMessage = "[ 통신프로그램 에러 ]\r" + eMsg;
            WriteLog("!! Error !! : " + eMsg);
            ButtonVisible("Error");  // 에러 창 On
            Thread.Sleep(200);
        }



        public bool RobotLabelSet()
        {
            R_IsOrigin = R_IsReady = R_IsRunning = R_IsStopped = ResetbtnEnable = false;
            return true;
        }



        public void WriteLog(string message)
        {
            string dirPath = path + "\\Log";
            string filePath = Path.Combine(dirPath, $"Log_{DateTime.Today:yyyy-MM-dd}.log");

            // 디렉토리 및 파일 생성
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            if (!File.Exists(filePath))
            {
                using (FileStream fs = File.Create(filePath))
                {
                    fs.Close();
                }
            }

            // 로그 메세지 포맷팅
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

            // 파일에 로그 메세지 추가
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(logMessage);
            }
        }
        #endregion





        #region Server Methods
        public void ServerOpenClose()
        {
            // 서버 열기/닫기 토글
            IsServerOpened = !IsServerOpened;

            if (IsServerOpened)
            {
                ServerListening = new Thread(new ThreadStart(delegate
                {
                    try
                    {
                        m_Server = new TcpListener(IPAddress.Parse(ClientIP), ClientPort);
                        m_Server.Start();

                        byte[] bytes = new byte[512];
                        string data = null;

                        while (IsServerOpened)
                        {
                            try
                            {
                                m_Client = m_Server.AcceptTcpClient();
                                m_Stream = m_Client.GetStream();

                                ServerConnection = true;
                                responseTimer.Start();

                                int len = 0;
                                while ((len = m_Stream.Read(bytes, 0, bytes.Length)) != 0)
                                {
                                    ParsingRobMsg(ReadByteToString(bytes, len));
                                }

                                m_Client.Close();
                                ServerConnection = false;
                                responseTimer.Stop();
                            }
                            catch (SocketException ex) // 소켓 예외 처리
                            {
                                SystemError(ex.Message);
                            }
                            catch (IOException) { }
                            catch (Exception ex) // 일반 예외 처리
                            {
                                SystemError(ex.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        IsServerOpened = false;
                        SystemError(ex.Message);
                    }
                }));

                ServerListening.Start();
            }
            else
            {
                ServerClose();
                responseTimer.Stop();
                RobotLabelSet();
            }
        }



        internal void ParsingRobMsg(string msg)
        {
            try
            {
                if (msg == null || msg.Length < 4) { return; }  // 메세지 유효성 검사
                string[] splitMsg = msg.Split(',');

                // _를 기준으로 라벨과 메세지 분리
                for (int i = 0; i < splitMsg.Length; i++)
                {
                    if (splitMsg[i].Length < 2 || splitMsg[i] == null) return;
                    string label = "";
                    string message = "";
                    bool lm = false;

                    foreach (char s in splitMsg[i])
                    {
                        if (lm == false)
                        {
                            label += s;
                        }
                        else
                        {
                            message += s;
                        }
                        if (s == '_')
                        {
                            lm = true;
                        }
                    }
                    if (label == "State_") RobState(message);

                    if (label == "Speed_") // _를 기준으로 숫자(서브라벨)과 값(메세지) 분리
                    {
                        string[] sublabel = message.Split("_");
                        if (sublabel.Length < 2) return;
                        RobSpeed(sublabel[0], sublabel[1]);
                    }

                    if (label == "Count_")
                    {
                        string[] sublabel = message.Split("_");
                        if (sublabel.Length < 2) return;
                        RobCount(sublabel[0], sublabel[1]);
                    }

                    if (label == "Motor_")
                    {
                        string[] sublabel = message.Split("_");
                        if (sublabel.Length < 2) return;
                        RobMotor(sublabel[0], sublabel[1]);
                    }

                    if (label == "Error_") RobError(message);

                    if (label == "Work_") RobWork(message);

                    if (label == "RobIO_") RobIO(message);

                    if (label == "Check_") RobPress(message);
                }
            }
            catch (Exception ex)
            {
                SystemError(ex.Message);
            }
        }



        internal void ServerClose()
        {
            IsServerOpened = false;
            ServerConnection = false;
            if (m_Stream != null) m_Stream.Close();
            if (m_Client != null) m_Client.Close();
            if (m_Server != null) m_Server.Stop();
            if (_sendTimer != null) _sendTimer.Stop();
            if (ServerListening != null && ServerListening.IsAlive) ServerListening.Join();
        }



        internal void ServerSend(string ServerSendMsg)
        {

            try
            {
                if (string.IsNullOrEmpty(ServerSendMsg)) return; // 빈 문자열 체크

                _sendQueue.Enqueue(ServerSendMsg); // 큐에 메세지 추가
            }
            catch (Exception ex)
            {
                SystemError(ex.Message);
            }
        }


        // 큐 처리 메서드
        private void ProcessSendQueue(object sender, EventArgs e)
        {
            if (m_Stream == null || !m_Stream.CanWrite || !ServerConnection)
            {
                return;
            }

            if (_sendQueue.IsEmpty) return;

            try
            {
                string currentPrefix = Prefix ?? "";
                string currentSuffix = Suffix ?? "";

                StringBuilder packetBuilder = new StringBuilder();

                while (_sendQueue.TryPeek(out string nextMsgBody))
                {
                    string fullSingleMessage = currentPrefix + nextMsgBody + currentSuffix;

                    // [길이 체크]
                    // (현재 담아둔 길이) + (이번에 추가할 메세지 길이)가 최대사이즈(80자)를 넘는지 확인
                    if (packetBuilder.Length + fullSingleMessage.Length > MAX_PACKET_SIZE)
                    {
                        // [예외 처리]
                        // 만약 현재 패킷이 비어있는데(0자), 메세지 하나 자체가 최대사이즈(80자)를 넘는 경우
                        // -> 안 보내면 큐가 영원히 막히므로, 잘리더라도 강제로 담아서 전송
                        if (packetBuilder.Length == 0)
                        {
                            _sendQueue.TryDequeue(out _); // 큐에서 제거
                            packetBuilder.Append(fullSingleMessage);
                            break; // 루프 종료 후 전송
                        }

                        // 이미 담아둔 데이터가 있다면, 이번 메세지는 다음 텀에 송신
                        break;
                    }

                    // 80자 이내로 들어간다면 큐에서 실제로 꺼내서 패킷에 추가
                    _sendQueue.TryDequeue(out _);
                    packetBuilder.Append(fullSingleMessage);
                }

                // 전송할 데이터가 있다면 전송
                if (packetBuilder.Length > 0)
                {
                    // WriteStringAsString 메서드는 이미 인코딩을 처리하므로 그대로 사용
                    byte[] msgBytes = WriteStringAsString(packetBuilder.ToString());

                    // 중복 호출을 막기 위해 lock 사용
                    lock (thisLock)
                    {
                        m_Stream.Write(msgBytes, 0, msgBytes.Length);
                        m_Stream.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Send Queue Error: {ex.Message}");
            }
        }



        private void ConnectionOK(object sender, EventArgs e)
        {
            ServerSend("ConnectionOK");  // 통신 상태 확인용 인데 E10이 통신 오류 있어서 있으나 마나 ( 관상용 )

            // 커넥션 확인 할 때마다 매뉴얼 온오프 상태 전송 ( 이젠 이 녀석이 이 메소드의 메인... 메소드명 바꿔라 애송이... )
            if (IsManual) ServerSend("Manual_On");
            else ServerSend("Manual_Off");
        }



        internal void ServerClear()
        {
            ServerOutput = "";
        }
        #endregion





        #region Server Message 
        private void RobState(string message)
        {
            switch (message)
            {
                case "Ready":
                    if (!R_IsReady)
                    {
                        R_IsReady = RobotLabelSet();
                        WriteLog("System_State : Ready");
                        ErrorData = "";
                        ResetbtnEnable = true;
                        // 에러 메세지 초기화
                        if (IsError) IsAuto = true;
                        ErrorImage = path + "";
                        ErrorMessage = "";
                    }
                    // 리셋 버튼 플리커 해제
                    FlickerTimer.Stop();
                    colorCount = 0;
                    ResetBtnOpacity = 1;
                    PressIOEnable = false;
                    StartButtonColor = "LimeGreen";
                    break;
                case "Stopped":
                    if (!R_IsStopped)
                    {
                        R_IsStopped = RobotLabelSet();
                        WriteLog("System_State : Stopped");
                        StartButtonColor = "Gray";
                        ResetbtnEnable = true;
                    }
                    break;
                case "Running":
                    if (!R_IsRunning)
                    {
                        R_IsRunning = RobotLabelSet();
                        WriteLog("System_State : Running");
                    }
                    break;
                case "OriginStart":
                    R_IsOrigin = RobotLabelSet();
                    IsAuto = true;
                    break;
                case "OriginEND":
                    R_IsStopped = RobotLabelSet();
                    ButtonVisible("Auto");
                    resetbtnEnable = true;
                    needOrigin = false;
                    break;
                case "ShotCycleEnd":
                    WriteLog("System_Cycle_End");
                    switch (IsWorkIndex)
                    {
                        case 1:
                            Dev1StackedShotCount++;
                            Dev1UnStackedShotCount++;
                            Dev1StackedDeviceCount = Dev1StackedShotCount * m_XmlParser.SavedData.SavedDeviceSize1;
                            Dev1UnStackedDeviceCount = Dev1UnStackedShotCount * m_XmlParser.SavedData.SavedDeviceSize1;

                            Display_StackedShotCount = Dev1StackedShotCount.ToString("N0");
                            Display_UnStackedShotCount = Dev1UnStackedShotCount.ToString("N0");
                            Display_StackedDeviceCount = Dev1StackedDeviceCount.ToString("N0");
                            Display_UnStackedDeviceCount = Dev1UnStackedDeviceCount.ToString("N0");
                            break;
                        case 2:
                            Dev2StackedShotCount++;
                            Dev2UnStackedShotCount++;
                            Dev2StackedDeviceCount = Dev2StackedShotCount * m_XmlParser.SavedData.SavedDeviceSize2;
                            Dev2UnStackedDeviceCount = Dev2UnStackedShotCount * m_XmlParser.SavedData.SavedDeviceSize2;

                            Display_StackedShotCount = Dev2StackedShotCount.ToString("N0");
                            Display_UnStackedShotCount = Dev2UnStackedShotCount.ToString("N0");
                            Display_StackedDeviceCount = Dev2StackedDeviceCount.ToString("N0");
                            Display_UnStackedDeviceCount = Dev2UnStackedDeviceCount.ToString("N0");
                            break;
                        case 3:
                            Dev3StackedShotCount++;
                            Dev3UnStackedShotCount++;
                            Dev3StackedDeviceCount = Dev3StackedShotCount * m_XmlParser.SavedData.SavedDeviceSize3;
                            Dev3UnStackedDeviceCount = Dev3UnStackedShotCount * m_XmlParser.SavedData.SavedDeviceSize3;

                            Display_StackedShotCount = Dev3StackedShotCount.ToString("N0");
                            Display_UnStackedShotCount = Dev3UnStackedShotCount.ToString("N0");
                            Display_StackedDeviceCount = Dev3StackedDeviceCount.ToString("N0");
                            Display_UnStackedDeviceCount = Dev3UnStackedDeviceCount.ToString("N0");
                            break;
                        default:
                            break;
                    }

                    break;
                case "LoadSkipTrue": LoadSkipColor = "Lime"; break;
                case "LoadSkipFalse": LoadSkipColor = "Gray"; break;
                case "ImgCollectTrue": ImgCollectColor = "Lime"; break;
                case "ImgCollectFalse": ImgCollectColor = "Gray"; break;
                default:
                    break;
            }
        }



        private void RobSpeed(string label, string message)
        {
            switch (label)
            {
                case "Override":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobSpeed_Persent) WriteLog("Override Change : " + IsRobSpeed_Persent + " → " + message);
                        IsRobSpeed_Persent = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Max":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobSpeed_MMS) WriteLog("Max Speed Change : " + IsRobSpeed_MMS + " → " + message);
                        IsRobSpeed_MMS = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Blow":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobBlowSpeed) WriteLog("Blow Speed Change : " + IsRobBlowSpeed + " → " + message);
                        IsRobBlowSpeed = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Spray":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobSpraySpeed) WriteLog("Spray Speed Change : " + IsRobSpraySpeed + " → " + message);
                        IsRobSpraySpeed = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Port":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobPortSpeed) WriteLog("Port Speed Change : " + IsRobPortSpeed + " → " + message);
                        IsRobPortSpeed = message;
                    }
                    catch (Exception) { return; }
                    break;
            }
        }



        private void RobCount(string label, string message)
        {
            switch (label)
            {
                case "PCB":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (IsPCBCount == "Empty" && a < 20)
                        {
                            string LogMsg = "PCB Count Change : Empty → " + message;
                            WriteLog(LogMsg);
                        }
                        else if (message != IsPCBCount && IsPCBCount != "Empty")
                        {
                            string LogMsg = (a < 21) ? "PCB Count Change : " + IsPCBCount + " → " + message : "PCB Count Change : " + IsPCBCount + " → " + "Empty";
                            WriteLog(LogMsg);
                        }
                        IsPCBCount = (a < 21) ? message : "Empty";
                    }
                    catch (Exception) { return; }
                    break;
                case "Blow1stTop":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobBlow1stTopCount) WriteLog("Blow1stTop Count Change : " + IsRobBlow1stTopCount + " → " + message);
                        IsRobBlow1stTopCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Blow1stBot":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobBlow1stBotCount) WriteLog("Blow1stBot Count Change : " + IsRobBlow1stBotCount + " → " + message);
                        IsRobBlow1stBotCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Blow2ndTop":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobBlow2ndTopCount) WriteLog("Blow2ndTop Count Change : " + IsRobBlow2ndTopCount + " → " + message);
                        IsRobBlow2ndTopCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Blow2ndBot":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobBlow2ndBotCount) WriteLog("Blow2ndBot Count Change : " + IsRobBlow2ndBotCount + " → " + message);
                        IsRobBlow2ndBotCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "BlowPort":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobBlowPortCount) WriteLog("BlowPort Count Change : " + IsRobBlowPortCount + " → " + message);
                        IsRobBlowPortCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "SprayTop":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobSprayTopCount) WriteLog("SprayTop Count Change : " + IsRobSprayTopCount + " → " + message);
                        IsRobSprayTopCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "SprayPort": // 포트 분사 딜레이 타임
                    try
                    {
                        double a = Convert.ToDouble(message);
                        if (message != IsRobSprayPortTime) WriteLog("SprayPort Delay Time Change : " + IsRobSprayPortTime + " → " + message);
                        IsRobSprayPortTime = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Extract":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsExtactCount)
                        {
                            if (a < 11)
                            {
                                WriteLog("Extract Count Change : " + IsExtactCount + " → " + message);
                                IsExtactCount = message;
                            }
                            else
                            {
                                WriteLog("Extract Count Change : " + IsExtactCount + " → Full");
                                isExtractCount = "Full";
                            }

                        }
                    }
                    catch (Exception) { return; }
                    break;
                case "Port":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobPortCount) WriteLog("Port Count Change : " + IsRobPortCount + " → " + message);
                        IsRobPortCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "PressCloseTime":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsPressCloseTime) WriteLog("Press Close Signal Time Change : " + IsPressCloseTime + " → " + message);
                        IsPressCloseTime = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "ChamberCloseTime":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsChamberCloseTime) WriteLog("Chamber Close Signal Time Change : " + IsChamberCloseTime + " → " + message);
                        IsChamberCloseTime = message;
                    }
                    catch (Exception) { return; }
                    break;
                default:
                    break;
            }
        }



        private void RobError(string message)
        {
            switch (message)
            {
                // 안전 관련 에러
                case "EStop": Error("Emergency", "[비상정지]"); break;
                case "DoorOpen": Error("Door", "[도어 열림 감지]"); break;

                // 작업 관련 에러
                case "WorkMode_Select": Error("WorkMode", "작업모드 선택 이상"); break;
                case "PCBMagazineEmpty": Error("Supply", "[Lot End]\r 작업완료 ( 매거진 없음 )"); break;
                case "ExtractCountOver": Error("", "10 샷 작업 완료\r 금형 클리닝 및 완제품 배출구를 비워주세요."); break;

                // 매거진 서플라이 관련 에러
                case "Servo_M1_ORGTO": Error("Supply", "매거진 서플라이 모터 오리진 동작 이상"); break;
                case "Servo_M1_SupTO": Error("Supply", "매거진 서플라이 모터 공급 동작 이상"); break;
                case "Servo_M1_RelTO": Error("Supply", "매거진 서플라이 회피위치 기동 동작 이상"); break;
                case "Cylinder_Supply_Left": Error("Supply", "매거진 서플라이 실린더 좌측 이동 동작 이상"); break;
                case "Cylinder_Supply_Right": Error("Supply", "매거진 서플라이 실린더 우측 이동 동작 이상"); break;
                case "MagazineSensorOn": Error("Supply", "매거진 센서 감지 이상"); break;
                case "PCBWorkCountOver": Error("Supply", "매거진 PCB 작업수량 초과"); break;
                case "Servo_M1_SupSensor": Error("Supply", "매거진 공급동작 신호시 센서 감지됨\r 매거진을 뒤로 물리고 다시 시작 해 주세요."); break;

                // 턴 테이블 관련 에러
                case "Servo_M2_ORGTO": Error("Table", "PCB 턴 테이블 오리진 이상"); break;
                case "Servo_M2_0TO": Error("Table", "PCB 턴 테이블 0도 동작 이상"); break;
                case "Servo_M2_180TO": Error("Table", "PCB 턴 테이블 180도 동작 이상"); break;

                // 로봇 관련 에러
                case "RobEResetFail": Error("Robot", "로봇 에러 리셋 실패"); FlickerTimer.Stop(); break;
                case "RobMotorFail": Error("Robot", "로봇 모터 가동 실패"); FlickerTimer.Stop(); break;
                case "RobError": Error("Robot", "[로봇 에러]\r로봇 에러 발생"); break;
                case "RobControlBGStoped": Error("Robot", "로봇 백그라운드 프로그램 정지상태"); break;

                // 프리히터 관련 에러
                case "PreheaterPowerOff": Error("Preheater", "프리히터 전원 OFF 이상"); break;
                case "Preheater_NotOpen": Error("Preheater", "프리히터 신호이상\r 프리히터 도어 열림 이상"); break;

                // EMC 공급함 관련 에러
                case "EMCBoxPickMiss": Error("", "EMC 박스 픽업 이상"); break;
                case "EMC_NotSupply": Error("EMCEmpty", "EMC 공급함 에러\r EMC 도달 감지 이상"); break;
                case "EMC_CylNotDown": Error("EMCCylDown", "EMC 공급함 에러\r EMC 개폐실린더 하강 이상"); break;

                // 프레스 관련 에러
                case "PrsSign1TimeOut": Error("", "프레스 신호 이상\r 프레스 준비 신호 수신 이상"); break;
                case "PrsSign2TimeOut": Error("", "프레스 신호 이상\r 프레스 상승 완료 신호 수신 이상"); break;
                case "PrsSign3TimeOut": Error("", "프레스 신호 이상\r 쳄버 닫힘 완료 신호 수신 이상"); break;
                case "PrsSign4TimeOut": Error("", "프레스 신호 이상\r 프레스 열림 신호 수신 이상"); break;
                case "PrsSign5TimeOut": Error("", "프레스 신호 이상\r 프레스 준비위치 도달 신호 이상"); break;

                // Robot Tool 관련 에러
                case "Chuck_Sensor": Error("ChuckSensor", "툴 교체 센서 이상"); break;
                case "ElectricGripperError": Error("EGripper", "전동 그리퍼 동작이상"); break;
                case "Extract_Table": Error("ExtactVac", "추출 툴 진공센서 이상\r 턴 테이블 PCB 픽업 이상"); break;
                case "Extract_Press": Error("ExtactVac", "추출 툴 진공센서 이상\r 프레스 완제품 픽업 이상"); break;
                case "Extract_Out": Error("ExtactVac", "추출 툴 진공센서 이상\r 배출부 이송중 제품 이탈 이상"); break;
                case "SprayCylDown": Error("SpraySensor", "이형제 분사 실린더 하강 이상"); break;
                case "SprayCylUp": Error("SpraySensor", "이형제 분사 실린더 상승 이상"); break;
                case "PCBTurnTableError": Error("", "PCB 턴테이블 동작 이상"); break;

                // 비전 관련 에러
                case "VisionNotReady": Error("Vision", "비전 검사 준비 이상\r 비전 프로그램 동작 확인"); break;
                case "VisionSignal": Error("Vision", "비전 검사 신호 이상"); break;
                case "VisionNG_PCB": Error("VisionPCB", "Vision NG\r 프레스 내 PCB 안착 이상"); break;
                case "VisionNG_Press": Error("VisionPress", "Vision NG\r 프레스 이물 검사 이상"); break;
                case "VisionNG_Table": Error("VisionTable", "Vision NG\r PCB 로드 테이블 안착 이상"); break;
                case "VisionNG_Extract": Error("VisionExtract", "Vision NG\r 완제품 성형 이상"); break;

                // 에러 통신 메세지 이상 ( 미할당 )
                default: Error("", $"에러메세지 미 할당\rCode : {message}"); break;
            }
        }



        private void RobWork(string message)
        {
            switch (message)
            {
                case "Press_Ready": RobotMessage = "프레스 준비신호 대기중"; break;
                case "ServoHome": RobotMessage = "서보모터 오리진 완료 대기중"; break;
                case "Home": RobotMessage = "로보트 오리진 진행중"; break;
                case "Blow": RobotMessage = "금형 클리닝"; break;
                case "Spray": RobotMessage = "이형제 분사"; break;
                case "PCB_TablePick": RobotMessage = "PCB 테이블 픽업"; break;
                case "PCB_PressPlace": RobotMessage = "PCB 프레스에 공급"; break;
                case "Signal1": RobotMessage = "금형 상승 신호 발신"; break;
                case "EMC_PreHeaterSupply": RobotMessage = "프리히터 EMC 공급"; break;
                case "EMC_Button": RobotMessage = "프리히터 동작 버튼 조작"; break;
                case "EMC_PicknPlace": RobotMessage = "프레스 EMC 공급"; break;
                case "Signal2": RobotMessage = "트랜스퍼 하강 신호 발신"; break;
                case "PCB_Magazine": RobotMessage = "매거진 PCB 추출"; break;
                case "PCB_TableLoad": RobotMessage = "PCB 테이블 로딩"; break;
                case "Extract1": RobotMessage = "완제품 추출"; break;
                case "Extract2": RobotMessage = "완제품 배출"; break;
                case "Vision": RobotMessage = "비전 검사"; break;
                case "ToolChange": RobotMessage = "툴 교체"; break;
                case "PressWait": RobotMessage = "패키지 성형 완료 대기"; break;
                case "Clear": RobotMessage = ""; break;

                default: break;
            }
        }



        private void RobIO(string message)
        {
            switch (message)
            {
                case "AutoOn": RobAutoColor = "Lime"; break;
                case "AutoOff": RobAutoColor = "Gray"; break;
                case "MotorOn": RobMotorColor = "Lime"; break;
                case "MotorOff": RobMotorColor = "Gray"; break;
                case "TaskRunOn": RobTaskRunColor = "Lime"; break;
                case "TaskRunOff": RobTaskRunColor = "Gray"; break;
                case "BatteryLow":
                    {
                        WriteLog("Robot fatal error ! : Battery Low !!");
                        RobBatteryColor = "Red";
                    }
                    break;
                case "BatteryHigh": RobBatteryColor = "Gray"; break;
                default:
                    break;
            }
        }



        private void RobPress(string message)
        {
            switch (message)
            {
                // Robot Digital In Signal
                case "In1On": In1Color = "SkyBlue"; break;
                case "In1Off": In1Color = "White"; break;

                case "In2On": In2Color = "SkyBlue"; break;
                case "In2Off": In2Color = "White"; break;

                case "In3On": In3Color = "SkyBlue"; break;
                case "In3Off": In3Color = "White"; break;

                case "In4On": In4Color = "SkyBlue"; break;
                case "In4Off": In4Color = "White"; break;

                case "In5On": In5Color = "SkyBlue"; break;
                case "In5Off": In5Color = "White"; break;

                case "In6On": In6Color = "SkyBlue"; break;
                case "In6Off": In6Color = "White"; break;

                // Robot Digital Out Signal
                case "Out1On": Out1Color = "SkyBlue"; Btn1Checked = true; break;
                case "Out1Off": Out1Color = "White"; Btn1Checked = false; break;

                case "Out2On": Out2Color = "SkyBlue"; Btn2Checked = true; break;
                case "Out2Off": Out2Color = "White"; Btn2Checked = false; break;

                case "Out3On": Out3Color = "SkyBlue"; Btn3Checked = true; break;
                case "Out3Off": Out3Color = "White"; Btn3Checked = false; break;

                case "Out4On": Out4Color = "SkyBlue"; Btn4Checked = true; break;
                case "Out4Off": Out4Color = "White"; Btn4Checked = false; break;

                case "Out5On": Out5Color = "SkyBlue"; Btn5Checked = true; break;
                case "Out5Off": Out5Color = "White"; Btn5Checked = false; break;

                default:
                    break;
            }
        }



        private void RobMotor(string label, string message)
        {
            switch (label)
            {
                case "M1Current":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        M1_CurrentPosition = a.ToString("N0");
                    }
                    catch (Exception ex) { WriteLog(ex.Message); }
                    break;
                case "M2Current":
                    try
                    {
                        if (message == "0") { M2_CurrentPosition = "0"; break; }
                        double a = Convert.ToDouble(message) / 100000;  // 모터 위치 표기가 소수점 없이 수신되므로 소수점 위치 옮겨줘야함...
                        M2_CurrentPosition = a.ToString("N5");  // 표시는 소수점 5자리까지
                    }
                    catch (Exception ex) { WriteLog(ex.Message); }
                    break;
                case "M1MoveSpd":
                    try
                    {
                        if (message == "0") { M1_Speed = "0"; break; }
                        int a = Convert.ToInt32(message);
                        M1_Speed = a.ToString("N0");
                    }
                    catch (Exception ex) { WriteLog(ex.Message); }
                    break;
                case "M2MoveSpd":
                    try
                    {
                        if (message == "0") { M2_Speed = "0"; break; }
                        int a = Convert.ToInt32(message);
                        M2_Speed = a.ToString("N0");
                    }
                    catch (Exception ex) { WriteLog(ex.Message); }
                    break;
                default:
                    break;
            }
        }
        #endregion





        #region Timer Methodes

        public void TimerCount(object sender, EventArgs e) // 버튼 눌림 딜레이타임용 메소드
        {
            try
            {
                TimerStack++;  // 디스패쳐 타이머인 timer 호출시 스택 증가 ( 100ms ) 이거 퍼블릭으로 선언하려고 했다가 뭔가가 안 되서 겟셋으로 해 놨는데 머가 안됐는지 기억 안남...

                switch (TimerWorkSelect)
                {
                    case "WorkCountReset":  // 작업 수량 초기화 버튼 동작 딜레이
                        if (TimerStack > CountresetBtnDelay)
                        {
                            if (IsWorkIndex == 1)
                            {
                                Dev1UnStackedDeviceCount = 0;
                                Dev1UnStackedShotCount = 0;
                            }
                            Display_UnStackedShotCount = "0";
                            Display_UnStackedDeviceCount = "0";
                            timer.Stop();
                            TimerStack = 0;
                        }
                        break;

                    case "PCBCountReset":  // 매거진의 PCB 작업 수량 초기화 버튼 동작 딜레이
                        if (TimerStack > CountresetBtnDelay)
                        {
                            ServerSend("RobPCBCount_1");
                            timer.Stop();
                            TimerStack = 0;
                        }
                        break;

                    case "ExtractCountReset":  // 배출 작업 완료 수량 초기화 버튼 동작 딜레이
                        if (TimerStack > CountresetBtnDelay)
                        {
                            ServerSend("RobExtractCount_1");
                            timer.Stop();
                            TimerStack = 0;
                        }
                        break;

                    case "StartButtonDown":  // 설비 가동 버튼 동작 딜레이
                        if (TimerStack > StartBtnDelay)
                        {
                            StartButtonColor = "DarkGreen";
                            ServerSend("Start");
                            timer.Stop();
                            TimerStack = 0;
                        }
                        break;

                    case "RobotHome":  // 로봇 홈 위치 이동 버튼 동작 딜레이
                        if (TimerStack > HomeBtnDelay)
                        {
                            ServerSend("Home");
                            timer.Stop();
                            TimerStack = 0;
                        }
                        break;

                    case "Stop":  // 정지 할 때 까지 ( Max 3s ) 200ms 간격으로 정지 신호 전송
                        if (TimerStack % 2 == 0) ServerSend("Stop");
                        if (R_IsStopped) timer.Stop();
                        if (TimerStack > 30) { timer.Stop(); TimerStack = 0; }
                        break;

                    case "Stoooooop": // EGG
                        if (timerBreak) timer.Stop();
                        if (TimerStack > CountresetBtnDelay + StartBtnDelay + HomeBtnDelay + 6 + 6 + 6)
                        {
                            timer.Stop();
                            TimerStack = 0;
                            Stoooooop();
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex) { SystemError(ex.Message); }
        }



        public void ResetButtonFlicker(object sender, EventArgs e)  // 리셋 버튼 플리커(깜빡깜빡) 타이머 메소드
        {
            if (OpReverse == false) ResetBtnOpacity -= 0.02;
            if (ResetBtnOpacity <= 0) OpReverse = true;
            if (OpReverse) ResetBtnOpacity += 0.02;
            if (ResetBtnOpacity >= 1) OpReverse = false;
            colorCount++;  // 얘는 또 퍼블릭이네...
            if (colorCount % 100 == 0) ServerSend("Reset");
            if (colorCount > 500)
            {
                StopButtonPress();
                ButtonVisible("Error");
                ErrorMessage = "로봇 초기화 실패.  ";
            }
        }



        public void DeviceSend(object sender, EventArgs e)  // 주기적 (383 ms) 으로 디바이스 번호 전송
        {

            if (WorkMode == "" || WorkMode == null) return;
            ServerSend("WorkMode_" + IsWorkIndex);
        }
        #endregion





        #region MainWindow Button Action
        internal void StartButtonDown(MouseButtonEventArgs e)  // 로봇 가동 시작
        {
            if (!R_IsReady) return;
            if (!IsWorkModeSelected || WorkMode == "") { MessageBox.Show("작업 모드가 선택되지 않았습니다."); return; }
            if (needOrigin) { Error("", "작업모드 변경 후 HOME 동작 미완료"); return; }
            StartButtonColor = "#63AA00";
            TimerWorkSelect = "StartButtonDown";
            timer.Start();
        }



        internal void StartButtonUp()
        {
            StartButtonColor = "LimeGreen";
            timer.Stop();
            TimerStack = 0;
            if (RobBatteryColor == "Red") MessageBox.Show("로봇의 배터리가 부족합니다!!\r !!전원을 유지한 상태로!!\r !!전원을 유지한 상태로!!\r 배터리를 교체 해 주세요."); ;
            // 다운에 걸 경우 스타트 안 걸릴듯, 업에 걸어서 스타트는 하고 메세지 박스 뜨도록 ( 이랬는데 작업자가 팝언 아보면 말짱 꽝 이긴 함, 나중에 라도 보것지 아마 )
        }



        internal void StopButtonPress()
        {
            ResetbtnEnable = true;
            ResetBtnOpacity = 1;
            OpReverse = false;
            timerBreak = false;
            FlickerTimer.Stop();
            colorCount = 0;
            ServerSend("Stop");
            if (IsAuto)
            {
                TimerWorkSelect = "Stop";
                timer.Start();
            }
            if (IsManual)
            {
                TimerWorkSelect = "Stoooooop";
                timer.Start();
            }
        }



        internal void StopButtonUp()
        {
            timerBreak = true;
            TimerStack = 0;
        }



        internal void ResetButtonPress()
        {
            if (!R_IsStopped) return;
            if (!IsWorkModeSelected || WorkMode == "") { IsAuto = true; MessageBox.Show("작업 모드가 선택되지 않았습니다."); return; }
            if (IsWorkIndex == 0) { IsAuto = true; MessageBox.Show("정지모드가 선택되어 있습니다."); return; }
            ErrorMessage = "";
            ErrorImage = "";
            ButtonVisible("Auto");
            if (RobAutoColor == "Gray") return;
            ResetbtnEnable = false;
            OpReverse = false;    // 플리커
            ResetBtnOpacity = 1;  // 관련
            colorCount = 0;       // 변수들
            ServerSend("Reset");
            FlickerTimer.Start();
        }



        internal void ResetButtonUp()
        {

        }



        internal void SettingButtonPress()
        {
            LoginCheck = false;
            DisplayedData = "";
            if (IsSetting) ButtonVisible("Auto");
            else
            {
                CallLogin();
                if (!LoginCheck) return;
                ButtonVisible("Setting");
                RobotSetting = true;
            }
        }



        internal void SetViewButtonPress()
        {
            if (IsSetView) ButtonVisible("Auto");
            else { ButtonVisible("SetView"); RobotSetting = true; }
        }



        internal void AutoButtonPress()
        {
            if (IsSetting && AutoBtn) ButtonVisible("Auto");
            else ButtonVisible("Manual");

        }



        internal void ManualButtonPress()
        {
            if (IsSetting && ManualBtn) ButtonVisible("Auto");
            else ButtonVisible("Auto");
        }
        #endregion





        #region Auto Page Methods
        internal void WorkCountReset()
        {
            TimerWorkSelect = "WorkCountReset";
            timer.Start();
        }



        internal void CountResetPressUp()
        {
            timer.Stop();
            TimerStack = 0;
        }



        internal void LoadingsSkipPress()
        {
            if (R_IsStopped)
            {
                if (LoadSkipColor == "Lime")
                {
                    ServerSend("LoadingSkipOff");
                }
                else
                {
                    ServerSend("LoadingSkipOn");
                }

            }
            else return;
        }



        internal void LoadingSkipRelease()
        {
        }




        internal void PCBCount()
        {
            string MsgBuff;
            DisplayedData = IsPCBCount;
            MsgBuff = "RobPCBCount_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) > robPCBCountLimit[1] || Convert.ToInt32(KeyboardData) < robPCBCountLimit[0])
                { MessageBox.Show(robPCBCountLimit[0] + "~" + robPCBCountLimit[1] + "사이의 숫자만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show(robPCBCountLimit[0] + "~" + robPCBCountLimit[1] + "사이의 숫자만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void PCBResetPress()
        {
            TimerWorkSelect = "PCBCountReset";
            timer.Start();
        }



        internal void PCBResetRelease()
        {
            timer.Stop();
            TimerStack = 0;
            TCounting = "";
        }



        internal void ExtractCount()
        {
            string MsgBuff;
            DisplayedData = IsExtactCount;
            MsgBuff = "RobExtractCount_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) > robExtractCountLimit[1] || Convert.ToInt32(KeyboardData) < robExtractCountLimit[0])
                { MessageBox.Show(robExtractCountLimit[0] + "~" + robExtractCountLimit[1] + "사이의 숫자만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show(robExtractCountLimit[0] + "~" + robExtractCountLimit[1] + "사이의 숫자만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void ExtractResetPress()
        {
            TimerWorkSelect = "ExtractCountReset";
            timer.Start();
        }



        internal void ExtractResetRelease()
        {
            timer.Stop();
            TimerStack = 0;
        }



        internal void WorkModeOpen()  // 디바이스 선택 드롭다운 열릴 때 버퍼에 현대 디바이스 저장
        {
            workModeBuffer = WorkMode;
        }



        internal void WorkModeClose()  // 드롭다운 닫힐 때 (선택 했든 안 했든) 호출
        {
            if (IsSelectedWorkMode == "") { IsSelectedWorkMode = workModeBuffer; return; }
            if (IsSelectedWorkMode == null) { IsWorkModeSelected = false; return; }
            else
            {
                if (IsSelectedWorkMode == workModeBuffer) return;  // 디바이스가 이전과 같으면 돌아가
                // 메세지 받스 띄워서 확정 여부 확인
                var result = MessageBox.Show("선택한 작업은 " + IsSelectedWorkMode + " 입니다.\r 확정시 시스템 초기화를 진행 해야 합니다.\r 진행 하시겠습니까?", "", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)  // 진행 한다 하면 GO
                {
                    IsWorkModeSelected = true;  // 디바이스 변경 플래그 키고
                    needOrigin = true;          // "오리진 잡아야 해" 도 키고
                    WorkMode = IsSelectedWorkMode;
                    workModeBuffer = IsSelectedWorkMode;
                    WriteLog("WorkMode Change : " + WorkMode);
                    switch (IsWorkIndex)  // 디바이스에 따라 작업 수량 디스플레이에 갱신
                    {
                        case 1:
                            Display_StackedShotCount = Dev1StackedShotCount.ToString("N0");
                            Display_UnStackedShotCount = Dev1UnStackedShotCount.ToString("N0");
                            Display_StackedDeviceCount = Dev1StackedDeviceCount.ToString("N0");
                            Display_UnStackedDeviceCount = Dev1UnStackedDeviceCount.ToString("N0");
                            break;
                        case 2:
                            Display_StackedShotCount = Dev2StackedShotCount.ToString("N0");
                            Display_UnStackedShotCount = Dev2UnStackedShotCount.ToString("N0");
                            Display_StackedDeviceCount = Dev2StackedDeviceCount.ToString("N0");
                            Display_UnStackedDeviceCount = Dev2UnStackedDeviceCount.ToString("N0");
                            break;
                        case 3:
                            Display_StackedShotCount = Dev3StackedShotCount.ToString("N0");
                            Display_UnStackedShotCount = Dev3UnStackedShotCount.ToString("N0");
                            Display_StackedDeviceCount = Dev3StackedDeviceCount.ToString("N0");
                            Display_UnStackedDeviceCount = Dev3UnStackedDeviceCount.ToString("N0");
                            break;
                        default: // 정지모드 or Null 시
                            Display_StackedShotCount = "0";
                            Display_UnStackedShotCount = "0";
                            Display_StackedDeviceCount = "0";
                            Display_UnStackedDeviceCount = "0";
                            break;
                    }
                }
            }
            IsSelectedWorkMode = workModeBuffer;
            DeviceTimer.Start();
        }



        // 신호 확인용 버튼 메소드
        internal void OutBtn1Click()
        {
            ServerSend("Out1");
        }



        internal void OutBtn2Click()
        {
            ServerSend("Out2");
        }



        internal void OutBtn3Click()
        {
            ServerSend("Out3");
        }



        internal void OutBtn4Click()
        {
            ServerSend("Out4");
        }



        internal void OutBtn5Click()
        {
            ServerSend("Out5");
        }



        internal void OutBtn6Click()
        {
            ServerSend("Out6");
        }
        #endregion





        #region Setting Page Methods
        internal void DeviceNameSet()  // 디바이스 명 설정 윈도우 팝업
        {
            DeviceWindow deviceWindow = new DeviceWindow();
            var MainWithSize = MainWindow.Width;
            var MainHeigtSize = MainWindow.Height;
            var SettingWithSize = deviceWindow.Width;
            var SettingHeigtSize = deviceWindow.Height;

            deviceWindow.ResizeMode = ResizeMode.NoResize;
            deviceWindow.Topmost = true;
            deviceWindow.Left = (MainWithSize / 2) - (SettingWithSize / 2);
            deviceWindow.Top = (MainHeigtSize / 2) - (SettingHeigtSize / 2);

            deviceWindow.ShowDialog();

            m_XmlParser.SavedDataLoad();  // 설정 끝나면 저장된 데이터 로드

            // 워크 모드 리스트 갱신
            WorkModeList.Clear();
            WorkModeList.Add("정지모드"); // 0
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName1);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName2);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName3);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName4);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName5);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName6);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName7);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName8);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName9);
            WorkModeList.Add(m_XmlParser.SavedData.SavedDeviceName10);
        }


        internal void IPAdressBox()
        {
            DisplayedData = ClientIP;
            string MsgBuff = ClientIP;
            ClientIP = CallNumKey(DisplayedData);
            WriteLog("Client IP Change : " + MsgBuff + " → " + ClientIP);
        }



        internal void PortBox()
        {
            try
            {
                DisplayedData = ClientPort.ToString();
                string MsgBuff = ClientPort.ToString();
                ClientPort = Convert.ToInt32(CallNumKey(DisplayedData));
                WriteLog("Client Port Change : " + MsgBuff + " → " + ClientPort);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }



        internal void StartDelayBox()
        {
            try
            {
                DisplayedData = StartBtnDelay.ToString();
                string MsgBuff = StartBtnDelay.ToString();
                StartBtnDelay = Convert.ToInt32(CallNumKey(DisplayedData));
                WriteLog("Start Button Delay Change : " + MsgBuff + " → " + StartBtnDelay);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }



        internal void HomeDelayBox()
        {
            try
            {
                DisplayedData = HomeBtnDelay.ToString();
                string MsgBuff = HomeBtnDelay.ToString();
                HomeBtnDelay = Convert.ToInt32(CallNumKey(DisplayedData));
                WriteLog("Home Button Delay Change : " + MsgBuff + " → " + HomeBtnDelay);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }



        internal void CountResetDelayBox()
        {
            try
            {
                DisplayedData = CountresetBtnDelay.ToString();
                string MsgBuff = CountresetBtnDelay.ToString();
                CountresetBtnDelay = Convert.ToInt32(CallNumKey(DisplayedData));
                WriteLog("Count Reset Button Delay Change : " + MsgBuff + " → " + CountresetBtnDelay);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }



        internal void RobSpeedSet_Persent()
        {
            string MsgBuff;
            DisplayedData = IsRobSpeed_Persent;
            MsgBuff = "RobOverride_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) > robOverrideLimit[1] || Convert.ToInt32(KeyboardData) < robOverrideLimit[0])
                { MessageBox.Show(robOverrideLimit[0] + "~" + robOverrideLimit[1] + "사이의 숫자만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show(robOverrideLimit[0] + "~" + robOverrideLimit[1] + "사이의 숫자만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void RobSpeedSet_MMS()
        {
            string MsgBuff;
            DisplayedData = IsRobSpeed_MMS;
            MsgBuff = "RobMaxSpeed_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) > robSpeedMaxLimit[1] || Convert.ToInt32(KeyboardData) < robSpeedMaxLimit[0])
                { MessageBox.Show(robSpeedMaxLimit[0] + "~" + robSpeedMaxLimit[1] + "사이의 숫자만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show(robSpeedMaxLimit[0] + "~" + robSpeedMaxLimit[1] + "사이의 숫자만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void PressCloseTimeSet()
        {
            try
            {
                string MsgBuff;
                DisplayedData = IsPressCloseTime;
                MsgBuff = "PressCloseTime_" + CallNumKey(DisplayedData);

                if (!int.TryParse(KeyboardData, out int inputTime))
                {
                    if (KeyboardData != "") { MessageBox.Show("정수(숫자)만 입력 해 주시기 바랍니다."); }
                    return;
                }
                if (inputTime < 0)
                {
                    MessageBox.Show("0 또는 양의 정수만 입력 해 주시기 바랍니다.");
                    return;
                }

                ServerSend(MsgBuff);
            }
            catch (Exception e)
            {
                SystemError(e.Message);
            }
        }



        internal void ChamberCloseTimeSet()
        {
            try
            {
                string MsgBuff;
                DisplayedData = IsChamberCloseTime;
                MsgBuff = "ChamberCloseTime_" + CallNumKey(DisplayedData);
                if (!int.TryParse(KeyboardData, out int inputTime))
                {
                    if (KeyboardData != "") { MessageBox.Show("정수(숫자)만 입력 해 주시기 바랍니다."); }
                    return;
                }
                if (inputTime < 0)
                {
                    MessageBox.Show("0 또는 양의 정수만 입력 해 주시기 바랍니다.");
                    return;
                }
                ServerSend(MsgBuff);
            }
            catch (Exception e)
            {
                SystemError(e.Message);
            }
        }



        internal void ImageColloection()
        {
            if (R_IsStopped)
            {
                if (ImgCollectColor == "Lime")
                {
                    ServerSend("ImgCollectOff");
                }
                else
                {
                    ServerSend("ImgCollectOn");
                }
            }
        }



        internal void RobBlowSpeedSet()
        {
            string MsgBuff;
            DisplayedData = IsRobBlowSpeed;
            MsgBuff = "RobBlowSpeed_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) > robSpeedMaxLimit[1] || Convert.ToInt32(KeyboardData) < robSpeedMaxLimit[0])
                { MessageBox.Show(robSpeedMaxLimit[0] + "~" + IsRobSpeed_MMS + "사이의 숫자만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show(robSpeedMaxLimit[0] + "~" + IsRobSpeed_MMS + "사이의 숫자만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void RobSpraySpeedSet()
        {
            string MsgBuff;
            DisplayedData = IsRobSpraySpeed;
            MsgBuff = "RobSpraySpeed_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) > robSpeedMaxLimit[1] || Convert.ToInt32(KeyboardData) < robSpeedMaxLimit[0])
                { MessageBox.Show(robSpeedMaxLimit[0] + "~" + IsRobSpeed_MMS + "사이의 숫자만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show(robSpeedMaxLimit[0] + "~" + IsRobSpeed_MMS + "사이의 숫자만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void RobBlowPort()
        {
            string MsgBuff;
            DisplayedData = IsRobBlowPortCount;
            MsgBuff = "RobBlowPortCount_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) < 0)
                { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void RobBlowTop1()
        {
            string MsgBuff;
            DisplayedData = IsRobBlow1stTopCount;
            MsgBuff = "RobBlowTop1Count_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) < 0)
                { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void RobBlowTop2()
        {
            string MsgBuff;
            DisplayedData = IsRobBlow2ndTopCount;
            MsgBuff = "RobBlowTop2Count_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) < 0)
                { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void RobBlowBot1()
        {
            string MsgBuff;
            DisplayedData = IsRobBlow1stBotCount;
            MsgBuff = "RobBlowBot1Count_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) < 0)
                { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void RobBlowBot2()
        {
            string MsgBuff;
            DisplayedData = IsRobBlow2ndBotCount;
            MsgBuff = "RobBlowBot2Count_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) < 0)
                { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void RobSprayTop()
        {
            string MsgBuff;
            DisplayedData = IsRobSprayTopCount;
            MsgBuff = "RobSprayTopCount_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) < 0)
                { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void RobSprayPort() // Count에 있지만 이 아이는 Delay 입니다...
        {
            string MsgBuff;
            DisplayedData = IsRobSprayPortTime;
            MsgBuff = "RobSprayPortTime_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToDouble(KeyboardData) < 0)
                { MessageBox.Show("숫자만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show("숫자만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void RobPortSpeedSet()
        {
            string MsgBuff;
            DisplayedData = IsRobPortSpeed;
            MsgBuff = "RobPortSpeed_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) > robSpeedMaxLimit[1] || Convert.ToInt32(KeyboardData) < robSpeedMaxLimit[0])
                { MessageBox.Show(robSpeedMaxLimit[0] + "~" + IsRobSpeed_MMS + "사이의 숫자만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show(robSpeedMaxLimit[0] + "~" + IsRobSpeed_MMS + "사이의 숫자만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void RobPortCountSet()
        {
            string MsgBuff;
            DisplayedData = IsRobPortCount;
            MsgBuff = "RobPortCount_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) < 0)
                { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void M1_SpeedSet()
        {
            string MsgBuff;
            DisplayedData = M1_Speed;
            try
            {
                MsgBuff = "Motor1Speed_" + CallNumKey(DisplayedData);

            }
            catch { if (KeyboardData != "") { MessageBox.Show("입력된 값 오류"); } return; }
            ServerSend(MsgBuff);
        }



        internal void M2_SpeedSet()
        {
            string MsgBuff;
            DisplayedData = M2_Speed;
            try
            {
                MsgBuff = "Motor2Speed_" + CallNumKey(DisplayedData);
            }
            catch { if (KeyboardData != "") { MessageBox.Show("입력된 값 오류"); } return; }
            ServerSend(MsgBuff);
        }



        internal void EMC_DoorOpenDelaySet()
        {
            string MsgBuff;
            DisplayedData = EMC_CylOpenDelay;
            MsgBuff = "EMCDoorOpenDelay_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) < 0)
                { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void EMC_DoorCloseDelaySet()
        {
            string MsgBuff;
            DisplayedData = EMC_CylCloseDelay;
            MsgBuff = "EMCDoorCloseDelay_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) < 0)
                { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void EMC_StopperFWDDelaySet()
        {
            string MsgBuff;
            DisplayedData = EMC_StopperFWDDelay;
            MsgBuff = "EMCStopperFWDDelay_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) < 0)
                { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }



        internal void EMC_StopperBWDDelaySet()
        {
            string MsgBuff;
            DisplayedData = EMC_StopperBWDDelay;
            MsgBuff = "EMCStopperBWDDelay_" + CallNumKey(DisplayedData);
            try
            {
                if (Convert.ToInt32(KeyboardData) < 0)
                { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); return; }
            }
            catch { if (KeyboardData != "") { MessageBox.Show("자연수만 입력 해 주시기 바랍니다"); } return; }
            ServerSend(MsgBuff);
        }
        #endregion





        #region Manual Page Methods

        public void ScreenOffCountDown(int sec)
        {
            var timerMsgBox = new TimerMessageBox(sec);
            timerMsgBox.WindowStyle = WindowStyle.None;
            bool? resualt = timerMsgBox.ShowDialog();

            if (resualt == true)
            {
                MonitorControl.TurnOffMonitor();
                ButtonVisible("Auto");
            }
        }



        internal void ScreenOff()
        {
            ScreenOffCountDown(5);
        }



        internal void HomeButtonPress()
        {
            if (R_IsReady == false) { HomeButton = false; return; }
            TimerWorkSelect = "RobotHome";
            timer.Start();
        }



        internal void HomeButtonUp()
        {
            HomeButton = true;
            timer.Stop();
            TimerStack = 0;
        }



        internal void Servo2Home()
        {
            ServerSend("Servo2_Home");
        }



        internal void Servo2Turn()
        {
            ServerSend("Servo2_Turn");
        }



        internal void Servo2Return()
        {
            ServerSend("Servo2_Return");
        }



        internal void Servo1Home()
        {
            ServerSend("Servo1_Home");
        }



        internal void Servo1Feed()
        {
            ServerSend("Servo1_Feed");
        }



        internal void Servo1Release()
        {
            ServerSend("Servo1_Release");
        }



        internal void Servo1CylinderLeft()
        {
            ServerSend("Servo1_CylLeft");
        }



        internal void Servo1CylinderRight()
        {
            ServerSend("Servo1_CylRight");
        }



        internal void EMCOpen()
        {
            ServerSend("EMC_Open");
        }



        internal void EMCClose()
        {
            ServerSend("EMC_Close");
        }



        internal void EMCSuplly()
        {
            ServerSend("EMC_Supply");
        }



        internal void EMCStopperFWD()
        {
            ServerSend("EMC_StopperFWD");
        }



        internal void EMCStopperBWD()
        {
            ServerSend("EMC_StopperBWD");
        }
        #endregion





        #region DataConverter
        private string ReadByteToString(byte[] data, int len)
        {
            return Encoding.Default.GetString(data, 0, len);
        }



        private byte[] WriteStringAsString(string data)
        {
            return Encoding.Default.GetBytes(data);
        }
        #endregion





        #region Easter Egg
        public void EggEnd(object sender, EventArgs e)
        {
            eggCount++;
            if (eggCount > 41)
            {
                timer.Stop();
                eggTimer.Stop();
                FlickerTimer.Stop();
                ErrorImage = "";
                ButtonVisible("Auto");
            }
        }



        internal void Stoooooop()
        {
            if (CountresetBtnDelay == 6 && StartBtnDelay == 6 && HomeBtnDelay == 6)
            {
                IsAuto = false;
                IsManual = false;
                IsError = true;
                HomeButton = false;
                AutoBtn = true;
                ManualBtn = false;
                EGG = path + @"\\Images\\EGG\\notpass.gif";
                RobotMessage = "";
                eggCount = 0;
                eggTimer.Start();
            }
        }
        #endregion
    }













}
