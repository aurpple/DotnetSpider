using System.Collections.Generic;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Pipeline;
using Java2Dotnet.Spider.Extension;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension.Pipeline;
using Java2Dotnet.Spider.Extension.Scheduler;

namespace Java2Dotnet.Spider.Samples.Model
{
	[TargetUrl(Value = new[] { "http://news.163.com/\\d+/\\d+/\\d+/\\w+*.html" })]
	public class News163 : IMultiPageModel
	{
		[ExtractByUrl("http://news\\.163\\.com/\\d+/\\d+/\\d+/([^_]*).*\\.html")]
		public string PageKey { get; set; }

		[ExtractByUrl(Value = "http://news\\.163\\.com/\\d+/\\d+/\\d+/\\w+_(\\d+)\\.html", NotNull = false)]
		public string Page { get; set; }

		////div[@class=\"ep-pages\"]/a[substring(@href,string-length(@href)-5)='.html']

		[ExtractBy(Value = "//div[@class=\"ep-pages\"]/a/@href", Multi = true, NotNull = false, Expresion = "regex('http://news\\.163\\.com/\\d+/\\d+/\\d+/\\w+_(\\d+)\\.html',1)")]
		public HashSet<string> OtherPage { get; set; }

		[ExtractBy(Value = "//h1[@id=\"h1title\"]/text()")]
		public string Title { get; set; }

		[ExtractBy(Value = "//div[@id=\"epContentLeft\"]")]
		public string Content { get; set; }

		public string GetPageKey()
		{
			return PageKey;
		}

		public string GetPage()
		{
			if (Page == null)
			{
				return "1";
			}
			return Page;
		}

		public ICollection<string> GetOtherPages()
		{
			return OtherPage;
		}

		public IMultiPageModel Combine(IMultiPageModel multiPageModel)
		{
			News163 news163 = new News163 { Title = Title };
			News163 pagedModel1 = (News163)multiPageModel;
			news163.Content = Content + pagedModel1.Content;
			return news163;
		}

		public override string ToString()
		{
			return "News163{" +
					"content='" + Content + '\'' +
					", title='" + Title + '\'' +
					", otherPage=" + OtherPage +
					'}';
		}

		public static void Run()
		{
			OoSpider.Create(new Site(), typeof(News163)).AddUrl("http://news.163.com/13/0802/05/958I1E330001124J_2.html")
				.AddPipeline(new MultiPagePipeline())
				.AddPipeline(new ConsolePipeline())
				.SetScheduler(new RedisScheduler("localhost", "")).Run();
		}
	}
}
