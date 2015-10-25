namespace Java2Dotnet.Spider.Core.Selector
{
	/// <summary>
	/// Convenient methods for selectors.
	/// </summary>
	public class Selectors
	{
		public static RegexSelector Regex(string expr)
		{
			return new RegexSelector(expr);
		}

		public static CssSelector Css(string expr)
		{
			return new CssSelector(expr);
		}

		public static CssSelector Css(string expr, string attrName)
		{
			return new CssSelector(expr, attrName);
		}

		public static RegexSelector Regex(string expr, int group)
		{
			return new RegexSelector(expr, group);
		}

		public static SmartContentSelector SmartContent()
		{
			return new SmartContentSelector();
		}

		public static XPathSelector XPath(string expr)
		{
			return new XPathSelector(expr);
		}

		/// <summary>
		/// @Deprecated
		/// @see #xpath(string)
		/// </summary>
		/// <param name="expr"></param>
		/// <returns></returns>
		public static XPathSelector XSoup(string expr)
		{
			return new XPathSelector(expr);
		}

		public static AndSelector And(params ISelector[] selectors)
		{
			return new AndSelector(selectors);
		}

		public static OrSelector Or(params ISelector[] selectors)
		{
			return new OrSelector(selectors);
		}
	}
}