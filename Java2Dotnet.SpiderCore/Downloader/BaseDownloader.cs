using System;
using log4net;

namespace Java2Dotnet.Spider.Core.Downloader
{
	public class BaseDownloader : IDownloader, IDisposable
	{
		protected static ILog Logger;
		protected int ThreadNum;

		public BaseDownloader()
		{
			if (Logger == null)
			{
				Logger = LogManager.GetLogger(GetType());
			}
		}

		public virtual Page Download(Request request, ITask task)
		{
			return null;
		}

		protected virtual void OnSuccess(Request request)
		{
		}

		protected virtual void OnError(Request request, Exception e)
		{
			Logger.Warn("Download page " + request.Url + " failed.", e);
			//throw e;
		}

		protected Page AddToCycleRetry(Request request, Site site)
		{
			Page page = new Page(request);
			dynamic cycleTriedTimesObject = request.GetExtra(Request.CycleTriedTimes);
			if (cycleTriedTimesObject == null)
			{
				// 把自己加到目标Request中(无法控制主线程再加载此Request), 传到主线程后会把TargetRequest加到Pool中
				request.Priority = 0;
				page.AddTargetRequest(request.PutExtra(Request.CycleTriedTimes, 1));
			}
			else
			{
				int cycleTriedTimes = (int)cycleTriedTimesObject;
				cycleTriedTimes++;
				if (cycleTriedTimes >= site.CycleRetryTimes)
				{
					// 超过最大尝试次数, 返回空.
					return null;
				}
				request.Priority = 0;
				page.AddTargetRequest(request.PutExtra(Request.CycleTriedTimes, cycleTriedTimes));
			}
			page.SetNeedCycleRetry(true);
			return page;
		}

		public virtual void Dispose()
		{
		}

		public void SetThreadNum(int threadNum)
		{
			ThreadNum = threadNum;
		}
	}
}
