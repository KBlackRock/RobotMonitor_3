using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RobotMonitor_3.Services
{
    public class TcpServerService
    {
        private TcpListener _listener;
        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cts; // 안전한 종료를 위한 토큰

        // 뷰모델에 상황을 알려줄 이벤트들
        public event Action<string> OnDataReceived;  // 데이터 수신 시
        public event Action<string> OnErrorOccurred; // 에러 발생 시
        public event Action<bool> OnConnectionStatusChanged; // 연결 상태 변경 시

        public bool IsRunning { get; private set; }
        public bool IsConnected => _client != null && _client.Connected;

        // 서버 시작
        public async void StartServer(string ip, int port)
        {
            if (IsRunning) return;

            try
            {
                _cts = new CancellationTokenSource();
                IPAddress localAddr = IPAddress.Parse(ip);
                _listener = new TcpListener(localAddr, port);
                _listener.Start();

                IsRunning = true;
                // 클라이언트 대기 시작 (비동기)
                await AcceptClientsAsync(_cts.Token);
            }
            catch (Exception ex)
            {
                OnErrorOccurred?.Invoke($"서버 시작 실패: {ex.Message}");
                StopServer();
            }
        }

        // 클라이언트 접속 대기 루프
        private async Task AcceptClientsAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    // 클라이언트 접속 대기
                    TcpClient tempClient = await _listener.AcceptTcpClientAsync(token);

                    // 기존 연결이 있다면 끊고 새로 연결 (1:1 통신)
                    if (_client != null)
                    {
                        _client.Close();
                    }

                    _client = tempClient;
                    _stream = _client.GetStream();

                    OnConnectionStatusChanged?.Invoke(true); // 연결됨 알림

                    // 데이터 수신 루프 시작
                    _ = ReceiveDataAsync(token);
                }
            }
            catch (OperationCanceledException) { /* 정상 종료 */ }
            catch (Exception ex)
            {
                if (IsRunning) OnErrorOccurred?.Invoke($"접속 대기 오류: {ex.Message}");
            }
        }

        // 데이터 수신 루프
        private async Task ReceiveDataAsync(CancellationToken token)
        {
            byte[] buffer = new byte[1024];
            try
            {
                while (!token.IsCancellationRequested && IsConnected)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);

                    if (bytesRead == 0) break; // 연결 끊김

                    string msg = Encoding.Default.GetString(buffer, 0, bytesRead);
                    OnDataReceived?.Invoke(msg); // 뷰모델로 데이터 전달
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                OnErrorOccurred?.Invoke($"수신 오류: {ex.Message}");
            }
            finally
            {
                // 루프 탈출 시 연결 종료 처리
                OnConnectionStatusChanged?.Invoke(false);
            }
        }

        // 데이터 송신
        public async Task SendAsync(string message)
        {
            if (!IsConnected || _stream == null) return;

            try
            {
                byte[] data = Encoding.Default.GetBytes(message);
                await _stream.WriteAsync(data, 0, data.Length);
                await _stream.FlushAsync();
            }
            catch (Exception ex)
            {
                OnErrorOccurred?.Invoke($"송신 오류: {ex.Message}");
            }
        }

        // 서버 종료
        public void StopServer()
        {
            if (!IsRunning && _cts == null) return;   
            IsRunning = false;

            try { _cts?.Cancel(); } catch { }        
            try { _listener?.Stop(); } catch { }      
            try { _stream?.Dispose(); } catch { }
            try { _client?.Close(); } catch { }

            _cts?.Dispose();
            _cts = null; _stream = null; _client = null; _listener = null;

            OnConnectionStatusChanged?.Invoke(false);
        }
    }
}