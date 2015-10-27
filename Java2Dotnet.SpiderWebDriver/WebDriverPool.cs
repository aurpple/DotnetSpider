using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.PhantomJS;

namespace Java2Dotnet.Spider.WebDriver
{
	public class WebDriverPool
	{
		private readonly ILog _logger = LogManager.GetLogger(typeof(WebDriverPool));

		private static int DEFAULT_CAPACITY = 5;

		private readonly int _capacity;

		private static int STAT_RUNNING = 1;

		private static int STAT_CLODED = 2;

		private readonly AtomicInteger _stat = new AtomicInteger(STAT_RUNNING);

		private readonly Browser _browser;

		/**
		 * store webDrivers created
		 */
		private readonly BlockingCollection<WebDriverItem> _webDriverList = new BlockingCollection<WebDriverItem>();
		private readonly ConcurrentQueue<WebDriverItem> _innerQueue = new ConcurrentQueue<WebDriverItem>();
		/**
		 * store webDrivers available
		 */

		public WebDriverPool(Browser browser, int capacity = 5)
		{
			_capacity = capacity;
			_browser = browser;
		}

		public WebDriverPool() : this(Browser.Phantomjs, DEFAULT_CAPACITY)
		{
		}

		public WebDriverItem Get()
		{
			CheckRunning();

			if (_webDriverList.Count < _capacity)
			{
				if (_innerQueue.Count == 0)
				{
					IWebDriver e = null;
					switch (_browser)
					{
						case Browser.Phantomjs:
							e = new PhantomJSDriver();
							break;
						case Browser.Firefox:
							e = new FirefoxDriver();
							break;
					}
					_innerQueue.Enqueue(new WebDriverItem(e));
					_webDriverList.Add(new WebDriverItem(e));
				}
			}

			//else
			//{
			//	while (true)
			//	{
			//		lock (_innerQueue)
			//		{
			//			if (_innerQueue.Count > 0)
			//			{
			//				break;
			//			}
			//			Thread.Sleep(150);
			//		}
			//	}
			//}

			WebDriverItem webDriver;
			while (!_innerQueue.TryDequeue(out webDriver))
			{
				Thread.Sleep(150);
			}

			return webDriver;
		}

		public void ReturnToPool(WebDriverItem webDriver)
		{
			CheckRunning();

			if (_webDriverList.Contains(webDriver))
			{
				_innerQueue.Enqueue(webDriver);
			}
		}

		private void CheckRunning()
		{
			if (!_stat.CompareAndSet(STAT_RUNNING, STAT_RUNNING))
			{
				throw new SpiderExceptoin("Already closed!");
			}
		}

		public void CloseAll()
		{
			lock (_innerQueue)
			{
				bool b = _stat.CompareAndSet(STAT_RUNNING, STAT_CLODED);
				if (!b)
				{
					throw new SpiderExceptoin("Already closed!");
				}

				foreach (WebDriverItem webDriver in _webDriverList)
				{
					_logger.Info("Quit webDriver" + webDriver);
					Close(webDriver);
				}
			}
		}

		public void Close(WebDriverItem webDriver)
		{
			try
			{
				if (_webDriverList.Contains(webDriver))
				{
					_webDriverList.TryTake(out webDriver);
					webDriver.WebDriver.Quit();
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}
	}
}
