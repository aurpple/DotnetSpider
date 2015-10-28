﻿using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Pipeline;

namespace Java2Dotnet.Spider.Samples
{
	class GanjiPostCount
	{
		public static void RunTask()
		{
			OoSpider ooSpider = OoSpider.Create(new Site
			{
				SleepTime = 10000
			}, new PageModelToDbPipeline(), typeof(Ganji));
			ooSpider.SetThreadNum(1);
			Request request = new Request("http://mobds.ganji.com/datashare/", 1, null);
			request.Method = "POST";
			ooSpider.AddRequest(request);
			ooSpider.Run();
		}
	}
}
