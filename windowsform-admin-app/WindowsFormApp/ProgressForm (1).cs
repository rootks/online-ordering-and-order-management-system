using Google.Cloud.Firestore;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp9
{
    public partial class ProgressForm : Form
    {
        private FirestoreDb db;

        public ProgressForm()
        {
            InitializeComponent();
        }

        private async void ProgressForm_Load(object sender, EventArgs e)
        {
            // Initializes charts on initial UI window generation
            await LoadPieChart();
        }

        // =========================================================================
        // REDESIGNED EXECUTIVE REPORT GENERATION (PDF)
        // =========================================================================
        private async void btnPrint_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            saveFileDialog.Title = "Export Executive Management Report";
            saveFileDialog.FileName = "Executive_Business_Report_" + DateTime.Now.ToString("yyyyMMdd") + ".pdf";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 1. Initialize Canvas (A4 Portrait with comfortable 40pt margins)
                    Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    doc.Open();

                    // 2. Refreshed Color Palette (Modern "Slate & Emerald" Minimalist Aesthetic)
                    BaseColor primaryColor = new BaseColor(30, 41, 59);       // Deep Slate Blue
                    BaseColor secondaryColor = new BaseColor(71, 85, 105);   // Cool Muted Grey
                    BaseColor accentColor = new BaseColor(16, 185, 129);     // Vibrant Emerald Success
                    BaseColor darkTextColor = new BaseColor(15, 23, 42);     // Rich Charcoal/Jet Black
                    BaseColor lightBgColor = new BaseColor(248, 250, 252);   // Clean Cool Off-White
                    BaseColor borderColor = new BaseColor(226, 232, 240);    // Subtle Light Grey Border

                    // 3. Font Typographic Hierarchy
                    iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.WHITE);
                    iTextSharp.text.Font subTitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, new BaseColor(203, 213, 225));
                    iTextSharp.text.Font sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13, primaryColor);
                    iTextSharp.text.Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
                    iTextSharp.text.Font boldBodyFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, darkTextColor);
                    iTextSharp.text.Font bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, darkTextColor);

                    // --- REFRESHED HEADER: Integrated Solid Minimalist Top Banner Block ---
                    PdfPTable headerBlock = new PdfPTable(1);
                    headerBlock.WidthPercentage = 100;
                    
                    PdfPCell bannerCell = new PdfPCell();
                    bannerCell.BackgroundColor = primaryColor;
                    bannerCell.PaddingLeft = 16f;
                    bannerCell.PaddingRight = 16f;
                    bannerCell.PaddingTop = 20f;
                    bannerCell.PaddingBottom = 20f;
                    bannerCell.Border = 0;

                    Paragraph title = new Paragraph("EXECUTIVE BUSINESS PERFORMANCE", titleFont);
                    title.SpacingAfter = 4f;
                    bannerCell.AddElement(title);

                    Paragraph subtitle = new Paragraph($"Operational Analytics & Intelligence  |  Generated: {DateTime.Now.ToString("F")}", subTitleFont);
                    bannerCell.AddElement(subtitle);
                    
                    headerBlock.AddCell(bannerCell);
                    doc.Add(headerBlock);
                    doc.Add(new Paragraph("\n")); 

                    // Fetch live raw sets from Firebase
                    string path = AppDomain.CurrentDomain.BaseDirectory + @"firebase-key.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                    FirestoreDb db = FirestoreDb.Create("delivery-7ccaa");
                    
                    QuerySnapshot userSnapshot = await db.Collection("users").GetSnapshotAsync();
                    QuerySnapshot prodSnapshot = await db.Collection("product").GetSnapshotAsync();
                    QuerySnapshot orderSnapshot = await db.Collection("orders").GetSnapshotAsync();

                    // --- DATA AGGREGATION ENGINE ---
                    int totalUsers = userSnapshot.Documents.Count;
                    int totalProducts = prodSnapshot.Documents.Count;
                    int totalOrders = orderSnapshot.Documents.Count;

                    double grossRevenue = 0;
                    int completedOrders = 0;
                    int pendingOrders = 0;
                    int unavailableProductsCount = 0;

                    Dictionary<string, int> productVolumeMap = new Dictionary<string, int>();

                    foreach (var docSnap in prodSnapshot.Documents)
                    {
                        if (docSnap.ContainsField("available") && docSnap.GetValue<string>("available").ToLower() == "no")
                            unavailableProductsCount++;
                    }

                    foreach (var docSnap in orderSnapshot.Documents)
                    {
                        Dictionary<string, object> data = docSnap.ToDictionary();
                        string status = data.ContainsKey("status") ? data["status"].ToString().ToLower() : "";
                        if (status == "delivered" || status == "completed") completedOrders++;
                        else pendingOrders++;

                        if (data.ContainsKey("total"))
                        {
                            double.TryParse(data["total"].ToString(), out double orderTotal);
                            grossRevenue += orderTotal;
                        }

                        if (data.ContainsKey("items") && data["items"] is List<object> itemsList)
                        {
                            foreach (var obj in itemsList)
                            {
                                if (obj is Dictionary<string, object> item && item.ContainsKey("productName"))
                                {
                                    string pName = item["productName"].ToString().Trim();
                                    if (productVolumeMap.ContainsKey(pName)) productVolumeMap[pName]++;
                                    else productVolumeMap[pName] = 1;
                                }
                            }
                        }
                    }

                    var topProducts = productVolumeMap.OrderByDescending(key => key.Value).Take(3).ToList();

                    // --- VISUAL METRIC KPI CARDS (4-Column Layout Grid) ---
                    PdfPTable kpiGrid = new PdfPTable(4);
                    kpiGrid.WidthPercentage = 100;
                    kpiGrid.SetWidths(new float[] { 1f, 1f, 1f, 1f });

                    kpiGrid.AddCell(CreateKpiCard("TOTAL REVENUE", $"Rs. {grossRevenue:N2}", accentColor, lightBgColor, borderColor, boldBodyFont, bodyFont));
                    kpiGrid.AddCell(CreateKpiCard("TOTAL ORDERS", totalOrders.ToString(), primaryColor, lightBgColor, borderColor, boldBodyFont, bodyFont));
                    kpiGrid.AddCell(CreateKpiCard("ACTIVE SYSTEM USERS", totalUsers.ToString(), secondaryColor, lightBgColor, borderColor, boldBodyFont, bodyFont));
                    kpiGrid.AddCell(CreateKpiCard("OUT OF STOCK ITEMS", unavailableProductsCount.ToString(), new BaseColor(239, 68, 68), lightBgColor, borderColor, boldBodyFont, bodyFont));

                    doc.Add(kpiGrid);
                    doc.Add(new Paragraph("\n"));

                    // --- VISUAL ACCENT DATA BARS ---
                    Paragraph chartHeading = new Paragraph("PRODUCT DEMAND HIGHLIGHTS", sectionFont);
                    chartHeading.SpacingAfter = 12f;
                    doc.Add(chartHeading);

                    PdfPTable chartTable = new PdfPTable(3);
                    chartTable.WidthPercentage = 100;
                    chartTable.SetWidths(new float[] { 3.5f, 4.5f, 2f });

                    int maxVolume = topProducts.Count > 0 ? topProducts.Max(p => p.Value) : 1;

                    foreach (var product in topProducts)
                    {
                        chartTable.AddCell(new PdfPCell(new Phrase(product.Key, bodyFont)) { Border = 0, Padding = 8, VerticalAlignment = Element.ALIGN_MIDDLE });

                        float ratio = (float)product.Value / maxVolume;
                        PdfPTable progressBar = new PdfPTable(2);
                        progressBar.WidthPercentage = 100;
                        progressBar.SetWidths(new float[] { ratio, 1f - ratio });

                        progressBar.AddCell(new PdfPCell { BackgroundColor = accentColor, Border = 0, FixedHeight = 10f });
                        progressBar.AddCell(new PdfPCell { BackgroundColor = lightBgColor, Border = 0, FixedHeight = 10f });

                        PdfPCell barContainerCell = new PdfPCell(progressBar) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 8 };
                        chartTable.AddCell(barContainerCell);

                        chartTable.AddCell(new PdfPCell(new Phrase($"{product.Value} units", boldBodyFont)) { Border = 0, Padding = 8, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE });
                    }
                    doc.Add(chartTable);
                    doc.Add(new Paragraph("\n\n"));

                    // --- CLEAN DATA SUMMARY TABULAR GRID ---
                    doc.Add(new Paragraph("ORDER FULFILLMENT MATRIX", sectionFont) { SpacingAfter = 10f });
                    
                    PdfPTable orderTable = new PdfPTable(5);
                    orderTable.WidthPercentage = 100;
                    orderTable.SetWidths(new float[] { 2.2f, 3.8f, 2.2f, 2.2f, 2.1f });

                    string[] orderHeaders = { "Customer ID", "Items Logged", "Address Location", "Total Value", "Status" };
                    foreach (string header in orderHeaders)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(header, headerFont));
                        cell.BackgroundColor = primaryColor;
                        cell.Padding = 8f;
                        cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        orderTable.AddCell(cell);
                    }

                    bool alterColorToggle = false; 
                    foreach (var docSnap in orderSnapshot.Documents.Take(5))
                    {
                        Dictionary<string, object> data = docSnap.ToDictionary();
                        string cId = data.ContainsKey("customerId") ? data["customerId"].ToString() : "N/A";
                        string address = data.ContainsKey("address") ? data["address"].ToString() : "N/A";
                        string total = data.ContainsKey("total") ? $"Rs. {data["total"]}" : "Rs. 0";
                        string status = data.ContainsKey("status") ? data["status"].ToString().ToUpper() : "PENDING";

                        string itemsSummary = "";
                        if (data.ContainsKey("items") && data["items"] is List<object> items)
                        {
                            foreach (var obj in items)
                            {
                                if (obj is Dictionary<string, object> item && item.ContainsKey("productName"))
                                    itemsSummary += item["productName"].ToString() + ", ";
                            }
                            itemsSummary = itemsSummary.TrimEnd(',', ' ');
                        }

                        BaseColor rowBgColor = alterColorToggle ? lightBgColor : BaseColor.WHITE;

                        orderTable.AddCell(new PdfPCell(new Phrase(cId, bodyFont)) { Padding = 8f, BorderColor = borderColor, BackgroundColor = rowBgColor, BorderWidth = 0.5f });
                        orderTable.AddCell(new PdfPCell(new Phrase(itemsSummary, bodyFont)) { Padding = 8f, BorderColor = borderColor, BackgroundColor = rowBgColor, BorderWidth = 0.5f });
                        orderTable.AddCell(new PdfPCell(new Phrase(address, bodyFont)) { Padding = 8f, BorderColor = borderColor, BackgroundColor = rowBgColor, BorderWidth = 0.5f });
                        orderTable.AddCell(new PdfPCell(new Phrase(total, bodyFont)) { Padding = 8f, BorderColor = borderColor, BackgroundColor = rowBgColor, BorderWidth = 0.5f });

                        PdfPCell statusCell = new PdfPCell(new Phrase(status, boldBodyFont)) { Padding = 8f, BorderColor = borderColor, BorderWidth = 0.5f };
                        if (status == "DELIVERED" || status == "COMPLETED")
                        {
                            statusCell.BackgroundColor = new BaseColor(209, 250, 229); 
                            statusCell.Phrase.Font.Color = new BaseColor(6, 95, 70);
                        }
                        else
                        {
                            statusCell.BackgroundColor = new BaseColor(254, 243, 199); 
                            statusCell.Phrase.Font.Color = new BaseColor(146, 64, 14);
                        }
                        orderTable.AddCell(statusCell);
                        
                        alterColorToggle = !alterColorToggle;
                    }
                    doc.Add(orderTable);
                    doc.Add(new Paragraph("\n\n"));

                    // --- INSIGHT FOOTER BOX ---
                    doc.Add(new Paragraph("ANALYTICAL CONCLUSIONS", sectionFont) { SpacingAfter = 10f });

                    PdfPTable conclusionBox = new PdfPTable(1);
                    conclusionBox.WidthPercentage = 100;

                    string topProdName = topProducts.Count > 0 ? topProducts[0].Key : "N/A";
                    double fulfillmentRate = totalOrders > 0 ? ((double)completedOrders / totalOrders) * 100 : 0;

                    string conclusionText =
                        $"• Volume Growth Analysis: System metrics successfully verify an operational volume of {totalOrders} total orders processed via customer channels.\n\n" +
                        $"• Fulfillment Performance: The current system order delivery completion rate is operating at {fulfillmentRate:F1}%. Core attention should look at reducing remaining pending queues ({pendingOrders} items active).\n\n" +
                        $"• Inventory Demand Signals: High-volume logistics data confirms that '{topProdName}' represents the primary driving volume asset within inventory metrics. Conversely, critical attention is immediately advised regarding the {unavailableProductsCount} items flagged as unavailable to prevent ongoing fulfillment bottlenecks.";

                    PdfPCell boxCell = new PdfPCell(new Phrase(conclusionText, bodyFont));
                    boxCell.BackgroundColor = lightBgColor;
                    boxCell.Padding = 14f;
                    boxCell.BorderColor = borderColor;
                    boxCell.BorderWidth = 1f;
                    conclusionBox.AddCell(boxCell);

                    doc.Add(conclusionBox);

                    doc.Close();
                    MessageBox.Show("Professional Executive Analytics Report exported!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed compiling professional layout: {ex.Message}", "Export Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private PdfPCell CreateKpiCard(string title, string value, BaseColor accentColor, BaseColor bgColor, BaseColor borderCol, iTextSharp.text.Font titleFont, iTextSharp.text.Font valueFont)
        {
            PdfPTable cardInner = new PdfPTable(1);
            cardInner.WidthPercentage = 100;

            PdfPCell tCell = new PdfPCell(new Phrase(title, titleFont)) { Border = 0, PaddingTop = 6, PaddingBottom = 4, PaddingLeft = 4 };
            tCell.Phrase.Font.Color = new BaseColor(100, 116, 139); 
            tCell.Phrase.Font.Size = 7.5f;
            cardInner.AddCell(tCell);

            PdfPCell vCell = new PdfPCell(new Phrase(value, valueFont)) { Border = 0, PaddingTop = 2, PaddingBottom = 6, PaddingLeft = 4 };
            vCell.Phrase.Font.Color = accentColor;
            vCell.Phrase.Font.Size = 12f;
            cardInner.AddCell(vCell);

            PdfPCell containerCell = new PdfPCell(cardInner);
            containerCell.BackgroundColor = bgColor;
            containerCell.BorderColor = borderCol;
            containerCell.BorderWidth = 1f;
            containerCell.Padding = 6;

            return containerCell;
        }

        // =========================================================================
        // DATA CHART PROCESSING (LINE & PIE CONTROLS)
        // =========================================================================
        private async void btnLoad_Click(object sender, EventArgs e)
        {
            await LoadPieChart();
        }

        private async Task LoadCharts()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"firebase-key.json";
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb db = FirestoreDb.Create("delivery-7ccaa");

                Dictionary<string, int> dailyOrderCounts = new Dictionary<string, int>();
                QuerySnapshot snapshot = await db.Collection("orders").GetSnapshotAsync();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    Dictionary<string, object> data = doc.ToDictionary();
                    if (data.ContainsKey("createdAt") && data["createdAt"] != null)
                    {
                        Timestamp ts = (Timestamp)data["createdAt"];
                        string dateKey = ts.ToDateTime().ToString("yyyy-MM-dd");

                        if (dailyOrderCounts.ContainsKey(dateKey)) dailyOrderCounts[dateKey]++;
                        else dailyOrderCounts[dateKey] = 1;
                    }
                }

                var sortedDailyOrders = dailyOrderCounts.OrderBy(d => d.Key).ToList();

                chart1.Series.Clear();
                chart1.Titles.Clear();
                chart1.ChartAreas.Clear();

                ChartArea chartArea = new ChartArea("MainArea");
                chartArea.AxisX.Title = "Timeline (Days)";
                chartArea.AxisY.Title = "Total Orders Logged";

                chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(230, 230, 230);
                chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(230, 230, 230);
                chartArea.AxisX.LabelStyle.Angle = -45;
                chartArea.AxisX.Interval = 1;

                chart1.ChartAreas.Add(chartArea);
                chart1.Titles.Add("Daily Order Performance Growth Trend");
                chart1.Titles[0].Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);

                Series lineSeries = new Series
                {
                    Name = "Orders Growth",
                    ChartArea = "MainArea",
                    ChartType = SeriesChartType.Line,
                    Color = System.Drawing.Color.FromArgb(26, 54, 93),
                    BorderWidth = 3,
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 8,
                    MarkerColor = System.Drawing.Color.FromArgb(94, 53, 177),
                    IsValueShownAsLabel = true,
                    Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
                };

                foreach (var dayRecord in sortedDailyOrders)
                {
                    lineSeries.Points.AddXY(dayRecord.Key, dayRecord.Value);
                }

                chart1.Series.Add(lineSeries);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed loading data timeline grid: {ex.Message}", "Chart Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadPieChart()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"firebase-key.json";
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb db = FirestoreDb.Create("delivery-7ccaa");

                QuerySnapshot snapshot = await db.Collection("orders").GetSnapshotAsync();

                Dictionary<string, int> dailyOrderCounts = new Dictionary<string, int>();
                Dictionary<string, int> statusCounts = new Dictionary<string, int>();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    Dictionary<string, object> data = doc.ToDictionary();

                    if (data.ContainsKey("createdAt") && data["createdAt"] != null)
                    {
                        Timestamp ts = (Timestamp)data["createdAt"];
                        string dateKey = ts.ToDateTime().ToString("yyyy-MM-dd");
                        if (dailyOrderCounts.ContainsKey(dateKey)) dailyOrderCounts[dateKey]++;
                        else dailyOrderCounts[dateKey] = 1;
                    }

                    if (data.ContainsKey("status") && data["status"] != null)
                    {
                        string status = data["status"].ToString();
                        if (string.IsNullOrEmpty(status)) status = "Pending";
                        if (statusCounts.ContainsKey(status)) statusCounts[status]++;
                        else statusCounts[status] = 1;
                    }
                }

                // --- Render Line Chart ---
                var sortedDailyOrders = dailyOrderCounts.OrderBy(d => d.Key).ToList();
                chart1.Series.Clear();
                chart1.ChartAreas.Clear();
                chart1.Titles.Clear();

                ChartArea area1 = new ChartArea("LineArea");
                chart1.ChartAreas.Add(area1);
                chart1.Titles.Add("Daily Order Increments");
                chart1.Titles[0].Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);

                Series lineSeries = new Series("Orders")
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 3,
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 8
                };

                foreach (var day in sortedDailyOrders)
                {
                    lineSeries.Points.AddXY(day.Key, day.Value);
                }
                chart1.Series.Add(lineSeries);

                // --- Render Pie Chart ---
                chart2.Series.Clear();
                chart2.ChartAreas.Clear();
                chart2.Titles.Clear();

                ChartArea area2 = new ChartArea("PieArea");
                chart2.ChartAreas.Add(area2);

                chart2.Titles.Add("Order Status Fulfillment Breakdown");
                chart2.Titles[0].Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);

                Series pieSeries = new Series("StatusDistribution")
                {
                    ChartArea = "PieArea",
                    ChartType = SeriesChartType.Pie,
                    IsValueShownAsLabel = true
                };
                pieSeries["PieLabelStyle"] = "Inside";
                pieSeries["PieDrawingStyle"] = "Default";

                foreach (var statusPair in statusCounts)
                {
                    pieSeries.Points.AddXY(statusPair.Key, statusPair.Value);
                }

                chart2.Series.Add(pieSeries);

                chart2.Legends.Clear();
                chart2.Legends.Add(new Legend("DefaultLegend") { Font = new System.Drawing.Font("Segoe UI", 9) });
                pieSeries.Legend = "DefaultLegend";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed plotting metrics: {ex.Message}", "UI Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================================
        // ADVANCED USER INTERFACE PAINT CUSTOMIZATIONS (GDI+)
        // =========================================================================
        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(pnl.Parent.BackColor);

            int radius = 10;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(pnl.Width - radius - 1, 0, radius, radius, 270, 90);
            path.AddArc(pnl.Width - radius - 1, pnl.Height - radius - 1, radius, radius, 0, 90);
            path.AddArc(0, pnl.Height - radius - 1, radius, radius, 90, 90);
            path.CloseFigure();

            using (SolidBrush brush = new SolidBrush(pnl.BackColor))
            {
                e.Graphics.FillPath(brush, path);
            }

            Color borderColor = pnl.ContainsFocus ? Color.FromArgb(94, 53, 177) : Color.FromArgb(220, 220, 220);
            using (Pen pen = new Pen(borderColor, 1))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void btnLoad_Paint(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.Clear(btn.Parent.BackColor);

            int radius = 13;
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();

            Color baseColor = btn.BackColor;
            Point clientMousePos = btn.PointToClient(System.Windows.Forms.Cursor.Position);

            Color btnColor = btn.ClientRectangle.Contains(clientMousePos)
                             ? ControlPaint.Light(baseColor, 0.2f)
                             : baseColor;

            using (SolidBrush brush = new SolidBrush(btnColor))
            {
                e.Graphics.FillPath(brush, path);
            }

            int iconSize = 24;
            int xMargin = 12;
            if (btn.Image != null)
            {
                int yPos = (btn.Height - iconSize) / 2;
                e.Graphics.DrawImage(btn.Image, xMargin, yPos, iconSize, iconSize);
            }

            int textLeftOffset = xMargin + iconSize + 8;
            System.Drawing.Rectangle textRect = new System.Drawing.Rectangle(textLeftOffset, 0, btn.Width - textLeftOffset - xMargin, btn.Height);

            TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, textRect,
                                  Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }
    }
}