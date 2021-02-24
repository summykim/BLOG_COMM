namespace BLOG_COMM
{
    partial class FriendReplyPop
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
            this.label3 = new System.Windows.Forms.Label();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNickName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtReplyInput = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkEmpathy = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 345);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 13;
            this.label3.Text = "댓글입력";
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(69, 92);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ReadOnly = true;
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtContent.Size = new System.Drawing.Size(702, 247);
            this.txtContent.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "글내용:";
            // 
            // txtNickName
            // 
            this.txtNickName.Location = new System.Drawing.Point(69, 11);
            this.txtNickName.Name = "txtNickName";
            this.txtNickName.ReadOnly = true;
            this.txtNickName.Size = new System.Drawing.Size(135, 21);
            this.txtNickName.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = " 사용자:";
            // 
            // txtReplyInput
            // 
            this.txtReplyInput.Location = new System.Drawing.Point(69, 345);
            this.txtReplyInput.Multiline = true;
            this.txtReplyInput.Name = "txtReplyInput";
            this.txtReplyInput.Size = new System.Drawing.Size(702, 201);
            this.txtReplyInput.TabIndex = 8;
            this.txtReplyInput.TextChanged += new System.EventHandler(this.txtReplyInput_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(404, 552);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(81, 24);
            this.button1.TabIndex = 15;
            this.button1.Text = "취소";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(297, 552);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(81, 24);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "입력완료";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(69, 45);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.ReadOnly = true;
            this.txtTitle.Size = new System.Drawing.Size(702, 21);
            this.txtTitle.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 12);
            this.label4.TabIndex = 16;
            this.label4.Text = "제목:";
            // 
            // chkEmpathy
            // 
            this.chkEmpathy.AutoSize = true;
            this.chkEmpathy.Location = new System.Drawing.Point(113, 558);
            this.chkEmpathy.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkEmpathy.Name = "chkEmpathy";
            this.chkEmpathy.Size = new System.Drawing.Size(72, 16);
            this.chkEmpathy.TabIndex = 18;
            this.chkEmpathy.Text = "공감여부";
            this.chkEmpathy.UseVisualStyleBackColor = true;
            // 
            // FriendReplyPop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 613);
            this.Controls.Add(this.chkEmpathy);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtNickName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtReplyInput);
            this.Name = "FriendReplyPop";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "블로그";
            this.Load += new System.EventHandler(this.FriendReplyPop_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNickName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtReplyInput;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkEmpathy;
    }
}