using Google.Cloud.Firestore;
using Google.Type;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp9
{
    public partial class OrderForm : Form
    {
        private FirestoreDb db;
        private string selectedDocId = "";
        public OrderForm()
        {
            InitializeComponent();
            string path = @"firebase-key.json";

            Environment.SetEnvironmentVariable(
                "GOOGLE_APPLICATION_CREDENTIALS",
                path
            );

            db = FirestoreDb.Create("delivery-7ccaa");
        }
        private async Task LoadOrders() //Load methode
        {
            try
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("Customer ID");
                dt.Columns.Add("Address");
                dt.Columns.Add("Products");
                dt.Columns.Add("Total(Rs.)");
                dt.Columns.Add("Status");
                dt.Columns.Add("Rider ID");
                dt.Columns.Add("Created At");
                dt.Columns.Add("DocID");


                QuerySnapshot snapshot = await db.Collection("orders").GetSnapshotAsync();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    Dictionary<string, object> data =
                        doc.ToDictionary();

                    string customerId = "";
                    string address = "";
                    string products = "";
                    string total = "";
                    string status = "";
                    string riderId = "";
                    string createdAt = "";

                    if (data.ContainsKey("customerId"))
                        customerId = data["customerId"].ToString();

                    if (data.ContainsKey("address"))
                        address = data["address"].ToString();

                    if (data.ContainsKey("total"))
                        total = data["total"].ToString();

                    if (data.ContainsKey("status"))
                        status = data["status"].ToString();

                    if (data.ContainsKey("riderId"))
                        riderId = data["riderId"].ToString();

                    if (data.ContainsKey("createdAt"))
                    {
                        Timestamp ts =
                            (Timestamp)data["createdAt"];

                        createdAt =
                            ts.ToDateTime()
                              .ToString("yyyy-MM-dd HH:mm");
                    }
                    if (data.ContainsKey("items"))
                    {
                        var items =
                            data["items"] as List<object>;

                        if (items != null)
                        {
                            foreach (var obj in items)
                            {
                                var item =
                                    obj as Dictionary<string, object>;

                                if (item != null &&
                                    item.ContainsKey("productName"))
                                {
                                    products +=
                                        item["productName"]
                                        .ToString() + ", ";
                                }
                            }

                            products =
                                products.TrimEnd(',', ' ');
                        }
                    }
                    dt.Rows.Add(
                            customerId,
                            address,
                            products,
                            total,
                            status,
                            riderId,
                            createdAt,
                            doc.Id
                        );

                }

                dataGridView1.DataSource = dt;

                dataGridView1.Columns["DocID"].Visible = false;

                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                dataGridView1.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11);

                dataGridView1.RowTemplate.Height = 40;
                dataGridView1.Columns["Customer ID"].Width = 103;
                dataGridView1.Columns["Address"].Width = 127;
                dataGridView1.Columns["Products"].Width = 190;
                dataGridView1.Columns["Total(Rs.)"].Width = 70;
                dataGridView1.Columns["Status"].Width = 80;
                dataGridView1.Columns["Rider ID"].Width = 115;
                dataGridView1.Columns["Created At"].Width = 130;

                /*MessageBox.Show(
                    "Orders Loaded Successfully",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );*/
                // Last synce...

                dataGridView1.DataSource = dt;

                lastSyncTime = System.DateTime.Now;

                label9.Text = System.DateTime.Now.ToString("HH:mm:ss");


            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            NotDeliverCount(); //Label update call methodes
            DeliveredCount();  //Label update call methodes
            label7.Text = (dataGridView1.Rows.Count-1).ToString();   //Label update call methodes


        }

        private void NotDeliverCount()  //Not Complete oreder counter methode
        {
            if (dataGridView1.DataSource == null)
                return;

            DataTable dt = (DataTable)dataGridView1.DataSource;

            int yesCount = dt.AsEnumerable()
                             .Count(row =>
                                 row["Status"] != DBNull.Value &&
                                 (row["Status"].ToString().ToLower() == "pending") ||
                                  (row["Status"].ToString().ToLower() == "preparing")||
                                  (row["Status"].ToString().ToLower() == "delivering"));

            label8.Text = yesCount.ToString();
        }
        private void DeliveredCount() //Complete oreder counter methode
        {
            if (dataGridView1.DataSource == null)
                return;

            DataTable dt = (DataTable)dataGridView1.DataSource;

            int yesCount = dt.AsEnumerable()
                             .Count(row =>
                                 row["Status"] != DBNull.Value &&
                                 (row["Status"].ToString().ToLower() == "delivered")); 
                                  

            label3.Text = yesCount.ToString();
        }
        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void OrderForm_Load(object sender, EventArgs e)
        {
            //Add order sts options

            cmbStatus.Items.Add("Pending");
            cmbStatus.Items.Add("Preparing");
            cmbStatus.Items.Add("Delivering");
            cmbStatus.Items.Add("Delivered");

            cmbStatus.SelectedIndex = 0;

            await LoadOrders();
            //Load Product imto combobox


        }

        private void LoadProducts()
        { 
            cmbStatus.Items.Add("Pending");
            cmbStatus.Items.Add("Picked");
            cmbStatus.Items.Add("Delivering");
            cmbStatus.Items.Add("Confirmed");
        }
        

        private void cmbProducts_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private System.DateTime lastSyncTime;
        private async void btnLoad1_Click(object sender, EventArgs e)
        {
            await LoadOrders(); //call load method

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            selectedDocId = row.Cells["DocID"].Value.ToString();

            txtAddress.Text = row.Cells["Address"].Value.ToString();

            cmbStatus.Text = row.Cells["Status"].Value.ToString();
        }

        private async void btnUpdate1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedDocId))
                {
                    MessageBox.Show(
                        "Select an order first.");
                    return;
                }

                DocumentReference docRef =
                    db.Collection("orders")
                      .Document(selectedDocId);

                Dictionary<string, object> updates =
                    new Dictionary<string, object>()
                {
            { "address", txtAddress.Text.Trim() },
            { "status", cmbStatus.Text }
                };

                await docRef.UpdateAsync(updates);

                MessageBox.Show(
                    "Order Updated Successfully");

                btnLoad1.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            await LoadOrders();
        }

        private async void btnDelete1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedDocId))
                {
                    MessageBox.Show(
                        "Please select an order first.",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this order?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;

                await db.Collection("orders")
                        .Document(selectedDocId)
                        .DeleteAsync();
                MessageBox.Show(
                            "Order deleted successfully.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                txtAddress.Clear();
                cmbStatus.SelectedIndex = -1;
                selectedDocId = "";

                btnLoad1.PerformClick(); // Refresh grid
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            await LoadOrders();
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
            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.White))
            {
                e.Graphics.FillPath(brush, path);
            }
            // 3. OPTIONAL: DRAW A THIN BORDER (Makes it look sharper)
            using (Pen pen = new Pen(System.Drawing.Color.FromArgb(230, 230, 230), 4))
            {
                e.Graphics.DrawPath(pen, path);
            }
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
            System.Drawing.Color borderColor = pnl.ContainsFocus ? System.Drawing.Color.FromArgb(94, 53, 177) : System.Drawing.Color.FromArgb(220, 220, 220);
            using (Pen pen = new Pen(borderColor, 1))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void btnDelete1_Paint(object sender, PaintEventArgs e)
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
            System.Drawing.Color baseColor = btn.BackColor;
            Point clientMousePos = btn.PointToClient(Cursor.Position);

            // If mouse is hovering, lighten the base color automatically
            System.Drawing.Color btnColor = btn.ClientRectangle.Contains(clientMousePos)
                         ? ControlPaint.Light(baseColor, 0.2f)
                         : baseColor;
            // 3. DRAW THE BUTTON BODY
            using (SolidBrush brush = new SolidBrush(btnColor))
            {
                e.Graphics.FillPath(brush, path);
            }

            // 4. DRAW THE IMAGE (Middle Left Alignment)
            int iconSize = 16;
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
                                  System.Drawing.Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        private void OrderForm_Paint(object sender, PaintEventArgs e)
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
                using (SolidBrush shadowBrush = new SolidBrush(System.Drawing.Color.FromArgb(40, System.Drawing.Color.Black)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
            }
        }

        private void btnSave1_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. DRAW THE SHADOW (drawn on the parent container)
            // Note: To see the shadow, the panel needs a bit of margin from its neighbors
            int shadowOffset = 4;
            int shadowBlur = 10;
            pnl.Width = 207;
            pnl.Height = 67;
            int radius = 15; // Subtle curve for the main plan/panel

            // Create a path for the rounded panel
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(pnl.Width - radius - 1, 0, radius, radius, 270, 90);
            path.AddArc(pnl.Width - radius - 1, pnl.Height - radius - 1, radius, radius, 0, 90);
            path.AddArc(0, pnl.Height - radius - 1, radius, radius, 90, 90);
            path.CloseFigure();

            // 2. FILL THE PANEL BACKGROUND
            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.White))
            {
                e.Graphics.FillPath(brush, path);
            }
            // 3. OPTIONAL: DRAW A THIN BORDER (Makes it look sharper)
            using (Pen pen = new Pen(System.Drawing.Color.FromArgb(230, 230, 230), 4))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
        

}