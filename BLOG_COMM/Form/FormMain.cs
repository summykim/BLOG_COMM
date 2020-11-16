using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Keys = OpenQA.Selenium.Keys;

namespace BLOG_COMM
{
	
		public partial class FormMain : Form
    {


		protected StatusBar mainStatusBar = new StatusBar();
		protected StatusBarPanel statusPanel = new StatusBarPanel();
		protected StatusBarPanel datetimePanel = new StatusBarPanel();
		protected String blog_comment_id="";
		WebDriverWait wait;
		private void CreateStatusBar()
		{
			// Set first panel properties and add to StatusBar  
			statusPanel.BorderStyle = StatusBarPanelBorderStyle.Sunken;
			statusPanel.Text = "Application started. No action yet.";
			statusPanel.ToolTipText = "Last Activity";
			statusPanel.AutoSize = StatusBarPanelAutoSize.Spring;
			mainStatusBar.Panels.Add(statusPanel);

			// Set second panel properties and add to StatusBar  
			datetimePanel.BorderStyle = StatusBarPanelBorderStyle.Raised;

			datetimePanel.ToolTipText = "DateTime: " + System.DateTime.Today.ToString();

			datetimePanel.Text = System.DateTime.Today.ToLongDateString();
			datetimePanel.AutoSize = StatusBarPanelAutoSize.Contents;
			mainStatusBar.Panels.Add(datetimePanel);
			mainStatusBar.ShowPanels = true;
			// Add StatusBar to Form controls  
			this.Controls.Add(mainStatusBar);
		}

		private void setStatusText(string statusText)
        {
			statusPanel.Text = statusText;

		}
		public FormMain()
        {
            InitializeComponent();

			CreateStatusBar();

		}

		private void FormMain_Load(object sender, EventArgs e)
		{

			newLogin();//로그인 호출 

		}
		//로그인 호출 
		private  void newLogin()
        {
			Login frmLogin = new Login();
			this.Hide();

			frmLogin.ShowDialog();//로그인 폼 호출 

			this.Show();
			Application.DoEvents();
			this.TopMost = true;
			if (Common.currUser != null)//로그인 성공
			{
				txtBlogUrl.Text = Common.currUser.BlogUrl;

				setStatusText("로그인성공..");
				buttonLogin.Text = "로그아웃";

				
				//getBlogList();
			}
			this.TopMost = false;
		}


		//로그아웃 버튼 클릭
		private void buttonLogin_Click(object sender, EventArgs e)
        {
			var result = MessageBox.Show("다른계정으로 로그인할까요?", "로그아웃", MessageBoxButtons.OKCancel);

			if (result == DialogResult.Cancel) return;
				
			Common._driver.Quit();
			Common.currUser = null;
			setStatusText("로그아웃..");
			dataGridView1.Rows.Clear();
			dataGridView2.Rows.Clear();
			dgvBlog.Rows.Clear();
			PostUrl.Text = "";
			txtPostTitle.Text = "";
			tabControl1.SelectedIndex = 0;
			newLogin();//로그인 호출 
		}


		// 내 블로그 목록 가져오기 
		private void getBlogList()
		{
			btnBlogList.Enabled = false;//조회버튼  disable

			WebDriverWait wait = new WebDriverWait(Common._driver, TimeSpan.FromSeconds(10));
			Common._driver.Navigate().GoToUrl(txtBlogUrl.Text); // 블로그 리스트에 접속합니다.
			Application.DoEvents();
			Thread.Sleep(500);
			Common._driver.SwitchTo().Frame("mainFrame");

			wait.Until(driver => Common._driver.FindElement(By.XPath("//*[@id='categoryTitle']")).Displayed);

			//펼쳐졌는제 체크
			IWebElement Result = FindElement(By.XPath("//*[@id='listCountToggle']"));
			if (Result != null)//펼쳐져있으면 30줄선택 
			{
				Result.Click();
				Application.DoEvents();
				Result = FindElement(By.XPath("//*[@id='changeListCount']/a[5]"));
				if (Result != null) Result.Click();
			}
			else//접혀있음.
			{
				//전체보기 타이틀 클릭
				Result = FindElement(By.XPath("//*[@id='categoryTitle']"));
				if (Result != null) Result.Click();
				Application.DoEvents();
				//펼쳐져있으면 30줄선택
				Result = FindElement(By.XPath("//*[@id='listCountToggle']"));
				if (Result != null) Result.Click();
				Result = FindElement(By.XPath("//*[@id='changeListCount']/a[5]"));
				if (Result != null) Result.Click();
			}






			IWebElement cntel = Common._driver.FindElementByXPath("//*[@id='category-name']/div/table[2]/tbody/tr/td[2]/div/h4");
			int allcnt = int.Parse(cntel.Text.Replace("전체보기", "").Replace("개의 글", "").Trim());
			var list = Common._driver.FindElementsByXPath("//*[@id='listTopForm']/table/tbody/tr");
			makeBlogList();
			int index = 1, cnt = 1;
			foreach (var el in list)
			{

				var eltext = FindElement(By.XPath("//*[@id='listTopForm']/table/tbody/tr[" + index + "]/td[1]/div/span/a"));
				String title = "";
				if (eltext != null && !eltext.Equals(""))
					title = eltext.Text;
				else
					continue;
			

				String url = eltext.GetAttribute("href");
				String blogids = url.Split('&')[1];
				String blogid = blogids.Split('=')[1];
				String postUrl = txtBlogUrl.Text + "/" + blogid;

				eltext = FindElement(By.XPath("//*[@id='listTopForm']/table/tbody/tr[" + index + "]/td[3]/div/span"));
				String writedt = eltext.Text;

				string[] row = new string[] { index.ToString(), title, postUrl, writedt };
				dgvBlog.Rows.Add(row);
				index = index + 2;
				cnt++;
				Application.DoEvents();
				if (cnt > 30 || cnt>allcnt) break;
			}

			DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
			dgvBlog.Columns.Add(btn);
			btn.HeaderText = "포스팅";
			btn.Text = "이동";
			btn.Name = "btn";
			btn.UseColumnTextForButtonValue = true;

			btnBlogList.Enabled = true;//조회버튼  enable
		}

		private string getBlogId(String url)
        {
			String blogids = url.Split('&')[1];
			String blogid = blogids.Split('=')[1];
			return blogid;
		}
		//내 블로그그리드  정의
		private void makeBlogList()
		{
			dgvBlog.Rows.Clear();
			dgvBlog.ColumnCount = 4;
			dgvBlog.Columns[0].Name = "NO";
			dgvBlog.Columns[0].Visible = false;
			dgvBlog.Columns[1].Name = "제목";
			dgvBlog.Columns[1].Width = 200;
			dgvBlog.Columns[1].ReadOnly = true;

			dgvBlog.Columns[2].Name = "URL";
			dgvBlog.Columns[2].Width = 300;
			dgvBlog.Columns[2].ReadOnly = true;
			dgvBlog.Columns[3].Name = "작성일";
			dgvBlog.Columns[3].Width = 200;
			dgvBlog.Columns[3].ReadOnly = true;

			dgvBlog.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
		}

		// 내 블로그 셀 클릭이벤트 
		private void dgvBlog_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 4)//이동버튼
			{

				//MessageBox.Show((e.RowIndex + 1) + "  Row  " + (e.ColumnIndex + 1) + "  Column button clicked ");
				var index = dgvBlog.Rows[e.RowIndex].Cells[0].Value;
				if (index != null)
				{

					PostUrl.Text = (String)dgvBlog.Rows[e.RowIndex].Cells[2].Value;
					txtPostTitle.Text=(String)dgvBlog.Rows[e.RowIndex].Cells[1].Value;
					tabControl1.SelectedTab = tabReply;
					getBlogReplayList(PostUrl.Text);
				}
			}
		}



		// 내블로그 재 조회
		private void btnBlogList_Click(object sender, EventArgs e)
		{
			allReply.Enabled = false;
			dataGridView1.Rows.Clear();
			dataGridView2.Rows.Clear();
			dgvBlog.Rows.Clear();
			PostUrl.Text = "";
			txtPostTitle.Text = "";
			getBlogList();
		}

		/********* 내 블로그 댓글 작성 시작 ******************************/
		// 내 블로그 댓글 목록 구성 
		private void makeList()
		{
			DataGridView dataGv = dataGridView1;
			dataGv.Rows.Clear();
			dataGv.ColumnCount = 5;
			dataGv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			
			int index = 0;
			dataGv.Columns[index].Name = "URL";
			dataGv.Columns[index].ReadOnly = true;
			dataGv.Columns[index].Visible = false;
			dataGv.Columns[index++].Width = 0;


			dataGv.Columns[index].Name = "이름";
			dataGv.Columns[index].ReadOnly = true;
			dataGv.Columns[index++].Width = 150;


			dataGv.Columns[index].Name = "일시";
			dataGv.Columns[index].ReadOnly = true;
			dataGv.Columns[index++].Width = 200;

			dataGv.Columns[index].Name = "댓글내용";
			dataGv.Columns[index].ReadOnly = true;
			dataGv.Columns[index++].Width = 300;

			dataGv.Columns[index].Name = "답글달아주기";
			dataGv.Columns[index].ReadOnly = true;
			dataGv.Columns[index++].Width = 400;

		}

		//블로그 댓글 리스트조회
		private void getBlogReplayList(String PostUrl)
		{
			String[] url = PostUrl.Split('/');
			blog_comment_id = url[url.Length - 1];
			WebDriverWait wait = new WebDriverWait(Common._driver, TimeSpan.FromSeconds(10));
			Common._driver.Navigate().GoToUrl(PostUrl); // 웹 사이트.에 접속합니다.
			Common._driver.SwitchTo().Frame("mainFrame");

			IWebElement Result = FindElement(By.XPath("//*[@id='Comi" + blog_comment_id + "']"));
			if (Result != null) Result.Click();


			var list = Common._driver.FindElementsByXPath("//*[@id='naverComment_201_" + blog_comment_id + "_wai_u_cbox_content_wrap_tabpanel']/ul/li");
			makeList();
			makeList2();
			var index = 1;
			foreach (var el in list)
			{




				var elnick = el.FindElement(By.ClassName("u_cbox_nick"));
				String nickname = elnick.Text;

				elnick = el.FindElement(By.ClassName("u_cbox_thumb_wrap"));
				String blogurl = elnick.GetAttribute("href");


				elnick = el.FindElement(By.ClassName("u_cbox_date"));
				String update = elnick.Text;


				elnick = el.FindElement(By.ClassName("u_cbox_contents"));
				String content = elnick.Text;
				string[] row = new string[] { blogurl, nickname, update, content };
				dataGridView1.Rows.Add(row);


			
				string[] row2 = new string[] { blogurl, nickname };
				dataGridView2.Rows.Add(row2);
				index++;

			}

            if (index > 1)
            {

				DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
				dataGridView1.Columns.Add(btn);
				btn.HeaderText = "답글달아주기";
				btn.Text = "답글달기";
				btn.Name = "btn";
				btn.UseColumnTextForButtonValue = true;

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

				allReply.Visible = true;
			}
		}

		// 내 블로그 댓글 그리드 셀 클릭 이벤트
		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
			int colIndex = e.ColumnIndex;
			if (colIndex == 5)//내블로그 댓글달아주기
			{

				ReplyPop.NickName = (string)dataGridView1.Rows[e.RowIndex].Cells[1].Value;//이름
				ReplyPop.Content = (string)dataGridView1.Rows[e.RowIndex].Cells[3].Value;//내용

				ReplyPop.ReplyContent = (string)dataGridView1.Rows[e.RowIndex].Cells[4].Value;


				ReplyPop rp = new ReplyPop();
				this.TopMost = false;
				rp.ShowDialog();
				this.TopMost = true;
				dataGridView1.Rows[e.RowIndex].Cells[4].Value = ReplyPop.ReplyContent;
				this.TopMost = false;

			}

		}
		private void writeReply(int rowindex, Boolean auto=false)
        {
			//MessageBox.Show((e.RowIndex + 1) + "  Row  " + (e.ColumnIndex + 1) + "  Column button clicked ");
			String replytext = (string)dataGridView1.Rows[rowindex].Cells[4].Value;
			if (replytext != null && replytext.Length > 1)
			{
				String xpath = "//*[@id='naverComment_201_" + blog_comment_id + "_wai_u_cbox_content_wrap_tabpanel']/ul/li[" + (rowindex + 1) + "]/div[1]/div/div[4]/a";
				//wait.Until(driver => Common._driver.FindElementByXPath(xpath).Displayed);
				var element = Common._driver.FindElementByXPath(xpath);
				element.Click();

				xpath = "//*[@id='naverComment_201_" + blog_comment_id + "_wai_u_cbox_content_wrap_tabpanel']/ul/li[" + (rowindex + 1) + "]/div[2]/div/div[1]";
				//wait.Until(driver => Common._driver.FindElementByXPath(xpath).Displayed);
				element = Common._driver.FindElementByXPath(xpath);
				
				Clipboard.SetText((string)replytext);


				IWebElement el_reply = element.FindElement(By.ClassName("u_cbox_text"));
				
				el_reply.SendKeys(Keys.Control + "v");

				xpath = "//*[@id='naverComment_201_" + blog_comment_id + "_wai_u_cbox_content_wrap_tabpanel']/ul/li[" + (rowindex + 1) + "]/div[2]/div/div[1]/form/fieldset/div/div/div[6]/button";
				//wait.Until(driver => Common._driver.FindElementByXPath(xpath).Displayed);

				element = Common._driver.FindElementByXPath(xpath);
				element.Click();


			}
			else
			{
				if(!auto)
					MessageBox.Show("댓글을 입력해주세요.");
			}
		}


		private void allReply_Click(object sender, EventArgs e)
		{
            if (dataGridView1.Rows.Count == 0)
            {
				MessageBox.Show("데이터가 없네요.");
				return;
            }

			DialogResult result = MessageBox.Show("일괄댓글을 실행할까요?", "일괄실행", MessageBoxButtons.OKCancel);

            if(result == DialogResult.OK)//실행
            {
				for (int i = 0; i < dataGridView1.Rows.Count; i++)
				{
					writeReply(i, true);
					Thread.Sleep(1000);

				}

				getBlogReplayList(PostUrl.Text);
			}

		}

		private void btnBlogConnect_Click_1(object sender, EventArgs e)
        {
            if (!PostUrl.Text.Contains("https://blog.naver.com/") && PostUrl.Text.Split('/').Length<3)
            {
				MessageBox.Show("정확한 블로그 주소를 입력하세요.");
				return;

			}

			getBlogReplayList(PostUrl.Text);


		}





		// 내 블로그 댓글 사용자 목록 구성 
		private void makeList2()
		{

			DataGridView dataGv = dataGridView2;
			dataGv.Rows.Clear();
			dataGv.ColumnCount = 3;
			dataGv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

			int index = 0;
			dataGv.Columns[index].Name = "URL";
			dataGv.Columns[index].ReadOnly = true;
			dataGv.Columns[index].Visible = false;
			dataGv.Columns[index++].Width = 0;


			dataGv.Columns[index].Name = "이름";
			dataGv.Columns[index].ReadOnly = true;
			dataGv.Columns[index++].Width = 150;

			dataGv.Columns[index].Name = "댓글결과";
			dataGv.Columns[index].ReadOnly = true;
			dataGv.Columns[index++].Width = 400;

		}
		// 내 블로그 댓글 그리드 셀 클릭 이벤트
		private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			
			int colIndex = e.ColumnIndex;
			
			if (colIndex == 3)// 이웃블로그 댓글달기
			{

				string nblogUrl = (string)dataGridView2.Rows[e.RowIndex].Cells[0].Value;//url
				string nickname = (string)dataGridView2.Rows[e.RowIndex].Cells[1].Value;//이름

				string result=writeReplyForMyFriend(nblogUrl, nickname);

				if(result.Length>0)//입력성공했으면 표시
					dataGridView2.Rows[e.RowIndex].Cells[2].Value = result;

			}
			else if (colIndex == 4)// 친한이웃등록
			{

				string nblogUrl = (string)dataGridView2.Rows[e.RowIndex].Cells[0].Value;//url
				string naverid = nblogUrl.Split('/')[3];
				string nickname = (string)dataGridView2.Rows[e.RowIndex].Cells[1].Value;//이름

	
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

		//이웃블로그 댓글달아주기 (첫글을 찾아서 ..)

		private string  writeReplyForMyFriend(string nblogUrl, string nickname)
		{
			// 이웃블로그 댓글달기
			
				this.TopMost = false;



				string allBlogListUrl = "https://blog.naver.com/PostList.nhn?blogId=" + nblogUrl.Split('/')[3] + "&categoryNo=0&from=postList";

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


				var elFinded = Common.FindElement(By.XPath("//*[@id='toplistSpanBlind']"));
				if (elFinded != null && elFinded.Text.Equals("목록열기"))
				{
					elFinded.Click();// 목록열기 클릭
				}



				//첫번째 최신글  찾기 	
				elFinded = Common.FindElement(By.XPath("//*[@id='listTopForm']/table/tbody/tr[1]/td[1]/div/span/a"));
				string blogurl = elFinded.GetAttribute("href");
				string blogId = getBlogId(blogurl);
				if (elFinded == null)
				{
					this.TopMost = true;
					MessageBox.Show("글이 하나도 없나봐요. 확인이 필요해요.");
					return "";

				}
				else
				{
					string posturl = elFinded.GetAttribute("href");
					string[] arr = posturl.Split('&');
					string[] postIds = arr[1].Split('=');
					string postId = postIds[1];


					elFinded.Click();// 최신글 클릭 이동



					Thread.Sleep(500);


					//글포스트 문 찾기 
					elFinded = Common.FindElement(By.XPath("//*[@id='post-view" + postId + "']"));
					var ArrText = elFinded.FindElements(By.ClassName("se-fs-"));
					string Title = "";
					string Contents = "";
					for (int i = 0; i < ArrText.Count; i++)
					{
						if (i == 0)
							Title = ArrText[i].Text;
						else
						{
							Contents += ArrText[i].Text + System.Environment.NewLine;

						}

					}


					FriendReplyPop.NickName = nickname;//이름
					FriendReplyPop.Title = Title;//제목
					FriendReplyPop.Content = Contents;//내용

					FriendReplyPop.ReplyContent = "";

					this.TopMost = true;
					FriendReplyPop rp = new FriendReplyPop();


					this.TopMost = false;
					rp.ShowDialog();
					this.TopMost = true;

					this.TopMost = false;
					// 답글을 입력했으면 등록처리
					if (FriendReplyPop.ReplyContent.Length > 1)
					{
						//댓글보기크릭
						elFinded = Common.FindElement(By.XPath("//*[@id='Comi" + blogId + "']"));
						elFinded.Click();

						//댓글입력창크릭
						elFinded = Common.FindElement(By.XPath("//*[@id='naverComment_201_" + blogId + "']/div/div[5]/div[1]/form/fieldset/div/div/div[2]/div/label"));
						if (elFinded != null)
						{
							elFinded.Click();


							elFinded = Common.FindElement(By.XPath("//*[@id='naverComment_201_" + blogId + "__write_textarea']"));
							// 댓글입력
							elFinded.SendKeys(FriendReplyPop.ReplyContent);


							//등록버튼 클릭

							elFinded = Common.FindElement(By.XPath("//*[@id='naverComment_201_" + blogId + "']/div/div[5]/div[1]/form/fieldset/div/div/div[6]/button"));
							elFinded.Click();




						}
						else
						{
							MessageBox.Show("입력창을 찾지 못했어요.");
							return "";
						}

					}
					this.TopMost = true;
					return FriendReplyPop.ReplyContent;

				}
		}


		private IWebElement FindElement(By selector)
		{
			// Return null by default
			IWebElement elementToReturn = null;

			try
			{
				// Use the selenium driver to find the element
				elementToReturn = Common._driver.FindElement(selector);
			}
			catch (NoSuchElementException)
			{
				// Do something if the exception occurs, I am just logging
				Console.WriteLine($"No such element: {selector.ToString()} could be found.");
			}
			catch (Exception e)
			{
				// Throw any error we didn't account for
				throw e;
			}

			// return either the element or null
			return elementToReturn;
		}




        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
	
			if (Common._driver!=null )
			  Common._driver.Quit();


			DbUtil.DbClose();
		}

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void PostUrl_TextChanged(object sender, EventArgs e)
        {

        }

		private Boolean isChange=false;
        private void TabIndexchanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2)//친구댓글
            {
				isChange = true;
			}
			else if (tabControl1.SelectedIndex == 1)//내블로그 댓글
			{
				IList<string> tabs = new List<string>(Common._driver.WindowHandles);
				if (tabs.Count > 1)
				{
					Common._driver.SwitchTo().Window(tabs[0]);
				}
				if (isChange == true)
                {
					getBlogReplayList(PostUrl.Text);
					isChange = false;
				}
            }
			else if(tabControl1.SelectedIndex == 0)//내블로그 댓글목록

			{
				dataGridView1.Rows.Clear();
				dataGridView2.Rows.Clear();
				PostUrl.Text = "";
				txtPostTitle.Text = "";
			}
			else if (tabControl1.SelectedIndex == 3)//친한이웃 목록

			{
				DbUtil.GridviewLoad(dgFriendsList);
			}
		}
		//프로그램 종료
        private void button1_Click(object sender, EventArgs e)
        {
			var result=MessageBox.Show("정말 종료하실거죠?", "프로그램종료", MessageBoxButtons.OKCancel);
			if(result==DialogResult.OK)
				Application.Exit();
        }

		/* 
		 * 친한이웃 소통하기 시작 
		 */

		//친한이웃 수집
        private void btnFriendsInit_Click(object sender, EventArgs e)
        {
			if(txtStartPage.Text.Length==0 || !IsNumeric(txtStartPage.Text))
            {
				MessageBox.Show("시작 페이지 번호를 입력하세요.");
				return;
			}
			else if (txtEndPage.Text.Length == 0 || !IsNumeric(txtEndPage.Text))
			{
				MessageBox.Show("마지막 페이지 번호를 입력하세요.");
				return;
			}

			if(int.Parse(txtEndPage.Text)< int.Parse(txtStartPage.Text))
            {
				MessageBox.Show("시작 페이지 번호가  마지막 페이지 번호 보다 클수는 없습니다.");
				return;
			}

			string start_msg = txtStartPage.Text + " 페이지 부터 " + txtEndPage.Text + " 페이지 까지 \n";
			var result = MessageBox.Show(start_msg+ "네이버 이웃블로그 사용자 수집을 시작합니다. \n 전체 데이터를 삭제하고 시작합니다.", "네이버 이웃블로그수집", MessageBoxButtons.OKCancel);
			if (result == DialogResult.Cancel)
            {
				return;
            }

				var friendBlogerMgmtUrl = "https://admin.blog.naver.com/AdminMain.nhn?blogId=" + Common.currUser.Id + "&Redirect=BuddyMe";
			Common._driver.Navigate().GoToUrl(friendBlogerMgmtUrl);
			Common._driver.SwitchTo().Frame("papermain");// iframe 선택


			//*[@id="wrap"]/div[2]/div[3]/div/a[2]

			//전체 데이터 삭제
			DbUtil.allDeleteFriends();
			DbUtil.searchFriends(dgFriendsList);
			FriendGridSettings();
			int startPage = int.Parse(txtStartPage.Text);
			int endPage = int.Parse(txtEndPage.Text);
			int progressCnt = 0;
		  for(var index= startPage; index <= endPage; index++) {
				// 헌재페이지 읽어오기 
				var friendElements = Common._driver.FindElements(By.XPath("//*[@id='wrap']/div[2]/table/tbody/tr")); // row  추출
				Console.WriteLine(" size  ==> " + friendElements.Count);

			
				foreach (var friendInfo in friendElements)
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
					Console.WriteLine(freindId + ":" + nickname + "|" + blogtile + "|" + gubun_type1 + "|" + gubun_type2 + "|" + add_date);
					DbUtil.InsertFriend(freindId, nickname, blogtile,friendBlogUrl, add_date, gubun_type2);

					progressCnt++;
					string msg = "진행중 페이지:" + index.ToString();
					progressText.Text = msg + "/ 진행건수:" + progressCnt.ToString();
					

				}



				DbUtil.searchFriends(dgFriendsList);//화면표시

				//페이지이동
				int intNextPage = index + 1;
                if (intNextPage > endPage)
                {
					break;
                }
				String nextpage = intNextPage.ToString();
				Common._driver.ExecuteScript("goPage('"+ nextpage + "')", "");//다음페이지 이동

			}


			progressText.Text = "수집 완료 건수:" + progressCnt.ToString();

			MessageBox.Show("수집작업 완료 총건수:" + progressCnt.ToString());
		}


		private  static bool IsNumeric(string text)
		{
			double test;
			return double.TryParse(text, out test);
		}

        private void btnFriendSearch_Click(object sender, EventArgs e)
        {
			DbUtil.searchFriends(dgFriendsList, searchName.Text,searchBlogTitle.Text,cboGubun.Text,searchId.Text);

		}

		private void FriendGridSettings()
		{

			
			int index = 0;//seq
			
			dgFriendsList.Columns[index].ReadOnly = true; ;
			dgFriendsList.Columns[index].Width = 100;


			index =1;//id
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
			dgFriendsList.Columns[index].Visible = false;

			index = 5;//네이버등록일

			dgFriendsList.Columns[index].ReadOnly = true; ;
			dgFriendsList.Columns[index].Visible = false;

			index = 6;//gubun
			dgFriendsList.Columns[index].ReadOnly = true; ;
			dgFriendsList.Columns[index].Width = 100;

		}
		// ROW 선택
		private void dgFriendsList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
			if (e.RowIndex < 0) return;
			txtUpId.Text = (string)dgFriendsList.Rows[e.RowIndex].Cells[1].Value;
			txtUpName.Text = (string)dgFriendsList.Rows[e.RowIndex].Cells[2].Value;
			if(dgFriendsList.Rows[e.RowIndex].Cells[3].Value!=System.DBNull.Value)
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
		}
		//이웃추가
        private void btnAddFriend_Click(object sender, EventArgs e)
        {
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


		//댓글달기
        private void btnFriendReply_Click(object sender, EventArgs e)
        {
			// 이웃블로그 댓글달기
			
				string nblogUrl = Common.naver_blog_url+"/"+ txtUpId.Text;//url
				string nickname = txtUpName.Text;//이름

				string result = writeReplyForMyFriend(nblogUrl, nickname);

			if (result.Length > 0)//입력성공했으면 표시
				MessageBox.Show("정상처리되었어요.^^");

			
		}
		//친한친구 검색
        private void btnSearchMyFRIEND_Click(object sender, EventArgs e)
        {
			DbUtil.searchMyFriends(dgMyFriendsList, txtSearchFname.Text,"", txtSearchFId.Text);

		}

	
        private void dgMyFriendsList_CellClick(object sender, DataGridViewCellEventArgs e)
        {



		}

        private void dgFriendsList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

		// 친한친구 댓글달기
        private void btnReplyMyFriend_Click(object sender, EventArgs e)
        {
			// 이웃블로그 댓글달기

			string nblogUrl = Common.naver_blog_url + "/" + txtMyFId.Text;//url
			string nickname = txtMyFName.Text;//이름

			string result = writeReplyForMyFriend(nblogUrl, nickname);

			if (result.Length > 0)//입력성공했으면 표시
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
			int cnt=DbUtil.DeleteMyFriends(txtMyFId.Text);
			string message = "정상 삭제되었습니다.";
			if (cnt == 0) message = "삭제에 살패했습니다.";
			else DbUtil.searchMyFriends(dgMyFriendsList, txtSearchFname.Text, "", txtSearchFId.Text);

			MessageBox.Show(message);


		}
		//친한친구 상세
		private void dgMyFriendsList_CellClick(object sender, DataGridViewCellMouseEventArgs e)
        {
			if (e.RowIndex < 0) return;
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
    }


}
