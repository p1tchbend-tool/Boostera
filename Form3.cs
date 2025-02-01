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
        private string boosteraKeyFolder = Program.BoosteraDataFolder;
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

        public Form3(string ttermproPath, string winscpPath, string boosteraKeyFolder)
        {
            InitializeComponent();
            this.Shown += (s, e) => textBox13.Focus();

            this.ttermproPath = ttermproPath;
            this.winscpPath = winscpPath;
            this.boosteraKeyFolder = boosteraKeyFolder;
            comboBox2.SelectedIndex = 0;

            listBox1.Visible = false;
            listBox1.Top = label12.Top;
            listBox1.Width = textBox13.Width;

            float f = NativeMethods.GetDpiForSystem();
            listBox1.ItemHeight = (int)Math.Round(listBox1.ItemHeight * (f / 96f));
            listBox1.Height = listBox1.ItemHeight * 10;

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
                checkBox3.Checked = history.IsForwarding;
                textBox6.Text = history.ForwardingHost;
                textBox10.Text = history.ForwardingUser;
                textBox9.Text = history.ForwardingPort;
                textBox7.Text = history.ForwardingPrivateKey;
                textBox8.Text = history.ForwardingPassword;

                listBox1.Visible = false;
                button1.Focus();
            };

            listBox1.KeyDown += (s, e) =>
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
                    checkBox3.Checked = history.IsForwarding;
                    textBox6.Text = history.ForwardingHost;
                    textBox10.Text = history.ForwardingUser;
                    textBox9.Text = history.ForwardingPort;
                    textBox7.Text = history.ForwardingPrivateKey;
                    textBox8.Text = history.ForwardingPassword;
                }
                else
                {
                    textBox13.Focus();
                }
            };

            textBox13.KeyDown += (s, e) =>
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
                    checkBox3.Checked = history.IsForwarding;
                    textBox6.Text = history.ForwardingHost;
                    textBox10.Text = history.ForwardingUser;
                    textBox9.Text = history.ForwardingPort;
                    textBox7.Text = history.ForwardingPrivateKey;
                    textBox8.Text = history.ForwardingPassword;
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

            try
            {
                var historyEncrypted = JsonSerializer.Deserialize<HistoryEncrypted>(File.ReadAllText(Path.Combine(Program.BoosteraDataFolder, "history.json")));
                var historyJson = HistoryEncrypted.DecryptData(historyEncrypted, Program.BoosteraDataFolder, Program.BoosteraKeyFileName);
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
                checkBox3.Checked = history.IsForwarding;
                textBox6.Text = history.ForwardingHost;
                textBox10.Text = history.ForwardingUser;
                textBox9.Text = history.ForwardingPort;
                textBox7.Text = history.ForwardingPrivateKey;
                textBox8.Text = history.ForwardingPassword;
                textBox12.Text = history.LogonScript;
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
                    openFileDialog.RestoreDirectory = false;
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
                    openFileDialog.RestoreDirectory = false;
                    if (openFileDialog.ShowDialog() == DialogResult.OK) textBox7.Text = openFileDialog.FileName;
                }
            };

            checkBox3.CheckedChanged += (s, e) =>
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
                }
            };

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
            }

            toolTip1.Draw += (s, e) =>
            {
                e.DrawBackground();
                e.DrawBorder();
                e.DrawText(TextFormatFlags.WordBreak);
            };
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
                if (!File.Exists(Path.Combine(Program.BoosteraDataFolder, Program.BoosteraKeyFileName)))
                {
                    if (HistoryEncrypted.CreateKey(Program.BoosteraDataFolder, Program.BoosteraKeyFileName))
                    {
                        MessageBox.Show("接続情報保護用のシークレットが作成されました。\nこれは他の人に共有しないように注意してください。\n\n" +
                            Path.Combine(Program.BoosteraDataFolder, Program.BoosteraKeyFileName), "Boostera");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
                var arguments = EscapedTextForTtl(forwardingHost) + ":" + forwardingPort + " /ssh2";
                if (isHide)
                {
                    arguments += " /V";
                }
                else
                {
                    arguments += " /I";
                }
                arguments += " /ssh-L" + forwardingLocalPort + ":" + EscapedTextForTtl(host) + ":" + port;

                if (!string.IsNullOrEmpty(forwardingPrivateKey))
                {
                    arguments += " /auth=publickey /user=" + EscapedTextForTtl(forwardingUser) + " /keyfile=\"" + forwardingPrivateKey + "\"";
                }
                else
                {
                    arguments += " /auth=password /user=" + EscapedTextForTtl(forwardingUser);
                }
                if (!string.IsNullOrEmpty(forwardingPassword)) arguments += " /passwd=" + EscapedTextForTtl(forwardingPassword);

                var windowTitle = forwardingUser + "@" + forwardingHost;
                arguments += " /W=" + windowTitle;

                var psi = new ProcessStartInfo(ttermproPath);
                psi.UseShellExecute = true;
                psi.Arguments = arguments;
                try
                {
                    Process.Start(psi);
                    Thread.Sleep(200);
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
                    arguments = EscapedTextForTtl(host) + ":" + port + " /ssh2";
                }

                if (!string.IsNullOrEmpty(privateKey))
                {
                    arguments += " /auth=publickey /user=" + EscapedTextForTtl(user) + " /keyfile=\"" + privateKey + "\"";
                }
                else
                {
                    arguments += " /auth=password /user=" + EscapedTextForTtl(user);
                }
                if (!string.IsNullOrEmpty(password)) arguments += " /passwd=" + EscapedTextForTtl(password);

                if (!string.IsNullOrEmpty(logonScript))
                {
                    try
                    {
                        if (!Directory.Exists(Path.Combine(Program.BoosteraDataFolder, ".temp")))
                            Directory.CreateDirectory(Path.Combine(Program.BoosteraDataFolder, ".temp"));

                        var script = @"wait ''
mpause 200
sendln '" + EscapedTextForTtl(logonScript) + "'\r\n" +
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

            var hitory = new History(uniqueKey, protocol, host, user, port, privateKey, password, isForwarding, forwardingHost,
                forwardingUser, forwardingPort, forwardingLocalPort, isHide, forwardingPrivateKey, forwardingPassword, tag, logonScript, searchKey);

            histories.RemoveAll(x => x.UniqueKey == uniqueKey);
            histories.Insert(0, hitory);
            if (histories.Count > MAX_HISTORIES_COUNT) histories.RemoveRange(MAX_HISTORIES_COUNT, histories.Count - MAX_HISTORIES_COUNT);

            try
            {
                var historyEncrypted = HistoryEncrypted.EncryptData(JsonSerializer.Serialize(histories), Program.BoosteraDataFolder, Program.BoosteraKeyFileName);
                var historyJson = JsonSerializer.Serialize(historyEncrypted, jsonSerializerOptions);
                File.WriteAllText(Path.Combine(Program.BoosteraDataFolder, "history.json"), historyJson);
            }
            catch { }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = Regex.Replace(textBox1.Text, @"[^\d]", string.Empty);
            textBox1.SelectionStart = textBox1.Text.Length;
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            textBox9.Text = Regex.Replace(textBox9.Text, @"[^\d]", string.Empty);
            textBox9.SelectionStart = textBox9.Text.Length;
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

        private string EscapedTextForTtl (string text)
        {
            text = text.Replace("\"", "#34");
            text = text.Replace("'", "#39");
            text = text.Replace(";", "#59");
            return text;
        }
    }
}
