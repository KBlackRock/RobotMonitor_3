using RobotMonitor_3.Utilities;
using RobotMonitor_3.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RobotMonitor_3.Services
{
    /// <summary>
    /// Mitsubishi PLC 통신을 위한 MC 프로토콜 (3E Frame, Binary)
    /// </summary>
    public class McProtocolService : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;

        private readonly MainWindow_ViewModel _viewModel;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // 동시 접근 제어용 세마포어
        private bool _disposed = false;

        public bool IsConnected => _client != null && _client.Connected;

        /// <summary>
        /// PLC와 TCP 연결 수행
        /// </summary>
        public async Task<bool> ConnectAsync(string ip, int port)
        {
            await _semaphore.WaitAsync(); // 연결 시도 중 다른 작업이 접근하지 못하도록 세마포어로 제어
            try
            {
                if (IsConnected) return true; // 이미 연결되어 있으면 바로 성공 반환

                _client = new TcpClient();
                await _client.ConnectAsync(ip, port);
                _stream = _client.GetStream();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Write($"PLC 연결 실패: {ex.Message}");
                return false;
            }
            finally
            {
                _semaphore.Release(); // 연결 시도 완료 후 세마포어 해제
            }
        }

        /// <summary>
        /// PLC와의 연결 종료
        /// </summary>
        public void Disconnect()
        {
            _semaphore.Wait(); // 연결 종료 중 다른 작업이 접근하지 못하도록 세마포어로 제어
            try
            {
                _stream?.Close();
                _client?.Close();
                _stream = null;
                _client = null;
            }
            finally
            {
                _semaphore.Release(); // 연결 종료 완료 후 세마포어 해제
            }

        }

        /// <summary>
        /// 지정한 디바이스(예: D, M)의 데이터를 연속으로 수신
        /// </summary>
        /// <param name="deviceCode">디바이스 종류 (예: "D", "M")</param>
        /// <param name="headDevice">시작 디바이스 번호</param>
        /// <param name="readCount">읽을 워드(Word) 점수</param>
        /// <returns>읽어온 바이트 배열 데이터 (실패 시 null)</returns>
        public async Task<byte[]> ReadDeviceAsync(string deviceCode, int headDevice, ushort readCount)
        {
            if (!IsConnected) throw new InvalidOperationException("PLC에 연결되어 있지 않습니다.");

            int expectedResponseLength = 11 + (readCount * 2); // 응답 데이터 길이 계산 (11바이트 헤더 + 읽을 데이터 길이)

            await _semaphore.WaitAsync(); // 통신 세마포어 제어 ( 다른 요청 대기 )

            try
            {
                // 3E Frame 커맨드 구성 (바이너리)
                List<byte> command = new List<byte>();

                command.AddRange(BitConverter.GetBytes((ushort)0x0050)); // 서브헤더 (50 00)
                command.Add(0x00);                                       // 네트워크 번호
                command.Add(0xFF);                                       // PC 번호
                command.AddRange(BitConverter.GetBytes((ushort)0x03FF)); // 요구 대상 모듈 I/O 번호
                command.Add(0x00);                                       // 요구 대상 모듈 국번

                command.AddRange(BitConverter.GetBytes((ushort)0x000C)); // 이후의 데이터 길이 (12바이트)
                command.AddRange(BitConverter.GetBytes((ushort)0x0010)); // CPU 감시 타이머

                command.AddRange(BitConverter.GetBytes((ushort)0x0401)); // 커맨드: 일괄 읽기 (01 04)
                command.AddRange(BitConverter.GetBytes((ushort)0x0000)); // 서브 커맨드: 워드 단위 (00 00)

                // 시작 디바이스 번호 (3 Byte)
                byte[] headBytes = BitConverter.GetBytes(headDevice);
                command.Add(headBytes[0]);
                command.Add(headBytes[1]);
                command.Add(headBytes[2]);

                // 디바이스 코드 지정
                command.Add(GetDeviceCode(deviceCode));

                // 디바이스 점수
                command.AddRange(BitConverter.GetBytes(readCount));

                byte[] request = command.ToArray();

                // 데이터 송신
                await _stream.WriteAsync(request, 0, request.Length);

                // 데이터 수신
                byte[] responseBuffer = new byte[expectedResponseLength];
                bool isReadSuccess = await ReadExactBytesAsync(responseBuffer, expectedResponseLength);

                if (!isReadSuccess)
                {
                    Logger.Write("PLC 읽기 에러: 수신된 데이터가 부족하거나 연결이 끊겼습니다.");
                    return null;
                }

                // 종료 코드(0x0000) 정상 여부 확인
                if (responseBuffer[9] == 0x00 && responseBuffer[10] == 0x00)
                {
                    int dataLength = expectedResponseLength - 11;
                    byte[] data = new byte[dataLength];
                    Array.Copy(responseBuffer, 11, data, 0, dataLength);
                    return data;
                }

                // 여기 까지 오면 에러
                Logger.Write($"PLC 읽기 에러. 종료 코드: {responseBuffer[9]:X2}{responseBuffer[10]:X2}");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Write($"PLC ReadDevice 예외 발생: {ex.Message}");
                return null;
            }
            finally
            {
                _semaphore.Release();
            }
        }


        /// <summary>
        /// 지정한 디바이스(예: D, M)에 워드 데이터를 연속으로 송신
        /// </summary>
        /// <param name="deviceCode">디바이스 종류 (예: "D", "M")</param>
        /// <param name="headDevice">시작 디바이스 번호</param>
        /// <param name="writeData">쓸 워드 데이터 배열 (2바이트씩 1워드)</param>
        /// <returns>쓰기 성공 여부</returns>
        public async Task<bool> WriteDeviceAsync(string deviceCode, int headDevice, short[] writeData)
        {
            if (!IsConnected) throw new InvalidOperationException("PLC에 연결되어 있지 않습니다.");

            ushort writeCount = (ushort)writeData.Length;
            // 요청 데이터 길이 계산 = 12바이트(커맨드부터 점수까지) + 쓸 데이터 길이(워드 개수 * 2바이트)
            ushort dataLength = (ushort)(12 + (writeCount * 2));
            int expectedResponseLength = 11; // 쓰기 응답은 11바이트 (헤더 + 종료 코드)

            await _semaphore.WaitAsync(); // 통신 세마포어 제어 ( 다른 요청 대기 )
            try
            {

                List<byte> command = new List<byte>();

                command.AddRange(BitConverter.GetBytes((ushort)0x0050)); // 서브헤더 (50 00)
                command.Add(0x00);                                       // 네트워크 번호
                command.Add(0xFF);                                       // PC 번호
                command.AddRange(BitConverter.GetBytes((ushort)0x03FF)); // 요구 대상 모듈 I/O 번호
                command.Add(0x00);                                       // 요구 대상 모듈 국번

                command.AddRange(BitConverter.GetBytes(dataLength));     // 이후의 데이터 길이
                command.AddRange(BitConverter.GetBytes((ushort)0x0010)); // CPU 감시 타이머

                command.AddRange(BitConverter.GetBytes((ushort)0x1401)); // 커맨드: 일괄 쓰기 (01 14)
                command.AddRange(BitConverter.GetBytes((ushort)0x0000)); // 서브 커맨드: 워드 단위 (00 00)

                // 시작 디바이스 번호 (3 Byte)
                byte[] headBytes = BitConverter.GetBytes(headDevice);
                command.Add(headBytes[0]);
                command.Add(headBytes[1]);
                command.Add(headBytes[2]);

                // 디바이스 코드 지정
                command.Add(GetDeviceCode(deviceCode));

                // 디바이스 점수
                command.AddRange(BitConverter.GetBytes(writeCount));

                // 쓸 데이터 순차적으로 추가
                foreach (short data in writeData)
                {
                    command.AddRange(BitConverter.GetBytes(data));
                }

                byte[] request = command.ToArray();

                // 데이터 송신
                await _stream.WriteAsync(request, 0, request.Length);

                // 데이터 수신 (정상 처리 여부 확인용 응답)
                byte[] responseBuffer = new byte[expectedResponseLength];
                bool isReadSuccess = await ReadExactBytesAsync(responseBuffer, expectedResponseLength);

                if (!isReadSuccess)
                {
                    Logger.Write("PLC 쓰기 응답 에러: 수신된 데이터가 부족하거나 연결이 끊겼습니다.");
                    return false;
                }

                // 쓰기 응답 데이터 최소 길이(11바이트) 및 종료 코드(0x0000) 정상 여부 확인
                if (responseBuffer[9] == 0x00 && responseBuffer[10] == 0x00)
                {
                    return true;
                }

                // 에러 발생 시 로깅
                Logger.Write($"PLC 쓰기 에러. 종료 코드: {responseBuffer[9]:X2}{responseBuffer[10]:X2}");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Write($"PLC WriteDevice 예외 발생: {ex.Message}");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 네트워크 스트림에서 정확한 바이트 수만큼 데이터를 읽어오는 헬퍼 메서드 (단편화 방지)
        /// </summary>
        private async Task<bool> ReadExactBytesAsync(byte[] buffer, int length)
        {
            int totalBytesRead = 0;
            while (totalBytesRead < length)
            {
                int bytesRead = await _stream.ReadAsync(buffer, totalBytesRead, length - totalBytesRead);
                // 0을 반환하면 소켓 연결이 끊어진 것
                if (bytesRead == 0) return false;
                totalBytesRead += bytesRead;
            }
            return true;
        }

        private byte GetDeviceCode(string type)
        {
            switch (type.ToUpper())
            {
                case "D": return 0xA8;
                case "M": return 0x90;
                case "W": return 0xB4;
                default: throw new ArgumentException("지원하지 않는 디바이스 타입입니다.");
            }
        }

        /// <summary>
        /// IDisposable 구현 (자원 해제)
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                Disconnect();
                _semaphore?.Dispose();
                _disposed = true;
            }
        }

    }
}