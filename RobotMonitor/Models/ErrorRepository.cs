using System.Collections;
using System.Collections.Generic;

namespace RobotMonitor_3.Models
{
    // 에러 정보를 담을 간단한 모델
    public class ErrorInfo
    {
        public string ImageName { get; set; }
        public string Message { get; set; }
        public string Category { get; set; } // "Supply", "Robot" 등 (필요 시 확장)

        private string path = (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RobotMonitor_3\img\");

        public ErrorInfo(string img, string msg, string cat = "")
        {
            ImageName = path + img;
            Message = msg;
            Category = cat;
        }
    }

    public class ErrorRepository
    {
        private readonly Dictionary<string, ErrorInfo> _errorMapRobot;
        private readonly Dictionary<short, ErrorInfo> _errorMapPLC;

        // 비트맵 워드 정의 : PLC 주소 → (카테고리명, 코드 베이스)
        public static readonly Dictionary<int, (string Category, int CodeBase)> BitmapWords
            = new Dictionary<int, (string, int)>
        {
        { 190, ("System",     0) },
        { 191, ("Robot",    100) },
        { 192, ("Tool",     200) },
        { 193, ("Press",    300) },
        { 194, ("Loader",   400) },
        { 195, ("Preheater",500) },
        { 196, ("EMC",      600) },
        };

        public ErrorRepository()
        {
            _errorMapPLC = new Dictionary<short, ErrorInfo>
            {
                // 시스템 ( D190 : bit0~15 → 0~15 )
                {   0, new ErrorInfo("", "시스템 [CC-Link]\r PLC-Robot 통신이상", "System") },
                {   1, new ErrorInfo("", "시스템 [MC-Protocol]\r PLC-PC 통신이상", "System") },
                {   2, new ErrorInfo("", "시스템 [안전]\r 도어 열림 감지", "System") },
                {   3, new ErrorInfo("", "시스템 [안전]\r 비상정지 버튼 눌림", "System") },
                {   4, new ErrorInfo("", "시스템 [준비]\r 설비 리셋 타임오버", "System") },
                {   5, new ErrorInfo("", "시스템 [준비]\r 로봇 리셋 타임오버", "System") },

                // 로봇 ( D191 : bit0~15 → 100~115 )
                { 100, new ErrorInfo("", "로봇 [툴]\r 툴 감지 센서 이상", "Robot") },
                { 101, new ErrorInfo("", "로봇 [이형제]\r 실린더 하강 센서 감지 이상", "Robot") },
                { 102, new ErrorInfo("", "로봇 [이형제]\r 실린더 상승 센서 감지 이상", "Robot") },
                { 103, new ErrorInfo("", "로봇 \r ", "Robot") },
                { 104, new ErrorInfo("", "로봇 \r ", "Robot") },
                { 105, new ErrorInfo("", "로봇 \r ", "Robot") },
                { 106, new ErrorInfo("", "로봇 \r ", "Robot") },
                { 107, new ErrorInfo("", "로봇 \r ", "Robot") },
                { 108, new ErrorInfo("", "로봇 \r ", "Robot") },
                { 109, new ErrorInfo("", "로봇 \r ", "Robot") },
                { 110, new ErrorInfo("", "로봇 \r ", "Robot") },
                { 111, new ErrorInfo("", "로봇 \r ", "Robot") },
                { 112, new ErrorInfo("", "로봇 \r ", "Robot") },
                { 113, new ErrorInfo("", "로봇 [비전]\r 오토로더 로딩 이상", "Robot") },
                { 114, new ErrorInfo("", "로봇 [비전]\r 프레스 로딩 이상", "Robot") },
                { 115, new ErrorInfo("", "로봇 [비전]\r 프레스 성형제품 상태 이상", "Robot") },

                // 툴 ( D192 : bit0~15 → 200~215 )
                { 200, new ErrorInfo("", "로봇 툴 에러\r ", "Tool") },
                { 201, new ErrorInfo("", "로봇 툴 에러\r ", "Tool") },
                { 202, new ErrorInfo("", "로봇 툴 에러\r ", "Tool") },

                // 프레스 ( D193 : bit0~15 → 300~315 )
                { 300, new ErrorInfo("", "프레스 \r 준비 신호 이상", "Press") },
                { 301, new ErrorInfo("", "프레스 \r 상승 완료 신호 이상", "Press") },
                { 302, new ErrorInfo("", "프레스 \r 쳄버 닫힘 신호 이상", "Press") },
                { 303, new ErrorInfo("", "프레스 \r 열림 신호 이상", "Press") },
                { 304, new ErrorInfo("", "프레스 \r 준비위치 도달 이상", "Press") },

                // 오토로더 ( D194 : bit0~15 → 400~415 )
                { 400, new ErrorInfo("", "오토로더 [히터]\r 히터 1 고온 에러", "Loader") },
                { 401, new ErrorInfo("", "오토로더 [히터]\r 히터 2 고온 에러", "Loader") },
                { 402, new ErrorInfo("", "오토로더 [히터]\r 히터 1 저온 에러", "Loader") },
                { 403, new ErrorInfo("", "오토로더 [히터]\r 히터 2 저온 에러", "Loader") },
                { 404, new ErrorInfo("", "오토로더 [인버터모터]\r 프레임 공급 모터 동작이상", "Loader") },
                { 405, new ErrorInfo("", "오토로더 [인버터모터]\r 프레임 공급 이상 ( 프레임 없음 )", "Loader") },
                { 406, new ErrorInfo("", "오토로더 [프레임 이송]\r 실린더 1 하강동작 이상", "Loader") },
                { 407, new ErrorInfo("", "오토로더 [프레임 이송]\r 실린더 2 상승동작 이상", "Loader") },
                { 408, new ErrorInfo("", "오토로더 [프레임 이송]\r 실린더 1 하강동작 이상", "Loader") },
                { 409, new ErrorInfo("", "오토로더 [프레임 이송]\r 실린더 2 상승동작 이상", "Loader") },
                { 410, new ErrorInfo("", "오토로더 [프레임 이송]\r M1 모터 동작 이상", "Loader") },
                { 411, new ErrorInfo("", "오토로더 [매거진 엘레베이터]\r M2 모터 동작 이상", "Loader") },
                { 412, new ErrorInfo("", "오토로더 [매거진 엘레베이터]\r 작업 완료 ( 프레임 없음 )", "Loader") },

                // 프리히터 ( D195 : bit0~15 → 500~515 )
                { 500, new ErrorInfo("", "프리히터 \r 전원 OFF", "Preheater") },
                { 501, new ErrorInfo("", "프리히터 \r 도어 열림 이상", "Preheater") },
                { 502, new ErrorInfo("", "프리히터 \r 예열 동작 이상", "Preheater") },

                // EMC ( D196 : bit0~15 → 600~615 )
                { 600, new ErrorInfo("", "EMC 박스 \r 커버 열림 이상", "EMC") },
                { 601, new ErrorInfo("", "EMC 박스 \r EMC 공급 이상", "EMC") },
            };
        }


        public ErrorInfo GetPlcError(short code)
            => _errorMapPLC.TryGetValue(code, out var info) ? info : new ErrorInfo("", $"PLC 에러코드 미 할당\rCode : {code}");

        public static short ToErrorCode(int address, int bit)
        {
            int baseCode = BitmapWords.TryGetValue(address, out var v) ? v.CodeBase : 0;
            return (short)(baseCode + bit);
        }
    }
}