using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using RobotMonitor_3.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobotMonitor_3.ViewModels
{
    internal partial class NumberKeyboard_ViewModel : ObservableObject
    {

        public string symbol;
        [ObservableProperty] private string displayNumber;
        [ObservableProperty] private string displayNumberOld;

        public NumberKeyboard_ViewModel()
        {
            WeakReferenceMessenger.Default.Unregister<DisplayDataSender>(this);
            WeakReferenceMessenger.Default.Register<DisplayDataSender>(this, (r, m) => { DisplayNumberOld = m.Value; });
        }

        internal void Btn_0() { DisplayNumber += "0"; }
        internal void Btn_1() { DisplayNumber += "1"; }
        internal void Btn_2() { DisplayNumber += "2"; }
        internal void Btn_3() { DisplayNumber += "3"; }
        internal void Btn_4() { DisplayNumber += "4"; }
        internal void Btn_5() { DisplayNumber += "5"; }
        internal void Btn_6() { DisplayNumber += "6"; }
        internal void Btn_7() { DisplayNumber += "7"; }
        internal void Btn_8() { DisplayNumber += "8"; }
        internal void Btn_9() { DisplayNumber += "9"; }

        internal void Btn_Dot() => DisplayNumber = DisplayNumber == "" ? "0." : DisplayNumber += ".";

        internal void Btn_BS()
        {
            try
            {
                if (DisplayNumber == "" || DisplayNumber == null) return;
                DisplayNumber = DisplayNumber.Remove((DisplayNumber.Length - 1));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        internal void Btn_Del() { DisplayNumber = ""; }

        internal void Btn_Reverse()
        {
            if (DisplayNumber != "")
            {
                symbol = DisplayNumber.Substring(0, 1);
                if (symbol != "-")
                {
                    DisplayNumber = "-" + DisplayNumber;
                }
                if (symbol == "-")
                {
                    DisplayNumber = DisplayNumber.Remove(0, 1);
                }
            }
            else
            {
                DisplayNumber = "-";
            }
        }

        internal void OldDataDisplay()
        {
            WeakReferenceMessenger.Default.Unregister<DisplayDataSender>(this);
            WeakReferenceMessenger.Default.Register<DisplayDataSender>(this, (r, m) => { DisplayNumber = m.Value; });
        }

        internal void Btn_Cancel()
        {
            WeakReferenceMessenger.Default.Send(new KeyboardDataSender(""));
        }

        internal void Btn_OK()
        {
            WeakReferenceMessenger.Default.Send(new KeyboardDataSender(DisplayNumber));
        }

    }


}
