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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RobotMonitor_3.ViewModels;

namespace RobotMonitor_3.Pages
{
    /// <summary>
    /// Error_Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Error_Page : Page
    {
        public Error_Page()
        {
            InitializeComponent();
            DataContext = new MainWindow_ViewModel();
        }
    }
}
