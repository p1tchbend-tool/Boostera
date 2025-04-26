namespace Boostera
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            textBox1 = new System.Windows.Forms.TextBox();
            listBox1 = new System.Windows.Forms.ListBox();
            timer1 = new System.Windows.Forms.Timer(components);
            timer2 = new System.Windows.Forms.Timer(components);
            label1 = new System.Windows.Forms.Label();
            timer3 = new System.Windows.Forms.Timer(components);
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            notifyIcon1 = new System.Windows.Forms.NotifyIcon(components);
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            myPanel1 = new MyPanel();
            myPanel2 = new MyPanel();
            myPanel4 = new MyPanel();
            myPanel5 = new MyPanel();
            panel12 = new System.Windows.Forms.Panel();
            panel11 = new System.Windows.Forms.Panel();
            panel2 = new System.Windows.Forms.Panel();
            panel6 = new System.Windows.Forms.Panel();
            panel5 = new System.Windows.Forms.Panel();
            panel10 = new System.Windows.Forms.Panel();
            panel9 = new System.Windows.Forms.Panel();
            panel8 = new System.Windows.Forms.Panel();
            panel4 = new System.Windows.Forms.Panel();
            panel1 = new System.Windows.Forms.Panel();
            panel7 = new System.Windows.Forms.Panel();
            panel3 = new System.Windows.Forms.Panel();
            timer4 = new System.Windows.Forms.Timer(components);
            panel13 = new System.Windows.Forms.Panel();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            textBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            textBox1.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBox1.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
            textBox1.ForeColor = System.Drawing.Color.White;
            textBox1.Location = new System.Drawing.Point(18, 94);
            textBox1.Margin = new System.Windows.Forms.Padding(2, 2, 4, 14);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(724, 41);
            textBox1.TabIndex = 0;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // listBox1
            // 
            listBox1.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            listBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            listBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            listBox1.Font = new System.Drawing.Font("メイリオ", 20F);
            listBox1.ForeColor = System.Drawing.Color.White;
            listBox1.FormattingEnabled = true;
            listBox1.Location = new System.Drawing.Point(9, 152);
            listBox1.Margin = new System.Windows.Forms.Padding(2);
            listBox1.Name = "listBox1";
            listBox1.ScrollAlwaysVisible = true;
            listBox1.Size = new System.Drawing.Size(780, 492);
            listBox1.TabIndex = 1;
            // 
            // timer1
            // 
            timer1.Interval = 20;
            timer1.Tick += timer1_Tick;
            // 
            // timer2
            // 
            timer2.Interval = 600000;
            timer2.Tick += timer2_Tick;
            // 
            // label1
            // 
            label1.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            label1.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
            label1.ForeColor = System.Drawing.Color.Gray;
            label1.Location = new System.Drawing.Point(50, 101);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 6);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(400, 24);
            label1.TabIndex = 3;
            label1.Text = "「SPACE」 で TTL マクロの検索開始...";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            label1.Click += label1_Click;
            // 
            // timer3
            // 
            timer3.Tick += timer3_Tick;
            // 
            // toolTip1
            // 
            toolTip1.BackColor = System.Drawing.Color.FromArgb(31, 31, 31);
            toolTip1.ForeColor = System.Drawing.Color.White;
            toolTip1.OwnerDraw = true;
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = (System.Drawing.Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "Boostera";
            notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // myPanel1
            // 
            myPanel1.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            myPanel1.ForeColor = System.Drawing.Color.White;
            myPanel1.Location = new System.Drawing.Point(9, 86);
            myPanel1.Margin = new System.Windows.Forms.Padding(2, 8, 2, 2);
            myPanel1.Name = "myPanel1";
            myPanel1.Size = new System.Drawing.Size(781, 56);
            myPanel1.TabIndex = 5;
            // 
            // myPanel2
            // 
            myPanel2.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            myPanel2.ForeColor = System.Drawing.Color.White;
            myPanel2.Location = new System.Drawing.Point(345, 20);
            myPanel2.Margin = new System.Windows.Forms.Padding(2);
            myPanel2.Name = "myPanel2";
            myPanel2.Size = new System.Drawing.Size(444, 56);
            myPanel2.TabIndex = 6;
            // 
            // myPanel4
            // 
            myPanel4.BackColor = System.Drawing.Color.FromArgb(100, 100, 100);
            myPanel4.ForeColor = System.Drawing.Color.White;
            myPanel4.Location = new System.Drawing.Point(9, 7);
            myPanel4.Margin = new System.Windows.Forms.Padding(2);
            myPanel4.Name = "myPanel4";
            myPanel4.Size = new System.Drawing.Size(70, 56);
            myPanel4.TabIndex = 15;
            // 
            // myPanel5
            // 
            myPanel5.BackColor = System.Drawing.Color.FromArgb(100, 100, 100);
            myPanel5.ForeColor = System.Drawing.Color.White;
            myPanel5.Location = new System.Drawing.Point(83, 7);
            myPanel5.Margin = new System.Windows.Forms.Padding(2);
            myPanel5.Name = "myPanel5";
            myPanel5.Size = new System.Drawing.Size(60, 56);
            myPanel5.TabIndex = 16;
            // 
            // panel12
            // 
            panel12.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            panel12.BackgroundImage = Properties.Resources.hide;
            panel12.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            panel12.Location = new System.Drawing.Point(571, 33);
            panel12.Margin = new System.Windows.Forms.Padding(2, 2, 4, 8);
            panel12.Name = "panel12";
            panel12.Size = new System.Drawing.Size(30, 30);
            panel12.TabIndex = 19;
            // 
            // panel11
            // 
            panel11.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            panel11.BackgroundImage = Properties.Resources.show;
            panel11.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            panel11.Location = new System.Drawing.Point(607, 33);
            panel11.Margin = new System.Windows.Forms.Padding(2, 2, 4, 8);
            panel11.Name = "panel11";
            panel11.Size = new System.Drawing.Size(30, 30);
            panel11.TabIndex = 18;
            // 
            // panel2
            // 
            panel2.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            panel2.BackgroundImage = Properties.Resources.connect;
            panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            panel2.Location = new System.Drawing.Point(679, 33);
            panel2.Margin = new System.Windows.Forms.Padding(2, 2, 4, 8);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(30, 30);
            panel2.TabIndex = 14;
            // 
            // panel6
            // 
            panel6.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            panel6.BackgroundImage = Properties.Resources.layout_columns_negate;
            panel6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            panel6.Location = new System.Drawing.Point(391, 33);
            panel6.Margin = new System.Windows.Forms.Padding(2, 2, 4, 8);
            panel6.Name = "panel6";
            panel6.Size = new System.Drawing.Size(30, 30);
            panel6.TabIndex = 8;
            // 
            // panel5
            // 
            panel5.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            panel5.BackgroundImage = Properties.Resources.layout_rows_negate;
            panel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            panel5.Location = new System.Drawing.Point(427, 33);
            panel5.Margin = new System.Windows.Forms.Padding(2, 2, 4, 8);
            panel5.Name = "panel5";
            panel5.Size = new System.Drawing.Size(30, 30);
            panel5.TabIndex = 7;
            // 
            // panel10
            // 
            panel10.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            panel10.BackgroundImage = Properties.Resources.close;
            panel10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            panel10.Location = new System.Drawing.Point(643, 33);
            panel10.Margin = new System.Windows.Forms.Padding(2, 2, 4, 8);
            panel10.Name = "panel10";
            panel10.Size = new System.Drawing.Size(30, 30);
            panel10.TabIndex = 12;
            // 
            // panel9
            // 
            panel9.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            panel9.BackgroundImage = (System.Drawing.Image)resources.GetObject("panel9.BackgroundImage");
            panel9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            panel9.Location = new System.Drawing.Point(463, 33);
            panel9.Margin = new System.Windows.Forms.Padding(2, 2, 4, 8);
            panel9.Name = "panel9";
            panel9.Size = new System.Drawing.Size(30, 30);
            panel9.TabIndex = 11;
            // 
            // panel8
            // 
            panel8.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            panel8.BackgroundImage = (System.Drawing.Image)resources.GetObject("panel8.BackgroundImage");
            panel8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            panel8.Location = new System.Drawing.Point(499, 33);
            panel8.Margin = new System.Windows.Forms.Padding(2, 2, 4, 8);
            panel8.Name = "panel8";
            panel8.Size = new System.Drawing.Size(30, 30);
            panel8.TabIndex = 10;
            // 
            // panel4
            // 
            panel4.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            panel4.BackgroundImage = Properties.Resources.minimize;
            panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            panel4.Location = new System.Drawing.Point(535, 33);
            panel4.Margin = new System.Windows.Forms.Padding(2, 2, 4, 8);
            panel4.Name = "panel4";
            panel4.Size = new System.Drawing.Size(30, 30);
            panel4.TabIndex = 9;
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            panel1.BackgroundImage = Properties.Resources.search;
            panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            panel1.Location = new System.Drawing.Point(748, 99);
            panel1.Margin = new System.Windows.Forms.Padding(2);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(30, 30);
            panel1.TabIndex = 2;
            // 
            // panel7
            // 
            panel7.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            panel7.BackgroundImage = Properties.Resources.arrow_negate;
            panel7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            panel7.Location = new System.Drawing.Point(355, 33);
            panel7.Margin = new System.Windows.Forms.Padding(2, 2, 4, 8);
            panel7.Name = "panel7";
            panel7.Size = new System.Drawing.Size(30, 30);
            panel7.TabIndex = 10;
            // 
            // panel3
            // 
            panel3.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            panel3.BackgroundImage = Properties.Resources.setting;
            panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            panel3.Location = new System.Drawing.Point(751, 33);
            panel3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(30, 30);
            panel3.TabIndex = 5;
            // 
            // timer4
            // 
            timer4.Interval = 500;
            timer4.Tick += timer4_Tick;
            // 
            // panel13
            // 
            panel13.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);
            panel13.BackgroundImage = (System.Drawing.Image)resources.GetObject("panel13.BackgroundImage");
            panel13.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            panel13.Location = new System.Drawing.Point(715, 33);
            panel13.Margin = new System.Windows.Forms.Padding(2, 2, 4, 8);
            panel13.Name = "panel13";
            panel13.Size = new System.Drawing.Size(30, 30);
            panel13.TabIndex = 20;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.Green;
            ClientSize = new System.Drawing.Size(810, 670);
            ControlBox = false;
            Controls.Add(panel13);
            Controls.Add(panel12);
            Controls.Add(panel11);
            Controls.Add(panel2);
            Controls.Add(panel6);
            Controls.Add(panel5);
            Controls.Add(panel10);
            Controls.Add(panel9);
            Controls.Add(panel8);
            Controls.Add(panel4);
            Controls.Add(label1);
            Controls.Add(panel1);
            Controls.Add(panel7);
            Controls.Add(listBox1);
            Controls.Add(textBox1);
            Controls.Add(panel3);
            Controls.Add(myPanel1);
            Controls.Add(myPanel2);
            Controls.Add(myPanel4);
            Controls.Add(myPanel5);
            DoubleBuffered = true;
            ForeColor = System.Drawing.SystemColors.ControlText;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(2);
            MaximizeBox = false;
            Name = "MainForm";
            Opacity = 0D;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel10;
        private MyPanel myPanel1;
        private MyPanel myPanel2;
        private System.Windows.Forms.Panel panel2;
        private MyPanel myPanel4;
        private MyPanel myPanel5;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Timer timer4;
        private System.Windows.Forms.Panel panel13;
    }
}

