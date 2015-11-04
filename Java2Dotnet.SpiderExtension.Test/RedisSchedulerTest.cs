using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Extension.Test
{
	[TestClass]
	public class RedisSchedulerTest
	{
		[TestMethod]
		public void RedisTest()
		{
			RedisScheduler redisScheduler = new RedisScheduler("localhost", "");

			ITask task = new TestTask();
			Request request = new Request("http://www.ibm.com/developerworks/cn/java/j-javadev2-22/", 1, null);
			request.PutExtra("1", "2");
			redisScheduler.Push(request, task);
			redisScheduler.Poll(task);
			//System.out.println(poll);
		}
	}

	internal class TestTask : ITask
	{
		public string Identify => "1";

		public Site Site => null;
	};
}
