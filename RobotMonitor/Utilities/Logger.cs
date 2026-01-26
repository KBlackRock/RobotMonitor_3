using System;
using System.IO;

namespace RobotMonitor_3.Utilities
{
    public static class Logger
    {
        // 경로 설정 로직 이동
        private static readonly string LogDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"RobotMonitor_3\Log");
        private static readonly object _lock = new object(); // 파일 충돌 방지

        public static void Write(string message)
        {
            lock (_lock)
            {
                try
                {
                    if (!Directory.Exists(LogDir)) Directory.CreateDirectory(LogDir);

                    string fileName = $"Log_{DateTime.Today:yyyy-MM-dd}.log";
                    string filePath = Path.Combine(LogDir, fileName);
                    string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

                    // AppendText는 내부적으로 Stream을 열고 닫으므로 using 블록 불필요하지만, 명시적으로 처리
                    File.AppendAllText(filePath, logMessage + Environment.NewLine);
                }
                catch
                {
                    // 로깅 실패는 프로그램 중단을 막기 위해 조용히 넘어감 (디버그용)
                }
            }
        }
    }
}