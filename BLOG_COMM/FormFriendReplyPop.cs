using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using OpenQA.Selenium;
using Microsoft.Web.WebView2.Core;
namespace BLOG_COMM
{
    public partial class FormFriendReplyPop : KryptonForm
    {

		BackgroundWorker BgWorker = null;// 백그라운드워커
		public static string NickName { get; set; }
        public static string Title { get; set; }
        public static string Content { get; set; }
        public static string ReplyContent { get; set; }

        public static bool isEmpathy { get; set; }

        public static string result { get; set; }

		public static string BlogUrl { get; set; }
		private string blogId = "";
		private string WorkType = "";
		public FormFriendReplyPop()
        {
            InitializeComponent();
        }

        private void FormFriendReplyPop_Load(object sender, EventArgs e)
        {
			worker_start("1");

			
        }

        private void setInfo()
        {
            txtNickName.Text = NickName;
           // txtTitle.Text = Title;
            txtReplyInput.Text = ReplyContent;
            txtReplyInput.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            NickName = "";
            Title = "";
            Content = "";
            ReplyContent = "";
			webView2.Hide();
			this.Close();
			
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            ReplyContent = txtReplyInput.Text;
            isEmpathy = chkEmpathy.Checked;
			webView2.Hide();
			worker_start("2");
		
        }




		private void worker_start(string  worker_type)
		{

			WorkType = worker_type;
			BgWorker = new BackgroundWorker();


			//ReportProgress메소드를 호출하기 위해서 반드시 true로 설정, false일 경우 ReportProgress메소드를 호출하면 exception 발생
			BgWorker.WorkerReportsProgress = true;

			//스레드에서 취소 지원 여부
			BgWorker.WorkerSupportsCancellation = true;

			//스레드가 run시에 호출되는 핸들러 등록
			BgWorker.DoWork += new DoWorkEventHandler(bw_DoWork);


			// ReportProgress메소드 호출시 호출되는 핸들러 등록
			BgWorker.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);


			// 스레드 완료(종료)시 호출되는 핸들러 동록
			BgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

			BgWorker.RunWorkerAsync();

			btnOK.Enabled = false;
			btnCancel.Enabled = false;
			progressBar1.Visible = true;
			this.TopMost = false;
		}

		//첫번째 글찾기
		private  void FindFirstPosting()
		{
			// 이웃블로그 댓글달기

			try
			{
				int pos = 0;

				BgWorker.ReportProgress(++pos, ""); 
				string allBlogListUrl = "https://blog.naver.com/PostList.nhn?blogId=" + BlogUrl.Split('/')[3] + "&categoryNo=0&from=postList";


				IList<string> tabs = new List<string>(Common._driver.WindowHandles);
				if (tabs.Count > 1)
				{
					Common._driver.SwitchTo().Window(tabs[1]).Close();
					Common._driver.SwitchTo().Window(tabs[0]);
				}

				IJavaScriptExecutor js = (IJavaScriptExecutor)Common._driver;
				js.ExecuteScript("window.open();");
				tabs = new List<string>(Common._driver.WindowHandles);
				Common._driver.SwitchTo().Window(tabs[1]);
				Common._driver.Navigate().GoToUrl(allBlogListUrl);


				BgWorker.ReportProgress(++pos, "");

				var elFinded = Common.FindElement(By.XPath("//*[@id='toplistSpanBlind']"));
				if (elFinded != null && elFinded.Text.Equals("목록열기"))
				{
					elFinded.Click();// 목록열기 클릭
                }


				BgWorker.ReportProgress(++pos, "");


				//첫번째 최신글  찾기 	
				elFinded = Common.FindElement(By.XPath("//*[@id='listTopForm']/table/tbody/tr[1]/td[1]/div/span/a"));
				string blogurl = elFinded.GetAttribute("href");
				blogId = getBlogId(blogurl);
				if (elFinded == null)
				{
					Common.log.Debug("글이 하나도 없나봐요. 확인이 필요해요.");
					BgWorker.ReportProgress(0, "글이존재하지 않나봐요.");
					BgWorker.CancelAsync();//취소

				}
				else
				{
					string posturl = elFinded.GetAttribute("href");
					string[] arr = posturl.Split('&');
					string[] postIds = arr[1].Split('=');
					string postId = postIds[1];

					BgWorker.ReportProgress(++pos, "");

					elFinded.Click();// 최신글 클릭 이동

					BgWorker.ReportProgress(++pos, "");


					Thread.Sleep(100);
					string curBlogtUrl = "https://blog.naver.com/PostView.nhn?blogId=" + BlogUrl.Split('/')[3] + "&logNo="+ postId;

					//글포스트 문 찾기 
					elFinded = Common.FindElement(By.XPath("//*[@id='post-view" + postId + "']"));

					string Content = "";
					if (js != null)
					{
						Content = (string)js.ExecuteScript("return arguments[0].innerHTML;",elFinded);
					}



					BgWorker.ReportProgress(10, curBlogtUrl);

				}

			}
			catch (Exception ex)
			{
				Common.log.Error(ex.Message);
			}
		}

		//이웃블로그 댓글달아주기 
		private void writeReplyForMyFriend()
		{
			// 이웃블로그 댓글달기

			try
			{
				IWebElement elFinded = null;
				int pos = 0;

				BgWorker.ReportProgress(++pos, "");
				// 답글을 입력했으면 등록처리
				if (ReplyContent.Length > 1)
				{

					BgWorker.ReportProgress(++pos, ""); 
					if (isEmpathy)
					{

						//공감댓글찾기 & 클릭
						elFinded = Common.FindElement(By.XPath("//*[@id='area_sympathy" + blogId + "']/div/a/span[1]"));
						if (elFinded != null)
							elFinded.Click();
						BgWorker.ReportProgress(++pos, "");

					}

					//댓글보기크릭
					elFinded = Common.FindElement(By.XPath("//*[@id='Comi" + blogId + "']"));
					elFinded.Click();

					BgWorker.ReportProgress(++pos, "");
					//댓글입력창크릭
					elFinded = Common.FindElement(By.XPath("//*[@id='naverComment_201_" + blogId + "']/div/div[5]/div[1]/form/fieldset/div/div/div[2]/div/label"));
					BgWorker.ReportProgress(++pos, "");
					if (elFinded != null)
					{
						elFinded.Click();

						BgWorker.ReportProgress(++pos, "");

						elFinded = Common.FindElement(By.XPath("//*[@id='naverComment_201_" + blogId + "__write_textarea']"));
						// 댓글입력
						elFinded.SendKeys(ReplyContent);

						BgWorker.ReportProgress(++pos, "");

						//등록버튼 클릭

						elFinded = Common.FindElement(By.XPath("//*[@id='naverComment_201_" + blogId + "']/div/div[5]/div[1]/form/fieldset/div/div/div[6]/button"));
						elFinded.Click();
						BgWorker.ReportProgress(++pos, "");


					}


				}

			}
			catch (Exception ex)
			{
				Common.log.Error(ex.Message);
				BgWorker.ReportProgress(0, "error");

			}
		}

		//  백그라운드 작업 
		[Obsolete]
		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			if (WorkType.Equals("1"))
				FindFirstPosting();
			else
				writeReplyForMyFriend();
			Application.DoEvents();


	
		}

		// 백그라운드 작업 결과  화면 업데이트
		private  void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{

			string status= (string)e.UserState;
            if (status.Equals("")){
				progressBar1.Value = e.ProgressPercentage;
				Application.DoEvents();
            }
            else
            {
                if (e.ProgressPercentage == 10)
                {
					if (webView2 != null )
					{
						webView2.Source = new Uri(status);

					}
                }
                else
                {

					MessageBox.Show(status);
					ReplyContent = "";
				}

            }

		}

		//  백그라운드 작업 결과  종료
		private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{


			btnOK.Enabled = true;
			btnCancel.Enabled = true;
			progressBar1.Visible = false;
			if (WorkType.Equals("1"))
				setInfo();
            else
            {
				result = ReplyContent;
				this.Close();
            }

			BgWorker.Dispose();
		}

		private string getBlogId(String url)
		{
			String blogids = url.Split('&')[1];
			String blogid = blogids.Split('=')[1];
			return blogid;
		}

        private void webView2_Click(object sender, EventArgs e)
        {

        }
    }
}
