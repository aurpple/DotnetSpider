using Java2Dotnet.Spider.Scripts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Scripts
{
	[TestClass]
	public class JavaScriptTest
	{
		[TestMethod]
		public void TestJavaScriptTask()
		{
			ScriptProcessor pageProcessor = ScriptProcessorBuilder.Custom().Language(Language.Javascript).ScriptFromClassPathFile("Java2Dotnet.Spider.Scripts.Resource.js.youkuvideo.js").Build();
			pageProcessor.Site.SleepTime = 500;
			Spider.Create(pageProcessor).AddUrl("http://my.oschina.net/flashsword/blog").SetSpawnUrl(false).Run();
		}

		[TestMethod]
		public void TestJavaConsoleCommand()
		{
			ScriptConsole.Main(new string[] { "-l", "JAVASCRIPT", "-t", "2", "-f", "c:\\", "-s", "500" });
		}
	}
}
