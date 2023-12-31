﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Boostera
{
    public partial class Form2 : Form
    {
        public string TtermproPath { get { return textBox4.Text; } }
        public string TtpmacroPath { get { return textBox1.Text; } }
        public string WinscpPath { get { return textBox6.Text; } }
        public string SearchFolder { get { return textBox2.Text; } }
        public string SearchExclusionFolders { get { return textBox3.Text; } }
        public string BoosteraKeyPath { get { return textBox7.Text; } }
        public bool IsStartUp { get { return checkBox1.Checked; } }
        public int ModKey { get; set; }
        public Keys Key { get; set; }

        public Form2(string ttermproPath, string ttpmacroPath, string winscpPath, string searchFolder, string searchExclusionFolders, string boosteraKeyPath, bool isStartUp, int modKey, Keys key)
        {
            InitializeComponent();
            label7.Text = string.Empty;
            this.Shown += (s, e) => firstLabel.Focus();

            textBox4.Text = ttermproPath;
            textBox4.TextChanged += (s, e) => textBox4.Text = textBox4.Text.Replace("\"", "");
            textBox1.Text = ttpmacroPath;
            textBox1.TextChanged += (s, e) => textBox1.Text = textBox1.Text.Replace("\"", "");
            textBox6.Text = winscpPath;
            textBox6.TextChanged += (s, e) => textBox6.Text = textBox6.Text.Replace("\"", "");
            textBox2.Text = searchFolder;
            textBox2.TextChanged += (s, e) => textBox2.Text = textBox2.Text.Replace("\"", "");
            textBox3.Text = searchExclusionFolders;
            textBox3.TextChanged += (s, e) => textBox3.Text = textBox3.Text.Replace("\"", "");
            textBox7.Text = boosteraKeyPath;
            textBox7.TextChanged += (s, e) => textBox7.Text = textBox7.Text.Replace("\"", "");

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

            if ((modkey & HotKey.MOD_KEY_CONTROL) == HotKey.MOD_KEY_CONTROL) textBox5.Text = "Control + ";
            if ((modkey & HotKey.MOD_KEY_ALT) == HotKey.MOD_KEY_ALT) textBox5.Text += "Alt + ";
            if ((modkey & HotKey.MOD_KEY_SHIFT) == HotKey.MOD_KEY_SHIFT) textBox5.Text += "Shift + ";

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
        }
    }
}
