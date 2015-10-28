using System;
using System.Collections.Generic;
using System.Text;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension.Pipeline;
using Java2Dotnet.Spider.Extension.Scheduler;

namespace Java2Dotnet.Spider.Samples
{

	[ExtractBy(Value = "//*[@id=\"tab_top50\"]/div[1]/ul/li", Multi = true, Count = 15)]
	[Scheme("aiqiyi")]
	[StoredAs("movies")]
	public class walter : BaseEntity
	{
		public static void RunTask()
		{
			OoSpider ooSpider = OoSpider.Create("aiqiyi_movies_" + DateTime.Now.ToLocalTime().ToString(),
				new Site { SleepTime = 1000, Encoding = Encoding.UTF8 }, new PageModelToDbPipeline(), typeof(walter));
			ooSpider.SetEmptySleepTime(15000);
			ooSpider.SetThreadNum(10);
			ooSpider.SetScheduler(new RedisScheduler("localhost", null));
			ooSpider.AddUrl("http://top.iqiyi.com/dianshiju.html#");
			ooSpider.Run();
		}

		[StoredAs("rank", StoredAs.ValueType.Varchar, false, 20)]
		[ExtractBy(Value = "li/em")]
		public string Rank { get; set; }

		[StoredAs("name", StoredAs.ValueType.Varchar, false, 20)]
		[ExtractBy(Value = "/li/a[1]/@title")]
		public string Name { get; set; }

		[StoredAs("url", StoredAs.ValueType.Varchar, false, 100)]
		[ExtractBy(Value = "/li/a[1]/@href")]
		public string Url { get; set; }

		[ExtractBy(Value = "/li/span[1]/a", Multi = true)]
		public List<string> Label_1 { get; set; }

		[StoredAs("tag", StoredAs.ValueType.Varchar, false, 500)]
		public string Tag
		{
			get
			{
				return string.Join("|", Label_1);
			}
		}
		//[StoredAs("label_2", StoredAs.ValueType.Varchar, false, 20)]
		//[ExtractBy(Value = "/li/span[1]/a[2]")]
		//public string Label_2 { get; set; }

		//[StoredAs("label_3", StoredAs.ValueType.Varchar, false, 20)]
		//[ExtractBy(Value = "/li/span[1]/a[3]")]
		//public string Label_3 { get; set; }

		[StoredAs("uuid", StoredAs.ValueType.Varchar, false, 20)]
		public string Uuid => Encrypt.Md5Encrypt(Name + Url);

		[StoredAs("cdate", StoredAs.ValueType.Date)]
		public DateTime CDate => DateTime.Now;
	}
}
