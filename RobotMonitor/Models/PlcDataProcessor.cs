using RobotMonitor_3.Services;
using RobotMonitor_3.Utilities;
using RobotMonitor_3.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotMonitor_3.Models
{
    class PlcDataProcessor
    {
        private readonly Dictionary<int, Action<short>> _handlers;

        private readonly MainWindow_ViewModel _viewModel;

        private readonly ErrorRepository _errorRepository = new ErrorRepository();

        // 에러 워드 이전값 보관 ( 에지 검출용 )  key : PLC 주소
        private readonly Dictionary<int, short> _prevErrorWord = new Dictionary<int, short>();

        // 32비트(DMOV) 핸들러 : key = 하위 워드 인덱스 (D100 기준 오프셋)
        private readonly Dictionary<int, Action<int>> _dwordHandlers;

        // 32비트 이전값 (변화 검출용)
        private readonly Dictionary<int, int> _prevDword = new Dictionary<int, int>();

        public PlcDataProcessor(MainWindow_ViewModel viewModel)
        {
            _viewModel = viewModel;

            foreach (var addr in ErrorRepository.BitmapWords.Keys)
                _prevErrorWord[addr] = 0;

            _handlers = new Dictionary<int, Action<short>>
            {
                { 0,  D100}, { 1,  D101}, { 2,  D102}, { 3,  D103}, { 4,  D104}, { 5,  D105}, { 6,  D106}, { 7,  D107}, { 8,  D108}, { 9,  D109},
                {10,  D110}, {11,  D111}, {12,  D112}, {13,  D113}, {14,  D114}, {15,  D115}, {16,  D116}, {17,  D117}, {18,  D118}, {19,  D119},
                {20,  D120}, {21,  D121}, {22,  D122}, {23,  D123}, {24,  D124}, {25,  D125}, {26,  D126}, {27,  D127}, {28,  D128}, {29,  D129},
                {30,  D130}, {31,  D131}, {32,  D132}, {33,  D133}, {34,  D134}, {35,  D135}, {36,  D136}, {37,  D137}, {38,  D138}, {39,  D139},
                {40,  D140}, {41,  D141}, {42,  D142}, {43,  D143}, {44,  D144}, {45,  D145}, {46,  D146}, {47,  D147}, {48,  D148}, {49,  D149},
                {50,  D150}, {51,  D151}, {52,  D152}, {53,  D153}, {54,  D154}, {55,  D155}, {56,  D156}, {57,  D157}, {58,  D158}, {59,  D159},
                {60,  D160}, {61,  D161}, {62,  D162}, {63,  D163}, {64,  D164}, {65,  D165}, {66,  D166}, {67,  D167}, {68,  D168}, {69,  D169},
                {70,  D170}, {71,  D171}, {72,  D172}, {73,  D173}, {74,  D174}, {75,  D175}, {76,  D176}, {77,  D177}, {78,  D178}, {79,  D179},
                {80,  D180}, {81,  D181}, {82,  D182}, {83,  D183}, {84,  D184}, {85,  D185}, {86,  D186}, {87,  D187}, {88,  D188}, {89,  D189},
                {90,  D190}, {91,  D191}, {92,  D192}, {93,  D193}, {94,  D194}, {95,  D195}, {96,  D196}, {97,  D197}, {98,  D198}, {99,  D199}
            };

            _dwordHandlers = new Dictionary<int, Action<int>>
            {
                { 30, D130_D131 },
                { 32, D132_D133 },
                { 34, D134_D135 },
                { 36, D136_D137 },
                { 38, D138_D139 },
                { 40, D140_D141 },
                { 42, D142_D143 },
                { 44, D144_D145 },
            };
        }

        public void Execute(int index, short data)
        {
            if (_handlers.TryGetValue(index, out var handler))
            {
                handler.Invoke(data);
            }
        }

        public void ExecuteDword(byte[] data)
        {
            foreach (var kv in _dwordHandlers)
            {
                int low = kv.Key;
                int value = BitConverter.ToInt32(data, low * 2);   // 하위 워드 먼저 = 리틀 엔디안

                if (_prevDword.TryGetValue(low, out int prev) && prev == value) continue;
                _prevDword[low] = value;

                kv.Value.Invoke(value);
            }
        }

        /// <summary>비트맵 에러 워드 공통 처리</summary>
        private void ProcessErrorWord(int address, short data)
        {
            short prev = _prevErrorWord.TryGetValue(address, out var p) ? p : (short)0;
            if (prev == data) return;

            ushort now = unchecked((ushort)data);
            ushort old = unchecked((ushort)prev);
            ushort rise = (ushort)(now & ~old);   // 0 → 1 : 에러 발생
            ushort fall = (ushort)(old & ~now);   // 1 → 0 : 에러 해제

            for (int bit = 0; bit < 16; bit++)
            {
                ushort mask = (ushort)(1 << bit);

                if ((rise & mask) != 0)
                {
                    short code = ErrorRepository.ToErrorCode(address, bit);
                    _viewModel.RaisePlcError(code);
                }
                else if ((fall & mask) != 0)
                {
                    short code = ErrorRepository.ToErrorCode(address, bit);
                    _viewModel.ClearPlcError(code);
                }
            }

            _prevErrorWord[address] = data;
        }

        public void D130_D131(int data)
        {
            SettingsStore.Current.M1_CurrentPos = data;
        }

        public void D132_D133(int data)
        {
            SettingsStore.Current.M2_CurrentPos = data;
        }

        public void D134_D135(int data)
        {
        }

        public void D136_D137(int data)
        {
        }

        public void D138_D139(int data)
        {
        }

        public void D140_D141(int data)
        {
        }

        public void D142_D143(int data)
        {
        }

        public void D144_D145(int data)
        {
        }



        public void D100(short data)
        {
            switch (data)
            {
                case 0:
                    _viewModel.LabelSet("");
                    break;
                case 1:
                    _viewModel.LabelSet("Stopped");
                    _viewModel.ButtonVisible("Auto");
                    break;
                case 2:
                    _viewModel.LabelSet("Ready");
                    _viewModel.ButtonVisible("Auto");
                    break;
                case 3:
                    _viewModel.LabelSet("Running");
                    _viewModel.ButtonVisible("Auto");
                    break;
                case 4:
                    _viewModel.LabelSet("Origin");
                    _viewModel.ButtonVisible("Auto");
                    break;
                case 5:
                    _viewModel.LabelSet("Stopped");
                    _viewModel.ButtonVisible("Error");
                    break;
                default:
                    break;
            }
        }



        public void D101(short data)
        {
            _viewModel.needOrigin = data == 0;
        }

        public void D102(short data)
        {
            _viewModel.VisionReadyColor = data == 0 ? "LightGray" : "LimeGreen";
        }

        public void D103(short data)
        {
            _viewModel.PreheaterReadyColor = data == 0 ? "LightGray" : "LimeGreen";
        }

        public void D104(short data)
        {
            _viewModel.HeaterReadyColor = data == 0 ? "LightGray" : "LimeGreen";
        }

        public void D105(short data)
        {
        }

        public void D106(short data)
        {
        }

        public void D107(short data)
        {
        }

        public void D108(short data)
        {
        }

        public void D109(short data)
        {
        }

        public void D110(short data)
        {
            _viewModel.HeaterBtn1BG = data == 0 ? "LightGray" : "LimeGreen";
        }

        public void D111(short data)
        {
            _viewModel.HeaterBtn2BG = data == 0 ? "LightGray" : "LimeGreen";
        }

        public void D112(short data)
        {
            _viewModel.MagazineElevPos = data == 0 ? "대기위치" : Convert.ToString(data);
        }

        public void D113(short data)
        {
            switch (data)
            {
                case 0:
                    _viewModel.FrameLoad1Color = "LightGray";
                    _viewModel.FrameLoad2Color = "LightGray";
                    _viewModel.FrameLoad3Color = "LightGray";
                    _viewModel.FrameLoad4Color = "LightGray";
                    break;
                case 1:
                    _viewModel.FrameLoad1Color = "LightGray";
                    _viewModel.FrameLoad2Color = "LightGray";
                    _viewModel.FrameLoad3Color = "LightGreen";
                    _viewModel.FrameLoad4Color = "LightGreen";
                    break;
                case 2:
                    _viewModel.FrameLoad1Color = "LightGreen";
                    _viewModel.FrameLoad2Color = "LightGreen";
                    _viewModel.FrameLoad3Color = "LightGreen";
                    _viewModel.FrameLoad4Color = "LightGreen";
                    break;
                default:
                    break;
            }
        }

        public void D114(short data)
        {
        }

        public void D115(short data)
        {
        }

        public void D116(short data)
        {
        }

        public void D117(short data)
        {
        }

        public void D118(short data)
        {
        }

        public void D119(short data)
        {
        }

        public void D120(short data)
        {
        }

        public void D121(short data)
        {
        }

        public void D122(short data)
        {
        }

        public void D123(short data)
        {
        }

        public void D124(short data)
        {
        }

        public void D125(short data)
        {
        }

        public void D126(short data)
        {
        }

        public void D127(short data)
        {
        }

        public void D128(short data)
        {
        }

        public void D129(short data)
        {
        }

        public void D130(short data)
        {
        }

        public void D131(short data)
        {
        }

        public void D132(short data)
        {
        }

        public void D133(short data)
        {
        }

        public void D134(short data)
        {
        }

        public void D135(short data)
        {
        }

        public void D136(short data)
        {
        }

        public void D137(short data)
        {
        }

        public void D138(short data)
        {
        }

        public void D139(short data)
        {
        }

        public void D140(short data)
        {
        }

        public void D141(short data)
        {
        }

        public void D142(short data)
        {
        }

        public void D143(short data)
        {
        }

        public void D144(short data)
        {
        }

        public void D145(short data)
        {
        }

        public void D146(short data)
        {
        }

        public void D147(short data)
        {
        }

        public void D148(short data)
        {
        }

        public void D149(short data)
        {
        }

        public void D150(short data)
        {
        }

        public void D151(short data)
        {
        }

        public void D152(short data)
        {
        }

        public void D153(short data)
        {
        }

        public void D154(short data)
        {
        }

        public void D155(short data)
        {
        }

        public void D156(short data)
        {
        }

        public void D157(short data)
        {
        }

        public void D158(short data)
        {
        }

        public void D159(short data)
        {
        }

        public void D160(short data)
        {
        }

        public void D161(short data)
        {
        }

        public void D162(short data)
        {
        }

        public void D163(short data)
        {
        }

        public void D164(short data)
        {
        }

        public void D165(short data)
        {
        }

        public void D166(short data)
        {
        }

        public void D167(short data)
        {
        }

        public void D168(short data)
        {
        }

        public void D169(short data)
        {
        }

        public void D170(short data)
        {
            _viewModel.PressReadyColor = data == 0 ? "LightGray" : "LimeGreen";
        }

        public void D171(short data)
        {
            _viewModel.PressWork1Color = data == 0 ? "LightGray" : "LimeGreen";
        }

        public void D172(short data)
        {
            _viewModel.PressWork2Color = data == 0 ? "LightGray" : "LimeGreen";
        }

        public void D173(short data)
        {
            _viewModel.PressWork3Color = data == 0 ? "LightGray" : "LimeGreen";
        }

        public void D174(short data)
        {
            _viewModel.PressWork4Color = data == 0 ? "LightGray" : "LimeGreen";
        }

        public void D175(short data)
        {
            _viewModel.PressWork5Color = data == 0 ? "LightGray" : "LimeGreen";
        }

        public void D176(short data)
        {
        }

        public void D177(short data)
        {
        }

        public void D178(short data)
        {
        }

        public void D179(short data)
        {
        }

        public void D180(short data)
        {
        }

        public void D181(short data)
        {
        }

        public void D182(short data)
        {
        }

        public void D183(short data)
        {
        }

        public void D184(short data)
        {
        }

        public void D185(short data)
        {
        }

        public void D186(short data)
        {
        }

        public void D187(short data)
        {
        }

        public void D188(short data)
        {
        }

        public void D189(short data)
        {
        }

        public void D190(short data) => ProcessErrorWord(190, data);  // 시스템
        public void D191(short data) => ProcessErrorWord(191, data);  // 로봇
        public void D192(short data) => ProcessErrorWord(192, data);  // 툴
        public void D193(short data) => ProcessErrorWord(193, data);  // 프레스
        public void D194(short data) => ProcessErrorWord(194, data);  // 오토로더
        public void D195(short data) => ProcessErrorWord(195, data);  // 프리히터
        public void D196(short data) => ProcessErrorWord(196, data);  // EMC

        public void D197(short data)
        {

        }

        public void D198(short data)
        {

        }

        public void D199(short data)
        {

        }
    }
}
