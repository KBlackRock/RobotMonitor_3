using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RobotMonitor_3.Models;
using RobotMonitor_3.Services;
using RobotMonitor_3.Utilities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;

namespace RobotMonitor_3.ViewModels
{
    internal partial class MainWindow_ViewModel : ObservableObject
    {

        #region Member Variable
        private readonly TcpServerService _tcpService;
        private readonly RobotMessageParser _parser;
        private Dictionary<string, Action<string>> _commandMap;
        private readonly ErrorRepository _errorRepo = new ErrorRepository();
        private readonly Dictionary<string, ErrorInfo> _activeErrors = new Dictionary<string, ErrorInfo>();

        private McProtocolService _plc;
        private bool _isPlcReading = false;
        private bool _isPlcConnected;
        private bool _isPlcBusy;
        private int _retryTick;

        public bool IsPlcConnected
        {
            get => _isPlcConnected;
            private set { if (_isPlcConnected != value) { _isPlcConnected = value; OnPropertyChanged(); } }
        }

        private XmlParser m_XmlParser;

        DispatcherTimer timer;
        DispatcherTimer responseTimer;
        DispatcherTimer DeviceTimer;
        DispatcherTimer plcTimer;

        private readonly PlcDataProcessor _dataProcessor;

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
        private string m_PlcIp;
        private int m_PlcPort;
        private List<string> ClientMsgList;
        private int ClientMsgListNum;
        private bool buttonTiger;
        private bool _isPlcConnecting;

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
        public ICommand PLCDataClick { get; private set; }
        #endregion

        public SettingDatas Setting => SettingsStore.Current;

        #region OnPropertyChange Properties
        public ObservableCollection<ErrorInfo> ErrorList { get; set; } = new ObservableCollection<ErrorInfo>();
        public ObservableCollection<short> PlcInput { get; } = new ObservableCollection<short>(new short[100]);
        public ObservableCollection<short> PlcOutput { get; } = new ObservableCollection<short>(new short[100]);
        private ObservableCollection<string> _WorkModeList; public ObservableCollection<string> WorkModeList { get { return _WorkModeList; } set { _WorkModeList = value; OnPropertyChanged(); } }
        [ObservableProperty] private string errorImage;
        [ObservableProperty] private string windowPage;
        [ObservableProperty] private bool tabVisible;
        [ObservableProperty] private bool isRunning;
        [ObservableProperty] private bool isStopped;
        [ObservableProperty] private bool isReady;
        [ObservableProperty] private bool isOrigin;
        [ObservableProperty] private string startButtonColor;
        [ObservableProperty] private double resetBtnOpacity;
        [ObservableProperty] private bool resetbtnEnable;
        [ObservableProperty] private int timerStack;
        [ObservableProperty] private bool isAuto;
        [ObservableProperty] private bool isManual;
        [ObservableProperty] private bool isError;
        [ObservableProperty] private bool isSetView;
        [ObservableProperty] private bool autoBtn;
        [ObservableProperty] private bool manualBtn;
        [ObservableProperty] private bool isSetting;
        [ObservableProperty] private bool homeButton;
        [ObservableProperty] private string selectPage;
        [ObservableProperty] private string robotMessage;
        [ObservableProperty] private bool btnWorkWait;
        [ObservableProperty] private string robotImage;
        [ObservableProperty] private string isRobSpeed_Percent;
        [ObservableProperty] private string isRobSpeed_MMS;
        [ObservableProperty] private string isPressCloseTime;
        [ObservableProperty] private string isChamberCloseTime;
        [ObservableProperty] private string isPCBCount;
        [ObservableProperty] private string serverConnectBtnColor;
        [ObservableProperty] private string out1Color;
        [ObservableProperty] private string out2Color;
        [ObservableProperty] private string out3Color;
        [ObservableProperty] private string out4Color;
        [ObservableProperty] private string out5Color;
        [ObservableProperty] private string out6Color;
        [ObservableProperty] private string in1Color;
        [ObservableProperty] private string in2Color;
        [ObservableProperty] private string in3Color;
        [ObservableProperty] private string in4Color;
        [ObservableProperty] private string in5Color;
        [ObservableProperty] private string in6Color;
        [ObservableProperty] private string sv1pw_borderColor;
        [ObservableProperty] private string sv2pw_borderColor;
        [ObservableProperty] private string sv1pw_content;
        [ObservableProperty] private string sv2pw_content;
        [ObservableProperty] private string robAutoColor;
        [ObservableProperty] private string robMotorColor;
        [ObservableProperty] private string robTaskRunColor;
        [ObservableProperty] private string robBatteryColor;
        [ObservableProperty] private string workMode;
        [ObservableProperty] private string isSelectedWorkMode;
        [ObservableProperty] private bool isWorkModeSelected;
        [ObservableProperty] private bool isWorkModeOpen;
        [ObservableProperty] private string isWorkModeTextColor;
        [ObservableProperty] private string loadSkipColor;
        [ObservableProperty] private string imgCollectColor;
        [ObservableProperty] private string tCounting;
        [ObservableProperty] private bool pressIOEnable;
        [ObservableProperty] private string isExtractCount;
        [ObservableProperty] private string isRobBlowSpeed;
        [ObservableProperty] private string isRobBlow1stTopCount;
        [ObservableProperty] private string isRobBlow1stBotCount;
        [ObservableProperty] private string isRobBlow2ndTopCount;
        [ObservableProperty] private string isRobBlow2ndBotCount;
        [ObservableProperty] private string isRobBlowPortCount;
        [ObservableProperty] private string isRobSpraySpeed;
        [ObservableProperty] private string isRobSprayTopCount;
        [ObservableProperty] private string isRobSprayPortTime;
        [ObservableProperty] private string isRobPortSpeed;
        [ObservableProperty] private string isRobPortCount;
        [ObservableProperty] private int isWorkIndex;
        [ObservableProperty] private string display_StackedDeviceCount;
        [ObservableProperty] private string display_UnStackedDeviceCount;
        [ObservableProperty] private string display_StackedShotCount;
        [ObservableProperty] private string display_UnStackedShotCount;
        [ObservableProperty] private bool robotSetting;
        [ObservableProperty] private string emc_CylOpenDelay;
        [ObservableProperty] private string emc_CylCloseDelay;
        [ObservableProperty] private string emc_StopperFWDDelay;
        [ObservableProperty] private string emc_StopperBWDDelay;
        [ObservableProperty] private string dataName;
        [ObservableProperty] private string dataComment;
        [ObservableProperty] private string heaterBtn1BG;
        [ObservableProperty] private string heaterBtn2BG;
        [ObservableProperty] private string magazineElevPos;
        [ObservableProperty] private string frameLoad1Color;
        [ObservableProperty] private string frameLoad2Color;
        [ObservableProperty] private string frameLoad3Color;
        [ObservableProperty] private string frameLoad4Color;
        [ObservableProperty] private string visionReadyColor;
        [ObservableProperty] private string preheaterReadyColor;
        [ObservableProperty] private string heaterReadyColor;
        [ObservableProperty] private string pressReadyColor;

        #endregion

        #region Server Properties
        public string Prefix { get { return m_XmlParser.SavedData.Prefix; } set { if (m_XmlParser.SavedData.Prefix != value) { m_XmlParser.SavedData.Prefix = value; OnPropertyChanged(); } } }
        public string Suffix { get { return m_XmlParser.SavedData.Suffix; } set { if (m_XmlParser.SavedData.Suffix != value) { m_XmlParser.SavedData.Suffix = value; OnPropertyChanged(); } } }

        public string[] ServerItems { get; set; }
        public int ClientPort { get { return m_XmlParser.SavedData.ClientPort; } set { if (m_XmlParser.SavedData.ClientPort != value) { m_XmlParser.SavedData.ClientPort = value; OnPropertyChanged(); } } }
        public string ClientIP { get { return m_XmlParser.SavedData.ClientIP; } set { if (m_XmlParser.SavedData.ClientIP != value) { m_XmlParser.SavedData.ClientIP = value; OnPropertyChanged(); } } }
        public bool IsServerOpened { get { return m_IsServerOpened; } set { if (m_IsServerOpened != value) { m_IsServerOpened = value; OnPropertyChanged(); } } }
        public bool ServerConnection { get { return m_ServerConnection; } set { if (m_ServerConnection != value) { m_ServerConnection = value; OnPropertyChanged(); } } }
        public string ServerOutput { get { return m_ServerOutput; } set { if (m_ServerOutput != value) { m_ServerOutput = value; OnPropertyChanged(); } } }
        public string PlcIP { get { return m_XmlParser.SavedData.PlcIp; } set { if (m_XmlParser.SavedData.PlcIp != value) { m_XmlParser.SavedData.PlcIp = value; OnPropertyChanged(); } } }
        public int PlcPort { get { return m_XmlParser.SavedData.PlcPort; } set { if (m_XmlParser.SavedData.PlcPort != value) { m_XmlParser.SavedData.PlcPort = value; OnPropertyChanged(); } } }
        #endregion

        #region Setting Data Properties
        public int Dev1StackedShotCount { get { return m_XmlParser.SavedData.dev1StackedShotCount; } set { if (m_XmlParser.SavedData.dev1StackedShotCount != value) { m_XmlParser.SavedData.dev1StackedShotCount = value; OnPropertyChanged(); } } }
        public int Dev1StackedDeviceCount { get { return m_XmlParser.SavedData.dev1StackedDeviceCount; } set { if (m_XmlParser.SavedData.dev1StackedDeviceCount != value) { m_XmlParser.SavedData.dev1StackedDeviceCount = value; OnPropertyChanged(); } } }
        public int Dev1UnStackedShotCount { get { return m_XmlParser.SavedData.dev1UnstackedShotCount; } set { if (m_XmlParser.SavedData.dev1UnstackedShotCount != value) { m_XmlParser.SavedData.dev1UnstackedShotCount = value; OnPropertyChanged(); } } }
        public int Dev1UnStackedDeviceCount { get { return m_XmlParser.SavedData.dev1UnstackedDeviceCount; } set { if (m_XmlParser.SavedData.dev1UnstackedDeviceCount != value) { m_XmlParser.SavedData.dev1UnstackedDeviceCount = value; OnPropertyChanged(); } } }
        public int Dev2StackedShotCount { get { return m_XmlParser.SavedData.dev2StackedShotCount; } set { if (m_XmlParser.SavedData.dev2StackedShotCount != value) { m_XmlParser.SavedData.dev2StackedShotCount = value; OnPropertyChanged(); } } }
        public int Dev2StackedDeviceCount { get { return m_XmlParser.SavedData.dev2StackedDeviceCount; } set { if (m_XmlParser.SavedData.dev2StackedDeviceCount != value) { m_XmlParser.SavedData.dev2StackedDeviceCount = value; OnPropertyChanged(); } } }
        public int Dev2UnStackedShotCount { get { return m_XmlParser.SavedData.dev2UnstackedShotCount; } set { if (m_XmlParser.SavedData.dev2UnstackedShotCount != value) { m_XmlParser.SavedData.dev2UnstackedShotCount = value; OnPropertyChanged(); } } }
        public int Dev2UnStackedDeviceCount { get { return m_XmlParser.SavedData.dev2UnstackedDeviceCount; } set { if (m_XmlParser.SavedData.dev2UnstackedDeviceCount != value) { m_XmlParser.SavedData.dev2UnstackedDeviceCount = value; OnPropertyChanged(); } } }
        public int Dev3StackedShotCount { get { return m_XmlParser.SavedData.dev3StackedShotCount; } set { if (m_XmlParser.SavedData.dev3StackedShotCount != value) { m_XmlParser.SavedData.dev3StackedShotCount = value; OnPropertyChanged(); } } }
        public int Dev3StackedDeviceCount { get { return m_XmlParser.SavedData.dev3StackedDeviceCount; } set { if (m_XmlParser.SavedData.dev3StackedDeviceCount != value) { m_XmlParser.SavedData.dev3StackedDeviceCount = value; OnPropertyChanged(); } } }
        public int Dev3UnStackedShotCount { get { return m_XmlParser.SavedData.dev3UnstackedShotCount; } set { if (m_XmlParser.SavedData.dev3UnstackedShotCount != value) { m_XmlParser.SavedData.dev3UnstackedShotCount = value; OnPropertyChanged(); } } }
        public int Dev3UnStackedDeviceCount { get { return m_XmlParser.SavedData.dev3UnstackedDeviceCount; } set { if (m_XmlParser.SavedData.dev3UnstackedDeviceCount != value) { m_XmlParser.SavedData.dev3UnstackedDeviceCount = value; OnPropertyChanged(); } } }
        public int StartBtnDelay { get { return m_XmlParser.SavedData.startBtnDelay; } set { if (m_XmlParser.SavedData.startBtnDelay != value) { m_XmlParser.SavedData.startBtnDelay = value; OnPropertyChanged(); } } }
        public int HomeBtnDelay { get { return m_XmlParser.SavedData.homeBtnDelay; } set { if (m_XmlParser.SavedData.homeBtnDelay != value) { m_XmlParser.SavedData.homeBtnDelay = value; OnPropertyChanged(); } } }
        public int CountResetBtnDelay { get { return m_XmlParser.SavedData.countResetBtnDelay; } set { if (m_XmlParser.SavedData.countResetBtnDelay != value) { m_XmlParser.SavedData.countResetBtnDelay = value; OnPropertyChanged(); } } }
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
            PLCDataClick = new RelayCommand<string>(ExecutePLCDataClick);

            m_XmlParser = new Utilities.XmlParser();

            IsServerOpened = false;
            ServerConnection = false;
            TabVisible = false;

            _dataProcessor = new PlcDataProcessor(this);
            _plc = new McProtocolService();

            robOverrideLimit = new int[2];
            robSpeedMaxLimit = new int[2];
            robPCBCountLimit = new int[2];
            robExtractCountLimit = new int[2];
            robOverrideLimit[0] = 0; robOverrideLimit[1] = 100;
            robSpeedMaxLimit[0] = 0; robSpeedMaxLimit[1] = 7500;
            robPCBCountLimit[0] = 1; robPCBCountLimit[1] = 20;
            robExtractCountLimit[0] = 1; robExtractCountLimit[1] = 10;


            IsRunning = false;
            IsStopped = false;
            IsReady = false;
            AutoBtn = true;
            IsAuto = true;
            IsManual = false;
            IsSetting = false;
            IsError = false;
            IsSetView = false;
            HomeButton = false;
            BtnWorkWait = true;
            ResetbtnEnable = true;
            IsWorkModeSelected = false;
            LoginCheck = false;
            needOrigin = false;
            IsRobSpeed_Percent = "";
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

            StartButtonColor = "LimeGreen";
            IsWorkModeTextColor = "Black";
            ResetBtnOpacity = 1;

            RobAutoColor = "Gray";
            RobMotorColor = "Gray";
            RobTaskRunColor = "Gray";
            RobBatteryColor = "Gray";

            HeaterBtn1BG = "LightGray";
            HeaterBtn2BG = "LightGray";

            FrameLoad1Color = "LightGray";
            FrameLoad2Color = "LightGray";
            FrameLoad3Color = "LightGray";
            FrameLoad4Color = "LightGray";

            MagazineElevPos = "연결대기";

            Btn1Checked = false;
            Btn2Checked = false;
            Btn3Checked = false;
            Btn4Checked = false;
            Btn5Checked = false;
            Btn6Checked = false;

            IsServoBtnPressed = false;
            #endregion

            // Server Service Initialization
            _tcpService = new TcpServerService();
            _tcpService.OnDataReceived += OnDataReceivedFromService;
            _tcpService.OnConnectionStatusChanged += (isConnected) =>
            {
                ServerConnection = isConnected; // UI 바인딩 변수 업데이트
            };
            _tcpService.OnErrorOccurred += (errMsg) =>
            {
                RaiseMessage(errMsg);
            };

            // Parser Initialization
            _parser = new RobotMessageParser();
            InitializeCommandMap();

            #region Timer Setting
            timer = new DispatcherTimer(DispatcherPriority.Send, System.Windows.Application.Current.Dispatcher);
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(TimerCount);

            DeviceTimer = new DispatcherTimer(DispatcherPriority.Send, System.Windows.Application.Current.Dispatcher);
            DeviceTimer.Interval = TimeSpan.FromMilliseconds(383);
            DeviceTimer.Tick += new EventHandler(DeviceSend);

            responseTimer = new DispatcherTimer(DispatcherPriority.Send, System.Windows.Application.Current.Dispatcher);
            responseTimer.Interval = TimeSpan.FromMilliseconds(200);
            responseTimer.Tick += new EventHandler(ConnectionOK);

            plcTimer = new DispatcherTimer(DispatcherPriority.Send, System.Windows.Application.Current.Dispatcher);
            plcTimer.Interval = TimeSpan.FromMilliseconds(100);
            plcTimer.Tick += new EventHandler(PLCRead);


            #endregion

        }


        internal async Task OpeningAsync(IProgress<LoadStep> progress = null)
        {
            MainWindow _window = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (_window == null) return;

            progress?.Report(new LoadStep("화면을 구성하는 중...", 10));

            _window.ResizeMode = ResizeMode.NoResize;
            _window.WindowState = WindowState.Maximized;
            _window.WindowStyle = WindowStyle.None;
            WindowPage = "Pages/Main_Page.xaml";

            ServerOpen(progress);

            progress?.Report(new LoadStep("작업 모드 목록을 구성하는 중...", 90));
            LoadWorkModeList();
            IsWorkIndex = -1;
            HomeButton = true;

            progress?.Report(new LoadStep("준비 완료", 100));

            await Task.CompletedTask;   // 시그니처 유지용
        }

        private void LoadWorkModeList()
        {
            var d = m_XmlParser.SavedData;
            WorkModeList.Clear();
            WorkModeList.Add("정지모드"); // 0
            WorkModeList.Add(d.SavedDeviceName1);
            WorkModeList.Add(d.SavedDeviceName2);
            WorkModeList.Add(d.SavedDeviceName3);
            WorkModeList.Add(d.SavedDeviceName4);
            WorkModeList.Add(d.SavedDeviceName5);
            WorkModeList.Add(d.SavedDeviceName6);
            WorkModeList.Add(d.SavedDeviceName7);
            WorkModeList.Add(d.SavedDeviceName8);
            WorkModeList.Add(d.SavedDeviceName9);
            WorkModeList.Add(d.SavedDeviceName10);
        }



        internal void Closing()
        {
            MessageBoxResult mBoxRst = MessageBox.Show("프로그램을 종료 하시겠습니까?", "Program Message", MessageBoxButton.YesNo);
            if (mBoxRst == MessageBoxResult.Yes)
            {
                timer.Stop();
                DeviceTimer.Stop();
                responseTimer.Stop();
                plcTimer.Stop();

                m_XmlParser.SavedDataSave();
                _tcpService.StopServer();


                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }


        #region Function Methods
        private void ExecutePLCDataClick(string para)
        {
            PLCDataComments _comment = new PLCDataComments(this);
            Window PlcCommentWindow = new PLCDataCommentWindow();
            Window openInterfaceWindow = Application.Current.Windows.OfType<InterfaceWindow>().FirstOrDefault();

            PlcCommentWindow.ResizeMode = ResizeMode.NoResize;
            PlcCommentWindow.Topmost = true;

            if (openInterfaceWindow != null)
            {
                PlcCommentWindow.Owner = openInterfaceWindow;
                PlcCommentWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                PlcCommentWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            PlcCommentWindow.DataContext = this;
            DataName = para;
            DataComment = _comment.Execute(para);

            PlcCommentWindow.ShowDialog();
        }

        public string CallNumKey(string originalData) // 숫자키보드 출력
        {
            try
            {
                MainWindow mWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (IsSetView) return originalData;
                var KeyWindow = new NumberKeyboard();
                var MainWithSize = mWindow.Width;
                var MainHeightSize = mWindow.Height;
                var KeyWithSize = KeyWindow.Width;
                var keyHeightSize = KeyWindow.Height;

                // 키 윈도우 출력 위치 설정
                Point mPoint = Mouse.GetPosition(mWindow);
                // 위치가 메인윈도우 크기보다 작을때는 마우스 포인터 기준으로, 클때는 메인윈도우 크기 기준으로 설정
                if (mPoint.X < MainWithSize - KeyWithSize)
                    KeyWindow.Left = mPoint.X - (KeyWindow.ActualWidth);
                else KeyWindow.Left = MainWithSize - (KeyWithSize * 1.2);  // 아예 벽에 붙어버리면 안 되니 1.2배수 적용

                if (mPoint.Y < MainHeightSize - keyHeightSize)
                    KeyWindow.Top = mPoint.Y - (KeyWindow.ActualHeight);
                else KeyWindow.Top = MainHeightSize - (keyHeightSize * 1.1); // 여기도 left와 동일 이유로 1.1 배수 적용 ( 세로 폭이 더 길으니까 1.1 )
                KeyWindow.ResizeMode = ResizeMode.NoResize;
                KeyWindow.Topmost = true;

                WeakReferenceMessenger.Default.Send(new DisplayDataSender(originalData)); // 기존 데이터 전송
                WeakReferenceMessenger.Default.Unregister<KeyboardDataSender>(this);  // 키보드 데이터 수신 대기
                WeakReferenceMessenger.Default.Register<KeyboardDataSender>(this, (r, m) => { KeyboardData = m.Value; }); // 키보드 데이터 수신

                KeyWindow.ShowDialog();

                return KeyboardData != "" ? KeyboardData : originalData; //키보드 데이터가 없으면("") 기존값, 있으면 키보드 데이터 반환
            }
            catch (Exception ex)
            {
                RaiseMessage("숫자키보드 실행 중 오류가 발생했습니다.\r" + ex.Message);
            }
            return originalData;
        }



        public void CallLogin() // 로그인 창 출력
        {
            MainWindow mWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            var LoginWindow = new LoginWindow();
            var MainWithSize = mWindow.Width;
            var MainHeightSize = mWindow.Height;

            LoginWindow.ResizeMode = ResizeMode.NoResize;
            LoginWindow.Topmost = true;
            LoginWindow.Left = MainWithSize / 3;
            LoginWindow.Top = MainHeightSize / 3;

            WeakReferenceMessenger.Default.Unregister<LoginBool>(this);
            WeakReferenceMessenger.Default.Register<LoginBool>(this, (r, m) => { LoginCheck = m.Value; });
            LoginWindow.ShowDialog();
        }



        internal void ButtonVisible(string select)
        {
            IsSetting = false;
            switch (select)
            {
                case "Auto":
                    WindowPage = "Pages/Main_Page.xaml";
                    PLCWrite(203, 1, 0);
                    IsAuto = true;
                    IsManual = false;
                    IsSetView = false;
                    HomeButton = true;
                    AutoBtn = true;
                    ManualBtn = false;
                    break;
                case "Manual":
                    WindowPage = "Pages/Manual_Page.xaml";
                    PLCWrite(203, 0, 0);
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



        public void RaisePlcError(short code) => ProcessError("PLC_" + code, _errorRepo.GetPlcError(code));
        public void RaiseMessage(string text) => ProcessError("MSG_" + text, new ErrorInfo("", text));


        private void ProcessError(string key, ErrorInfo errorData)
        {
            if (_activeErrors.ContainsKey(key)) return;   // 이미 표시중이면 무시

            _activeErrors[key] = errorData;
            ButtonVisible("Error");
            ErrorList.Add(errorData);  // UI갱신
            Logger.Write("!! Error !! : " + errorData.Message);
        }

        /// <summary>PLC 비트 해제 시 해당 에러 제거</summary>
        public void ClearPlcError(short code)
        {
            string key = "PLC_" + code;
            if (!_activeErrors.TryGetValue(key, out var info)) return;

            _activeErrors.Remove(key);
            ErrorList.Remove(info);
            ButtonVisible("Auto");
            Logger.Write("Error Cleared : " + info.Message);
        }

        public void LabelSet(string label)
        {
            IsOrigin = IsReady = IsRunning = IsStopped = false;
            switch (label)
            {
                case "Origin":
                    IsOrigin = true;
                    break;
                case "Ready":
                    IsReady = true;
                    ErrorList.Clear();
                    _activeErrors.Clear();
                    break;
                case "Running":
                    IsRunning = true;
                    break;
                case "Stopped":
                    IsStopped = true;
                    break;
                default:

                    break;
            }
        }

        private static async Task<bool> WaitUntilAsync(Func<bool> condition, TimeSpan timeout, int intervalMs = 50, CancellationToken ct = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(timeout);
            try
            {
                while (!condition())
                    await Task.Delay(intervalMs, cts.Token);
                return true;
            }
            catch (OperationCanceledException) { return false; }
        }

        #endregion


        #region Server Methods

        /// <summary>서버 열기 — PLC 연결 완료를 기다리지 않음</summary>
        internal void ServerOpen(IProgress<LoadStep> progress = null)
        {
            if (IsServerOpened) return;

            try
            {
                progress?.Report(new LoadStep($"로봇 서버를 여는 중... ({ClientIP}:{ClientPort})", 40));
                _tcpService.StartServer(ClientIP, ClientPort);
                IsServerOpened = true;
            }
            catch (Exception ex)
            {
                IsServerOpened = false;
                Logger.Write("서버 시작 실패 : " + ex.Message);
                progress?.Report(new LoadStep("서버 시작에 실패했습니다. 설정을 확인해 주십시오.", 40));
                return;   // 예외를 던지지 않음 — 프로그램은 계속 기동
            }

            responseTimer.Start();

            _retryTick = 20;    // 첫 tick에서 곧바로 연결 시도하도록
            plcTimer.Start();

            progress?.Report(new LoadStep("PLC 연결을 백그라운드에서 시도합니다.", 70));
        }

        /// <summary>서버 닫기</summary>
        internal void ServerClose()
        {
            if (!IsServerOpened) return;

            plcTimer.Stop();
            responseTimer.Stop();

            _tcpService.StopServer();
            _plc.Disconnect();

            IsPlcConnected = false;
            IsServerOpened = false;
            _isPlcConnecting = false;
            _retryTick = 0;

            LabelSet("");
            Logger.Write("서버 종료");
        }

        /// <summary>UI 버튼용 토글</summary>
        internal void ServerToggle()
        {
            if (IsServerOpened) ServerClose();
            else ServerOpen();
        }

        private async Task ConnectPlcAsync()
        {
            if (_isPlcConnecting) return;
            _isPlcConnecting = true;

            try
            {
                IsPlcConnected = await _plc.ConnectAsync(m_XmlParser.SavedData.PlcIp, m_XmlParser.SavedData.PlcPort);
            }
            catch (Exception ex)
            {
                IsPlcConnected = false;
                Logger.Write("PLC 연결 예외 : " + ex.Message);
            }
            finally
            {
                _isPlcConnecting = false;
            }

            if (IsPlcConnected)
            {
                await SendAllSettingsToPlcAsync();
                Logger.Write("PLC 연결 완료, 설정값 전송");
            }
        }

        private async Task SendAllSettingsToPlcAsync()
        {
            var s = SettingsStore.Current;
            PLCWrite(299, 1, 0);

            PLCWriteDword(230, s.M1_PickUpPos);
            PLCWriteDword(232, s.M1_FirstPos);
            PLCWriteDword(234, s.M1_SecondPos);
            PLCWriteDword(236, s.M1_CheckPos);
            PLCWriteDword(238, s.M1_HighSpeed);
            PLCWriteDword(240, s.M1_LowSpeed);

            PLCWriteDword(242, s.M2_FirstPos);
            PLCWriteDword(244, s.M2_Pitch);
            PLCWriteDword(246, s.M2_Speed);

            PLCWrite(248, s.MoveCyl1DownDelay, 0);
            PLCWrite(249, s.MoveCyl2DownDelay, 0);
            PLCWrite(250, s.MoveCyl1UpDelay, 0);
            PLCWrite(251, s.MoveCyl2UpDelay, 0);
            PLCWrite(252, s.FixCylUpDelay, 0);
            PLCWrite(253, s.FixCylDownDelay, 0);

            PLCWrite(254, s.EMCOpenDelay, 0);
            PLCWrite(255, s.EMCCloseDelay, 0);
            PLCWrite(256, s.EMCFWDDelay, 0);
            PLCWrite(257, s.EMCBWDDelay, 0);

            if (PlcInput[0] != 5)
            {
                ButtonVisible("Auto");
            }
        }


        private void OnDataReceivedFromService(string msg) // 데이터 수신
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ParsingRobMsg(msg);
            });
        }


        internal async void ServerSend(string msg) // 데이터 송신
        {
            if (string.IsNullOrEmpty(msg)) return;

            string fullMsg = (Prefix ?? "") + msg + (Suffix ?? ""); // 접두어와 접미어 부착 ( 없다면 생략 )

            await _tcpService.SendAsync(fullMsg);
        }

        private void InitializeCommandMap()
        {
            _commandMap = new Dictionary<string, Action<string>>
            {
                { "State",   RobState },                             // State_ 처리
                { "Speed",   (msg) => SplitAndCall(msg, RobSpeed) }, // Speed_는 내부에서 또 쪼개짐
                { "Count",   (msg) => SplitAndCall(msg, RobCount) }, // Count_ 처리
                { "RobIO",   RobIO },                                // RobIO_ 처리
            };
        }

        // [보조] Speed, Count 처럼 내부에서 "Key_Value"로 한 번 더 나뉘는 경우 처리
        private void SplitAndCall(string message, Action<string, string> targetMethod)
        {
            if (string.IsNullOrEmpty(message)) return;

            int sepIndex = message.IndexOf('_');
            if (sepIndex > 0)
            {
                string subKey = message.Substring(0, sepIndex);
                string subValue = message.Substring(sepIndex + 1);
                targetMethod(subKey, subValue);
            }
        }

        internal void ParsingRobMsg(string msg)
        {
            try
            {
                // 파서를 통해 분리된 데이터 수신
                foreach (var parsedData in _parser.Parse(msg))
                {
                    // 딕셔너리에 등록된 키인지 확인하고 해당 메서드 실행
                    if (_commandMap.ContainsKey(parsedData.Key))
                    {
                        _commandMap[parsedData.Key]?.Invoke(parsedData.Value);
                    }
                    else
                    {
                        // 딕셔너리에 등록 안된 키는 로그 기록
                        Logger.Write($"Unknown Command: {parsedData.Key}");
                    }
                }
            }
            catch (Exception ex)
            {
                RaiseMessage(ex.Message); // 예외 처리 유지
            }
        }


        private void ConnectionOK(object sender, EventArgs e)
        {
            ServerSend("ConnectionOK");  // 통신 상태 확인용

            PLCWrite(299, 1, 0); // PLC 통신 상태 확인용 D299번지 1로 설정
        }


        internal void ServerClear()
        {
            ServerOutput = "";
        }

        private async void PLCRead(object sender, EventArgs e)
        {
            if (_isPlcBusy) return;
            _isPlcBusy = true;
            try
            {
                if (!IsPlcConnected)
                {
                    if (++_retryTick < 20) return;    // 100ms × 20 = 2초 간격 재시도
                    _retryTick = 0;
                    _plc.Disconnect();                // 죽은 소켓 정리 후 재연결
                    await ConnectPlcAsync();
                    return;
                }

                byte[] readData = await _plc.ReadDeviceAsync("D", 100, 200);
                if (readData != null && readData.Length >= 400) PLCParser(readData);
                else MarkDisconnected("응답 데이터 길이 이상");
            }
            catch (Exception ex)
            {
                MarkDisconnected(ex.Message);
            }
            finally { _isPlcBusy = false; }
        }

        private void MarkDisconnected(string reason)
        {
            if (!IsPlcConnected) return;   // 중복 알림 방지
            IsPlcConnected = false;
            _retryTick = 0;
            Logger.Write("PLC 통신 단절 : " + reason);
            RaiseMessage("PLC 통신이 끊어졌습니다. 재연결을 시도합니다.");
        }

        private void PLCParser(byte[] data)
        {
            bool isInputChange = false;
            bool isOutputChange = false;

            // 데이터 파싱
            for (int i = 0; i < 100; i++) // Input D100 ~ D199
            {
                short newValue = BitConverter.ToInt16(data, i * 2);

                if (PlcInput[i] != newValue)
                {
                    PlcInput[i] = newValue;
                    isInputChange = true;

                    _dataProcessor.Execute(i, newValue); // 파싱데이터 바로 처리
                }
            }

            _dataProcessor.ExecuteDword(data);

            for (int i = 100; i < 200; i++) // Output D200 ~ D299
            {
                short newValue = BitConverter.ToInt16(data, i * 2);
                int outIndex = i - 100;

                if (PlcOutput[outIndex] != newValue)
                {
                    PlcOutput[outIndex] = newValue;
                    isOutputChange = true;
                }
            }

        }


        /// <summary>
        /// PLC D 데이터 송신
        /// </summary>
        /// <param name="Address">유효범위 : 200 ~ 299 ( D200 ~ D299 )</param>
        /// <param name="inputData"></param>
        public async void PLCWrite(int Address, short inputData, int pulse)
        {
            if (Address < 200 || Address >= 300) return; // 유효한 주소 범위 체크 ( D200 ~ D299 )
            if (_plc != null && _plc.IsConnected)
            {
                short[] data = new short[] { inputData };

                bool isSuccess = await _plc.WriteDeviceAsync("D", Address, data);  // PLC에 데이터 쓰기

                if (isSuccess)
                {
                    PlcOutput[Address - 200] = inputData;
                }
                if (pulse > 0) // 펄스 신호인 경우 일정 시간 후 자동으로 0으로 리셋
                {
                    await Task.Delay(pulse);
                    short[] resetData = new short[] { 0 };
                    await _plc.WriteDeviceAsync("D", Address, resetData);
                    PlcOutput[Address - 200] = 0;
                }

            }
        }

        public async void PLCWriteDword(int address, int inputData)
        {
            // D200 ~ D298 (2워드를 쓰므로 상위 주소까지 범위 내여야 함)
            if (address < 200 || address + 1 >= 300) return;
            if (_plc == null || !_plc.IsConnected) return;

            short[] data =
            {
                unchecked((short)(inputData & 0xFFFF)),           // 하위 워드
                unchecked((short)((inputData >> 16) & 0xFFFF))    // 상위 워드
            };

            bool isSuccess = await _plc.WriteDeviceAsync("D", address, data);   // 한 명령으로 2워드

            if (isSuccess)
            {
                PlcOutput[address - 200] = data[0];
                PlcOutput[address - 200 + 1] = data[1];
            }
        }

        #endregion



        #region Server Message 
        private void RobState(string message)
        {
            switch (message)
            {
                case "ShotCycleEnd":
                    Logger.Write("System_Cycle_End");
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
                        if (message != IsRobSpeed_Percent) Logger.Write("Override Change : " + IsRobSpeed_Percent + " → " + message);
                        IsRobSpeed_Percent = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Max":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobSpeed_MMS) Logger.Write("Max Speed Change : " + IsRobSpeed_MMS + " → " + message);
                        IsRobSpeed_MMS = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Blow":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobBlowSpeed) Logger.Write("Blow Speed Change : " + IsRobBlowSpeed + " → " + message);
                        IsRobBlowSpeed = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Spray":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobSpraySpeed) Logger.Write("Spray Speed Change : " + IsRobSpraySpeed + " → " + message);
                        IsRobSpraySpeed = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Port":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobPortSpeed) Logger.Write("Port Speed Change : " + IsRobPortSpeed + " → " + message);
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
                            Logger.Write(LogMsg);
                        }
                        else if (message != IsPCBCount && IsPCBCount != "Empty")
                        {
                            string LogMsg = (a < 21) ? "PCB Count Change : " + IsPCBCount + " → " + message : "PCB Count Change : " + IsPCBCount + " → " + "Empty";
                            Logger.Write(LogMsg);
                        }
                        IsPCBCount = (a < 21) ? message : "Empty";
                    }
                    catch (Exception) { return; }
                    break;
                case "Blow1stTop":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobBlow1stTopCount) Logger.Write("Blow1stTop Count Change : " + IsRobBlow1stTopCount + " → " + message);
                        IsRobBlow1stTopCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Blow1stBot":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobBlow1stBotCount) Logger.Write("Blow1stBot Count Change : " + IsRobBlow1stBotCount + " → " + message);
                        IsRobBlow1stBotCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Blow2ndTop":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobBlow2ndTopCount) Logger.Write("Blow2ndTop Count Change : " + IsRobBlow2ndTopCount + " → " + message);
                        IsRobBlow2ndTopCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Blow2ndBot":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobBlow2ndBotCount) Logger.Write("Blow2ndBot Count Change : " + IsRobBlow2ndBotCount + " → " + message);
                        IsRobBlow2ndBotCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "BlowPort":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobBlowPortCount) Logger.Write("BlowPort Count Change : " + IsRobBlowPortCount + " → " + message);
                        IsRobBlowPortCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "SprayTop":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobSprayTopCount) Logger.Write("SprayTop Count Change : " + IsRobSprayTopCount + " → " + message);
                        IsRobSprayTopCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "SprayPort": // 포트 분사 딜레이 타임
                    try
                    {
                        double a = Convert.ToDouble(message);
                        if (message != IsRobSprayPortTime) Logger.Write("SprayPort Delay Time Change : " + IsRobSprayPortTime + " → " + message);
                        IsRobSprayPortTime = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "Extract":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsExtractCount)
                        {
                            if (a < 11)
                            {
                                Logger.Write("Extract Count Change : " + IsExtractCount + " → " + message);
                                IsExtractCount = message;
                            }
                            else
                            {
                                Logger.Write("Extract Count Change : " + IsExtractCount + " → Full");
                                IsExtractCount = "Full";
                            }

                        }
                    }
                    catch (Exception) { return; }
                    break;
                case "Port":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsRobPortCount) Logger.Write("Port Count Change : " + IsRobPortCount + " → " + message);
                        IsRobPortCount = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "PressCloseTime":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsPressCloseTime) Logger.Write("Press Close Signal Time Change : " + IsPressCloseTime + " → " + message);
                        IsPressCloseTime = message;
                    }
                    catch (Exception) { return; }
                    break;
                case "ChamberCloseTime":
                    try
                    {
                        int a = Convert.ToInt32(message);
                        if (message != IsChamberCloseTime) Logger.Write("Chamber Close Signal Time Change : " + IsChamberCloseTime + " → " + message);
                        IsChamberCloseTime = message;
                    }
                    catch (Exception) { return; }
                    break;
                default:
                    break;
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
                        Logger.Write("Robot fatal error ! : Battery Low !!");
                        RobBatteryColor = "Red";
                    }
                    break;
                case "BatteryHigh": RobBatteryColor = "Gray"; break;
                default:
                    break;
            }
        }
        #endregion


        #region PLC Data Process Methods

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
                        if (TimerStack > CountResetBtnDelay)
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
                        if (TimerStack > CountResetBtnDelay)
                        {
                            ServerSend("RobPCBCount_1");
                            timer.Stop();
                            TimerStack = 0;
                        }
                        break;

                    case "ExtractCountReset":  // 배출 작업 완료 수량 초기화 버튼 동작 딜레이
                        if (TimerStack > CountResetBtnDelay)
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
                            PLCWrite(202, 1, 100);
                            timer.Stop();
                            TimerStack = 0;
                        }
                        break;

                    case "RobotHome":  // 로봇 홈 위치 이동 버튼 동작 딜레이
                        if (TimerStack > HomeBtnDelay)
                        {
                            PLCWrite(204, 1, 100);
                            timer.Stop();
                            TimerStack = 0;
                        }
                        break;

                    case "Stop":  // 정지 할 때 까지 ( Max 3s ) 200ms 간격으로 정지 신호 전송
                        if (TimerStack % 2 == 0) PLCWrite(200, 1, 100);
                        if (IsStopped) timer.Stop();
                        if (TimerStack > 30) { timer.Stop(); TimerStack = 0; }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) { RaiseMessage(ex.Message); }
        }



        public void DeviceSend(object sender, EventArgs e)  // 주기적 (383 ms) 으로 디바이스 번호 전송
        {
            if (WorkMode == "" || WorkMode == null) return;
            ServerSend("WorkMode_" + IsWorkIndex);
        }
        #endregion





        #region MainWindow Button Action
        internal void StartButtonDown(MouseButtonEventArgs e)  // 설비 가동 신호 송신
        {
            if (!IsReady) return;
            if (!IsWorkModeSelected || WorkMode == "") { MessageBox.Show("작업 모드가 선택되지 않았습니다."); return; }
            if (IsWorkIndex == 0) { MessageBox.Show("정지모드가 선택되어 있습니다."); return; }
            if (needOrigin) { RaiseMessage("작업모드 변경 후 HOME 동작 미완료"); return; }
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
            colorCount = 0;
            PLCWrite(200, 1, 100);
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



        internal async Task ResetButtonPress()
        {
            if (!IsStopped) { return; }

            PLCWrite(201, 1, 100);

            bool ok = await WaitUntilAsync(() => PlcInput[0] != 5, TimeSpan.FromSeconds(4));
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
            if (IsStopped)
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



        internal void Heater1Press()
        {
            if (IsStopped)
            {
                if (PlcInput[10] == 1) // 히터 1이 켜져 있으면
                {
                    MessageBoxResult result = MessageBox.Show("히터 1을 끄시겠습니까?", "", MessageBoxButton.YesNo);  // 끌지 재 확인 하고
                    if (result == MessageBoxResult.Yes)
                    {
                        PLCWrite(210, 0, 0);  // 히터 1 OFF ( Input 10 : Heater 1, Out210 : 히터 1 제어 )
                    }
                }
                else PLCWrite(210, 1, 0);     // 아니면 ON
            }
            else return;
        }



        internal void Heater2Press()
        {
            if (IsStopped)
            {
                if (PlcInput[11] == 1) // 히터 2가 켜져 있으면
                {
                    MessageBoxResult result = MessageBox.Show("히터 2를 끄시겠습니까?", "", MessageBoxButton.YesNo); // 끌래?
                    if (result == MessageBoxResult.Yes)
                    {
                        PLCWrite(211, 0, 0);   // 히터 2 OFF ( Input 11 : Heater 2, Out211 : 히터 2 제어 )
                    }
                }
                else PLCWrite(211, 1, 0);      // 아니면 ON
            }
            else return;
        }

        internal void MGZPositionUpdate()
        {
            try
            {
                DisplayedData = MagazineElevPos;
                int inputData = Convert.ToInt32(CallNumKey(DisplayedData));
                if (inputData < 0 || inputData > 40) { RaiseMessage("입력값 범위 초과"); return; }
                else
                {
                    PLCWrite(212, (short)inputData, 0); // 매거진 위치값 전송 후
                    PLCWrite(213, 1, 100);              // 매거진 위치 업데이트 신호 송신 (100ms 펄스)
                }
            }
            catch
            {
                RaiseMessage("입력값이 잘못되었습니다.");
            }
        }



        internal void WorkModeOpen()  // 디바이스 선택 드롭다운 열릴 때 버퍼에 현재 디바이스 저장
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
                    Logger.Write("WorkMode Change : " + WorkMode);
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
            MainWindow mWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            DeviceWindow deviceWindow = new DeviceWindow();
            var MainWithSize = mWindow.Width;
            var MainHeightSize = mWindow.Height;
            var SettingWithSize = deviceWindow.Width;
            var SettingHeightSize = deviceWindow.Height;

            deviceWindow.ResizeMode = ResizeMode.NoResize;
            deviceWindow.Topmost = true;
            deviceWindow.Left = (MainWithSize / 2) - (SettingWithSize / 2);
            deviceWindow.Top = (MainHeightSize / 2) - (SettingHeightSize / 2);

            deviceWindow.ShowDialog();

            m_XmlParser.SavedDataLoad();  // 설정 끝나면 저장된 데이터 로드

            // 워크 모드 리스트 갱신
            LoadWorkModeList();
        }


        internal void InterfaceView()
        {
            InterfaceWindow interfaceWindow = new InterfaceWindow();
            interfaceWindow.DataContext = this;
            Window? openInterfaceWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            if (openInterfaceWindow != null)
            {
                interfaceWindow.Owner = openInterfaceWindow;
                interfaceWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                interfaceWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            interfaceWindow.ResizeMode = ResizeMode.NoResize;
            interfaceWindow.ShowDialog();
        }


        internal void IPAddressBox_Robot()
        {
            DisplayedData = ClientIP;
            string MsgBuff = ClientIP;
            ClientIP = CallNumKey(DisplayedData);
            Logger.Write("Robot IP Change : " + MsgBuff + " → " + ClientIP);
        }



        internal void PortBox_Robot()
        {
            try
            {
                DisplayedData = ClientPort.ToString();
                string MsgBuff = ClientPort.ToString();
                ClientPort = Convert.ToInt32(CallNumKey(DisplayedData));
                Logger.Write("Robot Port Change : " + MsgBuff + " → " + ClientPort);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }



        internal void IPAddressBox_PLC()
        {
            DisplayedData = PlcIP;
            string MsgBuff = PlcIP;
            PlcIP = CallNumKey(DisplayedData);
            Logger.Write("PLC IP Change : " + MsgBuff + " → " + PlcIP);
        }



        internal void PortBox_PLC()
        {
            try
            {
                DisplayedData = PlcPort.ToString();
                string MsgBuff = PlcPort.ToString();
                PlcPort = Convert.ToInt32(CallNumKey(DisplayedData));
                Logger.Write("PLC Port Change : " + MsgBuff + " → " + PlcPort);
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
                Logger.Write("Start Button Delay Change : " + MsgBuff + " → " + StartBtnDelay);
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
                Logger.Write("Home Button Delay Change : " + MsgBuff + " → " + HomeBtnDelay);
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
                DisplayedData = CountResetBtnDelay.ToString();
                string MsgBuff = CountResetBtnDelay.ToString();
                CountResetBtnDelay = Convert.ToInt32(CallNumKey(DisplayedData));
                Logger.Write("Count Reset Button Delay Change : " + MsgBuff + " → " + CountResetBtnDelay);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }



        internal void RobSpeedSet_Percent()
        {
            string MsgBuff;
            DisplayedData = IsRobSpeed_Percent;
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
                RaiseMessage(e.Message);
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
                RaiseMessage(e.Message);
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

        // ============================== Auto Loader Setting ==============================
        internal void MotorParaSetPress()
        {
            MessageBoxResult result = MessageBox.Show("모터 파라미터를 설정 하시겠습니까?", "모터 파라미터 설정", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                PLCWrite(260, 1, 100);
                SettingsStore.Save();
            }
        }

        internal void MotorParaSetRelease()
        {
        }

        internal void M1_PickUpPosSet() { DisplayedData = Convert.ToString(SettingsStore.Current.M1_PickUpPos); PLCWriteDword(230, SettingsStore.Current.M1_PickUpPos = Convert.ToInt32(CallNumKey(DisplayedData))); }
        internal void M1_FirstPosSet() { DisplayedData = Convert.ToString(SettingsStore.Current.M1_FirstPos); PLCWriteDword(232, SettingsStore.Current.M1_FirstPos = Convert.ToInt32(CallNumKey(DisplayedData))); }
        internal void M1_SecondPosSet() { DisplayedData = Convert.ToString(SettingsStore.Current.M1_SecondPos); PLCWriteDword(234, SettingsStore.Current.M1_SecondPos = Convert.ToInt32(CallNumKey(DisplayedData))); }
        internal void M1_CheckPosSet() { DisplayedData = Convert.ToString(SettingsStore.Current.M1_CheckPos); PLCWriteDword(236, SettingsStore.Current.M1_CheckPos = Convert.ToInt32(CallNumKey(DisplayedData))); }
        internal void M1_HighSpeedSet() { DisplayedData = Convert.ToString(SettingsStore.Current.M1_HighSpeed); PLCWriteDword(238, SettingsStore.Current.M1_HighSpeed = Convert.ToInt32(CallNumKey(DisplayedData))); }
        internal void M1_LowSpeedSet() { DisplayedData = Convert.ToString(SettingsStore.Current.M1_LowSpeed); PLCWriteDword(240, SettingsStore.Current.M1_LowSpeed = Convert.ToInt32(CallNumKey(DisplayedData))); }

        internal void M2_1stPosSet() { DisplayedData = Convert.ToString(SettingsStore.Current.M2_FirstPos); PLCWriteDword(242, SettingsStore.Current.M2_FirstPos = Convert.ToInt32(CallNumKey(DisplayedData))); }
        internal void M2_PitchSet() { DisplayedData = Convert.ToString(SettingsStore.Current.M2_Pitch); PLCWriteDword(244, SettingsStore.Current.M2_Pitch = Convert.ToInt32(CallNumKey(DisplayedData))); }
        internal void M2_SpeedSet() { DisplayedData = Convert.ToString(SettingsStore.Current.M2_Speed); PLCWriteDword(246, SettingsStore.Current.M2_Speed = Convert.ToInt32(CallNumKey(DisplayedData))); }

        internal void MoveCyl1DownDelaySet() { DisplayedData = Convert.ToString(SettingsStore.Current.MoveCyl1DownDelay); PLCWrite(248, SettingsStore.Current.MoveCyl1DownDelay = Convert.ToInt16(CallNumKey(DisplayedData)), 0); }
        internal void MoveCyl2DownDelaySet() { DisplayedData = Convert.ToString(SettingsStore.Current.MoveCyl2DownDelay); PLCWrite(249, SettingsStore.Current.MoveCyl2DownDelay = Convert.ToInt16(CallNumKey(DisplayedData)), 0); }
        internal void MoveCyl1UpDelaySet() { DisplayedData = Convert.ToString(SettingsStore.Current.MoveCyl1UpDelay); PLCWrite(250, SettingsStore.Current.MoveCyl1UpDelay = Convert.ToInt16(CallNumKey(DisplayedData)), 0); }
        internal void MoveCyl2UpDelaySet() { DisplayedData = Convert.ToString(SettingsStore.Current.MoveCyl2UpDelay); PLCWrite(251, SettingsStore.Current.MoveCyl2UpDelay = Convert.ToInt16(CallNumKey(DisplayedData)), 0); }
        internal void FixCylUpDelaySet() { DisplayedData = Convert.ToString(SettingsStore.Current.FixCylUpDelay); PLCWrite(252, SettingsStore.Current.FixCylUpDelay = Convert.ToInt16(CallNumKey(DisplayedData)), 0); }
        internal void FixCylDownDelaySet() { DisplayedData = Convert.ToString(SettingsStore.Current.FixCylDownDelay); PLCWrite(253, SettingsStore.Current.FixCylDownDelay = Convert.ToInt16(CallNumKey(DisplayedData)), 0); }

        // =================================================== EMC Box Setting =========================================================
        internal void EMCOpenSet() { DisplayedData = Convert.ToString(SettingsStore.Current.EMCOpenDelay); PLCWrite(254, SettingsStore.Current.EMCOpenDelay = Convert.ToInt16(CallNumKey(DisplayedData)), 0); }
        internal void EMCCloseSet() { DisplayedData = Convert.ToString(SettingsStore.Current.EMCCloseDelay); PLCWrite(255, SettingsStore.Current.EMCCloseDelay = Convert.ToInt16(CallNumKey(DisplayedData)), 0); }
        internal void EMCFWDSet() { DisplayedData = Convert.ToString(SettingsStore.Current.EMCFWDDelay); PLCWrite(256, SettingsStore.Current.EMCFWDDelay = Convert.ToInt16(CallNumKey(DisplayedData)), 0); }
        internal void EMCBWDSet() { DisplayedData = Convert.ToString(SettingsStore.Current.EMCBWDDelay); PLCWrite(257, SettingsStore.Current.EMCBWDDelay = Convert.ToInt16(CallNumKey(DisplayedData)), 0); }


        #endregion





        #region Manual Page Methods
        public void ScreenOffCountDown(int sec)
        {
            var timerMsgBox = new TimerMessageBox(sec);
            timerMsgBox.WindowStyle = WindowStyle.None;
            bool? result = timerMsgBox.ShowDialog(); // Timer Out 되면 true 반환, 취소 입력시 타이머 멈추고 false 반환

            if (result == true)                      // 시간 다 되면
            {
                MonitorControl.TurnOffMonitor();     // 모니터 끄고
                ButtonVisible("Auto");               // 메인 화면으로 변경
            }
        }



        internal void ScreenOff()
        {
            ScreenOffCountDown(5);
        }



        internal void HomeButtonPress()
        {
            if (IsReady == false) { return; }
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



        internal void EMCSupply()
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

    }
}
