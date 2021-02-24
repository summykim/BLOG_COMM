using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
namespace BLOG_COMM
{
    public partial class FormReplyPop : KryptonForm
    {
        public static string NickName { get; set; }
        public static string Content { get; set; }
        public static string ReplyContent { get; set; }


        public FormReplyPop()
        {
            InitializeComponent();
        }

        private void FormReplyPop_Load(object sender, EventArgs e)
        {
            setInfo();
        }


        private void setInfo()
        {
            txtNickName.Text = NickName;
            txtContent.Text = Content;
            txtReplyInput.Text = ReplyContent;
            txtReplyInput.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
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
