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
using RobotMonitor_3.ViewModels;

namespace RobotMonitor_3
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginWindow_ViewModel();

            (DataContext as LoginWindow_ViewModel).Start(this);
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as LoginWindow_ViewModel).CancelClick();
        }

        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as LoginWindow_ViewModel).InputPW();
        }
    }
}
