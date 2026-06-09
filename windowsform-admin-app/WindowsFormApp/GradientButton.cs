using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class GradientButton : Button
{
    public Color Color1 { get; set; } = Color.DeepSkyBlue;
    public Color Color2 { get; set; } = Color.MediumPurple;

    public GradientButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10, FontStyle.Bold);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        using (LinearGradientBrush brush =
            new LinearGradientBrush(
                ClientRectangle,
                Color1,
                Color2,
                LinearGradientMode.Horizontal))
        {
            g.FillRectangle(brush, ClientRectangle);
        }

        TextRenderer.DrawText(
            g,
            Text,
            Font,
            ClientRectangle,
            ForeColor,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter);
    }
}