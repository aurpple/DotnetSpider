namespace Java2Dotnet.Spider.Core.Selector
{
	public class RegexResult
	{
		private readonly string[] _groups;
		public static RegexResult EmptyResult = new RegexResult();

		private RegexResult()
		{
		}

		public RegexResult(string[] groups)
		{
			_groups = groups;
		}

		public string Get(int groupId)
		{
			return _groups?[groupId];
		}
	}
}
