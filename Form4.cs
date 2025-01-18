using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Boostera
{
    public partial class Form4 : Form
    {
        public string Password { get; set; }

        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            var value = 1;
            NativeMethods.DwmSetWindowAttribute(
                this.Handle, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, (uint)Marshal.SizeOf(typeof(int)));

            Program.ChangeFont(this);
            Program.SortTabIndex(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Password = textBox5.Text;
            this.Close();
        }
    }
}
