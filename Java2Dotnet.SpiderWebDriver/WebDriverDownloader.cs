using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Downloader;
using Java2Dotnet.Spider.Core.Selector;
using OpenQA.Selenium;

namespace Java2Dotnet.Spider.WebDriver
{
	public class WebDriverDownloader : BaseDownloader
	{
		private volatile WebDriverPool _webDriverPool;
		private readonly int _webDriverWaitTime;
		private readonly Browser _browser;
		private readonly Option _option;
		public bool LoadImage { get; set; } = true;

		public Func<IWebDriver, bool> LoginFunc;

		public WebDriverDownloader(Browser browser = Browser.Phantomjs, int webDriverWaitTime = 200, Option option = null)
		{
			_option = option ?? new Option();
			_webDriverWaitTime = webDriverWaitTime;
			_browser = browser;

			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					Process[] faultProcesses = Process.GetProcessesByName("WerFault");
					foreach (var process in faultProcesses)
					{
						try
						{
							process.Kill();
						}
						catch (Exception)
						{
							// ignored
						}
					}

					Thread.Sleep(500);
				}
				// ReSharper disable once FunctionNeverReturns
			});
		}

		public override Page Download(Request request, ITask task)
		{
			CheckInit();

			WebDriverItem driverService = null;

			try
			{
				driverService = _webDriverPool.Get();
				Site site = task.Site;
				if (!driverService.IsLogined && LoginFunc != null)
				{
					driverService.IsLogined = LoginFunc.Invoke(driverService.WebDriver);
					if (!driverService.IsLogined)
					{
						throw new SpiderExceptoin("Login failed. Please check your login codes.");
					}
				}

				IOptions manage = driverService.WebDriver.Manage();
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
				string realUrl = uri.Scheme + "://" + uri.DnsSafeHost + uri.AbsolutePath + (string.IsNullOrEmpty(query) ? ""
					: ("?" + HttpUtility.UrlPathEncode(uri.Query.Substring(1, uri.Query.Length - 1))));

				driverService.WebDriver.Navigate().GoToUrl(realUrl);

				Thread.Sleep(_webDriverWaitTime);

				Page page = new Page(request);
				page.SetRawText(driverService.WebDriver.PageSource);
				page.SetUrl(new PlainText(request.Url));
				page.SetTargetUrl(new PlainText(driverService.WebDriver.Url));

				//customer verify
				if (DownloadVerifyEvent != null)
				{
					string msg = "";
					if (!DownloadVerifyEvent(page, ref msg))
					{
						_webDriverPool.Close(driverService);
						throw new SpiderExceptoin(msg);
					}
				}

				// 结束后要置空, 这个值存到Redis会导置无限循环跑单个任务
				request.PutExtra(Request.CycleTriedTimes, null);

				return page;
			}
			finally
			{
				_webDriverPool.ReturnToPool(driverService);
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
					_webDriverPool = new WebDriverPool(_browser, ThreadNum, _option);
				}
			}
		}
	}
}