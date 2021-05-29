using BLOG_COMM.Class;
using OpenQA.Selenium;
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

namespace BLOG_COMM
{
    public partial class FormAddNFriends : Form
    {
        BackgroundWorker BgWorker = null;// 백그라운드워커
        public FormAddNFriends()
        {
            InitializeComponent();
			btnAddNFriendCancel.Enabled = false;
			btnAddNFriendStart.Enabled = true;

			var list = DbUtil.getAddFreindJobLog(DateTime.Today.ToShortDateString());
			int today_cnt = 0;
			if (list != null) today_cnt = list.Count;
			StatusLabel4.Text = "오늘이웃추가건수: " + today_cnt.ToString();
		}

		private string getNaverId(String blogurl)
		{
			String naverid = blogurl.Split('/')[3];
			return naverid;
		}
		private void worker_start()
		{


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


			Hashtable ht = new Hashtable();
			ht.Add("keyword", txtNFriendSearchKeyword.Text);
			ht.Add("grpNm", txtAddGrpNm.Text);
			ht.Add("content", txtNFriendReqContent.Text);
			ht.Add("strMaxCnt", cboNFriendMaxCnt.Text);
			BgWorker.RunWorkerAsync( ht);

		}

		//첫번째 글찾기
		private void FindNFriendPostByKeyword(DoWorkEventArgs e,string keyword,string grpNm,string content,string strMaxCnt)
		{
			// 이웃블로그 댓글달기

			    // 블로그 이동
				string BlogUrl = "https://section.blog.naver.com";
	
				IList<string> tabs = new List<string>(Common._driver.WindowHandles);
				if (tabs.Count > 1)
				{
					Common._driver.SwitchTo().Window(tabs[1]).Close();
					Common._driver.SwitchTo().Window(tabs[0]);
				}

				IJavaScriptExecutor js = (IJavaScriptExecutor)Common._driver;
				js.ExecuteScript("window.open();");
				tabs = new List<string>(Common._driver.WindowHandles);

				int pageNo = 1;
				int successCnt = 0;
				int processCnt = 0;
				int maxCnt = int.Parse(strMaxCnt);

				while (true)
              {
				//CancellationPending 속성이 true로 set되었다면
				if (BgWorker.CancellationPending == true)
				{
					//루프를 break한다.
					e.Cancel = true;
					break;
				}
				else
				{
					string searchBlogUrl = BlogUrl + "/Search/Post.naver?pageNo=" + pageNo.ToString() + "&rangeType=ALL&orderBy=sim&keyword=" + keyword;

					Common._driver.SwitchTo().Window(tabs[1]);
					Common._driver.Navigate().GoToUrl(searchBlogUrl);//블로그 이동

					//블로그 목록   찾기 	
					var blogElements = Common._driver.FindElements(By.ClassName("list_search_post")); // row  추출
					Common.log.Debug(" size  ==> " + blogElements.Count);


					foreach (var blogInfo in blogElements)
					{
						//CancellationPending 속성이 true로 set되었다면
						if (BgWorker.CancellationPending == true)
						{
							//루프를 break한다.
							e.Cancel = true;
							break;
						}

						processCnt++;
						List<string> userstate = new List<string>();
						userstate.Add(pageNo.ToString());//페이지번호
						userstate.Add(processCnt.ToString());//진행건수
						userstate.Add(maxCnt.ToString());//목표건수					
						BgWorker.ReportProgress(successCnt, userstate);

						// 글쓴이 찾기 
						var nameAuthor = blogInfo.FindElement(By.ClassName("author"));

						//블로그 URL 찾기
						var targetBlogUrl = nameAuthor.GetAttribute("href");

						string  friend_id=getNaverId(targetBlogUrl);


						//블로그이동 
						tabs = new List<string>(Common._driver.WindowHandles);
						Common.log.Debug(" tabs size  ==> " + tabs.Count);
						if (tabs.Count > 2)
						{
							Common._driver.SwitchTo().Window(tabs[2]).Close();
							Common._driver.SwitchTo().Window(tabs[1]);
						}

						js = (IJavaScriptExecutor)Common._driver;
						js.ExecuteScript("window.open();");
						tabs = new List<string>(Common._driver.WindowHandles);
						Common._driver.SwitchTo().Window(tabs[2]);
						Common._driver.Navigate().GoToUrl(targetBlogUrl);// 타겟 블로그 이동
						Common._driver.SwitchTo().Frame("mainFrame");// iframe 선택

						// 이웃추가 가능한지 확인 
						var isAddFriend = Common.FindElement(By.CssSelector("#blog-profile > div > div.bg-body > div > div.btn_area > a > span"));
						Common.log.Debug("isAddFriend.Text  ==> " + isAddFriend.GetAttribute("innerHTML"));
						if (isAddFriend != null && !"이웃추가".Equals(isAddFriend.GetAttribute("innerHTML")))//이웃추가불가 
						{
							Common._driver.SwitchTo().Window(tabs[1]);
							continue;
						}

						//이웃추가버튼 찾기 
						var findBtn = Common.FindElement(By.XPath("//*[@id='blog-profile']/div/div[2]/div/div[3]/a"));
						findBtn.Click();//이웃추가 클릭
						Thread.Sleep(300);
						//팝업선택
						Common._driver.SwitchTo().Window(Common._driver.WindowHandles.ToList().Last());

						Thread.Sleep(300);
						//서로이웃 선택 옵션찾기
						var findOpt = Common.FindElement(By.XPath("//*[@id='content']/div/form/fieldset/div[2]/div[1]/p/span[2]"));
						bool each_buddy_add = false;

						if (findOpt != null)
						{
							Console.WriteLine("findOpt  disabled" + findOpt.GetAttribute("disabled"));
							// 서로이웃추가 가능
							if (!"true".Equals(findOpt.GetAttribute("disabled")))
							{
								each_buddy_add = true;
								findOpt.Click();// 서로이웃 옵션 선택							
												//js.ExecuteScript("document.getElementById('each_buddy_add').checked = true;");
							}

						}
						else//이웃취소면  닫기
						{

							findOpt = Common.FindElement(By.XPath("//*[@id='buddy_delete']"));
							if (findOpt != null)
							{
								//*[@id="content"]/div/form/fieldset/div[3]/a[1]
								Common._driver.Close();
								//리스트 탭 이동
								Common._driver.SwitchTo().Window(tabs[1]);
								continue;
							}


						}

						//다음버튼 찾기 
						var nextBtn = Common.FindElement(By.ClassName("_buddyAddNext"));
						if (nextBtn != null) nextBtn.Click();

						//서로 이웃 신청중일때는   alert 창 표시됨	
						try
						{
							Common._driver.SwitchTo().Alert().Accept();
							Common._driver.SwitchTo().Window(tabs[1]);
							continue;
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}

						try
						{
							var cboList = Common.FindElement(By.XPath("/ html / body / form / div / div / fieldset / div[2] / div[1] / div[1] / div / a"));
							if (cboList != null)
							{
								cboList.Click();


								//그룹명 찾기
								var grpNmList = Common._driver.FindElements(By.XPath("/ html / body / form / div / div / fieldset / div[2] / div[1] / div[1] / div / div / a"));
								
								foreach (var elGrpNm in grpNmList)
								{
									//그룹명을 찾으면 클릭
									string tmpGrpNm = elGrpNm.GetAttribute("groupname");
									Common.log.Debug("groupname  ==> " + tmpGrpNm);

									if (grpNm.Equals(tmpGrpNm))
									{
										elGrpNm.Click();
										break;
									}
								}
								//내용입력
								if (each_buddy_add)//서로이웃일때만 가능
								{
									var elContent = Common.FindElement(By.XPath("//*[@id='message']"));
									if (elContent != null) elContent.SendKeys(content);
								}

								//다음버튼 이동 
								nextBtn = Common.FindElement(By.ClassName("button_next"));
								if (nextBtn != null) nextBtn.Click();



							}
							// DB 로그 등록
							DbUtil.InsertAddFreindJobLog(friend_id);
							++successCnt;
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						//닫기 
						Common._driver.Close();
						//리스트 탭 이동
						Common._driver.SwitchTo().Window(tabs[1]);
						Thread.Sleep(500);

					}//for end

					++pageNo;//페이지 이동 

					// 오늘 작업이  최대값을 초과했는지 확인  작업 종료 조건
					string today = DateTime.Today.ToShortDateString();
					List<AddFriendJobModel> curList=DbUtil.getAddFreindJobLog(today);
					if(curList.Count>=maxCnt )
                    {
						BgWorker.CancelAsync();
						break;
					}

				}
			}//while end


		}

		


		//  백그라운드 작업 
		[Obsolete]
		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			Hashtable ht = e.Argument as Hashtable;
			string keyword = (string)ht["keyword"];
			string grpNm = (string)ht["grpNm"];
			string content = (string)ht["content"];
			string strMaxCnt = (string)ht["strMaxCnt"];
			FindNFriendPostByKeyword(e,keyword, grpNm, content, strMaxCnt);
		}

		// 백그라운드 작업 결과  화면 업데이트
		private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{

			List<string> userstate = ( List<string>)e.UserState;
			int i = 0;
			string pageNo= userstate[i++];//페이지번호
			string processCnt = userstate[i++];//진행건수
			string maxCnt = userstate[i++];//목표건수		

			StatusLabel.Text = "성공건수: " + e.ProgressPercentage.ToString() ;
			StatusLabel2.Text = "현재페이지: " + pageNo;
			StatusLabel3.Text = "전체진행건수: " + processCnt;

			var list = DbUtil.getAddFreindJobLog(DateTime.Today.ToShortDateString());
			int today_cnt = 0;
			if (list!=null) today_cnt = list.Count;
			StatusLabel4.Text = "오늘이웃추가건수: " + today_cnt.ToString();

			// 오늘 작업이  최대값을 초과했는지 확인  작업 종료 조건

			if (today_cnt >=int.Parse(maxCnt) )
			{
				BgWorker.CancelAsync();
			}
		}

		//  백그라운드 작업 결과  종료
		private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

			btnAddNFriendCancel.Enabled = false;
			btnAddNFriendStart.Enabled = true;

			BgWorker.Dispose();
			if(e.Cancelled)
				MessageBox.Show("이웃추가작업이 취소 되었습니다.", "작업중지");
			else
				MessageBox.Show("이웃추가작업이 완료 되었습니다.", "작업완료");

		}

		private void btnAddNFriendStart_Click(object sender, EventArgs e)
        {
			btnAddNFriendCancel.Enabled = true;
			btnAddNFriendStart.Enabled = false ;

			worker_start();
			
		}

        private void btnAddNFriendCancel_Click(object sender, EventArgs e)
        {
			String msg = "백그라운드 작업을 중지합니다." + Environment.NewLine + "현재 작업주기 시간이 지나면 중지됩니다.";
			DialogResult result = MessageBox.Show(msg, "작업중지", MessageBoxButtons.OKCancel);
			if (result == DialogResult.Cancel) return;
				BgWorker.CancelAsync();
		}
    }
}
