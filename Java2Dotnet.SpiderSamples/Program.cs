using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Extension.DbSupport;
using Java2Dotnet.Spider.Samples.Model;
using Java2Dotnet.Spider.Samples.Samples;

namespace Java2Dotnet.Spider.Samples
{
	class Program
	{
		private static Dictionary<String, Type> _typesMap;

		private static Dictionary<String, String> _urlMap;

		private static void Init()
		{
			_typesMap = new Dictionary<string, Type>
			{
				{"1", typeof (OschinaBlog)},
				{"2", typeof (IteyeBlog)},
				{"3", typeof (News163)}
			};
			_urlMap = new Dictionary<String, String>
			{
				{"1", "http://my.oschina.net/flashsword/blog"},
				{"2", "http://flashsword20.iteye.com/"},
				{"3", "http://news.163.com/"}
			};
		}

		private static string ReadKey(String key)
		{
			foreach (var classEntry in _typesMap)
			{
				Console.WriteLine(classEntry.Key + "\t" + classEntry.Value.Name + "\t" + _urlMap[classEntry.Key]);
			}
			while (key == null)
			{
				key = Console.ReadLine();
				if (string.IsNullOrEmpty(key))
				{
					continue;
				}
				if (!_typesMap.ContainsKey(key))
				{
					Console.WriteLine("Invalid choice!");
					key = null;
				}
			}
			return key;
		}

		static void Main(string[] args)
		{
			// 必须指定Provider
			DbProviderUtil.Provider = new DataProviderManager().LoadDataProvider();

			//SingleSample.RunTask();
            
			Ganji.RunTask();
			//News163.Run();
			//OschinaAnswer.Run();

			//// QQ 美食已经不提供服务了
			////QqMeishi.Run();

			//OschinaBlog.Run();


			//IteyeBlog.Run();

			//Init();

			//String key = null;
			//key = ReadKey(key);
			//Console.WriteLine("The demo started and will last 20 seconds...");
			////Start spider
			//Site site = new Site { Encoding = Encoding.UTF8 };
			//site.AddStartUrl(_urlMap[key]);
			//OoSpider.Create(site, _typesMap[key]).AddPipeline(new MultiPagePipeline()).AddPipeline(new ConsolePipeline()).RunAsync();

			//try
			//{
			//	Thread.Sleep(200000);
			//}
			//catch (Exception e)
			//{
			//	Console.WriteLine(e.ToString());
			//}
			//Console.WriteLine("The demo stopped!");
			//Console.WriteLine("To more usage, try to customize your own Spider!");
		}
	}
}
