using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;

namespace Java2Dotnet.Spider.Samples.Model
{
	[TargetUrl(Value = new[] { "http://*.iteye.com/blog/*" })]
	public class IteyeBlog
	{
		[ExtractBy(Value = "//title")]
		public string Title { get; set; }

		[ExtractBy(Value = "div#blog_content", Type = ExtractBy.ExtracType.Css)]
		public string Content { get; set; }

		public static void Run()
		{
			var site = new Site();
			site.AddStartUrl("http://flashsword20.iteye.com/blog");
			OoSpider.Create(site, typeof(IteyeBlog)).Run();
		}
	}
}
