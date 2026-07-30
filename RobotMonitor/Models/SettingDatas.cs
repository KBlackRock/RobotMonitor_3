using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RobotMonitor_3.Models
{
    public partial class SettingDatas : ObservableObject
    {
        [ObservableProperty] private int m1_CurrentPos;
        [ObservableProperty] private int m1_PickUpPos = 469250;
        [ObservableProperty] private int m1_FirstPos = -3995750;
        [ObservableProperty] private int m1_SecondPos = -1432050;
        [ObservableProperty] private int m1_CheckPos = -1132050;
        [ObservableProperty] private int m1_HighSpeed = 400000;
        [ObservableProperty] private int m1_LowSpeed = 100000;
        [ObservableProperty] private int m2_CurrentPos;
        [ObservableProperty] private int m2_FirstPos = -1604000;
        [ObservableProperty] private int m2_Pitch = 30000;
        [ObservableProperty] private int m2_Speed = 100000;
        [ObservableProperty] private short moveCyl1DownDelay;
        [ObservableProperty] private short moveCyl2DownDelay;
        [ObservableProperty] private short moveCyl1UpDelay;
        [ObservableProperty] private short moveCyl2UpDelay;
        [ObservableProperty] private short fixCylUpDelay;
        [ObservableProperty] private short fixCylDownDelay;
    }
}
