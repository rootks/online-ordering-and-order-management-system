using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace WindowsFormsApp9
{
    public partial class LoginForm : Form
    {
        private Timer animationTimer;

        private double loginCurrentX;
        private double registerCurrentX;

        private double loginTargetX;
        private double registerTargetX;

        // Smooth animation
        private const double EasingFactor = 0.08;
        private const double MinimumStep = 0.1;

        private bool goingToRegister = false;
        private bool overshootPhase = false;

        public LoginForm()
        {
            InitializeComponent();

            DoubleBuffered = true;

            SetupAnimation();
        }

        private void SetupAnimation()
        {
            animationTimer = new Timer();
            animationTimer.Interval = 15;
            animationTimer.Tick += AnimationTimer_Tick;
        }


        private void LoginForm_Load(object sender, EventArgs e)
        {
            panelLogin.Left = 0;
            panelRegister.Left = panelLogin.Width;

            loginCurrentX = panelLogin.Left;
            registerCurrentX = panelRegister.Left;

            



        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            bool animating = false;

            // LOGIN PANEL
            double loginDiff = loginTargetX - loginCurrentX;

            if (Math.Abs(loginDiff) > MinimumStep)
            {
                loginCurrentX += loginDiff * EasingFactor;
                panelLogin.Left = (int)Math.Round(loginCurrentX);
                animating = true;
            }
            else
            {
                panelLogin.Left = (int)loginTargetX;
                loginCurrentX = loginTargetX;
            }

            // REGISTER PANEL
            double registerDiff = registerTargetX - registerCurrentX;

            if (Math.Abs(registerDiff) > MinimumStep)
            {
                registerCurrentX += registerDiff * EasingFactor;
                panelRegister.Left = (int)Math.Round(registerCurrentX);
                animating = true;
            }
            else
            {
                panelRegister.Left = (int)registerTargetX;
                registerCurrentX = registerTargetX;
            }

            // Spring effect
            if (!animating && overshootPhase)
            {
                overshootPhase = false;

                if (goingToRegister)
                {
                    loginTargetX = -panelLogin.Width;
                    registerTargetX = 0;
                }
                else
                {
                    loginTargetX = 0;
                    registerTargetX = panelLogin.Width;
                }

                animationTimer.Start();
                return;
            }

            if (!animating)
            {
                animationTimer.Stop();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (LinearGradientBrush brush =
                new LinearGradientBrush(
                    ClientRectangle,
                    Color.FromArgb(44, 62, 80),
                    Color.FromArgb(52, 152, 219),
                    45f))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }

            base.OnPaint(e);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panelSlidingContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void btnSwitchToRegister_Click(object sender, EventArgs e)
        {
            
        }

        private void gradientButton1_Click(object sender, EventArgs e)
        {
            if (txtUser.Text == "admin" && txtPassword.Text == "1234")
            {
                Form1 dashbord = new Form1();
                dashbord.Show();
                this.Hide();
                

            }
            else
            {
                MessageBox.Show("Invalid User name or Password!");
                

            }
        }

        private void panelRegister_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gradientButton2_Click(object sender, EventArgs e)
        {
            
        }

        private void gradienText2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void gradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gradienText1_Click(object sender, EventArgs e)
        {

        }

        private void gradientText21_Click(object sender, EventArgs e)
        {
            
            animationTimer.Stop();

            goingToRegister = true;
            overshootPhase = true;

            loginCurrentX = panelLogin.Left;
            registerCurrentX = panelRegister.Left;

            // Overshoot
            loginTargetX = -panelLogin.Width - 25;
            registerTargetX = -25;

            animationTimer.Start();
            
        }
    }
}