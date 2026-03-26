using System;
using System.Collections.Generic;

namespace RobotMonitor_3.Services
{
    public struct ParsedMessage
    {
        public string Key { get; set; }  
        public string Value { get; set; } 
    }

    public class RobotMessageParser
    {
        public IEnumerable<ParsedMessage> Parse(string rawMsg)
        {
            if (string.IsNullOrEmpty(rawMsg) || rawMsg.Length < 4)
                yield break; // 유효성 검사 유지

            // 콤마(,)로 다중 명령 분리
            string[] commands = rawMsg.Split(',');

            foreach (var cmd in commands)
            {
                if (string.IsNullOrWhiteSpace(cmd) || cmd.Length < 2) continue;

                // 언더바(_)로 Key와 Value 분리
                int separatorIndex = cmd.IndexOf('_');

                if (separatorIndex > 0)
                {
                    string key = cmd.Substring(0, separatorIndex); // "State"
                    string value = cmd.Substring(separatorIndex + 1); // "Ready"

                    yield return new ParsedMessage { Key = key, Value = value };
                }
            }
        }
    }
}