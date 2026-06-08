namespace WindowsFormsApp9
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelSlidingContainer = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.panelRegister = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gradientText22 = new WindowsFormsApp9.GradientText2();
            this.gradientButton1 = new GradientButton();
            this.txtUser = new BorderTextBox();
            this.txtPassword = new PasswordTextBox();
            this.panelLogin = new WindowsFormsApp9.GradientPanel();
            this.gradientText21 = new WindowsFormsApp9.GradientText2();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.panelSlidingContainer.SuspendLayout();
            this.panelRegister.SuspendLayout();
            this.panelLogin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSlidingContainer
            // 
            this.panelSlidingContainer.Controls.Add(this.button2);
            this.panelSlidingContainer.Location = new System.Drawing.Point(544, 0);
            this.panelSlidingContainer.Name = "panelSlidingContainer";
            this.panelSlidingContainer.Size = new System.Drawing.Size(512, 593);
            this.panelSlidingContainer.TabIndex = 0;
            this.panelSlidingContainer.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSlidingContainer_Paint);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(461, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(30, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "X";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // panelRegister
            // 
            this.panelRegister.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(246)))), ((int)(((byte)(247)))));
            this.panelRegister.Controls.Add(this.gradientText22);
            this.panelRegister.Controls.Add(this.gradientButton1);
            this.panelRegister.Controls.Add(this.txtUser);
            this.panelRegister.Controls.Add(this.txtPassword);
            this.panelRegister.Controls.Add(this.label4);
            this.panelRegister.Controls.Add(this.label3);
            this.panelRegister.Controls.Add(this.label2);
            this.panelRegister.Location = new System.Drawing.Point(529, 0);
            this.panelRegister.Name = "panelRegister";
            this.panelRegister.Size = new System.Drawing.Size(552, 593);
            this.panelRegister.TabIndex = 2;
            this.panelRegister.Paint += new System.Windows.Forms.PaintEventHandler(this.panelRegister_Paint);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(119, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(297, 20);
            this.label4.TabIndex = 4;
            this.label4.Text = "Sign in to manage your system in real-time ";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Image = global::WindowsFormsApp9.Properties.Resources.Padlock;
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Location = new System.Drawing.Point(134, 354);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Password      ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Image = global::WindowsFormsApp9.Properties.Resources.User_Male;
            this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label2.Location = new System.Drawing.Point(132, 272);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "User Name     ";
            // 
            // gradientText22
            // 
            this.gradientText22.AutoSize = true;
            this.gradientText22.Font = new System.Drawing.Font("Arial Rounded MT Bold", 37.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gradientText22.Location = new System.Drawing.Point(181, 56);
            this.gradientText22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.gradientText22.Name = "gradientText22";
            this.gradientText22.Size = new System.Drawing.Size(164, 59);
            this.gradientText22.TabIndex = 20;
            this.gradientText22.Text = "Login";
            // 
            // gradientButton1
            // 
            this.gradientButton1.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(64)))), ((int)(((byte)(17)))));
            this.gradientButton1.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(207)))), ((int)(((byte)(14)))));
            this.gradientButton1.FlatAppearance.BorderSize = 0;
            this.gradientButton1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.gradientButton1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gradientButton1.ForeColor = System.Drawing.Color.White;
            this.gradientButton1.Location = new System.Drawing.Point(135, 485);
            this.gradientButton1.Name = "gradientButton1";
            this.gradientButton1.Size = new System.Drawing.Size(261, 34);
            this.gradientButton1.TabIndex = 10;
            this.gradientButton1.Text = "Login";
            this.gradientButton1.UseVisualStyleBackColor = true;
            this.gradientButton1.Click += new System.EventHandler(this.gradientButton1_Click);
            // 
            // txtUser
            // 
            this.txtUser.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.txtUser.Location = new System.Drawing.Point(135, 298);
            this.txtUser.Name = "txtUser";
            this.txtUser.Padding = new System.Windows.Forms.Padding(2);
            this.txtUser.Size = new System.Drawing.Size(261, 31);
            this.txtUser.TabIndex = 18;
            // 
            // txtPassword
            // 
            this.txtPassword.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.txtPassword.FocusBorderColor = System.Drawing.Color.DeepSkyBlue;
            this.txtPassword.Location = new System.Drawing.Point(134, 378);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Password = "";
            this.txtPassword.Size = new System.Drawing.Size(261, 33);
            this.txtPassword.TabIndex = 17;
            // 
            // panelLogin
            // 
            this.panelLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(246)))), ((int)(((byte)(247)))));
            this.panelLogin.ColorBottom = System.Drawing.Color.Empty;
            this.panelLogin.ColorTop = System.Drawing.Color.Empty;
            this.panelLogin.Controls.Add(this.gradientText21);
            this.panelLogin.Controls.Add(this.pictureBox3);
            this.panelLogin.Location = new System.Drawing.Point(0, 0);
            this.panelLogin.Margin = new System.Windows.Forms.Padding(2);
            this.panelLogin.Name = "panelLogin";
            this.panelLogin.Size = new System.Drawing.Size(524, 593);
            this.panelLogin.TabIndex = 12;
            this.panelLogin.Paint += new System.Windows.Forms.PaintEventHandler(this.gradientPanel1_Paint);
            // 
            // gradientText21
            // 
            this.gradientText21.AutoSize = true;
            this.gradientText21.Font = new System.Drawing.Font("Arial Rounded MT Bold", 28.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gradientText21.Location = new System.Drawing.Point(121, 401);
            this.gradientText21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.gradientText21.Name = "gradientText21";
            this.gradientText21.Size = new System.Drawing.Size(309, 44);
            this.gradientText21.TabIndex = 14;
            this.gradientText21.Text = "Welcome Back!";
            this.gradientText21.Click += new System.EventHandler(this.gradientText21_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::WindowsFormsApp9.Properties.Resources.canva_abstract_chef_cooking_restaurant_free_logo_9Gfim1S8fHg;
            this.pictureBox3.Location = new System.Drawing.Point(128, 56);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(267, 290);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 13;
            this.pictureBox3.TabStop = false;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 590);
            this.Controls.Add(this.panelRegister);
            this.Controls.Add(this.panelLogin);
            this.Controls.Add(this.panelSlidingContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.panelSlidingContainer.ResumeLayout(false);
            this.panelRegister.ResumeLayout(false);
            this.panelRegister.PerformLayout();
            this.panelLogin.ResumeLayout(false);
            this.panelLogin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSlidingContainer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private GradientButton gradientButton1;
        private PasswordTextBox txtPassword;
        private BorderTextBox txtUser;
        private GradientPanel panelLogin;
        private System.Windows.Forms.PictureBox pictureBox3;
        private GradientText2 gradientText21;
        private System.Windows.Forms.Panel panelRegister;
        private GradientText2 gradientText22;
    }
}