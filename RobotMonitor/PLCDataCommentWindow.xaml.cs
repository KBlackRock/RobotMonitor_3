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
    /// PLCDataComment.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PLCDataCommentWindow : Window
    {

        public PLCDataCommentWindow()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
