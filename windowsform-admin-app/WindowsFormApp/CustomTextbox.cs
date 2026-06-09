using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

public class BorderTextBox : UserControl
{
    private System.Windows.Forms.TextBox textBox;

    public Color BorderColor { get; set; } = Color.DeepSkyBlue;



    public BorderTextBox()
    {
        textBox = new System.Windows.Forms.TextBox();
        textBox.BorderStyle = BorderStyle.None;
        textBox.Location = new Point(5, 5);
        textBox.Width = Width - 10;

        Controls.Add(textBox);

        Padding = new Padding(2);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        using (Pen pen = new Pen(BorderColor, 2))
        {
            e.Graphics.DrawRectangle(
                pen,
                0,
                0,
                Width - 1,
                Height - 1);
        }
    }

    
    public override string Text
    {
        get
        {
            return textBox.Text;
        }
        set
        {
            textBox.Text = value;
        }
    }



}