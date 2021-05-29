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
using Google.Cloud.Firestore;

namespace BLOG_COMM
{
    public partial class FormLogin :  KryptonForm
    {

        BackgroundWorker BgLoginWorker = null;
        public FormLogin()
        {
            InitializeComponent();
        }

        //로그인 버튼 
        private  void buttonLogin_Click(object sender, EventArgs e)
        {
            List<Users> list = null;
            Common.debugPrint("start login!!");
            try
            {

                UserRepository ur = new UserRepository();
                list = ur.GetUserWhereNaverId(txtId.Text);

            }catch(Exception ex)
            {
                Common.debugPrint(ex.Message);
                return;
            }
            Common.debugPrint("start login2");

            if (list == null || list.Count==0)
            {
                MessageBox.Show("네이버 아이디( "+ txtId.Text+ " )에 대한 프로그램 사용 요청을 해주세요!.");
                return;
            }
            else
            {
                if (list[0].use_yn == false)
                {
                    MessageBox.Show("네이버 아이디( " + txtId.Text + " )에 대한 프로그램 사용 중지 상태입니다!.");
                    return;
                }
                Console.WriteLine(list[0].user_name);
                Common.UserGroup = list[0].user_group;
            }

            LogRepository logRep = new LogRepository();
            App_logs app_Logs = new App_logs();
            app_Logs.naverId = txtId.Text;            
            app_Logs.reg_dtm= Timestamp.GetCurrentTimestamp();
            logRep.Add(app_Logs);
            buttonLogin.Text = "로그인 처리중...";
            buttonLogin.Enabled = false;
            BgLoginWorker = new BackgroundWorker();


            //ReportProgress메소드를 호출하기 위해서 반드시 true로 설정, false일 경우 ReportProgress메소드를 호출하면 exception 발생
            BgLoginWorker.WorkerReportsProgress = true;

            //스레드에서 취소 지원 여부
            BgLoginWorker.WorkerSupportsCancellation = true;

            //스레드가 run시에 호출되는 핸들러 등록
            BgLoginWorker.DoWork += new DoWorkEventHandler(bw_DoWork);


            // ReportProgress메소드 호출시 호출되는 핸들러 등록
            BgLoginWorker.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);


            // 스레드 완료(종료)시 호출되는 핸들러 동록
            BgLoginWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            BgLoginWorker.RunWorkerAsync();


        }
        [Obsolete]
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {


            // Loop 
            while (true)
            {


                //CancellationPending 속성이 true로 set되었다면
                if ((BgLoginWorker.CancellationPending == true))
                {
                    //루프를 break한다.(즉 스레드 run 핸들러를 벗어나겠죠)
                    e.Cancel = true;
                    break;
                }
                else
                {


                    Common cmmLogin = new Common();
                    var result = cmmLogin.NaverLogin(txtId.Text, txtPwd.Text, checkBoxIsShow.Checked);

                    BgLoginWorker.ReportProgress(0, result);



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

                BgLoginWorker.CancelAsync();//백그라운드 작업 취소

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
            buttonLogin.Text = "로그인";
            buttonLogin.Enabled = true ;
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

        private void kryptonLabel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void kryptonTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void FormLogin_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        // 로그인 취소
        private void kryptonButton1_Click(object sender, EventArgs e)
        {
           
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
