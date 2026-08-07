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

namespace RobotMonitor_3
{
    /// <summary>
    /// SplashWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow() => InitializeComponent();

        public void SetStatus(LoadStep step)
        {
            StatusText.Text = step.Message;
            Bar.Value = step.Percent;
        }
    }

    public class LoadStep
    {
        public string Message { get; }
        public int Percent { get; }
        public LoadStep(string message, int percent)
        {
            Message = message;
            Percent = percent;
        }
    }
}
