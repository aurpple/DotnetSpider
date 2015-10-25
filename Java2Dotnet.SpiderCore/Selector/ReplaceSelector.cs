using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Java2Dotnet.Spider.Core.Selector
{
	/// <summary>
	/// Replace selector.
	/// </summary>
	public class ReplaceSelector : ISelector
	{
		private readonly string _regexStr;
		private readonly string _replacement;

		public ReplaceSelector(string regexStr, string replacement)
		{
			_regexStr = regexStr;
			_replacement = replacement;
		}

		public string Select(string text)
		{
			return Regex.Replace(text, _regexStr, _replacement);
		}

		public IList<string> SelectList(string text)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return _regexStr + "_" + _replacement;
		}
	}
}