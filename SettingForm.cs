using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;

namespace Boostera
{
    public partial class SettingForm : Form
    {
        public string TtermproPath { get { return textBox4.Text; } }
        public string TtpmacroPath { get { return textBox1.Text; } }
        public string WinscpPath { get { return textBox6.Text; } }
        public string SearchFolder { get { return textBox2.Text; } }
        public string SearchExclusionFolders { get { return textBox3.Text; } }
        public string BoosteraKeyPath { get { return textBox7.Text; } }
        public bool IsStartUp { get { return checkBox1.Checked; } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ModKey { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Keys Key { get; set; }

        private static int? initialWidth = null;
        private static int? initialHeight = null;

        public SettingForm(string ttermproPath, string ttpmacroPath, string winscpPath, string boosteraKeyPath, string searchFolder, string searchExclusionFolders, bool isStartUp, int modKey, Keys key)
        {
            InitializeComponent();

            if (initialWidth == null) initialWidth = this.Width;
            this.Width = (int)Math.Round((decimal)initialWidth * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));

            if (initialHeight == null) initialHeight = this.Height;
            this.Height = (int)Math.Round((decimal)initialHeight * (this.DeviceDpi / NativeMethods.GetDpiForSystem()));

            label7.Text = string.Empty;
            this.Shown += (s, e) => firstLabel.Focus();

            textBox4.Text = ttermproPath;
            textBox4.TextChanged += (s, e) =>
            {
                var caret = textBox4.SelectionStart;
                textBox4.Text = textBox4.Text.Replace("\"", "");
                textBox4.SelectionStart = caret;
            };
            textBox1.Text = ttpmacroPath;
            textBox1.TextChanged += (s, e) =>
            {
                var caret = textBox1.SelectionStart;
                textBox1.Text = textBox1.Text.Replace("\"", "");
                textBox1.SelectionStart = caret;
            };
            textBox6.Text = winscpPath;
            textBox6.TextChanged += (s, e) =>
            {
                var caret = textBox6.SelectionStart;
                textBox6.Text = textBox6.Text.Replace("\"", "");
                textBox6.SelectionStart = caret;
            };
            textBox7.Text = boosteraKeyPath;
            textBox7.TextChanged += (s, e) =>
            {
                var caret = textBox7.SelectionStart;
                textBox7.Text = textBox7.Text.Replace("\"", "");
                textBox7.SelectionStart = caret;
            };
            textBox2.Text = searchFolder;
            textBox2.TextChanged += (s, e) =>
            {
                var caret = textBox2.SelectionStart;
                textBox2.Text = textBox2.Text.Replace("\"", "");
                textBox2.SelectionStart = caret;
            };
            textBox3.Text = searchExclusionFolders;
            textBox3.TextChanged += (s, e) =>
            {
                var caret = textBox3.SelectionStart;
                textBox3.Text = textBox3.Text.Replace("\"", "");
                textBox3.SelectionStart = caret;
            };

            checkBox1.Checked = isStartUp;
            ModKey = modKey;
            Key = key;
            ChangeHotKeyText(modKey, key);

            textBox5.Enter += (s, e) => label7.Text = "登録するにはキーを押してください。";
            textBox5.Leave += (s, e) => label7.Text = "";
            textBox5.KeyDown += (s, e) =>
            {
                int modkey = 0;
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control) modkey += HotKey.MOD_KEY_CONTROL;
                if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt) modkey += HotKey.MOD_KEY_ALT;
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) modkey += HotKey.MOD_KEY_SHIFT;

                if (modkey == HotKey.MOD_KEY_NONE)
                {
                    label7.Text = "先に修飾子キーを押してください。";
                }
                else if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu || e.KeyCode == Keys.ShiftKey)
                {
                    label7.Text = "修飾子以外のキーも押してください。";
                }
                else
                {
                    ChangeHotKeyText(modkey, e.KeyCode);

                    if (Program.ProgramHotKey.Add(modkey, e.KeyCode, Program.HotKeyShowForm))
                    {
                        Program.ProgramHotKey.Remove(Program.HotKeyShowForm);
                        Program.ProgramHotKey.Add(modkey, e.KeyCode, Program.HotKeyShowForm);
                        label7.Text = "登録に成功しました。";

                        ModKey = modkey;
                        Key = e.KeyCode;
                    }
                    else
                    {
                        label7.Text = "登録に失敗しました。";
                    }
                }
            };
        }

        private void ChangeHotKeyText(int modkey, Keys key)
        {
            textBox5.Text = string.Empty;

            if ((modkey & HotKey.MOD_KEY_CONTROL) == HotKey.MOD_KEY_CONTROL) textBox5.Text = " Control + ";
            if ((modkey & HotKey.MOD_KEY_ALT) == HotKey.MOD_KEY_ALT) textBox5.Text += " Alt + ";
            if ((modkey & HotKey.MOD_KEY_SHIFT) == HotKey.MOD_KEY_SHIFT) textBox5.Text += " Shift + ";

            var kc = new KeysConverter();
            textBox5.Text += kc.ConvertToString(key);
        }

        private void Form2_Load(object sender, EventArgs e)
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
    }
}
