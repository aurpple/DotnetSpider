using Java2Dotnet.Spider.Core.Selector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test
{
	[TestClass]
	public class HtmlTest
	{
		[TestMethod]
		public void TestRegexSelector()
		{
			Html selectable = new Html("aaaaaaab","");
			//        Assert.assertEquals("abbabbab", (selectable.regex("(.*)").replace("aa(a)", "$1bb").toString()));
			string value = selectable.Regex("(.*)").Replace("aa(a)", "$1bb").ToString();
			Assert.AreEqual("abbabbab", value);

		}
	}
}
