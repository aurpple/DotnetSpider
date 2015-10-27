using System;
using Java2Dotnet.Spider.Core.Pipeline;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Pipeline
{
	[TestClass]
	public class FilePipelineTest
	{
		private ResultItems _resultItems;
		private ITask _task;

		[TestInitialize]
		public void Before()
		{
			_resultItems = new ResultItems();
			_resultItems.Put("content", "webmagic 爬虫工具");
			Request request = new Request("http://www.baidu.com", 1, null);
			_resultItems.Request = request;
			_task = new TestTask();

		}

		private class TestTask : ITask
		{
			public string Identify => Guid.NewGuid().ToString();

			public Site Site => null;
		};

		[TestMethod]
		public void TestProcess()
		{
			FilePipeline filePipeline = new FilePipeline();
			filePipeline.Process(_resultItems, _task);
		}
	}
}
