using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Lib;

namespace Java2Dotnet.Spider.Core.Scheduler.Component
{
	/// <summary>
	/// BloomFilterDuplicateRemover for huge number of urls.
	/// </summary>
	public class BloomFilterDuplicateRemover : IDuplicateRemover
	{
		private readonly int _expectedInsertions;
		private readonly BloomFilter _bloomFilter;
		private readonly double _fpp;
		private AtomicInteger _counter;

		public BloomFilterDuplicateRemover(int expectedInsertions)
			: this(expectedInsertions, 0.01)
		{
		}

		public BloomFilterDuplicateRemover(int expectedInsertions, double fpp)
		{
			_expectedInsertions = expectedInsertions;
			_fpp = fpp;
			_bloomFilter = RebuildBloomFilter();
		}

		protected BloomFilter RebuildBloomFilter()
		{
			_counter = new AtomicInteger(0);
			return new BloomFilter(_fpp, _expectedInsertions);
		}

		public bool IsDuplicate(Request request, ITask task)
		{
			bool isDuplicate = _bloomFilter.Contains(request.Url);
			if (!isDuplicate)
			{
				_bloomFilter.Add(request.Url);
				_counter.Inc();
			}
			return isDuplicate;
		}

		public void ResetDuplicateCheck(ITask task)
		{
			RebuildBloomFilter();
		}

		public int GetTotalRequestsCount(ITask task)
		{
			return _counter.Value;
		}
	}
}