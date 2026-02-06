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
    /// NumbereKeyboard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NumberKeyboard : Window
    {
        public NumberKeyboard()
        {
            InitializeComponent();
            DataContext = new NumberKeyboard_ViewModel();
        }


        private void btn_OldData_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as NumberKeyboard_ViewModel).OldDataDisplay();


        private void btn_0_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_0();
        private void btn_1_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_1();
        private void btn_2_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_2();
        private void btn_3_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_3();
        private void btn_4_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_4();
        private void btn_5_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_5();
        private void btn_6_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_6();
        private void btn_7_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_7();
        private void btn_8_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_8();
        private void btn_9_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_9();
        private void btn_Dot_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_Dot();
        private void btn_BS_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_BS();
        private void btn_Del_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_Del();
        private void btn_Reverse_Click(object sender, RoutedEventArgs e) => (DataContext as NumberKeyboard_ViewModel).Btn_Reverse();

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as NumberKeyboard_ViewModel).Btn_Cancel();
            this.Close();
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as NumberKeyboard_ViewModel).Btn_OK();
            this.Close();
        }
    }
}
