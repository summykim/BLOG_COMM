namespace BLOG_COMM
{
    partial class Login
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.isSave = new System.Windows.Forms.CheckBox();
            this.checkBoxIsShow = new System.Windows.Forms.CheckBox();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.isSave);
            this.panel1.Controls.Add(this.checkBoxIsShow);
            this.panel1.Controls.Add(this.buttonLogin);
            this.panel1.Controls.Add(this.txtPwd);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtId);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(21, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(305, 155);
            this.panel1.TabIndex = 1;
            // 
            // isSave
            // 
            this.isSave.AutoSize = true;
            this.isSave.Checked = true;
            this.isSave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.isSave.Location = new System.Drawing.Point(23, 105);
            this.isSave.Name = "isSave";
            this.isSave.Size = new System.Drawing.Size(48, 16);
            this.isSave.TabIndex = 6;
            this.isSave.Text = "저장";
            this.isSave.UseVisualStyleBackColor = true;
            // 
            // checkBoxIsShow
            // 
            this.checkBoxIsShow.AutoSize = true;
            this.checkBoxIsShow.Checked = true;
            this.checkBoxIsShow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIsShow.Location = new System.Drawing.Point(24, 134);
            this.checkBoxIsShow.Name = "checkBoxIsShow";
            this.checkBoxIsShow.Size = new System.Drawing.Size(96, 16);
            this.checkBoxIsShow.TabIndex = 5;
            this.checkBoxIsShow.Text = "브라우저표시";
            this.checkBoxIsShow.UseVisualStyleBackColor = true;
            this.checkBoxIsShow.Visible = false;
            // 
            // buttonLogin
            // 
            this.buttonLogin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonLogin.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.buttonLogin.Location = new System.Drawing.Point(77, 101);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(168, 20);
            this.buttonLogin.TabIndex = 4;
            this.buttonLogin.Text = "로그인";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(77, 62);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.PasswordChar = '*';
            this.txtPwd.Size = new System.Drawing.Size(168, 21);
            this.txtPwd.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "패스워드 :";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(77, 22);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(168, 21);
            this.txtId.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "아이디 :";
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 198);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Login";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "네이버로그인";
            this.Load += new System.EventHandler(this.Login_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox isSave;
        private System.Windows.Forms.CheckBox checkBoxIsShow;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label1;
    }
}