using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static Google.Cloud.Firestore.V1.Pipeline.Types;


namespace WindowsFormsApp9
{
    public partial class BillingForm : Form
    {
        public BillingForm()
        {
            InitializeComponent();
        }

        private double billTotal = 0.0;

        private async Task LoadProducts()
        {
            string credentialPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "firebase-key.json");

            Environment.SetEnvironmentVariable(
                "GOOGLE_APPLICATION_CREDENTIALS",
                credentialPath);

            FirestoreDb db = FirestoreDb.Create("delivery-7ccaa");

            QuerySnapshot snapshot = await db.Collection("product").GetSnapshotAsync();

            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("Name");
            dt.Columns.Add("Price");
            dt.Columns.Add("Available");
            dt.Columns.Add("Image URL");

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                dt.Rows.Add(
                    doc.Id,
                    doc.GetValue<string>("name"),
                    doc.GetValue<long>("price"),
                    doc.GetValue<string>("available"),
                    doc.GetValue<string>("image")
                );
            }

            dataGridView1.DataSource = dt;


            // Set column widths
            dataGridView1.Columns["ID"].Width = 150;
            dataGridView1.Columns["Name"].Width = 100;
            dataGridView1.Columns["Price"].Width = 100;
            dataGridView1.Columns["Available"].Width = 80;
            dataGridView1.Columns["Image URL"].Width = 350;

            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11);

            dataGridView1.RowTemplate.Height = 35;
            dataGridView1.ColumnHeadersHeight = 40;



            // Load data from Firebase...

            dataGridView1.DataSource = dt;
        }

        private async void BillingForm_Load(object sender, EventArgs e)
        {
            await LoadProducts();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the user is clicking an actual data row, not the column headers
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Extract values from your exact DataTable structure ("Name" and "Price")
                string productName = row.Cells["Name"].Value?.ToString();
                string priceStr = row.Cells["Price"].Value?.ToString();

                if (!string.IsNullOrEmpty(productName) && double.TryParse(priceStr, out double itemPrice))
                {
                    // 1. Update the ongoing calculation balance
                    billTotal += itemPrice;
                    // 2. Append the item text line straight into the txtBill receipt window
                    // Pads the string layout neatly so prices align to the right margin
                    txtBill.AppendText($"{productName.PadRight(25)} Rs. {itemPrice:N2}" + Environment.NewLine);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Draw a divider line
            txtBill.AppendText("--------------------------------------------------\n"+ Environment.NewLine);

            // Print out the absolute final total
            txtBill.AppendText($"GRAND TOTAL:".PadRight(25) + $"Rs. {billTotal:N2}\n" + Environment.NewLine);
            txtBill.AppendText("--------------------------------------------------\n" + Environment.NewLine);
            txtBill.AppendText("       Thank you for your order!       \n");

            MessageBox.Show($"Transaction Closed. Total: Rs. {billTotal:N2}", "POS Invoice Calculated", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocument1_PrintPage);

            // Open up the native Windows print preview or send directly to default queue
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument1;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();

                // Reset POS fields ready for the next incoming customer queue
                txtBill.Clear();
                billTotal = 0.0;
            }
        }

        // The internal drawing routine that places the txtBill text directly on the printing canvas
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Use a monospaced font like Courier New so columns line up perfectly
            System.Drawing.Font receiptFont = new System.Drawing.Font("Courier New", 10, System.Drawing.FontStyle.Regular);

            // Draw the text starting at coordinates X=10, Y=10 on the page surface
            e.Graphics.DrawString(txtBill.Text, receiptFont, System.Drawing.Brushes.Black, 10, 10);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            txtBill.Clear();
            billTotal = 0.0;
            MessageBox.Show("POS Counter Reset Successfully.", "Cleared", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void BillingForm_Paint(object sender, PaintEventArgs e)
        {
            if (!(sender is Panel pnl)) return; // safe-guard: ignore non-Panel painters

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(pnl.Parent?.BackColor ?? this.BackColor);

            int radius = 10;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(pnl.Width - radius - 1, 0, radius, radius, 270, 90);
                path.AddArc(pnl.Width - radius - 1, pnl.Height - radius - 1, radius, radius, 0, 90);
                path.AddArc(0, pnl.Height - radius - 1, radius, radius, 90, 90);
                path.CloseFigure();

                using (SolidBrush brush = new SolidBrush(pnl.BackColor))
                    e.Graphics.FillPath(brush, path);

                Color borderColor = pnl.ContainsFocus ? Color.FromArgb(94, 53, 177) : Color.FromArgb(220, 220, 220);
                using (Pen pen = new Pen(borderColor, 1))
                    e.Graphics.DrawPath(pen, path);
            }
        }

        private void btnDelete_Paint(object sender, PaintEventArgs e)
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
            Point clientMousePos = btn.PointToClient(System.Windows.Forms.Cursor.Position);

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
            int iconSize = 24;
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
    }
}

