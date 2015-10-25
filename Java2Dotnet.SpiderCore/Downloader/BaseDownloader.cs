using System;
using System.Text;
using Java2Dotnet.Spider.Core.Selector;
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

		/// <summary>
		/// A simple method to download a url
		/// </summary>
		/// <param name="url">url</param>
		/// <returns>html</returns>
		public Html Download(string url)
		{
			return Download(url, null);
		}

		/// <summary>
		/// A simple method to download a url
		/// </summary>
		/// <param name="url">url</param>
		/// <param name="encoding">charset</param>
		/// <returns>Html</returns>
		public Html Download(string url, Encoding encoding)
		{
			var site = new Site { Encoding = encoding };
			Page page = Download(new Request(url, null), site.ToTask());
			return page.GetHtml();
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
			object cycleTriedTimesObject = request.GetExtra(Request.CycleTriedTimes);
			if (cycleTriedTimesObject == null)
			{
				// 把自己加到目标Request中(无法控制主线程再加载此Request), 传到主线程后会把TargetRequest加到Pool中
				request.Priority = 0;
				page.AddTargetRequest(request.PutExtra(Request.CycleTriedTimes, 1));
			}
			else
			{
				long cycleTriedTimes = (long)cycleTriedTimesObject;
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
