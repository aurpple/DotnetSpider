using System.IO;
using System.Text;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Downloader;
using Java2Dotnet.Spider.Core.Pipeline;
using Java2Dotnet.Spider.Core.Processor;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Core.Utils;
using log4net;

namespace Java2Dotnet.Spider.Extension.Downloader
{
	/// <summary>
	/// Download file and saved to file for cache.
	/// </summary>
	[Experimental]
	public class FileCache : FilePersistentBase, IDownloader, IPipeline, IPageProcessor
	{
		private IDownloader _downloaderWhenFileMiss;
		private readonly IPageProcessor _pageProcessor;

		public FileCache(string startUrl, string urlPattern)
			: this(startUrl, urlPattern, "/data/dotnetspider/temp/")
		{
		}

		public FileCache(string startUrl, string urlPattern, string path)
		{
			_pageProcessor = new SimplePageProcessor(startUrl, urlPattern);
			SetPath(path);
			_downloaderWhenFileMiss = new HttpClientDownloader();
		}

		public FileCache SetDownloaderWhenFileMiss(IDownloader downloaderWhenFileMiss)
		{
			_downloaderWhenFileMiss = downloaderWhenFileMiss;
			return this;
		}

		public Page Download(Request request, ITask task)
		{
			// ReSharper disable once UnusedVariable
			string path = BasePath + "/" + task.Identify + "/";
			Page page;
			try
			{
				FileInfo file = PrepareFile(path + Encrypt.Md5Encrypt(request.Url));

				StreamReader bufferedReader = new StreamReader(file.OpenRead());
				string line = bufferedReader.ReadLine();
				if (("url:\t" + request.Url).Equals(line))
				{
					string html = GetHtml(bufferedReader);
					page = new Page(request);
					page.SetUrl(PlainText.Create(request.Url));
					page.SetHtml(Html.Create(html));
				}
			}
			catch (IOException e)
			{
				if (e.GetType().IsInstanceOfType(typeof(FileNotFoundException)))
				{
					Logger.Info("File not exist for url " + request.Url);
				}
				else
				{
					Logger.Warn("File read error for url " + request.Url, e);
				}
			}
			page = DownloadWhenMiss(request, task);
			return page;
		}

		public void SetThreadNum(int threadNum)
		{
		}

		private string GetHtml(StreamReader bufferedReader)
		{
			StringBuilder htmlBuilder = new StringBuilder();
			var line = bufferedReader.ReadLine();
			//check
			if (line != null)
			{
				line = line.Replace("html:\t", "");
				htmlBuilder.Append(line);
				while ((line = bufferedReader.ReadLine()) != null)
				{
					htmlBuilder.Append(line);
				}
			}
			return htmlBuilder.ToString();
		}

		private Page DownloadWhenMiss(Request request, ITask task)
		{
			Page page = null;
			if (_downloaderWhenFileMiss != null)
			{
				page = _downloaderWhenFileMiss.Download(request, task);
			}
			return page;
		}

		public void Process(ResultItems resultItems, ITask task)
		{
			string path = BasePath + PathSeperator + task.Identify + PathSeperator;
			try
			{
				FileInfo fileInfo = PrepareFile(path + Encrypt.Md5Encrypt(resultItems.Request.Url) + ".html");
				using (StreamWriter writer = new StreamWriter(fileInfo.OpenWrite(), Encoding.UTF8))
				{
					writer.WriteLine("url:\t" + resultItems.Request.Url);
					writer.WriteLine("html:\t" + resultItems.Get("html"));
				}
			}
			catch (IOException e)
			{
				Logger.Warn("write file error", e);
			}
		}

		public void Process(Page page)
		{
			_pageProcessor.Process(page);
		}

		public Site Site => _pageProcessor.Site;
	}
}