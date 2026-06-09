using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static Google.Cloud.Firestore.V1.Pipeline.Types;
using System.Drawing.Drawing2D;



namespace WindowsFormsApp9
{
    
    
    public partial class ProductForm : Form
    {
        
        public ProductForm()
        {
            InitializeComponent();

            dataGridView1.Width = 960;
            dataGridView1.Height = 220;

        }

        

        private async void btnSave_Click(object sender, EventArgs e)
        {
            FirestoreDb db = GetFirestoreDb();
            

            Dictionary<string, object> product = new Dictionary<string, object>()
               {
                   { "name", txtProductName.Text },
                   { "price", Convert.ToInt32(txtPrice.Text) },
                   { "available", comboBox1.Text },
                   { "image", txtImage.Text }
               };

            await db.Collection("product").AddAsync(product);

            MessageBox.Show("Product Added Successfully!");
            ClearForm();
            await LoadProducts();//call the load metode
            AvailableCount();//available counter
        }

        private FirestoreDb GetFirestoreDb()
        {
            string credentialPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "firebase-key.json");

            Environment.SetEnvironmentVariable(
                "GOOGLE_APPLICATION_CREDENTIALS",
                credentialPath);

            return FirestoreDb.Create("delivery-7ccaa");
        }

        private void ClearForm()
        {
            selectedDocId = "";

            txtProductName.Clear();
            txtPrice.Clear();
            txtImage.Clear();

            comboBox1.SelectedIndex = -1;
        }

        private void AvailableCount()
        {
            if (dataGridView1.DataSource == null)
                return;

            DataTable dt = (DataTable)dataGridView1.DataSource;

            int yesCount = dt.AsEnumerable()
                             .Count(row =>
                                 row["Available"] != DBNull.Value &&
                                 row["Available"].ToString().ToLower() == "yes");

            label8.Text = yesCount.ToString();
        }

        private DateTime lastSyncTime;
        private async Task LoadProducts() //load methode
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

            label7.Text = snapshot.Documents.Count.ToString();

            // Load data from Firebase...

            dataGridView1.DataSource = dt;

            lastSyncTime = DateTime.Now;

            label9.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        private async void btnLoad_Click(object sender, EventArgs e)
        {
            await LoadProducts();//call the load metode
            label7.Text = dataGridView1.Rows.Count.ToString();//item counter
            AvailableCount();//available counter
            /*dataGridView1.Columns["ID"].Width = 200;
            dataGridView1.Columns["Image"].Width = 350;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;*/
        }
        

        private async void ProductForm_Load(object sender, EventArgs e)
        {
            await LoadProducts();//call the load metode
            label7.Text = dataGridView1.Rows.Count.ToString();//item counter
            AvailableCount();//available counter

        }

        private string selectedDocId = "";
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedDocId))
            {
                MessageBox.Show("Select a product first");
                return;
            }

            FirestoreDb db = GetFirestoreDb();

            DocumentReference docRef = db.Collection("product").Document(selectedDocId);

            Dictionary<string, object> updates = new Dictionary<string, object>()
                {
                      { "name", txtProductName.Text },
                      { "price", Convert.ToInt32(txtPrice.Text) },
                      { "image", txtImage.Text },
                      { "available", comboBox1.Text }
                };

            await docRef.SetAsync(updates);

            MessageBox.Show("Product Updated Successfully");
            ClearForm();
            await LoadProducts();//call the load metode
            AvailableCount();//available counter
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedDocId))
            {
                MessageBox.Show("Select a product first");
                return;
            }

            DialogResult result = MessageBox.Show("Delete this product?","Confirm",MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                FirestoreDb db = GetFirestoreDb();

                await db.Collection("product").Document(selectedDocId).DeleteAsync();

                MessageBox.Show("Product Deleted Successfully");
                ClearForm();
                await LoadProducts();//call the load metode
                AvailableCount();//available counter
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                selectedDocId = row.Cells["ID"].Value.ToString();

                txtProductName.Text = row.Cells["Name"].Value.ToString();
                txtPrice.Text = row.Cells["Price"].Value.ToString();
                txtImage.Text = row.Cells["Image URL"].Value.ToString();

                comboBox1.Text = row.Cells["Available"].Value.ToString();
            }
        }

       

        private void btnDelete_Paint_1(object sender, PaintEventArgs e)
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
            Rectangle textRect = new Rectangle(textLeftOffset, 0, btn.Width - textLeftOffset - xMargin, btn.Height);

            TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, textRect,
                                  Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            
        }

        private void btnSave_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void btnSave_MouseLeave(object sender, EventArgs e)
        {
            
        }

        private void btnSave_MouseClick(object sender, MouseEventArgs e)
        {
            ((Button)sender).Invalidate();
        }

        private void btnSave_MouseEnter(object sender, EventArgs e)
        {
            ((Button)sender).Invalidate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
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
            Color borderColor = pnl.ContainsFocus ? Color.FromArgb(94, 53, 177) : Color.FromArgb(220, 220, 220);
            using (Pen pen = new Pen(borderColor, 1))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void ProductForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Get the position of your panel
            Rectangle shadowRect = panel1.Bounds;

            // 2. Adjust these to increase the shadow size/depth
            shadowRect.Inflate(1, 1);   // Makes the shadow slightly larger than the panel
            shadowRect.Offset(1, 1);    // Move it down and right for depth

            int radius = 15; // Match the panel's radius
            using (GraphicsPath shadowPath = new GraphicsPath())
            {
                shadowPath.AddArc(shadowRect.X, shadowRect.Y, radius, radius, 180, 90);
                shadowPath.AddArc(shadowRect.Right - radius, shadowRect.Y, radius, radius, 270, 90);
                shadowPath.AddArc(shadowRect.Right - radius, shadowRect.Bottom - radius, radius, radius, 0, 90);
                shadowPath.AddArc(shadowRect.X, shadowRect.Bottom - radius, radius, radius, 90, 90);
                shadowPath.CloseFigure();

                // 3. SET SHADOW INTENSITY (Change 40 to 60 for a darker shadow)
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(40, Color.Black)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. DRAW THE SHADOW (drawn on the parent container)
            // Note: To see the shadow, the panel needs a bit of margin from its neighbors
            int shadowOffset = 4;
            int shadowBlur = 10;
            int radius = 15; // Subtle curve for the main plan/panel

            // Create a path for the rounded panel
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(pnl.Width - radius - 1, 0, radius, radius, 270, 90);
            path.AddArc(pnl.Width - radius - 1, pnl.Height - radius - 1, radius, radius, 0, 90);
            path.AddArc(0, pnl.Height - radius - 1, radius, radius, 90, 90);
            path.CloseFigure();

            // 2. FILL THE PANEL BACKGROUND
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                e.Graphics.FillPath(brush, path);
            }
            // 3. OPTIONAL: DRAW A THIN BORDER (Makes it look sharper)
            using (Pen pen = new Pen(Color.FromArgb(230, 230, 230), 4))
            {
                e.Graphics.DrawPath(pen, path);
            }

        }

        private void label7_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

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
            Color borderColor = pnl.ContainsFocus ? Color.FromArgb(94, 53, 177) : Color.FromArgb(220, 220, 220);
            using (Pen pen = new Pen(borderColor, 1))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
    
}
