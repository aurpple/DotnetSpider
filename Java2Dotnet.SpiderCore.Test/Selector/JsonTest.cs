using Java2Dotnet.Spider.Core.Selector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Selector
{
	[TestClass]
	public class JsonTest
	{
		private string _text = "callback({\"name\":\"json\"});";

		private string _textWithBrackerInContent = "callback({\"name\":\"json)\"})";

		[TestMethod]
		public void TestRemovePadding()
		{
			string name = new Json(_text).RemovePadding("callback").JsonPath("$.name").Value;
			Assert.AreEqual(name, "json");
		}

		[TestMethod]
		public void TestRemovePaddingForQuotes()
		{
			string name = new Json(_textWithBrackerInContent).RemovePadding("callback").JsonPath("$.name").Value;
			Assert.AreEqual(name, "json)");
		}
	}
}
