using System.Collections.Generic;

namespace Java2Dotnet.Spider.Core.Scheduler
{
	/// <summary>
	/// Basic Scheduler implementation. 
	/// Store urls to fetch in LinkedBlockingQueue and remove duplicate urls by HashMap.
	/// </summary>
	//check:
	//[Synchronization]
	public class QueueScheduler : DuplicateRemovedScheduler, IMonitorableScheduler
	{
		//check:
		private readonly Queue<Request> _queue = new Queue<Request>();

		protected override void PushWhenNoDuplicate(Request request, ITask task)
		{
			_queue.Enqueue(request);
		}

		public override Request Poll(ITask task)
		{
			return _queue.Count > 0 ? _queue.Dequeue() : null;
		}

		public int GetLeftRequestsCount(ITask task)
		{
			return _queue.Count;
		}

		public int GetTotalRequestsCount(ITask task)
		{
			return DuplicateRemover.GetTotalRequestsCount(task);
		}
	}
}