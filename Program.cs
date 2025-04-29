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
                var errLogFolder = Path.Combine(Program.BoosteraDataFolder, "log");
                if (!Directory.Exists(errLogFolder)) Directory.CreateDirectory(errLogFolder);

                File.WriteAllText(
                    Path.Combine(errLogFolder, DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString("N") + ".log"), e.Exception.ToString());
            }
            catch { }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                try
                {
                    var errLogFolder = Path.Combine(Program.BoosteraDataFolder, "log");
                    if (!Directory.Exists(errLogFolder)) Directory.CreateDirectory(errLogFolder);

                    File.WriteAllText(
                        Path.Combine(errLogFolder, DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString("N") + ".log"), ((Exception)e.ExceptionObject).ToString());
                }
                catch { }
            }
            finally { Environment.Exit(1); }
        }
    }
}
