using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Boostera
{
    public partial class ConnectionForm : Form
    {
        private string ttermproPath = @"C:\Program Files (x86)\teraterm5\ttermpro.exe";
        private string winscpPath = @"C:\Program Files (x86)\WinSCP\WinSCP.exe";
        private string boosteraKeyPath = Path.Combine(Program.BoosteraDataFolder, "Boostera.Key");
        private bool isLogging = true;
        private string logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".Boostera\log");
        private string boosteraMacroFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".Boostera");
        private string sshFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh");
        private List<History> histories = new List<History>();
        private int preIndex = ListBox.NoMatches;
        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        private ConnectionManager connectionManager = new ConnectionManager();

        private static readonly int MAX_HISTORIES_COUNT = 10000;
        private static readonly int SSH = 0;
        private static readonly int RDP = 1;
        private static readonly int SFTP = 2;

        private static int? initialWidth = null;
        private static int? initialHeight = null;

        public ConnectionForm(string ttermproPath, string winscpPath, string boosteraKeyPath, bool isLogging, string logFolder)
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
            this.isLogging = isLogging;
            this.logFolder = logFolder;
            if (!Directory.Exists(logFolder))
            {
                try
                {
                    Directory.CreateDirectory(logFolder);
                }
                catch { }
            }
            comboBox2.SelectedIndex = 0;

            toolTip1.SetToolTip(comboBox2, "接続先のプロトコルを選択してください。\nSSHはTeraTerm、SFTPはWinSCPの事前設定が必要です。");
            toolTip1.SetToolTip(textBox5, "接続先のホストを入力してください。");
            toolTip1.SetToolTip(textBox4, "接続先のユーザーを入力してください。");
            toolTip1.SetToolTip(textBox1, "接続先のポートを入力してください。");
            toolTip1.SetToolTip(textBox3, "接続先の秘密鍵のパスを入力してください。");
            toolTip1.SetToolTip(textBox2, "接続先のパスワード／パスフレーズを入力してください。");
            toolTip1.SetToolTip(checkBox2, "チェックを外した場合、テキストボックスの値をそのまま使用します。\nチェックを入れた場合、テキストボックスの値と同名の環境変数を使用します。");
            toolTip1.SetToolTip(textBox12, "接続時に実行するコマンドを入力してください。");
            toolTip1.SetToolTip(textBox14, "指定した文字列が表示されるまで、コマンドの実行を待機します。\n空白の場合、任意の文字が表示されるまで、コマンドの実行を待機します。");
            toolTip1.SetToolTip(textBox15, "文字列が表示されてから、コマンドを実行するまでの待機時間を入力してください。");
            toolTip1.SetToolTip(checkBox3, "SSHトンネリングを使用する場合、チェックを入れてください。");
            toolTip1.SetToolTip(textBox6, "SSHトンネリングで利用するホストを入力してください。");
            toolTip1.SetToolTip(textBox10, "SSHトンネリングで利用するユーザーを入力してください。");
            toolTip1.SetToolTip(textBox9, "SSHトンネリングで利用するポートを入力してください。");
            toolTip1.SetToolTip(textBox7, "SSHトンネリングで利用する秘密鍵のパスを入力してください。");
            toolTip1.SetToolTip(textBox8, "SSHトンネリングで利用するパスワード／パスフレーズを入力してください。");
            toolTip1.SetToolTip(checkBox4, "チェックを外した場合、テキストボックスの値をそのまま使用します。\nチェックを入れた場合、テキストボックスの値と同名の環境変数を使用します。");
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
                checkBox2.Checked = history.IsEnvPassword;
                textBox12.Text = history.LogonScript;
                textBox14.Text = history.WaitingString;
                textBox15.Text = history.WaitingTime;
                checkBox3.Checked = history.IsForwarding;
                textBox6.Text = history.ForwardingHost;
                textBox10.Text = history.ForwardingUser;
                textBox9.Text = history.ForwardingPort;
                textBox7.Text = history.ForwardingPrivateKey;
                textBox8.Text = history.ForwardingPassword;
                checkBox4.Checked = history.ForwardingIsEnvPassword;
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
                    checkBox2.Checked = history.IsEnvPassword;
                    textBox12.Text = history.LogonScript;
                    textBox14.Text = history.WaitingString;
                    textBox15.Text = history.WaitingTime;
                    checkBox3.Checked = history.IsForwarding;
                    textBox6.Text = history.ForwardingHost;
                    textBox10.Text = history.ForwardingUser;
                    textBox9.Text = history.ForwardingPort;
                    textBox7.Text = history.ForwardingPrivateKey;
                    textBox8.Text = history.ForwardingPassword;
                    checkBox4.Checked = history.ForwardingIsEnvPassword;
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
                    checkBox2.Checked = history.IsEnvPassword;
                    textBox12.Text = history.LogonScript;
                    textBox14.Text = history.WaitingString;
                    textBox15.Text = history.WaitingTime;
                    checkBox3.Checked = history.IsForwarding;
                    textBox6.Text = history.ForwardingHost;
                    textBox10.Text = history.ForwardingUser;
                    textBox9.Text = history.ForwardingPort;
                    textBox7.Text = history.ForwardingPrivateKey;
                    textBox8.Text = history.ForwardingPassword;
                    checkBox4.Checked = history.ForwardingIsEnvPassword;
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
                var historyEncrypted = JsonSerializer.Deserialize<EncryptedText>(File.ReadAllText(Path.Combine(Program.BoosteraDataFolder, "history.json")));
                var historyJson = EncryptedText.Decrypt(historyEncrypted, boosteraKeyPath);
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
                checkBox2.Checked = history.IsEnvPassword;
                textBox12.Text = history.LogonScript;
                textBox14.Text = history.WaitingString;
                textBox15.Text = history.WaitingTime;
                checkBox3.Checked = history.IsForwarding;
                textBox6.Text = history.ForwardingHost;
                textBox10.Text = history.ForwardingUser;
                textBox9.Text = history.ForwardingPort;
                textBox7.Text = history.ForwardingPrivateKey;
                textBox8.Text = history.ForwardingPassword;
                checkBox4.Checked = history.ForwardingIsEnvPassword;
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
                    textBox14.Enabled = true;
                    textBox15.Enabled = true;
                    label16.Enabled = true;
                    label17.Enabled = true;
                    label18.Enabled = true;
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
                    textBox14.Enabled = false;
                    textBox15.Enabled = false;
                    label16.Enabled = false;
                    label17.Enabled = false;
                    label18.Enabled = false;
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
                    textBox14.Enabled = false;
                    textBox15.Enabled = false;
                    label16.Enabled = false;
                    label17.Enabled = false;
                    label18.Enabled = false;
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

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBox3.Text = openFileDialog.FileName;
                        sshFolder = Path.GetDirectoryName(openFileDialog.FileName);
                    }
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

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBox7.Text = openFileDialog.FileName;
                        sshFolder = Path.GetDirectoryName(openFileDialog.FileName);
                    }
                }
            };

            panel8.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                using (var envForm = new EnvForm(boosteraKeyPath))
                {
                    envForm.ShowDialog();
                    if (envForm.DialogResult != DialogResult.OK) return;
                    if (string.IsNullOrEmpty(envForm.SelectedEnv)) return;

                    textBox2.Text = envForm.SelectedEnv;
                    checkBox2.Checked = true;
                }
            };

            panel9.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                using (var envForm = new EnvForm(boosteraKeyPath))
                {
                    envForm.ShowDialog();
                    if (envForm.DialogResult != DialogResult.OK) return;
                    if (string.IsNullOrEmpty(envForm.SelectedEnv)) return;

                    textBox8.Text = envForm.SelectedEnv;
                    checkBox4.Checked = true;
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
                textBox14.Enabled = true;
                textBox15.Enabled = true;
                label16.Enabled = true;
                label17.Enabled = true;
                label18.Enabled = true;
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
                textBox14.Enabled = false;
                textBox15.Enabled = false;
                label16.Enabled = false;
                label17.Enabled = false;
                label18.Enabled = false;
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
                textBox14.Enabled = false;
                textBox15.Enabled = false;
                label16.Enabled = false;
                label17.Enabled = false;
                label18.Enabled = false;
            }

            if (checkBox3.Checked)
            {
                label7.Enabled = true;
                label8.Enabled = true;
                label9.Enabled = true;
                label10.Enabled = true;
                label11.Enabled = true;
                label15.Enabled = true;
                label19.Enabled = true;
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox8.Enabled = true;
                textBox9.Enabled = true;
                textBox10.Enabled = true;
                panel4.Enabled = true;
                panel5.Enabled = true;
                panel6.Enabled = true;
                panel9.Enabled = true;
                checkBox1.Enabled = true;
                checkBox4.Enabled = true;
            }
            else
            {
                label7.Enabled = false;
                label8.Enabled = false;
                label9.Enabled = false;
                label10.Enabled = false;
                label11.Enabled = false;
                label15.Enabled = false;
                label19.Enabled = false;
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                textBox8.Enabled = false;
                textBox9.Enabled = false;
                textBox10.Enabled = false;
                panel4.Enabled = false;
                panel5.Enabled = false;
                panel6.Enabled = false;
                panel9.Enabled = false;
                checkBox1.Enabled = false;
                checkBox4.Enabled = false;
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            var value = 1;
            NativeMethods.DwmSetWindowAttribute(
                this.Handle, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, (uint)Marshal.SizeOf(typeof(int)));

            Program.ChangeFont(this);
            Program.SortTabIndex(this);

            timer1.Start();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox1.SelectionStart;
            textBox1.Text = Regex.Replace(textBox1.Text, @"[^\d]", string.Empty);
            textBox1.Text = Regex.Replace(textBox1.Text, @"^(?!0$)0+", string.Empty);
            if (string.IsNullOrEmpty(textBox1.Text)) textBox1.Text = "0";
            textBox1.SelectionStart = caret;
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox9.SelectionStart;
            textBox9.Text = Regex.Replace(textBox9.Text, @"[^\d]", string.Empty);
            textBox9.Text = Regex.Replace(textBox9.Text, @"^(?!0$)0+", string.Empty);
            if (string.IsNullOrEmpty(textBox9.Text)) textBox9.Text = "0";
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

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox14.SelectionStart;
            textBox14.Text = textBox14.Text.Replace("'", "");
            textBox14.SelectionStart = caret;
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            var caret = textBox15.SelectionStart;
            textBox15.Text = Regex.Replace(textBox15.Text, @"[^\d]", string.Empty);
            textBox15.Text = Regex.Replace(textBox15.Text, @"^(?!0$)0+", string.Empty);
            if (string.IsNullOrEmpty(textBox15.Text)) textBox15.Text = "0";
            textBox15.SelectionStart = caret;
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

        private async void button1_Click(object sender, EventArgs e)
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
            var isEnvPassword = checkBox2.Checked;
            var logonScript = textBox12.Text;
            var waitingString = textBox14.Text;
            var waitingTime = textBox15.Text;
            var isForwarding = checkBox3.Checked;
            var forwardingHost = textBox6.Text;
            var forwardingUser = textBox10.Text;
            var forwardingPort = textBox9.Text;
            var forwardingPrivateKey = textBox7.Text;
            var forwardingPassword = textBox8.Text;
            var forwardingIsEnvPassword = checkBox4.Checked;
            var isHide = checkBox1.Checked;
            var tag = textBox11.Text;

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

            var hitory = new History(uniqueKey, searchKey, protocol, host, user, port, privateKey, password,
                isEnvPassword, logonScript, waitingString, waitingTime, isForwarding, forwardingHost, forwardingUser,
                forwardingPort, forwardingPrivateKey, forwardingPassword, forwardingIsEnvPassword, isHide, tag);

            histories.RemoveAll(x => x.UniqueKey == uniqueKey);
            histories.Insert(0, hitory);
            if (histories.Count > MAX_HISTORIES_COUNT) histories.RemoveRange(MAX_HISTORIES_COUNT, histories.Count - MAX_HISTORIES_COUNT);

            try
            {
                var historyEncrypted = EncryptedText.Encrypt(JsonSerializer.Serialize(histories), boosteraKeyPath);
                var historyJson = JsonSerializer.Serialize(historyEncrypted, jsonSerializerOptions);
                File.WriteAllText(Path.Combine(Program.BoosteraDataFolder, "history.json"), historyJson);
            }
            catch { }

            try
            {
                await connectionManager.Connect(ttermproPath, winscpPath, isLogging, logFolder, protocolText, host, user, port, privateKey, password,
                    isEnvPassword, logonScript, waitingString, waitingTime, isForwarding, forwardingHost, forwardingUser, forwardingPort, forwardingLocalPort,
                    forwardingPrivateKey, forwardingPassword, forwardingIsEnvPassword, isHide, tag);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
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

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        targetFolder = Path.GetDirectoryName(openFileDialog.FileName);
                        boosteraMacroFolder = Path.GetDirectoryName(openFileDialog.FileName);
                    }
                }

                if (string.IsNullOrEmpty(targetFolder) || !Directory.Exists(targetFolder)) return;

                Random random = new Random();
                var ttlFilePath = connectionManager.ExportTtl(targetFolder, comboBox2.Text, textBox5.Text, textBox4.Text, textBox1.Text, textBox3.Text,
                    textBox2.Text, checkBox2.Checked, textBox12.Text, textBox14.Text, textBox15.Text, checkBox3.Checked, textBox6.Text, textBox10.Text,
                    textBox9.Text, random.Next(49152, 65535).ToString(), textBox7.Text, textBox8.Text, checkBox4.Checked, checkBox1.Checked, textBox11.Text);

                MessageBox.Show("TTL マクロをエクスポートしました。\n\n" + ttlFilePath, "Boostera");
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

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        ttlFilePath = openFileDialog.FileName;
                        boosteraMacroFolder = Path.GetDirectoryName(openFileDialog.FileName);
                    }
                }

                if (string.IsNullOrEmpty(ttlFilePath) || !File.Exists(ttlFilePath)) return;

                var dict = connectionManager.ImportTtl(ttlFilePath);
                comboBox2.Text = dict["Protocol"];
                textBox5.Text = dict["Host"];
                textBox4.Text = dict["User"];
                textBox1.Text = dict["Port"];
                textBox3.Text = dict["PrivateKey"];
                textBox2.Text = dict["Password"];
                checkBox2.Checked = dict["IsEnvPassword"] == "true";
                textBox12.Text = dict["LogonScript"];
                textBox14.Text = dict["WaitingString"];
                textBox15.Text = dict["WaitingTime"];
                checkBox3.Checked = dict["IsForwarding"] == "true";
                textBox6.Text = dict["ForwardingHost"];
                textBox10.Text = dict["ForwardingUser"];
                textBox9.Text = dict["ForwardingPort"];
                textBox7.Text = dict["ForwardingPrivateKey"];
                textBox8.Text = dict["ForwardingPassword"];
                checkBox4.Checked = dict["ForwardingIsEnvPassword"] == "true";
                checkBox1.Checked = dict["IsHide"] == "true";
                textBox11.Text = dict["Tag"];
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
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
                label15.Enabled = true;
                label19.Enabled = true;
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox8.Enabled = true;
                textBox9.Enabled = true;
                textBox10.Enabled = true;
                panel4.Enabled = true;
                panel5.Enabled = true;
                panel6.Enabled = true;
                panel9.Enabled = true;
                checkBox1.Enabled = true;
                checkBox4.Enabled = true;
            }
            else
            {
                label7.Enabled = false;
                label8.Enabled = false;
                label9.Enabled = false;
                label10.Enabled = false;
                label11.Enabled = false;
                label15.Enabled = false;
                label19.Enabled = false;
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                textBox8.Enabled = false;
                textBox9.Enabled = false;
                textBox10.Enabled = false;
                panel4.Enabled = false;
                panel5.Enabled = false;
                panel6.Enabled = false;
                panel9.Enabled = false;
                checkBox1.Enabled = false;
                checkBox4.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Width = (int)Math.Round((decimal)initialWidth * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));
            this.Height = (int)Math.Round((decimal)initialHeight * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));
        }
    }
}
