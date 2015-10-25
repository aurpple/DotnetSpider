using System;

namespace Java2Dotnet.Spider.Extension.Model.Attribute
{
	/// <summary>
	/// Define the 'help' url patterns for class.
	/// All urls matching the pattern will be crawled and but not extracted for new objects.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class HelpUrl : System.Attribute
	{
		/// <summary>
		/// The url patterns to crawl.
		/// Use regex expression with some changes:
		///      "." stand for literal character "." instead of "any character".
		///      "*" stand for any legal character for url in 0-n length ([^"'#]*) instead of "any length".
		/// </summary>
		public string[] Value { get; set; }

		/// <summary>
		/// Define the region for url extracting.
		/// Only support XPath.
		/// When sourceRegion is set, the urls will be extracted only from the region instead of entire content.
		/// </summary>
		public string SourceRegion { get; set; }
	}
}