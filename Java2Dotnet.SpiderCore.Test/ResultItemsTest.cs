using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test
{
	[TestClass]
	public class ResultItemsTest
	{
		[TestMethod]
		public void TestOrderOfEntries()
		{
			ResultItems resultItems = new ResultItems();
			resultItems.Put("a", "a");
			resultItems.Put("b", "b");
			resultItems.Put("c", "c");

			dynamic a = resultItems.Get("a");
			dynamic b = resultItems.Get("b");
			dynamic c = resultItems.Get("c");
			//Assert.AreEqual(a, "a");
			//Assert.AreEqual(b, "b");
			//Assert.AreEqual(c, "c");
		}
	}
}
