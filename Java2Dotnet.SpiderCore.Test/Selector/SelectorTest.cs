using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Selector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Selector
{
	[TestClass]
	public class SelectorTest
	{
		private string _html = "<div><a href='http://whatever.com/aaa'></a></div><div><a href='http://whatever.com/bbb'></a></div>";

		[TestMethod]
		public void TestChain()
		{
			Html selectable = new Html(_html,"");
			IList<string> linksWithoutChain = selectable.Links().GetAll();
			ISelectable xpath = selectable.XPath("//div");
			IList<string> linksWithChainFirstCall = xpath.Links().GetAll();
			IList<string> linksWithChainSecondCall = xpath.Links().GetAll();
			Assert.AreEqual(linksWithoutChain.Count, linksWithChainFirstCall.Count);
			Assert.AreEqual(linksWithChainFirstCall.Count, linksWithChainSecondCall.Count);
		}

		[TestMethod]
		public void TestNodes()
		{
			Html selectable = new Html(_html,"");
			IList<ISelectable> links = selectable.XPath("//a").Nodes();
			Assert.AreEqual(links[0].Links().Value, "http://whatever.com/aaa");
		}
	}
}
