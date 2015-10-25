using System;

namespace Java2Dotnet.Spider.Core.Utils
{
	public class StringUtils
	{
		public static int CountMatches(string str, string sub)
		{
			if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(sub))
			{
				int count = 0;

				for (int idx = 0; (idx = str.IndexOf(sub, idx, StringComparison.Ordinal)) != -1; idx += sub.Length)
				{
					++count;
				}

				return count;
			}
			return 0;
		}
	}
}
