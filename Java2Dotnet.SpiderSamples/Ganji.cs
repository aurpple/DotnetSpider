using System;
using System.Text;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension.Pipeline;
using Java2Dotnet.Spider.Extension.Scheduler;

namespace Java2Dotnet.Spider.Samples
{
	////*[@id="list-job-id"]/div[15]/ul
	[TargetUrl(Value = new[] { "zpdianhuaxiaoshou/o[0-9]+/" }, SourceRegion = "//*[@id=\"list-job-id\"]/div[15]/ul")]

	//////*[@id="list-job-id"]/div[9]/dl[1]
	[ExtractBy(Value = "//*[@id=\"list-job-id\"]/div[9]/dl", Multi = true)]
	[Scheme("ganji")]
	[StoredAs("post")]
	public class Ganji : SpiderEntity
	{
		public static void RunTask()
		{
			OoSpider ooSpider = OoSpider.Create("ganji_posts_" + DateTime.Now.Date.ToString("yyyy-MM-dd"),
				new Site { SleepTime = 1000, Encoding = Encoding.UTF8 },
				new PageModelToDbPipeline(), typeof(Ganji));
			ooSpider.SetEmptySleepTime(15000);
			ooSpider.SetThreadNum(1);
			ooSpider.ModelPipeline.CachedSize = 2;
			ooSpider.SetScheduler(new QueueScheduler());
			ooSpider.AddUrl("http://sh.ganji.com/zpdianhuaxiaoshou/o1/");
			ooSpider.Run();
		}

		[StoredAs("title", StoredAs.ValueType.Varchar, false, 100)]
		////*[@id="list-job-id"]/div[9]/dl[1]/dt/a
		[ExtractBy(Value = "/dl/dt/a")]
		public string Title { get; set; }

		[StoredAs("company", StoredAs.ValueType.Varchar, false, 100)]
		[ExtractBy(Value = "/dl/dd[1]/a")]
		public string Company { get; set; }

		[StoredAs("ishr", StoredAs.ValueType.Varchar, false, 5)]
		[ExtractBy(Value = "/dl/dd[1]/span/@class")]
		public string IsHr { get; set; }

		[StoredAs("bangbang", StoredAs.ValueType.Varchar, false, 20)]
		[ExtractBy(Value = "/dl/dd[1]/span")]
		public string BangBang { get; set; }

		[StoredAs("third", StoredAs.ValueType.Varchar, false, 20)]
		[ExtractBy(Value = "/dl/dd[1]/i/@title")]
		public string Third { get; set; }

		[StoredAs("corpmail", StoredAs.ValueType.Varchar, false, 20)]
		[ExtractBy(Value = "/dl/dd[1]/span[1]/@title")]
		public string CorpMail { get; set; }

		[StoredAs("uuid", StoredAs.ValueType.Varchar, false, 20)]
		public string Uuid => Encrypt.Md5Encrypt(Title + Company);

		[ExtractBy(Value = "url", Type = ExtractBy.ExtracType.Enviroment)]
		[StoredAs("url", StoredAs.ValueType.Text)]
		public string Url { get; set; }

		[StoredAs("cdate", StoredAs.ValueType.Date)]
		public DateTime CDate => DateTime.Now;
	}
}
