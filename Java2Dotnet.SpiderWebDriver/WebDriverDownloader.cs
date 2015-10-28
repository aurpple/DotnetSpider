﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Downloader;
using Java2Dotnet.Spider.Core.Selector;
using OpenQA.Selenium;

namespace Java2Dotnet.Spider.WebDriver
{
	public enum Browser
	{
		Firefox,
		Phantomjs,
		Chrome
	}

	public class WebDriverDownloader : BaseDownloader
	{
		private volatile WebDriverPool _webDriverPool;
		private readonly int _webDriverWaitTime;
		private readonly Browser _browser;
		public Func<IWebDriver, bool> LoginFunc;

		public WebDriverDownloader(Browser browser = Browser.Phantomjs, int webDriverWaitTime = 200)
		{
			_webDriverWaitTime = webDriverWaitTime;
			_browser = browser;
		}

		public override Page Download(Request request, ITask task)
		{
			CheckInit();
			Site site = task.Site;
			WebDriverItem webDriver = null;
			try
			{
				webDriver = _webDriverPool.Get();
				if (!webDriver.IsLogined && LoginFunc != null)
				{
					webDriver.IsLogined = LoginFunc.Invoke(webDriver.WebDriver);
					if (!webDriver.IsLogined)
					{
						throw new SpiderExceptoin("Login failed. Please check your login codes.");
					}
				}

				IOptions manage = webDriver.WebDriver.Manage();
				if (site.GetCookies() != null)
				{
					foreach (KeyValuePair<String, String> cookieEntry in site.GetCookies())
					{
						Cookie cookie = new Cookie(cookieEntry.Key, cookieEntry.Value);
						manage.Cookies.AddCookie(cookie);
					}
				}

				//Logger.Info("Downloading page " + request.Url);

				//中文乱码URL
				Uri uri = new Uri(request.Url);
				string query = uri.Query;
				string realUrl = uri.Scheme + "://" + uri.DnsSafeHost + uri.AbsolutePath +
								 (string.IsNullOrEmpty(query)
									 ? ""
									 : ("?" + HttpUtility.UrlPathEncode(uri.Query.Substring(1, uri.Query.Length - 1))));

				webDriver.WebDriver.Navigate().GoToUrl(realUrl);

				string resultUrl = webDriver.WebDriver.Url;
				if (resultUrl.Contains("error") || resultUrl.Contains("login") || resultUrl.Contains("//www.tmall.com"))
				{
					Logger.Error("Url error: " + realUrl);
					_webDriverPool.Close(webDriver);
					// throw exception without return this webdriver
					throw new SpiderExceptoin("Browser request too much.");
				}

				Thread.Sleep(_webDriverWaitTime);

				//IWebElement webElement = webDriver.FindElement(By.XPath("/html"));
				//String content = webElement.GetAttribute("outerHTML");

				Page page = new Page(request);
				page.SetRawText(webDriver.WebDriver.PageSource);
				page.SetUrl(new PlainText(request.Url));

				// 结束后要置空, 这个值存到Redis会导置无限循环跑单个任务
				request.PutExtra(Request.CycleTriedTimes, null);

				return page;
			}
			// 应该把整个流程当成一个原子操作, 如果出错则加回Scheduler中重新调度
			//catch (Exception e)
			//{
			//	if (site.CycleRetryTimes > 0)
			//	{
			//		return AddToCycleRetry(request, site);
			//	}
			//	OnError(request, e);
			//	return null;
			//}
			finally
			{
				_webDriverPool.ReturnToPool(webDriver);
			}
		}

		public override void Dispose()
		{
			_webDriverPool?.CloseAll();
		}

		private void CheckInit()
		{
			if (_webDriverPool == null)
			{
				lock (this)
				{
					_webDriverPool = new WebDriverPool(_browser, ThreadNum);
				}
			}
		}
	}
}