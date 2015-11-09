using System.Threading.Tasks;
using Java2Dotnet.Spider.Core.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Scheduler
{
	[TestClass]
	public class QueueSchedulerTestcase
	{
		[TestMethod]
		public void QueueSchedulerPushPollSynchronized()
		{
			QueueScheduler scheduler = new QueueScheduler();
			ITask task = new DefaultTask("test", new Site());

			Parallel.For(0, 1000, new ParallelOptions() { MaxDegreeOfParallelism = 30 }, i =>
			{
				scheduler.Push(new Request("a", 1, null), task);
				scheduler.Push(new Request("a", 1, null), task);
				scheduler.Push(new Request("a", 1, null), task);

				scheduler.Push(new Request("b", 1, null), task);

				scheduler.Push(new Request(i.ToString(), 1, null), task);
			});

			Parallel.For(0, 1000, new ParallelOptions() { MaxDegreeOfParallelism = 30 }, i =>
			{
				scheduler.Poll(task);
			});

			int left = scheduler.GetLeftRequestsCount(task);
			int total = scheduler.GetTotalRequestsCount(task);

			Assert.AreEqual(left, 2);
			Assert.AreEqual(total, 1002);
		}

		[TestMethod]
		public void QueueSchedulerPush()
		{
			QueueScheduler scheduler = new QueueScheduler();
			ITask task = new DefaultTask("test", new Site());
			scheduler.Push(new Request("a", 1, null), task);
			scheduler.Push(new Request("a", 1, null), task);
			scheduler.Push(new Request("a", 1, null), task);

			scheduler.Push(new Request("b", 1, null), task);
			int left = scheduler.GetLeftRequestsCount(task);
			int total = scheduler.GetTotalRequestsCount(task);

			Assert.AreEqual(left, 2);
			Assert.AreEqual(total, 2);
		}


		[TestMethod]
		public void QueueSchedulerPoll()
		{
			QueueScheduler scheduler = new QueueScheduler();
			ITask task = new DefaultTask("test", new Site());
			scheduler.Push(new Request("a", 1, null), task);
			scheduler.Push(new Request("a", 1, null), task);
			scheduler.Push(new Request("a", 1, null), task);

			scheduler.Push(new Request("b", 1, null), task);

			var request = scheduler.Poll(task);
			Assert.AreEqual(request.Url, "a");

			int left = scheduler.GetLeftRequestsCount(task);
			int total = scheduler.GetTotalRequestsCount(task);

			Assert.AreEqual(left, 1);
			Assert.AreEqual(total, 2);
		}
	}
}
