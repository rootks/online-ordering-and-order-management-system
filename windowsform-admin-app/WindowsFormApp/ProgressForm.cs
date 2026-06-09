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
                    // 1. Initialize Document Canvas (A4 Portrait with 20mm margins)
                    Document doc = new Document(PageSize.A4, 56, 56, 56, 56);
                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    doc.Open();

                    // 2. Define Professional Color Palette (Corporate Navy Aesthetic)
                    BaseColor primaryColor = new BaseColor(26, 54, 93);     // Deep Navy
                    BaseColor secondaryColor = new BaseColor(43, 108, 176); // Slate Blue
                    BaseColor darkTextColor = new BaseColor(45, 55, 72);    // Charcoal
                    BaseColor lightBgColor = new BaseColor(247, 250, 252);  // Off-white/Light Gray
                    BaseColor accentGreen = new BaseColor(56, 161, 105);    // Success Green

                    // 3. Font Typographic Hierarchy
                    iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, primaryColor);
                    iTextSharp.text.Font subTitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);
                    iTextSharp.text.Font sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, primaryColor);
                    iTextSharp.text.Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                    iTextSharp.text.Font boldBodyFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, darkTextColor);
                    iTextSharp.text.Font bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, darkTextColor);

                    // =========================================================================
                    // REPORT HEADER
                    // =========================================================================
                    Paragraph title = new Paragraph("EXECUTIVE BUSINESS PERFORMANCE REPORT", titleFont);
                    title.SpacingAfter = 2f;
                    doc.Add(title);

                    Paragraph subtitle = new Paragraph($"Operational Analytics & Intelligence  |  Generated: {DateTime.Now.ToString("F")}", subTitleFont);
                    subtitle.SpacingAfter = 20f;
                    doc.Add(subtitle);

                    // Subtle divider line
                    PdfPTable lineTable = new PdfPTable(1);
                    lineTable.WidthPercentage = 100;
                    PdfPCell lineCell = new PdfPCell();
                    lineCell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
                    lineCell.BorderWidthBottom = 2f;
                    lineCell.BorderColorBottom = secondaryColor;
                    lineCell.Padding = 0;
                    lineTable.AddCell(lineCell);
                    doc.Add(lineTable);
                    doc.Add(new Paragraph("\n"));

                    // Fetch live raw sets from Firebase
                    FirestoreDb db = FirestoreDb.Create("delivery-7ccaa");
                    QuerySnapshot userSnapshot = await db.Collection("users").GetSnapshotAsync();
                    QuerySnapshot prodSnapshot = await db.Collection("product").GetSnapshotAsync();
                    QuerySnapshot orderSnapshot = await db.Collection("orders").GetSnapshotAsync();

                    // =========================================================================
                    // DATA AGGREGATION & ANALYTICS ENGINE
                    // =========================================================================
                    int totalUsers = userSnapshot.Documents.Count;
                    int totalProducts = prodSnapshot.Documents.Count;
                    int totalOrders = orderSnapshot.Documents.Count;

                    double grossRevenue = 0;
                    int completedOrders = 0;
                    int pendingOrders = 0;
                    int unavailableProductsCount = 0;

                    Dictionary<string, int> productVolumeMap = new Dictionary<string, int>();

                    // Process inventory availability
                    foreach (var docSnap in prodSnapshot.Documents)
                    {
                        if (docSnap.ContainsField("available") && docSnap.GetValue<string>("available").ToLower() == "no")
                            unavailableProductsCount++;
                    }

                    // Process order performance indicators & trends
                    foreach (var docSnap in orderSnapshot.Documents)
                    {
                        Dictionary<string, object> data = docSnap.ToDictionary();

                        // Track Status KPIs
                        string status = data.ContainsKey("status") ? data["status"].ToString().ToLower() : "";
                        if (status == "delivered" || status == "completed") completedOrders++;
                        else pendingOrders++;

                        // Track Financial Metrics
                        if (data.ContainsKey("total"))
                        {
                            double.TryParse(data["total"].ToString(), out double orderTotal);
                            grossRevenue += orderTotal;
                        }

                        // Parse item collections for high-volume demand trend processing
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

                    // Sort map values to obtain top performing line items
                    var topProducts = productVolumeMap.OrderByDescending(key => key.Value).Take(3).ToList();

                    // =========================================================================
                    // VISUAL EXECUTIVE KPI CARDS (2x2 Matrix Grid)
                    // =========================================================================
                    PdfPTable kpiGrid = new PdfPTable(4);
                    kpiGrid.WidthPercentage = 100;
                    kpiGrid.SetWidths(new float[] { 1f, 1f, 1f, 1f });

                    kpiGrid.AddCell(CreateKpiCard("TOTAL REVENUE", $"Rs. {grossRevenue:N2}", accentGreen, lightBgColor, boldBodyFont, bodyFont));
                    kpiGrid.AddCell(CreateKpiCard("TOTAL ORDERS", totalOrders.ToString(), secondaryColor, lightBgColor, boldBodyFont, bodyFont));
                    kpiGrid.AddCell(CreateKpiCard("ACTIVE SYSTEM USERS", totalUsers.ToString(), primaryColor, lightBgColor, boldBodyFont, bodyFont));
                    kpiGrid.AddCell(CreateKpiCard("OUT OF STOCK ITEMS", unavailableProductsCount.ToString(), BaseColor.RED, lightBgColor, boldBodyFont, bodyFont));

                    doc.Add(kpiGrid);
                    doc.Add(new Paragraph("\n"));

                    // =========================================================================
                    // VISUAL GRAPH / DEMAND CHART REPRESENTATION (iText-Native Percentage Bars)
                    // =========================================================================
                    Paragraph chartHeading = new Paragraph("PRODUCT DEMAND & VOLUME HIGHLIGHTS", sectionFont);
                    chartHeading.SpacingAfter = 10f;
                    doc.Add(chartHeading);

                    PdfPTable chartTable = new PdfPTable(3);
                    chartTable.WidthPercentage = 100;
                    chartTable.SetWidths(new float[] { 3f, 5f, 2f });

                    int maxVolume = topProducts.Count > 0 ? topProducts.Max(p => p.Value) : 1;

                    foreach (var product in topProducts)
                    {
                        chartTable.AddCell(new PdfPCell(new Phrase(product.Key, bodyFont)) { Border = 0, Padding = 6 });

                        // Programmatic visual volume indicator bar using cell backgrounds
                        float ratio = (float)product.Value / maxVolume;
                        PdfPTable progressBar = new PdfPTable(2);
                        progressBar.WidthPercentage = 100;
                        progressBar.SetWidths(new float[] { ratio, 1f - ratio });

                        progressBar.AddCell(new PdfPCell { BackgroundColor = secondaryColor, Border = 0, FixedHeight = 12f });
                        progressBar.AddCell(new PdfPCell { Border = 0, FixedHeight = 12f });

                        PdfPCell barContainerCell = new PdfPCell(progressBar) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 6 };
                        chartTable.AddCell(barContainerCell);

                        chartTable.AddCell(new PdfPCell(new Phrase($"{product.Value} units ordered", boldBodyFont)) { Border = 0, Padding = 6, HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                    doc.Add(chartTable);
                    doc.Add(new Paragraph("\n\n"));

                    // =========================================================================
                    // DATA SUMMARY SECTIONS (Clean Tabular Reports)
                    // =========================================================================

                    // --- 1. RECENT ORDERS TABLE ---
                    doc.Add(new Paragraph("ORDER FULFILLMENT MATRIX", sectionFont) { SpacingAfter = 8f });
                    PdfPTable orderTable = new PdfPTable(5);
                    orderTable.WidthPercentage = 100;
                    orderTable.SetWidths(new float[] { 2f, 4f, 2f, 2f, 2.5f });

                    string[] orderHeaders = { "Customer ID", "Items", "Address", "Total Value", "Status" };
                    foreach (string header in orderHeaders)
                    {
                        orderTable.AddCell(new PdfPCell(new Phrase(header, headerFont)) { BackgroundColor = primaryColor, Padding = 6 });
                    }

                    foreach (var docSnap in orderSnapshot.Documents.Take(5)) // Limit to top 5 for executive brevity
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

                        orderTable.AddCell(new PdfPCell(new Phrase(cId, bodyFont)) { Padding = 5 });
                        orderTable.AddCell(new PdfPCell(new Phrase(itemsSummary, bodyFont)) { Padding = 5 });
                        orderTable.AddCell(new PdfPCell(new Phrase(address, bodyFont)) { Padding = 5 });
                        orderTable.AddCell(new PdfPCell(new Phrase(total, bodyFont)) { Padding = 5 });

                        // Contextual coloring for status indicators
                        PdfPCell statusCell = new PdfPCell(new Phrase(status, boldBodyFont)) { Padding = 5 };
                        if (status == "DELIVERED" || status == "COMPLETED") statusCell.BackgroundColor = new BaseColor(225, 245, 234);
                        else statusCell.BackgroundColor = new BaseColor(254, 243, 199);
                        orderTable.AddCell(statusCell);
                    }
                    doc.Add(orderTable);
                    doc.Add(new Paragraph("\n\n"));

                    // =========================================================================
                    // STRATEGIC CONCLUSIONS & MANAGEMENT RECOMMENDATIONS
                    // =========================================================================
                    doc.Add(new Paragraph("ANALYTICAL CONCLUSIONS & INSIGHTS", sectionFont) { SpacingAfter = 8f });

                    PdfPTable conclusionBox = new PdfPTable(1);
                    conclusionBox.WidthPercentage = 100;

                    string topProdName = topProducts.Count > 0 ? topProducts[0].Key : "N/A";
                    double fulfillmentRate = totalOrders > 0 ? ((double)completedOrders / totalOrders) * 100 : 0;

                    string conclusionText =
                        $"• Volume Growth Analysis: System metrics successfully verify a operational volume of {totalOrders} total orders processed via customer channels.\n\n" +
                        $"• Fulfillment Performance: The current system order delivery completion rate is operating at {fulfillmentRate:F1}%. Core attention should look at reducing remaining pending queues ({pendingOrders} items active).\n\n" +
                        $"• Inventory Demand Signals: High-volume logistics data confirms that '{topProdName}' represents the primary driving volume asset within inventory metrics. Conversely, critical attention is immediately advised regarding the {unavailableProductsCount} items flagged as unavailable to prevent ongoing fulfillment bottlenecks.";

                    PdfPCell boxCell = new PdfPCell(new Phrase(conclusionText, bodyFont));
                    boxCell.BackgroundColor = lightBgColor;
                    boxCell.Padding = 12;
                    boxCell.BorderColor = secondaryColor;
                    boxCell.BorderWidth = 1f;
                    conclusionBox.AddCell(boxCell);

                    doc.Add(conclusionBox);

                    // 4. Wrap up stream structures
                    doc.Close();
                    MessageBox.Show("Professional Executive Analytics Report exported!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed compiling professional layout: {ex.Message}", "Export Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Helper Routine to programmatically construct isolated KPI Cards
        private PdfPCell CreateKpiCard(string title, string value, BaseColor accentColor, BaseColor bgColor, iTextSharp.text.Font titleFont, iTextSharp.text.Font valueFont)
        {
            PdfPTable cardInner = new PdfPTable(1);
            cardInner.WidthPercentage = 100;

            PdfPCell tCell = new PdfPCell(new Phrase(title, titleFont)) { Border = 0, PaddingTop = 4, PaddingBottom = 2, PaddingLeft = 6 };
            tCell.Phrase.Font.Color = BaseColor.GRAY;
            cardInner.AddCell(tCell);

            PdfPCell vCell = new PdfPCell(new Phrase(value, valueFont)) { Border = 0, PaddingTop = 2, PaddingBottom = 6, PaddingLeft = 6 };
            vCell.Phrase.Font.Color = accentColor;
            vCell.Phrase.Font.Size = 13;
            cardInner.AddCell(vCell);

            PdfPCell containerCell = new PdfPCell(cardInner);
            containerCell.BackgroundColor = bgColor;
            containerCell.BorderColor = new BaseColor(226, 232, 240);
            containerCell.BorderWidth = 1f;
            containerCell.Padding = 6;

            return containerCell;
        }

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

                // 1. Initialize a temporary dictionary to hold: Short Date -> Count of Orders
                Dictionary<string, int> dailyOrderCounts = new Dictionary<string, int>();

                // 2. Fetch the live order collection snapshot from your Firestore DB
                QuerySnapshot snapshot = await db.Collection("orders").GetSnapshotAsync();


                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    Dictionary<string, object> data = doc.ToDictionary();

                    // Extract the timestamp using your exact database key logic
                    if (data.ContainsKey("createdAt") && data["createdAt"] != null)
                    {
                        Timestamp ts = (Timestamp)data["createdAt"];

                        // Format timestamp to extract only the Calendar Date string
                        string dateKey = ts.ToDateTime().ToString("yyyy-MM-dd");

                        // Increment the count for this specific day
                        if (dailyOrderCounts.ContainsKey(dateKey))
                            dailyOrderCounts[dateKey]++;
                        else
                            dailyOrderCounts[dateKey] = 1;
                    }
                }

                // 3. Sort the daily tracking records chronologically (oldest date to newest date)
                var sortedDailyOrders = dailyOrderCounts.OrderBy(d => d.Key).ToList();

                // =========================================================================
                // CHART CONFIGURATION & DRAWING
                // =========================================================================

                // Clear any previous lines, bars, or legends
                chart1.Series.Clear();
                chart1.Titles.Clear();
                chart1.ChartAreas.Clear();

                // Add a clean, responsive Chart Area layout
                ChartArea chartArea = new ChartArea("MainArea");
                chartArea.AxisX.Title = "Timeline (Days)";
                chartArea.AxisY.Title = "Total Orders Logged";

                // Clean Gridlines to look modern and professional
                chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(230, 230, 230);
                chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(230, 230, 230);

                // Rotate X-axis text labels slightly if they crowd each other out
                chartArea.AxisX.LabelStyle.Angle = -45;
                chartArea.AxisX.Interval = 1; // Show every single date row entry

                chart1.ChartAreas.Add(chartArea);

                // Add Chart Header Title Title
                chart1.Titles.Add("Daily Order Performance Growth Trend");
                chart1.Titles[0].Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);

                // 4. Create a specialized Line Chart Series object
                Series lineSeries = new Series();
                lineSeries.Name = "Orders Growth";
                lineSeries.ChartArea = "MainArea";
                lineSeries.ChartType = SeriesChartType.Line; // <-- Sets the visual structure to a line

                // Styling the line layout to look clean (Deep Navy accent matching your UI theme)
                lineSeries.Color = System.Drawing.Color.FromArgb(26, 54, 93);
                lineSeries.BorderWidth = 3; // Thicker, visible line path

                // Add circular bullet node markers on each individual data checkpoint coordinate
                lineSeries.MarkerStyle = MarkerStyle.Circle;
                lineSeries.MarkerSize = 8;
                lineSeries.MarkerColor = System.Drawing.Color.FromArgb(94, 53, 177); // Soft purple highlight

                // 5. Populate Data Points systematically into the plot
                foreach (var dayRecord in sortedDailyOrders)
                {
                    // X-Value: The Date String ("2026-06-06"), Y-Value: The integer volume counter
                    lineSeries.Points.AddXY(dayRecord.Key, dayRecord.Value);
                }

                // Add the populated series string data directly into the chart container control
                chart1.Series.Add(lineSeries);

                // Optional: Show value numeral overlays right above each node checkpoint
                lineSeries.IsValueShownAsLabel = true;
                lineSeries.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);

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
                // 1. Initialize Firebase connection safely inside ProgressForm
                string path = AppDomain.CurrentDomain.BaseDirectory + @"firebase-key.json";
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb db = FirestoreDb.Create("delivery-7ccaa");

                // 2. Fetch live data
                QuerySnapshot snapshot = await db.Collection("orders").GetSnapshotAsync();

                // Dictionaries to track our chart data
                Dictionary<string, int> dailyOrderCounts = new Dictionary<string, int>();
                Dictionary<string, int> statusCounts = new Dictionary<string, int>();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    Dictionary<string, object> data = doc.ToDictionary();

                    // --- DATA FOR LINE CHART (chart1) ---
                    if (data.ContainsKey("createdAt") && data["createdAt"] != null)
                    {
                        Timestamp ts = (Timestamp)data["createdAt"];
                        string dateKey = ts.ToDateTime().ToString("yyyy-MM-dd");

                        if (dailyOrderCounts.ContainsKey(dateKey)) dailyOrderCounts[dateKey]++;
                        else dailyOrderCounts[dateKey] = 1;
                    }

                    // --- DATA FOR PIE CHART (chart2) ---
                    if (data.ContainsKey("status") && data["status"] != null)
                    {
                        string status = data["status"].ToString();
                        if (string.IsNullOrEmpty(status)) status = "Pending"; // Default fallback

                        if (statusCounts.ContainsKey(status)) statusCounts[status]++;
                        else statusCounts[status] = 1;
                    }
                }

                // =========================================================================
                // DRAW CHART 1: LINE CHART (Day-by-Day Increments)
                // =========================================================================
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


                // =========================================================================
                // DRAW CHART 2: PIE CHART (Order Status Metrics)
                // =========================================================================
                chart2.Series.Clear();
                chart2.ChartAreas.Clear();
                chart2.Titles.Clear();

                // Add a dedicated area for the Pie Chart
                ChartArea area2 = new ChartArea("PieArea");
                chart2.ChartAreas.Add(area2);

                // Add Title
                chart2.Titles.Add("Order Status Fulfillment Breakdown");
                chart2.Titles[0].Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);

                // Configure the Pie Series
                Series pieSeries = new Series("StatusDistribution");
                pieSeries.ChartArea = "PieArea";
                pieSeries.ChartType = SeriesChartType.Pie; // <-- Sets type to Pie Chart

                pieSeries["PieDrawingStyle"] = "Default";

                // Populate with our status keys and values
                foreach (var statusPair in statusCounts)
                {
                    // X-value: Status name (e.g. "Delivered"), Y-value: Total Count
                    pieSeries.Points.AddXY(statusPair.Key, statusPair.Value);
                }

                chart2.Series.Add(pieSeries);

                // Make it look professional and modern
                pieSeries.IsValueShownAsLabel = true; // Show numbers/percentages inside the slices
                pieSeries["PieLabelStyle"] = "Inside"; // Force numbers inside
                pieSeries["PieDrawingStyle"] = "Default"; // Makes it an elegant doughnut style chart instead of a solid pie!

                // Enable the Legend box on the side so team members know what colors mean
                chart2.Legends.Clear();
                chart2.Legends.Add(new Legend("DefaultLegend") { Font = new System.Drawing.Font("Segoe UI", 9) });
                pieSeries.Legend = "DefaultLegend";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed plotting metrics: {ex.Message}", "UI Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

        private async void ProgressForm_Load(object sender, EventArgs e)
        {
            //await LoadCharts();
            await LoadPieChart();

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

        private void btnLoad_Paint(object sender, PaintEventArgs e)
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

