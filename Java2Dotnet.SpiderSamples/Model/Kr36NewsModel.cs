using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension.Monitor;
using Java2Dotnet.Spider.Extension.Pipeline;

namespace Java2Dotnet.Spider.Samples.Model
{

	/**
	 * @author code4crafter@gmail.com <br>
	 */
	[TargetUrl(Value = new[] { "http://www.36kr.com/p/\\d+.html" })]
	[HelpUrl(Value = new[] { "http://www.36kr.com/#/page/\\d+" })]
	public class Kr36NewsModel
	{
		[ExtractBy(Value = "//h1[@class='entry-title sep10']")]
		public string Title { get; set; }

		[ExtractBy(Value = "//div[@class='mainContent sep-10']/tidyText()")]
		public string Content { get; set; }

		[ExtractByUrl]
		public string Url { get; set; }

		public static void Run()
		{
			Site site = new Site();
			site.AddStartUrl("http://www.36kr.com/");
			Core.Spider thread = OoSpider.Create(site, new PageModelToDbPipeline(), typeof(Kr36NewsModel)).SetThreadNum(20);
			thread.Start();
			SpiderMonitor spiderMonitor = SpiderMonitor.Instance;
			spiderMonitor.Register(thread);
		}
	}
}