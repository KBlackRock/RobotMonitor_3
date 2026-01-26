using RobotMonitor_3.ViewModels;
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
    /// DeviceWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DeviceWindow : Window
    {
        private DeviceWindow_ViewModel _viewModel;

        public DeviceWindow()
        {
            InitializeComponent();
            _viewModel = new DeviceWindow_ViewModel();
            this.DataContext = _viewModel;

            _viewModel.Start(this);
        }

        private void btn_Apply_Click(object sender, RoutedEventArgs e) => _viewModel.ApplyClick();
        private void btn_OK_Click(object sender, RoutedEventArgs e) => _viewModel.OKClick();
        private void btn_Cancel_Click(object sender, RoutedEventArgs e) => _viewModel.CancelClick();

        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tBox = sender as TextBox;

            if (tBox != null)
            {
                tBox.Focus();

                _viewModel.OpenTouchKeyboard();
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) => _viewModel.KillTouchKeyboard();

        private void SizeBox1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => _viewModel.SizeBtn1_Click();
        private void SizeBox2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => _viewModel.SizeBtn2_Click();
        private void SizeBox3_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => _viewModel.SizeBtn3_Click();
        private void SizeBox4_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => _viewModel.SizeBtn4_Click();
        private void SizeBox5_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => _viewModel.SizeBtn5_Click();
        private void SizeBox6_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => _viewModel.SizeBtn6_Click();
        private void SizeBox7_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => _viewModel.SizeBtn7_Click();
        private void SizeBox8_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => _viewModel.SizeBtn8_Click();
        private void SizeBox9_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => _viewModel.SizeBtn9_Click();
        private void SizeBox10_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => _viewModel.SizeBtn10_Click();

    }
}
