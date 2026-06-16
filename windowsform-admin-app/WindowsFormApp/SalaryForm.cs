using iTextSharp.text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp9
{
    public partial class SalaryForm : Form
    {
        public SalaryForm()
        {
            InitializeComponent();
        }

        private void SalaryForm_Load(object sender, EventArgs e)
        {

        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
           
            string name = txtName.Text;
            string role = cmbRole.Text;

            double basic;

            if (!double.TryParse(txtBasic.Text, out basic))
            {
                MessageBox.Show("Enter a valid basic salary.");
                return;
            }

            double allowance = 0;

            switch (role)
            {
                case "Manager":
                    allowance = basic * 0.30;
                    break;

                case "Rider":
                    allowance = basic * 0.15;
                    break;

                case "Cashier":
                    allowance = basic * 0.10;
                    break;

                default:
                    allowance = basic * 0.05;
                    break;
            }

            double totalSalary = basic + allowance;

            txtSal.Text =
                "========== PAY SHEET ==========" + Environment.NewLine +
                "Name      : " + name + Environment.NewLine +
                "Role      : " + role + Environment.NewLine +
                "Basic     : Rs. " + basic.ToString("N2") + Environment.NewLine +
                "Allowance : Rs. " + allowance.ToString("N2") + Environment.NewLine +
                "--------------------------------" + Environment.NewLine +
                "Net Salary: Rs. " + totalSalary.ToString("N2");
        
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
            saveFileDialog.FileName = txtName.Text + "_Paysheet.pdf";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Document doc = new Document(PageSize.A4);

                PdfWriter.GetInstance(doc,
                    new FileStream(saveFileDialog.FileName,
                    FileMode.Create));

                doc.Open();

                iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                iTextSharp.text.Font bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                Paragraph title = new Paragraph("EMPLOYEE PAYSHEET", titleFont);
                title.Alignment = Element.ALIGN_CENTER;

                doc.Add(title);
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph(txtSal.Text, bodyFont));

                doc.Close();

                MessageBox.Show("PDF saved successfully!");
            }
        }

        private void btnCalculate_Paint(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;

            // Set high-quality rendering for the smooth "fantastic" look
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Clear the area using the parent's color to avoid artifacts on the corners
            e.Graphics.Clear(btn.Parent.BackColor);

            // 1. DEFINE THE SHAPE (Radius 10 for that professional modern look)
            int radius = 13;
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();

            // 2. COLOR LOGIC (Dynamic based on each button's unique BackColor)
            Color baseColor = btn.BackColor;
            Point clientMousePos = btn.PointToClient(Cursor.Position);

            // If mouse is hovering, lighten the base color automatically
            Color btnColor = btn.ClientRectangle.Contains(clientMousePos)
                         ? ControlPaint.Light(baseColor, 0.2f)
                         : baseColor;
            // 3. DRAW THE BUTTON BODY
            using (SolidBrush brush = new SolidBrush(btnColor))
            {
                e.Graphics.FillPath(brush, path);
            }

            // 4. DRAW THE IMAGE (Middle Left Alignment)
            int iconSize = 25;
            int xMargin = 12;
            if (btn.Image != null)
            {
                int yPos = (btn.Height - iconSize) / 2;
                e.Graphics.DrawImage(btn.Image, xMargin, yPos, iconSize, iconSize);
            }
            // 5. DRAW THE TEXT
            // Offset text so it doesn't overlap the icon
            int textLeftOffset = xMargin + iconSize + 8;
            System.Drawing.Rectangle textRect = new System.Drawing.Rectangle(textLeftOffset, 0, btn.Width - textLeftOffset - xMargin, btn.Height);

            TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, textRect,
                                  Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Clear background to match the parent white panel
            e.Graphics.Clear(pnl.Parent.BackColor);

            // Modern radius (8-10 is best for textboxes)
            int radius = 10;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(pnl.Width - radius - 1, 0, radius, radius, 270, 90);
            path.AddArc(pnl.Width - radius - 1, pnl.Height - radius - 1, radius, radius, 0, 90);
            path.AddArc(0, pnl.Height - radius - 1, radius, radius, 90, 90);
            path.CloseFigure();
            // Fill the "Input Well"
            using (SolidBrush brush = new SolidBrush(pnl.BackColor))
            {
                e.Graphics.FillPath(brush, path);
            }

            // Optional: Add a subtle focus border
            Color borderColor = pnl.ContainsFocus ? Color.FromArgb(94, 53, 177) : Color.FromArgb(116, 0, 184);
            using (Pen pen = new Pen(borderColor, 1))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
    
}
