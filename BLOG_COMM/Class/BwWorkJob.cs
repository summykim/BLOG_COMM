using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLOG_COMM.Class
{
    class BwWorkJob
    {

		public static bool isDebug=false;

		//블로그홈 최신 포스트  자동 댓글달아주기 
		public static string AutoReplyForPost(string naverid, string postId, string nickname, string ReplyContent, bool isEmpathy)
		{
			//답글내용 생성 : nickname 님   prefix 
			 ReplyContent = nickname + "님 " + " " + ReplyContent;

			String postUrl = Common.naver_blog_url + "/" + naverid + "/" + postId;

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
			Common._driver.Navigate().GoToUrl(postUrl);//해당 포스팅으로 이동 
					
			Thread.Sleep(500);
			Common._driver.SwitchTo().Frame("mainFrame");


			//글포스트 문 찾기 (//*[@id="post-view222207651367"]
			IWebElement elFinded = Common.FindElement(By.XPath("//*[@id='post-view" + postId + "']"));
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

			//실행여부 
            if(isDebug == false) {

			// 답글을 입력했으면 등록처리
				if (ReplyContent.Length > 1)
				{
					if (isEmpathy)
					{

						//공감댓글찾기 & 클릭
						elFinded = Common.FindElement(By.XPath("//*[@id='area_sympathy" + postId + "']/div/a/span[1]"));
						if (elFinded != null)
							elFinded.Click();
					}

					//댓글보기크릭
					elFinded = Common.FindElement(By.XPath("//*[@id='Comi" + postId + "']"));
					if (elFinded != null)
						elFinded.Click();

					//댓글입력창클릭
					elFinded = Common.FindElement(By.XPath("//*[@id='naverComment_201_" + postId + "']/div/div[5]/div[1]/form/fieldset/div/div/div[2]/div/label"));
					if (elFinded != null)
					{
						elFinded.Click();


						elFinded = Common.FindElement(By.XPath("//*[@id='naverComment_201_" + postId + "__write_textarea']"));
						if (elFinded != null)// 댓글입력
							elFinded.SendKeys(ReplyContent);


						//등록버튼 클릭

						elFinded = Common.FindElement(By.XPath("//*[@id='naverComment_201_" + postId + "']/div/div[5]/div[1]/form/fieldset/div/div/div[6]/button"));
						if (elFinded != null)
							elFinded.Click();


					}
				

				}
			}
			return ReplyContent;

		}
		//이웃블로그 자동 댓글달아주기 
		public static string AutoWriteReplyForMyFriend(string nblogUrl, string nickname, string ReplyContent, bool isEmpathy, string work_id, String naverid)
		{
			// 작업시작 등록
			String group_type = "NH";
			DbUtil.InsertAutoReplyWork(naverid, "", nickname, "", "", work_id, group_type);
			DataTable dt = DbUtil.getAutoReplyWork(naverid, work_id, group_type);
			int seq = dt.Rows[0].Field<int>("seq");

			// 이웃블로그 자동 댓글달기

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


			DbUtil.UpdateAutoReplyWorkStart(seq.ToString());

			//첫번째 최신글  찾기 	
			elFinded = Common.FindElement(By.XPath("//*[@id='listTopForm']/table/tbody/tr[1]/td[1]/div/span/a"));
			string blogurl = elFinded.GetAttribute("href");
			string blogId = getBlogId(blogurl);
			if (elFinded == null) // 글이 없을 경우
			{
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

				DbUtil.UpdateAutoReplyWorkPost(seq.ToString(), postId, Title, "");

				// 답글을 입력했으면 등록처리
				if (ReplyContent.Length > 1 && isDebug == false)
				{
					if (isEmpathy)
					{

						//공감댓글찾기 & 클릭
						elFinded = Common.FindElement(By.XPath("//*[@id='area_sympathy" + blogId + "']/div/a/span[1]"));
						if (elFinded != null)
							elFinded.Click();

					}

					//댓글보기크릭
					elFinded = Common.FindElement(By.XPath("//*[@id='Comi" + blogId + "']"));
					if (elFinded != null)
						elFinded.Click();

					//댓글입력창클릭
					elFinded = Common.FindElement(By.XPath("//*[@id='naverComment_201_" + blogId + "']/div/div[5]/div[1]/form/fieldset/div/div/div[2]/div/label"));
					if (elFinded != null)
					{
						elFinded.Click();


						elFinded = Common.FindElement(By.XPath("//*[@id='naverComment_201_" + blogId + "__write_textarea']"));
						if (elFinded != null)
							// 댓글입력
							elFinded.SendKeys(ReplyContent);


						//등록버튼 클릭

						elFinded = Common.FindElement(By.XPath("//*[@id='naverComment_201_" + blogId + "']/div/div[5]/div[1]/form/fieldset/div/div/div[6]/button"));
						if (elFinded != null)
							elFinded.Click();


					}

				}// end if

			}
			DbUtil.UpdateAutoReplyWorkEnd(seq.ToString(), "SUCCESS");

			return seq.ToString();

			
		}
		public static  void writeReply(String blog_comment_id, String replytext, int rowindex)
		{

			if (replytext != null && replytext.Length > 1)
			{
				// 답글창  찾기
				String xpath = "//*[@id='naverComment_201_" + blog_comment_id + "_wai_u_cbox_content_wrap_tabpanel']/ul/li[" + (rowindex + 1) + "]/div[1]/div/div[4]/a";

				var element = Common._driver.FindElementByXPath(xpath);
				element.Click();
				if (isDebug == false)
				{
					xpath = "//*[@id='naverComment_201_" + blog_comment_id + "_wai_u_cbox_content_wrap_tabpanel']/ul/li[" + (rowindex + 1) + "]/div[2]/div/div[1]";

					element = Common._driver.FindElementByXPath(xpath);

					IWebElement el_reply = element.FindElement(By.ClassName("u_cbox_text"));

					el_reply.SendKeys(replytext);

					xpath = "//*[@id='naverComment_201_" + blog_comment_id + "_wai_u_cbox_content_wrap_tabpanel']/ul/li[" + (rowindex + 1) + "]/div[2]/div/div[1]/form/fieldset/div/div/div[6]/button";


					element = Common._driver.FindElementByXPath(xpath);
					element.Click();
				}


			}

		}
		static string getBlogId(String url)
		{
			String blogids = url.Split('&')[1];
			String blogid = blogids.Split('=')[1];
			return blogid;
		}
		
	}
}
