namespace Java2Dotnet.Spider.Scripts
{
	public class Language
	{
		public static Language Javascript = new Language("javascript", "Java2Dotnet.Spider.Scripts.Resource.js.define.js", "");
		public static Language Ruby = new Language("ruby", "js/ruby/defines.rb", "");
		public static Language Python = new Language("python", "python/defines.py", "");

		private readonly string _engineName;
		private readonly string _defineFile;
		private readonly string _gatherFile;

		public Language(string engineName, string defineFile, string gatherFile)
		{
			_engineName = engineName;
			_defineFile = defineFile;
			_gatherFile = gatherFile;
		}

		public string GetEngineName()
		{
			return _engineName;
		}

		public string GetDefineFile()
		{
			return _defineFile;
		}

		public string GetGatherFile()
		{
			return _gatherFile;
		}
	}
}
