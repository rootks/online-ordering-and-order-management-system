using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Cloud.Firestore;
using Google.Type;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Net;


namespace WindowsFormsApp9
{
    public partial class UsersForm : Form
    {
        private FirestoreDb db;
        private string selectedDocId = "";

        public UsersForm()
        {
            InitializeComponent();
            string path = @"firebase-key.json";

            Environment.SetEnvironmentVariable(
                "GOOGLE_APPLICATION_CREDENTIALS",
                path
            );

            db = FirestoreDb.Create("delivery-7ccaa");
        }

        private async void UsersForm_Load(object sender, EventArgs e)
        {
            //Add user role options

            cmbRole.Items.Add("Customer");
            cmbRole.Items.Add("Rider");

            cmbRole.SelectedIndex = -1;

            await LoadUsers();
            //Load users imto combobox
        }

        private async Task LoadUsers() //Load methode
        {
            try
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("User Name");
                dt.Columns.Add("Phone");
                dt.Columns.Add("Role");
                dt.Columns.Add("DocID");


                QuerySnapshot snapshot = await db.Collection("users").GetSnapshotAsync();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    Dictionary<string, object> data =
                        doc.ToDictionary();

                    string name = "";
                    string mobile = "";
                    string role = "";


                    if (data.ContainsKey("name"))
                        name = data["name"].ToString();

                    if (data.ContainsKey("mobile"))
                        mobile = data["mobile"].ToString();

                    if (data.ContainsKey("role"))
                        role = data["role"].ToString();


                
                
                dt.Rows.Add(
                        name,
                        mobile,
                        role,
                        doc.Id
                    );

            }

                dataGridView1.DataSource = dt;

            dataGridView1.Columns["DocID"].Visible = false;

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dataGridView1.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11);

            dataGridView1.RowTemplate.Height = 40;
            dataGridView1.Columns["User Name"].Width = 103;
            dataGridView1.Columns["Phone"].Width = 127;
            dataGridView1.Columns["Role"].Width = 190;


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

               CustomerCount(); //Label update call methodes
               RiderCount();  //Label update call methodes
               label7.Text = (dataGridView1.Rows.Count).ToString();   //Label update call methodes
    }

        private void RiderCount()  //Riders counter methode
        {
            if (dataGridView1.DataSource == null)
                return;

            DataTable dt = (DataTable)dataGridView1.DataSource;

            int yesCount = dt.AsEnumerable()
                             .Count(row =>
                                 row["Role"] != DBNull.Value &&
                                 (row["Role"].ToString().ToLower() == "rider") );

            label11.Text = yesCount.ToString();
        }

        private void CustomerCount()  //Customer counter methode
        {
            if (dataGridView1.DataSource == null)
                return;

            DataTable dt = (DataTable)dataGridView1.DataSource;

            int yesCount = dt.AsEnumerable()
                             .Count(row =>
                                 row["Role"] != DBNull.Value &&
                                 (row["Role"].ToString().ToLower() == "customer"));

            label8.Text = yesCount.ToString();
        }

        private System.DateTime lastSyncTime;
        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void UsersForm_Paint(object sender, PaintEventArgs e)
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

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            await LoadUsers();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            selectedDocId = row.Cells["DocID"].Value.ToString();

            txtUserName.Text = row.Cells["User Name"].Value.ToString();

            txtPhone.Text = row.Cells["Phone"].Value.ToString();

            cmbRole.Text = row.Cells["Role"].Value.ToString();
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedDocId))
                {
                    MessageBox.Show(
                        "Select a User first.");
                    return;
                }

                DocumentReference docRef =
                    db.Collection("users")
                      .Document(selectedDocId);

                Dictionary<string, object> updates =
                    new Dictionary<string, object>()
                {
            { "name", txtUserName.Text.Trim() },
            { "role", cmbRole.Text }
                };

                await docRef.UpdateAsync(updates);

                MessageBox.Show(
                    "User Updated Successfully");

                btnLoad.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            await LoadUsers();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedDocId))
                {
                    MessageBox.Show(
                        "Please select a user first.",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this user?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;

                await db.Collection("users")
                        .Document(selectedDocId)
                        .DeleteAsync();
                MessageBox.Show(
                            "User deleted successfully.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                txtUserName.Clear();
                cmbRole.SelectedIndex = -1;
                selectedDocId = "";

                btnLoad.PerformClick(); // Refresh grid
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            await LoadUsers();
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
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

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
}
