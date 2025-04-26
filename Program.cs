using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Boostera
{
    internal static class Program
    {
        public static readonly string BoosteraDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Boostera");
        public static readonly HotKey ProgramHotKey = new HotKey();
        public static readonly int HotKeyShowForm = 1;

        public static void SortTabIndex(Control control)
        {
            var children = new List<Control>();
            foreach (Control child in control.Controls) children.Add(child);
            children.Sort((x, y) =>
            {
                if (x.Top == y.Top) return x.Left.CompareTo(y.Left);
                return x.Top.CompareTo(y.Top);
            });
            for (int i = 0; i < children.Count; i++) children[i].TabIndex = i;
        }

        public static void ChangeFont(Control control)
        {
            try
            {
                Install(control);
                void Install(Control ctrl)
                {
                    foreach (Control c in ctrl.Controls)
                    {
                        c.Font = new Font("メイリオ", c.Font.SizeInPoints, c.Font.Style);
                        Install(c);
                    }
                }
            }
            catch { }
        }

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
