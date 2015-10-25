using Java2Dotnet.Spider.Core.Selector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Selector
{
	[TestClass]
	public class ExtractorsTest
	{
		string _html = "<div><h1>test<a href=\"xxx\">aabbcc</a></h1></div>";
		string _html2 = "<title>aabbcc</title>";

		[TestMethod]
		public void TestEach()
		{
			Assert.AreEqual(Selectors.Css("div h1 a").Select(_html), "<a href=\"xxx\">aabbcc</a>");
			Assert.AreEqual(Selectors.Css("div h1 a", "href").Select(_html), "xxx");
			Assert.AreEqual(Selectors.Css("div h1 a", "innerHtml").Select(_html), "aabbcc");
			Assert.AreEqual(Selectors.XPath("//a/@href").Select(_html), "xxx");
			Assert.AreEqual(Selectors.Regex("a href=\"(.*)\"").Select(_html), "xxx");
			Assert.AreEqual(Selectors.Regex("(a href)=\"(.*)\"", 2).Select(_html), "xxx");
		}

		[TestMethod]
		public void TestCombo()
		{
			var value1 = Selectors.And(Selectors.Css("title"), Selectors.Regex("aa(bb)cc")).Select(_html2);
			Assert.AreEqual(value1, "bb");

			var or = Selectors.Or(Selectors.Css("div h1 a", "innerHtml"), Selectors.XPath("//title"));
			Assert.AreEqual(or.Select(_html), "aabbcc");
			Assert.AreEqual(or.Select(_html2), "aabbcc");
		}
	}
}
