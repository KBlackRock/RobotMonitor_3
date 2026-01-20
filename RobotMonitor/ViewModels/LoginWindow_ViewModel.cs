using CommunityToolkit.Mvvm.Messaging;
using RobotMonitor_2.Models;
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

namespace RobotMonitor_2.ViewModels
{
    internal class LoginWindow_ViewModel : INotifyPropertyChanged
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

        private Models.XmlParser m_XmlParser;
        public Window LoginWindow;

        private string inputPassWord; public string InputPassWord { get { return inputPassWord; } set { inputPassWord = value; OnPropertyChanged(); } }

        public string PassWord { get { return m_XmlParser.SavedData.PassWord; } set { if (m_XmlParser.SavedData.PassWord != value) { m_XmlParser.SavedData.PassWord = value; RaisePropertyChangedEvent("PassWord"); } } }


        public LoginWindow_ViewModel()
        {
            m_XmlParser = new Models.XmlParser();
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
