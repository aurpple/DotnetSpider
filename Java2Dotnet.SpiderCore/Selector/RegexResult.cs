namespace Java2Dotnet.Spider.Core.Selector
{
	public class RegexResult
	{
		private readonly string[] _groups;
		public static RegexResult EmptyResult = new RegexResult();

		public RegexResult()
		{
		}

		public RegexResult(string[] groups)
		{
			_groups = groups;
		}

		public string Get(int groupId)
		{
			// 如果匹配出错最好返回异常, 不然不知道这边正则匹配出错了.
			//if (_groups.Length > groupId)
			//{
			return _groups?[groupId];
			//}

			//return null;
		}
	}
}
