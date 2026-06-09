using System;
using System.Drawing;
using System.Windows.Forms;

public class PasswordTextBox : UserControl
{
    private TextBox txtBox;

    public Color BorderColor { get; set; } = Color.Gray;
    public Color FocusBorderColor { get; set; } = Color.DeepSkyBlue;

    private bool isFocused = false;

    public PasswordTextBox()
    {
        this.Height = 40;

        txtBox = new TextBox();
        txtBox.BorderStyle = BorderStyle.None;
        txtBox.UseSystemPasswordChar = true;

        txtBox.Location = new Point(10, 12);
        txtBox.Width = this.Width - 20;

        txtBox.Enter += (s, e) =>
        {
            isFocused = true;
            Invalidate();
        };

        txtBox.Leave += (s, e) =>
        {
            isFocused = false;
            Invalidate();
        };

        this.Controls.Add(txtBox);

        this.Resize += (s, e) =>
        {
            txtBox.Width = this.Width - 20;
        };

        this.DoubleBuffered = true;
    }

    public string Password
    {
        get { return txtBox.Text; }
        set { txtBox.Text = value; }
    }
    public override string Text
    {
        get
        {
            return txtBox.Text;
        }
        set
        {
            txtBox.Text = value;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Color border =
            isFocused ? FocusBorderColor : BorderColor;

        using (Pen pen = new Pen(border, 2))
        {
            e.Graphics.DrawRectangle(
                pen,
                0,
                0,
                Width - 1,
                Height - 1);
        }
    }

}
