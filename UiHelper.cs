using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Boostera
{
    public class UiHelper
    {
        public static void SetDarkMode(Form form, bool isDarkMode)
        {
            if (form == null) return;

            int attribute = isDarkMode ? 1 : 0;
            NativeMethods.DwmSetWindowAttribute(form.Handle, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, ref attribute, sizeof(int));
        }

        public static void AdjustDpi(Form form, int? initialWidth, int? initialHeight)
        {
            if (form == null || initialWidth == null || initialHeight == null) return;

            form.Width = (int)Math.Round((decimal)initialWidth * (form.DeviceDpi / NativeMethods.GetDpiForSystem()));
            form.Height = (int)Math.Round((decimal)initialHeight * (form.DeviceDpi / NativeMethods.GetDpiForSystem()));
        }

        public static void SortTabIndex(Control control)
        {
            if (control == null) return;

            var children = new List<Control>();
            foreach (Control child in control.Controls) children.Add(child);
            children.Sort((x, y) =>
            {
                if (x.Top == y.Top) return x.Left.CompareTo(y.Left);
                return x.Top.CompareTo(y.Top);
            });
            for (int i = 0; i < children.Count; i++) children[i].TabIndex = i;
        }

        public static void ChangeFontFamily(Control control, string fontFamilyName)
        {
            if (control == null || string.IsNullOrEmpty(fontFamilyName)) return;

            Install(control);
            void Install(Control ctrl)
            {
                foreach (Control c in ctrl.Controls)
                {
                    try
                    {
                        c.Font = new Font(fontFamilyName, c.Font.SizeInPoints, c.Font.Style);
                        Install(c);
                    }
                    catch { }
                }
            }
        }
    }
}
