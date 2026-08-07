using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RobotMonitor_3.ViewModels;

namespace RobotMonitor_3
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var splash = new SplashWindow();
            splash.Show();

            IProgress<LoadStep> progress = new Progress<LoadStep>(s => splash.SetStatus(s));

            MainWindow main = null;
            try
            {
                progress.Report(new LoadStep("설정 파일을 읽는 중...", 5));

                // 이 시점에 ViewModel 생성자(XML 로드)가 실행됨
                main = new MainWindow();
                MainWindow = main;

                var vm = main.DataContext as MainWindow_ViewModel;
                if (vm == null) throw new InvalidOperationException("DataContext 설정이 확인되지 않았습니다.");

                await vm.OpeningAsync(progress);

                await Task.Delay(300);      // 완료 문구를 잠깐 보여주는 용도, 불필요하면 삭제

                ShutdownMode = ShutdownMode.OnMainWindowClose;
                main.Show();
            }
            catch (Exception ex)
            {
                splash.Close();
                MessageBox.Show($"프로그램 초기화에 실패했습니다.\n\n{ex.Message}",
                                "시작 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
                return;
            }
            finally
            {
                if (splash.IsVisible) splash.Close();
            }
        }
    }
}