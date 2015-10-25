using System;
using Java2Dotnet.Spider.Core.Selector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Selector
{
	[TestClass]
	public class RegexSelectorTest
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TestRegexWithSingleLeftBracket()
		{
			string regex = "\\d+(";
			new RegexSelector(regex);
		}

		[TestMethod]
		public void TestRegexWithLeftBracketQuoted()
		{
			string regex = "\\(.+";
			string source = "(hello world";
			RegexSelector regexSelector = new RegexSelector(regex);
			string select = regexSelector.Select(source);
			Assert.AreEqual(select, source);
		}
	}
}
