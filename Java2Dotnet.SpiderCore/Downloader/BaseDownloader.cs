using System;
using log4net;

namespace Java2Dotnet.Spider.Core.Downloader
{
	public class BaseDownloader : IDownloader, IDisposable
	{
		protected static readonly ILog Logger = LogManager.GetLogger(typeof(BaseDownloader));
		protected int ThreadNum;

		public virtual Page Download(Request request, ITask task)
		{
			return null;
		}

		protected virtual void OnSuccess(Request request)
		{
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
