using RobotMonitor_3.Utilities;
using RobotMonitor_3.ViewModels;
using System;
using System.Collections.Generic;
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

        private short _tempD198;

        public PlcDataProcessor(MainWindow_ViewModel viewModel)
        {
            _viewModel = viewModel;

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
        }

        public void Execute(int index, short data)
        {
            if (_handlers.TryGetValue(index, out var handler))
            {
                handler.Invoke(data);
            }
        }

        public void D100(short data)  // PLC 에러 코드
        {
            var errorInfo = _errorRepository.GetPlcError(data);
            _viewModel.Error(errorInfo.ImageName, errorInfo.Message);
        }

        public void D101(short data)
        {

        }

        public void D102(short data)
        {

        }

        public void D103(short data)
        {

        }

        public void D104(short data)
        {
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
        }

        public void D111(short data)
        {
        }

        public void D112(short data)
        {
        }

        public void D113(short data)
        {
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
        }

        public void D171(short data)
        {
        }

        public void D172(short data)
        {
        }

        public void D173(short data)
        {
        }

        public void D174(short data)
        {
        }

        public void D175(short data)
        {
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

        public void D190(short data)
        {
        }

        public void D191(short data)
        {
        }

        public void D192(short data)
        {
        }

        public void D193(short data)
        {
        }

        public void D194(short data)
        {
        }

        public void D195(short data)
        {
        }

        public void D196(short data)
        {
        }

        public void D197(short data)
        {
        }

        public void D198(short data)
        {
            _tempD198 = data;
        }

        public void D199(short data)
        {
            int combinedInt = (int)((ushort)_tempD198 | ((uint)data << 16));
        }
    }
}
