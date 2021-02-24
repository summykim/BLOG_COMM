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

		BackgroundWorker bgAgentWorker=null;

		public Login()
        {
            InitializeComponent();
        }

		//로그인 버튼 
        private void buttonLogin_Click(object sender, EventArgs e)
        {

			bgAgentWorker = new BackgroundWorker();


			//ReportProgress메소드를 호출하기 위해서 반드시 true로 설정, false일 경우 ReportProgress메소드를 호출하면 exception 발생
			bgAgentWorker.WorkerReportsProgress = true;

			//스레드에서 취소 지원 여부
			bgAgentWorker.WorkerSupportsCancellation = true;

			//스레드가 run시에 호출되는 핸들러 등록
			bgAgentWorker.DoWork += new DoWorkEventHandler(bw_DoWork);


			// ReportProgress메소드 호출시 호출되는 핸들러 등록
			bgAgentWorker.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);


			// 스레드 완료(종료)시 호출되는 핸들러 동록
			bgAgentWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

			bgAgentWorker.RunWorkerAsync();


		}
        [Obsolete]
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {


            // Loop 
            while (true)
            {


                //CancellationPending 속성이 true로 set되었다면
                if ((bgAgentWorker.CancellationPending == true))
                {
                    //루프를 break한다.(즉 스레드 run 핸들러를 벗어나겠죠)
                    e.Cancel = true;
                    break;
                }
                else
                {


                    Common cmmLogin = new Common();
                    var result = cmmLogin.NaverLogin(txtId.Text, txtPwd.Text, checkBoxIsShow.Checked);

                    bgAgentWorker.ReportProgress(0, result);



                    System.Threading.Thread.Sleep(500);

                    Application.DoEvents();

                }
            }
        }

        // Agent 업무처리
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {


            if (e.ProgressPercentage == 0)
            {

                var result = (bool)e.UserState;

                bgAgentWorker.CancelAsync();//백그라운드 작업 취소

                if (result)
                {
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

            Application.DoEvents();

        }

        //스레드의 run함수가 종료될 경우 해당 핸들러가 호출됩니다.
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

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
