using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Boostera
{
    public partial class EnvForm : Form
    {
        public string SelectedEnv { get { return textBox2.Text; } }
        private static int? initialWidth = null;
        private static int? initialHeight = null;
        private List<string> running = new List<string>();

        public EnvForm()
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
        }

        private void EnvForm_Load(object sender, EventArgs e)
        {
            var value = 1;
            NativeMethods.DwmSetWindowAttribute(
                this.Handle, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, (uint)Marshal.SizeOf(typeof(int)));

            Program.ChangeFont(this);
            Program.SortTabIndex(this);

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Width = (int)Math.Round((decimal)initialWidth * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));
            this.Height = (int)Math.Round((decimal)initialHeight * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));
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

            var item = new ListViewItem(textBox2.Text);
            item.Name = textBox2.Text;
            item.SubItems.Add(textBox1.Text);
            listView1.Items.Add(item);
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
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
