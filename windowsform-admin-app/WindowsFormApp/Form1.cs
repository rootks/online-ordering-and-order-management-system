using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using MongoDB.Bson;



namespace WindowsFormsApp9
{

    public partial class Form1 : Form
    {

        bool sidebarExpand;
        ProductForm products;
        OrderForm orders;
        UsersForm users;
        SalaryForm salary;
        ProgressForm progress;
        BillingForm billing;

        Dashbord dashbord;


        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private string selectedCustomerId = ""; //selectRowID
        private void Form1_Load(object sender, EventArgs e)
        {
            if (dashbord == null)
            {
                dashbord = new Dashbord();
                dashbord.FormClosed += Dashbord_FormClosed;
                dashbord.MdiParent = this;
                dashbord.Dock = DockStyle.Fill;
                dashbord.Show();
            }
            else
            {
                dashbord.Activate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        /* private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                var client = new MongoClient(
                    "YOUR_CONNECTION_STRING");

                var db = client.GetDatabase("DeliveryAppDB");

                MessageBox.Show("Connected Successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }   
        }   */

        private void btnLoad_Click(object sender, EventArgs e)
        {
            
        }

        

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            
           /* ProductForm form = new ProductForm();
            form.StartPosition = FormStartPosition.CenterScreen;
            this.Hide();
            form.Show();    */
            
        }

        private void sidebarTimer_Tick(object sender, EventArgs e)
        {
             
            if (sidebarExpand)
            {
                sidebar.Width -= 10;
                if (sidebar.Width == sidebar.MinimumSize.Width)
                {
                    sidebarExpand = false;
                    sidebarTimer.Stop();

                    pnMenu.Width = sidebar.Width;
                    pnBilling.Width = sidebar.Width;
                    pnLogout.Width = sidebar.Width;
                    pnOrders.Width = sidebar.Width;
                    pnProducts.Width = sidebar.Width;
                    pnSalary.Width = sidebar.Width;
                    pnUsers.Width = sidebar.Width;
                    panel6.Width = sidebar.Width;
                }
            }
            else
            {
                sidebar.Width += 15;
                if (sidebar.Width == sidebar.MaximumSize.Width)
                {
                    sidebarExpand = true;
                    sidebarTimer.Stop();

                    pnMenu.Width = sidebar.Width;
                    pnBilling.Width = sidebar.Width;
                    pnLogout.Width = sidebar.Width;
                    pnOrders.Width = sidebar.Width;
                    pnProducts.Width = sidebar.Width;
                    pnSalary.Width = sidebar.Width;
                    pnUsers.Width = sidebar.Width;
                    panel6.Width = sidebar.Width;
                }
            }
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            sidebarTimer.Start();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (products == null)
            {
                products = new ProductForm();
                products.FormClosed += Products_FormClosed;
                products.MdiParent = this;
                products.Dock= DockStyle.Fill;
                products.Show();
            }
            else
            {
                products.Activate();
            }
        }

        private void Products_FormClosed(object sender, FormClosedEventArgs e)
        {
            products = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (orders == null)
            {
                orders = new OrderForm();
                orders.FormClosed += Order_FormClosed;
                orders.MdiParent = this;
                orders.Dock = DockStyle.Fill;
                orders.Show();
            }
            else
            {
                orders.Activate();
            }
        }
        private void Order_FormClosed(object sender, FormClosedEventArgs e)
        {
            orders = null;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (users == null)
            {
                users = new UsersForm();
                users.FormClosed += Users_FormClosed;
                users.MdiParent = this;
                users.Dock = DockStyle.Fill;
                users.Show();
            }
            else
            {
                users.Activate();
            }
        }

        private void Users_FormClosed(object sender, FormClosedEventArgs e)
        {
            users = null;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (salary == null)
            {
                salary = new SalaryForm();
                salary.FormClosed += Salary_FormClosed;
                salary.MdiParent = this;
                salary.Dock = DockStyle.Fill;
                salary.Show();
            }
            else
            {
                salary.Activate();
            }
        }

        private void Salary_FormClosed(object sender, FormClosedEventArgs e)
        {
            salary = null;
        }

        private void Dashbord_FormClosed(object sender, FormClosedEventArgs e)
        {
            dashbord = null;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if (dashbord == null)
            {
                dashbord = new Dashbord();
                dashbord.FormClosed += Dashbord_FormClosed;
                dashbord.MdiParent = this;
                dashbord.Dock = DockStyle.Fill;
                dashbord.Show();
            }
            else
            {
                dashbord.Activate();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (progress == null)
            {
                progress = new ProgressForm();
                progress.FormClosed += Progress_FormClosed;
                progress.MdiParent = this;
                progress.Dock = DockStyle.Fill;
                progress.Show();
            }
            else
            {
                progress.Activate();
            }
        }

        private void Progress_FormClosed(object sender, FormClosedEventArgs e)
        {
            progress = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (billing == null)
            {
                billing = new BillingForm();
                billing.FormClosed += Billing_FormClosed;
                billing.MdiParent = this;
                billing.Dock = DockStyle.Fill;
                billing.Show();
            }
            else
            {
                billing.Activate();
            }
        }
        private void Billing_FormClosed(object sender, FormClosedEventArgs e)
        {
            billing = null;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            LoginForm login = new LoginForm();
            login.Show();
            this.Hide();
        }
    }

        /* public class MongoDBConnection
        {
            private static string connectionUri =
                "mongodb+srv://admin:yourpassword@cluster0.xxxxx.mongodb.net/?retryWrites=true&w=majority";

            private static MongoClient client =
                new MongoClient(connectionUri);

            public static IMongoDatabase database =
                client.GetDatabase("DeliveryAppDB"); 
        }  */
    
}
