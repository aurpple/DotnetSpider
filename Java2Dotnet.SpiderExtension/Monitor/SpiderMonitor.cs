using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Extension.Monitor
{
	/**
	 * @author code4crafer@gmail.com
	 * @since 0.5.0
	 */
	[Experimental]
	public class SpiderMonitor
	{
		private static SpiderMonitor _instanse;

		private static readonly object Locker = new object();

		private SpiderMonitor()
		{
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public SpiderMonitor Register(params Core.Spider[] spiders)
		{
			foreach (Core.Spider spider in spiders)
			{
				MonitorSpiderListener monitorSpiderListener = new MonitorSpiderListener();
				if (spider.GetSpiderListeners() == null)
				{
					List<ISpiderListener> spiderListeners = new List<ISpiderListener> { monitorSpiderListener };
					spider.SetSpiderListeners(spiderListeners);
				}
				else
				{
					spider.GetSpiderListeners().Add(monitorSpiderListener);
				}
				ISpiderStatus spiderStatus = GetSpiderStatus(spider, monitorSpiderListener);
				Register(spider, spiderStatus, monitorSpiderListener);
			}
			return this;
		}

		private ISpiderStatus GetSpiderStatus(Core.Spider spider, MonitorSpiderListener monitorSpiderListener)
		{
			return new SpiderStatus(spider, monitorSpiderListener);
		}

		public static SpiderMonitor Instance
		{
			get
			{
				lock (Locker)
				{
					return _instanse ?? (_instanse = new SpiderMonitor());
				}
			}
		}

		private void Register(Core.Spider spider, ISpiderStatus spiderStatus, MonitorSpiderListener monitorSpiderListener)
		{
			if (spider.ShowControl)
			{
				Form1 form1 = new Form1(spiderStatus);
				form1.ShowDialog();
			}

			if (spider.SaveStatusInRedis)
			{
				RedisStatusUpdater statusUpdater = new RedisStatusUpdater(spider, spiderStatus);
				monitorSpiderListener.ClosingEvent += statusUpdater.UpdateStatus;
				statusUpdater.Run();
			}
		}

		public delegate void SpiderClosing();

		public class MonitorSpiderListener : ISpiderListener
		{
			private readonly AutomicLong _successCount = new AutomicLong(0);
			private readonly AutomicLong _errorCount = new AutomicLong(0);
			private readonly List<string> _errorUrls = new List<string>();

			public event SpiderClosing ClosingEvent;

			public void OnSuccess(Request request)
			{
				_successCount.Inc();
			}

			public void OnError(Request request)
			{
				_errorUrls.Add(request.Url);
				_errorCount.Inc();
			}

			public void OnClose()
			{
				Closed = true;
				ClosingEvent?.Invoke();
			}

			public long SuccessCount => _successCount.Value;

			public long ErrorCount => _errorCount.Value;

			public List<string> ErrorUrls => _errorUrls;

			public bool Closed { get; set; }
		}
	}
}