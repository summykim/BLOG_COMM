﻿using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLOG_COMM
{
	class Common
	{
		private  ChromeDriverService _driverService = null;
		private  ChromeOptions _options = null;
		public static ChromeDriver _driver = null;
		public static UserInfo currUser = null;
		private string action_url = "https://nid.naver.com/nidlogin.login";
		public static string naver_blog_url = "https://blog.naver.com";

		protected String blog_comment_id = "";
		public static WebDriverWait wait;
		public static readonly ILog log = LogManager.GetLogger(typeof(Common));

		public Common(){

			optionInit();
		}


		// 크롬드라이버  설정 
		public void optionInit()
        {
			_driverService = ChromeDriverService.CreateDefaultService();
			_driverService.HideCommandPromptWindow = true;

			_options = new ChromeOptions();
			_options.AddArgument("disable-gpu");
			_options.AddArgument("window-size=1920,1080");
			_options.AddArgument("disable-extensions");
			_options.AddArgument("proxy-server='direct://'");
			_options.AddArgument("proxy-bypass-list=*");
			_options.AddArgument("start-maximized");
			_options.AddArgument("disable-dev-shm-usage");
			_options.AddArgument("no-sandbox");
			_options.AddArgument("ignore-certificate-errors");
			_options.AddArgument("disable-infobars");
			_options.AddArgument("--disable-extensions");

		}

		// 네이버 로그인 처리 
		public Boolean NaverLogin(string id, string pw, Boolean ShowBrowser = true)
		{
			if (!ShowBrowser)
			{
				_options.AddArgument("headless"); // 창을 숨기는 옵션입니다.

				debugPrint(">>브라우저 숨기기 실행");
			}

			try
			{
				_driver = new ChromeDriver(_driverService, _options);
				wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
				_driver.Navigate().GoToUrl(action_url); // 웹 사이트에 접속합니다.

				_driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);


				_driver.ExecuteScript(@"document.getElementById('id').value='" +id +"'");

				_driver.ExecuteScript(@"document.getElementById('pw').value='" + pw + "'");

				// 로그인 버튼 클릭
				IWebElement form = FindElement(By.XPath(@"//*[@id='log.login']"));

				form.Click();
				
				Thread.Sleep(500);

				if (_driver.Url.Contains("deviceConfirm.nhn"))
				{
					debugPrint("deviceConfirm 만남 " );

					var element = _driver.FindElementByXPath("//*[@id='new.save']");
					element.Click();

				}

				//로그인성공여부 체크 
				
				_driver.SwitchTo().Frame("minime");// 내정보 iframe 선택
				var loginResult=FindElement(By.ClassName("btn_logout"));
                if (loginResult != null)
                {
					UserInfo loginInfo = new UserInfo();
					loginInfo.Id = id;
					loginInfo.Pwd = pw;
					loginInfo.BlogUrl = naver_blog_url+"/" + id;

					Common.currUser = loginInfo;

					return true;
                }
                else
                {
					return false;
				}


			}
			catch(Exception ex)
			{
				log.Error(ex.Message);
				MessageBox.Show(ex.Message);
				return false;
			}


		}


		// appSettings 프로퍼티 값 가져오기
		public static string GetSetting(string key)
		{
			return ConfigurationManager.AppSettings[key];
		}
		// appSettings 프로퍼티 값 저장
		public static void SetSetting(string key, string value)
		{
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			configuration.AppSettings.Settings[key].Value = value;
			configuration.Save(ConfigurationSaveMode.Full, true);
			ConfigurationManager.RefreshSection("appSettings");
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
				log.Error($"No such element: {selector.ToString()} could be found.");
			}
			catch (Exception e)
			{
				// Throw any error we didn't account for
				log.Error($"No such : {e.Message} could be found.");
			}

			// return either the element or null
			return elementToReturn;
		}

		public static void debugPrint(string message)
        {
			log.Debug(message);
        }
	}
}
