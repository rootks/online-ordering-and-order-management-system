using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class GradienText : Label
{
    protected override void OnPaint(PaintEventArgs e)
    {
        using (LinearGradientBrush brush =
            new LinearGradientBrush(
                this.ClientRectangle,
                Color.DeepSkyBlue,
                Color.MediumPurple,
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