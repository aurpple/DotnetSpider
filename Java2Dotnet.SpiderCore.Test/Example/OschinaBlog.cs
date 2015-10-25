using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension.Pipeline;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Example
{
	[TargetUrl(Value = new [] { "http://my.oschina.net/flashsword/blog" })]
	public class OschinaBlog
	{
		[ExtractBy(Value = "//title/text()")]
		public string Title { get; set; }

		[ExtractBy(Value = "div.BlogContent", Type = ExtractBy.ExtracType.Css)]
		public string Content { get; set; }

		[ExtractBy(Value = "//div[@class='BlogTags']/a/text()", Multi = true)]
		public List<string> Tags { get; set; }

		[ExtractBy(Value = "//div[@class='BlogStat']/regex('\\d+-\\d+-\\d+\\s+\\d+:\\d+')")]
		public DateTime Date { get; set; }
	}

	[TestClass]
	public class OschinaBlogTest
	{
		[TestMethod]
		public void TestOschinaBlog()
		{
			//results will be saved to "/data/webmagic/" in json format
			OoSpider.Create(new Site(), new JsonFilePageModelPipeline("/data/webmagic/"), typeof(OschinaBlog)).AddUrl("http://my.oschina.net/flashsword/blog").Run();
		}
	}
}