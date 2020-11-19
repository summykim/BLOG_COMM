using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using SimpleBrowser.WebDriver;
namespace BLOG_COMM
{
    class Phantom
    {

		public static SimpleBrowserDriver _driver = null;
		public static UserInfo currUser = null;
		private string action_url = "https://nid.naver.com/nidlogin.login";
		public static string naver_blog_url = "https://blog.naver.com";


		// 네이버 로그인 처리 
		public Boolean NaverLogin(string id, string pw, Boolean ShowBrowser = true)
		{

			_driver = new SimpleBrowserDriver();


            try
			{
				_driver.Navigate().GoToUrl(action_url);

				var questions = _driver.FindElements(By.ClassName("question-hyperlink"));


				Clipboard.SetText(id);

				var keysEventInput = _driver.FindElement(By.Name("id"));
				keysEventInput.Click();
				keysEventInput.SendKeys(OpenQA.Selenium.Keys.Control + "v");
				debugPrint("ID: " + keysEventInput.Text);

				/*Actions action = new Actions(_driver);
				IAction pressControl = action.KeyDown(OpenQA.Selenium.Keys.Control)
				.SendKeys("v")
				.KeyUp(OpenQA.Selenium.Keys.Control).Build();
				pressControl.Perform(); */

				Clipboard.SetText(pw);
				keysEventInput = _driver.FindElement(By.Name("pw"));
				keysEventInput.Click();
				keysEventInput.SendKeys(OpenQA.Selenium.Keys.Control + "v");
				debugPrint("pw: " + keysEventInput.Text);
				var form = _driver.FindElement(By.CssSelector("input.btn_global[type=submit]"));
				form.Submit();

				Thread.Sleep(500);

				debugPrint(_driver.PageSource);
				if (_driver.Url.Contains("deviceConfirm.nhn"))
				{
					debugPrint("deviceConfirm 만남 ");

					var element = _driver.FindElement(By.XPath("//*[@id='new.save']"));
					element.Click();

				}

				//로그인성공여부 체크 

				_driver.SwitchTo().Frame("minime");// 내정보 iframe 선택
				var loginResult = FindElement(By.ClassName("btn_logout"));
				if (loginResult != null)
				{
					UserInfo loginInfo = new UserInfo();
					loginInfo.Id = id;
					loginInfo.Pwd = pw;
					loginInfo.BlogUrl = naver_blog_url + "/" + id;

					Common.currUser = loginInfo;

					return true;
				}
				else
				{
					return false;
				}

			}
			catch
			{
				return false;
			}


		}



		// 엘리먼트 찾기 
		public static IWebElement FindElement(By selector)
		{
			// Return null by default
			IWebElement elementToReturn = null;

			try
			{
				// Use the selenium driver to find the element
				elementToReturn = _driver.FindElement(selector);
			}
			catch (NoSuchElementException)
			{
				// Do something if the exception occurs, I am just logging
				Console.WriteLine($"No such element: {selector.ToString()} could be found.");
			}
			catch (Exception e)
			{
				// Throw any error we didn't account for
				Console.WriteLine($"No such : {e.Message} could be found.");
			}

			// return either the element or null
			return elementToReturn;
		}

		public static void debugPrint(string message)
		{
			Console.WriteLine(message);
		}
	}
}
