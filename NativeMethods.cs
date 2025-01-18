using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Boostera
{
    static class NativeMethods
    {
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int pvAttribute, uint cbAttribute);
        public static readonly int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, int bRepaint);

        public static readonly int SW_MINIMIZE = 6;
        public static readonly int SW_RESTORE = 9;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hWnd, int Msg);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public static readonly int SWP_NOSIZE = 0x0001;
        public static readonly int SWP_NOMOVE = 0x0002;
        public static readonly int SWP_SHOWWINDOW = 0x0040;

        public static readonly int HWND_TOPMOST = -1;
        public static readonly int HWND_NOTOPMOST = -2;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        public static readonly uint WM_ENDTERATERM = 0x0428;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int RegisterHotKey(IntPtr HWnd, int ID, int MOD_KEY, Keys KEY);

        [DllImport("user32.dll")]
        public static extern int UnregisterHotKey(IntPtr HWnd, int ID);

        [DllImport("User32.dll")]
        public static extern int GetDpiForSystem();
    }
}
