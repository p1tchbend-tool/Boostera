using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace Boostera
{
    public partial class EnvForm : Form
    {
        public string SelectedEnv { get { return textBox2.Text; } }

        private static string targetFolder = string.Empty;
        private static int? initialWidth = null;
        private static int? initialHeight = null;

        private string boosteraKeyPath = Path.Combine(Program.BoosteraDataFolder, "Boostera.Key");
        private string boosteraEnvFileFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".Boostera\env");
        private List<Env> envs = new List<Env>();
        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        private EnvManager envManager = new EnvManager();

        public EnvForm(string boosteraKeyPath)
        {
            InitializeComponent();

            if (initialWidth == null) initialWidth = this.Width;
            this.Width = (int)Math.Round((decimal)initialWidth * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));

            if (initialHeight == null) initialHeight = this.Height;
            this.Height = (int)Math.Round((decimal)initialHeight * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));

            this.FormClosing += (s, e) =>
            {
                if (envManager.IsRunning)
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

            toolTip1.SetToolTip(button1, "テキストボックスの変数と値をユーザー環境変数に追加します。\n反映には再起動が必要な場合があります。");
            toolTip1.SetToolTip(button2, "テキストボックスの変数をユーザー環境変数から削除します。\n反映には再起動が必要な場合があります。");
            toolTip1.SetToolTip(button3, "テキストボックスの変数を選択して、このフォームを閉じます。");
            toolTip1.SetToolTip(button4, "ビューの変数と値をcsvにエクスポートします。");
            toolTip1.SetToolTip(button5, "csvの変数と値をユーザー環境変数にインポートします。\n反映には再起動が必要な場合があります。");

            toolTip1.Draw += (s, e) =>
            {
                e.DrawBackground();
                e.DrawBorder();
                e.DrawText(TextFormatFlags.WordBreak);
            };
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

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox1.Text)) return;

            var result = MessageBox.Show($"環境変数「{textBox2.Text}」を追加してよろしいですか？\n※既存の変数は上書きされます。", "Boostera", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;

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

            await envManager.SetEnv(new Env(textBox2.Text, textBox1.Text));
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text)) return;

            var result = MessageBox.Show($"環境変数「{textBox2.Text}」を削除してよろしいですか？", "Boostera", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;

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

            await envManager.SetEnv(new Env(textBox2.Text, null));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count < 1) return;

            using (var openFileDialog = new OpenFileDialog())
            {
                if (!Directory.Exists(boosteraEnvFileFolder)) Directory.CreateDirectory(boosteraEnvFileFolder);
                if (string.IsNullOrEmpty(targetFolder) || !Directory.Exists(targetFolder)) targetFolder = boosteraEnvFileFolder;

                openFileDialog.InitialDirectory = targetFolder;
                openFileDialog.FileName = "Folder";
                openFileDialog.Filter = "Folder|.";
                openFileDialog.ValidateNames = false;
                openFileDialog.CheckFileExists = false;
                openFileDialog.CheckPathExists = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                targetFolder = Path.GetDirectoryName(openFileDialog.FileName);
            }

            if (string.IsNullOrEmpty(targetFolder) || !Directory.Exists(targetFolder)) return;

            envs.Clear();
            foreach (ListViewItem item in listView1.Items)
            {
                envs.Add(new Env(item.Text, item.SubItems[1].Text));
            }

            try
            {
                envManager.ExportEnvsToCsv(envs, Path.Combine(targetFolder, "env.csv"));
                MessageBox.Show("環境変数を 「env.csv」 にエクスポートしました。", "Boostera");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            var envFilePath = string.Empty;
            using (var openFileDialog = new OpenFileDialog())
            {
                if (!Directory.Exists(boosteraEnvFileFolder)) Directory.CreateDirectory(boosteraEnvFileFolder);
                if (string.IsNullOrEmpty(targetFolder) || !Directory.Exists(targetFolder)) targetFolder = boosteraEnvFileFolder;

                openFileDialog.InitialDirectory = targetFolder;
                openFileDialog.Filter = "CSV|*.csv";

                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                envFilePath = openFileDialog.FileName;
                targetFolder = Path.GetDirectoryName(openFileDialog.FileName);
            }

            if (string.IsNullOrEmpty(envFilePath) || !File.Exists(envFilePath)) return;

            var result = MessageBox.Show($"環境変数をファイルから一括インポートしてよろしいですか？\n※既存の変数はすべて上書きされます。", "Boostera", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;

            try
            {
                envs = envManager.ImportEnvsFromCsv(envFilePath);
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

            await envManager.SetEnvs(envs);
        }
    }
}
