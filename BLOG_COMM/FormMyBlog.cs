using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLOG_COMM.Class;
using ComponentFactory.Krypton.Toolkit;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace BLOG_COMM
{
    public partial class FormMyBlog : KryptonForm
    {

		BackgroundWorker BgWorkerMyBlog = null;//내블로그 목록조회 백그라운드워커
		BackgroundWorker BgWorkerMyBlogReply = null;//내블로그 답글댓글 목록조회 백그라운드워커
		BackgroundWorker BgWorkerFriendsCollect = null;//이웃블로그 수집용
		BackgroundWorker BgWorkerFriendsReply = null;//이웃블로그 댓글용
		BackgroundWorker BgAutoReplyWorker = null;//블로그홈 자동 댓들용 
		public FormMyBlog()
        {

			InitializeComponent();



		}

        private void kryptonGroup1_Panel_Paint(object sender, PaintEventArgs e)
        {

        }

		// 로그인 폼 호출
		private bool newLogin()
		{

            if (Common._driver!=null && Common._driver.WindowHandles.Count>0)
            {
				return true;

            }


			FormLogin frmLogin = new FormLogin();


			frmLogin.ShowDialog();//로그인 폼 호출 

			Application.DoEvents();

			if (Common.currUser != null)//로그인 성공
			{

				return true;

            }
            else
            {
				return false;
			}
			
		}
		/************************************************************************************************************************************
		 * 내블로그 검색 start
		*************************************************************************************************************************************/

		private void btnBlogList_Click(object sender, EventArgs e)
        {

			BgWorkerMyBlog = new BackgroundWorker();


			//ReportProgress메소드를 호출하기 위해서 반드시 true로 설정, false일 경우 ReportProgress메소드를 호출하면 exception 발생
			BgWorkerMyBlog.WorkerReportsProgress = true;

			//스레드에서 취소 지원 여부
			BgWorkerMyBlog.WorkerSupportsCancellation = true;

			//스레드가 run시에 호출되는 핸들러 등록
			BgWorkerMyBlog.DoWork += new DoWorkEventHandler(bw_DoWork);


			// ReportProgress메소드 호출시 호출되는 핸들러 등록
			BgWorkerMyBlog.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);


			// 스레드 완료(종료)시 호출되는 핸들러 동록
			BgWorkerMyBlog.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

			BgWorkerMyBlog.RunWorkerAsync();

			dgvBlog.Rows.Clear();
			btnBlogList.Enabled = false;
			btnBlogList.Text = "진행중";
		}
		// 내 블로그 목록 가져오기  백그라운드 작업 
		[Obsolete]
		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{

			// Loop 
			while (true)
			{


				//CancellationPending 속성이 true로 set되었다면
				if ((BgWorkerMyBlog.CancellationPending == true))
				{
					//루프를 break한다.(즉 스레드 run 핸들러를 벗어나겠죠)
					e.Cancel = true;
					break;
				}
                else
                {
					WebDriverWait wait = new WebDriverWait(Common._driver, TimeSpan.FromSeconds(10));
					Common._driver.Navigate().GoToUrl(Common.currUser.BlogUrl); // 블로그 리스트에 접속합니다.
					Application.DoEvents();
					Thread.Sleep(500);
					Common._driver.SwitchTo().Frame("mainFrame");

					wait.Until(driver => Common._driver.FindElement(By.XPath("//*[@id='categoryTitle']")).Displayed);

					//펼쳐졌는제 체크
					IWebElement Result = Common.FindElement(By.XPath("//*[@id='listCountToggle']"));
					if (Result != null)//펼쳐져있으면 30줄선택 
					{
						Result.Click();
						Application.DoEvents();
						Result = Common.FindElement(By.XPath("//*[@id='changeListCount']/a[5]"));
						if (Result != null) Result.Click();
					}
					else//접혀있음.
					{
						//전체보기 타이틀 클릭
						Result = Common.FindElement(By.XPath("//*[@id='categoryTitle']"));
						if (Result != null) Result.Click();
						Application.DoEvents();
						//펼쳐져있으면 30줄선택
						Result = Common.FindElement(By.XPath("//*[@id='listCountToggle']"));
						if (Result != null) Result.Click();
						Result = Common.FindElement(By.XPath("//*[@id='changeListCount']/a[5]"));
						if (Result != null) Result.Click();
					}


					IWebElement cntel = Common._driver.FindElementByXPath("//*[@id='category-name']/div/table[2]/tbody/tr/td[2]/div/h4");
					int allcnt = int.Parse(cntel.Text.Replace("전체보기", "").Replace("개의 글", "").Trim());
					var list = Common._driver.FindElementsByXPath("//*[@id='listTopForm']/table/tbody/tr");
					
					int index = 0, cnt = 1;
					foreach (var el in list)
					{
                        try
                        {
							if ((++index % 2) == 0)//줄태그는 pass
							{
								continue;
							}
							var eltext = Common.FindElement(By.XPath("//*[@id='listTopForm']/table/tbody/tr[" + index + "]/td[1]/div/span/a"));
							String title = "";
							if (eltext != null && !eltext.Equals(""))
								title = eltext.Text;
							else
							{
								continue;
							}



							String url = eltext.GetAttribute("href");
							String blogids = url.Split('&')[1];
							String blogid = blogids.Split('=')[1];
							String postUrl = Common.currUser.BlogUrl + "/" + blogid;

							eltext = Common.FindElement(By.XPath("//*[@id='listTopForm']/table/tbody/tr[" + index + "]/td[3]/div/span"));
							String writedt = eltext.Text;

							string[] row = new string[] { cnt.ToString(), title, postUrl, writedt };
							BgWorkerMyBlog.ReportProgress(cnt, row);


						}
						catch(Exception ex)
                        {
							Common.log.Debug(ex.Message);
                        }


						++cnt;

						System.Threading.Thread.Sleep(500);

						Application.DoEvents();
						if (cnt > 30 || cnt > allcnt)
						{
							break;
						}
					}//foreach end 



					break;

				}
			}
		}

		// 내 블로그 목록 가져오기  백그라운드 작업 결과  화면 업데이트
		private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{


			btnBlogList.Text = "진행중("+e.ProgressPercentage+")";

			string[] row =(string[])e.UserState;

				dgvBlog.Rows.Add(row);

			    Application.DoEvents();

		}

		// 내 블로그 목록 가져오기  백그라운드 작업 결과  종료
		private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			btnBlogList.Text = "조회";
			btnBlogList.Enabled = true;

			BgWorkerMyBlog.Dispose();
		}


		/************************************************************************************************************************************
		 * 내블로그 검색 end
		*************************************************************************************************************************************/



		/************************************************************************************************************************************
		 * 내블로그에 답글/댓글 대상 조회 START
		*************************************************************************************************************************************/

		private void dgvBlog_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			var index = dgvBlog.Rows[e.RowIndex].Cells[0].Value;
			if (index != null)
			{

				PostUrl.Text = (String)dgvBlog.Rows[e.RowIndex].Cells[2].Value;
				txtPostTitle.Text = (String)dgvBlog.Rows[e.RowIndex].Cells[1].Value;

				BgWorkerMyBlogReply = new BackgroundWorker();


				//ReportProgress메소드를 호출하기 위해서 반드시 true로 설정, false일 경우 ReportProgress메소드를 호출하면 exception 발생
				BgWorkerMyBlogReply.WorkerReportsProgress = true;

				//스레드에서 취소 지원 여부
				BgWorkerMyBlogReply.WorkerSupportsCancellation = true;

				//스레드가 run시에 호출되는 핸들러 등록
				BgWorkerMyBlogReply.DoWork += new DoWorkEventHandler(bw_reply_DoWork);


				// ReportProgress메소드 호출시 호출되는 핸들러 등록
				BgWorkerMyBlogReply.ProgressChanged += new ProgressChangedEventHandler(bw_reply_ProgressChanged);


				// 스레드 완료(종료)시 호출되는 핸들러 동록
				BgWorkerMyBlogReply.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_reply_RunWorkerCompleted);


				BgWorkerMyBlogReply.RunWorkerAsync(PostUrl.Text);

				dataGridView1.Rows.Clear();
				dataGridView2.Rows.Clear();
				dgvBlog.Enabled = false;
				dgvBlog.UseWaitCursor = true;
			}
		}



		// 내 블로그 목록 가져오기  백그라운드 작업 
		[Obsolete]
		private void bw_reply_DoWork(object sender, DoWorkEventArgs e)
		{


			// Loop 
			while (true)
			{


				//CancellationPending 속성이 true로 set되었다면
				if ((BgWorkerMyBlogReply.CancellationPending == true))
				{
					//루프를 break한다.(즉 스레드 run 핸들러를 벗어나겠죠)
					e.Cancel = true;
					break;
				}
				else
				{
					String PostUrl = (string)e.Argument;
					String[] url = PostUrl.Split('/');
					String blog_comment_id = url[url.Length - 1];
					WebDriverWait wait = new WebDriverWait(Common._driver, TimeSpan.FromSeconds(10));
					Common._driver.Navigate().GoToUrl(PostUrl); // 웹 사이트.에 접속합니다.
					Common._driver.SwitchTo().Frame("mainFrame");

					IWebElement Result = Common.FindElement(By.XPath("//*[@id='Comi" + blog_comment_id + "']"));
					if (Result != null) Result.Click();


					var list = Common._driver.FindElementsByXPath("//*[@id='naverComment_201_" + blog_comment_id + "_wai_u_cbox_content_wrap_tabpanel']/ul/li");

					var index = 1;
					foreach (var el in list)
					{
                        try { 
							var elnick = el.FindElement(By.ClassName("u_cbox_nick"));
							String nickname = elnick.Text;

							elnick = el.FindElement(By.ClassName("u_cbox_thumb_wrap"));
							String blogurl = elnick.GetAttribute("href");


							elnick = el.FindElement(By.ClassName("u_cbox_date"));
							String update = elnick.Text;


							elnick = el.FindElement(By.ClassName("u_cbox_contents"));
							String content = elnick.Text;


							string[] row = new string[] { blogurl, nickname, update, content };
							string[] row2 = new string[] { blogurl, nickname };


							List<string[]> ProgressObject = new List<string[]>();
							ProgressObject.Add(row);
							ProgressObject.Add(row2);
							BgWorkerMyBlogReply.ReportProgress(index, ProgressObject);
							index++;
                        }
                        catch (Exception ex)
                        {
							Common.log.Error(ex.Message);
                        }

					}

					break;
				}
			}
		}

		// 내 블로그 목록 가져오기  백그라운드 작업 결과  화면 업데이트
		private void bw_reply_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{



			List<string[]> ProgressObject = (List<string[]>)e.UserState;

			dataGridView1.Rows.Add(ProgressObject[0]);
			dataGridView2.Rows.Add(ProgressObject[1]);

			Application.DoEvents();

		}

		// 내 블로그 목록 가져오기  백그라운드 작업 결과  종료
		private void bw_reply_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

			if (dataGridView1.Rows.Count>0)
			{
                if (dataGridView1.Columns.Count <= 5)
                {
					DataGridViewButtonColumn btn = new DataGridViewButtonColumn();

					dataGridView1.Columns.Add(btn);
					btn.HeaderText = "답글달아주기";
					btn.Text = "답글달기";
					btn.Name = "btn";
					btn.UseColumnTextForButtonValue = true;
				}

			}
			if (dataGridView2.Rows.Count > 0)
			{
				if (dataGridView2.Columns.Count < 4)
				{


					DataGridViewButtonColumn btn2 = new DataGridViewButtonColumn();
					dataGridView2.Columns.Add(btn2);
					btn2.HeaderText = "댓글달기주기";
					btn2.Text = "댓글달기";
					btn2.Name = "btn2";
					btn2.UseColumnTextForButtonValue = true;

					DataGridViewButtonColumn btn3 = new DataGridViewButtonColumn();
					dataGridView2.Columns.Add(btn3);
					btn3.HeaderText = "이웃등록";
					btn3.Text = "친한이웃등록";
					btn3.Name = "btn3";
					btn3.UseColumnTextForButtonValue = true;
				}
			}
			
			dgvBlog.Enabled = true;
			dgvBlog.UseWaitCursor = false;

			BgWorkerMyBlogReply.Dispose();
		}


		//답글 달기 버튼 클릭
		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
			int colIndex = e.ColumnIndex;
			if (colIndex == 5)//내블로그 댓글애  답글 달기 
			{

				FormReplyPop.NickName = (string)dataGridView1.Rows[e.RowIndex].Cells[1].Value;//이름
				FormReplyPop.Content = (string)dataGridView1.Rows[e.RowIndex].Cells[3].Value;//내용

				FormReplyPop.ReplyContent = (string)dataGridView1.Rows[e.RowIndex].Cells[4].Value;


				FormReplyPop rp = new FormReplyPop();
				rp.ShowDialog();
				dataGridView1.Rows[e.RowIndex].Cells[4].Value = FormReplyPop.ReplyContent;


			}
		}



		// 내 블로그 댓글 그리드 셀 클릭 이벤트
		private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
		{

			int colIndex = e.ColumnIndex;

			if (colIndex == 3)// 이웃블로그 댓글달기
			{

				string nblogUrl = (string)dataGridView2.Rows[e.RowIndex].Cells[0].Value;//url
				string nickname = (string)dataGridView2.Rows[e.RowIndex].Cells[1].Value;//이름

				FormFriendReplyPop rp = new FormFriendReplyPop();
				FormFriendReplyPop.NickName = nickname;
				FormFriendReplyPop.BlogUrl = nblogUrl;
				rp.ShowDialog();
				string result = FormFriendReplyPop.result;

				if (result != null &&  result.Length > 0)//입력성공했으면 표시
					dataGridView2.Rows[e.RowIndex].Cells[2].Value = result;

			}
			else if (colIndex == 4)// 친한이웃등록
			{

				string nblogUrl = (string)dataGridView2.Rows[e.RowIndex].Cells[0].Value;//url
				string naverid = nblogUrl.Split('/')[3];
				string nickname = (string)dataGridView2.Rows[e.RowIndex].Cells[1].Value;//이름

				DataTable dt = DbUtil.searchMyFriends(naverid);
				if (dt.Rows.Count > 0)
				{
					MessageBox.Show(nickname + " 님은 이미 친한이웃입니다.");
					return;
				}

				string msg = nickname + " 님을 ";
				var result = MessageBox.Show(msg + "친한이웃으로 등록 하시겠습니까?", "친한이웃등록", MessageBoxButtons.OKCancel);
				if (result == DialogResult.Cancel)//취소
				{
					return;
				}

				DbUtil.InsertMyFriend(naverid, nickname, "Y");

				// 친한이웃 등록 시작 
				MessageBox.Show(msg + "친한이웃으로 등록했습니다.");


			}
		}
		//일괄 답글 달기 버튼 클릭
		private void BatchAllReply_Click(object sender, EventArgs e)
		{
			if (dataGridView1.Rows.Count == 0)
			{
				MessageBox.Show("데이터가 없네요.");
				return;
			}

			DialogResult result = MessageBox.Show("일괄답글을 실행할까요?", "답글일괄실행", MessageBoxButtons.OKCancel);

			if (result == DialogResult.OK)//실행
			{
				String[] url = PostUrl.Text.Split('/');
				String blog_comment_id = url[url.Length - 1];

				BgWorkerMyBlogReply = new BackgroundWorker();


				//ReportProgress메소드를 호출하기 위해서 반드시 true로 설정, false일 경우 ReportProgress메소드를 호출하면 exception 발생
				BgWorkerMyBlogReply.WorkerReportsProgress = true;

				//스레드에서 취소 지원 여부
				BgWorkerMyBlogReply.WorkerSupportsCancellation = true;

				//스레드가 run시에 호출되는 핸들러 등록
				BgWorkerMyBlogReply.DoWork += new DoWorkEventHandler(bw_reply2_DoWork);


				// ReportProgress메소드 호출시 호출되는 핸들러 등록
				BgWorkerMyBlogReply.ProgressChanged += new ProgressChangedEventHandler(bw_reply2_ProgressChanged);


				// 스레드 완료(종료)시 호출되는 핸들러 동록
				BgWorkerMyBlogReply.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_reply2_RunWorkerCompleted);

				Hashtable ht = new Hashtable();

				ht.Add("blog_comment_id", blog_comment_id);

				Hashtable gridht = new Hashtable();
				for (int i = 0; i < dataGridView1.Rows.Count; i++)
				{
					String replytext = (string)dataGridView1.Rows[i].Cells[4].Value;//답글내용


					gridht.Add(i, replytext);
					
				}
				ht.Add("grids", gridht);

                BgWorkerMyBlogReply.RunWorkerAsync(ht);

				BatchAllReply.Enabled = false;
				dataGridView1.Enabled = false;

			}

		}


		// 내 블로그 목록 가져오기  백그라운드 작업 
		[Obsolete]
		private void bw_reply2_DoWork(object sender, DoWorkEventArgs e)
		{


			// Loop 
			while (true)
			{


				//CancellationPending 속성이 true로 set되었다면
				if ((BgWorkerMyBlogReply.CancellationPending == true))
				{
					//루프를 break한다.(즉 스레드 run 핸들러를 벗어나겠죠)
					e.Cancel = true;
					break;
				}
				else
				{
					Hashtable ht =(Hashtable) e.Argument;
					String blog_comment_id = ht["blog_comment_id"] as String;
					Hashtable gridht = ht["grids"] as Hashtable;
					for (int i = 0; i < gridht.Count; i++)
					{
                        try
                        {
							String replytext = gridht[i] as String;//답글내용
							BwWorkJob.writeReply(blog_comment_id, replytext, i);
						}
                        catch (Exception ex)
                        {
							Common.log.Debug(ex.Message);
                        }			

					}
					
					break;
				}
			}
		}

		// 내 블로그 목록 가져오기  백그라운드 작업 결과  화면 업데이트
		private void bw_reply2_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			BatchAllReply.Text = "진행중(" + e.ProgressPercentage+")";
			Application.DoEvents();

		}
		private void bw_reply2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

			BatchAllReply.Enabled = true;
			BatchAllReply.Text = "답글일괄수행";
			dataGridView1.Enabled = true;
			BgWorkerMyBlogReply.Dispose();
		}
		


		/************************************************************************************************************************************
		 * 내블로그에 답글/댓글 대상 조회 END
		*************************************************************************************************************************************/

		/************************************************************************************************************************************
		 * 친한친구 관리 Start 
		*************************************************************************************************************************************/
		//친한친구 검색
		private void btnSearchMyFRIEND_Click(object sender, EventArgs e)
		{
			DbUtil.searchMyFriends(dgMyFriendsList, txtSearchFId.Text, txtSearchFname.Text, "");
			MyFriendGridSettings();

		}
		// 친한친구 댓글달기
		private void btnReplyMyFriend_Click(object sender, EventArgs e)
		{
			// 이웃블로그 댓글달기

			string nblogUrl = Common.naver_blog_url + "/" + txtMyFId.Text;//url
			string nickname = txtMyFName.Text;//이름


			FormFriendReplyPop rp = new FormFriendReplyPop();
			FormFriendReplyPop.NickName = nickname;
			FormFriendReplyPop.BlogUrl = nblogUrl;
			rp.ShowDialog();
			string result = FormFriendReplyPop.result;

			if (result != null && result.Length > 0)//입력성공했으면 표시
				MessageBox.Show("정상처리되었어요.^^");
		}

		//친한친구삭제
		private void btnDelMyFriend_Click(object sender, EventArgs e)
		{
			string msg = txtMyFName.Text + " 님을 ";
			var result = MessageBox.Show(msg + " 삭제 하시겠습니까?", "친한이웃삭제", MessageBoxButtons.OKCancel);
			if (result == DialogResult.Cancel)//취소
			{
				return;
			}
			int cnt = DbUtil.DeleteMyFriends(txtMyFId.Text);
			string message = "정상 삭제되었습니다.";
			if (cnt == 0) message = "삭제에 살패했습니다.";
			else DbUtil.searchMyFriends(dgMyFriendsList, txtSearchFId.Text, txtSearchFname.Text, "");

			txtMyFName.Text = ""; txtMyFId.Text = "";

			MessageBox.Show(message);


		}
		//친한친구 상세
		private void dgMyFriendsList_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 ) return;
			if (DBNull.Value.Equals(dgMyFriendsList.Rows[e.RowIndex].Cells[0].Value)) return;
			txtMyFId.Text = (string)dgMyFriendsList.Rows[e.RowIndex].Cells[0].Value;
			txtMyFName.Text = (string)dgMyFriendsList.Rows[e.RowIndex].Cells[1].Value;

			if (txtMyFId.Text.Length > 0)
			{
				btnDelMyFriend.Visible = true;
				btnReplyMyFriend.Visible = true;
			}
			else
			{
				btnDelMyFriend.Visible = false;
				btnReplyMyFriend.Visible = false;
			}
		}
		private void MyFriendGridSettings()
		{


			int index = 0;//id
			dgMyFriendsList.Columns[index].ReadOnly = true; ;
			dgMyFriendsList.Columns[index].Width = 200;

			index = 1;//nickname
			dgMyFriendsList.Columns[index].ReadOnly = true; ;
			dgMyFriendsList.Columns[index].Width = 200;


			index = 2;//등록일

			dgMyFriendsList.Columns[index].ReadOnly = true; ;
			dgMyFriendsList.Columns[index].Width = 200;


		}
		/************************************************************************************************************************************
		 * 친한친구 관리 END  
		*************************************************************************************************************************************/


		/************************************************************************************************************************************
		 * 친한이웃 소통하기 start 
		*************************************************************************************************************************************/

		private void btnFriendsStop_Click(object sender, EventArgs e)
		{
			btnFriendsStop.Visible = false;
			DialogResult result= MessageBox.Show("수집작업을 중단하시겠습니까?","이웃블로그 수집중단.", MessageBoxButtons.YesNo);
            if (result.Equals(DialogResult.Yes)) { 
				BgWorkerFriendsCollect.CancelAsync();
			}
		}
		//친한이웃 수집 시작
		private void btnFriendsInit_Click(object sender, EventArgs e)
		{

			try
			{
				if (txtStartPage.Text.Length == 0 || !IsNumeric(txtStartPage.Text))
				{
					MessageBox.Show("시작 페이지 번호를 입력하세요.");
					return;
				}
				else if (txtEndPage.Text.Length == 0 || !IsNumeric(txtEndPage.Text))
				{
					MessageBox.Show("마지막 페이지 번호를 입력하세요.");
					return;
				}

				if (int.Parse(txtEndPage.Text) < int.Parse(txtStartPage.Text))
				{
					MessageBox.Show("시작 페이지 번호가  마지막 페이지 번호 보다 클수는 없습니다.");
					return;
				}

				string start_msg = txtStartPage.Text + " 페이지 부터 " + txtEndPage.Text + " 페이지 까지 \n";

				String DelStr = "";
				if (chkDataAllDel.Checked)
				{
					DelStr = "전체 데이터를 삭제하고 시작하고 \n";
				}
				var result = MessageBox.Show(DelStr + start_msg + "네이버 이웃블로그 사용자 수집을 시작합니다.", "네이버 이웃블로그수집", MessageBoxButtons.OKCancel);
				if (result == DialogResult.Cancel)
				{
					return;
				}
				btnFriendsInit.Visible = false;
				btnFriendsStop.Visible = true;

				//전체 데이터 삭제
				if (chkDataAllDel.Checked) DbUtil.allDeleteFriends();

				DbUtil.searchFriends(dgFriendsList);
				FriendGridSettings();

				FriendsCollectWorkerStart();
			}
			catch (Exception ex)
			{
				Common.log.Error(ex.Message.ToString());
			}

		}


		//백그라운드 워커 실행 
		private void FriendsCollectWorkerStart()
		{


			BgWorkerFriendsCollect = new BackgroundWorker();


			//ReportProgress메소드를 호출하기 위해서 반드시 true로 설정, false일 경우 ReportProgress메소드를 호출하면 exception 발생
			BgWorkerFriendsCollect.WorkerReportsProgress = true;
			//스레드에서 취소 지원 여부
			BgWorkerFriendsCollect.WorkerSupportsCancellation = true;
			//스레드가 run시에 호출되는 핸들러 등록
			BgWorkerFriendsCollect.DoWork += new DoWorkEventHandler(bw_FriendsCollect_DoWork);
			// ReportProgress메소드 호출시 호출되는 핸들러 등록
			BgWorkerFriendsCollect.ProgressChanged += new ProgressChangedEventHandler(bw_FriendsCollect_ProgressChanged);

			// 스레드 완료(종료)시 호출되는 핸들러 동록
			BgWorkerFriendsCollect.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_FriendsCollect_RunWorkerCompleted);

			BgWorkerFriendsCollect.RunWorkerAsync();


		}


		//   백그라운드 작업 함수
		[Obsolete]
		private void bw_FriendsCollect_DoWork(object sender, DoWorkEventArgs e)
		{


			// Loop 
			while (true)
			{
				List<String> userstate = null;

				//CancellationPending 속성이 true로 set되었다면
				if ((BgWorkerFriendsCollect.CancellationPending == true))
				{
					//루프를 break한다.(즉 스레드 run 핸들러를 벗어나겠죠)
					e.Cancel = true;
					break;
				}
				else
				{
					var friendBlogerMgmtUrl = "https://admin.blog.naver.com/AdminMain.nhn?blogId=" + Common.currUser.Id + "&Redirect=BuddyMe";
					Common._driver.Navigate().GoToUrl(friendBlogerMgmtUrl);
					Common._driver.SwitchTo().Frame("papermain");// iframe 선택



					int startPage = int.Parse(txtStartPage.Text);
					int endPage = int.Parse(txtEndPage.Text);
					int progressCnt = 0;
					for (var index = startPage; index <= endPage; index++)
					{
						// 헌재페이지 읽어오기 
						var friendElements = Common._driver.FindElements(By.XPath("//*[@id='wrap']/div[2]/table/tbody/tr")); // row  추출
						Common.log.Debug(" size  ==> " + friendElements.Count);


						foreach (var friendInfo in friendElements)
						{
                            try
                            {
								var cols = friendInfo.FindElements(By.TagName("td"));


								string freindId = cols[0].FindElement(By.TagName("input")).GetAttribute("value");// 네이버아이디


								string nickname = cols[1].FindElement(By.ClassName("nickname")).Text;
								string friendBlogUrl = cols[1].FindElement(By.CssSelector("div > div > a")).GetAttribute("href");
								string blogtile = cols[1].FindElement(By.CssSelector("div > div > a")).Text;

								//구분 
								string gubun_type1 = "", gubun_type2 = "";
								var gubun1 = cols[2].FindElement(By.CssSelector("img")).GetAttribute("src");//서로이웃신청
								if (gubun1.Contains("_on.gif")) gubun_type1 = "서로이웃신청";
								else gubun_type1 = "서로이웃";
								var gubun2 = cols[2].FindElement(By.CssSelector("img")).GetAttribute("src");//이웃추가
								if (gubun2.Contains("_on.gif")) gubun_type2 = "이웃추가";
								else gubun_type2 = "이웃";
								string add_date = "20" + cols[3].Text.Replace('.', '-');
								add_date = add_date.Substring(0, add_date.Length - 1);
								Common.log.Debug(freindId + ":" + nickname + "|" + blogtile + "|" + gubun_type1 + "|" + gubun_type2 + "|" + add_date);

								DataTable dt = null;
								if (!chkDataAllDel.Checked)
									dt = DbUtil.existFriend(freindId);

								if (dt == null || dt.Rows.Count == 0)
									DbUtil.InsertFriend(freindId, nickname, blogtile, friendBlogUrl, add_date, gubun_type2);
								else
									DbUtil.UpdateFriend(freindId, nickname, blogtile, add_date, gubun_type2);

								progressCnt++;

								userstate = new List<string>();
								userstate.Add(index.ToString());
								userstate.Add("PROGRESS");
								BgWorkerFriendsCollect.ReportProgress(progressCnt, userstate);

							}catch(Exception ex)
                            {
								Common.log.Error(ex.Message);
                            }


							if ((BgWorkerFriendsCollect.CancellationPending == true))
							{
								//루프를 break한다.(즉 스레드 run 핸들러를 벗어나겠죠)
								e.Cancel = true;
								break;
							}

						}//for loop


						userstate = new List<string>();
						userstate.Add(index.ToString());
						userstate.Add("REFRESH");
						BgWorkerFriendsCollect.ReportProgress(progressCnt, userstate);

						if ((BgWorkerFriendsCollect.CancellationPending == true))
						{
							//루프를 break한다.(즉 스레드 run 핸들러를 벗어나겠죠)
							e.Cancel = true;
							break;
						}


						//페이지이동
						int intNextPage = index + 1;
						if (intNextPage > endPage)
						{
							break;
						}
						String nextpage = intNextPage.ToString();
						Common._driver.ExecuteScript("goPage('" + nextpage + "')", "");//다음페이지 이동



					}




					break;
				}
			}
		}

		//  백그라운드 작업 결과  화면 업데이트
		private void bw_FriendsCollect_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{

			int progressCnt = e.ProgressPercentage;

			List<String> userstate= e.UserState as List<String>;
			String page = userstate[0]; ;
			String state = userstate[1]; ;

			string msg = "진행중 페이지:" + page;
			msg= msg + "/ 진행건수:" + progressCnt.ToString();
			
			progressText.Text = msg;

			if ("REFRESH".Equals(state))
				DbUtil.searchFriends(dgFriendsList);//화면표시
			Application.DoEvents();

		}
		
		
		
		//백그라운드 작업 종료
		private void bw_FriendsCollect_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

            if (e.Cancelled)
            {
				MessageBox.Show("사용자에 의해 중지되었습니다.");
            }

			btnFriendsInit.Visible = true;
			btnFriendsStop.Visible = false;
			BgWorkerFriendsCollect.Dispose();
		}

		private static bool IsNumeric(string text)
		{
			double test;
			return double.TryParse(text, out test);
		}

		//친한이웃 검색
		private void btnFriendSearch_Click(object sender, EventArgs e)
		{
			DbUtil.searchFriends(dgFriendsList, searchName.Text, searchBlogTitle.Text, cboGubun.Text, searchId.Text);
			if (dgFriendsList.Rows.Count > 0)
			{
				txtStartNum.Text = Convert.ToString(dgFriendsList.Rows[0].Cells[0].Value);
				txtEndNum.Text = Convert.ToString(dgFriendsList.Rows[dgFriendsList.Rows.Count - 1].Cells[0].Value);
				FreindAutuJobName.Text="이웃["+DateTime.Now.ToString() + "]";
			}
			
		}

		//초기화
		private void intiTab2()
		{
			if(txtStartNum.Text.Length==0)
			  txtStartNum.Text = "1";
			if (txtEndNum.Text.Length == 0)
				txtEndNum.Text = "2";
			FreindAutuJobName.Text = "이웃[" + DateTime.Now.ToString() + "]";
		}

		private void FriendGridSettings()
		{


			int index = 0;//seq

			dgFriendsList.Columns[index].ReadOnly = true; ;
			dgFriendsList.Columns[index].Width = 100;


			index = 1;//id
			dgFriendsList.Columns[index].ReadOnly = true; ;
			dgFriendsList.Columns[index].Width = 100;

			index = 2;//nickname
			dgFriendsList.Columns[index].ReadOnly = true; ;
			dgFriendsList.Columns[index].Width = 200;


			index = 3;//blogtitle
			dgFriendsList.Columns[index].ReadOnly = true; ;
			dgFriendsList.Columns[index].Width = 300;

			index = 4;//url
			dgFriendsList.Columns[index].ReadOnly = true; ;
			dgFriendsList.Columns[index].Width = 150;
			dgFriendsList.Columns[index].Visible = false;

			index = 5;//네이버등록일

			dgFriendsList.Columns[index].ReadOnly = true; ;
			dgFriendsList.Columns[index].Visible = true;

			index = 6;//gubun
			dgFriendsList.Columns[index].ReadOnly = true; ;
			dgFriendsList.Columns[index].Width = 100;

		}
		// ROW 선택
		private void dgFriendsList_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0) return;
			txtUpId.Text = (string)dgFriendsList.Rows[e.RowIndex].Cells[1].Value;
			txtUpName.Text = (string)dgFriendsList.Rows[e.RowIndex].Cells[2].Value;
			if (dgFriendsList.Rows[e.RowIndex].Cells[3].Value != System.DBNull.Value)
				txtUpBlogTitle.Text = (string)dgFriendsList.Rows[e.RowIndex].Cells[3].Value;

			if (txtUpId.Text.Length > 0)
			{
				btnFriendReply.Visible = true;
				btnAddFriend.Visible = true;
			}
			else
			{
				btnFriendReply.Visible = false;
				btnAddFriend.Visible = false;
			}

			DataTable dt = DbUtil.searchMyFriends(txtUpId.Text);
			if (dt.Rows.Count > 0) btnAddFriend.Visible = false;
		}
		//이웃추가
		private void btnAddFriend_Click(object sender, EventArgs e)
		{
			string nickname = txtUpName.Text;//이름
			if (nickname == null || nickname.Length == 0)
			{
				MessageBox.Show("작업대상을 선택하세요.");
				return;
			}


			string msg = txtUpName.Text + " 님을 ";
			var result = MessageBox.Show(msg + "친한이웃으로 등록 하시겠습니까?", "친한이웃등록", MessageBoxButtons.OKCancel);
			if (result == DialogResult.Cancel)//취소
			{
				return;
			}

			DbUtil.InsertMyFriend(txtUpId.Text, txtUpName.Text, "Y");

			// 친한이웃 등록 시작 
			MessageBox.Show(msg + "친한이웃으로 등록했습니다.");
		}


		//이웃블로그 수동 댓글달기
		private void btnFriendReply_Click(object sender, EventArgs e)
		{
			// 이웃블로그 댓글달기

			string nblogUrl = Common.naver_blog_url + "/" + txtUpId.Text;//url
			string nickname = txtUpName.Text;//이름

			if(nickname==null || nickname.Length == 0)
            {
				MessageBox.Show("작업대상을 선택하세요.");
				return;
            }

			FormFriendReplyPop rp = new FormFriendReplyPop();
			FormFriendReplyPop.NickName = nickname;
			FormFriendReplyPop.BlogUrl = nblogUrl;
			rp.ShowDialog();
			string result = FormFriendReplyPop.result;

			if (result!=null && result.Length > 0)//입력성공했으면 표시
				MessageBox.Show("정상처리되었어요.^^");


		}

		//******************  자동 배치 댓글달기 start *****************************/

		// 자동 배치 댓글달기 중지 
		private void btnBatchFriendReplyStop_Click(object sender, EventArgs e)
		{
			btnBatchFriendReplyStop.Enabled = false;
			DialogResult result = MessageBox.Show("작업을 중단하시겠습니까?", "작업중단.", MessageBoxButtons.YesNo);
			if (result.Equals(DialogResult.Yes))
			{
				BgWorkerFriendsReply.CancelAsync();
			}
		}
		// 자동 배치 댓글달기실행 
		private void btnBatchFriendReply_Click(object sender, EventArgs e)
		{
			int delaytime = 1000;

			delaytime = int.Parse(cboDelayTime.Text) * 1000;

			if (dgFriendsList.Rows.Count == 0)
			{
				MessageBox.Show("작업 대상이 없습니다.");
				return;
			}

			String ReplyContent = "";
			if (txtReplyInput.Text.Length > 0)
			{
				ReplyContent = txtReplyInput.Text;
			}

			else
			{
				MessageBox.Show("댓글 내용을 입력하세요.");
				return;
			}

			int startNum = 0;
			if (txtStartNum.Text.Length > 0 && IsNumeric(txtStartNum.Text))
			{
				startNum = int.Parse(txtStartNum.Text);
			}
			else
			{
				MessageBox.Show("시작번호를 확인하세요.");
				return;
			}

			int endNum = 0;
			if (txtEndNum.Text.Length > 0 && IsNumeric(txtEndNum.Text))
			{
				endNum = int.Parse(txtEndNum.Text);
			}
			else
			{
				MessageBox.Show("끝 번호를 확인하세요.");
				return;
			}


			if (startNum > endNum)
			{
				MessageBox.Show("시작번호가 끝번호 보다 크면 안됩니다.");
				return;
			}

            if (FreindAutuJobName.Text.Length < 3)
            {
				MessageBox.Show("작업그룹명을 확인하세요..");
				return;
            }
            else
            {
				FreindAutoJobMoel chkfreindAutoJobMoel= DbUtil.getFreindAutoJob(FreindAutuJobName.Text);
                if (chkfreindAutoJobMoel != null)
                {
					MessageBox.Show("작업그룹명이 존재합니다.다른 이름을 입력하세요.");
					return;
				}

            }

			string tmpReploycontent = "아래 내용과 같이 입력됩니다. 진행하시겠습니까?" + Environment.NewLine
				                    + "네이버님 " + " " + ReplyContent;
			DialogResult result = MessageBox.Show(tmpReploycontent, "댓글내용확인", MessageBoxButtons.OKCancel);
			if (result == DialogResult.Cancel) return;

			// JOB 등록
			FreindAutoJobMoel freindAutoJobMoel = new FreindAutoJobMoel();
			freindAutoJobMoel.work_title = FreindAutuJobName.Text;
			freindAutoJobMoel.start_num = startNum;
			freindAutoJobMoel.end_num = endNum;
			freindAutoJobMoel.delaytime = int.Parse(cboDelayTime.Text);
			freindAutoJobMoel.reply_content = ReplyContent;
			freindAutoJobMoel.empathy = chkEmpathy.Checked;
            try { 
				DbUtil.InsertFreindAutoJob(freindAutoJobMoel);//DB insert

				freindAutoJobMoel = DbUtil.getFreindAutoJob(freindAutoJobMoel.work_title);
			
			}
			catch(Exception ex)           
			{
				Common.log.Debug(ex.Message);
				MessageBox.Show("작업그룹명이 등록에 문제가 있네요.");
				return;
			}
			//버튼 막기
			btnBatchFriendReply.Enabled = false;
			btnBatchFriendReplyStop.Enabled = true;

			FriendsReplyWorkerStart(freindAutoJobMoel);



		}



		//백그라운드 워커 실행 
		private void FriendsReplyWorkerStart(FreindAutoJobMoel freindAutoJobMoel)
		{


			BgWorkerFriendsReply = new BackgroundWorker();


			//ReportProgress메소드를 호출하기 위해서 반드시 true로 설정, false일 경우 ReportProgress메소드를 호출하면 exception 발생
			BgWorkerFriendsReply.WorkerReportsProgress = true;
			//스레드에서 취소 지원 여부
			BgWorkerFriendsReply.WorkerSupportsCancellation = true;
			//스레드가 run시에 호출되는 핸들러 등록
			BgWorkerFriendsReply.DoWork += new DoWorkEventHandler(bw_FriendsReply_DoWork);
			// ReportProgress메소드 호출시 호출되는 핸들러 등록
			BgWorkerFriendsReply.ProgressChanged += new ProgressChangedEventHandler(bw_FriendsReply_ProgressChanged);

			// 스레드 완료(종료)시 호출되는 핸들러 동록
			BgWorkerFriendsReply.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_FriendsReply_RunWorkerCompleted);

			Hashtable ht = new Hashtable();
			ht.Add("freindAutoJobMoel", freindAutoJobMoel);
			ht.Add("searchBlogTitle", searchBlogTitle.Text);
			ht.Add("cboGubun", cboGubun.Text);
			ht.Add("searchId", searchId.Text);
			ht.Add("searchName", searchName.Text);

			BgWorkerFriendsReply.RunWorkerAsync(ht);


		}


		//   백그라운드 작업 함수
		[Obsolete]
		private void bw_FriendsReply_DoWork(object sender, DoWorkEventArgs e)
		{

			int suc_cnt = 0;
			int fail_cnt = 0;
			// Loop 
			while (true)
			{
				List<String> userstate = null;

				//CancellationPending 속성이 true로 set되었다면
				if (BgWorkerFriendsReply.CancellationPending == true)
				{
					//루프를 break한다.
					e.Cancel = true;
					break;
				}
				else
				{
                    try
                    {

						
						Hashtable ht =e.Argument as Hashtable;
						FreindAutoJobMoel freindAutoJobMoel = (FreindAutoJobMoel)ht["freindAutoJobMoel"];// 객제
						int delaytime = freindAutoJobMoel.delaytime;// 작업 간격 
						delaytime = delaytime * 1000;
						int startNum = freindAutoJobMoel.start_num;
						int endNum = freindAutoJobMoel.end_num;
						int processCnt = 0;
						String  ReplyContent = freindAutoJobMoel.reply_content;

						string searchName = ht["searchName"] as String;
						string searchBlogTitle = ht["searchBlogTitle"] as String;
						string cboGubun = ht["cboGubun"] as String;
						string searchId = ht["searchId"] as String;

						DataTable dt = DbUtil.searchFriends(searchName, searchBlogTitle, cboGubun, searchId);

						// 대상 리스트 loop
						for (int i = 0; i < dt.Rows.Count; i++)
						{
							int curNum = Convert.ToInt32(dt.Rows[i][0]);

							if (curNum >= startNum && curNum <= endNum)
							{

								string userId = (string)dt.Rows[i][1];
								string nblogUrl = Common.naver_blog_url + "/" + userId;//url
								string nickname = (string)dt.Rows[i][2];

								//답글내용 생성 : nickname 님   prefix 
								String tmpReploycontent = nickname + "님 " + " " + ReplyContent;
								++processCnt;
								try
								{
									userstate = new List<String>();
									userstate.Add(curNum.ToString());
									userstate.Add(nickname);
									userstate.Add("START");//상태 표시
									userstate.Add(suc_cnt.ToString());//성공건수
									userstate.Add(fail_cnt.ToString());//실패건수
									BgWorkerFriendsReply.ReportProgress(processCnt, userstate);

									
									// 작업 시작
									String seq =BwWorkJob.AutoWriteReplyForMyFriend(nblogUrl, nickname, tmpReploycontent, chkEmpathy.Checked, freindAutoJobMoel.work_id.ToString(), userId);

									++suc_cnt;
									userstate = new List<String>();
									userstate.Add(curNum.ToString());
									userstate.Add(nickname);
									userstate.Add("END");//상태 표시
									userstate.Add(suc_cnt.ToString());//성공건수
									userstate.Add(fail_cnt.ToString());//실패건수
									BgWorkerFriendsReply.ReportProgress(processCnt, userstate);

								}
								catch (Exception ex)
								{
									Common.log.Debug(ex.Message);
									++fail_cnt;
									userstate = new List<String>();
									userstate.Add(curNum.ToString());
									userstate.Add(nickname);
									userstate.Add("ERROR");//상태 표시
									userstate.Add(suc_cnt.ToString());//성공건수
									userstate.Add(fail_cnt.ToString());//실패건수
									BgWorkerFriendsReply.ReportProgress(processCnt, userstate);

								}

								//작업 취소 요청됨
								if (BgWorkerFriendsReply.CancellationPending == true)
								{
									e.Cancel = true;
									break;
								}

								if (curNum >= endNum) break;// 작업 종료

								userstate = new List<String>();
								userstate.Add(curNum.ToString());
								userstate.Add(nickname);
								userstate.Add("SLEEP");//상태 표시
								userstate.Add(suc_cnt.ToString());//성공건수
								userstate.Add(fail_cnt.ToString());//실패건수
								BgWorkerFriendsReply.ReportProgress(processCnt, userstate);

								//작업 간격 
								Thread.Sleep(delaytime);

								userstate = new List<String>();
								userstate.Add(curNum.ToString());
								userstate.Add(nickname);
								userstate.Add("WAKEUP");//상태 표시
								userstate.Add(suc_cnt.ToString());//성공건수
								userstate.Add(fail_cnt.ToString());//실패건수
								BgWorkerFriendsReply.ReportProgress(processCnt, userstate);


							}

							if (curNum >= endNum) break;// 작업 종료

						}


                    }
					catch (Exception ex)
					{
						Common.log.Debug(ex.Message.ToString());

					}
					break;
				}
			}
		}

		//  백그라운드 작업 결과  화면 업데이트
		private void bw_FriendsReply_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{

			int progressCnt = e.ProgressPercentage;

			List<String> userstate = e.UserState as List<String>;
			String  curNum = userstate[0]; ;//진행번호
			String curNm = userstate[1]; ;//진행중 이름

			String workStatus = userstate[2]; ;//작업 상태
			String sucCnt = userstate[3]; ;//성공건수
			String failCnt = userstate[4]; ;//실패건수


			string msg = "진행중 번호:" + curNum+"/ 이름:" + curNm;
			msg = msg + Environment.NewLine+ "작업 상태:" + workStatus;
			msg = msg + Environment.NewLine + "진행건수:" + progressCnt.ToString();
			msg = msg + Environment.NewLine + "성공건수:" + sucCnt;
			msg = msg + Environment.NewLine + "실패건수:" + failCnt;

			BatchFriendReplyStatus.Text = msg;


			// 성공/실패를 그리드에 색으로 표시
			if(workStatus.Equals("END") || workStatus.Equals("ERROR"))
            {
				foreach (DataGridViewRow row in dgFriendsList.Rows)
				{
					if (row.Cells[0].Value.ToString().Equals(curNum))
					{
						//dgFriendsList.Rows[row.Index].Selected = true;
						
						if (workStatus.Equals("END"))//성공
						   dgFriendsList.Rows[row.Index].DefaultCellStyle.BackColor = Color.Green;
						else if (workStatus.Equals("ERROR"))//실패
							dgFriendsList.Rows[row.Index].DefaultCellStyle.BackColor = Color.Red;
						dgFriendsList.Update();
						break;
					}
				}
			}



			Application.DoEvents();

		}
		
		//백그라운드 작업 종료
		private void bw_FriendsReply_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

			if (e.Cancelled)
			{
				MessageBox.Show("사용자에 의해 중지되었습니다.");
			}

			//버튼 막기
			btnBatchFriendReply.Enabled = true;
			btnBatchFriendReplyStop.Enabled = false;
			BgWorkerFriendsReply.Dispose();
		}






		/************************************************************************************************************************************
		 * 친한이웃 소통하기 END 
		*************************************************************************************************************************************/

		/************************************************************************************************************************************
		 *  블로그홈 자동 댓글 START
		*************************************************************************************************************************************/
		// 테이블 목록 조회
		private void AutoWorkListSearch_Click(object sender, EventArgs e)
		{
			String grouptype = (cboGroupType.Text.Equals("네이버이웃작업")) ? "NH" : "BH";
			string work_id = getAutoWorkGrpCd(cboAutoWorkGrp.Text, grouptype);
			String result_cd = (cboResultCode.Text.Equals("전체")) ? "" : cboResultCode.Text;

			String searchDate = "";
            if (searchRegdate.Enabled)
            {
				searchDate = searchRegdate.Value.ToShortDateString();

			}
			DbUtil.searchAutoReplyWorkList(AutoReplyWorkGrid, searchDate, "", "", result_cd, work_id,grouptype);
		}

		//자동 댓글 바로 시작 
		private void AutoReplyWorkStart_Click(object sender, EventArgs e)
		{

			// 작업그룹 호출 
			int work_id = AutoReplyWorkBranch("IMD");
			if (work_id < 0) return;


			AutoReplyWorkStart.Enabled = false;      //즉시 댓글작업 
			AutoReplyWorkReserve.Enabled = false;   //예약작업 시작
			AutoReplyTargetSearch.Enabled = false;   //예약대상 수집
			AutoReplyTargetSearchStop.Enabled = true;//작업 중단 버튼 
			AutoWorkListSearch.Enabled = false;//검색버튼


			AutoReplyWorkerStart(int.Parse(AutoReplyDelayTime.Text), int.Parse(AutoReplyTargetCount.Text), work_id.ToString(), "IMD");//즉시작업




			usecontrols(false);

		}


		// 댓글 예약작업 시작
		private void AutoReplyWorkReserve_Click(object sender, EventArgs e)
		{

			DataTable ds = DbUtil.getAutoWorkJob(txtAutoWorkJobTitle.Text);
			AutoWorkJobMoel autoWorkJobModel = new AutoWorkJobMoel();
			autoWorkJobModel.work_title = ds.Rows[0].Field<string>("work_title");
			autoWorkJobModel.work_type = ds.Rows[0].Field<String>("work_type");
			autoWorkJobModel.target_cnt = ds.Rows[0].Field<int>("target_cnt");
			autoWorkJobModel.delaytime = ds.Rows[0].Field<int>("delaytime");
			autoWorkJobModel.work_id = ds.Rows[0].Field<int>("work_id");
			autoWorkJobModel.reply_content = ds.Rows[0].Field<String>("reply_content");


			//예약작업인지 확인
			if (!autoWorkJobModel.work_type.Equals("IMD"))
			{
				AutoReplyWorkStart.Enabled = false;      //즉시 댓글작업 
				AutoReplyWorkReserve.Enabled = false;   //예약작업 시작
				AutoReplyTargetSearch.Enabled = false;   //예약대상 수집
				usecontrols(false);


				AutoReplyWorkerJobStart(autoWorkJobModel.work_id.ToString());

			}
			else
			{
				MessageBox.Show("이미 실행한 작업 입니다..");
			}

		}
		//작업 그룹 새로 입력
		private void txtAutoWorkJobTitle_TextChanged(object sender, EventArgs e)
		{
			AutoReplyWorkStart.Enabled = true;    //즉시 댓글작업 
			AutoReplyWorkReserve.Enabled = true;   //예약작업 시작
			AutoReplyTargetSearch.Enabled = true; //예약대상 수집
		}
		// 다른 작업 그룹 선택
		private void txtAutoWorkJobTitle_SelectedIndexChanged(object sender, EventArgs e)
		{
			DataTable ds = DbUtil.getAutoWorkJob(txtAutoWorkJobTitle.Text);
			if (ds.Rows.Count > 0)
			{
				AutoWorkJobMoel autoWorkJobModel = new AutoWorkJobMoel();
				autoWorkJobModel.work_type = ds.Rows[0].Field<string>("work_type");
				autoWorkJobModel.work_title = ds.Rows[0].Field<string>("work_title");
				autoWorkJobModel.target_cnt = ds.Rows[0].Field<int>("target_cnt");
				autoWorkJobModel.delaytime = ds.Rows[0].Field<int>("delaytime");
				autoWorkJobModel.work_id = ds.Rows[0].Field<int>("work_id");
				autoWorkJobModel.reply_content = ds.Rows[0].Field<String>("reply_content");
				autoWorkJobModel.empathy = ds.Rows[0].Field<bool>("empathy");

				//txtAutoWorkJobTitle.Text = autoWorkJobModel.work_title;


				autoWorkJobModel.reply_content = autoWorkJobModel.reply_content;
				AutoReplyChkEmpathy.Checked = autoWorkJobModel.empathy;


				if ("IMD".Equals(autoWorkJobModel.work_type))//즉시 작업
				{
					AutoReplyWorkStart.Enabled = false;      //즉시 댓글작업 
					AutoReplyWorkReserve.Enabled = false;   //예약작업 시작
					AutoReplyTargetSearch.Enabled = false;   //예약대상 수집
                }
                else
                {
					AutoReplyWorkStart.Enabled = false;    //즉시 댓글작업 
					AutoReplyWorkReserve.Enabled = true;   //예약작업 시작
					AutoReplyTargetSearch.Enabled = false; //예약대상 수집
				}

            }
            else
            {

				AutoReplyWorkStart.Enabled = true;    //즉시 댓글작업 
				AutoReplyWorkReserve.Enabled = true;   //예약작업 시작
				AutoReplyTargetSearch.Enabled = true; //예약대상 수집
			}

		}

		//예약 작업을 위한  블로그홈 새글  추출 
		private void AutoReplyTargetSearch_Click(object sender, EventArgs e)
		{
			// 작업그룹 호출 
			int  work_id = AutoReplyWorkBranch("RESV");
			if (work_id < 0) return;

			AutoReplyWorkerStart(int.Parse(AutoReplyDelayTime.Text), int.Parse(AutoReplyTargetCount.Text) , work_id.ToString(),"RESV");//예약작업

			AutoReplyTargetSearch.Enabled = false;
			AutoReplyTargetSearchStop.Enabled = true;
		}


		// 예약작업 대상 추출 또는  즉시 댓글 작업 
		private int AutoReplyWorkBranch(String work_type="RESV")
        {
			//작업 그룹 등록 
			if (txtAutoWorkJobTitle.Text.Length < 2)
			{
				MessageBox.Show("작업제목을 입력하세요.");
				return -1;
			}

			DataTable ds = DbUtil.getAutoWorkJob(txtAutoWorkJobTitle.Text);
			if (ds.Rows.Count > 0)
			{
				MessageBox.Show("이미 존재하는 작업명입니다.변경해주세요.");
				return -1;
			}

			//댓글내용
			if (AutoReplyInputContent.Text.Length < 3)
			{
				MessageBox.Show("댓글내용을 두글자 이상 입력하세요.");
				return -1;
			}


			//예약 작업그룹 등록 
			AutoWorkJobMoel autoWorkJobModel = new AutoWorkJobMoel();
			autoWorkJobModel.work_title = txtAutoWorkJobTitle.Text;
			autoWorkJobModel.work_desc = txtAutoWorkJobTitle.Text;
			autoWorkJobModel.target_cnt = int.Parse(AutoReplyTargetCount.Text);
			autoWorkJobModel.delaytime = int.Parse(AutoReplyDelayTime.Text);
			autoWorkJobModel.reply_content = AutoReplyInputContent.Text;//댓글내용
			autoWorkJobModel.empathy = AutoReplyChkEmpathy.Checked;//공감여부
			autoWorkJobModel.work_type = work_type;

			String msg = "";
			msg = "시간주기:" + autoWorkJobModel.delaytime + "초" + Environment.NewLine +
		   "수집건수 : " + autoWorkJobModel.target_cnt + Environment.NewLine +
		   "작업그룹명:  " + autoWorkJobModel.work_title + Environment.NewLine;

			String msgboxTitle = "";
			if (work_type.Equals("RESV"))//예약 작업 이면
            {

				msg +="위의 내용으로 예약 작업을 위한 데이터 수집을 시작합니다.";

				msgboxTitle = "블로그홈 새글수집";


			}
            else//즉시 댓글 작업
            {
				msg += "위의 내용으로 댓글작업을 시작합니다.";
				msgboxTitle = "블로그홈 댓글작업";
				autoWorkJobModel.work_start_dtm = DateTime.Now;

			}




			DialogResult result = MessageBox.Show(msg, msgboxTitle, MessageBoxButtons.OKCancel);
			if (result == DialogResult.Cancel) return -1;


			//작업 그룹 등록 
			DbUtil.InsertAutoWorkJobReserve(autoWorkJobModel);
			makeAutoWorkGrpCombo(autoWorkJobModel.work_title);//작업그룹 콤보 새로고침

			string work_id = getAutoWorkGrpCd(autoWorkJobModel.work_title);//작업 그룹 조회

			return int.Parse(work_id);

		}

		// 블로그홈 백그라운드 작업 중지 
		private void AutoReplyTargetSearchStop_Click(object sender, EventArgs e)
		{
			String msg = "백그라운드 작업을 중지합니다."+Environment.NewLine +"현재 작업주기 시간이 지나면 중지됩니다.";
			DialogResult result = MessageBox.Show(msg, "작업중지", MessageBoxButtons.OKCancel);
			if (result == DialogResult.Cancel) return ;
			BgAutoReplyWorker.CancelAsync();


		}

		//백그라운드 워커 실행 
		private void AutoReplyWorkerStart(int delaytime, int targetCnt,String   work_id,String  work_type ="RESV")
		{

			

			BgAutoReplyWorker = new BackgroundWorker();


			//ReportProgress메소드를 호출하기 위해서 반드시 true로 설정, false일 경우 ReportProgress메소드를 호출하면 exception 발생
			BgAutoReplyWorker.WorkerReportsProgress = true;
			//스레드에서 취소 지원 여부
			BgAutoReplyWorker.WorkerSupportsCancellation = true;
			//스레드가 run시에 호출되는 핸들러 등록
			BgAutoReplyWorker.DoWork += new DoWorkEventHandler(bw_AutoReplyGet_DoWork);
			// ReportProgress메소드 호출시 호출되는 핸들러 등록
			BgAutoReplyWorker.ProgressChanged += new ProgressChangedEventHandler(bw_AutoReplyGet_ProgressChanged);

			// 스레드 완료(종료)시 호출되는 핸들러 동록
			BgAutoReplyWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_AutoReplyGet_RunWorkerCompleted);

			Hashtable ht = new Hashtable();
			ht.Add("work_id", work_id);
			BgAutoReplyWorker.RunWorkerAsync(ht);


		}


		//   백그라운드 작업 함수
		[Obsolete]
		private void bw_AutoReplyGet_DoWork(object sender, DoWorkEventArgs e)
		{


			Hashtable ht = e.Argument as Hashtable;

			string work_id = (string)ht["work_id"];


			DataTable ds = DbUtil.getAutoWorkJob("", int.Parse(work_id));
			AutoWorkJobMoel autoWorkJobModel = new AutoWorkJobMoel();
			autoWorkJobModel.work_title = ds.Rows[0].Field<string>("work_title");
			autoWorkJobModel.work_type = ds.Rows[0].Field<string>("work_type");
			autoWorkJobModel.target_cnt = ds.Rows[0].Field<int>("target_cnt");
			autoWorkJobModel.delaytime = ds.Rows[0].Field<int>("delaytime");
			autoWorkJobModel.reply_content = ds.Rows[0].Field<String>("reply_content");
			autoWorkJobModel.empathy = ds.Rows[0].Field<bool>("empathy");

			int delaytime = autoWorkJobModel.delaytime * 1000;// 작업 간격 
			int targetcnt = autoWorkJobModel.target_cnt;
			String ReplyContent = autoWorkJobModel.reply_content;
			bool isEmpathy=autoWorkJobModel.empathy;
			string work_type = autoWorkJobModel.work_type;
			int process_cnt = 0;
			int curpage = 1;
			WebDriverWait wait = new WebDriverWait(Common._driver, TimeSpan.FromSeconds(10));
			String blogHome = "https://section.blog.naver.com/BlogHome.nhn?directoryNo=0&groupId=0&currentPage=";
			String curBlogHOme= blogHome + curpage.ToString();//1페이지 시작
			Common._driver.Navigate().GoToUrl(blogHome); // 블로그 홈  접속합니다.

			Thread.Sleep(500);
			// Loop 
			while (true)
			{

				//CancellationPending 속성이 true로 set되었다면
				if (BgAutoReplyWorker.CancellationPending == true)
				{
					//루프를 break한다.
					e.Cancel = true;
					break;
				}
				else
				{

					try
					{

						var list =Common._driver.FindElements(By.XPath("//*[@id='content']/section/div[2]/child::div"));// 전체 목록 가져오기 
						int index = 1;
						foreach (var el in list)
						{
							int seq = -1;
							IList<string> tabs = new List<string>(Common._driver.WindowHandles);
							if (tabs.Count > 1)
							{
								Common._driver.SwitchTo().Window(tabs[0]);
							}
							try {

								// 이름 
								var eltext = el.FindElement(By.ClassName("name_author"));
								String nickname = "";
								if (eltext != null && !eltext.Equals(""))
									nickname = eltext.Text;

								// 시간 
								eltext = el.FindElement(By.ClassName("time"));
								String naver_reg_time = "";
								if (eltext != null && !eltext.Equals(""))
									naver_reg_time = eltext.Text;

								// 제목 
								eltext = el.FindElement(By.ClassName("title_post"));
								String title_post = "";
								if (eltext != null && !eltext.Equals(""))
									title_post = eltext.Text;


								//  글내용 
								eltext = el.FindElement(By.ClassName("text"));
								String content = "";
								if (eltext != null && !eltext.Equals(""))
									content = eltext.Text;
								//네이버아이디 추출 
								eltext = Common.FindElement(By.XPath("//*[@id='content']/section/div[2]/div[" + index + "]/div/div[1]/div[1]/a[1]"));
								String url = eltext.GetAttribute("href");
								String naverid = getNaverId(url);
								String postid = getPostId(url);


								Common.log.Debug(naverid +"/"+  nickname + "/" + title_post + "/" + content);


								if (work_type.Equals("RESV"))
								{ //예약 대상 수집


								  // 수집 데이터 입력 
									DbUtil.InsertAutoReplyWork(naverid, postid, nickname, title_post, content, work_id);


                                }
                                else
                                {

									DbUtil.InsertAutoReplyWork(naverid, postid, nickname, title_post, content, work_id);

									DataTable dt=DbUtil.getAutoReplyWork(naverid, work_id);
									
									seq = dt.Rows[0].Field<int>("seq");
									DbUtil.UpdateAutoReplyWorkStart(seq.ToString());//시작시간

									//포스팅 
									BwWorkJob.AutoReplyForPost(naverid, postid, nickname, ReplyContent, isEmpathy);

									DbUtil.UpdateAutoReplyWorkEnd(seq.ToString(),"SUCCESS");//종료시간
									
								}
								++process_cnt;
								BgAutoReplyWorker.ReportProgress(process_cnt);


								++index;

							}
							catch(Exception ex)
                            {
								Common.log.Error(ex.Message);

								if (!work_type.Equals("RESV"))
                                {
									DbUtil.UpdateAutoReplyWorkEnd(seq.ToString(), "FAIL");//종료시간
								}

							}

							if (process_cnt >= targetcnt)
							{
								break;
							}

							if (BgAutoReplyWorker.CancellationPending == true)
							{
								//루프를 break한다.
								e.Cancel = true;
								break;
							}
							if (work_type.Equals("RESV"))
							{ //예약 대상 수집은 딜레이 짧게 
								delaytime = 500;
							}
								System.Threading.Thread.Sleep(delaytime);

							if (index > list.Count) break;
							if (BgAutoReplyWorker.CancellationPending == true)
							{
								//루프를 break한다.
								e.Cancel = true;
								break;
							}
						}


					}
					catch (Exception ex)
					{
						Common.log.Error(ex.Message.ToString());

					}

					// 목표수에 도달 
					if (process_cnt >= targetcnt)
					{
						break;
                    }
                    else//목표수 미달은  다음 페이지 이동 
                    {
						++curpage;
						curBlogHOme = blogHome + curpage.ToString();//다음 페이지 
						Common._driver.Navigate().GoToUrl(curBlogHOme); // 블로그 홈 다음페이지  접속합니다.
					}
				}
			}
		}

		//  백그라운드 작업 결과  화면 업데이트
		private void bw_AutoReplyGet_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{

			int progressCnt = e.ProgressPercentage;

			string work_id = getAutoWorkGrpCd(txtAutoWorkJobTitle.Text);//작업 그룹 조회
			DbUtil.searchAutoReplyWorkList(AutoReplyWorkGrid, "", "", "","", work_id);

			txtAutoReplyWorkStatus.Text = progressCnt.ToString() + "건 진행중  /  전체 " + AutoReplyTargetCount.Text;

			Application.DoEvents();

		}

		//백그라운드 작업 종료
		private void bw_AutoReplyGet_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

			if (e.Cancelled)
			{
				MessageBox.Show("사용자에 의해 중지되었습니다.");
			}

			//버튼 막기
			AutoReplyWorkStart.Enabled = true;      //즉시 댓글작업 
			AutoReplyWorkReserve.Enabled = true;   //예약작업 시작
			AutoReplyTargetSearch.Enabled = true;   //예약대상 수집
			AutoReplyTargetSearchStop.Enabled = false;//작업 중단 버튼 
			AutoWorkListSearch.Enabled = true;//검색버튼
			usecontrols(true);
			BgAutoReplyWorker.Dispose();
		}


		//백그라운드 워커 실행 
		private void AutoReplyWorkerJobStart(String work_id)
		{


			BgAutoReplyWorker = new BackgroundWorker();


			//ReportProgress메소드를 호출하기 위해서 반드시 true로 설정, false일 경우 ReportProgress메소드를 호출하면 exception 발생
			BgAutoReplyWorker.WorkerReportsProgress = true;
			//스레드에서 취소 지원 여부
			BgAutoReplyWorker.WorkerSupportsCancellation = true;
			//스레드가 run시에 호출되는 핸들러 등록
			BgAutoReplyWorker.DoWork += new DoWorkEventHandler(bw_AutoReplyJob_DoWork);
			// ReportProgress메소드 호출시 호출되는 핸들러 등록
			BgAutoReplyWorker.ProgressChanged += new ProgressChangedEventHandler(bw_AutoReplyJob_ProgressChanged);

			// 스레드 완료(종료)시 호출되는 핸들러 동록
			BgAutoReplyWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_AutoReplyJob_RunWorkerCompleted);



			Hashtable ht = new Hashtable();

			ht.Add("work_id", work_id);

			BgAutoReplyWorker.RunWorkerAsync(ht);


		}


		//   백그라운드 작업 함수
		[Obsolete]
		private void bw_AutoReplyJob_DoWork(object sender, DoWorkEventArgs e)
		{


			Hashtable ht = e.Argument as Hashtable;
			string work_id = (string)ht["work_id"];


			int process_cnt = 0;
			DataTable dt = DbUtil.getAutoWorkJob("", int.Parse(work_id));

			int delaytime = dt.Rows[0].Field<int>("delaytime") * 1000;// 주기
			int target_cnt = dt.Rows[0].Field<int>("target_cnt");// 목표건수
			String ReplyContent = dt.Rows[0].Field<String>("reply_content");// 댓글내용
			bool isEmpathy = dt.Rows[0].Field<bool >("empathy");

			Thread.Sleep(500);
			// Loop 
			while (true)
			{
				List<String> userstate = null;

				//CancellationPending 속성이 true로 set되었다면
				if (BgAutoReplyWorker.CancellationPending == true)
				{
					//루프를 break한다.
					e.Cancel = true;
					break;
				}
				else
				{
					DataTable dt_list = DbUtil.searchAutoReplyWorkList2("", "", "", "", work_id);
					DbUtil.UpdateAutoWorkJobStartTime(work_id);
					//예약 데이터 Loop
					for (int i = 0; i < dt_list.Rows.Count; i++)
					{
						String result_code = null;
						int seq = 0;
						try
						{
							seq = dt_list.Rows[i].Field<int>("seq");
							String naverid = dt_list.Rows[i].Field<string>("naverid");
							String postId = dt_list.Rows[i].Field<string>("postid");
							String nickname = dt_list.Rows[i].Field<string>("nickname");
							result_code = dt_list.Rows[i].Field<string>("result_code");


							if (result_code.Equals("WAIT")){//결과코드가 없는 경우
								DbUtil.UpdateAutoReplyWorkStart(seq.ToString());
								
								BwWorkJob.AutoReplyForPost(naverid, postId, nickname, ReplyContent, isEmpathy);

								DbUtil.UpdateAutoReplyWorkEnd(seq.ToString(),"SUCCESS");
							}

						}
						catch (Exception ex)
						{
							Common.log.Error(ex.Message.ToString());



							if (result_code == null && seq >0)
							{//결과코드가 없는 경우
								DbUtil.UpdateAutoReplyWorkEnd(seq.ToString(), "FAIL");
							}
						}
						finally
						{
							process_cnt++; //전체처리건수

							userstate = new List<string>();
							userstate.Add(work_id);
							BgAutoReplyWorker.ReportProgress(process_cnt, userstate);

						}

						// 목표수에 도달 
						if (process_cnt >= target_cnt)
						{
						
							break;
						}

						Thread.Sleep(delaytime);// 실행 주기 

					}// for end


	
					DbUtil.UpdateAutoWorkJobEndTime(work_id);

					break;


				}
			}
		}

		//  백그라운드 작업 결과  화면 업데이트
		private void bw_AutoReplyJob_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{

			int progressCnt = e.ProgressPercentage;
			List<String> userstate = e.UserState as List<string>;
			string work_id = userstate[0];//작업 그룹 조회
			DbUtil.searchAutoReplyWorkList(AutoReplyWorkGrid, "", "", "", "", work_id);

			txtAutoReplyWorkStatus.Text = progressCnt.ToString() + "건 진행중.... ";

			Application.DoEvents();

		}

		//백그라운드 작업 종료
		private void bw_AutoReplyJob_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

			if (e.Cancelled)
			{
				MessageBox.Show("사용자에 의해 중지되었습니다.");
			}

			//버튼 막기
			AutoReplyWorkStart.Enabled = true;      //즉시 댓글작업 
			AutoReplyWorkReserve.Enabled = true;   //예약작업 시작
			AutoReplyTargetSearch.Enabled = true;   //예약대상 수집
			AutoReplyTargetSearchStop.Enabled = false;//작업 중단 버튼 
			AutoWorkListSearch.Enabled = true;//검색버튼
			usecontrols(true);
			BgAutoReplyWorker.Dispose();
		}





		// 탭변경시 
		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			// 블로그홈 댓글 탭
			if (tabControl1.SelectedIndex == 3)
			{
				intiTab3();
			}           // 네이버이웃 탭
			
			else if (tabControl1.SelectedIndex == 2)
			{
				intiTab2();
			}


		}

		//AutoWorkGroup 콤보박스 구성 
		private void makeAutoWorkGrpCombo(string selectedVal="", String group_type = "BH")
		{
			//콤보 로딩
			DataTable dt = null;
			if (group_type.Equals("BH"))
				dt = DbUtil.getAutoWorkJobList();
			else
				dt = DbUtil.getFreindAutoJobList();

			cboAutoWorkGrp.Items.Clear();
			cboAutoWorkGrp.Items.Add("전체");

			txtAutoWorkJobTitle.Items.Clear();

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				int work_id = dt.Rows[i].Field<int>("work_id");
				String work_title = dt.Rows[i].Field<string>("work_title");


				cboAutoWorkGrp.Items.Add(work_title);
				txtAutoWorkJobTitle.Items.Add(work_title);
			}
			if(selectedVal.Equals(""))
			    cboAutoWorkGrp.SelectedIndex = 0;//전체 선택
            else
            {
				cboAutoWorkGrp.Text = selectedVal;

			}

			txtAutoWorkJobTitle.Text = selectedVal;

		}

		// 콤보코드 찾기 
		private string getAutoWorkGrpCd(String work_title,String group_type="BH")
		{
			DataTable dt = null;
			if (group_type.Equals("BH"))
				dt = DbUtil.getAutoWorkJobList();
			else
				dt = DbUtil.getFreindAutoJobList();
			for (int i = 0; i < dt.Rows.Count; i++)			{
				int work_id = dt.Rows[i].Field<int>("work_id");
				String curTitle = dt.Rows[i].Field<string>("work_title");
                if (curTitle.Equals(work_title))
                {
					return work_id.ToString();
                }
			}

			return "";
		}

		bool intittab3Yn = false;
		private void intiTab3()
        {
            if (!intittab3Yn)
            {
				makeAutoWorkGrpCombo();
				cboGroupType.SelectedIndex = 0;
				intittab3Yn = true;

				//기본 작업 타이틀 입력
				txtAutoWorkJobTitle.Text = DateTime.Now.ToLongDateString() +" "+ DateTime.Now.ToLongTimeString() ;
			}

		}


		/************************************************************************************************************************************
		 *  블로그홈 자동 댓글 END
		*************************************************************************************************************************************/

		private string getBlogId(String url)
		{
			String blogids = url.Split('&')[1];
			String blogid = blogids.Split('=')[1];
			return blogid;
		}


		private string getNaverId(String blogurl)
		{
			String naverid = blogurl.Split('/')[3];
			return naverid;
		}
		private string getPostId(String blogurl)
		{
			String postid = blogurl.Split('/')[4];
			return postid;
		}

		private void tabControl2_Click(object sender, EventArgs e)
        {
			if (tabControl2.SelectedIndex == 0)
				BatchAllReply.Enabled = true;
			else
				BatchAllReply.Enabled = false;

		}



		// 콘트로 잠그기 
		private void usecontrols(bool useYN)
        {
			txtAutoWorkJobTitle.Enabled = useYN;
			AutoReplyTargetCount.Enabled = useYN;
			AutoReplyDelayTime.Enabled = useYN;

			AutoReplyInputContent.Enabled = useYN;
		}

        private void FormMyBlog_FormClosed(object sender, FormClosedEventArgs e)
        {
			if (Common._driver != null)
				Common._driver.Quit();


			DbUtil.DbClose();
		}

		// 날짜검색조건 추가 여부 
        private void chkDateSearch_CheckedChanged(object sender, EventArgs e)
        {
			searchRegdate.Enabled = chkDateSearch.Checked;

		}

        private void cboGroupType_SelectedIndexChanged(object sender, EventArgs e)
        {
			String grouptype = (cboGroupType.Text.Equals("네이버이웃작업")) ? "NH" : "BH";
			makeAutoWorkGrpCombo("", grouptype);

		}

        private void FormMyBlog_Load(object sender, EventArgs e)
        {
			bool result = false;

			while (true)
			{
				result = newLogin();

				if (result)
				{
					if(!Common.UserGroup.Equals("admin"))
						tabControl1.TabPages.Remove(tabControl1.TabPages[4]);
					break;
				}
				else
				{
					this.Close();
					break;
				}

			}

			if (BwWorkJob.isDebug)
				this.Text = this.Text + "[테스트용]";
		}

		//  사용자관리(네이버 아이디기준)
        private void 사용자관리ToolStripMenuItem_Click(object sender, EventArgs e)
        {
			MessageBox.Show("준비중.");
        }
		//사용자관리 검색
        private void btnUserSearch_Click(object sender, EventArgs e)
        {
			adminUserSearch();

		}

		private void adminUserSearch()
        {
			UserRepository userRepo = new UserRepository();
			List<Users> userlist = userRepo.GetUserWhere(txtSearchNaverId.Text, txtSearchUserName.Text);
			UsersGridView.Rows.Clear();
			for (int i = 0; i < userlist.Count; i++)
			{
				string[] rowdata = { userlist[i].Id, userlist[i].user_name, userlist[i].user_desc, userlist[i].naverId, userlist[i].user_group, (userlist[i].use_yn) ? "Y" : "N" };
				UsersGridView.Rows.Add(rowdata);

			}
		}



		private void UsersGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
			if (e.RowIndex < 0) return;
			txtUserId.Text = (string)UsersGridView.Rows[e.RowIndex].Cells[0].Value;
			txtUserName.Text = (string)UsersGridView.Rows[e.RowIndex].Cells[1].Value;
			txtUserDesc.Text = (string)UsersGridView.Rows[e.RowIndex].Cells[2].Value;
			txtNaverId.Text = (string)UsersGridView.Rows[e.RowIndex].Cells[3].Value;
			cboUserGroup.Text = (string)UsersGridView.Rows[e.RowIndex].Cells[4].Value;
			cboUse_YN.Text = (string)UsersGridView.Rows[e.RowIndex].Cells[5].Value;
		}
		//신규등록
        private void btnUserReg_Click(object sender, EventArgs e)
        {
			UserRepository userRepo = new UserRepository();
			Users user = new Users();
			user.Id = txtUserId.Text; 
			user.user_name = txtUserName.Text;
			user.user_desc = txtUserDesc.Text;
			user.naverId = txtNaverId.Text;
			user.user_group = cboUserGroup.Text;
			user.use_yn = (cboUse_YN.Text.Equals("Y"))?true:false;

			Users checkUser  = userRepo.Get(user);

			if (checkUser.naverId.Equals(user.naverId))
            {
				MessageBox.Show("네이버 아이디가 존재하므로  변경 부탁드립니다.");
				return;
            }
			userRepo.Add(user);
			MessageBox.Show("등록되었습니다.");
			adminUserSearch();
		}
		//업데이트
        private void btnUserUpd_Click(object sender, EventArgs e)
        {
            if (txtUserId.Text.Length == 0)
            {
				MessageBox.Show("수정할 사용자를 선택하세요.");
				return;
			}
			UserRepository userRepo = new UserRepository();
			Users user = new Users();
			user.Id = txtUserId.Text;
			user.user_name = txtUserName.Text;
			user.user_desc = txtUserDesc.Text;
			user.naverId = txtNaverId.Text;
			user.user_group = cboUserGroup.Text;
			user.use_yn = (cboUse_YN.Text.Equals("Y")) ? true : false;
			DialogResult result = MessageBox.Show(txtUserName.Text + "(" + txtNaverId.Text + ") 님의 정보를 수정하시겠습니까?", "사용자정보수정", MessageBoxButtons.OKCancel);

			if (result.Equals(DialogResult.Cancel)) return;//취소 

			userRepo.Update(user);

			MessageBox.Show("수정되었습니다.");
			adminUserSearch();
		}

        private void btnUserDelete_Click(object sender, EventArgs e)
        {
			if (txtUserId.Text.Length == 0)
			{
				MessageBox.Show("삭제할 사용자를 선택하세요.");
				return;
			}
			UserRepository userRepo = new UserRepository();
			Users user = new Users();
			user.Id = txtUserId.Text;


			DialogResult result= MessageBox.Show(txtUserName.Text+"("+txtNaverId.Text+") 님을 삭제하시겠습니까?","사용자삭제",MessageBoxButtons.OKCancel);

			if (result.Equals(DialogResult.Cancel)) return;//취소 

			userRepo.Delete(user);

			MessageBox.Show("삭제 되었습니다.");
			adminUserSearch();
		}
    }
}
