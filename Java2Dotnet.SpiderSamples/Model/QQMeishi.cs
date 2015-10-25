using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension.Pipeline;

namespace Java2Dotnet.Spider.Samples.Model
{
	[TargetUrl(Value = new[] { "http://meishi.qq.com/beijing/c/all[\\-p2]*" })]
	[ExtractBy(Value = "//ul[@id=\"promos_list2\"]/li", Multi = true)]
	public class QqMeishi
	{
		[ExtractBy(Value = "//div[@class=info]/a[@class=title]/h4/text()")]
		private string ShopName { get; set; }

		[ExtractBy(Value = "//div[@class=info]/a[@class=title]/text()")]
		private string Promo { get; set; }

		public static void Run()
		{
			OoSpider.Create(new Site(), new ConsolePageModelPipeline(), typeof(QqMeishi)).AddUrl("http://meishi.qq.com/beijing/c/all").SetThreadNum(4).Run();
		}
	}
}
