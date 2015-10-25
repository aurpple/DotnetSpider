using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Core.Scheduler 
{
	/// <summary>
	/// Priority scheduler. Request with higher priority will poll earlier.
	/// </summary>
	[Synchronization]
	public class PriorityScheduler : DuplicateRemovedScheduler, IMonitorableScheduler
	{
		public static int InitialCapacity = 5;

		private readonly Queue<Request> _noPriorityQueue = new Queue<Request>();
		private readonly PriorityBlockingQueue<Request> _priorityQueuePlus = new PriorityBlockingQueue<Request>(InitialCapacity);
		private readonly PriorityBlockingQueue<Request> _priorityQueueMinus = new PriorityBlockingQueue<Request>(InitialCapacity, new Comparator());

		protected override void PushWhenNoDuplicate(Request request, ITask task)
		{
			if (request.Priority == 0)
			{
				_noPriorityQueue.Enqueue(request);
			}
			else if (request.Priority > 0)
			{
				_priorityQueuePlus.Push(request);
			}
			else
			{
				_priorityQueueMinus.Pop();
			}
		}

		public override Request Poll(ITask task)
		{
			Request poll = _priorityQueuePlus.Pop();
			if (poll != null)
			{
				return poll;
			}
			poll = _noPriorityQueue.Dequeue();
			if (poll != null)
			{
				return poll;
			}
			return _priorityQueueMinus.Pop();
		}

		public int GetLeftRequestsCount(ITask task)
		{
			return _noPriorityQueue.Count;
		}

		public int GetTotalRequestsCount(ITask task)
		{
			return DuplicateRemover.GetTotalRequestsCount(task);
		}

		private class Comparator : IComparer<Request>
		{
			public int Compare(Request x, Request y)
			{
				if (x.Priority > y.Priority)
				{
					return -1;
				}
				if (x.Priority == y.Priority)
				{
					return 0;
				}
				return 1;
			}
		}
	}
}

