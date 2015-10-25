using System.Collections.Concurrent;

namespace Java2Dotnet.Spider.Core.Scheduler.Component
{
	public class HashSetDuplicateRemover : IDuplicateRemover
	{
		private readonly ConcurrentDictionary<string, string> _urls = new ConcurrentDictionary<string, string>();

		public bool IsDuplicate(Request request, ITask task)
		{
			bool isDuplicate = _urls.ContainsKey(request.Url);
			if (!isDuplicate)
			{
				_urls.GetOrAdd(request.Url, string.Empty);
			}
			return isDuplicate;
		}

		public void ResetDuplicateCheck(ITask task)
		{
			_urls.Clear();
		}

		public int GetTotalRequestsCount(ITask task)
		{
			return _urls.Count;
		}
	}
}
