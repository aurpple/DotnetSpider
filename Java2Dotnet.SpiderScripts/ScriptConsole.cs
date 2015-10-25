using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommandLine;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Scripts
{
	public class ScriptConsole
	{
		//private class Param
		//{
		//	Language language = Language.JAVASCRIPT;
		//	string scriptFileName;
		//	List<string> urls;
		//	int thread = 1;
		//	int sleepTime = 1000;
		//	private static Dictionary<Language, List<string>> alias = new Dictionary<Language, List<string>>();

		//	static Param()
		//	{
		//		alias.Add(Language.JAVASCRIPT, new List<string>() { "js", "javascript", "JavaScript", "JS" });
		//		//alias.Add(Language.JRuby, Sets.<string>newHashSet("ruby", "jruby", "Ruby", "JRuby"));
		//	}

		//	public void SetLanguagefromArg(string arg)
		//	{
		//		foreach (KeyValuePair<Language, List<string>> languageSetEntry in alias)
		//		{
		//			if (languageSetEntry.Value.Contains(arg))
		//			{
		//				this.language = languageSetEntry.Key;
		//				return;
		//			}
		//		}
		//	}

		//	public Language Lang
		//	{
		//		get { return language; }
		//		set
		//		{
		//			if (this.language != value)
		//			{
		//				this.language = value;
		//			}
		//		}
		//	}

		//	public string ScriptFileName
		//	{
		//		get { return scriptFileName; }
		//		set
		//		{
		//			if (this.scriptFileName != value)
		//			{
		//				this.scriptFileName = value;
		//			}
		//		}
		//	}

		//	public List<string> Urls
		//	{
		//		get { return urls; }
		//		set
		//		{
		//			if (this.urls != value)
		//			{
		//				this.urls = value;
		//			}
		//		}
		//	}

		//	public int Thread
		//	{
		//		get { return thread; }
		//		set
		//		{
		//			if (this.thread != value)
		//			{
		//				this.thread = value;
		//			}
		//		}
		//	}

		//	public int SleepTime
		//	{
		//		get { return sleepTime; }
		//		set
		//		{
		//			if (this.sleepTime != value)
		//			{
		//				this.sleepTime = value;
		//			}
		//		}
		//	}
		//}

		public static void Main(string[] args)
		{
			Options param = ParseCommand(args);
			if (param != null)
			{
				StartSpider(param);
			}
		}

		private static void StartSpider(Options param)
		{
			ScriptProcessor pageProcessor = ScriptProcessorBuilder.Custom().Language(param.Lang).ScriptFromFile(param.File).Thread(param.Thread).Build();
			pageProcessor.Site.SleepTime = param.Sleep;
			pageProcessor.Site.RetryTimes = 3;
			pageProcessor.Site.AcceptStatCode=new HashSet<int> { 200, 404, 403, 500, 502 };
			Core.Spider spider = Core.Spider.Create(pageProcessor).SetThreadNum(param.Thread);
			spider.ClearPipeline();

			StringBuilder builder = new StringBuilder();
			using (StreamReader sr = new StreamReader(typeof(ScriptConsole).Assembly.GetManifestResourceStream("Java2Dotnet.Spider.Scripts.Resource.js.define.js")))
			{
				string line;

				while ((line = sr.ReadLine()) != null)
				{
					builder.AppendLine(line);
				}
			}

			string script = builder + Environment.NewLine + File.ReadAllText(param.File);

			Jurassic.ScriptEngine engine = new Jurassic.ScriptEngine { EnableExposedClrTypes = true };
			//engine.SetGlobalValue("page", new Page());
			engine.SetGlobalValue("config", new Site());

			engine.Evaluate(script);

			foreach (string url in param.Urls)
			{
				spider.AddUrl(url);
			}
			spider.Run();
		}

		internal class Options
		{
			[Option('l', "language", Required = false, HelpText = "language")]
			public Language Lang { get; set; }

			[Option('t', "thread", Required = false, HelpText = "thread")]
			public int Thread { get; set; }

			[Option('f', "file", Required = true, HelpText = "script file")]
			public string File { get; set; }

			[Option('i', "input", Required = false, HelpText = "input file")]
			public string Input { get; set; }
			[Option('s', "sleep", Required = false, HelpText = "sleep time")]
			public int Sleep { get; set; }

			[Option('u', "url", Required = false, HelpText = "start urls")]
			public IEnumerable<string> Urls { get; set; }

			public Options()
			{
				Lang = Language.Javascript;
				Thread = 1;
				Sleep = 1000;
			}
		}

		private static Options ParseCommand(string[] args)
		{
			try
			{
				var result = Parser.Default.ParseArguments<Options>(new List<string>(args));
				var par = result
					.MapResult(options => options,
					errors =>
					{
						foreach (var error in errors)
						{
							NamedError named = error as NamedError;
							Console.WriteLine(named + ":" + error.ToString());
						}
						return null;
					});
				return par;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}