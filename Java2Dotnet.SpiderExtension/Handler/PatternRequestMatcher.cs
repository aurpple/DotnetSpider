using System.Text.RegularExpressions;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Handler
{
	/// <summary>
	/// A PatternHandler is in charge of both page extraction and data processing by implementing
	/// its two abstract methods.
	/// </summary>
	public abstract class PatternRequestMatcher : IRequestMatcher
	{
		/// <summary>
		/// match pattern. only matched page should be handled.
		/// </summary>
		protected string Pattern;
		private readonly Regex _patternCompiled;

		protected PatternRequestMatcher(string pattern)
		{
			Pattern = pattern;
			_patternCompiled = new Regex(pattern);
		}

		public bool Match(Request request)
		{
			return _patternCompiled.IsMatch(request.Url);
		}
	}
}