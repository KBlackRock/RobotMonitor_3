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
        [ObservableProperty] private int _M1_CurrentPos;
        [ObservableProperty] private int _M1_PickUpPos = 469250;
        [ObservableProperty] private int _M1_FirstPos = -3995750;
        [ObservableProperty] private int _M1_SecondPos = -1432050;
        [ObservableProperty] private int _M1_CheckPos = -1132050;
        [ObservableProperty] private int _M1_HighSpeed = 400000;
        [ObservableProperty] private int _M1_LowSpeed = 100000;
        [ObservableProperty] private int _M2_CurrentPos;
        [ObservableProperty] private int _M2_FirstPos = -1604000;
        [ObservableProperty] private int _M2_Pitch = 30000;
        [ObservableProperty] private int _M2_Speed = 100000;
        [ObservableProperty] private short _MoveCyl1DownDelay;
        [ObservableProperty] private short _MoveCyl2DownDelay;
        [ObservableProperty] private short _MoveCyl1UpDelay;
        [ObservableProperty] private short _MoveCyl2UpDelay;
        [ObservableProperty] private short _FixCylUpDelay;
        [ObservableProperty] private short _FixCylDownDelay;
        [ObservableProperty] private short _EMCOpenDelay = 10;
        [ObservableProperty] private short _EMCCloseDelay = 10;
        [ObservableProperty] private short _EMCFWDDelay = 10;
        [ObservableProperty] private short _EMCBWDDelay = 10;

    }
}
