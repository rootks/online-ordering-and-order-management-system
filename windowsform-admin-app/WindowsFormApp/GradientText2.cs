using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace WindowsFormsApp9
{
    public class GradientText2 : Label
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            using (LinearGradientBrush brush =
                new LinearGradientBrush(
                    this.ClientRectangle,
                    Color.FromArgb(215, 64, 17),
                    Color.FromArgb(255, 207, 14),
                    0f))
            {
                e.Graphics.DrawString(
                    this.Text,
                    this.Font,
                    brush,
                    0,
                    0);
            }
        }
    }
}
