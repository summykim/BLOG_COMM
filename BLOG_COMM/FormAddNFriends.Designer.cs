
namespace BLOG_COMM
{
    partial class FormAddNFriends
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
            this.kryptonGroupBox24 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.txtAddGrpNm = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.btnAddNFriendStart = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonLabel37 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.txtNFriendReqContent = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel38 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.cboNFriendMaxCnt = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel39 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.txtNFriendSearchKeyword = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnAddNFriendCancel = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.StatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox24)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox24.Panel)).BeginInit();
            this.kryptonGroupBox24.Panel.SuspendLayout();
            this.kryptonGroupBox24.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboNFriendMaxCnt)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonGroupBox24
            // 
            this.kryptonGroupBox24.CaptionOverlap = 1D;
            this.kryptonGroupBox24.Location = new System.Drawing.Point(10, 12);
            this.kryptonGroupBox24.Name = "kryptonGroupBox24";
            // 
            // kryptonGroupBox24.Panel
            // 
            this.kryptonGroupBox24.Panel.Controls.Add(this.btnAddNFriendCancel);
            this.kryptonGroupBox24.Panel.Controls.Add(this.kryptonLabel1);
            this.kryptonGroupBox24.Panel.Controls.Add(this.txtAddGrpNm);
            this.kryptonGroupBox24.Panel.Controls.Add(this.btnAddNFriendStart);
            this.kryptonGroupBox24.Panel.Controls.Add(this.kryptonLabel37);
            this.kryptonGroupBox24.Panel.Controls.Add(this.txtNFriendReqContent);
            this.kryptonGroupBox24.Panel.Controls.Add(this.kryptonLabel38);
            this.kryptonGroupBox24.Panel.Controls.Add(this.cboNFriendMaxCnt);
            this.kryptonGroupBox24.Panel.Controls.Add(this.kryptonLabel39);
            this.kryptonGroupBox24.Panel.Controls.Add(this.txtNFriendSearchKeyword);
            this.kryptonGroupBox24.Size = new System.Drawing.Size(545, 293);
            this.kryptonGroupBox24.TabIndex = 2;
            this.kryptonGroupBox24.Values.Heading = "이웃추가설정";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(55, 50);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.SparklePurple;
            this.kryptonLabel1.Size = new System.Drawing.Size(78, 20);
            this.kryptonLabel1.TabIndex = 25;
            this.kryptonLabel1.Values.Text = "추가할 그룹:";
            // 
            // txtAddGrpNm
            // 
            this.txtAddGrpNm.Location = new System.Drawing.Point(139, 51);
            this.txtAddGrpNm.Name = "txtAddGrpNm";
            this.txtAddGrpNm.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.SparklePurple;
            this.txtAddGrpNm.Size = new System.Drawing.Size(136, 23);
            this.txtAddGrpNm.TabIndex = 24;
            this.txtAddGrpNm.Text = "친구";
            // 
            // btnAddNFriendStart
            // 
            this.btnAddNFriendStart.Location = new System.Drawing.Point(14, 222);
            this.btnAddNFriendStart.Name = "btnAddNFriendStart";
            this.btnAddNFriendStart.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.SparkleBlue;
            this.btnAddNFriendStart.Size = new System.Drawing.Size(179, 30);
            this.btnAddNFriendStart.TabIndex = 23;
            this.btnAddNFriendStart.Values.Text = "이웃추가 작업 시작";
            this.btnAddNFriendStart.Click += new System.EventHandler(this.btnAddNFriendStart_Click);
            // 
            // kryptonLabel37
            // 
            this.kryptonLabel37.Location = new System.Drawing.Point(24, 76);
            this.kryptonLabel37.Name = "kryptonLabel37";
            this.kryptonLabel37.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.SparklePurple;
            this.kryptonLabel37.Size = new System.Drawing.Size(106, 20);
            this.kryptonLabel37.TabIndex = 21;
            this.kryptonLabel37.Values.Text = "이웃 신청글 내용:";
            // 
            // txtNFriendReqContent
            // 
            this.txtNFriendReqContent.Location = new System.Drawing.Point(14, 102);
            this.txtNFriendReqContent.Multiline = true;
            this.txtNFriendReqContent.Name = "txtNFriendReqContent";
            this.txtNFriendReqContent.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.SparklePurple;
            this.txtNFriendReqContent.Size = new System.Drawing.Size(514, 105);
            this.txtNFriendReqContent.TabIndex = 20;
            this.txtNFriendReqContent.Text = "안녕하세요. 서로 이웃 신청합니다.^^\r\n서로 소통하며 함께 발전했으면 좋겠습니다.";
            // 
            // kryptonLabel38
            // 
            this.kryptonLabel38.Location = new System.Drawing.Point(301, 22);
            this.kryptonLabel38.Name = "kryptonLabel38";
            this.kryptonLabel38.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.SparklePurple;
            this.kryptonLabel38.Size = new System.Drawing.Size(181, 20);
            this.kryptonLabel38.TabIndex = 19;
            this.kryptonLabel38.Values.Text = "추가할 이웃 수(1일 최대100명):";
            // 
            // cboNFriendMaxCnt
            // 
            this.cboNFriendMaxCnt.DropDownWidth = 92;
            this.cboNFriendMaxCnt.Items.AddRange(new object[] {
            "5",
            "10",
            "15",
            "20",
            "25",
            "30",
            "35",
            "40",
            "45",
            "50",
            "100"});
            this.cboNFriendMaxCnt.Location = new System.Drawing.Point(478, 22);
            this.cboNFriendMaxCnt.Name = "cboNFriendMaxCnt";
            this.cboNFriendMaxCnt.Size = new System.Drawing.Size(50, 21);
            this.cboNFriendMaxCnt.TabIndex = 18;
            this.cboNFriendMaxCnt.Text = "50";
            // 
            // kryptonLabel39
            // 
            this.kryptonLabel39.Location = new System.Drawing.Point(14, 22);
            this.kryptonLabel39.Name = "kryptonLabel39";
            this.kryptonLabel39.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.SparklePurple;
            this.kryptonLabel39.Size = new System.Drawing.Size(119, 20);
            this.kryptonLabel39.TabIndex = 16;
            this.kryptonLabel39.Values.Text = "추가할 이웃 키워드:";
            // 
            // txtNFriendSearchKeyword
            // 
            this.txtNFriendSearchKeyword.Location = new System.Drawing.Point(139, 22);
            this.txtNFriendSearchKeyword.Name = "txtNFriendSearchKeyword";
            this.txtNFriendSearchKeyword.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.SparklePurple;
            this.txtNFriendSearchKeyword.Size = new System.Drawing.Size(136, 23);
            this.txtNFriendSearchKeyword.TabIndex = 15;
            this.txtNFriendSearchKeyword.Text = "단독주택";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel,
            this.StatusLabel2,
            this.StatusLabel3,
            this.StatusLabel4});
            this.statusStrip1.Location = new System.Drawing.Point(0, 289);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(561, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // StatusLabel2
            // 
            this.StatusLabel2.Name = "StatusLabel2";
            this.StatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // StatusLabel3
            // 
            this.StatusLabel3.Name = "StatusLabel3";
            this.StatusLabel3.Size = new System.Drawing.Size(0, 17);
            // 
            // btnAddNFriendCancel
            // 
            this.btnAddNFriendCancel.Location = new System.Drawing.Point(339, 222);
            this.btnAddNFriendCancel.Name = "btnAddNFriendCancel";
            this.btnAddNFriendCancel.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.SparkleBlue;
            this.btnAddNFriendCancel.Size = new System.Drawing.Size(179, 30);
            this.btnAddNFriendCancel.TabIndex = 27;
            this.btnAddNFriendCancel.Values.Text = "이웃추가 작업 취소";
            this.btnAddNFriendCancel.Click += new System.EventHandler(this.btnAddNFriendCancel_Click);
            // 
            // StatusLabel4
            // 
            this.StatusLabel4.Name = "StatusLabel4";
            this.StatusLabel4.Size = new System.Drawing.Size(0, 17);
            // 
            // FormAddNFriends
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 311);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.kryptonGroupBox24);
            this.Name = "FormAddNFriends";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "이웃추가작업";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox24.Panel)).EndInit();
            this.kryptonGroupBox24.Panel.ResumeLayout(false);
            this.kryptonGroupBox24.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox24)).EndInit();
            this.kryptonGroupBox24.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboNFriendMaxCnt)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox24;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnAddNFriendStart;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel37;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtNFriendReqContent;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel38;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cboNFriendMaxCnt;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel39;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtNFriendSearchKeyword;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtAddGrpNm;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnAddNFriendCancel;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel4;
    }
}