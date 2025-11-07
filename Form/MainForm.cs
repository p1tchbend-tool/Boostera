using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Boostera
{
    public partial class MainForm : Form
    {
        private string ttermproPath = @"C:\Program Files\teraterm5\ttermpro.exe";
        private string ttpmacroPath = @"C:\Program Files\teraterm5\ttpmacro.exe";
        private string winscpPath = @"C:\Program Files (x86)\WinSCP\WinSCP.exe";
        private string boosteraKeyPath = Path.Combine(Program.BoosteraDataFolder, "Boostera.Key");
        private string searchFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private string[] searchExclusionFolders = new string[] {
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) };
        private bool isLogging = true;
        private string logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".Boostera\log");
        private bool isStartUp = true;
        private int modkey = HotKey.MOD_KEY_ALT;
        private Keys key = Keys.T;

        private List<IntPtr> ttermproHwnds = new List<IntPtr>();
        private List<IntPtr> ttermproHwndsTopMost = new List<IntPtr>();
        private List<Index> files = new List<Index>();
        private List<Index> files_shadow = new List<Index>();
        private bool isEnumeration = false;
        private bool isTopMost = false;
        private bool isShowingChildForm = false;
        private int preIndex = ListBox.NoMatches;
        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        private static int? initialWidth = null;
        private static int? initialHeight = null;

        public MainForm()
        {
            if (!Directory.Exists(Program.BoosteraDataFolder)) Directory.CreateDirectory(Program.BoosteraDataFolder);
            if (!Directory.Exists(logFolder))
            {
                try
                {
                    Directory.CreateDirectory(logFolder);
                }
                catch { }
            }

            try
            {
                if (!File.Exists(boosteraKeyPath))
                {
                    if (EncryptedText.CreateKey(boosteraKeyPath))
                    {
                        MessageBox.Show("秘密情報保護用のシークレットが作成されました。\nこれは他の人に共有しないように注意してください。\n\n" +
                            boosteraKeyPath, "Boostera");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            Program.ProgramHotKey.OnHotKey += (s, e) =>
            {
                if (((HotKey.HotKeyEventArgs)e).Id == Program.HotKeyShowForm) ShowSearchForm();
            };

            InitializeComponent();

            if (initialWidth == null) initialWidth = this.Width;
            this.Width = (int)Math.Round((decimal)initialWidth * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));

            if (initialHeight == null) initialHeight = this.Height;
            this.Height = (int)Math.Round((decimal)initialHeight * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));

            this.Shown += (s, e) =>
            {
                textBox1.Text = string.Empty;
                label1.Visible = true;
                listBox1.Visible = false;

                this.Hide();
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;

                using (var settingForm = new SettingForm(ttermproPath, ttpmacroPath, winscpPath, boosteraKeyPath,
                    searchFolder, string.Join(",", searchExclusionFolders), isLogging, logFolder, isStartUp, modkey, key))
                {
                    settingForm.Show();
                    settingForm.Close();
                };
                using (var envForm = new EnvForm(boosteraKeyPath))
                {
                    envForm.Show();
                    envForm.Close();
                }
                using (var connectionForm = new ConnectionForm(ttermproPath, winscpPath, boosteraKeyPath, isLogging, logFolder))
                {
                    connectionForm.Show();
                    connectionForm.Hide();
                    connectionForm.Close();
                }
            };

            listBox1.Left = listBox1.Left - 2;
            listBox1.Width = listBox1.Width + 6;

            myPanel4.Left = myPanel2.Left - 2;
            myPanel4.Top = myPanel2.Top - 2;
            myPanel4.Size = new Size(myPanel2.Width + 4, myPanel2.Height + 4);

            myPanel5.Left = myPanel1.Left - 2;
            myPanel5.Top = myPanel1.Top - 2;
            myPanel5.Size = new Size(myPanel1.Width + 4, myPanel1.Height + 4);

            this.Leave += (s, e) =>
            {
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            };
            this.Deactivate += (s, e) =>
            {
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            };

            notifyIcon1.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                ShowSearchForm();
            };

            var item1 = notifyIcon1.ContextMenuStrip.Items.Add("表示");
            item1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            item1.BackColor = Color.FromArgb(10, 10, 10);
            item1.ForeColor = Color.FromArgb(255, 255, 255);
            item1.Click += (s, e) => ShowSearchForm();

            var item2 = notifyIcon1.ContextMenuStrip.Items.Add("再起動");
            item2.DisplayStyle = ToolStripItemDisplayStyle.Text;
            item2.BackColor = Color.FromArgb(10, 10, 10);
            item2.ForeColor = Color.FromArgb(255, 255, 255);
            item2.Click += (s, e) => Application.Restart();

            var item3 = notifyIcon1.ContextMenuStrip.Items.Add("終了");
            item3.DisplayStyle = ToolStripItemDisplayStyle.Text;
            item3.BackColor = Color.FromArgb(10, 10, 10);
            item3.ForeColor = Color.FromArgb(255, 255, 255);
            item3.Click += (s, e) =>
            {
                var result = MessageBox.Show("終了しますか？", "Boostera", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes) return;

                this.Close();
            };

            var showInExplorerMenuItem = contextMenuStrip2.Items.Add("エクスプローラーで表示する");
            showInExplorerMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            showInExplorerMenuItem.BackColor = Color.FromArgb(31, 31, 31);
            showInExplorerMenuItem.ForeColor = Color.FromArgb(255, 255, 255);
            showInExplorerMenuItem.Click += (s, e) =>
            {
                if (listBox1.SelectedItems.Count == 0) return;

                var psi = new ProcessStartInfo("EXPLORER.EXE");
                psi.Arguments = $@"/select,""{((MyListBoxItem)listBox1.SelectedItems[0]).Path}""";
                psi.UseShellExecute = true;
                try
                {
                    Process.Start(psi);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };

            contextMenuStrip2.Opening += (s, e) =>
            {
                if (listBox1.Cursor != Cursors.Hand) e.Cancel = true;
            };

            listBox1.ContextMenuStrip = contextMenuStrip2;

            toolTip1.Draw += (s, e) =>
            {
                e.DrawBackground();
                e.DrawBorder();
                e.DrawText(TextFormatFlags.WordBreak);
            };

            toolTip1.SetToolTip(panel3, "設定");
            toolTip1.SetToolTip(panel13, "環境変数マネージャー");
            toolTip1.SetToolTip(panel4, "TeraTerm 最小化");
            toolTip1.SetToolTip(panel5, "上下に並べる");
            toolTip1.SetToolTip(panel6, "左右に並べる");
            toolTip1.SetToolTip(panel7, "TeraTerm 起動");
            toolTip1.SetToolTip(panel8, "最前面固定／固定解除");
            toolTip1.SetToolTip(panel9, "格子状に並べる");
            toolTip1.SetToolTip(panel10, "TeraTerm 終了");
            toolTip1.SetToolTip(panel11, "TeraTerm 表示");
            toolTip1.SetToolTip(panel12, "TeraTerm 非表示");
            toolTip1.SetToolTip(panel2, "接続");

            panel2.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                var result = DialogResult.None;
                try
                {
                    isShowingChildForm = true;

                    using (var connectionForm = new ConnectionForm(ttermproPath, winscpPath, boosteraKeyPath, isLogging, logFolder))
                    {
                        result = connectionForm.ShowDialog();
                    }
                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;
                }
                finally
                {
                    isShowingChildForm = false;
                    if (result != DialogResult.OK) ShowSearchForm();
                }
            };

            panel3.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                try
                {
                    isShowingChildForm = true;

                    using (var settingForm = new SettingForm(ttermproPath, ttpmacroPath, winscpPath, boosteraKeyPath,
                        searchFolder, string.Join(",", searchExclusionFolders), isLogging, logFolder, isStartUp, modkey, key))
                    {
                        settingForm.ShowDialog();
                        if (!string.IsNullOrEmpty(settingForm.TtermproPath) && File.Exists(settingForm.TtermproPath)) ttermproPath = settingForm.TtermproPath;
                        if (!string.IsNullOrEmpty(settingForm.TtpmacroPath) && File.Exists(settingForm.TtpmacroPath)) ttpmacroPath = settingForm.TtpmacroPath;
                        if (!string.IsNullOrEmpty(settingForm.WinscpPath) && File.Exists(settingForm.WinscpPath)) winscpPath = settingForm.WinscpPath;
                        if (!string.IsNullOrEmpty(settingForm.BoosteraKeyPath) && File.Exists(settingForm.BoosteraKeyPath)) boosteraKeyPath = settingForm.BoosteraKeyPath;
                        if (!string.IsNullOrEmpty(settingForm.SearchFolder) && Directory.Exists(settingForm.SearchFolder)) searchFolder = settingForm.SearchFolder;
                        if (!string.IsNullOrEmpty(settingForm.SearchExclusionFolders)) searchExclusionFolders = settingForm.SearchExclusionFolders.Split(',');
                        isLogging = settingForm.IsLogging;
                        if (!string.IsNullOrEmpty(settingForm.LogFolder) && Directory.Exists(settingForm.LogFolder)) logFolder = settingForm.LogFolder;
                        isStartUp = settingForm.IsStartUp;
                        modkey = settingForm.ModKey;
                        key = settingForm.Key;

                        try
                        {
                            var settings = new Settings(ttermproPath, ttpmacroPath, winscpPath, boosteraKeyPath,
                                searchFolder, string.Join(",", searchExclusionFolders), isLogging, logFolder, isStartUp, modkey, key);
                            File.WriteAllText(Path.Combine(Program.BoosteraDataFolder, "settings.json"), JsonSerializer.Serialize(settings, jsonSerializerOptions));
                        }
                        catch { }

                        try
                        {
                            var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                            if (isStartUp)
                            {
                                regkey.SetValue(Application.ProductName, $@"""{Application.ExecutablePath}""");
                            }
                            else
                            {
                                regkey.DeleteValue(Application.ProductName, false);
                            }
                            regkey.Close();
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }

                        Task.Run(() =>
                        {
                            try
                            {
                                Environment.SetEnvironmentVariable("BOOSTERA_WinscpPath", winscpPath, EnvironmentVariableTarget.User);
                                Environment.SetEnvironmentVariable("BOOSTERA_IsLogging", isLogging.ToString().ToLower(), EnvironmentVariableTarget.User);
                                Environment.SetEnvironmentVariable("BOOSTERA_LogFolder", logFolder, EnvironmentVariableTarget.User);
                            }
                            catch { }
                        });

                        this.Hide();
                        this.WindowState = FormWindowState.Minimized;
                    }
                }
                finally
                {
                    isShowingChildForm = false;
                    ShowSearchForm();
                }
            };

            panel4.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                var ps = Process.GetProcessesByName("ttermpro");
                for (int i = 0; i < 2; i++)  // 一度では反映されない場合があるため、2回実行
                {
                    for (int j = 0; j < ps.Length; j++)
                    {
                        NativeMethods.ShowWindow(ps[j].MainWindowHandle, NativeMethods.SW_MINIMIZE);
                    }
                }
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            };

            panel5.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                var ps = Process.GetProcessesByName("ttermpro");
                for (int i = 0; i < 2; i++)  // 一度では反映されない場合があるため、2回実行
                {
                    if (ps.Length == 1)
                    {
                        NativeMethods.ShowWindow(ps[0].MainWindowHandle, NativeMethods.SW_RESTORE);

                        var area = Screen.FromPoint(Cursor.Position).WorkingArea;
                        var height = area.Height / 2;
                        NativeMethods.MoveWindow(ps[0].MainWindowHandle, area.Left, area.Top, area.Width, height, 1);

                        NativeMethods.SetForegroundWindow(ps[0].MainWindowHandle);
                    }
                    else
                    {
                        for (int j = 0; j < ps.Length; j++)
                        {
                            NativeMethods.ShowWindow(ps[j].MainWindowHandle, NativeMethods.SW_RESTORE);

                            var area = Screen.FromPoint(Cursor.Position).WorkingArea;
                            var height = area.Height / ps.Length;
                            NativeMethods.MoveWindow(ps[j].MainWindowHandle, area.Left, area.Top + j * height, area.Width, height, 1);

                            NativeMethods.SetForegroundWindow(ps[j].MainWindowHandle);
                        }
                    }
                }
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            };

            panel6.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                var ps = Process.GetProcessesByName("ttermpro");
                for (int i = 0; i < 2; i++)  // 一度では反映されない場合があるため、2回実行
                {
                    if (ps.Length == 1)
                    {
                        NativeMethods.ShowWindow(ps[0].MainWindowHandle, NativeMethods.SW_RESTORE);

                        var area = Screen.FromPoint(Cursor.Position).WorkingArea;
                        var width = area.Width / 2;
                        NativeMethods.MoveWindow(ps[0].MainWindowHandle, area.Left, area.Top, width, area.Height, 1);

                        NativeMethods.SetForegroundWindow(ps[0].MainWindowHandle);
                    }
                    else
                    {
                        for (int j = 0; j < ps.Length; j++)
                        {
                            NativeMethods.ShowWindow(ps[j].MainWindowHandle, NativeMethods.SW_RESTORE);

                            var area = Screen.FromPoint(Cursor.Position).WorkingArea;
                            var width = area.Width / ps.Length;
                            NativeMethods.MoveWindow(ps[j].MainWindowHandle, area.Left + j * width, area.Top, width, area.Height, 1);

                            NativeMethods.SetForegroundWindow(ps[j].MainWindowHandle);
                        }
                    }
                }
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            };

            panel7.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                var psi = new ProcessStartInfo(ttermproPath);
                psi.UseShellExecute = true;
                try
                {
                    Process.Start(psi);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }

                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            };

            panel8.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                isTopMost = !isTopMost;
                panel8.BackgroundImage = isTopMost ? Properties.Resources.clip_gray : Properties.Resources.clip;

                if (!isTopMost)
                {
                    var ps = Process.GetProcessesByName("ttermpro");
                    for (int i = 0; i < 2; i++)  // 一度では反映されない場合があるため、2回実行
                    {
                        for (int j = 0; j < ps.Length; j++)
                        {
                            NativeMethods.SetWindowPos(
                                ps[j].MainWindowHandle, NativeMethods.HWND_NOTOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_SHOWWINDOW);
                        }
                    }
                    ttermproHwndsTopMost = new List<IntPtr>();
                }
            };

            panel9.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                var ps = Process.GetProcessesByName("ttermpro");
                for (int i = 0; i < 2; i++)  // 一度では反映されない場合があるため、2回実行
                {
                    var length = ps.Length;
                    var j = 0;
                    var area = Screen.FromPoint(Cursor.Position).WorkingArea;
                    var width = area.Width / 2;
                    var height = area.Height / 2;

                    while (true)
                    {
                        NativeMethods.ShowWindow(ps[j].MainWindowHandle, NativeMethods.SW_RESTORE);
                        NativeMethods.MoveWindow(ps[j].MainWindowHandle, area.Left, area.Top, width, height, 1);
                        NativeMethods.SetForegroundWindow(ps[j].MainWindowHandle);
                        j++;
                        if (j >= length) break;

                        NativeMethods.ShowWindow(ps[j].MainWindowHandle, NativeMethods.SW_RESTORE);
                        NativeMethods.MoveWindow(ps[j].MainWindowHandle, area.Left + width, area.Top, width, height, 1);
                        NativeMethods.SetForegroundWindow(ps[j].MainWindowHandle);
                        j++;
                        if (j >= length) break;

                        NativeMethods.ShowWindow(ps[j].MainWindowHandle, NativeMethods.SW_RESTORE);
                        NativeMethods.MoveWindow(ps[j].MainWindowHandle, area.Left, area.Top + height, width, height, 1);
                        NativeMethods.SetForegroundWindow(ps[j].MainWindowHandle);
                        j++;
                        if (j >= length) break;

                        NativeMethods.ShowWindow(ps[j].MainWindowHandle, NativeMethods.SW_RESTORE);
                        NativeMethods.MoveWindow(ps[j].MainWindowHandle, area.Left + width, area.Top + height, width, height, 1);
                        NativeMethods.SetForegroundWindow(ps[j].MainWindowHandle);
                        j++;
                        if (j >= length) break;
                    }
                }
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            };

            panel10.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                var ps = Process.GetProcessesByName("ttermpro")
                    .OrderByDescending(p => p.StartTime)
                    .ToArray();

                if (ps.Length != 0)
                {
                    var result = MessageBox.Show("すべて終了しますか？\n※コマンド実行中でも終了します。", "", MessageBoxButtons.YesNo);
                    if (result != DialogResult.Yes) return;

                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;

                    for (int i = 0; i < ps.Length; i++)
                    {
                        uint processId = (uint)ps[i].Id;

                        NativeMethods.EnumWindows((hWnd, lParam) =>
                        {
                            NativeMethods.GetWindowThreadProcessId(hWnd, out uint currentProcessId);
                            if (currentProcessId == processId)
                            {
                                var className = new StringBuilder(256);
                                NativeMethods.GetClassName(hWnd, className, className.Capacity);

                                if (className.ToString() == "VTWin32") NativeMethods.ShowWindow(hWnd, NativeMethods.SW_SHOW);
                            }
                            return true;

                        }, IntPtr.Zero);
                    }

                    Thread.Sleep(1000);

                    for (int i = 0; i < ps.Length; i++)
                    {
                        NativeMethods.PostMessage(ps[i].MainWindowHandle, NativeMethods.WM_ENDTERATERM, IntPtr.Zero, IntPtr.Zero);
                    }
                    ttermproHwnds = new List<IntPtr>();
                }
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            };

            panel11.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                var ps = Process.GetProcessesByName("ttermpro");
                for (int i = 0; i < ps.Length; i++)
                {
                    uint processId = (uint)ps[i].Id;

                    NativeMethods.EnumWindows((hWnd, lParam) =>
                    {
                        NativeMethods.GetWindowThreadProcessId(hWnd, out uint currentProcessId);
                        if (currentProcessId == processId)
                        {
                            var className = new StringBuilder(256);
                            NativeMethods.GetClassName(hWnd, className, className.Capacity);

                            if (className.ToString() == "VTWin32") NativeMethods.ShowWindow(hWnd, NativeMethods.SW_SHOW);
                        }
                        return true;

                    }, IntPtr.Zero);
                }
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            };

            panel12.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                var ps = Process.GetProcessesByName("ttermpro");
                for (int i = 0; i < ps.Length; i++)
                {
                    NativeMethods.ShowWindow(ps[i].MainWindowHandle, NativeMethods.SW_HIDE);
                }
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            };

            panel13.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                try
                {
                    isShowingChildForm = true;

                    using (var envForm = new EnvForm(boosteraKeyPath))
                    {
                        envForm.ShowDialog();
                    }
                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;
                }
                finally
                {
                    isShowingChildForm = false;
                    ShowSearchForm();
                }
            };

            listBox1.MouseLeave += (s, e) => toolTip1.Hide(listBox1);
            listBox1.MouseMove += (s, e) =>
            {
                var index = listBox1.IndexFromPoint(e.Location);
                if (index == ListBox.NoMatches)
                {
                    listBox1.Cursor = Cursors.Default;
                    toolTip1.Hide(listBox1);
                    return;
                }
                else
                {
                    listBox1.Cursor = Cursors.Hand;
                    listBox1.SelectedIndex = index;
                }

                if (index != preIndex)
                {
                    var text = "名前: " + listBox1.Items[index].ToString() +
                        "\r\n場所: " + ((MyListBoxItem)listBox1.Items[index]).Path + "\r\n更新日時: " + ((MyListBoxItem)listBox1.Items[index]).Timestamp.ToString();
                    if (toolTip1.GetToolTip(listBox1) != text) toolTip1.SetToolTip(listBox1, text);
                }
                preIndex = index;
            };

            listBox1.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                if (listBox1.IndexFromPoint(e.Location) == ListBox.NoMatches) return;
                if (listBox1.SelectedItems.Count == 0) return;

                var psi = new ProcessStartInfo(@ttpmacroPath);
                psi.Arguments = '"' + ((MyListBoxItem)listBox1.SelectedItems[0]).Path + '"';
                psi.UseShellExecute = true;
                try
                {
                    Process.Start(psi);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };

            listBox1.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (listBox1.SelectedItems.Count == 0) return;

                    var psi = new ProcessStartInfo(@ttpmacroPath);
                    psi.Arguments = '"' + ((MyListBoxItem)listBox1.SelectedItems[0]).Path + '"';
                    psi.UseShellExecute = true;
                    try
                    {
                        Process.Start(psi);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
                else
                {
                    textBox1.Focus();
                }
            };

            textBox1.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (listBox1.SelectedItems.Count == 0) return;
                    if (string.IsNullOrEmpty(textBox1.Text)) return;

                    var psi = new ProcessStartInfo(@ttpmacroPath);
                    psi.Arguments = '"' + ((MyListBoxItem)listBox1.SelectedItems[0]).Path + '"';
                    psi.UseShellExecute = true;
                    try
                    {
                        Process.Start(psi);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
                else if (e.KeyCode == Keys.Up)
                {
                    e.Handled = true;
                    if (listBox1.Items.Count == 0) return;
                    if (listBox1.SelectedIndex > 0) listBox1.SelectedIndex -= 1;
                }
                else if (e.KeyCode == Keys.Down)
                {
                    e.Handled = true;
                    if (listBox1.Items.Count == 0) return;
                    if (listBox1.SelectedIndex < listBox1.Items.Count - 1) listBox1.SelectedIndex += 1;
                }
            };
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                label1.Visible = true;
                listBox1.Visible = false;
            }
            else
            {
                label1.Visible = false;
                listBox1.Visible = true;
            }

            var searchWords = textBox1.Text.ToLower().Split(new string[] { " ", "　" }, StringSplitOptions.RemoveEmptyEntries);
            var myListBoxItemsMatchName = new List<MyListBoxItem>();
            var myListBoxItemsMatchScriptText = new List<MyListBoxItem>();

            files.ForEach(x =>
            {
                var fileName = Path.GetFileNameWithoutExtension(x.Path);

                if (searchWords.All(y => fileName.ToLower().Contains(y)))
                {
                    myListBoxItemsMatchName.Add(new MyListBoxItem(fileName, x.Path, x.Timestamp));
                    return;
                }
                if (searchWords.All(y => x.Text.ToLower().Contains(y)))
                    myListBoxItemsMatchScriptText.Add(new MyListBoxItem(Path.GetFileNameWithoutExtension(x.Path), x.Path, x.Timestamp));
            });

            listBox1.BeginUpdate();
            listBox1.Items.Clear();
            listBox1.Items.AddRange(myListBoxItemsMatchName.OrderByDescending(x => x.Timestamp).ToArray());
            listBox1.Items.AddRange(myListBoxItemsMatchScriptText.OrderByDescending(x => x.Timestamp).ToArray());
            listBox1.EndUpdate();
            if (listBox1.Items.Count != 0) listBox1.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.TransparencyKey = Color.Green;
            try
            {
                var indexEncrypted = JsonSerializer.Deserialize<EncryptedText>(File.ReadAllText(Path.Combine(Program.BoosteraDataFolder, "search.index")));
                var indexJson = EncryptedText.Decrypt(indexEncrypted, boosteraKeyPath);
                files = JsonSerializer.Deserialize<List<Index>>(indexJson);
            }
            catch { }

            try
            {
                var settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(Path.Combine(Program.BoosteraDataFolder, "settings.json")));
                ttermproPath = settings.TtermproPath;
                ttpmacroPath = settings.TtpmacroPath;
                winscpPath = settings.WinscpPath;
                boosteraKeyPath = settings.BoosteraKeyPath;
                searchFolder = settings.SearchFolder;
                searchExclusionFolders = settings.SearchExclusionFolders.Split(',');
                isLogging = settings.IsLogging;
                logFolder = settings.LogFolder;
                isStartUp = settings.IsStartUp;
                modkey = settings.ModKey;
                key = settings.Key;
            }
            catch { }

            try
            {
                var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (isStartUp)
                {
                    regkey.SetValue(Application.ProductName, $@"""{Application.ExecutablePath}""");
                }
                else
                {
                    regkey.DeleteValue(Application.ProductName, false);
                }
                regkey.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            Task.Run(() =>
            {
                try
                {
                    Environment.SetEnvironmentVariable("BOOSTERA_WinscpPath", winscpPath, EnvironmentVariableTarget.User);
                    Environment.SetEnvironmentVariable("BOOSTERA_IsLogging", isLogging.ToString().ToLower(), EnvironmentVariableTarget.User);
                    Environment.SetEnvironmentVariable("BOOSTERA_LogFolder", logFolder, EnvironmentVariableTarget.User);
                }
                catch { }
            });

            Program.ProgramHotKey.Remove(Program.HotKeyShowForm);
            Program.ProgramHotKey.Add(modkey, key, Program.HotKeyShowForm);

            UiHelper.ChangeFontFamily(this, "メイリオ");
            UiHelper.SortTabIndex(this);
            this.WindowState = FormWindowState.Normal;

            timer1.Start();
            timer2.Start();
            timer3.Start();
            timer4.Start();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            UiHelper.AdjustDpi(this, initialWidth, initialHeight);
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            var ps = Process.GetProcessesByName("ttermpro");
            foreach (Process p in ps)
            {
                if (!ttermproHwnds.Any(x => x == p.MainWindowHandle))
                {
                    var value = 1;
                    NativeMethods.DwmSetWindowAttribute(
                        p.MainWindowHandle, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, (uint)Marshal.SizeOf(typeof(int)));
                    ttermproHwnds.Add(p.MainWindowHandle);
                }

                if (!isTopMost) continue;
                if (!ttermproHwndsTopMost.Any(x => x == p.MainWindowHandle))  // 一度では反映されない場合があるため、2回実行
                {
                    for (int i = 0; i < 2; i++)
                    {
                        NativeMethods.ShowWindow(p.MainWindowHandle, NativeMethods.SW_RESTORE);
                        NativeMethods.SetForegroundWindow(p.MainWindowHandle);
                        NativeMethods.SetWindowPos(
                            p.MainWindowHandle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE);
                    }
                    ttermproHwndsTopMost.Add(p.MainWindowHandle);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((NativeMethods.GetAsyncKeyState((int)Keys.Escape) & 0x8000) != 0)
            {
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            EnumerateAsync();
        }

        private async void EnumerateAsync()
        {
            if (isEnumeration) return;
            isEnumeration = true;
            isEnumeration = await EnumerateTaskAsync();
        }

        private async Task<bool> EnumerateTaskAsync()
        {
            await Task.Run(() =>
            {
                files_shadow = new List<Index>();
                EnumerateFilesRecursively(@searchFolder);

                files = files_shadow;
                try
                {
                    var indexEncrypted = EncryptedText.Encrypt(JsonSerializer.Serialize(files_shadow, jsonSerializerOptions), boosteraKeyPath);
                    var indexJson = JsonSerializer.Serialize(indexEncrypted, jsonSerializerOptions);
                    File.WriteAllText(Path.Combine(Program.BoosteraDataFolder, "search.index"), indexJson);
                }
                catch { }
            });
            return false;
        }

        private void EnumerateFilesRecursively(string path)
        {
            Directory.EnumerateFiles(path).ToList().ForEach(x =>
            {
                try
                {
                    var fileInfo = new FileInfo(x);
                    if ((fileInfo.Attributes & FileAttributes.System) == FileAttributes.System) return;
                    if (Path.GetExtension(x).ToLower() != ".ttl") return;

                    files_shadow.Add(new Index(x, File.ReadAllText(x), fileInfo.LastWriteTime));
                }
                catch { }
            });

            Directory.EnumerateDirectories(path).ToList().ForEach(x =>
            {
                try
                {
                    var directoryInfo = new DirectoryInfo(x);
                    if ((directoryInfo.Attributes & FileAttributes.System) == FileAttributes.System) return;
                    if (searchExclusionFolders?.Length >= 1)
                    {
                        if (searchExclusionFolders.Any(y => y.ToLower() == x.ToLower())) return;
                    };

                    EnumerateFilesRecursively(x);
                }
                catch { }
            });
        }

        private void ShowSearchForm()
        {
            if (isShowingChildForm) return;

            this.Opacity = 0;
            this.WindowState = FormWindowState.Normal;
            this.Left = Screen.FromPoint(Cursor.Position).WorkingArea.Left +
                Screen.FromPoint(Cursor.Position).WorkingArea.Width / 2 - this.Width / 2;
            this.Top = Screen.FromPoint(Cursor.Position).WorkingArea.Height / 8;
            this.Show();
            this.Opacity = 100;

            this.Activate();
            textBox1.Focus();
            this.TopMost = true;

            var forehWnd = NativeMethods.GetForegroundWindow();
            var foreThread = NativeMethods.GetWindowThreadProcessId(forehWnd, IntPtr.Zero);
            var thisThread = NativeMethods.GetCurrentThreadId();
            NativeMethods.AttachThreadInput(thisThread, foreThread, true);

            EnumerateAsync();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
