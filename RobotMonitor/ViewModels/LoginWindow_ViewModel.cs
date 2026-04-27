using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using RobotMonitor_3.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotMonitor_3.ViewModels
{
    internal partial class LoginWindow_ViewModel : ObservableObject
    {


        private XmlParser m_XmlParser;
        public Window LoginWindow;

        [ObservableProperty] private string inputPassWord;

        public string PassWord { get { return m_XmlParser.SavedData.PassWord; } set { if (m_XmlParser.SavedData.PassWord != value) { m_XmlParser.SavedData.PassWord = value; OnPropertyChanged(); } } }


        public LoginWindow_ViewModel()
        {
            m_XmlParser = new Utilities.XmlParser();
        }

        public void Start(Window window)
        {
            LoginWindow = window;
            LoginWindow.ResizeMode = ResizeMode.NoResize;
        }

        internal void CancelClick()
        {
            WeakReferenceMessenger.Default.Send(new LoginBool(false));
            LoginWindow.Close();
        }

        internal void InputPW()
        {
            CallNumKey();
            if (InputPassWord == PassWord)
            {
                WeakReferenceMessenger.Default.Send(new LoginBool(true));
                LoginWindow.Close();
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new LoginBool(false));
                MessageBox.Show("비밀번호가 맞지 않습니다.");
                InputPassWord = "";
            }
        }

        public void CallNumKey() // 숫자키보드 출력
        {
            var KeyWindow = new NumberKeyboard();
            KeyWindow.Left = 300;
            KeyWindow.Top = 100;
            KeyWindow.ResizeMode = ResizeMode.NoResize;
            KeyWindow.Topmost = true;
            WeakReferenceMessenger.Default.Unregister<KeyboardDataSender>(this);
            WeakReferenceMessenger.Default.Register<KeyboardDataSender>(this, (r, m) => { InputPassWord = m.Value; });
            KeyWindow.ShowDialog();
        }
    }
}
