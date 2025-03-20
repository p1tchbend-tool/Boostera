using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Boostera
{
    public partial class Form3 : Form
    {
        private string ttermproPath = @"C:\Program Files (x86)\teraterm5\ttermpro.exe";
        private string winscpPath = @"C:\Program Files (x86)\WinSCP\WinSCP.exe";
        private string boosteraKeyPath = Path.Combine(Program.BoosteraDataFolder, "Boostera.Key");
        private string boosteraMacroFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".Boostera");
        private string sshFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh");
        private List<History> histories = new List<History>();
        private int preIndex = ListBox.NoMatches;
        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        private static readonly int MAX_HISTORIES_COUNT = 10000;
        private static readonly int SSH = 0;
        private static readonly int RDP = 1;
        private static readonly int SFTP = 2;

        private static int? initialWidth = null;
        private static int? initialHeight = null;

        public Form3(string ttermproPath, string winscpPath, string boosteraKeyPath)
        {
            InitializeComponent();

            if (initialWidth == null) initialWidth = this.Width;
            this.Width = (int)Math.Round((decimal)initialWidth * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));

            if (initialHeight == null) initialHeight = this.Height;
            this.Height = (int)Math.Round((decimal)initialHeight * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));

            this.Shown += (s, e) => textBox13.Focus();

            this.ttermproPath = ttermproPath;
            this.winscpPath = winscpPath;
            this.boosteraKeyPath = boosteraKeyPath;
            comboBox2.SelectedIndex = 0;

            toolTip1.SetToolTip(comboBox2, "接続先のプロトコルを選択してください。\nSSHはTeraTerm、SFTPはWinSCPの事前設定が必要です。");
            toolTip1.SetToolTip(textBox5, "接続先のホストを入力してください。");
            toolTip1.SetToolTip(textBox4, "接続先のユーザーを入力してください。");
            toolTip1.SetToolTip(textBox1, "接続先のポートを入力してください。");
            toolTip1.SetToolTip(textBox3, "接続先の秘密鍵のパスを入力してください。");
            toolTip1.SetToolTip(textBox2, "接続先のパスワード／パスフレーズを入力してください。");
            toolTip1.SetToolTip(textBox12, "接続時に実行するコマンドを入力してください。");
            toolTip1.SetToolTip(checkBox3, "SSHトンネリングを使用する場合、チェックを入れてください。");
            toolTip1.SetToolTip(textBox6, "SSHトンネリングで利用するホストを入力してください。");
            toolTip1.SetToolTip(textBox10, "SSHトンネリングで利用するユーザーを入力してください。");
            toolTip1.SetToolTip(textBox9, "SSHトンネリングで利用するポートを入力してください。");
            toolTip1.SetToolTip(textBox7, "SSHトンネリングで利用する秘密鍵のパスを入力してください。");
            toolTip1.SetToolTip(textBox8, "SSHトンネリングで利用するパスワード／パスフレーズを入力してください。");
            toolTip1.SetToolTip(checkBox1, "SSHトンネリングで利用する接続先を、非表示のウィンドウで起動する場合、チェックを入れてください。\n" +
                "非表示で起動したウィンドウは、Boosteraの機能で再表示及び終了可能です。");
            toolTip1.SetToolTip(textBox11, "ヒストリーの検索や、エクスポート時のファイル名に使用するタグを入力してください。");
            toolTip1.SetToolTip(button2, "現在の接続設定をTTLマクロとしてエクスポートします。");
            toolTip1.SetToolTip(button3, "TTLマクロをこの画面に読み込みます。\nBoosteraでエクスポートしたマクロのみ、インポート可能です。");
            toolTip1.SetToolTip(button1, "現在の設定でホストに接続します。");

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
                    var text = listBox1.Items[index].ToString();
                    if (toolTip1.GetToolTip(listBox1) != text) toolTip1.SetToolTip(listBox1, text);
                }
                preIndex = index;
            };

            listBox1.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                if (listBox1.IndexFromPoint(e.Location) == ListBox.NoMatches) return;
                if (listBox1.SelectedItems.Count == 0) return;

                var history = (History)listBox1.SelectedItems[0];
                comboBox2.SelectedIndex = history.Protocol;
                textBox5.Text = history.Host;
                textBox4.Text = history.User;
                textBox1.Text = history.Port;
                textBox3.Text = history.PrivateKey;
                textBox2.Text = history.Password;
                textBox12.Text = history.LogonScript;
                checkBox3.Checked = history.IsForwarding;
                textBox6.Text = history.ForwardingHost;
                textBox10.Text = history.ForwardingUser;
                textBox9.Text = history.ForwardingPort;
                textBox7.Text = history.ForwardingPrivateKey;
                textBox8.Text = history.ForwardingPassword;
                checkBox1.Checked = history.IsHide;
                textBox11.Text = history.Tag;

                textBox13.Text = string.Empty;
                listBox1.Visible = false;
                textBox13.Focus();
            };

            listBox1.PreviewKeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (listBox1.SelectedItems.Count == 0) return;

                    var history = (History)listBox1.SelectedItems[0];
                    comboBox2.SelectedIndex = history.Protocol;
                    textBox5.Text = history.Host;
                    textBox4.Text = history.User;
                    textBox1.Text = history.Port;
                    textBox3.Text = history.PrivateKey;
                    textBox2.Text = history.Password;
                    textBox12.Text = history.LogonScript;
                    checkBox3.Checked = history.IsForwarding;
                    textBox6.Text = history.ForwardingHost;
                    textBox10.Text = history.ForwardingUser;
                    textBox9.Text = history.ForwardingPort;
                    textBox7.Text = history.ForwardingPrivateKey;
                    textBox8.Text = history.ForwardingPassword;
                    checkBox1.Checked = history.IsHide;
                    textBox11.Text = history.Tag;
                }
            };

            textBox13.PreviewKeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (listBox1.SelectedItems.Count == 0) return;
                    if (string.IsNullOrEmpty(textBox13.Text)) return;

                    var history = (History)listBox1.SelectedItems[0];
                    comboBox2.SelectedIndex = history.Protocol;
                    textBox5.Text = history.Host;
                    textBox4.Text = history.User;
                    textBox1.Text = history.Port;
                    textBox3.Text = history.PrivateKey;
                    textBox2.Text = history.Password;
                    textBox12.Text = history.LogonScript;
                    checkBox3.Checked = history.IsForwarding;
                    textBox6.Text = history.ForwardingHost;
                    textBox10.Text = history.ForwardingUser;
                    textBox9.Text = history.ForwardingPort;
                    textBox7.Text = history.ForwardingPrivateKey;
                    textBox8.Text = history.ForwardingPassword;
                    checkBox1.Checked = history.IsHide;
                    textBox11.Text = history.Tag;
                }
                else if (e.KeyCode == Keys.Up)
                {
                    if (listBox1.Items.Count == 0) return;
                    if (listBox1.SelectedIndex > 0) listBox1.SelectedIndex -= 1;
                }
                else if (e.KeyCode == Keys.Down)
                {
                    if (listBox1.Items.Count == 0) return;
                    if (listBox1.SelectedIndex < listBox1.Items.Count - 1) listBox1.SelectedIndex += 1;
                }
            };

            textBox13.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    e.Handled = true;
                }
            };

            try
            {
                var historyEncrypted = JsonSerializer.Deserialize<HistoryEncrypted>(File.ReadAllText(Path.Combine(Program.BoosteraDataFolder, "history.json")));
                var historyJson = HistoryEncrypted.DecryptData(historyEncrypted, boosteraKeyPath);
                histories = JsonSerializer.Deserialize<List<History>>(historyJson);
            }
            catch { }

            listBox1.BeginUpdate();
            histories.ForEach(x => listBox1.Items.Add(x));
            listBox1.EndUpdate();

            if (listBox1.Items.Count != 0)
            {
                var history = (History)listBox1.Items[0];
                if (history == null) return;

                comboBox2.SelectedIndex = history.Protocol;
                textBox5.Text = history.Host;
                textBox4.Text = history.User;
                textBox1.Text = history.Port;
                textBox3.Text = history.PrivateKey;
                textBox2.Text = history.Password;
                textBox12.Text = history.LogonScript;
                checkBox3.Checked = history.IsForwarding;
                textBox6.Text = history.ForwardingHost;
                textBox10.Text = history.ForwardingUser;
                textBox9.Text = history.ForwardingPort;
                textBox7.Text = history.ForwardingPrivateKey;
                textBox8.Text = history.ForwardingPassword;
                checkBox1.Checked = history.IsHide;
                textBox11.Text = history.Tag;

                listBox1.SelectedIndex = 0;
            }

            comboBox2.SelectedIndexChanged += (s, e) =>
            {
                if (comboBox2.SelectedIndex == SSH)
                {
                    textBox1.Text = "22";
                    label5.Enabled = true;
                    textBox3.Enabled = true;
                    panel1.Enabled = true;
                    panel3.Enabled = true;
                    label14.Enabled = true;
                    textBox12.Enabled = true;
                }
                else if (comboBox2.SelectedIndex == RDP)
                {
                    textBox1.Text = "3389";
                    label5.Enabled = false;
                    textBox3.Enabled = false;
                    panel1.Enabled = false;
                    panel3.Enabled = false;
                    label14.Enabled = false;
                    textBox12.Enabled = false;
                }
                else if (comboBox2.SelectedIndex == SFTP)
                {
                    textBox1.Text = "22";
                    label5.Enabled = true;
                    textBox3.Enabled = true;
                    panel1.Enabled = true;
                    panel3.Enabled = true;
                    label14.Enabled = false;
                    textBox12.Enabled = false;
                }
            };

            panel1.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                textBox3.UseSystemPasswordChar = !textBox3.UseSystemPasswordChar;
                panel1.BackgroundImage = textBox3.UseSystemPasswordChar ? Properties.Resources.eye_show : Properties.Resources.eye_hide;
            };

            panel2.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                textBox2.UseSystemPasswordChar = !textBox2.UseSystemPasswordChar;
                panel2.BackgroundImage = textBox2.UseSystemPasswordChar ? Properties.Resources.eye_show : Properties.Resources.eye_hide;
            };

            panel3.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                using (var openFileDialog = new OpenFileDialog())
                {
                    if (!Directory.Exists(sshFolder)) Directory.CreateDirectory(sshFolder);
                    openFileDialog.InitialDirectory = sshFolder;

                    if (openFileDialog.ShowDialog() == DialogResult.OK) textBox3.Text = openFileDialog.FileName;
                }
            };

            panel6.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                textBox7.UseSystemPasswordChar = !textBox7.UseSystemPasswordChar;
                panel6.BackgroundImage = textBox7.UseSystemPasswordChar ? Properties.Resources.eye_show : Properties.Resources.eye_hide;
            };

            panel5.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                textBox8.UseSystemPasswordChar = !textBox8.UseSystemPasswordChar;
                panel5.BackgroundImage = textBox8.UseSystemPasswordChar ? Properties.Resources.eye_show : Properties.Resources.eye_hide;
            };

            panel4.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                using (var openFileDialog = new OpenFileDialog())
                {
                    if (!Directory.Exists(sshFolder)) Directory.CreateDirectory(sshFolder);
                    openFileDialog.InitialDirectory = sshFolder;

                    if (openFileDialog.ShowDialog() == DialogResult.OK) textBox7.Text = openFileDialog.FileName;
                }
            };

            toolTip1.Draw += (s, e) =>
            {
                e.DrawBackground();
                e.DrawBorder();
                e.DrawText(TextFormatFlags.WordBreak);
            };

            this.FormClosing += (s, e) =>
            {
                if (listBox1.Visible)
                {
                    e.Cancel = true;
                    textBox13.Text = string.Empty;
                    listBox1.Visible = false;
                    textBox13.Focus();
                }
            };

            if (comboBox2.SelectedIndex == SSH)
            {
                textBox1.Text = "22";
                label5.Enabled = true;
                textBox3.Enabled = true;
                panel1.Enabled = true;
                panel3.Enabled = true;
                label14.Enabled = true;
                textBox12.Enabled = true;
            }
            else if (comboBox2.SelectedIndex == RDP)
            {
                textBox1.Text = "3389";
                label5.Enabled = false;
                textBox3.Enabled = false;
                panel1.Enabled = false;
                panel3.Enabled = false;
                label14.Enabled = false;
                textBox12.Enabled = false;
            }
            else if (comboBox2.SelectedIndex == SFTP)
            {
                textBox1.Text = "22";
                label5.Enabled = true;
                textBox3.Enabled = true;
                panel1.Enabled = true;
                panel3.Enabled = true;
                label14.Enabled = false;
                textBox12.Enabled = false;
            }

            if (checkBox3.Checked)
            {
                label7.Enabled = true;
                label8.Enabled = true;
                label9.Enabled = true;
                label10.Enabled = true;
                label11.Enabled = true;
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox8.Enabled = true;
                textBox9.Enabled = true;
                textBox10.Enabled = true;
                panel4.Enabled = true;
                panel5.Enabled = true;
                panel6.Enabled = true;
                label15.Enabled = true;
                checkBox1.Enabled = true;
            }
            else
            {
                label7.Enabled = false;
                label8.Enabled = false;
                label9.Enabled = false;
                label10.Enabled = false;
                label11.Enabled = false;
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                textBox8.Enabled = false;
                textBox9.Enabled = false;
                textBox10.Enabled = false;
                panel4.Enabled = false;
                panel5.Enabled = false;
                panel6.Enabled = false;
                label15.Enabled = false;
                checkBox1.Enabled = false;
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            var value = 1;
            NativeMethods.DwmSetWindowAttribute(
                this.Handle, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, (uint)Marshal.SizeOf(typeof(int)));

            Program.ChangeFont(this);
            Program.SortTabIndex(this);

            try
            {
                if (!File.Exists(boosteraKeyPath))
                {
                    if (HistoryEncrypted.CreateKey(boosteraKeyPath))
                    {
                        MessageBox.Show("接続情報保護用のシークレットが作成されました。\nこれは他の人に共有しないように注意してください。\n\n" +
                            boosteraKeyPath, "Boostera");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Visible) return;

            this.Hide();

            Random random = new Random();
            var forwardingLocalPort = random.Next(49152, 65535).ToString();

            var protocol = comboBox2.SelectedIndex;
            var protocolText = comboBox2.Text;
            var host = textBox5.Text;
            var user = textBox4.Text;
            var port = textBox1.Text;
            var privateKey = textBox3.Text;
            var password = textBox2.Text;
            var logonScript = textBox12.Text;
            var isForwarding = checkBox3.Checked;
            var forwardingHost = textBox6.Text;
            var forwardingUser = textBox10.Text;
            var forwardingPort = textBox9.Text;
            var forwardingPrivateKey = textBox7.Text;
            var forwardingPassword = textBox8.Text;
            var isHide = checkBox1.Checked;
            var tag = textBox11.Text;

            if (isForwarding)
            {
                var arguments = forwardingHost + ":" + forwardingPort + " /ssh2";
                if (isHide)
                {
                    arguments += " /V";
                }
                else
                {
                    arguments += " /I";
                }
                arguments += " /ssh-L" + forwardingLocalPort + ":" + host + ":" + port;

                if (!string.IsNullOrEmpty(forwardingPrivateKey))
                {
                    arguments += " /auth=publickey /user=" + forwardingUser + " /keyfile=\"" + forwardingPrivateKey + "\"";
                }
                else
                {
                    arguments += " /auth=password /user=" + forwardingUser;
                }
                if (!string.IsNullOrEmpty(forwardingPassword)) arguments += " /passwd=\"" + forwardingPassword + "\"";

                var windowTitle = forwardingUser + "@" + forwardingHost;
                arguments += " /W=" + windowTitle;

                var psi = new ProcessStartInfo(ttermproPath);
                psi.UseShellExecute = true;
                psi.Arguments = arguments;
                try
                {
                    Process.Start(psi);
                    Thread.Sleep(3000);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }

            if (comboBox2.SelectedIndex == SSH)
            {
                var arguments = string.Empty;
                if (isForwarding)
                {
                    arguments = "localhost:" + forwardingLocalPort + " /nosecuritywarning /ssh2";  // トンネリング時に限り検証無効
                }
                else
                {
                    arguments = host + ":" + port + " /ssh2";
                }

                if (!string.IsNullOrEmpty(privateKey))
                {
                    arguments += " /auth=publickey /user=" + user + " /keyfile=\"" + privateKey + "\"";
                }
                else
                {
                    arguments += " /auth=password /user=" + user;
                }
                if (!string.IsNullOrEmpty(password)) arguments += " /passwd=\"" + password + "\"";

                if (!string.IsNullOrEmpty(logonScript))
                {
                    try
                    {
                        if (!Directory.Exists(Path.Combine(Program.BoosteraDataFolder, ".temp")))
                            Directory.CreateDirectory(Path.Combine(Program.BoosteraDataFolder, ".temp"));

                        var script = @"wait ''
mpause 3000
sendln '" + logonScript + "'\r\n" +
"filedelete '" + Path.Combine(Program.BoosteraDataFolder, ".temp\\logon.ttl") + "'";

                        File.WriteAllText(Path.Combine(Program.BoosteraDataFolder, ".temp\\logon.ttl"), script);
                        arguments += " /M=\"" + Path.Combine(Program.BoosteraDataFolder, ".temp\\logon.ttl") + "\"";
                    }
                    catch { }
                }

                var windowTitle = user + "@" + host;
                arguments += " /W=" + windowTitle;

                var psi = new ProcessStartInfo(ttermproPath);
                psi.UseShellExecute = true;
                psi.Arguments = arguments;
                try
                {
                    Process.Start(psi);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            else if (comboBox2.SelectedIndex == RDP)
            {
                var arguments1 = string.Empty;
                var arguments2 = string.Empty;

                if (isForwarding)
                {
                    arguments1 = "/generic:TERMSRV/localhost /user:" + user + " /pass:" + password;
                    arguments2 = "/v:localhost:" + forwardingLocalPort;
                }
                else
                {
                    arguments1 = "/generic:TERMSRV/" + host + " /user:" + user + " /pass:" + password;
                    arguments2 = "/v:" + host + ":" + port;
                }

                var psi1 = new ProcessStartInfo("cmdkey");
                psi1.UseShellExecute = true;
                psi1.WindowStyle = ProcessWindowStyle.Minimized;
                psi1.Arguments = arguments1;
                try
                {
                    var process = Process.Start(psi1);
                    process.WaitForExit();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }

                var psi2 = new ProcessStartInfo("mstsc");
                psi2.UseShellExecute = true;
                psi2.Arguments = arguments2;
                try
                {
                    Process.Start(psi2);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            else if (comboBox2.SelectedIndex == SFTP)
            {
                var arguments = string.Empty;

                if (isForwarding)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        arguments = "sftp://" + user + ":" + password + "@localhost:" + forwardingLocalPort;
                    }
                    else
                    {
                        arguments = "sftp://" + user + "@localhost:" + forwardingLocalPort;
                    }
                    if (!string.IsNullOrEmpty(privateKey)) arguments += " /privatekey=\"" + privateKey + "\"";

                    arguments += " /hostkey=\"*\"";  // トンネリング時に限り検証無効
                }
                else
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        arguments = "sftp://" + user + ":" + password + "@" + host + ":" + port;
                    }
                    else
                    {
                        arguments = "sftp://" + user + "@" + host + ":" + port;
                    }
                    if (!string.IsNullOrEmpty(privateKey)) arguments += " /privatekey=\"" + privateKey + "\"";
                }

                arguments += " /sessionname=" + user + "@" + host;

                var psi = new ProcessStartInfo(winscpPath);
                psi.UseShellExecute = true;
                psi.Arguments = arguments;
                try
                {
                    Process.Start(psi);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }

            var searchKey = string.Empty;
            var uniqueKey = string.Empty;

            if (string.IsNullOrEmpty(tag))
            {
                searchKey = protocolText + user + host;
                uniqueKey = protocolText + "://" + user + "@" + host;
            }
            else
            {
                searchKey = protocolText + user + host + tag;
                uniqueKey = protocolText + "://" + user + "@" + host + " #" + tag;
            }

            var hitory = new History(uniqueKey, searchKey, protocol, host, user, port, privateKey, password, logonScript,
                isForwarding, forwardingHost, forwardingUser, forwardingPort, forwardingPrivateKey, forwardingPassword, isHide, tag);

            histories.RemoveAll(x => x.UniqueKey == uniqueKey);
            histories.Insert(0, hitory);
            if (histories.Count > MAX_HISTORIES_COUNT) histories.RemoveRange(MAX_HISTORIES_COUNT, histories.Count - MAX_HISTORIES_COUNT);

            try
            {
                var historyEncrypted = HistoryEncrypted.EncryptData(JsonSerializer.Serialize(histories), boosteraKeyPath);
                var historyJson = JsonSerializer.Serialize(historyEncrypted, jsonSerializerOptions);
                File.WriteAllText(Path.Combine(Program.BoosteraDataFolder, "history.json"), historyJson);
            }
            catch { }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox1.SelectionStart;
            textBox1.Text = Regex.Replace(textBox1.Text, @"[^\d]", string.Empty);
            textBox1.SelectionStart = caret;
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox9.SelectionStart;
            textBox9.Text = Regex.Replace(textBox9.Text, @"[^\d]", string.Empty);
            textBox9.SelectionStart = caret;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox5.SelectionStart;
            textBox5.Text = textBox5.Text.Replace("'", "");
            textBox5.SelectionStart = caret;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox4.SelectionStart;
            textBox4.Text = textBox4.Text.Replace("'", "");
            textBox4.SelectionStart = caret;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox2.SelectionStart;
            textBox2.Text = textBox2.Text.Replace("'", "");
            textBox2.SelectionStart = caret;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox6.SelectionStart;
            textBox6.Text = textBox6.Text.Replace("'", "");
            textBox6.SelectionStart = caret;
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox10.SelectionStart;
            textBox10.Text = textBox10.Text.Replace("'", "");
            textBox10.SelectionStart = caret;
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox8.SelectionStart;
            textBox8.Text = textBox8.Text.Replace("'", "");
            textBox8.SelectionStart = caret;
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox11.SelectionStart;
            textBox11.Text = textBox11.Text.Replace("'", "");
            textBox11.SelectionStart = caret;
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox12.SelectionStart;
            textBox12.Text = textBox12.Text.Replace("'", "\"");
            textBox12.SelectionStart = caret;
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox13.Text))
            {
                label6.Visible = true;
                listBox1.Visible = false;
            }
            else
            {
                label6.Visible = false;
                listBox1.Visible = true;
            }

            var searchWords = textBox13.Text.ToLower().Split(new string[] { " ", "　" }, StringSplitOptions.RemoveEmptyEntries);
            var matchedHistories = new List<History>();

            histories.ForEach(x =>
            {
                if (searchWords.All(y => x.ToString().ToLower().Contains(y))) matchedHistories.Add(x);
            });

            listBox1.BeginUpdate();
            listBox1.Items.Clear();
            matchedHistories.ForEach(x => listBox1.Items.Add(x));
            listBox1.EndUpdate();
            if (listBox1.Items.Count != 0) listBox1.SelectedIndex = 0;
        }

        private void label6_Click(object sender, EventArgs e)
        {
            textBox13.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var targetFolder = string.Empty;
                using (var openFileDialog = new OpenFileDialog())
                {
                    if (!Directory.Exists(boosteraMacroFolder)) Directory.CreateDirectory(boosteraMacroFolder);
                    openFileDialog.InitialDirectory = boosteraMacroFolder;
                    openFileDialog.FileName = "Folder";
                    openFileDialog.Filter = "Folder|.";
                    openFileDialog.ValidateNames = false;
                    openFileDialog.CheckFileExists = false;
                    openFileDialog.CheckPathExists = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK) targetFolder = Path.GetDirectoryName(openFileDialog.FileName);
                }

                if (string.IsNullOrEmpty(targetFolder) || !Directory.Exists(targetFolder)) return;

                Random random = new Random();
                var forwardingLocalPort = random.Next(49152, 65535).ToString();

                var protocolText = comboBox2.Text;
                var host = textBox5.Text;
                var user = textBox4.Text;
                var port = textBox1.Text;
                var privateKey = textBox3.Text;
                var password = textBox2.Text;
                var logonScript = textBox12.Text;
                var isForwarding = checkBox3.Checked;
                var forwardingHost = textBox6.Text;
                var forwardingUser = textBox10.Text;
                var forwardingPort = textBox9.Text;
                var forwardingPrivateKey = textBox7.Text;
                var forwardingPassword = textBox8.Text;
                var isHide = checkBox1.Checked;
                var tag = textBox11.Text;

                var ttl = TTL_TEMPLATE.Replace("{{Protocol}}", protocolText);
                ttl = ttl.Replace("{{Host}}", host);
                ttl = ttl.Replace("{{User}}", user);
                ttl = ttl.Replace("{{Port}}", port);
                ttl = ttl.Replace("{{PrivateKey}}", privateKey.Replace(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "%USERPROFILE%"));
                ttl = ttl.Replace("{{Password}}", password);
                ttl = ttl.Replace("{{LogonScript}}", logonScript);
                ttl = ttl.Replace("{{IsForwarding}}", isForwarding.ToString().ToLower());
                ttl = ttl.Replace("{{ForwardingHost}}", forwardingHost);
                ttl = ttl.Replace("{{ForwardingUser}}", forwardingUser);
                ttl = ttl.Replace("{{ForwardingPort}}", forwardingPort);
                ttl = ttl.Replace("{{ForwardingLocalPort}}", forwardingLocalPort);
                ttl = ttl.Replace("{{ForwardingPrivateKey}}", forwardingPrivateKey.Replace(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "%USERPROFILE%"));
                ttl = ttl.Replace("{{ForwardingPassword}}", forwardingPassword);
                ttl = ttl.Replace("{{IsHide}}", isHide.ToString().ToLower());
                ttl = ttl.Replace("{{WinscpPath}}", winscpPath.Replace(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "%USERPROFILE%"));
                ttl = ttl.Replace("{{Tag}}", tag);

                var ttlFileName = protocolText + "_" + user + "@" + host;
                if (!string.IsNullOrEmpty(tag)) ttlFileName += "_" + tag;
                ttlFileName = Regex.Replace(ttlFileName, @"[<>:""/\\|?*]", "");
                ttlFileName = ttlFileName.ToLower() + ".ttl";

                File.WriteAllText(Path.Combine(targetFolder, ttlFileName), ttl);
                MessageBox.Show("TTL マクロをエクスポートしました。\n\n" + Path.Combine(targetFolder, ttlFileName), "Boostera");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var ttlFilePath = string.Empty;
                using (var openFileDialog = new OpenFileDialog())
                {
                    if (!Directory.Exists(boosteraMacroFolder)) Directory.CreateDirectory(boosteraMacroFolder);
                    openFileDialog.InitialDirectory = boosteraMacroFolder;
                    openFileDialog.Filter = "TTL|*.ttl";

                    if (openFileDialog.ShowDialog() == DialogResult.OK) ttlFilePath = openFileDialog.FileName;
                }

                if (string.IsNullOrEmpty(ttlFilePath) || !File.Exists(ttlFilePath)) return;
                var ttl = File.ReadAllText(ttlFilePath);

                comboBox2.Text = RegexMatchedGroupText(ttl, @"^Protocol\s+=\s+'(.*?)'");
                textBox5.Text = RegexMatchedGroupText(ttl, @"^Host\s+=\s+'(.*?)'");
                textBox4.Text = RegexMatchedGroupText(ttl, @"^User\s+=\s+'(.*?)'");
                textBox1.Text = RegexMatchedGroupText(ttl, @"^Port\s+=\s+'(.*?)'");
                textBox3.Text = RegexMatchedGroupText(ttl, @"^PrivateKey\s+=\s+'(.*?)'")
                    .Replace("%USERPROFILE%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                textBox2.Text = RegexMatchedGroupText(ttl, @"^Password\s+=\s+'(.*?)'");
                textBox12.Text = RegexMatchedGroupText(ttl, @"^LogonScript\s+=\s+'(.*?)'");
                checkBox3.Checked = RegexMatchedGroupText(ttl, @"^IsForwarding\s+=\s+'(.*?)'") == "true";
                textBox6.Text = RegexMatchedGroupText(ttl, @"^ForwardingHost\s+=\s+'(.*?)'");
                textBox10.Text = RegexMatchedGroupText(ttl, @"^ForwardingUser\s+=\s+'(.*?)'");
                textBox9.Text = RegexMatchedGroupText(ttl, @"^ForwardingPort\s+=\s+'(.*?)'");
                textBox7.Text = RegexMatchedGroupText(ttl, @"^ForwardingPrivateKey\s+=\s+'(.*?)'")
                    .Replace("%USERPROFILE%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                textBox8.Text = RegexMatchedGroupText(ttl, @"^ForwardingPassword\s+=\s+'(.*?)'");
                checkBox1.Checked = RegexMatchedGroupText(ttl, @"^IsHide\s+=\s+'(.*?)'") == "true";
                textBox11.Text = RegexMatchedGroupText(ttl, @"^Tag\s+=\s+'(.*?)'");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private string RegexMatchedGroupText(string text, string pattern)
        {
            var matchedGroupText = string.Empty;
            try
            {
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                matchedGroupText = match.Groups[1].Value;
            }
            catch { }
            return matchedGroupText;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                label7.Enabled = true;
                label8.Enabled = true;
                label9.Enabled = true;
                label10.Enabled = true;
                label11.Enabled = true;
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox8.Enabled = true;
                textBox9.Enabled = true;
                textBox10.Enabled = true;
                panel4.Enabled = true;
                panel5.Enabled = true;
                panel6.Enabled = true;
                label15.Enabled = true;
                checkBox1.Enabled = true;
            }
            else
            {
                label7.Enabled = false;
                label8.Enabled = false;
                label9.Enabled = false;
                label10.Enabled = false;
                label11.Enabled = false;
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                textBox8.Enabled = false;
                textBox9.Enabled = false;
                textBox10.Enabled = false;
                panel4.Enabled = false;
                panel5.Enabled = false;
                panel6.Enabled = false;
                label15.Enabled = false;
                checkBox1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Width = (int)Math.Round((decimal)initialWidth * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));
            this.Height = (int)Math.Round((decimal)initialHeight * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));
        }

        private static readonly string TTL_TEMPLATE = @"Protocol = '{{Protocol}}'
Host = '{{Host}}'
User = '{{User}}'
Port = '{{Port}}'
PrivateKey = '{{PrivateKey}}'
Password = '{{Password}}'
LogonScript = '{{LogonScript}}'
IsForwarding = '{{IsForwarding}}'
ForwardingHost = '{{ForwardingHost}}'
ForwardingUser = '{{ForwardingUser}}'
ForwardingPort = '{{ForwardingPort}}'
ForwardingLocalPort = '{{ForwardingLocalPort}}'
ForwardingPrivateKey = '{{ForwardingPrivateKey}}'
ForwardingPassword = '{{ForwardingPassword}}'
IsHide = '{{IsHide}}'
WinscpPath = '{{WinscpPath}}'
Tag = '{{Tag}}'

expandenv PrivateKey
expandenv ForwardingPrivateKey
expandenv WinscpPath

buf = ''
strcompare IsForwarding 'true'
if result == 0 then
    strconcat buf ForwardingHost
    strconcat buf ':'
    strconcat buf ForwardingPort
    strconcat buf ' /ssh2'

    strcompare IsHide 'true'
    if result == 0 then
        strconcat buf ' /V'
    else
        strconcat buf ' /I'
    endif

    strconcat buf ' /ssh-L'
    strconcat buf forwardingLocalPort
    strconcat buf ':'
    strconcat buf Host
    strconcat buf ':'
    strconcat buf Port

    strcompare ForwardingPrivateKey ''
    if result != 0 then
        strconcat buf ' /auth=publickey /user='
        strconcat buf ForwardingUser
        strconcat buf ' /keyfile=""'
        strconcat buf ForwardingPrivateKey
        strconcat buf '""'
    else
        strconcat buf ' /auth=password /user='
        strconcat buf ForwardingUser
    endif

    strcompare ForwardingPassword ''
    if result != 0 then
        strconcat buf ' /passwd=""'
        strconcat buf ForwardingPassword
        strconcat buf '""'
    endif

    strconcat buf ' /W='
    strconcat buf ForwardingUser
    strconcat buf '@'
    strconcat buf ForwardingHost

    connect buf
    wait ''
    mpause 3000
    unlink
endif

buf = ''
strcompare Protocol 'SSH'
if result == 0 then

    strcompare IsForwarding 'true'
    if result == 0 then
        strconcat buf 'localhost:'
        strconcat buf forwardingLocalPort
        strconcat buf ' /nosecuritywarning /ssh2'
    else
        strconcat buf Host
        strconcat buf ':'
        strconcat buf Port
        strconcat buf ' /ssh2'
    endif

    strcompare PrivateKey ''
    if result != 0 then
        strconcat buf ' /auth=publickey /user='
        strconcat buf User
        strconcat buf ' /keyfile=""'
        strconcat buf PrivateKey
        strconcat buf '""'
    else
        strconcat buf ' /auth=password /user='
        strconcat buf User
    endif

    strcompare Password ''
    if result != 0 then
        strconcat buf ' /passwd=""'
        strconcat buf Password
        strconcat buf '""'
    endif

    strconcat buf ' /W='
    strconcat buf User
    strconcat buf '@'
    strconcat buf Host

    connect buf

    strcompare LogonScript ''
    if result != 0 then
        wait ''
        mpause 3000
        sendln LogonScript
    endif
endif

strcompare Protocol 'RDP'
if result == 0 then
    strconcat buf 'cmdkey'

    strcompare IsForwarding 'true'
    if result == 0 then
        strconcat buf ' /generic:TERMSRV/localhost /user:'
        strconcat buf User
        strconcat buf ' /pass:'
        strconcat buf Password
    else
        strconcat buf ' /generic:TERMSRV/'
        strconcat buf Host
        strconcat buf ' /user:'
        strconcat buf User
        strconcat buf ' /pass:'
        strconcat buf Password
    endif

    exec buf 'minimize' 1

    buf = ''
    strconcat buf 'mstsc'

    strcompare IsForwarding 'true'
    if result == 0 then
        strconcat buf ' /v:localhost:'
        strconcat buf forwardingLocalPort
    else
        strconcat buf ' /v:'
        strconcat buf Host
        strconcat buf ':'
        strconcat buf Port
    endif

    exec buf
endif

strcompare Protocol 'SFTP'
if result == 0 then
    strconcat buf WinscpPath

    strcompare IsForwarding 'true'
    if result == 0 then

        strcompare Password ''
        if result != 0 then
            strconcat buf ' sftp://'
            strconcat buf User
            strconcat buf ':'
            strconcat buf Password
            strconcat buf '@localhost:'
            strconcat buf forwardingLocalPort
        else
            strconcat buf ' sftp://'
            strconcat buf User
            strconcat buf '@localhost:'
            strconcat buf forwardingLocalPort
        endif

        strcompare PrivateKey ''
        if result != 0 then
            strconcat buf ' /privatekey=""'
            strconcat buf PrivateKey
            strconcat buf '""'
        endif

        strconcat buf ' /hostkey=""*""'
    else
        strcompare Password ''
        if result != 0 then
            strconcat buf ' sftp://'
            strconcat buf User
            strconcat buf ':'
            strconcat buf Password
            strconcat buf '@'
            strconcat buf Host
            strconcat buf ':'
            strconcat buf Port
        else
            strconcat buf ' sftp://'
            strconcat buf User
            strconcat buf '@'
            strconcat buf Host
            strconcat buf ':'
            strconcat buf Port
        endif

        strcompare PrivateKey ''
        if result != 0 then
            strconcat buf ' /privatekey=""'
            strconcat buf PrivateKey
            strconcat buf '""'
        endif
    endif

    strconcat buf ' /sessionname='
    strconcat buf User
    strconcat buf '@'
    strconcat buf Host

    exec buf
endif
";
    }
}
