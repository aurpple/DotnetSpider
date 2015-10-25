using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Scheduler;

namespace Java2Dotnet.Spider.Extension.Monitor
{
	public class SpiderStatus : ISpiderStatus
	{
		protected readonly Core.Spider Spider;

		protected readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(SpiderStatus));

		protected readonly SpiderMonitor.MonitorSpiderListener MonitorSpiderListener;

		public SpiderStatus(Core.Spider spider, SpiderMonitor.MonitorSpiderListener monitorSpiderListener)
		{
			Spider = spider;
			MonitorSpiderListener = monitorSpiderListener;
		}

		public string Name => Spider.Identify;

		public long LeftPageCount
		{
			get
			{
				IMonitorableScheduler scheduler = Spider.GetScheduler() as IMonitorableScheduler;
				if (scheduler != null)
				{
					return scheduler.GetLeftRequestsCount(Spider);
				}
				Logger.Warn("Get leftPageCount fail, try to use a Scheduler implement MonitorableScheduler for monitor count!");
				return -1;
			}
		}

		public long TotalPageCount
		{
			get
			{
				IMonitorableScheduler scheduler = Spider.GetScheduler() as IMonitorableScheduler;
				if (scheduler != null)
				{
					return scheduler.GetTotalRequestsCount(Spider);
				}
				Logger.Warn("Get totalPageCount fail, try to use a Scheduler implement MonitorableScheduler for monitor count!");
				return -1;
			}
		}

		public long SuccessPageCount => MonitorSpiderListener.SuccessCount;

		public long ErrorPageCount => MonitorSpiderListener.ErrorCount;

		public List<string> ErrorPages => MonitorSpiderListener.ErrorUrls;

		public string Status => Spider.GetStatus().ToString();

		public int AliveThreadCount => Spider.GetThreadAliveCount();

		public int ThreadCount => Spider.ThreadNum;

		public void Start()
		{
			Spider.Start();
		}

		public void Stop()
		{
			Spider.Stop();
		}

		public DateTime StartTime => Spider.GetStartTime();

		public DateTime EndTime => Spider.GetEndOrCurrentTime();

		public double PagePerSecond
		{
			get
			{
				double runSeconds = (EndTime - StartTime).TotalSeconds;
				if (runSeconds > 0)
				{
					return SuccessPageCount / runSeconds;
				}
				return 0;
			}
		}
	}
}
