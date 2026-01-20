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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RobotMonitor_2
{
    /// <summary>
    /// TimerMessageBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TimerMessageBox : Window
    {

        private DispatcherTimer _timer;
        private int _remainingSeconds;

        public TimerMessageBox(int seconds)
        {
            InitializeComponent();
            _remainingSeconds = seconds;
            MessageUpdate();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        public void Timer_Tick(Object sender, EventArgs e)
        {
            _remainingSeconds--;
            MessageUpdate();

            if (_remainingSeconds <= 0)
            {
                _timer.Stop();
                DialogResult = true;
                Close();
            }
        }

        public void MessageUpdate()
        {
            MessageText.Text = $"화면이 {_remainingSeconds}초 후에 꺼집니다.";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            DialogResult = false;
            Close();
        }
    }
}
