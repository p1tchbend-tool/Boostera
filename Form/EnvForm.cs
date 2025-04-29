using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using CsvHelper;
using System.Linq;

namespace Boostera
{
    public partial class EnvForm : Form
    {
        public string SelectedEnv { get { return textBox2.Text; } }
        private static int? initialWidth = null;
        private static int? initialHeight = null;
        private string boosteraKeyPath = Path.Combine(Program.BoosteraDataFolder, "Boostera.Key");
        private string boosteraEnvFileFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".Boostera\env");
        private List<string> running = new List<string>();
        private List<Env> envs = new List<Env>();
        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public EnvForm(string boosteraKeyPath)
        {
            InitializeComponent();

            if (initialWidth == null) initialWidth = this.Width;
            this.Width = (int)Math.Round((decimal)initialWidth * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));

            if (initialHeight == null) initialHeight = this.Height;
            this.Height = (int)Math.Round((decimal)initialHeight * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));

            this.FormClosing += (s, e) =>
            {
                if (running.Count > 0)
                {
                    MessageBox.Show("環境変数の設定中です。\nしばらく待ってからフォームを閉じてください。", "Boostera");
                    e.Cancel = true;
                }
            };

            this.boosteraKeyPath = boosteraKeyPath;
            try
            {
                var envsEncrypted = JsonSerializer.Deserialize<EncryptedText>(File.ReadAllText(Path.Combine(Program.BoosteraDataFolder, "env.json")));
                var envsJson = EncryptedText.Decrypt(envsEncrypted, boosteraKeyPath);
                envs = JsonSerializer.Deserialize<List<Env>>(envsJson);
            }
            catch { }

            listView1.BeginUpdate();
            envs.ForEach(env =>
            {
                var item = new ListViewItem(env.Key);
                item.Name = env.Key;
                item.SubItems.Add(env.Value);
                listView1.Items.Add(item);
            });
            listView1.EndUpdate();
        }

        private void EnvForm_Load(object sender, EventArgs e)
        {
            UiHelper.SetDarkMode(this, true);
            UiHelper.ChangeFontFamily(this, "メイリオ");
            UiHelper.SortTabIndex(this);
            
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UiHelper.AdjustDpi(this, initialWidth, initialHeight);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItems = listView1.SelectedItems;
            if (selectedItems.Count > 0)
            {
                textBox2.Text = selectedItems[0].Text;
                textBox1.Text = selectedItems[0].SubItems[1].Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox1.Text)) return;

            Task.Run(() =>
            {
                var id = Guid.NewGuid().ToString("N");
                running.Add(id);
                try
                {
                    Environment.SetEnvironmentVariable(textBox2.Text, textBox1.Text, EnvironmentVariableTarget.User);
                }
                catch { }
                running.Remove(id);
            });

            listView1.Items.RemoveByKey(textBox2.Text);

            var listViewItem = new ListViewItem(textBox2.Text);
            listViewItem.Name = textBox2.Text;
            listViewItem.SubItems.Add(textBox1.Text);
            listView1.Items.Add(listViewItem);

            envs.Clear();
            foreach (ListViewItem item in listView1.Items)
            {
                envs.Add(new Env(item.Text, item.SubItems[1].Text));
            }

            try
            {
                var envsEncrypted = EncryptedText.Encrypt(JsonSerializer.Serialize(envs), boosteraKeyPath);
                var envsJson = JsonSerializer.Serialize(envsEncrypted, jsonSerializerOptions);
                File.WriteAllText(Path.Combine(Program.BoosteraDataFolder, "env.json"), envsJson);
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text)) return;

            Task.Run(() =>
            {
                var id = Guid.NewGuid().ToString("N");
                running.Add(id);
                try
                {
                    Environment.SetEnvironmentVariable(textBox2.Text, null, EnvironmentVariableTarget.User);
                }
                catch { }
                running.Remove(id);
            });

            listView1.Items.RemoveByKey(textBox2.Text);

            envs.Clear();
            foreach (ListViewItem item in listView1.Items)
            {
                envs.Add(new Env(item.Text, item.SubItems[1].Text));
            }

            try
            {
                var envsEncrypted = EncryptedText.Encrypt(JsonSerializer.Serialize(envs), boosteraKeyPath);
                var envsJson = JsonSerializer.Serialize(envsEncrypted, jsonSerializerOptions);
                File.WriteAllText(Path.Combine(Program.BoosteraDataFolder, "env.json"), envsJson);
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count < 1) return;

            var targetFolder = string.Empty;
            using (var openFileDialog = new OpenFileDialog())
            {
                if (!Directory.Exists(boosteraEnvFileFolder)) Directory.CreateDirectory(boosteraEnvFileFolder);
                openFileDialog.InitialDirectory = boosteraEnvFileFolder;
                openFileDialog.FileName = "Folder";
                openFileDialog.Filter = "Folder|.";
                openFileDialog.ValidateNames = false;
                openFileDialog.CheckFileExists = false;
                openFileDialog.CheckPathExists = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    targetFolder = Path.GetDirectoryName(openFileDialog.FileName);
                    boosteraEnvFileFolder = Path.GetDirectoryName(openFileDialog.FileName);
                }
            }

            if (string.IsNullOrEmpty(targetFolder) || !Directory.Exists(targetFolder)) return;

            envs.Clear();
            foreach (ListViewItem item in listView1.Items)
            {
                envs.Add(new Env(item.Text, item.SubItems[1].Text));
            }

            try
            {
                using (var writer = new StreamWriter(Path.Combine(targetFolder, "env.csv")))
                {
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(envs);
                    }
                }

                MessageBox.Show("環境変数を 「env.csv」 にエクスポートしました。", "Boostera");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var envFilePath = string.Empty;
            using (var openFileDialog = new OpenFileDialog())
            {
                if (!Directory.Exists(boosteraEnvFileFolder)) Directory.CreateDirectory(boosteraEnvFileFolder);
                openFileDialog.InitialDirectory = boosteraEnvFileFolder;
                openFileDialog.Filter = "CSV|*.csv";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    envFilePath = openFileDialog.FileName;
                    boosteraEnvFileFolder = Path.GetDirectoryName(openFileDialog.FileName);
                }
            }

            if (string.IsNullOrEmpty(envFilePath) || !File.Exists(envFilePath)) return;

            try
            {
                using (var reader = new StreamReader(envFilePath))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        envs = csv.GetRecords<Env>().ToList();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            listView1.BeginUpdate();
            foreach (var env in envs)
            {
                listView1.Items.RemoveByKey(env.Key);

                var listViewItem = new ListViewItem(env.Key);
                listViewItem.Name = env.Key;
                listViewItem.SubItems.Add(env.Value);
                listView1.Items.Add(listViewItem);
            }
            listView1.EndUpdate();

            try
            {
                var envsEncrypted = EncryptedText.Encrypt(JsonSerializer.Serialize(envs), boosteraKeyPath);
                var envsJson = JsonSerializer.Serialize(envsEncrypted, jsonSerializerOptions);
                File.WriteAllText(Path.Combine(Program.BoosteraDataFolder, "env.json"), envsJson);
            }
            catch { }

            Task.Run(() =>
            {
                var id = Guid.NewGuid().ToString("N");
                running.Add(id);

                envs.ForEach(env =>
                {
                    try
                    {
                        Environment.SetEnvironmentVariable(env.Key, env.Value, EnvironmentVariableTarget.User);
                    }
                    catch { }
                });
                running.Remove(id);
            });

            MessageBox.Show("環境変数をインポートしました。", "Boostera");
        }
    }
}
