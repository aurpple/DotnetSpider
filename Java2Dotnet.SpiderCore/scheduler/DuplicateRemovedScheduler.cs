using Java2Dotnet.Spider.Core.Scheduler.Component;
using log4net;

namespace Java2Dotnet.Spider.Core.Scheduler
{
	/// <summary>
	/// Remove duplicate urls and only push urls which are not duplicate.
	/// </summary>
	public abstract class DuplicateRemovedScheduler : IScheduler
	{
		protected static ILog Logger;

		protected DuplicateRemovedScheduler()
		{
			Logger = LogManager.GetLogger(GetType());
		}

		protected IDuplicateRemover DuplicateRemover { get; set; } = new HashSetDuplicateRemover();

		public void Push(Request request, ITask task)
		{
			if (!DuplicateRemover.IsDuplicate(request, task) || ShouldReserved(request))
			{
				//_logger.InfoFormat("Push to queue {0}", request.Url);
				PushWhenNoDuplicate(request, task);
			}
		}

		public virtual void Init(ITask task)
		{
		}

		public virtual Request Poll(ITask task)
		{
			return null;
		}

		protected virtual void PushWhenNoDuplicate(Request request, ITask task)
		{
		}

		private bool ShouldReserved(Request request)
		{
			var cycleTriedTimes = (int?)request.GetExtra(Request.CycleTriedTimes);

			return cycleTriedTimes > 0;
		}
	}
}