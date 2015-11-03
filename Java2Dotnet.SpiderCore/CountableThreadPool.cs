using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Core
{
	/// <summary>
	/// Thread pool for workers. 
	/// Use {@link java.util.concurrent.ExecutorService} as inner implement.  
	/// New feature: 
	/// 1. Block when thread pool is full to avoid poll many urls without process.
	/// 2. Count of thread alive for monitor.
	/// </summary>
	public class CountableThreadPool
	{
		private readonly int _maxDegreeOfParallelism;
		private readonly int _maxTaskCount;
		private readonly TaskFactory _factory;
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly LinkedList<Task> _tasks = new LinkedList<Task>();
		private bool _end;

		public CountableThreadPool(int threadNum = 5)
		{
			_maxDegreeOfParallelism = threadNum;
			_maxTaskCount = _maxDegreeOfParallelism + threadNum;

			LimitedConcurrencyLevelTaskScheduler lcts = new LimitedConcurrencyLevelTaskScheduler(threadNum);
			_factory = new TaskFactory(lcts);

			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					if (_end)
					{
						break;
					}

					lock (_tasks)
					{
						var finishedTasks = _tasks.Where(t => t.IsCompleted).ToList();
						foreach (var finishedTask in finishedTasks)
						{
							_tasks.Remove(finishedTask);
						}
						Thread.Sleep(50);
					}
				}
			});
		}

		public int GetThreadAlive()
		{
			lock (_tasks)
			{
				return _tasks.Count(t => t.Status == TaskStatus.Running);
			}
		}

		public int GetThreadNum()
		{
			return _maxDegreeOfParallelism;
		}

		private int GetAliveAndWaitingThreadCount()
		{
			lock (_tasks)
			{
				return _tasks.Count(t => t.Status == TaskStatus.Running || t.Status == TaskStatus.RanToCompletion || t.Status == TaskStatus.WaitingToRun);
			}
		}

		public void Execute(Func<object, CancellationTokenSource, int> func, object obj)
		{
			// List中保留比最大线程数多5个
			while (GetAliveAndWaitingThreadCount() > _maxTaskCount)
			{
				Thread.Sleep(50);
			}

			var task = _factory.StartNew(o =>
				{
					CancellationTokenSource cts1 = (CancellationTokenSource)o;
					func.Invoke(obj, cts1);
				}, _cts);

			lock (_tasks)
			{
				// ReSharper disable once InconsistentlySynchronizedField
				_tasks.AddLast(task);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void WaitToEnd()
		{
			_end = true;

			lock (_tasks)
			{
				Task.WaitAll(_tasks.ToArray());
			}
		}

		public bool IsShutdown => _cts.IsCancellationRequested;

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Shutdown()
		{
			_cts.Cancel();
			_end = true;
			while (!_cts.IsCancellationRequested)
			{
				Thread.Sleep(500);
			}
		}
	}
}