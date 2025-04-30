using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Boostera
{
    internal static class Program
    {
        public static readonly string BoosteraDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Boostera");
        public static readonly HotKey ProgramHotKey = new HotKey();
        public static readonly int HotKeyShowForm = 1;
        public static readonly Logger Logger = new Logger(Path.Combine(BoosteraDataFolder, "log"), "Boostera.log", 10000);

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            NativeMethods.SetProcessDpiAwarenessContext(NativeMethods.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE);

            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                Logger.LogException(e.Exception);
            }
            catch { }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                try
                {
                    Logger.LogException((Exception)e.ExceptionObject);
                }
                catch { }
            }
            finally { Environment.Exit(1); }
        }
    }
}
