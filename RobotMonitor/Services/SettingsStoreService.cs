using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using RobotMonitor_3.Models;

namespace RobotMonitor_3.Services
{
    public static class SettingsStore
    {
        private static readonly string Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MRS3", "settings.json");

        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true
        };

        public static SettingDatas Current { get; private set; } = new();

        public static void Load()
        {
            try
            {
                if (File.Exists(Path))
                    Current = JsonSerializer.Deserialize<SettingDatas>(
                        File.ReadAllText(Path), Options) ?? new SettingDatas();
            }
            catch { Current = new SettingDatas(); }   // 파일 손상 시 기본값
        }

        public static void Save()
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path)!);
            var tmp = Path + ".tmp";
            File.WriteAllText(tmp, JsonSerializer.Serialize(Current, Options));
            File.Copy(tmp, Path, true);             // 쓰기 중 종료 시 원본 보호
            File.Delete(tmp);
        }
    }
}
