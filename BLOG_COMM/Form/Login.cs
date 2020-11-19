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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

		//로그인 버튼 
        private void buttonLogin_Click(object sender, EventArgs e)
        {

			Common cmmLogin = new Common();
			//Phantom cmmLogin = new Phantom();
			var  result=cmmLogin.NaverLogin(txtId.Text, txtPwd.Text, checkBoxIsShow.Checked);

			Application.DoEvents();
			if (result) {
				if (isSave.Checked)
				{
					Common.SetSetting("naverid", txtId.Text);
					Common.SetSetting("naverpw", txtPwd.Text);
				}
				this.Close(); 
			}
			else
				MessageBox.Show("로그인에 실패했습니다.");

		}

        private void Login_Load(object sender, EventArgs e)
        {
			if (Common.GetSetting("naverid") != null)//아이디가 있음
			{
				txtId.Text = Common.GetSetting("naverid");

			}
			if (Common.GetSetting("naverpw") != null)//암호가 있음
			{
				txtPwd.Text = Common.GetSetting("naverpw");

			}

		}
    }
}
