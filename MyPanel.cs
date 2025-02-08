using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Boostera
{
    public partial class MyPanel : Control
    {
        [Browsable(true)]
        [Description("角丸の半径")]
        [Category("表示")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Radius
        {
            get
            {
                if (_Radius < 0) return 0;

                var diameter = (uint)_Radius * 2;
                if (Width < diameter || Height < diameter)
                    return ((Width < Height) ? Width : Height) / 2;

                return _Radius;
            }
            set
            {
                _Radius = value;
                Refresh();
            }
        }

        int _Radius = int.MaxValue;

        public MyPanel()
        {
            SetStyle(ControlStyles.Opaque, true);
        }

        protected override void InitLayout()
        {
            Region = GetRegion();
        }

        protected override void OnResize(EventArgs e)
        {
            Region = GetRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            // アンチエイリアスを掛ける
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // バック
            g.FillPath(new SolidBrush(BackColor), GetGraphicsPath(true));

            if (Text != "")
            {
                // フォント
                var displayRect = new Rectangle(0 + Radius / 4, 0 + Radius / 4, Width - (Radius / 2), Height - (Radius / 2));
                var drawFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(Text, Font, new SolidBrush(ForeColor), displayRect, drawFormat);
            }
        }

        protected GraphicsPath GetGraphicsPath(bool forPaint = false)
        {
            int diameter = Radius * 2;
            var gp = new GraphicsPath();

            // 中央
            gp.AddRectangle(new Rectangle(Radius, 0, this.Width - diameter, this.Height));
            // 左
            gp.AddRectangle(new Rectangle(0, Radius, Radius, this.Height - diameter));
            // 右
            gp.AddRectangle(new Rectangle(this.Width - Radius, Radius, Radius, this.Height - diameter));

            // 右上
            gp.AddPie(this.Width - diameter, 0, diameter, diameter, 270, 90);
            // 右下
            gp.AddPie(this.Width - diameter, this.Height - diameter, diameter, diameter, 0, 90);
            // 左下
            gp.AddPie(0, this.Height - diameter, diameter, diameter, 90, 90);
            // 左上
            gp.AddPie(0, 0, diameter, diameter, 180, 90);

            return gp;
        }

        protected Region GetRegion()
        {
            return new Region(GetGraphicsPath());
        }
    }
}
