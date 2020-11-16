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
    public partial class FriendReplyPop : Form
    {

        public static string NickName { get; set; }
        public static string Title { get; set; }
        public static string Content { get; set; }
        public static string ReplyContent { get; set; }

        public FriendReplyPop()
        {
            InitializeComponent();
        }

        private void FriendReplyPop_Load(object sender, EventArgs e)
        {
            setInfo();
        }

        private void setInfo()
        {
            txtNickName.Text = NickName;
            txtTitle.Text = Title;
            txtContent.Text = Content;
            txtReplyInput.Text = ReplyContent;
            txtReplyInput.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NickName = "";
            Title = "";
            Content = "";
            ReplyContent = "";
            this.Close();
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            ReplyContent = txtReplyInput.Text;
            this.Close();
        }

        private void txtReplyInput_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
