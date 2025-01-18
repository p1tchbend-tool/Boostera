using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Forms;

namespace Boostera
{
    public partial class Form3 : Form
    {
        private string ttermproPath = @"C:\Program Files (x86)\teraterm\ttermpro.exe";
        private string winscpPath = @"C:\Program Files (x86)\WinSCP\WinSCP.exe";
        private string boosteraKeyFolder = Program.BoosteraDataFolder;
        private List<History> histories = new List<History>();
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
            this.Shown += (s, e) => firstLabel.Focus();

            this.ttermproPath = ttermproPath;
            this.winscpPath = winscpPath;
            this.boosteraKeyFolder = boosteraKeyFolder;
            comboBox2.SelectedIndex = 0;

            try
            {
                if (!File.Exists(Path.Combine(boosteraKeyFolder, "Boostera.Key")))
                {
                    var password = string.Empty;
                    while (string.IsNullOrEmpty(password))
                    {
                        using (Form4 form4 = new Form4())
                        {
                            form4.ShowDialog();
                            password = form4.Password;
                        }
                    }

                    var encrypt = new Encrypt(boosteraKeyFolder);
                    if (encrypt.CreateKey(password))
                    {
                        MessageBox.Show("パスワードが保存されました。\nこれは他の人に共有しないようにしてください。\n\n" + Path.Combine(boosteraKeyFolder, "Boostera.Key"));
                    }
                    else
                    {
                        MessageBox.Show("パスワードの登録に失敗しました。\nアプリを再起動して、もう一度やりなおしてください。");
                    }
                }
                else
                {
                    var str = new Encrypt(boosteraKeyFolder).DecryptString(File.ReadAllText(Path.Combine(Program.BoosteraDataFolder, "history.dat")));
                    histories = JsonSerializer.Deserialize<List<History>>(str);
                }
            }
            catch { }

            histories.ForEach(x =>
            {
                comboBox1.BeginUpdate();
                comboBox1.Items.Add(x.UniqueKey);
                comboBox1.EndUpdate();
            });

            if (comboBox1.Items.Count != 0)
            {
                var history = histories.FirstOrDefault(x => x.UniqueKey == comboBox1.Items[0].ToString());
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

                comboBox1.SelectedIndex = 0;
            }

            comboBox1.SelectedIndexChanged += (s, e) =>
            {
                if (comboBox1.SelectedItem == null) return;

                var history = histories.FirstOrDefault(x => x.UniqueKey == comboBox1.SelectedItem.ToString());
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
            };

            comboBox2.SelectedIndexChanged += (s, e) =>
            {
                if (comboBox2.SelectedIndex == SSH || comboBox2.SelectedIndex == SFTP)
                {
                    textBox1.Text = "22";
                    label5.Enabled = true;
                    textBox3.Enabled = true;
                    panel1.Enabled = true;
                    panel3.Enabled = true;
                }
                else if (comboBox2.SelectedIndex == RDP)
                {
                    textBox1.Text = "3389";
                    label5.Enabled = false;
                    textBox3.Enabled = false;
                    panel1.Enabled = false;
                    panel3.Enabled = false;
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
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            var value = 1;
            NativeMethods.DwmSetWindowAttribute(
                this.Handle, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, (uint)Marshal.SizeOf(typeof(int)));

            Program.ChangeFont(this);
            Program.SortTabIndex(this);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            var localPort = random.Next(49152, 65535).ToString();

            if (checkBox3.Checked)
            {
                var host = "\"" + textBox6.Text.Replace("\"", "\"\"") + "\"";
                var forwardingHost = textBox5.Text.Replace("\"", "\"\"") + "\"";
                var user = "\"" + textBox10.Text.Replace("\"", "\"\"") + "\"";
                var port = "\"" + textBox9.Text.Replace("\"", "\"\"") + "\"";
                var forwardingPort = textBox1.Text.Replace("\"", "\"\"") + "\"";
                var key = textBox7.Text;
                var passwd = textBox8.Text;

                var arguments = host + ":" + port + " /ssh2 /I /ssh-L" + localPort + ":" + forwardingHost + ":" + forwardingPort;
                if (!string.IsNullOrEmpty(key))
                {
                    arguments += " /auth=publickey /user=" + user + " /keyfile=\"" + key.Replace("\"", "\"\"") + "\"";
                }
                else
                {
                    arguments += " /auth=password /user=" + user;
                }
                if (!string.IsNullOrEmpty(passwd)) arguments += " /passwd=\"" + passwd.Replace("\"", "\"\"") + "\"";

                var psi = new ProcessStartInfo(ttermproPath);
                psi.UseShellExecute = true;
                psi.Arguments = arguments;
                try
                {
                    Process.Start(psi);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }

            if (comboBox2.SelectedIndex == SSH)
            {
                var host = "\"" + textBox5.Text.Replace("\"", "\"\"") + "\"";
                var user = "\"" + textBox4.Text.Replace("\"", "\"\"") + "\"";
                var port = "\"" + textBox1.Text.Replace("\"", "\"\"") + "\"";
                var key = textBox3.Text;
                var passwd = textBox2.Text;

                var arguments = string.Empty;
                if (checkBox3.Checked)
                {
                    arguments = "localhost:" + localPort + " /nosecuritywarning /ssh2";  // トンネリング時に限り検証無効
                }
                else
                {
                    arguments = host + ":" + port + " /ssh2";
                }

                if (!string.IsNullOrEmpty(key))
                {
                    arguments += " /auth=publickey /user=" + user + " /keyfile=\"" + key.Replace("\"", "\"\"") + "\"";
                }
                else
                {
                    arguments += " /auth=password /user=" + user;
                }
                if (!string.IsNullOrEmpty(passwd)) arguments += " /passwd=\"" + passwd.Replace("\"", "\"\"") + "\"";
                arguments += " /W=" + textBox4.Text.Replace("\"", "\"\"") + "@" + textBox5.Text.Replace("\"", "\"\"");

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
                var host = textBox5.Text;
                var user = textBox4.Text;
                var port = textBox1.Text;
                var passwd = textBox2.Text;
                var arguments1 = string.Empty;
                var arguments2 = string.Empty;
                var arguments3 = string.Empty;

                if (checkBox3.Checked)
                {
                    arguments1 = "/generic:TERMSRV/localhost" + " /user:" + user + " /pass:" + passwd;
                    arguments2 = "/v:localhost" + ":" + localPort;
                    arguments3 = "/delete:TERMSRV/localhost";
                }
                else
                {
                    arguments1 = "/generic:TERMSRV/" + host + " /user:" + user + " /pass:" + passwd;
                    arguments2 = "/v:" + host + ":" + port;
                    arguments3 = "/delete:TERMSRV/" + host;
                }

                var psi1 = new ProcessStartInfo("cmdkey");
                psi1.UseShellExecute = true;
                psi1.WindowStyle = ProcessWindowStyle.Minimized;
                psi1.Arguments = arguments1;
                try
                {
                    Process.Start(psi1);
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
                var host = textBox5.Text;
                var user = textBox4.Text;
                var port = textBox1.Text;
                var key = textBox3.Text;
                var passwd = textBox2.Text;
                var arguments = string.Empty;

                if (checkBox3.Checked)
                {
                    if (string.IsNullOrEmpty(passwd))
                    {
                        arguments = "sftp://" + user + "@localhost:" + localPort;
                    }
                    else
                    {
                        arguments = "sftp://" + user + ":" + passwd + "@localhost:" + localPort;
                    }
                    if (!string.IsNullOrEmpty(key)) arguments += " /privatekey=\"" + key + "\"";
                    arguments += " /sessionname=" + user + "@" + host;
                    arguments += " /hostkey=\"*\"";  // トンネリング時に限り検証無効
                }
                else
                {
                    if (string.IsNullOrEmpty(passwd))
                    {
                        arguments = "sftp://" + user + "@" + host + ":" + port;
                    }
                    else
                    {
                        arguments = "sftp://" + user + ":" + passwd + "@" + host + ":" + port;
                    }
                    if (!string.IsNullOrEmpty(key)) arguments += " /privatekey=\"" + key + "\"";
                }

                var psi = new ProcessStartInfo(winscpPath);
                psi.UseShellExecute = true;
                psi.Arguments = arguments;
                try
                {
                    Process.Start(psi);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }

            var uniqueKey = comboBox2.Text + " : " + textBox5.Text + " : " + textBox4.Text + " : " + textBox1.Text;
            var privateKey = textBox3.Text;
            var password = textBox2.Text;
            var forwardingPrivateKey = textBox7.Text;
            var forwardingPassword = textBox8.Text;
            var hitory = new History(uniqueKey, comboBox2.SelectedIndex, textBox5.Text, textBox4.Text, textBox1.Text, privateKey, password,
                checkBox3.Checked, textBox6.Text, textBox10.Text, textBox9.Text, forwardingPrivateKey, forwardingPassword);

            histories.RemoveAll(x => x.UniqueKey == uniqueKey);
            histories.Insert(0, hitory);
            if (histories.Count > MAX_HISTORIES_COUNT) histories.RemoveRange(MAX_HISTORIES_COUNT, histories.Count - MAX_HISTORIES_COUNT);

            try
            {
                var str = new Encrypt(boosteraKeyFolder).EncryptString(JsonSerializer.Serialize(histories, jsonSerializerOptions));
                File.WriteAllText(Path.Combine(Program.BoosteraDataFolder, "history.dat"), str);
            }
            catch { }
        }
    }
}
