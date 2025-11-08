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

        private static Mutex mutex = null;
        private static bool hasMutex = false;

        [STAThread]
        static void Main()
        {
            NativeMethods.SetProcessDpiAwarenessContext(NativeMethods.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE);

            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            mutex = new Mutex(true, Application.ProductName, out hasMutex);
            if (!hasMutex)
            {
                MessageBox.Show($@"{Application.ProductName} は既に起動中です。");
                return;
            }

            try
            {
                Application.Run(new MainForm());
            }
            finally
            {
                if (hasMutex) mutex.ReleaseMutex();
            }
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
            finally
            {
                if (hasMutex) mutex.ReleaseMutex();
                Environment.Exit(1);
            }
        }
    }
}
