using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLOG_COMM
{
    public partial class ReplyPop : Form
    {
        public static string NickName { get; set; }
        public static string Content { get; set; }
        public static string ReplyContent { get; set; }
        public ReplyPop()
        {
            InitializeComponent();
        }

        private void ReplyPop_Load(object sender, EventArgs e)
        {
            setInfo();
        }


        private  void setInfo()
        {
            txtNickName.Text = NickName;
            txtContent.Text = Content;
            txtReplyInput.Text = ReplyContent;
            txtReplyInput.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NickName = "";
            Content = "";
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ReplyContent = txtReplyInput.Text;
            this.Close();
        }
    }
}
