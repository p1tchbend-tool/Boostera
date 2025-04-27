namespace Boostera
{
    partial class EnvForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnvForm));
            timer1 = new System.Windows.Forms.Timer(components);
            listView1 = new System.Windows.Forms.ListView();
            変数 = new System.Windows.Forms.ColumnHeader();
            値 = new System.Windows.Forms.ColumnHeader();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            textBox1 = new System.Windows.Forms.TextBox();
            textBox2 = new System.Windows.Forms.TextBox();
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            button3 = new System.Windows.Forms.Button();
            button4 = new System.Windows.Forms.Button();
            button5 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            // 
            // listView1
            // 
            listView1.BackColor = System.Drawing.Color.FromArgb(44, 44, 44);
            listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { 変数, 値 });
            listView1.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
            listView1.ForeColor = System.Drawing.Color.White;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            listView1.Location = new System.Drawing.Point(11, 120);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.Size = new System.Drawing.Size(960, 330);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = System.Windows.Forms.View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // 変数
            // 
            変数.Text = "変数";
            変数.Width = 200;
            // 
            // 値
            // 
            値.Text = "値";
            値.Width = 760;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = System.Drawing.Color.Transparent;
            label1.Font = new System.Drawing.Font("メイリオ", 9.75F);
            label1.ForeColor = System.Drawing.Color.White;
            label1.Location = new System.Drawing.Point(11, 13);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(48, 20);
            label1.TabIndex = 4;
            label1.Text = "変数：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = System.Drawing.Color.Transparent;
            label2.Font = new System.Drawing.Font("メイリオ", 9.75F);
            label2.ForeColor = System.Drawing.Color.White;
            label2.Location = new System.Drawing.Point(11, 47);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(35, 20);
            label2.TabIndex = 5;
            label2.Text = "値：";
            // 
            // textBox1
            // 
            textBox1.BackColor = System.Drawing.Color.FromArgb(44, 44, 44);
            textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox1.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
            textBox1.ForeColor = System.Drawing.Color.White;
            textBox1.Location = new System.Drawing.Point(65, 45);
            textBox1.Margin = new System.Windows.Forms.Padding(2);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(830, 27);
            textBox1.TabIndex = 10;
            // 
            // textBox2
            // 
            textBox2.BackColor = System.Drawing.Color.FromArgb(44, 44, 44);
            textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox2.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
            textBox2.ForeColor = System.Drawing.Color.White;
            textBox2.Location = new System.Drawing.Point(65, 11);
            textBox2.Margin = new System.Windows.Forms.Padding(2);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(830, 27);
            textBox2.TabIndex = 11;
            // 
            // button1
            // 
            button1.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
            button1.Font = new System.Drawing.Font("メイリオ", 10.2F);
            button1.ForeColor = System.Drawing.Color.White;
            button1.Location = new System.Drawing.Point(906, 10);
            button1.Margin = new System.Windows.Forms.Padding(2, 2, 5, 2);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(65, 28);
            button1.TabIndex = 15;
            button1.Text = "追加";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
            button2.Font = new System.Drawing.Font("メイリオ", 10.2F);
            button2.ForeColor = System.Drawing.Color.White;
            button2.Location = new System.Drawing.Point(906, 45);
            button2.Margin = new System.Windows.Forms.Padding(2, 2, 5, 2);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(65, 28);
            button2.TabIndex = 16;
            button2.Text = "削除";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
            button3.DialogResult = System.Windows.Forms.DialogResult.OK;
            button3.Font = new System.Drawing.Font("メイリオ", 10.2F);
            button3.ForeColor = System.Drawing.Color.White;
            button3.Location = new System.Drawing.Point(906, 80);
            button3.Margin = new System.Windows.Forms.Padding(2, 2, 5, 2);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(65, 28);
            button3.TabIndex = 17;
            button3.Text = "O K";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
            button4.Font = new System.Drawing.Font("メイリオ", 10.2F);
            button4.ForeColor = System.Drawing.Color.White;
            button4.Location = new System.Drawing.Point(65, 80);
            button4.Margin = new System.Windows.Forms.Padding(2, 2, 5, 2);
            button4.Name = "button4";
            button4.Size = new System.Drawing.Size(130, 28);
            button4.TabIndex = 18;
            button4.Text = "エクスポート";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
            button5.Font = new System.Drawing.Font("メイリオ", 10.2F);
            button5.ForeColor = System.Drawing.Color.White;
            button5.Location = new System.Drawing.Point(205, 80);
            button5.Margin = new System.Windows.Forms.Padding(2, 2, 5, 2);
            button5.Name = "button5";
            button5.Size = new System.Drawing.Size(130, 28);
            button5.TabIndex = 19;
            button5.Text = "インポート";
            button5.UseVisualStyleBackColor = false;
            button5.Click += button5_Click;
            // 
            // EnvForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.FromArgb(31, 31, 31);
            ClientSize = new System.Drawing.Size(984, 461);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(listView1);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
            ForeColor = System.Drawing.Color.White;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EnvForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Boostera";
            Load += EnvForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader 変数;
        private System.Windows.Forms.ColumnHeader 値;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
    }
}