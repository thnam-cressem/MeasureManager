using NLog;
using OpticaNX.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace OpticaNX
{
	/// <summary>
	/// App.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class App : Application
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		private static MainViewModel _mainViewModel; // 메인 윈도우 뷰모델
		/// <summary>
		/// 메인 윈도우 뷰
		/// </summary>
		public static MainWindow MainWindowView
		{
			get
			{
				return Application.Current.MainWindow as MainWindow;
			}
		}

		public static MainViewModel MainWindowViewModel
		{
			get
			{
				return _mainViewModel;
			}
		}

		private void App_Startup(object sender, StartupEventArgs e)
		{
			#region License Check
#if DEBUG
			var licenseCheck = true;
#else
			var licenseCheck = true;
// 			using (Cressem.DongleReadApi.DongleRead dongle = new Cressem.DongleReadApi.DongleRead())
// 			{
// 				if (!dongle.Read_RW("SPC"))
// 				{
// 					MessageBox.Show("The license key could not be found. Only available local.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
// 					System.Diagnostics.Process.GetCurrentProcess().Kill();
// 					return;
// 				}
// 			}
#endif
			#endregion

			AppDomain.CurrentDomain.FirstChanceException += new EventHandler<FirstChanceExceptionEventArgs>(CurrentDomain_FirstChanceException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			// 메인 윈도우 활성
			MainWindow = new OpticaNX.MainWindow();
			MainWindow.Show();

			_mainViewModel = (MainViewModel)MainWindow.DataContext;

			//_mainWindowViewModel.LicenseCheck = licenseCheck;
		}

		// Occurs when an exception is thrown in managed code, before the runtime searches the call stack for an exception handler in the application domain.
		private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
		{
			// TODO: log every exception
			//Console.WriteLine("FirstChangeException event raised in {0}: {1}", AppDomain.CurrentDomain.FriendlyName, e.Exception.Message);
			//_logger.Error("FirstChanceException: {0}", DebugHelper.ExceptionToString(e.Exception));
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			_logger.Error("Unhandled thread exception: {0}", ex.ToString());

			string msg = "Unhandled thread exception occurred." + String.Format("\r\n\r\nOriginal error: {0}", ex.Message);
			MessageBox.Show("Uncaught Thread Exception" + msg, msg);
			Application.Current.Shutdown();
		}

		// Handler for any UI exception
		private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			var comException = e.Exception as System.Runtime.InteropServices.COMException;

			// Clipboard에 데이터 넣을때 발생, WPF 자체 문제점으로 알려짐.
			// System.Runtime.InteropServices.COMException (0x800401D0): OpenClipboard Failed (Exception from HRESULT: 0x800401D0 (CLIPBRD_E_CANT_OPEN))
			if (comException != null && comException.ErrorCode == -2147221040)
			{
				e.Handled = true;
			}
			else
			{
				// Process unhandled UI exception
				_logger.Error("Unhandled UI exception: {0}", e.Exception.ToString());

				string msg = "Unhandled UI exception occurred." + String.Format("\r\n\r\nOriginal error: {0}", e.Exception.Message);
				MessageBox.Show("Uncaught UI Exception" + msg, msg);
				Application.Current.Shutdown();

				// Prevent default unhandled exception processing
				e.Handled = true;
			}
		}

		// Occurs when the user ends the Windows session by logging off or shutting down the operating system.
		private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
		{
			// Ask the user if they want to allow the session to end
			var msg = String.Format("{0}. End session?", e.ReasonSessionEnding);
			//var result = MessageBox.Show(msg);

			//// End session, if specified
			//if (result == false)
			//{
			//	e.Cancel = true;
			//}
		}

		// Occurs when the application exists.
		private void App_Exit(object sender, ExitEventArgs e)
		{
			this.Resources = null;

			_logger.Info("Application has been shut down.");
		}
	}
}
