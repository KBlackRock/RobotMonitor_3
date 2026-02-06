using System.Collections.Generic;

namespace RobotMonitor_3.Services
{
    // 에러 정보를 담을 간단한 모델
    public class ErrorInfo
    {
        public string ImageName { get; set; }
        public string Message { get; set; }
        public string Category { get; set; } // "Supply", "Robot" 등 (필요 시 확장)

        public ErrorInfo(string img, string msg, string cat = "")
        {
            ImageName = img;
            Message = msg;
            Category = cat;
        }
    }

    public class ErrorRepository
    {
        private readonly Dictionary<string, ErrorInfo> _errorMap;

        public ErrorRepository()
        {
            _errorMap = new Dictionary<string, ErrorInfo>
            {
                // 안전 관련
                { "EStop",                    new ErrorInfo("Emergency", "[비상정지]") },
                { "DoorOpen",                 new ErrorInfo("Door", "[도어 열림 감지]") },
                                              
                // 작업 관련                  
                { "WorkMode_Select",          new ErrorInfo("WorkMode", "작업모드 선택 이상") },
                { "PCBMagazineEmpty",         new ErrorInfo("Supply", "[Lot End]\r 작업완료 ( 매거진 없음 )") },
                { "ExtractCountOver",         new ErrorInfo("", "10 샷 작업 완료\r 금형 클리닝 및 완제품 배출구를 비워주세요.") },
                                              
                // 서플라이 관련              
                { "Servo_M1_ORGTO",           new ErrorInfo("Supply", "매거진 서플라이 모터 오리진 동작 이상") },
                { "Servo_M1_SupTO",           new ErrorInfo("Supply", "매거진 서플라이 모터 공급 동작 이상") },
                { "Servo_M1_RelTO",           new ErrorInfo("Supply", "매거진 서플라이 모터 리턴 동작 이상") },
                { "Cylinder_Supply_Left",     new ErrorInfo("Supply", "매거진 서플라이 좌측 실린더 동작 이상")  },
                { "Cylinder_Supply_Right",    new ErrorInfo("Supply", "매거진 서플라이 우측 실린더 동작 이상") },
                { "MagazineSensorOn",         new ErrorInfo("Supply", "매거진 센서 감지 이상") },
                { "PCBWorkCountOver",         new ErrorInfo("Supply", "매거진 PCB 작업수량 초과") },
                { "Servo_M1_SupSensor",       new ErrorInfo("Supply", "매거진 공급동작 신호시 센서 감지됨\r 매거진을 뒤로 물리고 다시 시작 해 주세요.")  },
                                              
                // 턴 테이블 관련        
                { "Servo_M2_ORGTO",           new ErrorInfo("TurnTable", "턴테이블 모터 오리진 동작 이상")  },
                { "Servo_M2_0TO",             new ErrorInfo("TurnTable", "PCB 턴 테이블 0도 동작 이상") },
                { "Servo_M2_180TO",           new ErrorInfo("TurnTable", "PCB 턴 테이블 180도 동작 이상") },
                                              
                // 로봇 관련            
                { "RobError",                 new ErrorInfo("Robot", "[로봇 에러]\r로봇 에러 발생") },
                { "RobEResetFail",            new ErrorInfo("Robot", "로봇 모터 가동 실패") },
                { "RobMotorFail",             new ErrorInfo("Robot", "로봇 에러 리셋 실패") },
                { "RobControlBGStoped",       new ErrorInfo("Robot", "로봇 백그라운드 프로그램 정지상태") },
                                              
                // 프리히터 관련
                { "PreheaterPowerOff",       new ErrorInfo("Preheater", "프리히터 전원 OFF 이상") },
                { "Preheater_NotOpen",       new ErrorInfo("Preheater", "프리히터 신호이상\r 프리히터 도어 열림 이상") },

                // EMC 공급함 관련
                { "EMCBoxPickMiss",          new ErrorInfo("", "EMC 박스 픽업 이상") },
                { "EMC_NotSupply",           new ErrorInfo("EMCEmpty", "EMC 공급함 에러\r EMC 도달 감지 이상") },
                { "EMC_CylNotDown",          new ErrorInfo("EMCCylDown", "EMC 공급함 에러\r EMC 개폐실린더 하강 이상") },

                // 프레스 관련
                { "PrsSign1TimeOut",         new ErrorInfo("", "프레스 신호 이상\r 프레스 준비 신호 수신 이상") },
                { "PrsSign2TimeOut",         new ErrorInfo("", "프레스 신호 이상\r 프레스 상승 완료 신호 수신 이상") },
                { "PrsSign3TimeOut",         new ErrorInfo("", "프레스 신호 이상\r 쳄버 닫힘 완료 신호 수신 이상") },
                { "PrsSign4TimeOut",         new ErrorInfo("", "프레스 신호 이상\r 프레스 열림 신호 수신 이상") },
                { "PrsSign5TimeOut",         new ErrorInfo("", "프레스 신호 이상\r 프레스 준비위치 도달 신호 이상") },

                // 로봇 툴 관련
                { "Chuck_Sensor",            new ErrorInfo("ChuckSensor", "툴 교체 센서 이상") },
                { "ElectricGripperError",    new ErrorInfo("EGripper", "전동 그리퍼 동작이상") },
                { "Extract_Table",           new ErrorInfo("ExtractVac", "추출 툴 진공센서 이상\r 턴 테이블 PCB 픽업 이상") },
                { "Extract_Press",           new ErrorInfo("ExtractVac", "추출 툴 진공센서 이상\r 프레스 완제품 픽업 이상") },
                { "Extract_Out",             new ErrorInfo("ExtractVac", "추출 툴 진공센서 이상\r 배출부 이송중 제품 이탈 이상") },
                { "SprayCylDown",            new ErrorInfo("SpraySensor", "이형제 분사 실린더 하강 이상") },
                { "SprayCylUp",              new ErrorInfo("SpraySensor", "이형제 분사 실린더 상승 이상") },
                { "PCBTurnTableError",       new ErrorInfo("", "PCB 턴테이블 동작 이상") },


                // 비전 관련
                { "VisionNotReady",          new ErrorInfo("Vision", "비전 검사 준비 이상\r 비전 프로그램 동작 확인") },
                { "VisionSignal",            new ErrorInfo("Vision", "비전 검사 신호 이상") },
                { "VisionNG_PCB",            new ErrorInfo("VisionPCB", "Vision NG\r 프레스 내 PCB 안착 이상") },
                { "VisionNG_Press",          new ErrorInfo("VisionPress", "Vision NG\r 프레스 이물 검사 이상") },
                { "VisionNG_Table",          new ErrorInfo("VisionTable", "Vision NG\r PCB 로드 테이블 안착 이상") },
                { "VisionNG_Extract",        new ErrorInfo("VisionExtract", "Vision NG\r 완제품 성형 이상") },

            };
        }

        public ErrorInfo GetError(string code)
        {
            if (_errorMap.ContainsKey(code))
            {
                return _errorMap[code];
            }
            // 정의되지 않은 에러 처리
            return new ErrorInfo("", $"에러메세지 미 할당\rCode : {code}");
        }
    }
}