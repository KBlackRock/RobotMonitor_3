using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotMonitor_3.Models
{
    public class DeviceItem : ObservableObject
    {
        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private int _deviceSize;
        public int DeviceSize { get => _deviceSize; set => SetProperty(ref _deviceSize, value); }

        private int _stackedShotCount;
        public int StackedShotCount
        {
            get => _stackedShotCount;
            set
            {
                if (SetProperty(ref _stackedShotCount, value))
                {
                    OnPropertyChanged(nameof(StackedDeviceCount));
                }
            }
        }

        private int _unStackedShotCount;
        public int UnStackedShotCount
        {
            get => _unStackedShotCount;
            set
            {
                if (SetProperty(ref _unStackedShotCount, value))
                {
                    OnPropertyChanged(nameof(UnStackedDeviceCount));
                }
            }
        }

        public long StackedDeviceCount => (long)StackedShotCount * DeviceSize;
        public long UnStackedDeviceCount => (long)UnStackedShotCount * DeviceSize;

        public DeviceItem()
        {
            Name = "New Device";
            DeviceSize = 1;
        }
    }
}
